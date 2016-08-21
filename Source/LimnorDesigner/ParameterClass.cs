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
using ProgElements;
using System.ComponentModel;
using System.Drawing;
using System.CodeDom;
using MathExp;
using System.Xml;
using XmlSerializer;
using XmlUtility;
using VSPrj;
using VPL;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using System.Globalization;
using LimnorDesigner.Web;
using Limnor.WebBuilder;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using Limnor.WebServerBuilder;

namespace LimnorDesigner
{
	public class NamedDataType : DataTypePointer
	{
		const string NAME = "Name";
		const string ID = "ID";
		private string _name;
		private UInt32 _id;
		public NamedDataType()
		{
		}
		public NamedDataType(TypePointer type)
			: base(type)
		{
		}
		public NamedDataType(ClassPointer component)
			: base(component)
		{
		}
		public NamedDataType(TypePointer type, string name)
			: this(type)
		{
			_name = name;
		}
		public NamedDataType(Type type, string name)
			: this(new TypePointer(type), name)
		{
		}
		[Browsable(false)]
		public override string ExpressionDisplay
		{
			get
			{
				return _name;
			}
		}
		[ReadOnly(false)]
		[ParenthesizePropertyName(true)]
		public override string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (_name != value)
				{
					EventArgNameChange e = new EventArgNameChange(value, _name);
					OnBeforeNameChange(e);
					if (!e.Cancel)
					{
						_name = value;
						OnNameChanged();
					}
				}
			}
		}
		[Browsable(false)]
		public UInt32 ParameterID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		[Browsable(false)]
		public override UInt32 MemberId
		{
			get
			{
				return ParameterID;
			}
		}

		protected virtual void OnBeforeNameChange(EventArgNameChange e)
		{
		}
		protected virtual void OnNameChanged()
		{
		}
		/// <summary>
		/// not triggering name checking
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			_name = name;
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(_name))
				return DataTypeName;
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Name, DataTypeName);
		}
		#region ICloneable Members
		public override object Clone()
		{
			NamedDataType obj = (NamedDataType)base.Clone();
			obj._id = _id;
			obj._name = _name;
			return obj;
		}
		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlNode nd = XmlUtil.CreateSingleNewElement(node, NAME);
			XmlUtil.SetNameAttribute(nd, Name);
			XmlUtil.SetAttribute(nd, ID, ParameterID);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			XmlNode nd = node.SelectSingleNode(NAME);
			if (nd != null)
			{
				Name = XmlUtil.GetNameAttribute(nd);
				ParameterID = XmlUtil.GetAttributeUInt(nd, ID);
			}
		}
		#endregion
	}
	/// <summary>
	/// for customer methods
	/// method parameter, method variable
	/// </summary>
	[UseParentObject]
	public class ParameterClass : NamedDataType, IParameter, IWithProject, ICustomTypeDescriptor, ILocalvariable, ISourceValuePointer
	{
		#region fields and constructors

		const string DESC = "Description";
		private object _value; //for test execution
		private string _desc;
		private Bitmap _icon;
		private IMethod _method;//method using the parameter, should be a MethodClass
		private IWithProject _prjItem;
		public event EventHandler NameChanging;
		public event EventHandler NameChanged;
		public event EventHandler OnDescriptionChanged;
		public ParameterClass(IMethod method)
		{
			_method = method;
			TypeChanged += new EventHandler(ParameterDef_OnTypeChange);
		}
		public ParameterClass(ParameterValue v)
			: this(v.OwnerMethod)
		{
		}
		public ParameterClass(DataTypePointer type, IMethod method)
			: this(method)
		{
			SetDataType(type);
		}
		public ParameterClass(TypePointer type, IMethod method)
			: base(type)
		{
			_method = method;
			TypeChanged += new EventHandler(ParameterDef_OnTypeChange);
		}
		public ParameterClass(ComponentIconParameter componentIcon)
			: this(componentIcon.Method)
		{
		}
		public ParameterClass(TypePointer type, string name, IMethod method)
			: base(type, name)
		{
			_method = method;
			TypeChanged += new EventHandler(ParameterDef_OnTypeChange);
		}
		public ParameterClass(Type type, string name, IMethod method)
			: base(type, name)
		{
			_method = method;
			TypeChanged += new EventHandler(ParameterDef_OnTypeChange);
		}
		public ParameterClass(Type type, string name, IWithProject prjHolder, IMethod method)
			: this(new TypePointer(type), name, method)
		{
			_prjItem = prjHolder;
		}
		#endregion
		#region events
		void ParameterDef_OnTypeChange(object sender, EventArgs e)
		{
			_icon = null;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override ClassPointer RootPointer
		{
			get
			{
				ClassPointer root = base.RootPointer;
				if (root != null)
					return root;
				return _prjItem as ClassPointer;
			}
		}
		public override UInt32 ClassId
		{
			get
			{
				MethodClass mc = _method as MethodClass;
				if (mc != null)
				{
					return mc.ClassId;
				}
				if (this.RootPointer != null)
				{
					return this.RootPointer.ClassId;
				}
				throw new DesignerException("ParameterClass {0} missing MethodClass", Name);
			}
		}
		[Browsable(false)]
		public override ulong WholeId
		{
			get
			{
				return DesignUtil.MakeDDWord(ParameterID, ClassId);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public virtual IMethod Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
				if (this.IsGenericParameter)
				{
					if (this.ConcreteType == null)
					{
						MethodPointer mp = _method as MethodPointer;
						if (mp != null)
						{
							DataTypePointer dp = mp.GetConcreteType(this.BaseClassType);
							if (dp != null)
							{
								this.SetConcreteType(dp);
							}
						}
					}
				}
				else if (this.IsGenericType)
				{
					if (this.TypeParameters == null)
					{
						MethodPointer mp = _method as MethodPointer;
						if (mp != null)
						{
							DataTypePointer dp = mp.GetConcreteType(this.BaseClassType);
							if (dp != null)
							{
								this.TypeParameters = dp.TypeParameters;
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override bool ReadOnly
		{
			get
			{
				MethodClass mc = _method as MethodClass;
				if (mc != null)
				{
					if (mc.IsOverride)
					{
						return true;
					}
				}
				return base.ReadOnly;
			}
			set
			{
				base.ReadOnly = value;
			}
		}
		[Browsable(false)]
		public virtual UInt32 MethodId
		{
			get
			{
				MethodClass mc = _method as MethodClass;
				if (mc != null)
				{
					return mc.MethodID;
				}
				return 0;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IWithProject ProjectItem
		{
			get
			{
				return _prjItem;
			}
			set
			{
				_prjItem = value;
			}
		}
		[Browsable(false)]
		public override Image ImageIcon
		{
			get
			{
				return Resources._param.ToBitmap();
			}
		}
		[Browsable(false)]
		public Bitmap Icon
		{
			get
			{
				if (_icon == null)
				{
					Type t = this.ObjectType;
					if (t == null || t.Equals(typeof(void)))
					{
						_icon = Resources._void;
					}
					else
					{
						_icon = (Bitmap)TreeViewObjectExplorer.GetTypeImage(t);
					}
				}
				return _icon;
			}
		}
		[Browsable(false)]
		public bool AllowTypeChange { get; set; }

		/// <summary>
		/// for test execution
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public override object ObjectInstance
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
				if (OnDescriptionChanged != null)
				{
					OnDescriptionChanged(this, EventArgs.Empty);
				}
			}
		}
		[Browsable(false)]
		public override string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(Name))
				{
					return base.DisplayName;
				}
				return Name + ":" + base.DisplayName;
			}
		}
		[Browsable(false)]
		public override string CodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_PHP)
				{
					return string.Format(CultureInfo.InvariantCulture, "${0}", Name);
				}
				return Name;
			}
		}
		#endregion
		#region ICloneable Members
		protected override DataTypePointer CreateCloneObject()
		{
			return (ParameterClass)Activator.CreateInstance(this.GetType(), _method);
		}
		public override object Clone()
		{
			ParameterClass dtp = (ParameterClass)base.Clone();
			dtp._desc = _desc;
			dtp.Name = Name;
			dtp.ParameterID = ParameterID;
			dtp.ReadOnly = ReadOnly;
			return dtp;
		}

		#endregion
		#region Methods
		protected override void OnBeforeNameChange(EventArgNameChange e)
		{
			if (NameChanging != null)
			{
				NameChanging(this, e);
			}
		}
		protected override void OnNameChanged()
		{
			if (NameChanged != null)
			{
				NameChanged(this, EventArgs.Empty);
			}
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
			{
				return DataTypeName;
			}
			return DataTypeName + " " + Name;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (Project != null && Project.ProjectType == EnumProjectType.WebAppAspx)
			{
				return CompilerUtil.GetWebRequestValue(this.DataPassingCodeName);
			}
			return new CodeVariableReferenceExpression(CodeName);
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			return CodeName;
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			return CodeName;
		}
		public override string CreateJavaScript(StringCollection sb)
		{
			return CodeName;
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (typeof(HtmlElement_BodyBase).IsAssignableFrom(this.ParameterLibType))
			{
				HtmlElement_BodyBase.CompilerCreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
			}
		}
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (this.ObjectInstance == null && this.ObjectType != null)
			{
				try
				{
					this.ObjectInstance = Activator.CreateInstance(this.ObjectType);
				}
				catch
				{
				}
			}
			if (this.ObjectInstance != null)
			{
				IPhpCodeType php = this.ObjectInstance as IPhpCodeType;
				if (php != null)
				{
					php.CreateActionPhpScript(this.CodeName, methodName, code, parameters, returnReceiver);
				}
				else
				{
					IWebServerProgrammingSupport wp = this.ObjectInstance as IWebServerProgrammingSupport;
					if (wp != null)
					{
						wp.CreateActionPhpScript(this.CodeName,methodName, code, parameters, returnReceiver);
					}
				}
			}
		}
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			ParameterClass p = objectPointer as ParameterClass;
			if (p == null)
			{
				CustomMethodParameterPointer cmpp = objectPointer as CustomMethodParameterPointer;
				if (cmpp != null)
				{
					p = cmpp.Parameter;
				}
			}
			if (p != null)
			{
				if (p.ParameterID == this.ParameterID)
				{
					return true;
				}
				if (string.CompareOrdinal(p.CodeName, this.CodeName) == 0)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public override bool IsSameProperty(IPropertyPointer objectPointer)
		{
			ParameterClass p = objectPointer as ParameterClass;
			if (p != null)
			{
				if (p.ParameterID == this.ParameterID)
				{
					return true;
				}
				if (string.CompareOrdinal(p.CodeName, this.CodeName) == 0)
				{
					return true;
				}
			}
			return false;
		}
		#endregion
		#region IParameter Members

		[Browsable(false)]
		public Type ParameterLibType
		{
			get
			{
				if (IsLibType)
				{
					return LibTypePointer.ClassType;
				}
				if (ClassTypePointer != null)
				{
					return ClassTypePointer.TypeDesigntime;
				}
				return null;
			}
			set
			{
				if (IsLibType)
				{
					LibTypePointer.ClassType = value;
				}
			}
		}
		[Browsable(false)]
		public string ParameterTypeString
		{
			get
			{
				if (IsLibType)
				{
					return LibTypePointer.ClassType.AssemblyQualifiedName;
				}
				if (ClassTypePointer != null)
				{
					return ClassTypePointer.TypeString;
				}
				return null;
			}
		}

		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);

			if (!string.IsNullOrEmpty(Description))
			{
				XmlNode nd = node.OwnerDocument.CreateElement(DESC);
				node.AppendChild(nd);
				nd.InnerText = Description;
			}
			MethodClass mc = _method as MethodClass;
			if (mc != null)
			{
				XmlUtil.SetAttribute(node.ParentNode, XmlTags.XMLATT_ownerMethodId, mc.MethodID);
			}
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);

			XmlNode nd = node.SelectSingleNode(DESC);
			if (nd != null)
			{
				Description = nd.InnerText;
			}
		}

		#endregion
		#region IWithProject Members
		[Browsable(false)]
		public override LimnorProject Project
		{
			get
			{
				if (_prjItem != null)
					return _prjItem.Project;
				IWithProject w = _method as IWithProject;
				if (w != null)
				{
					return w.Project;
				}
				MethodClass mc = _method as MethodClass;
				if (mc != null)
				{
					return mc.Project;
				}
				SubMethodInfoPointer sm = _method as SubMethodInfoPointer;
				if (sm != null && sm.ActionOwner != null && sm.ActionOwner.Class != null)
				{
					return sm.ActionOwner.Class.Project;
				}
				return null;
			}
		}

		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (ReadOnly || !AllowTypeChange)
			{
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				foreach (PropertyDescriptor p in ps)
				{
					if (string.CompareOrdinal(p.Name, "DataType") == 0)
					{
						if (AllowTypeChange)
						{
							list.Add(p);
						}
						else
						{
							PropertyDescriptorForDisplay pdf = new PropertyDescriptorForDisplay(this.GetType(), p.Name, TypeName, new Attribute[] { });
							list.Add(pdf);
						}
					}
					else
					{
						if (ReadOnly)
						{
							ReadOnlyPropertyDesc rp = new ReadOnlyPropertyDesc(p);
							list.Add(rp);
						}
						else
						{
							list.Add(p);
						}
					}
				}
				ps = new PropertyDescriptorCollection(list.ToArray());
			}
			return ps;

		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region ILocalvariable Members

		public DataTypePointer ValueType
		{
			get { return this; }
		}

		#endregion

		#region ISourceValuePointer Members
		uint _taskId;
		private object _owner;
		[Browsable(false)]
		public object ValueOwner
		{
			get
			{
				if (_owner != null)
				{
					return _owner;
				}
				return _method;
			}
		}
		[Browsable(false)]
		public bool IsMethodReturn { get { return false; } }
		[Browsable(false)]
		public string DataPassingCodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_JS || VPLUtil.CompilerContext_PHP)
				{
					if (_method != null && !string.IsNullOrEmpty(_method.MethodName) && !string.IsNullOrEmpty(this.Name))
					{
						EventHandlerMethod ehm = _method as EventHandlerMethod;
						if (ehm != null)
						{
							return CompilerUtil.CreateJsCodeName(ehm.Event, this.Name);
						}
					}
				}
				return CompilerUtil.CreateJsCodeName(this.ParameterID, _taskId); 
			}
		}
		[Browsable(false)]
		public uint TaskId
		{
			get { return _taskId; }
		}

		public void SetTaskId(uint taskId)
		{
			_taskId = taskId;
		}

		public void SetValueOwner(object o)
		{
			_owner = o;
		}

		public bool IsSameProperty(ISourceValuePointer p)
		{
			ParameterClass pc = p as ParameterClass;
			if (pc != null)
			{
				return pc.ParameterID == this.ParameterID;
			}
			else
			{
				CustomMethodParameterPointer cmp = p as CustomMethodParameterPointer;
				if (cmp != null)
				{
					if (cmp.Parameter != null)
					{
						if (cmp.Parameter.ParameterID == this.ParameterID)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool IsWebClientValue()
		{
			MethodClass mc = _method as MethodClass;
			if (mc != null)
			{
				return mc.RunAt == EnumWebRunAt.Client;
			}
			return false;
		}

		public bool IsWebServerValue()
		{
			MethodClass mc = _method as MethodClass;
			if (mc != null)
			{
				return mc.RunAt == EnumWebRunAt.Server;
			}
			return false;
		}

		public bool CanWrite
		{
			get { return true; }
		}

		#endregion
	}
}
