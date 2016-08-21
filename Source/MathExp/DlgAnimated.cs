/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using WindowsUtility;

namespace MathExp
{
    /// <summary>
    /// animated dialogue box.
    /// DO NOT call ShowDialog because it will cause memory leak. 
    /// Must call ShowDialogAnimated.
    /// </summary>
    public class DlgAnimated:Form
    {
        private Rectangle _rcStart;
        private Rectangle _rcEnd;
        private IntPtr _closeBitmap = IntPtr.Zero;
        private bool _correctCall;
        private bool _disableAnimate;
        public DlgAnimated()
        {
            //_correctCall = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rcStart">starting and ending rectangle in screen coordinates</param>
        public DlgAnimated(Rectangle rcStart)
            : base()
        {
            StartPosition = FormStartPosition.Manual;
            _rcStart = rcStart;
        }
        public void DisableAnimate()
        {
            _disableAnimate = true;
        }
        protected virtual void OnLoaded()
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!_correctCall && !_disableAnimate)
            {
                MessageBox.Show("ShowDialogAnimated should have been used in place of ShowDialog");
            }
            else
            {
                Point pc = this.Location;
                if (pc.X == 0 && pc.Y == 0)
                {
                    pc = new Point((Screen.PrimaryScreen.Bounds.Width - this.Width) / 2, (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2);
                    if (pc.X < 0)
                        pc.X = 0;
                    if (pc.Y < 0)
                        pc.Y = 0;
                    this.Location = pc;
                }
                //IntPtr hBitmap;
                //try
                //{
                //    hBitmap = WinUtil.CaptureScreen(IntPtr.Zero, _rcStart.X, _rcStart.Y, _rcStart.Width, _rcStart.Height);
                //}
                //catch
                //{
                //    hBitmap = IntPtr.Zero;
                //}
                //if (hBitmap != IntPtr.Zero)
                //{
                //    WinUtil.DrawBmp(hBitmap, _rcStart.Width, _rcStart.Height, _rcStart.X, _rcStart.Y, _rcStart.Width, _rcStart.Height, pc.X, pc.Y, this.Width, this.Height, 5, 40, true);
                //}
            }
            OnLoaded();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
            {
                _rcEnd = this.Bounds;
                try
                {
                    if (_closeBitmap != IntPtr.Zero)
                    {
                        WinUtil.DeleteObject(_closeBitmap);
                    }
                    _closeBitmap = WinUtil.CaptureWindow(this.Handle);
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// this is the function to be called instead of calling ShowDialog
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public DialogResult ShowDialogAnimated(IWin32Window caller)
        {
            _correctCall = true;
            DialogResult ret = base.ShowDialog(caller);
            if (_closeBitmap != IntPtr.Zero)
            {
                WinUtil.DrawBmp(_closeBitmap, _rcEnd.Width, _rcEnd.Height, _rcEnd.X, _rcEnd.Y, _rcEnd.Width, _rcEnd.Height, _rcStart.X, _rcStart.Y, _rcStart.Width, _rcStart.Height, 10, 20, true);
            }
            return ret;
        }
    }
    public class AnimatedFormLoader : Form
    {
        DlgAnimated _dialog;
        //Rectangle _rc;
        Timer _tm;
        bool _load;
        public AnimatedFormLoader(DlgAnimated dlgToLoad)//, Rectangle startRect)
        {
            _dialog = dlgToLoad;
            //_rc = startRect;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1, 1);
            _tm = new Timer();
            _tm.Interval = 100;
            _tm.Enabled = false;
            _tm.Tick += new EventHandler(_tm_Tick);
        }

        void _tm_Tick(object sender, EventArgs e)
        {
            _load = true;
            _tm.Enabled = false;
            this.DialogResult = _dialog.ShowDialogAnimated(this);
            Close();
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!_load)
            {
                _load = true;
                _tm.Enabled = true;
            }
        }
    }
}
