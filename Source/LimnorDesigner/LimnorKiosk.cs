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
using VSPrj;
using System.Drawing;
using LFilePath;
using System.Drawing.Design;
using System.CodeDom;
using LimnorKiosk;
using System.Windows.Forms;
using WindowsUtility;
using Limnor.Drawing2D;

namespace LimnorDesigner
{
	[ProjectMainComponent(EnumProjectType.Kiosk)]
	[Serializable]
	public class LimnorKioskApp : LimnorWinApp
	{
		public const string MethodName_LoadMainForm = "StartFirstForm";
		static string _formBKName;

		string backgroundTypeName = "KioskBackground";
		string _exitCode = "exit";
		Keys _hotKey = Keys.F2;
		public LimnorKioskApp()
		{
			OneInstanceOnly = true;
		}
		public static CodeFieldReferenceExpression GetBackgroundFormRef(string appName)
		{
			return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(appName), formBKName);
		}
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
		[Browsable(false)]
		public CodeFieldReferenceExpression BackgroundFormRef
		{
			get
			{
				return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(Name), formBKName);
			}
		}
		[Browsable(false)]
		public CodePropertyReferenceExpression CodeFirstForm
		{
			get
			{
				if (StartClassId == 0)
				{
					if (StartPage != null)
					{
						return new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(Name), this.StartPage.Name);
					}
					else
					{
						return null;
					}
				}
				else
				{
					return new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(StartFormType), DrawingPage.DEFAULTFORM);
				}
			}
		}
		protected override void OnBeforeCreateFirstForm()
		{
			//create background form variable
			CodeMemberField cmf = new CodeMemberField(this.BackGroundType, formBKName);
			cmf.Attributes = (MemberAttributes.Static | MemberAttributes.Public);
			typeDeclaration.Members.Add(cmf);
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
			CodeMemberMethod mm = new CodeMemberMethod();
			mm.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			mm.Name = MethodName_LoadMainForm;
			if (StartClassId != 0)
			{
				CodeAssignStatement casO = new CodeAssignStatement();
				casO.Left = new CodePropertyReferenceExpression(CodeFirstForm, "Owner");
				casO.Right = BackgroundFormRef;
				mm.Statements.Add(casO);
			}
			CodeStatement ccs = new CodeExpressionStatement(new CodeMethodInvokeExpression(
					CodeFirstForm, "Show"
					));
			mm.Statements.Add(ccs);
			ccs = new CodeExpressionStatement(new CodeMethodInvokeExpression(
					CodeFirstForm, "Focus"
					));
			mm.Statements.Add(ccs);
			typeDeclaration.Members.Add(mm);
			//
			if (!UseDefaultDesktopLocking)
			{
				mainMethod.Statements.Add(
					new CodeAssignStatement(
						new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(LKiosk)), "UseCustomKiosk"),
						new CodePrimitiveExpression(UseDefaultDesktopLocking)
					)
				);
			}
			mainMethod.Statements.Add(
				new CodeAssignStatement(
					BackgroundFormRef,
					new CodeObjectCreateExpression(BackGroundType, new CodeDelegateCreateExpression(
						new CodeTypeReference(typeof(fnSimpleFunction))
						, new CodeTypeReferenceExpression(typeDeclaration.Name)
						, "StartFirstForm"
						))

					)
				);
			//
			string sExitCode = this.ExitCode;
			if (string.IsNullOrEmpty(sExitCode))
			{
				sExitCode = "Exit";
			}
			mainMethod.Statements.Add(
				new CodeAssignStatement(
					new CodePropertyReferenceExpression(BackgroundFormRef, "ExitCode"),
					new CodePrimitiveExpression(sExitCode)
					)
				);

			mainMethod.Statements.Add(
				new CodeMethodInvokeExpression(
					BackgroundFormRef, "SetExitKey", new CodePrimitiveExpression(ExitHotKey.ToString())
					)
				);

			//
			if (debug)
			{
				mainMethod.Statements.Add(
					new CodeAttachEventStatement(
						BackgroundFormRef, "FormClosing",
						new CodeDelegateCreateExpression(
							new CodeTypeReference(typeof(FormClosingEventHandler)), DebuggerVar, "ExitDebug"
							)
						)
					);
			}
			//
			CodeExpressionStatement cs;
			cs = new CodeExpressionStatement(
				  new CodeMethodInvokeExpression(
					  new CodeTypeReferenceExpression(typeof(Application)), "Run",
					  BackgroundFormRef));
			mainMethod.Statements.Add(cs);
		}
		//It is assumed that your kiosk computers are setup using Windows SteadyState, see http://www.microsoft.com/windows/products/winfamily/sharedaccess/default.mspx.
		[Description("The computer is locked to prevent the users from directly accessing other applications while this kiosk application is running. To exit the kiosk application the user may press the hot key defined by the ExitHotKey property. It will ask the user for the Exit Code. The Exit Code the user enters must match the ExitCode property in order to shut down the kiosk.")]
		public string ExitCode
		{
			get
			{
				return _exitCode;
			}
			set
			{
				_exitCode = value;
			}
		}
		[Description("The computer is locked to prevent the users from directly accessing other applications while this kiosk application is running. To exit the kiosk application the user may press the hot key defined by the ExitHotKey property. It will ask the user for the Exit Code. The Exit Code the user enters must match the ExitCode property in order to shut down the kiosk.")]
		public Keys ExitHotKey
		{
			get
			{
				return _hotKey;
			}
			set
			{
				_hotKey = value;
			}
		}
		[DefaultValue(true)]
		[Description("The desktop has to be locked for the kiosk application. By default Limnor Studio provides a desktop-locking. If you want to use your own desktop-locking then set this property to False.")]
		public bool UseDefaultDesktopLocking
		{
			get
			{
				return LKiosk.UseCustomKiosk;
			}
			set
			{
				LKiosk.UseCustomKiosk = value;
			}
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
		[Description("Close this kiosk application")]
		public static void ExitKiosk()
		{
		}
	}
}
