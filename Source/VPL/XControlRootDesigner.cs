/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace VPL
{
	public class XControlRootDesigner : ControlDesigner, IRootDesigner
	{
		public XControlRootDesigner()
		{
		}
		public ContextMenu ContextMenu
		{
			get
			{
				return new ContextMenu();
			}
		}
		#region IRootDesigner Members
		private RootDesignerView _view;
		public object GetView(ViewTechnology technology)
		{
			// If the design environment requests a view technology other than Windows 
			// Forms, this method throws an Argument Exception.
			if (technology != ViewTechnology.Default)
				throw new ArgumentException("An unsupported view technology was requested",
				"Unsupported view technology.");

			// Creates the view object if it has not yet been initialized.
			if (_view == null)
				_view = new RootDesignerView(this);

			return _view;

		}

		public ViewTechnology[] SupportedTechnologies
		{
			get
			{
				return new ViewTechnology[] { ViewTechnology.Default };
			}
		}

		#endregion
		// This control provides a Windows Forms view technology view object that 
		// provides a display for the SampleRootDesigner.
		[DesignerAttribute(typeof(ControlDesigner), typeof(IDesigner))]
		internal class RootDesignerView : Control
		{
			// This field stores a reference to a designer.
			private IDesigner m_designer;

			public RootDesignerView(IDesigner designer)
			{
				// Perform basic control initialization.
				m_designer = designer;
				BackColor = Color.Blue;
				Font = new Font(Font.FontFamily.Name, 24.0f);
			}

			// This method is called to draw the view for the SampleRootDesigner.
			protected override void OnPaint(PaintEventArgs pe)
			{
				base.OnPaint(pe);
				// Draw the name of the component in large letters.
				pe.Graphics.DrawString(m_designer.Component.Site.Name, Font, Brushes.Yellow, ClientRectangle);
			}
		}

	}
}
