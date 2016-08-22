/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace VPL
{
	public class WebServiceProxy
	{
		private string _proxyCodeFile;
		private string _asmxUrl;
		private Assembly _assembly;
		private List<Type> _proxyTypes;
		public WebServiceProxy(string codeFile, Assembly assembly, string urlAsmx)
		{
			_proxyCodeFile = codeFile;
			_assembly = assembly;
			_asmxUrl = urlAsmx;
		}
		public string AsmxUrl
		{
			get
			{
				return _asmxUrl;
			}
		}
		public string ProxyFile
		{
			get
			{
				return _proxyCodeFile;
			}
		}
		public Assembly ProxyAssembly
		{
			get
			{
				return _assembly;
			}
		}
		public List<Type> ProxyTypes
		{
			get
			{
				if (_proxyTypes == null)
				{
					_proxyTypes = new List<Type>();
					Type[] tps = _assembly.GetExportedTypes();
					if (tps != null && tps.Length > 0)
					{
						for (int i = 0; i < tps.Length; i++)
						{
							if (typeof(System.Web.Services.Protocols.SoapHttpClientProtocol).IsAssignableFrom(tps[i]))
							{
								_proxyTypes.Add(tps[i]);
							}
						}
					}
				}
				return _proxyTypes;
			}
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			List<Type> ps = ProxyTypes;
			if (ps.Count > 0)
			{
				sb.Append(ps[0].Name);
				for (int i = 1; i < ps.Count; i++)
				{
					sb.Append(", ");
					sb.Append(ps[i].Name);
				}
			}
			return sb.ToString();
		}
	}
}
