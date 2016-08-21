/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
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

namespace MathComponent
{
	public partial class DlgMathXmlString : Form
	{
		public string ResultXml;
		public DlgMathXmlString()
		{
			InitializeComponent();
		}
		public void LoadData(string data)
		{
			textBoxXml.Text = data;
		}
		private void buttonCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(textBoxXml.Text);
		}

		private void buttonPaste_Click(object sender, EventArgs e)
		{
			string s = Clipboard.GetText();
			if (!string.IsNullOrEmpty(s))
			{
				textBoxXml.Text = s;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			ResultXml = textBoxXml.Text;
			this.DialogResult = DialogResult.OK;
		}

		private void buttonFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select XML file for Math Expression";
			dlg.Filter = "Xml files|*.xml";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				textBoxFile.Text = dlg.FileName;
				try
				{
					StreamReader sr = new StreamReader(dlg.FileName);
					textBoxXml.Text = sr.ReadToEnd();
					sr.Close();
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Title = "Save Math Expression to Xml File";
				dlg.Filter = "Xml files|*.xml";
				dlg.OverwritePrompt = true;
				dlg.ValidateNames = true;
				dlg.CheckFileExists = false;
				dlg.CheckPathExists = true;
				dlg.RestoreDirectory = true;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					StreamWriter sw = new StreamWriter(dlg.FileName, false);
					sw.Write(textBoxXml.Text);
					sw.Close();
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
