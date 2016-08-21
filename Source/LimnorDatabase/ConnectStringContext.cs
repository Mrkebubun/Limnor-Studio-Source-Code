/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LimnorDatabase
{
	class ConnectStringContext : ITypeDescriptorContext
	{
		private Connection _connect;
		public ConnectStringContext(Connection connect)
		{
			_connect = connect;
		}
		#region ITypeDescriptorContext Members

		public IContainer Container
		{
			get { throw new NotImplementedException(); }
		}

		public object Instance
		{
			get { return _connect; }
		}

		public void OnComponentChanged()
		{
			throw new NotImplementedException();
		}

		public bool OnComponentChanging()
		{
			throw new NotImplementedException();
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
