/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsUtility
{
	struct MOUSEINPUT
	{
		public int dx;
		public int dy;
		public uint mouseData;
		public uint dwFlags;
		public uint time;
		public IntPtr dwExtraInfo;
	}

	struct KEYBDINPUT
	{
		public ushort wVk;
		public ushort wScan;
		public uint dwFlags;
		public uint time;
		public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Explicit)]
	struct MOUSEKEYBDHARDWAREINPUT
	{
		[FieldOffset(0)]
		public MOUSEINPUT mi;

		[FieldOffset(0)]
		public KEYBDINPUT ki;
	}

	struct INPUT
	{
		public int type;
		public MOUSEKEYBDHARDWAREINPUT mkhi;
	}

	public enum VK : ushort
	{
		VK_VOID = 0,
		//
		// Virtual Keys, Standard Set
		//
		VK_LBUTTON = 0x01,
		VK_RBUTTON = 0x02,
		VK_CANCEL = 0x03,
		VK_MBUTTON = 0x04,    // NOT contiguous with L & RBUTTON

		VK_XBUTTON1 = 0x05,    // NOT contiguous with L & RBUTTON
		VK_XBUTTON2 = 0x06,    // NOT contiguous with L & RBUTTON

		// 0x07 : unassigned

		VK_BACK = 0x08,
		VK_TAB = 0x09,

		// 0x0A - 0x0B : reserved

		VK_CLEAR = 0x0C,
		VK_RETURN = 0x0D,

		VK_SHIFT = 0x10,
		VK_CONTROL = 0x11,
		VK_MENU = 0x12,
		VK_PAUSE = 0x13,
		VK_CAPITAL = 0x14,

		VK_KANA = 0x15,
		VK_HANGEUL = 0x15,  // old name - should be here for compatibility
		VK_HANGUL = 0x15,
		VK_JUNJA = 0x17,
		VK_FINAL = 0x18,
		VK_HANJA = 0x19,
		VK_KANJI = 0x19,

		VK_ESCAPE = 0x1B,

		VK_CONVERT = 0x1C,
		VK_NONCONVERT = 0x1D,
		VK_ACCEPT = 0x1E,
		VK_MODECHANGE = 0x1F,

		VK_SPACE = 0x20,
		VK_PRIOR = 0x21,
		VK_NEXT = 0x22,
		VK_END = 0x23,
		VK_HOME = 0x24,
		VK_LEFT = 0x25,
		VK_UP = 0x26,
		VK_RIGHT = 0x27,
		VK_DOWN = 0x28,
		VK_SELECT = 0x29,
		VK_PRINT = 0x2A,
		VK_EXECUTE = 0x2B,
		VK_SNAPSHOT = 0x2C,
		VK_INSERT = 0x2D,
		VK_DELETE = 0x2E,
		VK_HELP = 0x2F,

		//
		// VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
		VK_0 = 0x30,
		VK_1 = 0x31,
		VK_2 = 0x32,
		VK_3 = 0x33,
		VK_4 = 0x34,
		VK_5 = 0x35,
		VK_6 = 0x36,
		VK_7 = 0x37,
		VK_8 = 0x38,
		VK_9 = 0x39,
		// 0x40 : unassigned
		// VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
		VK_A = 0x41,
		VK_B = 0x42,
		VK_C = 0x43,
		VK_D = 0x44,
		VK_E = 0x45,
		VK_F = 0x46,
		VK_G = 0x47,
		VK_H = 0x48,
		VK_I = 0x49,
		VK_J = 0x4A,
		VK_K = 0x4B,
		VK_L = 0x4C,
		VK_M = 0x4D,
		VK_N = 0x4E,
		VK_O = 0x4F,
		VK_P = 0x50,
		VK_Q = 0x51,
		VK_R = 0x52,
		VK_S = 0x53,
		VK_T = 0x54,
		VK_U = 0x55,
		VK_V = 0x56,
		VK_W = 0x57,
		VK_X = 0x58,
		VK_Y = 0x59,
		VK_Z = 0x5A,
		//

		VK_LWIN = 0x5B,
		VK_RWIN = 0x5C,
		VK_APPS = 0x5D,

		//
		// 0x5E : reserved
		//

		VK_SLEEP = 0x5F,

		VK_NUMPAD0 = 0x60,
		VK_NUMPAD1 = 0x61,
		VK_NUMPAD2 = 0x62,
		VK_NUMPAD3 = 0x63,
		VK_NUMPAD4 = 0x64,
		VK_NUMPAD5 = 0x65,
		VK_NUMPAD6 = 0x66,
		VK_NUMPAD7 = 0x67,
		VK_NUMPAD8 = 0x68,
		VK_NUMPAD9 = 0x69,
		VK_MULTIPLY = 0x6A,
		VK_ADD = 0x6B,
		VK_SEPARATOR = 0x6C,
		VK_SUBTRACT = 0x6D,
		VK_DECIMAL = 0x6E,
		VK_DIVIDE = 0x6F,
		VK_F1 = 0x70,
		VK_F2 = 0x71,
		VK_F3 = 0x72,
		VK_F4 = 0x73,
		VK_F5 = 0x74,
		VK_F6 = 0x75,
		VK_F7 = 0x76,
		VK_F8 = 0x77,
		VK_F9 = 0x78,
		VK_F10 = 0x79,
		VK_F11 = 0x7A,
		VK_F12 = 0x7B,
		VK_F13 = 0x7C,
		VK_F14 = 0x7D,
		VK_F15 = 0x7E,
		VK_F16 = 0x7F,
		VK_F17 = 0x80,
		VK_F18 = 0x81,
		VK_F19 = 0x82,
		VK_F20 = 0x83,
		VK_F21 = 0x84,
		VK_F22 = 0x85,
		VK_F23 = 0x86,
		VK_F24 = 0x87,

		//
		// 0x88 - 0x8F : unassigned
		//

		VK_NUMLOCK = 0x90,
		VK_SCROLL = 0x91,

		//
		// VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
		// Used only as parameters to GetAsyncKeyState() and GetKeyState().
		// No other API or message will distinguish left and right keys in this way.
		//
		VK_LSHIFT = 0xA0,
		VK_RSHIFT = 0xA1,
		VK_LCONTROL = 0xA2,
		VK_RCONTROL = 0xA3,
		VK_LMENU = 0xA4,
		VK_RMENU = 0xA5,

		VK_BROWSER_BACK = 0xA6,
		VK_BROWSER_FORWARD = 0xA7,
		VK_BROWSER_REFRESH = 0xA8,
		VK_BROWSER_STOP = 0xA9,
		VK_BROWSER_SEARCH = 0xAA,
		VK_BROWSER_FAVORITES = 0xAB,
		VK_BROWSER_HOME = 0xAC,

		VK_VOLUME_MUTE = 0xAD,
		VK_VOLUME_DOWN = 0xAE,
		VK_VOLUME_UP = 0xAF,
		VK_MEDIA_NEXT_TRACK = 0xB0,
		VK_MEDIA_PREV_TRACK = 0xB1,
		VK_MEDIA_STOP = 0xB2,
		VK_MEDIA_PLAY_PAUSE = 0xB3,
		VK_LAUNCH_MAIL = 0xB4,
		VK_LAUNCH_MEDIA_SELECT = 0xB5,
		VK_LAUNCH_APP1 = 0xB6,
		VK_LAUNCH_APP2 = 0xB7,

		//
		// 0xB8 - 0xB9 : reserved
		//

		VK_OEM_1 = 0xBA,   // ';:' for US
		VK_OEM_PLUS = 0xBB,   // '+' any country
		VK_OEM_COMMA = 0xBC,   // ',' any country
		VK_OEM_MINUS = 0xBD,   // '-' any country
		VK_OEM_PERIOD = 0xBE,   // '.' any country
		VK_OEM_2 = 0xBF,   // '/?' for US
		VK_OEM_3 = 0xC0,   // '`~' for US

		//
		// 0xC1 - 0xD7 : reserved
		//

		//
		// 0xD8 - 0xDA : unassigned
		//

		VK_OEM_4 = 0xDB,  //  '[{' for US
		VK_OEM_5 = 0xDC,  //  '\|' for US
		VK_OEM_6 = 0xDD,  //  ']}' for US
		VK_OEM_7 = 0xDE,  //  ''"' for US
		VK_OEM_8 = 0xDF

		//
		// 0xE0 : reserved
		//
	}
	/*
     
	 Big GOTCHA: It is very unclear from Microsoft's documentation, but if you use a VK_ code, and that code represents a key that is an "Extended" key, you can get some strange responses. Things won't work for odd reasons. But, a line like:

if ((x >= 33 && x <= 46) || (x >= 91 && x <= 93)) ki.dwFlags += KEYEVENTF_EXTENDEDKEY;

will fix this. The affected keys are identifiable by E0 in the scan code. This is true even though scan codes and VK_ enumeration codes are different! It is the key that makes the difference. You have to declare it "extended" as in the code line above.

Another GOTCHA: Characters appear in text boxes just fine. BUT, if you want to send control codes such as <ctl>C, the system converts the unicode character to a packet and it will not work. In this case, you have to use "real" scan codes with appropriate modifier keys.
*/

	public sealed class InputUtil
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetMessageExtraInfo();

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

		const int INPUT_MOUSE = 0;
		const int INPUT_KEYBOARD = 1;
		const int INPUT_HARDWARE = 2;

		const uint KEYEVENTF_KEYDOWN = 0x0000;
		const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		const uint KEYEVENTF_KEYUP = 0x0002;
		const uint KEYEVENTF_UNICODE = 0x0004;
		const uint KEYEVENTF_SCANCODE = 0x0008;
		const uint XBUTTON1 = 0x0001;
		const uint XBUTTON2 = 0x0002;
		const uint MOUSEEVENTF_MOVE = 0x0001;
		const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		const uint MOUSEEVENTF_LEFTUP = 0x0004;
		const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
		const uint MOUSEEVENTF_XDOWN = 0x0080;
		const uint MOUSEEVENTF_XUP = 0x0100;
		const uint MOUSEEVENTF_WHEEL = 0x0800;
		const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
		const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

		public static void SendKeyPress(VK key, bool alt, bool ctrl, bool shift)
		{
			int cbSize = Marshal.SizeOf(new INPUT());
			INPUT kAlt;
			INPUT kCtrl;
			INPUT kShift;
			INPUT k;
			List<INPUT> ins = new List<INPUT>();
			if (alt)
			{
				kAlt = new INPUT();
				kAlt.type = INPUT_KEYBOARD;
				kAlt.mkhi.ki.wScan = 0;
				kAlt.mkhi.ki.time = 0;
				kAlt.mkhi.ki.dwFlags = 0;
				kAlt.mkhi.ki.dwExtraInfo = IntPtr.Zero;
				kAlt.mkhi.ki.wVk = (ushort)VK.VK_MENU;
				ins.Add(kAlt);
			}
			if (ctrl)
			{
				kCtrl = new INPUT();
				kCtrl.type = INPUT_KEYBOARD;
				kCtrl.mkhi.ki.wScan = 0;
				kCtrl.mkhi.ki.time = 0;
				kCtrl.mkhi.ki.dwFlags = 0;
				kCtrl.mkhi.ki.dwExtraInfo = IntPtr.Zero;
				kCtrl.mkhi.ki.wVk = (ushort)VK.VK_CONTROL;
				ins.Add(kCtrl);
			}
			if (shift)
			{
				kShift = new INPUT();
				kShift.type = INPUT_KEYBOARD;
				kShift.mkhi.ki.wScan = 0;
				kShift.mkhi.ki.time = 0;
				kShift.mkhi.ki.dwFlags = 0;
				kShift.mkhi.ki.dwExtraInfo = IntPtr.Zero;
				kShift.mkhi.ki.wVk = (ushort)VK.VK_SHIFT;
				ins.Add(kShift);
			}
			if (key != VK.VK_VOID)
			{
				k = new INPUT();
				k.type = INPUT_KEYBOARD;
				k.mkhi.ki.wScan = 0;
				k.mkhi.ki.time = 0;
				k.mkhi.ki.dwFlags = 0;
				k.mkhi.ki.dwExtraInfo = IntPtr.Zero;
				k.mkhi.ki.wVk = (ushort)key;
				ins.Add(k);
			}
			if (ins.Count > 0)
			{
				INPUT[] structInput = new INPUT[ins.Count];
				ins.CopyTo(structInput);
				//
				for (int i = 0; i < structInput.Length; i++)
				{
					ushort x = structInput[i].mkhi.ki.wVk;
					if ((x >= 33 && x <= 46) || (x >= 91 && x <= 93))
					{
						structInput[i].mkhi.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
					}
				}
				//
				SendInput((uint)structInput.Length, ref structInput[0], cbSize);
				//
				for (int i = 0; i < structInput.Length; i++)
				{
					structInput[i].mkhi.ki.dwFlags = KEYEVENTF_KEYUP;
					structInput[i].mkhi.ki.wVk = (ushort)key;
					structInput[i].mkhi.ki.time = 0;
					ushort x = structInput[i].mkhi.ki.wVk;
					if ((x >= 33 && x <= 46) || (x >= 91 && x <= 93))
					{
						structInput[i].mkhi.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
					}
				}
				//
				SendInput((uint)structInput.Length, ref structInput[0], cbSize);
			}
		}
		public static void SendKeyPress(VK key)
		{
			int cbSize = Marshal.SizeOf(new INPUT());
			INPUT[] structInput = new INPUT[1];
			structInput[0] = new INPUT();
			structInput[0].type = INPUT_KEYBOARD;
			structInput[0].mkhi.ki.wScan = 0;
			structInput[0].mkhi.ki.time = 0;
			structInput[0].mkhi.ki.dwFlags = 0;
			structInput[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;

			ushort x = (ushort)key;

			// Key down the actual key-code 
			structInput[0].mkhi.ki.dwFlags = KEYEVENTF_KEYDOWN;
			structInput[0].mkhi.ki.wVk = (ushort)key;
			if ((x >= 33 && x <= 46) || (x >= 91 && x <= 93))
			{
				structInput[0].mkhi.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
			}
			SendInput(1, ref structInput[0], cbSize);

			// Key up the actual key-code 
			structInput[0].mkhi.ki.dwFlags = KEYEVENTF_KEYUP;
			structInput[0].mkhi.ki.wVk = (ushort)key;
			structInput[0].mkhi.ki.time = 0;
			if ((x >= 33 && x <= 46) || (x >= 91 && x <= 93))
			{
				structInput[0].mkhi.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
			}
			SendInput((uint)1, ref structInput[0], cbSize);
		}
		public static void ClickMouse(int x0, int y0)
		{
			double sx = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
			double sy = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
			sx = 65535.0 / sx;
			sy = 65535.0 / sy;
			Int32 x = (Int32)(sx * x0);
			Int32 y = (Int32)(sy * y0);
			INPUT[] input = new INPUT[3];
			for (int i = 0; i < 3; i++)
			{
				input[i] = new INPUT();
				input[i].mkhi.mi.dwExtraInfo = IntPtr.Zero;
				input[i].mkhi.mi.mouseData = 0;
				input[i].mkhi.mi.time = 0;
				input[i].mkhi.mi.dwFlags = 0;
				input[i].mkhi.mi.dx = 0;
				input[i].mkhi.mi.dy = 0;
			}
			input[0].type = INPUT_MOUSE;
			input[0].mkhi.mi.time = 0;
			input[0].mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;
			input[0].mkhi.mi.dx = x;
			input[0].mkhi.mi.dy = y;
			input[1].type = INPUT_MOUSE;
			input[1].mkhi.mi.time = 0;
			input[1].mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN;
			input[1].mkhi.mi.dx = x;
			input[1].mkhi.mi.dy = y;
			input[2].type = INPUT_MOUSE;
			input[2].mkhi.mi.time = 0;
			input[2].mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP;
			input[2].mkhi.mi.dx = x;
			input[2].mkhi.mi.dy = y;

			SendInput(3, ref input[0], Marshal.SizeOf(input[0]));
		}
		public static void ClickMouse()
		{
			Int32 x = System.Windows.Forms.Cursor.Position.X;
			Int32 y = System.Windows.Forms.Cursor.Position.Y;
			INPUT[] input = new INPUT[2];
			for (int i = 0; i < 2; i++)
			{
				input[i] = new INPUT();
				input[i].mkhi.mi.dwExtraInfo = IntPtr.Zero;
				input[i].mkhi.mi.mouseData = 0;
				input[i].mkhi.mi.time = 0;
				input[i].mkhi.mi.dwFlags = 0;
				input[i].mkhi.mi.dx = 0;
				input[i].mkhi.mi.dy = 0;
			}
			input[0].type = INPUT_MOUSE;
			input[0].mkhi.mi.time = 0;
			input[0].mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN;
			input[0].mkhi.mi.dx = x;
			input[0].mkhi.mi.dy = y;
			//
			input[1].type = INPUT_MOUSE;
			input[1].mkhi.mi.time = 0;
			input[1].mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP;
			input[1].mkhi.mi.dx = x;
			input[1].mkhi.mi.dy = y;

			SendInput(2, ref input[0], Marshal.SizeOf(input[0]));
		}
		public static void MoveMouse(int x0, int y0)
		{
			double sx = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
			double sy = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
			sx = 65535.0 / sx;
			sy = 65535.0 / sy;
			Int32 x = (Int32)(sx * x0);
			Int32 y = (Int32)(sy * y0);
			INPUT[] input = new INPUT[1];
			input[0] = new INPUT();
			input[0].type = INPUT_MOUSE;
			input[0].mkhi.mi.time = 0;
			input[0].mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;
			input[0].mkhi.mi.dx = x;
			input[0].mkhi.mi.dy = y;
			SendInput(1, ref input[0], Marshal.SizeOf(input[0]));
		}
	}
}
