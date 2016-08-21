using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LimnorDesigner.Event;

namespace LimnorDesigner.EventMap
{
    public class EventIconCustom : EventIcon
    {
        #region fields and constructors
        public EventIconCustom(Control owner)
            : base(owner)
        {
        }

        public EventIconCustom(Control owner, EventAction ea, EventPathData eventPathData)
            : base(owner, ea, eventPathData)
        {
        }
        #endregion
        #region Properties
        public EventClass CustomEvent
        {
            get
            {
                CustomEventPointer cep = Event as CustomEventPointer;
                return cep.Event;
            }
        }
        #endregion
    }
}
