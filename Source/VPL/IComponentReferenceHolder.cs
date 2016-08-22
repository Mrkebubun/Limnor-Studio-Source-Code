/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace VPL
{
	/// <summary>
	/// a component may hold references to other components. Such a component is an IComponentReferenceHolder.
	/// </summary>
	public interface IComponentReferenceHolder : IComponent
	{
		/// <summary>
		/// components are referenced by names. call this function to find the components by names
		/// </summary>
		/// <param name="components"></param>
		void ResolveComponentReferences(object[] components);
		/// <summary>
		/// set all components
		/// </summary>
		/// <param name="components">all components</param>
		void SetComponentReferences(object[] components);
		/// <summary>
		/// get: create name array from components
		/// set: saves component names
		/// </summary>
		string[] ComponentReferenceNames { get; set; }
		/// <summary>
		/// component names are used by compilers to get components for calling SetComponentReferences.
		/// </summary>
		/// <returns>component names saved</returns>
		string[] GetComponentReferenceNames();
	}
}
