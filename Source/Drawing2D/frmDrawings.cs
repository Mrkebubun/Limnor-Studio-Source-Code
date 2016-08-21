/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Show a list of all drawings
	/// </summary>
	public class frmDrawings : System.Windows.Forms.Form//, IAutoSettings
	{
		private System.Windows.Forms.ListBox lstDrawings;
		private System.Windows.Forms.ListBox lstParameters;
		private System.Windows.Forms.TextBox txtDesc;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton btNew;
		private System.Windows.Forms.ToolBarButton btDel;
		private System.Windows.Forms.PictureBox picSep1;
		private System.Windows.Forms.PictureBox picSep2;
		private System.Windows.Forms.ToolBarButton btEdit;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton btOK;
		private System.Windows.Forms.ToolBarButton btCancel;
		private System.Windows.Forms.ToolBarButton btUp;
		private System.Windows.Forms.ToolBarButton btDown;
		//
		public DrawingLayer drawings = null;
		private System.Windows.Forms.ToolBarButton sep;
		private System.Windows.Forms.ToolBarButton btShowBK;
		internal dlgDrawings frmParent = null;
		public bool HotspotOnly = false;
		private System.Windows.Forms.Label lblSep;
		private PropertyGrid lstProperty;
		private System.Windows.Forms.Label lblSepH;
		int x0 = 0;
		int y0 = 0;
		public frmDrawings()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDrawings));
			this.lstDrawings = new System.Windows.Forms.ListBox();
			this.lstParameters = new System.Windows.Forms.ListBox();
			this.txtDesc = new System.Windows.Forms.TextBox();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.btNew = new System.Windows.Forms.ToolBarButton();
			this.btEdit = new System.Windows.Forms.ToolBarButton();
			this.btDel = new System.Windows.Forms.ToolBarButton();
			this.btUp = new System.Windows.Forms.ToolBarButton();
			this.btDown = new System.Windows.Forms.ToolBarButton();
			this.btOK = new System.Windows.Forms.ToolBarButton();
			this.btCancel = new System.Windows.Forms.ToolBarButton();
			this.sep = new System.Windows.Forms.ToolBarButton();
			this.btShowBK = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.picSep1 = new System.Windows.Forms.PictureBox();
			this.picSep2 = new System.Windows.Forms.PictureBox();
			this.lblSep = new System.Windows.Forms.Label();
			this.lstProperty = new System.Windows.Forms.PropertyGrid();
			this.lblSepH = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picSep1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picSep2)).BeginInit();
			this.SuspendLayout();
			// 
			// lstDrawings
			// 
			this.lstDrawings.IntegralHeight = false;
			this.lstDrawings.Location = new System.Drawing.Point(2, 48);
			this.lstDrawings.Name = "lstDrawings";
			this.lstDrawings.Size = new System.Drawing.Size(216, 108);
			this.lstDrawings.TabIndex = 0;
			this.lstDrawings.SelectedIndexChanged += new System.EventHandler(this.lstDrawings_SelectedIndexChanged);
			// 
			// lstParameters
			// 
			this.lstParameters.Location = new System.Drawing.Point(0, 160);
			this.lstParameters.Name = "lstParameters";
			this.lstParameters.Size = new System.Drawing.Size(216, 69);
			this.lstParameters.TabIndex = 1;
			this.lstParameters.Visible = false;
			// 
			// txtDesc
			// 
			this.txtDesc.Location = new System.Drawing.Point(232, 112);
			this.txtDesc.Multiline = true;
			this.txtDesc.Name = "txtDesc";
			this.txtDesc.ReadOnly = true;
			this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDesc.Size = new System.Drawing.Size(128, 32);
			this.txtDesc.TabIndex = 2;
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.btNew,
            this.btEdit,
            this.btDel,
            this.btUp,
            this.btDown,
            this.btOK,
            this.btCancel,
            this.sep,
            this.btShowBK});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(368, 42);
			this.toolBar1.TabIndex = 3;
			this.toolBar1.Visible = false;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// btNew
			// 
			this.btNew.ImageIndex = 0;
			this.btNew.Name = "btNew";
			this.btNew.Text = "New";
			// 
			// btEdit
			// 
			this.btEdit.ImageIndex = 1;
			this.btEdit.Name = "btEdit";
			this.btEdit.Text = "Edit";
			// 
			// btDel
			// 
			this.btDel.ImageIndex = 2;
			this.btDel.Name = "btDel";
			this.btDel.Text = "Delete";
			// 
			// btUp
			// 
			this.btUp.ImageIndex = 3;
			this.btUp.Name = "btUp";
			this.btUp.Text = "Up";
			// 
			// btDown
			// 
			this.btDown.ImageIndex = 4;
			this.btDown.Name = "btDown";
			this.btDown.Text = "Down";
			// 
			// btOK
			// 
			this.btOK.ImageIndex = 5;
			this.btOK.Name = "btOK";
			this.btOK.Text = "Finish";
			// 
			// btCancel
			// 
			this.btCancel.ImageIndex = 6;
			this.btCancel.Name = "btCancel";
			this.btCancel.Text = "Cancel";
			// 
			// sep
			// 
			this.sep.Name = "sep";
			this.sep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// btShowBK
			// 
			this.btShowBK.ImageIndex = 8;
			this.btShowBK.Name = "btShowBK";
			this.btShowBK.Pushed = true;
			this.btShowBK.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.btShowBK.Text = "Background";
			this.btShowBK.ToolTipText = "Show/hide background image";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.imageList1.Images.SetKeyName(5, "_ok.ico");
			this.imageList1.Images.SetKeyName(6, "");
			this.imageList1.Images.SetKeyName(7, "");
			this.imageList1.Images.SetKeyName(8, "");
			// 
			// picSep1
			// 
			this.picSep1.BackColor = System.Drawing.SystemColors.GrayText;
			this.picSep1.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.picSep1.Location = new System.Drawing.Point(0, 160);
			this.picSep1.Name = "picSep1";
			this.picSep1.Size = new System.Drawing.Size(216, 4);
			this.picSep1.TabIndex = 4;
			this.picSep1.TabStop = false;
			this.picSep1.Visible = false;
			// 
			// picSep2
			// 
			this.picSep2.BackColor = System.Drawing.SystemColors.GrayText;
			this.picSep2.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.picSep2.Location = new System.Drawing.Point(0, 240);
			this.picSep2.Name = "picSep2";
			this.picSep2.Size = new System.Drawing.Size(216, 4);
			this.picSep2.TabIndex = 5;
			this.picSep2.TabStop = false;
			this.picSep2.Visible = false;
			// 
			// lblSep
			// 
			this.lblSep.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.lblSep.Cursor = System.Windows.Forms.Cursors.SizeWE;
			this.lblSep.Location = new System.Drawing.Point(224, 0);
			this.lblSep.Name = "lblSep";
			this.lblSep.Size = new System.Drawing.Size(4, 144);
			this.lblSep.TabIndex = 6;
			this.lblSep.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblSep_MouseMove);
			this.lblSep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblSep_MouseDown);
			// 
			// lstProperty
			// 
			this.lstProperty.Location = new System.Drawing.Point(232, 0);
			this.lstProperty.Name = "lstProperty";
			this.lstProperty.Size = new System.Drawing.Size(130, 130);
			this.lstProperty.TabIndex = 7;
			// 
			// lblSepH
			// 
			this.lblSepH.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.lblSepH.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.lblSepH.Location = new System.Drawing.Point(232, 104);
			this.lblSepH.Name = "lblSepH";
			this.lblSepH.Size = new System.Drawing.Size(100, 4);
			this.lblSepH.TabIndex = 8;
			this.lblSepH.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblSepH_MouseMove);
			this.lblSepH.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblSepH_MouseDown);
			// 
			// frmDrawings
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(368, 150);
			this.ControlBox = false;
			this.Controls.Add(this.lblSepH);
			this.Controls.Add(this.lstProperty);
			this.Controls.Add(this.lblSep);
			this.Controls.Add(this.picSep2);
			this.Controls.Add(this.picSep1);
			this.Controls.Add(this.toolBar1);
			this.Controls.Add(this.txtDesc);
			this.Controls.Add(this.lstParameters);
			this.Controls.Add(this.lstDrawings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmDrawings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Drawings";
			this.Resize += new System.EventHandler(this.frmDrawings_Resize);
			((System.ComponentModel.ISupportInitialize)(this.picSep1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picSep2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public void LoadData(DrawingLayer lst)
		{
			drawings = lst;
			ReloadData();
			ShowBKbutton();
		}
		public void ReloadData()
		{
			lstDrawings.Items.Clear();
			foreach (DrawingItem v in drawings)
			{
				lstDrawings.Items.Add(v);
			}
		}
		public void ShowBKbutton()
		{
			bool visible = false;
			if (frmParent != null)
			{
				if (frmParent.BKImage != null)
				{
					visible = true;
					frmParent.BackgroundImage = frmParent.BKImage;
					toolBar1.Buttons[8].Pushed = true;
				}
			}
			toolBar1.Buttons[8].Visible = visible;
		}
		public void AddItem(DrawingItem v)
		{
			lstDrawings.Items.Add(v);
		}
		public void RefreshItems()
		{
			lstDrawings.Items.Clear();
			foreach (DrawingItem v in drawings)
			{
				lstDrawings.Items.Add(v);
			}
		}
		public void ClearSelection()
		{
			lstProperty.SelectedObject = null;
		}
		private void frmDrawings_Resize(object sender, System.EventArgs e)
		{
			lstDrawings.Left = 0;
			lstDrawings.Top = 0;
			lstDrawings.Height = this.ClientSize.Height;
			lblSep.Top = 0;
			lblSep.Height = this.ClientSize.Height;
			lstProperty.Top = 0;
			lstProperty.Left = lblSep.Left + lblSep.Width;
			lblSepH.Left = lstProperty.Left;
			lstProperty.Height = lblSepH.Top;
			//
			lstDrawings.Width = lblSep.Left;

			if (this.ClientSize.Width > lstProperty.Left)
			{
				lstProperty.Width = this.ClientSize.Width - lstProperty.Left;
				lblSepH.Width = lstProperty.Width;
				txtDesc.Width = lstProperty.Width;
			}
			txtDesc.Left = lstProperty.Left;
			txtDesc.Top = lblSepH.Top + lblSepH.Height;
			if (this.ClientSize.Height > txtDesc.Top)
			{
				txtDesc.Height = this.ClientSize.Height - txtDesc.Top;
			}
		}
		public void DeleteObject()
		{
			if (lstDrawings.SelectedIndex >= 0)
			{
				int i = lstDrawings.SelectedIndex;
				DrawingItem obj = (DrawingItem)lstDrawings.Items[i];
				dlgDrawings frm = this.Parent as dlgDrawings;
				if (drawings.Remove(obj))
				{
					lstDrawings.Items.RemoveAt(i);
					frm.DeleteDrawing(obj);
				}
			}
		}
		public void MoveUp()
		{
			if (lstDrawings.SelectedIndex > 0)
			{
				int i = lstDrawings.SelectedIndex;
				DrawingItem obj = (DrawingItem)lstDrawings.Items[i];
				if (drawings.MoveUp(obj))
				{
					ReloadData();
					lstDrawings.SelectedIndex = i - 1;
					dlgDrawings frm = this.Parent as dlgDrawings;
					frm.Invalidate();
				}
			}
		}
		public void MoveDown()
		{
			if (lstDrawings.SelectedIndex >= 0 && lstDrawings.SelectedIndex < lstDrawings.Items.Count - 1)
			{
				int i = lstDrawings.SelectedIndex;
				DrawingItem obj = (DrawingItem)lstDrawings.Items[i];
				if (drawings.MoveDown(obj))
				{
					ReloadData();
					lstDrawings.SelectedIndex = i + 1;
					dlgDrawings frm = this.Parent as dlgDrawings;
					frm.Invalidate();
				}
			}
		}
		public void ShowProperties(DrawingItem obj)
		{
			lstProperty.SelectedObject = obj;
		}
		public void SelectItem(Guid id)
		{
			for (int i = 0; i < lstDrawings.Items.Count; i++)
			{
				DrawingItem obj = (DrawingItem)lstDrawings.Items[i];
				if (obj.DrawingId == id)
				{
					lstDrawings.SelectedIndex = i;
					ShowProperties(obj);
					break;
				}
			}
		}
		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch (toolBar1.Buttons.IndexOf(e.Button))
			{
				case 0://new
					{
						dlgNewDrawing dlg = new dlgNewDrawing();
						dlg.LoadData(HotspotOnly);
						if (dlg.ShowDialog(this) == DialogResult.OK)
						{
							dlgDrawings frm = this.Parent as dlgDrawings;
							dlg.objRet.Page = frm;
							frm.StartDrawing(dlg.objRet);
							this.Hide();
						}
					}
					break;
				case 1://edit
					if (lstDrawings.SelectedIndex >= 0)
					{
						dlgDrawings frm = this.Parent as dlgDrawings;
						DrawingItem obj = lstDrawings.Items[lstDrawings.SelectedIndex] as DrawingItem;
						if (obj != null)
						{
							obj.Edit(frm);
						}
					}
					break;
				case 2://delete
					DeleteObject();
					break;
				case 3: //up
					MoveUp();
					break;
				case 4: //down
					MoveDown();

					break;
				case 5: //finish
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
					frmParent.DialogResult = System.Windows.Forms.DialogResult.OK;
					frmParent.Close();
					break;
				case 6: //cancel
					this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
					frmParent.DialogResult = System.Windows.Forms.DialogResult.Cancel;
					frmParent.Close();
					break;
				case 8: //show/hide background image
					if (e.Button.Pushed)
					{
						if (frmParent != null)
						{
							if (frmParent.BKImage != null)
							{
								frmParent.BackgroundImage = frmParent.BKImage;
							}
						}
					}
					else
					{
						frmParent.BackgroundImage = null;
					}
					frmParent.Invalidate();
					break;
			}
		}

		private void lstDrawings_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (frmParent != null)
			{
				if (lstDrawings.SelectedIndex >= 0)
				{
					DrawingItem obj = (DrawingItem)lstDrawings.Items[lstDrawings.SelectedIndex];
					frmParent.SelectDrawing(obj.DrawingId);
					ShowProperties(obj);
				}
			}
		}

		private void lblSep_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			x0 = e.X;
		}

		private void lblSep_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				int x = lblSep.Left + e.X - x0;
				if (x > 30 && x < this.ClientSize.Width - 30)
				{
					lblSep.Left = x;
					lstDrawings.Width = x;
					lstProperty.Left = x + lblSep.Width;
					lstProperty.Width = this.ClientSize.Width - lstProperty.Left;
					txtDesc.Left = lstProperty.Left;
					lblSepH.Left = lstProperty.Left;
					txtDesc.Width = lstProperty.Width;
					lblSepH.Width = lstProperty.Width;
				}
			}
		}

		private void lblSepH_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			y0 = e.Y;
		}

		private void lblSepH_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				int y = lblSepH.Top + e.Y - y0;
				if (y > 30 && y < this.ClientSize.Height - 30)
				{
					lblSepH.Top = y;
					lstProperty.Height = y;
					txtDesc.Top = y + lblSepH.Height;
					txtDesc.Height = this.ClientSize.Height - txtDesc.Top;
				}
			}
		}

		#region IAutoSettings Members

		public void AutoSettings(ref object v, bool save)
		{
			if (save)
			{
				v = new System.Drawing.Size(lblSep.Left, lblSepH.Top);
			}
			else
			{
				try
				{
					System.Drawing.Size size = (System.Drawing.Size)v;
					if (size.Width > 10 && size.Height > 10)
					{
						lblSep.Left = size.Width;
						lblSepH.Top = size.Height;
						frmDrawings_Resize(null, null);
					}
				}
				catch
				{
				}
			}
		}

		#endregion
	}
}
