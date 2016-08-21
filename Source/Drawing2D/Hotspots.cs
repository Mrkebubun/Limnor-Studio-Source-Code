/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Design;
using System.Collections.Generic;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for Hotspots.
	/// </summary>
	public class Hotspots : UserControl
	{
		#region Hotspots
		private frmHotspots form = null;
		private MouseEventArgs _mouseData;
		bool bLinked = false;
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			OnMove2();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			OnMove2();
		}
		protected void onPMove(object sender, EventArgs e)
		{
			OnMove2();
		}
		public void OnMove2()
		{
			if (form != null)
			{
				if (!form.IsDisposed)
				{
					form.Location = this.Parent.PointToScreen(this.Location);
					form.Size = this.Size;
				}
			}
		}
		public void clearBK()
		{
			if (form != null)
			{
				if (!form.IsDisposed)
				{
					form.Close();
					form.Dispose();
				}
				form = null;
			}
		}
		protected override void OnCreateControl()
		{
			if (form == null)
			{
				InitBK();
			}
		}
		public void InitBK()
		{
			if (form != null)
			{
				if (!form.IsDisposed)
				{
					if (!this.Created)
					{
						form.Close();
						form = null;
					}
					else
					{
						return;
					}
				}
				else
					form = null;
			}
			if (this.Created)
			{
				form = new frmHotspots();
				form.TopLevel = true;
				form.TopMost = true;
				form.SetCoverOwner(this);

				OnMove2();
				form.BackColor = form.TransparencyKey;
				form.Opacity = CanvasOpacity;
				form.Show();
				form.Cursor = CanvasCursor;
				form.TopMost = false;
				this.Visible = false;
				//
				form.MouseDown += new MouseEventHandler(form_mousedown);
				form.MouseMove += new MouseEventHandler(form_mousemove);
				form.MouseUp += new MouseEventHandler(form_mouseup);
				form.KeyDown += new KeyEventHandler(form_keydown);
				form.KeyUp += new KeyEventHandler(form_keyup);
				form.DoubleClick += new EventHandler(form_mousedoubleclick);
				//
				System.Windows.Forms.Control ctrl = this.Parent;
				while (ctrl != null)
				{
					ctrl.Move += new EventHandler(onPMove);
					ctrl = ctrl.Parent;
				}
				if (!bLinked)
				{
					bLinked = true;
					this.FindForm().VisibleChanged += new EventHandler(Hotspots_VisibleChanged);
					this.FindForm().GotFocus += new EventHandler(Hotspots_GotFocus);
				}
				if (!bHostVisible)
				{
					form.Hide();
				}
			}
		}
		public void SetDrawings(DrawingLayer drawings)
		{
			_drawings = drawings;
		}
		#endregion
		#region pass events
		void form_keydown(object sender, KeyEventArgs e)
		{
			NativeWIN32.SendMessage(this.Handle, NativeWIN32.WM_KEYDOWN, e.KeyValue, (int)e.KeyCode);
		}
		void form_keyup(object sender, KeyEventArgs e)
		{
			NativeWIN32.SendMessage(this.Handle, NativeWIN32.WM_KEYUP, e.KeyValue, (int)e.KeyCode);
		}
		void form_mousedown(object sender, MouseEventArgs e)
		{
			int wParam;
			int msg;
			_mouseData = e;
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				msg = NativeWIN32.WM_LBUTTONDOWN;
				wParam = NativeWIN32.MK_LBUTTON;
			}
			else if (e.Button == MouseButtons.Middle)
			{
				msg = NativeWIN32.WM_MBUTTONDOWN;
				wParam = NativeWIN32.MK_MBUTTON;
			}
			else if (e.Button == MouseButtons.Right)
			{
				msg = NativeWIN32.WM_RBUTTONDOWN;
				wParam = NativeWIN32.MK_RBUTTON;
			}
			else
			{
				msg = NativeWIN32.WM_LBUTTONDOWN;
				wParam = NativeWIN32.MK_LBUTTON;
			}
			NativeWIN32.SendMessage(this.Handle, msg, wParam, NativeWIN32.MakeDWord(e.X, e.Y));
		}
		void form_mousemove(object sender, MouseEventArgs e)
		{
			int wParam;
			int msg = NativeWIN32.WM_MOUSEMOVE;
			if (e.Button == MouseButtons.Left)
			{
				wParam = NativeWIN32.MK_LBUTTON;
			}
			else if (e.Button == MouseButtons.Middle)
			{
				wParam = NativeWIN32.MK_MBUTTON;
			}
			else if (e.Button == MouseButtons.Right)
			{
				wParam = NativeWIN32.MK_RBUTTON;
			}
			else
			{
				wParam = NativeWIN32.MK_LBUTTON;
			}
			NativeWIN32.SendMessage(this.Handle, msg, wParam, NativeWIN32.MakeDWord(e.X, e.Y));
		}
		void form_mouseup(object sender, MouseEventArgs e)
		{
			int wParam;
			int msg;
			_mouseData = e;
			if (e.Button == MouseButtons.Left)
			{
				msg = NativeWIN32.WM_LBUTTONUP;
				wParam = NativeWIN32.MK_LBUTTON;
			}
			else if (e.Button == MouseButtons.Middle)
			{
				msg = NativeWIN32.WM_MBUTTONUP;
				wParam = NativeWIN32.MK_MBUTTON;
			}
			else if (e.Button == MouseButtons.Right)
			{
				msg = NativeWIN32.WM_RBUTTONUP;
				wParam = NativeWIN32.MK_RBUTTON;
			}
			else
			{
				msg = NativeWIN32.WM_LBUTTONUP;
				wParam = NativeWIN32.MK_LBUTTON;
			}
			NativeWIN32.SendMessage(this.Handle, msg, wParam, (int)NativeWIN32.MakeDWord(e.X, e.Y));
		}
		void form_mousedoubleclick(object sender, EventArgs e)
		{
			form_mousedoubleclick2(sender, _mouseData);
		}
		void form_mousedoubleclick2(object sender, MouseEventArgs e)
		{
			int wParam;
			int msg;
			if (e.Button == MouseButtons.Left)
			{
				msg = NativeWIN32.WM_LBUTTONDBLCLK;
				wParam = NativeWIN32.MK_LBUTTON;
			}
			else if (e.Button == MouseButtons.Middle)
			{
				msg = NativeWIN32.WM_MBUTTONDBLCLK;
				wParam = NativeWIN32.MK_MBUTTON;
			}
			else if (e.Button == MouseButtons.Right)
			{
				msg = NativeWIN32.WM_RBUTTONDBLCLK;
				wParam = NativeWIN32.MK_RBUTTON;
			}
			else
			{
				msg = NativeWIN32.WM_LBUTTONDBLCLK;
				wParam = NativeWIN32.MK_LBUTTON;
			}
			NativeWIN32.SendMessage(this.Handle, msg, wParam, NativeWIN32.MakeDWord(e.X, e.Y));
		}
		#endregion
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Initilializer
		public Hotspots()
		{
			//
			Init();
			//
		}
		private void Init()
		{
			InitializeComponent();
		}
		#endregion
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region IPerformer Members
		public void OnClosing()
		{
			clearBK();
		}

		#endregion

		#region IProperties Members
		private DrawingLayer _drawings;
		[Description("Drawing objects for form hot spots")]
		[Editor(typeof(PropEditorDrawings), typeof(UITypeEditor))]
		public DrawingLayer Drawings
		{
			get
			{
				if (_drawings == null)
				{
					_drawings = new DrawingLayer();
				}
				return _drawings;
			}
			set
			{
				_drawings = value;
				if (form != null)
				{
					if (!form.IsDisposed)
					{
						form.BackColor = form.TransparencyKey;
						form.Invalidate();
					}
				}
				if (_drawings == null)
				{
					_drawings = new DrawingLayer();
				}
			}
		}
		private Color _color;
		public Color SpotColor
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
				if (form != null)
				{
					if (!form.IsDisposed)
					{
						form.BackColor = form.TransparencyKey;
						form.Invalidate();
					}
				}
			}
		}
		private float _opacity = 0.01F;
		public float CanvasOpacity
		{
			get
			{
				if (_opacity < 0.01)
					_opacity = 0.01F;
				if (_opacity > 1)
					_opacity = 1.0F;
				return _opacity;
			}
			set
			{
				if (value >= 0.01F && value <= 1.0F)
				{
					_opacity = value;
					if (form != null)
					{
						if (!form.IsDisposed)
						{
							form.BackColor = form.TransparencyKey;
							form.Opacity = (double)_opacity;
						}
					}
				}
			}
		}
		private bool _visible = true;
		public bool CanvasVisible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				if (!IsInDesignMode)
				{
					if (form != null)
					{
						if (!form.IsDisposed)
						{
							if (_visible)
							{
								form.BackColor = form.TransparencyKey;
								form.Opacity = CanvasOpacity;
							}
							form.Visible = _visible;
						}
					}
				}
			}
		}
		[Browsable(false)]
		public bool IsInDesignMode
		{
			get
			{
				if (Site != null)
				{
					return Site.DesignMode;
				}
				return false;
			}
		}
		private bool _enable;
		public bool CanvasEnable
		{
			get
			{
				return _enable;
			}
			set
			{
				_enable = value;
				if (!IsInDesignMode)
				{

					if (form != null)
					{
						if (!form.IsDisposed)
						{
							form.Enabled = _enable;
						}
					}
					this.Enabled = _enable;
				}
			}
		}
		private Cursor _cursor;
		public Cursor CanvasCursor
		{
			get
			{
				return _cursor;
			}
			set
			{
				_cursor = value;
				try
				{
					if (form != null)
					{
						if (!form.IsDisposed)
						{
							form.Cursor = _cursor;
						}
					}
					this.Cursor = _cursor;
				}
				catch
				{
				}
			}
		}


		#endregion

		#region IDeserialize2 Members

		public void OnDeserialize2()
		{
			this.clearBK();
			this.InitBK();
		}
		#endregion

		#region IEPDesign Members
		bool bHostVisible = true;
		public void OnMoveSize()
		{
			// TODO:  Add Hotspots.OnMoveSize implementation
		}

		public Control desiner
		{
			get
			{
				// TODO:  Add Hotspots.desiner getter implementation
				return null;
			}
		}

		public void OnChangeContainer()
		{
			clearBK();
			InitBK();
		}
		public void OnHostVisibleChange(bool bVisible)
		{
			bHostVisible = bVisible;
			if (form != null)
			{
				if (!form.IsDisposed)
				{
					form.BackColor = form.TransparencyKey;
					form.Opacity = CanvasOpacity;// GetOpacity();
					form.Visible = bVisible;
				}
			}
		}
		#endregion

		public string DistributionFiles()
		{
			return "";
		}

		private void Hotspots_VisibleChanged(object sender, EventArgs e)
		{
			if (form != null)
			{
				if (!form.IsDisposed)
				{
					form.BackColor = form.TransparencyKey;
					form.Opacity = CanvasOpacity;// GetOpacity();
					bHostVisible = this.FindForm().Visible;
					form.Visible = bHostVisible;
				}
			}
		}

		private void Hotspots_GotFocus(object sender, EventArgs e)
		{
			if (form != null)
			{
				if (!form.IsDisposed)
				{
					form.BackColor = form.TransparencyKey;
					form.Opacity = CanvasOpacity;// GetOpacity();
				}
			}
		}
	}
}
