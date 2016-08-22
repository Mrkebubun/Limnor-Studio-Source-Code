/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace XHost
{
	/// <summary>
	/// host the control under design
	/// </summary>
	public partial class ControlDesignerHost : UserControl
	{
		public ControlDesignerHost()
		{
			InitializeComponent();
		}
		public void AddHostee(Control c)
		{
			if (Controls.Count == 0)
			{
				Controls.Add(c);
				c.Location = new Point(0, 0);
				c.Move += new EventHandler(c_Move);
			}
		}

		void c_Move(object sender, EventArgs e)
		{
			if (Controls.Count > 0)
			{
				Controls[0].Location = new Point(0, 0);
			}
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (Controls.Count > 0)
			{
				Controls[0].Location = new Point(0, 0);
				Controls[0].Size = new Size(this.Width, this.Height);
			}
		}
	}
}
