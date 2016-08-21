/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design.Behavior;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Limnor.Drawing2D
{
	class DrawingItemDesignerGlyph : Glyph
	{
		// Cache references to services that will be needed.
		private BehaviorService behaviorService = null;
		private IComponentChangeService changeService = null;
		private ISelectionService selectionService = null;

		// This defines the bounds of the anchor glyph.
		protected Rectangle boundsValue;

		// Keep a reference to the designer for convenience.
		private IDesigner relatedDesigner = null;

		// Keep a reference to the adorner for convenience.
		private Adorner anchorAdorner = null;

		// Keep a reference to the DrawingItem being designed.
		private DrawingItem drawingItem = null;

		//private DrawingPage _page = null;

		public DrawingItemDesignerGlyph(
			BehaviorService behaviorService,
			IComponentChangeService changeService,
			ISelectionService selectionService,
			IDesigner relatedDesigner,
			Adorner anchorAdorner)
			: base(new DrawingItemBehavior(relatedDesigner))
		{
			// Cache references for convenience.

			this.behaviorService = behaviorService;
			this.changeService = changeService;
			this.selectionService = selectionService;
			this.relatedDesigner = relatedDesigner;
			this.anchorAdorner = anchorAdorner;

			// Cache a reference to the control being designed.
			this.drawingItem =
				this.relatedDesigner.Component as DrawingItem;
			//
			// Hook the SelectionChanged event. 
			this.selectionService.SelectionChanged +=
				new EventHandler(selectionService_SelectionChanged);

			// Hook the ComponentChanged event so the anchor glyphs
			// can correctly track the control's bounds.
			this.changeService.ComponentChanged +=
				new ComponentChangedEventHandler(changeService_ComponentChanged);
			//
			computeBounds();
		}
		void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			if (object.ReferenceEquals(
				this.selectionService.PrimarySelection,
				this.drawingItem))
			{
				this.computeBounds();
			}
		}

		// If any of several properties change, the bounds of the 
		// AnchorGlyph must be computed again.
		void changeService_ComponentChanged(
			object sender,
			ComponentChangedEventArgs e)
		{
			if (object.ReferenceEquals(
				e.Component,
				this.drawingItem))
			{
				if (
					string.CompareOrdinal(e.Member.Name, "Size") == 0 ||
					string.CompareOrdinal(e.Member.Name, "Height") == 0 ||
					string.CompareOrdinal(e.Member.Name, "Width") == 0 ||
					string.CompareOrdinal(e.Member.Name, "Location") == 0)
				{
					// Compute the bounds of this glyph.
					this.computeBounds();

					// Tell the adorner to repaint itself.
					this.anchorAdorner.Invalidate();
				}
			}
		}

		// This utility method computes the position and size of 
		// the AnchorGlyph in the Adorner window's coordinates.
		// It also computes the hit test bounds, which are
		// slightly larger than the glyph's bounds.
		private void computeBounds()
		{
			if (drawingItem.Page != null)
			{
				Rectangle rc = drawingItem.Bounds;
				boundsValue = new Rectangle(
					this.behaviorService.MapAdornerWindowPoint(drawingItem.Page.Handle, rc.Location),
					rc.Size);
			}
		}
		public override Rectangle Bounds
		{
			get
			{
				if (boundsValue.Width == 0 || boundsValue.Height == 0)
				{
					computeBounds();
				}
				return this.boundsValue;
			}
		}
		public override Cursor GetHitTest(Point p)
		{
			if (drawingItem.Page != null)
			{
				p = behaviorService.AdornerWindowPointToScreen(p);
				p = drawingItem.Page.PointToClient(p);
				if (drawingItem.HitTest(drawingItem.Page, p.X, p.Y))
				{
					return System.Windows.Forms.Cursors.Hand;
				}
			}
			return null;
		}

		public override void Paint(PaintEventArgs pe)
		{
		}

		#region Behavior Implementation


		// This Behavior specifies mouse and keyboard handling when
		// an AnchorGlyph is active. This happens when 
		// AnchorGlyph.GetHitTest returns a non-null value.
		internal class DrawingItemBehavior : Behavior
		{
			private IDesigner relatedDesigner = null;
			private DrawingItem _drawingItem = null;

			MouseButtons mb = MouseButtons.None;
			Point dp;

			internal DrawingItemBehavior(IDesigner relatedDesigner)
			{
				this.relatedDesigner = relatedDesigner;
				this._drawingItem = relatedDesigner.Component as DrawingItem;
			}
			public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
			{
				if ((mb & MouseButtons.Left) == MouseButtons.Left && mb == button)
				{

				}
				return base.OnMouseMove(g, button, mouseLoc);
			}
			public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
			{
				mb = button;
				if ((mb & MouseButtons.Left) == MouseButtons.Left)
				{
					dp = mouseLoc;

					if (_drawingItem.Page.NotifySelectionChange != null)
					{
						_drawingItem.Page.NotifySelectionChange(_drawingItem, EventArgs.Empty);
						return false;
					}
				}
				return base.OnMouseDown(g, button, mouseLoc);
			}
			public override bool OnMouseUp(Glyph g, MouseButtons button)
			{
				mb = MouseButtons.None;
				return base.OnMouseUp(g, button);
			}
			// When you double-click on an AnchorGlyph, the value of 
			// the control's Anchor property is toggled.
			//
			// Note that the value of the Anchor property is not set
			// by direct assignment. Instead, the 
			// PropertyDescriptor.SetValue method is used. This 
			// enables notification of the design environment, so 
			// related events can be raised, for example, the
			// IComponentChangeService.ComponentChanged event.

			public override bool OnMouseDoubleClick(
				Glyph g,
				MouseButtons button,
				Point mouseLoc)
			{
				base.OnMouseDoubleClick(g, button, mouseLoc);

				return true;
			}
			public override void OnQueryContinueDrag(Glyph g, QueryContinueDragEventArgs e)
			{
				e.Action = DragAction.Continue;
			}
		}


		#endregion

	}
}
