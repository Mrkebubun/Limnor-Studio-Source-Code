/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
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
using System.Globalization;
using System.Collections.Specialized;

namespace VPL
{
	public partial class DlgLanguages : Form
	{
		private bool _specificCulture = false;
		public StringCollection SelectedLanguages;
		public DlgLanguages()
		{
			InitializeComponent();
		}
		public static StringCollection SelectLanguages(Form caller, IList<string> ls)
		{
			DlgLanguages dlg = new DlgLanguages();
			StringCollection sc = new StringCollection();
			foreach (string s in ls)
			{
				sc.Add(s);
			}
			dlg.LoadData(sc);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				return dlg.SelectedLanguages;
			}
			return null;
		}
		public void SetUseSpecificCulture()
		{
			_specificCulture = true;
		}
		public void LoadData(StringCollection currentLanguages)
		{
			System.Globalization.CultureInfo[] cs = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
			SortedList<string, CultureInfo> list = new SortedList<string, CultureInfo>();
			for (int i = 0; i < cs.Length; i++)
			{
				if (!string.IsNullOrEmpty(cs[i].Name))
				{
					if (_specificCulture)
					{
						if (cs[i].Name.IndexOf('-') < 0)
						{
							continue;
						}
					}
					if (string.CompareOrdinal("zh-CHS", cs[i].Name) == 0)
					{
						if (!list.ContainsKey("zh"))
						{
							list.Add("zh", cs[i]);
						}
					}
					list.Add(cs[i].Name, cs[i]);
				}
			}
			IEnumerator<KeyValuePair<string, CultureInfo>> ie = list.GetEnumerator();
			while (ie.MoveNext())
			{
				int n;
				if (string.CompareOrdinal("zh", ie.Current.Key) == 0)
				{
					n = checkedListBox1.Items.Add(new CultureZh());
				}
				else
				{
					n = checkedListBox1.Items.Add(new Cultrue(ie.Current.Value));
				}
				if (currentLanguages != null && currentLanguages.Contains(ie.Current.Key))
				{
					checkedListBox1.SetItemChecked(n, true);
				}
			}
		}


		private void buttonOK_Click(object sender, EventArgs e)
		{
			SelectedLanguages = new StringCollection();
			for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
			{
				Cultrue c = checkedListBox1.CheckedItems[i] as Cultrue;
				if (c != null)
				{
					SelectedLanguages.Add(c.Name);
				}
			}
			this.DialogResult = DialogResult.OK;
		}
	}
	class Cultrue
	{
		private CultureInfo _culture;
		private Image _img;
		public Cultrue(CultureInfo c)
		{
			_culture = c;
			if (VPLUtil.GetLanguageImageByName != null)
			{
				_img = VPLUtil.GetLanguageImageByName(c.Name);
			}
		}
		public Image Image
		{
			get
			{
				return _img;
			}
		}
		public virtual string Name
		{
			get
			{
				return _culture.Name;
			}
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{1} [{0}]", _culture.Name, _culture.NativeName);
		}
	}
	class CultureZh : Cultrue
	{
		public CultureZh()
			: base(new CultureInfo("zh-CHT"))
		{
		}
		public override string Name
		{
			get
			{
				return "zh";
			}
		}
		public override string ToString()
		{
			return "中文 zh";
		}
	}
}
