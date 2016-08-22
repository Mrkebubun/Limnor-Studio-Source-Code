using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;

namespace XHost
{
    public class MenuService2 : MenuCommandService
    {
        public MenuService2(IDesignerHost host)
            : base(host)
		{
            //this.host = host;
            //AddVerb(new DesignerVerb("Delete",new EventHandler(null_Click),StandardCommands.Delete));
            //AddVerb(new DesignerVerb("Undo", new EventHandler(null_Click), StandardCommands.Undo));
            //AddCommand(new MenuCommand(new EventHandler(null_Click), MenuCommands.Undo));
		}
        private void menuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            this.GlobalInvoke((CommandID)menuItem.Tag);
        }
        public override void ShowContextMenu(System.ComponentModel.Design.CommandID menuID, int x, int y)
        {
            
            //if (this.Verbs != null)
            {
                ContextMenu cm = new ContextMenu();
                //foreach (DesignerVerb verb in this.Verbs)
                //{
                MenuItem menuItem = new MenuItem("Delete");
                menuItem.Click += new EventHandler(menuItem_Click);
                menuItem.Tag = StandardCommands.Delete;
                cm.MenuItems.Add(menuItem);
                //}
                //
                menuItem = new MenuItem("Undo");
                menuItem.Click += new EventHandler(menuItem_Click);
                menuItem.Tag = StandardCommands.Undo;
                cm.MenuItems.Add(menuItem);
                //
                if (cm.MenuItems.Count > 0)
                {
                    IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                    ISelectionService ss = host.GetService(typeof(ISelectionService)) as ISelectionService;
                    Control ps = ss.PrimarySelection as Control;
                    Point s = ps.PointToScreen(new Point(0, 0));
                    cm.Show(ps, new Point(x - s.X, y - s.Y));
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (components != null)
                //    components.Dispose();
                while (true)
                {
                    if (this.Verbs == null)
                        break;
                    if (this.Verbs.Count == 0)
                        break;
                    this.RemoveVerb(this.Verbs[0]);
                }
            }
            base.Dispose(disposing);
        }
    }
}
