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
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// To visually create drawing objects, the transformation
	/// of the object being edited must be the current graphic state.
	/// </summary>
	public class dlgDrawings : DrawingPage
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		const string XML_Editor = "Editor";
		const string XML_Toolbox = "Toolbox";
		//
		public FormDrawingsProperties editor = null;
		private FormToolbox _toolBox;
		//
		public DrawingLayer lstShapes = null;

		protected frmHotspots formHS = null;
		protected Hotspots hs = null;
		protected int nType = 0;
		protected string sHelp = "";
		protected int xH = 0, yH = 0;
		protected Font ftHelp = new Font(FontFamily.GenericSansSerif, 12);
		private int nStep = 0;
		private DrawingItem curDrawing = null;//for current obj drawing
		private Matrix curTransform = new Matrix(1, 0, 0, 1, 0, 0);
		private Image _imgBK = null;
		//
		protected DrawingItem selectedDrawing;
		protected DrawingDesign designer;
		private DrawingItem _newObj;
		//
		private Label[] frames;
		//
		const int FrameWidth = 2;
		const int ShadowWidth = 6;
		//
		static DefaultDrawings _defaultDrawings;
		//
		public dlgDrawings()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			frames = new Label[4];
			frames[0] = new Label();
			frames[0].Visible = false;
			frames[0].Height = FrameWidth;
			frames[0].Text = "";
			frames[0].BackColor = System.Drawing.Color.Gray;
			this.Controls.Add(frames[0]);
			frames[1] = new Label();
			frames[1].Visible = false;
			frames[1].Width = FrameWidth;
			frames[1].Text = "";
			frames[1].BackColor = System.Drawing.Color.Gray;
			this.Controls.Add(frames[1]);
			//
			frames[2] = new Label();
			frames[2].Visible = false;
			frames[2].Height = ShadowWidth;
			frames[2].Text = "";
			frames[2].BackColor = System.Drawing.Color.LightGray;
			this.Controls.Add(frames[2]);
			frames[3] = new Label();
			frames[3].Visible = false;
			frames[3].Width = ShadowWidth;
			frames[3].Text = "";
			frames[3].BackColor = System.Drawing.Color.LightGray;
			this.Controls.Add(frames[3]);
			//
			editor = new FormDrawingsProperties();
			editor.TopLevel = false;
			editor.Parent = this;
			//			editor.Owner = this;
			editor.SetPage(this);
			//editor.Show();
			editor.Location = Config.GetEditorPropertyGridPosition(XML_Editor);
			//
			_toolBox = new FormToolbox();
			_toolBox.TopLevel = false;
			_toolBox.Parent = this;
			_toolBox.Visible = true;
			_toolBox.Location = Config.GetEditorPropertyGridPosition(XML_Toolbox);
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
			// dlgDrawings
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.AutoScrollMargin = new System.Drawing.Size(30, 30);
			this.AutoScrollMinSize = new System.Drawing.Size(10, 10);
			this.ClientSize = new System.Drawing.Size(648, 334);
			this.ControlBox = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MinimizeBox = false;
			this.Name = "dlgDrawings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Make drawings";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dlgDrawings_KeyPress);
			this.Activated += new System.EventHandler(this.dlgDrawings_Activated);
			//this.Paint += new System.Windows.Forms.PaintEventHandler(this.dlgDrawings_Paint);
		}
		#endregion
		//
		[Browsable(false)]
		public override bool InDesignMode
		{
			get
			{
				return true;
			}
		}
		public Image BKImage
		{
			get
			{
				return _imgBK;
			}
			set
			{
				_imgBK = value;
			}
		}
		public void UseSubset(Type[] types)
		{
			_toolBox.UseSubset(types);
		}

		public void ReloadShapes(IList<DrawingItem> shapes)
		{
			this.ClearDrawings();
			if (shapes != null)
			{
				foreach (DrawingItem di in shapes)
				{
					this.AddDrawing(di);
				}
			}
		}
		public void SetPageFrame()
		{
			if (frames != null)
			{
				if (this.PageAttributes.ShowPrintPageEdges)
				{
					Size sz = PageAttributes.PageSizeInPixels;
					int w = sz.Width;
					int h = sz.Height;
					//page frame
					frames[0].Left = 0;
					frames[0].Width = w + FrameWidth;
					frames[0].Top = h;
					frames[0].Visible = true;
					frames[1].Top = 0;
					frames[1].Height = h + FrameWidth;
					frames[1].Left = w;
					frames[1].Visible = true;
					//shadown
					frames[2].Left = ShadowWidth;
					frames[2].Width = w + FrameWidth;
					frames[2].Top = h + FrameWidth;
					frames[2].Visible = true;
					frames[3].Top = ShadowWidth;
					frames[3].Height = h + FrameWidth;
					frames[3].Left = w + FrameWidth;
					frames[3].Visible = true;
				}
				else
				{
					for (int i = 0; i < frames.Length; i++)
					{
						frames[i].Visible = false;
					}
				}
			}
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetPageFrame();
		}

		public void LoadData(Hotspots hotspots)
		{
		}
		public void DeleteDrawing(DrawingItem obj)
		{
			if (nType == 0)
			{
				RemoveDrawing(obj);
				this.ClearMarks();
				this.Invalidate();
				editor.RefreshPropertyGrids();
			}
			else
			{
				lstShapes.DeleteDrawingByID(obj.DrawingId);
				formHS.Invalidate();
			}
		}
		public void StartDrawing(DrawingItem obj)
		{
			nStep = 0;
			curDrawing = obj.Clone();
			if (formHS != null)
			{
				curDrawing.ConvertToScreen(formHS.Location);
			}
			curDrawing.StartDesign();
			saveTransform();
			sHelp = curDrawing.Help();
			this.Invalidate();
			this.Capture = true;
		}
		/// <summary>
		/// translate page coordinates to current drawing object's coordinates
		/// </summary>
		/// <param name="x0"></param>
		/// <param name="y0"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		public void systemToCustomerXY(float x0, float y0, out float x1, out float y1)
		{
			float x = x0 - curTransform.Elements[4];
			float y = y0 - curTransform.Elements[5];
			x1 = (x * curTransform.Elements[0] + y * curTransform.Elements[1]);
			y1 = (x * curTransform.Elements[2] + y * curTransform.Elements[3]);
		}
		private void saveTransform()
		{
			if (curDrawing != null)
			{
				bool bFound = false;
				Graphics g = this.CreateGraphics();
				DrawingLayer lstDrawings = GetLayerById(curDrawing.LayerId);
				if (lstDrawings == null)
				{
					throw new ExceptionDrawing2D("Layer {0} not found", curDrawing.LayerId);
				}
				foreach (DrawingItem obj in lstDrawings)
				{
					if (obj.DrawingId == curDrawing.DrawingId)
					{
						curTransform = g.Transform;
						bFound = true;
						break;
					}
					obj.Draw(g);
				}
				if (!bFound)
					curTransform = g.Transform;
				g.Dispose();
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			GraphicsState gs = e.Graphics.Save();
			GraphicsState gsCur = null;
			e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
			foreach (DrawingLayer lstDrawings in DrawingLayers)
			{
				if (lstDrawings.Visible)
				{
					foreach (DrawingItem obj in lstDrawings)
					{
						if (curDrawing != null)
						{
							if (obj.DrawingId == curDrawing.DrawingId)
							{
								gsCur = e.Graphics.Save();
							}
						}
						obj.Draw(e.Graphics);
					}
				}
			}
			if (curDrawing != null)
			{
				if (gsCur != null)
				{
					e.Graphics.Restore(gsCur);
				}
				//draw coordinate lines
				e.Graphics.DrawLine(System.Drawing.Pens.CadetBlue, -600, 0, 600, 0);
				e.Graphics.DrawLine(System.Drawing.Pens.CadetBlue, 0, -600, 0, 600);
				curDrawing.Draw(e.Graphics);
			}
			e.Graphics.Restore(gs);
			if (formHS != null)
			{
				e.Graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 8), formHS.Left - 8, formHS.Top - 8, formHS.Width + 16, formHS.Height + 16);
			}
			if (sHelp.Length > 0)
			{
				e.Graphics.DrawString(sHelp, ftHelp, System.Drawing.Brushes.LightGreen, xH, yH);
			}
		}

		private void dlgDrawings_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				//finish drawing item or finish all (close)
			}
			else if (e.KeyChar == 27)
			{
				//start again
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			sHelp = "";
			if (curDrawing != null)
			{
				float x1, y1;
				bool bAdded;
				systemToCustomerXY(e.X, e.Y, out x1, out y1);
				System.Windows.Forms.MouseEventArgs e1 = new System.Windows.Forms.MouseEventArgs(e.Button, e.Clicks, (int)x1, (int)y1, e.Delta);
				System.Windows.Forms.DialogResult ret = curDrawing.CallDlgDrawingsMouseDown(this, e1, ref nStep, this);
				switch (ret)
				{
					case System.Windows.Forms.DialogResult.OK:
						if (nType == 0)
						{
							bAdded = this.DrawingLayers[0].AddDrawing(curDrawing);
						}
						else
						{
							bAdded = lstShapes.AddDrawing(curDrawing);
							hs.SetDrawings(lstShapes);
							formHS.Show();
							formHS.TopMost = true;
							formHS.Invalidate();
						}
						if (bAdded)
						{
							AddDrawing(curDrawing);
						}
						else
						{
						}
						editor.SelectItem(curDrawing);
						curDrawing.FinishDesign();
						curDrawing = null;
						nStep = 0;
						editor.Show();
						break;
					case System.Windows.Forms.DialogResult.Cancel:
						curDrawing.FinishDesign();
						curDrawing = null;
						nStep = 0;
						editor.Show();
						break;
					case System.Windows.Forms.DialogResult.Retry:
						nStep = 0;
						break;
					case System.Windows.Forms.DialogResult.None:
						break;
				}
				this.Invalidate();
			}
			else
			{
				Point p = new Point(e.X, e.Y);
				DrawingItem item = HitTest(p);
				if (item != null)
				{
					SetItemSelection(item);
					if (e.Button == MouseButtons.Right)
					{
						MenuItem[] menus = item.GetContextMenuItems(p);
						if (menus != null && menus.Length > 0)
						{
							ContextMenu cm = new ContextMenu();
							cm.MenuItems.AddRange(menus);
							cm.Show(this, p);
						}
					}
				}
				else
				{
					ClearMarks();
					if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
					{
						Type t = _toolBox.SelectedToolboxItem;
						if (t != null)
						{
							DrawingItem newObj = (DrawingItem)Activator.CreateInstance(t);
							if (_defaultDrawings == null)
							{
								_defaultDrawings = new DefaultDrawings();
							}
							DrawingItem def = _defaultDrawings.GetDefaultDrawing(newObj.GetType());
							if (def != null)
							{
								newObj.Copy(def);
							}
							_toolBox.ClearToolboxSelection();
							newObj.ResetGuid();
							makeNewName(newObj);
							newObj.Page = this;
							newObj.Location = p;
							AddDrawing(newObj);
							editor.SelectItem(newObj);
							editor.RefreshPropertyGrids();
							_newObj = newObj;
							SetItemSelection(newObj);
						}
					}
				}
			}
		}
		public void SetItemSelection(DrawingItem item)
		{
			ClearMarks();
			selectedDrawing = item;
			selectedDrawing.IsSelected = true;
			if (selectedDrawing.Container != null)
			{
				selectedDrawing.Container.IsSelected = true;
			}
			designer = selectedDrawing.CreateDesigner();
			editor.SelectItem(selectedDrawing);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			try
			{
				if (curDrawing != null)
				{
					if (curDrawing.UseMouseMove())
					{
						if (sHelp.Length > 0)
						{
							xH = e.X;
							yH = e.Y;
							System.Drawing.Graphics g = this.CreateGraphics();
							g.DrawString(sHelp, ftHelp, System.Drawing.Brushes.LightGreen, e.X, e.Y);
							g.Dispose();
						}
						float x1, y1;
						systemToCustomerXY(e.X, e.Y, out x1, out y1);
						MouseEventArgs e1 = new MouseEventArgs(e.Button, e.Clicks, (int)x1, (int)y1, e.Delta);
						if (curDrawing.dlgDrawingsMouseMove(this, e1, ref nStep, this))
						{
							this.Invalidate();
						}
					}
					else
					{
						OnMouseDown(e);
					}
				}
				else
				{
					if (e.Button == System.Windows.Forms.MouseButtons.None)
					{
						DrawingItem v = HitTest(new Point(e.X, e.Y));
						if (v != null)
						{
							this.Cursor = System.Windows.Forms.Cursors.Hand;
						}
						else
						{
							this.Cursor = System.Windows.Forms.Cursors.Default;
						}
					}
					else if ((e.Button & MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left)
					{
						if (_newObj != null)
						{
						}
					}
				}
			}
			catch
			{
			}
		}
		public void ResetPositions()
		{
			if (editor != null && !editor.IsDisposed)
			{
				editor.Location = new Point(this.ClientSize.Width - editor.Width - 30, 0);
				editor.Size = new Size(300, 300);
			}
			if (_toolBox != null && !_toolBox.IsDisposed)
			{
				_toolBox.Location = new Point(this.ClientSize.Width - editor.Width - 30 - _toolBox.Width, 0);
			}
		}
		public void ToggleToolboxVisible()
		{
			_toolBox.Visible = !_toolBox.Visible;
		}
		public void ClearToolboxSelection()
		{
			_toolBox.ClearToolboxSelection();
		}
		public Type SelectedToolboxItem
		{
			get
			{
				return _toolBox.SelectedToolboxItem;
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			Config.SetEditorPropertyGridPosition(editor.Location, XML_Editor);
			Config.SetEditorPropertyGridPosition(_toolBox.Location, XML_Toolbox);
			if (formHS != null)
			{
				if (!formHS.IsDisposed)
				{
					formHS.Close();
					formHS.Dispose();
				}
				formHS = null;
			}
			Form p = this.Parent as Form;
			if (p != null)
			{
				p.DialogResult = this.DialogResult;
				p.Close();
			}
		}
		public void ClearMarks()
		{
			if (selectedDrawing != null)
			{
				selectedDrawing.IsSelected = false;
				if (selectedDrawing.Container != null)
				{
					selectedDrawing.Container.IsSelected = false;
				}
			}
			bool b = true;
			while (b)
			{
				b = false;
				for (int i = 0; i < Controls.Count; i++)
				{
					if (Controls[i] is DrawingMark)
					{
						b = true;
						Controls.RemoveAt(i);
						break;
					}
				}
			}
			selectedDrawing = null;
			designer = null;
		}
		public void MoveSelectedItemUp()
		{
			if (selectedDrawing != null)
			{
				MoveItemUp(selectedDrawing);
				editor.RefreshPropertyGrids();
			}
		}
		public void MoveSelectedItemDown()
		{
			if (selectedDrawing != null)
			{
				MoveItemDown(selectedDrawing);
				editor.RefreshPropertyGrids();
			}
		}
		public void SetSelectedItemAsDefaultDrawing()
		{
			if (selectedDrawing != null)
			{
				if (_defaultDrawings == null)
				{
					_defaultDrawings = new DefaultDrawings();
				}
				_defaultDrawings.SetDefaultDrawing(selectedDrawing);
			}
		}
		public void NewObject()
		{
			dlgNewDrawing dlg = new dlgNewDrawing();
			dlg.LoadData(false);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				if (_defaultDrawings == null)
				{
					_defaultDrawings = new DefaultDrawings();
				}
				DrawingItem def = _defaultDrawings.GetDefaultDrawing(dlg.objRet.GetType());
				if (def != null)
				{
					dlg.objRet.Copy(def);
				}
				dlg.objRet.ResetGuid();
				makeNewName(dlg.objRet);
				dlg.objRet.Page = this;
				AddDrawing(dlg.objRet);
				editor.RefreshPropertyGrids();
			}
		}
		public override void MoveItemToLayer(DrawingItem item, DrawingLayer layer)
		{
			base.MoveItemToLayer(item, layer);
			editor.RefreshPropertyGrids();
		}
		public void DeleteSelectedItem()
		{
			if (selectedDrawing != null)
			{
				DeleteDrawing(selectedDrawing);
				selectedDrawing = null;
			}
		}
		public void OK()
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
		public void CANCEL()
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
		public void EditObject()
		{
			if (selectedDrawing != null)
			{
				selectedDrawing.Edit(this);
			}
		}
		public void SelectDrawing(Guid id)
		{
			DrawingItem obj = GetDrawingItemById(id);
			if (obj != null)
			{
				selectedDrawing = obj;
				designer = selectedDrawing.CreateDesigner();
			}
		}
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			editor.Show();
		}
		protected override void WndProc(ref Message m)
		{
			const int WM_HSCROLL = 0x0114;
			const int WM_VSCROLL = 0x0115;
			switch (m.Msg)
			{
				case WM_HSCROLL:
					if (this.Parent != null)
					{
						this.Parent.Invalidate();
					}
					this.Invalidate();
					break;
				case WM_VSCROLL:
					if (this.Parent != null)
					{
						this.Parent.Invalidate();
					}
					this.Invalidate();
					break;
			}
			base.WndProc(ref m);
		}

		private void dlgDrawings_Activated(object sender, System.EventArgs e)
		{
			this.Invalidate();
		}
		private void makeNewName(DrawingItem draw)
		{
			this.DrawingLayers.SetNewName(draw);
		}
	}
	[ReadOnly(true)]
	public class DrawingMark : System.Windows.Forms.Label, IMessageReceiver
	{
		public int Index = 0;
		public DrawingItem Owner = null;
		public string Info = "";
		protected System.Drawing.Font ftInfo;
		int x0 = 0;
		int y0 = 0;
		public DrawingMark()
		{
			this.BackColor = System.Drawing.Color.DodgerBlue;
			this.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Width = DrawingItem.BOXSIZE;
			this.Height = DrawingItem.BOXSIZE;
			ftInfo = new Font("Times New Roman", 6);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Info.Length > 0)
			{
				e.Graphics.DrawString(Info, ftInfo, System.Drawing.Brushes.Red, e.Graphics.ClipBounds);
			}
		}

		public int X
		{
			get
			{
				return this.Left + DrawingItem.BOXSIZE2;
			}
			set
			{
				this.Left = value - DrawingItem.BOXSIZE2;
			}
		}
		public int Y
		{
			get
			{
				return this.Top + DrawingItem.BOXSIZE2;
			}
			set
			{
				this.Top = value - DrawingItem.BOXSIZE2;
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			x0 = e.X;
			y0 = e.Y;
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (Owner != null)
			{
				Owner._OnMouseUp(e);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (Owner != null && this.Parent != null)
			{
				if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
				{
					int x = this.Left + e.X - x0;
					if (x >= 0 && x < this.Parent.ClientSize.Width)
					{
						this.Left = x;
					}
					x = this.Top + e.Y - y0;
					if (x >= 0 && x < this.Parent.ClientSize.Height)
					{
						this.Top = x;
					}
					Owner._OnMouseMove(this, e);
				}
			}
		}


		#region IMessageReceiver Members
		protected bool _messageReturn = false; //allow message to go through
		public bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDown(e);
			return _messageReturn;
		}

		public bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseMove(e);
			return _messageReturn;
		}

		public bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseUp(e);
			return _messageReturn;
		}

		public bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDoubleClick(e);
			return _messageReturn;
		}

		public bool FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
			return _messageReturn;
		}

		public bool FireKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
			return _messageReturn;
		}

		#endregion
	}
	[ReadOnly(true)]
	public class DrawingMover : DrawingMark
	{
		public DrawingMover()
			: base()
		{
			this.Cursor = System.Windows.Forms.Cursors.SizeAll;
			this.BackColor = System.Drawing.Color.Yellow;
		}
	}
	[ReadOnly(true)]
	public class DrawingRotate : DrawingMark
	{
		public DrawingRotate()
			: base()
		{
			this.Cursor = System.Windows.Forms.Cursors.Cross;
			this.BackColor = System.Drawing.Color.Red;
		}
	}
	public class DrawingDesign
	{
		public DrawingMark[] Marks = null;
		public DrawingDesign()
		{
		}
	}
}
