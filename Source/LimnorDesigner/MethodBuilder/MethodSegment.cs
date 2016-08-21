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

namespace LimnorDesigner
{
	/// <summary>
	/// CodeStatmentCollection as part of a method,
	/// used for a goto branch
	/// </summary>
	public class MethodSegment
	{
		private CodeStatementCollection _statements;
		public MethodSegment(CodeStatementCollection statements)
		{
			_statements = statements;
		}
		public CodeStatementCollection Statements
		{
			get
			{
				return _statements;
			}
		}
		/// <summary>
		/// whether all branches ended with method return or go to.
		/// </summary>
		public bool Completed
		{
			get;
			set;
		}
	}
}
