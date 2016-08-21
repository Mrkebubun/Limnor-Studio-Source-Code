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
using LimnorDesigner.Event;
using System.Globalization;

namespace LimnorDesigner.DesignTimeType
{
	public class EventInfoX : EventInfo
	{
		private EventClass _event;
		private TypeX _type;
		public EventInfoX(EventClass e)
		{
			_event = e;
			_type = e.RootPointer.TypeDesigntime;
		}
		public EventClass Event
		{
			get
			{
				return _event;
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
				return _event.EventHandlerType;
			}
		}
		public new Type EventHandlerType
		{
			get
			{
				return _event.EventHandlerType.DataTypeEx;
			}
		}
		public override EventAttributes Attributes
		{
			get { return EventAttributes.None; }
		}

		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			if (nonPublic)
				return null;
			return new MethodInfoEventHandlerAdd(_event);
		}

		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			if (nonPublic)
				return null;
			return new MethodInfoEventHandlerRaise(_event);
		}

		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			if (nonPublic)
				return null;
			return new MethodInfoEventHandlerRemove(_event);
		}

		public override Type DeclaringType
		{
			get { return _type; }
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
			get { return _type; }
		}
	}
	public abstract class MethodInfoEventHandler : MethodInfo
	{
		private EventClass _event;
		private TypeX _type;
		public MethodInfoEventHandler(EventClass e)
		{
			_event = e;
			_type = e.RootPointer.TypeDesigntime;
		}
		public EventClass Owner
		{
			get
			{
				return _event;
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
			return new ParameterInfo[]{
                new ParameterInfoX(new NamedDataType(_event.EventHandlerType.DataTypeEx, "value"))};
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
			get { return _type; }
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

		public override Type ReflectedType
		{
			get { return _type; }
		}
	}
	public class MethodInfoEventHandlerAdd : MethodInfoEventHandler
	{
		public MethodInfoEventHandlerAdd(EventClass e)
			: base(e)
		{
		}
		public override string Name
		{
			get { return string.Format(CultureInfo.InvariantCulture, "add_{0}", Owner.Name); }
		}
	}
	public class MethodInfoEventHandlerRaise : MethodInfoEventHandler
	{
		public MethodInfoEventHandlerRaise(EventClass e)
			: base(e)
		{
		}
		public override string Name
		{
			get { return string.Format(CultureInfo.InvariantCulture, "raise_{0}", Owner.Name); }
		}
		public override ParameterInfo[] GetParameters()
		{
			return Owner.GetParameters();
		}
	}
	public class MethodInfoEventHandlerRemove : MethodInfoEventHandler
	{
		public MethodInfoEventHandlerRemove(EventClass e)
			: base(e)
		{
		}
		public override string Name
		{
			get { return string.Format(CultureInfo.InvariantCulture, "remove_{0}", Owner.Name); }
		}
	}
}
