using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Limnor.TreeViewExt
{
    public partial class DlgTreeNodeTemp : Form
    {
        private Guid _selectedId = Guid.Empty;
        private bool _changed;
        public DlgTreeNodeTemp()
        {
            InitializeComponent();
        }
        public void LoadData(TreeViewX treeView)
        {
            if (treeView.ImageList != null)
            {
                for (int i = 0; i < treeView.ImageList.Images.Count; i++)
                {
                    imageList1.Images.Add(treeView.ImageList.Images[i]);
                }
            }
            treeView1.XmlString = treeView.XmlString;
            //treeView1.SetEditMenu();
        }
        public bool Changed
        {
            get
            {
                return _changed;
            }
        }
        public Guid SelectedGuid
        {
            get
            {
                return _selectedId;
            }
        }
        public string XmlString
        {
            get
            {
                return treeView1.SaveToXmlDocument().OuterXml;
            }
        }
        public XmlNode GetTemplatesNode()
        {
            treeView1.SaveToXmlDocument();
            return treeView1.GetTemplatesXmlNode();
        }
        //private void enableButtons()
        //{
        //    TreeNodeTemp tnx = treeView1.SelectedNode as TreeNodeTemp;
        //    if (tnx != null)
        //    {
        //        buttonAddSub.Enabled = true;
        //        buttonAddValue.Enabled = true;
        //        buttonDelNode.Enabled = true;
        //        buttonDelValue.Enabled = false;
        //    }
        //    else
        //    {
        //    }
        //}
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNodeX tnx = e.Node as TreeNodeX;
            if (tnx != null)
            {
                _selectedId = tnx.TreeNodeId;
                propertyGrid1.SelectedObject = tnx;
                //
                buttonAddValue.Enabled = true;
                buttonDelNode.Enabled = true;
                buttonDelValue.Enabled = false;
                buttonAddSub.Enabled = true;
            }
            else
            {
                TreeNodeValue tnv = e.Node as TreeNodeValue;
                if (tnv != null)
                {
                    propertyGrid1.SelectedObject = tnv.Data;
                    //
                    buttonAddValue.Enabled = false;
                    buttonDelNode.Enabled = false;
                    buttonDelValue.Enabled = true;
                    buttonAddSub.Enabled = false;
                }
                else
                {
                    propertyGrid1.SelectedObject = null;
                    //
                    buttonAddValue.Enabled = false;
                    buttonDelNode.Enabled = false;
                    buttonDelValue.Enabled = false;
                    buttonAddSub.Enabled = false;
                }
            }
        }

        private void buttonAddRoot_Click(object sender, EventArgs e)
        {
            treeView1.AddRootNode("Root node");
            _changed = true;
        }

        private void buttonAddSub_Click(object sender, EventArgs e)
        {
            treeView1.AddSubNode("Sub node");
            _changed = true;
        }

        private void buttonDelNode_Click(object sender, EventArgs e)
        {
            treeView1.DeleteSelectedNodeTemplate(true);
            _changed = true;
        }

        private void buttonAddValue_Click(object sender, EventArgs e)
        {
            treeView1.CreateNewValue();
            _changed = true;
        }

        private void buttonDelValue_Click(object sender, EventArgs e)
        {
            treeView1.DeleteSelectedValue(true);
            _changed = true;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _changed = true;
            if (string.CompareOrdinal("ImageIndex", e.ChangedItem.PropertyDescriptor.Name) == 0)
            {
                if (typeof(int).Equals(e.ChangedItem.PropertyDescriptor.PropertyType))
                {
                    int n = (int)(e.ChangedItem.Value);
                    if (n >= 0)
                    {
                        TreeNode tn = propertyGrid1.SelectedObject as TreeNode;
                        if (tn != null)
                        {
                            if (tn.SelectedImageIndex < 0)
                            {
                                tn.SelectedImageIndex = n;
                            }
                        }
                    }
                }
            }
        }

        private void buttonToRoot_Click(object sender, EventArgs e)
        {

        }
    }
}
