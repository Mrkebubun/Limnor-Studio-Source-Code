/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;

namespace LimnorDatabase
{
	/// <summary>
	/// provide fields mapping
	/// </summary>
	public class DataBind : ICloneable
	{
		/// <summary>
		/// data to be updated
		/// </summary>
		public string[] SourceFields = null; //for picking source field when used as DataLink property. came from DataBind property
		public StringMapList AdditionalJoins = null;
		public DataBind()
		{
		}
		public bool IsSourceValid(string name)
		{
			if (SourceFields != null)
			{
				for (int i = 0; i < SourceFields.Length; i++)
				{
					if (string.Compare(SourceFields[i], name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// From field to be updated find the source field
		/// </summary>
		/// <param name="name">field to be updated</param>
		/// <returns>source field</returns>
		public string GetMappedField(string name)
		{
			if (AdditionalJoins != null)
			{
				for (int i = 0; i < AdditionalJoins.Count; i++)
				{
					if (string.Compare(AdditionalJoins[i].Target, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return AdditionalJoins[i].Source;
					}
				}
			}
			return "";
		}
		public void ClearFieldMaps()
		{
			AdditionalJoins = null;
		}
		public void AddFieldMap(string s1, string s2)
		{
			if (AdditionalJoins == null)
			{
				AdditionalJoins = new StringMapList();
			}
			AdditionalJoins.AddFieldMap(s1, s2);
		}
		public int DestinationFieldCount
		{
			get
			{
				if (AdditionalJoins == null)
				{
					return 0;
				}
				return AdditionalJoins.Count;
			}
		}
		public int SourceFieldCount
		{
			get
			{
				if (SourceFields == null)
					return 0;
				return SourceFields.Length;
			}
		}
		public bool UseAdditionalJoins
		{
			get
			{
				if (SourceFields != null)
				{
					if (SourceFields.Length > 0)
						return true;
				}
				return false;
			}
		}
		public override string ToString()
		{
			return "Database lookup";
		}
		#region ICloneable Members

		public virtual object Clone()
		{
			DataBind obj = new DataBind();
			if (AdditionalJoins != null)
				obj.AdditionalJoins = (StringMapList)AdditionalJoins.Clone();
			if (SourceFields != null)
			{
				obj.SourceFields = new string[SourceFields.Length];
				for (int i = 0; i < SourceFields.Length; i++)
				{
					obj.SourceFields[i] = SourceFields[i];
				}
			}
			return obj;
		}

		#endregion
	}

}
