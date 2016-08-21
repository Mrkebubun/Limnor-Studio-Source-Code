/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;

namespace LimnorDatabase
{
	/// <summary>
	/// change default collection editor
	/// </summary>
	public class HideUITypeEditor : UITypeEditor
	{
		public HideUITypeEditor()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.None;
		}
	}
}
