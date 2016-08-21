using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.ComponentModel;
using XmlSerializer;
using MathExp;
using XmlUtility;

namespace LimnorDesigner
{
	public class ComponentLoader : BasicDesignerLoader
	{
		private XmlObjectReader _reader;
		private string _file;
		private IComponent _rootComponent;
		public ComponentLoader(XmlObjectReader reader, string fileName)
		{
			_reader = reader;
			_file = fileName;
		}

		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			throw new NotImplementedException("ComponentLoader is for loading only");
		}

		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			ComponentFactory o = new ComponentFactory();
			o.DesignerLoaderHost = this.LoaderHost;
			_reader.ObjectList.ClearItems();
			_reader.ObjectList.DocumentMoniker = _file;
			_rootComponent = (IComponent)_reader.ReadRootObject(o, _reader.ObjectList.XmlData);

			if (_reader.Errors != null && _reader.Errors.Count > 0)
			{
				MathNode.Log(_reader.Errors);
				_reader.ResetErrors();
			}
			if (_rootComponent != null)
			{
				uint id = _reader.ObjectList.GetObjectID(_rootComponent);
				if (id == 0)
				{
					id = _reader.ObjectList.AddNewObject(_rootComponent);
					if (id != 1)
					{
						throw new Exception("Object ID 1 must be for the root object");
					}
					XmlUtil.SetAttribute(_reader.ObjectList.XmlData, XmlTags.XMLATT_ComponentID, id);
				}
				this.LoaderHost.EndLoad(_rootComponent.Site.Name, true, null);
			}
		}
	}
}
