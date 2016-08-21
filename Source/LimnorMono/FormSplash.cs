/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace LimnorVOB
{
	public partial class FormSplash : Form
	{
		static FormSplash _form;
		//static bool _started;
		public FormSplash()
		{
			InitializeComponent();
		}
		public static void ShowSplash()
		{
			if (_form == null)
			{
				_form = new FormSplash();
				_form.Show();
				_form.Refresh();
			}
		}
		public static void CloseSplash()
		{
			if (_form != null && !_form.IsDisposed && !_form.Disposing)
			{
				_form.Close();
				_form = null;
			}
			using (var closeSplashEvent = new EventWaitHandle(false,
				EventResetMode.ManualReset, "CloseSplashScreenEventLimnorStudio"))
			{
				closeSplashEvent.Set();
			}
		}
	}
}
