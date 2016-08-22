<?php
header('Content-Type: text/html; charset=utf-8');

include_once 'libPhp/sqlClient.php';
include_once 'libPhp/dataSourceInterface.php';
include_once 'libPhp/JsonProcessPage.php';
include_once 'libPhp/jsonSource_mySqlI.php';
include_once 'libPhp/FastJSON.class.php';
include_once 'libPhp/jsonDataBind.php';
include_once 'libPhp/mySqlcredential.php';
include_once 'AppLimnorUserForum.php';
include_once 'libPhp/FileUploadItem.php';
include_once 'libPhp/filebrowser.php';
include_once 'libPhp/fileutility.php';

class WebFilePicker extends JsonProcessPage
{
private $WebAppPhp;
private $HtmlFileUpload1;
private $HtmlFileBrowser1;
function __construct()
{
  parent::__construct();
    $this->WebAppPhp = new AppLimnorUserForum();
$this->PhpPhysicalFolder = $this->WebAppPhp->PhpPhysicalFolder;
$this->HtmlFileUpload1=new HtmlFileUpload('HtmlFileUpload1');
$this->HtmlFileBrowser1=new HtmlFileBrowser('HtmlFileBrowser1');
}
protected function OnRequestStart()
{
	if ($this->DEBUG)
	{
		echo "PHP processor:". __FILE__."<br>";
	}
$this->HtmlFileUpload1->MaximumFileSize = 5242880;

}
protected function OnRequestClientData()
{
	if(property_exists($this->jsonFromClient->values,'allowedFileSize'))
	{
		$this->HtmlFileUpload1->MaximumFileSize = $this->jsonFromClient->values->allowedFileSize;
		if($GLOBALS["debug"]) {
			echo "file size limit:".$this->jsonFromClient->values->allowedFileSize."<br>";
		}
	}

}
protected function OnRequestFinish()
{

}
protected function OnRequestGetData($value)
{

}
protected function OnRequestPutData($value)
{

}
protected function OnRequestExecution($method, $value)
{
if($method == 'sdddb3fa1') $this->sdddb3fa1();
if($method == 'sabf74fbd') $this->sabf74fbd();
if($method == 'loadFolders_filebrowser') $this->HtmlFileBrowser1->loadFolders_filebrowser($this,$this->jsonFromClient->values->phpFolderName,$this->jsonFromClient->values->serverComponentName,$value);
if($method == 'loadFiles_filebrowser') $this->HtmlFileBrowser1->loadFiles_filebrowser($this,$this->jsonFromClient->values->phpFolderName,$this->jsonFromClient->values->serverComponentName,$this->jsonFromClient->values->filetypes,$value);

}
public $saveRet = "False";
function sdddb3fa1()
{
		$this->HtmlFileUpload1->FilePathSaved=$this->jsonFromClient->values->j5a272b4e;
		$this->HtmlFileUpload1->FileErrorText=$this->jsonFromClient->values->j82f2ad72;
		$this->saveRet=$this->jsonFromClient->values->jb0dc6b14;
		$this->saveRet=$this->jsonFromClient->values->jb0dc6b14;
		if(!($this->saveRet)) {
			return;
		}
$this->AddDownloadValue('j5a272b4e',$this->HtmlFileUpload1->FilePathSaved);
$this->AddClientScript('onclick_a6992b6b();');

}
function sabf74fbd()
{
	$this->saveRet=$this->HtmlFileUpload1->SaveToFolder($this->jsonFromClient->values->j25e2d7ec);
$this->AddDownloadValue('jb0dc6b14',$this->saveRet);
$this->AddDownloadValue('j82f2ad72',$this->HtmlFileUpload1->FileErrorText);
$this->AddDownloadValue('j5a272b4e',$this->HtmlFileUpload1->FilePathSaved);
$this->AddClientScript('onclick_c1c03d80();');

}

}
$w = new WebFilePicker();
$w->ProcessClientRequest();

?>
