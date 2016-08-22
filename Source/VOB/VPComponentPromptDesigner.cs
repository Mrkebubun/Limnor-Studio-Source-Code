/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Data;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace VOB
{
	/// <summary>
	/// A custom RootDesigner.
	/// </summary>
	public class VPComponentPromptDesigner : ComponentDesigner, IRootDesigner, IToolboxUser
	{
		VPComponentPromptView _rootView;

		#region Implementation of IRootDesigner
		public object GetView(System.ComponentModel.Design.ViewTechnology technology)
		{
			if (_rootView == null)
				_rootView = new VPComponentPromptView(this);
			return _rootView;
		}
		public System.ComponentModel.Design.ViewTechnology[] SupportedTechnologies
		{
			get
			{
				return new ViewTechnology[] { ViewTechnology.Default };
			}
		}
		#endregion

		#region Implementation of IToolboxUser
		public void ToolPicked(System.Drawing.Design.ToolboxItem tool)
		{
			_rootView.InvokeToolboxItem(tool);
		}

		public bool GetToolSupported(System.Drawing.Design.ToolboxItem tool)
		{
			return true;
		}
		#endregion

		public new object GetService(Type type)
		{
			return base.GetService(type);
		}


		#region VPComponentPromptView
		/// <summary>
		/// This is the View of the RootDesigner.
		/// </summary>
		public class VPComponentPromptView : Control
		{
			private string _displayString = "Limnor VOB is about building objects visually. Use objects in the Toolbox to compose new objects. \r\nDrag an item from the Toolbox and drop it here to start building a new object.";
			private VPComponentPromptDesigner _rootDesigner;
			public VPComponentPromptView(VPComponentPromptDesigner rootDesigner)
			{
				_rootDesigner = rootDesigner;
				this.AllowDrop = true;
				Invalidate();
			}
			protected override void OnPaint(PaintEventArgs pe)
			{
				try
				{
					StringFormat sf = new StringFormat();
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					pe.Graphics.FillRectangle(new LinearGradientBrush(this.Bounds, Color.White, Color.Blue, 45F), this.Bounds);
					pe.Graphics.DrawString(_displayString, Font, new SolidBrush(Color.Black), this.Bounds, sf);
				}
				catch (Exception ex)
				{
					pe.Graphics.DrawString(ex.ToString(), this.Font, Brushes.Red, 0, 0);
				}
			} // OnPaint


			public void InvokeToolboxItem(System.Drawing.Design.ToolboxItem tool)
			{
				IDesignerHost dh = DesignerHost;
				if (dh != null)
				{
					VOB.InterfaceVOB vob = dh.GetService(typeof(VOB.InterfaceVOB)) as VOB.InterfaceVOB;
					if (vob != null)
					{
						vob.SendNotice(VOB.enumVobNotice.NewObject, tool);
					}
				}
				Invalidate();
			}
			protected override void OnDragDrop(DragEventArgs e)
			{
				object data = e.Data.GetData(typeof(ToolboxItem));
				if (data != null)
				{
					IDesignerHost dh = DesignerHost;
					if (dh != null)
					{
						VOB.InterfaceVOB vob = dh.GetService(typeof(VOB.InterfaceVOB)) as VOB.InterfaceVOB;
						if (vob != null)
						{
							vob.SendNotice(VOB.enumVobNotice.NewObject, data);
						}
					}
				}
			}
			protected override void OnDragEnter(DragEventArgs e)
			{
				base.OnDragEnter(e);
				//
				IToolboxService ts = ToolboxService;
				if (ts != null && ts.IsToolboxItem(e.Data, DesignerHost))
				{
					e.Effect = DragDropEffects.Copy;
				}
			}

			/// <summary>
			/// Clear the drag object we may have stored in OnDragEnter.
			/// </summary>
			protected override void OnDragLeave(EventArgs e)
			{
				base.OnDragLeave(e);
			}

			/// <summary>
			/// Overridden so we can give feedback about the drag.
			/// </summary>
			protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
			{
				base.OnGiveFeedback(e);
			}
			protected override void OnMouseEnter(EventArgs e)
			{
				base.OnMouseEnter(e);
				IDesignerHost idh = _rootDesigner.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (idh != null)
				{
					VOB.InterfaceVOB vob = idh.GetService(typeof(VOB.InterfaceVOB)) as VOB.InterfaceVOB;
					if (vob != null)
					{
						vob.SendNotice(VOB.enumVobNotice.HideToolbox, true);
					}
				}
			}
			public IDesignerHost DesignerHost
			{
				get
				{
					return (IDesignerHost)_rootDesigner.GetService(typeof(IDesignerHost));
				}
			}

			public IToolboxService ToolboxService
			{
				get
				{
					return (IToolboxService)_rootDesigner.GetService(typeof(IToolboxService));
				}
			}
			protected override void OnResize(EventArgs e)
			{
				Invalidate();
			}
		} // class NewComponentPromptView
		#endregion

	}// class NewComponentPromptDesigner
}// namespace
