/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace XToolbox2
{
	class TabsPanel : Panel
	{
		public event EventHandler OnVScroll = null;
		VScrollBar vsb;
		public TabsPanel()
		{
			vsb = new VScrollBar();
			vsb.Visible = false;
			vsb.Minimum = 0;
			vsb.Maximum = 110;
			this.Controls.Add(vsb);
			this.HScroll = false;
			this.VScroll = false;
			this.AutoScroll = false;
			vsb.Scroll += new ScrollEventHandler(vsb_Scroll);
		}
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
			vsb.Height = this.ClientSize.Height;
		}
		int nTop = 0;
		public int ScrollTop
		{
			get
			{
				return nTop;
			}
		}
		int nContentHeight = 100;
		public void SetContentHeight(int h)
		{
			nContentHeight = h;
		}
		void vsb_Scroll(object sender, ScrollEventArgs e)
		{
			if (this.Controls.Count > 0)
			{
				int d = nContentHeight - this.ClientSize.Height;
				if (d > 0)
				{
					//0 : 0
					//100 : d
					double v = ((double)d) * ((double)(vsb.Value)) / ((double)100);
					nTop = (int)v;
				}
				else
				{
					nTop = 0;
				}
				if (OnVScroll != null)
				{
					OnVScroll(this, null);
				}
			}
		}
		public void Init()
		{
			this.HScroll = false;
			this.Invalidate();
		}
		public int ClientWidth
		{
			get
			{
				if (vsb.Visible)
				{
					if (this.ClientSize.Width > vsb.Width)
						return this.ClientSize.Width - vsb.Width;
					else
						return 1;
				}
				else
					return this.ClientSize.Width;
			}
		}
		public void ShowVScroll()
		{
			vsb.Top = 0;
			vsb.Left = this.ClientSize.Width - vsb.Width;
			vsb.Height = this.ClientSize.Height;
			vsb.Visible = true;
		}
		public void HideVScroll()
		{
			vsb.Visible = false;
		}
		public bool VScrollVisible
		{
			get
			{
				return vsb.Visible;
			}
		}
	}
}
