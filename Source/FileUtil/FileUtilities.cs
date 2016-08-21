/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	File Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace FileUtil
{
	public sealed class FileUtilities
	{
		private FileUtilities()
		{
		}
		public static string FormValidFilename(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				filename = "file1";
			}
			else
			{
				char[] invs = System.IO.Path.GetInvalidFileNameChars();
				for (int i = 0; i < invs.Length; i++)
				{
					filename = filename.Replace(invs[i], '_');
				}
			}
			return filename;
		}
		public static string CreateNewFilename(string filename)
		{
			if (System.IO.File.Exists(filename))
			{
				string sFolder = System.IO.Path.GetDirectoryName(filename);
				string sFilename = System.IO.Path.GetFileNameWithoutExtension(filename);
				string sExt = System.IO.Path.GetExtension(filename);
				if (string.IsNullOrEmpty(sFilename))
				{
					sFilename = "file";
				}
				int n = -1;
				for (int i = sFilename.Length - 1; i >= 0; i--)
				{
					if (sFilename[i] >= '0' && sFilename[i] <= '9')
					{
						n = i;
					}
				}
				if (n == 0)
				{
					sFilename = "file";
				}
				else if (n > 0)
				{
					sFilename = sFilename.Substring(0, n);
				}
				n = 1;
				while (System.IO.File.Exists(filename))
				{
					n++;
					filename = System.IO.Path.Combine(sFolder, sFilename) + n.ToString() + sExt;
				}
			}
			return filename;
		}
		public static void ClearFolders(string folder)
		{
			string[] files = Directory.GetFiles(folder);
			if (files != null && files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					File.Delete(files[i]);
				}
			}
			files = Directory.GetDirectories(folder);
			if (files != null && files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					ClearFolders(files[i]);
				}
			}
		}
		public static bool IsFilepathValid(string fullpath)
		{
			if (string.IsNullOrEmpty(fullpath))
				return false;
			string path = System.IO.Path.GetDirectoryName(fullpath);
			string filename = System.IO.Path.GetFileName(fullpath);

			if (!string.IsNullOrEmpty(path))
			{
				char[] cc = System.IO.Path.GetInvalidPathChars();
				for (int i = 0; i < cc.Length; i++)
				{
					if (path.IndexOf(cc[i]) >= 0)
						return false;
				}
			}
			if (!string.IsNullOrEmpty(filename))
			{
				char[] cc = System.IO.Path.GetInvalidFileNameChars();
				for (int i = 0; i < cc.Length; i++)
				{
					if (filename.IndexOf(cc[i]) >= 0)
						return false;
				}
			}
			return true;
		}
		public static void UpdateFile(string source, string target)
		{
			bool bCopy = File.Exists(source);
			if (bCopy)
			{
				if (File.Exists(target))
				{
					FileInfo fiSource = new FileInfo(source);
					FileInfo fiTarget = new FileInfo(target);
					if (fiTarget.LastWriteTimeUtc <= fiSource.LastWriteTimeUtc)
					{
						bCopy = false;
					}
				}
			}
			if (bCopy)
			{
				File.Copy(source, target, true);
			}
		}
		public static void UpdateFile(string sourceFolder, string targetFolder, string filename)
		{
			UpdateFile(System.IO.Path.Combine(sourceFolder, filename), System.IO.Path.Combine(targetFolder, filename));
		}
		public static void CopyDir(string source, string target, string pattern, StringCollection excludedNames)
		{
			string[] files = Directory.GetFiles(source, pattern);
			if (files != null && files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					string name = Path.GetFileName(files[i]);
					if (excludedNames != null && excludedNames.Contains(name.ToLowerInvariant()))
					{
						continue;
					}
					string tf = Path.Combine(target, name);
					if (File.Exists(tf))
					{
						FileInfo fi = new FileInfo(tf);
						if (fi.IsReadOnly)
						{
							continue;
						}
					}
					File.Copy(files[i], tf, true);
				}
			}
		}
		public static void CopyDirs(string source, string target, string pattern, StringCollection excludedNames)
		{
			if (!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
			}
			CopyDir(source, target, pattern, excludedNames);
			string[] folders = Directory.GetDirectories(source);
			if (folders != null && folders.Length > 0)
			{
				for (int i = 0; i < folders.Length; i++)
				{
					string tgt = Path.Combine(target, Path.GetFileName(folders[i]));
					CopyDirs(folders[i], tgt, pattern, excludedNames);
				}
			}
		}
	}
}
