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
using MathExp;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Event;
using System.Drawing;
using System.Windows.Forms;
using VPL;
using WindowsUtility;

namespace LimnorDesigner.EventMap
{
	public class ComponentIconMethod : ComponentIconEvent
	{
		#region fields and constructors
		private MethodClass _method;
		public ComponentIconMethod()
		{
			OnSetImage();
		}
		#endregion
		#region private methods
		private void mi_edit(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				UInt32 abId = 0;
				EventHandlerMethod ehm = _method as EventHandlerMethod;
				if (ehm != null)
				{
					abId = ehm.ActionBranchId;
				}
				_method.Edit(abId, ep.RectangleToScreen(this.Bounds), ep.Panes.Loader, this.FindForm());
			}
		}
		private void mi_makeCopy(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				MethodClass newMethod = root.MakeMethodCopy(_method, this.FindForm());
				if (newMethod != null)
				{
					ComponentIconMethod cim = ep.getMethodComponentIcon(newMethod.MemberId);
					if (cim != null)
					{
						cim.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
					}
				}
			}
		}
		private void mi_delete(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				root.AskDeleteMethod(_method, this.FindForm());
			}
		}
		private void mi_createAction(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				IAction act = _method.CreateAction(ep.Panes.Loader, null, null, this.FindForm());
				if (act != null)
				{
				}
			}
		}
		#endregion
		#region Methods
		public override bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			Dictionary<string, MethodClass> methods = root.CustomMethods;
			foreach (MethodClass mc in methods.Values)
			{
				if (IsForThePointer(mc))
				{
					Init(designer, root);
					Method = mc;
					return true;
				}
			}

			return false;
		}
		protected override void OnSelectByMouseDown()
		{
			if (DesignerPane != null && DesignerPane.PaneHolder != null)
			{
				DesignerPane.PaneHolder.OnMethodSelected(Method);
			}
			if (DesignerPane != null && DesignerPane.Loader != null)
			{
				DesignerPane.Loader.NotifySelection(Method);
			}
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			//
			MenuItemWithBitmap mi;

			if (!(this is ComponentIconEventhandle))
			{
				mi = new MenuItemWithBitmap("Create action", mi_createAction, Resources._newMethodAction.ToBitmap());
				mnu.MenuItems.Add(mi);
			}
			//
			mi = new MenuItemWithBitmap("Edit method", mi_edit, Resources._method.ToBitmap());
			mnu.MenuItems.Add(mi);
			//
			if (!(this is ComponentIconEventhandle))
			{
				mnu.MenuItems.Add("-");
				mi = new MenuItemWithBitmap("Make a new copy", mi_makeCopy, Resources._copy.ToBitmap());
				mnu.MenuItems.Add(mi);
			}
			//
			mnu.MenuItems.Add("-");
			//
			mi = new MenuItemWithBitmap("Delete method", mi_delete, Resources._cancel.ToBitmap());
			mnu.MenuItems.Add(mi);
		}
		public override bool IsForThePointer(IObjectPointer pointer)
		{
			MethodClass ic = pointer as MethodClass;
			if (ic != null)
			{
				return (ic.MethodID == this.MethodId);
			}
			return false;
		}
		public override bool IsActionExecuter(IAction act, ClassPointer root)
		{
			if (act.ActionMethod is CustomMethodPointer)
			{
				return (act.ExecuterMemberId == this.ClassPointer.MemberId);
			}
			return false;
		}
		/// <summary>
		/// this control is already added to Parent.Controls.
		/// 1. remove invalid inports
		/// 2. add missed EventPortIn
		/// </summary>
		/// <param name="viewer"></param>
		public override void Initialize(EventPathData eventData)
		{
			ClassPointer root = this.ClassPointer.RootPointer;
			List<EventAction> ehs = root.EventHandlers;
			if (ehs != null && ehs.Count > 0)
			{
				if (DestinationPorts == null)
				{
					DestinationPorts = new List<EventPortIn>();
				}
				else
				{
					//remove invalid inport
					List<EventPortIn> invalidInports = new List<EventPortIn>();
					foreach (EventPortIn pi in DestinationPorts)
					{
						bool bFound = false;
						foreach (EventAction ea in ehs)
						{
							if (pi.Event.IsSameObjectRef(ea.Event))
							{
								if (pi.Event.IsSameObjectRef(ea.Event))
								{
									if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
									{
										foreach (TaskID tid in ea.TaskIDList)
										{
											if (tid.IsEmbedded)
											{
												HandlerMethodID hid = tid as HandlerMethodID;
												if (hid != null)
												{
													if (hid.ActionId == this.MethodId)
													{
														bFound = true;
														break;
													}
												}
											}
											else
											{
												IAction a = root.GetActionInstance(tid.ActionId);//only public actions in map
												if (a != null)
												{
													CustomMethodPointer cmp = a.ActionMethod as CustomMethodPointer;
													if (cmp != null)
													{
														if (cmp.MemberId == this.MethodId)
														{
															bFound = true;
															break;
														}
													}
												}
											}
										}
									}
								}
								if (bFound)
								{
									break;
								}
							}
						}
						if (!bFound)
						{
							invalidInports.Add(pi);
						}
					}
					if (invalidInports.Count > 0)
					{
						foreach (EventPortIn pi in invalidInports)
						{
							DestinationPorts.Remove(pi);
						}
					}
				}
				//add missed EventPortIn
				foreach (EventAction ea in ehs)
				{
					if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
					{
						foreach (TaskID tid in ea.TaskIDList)
						{
							HandlerMethodID hid = tid as HandlerMethodID;
							if (hid != null)
							{
								if (hid.ActionId == this.MethodId)
								{
									bool bFound = false;
									foreach (EventPortIn pi in DestinationPorts)
									{
										if (pi.Event.IsSameObjectRef(ea.Event))
										{
											bFound = true;
											break;
										}
									}
									if (!bFound)
									{
										EventPortIn pi = new EventPortIn(this);

										pi.Event = ea.Event;
										double x, y;
										ComponentIconEvent.CreateRandomPoint(Width + ComponentIconEvent.PortSize, out x, out y);
										pi.Location = new Point((int)(Center.X + x), (int)(Center.Y + y));
										pi.SetLoaded();
										pi.SaveLocation();
										DestinationPorts.Add(pi);
									}
								}
							}
							else
							{
								//it is a root scope action
								IAction a = root.GetActionInstance(tid.ActionId);
								if (a == null)
								{
									MathNode.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture,
										"Action [{0}] not found", tid));
								}
								else
								{
									CustomMethodPointer cmp = a.ActionMethod as CustomMethodPointer;
									if (cmp != null)
									{
										if (cmp.MemberId == this.MethodId)
										{
											bool bFound = false;
											foreach (EventPortIn pi in DestinationPorts)
											{
												if (pi.Event.IsSameObjectRef(ea.Event))
												{

													bFound = true;
													break;

												}
											}
											if (!bFound)
											{
												EventPortIn pi = new EventPortIn(this);
												pi.Event = ea.Event;
												double x, y;
												ComponentIconEvent.CreateRandomPoint(Width + ComponentIconEvent.PortSize, out x, out y);
												pi.Location = new Point((int)(Center.X + x), (int)(Center.Y + y));
												pi.SetLoaded();
												pi.SaveLocation();
												DestinationPorts.Add(pi);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		protected override void OnSetImage()
		{
			if (_method == null && Designer != null)
			{
				ClassPointer root = Designer.GetRootId();
				_method = root.GetCustomMethodById(MethodId);
			}
			if (_method != null)
			{
				if (_method.Project.IsWebApplication)
				{
					if (_method.RunAt == EnumWebRunAt.Client)
					{
						SetIconImage(Resources._webClientMethod.ToBitmap());
					}
					else
					{
						SetIconImage(Resources._webServerMethod.ToBitmap());
					}
				}
				else
				{
					SetIconImage(Resources._method.ToBitmap());
				}
			}
			else
			{
				SetIconImage(Resources._method.ToBitmap());
			}
		}
		const string XMLATTR_methodId = "methodId";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATTR_methodId, MethodId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			MethodId = XmlUtil.GetAttributeUInt(node, XMLATTR_methodId);
		}
		#endregion
		#region Properties
		/// <summary>
		/// for method
		/// </summary>
		public override bool IsForComponent
		{
			get
			{
				return false;
			}
		}
		public UInt32 MethodId
		{
			get;
			set;
		}
		public MethodClass Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
				MethodId = _method.MemberId;
				SetLabelText(_method.Name);
				OnSetImage();
			}
		}
		public virtual string DisplayName
		{
			get
			{
				if (_method == null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"MethodID:{0}", MethodId);
				}
				return _method.MethodName;
			}
		}
		#endregion
	}
	public class ComponentIconEventhandle : ComponentIconMethod
	{
		#region fields and constructors
		public ComponentIconEventhandle()
		{
		}
		#endregion
		public override string DisplayName
		{
			get
			{
				EventHandlerMethod h = Method as EventHandlerMethod;
				if (h != null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}.{1}", h.Event.LongDisplayName, h.MethodName);
				}
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Handler:{0}", MethodId);
			}
		}
		#region Methods
		public override bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			List<EventAction> handlers = root.EventHandlers;
			if (handlers != null && handlers.Count > 0)
			{
				foreach (EventAction ea in handlers)
				{
					if (ea.TaskIDList != null && ea.TaskIDList.Count > 0)
					{
						foreach (TaskID tid in ea.TaskIDList)
						{
							HandlerMethodID hid = tid as HandlerMethodID;
							if (hid != null)
							{
								if (MethodId == hid.ActionId)
								{
									Init(designer, root);
									Method = hid.HandlerMethod;
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
		protected override void OnSetImage()
		{
			EventHandlerMethod em = Method as EventHandlerMethod;
			if (em != null)
			{
				if (em.ForAllTypes)
				{
					SetIconImage(Resources._methodActionForAll.ToBitmap());
					return;
				}
			}
			SetIconImage(Resources._methodAction.ToBitmap());
		}
		public override bool IsActionExecuter(IAction act, ClassPointer root)
		{
			return false;
		}
		public override bool IsActionExecuter(TaskID tid, ClassPointer root)
		{
			HandlerMethodID hid = tid as HandlerMethodID;
			if (hid != null)
			{
				return (hid.ActionId == this.MethodId);
			}
			return false;
		}
		#endregion
	}
}
