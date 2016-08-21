/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Video/Audio Capture component
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

namespace Limnor.DirectXCapturer
{
	public partial class DialogProperties : Form
	{
		public DialogProperties()
		{
			InitializeComponent();
		}
		public void LoadData(Capturer obj)
		{
			propertyGrid1.SelectedObject = obj;
		}
	}
}
