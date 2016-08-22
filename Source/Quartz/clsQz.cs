/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Play audio and video using Media Control Interface
 * License: GNU General Public License v3.0
 
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Limnor.Quartz
{
	class clsQZRun
	{
		private bool bCancel = false;
		private MciMediaPlayer qz = null;
		System.Threading.Thread th = null;
		System.Threading.ThreadStart onPlay;
		public clsQZRun(MciMediaPlayer q)
		{
			qz = q;
			onPlay = new System.Threading.ThreadStart(startPlay);
			th = null;
		}
		public void Play()
		{
			bCancel = false;
			if (th != null)
			{
				try
				{
					th.Abort();
				}
				catch
				{
				}
				th = null;
			}
			th = new System.Threading.Thread(onPlay);
			th.Priority = System.Threading.ThreadPriority.Lowest;
			th.Start();
		}
		public void Stop()
		{
			bCancel = true;
			if (th != null)
			{
				try
				{
					th.Abort();
				}
				catch
				{
				}
				th = null;
			}
		}
		public void Cancel()
		{
			bCancel = true;
		}
		private void startPlay()
		{
			System.Windows.Forms.MethodInvoker merr = new System.Windows.Forms.MethodInvoker(qz._OnError);
			try
			{
				if (qz.Loaded)
				{
					System.Windows.Forms.MethodInvoker mi = null;
					if (!qz.Loop)
						mi = new System.Windows.Forms.MethodInvoker(qz._OnFinish);
					qz._Run();
					while (!bCancel)
					{
						if (qz.Finished)
						{
							if (qz.Loop)
							{
								qz.ToBeginPos();
								qz._Run();
							}
							else
							{
								if (!bCancel)
									qz.BeginInvoke(mi);
								break;
							}
						}
						Application.DoEvents();
					}
				}
			}
			catch (System.Runtime.InteropServices.ExternalException ei)
			{
				if (ei.ErrorCode != -2147220953)
				{
					if (!bCancel)
					{
						qz.sErrMsg = ei.Message;
						qz.BeginInvoke(merr);
					}
				}
			}
			catch (Exception er)
			{
				if (!bCancel)
				{
					qz.sErrMsg = er.Message;
					qz.BeginInvoke(merr);
				}
			}
		}
	}
	class clsQZReturn
	{
		public int nCode = 0;
		public clsQZReturn()
		{
		}
	}

}
