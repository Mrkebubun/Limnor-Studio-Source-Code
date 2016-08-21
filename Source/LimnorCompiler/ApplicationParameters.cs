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
using System.Xml;
using LimnorDesigner;
using XmlUtility;

namespace LimnorCompiler
{
	class ApplicationParameters : MarshalByRefObject
	{
		private string _appCodeName;
		private string[] _sessionVarNames;
		private string[] _sessionVarValues;
		private int _sessionTimeoutMinutes = 20;
		private int _sessionVarCount;
		public ApplicationParameters()
		{
			_sessionVarNames = new string[] { };
			_sessionVarValues = new string[] { };
		}
		public void GetParameters(string projectFile, string classFile)
		{
			bool loadedHere = false;
			LimnorProject _project = new LimnorProject(projectFile);
			XmlDocument doc = new XmlDocument();
			doc.Load(classFile);
			UInt32 classId = XmlUtil.GetAttributeUInt(doc.DocumentElement, XmlTags.XMLATT_ClassID);
			ClassPointer appClassPointer = _project.GetTypedData<ClassPointer>(classId);
			if (appClassPointer == null)
			{
				loadedHere = true;
				appClassPointer = ClassPointer.CreateClassPointer(_project, doc.DocumentElement);
			}
			if (appClassPointer.ObjectList.Count == 0)
			{
				appClassPointer.ObjectList.LoadObjects();
			}
			_appCodeName = appClassPointer.CodeName;
			LimnorWebApp webapp = appClassPointer.ObjectInstance as LimnorWebApp;
			if (webapp != null)
			{
				_sessionVarCount = webapp.GlobalVariables.Count;
				if (webapp.GlobalVariables.Count > 0)
				{
					Dictionary<string, string> sessionVars = new Dictionary<string, string>();
					foreach (SessionVariable sv in webapp.GlobalVariables)
					{
						if (!sessionVars.ContainsKey(sv.Name))
						{
							if (!sv.Value.IsDefaultValue())
							{
								sessionVars.Add(sv.Name, sv.Value.GetValueString());
							}
						}
					}
					_sessionVarNames = new string[sessionVars.Count];
					_sessionVarValues = new string[sessionVars.Count];
					int idx = 0;
					foreach (KeyValuePair<string, string> kv in sessionVars)
					{
						_sessionVarNames[idx] = kv.Key;
						_sessionVarValues[idx] = kv.Value;
						idx++;
					}
				}
				_sessionTimeoutMinutes = webapp.GlobalVariableTimeout;
				if (_sessionTimeoutMinutes <= 0)
				{
					_sessionTimeoutMinutes = 20;
				}
			}
			if (loadedHere)
			{
				_project.RemoveTypedData<ClassPointer>(classId);
			}
		}
		public int SessionVarCount
		{
			get
			{
				return _sessionVarCount;
			}
		}
		public int SessionTimeoutMinutes
		{
			get
			{
				return _sessionTimeoutMinutes;
			}
		}
		public string[] SessionVarNames
		{
			get
			{
				return _sessionVarNames;
			}
		}
		public string[] SessionVarValues
		{
			get
			{
				return _sessionVarValues;
			}
		}
		public string AppCodeName
		{
			get
			{
				return _appCodeName;
			}
		}
	}
}
