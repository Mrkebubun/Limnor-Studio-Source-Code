/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Code Dom Helper
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorVisualProgramming
{
	public static class CodeDomHelper
	{
		public static sbyte ShifLeft(sbyte n, byte s)
		{
			return (sbyte)(n << s);
		}
		public static short ShifLeft(short n, byte s)
		{
			return (short)(n << s);
		}
		public static int ShifLeft(int n, byte s)
		{
			return n << s;
		}
		public static long ShifLeft(long n, byte s)
		{
			return n << s;
		}
		//
		public static sbyte ShifRight(sbyte n, byte s)
		{
			return (sbyte)(n >> s);
		}
		public static short ShifRight(short n, byte s)
		{
			return (short)(n >> s);
		}
		public static int ShifRight(int n, byte s)
		{
			return n >> s;
		}
		public static long ShifRight(long n, byte s)
		{
			return n >> s;
		}
		//
		public static byte ShifLeft(byte n, byte s)
		{
			return (byte)(n << s);
		}
		public static ushort ShifLeft(ushort n, byte s)
		{
			return (ushort)(n << s);
		}
		public static uint ShifLeft(uint n, byte s)
		{
			return n << s;
		}
		public static ulong ShifLeft(ulong n, byte s)
		{
			return n << s;
		}
		//
		public static byte ShifRight(byte n, byte s)
		{
			return (byte)(n >> s);
		}
		public static ushort ShifRight(ushort n, byte s)
		{
			return (ushort)(n >> s);
		}
		public static uint ShifRight(uint n, byte s)
		{
			return n >> s;
		}
		public static ulong ShifRight(ulong n, byte s)
		{
			return n >> s;
		}
	}
}
