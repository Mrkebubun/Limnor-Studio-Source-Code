/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
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

namespace Limnor.Drawing2D
{
	public partial class FormToolbox : Form
	{
		public FormToolbox()
		{
			InitializeComponent();
			TopLevel = false;
			Type[] tpsD = this.GetType().Assembly.GetExportedTypes();
			for (int i = 0; i < tpsD.Length; i++)
			{
				if (!tpsD[i].IsAbstract)
				{
					if (tpsD[i].IsSubclassOf(typeof(DrawingItem)))
					{
						listBox1.AddDrawItem(tpsD[i]);
					}
				}
			}
			//
			//load additional drawings
			string[] ss = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "drawing*.dll");
			if (ss != null)
			{
				for (int i = 0; i < ss.Length; i++)
				{
					System.Reflection.Assembly a = null;
					try
					{
						a = System.Reflection.Assembly.LoadFile(ss[i]);
						if (a != null)
						{
							System.Type[] tps = a.GetExportedTypes();
							if (tps != null)
							{
								for (int j = 0; j < tps.Length; j++)
								{
									if (!tps[j].IsAbstract && tps[j].IsSubclassOf(typeof(DrawingItem)))
									{
										listBox1.AddDrawItem(tps[j]);
									}
								}
							}
						}
					}
					catch
					{
					}
				}
			}
		}
		public void ClearToolboxSelection()
		{
			listBox1.SelectedIndex = -1;
		}
		public void UseSubset(Type[] types)
		{
			listBox1.UseSubset(types);
		}
		public Type SelectedToolboxItem
		{
			get
			{
				int n = listBox1.SelectedIndex;
				if (n >= 0 && n < listBox1.Items.Count)
				{
					DrawItemListItem item = listBox1.Items[n] as DrawItemListItem;
					if (item != null)
					{
						return item.ItemType;
					}
				}
				return null;
			}
		}
	}
}
