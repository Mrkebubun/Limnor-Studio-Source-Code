/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Data;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace VOB
{
	/// <summary>
	/// Uses the custom RootDesigner (NewComponentPrompt)
	/// </summary>
	[Designer(typeof(NewComponentPromptDesigner), typeof(IRootDesigner))]
	[Designer(typeof(ComponentDesigner))]
	public class NewComponentPrompt : Component
	{
		public NewComponentPrompt()
		{
		}
	}

}
