/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;

namespace VPL
{
	public class SetupContentsHolder : UserControl
	{
		public virtual bool LoadData(XmlNode node, string filename) { return false; }
		public virtual void OnBeforeSave() { }
		public virtual void SetPropertyGrid(PropertyGrid pg) { }
		public event EventHandler PropertyChanged;
		protected void OnPropertyChanged()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, EventArgs.Empty);
			}
		}
		public virtual void Compile() { }
		public static SetupContentsHolder CreateSetupContentsHolder()
		{
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "LimnorWix*.dll");
			if (files != null && files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					try
					{
						Assembly a = Assembly.LoadFile(files[i]);
						if (a != null)
						{
							Type[] tps = a.GetExportedTypes();
							if (tps != null && tps.Length > 0)
							{
								for (int k = 0; k < tps.Length; k++)
								{
									if (!tps[k].IsInterface)
									{
										if (typeof(SetupContentsHolder).IsAssignableFrom(tps[k]))
										{
											SetupContentsHolder dis = Activator.CreateInstance(tps[k]) as SetupContentsHolder;
											if (dis != null)
											{
												return dis;
											}
										}
									}
								}
							}
							MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, "SetupContentsHolder not found in {0}", files[i]), "Load Setup Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					catch (Exception err)
					{
						MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error loading {0}. {1}", files[i], err.Message), "Load Setup Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			else
			{
				MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Installer (LimnorWix*.dll) not found in [{0}].", AppDomain.CurrentDomain.BaseDirectory), "Load Setup Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return null;
		}
	}
}
