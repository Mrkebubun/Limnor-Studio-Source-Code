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
using XmlUtility;
using ProgElements;
using VSPrj;
using LimnorDesigner.Action;
using LimnorDesigner.MethodBuilder;
using VPL;
using System.Globalization;
using System.Collections;

namespace LimnorDesigner
{
	/// <summary>
	/// when a ActionBranch is a ISubMethod, it may have parameters.
	/// an Array's ExecuteForEachItem has an index and a value as parameters
	/// </summary>
	public abstract class ParameterClassSubMethod : ActionBranchParameter, IUseClassId, IPostOwnersSerialize
	{
		#region fields and constructors
		private UInt32 _classId;
		private UInt32 _methodId;
		private IObjectPointer _owner;//a local variable
		private MethodClass _scopeMethod; //method the action involving this parameter is located
		//for loading
		public ParameterClassSubMethod()
			: base((IMethod)null)
		{
		}
		public ParameterClassSubMethod(IMethod method)
			: base(method)
		{
		}
		public ParameterClassSubMethod(ActionBranch branch)
			: base(branch)
		{
		}
		public ParameterClassSubMethod(ComponentIconActionBranchParameter componentIcon)
			: base(componentIcon)
		{
		}
		public ParameterClassSubMethod(Type type, string name, ActionBranch branch)
			: base(type, name, branch)
		{
		}

		#endregion
		#region Properties
		[Browsable(false)]
		public override UInt32 ClassId
		{
			get
			{
				if (_classId == 0)
				{
					LocalVariable v = Owner as LocalVariable;
					if (v != null)
					{
						_classId = v.ClassId;
					}
				}
				return _classId;
			}
		}
		[Browsable(false)]
		public override UInt32 MethodId
		{
			get
			{
				if (_methodId == 0)
				{
					if (_scopeMethod == null)
					{
						MethodClass mc = Method as MethodClass;
						if (mc != null)
						{
							IAction a = mc.GetActionInstance(ActionId);
							if (a != null)
							{
								_scopeMethod = a.ScopeMethod as MethodClass;
							}
						}
					}
					if (_scopeMethod == null)
					{
						LimnorProject prj = this.Project;
						if (prj != null)
						{
							ClassPointer cp = ClassPointer.CreateClassPointer(ClassId, prj);
							if (cp != null)
							{
							}
						}
					}
					if (_scopeMethod != null)
					{
						_methodId = _scopeMethod.MethodID;
					}
				}
				return _methodId;
			}
		}
		/// <summary>
		/// a sub method is always associated with an action of type ActionSubMethod
		/// </summary>
		[Browsable(false)]
		public UInt32 ActionId { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		public override IMethod Method
		{
			get
			{
				return base.Method;
			}
			set
			{
				base.Method = value;
				SubMethodInfoPointer m = (SubMethodInfoPointer)value;
				ActionId = m.ActionOwner.ActionId;
			}
		}
		[Browsable(false)]
		public override string CodeName
		{
			get
			{
				if (VPLUtil.CompilerContext_PHP)
				{
					return string.Format(CultureInfo.InvariantCulture, "${0}_{1}", Name, ActionId.ToString("x", CultureInfo.InvariantCulture));
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", Name, ActionId.ToString("x", CultureInfo.InvariantCulture));
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
				if (Owner != null)
				{
					return Owner.DisplayName + "." + Name;
				}
				return base.DisplayName;
			}
		}
		[Browsable(false)]
		public override IObjectPointer Owner
		{
			get
			{
				if (_owner == null)
				{
					IMethod md = Method;
					SubMethodInfoPointer m = md as SubMethodInfoPointer;
					if (m != null)
					{
						_owner = m.ActionOwner.MethodOwner;
					}
					else
					{
					}
				}
				if (_owner == null)
				{
					_owner = base.Owner;
				}
				return _owner;
			}
			set
			{
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ActionID, ActionId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			XmlObjectReader reader = (XmlObjectReader)reader0;
			base.OnReadFromXmlNode(reader0, node);
			if (_classId == 0)
			{
				_classId = reader.ObjectList.ClassId;
			}
			ActionId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ActionID);
			if (base.Method == null)
			{
				IAction a = null;
				if (reader0.ObjectStack != null)
				{
					if (reader0.ObjectStack.Count > 0)
					{
						IEnumerator ie = reader0.ObjectStack.GetEnumerator();
						while (ie.MoveNext())
						{
							a = ie.Current as IAction;
							if (a != null)
							{
								if (a.ActionId == ActionId)
								{
									base.Method = a.ActionMethod.MethodPointed;
									break;
								}
							}
						}
					}
				}
				if (base.Method == null)
				{
					if (reader.ObjectStack != null && reader.ObjectStack.Count > 0)
					{
						IEnumerator ie = reader.ObjectStack.GetEnumerator();
						while (ie.MoveNext())
						{
							IActionsHolder actsHolder = ie.Current as IActionsHolder;
							if (actsHolder != null)
							{
								a = actsHolder.TryGetActionInstance(ActionId);
								if (a != null)
								{
									base.Method = a.ActionMethod.MethodPointed;
									break;
								}
							}
						}
					}
				}
			}
			if (base.Method == null)
			{
				reader0.AddPostOwnersDeserializers(this);
			}
		}
		#endregion

		#region IUseClassId Members

		public void SetClassId(uint classId)
		{
			_classId = classId;
		}
		public void SetMethodId(uint methodId)
		{
			_methodId = methodId;
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			ParameterClassSubMethod sm = (ParameterClassSubMethod)base.Clone();
			sm._classId = _classId;
			sm._methodId = _methodId;
			sm.ActionId = this.ActionId;
			return sm;
		}
		#endregion

		#region IPostOwnersSerialize Members

		public void OnAfterReadOwners(IXmlCodeReader serializer, XmlNode node, object[] owners)
		{
			if (Method == null && owners != null && owners.Length > 0 && ActionId != 0)
			{
				IActionsHolder actsHolder = owners[0] as IActionsHolder;
				if (actsHolder != null)
				{
					Dictionary<UInt32, IAction> acts = actsHolder.ActionInstances;
					if (acts != null)
					{
						IAction a;
						if (acts.TryGetValue(ActionId, out a))
						{
							if (a.ActionMethod == null)
							{
								throw new DesignerException("Error locating method for loop action [{0}]", ActionId);
							}
							Method = a.ActionMethod.MethodPointed;
						}
					}
				}
			}
		}

		#endregion
	}
}
