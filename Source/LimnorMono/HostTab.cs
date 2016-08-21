/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SolutionMan;
using XHost;
using LimnorDesigner;
using System.Globalization;
using LimnorDesigner.Web;
using System.IO;

namespace LimnorVOB
{
	class HostTab : TabPage
	{
		private NodeObjectComponent _componentData;
		private ILimnorDesignPane _xhd;
		public HostTab(ILimnorDesignPane xhd, NodeObjectComponent data)
		{
			_componentData = data;
			_xhd = xhd;
			OnInit();
		}
		protected virtual void OnInit()
		{
			Controls.Clear();
			Controls.Add(_xhd.Window as Control);
			Text = TabTitle;
		}
		protected virtual string TabTitle
		{
			get
			{
				return Path.GetFileNameWithoutExtension(_componentData.Filename);
			}
		}
		public NodeObjectComponent NodeData
		{
			get
			{
				return _componentData;
			}
		}
		public UInt32 ClassId
		{
			get
			{
				return _componentData.Class.ComponentId;
			}
		}
		public bool Changed
		{
			get
			{
				return _componentData.Dirty;
			}
		}
		public ILimnorDesignPane GetHost()
		{
			return _xhd;
		}
		public MultiPanes GetHostControl()
		{
			if (_xhd == null)
				return null;
			return _xhd.PaneHolder;
		}
		public void ResetTitle()
		{
			if (_componentData.Dirty)
			{
				Text = string.Format(CultureInfo.InvariantCulture, "{0}*", TabTitle);
			}
			else
			{
				Text = TabTitle;
			}
		}
	}
}
