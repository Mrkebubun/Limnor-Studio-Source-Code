using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
    public partial class DataInput : UserControl
    {
        public DataInput()
        {
            InitializeComponent();
        }
        public void SetPropertyValue(object v)
        {
            propertyGrid1.SelectedObject = v;
            propertyGrid1.Focus();
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

        }

        //private void DataInput_Resize(object sender, EventArgs e)
        //{
        //    propertyGrid1.Width = this.ClientSize.Width;
        //    if (this.ClientSize.Height > propertyGrid1.Top)
        //    {
        //        propertyGrid1.Height = this.ClientSize.Height - propertyGrid1.Top;
        //    }
        //}
    }
}
