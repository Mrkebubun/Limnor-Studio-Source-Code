/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using VPL;
using System.CodeDom;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Globalization;

namespace XmlUtility
{
	public delegate Type fnCreateDesignTimeType(UInt32 classId, Guid projectGuid);
	public sealed class XmlUtil
	{
		#region fields and constructors
		const string XML_TYPES = "Types";
		const string XML_Item = "Item";
		const string XMLATT_TYPE = "type";
		const string XMLATT_MetaToken = "metatoken";
		const string XMLATT_MethodSignature = "methodsignature";
		public const string XMLATT_NAME = "name";
		const string XMLATT_FullTypeName = "fullTypeName";
		const string XMLATT_filename = "filename";
		const string XMLATT_ownerTypeName = "ownerTypeName";
		const string XMLATT_typeName = "typeName";
		const string XMLATT_isreturn = "isreturn";
		const string XMLATT_arguments = "arguments";
		const string XMLATT_typeDefinition = "typeDefinition";
		const string XMLATT_arrayItemType = "arrayItemType";
		const string XMLATT_rank = "rank";
		static string baseDir;
		//
		const string XClassNamePrefix = "VPL.XClass`1[[";
		const string XControlNamePrefix = "VPL.XControl`1[[";
		//
		public const string APPFOLDER = "$ApplicationFolder$";
		//
		static Dictionary<string, string> _dllFileMapping;
		//
		private XmlUtil()
		{
		}
		#endregion
		#region Known types
		private static Dictionary<string, Type> _knownTypes;
		private static Dictionary<Guid, Dictionary<string, Type>> _dynamicTypes; //such types needs to be in each XmlNode
		static XmlUtil()
		{
			_knownTypes = new Dictionary<string, Type>();
			_knownTypes.Add("bool", typeof(bool));
			_knownTypes.Add("byte", typeof(byte));
			_knownTypes.Add("int", typeof(Int32));
			_knownTypes.Add("long", typeof(Int64));
			_knownTypes.Add("short", typeof(Int16));
			_knownTypes.Add("uint", typeof(UInt32));
			_knownTypes.Add("ulong", typeof(UInt64));
			_knownTypes.Add("ushort", typeof(UInt16));
			_knownTypes.Add("char", typeof(char));
			_knownTypes.Add("string", typeof(string));
			_knownTypes.Add("sbyte", typeof(sbyte));
			_knownTypes.Add("float", typeof(float));
			_knownTypes.Add("double", typeof(double));
			_knownTypes.Add("decimal", typeof(decimal));
			_knownTypes.Add("datetime", typeof(DateTime));
			_knownTypes.Add("type", typeof(Type));
			_knownTypes.Add("void", typeof(void));
			_knownTypes.Add("Color", typeof(Color));
			_knownTypes.Add("Size", typeof(Size));
			_knownTypes.Add("Point", typeof(Point));
		}

		public static void RemoveDynamicTypes()
		{
			_dynamicTypes = null;
			VPLUtil.RemoveDynamicAssemblies();
		}
		public static string CreateNewTypeName(string baseTypeName, XmlNode node)
		{
			string name;
			int n = 1;
			StringCollection sc = new StringCollection();
			XmlNodeList nds = node.OwnerDocument.DocumentElement.SelectNodes(string.Format(CultureInfo.InvariantCulture,
				"{0}//{1}", XML_TYPES, XmlTags.XML_Item));
			foreach (XmlNode nd in nds)
			{
				name = XmlUtil.GetLibTypeAttributeString(nd);
				if (!string.IsNullOrEmpty(name))
				{
					sc.Add(name);
				}
			}
			name = baseTypeName;
			while (true)
			{
				if (_knownTypes.ContainsKey(name))
				{
					n++;
				}
				else
				{
					if (sc.Contains(name))
					{
						n++;
					}
					else
					{
						break;
					}
				}
				name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseTypeName, n);
			}
			return name;
		}
		public static bool TryGetKnownType(string name, out Type type)
		{
			return _knownTypes.TryGetValue(name, out type);
		}
		public static string CheckCreateTypeName(Type type)
		{
			foreach (KeyValuePair<string, Type> kv in _knownTypes)
			{
				if (kv.Value.Equals(type))
				{
					return kv.Key;
				}
			}
			string name = type.Name;
			int n = 1;
			while (true)
			{
				if (_knownTypes.ContainsKey(name))
				{
					n++;
					name = type.Name + n.ToString();
				}
				else
				{
					break;
				}
			}
			return name;
		}
		static MethodBase getMethodByToken(Type t, int token, string methodSignature, params BindingFlags[] flags)
		{
			MethodBase mif = null;
			MethodInfo[] ms;
			BindingFlags f = BindingFlags.Default;
			if (flags != null && flags.Length > 0)
			{
				f = flags[0];
				for (int i = 1; i < flags.Length; i++)
				{
					f |= flags[i];
				}
			}
			if (flags != null && flags.Length > 0)
			{
				ms = t.GetMethods(f);
			}
			else
			{
				ms = t.GetMethods();
			}
			if (ms != null)
			{
				for (int k = 0; k < ms.Length; k++)
				{
					if (ms[k].MetadataToken == token)
					{
						mif = ms[k];
						break;
					}
					if (!string.IsNullOrEmpty(methodSignature))
					{
						string mg = VPLUtil.GetMethodSignature(ms[k]);
						if (string.CompareOrdinal(mg, methodSignature) == 0)
						{
							mif = ms[k];
							break;
						}
					}
				}
			}
			if (mif == null)
			{
				ConstructorInfo[] cif;
				if (flags != null && flags.Length > 0)
				{
					cif = t.GetConstructors(f);
				}
				else
				{
					cif = t.GetConstructors();
				}
				if (cif != null)
				{
					for (int k = 0; k < cif.Length; k++)
					{
						if (cif[k].MetadataToken == token)
						{
							mif = cif[k];
							break;
						}
						if (!string.IsNullOrEmpty(methodSignature))
						{
							string mg = VPLUtil.GetMethodSignature(cif[k]);
							if (string.CompareOrdinal(mg, methodSignature) == 0)
							{
								mif = cif[k];
								break;
							}
						}
					}
				}
			}
			return mif;
		}
		public static Assembly LoadAssembly(string path, bool adjust)
		{
			if (!System.IO.File.Exists(path) && adjust)
			{
				DialogFilePath dlg = new DialogFilePath();
				dlg.LoadData(path, null);
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					path = dlg.AdjustedPath;
				}
			}
			if (!VPLUtil.Shutingdown)
			{
				if (System.IO.File.Exists(path))
				{
					VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(path));
					return Assembly.LoadFrom(path);
				}
			}
			return null;
		}
		/// <summary>
		/// parse XClass or XControl to get type name inside the whole type name
		/// </summary>
		/// <param name="xclass"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static bool TryGetTypeNameFromXClass(string xclass, out string typeName)
		{
			if (xclass.StartsWith(XClassNamePrefix, true, System.Globalization.CultureInfo.InvariantCulture))
			{
				if (xclass.EndsWith("]]", StringComparison.InvariantCulture))
				{
					string subType = xclass.Substring(XClassNamePrefix.Length);
					subType = subType.Substring(0, subType.Length - 2);
					typeName = subType;
					return true;
				}
				else
				{
					typeName = string.Format(CultureInfo.InvariantCulture, "Invalid type:{0}", xclass);
				}
			}
			else if (xclass.StartsWith(XControlNamePrefix, true, System.Globalization.CultureInfo.InvariantCulture))
			{
				if (xclass.EndsWith("]]", StringComparison.InvariantCulture))
				{
					string subType = xclass.Substring(XControlNamePrefix.Length);
					subType = subType.Substring(0, subType.Length - 2);
					typeName = subType;
					return true;
				}
				else
				{
					typeName = string.Format(CultureInfo.InvariantCulture, "Invalid type:{0}", xclass);
				}
			}
			else
			{
				typeName = string.Empty;
			}
			return false;
		}

		public static string GetFilenameAttribute(XmlNode node)
		{
			string f = GetAttribute(node, XMLATT_filename);
			if (!string.IsNullOrEmpty(f))
			{
				f = f.Replace(APPFOLDER, AppDomain.CurrentDomain.BaseDirectory);
			}
			return f;
		}
		public static Type GetTypeThrow(XmlNode node)
		{
			Type t = GetLibTypeAttribute(node);
			if (t == null)
			{
				throw new SerializerException("Error resolving type. {0}", XmlUtil.GetTypeNodeInfo(node));
			}
			return t;
		}
		public static string GetTypeNodeInfo(XmlNode node)
		{
			StringBuilder sb = new StringBuilder();
			string typeKey = GetAttribute(node, XMLATT_TYPE);
			if (string.IsNullOrEmpty(typeKey))
			{
				sb.Append("type attribute not set. path:[");
				sb.Append(GetPath(node));
				sb.Append("]");
			}
			else
			{
				XmlNode typesNode = node.OwnerDocument.DocumentElement.SelectSingleNode(XML_TYPES);
				if (typesNode == null)
				{
					sb.Append("Types node does not exists.");
				}
				else
				{
					XmlNode typeNode = typesNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
						"{0}[@{1}='{2}']", XmlTags.XML_Item, XMLATT_TYPE, typeKey));
					if (typeNode == null)
					{
						sb.Append("Type definition not found");
					}
					else
					{
						sb.Append("qualified name:[");
						sb.Append(GetAttribute(typeNode, XMLATT_FullTypeName));
						sb.Append("] file:[");
						sb.Append(GetAttribute(typeNode, XMLATT_filename));
						sb.Append("] owner:[");
						sb.Append(GetAttribute(typeNode, XMLATT_ownerTypeName));
						sb.Append("] def:[");
						sb.Append(GetAttribute(typeNode, XMLATT_typeDefinition));
						sb.Append("]");
					}
				}
			}
			return sb.ToString();
		}
		/// <summary>
		/// get type using the key from _knownTypes or Type node.
		/// when getting from Type node, it loads Assembly from the DLL file to get the type
		/// </summary>
		/// <param name="node">the node containing type attribute</param>
		/// <param name="name">the key name identifying a type within _knownTypes</param>
		/// <returns></returns>
		public static Type GetTypeByName(XmlNode node, string name, out string additionalInfo)
		{
			additionalInfo = string.Empty;
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Type type = Type.GetType(name); //backward compatibility
			if (type != null)
			{
				return type;
			}
			if (_dynamicTypes != null)
			{
				Guid classGuid = GetAttributeGuid(node.OwnerDocument.DocumentElement, XmlTags.XMLATT_guid);
				if (classGuid != Guid.Empty)
				{
					Dictionary<string, Type> typeLst;
					if (_dynamicTypes.TryGetValue(classGuid, out typeLst))
					{
						if (typeLst.TryGetValue(name, out type))
						{
							return type;
						}
					}
				}
			}
			string subType;
			////case 1: name is XClass[subType] or XControl[subType] then return XClass<subType> or XControl<subType>
			if (TryGetTypeNameFromXClass(name, out subType))
			{
				Type t = GetTypeByName(node, subType, out additionalInfo);
				if (t == null)
				{
					throw new XmlSerializerException("Invalid type:{0}. Sub type not found.", name);
				}
				//reconstruct XClass or XControl
				type = VPLUtil.GetXClassType(t);
				return type;
			}
			else
			{
				if (!string.IsNullOrEmpty(subType))
				{
					//subType contains error message
					throw new XmlSerializerException(subType);
				}
			}
			//case 2: global type
			if (_knownTypes.TryGetValue(name, out type))
			{
				return type;
			}
			//case 3: local type
			XmlNode typeNode = null;
			int mtoken;
			int isReturn;
			Type ownerType;
			string methodSignature;
			XmlNode typesNode = node.OwnerDocument.DocumentElement.SelectSingleNode(XML_TYPES);
			if (typesNode == null)
			{
			}
			else
			{
				typeNode = typesNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']", XmlTags.XML_Item, XMLATT_TYPE, name));
				if (typeNode == null)
				{
				}
			}
			if (typesNode != null && typeNode != null)
			{
				//case 3.0: development item
				UInt32 classId = GetAttributeUInt(typeNode, XmlTags.XMLATT_ClassID);
				if (classId != 0)
				{
					Guid prjGuid = GetAttributeGuid(typeNode, XmlTags.XMLATT_guid);
					if (prjGuid != Guid.Empty)
					{
						if (CreateDesignTimeType != null)
						{
							type = CreateDesignTimeType(classId, prjGuid);
						}
						if (type != null)
						{
							return type;
						}
					}
				}
				//case 3.1: generic method return type
				string sOwnerType = GetAttribute(typeNode, XMLATT_ownerTypeName);
				string sTypeName = GetAttribute(typeNode, XMLATT_typeName);
				isReturn = GetAttributeInt(typeNode, XMLATT_isreturn);
				mtoken = GetAttributeInt(typeNode, XMLATT_MetaToken);
				methodSignature = GetAttribute(typeNode, XMLATT_MethodSignature);
				if (mtoken != 0 && !string.IsNullOrEmpty(sOwnerType))
				{
					ownerType = GetTypeByName(node, sOwnerType, out additionalInfo);
					if (ownerType == null)
					{
						throw new SerializerException("Cannot find owner type for generic parameter. Owner type:[{0}], parameter name:[{1}], token:[{2}], isReturn:{3}", sOwnerType, sTypeName, mtoken, isReturn);
					}
					MethodBase mif = null;
					mif = getMethodByToken(ownerType, mtoken, methodSignature);
					if (mif == null)
					{
						mif = getMethodByToken(ownerType, mtoken, methodSignature, BindingFlags.NonPublic);
						if (mif == null)
						{
							mif = getMethodByToken(ownerType, mtoken, methodSignature, BindingFlags.Static);
							if (mif == null)
							{
								mif = getMethodByToken(ownerType, mtoken, methodSignature, BindingFlags.Static | BindingFlags.NonPublic);
								if (mif == null)
								{
									throw new SerializerException("Cannot find declaring method for generic parameter. Owner type:[{0}], parameter name:[{1}], token:[{2}]", sOwnerType, sTypeName, mtoken);
								}
							}
						}
					}
					if (mif != null)
					{
						if (isReturn != 0)
						{
							MethodInfo mifo = mif as MethodInfo;
							type = mifo.ReturnType;
						}
						else
						{
							Type[] tcs = mif.GetGenericArguments();
							if (tcs == null || tcs.Length == 0)
							{
								throw new SerializerException("Owner method does not have generic parameters. Owner type:[{0}], parameter name:[{1}], token:[{2}], method:[{3}]", sOwnerType, sTypeName, mtoken, VPLUtil.GetMethodSignature(mif));
							}
							else
							{
								for (int i = 0; i < tcs.Length; i++)
								{
									if (string.CompareOrdinal(tcs[i].Name, sTypeName) == 0)
									{
										type = tcs[i];
										break;
									}
								}
								if (type == null)
								{
									throw new SerializerException("Owner method does not have matching generic parameter name. Owner type:[{0}], parameter name:[{1}], token:[{2}], method:[{3}]", sOwnerType, sTypeName, mtoken, VPLUtil.GetMethodSignature(mif));
								}
							}
						}
					}
					if (type != null)
					{
						return type;
					}
				}
				//case 3.2: type parameter
				if (!string.IsNullOrEmpty(sOwnerType) && !string.IsNullOrEmpty(sTypeName))
				{
					ownerType = GetTypeByName(node, sOwnerType, out additionalInfo);
					if (ownerType != null && ownerType.IsGenericType)
					{
						Type[] argTs = ownerType.GetGenericArguments();
						if (argTs != null)
						{
							for (int i = 0; i < argTs.Length; i++)
							{
								if (string.CompareOrdinal(argTs[i].Name, sTypeName) == 0)
								{
									type = argTs[i];
									return type;
								}
							}
						}
					}
				}
				//case 3.3: array type
				string sArrayItemType = XmlUtil.GetAttribute(typeNode, XMLATT_arrayItemType);
				if (!string.IsNullOrEmpty(sArrayItemType))
				{
					Type arrayItemType = GetTypeByName(node, sArrayItemType, out additionalInfo);
					if (arrayItemType == null)
					{
						throw new SerializerException("Cannot resolve array item type [{0}]", sArrayItemType);
					}
					int rnk = XmlUtil.GetAttributeInt(typeNode, XMLATT_rank);
					if (rnk > 1)
					{
						type = arrayItemType.MakeArrayType(rnk);
					}
					else
					{
						type = arrayItemType.MakeArrayType();
					}
					return type;
				}
				//case 3.4: concret generic type
				Type definitionType;
				string gtDefName = XmlUtil.GetAttribute(typeNode, XMLATT_typeDefinition);
				if (!string.IsNullOrEmpty(gtDefName))
				{
					definitionType = GetTypeByName(node, gtDefName, out additionalInfo);
					if (definitionType == null)
					{
						throw new SerializerException("Cannot resolve generic definition type [{0}]", gtDefName);
					}
					if (definitionType.IsGenericType && definitionType.ContainsGenericParameters)
					{
						XmlNodeList ndArgcs = typeNode.SelectNodes(XmlTags.XML_TYPEParameter);
						Type[] argTypes = new Type[ndArgcs.Count];
						for (int i = 0; i < ndArgcs.Count; i++)
						{
							string aname = XmlUtil.GetLibTypeAttributeString(ndArgcs[i]);
							if (string.IsNullOrEmpty(aname))
							{
								throw new SerializerException("Type parameter [{0}] is empty for type [{1}] of generic definition type [{2}]", i, name, gtDefName);
							}
							argTypes[i] = GetTypeByName(node, aname, out additionalInfo);
							if (argTypes[i] == null)
							{
								throw new SerializerException("Cannot resolve type parameter [{0},{1}] for type [{2}] of generic definition type [{3}]", i, aname, name, gtDefName);
							}
						}
						type = definitionType.MakeGenericType(argTypes);
						return type;
					}
				}
				//case 3.5: direct type
				string sFullName = XmlUtil.GetAttribute(typeNode, XMLATT_FullTypeName);
				if (!string.IsNullOrEmpty(sFullName))
				{
					type = Type.GetType(sFullName);
					if (type != null)
					{
						return type;
					}
					//load by file
					bool pathAdjusted = false;
					string sPath = GetFilenameAttribute(typeNode);
					if (string.IsNullOrEmpty(sPath))
					{
						//backward compatibility
						sPath = node.InnerText;
						if (!string.IsNullOrEmpty(sPath))
						{
							if (sPath.Length > 2)
							{
								if (sPath[1] != ':')
								{
									sPath = string.Empty;
								}
							}
							else
							{
								sPath = string.Empty;
							}
						}
						pathAdjusted = !string.IsNullOrEmpty(sPath);
					}
					if (!string.IsNullOrEmpty(sPath))
					{
						if (sPath.StartsWith(APPFOLDER, StringComparison.Ordinal))
						{
							sPath = sPath.Replace(APPFOLDER, AppDomain.CurrentDomain.BaseDirectory);
						}
						string originalPath = sPath.ToLowerInvariant();
						if (!System.IO.File.Exists(sPath))
						{
							bool mapped = false;
							if (_dllFileMapping != null)
							{
								string s;
								if (_dllFileMapping.TryGetValue(originalPath, out s))
								{
									if (File.Exists(s))
									{
										sPath = s;
										pathAdjusted = true;
										mapped = true;
									}
								}
							}
							if (!mapped)
							{
								DialogFilePath dlg = new DialogFilePath();
								dlg.LoadData(sPath, sFullName);
								if (dlg.ShowDialog() == DialogResult.OK)
								{
									sPath = dlg.AdjustedPath;
									pathAdjusted = true;
								}
								if (VPLUtil.Shutingdown)
								{
									return null;
								}
							}
						}
						if (System.IO.File.Exists(sPath))
						{
							VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(sPath));
							try
							{
								string sx = sFullName;
								Assembly a = Assembly.LoadFrom(sPath);
								type = a.GetType(sFullName);
								if (type == null)
								{
									//backward compatibility
									int h;
									h = sFullName.IndexOf('[');
									if (h > 0)
									{
										int n = 1;
										h++;
										while (h < sFullName.Length)
										{
											if (sFullName[h] == '[')
											{
												n++;
												h++;
											}
											else
											{
												break;
											}
										}
										string se = new string(']', n);
										h = sFullName.IndexOf(se, StringComparison.Ordinal);
										if (h > 0)
										{
											string s0 = sFullName.Substring(0, h + se.Length);
											type = a.GetType(s0);
										}
									}
									if (type == null)
									{
										h = sFullName.IndexOf(',');
										if (h > 0)
										{
											sFullName = sFullName.Substring(0, h);
										}
										type = a.GetType(sFullName);
										if (type == null)
										{
											Type[] ts = a.GetExportedTypes();
											if (ts != null)
											{
												for (int i = 0; i < ts.Length; i++)
												{
													if (string.CompareOrdinal(sFullName, ts[i].FullName) == 0)
													{
														type = ts[i];
														break;
													}
												}
											}
										}
										if (type == null)
										{
											h = sx.IndexOf('[');
											if (h > 0)
											{
												string ts1 = sx.Substring(0, h);
												Type typeX = a.GetType(ts1);
												if (typeX != null)
												{
													if (typeX.IsGenericType && typeX.ContainsGenericParameters)
													{
														Type[] gtps = typeX.GetGenericArguments();
														if (gtps != null && gtps.Length > 0)
														{
															string tsx = sx.Substring(h + 1);
															h = tsx.LastIndexOf(']');
															if (h > 0)
															{
																tsx = tsx.Substring(0, h);
															}
															string[] types = tsx.Split(new string[] { "],[" }, StringSplitOptions.None);
															if (types.Length == gtps.Length)
															{
																for (int i = 0; i < types.Length; i++)
																{
																	string sx1 = types[i];
																	if (sx1.StartsWith("[", StringComparison.Ordinal))
																	{
																		sx1 = sx1.Substring(1);
																	}
																	if (sx1.EndsWith("]", StringComparison.Ordinal))
																	{
																		sx1 = sx1.Substring(0, sx1.Length - 1);
																	}
																	Type txx = Type.GetType(sx1);
																	if (txx == null)
																	{
																	}
																	if (txx != null)
																	{
																		gtps[i] = txx;
																	}
																}
																type = typeX.MakeGenericType(gtps);
															}
														}
													}
												}
											}
										}
									}
								}
							}
							catch
							{
								throw;
							}
							finally
							{
								VPLUtil.RemoveExternalDllResolve();
							}
							if (type != null)
							{
								if (pathAdjusted)
								{
									SetAttribute(typeNode, XMLATT_filename, sPath);
									saveDoc(typeNode);
									if (_dllFileMapping == null)
									{
										_dllFileMapping = new Dictionary<string, string>();
									}
									if (!_dllFileMapping.ContainsKey(originalPath))
									{
										_dllFileMapping.Add(originalPath, sPath);
									}
									else
									{
										_dllFileMapping[originalPath] = sPath;
									}
								}
							}
						}
					}
				}
				if (type != null)
				{
					return type;
				}
			}
			//backward compatibility
			//parse {name2}[,,...,] === TBD: use key name instead of real type name for array
			if (name.EndsWith("]", StringComparison.Ordinal))
			{
				int pos = name.LastIndexOf('[');
				if (pos < 0)
				{
					throw new SerializerException("Invalid type key:[{0}]", name);
				}
				string sr = name.Substring(pos);
				string[] srs = sr.Split(',');
				int nr = srs.Length;
				string name2 = name.Substring(0, pos);
				Type ti = GetTypeByName(node, name2, out additionalInfo);
				if (ti != null)
				{
					if (nr < 2)
					{
						Array a = Array.CreateInstance(ti, 0);
						type = a.GetType();
					}
					else
					{
						int[] dims = new int[nr];
						for (int i = 0; i < nr; i++)
						{
							dims[i] = 0;
						}
						Array a = Array.CreateInstance(ti, dims);
						type = a.GetType();
					}
				}
			}
			if (type == null)
			{
				//get type node by type name
				typeNode = node.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}//{1}[@{2}='{3}']",
					XML_TYPES, XML_Item, XMLATT_TYPE, name));
				if (typeNode != null)
				{
					string s;
					s = GetAttribute(typeNode, XMLATT_typeDefinition);
					if (!string.IsNullOrEmpty(s))
					{
						string clids;
						Type gt = GetTypeByName(node, s, out clids);
						if (gt == null)
						{
							throw new SerializerException("Definity type not resolved:[{0}]. path:[{1}]", s, GetPath(node));
						}
						XmlNodeList valueNodes = typeNode.SelectNodes(XmlTags.XML_TYPEParameter);
						Type[] valueTypes = new Type[valueNodes.Count];
						for (int i = 0; i < valueNodes.Count; i++)
						{
							Type vt = GetLibTypeAttribute(valueNodes[i]);
							if (vt == null)
							{
								throw new SerializerException("Definity type not resolved:[{0}]. Type Parameter [{1}] not resolved. path:[{2}]", s, i, GetPath(node));
							}
							valueTypes[i] = vt;
						}
						type = gt.MakeGenericType(valueTypes);
					}
					if (type == null)
					{
						s = GetAttribute(typeNode, XMLATT_ownerTypeName);
						string s1 = GetAttribute(typeNode, XMLATT_typeName);
						isReturn = GetAttributeInt(typeNode, XMLATT_isreturn);
						mtoken = GetAttributeInt(typeNode, XMLATT_MetaToken);
						methodSignature = GetAttribute(typeNode, XMLATT_MethodSignature);
						if (!string.IsNullOrEmpty(s) || !string.IsNullOrEmpty(s1) || isReturn != 0)
						{
							if ((string.IsNullOrEmpty(s1) && isReturn == 0) || string.IsNullOrEmpty(s))
							{
								throw new SerializerException("Generic parameter is not fully defined. Owner type:[{0}], parameter name:[{1}], token:[{2}], isReturn:{3}", s, s1, mtoken, isReturn);
							}
							ownerType = GetTypeByName(node, s, out additionalInfo);
							if (ownerType == null)
							{
								throw new SerializerException("Cannot find owner type for generic parameter. Owner type:[{0}], parameter name:[{1}], token:[{2}], isReturn:{3}", s, s1, mtoken, isReturn);
							}
							else
							{
								MethodBase mif = null;
								if (mtoken != 0)
								{
									mif = getMethodByToken(ownerType, mtoken, methodSignature);
									if (mif == null)
									{
										mif = getMethodByToken(ownerType, mtoken, methodSignature, BindingFlags.NonPublic);
										if (mif == null)
										{
											mif = getMethodByToken(ownerType, mtoken, methodSignature, BindingFlags.Static);
											if (mif == null)
											{
												mif = getMethodByToken(ownerType, mtoken, methodSignature, BindingFlags.Static | BindingFlags.NonPublic);
												if (mif == null)
												{
													throw new SerializerException("Cannot find declaring method for generic parameter. Owner type:[{0}], parameter name:[{1}], token:[{2}]", s, s1, mtoken);
												}
											}
										}
									}
									if (isReturn != 0)
									{
										MethodInfo mifo = mif as MethodInfo;
										type = mifo.ReturnType;
									}
									else
									{
										Type[] tcs = mif.GetGenericArguments();
										if (tcs == null || tcs.Length == 0)
										{
											throw new SerializerException("Owner method does not have generic parameters. Owner type:[{0}], parameter name:[{1}], token:[{2}], method:[{3}]", s, s1, mtoken, VPLUtil.GetMethodSignature(mif));
										}
										else
										{
											for (int i = 0; i < tcs.Length; i++)
											{
												if (string.CompareOrdinal(tcs[i].Name, s1) == 0)
												{
													type = tcs[i];
													break;
												}
											}
											if (type == null)
											{
												throw new SerializerException("Owner method does not have matching generic parameter name. Owner type:[{0}], parameter name:[{1}], token:[{2}], method:[{3}]", s, s1, mtoken, VPLUtil.GetMethodSignature(mif));
											}
										}
									}
								}
								else
								{
									if (ownerType.ContainsGenericParameters)
									{
										Type[] tcs = ownerType.GetGenericArguments();
										if (tcs == null || tcs.Length == 0)
										{
											throw new SerializerException("X - Owner type does not have generic parameters. Owner type:[{0}], parameter name:[{1}]. path:[{2}]", s, s1, GetPath(node));
										}
										for (int i = 0; i < tcs.Length; i++)
										{
											if (string.CompareOrdinal(tcs[i].Name, s1) == 0)
											{
												type = tcs[i];
												break;
											}
										}
										if (type == null)
										{
											throw new SerializerException("X - Owner type does not have generic parameter name [{1}]. Owner type:[{0}], parameter name:[{1}]", s, s1);
										}
									}
									else
									{
										Type gt = ownerType.GetGenericTypeDefinition();
										Type[] tps = gt.GetGenericArguments();
										Type[] tcs = ownerType.GetGenericArguments();
										if (tcs == null || tcs.Length == 0)
										{
											throw new SerializerException("Owner type does not have generic parameters. Owner type:[{0}], parameter name:[{1}]. path:[{2}]", s, s1, GetPath(node));
										}
										if (tps == null || tps.Length == 0)
										{
											throw new SerializerException("Owner type does not have generic arguments. Owner type:[{0}], parameter name:[{1}]. path:[{2}]", s, s1, GetPath(node));
										}
										if (tps.Length != tcs.Length)
										{
											throw new SerializerException("Owner type does not have matching generic parameters. Owner type:[{0}], parameter name:[{1}]. path:[{2}]", s, s1, GetPath(node));
										}
										for (int i = 0; i < tcs.Length; i++)
										{
											if (string.CompareOrdinal(tps[i].Name, s1) == 0)
											{
												type = tcs[i];
												break;
											}
										}
										if (type == null)
										{
											throw new SerializerException("Owner type does not have generic parameter name [{1}]. Owner type:[{0}], parameter name:[{1}]", s, s1);
										}
									}
								}
							}
						}
						else
						{
							//get assembly qualified type name
							s = GetAttribute(typeNode, XMLATT_FullTypeName);
							type = Type.GetType(s);
						}
					}
				}
			}
			if (type == null && typeNode != null && !DisableTypePathAdjust)
			{
				string sFullName = GetAttribute(typeNode, XMLATT_FullTypeName);
				if (!string.IsNullOrEmpty(sFullName))
				{
					string sPath = null;
					string fname = null;
					int pos = sFullName.IndexOf("Version=", StringComparison.Ordinal);
					if (pos > 0)
					{
						fname = sFullName.Substring(0, pos);
					}
					while (type == null)
					{
						DialogFilePath dlg = new DialogFilePath();
						dlg.LoadData("{empty}", sFullName);
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							sPath = dlg.AdjustedPath;
							VPLUtil.SetupExternalDllResolve(Path.GetDirectoryName(sPath));
							try
							{
								Assembly a = Assembly.LoadFrom(sPath);
								type = a.GetType(sFullName);
								if (type == null)
								{
									Type[] ts = a.GetExportedTypes();
									if (ts != null && ts.Length > 0)
									{
										for (int i = 0; i < ts.Length; i++)
										{
											if (string.CompareOrdinal(ts[i].AssemblyQualifiedName, sFullName) == 0)
											{
												type = ts[i];
												break;
											}
										}
										if (type == null)
										{
											if (!string.IsNullOrEmpty(fname))
											{
												for (int i = 0; i < ts.Length; i++)
												{
													if (ts[i].AssemblyQualifiedName.StartsWith(fname, StringComparison.Ordinal))
													{
														type = ts[i];
														SetAttribute(typeNode, XMLATT_FullTypeName, ts[i].AssemblyQualifiedName);
														break;
													}
												}
											}
										}
									}
								}
								if (type == null)
								{
									MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
										"Type [{0}] does not exist in [{1}]", sFullName, sPath), "Adjust Assembly location", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								}
							}
							catch (Exception err)
							{
								MessageBox.Show(err.Message, "Adjust assembly location", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							finally
							{
								VPLUtil.RemoveExternalDllResolve();
							}
						}
						else
						{
							if (VPLUtil.Shutingdown)
							{
								return null;
							}
							break;
						}
					}
					if (type != null && File.Exists(sPath))
					{
						SetAttribute(typeNode, XMLATT_filename, sPath);
						saveDoc(typeNode);
					}
				}
			}
			return type;
		}
		private static void saveDoc(XmlNode typeNode)
		{
			string fileDoc = null;
			if (!string.IsNullOrEmpty(typeNode.OwnerDocument.BaseURI))
			{
				string sf = "file:///";
				if (typeNode.OwnerDocument.BaseURI.StartsWith(sf, StringComparison.OrdinalIgnoreCase))
				{
					fileDoc = typeNode.OwnerDocument.BaseURI.Substring(sf.Length);
				}
			}
			else
			{
				fileDoc = GetFilenameAttribute(typeNode.OwnerDocument.DocumentElement);
			}
			if (!string.IsNullOrEmpty(fileDoc) && File.Exists(fileDoc))
			{
				typeNode.OwnerDocument.Save(fileDoc);
			}
		}
		/// <summary>
		/// t must be an array
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string FormArrayTypeName(Type t)
		{
			if (t == null)
			{
				throw new SerializerException("Calling FormArrayTypeName with a null type");
			}
			if (!t.IsArray)
			{
				throw new SerializerException("Calling FormArrayTypeName with a non-array type:[{0}]", t.AssemblyQualifiedName);
			}
			StringBuilder sb = new StringBuilder("[");
			int n = t.GetArrayRank();
			if (n > 1)
			{
				sb.Append(new string(',', n - 1));
			}
			sb.Append("]");
			return sb.ToString();
		}
		/// <summary>
		/// get key name for the type. SetLibTypeAttribute writes the name to nodes.
		/// an Item node will be generated for non-global type; additional nodes will be created for generic type or array type will  
		/// </summary>
		/// <param name="node"></param>
		/// <param name="type"></param>
		/// <param name="extraTypes"></param>
		/// <returns>type key for the type</returns>
		public static string GetTypeName(XmlNode node, Type type, params object[] extraTypes)
		{
			string name = null;
			if (type == null)
			{
				throw new XmlSerializerException("Calling GetTypeName with null type");
			}
			//case 0: class type
			if (VPLUtil.IsDynamicAssembly(type.Assembly))
			{
				object[] vs = type.GetCustomAttributes(typeof(ClassIdAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					ClassIdAttribute cia = vs[0] as ClassIdAttribute;
					if (cia != null)
					{
						name = string.Format(CultureInfo.InvariantCulture,
							"class{0}", cia.ClassID);
					}
				}
			}
			if (string.IsNullOrEmpty(name))
			{
				//case 1: type == XClass<t> or XControl<t> return XClass[key for t] or XControl[key for t]
				EnumXType xtype = VPLUtil.IsXType(type);
				if (xtype != EnumXType.None)
				{
					Type t = VPLUtil.GetObjectType(type); //get wrapped type
					//extraTypes is passed in by SetLibTypeAttribute for passing back related types
					name = GetTypeName(node, t, extraTypes);
					if (extraTypes != null && extraTypes.Length > 0)
					{
						Dictionary<string, Type> tps = extraTypes[0] as Dictionary<string, Type>;
						if (tps != null)
						{
							tps.Add(name, t);
						}
					}
					//on returning to SetLibTypeAttribute, it will save type t
					if (xtype == EnumXType.Class)
					{
						name = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}]]", XClassNamePrefix, name);
					}
					else
					{
						name = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}]]", XControlNamePrefix, name);
					}
				}
			}
			if (string.IsNullOrEmpty(name))
			{
				//case 2: global type
				foreach (KeyValuePair<string, Type> kv in _knownTypes)
				{
					if (kv.Value.Equals(type))
					{
						name = kv.Key;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(name))
			{
				//case 3: local type
				XmlNode tNode;
				XmlNode typesNode = null;
				typesNode = node.OwnerDocument.DocumentElement.SelectSingleNode(XML_TYPES);
				if (typesNode == null)
				{
					typesNode = node.OwnerDocument.CreateElement(XML_TYPES);
					node.OwnerDocument.DocumentElement.AppendChild(typesNode);
				}
				//case 3.1: generic method parameter type or return type
				if (string.IsNullOrEmpty(type.AssemblyQualifiedName) && type.IsGenericType)
				{
					IMethodMeta imm = null;
					if (extraTypes != null && extraTypes.Length > 0)
					{
						imm = extraTypes[0] as IMethodMeta;
					}
					if (imm != null)
					{
						//use type, method signature to identify the method, from the method get return type and parameter types 
						MethodInfo mif = imm.GetMethodInfo();
						if (type.Equals(mif.ReturnType))
						{
							string dname = XmlUtil.GetTypeName(typesNode, mif.DeclaringType);
							string methodSignatue = VPLUtil.GetMethodSignature(mif);
							int mtoken = mif.MetadataToken;
							tNode = typesNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							 "{0}[@{1}='{2}' and @{3}='1' and @{4}='{5}']", XML_Item, XMLATT_ownerTypeName, dname, XMLATT_isreturn, XMLATT_MetaToken, mtoken));
							if (tNode == null)
							{
								if (!string.IsNullOrEmpty(methodSignatue))
								{
									tNode = typesNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
									"{0}[@{1}='{2}' and @{3}='1' and @{4}='{5}']", XML_Item, XMLATT_ownerTypeName, dname, XMLATT_isreturn, XMLATT_MethodSignature, methodSignatue));
								}
							}
							if (tNode != null)
							{
								name = GetAttribute(tNode, XMLATT_TYPE);
								if (string.IsNullOrEmpty(name))
								{
									throw new SerializerException("Type item node missing type attribute:[{0}]", GetPath(tNode));
								}
							}
							else
							{
								//not found, try to add it
								//create a unique name using type.Name as the base name
								name = CreateNewTypeName(type.Name, node);
								tNode = node.OwnerDocument.CreateElement(XML_Item);
								SetAttribute(tNode, XMLATT_ownerTypeName, dname);
								SetAttribute(tNode, XMLATT_isreturn, "1");
								SetAttribute(tNode, XMLATT_MetaToken, mtoken);
								if (!string.IsNullOrEmpty(methodSignatue))
								{
									SetAttribute(tNode, XMLATT_MethodSignature, methodSignatue);
								}
								SetAttribute(tNode, XMLATT_TYPE, name);
								typesNode.AppendChild(tNode);
							}
						}
						else
						{
							throw new SerializerException("Method meta data for type [{0}] does not match [{1}]", type, extraTypes[0]);
						}
					}
				}
				if (string.IsNullOrEmpty(name))
				{
					//case 3.2: type parameter
					if (type.IsGenericParameter)
					{
						if (type.DeclaringType == null && type.ReflectedType == null)
						{
							throw new SerializerException("Generic parameter [{0}] missing DeclaringType or ReflectedType", type.Name);
						}
						string dname;
						if (type.DeclaringType != null)
						{
							dname = GetTypeName(node, type.DeclaringType);
						}
						else
						{
							dname = GetTypeName(node, type.ReflectedType);
						}
						int mtoken = 0;
						string methodSignatue = string.Empty;
						if (type.DeclaringMethod != null)
						{
							mtoken = type.DeclaringMethod.MetadataToken;
							methodSignatue = VPLUtil.GetMethodSignature(type.DeclaringMethod);
						}
						//find the type node by attributes: (ownerTypeName == dname and typeName == type.Name)
						//if found then XMLATT_TYPE is the name for this type
						tNode = typesNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}[@{1}='{2}' and @{3}='{4}' and @{5}='{6}']", XML_Item, XMLATT_ownerTypeName, dname, XMLATT_typeName, type.Name, XMLATT_MetaToken, mtoken));
						if (tNode == null)
						{
							if (!string.IsNullOrEmpty(methodSignatue))
							{
								tNode = typesNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}[@{1}='{2}' and @{3}='{4}' and @{5}='{6}']", XML_Item, XMLATT_ownerTypeName, dname, XMLATT_typeName, type.Name, XMLATT_MethodSignature, methodSignatue));
							}
						}
						if (tNode != null)
						{
							name = GetAttribute(tNode, XMLATT_TYPE);
						}
						else
						{
							//not found, try to add it
							//create a unique name using type.Name as the base name
							name = CreateNewTypeName(type.Name, node);
							//
							tNode = node.OwnerDocument.CreateElement(XML_Item);
							SetAttribute(tNode, XMLATT_ownerTypeName, dname);
							SetAttribute(tNode, XMLATT_typeName, type.Name);
							SetAttribute(tNode, XMLATT_MetaToken, mtoken);
							if (!string.IsNullOrEmpty(methodSignatue))
							{
								SetAttribute(tNode, XMLATT_MethodSignature, methodSignatue);
							}
							SetAttribute(tNode, XMLATT_TYPE, name);
							typesNode.AppendChild(tNode);
						}
					}
				}
				if (string.IsNullOrEmpty(name))
				{
					//case 3.3: array type
					if (type.IsArray)
					{
						//try to see if it is an array of a known type
						Type ti = type.GetElementType();
						if (ti == null)
						{
							throw new SerializerException("Cannot get element type for array [{0}]", type.AssemblyQualifiedName);
						}
						if (ti.Equals(type))
						{
							throw new SerializerException("Array item type [{0}] is the same with the array type [{1}].", ti.AssemblyQualifiedName, type.AssemblyQualifiedName);
						}
						string aName = GetTypeName(node, ti);
						int rnk = type.GetArrayRank();
						XmlNodeList aNodes = typesNode.SelectNodes(string.Format(CultureInfo.InvariantCulture,
							"{0}[@{1}='{2}']", XmlTags.XML_Item, XMLATT_arrayItemType, aName));
						for (int i = 0; i < aNodes.Count; i++)
						{
							if (XmlUtil.GetAttributeInt(aNodes[i], XMLATT_rank) == rnk)
							{
								name = XmlUtil.GetAttribute(aNodes[i], XMLATT_TYPE);
								break;
							}
						}
						if (string.IsNullOrEmpty(name))
						{
							//not found. add it.
							name = CreateNewTypeName(type.Name, node);
							tNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
							typesNode.AppendChild(tNode);
							XmlUtil.SetAttribute(tNode, XMLATT_TYPE, name);
							XmlUtil.SetAttribute(tNode, XMLATT_arrayItemType, aName);
							XmlUtil.SetAttribute(tNode, XMLATT_rank, rnk);
						}
					}
				}
				if (string.IsNullOrEmpty(name))
				{
					//case 3.4: concret generic type
					if (type.IsGenericType && !type.ContainsGenericParameters)
					{
						//get generic definition first
						Type gt = type.GetGenericTypeDefinition();
						if (gt == null)
						{
							throw new XmlSerializerException("Error getting generic type definition for type [{0}]", type.Name);
						}
						string gtDefName = GetTypeName(node, gt);
						if (string.IsNullOrEmpty(gtDefName))
						{
							//this case should not have happened because a new name will be generated.
							//thus if it happens then it is an exception
							throw new XmlSerializerException("Generic type [{0}] for type [{1}] cannot be recorded", gt.Name, type.Name);
						}
						Type[] argTypes = type.GetGenericArguments();
						int argCount = 0;
						if (argTypes != null)
						{
							argCount = argTypes.Length;
						}
						XmlNodeList nodeConcTypes = node.OwnerDocument.DocumentElement.SelectNodes(string.Format(CultureInfo.InvariantCulture,
							"{0}/{1}[@{2}='{3}']",
							XML_TYPES, XmlTags.XML_Item, XMLATT_typeDefinition, gtDefName));
						foreach (XmlNode ndType in nodeConcTypes)
						{
							XmlNodeList ndArgcs = ndType.SelectNodes(XmlTags.XML_TYPEParameter);
							if (ndArgcs.Count == argCount)
							{
								bool match = true;
								for (int k = 0; k < argCount; k++)
								{
									Type argT = GetLibTypeAttribute(ndArgcs[k]);
									if (argT == null || !argT.Equals(argTypes[k]))
									{
										match = false;
										break;
									}
								}
								if (match)
								{
									name = XmlUtil.GetAttribute(ndType, XMLATT_TYPE);
									break;
								}
							}
						}
						if (string.IsNullOrEmpty(name))
						{
							//not found, create it
							name = CreateNewTypeName(type.Name, node);
							tNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
							typesNode.AppendChild(tNode);
							XmlUtil.SetAttribute(tNode, XmlTags.XMLATT_type, name);
							XmlUtil.SetAttribute(tNode, XMLATT_typeDefinition, gtDefName);
							for (int k = 0; k < argCount; k++)
							{
								string argname = GetTypeName(node, argTypes[k]);
								XmlNode argNode = node.OwnerDocument.CreateElement(XmlTags.XML_TYPEParameter);
								tNode.AppendChild(argNode);
								XmlUtil.SetAttribute(argNode, XmlTags.XMLATT_type, argname);
							}
						}
					}
				}
				if (string.IsNullOrEmpty(name))
				{
					//case 3.5: direct type
					//find the type node by fully qualified type name, including generic type definition type
					tNode = typesNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}[@{1}='{2}']", XML_Item, XMLATT_FullTypeName, type.AssemblyQualifiedName));
					if (tNode != null)
					{
						name = XmlUtil.GetLibTypeAttributeString(tNode);
					}
					else
					{
						//not found, try to add it
						if (string.IsNullOrEmpty(type.AssemblyQualifiedName))
						{
							throw new SerializerException("Type [{0}] is not resolved", type);
						}
						string typeFulleName = type.AssemblyQualifiedName;
						name = CreateNewTypeName(type.Name, node);
						tNode = node.OwnerDocument.CreateElement(XML_Item);
						SetAttribute(tNode, XMLATT_FullTypeName, typeFulleName);
						SetAttribute(tNode, XMLATT_TYPE, name);
						typesNode.AppendChild(tNode);
						//
						if (!type.Assembly.GlobalAssemblyCache)
						{
							string s = System.IO.Path.GetDirectoryName(type.Assembly.Location);
							if (string.IsNullOrEmpty(baseDir))
							{
								baseDir = AppDomain.CurrentDomain.BaseDirectory;
								if (baseDir.EndsWith("\\"))
								{
									baseDir = baseDir.Substring(0, baseDir.Length - 1);
								}
							}
							if (string.Compare(baseDir, s, StringComparison.OrdinalIgnoreCase) != 0)
							{
								SetAttribute(tNode, XMLATT_filename, type.Assembly.Location);
							}
						}
						if (type.IsGenericType)
						{
							//type is a definition
							Type[] tparems = type.GetGenericArguments();
							if (tparems != null)
							{
								for (int i = 0; i < tparems.Length; i++)
								{
									XmlNode pNode = node.OwnerDocument.DocumentElement.SelectSingleNode(
										string.Format(CultureInfo.InvariantCulture,
										"{0}/{1}[@{2}='{3}' and @{4}='{5}']",
										XML_TYPES, XmlTags.XML_Item, XMLATT_ownerTypeName, name, XMLATT_typeName, tparems[i].Name));
									if (pNode == null)
									{
										string spname = CreateNewTypeName(tparems[i].Name, node);
										pNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
										typesNode.AppendChild(pNode);
										XmlUtil.SetAttribute(pNode, XMLATT_TYPE, spname);
										XmlUtil.SetAttribute(pNode, XMLATT_ownerTypeName, name);
										XmlUtil.SetAttribute(pNode, XMLATT_typeName, tparems[i].Name);
									}
								}
							}
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(name))
			{
				Guid classGuid = GetAttributeGuid(node.OwnerDocument.DocumentElement, XmlTags.XMLATT_guid);
				if (classGuid != Guid.Empty)
				{
					if (_dynamicTypes == null)
					{
						_dynamicTypes = new Dictionary<Guid, Dictionary<string, Type>>();
					}
					Dictionary<string, Type> typeLst;
					if (!_dynamicTypes.TryGetValue(classGuid, out typeLst))
					{
						typeLst = new Dictionary<string, Type>();
						_dynamicTypes.Add(classGuid, typeLst);
					}
					if (!typeLst.ContainsKey(name))
					{
						typeLst.Add(name, type);
					}
				}
			}
			return name;
		}
		public static Type GetKnownType(string name)
		{
			Type t;
			if (_knownTypes.TryGetValue(name, out t))
			{
				return t;
			}
			return null;
		}
		public static void AddKnownType(string name, Type type)
		{
			Type t;
			if (_knownTypes.TryGetValue(name, out t))
			{
				if (!type.Equals(t))
				{
					MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"Change well known type from [{0}] to [{1}]",
						t.AssemblyQualifiedName, type.AssemblyQualifiedName), "Add known type", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					_knownTypes[name] = type;
				}
			}
			else
			{
				_knownTypes.Add(name, type);
			}
		}
		public static void AddKnownTypes(string toolboxCfg)
		{
			IList<Type> lst = GetTypes(toolboxCfg, false);
			foreach (Type ti in lst)
			{
				AddKnownType(ti.Name, ti);
			}
		}
		public static Dictionary<string, IList<Type>> GetTypesInCategories(string toolboxCfg, bool toolbox)
		{
			Dictionary<string, IList<Type>> cats = new Dictionary<string, IList<Type>>();
			XmlDocument doc = new XmlDocument();
			doc.Load(toolboxCfg);
			if (doc.DocumentElement != null)
			{
				string sDir = Path.GetDirectoryName(toolboxCfg);
				XmlNodeList catNodes = doc.DocumentElement.SelectNodes("Category");
				foreach (XmlNode ndc in catNodes)
				{
					string cname = XmlUtil.GetNameAttribute(ndc);
					if (!string.IsNullOrEmpty(cname))
					{
						List<Type> lst = new List<Type>();
						cats.Add(cname, lst);
						loadTypes(lst, ndc, sDir, toolbox);
					}
				}
			}
			return cats;
		}
		private static void loadTypes(List<Type> lst, XmlNode ndParent, string sDir, bool toolbox)
		{
			XmlNodeList fileNodes = ndParent.SelectNodes(XmlTags.XML_File);
			foreach (XmlNode nd in fileNodes)
			{
				string fname = XmlUtil.GetNameAttribute(nd);
				if (string.IsNullOrEmpty(fname))
				{
					XmlNodeList items = nd.SelectNodes(XmlTags.XML_Item);
					foreach (XmlNode ni in items)
					{
						if (toolbox)
						{
							if (GetAttributeInt(ni, "tool") == 0)
							{
								continue;
							}
						}
						string ids;
						Type ti = XmlUtil.GetLibTypeAttribute(ni, out ids);
						if (ti != null)
						{
							lst.Add(ti);
						}

					}
				}
				else
				{
					string sp = Path.Combine(sDir, fname);
					if (File.Exists(sp))
					{
						Assembly a = Assembly.LoadFile(sp);
						XmlNodeList items = nd.SelectNodes(XmlTags.XML_Item);
						foreach (XmlNode ni in items)
						{
							if (toolbox)
							{
								if (GetAttributeInt(ni, "tool") == 0)
								{
									continue;
								}
							}
							string sn = XmlUtil.GetNameAttribute(ni);
							if (!string.IsNullOrEmpty(sn))
							{
								Type ti = a.GetType(sn);
								if (ti != null)
								{
									lst.Add(ti);
								}
							}
							else
							{
								string ids;
								Type ti = XmlUtil.GetLibTypeAttribute(ni, out ids);
								if (ti != null)
								{
									lst.Add(ti);
								}
							}
						}
					}
				}
			}
		}
		public static IList<Type> GetTypes(string toolboxCfg, bool toolbox)
		{
			List<Type> lst = new List<Type>();
			XmlDocument doc = new XmlDocument();
			doc.Load(toolboxCfg);
			if (doc.DocumentElement != null)
			{
				string sDir = Path.GetDirectoryName(toolboxCfg);
				loadTypes(lst, doc.DocumentElement, sDir, toolbox);
			}
			SortedList<string, Type> sl = new SortedList<string, Type>();
			foreach (Type t in lst)
			{
				sl.Add(t.Name, t);
			}
			lst.Clear();
			IEnumerator<KeyValuePair<string, Type>> en = sl.GetEnumerator();
			while (en.MoveNext())
			{
				lst.Add(en.Current.Value);
			}
			return lst;
		}
		#endregion
		static public bool DisableTypePathAdjust = false;
		static public fnCreateDesignTimeType CreateDesignTimeType;
		static public void CopyXmlNode(XmlNode source, XmlNode target)
		{
			if (source.Attributes != null && source.Attributes.Count > 0)
			{
				foreach (XmlAttribute xa1 in source.Attributes)
				{
					XmlAttribute xa2 = target.OwnerDocument.CreateAttribute(xa1.Name);
					xa2.Value = xa1.Value;
					target.Attributes.Append(xa2);
				}
			}
			if (source.ChildNodes != null && source.ChildNodes.Count > 0)
			{
				foreach (XmlNode nd1 in source.ChildNodes)
				{
					XmlNode n2 = target.OwnerDocument.ImportNode(nd1, true);
					target.AppendChild(n2);
				}
			}
		}
		static public bool NotForLightRead(AttributeCollection attrs)
		{
			if (attrs != null)
			{
				foreach (Attribute a in attrs)
				{
					NotForLightReadAttribute sa = a as NotForLightReadAttribute;
					if (sa != null)
					{
						return !sa.ForLightRead;
					}
				}
			}
			return false;
		}
		static public string GetFileMapping(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				if (_dllFileMapping != null)
				{
					string s;
					if (_dllFileMapping.TryGetValue(file.ToLowerInvariant(), out s))
					{
						return s;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// checking for special cases
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		static public bool ShouldSaveProperty(object obj, string propertyName)
		{
			TreeNode tn = obj as TreeNode;
			if (tn != null)
			{
				if (string.CompareOrdinal(propertyName, "Nodes") == 0)
				{
					return true;
				}
			}
			else
			{
				if (string.CompareOrdinal(propertyName, "CustomValues") == 0)
				{
					if (obj.GetType().GetInterface("IWebClientComponent") != null)
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Simple helper method that returns true if the given type converter supports
		/// two-way conversion of the given type.
		/// </summary>
		static public bool GetConversionSupported(TypeConverter converter, Type conversionType)
		{
			return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
		}
		/// <summary>
		/// get all libType attributes
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		static public XmlNodeList GetTypeAttributeList(XmlDocument doc)
		{
			return doc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "//*[@{0}]", XMLATT_TYPE));
		}
		static public XmlNode FindTypeNode(XmlNodeList list, Type type)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Type t = XmlUtil.GetLibTypeAttribute(list[i]);
				if (t != null && VPLUtil.IsSameType(t, type))
				{
					return list[i];
				}
			}
			return null;
		}
		static public UInt64 MakeDDWord(UInt32 LoWord, UInt32 HiWord)
		{
			return ((((UInt64)LoWord & 0x00000000ffffffff)) | (((UInt64)(HiWord & 0x00000000ffffffff) << 32)));
		}
		static public void ParseDDWord(UInt64 ddword, out UInt32 LoWord, out UInt32 HiWord)
		{
			LoWord = (UInt32)(ddword & 0x00000000ffffffff);
			HiWord = (UInt32)((ddword & 0xffffffff00000000) >> 32);
		}
		public static bool IsValueType(Type t)
		{
			if (t.IsValueType)
				return true;
			if (t.Equals(typeof(string)))
				return true;
			return false;
		}
		public static string GetPath(XmlNode node)
		{
			StringBuilder sb = new StringBuilder();
			List<string> names = new List<string>();
			while (node != null)
			{
				string s = GetNameAttribute(node);
				if (!string.IsNullOrEmpty(s))
				{
					names.Insert(0, string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}[@name='{1}']", node.Name, s));
				}
				else
				{
					names.Insert(0, node.Name);
				}
				node = node.ParentNode;
			}
			if (names.Count > 0)
			{
				sb.Append(names[0]);
			}
			for (int i = 1; i < names.Count; i++)
			{
				sb.Append("/");
				sb.Append(names[i]);
			}
			return sb.ToString();
		}
		public static bool HasAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					return true;
				}
			}
			return false;
		}
		public static void RemoveAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					node.Attributes.Remove(xa);
				}
			}
		}
		public static void SetAttribute(XmlNode node, string name, string value)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa == null)
				{
					xa = node.OwnerDocument.CreateAttribute(name);
					node.Attributes.Append(xa);
				}
				if (value == null)
					xa.Value = string.Empty;
				else
					xa.Value = value;
			}
		}
		public static string GetAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new XmlSerializerException("getting attribute missing name. Xml path:{0}", GetPath(node));
				}
				if (node.Attributes != null)
				{
					XmlAttribute xa = node.Attributes[name];
					if (xa != null)
					{
						return xa.Value;
					}
				}
			}
			return "";
		}
		public static Font GetAttributeFont(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (!string.IsNullOrEmpty(xa.Value))
					{
						TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
						return (Font)converter.ConvertFromInvariantString(xa.Value);
					}
				}
			}
			return new Font("Times New Roman", 8);
		}
		public static int GetAttributeInt(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return 0;
					return Convert.ToInt32(xa.Value);
				}
			}
			return 0;
		}
		public static byte GetAttributeByte(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return 0;
					return Convert.ToByte(xa.Value);
				}
			}
			return 0;
		}
		public static uint GetAttributeUInt(XmlNode node, string name)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return 0;
					return Convert.ToUInt32(xa.Value);
				}
			}
			return 0;
		}
		public static UInt64 GetAttributeUInt64(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return 0;
					return Convert.ToUInt64(xa.Value);
				}
			}
			return (UInt64)0;
		}
		public static bool GetAttributeBoolDefFalse(XmlNode node, string name)
		{
			if (node != null)//&& node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return false;
					return Convert.ToBoolean(xa.Value);
				}
			}
			return false;
		}
		public static bool GetAttributeBoolDefTrue(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return true;
					return Convert.ToBoolean(xa.Value);
				}
			}
			return true;
		}
		public static Guid GetAttributeGuid(XmlNode node, string name)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (string.IsNullOrEmpty(xa.Value))
						return Guid.Empty;
					return new Guid(xa.Value);
				}
			}
			return Guid.Empty;
		}
		public static T GetAttributeEnum<T>(XmlNode node, string name)
		{
			string s = GetAttribute(node, name);
			if (!string.IsNullOrEmpty(s))
			{
				return (T)Enum.Parse(typeof(T), s);
			}
			else
			{
				Array a = Enum.GetValues(typeof(T));
				if (a != null && a.Length > 0)
				{
					return (T)a.GetValue(0);
				}
				return default(T);
			}
		}
		public static Type GetLibTypeAttribute(XmlNode node)
		{
			string additionalInfo;
			string name = GetLibTypeAttributeString(node);
			return GetTypeByName(node, name, out additionalInfo);
		}
		public static Type GetLibTypeAttribute(XmlNode node, out string additionalInfo)
		{
			Type t = GetTypeByName(node, GetLibTypeAttributeString(node), out additionalInfo);
			return t;
		}
		public static string GetLibTypeAttributeString(XmlNode node)
		{
			return GetAttribute(node, XMLATT_TYPE);
		}
		/// <summary>
		/// set type attribute to the node
		/// </summary>
		/// <param name="node"></param>
		/// <param name="type"></param>
		public static void SetLibTypeAttribute(XmlNode node, Type type, params object[] owner)
		{
			if (string.IsNullOrEmpty(type.AssemblyQualifiedName) && type.IsGenericType)
			{
				string name = XmlUtil.GetTypeName(node, type, owner);
				SetAttribute(node, XMLATT_TYPE, name);
			}
			else
			{
				Dictionary<string, Type> tps = new Dictionary<string, Type>();
				string sName = GetTypeName(node, type, tps);
				SetAttribute(node, XMLATT_TYPE, sName);
			}
		}
		public static string GetNameAttribute(XmlNode node)
		{
			return GetAttribute(node, XMLATT_NAME);
		}
		public static void SetNameAttribute(XmlNode node, string name)
		{
			SetAttribute(node, XMLATT_NAME, name);
		}
		public static void SetAttribute(XmlNode node, string name, object val)
		{
			SetAttribute(node, name, val.ToString());
		}
		public static void SetAttributeFont(XmlNode node, string name, Font val)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
			SetAttribute(node, name, converter.ConvertToInvariantString(val));
		}
		public static XmlNode CreateNewElement(XmlNode nodeParent, string name)
		{
			XmlNode node = nodeParent.OwnerDocument.CreateElement(name);
			nodeParent.AppendChild(node);
			return node;
		}
		public static XmlNode CreateSingleNewElement(XmlNode nodeParent, string name)
		{
			XmlNode node = nodeParent.SelectSingleNode(name);
			if (node == null)
			{
				node = nodeParent.OwnerDocument.CreateElement(name);
				nodeParent.AppendChild(node);
			}
			return node;
		}

		public static string GetSingleCDataValue(XmlNode node, string name)
		{
			XmlNode nd = node.SelectSingleNode(name);
			if (nd != null)
			{
				foreach (XmlNode n in nd.ChildNodes)
				{
					XmlCDataSection cd = n as XmlCDataSection;
					if (cd != null)
					{
						return cd.Value;
					}
				}
			}
			return string.Empty;
		}
		public static void SetSingleCDataValue(XmlNode node, string name, string value)
		{
			XmlCDataSection cd;
			XmlNode nd = CreateSingleNewElement(node, name);
			foreach (XmlNode n in nd.ChildNodes)
			{
				cd = n as XmlCDataSection;
				if (cd != null)
				{
					cd.Value = value;
					return;
				}
			}
			cd = node.OwnerDocument.CreateCDataSection(value);
			nd.AppendChild(cd);
		}
		public static void RemoveChildNode(XmlNode nodeParent, string name)
		{
			XmlNode node = nodeParent.SelectSingleNode(name);
			if (node != null)
			{
				nodeParent.RemoveChild(node);
			}
		}
		public static string GetSubNodeText(XmlNode nodeParent, string name)
		{
			XmlNode node = nodeParent.SelectSingleNode(name);
			if (node != null)
			{
				return node.InnerText;
			}
			return null;
		}
		public static IList<string> GetLanguages(XmlNode rootNode)
		{
			List<string> languageNames = new List<string>();
			XmlNodeList ns = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}", XmlTags.XML_LANGUAGES, XmlTags.XML_Item));
			foreach (XmlNode nd in ns)
			{
				languageNames.Add(XmlUtil.GetNameAttribute(nd));
			}
			return languageNames;
		}
		public static void UpdateLanguages(XmlNode rootNode, StringCollection languages)
		{
			if (languages != null)
			{
				XmlNode ns = XmlUtil.CreateSingleNewElement(rootNode, XmlTags.XML_LANGUAGES);
				ns.RemoveAll();
				foreach (string s in languages)
				{
					if (!string.IsNullOrEmpty(s))
					{
						XmlNode nd = ns.OwnerDocument.CreateElement(XmlTags.XML_Item);
						ns.AppendChild(nd);
						XmlUtil.SetNameAttribute(nd, s);
					}
				}
			}
		}
		public static bool RemoveLanguage(XmlNode rootNode, string name)
		{
			XmlNode nd = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@name='{2}']",
				XmlTags.XML_LANGUAGES, XmlTags.XML_Item, name));
			if (nd != null)
			{
				nd.ParentNode.RemoveChild(nd);
				return true;
			}
			return false;
		}
	}
}
