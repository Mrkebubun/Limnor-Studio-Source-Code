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
using System.CodeDom;
using MathExp;
using ProgElements;
using System.Xml;
using XmlUtility;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using VPL;
using LimnorDesigner.Action;
using Limnor.WebBuilder;
using System.Globalization;

namespace LimnorDesigner.Event
{
	/// <summary>
	/// a pointer to a EventClass
	/// 1. represent a custom event of an instance of ClassPointer as a member of another ClassPointer;
	/// 2. for EventClass selection
	/// </summary>
	public class CustomEventPointer : IEvent, IXmlNodeSerializable, ICustomObject, ICustomPointer
	{
		#region fields and constructors
		private IClass _holder;
		private UInt64 _wholeId;
		private EventClass _event;
		public CustomEventPointer()
		{
		}
		public CustomEventPointer(EventClass e, IClass holder)
		{
			_event = e;
			_wholeId = e.WholeId;
			_holder = holder;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public EventClass Event
		{
			get
			{
				if (_event == null && _holder != null)
				{
					UInt32 eid = this.MemberId;
					if (eid != 0)
					{
						ClassPointer cp = _holder as ClassPointer;
						if (cp != null)
						{
							_event = cp.GetCustomEventById(eid);
						}
					}
				}
				return _event;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				return _event.ParameterCount;
			}
		}
		[Browsable(false)]
		public List<ParameterClass> GetParameters(IMethod method)
		{
			if (_event == null)
				return new List<ParameterClass>();
			return _event.GetParameters(method);
		}
		#endregion
		#region IEvent Members
		[Browsable(false)]
		public bool IsCustomEvent
		{
			get { return true; }
		}
		[Browsable(false)]
		public bool IsOverride
		{
			get { return _event.IsOverride; }
		}
		[Description("The event handler type of the event")]
		public DataTypePointer EventHandlerType
		{
			get
			{
				return _event.EventHandlerType;
			}
			set
			{
				_event.EventHandlerType = value;
			}
		}
		[Browsable(false)]
		public string ShortDisplayName
		{
			get
			{
				StringBuilder sb = new StringBuilder(Name);
				IObjectPointer op = this.Owner;
				while (op != null && op.Owner != null)
				{
					if (op is IClass)
					{
						break;
					}
					sb.Append(".");
					sb.Append(op.CodeName);
					op = op.Owner;
				}
				return sb.ToString();
			}
		}
		public MethodClass CreateHandlerMethod(object compiler)
		{
			_event.SetHolder(_holder);
			return _event.CreateHandlerMethod(compiler);
		}
		#endregion
		#region Methods
		public void ResetEventClass(EventClass e)
		{
			_event = e;
		}
		public List<NamedDataType> GetEventParameters()
		{
			return _event.GetEventParameters();
		}
		#endregion
		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_event != null)
				{
					if (_event.Owner != null)
					{
						return _event.Owner.RunAt;
					}
				}
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				IObjectPointer root = this.Holder;
				if (root != null)
				{
					return root.RootPointer;
				}
				return null;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IObjectPointer Owner
		{
			get
			{
				if (Event != null)
				{
					return Event.Owner;
				}
				return null;
			}
			set
			{
				if (Event != null)
				{
					Event.Owner = value;
				}
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		[Browsable(false)]
		public Type ObjectType
		{
			get
			{
				if (Event != null)
				{
					return Event.ObjectType;
				}
				return null;
			}
			set
			{
				if (Event != null)
				{
					Event.ObjectType = value;
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectInstance
		{
			get
			{
				if (Event != null)
				{
					return Event.ObjectInstance;
				}
				return null;
			}
			set
			{
				if (Event != null)
				{
					Event.ObjectInstance = value;
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public object ObjectDebug
		{
			get
			{
				if (Event != null)
				{
					return Event.ObjectDebug;
				}
				return null;
			}
			set
			{
				if (Event != null)
				{
					Event.ObjectDebug = value;
				}
			}
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				if (Holder != null)
					return Holder.ReferenceName + "." + Name;
				return Name;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return Name; }
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				if (Event != null)
				{
					return Event.DisplayName;
				}
				return null;
			}
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				return DisplayName;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get
			{
				return this.Name;
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (Event != null)
			{
				return Event.IsTargeted(target);
			}
			return false;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				if (Event != null)
				{
					return Event.ObjectKey;
				}
				return "?";
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				if (Event != null)
				{
					return Event.TypeString;
				}
				return null;
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (Event == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "Event is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
					return false;
				}
				return Event.IsValid;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (Event != null)
			{
				Event.SetCompileHolder(_holder);
				return Event.GetReferenceCode(method, statements, forValue);
			}
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (Event != null)
			{
				Event.SetCompileHolder(_holder);
				return Event.GetJavaScriptReferenceCode(code);
			}
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (Event != null)
			{
				Event.SetCompileHolder(_holder);
				return Event.GetPhpScriptReferenceCode(code);
			}
			return null;
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

		public bool IsSameObjectRef(ProgElements.IObjectIdentity objectIdentity)
		{
			ICustomObject cp = objectIdentity as ICustomObject;
			if (cp != null)
			{
				return (cp.WholeId == this.WholeId);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			ICustomObject cp = p as ICustomObject;
			if (cp != null)
			{
				return (cp.WholeId == this.WholeId);
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (Event != null)
				{
					return Event.IdentityOwner;
				}
				return null;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (Event != null)
				{
					return Event.IsStatic;
				}
				return false;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Custom; }
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Event; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CustomEventPointer ep = new CustomEventPointer(_event, _holder);
			return ep;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer0, XmlNode node)
		{
			XmlObjectWriter writer = (XmlObjectWriter)writer0;
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_EventId, MemberId);
			if (ClassId != writer.ObjectList.ClassId)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ownerClassID, ClassId);
			}
			//_holder:LocalVariable -- inside a methd:MemberId = variable id
			//_holder:ClassPointer -- on the top level
			//_holder:ClassInstancePointer -- member of the parent level, identified by MemberId
			LocalVariable var = _holder as LocalVariable;
			if (var != null)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_varId, _holder.MemberId);
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
			//retrieve _holder, _event
			UInt32 memberId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_EventId);//declaring event id
			MemberId = memberId;
			//
			UInt32 varId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_varId);
			if (varId != 0)
			{
				ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
				LocalVariable lv = root0.GetLocalVariable(varId);
				_holder = lv;
				ClassId = lv.ClassType.DefinitionClassId;
				ClassPointer r = reader.ObjectList.Project.GetTypedData<ClassPointer>(ClassId);
				_event = r.GetCustomEventById(memberId);
			}
			else
			{
				XmlNode nd = node.SelectSingleNode(XmlTags.XML_ClassInstance);
				if (nd != null)
				{
					ClassInstancePointer cp = new ClassInstancePointer();
					cp.OnPostSerialize(reader.ObjectList, nd, false, reader);
					_event = cp.Definition.GetCustomEventById(memberId);
					_holder = cp;
					ClassId = cp.DefinitionClassId;
				}
				else
				{
					UInt32 cid = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ownerClassID);
					if (cid != 0)
					{
						ClassPointer root0 = ClassPointer.CreateClassPointer(cid, reader.ObjectList.Project);
						_holder = root0;
						ClassId = root0.ClassId;
						_event = root0.GetCustomEventById(memberId);
					}
					else
					{
						if (XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID) == 0)
						{
							ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
							_holder = root0;
							ClassId = root0.ClassId;
							_event = root0.GetCustomEventById(memberId);
						}
					}
				}
			}
			if (_event == null)
			{
				//===backward compatibility==================================================
				UInt32 instId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_instanceId);
				if (instId != 0)
				{
					XmlNode nd = node.SelectSingleNode(XmlTags.XML_ClassInstance);
					if (nd != null)
					{
						ClassInstancePointer cp = new ClassInstancePointer();
						cp.OnPostSerialize(reader.ObjectList, nd, false, reader);
						ClassInstancePointer vi = (ClassInstancePointer)cp.Definition.ObjectList.GetClassRefById(instId);
						MemberComponentIdCustomInstance mcci = new MemberComponentIdCustomInstance(cp, vi, instId);
						_holder = mcci;
						_event = vi.Definition.GetCustomEventById(memberId);
						ClassId = vi.ClassId;
					}
					else
					{
						ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
						ClassInstancePointer cp = (ClassInstancePointer)reader.ObjectList.GetClassRefById(instId);
						MemberComponentIdCustom mcc = new MemberComponentIdCustom(root0, cp, instId);
						_holder = mcc;
						_event = cp.Definition.GetCustomEventById(memberId);
						ClassId = cp.ClassId;
					}
				}
				else
				{
					if (XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID) == 0)
					{
						ClassPointer root0 = reader.ObjectList.GetTypedData<ClassPointer>();
						_holder = root0;
						ClassId = root0.ClassId;
						_holder = root0;
						_event = root0.GetCustomEventById(memberId);
					}
					else
					{
						//backward compatibility
						UInt32 classId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID); //declaring class id
						UInt32 holderMemberId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ComponentID); //holding class member id
						ClassId = classId;
						MemberId = memberId;
						UInt32 holderClassId = XmlUtil.GetAttributeUInt(node.OwnerDocument.DocumentElement, XmlTags.XMLATT_ClassID);
						ClassPointer root = reader.ObjectList.Project.GetTypedData<ClassPointer>(holderClassId);
						if (holderClassId == classId) //holder is in the same class
						{
							_event = root.GetCustomEventById(memberId);
							if (holderMemberId == 0 || holderMemberId == reader.ObjectList.MemberId)//not an instance
							{
								_holder = ClassPointer.CreateClassPointer(reader.ObjectList);
							}
							else
							{
								_holder = (IClass)reader.ObjectList.GetClassRefById(holderMemberId);
							}
						}
						else //holder and declarer are different classes
						{
							ClassPointer baseClass = root.GetBaseClass(classId);
							if (baseClass != null)
							{
								EventClass eb = baseClass.GetCustomEventById(memberId);
								if (eb == null)
								{
									throw new DesignerException("Error reading custom event pointer: invalid event Id [{0},{1}] in class [{2}]", classId, memberId, holderClassId);
								}
								_event = eb.CreateInherited(root);
							}
							else
							{
								ClassPointer declarer = ClassPointer.CreateClassPointer(classId, reader.ObjectList.Project);
								_event = declarer.GetCustomEventById(memberId);
							}
							//holder an instance?
							object v = reader.ObjectList.GetObjectByID(holderMemberId);
							if (v != null)
							{
								_holder = reader.ObjectList.GetClassRefByObject(v) as IClass;
							}
							if (_holder == null)
							{
								//holder is a local variable
								//ClassPointer root = reader.ObjectList.GetTypedData<ClassPointer>();
								LocalVariable lv = root.GetLocalVariable(holderMemberId);
								_holder = lv;
							}
						}
					}
				}
			}
			if (_event == null)
			{
				throw new DesignerException("Error reading custom event pointer: [{0}]", node.InnerXml);
			}
			if (_holder == null)
			{
				throw new DesignerException("Invalid custom event pointer. Holder not found. [{0}]", node.InnerXml);
			}
		}

		#endregion

		#region ICustomObject Members
		[Browsable(false)]
		public UInt64 WholeId
		{
			get { return _wholeId; }
		}
		/// <summary>
		/// id of the class defining the event
		/// </summary>
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
		/// <summary>
		/// event id as in the defining class
		/// </summary>
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
				_wholeId = DesignUtil.MakeDDWord((UInt32)value, c);
			}
		}

		public string Name
		{
			get 
			{
				if (Event == null)
					return "?";
				return Event.Name;
			}
		}

		#endregion

		#region ICustomPointer Members
		[Browsable(false)]
		public ClassPointer Declarer
		{
			get { return _event.Declarer; }
		}
		[Browsable(false)]
		public IClass Holder
		{
			get { return _holder; }
		}
		public void SetHolder(IClass holder)
		{
			_holder = holder;
		}
		#endregion
	}
}
