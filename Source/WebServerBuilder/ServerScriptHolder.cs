/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace Limnor.WebServerBuilder
{
	public class ServerScriptHolder
	{
		private StringCollection _OnRequestStart;
		private StringCollection _OnRequestClientData;
		private StringCollection _OnRequestFinish;
		private StringCollection _OnRequestGetData;
		private StringCollection _OnRequestPutData;
		private StringCollection _OnRequestExecution;
		private StringCollection _methods;

		public ServerScriptHolder()
		{
			_OnRequestStart = new StringCollection();
			_OnRequestClientData = new StringCollection();
			_OnRequestFinish = new StringCollection();
			_OnRequestGetData = new StringCollection();
			_OnRequestPutData = new StringCollection();
			_OnRequestExecution = new StringCollection();
			_methods = new StringCollection();
			//
			_OnRequestStart.Add("\tif ($this->DEBUG)\r\n\t{\r\n\t\techo \"PHP processor:\". __FILE__.\"<br>\";\r\n\t}\r\n");
		}
		public StringCollection OnRequestStart { get { return _OnRequestStart; } }
		public StringCollection OnRequestClientData { get { return _OnRequestClientData; } }
		public StringCollection OnRequestFinish { get { return _OnRequestFinish; } }
		public StringCollection OnRequestGetData { get { return _OnRequestGetData; } }
		public StringCollection OnRequestPutData { get { return _OnRequestPutData; } }
		public StringCollection OnRequestExecution { get { return _OnRequestExecution; } }
		public StringCollection Methods { get { return _methods; } }
		public void WriteCode(StreamWriter sw)
		{
			sw.WriteLine("protected function OnRequestStart()");
			sw.WriteLine("{");
			write(sw, _OnRequestStart);
			sw.WriteLine("}");
			//
			sw.WriteLine("protected function OnRequestClientData()");
			sw.WriteLine("{");
			write(sw, _OnRequestClientData);
			sw.WriteLine("}");
			//
			sw.WriteLine("protected function OnRequestFinish()");
			sw.WriteLine("{");
			write(sw, _OnRequestFinish);
			sw.WriteLine("}");
			//
			sw.WriteLine("protected function OnRequestGetData($value)");
			sw.WriteLine("{");
			write(sw, _OnRequestGetData);
			sw.WriteLine("}");
			//
			sw.WriteLine("protected function OnRequestPutData($value)");
			sw.WriteLine("{");
			write(sw, _OnRequestPutData);
			sw.WriteLine("}");
			//
			sw.WriteLine("protected function OnRequestExecution($method, $value)");
			sw.WriteLine("{");
			write(sw, _OnRequestExecution);
			sw.WriteLine("}");
			//
			write(sw, _methods);
		}
		private void write(StreamWriter sw, StringCollection sc)
		{
			for (int i = 0; i < sc.Count; i++)
			{
				sw.Write(sc[i]);
			}
			sw.WriteLine();
		}
	}
}
