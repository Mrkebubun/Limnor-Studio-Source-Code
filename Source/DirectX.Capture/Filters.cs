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
using DirectShowLib;

namespace DirectX.Capture
{
	/// <summary>
	///  Provides collections of devices and compression codecs
	///  installed on the system. 
	/// </summary>
	/// <example>
	///  Devices and compression codecs are implemented in DirectShow 
	///  as filters, see the <see cref="Filter"/> class for more 
	///  information. To list the available video devices:
	///  <code><div style="background-color:whitesmoke;">
	///   Filters filters = new Filters();
	///   foreach ( Filter f in filters.VideoInputDevices )
	///   {
	///		Debug.WriteLine( f.Name );
	///   }
	///  </div></code>
	///  <seealso cref="Filter"/>
	/// </example>
	public class Filters
    {
        #region fields and constructors
        private FilterCollection _videoInputDevices;
        private FilterCollection _audioInputDevices;
        private FilterCollection _videoCompressors;
        private FilterCollection _audioCompressors;
        /// <summary>
        /// load all filters
        /// </summary>
        public Filters()
        {
            try
            {
                _videoInputDevices = new FilterCollection(FilterCategory.VideoInputDevice);
            }
            catch (Exception err)
            {
                CErrorLog.ErrorLog.AddError("VideoInputDevice", err.Message);
            }
            try
            {
                _audioInputDevices = new FilterCollection(FilterCategory.AudioInputDevice);
            }
            catch (Exception err)
            {
                CErrorLog.ErrorLog.AddError("AudioInputDevices", err.Message);
            }
            try
            {
                _videoCompressors = new FilterCollection(FilterCategory.VideoCompressorCategory);
            }
            catch (Exception err)
            {
                CErrorLog.ErrorLog.AddError("AudioInputDevices", err.Message);
            }
            try
            {
                _audioCompressors = new FilterCollection(FilterCategory.AudioCompressorCategory);
            }
            catch (Exception err)
            {
                CErrorLog.ErrorLog.AddError("AudioInputDevices", err.Message);
            }
        }
        #endregion
        #region Properties 

		/// <summary> Collection of available video capture devices. </summary>
        public FilterCollection VideoInputDevices
        {
            get
            {
                //if (_videoInputDevices == null)
                //{
                //    try
                //    {
                //        _videoInputDevices = new FilterCollection(FilterCategory.VideoInputDevice);
                //    }
                //    catch (Exception err)
                //    {
                //        CErrorLog.ErrorLog.AddError("VideoInputDevice", err.Message);
                //    }
                //}
                return _videoInputDevices;
            }
        }

		/// <summary> Collection of available audio capture devices. </summary>
        public FilterCollection AudioInputDevices
        {
            get
            {
                
                return _audioInputDevices;
            }
        }

		/// <summary> Collection of available video compressors. </summary>
        public FilterCollection VideoCompressors
        {
            get
            {
                //try
                //{
                //    _videoCompressors = new FilterCollection(FilterCategory.VideoCompressorCategory);
                //}
                //catch (Exception err)
                //{
                //    CErrorLog.ErrorLog.AddError("AudioInputDevices", err.Message);
                //}
                return _videoCompressors;
            }
        }

		/// <summary> Collection of available audio compressors. </summary>
        public FilterCollection AudioCompressors
        {
            get
            {
                //try
                //{
                //    _audioCompressors = new FilterCollection(FilterCategory.AudioCompressorCategory);
                //}
                //catch (Exception err)
                //{
                //    CErrorLog.ErrorLog.AddError("AudioInputDevices", err.Message);
                //}
                return _audioCompressors;
            }
        }
        #endregion

    }
}
