/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel.Design;
using VOB;
using System.ComponentModel;
using System.IO;
using MathExp;
using System.Windows.Forms;
using MathExp.RaisTypes;
using System.Reflection;
using VPL;
using LimnorDesigner;
using VSPrj;
using System.Globalization;
using XmlUtility;
using Limnor.WebBuilder;
using System.Collections.Specialized;

namespace SolutionMan
{
	public class NodeObjectComponent : NodeData
	{
		#region fields and constructors
		private ILimnorDesignPane _xHolder = null;
		private ProjectNodeData _prj;
		private ClassData _classData;
		public NodeObjectComponent(ProjectNodeData prj, ClassData data, TreeNodeCollection parent)
		{
			_classData = data;
			_prj = prj;
			if (IsWebPage)
			{
				string nm = string.Format(CultureInfo.InvariantCulture, "{0}.html", Path.GetFileNameWithoutExtension(_classData.ComponentFile));
				_classData.Rename(nm);
				SetName0(nm);
			}
			else
			{
				if (string.IsNullOrEmpty(_classData.ComponentName))
				{
					_classData.Rename(Path.GetFileNameWithoutExtension(_classData.ComponentFile));
				}
				SetName0(_classData.ComponentName);
			}
		}
		#endregion
		#region Browsable Properties
		[Description("Filename for the object")]
		public string Filename
		{
			get
			{
				return Path.GetFileNameWithoutExtension(_classData.ComponentFile);
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string s = value.Trim();
					if (s.Length > 0)
					{
						bool isWebPage = _classData.ComponentType.IsAssignableFrom(typeof(WebPage));
						if (isWebPage)
						{
							Dictionary<UInt32, ILimnorDesignPane> designPanes = _prj.Project.GetTypedDataList<ILimnorDesignPane>();
							if (designPanes != null)
							{
								foreach (ILimnorDesignPane pn in designPanes.Values)
								{
									if (pn.Loader.ClassID == _classData.ComponentId)
									{
										MessageBox.Show("Please close the designers for this file before renaming it.", "Rename", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										return;
									}
								}
							}
						}
						s = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", s, NodeDataSolution.FILE_EXT_CLASS);
						string newFile = Path.Combine(Path.GetDirectoryName(_classData.ComponentFile), s);
						if (string.Compare(newFile, _classData.ComponentFile, StringComparison.OrdinalIgnoreCase) != 0)
						{
							if (File.Exists(newFile))
							{
								throw new DesignerException("Cannot rename file. The new file exists. [{0}]", newFile);
							}
							string htmlFile = null;
							string cssFile = null;
							bool bOK = !isWebPage;
							if (isWebPage)
							{
								try
								{
									//copy to new HTML and CSS files
									htmlFile = Path.Combine(Path.GetDirectoryName(_classData.ComponentFile), string.Format(CultureInfo.InvariantCulture, "{0}_design.html", Path.GetFileNameWithoutExtension(_classData.ComponentFile)));
									cssFile = Path.Combine(Path.GetDirectoryName(_classData.ComponentFile), string.Format(CultureInfo.InvariantCulture, "{0}_design.css", Path.GetFileNameWithoutExtension(_classData.ComponentFile)));
									if (File.Exists(htmlFile))
									{
										string newHtmlFile = Path.Combine(Path.GetDirectoryName(_classData.ComponentFile), string.Format(CultureInfo.InvariantCulture, "{0}_design.html", Path.GetFileNameWithoutExtension(newFile)));
										if (File.Exists(newHtmlFile))
										{
											throw new DesignerException("Cannot rename file to [{0}]. File exists: {1}. You may delete {1} if it is not used.", newFile, newHtmlFile);
										}
										else
										{
											string newCssFile = Path.Combine(Path.GetDirectoryName(_classData.ComponentFile), string.Format(CultureInfo.InvariantCulture, "{0}_design.css", Path.GetFileNameWithoutExtension(newFile)));
											if (File.Exists(newCssFile))
											{
												throw new DesignerException("Cannot rename file to [{0}]. File exists: {1}. You may delete {1} if it is not used.", newFile, newCssFile);
											}
											else
											{
												File.Copy(htmlFile, newHtmlFile, true);
												if (File.Exists(cssFile))
												{
													File.Copy(cssFile, newCssFile, true);
												}
												StreamReader sr = new StreamReader(newHtmlFile, true);
												Encoding en = sr.CurrentEncoding;
												string htmlContents = sr.ReadToEnd();
												sr.Close();
												int pos = htmlContents.IndexOf(Path.GetFileName(cssFile), StringComparison.OrdinalIgnoreCase);
												if (pos >= 0)
												{
													htmlContents = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", htmlContents.Substring(0, pos), Path.GetFileName(newCssFile), htmlContents.Substring(pos + Path.GetFileName(cssFile).Length));
													StreamWriter sw = new StreamWriter(newHtmlFile, false, en);
													sw.Write(htmlContents);
													sw.Close();
												}
												bOK = true;
											}
										}
									}
									else
									{
										bOK = true;
									}
								}
								catch (Exception err)
								{
									throw new DesignerException(err, "Cannot rename file to [{0}]. {1}", newFile, err.Message);
								}
							}
							if (bOK)
							{
								File.Move(_classData.ComponentFile, newFile);
								_prj.RenameClassFile(_classData.ComponentFile, newFile);
								_classData.SetFilename(newFile);
								_prj.Save();
								if (isWebPage)
								{
									if (File.Exists(htmlFile))
									{
										File.Delete(htmlFile);
									}
									if (File.Exists(cssFile))
									{
										File.Delete(cssFile);
									}
								}
							}
						}
					}
				}
			}
		}
		[Description("Full file path for the object")]
		public string Filepath
		{
			get
			{
				return _classData.ComponentFile;
			}
		}
		#endregion
		#region Non-Browsable Properties
		[Browsable(false)]
		public ClassData Class
		{
			get
			{
				return _classData;
			}
		}
		[Browsable(false)]
		public bool IsWebPage
		{
			get
			{
				if (_classData != null && _classData.ComponentType != null)
				{
					return (_classData.ComponentType.GetInterface("IWebPage") != null);
				}
				return false;
			}
		}
		[ParenthesizePropertyName]
		[Description("Class name.")]
		[Browsable(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					string s = value.Trim();
					if (VOB.VobUtil.IsGoodVarName(s))
					{
						base.Name = s;
						_classData.Rename(s);
						Dirty = true;
					}
				}
			}
		}
		[Browsable(false)]
		public ProjectNodeData OwnerProject
		{
			get
			{
				return _prj;
			}
			set
			{
				_prj = value;
			}
		}
		[Browsable(false)]
		public string Namespace
		{
			get
			{
				return _prj.Namespace;
			}
			set
			{
				_prj.Namespace = value;
			}
		}
		[Browsable(false)]
		public ILimnorDesignPane Holder
		{
			get
			{
				return _xHolder;
			}
			set
			{
				_xHolder = value;
			}
		}
		[Browsable(false)]
		public override bool Dirty
		{
			get
			{
				if (_xHolder != null)
				{
					IChangeControl icc = _xHolder.Loader as IChangeControl;
					if (icc != null && icc.Dirty)
						return true;
				}
				return base.Dirty;
			}
			set
			{
				if (!value)
				{
					if (_xHolder != null)
					{
						IChangeControl icc = _xHolder.Loader as IChangeControl;
						if (icc != null)
						{
							icc.Dirty = value;
						}
					}
				}
				base.Dirty = value;
			}
		}
		#endregion
		#region Methods
		public void Save()
		{
			Holder.SaveAll();
			_classData.Save();
			Dirty = false;
		}
		public override void ResetModified()
		{
			base.Dirty = false;
			if (_xHolder != null)
			{
				IChangeControl icc = _xHolder.Loader as IChangeControl;
				if (icc != null)
					icc.ResetModified();
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} from {1}", Name, Filepath);
		}
		#endregion
	}

	public class ComponentNode : TreeNodeClass
	{
		public ComponentNode(ClassData classData, ProjectNodeData prj)
		{
			Obj = CreateDataObject(prj, classData, this.Nodes);
			Obj.OnNameChanged += new EventHandler(Obj_OnNameChanged);
			Obj.OnNameChanging += new EventHandler(Obj_OnNameChanging);
			this.ImageIndex = SolutionTree.IMG_COMP;
			this.SelectedImageIndex = SolutionTree.IMG_COMP;
			Text = classData.ComponentName;
		}

		void Obj_OnNameChanging(object sender, EventArgs e)
		{
			EventArgNameChange en = (EventArgNameChange)e;
			TreeNode nd = this.Parent;
			if (nd != null)
			{
				for (int i = 0; i < nd.Nodes.Count; i++)
				{
					if (i != this.Index)
					{
						if (string.CompareOrdinal(nd.Nodes[i].Text, en.NewName) == 0)
						{
							MessageBox.Show(this.TreeView != null ? this.TreeView.FindForm() : null, "The component name is already used", "Reanme", MessageBoxButtons.OK, MessageBoxIcon.Error);
							en.Cancel = true;
						}
					}
				}
				if (!en.Cancel)
				{
					NameCreation nc = new NameCreation();
					if (!nc.IsValidName(en.NewName))
					{
						MessageBox.Show(this.TreeView != null ? this.TreeView.FindForm() : null, "Invalid component name", "Reanme", MessageBoxButtons.OK, MessageBoxIcon.Error);
						en.Cancel = true;
					}
				}
				if (!en.Cancel)
				{
					string sn = en.NewName.ToLowerInvariant();
					if (WebPageCompilerUtility.IsReservedPhpWord(sn))
					{
						MessageBox.Show(this.TreeView != null ? this.TreeView.FindForm() : null, "Invalid component name. It is a reserved word.", "Reanme", MessageBoxButtons.OK, MessageBoxIcon.Error);
						en.Cancel = true;
					}
				}
			}
		}
		void Obj_OnNameChanged(object sender, EventArgs e)
		{
			EventArgNameChange nc = e as EventArgNameChange;
			if (nc != null)
			{
				Text = nc.NewName;
			}
			else
			{
				NodeObjectComponent noc = this.PropertyObject as NodeObjectComponent;
				Text = noc.Class.ComponentName;
			}
		}
		protected virtual NodeObjectComponent CreateDataObject(ProjectNodeData prj, ClassData classData, TreeNodeCollection nodes)
		{
			return new NodeObjectComponent(prj, classData, nodes);
		}

		public override bool CanRemove
		{
			get
			{
				NodeObjectComponent noc = this.PropertyObject as NodeObjectComponent;
				bool b = XmlUtil.GetAttributeBoolDefFalse(noc.Class.ComponentXmlNode, XmlTags.XMLATT_isPermanent);
				if (b)
				{
					return false;
				}
				return true;
			}
		}

		protected override void SetNodeIcon()
		{
		}
	}
}
