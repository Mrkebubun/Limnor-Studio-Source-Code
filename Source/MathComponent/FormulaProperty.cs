/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MathComponent
{
	public class FormulaProperty
	{
		private string _xml;
		public FormulaProperty()
		{
		}
		public FormulaProperty(string xml)
		{
			_xml = xml;
		}
		public string Xml
		{
			get
			{
				return _xml;
			}
			set
			{
				_xml = value;
			}
		}
		public override string ToString()
		{
			return string.Empty;
		}
	}
}
