using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LimnorWebBrowser
{
    public partial class DialogHtmlContents : Form
    {
        private string _html;
        public DialogHtmlContents()
        {
            InitializeComponent();
        }
        public void LoadData(EditContents html)
        {
            editor1.BodyHtml = html.HtmlContents;
            editor1.BackColor = html.BackColor;
            editor1.ToolbarVisible = true;
        }
        public string BodyHtml
        {
            get
            {
                return _html;
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            _html = editor1.BodyHtml;
            this.DialogResult = DialogResult.OK;
        }
    }
}
