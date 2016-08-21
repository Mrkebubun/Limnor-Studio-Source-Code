/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Configuration;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Security.Principal;
using System.Security;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.AccessControl;
using VPL;
using System.CodeDom;
using System.Security.Cryptography;
using XmlUtility;
using System.Globalization;

namespace Limnor.Application
{
	[ToolboxBitmapAttribute(typeof(ApplicationConfiguration), "Resources.cfg.bmp")]
	[Description("This object lets applications save and load runtime configurations.")]
	public class ApplicationConfiguration : IComponent, ICustomTypeDescriptor, IIndexerAsProperty, ICustomPropertyCollection
	{
		#region fields and constructors
		public const string XMLATT_Password = "password";
		private static EnumProfileType _profileType = EnumProfileType.Default;
		private static CategoryList _config;
		private static bool _checkedLastRun;
		private static SecureString _password = null;
		private static string _enckey;
		private static string _profileName;
		private CategoryList _categories;
		private string _exefileFullpath;
		private static StringCollection _reservedWord;
		private bool _forIDE;
		//
		[Description("It occurs when a configuration file is loaded. It also occurs when ExecuteLoadingConfigurations method is called. Usually all actions for loading configurations into the application are assigned to this event.")]
		public static event EventHandler LoadingConfigurations;
		//
		[Description("It occurs when ExecuteSavingConfigurations method is executed. Usually all actions for saving configuration values are assigned to this event.")]
		public static event EventHandler SavingConfigurations;
		static ApplicationConfiguration()
		{
			_reservedWord = new StringCollection();
			_reservedWord.Add("ApplicationGuid");
			_reservedWord.Add("ConfigurationProfile");
			_reservedWord.Add("ProfileType");
			_reservedWord.Add("Filename");
			_reservedWord.Add("Configuration");
			_reservedWord.Add("Configurations");
		}
		public static bool IsReservedWord(string word)
		{
			return _reservedWord.Contains(word);
		}
		public ApplicationConfiguration()
		{

		}
		public ApplicationConfiguration(bool forIDE)
		{
			_forIDE = forIDE;
		}
		#endregion
		#region Encryption
		private static void resetPassword()
		{
			_password = null;
			_enckey = null;
		}
		[Browsable(false)]
		public static bool CanEncrypt
		{
			get
			{
				return (_password != null);
			}
		}
		private static string GetEncryptKey()
		{
			if (string.IsNullOrEmpty(_enckey))
			{
				if (_password != null)
				{
					IntPtr h = Marshal.SecureStringToBSTR(_password);
					_enckey = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Limnor {0} Studio", Marshal.PtrToStringBSTR(h));
					Marshal.ZeroFreeBSTR(h);
				}
			}
			return _enckey;
		}
		[Browsable(false)]
		public static string Encrypt(string s)
		{
			return SimpleEncryption.Encrypt(s, GetEncryptKey());
		}
		[Browsable(false)]
		public static string Decrypt(string s)
		{
			return SimpleEncryption.Decrypt(s, GetEncryptKey());
		}
		#endregion
		#region Properties
		private CategoryList DefaultAppConfig
		{
			get
			{
				if (_categories == null)
				{
					CategoryList cat = new CategoryList();
					if (!cat.LoadFromFile(defaultProfilePath))
					{
						if (VPLUtil.IsInDesignMode(this) && !_forIDE)
						{
							cat.Save();
						}
					}
					_categories = cat;
				}
				return _categories;
			}
		}
		[Browsable(false)]
		public static string ConfigFilePath
		{
			get
			{
				if (_config != null)
				{
					return _config.FilePath;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public static SecureString Password
		{
			get
			{
				return _password;
			}
		}
		[Browsable(false)]
		public string AppKey
		{
			get
			{
				return ApplicationGuid.ToString().Replace("-", "");
			}
		}
		[Description("This is a Guid uniquely identifies the application")]
		public Guid ApplicationGuid
		{
			get
			{
				return DefaultAppConfig.AppGuid;
			}
		}
		[Description("The application configuration profile currently used")]
		public string ConfigurationProfile
		{
			get
			{
				if (_profileType == EnumProfileType.Default)
				{
					return "Factory Settings";
				}
				if (_profileType == EnumProfileType.User)
				{
					WindowsIdentity wi = WindowsIdentity.GetCurrent();
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Profile for user [{0}]", wi.Name);
				}
				return _profileName;
			}
		}
		[Description("The type of the current profile")]
		public EnumProfileType ProfileType
		{
			get
			{
				if (VPLUtil.IsInDesignMode(this) && !_forIDE)
				{
					if (_profileType != EnumProfileType.Default)
					{
						UseFactoryProfile();
					}
				}
				return _profileType;
			}
		}
		[Description("If current profile is a named profile then this property if the name of the profile.")]
		public string ProfileName
		{
			get
			{
				return _profileName;
			}
		}
		[Description("Configuration file path")]
		public string Filename
		{
			get
			{
				getLastProfile();
				if (ProfileType == EnumProfileType.Default)
				{
					return defaultProfilePath;
				}
				if (ProfileType == EnumProfileType.User)
				{
					return userProfilePath;
				}
				if (!string.IsNullOrEmpty(_profileName))
				{
					return namedProfilePath(_profileName);
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		[Description("The configuration object used by this component")]
		public CategoryList Configuration
		{
			get
			{
				if (VPLUtil.IsInDesignMode(this) && !_forIDE)
				{
					_profileType = EnumProfileType.Default;
					return DefaultAppConfig;
				}
				getLastProfile();
				if (_config == null)
				{
					LogOnProfile(string.Empty, string.Empty);
				}
				if (_config == null)
				{
					return DefaultAppConfig;
				}
				return _config;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorCategories), typeof(UITypeEditor))]
		[Description("Definitions for the application configurations. Each configuration data has a name and belongs to a category.")]
		public CategoryList Configurations
		{
			get
			{
				return _categories;
			}
		}
		#endregion
		#region Runtime property access
		[Browsable(false)]
		public ConfigCategory this[string name]
		{
			get
			{
				return Configuration.GetCategoryByName(name); //it will not be null
			}
		}
		[Description("Gets setting by specifying category name and value name. If the category or value do not exist in the current configurations then a null is returned.")]
		public object GetValue(string categoryName, string valueName)
		{
			ConfigCategory cat = this[categoryName];
			return cat.GetPropertyValueByName(valueName);
		}
		[Description("Set setting by specifying category name, value name and the value. If the category does not exist then it will be created. If the value does not exist then it will be created.")]
		public void SetValue(string categoryName, string valueName, object value)
		{
			ConfigCategory cat = this[categoryName];
			cat.SetSetting(valueName, value);
		}
		[Description("Gets the number of values stored in a category specified by categoryName")]
		public int GetValueCount(string categoryName)
		{
			ConfigCategory cat = this[categoryName];
			return cat.ValueCount;
		}
		[Description("Returns a string array containing all property names in a category")]
		public string[] GetValueNames(string categoryName)
		{
			ConfigCategory cat = this[categoryName];
			return cat.GetValueNames();
		}
		[Description("Gets value name by specifying category name and value index. If the category or value do not exist in the current configurations then a null is returned.")]
		public string GetValueNameByIndex(string categoryName, int index)
		{
			ConfigCategory cat = this[categoryName];
			return cat.GetPropertyValueNameByIndex(index);
		}
		[Description("Gets setting by specifying category name and value index. If the category or value do not exist in the current configurations then a null is returned.")]
		public object GetValueByIndex(string categoryName, int index)
		{
			ConfigCategory cat = this[categoryName];
			return cat.GetPropertyValueByIndex(index);
		}
		[Description("Remove all values from a category")]
		public void RemoveAllValues(string categoryName)
		{
			ConfigCategory cat = this[categoryName];
			cat.RemoveAllProperties();
		}
		[Description("Set setting by specifying category name, value name and the value. If the category does not exist then it will be created. If the value does not exist then it will be created.")]
		public void SetValueByIndex(string categoryName, int index, object value)
		{
			ConfigCategory cat = this[categoryName];
			cat.SetPropertyValueByIndex(index, value);
		}
		#endregion

		#region Methods
		const string XML_APP = "App";
		const string XMLATT_Key = "key";
		const string XMLATT_User = "user";
		const string XMLATT_PType = "profileType";
		const string XMLATT_PName = "profileName";
		private void saveLastProfile()
		{
			if (!VPLUtil.IsInDesignMode(this) && !_forIDE)
			{
				XmlDocument doc = new XmlDocument();
				string file = mruFile();
				if (System.IO.File.Exists(file))
				{
					doc.Load(file);
				}
				if (doc.DocumentElement == null)
				{
					XmlNode root = doc.CreateElement("Root");
					doc.AppendChild(root);
				}
				WindowsIdentity id = WindowsIdentity.GetCurrent();
				XmlNode node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}[@{1}='{2}' and @{3}='{4}']",
							XML_APP, XMLATT_Key, this.AppKey, XMLATT_User, id.Name));
				if (node == null)
				{
					node = doc.CreateElement(XML_APP);
					doc.DocumentElement.AppendChild(node);
				}
				XmlAttribute xa = node.Attributes[XMLATT_Key];
				if (xa == null)
				{
					xa = doc.CreateAttribute(XMLATT_Key);
					node.Attributes.Append(xa);
				}
				xa.Value = AppKey;
				//
				xa = node.Attributes[XMLATT_User];
				if (xa == null)
				{
					xa = doc.CreateAttribute(XMLATT_User);
					node.Attributes.Append(xa);
				}
				xa.Value = id.Name;
				//
				xa = node.Attributes[XMLATT_PType];
				if (xa == null)
				{
					xa = doc.CreateAttribute(XMLATT_PType);
					node.Attributes.Append(xa);
				}
				xa.Value = _profileType.ToString();
				//
				if (_profileType == EnumProfileType.Named)
				{
					xa = node.Attributes[XMLATT_PName];
					if (xa == null)
					{
						xa = doc.CreateAttribute(XMLATT_PName);
						node.Attributes.Append(xa);
					}
					xa.Value = _profileName;
				}
				//
				doc.Save(file);
			}
		}
		/// <summary>
		/// this function will be called only once at runtime
		/// it will do nothing at design time
		/// </summary>
		private void getLastProfile()
		{
			if (!VPLUtil.IsInDesignMode(this) && !_forIDE)//not in design mode
			{
				if (!_checkedLastRun)
				{
					_checkedLastRun = true;
					_profileType = EnumProfileType.User;
					string file = mruFile();
					if (System.IO.File.Exists(file))
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(file);
						if (doc.DocumentElement != null)
						{
							WindowsIdentity id = WindowsIdentity.GetCurrent();
							XmlNode node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}[@{1}='{2}' and @{3}='{4}']",
							XML_APP, XMLATT_Key, this.AppKey, XMLATT_User, id.Name));
							if (node == null)
							{
								node = doc.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}[@{1}='{2}']",
								XML_APP, XMLATT_Key, this.AppKey));
							}
							if (node != null)
							{
								XmlAttribute xa = node.Attributes[XMLATT_PType];
								if (xa != null)
								{
									_profileType = (EnumProfileType)Enum.Parse(typeof(EnumProfileType), xa.Value);
									if (_profileType == EnumProfileType.Named)
									{
										string pname = string.Empty;
										xa = node.Attributes[XMLATT_PName];
										if (xa != null)
										{
											pname = xa.Value;
										}
										//try log in without password
										string msg = LogOnProfile(pname, string.Empty);
										if (!string.IsNullOrEmpty(msg))
										{
											//use UI to log in
											_profileName = pname;
											if (!LogOnProfileWithUI(null))
											{
												_profileType = EnumProfileType.User;
											}
										}
									}
								}
							}
						}
					}
					if (_profileType == EnumProfileType.User)
					{
						_profileType = EnumProfileType.Default;
						UseUserProfile();
					}
				}
			}
		}
		/// <summary>
		/// At runtime create {exe name}_{Guid}.config to {user app data} folder.
		/// </summary>
		private bool createUserSpecificProfile()
		{
			string file = userProfilePath;
			if (!System.IO.File.Exists(file))
			{
				System.IO.File.Copy(DefaultAppConfig.FilePath, file);
				return true;
			}
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string userProfilePath
		{
			get
			{
				string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				string file = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}.cfg", System.IO.Path.GetFileNameWithoutExtension(DefaultAppConfig.FilePath), AppKey);
				file = System.IO.Path.Combine(folder, file);
				return file;
			}
		}
		[Browsable(false)]
		internal static string GetPasswordhash()
		{
			if (_password != null)
			{
				IntPtr h = Marshal.SecureStringToBSTR(_password);
				string s = Marshal.PtrToStringBSTR(h);
				Marshal.ZeroFreeBSTR(h);
				return getPasswordHash(s);
			}
			return string.Empty;
		}
		private static string getPasswordWord(string password)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "A08hkr_{0}", password);
		}
		/// <summary>
		/// for checking
		/// </summary>
		/// <param name="password"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		private static bool checkPassword(string password, string hash)
		{
			return verifyMd5Hash(getPasswordWord(password), hash);
		}
		/// <summary>
		/// for saving
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private static string getPasswordHash(string password)
		{
			return getMd5Hash(getPasswordWord(password));
		}
		// Hash an input string and return the hash as
		// a 32 character hexadecimal string.
		private static string getMd5Hash(string input)
		{
			// Create a new instance of the MD5CryptoServiceProvider object.
			MD5 md5Hasher = MD5.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}

		// Verify a hash against a string.
		private static bool verifyMd5Hash(string input, string hash)
		{
			// Hash the input.
			string hashOfInput = getMd5Hash(input);

			// Create a StringComparer an compare the hashes.
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			if (0 == comparer.Compare(hashOfInput, hash))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static string commonAppDataFolder()
		{
			string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			folder = System.IO.Path.Combine(folder, "Limnor Studio\\App Config");
			if (!System.IO.Directory.Exists(folder))
			{
				System.IO.Directory.CreateDirectory(folder);
				bool isModified = false;
				DirectoryInfo myDirectoryInfo = new DirectoryInfo(folder);
				DirectorySecurity myDirectorySecurity = myDirectoryInfo.GetAccessControl();
				AccessRule rule = new FileSystemAccessRule("Users", FileSystemRights.Write | FileSystemRights.ReadAndExecute | FileSystemRights.Modify, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
				myDirectorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out isModified);
				myDirectoryInfo.SetAccessControl(myDirectorySecurity);
			}
			return folder;
		}
		private static string mruFile()
		{
			string folder = commonAppDataFolder();
			string file = System.IO.Path.Combine(folder, "mru_3B8D7AA0AFFC4905A0D2942F58F8C099.config");
			return file;
		}
		private string defaultProfilePath
		{
			get
			{
				if (VPLUtil.IsInDesignMode(this) && !_forIDE)
				{
					if (VPL.VPLUtil.CurrentProject == null)
					{
						throw new ConfigException("Current Project file not set");
					}
					string f = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}.exe.cfg", VPL.VPLUtil.CurrentProject.AssemblyName);
					return Path.Combine(VPL.VPLUtil.CurrentProject.ProjectFolder, f);
				}
				else
				{
					if (!string.IsNullOrEmpty(_exefileFullpath))
					{
						return string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}.cfg", _exefileFullpath);
					}
					string f = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}.cfg", System.Windows.Forms.Application.ExecutablePath);
					if (System.IO.File.Exists(f))
					{
						return f;
					}
					System.IO.FileInfo fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
					f = string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}.cfg", fi.FullName);
					return f;
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string namedProfilePath(string name)
		{
			string folder = commonAppDataFolder();
			string fn;
			string appName;
			if (!string.IsNullOrEmpty(_exefileFullpath))
			{
				appName = Path.GetFileName(_exefileFullpath);
			}
			else
			{
				appName = Path.GetFileNameWithoutExtension(DefaultAppConfig.FilePath);
			}
			fn = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}.cfg",
				 appName, this.AppKey, name);
			string file = System.IO.Path.Combine(folder, fn);
			return file;
		}
		/// <summary>
		/// factory settings changed due to application development
		/// </summary>
		private void verifyConfigurations()
		{
			if (_config != null)
			{
				CategoryList defs = DefaultAppConfig;
				if (defs != null)
				{
					foreach (ConfigCategory cat in defs.Categories)
					{
						_config.VerifyConfigCategory(cat);
					}
				}
			}
		}
		/// <summary>
		/// this function should only be used by TypeEditorCategories at design time
		/// </summary>
		/// <param name="list"></param>
		[Browsable(false)]
		public void SetFactoryConfigurations(CategoryList list)
		{
			list.SetAppGuid(_categories.AppGuid);
			_categories = list;
			_categories.SaveAll(defaultProfilePath);
		}
		[Browsable(false)]
		public string[] GetNames()
		{
			string folder = commonAppDataFolder();
			string fn = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}_*.cfg",
				System.IO.Path.GetFileNameWithoutExtension(DefaultAppConfig.FilePath), this.AppKey);
			string[] fs = System.IO.Directory.GetFiles(folder, fn);
			if (fs != null && fs.Length > 0)
			{
				for (int i = 0; i < fs.Length; i++)
				{
					string s = System.IO.Path.GetFileNameWithoutExtension(fs[i]);
					int pos = s.IndexOf(AppKey, StringComparison.OrdinalIgnoreCase);
					if (pos > 0)
					{
						pos = s.IndexOf("_", pos + 1, StringComparison.Ordinal);
						if (pos > 0)
						{
							s = s.Substring(pos + 1);
						}
					}
					fs[i] = s;
				}
			}
			return fs;
		}
		public string ChangePassword(string name, string password, string newPassword)
		{
			string file = namedProfilePath(name);
			if (!System.IO.File.Exists(file))
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "File not found:{0}", file);
			}
			string msg = LogOnProfile(name, password);
			if (!string.IsNullOrEmpty(msg))
			{
				return msg;
			}
			//change password
			resetPassword();
			if (!string.IsNullOrEmpty(newPassword))
			{
				_password = new SecureString();
				for (int i = 0; i < newPassword.Length; i++)
				{
					_password.AppendChar(newPassword[i]);
				}
				_password.MakeReadOnly();
			}
			//save all values and do encryption if needed
			Configuration.SaveAll(file);
			//
			return string.Empty;
		}
		/// <summary>
		/// Copy Default profile as the new profile. Save hash of the password (or unreversible encryption).
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		[Description("Create a named application configuration profile. Password is optional. If the application configuration profile exists then this method returns false.")]
		public bool CreateNamedProfile(string name, string password)
		{
			string file = namedProfilePath(name);
			if (!System.IO.File.Exists(file))
			{
				resetPassword();
				//copy factory settings
				string ff = null;
				try
				{
					ff = DefaultAppConfig.FilePath;
					if (!string.IsNullOrEmpty(ff) && File.Exists(ff))
					{
						System.IO.File.Copy(ff, file);
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error getting factory configuration from [{0}]. {1}", ff, err.Message), "Config", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				//load factory settings
				if (!string.IsNullOrEmpty(password))
				{
					resetPassword();
					_password = new SecureString();
					for (int i = 0; i < password.Length; i++)
					{
						_password.AppendChar(password[i]);
					}
					_password.MakeReadOnly();
					//
					_profileType = EnumProfileType.Named;
					_profileName = name;
					//save all values and do encryption
					DefaultAppConfig.SaveAll(file);
					saveLastProfile();
				}
				return true;
			}
			return false;
		}
		internal bool NamedProfileExists(string name)
		{
			string file = namedProfilePath(name);
			return System.IO.File.Exists(file);
		}
		[Description("It fires event LoadingConfigurations and thus executes all the actions assigned to this event. Usually the actions assigned to this event are for loading configuration values from the configuration file into the application.")]
		public void ExecuteLoadingConfigurations()
		{
			if (LoadingConfigurations != null)
			{
				LoadingConfigurations(this, EventArgs.Empty);
			}
		}
		[Description("It fires event SavingConfigurations and thus execute all actions assigned to this event. Usually the actions assigned to this event are for saving configuration values to the configuration file. The purpose of executing those actions is to saves all configuration values from the application into the configuration file.")]
		public void ExecuteSavingConfigurations()
		{
			if (SavingConfigurations != null)
			{
				SavingConfigurations(this, EventArgs.Empty);
			}
			Save();
		}
		[Description("It copies the default configurations to the specified file path")]
		public void ExportDefaultProfile(string exportFilePath)
		{
			string src = defaultProfilePath;
			if (File.Exists(src))
			{
				File.Copy(src, exportFilePath, true);
			}
		}
		[Description("It copies the specified file to the default configurations. ExecuteLoadingConfigurations is execute after copying.")]
		public void ImportDefaultProfile(string importFilePath)
		{
			string tgt = defaultProfilePath;
			if (File.Exists(importFilePath))
			{
				File.Copy(importFilePath, tgt, true);
				ExecuteLoadingConfigurations();
			}
		}
		[Description("Show a dialogue box for the user to modify configurable values at runtime")]
		public void SetValues(Form caller)
		{
			DlgCategories dlg = new DlgCategories();
			dlg.SetDataOnly();
			dlg.LoadData(Configuration);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				SetFactoryConfigurations(dlg.Ret);
				Save();
			}
		}
		/// <summary>
		/// Give options for creating user-specific profile if it does not exist, or creating a named-profile.
		/// </summary>
		[Description("Create a named application configuration profile or an application configuration profile for the current user")]
		public void CreateProfile(Form caller)
		{
			DlgCreateProfile dlg = new DlgCreateProfile();
			dlg.SetAppConfig(this);
			if (dlg.ShowDialog(caller) == System.Windows.Forms.DialogResult.OK)
			{
				if (dlg.ForUser)
				{
					if (createUserSpecificProfile())
					{
						MessageBox.Show(caller, "The application configuration profile for the user is created", "Create Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						MessageBox.Show(caller, "The application configuration profile for the user already exists", "Create Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					UseUserProfile();
				}
				else
				{
					if (CreateNamedProfile(dlg.ProfileName, dlg.ProfilePass))
					{
						MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture, "The application configuration profile named [{0}] is created", dlg.ProfileName), "Create Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture, "The application configuration profile named [{0}] already exists", dlg.ProfileName), "Create Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					string msg = LogOnProfile(dlg.ProfileName, dlg.ProfilePass);
					if (!string.IsNullOrEmpty(msg))
					{
						MessageBox.Show(caller, msg, "Create Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}
		[Description("Switch to use the default application configurations. Returns error message on error.")]
		public string UseFactoryProfile()
		{
			if (VPLUtil.IsInDesignMode(this) && !_forIDE)
			{
				_profileType = EnumProfileType.Default;
				_profileName = null;
			}
			else
			{
				string msg = LogOnProfile(string.Empty, string.Empty);
				if (!string.IsNullOrEmpty(msg))
				{
					return msg;
				}
			}
			resetPassword();
			saveLastProfile();
			return null;
		}
		[Description ("Show a dialoue box for specifying a folder to copy the current configuration file.")]
		public void CopyProfile(Form caller)
		{
			DlgCopyProfile dlg = new DlgCopyProfile();
			string file = this.Filename;
			if (string.IsNullOrEmpty(file))
			{
				file = namedProfilePath(string.Empty);
			}
			if (string.IsNullOrEmpty(file))
			{
				MessageBox.Show(caller, "Current configuration not loaded", "Copy profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				dlg.LoadData(file);
				dlg.ShowDialog(caller);
			}
		}
		[Description("Copy current profile to a specified folder named as {exe file name}.cfg. It returns an empty string if the operations succeeds. It returns an error message if an error occurs.")]
		public string CopyProfileToFolder(string folder, bool overwrite)
		{
			if (string.IsNullOrEmpty(folder))
			{
				return "target folder not sprovided";
			}
			if (!Directory.Exists(folder))
			{
				return string.Format(CultureInfo.InvariantCulture, "Target folder does not exist: {0}", folder);
			}
			string file = this.Filename;
			if (string.IsNullOrEmpty(file))
			{
				file = namedProfilePath(string.Empty);
			}
			if (string.IsNullOrEmpty(file))
			{
				return "Current configuration not loaded";
			}
			string exeName = Path.GetFileName(file);
			int pos = exeName.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
			if (pos > 0)
			{
				exeName = exeName.Substring(0, pos + 4);
				string target = string.Format(CultureInfo.InvariantCulture, "{0}.cfg", Path.Combine(folder, exeName));
				if (!overwrite && File.Exists(target))
				{
					return string.Format(CultureInfo.InvariantCulture, "Target file exists: {0}", target);
				}
				try
				{
					File.Copy(file, target, true);
				}
				catch (Exception err)
				{
					return err.Message;
				}
			}
			return string.Empty;
		}
		/// <summary>
		/// switch to user profile
		/// </summary>
		[Description("Switch to use the application configuration profile for the current user. If it does not exist then it will be created.")]
		public void UseUserProfile()
		{
			if (_profileType != EnumProfileType.User)
			{
				_profileType = EnumProfileType.User;
				_profileName = string.Empty;
				string file = userProfilePath;
				if (!System.IO.File.Exists(file))
				{
					System.IO.File.Copy(DefaultAppConfig.FilePath, file);
				}
				resetPassword();
				CategoryList cl = new CategoryList();
				cl.LoadFromFile(file);
				_config = cl;
				verifyConfigurations();
				saveLastProfile();
				if (LoadingConfigurations != null)
				{
					LoadingConfigurations(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		[Description("Switch to a named application configuration profile. The profile must exist. It returns an error message if log on fails.")]
		public string LogOnProfile(string name, string password)
		{
			string file = namedProfilePath(name);
			if (_profileType == EnumProfileType.Named)
			{
				if (_config != null)
				{
					if (string.Compare(file, _config.FilePath, StringComparison.OrdinalIgnoreCase) == 0)
					{
						_profileName = name;
						return string.Empty; //already using it
					}
				}
			}
			if (!System.IO.File.Exists(file))
			{
				if (string.IsNullOrEmpty(password))
				{
					CreateNamedProfile(name, string.Empty);
				}
				else
				{
					//runtime support
					string cfg = string.Format(CultureInfo.InvariantCulture,"{0}.cfg",System.Windows.Forms.Application.ExecutablePath);
					if (File.Exists(cfg))
					{
						//File.Copy(cfg, file);
						file = cfg;
					}
				}
			}
			if (System.IO.File.Exists(file))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				if (doc.DocumentElement == null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid configuration file: {0}", file);
				}
				string pass = XmlUtility.XmlUtil.GetAttribute(doc.DocumentElement, XMLATT_Password);
				if (!string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(pass) || !string.IsNullOrEmpty(password)))
				{
					if (!checkPassword(password, pass))
					{
						return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Password mismatch for configuration file: {0}", file);
					}
				}
				resetPassword();
				if (!string.IsNullOrEmpty(password))
				{
					_password = new SecureString();
					for (int i = 0; i < password.Length; i++)
					{
						_password.AppendChar(password[i]);
					}
					_password.MakeReadOnly();
				}

				CategoryList cl = new CategoryList();
				cl.LoadFromDocument(doc, file);
				_config = cl;
				verifyConfigurations();
				_profileType = EnumProfileType.Named;
				_profileName = name;

				saveLastProfile();
				//
				if (LoadingConfigurations != null)
				{
					LoadingConfigurations(this, EventArgs.Empty);
				}
				//
				return string.Empty;
			}
			else
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Configuration file not found:{0}", file);
			}
		}
		/// <summary>
		/// UI for select user-specific profile or named-profile.
		/// </summary>
		/// <returns></returns>
		[Description("Use a dialogue to select desired application configuration profile. It returns false if the dialogue is canceled.")]
		public bool LogOnProfileWithUI(Form caller)
		{
			DlgLogOnProfile dlg = new DlgLogOnProfile();
			dlg.LoadData(this);
			dlg.SetProfileType(_profileType);
			dlg.SetNameSelection(_profileName);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				if (dlg.Revert)
				{
					string file = string.Empty;
					if (dlg.ProfileType == EnumProfileType.Default)
					{
						file = namedProfilePath(string.Empty);
					}
					else if (dlg.ProfileType == EnumProfileType.User)
					{
						file = userProfilePath;
					}
					else
					{
						file = namedProfilePath(dlg.ProductName);
					}
					if (System.IO.File.Exists(file))
					{
						System.IO.File.Delete(file);
					}
					if (dlg.ProfileType == EnumProfileType.Named)
					{
						CreateNamedProfile(dlg.ProductName, dlg.ProfilePass);
					}
				}
				if (dlg.ProfileType == EnumProfileType.Default)
				{
					string msg = UseFactoryProfile();
					if (!string.IsNullOrEmpty(msg))
					{
						MessageBox.Show(caller, msg, "Switch to factory settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
				else if (dlg.ProfileType == EnumProfileType.User)
				{
					UseUserProfile();
				}
				else
				{
					string msg = LogOnProfile(dlg.ProfileName, dlg.ProfilePass);
					if (string.IsNullOrEmpty(msg))
					{
					}
					else
					{
						MessageBox.Show(caller, msg, "Switch Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
				return true;
			}
			return false;
		}
		public void ChangePasswordByUI(Form caller)
		{
			DlgChangePassword dlg = new DlgChangePassword();
			dlg.SetConfigApp(this);
			dlg.ShowDialog(caller);
		}

		const string DEFSECT = "default";
		[Description("Get application setting by name from the default section. This method is obsolete. Use GetValue or indexers")]
		public string GetSetting(string name)
		{
			object v = null;
			ConfigCategory cat = this.Configuration.GetCategoryByName(DEFSECT);
			if (cat != null)
			{
				v = cat[name];
			}
			if (v == null)
			{
				return string.Empty;
			}
			bool bt;
			return (string)VPLUtil.ConvertObject(v, typeof(string), out bt);
		}
		[Description("Set application setting within the default section. This method is obsolete. Use SetValue or indexers")]
		public void SetSetting(string name, object value)
		{
			ConfigCategory cat = this.Configuration.GetCategoryByName(DEFSECT);
			if (cat == null)
			{
				cat = Configuration.CreateSection(DEFSECT);
			}
			ConfigProperty p = cat.GetConfigProperty(name);
			if (p == null)
			{
				p = cat.CreateConfigProperty(name);
			}
			bool bt;
			p.DefaultData = VPLUtil.ConvertObject(value, typeof(string), out bt);
			if (!bt)
			{
			}
		}
		[Description("Save the settings")]
		public void Save()
		{
			this.Configuration.Save();
		}
		[Description("Set the application to be configered. Only used for making configuration utilities. The method works only the program file and its default configuration file exist.")]
		public bool SetApplicationFullPath(string exePath)
		{
			if (!string.IsNullOrEmpty(exePath))
			{
				if (File.Exists(exePath))
				{
					string cfg = string.Format(CultureInfo.InvariantCulture, "{0}.cfg", exePath);
					if (File.Exists(cfg))
					{
						_exefileFullpath = exePath;
						_config = null;
						_password = null;
						_enckey = null;
						_profileName = null;
						_categories = null;
						_checkedLastRun = false;
						getLastProfile();
						return true;
					}
				}
			}
			return false;
		}
		#endregion

		#region IComponent Members
		[Description("Occurs when this component is disposed")]
		public event EventHandler Disposed;
		ISite _site;
		[NotForProgramming]
		[XmlIgnore]
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[NotForProgramming]
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Configuration.SaveIfChanged();
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			//
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			PropertyDescriptor[] a = new PropertyDescriptor[ps.Count];
			ps.CopyTo(a, 0);
			list.AddRange(a);
			//
			foreach (ConfigCategory cc in Configuration.Categories)
			{
				PropertyDescriptorCategory pc = new PropertyDescriptorCategory(cc.CategoryName, attributes, new CategoryWrapper(cc, this));
				list.Add(pc);
			}
			ps = new PropertyDescriptorCollection(list.ToArray());
			//
			return ps;
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region PropertyDescriptorCategory
		/// <summary>
		/// represent a category
		/// </summary>
		class PropertyDescriptorCategory : PropertyDescriptor
		{
			private CategoryWrapper _category;
			/// <summary>
			/// 
			/// </summary>
			/// <param name="name">it is the category name</param>
			/// <param name="attrs"></param>
			public PropertyDescriptorCategory(string name, Attribute[] attrs, CategoryWrapper category)
				: base(name, attrs)
			{
				_category = category;
			}
			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(ApplicationConfiguration); }
			}

			public override object GetValue(object component)
			{
				return _category;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(CategoryWrapper);
				}
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{

			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region IIndexerAsProperty Members
		[Browsable(false)]
		public bool IsIndexer(string name)
		{
			string[] ss = name.Split('.');
			foreach (ConfigCategory c in Configuration.Categories)
			{
				if (string.Compare(ss[0], c.CategoryName, StringComparison.Ordinal) == 0)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public CodeExpression CreateCodeExpression(CodeExpression target, string name)
		{
			string[] ss = name.Split('.');
			if (ss.Length == 2)
			{
				return new CodeIndexerExpression(new CodeIndexerExpression(target, new CodePrimitiveExpression(ss[0])),
					new CodePrimitiveExpression(ss[1]));
			}
			else
			{
				if (ss.Length == 1)
				{
					if (string.CompareOrdinal(name, "ProfileName") == 0)
						return null;
					if (string.CompareOrdinal(name, "ProfileType") == 0)
						return null;
					if (string.CompareOrdinal(name, "AppKey") == 0)
						return null;
					if (string.CompareOrdinal(name, "ApplicationGuid") == 0)
						return null;
					if (string.CompareOrdinal(name, "Configuration") == 0)
						return null;
					if (string.CompareOrdinal(name, "Configurations") == 0)
						return null;
					if (string.CompareOrdinal(name, "ConfigurationProfile") == 0)
						return null;
					if (string.CompareOrdinal(name, "Filename") == 0)
						return null;
					return new CodeIndexerExpression(target, new CodePrimitiveExpression(name));
				}
			}
			throw new ConfigException("Invalid property name for CreateCodeExpression:[{0}]", name);
		}
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(string propOwner, string MemberName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", propOwner, MemberName);
		}
		public string GetPhpScriptReferenceCode(string propOwner, string MemberName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", propOwner, MemberName);
		}
		[Browsable(false)]
		public Type IndexerDataType(string name)
		{
			string[] ss = name.Split('.');
			if (ss.Length == 2)
			{
				ConfigCategory cat = Configuration.GetCategoryByName(ss[0]);
				if (cat != null)
				{
					ConfigProperty p = cat.GetConfigProperty(ss[1]);
					if (p != null)
					{
						return p.DataType;
					}
					else
					{
						throw new ConfigException("Invalid property name for IndexerDataType:[{0}]. Property [{1}] not found", name, ss[1]);
					}
				}
				else
				{
					throw new ConfigException("Invalid property name for IndexerDataType:[{0}]. Category [{1}] not found", name, ss[0]);
				}
			}
			return null;
		}
		#endregion

		#region ICustomPropertyCollection Members
		[Browsable(false)]
		public PropertyDescriptorCollection GetCustomPropertyCollection()
		{
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (ConfigCategory cat in Configuration.Categories)
			{
				cat.GetCustomProperties(list, this);
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		[Browsable(false)]
		public Type GetCustomPropertyType(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				string[] ss = name.Split('.');
				if (ss.Length == 2)
				{
					ConfigCategory cat = this[ss[0]];
					if (cat != null)
					{
						ConfigProperty p = cat.GetConfigProperty(ss[1]);
						if (p != null)
						{
							return p.DataType;
						}
						else
						{
							throw new ConfigException("Invalid ptoperty name:[{0}]. Property [{1}] not found", name, ss[1]);
						}
					}
					else
					{
						throw new ConfigException("Invalid ptoperty name:[{0}]. Category [{1}] not found", name, ss[0]);
					}
				}
			}
			throw new ConfigException("Invalid ptoperty name:[{0}]", name);
		}
		#endregion
	}
}
