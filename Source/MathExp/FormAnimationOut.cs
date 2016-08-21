using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
    public partial class FormAnimationOut : Form
    {
        Bitmap _bmp;
        Rectangle _rcBmp;
        public FormAnimationOut(Bitmap bmp)
            : base()
        {
            InitializeComponent();
            _bmp = bmp;
            _rcBmp = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_bmp, new Rectangle(0,0,this.ClientSize.Width,this.ClientSize.Height), _rcBmp, GraphicsUnit.Pixel);
        }
        public void Animation(Rectangle rcDest)
        {
            int nCount = 5;
            Rectangle rcStart = this.Bounds;
            int dw = (rcDest.Width - rcStart.Width) / nCount;
            int dh = (rcDest.Height - rcStart.Height) / nCount;
            int dx = (rcDest.X - rcStart.X) / nCount;
            int dy = (rcDest.Y - rcStart.Y) / nCount;
            //int dx2 = dx/2;
            //int dy2 = dy/2;
            //int delay = 500;
            for (int i = 0; i < nCount; i++)
            {
                //System.Threading.Thread.Sleep(delay);
                rcStart.X += dx;
                rcStart.Y += dy;
                rcStart.Width += dw;
                rcStart.Height += dh;
                this.Bounds = rcStart;
                this.Invalidate();
                Application.DoEvents();
                //delay /= 5;
            }
            this.Bounds = rcDest;
            Application.DoEvents();
            //System.Threading.Thread.Sleep(3000);
            Close();
        }
    }
}