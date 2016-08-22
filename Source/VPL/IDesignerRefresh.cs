/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VPL
{
	public interface IDesignerRefresh
	{
		void OnPropertyChangedInDesigner(string name);
	}
	public interface IDesignPane
	{
		void LoadProjectToolbox();
		IComponent CreateComponent(Type type, string name);
	}
}
