<?php
class SendMail
{
	public $SMTP_Debug;
	public $SMTP_Server;
	public $SMTP_Port;
	public $SMTP_Authenticate;
	public $SMTP_UserName;
	public $SMTP_Password;
	public $MailSender;
	public $From;
	public $FromName;
	public $To;
	public $ToName;
	public $Cc;
	public $Bcc;
	public $Body;
	public $Subject;
	public $Charset;
	public $IsBodyHtml;
	public $MimeVersion;
	public $ErrorMessage;
	public $SendCompleted;
	public $files;
	function __construct() 
	{
		$this->files = array();
		$this->SMTP_Debug = 0;
		$this->SMTP_Server = '';
		$this->SMTP_Port = 25;
		$this->SMTP_Authenticate = false;
		$this->SMTP_UserName = '';
		$this->SMTP_Password = '';
    
		$this->MailSender = 1;
    
		$this->From = '';
		$this->FromName = '';
		$this->To = '';
		$this->ToName = '';
		$this->Cc = '';
		$this->Bcc = '';
		$this->Body = '';
		$this->Subject = '';
		$this->Charset = '';
		$this->IsBodyHtml = false;
		$this->MimeVersion = '';
		$this->ErrorMessage = '';
		$this->SendCompleted = false;
	}
	public function Send()
	{
		$this->SendCompleted = false;
		$this->ErrorMessage = '';
		//
		if($GLOBALS["debug"])
		{
			echo "Sending email.<br>";
		}
		try
		{
			if(!filter_var($this->To, FILTER_VALIDATE_EMAIL)) 
			{
				$this->ErrorMessage = 'Invalid email address<br>';
				if($GLOBALS["debug"])
				{
					echo 'Invalid email address<br>';
				}
			}
			else
			{
				if($GLOBALS["debug"])
				{
					echo "To:". $this->To. ".<br>";
					echo "From:". $this->From. ".<br>";
				}
				require_once("libphp/class.phpmailer.php");
				$mail = new PHPMailer();
				$mail->Host  = $this->SMTP_Server;
				$mail->Port  = $this->SMTP_Port;
				$mail->SMTPAuth  = $this->SMTP_Authenticate;
				$mail->Username  = $this->SMTP_UserName;
				$mail->Password  = $this->SMTP_Password;
				//Smtp = 0, PhpMail = 1, Qmail = 2, sendmail = 3
				if($this->MailSender == 0)
				{
					if($GLOBALS["debug"])
					{
						echo "Mail sender:SMTP.<br>";
					}
					$mail->IsSMTP();
					$mail->SMTPDebug  = $this->SMTP_Debug;
					$mail->Host  = $this->SMTP_Server;
					$mail->Port  = $this->SMTP_Port;
					$mail->SMTPAuth  = $this->SMTP_Authenticate;
					//
					if($GLOBALS["debug"])
					{
						echo "smtp host:". $this->SMTP_Server. ".<br>";
						echo "smtp port:". $this->SMTP_Port. ".<br>";
						echo "smtp Auth:". $this->SMTP_Authenticate. ".<br>";
						echo "smtp user:". $this->SMTP_UserName. ".<br>";
						echo "smtp pass:". $this->SMTP_Password. ".<br>";
						echo "============<br>";
						echo "smtp host:". $mail->Host. ".<br>";
						echo "smtp port:". $mail->Port. ".<br>";
						echo "smtp Auth:". $mail->SMTPAuth. ".<br>";
						echo "smtp user:". $mail->Username. ".<br>";
						echo "smtp pass:". $mail->Password. ".<br>";
					}
				}
				else if($this->MailSender == 1)
				{
					$mail->IsMail();
					if($GLOBALS["debug"])
					{
						echo "Mail sender:PHP.<br>";
					}
				}
				else if($this->MailSender == 2)
				{
					$mail->IsQmail();
					if($GLOBALS["debug"])
					{
						echo "Mail sender:Qmail.<br>";
					}
				}
				else if($this->MailSender == 3)
				{
					$mail->IsSendmail();
					if($GLOBALS["debug"])
					{
						echo "Mail sender:SendMail.<br>";
					}
				}
				else
				{
					$mail->IsMail();
					if($GLOBALS["debug"])
					{
						echo "Mail sender:Php.<br>";
					}
				}
				if($this->FromName != '')
				{
					$mail->AddReplyTo($this->From,$this->FromName);
					$mail->From = $this->From;
					$mail->FromName = $this->FromName;
				}
				else
				{
					$mail->AddReplyTo($this->From);
					$mail->From = $this->From;
					$mail->FromName = $this->From;
				}
				if($this->ToName != '')
				{
					$mail->AddAddress($this->To, $this->ToName);
				}
				else
				{
					$mail->AddAddress($this->To);
				}
				if($this->Bcc != '')
				{
					$indiBCC = explode(" ", $this->Bcc);
					foreach ($indiBCC as $key => $value) 
					{
						$mail->AddBCC($value);
					}
				}
				if ( $this->Cc != '' ) 
				{
					$indiCC = explode(" ", $this->Cc);
					foreach ($indiCC as $key => $value) 
					{
						$mail->AddCC($value);
					}
				}
				$mail->Subject = $this->Subject;
				if($GLOBALS["debug"])
				{
					echo "Subject:".$mail->Subject."<br>";
					echo "Body:".$this->Body."<br>";
				}
				require_once('libphp/class.html2text.inc');
				$h2t = new html2text($this->Body);
				$mail->AltBody = $h2t->get_text();
				$mail->MsgHTML($this->Body);
				$mail->IsHTML($this->IsBodyHtml);
				$cn = count($this->files);
				if($GLOBALS["debug"])
				{
					echo "Attachments:". $cn. ".<br>";
				}
				if($cn>0)
				{
					foreach ($this->files as $i => $f) 
					{
						if($GLOBALS["debug"])
						{
							echo "file:[". $f. "].<br>";
						}
						if(strlen($f) > 0)
						{
							if($GLOBALS["debug"])
							{
								echo "adding file:[". $f. "].<br>";
							}
							$mail->AddAttachment($f);
						}
					}
				}
				if ( $mail->Send() ) 
				{
					$this->ErrorMessage = "OK";
					$this->SendCompleted = true;
					if($GLOBALS["debug"])
					{
						echo "mail sent to mail server.<br>";
					}
				}
				else
				{
					$this->ErrorMessage = "Unable to send the email.";
					if($GLOBALS["debug"])
					{
						echo $this->ErrorMessage. "<br>";
					}
				}
			}
		}
		catch (Exception $e)
		{
			$this->ErrorMessage = $e->getMessage();
			if($GLOBALS["debug"])
			{
				echo "Error sending mail. ". $this->ErrorMessage. "<br>";
			}
		}
		return ($this->ErrorMessage == "OK");
	}
	public function AddAttachments($files)
	{
		foreach($files->UploadedFilePaths as $i => $f)
		{
			$this->files[] = $f;
		}
	}
	public function AddAttachment($file)
	{
		$this->files[] = $file;
	}
}
?>