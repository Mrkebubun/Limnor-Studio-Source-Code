using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.TreeViewExt
{
    public partial class DlgEnumTreeViewMenu : Form
    {
        public EnumTreeViewMenu ResultItems;
        public DlgEnumTreeViewMenu()
        {
            InitializeComponent();
        }
        public void LoadData(EnumTreeViewMenu items)
        {
            ResultItems = items;
            Array vals = Enum.GetValues(typeof(EnumTreeViewMenu));
            for (int i = 0; i < vals.Length; i++)
            {
                EnumTreeViewMenu v = (EnumTreeViewMenu)vals.GetValue(i);
                if ((v & items) == v)
                {
                    checkedListBox1.Items.Add(v, true);
                }
                else
                {
                    checkedListBox1.Items.Add(v, false);
                }
            }
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            ResultItems = EnumTreeViewMenu.None;
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                ResultItems |= (EnumTreeViewMenu)checkedListBox1.CheckedItems[i];
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
