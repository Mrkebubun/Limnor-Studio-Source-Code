/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using VSPrj;
using System.Xml;
using XmlSerializer;
using System.Drawing;
using System.CodeDom;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LimnorWeb;
using VPL;
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;
using System.Xml.Serialization;

namespace LimnorDesigner
{

	[ProjectMainComponent(EnumProjectType.WebAppAspx)]
	[ToolboxBitmap(typeof(LimnorWebApp), "Resources.earth.bmp")]
	[Serializable]
	public class LimnorWebApp : LimnorApp, IDevClassReferencer, IResetOnComponentChange
	{
		#region fields and constructors
		private string _webName;
		public LimnorWebApp()
		{
			GlobalVariableTimeout = 20;
			AjaxTimeout = 0;
			SessionDataStorage = EnumSessionDataStorage.Cookies;
			_sessonVariables = new SessionVariableCollection(this);
		}
		#endregion
		#region methods
		[Browsable(false)]
		protected override void OnExportCode(bool forDebug)
		{
		}
		[Browsable(false)]
		public override void Run()
		{
		}
		#endregion
		#region Properties
		[DefaultValue(EnumSessionDataStorage.Cookies)]
		[Description("Gets and sets a value indicating how to save session data. Currently it supports Cookies and HTML5 Storage. Default is cookies.")]
		public EnumSessionDataStorage SessionDataStorage
		{
			get;
			set;
		}
		[Editor(typeof(TypeSelectorWebSite), typeof(UITypeEditor))]
		[Description("Local web site for testing this web application")]
		public string WebSiteName
		{
			get
			{
				return _webName;
			}
			set
			{
				_webName = value;
			}
		}
		[Description("Local web site for testing this web application")]
		public string TestWebSiteName(Form owner)
		{
				if (this.Project != null)
				{
					VirtualWebDir web = this.Project.GetTestWebSite(owner);
					if (web != null)
					{
						return web.WebName;
					}
				}
				//_webName is the default name
				return _webName;
		}
		[Description("Gets a web server programming technology.")]
		public virtual EnumWebServerProcessor WebServerProcessor
		{
			get { return EnumWebServerProcessor.Aspx; }
		}
		[Description("Gets and sets a Boolean indicating whether the web pages will run in debug mode. In debug mode, server processing information is displayed. Do not forget to turn debug mode off before doing a release build of your web application.")]
		public bool DebugMode
		{
			get;
			set;
		}
		[Description("Gets and sets an integer indicating client side debug level. 0 indicates no debugging. A value greater than 0 will enable showing debugging information in a popup window.")]
		public int ClientDebugLevel
		{
			get;
			set;
		}
		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating maximum web server execution time allowed, in seconds. If a web server operation takes longer than the maximum allowed time then the operation is canceled and an onAjaxTimeout event of a web page occurs. 0 indicates that there is not a time limitation.")]
		public int AjaxTimeout
		{
			get;
			set;
		}
		[WebClientMember]
		[DefaultValue(20)]
		[Description("Gets and sets an integer indicating how many minutes of page inactivity the global variables will expire.")]
		public int GlobalVariableTimeout
		{
			get { return _sessionTimeout; }
			set
			{
				if (value > 0)
					_sessionTimeout = value;
			}
		}
		private ComponentID _startPage;
		[XmlIgnore]
		[Editor(typeof(TypeSelectorWebPage), typeof(UITypeEditor))]
		[Description("Gets and sets the first web page for testing")]
		public ComponentID StartPage
		{
			get
			{
				if (_startPage == null)
				{
					if (_startId != 0 && Project != null)
					{
						_startPage = Project.GetComponentByID(_startId);
					}
				}
				return _startPage;
			}
			set
			{
				if (value != null)
				{
					if (value.ComponentId != 0)
					{
						_startPage = value;
						_startId = _startPage.ComponentId;
					}
				}
			}
		}
		private UInt32 _startId;
		[Browsable(false)]
		public UInt32 StartPageId
		{
			get { return _startId; }
			set
			{
				_startId = value;
				if (_startId != 0 && Project != null)
				{
					_startPage = Project.GetComponentByID(_startId);
				}
			}
		}
		private int _sessionTimeout = 20;
		private SessionVariableCollection _sessonVariables;
		[WebClientMember]
		[WebServerMember]
		[RefreshProperties(RefreshProperties.All)]
		[EditorAttribute(typeof(TypeEditorSessionVariables), typeof(UITypeEditor))]
		[Description("A global variable can be accessed from any web pages at both web client and web server. A global variable takes more system resources than a property. Use properties instead of global variables if cross-page access is not needed.")]
		public SessionVariableCollection GlobalVariables
		{
			get
			{
				if (_sessonVariables == null)
				{
					_sessonVariables = new SessionVariableCollection(this);
				}
				return _sessonVariables;
			}
		}
		[Browsable(false)]
		public SessionVariable[] SessionVariables
		{
			get
			{
				SessionVariable[] ret = new SessionVariable[GlobalVariables.Count];
				_sessonVariables.CopyTo(ret, 0);
				return ret;
			}
			set
			{
				_sessonVariables = new SessionVariableCollection(this);
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						if (!string.IsNullOrEmpty(value[i].Name))
						{
							_sessonVariables.Add(value[i]);
						}
					}
				}
			}
		}
		#endregion
		#region Methods
		public static VirtualWebDir CreateWebSite(LimnorProject project, string websitename, Form owner)
		{
			bool iisError = false;
			VirtualWebDir webSite = project.GetTestWebSite(owner);
			if (webSite == null)
			{
				webSite = IisUtility.FindLocalWebSiteByName(owner, websitename, out iisError);
			}
			if (webSite == null && !iisError)
			{
				webSite = IisUtility.FindLocalWebSite(websitename);
				if (webSite == null)
				{
					//create the web site
					DialogProjectOutput dlg = new DialogProjectOutput();
					dlg.LoadData(project, websitename);
					if (dlg.ShowDialog(owner) == DialogResult.OK)
					{
						if (dlg.WebSite != null)
						{
							webSite = dlg.WebSite;
						}
					}
				}
			}
			return webSite;
		}
		public VirtualWebDir CreateWebSite(Form owner,out bool newName)
		{
			newName = false;
			string wname = WebSiteName;
			if (wname != null)
			{
				wname = wname.Trim();
			}
			if (string.IsNullOrEmpty(wname))
			{
				throw new DesignerException("WebSiteName property is empty");
			}
			VirtualWebDir webSite = CreateWebSite(Project, wname, owner);
			if (webSite != null)
			{
				if (string.Compare(wname, webSite.WebName, StringComparison.OrdinalIgnoreCase) != 0)
				{
					newName = true;
				}
			}
			return webSite;
		}
		#endregion
		#region IDevClassReferencer Members
		private IDevClass _root;
		public void SetDevClass(IDevClass c)
		{
			_root = c;
		}

		public IDevClass GetDevClass()
		{
			return _root;
		}

		#endregion

		#region IResetOnComponentChange Members

		public bool OnResetDesigner(string memberName)
		{
			if (string.CompareOrdinal(memberName, "GlobalVariables") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(memberName, "New variable") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(memberName, "New value") == 0)
			{
				return true;
			}
			return false;
		}

		#endregion
	}
	[ProjectMainComponent(EnumProjectType.WebAppPhp)]
	[ToolboxBitmap(typeof(LimnorWebAppPhp), "Resources.earth.bmp")]
	public class LimnorWebAppPhp : LimnorWebApp
	{
		public LimnorWebAppPhp()
		{
		}
		[Description("Gets a web server programming technology.")]
		public override EnumWebServerProcessor WebServerProcessor
		{
			get { return EnumWebServerProcessor.PHP; }
		}
	}
}
