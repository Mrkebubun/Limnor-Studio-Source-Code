/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	UI Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorUI
{
	public partial class dlgObject : Form, IRefreshPropertyGrid
	{
		public object ret;
		public event EventHandler OnOK;
		int d = 6;
		public dlgObject()
		{
			InitializeComponent();
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			int nw = btCancel.Width + btOK.Width + d;
			nw = (this.ClientSize.Width - nw) / 2;
			btOK.Left = nw;
			btCancel.Left = btOK.Left + btOK.Width + d;
		}
		public static bool EditObject(IWin32Window owner, object v, string title, EventHandler onOk)
		{
			dlgObject dlg = new dlgObject();
			dlg.LoadData(v, title, onOk);
			return (dlg.ShowDialog(owner) == DialogResult.OK);
		}
		public void LoadData(object v, string title, EventHandler onOk)
		{
			ret = v;
			this.Text = title;
			OnOK += onOk;
			propertyGrid1.SelectedObject = ret;
			IPropertyGridHolder ip = v as IPropertyGridHolder;
			if (ip != null)
			{
				ip.SetEditor(this);
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (OnOK != null)
			{
				EventArgCancel ev = new EventArgCancel(ret);
				OnOK(this, ev);
				if (ev.Cancel)
				{
					if (!string.IsNullOrEmpty(ev.Message))
					{
						MessageBox.Show(this, ev.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
					}
				}
				else
				{
					this.DialogResult = DialogResult.OK;
				}
			}
			else
			{
				this.DialogResult = DialogResult.OK;
			}
		}

		#region IRefreshPropertyGrid Members

		public void OnRefreshPropertyGrid()
		{
			propertyGrid1.Refresh();
		}

		#endregion
	}
	public interface IRefreshPropertyGrid
	{
		void OnRefreshPropertyGrid();
	}
	public interface IPropertyGridHolder
	{
		void SetEditor(IRefreshPropertyGrid e);
	}
}