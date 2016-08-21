/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Limnor.WebBuilder;
using WindowsUtility;

namespace LimnorDesigner.Web
{
	/// <summary>
	/// capture a web page to an image
	/// </summary>
	public partial class FormWeb : Form
	{
		private bool _loaded;
		private WebPage _page;
		private ScriptManager0 _sm;
		private bool _started = false;
		public FormWeb()
		{
			InitializeComponent();
		}
		public void ShowWeb(string url, WebPage page)
		{
			_loaded = false;
			_page = page;
			webBrowser1.AllowWebBrowserDrop = false;
			webBrowser1.IsWebBrowserContextMenuEnabled = false;
			webBrowser1.WebBrowserShortcutsEnabled = false;
			_sm = new ScriptManager0(this);
			webBrowser1.ObjectForScripting = _sm;
			webBrowser1.Url = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?rand={1}", url, Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture)));
		}
		public Bitmap ExportImage()
		{
			if (this.Handle != IntPtr.Zero && webBrowser1.Handle != IntPtr.Zero)
			{
				return WinUtil.CaptureWindowImage(webBrowser1.Handle);
			}
			return null;
		}
		public bool WebPageLoaded
		{
			get
			{
				return _loaded;
			}
		}
		public void ExecuteJs(string js)
		{
			string func = string.Format(CultureInfo.InvariantCulture, "func{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			StringBuilder sb = new StringBuilder("javascript:function ");
			sb.Append(func);
			sb.Append("(){");
			sb.Append(js);
			sb.Append("}");
			sb.Append(func);
			sb.Append("();");
			js = sb.ToString();
			webBrowser1.Navigate(js);
		}
		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			timer1.Enabled = true;
		}

		private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			_loaded = true;
		}
		/// <summary>
		/// some pages may not include pageStarter.js
		/// </summary>
		public void onPageStarted()
		{
			_started = true;
			this.Refresh();
			timer1.Enabled = true;
		}
		private void takesnapshot()
		{
			if (_page != null)
			{
				Bitmap bmp = ExportImage();
				if (bmp != null)
				{
					Bitmap bmp2 = WinUtil.SetImageOpacity(bmp, (float)0.5);
					if (bmp2 != null)
					{
						_page.BackgroundImage = bmp2;
					}
					else
					{
						_page.BackgroundImage = bmp;
					}
				}
			}
			Close();
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			if (!_started)
			{
				object v = webBrowser1.Document.InvokeScript("limnorPageLoaderPresent");
				if (v != null)
				{
					if (Convert.ToBoolean(v))
					{
						//wait for onPageStarted event
						return;
					}
				}
			}
			takesnapshot();
		}
		private void timer2_Tick(object sender, EventArgs e)
		{
			timer2.Enabled = false;
			timer1.Enabled = true;
		}
	}
}
