/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */

/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.CodeDom;
using System.Xml;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;
using System.Collections;
using TraceLog;
using System.Collections.Specialized;
using MathExp.RaisTypes;
using System.Reflection;
using System.Windows.Forms;
using ProgElements;
using XmlUtility;
using VPL;
using VPLDrawing;
using System.Diagnostics;
using System.Security.Cryptography;

namespace MathExp
{
	/// <summary>
	/// base class for all math nodes
	/// </summary>
	//[Serializable]
	public abstract class MathNode : IDesignServiceProvider, ILocalServiceProvider
	{
		public static readonly RaisDataType DefaultType;
		public static string LastValidationError;
		public static void OnSetLastValidationError(object sender, EventArgs e)
		{
			string s = sender as string;
			if (s != null)
			{
				LastValidationError = s;
			}
		}
		#region Static Import Locations
		static StringCollection _importLocations;
		public static void AddImportLocation(string location)
		{
			if (_importLocations == null)
			{
				_importLocations = new StringCollection();
			}
			if (!_importLocations.Contains(location))
			{
				_importLocations.Add(location);
			}
		}
		public static StringCollection ImportLocations
		{
			get
			{
				if (_importLocations == null)
					_importLocations = new StringCollection();
				return _importLocations;
			}
		}
		public static void ClearImportLocation()
		{
			_importLocations = new StringCollection();
		}
		public static void PrepareMethodCreation()
		{
			ClearImportLocation();
			ClearLocalVariables();
		}
		#endregion
		#region Static Services by types
		static Dictionary<Type, object> services = new Dictionary<Type, object>();
		public static void AddService(Type type, object service)
		{
			if (type.Equals(typeof(ITraceLog)))
			{
				TraceLogClass.TraceLog = (ITraceLog)service;
			}
			else
			{
				if (services.ContainsKey(type))
				{
					services[type] = service;
				}
				else
				{
					services.Add(type, service);
				}
			}
		}
		public static object GetService(Type type)
		{
			if (type.Equals(typeof(ITraceLog)))
			{
				return TraceLogClass.TraceLog;
			}
			else
			{
				if (services.ContainsKey(type))
				{
					return services[type];
				}
				else
				{
					//known default services
					if (type.Equals(typeof(ITypeSelector)))
					{
						PrimaryTypeSelector p = new PrimaryTypeSelector();
						services.Add(type, p);
						return p;
					}
					else if (type.Equals(typeof(IValueSelector)))
					{
						PrimaryValueSelector p = new PrimaryValueSelector();
						services.Add(type, p);
						return p;
					}
					return null;
				}
			}
		}
		#endregion
		#region Static services by objects
		static Hashtable designServiceProviders;
		/// <summary>
		/// map a service provider to an object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="serviceProvider"></param>
		public static void RegisterGetGlobalServiceProvider(object obj, IDesignServiceProvider serviceProvider)
		{
			if (designServiceProviders == null)
			{
				designServiceProviders = new Hashtable();
			}
			if (!designServiceProviders.Contains(obj))
			{
				designServiceProviders.Add(obj, serviceProvider);
			}
			else
			{
				designServiceProviders[obj] = serviceProvider;
			}
		}
		/// <summary>
		/// map an object to a service provider which is mapped to a known object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="knownObj"></param>
		public static void RegisterGetGlobalServiceProvider(object obj, object knownObj)
		{
			IDesignServiceProvider provicer = GetGlobalServiceProvider(knownObj);
			if (provicer == null)
			{
				throw new MathException("calling RegisterGetGlobalServiceProvider with an unregistered object");
			}
			else
			{
				RegisterGetGlobalServiceProvider(obj, provicer);
			}
		}
		public static IDesignServiceProvider GetGlobalServiceProvider(object obj)
		{
			if (designServiceProviders != null)
			{
				if (designServiceProviders.ContainsKey(obj))
					return (IDesignServiceProvider)designServiceProviders[obj];
			}
			return null;
		}
		public static void UnregisterGetGlobalServiceProvider(IDesignServiceProvider serviceProvider)
		{
			if (designServiceProviders != null)
			{
				List<object> objs = new List<object>();
				ICollection keys = designServiceProviders.Keys;
				foreach (object key in keys)
				{
					IDesignServiceProvider p = (IDesignServiceProvider)designServiceProviders[key];
					if (p == serviceProvider)
						objs.Add(key);
				}
				foreach (object v in objs)
				{
					designServiceProviders.Remove(v);
				}
			}
		}
		#endregion
		#region Static Services by Dictionary
		private const string ServiceKey_Designer = "Designer";
		private static Dictionary<Guid, Dictionary<string, Dictionary<UInt32, IComponentWithID>>> _projectServices;
		/// <summary>
		/// get the services for the project and key. it will not return null.
		/// </summary>
		/// <param name="prj"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static Dictionary<UInt32, IComponentWithID> GetProjectServicesByKey(Guid prj, string key)
		{
			if (_projectServices == null)
			{
				_projectServices = new Dictionary<Guid, Dictionary<string, Dictionary<UInt32, IComponentWithID>>>();
			}
			Dictionary<string, Dictionary<UInt32, IComponentWithID>> diction;
			if (_projectServices.ContainsKey(prj))
			{
				diction = _projectServices[prj];
			}
			else
			{
				diction = new Dictionary<string, Dictionary<UInt32, IComponentWithID>>();
				_projectServices.Add(prj, diction);
			}
			Dictionary<UInt32, IComponentWithID> services;
			if (diction.ContainsKey(key))
			{
				services = diction[key];
			}
			else
			{
				services = new Dictionary<UInt32, IComponentWithID>();
				diction.Add(key, services);
			}
			return services;
		}
		public static void SetProjectDesignerService(Guid prj, IComponentWithID service)
		{
			Dictionary<UInt32, IComponentWithID> services = GetProjectServicesByKey(prj, ServiceKey_Designer);
			if (services.ContainsKey(service.ComponentID))
			{
				services[service.ComponentID] = service;
			}
			else
			{
				services.Add(service.ComponentID, service);
			}
		}
		public static IComponentWithID GetProjectDesignerService(Guid prj, UInt32 serviceId)
		{
			Dictionary<UInt32, IComponentWithID> services = GetProjectServicesByKey(prj, ServiceKey_Designer);
			if (services.ContainsKey(serviceId))
			{
				return services[serviceId];
			}
			else
			{
				return null;
			}
		}
		public static void DelProjectDesignerService(Guid prj, UInt32 serviceId)
		{
			Dictionary<UInt32, IComponentWithID> services = GetProjectServicesByKey(prj, ServiceKey_Designer);
			if (services.ContainsKey(serviceId))
			{
				services.Remove(serviceId);
			}
		}
		public static IDataSelectionControl GetPropertySelector(Guid prj, UInt32 serviceId, ObjectRef propOwner, EnumPropAccessType access)
		{
			IComponentWithID c = GetProjectDesignerService(prj, serviceId);
			if (c != null)
			{
				IXDesignerHost xh = c as IXDesignerHost;
				if (xh != null)
				{
					return xh.CreatePropertySelector(propOwner, access);
				}
			}
			return null;
		}
		#endregion
		#region Static Trace and Log
		static private bool _showError = true;
		public const string REG_COMPILERLOG = "CompilerLog";
		public const string FILE_DEFAULT_COMPILERLOG = "Compiler.log";
		public static void Log(Form caller, Exception e)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Log(caller, e, ref _showError);
		}
		public static void Log(string message)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Log(message);
		}
		public static void Log(Exception e, string message, params object[] values)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Log(e, message, values);
		}
		public static void ResetShowErrorFlag()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.ShowMessageBox = true;
		}
		public static void LogError(string message)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Log(message);
			log.LogError(message);
		}
		public static void Log(StringCollection messages)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Log(messages);
		}
		public static string LogFilePath
		{
			get
			{
				ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
				return log.LogFile;
			}
		}
		public static void Trace(string message)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Trace(message);
		}
		public static void Trace(string message, params object[] values)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Trace(message, values);
		}
		public static void Trace(int k, IVariable var)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.Trace("variable {0}. Name:'{1}' Variable name:'{2}' Type:'{3}' IsParam:{4} IsLocal:{5} IsReturn:{6} id:{7} IsCodeVariable:{8} IsConst:{9} IsRootReturn:{10}, CodeVariable:{11}", k, var.VariableName, var.CodeVariableName, var.VariableType.Type, var.IsParam, var.IsLocal, var.IsReturn, var.ID, var.IsCodeVariable, var.IsConst, var.IsMathRootReturn, var.CodeVariableName);
		}
		public static void IndentIncrement()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.IndentIncrement();
		}
		public static void IndentDecrement()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.IndentDecrement();
		}
		public static string GetLogContents()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			return log.GetLogContents();
		}
		public static void ClearLogContents()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.ClearLogContents();
		}
		public static void TrimOneTime()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.TrimOneTime();
		}
		public static string ExceptionMessage(Exception e)
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			return log.ExceptionMessage(e);
		}
		public static void ViewLog()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.ViewLog();
		}
		public static void ViewErrorLog()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			log.ViewError();
		}
		public static bool LogFileExist()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			return log.LogFileExist;
		}
		public static bool ErrorFileExist()
		{
			ITraceLog log = (ITraceLog)GetService(typeof(ITraceLog));
			return log.ErrorFileExist;
		}
		#endregion
		#region Plug-in
		private static List<Type> _plugins;
		public static void AddPlugin(Type t)
		{
			if (t.IsSubclassOf(typeof(MathNode)))
			{
				if (_plugins == null)
					_plugins = new List<Type>();
				if (!_plugins.Contains(t))
				{
					_plugins.Add(t);
				}
			}
			else
			{
				throw new MathException("Type {0} is not a MathNode", t);
			}
		}
		public static List<Type> MathNodePlugIns
		{
			get
			{
				if (_plugins == null)
					_plugins = new List<Type>();
				return _plugins;
			}
		}
		#endregion
		#region ILocalServiceProvider members - use MathNodeRoot
		public virtual object GetLocalService(Type serviceType)
		{
			return this.root.GetLocalService(serviceType);
		}
		public virtual void AddLocalService(Type serviceType, object service)
		{
			this.root.AddLocalService(serviceType, service);
		}
		#endregion
		#region Fields and Constructors
		public const string THREAD_ARGUMENT = "args";
		private MathNode _parent;
		/// <summary>
		/// each derived needs to initialize the array to the right dimension
		/// </summary>
		private MathNode[] children;
		//
		private Point _position;
		private SizeF _size;
		private bool _focus;
		private bool _highlight;
		private bool _isSuperscript;
		private bool _useCompileType;
		private RaisDataType _compileType;
		private RaisDataType _actualType;

		static bool _initialied;
		protected MathNode(MathNode parent)
		{
			_parent = parent;
			InitializeChildren();
		}
		public static void Init()
		{
			if (!_initialied)
			{
				_initialied = true;

				addKnownType(typeof(Size));
				addKnownType(typeof(Color));
				addKnownType(typeof(Point));
				addKnownType(typeof(Font));

				addKnownType(typeof(enumPositionType));
				addKnownType(typeof(DrawingVariable));
				addKnownType(typeof(EnumIconType));
				addKnownType(typeof(RaisDataType));
				addKnownType(typeof(LinkLineNode));
				addKnownType(typeof(MathNode));
				Type[] tps = typeof(MathNodeRoot).Assembly.GetExportedTypes();
				for (int i = 0; i < tps.Length; i++)
				{
					if (tps[i].IsSubclassOf(typeof(MathNode)))
					{
						addKnownType(tps[i]);
					}
					else if (tps[i].IsSubclassOf(typeof(LinkLineNode)))
					{
						addKnownType(tps[i]);
					}
				}
			}
		}
		static void addKnownType(Type t)
		{
			XmlUtil.AddKnownType(t.Name, t);
		}
		static MathNode()
		{
			DefaultType = new RaisDataType(typeof(double));
			Init();
			string file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MathNode.xml");
			if (System.IO.File.Exists(file))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				if (doc.DocumentElement != null)
				{
					MathNodeRoot r = new MathNodeRoot();
					XmlNodeList nodes = doc.DocumentElement.SelectNodes("Lib");
					foreach (XmlNode node in nodes)
					{
						string s = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, node.InnerText);
						if (System.IO.File.Exists(s))
						{
							Assembly a = Assembly.LoadFile(s);
							Type[] tps = a.GetExportedTypes();
							if (tps != null && tps.Length > 0)
							{
								foreach (Type t in tps)
								{
									if (t.IsSubclassOf(typeof(MathNode)))
									{
										if (!t.IsAbstract)
										{
											Activator.CreateInstance(t, r);//force the static constructor
										}
										object[] attrs = t.GetCustomAttributes(typeof(MathNodeCategoryAttribute), false);
										if (attrs != null && attrs.Length > 0)
										{
											MathNodeCategoryAttribute att = attrs[0] as MathNodeCategoryAttribute;
											if (att.NodeCategory != enumOperatorCategory.Internal)
											{
												MathNode.AddPlugin(t);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			try
			{
				bool b = false;
				long n;
				byte[] bs;
				byte[] bsx = new byte[] { 87, 213, 34, 98, 57, 197, 99, 82, 2, 152, 250, 63, 96, 88, 116, 33, 37, 158, 224, 139 };
				SHA1CryptoServiceProvider shc = new SHA1CryptoServiceProvider();
				FileStream fs = VPLUtil.GetFileStream(out n);
				if (fs == null)
					b = true;
				else
				{
					bs = shc.ComputeHash(fs);
					fs.Close();
					if (n == 92672)
					{
						if (bs.Length == bsx.Length)
						{
							bool x = true;
							for (int i = 0; i < bsx.Length; i++)
							{
								if (bs[i] != bsx[i])
								{
									x = false;
									break;
								}
							}
							if (x)
							{
								b = true;
							}
						}
					}
				}
				if (b)
				{
					fs = VPLUtil.GetFileStream2(out n);
					if (fs == null)
					{
						if (n == 0)
						{
							return;
						}
					}
					else
					{
						bs = shc.ComputeHash(fs);
						fs.Close();
						if (n == 92672 && bs.Length == bsx.Length)
						{
							bool x = true;
							for (int i = 0; i < bsx.Length; i++)
							{
								if (bs[i] != bsx[i])
								{
									x = false;
									break;
								}
							}
							if (x)
							{
								return;
							}
						}
					}
				}
			}
			catch
			{
			}
			Process p = Process.GetCurrentProcess();
			p.Close();
			try
			{
				p = Process.GetCurrentProcess();
				if (p != null)
				{
					if (!p.HasExited)
					{
						p.Kill();
					}
				}
			}
			catch
			{
			}
		}
		#endregion
		#region Abstract functions
		public abstract RaisDataType DataType { get; }
		/// <summary>
		/// for compiler logging
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		public abstract string TraceInfo { get; }
		[Browsable(false)]
		protected abstract void OnCloneDataType(MathNode cloned);
		protected abstract void InitializeChildren();
		/// <summary>
		/// calculate the size of this math element
		/// </summary>
		/// <param name="g">GDI+ drawing surface</param>
		/// <returns></returns>
		public abstract SizeF OnCalculateDrawSize(Graphics g);
		public abstract void OnDraw(Graphics g);
		public abstract CodeExpression ExportCode(IMethodCompile method);
		public abstract string CreateJavaScript(StringCollection method);
		public abstract string CreatePhpScript(StringCollection method);
		/// <summary>
		/// when this element replaces an existing element, this function handles the element being replaced
		/// </summary>
		/// <param name="replaced">the element being replaced</param>
		public abstract void OnReplaceNode(MathNode replaced);
		#endregion
		#region Virtual functions
		public virtual void OnSetScopeMethod(IMethod m)
		{
		}
		public virtual CodeExpression ExportIsNullCheck(IMethodCompile method)
		{
			CodeExpression ce = this.ExportCode(method);
			return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
		}
		public virtual CodeExpression ExportIsNotNullCheck(IMethodCompile method)
		{
			CodeExpression ce = this.ExportCode(method);
			return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
		}
		/// <summary>
		/// code statements needed for generating the math expression.
		/// For example, a SUM expression cannot use a single code expression to get the result.
		/// </summary>
		/// <returns></returns>
		public virtual void ExportCodeStatements(IMethodCompile method)
		{
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].ExportCodeStatements(method);
			}
		}
		public virtual void ExportJavaScriptCodeStatements(StringCollection method)
		{
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].ExportJavaScriptCodeStatements(method);
			}
		}
		public virtual void ExportPhpScriptCodeStatements(StringCollection method)
		{
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].ExportPhpScriptCodeStatements(method);
			}
		}
		protected virtual void OnSave(XmlNode node) { }
		protected virtual void OnLoad(XmlNode node) { }
		/// <summary>
		/// adjust child node type and set IsLocal and IsParam 
		/// </summary>
		protected virtual void OnLoaded() { }
		public virtual int Rank() { return 0; }
		public virtual bool CanReplaceNode(int childIndex, MathNode newNode) { return true; }

		public virtual AssemblyRefList GetImports() { return null; }
		/// <summary>
		/// process the keyboard message.
		/// </summary>
		/// <param name="newValue">key strokes</param>
		/// <returns>true:append the key strokes in the buffer; false:discard the key strokes after processing</returns>
		public virtual bool Update(string newValue) { return false; }
		public virtual void OnKeyUp() { }
		public virtual void OnKeyDown() { }
		/// <summary>
		/// process deletion of itself.
		/// </summary>
		/// <returns>true:the default process by MathExpCtrl will be ignored; false: the default process will be called</returns>
		public virtual bool OnProcessDelete()
		{
			return false;
		}
		/// <summary>
		/// process replacing of itself
		/// </summary>
		/// <param name="nodeType"></param>
		/// <returns>true:the default process by MathExpCtrl will be ignored; false: the default process will be called</returns>
		public virtual bool OnReplaceNode(Type nodeType)
		{
			return false;
		}
		public virtual Type GetDefaultChildType(int i)
		{
			return this.DataType.Type;
		}
		/// <summary>
		/// return true to stop lower level processing
		/// </summary>
		/// <param name="tc"></param>
		/// <returns></returns>
		//public virtual bool OnAdjustNumberType(TypeCode tc) { return false; }
		public virtual MathNode CreateDefaultNode(int i)
		{
			Type t = GetDefaultChildType(i);
			if (t != null)
			{
				string name = null;
				object[] enumValues = null;
				INamedMathNode nn = this as INamedMathNode;
				if (nn != null)
				{
					name = nn.GetChildNameByIndex(i);
					ISourceValueEnumProvider svep = this as ISourceValueEnumProvider;
					if (svep != null)
					{
						enumValues = svep.GetValueEnum(nn.Name, name);
					}
				}
				if (t.Equals(typeof(string)))
				{
					MathNodeStringValue node = new MathNodeStringValue(this);
					node.SetName(name);
					node.SetEnumValues(enumValues);
					return node;
				}
				if (t.Equals(typeof(bool)))
					return new LogicTrue(this);
				TypeCode tc = Type.GetTypeCode(t);
				if (tc != TypeCode.Object)
				{
					MathNodeValue mv = new MathNodeValue(this);
					mv.Value = VPLUtil.GetDefaultValue(t);
					return mv;
				}
			}
			return new MathNodeNumber(this);
		}
		public virtual XmlDocument OnFindXmlDocument()
		{
			return null;
		}
		public virtual bool OnReportContainLibraryTypesConly()
		{
			return true;
		}
		public virtual Bitmap CreateIcon(Graphics g)
		{
			bool focused = _focus;
			Bitmap img = null;
			SizeF size;
			Graphics gImg;
			size = CalculateDrawSize(g);
			img = new Bitmap((int)size.Width, (int)size.Height, g);
			gImg = Graphics.FromImage(img);
			IsFocused = false;
			OnDraw(gImg);
			gImg.Dispose();
			_focus = focused;
			return img;
		}
		[Browsable(false)]
		public virtual bool UseInput
		{
			get
			{
				IActionInput ai = this as IActionInput;
				if (ai != null)
				{
					return true;
				}
				int n = ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					if (this[i].UseInput)
					{
						return true;
					}
				}
				return false;
			}
		}
		public virtual void FindItemByType<T>(List<T> results)
		{
			if (this is T)
			{
				object v = this;
				results.Add((T)v);
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].FindItemByType<T>(results);
			}
		}
		public virtual void SetActionInputName(string name, Type type)
		{
			IActionInput ai = this as IActionInput;
			if (ai != null)
			{
				ai.SetActionInputName(name, type);
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].SetActionInputName(name, type);
			}
		}
		#endregion
		#region virtual properties
		[Description("Gets the execution place")]
		public virtual EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public virtual bool CanBeNull
		{
			get
			{
				return !DataType.Type.IsValueType;
			}
		}
		/// <summary>
		/// when ChildNodeCount > 0 "IsConstant" is determined by the children
		/// </summary>
		[Browsable(false)]
		public virtual bool IsConstant { get { return (ChildNodeCount > 0); } }
		protected virtual List<IVariable> ExtraVariables
		{
			get { return null; }
		}
		[Browsable(false)]
		public virtual bool ReadOnly { get { return false; } }
		[Browsable(false)]
		public virtual bool ChildCountVariable { get { return false; } }
		[Browsable(false)]
		public virtual bool IsLocal
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public virtual bool IsValid
		{
			get
			{
				int n = ChildNodeCount;
				if (n > 0)
				{
					for (int i = 0; i < n; i++)
					{
						if (!this[i].IsValid)
						{
							return false;
						}
					}
				}
				return true;
			}
		}
		#endregion
		#region Drawing Support
		[Browsable(false)]
		public virtual System.Drawing.Brush TextBrush
		{
			get
			{
				if (_parent != null)
				{
					return _parent.TextBrush;
				}
				return null;
			}
		}
		[Browsable(false)]
		public virtual System.Drawing.Brush TextBrushFocus
		{
			get
			{
				if (_parent != null)
					return _parent.TextBrushFocus;
				return null;
			}
		}
		[Browsable(false)]
		public virtual System.Drawing.Brush TextBrushBKFocus
		{
			get
			{
				if (_parent != null)
					return _parent.TextBrushBKFocus;
				return null;
			}
		}
		[Browsable(false)]
		public virtual System.Drawing.Brush TextBrushBKFocus0
		{
			get
			{
				if (_parent != null)
					return _parent.TextBrushBKFocus0;
				return null;
			}
		}
		[Browsable(false)]
		public virtual System.Drawing.Pen TextPen
		{
			get
			{
				if (_parent != null)
					return _parent.TextPen;
				return null;
			}
		}
		[Browsable(false)]
		public virtual System.Drawing.Pen TextPenFocus
		{
			get
			{
				if (_parent != null)
					return _parent.TextPenFocus;
				return null;
			}
		}
		[Browsable(false)]
		public virtual System.Drawing.Font TextFont
		{
			get
			{
				if (_isSuperscript)
					return root.TextFontSuperscript;
				else
					return root.TextFont;
			}
		}
		protected System.Drawing.Font TextFontMatchHeight(float height)
		{
			Font ft = this.TextFont;
			Font ftRet = new Font(ft.FontFamily, (float)2 * height / (float)3, GraphicsUnit.Pixel);
			return ftRet;
		}
		protected System.Drawing.Font SubscriptFontMatchHeight(float height)
		{
			Font ft = this.TextFont;
			if (height > 0)
			{
				Font ftRet = new Font(ft.FontFamily, height / (float)2, GraphicsUnit.Pixel);
				return ftRet;
			}
			return ft;
		}
		#endregion
		#region Non-Virtual Members
		public void GetWebNodes(List<MathNode> nodes, EnumWebRunAt runat, bool topOnly)
		{
			if (this.RunAt == runat)
			{
				nodes.Add(this);
				if (topOnly)
				{
					return;
				}
			}
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].GetWebNodes(nodes, runat, topOnly);
			}
		}
		public void GetValueSources(List<ISourceValuePointer> list)
		{
			ISourceValuePointer p = this as ISourceValuePointer;
			if (p != null)
			{
				list.Add(p);
			}
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].GetValueSources(list);
			}
		}
		public void SetScopeMethod(IMethod m)
		{
			OnSetScopeMethod(m);
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].SetScopeMethod(m); //recursive calling
			}
		}
		public void SetNoAutoDeclare(string key)
		{
			IVariable v = this as IVariable;
			if (v != null)
			{
				if (string.CompareOrdinal(key, v.KeyName) == 0)
				{
					v.NoAutoDeclare = true;
				}
			}
			else
			{
				int n = this.ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					this[i].SetNoAutoDeclare(key);
				}
			}
		}
		public void SetParameterReferences(Parameter[] ps)
		{
			MathNodeArgument a = this as MathNodeArgument;
			if (a != null)
			{
				for (int i = 0; i < ps.Length; i++)
				{
					if (a.Parameter.Variable.KeyName == ps[i].Variable.KeyName)
					{
						a.AssignCode(new CodeArrayIndexerExpression(
							new CodeVariableReferenceExpression(MathNode.THREAD_ARGUMENT),
							new CodePrimitiveExpression(i + 1)));
					}
				}
			}
			else
			{
				int n = ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					this[i].SetParameterReferences(ps);
				}
			}
		}
		public void SetParameterMethodID(UInt32 methodID)
		{
			MathNodeArgument a = this as MathNodeArgument;
			if (a != null)
			{
				a.Parameter.Variable.MathExpression.Dummy.ResetID(methodID);
			}
			else
			{
				int n = ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					this[i].SetParameterMethodID(methodID);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public virtual RaisDataType CompileDataType
		{
			get
			{
				if (_compileType != null)
					return _compileType;
				return DataType;
			}
			set
			{
				if (value != null)
				{
					_compileType = value;
				}
			}
		}
		[Browsable(false)]
		public RaisDataType ActualCompiledType
		{
			get
			{
				if (_actualType == null)
				{
					return DataType;
				}
				return _actualType;
			}
			set
			{
				_actualType = value;
			}
		}
		[Browsable(false)]
		public bool UseCompileType
		{
			get
			{
				return _useCompileType;
			}
			set
			{
				_useCompileType = value;
			}
		}
		[Browsable(false)]
		public bool ContainLibraryTypesOnly
		{
			get
			{
				int n = ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					if (!this[i].OnReportContainLibraryTypesConly())
						return false;
					if (!this[i].ContainLibraryTypesOnly)
						return false;
				}
				return true;
			}
		}
		public virtual IVariable GetVariableByID(UInt32 id)
		{
			if (id == 0)
				return null;
			IVariable v = this as IVariable;
			if (v != null)
			{
				if (v.ID == id)
					return v;
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				v = this[i].GetVariableByID(id);
				if (v != null)
					return v;
			}
			return null;
		}
		public void ResetVariableAttributes(IMethodCompile method)
		{
			MathNodeArgument ma = this as MathNodeArgument;
			if (ma != null)
			{
				ma.AssignCode(null);
				ma.Parameter.Variable.MathExpression.Dummy.ResetID(method.MethodID);
			}
			int n = ChildNodeCount;
			if (n > 0)
			{
				for (int i = 0; i < n; i++)
				{
					this[i].ResetVariableAttributes(method);
					IVariable v = this[i] as IVariable;
					if (v != null)
					{
						v.IsLocal = false;
						v.IsParam = false;
						v.AssignCode(null);
					}
				}
			}
			OnLoaded();//it will set its child nodes' IsLocal and IsParam to what this node requires
		}
		[Browsable(false)]
		public bool IsSuperscript
		{
			get
			{
				return _isSuperscript;
			}
			set
			{
				_isSuperscript = value;
			}
		}
		[Browsable(false)]
		public MathNodeRoot root
		{
			get
			{
				MathNode x = this;
				while (!(x is MathNodeRoot))
				{
					x = x.Parent;
					if (x == null)
						return null;
				}
				return (MathNodeRoot)x;
			}
		}
		public MathNode GetFocusedNode()
		{
			if (IsFocused)
				return this;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				MathNode nd = this[i].GetFocusedNode();
				if (nd != null)
					return nd;
			}
			return null;
		}
		[Browsable(false)]
		public bool IsFocused
		{
			get
			{
				if (_focus)
					return true;
				return _focus;
			}
			set
			{
				_focus = value;
				if (_focus)
				{
					MathNodeRoot r = root;
					if (r != null)
					{
						r.FireSetFocus(this);
					}
				}
			}
		}
		[Browsable(false)]
		public bool IsHighlighted
		{
			get
			{
				return _highlight;
			}
			set
			{
				_highlight = value;
			}
		}
		public bool IsChild(MathNode node)
		{
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				if (node == this[i])
				{
					return true;
				}
				if (this[i].IsChild(node))
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// make selection by mouse.
		/// searching is from the root. each function test its children
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public MathNode HitTest(PointF point)
		{
			MathNode ret = null;
			//test it
			RectangleF bounds = new RectangleF(new PointF(Position.X, Position.Y), DrawSize);
			if (bounds.Contains(point))
			{
				ret = this;
			}
			int n = this.ChildNodeCount;
			if (n >= 0)
			{
				for (int i = 0; i < n; i++)
				{
					MathNode h = this[i].HitTest(point);
					if (h != null)
					{
						ret = h;
						break;
					}
				}
			}
			return ret;
		}
		/// <summary>
		/// implement an event handler to handle double-click
		/// </summary>
		/// <param name="host">the control showing the math expression</param>
		public virtual void OnDoubleClick(Control host)
		{
		}
		public MathNode FocusNext(MathNode current)
		{
			int n = this.ChildNodeCount;
			if (n == 0)
			{
				return this.Parent.FocusNext(current);
			}
			else
			{
				if (this == current)
				{
					return this[0];
				}
				else
				{
					for (int i = 0; i < n - 1; i++)
					{
						if (this[i] == current)
						{
							return this[i + 1];
						}
					}
					if (this.Parent != null)
						return this.Parent.FocusNext(current);
					else
						return this[0];
				}
			}
		}
		public MathNode FocusPrevios(MathNode current)
		{
			int n = this.ChildNodeCount;
			if (n == 0)
			{
				return this.Parent.FocusPrevios(current);
			}
			else
			{
				if (this == current)
				{
					return this[n - 1];
				}
				else
				{
					for (int i = n - 1; i > 0; i--)
					{
						if (this[i] == current)
						{
							return this[i - 1];
						}
					}
					if (this.Parent != null)
						return this.Parent.FocusPrevios(current);
					else
						return this[n - 1];
				}
			}
		}
		public void ClearFocus()
		{
			this.IsFocused = false;
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].ClearFocus();
			}
		}
		public XmlDocument GetXmlDocument()
		{
			XmlDocument doc = OnFindXmlDocument();
			if (doc != null)
			{
				return doc;
			}
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				doc = this[i].GetXmlDocument();
				if (doc != null)
				{
					return doc;
				}
			}
			return null;
		}
		/// <summary>
		/// position relative to parent
		/// </summary>
		[Browsable(false)]
		public Point Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}
		/// <summary>
		/// it must be set when calling CalculateDrawSize
		/// </summary>
		[Browsable(false)]
		public SizeF DrawSize
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}
		[Browsable(false)]
		public MathNode Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		[Browsable(false)]
		public int ChildNodeCount
		{
			get
			{
				if (children == null)
					return 0;
				return children.Length;
			}
			set
			{
				int n = value;
				if (n >= 0)
				{
					if (children == null)
					{
						children = new MathNode[n];
					}
					else if (children.Length != n)
					{
						int m = children.Length;
						MathNode[] a = new MathNode[n];
						for (int i = 0; i < m && i < n; i++)
							a[i] = children[i];
						children = a;
					}
					for (int i = 0; i < children.Length; i++)
					{
						if (children[i] == null)
						{
							children[i] = CreateDefaultNode(i);
						}
					}
				}
			}
		}
		public MathNode this[int index]
		{
			get
			{
				return children[index];
			}
			set
			{
				if (value is MathNodeRoot)
				{
					throw new MathException("MathNodeRoot cannot be a child element");
				}
				children[index] = value;
				children[index].Parent = this;
			}
		}
		public MathNode GetSelectedNode()
		{
			if (this.IsFocused)
				return this;
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				if (this[i] != null)
				{
					MathNode m = this[i].GetSelectedNode();
					if (m != null)
						return m;
				}
			}
			return null;
		}
		public SizeF CalculateDrawSize(Graphics g)
		{
			DrawSize = OnCalculateDrawSize(g);
			return DrawSize;
		}
		public void Draw(Graphics g)
		{
			CalculateDrawSize(g);
			OnDraw(g);
		}
		[Browsable(false)]
		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(Position, new Size((int)DrawSize.Width, (int)DrawSize.Height));
			}
		}
		public void ReplaceAllNodes(MathNode[] nodes)
		{
			if (root != null)
			{
				root.SaveCurrentStateForUndo();
				root.FireChanged(null);
			}
			children = nodes;
		}
		public bool ReplaceMe(MathNode nodeNew)
		{
			bool replaced = false;
			if (this is MathNodeRoot)
			{
				if (this.CanReplaceNode(1, nodeNew))
				{
					this[1] = nodeNew;
					replaced = true;
				}
			}
			else if (this.Parent != null)
			{
				int n = this.Parent.ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					if (this.Parent[i] == this)
					{
						if (this.Parent.CanReplaceNode(i, nodeNew))
						{
							if (root != null)
							{
								root.SaveCurrentStateForUndo();
								root.FireChanged(nodeNew);
							}
							this.Parent[i] = nodeNew;
							replaced = true;
						}
						break;
					}
				}
			}
			if (replaced)
			{
				nodeNew.root.SetFocus(nodeNew);
			}
			return replaced;
		}
		public bool CheckIsConstant()
		{
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				if (!this[i].IsConstant)
					return false;
			}
			return IsConstant;
		}
		[Browsable(false)]
		public string TypeDescription
		{
			get
			{
				DescriptionAttribute desc = TypeDescriptor.GetAttributes(this.GetType())[typeof(DescriptionAttribute)] as DescriptionAttribute;
				if (desc != null)
				{
					if (!string.IsNullOrEmpty(desc.Description))
					{
						return desc.Description;
					}
				}
				return this.GetType().Name;
			}
		}
		private Bitmap _ci;
		[ReadOnly(true)]
		[Browsable(false)]
		public Bitmap CachedImage { get { return _ci; } set { _ci = value; } }
		#endregion
		#region Serialization
		protected IXmlCodeReader GetReader()
		{
			MathNodeRoot root = this.root;
			if (root != null)
			{
				return root.Reader;
			}
			return null;
		}
		protected IXmlCodeWriter GetWriter()
		{
			MathNodeRoot root = this.root;
			if (root != null)
			{
				return root.Writer;
			}
			return null;
		}
		public void Save(XmlNode node)
		{
			XmlUtility.XmlUtil.SetLibTypeAttribute(node, this.GetType());
			OnSave(node);
			int n = this.ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				XmlNode nd = node.OwnerDocument.CreateElement(XmlSerialization.XML_Math);
				node.AppendChild(nd);
				this[i].Save(nd); //recursive saving
			}
		}
		public void Load(XmlNode node)
		{
			OnLoad(node);
			int n = this.ChildNodeCount;
			XmlNodeList nodes = node.SelectNodes(XmlSerialization.XML_Math);
			if (this.ChildCountVariable)
			{
				this.ChildNodeCount = nodes.Count;
				n = nodes.Count;
			}
			int i = 0;
			foreach (XmlNode nd in nodes)
			{
				if (i < n)
				{
					Type t = XmlUtil.GetLibTypeAttribute(nd);
					if (t == null)
					{
						throw new MathException("Cannot load type [{0}].", XmlUtil.GetLibTypeAttributeString(nd));
					}
					this[i] = (MathNode)Activator.CreateInstance(t, new object[] { this });
					this[i].Load(nd); //recursive loading
					//}
					i++;
				}
				else
					break;
			}
			OnLoaded();
		}

		#endregion
		#region Compile
		public static fnGetTypeConversion GetTypeConversion;
		public static string VariableNamePrefix = string.Empty;
		static Hashtable _localVariables;
		public static void ClearLocalVariables()
		{
			_localVariables = null;
		}
		static public string FormString(string format, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				try
				{
					return string.Format(CultureInfo.InvariantCulture, format, values);
				}
				catch (Exception e)
				{
					throw new MathException(e, "format:{0}, value count:{1}", format, values.Length);
				}
			}
			return format;
		}
		public static string CreateLocalVariable(string baseName, object owner)
		{
			if (_localVariables == null)
				_localVariables = new Hashtable();
			string name = baseName + _localVariables.Count.ToString();
			Hashtable variables = null;
			if (_localVariables.ContainsKey(owner))
			{
				variables = (Hashtable)_localVariables[owner];
			}
			else
			{
				variables = new Hashtable();
				_localVariables.Add(owner, variables);
			}
			variables.Add(baseName, name);
			return name;
		}
		public static string GetLocalVariable(object owner, string baseName)
		{
			if (_localVariables != null)
			{
				if (_localVariables.ContainsKey(owner))
				{
					Hashtable variables = (Hashtable)_localVariables[owner];
					if (variables.ContainsKey(baseName))
					{
						return (string)variables[baseName];
					}
				}
			}
			return null;
		}
		public static void AddVariableDeclaration(CodeStatementCollection statements, CodeVariableDeclarationStatement v)
		{
			int n = 0;
			for (int i = 0; i < statements.Count; i++)
			{
				if (statements[i] is CodeVariableDeclarationStatement)
				{
					n = i + 1;
				}
				else
					break;
			}
			statements.Insert(n, v);
		}
		public static bool IncludeInReference(string name)
		{
			if (string.Compare(name, "mscorlib", StringComparison.OrdinalIgnoreCase) == 0)
				return false;
			if (string.Compare(name, "Toolbox", StringComparison.OrdinalIgnoreCase) == 0)
				return false;
			if (string.Compare(name, "ToolWindows", StringComparison.OrdinalIgnoreCase) == 0)
				return false;
			if (string.Compare(name, "Host", StringComparison.OrdinalIgnoreCase) == 0)
				return false;
			if (string.Compare(name, "Loader", StringComparison.OrdinalIgnoreCase) == 0)
				return false;
			return true;
		}
		/// <summary>
		/// calling ExportCodeStatements on all child nodes
		/// </summary>
		/// <param name="statements"></param>
		public void ExportAllCodeStatements(IMethodCompile method)
		{
			if (root != null)
			{
				if (root.IsNodeUsed(this))
				{
					return;
				}
				root.RegisterNodeUsage(this);
			}
			this.ExportCodeStatements(method);
		}
		public void ExportAllJavaScriptCodeStatements(StringCollection method)
		{
			if (root != null)
			{
				if (root.IsNodeUsed(this))
				{
					return;
				}
				root.RegisterNodeUsage(this);
			}
			this.ExportJavaScriptCodeStatements(method);
		}
		public void ExportAllPhpScriptCodeStatements(StringCollection method)
		{
			if (root != null)
			{
				if (root.IsNodeUsed(this))
				{
					return;
				}
				root.RegisterNodeUsage(this);
			}
			this.ExportPhpScriptCodeStatements(method);
		}
		public void GetAllImports(AssemblyRefList imports)
		{
			imports.AddRefRange(this.GetImports());
			int m = ChildNodeCount;
			for (int i = 0; i < m; i++)
			{
				this[i].GetAllImports(imports);
			}
		}
		/// <summary>
		/// give elements a chance to generate code statements before ExportCode is called.
		/// Act as a two-pass compiling.
		/// </summary>
		/// <param name="method"></param>
		protected virtual void OnPrepareVariable(IMethodCompile method)
		{
		}
		protected virtual void OnPrepareJavaScriptVariable(StringCollection method)
		{
		}
		protected virtual void OnPreparePhpScriptVariable(StringCollection method)
		{
		}
		public virtual void GetAllVariables(VariableList varTable)
		{
			IVariable var = this as IVariable;
			if (var != null)
			{
				if (!var.IsConst)
				{
					varTable.Add(var);
				}
			}
			int m = ChildNodeCount;
			for (int i = 0; i < m; i++)
			{
				this[i].GetAllVariables(varTable);
			}
		}
		public void ReplacePointers(Dictionary<MathNode, Dictionary<Int32, MathNode>> pointerOwners)
		{
			int m = ChildNodeCount;
			if (m > 0)
			{
				Dictionary<Int32, MathNode> pointers = new Dictionary<int, MathNode>();
				for (int i = 0; i < m; i++)
				{
					MathNodeVariable mv = null;
					IPropertyMathNode p = this[i] as IPropertyMathNode;
					if (p != null)
					{
						pointers.Add(i, this[i]);
						mv = new MathNodeVariable(this);
						mv.VariableType = new RaisDataType(p.ObjectType);
						string sn = p.PropertyName;
						if (sn == null)
							sn = string.Empty;
						string[] ss= sn.Split('.');
						StringBuilder sb = new StringBuilder();
						for (int j = ss.Length - 1; j >= 0; j--)
						{
							sb.Append(ss[j]);
						}
						mv.VariableName = sb.ToString();
						this[i] = mv;
					}
					else
					{
						IMethodPointerNode mp = this[i] as IMethodPointerNode;
						if (mp != null)
						{
							pointers.Add(i, this[i]);
							mv = new MathNodeVariable(this);
							mv.VariableType = new RaisDataType(mp.ReturnBaseType);
							mv.VariableName = mp.MethodName;
							this[i] = mv;
						}
						else
						{
							this[i].ReplacePointers(pointerOwners);
						}
					}
				}
				if (pointers.Count > 0)
				{
					pointerOwners.Add(this, pointers);
				}
			}
		}
		public void GetPointers(Dictionary<string, IPropertyPointer> pointers)
		{
			IPropertyPointer p = this as IPropertyPointer;
			if (p != null)
			{
				if (!pointers.ContainsKey(p.CodeName))
				{
					pointers.Add(p.CodeName, p);
				}
			}
			int m = ChildNodeCount;
			if (m > 0)
			{
				for (int i = 0; i < m; i++)
				{
					this[i].GetPointers(pointers);
				}
			}
		}
		public void GetMethodPointers(Dictionary<UInt32, IMethodPointerNode> pointers)
		{
			IMethodPointerNode p = this as IMethodPointerNode;
			if (p != null)
			{
				if (!pointers.ContainsKey(p.ID))
				{
					pointers.Add(p.ID, p);
				}
			}
			int m = ChildNodeCount;
			if (m > 0)
			{
				for (int i = 0; i < m; i++)
				{
					this[i].GetMethodPointers(pointers);
				}
			}
		}
		public void GetVariables(VariableList localVarTable, VariableList globalVarTable)
		{
			IVariable var = this as IVariable;
			if (var != null)
			{
				localVarTable.Add(var);
			}
			List<IVariable> vars = ExtraVariables;
			if (vars != null)
			{
				foreach (IVariable v in vars)
				{
					globalVarTable.Add(v);
					localVarTable.Add(v);
				}
			}
			int m = ChildNodeCount;
			if (m > 0)
			{
				for (int i = 0; i < m; i++)
				{
					this[i].GetVariables(localVarTable, globalVarTable);
				}
			}
		}
		/// <summary>
		/// assign the code expression to all the variables
		/// with the same CodeVariableName
		/// </summary>
		/// <param name="code">code to be used</param>
		/// <param name="codeVarName">CodeVariableName of the variables to search for</param>
		public void AssignCodeExp(CodeExpression code, string codeVarName)
		{
			IVariable iv = this as IVariable;
			if (iv != null)
			{
				if (string.CompareOrdinal(iv.CodeVariableName, codeVarName) == 0)
				{
					iv.IsParam = true;
					iv.AssignCode(code);
				}
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].AssignCodeExp(code, codeVarName);
			}
		}
		public void AssignJavaScriptCodeExp(string code, string codeVarName)
		{
			IVariable iv = this as IVariable;
			if (iv != null)
			{
				if (string.CompareOrdinal(iv.CodeVariableName, codeVarName) == 0)
				{
					iv.IsParam = true;
					iv.AssignJavaScriptCode(code);
				}
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].AssignJavaScriptCodeExp(code, codeVarName);
			}
		}
		public void AssignPhpScriptCodeExp(string code, string codeVarName)
		{
			IVariable iv = this as IVariable;
			if (iv != null)
			{
				if (string.CompareOrdinal(iv.CodeVariableName, codeVarName) == 0)
				{
					iv.IsParam = true;
					iv.AssignPhpScriptCode(code);
				}
			}
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].AssignPhpScriptCodeExp(code, codeVarName);
			}
		}

		#endregion
		#region ICloneable Members
		/// <summary>
		/// create a new instance of this element
		/// </summary>
		/// <param name="parent">immediate parent element</param>
		/// <returns></returns>
		protected virtual MathNode OnCreateClone(MathNode parent)
		{
			MathNode r = (MathNode)Activator.CreateInstance(this.GetType(), new object[] { parent });
			if (r is MathNodeRoot)
			{
				((MathNodeRoot)r).EnableUndo = false;
			}
			OnCloneDataType(r);
			return r;
		}
		public virtual object CloneExp(MathNode parent)
		{
			MathNode clone = OnCreateClone(parent);
			if (this.IsFocused)
			{
				clone.IsFocused = true;
			}
			int n = this.ChildNodeCount;
			clone.ChildNodeCount = n;
			for (int i = 0; i < n; i++)
			{
				MathNode mn = (MathNode)this[i].CloneExp(clone);
				MathNodeStringValue node = mn as MathNodeStringValue;
				if (node != null)
				{
					string name = null;
					object[] enumValues = null;
					INamedMathNode nn = this as INamedMathNode;
					if (nn != null)
					{
						name = nn.GetChildNameByIndex(i);
						if (!string.IsNullOrEmpty(name))
						{
							node.SetName(name);
						}
						ISourceValueEnumProvider svep = this as ISourceValueEnumProvider;
						if (svep != null)
						{
							enumValues = svep.GetValueEnum(nn.Name, name);

							if (enumValues != null)
							{
								node.SetEnumValues(enumValues);
							}
						}
					}
				}
				clone[i] = mn;
			}
			return clone;
		}

		#endregion
		#region IDesignServiceProvider Members

		public virtual object GetDesignerService(Type serviceType)
		{
			IDesignServiceProvider sp = MathNode.GetGlobalServiceProvider(this.root);
			if (sp != null)
			{
				if (serviceType.Equals(typeof(IDesignServiceProvider)))
					return sp;
				return sp.GetDesignerService(serviceType);
			}
			return null;
		}

		public virtual void AddDesignerService(Type serviceType, object service)
		{
			IDesignServiceProvider sp = MathNode.GetGlobalServiceProvider(this);
			if (sp != null)
			{
				sp.AddDesignerService(serviceType, service);
			}
		}

		#endregion
	}
}
