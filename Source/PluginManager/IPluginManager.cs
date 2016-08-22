/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Plugin Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorVisualProgramming
{
	public interface IPluginManager
	{
		string PluginConfigurationFoldername { get; }
		string PluginConfigurationFileFullpath { get; }
		IList<string> PluginDllFiles { get; }
		void RefreshPlugins();
		void OnNotifyDataChanged(IPlugin sender, EventArgsDataName data);
	}
}
