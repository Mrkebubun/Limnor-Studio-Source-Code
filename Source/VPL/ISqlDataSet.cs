/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public interface ISqlDataSet
	{
		bool Query();
		/// <summary>
		/// for creating visual display before loading data from database
		/// </summary>
		void CreateDataTable();
	}
}
