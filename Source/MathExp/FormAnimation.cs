using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
    public partial class FormAnimation : Form
    {
        public FormAnimation()
        {
            InitializeComponent();
        }
        public DialogResult Start(Control owner, Form dlg, Image bmp, Rectangle rc, Point destPoint)
        {
            pictureBox1.Image = bmp;
            this.Location = new Point(rc.X, rc.Y);
            this.Size = new Size(1, 1);
            this.Opacity = 100;
            Show();
            Application.DoEvents();
            this.DoubleBuffered = true;
            int n = 10;
            int dx = (destPoint.X - rc.X) / n;
            int dy = (destPoint.Y - rc.Y) / n;
            int dw = (dlg.Width - rc.Width) / n;
            int dh = (dlg.Height - rc.Height) / n;
            uint flag = Win32Util.SWP_ASYNCWINDOWPOS | Win32Util.SWP_DEFERERASE | Win32Util.SWP_NOREDRAW;
            for (int i = 1,x=rc.X,y=rc.Y,w=rc.Width,h=rc.Height; i <= n; i++,x+=dx,y+=dy,w+=dw,h+=dh)
            {
                Win32Util.SetWindowPos(this.Handle, Win32Util.HWND_TOPMOST, x, y, w, h, flag);
                this.Invalidate();
                this.Refresh();
                Application.DoEvents();
                System.Threading.Thread.Sleep(20);
            }
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Location = destPoint;
            this.Close();
            return dlg.ShowDialog(owner);
        }
    }
}