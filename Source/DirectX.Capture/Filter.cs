// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
//	2003-Jan-24		Brian Low		- created
//  2010-Apr-20     Jian Wang       - Converted to .Net 2.0
//                                  - Use DirectShow.Net from http://directshownet.sourceforge.net
//
// http://creativecommons.org/licenses/publicdomain/
// ------------------------------------------------------------------

using System;
using System.Collections;
using System.Runtime.InteropServices;
using DirectShowLib;
using System.Runtime.InteropServices.ComTypes;
//using DirectShowLib.Device;

namespace DirectX.Capture
{
	/// <summary>
	///  Represents a DirectShow filter (e.g. video capture device, 
	///  compression codec).
	/// </summary>
	/// <remarks>
	///  To save a chosen filer for later recall
	///  save the MonikerString property on the filter: 
	///  <code><div style="background-color:whitesmoke;">
	///   string savedMonikerString = myFilter.MonikerString;
	///  </div></code>
	///  
	///  To recall the filter create a new Filter class and pass the 
	///  string to the constructor: 
	///  <code><div style="background-color:whitesmoke;">
	///   Filter mySelectedFilter = new Filter( savedMonikerString );
	///  </div></code>
	/// </remarks>
	public class Filter : IComparable 
	{
		/// <summary> Human-readable name of the filter </summary>
		public string Name;

		/// <summary> Unique string referencing this filter. This string can be used to recreate this filter. </summary>
		public string MonikerString;

		/// <summary> Create a new filter from its moniker string. </summary>
		public Filter( string monikerString )
		{
			Name = getName( monikerString );
			MonikerString = monikerString;
		}

		/// <summary> Create a new filter from its moniker </summary>
		internal Filter( IMoniker moniker )
		{
			Name = getName( moniker );
			MonikerString = getMonikerString( moniker );
		}

		/// <summary> Retrieve the a moniker's display name (i.e. it's unique string) </summary>
		protected string getMonikerString(IMoniker moniker)
		{
			string s;
			moniker.GetDisplayName( null, null, out s );
			return( s );
		}

		/// <summary> Retrieve the human-readable name of the filter </summary>
		protected string getName(IMoniker moniker)
		{
			object bagObj = null;
			IPropertyBag bag = null;
			try 
			{
				Guid bagId = typeof( IPropertyBag ).GUID;
				moniker.BindToStorage( null, null, ref bagId, out bagObj );
				bag = (IPropertyBag) bagObj;
				object val = "";
                int hr = bag.Read("FriendlyName", out val, CErrorLog.ErrorLog);
				if( hr != 0 )
					Marshal.ThrowExceptionForHR( hr );
				string ret = val as string;
				if( (ret == null) || (ret.Length < 1) )
					throw new NotImplementedException( "Device FriendlyName" );
				return( ret );
			}
			catch( Exception )
			{
				return( "" );
			}
			finally
			{
				bag = null;
				if( bagObj != null )
					Marshal.ReleaseComObject( bagObj ); bagObj = null;
			}
		}

		/// <summary> Get a moniker's human-readable name based on a moniker string. </summary>
		protected string getName(string monikerString)
		{
			IMoniker parser = null; 
			IMoniker moniker = null;
			try
			{
				parser = getAnyMoniker();
				int eaten;
				parser.ParseDisplayName( null, null, monikerString, out eaten, out moniker );
				return( getName( parser ) );
			}
			finally
			{
				if ( parser != null )
					Marshal.ReleaseComObject( parser ); parser = null;
				if ( moniker != null )
					Marshal.ReleaseComObject( moniker ); moniker = null;
			}
		}

		/// <summary>
		///  This method gets a UCOMIMoniker object.
		/// 
		///  HACK: The only way to create a UCOMIMoniker from a moniker 
		///  string is to use UCOMIMoniker.ParseDisplayName(). So I 
		///  need ANY UCOMIMoniker object so that I can call 
		///  ParseDisplayName(). Does anyone have a better solution?
		/// 
		///  This assumes there is at least one video compressor filter
		///  installed on the system.
		/// </summary>
		protected IMoniker getAnyMoniker()
		{
			Guid				category = FilterCategory.VideoCompressorCategory;
			int					hr;
			object				comObj = null;
			ICreateDevEnum		enumDev = null;
			IEnumMoniker	enumMon = null;
			IMoniker[]		mon = new IMoniker[1];

			try 
			{
				// Get the system device enumerator
				Type srvType = Type.GetTypeFromCLSID( Clsid.SystemDeviceEnum );
				if( srvType == null )
					throw new NotImplementedException( "System Device Enumerator" );
				comObj = Activator.CreateInstance( srvType );
				enumDev = (ICreateDevEnum) comObj;

				// Create an enumerator to find filters in category
				hr = enumDev.CreateClassEnumerator( category, out enumMon, 0 );
				if( hr != 0 )
					throw new NotSupportedException( "No devices of the category" );

				// Get first filter
				IntPtr f = IntPtr.Zero;
				hr = enumMon.Next( 1, mon, f );
				if( (hr != 0) )
					mon[0] = null;

				return( mon[0] );
			}
			finally
			{
				enumDev = null;
				if( enumMon != null )
					Marshal.ReleaseComObject( enumMon ); enumMon = null;
				if( comObj != null )
					Marshal.ReleaseComObject( comObj ); comObj = null;
			}
		}
	
		/// <summary>
		///  Compares the current instance with another object of 
		///  the same type.
		/// </summary>
		public int CompareTo(object obj)
		{
			if ( obj == null )
				return( 1 );
			Filter f = (Filter) obj;
			return( this.Name.CompareTo( f.Name ) );
		}
        /// <summary>
        /// returns the name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
	}
}
