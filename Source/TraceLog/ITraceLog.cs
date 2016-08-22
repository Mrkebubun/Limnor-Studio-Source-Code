/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Trace and log
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace TraceLog
{
	public interface ITraceLog
	{
		string Name { get; }
		bool ShowMessageBox { get; set; }
		void Log(Exception e);
		void Log(StringCollection messages);
		void Log(string message);
		void Log(Exception e, string message, params object[] values);
		void LogError(string message);
		void Trace(string message);
		void Trace(string message, params object[] values);
		void IndentIncrement();
		void IndentDecrement();
		void ResetIndent();
		string GetLogContents();
		void ClearLogContents();
		string Log(Form caller, Exception e, ref bool display);
		void Log(string message, ref bool display);
		string LogFile { get; }
		string ErrorFile { get; }
		bool LogFileExist { get; }
		bool ErrorFileExist { get; }
		string ExceptionMessage(Exception e);
		void ViewLog();
		void ViewError();
		void EnableWriteTime(bool enable);
		void TrimOneTime();
		void SwitchLogFiles(string filename);
		void RestoreLogFiles();
	}
}
