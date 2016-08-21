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
using ProgElements;
using LimnorDesigner.Property;
using System.Windows.Forms;
using MathExp;
using System.Reflection;
using LimnorDesigner.Event;
using LimnorDesigner.MethodBuilder;
using VPL;
using WindowsUtility;

namespace LimnorDesigner.Interface
{
	public abstract class TreeNodeImplement : TreeNodeObject
	{
		public TreeNodeImplement(ClassPointer root)
			: base(false, root)
		{
			Nodes.Add(new CLoad());
		}
		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			l.Add(new CLoad());
			return l;
		}
		public ClassPointer Root
		{
			get
			{
				return (ClassPointer)(this.OwnerPointer);
			}
		}
		public abstract void OnLoadNextLevel();
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Instance; } }
		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Both; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return false;
		}
		class CLoad : TreeNodeLoader
		{
			public CLoad()
				: base(false)
			{
			}

			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{
				TreeNodeImplement tni = parentNode as TreeNodeImplement;
				tni.OnLoadNextLevel();
			}
		}
	}
	public class TreeNodeOverridesInterfaces : TreeNodeImplement
	{
		public TreeNodeOverridesInterfaces(ClassPointer root)
			: base(root)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_Interfaces;
			SelectedImageIndex = ImageIndex;
			Text = "Overrides and Interfaces";
		}

		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Interface; }
		}
		public void ResetEventNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeOverrideProperties tnop = Nodes[i] as TreeNodeOverrideProperties;
				if (tnop != null)
				{
					tnop.ResetNextLevel(tv);
				}
				TreeNodeInterfaces tnif = Nodes[i] as TreeNodeInterfaces;
				if (tnif != null)
				{
					tnif.ResetEventNodes();
				}
			}
		}
		public void ResetPropertyNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeOverrideProperties tnop = Nodes[i] as TreeNodeOverrideProperties;
				if (tnop != null)
				{
					tnop.ResetNextLevel(tv);
				}
				TreeNodeInterfaces tnif = Nodes[i] as TreeNodeInterfaces;
				if (tnif != null)
				{
					tnif.ResetPropertyNodes();
				}
			}
		}
		public void ResetMethodNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeOverrideMethods tnop = Nodes[i] as TreeNodeOverrideMethods;
				if (tnop != null)
				{
					tnop.ResetNextLevel(tv);
				}
				TreeNodeInterfaces tnif = Nodes[i] as TreeNodeInterfaces;
				if (tnif != null)
				{
					tnif.ResetMethodNodes();
				}
			}
		}
		public override void OnLoadNextLevel()
		{
			Nodes.Add(new TreeNodeOverrideProperties(Root));
			Nodes.Add(new TreeNodeOverrideMethods(Root));
			Nodes.Add(new TreeNodeInterfaces(Root));
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			IObjectPointer o = ownerStack.Peek();
			PropertyClassInherited p = o as PropertyClassInherited;
			if (p != null)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeOverrideProperties pns = Nodes[i] as TreeNodeOverrideProperties;
					if (pns != null)
					{
						return pns.LocateObjectNode(ownerStack);
					}
				}
			}
			else
			{
				MethodClassInherited m = o as MethodClassInherited;
				if (m != null)
				{
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeOverrideMethods mns = Nodes[i] as TreeNodeOverrideMethods;
						if (mns != null)
						{
							return mns.LocateObjectNode(ownerStack);
						}
					}
				}
				else
				{
					InterfacePointer ip = o as InterfacePointer;
					if (ip != null)
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeInterfaces ins = Nodes[i] as TreeNodeInterfaces;
							if (ins != null)
							{
								return ins.LocateObjectNode(ownerStack);
							}
						}
					}
				}
			}
			return null;
		}
		public void OnActionAssigned(EventAction ea)
		{
			ResetEventNodes();
		}
		public void OnActionChanged(UInt32 classId, IAction act, bool isNewAction)
		{
			SetterPointer sp = act.ActionMethod as SetterPointer;
			if (sp != null)
			{
				ResetPropertyNodes();
			}
			else
			{
				ResetMethodNodes();
			}
		}
		public TreeNodeOverrideProperties GetPropertiesNode()
		{
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				TreeNodeOverrideProperties tnps = Nodes[i] as TreeNodeOverrideProperties;
				if (tnps != null)
				{
					return tnps;
				}
			}
			return null;
		}
		public TreeNodeOverrideMethods GetMethodsNode()
		{
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				TreeNodeOverrideMethods tnps = Nodes[i] as TreeNodeOverrideMethods;
				if (tnps != null)
				{
					return tnps;
				}
			}
			return null;
		}
		public TreeNodeInterfaces GetInterfacesNode()
		{
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				TreeNodeInterfaces tnps = Nodes[i] as TreeNodeInterfaces;
				if (tnps != null)
				{
					return tnps;
				}
			}
			return null;
		}
		public void OnPropertyImplemented(PropertyOverride p)
		{
			TreeNodeOverrideProperties tnps = GetPropertiesNode();
			if (tnps != null)
			{
				tnps.OnPropertyImplemented(p);
			}
		}
		public void OnPropertyRemoved(PropertyOverride p)
		{
			TreeNodeOverrideProperties tnps = GetPropertiesNode();
			if (tnps != null)
			{
				tnps.OnPropertyRemoved(p);
			}
		}
		public void OnMethodImplemented(MethodOverride m)
		{
			TreeNodeOverrideMethods tnps = GetMethodsNode();
			if (tnps != null)
			{
				tnps.OnMethodImplemented(m);
			}
		}
		public void OnMethodRemoved(MethodOverride m)
		{
			TreeNodeOverrideMethods tnps = GetMethodsNode();
			if (tnps != null)
			{
				tnps.OnMethodRemoved(m);
			}
		}
	}
	public class TreeNodeOverrideProperties : TreeNodeImplement
	{
		public TreeNodeOverrideProperties(ClassPointer root)
			: base(root)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_PROPERTIES;
			SelectedImageIndex = ImageIndex;
			Text = "Properties";
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Property; }
		}
		public void OnPropertyRemoved(PropertyOverride p)
		{
			OnPropertyImplemented(p);
		}
		public void OnPropertyImplemented(PropertyOverride p)
		{
			if (this.NextLevelLoaded)
			{
				TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
				this.ResetNextLevel(tv);
				this.LoadNextLevel();
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					TreeNodeOverrideProperty tn = Nodes[i] as TreeNodeOverrideProperty;
					if (tn != null)
					{
						if (p.Name == tn.Property.Name)
						{
							this.TreeView.SelectedNode = tn;
							break;
						}
					}
				}
			}
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded)
			{
				if (ownerStack.Count > 0)
				{
					IObjectPointer o = ownerStack.Pop();
					PropertyClassInherited p = o as PropertyClassInherited;
					if (p == null)
					{
						DesignUtil.WriteToOutputWindow("LocateObjectNode:{0} is not a PropertyClassInherited", o.GetType());
					}
					else
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeOverrideProperty pn = Nodes[i] as TreeNodeOverrideProperty;
							if (pn != null)
							{
								if (p.IsSameObjectRef(pn.OwnerPointer))
								{
									if (ownerStack.Count == 0)
									{
										return pn;
									}
									else
									{
										return pn.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			UInt32 scopeId = ScopeMethodId;
			ClassPointer root = this.Root;
			Dictionary<UInt32, IAction> actions = root.GetActions();
			List<PropertyClassInherited> props = root.GetPropertyOverrides();
			foreach (PropertyClassInherited p in props)
			{
				TreeNodeOverrideProperty tn = new TreeNodeOverrideProperty(tv, p, Root, scopeId);
				Nodes.Add(tn);
				if (actions != null)
				{
					foreach (IAction a in actions.Values)
					{
						if (a != null)
						{
							if (tn.IncludeAction(a, tv, scopeId, false))
							{
								tn.OnShowActionIcon();
								break;
							}
						}
					}
				}
			}
		}
	}
	public class TreeNodeOverrideProperty : TreeNodeImplement
	{
		private PropertyClassInherited _property;
		public TreeNodeOverrideProperty(TreeViewObjectExplorer tv, PropertyClassInherited property, ClassPointer root, UInt32 scopeMethodId)
			: base(root)
		{
			_property = property;
			ImageIndex = TreeViewObjectExplorer.GetPropertyImageIndex(property);
			SelectedImageIndex = ImageIndex;
			Text = _property.ToString();
			//
			if (this.SelectionTarget == EnumObjectSelectType.All)
			{
				Nodes.Add(new TreeNodeAttributeCollection(tv, this, (IAttributeHolder)this.OwnerPointer, scopeMethodId));
			}
			this.Nodes.Add(new TreeNodeMemberLoader(false));
			this.Nodes.Add(new ActionLoader(false));
		}
		public override IObjectIdentity MemberOwner
		{
			get
			{
				return _property;
			}
		}
		public PropertyClassInherited Property
		{
			get
			{
				return _property;
			}
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Property; }
		}
		public override void OnNodeSelection(ILimnorDesignerLoader loader)
		{
			loader.NotifySelection(_property);
		}
		public override void OnShowActionIcon()
		{
			base.OnShowActionIcon();
			ImageIndex = TreeViewObjectExplorer.GetActPropertyImageIndex(_property);
			SelectedImageIndex = this.ImageIndex;
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded)
			{
				if (ownerStack.Count > 0)
				{
					IObjectPointer o = ownerStack.Peek();
					ConstObjectPointer co = o as ConstObjectPointer;
					if (co != null)
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeAttributeCollection ac = Nodes[i] as TreeNodeAttributeCollection;
							if (ac != null)
							{
								return ac.LocateObjectNode(ownerStack);
							}
						}
					}
					else
					{
						return TreeNodeMemberLoader.LocateObjectNode(ownerStack, this);
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
		}
		public override IAction CreateNewAction()
		{
			ActionClass act = new ActionClass(this.Root);
			act.ActionMethod = _property.CreateSetterMethodPointer(act);
			act.ActionName = act.ActionMethod.DefaultActionName;
			act.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
			return act;
		}
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!readOnly)
			{
				MenuItem[] menus = new MenuItem[3];
				menus[0] = new MenuItemWithBitmap("Create SetProperty Action", OnCreateNewAction, Resources._propAction.ToBitmap());
				menus[1] = new MenuItem("-");
				if (_property.Implemented)
				{
					menus[2] = new MenuItemWithBitmap("Remove Override", mnu_remove, Resources._cancel.ToBitmap());
					if (!_property.HasBaseImplementation)
					{
						menus[2].Enabled = false;
					}
				}
				else
				{
					menus[2] = new MenuItemWithBitmap("Override", mnu_implement, Resources._overrideProperty.ToBitmap());
				}
				return menus;
			}
			return null;
		}
		private void mnu_implement(object sender, EventArgs e)
		{
			ClassPointer r = Root;
			r.CreateOverrideProperty(_property);
		}
		private void mnu_remove(object sender, EventArgs e)
		{
			PropertyOverride po = _property as PropertyOverride;
			if (po == null)
			{
				throw new DesignerException("Overriden property is not found. [{0}]", _property);
			}
			if (MessageBox.Show("Do you want to remove the override of this property?", "Property", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				ClassPointer r = Root;
				r.DeleteProperty(po);
			}
		}
	}
	public class TreeNodeOverrideMethods : TreeNodeImplement
	{
		public TreeNodeOverrideMethods(ClassPointer root)
			: base(root)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_METHODS;
			SelectedImageIndex = ImageIndex;
			Text = "Methods";
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Method; }
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded)
			{
				if (ownerStack.Count > 0)
				{
					IObjectPointer o = ownerStack.Pop();
					MethodClassInherited p = o as MethodClassInherited;
					if (p == null)
					{
						DesignUtil.WriteToOutputWindow("LocateObjectNode:{0} is not a MethodClassInherited", o.GetType());
					}
					else
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeOverrideMethod pn = Nodes[i] as TreeNodeOverrideMethod;
							if (pn != null)
							{
								if (p.IsSameObjectRef(pn.OwnerPointer))
								{
									if (ownerStack.Count == 0)
									{
										return pn;
									}
									else
									{
										return pn.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			UInt32 scopeId = ScopeMethodId;
			ClassPointer root = this.Root;
			Dictionary<UInt32, IAction> actions = root.GetActions();
			List<MethodClassInherited> props = root.GetMethodOverrides();
			foreach (MethodClassInherited p in props)
			{
				TreeNodeOverrideMethod tn = new TreeNodeOverrideMethod(tv, p, Root, scopeId);
				Nodes.Add(tn);
				if (actions != null)
				{
					foreach (IAction a in actions.Values)
					{
						if (a != null)
						{
							if (tn.IncludeAction(a, tv, scopeId, false))
							{
								tn.OnShowActionIcon();
								break;
							}
						}
					}
				}
			}
		}
		public void OnMethodRemoved(MethodOverride m)
		{
			OnMethodImplemented(m);
		}
		public void OnMethodImplemented(MethodOverride m)
		{
			if (this.NextLevelLoaded)
			{
				TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
				this.ResetNextLevel(tv);
				this.LoadNextLevel();
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					TreeNodeOverrideMethod tn = Nodes[i] as TreeNodeOverrideMethod;
					if (tn != null)
					{
						if (m.Name == tn.Method.Name)
						{
							this.TreeView.SelectedNode = tn;
							break;
						}
					}
				}
			}
		}
	}
	public class TreeNodeOverrideMethod : TreeNodeImplement
	{
		private MethodClassInherited _method;
		public TreeNodeOverrideMethod(TreeViewObjectExplorer tv, MethodClassInherited method, ClassPointer root, UInt32 scopeMethodId)
			: base(root)
		{
			_method = method;
			ImageIndex = TreeViewObjectExplorer.GetMethodImageIndex(method);
			SelectedImageIndex = ImageIndex;
			Text = method.DisplayName;
			//
			if (this.SelectionTarget == EnumObjectSelectType.All)
			{
				if (method.Implemented)
				{
					Nodes.Add(new TreeNodeAttributeCollection(tv, this, (IAttributeHolder)this.OwnerPointer, scopeMethodId));
				}
			}
			this.Nodes.Add(new ActionLoader(false));
		}
		public override IObjectIdentity MemberOwner
		{
			get
			{
				return _method;
			}
		}
		public MethodClassInherited Method
		{
			get
			{
				return _method;
			}
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Method; }
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded)
			{
				if (ownerStack.Count > 0)
				{
					IObjectPointer o = ownerStack.Pop();
					ParameterClass p = o as ParameterClass;
					if (p != null)
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeCustomMethodParameter tmp = Nodes[i] as TreeNodeCustomMethodParameter;
							if (tmp != null)
							{
								if (p.IsSameObjectRef(tmp.OwnerPointer))
								{
									if (ownerStack.Count == 0)
									{
										return tmp;
									}
									else
									{
										return tmp.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			if (_method.ParameterCount > 0)
			{
				List<ParameterClass> parameters = _method.Parameters;
				foreach (ParameterClass p in parameters)
				{
					TreeNodeCustomMethodParameter tmp = new TreeNodeCustomMethodParameter(false, p);
					Nodes.Add(tmp);
				}
			}
		}
		public override void OnNodeSelection(ILimnorDesignerLoader loader)
		{
			loader.NotifySelection(_method);
		}
		public override void OnShowActionIcon()
		{
			base.OnShowActionIcon();
			ImageIndex = TreeViewObjectExplorer.GetActMethodImageIndex(_method);
			SelectedImageIndex = this.ImageIndex;
		}
		public override IAction CreateNewAction()
		{
			ActionClass act = new ActionClass(this.Root);
			act.ActionMethod = _method.CreatePointer(this.Root);
			act.ActionName = act.ActionMethod.DefaultActionName;
			act.ActionId = (UInt32)(Guid.NewGuid().GetHashCode());
			return act;
		}
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!readOnly)
			{
				MenuItem[] menus = new MenuItem[3];
				menus[0] = new MenuItemWithBitmap("Create Action", OnCreateNewAction, Resources._methodAction.ToBitmap());
				menus[1] = new MenuItem("-");
				if (_method.Implemented)
				{
					menus[2] = new MenuItemWithBitmap("Remove Override", mnu_remove, Resources._cancel.ToBitmap());
					if (!_method.HasBaseImplementation)
					{
						menus[2].Enabled = false;
					}
				}
				else
				{
					menus[2] = new MenuItemWithBitmap("Override", mnu_implement, Resources._overrideMethod.ToBitmap());
				}
				return menus;
			}
			return null;
		}
		private void mnu_implement(object sender, EventArgs e)
		{
			ClassPointer r = Root;
			r.ImplementOverrideMethod(_method);

		}
		private void mnu_remove(object sender, EventArgs e)
		{
			MethodOverride po = _method as MethodOverride;
			if (po == null)
			{
				throw new DesignerException("Overriden method is not found. [{0}]", _method);
			}
			if (MessageBox.Show("Do you want to remove the override of this method?", "Method", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				ClassPointer r = Root;
				r.RemoveOverrideMethod(po);
			}
		}
	}
	public class TreeNodeInterfaces : TreeNodeImplement
	{
		public TreeNodeInterfaces(ClassPointer root)
			: base(root)
		{
			ImageIndex = TreeViewObjectExplorer.IMG_Interfaces;
			SelectedImageIndex = ImageIndex;
			Text = "Interfaces";
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Interface; }
		}
		public void ResetMethodNodes()
		{
			if (NextLevelLoaded)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeInterface tnif = Nodes[i] as TreeNodeInterface;
					if (tnif != null)
					{
						tnif.ResetMethodNodes();
					}
				}
			}
		}
		public void ResetPropertyNodes()
		{
			if (NextLevelLoaded)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeInterface tnif = Nodes[i] as TreeNodeInterface;
					if (tnif != null)
					{
						tnif.ResetPropertyNodes();
					}
				}
			}
		}
		public void ResetEventNodes()
		{
			if (NextLevelLoaded)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					TreeNodeInterface tnif = Nodes[i] as TreeNodeInterface;
					if (tnif != null)
					{
						tnif.ResetEventNodes();
					}
				}
			}
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded)
			{
				if (ownerStack.Count > 0)
				{
					IObjectPointer o = ownerStack.Pop();
					InterfacePointer p = o as InterfacePointer;
					if (p == null)
					{
						DesignUtil.WriteToOutputWindow("LocateObjectNode:{0} is not a InterfacePointer", o.GetType());
					}
					else
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeInterface pn = Nodes[i] as TreeNodeInterface;
							if (pn != null)
							{
								if (p.IsSameObjectRef(pn.OwnerPointer))
								{
									if (ownerStack.Count == 0)
									{
										return pn;
									}
									else
									{
										return pn.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			ClassPointer root = this.Root;
			List<InterfacePointer> interfaces = root.GetInterfaces();
			foreach (InterfacePointer ip in interfaces)
			{
				this.Nodes.Add(new TreeNodeInterface(ip, root));
			}
		}
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!readOnly)
			{
				MenuItem[] menus = new MenuItem[1];
				menus[0] = new MenuItemWithBitmap("Add interface", mi_addInterface, Resources._interfaceIcon.ToBitmap());
				return menus;
			}
			return null;
		}
		private void mi_addInterface(object sender, EventArgs e)
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			if (tv != null)
			{
				ClassPointer cp = Root;
				DataTypePointer dp = DesignUtil.SelectDataType(cp.Project, tv.RootId, null, EnumObjectSelectType.Interface, null, null, null, tv.FindForm());
				if (dp != null)
				{
					InterfacePointer ip;
					ClassPointer cp2 = dp.VariableCustomType;
					if (cp2 != null)
					{
						cp2.EnsureLoaded();
						ip = new InterfacePointer(cp2);
					}
					else
					{
						ip = new InterfacePointer(new TypePointer(dp.VariableLibType));
					}
					if (cp.AddInterface(ip))
					{
						//locate the new interface
						Expand();
						for (int i = 0; i < this.Nodes.Count; i++)
						{
							TreeNodeInterface tni = Nodes[i] as TreeNodeInterface;
							if (tni != null)
							{
								if (ip.IsSameObjectRef(tni.Interface))
								{
									tv.SelectedNode = tni;
									tni.EnsureVisible();
									break;
								}
							}
						}
					}
				}
			}
		}
	}
	public class TreeNodeInterface : TreeNodeImplement
	{
		private InterfacePointer _interfacePointer;
		public TreeNodeInterface(InterfacePointer pointer, ClassPointer root)
			: base(root)
		{
			_interfacePointer = pointer;
			Text = pointer.ToString();
			if (pointer.IsAdded)
			{
				ImageIndex = TreeViewObjectExplorer.IMG_c_Interface;
			}
			else
			{
				ImageIndex = TreeViewObjectExplorer.IMG_Interface2;
			}
			SelectedImageIndex = ImageIndex;

		}
		public override void OnLoadNextLevel()
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			if (this.SelectionTarget == EnumObjectSelectType.All)
			{

				Nodes.Add(new TreeNodeAttributeCollection(tv, this, (IAttributeHolder)this.OwnerPointer, 0));
			}
			this.Nodes.Add(new TreeNodeInterfaceProperties(Interface, Root));
			this.Nodes.Add(new TreeNodeInterfaceMethods(Interface, Root));
			this.Nodes.Add(new TreeNodeInterfaceEvents(Interface, Root));
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded)
			{
				if (ownerStack.Count > 0)
				{
					IObjectPointer o = ownerStack.Peek();
					InterfaceElementProperty p = o as InterfaceElementProperty;
					if (p != null)
					{
						for (int i = 0; i < Nodes.Count; i++)
						{
							TreeNodeInterfaceProperties pn1 = Nodes[i] as TreeNodeInterfaceProperties;
							if (pn1 != null)
							{
								return pn1.LocateObjectNode(ownerStack);
							}
						}
					}
					else
					{
						InterfaceElementMethod m = o as InterfaceElementMethod;
						if (m != null)
						{
							for (int i = 0; i < Nodes.Count; i++)
							{
								TreeNodeInterfaceMethods pn2 = Nodes[i] as TreeNodeInterfaceMethods;
								if (pn2 != null)
								{
									return pn2.LocateObjectNode(ownerStack);
								}
							}
						}
						else
						{
							InterfaceElementEvent e = o as InterfaceElementEvent;
							if (e != null)
							{
								for (int i = 0; i < Nodes.Count; i++)
								{
									TreeNodeInterfaceEvents pn3 = Nodes[i] as TreeNodeInterfaceEvents;
									if (pn3 != null)
									{
										return pn3.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public void ResetMethodNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeInterfaceMethods tnif = Nodes[i] as TreeNodeInterfaceMethods;
				if (tnif != null)
				{
					tnif.ResetNextLevel(tv);
				}
			}
		}
		public void ResetPropertyNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeInterfaceProperties tnif = Nodes[i] as TreeNodeInterfaceProperties;
				if (tnif != null)
				{
					tnif.ResetNextLevel(tv);
				}
			}
		}
		public void ResetEventNodes()
		{
			TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
			for (int i = 0; i < Nodes.Count; i++)
			{
				TreeNodeInterfaceEvents tnif = Nodes[i] as TreeNodeInterfaceEvents;
				if (tnif != null)
				{
					tnif.ResetNextLevel(tv);
				}
			}
		}
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!readOnly)
			{
				if (_interfacePointer.ImplementerClassId == Root.ClassId)
				{
					MenuItem[] menus = new MenuItem[1];
					menus[0] = new MenuItemWithBitmap("Remove", mi_remove, Resources._cancel.ToBitmap());
					return menus;
				}
			}
			return null;
		}
		private void mi_remove(object sender, EventArgs e)
		{
			IRootNode r = this.RootClassNode;
			if (r != null && r.ViewersHolder != null)
			{
				if (MessageBox.Show(this.TreeView.FindForm(), "Do you want to remove the interface?", "Interface", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					ClassPointer root = this.Root;
					if (root.RemoveInterface(_interfacePointer, r.ViewersHolder.Loader))
					{
						this.Remove();
					}
				}
			}
		}
		public InterfacePointer Interface
		{
			get
			{
				return _interfacePointer;
			}
		}

		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Interface; }
		}
	}
	public class TreeNodeInterfaceMethods : TreeNodeImplement
	{
		private InterfacePointer _interface;
		public TreeNodeInterfaceMethods(InterfacePointer pointer, ClassPointer root)
			: base(root)
		{
			_interface = pointer;
			Text = "Methods";
			ImageIndex = TreeViewObjectExplorer.IMG_METHODS;
			SelectedImageIndex = ImageIndex;
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Interface; }
		}
		public InterfacePointer Interface
		{
			get
			{
				return _interface;
			}
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded && ownerStack.Count > 0)
			{
				IObjectPointer o = ownerStack.Pop();
				InterfaceElementMethod m = o as InterfaceElementMethod;
				if (m != null)
				{
					string key = m.GetMethodSignature();
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeCustomMethod cm = Nodes[i] as TreeNodeCustomMethod;
						if (cm != null)
						{
							if (string.CompareOrdinal(key, cm.Method.GetMethodSignature()) == 0)
							{
								if (ownerStack.Count == 0)
								{
									return cm;
								}
								else
								{
									return cm.LocateObjectNode(ownerStack);
								}
							}
						}
						else
						{
							TreeNodeMethod md = Nodes[i] as TreeNodeMethod;
							if (md != null)
							{
								int op;
								if (string.CompareOrdinal(key, md.MethodRef.GetMethodSignature(out op)) == 0)
								{
									if (ownerStack.Count == 0)
									{
										return md;
									}
									else
									{
										return md.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			List<InterfaceElementMethod> methods = Interface.Methods;
			if (methods != null)
			{
				TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
				UInt32 scopeId = ScopeMethodId;
				Dictionary<string, MethodClass> customMethods = Root.CustomMethods;
				Dictionary<string, MethodInfoPointer> libMethods = Root.GetLibMethods();
				foreach (InterfaceElementMethod m in methods)
				{
					MethodClass custMethod;
					string key = m.GetMethodSignature();
					Dictionary<UInt32, IAction> actions = Root.GetActions();
					if (customMethods.TryGetValue(key, out custMethod))
					{
						TreeNodeCustomMethod nodeMethod = new TreeNodeCustomMethod(tv, false, custMethod, Root, scopeId);
						nodeMethod.DoNotRemove = true;
						this.Nodes.Add(nodeMethod);
						bool bHasActions = false;
						foreach (IAction a in actions.Values)
						{
							if (a != null && a.IsStatic == IsStatic)
							{
								if (nodeMethod.IncludeAction(a, tv, scopeId, false))
								{
									bHasActions = true;
									break;
								}
							}
						}
						if (bHasActions)
						{
							nodeMethod.OnShowActionIcon();
						}
					}
					else
					{
						MethodInfoPointer mip = null;
						if (!libMethods.TryGetValue(key, out mip))
						{
							MethodInfo mif = VPLUtil.GetMethod(_interface.BaseClassType, m.Name, m.ParameterTypeArray, null);
							if (mif != null)
							{
								mip = new InterfaceMethodPointer(Root, _interface.BaseClassType, mif);
							}
						}
						if (mip != null)
						{
							TreeNodeMethod nodeMethod = new TreeNodeMethod(mip.IsStatic, mip);
							nodeMethod.DoNotRemove = true;
							this.Nodes.Add(nodeMethod);
							bool bHasActions = false;
							foreach (IAction a in actions.Values)
							{
								if (a != null && a.IsStatic == IsStatic)
								{
									if (nodeMethod.IncludeAction(a, tv, scopeId, false))
									{
										bHasActions = true;
										break;
									}
								}
							}
							if (bHasActions)
							{
								nodeMethod.OnShowActionIcon();
							}
						}
						else
						{
							if (_interface.VariableCustomType != null)
							{
								//the method is not implemented, implement it now
								custMethod = Root.implementMethod(m);
								if (custMethod != null)
								{
									TreeNodeCustomMethod nodeMethod = new TreeNodeCustomMethod(tv, false, custMethod, Root, scopeId);
									nodeMethod.DoNotRemove = true;
									this.Nodes.Add(nodeMethod);
								}
							}
							else
							{
								MathNode.Log(null, "Method [{0}] not found for interface [{1}]", key, Interface.Name);
							}
						}
					}
				}
			}
		}
	}
	public class TreeNodeInterfaceEvents : TreeNodeImplement
	{
		private InterfacePointer _interface;
		public TreeNodeInterfaceEvents(InterfacePointer pointer, ClassPointer root)
			: base(root)
		{
			_interface = pointer;
			Text = "Events";
			ImageIndex = TreeViewObjectExplorer.IMG_EVENTS;
			SelectedImageIndex = ImageIndex;
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Interface; }
		}
		public InterfacePointer Interface
		{
			get
			{
				return _interface;
			}
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded && ownerStack.Count > 0)
			{
				IObjectPointer o = ownerStack.Pop();
				InterfaceElementEvent e = o as InterfaceElementEvent;
				if (e != null)
				{
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeCustomEvent ce = Nodes[i] as TreeNodeCustomEvent;
						if (ce != null)
						{
							if (string.CompareOrdinal(e.Name, ce.Event.Name) == 0)
							{
								if (ownerStack.Count == 0)
								{
									return ce;
								}
								else
								{
									return ce.LocateObjectNode(ownerStack);
								}
							}
						}
						else
						{
							TreeNodeEvent ne = Nodes[i] as TreeNodeEvent;
							if (ne != null)
							{
								if (string.CompareOrdinal(e.Name, ne.OwnerEvent.Name) == 0)
								{
									if (ownerStack.Count == 0)
									{
										return ne;
									}
									else
									{
										return ne.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			List<InterfaceElementEvent> events = Interface.Events;
			if (events != null)
			{
				TreeViewObjectExplorer tv = TreeView as TreeViewObjectExplorer;
				UInt32 scopeId = ScopeMethodId;
				TreeNodeClassRoot tr = TopLevelRootClassNode;
				Dictionary<string, EventClass> customEvents = Root.CustomEvents;
				Dictionary<string, EventPointer> libEvents = Root.GetLibEvents();
				foreach (InterfaceElementEvent e in events)
				{
					EventClass custEvent;
					Dictionary<UInt32, IAction> actions = Root.GetActions();
					if (customEvents.TryGetValue(e.Name, out custEvent))
					{
						TreeNodeCustomEvent nodeEvent = new TreeNodeCustomEvent(custEvent.IsStatic, custEvent);
						nodeEvent.DoNotRemove = true;
						this.Nodes.Add(nodeEvent);
						bool bHasActions = false;
						foreach (IAction a in actions.Values)
						{
							if (a != null && a.IsStatic == IsStatic)
							{
								if (nodeEvent.IncludeAction(a, tv, scopeId, false))
								{
									bHasActions = true;
									break;
								}
							}
						}
						if (bHasActions)
						{
							nodeEvent.OnShowActionIcon();
						}
					}
					else
					{
						EventPointer eip = null;
						if (!libEvents.TryGetValue(e.Name, out eip))
						{
							EventInfo eif = _interface.BaseClassType.GetEvent(e.Name);
							if (eif != null)
							{
								eip = new InterfaceEventPointer(Root, _interface.BaseClassType, eif);
							}
						}
						if (eip != null)
						{
							TreeNodeEvent nodeEvent = new TreeNodeEvent(eip.IsStatic, eip);
							nodeEvent.DoNotRemove = true;
							this.Nodes.Add(nodeEvent);
							if (tr != null && tr.HasEventHandler(eip))
							{
								nodeEvent.OnShowActionIcon();
							}
						}
						else
						{
							if (_interface.VariableCustomType != null)
							{
								//the event has not been implemented. implement it now
								ILimnorDesignerLoader loader = null;
								if (tr != null)
								{
									loader = tr.DesignerLoader;
								}
								if (loader == null)
								{
									ILimnorDesignPane pane = Root.Project.GetTypedData<ILimnorDesignPane>(Root.ClassId);
									if (pane != null)
									{
										loader = pane.Loader;
									}
								}
								if (loader != null)
								{
									custEvent = Root.implementEvent(e);
									TreeNodeCustomEvent nodeEvent = new TreeNodeCustomEvent(custEvent.IsStatic, custEvent);
									nodeEvent.DoNotRemove = true;
									this.Nodes.Add(nodeEvent);
								}
							}
							else
							{
								MathNode.Log(null, "Event [{0}] not found for interface [{1}]", e.Name, Interface.Name);
							}
						}
					}
				}
			}
		}
	}
	public class TreeNodeInterfaceProperties : TreeNodeImplement
	{
		private InterfacePointer _interface;
		public TreeNodeInterfaceProperties(InterfacePointer pointer, ClassPointer root)
			: base(root)
		{
			_interface = pointer;
			Text = "Properties";
			ImageIndex = TreeViewObjectExplorer.IMG_PROPERTIES;
			SelectedImageIndex = ImageIndex;
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Interface; }
		}
		public InterfacePointer Interface
		{
			get
			{
				return _interface;
			}
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (NextLevelLoaded && ownerStack.Count > 0)
			{
				IObjectPointer o = ownerStack.Pop();
				InterfaceElementProperty p = o as InterfaceElementProperty;
				if (p != null)
				{
					for (int i = 0; i < Nodes.Count; i++)
					{
						TreeNodeCustomProperty pn = Nodes[i] as TreeNodeCustomProperty;
						if (pn != null)
						{
							if (string.CompareOrdinal(p.Name, pn.Property.Name) == 0)
							{
								if (ownerStack.Count == 0)
								{
									return pn;
								}
								else
								{
									return pn.LocateObjectNode(ownerStack);
								}
							}
						}
						else
						{
							TreeNodeProperty tp = Nodes[i] as TreeNodeProperty;
							if (tp != null)
							{
								if (string.CompareOrdinal(p.Name, tp.Property.Name) == 0)
								{
									if (ownerStack.Count == 0)
									{
										return tp;
									}
									else
									{
										return tp.LocateObjectNode(ownerStack);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public override void OnLoadNextLevel()
		{
			TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
			List<InterfaceElementProperty> properties = Interface.GetProperties();
			if (properties != null)
			{
				UInt32 scopeId = ScopeMethodId;
				Dictionary<UInt32, IAction> actions = Root.GetActions();
				foreach (InterfaceElementProperty p in properties)
				{
					object pi = Root.InterfacePropertyImplemented(p);
					if (pi == null)
					{
						//not implemented, implement it now
						PropertyClass custProp = Root.implementProperty(p);
						if (custProp != null)
						{
							TreeNodeCustomProperty nodeProperty = new TreeNodeCustomProperty(tv, false, custProp, scopeId);
							nodeProperty.DoNotRemove = true;
							this.Nodes.Add(nodeProperty);
						}
					}
					else
					{
						PropertyPointer pp = pi as PropertyPointer;
						if (pp == null)
						{
							PropertyInfo pif = pi as PropertyInfo;
							if (pif != null)
							{
								pp = new InterfacePropertyPointer(Root, _interface.BaseClassType, pif);
							}
						}
						if (pp != null)
						{
							TreeNodeProperty nodeProperty = new TreeNodeProperty(pp.IsStatic, pp);
							nodeProperty.DoNotRemove = true;
							this.Nodes.Add(nodeProperty);
							bool bHasActions = false;
							foreach (IAction a in actions.Values)
							{
								if (a != null && a.IsStatic == IsStatic)
								{
									if (nodeProperty.IncludeAction(a, tv, scopeId, false))
									{
										bHasActions = true;
										break;
									}
								}
							}
							if (bHasActions)
							{
								nodeProperty.OnShowActionIcon();
							}
						}
						else
						{
							PropertyClass custProp = pi as PropertyClass;
							if (custProp != null)
							{
								TreeNodeCustomProperty nodeProperty = new TreeNodeCustomProperty(tv, false, custProp, scopeId);
								nodeProperty.DoNotRemove = true;
								this.Nodes.Add(nodeProperty);
								bool bHasActions = false;
								foreach (IAction a in actions.Values)
								{
									if (a != null && a.IsStatic == IsStatic)
									{
										if (nodeProperty.IncludeAction(a, tv, scopeId, false))
										{
											bHasActions = true;
											break;
										}
									}
								}
								if (bHasActions)
								{
									nodeProperty.OnShowActionIcon();
								}
							}
							else
							{
								MathNode.Log(null, "Unhandled property type [{0}] for property [{1}] of type [{2}] for interface [{3}]", pi, p.Name, p.VariableLibType, Interface.Name);
							}
						}
					}
				}
			}
		}
	}
}
