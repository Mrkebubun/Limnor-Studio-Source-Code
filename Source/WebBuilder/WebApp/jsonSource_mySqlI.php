<?php

/*
	Json Data Binding Library -- MySQLI data source for php 
	Copyright Longflow Enterprises Ltd
	2011

	Provide PHP interface between MySQL and PHP Json Data Binding.
	It fetches MySql data and converts to PHP Json objects conforming to Json Data Binding standard.
	It updates MySql database using data from Json objects conforming to Json Data Binding standard.
*/

/*
       NOT_NULL_FLAG = 1                                                                              
       PRI_KEY_FLAG = 2                                                                               
       UNIQUE_KEY_FLAG = 4                                                                            
       BLOB_FLAG = 16                                                                                 
       UNSIGNED_FLAG = 32                                                                             
       ZEROFILL_FLAG = 64                                                                             
       BINARY_FLAG = 128                                                                              
       ENUM_FLAG = 256                                                                                
       AUTO_INCREMENT_FLAG = 512                                                                      
       TIMESTAMP_FLAG = 1024                                                                          
       SET_FLAG = 2048                                                                                
       NUM_FLAG = 32768                                                                               
       PART_KEY_FLAG = 16384                                                                          
       GROUP_FLAG = 32768                                                                             
       UNIQUE_FLAG = 65536
*/
class Credential
{
	public $host;
	public $user;
	public $password;
	public $database;
}
class JsonSourceMySql extends DataSource
{
	public $host;
	public $user;
	public $password;
	public $database;
  public function SetCredential($c)
  {
      $this->host = $c->host;
      $this->user = $c->user;
      $this->password = $c->password;
      $this->database = $c->database;
  }
	public function GetData($tbl, $query, $Sqlparameters)
	{
		$mysqli = new mysqli($this->host, $this->user, $this->password, $this->database) or 
  			die("Problem connecting: ".mysqli_error());

		$stmt = $mysqli->stmt_init();
		if ($stmt->prepare($query)) {
			  $st = "";
        $pn = count($Sqlparameters);
        $pvs = array();
        $bindpV[] = &$st;
        for($i=0;$i<$pn;$i++)
        {
          $st = $st.$Sqlparameters[$i]->type;
          $pvs["p".strval($i)] = $Sqlparameters[$i]->value;
          $bindpV[] = &$pvs["p".strval($i)];
        }
        call_user_func_array(array($stmt,'bind_param'),$bindpV);
		    $stmt->execute();
        //
        $meta = $stmt->result_metadata();
        while ($column = $meta->fetch_field()) 
        {
          $tbl->addColumn($column->name, ($column->flags & 2) || ($column->flags & 4), $column->flags & 512, $column->flags & 512);
          $bindVarsArray[] = &$results[$column->name];
        }   
        $tbl->createColumnMap();
        call_user_func_array(array($stmt, 'bind_result'), $bindVarsArray);
        //
        while($stmt->fetch())
        {
          $r = $tbl->addRow();
          foreach($results as $key => $value)
          {
            $r->addColumnValue($tbl->columnIndex($key), $value);
          }
        }
		}
		// Free resultset
		$stmt->free_result();
    $stmt->close();
		// Closing connection
		$mysqli->close();

	}
} 

?>