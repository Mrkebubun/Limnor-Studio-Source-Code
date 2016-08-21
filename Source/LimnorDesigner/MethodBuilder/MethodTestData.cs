/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MathExp;
using System.Runtime.Serialization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// pass data from one domain to another to re-constructor test objects
	/// </summary>
	[Serializable]
	public class MethodTestData : ITestData, ISerializable
	{
		#region fields and constructors
		private bool _finished;
		private string _testNodeFullContents;
		private string _xpath;
		private string _methodName;
		private Type _methodCreator;//the type providing a static method to create test method
		public MethodTestData(string xmlFull, string testNodePath, string testMethod, Type methodCreator)
		{
			_testNodeFullContents = xmlFull;
			_xpath = testNodePath;
			_methodName = testMethod;
			_methodCreator = methodCreator;
		}
		#endregion
		#region methods
		#endregion
		#region properties
		public string FullContents
		{
			get
			{
				return _testNodeFullContents;
			}
		}
		public string XPath
		{
			get
			{
				return _xpath;
			}
		}
		public string MethodName
		{
			get
			{
				return _methodName;
			}
		}
		public Type MethodCreatorType
		{
			get
			{
				return _methodCreator;
			}
		}
		#endregion
		#region ITestData Members

		public bool Finished
		{
			get
			{
				return _finished;
			}
			set
			{
				_finished = value;
			}
		}

		#endregion
		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("x", _testNodeFullContents);
			info.AddValue("p", _xpath);
			info.AddValue("m", _methodName);
			info.AddValue("t", _methodCreator);
		}
		protected MethodTestData(SerializationInfo info, StreamingContext context)
		{
			_testNodeFullContents = info.GetString("x");
			_xpath = info.GetString("p");
			_methodName = info.GetString("m");
			_methodCreator = (Type)info.GetValue("t", typeof(Type));
		}
		#endregion
	}
}
