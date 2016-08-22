using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Globalization;

namespace VPL
{
	public partial class ToolboxItemXType : Component
	{
		public static string SelectedToolboxTypeKey;
		public static UInt32 SelectedToolboxClassId;
		public static string SelectedToolboxClassName;
		public static Type SelectedToolboxType;
		public ToolboxItemXType()
		{
			InitializeComponent();
		}

		public ToolboxItemXType(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
		#region Toolbox help
		private static Dictionary<string, Type> _toolboxTypes;
		public static string AddToolboxType(Type t)
		{
			if (t != null)
			{
				if (_toolboxTypes == null)
				{
					_toolboxTypes = new Dictionary<string, Type>();
				}
				foreach (KeyValuePair<string, Type> kv in _toolboxTypes)
				{
					if (t.Equals(kv.Value))
					{
						return kv.Key;
					}
				}
				string name = VPLUtil.GetTypeDisplay(t);
				int n = 1;
				while (_toolboxTypes.ContainsKey(name))
				{
					n++;
					name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", t.Name, n);
				}
				_toolboxTypes.Add(name, t);
				return name;
			}
			return string.Empty;
		}
		public static Type GetToolboxType(string name)
		{
			if (_toolboxTypes != null)
			{
				Type t;
				if (_toolboxTypes.TryGetValue(name, out t))
				{
					return t;
				}
			}
			return null;
		}
		public static Type GetCurrentToolboxType()
		{
			return GetToolboxType(SelectedToolboxTypeKey);
		}
		#endregion
	}
}
