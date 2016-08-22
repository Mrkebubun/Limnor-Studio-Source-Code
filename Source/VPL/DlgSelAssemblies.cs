using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
    public partial class DlgSelAssemblies : Form
    {
        private List<Type> _types;
        public DlgSelAssemblies()
        {
            InitializeComponent();
        }
        public void LoadUsedTypes(List<Type> types)
        {
            _types = types;
            foreach (Type t in types)
            {
                tvUsed.Nodes.Add(new TreeNodeAssemblyType(t));
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            tvGac.LoadGac();
        }
    }
}
