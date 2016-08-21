/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LimnorDatabase
{
	public interface IFieldListHolder : IFieldsHolder
	{
		FieldList Fields { get; }
		ConnectionItem DatabaseConnection { get; set; }
	}
	public interface IFieldsHolder
	{
		string[] GetFieldNames();
	}
}
