/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using MathExp;

namespace LimnorDesigner
{
	public interface ILimnorCodeCompiler
	{
		CodeTypeDeclaration TypeDeclaration { get; }
		ClassPointer ActionEventList { get; }
		string AppName { get; }
		string KioskBackFormName { get; }
		bool Debug { get; }
		void AddError(string error);
		bool SupportPageNavigator { get; }
	}
}
