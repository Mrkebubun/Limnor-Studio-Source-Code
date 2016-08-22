using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
    public interface IWebClientEventHandler
    {
        void OnHandleWebClientEvent(StringCollection code, string eventName, string handlerName);
    }
}
