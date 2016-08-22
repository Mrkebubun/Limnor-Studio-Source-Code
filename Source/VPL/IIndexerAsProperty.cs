/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.ComponentModel;

namespace VPL
{
	/// <summary>
	/// x.PropertyName should be coded as x[PropertyName]
	/// </summary>
	public interface IIndexerAsProperty
	{
		bool IsIndexer(string name);
		Type IndexerDataType(string name);
		CodeExpression CreateCodeExpression(CodeExpression target, string name);
		string GetJavaScriptReferenceCode(string propOwner, string MemberName);
		string GetPhpScriptReferenceCode(string propOwner, string MemberName);
	}
}
