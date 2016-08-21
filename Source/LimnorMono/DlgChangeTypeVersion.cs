/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
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
using System.Globalization;

namespace LimnorVOB
{
	public partial class DlgChangeTypeVersion : Form
	{
		private Type _ret = null;
		public DlgChangeTypeVersion()
		{
			InitializeComponent();
			btOK.Image = Resource1._ok.ToBitmap();
		}
		public static Type SelectTypeVersion(Form caller, string typeString)
		{
			DlgChangeTypeVersion dlg = new DlgChangeTypeVersion();
			dlg.SetTypeText(typeString);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				return dlg.ReturnType;
			}
			return null;
		}
		public void SetTypeText(string typetext)
		{
			lblSourceType.Text = typetext;
		}
		public Type ReturnType
		{
			get
			{
				return _ret;
			}
		}
		private void btChange_Click(object sender, EventArgs e)
		{
			string ver = txtVer.Text.Trim();
			if (ver.Length > 0)
			{
				int n0 = lblSourceType.Text.IndexOf("Version=", StringComparison.Ordinal);
				if (n0 > 0)
				{
					int n1 = lblSourceType.Text.IndexOf(',', n0);
					if (n1 > 0)
					{
						string newtype = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", lblSourceType.Text.Substring(0, n0 + 8), ver, lblSourceType.Text.Substring(n1));
						Type t = Type.GetType(newtype);
						if (t == null)
						{
							MessageBox.Show(this, "The new version is invalid", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							_ret = t;
							lblGoodVer.Text = ver;
							btOK.Enabled = true;
							MessageBox.Show(this, "The new version is valid", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
				}
			}
		}
	}
}
