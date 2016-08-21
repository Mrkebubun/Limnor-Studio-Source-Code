/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.CodeDom;
using System.Reflection;

namespace LimnorCompiler
{
	class AssemblyInfoBuilder : CodeCompiler
	{
		public AssemblyInfoBuilder(LimnorXmlCompiler projectCompiler, LimnorProject project, string folder)
			: base(null, project, null, "AssemblyInfo", null, folder, null, null, false)
		{
			CodeCompileUnit code = new CodeCompileUnit();
			code.ReferencedAssemblies.Add("System.Reflection");
			code.ReferencedAssemblies.Add("System.Runtime.CompilerServices");
			code.ReferencedAssemblies.Add("System.Runtime.InteropServices");
			//
			if (!string.IsNullOrEmpty(projectCompiler.AssemblyTitle))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyTitleAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyTitle))));
			}
			if (!string.IsNullOrEmpty(projectCompiler.AssemblyDescription))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyDescriptionAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyDescription))));
			}
			if (!string.IsNullOrEmpty(projectCompiler.AssemblyProduct))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyProductAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyProduct))));
			}
			if (!string.IsNullOrEmpty(projectCompiler.AssemblyCompany))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyCompanyAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyCompany))));
			}
			if (!string.IsNullOrEmpty(projectCompiler.AssemblyCopyright))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyCopyrightAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyCopyright))));
			}
			if (string.IsNullOrEmpty(projectCompiler.AssemblyVersion))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyVersionAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression("1.0.0.0"))));
			}
			else
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyVersionAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyVersion))));
			}

			if (!string.IsNullOrEmpty(projectCompiler.AssemblyFileVersion))
			{
				code.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AssemblyFileVersionAttribute)),
					new CodeAttributeArgument(new CodePrimitiveExpression(projectCompiler.AssemblyFileVersion))));
			}
			generateCode(code);
		}
		public override bool Load()
		{
			return true;
		}
		public override string ComponentFile
		{
			get
			{
				return "AssemblyInfo";
			}
		}
		public override string ResourceFile
		{
			get
			{
				return string.Empty;
			}
		}
		public override string ResourceFileX
		{
			get
			{
				return string.Empty;
			}
		}
		public override string Resources
		{
			get
			{
				return string.Empty;
			}
		}
		public override string ResourcesX
		{
			get
			{
				return string.Empty;
			}
		}
		public override string SourceFileX
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
