<?php

class HtmlFileUpload
{
	public $Filename;
	public $FileSize;
	public $FileType;
	public $FilePathOnServer;
	public $FileError;
	public $FileErrorText;
	public $FilePathSaved;
	public $ObjectName;
	public $FileUploaded;
	public $MaximumFileSize;
	
	function __construct($name)
	{
		$this->ObjectName = $name;
		$this->FileUploaded = false;
		$this->MaximumFileSize = 0;
		if(isset($_FILES))
		{
			if(count($_FILES))
			{
				$this->Filename = $_FILES[$name]['name'];
				$this->FileSize = $_FILES[$name]['size'];
				$this->FileType = $_FILES[$name]['type'];
				$this->FilePathOnServer = $_FILES[$name]['tmp_name'];
				$this->FileError = $_FILES[$name]['error'];
				$this->FileUploaded = true;
			}
		}
		if(!$this->FileUploaded)
		{
			$this->Filename = "";
			$this->FileSize = 0;
			$this->FileType = "";
			$this->FilePathOnServer = "";
			$this->FileError = 0;
		}
		$this->FilePathSaved = "";
	}
	public function SaveToFile($folder, $filename)
	{
		return $this->saveToTarget($folder, $filename);
	}
	public function SaveToFolder($folder)
	{
		return $this->saveToTarget($folder, null);
	}
	public function saveToTarget($folder, $targetName)
	{
		if($GLOBALS["debug"])
		{
			echo "File Uploaded:". $this->FileUploaded. "<br>";
			echo "Uploader:". $this->ObjectName. "<br>";
			echo "Filename:". $this->Filename. "<br>";
			echo "FileSize:". $this->FileSize. "<br>";
			echo "FileType:". $this->FileType. "<br>";
			echo "FileError:". $this->FileError. "<br>";
			echo "FilePathOnServer:". $this->FilePathOnServer. "<br>";
			if($this->FileError == 0)
				echo "FileError:no error<br>";
			else if($this->FileError == 1)
				echo "FileError:The uploaded file exceeds the upload_max_filesize directive in php.ini.<br>";
			else if($this->FileError == 2)
				echo "FileError:The uploaded file exceeds the MAX_FILE_SIZE directive that was specified in the HTML form. <br>";
			else if($this->FileError == 3)
				echo "FileError:The uploaded file was only partially uploaded. <br>";
			else if($this->FileError == 4)
				echo "FileError:No file was uploaded. <br>";
			else if($this->FileError == 5)
				echo "FileError:error number 5<br>";
			else if($this->FileError == 6)
				echo "FileError:Missing a temporary folder. Introduced in PHP 4.3.10 and PHP 5.0.3. <br>";
			else if($this->FileError == 7)
				echo "FileError:Failed to write file to disk.<br>";
			else if($this->FileError == 8)
				echo "FileError:A PHP extension stopped the file upload. PHP does not provide a way to ascertain which extension caused the file upload to stop; examining the list of loaded extensions with phpinfo() may help.<br>";
			else
				echo "FileError:error number is ". $this->FileError. "<br>";
			if(file_exists($this->FilePathOnServer))
				echo "File uploaded on server:true<br>";
			else
				echo "File uploaded on server:false<br>";
			echo "Target file name:";
			if($targetName != null)
			{
				echo $targetName;
			}
			echo "<br>";
			echo "File size limit:".$this->MaximumFileSize."<br>";
		}
		if($this->FileError == 2 || ($this->MaximumFileSize > 0 && $this->MaximumFileSize < $this->FileSize))
		{
			$this->FileErrorText = 'File size of ['. $this->Filename. '] is over the limit. File size:'. $this->FileSize. '. Limit:'. $this->MaximumFileSize;
			if($GLOBALS["debug"])
			{
				echo $this->FileErrorText. "<br>";
			}
			return false;
		}
		try 
		{
			if($targetName == null)
			{
				$targetName = $this->Filename;
			}
			if($this->FileError == 0)
			{
				if(!$this->FileUploaded)
				{
					if($GLOBALS["debug"])
					{
						echo "No file selected. <br>";
					}
					return true;
				}
				if($folder == null)
				{
					$uploadfile = $targetName;
				}
				else
				{
					if(strlen($folder) == 0 || $folder == "/" || $folder == "\\")
					{
						$uploadfile = $targetName;
					}
					else
					{
						if($folder{0} == '/' || $folder{0} == '\\')
						{
							$folder = substr($folder, 1);
						}
						if($folder{strlen($folder)-1} == "/" || $folder{strlen($folder)-1} == "\\")
						{
							$uploadfile = $folder. $targetName;
						}
						else
						{
							$uploadfile = $folder. "/". $targetName;
						}
					}
				}
				if($GLOBALS["debug"])
				{
					echo "saving ". $this->FilePathOnServer. " to ". $uploadfile. "<br>";
				}
				$folderOK = true;
				if(!is_dir($folder))
				{
					if(!mkdir($folder,0700,true))
					{
						$this->FileErrorText = "Error creating folder.";
						if($GLOBALS["debug"])
						{
							echo "FileErrorText:". $this->FileErrorText. "<br>";
						}
						$folderOK = false;
					}
				}
				if($folderOK)
				{
					if (move_uploaded_file($this->FilePathOnServer, $uploadfile)) 
					{
						$this->FilePathSaved = $uploadfile;
						if($GLOBALS["debug"])
						{
							echo "OK saving file.<br>";
						}
						return true;
					}
					else
					{
						$this->FileErrorText = 'Error moving files on the web server. ['. $this->Filename. '] to ['.$targetName.']. Error message:'.$_FILES[$this->Filename]['error'];
						if($GLOBALS["debug"])
						{
							echo "FileErrorText:". $this->FileErrorText. "<br>";
						}
					}
				}
			}
			else 
			{
				if($this->FileError == 4)
					return true;
				else
					$this->FileErrorText = 'File upload error code:'.$this->FileError;
			}
		}
		catch (Exception $e) 
		{
			$this->FileErrorText = $e->getMessage(). '['. $this->Filename. ']. ';
			if($GLOBALS["debug"])
			{
				echo "File upload exception. FileErrorText:". $this->FileErrorText. "<br>";
			}
		}
		return false;
	}
}
class HtmlFileUploadItem extends HtmlFileUpload
{
	function __construct($name, $fn, $fs, $ft, $fp, $fe)
	{
        	$this->ObjectName = $name;
	        $this->Filename = $fn;
	        $this->FileSize = $fs;
	        $this->FileType = $ft;
	        $this->FilePathOnServer = $fp;
	        $this->FileError = $fe;
        	$this->FilePathSaved = "";
        	$this->FileUploaded = ($fe == 0);
	}
}
class HtmlFileUploadGroup
{
	public $FileUploaders;
	public $FileUploaderCount;
	public $SavedFilePaths;
	public $ObjectName;
	public $Group;
	function __construct($name) 
	{
		$this->ObjectName = $name;
		$this->FileUploaders = array();
		$this->FileUploaderCount = 0;
		$this->SavedFilePaths = array();
		$this->Group = array();
		if( !empty($_FILES[$name]))
		{
			$sz = count($_FILES[$name]['name']);
			for( $i = 0; $i < $sz; $i++)
			{
				$this->Group[] = new HtmlFileUploadItem($name. "_". $i, $_FILES[$name]['name'][$i],$_FILES[$name]['size'][$i],$_FILES[$name]['type'][$i],$_FILES[$name]['tmp_name'][$i],$_FILES[$name]['error'][$i]);
			}
		}
	}
	public function AddFileUploader($name, $loader)
	{
		$this->FileUploaders[$name] = $loader;
		$this->FileUploaderCount = count($this->FileUploaders);
	}
	public function SaveToFolder($folder)
	{
		$ret = true;
		$this->SavedFilePaths = array();
		if($GLOBALS["debug"])
		{
			echo "Call SaveToFolder by HtmlFileUploadGroup. <br>";
			echo "Static File count:". $this->FileUploaderCount. "<br>";
			echo "Dynamic File count:". count($this->Group). "<br>";
		}
		foreach ($this->FileUploaders as $name => $loader)
		{
			if($loader->SaveToFolder($folder))
			{
				$this->SavedFilePaths[] = $loader->FilePathSaved;
			}
			else
			{
				$ret = false;
			}
		}
		foreach($this->Group as $i => $f)
		{
			if($f->SaveToFolder($folder))
			{
				$this->SavedFilePaths[] = $f->FilePathSaved;
			}
			else
			{
				$ret = false;
			}
		}
		return $ret;
	}
}

class HtmlFilesSelector extends HtmlFileUploadGroup
{
	public $UploadedFilePaths;
	function __construct($name) 
	{
		parent::__construct($name);
		$this->UploadedFilePaths = array();
		if( !empty($this->Group))
		{
			foreach($this->Group as $i => $f)
			{
				$this->UploadedFilePaths[] = $f->FilePathOnServer;
			}
		}
	}
}
?>