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
using System.ComponentModel;
using System.Reflection;
using Parser;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;
using XmlSerializer;
using MathExp;
using System.CodeDom;
using VPL;
using TraceLog;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// a custom constructor
	/// </summary>
	public class ConstructorClass : MethodClass
	{
		#region fields and constructors
		private int _fixedParameterCount; //the first <n> parameters are from base constructor
		private string _display;
		private ConstructorInfo _info;
		public ConstructorClass(ClassPointer owner)
			: base(owner)
		{
			Name = ConstObjectPointer.VALUE_Constructor;
		}
		public ConstructorClass(ClassPointer owner, ConstructorInfo info)
			: base(owner)
		{
			_info = info;
			StringBuilder sb = new StringBuilder("(");
			List<ParameterClass> ps = new List<ParameterClass>();
			ParameterInfo[] pifs = info.GetParameters();
			if (pifs != null && pifs.Length > 0)
			{
				for (int i = 0; i < pifs.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(",");
					}
					sb.Append(pifs[i].ParameterType.Name);
					ParameterClass p = new ParameterClass(pifs[i].ParameterType, pifs[i].Name, this);
					ps.Add(p);
				}
				this.Parameters = ps;
				_fixedParameterCount = ps.Count;
			}
			else
			{
				_fixedParameterCount = 0;
			}
			sb.Append(")");
			_display = sb.ToString();
		}
		#endregion
		#region ICloneable Members
		protected override void CopyFromThis(MethodClass obj)
		{
			base.CopyFromThis(obj);
			ConstructorClass constructor = (ConstructorClass)obj;
			constructor._info = _info;
			constructor._fixedParameterCount = _fixedParameterCount;
			constructor._display = _display;
		}
		#endregion
		#region Properties
		public override string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(_display))
				{
					StringBuilder sb = new StringBuilder("(");
					if (Parameters != null)
					{
						for (int i = 0; i < Parameters.Count; i++)
						{
							if (i > 0)
							{
								sb.Append(",");
							}
							sb.Append(this.Parameters[i].CodeName);
						}
					}
					sb.Append(")");
					_display = sb.ToString();
				}
				return _display;
			}
		}
		[Browsable(false)]
		protected override string XmlTag
		{
			get
			{
				return XmlTags.XML_Item;
			}
		}
		[Browsable(false)]
		public override bool HasReturn
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public override ParameterClass ReturnValue
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public ConstructorInfo Info
		{
			get
			{
				if (_info == null)
				{
					ClassPointer owner = (ClassPointer)Owner;
					ConstructorInfo[] cifs = owner.BaseClassType.GetConstructors();
					if (cifs != null && cifs.Length > 0)
					{
						SortedList<string, ConstructorInfo> sorted = new SortedList<string, ConstructorInfo>();
						for (int i = 0; i < cifs.Length; i++)
						{
							sorted.Add(VPLUtil.GetMethodSignature(cifs[i]), cifs[i]);
						}
						IEnumerator<KeyValuePair<string, ConstructorInfo>> ie = sorted.GetEnumerator();
						while (ie.MoveNext())
						{
							if (this.IsDerivedFrom(ie.Current.Value))
							{
								_info = ie.Current.Value;
								break;
							}
						}
					}
				}
				if (_info == null)
				{
					_info = typeof(object).GetConstructor(new Type[] { });
				}
				return _info;
			}
		}
		/// <summary>
		/// the first {FixedParameters} parameters are from base constructor 
		/// and form the method signature to look for the constructor.
		/// in the method editor, the fixed parameters cannot be deleted or change order.
		/// </summary>
		[Browsable(false)]
		public int FixedParameters
		{
			get
			{
				return _fixedParameterCount;
			}
			set
			{
				_fixedParameterCount = value;
			}
		}
		#endregion
		#region Methods
		public bool IsDerivedFrom(ConstructorClass baseConstructor)
		{
			if (this.ParameterCount >= baseConstructor.ParameterCount)
			{
				for (int i = 0; i < baseConstructor.ParameterCount; i++)
				{
					if (!baseConstructor.Parameters[i].IsSameObjectRef(this.Parameters[i]))
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsDerivedFrom(ConstructorInfo constructor)
		{
			ParameterInfo[] ps = constructor.GetParameters();
			if (ParameterCount < ps.Length)
				return false;
			for (int i = 0; i < ps.Length; i++)
			{
				if (this.Parameters[i].IsLibType)
				{
					if (!ps[i].ParameterType.Equals(this.Parameters[i].LibTypePointer.ClassType))
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			return true;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeObjectCreateExpression cmr = new CodeObjectCreateExpression(Declarer.TypeString);
			return cmr;
		}
		private EnumRunContext _origiContext = EnumRunContext.Server;
		public override bool Edit(UInt32 actionBranchId, Rectangle rcStart, ILimnorDesignerLoader loader, Form caller)
		{
			if (this.Owner == null)
			{
				this.Owner = loader.GetRootId();
			}
			DlgMethod dlg = this.CreateMethodEditor(rcStart);
			try
			{
				_origiContext = VPLUtil.CurrentRunContext;
				if (loader.Project.IsWebApplication)
				{
					if (this.RunAt == EnumWebRunAt.Client)
					{
						VPLUtil.CurrentRunContext = EnumRunContext.Client;
					}
					else
					{
						VPLUtil.CurrentRunContext = EnumRunContext.Server;
					}
				}
				else
				{
					VPLUtil.CurrentRunContext = EnumRunContext.Server;
				}
				dlg.LoadMethod(this, EnumParameterEditType.Edit);
				if (dlg.ShowDialog(caller) == DialogResult.OK)
				{
					_display = null;
					XmlNode nodeMethodCollection = loader.Node.SelectSingleNode(XmlTags.XML_CONSTRUCTORS);
					if (nodeMethodCollection == null)
					{
						nodeMethodCollection = loader.Node.OwnerDocument.CreateElement(XmlTags.XML_CONSTRUCTORS);
						loader.Node.AppendChild(nodeMethodCollection);
					}
					XmlNode nodeMethod = nodeMethodCollection.SelectSingleNode(
						string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}[@{1}='{2}']",
						XmlTags.XML_Item, XmlTags.XMLATT_MethodID, MemberId));
					if (nodeMethod == null)
					{
						nodeMethod = nodeMethodCollection.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nodeMethodCollection.AppendChild(nodeMethod);
					}
					XmlUtil.SetAttribute(nodeMethod, XmlTags.XMLATT_MethodID, MemberId);
					XmlUtil.SetAttribute(nodeMethod, XmlTags.XMLATT_NAME, Name);
					XmlObjectWriter wr = loader.Writer;
					wr.WriteObjectToNode(nodeMethod, this);
					if (wr.HasErrors)
					{
						MathNode.Log(wr.ErrorCollection);
					}
					ILimnorDesignPane pane = loader.Project.GetTypedData<ILimnorDesignPane>(loader.ObjectMap.ClassId);

					pane.OnNotifyChanges();
					return true;
				}
			}
			catch (Exception err)
			{
				MathNode.Log(caller, err);
			}
			finally
			{
				ExitEditor();
				VPLUtil.CurrentRunContext = _origiContext;
			}
			return false;
		}
		public override bool ReloadMethod(ILimnorDesignerLoader loader)
		{
			try
			{
				XmlNode nodeMethod = loader.Node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}[@{2}='{3}']",
					XmlTags.XML_CONSTRUCTORS, XmlTags.XML_Item, XmlTags.XMLATT_MethodID, MemberId));
				if (nodeMethod != null)
				{
					loader.Reader.ReloadObjectFromXmlNode<MethodClass>(nodeMethod, this);
					return true;
				}
				else
				{
					throw new DesignerException("Error calling ReloadMethod. Constructor node not found for id [{0},{1}]", ClassId, MemberId);
				}
			}
			catch (Exception err)
			{
				MathNode.Log(TraceLogClass.MainForm,err);
			}
			return false;
		}
		public override void RemoveMethodXmlNode(XmlNode rootNode)
		{
			XmlNode nodeMethod = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}[@{2}='{3}']",
					XmlTags.XML_CONSTRUCTORS, XmlTags.XML_Item, XmlTags.XMLATT_MethodID, MemberId));
			if (nodeMethod != null)
			{
				XmlNode p = nodeMethod.ParentNode;
				p.RemoveChild(nodeMethod);
			}
		}
		public Dictionary<string, string> GetParameterTooltips()
		{
			Dictionary<string, string> pts = null;
			ClassPointer owner = (ClassPointer)Owner;
			if (owner.BaseClassPointer != null)
			{
				SortedList<string, ConstructorClass> sorted = owner.GetBaseConstructorsSortedByParameterCount();
				for (int i = sorted.Count - 1; i >= 0; i--)
				{
					ConstructorClass baseConstructor = sorted.ElementAt<KeyValuePair<string, ConstructorClass>>(i).Value;
					if (this.IsDerivedFrom(baseConstructor))
					{
						pts = baseConstructor.GetParameterTooltips();
						break;
					}
				}
				if (pts == null)
				{
					pts = new Dictionary<string, string>();
				}
			}
			else
			{
				string methodDesc;
				string retDesc;
				pts = PMEXmlParser.GetMethodDescription(owner.BaseClassType, Info, out methodDesc, out retDesc);
			}
			if (_fixedParameterCount < ParameterCount)
			{
				for (int i = _fixedParameterCount; i < ParameterCount; i++)
				{
					if (!string.IsNullOrEmpty(Parameters[i].Description))
					{
						pts.Add(Parameters[i].Name, Parameters[i].Description);
					}
				}
			}
			return pts;
		}
		public override string ToString()
		{
			return DisplayName;
		}
		#endregion
	}
}
