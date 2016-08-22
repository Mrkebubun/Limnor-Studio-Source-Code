/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Drawing;
using System.ComponentModel.Design;

namespace VPL
{
	public interface ILimnorToolbox : IToolboxService
	{
		void ShowToolbox();
		void HideToolbox();
		void RemoveTab(string tab);
		void AddTab(string tab);
		void AddTab(string tab, bool readOnly, bool persist, int idx, bool clearItems);
		void AdjustTabSizes();
		void AdjustTabSize(string tab);
		void AdjustTabSize(Guid projectGuid);
		void ResetProjectTab(Guid projectGuid);
		void DataUsed();
		void SetChanged(bool changed);
		void AddToolboxItem(string tab, Type tp, string name, Bitmap img, Dictionary<string, object> properties);
		void AddToolboxItem(string tab, Guid projectGuid, Type tp, string name, Bitmap img, Dictionary<string, object> properties);
		void ShowProjectTab(Guid projectGuid);
		void RefreshTabs();
		void ReqestHideToolbox();
		IDesignerHost Host { get; set; }

	}
}
