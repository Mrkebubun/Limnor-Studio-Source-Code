﻿<?php

include_once 'JsonProcessPage.php';
include_once 'FastJSON.class.php';
include_once 'jsonDataBind.php';

function CombinePath($p1, $p2)
{
	if(strlen($p1) == 0)
		return $p2;
	if(strlen($p2) == 0)
		return $p1;
	if($p2{0} == '/' || $p2{0} == '\\')
	{
		if($p1{strlen($p1)-1} == '/' || $p1{strlen($p1)-1} == '\\')
		{
			return $p1.substr($p2, 1);
		}
		else
		{
			return $p1.$p2;
		}
	}
	else
	{
		if($p1{strlen($p1)-1} == '/' || $p1{strlen($p1)-1} == '\\')
		{
			return $p1.$p2;
		}
		else
		{
			return $p1.'/'.$p2;
		}
	}
}
function url_exists($url)
{
	$file_headers = @get_headers($url);
	if($GLOBALS["debug"])
	{
		echo "header 0:".$file_headers[0]."<br>"; 
	}
	if($file_headers[0] == 'HTTP/1.1 404 Not Found') 
	{
		return false;
	}
	if($file_headers[0] == 'HTTP/1.0 404 Not Found') 
	{
		return false;
	}
	return true;
}

class LimnorPhpFileUtilit
{
	public $ObjectName;
	public $DEBUG;
	function __construct($name)
	{
		$this->ObjectName = $name;
	}
	//usually $phpFolderName is libphp
	public function getCallerFolder($phpFolderName)
	{
		$this->DEBUG = $GLOBALS["debug"];
		$dir = dirname(realpath(__FILE__));
		if ($this->DEBUG)
		{
			echo "base folder:".$dir." <br>";
		}
		$sl = strlen($phpFolderName);
		$dl = strlen($dir);
		if($sl > 0 && $dl >= $sl)
		{
			$s = substr($dir, $dl-$sl);
			if ($this->DEBUG)
			{
				echo "end part:".$s."<br>";
			}
			if(strcasecmp($s, $sl))
			{
				$dir = substr($dir, 0, $dl-$sl);
				if ($this->DEBUG)
				{
					echo "modified:".$dir."<br>";
				}
			}
		}
		return $dir;
	}
	//returns html folder combines with $path, without trailing directory-separator
	public function getPhysicalPath($path,$phpFolderName)
	{
		if(strlen($path) > 0)
		{
			if($path{0} == '/' || $path{0} == '\\')
			{
				$path = substr($path,1);
			}
			$dir = $this->getCallerFolder($phpFolderName).$path;
		}
		else
		{
			$dir = $this->getCallerFolder($phpFolderName);
		}
		if($dir{strlen($dir)-1} == '/' || $dir{strlen($dir)-1} == '\\')
		{
			$dir = substr($dir, 0, strlen($dir)-1);
		}
		return $dir;
	}
}

?>