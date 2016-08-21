/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for MenuWithID.
	/// </summary>
	public class MenuWithID : System.Windows.Forms.MenuItem
	{
		public Guid ID = Guid.Empty;
		public EnumPageUnit Unit;
		public MenuWithID(string text)
			: base(text)
		{
		}
		public MenuWithID()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
