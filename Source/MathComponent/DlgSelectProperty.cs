/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathExp;

namespace MathComponent
{
	public partial class DlgSelectProperty : Form
	{
		public MathPropertyPointer SelectedProperty;
		public DlgSelectProperty()
		{
			InitializeComponent();
		}
		public void LoadData(Form form)
		{
			treeView1.LoadData(form);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			TreeNodeObject tno = treeView1.SelectedNode as TreeNodeObject;
			if (tno != null)
			{
				SelectedProperty = tno.CreatePointer();
				DialogResult = DialogResult.OK;
			}
		}
	}
}
