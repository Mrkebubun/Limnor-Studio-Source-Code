/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Xml;
using XmlUtility;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Collections.Specialized;

namespace XmlSerializer
{
	/// <summary>
	/// write/read CodeStatementCollection to/from an XmlNode
	/// </summary>
	class CodeDomXml
	{
		#region XML Tags
		const string XMLATT_ArrayRank = "ArrayRank";
		const string XMLATT_Options = "Options";
		const string XMLATT_LineNumber = "LineNumber";
		const string XMLATT_FileName = "FileName";
		const string XMLATT_Label = "Label";
		const string XMLATT_LocalName = "LocalName";
		const string XMLATT_EventName = "EventName";
		const string XMLATT_DocComment = "DocComment";

		const string XML_Statements = "Statements";
		const string XML_LinePragma = "LinePragma";
		const string XML_ElementType = "ElementType";
		const string XML_TypeList = "TypeList";
		const string XML_Statement = "Statement";
		const string XML_Expression = "Expression";
		const string XML_EndDirectives = "EndDirectives";
		const string XML_StartDirectives = "StartDirectives";
		const string XML_CatchClauses = "CatchClauses";
		const string XML_FinallyStatements = "FinallyStatements";
		const string XML_TryStatements = "TryStatements";
		const string XML_Listener = "Listener";
		const string XML_TargetObject = "TargetObject";
		const string XML_IncrementStatement = "IncrementStatement";
		const string XML_InitStatement = "InitStatement";
		const string XML_TestExpression = "TestExpression";
		const string XML_FalseStatements = "FalseStatements";
		const string XML_TrueStatements = "TrueStatements";
		const string XML_Left = "Left";
		const string XML_Right = "Right";
		//
		const string XMLATT_ParameterName = "ParameterName";
		const string XMLATT_Size = "Size";
		const string XMLATT_Operator = "Operator";
		const string XMLATT_MethodName = "MethodName";
		const string XMLATT_Direction = "Direction";
		const string XMLATT_FieldName = "FieldName";
		const string XMLATT_PropertyName = "PropertyName";
		//
		const string XML_Initializers = "Initializers";
		const string XML_Indices = "Indices";
		const string XML_Expressions = "Expressions";
		const string XML_Types = "Types";
		const string XML_Arguments = "Arguments";
		const string XML_Attributes = "Attributes";
		//
		#endregion

		#region fields and constructors
		private ObjectIDmap _objMap;
		private XmlNode _rootXmlNode;
		private StringCollection _excludeVariables;
		private string _myname;
		public CodeDomXml(ObjectIDmap map, Type objectType, XmlNode node, string name)
		{
			_objMap = map;
			_myname = name;
			_excludeVariables = new StringCollection();
			_rootXmlNode = node.OwnerDocument.DocumentElement;
		}
		#endregion

		#region Write Statement
		private void WriteCodeLinePragma(CodeLinePragma code, XmlNode node)
		{
			if (code != null)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_LinePragma);
				//
				XmlUtil.SetAttribute(nd, XMLATT_LineNumber, code.LineNumber);
				XmlUtil.SetAttribute(nd, XMLATT_FileName, code.FileName);
			}
		}
		private void WriteTypeReferenceCollection(CodeTypeReferenceCollection code, XmlNode node, string name)
		{
			if (code != null)
			{
				if (code.Count > 0)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
					for (int i = 0; i < code.Count; i++)
					{
						XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nd.AppendChild(item);
						WriteTypeReference(code[i], item);
					}
				}
			}
		}
		private void WriteTypeReference(CodeTypeReference code, XmlNode node)
		{
			if (code != null)
			{
				Type t0 = Type.GetType(code.BaseType);
				if (t0 == null)
				{
					Assembly[] abs = AppDomain.CurrentDomain.GetAssemblies();
					for (int i = 0; i < abs.Length; i++)
					{
						t0 = abs[i].GetType(code.BaseType);
						if (t0 != null)
						{
							break;
						}
					}
				}

				XmlNode nd;
				XmlUtil.SetAttribute(node, XMLATT_ArrayRank, code.ArrayRank);
				if (code.ArrayElementType != null)
				{
					nd = XmlUtil.CreateSingleNewElement(node, XML_ElementType);
					WriteTypeReference(code.ArrayElementType, nd);
				}
				XmlNode ndBaseType = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_TYPE);
				if (t0 != null)
				{
					XmlUtil.SetLibTypeAttribute(ndBaseType, t0);
				}
				else
				{
					throw new XmlSerializerException("Could not resolve type [{0}]", code.BaseType);
				}
				if (code.TypeArguments != null && code.TypeArguments.Count > 0)
				{
					nd = XmlUtil.CreateSingleNewElement(node, XML_TypeList);
					for (int i = 0; i < code.TypeArguments.Count; i++)
					{
						XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nd.AppendChild(item);
						WriteTypeReference(code.TypeArguments[i], item);
					}
				}
				XmlUtil.SetAttribute(node, XMLATT_Options, code.Options);
			}
		}

		private bool WriteVariableDeclaration(CodeVariableDeclarationStatement code, XmlNode node)
		{
			if (code != null)
			{
				if (!isComponentName(code.Name))
				{
					if (string.CompareOrdinal(typeof(ComponentResourceManager).FullName, code.Type.BaseType) != 0)
					{
						XmlNode d = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
						XmlUtil.SetNameAttribute(d, code.Name);
						WriteTypeReference(code.Type, d);
						WriteCodeLinePragma(code.LinePragma, d);
						WriteCodeExpression(code.InitExpression, d, XML_Expression);
						return true;
					}
				}
			}
			return false;
		}
		private void WriteCodeSnippetStatement(CodeSnippetStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
				XmlCDataSection cd = nd.OwnerDocument.CreateCDataSection(XmlTags.XML_TEXTDATA);
				node.AppendChild(cd);
				cd.Value = code.Value;
			}
		}
		private void WriteCodeLabeledStatement(CodeLabeledStatement code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_Label, code.Label);
				WriteCodeLinePragma(code.LinePragma, node);
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_Statement);
				WriteStatement(code.Statement, nd, XML_Statement);
			}
		}
		private void WriteCodeDirective(CodeDirective code, XmlNode node)
		{
			if (code != null)
			{

			}
		}
		private void WriteCodeDirectiveCollection(CodeDirectiveCollection code, XmlNode node, string name)
		{
			if (code != null)
			{
				if (code.Count > 0)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
					for (int i = 0; i < code.Count; i++)
					{
						XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nd.AppendChild(item);
						WriteCodeDirective(code[i], item);
					}
				}
			}
		}
		private void WriteCodeCatchClause(CodeCatchClause code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_LocalName, code.LocalName);
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_ElementType);
				WriteTypeReference(code.CatchExceptionType, nd);
				WriteStatementCollection(code.Statements, node, XML_Statements);
			}
		}
		private void WriteCodeCatchClauseCollection(CodeCatchClauseCollection code, XmlNode node)
		{
			if (code != null && code.Count > 0)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XML_CatchClauses);
				for (int i = 0; i < code.Count; i++)
				{
					XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
					nd.AppendChild(item);
					WriteCodeCatchClause(code[i], item);
				}
			}
		}
		private void WriteCodeTryCatchFinallyStatement(CodeTryCatchFinallyStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeCatchClauseCollection(code.CatchClauses, node);
				WriteStatementCollection(code.FinallyStatements, node, XML_FinallyStatements);
				WriteStatementCollection(code.TryStatements, node, XML_TryStatements);
			}
		}
		private void WriteCodeThrowExceptionStatement(CodeThrowExceptionStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeExpression(code.ToThrow, node, XML_Expression);
			}
		}
		private void WriteCodeEventReferenceExpression(CodeEventReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_EventName, code.EventName);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
			}
		}
		private void WriteCodeRemoveEventStatement(CodeRemoveEventStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeEventReferenceExpression(code.Event, node);
				WriteCodeExpression(code.Listener, node, XML_Listener);
			}
		}
		private void WriteCodeMethodReturnStatement(CodeMethodReturnStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeExpression(code.Expression, node, XML_Expression);
			}
		}
		private void WriteCodeIterationStatement(CodeIterationStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteStatement(code.IncrementStatement, node, XML_IncrementStatement);
				WriteStatement(code.InitStatement, node, XML_InitStatement);
				WriteCodeExpression(code.TestExpression, node, XML_TestExpression);
				WriteStatementCollection(code.Statements, node, XML_Statements);
			}
		}
		private void WriteCodeGotoStatement(CodeGotoStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				XmlUtil.SetAttribute(node, XMLATT_Label, code.Label);
			}
		}
		private void WriteCodeExpressionStatement(CodeExpressionStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeExpression(code.Expression, node, XML_Expression);
			}
		}
		private void WriteCodeConditionStatement(CodeConditionStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeExpression(code.Condition, node, XML_Expression);
				WriteStatementCollection(code.FalseStatements, node, XML_FalseStatements);
				WriteStatementCollection(code.TrueStatements, node, XML_TrueStatements);
			}
		}
		private void WriteCodeComment(CodeComment code, XmlNode node)
		{
			if (code != null)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
				XmlUtil.SetAttribute(nd, XMLATT_DocComment, code.DocComment);
				XmlCDataSection cd = nd.OwnerDocument.CreateCDataSection(XmlTags.XML_TEXTDATA);
				nd.AppendChild(cd);
				cd.Value = code.Text;
			}
		}
		private void WriteCodeCommentStatement(CodeCommentStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeComment(code.Comment, node);
			}
		}
		private void WriteCodeAttachEventStatement(CodeAttachEventStatement code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeLinePragma(code.LinePragma, node);
				WriteCodeEventReferenceExpression(code.Event, node);
				WriteCodeExpression(code.Listener, node, XML_Listener);
			}
		}
		private CodeVariableReferenceExpression findTopOwner(CodeExpression code)
		{
			CodeVariableReferenceExpression c1 = code as CodeVariableReferenceExpression;
			if (c1 != null)
			{
				return c1;
			}
			CodePropertyReferenceExpression c2 = code as CodePropertyReferenceExpression;
			if (c2 != null)
			{
				return findTopOwner(c2.TargetObject);
			}
			CodeFieldReferenceExpression c3 = code as CodeFieldReferenceExpression;
			if (c3 != null)
			{
				return findTopOwner(c3.TargetObject);
			}
			return null;
		}
		private bool isComponent(CodeExpression code)
		{
			CodeVariableReferenceExpression c1 = findTopOwner(code);
			if (c1 != null)
			{
				return isComponentName(c1.VariableName);
			}
			return false;
		}
		private bool WriteCodeAssignStatement(CodeAssignStatement code, XmlNode node)
		{
			if (code != null)
			{
				if (!isComponent(code.Left))
				{
					WriteCodeLinePragma(code.LinePragma, node);
					WriteCodeExpression(code.Left, node, XML_Left);
					WriteCodeExpression(code.Right, node, XML_Right);
					return true;
				}
			}
			return false;
		}
		private bool WriteStatement(CodeStatement code, XmlNode node, string name)
		{
			if (code == null)
			{
				return false;
			}
			XmlUtil.SetLibTypeAttribute(node, code.GetType());
			if (code.EndDirectives != null && code.EndDirectives.Count > 0)
			{
				WriteCodeDirectiveCollection(code.EndDirectives, node, XML_EndDirectives);
			}
			if (code.StartDirectives != null && code.StartDirectives.Count > 0)
			{
				WriteCodeDirectiveCollection(code.StartDirectives, node, XML_StartDirectives);
			}
			CodeAssignStatement c = code as CodeAssignStatement;
			if (c != null)
			{
				return WriteCodeAssignStatement(c, node);
			}
			CodeExpressionStatement c5 = code as CodeExpressionStatement;
			if (c5 != null)
			{
				WriteCodeExpressionStatement(c5, node);
				return true;
			}
			CodeVariableDeclarationStatement c14 = code as CodeVariableDeclarationStatement;
			if (c14 != null)
			{
				return WriteVariableDeclaration(c14, node);
			}
			CodeAttachEventStatement c2 = code as CodeAttachEventStatement;
			if (c2 != null)
			{
				WriteCodeAttachEventStatement(c2, node);
				return true;
			}
			CodeCommentStatement c3 = code as CodeCommentStatement;
			if (c3 != null)
			{
				WriteCodeCommentStatement(c3, node);
				return true;
			}
			CodeConditionStatement c4 = code as CodeConditionStatement;
			if (c4 != null)
			{
				WriteCodeConditionStatement(c4, node);
				return true;
			}

			CodeGotoStatement c6 = code as CodeGotoStatement;
			if (c6 != null)
			{
				WriteCodeGotoStatement(c6, node);
				return true;
			}
			CodeIterationStatement c7 = code as CodeIterationStatement;
			if (c7 != null)
			{
				WriteCodeIterationStatement(c7, node);
				return true;
			}
			CodeLabeledStatement c8 = code as CodeLabeledStatement;
			if (c8 != null)
			{
				WriteCodeLabeledStatement(c8, node);
				return true;
			}
			CodeMethodReturnStatement c9 = code as CodeMethodReturnStatement;
			if (c9 != null)
			{
				WriteCodeMethodReturnStatement(c9, node);
				return true;
			}
			CodeRemoveEventStatement c10 = code as CodeRemoveEventStatement;
			if (c10 != null)
			{
				WriteCodeRemoveEventStatement(c10, node);
				return true;
			}
			CodeSnippetStatement c11 = code as CodeSnippetStatement;
			if (c11 != null)
			{
				WriteCodeSnippetStatement(c11, node);
				return true;
			}
			CodeThrowExceptionStatement c12 = code as CodeThrowExceptionStatement;
			if (c12 != null)
			{
				WriteCodeThrowExceptionStatement(c12, node);
				return true;
			}
			CodeTryCatchFinallyStatement c13 = code as CodeTryCatchFinallyStatement;
			if (c13 != null)
			{
				WriteCodeTryCatchFinallyStatement(c13, node);
				return true;
			}

			throw new XmlSerializerException("Unhandled statement: {0}", code.GetType());
		}
		private bool isComponentName(string name)
		{
			if (string.CompareOrdinal(name, _myname) == 0)
			{
				return false;
			}
			if (_excludeVariables.Contains(name))
			{
				return true;
			}
			object v = _objMap.GetObjectByName(name);
			if (v != null)
			{
				_excludeVariables.Add(name);
				return true;
			}
			XmlNode node = _rootXmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']",
					XmlTags.XML_Object, XmlTags.XMLATT_NAME, name));
			if (node != null)
			{
				_excludeVariables.Add(name);
				return true;
			}
			else
			{
				node = _rootXmlNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='Controls']//{2}[@{1}='{3}' and @{4}='True']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XmlTags.XML_Item, name, XmlTags.XMLATT_designMode));
				if (node != null)
				{
					_excludeVariables.Add(name);
					return true;
				}
			}
			return false;
		}
		public void WriteStatementCollection(CodeStatementCollection code, XmlNode node, string name)
		{
			if (code != null && code.Count > 0)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
				for (int i = 0; i < code.Count; i++)
				{
					XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
					if (WriteStatement(code[i], item, XML_Statement))
					{
						nd.AppendChild(item);
					}
				}
			}
		}
		#endregion

		#region Write Expression
		private void WriteCodeArgumentReferenceExpression(CodeArgumentReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_ParameterName, code.ParameterName);
			}
		}
		private void WriteCodeExpressionCollection(CodeExpressionCollection code, XmlNode node, string name)
		{
			if (code != null && code.Count > 0)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
				for (int i = 0; i < code.Count; i++)
				{
					XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
					nd.AppendChild(item);
					WriteCodeExpression(code[i], item, XML_Expression);
				}
			}
		}
		private void WriteCodeArrayCreateExpression(CodeArrayCreateExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteTypeReference(code.CreateType, node);
				WriteCodeExpressionCollection(code.Initializers, node, XML_Initializers);
				XmlUtil.SetAttribute(node, XMLATT_Size, code.Size);
				WriteCodeExpression(code.SizeExpression, node, XML_Expression);
			}
		}
		private void WriteCodeArrayIndexerExpression(CodeArrayIndexerExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeExpressionCollection(code.Indices, node, XML_Indices);
				WriteCodeExpression(code.TargetObject, node, XML_Expression);
			}
		}
		private void WriteCodeBaseReferenceExpression(CodeBaseReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{

			}
		}
		private void WriteCodeBinaryOperatorExpression(CodeBinaryOperatorExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeExpression(code.Left, node, XML_Left);
				WriteCodeExpression(code.Right, node, XML_Right);
				XmlUtil.SetAttribute(node, XMLATT_Operator, code.Operator);
			}
		}
		private void WriteCodeCastExpression(CodeCastExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeExpression(code.Expression, node, XML_Expression);
				WriteTypeReference(code.TargetType, node);
			}
		}
		private void WriteCodeDefaultValueExpression(CodeDefaultValueExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteTypeReference(code.Type, node);
			}
		}
		private void WriteCodeDelegateCreateExpression(CodeDelegateCreateExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteTypeReference(code.DelegateType, node);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
				XmlUtil.SetAttribute(node, XMLATT_MethodName, code.MethodName);
			}
		}
		private void WriteCodeDelegateInvokeExpression(CodeDelegateInvokeExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeExpressionCollection(code.Parameters, node, XML_Expressions);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
			}
		}

		private void WriteCodeDirectionExpression(CodeDirectionExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_Direction, code.Direction);
				WriteCodeExpression(code.Expression, node, XML_Expression);
			}
		}
		private void WriteCodeFieldReferenceExpression(CodeFieldReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_FieldName, code.FieldName);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
			}
		}
		private void WriteCodeIndexerExpression(CodeIndexerExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeExpressionCollection(code.Indices, node, XML_Indices);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
			}
		}
		private void WriteCodeMethodReferenceExpression(CodeMethodReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_MethodName, code.MethodName);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
				WriteTypeReferenceCollection(code.TypeArguments, node, XML_Types);
			}
		}
		private void WriteCodeMethodInvokeExpression(CodeMethodInvokeExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeExpressionCollection(code.Parameters, node, XML_Expressions);
				XmlNode m = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_METHOD);
				WriteCodeMethodReferenceExpression(code.Method, m);
			}
		}
		private void WriteCodeObjectCreateExpression(CodeObjectCreateExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteTypeReference(code.CreateType, node);
				WriteCodeExpressionCollection(code.Parameters, node, XML_Expressions);
			}
		}
		private void WriteCodeAttributeArgument(CodeAttributeArgument code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetNameAttribute(node, code.Name);
				WriteCodeExpression(code.Value, node, XML_Expression);
			}
		}
		private void WriteCodeAttributeArgumentCollection(CodeAttributeArgumentCollection code, XmlNode node, string name)
		{
			if (code != null)
			{
				if (code.Count > 0)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
					for (int i = 0; i < code.Count; i++)
					{
						XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nd.AppendChild(item);
						WriteCodeAttributeArgument(code[i], item);
					}
				}
			}
		}
		private void WriteCodeAttributeDeclaration(CodeAttributeDeclaration code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeAttributeArgumentCollection(code.Arguments, node, XML_Arguments);
				WriteTypeReference(code.AttributeType, node);
				XmlUtil.SetNameAttribute(node, code.Name);
			}
		}
		private void WriteCodeAttributeDeclarationCollection(CodeAttributeDeclarationCollection code, XmlNode node, string name)
		{
			if (code != null)
			{
				if (code.Count > 0)
				{
					XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
					for (int i = 0; i < code.Count; i++)
					{
						XmlNode item = nd.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nd.AppendChild(item);
						WriteCodeAttributeDeclaration(code[i], item);
					}
				}
			}
		}
		private void WriteCodeParameterDeclarationExpression(CodeParameterDeclarationExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteCodeAttributeDeclarationCollection(code.CustomAttributes, node, XML_Attributes);
				XmlUtil.SetAttribute(node, XMLATT_Direction, code.Direction);
				XmlUtil.SetNameAttribute(node, code.Name);
				WriteTypeReference(code.Type, node);
			}
		}
		private void WriteCodePrimitiveExpression(CodePrimitiveExpression code, XmlNode node)
		{
			if (code != null)
			{
				if (code.Value == null)
				{
					while (true)
					{
						XmlNode data = node.SelectSingleNode(XmlTags.XML_Data);
						if (data != null)
						{
							node.RemoveChild(data);
						}
						else
						{
							break;
						}
					}
				}
				else
				{
					XmlNode data = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
					Type t = code.Value.GetType();
					XmlUtil.SetLibTypeAttribute(data, t);
					TypeConverter converter = TypeDescriptor.GetConverter(code.Value);
					string txt = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, code.Value, typeof(string));
					if (!string.IsNullOrEmpty(txt))
					{
						if (string.IsNullOrEmpty(txt.Trim()))
						{
							XmlCDataSection xd = node.OwnerDocument.CreateCDataSection(XmlTags.XML_TEXTDATA);
							data.AppendChild(xd);
							xd.Value = txt;
						}
						else
						{
							data.InnerText = txt;
						}
					}
				}
			}
		}
		private void WriteCodePropertyReferenceExpression(CodePropertyReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_PropertyName, code.PropertyName);
				WriteCodeExpression(code.TargetObject, node, XML_TargetObject);
			}
		}
		private void WriteCodePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{

			}
		}
		private void WriteCodeSnippetExpression(CodeSnippetExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlCDataSection xd = node.OwnerDocument.CreateCDataSection(XmlTags.XML_TEXTDATA);
				node.AppendChild(xd);
				xd.Value = code.Value;

			}
		}
		private void WriteCodeTypeOfExpression(CodeTypeOfExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteTypeReference(code.Type, node);
			}
		}
		private void WriteCodeTypeReferenceExpression(CodeTypeReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				WriteTypeReference(code.Type, node);
			}
		}
		private void WriteCodeVariableReferenceExpression(CodeVariableReferenceExpression code, XmlNode node)
		{
			if (code != null)
			{
				XmlUtil.SetNameAttribute(node, code.VariableName);
			}
		}
		private void WriteCodeExpression(CodeExpression code, XmlNode node, string name)
		{
			if (code != null)
			{
				XmlNode nd = XmlUtil.CreateSingleNewElement(node, name);
				XmlUtil.SetLibTypeAttribute(nd, code.GetType());
				//
				CodeArgumentReferenceExpression c1 = code as CodeArgumentReferenceExpression;
				if (c1 != null)
				{
					WriteCodeArgumentReferenceExpression(c1, nd);
					return;
				}
				CodeArrayCreateExpression c2 = code as CodeArrayCreateExpression;
				if (c2 != null)
				{
					WriteCodeArrayCreateExpression(c2, nd);
					return;
				}
				CodeArrayIndexerExpression c3 = code as CodeArrayIndexerExpression;
				if (c3 != null)
				{
					WriteCodeArrayIndexerExpression(c3, nd);
					return;
				}
				CodeBaseReferenceExpression c4 = code as CodeBaseReferenceExpression;
				if (c4 != null)
				{
					WriteCodeBaseReferenceExpression(c4, nd);
					return;
				}
				CodeBinaryOperatorExpression c5 = code as CodeBinaryOperatorExpression;
				if (c5 != null)
				{
					WriteCodeBinaryOperatorExpression(c5, nd);
					return;
				}
				CodeCastExpression c6 = code as CodeCastExpression;
				if (c6 != null)
				{
					WriteCodeCastExpression(c6, nd);
					return;
				}
				CodeDefaultValueExpression c7 = code as CodeDefaultValueExpression;
				if (c7 != null)
				{
					WriteCodeDefaultValueExpression(c7, nd);
					return;
				}
				CodeDelegateCreateExpression c8 = code as CodeDelegateCreateExpression;
				if (c8 != null)
				{
					WriteCodeDelegateCreateExpression(c8, nd);
					return;
				}
				CodeDelegateInvokeExpression c9 = code as CodeDelegateInvokeExpression;
				if (c9 != null)
				{
					WriteCodeDelegateInvokeExpression(c9, nd);
					return;
				}
				CodeDirectionExpression c10 = code as CodeDirectionExpression;
				if (c10 != null)
				{
					WriteCodeDirectionExpression(c10, nd);
					return;
				}
				CodeEventReferenceExpression c11 = code as CodeEventReferenceExpression;
				if (c11 != null)
				{
					WriteCodeEventReferenceExpression(c11, nd);
					return;
				}
				CodeFieldReferenceExpression c12 = code as CodeFieldReferenceExpression;
				if (c12 != null)
				{
					WriteCodeFieldReferenceExpression(c12, nd);
					return;
				}
				CodeIndexerExpression c13 = code as CodeIndexerExpression;
				if (c13 != null)
				{
					WriteCodeIndexerExpression(c13, nd);
					return;
				}
				CodeMethodInvokeExpression c14 = code as CodeMethodInvokeExpression;
				if (c14 != null)
				{
					WriteCodeMethodInvokeExpression(c14, nd);
					return;
				}
				CodeMethodReferenceExpression c15 = code as CodeMethodReferenceExpression;
				if (c15 != null)
				{
					WriteCodeMethodReferenceExpression(c15, nd);
					return;
				}
				CodeObjectCreateExpression c16 = code as CodeObjectCreateExpression;
				if (c16 != null)
				{
					WriteCodeObjectCreateExpression(c16, nd);
					return;
				}
				CodeParameterDeclarationExpression c17 = code as CodeParameterDeclarationExpression;
				if (c17 != null)
				{
					WriteCodeParameterDeclarationExpression(c17, nd);
					return;
				}
				CodePrimitiveExpression c18 = code as CodePrimitiveExpression;
				if (c18 != null)
				{
					WriteCodePrimitiveExpression(c18, nd);
					return;
				}
				CodePropertyReferenceExpression c19 = code as CodePropertyReferenceExpression;
				if (c19 != null)
				{
					WriteCodePropertyReferenceExpression(c19, nd);
					return;
				}
				CodePropertySetValueReferenceExpression c20 = code as CodePropertySetValueReferenceExpression;
				if (c20 != null)
				{
					WriteCodePropertySetValueReferenceExpression(c20, nd);
					return;
				}
				CodeSnippetExpression c21 = code as CodeSnippetExpression;
				if (c21 != null)
				{
					WriteCodeSnippetExpression(c21, nd);
					return;
				}
				CodeThisReferenceExpression c22 = code as CodeThisReferenceExpression;
				if (c22 != null)
				{
					return;
				}
				CodeTypeOfExpression c23 = code as CodeTypeOfExpression;
				if (c23 != null)
				{
					WriteCodeTypeOfExpression(c23, nd);
					return;
				}
				CodeTypeReferenceExpression c24 = code as CodeTypeReferenceExpression;
				if (c24 != null)
				{
					WriteCodeTypeReferenceExpression(c24, nd);
					return;
				}
				CodeVariableReferenceExpression c25 = code as CodeVariableReferenceExpression;
				if (c25 != null)
				{
					WriteCodeVariableReferenceExpression(c25, nd);
					return;
				}
				throw new XmlSerializerException("Unhandled expression: {0}", code.GetType());
			}
		}

		#endregion

		#region Read Statement
		private CodeLinePragma ReadCodeLinePragma(XmlNode node)
		{
			CodeLinePragma code = new CodeLinePragma();

			XmlNode nd = node.SelectSingleNode(XML_LinePragma);
			if (nd != null)
			{

				//
				code.LineNumber = XmlUtil.GetAttributeInt(nd, XMLATT_LineNumber);
				code.FileName = XmlUtil.GetAttribute(nd, XMLATT_FileName);
			}
			return code;
		}
		private void ReadTypeReferenceCollection(CodeTypeReferenceCollection code, XmlNode node, string name)
		{
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", name, XmlTags.XML_Item));
			for (int i = 0; i < nds.Count; i++)
			{
				code.Add(ReadTypeReference(nds[i]));
			}

		}
		private CodeTypeReference ReadTypeReference(XmlNode node)
		{
			CodeTypeReference code = new CodeTypeReference();

			XmlNode nd;
			code.ArrayRank = XmlUtil.GetAttributeInt(node, XMLATT_ArrayRank);

			nd = node.SelectSingleNode(XML_ElementType);
			if (nd != null)
			{
				code.ArrayElementType = ReadTypeReference(nd);
			}
			XmlNode ndBaseType = node.SelectSingleNode(XmlTags.XML_TYPE);
			if (ndBaseType != null)
			{
				Type t0 = XmlUtil.GetTypeThrow(ndBaseType);
				code.BaseType = t0.AssemblyQualifiedName;
				code.UserData.Add(XmlTags.XML_TYPE, t0);
				if (t0 == null)
				{
					t0 = null;
				}
			}
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", XML_TypeList, XmlTags.XML_Item));
			if (nds.Count > 0)
			{

				for (int i = 0; i < nds.Count; i++)
				{
					code.TypeArguments.Add(ReadTypeReference(nds[i]));
				}
			}

			code.Options = XmlUtil.GetAttributeEnum<CodeTypeReferenceOptions>(node, XMLATT_Options);
			return code;
		}

		private CodeVariableDeclarationStatement ReadVariableDeclaration(XmlNode node)
		{
			CodeVariableDeclarationStatement code = new CodeVariableDeclarationStatement();

			XmlNode d = node.SelectSingleNode(XmlTags.XML_Data);
			if (d != null)
			{
				code.Name = XmlUtil.GetNameAttribute(d);
				code.Type = ReadTypeReference(d);
				code.LinePragma = ReadCodeLinePragma(d);
				code.InitExpression = ReadCodeExpression(d, XML_Expression);
			}
			return code;
		}
		private CodeSnippetStatement ReadCodeSnippetStatement(XmlNode node)
		{
			CodeSnippetStatement code = new CodeSnippetStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			XmlNode nd = node.SelectSingleNode(XmlTags.XML_Data);
			if (nd != null)
			{
				foreach (XmlNode n in nd.ChildNodes)
				{
					XmlCDataSection cd = n as XmlCDataSection;
					if (cd != null)
					{
						code.Value = cd.Value;
						break;
					}
				}
			}

			return code;
		}
		private CodeLabeledStatement ReadCodeLabeledStatement(XmlNode node)
		{
			CodeLabeledStatement code = new CodeLabeledStatement();

			code.Label = XmlUtil.GetAttribute(node, XMLATT_Label);
			code.LinePragma = ReadCodeLinePragma(node);
			XmlNode nd = node.SelectSingleNode(XML_Statement);
			if (nd != null)
			{
				code.Statement = ReadStatement(nd, XML_Statement);
			}
			return code;
		}
		private CodeDirective ReadCodeDirective(XmlNode node)
		{
			CodeDirective code = new CodeDirective();
			return code;
		}
		private void ReadCodeDirectiveCollection(CodeDirectiveCollection code, XmlNode node, string name)
		{
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", name, XmlTags.XML_Item));
			for (int i = 0; i < nds.Count; i++)
			{
				code.Add(ReadCodeDirective(nds[i]));
			}
		}
		private CodeCatchClause ReadCodeCatchClause(XmlNode node)
		{
			CodeCatchClause code = new CodeCatchClause();

			code.LocalName = XmlUtil.GetAttribute(node, XMLATT_LocalName);
			XmlNode nd = node.SelectSingleNode(XML_ElementType);
			if (nd != null)
			{
				code.CatchExceptionType = ReadTypeReference(nd);
			}
			ReadStatementCollection(code.Statements, node, XML_Statements);
			return code;
		}
		private void ReadCodeCatchClauseCollection(CodeCatchClauseCollection code, XmlNode node)
		{

			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/[1}", XML_CatchClauses, XmlTags.XML_Item));
			for (int i = 0; i < nds.Count; i++)
			{
				code.Add(ReadCodeCatchClause(nds[i]));
			}

		}
		private CodeTryCatchFinallyStatement ReadCodeTryCatchFinallyStatement(XmlNode node)
		{
			CodeTryCatchFinallyStatement code = new CodeTryCatchFinallyStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			ReadCodeCatchClauseCollection(code.CatchClauses, node);
			ReadStatementCollection(code.FinallyStatements, node, XML_FinallyStatements);
			ReadStatementCollection(code.TryStatements, node, XML_TryStatements);
			return code;
		}
		private CodeThrowExceptionStatement ReadCodeThrowExceptionStatement(XmlNode node)
		{
			CodeThrowExceptionStatement code = new CodeThrowExceptionStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.ToThrow = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private CodeEventReferenceExpression ReadCodeEventReferenceExpression(XmlNode node)
		{
			CodeEventReferenceExpression code = new CodeEventReferenceExpression();

			code.EventName = XmlUtil.GetAttribute(node, XMLATT_EventName);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			return code;
		}
		private CodeRemoveEventStatement ReadCodeRemoveEventStatement(XmlNode node)
		{
			CodeRemoveEventStatement code = new CodeRemoveEventStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Event = ReadCodeEventReferenceExpression(node);
			code.Listener = ReadCodeExpression(node, XML_Listener);
			return code;
		}
		private CodeMethodReturnStatement ReadCodeMethodReturnStatement(XmlNode node)
		{
			CodeMethodReturnStatement code = new CodeMethodReturnStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Expression = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private CodeIterationStatement ReadCodeIterationStatement(XmlNode node)
		{
			CodeIterationStatement code = new CodeIterationStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.IncrementStatement = ReadStatement(node, XML_IncrementStatement);
			code.InitStatement = ReadStatement(node, XML_InitStatement);
			code.TestExpression = ReadCodeExpression(node, XML_TestExpression);
			ReadStatementCollection(code.Statements, node, XML_Statements);
			return code;
		}
		private CodeGotoStatement ReadCodeGotoStatement(XmlNode node)
		{
			CodeGotoStatement code = new CodeGotoStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Label = XmlUtil.GetAttribute(node, XMLATT_Label);
			return code;
		}
		private CodeExpressionStatement ReadCodeExpressionStatement(XmlNode node)
		{
			CodeExpressionStatement code = new CodeExpressionStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Expression = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private CodeConditionStatement ReadCodeConditionStatement(XmlNode node)
		{
			CodeConditionStatement code = new CodeConditionStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Condition = ReadCodeExpression(node, XML_Expression);
			ReadStatementCollection(code.FalseStatements, node, XML_FalseStatements);
			ReadStatementCollection(code.TrueStatements, node, XML_TrueStatements);
			return code;
		}
		private CodeComment ReadCodeComment(XmlNode node)
		{
			CodeComment code = new CodeComment();

			XmlNode nd = node.SelectSingleNode(XmlTags.XML_Data);
			if (nd != null)
			{
				code.DocComment = XmlUtil.GetAttributeBoolDefFalse(nd, XMLATT_DocComment);
				foreach (XmlNode n in nd.ChildNodes)
				{
					XmlCDataSection cd = n as XmlCDataSection;// .OwnerDocument.CreateCDataSection(code.Text);
					if (cd != null)
					{
						code.Text = cd.Value;
						break;
					}
				}
			}
			return code;
		}
		private CodeCommentStatement ReadCodeCommentStatement(XmlNode node)
		{
			CodeCommentStatement code = new CodeCommentStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Comment = ReadCodeComment(node);
			return code;
		}
		private CodeAttachEventStatement ReadCodeAttachEventStatement(XmlNode node)
		{
			CodeAttachEventStatement code = new CodeAttachEventStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Event = ReadCodeEventReferenceExpression(node);
			code.Listener = ReadCodeExpression(node, XML_Listener);
			return code;
		}
		private CodeAssignStatement ReadCodeAssignStatement(XmlNode node)
		{
			CodeAssignStatement code = new CodeAssignStatement();

			code.LinePragma = ReadCodeLinePragma(node);
			code.Left = ReadCodeExpression(node, XML_Left);
			code.Right = ReadCodeExpression(node, XML_Right);
			return code;
		}
		private CodeStatement ReadStatement(XmlNode node, string name)
		{
			CodeStatement code = null;
			Type t = XmlUtil.GetTypeThrow(node);
			if (t.Equals(typeof(CodeAssignStatement)))
			{
				code = ReadCodeAssignStatement(node);
			}
			else if (t.Equals(typeof(CodeAttachEventStatement)))
			{
				code = ReadCodeAttachEventStatement(node);
			}
			else if (t.Equals(typeof(CodeCommentStatement)))
			{
				code = ReadCodeCommentStatement(node);
			}
			else if (t.Equals(typeof(CodeConditionStatement)))
			{
				code = ReadCodeConditionStatement(node);
			}
			else if (t.Equals(typeof(CodeExpressionStatement)))
			{
				code = ReadCodeExpressionStatement(node);
			}
			else if (t.Equals(typeof(CodeGotoStatement)))
			{
				code = ReadCodeGotoStatement(node);
			}
			else if (t.Equals(typeof(CodeIterationStatement)))
			{
				code = ReadCodeIterationStatement(node);
			}
			else if (t.Equals(typeof(CodeLabeledStatement)))
			{
				code = ReadCodeLabeledStatement(node);
			}
			else if (t.Equals(typeof(CodeMethodReturnStatement)))
			{
				code = ReadCodeMethodReturnStatement(node);
			}
			else if (t.Equals(typeof(CodeRemoveEventStatement)))
			{
				code = ReadCodeRemoveEventStatement(node);
			}
			else if (t.Equals(typeof(CodeSnippetStatement)))
			{
				code = ReadCodeSnippetStatement(node);
			}
			else if (t.Equals(typeof(CodeThrowExceptionStatement)))
			{
				code = ReadCodeThrowExceptionStatement(node);
			}
			else if (t.Equals(typeof(CodeTryCatchFinallyStatement)))
			{
				code = ReadCodeTryCatchFinallyStatement(node);
			}
			else if (t.Equals(typeof(CodeVariableDeclarationStatement)))
			{
				code = ReadVariableDeclaration(node);
			}
			if (code == null)
			{
				throw new XmlSerializerException("Unhandled statement: {0}", code.GetType());
			}
			ReadCodeDirectiveCollection(code.EndDirectives, node, XML_EndDirectives);
			ReadCodeDirectiveCollection(code.StartDirectives, node, XML_StartDirectives);
			return code;
		}
		public void ReadStatementCollection(CodeStatementCollection code, XmlNode node, string name)
		{
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", name, XmlTags.XML_Item));
			for (int i = 0; i < nds.Count; i++)
			{
				code.Add(ReadStatement(nds[i], XML_Statement));
			}
		}
		#endregion

		#region Read Expression
		private CodeArgumentReferenceExpression ReadCodeArgumentReferenceExpression(XmlNode node)
		{
			CodeArgumentReferenceExpression code = new CodeArgumentReferenceExpression();
			code.ParameterName = XmlUtil.GetAttribute(node, XMLATT_ParameterName);
			return code;
		}
		private void ReadCodeExpressionCollection(CodeExpressionCollection code, XmlNode node, string name)
		{
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", name, XmlTags.XML_Item));// XmlUtil.CreateSingleNewElement(node, name);
			for (int i = 0; i < nds.Count; i++)
			{
				CodeExpression c = ReadCodeExpression(nds[i], XML_Expression);
				if (c != null)
				{
					code.Add(c);
				}
				else
				{
					c = null;
				}
			}
		}
		private CodeArrayCreateExpression ReadCodeArrayCreateExpression(XmlNode node)
		{
			CodeArrayCreateExpression code = new CodeArrayCreateExpression();
			code.CreateType = ReadTypeReference(node);
			ReadCodeExpressionCollection(code.Initializers, node, XML_Initializers);
			code.Size = XmlUtil.GetAttributeInt(node, XMLATT_Size);
			code.SizeExpression = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private CodeArrayIndexerExpression ReadCodeArrayIndexerExpression(XmlNode node)
		{
			CodeArrayIndexerExpression code = new CodeArrayIndexerExpression();
			ReadCodeExpressionCollection(code.Indices, node, XML_Indices);
			code.TargetObject = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private CodeBaseReferenceExpression ReadCodeBaseReferenceExpression(XmlNode node)
		{
			CodeBaseReferenceExpression code = new CodeBaseReferenceExpression();
			return code;
		}
		private CodeBinaryOperatorExpression ReadCodeBinaryOperatorExpression(XmlNode node)
		{
			CodeBinaryOperatorExpression code = new CodeBinaryOperatorExpression();

			code.Left = ReadCodeExpression(node, XML_Left);
			code.Right = ReadCodeExpression(node, XML_Right);
			code.Operator = XmlUtil.GetAttributeEnum<CodeBinaryOperatorType>(node, XMLATT_Operator);
			return code;
		}
		private CodeCastExpression ReadCodeCastExpression(XmlNode node)
		{
			CodeCastExpression code = new CodeCastExpression();

			code.Expression = ReadCodeExpression(node, XML_Expression);
			code.TargetType = ReadTypeReference(node);
			return code;
		}
		private CodeDefaultValueExpression ReadCodeDefaultValueExpression(XmlNode node)
		{
			CodeDefaultValueExpression code = new CodeDefaultValueExpression();
			code.Type = ReadTypeReference(node);
			return code;
		}
		private CodeDelegateCreateExpression ReadCodeDelegateCreateExpression(XmlNode node)
		{
			CodeDelegateCreateExpression code = new CodeDelegateCreateExpression();
			code.DelegateType = ReadTypeReference(node);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			code.MethodName = XmlUtil.GetAttribute(node, XMLATT_MethodName);
			return code;
		}
		private CodeDelegateInvokeExpression ReadCodeDelegateInvokeExpression(XmlNode node)
		{
			CodeDelegateInvokeExpression code = new CodeDelegateInvokeExpression();

			ReadCodeExpressionCollection(code.Parameters, node, XML_Expressions);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			return code;
		}

		private CodeDirectionExpression ReadCodeDirectionExpression(XmlNode node)
		{
			CodeDirectionExpression code = new CodeDirectionExpression();
			code.Direction = XmlUtil.GetAttributeEnum<FieldDirection>(node, XMLATT_Direction);
			code.Expression = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private CodeFieldReferenceExpression ReadCodeFieldReferenceExpression(XmlNode node)
		{
			CodeFieldReferenceExpression code = new CodeFieldReferenceExpression();

			code.FieldName = XmlUtil.GetAttribute(node, XMLATT_FieldName);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			return code;
		}
		private CodeIndexerExpression ReadCodeIndexerExpression(XmlNode node)
		{
			CodeIndexerExpression code = new CodeIndexerExpression();

			ReadCodeExpressionCollection(code.Indices, node, XML_Indices);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			return code;
		}
		private CodeMethodReferenceExpression ReadCodeMethodReferenceExpression(XmlNode node)
		{
			CodeMethodReferenceExpression code = new CodeMethodReferenceExpression();

			code.MethodName = XmlUtil.GetAttribute(node, XMLATT_MethodName);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			ReadTypeReferenceCollection(code.TypeArguments, node, XML_Types);
			return code;
		}
		private CodeMethodInvokeExpression ReadCodeMethodInvokeExpression(XmlNode node)
		{
			CodeMethodInvokeExpression code = new CodeMethodInvokeExpression();

			ReadCodeExpressionCollection(code.Parameters, node, XML_Expressions);
			XmlNode m = node.SelectSingleNode(XmlTags.XML_METHOD);
			if (m != null)
			{
				code.Method = ReadCodeMethodReferenceExpression(m);
			}
			return code;
		}
		private CodeObjectCreateExpression ReadCodeObjectCreateExpression(XmlNode node)
		{
			CodeObjectCreateExpression code = new CodeObjectCreateExpression();

			code.CreateType = ReadTypeReference(node);
			ReadCodeExpressionCollection(code.Parameters, node, XML_Expressions);
			return code;
		}
		private CodeAttributeArgument ReadCodeAttributeArgument(XmlNode node)
		{
			CodeAttributeArgument code = new CodeAttributeArgument();

			code.Name = XmlUtil.GetNameAttribute(node);
			code.Value = ReadCodeExpression(node, XML_Expression);
			return code;
		}
		private void ReadCodeAttributeArgumentCollection(CodeAttributeArgumentCollection code, XmlNode node, string name)
		{
			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", name, XmlTags.XML_Item));// XmlUtil.CreateSingleNewElement(node, name);
			for (int i = 0; i < nds.Count; i++)
			{
				code.Add(ReadCodeAttributeArgument(nds[i]));
			}
		}
		private CodeAttributeDeclaration ReadCodeAttributeDeclaration(XmlNode node)
		{
			CodeTypeReference t = ReadTypeReference(node);
			CodeAttributeDeclaration code = new CodeAttributeDeclaration(t);

			ReadCodeAttributeArgumentCollection(code.Arguments, node, XML_Arguments);

			code.Name = XmlUtil.GetNameAttribute(node);
			return code;
		}
		private CodeAttributeDeclarationCollection ReadCodeAttributeDeclarationCollection(XmlNode node, string name)
		{
			CodeAttributeDeclarationCollection code = new CodeAttributeDeclarationCollection();

			XmlNodeList nds = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}", name, XmlTags.XML_Item));// XmlUtil.CreateSingleNewElement(node, name);
			for (int i = 0; i < nds.Count; i++)
			{
				code.Add(ReadCodeAttributeDeclaration(nds[i]));
			}
			return code;
		}
		private CodeParameterDeclarationExpression ReadCodeParameterDeclarationExpression(XmlNode node)
		{
			CodeParameterDeclarationExpression code = new CodeParameterDeclarationExpression();

			code.CustomAttributes = ReadCodeAttributeDeclarationCollection(node, XML_Attributes);
			code.Direction = XmlUtil.GetAttributeEnum<FieldDirection>(node, XMLATT_Direction);
			code.Name = XmlUtil.GetNameAttribute(node);
			code.Type = ReadTypeReference(node);
			return code;
		}
		private CodePrimitiveExpression ReadCodePrimitiveExpression(XmlNode node)
		{
			CodePrimitiveExpression code = new CodePrimitiveExpression();
			XmlNode data = node.SelectSingleNode(XmlTags.XML_Data);
			if (data == null)
			{
				code.Value = null;
			}
			else
			{
				Type t = XmlUtil.GetTypeThrow(data);
				string s = null;
				if (data.ChildNodes.Count > 0)
				{
					XmlCDataSection xd = data.ChildNodes[0] as XmlCDataSection;
					if (xd != null)
					{
						s = xd.Value;
					}
					else
					{
						s = data.ChildNodes[0].InnerText;
					}
				}
				else
				{
					s = data.InnerText;
				}
				if (string.IsNullOrEmpty(s))
				{
					code.Value = VPL.VPLUtil.GetDefaultValue(t);
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(t);
					code.Value = converter.ConvertFromInvariantString(s);
				}
			}
			return code;
		}
		private CodePropertyReferenceExpression ReadCodePropertyReferenceExpression(XmlNode node)
		{
			CodePropertyReferenceExpression code = new CodePropertyReferenceExpression();

			code.PropertyName = XmlUtil.GetAttribute(node, XMLATT_PropertyName);
			code.TargetObject = ReadCodeExpression(node, XML_TargetObject);
			return code;
		}
		private CodePropertySetValueReferenceExpression ReadCodePropertySetValueReferenceExpression(XmlNode node)
		{
			return new CodePropertySetValueReferenceExpression();
		}
		private CodeSnippetExpression ReadCodeSnippetExpression(XmlNode node)
		{
			CodeSnippetExpression code = new CodeSnippetExpression();
			foreach (XmlNode nd in node.ChildNodes)
			{
				XmlCDataSection xd = nd as XmlCDataSection;
				if (xd != null)
				{
					code.Value = xd.Value;
					break;
				}
			}
			return code;
		}
		private CodeTypeOfExpression ReadCodeTypeOfExpression(XmlNode node)
		{
			CodeTypeOfExpression code = new CodeTypeOfExpression();

			code.Type = ReadTypeReference(node);
			return code;
		}
		private CodeTypeReferenceExpression ReadCodeTypeReferenceExpression(XmlNode node)
		{
			CodeTypeReferenceExpression code = new CodeTypeReferenceExpression();

			code.Type = ReadTypeReference(node);
			return code;
		}
		private CodeVariableReferenceExpression ReadCodeVariableReferenceExpression(XmlNode node)
		{
			CodeVariableReferenceExpression code = new CodeVariableReferenceExpression();

			code.VariableName = XmlUtil.GetNameAttribute(node);
			return code;
		}
		private CodeExpression ReadCodeExpression(XmlNode node, string name)
		{
			XmlNode nd = node.SelectSingleNode(name);
			if (nd == null)
			{
				return null;
			}
			Type t = XmlUtil.GetTypeThrow(nd);
			//
			if (typeof(CodeArgumentReferenceExpression).Equals(t))
			{
				return ReadCodeArgumentReferenceExpression(nd);

			}
			if (typeof(CodeArrayCreateExpression).Equals(t))
			{
				return ReadCodeArrayCreateExpression(nd);
			}
			if (typeof(CodeArrayIndexerExpression).Equals(t))
			{
				return ReadCodeArrayIndexerExpression(nd);

			}
			if (typeof(CodeBaseReferenceExpression).Equals(t))
			{
				return ReadCodeBaseReferenceExpression(nd);

			}
			if (typeof(CodeBinaryOperatorExpression).Equals(t))
			{
				return ReadCodeBinaryOperatorExpression(nd);

			}
			if (typeof(CodeCastExpression).Equals(t))
			{
				return ReadCodeCastExpression(nd);

			}
			if (typeof(CodeDefaultValueExpression).Equals(t))
			{
				return ReadCodeDefaultValueExpression(nd);

			}
			if (typeof(CodeDelegateCreateExpression).Equals(t))
			{
				return ReadCodeDelegateCreateExpression(nd);

			}
			if (typeof(CodeDelegateInvokeExpression).Equals(t))
			{
				return ReadCodeDelegateInvokeExpression(nd);

			}
			if (typeof(CodeDirectionExpression).Equals(t))
			{
				return ReadCodeDirectionExpression(nd);

			}
			if (typeof(CodeEventReferenceExpression).Equals(t))
			{
				return ReadCodeEventReferenceExpression(nd);

			}
			if (typeof(CodeFieldReferenceExpression).Equals(t))
			{
				return ReadCodeFieldReferenceExpression(nd);

			}
			if (typeof(CodeIndexerExpression).Equals(t))
			{
				return ReadCodeIndexerExpression(nd);

			}
			if (typeof(CodeMethodInvokeExpression).Equals(t))
			{
				return ReadCodeMethodInvokeExpression(nd);

			}
			if (typeof(CodeMethodReferenceExpression).Equals(t))
			{
				return ReadCodeMethodReferenceExpression(nd);

			}
			if (typeof(CodeObjectCreateExpression).Equals(t))
			{
				return ReadCodeObjectCreateExpression(nd);

			}
			if (typeof(CodeParameterDeclarationExpression).Equals(t))
			{
				return ReadCodeParameterDeclarationExpression(nd);

			}
			if (typeof(CodePrimitiveExpression).Equals(t))
			{
				return ReadCodePrimitiveExpression(nd);

			}
			if (typeof(CodePropertyReferenceExpression).Equals(t))
			{
				return ReadCodePropertyReferenceExpression(nd);

			}
			if (typeof(CodePropertySetValueReferenceExpression).Equals(t))
			{
				return ReadCodePropertySetValueReferenceExpression(nd);

			}
			if (typeof(CodeSnippetExpression).Equals(t))
			{
				return ReadCodeSnippetExpression(nd);

			}
			if (typeof(CodeThisReferenceExpression).Equals(t))
			{
				return new CodeThisReferenceExpression();
			}
			if (typeof(CodeTypeOfExpression).Equals(t))
			{
				return ReadCodeTypeOfExpression(nd);

			}
			if (typeof(CodeTypeReferenceExpression).Equals(t))
			{
				return ReadCodeTypeReferenceExpression(nd);

			}
			if (typeof(CodeVariableReferenceExpression).Equals(t))
			{
				return ReadCodeVariableReferenceExpression(nd);

			}
			throw new XmlSerializerException("Unhandled expression: {0}", node.InnerXml);
		}
	}

		#endregion
}

