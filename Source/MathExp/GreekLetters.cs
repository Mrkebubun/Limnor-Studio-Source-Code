/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
	public partial class GreekLetters : UserControl
	{
		private bool _loadLetters = false;
		public event EventHandler OnLetterSelected;
		public GreekLetters()
		{
			InitializeComponent();
		}
		class letterLabel : Label
		{
			public letterLabel()
			{
			}
			protected override void OnMouseEnter(EventArgs e)
			{
				base.OnMouseEnter(e);
				for (int i = 0; i < this.Parent.Controls.Count; i++)
				{
					letterLabel l = this.Parent.Controls[i] as letterLabel;
					if (l != null)
					{
						l.BackColor = Color.White;
						l.BorderStyle = BorderStyle.None;
					}
				}
				this.BorderStyle = BorderStyle.Fixed3D;
				this.BackColor = Color.Yellow;
			}
			protected override void OnMouseLeave(EventArgs e)
			{
				base.OnMouseLeave(e);
				this.BackColor = Color.White;
				this.BorderStyle = BorderStyle.None;
			}
		}
		public void LoadData()
		{
			if (!_loadLetters)
			{
				_loadLetters = true;
				Graphics g = this.CreateGraphics();
				g.DrawString("Loading Greek letters, please wait ,,,", this.Font, Brushes.Red, (float)10, (float)10);
				g.Dispose();
				int nStart = 0x0391;
				int nEnd = 0x03ce;
				letterLabel[] lbls = new letterLabel[nEnd - nStart + 1];
				for (int i = 0, k = nStart; k <= nEnd; i++, k++)
				{
					lbls[i] = new letterLabel();
					lbls[i].Text = new string((char)k, 1);
					lbls[i].Size = new Size(16, 16);
					lbls[i].Click += new EventHandler(GreekLetters_Click);
				}
				this.Controls.AddRange(lbls);
				OnResize(null);
			}
		}
		protected override void OnResize(EventArgs e)
		{
			if (Controls.Count > 0)
			{
				int cols = this.ClientSize.Width / Controls[0].Width;
				int r = 0;
				int x = 0, y = 0;
				while (r < Controls.Count)
				{
					for (int i = 0; i < cols && r < Controls.Count; i++, r++)
					{
						Controls[r].Location = new Point(x, y);
						x += Controls[0].Width;
					}
					y += Controls[0].Height;
					x = 0;
				}
			}
		}
		void GreekLetters_Click(object sender, EventArgs e)
		{
			Control c = sender as Control;
			if (c != null)
			{
				if (OnLetterSelected != null)
				{
					OnLetterSelected(c.Text, null);
				}
			}
		}
	}
}
