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
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using ProgElements;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.Collections.Specialized;
using LimnorDesigner.Event;
using VPL;
using LimnorDesigner.Action;
using Limnor.WebBuilder;
using System.Globalization;
using System.Windows.Forms;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// pointer to a ParameterClass
	/// </summary>
	public class CustomMethodParameterPointer : IObjectPointer, IDelayedInitialize, ISourceValuePointer
	{
		private ParameterClass _parameter;
		public CustomMethodParameterPointer()
		{
		}
		public CustomMethodParameterPointer(ParameterClass p)
		{
			_parameter = p;
		}
		public ParameterClass Parameter
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter;
			}
		}
		public override string ToString()
		{
			if (_parameter == null)
				return "?";
			return _parameter.ToString();
		}
		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_parameter != null)
				{
					MethodClass mc = _parameter.Method as MethodClass;
					if (mc != null)
					{
						return mc.RunAt;
					}
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.RootPointer;
			}
		}
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
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return typeof(object);
				EventHandlerMethod handler = _parameter.Owner as EventHandlerMethod;
				if (handler != null)
				{
					if (typeof(object).Equals(_parameter.ObjectType) && string.CompareOrdinal(_parameter.Name, "sender") == 0)
					{
						_parameter.SetDataType(handler.Event.Owner.ObjectType);
					}
					else if (typeof(EventArgs).Equals(_parameter.ObjectType) && string.CompareOrdinal(_parameter.Name, "e") == 0)
					{
						ICustomEventMethodDescriptor ce = handler.Event.Owner.ObjectInstance as ICustomEventMethodDescriptor;
						if (ce != null)
						{
							Type pType = ce.GetEventArgumentType(handler.Event.Name);
							if (pType != null)
							{
								_parameter.SetDataType(pType);
							}
						}
					}
				}
				return _parameter.ObjectType;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.ObjectInstance;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.ObjectDebug;
			}
			set
			{
			}
		}
		public string ReferenceName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return string.Empty;
				return _parameter.ReferenceName;
			}
		}

		public string CodeName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.CodeName;
			}
		}

		public string DisplayName
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return "?";
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
					return "?";
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
					return "?";
				return _parameter.ExpressionDisplay;
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			tryDeseirialize();
			if (_parameter == null)
				return false;
			return _parameter.IsTargeted(target);
		}

		public string ObjectKey
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.ObjectKey;
			}
		}

		public string TypeString
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.TypeString;
			}
		}

		public bool IsValid
		{
			get
			{
				tryDeseirialize();
				if (_parameter != null)
				{
					return _parameter.IsValid;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_parameter is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			tryDeseirialize();
			if (_parameter == null)
				return null;
			return _parameter.GetReferenceCode(method, statements, forValue);
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			tryDeseirialize();
			if (_parameter == null)
				return null;
			return _parameter.GetJavaScriptReferenceCode(code);
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			tryDeseirialize();
			if (_parameter == null)
				return null;
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
				return false;
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
		public IObjectIdentity IdentityOwner
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return null;
				return _parameter.IdentityOwner;
			}
		}

		public bool IsStatic
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return false;
				return _parameter.IsStatic;
			}
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return EnumObjectDevelopType.Custom;
				return _parameter.ObjectDevelopType;
			}
		}

		public EnumPointerType PointerType
		{
			get
			{
				tryDeseirialize();
				if (_parameter == null)
					return EnumPointerType.Property;
				return _parameter.PointerType;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			tryDeseirialize();
			CustomMethodParameterPointer obj = new CustomMethodParameterPointer();
			obj._parameter = _parameter;
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
				tryDeseirialize();
				if (_parameter != null)
				{
					XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_MethodID, _parameter.MethodId);
					XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_ParamId, _parameter.ParameterID);
					XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_NAME, _parameter.Name);
					XmlUtil.SetAttribute(objectNode, XmlTags.XMLATT_ClassID, _parameter.ClassId);
				}
			}
			else
			{
				_reader = serializer as XmlObjectReader;
				_objMap = objMap;
				_objectNode = objectNode;
			}
		}

		#endregion

		#region private methods
		private void tryDeseirialize()
		{
			if (_parameter == null && _objectNode != null && _objMap != null && _reader != null)
			{
				OnDelayedPostSerialize(_objMap, _objectNode, _reader);
			}
		}
		#endregion
		#region IDelayedInitialize Members
		private ObjectIDmap _objMap;
		private XmlNode _objectNode;
		private XmlObjectReader _reader;
		public void OnDelayedPostSerialize(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			UInt32 methodId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_MethodID);
			if (methodId != 0)
			{
				ClassPointer root = null;
				MethodClass mc = null;
				UInt32 paramId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ParamId);
				string name = XmlUtil.GetAttribute(objectNode, XmlTags.XMLATT_NAME);
				UInt32 classId = XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_ClassID);
				if (classId != 0)
				{
					root = objMap.Project.GetTypedData<ClassPointer>(classId);
					if (root != null)
					{
						mc = root.GetCustomMethodById(methodId);
					}
				}
				if (mc == null)
				{
					classId = XmlUtil.GetAttributeUInt(objectNode.OwnerDocument.DocumentElement, XmlTags.XMLATT_ClassID);
					root = objMap.Project.GetTypedData<ClassPointer>(classId);
					if (root != null)
					{
						mc = root.GetCustomMethodById(methodId);
					}
				}
				if (mc == null)
				{
					classId = objMap.ClassId;
					root = objMap.Project.GetTypedData<ClassPointer>(classId);
					if (root != null)
					{
						mc = root.GetCustomMethodById(methodId);
					}
				}
				if (mc == null)
				{
					classId = reader.ClassId;
					root = objMap.Project.GetTypedData<ClassPointer>(classId);
					if (root != null)
					{
						mc = root.GetCustomMethodById(methodId);
					}
				}
				if (mc == null)
				{
					Form f = null;
					if (root != null)
					{
						Control c = root.ObjectInstance as Control;
						if (c != null)
						{
							f = c.FindForm();
						}
					}
					MathNode.Log(f, new DesignerException("CustomMethodParameterPointer.OnDelayedPostSerialize: Custom method not found [{0}]. Used in class [{1}]", methodId, XmlUtil.GetAttributeUInt(objectNode.OwnerDocument.DocumentElement, XmlTags.XMLATT_ClassID)));
				}
				else
				{
					ParameterClass pc = mc.GetParameterByID(paramId);
					if (pc == null)
					{
						pc = mc.GetParameterByName(name);
					}
					if (pc == null)
					{
						Form f = null;
						if (root != null)
						{
							Control c = root.ObjectInstance as Control;
							if (c != null)
							{
								f = c.FindForm();
							}
						}
						MathNode.Log(f, new DesignerException("Parameter [{0}] for custom method not found [{1},{2}]", paramId, root.ClassId, methodId));
					}
					_parameter = pc;
				}
			}
		}
		public void SetReader(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			_objMap = objMap;
			_objectNode = objectNode;
			_reader = reader;
		}
		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return false; } }
		public object ValueOwner
		{
			get
			{
				if (_parameter != null)
				{
					return _parameter.ValueOwner;
				}
				return null;
			}
		}

		public string DataPassingCodeName
		{
			get
			{
				if (_parameter != null)
				{
					return _parameter.DataPassingCodeName;
				}
				return null;
			}
		}
		uint _taskId;
		public uint TaskId
		{
			get
			{
				if (_parameter != null)
				{
					return _parameter.TaskId;
				}
				return _taskId;
			}
		}

		public void SetTaskId(uint taskId)
		{
			_taskId = taskId;
			if (_parameter != null)
			{
				_parameter.SetTaskId(taskId);
			}
		}
		object _owner;
		public void SetValueOwner(object o)
		{
			_owner = o;
			if (_parameter != null)
			{
				_parameter.SetValueOwner(o);
			}
		}

		public bool IsSameProperty(ISourceValuePointer p)
		{
			if (_parameter != null)
			{
				return _parameter.IsSameProperty(p);
			}
			return false;
		}

		public bool IsWebClientValue()
		{
			if (_parameter != null)
			{
				return _parameter.IsWebClientValue();
			}
			return false;
		}

		public bool IsWebServerValue()
		{
			if (_parameter != null)
			{
				return _parameter.IsWebServerValue();
			}
			return false;
		}

		public string CreateJavaScript(StringCollection method)
		{
			if (_parameter != null)
			{
				return _parameter.CreateJavaScript(method);
			}
			return null;
		}

		public string CreatePhpScript(StringCollection method)
		{
			if (_parameter != null)
			{
				return _parameter.CreatePhpScript(method);
			}
			return null;
		}

		public bool CanWrite
		{
			get
			{
				if (_parameter != null)
				{
					return _parameter.CanWrite;
				}
				return false;
			}
		}

		#endregion
	}
}
