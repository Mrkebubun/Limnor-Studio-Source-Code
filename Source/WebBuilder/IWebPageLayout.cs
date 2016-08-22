/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.WebBuilder
{
	/// <summary>
	/// a class implementing this interface is a container,
	/// it is responsible for creating html for the controls it contains.
	/// not all containers need to implement it, only the ones need to handle the creation of the html for children. 
	/// </summary>
	public interface IWebPageLayout
	{
		bool FlowStyle { get; }
	}
}
