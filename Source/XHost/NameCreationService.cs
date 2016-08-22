/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using VOB;
using VPL;

namespace XHost
{
	/// <summary>
	/// This is responsible for naming the components as they are created.
	/// This is added as a servide by the HostSurfaceManager
	/// </summary>
	public class NameCreationService : IVplNameService
	{
		private Type _componentType;
		public NameCreationService()
		{
		}

		string INameCreationService.CreateName(IContainer container, Type type)
		{
			if (container == null || type == null)
			{
				return "name1";
			}
			string baseName = VPLUtil.FormCodeNameFromname(VPLUtil.GetTypeDisplay(type));
			ComponentCollection cc = container.Components;
			int min = Int32.MaxValue;
			int max = Int32.MinValue;
			int count = 0;

			for (int i = 0; i < cc.Count; i++)
			{
				IComponent comp = cc[i] as IComponent;

				if (comp.GetType() == type)
				{
					count++;

					string name = comp.Site.Name;
					if (name.StartsWith(baseName))
					{
						try
						{
							int value = Int32.Parse(name.Substring(baseName.Length));

							if (value < min)
								min = value;

							if (value > max)
								max = value;
						}
						catch (Exception ex)
						{
							Trace.WriteLine(ex.ToString());
						}
					}
				}
			}// for

			if (count == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}1", baseName);
			}
			else if (min > 1)
			{
				int j = min - 1;

				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseName, j);
			}
			else
			{
				int j = max + 1;

				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseName, j);
			}
		}
		bool INameCreationService.IsValidName(string name)
		{
			return VOB.VobUtil.IsGoodVarName(name);
		}
		void INameCreationService.ValidateName(string name)
		{
			if (name == null || name.Length == 0)
				throw new Exception("The name cannot be empty");
			if (VobUtil.RootComponentSelected && VobUtil.CurrentComponent != null)
			{
				InterfaceVOB vob = VobUtil.VobService;
				if (vob != null)
				{
					if (name != VobUtil.CurrentComponentName)
					{
						EventArgNameChange en = new EventArgNameChange(name, VobUtil.CurrentComponentName);
						en.Attributes = VobUtil.CurrentProjectName;
						en.Owner = VobUtil.CurrentComponent;
						en.ComponentType = _componentType;
						vob.SendNotice(enumVobNotice.BeforeRootComponentNameChange, en);
						if (en.Cancel)
						{
							if (string.IsNullOrEmpty(en.Message))
								throw new Exception("Invalid name: " + name);
							else
								throw new Exception(en.Message);
						}
					}
				}
			}

			// First character must be a letter
			if (!Char.IsLetter(name, 0))
				throw new Exception("The first character of the name must be a letter");
			for (int i = 0; i < name.Length; i++)
			{
				char ch = name[i];
				if (ch != '_')
				{
					UnicodeCategory uc = Char.GetUnicodeCategory(ch);
					switch (uc)
					{
						case UnicodeCategory.UppercaseLetter:
						case UnicodeCategory.LowercaseLetter:
						case UnicodeCategory.TitlecaseLetter:
						case UnicodeCategory.DecimalDigitNumber:
							break;
						default:
							throw new Exception("The name '" + name + "' is not a valid identifier.");
					}
				}
			}

		}


		#region IVplNameService Members

		public Type ComponentType
		{
			get
			{
				return _componentType;
			}
			set
			{
				_componentType = value;
			}
		}

		#endregion
	}// class
}// namespace
