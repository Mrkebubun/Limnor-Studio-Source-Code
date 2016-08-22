/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace VPL
{
	/// <summary>
	/// When a DrawingPage is used as an instance, it is wrapped in XCLass, 
	/// XClass use this interface to get the properties DrawingPage intended to publish
	/// </summary>
	public interface IClassInstance
	{
		PropertyDescriptorCollection GetInstanceProperties(Attribute[] attributes);
	}
	public interface IDrawingHolder
	{
		void OnFinishLoad();
		void RefreshDrawingOrder();
		void BringObjectToFront(object drawingObject);
		void SendObjectToBack(object drawingObject);
		Point Location { get; }
		IDrawingHolder Holder { get; }
	}
}
