/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// for MethodAction, method as a single action, no need to create an action from it, no parameters,may have condition
	/// </summary>
	public class TopActionGroupDiagramViewer : MethodDiagramViewer
	{
		public TopActionGroupDiagramViewer()
		{
		}
		[Browsable(false)]
		public override IActionsHolder ActionsHolder
		{
			get
			{
				return RootClass;
			}
		}
	}
}
