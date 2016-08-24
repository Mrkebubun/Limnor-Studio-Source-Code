<?php

include_once 'libPhp/sqlClient.php';
include_once 'libPhp/dataSourceInterface.php';
include_once 'libPhp/JsonProcessPage.php';
include_once 'libPhp/jsonSource_mySqlI.php';
include_once 'libPhp/FastJSON.class.php';
include_once 'libPhp/jsonDataBind.php';
include_once 'libPhp/mySqlcredential.php';
include_once 'libPhp/WebApp.php';
include_once 'libPhp/serverfile.php';
include_once 'libPhp/fileutility.php';

class CssProperty
{
	public $name;
	public $value;
	public $subname;
	public $subvalue;
	function __construct()
	{
		$this->subname = '';
	}
	public function parseSubname()
	{
		$this->subname = '';
		if($this->name == 'background-image' || $this->name == 'background')
		{
			$pos = stripos($this->value, 'linear-gradient');
			if($pos != false)
			{
				$this->name = 'background-image';
				$pos = strpos($this->value, '(');
				if($pos != false)
				{
					$val = substr($this->value,$pos);
					$nm0 = trim(strtolower(substr($this->value, 0, $pos)));
					if(substr($val, strlen($val)-1,1) != ";")
						$val = $val.";";
					$this->subname = $nm0;
					$this->subvalue = $val;
				}
			}
		}
	}
	public function getStyleText()
	{
		if(strlen($this->name)>0 && strlen($this->value)>0)
		{
			$txt = '';
			if(substr($this->value, strlen($this->value)-1,1) == ";")
				$txt = $this->name.":".$this->value;
			else
				$txt = $this->name.":".$this->value.";";
			return $txt;
		}
		return "";
	}
}
class CssRule
{
	public $selector;
	public $cssText;
	function __construct()
	{
		$this->cssText = '';
		$this->selector = '';
	}
	public function getRuleText()
	{
		return $this->selector. " {". $this->cssText. "}"; 
	}
	public function isEmpty()
	{
		$v = trim($this->cssText);
		return (strlen($v) == 0);
	}
	public function parseProps($text)
	{
		$pr = false;
		$props = array();
		if(strlen($text) == 0)
		{
			return $props;
		}
		$name = strtok($text, ":");
		while($name !== false)
		{
			$value = strtok(":");
			if($GLOBALS["debug"])
			{
				echo "preprocess. name:";
				echo $name;
				echo "| value:";
				echo $value;
				echo "<br>";
			}
			if($value === false)
			{
				$value = "";
			}
			$pos = strrpos($value,";");
			if($GLOBALS["debug"])
			{
				echo "new name pos:";
				echo $pos;
				echo "<br>";
			}
			$newName = false;
			if($pos !== FALSE)
			{
				$newName = substr($value,$pos+1);
				$value = substr($value, 0, $pos);
				if($newName !== FALSE)
				{
					$newName = trim($newName);
					if(strlen($newName) == 0)
					{
						$newName = false;
					}
				}
			}
			$pr = new CssProperty();
			$pr->name = trim($name);
			$pr->value = trim($value);
			$pr->parseSubname();
			$props[] = $pr;
			if($GLOBALS["debug"])
			{
				echo "css name:";
				echo $pr->name;
				echo "<br>";
				echo "css value:";
				echo $pr->value;
				echo "<br>";
			}
			$name = $newName;
		}
		$pvs = array();
		foreach($props as $i => $p)
		{
			if($p->name == 'padding')
			{
				unset($props[$i]);
				$pvs = explode(' ', $p->value);
			}
		}
		if(count($pvs)>0)
		{
			$top = '0px';
			$bottom = '0px';
			$left = '0px';
			$right = '0px';
			foreach($pvs as $i => $s)
			{
				if(strlen(trim($s)) == 0)
				{
					unset($pvs[$i]);
				}
			}
			$pvs = array_values($pvs);
			if(count($pvs)>0)
			{
				$top = $pvs[0];
				$bottom = $pvs[0];
				$left = $pvs[0];
				$right = $pvs[0];
				if(count($pvs)>1)
				{
					$left = $pvs[1];
					$right = $pvs[1];
					if(count($pvs)>2)
					{
						$bottom = $pvs[2];
						if(count($pvs)>3)
						{
							$right = $pvs[3];
						}
					}
				}
				foreach($props as $i => $p)
				{
					if($p->name == 'padding-top' || $p->name == 'padding-bottom' || $p->name == 'padding-left' || $p->name == 'padding-right')
					{
						unset($props[$i]);
					}
				}
				$pr = new CssProperty();
				$pr->name = 'padding-top';
				$pr->value = $top;
				$props[] = $pr;
				$pr = new CssProperty();
				$pr->name = 'padding-bottom';
				$pr->value = $bottom;
				$props[] = $pr;
				$pr = new CssProperty();
				$pr->name = 'padding-left';
				$pr->value = $left;
				$props[] = $pr;
				$pr = new CssProperty();
				$pr->name = 'padding-right';
				$pr->value = $right;
				$props[] = $pr;
			}
		}
		////////////////////////////
		$pvs = array();
		foreach($props as $i => $p)
		{
			if($p->name == 'border-radius')
			{
				unset($props[$i]);
				$pvs = explode(' ', $p->value);
			}
		}
		if(count($pvs)>0)
		{
			$top = '0px';
			$bottom = '0px';
			$left = '0px';
			$right = '0px';
			foreach($pvs as $i => $s)
			{
				if(strlen(trim($s)) == 0)
				{
					unset($pvs[$i]);
				}
			}
			$pvs = array_values($pvs);
			if(count($pvs)>0)
			{
				$top = $pvs[0];
				$bottom = $pvs[0];
				$left = $pvs[0];
				$right = $pvs[0];
				if(count($pvs)>1)
				{
					$left = $pvs[1];
					$right = $pvs[1];
					if(count($pvs)>2)
					{
						$bottom = $pvs[2];
						if(count($pvs)>3)
						{
							$right = $pvs[3];
						}
					}
				}
			}
		}
		if(count($pvs)>0)
		{
			foreach($props as $i => $p)
			{
				if($p->name == 'border-top-left-radius' || $p->name == 'border-top-right-radius' || $p->name == 'border-bottom-right-radius' || $p->name == 'border-bottom-left-radius')
				{
					unset($props[$i]);
				}
			}
			$pr = new CssProperty();
			$pr->name = 'border-top-left-radius';
			$pr->value = $top;
			$props[] = $pr;
			$pr = new CssProperty();
			$pr->name = 'border-top-right-radius';
			$pr->value = $left;
			$props[] = $pr;
			$pr = new CssProperty();
			$pr->name = 'border-bottom-right-radius';
			$pr->value = $bottom;
			$props[] = $pr;
			$pr = new CssProperty();
			$pr->name = 'border-bottom-left-radius';
			$pr->value = $right;
			$props[] = $pr;
		}
		////////////////////////////
		return $props;
	}
	public function formCssText($props)
	{
		$ss = array();
		foreach($props as $i => $p)
		{
			$ss[] = $p->getStyleText();
		}
		return implode(" ",$ss);
	}
	public function mergeText($text)
	{
		if(strlen($text) > 0)
		{
			if($GLOBALS["debug"])
			{
				echo "existing css:";
				echo $this->cssText;
				echo "<br>";
				echo "new css:";
				echo $text;
				echo "<br>";
			}
			$props0 = $this->parseProps($this->cssText);
			$props2 = $this->parseProps($text);
			if($GLOBALS["debug"])
			{
				echo "existing properties:";
				echo count($props0);
				echo "<br>";
				echo "new properties:";
				echo count($props2);
				echo "<br>";
			}
			foreach($props2 as $i2 => $p2)
			{
				$found = 0;
				foreach($props0 as $i0 => $p0)
				{
					if($p2->name == $p0->name && $p2->subname == $p0->subname)
					{
						$found = 1;
						$p0->value = $p2->value;
						break;
					}
				}
				if($found == 0)
				{
					$props0[] = $p2;
				}
			}
			$this->cssText = $this->formCssText($props0);
		}
	}
	public function removeProperties($propNames)
	{
		$changed = FALSE;
		$props0 = $this->parseProps($this->cssText);
		foreach($propNames as $i => $s)
		{
			if($GLOBALS["debug"])
			{
				echo "   prop to be removed:";
				echo $s;
				echo "<br>";
			}
			foreach($props0 as $key => $prop)
			{
				if($prop->name == $s)
				{
					if($GLOBALS["debug"])
					{
						echo " prop key:";
						echo $key;
						echo "<br>";
					}
					unset($props0[$key]);
					$changed = true;
				}
			}
		}
		if($changed)
		{
			$this->cssText = $this->formCssText($props0);
			if($GLOBALS["debug"])
			{
				echo "css after removing properties:";
				echo $this->cssText;
				echo "<br>";
			}
		}
	}
}

class WebPageUpdater extends JsonProcessPage
{
	private $ServerFile1;
	private $ServerFile2;
	public $RemovedCssRules;
	function __construct()
	{
		parent::__construct();
		$this->ServerFile1=new ServerFile();
		$this->ServerFile2=new ServerFile();
		$this->ServerFile1->FileOpenMode=3;
		$this->ServerFile1->ForReading=true;
		$this->ServerFile1->ForWriting=false;
		$this->ServerFile2->FileOpenMode=0;
		$this->ServerFile2->ForReading=false;
		$this->ServerFile2->ForWriting=true;
	}
	protected function OnRequestStart()
	{
		if ($this->DEBUG)
		{
			echo "PHP processor:". __FILE__."<br>";
		}
		$this->ServerFile1->Init();
		$this->ServerFile2->Init();
	}
	protected function OnRequestClientData()
	{
	}
	protected function OnRequestFinish()
	{
		$this->ServerFile1->DeInit();
		$this->ServerFile2->DeInit();
	}
	protected function OnRequestGetData($value)
	{
	}
	protected function OnRequestPutData($value)
	{
	}
	protected function OnRequestExecution($method, $value)
	{
		if($method == 'callSaveHtmlPage') $this->callSaveHtmlPage($value);
		else if($method == 'removeCacheFiles') $this->removeCacheFiles($value);
		else if($method == 'callMergeStyles') $this->callMergeStyles($value);
		else if($method == 'sccd45cfc') $this->sccd45cfc();
		else if($method == 'removeStyleRule') $this->removeStyleRule($value);
		else if($method == 'checkHtmlCacheFileExist') $this->checkHtmlCacheFileExist($value);
	}
	function checkFileExist($value)
	{
		$dir = dirname(realpath(__FILE__));
		if ($this->DEBUG)
		{
			echo "Check file exist of ".$value." from ".$dir."<br>";
		}
		$fn = CombinePath($dir,$value);
		if ($this->DEBUG)
		{
			echo "target file path:".$fn." <br>";
		}
		try
		{
			$fh = file_exists($fn);
			$this->AddDownloadValue('fileExists',$fh);
		}
		catch(Exception $e)
		{
			$errMsg = "Error checking file exist ".$fn.". ".$e->getMessage();
			$this->AddDownloadValue('serverFailure',$errMsg);
			if ($this->DEBUG)
			{
				echo $errMsg."<br>";
			}
		}
	}
	function checkHtmlCacheFileExist($value)
	{
		$autosave = $value.'.autoSave.html';
		$dir = dirname(realpath(__FILE__));
		$fn = CombinePath($dir,$value);
		if(!file_exists($fn))
		{
			if ($this->DEBUG)
			{
				echo "Creating file:".$fn."<br>";
			}
			$h = fopen($fn, "a");
			if(property_exists($this->jsonFromClient->values,'lang') && strlen($this->jsonFromClient->values->lang) > 0)
			{
				fwrite($h,'<html lang="'.$this->jsonFromClient->values->lang.'"></html>');
			}
			else
			{
				fwrite($h,' ');
			}
			fclose($h);
		}
		$this->checkFileExist($autosave);
	}
	public function removeStyleRule($file)
	{
		if ($this->DEBUG)
		{
			echo 'removing css rule from file:'.$file.'<br>';
			echo 'css rule:'.$this->jsonFromClient->values->selector.'<br>';
		}
		$PhpString15b03cc7b='';
		$PhpArray144c2f387=array();
		$this->ServerFile1->Filename=$file;
		$this->ServerFile2->Filename=$file;
		if($this->ServerFile1->FileExists()) 
		{
			$PhpString15b03cc7b=$this->ServerFile1->ReadAll();
			$PhpArray144c2f387=explode("\r\n",$PhpString15b03cc7b);
			removeStringArrayElement($PhpArray144c2f387,$this->jsonFromClient->values->selector.'{',False,0);
			$PhpString15b03cc7b=implode("\r\n",$PhpArray144c2f387);
			$this->ServerFile1->DeInit();
			$this->ServerFile2->AppendText($PhpString15b03cc7b);
		}
	}
	public function saveStyle($cssFilename,$cssName,$styleText) 
	{
		$PhpString15b03cc7b='';
		$PhpArray144c2f387=array();
		$this->ServerFile1->Filename=$cssFilename;
		$this->ServerFile2->Filename=$cssFilename;
		if($this->ServerFile1->FileExists()) 
		{
			$PhpString15b03cc7b=$this->ServerFile1->ReadAll();
			$PhpArray144c2f387=explode("\r\n",$PhpString15b03cc7b);
			removeStringArrayElement($PhpArray144c2f387,$cssName.'{',False,0);
			$PhpString15b03cc7b=implode("\r\n",$PhpArray144c2f387);
			if(count($PhpArray144c2f387) > 0) 
			{
				$PhpString15b03cc7b = $PhpString15b03cc7b. "\r\n";
			}
		}
		else
		{
			$PhpString15b03cc7b='';
		}
		$PhpString15b03cc7b = $PhpString15b03cc7b. $cssName.$styleText;
		$this->ServerFile1->DeInit();
		$this->ServerFile2->AppendText($PhpString15b03cc7b);
	}
	function sccd45cfc()
	{
		if((is_null($this->jsonFromClient->values->jc2091656) || (is_string($this->jsonFromClient->values->jc2091656) && strlen($this->jsonFromClient->values->jc2091656) == 0)))
		{
			$vf538badc_a6a49aeb=$this->jsonFromClient->values->j6ba0918d;
		}
		else
		{
			$vf538badc_a6a49aeb=$this->jsonFromClient->values->j6ba0918d.'.'.$this->jsonFromClient->values->jc2091656;
		}
		$this->saveStyle($this->jsonFromClient->values->jba5d769b,$vf538badc_a6a49aeb,$this->jsonFromClient->values->jea4fef2);
	}
	private function parseCssText($csstext)
	{
		$rules = array();
		str_replace("\r", " ", $csstext);
		str_replace("\n", " ", $csstext);
		str_replace("\t", " ", $csstext);
		$csstext = trim($csstext);
		$selector = strtok($csstext, "{}");
		while($selector !== false)
		{
			$value = strtok("{}");
			if($value === false)
				break;
			$r = new CssRule();
			$r->selector = strtolower(trim($selector));
			$r->cssText = trim($value);
			if($GLOBALS["debug"])
			{
				echo "selector:";
				echo $r->selector;
				echo "| css text:";
				echo $r->cssText;
				echo "<br>";
			}
			$rules[] = $r;
			$selector = strtok("{}");
		}
		return $rules;
	}
	public function callMergeStyles($cssFile)
	{
		if($GLOBALS["debug"])
		{
			echo "merge styles<br>";
			echo "file:";
			echo $cssFile;
			echo "<br>";
			echo "text:";
			echo $this->jsonFromClient->values->styleText;
			echo "<br>";
		}
		$this->mergeStyles($cssFile,$this->jsonFromClient->values->styleText);
	}
	public function mergeStyles($cssFilename, $styleText) 
	{
		$this->ServerFile1->Filename=$cssFilename;
		$this->ServerFile2->Filename=$cssFilename;
		$removedCss = '';
		if($GLOBALS["debug"])
		{
			echo "new css contents:";
			echo $styleText;
			echo "<br>";
		}
		$pos = strpos($styleText,'$$$');
		if($pos !== FALSE)
		{
			if($pos >= 0)
			{
				$removedCss = substr($styleText, 0, $pos);
				$styleText = substr($styleText, $pos+3);
				if($GLOBALS["debug"])
				{
					echo "new styles contents:";
					echo $styleText;
					echo "<br> removed:";
					echo $removedCss;
					echo "<br>";
				}
			}
		}
		if($this->ServerFile1->FileExists()) 
		{
			$cssText = $this->ServerFile1->ReadAll();
			if($GLOBALS["debug"])
			{
				echo "file contents:";
				echo $cssText;
				echo "<br>";
			}
			$fileRules = $this->parseCssText($cssText);
			
			$newRules = $this->parseCssText($styleText);
			if($GLOBALS["debug"])
			{
				echo "number of existing rules:";
				echo count($fileRules);
				echo "<br>";
				echo "number of new rules:";
				echo count($newRules);
				echo "<br>";
			}
			foreach($newRules as $idx => $r0)
			{
				$found = false;
				foreach($fileRules as $i2 => $r2)
				{
					if($r0->selector == $r2->selector)
					{
						$found = true;
						$r2->mergeText($r0->cssText);
						break;
					}
				}
				if(!$found)
				{
					$fileRules[] = $r0;
				}
			}
			//
			if(strlen($removedCss) > 0)
			{
				$this->RemovedCssRules = json_decode($removedCss);
				if($GLOBALS["debug"])
				{
					echo "removed css selectors:";
					echo count($this->RemovedCssRules);
					echo "<br>";
				}
				foreach($this->RemovedCssRules as $ridx => $rl)
				{
					if($GLOBALS["debug"])
					{
						echo "  selector:";
						echo $rl->selector;
						echo "<br>";
					}
					$rule = FALSE;
					foreach($fileRules as $fi => $fr)
					{
						if($fr->selector == $rl->selector)
						{
							if(property_exists($rl,'all') && $rl->all)
							{
								if($GLOBALS["debug"])
								{
									echo " removing selector:";
									echo $fr->selector;
									echo "| css:";
									echo $fr->cssText;
									echo "<br>";
								}
								$fr->cssText = '';
							}
							else
							{
								$rule = $fr;
							}
							break;
						}
					}
					if($rule !== FALSE)
					{
						$rule->removeProperties($rl->props);
					}
				}
			}
			//
			$fileEmpty = true;
			foreach($fileRules as $i2 => $r2)
			{
				if(!$r2->isEmpty())
				{
					$this->ServerFile2->AppendText($r2->getRuleText()."\r\n");
					$fileEmpty = false;
				}
			}
			if($fileEmpty)
			{
				$this->ServerFile2->AppendText("\r\n");
			}
		}
		else
		{
			$this->ServerFile2->AppendText($styleText);
		}
		$this->ServerFile1->DeInit();
		$this->ServerFile2->DeInit();
	}
	public function callSaveHtmlPage($value)
	{
		$this->saveHtmlPage($value,$this->jsonFromClient->values->publish,$this->jsonFromClient->values->html,$this->jsonFromClient->values->css);
	}
	public function saveHtmlPage($filename, $publish, $html, $css)
	{
    if(property_exists($this->jsonFromClient->values,'saveToFolder'))
    {
      if($GLOBALS["debug"])
		  {
        echo "Use specified target folder<br>";
      }
      $dir = $this->jsonFromClient->values->saveToFolder;
    }
    else
    {
      if($GLOBALS["debug"])
		  {
        echo "Resolving target folder<br>";
      }
		  $dir = dirname(realpath(__FILE__));
    }
		if($GLOBALS["debug"])
		{
			echo "saving ";
			echo $filename;
			echo " to folder ";
			echo $dir;
			echo "<br>";
		}
		$html = base64_decode($html);
		if(strlen($css)>0)
		{
			$css = base64_decode($css);
		}
		//
		$pageCssName = substr($filename,0, strlen($filename)-4)."css";
		$pageCssCacheName = $filename.".autoSave.css";
		//
		$pathHtml = CombinePath($dir,$filename);
		$pathCss = CombinePath($dir,$pageCssName);
		$pathCacheHtml = CombinePath($dir,$filename.".autoSave.html");
		$pathCacheCss = CombinePath($dir,$pageCssCacheName);
		//
		$pageCssName0 = $pageCssName;
		$pageCssCacheName0 = $pageCssCacheName;
		$pos = strrpos($filename, "/");
		if($pos !== false)
		{
			$f0 = substr($filename, $pos + 1);
			if($GLOBALS["debug"])
			{
				echo "File name:";
				echo $f0;
				echo "<br>";
			}
			$pageCssName0 = substr($f0,0, strlen($f0)-4)."css";
			$pageCssCacheName0 = $f0.".autoSave.css";
		}
		//
		if($GLOBALS["debug"])
		{
			echo "page path: ";
			echo $pathHtml;
			echo "<br>";
			
			echo "css path: ";
			echo $pathCss;
			echo "<br>";
			
			echo "cache path: ";
			echo $pathCacheHtml;
			echo "<br>";
			
			echo "cache css path: ";
			echo $pathCacheCss;
			echo "<br>";
			
			echo "page css name:";
			echo $pageCssName0;
			echo "<br>";
			
			echo "cache css name:";
			echo $pageCssCacheName0;
			echo "<br>";
		}
		if($publish)
		{
			if(strlen($css)>0)
			{
				if(file_exists($pathCacheCss))
				{
					$this->mergeStyles($pathCacheCss, $css);
					if($GLOBALS["debug"])
					{
						echo "Updated css cache<br>";
					}
					if(copy($pathCacheCss,$pathCss))
					{
						if($GLOBALS["debug"])
						{
							echo "copied cached css<br>";
						}
					}
					else
					{
						$errMsg = "Error copying css cache file";
						$this->AddDownloadValue('serverFailure', $errMsg);
						if($GLOBALS["debug"])
						{
							echo $errMsg. "<br>";
						}
						return;
					}
				}
				else
				{
					$this->mergeStyles($pathCss, $css);
					if($GLOBALS["debug"])
					{
						echo "Updated css<br>";
					}
				}
			}
			else
			{
				//no new css
				if(file_exists($pathCacheCss))
				{
					if(copy($pathCacheCss,$pathCss))
					{
						if($GLOBALS["debug"])
						{
							echo "copied cached css<br>";
						}
					}
					else
					{
						$errMsg = "Error copying css cache file (case 2)";
						$this->AddDownloadValue('serverFailure', $errMsg);
						if($GLOBALS["debug"])
						{
							echo $errMsg. "<br>";
						}
						return;
					}
				}
			}
			$pos = stripos($html,$pageCssCacheName0);
			if($pos === false)
      {
        $pos = stripos($html,$pageCssName0);
        if($pos !== false)
        {
          if($GLOBALS["debug"])
				  {
					  echo "refreshing css file ".$pageCssName0."<br>";
				  }
          $pos2=strpos($html,"'",$pos+1);
          if($pos2 === false)
          {
            $pos2=strpos($html,'"',$pos+1);
          }
          else
          {
            $pos3=strpos($html,'"',$pos+1);
            if($pos3 !== false && $pos3 < $pos2)
            {
              $pos2 = $pos3;
            }
          }
          if($pos2 !== false)
          {
            $html = substr($html, 0, $pos).$pageCssName0."?lr=".rand().substr($html, $pos2);
          }
          else 
          {
            if($GLOBALS["debug"])
				    {
					    echo "Error refreshing css file ".$pageCssName0.". ending quote not found.<br>";
				    }
          }
        }
      }
      else
			{
        $pos2=strpos($html,"'",$pos+1);
        if($pos2 === false)
        {
          $pos2=strpos($html,'"',$pos+1);
        }
        else
        {
          $pos3=strpos($html,'"',$pos+1);
          if($pos3 !== false && $pos3 < $pos2)
          {
            $pos2 = $pos3;
          }
        }
        if($pos2 === false)
        {
				  $html = substr($html, 0, $pos).$pageCssName0.substr($html, $pos+strlen($pageCssCacheName0));
        }
        else
        {
          $html = substr($html, 0, $pos).$pageCssName0."?lr=".rand().substr($html, $pos2);
        }
			}
			$fh = fopen($pathHtml, 'w');
			if($fh === false)
			{
				$errMsg = 'Cannot open file for writing:'.$pathHtml;
				$this->AddDownloadValue('serverFailure',$errMsg);
				if($GLOBALS["debug"])
				{
					echo $errMsg. "<br>";
				}
			}
			else
			{
				try
				{
					fwrite($fh, $html);
					fclose($fh);
					if($GLOBALS["debug"])
					{
						echo "Updated html<br>";
					}
				}
				catch(Exception $e)
				{
					$errMsg = "Error updating file ".$pathHtml.". ".$e->getMessage();
					$this->AddDownloadValue('serverFailure',$errMsg);
					if($GLOBALS["debug"])
					{
						echo $errMsg. "<br>";
					}
					return;
				}
			}
			if(file_exists($pathCacheCss))
			{
				if(unlink($pathCacheCss))
				{
					if($GLOBALS["debug"])
					{
						echo "cleared css cache<br>";
					}
				}
				else
				{
					$errMsg = "Error removing css cache file";
					$this->AddDownloadValue('serverFailure', $errMsg);
					if($GLOBALS["debug"])
					{
						echo $errMsg. "<br>";
					}
				}
			}
			if(file_exists($pathCacheHtml))
			{
				if(unlink($pathCacheHtml))
				{
					if($GLOBALS["debug"])
					{
						echo "cleared html cache<br>";
					}
				}
				else
				{
					$errMsg = "Error removing html cache file";
					$this->AddDownloadValue('serverFailure', $errMsg);
					if($GLOBALS["debug"])
					{
						echo $errMsg. "<br>";
					}
				}
			}
		}
		else
		{
			//writing cache
			if(!file_exists($pathCacheCss))
			{
				if(file_exists($pathCss))
				{
					if(copy($pathCss,$pathCacheCss))
					{
						if($GLOBALS["debug"])
						{
							echo "copied original css to cache<br>";
						}
					}
					else
					{
						$errMsg = "Error making copy of css file";
						$this->AddDownloadValue('serverFailure', $errMsg);
						if($GLOBALS["debug"])
						{
							echo $errMsg. "<br>";
						}
						return;
					}
				}
			}
			if(strlen($css)>0)
			{
				$this->mergeStyles($pathCacheCss, $css);
				if($GLOBALS["debug"])
				{
					echo "Updated css cache<br>";
				}
			}
			$pos = stripos($html,$pageCssName0);
			if($pos === false)
      {
        $pos = stripos($html,$pageCssCacheName0);
        if($pos !== false)
        {
          if($GLOBALS["debug"])
				  {
					  echo "refreshing css file ".$pageCssCacheName0."<br>";
				  }
          $pos2=strpos($html,"'",$pos+1);
          if($pos2 === false)
          {
            $pos2=strpos($html,'"',$pos+1);
          }
          else
          {
            $pos3=strpos($html,'"',$pos+1);
            if($pos3 !== false && $pos3 < $pos2)
            {
              $pos2 = $pos3;
            }
          }
          if($pos2 !== false)
          {
            $html = substr($html, 0, $pos).$pageCssCacheName0."?lr=".rand().substr($html, $pos2);
          }
          else 
          {
            if($GLOBALS["debug"])
				    {
					    echo "Error refreshing css file ".$pageCssCacheName0.". ending quote not found.<br>";
				    }
          }
        }
      }
      else
			{
				if($GLOBALS["debug"])
				{
					echo "switching css file name to ".$pageCssCacheName0."<br>";
				}
        $pos2=strpos($html,"'",$pos+1);
        if($pos2 === false)
        {
          $pos2=strpos($html,'"',$pos+1);
        }
        else
        {
          $pos3=strpos($html,'"',$pos+1);
          if($pos3 !== false && $pos3 < $pos2)
          {
            $pos2 = $pos3;
          }
        }
        if($pos2 === false)
        {
				  $html = substr($html, 0, $pos).$pageCssCacheName0.substr($html, $pos+strlen($pageCssName0));
        }
        else
        {
          $html = substr($html, 0, $pos).$pageCssCacheName0."?lr=".rand().substr($html, $pos2);
        }
			}
			$fh = fopen($pathCacheHtml, 'w');
			if($fh === false)
			{
				$errMsg = 'Cannot open file for writing:'.$pathCacheHtml;
				$this->AddDownloadValue('serverFailure',$errMsg);
				if($GLOBALS["debug"])
				{
					echo $errMsg. "<br>";
				}
			}
			else
			{
				try
				{
					fwrite($fh, $html);
					fclose($fh);
					if($GLOBALS["debug"])
					{
						echo "Updated html cache<br>";
					}
				}
				catch(Exception $e)
				{
					$errMsg = "Error updating file ".$pathCacheHtml.". ".$e->getMessage();
					$this->AddDownloadValue('serverFailure',$errMsg);
					if($GLOBALS["debug"])
					{
						echo $errMsg. "<br>";
					}
					return;
				}
			}
		}
	}
	public function removeCacheFiles($cacheHtmlFile)
	{
		$cacheCssName = substr($cacheHtmlFile,0, strlen($cacheHtmlFile)-4)."css";
		$dir = dirname(realpath(__FILE__));
		if($GLOBALS["debug"])
		{
			echo "removing cache files from folder ";
			echo $dir;
			echo "<br>";
		}
		$pathCacheHtml = CombinePath($dir,$cacheHtmlFile);
		$pathCacheCss = CombinePath($dir,$cacheCssName);
		if(file_exists($pathCacheCss))
		{
			if(unlink($pathCacheCss))
			{
				if($GLOBALS["debug"])
				{
					echo "cleared css cache<br>";
				}
			}
			else
			{
				$errMsg = "Error removing css cache file ".$pathCacheCss;
				$this->AddDownloadValue('serverFailure', $errMsg);
				if($GLOBALS["debug"])
				{
					echo $errMsg. "<br>";
				}
			}
		}
		if(file_exists($pathCacheHtml))
		{
			if(unlink($pathCacheHtml))
			{
				if($GLOBALS["debug"])
				{
					echo "cleared html cache<br>";
				}
			}
			else
			{
				$errMsg = "Error removing html cache file ".$pathCacheHtml;
				$this->AddDownloadValue('serverFailure', $errMsg);
				if($GLOBALS["debug"])
				{
					echo $errMsg. "<br>";
				}
			}
		}
	}
}
$w = new WebPageUpdater();
$w->ProcessClientRequest();
?>
