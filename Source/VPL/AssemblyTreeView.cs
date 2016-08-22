/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Library
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Specialized;
using System.Globalization;

namespace VPL
{
	public class EventArgObjectSelected : EventArgs
	{
		private object _selObj;
		public EventArgObjectSelected(object data)
		{
			_selObj = data;
		}
		public object SelectedObject
		{
			get
			{
				return _selObj;
			}
		}
	}
	public class AssemblyTreeView : TreeView
	{
		public event EventHandler TypeSelected;
		private static Dictionary<string,Assembly> manualDlls;
		private Label lblInfo;
		private bool _selectStaticType;
		public AssemblyTreeView()
		{
			lblInfo = new Label();
			this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblInfo.ForeColor = System.Drawing.Color.Blue;
			this.lblInfo.Location = new System.Drawing.Point(107, 288);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(164, 29);
			this.lblInfo.TabIndex = 2;
			this.lblInfo.Text = "Loading ......";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			lblInfo.Visible = false;
			this.Controls.Add(lblInfo);
			manualDlls = new Dictionary<string,Assembly>();
			loadCommonDll("FormComponents.dll");
			loadCommonDll("ProgUtility.dll");
			loadCommonDll("InternetUtility.dll");
			loadCommonDll("SendMail.dll");
			loadCommonDll("MathComponent.dll");
			loadCommonDll("WindowsUtility.dll");
		}
		private void loadCommonDll(string filename)
		{
			string dir = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
			string file = System.IO.Path.Combine(dir, filename);
			if (System.IO.File.Exists(file))
			{
				try
				{
					Assembly a = Assembly.LoadFile(file);
					if (a != null)
					{
						manualDlls.Add(a.FullName, a);
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error loading {0}. {1}", filename, e.Message));
				}
			}
		}
		public void SetForStatic()
		{
			_selectStaticType = true;
		}
		public bool IsForStatic
		{
			get
			{
				return _selectStaticType;
			}
		}
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			if (TypeSelected != null)
			{
				TreeNodeAssemblyView node = e.Node as TreeNodeAssemblyView;
				if (node != null)
				{
					TypeSelected(this, new EventArgObjectSelected(node.Data));
				}
			}
		}
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			TreeNodeAssemblyView na = e.Node as TreeNodeAssemblyView;
			if (na != null)
			{
				na.LoadNextLevel();
			}
		}
		public void LoadDLL()
		{
			//NET_40;
#if (NET_40)
            int n0 =0;
            if (n0 == 0)
            {
            }
#endif
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = ".Net DLL|*.Dll";
			dlg.Title = "Select a .Net DLL file";
			if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
			{
				try
				{
					Assembly a = Assembly.LoadFile(dlg.FileName);
					if (manualDlls == null)
					{
						manualDlls = new Dictionary<string,Assembly>();
					}
					if (!manualDlls.ContainsKey(a.FullName))
					{
						manualDlls.Add(a.FullName,a);
						Nodes.Insert(0, new TreeNodeAssembly(a.FullName, a));
						this.SelectedNode = Nodes[0];
					}
					else
					{
						this.SelectedNode = null;
						for (int i = 0; i < Nodes.Count; i++)
						{
							if (Nodes[i].Text == a.FullName)
							{
								this.SelectedNode = Nodes[i];
								break;
							}
						}
						if (this.SelectedNode == null)
						{
							Nodes.Insert(0, new TreeNodeAssembly(a.FullName, a));
							this.SelectedNode = Nodes[0];
						}
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(this.FindForm(), VPLException.FormExceptionText(e), "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		private static int compareStringNoCase(string s1, string s2)
		{
			return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
		}
		public void LoadGac(params string[] namespaceAndType)
		{
			string _namespace = null;
			string _typename = null;
			if (namespaceAndType != null)
			{
				if (namespaceAndType.Length > 0)
				{
					_namespace = namespaceAndType[0];
					if (namespaceAndType.Length > 1)
					{
						_typename = namespaceAndType[1];
					}
				}
			}
			List<TreeNodeAssembly> traFound = new List<TreeNodeAssembly>();
			lblInfo.Location = new System.Drawing.Point(
				(this.Width - lblInfo.Width) / 2, (this.Height - lblInfo.Height) / 2);
			lblInfo.Visible = true;
			lblInfo.Refresh();
			List<string> ss = new List<string>();

			AssemblyCacheEnum ace = new AssemblyCacheEnum(null);
			string s;
			s = ace.GetNextAssembly();
			while (!string.IsNullOrEmpty(s))
			{
				ss.Add(s);
				s = ace.GetNextAssembly();
			}
			ss.Sort(new Comparison<string>(compareStringNoCase));
			if (manualDlls != null)
			{
				foreach (KeyValuePair<string, Assembly> s0 in manualDlls)
				{
					Nodes.Add(new TreeNodeAssembly(s0.Key, s0.Value));
				}
			}
			for (int i = 0; i < ss.Count; i++)
			{
				TreeNodeAssembly tra = new TreeNodeAssembly(ss[i]);
				Nodes.Add(tra);
				if (!string.IsNullOrEmpty(_namespace))
				{
					if (ss[i].StartsWith(_namespace, StringComparison.Ordinal))
					{
						traFound.Add(tra);
					}
				}
			}
			if (traFound.Count > 0)
			{
				this.SelectedNode = traFound[0];
			}
			if (!string.IsNullOrEmpty(_typename))
			{
				foreach (TreeNodeAssembly tra in traFound)
				{
					tra.Expand();
					bool b = false;
					for (int i = 0; i < tra.Nodes.Count; i++)
					{
						TreeNodeAssemblyType n = tra.Nodes[i] as TreeNodeAssemblyType;
						if (n != null)
						{
							if (string.CompareOrdinal(n.ExportedType.FullName, _typename) == 0)
							{
								this.SelectedNode = n;
								b = true;
								break;
							}
						}
						if (b)
						{
							break;
						}
					}
				}
			}
			lblInfo.Visible = false;
		}
	}
	abstract class TreeNodeAssemblyViewerLoader : TreeNode
	{
		public TreeNodeAssemblyViewerLoader()
		{
		}
		public abstract void LoadNextLevel(TreeNodeAssemblyView parentNode);
	}
	abstract class TreeNodeAssemblyView : TreeNode
	{
		private bool _nextLevelLoaded;
		public TreeNodeAssemblyView()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = GetLoaders();
			if (loaders != null)
			{
				foreach (TreeNodeAssemblyViewerLoader l in loaders)
				{
					this.Nodes.Add(l);
				}
			}
		}
		public abstract object Data { get; }
		public abstract List<TreeNodeAssemblyViewerLoader> GetLoaders();		
		public void LoadNextLevel()
		{
			if (_nextLevelLoaded)
				return;
			_nextLevelLoaded = true;
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				TreeNodeAssemblyViewerLoader l = Nodes[i] as TreeNodeAssemblyViewerLoader;
				if (l != null)
				{
					loaders.Add(l);
				}
			}
			if (loaders.Count > 0)
			{
				foreach (TreeNodeAssemblyViewerLoader l in loaders)
				{
					l.Remove();
					l.LoadNextLevel(this);
				}
			}
		}
	}
	class TreeNodeAssembly : TreeNodeAssemblyView
	{
		private string _name;
		private Assembly _assembly;
		public TreeNodeAssembly(string name, Assembly a)
			: base()
		{
			_assembly = a;
			_name = name;
			Text = name;
		}
		public TreeNodeAssembly(string name)
			: base()
		{
			_assembly = null;
			_name = name;
			Text = name;
		}
		public Assembly LoadedAssembly
		{
			get
			{
				if (_assembly == null)
				{
					_assembly = Assembly.Load(this.Text);
				}
				return _assembly;
			}
		}
		public override object Data { get { return _name; } }
		class TreeNodeAssemblyLoader : TreeNodeAssemblyViewerLoader
		{
			public TreeNodeAssemblyLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeAssemblyView parentNode)
			{
				TreeNodeAssembly tna = parentNode as TreeNodeAssembly;
				SortedList<string, TreeNode> nodes = new SortedList<string, TreeNode>();
				Assembly a = tna.LoadedAssembly;
				Type[] tps = a.GetExportedTypes();
				for (int i = 0; i < tps.Length; i++)
				{
					bool b = tps[i].IsPublic;
					if (b)
					{
						TreeNodeAssemblyType n;
						if (tps[i].IsEnum)
						{
							n = new TreeNodeEnumType(tps[i]);
						}
						else
						{
							n = new TreeNodeAssemblyType(tps[i]);
						}
						if (nodes.ContainsKey(n.Text))
						{
							uint mx = 2;
							while (true)
							{
								string nm = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", n, Text, mx.ToString("x", CultureInfo.InvariantCulture));
								if (nodes.ContainsKey(nm))
								{
									mx++;
								}
								else
								{
									nodes.Add(nm, n);
									break;
								}
							}
						}
						else
						{
							nodes.Add(n.Text, n);
						}
					}
				}
				IEnumerator<KeyValuePair<string, TreeNode>> ie = nodes.GetEnumerator();
				while (ie.MoveNext())
				{
					parentNode.Nodes.Add(ie.Current.Value);
				}
			}
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			loaders.Add(new TreeNodeAssemblyLoader());
			return loaders;
		}
	}
	class TreeNodeAssemblyType : TreeNodeAssemblyView
	{
		Type _type;
		public TreeNodeAssemblyType(Type type)
			: base()
		{
			_type = type;
			Text = _type.AssemblyQualifiedName;
		}
		public override object Data { get { return _type; } }
		public Type ExportedType
		{
			get
			{
				return _type;
			}
		}
		class TreeNodeTypeLoader : TreeNodeAssemblyViewerLoader
		{
			public TreeNodeTypeLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeAssemblyView parentNode)
			{
				TreeNodeAssemblyType at = parentNode as TreeNodeAssemblyType;
				parentNode.Nodes.Add(new TreeNodeProperties(at.ExportedType));
				parentNode.Nodes.Add(new TreeNodeMethods(at.ExportedType));
				parentNode.Nodes.Add(new TreeNodeEvents(at.ExportedType));
			}
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			loaders.Add(new TreeNodeTypeLoader());
			return loaders;
		}
	}
	class TreeNodeEnumType : TreeNodeAssemblyType
	{
		public TreeNodeEnumType(Type type)
			: base(type)
		{
		}
		class CLoader : TreeNodeAssemblyViewerLoader
		{
			class TreeNodeEnumValue : TreeNodeEnumType
			{
				object _value;
				public TreeNodeEnumValue(Type type, object value)
					: base(type)
				{
					_value = value;
					Text = _value.ToString();
				}
				public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
				{
					return null;
				}
			}
			public override void LoadNextLevel(TreeNodeAssemblyView parentNode)
			{
				TreeNodeAssemblyType at = parentNode as TreeNodeAssemblyType;
				Type t = at.ExportedType;
				Array ss = Enum.GetValues(t);
				if (ss != null)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						parentNode.Nodes.Add(new TreeNodeEnumValue(t, ss.GetValue(i)));
					}
				}
			}
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			loaders.Add(new CLoader());
			return loaders;
		}
	}
	class TreeNodeProperty : TreeNodeAssemblyView
	{
		PropertyInfo _pif;
		public TreeNodeProperty(PropertyInfo pif)
			: base()
		{
			_pif = pif;
			Text = pif.Name;
		}
		public override object Data { get { return _pif; } }
		public PropertyInfo Property
		{
			get
			{
				return _pif;
			}
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			return null;
		}
	}
	class TreeNodeProperties : TreeNodeAssemblyType
	{
		public TreeNodeProperties(Type type)
			: base(type)
		{
			Text = "Properties";
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			loaders.Add(new Cloader());
			return loaders;
		}
		class Cloader : TreeNodeAssemblyViewerLoader
		{
			public Cloader()
			{
			}

			public override void LoadNextLevel(TreeNodeAssemblyView parentNode)
			{
				TreeNodeAssemblyType at = parentNode as TreeNodeAssemblyType;
				Type t = at.ExportedType;
				PropertyInfo[] pifs = t.GetProperties();
				for (int i = 0; i < pifs.Length; i++)
				{
					if (!pifs[i].IsSpecialName)
					{
						parentNode.Nodes.Add(new TreeNodeProperty(pifs[i]));
					}
				}
			}
		}
	}
	class TreeNodeMethod : TreeNodeAssemblyView
	{
		MethodInfo _pif;
		public TreeNodeMethod(MethodInfo pif)
			: base()
		{
			_pif = pif;
			Text = pif.Name;
		}
		public override object Data { get { return _pif; } }
		public MethodInfo Method
		{
			get
			{
				return _pif;
			}
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			return null;
		}
	}
	class TreeNodeMethods : TreeNodeAssemblyType
	{
		public TreeNodeMethods(Type type)
			: base(type)
		{
			Text = "Methods";
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			loaders.Add(new Cloader());
			return loaders;
		}
		class Cloader : TreeNodeAssemblyViewerLoader
		{
			public Cloader()
			{
			}

			public override void LoadNextLevel(TreeNodeAssemblyView parentNode)
			{
				TreeNodeAssemblyType at = parentNode as TreeNodeAssemblyType;
				Type t = at.ExportedType;
				MethodInfo[] pifs = t.GetMethods();
				for (int i = 0; i < pifs.Length; i++)
				{
					if (!pifs[i].IsSpecialName)
					{
						parentNode.Nodes.Add(new TreeNodeMethod(pifs[i]));
					}
				}
			}
		}
	}
	class TreeNodeEvent : TreeNodeAssemblyView
	{
		EventInfo _pif;
		public TreeNodeEvent(EventInfo pif)
			: base()
		{
			_pif = pif;
			Text = pif.Name;
		}
		public override object Data { get { return _pif; } }
		public EventInfo Event
		{
			get
			{
				return _pif;
			}
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			return null;
		}
	}
	class TreeNodeEvents : TreeNodeAssemblyType
	{
		public TreeNodeEvents(Type type)
			: base(type)
		{
			Text = "Events";
		}
		public override List<TreeNodeAssemblyViewerLoader> GetLoaders()
		{
			List<TreeNodeAssemblyViewerLoader> loaders = new List<TreeNodeAssemblyViewerLoader>();
			loaders.Add(new Cloader());
			return loaders;
		}
		class Cloader : TreeNodeAssemblyViewerLoader
		{
			public Cloader()
			{
			}
			public override void LoadNextLevel(TreeNodeAssemblyView parentNode)
			{
				TreeNodeAssemblyType at = parentNode as TreeNodeAssemblyType;
				Type t = at.ExportedType;
				EventInfo[] pifs = t.GetEvents();
				for (int i = 0; i < pifs.Length; i++)
				{
					if (!pifs[i].IsSpecialName)
					{
						parentNode.Nodes.Add(new TreeNodeEvent(pifs[i]));
					}
				}
			}
		}
	}
}
