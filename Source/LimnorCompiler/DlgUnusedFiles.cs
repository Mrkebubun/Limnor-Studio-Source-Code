/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
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
using System.Collections.Specialized;

namespace LimnorCompiler
{
	public partial class DlgUnusedFiles : Form
	{
		public DlgUnusedFiles()
		{
			InitializeComponent();
		}
		public void AddList(StringCollection lst)
		{
			foreach (string s in lst)
			{
				listBox1.Items.Add(s);
			}
		}
		public static void ShowList(StringCollection lst, Form caller)
		{
			DlgUnusedFiles dlg = new DlgUnusedFiles();
			dlg.AddList(lst);
			dlg.ShowDialog(caller);
		}
	}
}
