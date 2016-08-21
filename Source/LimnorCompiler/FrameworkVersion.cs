/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
namespace LimnorCompiler
{
	public enum FrameworkVersion
	{
		/// <summary>
		/// The .NET Framework version was unrecognized
		/// </summary>
		Unknown = -1,

		/// <summary>
		/// .NET Framework 2.0 (includes .NET 3.0 & 3.5)
		/// </summary>
		Net2_0 = 0,

		/// <summary>
		/// .NET Framework 4.0
		/// </summary>
		Net4_0 = 1
	}
}