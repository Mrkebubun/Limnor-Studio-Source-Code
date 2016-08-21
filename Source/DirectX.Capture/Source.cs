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
using System.Runtime.InteropServices; 
using DirectShowLib;

namespace DirectX.Capture
{
	/// <summary>
	///  Represents a physical connector or source on an audio/video device.
	/// </summary>
	public class Source : IDisposable
	{

		// --------------------- Private/Internal properties -------------------------
        /// <summary>
        /// Name of the source
        /// </summary>
		protected string				name;			



		// ----------------------- Public properties -------------------------

		/// <summary> The name of the source. Read-only. </summary>
		public string Name { get { return( name ); } }

		/// <summary> Obtains the String representation of this instance. </summary>
		public override string ToString() { return( Name ); }

		/// <summary> Is this source enabled. </summary>
		public virtual bool Enabled 
		{
			get { throw new NotSupportedException( "This method should be overriden in derrived classes." ); } 
			set { throw new NotSupportedException( "This method should be overriden in derrived classes." ); } 
		}

		
		// -------------------- Constructors/Destructors ----------------------

		/// <summary> Release unmanaged resources. </summary>
		~Source()
		{
			Dispose();
		}


		
		// -------------------- IDisposable -----------------------

		/// <summary> Release unmanaged resources. </summary>
		public virtual void Dispose()
		{
			name = null;
		}

	}
}
