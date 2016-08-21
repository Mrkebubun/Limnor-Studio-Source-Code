/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using MathExp;
using System.CodeDom;
using System.ComponentModel;
using ProgElements;
using VPL;

namespace MathExp.RaisTypes
{
	public enum EnumThreadType
	{
		Unknown = 0,
		Main,
		SubMain,
		Independent,
		Merged
	}
	public enum EnumReturnBranchDetection
	{
		Unknown = 0,
		AllReturned,
		NotAllReturned
	}
	/// <summary>
	///   <xs:complexType name="MethodType">
	///     <!-- Describes a method signature -->
	///     <xs:sequence>
	///       <xs:element name="ReturnType" type="DataType" minOccurs="0" maxOccurs="1" />
	///       <xs:element name="Parameters" minOccurs="0" maxOccurs="1">
	///         <xs:complexType>
	///           <xs:sequence>
	///             <xs:element name="Parameter" type="DataType" minOccurs="0" maxOccurs="unbounded" />
	///           </xs:sequence>
	///         </xs:complexType>
	///       </xs:element>
	///     </xs:sequence>
	///     <xs:attribute name="name" type="xs:string" use="required" />
	///     <!-- method name -->
	///   </xs:complexType>
	/// </summary>
	public class MethodType : IXmlNodeSerializable, IMethodCompile
	{
		#region Fields and constructors
		private UInt32 _id; //method ID
		private string _name;
		private string _desc;
		private bool _static;
		private Parameter[] _parameters;
		private RaisDataType _returnType;
		private CodeTypeDeclaration _hostType;
		private EnumThreadType _threadType = EnumThreadType.Unknown;
		//
		private CodeMemberMethod _methodCode;
		private CodeTypeDeclaration _td;
		private EnumReturnBranchDetection _returnDetection = EnumReturnBranchDetection.Unknown;
		public MethodType()
		{
		}
		#endregion
		#region properties
		[Browsable(false)]
		public EnumReturnBranchDetection ReturnDetection
		{
			get
			{
				return _returnDetection;
			}
			set
			{
				_returnDetection = value;
			}
		}
		[Browsable(false)]
		public EnumThreadType ThreadType
		{
			get
			{
				return _threadType;
			}
			set
			{
				_threadType = value;
			}
		}
		[Browsable(false)]
		public virtual ObjectRef OwnerObject
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public virtual string OwnerTypeCodeName
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public CodeTypeDeclaration TypeDeclaration
		{
			get
			{
				return _td;
			}
			set
			{
				_td = value;
			}
		}
		[Browsable(false)]
		public CodeMemberMethod MethodCode
		{
			get
			{
				return _methodCode;
			}
			set
			{
				_methodCode = value;
			}
		}
		[Browsable(false)]
		public CodeTypeDeclaration HostType
		{
			get
			{
				return _hostType;
			}
			set
			{
				_hostType = value;
			}
		}
		private bool _mu;
		[Description("If this property is True then Control updating actions are executed under UI thread.")]
		public virtual bool MakeUIThreadSafe
		{
			get { return _mu; }
			set { _mu = value; }
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
			}
		}
		public string MethodName
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		public UInt32 MethodID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)Guid.NewGuid().GetHashCode();
					onIDset();
				}
				return _id;
			}
			set
			{
				_id = value;
				onIDset();
			}
		}
		public bool IsStatic
		{
			get
			{
				return _static;
			}
			set
			{
				_static = value;
			}
		}
		public Parameter[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
				onIDset();
			}
		}
		public int ParameterCount
		{
			get
			{
				if (_parameters == null)
					return 0;
				return _parameters.Length;
			}
		}
		public bool HasReturn
		{
			get
			{
				if (_returnType != null)
				{
					if (_returnType.IsVoid)
					{
						return false;
					}
					return true;
				}
				return false;
			}
		}
		public RaisDataType ReturnType
		{
			get
			{
				if (_returnType == null)
				{
					_returnType = new RaisDataType(typeof(double));
				}
				return _returnType;
			}
			set
			{
				_returnType = value;
			}
		}
		private Stack<IMethod0> _subMethods;
		/// <summary>
		/// when compiling a sub method, set the sub-method here to pass ot into compiling elements
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public Stack<IMethod0> SubMethod
		{
			get
			{
				if (_subMethods == null)
					_subMethods = new Stack<IMethod0>();
				return _subMethods;
			}
		}
		public IMethod GetSubMethodByParameterId(UInt32 parameterId)
		{
			IMethod smi = null;
			Stack<IMethod0>.Enumerator en = SubMethod.GetEnumerator();
			while (en.MoveNext())
			{
				IMethod mp = en.Current as IMethod;
				if (mp != null)
				{
					IList<IParameter> ps = mp.MethodParameterTypes;
					if (ps != null && ps.Count > 0)
					{
						foreach (IParameter p in ps)
						{
							if (p.ParameterID == parameterId)
							{
								smi = mp;
								break;
							}
						}
					}
					if (smi != null)
					{
						break;
					}
				}
			}
			return smi;
		}
		public object ModuleProject
		{
			get
			{
				throw new NotSupportedException("ProjectData is not supported by MethodType");
			}
		}
		#endregion
		#region methods
		public object CompilerData(string key)
		{
			return null;
		}
		public virtual MethodType CloneMethod()
		{
			MethodType obj = (MethodType)Activator.CreateInstance(this.GetType());
			obj.MethodID = MethodID;
			obj.MethodName = MethodName;
			obj.Description = Description;
			obj.IsStatic = IsStatic;
			obj.MakeUIThreadSafe = MakeUIThreadSafe;
			if (ParameterCount > 0)
			{
				Parameter[] ps = new Parameter[ParameterCount];
				for (int i = 0; i < ps.Length; i++)
				{
					ps[i] = (Parameter)Parameters[i].Clone();
				}
				obj.Parameters = ps;
			}
			obj.ReturnType = ReturnType;
			obj.HostType = HostType;
			obj.ThreadType = _threadType;
			obj.ReturnDetection = _returnDetection;
			return obj;
		}
		public bool ParameterNameExist(string name, UInt32 id)
		{
			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Length; i++)
				{
					if (_parameters[i].ID != id)
					{
						if (name == _parameters[i].Name)
							return true;
					}
				}
			}
			return false;
		}
		public bool ParameterNameValid(string name)
		{
			NameCreation nc = new NameCreation();
			return nc.IsValidName(name);
		}
		public Parameter AddNewParameter(RaisDataType p)
		{
			int n = 1;
			string sbase = "parameter";
			string name = sbase + n.ToString();
			if (_parameters != null)
			{
				bool b = true;
				while (b)
				{
					b = false;
					for (int i = 0; i < _parameters.Length; i++)
					{
						if (_parameters[i].Name == name)
						{
							b = true;
							break;
						}
					}
					if (b)
					{
						n++;
						name = sbase + n.ToString();
					}
				}
			}
			p.Name = name;
			if (_parameters != null)
			{
				n = _parameters.Length;
				Parameter[] a = new Parameter[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = _parameters[i];
				_parameters = a;
			}
			else
			{
				_parameters = new Parameter[1];
				n = 0;
			}
			_parameters[n] = new Parameter(this, p);
			_parameters[n].Variable.MathExpression.Dummy.ResetID(this.MethodID);
			return _parameters[n];
		}
		public Parameter GetParameterByID(UInt32 id)
		{
			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Length; i++)
				{
					if (_parameters[i].ID == id)
					{
						return _parameters[i];
					}
				}
			}
			return null;
		}
		public string GetParameterCodeNameById(UInt32 id)
		{
			Parameter p = GetParameterByID(id);
			if (p != null)
			{
				return p.Variable.CodeVariableName;
			}
			return "";
		}
		protected virtual void OnParameterRemoved(Parameter p)
		{
		}
		protected virtual void OnParameterAdded(Parameter p)
		{
		}
		public void RemoveParameter(UInt32 id)
		{
			Parameter rt = null;
			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Length; i++)
				{
					if (_parameters[i].ID == id)
					{
						rt = _parameters[i];
						if (_parameters.Length == 1)
							_parameters = null;
						else
						{
							Parameter[] a = new Parameter[_parameters.Length - 1];
							for (int k = 0; k < _parameters.Length; k++)
							{
								if (k < i)
									a[k] = _parameters[k];
								else if (k > i)
									a[k - 1] = _parameters[k];
							}
							_parameters = a;
						}
						break;
					}
				}
			}
			if (rt != null)
			{
				OnParameterRemoved(rt);
			}
		}
		private void onIDset()
		{
			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Length; i++)
				{
					_parameters[i].Variable.MathExpression.Dummy.ResetID(_id);
				}
			}
		}
		#endregion
		#region IXmlNodeSerializable Members

		public virtual void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			object v;
			_name = XmlSerialization.GetAttribute(node, XmlSerialization.XMLATT_NAME);
			_static = XmlSerialization.GetAttributeBool(node, XmlSerialization.XMLATT_STATIC);
			MakeUIThreadSafe = XmlSerialization.GetAttributeBool(node, XmlSerialization.XMLATT_UIThreadSafe);
			if (XmlSerialization.ReadValueFromChildNode(node, XmlSerialization.XML_DESCRIPT, out v))
			{
				_desc = (string)v;
			}
			XmlNode nd = node.SelectSingleNode(XmlSerialization.XML_RETURNTYPE);
			if (nd != null)
			{
				_returnType = (RaisDataType)XmlSerialization.ReadFromXmlNode(serializer, nd);
			}
			else
			{
				_returnType = new RaisDataType(typeof(double));
			}
			string qry = XmlSerialization.FormatString("{0}/{1}", XmlSerialization.XML_PARAMLIST, XmlSerialization.XML_PARAM);
			XmlNodeList nodes = node.SelectNodes(qry);
			if (nodes != null && nodes.Count > 0)
			{
				_parameters = new Parameter[nodes.Count];
				for (int i = 0; i < nodes.Count; i++)
				{
					_parameters[i] = (Parameter)XmlSerialization.ReadFromXmlNode(serializer, nodes[i]);
				}
			}
			this.MethodID = XmlSerialization.GetAttributeUInt(node, XmlSerialization.XMLATT_ID);
		}

		public virtual void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (!string.IsNullOrEmpty(_name))
			{
				XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_NAME, _name);
			}
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_ID, this.MethodID);
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_STATIC, _static.ToString());
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_UIThreadSafe, this.MakeUIThreadSafe);
			if (!string.IsNullOrEmpty(_desc))
			{
				XmlSerialization.WriteValueToChildNode(node, XmlSerialization.XML_DESCRIPT, _desc);
			}
			if (_returnType != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, XmlSerialization.XML_RETURNTYPE, _returnType);
			}
			if (_parameters != null && _parameters.Length > 0)
			{
				XmlNode nodePS = node.OwnerDocument.CreateElement(XmlSerialization.XML_PARAMLIST);
				node.AppendChild(nodePS);
				for (int i = 0; i < _parameters.Length; i++)
				{
					XmlSerialization.WriteToChildXmlNode(serializer, nodePS, XmlSerialization.XML_PARAM, _parameters[i]);
				}
			}
		}

		#endregion
	}
}
