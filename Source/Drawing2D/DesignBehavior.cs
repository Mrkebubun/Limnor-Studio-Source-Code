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
using System.Windows.Forms;
using System.Drawing;

namespace Limnor.Drawing2D
{
	public class DesignBehaviorDrawing2D : Behavior
	{
		DrawingPage _page;
		BehaviorService _behaviorService;
		DrawingItem _itemUnderMouse;
		public DesignBehaviorDrawing2D(DrawingPage page, BehaviorService behaviorService)
		{
			_page = page;
			_behaviorService = behaviorService;
		}
		public override Cursor Cursor
		{
			get
			{
				if (_itemUnderMouse != null)
				{
					return System.Windows.Forms.Cursors.Hand;
				}
				return base.Cursor;
			}
		}
		/// <summary>
		/// Called when any mouse-move message enters the adorner window of the BehaviorService. 
		/// The OnMouseDoubleClick{?} method is called when any mouse-move message enters the WndProc 
		/// of the adorner window of the BehaviorService. The message is first passed here, 
		/// to the top-most Behavior in the behavior stack. Returning true from this method signifies 
		/// that the message was handled by the Behavior and should not continue to be processed. 
		/// From here, the message is sent to the appropriate behavior. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="button"></param>
		/// <param name="mouseLoc"></param>
		/// <returns></returns>
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
		{
			Point p = _behaviorService.AdornerWindowPointToScreen(new Point(mouseLoc.X, mouseLoc.Y));
			p = _page.PointToClient(p);
			_itemUnderMouse = _page.HitTest(p);
			if (_itemUnderMouse != null)
			{
				_page.Cursor = System.Windows.Forms.Cursors.Hand;
			}
			return false;
		}
		/// <summary>
		/// Called when any mouse-down message enters the adorner window of the BehaviorService. 
		/// The OnMouseDoubleClick(?) method is called when any mouse-down message enters the WndProc of the adorner window of the BehaviorService. The message is first passed here, to the top-most Behavior in the behavior stack. Returning true from this method signifies that the message was handled by the Behavior and should not continue to be processed. From here, the message is sent to the appropriate behavior. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="button">A MouseButtons value indicating which button was clicked.</param>
		/// <param name="mouseLoc">The location at which the click occurred.</param>
		/// <returns>true if the message was handled; otherwise, false.</returns>
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
		{
			return _page.DesignTimeOnMouseDown(button, mouseLoc);
		}
	}
}
