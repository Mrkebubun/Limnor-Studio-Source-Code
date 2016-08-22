/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace VOB
{
	public sealed class RegistryData
	{
		enum RegWow64Options
		{
			None = 0,
			KEY_WOW64_64KEY = 0x0100,
			KEY_WOW64_32KEY = 0x0200
		}

		enum RegistryRights
		{
			ReadKey = 131097,
			WriteKey = 131078
		}

		/// <summary>
		/// Open a registry key using the Wow64 node instead of the default 32-bit node.
		/// </summary>
		/// <param name="parentKey">Parent key to the key to be opened.</param>
		/// <param name="subKeyName">Name of the key to be opened</param>
		/// <param name="writable">Whether or not this key is writable</param>
		/// <param name="options">32-bit node or 64-bit node</param>
		/// <returns></returns>
		static RegistryKey _openSubKey(RegistryKey parentKey, string subKeyName, bool writable, RegWow64Options options)
		{
			//Sanity check
			if (parentKey == null || _getRegistryKeyHandle(parentKey) == IntPtr.Zero)
			{
				return null;
			}

			//Set rights
			int rights = (int)RegistryRights.ReadKey;
			if (writable)
				rights = (int)RegistryRights.WriteKey;

			//Call the native function >.<
			int subKeyHandle, result = RegOpenKeyEx(_getRegistryKeyHandle(parentKey), subKeyName, 0, rights | (int)options, out subKeyHandle);

			//If we errored, return null
			if (result != 0)
			{
				return null;
			}

			//Get the key represented by the pointer returned by RegOpenKeyEx
			RegistryKey subKey = _pointerToRegistryKey((IntPtr)subKeyHandle, writable, false);
			return subKey;
		}

		/// <summary>
		/// Get a pointer to a registry key.
		/// </summary>
		/// <param name="registryKey">Registry key to obtain the pointer of.</param>
		/// <returns>Pointer to the given registry key.</returns>
		static IntPtr _getRegistryKeyHandle(RegistryKey registryKey)
		{
			//Get the type of the RegistryKey
			Type registryKeyType = typeof(RegistryKey);
			//Get the FieldInfo of the 'hkey' member of RegistryKey
			System.Reflection.FieldInfo fieldInfo =
			registryKeyType.GetField("hkey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			//Get the handle held by hkey
			SafeHandle handle = (SafeHandle)fieldInfo.GetValue(registryKey);
			//Get the unsafe handle
			IntPtr dangerousHandle = handle.DangerousGetHandle();
			return dangerousHandle;
		}

		/// <summary>
		/// Get a registry key from a pointer.
		/// </summary>
		/// <param name="hKey">Pointer to the registry key</param>
		/// <param name="writable">Whether or not the key is writable.</param>
		/// <param name="ownsHandle">Whether or not we own the handle.</param>
		/// <returns>Registry key pointed to by the given pointer.</returns>
		static RegistryKey _pointerToRegistryKey(IntPtr hKey, bool writable, bool ownsHandle)
		{
			//Get the BindingFlags for private contructors
			System.Reflection.BindingFlags privateConstructors = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
			//Get the Type for the SafeRegistryHandle
			Type safeRegistryHandleType = typeof(Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid).Assembly.GetType("Microsoft.Win32.SafeHandles.SafeRegistryHandle");
			//Get the array of types matching the args of the ctor we want
			Type[] safeRegistryHandleCtorTypes = new Type[] { typeof(IntPtr), typeof(bool) };
			//Get the constructorinfo for our object
			System.Reflection.ConstructorInfo safeRegistryHandleCtorInfo = safeRegistryHandleType.GetConstructor(
			privateConstructors, null, safeRegistryHandleCtorTypes, null);
			//Invoke the constructor, getting us a SafeRegistryHandle
			Object safeHandle = safeRegistryHandleCtorInfo.Invoke(new Object[] { hKey, ownsHandle });

			//Get the type of a RegistryKey
			Type registryKeyType = typeof(RegistryKey);
			//Get the array of types matching the args of the ctor we want
			Type[] registryKeyConstructorTypes = new Type[] { safeRegistryHandleType, typeof(bool) };
			//Get the constructorinfo for our object
			System.Reflection.ConstructorInfo registryKeyCtorInfo = registryKeyType.GetConstructor(
			privateConstructors, null, registryKeyConstructorTypes, null);
			//Invoke the constructor, getting us a RegistryKey
			RegistryKey resultKey = (RegistryKey)registryKeyCtorInfo.Invoke(new Object[] { safeHandle, writable });
			//return the resulting key
			return resultKey;
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, int ulOptions, int samDesired, out int phkResult);


		/*
				RegistryKey sqlsrvKey = _openSubKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Microsoft SQL Server\90", false, RegWow64Options.KEY_WOW64_64KEY);
		*/

		public static string GetTutorialFolder()
		{
			string folder = null;
#if DOTNET40
			RegistryKey localKey32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
			if (localKey32 != null)
			{
				RegistryKey key = localKey32.OpenSubKey(@"SOFTWARE\Longflow Enterprises\Limnor Tutorial");
				if (localKey32 != null)
				{
					object v = key.GetValue("Tutorial");
					if (v != null)
					{
						folder = v.ToString();
					}
					key.Close();
				}
				localKey32.Close();
			}
#else
			if (IntPtr.Size == 4)
			{
				RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Longflow Enterprises\Limnor Tutorial");
				if (key != null)
				{
					object v = key.GetValue("Tutorial");
					if (v != null)
					{
						folder = v.ToString();
					}
					key.Close();
				}
			}
			else
			{
				RegistryKey key = _openSubKey(Registry.LocalMachine, @"SOFTWARE\Longflow Enterprises\Limnor Tutorial", false, RegWow64Options.KEY_WOW64_32KEY);
				if (key != null)
				{
					object v = key.GetValue("Tutorial");
					if (v != null)
					{
						folder = v.ToString();
					}
					key.Close();
				}
			}
#endif
			if (!string.IsNullOrEmpty(folder))
			{
				return Path.Combine(folder, "Tutorials");
			}
			return null;
		}
	}
}
