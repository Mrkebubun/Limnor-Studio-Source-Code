/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace XHost
{
	public class TestLoader : BasicDesignerLoader
	{
		private IDesignerLoaderHost host;
		public TestLoader(Type rootComponentType)
		{
		}
		protected override void PerformLoad(IDesignerSerializationManager designerSerializationManager)
		{
			this.host = this.LoaderHost;
			ArrayList errors = new ArrayList();
			bool successful = true;
			string baseClassName;
			baseClassName = "Home";
			host.EndLoad(baseClassName, successful, errors);
		}

		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{

		}
	}
}
