/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace XmlSerializer
{
	public class DesignerLoaderHost : IDesignerLoaderHost, IContainer
	{
		#region fields and constructors
		private bool _loading;
		private IComponent _root;
		public DesignerLoaderHost()
		{
		}
		#endregion

		#region Methods
		public void SetRoot(IComponent c)
		{
			_root = c;
		}
		#endregion

		#region IDesignerLoaderHost Members

		public void EndLoad(string baseClassName, bool successful, ICollection errorCollection)
		{
			if (LoadComplete != null)
			{
				LoadComplete(this, EventArgs.Empty);
			}
			_loading = false;
		}

		public void Reload()
		{
			_loading = true;
		}

		#endregion

		#region IDesignerHost Members

		public void Activate()
		{
			if (Activated != null)
			{
				Activated(this, EventArgs.Empty);
			}
		}

		public event EventHandler Activated;

		public IContainer Container
		{
			get { return this; }
		}

		public IComponent CreateComponent(Type componentClass, string name)
		{
			IComponent c = CreateComponent(componentClass);
			if (c.Site == null)
			{
				c.Site = new XSite(c);
			}
			if (c.Site != null)
			{
				c.Site.Name = name;
			}
			return c;
		}

		public IComponent CreateComponent(Type componentClass)
		{
			IComponent c = (IComponent)Activator.CreateInstance(componentClass);
			return c;
		}

		public System.ComponentModel.Design.DesignerTransaction CreateTransaction(string description)
		{
			return null;
		}

		public System.ComponentModel.Design.DesignerTransaction CreateTransaction()
		{
			return null;
		}



		public void DestroyComponent(IComponent component)
		{
			Remove(component);
		}

		public System.ComponentModel.Design.IDesigner GetDesigner(IComponent component)
		{
			return null;
		}

		public Type GetType(string typeName)
		{
			return Type.GetType(typeName);
		}

		public bool InTransaction
		{
			get { return false; }
		}


		public bool Loading
		{
			get { return _loading; }
		}

		public IComponent RootComponent
		{
			get { return _root; }
		}

		public string RootComponentClassName
		{
			get { return _root.GetType().Name; }
		}


		public string TransactionDescription
		{
			get { return ""; }
		}

		public event DesignerTransactionCloseEventHandler TransactionClosed { add { } remove { } }

		public event DesignerTransactionCloseEventHandler TransactionClosing { add { } remove { } }

		public event EventHandler LoadComplete;

		public event EventHandler TransactionOpened { add { } remove { } }

		public event EventHandler TransactionOpening { add { } remove { } }
		public event EventHandler Deactivated { add { } remove { } }

		#endregion

		#region IServiceContainer Members
		Dictionary<Type, object> _services;
		public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
		{
			if (_services == null)
			{
				_services = new Dictionary<Type, object>();
			}
			if (!_services.ContainsKey(serviceType))
			{
				_services.Add(serviceType, Activator.CreateInstance(serviceType));
			}
		}

		public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback)
		{
			if (_services == null)
			{
				_services = new Dictionary<Type, object>();
			}
			if (!_services.ContainsKey(serviceType))
			{
				_services.Add(serviceType, Activator.CreateInstance(serviceType));
			}
		}

		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (_services == null)
			{
				_services = new Dictionary<Type, object>();
			}
			if (_services.ContainsKey(serviceType))
			{
				_services[serviceType] = serviceInstance;
			}
			else
			{
				_services.Add(serviceType, serviceInstance);
			}
		}

		public void AddService(Type serviceType, object serviceInstance)
		{
			if (_services == null)
			{
				_services = new Dictionary<Type, object>();
			}
			if (_services.ContainsKey(serviceType))
			{
				_services[serviceType] = serviceInstance;
			}
			else
			{
				_services.Add(serviceType, serviceInstance);
			}
		}

		public void RemoveService(Type serviceType, bool promote)
		{
			if (_services != null)
			{
				if (_services.ContainsKey(serviceType))
				{
					_services.Remove(serviceType);
				}
			}
		}

		public void RemoveService(Type serviceType)
		{
			if (_services != null)
			{
				if (_services.ContainsKey(serviceType))
				{
					_services.Remove(serviceType);
				}
			}
		}

		#endregion

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (_services != null)
			{
				if (_services.ContainsKey(serviceType))
				{
					return _services[serviceType];
				}
			}
			return null;
		}

		#endregion

		#region IContainer Members
		List<IComponent> _components;
		public void Add(IComponent component, string name)
		{
			if (_components == null)
				_components = new List<IComponent>();
			if (!_components.Contains(component))
				_components.Add(component);
			if (component.Site == null)
				component.Site = new XSite(component);
			component.Site.Name = name;
		}

		public void Add(IComponent component)
		{
			if (_components == null)
				_components = new List<IComponent>();
			if (!_components.Contains(component))
				_components.Add(component);
		}

		public ComponentCollection Components
		{
			get
			{
				if (_components == null)
					_components = new List<IComponent>();
				IComponent[] a = new IComponent[_components.Count];
				_components.CopyTo(a);
				ComponentCollection cc = new ComponentCollection(a);
				return cc;
			}
		}

		public void Remove(IComponent component)
		{
			if (_components != null)
			{
				if (_components.Contains(component))
				{
					_components.Remove(component);
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_components != null)
			{
				foreach (IComponent c in _components)
				{
					c.Dispose();
				}
				_components = null;
			}
		}

		#endregion
	}
}
