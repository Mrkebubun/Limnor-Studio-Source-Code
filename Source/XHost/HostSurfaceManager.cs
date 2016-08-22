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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.CodeDom;
using VOB;
using System.Xml;
using VPL;
using MathExp;
using VSPrj;
using LimnorDesigner;
using TraceLog;

namespace XHost
{
	/// <summary>
	/// Manages numerous HostSurfaces. Any services added to HostSurfaceManager
	/// will be accessible to all HostSurfaces
	/// </summary>
	public class HostSurfaceManager : DesignSurfaceManager
	{
		public HostSurfaceManager()
			: base()
		{
			this.AddService(typeof(INameCreationService), new NameCreationService());
			this.ActiveDesignSurfaceChanged += new ActiveDesignSurfaceChangedEventHandler(HostSurfaceManager_ActiveDesignSurfaceChanged);
		}

		protected override DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider)
		{
			return new HostSurface(parentProvider);
		}
		protected DesignSurface CreateDesignSurfaceCore()
		{
			return CreateDesignSurfaceCore(this.ServiceContainer);
		}
		public ILimnorDesignPane CreateDesigner(ClassData classData)
		{
			ILimnorDesignPane limnorXmlPane = null;
			try
			{
				DesignUtil.LogIdeProfile("Create designer loader");
				LimnorXmlDesignerLoader2 designerLoader = new LimnorXmlDesignerLoader2(classData);
				DesignUtil.LogIdeProfile("Create designer surface");
				HostSurface designSurface = (HostSurface)this.CreateDesignSurface(this.ServiceContainer);

				IDesignerHost dh = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));

				dh.AddService(typeof(INameCreationService), new NameCreationService());

				IServiceContainer serviceContainer = dh.GetService(typeof(ServiceContainer)) as IServiceContainer;
				DesignUtil.LogIdeProfile("Load designer surface");
				designSurface.Loader = designerLoader;
				designSurface.BeginLoad(designerLoader);
				if (VPLUtil.Shutingdown)
				{
					return null;
				}
				DesignUtil.LogIdeProfile("Add designer services");
				XMenuCommandService menuServices = new XMenuCommandService(serviceContainer);
				dh.AddService(typeof(IMenuCommandService), menuServices);
				menuServices.AddVerb(new DesignerVerb("Cut", null, StandardCommands.Cut));
				menuServices.AddVerb(new DesignerVerb("Copy", null, StandardCommands.Copy));
				menuServices.AddVerb(new DesignerVerb("Paste", null, StandardCommands.Paste));
				menuServices.AddVerb(new DesignerVerb("Delete", null, StandardCommands.Delete));
				menuServices.AddVerb(new DesignerVerb("Undo", null, StandardCommands.Undo));
				menuServices.AddVerb(new DesignerVerb("Redo", null, StandardCommands.Redo));

				if (dh.GetService(typeof(IDesignerSerializationService)) == null)
				{
					dh.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(serviceContainer));
				}

				if (dh.GetService(typeof(ComponentSerializationService)) == null)
				{
					//DesignerSerializationService uses CodeDomComponentSerializationService
					CodeDomComponentSerializationService codeDomComponentSerializationService = new CodeDomComponentSerializationService(serviceContainer);
					dh.AddService(typeof(ComponentSerializationService), codeDomComponentSerializationService);
				}
				VOB.UndoEngineImpl undoEngine = new VOB.UndoEngineImpl(serviceContainer);
				undoEngine.Enabled = false;
				dh.AddService(typeof(UndoEngine), undoEngine);

				designerLoader.AddComponentChangeEventHandler();
				DesignUtil.LogIdeProfile("Create designer pane");
				limnorXmlPane = new LimnorXmlPane2(designSurface, designerLoader);
				if (designerLoader.IsSetup)
				{
				}
				else
				{
					DesignUtil.LogIdeProfile("Apply config");
					if (limnorXmlPane.Loader.ObjectMap != null)
					{
						limnorXmlPane.BeginApplyConfig();
					}
				}
				DesignUtil.LogIdeProfile("Initialize designer surface");
				designSurface.Initialize();
				ILimnorToolbox toolbox = (ILimnorToolbox)GetService(typeof(IToolboxService));
				toolbox.Host = dh;
				this.ActiveDesignSurface = designSurface;
				//
			}
			catch (Exception ex)
			{
				MathNode.Log(TraceLogClass.MainForm, ex);
				// Just rethrow for now
				throw;
			}
			DesignUtil.LogIdeProfile("Finish creating designer");
			return limnorXmlPane;
		}

		/// <summary>
		/// Gets a new HostSurface and loads it with the appropriate type of
		/// root component. 
		/// </summary>
		public HostControl2 GetNewHost(Type rootComponentType)
		{
			HostSurface2 hostSurface = new HostSurface2(this.ServiceContainer);
			hostSurface.BeginLoad(rootComponentType);

			hostSurface.Initialize();

			this.ActiveDesignSurface = hostSurface;
			return new HostControl2(hostSurface);
		}
		public HostControl GetNewHost(string filename)
		{
			return null;
		}
		public HostControl2 GetNewHost(Type rootComponentType, int load)
		{
			TestLoader tl = new TestLoader(rootComponentType);
			HostSurface2 hostSurface = new HostSurface2(this.ServiceContainer);
			hostSurface.BeginLoad(tl);

			this.ActiveDesignSurface = hostSurface;
			return new HostControl2(hostSurface);
		}
		public void AddService(Type type, object serviceInstance)
		{
			this.ServiceContainer.AddService(type, serviceInstance);
		}

		/// <summary>
		/// Uses the OutputWindow service and writes out to it.
		/// </summary>
		void HostSurfaceManager_ActiveDesignSurfaceChanged(object sender, ActiveDesignSurfaceChangedEventArgs e)
		{
		}
		public void ShowMessage(string msg, bool bMsgBox)
		{
			VOB.OutputWindow o = this.GetService(typeof(VOB.OutputWindow)) as VOB.OutputWindow;
			if (o != null)
			{
				o.AppendMessage(msg);
			}
			if (bMsgBox)
			{
				MessageBox.Show(msg);
			}
		}
		public void ShowException(Exception e)
		{
			VOB.OutputWindow o = this.GetService(typeof(VOB.OutputWindow)) as VOB.OutputWindow;
			if (o != null)
			{
				o.ShowException(e);
			}
		}
	}// class
}// namespace
