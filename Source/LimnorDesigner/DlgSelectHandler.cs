/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
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
using VPL;
using LimnorDesigner.Action;

namespace LimnorDesigner
{
	public partial class DlgSelectHandler : Form
	{
		private ClassPointer _root;
		private Type[] _paramTypes;
		private string[] _pnames;
		private bool _runatwebclient;
		public VplMethodPointer SelectedMethod;
		public DlgSelectHandler()
		{
			InitializeComponent();
		}
		public void LoadData(ClassPointer root, bool runAtWebClient, Type[] paramTypes, string[] pnames, UInt32 initSelected)
		{
			_root = root;
			_paramTypes = paramTypes;
			_pnames = pnames;
			_runatwebclient = runAtWebClient;
			List<VplMethodPointer> methods = root.GetCustomMethodsByParamTypes(runAtWebClient, paramTypes);
			listBox1.Items.Add(new VplMethodPointer());
			foreach (VplMethodPointer m in methods)
			{
				int n = listBox1.Items.Add(m);
				if (initSelected == m.MethodId)
				{
					listBox1.SelectedIndex = n;
				}
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				SelectedMethod = listBox1.Items[listBox1.SelectedIndex] as VplMethodPointer;
				buttonOK.Enabled = true;
			}
			else
			{
				buttonOK.Enabled = false;
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			EnumMethodWebUsage webuse = EnumMethodWebUsage.Server;
			if (_runatwebclient)
				webuse = EnumMethodWebUsage.Client;
			MethodClass mc = _root.CreateNewMethodFixParams(webuse, _paramTypes, _pnames, buttonAdd.ClientRectangle, this);
			if (mc != null)
			{
				SelectedMethod = new VplMethodPointer(mc.MethodID, mc.MethodName);
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
