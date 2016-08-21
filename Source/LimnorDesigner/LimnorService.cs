/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.ComponentModel;
using System.CodeDom;
using System.Configuration.Install;
using System.Collections.Specialized;
using System.IO;
using VSPrj;
using System.Drawing;
using System.Globalization;

namespace LimnorDesigner
{
	[ToolboxBitmap(typeof(LimnorService), "Resources.winService.bmp")]
	[ProjectMainComponent(EnumProjectType.WinService)]
	public class LimnorService : LimnorApp
	{
		private System.ServiceProcess.ServiceAccount _accountType = ServiceAccount.LocalSystem;
		public LimnorService()
		{
			ThreadMode = EnumThreadMode.None;
		}
		void createService()
		{
			CodeTypeDeclaration td = new CodeTypeDeclaration("ProjectInstaller");
			typeNamespace.Types.Add(td);
			//
			td.BaseTypes.Add(typeof(Installer));
			//
			td.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(RunInstallerAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(true))));
			//
			CodeConstructor cc = new CodeConstructor();
			td.Members.Add(cc);
			cc.Attributes = MemberAttributes.Public;
			cc.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "InitializeComponent"));
			//
			CodeMemberField mf = new CodeMemberField(typeof(System.ComponentModel.IContainer), "components");
			td.Members.Add(mf);
			mf = new CodeMemberField(typeof(System.ServiceProcess.ServiceProcessInstaller), "serviceProcessInstaller1");
			td.Members.Add(mf);
			mf = new CodeMemberField(typeof(System.ServiceProcess.ServiceInstaller), "serviceInstaller1");
			td.Members.Add(mf);
			//
			CodeMemberMethod mm = new CodeMemberMethod();
			td.Members.Add(mm);
			mm.Name = "InitializeComponent";
			mm.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), "serviceProcessInstaller1"),
				new CodeObjectCreateExpression(typeof(System.ServiceProcess.ServiceProcessInstaller))));
			mm.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), "serviceInstaller1"),
				new CodeObjectCreateExpression(typeof(System.ServiceProcess.ServiceInstaller))));
			//
			//===set properties========
			mm.Statements.Add(new CodeCommentStatement(new CodeComment()));
			mm.Statements.Add(new CodeCommentStatement(new CodeComment("serviceProcessInstaller1")));
			mm.Statements.Add(new CodeCommentStatement(new CodeComment()));
			//
			CodeAssignStatement cas = new CodeAssignStatement(
				new CodePropertyReferenceExpression(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						"serviceProcessInstaller1"), "Account"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.ServiceProcess.ServiceAccount)), Account.ToString())
				);
			mm.Statements.Add(cas);
			//
			mm.Statements.Add(new CodeCommentStatement(new CodeComment()));
			mm.Statements.Add(new CodeCommentStatement(new CodeComment("serviceInstaller1")));
			mm.Statements.Add(new CodeCommentStatement(new CodeComment()));
			//
			cas = new CodeAssignStatement(
				new CodePropertyReferenceExpression(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						"serviceInstaller1"), "Description"),
				new CodePrimitiveExpression(Description)
				);
			mm.Statements.Add(cas);
			//
			cas = new CodeAssignStatement(
				new CodePropertyReferenceExpression(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						"serviceInstaller1"), "DisplayName"),
				new CodePrimitiveExpression(DisplayName)
				);
			mm.Statements.Add(cas);
			//
			cas = new CodeAssignStatement(
				new CodePropertyReferenceExpression(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						"serviceInstaller1"), "ServiceName"),
				new CodePrimitiveExpression(ServiceName)
				);
			mm.Statements.Add(cas);
			//
			cas = new CodeAssignStatement(
				new CodePropertyReferenceExpression(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						"serviceInstaller1"), "StartType"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.ServiceProcess.ServiceStartMode)), StartType.ToString())
				);
			mm.Statements.Add(cas);
			//
			//=========================
			//
			mm.Statements.Add(new CodeCommentStatement(new CodeComment()));
			mm.Statements.Add(new CodeCommentStatement(new CodeComment("ProjectInstaller")));
			mm.Statements.Add(new CodeCommentStatement(new CodeComment()));
			CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression();
			mm.Statements.Add(mie);
			mie.Method = new CodeMethodReferenceExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Installers"), "AddRange");
			CodeArrayCreateExpression ar = new CodeArrayCreateExpression();
			ar.CreateType = new CodeTypeReference(typeof(System.Configuration.Install.Installer[]));
			ar.Initializers.AddRange(new CodeExpression[] {
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"serviceProcessInstaller1"),
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"serviceInstaller1")
            });
			mie.Parameters.Add(ar);
			//
			mm = new CodeMemberMethod();
			td.Members.Add(mm);
			mm.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			mm.Name = "Dispose";
			mm.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "disposing"));
			CodeBinaryOperatorExpression boe = new CodeBinaryOperatorExpression();
			boe.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "components");
			boe.Operator = CodeBinaryOperatorType.IdentityInequality;
			boe.Right = new CodePrimitiveExpression(null);
			CodeBinaryOperatorExpression boe2 = new CodeBinaryOperatorExpression();
			boe2.Left = new CodeArgumentReferenceExpression("disposing");
			boe2.Operator = CodeBinaryOperatorType.BooleanAnd;
			boe2.Right = boe;
			CodeConditionStatement ccs = new CodeConditionStatement();
			mm.Statements.Add(ccs);
			ccs.Condition = boe2;
			ccs.TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "components"), "Dispose")));
			mm.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
				new CodeBaseReferenceExpression(), "Dispose", new CodeArgumentReferenceExpression("disposing")
				)));

		}
		[Description("Indicates the account type under which the service will run")]
		public ServiceAccount Account
		{
			get
			{
				return _accountType;
			}
			set
			{
				_accountType = value;
			}
		}
		[Description("Indicates the name used by the system to identify this service")]
		public string ServiceName
		{
			get;
			set;
		}
		[Description("Indicates the friendly name that identifies the service to the user.")]
		public string DisplayName
		{
			get;
			set;
		}
		[Description("Indicates the service's description (a brief comment that explains the purpose of the service).")]
		public string Description
		{
			get;
			set;
		}
		[Description("Indicates how and when this service is started.")]
		public ServiceStartMode StartType
		{
			get;
			set;
		}
		protected override void OnExportCode(bool forDebug)
		{
			foreach (CodeTypeMember cs in typeDeclaration.Members)
			{
				CodeMemberMethod cmm = cs as CodeMemberMethod;
				if (cmm != null)
				{
					if (string.CompareOrdinal(cmm.Name, "InitializeComponent") == 0)
					{
						typeDeclaration.Members.Remove(cs);
						break;
					}
				}
			}
			//
			StringCollection sc = new StringCollection();
			foreach (ComponentID cid in rootClasses)
			{
				if (typeof(ServiceBase).IsAssignableFrom(cid.ComponentType))
				{
					sc.Add(cid.TypeString);
				}
			}
			if (sc.Count > 0)
			{
				CodeVariableDeclarationStatement vds = new CodeVariableDeclarationStatement();
				vds.Name = "ServicesToRun";
				vds.Type = new CodeTypeReference(typeof(ServiceBase[]));
				CodeArrayCreateExpression ace = new CodeArrayCreateExpression();
				vds.InitExpression = ace;
				ace.CreateType = vds.Type;

				CodeExpression[] items = new CodeExpression[sc.Count];
				for (int i = 0; i < sc.Count; i++)
				{
					items[i] = new CodeObjectCreateExpression(sc[i]);
				}
				ace.Initializers.AddRange(items);
				//
				mainMethod.Statements.Add(vds);
				//
				mainMethod.Statements.Add(new CodeExpressionStatement(
					new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(ServiceBase)), "Run", new CodeVariableReferenceExpression("ServicesToRun"))
					));
				//
				//create cmd files for install/uninstall
				string netF = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
				string inst = System.IO.Path.Combine(netF, "InstallUtil.exe");
				string cmdInst = System.IO.Path.Combine(OutputFullPath, "install.cmd");
				if (System.IO.File.Exists(cmdInst))
				{
					System.IO.File.Delete(cmdInst);
				}
				if (!System.IO.Directory.Exists(OutputFullPath))
				{
					System.IO.Directory.CreateDirectory(OutputFullPath);
				}
				StreamWriter sw = new StreamWriter(cmdInst);
				sw.WriteLine("REM This command is for installing this Windows service");
				sw.WriteLine("REM Change the path to InstallUtil.exe according to your Windows installation");
				sw.WriteLine("REM It must run as administrator");
				sw.WriteLine("");
				sw.WriteLine("\"" + inst + "\" \"" + OutputFilename + ".exe\"");
				sw.Close();
				//
				cmdInst = System.IO.Path.Combine(OutputFullPath, "uninstall.cmd");
				if (System.IO.File.Exists(cmdInst))
				{
					System.IO.File.Delete(cmdInst);
				}
				sw = new StreamWriter(cmdInst);
				sw.WriteLine("REM This command is for uninstalling this Windows service");
				sw.WriteLine("REM Change the path to InstallUtil.exe according to your Windows installation");
				sw.WriteLine("REM It must run as administrator");
				sw.WriteLine("");
				sw.WriteLine("\"" + inst + "\" /u \"" + OutputFilename + ".exe\"");
				sw.Close();
			}
			//
			createService();
			//
		}

		public override void Run()
		{

		}
	}
}
