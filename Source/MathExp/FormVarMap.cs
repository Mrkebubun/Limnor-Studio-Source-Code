/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * variable map editor
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
    public partial class FormVarMap : Form
    {
        public FormVarMap()
        {
            InitializeComponent();
        }
        public void LoadData(VariableMap map)
        {
            propertyGrid1.SelectedObject = map;
            propertyGrid1.Refresh();
        }
    }
}
