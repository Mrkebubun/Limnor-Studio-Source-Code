/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Wrapper of Web Browser Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LimnorWebBrowser
{
	public class BrowserNavigationEventArgs : CancelEventArgs
	{
		#region fields and constructors
		private Uri _url;
		private string _frame;
		private EnumUrlContext _navigationContext;
		private object _pDisp;
		/// <summary>
		/// Creates a new instance of WebBrowserExtendedNavigatingEventArgs
		/// </summary>
		/// <param name="automation">Pointer to the automation object of the browser</param>
		/// <param name="url">The URL to go to</param>
		/// <param name="frame">The name of the frame</param>
		/// <param name="navigationContext">The new window flags</param>
		public BrowserNavigationEventArgs(object automation, Uri url, string frame, EnumUrlContext navigationContext)
			: base()
		{
			_url = url;
			_frame = frame;
			_navigationContext = navigationContext;
			_pDisp = automation;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The URL to navigate to
		/// </summary>
		//[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public Uri Url
		{
			get { return _url; }
		}


		/// <summary>
		/// The name of the frame to navigate to
		/// </summary>
		public string Frame
		{
			get { return _frame; }
		}


		/// <summary>
		/// The flags when opening a new window
		/// </summary>
		public EnumUrlContext NavigationContext
		{
			get { return _navigationContext; }
		}


		/// <summary>
		/// The pointer to ppDisp
		/// </summary>
		public object AutomationObject
		{
			get { return this._pDisp; }
			set { this._pDisp = value; }
		}
		#endregion
	}
}
