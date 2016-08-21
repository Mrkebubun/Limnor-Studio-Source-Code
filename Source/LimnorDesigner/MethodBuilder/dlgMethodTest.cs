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
using System.Text;
using System.Windows.Forms;
using MathExp;
using FileUtil;
using System.Xml;
using System.Collections;
using System.Reflection;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// unit test dialogue for methods
	/// </summary>
	public partial class dlgMethodTest : Form, ITestDialog
	{
		#region fields and constructors
		XmlNode testData;
		private bool loaded;
		private object obj; //created by compiling
		private PropertyTable parameters;
		private MethodInfo mi;
		private MethodTestData data;
		public dlgMethodTest()
		{
			InitializeComponent();
			parameters = new PropertyTable();
			propertyGrid1.SelectedObject = parameters;
		}
		#endregion
		#region ITestDialog members
		public bool LoadData(ITestData test)
		{
			data = (MethodTestData)test;
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(data.FullContents);
			testData = doc.DocumentElement.SelectSingleNode(data.XPath);
			if (testData == null)
			{
				throw new MathException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid xpath '{0}' for the test node", data.XPath));
			}
			return true;
		}
		#endregion
		#region methods
		protected void AddFile(string f)
		{
			listBoxSourceFiles.Items.Add(new FileName(f));
		}
		protected void AddFile(string f, string title)
		{
			listBoxSourceFiles.Items.Add(new FileName(f, title));
		}
		protected void AddErrorMessage(string msg)
		{
			listBoxSourceFiles.Items.Add(new ListItemErrorMessage(msg));
		}
		private string createTestFolder()
		{
			string _testFolder;
			_testFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "UnitTest");
			if (System.IO.Directory.Exists(_testFolder))
			{
				FileUtilities.ClearFolders(_testFolder);
			}
			else
			{
				System.IO.Directory.CreateDirectory(_testFolder);
			}
			return _testFolder;
		}
		#endregion
		#region Compile
		protected void Compile()
		{
			obj = null;
		}
		#endregion
		#region form event handlers
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (!loaded)
			{
				loaded = true;
				Compile();
			}
		}
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				mi = e.Node.Tag as MethodInfo;
				if (mi != null)
				{
					this.Text = "Method:" + e.Node.Text;
					ParameterInfo[] pis = mi.GetParameters();
					parameters = new PropertyTable();
					for (int i = 0; i < pis.Length; i++)
					{
						try
						{
							object v = VPLUtil.GetDefaultValue(pis[i].ParameterType);
							Type t = pis[i].ParameterType;
							if (t.Equals(typeof(object)))
							{
								t = typeof(string);
							}
							parameters.Properties.Add(new PropertySpec(pis[i].Name, t, "", "", v));
							parameters[pis[i].Name] = v;
						}
						catch (Exception err)
						{
							MathNode.Log(this, err);
						}
					}
					propertyGrid1.SelectedObject = parameters;
				}
			}
		}

		private void btCalculate_Click(object sender, EventArgs e)
		{
			if (mi != null)
			{
				try
				{
					object[] ps = new object[parameters.Properties.Count];
					for (int i = 0; i < ps.Length; i++)
					{
						ps[i] = parameters[parameters.Properties[i].Name];
					}
					object ret = mi.Invoke(obj, ps);
					if (mi.ReturnType == null || typeof(void).Equals(mi.ReturnType))
					{
						txtResult.Text = "";
					}
					else
					{
						if (ret == null)
							txtResult.Text = "null";
						else
							txtResult.Text = ret.ToString();
					}
				}
				catch (Exception err)
				{
					MathNode.Log(this, err);
				}
			}
		}

		private void splitContainer2_Panel1_Resize(object sender, EventArgs e)
		{

			if (splitContainer2.Panel1.ClientSize.Width > txtResult.Left)
			{
				txtResult.Width = splitContainer2.Panel1.ClientSize.Width - txtResult.Left;
			}
			if (splitContainer2.Panel1.ClientSize.Height > txtResult.Top)
			{
				txtResult.Height = splitContainer2.Panel1.ClientSize.Height - txtResult.Top;
			}
		}

		private void listBoxSourceFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = listBoxSourceFiles.SelectedIndex;
			if (n >= 0)
			{
				try
				{
					FileName f = listBoxSourceFiles.Items[n] as FileName;
					if (f != null)
					{
						System.IO.StreamReader sr = new System.IO.StreamReader(f.Filename);
						txtSource.Text = sr.ReadToEnd();
						sr.Close();
					}
					else
					{
						ListItemErrorMessage err = listBoxSourceFiles.Items[n] as ListItemErrorMessage;
						if (err != null)
						{
							txtSource.Text = err.Message;
						}
					}
				}
				catch (Exception er)
				{
					MessageBox.Show(er.Message);
				}
			}
		}
		#endregion
	}
}