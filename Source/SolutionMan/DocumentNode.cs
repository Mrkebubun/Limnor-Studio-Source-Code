/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SolutionMan
{
	//TBD: for project related documents
	public class DocumentNode : TreeNodeClass
	{
		private string _file;
		public DocumentNode(string file)
		{
			_file = file;
			this.ImageIndex = SolutionTree.IMG_DOC;
			this.SelectedImageIndex = SolutionTree.IMG_DOC;
			Text = Path.GetFileName(file);
		}
		public string DocFile
		{
			get
			{
				return _file;
			}
		}
	}
}
