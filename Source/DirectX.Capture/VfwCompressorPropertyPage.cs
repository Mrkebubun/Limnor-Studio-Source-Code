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
using System.Windows.Forms;
using DirectShowLib;

namespace DirectX.Capture
{
	/// <summary>
	///  The property page to configure a Video for Windows compliant
	///  compression codec. Most compressors support this property page
	///  rather than a DirectShow property page. Also, most compressors
	///  do not support the IAMVideoCompression interface so this
	///  property page is the only method to configure a compressor. 
	/// </summary>
	public class VfwCompressorPropertyPage : PropertyPage
	{

		// ---------------- Properties --------------------

		/// <summary> Video for Windows compression dialog interface </summary>
		protected IAMVfwCompressDialogs vfwCompressDialogs = null;

		/// <summary> 
		///  Get or set the state of the property page. This is used to save
		///  and restore the user's choices without redisplaying the property page.
		///  This property will be null if unable to retrieve the property page's
		///  state.
		/// </summary>
		/// <remarks>
		///  After showing this property page, read and store the value of 
		///  this property. At a later time, the user's choices can be 
		///  reloaded by setting this property with the value stored earlier. 
		///  Note that some property pages, after setting this property, 
		///  will not reflect the new state. However, the filter will use the
		///  new settings.
		/// </remarks>
		public override byte[] State
		{
			get 
			{ 
				byte[] data = null;
				int size = 0;

				int hr = vfwCompressDialogs.GetState( IntPtr.Zero, ref size );
				if ( ( hr == 0 ) && ( size > 0 ) )
				{
					data = new byte[size];
                    int sizeAr = Marshal.SizeOf(data[0]) * data.Length;

                    IntPtr pnt = Marshal.AllocHGlobal(sizeAr);
                    try
                    {
                        hr = vfwCompressDialogs.GetState(pnt, ref size);
                        if (hr != 0)
                        {
                            data = null;
                        }
                        else
                        {
                            Marshal.Copy(pnt, data, 0, data.Length);
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pnt);
                    }
				}
				return( data );
			}
			set 
			{
                int sizeAr = Marshal.SizeOf(value[0]) * value.Length;
                IntPtr pnt = Marshal.AllocHGlobal(sizeAr);
                try
                {
                    Marshal.Copy(value, 0, pnt, value.Length);

                    int hr = vfwCompressDialogs.SetState(pnt, value.Length);
                    if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                }
                finally
                {
                    Marshal.FreeHGlobal(pnt);
                }
			}
		}


		// ---------------- Constructors --------------------

		/// <summary> Constructor </summary>
		public VfwCompressorPropertyPage(string name, IAMVfwCompressDialogs compressDialogs)
		{
			Name = name;
			SupportsPersisting = true;
			this.vfwCompressDialogs = compressDialogs;
		}



		// ---------------- Public Methods --------------------

		/// <summary> 
		///  Show the property page. Some property pages cannot be displayed 
		///  while previewing and/or capturing. 
		/// </summary>
		public override void Show(Control owner)
		{
			vfwCompressDialogs.ShowDialog( VfwCompressDialogs.Config, owner.Handle );
		}

	}
}
