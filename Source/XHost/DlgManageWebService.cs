using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VSPrj;
using System.Collections.Specialized;
using LimnorDesigner;
using VPL;
using XmlUtility;

namespace XHost
{
    public partial class DlgManageWebService : Form
    {
        private StringCollection _deleted;
        private LimnorProject _prj;
        public DlgManageWebService()
        {
            InitializeComponent();
        }
        public void LoadData(LimnorProject project)
        {
            _prj = project;
            VSPrj.PropertyBag sc = _prj.GetWebServiceFiles();
            foreach (Dictionary<string,string> s in sc)
            {
                listBox1.Items.Add(s[XmlTags.XMLATT_filename]);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            buttonDelete.Enabled = false;
            int n = listBox1.SelectedIndex;
            if (n >= 0)
            {
                string s = listBox1.Items[n] as string;
                if (MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Do you want to remove [{0}]?", s), "Delete web service", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    if (_deleted == null)
                    {
                        _deleted = new StringCollection();
                    }
                    _deleted.Add(s);
                    listBox1.Items.RemoveAt(n);
                    if (n >= listBox1.Items.Count)
                    {
                        n--;
                    }
                    listBox1.SelectedIndex = n;
                }
            }
            buttonDelete.Enabled = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_deleted != null && _deleted.Count > 0)
            {
                _prj.RemoveWebServices(_deleted);
                _prj.RemoveTypedProjectData<List<WebServiceProxy>>();
                //
            }
            DialogResult = DialogResult.OK;
        }
    }
}
