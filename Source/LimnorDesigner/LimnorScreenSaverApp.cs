/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.ComponentModel;
using System.CodeDom;
using System.Windows.Forms;
using Limnor.Drawing2D;
using WindowsUtility;
using System.Diagnostics.CodeAnalysis;

namespace LimnorDesigner
{
	[ProjectMainComponent(EnumProjectType.ScreenSaver)]
	[Serializable]
	public class LimnorScreenSaverApp : LimnorWinApp
	{
		#region Events
		[Description("Occurs when screensaver configuration is requested")]
		public static event fnSimpleFunction ScreenSaverConfiguration
		{
			add { }
			remove { }
		}
		[Description("Occurs when screensaver preview is requested")]
		public static event fnSimpleFunction ScreenSaverPreview
		{
			add { }
			remove { }
		}
		#endregion
		static string _formBKName;

		string backgroundTypeName = "ScreenSaverBackground";
		public LimnorScreenSaverApp()
		{
			OneInstanceOnly = true;
			PreviewParenthandle = new IntPtr(0);
		}
		[Description("Gets a window handle representing the parent window for calling screensaver preview or configuration")]
		public IntPtr PreviewParenthandle { get; private set; }

		[Browsable(false)]
		public override bool OneInstanceOnly { get; set; }
		[Browsable(false)]
		public override ComponentPointer StartPage { get; set; }
		//
		public static string formBKName
		{
			get
			{
				if (string.IsNullOrEmpty(_formBKName))
				{
					_formBKName = DesignUtil.CreateUniqueName("formBK");
				}
				return _formBKName;
			}
		}
		protected override void OnBeforeCreateFirstForm()
		{
			//create background form variable
			CodeMemberField cmf = new CodeMemberField(typeof(ScreenSaverBackgroundForm[]), formBKName);
			cmf.Attributes = (MemberAttributes.Static | MemberAttributes.Public);
			typeDeclaration.Members.Add(cmf);
			//
			CodeMemberMethod mm = new CodeMemberMethod();
			typeDeclaration.Members.Add(mm);
			mm.Name = "OnConfigure";
			mm.Attributes = MemberAttributes.Static;
			mm = new CodeMemberMethod();
			typeDeclaration.Members.Add(mm);
			mm.Name = "OnPreview";
			mm.Attributes = MemberAttributes.Static;
			//
			CodeMethodReferenceExpression mre = new CodeMethodReferenceExpression();
			mre.MethodName = "InitializeComponent";
			mainMethod.Statements.Add(new CodeExpressionStatement(
				new CodeMethodInvokeExpression(mre, new CodeExpression[] { })
				));
			//
			//call ScreenSaverBackgroundForm.ParseScreensaverCommandLine(string[] args, out string cmd, out int parentHandle)
			CodeVariableDeclarationStatement cv1 = new CodeVariableDeclarationStatement();
			cv1.Type = new CodeTypeReference(typeof(int));
			cv1.Name = "parentHandle";
			mainMethod.Statements.Add(cv1);
			CodeVariableDeclarationStatement cv2 = new CodeVariableDeclarationStatement();
			cv2.Type = new CodeTypeReference(typeof(EnumScreenSaverStartType));
			cv2.Name = "sscmd";
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression();

			cmi.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(ScreenSaverBackgroundForm)), "ParseScreensaverCommandLine");
			cmi.Parameters.Add(new CodeVariableReferenceExpression("CommandArguments"));
			CodeDirectionExpression vr = new CodeDirectionExpression(FieldDirection.Out, new CodeVariableReferenceExpression("parentHandle"));
			cmi.Parameters.Add(vr);

			cv2.InitExpression = cmi;
			mainMethod.Statements.Add(cv2);
			//
			CodeTypeReferenceExpression sss = new CodeTypeReferenceExpression(typeof(EnumScreenSaverStartType));
			//
			CodeVariableReferenceExpression sscmd = new CodeVariableReferenceExpression("sscmd");
			CodeConditionStatement ccs = new CodeConditionStatement();
			mainMethod.Statements.Add(ccs);
			ccs.Condition = new CodeBinaryOperatorExpression(sscmd, CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(sss, "Config"));
			CodeMethodInvokeExpression cmi2 = new CodeMethodInvokeExpression();
			cmi2.Method = new CodeMethodReferenceExpression();
			cmi2.Method.MethodName = "OnConfigure";
			ccs.TrueStatements.Add(new CodeExpressionStatement(cmi2));
			//
			CodeConditionStatement ccs2 = new CodeConditionStatement();
			ccs.FalseStatements.Add(ccs2);
			ccs2.Condition = new CodeBinaryOperatorExpression(sscmd, CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(sss, "Preview"));
			//
			CodeConditionStatement ccs3 = new CodeConditionStatement();
			ccs2.FalseStatements.Add(ccs3);
			ccs3.Condition = new CodeBinaryOperatorExpression(sscmd, CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(sss, "Password"));
			//
			CodeConditionStatement ccs4 = new CodeConditionStatement();
			ccs3.FalseStatements.Add(ccs4);
			ccs4.Condition = new CodeBinaryOperatorExpression(sscmd, CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(sss, "Start"));

			//
			CodeAssignStatement code = new CodeAssignStatement();
			code.Left = new CodeVariableReferenceExpression(formBKName);
			CodePropertyReferenceExpression sl = new CodePropertyReferenceExpression(
			   new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Screen)), "AllScreens"), "Length");
			CodeArrayCreateExpression ac = new CodeArrayCreateExpression(typeof(ScreenSaverBackgroundForm[]), sl);
			code.Right = ac;
			ccs4.TrueStatements.Add(code);
			//
			CodeMethodInvokeExpression mim = new CodeMethodInvokeExpression();
			mim.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursor)), "Hide");
			ccs4.TrueStatements.Add(new CodeExpressionStatement(mim));
			//
			CodeIterationStatement cis = new CodeIterationStatement();
			ccs4.TrueStatements.Add(cis);

			CodeVariableDeclarationStatement ii = new CodeVariableDeclarationStatement();
			ii.Name = "i";
			ii.Type = new CodeTypeReference(typeof(int));
			ii.InitExpression = new CodePrimitiveExpression(0);
			cis.InitStatement = ii;
			cis.TestExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.LessThan, sl);
			cis.IncrementStatement = new CodeSnippetStatement("i++");
			CodeAssignStatement as1 = new CodeAssignStatement();
			as1.Left = new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(formBKName), new CodeVariableReferenceExpression("i"));
			as1.Right = new CodeObjectCreateExpression(BackGroundType, new CodeExpression[] { });
			cis.Statements.Add(as1);
			//
			CodeMethodInvokeExpression miv = new CodeMethodInvokeExpression();
			miv.Method = new CodeMethodReferenceExpression(as1.Left, "SetAllScreens");
			miv.Parameters.Add(new CodeVariableReferenceExpression(formBKName));
			cis.Statements.Add(new CodeExpressionStatement(miv));
			//
			CodeExpressionStatement cs;
			cs = new CodeExpressionStatement(
				  new CodeMethodInvokeExpression(
					  new CodeTypeReferenceExpression(typeof(Application)), "Run",
					  as1.Left));
			cis.Statements.Add(cs);
		}
		protected override void OnCreateFirstForm()
		{
			createFirstForm(false);
		}

		protected override void OnCreateFirstFormDebug()
		{
			createFirstForm(true);
		}
		private void createFirstForm(bool debug)
		{
		}

		[Browsable(false)]
		public string BackGroundType
		{
			get
			{
				return backgroundTypeName;
			}
			set
			{
				backgroundTypeName = value;
			}
		}

	}
}
