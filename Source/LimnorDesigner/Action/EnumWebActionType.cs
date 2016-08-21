/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDesigner.Action
{
    /// <summary>
    /// value type 
    /// a value cannot involve both client side and server side. so, determining value type can be done in one paass.
    /// </summary>
    public enum EnumWebValueType
    {
        Unknown = 0,  //not checked
        Constant,     //do not affect the action type
        Client,       //only involves client side values
        Server,       //only involves server side values
        CustomMethodParameter //used as if it were a constant
    }
    /// <summary>
    /// indicates sources for an IList<IPropertyPointer>
    /// </summary>
    public enum EnumWebValueSources
    {
        Unknown         = 0,
        HasClientValues = 1,
        HasServerValues = 2,
        HasBothValues   = 3 //should not allow this case
    }
    /// <summary>
    /// action type as a member of IAction: WebActionType
    /// a recursive situation is for custom method.
    /// IAction use another member CustomActionWebType for Custom type. not using bit-wise to simplify programming.
    ///   CustomActionWebType =
    ///     Unknown  - Custom Method = Unknown
    ///     Client   - Custom Method = Client and parameters values not include Server
    ///     Download - Custom Method = Client and parameters values include Server
    ///     Server   - Custom Method = Server and parameters values not include Client
    ///     Upload   - Custom Method = Server and parameters values include Client
    ///     Custom   - Custom Method = ClientServer
    /// </summary>
    public enum EnumWebActionType
    {
        Unknown = 0, //not checked
        Client,      //client action involving client values
        Download,    //client action having server values
        Server,      //server action involving server values
        Upload      //server action having client values
        //Custom       //ActionMethod is a custom method, further type info is in CustomActionWebType
    }
    /*
     * 1. check all actions for non-custom methods; set all action of custom method to Custom
     * 2. check all custom methods. for each custom method,
     *      Client - if all actions are Client or Custom with Client custom method
     *      Server - if all actions are Server or Custom with Server custom method
     *      ClientServer - if actions include both Client and Server, 
     *          or Download|Upload, 
     *          or Custom actions include both Client and server custom methods,
     *          or Custom actions include Unknown custom methods with both Client and Server TemporaryMethodType
     *      Unknown - if actions include Custom with method type = Unknown
     *          TemporaryMethodType -
     *              Client - all non-custom actions are Client and all known-custom-actions are Client
     *              Server - all non-custom actions are Server and all known-custom-actions are Server
     * 
     *      
     * 3. get count of unknown custom methods
     *    go to 2. 3.
     *    if the count does not decrease then recursions are used
     *      for each unknown custom methods
     *          for each custom action with Custom type, use method type and TemporaryMethodType to determine method type
     *       
     *      MethodClass:
     *      EnumWebMethodType CheckRecursiveMethodType(List<UInt32> usedActions, List<UInt32> usedMethods, EnumWebMethodType initType)
     *      {
     *      }
     */
    /// <summary>
    /// a property of MethodClass
    /// </summary>
    public enum EnumMethodWebUsage
    {
        /// <summary>
        /// executers are server objects 
        /// - all local variables are server variables (C# variables)
	    /// - may include Upload actions
	    /// - may include Server actions
	    /// - may include Custom action with Server MethodClass
	    /// Server actions are compiled into c# {methodname}(args0)
	    /// Upload actions are compiled into js {methodname}_u(args0) and c# {methodname}(args0)
	    /// Since no mixed sequence of client actions and server actions, and the client code is only for retrieving client values, the action order is preserved.
        /// </summary>
        Server = 0,
        /// <summary>
        /// executers are client objects
	    /// - all local variables are client variables (js variables)
	    /// - may include Client actions
	    /// - may include Download actions
	    /// - may include Custom actions with Client MethodClass
	    /// Client actions are compiled in js {methodname}_d(args0)
	    /// Download actions are compiled into c# {methodname)(args0) and js {methodname}_d(args0, args). 
	    /// Since no mixed sequence of client actions and server actions, and the server code is only for retrieving server values and call  js {methodname}_d, the action order is preserved.
        /// 
        /// </summary>
        Client = 1,
        /// <summary>
        /// Event handler methd for web client event
        /// - a Client method.
	    /// - no parameters
	    /// - all local variables are client variables (js variables)
	    /// - may include all kinds of actions
	    /// Since event handlers cannot be used to create actions, we avoid multi-level upload/download actions
        /// </summary>
        //ClientEventHandler
    }
}
