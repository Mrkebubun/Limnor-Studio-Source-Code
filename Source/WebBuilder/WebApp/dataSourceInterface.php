<?php

/*
	Json Data Binding Library -- An abstract interface to data sources
	Copyright Longflow Enterprises Ltd
	2011

*/

abstract class DataSource
{
    abstract public function SetCredential($c);
	abstract public function GetData($tbl, $query, $Sqlparameters);
	
}

?>