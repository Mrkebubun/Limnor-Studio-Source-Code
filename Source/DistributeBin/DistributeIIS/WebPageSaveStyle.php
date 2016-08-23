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
	public function getStyleText()
	{
		if(strlen($this->name)>0 && strlen($this->value)>0)
		{
			if(substr($this->value, strlen($this->value)-1,1) == ";")
				return $this->name.":".$this->value;
			else
				return $this->name.":".$this->value.";";
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
		$props = array();
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
		if(strlen($this->cssText) == 0)
			$this->cssText = $text;
		else if(strlen($text) > 0)
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
					if($p2->name == $p0->name)
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

class WebPageSaveStyle extends JsonProcessPage
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
		else if($method == 'callMergeStyles') $this->callMergeStyles($value);
		else if($method == 'sccd45cfc') $this->sccd45cfc();
		else if($method == 'removeStyleRule') $this->removeStyleRule($value);
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
			echo "new contents:";
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
					echo "new css contents:";
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
			if(strlen($removedCss) > 0)
			{
				$this->RemovedCssRules = json_decode($removedCss);
				if($GLOBALS["debug"])
				{
					echo "removed css. selectors:";
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
			foreach($fileRules as $i2 => $r2)
			{
				if(!$r2->isEmpty())
				{
					$this->ServerFile2->AppendText($r2->getRuleText()."\r\n");
				}
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
		$dir = dirname(realpath(__FILE__));
		if($GLOBALS["debug"])
		{
			echo "saving ";
			echo $filename;
			echo " to folder ";
			echo $dir;
			echo "<br>";
		}
		$html = base64_decode($html);
		$css = base64_decode($css);
		//
		$pathHtml = CombinePath($dir,$filename);
		$pathCss = CombinePath($dir,substr($filename,0, strlen($filename)-4)."css");
		$pathCacheHtml = CombinePath($dir,$filename.".autoSave.html");
		$pathCacheCss = CombinePath($dir,$filename.".autoSave.css");
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
		}
	}
}
$w = new WebPageSaveStyle();
$w->ProcessClientRequest();
?>
