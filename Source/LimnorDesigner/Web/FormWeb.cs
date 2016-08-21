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
	/// for showing HTML design as an image after toggling from HTML designer to WebPage designer
	/// </summary>
	public partial class FormWeb : Form
	{
		private bool _loaded;
		private WebPage _page;
		public FormWeb()
		{
			InitializeComponent();
		}
		public void ShowWeb(string url, WebPage page)
		{
			_loaded = false;
			_page = page;
			if (string.IsNullOrEmpty(url))
			{
				Close();
			}
			else
			{
				webBrowser1.Url = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?rand={1}", url, Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture)));
			}
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
		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			webBrowser1.Refresh();
		}

		private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			_loaded = true;
			webBrowser1.Refresh();
			timer1.Enabled = true;
			//if (_page != null)
			//{
			//    Bitmap bmp = ExportImage();
			//    if (bmp != null)
			//    {
			//        _page.BackgroundImage = bmp;
			//    }
			//}
			//Close();
		}

		private void timer1_Tick(object sender, EventArgs e)
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
	}
}
