/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LimnorDesigner.ResourcesManager;
using System.Globalization;
using System.Runtime.InteropServices;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner
{
	public partial class FormLanguageIcon : Form
	{
		int x0 = 0;
		int y0 = 0;
		ProjectResources _resman;
		Control _parent;
		public FormLanguageIcon()
		{
			InitializeComponent();

		}
		public void CreateContextmenu()
		{
			ContextMenu mnu = new ContextMenu();
			mnu.MenuItems.Add(new MenuItemWithBitmap("Edit resources", mnu_editresources, Resources.resx.ToBitmap()));
			//
			if (_resman.Languages.Count > 0)
			{
				mnu.MenuItems.Add(new MenuItem("-"));
				MenuItemWithBitmap m = new MenuItemWithBitmap("default culture", mnu_changeCulture, TreeViewObjectExplorer.GetLangaugeBitmapByName(string.Empty));
				m.Tag = "";
				mnu.MenuItems.Add(m);
				foreach (string s in _resman.Languages)
				{
					if (string.CompareOrdinal(s, "zh") == 0)
					{
						m = new MenuItemWithBitmap("中文 zh", mnu_changeCulture, TreeViewObjectExplorer.GetLangaugeBitmapByName(s));
						m.Tag = s;
						mnu.MenuItems.Add(m);
					}
					else
					{
						CultureInfo c = CultureInfo.GetCultureInfo(s);
						if (c != null)
						{
							m = new MenuItemWithBitmap(ProjectResources.CultureDisplay(c), mnu_changeCulture, TreeViewObjectExplorer.GetLangaugeBitmapByName(s));
							m.Tag = s;
							mnu.MenuItems.Add(m);
						}
					}
				}
			}
			//
			this.ContextMenu = mnu;
			lblLang.ContextMenu = mnu;
			picLang.ContextMenu = mnu;
		}
		private void mnu_changeCulture(object sender, EventArgs e)
		{
			string curLanguageName = _resman.DesignerLanguageName;
			MenuItem m = (MenuItem)sender;
			_resman.DesignerLanguageName = (string)(m.Tag);
			OnLanguageChanged();
			if (string.CompareOrdinal(curLanguageName, _resman.DesignerLanguageName) != 0)
			{
				_resman.NotifyLanguageChange();
			}
		}
		private void mnu_editresources(object sender, EventArgs e)
		{
			DlgResMan.EditResources(this, _resman.Project);
			CreateContextmenu();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (this.ClientSize.Width > 3)
			{
				e.Graphics.DrawRectangle(Pens.Green, new Rectangle(this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2));
				e.Graphics.DrawLine(Pens.Black, new Point(this.ClientRectangle.Right - 1, this.ClientRectangle.Top + 2), new Point(this.ClientRectangle.Right - 1, this.ClientRectangle.Bottom - 1));
				e.Graphics.DrawLine(Pens.Black, new Point(this.ClientRectangle.Left + 2, this.ClientRectangle.Bottom - 1), new Point(this.ClientRectangle.Right - 1, this.ClientRectangle.Bottom - 1));

			}
		}
		public void LoadData(ProjectResources resources, Control p)
		{
			_parent = p;
			_resman = resources;
			OnLanguageChanged();
			CreateContextmenu();
		}
		public void OnLanguageChanged()
		{
			if (string.IsNullOrEmpty(_resman.DesignerLanguageName))
			{
				this.Hide();
			}
			else
			{
				this.Show();
			}
			picLang.Image = TreeViewObjectExplorer.GetLangaugeBitmapByName(_resman.DesignerLanguageName);
			lblLang.Text = _resman.DesingerCultureDisplay;
			this.Width = lblLang.Left + lblLang.Width + 3;
			this.Refresh();
		}
		int x1 = 0;
		int y1 = 0;
		int w1 = 800;
		int h1 = 300;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			x0 = e.X;
			y0 = e.Y;
			if (_parent != null)
			{
				Point p = _parent.PointToScreen(_parent.Location);
				x1 = p.X;
				y1 = p.Y;
				w1 = _parent.Width;
				h1 = _parent.Height;
			}
			if (e.Button == MouseButtons.Right)
			{
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left)
			{
				if (e.X != x0 || e.Y != y0)
				{
					Point p = new Point(this.Location.X + e.X - x0 - x1, this.Location.Y + e.Y - y0 - y1);
					if (p.X > 0 && p.Y > 0)
					{
						if (p.X + this.Width < w1 && p.Y + this.Height < h1)
						{
							this.Location = p;
						}
					}
				}
			}
		}

		private void lblLang_MouseDown(object sender, MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		private void lblLang_MouseMove(object sender, MouseEventArgs e)
		{
			OnMouseMove(e);
		}

		private void picLang_MouseDown(object sender, MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		private void picLang_MouseMove(object sender, MouseEventArgs e)
		{
			OnMouseMove(e);
		}
	}
}
