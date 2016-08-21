/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Reflection;

namespace Limnor.Net
{
	[ToolboxBitmapAttribute(typeof(MailSender), "Resources.sendMail.bmp")]
	[Description("This component can be used to send emails with attachments and embeded images. It supports Secure Socket Layer (SSL) connections. It supports authentication request of SMTP servers. ")]
	public class MailSender : IComponent
	{
		#region fields and constructors
		private SmtpClient _smtp;
		private MailMessage _message;
		private Guid _guid = Guid.Empty;
		private string _errorMessage;
		public MailSender()
		{
			_smtp = new SmtpClient();
			_smtp.UseDefaultCredentials = true;
			_message = new MailMessage();
			_smtp.SendCompleted += new SendCompletedEventHandler(_smtp_SendCompleted);
		}

		void _smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (SendCompleted != null)
			{
				SendCompleted(this, e);
			}
		}
		[Browsable(false)]
		[Description("The Guid identifies this object instance")]
		public Guid ObjectId
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
		}
		#endregion

		#region Events
		[Description("Occurs when an asynchronous email send operation completes")]
		public event SendCompletedEventHandler SendCompleted;
		[Description("Occurs when an error occurs calling Send or SendAsync")]
		public event OperationFailHandler OperationFailed;
		#endregion

		#region SMTP Properties
		[Description("This is the object for sending the email")]
		public SmtpClient Smtp
		{
			get
			{
				return _smtp;
			}
		}
		[DefaultValue(null)]
		[Description("Gets or sets the name or IP address of the host used for SMTP transactions")]
		public string MailServer
		{
			get
			{
				return _smtp.Host;
			}
			set
			{
				if (value != null)
				{
					_smtp.Host = value;
				}
			}
		}
		[DefaultValue(25)]
		[Description("Gets or sets the port used for SMTP transactions")]
		public int Port
		{
			get
			{
				return _smtp.Port;
			}
			set
			{
				_smtp.Port = value;
			}
		}
		[DefaultValue(false)]
		[Description("Specify whether Secure Socket Layer (SSL) is used to encrypt the connection")]
		public bool EnableSsl
		{
			get
			{
				return _smtp.EnableSsl;
			}
			set
			{
				_smtp.EnableSsl = value;
			}
		}
		[DefaultValue(100000)]
		[Description("Gets or sets a value that specifies the amount of time after which a synchronous Send action times out")]
		public int Timeout
		{
			get
			{
				return _smtp.Timeout;
			}
			set
			{
				_smtp.Timeout = value;
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a value that controls whether DefaultCredentials are sent with requests")]
		public bool UseDefaultCredentials
		{
			get
			{
				return _smtp.UseDefaultCredentials;
			}
			set
			{
				_smtp.UseDefaultCredentials = value;
			}
		}
		[Description("Gets the network connection used to transmit the e-mail message")]
		public ServicePoint ServicePoint
		{
			get
			{
				return _smtp.ServicePoint;
			}
		}
		[Description("A string that represents the SPN to use for authentication when using extended protection to connect to an SMTP mail server. This is only supported by Microsoft .Net 4 and later")]
		[DefaultValue("SMTPSVC/")]
		public string TargetName
		{
			get
			{
				if (System.Environment.Version.Major >= 4)
				{
					PropertyInfo pif = _smtp.GetType().GetProperty("TargetName");
					if (pif != null)
					{
						return (string)pif.GetValue(_smtp, new object[] { });
					}
				}
				return "SMTPSVC/";
			}
			set
			{
				if (System.Environment.Version.Major >= 4)
				{
					PropertyInfo pif = _smtp.GetType().GetProperty("TargetName");
					if (pif != null)
					{
						pif.SetValue(_smtp, value, new object[] { });
					}
				}
			}
		}
		[Description("Specifies which certificates should be used to establish the Secure Socket Layer (SLL) connection")]
		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return _smtp.ClientCertificates;
			}
		}
		[DefaultValue(System.Net.Mail.SmtpDeliveryMethod.Network)]
		[Description("Specifies how the outgoing email messages will be handles")]
		public SmtpDeliveryMethod DeliveryMethod
		{
			get
			{
				return _smtp.DeliveryMethod;
			}
			set
			{
				_smtp.DeliveryMethod = value;
			}
		}
		[DefaultValue(null)]
		[Description("Gets or sets the folder where applications save mail messages to be processed by the local SMTP server")]
		public string PickupDirectoryLocation
		{
			get
			{
				return _smtp.PickupDirectoryLocation;
			}
			set
			{
				_smtp.PickupDirectoryLocation = value;
			}
		}
		#endregion

		#region Sender Properties
		private string _senderAddress;
		private string _senderDisplay;
		private EnumCharEncode _senderDisplayEncode = EnumCharEncode.Default;
		[DefaultValue(null)]
		public string SenderAddress
		{
			get
			{
				return _senderAddress;
			}
			set
			{
				_senderAddress = value;
			}
		}
		[DefaultValue(null)]
		public string SenderDisplay
		{
			get
			{
				return _senderDisplay;
			}
			set
			{
				_senderDisplay = value;
			}
		}
		[DefaultValue(EnumCharEncode.Default)]
		public EnumCharEncode SenderDisplayEncode
		{
			get
			{
				return _senderDisplayEncode;
			}
			set
			{
				_senderDisplayEncode = value;
			}
		}
		#endregion

		#region From Properties
		private string _fromAddress;
		private string _fromDisplay;
		private EnumCharEncode _fromDisplayEncode = EnumCharEncode.Default;
		[DefaultValue(null)]
		public string FromAddress
		{
			get
			{
				return _fromAddress;
			}
			set
			{
				_fromAddress = value;
			}
		}
		[DefaultValue(null)]
		public string FromDisplay
		{
			get
			{
				return _fromDisplay;
			}
			set
			{
				_fromDisplay = value;
			}
		}
		[DefaultValue(EnumCharEncode.Default)]
		public EnumCharEncode FromDisplayEncode
		{
			get
			{
				return _fromDisplayEncode;
			}
			set
			{
				_fromDisplayEncode = value;
			}
		}
		#endregion

		#region ReplyTo Properties
		private string _replyToAddress;
		private string _replyToDisplay;
		private EnumCharEncode _replyToDisplayEncode = EnumCharEncode.Default;
		[DefaultValue(null)]
		public string ReplyToAddress
		{
			get
			{
				return _replyToAddress;
			}
			set
			{
				_replyToAddress = value;
			}
		}
		[DefaultValue(null)]
		public string ReplyToDisplay
		{
			get
			{
				return _replyToDisplay;
			}
			set
			{
				_replyToDisplay = value;
			}
		}
		[DefaultValue(EnumCharEncode.Default)]
		public EnumCharEncode ReplyToDisplayEncode
		{
			get
			{
				return _replyToDisplayEncode;
			}
			set
			{
				_replyToDisplayEncode = value;
			}
		}
		#endregion

		#region Mail Properties

		[Description("Mail recipients")]
		public string Recipients
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (MailAddress a in _message.To)
				{
					sb.Append(a.Address);
					sb.Append(";");
				}
				return sb.ToString();
			}
		}
		[Description("Mail CC recipients")]
		public string CCRecipients
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (MailAddress a in _message.CC)
				{
					sb.Append(a.Address);
					sb.Append(";");
				}
				return sb.ToString();
			}
		}
		[Description("Mail BCC recipients")]
		public string BccRecipients
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (MailAddress a in _message.Bcc)
				{
					sb.Append(a.Address);
					sb.Append(";");
				}
				return sb.ToString();
			}
		}
		[DefaultValue(System.Net.Mail.MailPriority.Normal)]
		[Description("Gets or sets the priority of this email message")]
		public MailPriority Priority
		{
			get
			{
				return _message.Priority;
			}
			set
			{
				_message.Priority = value;
			}
		}
		[DefaultValue(false)]
		[Description("Gets or sets a value indicating whether the mail message body is in HTML")]
		public bool IsBodyHtml
		{
			get
			{
				return _message.IsBodyHtml;
			}
			set
			{
				_message.IsBodyHtml = value;
			}
		}
		[DefaultValue(System.Net.Mail.DeliveryNotificationOptions.None)]
		[Description("Gets or sets the delivery notifications for this email message.")]
		public DeliveryNotificationOptions DeliveryNotificationOptions
		{
			get
			{
				return _message.DeliveryNotificationOptions;
			}
			set
			{
				_message.DeliveryNotificationOptions = value;
			}
		}
		[DefaultValue("")]
		[Description("Gets or sets the subject line for this e-mail message")]
		public string Subject
		{
			get
			{
				return _message.Subject;
			}
			set
			{
				_message.Subject = value;
			}
		}
		private EnumCharEncode _subjectEncoding = EnumCharEncode.Default;
		[DefaultValue(EnumCharEncode.Default)]
		[Description("Gets or sets the encoding used for the subject content of this email message")]
		public EnumCharEncode SubjectEncoding
		{
			get
			{
				return _subjectEncoding;
			}
			set
			{
				_subjectEncoding = value;
				_message.SubjectEncoding = EncodeUtility.GetEncoding(value);
			}
		}
		[DefaultValue("")]
		[Description("Gets or sets the message body")]
		public string Body
		{
			get
			{
				return _message.Body;
			}
			set
			{
				_message.Body = value;
			}
		}
		private EnumCharEncode _bodyEncoding = EnumCharEncode.Default;
		[DefaultValue(EnumCharEncode.Default)]
		[Description("Gets or sets the encoding used to encode the message body")]
		public EnumCharEncode BodyEncoding
		{
			get
			{
				return _bodyEncoding;
			}
			set
			{
				_bodyEncoding = value;
				_message.BodyEncoding = EncodeUtility.GetEncoding(value);
			}
		}
		[Description("Gets a message indicating the error on executing a Send/SendAsync action if the action fails.")]
		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
		}
		#endregion

		#region Credential Properties
		[DefaultValue(null)]
		[Description("UserDomain, UserAccount and UserPassword are used as the credential for using the SMTP server when UseDefaultCredentials is false")]
		public string UserDomain { get; set; }
		[DefaultValue(null)]
		[Description("UserDomain, UserAccount and UserPassword are used as the credential for using the SMTP server when UseDefaultCredentials is false")]
		public string UserAccount { get; set; }

		[PasswordPropertyText(true)]
		[DefaultValue(null)]
		[Description("UserDomain, UserAccount and UserPassword are used as the credential for using the SMTP server when UseDefaultCredentials is false")]
		public string UserPassword { get; set; }
		#endregion

		#region Embed images
		private Dictionary<string, string> _embededImages;
		public void ClearEmbededImages()
		{
			_embededImages = null;
		}
		public void RemoveEmbededImage(string id)
		{
			if (_embededImages != null)
			{
				if (_embededImages.ContainsKey(id))
				{
					_embededImages.Remove(id);
				}
			}
		}
		public void AddEmbededImage(string id, string filePath)
		{
			if (_embededImages == null)
			{
				_embededImages = new Dictionary<string, string>();
			}
			if (_embededImages.ContainsKey(id))
			{
				_embededImages[id] = filePath;
			}
			else
			{
				_embededImages.Add(id, filePath);
			}
		}
		#endregion

		#region Attachments
		private string[] _attachments;
		public void ClearAttachments()
		{
			_attachments = null;
		}
		public void RemoveAttachment(string filename)
		{
			if (_attachments != null && _attachments.Length > 0)
			{
				for (int i = 0; i < _attachments.Length; i++)
				{
					if (string.Compare(_attachments[i], filename, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (_attachments.Length == 1)
						{
							_attachments = null;
						}
						else
						{
							string[] a = new string[_attachments.Length - 1];
							for (int j = 0; j < i; j++)
							{
								a[j] = _attachments[j];
							}
							for (int j = i; j < a.Length; j++)
							{
								a[j] = _attachments[j + 1];
							}
							_attachments = a;
						}
						break;
					}
				}
			}
		}

		public void AddAttachment(string filename)
		{
			if (_attachments == null || _attachments.Length == 0)
			{
				_attachments = new string[1];
				_attachments[0] = filename;
			}
			else
			{
				string[] a = new string[_attachments.Length + 1];
				_attachments.CopyTo(a, 0);
				a[_attachments.Length] = filename;
				_attachments = a;
			}
		}
		[DefaultValue(null)]
		[Description("Names for the files to be attached to the email")]
		public string[] Attachments
		{
			get
			{
				return _attachments;
			}
			set
			{
				_attachments = value;
			}
		}
		#endregion

		#region Methods
		public void AddRecipient(string address, string display, EnumCharEncode displayEncoding)
		{
			foreach (MailAddress a in _message.To)
			{
				if (string.Compare(a.Address, address, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}
			_message.To.Add(new MailAddress(address, display, EncodeUtility.GetEncoding(displayEncoding)));
		}
		public void ClearRecipients()
		{
			_message.To.Clear();
		}
		public void AddCCRecipient(string address, string display, EnumCharEncode displayEncoding)
		{
			foreach (MailAddress a in _message.CC)
			{
				if (string.Compare(a.Address, address, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}
			_message.CC.Add(new MailAddress(address, display, EncodeUtility.GetEncoding(displayEncoding)));
		}
		public void ClearCCRecipients()
		{
			_message.CC.Clear();
		}
		public void AddBCCRecipient(string address, string display, EnumCharEncode displayEncoding)
		{
			foreach (MailAddress a in _message.Bcc)
			{
				if (string.Compare(a.Address, address, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}
			_message.Bcc.Add(new MailAddress(address, display, EncodeUtility.GetEncoding(displayEncoding)));
		}
		public void ClearBCCRecipients()
		{
			_message.Bcc.Clear();
		}
		public void AddHeaderItem(string itemName, string itemContent)
		{
			for (int i = 0; i < _message.Headers.Keys.Count; i++)
			{
				if (string.Compare(itemName, _message.Headers.Keys[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					_message.Headers.Remove(itemName);
					break;
				}
			}
			_message.Headers.Add(itemName, itemContent);
		}
		private void prepareMessage()
		{
			if (!string.IsNullOrEmpty(_senderAddress))
			{
				_message.Sender = new MailAddress(_senderAddress, _senderDisplay, EncodeUtility.GetEncoding(_senderDisplayEncode));
			}
			if (!string.IsNullOrEmpty(_fromAddress))
			{
				_message.From = new MailAddress(_fromAddress, _fromDisplay, EncodeUtility.GetEncoding(_fromDisplayEncode));
			}
			if (!string.IsNullOrEmpty(_replyToAddress))
			{
				_message.ReplyTo = new MailAddress(_replyToAddress, _replyToDisplay, EncodeUtility.GetEncoding(_replyToDisplayEncode));
			}
			if (IsBodyHtml || (_embededImages != null && _embededImages.Count > 0))
			{
				AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, EncodeUtility.GetEncoding(_bodyEncoding), "text/html");
				_message.AlternateViews.Add(htmlView);
				if (_embededImages != null && _embededImages.Count > 0)
				{
					foreach (KeyValuePair<string, string> kv in _embededImages)
					{
						LinkedResource lr = new LinkedResource(kv.Value);
						lr.ContentId = kv.Key;
						htmlView.LinkedResources.Add(lr);
					}
				}
			}
			_message.Attachments.Clear();
			if (_attachments != null && _attachments.Length > 0)
			{
				for (int i = 0; i < _attachments.Length; i++)
				{
					_message.Attachments.Add(new Attachment(_attachments[i]));
				}
			}
			if (!UseDefaultCredentials)
			{
				NetworkCredential c = new NetworkCredential(UserAccount, UserPassword, UserDomain);
				_smtp.Credentials = c;
			}
		}
		[Description("Send the email to an SMTP server for delivery. It returns False if an error is detected.")]
		public bool Send()
		{
			try
			{
				prepareMessage();
				_smtp.Send(_message);
				return true;
			}
			catch (Exception e)
			{
				_errorMessage = e.Message;
				if (OperationFailed != null)
				{
					OperationFailed(this, new OperationFailEventArgs(e, "Error calling Send:{0}", _errorMessage));
				}
			}
			return false;
		}
		[Description("Send the email to an SMTP server for delivery. This method does not block the calling thread. When the send mail operation completes the event SendCompleted occurs. It returns False if an error is detected.")]
		public bool SendAsync()
		{
			try
			{
				prepareMessage();
				_smtp.SendAsync(_message, ObjectId);
				return true;
			}
			catch (Exception e)
			{
				_errorMessage = e.Message;
				if (OperationFailed != null)
				{
					OperationFailed(this, new OperationFailEventArgs(e, "Error calling SendAsync:{0}", _errorMessage));
				}
				else
				{
					//throw;
				}
			}
			return false;
		}
		[Description("Cancel an asynchronous operation to send an email message")]
		public void SendAsyncCancel()
		{
			_smtp.SendAsyncCancel();
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
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

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
