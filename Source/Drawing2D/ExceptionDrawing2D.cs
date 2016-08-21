/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Drawing2D
{
	class ExceptionDrawing2D : Exception
	{
		public ExceptionDrawing2D(string message, params object[] values) :
			base(string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values))
		{

		}
	}
}
