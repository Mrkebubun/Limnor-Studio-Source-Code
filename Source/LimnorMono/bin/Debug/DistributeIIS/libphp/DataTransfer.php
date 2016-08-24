<?php

class EasyTransfer
{
	public $ObjectName;
	public $EndPointType;
	public $sourceCredential;
	public $destinationCredential;
	public $DEBUG;
	function __construct($name)
	{
		$this->ObjectName = $name;
	}
	public function startDataTransfer() 
	{
		$msql = new JsonSourceMySql();
		$msql->SetCredential($this->sourceCredential);
		$msql->SetDebug($this->DEBUG);
	}
}

?>