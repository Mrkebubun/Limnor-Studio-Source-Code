using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XHost
{
    public partial class DlgAskFileOverwrite : Form
    {
        private string _folder;
        private string _ext;
        //private string _filePath;
        public string NewFilePath;
        public DlgAskFileOverwrite()
        {
            InitializeComponent();
        }
        public void SetFilePath(string filePath)
        {
            //_filePath = filePath;
            _folder = System.IO.Path.GetDirectoryName(filePath);
            _ext = System.IO.Path.GetExtension(filePath);
            textBoxFile.Text = filePath;
        }
        private void rdbNewFile_CheckedChanged(object sender, EventArgs e)
        {
            textBoxNewName.ReadOnly = !rdbNewFile.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (rdbOverwrite.Checked)
            {
                this.DialogResult = DialogResult.Ignore;
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxNewName.Text))
                {
                    MessageBox.Show(this, "New file name cannot be empty",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (textBoxNewName.Text.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show(this, "New file name cannot end with dot ('.')", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                        int n = textBoxNewName.Text.IndexOfAny(invalidChars);
                        if (n < 0)
                        {
                            n = textBoxNewName.Text.IndexOf(".", StringComparison.Ordinal);
                        }
                        if (n >= 0)
                        {
                            MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "New file name [{0}] contains invalid characters, for example, '{1}'", textBoxNewName.Text, textBoxNewName.Text[n]), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            NewFilePath = System.IO.Path.Combine(_folder, textBoxNewName.Text) + _ext;
                            if (System.IO.File.Exists(NewFilePath))
                            {
                                MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "New file name [{0}] already exist", NewFilePath), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else
                            {
                                DialogResult = DialogResult.OK;
                            }
                        }
                    }
                }
            }
        }

        private void textBoxNewName_TextChanged(object sender, EventArgs e)
        {
            textBoxNewPath.Text = System.IO.Path.Combine(_folder, textBoxNewName.Text) + _ext;
        }
    }
}
