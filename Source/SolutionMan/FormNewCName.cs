/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace SolutionMan
{
	public partial class FormNewCName : Form
	{
		public string Ret;
		private StringCollection _names;
		private string _folder;
		public FormNewCName()
		{
			InitializeComponent();
		}
		public void LoadData(StringCollection names, string prjFolder)
		{
			_names = names;
			_folder = prjFolder;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			string name = textBox1.Text.Trim();
			if (name.Length == 0)
			{
				MessageBox.Show(this, "Name cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			for (int i = 0; i < _names.Count; i++)
			{
				if (string.Compare(name, _names[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					MessageBox.Show(this, "The name is in use", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			if (!Char.IsLetter(name, 0))
			{
				MessageBox.Show(this, "The first character of the name must be a letter",Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			for (int i = 0; i < name.Length; i++)
			{
				char ch = name[i];
				if (ch != '_')
				{
					UnicodeCategory uc = Char.GetUnicodeCategory(ch);
					switch (uc)
					{
						case UnicodeCategory.UppercaseLetter:
						case UnicodeCategory.LowercaseLetter:
						case UnicodeCategory.TitlecaseLetter:
						case UnicodeCategory.DecimalDigitNumber:
							break;
						default:
							MessageBox.Show(this, "The name '" + name + "' is not a valid identifier. Use only letters, numbers, and underscores.",Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
					}
				}
			}
			string f = Path.Combine(_folder, string.Format(CultureInfo.InvariantCulture, "{0}.limnor", name));
			if (File.Exists(f))
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Cannot create new component. File exists:{0}", f), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Ret = name;
			this.DialogResult = DialogResult.OK;
		}
	}
}
