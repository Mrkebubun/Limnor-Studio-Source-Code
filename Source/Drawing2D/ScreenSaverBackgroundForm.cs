/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	public enum EnumScreenSaverStartType { Unknown, Start, Config, Preview, Password }
	public class ScreenSaverBackgroundForm : DrawingPage
	{
		private ScreenSaverBackgroundForm[] _allScreens;
		private Point _mousePointer = Point.Empty;
		public ScreenSaverBackgroundForm()
		{
			KeyPreview = true;

		}
		[Browsable(false)]
		public void SetAllScreens(ScreenSaverBackgroundForm[] scs)
		{
			_allScreens = scs;
		}
		private static EnumScreenSaverStartType getcmd(string s)
		{
			if (string.CompareOrdinal(s, "/a") == 0)
			{
				return EnumScreenSaverStartType.Password;
			}
			if (string.CompareOrdinal(s, "/c") == 0)
			{
				return EnumScreenSaverStartType.Config;
			}
			if (string.CompareOrdinal(s, "/p") == 0)
			{
				return EnumScreenSaverStartType.Preview;
			}
			return EnumScreenSaverStartType.Start;
		}
		[Browsable(false)]
		public static EnumScreenSaverStartType ParseScreensaverCommandLine(string[] args, out int parentHandle)
		{
			EnumScreenSaverStartType cmd = EnumScreenSaverStartType.Unknown;
			string curArg, s = "void";
			char[] SpacesOrColons = { ' ', ':' };
			parentHandle = 0;
			switch (args.Length)
			{
				case 0: // Nothing on command line, so just start the screensaver.
				case 1:
					cmd = EnumScreenSaverStartType.Start;
					parentHandle = 0;
					break;
				case 2:
					curArg = args[1];
					s = curArg.Substring(0, 2);
					curArg = curArg.Replace(s, ""); // Drop the slash /? part.
					curArg = curArg.Trim(SpacesOrColons); // Remove colons and spaces.
					try
					{
						parentHandle = curArg.Length == 0 ? 0 : int.Parse(curArg); // if empty return zero. else get handle.
					}
					catch
					{
					}
					cmd = getcmd(s);
					break;
				case 3:
					s = args[1].Substring(0, 2);
					try
					{
						parentHandle = int.Parse(args[2].ToString());
					}
					catch
					{
					}
					cmd = getcmd(s);
					break;
				default:
					parentHandle = 0;
					cmd = EnumScreenSaverStartType.Start;
					break;

			}
			return cmd;
		}

		[Browsable(false)]
		public void ShutDown()
		{
			ContinuousDrawingStarted = false;
			Close();
		}
		public void ShutDownAll()
		{
			Cursor.Show();
			for (int i = 0; i < _allScreens.Length; i++)
			{
				_allScreens[i].ShutDown();
			}
		}
		protected override void OnSizeChanged(EventArgs e)
		{
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
		}
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);
			ShutDownAll();
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			onMouseEvent(e);
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			onMouseEvent(e);
		}
		private void onMouseEvent(System.Windows.Forms.MouseEventArgs e)
		{
			if (!_mousePointer.IsEmpty) // Do nothing if mouse location has not been initialized.
			{
				if ((_mousePointer != new Point(e.X, e.Y)) || (e.Clicks > 0))
				{
					ShutDownAll();
					return;
				}
			}
			_mousePointer = new Point(e.X, e.Y); // Get current position of mouse.
		}
	}
}
