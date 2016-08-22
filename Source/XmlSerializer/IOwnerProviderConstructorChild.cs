/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSerializer
{
	public interface IOwnerProviderConstructorChild
	{
		object GetChildConstructorOwner();
	}
	public class LookupOwnerStatckOnReadAttribute : Attribute
	{
		public LookupOwnerStatckOnReadAttribute()
		{
		}
	}
}
