/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit - support of programming entities
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.CodeDom;
using XmlSerializer;
using System.Xml;
using LimnorDesigner;
using System.Windows.Forms;
using ProgElements;
using XmlUtility;
using LimnorDesigner.MethodBuilder;
using System.Collections.Specialized;
using VPL;
using System.Globalization;
using LimnorDesigner.Property;

namespace MathItem
{
	/// <summary>
	/// a math node representing a property
	/// </summary>
	[ProjectItem]
	[UseModalSelector]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[MathNodeCategoryAttribute(enumOperatorCategory.System)]
	[Description("A property")]
	[ToolboxBitmapAttribute(typeof(MathNodePointer), "Resources.property.bmp")]
	public class MathNodePointer : MathNode, IDataScope, ISourceValuePointer, IXmlNodeSerializable, IDatabaseFieldPointer, IPropertyMathNode
	{
		#region fields and constructors
		private IObjectPointer _valuePointer;
		private UInt32 _taskId;
		static MathNodePointer()
		{
			XmlUtil.AddKnownType("MathNodePointer", typeof(MathNodePointer));
		}
		/// <summary>
		/// force call to MathNodePointer
		/// </summary>
		public static void InitType()
		{
		}
		public MathNodePointer(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region properties
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (_valuePointer != null)
				{
					return _valuePointer.IsValid;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_valuePointer is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return true;
			}
		}
		[Description("Gets the execution place")]
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (IsWebClientValue())
				{
					if (!IsWebServerValue())
					{
						return EnumWebRunAt.Client;
					}
				}
				else
				{
					if (IsWebServerValue())
					{
						return EnumWebRunAt.Server;
					}
				}
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public bool IsDatabaseField
		{
			get
			{

				if (_valuePointer != null)
				{
					IDatabaseFieldProvider dp = _valuePointer.Owner as IDatabaseFieldProvider;
					if (dp != null)
					{
						return true;
					}
				}
				return false;
			}
		}
		[Browsable(false)]
		public override bool CanBeNull
		{
			get
			{
				if (_valuePointer != null)
				{
					IDatabaseFieldProvider dp = _valuePointer.Owner as IDatabaseFieldProvider;
					if (dp != null)
					{
						return false;
					}
					else
					{
						if (_valuePointer.ObjectType != null && _valuePointer.ObjectType.IsValueType)
						{
							return false;
						}
						return true;
					}
				}
				return false;
			}
		}
		[IgnoreReadOnlyAttribute]
		[ReadOnly(true)]
		[Editor(typeof(PropEditorPropertyPointer), typeof(UITypeEditor))]
		[Description("It points to a property of an object")]
		public IObjectPointer Property
		{
			get
			{
				if (_valuePointer == null)
				{
					ObjectIDmap map = (ObjectIDmap)MathNode.GetService(typeof(ObjectIDmap));
					if (map != null && map.Count > 0)
					{
						_valuePointer = new PropertyPointer();
						ClassPointer cid = ClassPointer.CreateClassPointer(map);
						_valuePointer.Owner = cid;
					}
				}
				return _valuePointer;
			}
			set
			{
				if (value != null)
				{
					_valuePointer = value;
				}
			}
		}
		public IPropertyPointer PropertyOwner { get { return _valuePointer; } }
		public override MathExp.RaisTypes.RaisDataType DataType
		{
			get
			{
				if (_valuePointer == null)
					return new MathExp.RaisTypes.RaisDataType(typeof(double));
				return new MathExp.RaisTypes.RaisDataType(_valuePointer.ObjectType);
			}
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get
			{
				if (_valuePointer == null)
					return "{null}";
				return _valuePointer.ToString();
			}
		}
		#endregion
		#region methods
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override string ToString()
		{
			string s;
			if (_valuePointer == null)
				s = "{null}";
			else
				s = _valuePointer.ExpressionDisplay;
			return s;
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			string s = this.ToString();
			return g.MeasureString(s, TextFont);
		}

		public override void OnDraw(Graphics g)
		{
			string s = this.ToString();
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
				g.DrawString(s, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(s, TextFont, TextBrush, (float)0, (float)0);
			}
		}
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			_forValue = forValue;
			return ExportCode(method);
		}
		public override void SetActionInputName(string name, Type type)
		{
			IActionInput ai = _valuePointer as IActionInput;
			if (ai != null)
			{
				ai.SetActionInputName(name, type);
			}
		}
		public override CodeExpression ExportIsNullCheck(IMethodCompile method)
		{
			if (_valuePointer != null)
			{
				PropertyPointer mp = _valuePointer as PropertyPointer;
				IDatabaseFieldProvider ep = _valuePointer.Owner as IDatabaseFieldProvider;
				if (ep != null && mp != null)
				{
					CodeExpression propOwner = _valuePointer.Owner.GetReferenceCode(method, method.MethodCode.Statements, true);
					return ep.GetIsNullCheck(method, method.MethodCode.Statements, mp.MemberName, propOwner);
				}
				else
				{
					CodeExpression ce = _valuePointer.GetReferenceCode(method, method.MethodCode.Statements, true);
					return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
				}
			}
			else
			{
				CodeArgumentReferenceExpression ce = new CodeArgumentReferenceExpression(CodeName);
				return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
			}
		}
		public override CodeExpression ExportIsNotNullCheck(IMethodCompile method)
		{
			if (_valuePointer != null)
			{
				PropertyPointer mp = _valuePointer as PropertyPointer;
				IDatabaseFieldProvider ep = _valuePointer.Owner as IDatabaseFieldProvider;
				if (ep != null && mp != null)
				{
					CodeExpression propOwner = _valuePointer.Owner.GetReferenceCode(method, method.MethodCode.Statements, true);
					return ep.GetIsNotNullCheck(method, method.MethodCode.Statements, mp.MemberName, propOwner);
				}
				else
				{
					CodeExpression ce = _valuePointer.GetReferenceCode(method, method.MethodCode.Statements, true);
					return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
				}
			}
			else
			{
				CodeArgumentReferenceExpression ce = new CodeArgumentReferenceExpression(CodeName);
				return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			}
		}
		public bool IsField()
		{
			PropertyPointer pp = _valuePointer as PropertyPointer;
			if (pp != null)
			{
				return pp.IsField();
			}
			return false;
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			CodeExpression ce;
			if (_valuePointer != null)
			{
				bool isWeb = false;
				if (_valuePointer.RootPointer != null)
				{
					isWeb = _valuePointer.RootPointer.IsWebPage;
				}
				if (!IsField() && isWeb && this.IsWebClientValue())
				{
					//this is a client property, use clientRequest.GetStringValue(DataPassingCodeName)
					CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
						new CodeVariableReferenceExpression("clientRequest"), "GetStringValue", new CodePrimitiveExpression(DataPassingCodeName));
					ce = cmie;
				}
				else
				{
					ce = _valuePointer.GetReferenceCode(method, method.MethodCode.Statements, _forValue);
				}
				ActualCompiledType = new MathExp.RaisTypes.RaisDataType(_valuePointer.ObjectType);
				if (CompileDataType != null)
				{
					if (CompileDataType.Type != null && _valuePointer.ObjectType != null)
					{
						DataTypePointer targetType = new DataTypePointer(CompileDataType.Type);
						DataTypePointer sourceType = new DataTypePointer(_valuePointer.ObjectType);
						IGenericTypePointer igp = _valuePointer as IGenericTypePointer;
						if (igp != null)
						{
							if (targetType.IsGenericType || targetType.IsGenericParameter)
							{
								DataTypePointer dtp = igp.GetConcreteType(targetType.BaseClassType);
								if (dtp != null)
								{
									targetType = dtp;
								}
							}
							if (sourceType.IsGenericType || sourceType.IsGenericParameter)
							{
								DataTypePointer dtp = igp.GetConcreteType(sourceType.BaseClassType);
								if (dtp != null)
								{
									sourceType = dtp;
								}
							}
						}
						if (!targetType.IsAssignableFrom(sourceType))
						{
							ce = CompilerUtil.GetTypeConversion(targetType, ce, sourceType, method.MethodCode.Statements);
							ActualCompiledType = new MathExp.RaisTypes.RaisDataType(targetType.BaseClassType);
						}
					}
				}
			}
			else
			{
				ce = new CodeArgumentReferenceExpression(CodeName);
			}
			return ce;
		}
		//generate client code
		public override string CreateJavaScript(StringCollection method)
		{
			string ce;
			if (_valuePointer != null)
			{
				if (this.CanBeWebClientValue())
				{
					ce = _valuePointer.GetJavaScriptReferenceCode(method);
				}
				else
				{
					//a server value downloaded
					//args.{0}
					ce = string.Format(System.Globalization.CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", DataPassingCodeName);
				}
			}
			else
			{
				ce = CodeName;
			}
			if (DataType != null && typeof(bool).Equals(this.DataType.LibType))
			{
				ce = string.Format(System.Globalization.CultureInfo.InvariantCulture, "JsonDataBinding.isValueTrue({0})", ce);
			}
			return ce;
		}
		//generate server php code
		public override string CreatePhpScript(StringCollection method)
		{
			string ce;
			if (_valuePointer != null)
			{
				if (DesignUtil.IsDataFieldPointer(_valuePointer))
				{
					ce = _valuePointer.GetPhpScriptReferenceCode(method);
				}
				else if (this.CanBeWebServerValue())
				{
					//a server value
					ce = _valuePointer.GetPhpScriptReferenceCode(method);
				}
				else
				{
					//a client value uploaded
					ce = string.Format(System.Globalization.CultureInfo.InvariantCulture, "$this->jsonFromClient->values->{0}", DataPassingCodeName);
				}
			}
			else
			{
				ce = CodeName;
			}
			return ce;
		}
		public override void OnSetScopeMethod(IMethod m)
		{
			LocalVariable lv = _valuePointer as LocalVariable;
			if (lv != null)
			{
				lv.Owner = m as IObjectPointer;
			}
		}
		/// <summary>
		/// do nothing
		/// </summary>
		/// <param name="replaced">the node replaced by this</param>
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		protected override void OnSave(XmlNode node)
		{
			XmlObjectWriter xw = this.root.Writer as XmlObjectWriter;
			if (xw != null)
			{
				OnWriteToXmlNode(xw, node);
			}
		}
		protected override void OnLoad(XmlNode node)
		{
			XmlObjectReader xr = this.root.Reader as XmlObjectReader;
			if (xr != null)
			{
				OnReadFromXmlNode(xr, node);
			}
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodePointer node = (MathNodePointer)base.CloneExp(parent);
			if (_valuePointer != null)
			{
				node.Property = (IObjectPointer)_valuePointer.Clone();
			}
			return node;
		}
		public override void OnDoubleClick(Control host)
		{
			Form f = host.FindForm();
			host.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			if (f != null)
			{
				f.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			}
			IObjectPointer pp = _valuePointer;
			IMethod method = null;
			MathNodeRoot r = this.root;
			if (r != null)
			{
				method = r.ScopeMethod;
			}
			FrmObjectExplorer dlg = DesignUtil.GetPropertySelector(pp, method, null);
			if (dlg != null)
			{
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					_valuePointer = dlg.SelectedObject as IObjectPointer;
					host.Invalidate();
				}
			}
			host.Cursor = System.Windows.Forms.Cursors.Default;
			if (f != null)
			{
				f.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}
		#endregion
		#region IDataScope Members
		[Browsable(false)]
		public Type ScopeDataType
		{
			get
			{
				if (_valuePointer != null)
					return _valuePointer.ObjectType;
				return typeof(double);
			}
			set
			{
				if (_valuePointer != null)
					_valuePointer.ObjectType = value;
			}
		}
		[Browsable(false)]
		public IObjectPointer ScopeOwner
		{
			get
			{
				return _valuePointer;
			}
			set
			{
				_valuePointer = value;
			}
		}

		#endregion
		#region IPropertyPointer Members
		[Browsable(false)]
		public string PropertyName
		{
			get
			{
				if (_valuePointer != null)
					return _valuePointer.ReferenceName;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_valuePointer != null && _valuePointer.ObjectKey != null)
				{
					return "h_" + ((uint)(_valuePointer.ObjectKey.GetHashCode())).ToString("x", CultureInfo.InvariantCulture);
				}
				throw new DesignerException("Cannot generate CodeVariableName for MathNodePointer. _valuePointer:{0}", _valuePointer);
			}
		}
		[Browsable(false)]
		public Type ObjectType
		{
			get
			{
				return ScopeDataType;
			}
			set
			{
				ScopeDataType = value;
			}
		}
		[Browsable(false)]
		public object ObjectInstance
		{
			get
			{
				return _valuePointer.ObjectInstance;
			}
			set
			{
				_valuePointer.ObjectInstance = value;
			}
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			if (_valuePointer != null)
			{
				XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				node.AppendChild(nd);
				writer.WriteObjectToNode(nd, _valuePointer);
			}
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode(XmlTags.XML_PROPERTY);
			if (nd != null)
			{
				_valuePointer = (IObjectPointer)reader.ReadObject(nd, null);
			}
		}

		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return false; } }
		public bool CanWrite
		{
			get
			{
				if (_valuePointer != null)
				{
					PropertyPointer ip = _valuePointer as PropertyPointer;
					if (ip != null)
					{
						return ip.CanWrite;
					}
					else
					{
						CustomPropertyPointer cpp = _valuePointer as CustomPropertyPointer;
						if (cpp != null)
						{
							return cpp.CanWrite;
						}
					}
				}
				ISourceValuePointer sp = _valuePointer as ISourceValuePointer;
				if (sp != null)
				{
					return sp.CanWrite;
				}
				return true;
			}
		}
		public UInt32 TaskId { get { return _taskId; } }
		[Browsable(false)]
		public object ValueOwner
		{
			get
			{
				return _valuePointer;
			}
		}
		public void SetTaskId(UInt32 taskId)
		{
			_taskId = taskId;
			ISourceValuePointer sp = _valuePointer as ISourceValuePointer;
			if (sp != null)
			{
				sp.SetTaskId(taskId);
			}
		}
		public void SetValueOwner(object o)
		{
			IObjectPointer p = o as IObjectPointer;
			if (p != null)
			{
				if (_valuePointer == null)
				{
				}
				else
				{
					if (_valuePointer.Owner == null)
					{
					}
				}
			}
		}
		[Browsable(false)]
		public bool IsWebClientValue()
		{
			if (_valuePointer == null)
				return false;
			return DesignUtil.IsWebClientObject(_valuePointer);
		}
		[Browsable(false)]
		public bool CanBeWebClientValue()
		{
			if (_valuePointer == null)
				return false;
			return DesignUtil.CanBeWebClientObject(_valuePointer);
		}
		[Browsable(false)]
		public bool IsWebServerValue()
		{
			if (_valuePointer == null)
				return false;
			return DesignUtil.IsWebServerObject(_valuePointer);
		}
		[Browsable(false)]
		public bool CanBeWebServerValue()
		{
			if (_valuePointer == null)
				return false;
			return DesignUtil.CanBeWebServerObject(_valuePointer);
		}
		[Browsable(false)]
		public bool IsSameProperty(ISourceValuePointer p)
		{
			if (_valuePointer != null)
			{
				IObjectPointer vp = p as IObjectPointer;
				if (vp != null)
				{
					return _valuePointer.IsSameObjectRef(vp);
				}
				MathNodePointer mp = p as MathNodePointer;
				if (mp != null)
				{
					return _valuePointer.IsSameObjectRef(mp._valuePointer);
				}
			}
			return false;
		}
		[Browsable(false)]
		public string DataPassingCodeName
		{
			get
			{
				ISourceValuePointer sp = _valuePointer as ISourceValuePointer;
				if (sp != null)
				{
					return sp.DataPassingCodeName;
				}
				if (_valuePointer != null && _valuePointer.ObjectKey != null)
				{
					return CompilerUtil.CreateJsCodeName(_valuePointer, _taskId);
				}
				throw new DesignerException("Cannot generate DataPassingCodeName for MathNodePointer. _valuePointer:{0}", _valuePointer);
			}
		}
		#endregion

		#region IDatabaseFieldPointer Members
		[Browsable(false)]
		public bool IsdatabaseField
		{
			get
			{
				if (_valuePointer != null)
				{
					if (_valuePointer.Owner != null)
					{
						IDatabaseFieldProvider dbf = _valuePointer.Owner.ObjectInstance as IDatabaseFieldProvider;
						return true;
					}
				}
				return false;
			}
		}
		bool _forValue = true;
		public void SetCompileForValue(bool forValue)
		{
			_forValue = forValue;
		}
		#endregion
	}
}
