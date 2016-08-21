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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MathExp.RaisTypes;
using ProgElements;
using LimnorUI;
using VPL;
//using ProgElements;

namespace MathExp
{
	/// <summary>
	/// edit a compound math expression
	/// </summary>
	public partial class dlgMathExpGroupEditor : Form/*DlgAnimated*/, IMathEditor
	{
		DiagramDesignerHolder holder;
		private IXmlCodeReader _reader;
		public dlgMathExpGroupEditor(Rectangle rcStart)
			: base()
		{
			InitializeComponent();
			holder = new DiagramDesignerHolder();
			holder.Dock = DockStyle.Fill;
			this.Controls.Add(holder);
		}
		public void SetSerializers(IXmlCodeReader reader, IXmlCodeWriter writer)
		{
			_reader = reader;
			holder.SetSerializers(reader, writer);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			if (this.DialogResult != DialogResult.OK)
			{
				if (holder.Changed)
				{
					e.Cancel = UIUtil.AskCancelCloseDialog(this);
				}
			}
			if (!e.Cancel)
			{
				holder.AbortTest();
				holder.OnClosing();
				base.OnClosing(e);
			}
		}
		public void CreateViewer(MathNodeRoot item)
		{
			MathExpViewer mv = holder.AddMathViewer(item, 50, 50);
			ReturnIcon ri = holder.CreateReturnIcon(item.DataType);
			LinkLineNode.JoinToEnd((LinkLineNode)mv.OutputPorts[0].NextNode, ri.InPort);

		}
		/// <summary>
		/// for empty action
		/// </summary>
		public void CreateDefaultReturnIcon()
		{
			ReturnIcon ri = holder.CreateReturnIcon(new RaisDataType());
			ri.Location = new Point(ri.Parent.ClientSize.Width / 2 - ri.Width / 2, ri.Parent.ClientSize.Height - ri.Height);
			ri.InPort.CheckCreatePreviousNode();
			ri.Parent.Controls.Add((Control)ri.InPort.PrevNode);
			ri.InPort.CreateBackwardLine();
		}
		public void AddControls(Control[] ctrls)
		{
			holder.AddControls(ctrls);
		}
		public void CreateReturnIcon(RaisDataType t)
		{
			holder.CreateReturnIcon(t);
		}
		public void AddMathViewer(MathNodeRoot MathExpression)
		{
			holder.AddMathViewer(MathExpression, 50, 50);
		}
		protected void LoadGroup(MathExpGroup mathGroup)
		{
			holder.LoadGroup(mathGroup);
		}
		public void SetMathName(string name)
		{
			holder.SetMathName(name);
		}
		public void LoadGroupFromXmlFile(string file)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(file);
			if (string.CompareOrdinal(doc.DocumentElement.Name, "MathGroup") == 0)
			{
				LoadGroupFromXmlNode(doc.DocumentElement);
			}
			else if (doc.DocumentElement.Name == XmlSerialization.XML_Math)
			{
				MathExpEditor mathExpEditor1 = new MathExpEditor();
				mathExpEditor1.LoadFromXmlNode(doc.DocumentElement);
				this.AddMathViewer(mathExpEditor1.MathExpression);
			}
		}
		public void LoadGroupFromXmlNode(XmlNode node)
		{
			MathExpGroup g = (MathExpGroup)XmlSerialization.ReadFromXmlNode(_reader, node);
			this.LoadGroup(g);
		}
		/// <summary>
		/// parameters may be added or deleted. before showing MathExpGroup view, the parameters for the view need to be adjusted.
		/// </summary>
		/// <param name="method"></param>
		public void AdjustParameters(MethodType method)
		{
			Parameter[] ps = method.Parameters;
			//remove deleted parameters
			List<ParameterDrawing> existParameters = new List<ParameterDrawing>();
			List<ParameterDrawing> curParameters = holder.GetParameters();
			if (curParameters != null && curParameters.Count > 0)
			{
				foreach (ParameterDrawing p in curParameters)
				{
					bool bExist = false;
					if (ps != null && ps.Length > 0)
					{
						for (int i = 0; i < ps.Length; i++)
						{
							if (ps[i].ID == p.Parameter.ID)
							{
								bExist = true;
								break;
							}
						}
					}
					if (bExist)
					{
						existParameters.Add(p);
					}
					else
					{
						//remove it
						holder.RemoveParameter(p);
					}
				}
			}
			//addjust casting and add new parameters
			if (ps != null && ps.Length > 0)
			{
				int dx = 30;
				for (int i = 0; i < ps.Length; i++)
				{
					bool bExist = false;
					foreach (ParameterDrawing p in existParameters)
					{
						if (p.Parameter.ID == ps[i].ID)
						{
							bExist = true;
							if (ps[i].ProgEntity != null)
							{
								if (p.Parameter.ProgEntity == null)
								{
									p.Parameter.ProgEntity = (ProgramEntity)ps[i].ProgEntity.Clone();
								}
								else
								{
									if (!p.Parameter.ProgEntity.OwnerObject.IsSameType(ps[i].ProgEntity.OwnerObject))
									{
										p.Parameter.ProgEntity.OwnerObject = (ObjectRef)ps[i].ProgEntity.OwnerObject.Clone();
									}
								}
							}
							break;
						}
					}
					if (!bExist)
					{
						//add it
						Parameter pr = new Parameter(method, ps[i]);
						if (ps[i].ProgEntity != null)
						{
							pr.ProgEntity = (ProgramEntity)ps[i].ProgEntity.Clone();
						}
						pr.ID = ps[i].ID;
						// create ParameterDrawing and ports
						AddControls(pr.GetControls());
						pr.Location = new Point((1 + i) * dx, 30);
						pr.OutPorts[0].Location = new Point(pr.Location.X, pr.Location.Y + ParameterDrawing.DefaultDotSize);
						pr.OutPorts[0].SaveLocation();
						pr.OutPorts[0].CheckCreateNextNode();
						AddControls(new Control[] { (Control)pr.OutPorts[0].NextNode });
						pr.InitLinkLine();
					}
				}
			}

		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			holder.ExecuteOnShowHandlers();
		}

		#region IMathEditor Members
		public IMathExpression MathExpression
		{
			get
			{
				return holder.Result;
			}
			set
			{
				holder.LoadGroup((MathExpGroup)value);
			}
		}
		public void SetScopeMethod(IMethod method)
		{
			throw new NotImplementedException("SetScopeMethod for math group");
		}
		private Type _vtt;
		private IActionContext _ac;
		/// <summary>
		/// when this math expression is used in an action, this is the action.
		/// if it is a property-setting action then the method is a set-property action;
		/// if it is a method execution action then this is the method-execution action.
		/// </summary>
		public IActionContext ActionContext { get { return _ac; } set { _ac = value; } }
		/// <summary>
		/// the type for creating mapped instance
		/// currently it is typeof(ParameterValue)
		/// </summary>
		public Type VariableMapTargetType { get { return _vtt; } set { _vtt = value; } }
		public void AddService(Type t, object service)
		{
			holder.AddService(t, service);
		}
		public void DisableTest()
		{
			holder.DisableTest();
		}
		public void AddMathNode(Type type)
		{

		}
		#endregion
	}
}