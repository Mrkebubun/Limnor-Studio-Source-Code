/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
namespace LimnorCompiler
{
	public enum CPUVersion
	{
		/// <summary>
		/// A .NET assembly that is compiled with the AnyCPU flag
		/// </summary>
		AnyCPU = 0,

		/// <summary>
		/// A file that's built for x86 machines
		/// </summary>
		x86 = 1,

		/// <summary>
		/// A file that's built for x64 machines
		/// </summary>
		x64 = 2
	}
}