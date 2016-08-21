/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for frmDraw.
	/// </summary>
	public class frmHotspots : Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		protected Hotspots hotspots = null;
		protected bool bDesign = false;
		public frmHotspots()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// frmHotspots
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(256, 208);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmHotspots";
			this.Opacity = 0.02;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "";
			this.TransparencyKey = System.Drawing.Color.White;
		}
		#endregion

		public void SetCoverOwner(Hotspots c)
		{
			hotspots = c;
		}
		private void f_OnPageWindowStateChange(Form frmOwner, bool ChildVisible)
		{
			if (!this.IsDisposed)
			{
				this.Visible = ChildVisible;
			}
		}
		private void clsEPDesignTools_OnNotifyAppMinimize(object sender, EventArgs e)
		{
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (hotspots == null)
				return;
			DrawingLayer lst = hotspots.Drawings;//.GetDrawings();
			System.Drawing.Color color = hotspots.SpotColor;
			GraphicsPath myPath = new GraphicsPath();
			myPath.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
			foreach (DrawingItem objDraw in lst)
			{
				if (objDraw != null)
				{
					objDraw.AddToPath(myPath);
				}
			}
			e.Graphics.DrawPath(new Pen(color, 1), myPath);
			System.Drawing.SolidBrush br = new SolidBrush(color);
			e.Graphics.FillPath(br, myPath);

		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (bDesign)
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					System.Windows.Forms.ContextMenu menu = new ContextMenu();
					MenuItem mnuItem = new MenuItem("Finish");
					mnuItem.Click += new EventHandler(mnuItemFinish);
					menu.MenuItems.Add(mnuItem);
					menu.Show(this, new System.Drawing.Point(e.X, e.Y));
				}
			}
			else
			{
				base.OnMouseDown(e);
			}
		}

		private void mnuItemFinish(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
	}
}
