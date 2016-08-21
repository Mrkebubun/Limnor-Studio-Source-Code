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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.CodeDom;
using System.Collections.Specialized;
using System.Reflection;
using MathExp.RaisTypes;
using XmlUtility;
using ProgElements;
using TraceLog;

namespace MathExp
{
	[Flags()]
	public enum enumOperatorCategory { Decimal = 1, Logic = 2, Integer = 4, String = 8, Internal = 16, System = 32 }
	/// <summary>
	/// UI for editing
	/// </summary>
	public partial class MathExpEditor : UserControl, IUnitTestOwner
	{
		#region fields and constructors
		const int IMG_Paste_Enable = 6;
		const int IMG_Paste_Disable = 9;
		public static bool ExcludeProjectItem;
		public event EventHandler OnOK;
		public event EventHandler OnCancel;
		public event EventHandler OnCreateCompound;
		private XmlNode mathNode;
		private IActionContext _actionContext;
		private ITestData _testData;
		private System.Threading.Thread thTest;
		public MathExpEditor()
		{
			InitializeComponent();
			//
			mathExpCtrl1.SetMathEditor(this);
			//
			List<Type> NodeTypes;
			List<Type> NodeTypesLogic;
			List<Type> NodeTypesInteger;
			List<Type> NodeTypesString;
			List<Type> NodeTypesOther;
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
			propertyGrid2.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid2_PropertyValueChanged);
			tabGreek.Text = new string((char)0x03b1, 1) + " - " + new string((char)0x03c9, 1);
			NodeTypes = new List<Type>(); //decimal nodes
			NodeTypesLogic = new List<Type>(); //logic nodes
			NodeTypesInteger = new List<Type>(); //integer nodes
			NodeTypesString = new List<Type>(); //string nodes
			NodeTypesOther = new List<Type>(); //other nodes
			//load types in this DLL
			Type[] tps0 = this.GetType().Assembly.GetExportedTypes();
			List<Type> types = MathNode.MathNodePlugIns;
			Type[] tps = new Type[tps0.Length + types.Count];
			tps0.CopyTo(tps, 0);
			types.CopyTo(tps, tps0.Length);
			for (int i = 0; i < tps.Length; i++)
			{
				enumOperatorCategory opCat = MathNodeCategoryAttribute.GetTypeCategory(tps[i]);
				if (opCat != enumOperatorCategory.Internal)
				{
					if (!tps[i].IsAbstract && tps[i].IsSubclassOf(typeof(MathNode)))
					{
						if (!tps[i].Equals(typeof(MathNodeRoot)))
						{
							if ((opCat & enumOperatorCategory.Logic) != 0)
							{
								NodeTypesLogic.Add(tps[i]);
							}
							if ((opCat & enumOperatorCategory.Integer) != 0)
							{
								NodeTypesInteger.Add(tps[i]);
							}
							if ((opCat & enumOperatorCategory.String) != 0)
							{
								NodeTypesString.Add(tps[i]);
							}
							if ((opCat & enumOperatorCategory.System) != 0)
							{
								if (ExcludeProjectItem)
								{
									object[] vs = tps[i].GetCustomAttributes(typeof(ProjectItemAttribute), true);
									if (vs != null && vs.Length > 0)
									{
										continue;
									}
								}
								NodeTypesOther.Add(tps[i]);
							}
							if ((opCat & enumOperatorCategory.Decimal) != 0)
							{
								NodeTypes.Add(tps[i]);
							}
						}
					}
				}
			}
			//load types from configuration XML
			string file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location), "MathTypes.xml");
			if (System.IO.File.Exists(file))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				if (doc.DocumentElement != null)
				{
					for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
					{
						try
						{
							Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement.ChildNodes[i]);
							enumOperatorCategory opCat = MathNodeCategoryAttribute.GetTypeCategory(t);
							if (opCat != enumOperatorCategory.Internal)
								if ((opCat & enumOperatorCategory.Logic) != 0)
								{
									if (!NodeTypesLogic.Contains(t))
										NodeTypesLogic.Add(t);
								}
							if ((opCat & enumOperatorCategory.Integer) != 0)
							{
								if (!NodeTypesInteger.Contains(t))
									NodeTypesInteger.Add(t);
							}
							if ((opCat & enumOperatorCategory.String) != 0)
							{
								if (!NodeTypesString.Contains(t))
									NodeTypesString.Add(t);
							}
							if ((opCat & enumOperatorCategory.System) != 0)
							{
								if (!NodeTypesOther.Contains(t))
									NodeTypesOther.Add(t);
							}
							if ((opCat & enumOperatorCategory.Decimal) != 0)
							{
								if (!NodeTypes.Contains(t))
									NodeTypes.Add(t);
							}
						}
						catch (Exception err)
						{
							MessageBox.Show(err.Message);
						}
					}
				}
			}
			typeIcons1.Category = enumOperatorCategory.Decimal;
			typeIcons1.lblTooltips = lblTooltips;
			typeIcons1.OnTypeSelected += new EventHandler(typeIcons1_OnTypeSelected);
			typeIcons1.LoadData(NodeTypes);

			typeIconsLogic.lblTooltips = lblTooltips;
			typeIconsLogic.Category = enumOperatorCategory.Logic;
			typeIconsLogic.OnTypeSelected += new EventHandler(typeIcons1_OnTypeSelected);
			typeIconsLogic.LoadData(NodeTypesLogic);

			typeIconsInteger.lblTooltips = lblTooltips;
			typeIconsInteger.Category = enumOperatorCategory.Integer;
			typeIconsInteger.OnTypeSelected += new EventHandler(typeIcons1_OnTypeSelected);
			typeIconsInteger.LoadData(NodeTypesInteger);

			typeIconsString.lblTooltips = lblTooltips;
			typeIconsString.Category = enumOperatorCategory.String;
			typeIconsString.OnTypeSelected += new EventHandler(typeIcons1_OnTypeSelected);
			typeIconsString.LoadData(NodeTypesString);

			typeIconsOther.lblTooltips = lblTooltips;
			typeIconsOther.Category = enumOperatorCategory.System;
			typeIconsOther.OnTypeSelected += new EventHandler(typeIcons1_OnTypeSelected);
			typeIconsOther.LoadData(NodeTypesOther);

			//
			greekLetters1.OnLetterSelected += new EventHandler(greekLetters1_OnLetterSelected);
			//
			mathExpCtrl1.OnFinish += new EventHandler(btOK_Click);
			mathExpCtrl1.OnCancel += new EventHandler(btCancel_Click);
			mathExpCtrl1.OnCreateCompound += new EventHandler(mathExpCtrl1_OnCreateCompound);
			//
			//
			MethodType mt = (MethodType)MathNode.GetService(typeof(MethodType));
			typeIconsOther.SetIconVisible(typeof(MathNodeArgument), (mt != null && mt.ParameterCount > 0));
			//
			propertyGrid2.SetOnValueChange(onValueChanged);
			//
			if (Clipboard.ContainsData("MathNode"))
			{
				btPaste.Enabled = true;
				btPaste.ImageIndex = IMG_Paste_Enable;
			}
			tabControl1.SelectedTab = tabPageOther;
			typeIconsOther.LoadIcons();
		}
		#endregion

		void mathExpCtrl1_OnCreateCompound(object sender, EventArgs e)
		{
			if (OnCreateCompound != null)
			{
				btOK_Click(this, e);
				OnCreateCompound(this, e);
			}
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			SetEditorFocus();
		}
		private bool _inited = false;
		public void OnLoaded()
		{
			if (!_inited)
			{
				_inited = true;
				this.btOK.Click += new System.EventHandler(this.btOK_Click);
				this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
				this.btPaste.Click += new System.EventHandler(this.btPaste_Click);
				this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
				this.btCut.Click += new System.EventHandler(this.btCut_Click);
				this.btRedo.Click += new System.EventHandler(this.btRedo_Click);
				this.btUndo.Click += new System.EventHandler(this.btUndo_Click);
				this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
				this.btInsert.Click += new System.EventHandler(this.btInsert_Click);
			}
		}
		public void RefreshButtons()
		{
			if (Clipboard.ContainsData("MathNode"))
			{
				btPaste.Enabled = true;
				btPaste.ImageIndex = IMG_Paste_Enable;
			}
			else
			{
				btPaste.Enabled = false;
				btPaste.ImageIndex = IMG_Paste_Disable;
			}
		}
		public void SetEditorFocus()
		{
			OnLoaded();
			mathExpCtrl1.Focus();
		}
		public void AddMathNode(Type type)
		{
			enumOperatorCategory category = MathNodeCategoryAttribute.GetTypeCategory(type);
			if ((category & enumOperatorCategory.Logic) != 0)
			{
				if (!typeIconsLogic.Contains(type))
					typeIconsLogic.AddIcon(type);
			}
			if ((category & enumOperatorCategory.Integer) != 0)
			{
				if (!typeIconsInteger.Contains(type))
					typeIconsInteger.AddIcon(type);
			}
			if ((category & enumOperatorCategory.String) != 0)
			{
				if (!typeIconsString.Contains(type))
					typeIconsString.AddIcon(type);
			}
			if ((category & enumOperatorCategory.System) != 0)
			{
				if (!typeIconsOther.Contains(type))
					typeIconsOther.AddIcon(type);
			}
			if ((category & enumOperatorCategory.Decimal) != 0)
			{
				if (!typeIcons1.Contains(type))
					typeIcons1.AddIcon(type);
			}
		}
		public void SetScopeMethod(IMethod method)
		{
			propertyGrid1.ScopeMethod = method;
			propertyGrid2.ScopeMethod = method;
		}
		public void NoCompoundCreation()
		{
			mathExpCtrl1.OnCreateCompound -= new EventHandler(mathExpCtrl1_OnCreateCompound);
		}
		public MathNodeRoot MathExpression
		{
			get
			{
				return mathExpCtrl1.Root;
			}
			set
			{
				mathExpCtrl1.Root = value;
			}
		}
		public RaisDataType DataType
		{
			get
			{
				return mathExpCtrl1.DataType;
			}
		}
		public RaisDataType ExpDataType
		{
			get
			{
				return mathExpCtrl1.ExpDataType;
			}
		}
		void greekLetters1_OnLetterSelected(object sender, EventArgs e)
		{
			if (sender != null)
			{
				string s = sender.ToString();
				mathExpCtrl1.SendLetter(s);
				mathExpCtrl1.RefreshVariableMap();
			}
		}
		void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabControl1.SelectedTab != null)
			{
				for (int i = 0; i < tabControl1.SelectedTab.Controls.Count; i++)
				{
					TypeIcons ti = tabControl1.SelectedTab.Controls[i] as TypeIcons;
					if (ti != null)
					{
						ti.LoadIcons();
						break;
					}
					else
					{
						GreekLetters gk = tabControl1.SelectedTab.Controls[i] as GreekLetters;
						if (gk != null)
						{
							gk.LoadData();
						}
					}
				}
			}
		}
		void typeIcons1_OnTypeSelected(object sender, EventArgs e)
		{
			Type t = sender as Type;
			if (t != null)
			{
				enumOperatorCategory category = enumOperatorCategory.Decimal;
				EventArgsEletementSelect ec = e as EventArgsEletementSelect;
				if (ec != null)
				{
					category = ec.Category;
				}
				MathNode newNode = null;
				MathNode node = mathExpCtrl1.FocusedNode;
				if (node != null && node != mathExpCtrl1.Root[0])
				{
					newNode = mathExpCtrl1.ReplaceMathNode(node, t);
				}
				else
				{
					newNode = mathExpCtrl1.AddMathNode(t);
				}
				mathExpCtrl1.RefreshVariableMap();
				if (newNode != null)
				{
					MathNodeValue nv = newNode as MathNodeValue;
					if (nv != null)
					{
						if (category == enumOperatorCategory.Integer)
						{
							nv.ValueType = new RaisDataType(typeof(Int32));
						}
						else if (category == enumOperatorCategory.Logic)
						{
							nv.ValueType = new RaisDataType(typeof(bool));
						}
						else if (category == enumOperatorCategory.String)
						{
							nv.ValueType = new RaisDataType(typeof(string));
						}
					}
					object[] vs = t.GetCustomAttributes(typeof(UseModalSelectorAttribute), true);
					if (vs != null && vs.Length > 0)
					{
						mathExpCtrl1.SetNodeSelection(newNode);
						mathExpCtrl1.DoDoubleClick();
					}
				}
			}
		}

		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (string.CompareOrdinal(e.ChangedItem.PropertyDescriptor.Name, "VariableName") == 0
				|| string.CompareOrdinal(e.ChangedItem.PropertyDescriptor.Name, "SubscriptName") == 0)
			{
				mathExpCtrl1.RefreshVariableMap();
			}
			if (e.ChangedItem.PropertyDescriptor.PropertyType.Equals(typeof(RaisDataType)))
			{
				propertyGrid1.Refresh();
			}
			mathExpCtrl1.Changed = true;
			mathExpCtrl1.Refresh();
		}
		void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			mathExpCtrl1.Changed = true;
		}
		void onValueChanged(object sender, EventArgs e)
		{
			mathExpCtrl1.Changed = true;
		}
		public void RefreshPropertyGrid()
		{
			propertyGrid1.Refresh();
		}
		public void ShowVariableMap(VariableMap map)
		{
			propertyGrid2.SelectedObject = map;
		}
		public void SaveToXmlNode(XmlNode node)
		{
			mathExpCtrl1.SaveToXmlNode(node);
		}
		public void LoadFromXmlNode(XmlNode node)
		{
			mathExpCtrl1.LoadFromXmlNode(node);
			mathNode = node;
		}
		private void mathExpCtrl1_OnSetFocus(object sender, EventArgs e)
		{
			if (sender != null)
				propertyGrid1.SelectedObject = sender;
			else
				propertyGrid1.SelectedObject = mathExpCtrl1.Root;
			onNodeSelectionChanged();
		}
		private void onNodeSelectionChanged()
		{
			mathExpCtrl1.Refresh();
			mathExpCtrl1.Focus();
		}
		private void enableUndoButtons()
		{
			const int IMG_UndoEnable = 2;
			const int IMG_UndoDisable = 3;
			const int IMG_RedoEnable = 4;
			const int IMG_RedoDisable = 5;
			btUndo.Enabled = mathExpCtrl1.HasUndo;
			if (btUndo.Enabled)
				btUndo.ImageIndex = IMG_UndoEnable;
			else
				btUndo.ImageIndex = IMG_UndoDisable;
			btRedo.Enabled = mathExpCtrl1.HasRedo;
			if (btRedo.Enabled)
				btRedo.ImageIndex = IMG_RedoEnable;
			else
				btRedo.ImageIndex = IMG_RedoDisable;
		}
		private void mathExpCtrl1_OnUndoStateChanged(object sender, EventArgs e)
		{
			enableUndoButtons();
		}

		private void btInsert_Click(object sender, EventArgs e)
		{
			mathExpCtrl1.AddEmptyNode();
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			if (mathExpCtrl1.FocusedNode == null)
			{
				mathExpCtrl1.SetNodeSelection(mathExpCtrl1.Root[1]);
			}
			mathExpCtrl1.DeleteSelectedNode();
			onNodeSelectionChanged();
			mathExpCtrl1.RefreshVariableMap();
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			mathExpCtrl1.Undo();
			onNodeSelectionChanged();
			mathExpCtrl1.RefreshVariableMap();
		}

		private void btRedo_Click(object sender, EventArgs e)
		{
			mathExpCtrl1.Redo();
			onNodeSelectionChanged();
			mathExpCtrl1.RefreshVariableMap();
		}
		private void copyToClipboard()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode node = doc.CreateElement(XmlSerialization.XML_Math);
			doc.AppendChild(node);
			if (mathExpCtrl1.FocusedNode == null)
			{
				mathExpCtrl1.SetNodeSelection(mathExpCtrl1.Root[1]);
			}
			mathExpCtrl1.FocusedNode.Save(node);
			Clipboard.SetData("MathNode", doc.OuterXml);
			if (Clipboard.ContainsData("MathNode"))
			{
				btPaste.Enabled = true;
				btPaste.ImageIndex = IMG_Paste_Enable;
			}
		}
		private void btCopy_Click(object sender, EventArgs e)
		{
			copyToClipboard();
		}

		private void btCut_Click(object sender, EventArgs e)
		{
			copyToClipboard();
			mathExpCtrl1.DeleteSelectedNode();
			mathExpCtrl1.RefreshVariableMap();
			//}
		}

		private void btPaste_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsData("MathNode"))
			{
				object data = Clipboard.GetData("MathNode");
				if (data != null)
				{
					string s = data.ToString();
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(s);
					if (doc.DocumentElement != null)
					{
						try
						{
							Type t = XmlUtil.GetLibTypeAttribute(doc.DocumentElement);
							MathNode node = mathExpCtrl1.AddMathNode(t);
							node.Load(doc.DocumentElement);
							mathExpCtrl1.RefreshVariableMap();
							mathExpCtrl1.Refresh();
							mathExpCtrl1.Focus();
						}
						catch (Exception err)
						{
							MessageBox.Show(err.Message);
						}
					}
				}
			}
		}
		public void ExportAllCodeStatements(MethodType method)
		{
			mathExpCtrl1.ExportAllCodeStatements(method);
		}
		public void GetAllImports(AssemblyRefList imports)
		{
			mathExpCtrl1.GetAllImports(imports);
		}
		public void GenerateInputVariables()
		{
			mathExpCtrl1.GenerateInputVariables();
		}
		public VariableList InputVariables
		{
			get
			{
				return mathExpCtrl1.InputVariables;
			}
		}
		public CodeExpression ExportCode(MethodType method)
		{
			return mathExpCtrl1.ExportCode(method);
		}
		public void AddService(Type t, object service)
		{
			mathExpCtrl1.AddService(t, service);
		}
		public bool Changed
		{
			get
			{
				return mathExpCtrl1.Changed;
			}
		}
		private bool _useXml;
		public bool SaveInXml
		{
			get
			{
				return _useXml;
			}
			set
			{
				_useXml = value;
			}
		}
		public XmlNode MathExpNode
		{
			get
			{
				return mathNode;
			}
		}
		/// <summary>
		/// when this math expression is used in an action, this is the action.
		/// if it is a property-setting action then the method is a set-property action;
		/// if it is a method execution action then this is the method-execution action.
		/// </summary>
		public IActionContext ActionContext
		{
			get
			{
				return _actionContext;
			}
			set
			{
				_actionContext = value;
				mathExpCtrl1.ActionContext = _actionContext;
			}
		}
		[Browsable(false)]
		public IMethod ScopeMethod
		{
			get
			{
				return mathExpCtrl1.ScopeMethod;
			}
			set
			{
				mathExpCtrl1.ScopeMethod = value;
			}
		}
		public void DisableTest()
		{
			btTest.Enabled = false;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			mathExpCtrl1.SaveLocations();
			mathExpCtrl1.Root.ClearFocus();
			if (OnOK != null)
			{
				if (_useXml)
				{
					if (mathNode == null)
					{
						XmlDocument doc = new XmlDocument();
						mathNode = doc.CreateElement(XmlSerialization.XML_Math);
						doc.AppendChild(mathNode);
					}
					else
					{
						mathNode.RemoveAll();
					}
					SaveToXmlNode(mathNode);
				}
				OnOK(this, null);
			}
		}

		private void btCancel_Click(object sender, EventArgs e)
		{
			if (OnCancel != null)
			{
				OnCancel(this, null);
			}
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			btTest.Enabled = false;
			try
			{
				//check if compilation is needed
				_testData = null;
				IObjectTypeUnitTester uc = (IObjectTypeUnitTester)MathNode.GetService(typeof(IObjectTypeUnitTester));
				if (uc != null)
				{
					_testData = uc.UseMemberTest(this);
				}
				if (_testData == null)
				{
					//
					CompileResult result = mathExpCtrl1.CreateMethodCompilerUnit("TestMathExpression", "Test", "TestMathExpression");
					//
					thTest = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(test));
					thTest.SetApartmentState(System.Threading.ApartmentState.STA);
					_testData = new TestData(result);//"Test", "TestMathExpression", variables, pointerList, code, imports);
					thTest.Start(_testData);
				}
				timer1.Enabled = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
			//
			timer1.Enabled = true;
		}
		private void test(object data)
		{
			dlgTest dlg = new dlgTest();
			if (dlg.LoadData((TestData)data))
			{
				dlg.TopMost = true;
				dlg.ShowDialog(null);
			}
			else
			{
				((TestData)data).Finished = true;
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (_testData == null)
				timer1.Enabled = false;
			else
			{
				if (_testData.Finished)
				{
					thTest = null;
					timer1.Enabled = false;
					btTest.Enabled = true;
				}
			}
		}
		public void AbortTest()
		{
			if (thTest != null)
			{
				if (thTest.ThreadState != System.Threading.ThreadState.Unstarted && thTest.ThreadState != System.Threading.ThreadState.Aborted && thTest.ThreadState != System.Threading.ThreadState.AbortRequested && thTest.ThreadState != System.Threading.ThreadState.Stopped && thTest.ThreadState != System.Threading.ThreadState.StopRequested)
				{
					thTest.Abort();
				}
			}
		}

		#region IUnitTestOwner Members

		public void AddMethod(CodeTypeDeclaration t, CodeNamespace ns, CodeMemberMethod testMethod, AssemblyRefList imports, VariableList parameters, List<IPropertyPointer> pointers)
		{
			DiagramDesignerHolder.CreateTestMethod(mathExpCtrl1.Root, t, ns, testMethod, imports, parameters, pointers);
		}

		IMathExpression IUnitTestOwner.MathExpression
		{
			get { return mathExpCtrl1.Root; }
		}

		#endregion
	}
}
