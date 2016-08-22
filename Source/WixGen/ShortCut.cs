using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlUtility;

namespace WixGen
{
	class ShortCut
	{
		private XmlNode _node;
		private string _filename;
		private string _key;
		public ShortCut(XmlNode node)
		{
			_node = node;
			_filename = XmlUtil.GetAttribute(node, "file");
			_key = _filename.ToLowerInvariant();
		}
		public string Key { get { return _key; } }
		public bool Used { get; set; }
		public bool IconSet { get; set; }
		public XmlNode DirNode { get; set; }
		public XmlNode FeatureNode { get; set; }
		public XmlNode Node { get { return _node; } }
		public string File { get { return _filename; } }
		public void Create(XmlNode fNode)
		{
			XmlNode nd = DirNode.OwnerDocument.CreateElement("Shortcut", Program.uri);
			fNode.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "Id", Program.formFileId(_filename));
			XmlUtil.SetAttribute(nd, "Advertise", "yes");
			Program.passAttribute(nd, _node, "Directory");
			Program.passAttribute(nd, _node, "Name");
			Program.passAttribute(nd, _node, "WorkingDirectory");
			Program.passAttribute(nd, _node, "Icon");
			//
		}
	}
	class ShortCutList : Dictionary<string, List<ShortCut>>
	{
		public ShortCutList(XmlNode list)
		{
			if (list != null)
			{
				XmlNodeList nds = list.SelectNodes("Item");
				foreach (XmlNode nd in nds)
				{
					ShortCut sc = new ShortCut(nd);
					List<ShortCut> sl;
					if (!this.TryGetValue(sc.Key, out sl))
					{
						sl = new List<ShortCut>();
						this.Add(sc.Key, sl);
					}
					sl.Add(sc);
				}
			}
		}
		public bool IsShortcut(string file, XmlNode dirNode, XmlNode featureNode)
		{
			bool b = false;
			foreach (KeyValuePair<string, List<ShortCut>> kv in this)
			{
				if (string.Compare(file, kv.Key, StringComparison.OrdinalIgnoreCase) == 0)
				{
					foreach (ShortCut s in kv.Value)
					{
						s.Used = true;
						s.DirNode = dirNode;
						s.FeatureNode = featureNode;
					}
					b = true;
				}
			}
			return b;
		}
		public void Create(StringBuilder errorReport)
		{
			foreach (KeyValuePair<string, List<ShortCut>> kv in this)
			{
				if (kv.Value.Count > 0)
				{
					if (kv.Value[0].Used)
					{
						XmlNode dirNode = kv.Value[0].DirNode;
						XmlNode featureNode = kv.Value[0].FeatureNode;
						XmlNode cNode = dirNode.OwnerDocument.CreateElement("Component", Program.uri);
						dirNode.AppendChild(cNode);
						string cid = Program.GetNewId();
						XmlUtil.SetAttribute(cNode, "Id", cid);
						XmlUtil.SetAttribute(cNode, "Guid", Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
						XmlNode fNode = dirNode.OwnerDocument.CreateElement("File", Program.uri);
						string fid = Program.formFileId(kv.Key);
						XmlUtil.SetAttribute(fNode, "Id", fid);
						XmlUtil.SetAttribute(fNode, "Name", Path.GetFileName(kv.Value[0].File));
						XmlUtil.SetAttribute(fNode, "DiskId", "1");
						XmlUtil.SetAttribute(fNode, "Source", kv.Value[0].File);
						XmlUtil.SetAttribute(fNode, "KeyPath", "yes");
						cNode.AppendChild(fNode);
						//
						XmlNode crf = dirNode.OwnerDocument.CreateElement("ComponentRef", Program.uri);
						XmlUtil.SetAttribute(crf, "Id", cid);
						featureNode.AppendChild(crf);
						//
						foreach (ShortCut s in kv.Value)
						{
							s.Create(fNode);
						}
					}
					else
					{
						errorReport.Append("Shortcut target not found:");
						errorReport.Append(kv.Key);
						errorReport.Append("\r\n");
					}
				}
			}
		}
	}
}
