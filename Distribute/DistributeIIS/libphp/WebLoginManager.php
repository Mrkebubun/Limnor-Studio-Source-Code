<?php
class WebLoginManager
{
	public $myname;
	public $NameDelimiterBegin;
	public $NameDelimiterEnd;
	public $FailedMessageLableId;
	public $InactivityMinutes;
	public $LoginFailedMessage;
	public $UserAccountLoginFieldName;
	public $UserAccountPasswordFieldName;
	public $UserAccountLevelFieldName;
	public $UserAccountTableName;
	public $jsMySql;  //
	public $jsp;
	public $levelRead;
	public $COOKIE_UserLogin;
	function __construct($name)
	{
		$this->myname = $name;
	}
	public function Initialize($page, $c, $id)
	{
		$this->jsp = $page;
		$this->jsMySql = new JsonSourceMySql();
		$this->jsMySql->SetCredential($c);
		$this->COOKIE_UserLogin = $id;
		$this->levelRead = 0;
	}
	private function checkLogin($loginName, $password)
    {
    	$this->jsp->LogDebugInfo("using ". $this->myname. "<br>");
        $this->jsp->LogDebugInfo("checkLogin<br>");
        $bPassed = false;
        $sSQL;
        $sSQL = "SELECT ". 
					$this->NameDelimiterBegin. 
                    $this->UserAccountPasswordFieldName.
                    $this->NameDelimiterEnd.
                    ", PASSWORD(?)"; 
		if($this->UserAccountLevelFieldName != null && strlen($this->UserAccountLevelFieldName)>0)
		{
			$sSQL = $sSQL. ",".
					$this->NameDelimiterBegin. 
                    $this->UserAccountLevelFieldName.
                    $this->NameDelimiterEnd;
		}           
		$sSQL = $sSQL.        
                    " FROM ".
                    $this->NameDelimiterBegin. 
                    $this->UserAccountTableName.
                    $this->NameDelimiterEnd.
                    " WHERE ".
					$this->NameDelimiterBegin. 
                    $this->UserAccountLoginFieldName.
                    $this->NameDelimiterEnd.
                     "=?";
        $this->jsp->LogDebugInfo("SQL:". $sSQL. "<br>");
        $ps = array();
		$p = new SqlClientParameter();
		$p->name = '@c1';
		$p->type = 's';
		$p->value = $password;
		$ps[] = $p;
		//
		$p = new SqlClientParameter();
		$p->name = '@c2';
		$p->type = 's';
		$p->value = $loginName;
		$ps[] = $p;
		//
		$tbl = new JsonDataTable();
		$this->jsMySql->GetData($tbl, $sSQL, $ps);
		$rn = count($tbl->Rows);
		if($rn > 0)
        {
           $s1 = $tbl->Rows[0]->ItemArray[0];
           $s2 = $tbl->Rows[0]->ItemArray[1];
           if(count($tbl->Rows[0]->ItemArray)>2)
           {
				$this->levelRead = $tbl->Rows[0]->ItemArray[2];
				if($this->levelRead == NULL) 
				{
					$this->levelRead = 0;
				}
           }
           if ($s1 == $s2)
           {
               $bPassed = true;
           }
           else
           {
				$this->jsp->LogDebugInfo( "log in failed. password in db:[");
				$this->jsp->LogDebugInfo(  $s1);
				$this->jsp->LogDebugInfo(  "] password tried:[");
				$this->jsp->LogDebugInfo(  $s2);
				$this->jsp->LogDebugInfo(  "]<br>");
           }
        }
        else
        {
			$this->jsp->LogDebugInfo( "log in failed. invalid user:[");
			$this->jsp->LogDebugInfo( $loginName);
			$this->jsp->LogDebugInfo( "]<br>");
		}
        return $bPassed;
    }
	public function Login($loginName, $password)
	{
		$this->jsMySql->SetDebug($this->jsp->DEBUG);
        $userLogin = "";
        /*
        if (isset($_COOKIE[$this->COOKIE_UserLogin]))
		{
			if(strlen($_COOKIE[$this->COOKIE_UserLogin]) > 0)
			{
				$this->jsp->LogDebugInfo( "Already log in to:{0}". $_COOKIE[$this->COOKIE_UserLogin]);
				$this->jsp->LogDebugInfo( "<br>");
				$this->jsp->AddClientScript("JsonDataBinding.LoginPassed2();");

                return;
			}
		}
		*/
        $this->jsp->LogDebugInfo( "Start log in<br>");
        if(strlen($loginName) == 0 && strlen($password) == 0)
        {
			$this->jsp->LogDebugInfo( "Missing log in information. If the table is empty then a default account will be created.");
            //try to see if the table is empty
            $sSQL = "SELECT COUNT(*) FROM ".
                    $this->NameDelimiterBegin.
                    $this->UserAccountTableName.
                    $this->NameDelimiterEnd;
            $this->jsp->LogDebugInfo( "SQL:".  $sSQL. "<br>");
            $ps = array();
            $tbl = new JsonDataTable();
			$this->jsMySql->GetData($tbl, $sSQL, $ps);
			$rn = count($tbl->Rows);
			if($rn > 0)
			{
				$n1 = $tbl->Rows[0]->ItemArray[0];
                if ($n1 == 0)
                {
                    $this->jsp->LogDebugInfo( "Table [". $this->UserAccountTableName. "] is empty. The web page is not blocked. Create an account named Admin and password 123");
                    $sSQL = "INSERT INTO ". 
                                    $this->NameDelimiterBegin. 
                                    $this->UserAccountTableName.
                                    $this->NameDelimiterEnd. 
                                    "(".
                                    $this->NameDelimiterBegin. 
                                    $this->UserAccountLoginFieldName.
                                    $this->NameDelimiterEnd.  
                                    ",".
                                    $this->NameDelimiterBegin. 
                                    $this->UserAccountPasswordFieldName.
                                    $this->NameDelimiterEnd.                                     
                                    ") VALUES (?,PASSWORD(?))";
					$this->jsp->LogDebugInfo( "SQL:". $sSQL. "<br>");
					$ps = array();
					$p = new SqlClientParameter();
					$p->name = '@c1';
					$p->type = 's';
					$p->value = 'Admin';
					$ps[] = $p;
					//
					$p = new SqlClientParameter();
					$p->name = '@c2';
					$p->type = 's';
					$p->value = '123';
					$ps[] = $p;
					if($this->jsMySql->ExecuteNonQuery($sSQL, $ps))
					{
						$this->jsp->AddClientScript("JsonDataBinding.LoginFailed('". $this->FailedMessageLableId. "', 'A default account was added to the empty table [". $this->UserAccountTableName. "]. The account login name is Admin. The account password is 123. Please use this account to log in.');");
                    }
                    else
                    {
						$this->jsp->AddClientScript("JsonDataBinding.LoginFailed('". $this->FailedMessageLableId. "', 'The account table [". $this->UserAccountTableName. "] is empty.');");
                    }
                    return;
                }
            }
			$this->jsp->LogDebugInfo( "Missing login information");
        }
        //
        $bPassed = $this->checkLogin($loginName, $password);
        $this->jsp->SetServerComponentName($this->myname);
        if ($bPassed)
        {
           if ($this->InactivityMinutes <= 0)
           {
               $this->InactivityMinutes = 10;
           }
           $this->jsp->AddClientScript("JsonDataBinding.LoginPassed('". $loginName. "',". $this->InactivityMinutes. ", ". $this->levelRead. ");");
        }
        else
        {
           $this->jsp->AddClientScript("JsonDataBinding.LoginFailed('". $this->FailedMessageLableId. "','". $this->LoginFailedMessage. "');");
        }
	}
	public function ChangePassword($loginName, $currentPassword, $newPassword)
    {
		$this->jsMySql->SetDebug($this->jsp->DEBUG);
		$this->jsp->SetServerComponentName($this->myname);
        $this->jsp->LogDebugInfo("Changing password<br>");
        if (strlen($this->UserAccountTableName) != 0
            && strlen($this->UserAccountLoginFieldName) != 0
            && strlen($this->UserAccountPasswordFieldName) != 0)
        {
            if (strlen($loginName) == 0 || strlen($newPassword) == 0)
            {
                if (strlen($loginName) == 0)
                {
                    $this->jsp->LogDebugInfo("Missing login name<br>");
                }
                if (strlen($newPassword) == 0)
                {
                    $this->jsp->LogDebugInfo("Missing new password<br>");
                }
            }
            else
            {
				$info = "";
                $bOK = false;
                $bLoginFailed = false;
                $sSQL = "";
                if (strlen($currentPassword) == 0)
                {
                    $sSQL = "SELECT ".
                                    $this->NameDelimiterBegin.
                                    $this->UserAccountPasswordFieldName.
                                    $this->NameDelimiterEnd.
							" FROM ".
                                    $this->NameDelimiterBegin.
                                    $this->UserAccountTableName.
                                    $this->NameDelimiterEnd.							
							" WHERE ".
                                    $this->NameDelimiterBegin.
                                    $this->UserAccountLoginFieldName.
                                    $this->NameDelimiterEnd.
                             " = ?";
                    $this->jsp->LogDebugInfo("SQL:". $sSQL. "<br>");
                    $ps = array();
                    $p = new SqlClientParameter();
					$p->name = '@c1';
					$p->type = 's';
					$p->value = $loginName;
					$ps[] = $p;
					$tbl = new JsonDataTable();
					$this->jsMySql->GetData($tbl, $sSQL, $ps);
					$rn = count($tbl->Rows);
					if($rn > 0)
					{
						$s1 = $tbl->Rows[0]->ItemArray[0];
                        if (strlen($s1) == 0)
                        {
                            $bOK = true;
                        }
                        else
                        {
                            $info = $info. "Empty current password entered. Password in the database is not empty.<br>";
                        }
                    }
                    else
                    {
                            $info = $info. "Invalid user:[".
                                    $loginName.
                                    "]<br>";
                    }
                }
                else
                {
                     $bOK = $this->checkLogin($loginName, $currentPassword);
                     $bLoginFailed = !$bOK;
                }
                if ($bOK)
                {
                    $sSQL = "UPDATE ".
                               $this->NameDelimiterBegin.
                               $this->UserAccountTableName.
                               $this->NameDelimiterEnd.
                    " SET ".
                               $this->NameDelimiterBegin.
                               $this->UserAccountPasswordFieldName.
                               $this->NameDelimiterEnd.                    
                    " = PASSWORD(?) WHERE ".
                               $this->NameDelimiterBegin.
                               $this->UserAccountLoginFieldName.
                               $this->NameDelimiterEnd.                      
                    " = ?";
                    $this->jsp->LogDebugInfo("SQL:". $sSQL. "<br>");
                    $ps = array();
                    $p = new SqlClientParameter();
					$p->name = '@c1';
					$p->type = 's';
					$p->value = $newPassword;
					$ps[] = $p;  
					//       
                    $p = new SqlClientParameter();
					$p->name = '@c2';
					$p->type = 's';
					$p->value = $loginName;
					$ps[] = $p;  
					$this->jsMySql->ExecuteNonQuery($sSQL, $ps);					           
                    if (strlen($this->FailedMessageLableId) != 0)
                    {
                        $this->jsp->AddClientScript($this->FailedMessageLableId. ".innerHTML = 'Password changed';");
                    }
                }
                else
                {
                     $info = $info. "Invalid login or current password<br>";
                }
                $this->jsp->LogDebugInfo($info);
                if (!$bOK)
                {
                    if($bLoginFailed)
                    {
                        $this->jsp->AddClientScript("JsonDataBinding.LoginFailed('". $this->FailedMessageLableId. "','". $this->LoginFailedMessage. "');");
                    }
                    else
                    {
                        $this->jsp->AddClientScript($this->FailedMessageLableId. ".innerHTML = escape('". str_replace("'","\'", $info). "');");
                    }
                }
            }
        }
        else
        {
            $this->jsp->LogDebugInfo("Missing required properties. UserAccountTableName:". $this->UserAccountTableName. ", UserAccountLoginFieldName:". $this->UserAccountLoginFieldName. ", UserAccountPasswordFieldName:". $this->UserAccountPasswordFieldName);
        }
    }
}
?>