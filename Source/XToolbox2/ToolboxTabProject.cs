/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace XToolbox2
{
	public class ToolboxTabProject : ToolboxTab2
	{
		private Guid _prjGuid;
		public ToolboxTabProject(ToolboxPane2 owner, string name, Guid projectGuid)
			: base(owner, string.Format(CultureInfo.InvariantCulture, "p{0}", projectGuid.GetHashCode().ToString("x", CultureInfo.InvariantCulture)), 0, true)
		{
			_prjGuid = projectGuid;
			SetTitle(name);
		}
		public Guid ProjectGuid
		{
			get
			{
				return _prjGuid;
			}
		}
		public override bool Persist
		{
			get
			{
				return false;
			}
			set
			{

			}
		}
	}
}
