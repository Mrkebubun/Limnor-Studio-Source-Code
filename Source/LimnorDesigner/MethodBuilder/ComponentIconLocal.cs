/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.Windows.Forms;
using System.ComponentModel;
using VPL;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Action;
using MathExp;
using System.Drawing;
using Limnor.PhpComponents;
using System.Drawing.Design;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// the icon for a local variable in a method
	/// </summary>
	public class ComponentIconLocal : ComponentIconForMethod
	{
		#region fields and constructors

		private DataTypePointer _type;
		private string _name;
		private ActiveTextBox _editBox;
		private UInt32 _scopeGroupId;
		public ComponentIconLocal()
		{
			SetIconImage(Resources._var.ToBitmap());
		}
		public ComponentIconLocal(MethodClass method)
			: base(method)
		{
			SetIconImage(Resources._var.ToBitmap());
		}
		public ComponentIconLocal(ActionBranch branch)
			: base(branch)
		{
			SetIconImage(Resources._var.ToBitmap());
		}
		public ComponentIconLocal(ILimnorDesigner designer, LocalVariable pointer, MethodClass method)
			: base(designer, pointer, method)
		{
			_type = pointer.ClassType;
			_name = pointer.Name;
			SetIconImage(Resources._var.ToBitmap());
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			ComponentIconLocal obj = (ComponentIconLocal)base.Clone();
			obj._name = _name;
			obj._type = _type;
			obj._scopeGroupId = _scopeGroupId;
			return obj;
		}
		#endregion

		#region IXmlNodeSerialization Members
		const string XMLATT_ScopeId = "scopeGroupId";
		protected virtual void OnWrite(IXmlCodeWriter writer, XmlNode dataNode)
		{
			if (_type != null)
			{
				writer.WriteObjectToNode(dataNode, _type, true);
				if (_type.IsPrimitive)
				{
					LocalVariable v = this.LocalPointer;
					if (v != null)
					{
						if (v.ObjectInstance != null)
						{
							XmlNode nd = XmlUtil.CreateSingleNewElement(dataNode, "Value");
							writer.WriteValue(nd, v.ObjectInstance, null);
						}
					}
				}
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATT_ScopeId, ScopeGroupId);
			XmlNode dataNode = node.SelectSingleNode(XmlTags.XML_Var);
			if (dataNode == null)
			{
				dataNode = node.OwnerDocument.CreateElement(XmlTags.XML_Var);
				node.AppendChild(dataNode);
			}
			if (!string.IsNullOrEmpty(_name))
				XmlUtil.SetNameAttribute(dataNode, _name);
			OnWrite(writer, dataNode);
		}
		protected virtual void OnRead(IXmlCodeReader reader, XmlNode dataNode)
		{
			Type t = XmlUtil.GetLibTypeAttribute(dataNode);
			if (t != null)
			{
				if (t.Equals(typeof(ParameterClass)))
				{
					_type = (DataTypePointer)Activator.CreateInstance(t, Method);
				}
				else
				{
					_type = (DataTypePointer)Activator.CreateInstance(t);
				}
				reader.ReadObjectFromXmlNode(dataNode, _type, t, this);
				CreateLocalVariable();
				if (_type.IsPrimitive)
				{
					LocalVariable v = this.LocalPointer;
					if (v != null)
					{
						XmlNode nd = dataNode.SelectSingleNode("Value");
						if (nd != null)
						{
							object val = reader.ReadValue(nd, null, _type.ObjectType);
							if (val != null)
							{
								v.ObjectInstance = val;
							}
						}
					}
				}
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			ScopeGroupId = XmlUtil.GetAttributeUInt(node, XMLATT_ScopeId);
			XmlNode dataNode = node.SelectSingleNode(XmlTags.XML_Var);
			if (dataNode != null)
			{
				string s = XmlUtil.GetNameAttribute(dataNode);
				if (!string.IsNullOrEmpty(s))
				{
					_name = s;
				}
				OnRead(reader, dataNode);
			}
		}
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{
				if (ClassPointer == null)
				{
					if (_type != null)
					{
						LocalVariable v = _type.CreateVariable(_name, objMap.ClassId, MemberId);
						v.Owner = Method;
						ClassPointer = v;
					}
				}
			}
		}

		#endregion
		#region Properties
		public LocalVariable LocalPointer
		{
			get
			{
				return this.ClassPointer as LocalVariable;
			}
		}
		public UInt32 ScopeGroupId
		{
			get
			{
				return _scopeGroupId;
			}
			set
			{
				_scopeGroupId = value;
				if (LocalPointer != null)
				{
					LocalPointer.ScopeGroupId = _scopeGroupId;
				}
			}
		}
		public override bool NameReadOnly
		{
			get
			{
				return false;
			}
		}
		public override PropertyPointer CreatePropertyPointer(string propertyName)
		{
			PropertyDescriptor pd = VPLUtil.GetProperty(_type, propertyName);
			PropertyPointer pp = new PropertyPointer();
			pp.Owner = ClassPointer;
			pp.SetPropertyInfo(pd);
			return pp;
		}
		#endregion
		#region Methods
		protected void SetNameType(DataTypePointer t, string n)
		{
			_type = t;
			_name = n;
		}
		protected string GetVariableName()
		{
			return _name;
		}
		protected override IList<PropertyDescriptor> OnGetProperties(Attribute[] attrs)
		{
			if (_type != null)
			{
				if (_type.IsGenericType)
				{
					if (_type.TypeParameters != null && _type.TypeParameters.Length > 0)
					{
						Type[] tcs = _type.BaseClassType.GetGenericArguments();
						if (tcs != null && tcs.Length == _type.TypeParameters.Length)
						{
							List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
							for (int i = 0; i < tcs.Length; i++)
							{
								lst.Add(new PropertyDescriptorForDisplay(this.GetType(), tcs[i].Name, _type.TypeParameters[i].DataTypeName, new Attribute[] { new ParenthesizePropertyNameAttribute(true) }));
							}
							return lst;
						}
					}
				}
				else if (_type.IsGenericParameter)
				{
					if (_type.ConcreteType == null && this.ClassPointer != null)
					{
						MethodClass mc = this.ClassPointer.Owner as MethodClass;
						if (mc != null)
						{
							DataTypePointer dp = mc.GetConcreteType(_type.BaseClassType);
							if (dp != null)
							{
								_type.SetConcreteType(dp);
							}
						}
					}
					if (_type.ConcreteType != null)
					{
						List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
						lst.Add(new PropertyDescriptorForDisplay(this.GetType(), _type.Name, _type.ConcreteType.DataTypeName, new Attribute[] { new ParenthesizePropertyNameAttribute(true) }));
						return lst;
					}
				}
				else if (_type.IsPrimitive)
				{
					LocalVariable v = ClassPointer as LocalVariable;
					if (v != null)
					{
						List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
						PropertyDescriptorVariableValue p = new PropertyDescriptorVariableValue("Value", attrs, "", v, this.GetType());
						p.ValueChanged += p_ValueChanged;
						lst.Add(p);
						return lst;
					}
				}
			}
			return null;
		}

		void p_ValueChanged(object sender, EventArgs e)
		{
			MethodDesignerHolder mh = MethodViewer;
			if (mh != null)
			{
				mh.Changed = true;
			}
		}
		protected override void OnEstablishObjectOwnership(MethodClass owner)
		{
			if (this.ClassPointer.Owner == null)
			{
				this.ClassPointer.Owner = owner;
			}
		}
		public void HookNameChecker()
		{
			LocalVariable v = ClassPointer as LocalVariable;
			if (v != null)
			{
				v.SetNameChecker(checkNameChange);
			}
		}
		public void CreateLocalVariable()
		{
			LocalVariable v = _type.CreateVariable(_name, ClassId, MemberId);
			v.Owner = Method;
			v.ScopeGroupId = ScopeGroupId;
			ClassPointer = v;
		}
		protected override LimnorContextMenuCollection GetMenuData()
		{
			if (LocalPointer == null)
			{
				throw new DesignerException("Calling GetMenuData with null local variable");
			}
			return LimnorContextMenuCollection.GetMenuCollection(LocalPointer);//, runAt);
		}
		public override void OnRelativeDrawingMouseDoubleClick(RelativeDrawing relDraw, MouseEventArgs e)
		{
			rename();
		}
		protected override IAction OnCreateAction(MenuItemDataMethod data, ILimnorDesignPane designPane)
		{
			IAction act = data.CreateMethodAction(designPane, this.LocalPointer, MethodViewer.Method, MethodViewer.ActionsHolder);
			if (act != null)
			{
				act.ScopeMethod = MethodViewer.Method;
				act.ActionHolder = MethodViewer.ActionsHolder;
			}
			return act;
		}
		protected override ActionClass OnCreateSetPropertyAction(MenuItemDataProperty data)
		{
			ActionClass act = data.CreateSetPropertyAction(MethodViewer.Loader.DesignPane, this.LocalPointer, MethodViewer.Method, MethodViewer.ActionsHolder);
			if (act != null)
			{
				act.SetScopeMethod(MethodViewer.Method);
			}
			return act;
		}
		protected override void OnSetImage()
		{
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			MenuItem mi;
			//
			mi = new MenuItemWithBitmap("Create Set Value Action", Resources._setVar.ToBitmap());
			mi.Click += new EventHandler(miNewInstance_Click);
			mnu.MenuItems.Add(mi);
			//
			mi = new MenuItem("-");
			mnu.MenuItems.Add(mi);
			//
			mi = new MenuItemWithBitmap("Rename", Resources._rename.ToBitmap());
			mi.Click += new EventHandler(miRename_Click);
			mnu.MenuItems.Add(mi);
			//
			mi = new MenuItemWithBitmap("Change Icon", Resources._changeIcon.ToBitmap());
			mi.Click += new EventHandler(miChangeIcon_Click);
			mnu.MenuItems.Add(mi);
			//
			mi = new MenuItem("-");
			mnu.MenuItems.Add(mi);
			//
			mi = new MenuItemWithBitmap("Remove", Resources._cancel.ToBitmap());
			mi.Click += new EventHandler(miDelete_Click);
			mnu.MenuItems.Add(mi);

		}
		private void rename()
		{
			if (this.Parent != null)
			{
				if (_editBox == null)
				{
					_editBox = new ActiveTextBox();
					this.Parent.Controls.Add(_editBox);
					_editBox.Initialize();
					_editBox.CancelEdit += new EventHandler(_editBox_CancelEdit);
					_editBox.FinishEdit += new EventHandler(_editBox_FinishEdit);
				}
				_editBox.Size = GetLabelSize();
				if (_editBox.Width < 80)
				{
					_editBox.Width = 80;
				}
				_editBox.Height = _editBox.Height + 4;
				_editBox.TextBoxText = _name;
				_editBox.Location = GetLabelLocation();
				_editBox.Visible = true;
				ShowLabel(false);
			}
		}
		public void ChangeName(string newName)
		{
			LocalVariable lv = this.ClassPointer as LocalVariable;
			_name = newName;
			lv.SetName(_name);
			SetLabelText(_name);
			IList<MethodDiagramViewer> lst = MethodViewer.GetViewer();
			foreach (MethodDiagramViewer dv in lst)
			{
				for (int i = 0; i < dv.Controls.Count; i++)
				{
					ActionViewerConstructor avc = dv.Controls[i] as ActionViewerConstructor;
					if (avc != null)
					{
						AB_Constructor bc = avc.ActionObject as AB_Constructor;
						ConstructorPointer cp = bc.ActionData.ActionMethod as ConstructorPointer;
						LocalVariable lv2 = cp.Owner as LocalVariable;
						if (lv2.IsSameObjectRef(lv))
						{
							lv2.SetName(_name);
							avc.ActionName = "Create " + _name;
							break;
						}
					}
				}
			}
		}
		void _editBox_FinishEdit(object sender, EventArgs e)
		{
			string newName = _editBox.TextBoxText;
			if (!string.IsNullOrEmpty(newName))
			{
				if (newName != _name)
				{
					IList<MethodDiagramViewer> l = MethodViewer.GetViewer();
					foreach (MethodDiagramViewer dv in l)
					{
						if (dv.IsNameUsed(newName))
						{
							_editBox.TextBoxText = _name;
							MessageBox.Show(this.FindForm(), string.Format("The name, {0}, is in use", newName), "Rename", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							_editBox.SetTextFocus();
							return;
						}
						else
						{
							ChangeName(newName);
						}
						dv.Changed = true;
					}
				}
			}
			_editBox.Visible = false;
			ShowLabel(true);
		}

		void _editBox_CancelEdit(object sender, EventArgs e)
		{
			_editBox.Visible = false;
			ShowLabel(true);
		}
		void miChangeIcon_Click(object sender, EventArgs e)
		{
			LocalVariable lv = this.ClassPointer as LocalVariable;
			Image img = DesignUtil.ChangeTypeIcon(lv.ObjectType, IconImage, this.FindForm());
			if (img != null)
			{
				SetIconImage(img);
				Refresh();
			}
		}
		void miRename_Click(object sender, EventArgs e)
		{
			rename();
		}
		void miNewInstance_Click(object sender, EventArgs e)
		{
			MethodDesignerHolder mv = MethodViewer;
			if (mv != null)
			{
				LocalVariable lv = this.ClassPointer as LocalVariable;
				ActionAssignInstance act = mv.CreateSetValueAction(lv);
				//
				Point p = mv.PointToClient(System.Windows.Forms.Cursor.Position);
				if (p.X < 0 || p.X > (mv.Width / 2))
					p.X = 10;
				if (p.Y < 0 || p.Y > (mv.Height / 2))
					p.Y = 10;
				ActionViewer av = mv.AddNewAction(act, p);
#if DEBUG
				if (av != null)
				{
					if (!av.Visible)
					{
						MessageBox.Show("Action Viewer not visible");
					}
				}
#endif
			}
		}
		void miDelete_Click(object sender, EventArgs e)
		{
			LocalVariable lv = this.ClassPointer as LocalVariable;
			List<IAction> acts = lv.RootPointer.GetRelatedActions(lv.MemberId);
			bool bOK = (acts.Count == 0);
			if (bOK)
			{
				MethodDesignerHolder mdh = MethodViewer;
				if (mdh != null)
				{
					BranchList bl = mdh.ActionList;
					if (bl != null)
					{
						Dictionary<UInt32, IAction> actions = new Dictionary<uint, IAction>();
						bl.GetActionsUseLocalVariable(lv.MemberId, actions);
						if (actions.Count > 0)
						{
							foreach (KeyValuePair<UInt32, IAction> kv in actions)
							{
								acts.Add(kv.Value);
							}
							bOK = false;
						}
					}
				}
			}
			if (!bOK)
			{
				List<ObjectTextID> objs = new List<ObjectTextID>();
				foreach (IAction a in acts)
				{
					objs.Add(new ObjectTextID("Action", "", a.Display));
				}
				dlgObjectUsage dlg = new dlgObjectUsage();
				dlg.LoadData("There are actions using this object. These actions must be removed before this object can be removed.", "Remove object", objs);
				//currently no OK will be returned
				bOK = (dlg.ShowDialog(this.FindForm()) == DialogResult.OK);
				if (bOK)
				{
					IList<MethodDiagramViewer> l = MethodViewer.GetViewer();
					foreach (MethodDiagramViewer mv in l)
					{
						//remove local/constructor actions belong to it
						List<IComponent> avList = new List<IComponent>();
						for (int i = 0; i < mv.Controls.Count; i++)
						{
							ActionViewer av = mv.Controls[i] as ActionViewer;
							if (av != null)
							{
								foreach (IAction a in acts)
								{
									if (av.ActionObject.ContainsActionId(a.ActionId))
									{
										avList.Add(av);
									}
								}
							}
						}
						if (avList.Count > 0)
						{
							mv.DeleteComponents(avList.ToArray());
						}
					}
				}
			}
			if (bOK)
			{
				MethodViewer.RemoveLocalVariable(this);
				RemoveLabel();
				Control p = this.Parent;
				if (p != null)
				{
					p.Controls.Remove(this);
				}
				MethodViewer.Changed = true;
			}
		}
		private void checkNameChange(object sender, EventArgs e)
		{
			EventArgNameChange anc = e as EventArgNameChange;
			if (anc != null)
			{
				if (string.IsNullOrEmpty(anc.NewName))
				{
					MessageBox.Show(this.FindForm(), "The new name cannot be empty", "Change variable name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					IList<MethodDiagramViewer> l = MethodViewer.GetViewer();
					foreach (MethodDiagramViewer mv in l)
					{
						if (mv.IsNameUsed(anc.NewName))
						{
							anc.Cancel = true;
							MessageBox.Show(this.FindForm(), "The new name is in use", "Change variable name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}
					_name = anc.NewName;
					SetLabelText(anc.NewName);
					MethodViewer.Changed = true;
				}
			}
		}
		#endregion
	}
	class PropertyDescriptorItemType : PropertyDescriptor
	{
		private PhpArray _pa;
		public PropertyDescriptorItemType(PhpArray pa)
			: base("ItemType", new Attribute[]{
				new DescriptionAttribute("Data type for each array item"),
				new EditorAttribute(typeof(TypeEditorSelectPhpType),typeof(UITypeEditor))
			})
		{
			_pa = pa;
		}
		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return typeof(PhpArray); }
		}

		public override object GetValue(object component)
		{
			return _pa.ItemType;
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return typeof(Type); }
		}

		public override void ResetValue(object component)
		{
			_pa.ItemType = typeof(PhpString);
		}

		public override void SetValue(object component, object value)
		{
			_pa.ItemType = value as Type;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
