/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using LimnorDesigner;

namespace LimnorCompiler
{
	class ClassMemberField
	{
		private CodeMemberField _memberField;
		private ClassInstancePointer _classRef;
		public ClassMemberField(CodeMemberField member, ClassInstancePointer classRef)
		{
			_memberField = member;
			_classRef = classRef;
		}
		public CodeMemberField MemberField
		{
			get
			{
				return _memberField;
			}
		}
		public ClassInstancePointer Class
		{
			get
			{
				return _classRef;
			}
		}
	}
}
