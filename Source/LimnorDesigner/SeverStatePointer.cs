/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Property;
using ProgElements;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;
using System.Xml;
using System.Globalization;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// for creating upload action to preserve web server state
	/// </summary>
	public class SeverStatePointer : IProperty, ISourceValuePointer
	{
		#region fields and constructors
		private object _owner;
		private UInt32 _taskId;
		private string _dataName;
		private DataTypePointer _dataType;
		private ClassPointer _root;
		private ISourceValuePointer _value;
		private IProperty _prop;
		private EnumWebRunAt _runAt = EnumWebRunAt.Client;
		public SeverStatePointer(ISourceValuePointer sv, ClassPointer root, EnumWebRunAt runAt)
			: this(sv.DataPassingCodeName, new DataTypePointer(typeof(string)), root, runAt)
		{
			_value = sv;
			_prop = sv as IProperty;
			if (_prop != null)
			{
				_dataType = new DataTypePointer(_prop.ObjectType);
			}
		}
		public SeverStatePointer(string dataName, DataTypePointer dataType, ClassPointer root, EnumWebRunAt runAt)
		{
			_runAt = runAt;
			_dataName = dataName;
			_dataType = dataType;
			_root = root;
		}
		#endregion
		#region Methods
		public void SetRunAt(EnumWebRunAt runAt)
		{
			_runAt = runAt;
		}
		#endregion
		#region Properties
		public ISourceValuePointer SourceValue
		{
			get
			{
				return _value;
			}
		}
		#endregion
		#region IProperty Members

		public string Name
		{
			get { return _dataName; }
		}
		public bool IsReadOnly { get { return !this.CanWrite; } }
		public DataTypePointer PropertyType
		{
			get { return _dataType; }
		}

		public EnumAccessControl AccessControl
		{
			get
			{
				return EnumAccessControl.Public;
			}
			set
			{
			}
		}

		public bool IsCustomProperty
		{
			get { return false; }
		}

		public bool Implemented
		{
			get { return true; }
		}

		public ClassPointer Declarer
		{
			get { return _root; }
		}

		public IClass Holder
		{
			get { return _root; }
		}

		public void SetName(string name)
		{
			_dataName = name;
		}

		public void SetValue(object value)
		{
		}

		public IList<Attribute> GetUITypeEditor()
		{
			return null;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			if (_value != null)
			{
				return new SeverStatePointer(_value, _root, _runAt);
			}
			return new SeverStatePointer(_dataName, _dataType, _root, _runAt);
		}

		#endregion

		#region IObjectPointer Members

		public ClassPointer RootPointer
		{
			get { return _root; }
		}

		public IObjectPointer Owner
		{
			get
			{
				IObjectPointer o = _owner as IObjectPointer;
				if (o == null)
				{
					if (_prop != null)
					{
						if (_prop.Owner != null)
						{
							o = _prop.Owner;
						}
					}
				}
				if (o == null)
				{
					if (_value != null)
					{
						o = _value.ValueOwner as IObjectPointer;
					}
				}
				if (o == null)
				{
					o = _root;
				}
				return o;
			}
			set
			{
			}
		}

		public object ObjectDebug
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public string ReferenceName
		{
			get { return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", this.DataPassingCodeName); }
		}

		public string DisplayName
		{
			get { return DataPassingCodeName; }
		}

		public string LongDisplayName
		{
			get { return DataPassingCodeName; }
		}

		public string ExpressionDisplay
		{
			get { return DataPassingCodeName; }
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			return false;
		}

		public string ObjectKey
		{
			get { return DataPassingCodeName; }
		}

		public string TypeString
		{
			get
			{
				if (_dataType == null)
				{
					return string.Empty;
				}
				return _dataType.TypeString;
			}
		}

		public bool IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(DataPassingCodeName))
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "DataPassingCodeName is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (this._dataType.ObjectType.IsArray)
			{
				return new CodeMethodInvokeExpression(
				   new CodeTypeReferenceExpression("clientRequest"),
				   "GetStringArrayValue",
				   new CodePrimitiveExpression(DataPassingCodeName)
				   );
			}
			else
			{
				return new CodeMethodInvokeExpression(
					new CodeTypeReferenceExpression("clientRequest"),
					"GetStringValue",
					new CodePrimitiveExpression(DataPassingCodeName)
					);
			}
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			throw new NotImplementedException();
		}

		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", DataPassingCodeName);
		}

		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_runAt == EnumWebRunAt.Client)
			{
				return string.Format(CultureInfo.InvariantCulture, "$this->jsonFromClient->values->{0}", DataPassingCodeName);
			}
			if (_value != null)
			{
				return _value.CreatePhpScript(code);
			}
			return string.Empty;
		}

		public EnumWebRunAt RunAt
		{
			get { return _runAt; }
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			SeverStatePointer ssp = objectIdentity as SeverStatePointer;
			if (ssp != null)
			{
				return string.CompareOrdinal(_dataName, ssp._dataName) == 0;
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_prop != null)
				{
					return _prop.Owner;
				}
				if (_value != null)
				{
					return _value.ValueOwner as IObjectIdentity;
				}
				return _root;
			}
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Property; }
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IPropertyPointer Members

		public string CodeName
		{
			get { return _dataName; }
		}

		public Type ObjectType
		{
			get
			{
				if (_dataType != null)
					return _dataType.BaseClassType;
				return typeof(string);
			}
			set
			{
			}
		}

		public object ObjectInstance
		{
			get
			{
				IObjectPointer op = _value as IObjectPointer;
				if (op != null)
				{
					return op.ObjectInstance;
				}
				return null;
			}
			set
			{
			}
		}

		public IPropertyPointer PropertyOwner
		{
			get
			{
				if (_prop != null)
				{
					return _prop.PropertyOwner;
				}
				if (_value != null)
				{
					return _value.ValueOwner as IPropertyPointer;
				}
				return null;
			}
		}

		#endregion

		#region IGenericTypePointer Members

		public DataTypePointer[] GetConcreteTypes()
		{
			return null;
		}

		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			return null;
		}

		public CodeTypeReference GetCodeTypeReference()
		{
			throw new NotImplementedException();
		}

		public IList<DataTypePointer> GetGenericTypes()
		{
			return null;
		}

		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return false; } }
		public object ValueOwner
		{
			get { return this.Owner; }
		}

		public string DataPassingCodeName
		{
			get
			{
				if (_value != null)
					return _value.DataPassingCodeName;
				return _dataName;
			}
		}

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
			if (_value != null)
			{
				return _value.IsSameProperty(p);
			}
			return false;
		}

		public bool IsWebClientValue()
		{
			if (_runAt == EnumWebRunAt.Client)
			{
				return true;
			}
			if (_value != null)
			{
				return _value.IsWebClientValue();
			}
			return false;
		}

		public bool IsWebServerValue()
		{
			if (_runAt == EnumWebRunAt.Server)
			{
				return true;
			}
			if (_value != null)
			{
				return _value.IsWebServerValue();
			}
			return false;
		}

		public string CreateJavaScript(StringCollection method)
		{
			return GetJavaScriptReferenceCode(method);
		}

		public string CreatePhpScript(StringCollection method)
		{
			return GetPhpScriptReferenceCode(method);
		}

		public bool CanWrite
		{
			get
			{
				if (_value != null)
				{
					return _value.CanWrite;
				}
				return false;
			}
		}

		#endregion
	}
}
