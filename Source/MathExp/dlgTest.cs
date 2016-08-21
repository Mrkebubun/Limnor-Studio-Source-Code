/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.CodeDom;
using System.Reflection;

namespace MathExp
{
	/// <summary>
	/// unit test dialogue for math expression (group)
	/// </summary>
	public partial class dlgTest : Form, ITestDialog
	{
		#region fields and constructors
		private PropertyTable parameters;
		private MathNodeRoot mroot;
		private TestData _testData;
		private MethodInfo _mi;
		public dlgTest()
		{
			InitializeComponent();
			parameters = new PropertyTable();
			propertyGrid1.SelectedObject = parameters;
		}
		#endregion
		#region form event handlers
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			_testData.Finished = true;
		}
		private void btCalculate_Click(object sender, EventArgs e)
		{
			calculate();
		}
		#endregion
		#region test
		private void calculate()
		{
			if (_mi != null)
			{
				try
				{
					object[] ps = null;
					if (parameters != null && parameters.Properties.Count > 0)
					{
						ps = new object[parameters.Properties.Count];
						for (int i = 0; i < parameters.Properties.Count; i++)
						{
							ps[i] = parameters[parameters.Properties[i].Name];
						}
					}
					object v = _mi.Invoke(null, ps);
					if (v == null)
						txtResult.Text = "null";
					else
						txtResult.Text = v.ToString();
				}
				catch (Exception er)
				{
					txtResult.Text = er.Message;
				}
			}
		}
		#endregion
		#region ITestDialog members
		public bool LoadData(ITestData test)
		{
			try
			{
				_testData = (TestData)test;
				//provide drawing attributes for parameters
				mroot = new MathNodeRoot();
				if (_testData.Parameters.Count > 2)
				{
					mroot.ChildNodeCount = _testData.Parameters.Count;
				}
				parameters.Properties.Clear();
				for (int i = 0; i < _testData.Parameters.Count; i++)
				{
					mroot[i] = (MathNode)_testData.Parameters[i].CloneExp(mroot);
					parameters.Properties.Add(new PropertySpec(_testData.Parameters[i].VariableName + ":" + _testData.Parameters[i].CodeVariableName, _testData.Parameters[i].VariableType.Type));
				}
				foreach (IPropertyPointer pp in _testData.Pointers)
				{
					parameters.Properties.Add(new PropertySpec(pp.ToString() + ":" + pp.CodeName, pp.ObjectType));//.PointerDataType));
				}
				//
				CodeGeneratorOptions o = new CodeGeneratorOptions();
				o.BlankLinesBetweenMembers = false;
				o.BracingStyle = "C";
				o.ElseOnClosing = false;
				o.IndentString = "    ";
				//
				CSharpCodeProvider cs = new CSharpCodeProvider();
				StringWriter sw;
				sw = new StringWriter();
				cs.GenerateCodeFromCompileUnit(_testData.CU, sw, o);
				//
				string sCode = sw.ToString();
				sw.Close();
				//
				int pos = sCode.IndexOf("a tool.");
				if (pos > 0)
				{
					sCode = sCode.Substring(0, pos) + "Limnor Visual Object Builder." + sCode.Substring(pos + 7);
				}
				//
				textBox1.Text = sCode;
				//
				CompilerParameters cp = new CompilerParameters();
				foreach (AssemblyRef ar in _testData.Assemblies)
				{
					MathNode.AddImportLocation(ar.Location);
				}
				foreach (string s in MathNode.ImportLocations)
				{
					cp.ReferencedAssemblies.Add(s);
				}
				cp.GenerateExecutable = false;
				CompilerResults crs = cs.CompileAssemblyFromDom(cp, new CodeCompileUnit[] { _testData.CU });
				if (crs.Errors.HasErrors)
				{
					MathNode.Trace("Error compiling.");
					MathNode.IndentIncrement();
					FormCompilerError dlg = new FormCompilerError();
					for (int i = 0; i < crs.Errors.Count; i++)
					{
						MathNode.Trace(crs.Errors[i].ToString());
						dlg.AddItem(crs.Errors[i]);
					}
					MathNode.IndentDecrement();
					dlg.TopLevel = false;
					dlg.Parent = this;
					dlg.Show();
					dlg.TopMost = true;
					dlg.BringToFront();
				}
				else
				{
					Type[] types = crs.CompiledAssembly.GetExportedTypes();
					if (types != null)
					{
						for (int i = 0; i < types.Length; i++)
						{
							if (types[i].Name == _testData.ClassName)
							{
								_mi = types[i].GetMethod(_testData.MethodName);
								if (_mi != null)
								{
									break;
								}
							}
						}
					}
				}
				textBox2.Text = MathNode.GetLogContents();
				return true;
			}
			catch (Exception err)
			{
				MathNode.Log(this, err);
				textBox2.Text = MathNode.GetLogContents();
			}

			return false;
		}
		#endregion
	}
}