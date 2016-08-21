/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.CodeDom;
using System.Collections.Specialized;
using MathExp.RaisTypes;
using System.Drawing;
using System.Drawing.Design;

namespace MathExp
{
	/// <summary>
	/// a method call is a math expression
	/// </summary>
	[MathNodeCategoryAttribute(enumOperatorCategory.Internal)]
	[Description("call a function")]
	[ToolboxBitmapAttribute(typeof(MathNodeMethodInvoke), "Resources.MathNodeMethodInvoke.bmp")]
	public class MathNodeMethodInvoke : MathNodeFunction
	{
		private MethodRef _methodRef;
		//set before calling ExportCode
		private CodeExpression[] parameterCode;
		private string[] parameterCodeJS;
		private string[] parameterCodePhp;
		public MathNodeMethodInvoke(MathNode parent)
			: base(parent)
		{
			_methodRef = new MethodRef();
		}
		public MathNodeMethodInvoke(MathNode parent, ObjectRef obj, string name, int id, RaisDataType retType, ParameterDef[] parameters)
			: base(parent)
		{
			_methodRef = new MethodRef(obj, name, id, parameters, retType);
			OnParametersSet();
		}
		[Browsable(false)]
		public MethodRef MethodReference
		{
			get
			{
				return _methodRef;
			}
			set
			{

				if (value == null)
				{
					_methodRef = new MethodRef();
					ChildNodeCount = _methodRef.ParameterCount;
				}
				else
				{
					_methodRef.MethodOwner = value.MethodOwner;
					_methodRef.MethodName = value.MethodName;
					_methodRef.ReturnType = value.ReturnType;
					_methodRef.Parameters = value.Parameters;
					//ensure parameter type
					int n = value.ParameterCount;
					ChildNodeCount = n;
					for (int i = 0; i < n; i++)
					{
						((MathNodeParameter)this[i]).VariableType = value.Parameters[i].DataType;
					}
				}
			}
		}
		[Browsable(false)]
		public ObjectRef MethodOwner
		{
			get
			{
				return _methodRef.MethodOwner;
			}
			set
			{
				_methodRef.MethodOwner = value;
			}
		}
		public string MethodOwnerName
		{
			get
			{
				if (_methodRef.MethodOwner != null)
				{
					return _methodRef.MethodOwner.Name;
				}
				return "";
			}
		}
		[ReadOnly(true)]
		public string MethodName
		{
			get
			{
				return _methodRef.MethodName;
			}
			set
			{
				_methodRef.MethodName = value;
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		[Browsable(false)]
		public int ParameterCount0
		{
			get
			{
				if (_methodRef.Parameters == null)
					return 0;
				return _methodRef.Parameters.Length;
			}
		}
		[Browsable(false)]
		public ParameterDef[] Parameters
		{
			get
			{
				return _methodRef.Parameters;
			}
			set
			{
				_methodRef.Parameters = value;
				OnParametersSet();
			}
		}
		[Browsable(false)]
		public RaisDataType ReturnType
		{
			get
			{
				return _methodRef.ReturnType;
			}
			set
			{
				_methodRef.ReturnType = value;
			}
		}
		[ReadOnly(true)]
		[Editor(typeof(UITypeEditorMethodSelector), typeof(UITypeEditor))]
		public override string FunctionName
		{
			get
			{
				if (_methodRef.MethodName == XmlSerialization.CONSTRUCTOR_METHOD)
					return "new " + _methodRef.MethodOwnerName;
				return _methodRef.MethodOwnerName + "." + _methodRef.MethodName;
			}
			set
			{
			}
		}
		[Browsable(false)]
		protected override string FunctionDisplay
		{
			get
			{
				return FunctionName;
			}
		}
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(FunctionName);
				sb.Append("(");
				int n = ChildNodeCount;
				if (n > 0)
				{
					sb.Append(this[0].TraceInfo);
					for (int i = 1; i < n; i++)
					{
						sb.Append(",");
						sb.Append(this[i].TraceInfo);
					}
				}
				sb.Append(")");
				return sb.ToString();
			}
		}
		public override string ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder(FunctionName);
			sb.Append("(");
			int n = ChildNodeCount;
			if (n > 0)
			{
				sb.Append(this[0].ToString());
				for (int i = 1; i < n; i++)
				{
					sb.Append(",");
					sb.Append(this[i].ToString());
				}
			}
			sb.Append(")");
			return sb.ToString();
		}
		/// <summary>
		/// return value type as library type
		/// </summary>
		public override RaisDataType DataType
		{
			get
			{
				return _methodRef.ReturnType;
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void SetFunctionName()
		{
			if (!string.IsNullOrEmpty(_methodRef.MethodName))
			{
				base.SetFunctionName(_methodRef.MethodOwnerName + "." + _methodRef.MethodName);
			}
		}

		#region IMethodNode Members

		public override void SetFunction(object func)
		{
			MathNodeRoot math = func as MathNodeRoot;
			if (math != null)
			{
				MathNodeMethodInvoke mi = math[1] as MathNodeMethodInvoke;
				if (mi != null)
				{
					this.MethodReference = mi.MethodReference;
				}
			}
		}
		public override object GetFunction()
		{
			return this.MethodReference;
		}
		public override string GetFunctionName()
		{
			SetFunctionName();
			return FunctionName;
		}
		#endregion
		protected void OnParametersSet()
		{
			if (_methodRef.Parameters == null)
			{
				ParameterCount = 0;
			}
			else
			{
				ParameterCount = _methodRef.Parameters.Length;
			}

			SetFunctionName();
		}
		public override MathNode CreateDefaultNode(int i)
		{
			MathNodeParameter p = new MathNodeParameter(this);
			if (_methodRef.Parameters != null && i < _methodRef.Parameters.Length)
			{
				p.VariableName = _methodRef.Parameters[i].Name;
				p.VariableType = _methodRef.Parameters[i];
				p.DefaultValue = XmlSerialization.CreateObject(p.VariableType.Type);
			}
			return p;
		}
		public override AssemblyRefList GetImports()
		{
			if (_methodRef.MethodOwner == null)
				throw new Exception("methodOwner is not assigned to MethodNode.");
			if (_methodRef.MethodOwner.Type == ObjectRefType.Type)
			{
				AssemblyRefList sc = new AssemblyRefList();
				sc.AddRef(_methodRef.MethodOwner.Value.LibType.Assembly.GetName().Name, _methodRef.MethodOwner.Value.LibType.Assembly.Location);
				return sc;
			}
			return null;
		}
		public override string CreateJavaScript(StringCollection method)
		{
			if (_methodRef.MethodOwner == null)
				throw new Exception("methodOwner is not assigned to MethodNode.");
			MathNode.Trace("{0}:CreateJavaScript for {1}", this.GetType().Name, this.TraceInfo);
			if (_methodRef.Parameters != null)
			{
				int n = ChildNodeCount;
				if (_methodRef.ParameterCount != n)
				{
					throw new MathException("method parameters are not initialized for {0}.", this.GetType().Name);
				}
				MathNode.Trace("{0} parameter(s)", n);
				MathNode.IndentIncrement();
				parameterCodeJS = new string[n];
				for (int i = 0; i < n; i++)
				{
					MathNode.Trace("parameter {0}: {1}", i, this[i].TraceInfo);
					string ce = this[i].CreateJavaScript(method);
					parameterCodeJS[i] = ce;
				}
				MathNode.IndentDecrement();
			}
			else
			{
				parameterCodeJS = new string[] { };
			}

			if (_methodRef.MethodOwner.Type == ObjectRefType.Type)//a static function
			{
				MathNode.Trace("Invoke static function {0}.{1}", _methodRef.MethodOwner.Value.LibType, _methodRef.MethodName);
				StringBuilder mCode = new StringBuilder();
				mCode.Append(_methodRef.MethodOwner.Value.LibType.FullName);
				mCode.Append(".");
				mCode.Append(_methodRef.MethodName);
				mCode.Append("(");
				if (parameterCodeJS.Length > 0)
				{
					mCode.Append(parameterCodeJS[0]);
					for (int i = 1; i < parameterCodeJS.Length; i++)
					{
						mCode.Append(",");
						mCode.Append(parameterCodeJS[i]);
					}
				}
				mCode.Append(")");
				return mCode.ToString();
			}
			else
			{
				if (_methodRef.MethodName == XmlSerialization.CONSTRUCTOR_METHOD)
				{
					MathNode.Trace("Invoke constructor for {0}", _methodRef.MethodOwner.TypeString);
					if (parameterCodeJS.Length == 0)
					{
						return MathNode.FormString("new _methodRef.MethodOwner.TypeString()");
					}
					else
					{
						string cn = MathNode.FormString("_const{0}", _methodRef.MethodOwner.TypeString.Replace(".", "_"));
						StringBuilder constructor = new StringBuilder("function ");
						constructor.Append(cn);
						constructor.Append("(");
						constructor.Append(_methodRef.Parameters[0].Name);
						for (int i = 1; i < _methodRef.Parameters.Length; i++)
						{
							constructor.Append(",");
							constructor.Append(_methodRef.Parameters[i].Name);
						}
						constructor.Append(")");
						string cst = constructor.ToString();
						string scode = method.ToString();
						bool bExist = (scode.IndexOf(cst) >= 0);
						if (!bExist)
						{
							method.Add(cst);
							method.Add("\r\n{\r\n");
							method.Add("this.");
							method.Add(_methodRef.Parameters[0].Name);
							method.Add(";\r\n");
							for (int i = 1; i < _methodRef.Parameters.Length; i++)
							{
								constructor.Append("this.");
								constructor.Append(_methodRef.Parameters[i].Name);
								method.Add(";\r\n");
							}
							method.Add("\r\n}\r\n");
						}
						StringBuilder callC = new StringBuilder("new ");
						callC.Append(cn);
						callC.Append("(");
						callC.Append(parameterCodeJS[0]);
						for (int i = 1; i < parameterCodeJS.Length; i++)
						{
							callC.Append(",");
							callC.Append(parameterCodeJS[i]);
						}
						callC.Append(")");
						return callC.ToString();
					}
				}
				MathNode.Trace("Invoke member function {0} from {1}", FunctionName, _methodRef.MethodOwner.Name);
				if (string.IsNullOrEmpty(TargetObjectJS))
				{
					if (_methodRef.IsStatic)
					{
						TargetObjectJS = _methodRef.MethodOwner.TypeString;
					}
					else
					{
						TargetObjectJS = this.MethodOwner.CreateJavaScript(method, _methodRef.MethodOwner.XPath);
					}
				}
				//
				StringBuilder e = new StringBuilder(TargetObjectJS);
				if (!string.IsNullOrEmpty(TargetObjectJS))
				{
					e.Append(".");
				}
				e.Append(_methodRef.MethodName);
				e.Append("(");
				if (parameterCodeJS.Length > 0)
				{
					e.Append(parameterCodeJS[0]);
					for (int i = 1; i < parameterCodeJS.Length; i++)
					{
						e.Append(",");
						e.Append(parameterCodeJS[i]);
					}
				}
				e.Append(")");
				return e.ToString();
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			if (_methodRef.MethodOwner == null)
				throw new Exception("methodOwner is not assigned to MethodNode.");
			MathNode.Trace("{0}:CreatePhpScript for {1}", this.GetType().Name, this.TraceInfo);
			if (_methodRef.Parameters != null)
			{
				int n = ChildNodeCount;
				if (_methodRef.ParameterCount != n)
				{
					throw new MathException("method parameters are not initialized for {0}.", this.GetType().Name);
				}
				MathNode.Trace("{0} parameter(s)", n);
				MathNode.IndentIncrement();
				parameterCodePhp = new string[n];
				for (int i = 0; i < n; i++)
				{
					MathNode.Trace("parameter {0}: {1}", i, this[i].TraceInfo);
					string ce = this[i].CreatePhpScript(method);
					parameterCodePhp[i] = ce;
				}
				MathNode.IndentDecrement();
			}
			else
			{
				parameterCodePhp = new string[] { };
			}

			if (_methodRef.MethodOwner.Type == ObjectRefType.Type)//a static function
			{
				MathNode.Trace("Invoke static function {0}.{1}", _methodRef.MethodOwner.Value.LibType, _methodRef.MethodName);
				StringBuilder mCode = new StringBuilder();
				mCode.Append(_methodRef.MethodOwner.Value.LibType.FullName);
				mCode.Append("->");
				mCode.Append(_methodRef.MethodName);
				mCode.Append("(");
				if (parameterCodePhp.Length > 0)
				{
					mCode.Append(parameterCodePhp[0]);
					for (int i = 1; i < parameterCodePhp.Length; i++)
					{
						mCode.Append(",");
						mCode.Append(parameterCodePhp[i]);
					}
				}
				mCode.Append(")");
				return mCode.ToString();
			}
			else
			{
				if (_methodRef.MethodName == XmlSerialization.CONSTRUCTOR_METHOD)
				{
					MathNode.Trace("Invoke constructor for {0}", _methodRef.MethodOwner.TypeString);
					if (parameterCodePhp.Length == 0)
					{
						return MathNode.FormString("new _methodRef.MethodOwner.TypeString()");
					}
					else
					{
						string cn = MathNode.FormString("_const{0}", _methodRef.MethodOwner.TypeString.Replace(".", "_"));
						StringBuilder constructor = new StringBuilder("function ");
						constructor.Append(cn);
						constructor.Append("(");
						constructor.Append(_methodRef.Parameters[0].Name);
						for (int i = 1; i < _methodRef.Parameters.Length; i++)
						{
							constructor.Append(",");
							constructor.Append(_methodRef.Parameters[i].Name);
						}
						constructor.Append(")");
						string cst = constructor.ToString();
						string scode = method.ToString();
						bool bExist = (scode.IndexOf(cst) >= 0);
						if (!bExist)
						{
							method.Add(cst);
							method.Add("\r\n{\r\n");
							method.Add("$this->");
							method.Add(_methodRef.Parameters[0].Name);
							method.Add(";\r\n");
							for (int i = 1; i < _methodRef.Parameters.Length; i++)
							{
								constructor.Append("this->");
								constructor.Append(_methodRef.Parameters[i].Name);
								method.Add(";\r\n");
							}
							method.Add("\r\n}\r\n");
						}
						StringBuilder callC = new StringBuilder("new ");
						callC.Append(cn);
						callC.Append("(");
						callC.Append(parameterCodePhp[0]);
						for (int i = 1; i < parameterCodePhp.Length; i++)
						{
							callC.Append(",");
							callC.Append(parameterCodePhp[i]);
						}
						callC.Append(")");
						return callC.ToString();
					}
				}
				MathNode.Trace("Invoke member function {0} from {1}", FunctionName, _methodRef.MethodOwner.Name);
				if (string.IsNullOrEmpty(TargetObjectPhp))
				{
					if (_methodRef.IsStatic)
					{
						TargetObjectPhp = _methodRef.MethodOwner.TypeString;
					}
					else
					{
						TargetObjectPhp = this.MethodOwner.CreatePhpScript(method, _methodRef.MethodOwner.XPath);
					}
				}
				//
				StringBuilder e = new StringBuilder(TargetObjectPhp);
				if (!string.IsNullOrEmpty(TargetObjectPhp))
				{
					e.Append("->");
				}
				e.Append(_methodRef.MethodName);
				e.Append("(");
				if (parameterCodePhp.Length > 0)
				{
					e.Append(parameterCodePhp[0]);
					for (int i = 1; i < parameterCodePhp.Length; i++)
					{
						e.Append(",");
						e.Append(parameterCodePhp[i]);
					}
				}
				e.Append(")");
				return e.ToString();
			}
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			if (_methodRef.MethodOwner == null)
				throw new Exception("methodOwner is not assigned to MethodNode.");
			MathNode.Trace("{0}:ExportCode for {1}", this.GetType().Name, this.TraceInfo);
			if (_methodRef.Parameters != null)
			{
				int n = ChildNodeCount;
				if (_methodRef.ParameterCount != n)
				{
					throw new MathException("method parameters are not initialized for {0}.", this.GetType().Name);
				}
				MathNode.Trace("{0} parameter(s)", n);
				MathNode.IndentIncrement();
				parameterCode = new CodeExpression[n];
				for (int i = 0; i < n; i++)
				{
					MathNode.Trace("parameter {0}: {1}", i, this[i].TraceInfo);
					CodeExpression ce = this[i].ExportCode(method);
					if (!_methodRef.Parameters[i].DataType.IsSameType(this[i].DataType))
					{
						ce = RaisDataType.GetConversionCode(this[i].DataType, ce, _methodRef.Parameters[i].DataType, method.MethodCode.Statements);
					}
					parameterCode[i] = ce;
					if (_methodRef.Parameters[i].Direction != FieldDirection.In)
					{
						parameterCode[i] = new CodeDirectionExpression(_methodRef.Parameters[i].Direction, parameterCode[i]);
					}
				}
				MathNode.IndentDecrement();
			}
			else
			{
				parameterCode = new CodeExpression[] { };
			}
			if (_methodRef.MethodOwner.Type == ObjectRefType.Type)//a static function
			{
				MathNode.Trace("Invoke static function {0}.{1}", _methodRef.MethodOwner.Value.LibType, _methodRef.MethodName);
				CodeMethodInvokeExpression e = new CodeMethodInvokeExpression(
					new CodeTypeReferenceExpression(_methodRef.MethodOwner.Value.LibType),
					_methodRef.MethodName, parameterCode);
				return e;
			}
			else
			{
				if (_methodRef.MethodName == XmlSerialization.CONSTRUCTOR_METHOD)
				{
					MathNode.Trace("Invoke constructor for {0}", _methodRef.MethodOwner.TypeString);
					CodeObjectCreateExpression oc = new CodeObjectCreateExpression(_methodRef.MethodOwner.TypeString, parameterCode);
					return oc;
				}
				MathNode.Trace("Invoke member function {0} from {1}", FunctionName, _methodRef.MethodOwner.Name);
				if (TargetObject == null)
				{
					if (_methodRef.IsStatic)
					{
						TargetObject = new CodeTypeReferenceExpression(_methodRef.MethodOwner.TypeString);
					}
					else
					{
						TargetObject = this.MethodOwner.ExportCode(_methodRef.MethodOwner.XPath);
					}
				}
				//
				CodeMethodInvokeExpression e = new CodeMethodInvokeExpression(
					TargetObject, _methodRef.MethodName, parameterCode);
				return e;
			}
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = ParameterCount;
		}
		protected override MathNode OnCreateClone(MathNode parent)
		{
			ParameterDef[] ps = null;
			if (_methodRef.Parameters != null)
			{
				ps = new ParameterDef[_methodRef.Parameters.Length];
				for (int i = 0; i < _methodRef.Parameters.Length; i++)
				{
					ps[i] = (ParameterDef)_methodRef.Parameters[i].Clone();
				}
			}
			ObjectRef ow = null;
			if (_methodRef.MethodOwner != null)
			{
				ow = (ObjectRef)_methodRef.MethodOwner.Clone();
			}
			RaisDataType ret;
			if (_methodRef.ReturnType != null)
			{
				ret = (RaisDataType)_methodRef.ReturnType.Clone();
			}
			else
			{
				ret = new RaisDataType(typeof(void));
			}
			return new MathNodeMethodInvoke(parent, ow, _methodRef.MethodName, _methodRef.MethodID, ret, ps);
		}
		public override object CloneExp(MathNode parent)
		{
			//OnCreateClone will populate the members
			MathNodeMethodInvoke node = (MathNodeMethodInvoke)base.CloneExp(parent);
			if (_methodRef.MethodID != 0)
			{
				node.OnParametersSet();
			}
			return node;
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.WriteToChildXmlNode(GetWriter(), node, XmlSerialization.XML_METHODREF, _methodRef);
		}
		protected override void OnLoad(XmlNode node)
		{
			_methodRef = (MethodRef)XmlSerialization.ReadFromChildXmlNode(GetReader(), node, XmlSerialization.XML_METHODREF);
		}
		protected override void OnLoaded()
		{
			if (_methodRef.MethodID != 0)
			{
				OnParametersSet();
			}
		}
		public override bool OnReportContainLibraryTypesConly()
		{
			if (_methodRef.MethodOwner != null)
				return !_methodRef.MethodOwner.NeedCompile;
			return true;
		}
	}
}
