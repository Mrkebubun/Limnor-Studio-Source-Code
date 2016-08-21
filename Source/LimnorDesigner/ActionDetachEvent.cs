/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using LimnorDesigner.Event;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using LimnorDatabase;
using System.Collections.Specialized;
using XmlSerializer;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner.Action
{
	public class ActionDetachEvent : ActionAttachEvent
	{
		#region fields and constructors
		private UInt32 _attachedActionId;
		public ActionDetachEvent(ClassPointer owner)
			: base(owner)
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public UInt32 AttachedActionId
		{
			get
			{
				return _attachedActionId;
			}
			set
			{
				_attachedActionId = value;
			}
		}
		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		public override string ActionName
		{
			get
			{
				loadEventAction();
				EventAction ea = AssignedActions;
				if (ea != null && ea.Event != null)
				{
					EventHandlerMethod ehm = null;
					if (ea.TaskIDList.Count > 0)
					{
						HandlerMethodID hmd = ea.TaskIDList[0] as HandlerMethodID;
						if (hmd != null)
						{
							ehm = hmd.HandlerMethod;
						}
					}
					if (ehm != null)
					{
						return string.Format(CultureInfo.InvariantCulture, "Detach_{0}_from_{1}", ehm.Name, ea.Event.ExpressionDisplay);
					}
					else
					{
						return string.Format(CultureInfo.InvariantCulture, "Detach_?_from_{0}", ea.Event.ExpressionDisplay);
					}
				}
				else
				{
					return "Detach_?_from_?";
				}
			}
			set
			{
			}
		}
		[Browsable(false)]
		public override bool IsAttach
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region Methods
		const string XMLATT_attachId = "attachId";
		public override void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			base.OnBeforeRead(reader, node);
			_attachedActionId = XmlUtil.GetAttributeUInt(node, XMLATT_attachId);
		}
		public override void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			base.OnBeforeWrite(writer, node);
			XmlUtil.SetAttribute(node, XMLATT_attachId, _attachedActionId);
		}
		protected override void loadEventAction()
		{
			if (AssignedActions0 == null)
			{
				if (_attachedActionId != 0)
				{
					ActionAttachEvent aae = Class.GetActionInstance(_attachedActionId) as ActionAttachEvent;
					if (aae != null)
					{
						SetHandlerOwner(aae.AssignedActions);
					}
				}
			}
		}
		[Browsable(false)]
		public override void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
		{
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				ClassPointer root = Class;
				CodeExpression methodTarget;
				CodeEventReferenceExpression ceRef = ea.Event.GetReferenceCode(methodToCompile, statements, false) as CodeEventReferenceExpression;
				if (ea.Event.IsStatic)
					methodTarget = new CodeTypeReferenceExpression(root.CodeName);
				else
					methodTarget = new CodeThisReferenceExpression();
				CodeRemoveEventStatement caes = new CodeRemoveEventStatement(ceRef,
										new CodeDelegateCreateExpression(new CodeTypeReference(ea.Event.EventHandlerType.TypeString),
											methodTarget, ea.GetLocalHandlerName()));
				statements.Add(caes);
			}
		}
		[Browsable(false)]
		public override void CreateJavaScript(StringCollection methodCode, Dictionary<string, StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			EventAction ea = AssignedActions;
			if (ea != null)
			{
				if (ea.IsExtendWebClientEvent())
				{
					methodCode.Add("JsonDataBinding.detachExtendedEvent('");
					methodCode.Add(ea.Event.Name);
					methodCode.Add("','");
					EasyDataSet eds = ea.Event.Owner.ObjectInstance as EasyDataSet;
					if (eds != null)
					{
						methodCode.Add(eds.TableName);
					}
					else
					{
						methodCode.Add(ea.Event.Owner.CodeName);
					}
					methodCode.Add("',");
					methodCode.Add(ea.GetLocalHandlerName());
					methodCode.Add(");\r\n");
				}
				else
				{
					methodCode.Add("var ");
					methodCode.Add(ea.Event.Owner.CodeName);
					methodCode.Add(" = document.getElementById('");
					methodCode.Add(ea.Event.Owner.CodeName);
					methodCode.Add("');\r\n");
					//
					methodCode.Add("JsonDataBinding.DetachEvent(");
					methodCode.Add(ea.Event.Owner.CodeName);
					methodCode.Add(",'");
					methodCode.Add(ea.Event.Name);
					methodCode.Add("',");
					methodCode.Add(ea.GetLocalHandlerName());
					methodCode.Add(");\r\n");
				}
			}
		}
		#endregion
	}
}
