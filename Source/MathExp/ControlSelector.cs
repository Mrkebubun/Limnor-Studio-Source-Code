using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MathExp
{
    class ControlSelector
    {
        private Box lblLT;
        private Box lblCT;
        private Box lblRT;
        private Box lblLC;
        private Box lblLB;
        private Box lblCB;
        private Box lblRB;
        private Box lblRC;
        //
        protected Pen penBox;
        Pen penLine;
        //selected control
        Control _owner;
        bool bLoaded;
        public ControlSelector(Control owner)
        {
            penBox = Pens.Black;
            penLine = new Pen(Color.Black,1);
            penLine.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            _owner = owner;
            lblLT = new BoxLT(this);
            lblCT = new BoxCT(this);
            lblRT = new BoxRT(this);
            lblLC = new BoxLC(this);
            lblLB = new BoxLB(this);
            lblCB = new BoxCB(this);
            lblRB = new BoxRB(this);
            lblRC = new BoxRC(this);
            _owner.Parent.Controls.AddRange(new Control[] {lblLT, lblCT, lblRT, lblLC, lblLB, lblCB, lblRB, lblRC });
            _owner.Parent.Paint += new PaintEventHandler(Parent_Paint);
            _owner.MouseDown += new MouseEventHandler(_owner_MouseDown);
            _owner.MouseMove += new MouseEventHandler(_owner_MouseMove);
        }
        public void OnBoxMove(Box box)
        {
            if (box == lblLT)
            {
                lblCT.Top = box.Top;
                lblRT.Top = box.Top;
                lblLC.Left = box.Left;
                lblLB.Left = box.Left;
                Point p = new Point(lblLT.Left + lblLT.Width, lblLT.Top + lblLT.Height);
                Size s = new Size(lblRB.Left - p.X, lblRB.Top - p.Y);
                _owner.Location = p;
                _owner.Size = s;
                _owner.Parent.Refresh();
            }
            else if (box == lblCT)
            {
            }
            else if (box == lblRT)
            {
            }
            else if (box == lblLC)
            {
            }
            else if (box == lblLB)
            {
            }
            else if (box == lblCB)
            {
            }
            else if (box == lblRB)
            {
            }
            else if (box == lblRC)
            {
            }
        }
        //static FormDebug fDebug;
        void _owner_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && bLoaded)
            {
                //Point p = _owner.PointToClient(_owner.Parent.PointToScreen(new Point(e.X, e.Y)));
                int x = _owner.Left + e.X - x0;
                if (x > 0 && x < _owner.Parent.ClientSize.Width)
                {
                    //_owner.Left = x;
                }
                int y = _owner.Top + e.Y - y0;
                if (y > 0 && y < _owner.Parent.ClientSize.Height)
                {
                    //_owner.Top = y;
                }
                //if (fDebug == null)
                //{
                //    fDebug = new FormDebug();
                //    fDebug.Show();
                //}
                //fDebug.SetStr2(x.ToString() + ", " + y.ToString());
                _owner.Parent.Refresh();
            }
        }
        int x0 = 0;
        int y0 = 0;
        void _owner_MouseDown(object sender, MouseEventArgs e)
        {
            //Point p = _owner.PointToClient(_owner.Parent.PointToScreen(new Point(e.X, e.Y)));
            x0 = e.X;
            y0 = e.Y;
            //if (fDebug == null)
            //{
            //    fDebug = new FormDebug();
            //    fDebug.Show();
            //}
            //fDebug.SetStr1((_owner.Left+x0).ToString() + ", " + (_owner.Top + y0).ToString());
        }
        public Control OwnerControl
        {
            get
            {
                return _owner;
            }
        }
        public Pen PenForBox
        {
            get
            {
                return penBox;
            }
        }
        public void Clear()
        {
            _owner.Parent.Controls.Remove(lblLT);
            _owner.Parent.Controls.Remove(lblCT);
            _owner.Parent.Controls.Remove(lblRT);
            _owner.Parent.Controls.Remove(lblLC);
            _owner.Parent.Controls.Remove(lblLB);
            _owner.Parent.Controls.Remove(lblCB);
            _owner.Parent.Controls.Remove(lblRB);
            _owner.Parent.Controls.Remove(lblRC);
            _owner.Parent.Paint -= new PaintEventHandler(Parent_Paint);
        }
        public void OnAdjustPos()
        {
            _hide = false;
            lblLT.OnAdjustPos();
            lblCT.OnAdjustPos();
            lblRT.OnAdjustPos();
            lblLC.OnAdjustPos();
            lblLB.OnAdjustPos();
            lblCB.OnAdjustPos();
            lblRB.OnAdjustPos();
            lblRC.OnAdjustPos();
        }
        bool _hide;
        public void Hide()
        {
            lblLT.Hide();
            lblCT.Hide();
            lblRT.Hide();
            lblLC.Hide();
            lblLB.Hide();
            lblCB.Hide();
            lblRB.Hide();
            lblRC.Hide();
            _hide = true;
        }
        public void Loaded()
        {
            bLoaded = true;
        }
        void Parent_Paint(object sender, PaintEventArgs e)
        {
            if (!_hide)
            {
                e.Graphics.DrawRectangle(penLine, new Rectangle(_owner.Left - 2, _owner.Top - 2, _owner.Width + 4, _owner.Height + 4));
            }
        }
        public class Box : Label
        {
            protected ControlSelector selector;
            protected Control _owner;
            int boxSize = 6;
            public Box(ControlSelector c)
            {
                selector = c;
                _owner = c.OwnerControl;
                this.Width = boxSize;
                this.Height = boxSize;
                this.BackColor = Color.White;
                _owner.Move += new EventHandler(c_Move);
            }

            void c_Move(object sender, EventArgs e)
            {
                OnAdjustPos();
            }
            public virtual void OnAdjustPos()
            {
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.DrawRectangle(selector.PenForBox, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
            int x0 = 0;
            int y0 = 0;
            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                x0 = e.X;
                y0 = e.Y;
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (e.Button == MouseButtons.Left)
                {
                    if (!this.Capture)
                    {
                        this.FindForm().Cursor = this.Cursor;
                        selector.Hide();
                        this.Visible = true;
                        this.Capture = true;
                    }
                    bool b = false;
                    int x = this.Left + e.X - x0;
                    if (x > 0 && x < this.Parent.ClientSize.Width)
                    {
                        this.Left = x;
                        b = true;
                    }
                    int y = this.Top + e.Y - y0;
                    if (y > 0 && y < this.Parent.ClientSize.Height)
                    {
                        this.Top = y;
                        b = true;
                    }
                    if (b)
                    {
                        selector.OnBoxMove(this);
                    }
                }
            }
        }
        class BoxLT : Box
        {
            public BoxLT(ControlSelector c)
                : base(c)
            {
                this.Cursor = Cursors.SizeNWSE ;
            }
            public override void OnAdjustPos()
            {
                this.Location = new System.Drawing.Point(_owner.Left - this.Width, _owner.Top - this.Height);
                this.Visible = true;
            }
        }
        class BoxRT : Box
        {
            public BoxRT(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                this.Location = new System.Drawing.Point(_owner.Left + _owner.Width, _owner.Top - this.Height);
                this.Visible = true;
            }
        }
        class BoxLB : Box
        {
            public BoxLB(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                this.Location = new System.Drawing.Point(_owner.Left - this.Width, _owner.Top + _owner.Height);
                this.Visible = true;
            }
        }
        class BoxRB : Box
        {
            public BoxRB(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                this.Location = new System.Drawing.Point(_owner.Left + _owner.Width, _owner.Top + _owner.Height);
                this.Visible = true;
            }
        }
        class BoxCT : Box
        {
            public BoxCT(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                if (_owner.Width > this.Width)
                {
                    this.Location = new System.Drawing.Point(_owner.Left + (_owner.Width-this.Width) / 2, _owner.Top - this.Height);
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
        }
        class BoxLC : Box
        {
            public BoxLC(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                if (_owner.Height > this.Height)
                {
                    this.Location = new System.Drawing.Point(_owner.Left - this.Width, _owner.Top + (_owner.Height - this.Height)/2);
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
        }
        class BoxRC : Box
        {
            public BoxRC(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                if (_owner.Height > this.Height)
                {
                    this.Location = new System.Drawing.Point(_owner.Left + _owner.Width, _owner.Top + (_owner.Height - this.Height) / 2);
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
        }
        class BoxCB : Box
        {
            public BoxCB(ControlSelector c)
                : base(c)
            {

            }
            public override void OnAdjustPos()
            {
                if (_owner.Width > this.Width)
                {
                    this.Location = new System.Drawing.Point(_owner.Left + (_owner.Width - this.Width) / 2, _owner.Top + _owner.Height);
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
        }
    }
}
