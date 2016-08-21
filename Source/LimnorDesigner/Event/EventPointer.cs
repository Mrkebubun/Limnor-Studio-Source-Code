/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using VPL;
using LimnorDesigner.MethodBuilder;
using System.CodeDom;
using MathExp;
using ProgElements;
using DynamicEventLinker;
using LimnorDesigner.ResourcesManager;
using VSPrj;
using System.Collections.Specialized;
using System.Globalization;
using LimnorDesigner.Action;
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;
using LimnorDesigner.DesignTimeType;

namespace LimnorDesigner.Event
{
	/// <summary>
	/// pointer to an event. the MemberName is the event name.
	/// ObjectType and ObjectInstance points to the event owner
	/// </summary>
	[Serializable]
	public class EventPointer : MemberPointer, IEventPointer, IEvent
	{
		#region fields and constructors
		private object[] _parameterValues;
		private EventInfo _eif;
		private string _displaye;
		private int _eventId;//for ICustomEventMethodDescriptor to save the id so that when the name changed it can be matched by id
		public EventPointer()
		{
		}
		#endregion
		#region Static members
		public static ParameterInfo[] GetEventParameters(EventInfo e)
		{
			EventInfoX x = e as EventInfoX;
			if (x != null)
			{
				return x.Event.GetParameters();
			}
			EventInfoInterface ei = e as EventInfoInterface;
			if (ei != null)
			{
				return ei.GetParameters();
			}
			MethodInfo mif;
			mif = e.GetRaiseMethod();
			if (mif != null)
			{
				return mif.GetParameters();
			}
			if (e.EventHandlerType != null)
			{
				mif = e.EventHandlerType.GetMethod("Invoke");
				if (mif != null)
				{
					return mif.GetParameters();
				}
			}
			return new ParameterInfo[] { };
		}
		#endregion
		#region properties
		[ReadOnly(true)]
		[Description("Event arguments. Their values are generated when the event occurs at runtime.")]
		public object[] ParameterValues
		{
			get
			{
				return _parameterValues;
			}
			set
			{
				_parameterValues = value;
			}
		}
		[DefaultValue(0)]
		[Browsable(false)]
		public int EventId
		{
			get
			{
				if (_eventId == 0 && Owner != null)
				{
					ICustomEventDescriptor em = Owner.ObjectInstance as ICustomEventDescriptor;
					if (em != null)
					{
						IEventInfoTree eit = em.GetEvent(MemberName) as IEventInfoTree;
						if (eit != null)
						{
							_eventId = eit.GetEventId();
						}
					}
				}
				return _eventId;
			}
			set
			{
				_eventId = value;
			}
		}
		public virtual EventInfo Info
		{
			get
			{
				if (_eif == null)
				{
					Type t = null;
					if (Owner == null)
					{
					}
					else if (Owner.ObjectInstance != null)
					{
						ICustomEventDescriptor em = Owner.ObjectInstance as ICustomEventDescriptor;
						if (em != null)
						{
							EventInfo inf;
							if (_eventId != 0)
							{
								inf = em.GetEventById(_eventId);
							}
							else
							{
								inf = em.GetEvent(MemberName);
								if (inf != null)
								{
									IEventInfoTree eit = inf as IEventInfoTree;
									if (eit != null)
									{
										_eventId = eit.GetEventId();
									}
									else
									{
										_eventId = em.GetEventId(MemberName);
									}
								}
							}
							return inf;
						}
						else
						{
							IObjectPointer op = Owner.ObjectInstance as IObjectPointer;
							if (op != null)
							{
								t = VPLUtil.GetObjectType(op.ObjectInstance);
							}
							else
							{
								t = VPLUtil.GetObjectType(Owner.ObjectInstance);
							}
						}
					}
					else
					{
						t = Owner.ObjectType;
					}
					if (t != null)
					{
						_eif = VPLUtil.GetEventInfo(t, this.MemberName);
					}
				}
				return _eif;
			}
		}
		public ParameterInfo[] Parameters
		{
			get
			{
				EventInfo eif = Info;
				if (eif != null)
				{
					return GetEventParameters(eif);
				}
				return null;
			}
		}

		public override string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(_displaye))
				{
					EventInfo eif = Info;
					if (eif != null)
					{
						ParameterInfo[] ps = GetEventParameters(eif);
						StringBuilder sb = new StringBuilder(eif.Name);
						if (ps != null && ps.Length > 0)
						{
							sb.Append("(");
							sb.Append(ps[0].ParameterType.Name);
							sb.Append(" ");
							sb.Append(ps[0].Name);
							for (int i = 1; i < ps.Length; i++)
							{
								sb.Append(",");
								sb.Append(ps[i].ParameterType.Name);
								sb.Append(" ");
								sb.Append(ps[i].Name);
							}
							sb.Append(")");
						}
						_displaye = sb.ToString();
					}
					else
					{
						_displaye = "(?)";
					}
				}
				return _displaye;
			}
		}
		[Browsable(false)]
		public override string ExpressionDisplay
		{
			get
			{
				return this.Name;
			}
		}
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return Owner.ObjectType;
			}
			set
			{
				Owner.ObjectType = value;
			}
		}
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				return Owner.ObjectInstance;
			}
			set
			{
				Owner.ObjectInstance = value;
			}
		}

		#endregion
		#region Methods
		public void SetEventInfo(EventInfo e)
		{
			_displaye = null;
			_eif = e;
			MemberName = e.Name;
			ParameterInfo[] ps = GetEventParameters(_eif);
			if (ps == null)
				_parameterValues = new object[] { };
			else
				_parameterValues = new object[ps.Length];
		}
		public override bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Event || target == EnumObjectSelectType.All);
		}
		protected override void OnCopy(MemberPointer obj)
		{
			EventPointer ep = obj as EventPointer;
			if (ep != null)
			{
				if (ep._parameterValues != null)
				{
					_parameterValues = new object[ep._parameterValues.Length];
					for (int i = 0; i < _parameterValues.Length; i++)
					{
						ICloneable ic = ep._parameterValues[i] as ICloneable;
						if (ic != null)
						{
							_parameterValues[i] = ic.Clone();
						}
						else
						{
							_parameterValues[i] = ep._parameterValues[i];
						}
					}
				}
				ICloneable eic = ep.Info as ICloneable;
				if (eic != null)
				{
					_eif = (EventInfo)eic.Clone();
				}
				else
				{
					_eif = ep.Info;
				}
			}
		}
		public override bool IsSameObjectRef(IObjectIdentity obj)
		{
			EventPointer ep = obj as EventPointer;
			if (ep != null)
			{
				if (base.IsSameObjectRef(obj))
				{
					return true;
				}
			}
			return false;
		}
		public override string ToString()
		{
			if (Owner == null)
			{
				if (string.IsNullOrEmpty(this.MemberName))
					return "?.?";
				return "?." + MemberName;
			}
			if (string.IsNullOrEmpty(this.MemberName))
				return Owner.ToString() + "!?";
			return Owner.ToString() + "!" + this.MemberName;
		}
		#endregion
		#region IObjectPointer Members
		[Browsable(false)]
		public override ClassPointer RootPointer
		{
			get
			{
				ClassPointer root = base.RootPointer;
				if (root != null)
				{
					return root;
				}
				if (this.Owner != null)
				{
					root = Owner.RootPointer;
					if (root != null)
					{
						return root;
					}
				}
				return null;
			}
		}
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (_eif != null)
				{
					object[] a = _eif.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
					if (a != null && a.Length > 0)
					{
						return EnumWebRunAt.Client;
					}
					a = _eif.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
					if (a != null && a.Length > 0)
					{
						return EnumWebRunAt.Server;
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
				if (_eif != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_eif is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				if (_eif != null)
				{
					EventInfoX x = _eif as EventInfoX;
					if (x != null)
					{
						return x.IsStatic;
					}
					EventInfoInterface eii = _eif as EventInfoInterface;
					if (eii != null)
					{
						return eii.IsStatic;
					}
					MethodInfo mi = _eif.GetAddMethod(true);
					if (mi != null)
					{
						return mi.IsStatic;
					}
					mi = _eif.GetRaiseMethod();
					if (mi != null)
					{
						return mi.IsStatic;
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public override string TypeString
		{
			get
			{
				return Info.EventHandlerType.AssemblyQualifiedName;
			}
		}
		protected virtual CodeExpression OnGetTargetObject(CodeExpression targetObject)
		{
			return targetObject;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="method"></param>
		/// <returns>CodeEventReferenceExpression</returns>
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (IsStatic)
			{
				if (typeof(ProjectResources).Equals(ObjectType))
				{
					ProjectResources rm = ((LimnorProject)(method.ModuleProject)).GetProjectSingleData<ProjectResources>();
					return new CodeEventReferenceExpression(new CodeTypeReferenceExpression(rm.HelpClassName), this.MemberName);
				}
				return new CodeEventReferenceExpression(new CodeTypeReferenceExpression(ObjectType), this.MemberName);
			}
			else
			{
				CodeExpression targetObject;
				if (this.Owner != null)
				{
					targetObject = this.Owner.GetReferenceCode(method, statements, forValue);
				}
				else
				{
					targetObject = this.Holder.GetReferenceCode(method, statements, forValue);
				}
				return new CodeEventReferenceExpression(OnGetTargetObject(targetObject), this.MemberName);
			}
		}
		public override string GetJavaScriptReferenceCode(StringCollection code)
		{
			string o = this.Holder.GetJavaScriptReferenceCode(code);
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", o, this.MemberName);
		}
		public override string GetPhpScriptReferenceCode(StringCollection code)
		{
			string o = this.Holder.GetJavaScriptReferenceCode(code);
			return string.Format(CultureInfo.InvariantCulture, "{0}->{1}", o, this.MemberName);
		}
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public override EnumPointerType PointerType { get { return EnumPointerType.Event; } }
		#endregion
		#region IEvent Members
		[Browsable(false)]
		public bool IsCustomEvent
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool IsOverride
		{
			get { return false; }
		}
		public string Name
		{
			get
			{
				return MemberName;
			}
		}
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
			string methodName = Name + "_" + Guid.NewGuid().GetHashCode().ToString("x");
			EventInfo eif = Info;
			if (eif != null)
			{
				MethodClass mc = new MethodClass(RootPointer);
				mc.MethodName = methodName;
				mc.SetCompilerData(compiler);
				mc.MemberId = (UInt32)(Guid.NewGuid().GetHashCode());
				List<ParameterClass> pcs = new List<ParameterClass>();
				ParameterInfo[] pifs = GetEventParameters(eif);
				if (pifs != null)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						pcs.Add(new ParameterClass(new TypePointer(pifs[i].ParameterType), pifs[i].Name, mc));
					}
				}
				mc.Parameters = pcs;
				return mc;
			}
			return null;
		}
		public DataTypePointer EventHandlerType
		{
			get
			{
				EventInfo eif = Info;
				if (eif == null) return new DataTypePointer(typeof(EventHandler));
				EventInfoX x = eif as EventInfoX;
				if (x != null)
				{
					return x.HandlerType;
				}
				EventInfoInterface eii = eif as EventInfoInterface;
				if (eii != null)
				{
					return eii.HandlerType;
				}
				return new DataTypePointer(new TypePointer(eif.EventHandlerType));
			}
		}
		public List<NamedDataType> GetEventParameters()
		{
			List<NamedDataType> pcList = new List<NamedDataType>();
			ParameterInfo[] ps = Parameters;
			if (ps != null && ps.Length > 0)
			{
				IWithProject p = null;
				if (Owner != null)
				{
					p = Owner.RootPointer;
				}
				for (int i = 0; i < ps.Length; i++)
				{
					NamedDataType pc;
					pc = new NamedDataType(ps[i].ParameterType, ps[i].Name);
					pcList.Add(pc);
				}
			}
			return pcList;
		}
		public List<ParameterClass> GetParameters(IMethod method)
		{
			List<ParameterClass> pcList = new List<ParameterClass>();
			ParameterInfo[] ps = Parameters;
			if (ps != null && ps.Length > 0)
			{
				IWithProject p = null;
				if (Owner != null)
				{
					p = Owner.RootPointer;
				}
				for (int i = 0; i < ps.Length; i++)
				{
					ParameterClass pc;
					pc = new ParameterClass(ps[i].ParameterType, ps[i].Name, p, method);
					pcList.Add(pc);
				}
			}
			return pcList;
		}
		#endregion
	}
}
