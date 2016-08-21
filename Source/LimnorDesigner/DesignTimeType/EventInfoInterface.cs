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
using LimnorDesigner.Interface;
using System.Globalization;

namespace LimnorDesigner.DesignTimeType
{
	public class EventInfoInterface : EventInfo
	{
		private InterfaceElementEvent _event;
		public EventInfoInterface(InterfaceElementEvent ee)
		{
			_event = ee;
		}
		public InterfaceElementEvent Event
		{
			get
			{
				return _event;
			}
		}
		public new Type EventHandlerType
		{
			get
			{
				return _event.DataTypeEx;
			}
		}
		public bool IsStatic
		{
			get
			{
				return _event.IsStatic;
			}
		}
		public DataTypePointer HandlerType
		{
			get
			{
				return _event;
			}
		}
		public ParameterInfo[] GetParameters()
		{
			List<NamedDataType> ps = _event.Parameters;
			if (ps != null && ps.Count > 0)
			{
				ParameterInfoX[] pms = new ParameterInfoX[ps.Count];
				for (int i = 0; i < ps.Count; i++)
				{
					pms[i] = new ParameterInfoX(ps[i]);
				}
				return pms;
			}
			return new ParameterInfo[] { };
		}
		public override EventAttributes Attributes
		{
			get { return EventAttributes.None; }
		}

		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			if (nonPublic)
				return null;
			return new MethodInfoInterfaceEventHandlerAdd(_event);
		}

		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			if (nonPublic)
				return null;
			return new MethodInfoInterfaceEventHandlerRaise(_event);
		}

		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			if (nonPublic)
				return null;
			return new MethodInfoInterfaceEventHandlerRemove(_event);
		}

		public override Type DeclaringType
		{
			get { return _event.Interface.DataTypeEx; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[] { };
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[] { };
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override string Name
		{
			get { return _event.Name; }
		}

		public override Type ReflectedType
		{
			get { return _event.Interface.DataTypeEx; }
		}
	}
	public abstract class MethodInfoInterfaceEventHandler : MethodInfo
	{
		private InterfaceElementEvent _ee;
		public MethodInfoInterfaceEventHandler(InterfaceElementEvent ee)
		{
			_ee = ee;
		}
		public InterfaceElementEvent Event
		{
			get
			{
				return _ee;
			}
		}
		public override MethodInfo GetBaseDefinition()
		{
			return null;
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get { return null; }
		}

		public override MethodAttributes Attributes
		{
			get { return MethodAttributes.Public; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.Managed;
		}

		public override ParameterInfo[] GetParameters()
		{
			return new ParameterInfo[] { new ParameterInfoX(_ee) };
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { return new RuntimeMethodHandle(); }
		}

		public override Type DeclaringType
		{
			get { return _ee.Interface.DataTypeEx; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[] { };
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[] { };
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override string Name
		{
			get { return _ee.Name; }
		}

		public override Type ReflectedType
		{
			get { return _ee.Interface.DataTypeEx; }
		}
	}
	public class MethodInfoInterfaceEventHandlerAdd : MethodInfoInterfaceEventHandler
	{
		public MethodInfoInterfaceEventHandlerAdd(InterfaceElementEvent e)
			: base(e)
		{
		}
		public override string Name
		{
			get { return string.Format(CultureInfo.InvariantCulture, "add_{0}", Name); }
		}
	}
	public class MethodInfoInterfaceEventHandlerRaise : MethodInfoInterfaceEventHandler
	{
		public MethodInfoInterfaceEventHandlerRaise(InterfaceElementEvent e)
			: base(e)
		{
		}
		public override string Name
		{
			get { return string.Format(CultureInfo.InvariantCulture, "raise_{0}", Name); }
		}
		public override ParameterInfo[] GetParameters()
		{
			List<NamedDataType> ps = this.Event.Parameters;
			if (ps != null && ps.Count > 0)
			{
				ParameterInfoX[] pms = new ParameterInfoX[ps.Count];
				for (int i = 0; i < ps.Count; i++)
				{
					pms[i] = new ParameterInfoX(ps[i]);
				}
				return pms;
			}
			return new ParameterInfo[] { };
		}
	}
	public class MethodInfoInterfaceEventHandlerRemove : MethodInfoInterfaceEventHandler
	{
		public MethodInfoInterfaceEventHandlerRemove(InterfaceElementEvent e)
			: base(e)
		{
		}
		public override string Name
		{
			get { return string.Format(CultureInfo.InvariantCulture, "remove_{0}", Name); }
		}
	}
}
