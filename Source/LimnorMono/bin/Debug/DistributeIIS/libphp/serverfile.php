<?php
class ServerFile
{
	public $Filename;
	public $handle;
	public $ForReading;
	public $ForWriting;
	public $FileOpenMode;//0:TruncateFile; 1: AppendFile; 2: CreateNewFile; 3: UseExistFile
	//
	public function GetFileMode()
	{
		$mode;
		if($this->ForReading)
		{
			if($this->ForWriting)
			{
				if($this->FileCreation == 0)
				{
					$mode = 'w+';
				}
				else if($this->FileCreation == 1)
				{
					$mode = 'a+';
				}
				else if($this->FileCreation == 2)
				{
					$mode = 'x+';
				}
				else if($this->FileCreation == 3)
				{
					$mode = 'c+';
				}
			}
			else
			{
				$mode = 'r';
			}
		}
		else
		{
			if($this->ForWriting)
			{
				if($this->FileCreation == 0)
				{
					$mode = 'w';
				}
				else if($this->FileCreation == 1)
				{
					$mode = 'a';
				}
				else if($this->FileCreation == 2)
				{
					$mode = 'x';
				}
				else if($this->FileCreation == 3)
				{
					$mode = 'c';
				}
			}
			else
			{
				$mode = 'r+';//???
			}
		}
		if($GLOBALS["debug"])
		{
			echo 'file open mode:'. $mode.'<br>';
		}
		return $mode;
	}
	//
	//
	public $exists;
	private $currentFilename;
	function __construct()
	{
		$this->Filename = null;
		$this->handle = null;
		$this->ForReading = false;
		$this->ForWriting = false;
		$this->currentFilename = null;
		$this->exists = false;
	}
	public function FileExists()
	{
		return file_exists($this->Filename);
	}
	public function Init()
	{
		if($this->handle != null)
			return;
		$m = $this->GetFileMode();
		//if($this->ForReading)
		//{
		//	$m = $m. 'r';
		//}
		//if($this->ForWriting)
		//{
		//	$m = $m. 'a+';
		//}
		if($this->Filename != null && strlen($this->Filename) > 0 && $m != '')
		{
			$this->exists = file_exists($this->Filename);
			if($this->exists || $this->ForWriting)
			{
				$this->handle = fopen($this->Filename, $m);
				$this->currentFilename = $this->Filename;
			}
			else
			{
				$this->handle = null;
				$this->currentFilename = null;
			}
		}
		else
		{
			$this->exists = false;
			$this->handle = null;
			$this->currentFilename = null;
		}
	}
	public function DeInit()
	{
		if($this->handle != null)
		{
			fclose($this->handle);
			$this->handle = null;
		}
	}
	public function verifyOpenFile()
	{
		if($this->currentFilename != $this->Filename || $this->handle == null)
		{
			$this->DeInit();
			$this->Init();
		}
		return ($this->handle != null);
	}
	public function AppendText($text)
	{
		if($this->verifyOpenFile())
		{
			fwrite($this->handle, $text); 
		}
	}
	public function AppendLine($text)
	{
		if($this->verifyOpenFile())
		{
			fwrite($this->handle, $text); 
			fwrite($this->handle, "\r\n"); 
		}
	}
	public function ReadAll()
	{
		if($this->verifyOpenFile())
		{
			return fread($this->handle, filesize($this->Filename));
		}
		return "";
	}
	public function DeleteFile($file)
	{
		if($GLOBALS["debug"])
		{
			echo 'deleting file:'. $file.'<br>';
		}
		if(file_exists($file))
		{
			return unlink($file);
		}
		else
		{
			return TRUE;
		}
	}
	public function CopyFile($sourceFile,$destinationFile)
	{
		return copy($sourceFile,$destinationFile);
	}
	public function Rename($oldName,$newName)
	{
		return rename($oldName,$newName);
	}
	public function GetFileNames($folderName)
	{
		$files = array();
		$names = scandir($folderName);
		foreach($names as $i => $v)
		{
			if(!is_dir($v))
			{
				$files[] = $v;
			}
		}
		return $files;
	}
	public function GetSubFolderNames($folderName)
	{
		$files = array();
		$names = scandir($folderName);
		foreach($names as $i => $v)
		{
			if(is_dir($v))
			{
				$files[] = $v;
			}
		}
		return $files;
	}
	public function CreateFolder($folderName)
	{
		if(!is_dir($folderName))
		{
			if(mkdir($folderName, 0700, true))
			{
				return true;
			}
			return false;
		}
		return true;
	}
	public function Combine($path1, $path2)
	{
		if(strlen($path1) > 0 && strlen($path2) > 0)
		{
			$p0 = substr($path1, strlen($path1)-1, 1);
			$p1 = substr($path2, 0, 1);
			if($p0 == '/' || $p0 == '\\')
			{
				if($p1 == '/' || $p1 == '\\')
				{
					return $path1.substr($path2,1);
				}
				else
				{
					return $path1.$path2;
				}
			}
			else
			{
				if($p1 == '/' || $p1 == '\\')
				{
					return $path1.$path2;
				}
				else
				{
					return $path1."/".$path2;
				}
			}
		}
	}
}
?>