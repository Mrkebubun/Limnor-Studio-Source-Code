<?php

/*
	Json Data Binding Library -- php implementation
	Copyright Longflow Enterprises Ltd
	2011

	Provide PHP interface to Json Data Binding.
	It is an interface between data sources (MySQL, SQL Server, etc) and the binding standard
*/



class JsonDataRow
{
	public $ItemArray = array();
	
	public function addValue($v)
	{
		if($this->ItemArray == null)
		{
			$this->ItemArray = array($v);
		}
		else
		{
			$this->ItemArray = array_values($this->ItemArray);
	        $this->ItemArray[] = $v;
		}
	}
    public function addColumnValue($i, $v)
	{
		$this->ItemArray[$i] = $v;
	}
}
class JsonDataRowUpdate extends JsonDataRow
{
	public $KeyValues = array();
	public $Deleted;
	public $Added;
}
class JsonDataColumn
{
	public $Name;
	public $ReadOnly;
	public $Type; //int,string,datetime,float
}
class JsonDataTable
{
    public $TableName;
    public $CaseSensitive;
	public $Columns = array();
    public $PrimaryKey = array();
    public $Rows = array();
	//
    private $useAutoNumber;
    private $columnMap;
    function __construct()
    {
        $this->useAutoNumber = false;
    }
	public function addRow()
	{
		$r = new JsonDataRow();
		if($this->Rows == null)
		{
			$this->Rows = array($r);
		}
		else
		{
			$this->Rows = array_values($this->Rows);
	       	$this->Rows[] = $r;
		}
		return $r;
	}
    public function createColumnMap()
    {
        foreach($this->Columns as $key => $value)
        {
          $this->columnMap[$value->Name] = $key;
        }
    }
    public function columnIndex($name)
    {
        return $this->columnMap[$name];
    }
    public function addColumn($name, $primary_key, $isAutoNumber, $isReadOnly) 
	{
		$c = new JsonDataColumn();
		$c->Name = $name;
		$c->ReadOnly = ($isAutoNumber | $isReadOnly);
		if($this->Columns == null)
		{
			$this->Columns = array($c);
		}
		else
		{
			$this->Columns = array_values($this->Columns);
	       	$this->Columns[] = $c;
		}
        if($isAutoNumber)
        {
            $this->useAutoNumber = true;
            $this->PrimaryKey = array($name);
        }
        else
        {
		    if($primary_key && !$this->useAutoNumber)
		    {
			    if($this->PrimaryKey == null)
			    {
				  $this->PrimaryKey = array($name);
			    }
			    else
			    {
				  $this->PrimaryKey = array_values($this->PrimaryKey);
	        	  $this->PrimaryKey[] = $name;
			    }
		    }
        }
    }
}
// some data used in client
class JsonDataTableUpdate extends JsonDataTable
{
	public $RowIndex;
	public $columnIndexes; 
}
class JsonDataSet
{
	public $DataSetName;
    public $Locale;
    public $Tables = array(); //a JsonDataTable array
}
class RequestCommand
{
	public $Method; //a string
	public $Value; //a string
}

class WebRequestOrResponse
{
	//RequestCommand array for request from client to server
	//a string array for response from server to client
    public $Calls = array(); //unlike c#, php may holds different types
	public $Data; //a JsonDataSet
	public $Values; //an array as a Dictionary<string, object> in c#

    // method declaration
    public function addCall($call) 
	{
	    if($this->Calls == null)
	    {
		    $this->Calls = array($call);
	    }
	    else
	    {
		    $this->Calls = array_values($this->Calls);
	        $this->Calls[] = $call;
	    }
    }
    public function createDataset($name)
    {
		if($this->Data == null)
		{
			$this->Data = new JsonDataSet();
			$this->Data->DataSetName = $name;
		}
    }
    public function addTable($name) 
	{
	    $t = new JsonDataTable();
	    $t->TableName = $name;
		if($this->Data == null)
		{
			$this->Data = new JsonDataSet();
			$this->Data->DataSetName = 'data1';
		}
	    if($this->Data->Tables == null)
	    {
		    $this->Data->Tables = array($t);
	    }
	    else
	    {
		    $this->Data->Tables = array_values($this->Data->Tables);
	        $this->Data->Tables[] = $t;
	    }
	    return $t;
    }
}


?>