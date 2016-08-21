/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathExp;
using VSPrj;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public partial class ComponentIconsHolder : UserControl, IIconHolder, IWithProject
	{
		#region fields and constructors
		private MethodDesignerHolder _viewer;
		const int ICON_Width = 64;
		const int ICON_Height = 64;
		const int Off_X = 10;
		const int Off_Y = 10;
		public ComponentIconsHolder()
		{
			InitializeComponent();
		}
		#endregion
		#region Properties
		public List<ComponentIcon> ComponentIcons
		{
			get
			{
				List<ComponentIcon> icons = new List<ComponentIcon>();
				foreach (Control c in Controls)
				{
					ComponentIcon ic = c as ComponentIcon;
					if (ic != null)
					{
						icons.Add(ic);
					}
				}
				return icons;
			}
		}
		#endregion
		#region Methods
		public void SaveIconLocations()
		{
			foreach (Control c in Controls)
			{
				ActiveDrawing ad = c as ActiveDrawing;
				if (ad != null)
				{
					ad.SaveLocation();
				}
				RelativeDrawing rd = c as RelativeDrawing;
				if (rd != null)
				{
					rd.SaveRelativePosition();
				}
			}
		}
		public void SetMethodViewer(MethodDesignerHolder viewer)
		{
			_viewer = viewer;
		}

		public void PrepareNewIconLocation(Point loc)
		{
			int d = 10;
			int x = 0;
			int y = 0;
			foreach (Control c in Controls)
			{
				ComponentIcon ic = c as ComponentIcon;
				if (ic != null)
				{
					if (ic.Location.X == loc.X && ic.Location.Y == loc.Y)
					{
						x += d;
						if (x > 300)
						{
							x = d;
							y += d;
						}
						ic.Location = new Point(loc.X + x, loc.Y + y);
					}
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
			{
				ContextMenu mnu = new ContextMenu();
				MenuItem mi;
				if (_viewer.ReadOnly)
				{
				}
				else
				{
					mi = new MenuItemWithBitmap("Add local variable", Resources._obj.ToBitmap());
					mi.Click += new EventHandler(miNewObject_Click);
					mi.Tag = new Point(e.X, e.Y);
					mnu.MenuItems.Add(mi);
					//
					mi = new MenuItemWithBitmap("Arrange Icons By", Resources.grid);
					mi.Tag = new Point(e.X, e.Y);
					mnu.MenuItems.Add(mi);
					MenuItem msi = new MenuItem("Name", miArrangeByName_Click);
					mi.MenuItems.Add(msi);
					msi = new MenuItem("Type", miArrangeByType_Click);
					mi.MenuItems.Add(msi);
				}
				if (mnu.MenuItems.Count > 0)
				{
					mnu.Show(this, new Point(e.X, e.Y));
				}
			}
		}
		#endregion
		#region private methods
		private void arrangeIcon(SortedList<string, ComponentIcon> ics)
		{
			this.AutoScrollPosition = new Point(0, 0);
			int lx = (ICON_Width - ComponentIcon.ICONSIZE) / 2;
			int c = 0, r = 0, x = Off_X + lx, y = Off_Y;
			int colCount = (int)Math.Sqrt((double)(ics.Count));
			foreach (KeyValuePair<string, ComponentIcon> ic in ics)
			{
				ic.Value.Location = new Point(x + this.AutoScrollPosition.X, y + this.AutoScrollPosition.Y);
				ic.Value.ResetLabelPosition();
				c++;
				x += ICON_Width;
				if (c >= colCount)
				{
					c = 0;
					r++;
					x = Off_X + lx;
					y += ICON_Height;
				}
			}
		}
		private void miArrangeByName_Click(object sender, EventArgs e)
		{
			SortedList<string, ComponentIcon> ics = new SortedList<string, ComponentIcon>();
			foreach (Control co in Controls)
			{
				ComponentIcon ic = co as ComponentIcon;
				if (ic != null)
				{
					if (ics.ContainsKey(ic.ClassPointer.DisplayName))
					{
						ics.Add(ic.ClassPointer.DisplayName + " " + Guid.NewGuid().ToString(), ic);
					}
					else
					{
						ics.Add(ic.ClassPointer.DisplayName, ic);
					}
				}
			}
			arrangeIcon(ics);
		}
		private void miArrangeByType_Click(object sender, EventArgs e)
		{
			SortedList<string, ComponentIcon> ics = new SortedList<string, ComponentIcon>();
			foreach (Control co in Controls)
			{
				ComponentIcon ic = co as ComponentIcon;
				if (ic != null)
				{
					if (ics.ContainsKey(ic.ClassPointer.ObjectType.Name))
					{
						ics.Add(ic.ClassPointer.ObjectType.Name + " " + Guid.NewGuid().ToString(), ic);
					}
					else
					{
						ics.Add(ic.ClassPointer.ObjectType.Name, ic);
					}
				}
			}
			arrangeIcon(ics);
		}
		private void miNewObject_Click(object sender, EventArgs e)
		{
			Point loc = (Point)(((MenuItem)sender).Tag);
			_viewer.GetCurrentViewer().CreateLocalVariable(loc);
		}
		#endregion
		#region IMethodDesignerHolder Members

		public MethodDesignerHolder MethodViewer
		{
			get { return _viewer; }
		}

		#endregion
		#region IIconHolder Members

		public void ClearIconSelection()
		{
			foreach (Control c in Controls)
			{
				ComponentIcon ic = c as ComponentIcon;
				if (ic != null)
				{
					if (ic.IsSelected)
					{
						ic.IsSelected = false;
						ic.Invalidate();
					}
				}
			}
		}

		public void SetIconSelection(ComponentIcon icon)
		{
			foreach (Control c in Controls)
			{
				ComponentIcon ic = c as ComponentIcon;
				if (ic != null)
				{
					if (ic == icon)
					{
						if (!ic.IsSelected)
						{
							ic.IsSelected = true;
							ic.Invalidate();
							_viewer.SetIconSelection(ic);
						}
					}
					else
					{
						if (ic.IsSelected)
						{
							ic.IsSelected = false;
							ic.Invalidate();
						}
					}
				}
			}
		}

		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get { return _viewer.Project; }
		}

		#endregion
	}
}
