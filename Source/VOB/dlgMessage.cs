using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VOB
{
    public partial class dlgMessage : Form
    {
        public bool bShowMessage = true;
        public dlgMessage()
        {
            InitializeComponent();
        }
        public void SetMessage(string file,string msg)
        {
            txtLog.Text = file;
            txtMsg.Text = msg;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            bShowMessage = !chkNoMsg.Checked;
        }
    }
}