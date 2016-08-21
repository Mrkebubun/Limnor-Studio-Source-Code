/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using VPL;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using ProgElements;
using System.Drawing;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.Action
{
	public class ActionBranchParameterPointer : IClass, IObjectPointer, IXmlNodeSerializable, IPostDeserializeProcess, IDelayedInitialize, ILocalvariable
	{
		#region fields and constructors
		private ActionBranchParameter _parameter;
		private ClassPointer _root;
		private Image _img;
		private Type _parameterType;
		public ActionBranchParameterPointer()
		{
		}
		public ActionBranchParameterPointer(ActionBranchParameter p, ClassPointer root)
		{
			_parameter = p;
			if (p != null)
			{
				_parameterType = p.ObjectType;
			}
			_root = root;
			ParameterName = p.Name;
			ParameterId = p.ParameterID;
		}
		#endregion
		#region Properties
		public ActionBranchParameter Parameter
		{
			get
			{
				tryDeseirialize();
				if (_parameter != null)
				{
					return _parameter;
				}
				return null;
			}
		}
		[Browsable(false)]
		public UInt32 DefinitionClassId
		{
			get
			{
				return ClassId;
			}
		}
		public UInt32 ClassId { get; set; }
		public UInt32 MethodId { get; set; }
		public UInt32 BranchId { get; set; }
		public UInt32 ParameterId { get; set; }
		public string ParameterName { get; set; }
		#endregion
		#region Methods
		public override string ToString()
		{
			if (_parameter != null)
			{
				return _parameter.DisplayName;
			}
			return ParameterName;
		}
		public void SetParameterType(Type t)
		{
			_parameterType = t;
		}
		#endregion
		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_MethodID, MethodId);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ActionID, BranchId);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_NAME, ParameterName);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ParamId, ParameterId);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			MethodId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_MethodID);
			BranchId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ActionID);
			ParameterName = XmlUtil.GetAttribute(node, XmlTags.XMLATT_NAME);
			ParameterId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ParamId);
		}

		#endregion
		#region IPostDeserializeProcess Members

		public void OnDeserialize(object context)
		{
			string err;
			_root = context as ClassPointer;
			if (_root == null)
			{
				if (context == null)
				{
					err = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.OnDeserialize(object context): context is null. MethodId:{1},BranchId:{2},ParameterId:{3},ParameterName:{4}", this.GetType().Name, MethodId, BranchId, ParameterId, ParameterName);
				}
				else
				{
					err = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.OnDeserialize(object context): context {1} is not a ClassPointer. MethodId:{2},BranchId:{3},ParameterId:{4},ParameterName:{5}", this.GetType().Name, context.GetType().Name, MethodId, BranchId, ParameterId, ParameterName);
				}
				MathNode.LogError(err);
			}
			else
			{
				MethodClass mc = _root.GetCustomMethodById(MethodId);
				if (mc == null)
				{
					if (_root.ActionInstances != null)
					{
						IAction a;
						if (_root.ActionInstances.TryGetValue(MethodId, out a))
						{
							MethodActionForeach ma = a as MethodActionForeach;
							if (ma != null)
							{
								mc = ma;
							}
						}
					}
				}
				if (mc == null)
				{
					err = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.OnDeserialize(object context): method [{1},{2}] is not found in ClassPointer.  MethodId:{2},BranchId:{3},ParameterId:{4},ParameterName:{5}", this.GetType().Name, _root.ClassId, MethodId, BranchId, ParameterId, ParameterName);
					MathNode.LogError(err);
				}
				if (mc != null)
				{
					ActionBranch ab = mc.SearchBranchById(BranchId);
					if (ab == null)
					{
						//throw new DesignerException("{0}.OnDeserialize(object context): Action Branch [{1}] not found in method [{2},{3}][{4}]", this.GetType().Name, BranchId, _root.ClassId, MethodId, mc.Name);
						mc.AddDeserializer(this);
					}
					else
					{
						AB_SubMethodAction sma = ab as AB_SubMethodAction;
						if (sma != null)
						{
							_parameter = sma.GetParameterById(ParameterId);
						}
						if (_parameter == null)
						{
							_parameter = ab.GetActionBranchParameterByName(ParameterName);
							if (_parameter != null)
							{

								_parameter.ParameterID = ParameterId;
							}
						}
					}
				}
			}
		}

		#endregion

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_parameter != null)
				{
					if (_parameter.ActionBranch != null)
					{
						if (_parameter.ActionBranch.Method != null)
						{
							return _parameter.ActionBranch.Method.RunAt;
						}
					}
					MethodClass mc = _parameter.Method as MethodClass;
					if (mc != null)
					{
						return mc.RunAt;
					}
				}
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				if (_root == null)
				{
					if (_objMap != null)
					{
						_root = _objMap.GetTypedData<ClassPointer>();
					}
				}
				return _root;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				tryDeseirialize();
				if (_parameter != null)
				{
					return _parameter.Owner;
				}
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		public IClass Host
		{
			get
			{
				return _root;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				tryDeseirialize();
				if (_parameter != null)
					return _parameter.ObjectType;
				return _parameterType;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				tryDeseirialize();
				if (_parameter != null)
					return _parameter.ObjectInstance;
				return null;
			}
			set
			{
				if (_parameter != null)
				{
					_parameter.ObjectInstance = value;
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return null;
				}
				return _parameter.ObjectDebug;
			}
			set
			{
				tryDeseirialize();
				if (_parameter != null)
				{
					_parameter.ObjectDebug = value;
				}
			}
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return "?";
				return _parameter.ReferenceName;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return "?";
				}
				return _parameter.CodeName;
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return "?";
				}
				return _parameter.DisplayName;
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return "?";
				}
				return _parameter.LongDisplayName;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return ParameterName;
				}
				return _parameter.ExpressionDisplay;
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			tryDeseirialize();
			if (_parameter == null)
			{
				return false;
			}
			return _parameter.IsTargeted(target);
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return "?";
				}
				return _parameter.ObjectKey;
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return "?";
				}
				return _parameter.TypeString;
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_parameter is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
					return false;
				}
				return _parameter.IsValid;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			tryDeseirialize();
			if (_parameter == null)
			{
				return null;
			}
			return _parameter.GetReferenceCode(method, statements, forValue);
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			tryDeseirialize();
			if (_parameter == null)
			{
				return null;
			}
			return _parameter.GetJavaScriptReferenceCode(code);
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			tryDeseirialize();
			if (_parameter == null)
			{
				return null;
			}
			return _parameter.GetPhpScriptReferenceCode(code);
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			tryDeseirialize();
			if (_parameter == null)
			{
				return false;
			}
			return _parameter.IsSameObjectRef(objectIdentity);
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectIdentity = p as IObjectIdentity;
			if (objectIdentity != null)
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return false;
				}
				return _parameter.IsSameObjectRef(objectIdentity);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return null;
				}
				return _parameter.IdentityOwner;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return false;
				}
				return _parameter.IsStatic;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return EnumObjectDevelopType.Both;
				}
				return _parameter.ObjectDevelopType;
			}
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return EnumPointerType.Unknown;
				}
				return _parameter.PointerType;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ActionBranchParameterPointer obj = new ActionBranchParameterPointer();
			obj.ClassId = ClassId;
			obj.BranchId = BranchId;
			obj.MethodId = MethodId;
			obj.ParameterName = ParameterName;
			obj.ParameterId = ParameterId;
			obj._root = _root;
			obj._parameter = _parameter;
			obj._img = _img;
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			_objMap = objMap;
			_objectNode = objectNode;
			_reader = serializer as XmlObjectReader;
		}

		#endregion

		#region IClass Members
		[Browsable(false)]
		[ReadOnly(true)]
		public Image ImageIcon
		{
			get
			{
				if (_img == null)
				{
					_img = Resources.param;
				}
				return _img;
			}
			set
			{
				_img = value;
			}
		}

		public Type VariableLibType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return typeof(object);
				}
				return _parameter.VariableLibType;
			}
		}

		public ClassPointer VariableCustomType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					if (_root != null)
					{
						return _root;
					}
					if (_objMap != null)
					{
						return _objMap.GetTypedData<ClassPointer>();
					}
					return null;
				}
				return _parameter.VariableCustomType;
			}
		}

		public LimnorDesigner.MenuUtil.IClassWrapper VariableWrapperType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return null;
				}
				return _parameter.VariableWrapperType;
			}
		}

		#endregion

		#region ICustomObject Members

		public ulong WholeId
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return 0;
				}
				return _parameter.WholeId;
			}
		}

		public uint MemberId
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return 0;
				}
				return _parameter.MemberId;
			}
		}

		public string Name
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
				{
					return "?";
				}
				return _parameter.Name;
			}
		}

		#endregion

		#region IDelayedInitialize Members
		private ObjectIDmap _objMap;
		private XmlNode _objectNode;
		private XmlObjectReader _reader;
		public void OnDelayedPostSerialize(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			OnDeserialize(objMap.GetTypedData<ClassPointer>());
		}

		public void SetReader(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			_objMap = objMap;
			_objectNode = objectNode;
			_reader = reader;
		}
		private void tryDeseirialize()
		{
			if (_parameter == null && _objectNode != null && _objMap != null && _reader != null)
			{
				OnDelayedPostSerialize(_objMap, _objectNode, _reader);
			}
		}
		#endregion

		#region ILocalvariable Members

		public DataTypePointer ValueType
		{
			get { return _parameter; }
		}
		public DataTypePointer[] GetConcreteTypes()
		{
			DataTypePointer p = ValueType;
			if (p != null)
			{
				return p.TypeParameters;
			}
			return null;
		}
		public IList<DataTypePointer> GetGenericTypes()
		{
			DataTypePointer p = ValueType;
			if (p != null)
			{
				return p.GetGenericTypes();
			}
			return null;
		}
		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			DataTypePointer p = ValueType;
			if (p != null)
			{
				return p.GetConcreteType(typeParameter);
			}
			return null;
		}
		public CodeTypeReference GetCodeTypeReference()
		{
			DataTypePointer p = ValueType;
			if (p != null)
			{
				return p.GetCodeTypeReference();
			}
			return null;
		}
		#endregion
	}
}
