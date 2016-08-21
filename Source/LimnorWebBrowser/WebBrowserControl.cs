/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Wrapper of Web Browser Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.IO;

namespace LimnorWebBrowser
{
	[Description("This control derives from .Net WebBrowser. It allows blocking of pop-up windows")]
	[ToolboxBitmapAttribute(typeof(WebBrowserControl), "Resources.webb.bmp")]
	public class WebBrowserControl : WebBrowser
	{
		#region fields and constructors
		private IWebBrowser2 _webBrowser2;
		private System.Windows.Forms.AxHost.ConnectionPointCookie _cookie;
		private DWebBrowserEvents2Class _events;
		private EnumPopupLevel _popupLevel = EnumPopupLevel.AllowAllPopups;
		private string _htmlText;
		private bool _kioskMode;
		public WebBrowserControl()
		{
		}
		#endregion

		#region Events
		[Description("Occurs when downloading of a document starts")]
		public event EventHandler Downloading;
		[Description("Occurs when downloading of a document is completed")]
		public event EventHandler DownloadComplete;
		[Description("Occurs before navigation occurs")]
		public event EventHandler<BrowserNavigationEventArgs> StartNavigate;
		[Description("Occurs when a new window is to be created")]
		public event EventHandler<BrowserNavigationEventArgs> StartNewWindow;
		[Description("Occurs when the browser application quits")]
		public event EventHandler ApplicationQuit;
		//
		[Description("Occurs before downloading of a file starts")]
		public event FileDownloadEventHandler FileDownloadEx;
		#endregion

		#region Properties
		[Description("If this property is set to True then it will set IsWebBrowserContextMenuEnabled to False, set WebBrowserShortcutsEnabled to False, disable file download")]
		public bool KioskMode
		{
			get { return _kioskMode; }
			set
			{
				_kioskMode = value;
				if (_kioskMode)
				{
					this.IsWebBrowserContextMenuEnabled = false;
					this.WebBrowserShortcutsEnabled = false;
				}
			}
		}

		[DefaultValue(0)]
		[Description("Block of pop-up windows. AllowAllPopups: do not block pop-ups; AllowSecureSites: do not block pop-ups from secure web sites; BlockMost: block most pop-ups; BlockAll: block all pop-ups")]
		public EnumPopupLevel PopupLevel
		{
			get
			{
				return _popupLevel;
			}
			set
			{
				_popupLevel = value;
			}
		}
		/// <summary>
		/// Get/Set the contents of the document Body, in html.
		/// It is readonly to disable editing in propertygrid
		/// </summary>
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(TypeEditorText), typeof(UITypeEditor))]
		[Description("Get and set the contents of the document Body, in html.")]
		public string HtmlText
		{
			get
			{
				return _htmlText;
			}
			set
			{
				HtmlText0 = value;
			}
		}
		/// <summary>
		/// for serialization
		/// </summary>
		[Browsable(false)]
		public string HtmlText0
		{
			get
			{
				return _htmlText;
			}
			set
			{
				_htmlText = value;
				this.DocumentText = _htmlText;
			}
		}
		#endregion

		#region Methods
		[Browsable(false)]
		public void OnFileDownload(bool load, ref bool cancel)
		{
			if (KioskMode)
			{
				cancel = true;
			}
			else
			{
				if (FileDownloadEx != null)
				{
					FileDownloadEventArgs e = new FileDownloadEventArgs(load, cancel);
					FileDownloadEx(this, e);
					cancel = e.Cancel;
				}
			}
		}
		[Description("Load an Html file into this control. filePath can be a local html file path, a web Url starting with http://, or a local file path starting with file:///")]
		public void LoadHtmlFile(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath))
			{
				if (filePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
				{
					this.Url = new Uri(filePath);
				}
				else
				{
					const string PROTOCOL_FILE = "file:///";
					string f;
					if (filePath.StartsWith(PROTOCOL_FILE, StringComparison.OrdinalIgnoreCase))
					{
						f = filePath.Substring(PROTOCOL_FILE.Length);
					}
					else
					{
						f = filePath;
					}
					if (File.Exists(f))
					{
						StreamReader sr = new StreamReader(f);
						string html = sr.ReadToEnd();
						sr.Close();
						this.DocumentText = html;
					}
				}
			}
		}
		#endregion

		#region Protected methods
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void AttachInterfaces(object nativeActiveXObject)
		{
			_webBrowser2 = (IWebBrowser2)nativeActiveXObject;
			base.AttachInterfaces(nativeActiveXObject);
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void DetachInterfaces()
		{
			_webBrowser2 = null;
			base.DetachInterfaces();
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void CreateSink()
		{
			// Make sure to call the base class or the normal events won't fire
			base.CreateSink();
			_events = new DWebBrowserEvents2Class(this);
			_cookie = new AxHost.ConnectionPointCookie(this.ActiveXInstance, _events, typeof(DWebBrowserEvents2));
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void DetachSink()
		{
			if (_cookie != null)
			{
				_cookie.Disconnect();
				_cookie = null;
			}
		}

		protected void OnDownloading(EventArgs e)
		{
			if (Downloading != null)
			{
				Downloading(this, e);
			}
		}

		protected virtual void OnDownloadComplete(EventArgs e)
		{
			if (DownloadComplete != null)
			{
				DownloadComplete(this, e);
			}
		}

		protected void OnStartNewWindow(BrowserNavigationEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			bool allowPopup = (e.NavigationContext == EnumUrlContext.None) || ((e.NavigationContext & EnumUrlContext.OverrideKey) == EnumUrlContext.OverrideKey);

			if (!allowPopup)
			{
				switch (_popupLevel)
				{
					case EnumPopupLevel.AllowAllPopups:
						allowPopup = true;
						break;
					case EnumPopupLevel.AllowSecureSites:
						if (EncryptionLevel != WebBrowserEncryptionLevel.Insecure)
						{
							allowPopup = true;
							break;
						}
						else
						{
							goto EnumPopupLevel_BlockMost;
						}
					case EnumPopupLevel.BlockMost:
					EnumPopupLevel_BlockMost:
						if ((e.NavigationContext & EnumUrlContext.UserFirstInited) == EnumUrlContext.UserFirstInited
							&&
							(e.NavigationContext & EnumUrlContext.UserInited) == EnumUrlContext.UserInited)
						{
							allowPopup = true;
						}
						break;
				}
			}
			if (allowPopup)
			{
				if (this.StartNewWindow != null)
				{
					this.StartNewWindow(this, e);
				}
				if (e.Cancel)
				{
					if (_webBrowser2 != null)
					{
						e.AutomationObject = _webBrowser2.Application;
					}
				}
			}
			else
			{
				e.Cancel = true;
			}
		}

		protected void OnStartNavigate(BrowserNavigationEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (this.StartNavigate != null)
			{
				this.StartNavigate(this, e);
			}
		}

		protected void OnQuit()
		{
			if (ApplicationQuit != null)
			{
				ApplicationQuit(this, EventArgs.Empty);
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			const int WM_PARENTNOTIFY = 0x210;
			const int WM_DESTROY = 0x2;
			if (m.Msg == WM_PARENTNOTIFY)
			{
				int wp = m.WParam.ToInt32();
				int X = wp & 0xFFFF;
				if (X == WM_DESTROY)
				{
					this.OnQuit();
				}
			}

			base.WndProc(ref m);
		}
		#endregion

		#region class DWebBrowserEvents2Class
		class DWebBrowserEvents2Class : DWebBrowserEvents2
		{
			WebBrowserControl _webBrowser;
			public DWebBrowserEvents2Class()
			{
			}


			public DWebBrowserEvents2Class(WebBrowserControl browser)
			{
				_webBrowser = browser;
			}

			#region DWebBrowserEvents2 Members

			//Implement whichever events you wish
			public void BeforeNavigate2(object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
			{
				Uri urlUri = new Uri(URL.ToString());

				string tFrame = null;
				if (targetFrameName != null)
				{
					tFrame = targetFrameName.ToString();
				}
				if (_webBrowser != null)
				{
					BrowserNavigationEventArgs args = new BrowserNavigationEventArgs(pDisp, urlUri, tFrame, EnumUrlContext.None);
					_webBrowser.OnStartNavigate(args);

					cancel = args.Cancel;
				}
			}
			//The NewWindow2 event, used on Windows XP SP1 and below
			public void NewWindow2(ref object pDisp, ref bool cancel)
			{
				BrowserNavigationEventArgs args = new BrowserNavigationEventArgs(pDisp, null, null, EnumUrlContext.None);
				_webBrowser.OnStartNewWindow(args);
				cancel = args.Cancel;
				pDisp = args.AutomationObject;
			}

			// NewWindow3 event, used on Windows XP SP2 and higher
			public void NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
			{
				BrowserNavigationEventArgs args = new BrowserNavigationEventArgs(ppDisp, new Uri(bstrUrl), null, (EnumUrlContext)dwFlags);
				_webBrowser.OnStartNewWindow(args);
				Cancel = args.Cancel;
				ppDisp = args.AutomationObject;
			}

			// Fired when downloading begins
			public void DownloadBegin()
			{
				_webBrowser.OnDownloading(EventArgs.Empty);
			}

			// Fired when downloading is completed
			public void DownloadComplete()
			{
				_webBrowser.OnDownloadComplete(EventArgs.Empty);
			}

			#region Unused events

			// This event doesn't fire. 
			[DispId(0x00000107)]
			public void WindowClosing(bool isChildWindow, ref bool cancel)
			{
			}

			public void OnQuit()
			{

			}

			public void StatusTextChange(string text)
			{
			}

			public void ProgressChange(int progress, int progressMax)
			{
			}

			public void TitleChange(string text)
			{
			}

			public void PropertyChange(string szProperty)
			{
			}

			public void NavigateComplete2(object pDisp, ref object URL)
			{
			}

			public void DocumentComplete(object pDisp, ref object URL)
			{
			}

			public void OnVisible(bool visible)
			{
			}

			public void OnToolBar(bool toolBar)
			{
			}

			public void OnMenuBar(bool menuBar)
			{
			}

			public void OnStatusBar(bool statusBar)
			{
			}

			public void OnFullScreen(bool fullScreen)
			{
			}

			public void OnTheaterMode(bool theaterMode)
			{
			}

			public void WindowSetResizable(bool resizable)
			{
			}

			public void WindowSetLeft(int left)
			{
			}

			public void WindowSetTop(int top)
			{
			}

			public void WindowSetWidth(int width)
			{
			}

			public void WindowSetHeight(int height)
			{
			}

			public void SetSecureLockIcon(int secureLockIcon)
			{
			}

			public void FileDownload(bool load, ref bool cancel)
			{
				_webBrowser.OnFileDownload(load, ref cancel);
			}

			public void NavigateError(object pDisp, ref object URL, ref object frame, ref object statusCode, ref bool cancel)
			{
			}

			public void PrintTemplateInstantiation(object pDisp)
			{
			}

			public void PrintTemplateTeardown(object pDisp)
			{
			}

			public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
			{
			}

			public void PrivacyImpactedStateChange(bool bImpacted)
			{
			}

			public void CommandStateChange(int Command, bool Enable)
			{
			}

			public void ClientToHostWindow(ref int CX, ref int CY)
			{
			}
			#endregion

			#endregion
		}
		#endregion
	}
}
