using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace LimnorDesigner
{
    public partial class FormMsgList : Form
    {
        #region form
        public FormMsgList()
        {
            InitializeComponent();
        }
        public void LoadStrings(StringCollection ss)
        {
            listBox1.Items.Clear();
            foreach (string s in ss)
            {
                listBox1.Items.Add(s);
            }
        }
        public void AddMessage(string s)
        {
            listBox1.Items.Add(s);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.Text;
        }
        #endregion
        public static void ShowStrings(StringCollection ss)
        {
            FormMsgList dlg = new FormMsgList();
            dlg.LoadStrings(ss);
            dlg.ShowDialog();
        }
        #region Runtime trace
        private static FormMsgList _form;
        public static void Trace(string message)
        {
            if (_form == null)
            {
                _form = new FormMsgList();
                _form.TopMost = true;
                _form.Show();
            }
            _form.AddMessage(message);
        }
        #endregion
    }
}
