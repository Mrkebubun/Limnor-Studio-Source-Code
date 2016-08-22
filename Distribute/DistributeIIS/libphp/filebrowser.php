<?php

include_once 'fileutility.php';

class HtmlFileBrowser extends LimnorPhpFileUtilit
{
	function __construct($name)
	{
		parent::__construct($name);
	}
	public function loadFiles($path, $filetypes, $phpFolderName)
	{
		$this->DEBUG = $GLOBALS["debug"];
		$files = array();
		// '.' for current
		$dir = $this->getPhysicalPath($path, $phpFolderName);
		if ($this->DEBUG)
		{
			echo "start of listing files from ".$dir.":============= <br>";
		}
		$di = new DirectoryIterator($dir);
		foreach ($di as $file)
		{
			$fn = $file->getFilename();
			if ($this->DEBUG)
			{
				echo "item:".$fn." <br>";
			}
			if($file->isDot()) continue;
			$f = $dir.'/'.$fn;
			if(!is_dir($f))
			{
				$b = ($filetypes == null || strlen($filetypes) == 0);
				if(!$b)
				{
					$ext = strtolower(substr($fn, strrpos($fn, '.') + 1));
					if($filetypes == '.html')
					{
						if($ext == 'html' || $ext == 'htm')
						{
							$b = true;
						}
					}
					else if($filetypes == '.image')
					{
						if($ext == 'jpg' || $ext == 'png' || $ext == 'bmp' || $ext == 'gif')
						{
							$b = true;
						}
					}
					else if($filetypes == '.href')
					{
						if($ext == 'html' || $ext == 'htm')
						{
							$b = true;
						}
						else if($ext == 'jpg' || $ext == 'png' || $ext == 'bmp' || $ext == 'gif')
						{
							$b = true;
						}
						else if($ext == 'css' || $ext == 'js' || $ext == 'mp3' || $ext == 'mp4'|| $ext == 'swf')
						{
							$b = true;
						}
					}
					else
					{
						$el = explode(';',$filetypes);
						$b = in_array($ext, $el);
					}
				}
				if($b)
				{
					$files[] =  $fn;
				}
			}
		}
		if ($this->DEBUG)
		{
			echo "end of listing files===========<br>";
		}
		return $files;
	}
	public function loadFolders($path, $phpFolderName)
	{
		$this->DEBUG = $GLOBALS["debug"];
		$folders = array();
		$dir = $this->getPhysicalPath($path, $phpFolderName);
		if ($this->DEBUG)
		{
			echo "start of listing sub-folders:".$dir."============= <br>";
		}
		$di = new DirectoryIterator($dir);
		foreach ($di as $file)
		{
			if ($this->DEBUG)
			{
				echo "item:".$file->getFilename()." <br>";
			}
			if($file->isDot()) continue;
			$f = $dir.'/'.$file->getFilename();
			if(is_dir($f))
			{
				$folders[] =  $file->getFilename();
			}
		}
		if ($this->DEBUG)
		{
			echo "end of listing sub-folders===========<br>";
		}
		return $folders;
	}
	public function loadFolders_filebrowser($page,$phpFolderName, $serverComponentName, $value)
	{
		$this->DEBUG = $GLOBALS["debug"];
		$folders = $this->loadFolders($value, $phpFolderName);
		$page->AddDownloadValue('folders',$folders);
		$page->SetServerComponentName($serverComponentName);
	}
	public function loadFiles_filebrowser($page,$phpFolderName, $serverComponentName,$filetypes, $value)
	{
		$this->DEBUG = $GLOBALS["debug"];
		if ($this->DEBUG)
		{
			echo 'file types:'.$filetypes.'<br>';
		}
		$files = $this->loadFiles($value, $filetypes, $phpFolderName);
		$page->AddDownloadValue('filenames',$files);
		$page->SetServerComponentName($serverComponentName);
	}

}

?>