/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XmlUtility;
using System.IO;
using System.Globalization;
using LimnorWeb;
using VSPrj;
using Limnor.WebBuilder;
using System.Xml;
using MathExp;
using mshtml;
using VPL;

namespace LimnorDesigner.Web
{
	/// <summary>
	/// HTML designer in parallel with form designer (WebPage), at anytime only one of the two is displayed, toggle them by a label
	/// </summary>
	public partial class WebHost : UserControl, ISkipWrite, IWebHost
	{
		#region fields and constructors
		private string _projectFolder;
		private string _htmlUrl;
		private string _htmlFile;
		private bool _htmlChanged;
		private string _docType;
		private Encoding _encoding = Encoding.Unicode;
		private bool _editorStarted;
		private bool _editorLoaded;
		private string _selectedElement = string.Empty;
		private WebPage _page;
		private string _webPhysicalFolder;
		public WebHost()
		{
			InitializeComponent();
			webBrowser1.HtmlText = Resources.emptyPageEdit;
			this.Dock = DockStyle.Fill;
			this.AutoScroll = false;
		}
		private ScriptManager _sm;
		public WebHost(string prjFolder, string webFolder, string webName, string htmlFile, Encoding encode, string docType, WebPage wpage)
		{
			InitializeComponent();
			//
			_projectFolder = prjFolder;
			_webPhysicalFolder = webFolder;
			_encoding = encode;
			_docType = docType;
			_page = wpage;
			_sm = new ScriptManager(_page);
			_page.SetHtmlEditor(this);
			webBrowser1.AllowWebBrowserDrop = false;
			webBrowser1.IsWebBrowserContextMenuEnabled = false;
			webBrowser1.WebBrowserShortcutsEnabled = false;
			webBrowser1.ObjectForScripting = _sm;
			this.Dock = DockStyle.Fill;
			this.AutoScroll = false;
			_htmlFile = htmlFile;
			//physical folder at {project folder}/Webfiles
			//default virtual folder at localhost/{project folder}
			_htmlUrl = string.Format(CultureInfo.InvariantCulture,
				"http://localhost/{0}/{1}", webName, Path.GetFileName(htmlFile));
		}

		#endregion
		#region Static utility
		private static string[] _htmlEditorJsFiles;
		private static string[] _htmlEditorCssFiles;
		private static string[] _htmlEditorImages;
		private static string[] _jscolorImages;
		static WebHost()
		{
			//TBD2
			_htmlEditorJsFiles = new string[3];
			_htmlEditorJsFiles[0] = "pageStarter.js";
			_htmlEditorJsFiles[1] = "slideshow.js";
			_htmlEditorJsFiles[2] = "limnorsvg.js";
			_htmlEditorCssFiles = new string[1];
			_htmlEditorCssFiles[0] = "limnortv.css";
			//
			_jscolorImages = new string[0];
			//
			_htmlEditorImages = new string[0];
		}
		public static VirtualWebDir GetWebSite(LimnorProject project, Form caller)
		{
			return project.VerifyWebSite(caller);
		}
		public static VirtualWebDir CreateWebSite(string webSiteName, LimnorProject project, Form caller)
		{
			try
			{
				return LimnorWebApp.CreateWebSite(project, webSiteName, caller);
			}
			catch (Exception err)
			{
				MathNode.Log(err, "Cannot create website [{0}] on {1}", webSiteName, project.WebPhysicalFolder(caller));
			}
			return null;
		}
		public static string[] HtmlEditorImages
		{
			get
			{
				return _htmlEditorImages;
			}
		}
		public static string[] HtmlJsColorImages
		{
			get
			{
				return _jscolorImages;
			}
		}
		public static string GetHtmlFile(string componentFile)
		{
			string f = Path.GetFileNameWithoutExtension(componentFile);
			return Path.Combine(Path.GetDirectoryName(componentFile), string.Format(CultureInfo.InvariantCulture, "{0}_design.html", f));
		}
		public static void CopyFile(string f, string folder, string webFolder)
		{
			string src = null;
			string tgt = null;
			try
			{
				src = Path.Combine(folder, f);
				tgt = Path.Combine(webFolder, f);
				if (File.Exists(tgt))
				{
					FileInfo fi = new FileInfo(tgt);
					FileInfo f0 = new FileInfo(src);
					if (fi.LastWriteTime >= f0.LastWriteTime)
					{
						return;
					}
				}
				File.Copy(src, tgt, true);
			}
			catch (Exception err)
			{
				throw new DesignerException(err, "Error copying file from [{0}] to [{1}]", src, tgt);
			}
		}
		public static void CopyJsLib(string webFolder)
		{
			string libjs = "libjs";
			string folder = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DistributeIIS"), libjs);
			string wfolder = Path.Combine(webFolder, libjs);
			if (!Directory.Exists(wfolder))
			{
				Directory.CreateDirectory(wfolder);
			}
			for (int i = 0; i < _htmlEditorJsFiles.Length; i++)
			{
				CopyFile(_htmlEditorJsFiles[i], folder, wfolder);
			}
			for (int i = 0; i < _htmlEditorImages.Length; i++)
			{
				CopyFile(_htmlEditorImages[i], folder, wfolder);
			}
			//
			libjs = "css";
			folder = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DistributeIIS"), libjs);
			wfolder = Path.Combine(webFolder, libjs);
			if (!Directory.Exists(wfolder))
			{
				Directory.CreateDirectory(wfolder);
			}
			for (int i = 0; i < _htmlEditorCssFiles.Length; i++)
			{
				CopyFile(_htmlEditorCssFiles[i], folder, wfolder);
			}
		}
		public static void CopyJsColor(string webFolder)
		{
			const string libjs = "jsColor";
			string folder = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DistributeIIS"), libjs);
			string wfolder = Path.Combine(webFolder, libjs);
			if (!Directory.Exists(wfolder))
			{
				Directory.CreateDirectory(wfolder);
			}
			for (int i = 0; i < _jscolorImages.Length; i++)
			{
				CopyFile(_jscolorImages[i], folder, wfolder);
			}
		}
		private static void copyFolder(string srcDir, string tgtDir, StringBuilder errs)
		{
			if (!Directory.Exists(tgtDir))
			{
				Directory.CreateDirectory(tgtDir);
			}
			string s = VPLUtil.GrantFolderFullPermission(tgtDir);
			if (!string.IsNullOrEmpty(s))
			{
				errs.Append(string.Format(CultureInfo.InvariantCulture, "Error granting permissions to [{0}]. {1}\r\n", tgtDir, s));
			}
			string[] ss = Directory.GetFiles(srcDir);
			for (int i = 0; i < ss.Length; i++)
			{
				string tgtFile = Path.Combine(tgtDir, Path.GetFileName(ss[i]));
				File.Copy(ss[i], tgtFile, true);
			}
			ss = Directory.GetDirectories(srcDir);
			if (ss != null && ss.Length > 0)
			{
				for (int i = 0; i < ss.Length; i++)
				{
					string s0 = Path.Combine(tgtDir, Path.GetFileName(ss[i]));
					copyFolder(ss[i], s0, errs);
				}
			}
		}
		private static void installHtmlEdiotr(Form caller)
		{
			try
			{
				string srcDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DistributeIIS");
				if (Directory.Exists(srcDir))
				{
					string newFile = Path.Combine(srcDir, "new.txt");
					if (File.Exists(newFile))
					{
						bool bErr;
						string localhostDir = IisUtility.FindLocalRootWebPath(caller, out bErr);
						if (string.IsNullOrEmpty(localhostDir))
						{
							MessageBox.Show(caller, "Cannot get physical path for web root.", "Checking web root", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else if (!Directory.Exists(localhostDir))
						{
							MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Folder {0} does not exist", srcDir), "Initialize Visual Programming", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							if (MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Visual Web Page Editor requires that full control permission is granted to Everyone for folder {0}. \r\n\r\nDo you want to allow it?", localhostDir), "Visual Web Page Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
							{
								string e;
								StringBuilder sbErr = new StringBuilder();
								e = VPLUtil.GrantFileFullPermission(localhostDir);
								if (!string.IsNullOrEmpty(e))
								{
									sbErr.Append(string.Format(CultureInfo.InvariantCulture, "Error granting permissions for [{0}]. {1}\r\n", localhostDir, e));
								}
								else
								{
									string tgt;
									string[] ss = Directory.GetFiles(srcDir);
									for (int i = 0; i < ss.Length; i++)
									{
										string sn = Path.GetFileName(ss[i]);
										if (string.Compare(sn, "new.txt", StringComparison.OrdinalIgnoreCase) != 0)
										{
											tgt = Path.Combine(localhostDir, sn);
											File.Copy(ss[i], tgt, true);
											e = VPLUtil.GrantFileFullPermission(tgt);
											if (!string.IsNullOrEmpty(e))
											{
												sbErr.Append(string.Format(CultureInfo.InvariantCulture, "Error granting permissions for [{0}]. {1}\r\n", tgt, e));
											}
										}
									}
									ss = Directory.GetDirectories(srcDir);
									for (int i = 0; i < ss.Length; i++)
									{
										tgt = Path.Combine(localhostDir, Path.GetFileName(ss[i]));
										copyFolder(ss[i], tgt, sbErr);
									}
								}
								if (sbErr.Length > 0)
								{
									MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error setting up Visual Web Editor.\r\n {0}", sbErr.ToString()), "Initialize Visual Programming", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
								else
								{
									File.Delete(newFile);
									DesignUtil.EnableHtmlEditor();
								}
							}
						}
					}
					else
					{
						DesignUtil.EnableHtmlEditor();
					}
				}
				else
				{
					MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Folder {0} does not exist", srcDir), "Initialize Visual Programming", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(caller, err.Message, "Initialize Visual Programming", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		public static IWebHost CreateHtmlHost(ILimnorDesignerLoader loader, Form caller)
		{
			IWebHost host = null;
			Encoding encode = Encoding.Unicode;
			string docType = null;
			VirtualWebDir webSite = WebHost.GetWebSite(loader.Project, caller);
			if (webSite != null)
			{
				installHtmlEdiotr(caller);
				if (!loader.Project.WebFolderGranted)
				{
					string err = VPLUtil.GrantFolderFullPermission(webSite.PhysicalDirectory);
					if (string.IsNullOrEmpty(err))
					{
						loader.Project.SetWebFolderGranted();
					}
					else
					{
						MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error granting permissions to [{0}]. {1}", webSite.PhysicalDirectory, err), "Web Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				WebPage wpage = loader.RootObject as WebPage;
				bool openWebTab = true;
				string hf = WebHost.GetHtmlFile(loader.ComponentFilePath); //saved HTML file
				string editFile = Path.Combine(webSite.PhysicalDirectory, Path.GetFileName(hf)); //in WebFiles
				//create editFile contents
				if (File.Exists(hf))
				{
					try
					{
						//set editing file to the doc type for editing (IE requirement)
						//for Chrome, try not to do it
						string line;
						StringBuilder sbBeforeHtml = new StringBuilder();
						bool gotHtml = false;
						StreamReader sr = new StreamReader(hf, true);
						encode = sr.CurrentEncoding;
						while (!sr.EndOfStream)
						{
							line = sr.ReadLine();
							if (gotHtml)
							{
							}
							else
							{
								if (!string.IsNullOrEmpty(line))
								{
									int pos = line.IndexOf("<html", StringComparison.OrdinalIgnoreCase);
									if (pos >= 0)
									{
										if (pos > 0)
										{
											sbBeforeHtml.Append(line.Substring(0, pos));
										}
										docType = sbBeforeHtml.ToString();
										gotHtml = true;
										break; //for Chrome
									}
									else
									{
										sbBeforeHtml.Append(line);
										sbBeforeHtml.Append("\r\n");
									}
								}
							}
						}
						sr.Close();
					}
					catch (Exception err)
					{
						MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error copying [{0}] to [{1}]. {2}", hf, editFile, err.Message), "Load HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
						openWebTab = false;
					}
					//for Chrome
					if (openWebTab)
					{
						try
						{
							File.Copy(hf, editFile, true);
						}
						catch (Exception err)
						{
							MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error copying from [{0}] to [{1}]. {2}.", hf, editFile, err.Message), "Load HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
							openWebTab = false;
						}
					}
					if (openWebTab)
					{
						int p0 = hf.LastIndexOf('.');
						if (p0 > 0)
						{
							string css = string.Format(CultureInfo.InvariantCulture, "{0}.css", hf.Substring(0, p0));
							if (File.Exists(css))
							{
								string tgt = Path.Combine(webSite.PhysicalDirectory, Path.GetFileName(css));
								try
								{
									File.Copy(css, tgt, true);
								}
								catch (Exception err)
								{
									MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error copying from [{0}] to [{1}]. {2}.", css, tgt, err.Message), "Load HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
									openWebTab = false;
								}
							}
						}
					}
				}
				else
				{
					try
					{
						StreamWriter sw = new StreamWriter(editFile);
						sw.Write(Resources.emptyPageEdit);
						sw.Close();
					}
					catch (Exception err)
					{
						MessageBox.Show(caller, string.Format(CultureInfo.InvariantCulture, "Error creating empty html file for editing: [{0}]. {1}", editFile, err.Message), "Load HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
						openWebTab = false;
					}
				}
				if (openWebTab)
				{
					try
					{
						CopyJsLib(webSite.PhysicalDirectory);
						CopyJsColor(webSite.PhysicalDirectory);
					}
					catch (Exception err)
					{
						MathNode.Log(err, "Error preparing html file for editing: [{0}]", editFile);
						openWebTab = false;
					}
					bool bErr;
					string rootPath = IisUtility.FindLocalRootWebPath(caller, out bErr);
					if (string.IsNullOrEmpty(rootPath))
					{
						MathNode.Log("Cannot get physical path for web root.");
					}
					else
					{
						string libjs = Path.Combine(rootPath, "libjs");
						if (!Directory.Exists(libjs))
						{
							try
							{
								Directory.CreateDirectory(libjs);
							}
							catch (Exception err2)
							{
								MathNode.Log(err2, "Error creating web folder: [{0}]", libjs);
							}
						}
						string jsfile = Path.Combine(libjs, "htmlEditorClient.js");
						if (!File.Exists(jsfile))
						{
							try
							{
								StreamWriter sw = new StreamWriter(jsfile, false, Encoding.ASCII);
								sw.Write(Resources.htmleditorClient);
								sw.Close();
							}
							catch (Exception err3)
							{
								MathNode.Log(err3, "Error creating file: [{0}]", jsfile);
							}
						}
					}
				}
				if (openWebTab)
				{
#if USECEF
					if (IntPtr.Size == 4)
					{
						WebHostX86 hx86 = new WebHostX86();
						host = hx86;
						hx86.LoadWebHost(loader.Project.ProjectFolder, webSite.PhysicalDirectory, webSite.WebName, editFile, encode, docType, wpage);
					}
					else
					{
					}
					//host.SetHtmlFile(editFile, webSite.WebName);
#else
					host = new WebHost(loader.Project.ProjectFolder, webSite.PhysicalDirectory, webSite.WebName, editFile, encode, docType, wpage);
					host.SetEncode(encode);
					if (!string.IsNullOrEmpty(docType))
					{
						host.SetDocType(docType);
					}
#endif
					wpage.RefreshWebDisplay();
				}
			}
			return host;
		}
		#endregion
		#region Methods
		protected override void OnDragOver(DragEventArgs drgevent)
		{
			drgevent.Effect = DragDropEffects.None;
		}
		public void LoadIntoControl()
		{
			_page.RemovePageCache();
			webBrowser1.Url = new Uri(string.Format(CultureInfo.InvariantCulture, "http://localhost/ideloader.html?{0}", _htmlUrl));
		}
		public bool EditorReady()
		{
			object v = webBrowser1.Document.InvokeScript("vheLoaded");
			if (v != null)
			{
				if (Convert.ToBoolean(v))
				{
					return true;
				}
			}
			return false;
		}
		public bool checkHtmlChange()
		{
			if (_htmlChanged)
				return true;
			object v = webBrowser1.Document.InvokeScript("pageModified");
			if (v != null)
			{
				if (Convert.ToBoolean(v))
				{
					_htmlChanged = true;
				}
			}
			return _htmlChanged;
		}
		public void ApplyTextBoxChanges()
		{
			webBrowser1.Document.InvokeScript("onBeforeIDErun");
		}
		private bool _saving;
		private Timer _timerX = null;
		public void Save()
		{
			if (_editorLoaded && _timerX == null)
			{
				_saving = true;
#if HTMLDEBUG
				webBrowser1.Document.InvokeScript("setDebug", new object[] { true });
#endif
				webBrowser1.Document.InvokeScript("saveAndFinish", new object[] { Path.GetDirectoryName(_htmlFile) });
				_htmlChanged = false;
			}
		}
		public void ResetSavingFlag()
		{
			_saving = false;
		}
		public void CopyToProjectFolder()
		{
			_timerX_Tick(null, EventArgs.Empty);
		}
		void _timerX_Tick(object sender, EventArgs e)
		{
			if (_timerX != null)
			{
				_timerX.Enabled = false;
				_timerX = null;
			}
			string src = string.Empty;
			string tgt = string.Empty;
			try
			{
				src = _htmlFile;
				tgt = Path.Combine(_projectFolder, Path.GetFileName(_htmlFile));
				File.Copy(src, tgt, true);
				src = Path.Combine(Path.GetDirectoryName(_htmlFile), string.Format(CultureInfo.InvariantCulture, "{0}.css", Path.GetFileNameWithoutExtension(_htmlFile)));
				if (File.Exists(src))
				{
					tgt = Path.Combine(_projectFolder, Path.GetFileName(src));
					File.Copy(src, tgt, true);
				}
			}
			catch (Exception err)
			{
				MathNode.LogError(string.Format(CultureInfo.InvariantCulture, "Error copying from [{0}] to [{1}]. {2}", src, tgt, err.Message));
			}
			int p = _htmlFile.LastIndexOf('.');
			if (p > 0)
			{
				src = string.Format(CultureInfo.InvariantCulture, "{0}.css", _htmlFile.Substring(0, p));
				if (File.Exists(src))
				{
					tgt = Path.Combine(_projectFolder, Path.GetFileName(src));
					try
					{
						File.Copy(src, tgt, true);
					}
					catch (Exception err)
					{
						MathNode.LogError(string.Format(CultureInfo.InvariantCulture, "Error copying from [{0}] to [{1}]. {2}", src, tgt, err.Message));
					}
				}
			}
		}
		public void SelectHtmlElementByGuid(Guid guid)
		{
			webBrowser1.Document.InvokeScript("setSelectedObject", new object[] { HtmlElement_Base.GetGuidString(guid) });
		}
		public object InvoleJs(string cmd, params object[] ps)
		{
			return webBrowser1.Document.InvokeScript(cmd, ps);
		}
		public Bitmap ExportHtmlImage()
		{
			return null;
		}
		public void SetEncode(Encoding encode)
		{
			_encoding = encode;
		}
		public void SetDocType(string docType)
		{
			_docType = docType;
		}
		public void Reload()
		{
		}
		public void StartEditor()
		{
			if (_editorStarted)
			{
				if (_page != null)
				{
					_page.RefreshHtmlElementSelection();
				}
			}
			else
			{
				if (webBrowser1.Document != null)
				{
					_editorStarted = true;
					_editorLoaded = true;
				}
			}
		}
		public void ExecuteJs(string js)
		{
			string func = string.Format(CultureInfo.InvariantCulture, "func{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			StringBuilder sb = new StringBuilder("javascript:function ");
			sb.Append(func);
			sb.Append("(){");
			sb.Append(js);
			sb.Append("}");
			sb.Append(func);
			sb.Append("();");
			js = sb.ToString();
			webBrowser1.Navigate(js);
		}
		public void RemoveEditor()
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("removePageEditor");
					_editorStarted = false;
				}
			}
		}
		public string CreateOrGetIdForCurrentElement(string newGuid)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("createOrGetId", new object[] { newGuid });
					return v as string;
				}
			}
			return string.Empty;
		}
		public void PasteToHtmlInput(string txt, int selStart, int selEnd)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("pasteToHtmlInput", new object[] { txt, selStart, selEnd });
				}
			}
		}
		public bool SetGuidById(string id, string guid)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("setGuidById", new object[] { id, guid });
					if (v != null)
					{
						return Convert.ToBoolean(v);
					}
				}
			}
			return false;
		}
		public string getTagNameById(string id)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("getTagNameById", new object[] { id });
					string tag = v as string;
					if (tag == null)
						return string.Empty;
					return tag.ToLower();
				}
			}
			return string.Empty;
		}
		public string getIdByGuid(string guid)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("getIdByGuid", new object[] { guid });
					string id = v as string;
					if (id == null)
						return string.Empty;
					return id;
				}
			}
			return string.Empty;
		}
		public Dictionary<string, string> getIdList()
		{
			Dictionary<string, string> l = new Dictionary<string, string>();
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("getIdList");
					string ids = v as string;
					if (!string.IsNullOrEmpty(ids))
					{
						string[] ss = ids.Split(',');
						for (int i = 0; i < ss.Length; i++)
						{
							string[] kv = ss[i].Split(':');
							l.Add(kv[0], kv[1]);
						}
					}
				}
			}
			return l;
		}
		public string GetMaps()
		{
			string ret = null;
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("getMaps");
					ret = v as string;
				}
			}
			return ret;
		}
		public string GetAreas(string mapId)
		{
			string ret = null;
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("getAreas", new object[] { mapId });
					ret = v as string;
				}
			}
			return ret;
		}
		public string CreateNewMap(string guid)
		{
			string ret = null;
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("createNewMap", new object[] { guid });
					ret = v as string;
				}
			}
			return ret;
		}
		public void UpdateMapAreas(string mapId, string areas)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("setMapAreas", new object[] { mapId, areas });
				}
			}
		}
		public void SetUseMap(string imgId, string mapId)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("setUseMap", new object[] { imgId, mapId });
				}
			}
		}
		public void SetWebProperty(string name, string value)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("setPropertyValue", new object[] { name, value });
				}
			}
		}
		public string AppendArchiveFile(string name, string archiveFilePath)
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					object v = webBrowser1.Document.InvokeScript("appendArchiveFile", new object[] { name, archiveFilePath });
					return v as string;
				}
			}
			return string.Empty;
		}
		public bool hasPageChanged()
		{
			if (webBrowser1.Document != null)
			{
				object v = webBrowser1.Document.InvokeScript("hasPageChanged");
				return Convert.ToBoolean(v);
			}
			return false;
		}
		public void DoCopy()
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("doCopy");
				}
			}
		}
		public void DoPaste()
		{
			if (_editorStarted)
			{
				if (webBrowser1.Document != null)
				{
					webBrowser1.Document.InvokeScript("doPaste");
				}
			}
		}
		#endregion
		#region Properties
		public bool IsSaving
		{
			get
			{
				return _saving;
			}
		}
		public bool EditorLoaded
		{
			get
			{
				return _editorLoaded;
			}
		}
		public string HtmlUrl
		{
			get
			{
				return _htmlUrl;
			}
		}
		public bool EditorStarted
		{
			get
			{
				return _editorStarted;
			}
		}
		public string PhysicalFolder
		{
			get
			{
				return _webPhysicalFolder;
			}
		}
		[Browsable(false)]
		public bool HtmlChanged
		{
			get
			{
				return _htmlChanged;
			}
		}
		public bool DocLoaded
		{
			get
			{
				if (webBrowser1.Document != null)
				{
					if (webBrowser1.Document.DomDocument != null)
					{
						return true;
					}
				}
				return false;
			}
		}
		public string HtmlFile
		{
			get
			{
				return _htmlFile;
			}
		}
		#endregion
		#region ISkipWrite Members

		public bool SkipSerialize
		{
			get { return true; }
		}

		#endregion
	}
}
