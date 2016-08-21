/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Property;
using MathExp;
using System.Drawing;
using XmlUtility;
using System.Windows.Forms;
using LimnorDesigner.Interface;
using XmlSerializer;
using System.Xml;
using VPL;
using LimnorDesigner.Action;
using WindowsUtility;

namespace LimnorDesigner.EventMap
{
	public class ComponentIconProperty : ComponentIconEvent
	{
		#region fields and constructors
		private PropertyClass _property;
		public ComponentIconProperty()
		{
			OnSetImage();
		}
		#endregion
		#region private methods

		private void mi_delete(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				root.AskDeleteProperty(_property, this.FindForm());
			}
		}
		private void mi_createAction(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				ActionClass act = root.CreateSetPropertyAction(_property);
				act.ActionHolder = root;
				if (root.CreateNewAction(act, ep.Panes.Loader.Writer, null, this.FindForm()))
				{
				}
			}
		}
		void mnu_remove(object sender, EventArgs e)
		{
			EventPath ep = this.Parent as EventPath;
			if (ep != null)
			{
				ClassPointer root = ep.Panes.Loader.GetRootId();
				PropertyOverride po = Property as PropertyOverride;
				if (po == null)
				{
					throw new DesignerException("Overriden property is not found. [{0}]", Property);
				}
				if (MessageBox.Show(ep.FindForm(), "Do you want to remove the override of this property?", "Property", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					root.DeleteProperty(po);
				}
			}
		}
		#endregion
		#region Methods
		public override bool OnDeserialize(ClassPointer root, ILimnorDesigner designer)
		{
			Dictionary<string, PropertyClass> props = root.CustomProperties;
			foreach (PropertyClass p in props.Values)
			{
				if (PropertyId == p.MemberId)
				{
					Property = p;
					Init(designer, root);
					return true;
				}
			}
			return false;
		}
		public override bool IsActionExecuter(IAction act, ClassPointer root)
		{
			SetterPointer sp = act.ActionMethod as SetterPointer;
			if (sp != null)
			{
				CustomPropertyPointer cpp = sp.SetProperty as CustomPropertyPointer;
				if (cpp != null)
				{
					return (cpp.MemberId == this.PropertyId);
				}
			}
			return false;
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			//
			MenuItemWithBitmap mi;

			mi = new MenuItemWithBitmap("Create Set-Property action", mi_createAction, Resources._newPropAction.ToBitmap());
			mnu.MenuItems.Add(mi);
			//
			mnu.MenuItems.Add("-");
			//
			if (_property.IsOverride)
			{
				mi = new MenuItemWithBitmap("Remove Override", mnu_remove, Resources._cancel.ToBitmap());
			}
			else
			{
				mi = new MenuItemWithBitmap("Delete property", mi_delete, Resources._cancel.ToBitmap());
			}
			mnu.MenuItems.Add(mi);
		}
		public override bool IsForThePointer(IObjectPointer pointer)
		{
			PropertyClass ic = pointer as PropertyClass;
			if (ic != null)
			{
				return (ic.MemberId == this.PropertyId);
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
											}
											else
											{
												tid.LoadActionInstance(root);
												IAction a = tid.Action;
												if (a != null)
												{
													SetterPointer cmp = a.ActionMethod as SetterPointer;
													if (cmp != null)
													{
														CustomPropertyPointer cpp = cmp.SetProperty as CustomPropertyPointer;
														if (cpp != null)
														{
															if (cpp.MemberId == this.PropertyId)
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
							if (!tid.IsEmbedded)
							{
								IAction a = tid.LoadActionInstance(root);
								if (a == null)
								{
									MathNode.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture,
										"Action [{0}] not found", tid));
								}
								else
								{
									SetterPointer cmp = a.ActionMethod as SetterPointer;
									if (cmp != null)
									{
										CustomPropertyPointer cpp = cmp.SetProperty as CustomPropertyPointer;
										if (cpp != null && cpp.MemberId == this.PropertyId)
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
			if (_property == null && Designer != null)
			{
				ClassPointer root = Designer.GetRootId();
				_property = root.GetCustomPropertyById(PropertyId);
			}
			if (_property != null)
			{
				if (_property.RunAt == EnumWebRunAt.Client)
				{
					SetIconImage(Resources._custPropClient.ToBitmap());
				}
				else if (_property.RunAt == EnumWebRunAt.Server)
				{
					SetIconImage(Resources._custPropServer.ToBitmap());
				}
				else
				{
					SetIconImage(Resources._custProp.ToBitmap());
				}
			}
			else
			{
				SetIconImage(Resources._custProp.ToBitmap());
			}
		}
		protected override void OnSelectByMouseDown()
		{
			DesignerPane.PaneHolder.OnPropertySelected(Property);
			DesignerPane.Loader.NotifySelection(Property);
		}
		const string XMLATTR_propertyId = "propertyId";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATTR_propertyId, PropertyId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			PropertyId = XmlUtil.GetAttributeUInt(node, XMLATTR_propertyId);
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
		public UInt32 PropertyId
		{
			get;
			set;
		}
		public PropertyClass Property
		{
			get
			{
				return _property;
			}
			set
			{
				_property = value;
				PropertyId = _property.MemberId;
				SetLabelText(DisplayName);
				OnSetImage();
			}
		}
		public virtual string DisplayName
		{
			get
			{
				if (_property == null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"Property:{0}", PropertyId);
				}
				return _property.Name;
			}
		}
		#endregion
	}
}
