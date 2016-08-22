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
	public function getColumnValue($i)
	{
		return $this->ItemArray[$i];
	}
	public function getColumnCount()
	{
		return count($ItemArray);
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
	public $isAutoNumber;
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
	public function isBlob($i)
	{
		if($this->Columns[$i]->Type == 252)
		{
			return true;
		}
		return false;
	}
	public function GetColumnByName($name)
	{
		$cn = count($this->Columns);
		for($i = 0;$i<$cn;$i++)
		{
			if($this->Columns[$i]->Name == $name)
			{
				return $this->Columns[$i];
			}
		}
		return null;
	}
	public function GetMaxValue($colName)
	{
		$v = null;
		$c = $this->columnMap[$name];
		if($this->Rows != null)
		{
			foreach($this->Rows as $rIdx => $row)
			{
				if($v == null)
				{
					$v = $row->ItemArray[$c];
				}
				else
				{
					if($row->ItemArray[$c] > $v)
					{
						$v = $row->ItemArray[$c];
					}
				}
			}
		}
		return $v;
	}
	public function addColumn($name, $primary_key, $isAutoNumber, $isReadOnly, $type) 
	{
		$c = new JsonDataColumn();
		$c->Name = $name;
		$c->ReadOnly = ($isAutoNumber | $isReadOnly);
		$c->Type = $type;
		$c->isAutoNumber = $isAutoNumber;
		if($this->Columns == null)
		{
			$this->Columns = array($c);
		}
		else
		{
			$colExist = false;
			foreach($this->Columns as $ci => $col)
			{
				if($col->Name == $name)
				{
					$colExist = true;
					break;
				}
			}
			if(!$colExist)
			{
				$this->Columns = array_values($this->Columns);
				$this->Columns[] = $c;
			}
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
					$colExist = false;
					foreach($this->PrimaryKey as $ci => $colN)
					{
						if($colN == $name)
						{
							$colExist = true;
							break;
						}
					}
					if(!$colExist)
					{
						$this->PrimaryKey = array_values($this->PrimaryKey);
						$this->PrimaryKey[] = $name;
					}
				}
			}
		}
		if($GLOBALS["debug"])
		{
			echo "added column ". $name. ", is key:". $primary_key. ", is auto:". $isAutoNumber. ", read-only:". $isReadOnly. ", type:", $type. "<br>";
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
	public $method; //a string
	public $value; //a string
}
class DatabaseExecuter
{
	public $outputValues;
	public $ErrorMessage;
	function __construct() 
	{
		$this->outputValues = array();
		$this->ErrorMessage = '';
	}
	public function ExecuteWithOutputs($sql, $query, $ps, $msql)
	{
		$this->ErrorMessage = '';
		$tbl = new JsonDataTable();
		$tbl->TableName = 'data';
		if(!$msql->QueryWithPreparer($sql, $ps, $tbl, $query, null))
		{
			$this->ErrorMessage = "db error:".$msql->errorMessage;
			if($GLOBALS["debug"])
			{
				echo $this->ErrorMessage."<br>";
			}
			return $this->ErrorMessage;
		}
		if($GLOBALS["debug"])
		{
			echo "rows:".count($tbl->Rows)."<br>";
		}
		if(count($tbl->Rows)>0)
		{
			if($GLOBALS["debug"])
			{
				echo "columns:".count($tbl->Columns)."<br>";
			}
			if(count($tbl->Columns) > 0)
			{
				foreach($tbl->Rows as $ridx => $row)
				{
					foreach($row->ItemArray as $cidx => $col)
					{
						$this->outputValues[$tbl->Columns[$cidx]->Name] = $col;
						if($GLOBALS["debug"])
						{
							echo "col ".$cidx.", name:".$tbl->Columns[$cidx]->Name.", value:".$col;
							echo "<br>";
						}
					}
				}
			}
			else
			{
				if($GLOBALS["debug"])
				{
					echo "No output parameters found";
				}
			}
		}
		else
		{
			$this->ErrorMessage = 'failed to return output parameters';
		}
		return $this->ErrorMessage;
	}
}
class WebRequestOrResponse
{
	//RequestCommand array for request from client to server
	//a string array for response from server to client
	public $Calls = array(); //unlike c#, php may holds different types
	public $Data; //a JsonDataSet
	public $values = array(); //an array as a Dictionary<string, object> in c#
	public $serverComponentName; //component name for firing client event
	public function GetNumberOfCalls()
	{
		if($this->Calls == null)
		{
			return 0;
		}
		else
		{
			return count($this->Calls);
		}
	}
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