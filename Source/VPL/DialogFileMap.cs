/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
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
using System.Globalization;
using System.IO;
using System.Xml;

namespace VPL
{
	public partial class DialogFileMap : Form
	{
		public string ReplaceFile;
		private static Dictionary<string, string> _filemaps;
		public DialogFileMap()
		{
			InitializeComponent();
		}
		public static Dictionary<string, string> FileMappingList
		{
			get
			{
				return _filemaps;
			}
		}
		const string XML_Item = "Item";
		const string XMLATT_Origin = "origin";
		const string XMLATT_MapTo = "mapto";
		public static void SaveFileMappings(string projectFile)
		{
			if (_filemaps != null && _filemaps.Count > 0)
			{
				string mapingFile = string.Format(CultureInfo.InvariantCulture, "{0}.filemapping", projectFile);
				if (File.Exists(mapingFile))
				{
					FileInfo fi = new FileInfo(mapingFile);
					fi.Attributes = FileAttributes.Normal;
					fi.Delete();
				}
				XmlDocument doc = new XmlDocument();
				XmlNode root = doc.CreateElement("Maps");
				doc.AppendChild(root);
				foreach (KeyValuePair<string, string> kv in _filemaps)
				{
					XmlNode node = doc.CreateElement(XML_Item);
					root.AppendChild(node);
					XmlAttribute xa;
					xa = node.OwnerDocument.CreateAttribute(XMLATT_Origin);
					node.Attributes.Append(xa);
					xa.Value = kv.Key;
					xa = node.OwnerDocument.CreateAttribute(XMLATT_MapTo);
					node.Attributes.Append(xa);
					xa.Value = kv.Value;
				}
				doc.Save(mapingFile);
			}
		}
		public static void LoadFileMappings(string projectFile)
		{
			if (_filemaps == null)
			{
				_filemaps = new Dictionary<string, string>();
			}
			string mapingFile = string.Format(CultureInfo.InvariantCulture, "{0}.filemapping", projectFile);
			if (File.Exists(mapingFile))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(mapingFile);
				if (doc.DocumentElement != null)
				{
					XmlNodeList nodes = doc.DocumentElement.SelectNodes(XML_Item);
					foreach (XmlNode nd in nodes)
					{
						if (nd.Attributes != null)
						{
							XmlAttribute xa = nd.Attributes[XMLATT_Origin];
							if (xa != null && !string.IsNullOrEmpty(xa.Value))
							{
								string origin = xa.Value.Trim().ToLowerInvariant();
								if (origin.Length > 0)
								{
									if (!_filemaps.ContainsKey(origin))
									{
										xa = nd.Attributes[XMLATT_MapTo];
										if (xa != null && !string.IsNullOrEmpty(xa.Value))
										{
											string mapto = xa.Value.Trim().ToLowerInvariant();
											if (mapto.Length > 0)
											{
												if (File.Exists(mapto))
												{
													_filemaps.Add(origin, mapto);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		public void LoadData(string invalidFile)
		{
			lblInvalidFile.Text = invalidFile;
			if (!string.IsNullOrEmpty(invalidFile))
			{
				if (_filemaps != null)
				{
					string s;
					string s0 = invalidFile.ToLowerInvariant();
					if (_filemaps.TryGetValue(s0, out s))
					{
						ReplaceFile = s;
						txtMapTo.Text = s;
					}
				}
			}
		}
		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (!string.IsNullOrEmpty(lblInvalidFile.Text))
			{
				try
				{
					dlg.Title = string.Format(CultureInfo.InvariantCulture, "Find a file to replace [{0}]", Path.GetFileName(lblInvalidFile.Text));
				}
				catch
				{
					dlg.Title = string.Format(CultureInfo.InvariantCulture, "Find a file to replace [{0}]", lblInvalidFile.Text);
				}
			}
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				txtMapTo.Text = dlg.FileName;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			string s = txtMapTo.Text.Trim();
			if (s.Length == 0)
			{
				MessageBox.Show(this, "Replace file cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				try
				{
					if (!File.Exists(s))
					{
						MessageBox.Show(this, "Replace file cannot be found", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						ReplaceFile = s;
						if (_filemaps == null)
						{
							_filemaps = new Dictionary<string, string>();
						}
						string s0 = lblInvalidFile.Text.ToLowerInvariant();
						if (!string.IsNullOrEmpty(s0) && !_filemaps.ContainsKey(s0))
						{
							_filemaps.Add(s0, s);
						}
						this.DialogResult = DialogResult.OK;
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
