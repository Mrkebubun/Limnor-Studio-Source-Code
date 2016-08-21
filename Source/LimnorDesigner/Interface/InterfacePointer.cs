/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.Reflection;
using ProgElements;
using System.ComponentModel;
using LimnorDesigner.Property;
using System.CodeDom;
using System.Xml;
using XmlSerializer;
using MathExp;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// pointer to a type which is an interface.
	/// for a custom interface, the _classRef is the designer class for the custom interface
	/// for a lib interface, the _typePointer points to the interface type
	/// </summary>
	public class InterfacePointer : DataTypePointer, IAttributeHolder
	{
		#region fields and constructors
		private UInt32 _classId;
		private InterfaceClass _owner;
		public InterfacePointer()
		{
		}
		/// <summary>
		/// point to a lib interface
		/// </summary>
		/// <param name="type"></param>
		public InterfacePointer(TypePointer type)
			: base(type)
		{
		}
		public InterfacePointer(InterfaceClass owner)
		{
			if (owner == null)
			{
				throw new DesignerException("owner as InterfaceClass is null for InterfacePointer constructor");
			}
			_owner = owner;
			_classId = _owner.ClassId;
			SetInterfaceClass();
		}
		/// <summary>
		/// owner is a designer class for a custom interface
		/// </summary>
		/// <param name="owner"></param>
		public InterfacePointer(ClassPointer owner)
			: base(owner)
		{
			if (owner == null)
			{
				throw new DesignerException("owner as ClassPointer is null for InterfacePointer constructor");
			}
			_owner = owner.Interface;
			_classId = owner.ClassId;
		}
		#endregion
		#region Methods
		public CodeExpression GetTargetObject(CodeExpression targetObject)
		{
			Type t = this.VariableLibType;
			if (t != null)
			{
				return new CodeCastExpression(t, targetObject);
			}
			return new CodeCastExpression(_owner.InterfaceName, targetObject);
		}
		public void SetInterfaceClass()
		{
			if (_owner != null)
			{
				if (_owner.Holder != null)
				{
					SetDataType(_owner.Holder);
				}
			}
		}
		public void SetClassId(UInt32 classId)
		{
			_classId = classId;
		}
		public bool IsNameInUse(string name)
		{
			if (IsValid)
			{
				List<InterfaceElementProperty> ps = GetProperties();
				if (ps != null)
				{
					foreach (InterfaceElementProperty p in ps)
					{
						if (p.Name == name)
						{
							return true;
						}
					}
				}
				List<InterfaceElementMethod> ms = Methods;
				if (ms != null)
				{
					foreach (InterfaceElementMethod m in ms)
					{
						if (m.Name == name)
						{
							return true;
						}
					}
				}
				List<InterfaceElementEvent> es = Events;
				if (es != null)
				{
					foreach (InterfaceElementEvent e in es)
					{
						if (e.Name == name)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public ClassPointer GetHolder()
		{
			ClassPointer cp = this.ClassTypePointer;
			if (cp == null)
			{
				if (_owner != null)
				{
					cp = _owner.Holder;
					if (cp != null)
					{
						SetDataType(cp);
					}
				}
			}
			return cp;
		}
		public void NotifyEventChange(InterfaceElementEvent e)
		{
			ClassPointer cp = GetHolder();
			if (cp != null && _owner != null)
			{
				ILimnorDesignPane pane = cp.Project.GetTypedData<ILimnorDesignPane>(cp.ClassId);
				if (pane != null)
				{
					_owner.NotifyEventChange(pane.Loader, e);
				}
			}
		}
		public void NotifyPropertyChange(InterfaceElementProperty p)
		{
			ClassPointer cp = GetHolder();
			if (cp != null && _owner != null)
			{
				ILimnorDesignPane pane = cp.Project.GetTypedData<ILimnorDesignPane>(cp.ClassId);
				if (pane != null)
				{
					_owner.NotifyPropertyChange(pane.Loader, p);
				}
			}
		}
		public void NotifyMethodChange(InterfaceElementMethod m)
		{
			ClassPointer cp = GetHolder();
			if (cp != null && _owner != null)
			{
				ILimnorDesignPane pane = cp.Project.GetTypedData<ILimnorDesignPane>(cp.ClassId);
				if (pane != null)
				{
					_owner.NotifyMethodChange(pane.Loader, m);
				}
			}
		}
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			InterfacePointer ip = objectPointer as InterfacePointer;
			if (ip != null)
			{
				if (ip.IsLibType)
				{
					if (this.IsLibType)
					{
						if (ip.VariableLibType.Equals(this.VariableLibType))
						{
							return true;
						}
					}
				}
				else
				{
					return (ip.ClassId == this.ClassId);
				}
			}
			return false;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsAdded
		{
			get
			{
				return (ImplementerClassId != 0);
			}
		}
		/// <summary>
		/// 0: implemented by the base lib type
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 ImplementerClassId
		{
			get;
			set;
		}
		public override uint ClassId
		{
			get
			{
				if (_classId == 0)
				{
					if (this.ClassTypePointer != null)
					{
						return this.ClassTypePointer.ClassId;
					}
				}
				return _classId;
			}
		}
		public List<InterfaceElementProperty> GetProperties()
		{
			ClassPointer cp = this.ClassTypePointer;
			if (cp != null)
			{
				if (!cp.IsInterface)
				{
					throw new DesignerException("Invalid interface pointer. Class Id:{0}", cp.ClassId);
				}
				if (cp.ObjectInstance == null)
				{
					throw new DesignerException("Interface object not loaded. Class Id:{0}", cp.ClassId);
				}
				InterfaceClass ifc = VPLUtil.GetObject(cp.ObjectInstance) as InterfaceClass;
				if (ifc == null)
				{
					throw new DesignerException("Invalid interface pointer. object:{0}", cp.ObjectInstance);
				}
				return ifc.Properties;
			}
			else
			{
				Type t = this.VariableLibType;
				if (t == null)
				{
					throw new DesignerException("Invalid interface pointer. VariableLibType is null.");
				}
				if (t.IsInterface)
				{
					List<InterfaceElementProperty> ls = new List<InterfaceElementProperty>();
					PropertyInfo[] mifs = VPLUtil.GetProperties(t);
					if (mifs != null && mifs.Length > 0)
					{
						for (int i = 0; i < mifs.Length; i++)
						{
							if (!mifs[i].IsSpecialName)
							{
								InterfaceElementProperty em = new InterfaceElementProperty(this, mifs[i]);
								ls.Add(em);
							}
						}
					}
					return ls;
				}
				else
				{
					throw new DesignerException("Invalid interface pointer. Type is not an interface:{0}.", t);
				}
			}
		}
		public List<InterfaceElementMethod> Methods
		{
			get
			{
				ClassPointer cp = this.ClassTypePointer;
				if (cp != null)
				{
					if (!cp.IsInterface)
					{
						throw new DesignerException("Invalid interface pointer. Class Id:{0}", cp.ClassId);
					}
					if (cp.ObjectInstance == null)
					{
						throw new DesignerException("Interface object not loaded. Class Id:{0}", cp.ClassId);
					}
					InterfaceClass ifc = VPLUtil.GetObject(cp.ObjectInstance) as InterfaceClass;
					if (ifc == null)
					{
						throw new DesignerException("Invalid interface pointer. object:{0}", cp.ObjectInstance);
					}
					return ifc.Methods;
				}
				else
				{
					Type t = this.VariableLibType;
					if (t == null)
					{
						throw new DesignerException("Invalid interface pointer. VariableLibType is null.");
					}
					if (t.IsInterface)
					{
						List<InterfaceElementMethod> ls = new List<InterfaceElementMethod>();
						MethodInfo[] mifs = VPLUtil.GetMethods(t);
						if (mifs != null && mifs.Length > 0)
						{
							for (int i = 0; i < mifs.Length; i++)
							{
								if (!mifs[i].IsSpecialName)
								{
									InterfaceElementMethod em = new InterfaceElementMethod(this);
									ls.Add(em);
									em.SetName(mifs[i].Name);
									em.ReturnType = new DataTypePointer(new TypePointer(mifs[i].ReturnType));
									em.Parameters = new List<InterfaceElementMethodParameter>();
									ParameterInfo[] pifs = mifs[i].GetParameters();
									if (pifs != null && pifs.Length > 0)
									{
										for (int k = 0; k < pifs.Length; k++)
										{
											InterfaceElementMethodParameter dp = new InterfaceElementMethodParameter(pifs[k].ParameterType, pifs[k].Name, em);
											em.Parameters.Add(dp);
										}
									}
								}
							}
						}
						return ls;
					}
					else
					{
						throw new DesignerException("Invalid interface pointer. Type is not an interface:{0}.", t);
					}
				}
			}
		}
		public List<InterfaceElementEvent> Events
		{
			get
			{
				ClassPointer cp = this.ClassTypePointer;
				if (cp != null)
				{
					if (!cp.IsInterface)
					{
						throw new DesignerException("Invalid interface pointer. Class Id:{0}", cp.ClassId);
					}
					InterfaceClass ifc = VPLUtil.GetObject(cp.ObjectInstance) as InterfaceClass;
					if (ifc == null)
					{
						throw new DesignerException("Invalid interface pointer. object:{0}", cp.ObjectInstance);
					}
					return ifc.Events;
				}
				else
				{
					Type t = this.VariableLibType;
					if (t == null)
					{
						throw new DesignerException("Invalid interface pointer. VariableLibType is null.");
					}
					if (t.IsInterface)
					{
						List<InterfaceElementEvent> ls = new List<InterfaceElementEvent>();
						EventInfo[] mifs = VPLUtil.GetEvents(t);
						if (mifs != null && mifs.Length > 0)
						{
							for (int i = 0; i < mifs.Length; i++)
							{
								if (!mifs[i].IsSpecialName)
								{
									InterfaceElementEvent em = new InterfaceElementEvent(this);
									em.SetDataType(mifs[i].EventHandlerType);
									em.SetName(mifs[i].Name);
									ls.Add(em);
								}
							}
						}
						return ls;
					}
					else
					{
						throw new DesignerException("Invalid interface pointer. Type is not an interface:{0}.", t);
					}
				}
			}
		}
		public List<InterfacePointer> BaseInterfaces
		{
			get
			{
				ClassPointer cp = this.ClassTypePointer;
				if (cp != null)
				{
					if (!cp.IsInterface)
					{
						throw new DesignerException("Invalid interface pointer. Class Id:{0}", cp.ClassId);
					}
					InterfaceClass ifc = VPLUtil.GetObject(cp.ObjectInstance) as InterfaceClass;
					if (ifc == null)
					{
						throw new DesignerException("Invalid interface pointer. object:{0}", cp.ObjectInstance);
					}
					return ifc.BaseInterfaces;
				}
				else
				{
					Type t = this.VariableLibType;
					if (t == null)
					{
						throw new DesignerException("Invalid interface pointer. VariableLibType is null.");
					}
					if (t.IsInterface)
					{
						List<InterfacePointer> ls = new List<InterfacePointer>();
						Type[] ts = t.GetInterfaces();
						if (ts != null && ts.Length > 0)
						{
							for (int i = 0; i < ts.Length; i++)
							{
								InterfacePointer ip = new InterfacePointer(new TypePointer(ts[i]));
								ls.Add(ip);
							}
						}
						return ls;
					}
					else
					{
						throw new DesignerException("Invalid interface pointer. Type is not an interface:{0}.", t);
					}
				}
			}
		}
		#endregion
		#region IAttributeHolder Members

		public void AddAttribute(ConstObjectPointer attr)
		{
			_owner.AddAttribute(attr);
		}

		public void RemoveAttribute(ConstObjectPointer attr)
		{
			_owner.RemoveAttribute(attr);
		}

		public List<ConstObjectPointer> GetCustomAttributeList()
		{
			return _owner.GetCustomAttributeList();
		}

		#endregion
	}
}
