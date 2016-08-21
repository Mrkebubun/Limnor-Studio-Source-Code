/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
//------------------------------------------------------------------------------
/// <copyright from='2006' to='2020' company='Longflow Enterprises Ltd'>
///    Copyright (c) Longflow Enterprises Ltd. All Rights Reserved.
///
/// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using System.ComponentModel.Design.Serialization;

namespace MathExp
{
	/// The IMenuCommandService keeps track of MenuCommands and Designer Verbs
	/// that are available at any single moment. It can invoke these commands
	/// and also handles the displaying of ContextMenus for designers that support them.
	public class MathMenuCommandService : MenuCommandService//System.ComponentModel.Design.IMenuCommandService
	{
		private Hashtable commands; // added MenuCommands
		private Hashtable verbsFromMenuItems; // DesignerVerb values mapped to MenuItem keys
		private Hashtable menuItemsFromVerbs; // MenuItem values mapped to DesignerVerb keys
		private ArrayList globalVerbs; // verbs currently available to all designers
		private ContextMenu cm;
		private IComponent lastSelection; // needed to clean up local verbs from a previous selection
		public MathMenuCommandService(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			//this.host = host;
			MenuCommand undoCommand = new MenuCommand(new EventHandler(ExecuteUndo), StandardCommands.Undo);
			undoCommand.Enabled = false;
			base.AddCommand(undoCommand);

			MenuCommand redoCommand = new MenuCommand(new EventHandler(ExecuteRedo), StandardCommands.Redo);
			redoCommand.Enabled = false;
			base.AddCommand(redoCommand);

			MenuCommand cutCommand = new MenuCommand(new EventHandler(ExecuteCut), StandardCommands.Cut);
			base.AddCommand(cutCommand);

			MenuCommand delCommand = new MenuCommand(new EventHandler(ExecuteDel), StandardCommands.Delete);
			base.AddCommand(delCommand);

			MenuCommand copyCommand = new MenuCommand(new EventHandler(ExecuteCopy), StandardCommands.Copy);
			base.AddCommand(copyCommand);

			MenuCommand pasteCommand = new MenuCommand(new EventHandler(ExecutePaste), StandardCommands.Paste);
			base.AddCommand(pasteCommand);
		}
		void enableUndoMenu()
		{
			MenuCommand undoCommand = this.FindCommand(StandardCommands.Undo);
			undoCommand.Enabled = true;
		}
		#region Menu Commands
		void ExecuteUndo(object sender, EventArgs e)
		{
			UndoEngineImpl undoEngine = GetService(typeof(UndoEngine)) as UndoEngineImpl;
			if (undoEngine != null)
				undoEngine.DoUndo();
		}
		void ExecuteRedo(object sender, EventArgs e)
		{
			UndoEngineImpl undoEngine = GetService(typeof(UndoEngine)) as UndoEngineImpl;
			if (undoEngine != null)
				undoEngine.DoRedo();
		}
		void ExecuteCut(object sender, EventArgs e)
		{
			IDesignerHost host = (IDesignerHost)(this.GetService(typeof(IDesignerHost)));
			if (host != null)
			{
				ISelectionService ss = (ISelectionService)host.GetService(typeof(ISelectionService));
				if (ss != null)
				{
					ICollection cc = ss.GetSelectedComponents();
					if (cc != null)
					{

						using (DesignerTransaction trans = host.CreateTransaction("Cut " + cc.Count + " component(s)"))
						{
							ExecuteCopy(sender, e);
							ExecuteDel(sender, e);
							trans.Commit();
							enableUndoMenu();
						}

					}
				}
			}
		}
		void ExecuteCopy(object sender, EventArgs e)
		{
			ISelectionService ss = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (ss != null)
			{
				if (ss.PrimarySelection != null)
				{
					IDesignerSerializationService dss = (IDesignerSerializationService)this.GetService(typeof(IDesignerSerializationService));
					if (dss != null)
					{
						object v = dss.Serialize(ss.GetSelectedComponents());
						System.Windows.Forms.Clipboard.SetDataObject(new DataObject("DesignObject", v), true);
					}
				}
			}
		}
		void ExecutePaste(object sender, EventArgs e)
		{
			IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if (host != null)
			{
				IDesignerSerializationService dss = (IDesignerSerializationService)this.GetService(typeof(IDesignerSerializationService));
				if (dss != null)
				{
					object v = System.Windows.Forms.Clipboard.GetData("DesignObject");
					if (v != null)
					{
						ICollection cc = dss.Deserialize(v);
						if (cc != null)
						{
							using (DesignerTransaction trans = host.CreateTransaction("Paste"))
							{
								foreach (object x in cc)
								{
									if (x is IComponent)
									{
										host.Container.Add((IComponent)x);
										if (x is Control)
										{
											if (host.RootComponent is Control)
											{
												((Control)host.RootComponent).Controls.Add((Control)x);
												((Control)x).Location = new Point(((Control)x).Left + 10, ((Control)x).Top + 10);
											}
										}
										//notify
									}
								}
								ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
								if (selSvc != null)
								{
									selSvc.SetSelectedComponents(cc, SelectionTypes.Replace);
								}
								trans.Commit();
								enableUndoMenu();
							}
						}
					}
				}
			}
		}
		void ExecuteDel(object sender, EventArgs e)
		{
			ISelectionService ss = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (ss != null)
			{
				ICollection cc = ss.GetSelectedComponents();
				if (cc != null)
				{
					IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
					if (host != null)
					{
						using (DesignerTransaction trans = host.CreateTransaction("Delete " + cc.Count + " component(s)"))
						{
							foreach (object v in cc)
							{
								if (v is System.ComponentModel.IComponent)
								{
									if (v != host.RootComponent)
									{
										host.DestroyComponent((System.ComponentModel.IComponent)v);
									}
								}
							}
							trans.Commit();
							enableUndoMenu();
						}
					}
				}
			}
		}
		#endregion
		/// The IMenuCommandService deals with two kinds of verbs: 1) local verbs specific
		/// to each designer (i.e. Add/Remove Tab on a TabControl) which are added
		/// and removed on-demand, each time a designer is right-clicked, 2) the rarer
		/// global verbs, which once added are available to all designers,
		/// until removed. This method (not a standard part of IMenuCommandService) is used
		/// to add a local verb. If the verb is already in our global list, we don't add it 
		/// again. It is called through IMenuCommandService.ShowContextMenu().
		public void AddLocalVerb(DesignerVerb verb)
		{
			if ((globalVerbs == null) || (!globalVerbs.Contains(verb)))
			{
				if (cm == null)
				{
					cm = new ContextMenu();
					verbsFromMenuItems = new Hashtable();
					menuItemsFromVerbs = new Hashtable();
				}

				// Verbs and MenuItems are dually mapped to each other, so that we can
				// check for association given either half of the pair. All of our MenuItems
				// use the same event handler, but we can check the event sender to see
				// what verb to invoke. MenuItems like to only be assigned to one Menu in their
				// lifetime, so we have to create a single ContextMenu and use that thereafter.
				// If we were to instead create a ContextMenu every time we need to show one,
				// the MenuItems' click events might not work properly.
				//
				MenuItem menuItem = new MenuItem(verb.Text);
				menuItem.Click += new EventHandler(menuItem_Click);
				verbsFromMenuItems.Add(menuItem, verb);
				menuItemsFromVerbs.Add(verb, menuItem);
				cm.MenuItems.Add(menuItem);
			}
		}

		/// Remove a local verb, but only if it isn't in our global list.
		/// It is also called through IMenuCommandService.ShowContextMenu().
		public void RemoveLocalVerb(DesignerVerb verb)
		{
			if ((globalVerbs == null) || (!globalVerbs.Contains(verb)))
			{
				if (cm != null)
				{
					// Remove the verb and its mapped MenuItem from our tables and menu.
					MenuItem key = menuItemsFromVerbs[verb] as MenuItem;
					verbsFromMenuItems.Remove(key);
					menuItemsFromVerbs.Remove(verb);
					cm.MenuItems.Remove(key);
				}
			}
		}

		#region Implementation of IMenuCommandService

		/// If a designer supports a MenuCommand, it will add it here.
		public override void AddCommand(System.ComponentModel.Design.MenuCommand command)
		{
			if (commands == null)
			{
				commands = new Hashtable();
			}

			// Only add a command if we haven't already.
			if (FindCommand(command.CommandID) == null)
			{
				commands.Add(command.CommandID, command);
			}
		}

		/// This is only called by external callers if someone wants to
		/// remove a verb that is available for all designers. We keep track of
		/// such verbs to make sure that they are not re-added or removed
		/// when we frequently manipulate our local verbs.
		public override void RemoveVerb(System.ComponentModel.Design.DesignerVerb verb)
		{
			if (globalVerbs != null)
			{
				globalVerbs.Remove(verb);

				if (cm != null)
				{
					// Remove the verb and its mapped MenuItem from our tables and menu.
					MenuItem key = menuItemsFromVerbs[verb] as MenuItem;
					verbsFromMenuItems.Remove(key);
					menuItemsFromVerbs.Remove(verb);
					cm.MenuItems.Remove(key);
				}
			}
		}

		/// If a command is no longer viable for a designer, it is removed.
		public override void RemoveCommand(System.ComponentModel.Design.MenuCommand command)
		{
			if (commands != null)
			{
				// Hashtable already has nothing happen if the command isn't there.
				commands.Remove(command.CommandID);
			}
		}

		/// We only invoke commands that have been added.
		public override bool GlobalInvoke(System.ComponentModel.Design.CommandID commandID)
		{
			MenuCommand command = FindCommand(commandID);
			if (command != null)
			{
				command.Invoke();
				return true;
			}
			return false;
		}

		/// This is called whenever the user right-clicks on a designer. It removes any local verbs
		/// added by a previous, different selection and adds the local verbs for the current (primary)
		/// selection. Then it displays the ContextMenu.
		public override void ShowContextMenu(System.ComponentModel.Design.CommandID menuID, int x, int y)
		{
			ISelectionService ss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			//
			// If this is the same component as was last right-clicked on, then we don't need to
			// make any changes to our collection of local verbs.
			//
			if ((lastSelection != null) && (lastSelection != ss.PrimarySelection))
			{
				if (host != null)
				{
					IDesigner d = host.GetDesigner(lastSelection);
					if (d != null)
					{
						foreach (DesignerVerb verb in d.Verbs)
						{
							RemoveLocalVerb(verb);
						}
					}
				}
			}

			// Update the local verbs for the new selection, if it is indeed new.
			if (lastSelection != ss.PrimarySelection)
			{
				if (host != null)
				{
					IDesigner d = host.GetDesigner(ss.PrimarySelection as IComponent);
					if (d != null)
					{
						foreach (DesignerVerb verb in d.Verbs)
						{
							AddLocalVerb(verb);
						}
					}
				}
			}
			if (host != null)
			{
				if (host.RootComponent != ss.PrimarySelection)
				{
					// Display our ContextMenu! Note that the coordinate parameters to this method
					// are in screen coordinates, so we've got to translate them into client coordinates.
					//
					if (cm != null)
					{
						Control ps = ss.PrimarySelection as Control;
						Point s = ps.PointToScreen(new Point(0, 0));
						cm.Show(ps, new Point(x - s.X, y - s.Y));
					}
				}
			}
			// ss.PrimarySelection might be old news by the next right-click. We need to 
			// be able to remove its verbs if we get a different selection next time
			// this method is called. We can't remove them right now because we aren't sure
			// if the MenuItem click events have finished yet (and removing verbs kills
			// their MenuItem mapping). So we save the selection and do it next time if necessary.
			//
			lastSelection = ss.PrimarySelection as IComponent;
		}

		/// This is only called by external callers if someone wants to
		/// remove a verb that is available for all designers. We keep track of
		/// such verbs to make sure that they are not re-added or removed
		/// when we frequently manipulate our local verbs.
		public override void AddVerb(System.ComponentModel.Design.DesignerVerb verb)
		{
			if (globalVerbs == null)
			{
				globalVerbs = new ArrayList();
			}

			globalVerbs.Add(verb);

			if (cm == null)
			{
				cm = new ContextMenu();
				verbsFromMenuItems = new Hashtable();
				menuItemsFromVerbs = new Hashtable();
				//commandFromMenuItems = new Hashtable();
			}

			// Verbs and MenuItems are dually mapped to each other, so that we can
			// check for association given either half of the pair. All of our MenuItems
			// use the same event handler, but we can check the event sender to see
			// what verb to invoke. We have to have a stable ContextMenu, and we need
			// to add the MenuItems to it now. If we try to add the MenuItems on-demand
			// right before we show a ContextMenu, the clicks events won't work.
			//
			MenuItem menuItem = new MenuItem(verb.Text);
			menuItem.Click += new EventHandler(menuItem_Click);
			verbsFromMenuItems.Add(menuItem, verb);
			menuItemsFromVerbs.Add(verb, menuItem);
			cm.MenuItems.Add(menuItem);
		}

		/// This is frequently called to get the current list of available verbs. We have
		/// to be careful in what verbs we return, however, because the information contained
		/// in menuItemsFromVerbs and verbsFromMenuItems will not be current if there has been
		/// no right-click since the last selection change. Thus we return the global verbs
		/// plus any additional local verbs for the current selection.
		public override System.ComponentModel.Design.DesignerVerbCollection Verbs
		{
			get
			{
				ArrayList currentVerbs;
				if (globalVerbs != null)
				{
					currentVerbs = new ArrayList(globalVerbs);
				}
				else
				{
					currentVerbs = new ArrayList();
				}
				IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				if (host != null)
				{
					ISelectionService ss = this.GetService(typeof(ISelectionService)) as ISelectionService;
					if (ss != null && ss.PrimarySelection is IComponent)
					{
						IDesigner dr = host.GetDesigner(ss.PrimarySelection as IComponent);
						if (dr != null && dr.Verbs != null)
						{
							foreach (DesignerVerb verb in dr.Verbs)
							{
								if (!currentVerbs.Contains(verb))
								{
									currentVerbs.Add(verb);
								}
							}
						}
					}
				}
				if (currentVerbs.Count > 0)
				{
					DesignerVerb[] ret = new DesignerVerb[currentVerbs.Count];
					currentVerbs.CopyTo(ret);
					return new DesignerVerbCollection(ret);
				}
				else
				{
					return new DesignerVerbCollection();
				}
			}
		}

		#endregion
		/// All of our MenuItems' click events are handled by this method. It checks the
		/// mapping to see which verb is associated with the MenuItem that sent the event
		/// and then invokes that verb.
		private void menuItem_Click(object sender, EventArgs e)
		{
			MenuItem key = sender as MenuItem;
			DesignerVerb v = verbsFromMenuItems[key] as DesignerVerb;
			this.GlobalInvoke(v.CommandID);
		}
	}
}
