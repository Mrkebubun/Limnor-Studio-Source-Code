/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using VSPrj;
using XmlSerializer;
using LimnorDesigner.Property;
using System.Xml;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using LimnorDesigner.Event;
using LimnorDesigner.Interface;
using VPL;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using LimnorDesigner.Web;

namespace LimnorDesigner
{
	public delegate ILimnorDesignerLoader fnGetLoaderByClassId(UInt32 classId);
	public enum EnumMaxButtonLocation { Left, Right }
	public enum EnumParameterEditType { Edit, TypeReadOnly, ReadOnly }
	public interface ILimnorDesignerLoader : IXmlDesignerLoader, ILimnorDesigner
	{
		string Namespace { get; }
		bool IsInDesign { get; }
		object RootObject { get; }
		string ComponentFilePath { get; }
		ILimnorDesignPane DesignPane { get; }
		IDesignerLoaderHost Host { get; }
		XmlObjectReader Reader { get; }
		XmlObjectWriter Writer { get; }
		PropertyPointer CreatePropertyPointer(object v, string propertyName);
		void ReloadClassRefProperties(ClassInstancePointer cr);
		bool IsNameUsed(string name, UInt32 methodScope);
		void ResetObjectMap();
		void ClearMethodNames();
		void SaveHtmlFile();
	}

	/// <summary>
	/// designer shown in IDE
	/// </summary>
	public interface IXDesignerViewer
	{
		#region IDE interface
		/// <summary>
		/// MultiPanes holds all IXDesignerViewer instances.
		/// It also provides some programming utilities
		/// </summary>
		/// <param name="mp"></param>
		void SetDesigner(MultiPanes mp);
		/// <summary>
		/// this is the control shown in the IDE
		/// </summary>
		/// <returns></returns>
		Control GetDesignSurfaceUI();
		/// <summary>
		/// for loading data. MultiPanes.Loaded is false at this time.
		/// </summary>
		void OnDataLoaded();
		/// <summary>
		/// for loading UI configuration. MultiPanes.Loaded is false at this time.
		/// after calling this function on all IXDesignerViewer instances, MultiPanes.Loaded is true.
		/// </summary>
		void OnUIConfigLoaded();
		/// <summary>
		/// the location for the button for maximizing this designer
		/// </summary>
		EnumMaxButtonLocation MaxButtonLocation { get; }
		/// <summary>
		/// called after compiling to reset the event handlers
		/// </summary>
		void OnResetMap();
		/// <summary>
		/// called after a database connection selection is done, which might change the connection list for the project.
		/// </summary>
		void OnDatabaseListChanged();
		/// <summary>
		/// called after a database connection name has changed
		/// </summary>
		/// <param name="connectionid"></param>
		/// <param name="newName"></param>
		void OnDatabaseConnectionNameChanged(Guid connectionid, string newName);
		/// <summary>
		/// a class is loaded into designer
		/// </summary>
		/// <param name="classId"></param>
		void OnClassLoadedIntoDesigner(UInt32 classId);
		/// <summary>
		/// the designer is closing
		/// </summary>
		void OnClosing();
		#endregion

		#region Programming interface
		//components
		void OnComponentAdded(object obj);
		void OnComponentRename(object obj, string newName);
		void OnComponentRemoved(object obj);
		void OnComponentSelected(object obj);
		void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade);
		void OnResetDesigner(object obj);
		//actions
		void OnActionSelected(IAction action);
		void OnActionChanged(UInt32 classId, IAction act, bool isNewAction);
		void OnActionDeleted(IAction action);
		void OnAssignAction(EventAction ea);
		//event handling
		void OnRemoveEventHandler(EventAction ea, TaskID task);
		void OnRemoveAllEventHandlers(IEvent e);
		void OnActionListOrderChanged(object sender, EventAction ea);
		void OnDeleteEventMethod(EventHandlerMethod method);
		//methods
		void OnMethodChanged(MethodClass method, bool isNewMethod);
		void OnDeleteMethod(MethodClass method);
		void OnMethodSelected(MethodClass method);
		//properties
		void OnDeleteProperty(PropertyClass property);
		void OnAddProperty(PropertyClass property);
		void OnPropertySelected(PropertyClass property);
		void OnPropertyChanged(INonHostedObject property, string name);
		//events
		void OnDeleteEvent(EventClass eventObject);
		void OnAddEvent(EventClass eventObject);
		void OnEventSelected(EventClass eventObject);
		void OnFireEventActionSelected(FireEventMethod method);
		//
		/// <summary>
		/// name changed for objects other than IComponents
		/// </summary>
		/// <param name="obj"></param>
		void OnObjectNameChanged(INonHostedObject obj);
		/// <summary>
		/// the icon for the class identified by classId is changed.
		/// </summary>
		/// <param name="classId"></param>
		void OnIconChanged(UInt32 classId);
		/// <summary>
		/// for convenience of using static members
		/// </summary>
		/// <param name="classId"></param>
		/// <param name="t"></param>
		void OnAddExternalType(UInt32 classId, Type t);
		void OnRemoveExternalType(UInt32 classId, Type t);
		/// <summary>
		/// the icon for the class identified by classId is changed to img
		/// </summary>
		/// <param name="classId"></param>
		/// <param name="img"></param>
		void SetClassRefIcon(UInt32 classId, System.Drawing.Image img);
		/// <summary>
		/// for a class implemented ICustomEventMethodDescriptor, its event list
		/// may change by the developer. this function notifies all designers when
		/// the event list changes.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="objectId"></param>
		void OnEventListChanged(ICustomEventMethodDescriptor owner, UInt32 objectId);
		/// <summary>
		/// an html element is selected on the *_design.html
		/// </summary>
		/// <param name="element"></param>
		void OnHtmlElementSelected(HtmlElement_Base element);
		/// <summary>
		/// selected by a designer from the IDE
		/// </summary>
		/// <param name="guid">guid for the selected html element</param>
		/// <param name="selector">designer making the selection</param>
		void OnSelectedHtmlElement(Guid guid, object selector);
		/// <summary>
		/// an html element is used. guid and id should have been created
		/// </summary>
		/// <param name="element"></param>
		void OnUseHtmlElement(HtmlElement_BodyBase element);
		#endregion

		#region Interface Programming
		void OnInterfaceAdded(InterfacePointer interfacePointer);
		void OnRemoveInterface(InterfacePointer interfacePointer);
		void OnInterfaceMethodDeleted(InterfaceElementMethod method);
		void OnInterfaceMethodChanged(InterfaceElementMethod method);
		void OnInterfaceEventDeleted(InterfaceElementEvent eventType);
		void OnInterfaceEventAdded(InterfaceElementEvent eventType);
		void OnInterfaceEventChanged(InterfaceElementEvent eventType);
		void OnInterfacePropertyDeleted(InterfaceElementProperty property);
		void OnInterfacePropertyAdded(InterfaceElementProperty property);
		void OnInterfacePropertyChanged(InterfaceElementProperty property);
		void OnInterfaceMethodCreated(InterfaceElementMethod method);
		void OnBaseInterfaceAdded(InterfaceClass owner, InterfacePointer baseInterface);
		#endregion

	}
	public enum EnumClassChangeType { None, ClassReloaded, ComponentAdded, ComponentDeleted, ComponentRenamed }
	/// <summary>
	/// implemented by LimnorXmlPane
	/// </summary>
	public interface ILimnorDesignPane
	{
		IWin32Window Window { get; }
		ILimnorDesignerLoader Loader { get; }
		ClassPointer RootClass { get; }
		XmlNode RootXmlNode { get; }
		MultiPanes PaneHolder { get; }
		bool IsClosing { get; set; }
		DesignSurface Surface2 { get; }
		object CurrentObject { get; }
		bool CanBuild { get; }
		//
		void InitToolbox();
		void HideToolbox();
		void BeginApplyConfig();
		void OnClosing();
		void OnClosed();
		void RefreshWebServiceToolbox();
		//
		IComponent CreateComponent(Type type, string name);
		void SelectComponent(IComponent c);
		//
		IList<IXDesignerViewer> GetDettachedViewers();
		//
		void OnIconChanged();
		void AddDesigner(IXDesignerViewer designer, bool detached);
		void RemoveDesigner(IXDesignerViewer designer, bool detached);
		void RefreshPropertyGrid();
		void OnNotifyChanges();
		void OnActionSelected(IAction action);
		void OnActionDeleted(IAction act);
		void OnAssignAction(EventAction ea);
		void OnRemoveEventHandler(EventAction ea, TaskID task);
		void OnActionListOrderChanged(object sender, EventAction ea);
		void OnActionChanged(UInt32 classId, IAction act, bool isNewAction);
		void OnMethodChanged(MethodClass method, bool isNewAction);
		void OnDeleteMethod(MethodClass method);
		void OnDeleteEventMethod(EventHandlerMethod method);
		void OnDeleteProperty(PropertyClass property);
		void OnAddProperty(PropertyClass property);
		void OnPropertyChanged(INonHostedObject property, string name);
		void OnDeleteEvent(EventClass eventObject);
		void OnAddEvent(EventClass eventObject);
		void SaveAll();
		void SetClassRefIcon(UInt32 classId, System.Drawing.Image img);
		void ResetContextMenu();
		void OnDatabaseListChanged();
		void OnDatabaseConnectionNameChanged(Guid connectionid, string newName);
		void OnResetDesigner(object obj);
		//
		void OnComponentRename(object obj, string newName);
		//
		void OnInterfaceAdded(InterfacePointer interfacePointer);
		void OnRemoveInterface(InterfacePointer interfacePointer);
		void OnInterfaceMethodDeleted(InterfaceElementMethod method);
		void OnInterfaceMethodChanged(InterfaceElementMethod method);
		void OnInterfaceEventDeleted(InterfaceElementEvent eventType);
		void OnInterfaceEventAdded(InterfaceElementEvent eventType);
		void OnInterfaceEventChanged(InterfaceElementEvent eventType);
		void OnInterfacePropertyDeleted(InterfaceElementProperty property);
		void OnInterfacePropertyAdded(InterfaceElementProperty property);
		void OnInterfacePropertyChanged(InterfaceElementProperty property);
		void OnInterfaceMethodCreated(InterfaceElementMethod method);
		void OnBaseInterfaceAdded(InterfaceClass owner, InterfacePointer baseInterface);
		//
		void OnCultureChanged();
		void OnResourcesEdited();
		//
		void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade);
		void OnClassLoadedIntoDesigner(UInt32 classId);
		void AddToolboxItem();
		void LoadProjectToolbox();
		//
		void OnAddExternalType(Type type);
		//selection from HTML document
		void OnHtmlElementSelected(HtmlElement_Base element);
		//selection from IDE
		void OnSelectedHtmlElement(Guid guid, object selector);
		//an html element is used. guid and id should have been created
		void OnUseHtmlElement(HtmlElement_BodyBase element);
	}
	/// <summary>
	/// implemented by LimnorXmlDesignerLoader and MethodDebugDesigner
	/// </summary>
	public interface ILimnorDesigner : IWithProject
	{
		/// <summary>
		/// select a data type to create a variable or a method parameter
		/// </summary>
		/// <param name="caller">owner of dialogue boxes</param>
		/// <returns>a TypePointer or a RootClassId</returns>
		IObjectPointer SelectDataType(Form caller);
		bool IsMethodNameUsed(UInt32 methodId, string name);
		string CreateMethodName(string baseName, StringCollection names);
		ObjectIDmap ObjectMap { get; }
		ClassPointer GetRootId();
		IXmlCodeReader CodeReader { get; }
		IXmlCodeWriter CodeWriter { get; }
	}
	public interface ILimnorLoader
	{
		XmlObjectReader Reader { get; }
		XmlObjectWriter Writer { get; }
		XmlNode Node { get; }
		void NotifyChanges();
	}
	/// <summary>
	/// implemented by objects supposed to be edited in the PropertyGrid, other than hosted components.
	/// PropertyClass, NethodClass, EventClass and ActionClass implement it
	/// </summary>
	public interface INonHostedObject
	{
		void OnPropertyChanged(string name, object property, XmlNode rootNode, XmlObjectWriter writer);
		string Name { get; }
		UInt32 ClassId { get; }
		UInt32 MemberId { get; }
		void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange);
	}
}
