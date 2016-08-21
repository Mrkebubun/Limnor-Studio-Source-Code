/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDesigner.MethodBuilder
{
	public class TopActionGroupDesignerHolder : MethodDesignerHolder
	{
		public TopActionGroupDesignerHolder(ILimnorDesignerLoader designer)
			: base(designer, 0)
		{
		}
		protected override Type ViewerType
		{
			get
			{
				return typeof(TopActionGroupDiagramViewer);
			}
		}
	}

}
