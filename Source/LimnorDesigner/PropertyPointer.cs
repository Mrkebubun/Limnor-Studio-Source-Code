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
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;
using System.Drawing.Design;
using XmlSerializer;
using DynamicEventLinker;
using System.Xml;
using MathExp;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using ProgElements;
using System.CodeDom;
using VPL;
using LimnorDesigner.Property;
using Parser;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Action;
using LimnorDesigner.Event;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Interface;
using LimnorDesigner.ResourcesManager;
using VSPrj;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using System.Xml.Serialization;
using Limnor.WebServerBuilder;
using XmlUtility;
using LimnorDatabase;
using LFilePath;
using LimnorDesigner.Web;
using Limnor.Drawing2D;

namespace LimnorDesigner
{
	public interface IDataScope
	{
		Type ScopeDataType { get; set; }
		IObjectPointer ScopeOwner { get; set; }
	}
	public interface IOwnerScope
	{
		DataTypePointer OwnerScope { get; }
	}
	/// <summary>
	/// represent P/M/E/F from library. Holder/Declarer are calculated by walking through its Owner.
	/// the Owner is the member owner.
	/// _memberName identifies the member.
	/// the derived classes override Copy to implement Clone.
	/// </summary>
	[SaveAsProperties]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class MemberPointer : IObjectPointer, IMemberPointer
	{
		#region fields and constructors
		private IObjectPointer _owner;
		private string _memberName;
		private string _key;
		public MemberPointer()
		{
		}
		#endregion
		#region Serializable properties
		[Browsable(false)]
		[Description("Name for property/event/method/parameter")]
		public string MemberName
		{
			get
			{
				return _memberName;
			}
			set
			{
				_memberName = value;
			}
		}
		[SaveAsProperties]
		[Browsable(false)]
		public virtual IObjectPointer Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				PropertyClass p = value as PropertyClass;
				if (p != null)
				{
					_owner = p.CreatePointer();
				}
				else
				{
					_owner = value;
				}
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				return Owner;
			}
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public abstract bool IsStatic { get; }
		#endregion
		#region methods
		public void SetObjectKey(string key)
		{
			_key = key;
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(_memberName))
			{
				if (Owner == null)
					return "?.?";
				return Owner.ToString() + ".?";
			}
			return Owner.ToString() + "." + _memberName;
		}
		#endregion
		#region ICloneable Members
		/// <summary>
		/// copies the properties of obj onto this.
		/// it must call base version first.
		/// </summary>
		/// <param name="obj"></param>
		protected abstract void OnCopy(MemberPointer obj);
		private void copy(MemberPointer obj)
		{
			if (obj.Owner != null)
			{
				_owner = (IObjectPointer)obj.Owner.Clone();
			}
			this._memberName = obj._memberName;
			this._key = obj._key;
			OnCopy(obj);
		}
		public object Clone()
		{
			MemberPointer obj = (MemberPointer)Activator.CreateInstance(this.GetType());
			obj.copy(this);
			return obj;
		}

		#endregion
		#region IObjectPointer Members
		public abstract EnumWebRunAt RunAt { get; }
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public virtual ClassPointer RootPointer
		{
			get
			{
				IClass c = this.Holder;
				if (c != null)
				{
					return c.RootPointer;
				}
				IObjectPointer root = this.Owner;
				if (root != null)
				{
					return root.RootPointer;
				}
				return null;
			}
		}
		/// <summary>
		/// variable name
		/// </summary>
		[Browsable(false)]
		public virtual string CodeName
		{
			get
			{
				ICustomCodeName ccn = ObjectInstance as ICustomCodeName;
				if (ccn != null)
				{
					return ccn.CodeName;
				}
				return MemberName;
			}
		}
		/// <summary>
		/// fully qualified variable name
		/// </summary>
		[Browsable(false)]
		public virtual string ReferenceName
		{
			get
			{
				if (Owner != null)
				{
					if (string.IsNullOrEmpty(MemberName))
						return Owner.ReferenceName + ".?";
					return Owner.ReferenceName + "." + MemberName;
				}
				if (string.IsNullOrEmpty(MemberName))
					return "?";
				return MemberName;
			}
		}
		[Browsable(false)]
		public abstract bool IsValid { get; }
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug { get; set; }
		/// <summary>
		/// CodeExpress referencing this pointer
		/// </summary>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <param name="forValue"></param>
		/// <returns></returns>
		public abstract CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue);
		public abstract void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver);
		public abstract void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver);
		public abstract string GetJavaScriptReferenceCode(StringCollection code);
		public abstract string GetPhpScriptReferenceCode(StringCollection code);

		public abstract bool IsTargeted(EnumObjectSelectType target);
		public abstract Type ObjectType { get; set; }
		[Browsable(false)]
		public abstract object ObjectInstance { get; set; }
		[Browsable(false)]
		public abstract string TypeString { get; }
		[Browsable(false)]
		public abstract EnumObjectDevelopType ObjectDevelopType { get; }
		[Browsable(false)]
		public abstract EnumPointerType PointerType { get; }
		public virtual bool IsSameObjectRef(IObjectIdentity obj)
		{
			if (Owner != null)
			{
				MemberPointer mp = obj as MemberPointer;
				if (mp != null)
				{
					if (_memberName == mp.MemberName)
					{
						if (Owner.IsSameObjectRef(mp.Owner))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(ISourceValuePointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		[Browsable(false)]
		public virtual string DisplayName
		{
			get
			{
				if (this.ObjectType == null)
				{
					return _memberName;
				}
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", _memberName, this.ObjectType.Name);
			}
		}
		[Browsable(false)]
		public virtual string LongDisplayName
		{
			get
			{
				if (_owner == null)
				{
					return DisplayName;
				}
				else
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", _owner.CodeName, _memberName);
				}
			}
		}
		[Browsable(false)]
		public virtual string ExpressionDisplay
		{
			get
			{
				if (string.IsNullOrEmpty(_memberName))
				{
					if (Owner != null)
					{
						if (Owner.Owner == null)
						{
							return "{?}";
						}
						else
						{
							return string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.?", Owner.ExpressionDisplay);
						}
					}
					else
					{
						return "{?}";
					}
				}
				else
				{
					if (Owner != null)
					{
						if (Owner.Owner == null)
						{
							CustomMethodParameterPointer cmpp = Owner as CustomMethodParameterPointer;
							if (cmpp != null)
							{
								return string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.{1}", cmpp.CodeName, _memberName);
							}
							else
							{
								return _memberName;
							}
						}
						else
						{
							return string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}.{1}", Owner.ExpressionDisplay, _memberName);
						}
					}
					else
					{
						return _memberName;
					}
				}
			}
		}
		[Browsable(false)]
		public string ManualObjectKey
		{
			get
			{
				return _key;
			}
		}
		[Browsable(false)]
		public virtual string ObjectKey
		{
			get
			{
				if (string.IsNullOrEmpty(_key))
				{
					string s;
					MemberComponentId mci = Owner as MemberComponentId;
					if (mci != null)
					{
						s = mci.Name;
					}
					else
					{
						s = Owner.ObjectKey;
					}
					if (string.IsNullOrEmpty(_memberName))
					{
						return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.?", s);
					}
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", s, _memberName);
				}
				return _key;
			}
		}
		#endregion
		#region ISerializerProcessor Members

		public virtual void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IMemberPointer Members

		[Browsable(false)]
		public ClassPointer Declarer
		{
			get
			{
				ClassInstancePointer cip = this.Owner as ClassInstancePointer;
				if (cip != null)
				{
					if (cip.Definition == null)
					{
						cip.ReplaceDeclaringClassPointer(ClassPointer.CreateClassPointer(cip.DefinitionClassId, cip.RootHost.Project));
					}
					return cip.Definition;
				}
				ClassPointer ic = null;
				IObjectPointer ip = this.Owner;
				while (ip != null)
				{
					ic = ip as ClassPointer;
					if (ic != null)
					{
						break;
					}
					IMemberPointer p = ip as IMemberPointer;
					if (p != null)
					{
						return p.Declarer;
					}
					ip = ip.Owner;
				}
				return ic;
			}
		}
		[Browsable(false)]
		public IClass Holder
		{
			get
			{
				IObjectPointer o = Owner;
				while (o != null)
				{
					IClass c = o as IClass;
					if (c != null)
					{
						return c;
					}
					IMemberPointer p = o as IMemberPointer;
					if (p != null)
					{
						return p.Holder;
					}
					o = o.Owner;
				}
				return null;
			}
		}
		public void SetHolder(IClass holder)
		{
			ActionBranchParameterPointer abpp = holder as ActionBranchParameterPointer;
			if (abpp != null)
			{
				ActionBranchParameter abp = _owner as ActionBranchParameter;
				if (abp != null)
				{
					_owner = abpp;
				}
			}
			//_owner = holder;
		}
		#endregion
	}
	/// <summary>
	/// Indexers are treated as methods not properties.
	/// Use PropertyDescriptor, not PropertyInfo, so that customer property descriptors can be honored
	/// </summary>
	//[Serializable]
	public class PropertyPointer : MemberPointer, IPropertyEx, ISourceValuePointer
	{
		#region fields and constructors
		private Type _scope;
		private PropertyDescriptor _propDesc;
		private Type _codeType;
		private UInt32 _taskId;
		private EnumWebRunAt _runAt = EnumWebRunAt.Unknown;
		private DataTypePointer _propType = null;
		public PropertyPointer()
		{
		}
		#endregion
		#region properties
		[Browsable(false)]
		public override string CodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_JS)
				{
					IJavascriptPropertyHolder iwc = this.Owner.ObjectInstance as IJavascriptPropertyHolder;
					if (iwc != null)
					{
						string cn = iwc.GetJavascriptPropertyCodeName(this.MemberName);
						if (!string.IsNullOrEmpty(cn))
						{
							return cn;
						}
					}
					IScriptCodeName scn = this.ObjectInstance as IScriptCodeName;
					if (scn != null)
					{
						return scn.GetJavascriptCodeName();
					}
					if (this.ObjectType != null && this.ObjectType.GetInterface("IWebClientControl") != null)
					{
						return base.CodeName;
					}
					else
					{
						IJavascriptVariable ijv = this.ObjectInstance as IJavascriptVariable;
						if (ijv != null)
						{
							return ijv.GetVarName();
						}
						else
						{
							if (this.Owner != null)
							{
								string mn = this.MemberName;
								if (string.CompareOrdinal(MemberName, "Length") == 0)
								{
									if (Owner.ObjectType != null && Owner.ObjectType.IsArray)
									{
										mn = "length";
									}
								}
								IFieldList fl = Owner.ObjectInstance as IFieldList;
								if (fl != null)
								{
									return mn;
								}
								return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Owner.CodeName, mn);
							}
						}
						return MemberName;
					}
				}
				else if (VPLUtil.CompilerContext_PHP)
				{
					IScriptCodeName scn = this.ObjectInstance as IScriptCodeName;
					if (scn != null)
					{
						return scn.GetPhpCodeName();
					}
				}
				return base.CodeName;
			}
		}
		[Browsable(false)]
		public bool IsSessionvariable
		{
			get
			{
				PropertyDescriptor p = Info;
				if (p != null)
				{
					if (p is SessionVariableCollection.PropertyDescriptorSessionVariable)
					{
						return true;
					}
				}
				if (Owner != null)
				{
					if (typeof(SessionVariableCollection).Equals(Owner.ObjectType))
					{
						return true;
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public Type CodeType
		{
			get
			{
				return _codeType;
			}
		}
		[Browsable(false)]
		public override string LongDisplayName
		{
			get
			{
				if (Owner == null)
				{
					return DisplayName;
				}
				else
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", Owner.DisplayName, MemberName);
				}
			}
		}
		[Browsable(false)]
		public bool IsFinal
		{
			get
			{
				IPropertyDescriptor pd = _propDesc as IPropertyDescriptor;
				if (pd != null)
				{
					return pd.IsFinal;
				}
				return true;
			}
		}
		[Browsable(false)]
		public virtual bool IsReadOnlyForProgramming
		{
			get
			{
				AttributeCollection ac = Info.Attributes;
				if (ac != null && ac.Count > 0)
				{
					foreach (Attribute a in ac)
					{
						ReadOnlyInProgrammingAttribute r = a as ReadOnlyInProgrammingAttribute;
						if (r != null)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public virtual bool IsReadOnly
		{
			get
			{
				if (Info.IsReadOnly)
				{
					ReadOnlyAttribute ra = null;
					XmlIgnoreAttribute xa = null;
					foreach (Attribute a in Info.Attributes)
					{
						if (ra == null)
						{
							ra = a as ReadOnlyAttribute;
						}
						if (xa == null)
						{
							xa = a as XmlIgnoreAttribute;
						}
						if (ra != null && xa != null)
						{
							break;
						}
					}
					if (xa == null || ra == null)
					{
						return true;
					}
					if (ra.IsReadOnly)
					{
						return true;
					}
				}
				PropertyPointer pp = this.Owner as PropertyPointer;
				if (pp != null)
				{
					if (pp.ObjectType.IsValueType)
					{
						return true;
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public virtual PropertyDescriptor Info
		{
			get
			{
				if (_propDesc == null && !string.IsNullOrEmpty(MemberName))
				{
					if (Owner == null)
					{
						throw new DesignerException("PropertyPointer {0} missing owner", MemberName);
					}
					if (Owner.ObjectInstance != null)
					{
						Type t = Owner.ObjectInstance as Type;
						if (t != null)
						{
							PropertyInfo p = Owner.ObjectType.GetProperty(this.MemberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
							if (p != null)
							{
								StaticPropertyDescriptor2 tp = new StaticPropertyDescriptor2(t, this.MemberName, null, p);
								_propDesc = tp;
							}
						}
						else
						{
							_propDesc = VPLUtil.GetProperty(Owner.ObjectInstance, this.MemberName);
						}
					}
					if (_propDesc == null && Owner.ObjectType != null)
					{
						Type t = VPLUtil.GetCoClassType(Owner.ObjectType);
						if (t == null)
						{
							t = Owner.ObjectType;
						}

						PropertyInfo p = null;
						try
						{
							p = VPLUtil.GetPropertyInfo(t, this.MemberName);
						}
						catch (Exception err)
						{
							PropertyInfo[] pifs = t.GetProperties();
							if (pifs != null && pifs.Length > 0)
							{
								for (int i = 0; i < pifs.Length; i++)
								{
									if (string.CompareOrdinal(this.MemberName, pifs[i].Name) == 0)
									{
										p = pifs[i];
										break;
									}
								}
							}
							if (p == null)
							{
								Form f = null;
								ClassPointer root = this.RootPointer;
								if (root != null)
								{
									Control c = root.ObjectInstance as Control;
									if (c != null)
									{
										f = c.FindForm();
									}
								}
								MathNode.Log(f, err);
							}
						}
						if (p != null)
						{
							TypePropertyDescriptor tp = new TypePropertyDescriptor(Owner.ObjectType, this.MemberName, null, p);
							_propDesc = tp;
						}
						else
						{
							p = t.GetProperty(this.MemberName, BindingFlags.Public | BindingFlags.Static);
							if (p != null)
							{
								StaticPropertyDescriptor2 tp = new StaticPropertyDescriptor2(Owner.ObjectType, this.MemberName, null, p);
								_propDesc = tp;
							}
						}
					}
				}
				return _propDesc;
			}
		}
		[Browsable(false)]
		public override string DisplayName
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (string.IsNullOrEmpty(MemberName))
					sb.Append("?");
				else
					sb.Append(MemberName);
				if (!typeof(ClassInstancePointer).Equals(this.ObjectType))
				{
					sb.Append(":");
					sb.Append(TypeDisplay);
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public virtual string TypeDisplay
		{
			get
			{
				PropertyDescriptor info = Info;
				if (info != null)
					return info.PropertyType.Name;
				return "{Unknown Property}";
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				IPropertyDescriptor ip = Info as IPropertyDescriptor;
				if (ip != null)
				{
					return ip.IsStatic;
				}
				return false;
			}
		}
		/// <summary>
		/// when it is used for DataValue, it specify the type of the data
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public Type Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}
		[Browsable(false)]
		public virtual PropertyInfo PropertyInformation
		{
			get
			{
				IPropertyDescriptor ip = Info as IPropertyDescriptor;
				if (ip != null)
				{
					PropertyInfo pi = ip.GetPropertyInfo();
					if (pi != null)
					{
						return pi;
					}
				}
				if (!string.IsNullOrEmpty(MemberName))
				{
					PropertyDescriptor pd = Info;
					if (pd != null)
					{
						if (pd.PropertyType.DeclaringType != null)
						{
							try
							{
								return pd.PropertyType.DeclaringType.GetProperty(this.MemberName);
							}
							catch
							{
							}
							PropertyInfo[] pifs = pd.PropertyType.DeclaringType.GetProperties();
							if (pifs != null && pifs.Length > 0)
							{
								PropertyInfo p = null;
								for (int i = 0; i < pifs.Length; i++)
								{
									if (string.CompareOrdinal(pifs[i].Name, this.MemberName) == 0)
									{
										p = pifs[i];
										if (pd.PropertyType.DeclaringType.Equals(p.DeclaringType))
										{
											return p;
										}
									}
								}
								if (p != null)
								{
									return p;
								}
							}
						}
						try
						{
							return pd.ComponentType.GetProperty(this.MemberName);
						}
						catch
						{
						}
					}
				}
				return null;
			}
		}
		#endregion
		#region methods
		[Browsable(false)]
		[NotForProgramming]
		public bool IsField()
		{
			if (this.Owner != null && typeof(EasyDataSet).IsAssignableFrom(this.Owner.ObjectType))
			{
				if (string.CompareOrdinal("Fields", this.Name) == 0)
				{
					return true;
				}
			}
			if (this.Owner.Owner != null && typeof(EasyDataSet).IsAssignableFrom(this.Owner.Owner.ObjectType))
			{
				PropertyPointer pp = this.Owner as PropertyPointer;
				if (string.CompareOrdinal("Fields", pp.Name) == 0)
				{
					return true;
				}
			}
			return false;
		}
		protected override void OnCopy(MemberPointer obj)
		{
			PropertyPointer pp = obj as PropertyPointer;
			if (pp != null)
			{
				_scope = pp._scope;
				if (pp._propDesc != null)
				{
					SetPropertyInfo(pp._propDesc);
				}
			}
		}
		public void SetPropertyInfo(PropertyDescriptor propInfo)
		{
			_propDesc = propInfo;
			MemberName = _propDesc.Name;
		}
		public virtual SetterPointer CreateSetterMethodPointer(IAction act)
		{
			PropertyPointer pp = (PropertyPointer)this.Clone();
			SetterPointer mp = new SetterPointer(act);
			MemberComponentIdCustom.AdjustHost(pp);
			mp.Owner = Owner;
			mp.SetProperty = pp;
			return mp;
		}
		public void SetRunAt(EnumWebRunAt runAt)
		{
			_runAt = runAt;
		}
		public void SetPropertyType(DataTypePointer type)
		{
			_propType = type;
		}
		#endregion
		#region IObjectPointer Members
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (_runAt != EnumWebRunAt.Unknown)
					return _runAt;
				if (this.PropertyType != null)
				{
					//it can be an unfinished property
					if (typeof(SessionVariableCollection).Equals(this.PropertyType.BaseClassType))
					{
						return EnumWebRunAt.Inherit;
					}
					PropertyDescriptor p = Info;
					if (p != null)
					{
						if (p is SessionVariableCollection.PropertyDescriptorSessionVariable)
						{
							return EnumWebRunAt.Inherit;
						}
						Attribute a = p.Attributes[typeof(WebClientMemberAttribute)];
						if (a != null)
						{
							a = p.Attributes[typeof(WebServerMemberAttribute)];
							if (a != null)
							{
								if (VPL.VPLUtil.CompilerContext_JS)
								{
									return EnumWebRunAt.Client;
								}
								else
								{
									return EnumWebRunAt.Server;
								}
							}
							else
							{
								return EnumWebRunAt.Client;
							}
						}
						a = p.Attributes[typeof(WebServerMemberAttribute)];
						if (a != null)
						{
							return EnumWebRunAt.Server;
						}
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
		public override bool IsValid
		{
			get
			{
				if (string.IsNullOrEmpty(MemberName))
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "MemberName is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					return false;
				}
				HtmlElement_Base heb = this.Owner as HtmlElement_Base;
				if (heb != null)
					return heb.IsValid;
				HtmlElementPointer hp = this.Owner as HtmlElementPointer;
				if (hp != null)
				{
					return hp.IsValid;
				}
				return true;
			}
		}
		[Browsable(false)]
		public override string TypeString
		{
			get
			{
				return Info.PropertyType.AssemblyQualifiedName;
			}
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (string.IsNullOrEmpty(this.MemberName))
				throw new DesignerException("MemberName is null at {0}.GetReferenceCode", this.GetType());
			if (this.Owner != null)
			{
				SessionVariableCollection svc = this.Owner.ObjectInstance as SessionVariableCollection;
				if (svc != null)
				{
					CodePropertyReferenceExpression cpr = new CodePropertyReferenceExpression();
					cpr.TargetObject = new CodeTypeReferenceExpression(svc.Owner.Name);
					cpr.PropertyName = "Cookies";
					CodeArrayIndexerExpression caie = new CodeArrayIndexerExpression();
					caie.TargetObject = cpr;
					caie.Indices.Add(new CodePrimitiveExpression(this.MemberName));
					return caie;
				}
			}
			ClassPointer root = this.RootPointer;
			if (root == null)
			{
				MethodClass mc = method as MethodClass;
				if (mc != null)
				{
					root = mc.RootPointer;
				}
			}
			if (root != null)
			{
				if (root.IsWebPage)
				{
					if (this.Owner != null && typeof(SessionVariableCollection).Equals(this.Owner.ObjectType))
					{
						CodePropertyReferenceExpression cpr = new CodePropertyReferenceExpression();
						cpr.TargetObject = new CodeThisReferenceExpression();
						cpr.PropertyName = "Cookies";
						CodeArrayIndexerExpression caie = new CodeArrayIndexerExpression();
						caie.TargetObject = cpr;
						caie.Indices.Add(new CodePrimitiveExpression(this.MemberName));
						return caie;
					}
					if (!IsField())
					{
						if (this.RunAt == EnumWebRunAt.Client)
						{
							if (forValue)
							{
								if (this.ObjectType.IsArray)
								{
									CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
										new CodeVariableReferenceExpression("clientRequest"), "GetStringArrayValue", new CodePrimitiveExpression(DataPassingCodeName));
									return cmie;
								}
								else
								{
									CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
										new CodeVariableReferenceExpression("clientRequest"), "GetStringValue", new CodePrimitiveExpression(DataPassingCodeName));
									return cmie;
								}
							}
							else
							{
								CodeArrayIndexerExpression caie = new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("clientRequest"), new CodePrimitiveExpression(DataPassingCodeName));
								return caie;
							}
						}
					}
				}
			}
			CodeExpression propOwner;
			if (IsStatic)
			{
				if (typeof(ProjectResources).Equals(Owner.ObjectInstance))
				{
					ProjectResources rm = ((LimnorProject)(method.ModuleProject)).GetProjectSingleData<ProjectResources>();
					return new CodePropertyReferenceExpression(
						new CodeTypeReferenceExpression(rm.HelpClassName)
						, MemberName
						);
				}
				else
				{
					propOwner = new CodeTypeReferenceExpression(Owner.TypeString);
				}
			}
			else
			{
				bool bf = forValue;
				if (bf)
				{
					if (this.Owner is DataTypePointer)
					{
						bf = false;
					}
					else if (this.Owner is TypePointer)
					{
						bf = false;
					}
					else if (this.Owner is ClassPointer)
					{
						bf = false;
					}
				}
				ICustomPropertyCompile cpc = this.Owner.ObjectInstance as ICustomPropertyCompile;
				if (cpc != null)
				{
					propOwner = cpc.GetReferenceCode();
				}
				else
				{
					if (this.Owner == root)
					{
						propOwner = new CodeThisReferenceExpression();
					}
					else
					{
						DrawingControl dc = this.Owner.ObjectInstance as DrawingControl;
						CodeExpression ce = CompilerUtil.GetDrawItemExpression(dc);
						if (ce != null)
						{
							propOwner = ce;
						}
						else
						{
							propOwner = this.Owner.GetReferenceCode(method, statements, bf);
						}
					}
				}
				//for Owner as FieldList, if MemberName is a field name then the code is Owner[MemberName].Value
				IExtendedPropertyOwner ep = Owner.ObjectInstance as IExtendedPropertyOwner;
				if (ep != null)
				{
					CodeExpression epCode = ep.GetReferenceCode(method, statements, MemberName, propOwner, forValue);
					_codeType = ep.PropertyCodeType(MemberName);
					return epCode;
				}
				if (typeof(ProjectResources).Equals(Owner.ObjectInstance))
				{
					ProjectResources rm = ((LimnorProject)(method.ModuleProject)).GetProjectSingleData<ProjectResources>();
					return new CodePropertyReferenceExpression(
						new CodeTypeReferenceExpression(rm.HelpClassName)
						, MemberName
						);
				}
				IIndexerAsProperty ip = Owner.ObjectInstance as IIndexerAsProperty;
				if (ip != null)
				{
					CodeExpression ceap = ip.CreateCodeExpression(propOwner, MemberName);
					if (ceap != null)
					{
						if (forValue)
						{
							Type t = ip.IndexerDataType(MemberName);
							if (t != null)
							{
								return CompilerUtil.GetTypeConversion(new DataTypePointer(t), ceap, new DataTypePointer(typeof(object)), statements);
							}
						}
						return ceap;
					}
				}
			}

			return new CodePropertyReferenceExpression(propOwner, this.MemberName);
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			HtmlElement_Base hb = null;
			IWebDataRepeater dp = null;
			bool isCustomValue = false;
			if (this.Owner != null && typeof(WebClientValueCollection).IsAssignableFrom(this.Owner.ObjectType))
			{
				isCustomValue = true;
			}
			Control ct = Owner.ObjectInstance as Control;
			if (ct != null)
			{
				while (ct != null)
				{
					dp = ct.Parent as IWebDataRepeater;
					if (dp != null)
					{
						break;
					}
					ct = ct.Parent;
				}
			}
			else
			{
				hb = Owner.ObjectInstance as HtmlElement_Base;
				if (hb == null)
				{
					if (isCustomValue)
					{
						if (this.Owner.Owner != null)
						{
							hb = this.Owner.Owner.ObjectInstance as HtmlElement_Base;
						}
					}
				}
				if (hb != null)
				{
					if (HtmlElement_Base.GetDataRepeater != null)
					{
						dp = HtmlElement_Base.GetDataRepeater(hb);
					}
				}
			}
			if (string.IsNullOrEmpty(this.MemberName))
				throw new DesignerException("MemberName is null at {0}.GetJavaScriptReferenceCode", this.GetType());
			if (isCustomValue && dp != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}('{1}').{2}", dp.GetElementGetter(), hb.id, this.MemberName);
			}
			Type otp = this.Owner.ObjectType;
			if (otp != null && otp.GetInterface("IJavascriptType") != null)
			{
				IJavascriptType js = Activator.CreateInstance(otp) as IJavascriptType;
				return js.GetJavascriptPropertyRef(this.Owner.GetJavaScriptReferenceCode(code), this.MemberName);
			}
			if (typeof(SessionVariableCollection).Equals(this.Owner.ObjectType))
			{
				return string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.getSessionVariable('{0}')", this.MemberName);
			}
			if (typeof(LimnorWebApp).IsAssignableFrom(this.Owner.ObjectType))
			{
				if (string.CompareOrdinal(this.MemberName, "GlobalVariableTimeout") == 0)
				{
					return "JsonDataBinding.getSessionTimeout()";
				}
			}
			if (typeof(VplPropertyBag).Equals(this.Owner.ObjectType))
			{
				if (this.Owner.Owner != null)
				{
					if (typeof(JavascriptFiles).Equals(this.Owner.Owner.ObjectType))
					{
						return this.MemberName;
					}
				}
			}
			if (this.Owner.Owner != null)
			{
				IWebClientPropertyConverter wcpc = this.Owner.Owner.ObjectInstance as IWebClientPropertyConverter;
				if (wcpc != null)
				{
					PropertyPointer pp = this.Owner as PropertyPointer;
					if (pp != null)
					{
						string scode = wcpc.GetJavaScriptPropertyCode(pp.Name, this.MemberName);
						if (!string.IsNullOrEmpty(scode))
						{
							return scode;
						}
					}
				}
			}
			string propOwner;
			string mname = null;
			bool isClientValue = this.IsWebClientValue();
			if (!isClientValue)
			{
				//download value
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", DataPassingCodeName);
			}
			else
			{
				//document.getElementById('HtmlDataRepeater1').jsData.getElement(
				IWebClientSupport wcs = Owner.ObjectInstance as IWebClientSupport;
				if (wcs != null)
				{
					string pr = null;
					if (dp != null)
					{
						IWebClientElementGetter wce = wcs as IWebClientElementGetter;
						if (wce != null)
						{
							wce.SetElementGetter(dp.GetElementGetter());
							pr = wcs.GetJavaScriptWebPropertyReferenceCode(code, this.MemberName, new string[] { });
						}
					}
					else
					{
						pr = wcs.GetJavaScriptWebPropertyReferenceCode(code, this.MemberName, new string[] { });
					}
					if (!string.IsNullOrEmpty(pr))
					{
						return pr;
					}
				}
				bool isWebType = false;
				IWebClientComponent webc = Owner as IWebClientComponent;
				if (webc == null)
				{
					webc = Owner.ObjectInstance as IWebClientComponent;
				}
				if (webc != null)
				{
					if (dp == null)
					{
						mname = webc.GetJavaScriptReferenceCode(code, this.MemberName, new string[] { });
						if (!string.IsNullOrEmpty(mname))
						{
							return mname;
						}
					}
					mname = webc.MapJavaScriptCodeName(this.MemberName);
				}
				else
				{
					ArrayVariable av = Owner as ArrayVariable;
					if (av != null && string.CompareOrdinal(this.MemberName, "Length") == 0)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.length", Owner.GetJavaScriptReferenceCode(code));
					}
					else
					{
						if (Owner.ObjectInstance == null && Owner.ObjectType != null)
						{
							if (!Owner.ObjectType.IsInterface && Owner.ObjectType.GetInterface("IWebClientControl") != null)
							{
								isWebType = true;
							}
							else if (typeof(IWebClientComponent).Equals(Owner.ObjectType) || typeof(IWebClientControl).Equals(Owner.ObjectType))
							{
								isWebType = true;
							}
							else if (Owner.ObjectType.IsSubclassOf(typeof(HtmlElement_BodyBase)))
							{
								isWebType = true;
							}
						}
					}
					if (webc != null)
					{
						if (dp == null)
						{
							mname = webc.GetJavaScriptReferenceCode(code, this.MemberName, new string[] { });
							if (!string.IsNullOrEmpty(mname))
							{
								return mname;
							}
						}
						mname = webc.MapJavaScriptCodeName(this.MemberName);
					}
					else
					{
						if (isWebType)
						{
							mname = WebPageCompilerUtility.MapJavaScriptCodeName(MemberName);
						}
						if (string.IsNullOrEmpty(mname))
						{
							mname = this.MemberName;
						}
					}
				}
			}
			propOwner = this.Owner.GetJavaScriptReferenceCode(code);
			if (this.ObjectType != null && this.ObjectType.IsAssignableFrom(typeof(WebClientValueCollection)))
			{
				if (string.CompareOrdinal(MemberName, "CustomValues") == 0)
				{
					return propOwner;
				}
			}
			//for Owner as FieldList, if MemberName is a field name then the code is Owner[MemberName].Value
			IExtendedPropertyOwner ep = Owner.ObjectInstance as IExtendedPropertyOwner;
			if (ep != null)
			{
				string epCode = ep.GetJavaScriptReferenceCode(code, mname, propOwner);
				return epCode;
			}
			if (this.RootPointer != null && this.RootPointer.Project != null && (this.RootPointer.Project.ProjectType == VSPrj.EnumProjectType.WebAppAspx || this.RootPointer.Project.ProjectType == VSPrj.EnumProjectType.WebAppPhp))
			{
				PropertyPointer pp = this.Owner as PropertyPointer;
				if (pp != null && pp.RunAt == EnumWebRunAt.Client)
				{
					IExtendedPropertyOwner epo = pp.Owner.ObjectInstance as IExtendedPropertyOwner;
					if (epo != null)
					{
						Type t = epo.PropertyCodeType(pp.MemberName);
						if (t != null)
						{
							if (t.Equals(typeof(DateTime)))
							{
								JsDateTime jd = new JsDateTime();
								string ce = jd.GetJavascriptPropertyRef(propOwner, this.MemberName);
								if (!string.IsNullOrEmpty(ce))
								{
									return ce;
								}
							}
						}
					}
				}
			}
			if (typeof(ProjectResources).Equals(Owner.ObjectInstance))
			{
				if (string.CompareOrdinal(mname, "ProjectCultureName") == 0)
				{
					return "JsonDataBinding.GetCulture()";
				}
				ProjectResources rm = (this.RootPointer.Project).GetProjectSingleData<ProjectResources>();
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
					rm.HelpClassName
					, mname
					);
			}
			IIndexerAsProperty ip = Owner.ObjectInstance as IIndexerAsProperty;
			if (ip != null)
			{
				string ceap = ip.GetJavaScriptReferenceCode(propOwner, mname);
				if (ceap != null)
				{
					return ceap;
				}
			}
			if (dp != null)
			{
				if (string.CompareOrdinal(mname, "innerText") == 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.GetInnerText(document.getElementById('{0}').jsData.getElement('{1}'))", dp.CodeName, Owner.CodeName);
				}
				else if (string.CompareOrdinal(mname, "Opacity") == 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getOpacity(document.getElementById('{0}').jsData.getElement('{1}'))", dp.CodeName, Owner.CodeName);
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.getElement('{1}').{2}", dp.CodeName, Owner.CodeName, mname);
				}
			}
			else
			{
				if (string.CompareOrdinal(mname, "innerText") == 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.GetInnerText({0})", propOwner);
				}
				else if (string.CompareOrdinal(mname, "Opacity") == 0)
				{
					return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getOpacity({0})", propOwner);
				}
				else
				{
					if (ObjectCompilerAttribute.UsObjectCompiler(Owner.ObjectType))
					{
						return this.CodeName;
					}
					if (string.CompareOrdinal("Length", mname) == 0 && Owner.ObjectType != null && Owner.ObjectType.IsArray)
					{
						mname = "length";
					}
					return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", propOwner, mname);
				}
			}
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (string.IsNullOrEmpty(this.MemberName))
				throw new DesignerException("MemberName is null at {0}.GetPhpScriptReferenceCode", this.GetType());
			if (this.Owner.ObjectType != null && this.Owner.ObjectType.GetInterface("IPhpType") != null)
			{
				IPhpType php = Activator.CreateInstance(this.Owner.ObjectType) as IPhpType;
				return php.GetPropertyRef(this.Owner.CodeName, this.MemberName);
			}
			if (typeof(SessionVariableCollection).Equals(this.Owner.ObjectType))
			{
				if (VPLUtil.SessionDataStorage == EnumSessionDataStorage.HTML5Storage)
				{
					return string.Format(CultureInfo.InvariantCulture,
						"$this->jsonFromClient->values->{0}", this.DataPassingCodeName);
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture,
						"$this->WebAppPhp->GetSessionVariable('{0}')", this.MemberName);
				}
			}
			string retUpload = null;
			if (IsWebClientValue() && !IsWebServerValue())
			{
				//upload value
				retUpload = string.Format(CultureInfo.InvariantCulture, "$this->jsonFromClient->values->{0}", this.DataPassingCodeName);
				if (this.IsDatetime)
				{
					retUpload = string.Format(CultureInfo.InvariantCulture, "date('Y-m-d H:i:s', strtotime({0}) )", retUpload);
				}
			}
			if (this.Owner.Owner != null && typeof(EasyDataSet).IsAssignableFrom(this.Owner.Owner.ObjectType))
			{
				PropertyPointer pp = this.Owner as PropertyPointer;
				if (string.CompareOrdinal("Fields", pp.Name) == 0)
				{
					EasyDataSet eds = this.Owner.Owner.ObjectInstance as EasyDataSet;
					if (eds != null)
					{
						if (string.IsNullOrEmpty(retUpload))
						{
							return string.Format(CultureInfo.InvariantCulture,
								"${1}->getCurrentColumnValue('{0}')", this.MemberName, eds.TableVariableName);
						}
						else
						{
							return string.Format(CultureInfo.InvariantCulture,
								"(isset(${1})? ${1}->getCurrentColumnValue('{0}'):{2})", this.MemberName, eds.TableVariableName, retUpload);
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(retUpload))
			{
				return retUpload;
			}
			string propOwner;
			propOwner = this.Owner.GetPhpScriptReferenceCode(code);
			//for Owner as FieldList, if MemberName is a field name then the code is Owner[MemberName].Value
			IExtendedPropertyOwner ep = Owner.ObjectInstance as IExtendedPropertyOwner;
			if (ep != null)
			{
				string epCode = ep.GetPhpScriptReferenceCode(code, this.MemberName, propOwner);
				return epCode;
			}
			if (typeof(ProjectResources).Equals(Owner.ObjectInstance))
			{
				if (this.RootPointer != null && this.RootPointer.Project != null)
				{
					ProjectResources rm = (this.RootPointer.Project).GetProjectSingleData<ProjectResources>();
					return string.Format(CultureInfo.InvariantCulture, "{0}->{1}",
						rm.HelpClassName
						, this.MemberName
						);
				}
			}
			IIndexerAsProperty ip = Owner.ObjectInstance as IIndexerAsProperty;
			if (ip != null)
			{
				string ceap = ip.GetPhpScriptReferenceCode(propOwner, this.MemberName);
				if (ceap != null)
				{

					return ceap;
				}
			}

			return string.Format(CultureInfo.InvariantCulture, "{0}->{1}", propOwner, this.MemberName);
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			string o = GetJavaScriptReferenceCode(code);
			if (!string.IsNullOrEmpty(o))
			{
				IExtendedPropertyOwner epo = Owner.ObjectInstance as IExtendedPropertyOwner;
				if (epo != null)
				{
					Type t = epo.PropertyCodeType(this.MemberName);
					if (t != null)
					{
						if (this.RunAt == EnumWebRunAt.Client)
						{
							if (t.Equals(typeof(DateTime)) || t.Equals(typeof(JsDateTime)))
							{
								JsDateTime jd = new JsDateTime();
								string ce = jd.GetJavascriptMethodRef(o, methodName, code, parameters);
								if (!string.IsNullOrEmpty(ce))
								{
									bool isSetField = false;
									if (typeof(FieldList).IsAssignableFrom(Owner.ObjectType))
									{
										if (Owner.Owner != null && typeof(EasyDataSet).IsAssignableFrom(Owner.Owner.ObjectType))
										{
											EasyDataSet ds = Owner.Owner.ObjectInstance as EasyDataSet;
											if (ds != null)
											{
												isSetField = true;
												string vard = string.Format(CultureInfo.InvariantCulture, "d{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
												code.Add(string.Format(CultureInfo.InvariantCulture, "var {0}={1};\r\n", vard, ce));
												code.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.setColumnValue('{0}','{1}',JsonDataBinding.datetime.toIso({2}));\r\n", ds.TableName, this.CodeName, vard));
												if (!string.IsNullOrEmpty(returnReceiver))
												{
													code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", returnReceiver, vard));
												}
											}
										}
									}
									if (!isSetField)
									{
										if (string.IsNullOrEmpty(returnReceiver))
										{
											code.Add(string.Format(CultureInfo.InvariantCulture, "{0};\r\n", ce));
										}
										else
										{
											code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", returnReceiver, ce));
										}
									}
								}
								return;
							}
							else if (t.Equals(typeof(TimeSpan)) || t.Equals(typeof(JsTimeSpan)))
							{
								JsTimeSpan jd = new JsTimeSpan();
								string ce = jd.GetJavascriptMethodRef(o, methodName, code, parameters);
								if (!string.IsNullOrEmpty(ce))
								{
									bool isSetField = false;
									if (typeof(FieldList).IsAssignableFrom(Owner.ObjectType))
									{
										if (Owner.Owner != null && typeof(EasyDataSet).IsAssignableFrom(Owner.Owner.ObjectType))
										{
											EasyDataSet ds = Owner.Owner.ObjectInstance as EasyDataSet;
											if (ds != null)
											{
												isSetField = true;
												string vard = string.Format(CultureInfo.InvariantCulture, "d{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
												code.Add(string.Format(CultureInfo.InvariantCulture, "var {0}={1};\r\n", vard, ce));
												code.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.setColumnValue('{0}','{1}',JsonDataBinding.toText({2}));\r\n", ds.TableName, this.CodeName, vard));
												if (!string.IsNullOrEmpty(returnReceiver))
												{
													code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", returnReceiver, vard));
												}
											}
										}
									}
									if (!isSetField)
									{
										if (string.IsNullOrEmpty(returnReceiver))
										{
											code.Add(string.Format(CultureInfo.InvariantCulture, "{0};\r\n", ce));
										}
										else
										{
											code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1};\r\n", returnReceiver, ce));
										}
									}
								}
								return;
							}
						}
					}
				}
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					sb.Append(returnReceiver);
					sb.Append("=");
				}
				sb.Append(o);
				sb.Append(".");
				sb.Append(methodName);
				sb.Append("(");
				if (parameters != null && parameters.Count > 0)
				{
					sb.Append(parameters[0]);
					for (int i = 1; i < parameters.Count; i++)
					{
						sb.Append(",");
						sb.Append(parameters[i]);
					}
				}
				sb.Append(");\r\n");
				code.Add(sb.ToString());
			}
		}
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public override bool IsTargeted(EnumObjectSelectType target)
		{
			if (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All)
				return true;
			if (target == EnumObjectSelectType.Method)
			{
				if (!Info.IsReadOnly)
				{
					return true; //can create Setter action
				}
			}
			return false;
		}
		private bool loadingInstance = false;
		[Browsable(false)]
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				if (Owner == null)
				{
				}
				else
				{
					object owner = null;
					if (loadingInstance)
					{
						loadingInstance = false;
						MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error getting property instance for [{0}]. Circular call detected", this.MemberName));
					}
					else
					{
						loadingInstance = true;
						try
						{
							owner = Owner.ObjectInstance;
						}
						finally
						{
							loadingInstance = false;
						}
					}
					if (owner != null)
					{
						PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(owner, new Attribute[] { DesignOnlyAttribute.No, BrowsableAttribute.Yes });
						foreach (PropertyDescriptor prop in properties)
						{
							if (prop.Name == this.MemberName)
							{
								return prop.GetValue(owner);
							}
						}
					}
				}
				return null;
			}
			set
			{
				object owner = Owner.ObjectInstance;
				if (owner != null)
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(owner, new Attribute[] { DesignOnlyAttribute.No, BrowsableAttribute.Yes });
					foreach (PropertyDescriptor prop in properties)
					{
						if (prop.Name == this.MemberName)
						{
							prop.SetValue(owner, value);
						}
					}
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				if (_codeType != null)
				{
					return _codeType;
				}
				if (string.IsNullOrEmpty(MemberName))
				{
					return typeof(object);
				}
				else
				{
					if (Owner != null)
					{
						IIndexerAsProperty ip = Owner.ObjectInstance as IIndexerAsProperty;
						if (ip != null)
						{
							if (ip.IsIndexer(MemberName))
							{
								Type t = ip.IndexerDataType(MemberName);
								if (t != null)
								{
									return t;
								}
							}
						}
					}
					if (Info == null)
					{
						return null;
					}
					return Info.PropertyType;
				}
			}
			set
			{
			}
		}
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			PropertyPointer p = objectPointer as PropertyPointer;
			if (p != null)
			{
				if (base.IsSameObjectRef(objectPointer))
				{
					if (p.Info != null && this.Info != null)
					{
						if (p.Info.PropertyType.Equals(this.Info.PropertyType))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				if (_propDesc is PropertyClassDescriptor)
					return EnumObjectDevelopType.Custom;
				return EnumObjectDevelopType.Library;
			}
		}
		[Browsable(false)]
		public override EnumPointerType PointerType { get { return EnumPointerType.Property; } }
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IProperty Members
		[ReadOnly(true)]
		[Description("Public: all objects can access it; Protected: only this class and its derived classes can access it; Private: only this class can access it.")]
		public EnumAccessControl AccessControl
		{
			get
			{
				PropertyInfo p = PropertyInformation;
				if (p != null)
				{
					if (p.CanRead)
					{
						MethodInfo mif = p.GetGetMethod(true);
						if (mif.IsPrivate)
						{
							return EnumAccessControl.Private;
						}
						if (mif.IsPublic)
						{
							return EnumAccessControl.Public;
						}
						return EnumAccessControl.Protected;
					}
					if (p.CanWrite)
					{
						MethodInfo mif = p.GetSetMethod(true);
						if (mif.IsPrivate)
						{
							return EnumAccessControl.Private;
						}
						if (mif.IsPublic)
						{
							return EnumAccessControl.Public;
						}
						return EnumAccessControl.Protected;
					}
				}
				return EnumAccessControl.Public;
			}
			set
			{
			}
		}
		public bool IsDatetime
		{
			get
			{
				if (typeof(DateTime).Equals(PropertyType.BaseClassType))
				{
					return true;
				}
				if (typeof(JsDateTime).Equals(PropertyType.BaseClassType))
				{
					return true;
				}
				return false;
			}
		}
		public DataTypePointer PropertyType
		{
			get
			{
				if (_propType != null)
					return _propType;
				if (_propDesc != null)
				{
					return new DataTypePointer(new TypePointer(_propDesc.PropertyType));
				}
				if (Info != null)
				{
					return new DataTypePointer(new TypePointer(Info.PropertyType));
				}
				ICustomPropertyCollection cpc = this.Owner as ICustomPropertyCollection;
				if (cpc == null && this.Owner != null)
				{
					cpc = this.Owner.ObjectInstance as ICustomPropertyCollection;
				}
				if (cpc != null)
				{
					return new DataTypePointer(new TypePointer(cpc.GetCustomPropertyType(this.MemberName)));
				}
				if (this.Owner != null)
				{
					SessionVariableCollection svc = this.Owner.ObjectInstance as SessionVariableCollection;
					if (svc == null)
					{
						if (typeof(SessionVariableCollection).Equals(this.Owner.ObjectType))
						{
							if (this.Owner.RootPointer != null)
							{
								if (this.Owner.RootPointer.ObjectList.Count == 0)
								{
									this.Owner.RootPointer.ObjectList.LoadObjects();
									svc = this.Owner.ObjectInstance as SessionVariableCollection;
								}
							}
						}
					}
					if (svc != null)
					{
						SessionVariable sv = svc[this.MemberName];
						if (sv == null)
						{
							DesignUtil.WriteToOutputWindow("Session variable [{0}] not found", this.MemberName);
						}
						else
						{
							return new DataTypePointer(new TypePointer(sv.Value.GetValueType()));
						}
					}
				}
				DesignUtil.WriteToOutputWindow("PropertyType cannot be defined for {0}", this.MemberName);
				return null;
			}
		}
		public DataTypePointer[] GetConcreteTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.TypeParameters;
			}
			return null;
		}
		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetConcreteType(typeParameter);
			}
			return null;
		}
		public IList<DataTypePointer> GetGenericTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetGenericTypes();
			}
			return null;
		}
		public CodeTypeReference GetCodeTypeReference()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetCodeTypeReference();
			}
			return null;
		}
		/// <summary>
		/// the name cannot be changed
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			throw new DesignerException("Cannot change the name of a property from a library");
		}

		[Browsable(false)]
		public string Name
		{
			get { return this.MemberName; }
		}
		[Browsable(false)]
		public virtual bool IsCustomProperty
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool Implemented
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public bool HasBaseImplementation
		{
			get
			{
				PropertyInfo p = PropertyInformation;
				if (p != null)
				{
					return !VPLUtil.IsAbstract(p);
				}
				return false;
			}
		}
		[Browsable(false)]
		public void SetValue(object value)
		{
			IPropertyValueSetter pvs = this.Owner.ObjectInstance as IPropertyValueSetter;
			if (pvs != null)
			{
				if (pvs.SetPropertyValue(this.Name, value))
				{
					return;
				}
			}
			PropertyInfo p = this.PropertyInformation;
			if (p != null)
			{
				IComponent ic = null;
				ClassPointer o = Owner as ClassPointer;
				if (o != null)
				{
					ic = o.ObjectInstance as IComponent;
				}
				else
				{
					MemberComponentId mm = Owner as MemberComponentId;
					if (mm != null)
					{
						ic = mm.ObjectInstance as IComponent;
					}
				}
				if (ic != null && ic.Site != null && ic.Site.DesignMode)
				{
					p.SetValue(ic, value, null);
				}
			}
		}
		[Browsable(false)]
		public IList<Attribute> GetUITypeEditor()
		{
			List<Attribute> lst = new List<Attribute>();
			PropertyDescriptor inf = Info;
			if (inf != null && inf.Attributes != null && inf.Attributes.Count > 0)
			{
				for (int i = 0; i < inf.Attributes.Count; i++)
				{
					EditorAttribute ea = inf.Attributes[i] as EditorAttribute;
					if (ea != null)
					{
						lst.Add(ea);
					}
					else
					{
						FilePathAttribute fa = inf.Attributes[i] as FilePathAttribute;
						if (fa != null)
						{
							lst.Add(fa);
						}
					}
				}
			}
			return lst;
		}
		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return false; } }
		public bool CanWrite
		{
			get
			{
				PropertyDescriptor p = Info;
				if (p != null)
				{
					return !p.IsReadOnly;
				}
				return false;
			}
		}
		public UInt32 TaskId { get { return _taskId; } }
		public void SetTaskId(UInt32 taskId)
		{
			_taskId = taskId;
		}
		public bool IsWebClientValue()
		{
			PropertyDescriptor p = Info;
			if (p != null)
			{
				if (p.Attributes[typeof(WebClientMemberAttribute)] != null)
				{
					return true;
				}
				if (p.Attributes[typeof(WebServerMemberAttribute)] != null)
				{
					return false;
				}
			}
			return DesignUtil.IsWebClientObject(Owner);
		}
		public bool IsWebServerValue()
		{
			PropertyDescriptor p = Info;
			if (p != null)
			{
				if (p.Attributes[typeof(WebServerMemberAttribute)] != null)
				{
					return true;
				}
				if (p.Attributes[typeof(WebClientMemberAttribute)] != null)
				{
					return false;
				}
			}
			return DesignUtil.IsWebServerObject(Owner);
		}
		public string CreateJavaScript(StringCollection method)
		{
			return GetJavaScriptReferenceCode(method);
		}
		public string CreatePhpScript(StringCollection method)
		{
			return GetPhpScriptReferenceCode(method);
		}
		public void SetValueOwner(object o)
		{
			if (Owner == null)
			{
				IObjectPointer p = o as IObjectPointer;
				if (p != null)
				{
					Owner = p;
				}
			}
		}
		public object ValueOwner
		{
			get
			{
				return Owner;
			}
		}
		[Browsable(false)]
		public string DataPassingCodeName
		{
			get
			{
				return CompilerUtil.CreateJsCodeName(this, _taskId);
			}
		}
		#endregion
	}
	/// <summary>
	/// Variables for an object
	/// </summary>
	[Serializable]
	public class FieldPointer : MemberPointer, IPropertyEx, ISourceValuePointer
	{
		#region fields and constructors
		private FieldInfo _fif;
		private UInt32 _taskId;
		private bool _isStatic;
		//private Type _codeType;
		private DataTypePointer _propType = null;
		public FieldPointer()
		{
		}
		public FieldPointer(FieldInfo fi, IObjectPointer owner)
		{
			_fif = fi;
			if (fi != null)
			{
				_isStatic = fi.IsStatic;
				MemberName = fi.Name;
			}
			this.Owner = owner;
		}
		#endregion
		#region Methods
		public SetterPointer CreateSetterMethodPointer(IAction act)
		{
			FieldPointer pp = (FieldPointer)this.Clone();
			SetterPointer mp = new SetterPointer(act);
			MemberComponentIdCustom.AdjustHost(pp);
			mp.Owner = Owner;
			mp.SetProperty = pp;
			return mp;
		}
		public void SetFieldInfo(FieldInfo e, bool isStatic)
		{
			_isStatic = isStatic;
			_fif = e;
			MemberName = e.Name;
		}
		#endregion
		#region properties
		public FieldInfo Info
		{
			get
			{
				if (_fif == null)
				{
					Type ownerType;
					if (Owner.ObjectInstance != null)
					{
						ownerType = VPLUtil.GetObjectType(Owner.ObjectInstance);
					}
					else
					{
						ownerType = Owner.ObjectType;
					}
					if (ownerType != null)
					{
						_fif = ownerType.GetField(MemberName);
						if (_fif != null)
						{
							_isStatic = false;
						}
						else
						{
							_fif = ownerType.GetField(MemberName, BindingFlags.Static);
							if (_fif != null)
							{
								_isStatic = true;
							}
						}
					}
				}
				return _fif;
			}

		}
		#endregion
		#region IObjectPointer Members
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (Owner != null)
				{
					return Owner.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		protected override void OnCopy(MemberPointer obj)
		{
			FieldPointer fp = obj as FieldPointer;
			if (fp != null)
			{
				SetFieldInfo(fp.Info, obj.IsStatic);
			}
		}
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (Info != null)
				{
					return true;
				}
				if (_fif != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_fif is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				return _isStatic;
			}
		}
		[Browsable(false)]
		public override string TypeString
		{
			get
			{
				return Info.FieldType.AssemblyQualifiedName;
			}
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			bool bf = forValue;
			if (bf)
			{
				if (this.Owner is DataTypePointer)
				{
					bf = false;
				}
				else if (this.Owner is TypePointer)
				{
					bf = false;
				}
				else if (this.Owner is ClassPointer)
				{
					bf = false;
				}
			}
			return new CodeFieldReferenceExpression(this.Owner.GetReferenceCode(method, statements, bf), this.MemberName);
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
			  this.Owner.GetJavaScriptReferenceCode(code), this.MemberName);
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}->{1}",
			  this.Owner.GetPhpScriptReferenceCode(code), this.MemberName);
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public override bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All);
		}
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				if (Owner != null)
				{
					object owner = Owner.ObjectInstance;
					if (owner != null)
					{
						return Info.GetValue(owner);
					}
				}
				return null;
			}
			set
			{
				object owner = Owner.ObjectInstance;
				if (owner != null)
				{
					Info.SetValue(owner, value);
				}
			}
		}
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return Info.FieldType;
			}
			set
			{
			}
		}
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			if (objectPointer is FieldPointer)
			{
				return base.IsSameObjectRef(objectPointer);
			}
			return false;
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public override EnumPointerType PointerType { get { return EnumPointerType.Field; } }
		#endregion

		#region IProperty Members

		public string Name
		{
			get { return this.MemberName; }
		}
		public bool IsReadOnly { get { return false; } }
		[Browsable(false)]
		public Type CodeType
		{
			get
			{
				FieldInfo f = Info;
				if (f != null)
				{
					return f.FieldType;
				}
				return typeof(object);
			}
		}
		public DataTypePointer PropertyType
		{
			get
			{
				if (_propType != null)
					return _propType;
				FieldInfo f = Info;
				if (f != null)
				{
					return new DataTypePointer(f.FieldType);
				}
				return null;
			}
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
			get { return false; }
		}

		public void SetName(string name)
		{
			throw new DesignerException("Cannot change the name of a field from a library");
		}

		public void SetValue(object value)
		{
			FieldInfo f = Info;
			if (f != null)
			{
				if (f.IsStatic)
				{
					f.SetValue(null, value);
				}
				else
				{
					if (this.Owner != null)
					{
						if (this.Owner.ObjectInstance != null)
						{
							f.SetValue(this.Owner.ObjectInstance, value);
						}
					}
				}
			}
		}

		public IList<Attribute> GetUITypeEditor()
		{
			List<Attribute> lst = new List<Attribute>();
			FieldInfo f = Info;
			if (f != null)
			{
				object[] vs = f.GetCustomAttributes(true);
				if (vs != null && vs.Length > 0)
				{
					for (int i = 0; i < vs.Length; i++)
					{
						EditorAttribute ea = vs[i] as EditorAttribute;
						if (ea != null)
						{
							lst.Add(ea);
						}
						else
						{
							FilePathAttribute fa = vs[i] as FilePathAttribute;
							if (fa != null)
							{
								lst.Add(fa);
							}
						}
					}
				}
			}
			return lst;
		}

		#endregion

		#region IGenericTypePointer Members

		public DataTypePointer[] GetConcreteTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.TypeParameters;
			}
			return null;
		}

		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetConcreteType(typeParameter);
			}
			return null;
		}

		public CodeTypeReference GetCodeTypeReference()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.GetCodeTypeReference();
			}
			return null;
		}

		public IList<DataTypePointer> GetGenericTypes()
		{
			DataTypePointer p = PropertyType;
			if (p != null)
			{
				return p.TypeParameters;
			}
			return null;
		}

		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return false; } }
		public object ValueOwner
		{
			get { return Owner; }
		}

		public string DataPassingCodeName
		{
			get { return CompilerUtil.CreateJsCodeName(this, _taskId); }
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
			if (Owner == null)
			{
				IObjectPointer p = o as IObjectPointer;
				if (p != null)
				{
					Owner = p;
				}
			}
		}

		public bool IsWebClientValue()
		{
			FieldInfo f = Info;
			if (f != null)
			{
				object[] vs = f.GetCustomAttributes(true);
				if (vs != null && vs.Length > 0)
				{
					for (int i = 0; i < vs.Length; i++)
					{
						if (vs[i] is WebClientMemberAttribute)
						{
							return true;
						}
						if (vs[i] is WebServerMemberAttribute)
						{
							return false;
						}
					}
				}
			}
			return DesignUtil.IsWebClientObject(Owner);
		}

		public bool IsWebServerValue()
		{
			FieldInfo f = Info;
			if (f != null)
			{
				object[] vs = f.GetCustomAttributes(true);
				if (vs != null && vs.Length > 0)
				{
					for (int i = 0; i < vs.Length; i++)
					{
						if (vs[i] is WebClientMemberAttribute)
						{
							return false;
						}
						if (vs[i] is WebServerMemberAttribute)
						{
							return true;
						}
					}
				}
			}
			return DesignUtil.IsWebServerObject(Owner);
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
				return true;
			}
		}

		#endregion
	}
}
