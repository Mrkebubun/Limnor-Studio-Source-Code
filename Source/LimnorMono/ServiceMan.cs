/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace LimnorVOB
{
	class ServiceMan : IServiceProvider
	{
		Hashtable services;
		public ServiceMan()
		{
			services = new Hashtable();
		}
		public void AddService(Type serviceType, object provider)
		{
			services[serviceType] = provider;
		}
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			return services[serviceType];
		}

		#endregion

	}
}
