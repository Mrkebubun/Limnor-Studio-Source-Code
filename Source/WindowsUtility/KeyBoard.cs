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
	struct KEYBOARD_INPUT
	{
		public int type;
		public ushort wVk;
		public ushort scanCode;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
	}
	public static class InputKeyboard
	{

		private const int INPUT_KEYBOARD = 1;
		private const int KEY_EXTENDED = 0x0001;
		private const uint KEY_UP = 0x0002;
		private const uint KEYEVENTF_UNICODE = 0x0004; //THIS WORKS AS DEFAULT FLAG
		private const uint KEYEVENTF_SCANCODE = 0x0008;   //IF I USE THIS IN THE FLAGS STRANGE RESULTS
		[DllImport("User32.dll")]
		private static extern uint SendInput(uint numberOfInputs,
		[MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] KEYBOARD_INPUT[] input, int structSize);

		public static void SendKeyP(char cKey)
		{
			ushort iKey = (byte)cKey;
			SendKey(iKey, true);
		}
		/// <summary>
		/// Presses a Key
		/// </summary>
		/// 
		public static void SendKeyP(int scanCode)
		{
			SendKey(scanCode, true);
		}

		public static void SendKeyR(char cKey)
		{
			ushort iKey = (byte)cKey;
			SendKey(iKey, false);
		}
		/// <summary>
		/// Releases Key
		/// </summary>
		/// 
		///
		public static void SendKeyR(int scanCode)
		{
			SendKey(scanCode, false);
		}
		/// <summary>
		/// single Char Key, Press and Release
		/// </summary>
		/// 
		/// 
		public static void SendKey(char cKey)
		{
			int iKey = (Byte)cKey;
			SendKey(iKey, true);
			SendKey(iKey, false);

		}
		/// <summary>
		/// Single Int KeyCode hex or ascii
		/// </summary>
		/// 
		/// 
		public static void SendKey(int scanCode, bool press)
		{
			KEYBOARD_INPUT[] input = new KEYBOARD_INPUT[1];
			input[0] = new KEYBOARD_INPUT();
			input[0].type = INPUT_KEYBOARD;
			input[0].flags = KEYEVENTF_UNICODE;
			input[0].time = 0;
			input[0].wVk = 0;
			input[0].dwExtraInfo = IntPtr.Zero;

			if ((scanCode & 0xFF00) == 0xE000)
			{ // extended key?
				input[0].flags |= KEY_EXTENDED;
			}

			if (press)
			{ // press?
				input[0].scanCode = (ushort)(scanCode & 0xFF);
			}
			else
			{ // release?
				input[0].scanCode = (ushort)scanCode;
				input[0].flags |= KEY_UP;
			}

			uint result = SendInput(1, input, Marshal.SizeOf(input[0]));

			if (result != 1)
			{
				throw new Exception("Could not send key: " + scanCode);
			}
		}


		/// <summary>
		/// String of 1 to many letters, words, etc example
		/// SendKeys("HelloWorld");
		/// this does a Press and Release
		/// </summary>
		/// 
		public static void SendKeys(string sKeys)
		{
			Byte[] aKeys = UTF8Encoding.ASCII.GetBytes(sKeys);
			int iLen = aKeys.Length;
			ushort scanCode = 0;
			for (int x = 0; x < iLen; x++)
			{
				//2 elements for each key, one for press one for Release
				scanCode = aKeys[x];
				KEYBOARD_INPUT[] input = new KEYBOARD_INPUT[2];
				input[0] = new KEYBOARD_INPUT();
				input[0].type = INPUT_KEYBOARD;
				input[0].flags = KEYEVENTF_UNICODE;

				input[1] = new KEYBOARD_INPUT();
				input[1].type = INPUT_KEYBOARD;
				input[1].flags = KEYEVENTF_UNICODE;
				if ((scanCode & 0xFF00) == 0xE000)
				{ // extended key?
					input[0].flags |= KEY_EXTENDED;
					input[1].flags |= KEY_EXTENDED;
				}

				//input[0] for key press
				input[0].scanCode = (ushort)(scanCode & 0xFF);
				//input[1] for KeyReleasse
				input[1].scanCode = (ushort)scanCode;
				input[1].flags |= KEY_UP;
				//call sendinput once for both keys
				//result = 
				SendInput(2, input, Marshal.SizeOf(input[0]));

			}
		}

	}
}
