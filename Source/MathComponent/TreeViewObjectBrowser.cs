/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MathExp;
using System.ComponentModel;
using System.Reflection;

namespace MathComponent
{
	delegate void fnOnTreeNodeExplorer(TreeNodeObject node);
	class TreeViewObjectBrowser : TreeView
	{
		private Label lblInfo;
		private fnOnTreeNodeExplorer miLoadNextLevel;

		static public int IMG_DEFICON;
		static ImageList _imageList;
		static Dictionary<Type, int> _typeImages;

		public TreeViewObjectBrowser()
		{
			ImageList = ObjectImageList;
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
			//
			miLoadNextLevel = new fnOnTreeNodeExplorer(loadNextLevel);
			//
			this.ImageList = ObjectImageList;
			this.HideSelection = false;
			ShowNodeToolTips = true;
		}
		public static ImageList ObjectImageList
		{
			get
			{
				if (_imageList == null)
				{
					_imageList = new ImageList();
					IMG_DEFICON = _imageList.Images.Add(ResourceA._defaultObject, Color.White);
				}
				return _imageList;
			}
		}
		public static int GetTypeIcon(Type t)
		{
			if (_typeImages == null)
			{
				_typeImages = new Dictionary<Type, int>();
			}
			if (_typeImages.ContainsKey(t))
			{
				return _typeImages[t];
			}
			else
			{
				Image icon = null;
				if (t.IsEnum)
				{
					icon = ResourceA.enumValues;
				}
				else
				{
					TypeCode tc = Type.GetTypeCode(t);
					switch (tc)
					{
						case TypeCode.Boolean:
							icon = ResourceA._bool;
							break;
						case TypeCode.Byte:
							icon = ResourceA._byte;
							break;
						case TypeCode.Char:
							icon = ResourceA._char;
							break;
						case TypeCode.DateTime:
							icon = ResourceA.date;
							break;
						case TypeCode.Decimal:
						case TypeCode.Double:
						case TypeCode.Single:
							icon = ResourceA._decimal;
							break;
						case TypeCode.Int16:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.UInt16:
						case TypeCode.UInt32:
						case TypeCode.UInt64:
							icon = ResourceA._int;
							break;
						case TypeCode.SByte:
							icon = ResourceA._sbyte;
							break;
						case TypeCode.String:
							icon = ResourceA.abc;
							break;
						default:
							if (typeof(Size).Equals(t))
							{
								icon = ResourceA._size;
							}
							else if (typeof(SizeF).Equals(t))
							{
								icon = ResourceA._rect;
							}
							else if (typeof(Point).Equals(t))
							{
								icon = ResourceA._point;
							}
							else if (typeof(PointF).Equals(t))
							{
								icon = ResourceA._point;
							}
							else if (typeof(Rectangle).Equals(t))
							{
								icon = ResourceA._rect;
							}
							else if (typeof(RectangleF).Equals(t))
							{
								icon = ResourceA._rect;
							}
							else if (typeof(Color).Equals(t))
							{
								icon = ResourceA._3color;
							}
							else if (typeof(char).Equals(t))
							{
								icon = ResourceA._char;
							}
							else
							{
								icon = VPL.VPLUtil.GetTypeIcon(t);
							}
							break;
					}
				}
				if (icon != null)
				{
					int n = ObjectImageList.Images.Add(icon, Color.White);
					_typeImages.Add(t, n);
					return n;
				}
			}
			return IMG_DEFICON;
		}
		#region private methods
		private void loadNextLevel(TreeNodeObject tne)
		{
			if (!tne.NextLevelLoaded)
			{
				this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
				lblInfo.Location = new System.Drawing.Point(
				(this.Width - lblInfo.Width) / 2, (this.Height - lblInfo.Height) / 2);
				lblInfo.Visible = true;
				lblInfo.Refresh();
				tne.NextLevelLoaded = true;
				try
				{
					List<TreeNodeLoader> loaders = new List<TreeNodeLoader>();
					for (int i = 0; i < tne.Nodes.Count; i++)
					{
						TreeNodeLoader loader = tne.Nodes[i] as TreeNodeLoader;
						if (loader != null)
						{
							loaders.Add(loader);
						}
					}
					foreach (TreeNodeLoader l in loaders)
					{
						TreeNodeObject p = l.Parent as TreeNodeObject;
						l.Remove();
						if (p != null)
						{
							l.LoadNextLevel(p);
						}
					}
				}
				catch (Exception err)
				{
					MathNode.Log(this.FindForm(), err);
				}
				lblInfo.Visible = false;
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}
		#endregion
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			try
			{
				base.OnBeforeExpand(e);
				TreeNodeObject tne = e.Node as TreeNodeObject;
				if (tne != null)
				{
					ForceLoadNextLevel(tne);
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}
		public void ForceLoadNextLevel(TreeNodeObject tne)
		{
			bool invoked = false;
			if (this.Created)
			{
				Form f = this.FindForm();
				if (f != null)
				{
					if (f.InvokeRequired)
					{
						invoked = true;
						this.Invoke(miLoadNextLevel, tne);
					}
				}
			}
			if (!invoked)
			{
				loadNextLevel(tne);
			}
		}
		public void LoadData(Form form)
		{
			TreeNodeForm tf = new TreeNodeForm(form);
			Nodes.Add(tf);
		}
	}
	abstract class TreeNodeObject : TreeNode
	{
		private bool _nextLevelLoaded;
		public TreeNodeObject()
		{
		}
		public static void LoadNextLevel(TreeNodeObject parentNode, Type type)
		{
			SortedList<string, TreeNodeObject> sl = new SortedList<string, TreeNodeObject>();
			FieldInfo[] fifs = type.GetFields();
			if (fifs != null && fifs.Length > 0)
			{
				for (int i = 0; i < fifs.Length; i++)
				{
					if (!fifs[i].IsSpecialName && !fifs[i].IsStatic)
					{
						TreeNodeField tf = new TreeNodeField(fifs[i]);
						sl.Add(tf.Text, tf);
					}
				}
			}
			PropertyInfo[] pifs = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			if (pifs != null && pifs.Length > 0)
			{
				for (int i = 0; i < pifs.Length; i++)
				{
					if (!pifs[i].IsSpecialName)
					{
						TreeNodeProperty tp = new TreeNodeProperty(pifs[i]);
						sl.Add(tp.Text, tp);
					}
				}
			}
			IEnumerator<KeyValuePair<string, TreeNodeObject>> en = sl.GetEnumerator();
			while (en.MoveNext())
			{
				parentNode.Nodes.Add(en.Current.Value);
			}
		}
		public abstract MathPropertyPointer CreatePointer();
		public void LoadNextLevel()
		{
			if (!_nextLevelLoaded)
			{
				TreeViewObjectBrowser tv = TreeView as TreeViewObjectBrowser;
				if (tv != null)
				{
					tv.ForceLoadNextLevel(this);
				}
			}
		}
		public bool NextLevelLoaded
		{
			get
			{
				return _nextLevelLoaded;
			}
			set
			{
				_nextLevelLoaded = value;
			}
		}
	}
	abstract class TreeNodeLoader : TreeNode
	{
		public TreeNodeLoader()
		{
		}
		public abstract void LoadNextLevel(TreeNodeObject parentNode);
	}
	class TreeNodeForm : TreeNodeObject
	{
		private Form _form;
		public TreeNodeForm(Form form)
		{
			_form = form;
			Text = _form.Name;
			ImageIndex = TreeViewObjectBrowser.GetTypeIcon(typeof(Form));
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		public override MathPropertyPointer CreatePointer()
		{
			MathPropertyPointerForm mp = new MathPropertyPointerForm();
			mp.PropertyName = this.Text;
			return mp;
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}
			private bool onFindInterface(Type t, object v)
			{
				bool bRet = t.Equals(typeof(IComponent));
				return bRet;
			}
			public override void LoadNextLevel(TreeNodeObject parentNode)
			{
				TreeNodeForm tf = (TreeNodeForm)parentNode;
				for (int i = 0; i < tf._form.Controls.Count; i++)
				{
					TreeNodeControl tc = new TreeNodeControl(tf._form.Controls[i]);
					parentNode.Nodes.Add(tc);
				}
				if (tf._form.Site != null && tf._form.Site.DesignMode)
				{
					for (int i = 0; i < tf._form.Site.Container.Components.Count; i++)
					{
						if (!(tf._form.Site.Container.Components[i] is Control) && tf._form.Site.Container.Components[i].Site != null)
						{
							TreeNodeComponent tn = new TreeNodeComponent(tf._form.Site.Container.Components[i], tf._form.Site.Container.Components[i].Site.Name);
							parentNode.Nodes.Add(tn);
						}
					}
				}
				else
				{
					FieldInfo[] fifs = tf._form.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
					if (fifs != null && fifs.Length > 0)
					{
						for (int i = 0; i < fifs.Length; i++)
						{
							if (!fifs[i].IsStatic && !fifs[i].FieldType.IsSubclassOf(typeof(Control)))
							{
								TypeFilter tfl = new TypeFilter(onFindInterface);
								Type[] tps = fifs[i].FieldType.FindInterfaces(tfl, null);
								if (tps != null && tps.Length > 0)
								{
									TreeNodeComponent tn = new TreeNodeComponent((IComponent)fifs[i].GetValue(tf._form), fifs[i].Name);
									parentNode.Nodes.Add(tn);
								}
							}
						}
					}
				}
				//
				TreeNodeObject.LoadNextLevel(parentNode, typeof(Form));
				//
			}
		}
	}
	class TreeNodeComponent : TreeNodeObject
	{
		private IComponent _component;
		public TreeNodeComponent(IComponent ic, string name)
		{
			_component = ic;
			Text = name;
			ImageIndex = TreeViewObjectBrowser.GetTypeIcon(_component.GetType());
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		public override MathPropertyPointer CreatePointer()
		{
			TreeNodeObject pn = Parent as TreeNodeObject;
			if (pn != null)
			{
				MathPropertyPointer mp = pn.CreatePointer();
				MathPropertyPointerComponent mpp = new MathPropertyPointerComponent();
				mp.Child = mpp;
				mpp.PropertyName = this.Text;
				mpp.Instance = _component;
				return mpp;
			}
			return null;
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeObject parentNode)
			{
				TreeNodeComponent tf = (TreeNodeComponent)parentNode;
				Type t = tf._component.GetType();
				TreeNodeObject.LoadNextLevel(parentNode, t);
			}
		}
	}
	class TreeNodeControl : TreeNodeObject
	{
		private Control _control;
		public TreeNodeControl(Control c)
		{
			_control = c;
			Text = _control.Name;
			ImageIndex = TreeViewObjectBrowser.GetTypeIcon(_control.GetType());
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		public override MathPropertyPointer CreatePointer()
		{
			TreeNodeObject pn = Parent as TreeNodeObject;
			if (pn != null)
			{
				MathPropertyPointer mp = pn.CreatePointer();
				MathPropertyPointerControl mpp = new MathPropertyPointerControl();
				mp.Child = mpp;
				mpp.PropertyName = this.Text;
				mpp.Instance = _control;
				return mpp;
			}
			return null;
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeObject parentNode)
			{
				TreeNodeControl tf = (TreeNodeControl)parentNode;
				Type t = tf._control.GetType();
				TreeNodeObject.LoadNextLevel(parentNode, t);
			}
		}
	}
	class TreeNodeField : TreeNodeObject
	{
		FieldInfo _field;
		public TreeNodeField(FieldInfo field)
		{
			_field = field;
			Text = _field.Name;
			ImageIndex = TreeViewObjectBrowser.GetTypeIcon(_field.FieldType);
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		public override MathPropertyPointer CreatePointer()
		{
			TreeNodeObject pn = Parent as TreeNodeObject;
			if (pn != null)
			{
				MathPropertyPointer mp = pn.CreatePointer();
				MathPropertyPointerField mpp = new MathPropertyPointerField();
				mp.Child = mpp;
				mpp.PropertyName = this.Text;
				return mpp;
			}
			return null;
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeObject parentNode)
			{
				TreeNodeField tf = (TreeNodeField)parentNode;
				Type t = tf._field.FieldType;
				TreeNodeObject.LoadNextLevel(parentNode, t);
			}
		}
	}
	class TreeNodeProperty : TreeNodeObject
	{
		PropertyInfo _property;
		public TreeNodeProperty(PropertyInfo property)
		{
			_property = property;
			Text = _property.Name;
			ImageIndex = TreeViewObjectBrowser.GetTypeIcon(_property.PropertyType);
			SelectedImageIndex = ImageIndex;
			Nodes.Add(new CLoader());
		}
		public override MathPropertyPointer CreatePointer()
		{
			TreeNodeObject pn = Parent as TreeNodeObject;
			if (pn != null)
			{
				MathPropertyPointer mp = pn.CreatePointer();
				MathPropertyPointerProperty mpp = new MathPropertyPointerProperty();
				mp.Child = mpp;
				mpp.PropertyName = this.Text;
				return mpp;
			}
			return null;
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader()
			{
			}

			public override void LoadNextLevel(TreeNodeObject parentNode)
			{
				TreeNodeProperty tf = (TreeNodeProperty)parentNode;
				Type t = tf._property.PropertyType;
				TreeNodeObject.LoadNextLevel(parentNode, t);
			}
		}
	}
}
