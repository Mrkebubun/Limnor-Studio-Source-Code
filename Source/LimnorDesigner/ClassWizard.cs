/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Globalization;
using XmlUtility;

namespace LimnorDesigner
{
	public class ClassWizard
	{
		private LimnorProject _prj;
		public ClassWizard(LimnorProject project)
		{
			_prj = project;
		}
		public bool RunWizard(Dictionary<string, string> replacementsDictionary)
		{
			bool bOK;
			DataTypePointer dtp = DesignUtil.SelectDataType(_prj, null, null, EnumObjectSelectType.BaseClass, null, null, null, null);
			if (dtp != null)
			{
				//for non-generic type:
				// $typevalues$ is empty
				// $typeparameters$ is empty
				// $typeDefinition$ is empty
				// $ObjectType$ = $typeName$ = name
				// $qualifiedTypeName$ = t.AssemblyQualifiedName
				// $typeFileName$ = t.Assembly.Location
				//for generic type:
				// $ObjectType$ = $typeName$ = classBaseTypeName = name+"A"
				// $typeparameters$=<Item for t, type=name><Item for each generic type parameter><Item for each type argument>
				// $qualifiedTypeName$ = empty
				// $typeDefinition$ = name
				// $typevalues$ = <TypeParameter for each type argument>
				StringBuilder sbTypes = new StringBuilder();
				StringBuilder sbValues = new StringBuilder();
				Type t = dtp.ObjectType;
				Type tBase = t;
				//
				string name = XmlUtility.XmlUtil.CheckCreateTypeName(t);
				string classBaseTypeName = name;
				string fullTypename = t.AssemblyQualifiedName;
				string typeDefName = string.Empty;
				string fileLocation = t.Assembly.Location;
				if (t.IsGenericType)
				{
					typeDefName = name;
					fullTypename = string.Empty;
					fileLocation = string.Empty;
					classBaseTypeName = string.Format(CultureInfo.InvariantCulture, "{0}A", name);
					tBase = t.GetGenericTypeDefinition();
					Type[] targs = t.GetGenericArguments();
					string[] pnames = new string[targs.Length];
					sbTypes.Append("<Item type=\"");
					sbTypes.Append(name);
					sbTypes.Append("\" fullTypeName=\"");
					sbTypes.Append(t.AssemblyQualifiedName);
					sbTypes.Append("\" filename=\"");
					sbTypes.Append(t.Assembly.Location);
					sbTypes.Append("\" />");
					if (targs != null && targs.Length > 0)
					{
						for (int i = 0; i < targs.Length; i++)
						{
							sbTypes.Append("<Item ownerTypeName=\"");
							sbTypes.Append(name);
							sbTypes.Append("\" typeName=\"");
							sbTypes.Append(targs[i].Name);
							sbTypes.Append("\" type=\"");
							pnames[i] = XmlUtility.XmlUtil.CheckCreateTypeName(targs[i]);
							sbTypes.Append(pnames[i]);
							sbTypes.Append("\" />");
						}
					}
					if (dtp.TypeParameters != null)
					{
						for (int i = 0; i < dtp.TypeParameters.Length; i++)
						{
							sbValues.Append("<TypeParameter ");
							sbTypes.Append("<Item ");
							if (dtp.TypeParameters[i].IsLibType)
							{
								string argname = XmlUtility.XmlUtil.CheckCreateTypeName(dtp.TypeParameters[i].BaseClassType);
								sbValues.Append("type=\"");
								sbValues.Append(argname);
								sbValues.Append("\" />");
								//
								sbTypes.Append("type=\"");
								sbTypes.Append(argname);
								sbTypes.Append("\" fullTypeName=\"");
								sbTypes.Append(dtp.TypeParameters[i].BaseClassType.AssemblyQualifiedName);
								sbTypes.Append("\"");
								if (!dtp.TypeParameters[i].BaseClassType.Assembly.GlobalAssemblyCache)
								{
									sbTypes.Append(" filename=\"");
									sbTypes.Append(dtp.TypeParameters[i].BaseClassType.Assembly.Location);
									sbTypes.Append("\"");
								}
								sbTypes.Append(">");
							}
							else
							{
								string argname = string.Format(CultureInfo.InvariantCulture, "class{0}", dtp.TypeParameters[i].ClassId);
								sbValues.Append("type=\"");
								sbValues.Append(argname);
								sbValues.Append("\" />");
								//
								sbTypes.Append("type=\"");
								sbTypes.Append(argname);
								sbTypes.Append("\" ");
								sbTypes.Append(XmlTags.XMLATT_ClassID);
								sbTypes.Append("=\"");
								sbTypes.Append(dtp.TypeParameters[i].ClassId);
								sbTypes.Append("\" ");
								sbTypes.Append(XmlTags.XMLATT_guid);
								sbTypes.Append("=\"");
								sbTypes.Append(_prj.ProjectGuid.ToString("N", CultureInfo.InvariantCulture));
								sbTypes.Append("\">");
							}
							sbTypes.Append("</Item>");
						}
					}
				}
				replacementsDictionary.Add("$qualifiedTypeName$", fullTypename);
				replacementsDictionary.Add("$typeName$", classBaseTypeName);
				replacementsDictionary.Add("$typeFileName$", fileLocation);
				replacementsDictionary.Add("$typeDefinition$", typeDefName);
				bool hasTopDesigner = false;
				object[] vs = t.GetCustomAttributes(typeof(DesignerAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					for (int j = 0; j < vs.Length; j++)
					{
						DesignerAttribute da = (DesignerAttribute)vs[j];
						Type td = Type.GetType(da.DesignerTypeName);
						if (td != null)
						{
							if (td.GetInterface("IRootDesigner") != null)
							{
								hasTopDesigner = true;
								break;
							}
						}
					}
				}
				if (t.GetInterface("IComponent") != null && hasTopDesigner)
				{
					replacementsDictionary["$ObjectType$"] = classBaseTypeName;
				}
				else
				{
					if (typeof(Control).IsAssignableFrom(t))
					{
						replacementsDictionary["$ObjectType$"] = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"VPL.XControl`1[[{0}]]",
							classBaseTypeName);
					}
					else
					{
						replacementsDictionary["$ObjectType$"] = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"VPL.XClass`1[[{0}]]",
							classBaseTypeName);
					}
				}
				replacementsDictionary.Add("$BaseClassId$", dtp.ClassId.ToString());
				replacementsDictionary.Add("$typevalues$", sbValues.ToString());
				replacementsDictionary.Add("$typeparameters$", sbTypes.ToString());
				bOK = true;
			}
			else
			{
				bOK = false;
			}
			return bOK;
		}
	}
}
