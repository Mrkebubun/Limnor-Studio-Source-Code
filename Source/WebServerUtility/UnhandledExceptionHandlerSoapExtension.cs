using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using System.IO;
using System.Web.Services;

//''' <summary>
//''' Implements ASP unhandled exception manager as a SoapExtension
//''' </summary>
//''' <remarks>
//''' to use:
//''' 
//'''    1) place WebServerUtility.dll in the \bin folder
//'''    2) add this section to your Web.config under the <webServices> element:
//'''			<!-- Adds our error handler to the SOAP pipeline. -->
//'''		    <soapExtensionTypes>
//'''				<add type="Limnor.WebServer.UnhandledExceptionHandlerSoapExtension, WebServerUtility"
//'''				     priority="1" group="0" />
//'''			</soapExtensionTypes>
//'''
//'''  Jeff Atwood
//'''  http://www.codinghorror.com/
//''' </remarks>
namespace Limnor.WebServer
{
	public class UnhandledExceptionHandlerSoapExtension : SoapExtension
	{
		private Stream _OldStream;
		private Stream _NewStream;

		public override object GetInitializer(Type serviceType)
		{
			return null;
		}

		public override object GetInitializer(
		System.Web.Services.Protocols.LogicalMethodInfo methodInfo,
		System.Web.Services.Protocols.SoapExtensionAttribute attribute)
		{
			return null;
		}

		public override void Initialize(object initializer)
		{
		}

		public override Stream ChainStream(Stream stream)
		{
			_OldStream = stream;
			_NewStream = new MemoryStream();
			return _NewStream;
		}

		private void Copy(Stream fromStream, Stream toStream)
		{
			int size = 0x2000;
			if ((fromStream.CanSeek))
			{
				size = Math.Min((int)((fromStream.Length - fromStream.Position)), 0x2000);
			}
			byte[] buffer = new byte[size];
			int n;
			while (true)
			{
				n = fromStream.Read(buffer, 0, buffer.Length);
				if (n == 0)
				{
					break;
				}
				toStream.Write(buffer, 0, n);
			}
			toStream.Flush();
		}

		public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
		{
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeDeserialize:
					Copy(_OldStream, _NewStream);
					_NewStream.Position = 0;
					break;
				case SoapMessageStage.AfterSerialize:
					if (message.Exception != null)
					{
						Handler ueh = new Handler();
						string strDetailNode;
						//-- handle our exception, and get the SOAP <detail> string
						strDetailNode = ueh.HandleWebServiceException(message);
						//-- read the entire SOAP message stream into a string
						_NewStream.Position = 0;
						TextReader tr = new StreamReader(_NewStream);
						//-- insert our exception details into the string
						string s = tr.ReadToEnd();
						s = s.Replace("<detail />", strDetailNode);
						//-- overwrite the stream with our modified string
						_NewStream = new MemoryStream();
						TextWriter tw = new StreamWriter(_NewStream);
						tw.Write(s);
						tw.Flush();
					}
					_NewStream.Position = 0;
					Copy(_NewStream, _OldStream);
					break;
			}
		}

	}
}
