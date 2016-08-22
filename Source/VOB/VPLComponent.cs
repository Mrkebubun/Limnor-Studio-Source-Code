/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.ComponentModel.Design;

namespace VOB
{
	[Designer(typeof(VPComponentPromptDesigner), typeof(IRootDesigner))]
	public partial class VPComponent : Component
	{
		public VPComponent()
		{
			InitializeComponent();
		}

		public VPComponent(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
	}
}
