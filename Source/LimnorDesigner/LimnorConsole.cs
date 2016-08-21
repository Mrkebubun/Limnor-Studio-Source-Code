/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using VSPrj;
using System.ComponentModel;
using LimnorKiosk;
using System.CodeDom;
using WindowsUtility;

namespace LimnorDesigner
{
	[ProjectMainComponent(EnumProjectType.Console)]
	[ToolboxBitmap(typeof(LimnorConsole), "Resources.console2.bmp")]
	[Serializable]
	public class LimnorConsole : LimnorApp
	{
		[Description("Occurs when the console application starts. Actions linked to this event will be executed when the application starts")]
		public static event fnSimpleFunction Start;
		public LimnorConsole()
		{
		}
		/// <summary>
		/// generate code 
		/// </summary>
		protected override void OnExportCode(bool forDebug)
		{
			foreach (CodeTypeMember cs in typeDeclaration.Members)
			{
				CodeMemberMethod cmm = cs as CodeMemberMethod;
				if (cmm != null)
				{
					if (string.CompareOrdinal(cmm.Name, "InitializeComponent") == 0)
					{
						mainMethod.Statements.Add(new CodeMethodInvokeExpression(null, "InitializeComponent"));
						break;
					}
				}
			}
		}
		public override void Run()
		{
			if (Start != null)
			{
				Start();
			}
		}
	}
}
