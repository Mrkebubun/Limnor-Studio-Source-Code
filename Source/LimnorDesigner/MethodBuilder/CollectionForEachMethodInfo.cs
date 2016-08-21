/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LimnorDesigner.Action;
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// foreach method of a collection
	/// </summary>
	public class CollectionForEachMethodInfo : SubMethodInfo
	{
		private DataTypePointer _itemType;
		private Type _collectionType;
		private ParameterInfo[] _parameters;
		private string _methodKey;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemType">type of each item</param>
		/// <param name="methodKey">method id (codeName, name of the method?)</param>
		public CollectionForEachMethodInfo(DataTypePointer itemType, Type collectionType, string methodKey)
		{
			_collectionType = collectionType;
			_itemType = itemType;
			_methodKey = methodKey;
		}
		public override Type ReflectedType
		{
			get { return _collectionType; }
		}
		private string codeName
		{
			get
			{
				return _methodKey;
			}
		}
		public override Type ItemType
		{
			get
			{
				if (_itemType.IsGenericParameter && _itemType.ConcreteType != null)
				{
					return _itemType.ConcreteType.BaseClassType;
				}
				return _itemType.BaseClassType;
			}
		}
		public override bool IsForeach { get { return true; } }
		public override bool SaveParameterValues { get { return false; } }
		/// <summary>
		/// parameters of the method
		/// </summary>
		/// <returns></returns>
		public override ParameterInfo[] GetParameters()
		{
			if (_parameters == null)
			{
				_parameters = new ParameterInfo[1];
				_parameters[0] = new SubMethodParameterInfo("value", _itemType.BaseClassType, codeName, "Current collection item");
			}
			return _parameters;
		}

		public override string Name
		{
			get { return SubMethodInfo.ExecuteForEachItem; }
		}
		public override Type ReturnType
		{
			get
			{
				return typeof(void);
			}
		}
		public override Type DeclaringType
		{
			get { return _collectionType; }
		}

		public override Dictionary<string, string> IndexCodeNames
		{
			get
			{
				return null;
			}
		}
		public override ParameterClassSubMethod GetParameterType(UInt32 id, SubMethodInfoPointer owner, AB_SubMethodAction action)
		{
			ParameterClassSubMethod p;
			ParameterInfo[] ps = GetParameters();

			p = new ParameterClassCollectionItem(ps[0].ParameterType, ps[0].Name, action);
			p.ParameterID = (UInt32)(ps[0].GetHashCode());
			p.Method = owner;
			p.ActionId = owner.ActionOwner.ActionId;
			return p;
		}
		/// <summary>
		/// indices for accessing array item
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public override CodeExpression[] GetIndexCodes(MethodInfoPointer owner, IMethodCompile method, UInt32 branchId)
		{
			return null;
		}
		public override string GetIndexCodeJS(MethodInfoPointer owner, UInt32 branchId)
		{
			return null;
		}
		public override string[] GetIndexCodesJS(MethodInfoPointer owner, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override string[] GetIndexCodesPhp(MethodInfoPointer owner, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override string GetIndexCodePHP(MethodInfoPointer owner, UInt32 branchId)
		{
			return null;
		}
		public override string GetValueCodePHP(MethodInfoPointer owner, UInt32 branchId)
		{
			return null;
		}
		public override CodeStatement GetInitStatement(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			return null;
		}
		public override string GetInitStatementJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override string GetInitStatementPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override CodeExpression GetTestExpression(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			return null;
		}
		public override string GetTestExpressionJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override string GetTestExpressionPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override CodeStatement GetIncrementalStatement(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			return null;
		}
		public override string GetIncrementalStatementJS(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
		public override string GetIncrementalStatementPhp(MethodInfoPointer owner, StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data, UInt32 branchId)
		{
			return null;
		}
	}
}
