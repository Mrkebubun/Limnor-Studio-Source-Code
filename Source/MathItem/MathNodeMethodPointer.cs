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
using LimnorDesigner;
using MathExp.RaisTypes;
using System.Reflection;
using System.CodeDom;
using XmlSerializer;
using System.Xml;
using System.Drawing.Design;
using XmlUtility;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Action;
using System.Windows.Forms;
using LimnorDesigner.Property;
using ProgElements;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;

namespace MathItem
{
	/// <summary>
	/// represent a method that returns a value
	/// </summary>
	[ProjectItem]
	[UseModalSelector]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[MathNodeCategoryAttribute(enumOperatorCategory.System)]
	[Description("A method call")]
	[ToolboxBitmapAttribute(typeof(MathNodeMethodPointer), "Resources.method.bmp")]
	public class MathNodeMethodPointer : MathNode, IXmlNodeSerializable, IMethodPointerHolder, ISourceValuePointer, ISourceValueEnumProvider, INamedMathNode, IGenericTypePointer, IMethodPointerNode
	{
		#region fields and constructors
		private IActionMethodPointer _methodPointer;
		private UInt32 _id;
		private UInt32 _taskId;
		public MathNodeMethodPointer(MathNode parent)
			: base(parent)
		{
		}
		static MathNodeMethodPointer()
		{
			XmlUtil.AddKnownType("MathNodeMethodPointer", typeof(MathNodeMethodPointer));
		}
		#endregion
		#region public properties
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
		[Editor(typeof(TypeEditorMethodPointer), typeof(UITypeEditor))]
		[Description("The method to be executed")]
		public IActionMethodPointer Method
		{
			get
			{
				return _methodPointer;
			}
			set
			{
				_methodPointer = value;
				InitializeChildren();
			}
		}
		[Browsable(false)]
		public UInt32 ID
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _id;
			}
		}
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.IsValid;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_methodPointer is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return true;
			}
		}
		#endregion
		#region private properties
		private string methodDisplayName
		{
			get
			{
				if (_methodPointer == null)
					return "?";
				if (_methodPointer.IsArrayMethod)
				{
					if (_methodPointer.Owner != null)
					{
						return _methodPointer.Owner.ExpressionDisplay;
					}
					else
					{
						return "[]?";
					}
				}
				if (_methodPointer.Owner == null)
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "?.{0}", _methodPointer.MethodName);
				else
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", _methodPointer.Owner.ExpressionDisplay, _methodPointer.MethodName);
			}
		}
		private bool isArrayMethod
		{
			get
			{
				if (_methodPointer == null)
					return false;
				return _methodPointer.IsArrayMethod;
			}
		}
		#endregion
		#region public methods
		public override string ToString()
		{
			if (_methodPointer == null)
				return "?";
			StringBuilder s = new StringBuilder(methodDisplayName);
			if (isArrayMethod)
			{
				s.Append("[");
			}
			else
			{
				s.Append("(");
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				if (i > 0)
				{
					s.Append(",");
				}
				s.Append(this[i].ToString());
			}
			if (isArrayMethod)
			{
				s.Append("]");
			}
			else
			{
				s.Append(")");
			}
			return s.ToString();
		}
		#endregion
		#region MathNode implementation
		public override Type GetDefaultChildType(int i)
		{
			if (_methodPointer != null && i >= 0 && i < ChildNodeCount)
			{
				object vt = _methodPointer.GetParameterTypeByIndex(i);
				if (vt != null)
				{
					Type ptp = null;
					ParameterInfo pif = vt as ParameterInfo;
					if (pif != null)
					{
						ptp = pif.ParameterType;
					}
					else
					{
						ParameterClass pc = vt as ParameterClass;
						if (pc != null)
						{
							ptp = pc.BaseClassType;
						}
						else
						{
							DataTypePointer dt = vt as DataTypePointer;
							if (dt != null)
							{
								ptp = dt.BaseClassType;
							}
							else
							{
								IParameter ip = vt as IParameter;
								if (ip != null)
								{
									ptp = ip.ParameterLibType;
								}
							}
						}
					}
					if (ptp != null)
					{
						return ptp;
					}
				}
			}

			return this.DataType.Type;
		}
		[Browsable(false)]
		public override RaisDataType DataType
		{
			get
			{
				if (_methodPointer == null)
					return new RaisDataType(typeof(object));
				return new RaisDataType(_methodPointer.ReturnBaseType);
			}
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get
			{
				if (_methodPointer == null)
					return "?";
				return _methodPointer.ToString();
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		/// <summary>
		/// ChildNodeCount is the number of method parameters
		/// </summary>
		protected override void InitializeChildren()
		{
			if (_methodPointer == null)
			{
				ChildNodeCount = 0;
			}
			else
			{
				ChildNodeCount = _methodPointer.ParameterCount;
			}
		}
		private void checkTarget(TreeNodeObject node, BoolEventArgs e)
		{
			e.Result = false;
			TreeNodeCustomMethodPointer tncmp = node as TreeNodeCustomMethodPointer;
			if (tncmp != null)
			{
				if (tncmp.Method != null)
				{
					if (tncmp.Method.ReturnBaseType != null && !typeof(void).IsAssignableFrom(tncmp.Method.ReturnBaseType))
					{
						e.Result = true;
					}
				}
			}
			else
			{
				IActionMethodPointer mp = node.OwnerPointer as IActionMethodPointer;
				if (mp != null)
				{
					if (!mp.NoReturn)
					{
						e.Result = true;
					}
				}
				else
				{
					MethodClass mc = node.OwnerPointer as MethodClass;
					if (mc != null && mc.MethodReturnType != null)
					{
						if (!(typeof(void).Equals(mc.MethodReturnType.ParameterLibType)))
						{
							e.Result = true;
						}
					}
				}
			}
		}
		public override void OnDoubleClick(Control host)
		{
			Form f = host.FindForm();
			if (f != null)
			{
				f.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			}
			host.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			IMethod m = null;
			if (_methodPointer != null)
			{
				m = _methodPointer.MethodPointed;
			}
			MethodClass scopeMethod = null;
			MathExpCtrl mctrl = host as MathExpCtrl;
			if (mctrl != null)
			{
				scopeMethod = mctrl.ScopeMethod as MethodClass;
				if (scopeMethod == null)
				{
					MathNodeRoot r = mctrl.Root;
					if (r != null)
					{
						scopeMethod = r.ScopeMethod as MethodClass;
					}
				}
			}
			FrmObjectExplorer dlg = DesignUtil.CreateSelectMethodDialog(scopeMethod, m);
			if (dlg != null)
			{
				dlg.SetCheckTaget(checkTarget);
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					IAction act = null;
					if (_methodPointer != null)
					{
						act = _methodPointer.Action;
					}
					MethodClass mc = dlg.SelectedObject as MethodClass;
					if (mc != null)
					{
						_methodPointer = mc.CreateMethodPointer(act) as IActionMethodPointer;
					}
					else
					{
						_methodPointer = (IActionMethodPointer)dlg.SelectedObject;
					}
					_methodPointer.Action = act;
					ChildNodeCount = _methodPointer.ParameterCount;
					host.Invalidate();
				}
			}
			host.Cursor = System.Windows.Forms.Cursors.Default;
			if (f != null)
			{
				f.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}
		public override SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			string name = methodDisplayName;
			SizeF size0 = g.MeasureString(name, this.TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			SizeF size2 = g.MeasureString(",", TextFont);
			float w = size0.Width + size1.Width + size1.Width;
			float h = size0.Height;
			if (h < size1.Height)
				h = size1.Height;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].CalculateDrawSize(g);
				w = w + size.Width;
				if (h < size.Height)
					h = size.Height;
			}
			if (n > 1)
			{
				w = w + (n - 1) * size2.Width;
			}
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			string s = methodDisplayName;
			string s1, s2;
			if (isArrayMethod)
			{
				s1 = "[";
				s2 = "]";
			}
			else
			{
				s1 = "(";
				s2 = ")";
			}
			SizeF size0 = g.MeasureString(s, TextFont);
			SizeF size1 = g.MeasureString(s1, TextFont);
			SizeF size2 = g.MeasureString(",", TextFont);
			float w = DrawSize.Width;
			float h = DrawSize.Height;
			float y, x = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, size0.Width + size1.Width, h);
				y = 0;
				if (size0.Height < h)
					y = (h - size0.Height) / (float)2;
				g.DrawString(s, TextFont, TextBrushFocus, x, y);
				x = size0.Width;
				y = 0;
				if (size1.Height < h)
					y = (h - size1.Height) / (float)2;
				g.DrawString(s1, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				y = 0;
				if (size0.Height < h)
					y = (h - size0.Height) / (float)2;
				g.DrawString(s, TextFont, TextBrush, x, y);
				x = size0.Width;
				y = 0;
				if (size1.Height < h)
					y = (h - size1.Height) / (float)2;
				g.DrawString(s1, TextFont, TextBrush, x, y);
			}
			x = size0.Width + size1.Width;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].CalculateDrawSize(g);
				if (i > 0)
				{
					if (IsFocused)
					{
						g.FillRectangle(TextBrushBKFocus, x, (float)0, size2.Width, h);
						g.DrawString(",", TextFont, TextBrushFocus, x, h - size2.Height);
					}
					else
					{
						g.DrawString(",", TextFont, TextBrush, x, h - size2.Height);
					}
					x = x + size2.Width;
				}
				y = h - this[i].DrawSize.Height;
				System.Drawing.Drawing2D.GraphicsState gt = g.Save();
				g.TranslateTransform(x, y);
				this[i].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
				this[i].Draw(g);
				g.Restore(gt);
				x = x + this[i].DrawSize.Width;
			}
			//
			y = 0;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, size1.Width, h);
				g.DrawString(s2, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(s2, TextFont, TextBrush, x, y);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			if (_methodPointer == null)
			{
				MathNode.Trace("Warning: method pointer is null");
				return null;
			}
			bool isWeb = false;
			if (_methodPointer.Owner != null && _methodPointer.Owner.RootPointer != null)
			{
				isWeb = _methodPointer.Owner.RootPointer.IsWebPage;
			}
			if (isWeb && _methodPointer.RunAt == EnumWebRunAt.Client)
			{
				CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
						new CodeVariableReferenceExpression("clientRequest"), "GetStringValue", new CodePrimitiveExpression(DataPassingCodeName));
				return cmie;
			}
			if (method == null)
			{
				throw new DesignerException("Calling {0}.ExportCode with null method", this.GetType().Name);
			}
			if (method.MethodCode == null)
			{
				throw new DesignerException("Calling {0}.ExportCode with null method.MethodCode", this.GetType().Name);
			}
			CodeStatementCollection supprtStatements = method.MethodCode.Statements;
			int n = ChildNodeCount;
			CodeExpression[] ps;
			if (n > 0)
			{
				IList<IParameter> pList = _methodPointer.MethodPointed.MethodParameterTypes; ;

				//_methodPointer.MethodPointed.
				ps = new CodeExpression[n];
				for (int i = 0; i < n; i++)
				{
					this[i].CompileDataType = new RaisDataType(pList[i].ParameterLibType);
					if (typeof(string).Equals(pList[i].ParameterLibType))
					{
						if (this[i].Parent != null)
						{
							MathNodeMethodPointer mmp = this[i].Parent as MathNodeMethodPointer;
							if (mmp != null && mmp.Method != null && mmp.Method.Owner != null)
							{
								if (typeof(DateTime).Equals(mmp.Method.Owner.ObjectType))
								{
									if (this[i].ActualCompiledType != null && typeof(CultureInfo).Equals(this[i].ActualCompiledType.LibType))
									{
										this[i].CompileDataType = this[i].ActualCompiledType;
									}
								}
							}
						}
					}
					ps[i] = this[i].ExportCode(method);
				}
			}
			else
			{
				ps = new CodeExpression[] { };
			}
			_methodPointer.SetParameterExpressions(ps);
			CodeExpression ce = _methodPointer.GetReferenceCode(method, supprtStatements, true);
			if (_methodPointer.ReturnBaseType != null && this.CompileDataType != null && this.CompileDataType.Type != null)
			{
				DataTypePointer targetType = new DataTypePointer(this.CompileDataType.Type);
				DataTypePointer sourceType = new DataTypePointer(_methodPointer.ReturnBaseType);
				IGenericTypePointer igp = _methodPointer as IGenericTypePointer;
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
					ce = CompilerUtil.GetTypeConversion(targetType, ce, sourceType, supprtStatements);
				}
			}
			return ce;
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			if (this.IsWebClientValue())
			{
				if (_methodPointer == null)
				{
					MathNode.Trace("Warning: method pointer is null");
					return null;
				}
				if (method == null)
				{
					throw new DesignerException("Calling {0}.CreateJavaScript with null method", this.GetType().Name);
				}
				MethodParameterJsCodeAttribute jsCodeAttr = null;
				MethodInfoPointer mip = _methodPointer as MethodInfoPointer;
				if (mip != null)
				{
					MethodBase mb = mip.MethodDef;
					if (mb != null)
					{
						object[] vs = mb.GetCustomAttributes(typeof(MethodParameterJsCodeAttribute), true);
						if (vs != null && vs.Length > 0)
						{
							jsCodeAttr = vs[0] as MethodParameterJsCodeAttribute;
						}
					}
				}
				int n = ChildNodeCount;
				string[] ps;
				if (n > 0)
				{
					IList<IParameter> pList = _methodPointer.MethodPointed.MethodParameterTypes; ;
					ps = new string[n];
					for (int i = 0; i < n; i++)
					{
						this[i].CompileDataType = new RaisDataType(pList[i].ParameterLibType);
						if (jsCodeAttr != null && jsCodeAttr.IsJsCode(pList[i].Name))
						{
							MathNodeValue mv = this[i] as MathNodeValue;
							if (mv != null)
							{
								if (mv.Value != null)
									ps[i] = mv.Value.ToString();
								else
									ps[i] = string.Empty;
								continue;
							}
							else
							{
								MathNodeStringValue msv = this[i] as MathNodeStringValue;
								if (msv != null)
								{
									if (msv.Value != null)
										ps[i] = msv.Value;
									else
										ps[i] = string.Empty;
									continue;
								}
							}
						}
						ps[i] = this[i].CreateJavaScript(method);
					}
				}
				else
				{
					ps = new string[] { };
				}
				_methodPointer.SetParameterJS(ps);
				string ce = _methodPointer.GetJavaScriptReferenceCode(method);
				return ce;
			}
			else
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.values.{0}", DataPassingCodeName);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			if (this.IsWebClientValue())
			{
				return string.Format(CultureInfo.InvariantCulture, "$this->jsonFromClient->values->{0}", DataPassingCodeName);
			}
			else
			{
				if (_methodPointer == null)
				{
					MathNode.Trace("Warning: method pointer is null");
					return null;
				}
				if (method == null)
				{
					throw new DesignerException("Calling {0}.CreatePhpScript with null method", this.GetType().Name);
				}
				int n = ChildNodeCount;
				string[] ps;
				if (n > 0)
				{
					IList<IParameter> pList = _methodPointer.MethodPointed.MethodParameterTypes; ;
					ps = new string[n];
					for (int i = 0; i < n; i++)
					{
						this[i].CompileDataType = new RaisDataType(pList[i].ParameterLibType);
						ps[i] = this[i].CreatePhpScript(method);
					}
				}
				else
				{
					ps = new string[] { };
				}
				_methodPointer.SetParameterPhp(ps);
				string ce = _methodPointer.GetPhpScriptReferenceCode(method);
				return ce;
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		#endregion
		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			if (_methodPointer != null)
			{
				writer.WriteObjectToNode(node, _methodPointer);
			}
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_NodeId, ID);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			_methodPointer = (IActionMethodPointer)reader.ReadObject(node, null);
			_id = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_NodeId);
		}

		#endregion
		#region MathNode serialize
		protected override MathNode OnCreateClone(MathNode parent)
		{
			MathNodeMethodPointer node = (MathNodeMethodPointer)base.OnCreateClone(parent);
			if (_methodPointer != null)
			{
				node.Method = (IActionMethodPointer)_methodPointer.Clone();
			}
			node._id = _id;
			return node;
		}
		protected override void OnSave(XmlNode node)
		{
			if (_methodPointer != null)
			{
				MathNodeRoot r = this.root;
				if (r != null)
				{
					XmlObjectWriter xw = r.Writer as XmlObjectWriter;
					if (xw != null)
					{
						XmlNode nodeMethod = node.SelectSingleNode(XmlSerialization.XML_METHOD);
						if (nodeMethod == null)
						{
							nodeMethod = node.OwnerDocument.CreateElement(XmlSerialization.XML_METHOD);
							node.AppendChild(nodeMethod);
						}
						xw.WriteObjectToNode(nodeMethod, _methodPointer);
						XmlUtil.SetAttribute(nodeMethod, XmlTags.XMLATT_NodeId, ID);
					}
					else
					{
						throw new DesignerException("Writer not available calling MathNodeMethodPointer.OnSave");
					}
				}
			}
		}
		protected override void OnLoad(XmlNode node)
		{
			MathNodeRoot r = this.root;
			if (r != null)
			{
				XmlObjectReader xr = r.Reader as XmlObjectReader;
				if (xr != null)
				{
					XmlNode nodeMethod = node.SelectSingleNode(XmlSerialization.XML_METHOD);
					if (nodeMethod != null)
					{
						_methodPointer = (IActionMethodPointer)xr.ReadObject(nodeMethod, null);
						_id = XmlUtil.GetAttributeUInt(nodeMethod, XmlTags.XMLATT_NodeId);
						InitializeChildren();
					}
				}
				else
				{
					throw new DesignerException("Reader not available calling MathNodeMethodPointer.OnLoad");
				}
			}
		}

		#endregion

		#region IMethodPointerHolder Members

		public IActionMethodPointer GetMethodPointer()
		{
			return _methodPointer;
		}

		#endregion

		#region ISourceValuePointer Members
		public bool IsMethodReturn { get { return true; } }
		public UInt32 TaskId { get { return _taskId; } }
		[Browsable(false)]
		public object ValueOwner
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.Owner;
				}
				return null;
			}
		}
		public void SetTaskId(UInt32 taskId)
		{
			_taskId = taskId;
			ISourceValuePointer sv = _methodPointer as ISourceValuePointer;
			if (sv != null)
			{
				sv.SetTaskId(taskId);
			}
		}
		public void SetValueOwner(object o)
		{
			ISourceValuePointer sv = _methodPointer as ISourceValuePointer;
			if (sv != null)
			{
				sv.SetValueOwner(o);
			}
			if (_methodPointer != null)
			{
				if (_methodPointer.Owner == null)
				{
					IObjectPointer p = o as IObjectPointer;
					if (p != null)
					{
					}
				}
			}
		}
		[Browsable(false)]
		public string DataPassingCodeName
		{
			get
			{
				ISourceValuePointer sv = _methodPointer as ISourceValuePointer;
				if (sv != null)
				{
					return sv.DataPassingCodeName;
				}
				IObjectPointer op = _methodPointer as IObjectPointer;
				if (op != null)
				{
					return CompilerUtil.CreateJsCodeName(op, _taskId);
				}
				return string.Format(CultureInfo.InvariantCulture, "j{0}_{1}", ID.ToString("x", CultureInfo.InvariantCulture), _taskId.ToString("x", CultureInfo.InvariantCulture));
			}
		}
		public bool CanWrite
		{
			get
			{
				ISourceValuePointer sv = _methodPointer as ISourceValuePointer;
				if (sv != null)
				{
					return sv.CanWrite;
				}
				return false;
			}
		}
		public bool IsSameProperty(ISourceValuePointer p)
		{
			MathNodeMethodPointer mp = p as MathNodeMethodPointer;
			if (mp != null)
			{
				return (mp.ID == this.ID);
			}
			return false;
		}
		public bool IsWebClientValue()
		{
			if (_methodPointer != null)
			{
				MethodClass mc = _methodPointer.MethodPointed as MethodClass;
				if (mc != null)
				{
					if (mc.RunAt == EnumWebRunAt.Client)
					{
						return true;
					}
				}
				else
				{
					if (_methodPointer.MethodPointed != null)
					{
						MethodInfoPointer mifp = _methodPointer.MethodPointed as MethodInfoPointer;
						if (mifp != null && mifp.MethodInfo != null)
						{
							object[] vs = mifp.MethodInfo.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
							if (vs != null && vs.Length > 0)
							{
								return true;
							}
							vs = mifp.MethodInfo.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
							if (vs != null && vs.Length > 0)
							{
								return false;
							}
							return DesignUtil.IsWebClientObject(mifp);
						}
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public bool IsWebServerValue()
		{
			if (_methodPointer != null)
			{
				MethodClass mc = _methodPointer.MethodPointed as MethodClass;
				if (mc != null)
				{
					if (mc.RunAt == EnumWebRunAt.Server)
					{
						return true;
					}
				}
				else
				{
					if (_methodPointer.MethodPointed != null)
					{
						MethodInfoPointer mifp = _methodPointer.MethodPointed as MethodInfoPointer;
						if (mifp != null && mifp.MethodInfo != null)
						{
							object[] vs = mifp.MethodInfo.GetCustomAttributes(typeof(WebServerMemberAttribute), true);
							if (vs != null && vs.Length > 0)
							{
								return true;
							}
							vs = mifp.MethodInfo.GetCustomAttributes(typeof(WebClientMemberAttribute), true);
							if (vs != null && vs.Length > 0)
							{
								return false;
							}
							return DesignUtil.IsWebServerObject(mifp);
						}
					}
				}
			}
			return false;
		}
		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_methodPointer != null)
			{
				return _methodPointer.GetReferenceCode(method, statements, forValue);
			}
			return null;
		}

		#endregion

		#region ISourceValueEnumProvider Members

		public object[] GetValueEnum(string section, string item)
		{
			if (_methodPointer != null)
			{
				ISourceValueEnumProvider svep = _methodPointer.Owner.ObjectInstance as ISourceValueEnumProvider;
				if (svep == null)
				{
					ComponentPointer cip = _methodPointer.Owner.ObjectInstance as ComponentPointer;
					if (cip != null)
					{
						svep = cip.ObjectInstance as ISourceValueEnumProvider;
					}
				}
				if (svep != null)
				{
					return svep.GetValueEnum(section, item);
				}
			}
			return null;
		}

		#endregion

		#region INamedMathNode Members

		public string Name
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.MethodName;
				}
				return string.Empty;
			}
		}

		public string GetChildNameByIndex(int idx)
		{
			if (_methodPointer != null && idx >= 0 && idx < ChildNodeCount)
			{
				object vt = _methodPointer.GetParameterTypeByIndex(idx);
				if (vt != null)
				{
					ParameterInfo pif = vt as ParameterInfo;
					if (pif != null)
					{
						return pif.Name;
					}
					else
					{
						ParameterClass pc = vt as ParameterClass;
						if (pc != null)
						{
							return pc.Name;
						}
						else
						{
							NamedDataType dt = vt as NamedDataType;
							if (dt != null)
							{
								return dt.Name;
							}
							else
							{
								IParameter ip = vt as IParameter;
								if (ip != null)
								{
									return ip.Name;
								}
							}
						}
					}
				}
			}
			return string.Empty;
		}

		#endregion

		#region IGenericTypePointer Members

		public DataTypePointer[] GetConcreteTypes()
		{
			IGenericTypePointer gtp = _methodPointer as IGenericTypePointer;
			if (gtp != null)
			{
				return gtp.GetConcreteTypes();
			}
			return null;
		}

		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			IGenericTypePointer gtp = _methodPointer as IGenericTypePointer;
			if (gtp != null)
			{
				return gtp.GetConcreteType(typeParameter);
			}
			return null;
		}

		public CodeTypeReference GetCodeTypeReference()
		{
			IGenericTypePointer gtp = _methodPointer as IGenericTypePointer;
			if (gtp != null)
			{
				return gtp.GetCodeTypeReference();
			}
			return null;
		}

		public IList<DataTypePointer> GetGenericTypes()
		{
			IGenericTypePointer gtp = _methodPointer as IGenericTypePointer;
			if (gtp != null)
			{
				return gtp.GetGenericTypes();
			}
			return null;
		}

		#endregion

		#region IMethodPointerNode Members
		public string MethodName
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.MethodName;
				}
				return null;
			}
		}

		public object MethodExecuter
		{
			get
			{
				if (_methodPointer != null)
				{
					if (_methodPointer.Owner != null && _methodPointer.Owner.ObjectInstance != null)
					{
						return _methodPointer.Owner.ObjectInstance;
					}
				}
				return null;
			}
		}
		public object MethodObject
		{
			get
			{
				return _methodPointer;
			}
		}
		public Type ReturnBaseType 
		{
			get
			{
				if (_methodPointer != null)
				{
					return _methodPointer.ReturnBaseType;
				}
				return typeof(object);
			}
		}
		#endregion
	}
}
