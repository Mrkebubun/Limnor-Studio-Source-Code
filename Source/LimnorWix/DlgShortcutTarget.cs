/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;

namespace LimnorWix
{
	public partial class DlgShortcutTarget : Form
	{
		public string Target;
		public DlgShortcutTarget()
		{
			InitializeComponent();
		}
		private void addFiles(string dir)
		{
			if (!string.IsNullOrEmpty(dir))
			{
				if (Directory.Exists(dir))
				{
					string[] files = Directory.GetFiles(dir);
					if (files != null && files.Length > 0)
					{
						for (int i = 0; i < files.Length; i++)
						{
							listBox1.Items.Add(files[i]);
						}
					}
					string[] folders = Directory.GetDirectories(dir);
					if (folders != null && folders.Length > 0)
					{
						for (int i = 0; i < folders.Length; i++)
						{
							addFiles(folders[i]);
						}
					}
				}
			}
		}
		public void LoadData(XmlNode docnode)
		{
			XmlNodeList ns;
			ns = docnode.SelectNodes("Product/appFolder/Project");
			foreach (XmlNode nd in ns)
			{
				if (!XmlUtil.GetAttributeBoolDefFalse(nd, "removed"))
				{
					listBox1.Items.Add(string.Format(CultureInfo.InvariantCulture, "PROJECT - {0}", nd.InnerText));
				}
			}
			ns = docnode.SelectNodes("Product/appFolder/SourceFile");
			foreach (XmlNode nd in ns)
			{
				if (!XmlUtil.GetAttributeBoolDefFalse(nd, "removed"))
				{
					listBox1.Items.Add(nd.InnerText);
				}
			}
			ns = docnode.SelectNodes("Product/appFolder/SourceFolder");
			foreach (XmlNode nd in ns)
			{
				if (!XmlUtil.GetAttributeBoolDefFalse(nd, "removed"))
				{
					addFiles(nd.InnerText);
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				WixSourceFileNode f = listBox1.Items[n] as WixSourceFileNode;
				if (f != null)
					Target = f.Filename;
				else
					Target = listBox1.Text;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
		}
	}
}
