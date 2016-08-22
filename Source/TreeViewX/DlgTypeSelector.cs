/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.TreeViewExt
{
	public partial class DlgTypeSelector : Form
	{
		#region fields and constructors
		const int IMG_BOOL = 0;
		const int IMG_CHAR = 1;
		const int IMG_STR = 2;
		const int IMG_SBYTE = 3;
		const int IMG_INT = 4;
		const int IMG_BYTE = 5;
		const int IMG_DECIMAL = 6;
		const int IMG_DATETIME = 7;
		const int IMG_COLOR = 8;
		const int IMG_POINT = 9;
		const int IMG_SIZE = 10;

		public Type SelectedType = typeof(string);
		public DlgTypeSelector()
		{
			InitializeComponent();
			addTypeNode(typeof(bool), "Boolean", IMG_BOOL);
			addTypeNode(typeof(char), "Single letter", IMG_CHAR);
			addTypeNode(typeof(string), "String (one or more letters)", IMG_STR);
			addTypeNode(typeof(sbyte), "Integer(8-bit)", IMG_SBYTE);
			addTypeNode(typeof(short), "Integer(16-bit)", IMG_INT);
			addTypeNode(typeof(int), "Integer(32-bit)", IMG_INT);
			addTypeNode(typeof(long), "Integer(64-bit)", IMG_INT);
			addTypeNode(typeof(byte), "Usigned Integer(8-bit)", IMG_BYTE);
			addTypeNode(typeof(ushort), "Usigned Integer(16-bit)", IMG_INT);
			addTypeNode(typeof(uint), "Usigned Integer(32-bit)", IMG_INT);
			addTypeNode(typeof(ulong), "Usigned Integer(64-bit)", IMG_INT);
			addTypeNode(typeof(float), "Single(7 digits decimal)", IMG_DECIMAL);
			addTypeNode(typeof(double), "Double(15 digits decimal)", IMG_DECIMAL);
			addTypeNode(typeof(decimal), "Money(28 digits decimal)", IMG_DECIMAL);
			addTypeNode(typeof(DateTime), "DateTime", IMG_DATETIME);
			addTypeNode(typeof(TimeSpan), "TimeSpan", IMG_DATETIME);
			addTypeNode(typeof(Color), "Color", IMG_COLOR);
			addTypeNode(typeof(Point), "Point", IMG_POINT);
			addTypeNode(typeof(Size), "Size", IMG_SIZE);

			treeView1.SelectedNode = treeView1.Nodes[2];
		}
		#endregion
		#region static functions
		public static Type SelectType(Form caller)
		{
			DlgTypeSelector dlg = new DlgTypeSelector();
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				return dlg.SelectedType;
			}
			return null;
		}
		#endregion
		#region private methods
		private void addTypeNode(Type type, string name, int img)
		{
			treeView1.Nodes.Add(new TreeNodeType(name, type, img));
		}
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNodeType tnt = e.Node as TreeNodeType;
			if (tnt != null)
			{
				SelectedType = tnt.Type;
			}
		}
		#endregion
		#region class TreeNodeType
		class TreeNodeType : TreeNode
		{
			private Type _type;
			public TreeNodeType(string text, Type type, int img)
			{
				Text = text;
				_type = type;
				ImageIndex = img;
				SelectedImageIndex = img;
			}
			public Type Type
			{
				get
				{
					return _type;
				}
			}
		}
		#endregion

	}
}
