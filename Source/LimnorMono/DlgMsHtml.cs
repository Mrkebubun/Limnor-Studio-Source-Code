using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace LimnorVOB
{
    public partial class DlgMsHtml : Form
    {
        public DlgMsHtml()
        {
            InitializeComponent();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(textBox1.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(this, err.Message, "Copy URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "http://www.limnor.com/studiodownload/setupmshtml.msi";
                p.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(this, err.Message, "Download", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
