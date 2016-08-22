<?php
/*
	Json Data Binding Library -- An abstract class for server side processing and make client response
	Copyright Longflow Enterprises Ltd
	2011

	each server response php page should derive a sub-class from this class
	and implement abstract functions to do processing
*/

abstract class JsonProcessPage
{
	protected $jsonFromClient; //a WebRequestOrResponse from client
	protected $response;       //a WebRequestOrResponse to be sent to the client
	protected $shoudlSendResponse;
	abstract protected function OnRequestGetData($value);
	abstract protected function OnRequestPutData($value);
	abstract protected function OnRequestExecution($method, $value);
	abstract protected function OnRequestFinish();
	 
    function __construct() 
    {
		$this->response = new WebRequestOrResponse();
		$this->shoudlSendResponse = true;
    }
	protected function AddApplyDataBindMethod($dataName)
	{
		if($this->response->Calls == null)
		{
			$this->response->Calls = array();
		}
		$this->response->Calls = array_values($this->response->Calls);
		if($dataName == null)
		{
			$this->response->Calls[] = '_setDataSource.call(this);';
		}
		else
		{
			$this->response->Calls[] = '_setDataSource.call(this,\''.$dataName.'\');';
		}
	}
	protected function AddClientScript($script)
	{
		if($this->response->Calls == null)
		{
			$this->response->Calls = array();
		}
		$this->response->Calls = array_values($this->response->Calls);
		$this->response->Calls[] = $script;
	}
	protected function AddDownloadValue($name, $value)
	{
		if($this->response->Values == null)
		{
			$this->response->Values = array();
		}
		$this->response->Values[$name] = $value;
	}
	protected function AddDataTable($name)
	{
		return $this->response->addTable($name);
	}
    public function ProcessClientRequest()
    {
		$raw = file_get_contents('php://input');
		$this->jsonFromClient = json_decode($raw);
		if( $this->jsonFromClient->Calls == null)
		{
		}
		else
		{
			$pn = count($this->jsonFromClient->Calls);
            for($i=0;$i<$pn;$i++)
			{
				if($this->jsonFromClient->Calls[$i]->Method == 'jsonDb_getData')
				{
					$this->OnRequestGetData($this->jsonFromClient->Calls[$i]->Value);
				}
				else if($this->jsonFromClient->Calls[$i]->Method == 'jsonDb_putData')
				{
					$this->OnRequestPutData($this->jsonFromClient->Calls[$i]->Value);
				}
				else
				{
					$this->OnRequestExecution($this->jsonFromClient->Calls[$i]->Method,$this->jsonFromClient->Calls[$i]->Value);
				}
			}
		}
		$this->OnRequestFinish();
		if($this->shoudlSendResponse)
		{
			echo FastJSON::encode($this->response);
		}
	}
}

?>