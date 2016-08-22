<?php

include 'libPhp/sqlClient.php';
include 'libPhp/dataSourceInterface.php';
include 'libPhp/JsonProcessPage.php';
include 'libPhp/jsonSource_mySqlI.php';
include 'libPhp/FastJSON.class.php';
include 'libPhp/jsonDataBind.php';
include 'libPhp/mySqlcredential.php';

class Test extends JsonProcessPage
{
private $cr_0338061bff7b4388826a0e7dcef490cc;
function __construct($cr_0338061bff7b4388826a0e7dcef490cc)
{
  parent::__construct();
  $this->cr_0338061bff7b4388826a0e7dcef490cc = $cr_0338061bff7b4388826a0e7dcef490cc;
}
protected function OnRequestStart()
{

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
  if($method == 'testProcHtml') $this->testProcHtml();

}
function testProcHtml()
{
	$myFile = "clientData/testHtmlEdit.html";
	$fh = fopen($myFile, 'w') or die("can't open file for write");
	fwrite($fh, "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\">\r\n");
	fwrite($fh, $this->jsonFromClient->values->htmlStr);
	fclose($fh);
}

}
$w = new Test($cr_0338061bff7b4388826a0e7dcef490cc);
$w->ProcessClientRequest();

?>
