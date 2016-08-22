/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;
using System.Globalization;
using System.Reflection;
using System.CodeDom;
using LimnorDatabase;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;

namespace Limnor.TreeViewExt
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TreeNodeXTemplate : TreeNodeX, IPropertyValueLinkHolder, ICustomPropertyNames, ICustomPropertyCompile
	{
		#region fields and constructors
		private PropertyValueLinks _links;
		private TreeViewX _owner;
		public TreeNodeXTemplate()
		{
			ChildLoadStyle = EnumChildLoad.None;
			_links = new PropertyValueLinks(this, "Text", "ImageIndex", "SelectedImageIndex", "ImageKey", "SelectedImageKey", "ToolTipText");
		}
		#endregion
		#region Properties
		[ParenthesizePropertyName(true)]
		[Category("Database")]
		[DefaultValue(EnumChildLoad.None)]
		[Description("Gets and sets a Boolean indicating whether the loading of the child nodes should be done using the ChildNodesQuery and ChildNodesTemplate of the parent of this node. If this property is True then ChildNodesQuery and ChildNodesTemplate of this node are ignored.")]
		public EnumChildLoad ChildLoadStyle
		{
			get;
			set;
		}
		[Category("Database")]
		[ParenthesizePropertyName(true)]
		[Editor(typeof(TypeEditorTurnOffRecursion), typeof(UITypeEditor))]
		[XmlIgnore]
		[Description("Gets and sets the database query for loading the child nodes, if ChildRecursion is False.")]
		public DataQuery ChildNodesQuery
		{
			get
			{
				if (ChildLoadStyle == EnumChildLoad.Flat)
				{
					if (_owner != null)
					{
						return _owner.CreateDataQuery(this.Level + 1);
					}
				}
				return null;
			}
			set
			{
				if (ChildLoadStyle == EnumChildLoad.Flat)
				{
					if (_owner != null)
					{
						_owner.UpdateDataQuery(this.Level + 1, value);
					}
				}
			}
		}
		[Category("Database")]
		[ParenthesizePropertyName(true)]
		[Editor(typeof(TypeEditorTurnOffRecursion), typeof(UITypeEditor))]
		[Description("Gets and sets the templates for loading the child nodes via data-binding")]
		public TreeNodeXTemplate ChildNodesTemplate
		{
			get
			{
				TreeNodeXTemplate _templates = null;
				if (ChildLoadStyle == EnumChildLoad.Flat)
				{
					for (int i = 0; i < this.Nodes.Count; i++)
					{
						TreeNodeXTemplate tt = this.Nodes[i] as TreeNodeXTemplate;
						if (tt != null)
						{
							_templates = tt;
							break;
						}
					}
					if (_templates == null)
					{
						_templates = new TreeNodeXTemplate();
						Nodes.Add(_templates);
					}
					_templates.SetOwner(_owner);
				}
				return _templates;
			}
			set
			{
				if (this.Nodes.Count > 0)
				{
					List<TreeNodeXTemplate> l = new List<TreeNodeXTemplate>();
					for (int i = 0; i < this.Nodes.Count; i++)
					{
						TreeNodeXTemplate tt = this.Nodes[i] as TreeNodeXTemplate;
						if (tt != null)
						{
							l.Add(tt);
						}
					}
					if (l.Count > 0)
					{
						foreach (TreeNodeXTemplate tt in l)
						{
							tt.Remove();
						}
					}
				}
				if (value != null)
				{
					value.SetOwner(_owner);
					this.Nodes.Add(value);
				}
			}
		}
		public new string Text
		{
			get
			{
				return _links.GetValue("Text") as string;
			}
			set
			{
				_links.SetConstValue("Text", value);
			}
		}
		public new string ToolTipText
		{
			get
			{
				return _links.GetValue("ToolTipText") as string;
			}
			set
			{
				_links.SetConstValue("ToolTipText", value);
			}
		}
		public new string ImageKey
		{
			get
			{
				return _links.GetValue("ImageKey") as string;
			}
			set
			{
				_links.SetConstValue("ImageKey", value);
			}
		}
		public new string SelectedImageKey
		{
			get
			{
				return _links.GetValue("SelectedImageKey") as string;
			}
			set
			{
				_links.SetConstValue("SelectedImageKey", value);
			}
		}
		public new int ImageIndex
		{
			get
			{
				return _links.ValueInt32("ImageIndex");
			}
			set
			{
				_links.SetConstValue("ImageIndex", value);
			}
		}
		public new int SelectedImageIndex
		{
			get
			{
				return _links.ValueInt32("SelectedImageIndex");
			}
			set
			{
				_links.SetConstValue("SelectedImageIndex", value);
			}
		}
		#endregion
		#region Protected Methods
		protected override TreeViewX GetOwnerTreeView()
		{
			if (_owner != null)
			{
				return _owner;
			}
			return base.GetOwnerTreeView();
		}
		#endregion
		#region Methods
		public TreeNodeXTemplate CloneTemplate()
		{
			TreeNodeXTemplate tt = new TreeNodeXTemplate();
			tt.ForeColor = this.ForeColor;
			tt.BackColor = this.BackColor;
			tt.NodeFont = this.NodeFont;

			tt._owner = _owner;
			tt._links = _links;
			return tt;
		}
		[NotForProgramming]
		[Browsable(false)]
		public void NotifyDesignChange(string name)
		{
			if (_owner != null)
			{
				IDevClass dc = _owner.GetDevClass();
				if (dc != null)
				{
					dc.NotifyChange(this, name);
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CollectPropertyLinks(Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>> links)
		{
			string[] names = _links.GetLinkablePropertyNames();
			Dictionary<IPropertyValueLink, CodeExpression> kv = new Dictionary<IPropertyValueLink, CodeExpression>();
			for (int k = 0; k < names.Length; k++)
			{
				IPropertyValueLink pvk = GetPropertyLink(names[k]);
				if (pvk != null && pvk.IsValueLinkSet())
				{
					//TreeViewX {this}.GetNodeTemplate(int level) to set link to names[k]
					CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression();
					mie.Method.MethodName = "GetNodeTemplateBase";
					mie.Parameters.Add(new CodePrimitiveExpression(this.Level));
					mie.UserData.Add("name", names[k]);
					kv.Add(pvk, mie);
				}
			}
			if (kv.Count > 0)
			{
				links.Add(this, kv);
			}
			if (Nodes.Count > 0)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeXTemplate tt = Nodes[i] as TreeNodeXTemplate;
					if (tt != null)
					{
						tt.CollectPropertyLinks(links);
						break;
					}
				}
			}
		}
		public void SetOwner(TreeViewX owner)
		{
			_owner = owner;
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Node level:{0}", this.Level);
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				lst.Add(_links.GetPropertyDescriptor(p));
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		#endregion
		#region IPropertyValueLinkHolder Members
		public bool IsLinkableProperty(string propertyName)
		{
			return _links.IsLinkableProperty(propertyName);
		}
		public bool IsValueLinkSet(string propertyName)
		{
			return _links.IsValueLinkSet(propertyName);
		}
		public void SetPropertyLink(string propertyName, IPropertyValueLink link)
		{
			_links.SetPropertyLink(propertyName, link);
		}
		public IPropertyValueLink GetPropertyLink(string propertyName)
		{
			return _links.GetValueLink(propertyName);
		}
		public void OnDesignTimePropertyValueChange(string propertyName)
		{
			IDevClass c = _owner.GetDevClass();
			if (c != null)
			{
				c.NotifyChange(_owner, propertyName);
			}
		}
		public void SetPropertyGetter(string propertyName, fnGetPropertyValue getter)
		{
			_links.SetPropertyGetter(propertyName, getter);
		}

		public Type GetPropertyType(string propertyName)
		{
			PropertyInfo pif = this.GetType().GetProperty(propertyName);
			if (pif != null)
			{
				return pif.PropertyType;
			}
			return null;
		}
		public string[] GetLinkablePropertyNames()
		{
			return _links.GetLinkablePropertyNames();
		}
		#endregion

		#region class TypeEditorTurnOffRecursion
		class TypeEditorTurnOffRecursion : UITypeEditor
		{
			public TypeEditorTurnOffRecursion()
			{
			}
			public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (context != null && context.Instance != null && provider != null)
				{
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null)
					{
						TreeNodeXTemplate tt = context.Instance as TreeNodeXTemplate;
						if (tt != null)
						{
							CommandList list = new CommandList(edSvc);
							if (tt.ChildLoadStyle == EnumChildLoad.Flat)
							{
								list.SelectedIndex = 1;
							}
							else if (tt.ChildLoadStyle == EnumChildLoad.None)
							{
								list.SelectedIndex = 0;
							}
							else if (tt.ChildLoadStyle == EnumChildLoad.Recursion)
							{
								list.SelectedIndex = 2;
							}
							edSvc.DropDownControl(list);
							if (list.MadeSelection)
							{
								switch (list.Selection)
								{
									case 0:
										tt.ChildLoadStyle = EnumChildLoad.None; break;
									case 1:
										tt.ChildLoadStyle = EnumChildLoad.Flat; break;
									case 2:
										tt.ChildLoadStyle = EnumChildLoad.Recursion; break;
								}
								tt.NotifyDesignChange(context.PropertyDescriptor.Name);
							}
						}
					}
				}
				return value;
			}
		}
		#endregion
		#region class CommandList
		class CommandList : ListBox
		{
			public bool MadeSelection;
			public int Selection;
			private IWindowsFormsEditorService _service;
			public CommandList(IWindowsFormsEditorService service)
			{
				_service = service;
				this.Items.Add("None");
				this.Items.Add("Level by level");
				this.Items.Add("Recursion");
				Graphics g = this.CreateGraphics();
				SizeF sf = g.MeasureString("R", this.Font);
				g.Dispose();
				this.ItemHeight = (int)(sf.Height) + 2;
				this.DrawMode = DrawMode.OwnerDrawFixed;
			}
			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
				double dn = (double)(e.Y) / (double)(this.ItemHeight);
				int n = (int)dn;
				if (n >= this.Items.Count)
				{
					n = -1;
				}
				this.SelectedIndex = n;
			}
			protected override void OnDrawItem(DrawItemEventArgs e)
			{
				e.DrawBackground();
				if (e.Index >= 0)
				{
					if ((e.State & DrawItemState.Selected) != 0)
					{
						e.DrawFocusRectangle();
					}
					System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
					if (e.Index == 0)
					{
						e.Graphics.DrawImage(TreeViewXResources._empty.ToBitmap(), rc.X, rc.Y);
					}
					else if (e.Index == 1)
					{
						e.Graphics.DrawImage(TreeViewXResources._flat.ToBitmap(), rc.X, rc.Y);
					}
					else if (e.Index == 2)
					{
						e.Graphics.DrawImage(TreeViewXResources._recursion.ToBitmap(), rc.X, rc.Y);
					}
					rc.X += 16;
					if (rc.Width > 16)
					{
						rc.Width -= 16;
					}
					e.Graphics.DrawString(Items[e.Index] as string, this.Font, Brushes.Black, rc);
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = SelectedIndex;
				}
				_service.CloseDropDown();
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				finishSelection();

			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					finishSelection();
				}
			}
		}
		#endregion

		#region ICustomPropertyNames Members
		[NotForProgramming]
		[Browsable(false)]
		public bool UseCustomName(string propertyname)
		{
			if (string.CompareOrdinal(propertyname, "ChildNodesQuery") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(propertyname, "ChildNodesTemplate") == 0)
			{
				return true;
			}
			return false;
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetCustomPropertyName(string propertyname)
		{
			if (string.CompareOrdinal(propertyname, "ChildNodesQuery") == 0 || string.CompareOrdinal(propertyname, "ChildNodesTemplate") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} - Level {1}", propertyname, this.Level + 1);
			}
			return propertyname;
		}
		#endregion

		#region ICustomPropertyCompile Members

		public CodeExpression GetReferenceCode()
		{
			if (_owner == null)
			{
				throw new TreeViewXException("GetReferenceCode: _owner is null");
			}
			CodeFieldReferenceExpression tvCode = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _owner.Name);
			CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression();
			mie.Method.MethodName = "GetNodeTemplateBase";
			mie.Method.TargetObject = tvCode;
			mie.Parameters.Add(new CodePrimitiveExpression(this.Level));
			return mie;
		}

		#endregion
	}
}
