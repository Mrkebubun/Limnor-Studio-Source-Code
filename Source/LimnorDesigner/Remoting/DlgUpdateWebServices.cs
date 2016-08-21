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
using System.Xml;
using LimnorDesigner;
using XmlUtility;
using VSPrj;

namespace LimnorDesigner.Remoting
{
	public partial class DlgUpdateWebServices : Form
	{
		private List<AsmxURL> _urls;
		private string _folder;
		private string _prjFile;
		private string _namespace;
		public DlgUpdateWebServices()
		{
			InitializeComponent();
		}

		private void buttonFolder_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = "Select Limnor Studio project file";
				dlg.Filter = "Limnor Studio project|*.lrproj";
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					bool bSelected = false;
					XmlDocument docP = new XmlDocument();
					docP.PreserveWhitespace = false;
					docP.Load(dlg.FileName);
					if (docP.DocumentElement != null)
					{
						XmlNode ndNs = docP.DocumentElement.SelectSingleNode("x:PropertyGroup/x:RootNamespace", LimnorProject.GetProjectNamespace());
						if (ndNs != null)
						{
							string ns = ndNs.InnerText;
							string sVob = dlg.FileName + ".vob";
							if (System.IO.File.Exists(sVob))
							{
								XmlDocument doc = new XmlDocument();
								doc.PreserveWhitespace = false;
								doc.Load(sVob);
								if (doc.DocumentElement != null)
								{
									XmlNodeList ndList = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
										"//{0}/{1}",
										XmlTags.XML_WebServiceList, XmlTags.XML_Item));
									if (ndList != null && ndList.Count > 0)
									{
										List<AsmxURL> list = new List<AsmxURL>();
										foreach (XmlNode nd in ndList)
										{
											string s1 = XmlUtil.GetAttribute(nd, XmlTags.XMLATT_asmxUrl);
											if (!string.IsNullOrEmpty(s1))
											{
												string s0 = XmlUtil.GetAttribute(nd, XmlTags.XMLATT_filename);
												list.Add(new AsmxURL(s1, s0, XmlUtil.GetAttribute(nd, XmlTags.XMLATT_proxy), XmlUtil.GetAttribute(nd, XmlTags.XMLATT_wsdl)));
											}
										}
										if (list.Count > 0)
										{
											_urls = list;
											_prjFile = dlg.FileName;
											_folder = System.IO.Path.GetDirectoryName(dlg.FileName);
											_namespace = ns;
											if (string.IsNullOrEmpty(_namespace))
											{
												_namespace = "LimnorStudio";
											}
											bSelected = true;
										}
									}
								}
							}
						}
					}
					if (bSelected)
					{
						textBoxFolder.Text = _prjFile;
						listBox1.Items.Clear();
						foreach (AsmxURL a in _urls)
						{
							listBox1.Items.Add(a);
						}
						if (listBox1.Items.Count > 0)
						{
							listBox1.SelectedIndex = 0;
						}
					}
					else
					{
						MessageBox.Show(this, "No web services found", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, DesignerException.FormExceptionText(err), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
			}
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				AsmxURL au = listBox1.Items[n] as AsmxURL;
				DlgAddWebService dlg = new DlgAddWebService();
				dlg.SetData(_folder, _namespace);
				dlg.SetFixedAsmxUrl(au);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{

				}
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonUpdate.Enabled = (listBox1.SelectedIndex >= 0);
		}

	}
	internal class AsmxURL
	{
		private string _asmxUrl;
		private string _dll;
		private string _proxy;
		private string _wsdl;
		public AsmxURL(string asmx, string dll, string proxy, string wsdl)
		{
			_asmxUrl = asmx;
			_dll = dll;
			_proxy = proxy;
			_wsdl = wsdl;
		}
		public string AsmxUrl
		{
			get
			{
				return _asmxUrl;
			}
		}
		public string DllFile
		{
			get
			{
				return _dll;
			}
		}
		public string Proxy
		{
			get
			{
				return _proxy;
			}
		}
		public string Wsdl
		{
			get
			{
				return _wsdl;
			}
		}
		public override string ToString()
		{
			return _asmxUrl;
		}
	}
}
