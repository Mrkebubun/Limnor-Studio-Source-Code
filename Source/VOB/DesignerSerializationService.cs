/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Windows.Forms;

namespace VOB
{
	public class DesignerSerializationService : IDesignerSerializationService
	{
		IServiceProvider serviceProvider;

		public DesignerSerializationService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		#region IDesignerSerializationService Members

		public ICollection Deserialize(object serializationData)
		{
			SerializationStore serializationStore = serializationData as SerializationStore;
			if (serializationStore != null)
			{
				ComponentSerializationService componentSerializationService = serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
				ICollection collection = componentSerializationService.Deserialize(serializationStore);
				return collection;
			}
			return new object[0];
		}
		private void serializeSubControls(ComponentSerializationService componentSerializationService, SerializationStore serializationStore, Control c)
		{
			if (c.Controls.Count > 0)
			{
				foreach (Control c0 in c.Controls)
				{
					componentSerializationService.Serialize(serializationStore, c0);
					serializeSubControls(componentSerializationService, serializationStore, c0);
				}
			}
		}
		public object Serialize(ICollection objects)
		{
			ComponentSerializationService componentSerializationService = serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
			SerializationStore returnObject = null;
			using (SerializationStore serializationStore = componentSerializationService.CreateStore())
			{
				foreach (object obj in objects)
				{
					Control c = obj as Control;
					if (c != null)
					{
						componentSerializationService.Serialize(serializationStore, obj);
						serializeSubControls(componentSerializationService, serializationStore, c);
					}
				}
				returnObject = serializationStore;
			}
			return returnObject;
		}

		#endregion
	}
}
