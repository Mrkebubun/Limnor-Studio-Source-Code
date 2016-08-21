/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MathExp
{
	/// <summary>
	/// for attach data and postion to menu
	/// </summary>
	public class ObjectAndPos
	{
		public Point Location;
		public object Data;
		public ObjectAndPos(Point pos, object val)
		{
			Location = pos;
			Data = val;
		}
	}
}
