/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using MathExp;
using XmlUtility;

namespace LimnorVOB
{
	public partial class dlgEditLibrary : Form
	{
		Type _libType;
		string _configFile;
		public dlgEditLibrary()
		{
			InitializeComponent();
		}
		public void LoadData(string title, string filename, Type type)
		{
			lblTitle.Text = title;
			_configFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), filename);
			_libType = type;
			if (System.IO.File.Exists(filename))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				if (doc.DocumentElement != null)
				{
					for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
					{
						try
						{
							Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement.ChildNodes[i]);
							if (t != null)
							{
								listBox1.Items.Add(t);
							}
						}
						catch (Exception er)
						{
							listBox1.Items.Add("Loading type failed. " + er.Message);
						}
					}
				}
			}
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				listBox1.Items.RemoveAt(listBox1.SelectedIndex);
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			XmlDocument doc = new XmlDocument();
			XmlNode nodeRoot = doc.CreateElement("Types");
			doc.AppendChild(nodeRoot);
			List<Type> types = new List<Type>();
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				Type t = listBox1.Items[i] as Type;
				if (t != null)
				{
					if (!types.Contains(t))
					{
						types.Add(t);
						XmlNode node = doc.CreateElement(XmlSerialization.XML_TYPE);
						nodeRoot.AppendChild(node);
						XmlUtil.SetLibTypeAttribute(node, t);
					}
				}
			}
			doc.Save(_configFile);
			this.DialogResult = DialogResult.OK;
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = ".NET DLL|*.dll";
			dlg.Title = "Select library file";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				string sDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
				try
				{
					bool bCopy = true;
					string sTarget = System.IO.Path.Combine(sDir, System.IO.Path.GetFileName(dlg.FileName));
					if (System.IO.File.Exists(sTarget))
					{
						if (sTarget.ToLower() != dlg.FileName.ToLower())
						{
							bCopy = (MessageBox.Show("Do you want to overwrite existing file " + sTarget + "?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
						}
						else
						{
							bCopy = false;
						}
					}
					if (bCopy)
					{
						System.IO.File.Copy(dlg.FileName, sTarget);
					}
					Assembly a = Assembly.LoadFile(sTarget);
					Type[] types = a.GetExportedTypes();
					if (types != null && types.Length > 0)
					{
						for (int i = 0; i < types.Length; i++)
						{
							if (!types[i].IsAbstract && types[i].IsPublic)
							{
								if (types[i].IsSubclassOf(_libType))
								{
									listBox1.Items.Add(types[i]);
								}
							}
						}
					}
				}
				catch (Exception er)
				{
					MessageBox.Show(er.Message + "\r\nIf the file is in use then close all applications, including this application, and delete the file from " + sDir + " and try it again.\r\nIf the library needs supporting DLL files then copy them to " + sDir + " and try it again.");
				}
			}
		}
	}
}