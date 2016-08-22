/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Limnor.WebServerBuilder
{
	public class ServerDotNetCodeHolder
	{
		private CodeStatementCollection _OnRequestStart;
		private CodeStatementCollection _OnRequestFinish;
		private CodeStatementCollection _OnRequestGetData;
		private CodeStatementCollection _OnRequestPutData;
		private CodeStatementCollection _OnRequestExecution;

		public ServerDotNetCodeHolder()
		{
			_OnRequestStart = new CodeStatementCollection();
			_OnRequestFinish = new CodeStatementCollection();
			_OnRequestGetData = new CodeStatementCollection();
			_OnRequestPutData = new CodeStatementCollection();
			_OnRequestExecution = new CodeStatementCollection();
		}
		public CodeStatementCollection OnRequestStart { get { return _OnRequestStart; } }
		public CodeStatementCollection OnRequestFinish { get { return _OnRequestFinish; } }
		public CodeStatementCollection OnRequestGetData { get { return _OnRequestGetData; } }
		public CodeStatementCollection OnRequestPutData { get { return _OnRequestPutData; } }
		public CodeStatementCollection OnRequestExecution { get { return _OnRequestExecution; } }
		public void WriteCode(CodeTypeDeclaration sw)
		{
			if (_OnRequestStart.Count > 0)
			{
				write(sw, "OnRequestStart", _OnRequestStart);
			}
			//
			write(sw, "OnRequestFinish", _OnRequestFinish);
			//
			write(sw, "OnRequestGetData", _OnRequestGetData, "value");
			//
			write(sw, "OnRequestPutData", _OnRequestPutData, "value");
			//
			if (_OnRequestExecution.Count > 0)
			{
				CodeMemberMethod mm = write(sw, "OnRequestExecution", _OnRequestExecution, "method", "value");
				CodeMethodInvokeExpression cmie0 = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "OnRequestExecution", new CodeArgumentReferenceExpression("method"), new CodeArgumentReferenceExpression("value"));
				mm.Statements.Insert(0, new CodeExpressionStatement(cmie0));
			}
		}
		private CodeMemberMethod write(CodeTypeDeclaration td, string methodName, CodeStatementCollection sc, params string[] ps)
		{
			CodeMemberMethod mm = new CodeMemberMethod();
			mm.Attributes = MemberAttributes.Override | MemberAttributes.Family;
			mm.Name = methodName;
			if (ps != null && ps.Length > 0)
			{
				for (int i = 0; i < ps.Length; i++)
				{
					mm.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), ps[i]));
				}
			}
			mm.Statements.AddRange(sc);
			td.Members.Add(mm);
			return mm;
		}
	}
}
