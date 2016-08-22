/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	/// <summary>
	/// IDE uses it to return project database connection guid list.
	/// Implementation: A static function of LimnorProject, use active project to get the list
	/// Database module uses it to get project database connection guid list
	/// </summary>
	/// <param name="listOwner">i.e. current project name</param>
	/// <returns></returns>
	public delegate IList<Guid> GetObjectGuidList(out object listOwner);

	/// <summary>
	/// update project database connection guid list
	/// </summary>
	/// <param name="list"></param>
	public delegate void SetObjectGuidList(IList<Guid> list, object listOwner);
}
