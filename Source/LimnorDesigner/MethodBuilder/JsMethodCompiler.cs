/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using ProgElements;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	public class JsMethodCompiler
	{
		private MethodClass _mc;
		private ClassPointer _rootClassId;
		private Dictionary<UInt32, JsMethodSegment> _gotoBranches;
		private Dictionary<IMethod0, Dictionary<UInt32, JsMethodSegment>> _subbranches;
		private Dictionary<string, StringCollection> _formSubmissions;
		private bool _debug;
		public JsMethodCompiler(MethodClass m, ClassPointer root, bool debug, Dictionary<string, StringCollection> formSubmissions, bool isPhp)
		{
			_mc = m;
			_rootClassId = root;
			_debug = debug;
			_formSubmissions = formSubmissions;
			IsForPhp = isPhp;
		}
		public Dictionary<UInt32, JsMethodSegment> AddSubMethod(IMethod0 subMethod)
		{
			if (_subbranches == null)
			{
				_subbranches = new Dictionary<IMethod0, Dictionary<uint, JsMethodSegment>>();
			}
			Dictionary<UInt32, JsMethodSegment> branches;
			if (!_subbranches.TryGetValue(subMethod, out branches))
			{
				branches = new Dictionary<uint, JsMethodSegment>();
				_subbranches.Add(subMethod, branches);
			}
			return branches;
		}
		public void RemoveSubMethod(IMethod0 subMethod)
		{
			if (_subbranches != null)
			{
				if (_subbranches.ContainsKey(subMethod))
				{
					_subbranches.Remove(subMethod);
				}
			}
		}
		public void AddGotoBranch(UInt32 branchId, JsMethodSegment ms)
		{
			if (_gotoBranches == null)
			{
				_gotoBranches = new Dictionary<uint, JsMethodSegment>();
			}
			if (!_gotoBranches.ContainsKey(branchId))
			{
				_gotoBranches.Add(branchId, ms);
			}
		}
		public JsMethodSegment GetGotoBranch(UInt32 branchId)
		{
			if (_gotoBranches != null)
			{
				JsMethodSegment ms;
				if (_gotoBranches.TryGetValue(branchId, out ms))
				{
					return ms;
				}
			}
			return null;
		}
		public Dictionary<UInt32, JsMethodSegment> GetGotoBranches()
		{
			return _gotoBranches;
		}
		public bool Debug
		{
			get
			{
				return _debug;
			}
		}
		public Dictionary<string, StringCollection> FormSubmissions
		{
			get
			{
				return _formSubmissions;
			}
		}
		public ClassPointer ActionEventList
		{
			get
			{
				return _rootClassId;
			}
		}
		public bool IsForPhp { get; private set; }
	}
	public class JsMethodSegment
	{
		private StringCollection _statements;
		public JsMethodSegment(StringCollection statements)
		{
			_statements = statements;
		}
		public StringCollection Statements
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
