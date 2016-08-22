/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using LimnorDesigner;
using System.Runtime.InteropServices.ComTypes;
using VPL;
using LimnorDesigner.Interface;
using LimnorDesigner.Event;
using LimnorDesigner.Property;
using LimnorDesigner.Web;

namespace XHost
{
	[Description("GUI designer for building graphic user interface based on Windows Forms.")]
	public class FormViewer : Control, IXDesignerViewer, IDesignerFormView
	{
		#region fields and constructors
		private Control _ui;
		public FormViewer(Control view)
		{
			_ui = view;
		}
		#endregion
		#region Methods
		public void RefreshViewer()
		{
			for (int i = 0; i < _ui.Controls.Count; i++)
			{
				_ui.Controls[i].Refresh();
			}
		}
		public void SetAllowRefresh(bool allow)
		{
		}
		#endregion
		#region IXDesignerViewer Members
		public void OnClosing()
		{
		}
		public void OnEventListChanged(ICustomEventMethodDescriptor owner, UInt32 objectId)
		{
		}
		public void OnResetMap()
		{
		}
		public void OnDatabaseListChanged()
		{
		}
		public void OnDatabaseConnectionNameChanged(Guid connectionid, string newName)
		{
		}
		public void OnObjectNameChanged(INonHostedObject obj)
		{
		}
		public void SetDesigner(MultiPanes mp)
		{
		}

		public Control GetDesignSurfaceUI()
		{
			return _ui;
		}
		public void OnDataLoaded()
		{

		}
		public void OnUIConfigLoaded()
		{
		}
		public EnumMaxButtonLocation MaxButtonLocation
		{
			get
			{
				return EnumMaxButtonLocation.Left;
			}
		}
		public void OnHtmlElementSelected(HtmlElement_Base element)
		{
		}
		public void OnSelectedHtmlElement(Guid guid, object selector)
		{
			if (selector != this)
			{
			}
		}
		//an html element is used. guid and id should have been created
		public void OnUseHtmlElement(HtmlElement_BodyBase element)
		{
		}
		public void OnActionSelected(IAction action)
		{
		}
		public void OnClassLoadedIntoDesigner(UInt32 classId)
		{
		}
		public void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade)
		{
		}
		public void OnComponentAdded(object obj)
		{
		}
		public void OnComponentSelected(object obj)
		{
		}
		public void OnComponentRename(object obj, string newName)
		{
		}
		public void OnComponentRemoved(object obj)
		{
		}
		public void OnAssignAction(EventAction ea)
		{
		}
		public void OnRemoveEventHandler(EventAction ea, TaskID task)
		{
		}
		public void OnRemoveAllEventHandlers(IEvent e)
		{
		}
		public void OnActionListOrderChanged(object sender, EventAction ea)
		{
		}
		public void OnActionChanged(UInt32 classId, IAction act, bool isNewAction)
		{
		}
		public void OnResetDesigner(object obj)
		{
		}
		public void OnInterfaceAdded(InterfacePointer interfacePointer)
		{
		}
		public void OnRemoveInterface(InterfacePointer interfacePointer)
		{
		}
		public void OnBaseInterfaceAdded(InterfaceClass owner, InterfacePointer baseInterface)
		{
		}
		public void OnInterfaceEventChanged(InterfaceElementEvent eventType)
		{
		}
		public void OnInterfaceEventAdded(InterfaceElementEvent eventType)
		{
		}
		public void OnInterfaceEventDeleted(InterfaceElementEvent eventType)
		{
		}
		public void OnInterfacePropertyDeleted(InterfaceElementProperty property)
		{
		}
		public void OnInterfacePropertyAdded(InterfaceElementProperty property)
		{
		}
		public void OnInterfacePropertyChanged(InterfaceElementProperty property)
		{
		}
		public void OnInterfaceMethodDeleted(InterfaceElementMethod method)
		{
		}
		public void OnInterfaceMethodChanged(InterfaceElementMethod method)
		{
		}
		public void OnInterfaceMethodCreated(InterfaceElementMethod method)
		{
		}
		public void OnMethodChanged(MethodClass method, bool isNewAction)
		{
		}
		public void OnDeleteMethod(MethodClass method)
		{
		}
		public void OnMethodSelected(MethodClass method)
		{
		}
		public void OnActionDeleted(IAction action)
		{
		}
		public void OnDeleteEventMethod(EventHandlerMethod method)
		{
		}
		public void OnDeleteProperty(PropertyClass property)
		{
		}
		public void OnAddProperty(PropertyClass property)
		{
		}
		public void OnPropertySelected(PropertyClass property)
		{
		}
		public void OnPropertyChanged(INonHostedObject property, string name)
		{
		}
		public void OnActionNameChanged(string newActionName, UInt64 WholeActionId)
		{
		}
		public void OnDeleteEvent(EventClass eventObject)
		{
		}
		public void OnEventSelected(EventClass eventObject)
		{
		}
		public void OnFireEventActionSelected(FireEventMethod method)
		{
		}
		public void OnAddEvent(EventClass eventObject)
		{
		}
		public void OnIconChanged(UInt32 classId)
		{
		}
		public void SetClassRefIcon(UInt32 classId, System.Drawing.Image img)
		{
		}
		public void OnAddExternalType(UInt32 classId, Type t)
		{
		}
		public void OnRemoveExternalType(UInt32 classId, Type t)
		{
		}
		#endregion

		#region IDesignerFormView Members

		#endregion
	}
}
