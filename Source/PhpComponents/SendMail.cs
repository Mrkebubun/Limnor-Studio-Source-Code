/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Limnor.WebServerBuilder;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.UI;
using VPL;
using System.Drawing.Design;
using Limnor.WebBuilder;
using System.IO;
using System.Windows.Forms;

namespace Limnor.PhpComponents
{
	[ToolboxBitmapAttribute(typeof(SendMail), "Resources.sendMail.bmp")]
	[Description("This component can be used to send emails from a web server.")]
	public class SendMail : IComponent, IWebServerProgrammingSupport, IWebResourceFileUser, IFormSubmitter
	{
		#region fields and constructors
		const string def_Mimeversion = "1.0";
		const string def_Charset = "ISO-8859-1";
		private List<WebResourceFile> _resourceFiles;
		public SendMail()
		{
			setDefaults();
		}
		public SendMail(IContainer c)
			: this()
		{
			if (c != null)
			{
				c.Add(this);
			}
		}
		private void setDefaults()
		{
			SMTP_Server = "mail.yourserver.com";
			SMTP_Port = 25;
			SMTP_Authenticate = true;
			SMTP_Secure = string.Empty;
			EMailSender = EnumSender.PhpMail;
			MimeVersion = def_Mimeversion;
			Charset = def_Charset;
			IsBodyHtml = false;
			_resourceFiles = new List<WebResourceFile>();
			string file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "class.phpmailer.php");
			if (!File.Exists(file))
			{
				if (this.Site != null && this.Site.DesignMode)
				{
					MessageBox.Show("File not found:" + file);
				}
			}
			bool b;
			_resourceFiles.Add(new WebResourceFile(file, WebResourceFile.WEBFOLDER_Php, out b));
			file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "class.pop3.php");
			if (!File.Exists(file))
			{
				if (this.Site != null && this.Site.DesignMode)
				{
					MessageBox.Show("File not found:" + file);
				}
			}
			_resourceFiles.Add(new WebResourceFile(file, WebResourceFile.WEBFOLDER_Php, out b));
			file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "class.smtp.php");
			if (!File.Exists(file))
			{
				if (this.Site != null && this.Site.DesignMode)
				{
					MessageBox.Show("File not found:" + file);
				}
			}
			_resourceFiles.Add(new WebResourceFile(file, WebResourceFile.WEBFOLDER_Php, out b));
			file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "class.html2text.inc");
			if (!File.Exists(file))
			{
				if (this.Site != null && this.Site.DesignMode)
				{
					MessageBox.Show("File not found:" + file);
				}
			}
			_resourceFiles.Add(new WebResourceFile(file, WebResourceFile.WEBFOLDER_Php, out b));
		}
		#endregion

		#region IWebServerProgrammingSupport Members
		[Browsable(false)]
		public bool DoNotCreate()
		{
			return false;
		}
		[Browsable(false)]
		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> sc = new Dictionary<string, string>();
			sc.Add("sendMail.php", Resource1.SendMail_php);
			return sc;
		}
		[Browsable(false)]
		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return (webServerProcessor == EnumWebServerProcessor.PHP);
		}
		[Browsable(false)]
		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}
		[Browsable(false)]
		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "Send") == 0)
			{
				string rcv = string.Empty;
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					rcv = string.Format(CultureInfo.InvariantCulture, "{0}=", returnReceiver);
				}
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}$this->{1}->Send();\r\n", rcv, Site.Name));
			}
		}
		[Browsable(false)]
		public void OnRequestStart(Page webPage)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestStartPhp(StringCollection code)
		{
			if (Attachments != null)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\n$this->{0}->AddAttachments($this->{1});",
					this.Site.Name, Attachments.Site.Name));
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public bool NeedObjectName { get { return false; } }
		#endregion

		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		private ISite _site;
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
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Properties
		[Description("Gets and sets SMTP debug option")]
		[WebServerMember]
		public EnumSmtpDebug SMTPDebug { get; set; }

		[Browsable(false)]
		[WebServerMember]
		public int SMTP_Debug
		{
			get
			{
				return (int)SMTPDebug;
			}
			set
			{
				SMTPDebug = (EnumSmtpDebug)(value);
			}
		}

		[DefaultValue("mail.yourserver.com")]
		[Description("Gets and sets SMTP server")]
		[WebServerMember]
		public string SMTP_Server { get; set; }
		[Description("Gets and sets SMTP port")]
		[WebServerMember]
		public int SMTP_Port { get; set; }

		[Description("Gets and sets a Boolean indicating whether SMTP Authentication is needed")]
		[WebServerMember]
		public bool SMTP_Authenticate { get; set; }

		[Description("Gets and sets SMTP user name for Authentication")]
		[WebServerMember]
		public string SMTP_UserName { get; set; }
		[Description("Gets and sets SMTP password for Authentication")]
		[WebServerMember]
		public string SMTP_Password { get; set; }

		[Description("Gets and sets SMTP security type, it can be ssl, tls, or leave it empty.")]
		[WebServerMember]
		public string SMTP_Secure { get; set; }

		[DefaultValue(EnumSender.PhpMail)]
		[Description("Gets and sets mail sender")]
		[WebServerMember]
		public EnumSender EMailSender { get; set; }

		[Browsable(false)]
		[WebServerMember]
		public int MailSender
		{
			get
			{
				return (int)EMailSender;
			}
			set
			{
				EMailSender = (EnumSender)(value);
			}
		}

		[Description("Gets and sets an email address for 'To'")]
		[WebServerMember]
		public string To { get; set; }
		[Description("Gets and sets a display text for 'To'")]
		[WebServerMember]
		public string ToName { get; set; }

		[Description("Gets and sets an email address for 'From'")]
		[WebServerMember]
		public string From { get; set; }
		[Description("Gets and sets a display text for 'From'")]
		[WebServerMember]
		public string FromName { get; set; }

		[Description("Gets and sets email addresses, seperated by spaces, for 'Cc'")]
		[WebServerMember]
		public string Cc { get; set; }
		[Description("Gets and sets email addresses, seperated by spaces, for 'Bcc'")]
		[WebServerMember]
		public string Bcc { get; set; }

		[Description("Gets and sets email message")]
		[WebServerMember]
		public string Body { get; set; }
		[Description("Gets and sets email subject")]
		[WebServerMember]
		public string Subject { get; set; }
		[WebServerMember]
		public string Charset { get; set; }
		[Description("Gets and sets a Boolean indicating whether the email message is of HTML format")]
		[WebServerMember]
		public bool IsBodyHtml { get; set; }
		[WebServerMember]
		public string MimeVersion { get; set; }
		[WebServerMember]
		public string ErrorMessage { get; set; }
		[WebServerMember]
		public bool SendCompleted { get; set; }

		[Description("Specify a HtmlFilesSelector control for including attachments")]
		[ComponentReferenceSelectorType(typeof(HtmlFilesSelector))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent Attachments
		{
			get;
			set;
		}
		#endregion

		#region Methods
		[WebServerMember]
		[Description("Send an email defined by the properties of this object, using the web server configurations.")]
		public bool Send()
		{
			return true;
		}
		[WebServerMember]
		[Description("Use this method to add server files as attachments before calling Send. To attach client files set the Attachments property to a HtmlFilesSelector control on the web page.")]
		public void AddAttachment(string file)
		{
		}
		#endregion


		#region IWebResourceFileUser Members
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		#endregion

		#region IFormSubmitter Members
		[Browsable(false)]
		public string FormName
		{
			get
			{
				if (Attachments != null)
				{
					return Attachments.Site.Name;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public bool IsSubmissionMethod(string method)
		{
			return false;
		}
		[Browsable(false)]
		public bool RequireSubmissionMethod(string method)
		{
			if (string.CompareOrdinal(method, "Send") == 0)
			{
				return (Attachments != null);
			}
			return false;
		}

		#endregion
	}
	public enum EnumSmtpDebug { None = 0, ErrorsAndMessages = 1, Messages = 2 }
	public enum EnumSender { Smtp = 0, PhpMail = 1, Qmail = 2, SendMail = 3 }
}
