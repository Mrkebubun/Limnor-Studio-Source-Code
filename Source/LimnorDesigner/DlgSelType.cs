using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace LimnorDesigner
{
    public partial class DlgSelType : Form
    {
        public Type RetType;
        public DlgSelType()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            treeView1.LoadGac();
        }

        private void treeView1_TypeSelected(object sender, EventArgs e)
        {
            btOK.Enabled = false;
            EventArgObjectSelected e0 = e as EventArgObjectSelected;
            if (e0 != null)
            {
                RetType = e0.SelectedObject as Type;
                btOK.Enabled = (RetType != null);
            }
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            treeView1.LoadDLL();
        }
    }
}
