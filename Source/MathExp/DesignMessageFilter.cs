/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace MathExp
{
	public class DesignMessageFilter : IMessageFilter
	{
		DesignSurface surface;
		public DesignMessageFilter(DesignSurface designSurface)
		{
			surface = designSurface;
		}
		#region IMessageFilter Members
		public static UInt16 LOWORD(UInt32 l)
		{
			return ((UInt16)((UInt64)(l) & 0xffff));
		}
		public static UInt16 HIWORD(UInt32 l)
		{
			return ((UInt16)((UInt64)(l) >> 16));
		}
		public static Int32 GET_X_LPARAM(IntPtr lp)
		{
			return ((Int32)(Int16)LOWORD((UInt32)(lp.ToInt32())));
		}
		public static Int32 GET_Y_LPARAM(IntPtr lp)
		{
			return ((Int32)(Int16)HIWORD((UInt32)(lp.ToInt32())));
		}
		public bool PreFilterMessage(ref Message m)
		{
			/*
			 * #define WM_MOUSEFIRST                   0x0200



#define GET_X_LPARAM(lp)                        ((int)(short)LOWORD(lp))
#define GET_Y_LPARAM(lp)                        ((int)(short)HIWORD(lp))
#define MAKEWORD(a, b)      ((WORD)(((BYTE)((DWORD_PTR)(a) & 0xff)) | ((WORD)((BYTE)((DWORD_PTR)(b) & 0xff))) << 8))
#define MAKELONG(a, b)      ((LONG)(((WORD)((DWORD_PTR)(a) & 0xffff)) | ((DWORD)((WORD)((DWORD_PTR)(b) & 0xffff))) << 16))
#define LOWORD(l)           ((WORD)((DWORD_PTR)(l) & 0xffff))
#define HIWORD(l)           ((WORD)((DWORD_PTR)(l) >> 16))
#define LOBYTE(w)           ((BYTE)((DWORD_PTR)(w) & 0xff))
#define HIBYTE(w)           ((BYTE)((DWORD_PTR)(w) >> 8))

			 /*
 * Key State Masks for Mouse Messages
 */
			const int MK_LBUTTON = 0x0001;
			const int MK_RBUTTON = 0x0002;
			const int MK_SHIFT = 0x0004;
			const int MK_CONTROL = 0x0008;
			const int MK_MBUTTON = 0x0010;
			const int MK_XBUTTON1 = 0x0020;
			const int MK_XBUTTON2 = 0x0040;

			const int WM_MOUSEMOVE = 0x0200;
			const int WM_LBUTTONDOWN = 0x0201;
			const int WM_LBUTTONUP = 0x0202;
			const int WM_LBUTTONDBLCLK = 0x0203;
			const int WM_RBUTTONDOWN = 0x0204;
			const int WM_RBUTTONUP = 0x0205;
			const int WM_RBUTTONDBLCLK = 0x0206;
			const int WM_MBUTTONDOWN = 0x0207;
			const int WM_MBUTTONUP = 0x0208;
			const int WM_MBUTTONDBLCLK = 0x0209;

			const int WM_KEYDOWN = 0x100;
			const int WM_KEYUP = 0x101;

			Control c = surface.View as Control;
			if (c != null && ((m.Msg >= WM_MOUSEMOVE && m.Msg <= WM_MBUTTONDBLCLK) || (m.Msg >= WM_KEYDOWN && m.Msg <= WM_KEYUP)))
			{
				IMessageReceiver linkNode = null;
				Control dv = null;
				if (c.Controls.Count > 0 && c.Controls[0].Controls.Count > 0)
				{
					dv = c.Controls[0].Controls[0];
					//if (dv != null)
					{
						if (m.HWnd == dv.Handle)
							linkNode = dv as IMessageReceiver;
						else
						{
							for (int i = 0; i < dv.Controls.Count; i++)
							{
								if (m.HWnd == dv.Controls[i].Handle)
								{
									linkNode = dv.Controls[i] as IMessageReceiver;
									break;
								}
							}
						}
					}
				}
				if (linkNode == null && (m.Msg >= WM_KEYDOWN && m.Msg <= WM_KEYUP))
				{
					if (m.HWnd == c.Handle)
					{
						linkNode = dv as IMessageReceiver;
					}
				}
				if (linkNode != null)
				{
					if (m.Msg == WM_KEYDOWN)
					{
						KeyEventArgs ek = new KeyEventArgs((Keys)m.WParam.ToInt32());
						return linkNode.FireKeyDown(ek);
					}
					else if (m.Msg == WM_KEYUP)
					{
						KeyEventArgs ek = new KeyEventArgs((Keys)m.WParam.ToInt32());
						return linkNode.FireKeyUp(ek);
					}
					else
					{
						int w = m.WParam.ToInt32();
						Keys modifiers = Keys.None;
						if ((w & MK_SHIFT) != 0)
						{
							modifiers |= Keys.Shift;
						}
						if ((w & MK_CONTROL) != 0)
						{
							modifiers |= Keys.Control;
						}
						if (m.Msg == WM_LBUTTONDOWN)
						{
							return linkNode.FireMouseDown(MouseButtons.Left, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_MOUSEMOVE)
						{
							MouseButtons bts = MouseButtons.None;
							if ((w & MK_LBUTTON) != 0)
							{
								bts |= MouseButtons.Left;
							}
							if ((w & MK_RBUTTON) != 0)
							{
								bts |= MouseButtons.Right;
							}
							if ((w & MK_MBUTTON) != 0)
							{
								bts |= MouseButtons.Middle;
							}
							if ((w & MK_XBUTTON1) != 0)
							{
								bts |= MouseButtons.XButton1;
							}
							if ((w & MK_XBUTTON2) != 0)
							{
								bts |= MouseButtons.XButton2;
							}
							return linkNode.FireMouseMove(bts, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_LBUTTONUP)
						{
							return linkNode.FireMouseUp(MouseButtons.Left, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_RBUTTONDOWN)
						{
							return linkNode.FireMouseDown(MouseButtons.Right, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_RBUTTONUP)
						{
							return linkNode.FireMouseUp(MouseButtons.Right, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_MBUTTONDOWN)
						{
							return linkNode.FireMouseDown(MouseButtons.Middle, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_MBUTTONUP)
						{
							return linkNode.FireMouseUp(MouseButtons.Middle, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_LBUTTONDBLCLK)
						{
							return linkNode.FireMouseDblClick(MouseButtons.Left, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_RBUTTONDBLCLK)
						{
							return linkNode.FireMouseDblClick(MouseButtons.Right, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
						else if (m.Msg == WM_MBUTTONDBLCLK)
						{
							return linkNode.FireMouseDblClick(MouseButtons.Middle, GET_X_LPARAM(m.LParam), GET_Y_LPARAM(m.LParam), modifiers);
						}
					}
				}
			}
			return false;
		}

		#endregion
	}
}
