/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;

namespace VPL
{
	public class XEventService : IEventBindingService
	{
		public XEventService()
		{
		}
		#region IEventBindingService Members

		public string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			return component.Site.Name + "_" + e.Name;
		}

		public ICollection GetCompatibleMethods(EventDescriptor e)
		{
			return new StringCollection();
		}

		public EventDescriptor GetEvent(PropertyDescriptor property)
		{
			return new XEventDescriptor(property);
		}

		public PropertyDescriptorCollection GetEventProperties(EventDescriptorCollection events)
		{
			PropertyDescriptor[] pds = new PropertyDescriptor[events.Count];
			for (int i = 0; i < pds.Length; i++)
			{
				pds[i] = GetEventProperty(events[i]);
			}
			return new PropertyDescriptorCollection(pds);
		}

		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return new XPropertyDescriptor(e);
		}

		public bool ShowCode(IComponent component, EventDescriptor e)
		{
			return false;
		}

		public bool ShowCode(int lineNumber)
		{
			System.Windows.Forms.MessageBox.Show("Show code for " + lineNumber.ToString());
			return false;
		}

		public bool ShowCode()
		{
			System.Windows.Forms.MessageBox.Show("Show code");
			return false;
		}

		#endregion
	}
	public class XEventDescriptor : EventDescriptor
	{
		private MemberDescriptor memberDesc;
		private string _name;
		private Attribute[] _attrs;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descr">a PropertyDescriptor</param>
		public XEventDescriptor(MemberDescriptor descr)
			: base(descr)
		{
			memberDesc = descr;
			_name = descr.Name;
			_attrs = new Attribute[descr.Attributes.Count];
			descr.Attributes.CopyTo(_attrs, 0);
		}
		public XEventDescriptor(MemberDescriptor descr, Attribute[] attrs)
			: base(descr, attrs)
		{
			memberDesc = descr;
			_name = descr.Name;
			_attrs = attrs;
		}
		public XEventDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
			memberDesc = null;
			_name = name;
			_attrs = attrs;
		}
		public string MemberName { get { return _name; } }
		public override void AddEventHandler(object component, Delegate value)
		{
			((System.ComponentModel.EventDescriptor)memberDesc).AddEventHandler(component, value);
		}

		public override Type ComponentType
		{
			get
			{
				return ((System.ComponentModel.EventDescriptor)memberDesc).ComponentType;
			}
		}

		public override Type EventType
		{
			get
			{
				XPropertyDescriptor p = memberDesc as XPropertyDescriptor;
				if (p != null)
				{
					return p.PropertyType;
				}
				return null;
			}
		}

		public override bool IsMulticast
		{
			get
			{
				return ((System.ComponentModel.EventDescriptor)memberDesc).IsMulticast;
			}
		}

		public override void RemoveEventHandler(object component, Delegate value)
		{
			((System.ComponentModel.EventDescriptor)memberDesc).RemoveEventHandler(component, value);
		}
	}
	public class XPropertyDescriptor : PropertyDescriptor
	{
		private EventDescriptor memberDesc;
		private string _name;
		private Attribute[] _attrs;
		//
		private string _value = null;
		private Delegate eventHandler = null;
		//
		private static Dictionary<object, Dictionary<string, string>> handlers = null;
		public XPropertyDescriptor(MemberDescriptor descr)
			: base(descr)
		{
			memberDesc = (EventDescriptor)descr;
			_name = descr.Name;
			_attrs = new Attribute[descr.Attributes.Count];
			descr.Attributes.CopyTo(_attrs, 0);
		}
		public XPropertyDescriptor(MemberDescriptor descr, Attribute[] attrs)
			: base(descr, attrs)
		{
			memberDesc = (EventDescriptor)descr;
			_name = descr.Name;
			_attrs = attrs;
		}
		public XPropertyDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
			memberDesc = null;
			_name = name;
			_attrs = attrs;
		}
		public string MemberName { get { return _name; } }
		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get
			{
				return memberDesc.ComponentType;
			}
		}

		public override object GetValue(object component)
		{
			if (component != null)
			{
				if (handlers != null)
				{
					if (handlers.ContainsKey(component))
					{
						Dictionary<string, string> thisHandlers = handlers[component];
						if (thisHandlers.ContainsKey(memberDesc.Name))
						{
							string h = thisHandlers[memberDesc.Name];
							return h;
						}
					}
				}
			}
			//return _value;
			return eventHandler;
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				Type t = ((System.ComponentModel.EventDescriptor)memberDesc).EventType;
				return t;
			}
		}

		public override void ResetValue(object component)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		/// <summary>
		/// assign event handler
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		public override void SetValue(object component, object value)
		{
			if (component != null && value != null)
			{
				_value = value.ToString();
				Type t = component.GetType();
				System.Reflection.MethodInfo mi = t.GetMethod(_value);
				if (mi != null)
				{
					eventHandler = Delegate.CreateDelegate(typeof(System.EventHandler), component, mi);
					if (handlers == null)
					{
						handlers = new Dictionary<object, Dictionary<string, string>>();
					}
					Dictionary<string, string> thisHandlers = null;
					if (handlers.ContainsKey(component))
					{
						thisHandlers = handlers[component];
					}
					if (thisHandlers == null)
					{
						thisHandlers = new Dictionary<string, string>();
						handlers.Add(component, thisHandlers);
					}
					if (thisHandlers.ContainsKey(memberDesc.Name))
						thisHandlers[memberDesc.Name] = _value;
					else
						thisHandlers.Add(memberDesc.Name, _value);
					((System.ComponentModel.EventDescriptor)memberDesc).AddEventHandler(component, eventHandler);
				}
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
