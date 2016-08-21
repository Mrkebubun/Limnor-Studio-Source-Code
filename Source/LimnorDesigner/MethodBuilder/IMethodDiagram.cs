/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// implemented by MethodDiagramViewer
	/// </summary>
	public interface IMethodDiagram
	{
		XmlNode RootXmlNode { get; }
		void OnActionNameChanged(string newActionName, UInt64 WholeActionId);
		void ReloadActions(BranchList actions);
	}
}
