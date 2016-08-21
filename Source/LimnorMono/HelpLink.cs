/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using VOB;
using System.IO;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace LimnorVOB
{
	class HelpLink : Control, IDrawControl
	{
		private string _link;
		public HelpLink()
		{
			this.Anchor = AnchorStyles.None;
			this.ForeColor = Color.Blue;
			this.Font = new Font(this.Font.FontFamily, 12, FontStyle.Underline | FontStyle.Bold);
			this.Cursor = Cursors.Hand;
			this.BackColor = Color.LightBlue;
		}
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}
		public void OnDraw(PaintEventArgs pe)
		{
			if (this.Parent != null)
			{
				SizeF sf = pe.Graphics.MeasureString(this.Text, this.Font);
				Rectangle rc = new Rectangle();
				rc.Y = this.Top;
				rc.X = this.Left;
				rc.Width = (int)sf.Width;
				rc.Height = this.Height;
				this.Width = rc.Width;
				pe.Graphics.DrawString(this.Text, this.Font, Brushes.Blue, (float)rc.X, (float)rc.Y);
			}
		}
		public HelpLink(string link)
			: this()
		{
			_link = link;
		}
		public void SetLink(string link)
		{
			_link = link;
		}
		protected override void OnClick(EventArgs e)
		{
			if (!string.IsNullOrEmpty(_link))
			{
				string folder = null;
				string exe = null;
				UserControlUserGuide ug = this.Parent as UserControlUserGuide;
				if (ug != null)
				{
					folder = ug.HelpFolder;
					exe = ug.Exe;
				}

				Process p = new Process();
				if (string.IsNullOrEmpty(exe))
				{
					if (!string.IsNullOrEmpty(folder))
					{
						p.StartInfo.FileName = Path.Combine(folder, _link);
					}
					else
					{
						p.StartInfo.FileName = _link;
					}
				}
				else
				{
					p.StartInfo.FileName = exe;
					//p.StartInfo.WorkingDirectory = folder;
					p.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "\"{0}\" \"{1}\"", _link, folder);
				}
				try
				{
					p.Start();
				}
				catch (Exception err)
				{
					MessageBox.Show(this.FindForm(), err.Message, _link, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
