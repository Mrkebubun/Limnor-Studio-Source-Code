/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
///
namespace VOB
{
	using System;
	using System.Windows.Forms;
	using System.ComponentModel.Design;
	using System.Windows.Forms.Design;
	using System.Drawing;

	public delegate bool GetPropertyGridFocus();

	/// This filter is used to catch keyboard input that is meant for the designer.
	/// It does not prevent the message from continuing, but instead merely
	/// deciphers the keystroke and performs the appropriate MenuCommand.
	public class KeystrokeMessageFilter : System.Windows.Forms.IMessageFilter
	{
		private DesignSurfaceManager surfaceMan;
		public static bool CanDeleteComponent = true;
		public GetPropertyGridFocus GetMainPropertyGridFocus;
		public KeystrokeMessageFilter(DesignSurfaceManager manager)
		{
			surfaceMan = manager;
		}
		#region Implementation of IMessageFilter

		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYDOWN = 0x0100;
			const int WM_KEYUP = 0x0101;

			const int KEY_DEL = 46;

			if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
			{
				if ((int)m.WParam == KEY_DEL)
				{
					if (GetMainPropertyGridFocus != null)
					{
						bool fo = GetMainPropertyGridFocus();
						if (fo)
						{
							OutputWindow.AppendMessage2("In property grid");
							CanDeleteComponent = false;
						}
						else
						{
							OutputWindow.AppendMessage2("Not in pg");
							CanDeleteComponent = true;
						}
					}
				}
			}
			// not filter the message
			return false;
		}

		#endregion
	}
}
