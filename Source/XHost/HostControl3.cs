/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using VOB;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Drawing2D;
using LimnorDesigner;
using LimnorDesigner.Web;
using System.Collections.Generic;

namespace XHost
{
	/// <summary>
	/// Hosts the HostSurface which inherits from DesignSurface.
	/// </summary>
	public class HostControl3 : UserControl, ILimnorDesignPane
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private HostSurface _hostSurface;
		private StringFormat _componentTextFormat;
		private Font _componentTextFont;
		private LimnorXmlDesignerLoader2 _loader;
		public HostControl3(HostSurface hostSurface)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			InitializeHost(hostSurface);
			//
			_componentTextFormat = new StringFormat();
			_componentTextFormat.Alignment = StringAlignment.Center;
			_componentTextFormat.LineAlignment = StringAlignment.Center;
			_componentTextFormat.Trimming = StringTrimming.None;
			_componentTextFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
			//
			_componentTextFont = new Font("Times New Roman", 12);
		}
		public HostControl3(HostSurface hostSurface, LimnorXmlDesignerLoader2 loader)
		{
			_loader = loader;
			_pane = new MultiPanes();
			_pane.Dock = DockStyle.Fill;
			FormViewer fv = new FormViewer(hostSurface.View as Control);
			_pane.AddViewer(fv);
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// HostControl
			// 
			this.Name = "HostControl";
			this.Size = new System.Drawing.Size(268, 224);
		}
		#endregion

		internal void InitializeHost(HostSurface hostSurface)
		{
			try
			{
				if (hostSurface == null)
					return;

				_hostSurface = hostSurface;

				Control control = _hostSurface.View as Control;

				control.Parent = this;
				control.Dock = DockStyle.Fill;
				control.Visible = true;
				control.MouseEnter += new EventHandler(HostControl_MouseEnter);
				//
				if (!(DesignerHost.RootComponent is Control) && !(DesignerHost.RootComponent is NewComponentPrompt))
				{
					if (control.Controls.Count > 0)
					{
						if (control.Controls[0] != null)
						{
							control.Controls[0].Text = "";
							control.Controls[0].Paint += new PaintEventHandler(HostControl_Paint);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		void HostControl_MouseEnter(object sender, EventArgs e)
		{
			VOB.InterfaceVOB iv = (VOB.InterfaceVOB)_hostSurface.GetService(typeof(VOB.InterfaceVOB));
			if (iv != null)
			{
				iv.SendNotice(enumVobNotice.HideToolbox, true);
			}
		}

		void HostControl_Paint(object sender, PaintEventArgs e)
		{
			Control c = sender as Control;
			RectangleF _rectF = new RectangleF((float)c.Left, (float)c.Top, (float)c.Width, (float)c.Height);
			e.Graphics.FillRectangle(new LinearGradientBrush(c.ClientRectangle, Color.White, Color.Gray, 45F), c.ClientRectangle);
			e.Graphics.DrawString("To add components to your object, drag them from the Toolbox and use the Properties window to set their properties. To create properties, methods and events for your object, in the Object Explorer right-click on 'Properties', 'Methods' and 'Events' nodes under 'Attributes' node of your object.",
				_componentTextFont, Brushes.Black, _rectF, _componentTextFormat);
		}
		public HostSurface HostSurface
		{
			get
			{
				return _hostSurface;
			}
		}
		public IComponent CreateComponent(Type type, string name)
		{
			return null;
		}
		public void SelectComponent(IComponent c)
		{
		}
		#region IXHostControl Members

		public DesignSurface DesignSurface
		{
			get { return _hostSurface; }
		}
		public IDesignerHost DesignerHost
		{
			get
			{
				return (IDesignerHost)_hostSurface.GetService(typeof(IDesignerHost));
			}
		}
		public bool SelectComponent(object v)
		{
			if (_hostSurface != null)
			{
				return _hostSurface.SelectComponent(v);
			}
			return false;
		}

		#endregion

		#region ILimnorDesignPane Members
		public bool CanBuild
		{
			get
			{
				return true;
			}
		}
		public IList<IXDesignerViewer> GetDettachedViewers()
		{
			return null;
		}
		public IWin32Window Window
		{
			get { return this; }
		}

		public ILimnorDesignerLoader Loader
		{
			get { return _loader; }
		}

		public ClassPointer RootClass
		{
			get { return _loader.GetRootId(); }
		}

		public System.Xml.XmlNode RootXmlNode
		{
			get { throw new NotImplementedException(); }
		}
		private MultiPanes _pane;
		public MultiPanes PaneHolder
		{
			get
			{
				return _pane;
			}
		}

		public bool IsClosing
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DesignSurface Surface2
		{
			get { return _hostSurface; }
		}
		public object CurrentObject
		{
			get
			{
				if (_hostSurface != null)
				{
					ISelectionService selectionService = (ISelectionService)Surface2.GetService(typeof(ISelectionService));
					if (selectionService != null)
					{
						return selectionService.PrimarySelection;
					}
				}
				return null;
			}
		}
		public void BeginApplyConfig() { }
		public void OnComponentRename(object obj, string newName)
		{
		}
		public void OnClosing()
		{
		}
		public void OnClosed()
		{
			throw new NotImplementedException();
		}
		public void RefreshWebServiceToolbox()
		{
		}
		public void OnIconChanged()
		{
			throw new NotImplementedException();
		}
		public void LoadProjectToolbox() { }
		public void AddDesigner(IXDesignerViewer designer, bool detached)
		{
			throw new NotImplementedException();
		}

		public void RemoveDesigner(IXDesignerViewer designer, bool detached)
		{
			throw new NotImplementedException();
		}

		public void RefreshPropertyGrid()
		{
			throw new NotImplementedException();
		}

		public void OnNotifyChanges()
		{
			throw new NotImplementedException();
		}
		public void OnHtmlElementSelected(HtmlElement_Base element)
		{
		}
		public void OnSelectedHtmlElement(Guid guid, object selector)
		{
		}
		//an html element is used. guid and id should have been created
		public void OnUseHtmlElement(HtmlElement_BodyBase element)
		{
		}
		public void OnAddExternalType(Type type)
		{
			throw new NotImplementedException();
		}
		public void OnActionSelected(IAction action)
		{
			throw new NotImplementedException();
		}

		public void OnAssignAction(EventAction ea)
		{
			throw new NotImplementedException();
		}
		public void OnActionDeleted(IAction act)
		{
		}
		public void OnRemoveEventHandler(EventAction ea, TaskID task)
		{
			throw new NotImplementedException();
		}

		public void OnActionListOrderChanged(object sender, EventAction ea)
		{
			throw new NotImplementedException();
		}

		public void OnActionChanged(uint classId, IAction act, bool isNewAction)
		{
			throw new NotImplementedException();
		}
		public void OnResetDesigner(object obj)
		{
		}
		public void OnMethodChanged(MethodClass method, bool isNewAction)
		{
			throw new NotImplementedException();
		}

		public void OnDeleteMethod(MethodClass method)
		{
			throw new NotImplementedException();
		}

		public void OnDeleteEventMethod(LimnorDesigner.Event.EventHandlerMethod method)
		{
			throw new NotImplementedException();
		}

		public void OnDeleteProperty(LimnorDesigner.Property.PropertyClass property)
		{
			throw new NotImplementedException();
		}

		public void OnAddProperty(LimnorDesigner.Property.PropertyClass property)
		{
			throw new NotImplementedException();
		}

		public void OnPropertyChanged(INonHostedObject property, string name)
		{
			throw new NotImplementedException();
		}

		public void OnDeleteEvent(LimnorDesigner.Event.EventClass eventObject)
		{
			throw new NotImplementedException();
		}

		public void OnAddEvent(LimnorDesigner.Event.EventClass eventObject)
		{
			throw new NotImplementedException();
		}

		public void SaveAll()
		{
			throw new NotImplementedException();
		}

		public void SetClassRefIcon(uint classId, Image img)
		{
			throw new NotImplementedException();
		}

		public void ResetContextMenu()
		{
			throw new NotImplementedException();
		}

		public void OnDatabaseListChanged()
		{
			throw new NotImplementedException();
		}

		public void OnDatabaseConnectionNameChanged(Guid connectionid, string newName)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceAdded(LimnorDesigner.Interface.InterfacePointer interfacePointer)
		{
			throw new NotImplementedException();
		}

		public void OnRemoveInterface(LimnorDesigner.Interface.InterfacePointer interfacePointer)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceMethodDeleted(LimnorDesigner.Interface.InterfaceElementMethod method)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceMethodChanged(LimnorDesigner.Interface.InterfaceElementMethod method)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceEventDeleted(LimnorDesigner.Interface.InterfaceElementEvent eventType)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceEventAdded(LimnorDesigner.Interface.InterfaceElementEvent eventType)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceEventChanged(LimnorDesigner.Interface.InterfaceElementEvent eventType)
		{
			throw new NotImplementedException();
		}

		public void OnInterfacePropertyDeleted(LimnorDesigner.Interface.InterfaceElementProperty property)
		{
			throw new NotImplementedException();
		}

		public void OnInterfacePropertyAdded(LimnorDesigner.Interface.InterfaceElementProperty property)
		{
			throw new NotImplementedException();
		}

		public void OnInterfacePropertyChanged(LimnorDesigner.Interface.InterfaceElementProperty property)
		{
			throw new NotImplementedException();
		}

		public void OnInterfaceMethodCreated(LimnorDesigner.Interface.InterfaceElementMethod method)
		{
			throw new NotImplementedException();
		}

		public void OnBaseInterfaceAdded(LimnorDesigner.Interface.InterfaceClass owner, LimnorDesigner.Interface.InterfacePointer baseInterface)
		{
			throw new NotImplementedException();
		}

		public void OnCultureChanged()
		{
			throw new NotImplementedException();
		}

		public void OnResourcesEdited()
		{
			throw new NotImplementedException();
		}

		public void OnDefinitionChanged(uint classId, object relatedObject, EnumClassChangeType changeMade)
		{
			throw new NotImplementedException();
		}

		public void OnClassLoadedIntoDesigner(uint classId)
		{
			throw new NotImplementedException();
		}

		public void AddToolboxItem()
		{
			throw new NotImplementedException();
		}
		public void InitToolbox() { }
		public void HideToolbox() { }
		#endregion
	} // class
}// namespace
