/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
	public interface IForm
	{
		void AllowFocus();
		void NotAllowFocus();
		bool IsDisposed { get; }
		System.Windows.Forms.Control.ControlCollection Controls { get; }
		void AdjustImageUISelector(Control c, PropertyGrid pg);
	}
	public interface INoFocus
	{
		bool NoFocus();
	}
}
