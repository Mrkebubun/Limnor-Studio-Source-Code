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

namespace MathExp
{
	public class HitTestResult
	{
		MathNode _previous;
		MathNode _current;
		public HitTestResult(MathNode replaced, MathNode hit)
		{
			_previous = replaced;
			_current = hit;
		}
		public MathNode Replaced
		{
			get
			{
				return _previous;
			}
		}
		public MathNode Current
		{
			get
			{
				return _current;
			}
		}
	}
}
