using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;

namespace VOB
{
    public class XSerializer : IDesignerSerializationService
    {
        IServiceProvider serviceProvider;

        public XSerializer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #region IDesignerSerializationService Members

        public System.Collections.ICollection Deserialize(object serializationData)
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

        public object Serialize(System.Collections.ICollection objects)
        {
            ComponentSerializationService componentSerializationService = serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
            SerializationStore returnObject = null;
            using (SerializationStore serializationStore = componentSerializationService.CreateStore())
            {
                foreach (object obj in objects)
                {
                    //if (obj is Control)
                    //{
                        componentSerializationService.Serialize(serializationStore, obj);
                    //}
                }
                returnObject = serializationStore;
            }
            return returnObject;
        }

        #endregion
    }
}
