/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using VPL;
using LimnorDesigner.Web;

namespace LimnorDesigner
{
	public class ComboBoxClassPointer : ComboBox
	{
		private ClassPointer _root;
		private bool _selecting;
		public event EventHandler SelectedObject;
		public ComboBoxClassPointer()
		{
		}
		public void LoadClassPointer(ClassPointer root)
		{
			if (root != _root)
			{
				_root = root;
				ReloadClassPointer();
			}
		}
		public void ReloadClassPointer()
		{
			this.Items.Clear();
			if (_root != null)
			{
				ComponentDataDisplay s = getComponentDisplay(_root.ObjectInstance as IComponent, 1);
				if (s != null)
				{
					this.Items.Add(new ComponentDataDisplay(null, string.Empty, 0, string.Empty));
					this.Items.Add(s);
					if (_root.ObjectList != null)
					{
						foreach (KeyValuePair<object, UInt32> kv in _root.ObjectList)
						{
							if (kv.Key != _root.ObjectInstance)
							{
								s = getComponentDisplay(kv.Key as IComponent, kv.Value);
								if (s != null)
								{
									this.Items.Add(s);
								}
							}
						}
					}
					if (_root.IsWebPage)
					{
						IList<HtmlElement_BodyBase> lst = _root.UsedHtmlElements;
						if (lst != null && lst.Count > 0)
						{
							for (int i = 0; i < lst.Count; i++)
							{
								this.Items.Add(lst[i]);
							}
						}
					}
					_selecting = true;
					this.SelectedIndex = 1;
					_selecting = false;
				}
			}
		}
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (!_selecting)
			{
				if (SelectedObject != null)
				{
					if (this.SelectedIndex > 0)
					{
						ComponentDataDisplay s = this.Items[this.SelectedIndex] as ComponentDataDisplay;
						if (s != null)
						{
							EventArgsObjectSelection ev = new EventArgsObjectSelection(s);
							SelectedObject(this, ev);
						}
						else
						{
							HtmlElement_BodyBase heb = this.Items[this.SelectedIndex] as HtmlElement_BodyBase;
							if (heb != null)
							{
								EventArgsHtmlElementSelection eh = new EventArgsHtmlElementSelection(heb);
								SelectedObject(this, eh);
							}
						}
					}
				}
			}
		}
		public void SelectByName(string name)
		{
			bool found = false;
			if (!string.IsNullOrEmpty(name))
			{
				for (int i = 0; i < this.Items.Count; i++)
				{
					ComponentDataDisplay s = this.Items[i] as ComponentDataDisplay;
					if (s != null)
					{
						if (string.CompareOrdinal(s.Name, name) == 0)
						{
							found = true;
							_selecting = true;
							this.SelectedIndex = i;
							_selecting = false;
							break;
						}
					}
				}
			}
			if (!found)
			{
				this.SelectedIndex = 0;
			}
		}
		public void SelectHtmlElement(HtmlElement_BodyBase element)
		{
			bool found = false;
			if (this.Items.Count > 0)
			{
				for (int i = 0; i < this.Items.Count; i++)
				{
					HtmlElement_BodyBase heb = this.Items[i] as HtmlElement_BodyBase;
					if (heb != null)
					{
						if (heb.IsSameElement(element))
						{
							found = true;
							_selecting = true;
							this.SelectedIndex = i;
							_selecting = false;
							break;
						}
					}
				}
				if (!found)
				{
					this.SelectedIndex = 0;
				}
			}
		}
		private ComponentDataDisplay getComponentDisplay(IComponent obj, UInt32 id)
		{
			if (obj != null)
			{
				Type t = VPLUtil.GetObjectType(obj);
				if (obj.Site != null && !string.IsNullOrEmpty(obj.Site.Name))
				{
					return new ComponentDataDisplay(obj, string.Format(CultureInfo.InvariantCulture, "{0}({1})", obj.Site.Name, t.FullName), id, obj.Site.Name);
				}
			}
			return null;
		}

	}
	public class ComponentDataDisplay
	{
		IComponent _obj;
		private string _display;
		private string _name;
		private UInt32 _id;
		public ComponentDataDisplay(IComponent obj, string display, UInt32 id, string name)
		{
			_obj = obj;
			_display = display;
			_id = id;
			_name = name;
		}
		public IComponent Component
		{
			get
			{
				return _obj;
			}
		}
		public string Display
		{
			get
			{
				return _display;
			}
		}
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public UInt32 ID
		{
			get
			{
				return _id;
			}
		}
		public override string ToString()
		{
			return _display;
		}
	}
	public class EventArgsObjectSelection : EventArgs
	{
		private ComponentDataDisplay _obj;
		public EventArgsObjectSelection(ComponentDataDisplay obj)
		{
			_obj = obj;
		}
		public ComponentDataDisplay ObjectDisplay
		{
			get
			{
				return _obj;
			}
		}
	}
	public class EventArgsHtmlElementSelection : EventArgs
	{
		private HtmlElement_BodyBase _obj;
		public EventArgsHtmlElementSelection(HtmlElement_BodyBase obj)
		{
			_obj = obj;
		}
		public HtmlElement_BodyBase ObjectDisplay
		{
			get
			{
				return _obj;
			}
		}
	}
}
