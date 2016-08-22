/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace VOB
{
	public class VobSite : ISite, IContainer
	{
		private IComponent _component;
		private IComponent[] _items;
		private bool _designMode = true;
		private string _name = "";
		private IContainer _container = null;

		#region Constructors
		public VobSite(IComponent component)
		{
			_component = component;
			_items = new IComponent[0];
		}
		public VobSite(IServiceProvider serviceProvider, IComponent component)
		{
			_serviceProvider = serviceProvider;
			_component = component;
			_items = new IComponent[0];
		}
		#endregion
		public void SetContainer(IContainer c)
		{
			_container = c;
		}
		public void AddService(Type serviceType, object serviceProvider)
		{
			if (_services == null)
			{
				_services = new Hashtable();
			}
			_services.Add(serviceType, serviceProvider);
		}

		#region ISite Members

		public IComponent Component
		{
			get { return _component; }
		}

		public IContainer Container
		{
			get
			{
				if (_container != null)
					return _container;
				return this;
			}
		}

		public bool DesignMode
		{
			get
			{
				return _designMode;
			}
		}

		public string Name
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

		#endregion

		#region IServiceProvider Members
		IServiceProvider _serviceProvider = null;
		Hashtable _services = null;
		public object GetService(Type serviceType)
		{
			if (_serviceProvider != null)
			{
				object obj = _serviceProvider.GetService(serviceType);
				if (obj != null)
					return obj;
			}
			if (_services != null)
				return _services[serviceType];
			return null;
		}

		#endregion

		#region IContainer Members

		public void Add(IComponent component, string name)
		{
			component.Site.Name = name;
			Add(component);
		}

		public void Add(IComponent component)
		{
			if (_container != null)
			{
				_container.Add(component);
			}
			else
			{
				int n = _items.Length;
				IComponent[] a = new IComponent[n + 1];
				for (int i = 0; i < n; i++)
				{
					a[i] = _items[i];
				}
				_items = a;
				_items[n] = component;
			}
		}

		public ComponentCollection Components
		{
			get
			{
				if (_container != null)
					return _container.Components;
				return new ComponentCollection(_items);
			}
		}

		public void Remove(IComponent component)
		{
			if (_container != null)
			{
				_container.Remove(component);
			}
			else
			{
				int n = _items.Length;
				for (int i = 0; i < n; i++)
				{
					if (_items[i] == component)
					{
						if (n <= 1)
						{
							_items = new IComponent[0];
						}
						else
						{
							IComponent[] a = new IComponent[n - 1];
							for (int k = 0; k < n; k++)
							{
								if (k < i)
									a[k] = _items[k];
								else if (k > i)
									a[k - 1] = _items[k];
							}
							_items = a;
						}
						break;
					}
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

		#endregion
	}
}
