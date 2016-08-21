/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorCompiler
{
	public interface ICompilerLog
	{
		void LogError(string msg);
		void LogWarning(string msg, params object[] values);
		bool HasLoggedErrors { get; }
		void LogErrorFromException(Exception e);
	}
}
