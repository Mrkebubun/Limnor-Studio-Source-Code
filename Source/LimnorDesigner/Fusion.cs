using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GacUtilW
{
	public class ComUtil
	{
		public static bool SUCCEEDED(int errorCode)
		{
			return errorCode >= 0;
		}

		public static bool FAILED(int errorCode)
		{
			return errorCode < 0;
		}

		public static void ComCheck(int errorCode)
		{
			if(FAILED(errorCode))
			{
				Marshal.ThrowExceptionForHR(errorCode);
			}
		}  

		public static readonly int ERROR_SUCCESS  = 0;

		public static readonly int S_OK           = 0;
		public static readonly int S_FALSE        = 1;

		public static readonly int ERROR_INSUFFICIENT_BUFFER = -2147024774; // 0x8007007A
	}

	/// <summary>
	/// Fusion wrapper classes
	/// </summary>
	public class Fusion 
	{
		protected Fusion()
		{
		}

		[Flags] public enum ASM_CACHE_FLAGS
		{
			ASM_CACHE_ZAP            = 0x1,
			ASM_CACHE_GAC            = 0x2,
			ASM_CACHE_DOWNLOAD       = 0x4,
			ASM_CACHE_ROOT           = 0x8
		} 
  
		[Flags] public enum CREATE_ASM_NAME_OBJ_FLAGS
		{
			CANOF_PARSE_DISPLAY_NAME = 0x1,
			CANOF_SET_DEFAULT_VALUES = 0x2
		} 

		public enum ASM_NAME
		{
			ASM_NAME_PUBLIC_KEY,
			ASM_NAME_PUBLIC_KEY_TOKEN,
			ASM_NAME_HASH_VALUE,
			ASM_NAME_NAME,
			ASM_NAME_MAJOR_VERSION,
			ASM_NAME_MINOR_VERSION,
			ASM_NAME_BUILD_NUMBER,
			ASM_NAME_REVISION_NUMBER,
			ASM_NAME_CULTURE,
			ASM_NAME_PROCESSOR_ID_ARRAY,
			ASM_NAME_OSINFO_ARRAY,
			ASM_NAME_HASH_ALGID,
			ASM_NAME_ALIAS,
			ASM_NAME_CODEBASE_URL,
			ASM_NAME_CODEBASE_LASTMOD,
			ASM_NAME_NULL_PUBLIC_KEY,
			ASM_NAME_NULL_PUBLIC_KEY_TOKEN,
			ASM_NAME_CUSTOM,
			ASM_NAME_NULL_CUSTOM,                
			ASM_NAME_MVID,
			ASM_NAME_MAX_PARAMS
		} 			

		[Flags] public enum ASM_DISPLAY_FLAGS
		{
			ASM_DISPLAYF_VERSION                = 0x1,  // Includes the version number as part of the display name.
			ASM_DISPLAYF_CULTURE                = 0x2,  // Includes the culture.
			ASM_DISPLAYF_PUBLIC_KEY_TOKEN       = 0x4,  // Includes the public key token.
			ASM_DISPLAYF_PUBLIC_KEY             = 0x8,  // Includes the public key.
			ASM_DISPLAYF_CUSTOM                 = 0x10, // Includes the custom part of the assembly name.
			ASM_DISPLAYF_PROCESSORARCHITECTURE  = 0x20, // Includes the processor architecture.
			ASM_DISPLAYF_LANGUAGEID             = 0x40, // Includes the language ID. 
			ASM_DISPLAYF_DEFAULT                = ASM_DISPLAYF_VERSION | ASM_DISPLAYF_CULTURE | ASM_DISPLAYF_PUBLIC_KEY_TOKEN
		} 

		[Flags] public enum ASM_CMP_FLAGS
		{
			ASM_CMPF_NAME             = 0x1,
			ASM_CMPF_MAJOR_VERSION    = 0x2,
			ASM_CMPF_MINOR_VERSION    = 0x4,
			ASM_CMPF_BUILD_NUMBER     = 0x8,
			ASM_CMPF_REVISION_NUMBER  = 0x10,
			ASM_CMPF_PUBLIC_KEY_TOKEN = 0x20,
			ASM_CMPF_CULTURE          = 0x40,
			ASM_CMPF_CUSTOM           = 0x80,
			ASM_CMPF_ALL              = ASM_CMPF_NAME | ASM_CMPF_MAJOR_VERSION | ASM_CMPF_MINOR_VERSION |
				ASM_CMPF_REVISION_NUMBER | ASM_CMPF_BUILD_NUMBER |
				ASM_CMPF_PUBLIC_KEY_TOKEN | ASM_CMPF_CULTURE | ASM_CMPF_CUSTOM,

			// For strongly named assemblies, ASM_CMPF_DEFAULT==ASM_CMPF_ALL.
			// For simply named assemblies, this is also true, however, when
			// performing IAssemblyName::IsEqual, the build number/revision
			// number will be removed from the comparision.
			ASM_CMPF_DEFAULT          = 0x100
		} 

		public enum ASM_INSTALL_FLAG
		{
			IASSEMBLYCACHE_INSTALL_FLAG_REFRESH       = 1,
			IASSEMBLYCACHE_INSTALL_FLAG_FORCE_REFRESH = 2
		}

		public enum ASM_UNINSTALL_DISPOSITION
		{
			IASSEMBLYCACHE_UNINSTALL_DISPOSITION_UNINSTALLED            = 1,
			IASSEMBLYCACHE_UNINSTALL_DISPOSITION_STILL_IN_USE           = 2,
			IASSEMBLYCACHE_UNINSTALL_DISPOSITION_ALREADY_UNINSTALLED    = 3,
			IASSEMBLYCACHE_UNINSTALL_DISPOSITION_DELETE_PENDING         = 4,
			IASSEMBLYCACHE_UNINSTALL_DISPOSITION_HAS_INSTALL_REFERENCES = 5,
			IASSEMBLYCACHE_UNINSTALL_DISPOSITION_REFERENCE_NOT_FOUND    = 6
		}

		public enum QUERYASMINFO_FLAG
		{
			QUERYASMINFO_FLAG_VALIDATE  = 1,
			QUERYASMINFO_FLAG_GETSIZE   = 2
		}    

		public enum ASSEMBLYINFO_FLAG
		{
			ASSEMBLYINFO_FLAG_INSTALLED       = 1,
			ASSEMBLYINFO_FLAG_PAYLOADRESIDENT = 2
		}

		// The assembly is referenced by an application that appears in Add/Remove Programs. 
		// The szIdentifier field is the token that is used to register the application with Add/Remove programs.
		public static readonly Guid FUSION_REFCOUNT_UNINSTALL_SUBKEY_GUID = new Guid("{8cedc215-ac4b-488b-93c0-a50a49cb2fb8}");
		// The assembly is referenced by an application that is represented by a file in the file system. 
		// The szIdentifier field is the path to this file.
		public static readonly Guid FUSION_REFCOUNT_FILEPATH_GUID         = new Guid("{b02f9d65-fb77-4f7a-afa5-b391309f11c9}");
		// The assembly is referenced by an application that is only represented by an opaque string. 
		// The szIdentifier is this opaque string. The GAC does not perform existence checking for opaque references when you remove this.
		public static readonly Guid FUSION_REFCOUNT_OPAQUE_STRING_GUID    = new Guid("{2ec93463-b0c3-45e1-8364-327e96aea856}");
		// The assembly is referenced by an application that has been installed by using Windows Installer. 
		// The szIdentifier field is set to MSI, and szNonCannonicalData is set to Windows Installer. 
		// This GUID cannot be used for installing into GAC.
		public static readonly Guid FUSION_REFCOUNT_MSI_GUID              = new Guid("{25df0fc1-7f97-4070-add7-4b13bbfd7cb8}"); 

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			public struct FUSION_INSTALL_REFERENCE
		{
			public uint cbSize;                 // The size of the structure in bytes.
			public uint dwFlags;                // Reserved, must be zero.
			public Guid guidScheme;             // contains one of the pre-defined guids. The entity that adds the reference.
			public string szIdentifier;         // unique identifier for app installing this  assembly.
			public string szNonCannonicalData;  // data is description; relevent to the guid above. A string that is only understood by the entity that adds the reference. The GAC only stores this string.
		} 

		[StructLayout(LayoutKind.Sequential)]
			public struct ASSEMBLY_INFO
		{
			public uint cbAssemblyInfo; // size of this structure for future expansion
			public ASSEMBLYINFO_FLAG dwAssemblyFlags;
			public ulong uliAssemblySizeInKB;
			public IntPtr pszCurrentAssemblyPathBuf;
			public uint   cchBuf; // size of path buf.
		} 

		[ComImport, Guid("582dac66-e678-449f-aba6-6faaec8a9394"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IInstallReferenceItem
		{
			[PreserveSig]
			int GetReference(
				out IntPtr ppRefData,
				uint dwFlags, IntPtr pvReserved);
		}

		[ComImport, Guid("56b1a988-7c0c-4aa2-8639-c3eb5a90226f"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IInstallReferenceEnum
		{
			[PreserveSig]
			int GetNextInstallReferenceItem(
				out IInstallReferenceItem ppRefItem,
				uint dwFlags, IntPtr pvReserved);
		}

		[ComImport, Guid("9e3aaeb4-d1cd-11d2-bab9-00c04f8eceae"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IAssemblyCacheItem
		{
			[PreserveSig]
			int CreateStream(
				uint dwFlags,                                           // For general API flags
				[MarshalAs(UnmanagedType.LPWStr)] string pszStreamName, // Name of the stream to be passed in
				uint dwFormat,                                          // format of the file to be streamed in.
				uint dwFormatFlags,                                     // format-specific flags
				[MarshalAs(UnmanagedType.Interface)] out UCOMIStream ppIStream,
				[MarshalAs(UnmanagedType.U8)] ref ulong puliMaxSize);// Max size of the Stream.                                                           

			[PreserveSig]
			int Commit(
				uint dwFlags, // For general API flags like IASSEMBLYCACHEITEM _COMMIT_FLAG_REFRESH
				out uint pulDisposition);

			[PreserveSig]
			int AbortItem(); // If you have created IAssemblyCacheItem and don't plan to use it, its good idea to call AbortItem before releasing it.
		}

		[ComImport, Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IAssemblyCache
		{    
			[PreserveSig]
			int UninstallAssembly(
				uint dwFlags, 
				[MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName, 
				IntPtr pRefData, 
				out ASM_UNINSTALL_DISPOSITION pulDisposition); 

			[PreserveSig]
			int QueryAssemblyInfo(
				QUERYASMINFO_FLAG dwFlags, 
				[MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName, 
				ref ASSEMBLY_INFO pAsmInfo);

			[PreserveSig]
			int CreateAssemblyCacheItem(
				uint dwFlags, 
				IntPtr pvReserved, 
				[MarshalAs(UnmanagedType.LPWStr)] out IAssemblyCacheItem ppAsmItem, 
				string pszAssemblyName); 

			[PreserveSig]
			int CreateAssemblyScavenger(out object ppAsmScavenger);

			[PreserveSig]
			int InstallAssembly(
				ASM_INSTALL_FLAG dwFlags, 
				[MarshalAs(UnmanagedType.LPWStr)] string pszManifestFilePath, 
				IntPtr pRefData);
		}

		[ComImport, Guid("cd193bc0-b4bc-11d2-9833-00c04fc31d2e"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IAssemblyName
		{
			[PreserveSig]
			int SetProperty(uint PropertyId, IntPtr pvProperty, uint cbProperty); 

			[PreserveSig]
			int GetProperty(ASM_NAME PropertyId, IntPtr pvProperty, ref uint pcbProperty); 

			[PreserveSig]
			int Finalize(); 

			[PreserveSig]
			int GetDisplayName(
				[MarshalAs(UnmanagedType.LPWStr)] StringBuilder szDisplayName, // A pointer to a buffer that is to contain the display name. The display name is returned in Unicode.         
				ref uint pccDisplayName,            // The size of the buffer in characters (on input). The length of the returned display name (on return).
				ASM_DISPLAY_FLAGS dwDisplayFlags);  // One or more of the bits defined in the ASM_DISPLAY_FLAGS enumeration

			[PreserveSig]
			int BindToObject(
				object refIID, 
				object pAsmBindSink, 
				IApplicationContext pApplicationContext, 
				[MarshalAs(UnmanagedType.LPWStr)] string szCodeBase, 
				long llFlags, 
				int pvReserved, 
				uint cbReserved, 
				out int ppv); 

			[PreserveSig]
			int GetName(
				ref uint lpcwBuffer,                                      // Size of the pwszName buffer (on input). Length of the name (on return).
				[MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzName); // Pointer to the buffer that is to contain the name part of the assembly name.      

			[PreserveSig]
			int GetVersion(
				out uint pdwVersionHi,      // Pointer to a DWORD that contains the upper 32 bits of the version number.
				out uint pdwVersionLow);    // Pointer to a DWORD that contain the lower 32 bits of the version number.

			[PreserveSig]
			int IsEqual(
				IAssemblyName pName,        // The assembly name to compare to.
				ASM_CMP_FLAGS dwCmpFlags);  // Indicates which part of the assembly name to use in the comparison

			[PreserveSig]
			int Clone(out IAssemblyName pName);            
		}

		[ComImport, Guid("7c23ff90-33af-11d3-95da-00a024a85b51"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IApplicationContext
		{
			[PreserveSig]
			int SetContextNameObject(IAssemblyName pName); 

			[PreserveSig]
			int GetContextNameObject(out IAssemblyName ppName);

			[PreserveSig]
			int Set(
				[MarshalAs(UnmanagedType.LPWStr)] string szName, 
				IntPtr pvValue, 
				uint cbValue, 
				uint dwFlags); 

			[PreserveSig]
			int Get(
				[MarshalAs(UnmanagedType.LPWStr)] string szName, 
				IntPtr pvValue, 
				ref uint pcbValue, 
				uint dwFlags); 

			[PreserveSig]
			int GetDynamicDirectory(
				[MarshalAs(UnmanagedType.LPWStr)] StringBuilder wzDynamicDir, 
				ref uint pdwSize); 

			[PreserveSig]
			int GetAppCacheDirectory(
				[MarshalAs(UnmanagedType.LPWStr)] StringBuilder wzAppCacheDir, 
				ref uint pdwSize); 

			[PreserveSig]
			int RegisterKnownAssembly(
				IAssemblyName pName,        
				[MarshalAs(UnmanagedType.LPWStr)] string pwzAsmURL,
				out IntPtr ppAsmOut);//out IAssembly ppAsmOut);

			[PreserveSig]
			int PrefetchAppConfigFile();

			[PreserveSig]
			int SxsActivateContext(out IntPtr lpCookie);

			[PreserveSig]
			int SxsDeactivateContext(IntPtr lpCookie);
		}

		[ComImport, Guid("21b8916c-f28e-11d2-a473-00c04f8ef448"),
			InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]  
			public interface IAssemblyEnum
		{
			[PreserveSig]
			int GetNextAssembly(
				out IApplicationContext ppAppCtx, // Must be null.
				out IAssemblyName ppName,         // Pointer to a memory location that is to receive the interface pointer to the assembly name of the next assembly that is enumerated.
				uint dwFlags);                    // Must be zero

			[PreserveSig]
			int Reset();

			[PreserveSig]
			int Clone(out IAssemblyEnum ppEnum);
		}

		public const string DLL_NAME = "fusion.dll";

		[DllImportAttribute(DLL_NAME)]
		public static extern int CreateInstallReferenceEnum(
			out IInstallReferenceEnum ppRefEnum, 
			IAssemblyName pName, 
			uint dwFlags, 
			IntPtr pvReserved);
  
		[DllImportAttribute(DLL_NAME)]
		public static extern int CreateAssemblyEnum(
			out IAssemblyEnum ppEnum,     // Pointer to a memory location that contains the IAssemblyEnum pointer
			IApplicationContext pAppCtx,  // Must be null. ???
			IAssemblyName pName,          // An assembly name that is used to filter the enumeration. Can be null to enumerate all assemblies in the GAC.
			ASM_CACHE_FLAGS dwFlags,      // Exactly one bit from the ASM_CACHE_FLAGS enumeration.
			int pvReserved);              // Must be NULL

		[DllImportAttribute(DLL_NAME, CharSet=CharSet.Unicode)]
		public static extern int CreateAssemblyNameObject(
			out IAssemblyName ppEnum,           // Pointer to a memory location that receives the IAssemblyName pointer that is created.
			string szAssemblyName,              // A string representation of the assembly name or of a full assembly reference that is determined by dwFlags. The string representation can be null.
			CREATE_ASM_NAME_OBJ_FLAGS dwFlags,  // Zero or more of the bits that are defined in the CREATE_ASM_NAME_OBJ_FLAGS enumeration.
			int pvReserved);                    // Must be null. 

		[DllImportAttribute(DLL_NAME)]
		public static extern int CreateAssemblyCache(
			out IAssemblyCache ppAsmCache,      
			uint dwReserved);

		[DllImportAttribute(DLL_NAME)]
		public static extern int GetCachePath(
			ASM_CACHE_FLAGS dwCacheFlags, 
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzCachePath,
			ref uint pcchPath);
	}

	public enum AssemblyProperty
	{
		Name                = Fusion.ASM_NAME.ASM_NAME_NAME,
		Culture             = Fusion.ASM_NAME.ASM_NAME_CULTURE,
		PublicKey           = Fusion.ASM_NAME.ASM_NAME_PUBLIC_KEY,
		PublicKeyToken      = Fusion.ASM_NAME.ASM_NAME_PUBLIC_KEY_TOKEN,
		NullPublicKey       = Fusion.ASM_NAME.ASM_NAME_NULL_PUBLIC_KEY,
		NullPublicKeyToken  = Fusion.ASM_NAME.ASM_NAME_NULL_PUBLIC_KEY_TOKEN,
		HashValue           = Fusion.ASM_NAME.ASM_NAME_HASH_VALUE,
		HashAlgID           = Fusion.ASM_NAME.ASM_NAME_HASH_ALGID,
		MajorVersion        = Fusion.ASM_NAME.ASM_NAME_MAJOR_VERSION,
		MinorVersion        = Fusion.ASM_NAME.ASM_NAME_MINOR_VERSION,
		BuildNumber         = Fusion.ASM_NAME.ASM_NAME_BUILD_NUMBER,
		RevisionNumber      = Fusion.ASM_NAME.ASM_NAME_REVISION_NUMBER,
		ProcessorID         = Fusion.ASM_NAME.ASM_NAME_PROCESSOR_ID_ARRAY,
		OSInfo              = Fusion.ASM_NAME.ASM_NAME_OSINFO_ARRAY,
		Alias               = Fusion.ASM_NAME.ASM_NAME_ALIAS,
		CodeBaseUrl         = Fusion.ASM_NAME.ASM_NAME_CODEBASE_URL,
		LastModified        = Fusion.ASM_NAME.ASM_NAME_CODEBASE_LASTMOD,
		Custom              = Fusion.ASM_NAME.ASM_NAME_CUSTOM,
		NullCustom          = Fusion.ASM_NAME.ASM_NAME_NULL_CUSTOM,                
		MVID                = Fusion.ASM_NAME.ASM_NAME_MVID
	}

	public enum UninstallDisposition
	{
		Uninstalled           = Fusion.ASM_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_UNINSTALLED,
		StillInUse            = Fusion.ASM_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_STILL_IN_USE,
		AlreadyUninstalled    = Fusion.ASM_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_ALREADY_UNINSTALLED,
		DeletePending         = Fusion.ASM_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_DELETE_PENDING,
		HasInstallReferences  = Fusion.ASM_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_HAS_INSTALL_REFERENCES,
		ReferenceNotFound     = Fusion.ASM_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_REFERENCE_NOT_FOUND
	}

	public class FusionUtil : Fusion, IEnumerable 
	{
		public class Data
		{
			private byte[] _data;

			public Data(byte[] data)
			{
				_data = data;
			}      
			public byte[] GetData()
			{
				return _data;
			}
			public override string ToString()
			{       
				return IntToHex(_data);
			}

			public static string IntToHex(byte[] data)
			{
				StringBuilder sb = new StringBuilder(data.Length * 2);

				for(int i=0; i<data.Length; i++)
					sb.Append(data[i].ToString("X2"));

				return sb.ToString();
			}
		}

		public enum InstallReferenceScheme
		{
			MSI,
			Uninstall,
			FilePath,
			UserData,
			Unknown
		}

		public class InstallReference
		{
			private readonly Guid _scheme;
			private readonly string _identifier, _data;

			public InstallReference(Fusion.FUSION_INSTALL_REFERENCE refItem)
			{
				_scheme = refItem.guidScheme;
				_identifier = refItem.szIdentifier;
				_data = refItem.szNonCannonicalData;
			}

			public InstallReference(InstallReferenceScheme scheme, string id, string data)
			{
				switch(scheme)
				{
					case InstallReferenceScheme.MSI:       { _scheme = Fusion.FUSION_REFCOUNT_MSI_GUID;              break; }
					case InstallReferenceScheme.Uninstall: { _scheme = Fusion.FUSION_REFCOUNT_UNINSTALL_SUBKEY_GUID; break; }
					case InstallReferenceScheme.FilePath:  { _scheme = Fusion.FUSION_REFCOUNT_FILEPATH_GUID;         break; }
					case InstallReferenceScheme.UserData:  { _scheme = Fusion.FUSION_REFCOUNT_OPAQUE_STRING_GUID;    break; }
					default: { _scheme = Guid.Empty; break; }
				}

				_identifier = id;
				_data = data;
			}

			public Guid SchemeGuid
			{
				get
				{
					return _scheme;
				}
			}

			public InstallReferenceScheme Scheme
			{
				get
				{
					if(_scheme == Fusion.FUSION_REFCOUNT_MSI_GUID)
						return InstallReferenceScheme.MSI;
					else if(_scheme == Fusion.FUSION_REFCOUNT_UNINSTALL_SUBKEY_GUID)
						return InstallReferenceScheme.Uninstall;
					else if(_scheme == Fusion.FUSION_REFCOUNT_FILEPATH_GUID)
						return InstallReferenceScheme.FilePath;
					else if(_scheme == Fusion.FUSION_REFCOUNT_OPAQUE_STRING_GUID)
						return InstallReferenceScheme.UserData;
					else
						return InstallReferenceScheme.Unknown;
				}
			}

			public string Identifier
			{
				get
				{
					return _identifier;
				}
			}

			public string UserData
			{
				get
				{
					return _data;
				}
			}

			static public implicit operator Fusion.FUSION_INSTALL_REFERENCE(InstallReference obj)
			{
				Fusion.FUSION_INSTALL_REFERENCE refData = new Fusion.FUSION_INSTALL_REFERENCE();

				refData.cbSize = (uint)Marshal.SizeOf(refData);
				refData.dwFlags = 0;
				refData.guidScheme = obj._scheme;
				refData.szIdentifier = obj._identifier;
				refData.szNonCannonicalData = obj._data;

				return refData;
			}
		}

		public class AssemblyName
		{
			private IAssemblyName _name;
			private IApplicationContext _ctxt;

			public AssemblyName(IAssemblyName name, IApplicationContext ctxt)
			{
				this._name = name;
				this._ctxt = ctxt;

				Properties = new PropertyCollection(this);
				References = new ReferenceCollection(this);
			}

			protected string getDisplayName(ASM_DISPLAY_FLAGS flags)
			{                
				uint len = 0;

				int hr = _name.GetDisplayName(null, ref len, flags);

				if(hr == ComUtil.ERROR_INSUFFICIENT_BUFFER && len > 0)
				{
					StringBuilder displayName = new StringBuilder((int)len);

					ComUtil.ComCheck(_name.GetDisplayName(displayName, ref len, flags));

					return displayName.ToString();
				}
				else
				{
					return "";
				}
			}

			protected string getProperty(ASM_NAME id)
			{
				uint len = 0;

				int hr = _name.GetProperty(id, IntPtr.Zero, ref len);
        
				if(hr == ComUtil.ERROR_INSUFFICIENT_BUFFER && len > 0)
				{
					IntPtr buf = Marshal.AllocCoTaskMem((int)len);

					ComUtil.ComCheck(_name.GetProperty(id, buf, ref len));

					string str = Marshal.PtrToStringUni(buf);

					Marshal.FreeCoTaskMem(buf);

					return str;
				}
				else
				{
					return "";
				}
			}

			protected byte[] getPropertyData(ASM_NAME id)
			{
				uint len = 0;

				int hr = _name.GetProperty(id, IntPtr.Zero, ref len);

				if(hr == ComUtil.ERROR_INSUFFICIENT_BUFFER && len > 0)
				{
					IntPtr buf = Marshal.AllocCoTaskMem((int)len);

					ComUtil.ComCheck(_name.GetProperty(id, buf, ref len));

					byte[] data = new byte[len];

					Marshal.Copy(buf, data, 0, (int)len);

					Marshal.FreeCoTaskMem(buf);

					return data;
				}
				else
					return new byte[0];          
			}

			public string Name
			{
				get
				{
					return getProperty(ASM_NAME.ASM_NAME_NAME);
				}
			}

			public string DisplayName
			{
				get
				{
					return getDisplayName(ASM_DISPLAY_FLAGS.ASM_DISPLAYF_DEFAULT);
				}
			}

			public Version Version
			{
				get
				{
					uint hi, low;
					ComUtil.ComCheck(_name.GetVersion(out hi, out low));
					return new System.Version((int)(hi >> 16), (int)(hi & 0xFFFF), (int)(low >> 16), (int)(low & 0xFFFF));
				}        
			}

			public string Culture
			{
				get
				{
					return getProperty(ASM_NAME.ASM_NAME_CULTURE);          
				}
			}

			public Data PublicKey
			{
				get
				{
					return new Data(getPropertyData(ASM_NAME.ASM_NAME_PUBLIC_KEY));          
				}
			}

			public class PropertyCollection
			{
				private readonly AssemblyName _name;

				internal PropertyCollection(AssemblyName name)
				{
					_name = name;
				}

				public Data this[AssemblyProperty id]
				{
					get
					{
						if((int)id >= (int)Fusion.ASM_NAME.ASM_NAME_MAX_PARAMS)
							throw new IndexOutOfRangeException();

						return new Data(_name.getPropertyData((ASM_NAME)id));
					}
				}

				public int Count 
				{
					get
					{
						return (int)ASM_NAME.ASM_NAME_MAX_PARAMS;
					}
				}
			}

			public readonly PropertyCollection Properties;

			public class ReferenceCollection : IEnumerable
			{
				private readonly AssemblyName _name;

				private ArrayList _refs;

				internal ReferenceCollection(AssemblyName name)
				{
					_name = name;

					Refresh();
				}

				public void Refresh()
				{
					if(_refs == null) 
						_refs = new ArrayList();
					else
						_refs.Clear();

					Fusion.IInstallReferenceEnum refEnum;

					ComUtil.ComCheck(Fusion.CreateInstallReferenceEnum(out refEnum, _name._name, 0, IntPtr.Zero));

					Fusion.IInstallReferenceItem item;

					while(ComUtil.SUCCEEDED(refEnum.GetNextInstallReferenceItem(out item, 0, IntPtr.Zero)) && item != null)
					{
						IntPtr pRef;

						ComUtil.ComCheck(item.GetReference(out pRef, 0, IntPtr.Zero));            
            
						Fusion.FUSION_INSTALL_REFERENCE objRef = (Fusion.FUSION_INSTALL_REFERENCE)
							Marshal.PtrToStructure(pRef, typeof(Fusion.FUSION_INSTALL_REFERENCE));

						_refs.Add(new InstallReference(objRef));
					}
				}

				public ArrayList References
				{
					get
					{
						if(_refs == null)
						{   
							Refresh();
						}
						return _refs;
					}
				}

				public InstallReference this[int idx]
				{
					get
					{
						return (InstallReference)References[idx];    
					}          
				}

				public int Count 
				{
					get
					{
						return References.Count;
					}
				}

				static public implicit operator FusionUtil.InstallReference[](ReferenceCollection obj)
				{
					return (FusionUtil.InstallReference[])obj.References.ToArray(typeof(FusionUtil.InstallReference));
				}

				#region IEnumerable Members

				public IEnumerator GetEnumerator()
				{
					return References.GetEnumerator();
				}

				#endregion
			}

			public readonly ReferenceCollection References;
		}

    
		private ArrayList _asms = new ArrayList(256);
		private ASM_CACHE_FLAGS _cache = ASM_CACHE_FLAGS.ASM_CACHE_GAC;

		public int Count { get{ return _asms.Count; } }
		public ASM_CACHE_FLAGS Cache { get { return _cache; } }

		#region implement IEnumerator

		public class AssemblyNameEnumerator : IEnumerator 
		{
			FusionUtil _fusion;
			int _idx;

			public AssemblyNameEnumerator(FusionUtil fusion)
			{
				this._fusion = fusion;

				Reset();
			}
    
			#region IEnumerator Members

			public void Reset()
			{
				_idx = -1;
			}

			public object Current
			{
				get
				{          
					return _fusion._asms[_idx];
				}
			}

			public bool MoveNext()
			{        
				return ++_idx < _fusion._asms.Count;
			}

			#endregion
		}


		public IEnumerator GetEnumerator()
		{
			return new AssemblyNameEnumerator(this);
		}

		#endregion

		public static string GetCachePath(ASM_CACHE_FLAGS cache)
		{
			uint len = 0;

			int hr = Fusion.GetCachePath(cache, null, ref len);

			if(hr == ComUtil.ERROR_INSUFFICIENT_BUFFER && len > 0)
			{
				StringBuilder sb = new StringBuilder((int)len);

				ComUtil.ComCheck(Fusion.GetCachePath(cache, sb, ref len));

				return sb.ToString();
			}
			else
			{
				return "";
			}
		}

		public void Refresh(ASM_CACHE_FLAGS cache)
		{
			IAssemblyEnum asmEnum;

			this._cache = cache;

			ComUtil.ComCheck(CreateAssemblyEnum(out asmEnum, null, null, cache, 0));

			IApplicationContext appCtxt;
			IAssemblyName asmName; 

			_asms.Clear();

			while(ComUtil.SUCCEEDED(asmEnum.GetNextAssembly(out appCtxt, out asmName, 0)) && asmName != null)
			{
				_asms.Add(new AssemblyName(asmName, appCtxt));
			}
		}

		public void Refresh()
		{
			Refresh(ASM_CACHE_FLAGS.ASM_CACHE_GAC);
		}

		public void Install(string fileName, IntPtr pRef)
		{
			IAssemblyCache cache;

			ComUtil.ComCheck(Fusion.CreateAssemblyCache(out cache, 0));

			ComUtil.ComCheck(cache.InstallAssembly(Fusion.ASM_INSTALL_FLAG.IASSEMBLYCACHE_INSTALL_FLAG_REFRESH, fileName, pRef));
		}

		public void Install(string fileName)
		{
			Install(fileName, IntPtr.Zero);
		}

		public void Install(string fileName, InstallReference objRef)
		{
			IntPtr pRef = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Fusion.FUSION_INSTALL_REFERENCE)));

			Marshal.StructureToPtr((Fusion.FUSION_INSTALL_REFERENCE)objRef, pRef, false);
			try
			{
				Install(fileName, pRef);
			}
			finally
			{
				Marshal.DestroyStructure(pRef, typeof(Fusion.FUSION_INSTALL_REFERENCE));

				Marshal.FreeCoTaskMem(pRef);
			}
		}

		public UninstallDisposition Uninstall(string assemblyName, IntPtr pRef)
		{
			IAssemblyCache cache;

			ComUtil.ComCheck(Fusion.CreateAssemblyCache(out cache, 0));

			ASM_UNINSTALL_DISPOSITION disposition;

			ComUtil.ComCheck(cache.UninstallAssembly(0, assemblyName, pRef, out disposition));

			return (UninstallDisposition)disposition;        
		}

		public UninstallDisposition Uninstall(string assemblyName)
		{
			return Uninstall(assemblyName, IntPtr.Zero);
		}

		public UninstallDisposition Uninstall(string assemblyName, InstallReference objRef)
		{
			IntPtr pRef = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Fusion.FUSION_INSTALL_REFERENCE)));

			Marshal.StructureToPtr((Fusion.FUSION_INSTALL_REFERENCE)objRef, pRef, false);
			try
			{
				return Uninstall(assemblyName, pRef);
			}
			finally
			{
				Marshal.DestroyStructure(pRef, typeof(Fusion.FUSION_INSTALL_REFERENCE));

				Marshal.FreeCoTaskMem(pRef);
			}
		}

		public bool Check(string assemblyName, out string msg)
		{
			IAssemblyCache cache;

			ComUtil.ComCheck(Fusion.CreateAssemblyCache(out cache, 0));

			ASSEMBLY_INFO info = new ASSEMBLY_INFO();

			info.cbAssemblyInfo = (uint)Marshal.SizeOf(info);
			info.cchBuf = 0;
			info.pszCurrentAssemblyPathBuf = IntPtr.Zero;

			int hr = cache.QueryAssemblyInfo(QUERYASMINFO_FLAG.QUERYASMINFO_FLAG_VALIDATE, assemblyName, ref info);

			if(hr == ComUtil.ERROR_INSUFFICIENT_BUFFER && info.cchBuf > 0)
			{
				info.pszCurrentAssemblyPathBuf = Marshal.AllocCoTaskMem((int)info.cchBuf * 2);
				try
				{
					ComUtil.ComCheck(cache.QueryAssemblyInfo(
						QUERYASMINFO_FLAG.QUERYASMINFO_FLAG_VALIDATE | QUERYASMINFO_FLAG.QUERYASMINFO_FLAG_GETSIZE, 
						assemblyName, ref info));

					string path = Marshal.PtrToStringUni(info.pszCurrentAssemblyPathBuf);

					msg = string.Format("{0} ({1} KB) @ {2}", assemblyName, info.uliAssemblySizeInKB, path);
				}
				finally
				{
					Marshal.FreeCoTaskMem(info.pszCurrentAssemblyPathBuf);
				}

				return true;
			}
			else
			{
				try { Marshal.ThrowExceptionForHR(hr); msg = ""; } 
				catch(Exception e) { msg = e.Message; }

				return false;
			}
		}
	}
}
