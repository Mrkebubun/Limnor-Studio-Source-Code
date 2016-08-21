/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Limnor.WebBuilder;
using System.Security.Permissions;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.Web
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[ComVisible(true)]
	public class ScriptManager
	{
		private WebPage _designPage;
		public ScriptManager(WebPage page)
		{
			_designPage = page;
		}
		public void onPageStarted()
		{
			_designPage.onPageStarted();
		}
		public void OnElementIdChanged(string guid, string id)
		{
			if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(id))
			{
				Guid g = new Guid(guid);
				_designPage.OnElementIdChanged(g, id);
			}
		}
		public void OnElementSelected(string selectedElement)
		{
			_designPage.OnSelectHtmlElement(selectedElement);
		}
		public void OnRightClickElement(string selectedElement, int x, int y)
		{
			_designPage.OnRightClickElement(selectedElement, x, y);
		}
		public void OnEditorStarted()
		{
			_designPage.OnEditorStarted();
		}
		public void OnLoadEditorFailed()
		{
			_designPage.OnLoadEditorFailed();
		}
		public void OnShowDebugInfo(string message)
		{
			FormHtmlEditorDebug.Log(message);
		}
		public void OnUpdated()
		{
			_designPage.OnUpdated();
		}
		public string OnSelectFile(string title, string filetypes, string subFolder, string subName, int msize, bool disableUpload)
		{
			return _designPage.OnSelectFile( title,  filetypes,  subFolder,  subName,  msize,  disableUpload);
		}
		public void OnBodyStyleChanged(string styleName, string val)
		{
			if (string.Compare(styleName, "backgroundImage", StringComparison.OrdinalIgnoreCase) == 0)
			{
				_designPage.BackgroundImageFile = null;
			}
			else if (string.Compare(styleName, "backgroundRepeat", StringComparison.OrdinalIgnoreCase) == 0)
			{
				bool tile = false;
				if (!string.IsNullOrEmpty(val))
				{
					if (val.StartsWith("repeat", StringComparison.OrdinalIgnoreCase))
					{
						tile = true;
					}
				}
				_designPage.ReadingProperties = true;
				_designPage.BackgroundImageTile = tile;
				_designPage.ReadingProperties = false;
			}
		}
		public void OnAddFileupload(string id)
		{
			_designPage.OnAddFileupload(id);
		}
		private void mnu_doHtmlCopy(object sender, EventArgs e)
		{
			MenuItemWithBitmap mi = sender as MenuItemWithBitmap;
			if (mi != null && mi.Tag != null)
			{
				string s = mi.Tag.ToString();
				if (!string.IsNullOrEmpty(s))
				{
					Clipboard.SetText(s);
				}
			}
		}
		private void mnu_doHtmlPaste(object sender, EventArgs e)
		{
			MenuItemWithBitmap mi = sender as MenuItemWithBitmap;
			if (mi != null && mi.Tag != null)
			{
				if (Clipboard.ContainsText())
				{
					string txt = Clipboard.GetText();
					Point p = (Point)mi.Tag;
					IWebHost w = _designPage.GetHtmlEditor();
					w.PasteToHtmlInput(txt, p.X, p.Y);
				}
			}
		}
		public void OnPropInputMouseDown(string selectedText, int selStart, int selEnd, int x, int y)
		{
			Point p = Point.Empty;
			if (x != 0 && y != 0)
			{
				p = new Point(x, y);
			}
			if (p.IsEmpty)
			{
				p = System.Windows.Forms.Cursor.Position;
			}
			ContextMenu cmn = new ContextMenu();
			MenuItemWithBitmap mi = new MenuItemWithBitmap("Copy", mnu_doHtmlCopy, Resources._copy.ToBitmap());
			mi.Tag = selectedText;
			cmn.MenuItems.Add(mi);
			mi = new MenuItemWithBitmap("Paste", mnu_doHtmlPaste, Resources.paste);
			mi.Tag = new Point(selStart, selEnd);
			cmn.MenuItems.Add(mi);
			cmn.Show(_designPage.GetHtmlEditor() as Control, p);
		}
	}
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[ComVisible(true)]
	public class ScriptManager0
	{
		private FormWeb _designPage;
		public ScriptManager0(FormWeb page)
		{
			_designPage = page;
		}
		public void onPageStarted()
		{
			_designPage.onPageStarted();
		}
	}
}
