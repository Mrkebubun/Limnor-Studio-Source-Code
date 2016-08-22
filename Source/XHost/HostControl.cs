/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using VOB;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Drawing2D;

namespace XHost
{
	/// <summary>
	/// Hosts the HostSurface which inherits from DesignSurface.
	/// </summary>
	public class HostControl : UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private HostSurface _hostSurface;
		private StringFormat _componentTextFormat;
		private Font _componentTextFont;
		public HostControl(HostSurface hostSurface)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			InitializeHost(hostSurface);
			//
			_componentTextFormat = new StringFormat();
			_componentTextFormat.Alignment = StringAlignment.Center;
			_componentTextFormat.LineAlignment = StringAlignment.Center;
			_componentTextFormat.Trimming = StringTrimming.None;
			_componentTextFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
			//
			_componentTextFont = new Font("Times New Roman", 12);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
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
			// 
			// HostControl
			// 
			this.Name = "HostControl";
			this.Size = new System.Drawing.Size(268, 224);
		}
		#endregion

		internal void InitializeHost(HostSurface hostSurface)
		{
			try
			{
				if (hostSurface == null)
					return;

				_hostSurface = hostSurface;

				Control control = _hostSurface.View as Control;

				control.Parent = this;
				control.Dock = DockStyle.Fill;
				control.Visible = true;
				control.MouseEnter += new EventHandler(HostControl_MouseEnter);
				//
				if (!(DesignerHost.RootComponent is Control) && !(DesignerHost.RootComponent is NewComponentPrompt))
				{
					if (control.Controls.Count > 0)
					{
						if (control.Controls[0] != null)
						{
							control.Controls[0].Text = "";
							control.Controls[0].Paint += new PaintEventHandler(HostControl_Paint);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		void HostControl_MouseEnter(object sender, EventArgs e)
		{
			VOB.InterfaceVOB iv = (VOB.InterfaceVOB)_hostSurface.GetService(typeof(VOB.InterfaceVOB));
			if (iv != null)
			{
				iv.SendNotice(enumVobNotice.HideToolbox, true);
			}
		}

		void HostControl_Paint(object sender, PaintEventArgs e)
		{
			Control c = sender as Control;
			RectangleF _rectF = new RectangleF((float)c.Left, (float)c.Top, (float)c.Width, (float)c.Height);
			e.Graphics.FillRectangle(new LinearGradientBrush(c.ClientRectangle, Color.White, Color.Gray, 45F), c.ClientRectangle);
			e.Graphics.DrawString("To add components to your object, drag them from the Toolbox and use the Properties window to set their properties. To create properties, methods and events for your object, in the Object Explorer right-click on 'Properties', 'Methods' and 'Events' nodes under 'Attributes' node of your object.",
				_componentTextFont, Brushes.Black, _rectF, _componentTextFormat);
		}
		public HostSurface HostSurface
		{
			get
			{
				return _hostSurface;
			}
		}
		#region IXHostControl Members

		public DesignSurface DesignSurface
		{
			get { return _hostSurface; }
		}
		public IDesignerHost DesignerHost
		{
			get
			{
				return (IDesignerHost)_hostSurface.GetService(typeof(IDesignerHost));
			}
		}
		public bool SelectComponent(object v)
		{
			if (_hostSurface != null)
			{
				return _hostSurface.SelectComponent(v);
			}
			return false;
		}
		#endregion
	} // class
}// namespace
