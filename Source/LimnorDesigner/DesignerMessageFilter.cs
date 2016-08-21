/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace LimnorDesigner
{
	public class DesignerMessageFilter : IMessageFilter
	{
		#region fields and constructors
		private DesignSurface surface;
		public DesignerMessageFilter(DesignSurface designSurface)
		{
			surface = designSurface;
		}
		#endregion

		#region IMessageFilter Members

		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_KEYUP = 0x101;

			Control c = surface.View as Control;
			if (c != null && (m.Msg >= WM_KEYDOWN && m.Msg <= WM_KEYUP))
			{
			}
			return false;
		}

		#endregion
	}
}
