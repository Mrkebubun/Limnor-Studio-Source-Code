/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Net
{
	public class FtpFileInfo
	{
		private string _name;
		private long _size;
		private DateTime _time;
		public FtpFileInfo(string name, long size, DateTime time)
		{
			_name = name;
			_size = size;
			_time = time;
		}
		public string Filename
		{
			get
			{
				return _name;
			}
		}
		public long FileSize
		{
			get
			{
				return _size;
			}
		}
		public DateTime FileTime
		{
			get
			{
				return _time;
			}
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1} {2}", _name, _size, _time);
		}
	}
}
