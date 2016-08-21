/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
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

namespace LimnorDatabase
{
	public partial class DlgConnection : Form
	{
		private EventHandler onTest;
		StringCollection _refreshNames;
		public DlgConnection()
		{
			InitializeComponent();
			_refreshNames = new StringCollection();
		}
		public void LoadData(object data)
		{
			propertyGrid1.SelectedObject = data;
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
		}
		public void AddRefreshName(string name)
		{
			_refreshNames.Add(name);
		}
		public void SetTest(EventHandler test)
		{
			onTest = test;
			btTest.Visible = (onTest != null);
		}
		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (_refreshNames.Contains(e.ChangedItem.PropertyDescriptor.Name))
			{
				propertyGrid1.Refresh();
			}
		}

		public static bool EditObject(object data, Form caller)
		{
			DlgConnection dlg = new DlgConnection();
			dlg.LoadData(data);
			return (dlg.ShowDialog(caller) == DialogResult.OK);
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			if (onTest != null)
			{
				onTest(propertyGrid1.SelectedObject, EventArgs.Empty);
			}
		}
	}
}
