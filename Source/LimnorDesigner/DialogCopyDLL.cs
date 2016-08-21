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

namespace LimnorDesigner
{
	public partial class DialogCopyDLL : Form
	{
		public DialogCopyDLL()
		{
			InitializeComponent();
			textBoxFolder.Text = Path.GetDirectoryName(this.GetType().Assembly.Location);
		}
		public static void StartDialogue()
		{
			DialogCopyDLL dlg = new DialogCopyDLL();
			dlg.ShowDialog();
		}

		private void buttonFile_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = "Select DLL file";
				dlg.Filter = "DLL|*.dll";
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					textBoxFile.Text = dlg.FileName;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
				textBoxMsg.Text = err.Message;
			}
		}

		private void buttonCopy_Click(object sender, EventArgs e)
		{
			buttonCopy.Enabled = false;
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			try
			{
				textBoxMsg.Text = "Copying ...";
				textBoxMsg.Refresh();
				if (textBoxFile.Text.Length == 0)
				{
					throw new Exception("DLL file is not given");
				}
				if (!File.Exists(textBoxFile.Text))
				{
					throw new Exception("DLL file does not exist");
				}
				string st = Path.Combine(textBoxFolder.Text, Path.GetFileName(textBoxFile.Text));
				if (string.Compare(st, textBoxFile.Text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					throw new Exception("Do not copy from the target folder");
				}
				File.Copy(textBoxFile.Text, st, true);
				textBoxMsg.Text = "File copied.";
				MessageBox.Show(textBoxMsg.Text);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
				textBoxMsg.Text = err.Message;
			}
			buttonCopy.Enabled = true;
			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
	}
}
