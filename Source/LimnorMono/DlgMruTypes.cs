using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorVOB
{
	public partial class DlgMruTypes : Form
	{
		private EnumFuType _fuType= EnumFuType.None;
		public DlgMruTypes()
		{
			InitializeComponent();
			panelFuTypes.Visible = false;
		}

		private void buttonAddFu_Click(object sender, EventArgs e)
		{
			if (_fuType == EnumFuType.Web)
			{
			}
			else if (_fuType == EnumFuType.Forms)
			{
			}
		}

		private void buttonDeleteFu_Click(object sender, EventArgs e)
		{

		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (string.CompareOrdinal(e.Node.Name, "NodeFuTypes") == 0)
			{
				panelFuTypes.Visible = false;
				_fuType = EnumFuType.None;
			}
			else if (string.CompareOrdinal(e.Node.Name, "NodeFuForms") == 0)
			{
				panelFuTypes.Visible = true;
				_fuType = EnumFuType.Forms;
			}
			else if (string.CompareOrdinal(e.Node.Name, "NodeFuWebPage") == 0)
			{
				panelFuTypes.Visible = true;
				_fuType = EnumFuType.Web;
			}
		}
	}
	enum EnumFuType { None = 0, Web = 1, Forms = 2 }
}
