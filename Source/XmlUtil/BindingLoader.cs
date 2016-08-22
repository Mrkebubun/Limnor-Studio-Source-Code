/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlUtility
{
	public class BindingLoader
	{
		private string _member;
		private string _prop;
		private string _source;
		public BindingLoader(string member, string property, string sourceName)
		{
			_member = member;
			_prop = property;
			_source = sourceName;
		}
		public string Member
		{
			get
			{
				return _member;
			}
		}
		public string Property
		{
			get
			{
				return _prop;
			}
		}
		public string SourceName
		{
			get
			{
				return _source;
			}
		}
	}
}
