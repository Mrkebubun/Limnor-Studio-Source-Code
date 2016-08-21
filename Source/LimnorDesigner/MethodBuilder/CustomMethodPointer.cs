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
using System.ComponentModel;
using XmlUtility;
using System.Xml;
using ProgElements;
using System.CodeDom;
using VPL;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Action;
using MathExp;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// a reference to a custom method
	/// 1. represent a custom method of an instance of ClassPointer as a member of another ClassPointer;
	/// 2. for MethodClass selection
	/// </summary>
	public class CustomMethodPointer : IXmlNodeSerializable, ICustomObject, ICloneable, IActionMethodPointer, ICustomPointer, IActionCompiler, IObjectPointer
	{
		#region fields and constructors
		private IClass _holder;
		private UInt64 _wholeId;
		private MethodClass _method;
		private IAction _action;
		private CodeExpression[] _parameterExpressions; //for using in math exp compile, not used for this version
		private string[] _parameterJS;
		private string[] _parameterPhp;
		private XmlNode _xmlNode;
		private string _error;
		public CustomMethodPointer()
		{
		}
		public CustomMethodPointer(MethodClass method, IClass holder)
		{
			_method = method;
			_wholeId = DesignUtil.MakeDDWord(_method.MemberId, holder.ClassId);// _method.WholeId;
			_holder = holder;
		}
		#endregion
		#region Properties
		/// <summary>
		/// for using in math exp compile, not used for this version
		/// </summary>
		[Browsable(false)]
		protected CodeExpression[] ParameterCodeExpressions
		{
			get
			{
				return _parameterExpressions;
			}
		}
		[Browsable(false)]
		protected string[] ParameterJS
		{
			get
			{
				return _parameterJS;
			}
		}
		[Browsable(false)]
		public MethodClass MethodDef
		{
			get
			{
				return _method;
			}
		}
		[Browsable(false)]
		public bool IsWebMethod
		{
			get
			{
				if (_method == null)
					return false;
				return _method.IsWebMethod;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_holder != null && _method != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(_holder=[2], _method={3}) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name,_holder,_method);
				return false;
			}
		}
		[Browsable(false)]
		public string ErrorMessage
		{
			get
			{
				return _error;
			}
		}
		#endregion
		#region Methods
		public void ReadLoad(XmlObjectReader reader)
		{
			OnReadFromXmlNode(reader, _xmlNode);
		}
		/// <summary>
		/// for using in math exp compile, not used for this version
		/// </summary>
		/// <param name="ps"></param>
		public void SetParameterExpressions(CodeExpression[] ps)
		{
			_parameterExpressions = ps;
		}
		public void SetParameterJS(string[] ps)
		{
			_parameterJS = ps;
		}
		public void SetParameterPhp(string[] ps)
		{
			_parameterPhp = ps;
		}
		public void SetMethod(MethodClass m)
		{
			_method = m;
		}
		public override string ToString()
		{
			return Name;
		}
		private int GetParameterIndexByName(string name)
		{
			if (_method == null)
				return -1;
			return _method.GetParameterIndexByName(name);
		}
		#endregion
		#region ICustomObject Members

		[Browsable(false)]
		public UInt32 ClassId
		{
			get
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				return c;
			}
			set
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				_wholeId = DesignUtil.MakeDDWord(a, value);
			}
		}
		[Browsable(false)]
		public UInt32 MemberId
		{
			get
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				return a;
			}
			set
			{
				UInt32 c, a;
				DesignUtil.ParseDDWord(_wholeId, out a, out c);
				_wholeId = DesignUtil.MakeDDWord(value, c);
			}
		}
		[Browsable(false)]
		public UInt64 WholeId
		{
			get
			{
				return _wholeId;
			}
		}

		[Browsable(false)]
		public string Name
		{
			get
			{
				if (_method != null)
					return _method.Name;
				return "";
			}
		}

		#endregion
		#region IXmlNodeSerializable Members
		const string XML_Params = "Parameters";
		const string XML_ReturnPointer = "ReturnPointer";
		public void OnWriteToXmlNode(IXmlCodeWriter writer0, XmlNode node)
		{
			XmlObjectWriter writer = (XmlObjectWriter)writer0;
			UInt32 methodClassId = _method.Owner.RootPointer.ClassId;
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_MethodID, MemberId);
			if (methodClassId != writer.ObjectList.ClassId)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ownerClassID, methodClassId);
			}
			//_holder:LocalVariable -- inside a methd:MemberId = variable id
			//_holder:ClassPointer -- on the top level
			//_holder:ClassInstancePointer -- member of the parent level, identified by MemberId
			LocalVariable var = Holder as LocalVariable;
			if (var != null)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_varId, Holder.MemberId);
			}
			else
			{
				ClassInstancePointer cp = _holder as ClassInstancePointer;
				if (cp != null)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_ClassInstance);
					cp.OnPostSerialize(writer.ObjectList, nd, true, writer);
				}
				else
				{
					MemberComponentIdCustomInstance mcci = this.Holder as MemberComponentIdCustomInstance;
					if (mcci != null)
					{
						XmlUtil.SetAttribute(node, XmlTags.XMLATT_instanceId, mcci.Pointer.MemberId);
						cp = (ClassInstancePointer)(mcci.Owner);
						XmlNode nd = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_ClassInstance);
						cp.OnPostSerialize(writer.ObjectList, nd, true, writer);
					}
					else
					{
						MemberComponentIdCustom mcc = this.Holder as MemberComponentIdCustom;
						if (mcc != null)
						{
							XmlUtil.SetAttribute(node, XmlTags.XMLATT_instanceId, mcc.Pointer.MemberId);
						}
						else
						{
						}
					}
				}
			}
		}
		/// <summary>
		/// called from the holding class
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="node"></param>
		public void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			XmlObjectReader reader = (XmlObjectReader)reader0;
			_xmlNode = node;
			//retrieve _holder, _method
			//method id
			UInt32 memberId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_MethodID);
			MemberId = memberId; //method id
			//
			UInt32 varId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_varId);
			if (varId == 0)
			{
				//try backward compatible
				varId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ComponentID);
				if (varId != 0)
				{
					ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
					LocalVariable lv = root0.GetLocalVariable(varId);
					if (lv == null)
					{
						varId = 0;
					}
					else
					{
						_holder = lv;
						ClassId = lv.ClassType.DefinitionClassId;
						ClassPointer r = reader.ObjectList.Project.GetTypedData<ClassPointer>(ClassId);
						_method = r.GetCustomMethodById(memberId);
					}
				}
			}
			if (varId != 0)
			{
				ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
				LocalVariable lv = root0.GetLocalVariable(varId);
				_holder = lv;
				ClassId = lv.ClassType.DefinitionClassId;
				ClassPointer r = reader.ObjectList.Project.GetTypedData<ClassPointer>(ClassId);
				_method = r.GetCustomMethodById(memberId);
			}
			else
			{
				XmlNode nd = node.SelectSingleNode(XmlTags.XML_ClassInstance);
				if (nd != null)
				{
					ClassInstancePointer cp = new ClassInstancePointer();
					cp.OnPostSerialize(reader.ObjectList, nd, false, reader);
					if (cp.Definition == null)
					{
						cp.ReplaceDeclaringClassPointer(ClassPointer.CreateClassPointer(cp.DefinitionClassId, reader.ObjectList.Project));
					}
					_method = cp.Definition.GetCustomMethodById(memberId);
					_holder = cp;
					ClassId = cp.DefinitionClassId;
				}
				else
				{
					UInt32 cid = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ownerClassID);
					if (cid != 0)
					{
						ClassPointer root0 = ClassPointer.CreateClassPointer(cid, reader.ObjectList.Project);
						varId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_instanceId);
						if (varId != 0)
						{
							ClassPointer root = reader.ObjectList.GetTypedData<ClassPointer>();
							_holder = new ClassInstancePointer(root, root0, varId);
							ClassId = root.ClassId;
						}
						else
						{
							_holder = root0;
							ClassId = root0.ClassId;
						}
						_method = root0.GetCustomMethodById(memberId);
					}
					else
					{
						if (XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID) == 0)
						{
							ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
							_holder = root0;
							ClassId = root0.ClassId;
							_method = root0.GetCustomMethodById(memberId);
						}
					}
				}
			}
			if (_method == null)
			{
				//===backward compatibility========================================
				UInt32 classId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID);
				ClassId = classId;
				UInt32 instId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_instanceId);
				if (instId != 0)
				{
					if (classId != 0 && classId != reader.ObjectList.ClassId)
					{
						//it is a static instance from another class
						//for a custom class the holder is a MemberComponentIdCustom
						//for a library class the holder is a 
						ClassPointer cp0 = ClassPointer.CreateClassPointer(classId, reader.ObjectList.Project);
						if (cp0 == null)
						{
							_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Class {0} not found in the project", classId);
						}
						else
						{
							if (cp0.ObjectList.Count == 0)
							{
								//cp0 not loaded. load it now
								cp0.ObjectList.LoadObjects();
								if (cp0.ObjectList.Reader.HasErrors)
								{
									MathNode.Log(cp0.ObjectList.Reader.Errors);
									cp0.ObjectList.Reader.ResetErrors();
								}
							}
							IClassRef ic = cp0.ObjectList.GetClassRefById(instId);
							if (ic == null)
							{
								_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Instance {0} not found in Class {1}", instId, classId);
							}
							else
							{
								ClassInstancePointer vi = ic as ClassInstancePointer;
								if (vi == null)
								{
									_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Instance {0},{1} in Class {2} is not a class-instance-pointer", instId, ic.GetType(), classId);
								}
								else
								{
									MemberComponentIdCustom mc = new MemberComponentIdCustom(cp0, vi, instId);
									_holder = mc;
									_method = vi.Definition.GetCustomMethodById(memberId);
								}
							}
						}
					}
					else
					{
						XmlNode nd = node.SelectSingleNode(XmlTags.XML_ClassInstance);
						if (nd != null)
						{
							ClassInstancePointer cp = new ClassInstancePointer();
							cp.OnPostSerialize(reader.ObjectList, nd, false, reader);
							ClassInstancePointer vi = (ClassInstancePointer)cp.Definition.ObjectList.GetClassRefById(instId);
							MemberComponentIdCustomInstance mcci = new MemberComponentIdCustomInstance(cp, vi, instId);
							_holder = mcci;
							_method = vi.Definition.GetCustomMethodById(memberId);
							ClassId = vi.ClassId;
						}
						else
						{
							ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
							ClassInstancePointer cp = (ClassInstancePointer)reader.ObjectList.GetClassRefById(instId);
							if (cp != null)
							{
								MemberComponentIdCustom mcc = new MemberComponentIdCustom(root0, cp, instId);
								_holder = mcc;
								_method = cp.Definition.GetCustomMethodById(memberId);
								ClassId = cp.ClassId;
							}
							else
							{
								//try to fix the error
								foreach (object o in reader.ObjectList.Keys)
								{
									cp = reader.ObjectList.GetClassRefByObject(o) as ClassInstancePointer;
									if (cp != null)
									{
										_method = cp.Definition.GetCustomMethodById(memberId);
										if (_method != null)
										{
											instId = reader.ObjectList.GetObjectID(o);
											_holder = new MemberComponentIdCustom(root0, cp, instId);
											ClassId = cp.ClassId;
											//fix the instance id
											XmlUtil.SetAttribute(node, XmlTags.XMLATT_instanceId, instId);
											break;
										}
									}
								}
								if (_holder == null)
								{
									_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Component {0} is not a customer class instance in class {1}", instId, root0.ClassId);
								}
							}
						}
					}
				}
				else
				{
					if (classId == 0)
					{
						ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
						_holder = root0;
						_method = root0.GetCustomMethodById(memberId);
					}
					else
					{
						ClassPointer root0 = ClassPointer.CreateClassPointer(classId, reader.ObjectList.Project);
						if (root0 != null)
						{
							_holder = root0;
							_method = root0.GetCustomMethodById(memberId);
						}
						if (_method == null)
						{
							//backward compatibility
							UInt32 holderMemberId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ComponentID);
							ClassPointer root = reader.ObjectList.GetTypedData<ClassPointer>();
							UInt32 holderClassId = XmlUtil.GetAttributeUInt(node.OwnerDocument.DocumentElement, XmlTags.XMLATT_ClassID);
							if (holderClassId == classId) //holder is in the same class
							{
									_method = root.GetCustomMethodById(memberId);
									if (holderMemberId == 0 || holderMemberId == reader.ObjectList.MemberId)//not an instance
									{
										_holder = root;
									}
									else
									{
										_holder = (IClass)reader.ObjectList.GetClassRefById(holderMemberId);
									}
									if (_holder == null)
									{
										_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid method holder Id [{0},{1}] for method [{2}.{3}]", classId, holderMemberId, _method.Owner, _method.Name);
									}
							}
							else //holder and declarer are different classes
							{
								ClassPointer declarer = ClassPointer.CreateClassPointer(classId, reader.ObjectList.Project);
								_method = declarer.GetCustomMethodById(memberId);
								if (_method == null)
								{
									_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Method [{0}] not found in class [{1}]", memberId, classId);
								}
								else
								{

									//holder an instance?
									object v = reader.ObjectList.GetObjectByID(holderMemberId);
									if (v != null)
									{
										if (v == reader.ObjectList.GetRootObject())
										{
											_holder = reader.ObjectList.GetTypedData<ClassPointer>();
										}
										else
										{
											_holder = reader.ObjectList.GetClassRefByObject(v) as IClass;
										}
									}
									if (_holder == null)
									{
										//holder is a local variable
										LocalVariable lv = root.GetLocalVariable(holderMemberId);
										_holder = lv;
									}
									if (_holder == null)
									{
										_error = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid method holder Id [{0},{1}] for method [{2}.{3}] from declaring class {4}", holderClassId, holderMemberId, _method.Owner, _method.Name, classId);
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CustomMethodPointer cp = new CustomMethodPointer(_method, _holder);
			return cp;
		}

		#endregion

		#region IObjectPointer Members
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				IObjectPointer root = this.Holder;
				while (root != null && root.Owner != null)
				{
					root = root.Owner;
				}
				ClassPointer c = root as ClassPointer;
				return c;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
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
		public string ReferenceName
		{
			get
			{
				if (Holder != null)
					return Holder.ReferenceName + "." + Name;
				return CodeName;
			}
		}
		public string ExpressionDisplay
		{
			get
			{
				if (_method != null)
					return _method.MethodName;
				return "?";
			}
		}
		public bool IsStatic
		{
			get
			{
				if (_method != null)
					return _method.IsStatic;
				return false;
			}
		}

		public IObjectPointer Owner
		{
			get
			{
				if (_method != null)
					return _method.Owner;
				return null;
			}
			set
			{
				if (_method != null)
					_method.Owner = value;
			}
		}
		public IPropertyPointer PropertyOwner { get { return Owner; } }

		public Type ObjectType
		{
			get
			{
				if (_method != null)
					return _method.ObjectType;
				return null;
			}
			set
			{
				if (_method != null)
					_method.ObjectType = value;
			}
		}

		public object ObjectInstance
		{
			get
			{
				if (_method != null)
					return _method.ObjectInstance;
				return null;
			}
			set
			{
				if (_method != null)
					_method.ObjectInstance = value;
			}
		}

		public object ObjectDebug
		{
			get
			{
				if (_method != null)
					return _method.ObjectDebug;
				return null;
			}
			set
			{
				if (_method != null)
					_method.ObjectDebug = value;
			}
		}

		public string DisplayName
		{
			get
			{
				if (_method != null)
					return _method.DisplayName;
				return "";
			}
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (_method != null)
				return _method.IsTargeted(target);
			return false;
		}

		public string ObjectKey
		{
			get
			{
				if (_method != null)
					return _method.ObjectKey;
				return "";
			}
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get
			{
				return ObjectKey;
			}
		}
		public string TypeString
		{
			get
			{
				if (_method != null)
					return _method.TypeString;
				return "";
			}
		}
		/// <summary>
		/// for using in math exp compile, not used for this version
		/// </summary>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <param name="forValue"></param>
		/// <returns></returns>
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_method == null)
			{
				return null;
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression();
			cmi.Method = (CodeMethodReferenceExpression)_method.GetReferenceCode(method, statements, false);
			if (_parameterExpressions != null)
			{
				cmi.Parameters.AddRange(_parameterExpressions);
			}
			else
			{
				if (_method.ParameterCount > 0)
				{
					throw new DesignerException("Calling CustomMethodPointer.GetReferenceCode without setting parameters");
				}
			}
			return cmi;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_method == null)
			{
				return null;
			}
			bool isCrossPage = false;
			if (this.RootPointer != null && _method.RootPointer != null)
			{
				isCrossPage = this.RootPointer.ClassId != _method.RootPointer.ClassId;
			}
			StringBuilder m = new StringBuilder();
			if (isCrossPage)
			{
				m.Append("JsonDataBinding.executeByPageId(");
				m.Append(_method.RootPointer.ClassId);
				m.Append(",'");
				m.Append(_method.MethodName);
				m.Append("'");
				if (_parameterJS != null && _parameterJS.Length > 0)
				{
					m.Append(",");
				}
			}
			else
			{
				m.Append(_method.MethodName);
				m.Append("(");
			}
			if (_parameterJS != null && _parameterJS.Length > 0)
			{
				m.Append(_parameterJS[0]);
				for (int i = 1; i < _parameterJS.Length; i++)
				{
					m.Append(",");
					m.Append(_parameterJS[i]);
				}
			}
			m.Append(")");
			return m.ToString();
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_method == null)
			{
				return null;
			}
			StringBuilder m = new StringBuilder();
			m.Append("$this->");
			m.Append(_method.MethodName);
			m.Append("(");
			if (_parameterPhp != null && _parameterPhp.Length > 0)
			{
				m.Append(_parameterPhp[0]);
				for (int i = 1; i < _parameterPhp.Length; i++)
				{
					m.Append(",");
					m.Append(_parameterPhp[i]);
				}
			}
			m.Append(")");
			return m.ToString();
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			if (_method != null)
				return _method.IsSameObjectRef(objectIdentity);
			return false;
		}
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectIdentity = p as IObjectIdentity;
			if (objectIdentity != null)
			{
				if (_method != null)
					return _method.IsSameObjectRef(objectIdentity);
			}
			return false;
		}
		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_method != null)
					return _method.IdentityOwner;
				return null;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Custom; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Method; } }
		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IMethod Members
		public object ModuleProject
		{
			get
			{
				if (_method != null && _method.Project != null)
				{
					return _method.Project;
				}
				if (_holder != null && _holder.RootPointer != null)
				{
					return _holder.RootPointer.Project;
				}
				return null;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return true; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }

		[Browsable(false)]
		public Type ActionBranchType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public virtual Type ActionType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public bool NoReturn
		{
			get
			{
				if (_method == null)
				{
					return true;
				}
				return !_method.HasReturn;
			}
		}
		[Browsable(false)]
		public bool HasReturn
		{
			get
			{
				if (_method == null)
				{
					return false;
				}
				return _method.HasReturn;
			}
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (_method != null)
					return _method.ParameterCount;
				return 0;
			}
		}
		[Browsable(false)]
		public List<ParameterClass> Parameters
		{
			get
			{
				if (_method != null)
					return _method.Parameters;
				return null;
			}
		}
		public string MethodName
		{
			get
			{
				if (_method != null)
					return _method.MethodName;
				return "?";
			}
			set
			{
				if (_method != null)
					_method.MethodName = value;
			}
		}
		[Browsable(false)]
		public string DefaultActionName
		{
			get
			{
				return Holder.Name + "." + this.Name;
			}
		}
		public IParameter MethodReturnType
		{
			get
			{
				if (_method != null)
					return _method.MethodReturnType;
				return null;
			}
			set
			{
				if (_method != null)
					_method.MethodReturnType = value;
			}
		}

		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				if (_method != null)
					return _method.MethodParameterTypes;
				return null;
			}
		}
		public bool IsSameMethod(IMethodPointer method)
		{
			CustomMethodPointer cp = method as CustomMethodPointer;
			if (cp != null)
			{
				return (cp.WholeId == this.WholeId);
			}
			return false;
		}
		public void ValidateParameterValues(ParameterValueCollection parameters)
		{
			if (_method != null)
			{
				List<ParameterClass> ps = _method.Parameters;
				ParameterValueCollection.ValidateParameterValues(parameters, ps, this);
			}
		}
		public string ParameterName(int i)
		{
			if (_method == null)
				return string.Empty;
			return _method.Parameters[i].Name;
		}
		public Dictionary<string, string> GetParameterDescriptions()
		{
			if (_method != null)
			{
				List<ParameterClass> ps = _method.Parameters;
				if (ps != null)
				{
					Dictionary<string, string> desc = new Dictionary<string, string>();
					foreach (ParameterClass p in ps)
					{
						if (string.IsNullOrEmpty(p.Description))
							desc.Add(p.Name, p.Name);
						else
							desc.Add(p.Name, p.Description);
					}
					return desc;
				}
			}
			return null;
		}
		public object GetParameterType(UInt32 id)
		{
			if (_method == null)
				return typeof(object);
			return _method.GetParameterByID(id);
		}
		public object GetParameterTypeByIndex(int idx)
		{
			if (_method == null)
				return typeof(object);
			return _method.GetParameterTypeByIndex(idx);
		}
		public void SetParameterTypeByIndex(int idx, object type)
		{
			if (_method != null)
			{
				_method.SetParameterTypeByIndex(idx, type);
			}
		}
		public object GetParameterType(string name)
		{
			if (_method == null)
				return typeof(object);
			return _method.GetParameterByID(name);
		}
		#endregion

		#region IActionCompiler Members
		public void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (_method == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				sb.Add(returnReceiver);
				sb.Add("=");
			}
			sb.Add(_method.Name);
			sb.Add("(");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Add(parameters[0]);
				for (int i = 1; i < parameters.Count; i++)
				{
					sb.Add(",");
					sb.Add(parameters[i]);
				}
			}
			sb.Add(");\r\n");
		}
		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (_method == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				sb.Add(returnReceiver);
				sb.Add("=");
			}
			sb.Add("$this->");
			sb.Add(_method.Name);
			sb.Add("(");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Add(parameters[0]);
				for (int i = 1; i < parameters.Count; i++)
				{
					sb.Add(",");
					sb.Add(parameters[i]);
				}
			}
			sb.Add(");\r\n");
		}
		public virtual void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			if (_method == null)
			{
				return;
			}
			CodeExpression cmi = null;
			if (_method != null)
			{
				_method.SetHolder(_holder);
				CodeMethodReferenceExpression mref = (CodeMethodReferenceExpression)_method.GetReferenceCode(methodToCompile, statements, false);
				CodeMethodInvokeExpression cmim = new CodeMethodInvokeExpression();
				cmim.Method = mref;
				cmim.Parameters.AddRange(parameters);
				cmi = cmim;
			}

			bool useOutput = false;

			if (!NoReturn && nextAction != null && nextAction.UseInput)
			{
				CodeVariableDeclarationStatement output = new CodeVariableDeclarationStatement(
					currentAction.OutputType.TypeString, currentAction.OutputCodeName, cmi);
				statements.Add(output);
				cmi = new CodeVariableReferenceExpression(currentAction.OutputCodeName);

				useOutput = true;
			}
			if (HasReturn && returnReceiver != null)
			{
				CodeExpression cr = returnReceiver.GetReferenceCode(methodToCompile, statements, true);
				if (_method.ReturnValue != null)
				{
					Type target;
					IClassWrapper wrapper = returnReceiver as IClassWrapper;
					if (wrapper != null)
					{
						target = wrapper.WrappedType;
					}
					else
					{
						target = returnReceiver.ObjectType;
					}
					Type dt;
					if (useOutput)
						dt = currentAction.OutputType.BaseClassType;
					else
						dt = _method.ReturnValue.BaseClassType;
					CompilerUtil.CreateAssignment(cr, target, cmi, dt, statements, true);
				}
				else
				{
					CodeAssignStatement cas = new CodeAssignStatement(cr, cmi);
					statements.Add(cas);
				}
			}
			else
			{
				if (!useOutput)
				{
					CodeExpressionStatement ces = new CodeExpressionStatement(cmi);
					statements.Add(ces);
				}
			}
		}

		#endregion

		#region ICustomPointer Members
		[Browsable(false)]
		public IClass Holder
		{
			get { return _holder; }
		}

		/// <summary>
		/// the instance of the method declarer
		/// </summary>
		[Browsable(false)]
		public ClassPointer Declarer
		{
			get
			{
				if (_method == null)
					return null;
				return _method.Declarer;
			}
		}

		public void SetHolder(IClass holder)
		{
			_holder = holder;
		}
		#endregion

		#region IActionMethodPointer Members
		public bool HasFormControlParent
		{
			get
			{
				return _method.MakeUIThreadSafe;
			}
		}
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			IParameter p = MethodParameterTypes[i];
			ParameterValue pv = new ParameterValue(this.Action);
			pv.Name = p.Name;
			pv.ParameterID = p.ParameterID;
			ParameterClass pc = p as ParameterClass;
			if (pc != null)
			{
				pv.SetDataType(pc);
			}
			else
			{
				pv.SetDataType(p.ParameterLibType);
			}
			pv.SetOwnerAction(_action);
			pv.ValueType = EnumValueType.ConstantValue;
			return pv;
		}
		public IMethod MethodPointed
		{
			get { return _method; }
		}
		public IObjectPointer MethodDeclarer { get { return Declarer; } }
		public bool IsArrayMethod
		{
			get
			{
				return false;
			}
		}

		public Type ReturnBaseType
		{
			get
			{
				return this.MethodReturnType.ParameterLibType;
			}
		}
		public virtual EnumWebRunAt RunAt
		{
			get
			{
				if (_method == null)
					return EnumWebRunAt.Unknown;
				if (_method.WebUsage == EnumMethodWebUsage.Client)
					return EnumWebRunAt.Client;
				return EnumWebRunAt.Server;
			}
		}
		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethod method)
		{
			MethodClass mc = method as MethodClass;
			if (mc != null)
			{
				return mc.IsSameMethod(_method);
			}
			return false;
		}

		#endregion

		#region IObjectPointer Members


		public virtual string LongDisplayName
		{
			get
			{
				if (_method != null)
				{
					return _method.LongDisplayName;
				}
				return this.Name;
			}
		}

		#endregion
	}
}
