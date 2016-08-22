/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace XmlSerializer
{
	public class XSite : ISite, IContainer
	{
		IComponent _owner;
		IContainer _container;
		public XSite(IComponent owner)
		{
			_owner = owner;
			_owner.Site = this;
			_container = null;
		}
		public XSite(IComponent owner, IContainer c)
		{
			_owner = owner;
			_owner.Site = this;
			_container = c;
		}
		#region ISite Members

		public IComponent Component
		{
			get { return _owner; }
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
			get { return true; }
		}
		string _name;
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
		public object GetService(Type serviceType)
		{
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
