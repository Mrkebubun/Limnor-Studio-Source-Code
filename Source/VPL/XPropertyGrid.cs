/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;

namespace VPL
{
	public class XPropertyGrid : PropertyGrid
	{
		private System.ComponentModel.Container components = null;
		public event EventHandler LeaveItems;
		public XPropertyGrid()
		{
			InitializeComponent();
			this.CommandsVisibleIfAvailable = true;
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
		private object _items;
		private object _component;
		private TextBox _editbox;
		protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
		{
			base.OnSelectedGridItemChanged(e);
			if (_editbox == null)
			{
				AddTextEditorFocusWatcher();
			}
			bool isItems = false;
			if (e == null) return;
			if (e.NewSelection == null) return;
			if (e.NewSelection.PropertyDescriptor == null) return;
			PropertyDescriptor pd = e.NewSelection.PropertyDescriptor;
			if (pd.PropertyType != null && pd.PropertyType.GetInterface("ICollection") != null)
			{
				FieldInfo[] fifs = pd.Attributes.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
				for (int i = 0; i < fifs.Length; i++)
				{
					if (string.CompareOrdinal(fifs[i].Name, "_attributes") == 0)
					{
						Attribute[] attrs = fifs[i].GetValue(pd.Attributes) as Attribute[];
						if (attrs != null)
						{
							for (int j = 0; j < attrs.Length; j++)
							{
								EditorAttribute ea = attrs[j] as EditorAttribute;
								if (ea != null)
								{
									try
									{
										Type t = Type.GetType(ea.EditorTypeName);
										if (t != null)
										{
											if (typeof(CollectionEditor).IsAssignableFrom(t))
											{
												isItems = true;
												break;
											}
										}
									}
									catch
									{
									}
								}
							}
						}
					}
				}
			}
			if (_items != null)
			{
				if (e.NewSelection.Value != _items)
				{
					if (LeaveItems != null)
					{
						LeaveItems(this, EventArgs.Empty);
					}
				}
			}
			if (isItems)
			{
				_items = e.NewSelection.Value;
				_component = this.SelectedObject;
			}
			else
			{
				_items = null;
			}
		}
		public void CheckItemsSelection(object currentObject)
		{
			if (_items != null)
			{
				if (currentObject == _component)
				{
					if (LeaveItems != null)
					{
						LeaveItems(this, EventArgs.Empty);
					}
					_items = null;
				}
			}
		}
		public void CheckItemsSelection()
		{
			if (_items != null)
			{
				if (LeaveItems != null)
				{
					LeaveItems(this, EventArgs.Empty);
				}
			}
		}
		public void ShowEvents(bool show)
		{
			ShowEventsButton(show);
		}
		public void AddTextEditorFocusWatcher()
		{
			Control pc = this as Control;
			foreach (Control c in pc.Controls)
			{
				if (string.CompareOrdinal(c.GetType().Name, "PropertyGridView") == 0)
				{
					foreach (Control ce in c.Controls)
					{
						if (string.CompareOrdinal(ce.GetType().Name, "GridViewEdit") == 0)
						{
							TextBox tx = ce as TextBox;
							tx.GotFocus += new EventHandler(tx_GotFocus);
							tx.LostFocus += new EventHandler(tx_LostFocus);
							_editbox = tx;
							break;
						}
					}
					if (_editbox != null)
					{
						break;
					}
				}
			}
		}
		public void TextEditor_DeleteOne()
		{
			if (_editbox != null)
			{
				int n = _editbox.SelectionStart;
				if (n >= 0 && n < _editbox.Text.Length)
				{
					string newtext;
					if (_editbox.SelectionLength > 0)
					{
						newtext = string.Format(CultureInfo.InvariantCulture, "{0}{1}", _editbox.Text.Substring(0, n), _editbox.Text.Substring(n + _editbox.SelectionLength));
					}
					else
					{
						newtext = string.Format(CultureInfo.InvariantCulture, "{0}{1}", _editbox.Text.Substring(0, n), _editbox.Text.Substring(n + 1));
					}
					_editbox.SelectionLength = 0;
					_editbox.Text = newtext;
					if (_editbox.Text.Length >= n)
					{
						_editbox.SelectionStart = n;
					}
					else
					{
						if (_editbox.Text.Length > 0)
						{
							_editbox.SelectionStart = _editbox.Text.Length - 1;
						}
						else
						{
						}
					}
					if (string.IsNullOrEmpty(newtext))
					{
						if (SelectedGridItem != null && SelectedGridItem.PropertyDescriptor != null)
						{
							ICustomPropertyReseter cpr = this.SelectedObject as ICustomPropertyReseter;
							if (cpr != null)
							{
								cpr.ResetPropertyValue(this.SelectedGridItem.PropertyDescriptor.Name, this.SelectedGridItem.PropertyDescriptor.PropertyType);
							}
						}
					}
				}
			}
		}
		void tx_LostFocus(object sender, EventArgs e)
		{
			IsInEditing = false;
		}

		void tx_GotFocus(object sender, EventArgs e)
		{
			IsInEditing = true;
		}
		public bool IsInEditing
		{
			get;
			set;
		}
		public bool DrawFlat
		{
			get { return DrawFlatToolbar; }
			set { DrawFlatToolbar = value; }
		}
	}

}
