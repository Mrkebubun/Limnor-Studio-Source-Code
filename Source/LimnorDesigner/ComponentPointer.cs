/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using XmlSerializer;
using System.Drawing.Design;
using System.Windows.Forms;
using VSPrj;
using ProgElements;
using MathExp;
using System.CodeDom;
using System.Windows.Forms.Design;
using VPL;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner
{
	/// <summary>
	/// pointer to an instant of component hosted by a root class
	/// </summary>
	public class ComponentPointer : IObjectPointer, IPostDeserializeProcess
	{
		#region fields and constructors
		private UInt32 _memberId;
		private ClassInstancePointer _pointer;
		private MemberComponentId _memberPointer;
		public ComponentPointer()
		{
		}
		public ComponentPointer(ClassInstancePointer cr)
		{
			_pointer = cr;
			if (cr != null)
			{
				_memberId = cr.MemberId;
			}
		}
		public ComponentPointer(MemberComponentId cid)
		{
			_memberPointer = cid;
			if (cid != null)
			{
				_memberId = cid.MemberId;
			}
		}
		#endregion
		#region Properties
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt64 WholeId
		{
			get
			{
				if (_pointer != null)
					return _pointer.WholeId;
				if (_memberPointer != null)
					return _memberPointer.WholeId;
				return 0;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				if (_pointer != null)
					return _pointer.ClassId;
				if (_memberPointer != null)
					return _memberPointer.ClassId;
				return 0;
			}
		}
		[Browsable(false)]
		public UInt32 MemberId
		{
			get
			{
				return _memberId;
			}
			set
			{
				_memberId = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		[Description("Class or member name")]
		public string Name
		{
			get
			{
				if (_pointer != null)
					return _pointer.Name;
				if (_memberPointer != null)
					return _memberPointer.Name;
				return "?";
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public string Namespace
		{
			get
			{
				if (_pointer != null)
					return _pointer.Namespace;
				if (_memberPointer != null)
					if (_memberPointer.RootPointer != null)
					{
						return _memberPointer.RootPointer.Namespace;
					}
				return "Limnor";
			}
			set
			{
				if (_pointer != null)
					_pointer.Namespace = value;
				if (_memberPointer != null && _memberPointer.RootPointer != null)
				{
					_memberPointer.RootPointer.Namespace = value;
				}
			}
		}
		/// <summary>
		/// a static class cannot be a member component
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public bool AlwaysStatic
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		#endregion
		#region Methods
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public override string ToString()
		{
			if (_pointer != null)
			{
				if (_pointer.Definition != null)
				{
					return Name + " of " + _pointer.Definition.Name;
				}
			}
			else
			{
				if (_memberPointer != null)
				{
					return _memberPointer.ToString();
				}
			}
			return Name;
		}
		public string GetTypeName(string scopeNamespace)
		{
			if (_pointer != null)
			{
				return _pointer.GetTypeName(scopeNamespace);
			}
			if (_memberPointer != null)
			{
				return _memberPointer.TypeString;
			}
			return null;
		}
		#endregion
		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_pointer != null)
				{
					if (JsTypeAttribute.IsJsType(_pointer.ObjectType))
					{
						return EnumWebRunAt.Client;
					}
					if (PhpTypeAttribute.IsPhpType(_pointer.ObjectType))
					{
						return EnumWebRunAt.Server;
					}
					if (_pointer.ObjectInstance is IWebClientComponent)
					{
						return EnumWebRunAt.Client;
					}
				}
				if (_memberPointer != null)
				{
					if (JsTypeAttribute.IsJsType(_memberPointer.ObjectType))
					{
						return EnumWebRunAt.Client;
					}
					if (PhpTypeAttribute.IsPhpType(_memberPointer.ObjectType))
					{
						return EnumWebRunAt.Server;
					}
					if (_memberPointer.ObjectInstance is IWebClientComponent)
					{
						return EnumWebRunAt.Client;
					}
				}
				return EnumWebRunAt.Inherit;
			}
		}
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				if (_pointer != null)
					return _pointer.RootPointer;//.Host;
				if (_memberPointer != null)
					return _memberPointer.RootPointer;
				return null;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				return Name;
			}
		}
		/// <summary>
		/// fully qualified variable name
		/// </summary>
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return Name;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (_pointer != null)
					return _pointer.IsStatic;
				if (_memberPointer != null)
					return _memberPointer.IsStatic;
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				if (_pointer != null)
					return _pointer.Owner;
				if (_memberPointer != null)
					return _memberPointer.Owner;
				return null;
			}
			set
			{
				if (_pointer != null)
					_pointer.Owner = value;
				if (_memberPointer != null)
					_memberPointer.Owner = value;
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (_pointer != null)
					return _pointer.ObjectType;
				if (_memberPointer != null)
					return _memberPointer.ObjectType;
				return typeof(object);
			}
			set
			{
				if (_pointer != null)
					_pointer.ObjectType = value;
				if (_memberPointer != null)
					_memberPointer.ObjectType = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				if (_pointer != null)
					return _pointer.ObjectInstance;
				if (_memberPointer != null)
					return _memberPointer.ObjectInstance;
				return null;
			}
			set
			{
				if (_pointer != null)
					_pointer.ObjectInstance = value;
				if (_memberPointer != null)
					_memberPointer.ObjectInstance = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get
			{
				if (_pointer != null)
					return _pointer.ObjectDebug;
				if (_memberPointer != null)
					return _memberPointer.ObjectDebug;
				return null;
			}
			set
			{
				if (_pointer != null)
					_pointer.ObjectDebug = value;
				if (_memberPointer != null)
					_memberPointer.ObjectDebug = value;
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (_pointer != null)
					return _pointer.DisplayName;
				if (_memberPointer != null)
					return _memberPointer.DisplayName;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				if (_pointer != null)
					return _pointer.LongDisplayName;
				if (_memberPointer != null)
					return _memberPointer.LongDisplayName;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get
			{
				if (_pointer != null)
					return _pointer.Name;
				if (_memberPointer != null)
					return _memberPointer.ExpressionDisplay;
				return string.Empty;
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (_pointer != null)
				return _pointer.IsTargeted(target);
			if (_memberPointer != null)
				return _memberPointer.IsTargeted(target);
			return false;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				if (_pointer != null)
					return _pointer.ObjectKey;
				if (_memberPointer != null)
					return _memberPointer.ObjectKey;
				return "?";
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				if (_pointer != null)
					return _pointer.TypeString;
				if (_memberPointer != null)
					return _memberPointer.TypeString;
				return null;
			}
		}

		public System.CodeDom.CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_pointer != null)
				return _pointer.GetReferenceCode(method, statements, forValue);
			if (_memberPointer != null)
				return _memberPointer.GetReferenceCode(method, statements, forValue);
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_pointer != null)
				return _pointer.GetJavaScriptReferenceCode(code);
			if (_memberPointer != null)
				return _memberPointer.GetJavaScriptReferenceCode(code);
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_pointer != null)
				return _pointer.GetPhpScriptReferenceCode(code);
			if (_memberPointer != null)
				return _memberPointer.GetPhpScriptReferenceCode(code);
			return null;
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (_pointer != null)
				_pointer.CreateActionJavaScript(methodName, code, parameters, returnReceiver);
			else if (_memberPointer != null)
				_memberPointer.CreateActionJavaScript(methodName, code, parameters, returnReceiver);
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
			}
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (_pointer != null)
				_pointer.CreateActionPhpScript(methodName, code, parameters, returnReceiver);
			else if (_memberPointer != null)
				_memberPointer.CreateActionPhpScript(methodName, code, parameters, returnReceiver);
		}
		public bool IsValid
		{
			get
			{
				if (_pointer != null || _memberPointer != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_pointer and _memberPointer are null for [{0}] of [{1}]. (_pointer={2},_memberPointer={3})", this.ToString(), this.GetType().Name, _pointer, _memberPointer);
				return false;
			}
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			if (_pointer != null)
				return _pointer.IsSameObjectRef(objectIdentity);
			if (_memberPointer != null)
				return _memberPointer.IsSameObjectRef(objectIdentity);
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectIdentity = p as IObjectIdentity;
			if (objectIdentity != null)
			{
				if (_pointer != null)
					return _pointer.IsSameObjectRef(objectIdentity);
				if (_memberPointer != null)
					return _memberPointer.IsSameObjectRef(objectIdentity);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_pointer != null)
					return _pointer.IdentityOwner;
				if (_memberPointer != null)
					return _memberPointer.IdentityOwner;
				return null;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Custom; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Class; } }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			if (_pointer != null)
				return new ComponentPointer(_pointer);
			if (_memberPointer != null)
				return new ComponentPointer(_memberPointer);
			ComponentPointer c = new ComponentPointer();
			c._memberId = _memberId;
			return c;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{
				if (_pointer == null && _memberPointer == null)
				{
					XmlNode node = SerializeUtil.GetClassRefNodeByObjectId(objectNode, _memberId);
					if (node != null)
					{
						XmlObjectReader xr = (XmlObjectReader)serializer;
						_pointer = xr.ReadObject<ClassInstancePointer>(node, ClassPointer.CreateClassPointer(objMap));
						//_pointer.ObjectInstance = objMap.GetObjectByID(_pointer.MemberId);
					}
					else
					{
						_pointer = objMap.GetClassRefById(_memberId) as ClassInstancePointer;
					}
				}
				if (_pointer == null)
				{
					object v = objMap.GetObjectByID(_memberId);
					if (v != null)
					{
						_memberPointer = new MemberComponentId(objMap.GetTypedData<ClassPointer>(), v, _memberId);
					}
				}
				if (_pointer == null && _memberPointer == null)
				{
					objMap.AddPostProcessor(this);
				}
			}
		}

		#endregion

		#region IPostDeserializeProcess Members

		public void OnDeserialize(object context)
		{
			if (_pointer == null)
			{
				ClassPointer cp = (ClassPointer)context;
				_pointer = (ClassInstancePointer)cp.ObjectList.GetClassRefById(_memberId);
				if (_pointer != null)
				{
					if (_pointer.Definition == null)
					{
						//_pointer.ReplaceDeclaringClassPointer(cp);
					}
				}
			}
		}

		#endregion
	}
	public class ComponentPointerSelector<T> : PropEditorDropDown
	{
		public ComponentPointerSelector()
		{
		}
		class ComponentName
		{
			public IComponent Component;
			public ComponentName(IComponent c)
			{
				Component = c;
			}
			public override string ToString()
			{
				if (Component.Site != null)
					return Component.Site.Name;
				return "";
			}
		}
		public override object OnGetDropDownControl(ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService service, object value)
		{
			ComponentPointer cp = (ComponentPointer)value;
			IComponent h = context.Instance as IComponent;
			if (h == null || h.Site == null)
			{
				h = VPL.VPLUtil.GetObject(context.Instance) as IComponent;
			}
			if (h == null || h.Site == null)
			{
				ClassPointer root = context.Instance as ClassPointer;
				if (root != null)
				{
					h = root.ObjectInstance as IComponent;
				}
			}
			if (h == null || h.Site == null)
			{
				IObjectPointer op = context.Instance as IObjectPointer;
				if (op != null)
				{
					ClassPointer root = op.RootPointer;
					if (root != null)
					{
						h = root.ObjectInstance as IComponent;
					}
				}
			}
			if (h != null && h.Site != null)
			{
				ListBox list = new ListBox();
				list.Tag = service;
				list.Click += new EventHandler(list_Click);
				list.KeyPress += new KeyPressEventHandler(list_KeyPress);
				foreach (IComponent ic in h.Site.Container.Components)
				{
					if (ic is T)
					{
						list.Items.Add(new ComponentName(ic));
					}
					else
					{
						Type t = VPL.VPLUtil.GetObjectType(ic);
						if (typeof(T).IsAssignableFrom(t))
						{
							int n = list.Items.Add(new ComponentName(ic));
							if (cp != null && list.SelectedIndex < 0 && ic.Site != null)
							{
								if (cp.Name == ic.Site.Name)
								{
									list.SelectedIndex = n;
								}
							}
						}
					}
				}
				if (list.Items.Count > 0)
				{
					service.DropDownControl(list);
					if (list.SelectedIndex >= 0)
					{
						ComponentName c = list.Items[list.SelectedIndex] as ComponentName;
						if (c != null)
						{
							ILimnorDesignerLoader loader = LimnorProject.ActiveDesignerLoader as ILimnorDesignerLoader;
							UInt32 memberId = loader.ObjectMap.GetObjectID(c.Component);
							if (memberId != 0)
							{
								XmlNode nodeCr = SerializeUtil.GetClassRefNodeByObjectId(loader.Node, memberId);
								if (nodeCr != null)
								{
									loader.Reader.ResetErrors();
									ClassInstancePointer cr = loader.Reader.ReadObject<ClassInstancePointer>(nodeCr, loader.GetRootId());
									if (loader.Reader.HasErrors)
									{
										MathNode.Log(loader.Reader.Errors);
									}
									ComponentPointer pt = new ComponentPointer(cr);
									value = pt;
								}
								else
								{
									MemberComponentId cid = new MemberComponentId(loader.GetRootId(), c.Component, memberId);
									ComponentPointer pt = new ComponentPointer(cid);
									value = pt;
								}
							}
						}
					}
				}
			}
			return value;
		}

		void list_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				ListBox list = sender as ListBox;
				if (list != null)
				{
					IWindowsFormsEditorService service = list.Tag as IWindowsFormsEditorService;
					if (service != null)
					{
						service.CloseDropDown();
					}
				}
			}
		}

		void list_Click(object sender, EventArgs e)
		{
			ListBox list = sender as ListBox;
			if (list != null)
			{
				IWindowsFormsEditorService service = list.Tag as IWindowsFormsEditorService;
				if (service != null)
				{
					service.CloseDropDown();
				}
			}
		}
	}
}
