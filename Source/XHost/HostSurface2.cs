/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Collections;
using LimnorDesigner;

namespace XHost
{
	public class HostSurface2 : DesignSurface
	{
		private ISelectionService _selectionService;
		public HostSurface2()
			: base()
		{
		}
		public HostSurface2(IServiceProvider parentProvider)
			: base(parentProvider)
		{
		}
		public void Initialize()
		{
			_selectionService = (ISelectionService)(this.ServiceContainer.GetService(typeof(ISelectionService)));
			_selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
		}
		private void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			if (_selectionService != null)
			{
				ICollection selectedComponents = _selectionService.GetSelectedComponents();
				PropertyGrid propertyGrid = (PropertyGrid)this.GetService(typeof(PropertyGrid));

				if (propertyGrid != null)
				{
					object[] comps = new object[selectedComponents.Count];
					int i = 0;

					foreach (Object o in selectedComponents)
					{
						comps[i] = o;
						i++;
					}

					propertyGrid.SelectedObjects = comps;
				}
			}
		}

	}
	public class HostControl2 : UserControl
	{
		private HostSurface2 _hostSurface;
		public HostControl2(HostSurface2 hostSurface)
		{
			if (hostSurface == null)
				return;

			_hostSurface = hostSurface;

			Control control = _hostSurface.View as Control;

			control.Parent = this;
			control.Dock = DockStyle.Fill;
			control.Visible = true;
		}
		public DesignSurface DesignSurface
		{
			get { return _hostSurface; }
		}
		public IDesignerHost DesignerHost
		{
			get
			{
				return (IDesignerHost)_hostSurface.GetService(typeof(IDesignerHost));
			}
		}
		public HostSurface2 HostSurface
		{
			get
			{
				return _hostSurface;
			}
		}

	}
}
