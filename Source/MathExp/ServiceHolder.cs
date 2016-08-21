/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
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

namespace MathExp
{
	public class ServiceHolder : IDesignServiceProvider
	{
		Dictionary<Type, object> services;
		public ServiceHolder()
		{
		}
		#region IDesignServiceProvider Members

		public object GetDesignerService(Type serviceType)
		{
			if (services != null)
			{
				if (services.ContainsKey(serviceType))
					return services[serviceType];
			}
			return null;
		}

		public void AddDesignerService(Type serviceType, object service)
		{
			if (services == null)
				services = new Dictionary<Type, object>();
			if (services.ContainsKey(serviceType))
				services[serviceType] = service;
			else
				services.Add(serviceType, service);
		}

		#endregion
	}
}
