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
using System.IO;

namespace LimnorDesigner.Remoting
{
	public partial class FormWebUtil : Form
	{
		private string _wsdlPath;
		private string _asmxUrl;
		public bool Finished;
		public bool WsdlCreated;
		public FormWebUtil()
		{
			InitializeComponent();
		}
		public void SetAsmxUrl(string url, string wsdlPath)
		{
			_asmxUrl = url + "?wsdl";
			_wsdlPath = wsdlPath;
			webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
			webBrowser1.Url = new Uri(_asmxUrl);
		}

		void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			SaveWsdl();
			Finished = true;
		}
		private void SaveWsdl()
		{
			if (webBrowser1.Document == null)
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Cannot load {0}", _asmxUrl), "Create WSDL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				StreamWriter sw = null;
				try
				{
					bool gotXml = true;
					sw = new StreamWriter(_wsdlPath);
					string s = webBrowser1.Document.Body.InnerText;
					string[] ss = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < ss.Length; i++)
					{
						if (!string.IsNullOrEmpty(ss[i]))
						{
							if (ss[i].StartsWith("-", StringComparison.Ordinal))
							{
								ss[i] = ss[i].Substring(1);
							}
							ss[i] = ss[i].Trim();
						}
						sw.WriteLine(ss[i]);
						if (i == 0)
						{
							gotXml = ss[i].StartsWith("<?xml", StringComparison.OrdinalIgnoreCase);
						}
					}
					sw.Close();
					sw = null;
					WsdlCreated = gotXml;
				}
				catch (Exception err)
				{
					MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Cannot create WSDL. {0}", err.Message), "Create WSDL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				finally
				{
					//
					if (sw != null)
					{
						sw.Close();
					}
				}
			}
		}
	}
}
