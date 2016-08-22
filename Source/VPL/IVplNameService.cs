/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;

namespace VPL
{
	public interface IVplNameService : INameCreationService
	{
		/// <summary>
		/// the type for the component
		/// </summary>
		Type ComponentType { get; set; }
	}
}
