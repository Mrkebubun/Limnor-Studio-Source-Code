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
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;

namespace MathExp
{
	public class NameCreation : INameCreationService
	{
		public NameCreation()
		{
		}
		protected virtual bool IsNameUsed(string name)
		{
			return false;
		}
		public string CreateName(IContainer container, string baseName)
		{
			string s = "";
			int n = 0;
			bool b = true;
			while (b)
			{
				b = false;
				n++;
				s = baseName + n.ToString();
				for (int i = 0; i < container.Components.Count; i++)
				{
					if (s.Equals(container.Components[i].Site.Name, StringComparison.OrdinalIgnoreCase))
					{
						b = true;
						break;
					}
				}
				if (!b)
				{
					b = IsNameUsed(s);
				}
			}
			return s;
		}
		public virtual string FormValidName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return "name1";
			System.Text.StringBuilder sb = new StringBuilder();
			if (name[0] == '_' || (name[0] >= 'a' && name[0] <= 'z') || (name[0] >= 'A' && name[0] <= 'Z'))
				sb.Append(name[0]);
			else
				sb.Append("_");
			for (int i = 1; i < name.Length; i++)
			{
				if (name[i] == '_' || (name[i] >= 'a' && name[i] <= 'z') || (name[i] >= 'A' && name[i] <= 'Z')
					|| (name[i] >= '0' && name[i] <= '9')
					)
				{
					sb.Append(name[i]);
				}
				else
				{
					sb.Append("_");
				}
			}
			return sb.ToString();
		}
		#region INameCreationService Members
		// Creates an identifier for a particular data type that does not conflict 
		// with the identifiers of any components in the specified collection.
		public string CreateName(IContainer container, System.Type dataType)
		{
			// Create a basic type name string.
			string baseName = dataType.Name;
			int uniqueID = 1;

			bool unique = false;
			string name = "";
			// Continue to increment uniqueID numeral until a 
			// unique ID is located.
			while (!unique)
			{
				unique = true;
				name = baseName + uniqueID.ToString();
				// Check each component in the container for a matching 
				// base type name and unique ID.
				for (int i = 0; i < container.Components.Count; i++)
				{
					// Check component name for match with unique ID string.
					if (container.Components[i].Site.Name.StartsWith(name))
					{
						// If a match is encountered, set flag to recycle 
						// collection, increment ID numeral, and restart.
						unique = false;
						uniqueID++;
						break;
					}
				}
				if (unique)
				{
					unique = !IsNameUsed(name);
				}
			}

			return name;
		}

		public virtual bool IsValidName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;
			if (name[0] == '_' || (name[0] >= 'a' && name[0] <= 'z') || (name[0] >= 'A' && name[0] <= 'Z'))
			{
				for (int i = 1; i < name.Length; i++)
				{
					if (!(name[i] == '_' || (name[i] >= 'a' && name[i] <= 'z') || (name[i] >= 'A' && name[i] <= 'Z')
						|| (name[i] >= '0' && name[i] <= '9')
						))
					{
						return false;
					}
				}
				if (IsNameUsed(name))
					return false;
				return true;
			}
			return false;
		}

		public virtual void ValidateName(string name)
		{
			if (!IsValidName(name))
			{
				if (string.IsNullOrEmpty(name))
					throw new MathException("name cannot be empty");
				else
					throw new MathException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "The name {0} is invalid or is in use.", name));
			}
		}

		#endregion
	}
}
