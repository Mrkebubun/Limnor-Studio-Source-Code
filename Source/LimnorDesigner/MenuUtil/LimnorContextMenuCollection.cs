/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Xml;
using LimnorDesigner;
using System.Windows.Forms;
using VPL;
using LimnorDesigner.Event;
using LimnorDesigner.Property;
using ProgElements;
using System.ComponentModel;
using MathExp;
using Limnor.Drawing2D;
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using Limnor.WebBuilder;
using System.Globalization;
using Limnor.WebServerBuilder;
using Limnor.PhpComponents;
using LimnorDesigner.Web;
using LimnorDesigner.Interface;
using WindowsUtility;

namespace LimnorDesigner.MenuUtil
{
	/// <summary>
	/// context menu for following types of objects:
	/// Root class: ClassPoint
	/// Custom-type instance: ClassInstancePointer/MemberComponentIdCust
	///             Declarer -> ClassPoint
	/// Lib-type instance: MemberComponentId
	///             ObjectType -> Type
	/// Local-var: LocalVariable
	///     DataTypePointer -> {ClassPointer, TypePointer}
	/// Local-Array-var: ArrayVariable
	///     IClassWrapper
	/// The object is the owner of the P/M/E
	/// To get P/M/E, only three different types: Type, ClassPointer, IClassWrapper
	/// </summary>
	public class LimnorContextMenuCollection
	{
		#region fields and constructors
		private IClass _owner;
		private DataTypePointer _concretType;
		private ILimnorDesignPane _designPane;
		private bool _isStatic;
		private List<MenuItemDataMethod> _primaryMethods;
		private List<MenuItemDataMethod> _secondaryMethods;
		private List<MenuItemDataEvent> _primaryEvents;
		private List<MenuItemDataEvent> _secondaryEvents;
		private List<MenuItemDataProperty> _primaryProperties;
		private List<MenuItemDataProperty> _secondaryProperties;
		private MenuItemDataMethod _selectMethod;
		private MenuItemDataProperty _selectProperty;
		private MenuItemDataEvent _selectEvent;
		//
		private SortedDictionary<string, MethodInfo> methods;
		private SortedDictionary<string, EventInfo> events;
		private SortedDictionary<string, PropertyDescriptor> properties;
		//
		private SortedDictionary<string, MethodClass> methodsC;
		private SortedDictionary<string, EventClass> eventsC;
		private SortedDictionary<string, PropertyClass> propertiesC;
		//
		public const string FILE_EXT_ME = "pme";
		public const string XML_Primary = "Primary";
		public const string XML_Secondary = "Secondary";
		public const string XML_Methods = "Methods";
		public const string XML_Events = "Events";
		public const string XML_Properties = "Properties";
		public const string XML_Method = "Method";
		public const string XML_Event = "Event";
		public const string XML_Property = "Property";
		//
		const int MaxMenuItemCount = 20;
		//
		public LimnorContextMenuCollection(IClass type)
		{
			_owner = type;
			loadData();
		}
		public LimnorContextMenuCollection(IClass type, bool isStatic)
		{
			_isStatic = isStatic;
			_owner = type;
			loadData();
		}
		#endregion
		#region Static interface
		static Dictionary<UInt64, LimnorContextMenuCollection> s_menuCollections;
		static Dictionary<UInt64, LimnorContextMenuCollection> s_menuClassPointerCollections;
		static Dictionary<UInt64, LimnorContextMenuCollection> s_menuWrapperCollections;
		//==============================================================================
		static Dictionary<UInt64, LimnorContextMenuCollection> ss_menuCollections;
		static Dictionary<UInt64, LimnorContextMenuCollection> ss_menuClassPointerCollections;
		static Dictionary<UInt64, LimnorContextMenuCollection> ss_menuWrapperCollections;
		//==============================================================================
		public static LimnorContextMenuCollection ReloadMenuCollection(IClass type)
		{
			RemoveMenuCollection(type);
			return GetMenuCollection(type);
		}
		/// <summary>
		/// get context menu to be used in form designer and method designer
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static LimnorContextMenuCollection GetMenuCollection(IClass type)
		{
			if (!type.IsValid)
			{
				if (type.VariableLibType != null && !typeof(HtmlElement_Base).IsAssignableFrom(type.VariableLibType))
				{
					return null;
				}
			}
			if (type.VariableWrapperType != null)
			{
				LimnorContextMenuCollection cmc;
				if (s_menuWrapperCollections == null)
				{
					s_menuWrapperCollections = new Dictionary<UInt64, LimnorContextMenuCollection>();
					cmc = new LimnorContextMenuCollection(type);
					s_menuWrapperCollections.Add(type.VariableWrapperType.WholeId, cmc);
				}
				else if (s_menuWrapperCollections.TryGetValue(type.VariableWrapperType.WholeId, out cmc))
				{
					cmc.SetOwner(type);
				}
				else
				{
					cmc = new LimnorContextMenuCollection(type);
					s_menuWrapperCollections.Add(type.VariableWrapperType.WholeId, cmc);
				}
				return cmc;
			}
			else if (type.VariableCustomType != null)
			{
				LimnorContextMenuCollection cmc;
				if (s_menuClassPointerCollections == null)
				{
					s_menuClassPointerCollections = new Dictionary<UInt64, LimnorContextMenuCollection>();
					cmc = new LimnorContextMenuCollection(type);
					s_menuClassPointerCollections.Add(type.VariableCustomType.WholeId, cmc);
				}
				else if (s_menuClassPointerCollections.TryGetValue(type.VariableCustomType.WholeId, out cmc))
				{
					cmc.SetOwner(type);
				}
				else
				{
					cmc = new LimnorContextMenuCollection(type);
					s_menuClassPointerCollections.Add(type.VariableCustomType.WholeId, cmc);
				}
				return cmc;
			}
			else if (type.VariableLibType != null)
			{
				LimnorContextMenuCollection cmc;
				if (s_menuCollections == null)
				{
					s_menuCollections = new Dictionary<UInt64, LimnorContextMenuCollection>();
					cmc = new LimnorContextMenuCollection(type);
					s_menuCollections.Add(type.WholeId, cmc);
				}
				else if (s_menuCollections.TryGetValue(type.WholeId, out cmc))
				{
					cmc.SetOwner(type);
				}
				else
				{
					cmc = new LimnorContextMenuCollection(type);
					s_menuCollections.Add(type.WholeId, cmc);
				}
				return cmc;
			}
			else
			{
				return null;
			}
		}
		public static LimnorContextMenuCollection GetStaticMenuCollection(IClass type)
		{
			if (!type.IsValid)
			{
				return null;
			}
			if (type.VariableWrapperType != null)
			{
				LimnorContextMenuCollection cmc;
				if (ss_menuWrapperCollections == null)
				{
					ss_menuWrapperCollections = new Dictionary<UInt64, LimnorContextMenuCollection>();
					cmc = new LimnorContextMenuCollection(type, true);
					ss_menuWrapperCollections.Add(type.VariableWrapperType.WholeId, cmc);
				}
				else if (ss_menuWrapperCollections.TryGetValue(type.VariableWrapperType.WholeId, out cmc))
				{
					cmc.SetOwner(type);
				}
				else
				{
					cmc = new LimnorContextMenuCollection(type, true);
					ss_menuWrapperCollections.Add(type.VariableWrapperType.WholeId, cmc);
				}
				return cmc;
			}
			else if (type.VariableCustomType != null)
			{
				LimnorContextMenuCollection cmc;
				if (ss_menuClassPointerCollections == null)
				{
					ss_menuClassPointerCollections = new Dictionary<UInt64, LimnorContextMenuCollection>();
					cmc = new LimnorContextMenuCollection(type, true);
					ss_menuClassPointerCollections.Add(type.VariableCustomType.WholeId, cmc);
				}
				else if (ss_menuClassPointerCollections.TryGetValue(type.VariableCustomType.WholeId, out cmc))
				{
					cmc.SetOwner(type);
				}
				else
				{
					cmc = new LimnorContextMenuCollection(type, true);
					ss_menuClassPointerCollections.Add(type.VariableCustomType.WholeId, cmc);
				}
				return cmc;
			}
			else if (type.VariableLibType != null)
			{
				LimnorContextMenuCollection cmc;
				if (ss_menuCollections == null)
				{
					ss_menuCollections = new Dictionary<UInt64, LimnorContextMenuCollection>();
					cmc = new LimnorContextMenuCollection(type, true);
					ss_menuCollections.Add(type.WholeId, cmc);
				}
				else if (ss_menuCollections.TryGetValue(type.WholeId, out cmc))
				{
					cmc.SetOwner(type);
				}
				else
				{
					cmc = new LimnorContextMenuCollection(type, true);
					ss_menuCollections.Add(type.WholeId, cmc);
				}
				return cmc;
			}
			else
			{
				throw new DesignerException("null object type calling GetMenuCollection");
			}
		}
		public static void RemoveMenuCollection(IClass type)
		{
			if (type.VariableCustomType != null)
			{
				if (s_menuClassPointerCollections != null)
				{
					List<UInt64> ids = new List<ulong>();
					foreach (UInt64 id in s_menuClassPointerCollections.Keys)
					{
						UInt32 c, a;
						DesignUtil.ParseDDWord(id, out a, out c);
						if (type.ClassId == c)
						{
							ids.Add(id);
						}
					}
					foreach (UInt64 id in ids)
					{
						s_menuClassPointerCollections.Remove(id);
					}
				}
			}
			else if (type.VariableWrapperType != null)
			{
				if (s_menuWrapperCollections != null)
				{
					if (s_menuWrapperCollections.ContainsKey(type.WholeId))
					{
						s_menuWrapperCollections.Remove(type.WholeId);
					}
				}
			}
			else if (type.VariableLibType != null)
			{
				if (s_menuCollections != null)
				{
					if (s_menuCollections.ContainsKey(type.WholeId))
					{
						s_menuCollections.Remove(type.WholeId);
					}
				}
			}
		}
		private static void removemenus(Dictionary<UInt64, LimnorContextMenuCollection> menus, UInt32 classId)
		{
			if (menus != null)
			{
				List<UInt64> found = new List<ulong>();
				foreach (KeyValuePair<UInt64, LimnorContextMenuCollection> kv in menus)
				{
					UInt32 lo, hi;
					DesignUtil.ParseDDWord(kv.Key, out lo, out hi);
					if (hi == classId)
					{
						found.Add(kv.Key);
					}
				}
				for (int i = 0; i < found.Count; i++)
				{
					menus.Remove(found[i]);
				}
			}
		}
		public static void ClearMenuCollection(UInt32 classId)
		{
			removemenus(s_menuCollections, classId);
			removemenus(s_menuClassPointerCollections, classId);
			removemenus(s_menuWrapperCollections, classId);
			//
			removemenus(ss_menuCollections, classId);
			removemenus(ss_menuClassPointerCollections, classId);
			removemenus(ss_menuWrapperCollections, classId);

		}
		//==================================================================================
		public static string GetTypePathDirect(Type type)
		{
			string dir = DesignUtil.GetApplicationDataFolder();
			return System.IO.Path.Combine(dir, VPLUtil.GetInternalType(type).Name) + "." + FILE_EXT_ME;
		}
		public static string GetTypePath(Type type)
		{
			type = VPLUtil.GetInternalType(type);
			string baseDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			string dir = DesignUtil.GetApplicationDataFolder();
			string sFile = System.IO.Path.Combine(dir, type.Name) + "." + FILE_EXT_ME;
			while (!System.IO.File.Exists(sFile))
			{
				string baseFile = System.IO.Path.Combine(baseDir, type.Name) + "." + FILE_EXT_ME;
				if (System.IO.File.Exists(baseFile))
				{
					System.IO.File.Copy(baseFile, sFile);
					break;
				}
				if (type.Equals(typeof(object)))
					break;
				type = type.BaseType;
				if (type == null)
				{
					break;
				}
				sFile = System.IO.Path.Combine(dir, type.Name) + "." + FILE_EXT_ME;
			}
			return sFile;
		}
		public static string GetTypePathDirect(ClassPointer pointer)
		{
			string dir = pointer.ObjectList.Project.ProjectFolder;
			return System.IO.Path.Combine(dir, pointer.ReferenceName) + "." + FILE_EXT_ME;
		}
		#endregion
		//
		#region private help
		private void loadData()
		{
			_primaryMethods = new List<MenuItemDataMethod>();
			_secondaryMethods = new List<MenuItemDataMethod>();
			_primaryEvents = new List<MenuItemDataEvent>();
			_secondaryEvents = new List<MenuItemDataEvent>();
			_primaryProperties = new List<MenuItemDataProperty>();
			_secondaryProperties = new List<MenuItemDataProperty>();
			//
			_selectMethod = new MenuItemDataMethodSelector("?", this);
			_selectProperty = new MenuItemDataPropertySelector("?", this);
			_selectEvent = new MenuItemDataEventSelector("?", this);
			//
			ClassPointer xp = _owner.VariableCustomType;
			if (xp != null)
			{
				XClass<InterfaceClass> xi = xp.ObjectInstance as XClass<InterfaceClass>;
				if (xi != null)
				{
					return;
				}
			}
			if (_owner.VariableWrapperType != null)
			{
				loadWrapperData();
			}
			else if (_owner.VariableCustomType != null)
			{
				loadCustomData();
			}
			else if (_owner.VariableLibType != null)
			{
				if (_owner.VariableLibType.IsGenericParameter)
				{
					ParameterClassCollectionItem pcci = _owner as ParameterClassCollectionItem;
					if (pcci != null)
					{
						if (pcci.ConcreteType != null)
						{
							_concretType = pcci.ConcreteType;
							if (pcci.ConcreteType.IsLibType)
							{
								loadLibData();
							}
							else
							{
								loadCustomData();
							}
						}
						else
						{
							loadLibData();
						}
					}
					else
					{
						loadLibData();
					}
				}
				else
				{
					loadLibData();
				}
			}
			else
			{
				throw new DesignerException("Class type not set calling loadData");
			}
		}
		/// <summary>
		/// not support custom PME for wrapper
		/// </summary>
		private void loadWrapperData()
		{
			bool loadedM = false;
			bool loadedP = false;
			bool loadedE = false;
			IClassWrapper wrapper = _owner.VariableWrapperType;
			methods = wrapper.GetMethods();
			events = wrapper.GetEvents();
			properties = wrapper.GetProperties();
			XmlNode rootNode = wrapper.MenuItemNode;
			if (rootNode != null)
			{
				XmlNodeList nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}", XML_Methods, XML_Primary, XML_Method));
				if (nodes != null && nodes.Count > 0)
				{
					foreach (XmlNode node in nodes)
					{
						MethodInfo mi;
						if (methods.TryGetValue(node.InnerText, out mi))
						{
							_primaryMethods.Add(new MethodItem(node.InnerText, Owner, mi));
						}
					}
					loadedM = true;
				}
				nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}", XML_Methods, XML_Secondary, XML_Method));
				if (nodes != null && nodes.Count > 0)
				{
					foreach (XmlNode node in nodes)
					{
						MethodInfo mi;
						if (methods.TryGetValue(node.InnerText, out mi))
						{
							_secondaryMethods.Add(new MethodItem(node.InnerText, Owner, mi));
						}
					}
				}
				nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}", XML_Events, XML_Primary, XML_Event));
				if (nodes != null && nodes.Count > 0)
				{
					foreach (XmlNode node in nodes)
					{
						EventInfo mi;
						if (events.TryGetValue(node.InnerText, out mi))
						{
							_primaryEvents.Add(new EventItem(node.InnerText, Owner, mi));
						}
					}
					loadedE = true;
				}
				nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}", XML_Events, XML_Secondary, XML_Event));
				if (nodes != null && nodes.Count > 0)
				{
					foreach (XmlNode node in nodes)
					{
						EventInfo mi;
						if (events.TryGetValue(node.InnerText, out mi))
						{
							_secondaryEvents.Add(new EventItem(node.InnerText, Owner, mi));
						}
					}
				}
				//
				nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}", XML_Properties, XML_Primary, XML_Property));
				if (nodes != null && nodes.Count > 0)
				{
					foreach (XmlNode node in nodes)
					{
						PropertyDescriptor mi;
						if (properties.TryGetValue(node.InnerText, out mi))
						{
							_primaryProperties.Add(new PropertyItem(node.InnerText, Owner, mi));
						}
					}
					loadedP = true;
				}
				nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}", XML_Properties, XML_Secondary, XML_Property));
				if (nodes != null && nodes.Count > 0)
				{
					foreach (XmlNode node in nodes)
					{
						PropertyDescriptor mi;
						if (properties.TryGetValue(node.InnerText, out mi))
						{
							_secondaryProperties.Add(new PropertyItem(node.InnerText, Owner, mi));
						}
					}
				}
			}

			int pm = 0;
			int sm = 0;
			int pe = 0;
			int se = 0;
			if (!loadedM)
			{
				foreach (KeyValuePair<string, MethodInfo> kv in methods)
				{
					if (pm < MaxMenuItemCount)
					{
						_primaryMethods.Add(new MethodItem(kv.Key, Owner, kv.Value));
						pm++;
					}
					else
					{
						_secondaryMethods.Add(new MethodItem(kv.Key, Owner, kv.Value));
						sm++;
						if (sm >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
			if (!loadedE)
			{
				foreach (KeyValuePair<string, EventInfo> kv in events)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryEvents.Add(new EventItem(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryEvents.Add(new EventItem(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
			if (!loadedP)
			{
				foreach (KeyValuePair<string, PropertyDescriptor> kv in properties)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryProperties.Add(new PropertyItem(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryProperties.Add(new PropertyItem(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
		}
		private void loadCustomData()
		{
			bool loaded = false;
			//
			//all custom PME
			methodsC = new SortedDictionary<string, MethodClass>();
			eventsC = new SortedDictionary<string, EventClass>();
			propertiesC = new SortedDictionary<string, PropertyClass>();
			//
			ClassPointer _class = _owner.VariableCustomType;
			if (_concretType != null)
			{
				_class = _concretType.VariableCustomType;
			}
			//
			List<MethodClass> mlist = _class.GetCurrentLevelCustomMethods();
			if (mlist != null && mlist.Count > 0)
			{
				foreach (MethodClass mc in mlist)
				{
					if (_isStatic)
					{
						if (mc.IsStatic)
						{
							methodsC.Add(mc.GetMethodSignature(), mc);
						}
					}
					else
					{
						methodsC.Add(mc.GetMethodSignature(), mc);
					}
				}
			}
			List<PropertyClass> plist = _class.GetCurrentLevelCustomProperties();
			if (plist != null && plist.Count > 0)
			{
				foreach (PropertyClass pc in plist)
				{
					if ((_owner is ClassPointer) || pc.AccessControl == EnumAccessControl.Public)
					{
						if (_isStatic)
						{
							if (pc.IsStatic)
							{
								propertiesC.Add(pc.Name, pc);
							}
						}
						else
						{
							propertiesC.Add(pc.Name, pc);
						}
					}
				}
			}
			List<EventClass> elist = _class.GetCurrentLevelCustomEvents();
			if (elist != null && elist.Count > 0)
			{
				foreach (EventClass ec in elist)
				{
					if (_isStatic)
					{
						if (ec.IsStatic)
						{
							eventsC.Add(ec.DisplayName, ec);
						}
					}
					else
					{
						eventsC.Add(ec.DisplayName, ec);
					}
				}
			}
			//find out library PME
			IWebClientControl webc = _class.ObjectInstance as IWebClientControl;
			if (webc != null)
			{
				PropertyDescriptorCollection pdc = webc.GetWebClientProperties(false);
				properties = new SortedDictionary<string, PropertyDescriptor>();
				if (pdc != null)
				{
					foreach (PropertyDescriptor pd in pdc)
					{
						if (_isStatic)
						{

						}
						else
						{
							if (properties.ContainsKey(pd.Name))
							{
							}
							else
							{
								properties.Add(pd.Name, pd);
							}
						}
					}
				}
				methods = new SortedDictionary<string, MethodInfo>();
				MethodInfo[] ms = webc.GetWebClientMethods(false);
				if (ms != null)
				{
					for (int i = 0; i < ms.Length; i++)
					{
						if (!_isStatic || ms[i].IsStatic)
						{
							methods.Add(MethodPointer.GetMethodSignature(ms[i]), ms[i]);
						}
					}
				}
				events = new SortedDictionary<string, EventInfo>();
				EventInfo[] es = webc.GetWebClientEvents(false);
				if (es != null)
				{
					for (int i = 0; i < es.Length; i++)
					{
						if (!_isStatic)
						{
							events.Add(es[i].Name, es[i]);
						}
					}
				}
			}
			else
			{
				bool isWebPage = false;
				if (_class.ObjectInstance == null)
				{
					if (typeof(WebPage).IsAssignableFrom(_class.BaseClassType))
					{
						isWebPage = true;
					}
				}
				if (isWebPage)
				{
					methods = new SortedDictionary<string, MethodInfo>();
					events = new SortedDictionary<string, EventInfo>();
					properties = new SortedDictionary<string, PropertyDescriptor>();
					EnumReflectionMemberInfoSelectScope scope;
					if (_isStatic)
					{
						scope = EnumReflectionMemberInfoSelectScope.StaticOnly;
					}
					else
					{
						scope = EnumReflectionMemberInfoSelectScope.InstanceOnly;
					}
					MethodInfo[] mifs = VPLUtil.GetMethods(_class.BaseClassType, scope, false, true, (_owner is ClassPointer), _class.IsWebPage ||_class.IsWebApp);
					if (mifs != null && mifs.Length > 0)
					{
						for (int k = 0; k < mifs.Length; k++)
						{
							object[] objs = mifs[k].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
							if (objs != null && objs.Length > 0)
							{
								methods.Add(MethodInfoPointer.GetMethodSignature(mifs[k]), mifs[k]);
							}
						}
					}
				}
				else
				{
					Type tx = VPLUtil.GetObjectType(_class.BaseClassType);
					//
					if (!tx.IsSubclassOf(typeof(LimnorApp)))
					{
						methods = DesignUtil.FindAllMethods(tx, false);
						events = DesignUtil.FindAllEvents(tx, false);
						PropertyDescriptorCollection pdc = VPLUtil.GetProperties(tx, EnumReflectionMemberInfoSelectScope.Both, false, true, false);
						properties = new SortedDictionary<string, PropertyDescriptor>();
						foreach (PropertyDescriptor pd in pdc)
						{
							if (!_isStatic)
							{
								if (!properties.ContainsKey(pd.Name))
								{
									properties.Add(pd.Name, pd);
								}
							}
						}
					}
					else
					{
						methods = new SortedDictionary<string, MethodInfo>();
						properties = new SortedDictionary<string, PropertyDescriptor>();
						events = new SortedDictionary<string, EventInfo>();
						if (typeof(LimnorConsole).IsAssignableFrom(tx))
						{
							EventInfo[] eifs = tx.GetEvents(BindingFlags.Static | BindingFlags.Public);
							if (eifs != null)
							{
								for (int i = 0; i < eifs.Length; i++)
								{
									events.Add(eifs[i].Name, eifs[i]);
								}
							}
						}
					}
				}
			}
			//load frequently used menus
			string path = FilePath;
			if (!System.IO.File.Exists(path))
			{
				string fn = System.IO.Path.GetFileName(path);
				string appDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
				string src = System.IO.Path.Combine(appDir, fn);
				if (System.IO.File.Exists(src))
				{
					System.IO.File.Copy(src, path);
				}
			}
			if (System.IO.File.Exists(path))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);
				if (doc.DocumentElement != null)
				{
					XmlNode rootNode = doc.DocumentElement.SelectSingleNode(RootName);
					if (rootNode != null)
					{
						loaded = true;
						XmlNodeList nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}/{1}/{2}", XML_Methods, XML_Primary, XML_Method));
						if (nodes != null && nodes.Count > 0)
						{
							foreach (XmlNode node in nodes)
							{
								MethodClass mi;
								if (methodsC.TryGetValue(node.InnerText, out mi))
								{
									_primaryMethods.Add(new MethodItemClassPointer(node.InnerText, Owner, mi));
								}
								else
								{
									MethodInfo mi2;
									if (methods.TryGetValue(node.InnerText, out mi2))
									{
										_primaryMethods.Add(new MethodItem(node.InnerText, Owner, mi2));
									}
								}
							}
						}
						nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}/{1}/{2}", XML_Methods, XML_Secondary, XML_Method));
						if (nodes != null && nodes.Count > 0)
						{
							foreach (XmlNode node in nodes)
							{
								MethodClass mi;
								if (methodsC.TryGetValue(node.InnerText, out mi))
								{
									_secondaryMethods.Add(new MethodItemClassPointer(node.InnerText, Owner, mi));
								}
								else
								{
									MethodInfo mi2;
									if (methods.TryGetValue(node.InnerText, out mi2))
									{
										_secondaryMethods.Add(new MethodItem(node.InnerText, Owner, mi2));
									}
								}
							}
						}
						nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}/{1}/{2}", XML_Events, XML_Primary, XML_Event));
						if (nodes != null && nodes.Count > 0)
						{
							foreach (XmlNode node in nodes)
							{
								EventClass mi;
								if (eventsC.TryGetValue(node.InnerText, out mi))
								{
									_primaryEvents.Add(new EventItemClassPointer(node.InnerText, Owner, mi));
								}
								else
								{
									EventInfo ei;
									if (events.TryGetValue(node.InnerText, out ei))
									{
										_primaryEvents.Add(new EventItem(node.InnerText, Owner, ei));
									}
								}
							}
						}
						nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}/{1}/{2}", XML_Events, XML_Secondary, XML_Event));
						if (nodes != null && nodes.Count > 0)
						{
							foreach (XmlNode node in nodes)
							{
								EventClass mi;
								if (eventsC.TryGetValue(node.InnerText, out mi))
								{
									_secondaryEvents.Add(new EventItemClassPointer(node.InnerText, Owner, mi));
								}
								else
								{
									EventInfo ei;
									if (events.TryGetValue(node.InnerText, out ei))
									{
										_secondaryEvents.Add(new EventItem(node.InnerText, Owner, ei));
									}
								}
							}
						}
						//
						nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}/{1}/{2}", XML_Properties, XML_Primary, XML_Property));
						if (nodes != null && nodes.Count > 0)
						{
							foreach (XmlNode node in nodes)
							{
								PropertyClass mi;
								if (propertiesC.TryGetValue(node.InnerText, out mi))
								{
									_primaryProperties.Add(new PropertyItemClassPointer(node.InnerText, Owner, mi));
								}
								else
								{
									PropertyDescriptor pi;
									if (properties.TryGetValue(node.InnerText, out pi))
									{
										_primaryProperties.Add(new PropertyItem(node.InnerText, Owner, pi));
									}
								}
							}
						}
						nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}/{1}/{2}", XML_Properties, XML_Secondary, XML_Property));
						if (nodes != null && nodes.Count > 0)
						{
							foreach (XmlNode node in nodes)
							{
								PropertyClass mi;
								if (propertiesC.TryGetValue(node.InnerText, out mi))
								{
									_secondaryProperties.Add(new PropertyItemClassPointer(node.InnerText, Owner, mi));
								}
								else
								{
									PropertyDescriptor pi;
									if (properties.TryGetValue(node.InnerText, out pi))
									{
										_secondaryProperties.Add(new PropertyItem(node.InnerText, Owner, pi));
									}
								}
							}
						}
					}
				}
			}
			if (!loaded)
			{
				int pm = 0;
				int sm = 0;
				int pe = 0;
				int se = 0;
				foreach (KeyValuePair<string, MethodClass> kv in methodsC)
				{
					if (pm < MaxMenuItemCount)
					{
						_primaryMethods.Add(new MethodItemClassPointer(kv.Key, Owner, kv.Value));
						pm++;
					}
					else
					{
						_secondaryMethods.Add(new MethodItemClassPointer(kv.Key, Owner, kv.Value));
						sm++;
						if (sm >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
				foreach (KeyValuePair<string, EventClass> kv in eventsC)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryEvents.Add(new EventItemClassPointer(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryEvents.Add(new EventItemClassPointer(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
				foreach (KeyValuePair<string, PropertyClass> kv in propertiesC)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryProperties.Add(new PropertyItemClassPointer(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryProperties.Add(new PropertyItemClassPointer(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
				//=========================================================
				foreach (KeyValuePair<string, MethodInfo> kv in methods)
				{
					if (pm < MaxMenuItemCount)
					{
						_primaryMethods.Add(new MethodItem(kv.Key, Owner, kv.Value));
						pm++;
					}
					else
					{
						_secondaryMethods.Add(new MethodItem(kv.Key, Owner, kv.Value));
						sm++;
						if (sm >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
				foreach (KeyValuePair<string, EventInfo> kv in events)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryEvents.Add(new EventItem(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryEvents.Add(new EventItem(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
				foreach (KeyValuePair<string, PropertyDescriptor> kv in properties)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryProperties.Add(new PropertyItem(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryProperties.Add(new PropertyItem(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
		}
		private void loadLibData()
		{
			bool isWebPage = false;
			bool isPhpProj = false;
			Type objType = _owner.VariableLibType;
			EnumWebRunAt runAt = _owner.RunAt;
			ClassPointer root = _owner.RootPointer;
			if (root != null)
			{
				isWebPage = root.IsWebPage;
				isPhpProj = (root.Project.ProjectType == VSPrj.EnumProjectType.WebAppPhp);
			}
			if (_concretType != null)
			{
				objType = _concretType.VariableLibType;
			}
			if (MethodEditContext.UseClientExecuterOnly || (isWebPage && (MethodInfoWebClient.IsWebObject(_owner.ObjectInstance) || _owner.RunAt == EnumWebRunAt.Client)))
			{
				properties = new SortedDictionary<string, PropertyDescriptor>();
				if (_owner.ObjectInstance != null)
				{
					IWebServerProgrammingSupport wspp = _owner.ObjectInstance as IWebServerProgrammingSupport;
					PropertyDescriptorCollection pdc = MethodInfoWebClient.GetObjectProperties(EnumReflectionMemberInfoSelectScope.InstanceOnly, _owner.ObjectInstance, false);
					if (pdc != null)
					{
						bool isControl = (_owner.ObjectInstance is Control);
						foreach (PropertyDescriptor pd in pdc)
						{
							if (!pd.IsReadOnly)
							{
								if (MethodEditContext.UseClientExecuterOnly)
								{
									if (!WebClientMemberAttribute.IsClientProperty(pd))
									{
										if (isControl)
										{
											if (!WebClientData.ClientControlProperties.Contains(pd.Name))
											{
												continue;
											}
										}
										else
										{
											continue;
										}
									}
								}
								if (isPhpProj)
								{
									if (!WebServerMemberAttribute.IsServerProperty(pd) && !WebClientMemberAttribute.IsClientProperty(pd))
									{
										if (isControl)
										{
											if (!WebClientData.ClientControlProperties.Contains(pd.Name))
											{
												continue;
											}
										}
										else
										{
											continue;
										}
									}
								}
								if (!properties.ContainsKey(pd.Name))
								{
									properties.Add(pd.Name, pd);
								}
							}
						}
					}
				}
				else
				{
					PropertyInfo[] pifs = VPLUtil.GetProperties(objType);//.GetProperties();
					if (pifs != null && pifs.Length > 0)
					{
						for (int i = 0; i < pifs.Length; i++)
						{
							if (!properties.ContainsKey(pifs[i].Name))
							{
								if (pifs[i].CanWrite)
								{
									if (MethodEditContext.UseClientExecuterOnly)
									{
										if (!WebClientMemberAttribute.IsClientProperty(pifs[i]))
										{
											continue;
										}
									}
									if (isPhpProj)
									{
										if (!WebServerMemberAttribute.IsServerProperty(pifs[i]) && !WebClientMemberAttribute.IsClientProperty(pifs[i]))
										{
											continue;
										}
									}
									List<Attribute> attrs = new List<Attribute>();
									object[] vs = pifs[i].GetCustomAttributes(true);
									if (vs != null)
									{
										for (int k = 0; k < vs.Length; k++)
										{
											Attribute a = vs[k] as Attribute;
											if (a != null)
											{
												attrs.Add(a);
											}
										}
									}
									properties.Add(pifs[i].Name, new PropertyDescriptorValue(pifs[i].Name, attrs.ToArray(), pifs[i], pifs[i].PropertyType, _owner.VariableLibType));
								}
							}
						}
					}
				}
				methods = new SortedDictionary<string, MethodInfo>();
				if (_owner.ObjectInstance != null)
				{
					MethodInfo[] ms = MethodInfoWebClient.GetWebMethods(false, _owner.ObjectInstance);
					if (ms != null)
					{
						for (int i = 0; i < ms.Length; i++)
						{
							methods.Add(MethodPointer.GetMethodSignature(ms[i]), ms[i]);
						}
					}
				}
				else
				{
					MethodInfo[] mifs = VPLUtil.GetMethods(objType);
					if (mifs != null && mifs.Length > 0)
					{
						for (int i = 0; i < mifs.Length; i++)
						{
							string msign = MethodPointer.GetMethodSignature(mifs[i]);
							if (!methods.ContainsKey(msign))
							{
								if (WebClientMemberAttribute.IsClientMethod(mifs[i]))
								{
									methods.Add(msign, mifs[i]);
								}
							}
						}
					}
				}
				events = new SortedDictionary<string, EventInfo>();
				if (_owner.ObjectInstance != null)
				{
					EventInfo[] es = MethodInfoWebClient.GetWebClientEvents(false, _owner.ObjectInstance);
					if (es != null)
					{
						for (int i = 0; i < es.Length; i++)
						{
							if (runAt == EnumWebRunAt.Client)
							{
								if (!WebClientMemberAttribute.IsClientEvent(es[i]))
								{
									continue;
								}
							}
							events.Add(es[i].Name, es[i]);
						}
					}
				}
				else
				{
					EventInfo[] eifs = VPLUtil.GetEvents(objType);
					if (eifs != null && eifs.Length > 0)
					{
						for (int i = 0; i < eifs.Length; i++)
						{
							if (!events.ContainsKey(eifs[i].Name))
							{
								if (WebClientMemberAttribute.IsClientEvent(eifs[i]))
								{
									events.Add(eifs[i].Name, eifs[i]);
								}
							}
						}
					}
				}
			}
			else
			{
				Type _type = objType;
				Type tx;
				if (!typeof(DrawingItem).IsAssignableFrom(_type) && _type.GetInterface("IPhpType") == null)
				{
					tx = VPLUtil.GetInternalType(_type);
				}
				else
				{
					tx = _type;
				}
				Type tx0 = VPLUtil.GetCoClassType(tx);
				if (tx0 != null)
				{
					tx = tx0;
				}
				//
				object obj = tx;
				MemberComponentId mci = _owner as MemberComponentId;
				if (mci != null && mci.ObjectInstance != null)
				{
					obj = mci.ObjectInstance;
				}
				RuntimeInstance tli = obj as RuntimeInstance;
				if (tli != null)
				{
					methods = new SortedDictionary<string, MethodInfo>();
					MethodInfo[] mifs = tli.GetMethods(EnumReflectionMemberInfoSelectScope.InstanceOnly);
					if (mifs != null)
					{
						for (int i = 0; i < mifs.Length; i++)
						{
							if (VPLUtil.IsNotForProgramming(mifs[i]))
							{
								continue;
							}
							string sn = MethodPointer.GetMethodSignature(mifs[i]);
							int n = 2;
							string sn2 = sn;
							while (methods.ContainsKey(sn2))
							{
								sn2 = string.Format(CultureInfo.InvariantCulture, "{0}{1}", sn, n);
								n++;
							}
							methods.Add(sn2, mifs[i]);
						}
					}
					events = new SortedDictionary<string, EventInfo>();
					if (tli.InstanceType != null)
					{
						EventInfo[] evs = tli.InstanceType.GetEvents();
						if (evs != null)
						{
							for (int i = 0; i < evs.Length; i++)
							{
								events.Add(evs[i].Name, evs[i]);
							}
						}
					}
				}
				else
				{
					ICustomEventMethodDescriptor cem = _owner as ICustomEventMethodDescriptor;
					if (cem == null)
					{
						if (mci != null)
						{
							cem = mci.ObjectInstance as ICustomEventMethodDescriptor;
						}
					}
					if (cem != null)
					{
						methods = DesignUtil.SortMethods(cem.GetMethods(), tx);
						events = DesignUtil.SortEvents(cem.GetEvents(), tx);
					}
					else
					{
						methods = DesignUtil.FindAllMethods(tx, false);
						if (tx.Equals(typeof(PhpArray)) || tx.Equals(typeof(JsArray)))
						{
							ArrayForEachMethodInfo af = new ArrayForEachMethodInfo(typeof(object), _owner.MemberId.ToString("x", CultureInfo.InvariantCulture));
							methods.Add(af.Name, af);
						}
						events = DesignUtil.FindAllEvents(tx, false);
					}
					SortedDictionary<string, EventInfo> eifs = new SortedDictionary<string, EventInfo>();
					foreach (KeyValuePair<string, EventInfo> kv in events)
					{
						object[] vs = kv.Value.GetCustomAttributes(typeof(WebClientOnlyAttribute), true);
						if (vs == null || vs.Length == 0)
						{
							eifs.Add(kv.Key, kv.Value);
						}
					}
					events = eifs;
				}
				PropertyDescriptorCollection pdc;
				ICustomPropertyCollection cpc = obj as ICustomPropertyCollection;
				if (cpc != null)
				{
					pdc = cpc.GetCustomPropertyCollection();
				}
				else
				{
					pdc = VPLUtil.GetProperties(obj, EnumReflectionMemberInfoSelectScope.Both, false, false, true);
				}
				properties = new SortedDictionary<string, PropertyDescriptor>();
				foreach (PropertyDescriptor pd in pdc)
				{
					if (!properties.ContainsKey(pd.Name))
					{
						if (!pd.IsReadOnly) //may need to set the sub-property of it? menu is always set this level?
						{
							properties.Add(pd.Name, pd);
						}
					}
				}

				//
				// find XML file for the type
				string path = GetTypePath(tx);
				if (!System.IO.File.Exists(path))
				{
					if (_type.IsSubclassOf(typeof(Control)))
					{
						tx = typeof(Button);
						path = GetTypePath(tx);
					}
				}
				if (System.IO.File.Exists(path))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(path);
					if (doc.DocumentElement != null)
					{
						XmlNode rootNode = null;
						if (!tx.IsArray)
						{
							try
							{
								rootNode = doc.DocumentElement.SelectSingleNode(tx.FullName);
							}
							catch
							{
							}
						}
						if (rootNode == null)
						{
							Type t0 = _type;
							while (rootNode == null)
							{
								if (t0.Equals(typeof(object)))
									break;
								if (t0.BaseType == null)
									break;
								t0 = t0.BaseType;
								t0 = VPLUtil.GetObjectType(t0);
								if (!t0.IsArray)
								{
									try
									{
										rootNode = doc.DocumentElement.SelectSingleNode(t0.FullName);
									}
									catch
									{
									}
								}
							}
						}
						if (rootNode != null)
						{
							//loaded = true;
							XmlNodeList nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}/{2}", XML_Methods, XML_Primary, XML_Method));
							if (nodes != null && nodes.Count > 0)
							{
								foreach (XmlNode node in nodes)
								{
									MethodInfo mi;
									if (methods.TryGetValue(node.InnerText, out mi))
									{
										_primaryMethods.Add(new MethodItem(node.InnerText, Owner, mi));
									}
								}
							}
							nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}/{2}", XML_Methods, XML_Secondary, XML_Method));
							if (nodes != null && nodes.Count > 0)
							{
								foreach (XmlNode node in nodes)
								{
									MethodInfo mi;
									if (methods.TryGetValue(node.InnerText, out mi))
									{
										_secondaryMethods.Add(new MethodItem(node.InnerText, Owner, mi));
									}
								}
							}
							nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}/{2}", XML_Events, XML_Primary, XML_Event));
							if (nodes != null && nodes.Count > 0)
							{
								foreach (XmlNode node in nodes)
								{
									EventInfo mi;
									if (events.TryGetValue(node.InnerText, out mi))
									{
										_primaryEvents.Add(new EventItem(node.InnerText, Owner, mi));
									}
								}
							}
							nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}/{2}", XML_Events, XML_Secondary, XML_Event));
							if (nodes != null && nodes.Count > 0)
							{
								foreach (XmlNode node in nodes)
								{
									EventInfo mi;
									if (events.TryGetValue(node.InnerText, out mi))
									{
										_secondaryEvents.Add(new EventItem(node.InnerText, Owner, mi));
									}
								}
							}
							//
							nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}/{2}", XML_Properties, XML_Primary, XML_Property));
							if (nodes != null && nodes.Count > 0)
							{
								foreach (XmlNode node in nodes)
								{
									PropertyDescriptor mi;
									if (properties.TryGetValue(node.InnerText, out mi))
									{
										_primaryProperties.Add(new PropertyItem(node.InnerText, Owner, mi));
									}
								}
							}
							nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}/{1}/{2}", XML_Properties, XML_Secondary, XML_Property));
							if (nodes != null && nodes.Count > 0)
							{
								foreach (XmlNode node in nodes)
								{
									PropertyDescriptor mi;
									if (properties.TryGetValue(node.InnerText, out mi))
									{
										_secondaryProperties.Add(new PropertyItem(node.InnerText, Owner, mi));
									}
								}
							}
						}
					}
				}
			}
			int pm = _primaryMethods.Count;
			int sm = _secondaryMethods.Count;
			int pe = _primaryEvents.Count;
			int se = _secondaryEvents.Count;
			int pp = _primaryProperties.Count;
			int sp = _secondaryProperties.Count;
			if (pm == 0 || sm == 0)
			{
				foreach (KeyValuePair<string, MethodInfo> kv in methods)
				{
					if (pm < MaxMenuItemCount)
					{
						_primaryMethods.Add(new MethodItem(kv.Key, Owner, kv.Value));
						pm++;
					}
					else
					{
						_secondaryMethods.Add(new MethodItem(kv.Key, Owner, kv.Value));
						sm++;
						if (sm >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
			if (pe == 0 || se == 0)
			{
				foreach (KeyValuePair<string, EventInfo> kv in events)
				{
					if (pe < MaxMenuItemCount)
					{
						_primaryEvents.Add(new EventItem(kv.Key, Owner, kv.Value));
						pe++;
					}
					else
					{
						_secondaryEvents.Add(new EventItem(kv.Key, Owner, kv.Value));
						se++;
						if (se >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
			if (pp == 0 || sp == 0)
			{
				foreach (KeyValuePair<string, PropertyDescriptor> kv in properties)
				{
					if (pp < MaxMenuItemCount)
					{
						_primaryProperties.Add(new PropertyItem(kv.Key, Owner, kv.Value));
						pp++;
					}
					else
					{
						_secondaryProperties.Add(new PropertyItem(kv.Key, Owner, kv.Value));
						sp++;
						if (sp >= MaxMenuItemCount)
						{
							break;
						}
					}
				}
			}
		}
		#endregion
		#region Properties
		public string FilePath
		{
			get
			{
				if (_owner.VariableCustomType != null)
				{
					return _owner.VariableCustomType.ContextMenuFile;
				}
				else if (_owner.VariableLibType != null)
				{
					return LimnorContextMenuCollection.GetTypePathDirect(_owner.VariableLibType);
				}
				else if (_owner.VariableWrapperType != null)
				{
					return _owner.VariableWrapperType.MenuItemFilePath;
				}
				return null;
			}
		}
		public string RootName
		{
			get
			{
				string r = null;
				if (_owner.VariableCustomType != null)
				{
					r = _owner.VariableCustomType.BaseClassType.FullName;
				}
				else if (_owner.VariableLibType != null)
				{
					r = VPLUtil.GetInternalType(_owner.VariableLibType).FullName;
				}
				else if (_owner.VariableWrapperType != null)
				{
					r = _owner.VariableWrapperType.WrappedType.FullName;
				}
				if (!string.IsNullOrEmpty(r))
				{
					return r.Replace('[', '_').Replace(']', '_');
				}
				return null;
			}
		}
		public MenuItemDataMethod MethodSelector
		{
			get
			{
				return _selectMethod;
			}
		}
		public MenuItemDataProperty PropertySelector
		{
			get
			{
				return _selectProperty;
			}
		}
		public MenuItemDataEvent EventSelector
		{
			get
			{
				return _selectEvent;
			}
		}
		public IClass Owner
		{
			get
			{
				return _owner;
			}
		}
		public Type Type
		{
			get
			{
				return _owner.VariableLibType;
			}
		}
		public ClassPointer Pointer
		{
			get
			{
				return _owner.VariableCustomType;
			}
		}
		public IClassWrapper Wrapper
		{
			get
			{
				return _owner.VariableWrapperType;
			}
		}
		public List<MenuItemDataMethod> PrimaryMethods
		{
			get
			{
				if (_primaryMethods == null)
					return null;
				return _primaryMethods;
			}
		}
		public List<MenuItemDataMethod> SecondaryMethods
		{
			get
			{
				if (_secondaryMethods == null)
					return null;
				return _secondaryMethods;
			}
		}
		public List<MenuItemDataEvent> PrimaryEvents
		{
			get
			{
				if (_primaryEvents == null)
					return null;
				return _primaryEvents;
			}
		}
		public List<MenuItemDataEvent> SecondaryEvents
		{
			get
			{
				if (_secondaryEvents == null)
					return null;
				return _secondaryEvents;
			}
		}
		//
		public List<MenuItemDataProperty> PrimaryProperties
		{
			get
			{
				if (_primaryProperties == null)
					return null;
				return _primaryProperties;
			}
		}
		public List<MenuItemDataProperty> SecondaryProperties
		{
			get
			{
				if (_secondaryProperties == null)
					return null;
				return _secondaryProperties;
			}
		}
		#endregion
		#region private menu handler methods
		private void createNewAction(MenuItemDataMethod data)
		{
			if (_designPane != null)
			{
				IAction act = data.CreateMethodAction(_designPane, _owner, null, null);
				if (act != null)
				{

				}
			}
		}
		private void miAction_Click(object sender, EventArgs e)
		{
			MenuItemDataMethod data = (MenuItemDataMethod)(((MenuItem)sender).Tag);
			createNewAction(data);
		}
		private void miSetMethods_Click(object sender, EventArgs e)
		{
			miAction_Click(sender, e);
		}
		private void miSetProperty_Click(object sender, EventArgs e)
		{
			MenuItemDataProperty data = (MenuItemDataProperty)(((MenuItem)sender).Tag);
			ActionClass act = data.CreateSetPropertyAction(_designPane, _owner, null, null);
			if (act != null)
			{
			}
		}
		private void miSetProperties_Click(object sender, EventArgs e)
		{
			miSetProperty_Click(sender, e);
		}
		private void miAssignEvent_Click(object sender, EventArgs e)
		{
			MenuItemDataEvent data = (MenuItemDataEvent)(((MenuItem)sender).Tag);
			data.ExecuteMenuCommand(_designPane.RootClass.Project, _owner, _designPane.RootXmlNode, _designPane.PaneHolder, null, null);
		}
		private void miSetEvents_Click(object sender, EventArgs e)
		{
			miAssignEvent_Click(sender, e);
		}
		#endregion
		#region Methods
		public static void createEventTree(MenuItem parentMenuItem, Point location, IClass owner, IEventInfoTree[] subs, EventHandler h)
		{
			if (subs != null && subs.Length > 0)
			{
				for (int i = 0; i < subs.Length; i++)
				{
					if (!string.IsNullOrEmpty(subs[i].Name) && string.CompareOrdinal("-", subs[i].Name) != 0)
					{
						EventItem skv = new EventItem(subs[i].Name, owner, subs[i].GetEventInfo());
						MenuItemWithBitmap sm0 = new MenuItemWithBitmap(subs[i].Name, Resources._event1.ToBitmap());
						sm0.Click += h;
						skv.Location = location;
						sm0.Tag = skv;
						parentMenuItem.MenuItems.Add(sm0);
						IEventInfoTree[] subss = subs[i].GetSubEventInfo();
						createEventTree(sm0, location, owner, subss, h);
					}
				}
			}
		}
		public void CreateContextMenu(ContextMenu mnu, Point location, ILimnorDesignPane designer)
		{
			MenuItem mi;
			MenuItem m0;
			MenuItem m1;
			//
			_designPane = designer;
			//
			#region Create Action
			mi = new MenuItemWithBitmap("Create Action", Resources._newMethodAction.ToBitmap());

			List<MenuItemDataMethod> methods = PrimaryMethods;
			foreach (MenuItemDataMethod kv in methods)
			{
				if (kv.Key.Length > 60)
				{
					m0 = new MenuItemWithBitmap(string.Format(CultureInfo.InvariantCulture, "{0}...", kv.Key.Substring(0, 60)), Resources._methodAction.ToBitmap());
				}
				else
				{
					m0 = new MenuItemWithBitmap(kv.Key, Resources._methodAction.ToBitmap());
				}
				m0.Click += new EventHandler(miAction_Click);
				kv.Location = location;
				m0.Tag = kv;
				mi.MenuItems.Add(m0);
			}
			m1 = new MenuItemWithBitmap("More methods", Resources._methods.ToBitmap());
			mi.MenuItems.Add(m1);
			methods = SecondaryMethods;
			foreach (MenuItemDataMethod kv in methods)
			{
				if (kv.Key.Length > 60)
				{
					m0 = new MenuItemWithBitmap(string.Format(CultureInfo.InvariantCulture, "{0}...", kv.Key.Substring(0, 60)), Resources._methodAction.ToBitmap());
				}
				else
				{
					m0 = new MenuItemWithBitmap(kv.Key, Resources._methodAction.ToBitmap());
				}
				m0.Click += new EventHandler(miAction_Click);
				kv.Location = location;
				m0.Tag = kv;
				m1.MenuItems.Add(m0);
			}
			m0 = new MenuItemWithBitmap("*All methods* =>", Resources._dialog.ToBitmap());
			MenuItemDataMethodSelector miAll = new MenuItemDataMethodSelector(m0.Text, this);
			miAll.Location = location;
			m0.Tag = miAll;
			m1.MenuItems.Add(m0);
			m0.Click += new EventHandler(miSetMethods_Click);
			//
			mnu.MenuItems.Add(mi);
			#endregion
			//
			#region Create Set Property Action
			mi = new MenuItemWithBitmap("Create Set Property Action", Resources._newPropAction.ToBitmap());
			List<MenuItemDataProperty> properties = PrimaryProperties;
			foreach (MenuItemDataProperty kv in properties)
			{
				m0 = new MenuItemWithBitmap(kv.Key, Resources._propAction.ToBitmap());
				m0.Click += new EventHandler(miSetProperty_Click);
				kv.Location = location;
				m0.Tag = kv;
				mi.MenuItems.Add(m0);
			}
			m1 = new MenuItemWithBitmap("More properties", Resources._properties.ToBitmap());
			mi.MenuItems.Add(m1);
			properties = SecondaryProperties;
			foreach (MenuItemDataProperty kv in properties)
			{
				m0 = new MenuItemWithBitmap(kv.Key, Resources._propAction.ToBitmap());
				m0.Click += new EventHandler(miSetProperty_Click);
				kv.Location = location;
				m0.Tag = kv;
				m1.MenuItems.Add(m0);
			}
			m0 = new MenuItemWithBitmap("*All properties* =>", Resources._dialog.ToBitmap());
			m1.MenuItems.Add(m0);
			MenuItemDataPropertySelector pAll = new MenuItemDataPropertySelector(m0.Text, this);
			pAll.Location = location;
			m0.Tag = pAll;
			m0.Click += new EventHandler(miSetProperties_Click);
			//
			mnu.MenuItems.Add(mi);
			#endregion
			//
			#region Assign Actions
			//hooking events is not done inside a method
			mi = new MenuItemWithBitmap("Assign Action", Resources._events.ToBitmap());
			List<MenuItemDataEvent> events = PrimaryEvents;
			foreach (MenuItemDataEvent kv in events)
			{
				m0 = new MenuItemWithBitmap(kv.Key, Resources._event1.ToBitmap());
				m0.Click += new EventHandler(miAssignEvent_Click);
				kv.Location = location;
				m0.Tag = kv;
				mi.MenuItems.Add(m0);
				EventItem ei = kv as EventItem;
				if (ei != null)
				{
					IEventInfoTree emi = ei.Value as IEventInfoTree;
					if (emi != null)
					{
						IEventInfoTree[] subs = emi.GetSubEventInfo();
						createEventTree(m0, location, kv.Owner, subs, new EventHandler(miAssignEvent_Click));
					}
				}
			}
			m1 = new MenuItem("More events");
			mi.MenuItems.Add(m1);
			events = SecondaryEvents;
			foreach (MenuItemDataEvent kv in events)
			{
				m0 = new MenuItemWithBitmap(kv.Key, Resources._event1.ToBitmap());
				m0.Click += new EventHandler(miAssignEvent_Click);
				kv.Location = location;
				m0.Tag = kv;
				m1.MenuItems.Add(m0);
				EventItem ei = kv as EventItem;
				if (ei != null)
				{
					IEventInfoTree emi = ei.Value as IEventInfoTree;
					if (emi != null)
					{
						IEventInfoTree[] subs = emi.GetSubEventInfo();
						createEventTree(m0, location, kv.Owner, subs, new EventHandler(miAssignEvent_Click));
					}
				}
			}
			m0 = new MenuItem("*All events* =>");
			m1.MenuItems.Add(m0);
			MenuItemDataEventSelector eAll = new MenuItemDataEventSelector(m0.Text, this);
			eAll.Location = location;
			m0.Tag = eAll;
			m0.Click += new EventHandler(miSetEvents_Click);
			mnu.MenuItems.Add(mi);
			#endregion
		}
		public void SetOwner(IClass type)
		{
			_owner = type;
			if (_primaryMethods != null && _primaryMethods.Count > 0)
			{
				foreach (MenuItemDataMethod m in _primaryMethods)
				{
					m.ResetOwner(type);
				}
			}
			if (_secondaryMethods != null && _secondaryMethods.Count > 0)
			{
				foreach (MenuItemDataMethod m in _secondaryMethods)
				{
					m.ResetOwner(type);
				}
			}

			if (_primaryEvents != null && _primaryEvents.Count > 0)
			{
				foreach (MenuItemDataEvent m in _primaryEvents)
				{
					m.ResetOwner(type);
				}
			}
			if (_secondaryEvents != null && _secondaryEvents.Count > 0)
			{
				foreach (MenuItemDataEvent m in _secondaryEvents)
				{
					m.ResetOwner(type);
				}
			}

			if (_primaryProperties != null && _primaryProperties.Count > 0)
			{
				foreach (MenuItemDataProperty m in _primaryProperties)
				{
					m.ResetOwner(type);
				}
			}
			if (_secondaryProperties != null && _secondaryProperties.Count > 0)
			{
				foreach (MenuItemDataProperty m in _secondaryProperties)
				{
					m.ResetOwner(type);
				}
			}
		}
		public XmlNode GetTypeNode()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode root;
			string path = FilePath;
			if (System.IO.File.Exists(path))
			{
				doc.Load(path);
			}
			root = doc.DocumentElement;
			if (root == null)
			{
				root = doc.CreateElement("Root");
				doc.AppendChild(root);
			}
			XmlNode nodeType = root.SelectSingleNode(RootName);
			if (nodeType == null)
			{
				nodeType = doc.CreateElement(RootName);
				root.AppendChild(nodeType);
			}
			return nodeType;
		}
		public void RemoveMenuCollection()
		{
			LimnorContextMenuCollection.RemoveMenuCollection(_owner);
		}
		public SortedDictionary<string, IEvent> GetAllEvents()
		{
			SortedDictionary<string, IEvent> all = new SortedDictionary<string, IEvent>();
			if (_owner.VariableWrapperType == null)
			{
				if (eventsC != null && eventsC.Count > 0)
				{
					foreach (KeyValuePair<string, EventClass> kv in eventsC)
					{
						all.Add(kv.Key, kv.Value);
					}
				}
				if (events != null && events.Count > 0)
				{
					foreach (KeyValuePair<string, EventInfo> kv in events)
					{
						EventPointer ep = new EventPointer();
						ep.SetEventInfo(kv.Value);
						ep.Owner = _owner;
						ep.SetObjectKey(kv.Key);
						all.Add(kv.Key, ep);
					}
				}
			}
			else
			{
			}
			return all;
		}
		public SortedDictionary<string, IProperty> GetAllProperties()
		{
			SortedDictionary<string, IProperty> all = new SortedDictionary<string, IProperty>();
			if (_owner.VariableWrapperType == null)
			{
				if (propertiesC != null && propertiesC.Count > 0)
				{
					foreach (KeyValuePair<string, PropertyClass> kv in propertiesC)
					{
						all.Add(kv.Key, kv.Value);
					}
				}
				if (properties != null && properties.Count > 0)
				{
					foreach (KeyValuePair<string, PropertyDescriptor> kv in properties)
					{
						PropertyPointer pp = new PropertyPointer();
						pp.SetPropertyInfo(kv.Value);
						pp.Owner = _owner;
						pp.SetObjectKey(kv.Key);
						all.Add(kv.Key, pp);
					}
				}
			}
			return all;
		}
		public SortedDictionary<string, IMethod> GetAllMethods()
		{
			SortedDictionary<string, IMethod> all = new SortedDictionary<string, IMethod>();
			if (_owner.VariableWrapperType == null)
			{
				if (methodsC != null && methodsC.Count > 0)
				{
					foreach (KeyValuePair<string, MethodClass> kv in methodsC)
					{
						all.Add(kv.Key, kv.Value);
					}
				}
			}
			if (methods != null && methods.Count > 0)
			{
				foreach (KeyValuePair<string, MethodInfo> kv in methods)
				{
					MethodInfoPointer mp = new MethodInfoPointer();
					mp.SetMethodInfo(kv.Value);
					mp.Owner = _owner;
					all.Add(kv.Key, mp);
				}
			}
			return all;
		}
		#endregion
	}
}
