/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using VSPrj;
using System.Xml;
using XmlSerializer;
using System.Drawing;
using System.CodeDom;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using VPL;
using Limnor.Drawing2D;

namespace LimnorDesigner
{
	public delegate void fnSimpleFunction2();
	[ProjectMainComponent(EnumProjectType.WinForm)]
	[ToolboxBitmap(typeof(LimnorWinApp), "Resources.form2.bmp")]
	[Serializable]
	public class LimnorWinApp : LimnorApp, IXmlNodeHolder
	{
		#region fields and constructors
		protected ComponentPointer _startForm;
		//
		private UInt32 _startClassId; //use default-instance techniue, class id is enough
		private ClassTypePointer _startObject;
		//
		[Description("Occurs when trying to start this program and an instance of this program is already running. This event is enabled only when OneInstanceOnly or EnableDuplicationDetectedEvent are true.")]
		public static event fnSimpleFunction2 DuplicationDetected;
		private CodeStatementCollection _duplicationHandlers;
		//
		[Description("Occurs before the first page is displayed.")]
		public static event fnSimpleFunction2 BeforeStart
		{
			add { }
			remove { }
		}
		public LimnorWinApp()
		{
			if (DuplicationDetected != null)
			{
			}
		}
		#endregion
		#region methods
		[NotForProgramming]
		[Browsable(false)]
		public void SetDuplicationHandlers(CodeStatementCollection statements)
		{
			_duplicationHandlers = statements;
		}
		public bool UseDuplicationHandler()
		{
			return (_duplicationHandlers != null);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetBeforeStartHandlers(CodeStatementCollection statements)
		{
			int nInitializeComponent = -1;
			int nRun = -1;
			int nEnableVisualStyles = -1;
			for (int i = 0; i < mainMethod.Statements.Count; i++)
			{
				CodeExpressionStatement ces = mainMethod.Statements[i] as CodeExpressionStatement;
				if (ces != null)
				{
					CodeMethodInvokeExpression cmie = ces.Expression as CodeMethodInvokeExpression;
					if (cmie != null)
					{
						if (string.CompareOrdinal(cmie.Method.MethodName, "InitializeComponent") == 0)
						{
							nInitializeComponent = i;
							break;
						}
						else if (string.CompareOrdinal(cmie.Method.MethodName, "Run") == 0)
						{
							nRun = i;
						}
						else if (string.CompareOrdinal(cmie.Method.MethodName, "EnableVisualStyles") == 0)
						{
							nEnableVisualStyles = i;
						}
					}
				}
			}
			int k = -1;
			if (nInitializeComponent >= 0)
			{
				k = nInitializeComponent + 1;
			}
			else if (nEnableVisualStyles >= 0)
			{
				k = nEnableVisualStyles + 1;
			}
			else if (nRun >= 0)
			{
				k = nRun;
			}
			for (int j = 0; j < statements.Count; j++, k++)
			{
				mainMethod.Statements.Insert(k, statements[j]);
			}
		}
		[NotForProgramming]
		protected virtual void OnBeforeCreateFirstForm()
		{
		}
		[NotForProgramming]
		protected virtual void OnCreateFirstFormDebug()
		{
			ClassPointer startForm = null;

			if (_startClassId != 0)
			{
				startForm = ClassPointer.CreateClassPointer(_startClassId, Project);
			}
			if (startForm != null)
			{
				CodeExpressionStatement cs = new CodeExpressionStatement(
					 new CodeMethodInvokeExpression(
						 new CodeTypeReferenceExpression(typeof(Application)), "Run",
						 new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(startForm.TypeString), DrawingPage.DEFAULTFORM)));
				mainMethod.Statements.Add(cs);
			}
			else if (_startForm != null)
			{
				string formName = DesignUtil.CreateUniqueName("form");
				mainMethod.Statements.Add(new CodeVariableDeclarationStatement(
					_startForm.GetTypeName(this.Namespace), formName, new CodeObjectCreateExpression(_startForm.GetTypeName(this.Namespace))
					));
				//
				mainMethod.Statements.Add(new CodeExpressionStatement(
					new CodeMethodInvokeExpression(
						DebuggerVar, "OnCreateComponent",
						new CodePrimitiveExpression(_startForm.ObjectKey),
						new CodeVariableReferenceExpression(formName)
						)
						)
					);
				//
				if (!InitializeComponentAdded())
				{
					CodeMethodReferenceExpression mre = new CodeMethodReferenceExpression();
					mre.MethodName = "InitializeComponent";
					mainMethod.Statements.Add(new CodeExpressionStatement(
						new CodeMethodInvokeExpression(mre, new CodeExpression[] { })
						));
				}
				//
				CodeExpressionStatement cs = new CodeExpressionStatement(
					 new CodeMethodInvokeExpression(
						 new CodeTypeReferenceExpression(typeof(Application)), "Run",
						 new CodeVariableReferenceExpression(formName)));
				mainMethod.Statements.Add(cs);
			}
			else
			{
				throw new DesignerException("StartForm property is not specified in the application class.");
			}
		}
		private bool InitializeComponentAdded()
		{
			for (int i = 0; i < mainMethod.Statements.Count; i++)
			{
				CodeExpressionStatement ces = mainMethod.Statements[i] as CodeExpressionStatement;
				if (ces != null)
				{
					CodeMethodInvokeExpression cmie = ces.Expression as CodeMethodInvokeExpression;
					if (cmie != null)
					{
						if (string.CompareOrdinal(cmie.Method.MethodName, "InitializeComponent") == 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		[NotForProgramming]
		protected virtual void OnCreateFirstForm()
		{
			ClassPointer startForm = null;

			if (_startClassId != 0)
			{
				startForm = ClassPointer.CreateClassPointer(_startClassId, Project);
			}
			if (startForm != null)
			{
				if (!InitializeComponentAdded())
				{
					CodeMethodReferenceExpression mre = new CodeMethodReferenceExpression();
					mre.MethodName = "InitializeComponent";
					mainMethod.Statements.Add(new CodeExpressionStatement(
						new CodeMethodInvokeExpression(mre, new CodeExpression[] { })
						));
				}
				CodeExpressionStatement cs = new CodeExpressionStatement(
					 new CodeMethodInvokeExpression(
						 new CodeTypeReferenceExpression(typeof(Application)), "Run",
						 new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(startForm.TypeString), DrawingPage.DEFAULTFORM)));
				mainMethod.Statements.Add(cs);
			}
			else if (_startForm != null)
			{
				CodeMethodReferenceExpression mre = new CodeMethodReferenceExpression();
				mre.MethodName = "InitializeComponent";
				mainMethod.Statements.Add(new CodeExpressionStatement(
					new CodeMethodInvokeExpression(mre, new CodeExpression[] { })
					));
				//
				CodeExpressionStatement cs = new CodeExpressionStatement(
					 new CodeMethodInvokeExpression(
						 new CodeTypeReferenceExpression(typeof(Application)), "Run",
						 new CodeVariableReferenceExpression(StartPage.Name)));
				//
				mainMethod.Statements.Add(cs);
			}
			else
			{
				throw new DesignerException("StartForm property is not specified in the application class.");
			}
		}
		[NotForProgramming]
		protected override void OnExportCode(bool forDebug)
		{
			CodeExpressionStatement cs;
			cs = new CodeExpressionStatement(
				new CodeMethodInvokeExpression(
					new CodeTypeReferenceExpression(typeof(Application)), "EnableVisualStyles"));
			mainMethod.Statements.Add(cs);
			cs = new CodeExpressionStatement(
				 new CodeMethodInvokeExpression(
					 new CodeTypeReferenceExpression(typeof(Application)), "SetCompatibleTextRenderingDefault", new CodePrimitiveExpression(false)));
			mainMethod.Statements.Insert(0, cs);
			//
			CodeAttachEventStatement aes = new CodeAttachEventStatement(
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(AppDomain)), "CurrentDomain"),
				"UnhandledException",
				new CodeDelegateCreateExpression(new CodeTypeReference(typeof(UnhandledExceptionEventHandler)), new CodeTypeReferenceExpression(typeof(VPLException)), "OnUnhandledException"));
			mainMethod.Statements.Insert(0, aes);
			aes = new CodeAttachEventStatement(
				new CodeTypeReferenceExpression(typeof(Application)),
				"ThreadException",
				new CodeDelegateCreateExpression(new CodeTypeReference(typeof(ThreadExceptionEventHandler)), new CodeTypeReferenceExpression(typeof(VPLException)), "OnApplicationThreadException"));
			mainMethod.Statements.Insert(0, aes);
			//
			cs = new CodeExpressionStatement(
				 new CodeMethodInvokeExpression(
					 new CodeTypeReferenceExpression(typeof(Application)), "SetUnhandledExceptionMode", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(UnhandledExceptionMode)), "CatchException")));
			mainMethod.Statements.Insert(0, cs);
			//
			for (int i = 0; i < typeDeclaration.Members.Count; i++)
			{
				CodeMemberMethod cmm = typeDeclaration.Members[i] as CodeMemberMethod;
				if (cmm != null)
				{
					if (string.CompareOrdinal(cmm.Name, "InitializeComponent") == 0)
					{
						mainMethod.Statements.Add(new CodeExpressionStatement(
						new CodeMethodInvokeExpression(null, "InitializeComponent")));
						break;
					}
				}
			}
			//
			OnBeforeCreateFirstForm();
			//
			if (_startForm != null)
			{
				_startForm.Namespace = Namespace;
			}
			if (forDebug)
			{
				OnCreateFirstFormDebug();
			}
			else
			{
				OnCreateFirstForm();
			}
		}
		public override void Run()
		{
			if (_startForm != null)
			{
				Form f = _startForm.ObjectInstance as Form;
				if (f != null)
				{
					f.Show();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="statements">handler statements</param>
		[NotForProgramming]
		public void AddSingleInstanceDetection()
		{
			if (OneInstanceOnly || UseDuplicationHandler())
			{
				//add it out side of Main
				//using System.Runtime.InteropServices;
				//using System.Diagnostics;
				//[DllImportAttribute("user32.dll")]
				//[return: MarshalAs(UnmanagedType.Bool)]
				//static extern bool SetForegroundWindow(IntPtr hWnd);
				string sMu = "m" + Guid.NewGuid().GetHashCode().ToString("x");
				CodeMemberField mu = new CodeMemberField(typeof(Mutex), sMu);
				typeDeclaration.Members.Add(mu);
				mu.Attributes = MemberAttributes.Static;
				CodeSnippetTypeMember s2 = new CodeSnippetTypeMember("        [DllImportAttribute(\"user32.dll\")]\r\n        static extern bool SetForegroundWindow(IntPtr hWnd);");
				typeDeclaration.Members.Add(s2);
				s2 = new CodeSnippetTypeMember("\r\n        [DllImportAttribute(\"user32.dll\")]\r\n        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);");
				typeDeclaration.Members.Add(s2);
				int n = 0;
				string sId = Guid.NewGuid().ToString();
				string sCreateNew = "b" + Guid.NewGuid().GetHashCode().ToString("x");
				mainMethod.Statements.Insert(n++, new CodeVariableDeclarationStatement(typeof(bool), sCreateNew));
				mainMethod.Statements.Insert(n++, new CodeAssignStatement(new CodeVariableReferenceExpression(sMu),
					new CodeObjectCreateExpression(
						typeof(Mutex),
						new CodePrimitiveExpression(true),
						new CodePrimitiveExpression(sId),
						new CodeSnippetExpression("out " + sCreateNew)
						)
					));
				string sTP = "p" + Guid.NewGuid().GetHashCode().ToString("x");
				CodeConditionStatement ccs = new CodeConditionStatement();
				mainMethod.Statements.Insert(n++, ccs);
				ccs.Condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(sCreateNew), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false));
				if (OneInstanceOnly)
				{
					string sP = "p" + Guid.NewGuid().GetHashCode().ToString("x");
					ccs.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(Process), sTP, new CodeMethodInvokeExpression(
									new CodeTypeReferenceExpression(typeof(Process)),
									"GetCurrentProcess",
									new CodeExpression[] { }
									)));
					ccs.TrueStatements.Add(new CodeVariableDeclarationStatement(
						typeof(Process[]), sP, new CodeMethodInvokeExpression(
							new CodeTypeReferenceExpression(typeof(Process)), "GetProcessesByName",
							new CodePropertyReferenceExpression(
								new CodeVariableReferenceExpression(sTP),
								"ProcessName"
								)
							)
						));
					CodeIterationStatement cis = new CodeIterationStatement();
					ccs.TrueStatements.Add(cis);
					string sIndex = "i";
					cis.InitStatement = new CodeVariableDeclarationStatement(typeof(int), sIndex, new CodePrimitiveExpression(0));
					cis.IncrementStatement = new CodeSnippetStatement("i++");
					cis.TestExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(sIndex), CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(sP), "Length"));
					CodeConditionStatement cc = new CodeConditionStatement();
					cis.Statements.Add(cc);
					CodeArrayIndexerExpression ae = new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(sP), new CodeVariableReferenceExpression(sIndex));
					cc.Condition = new CodeBinaryOperatorExpression(
						new CodeBinaryOperatorExpression(
						new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(ae, "Id"), CodeBinaryOperatorType.IdentityInequality, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(sTP), "Id")),
						 CodeBinaryOperatorType.BooleanAnd,
						 new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare",
							 new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(ae, "MainModule"), "FileName"),
							 new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(sTP), "MainModule"), "FileName"),
							 new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(StringComparison)), "OrdinalIgnoreCase")
							 ), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0))
						 ), CodeBinaryOperatorType.BooleanAnd,
						 new CodeBinaryOperatorExpression(
							 new CodePropertyReferenceExpression(ae, "MainWindowHandle"),
							 CodeBinaryOperatorType.IdentityInequality,
							 new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(IntPtr)), "Zero")
							 )
						 );
					cc.TrueStatements.Add(
						new CodeExpressionStatement(
							new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.Name),
								"SetForegroundWindow", new CodePropertyReferenceExpression(ae, "MainWindowHandle"))
							)
						);
					cc.TrueStatements.Add(
						new CodeExpressionStatement(
							new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.Name),
								"ShowWindow", new CodePropertyReferenceExpression(ae, "MainWindowHandle"), new CodePrimitiveExpression(9))
							)
						);
					cc.TrueStatements.Add(new CodeSnippetStatement("                        break;"));
				}
				if (_duplicationHandlers != null)
				{
					ccs.TrueStatements.AddRange(_duplicationHandlers);
				}
				if (OneInstanceOnly)
				{
					ccs.TrueStatements.Add(new CodeMethodReturnStatement());
				}
			}
		}
		#endregion
		#region Properties
		[NotForProgramming]
		[Browsable(false)]
		protected CodeFieldReferenceExpression DebuggerVar
		{
			get
			{
				return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeDeclaration.Name), LimnorDebugger.Debugger);
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		protected string StartFormType
		{
			get
			{
				if (_startClassId != 0)
				{
					ClassPointer startForm = ClassPointer.CreateClassPointer(_startClassId, Project);
					if (startForm != null)
					{
						return startForm.TypeString;
					}
				}
				return string.Empty;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public UInt32 StartClassId
		{
			get
			{
				return _startClassId;
			}
			set
			{
				_startClassId = value;
				if (_startClassId != 0)
				{
					_startForm = null;

				}
				if (_startObject == null)
				{
					_startObject = new ClassTypePointer();
				}
				_startObject.SetData(_startClassId, Project);
			}
		}
		[XmlIgnore]
		[Editor(typeof(TypeEditorClassType), typeof(UITypeEditor))]
		[Description("This is the first window to be created and displayed when this application starts.")]
		public ClassTypePointer StartForm
		{
			get
			{
				if (_startObject == null)
				{
					_startObject = new ClassTypePointer();
				}
				_startObject.SetData(_startClassId, Project);
				return _startObject;
			}
			set
			{
			}
		}
		/// <summary>
		/// obsolete
		/// </summary>
		[NotForProgramming]
		[Browsable(false)]
		[Editor(typeof(ComponentPointerSelector<Form>), typeof(UITypeEditor))]
		[Description("This is the first window to be created and displayed when this application starts.")]
		public virtual ComponentPointer StartPage
		{
			get
			{
				if (_startForm != null)
				{
					_startForm.AlwaysStatic = true;
				}
				return _startForm;
			}
			set
			{
				bool changed;
				if (_startForm == null && value != null)
					changed = true;
				else if (_startForm != null && value == null)
					changed = true;
				else if (_startForm != null && value != null)
				{
					changed = (_startForm.WholeId != value.WholeId);
				}
				else
					changed = false;
				if (changed)
				{
					_startForm = value;
					if (_startForm != null)
					{
						_startForm.AlwaysStatic = true;
					}
				}
			}
		}
		[NotForProgramming]
		[Description("Indicates whether multiple instances can run at the same time")]
		public virtual bool OneInstanceOnly { get; set; }

		#endregion
		#region IXmlNodeHolder Members
		private XmlNode _xmldata;
		[NotForProgramming]
		[Browsable(false)]
		[ReadOnly(true)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _xmldata;
			}
			set
			{
				_xmldata = value;
			}
		}

		#endregion
	}
}
