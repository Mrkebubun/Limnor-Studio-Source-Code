<?php

/*
	Sql Client Library -- php 
	Copyright Longflow Enterprises Ltd
	2011

	Independent Sql Client classes for executing Sql queries
*/

class SqlClientParameter
{
	public $name;
	public $type;
	public $value;
}

class SqlScriptProcess
{
	public $script;
	public function FindParameterIndex($sql, $i)
	{
		$k;
		while ($i < strlen($sql))
		{
			if ($sql{$i} == '\'')
			{
				$k = $i;
				while (true)
				{
					$k = strpos($sql, '\'', $k+1);
					if ($k < 0)
					{
						//echo "Cannot find matching single-quote";
						return -1;
					}
					else
					{
						if ($k == strlen($sql) - 1)
							return -1;
						else
						{
							if ($sql{$k + 1} != '\'')
							{
								$i = $k;
								break;
							}
							else
								$k++;
						}
					}
				}
			}
			else if ($sql{$i} == '`')
			{
				$i = strpos($sql, '`', $i+1);
				if ($i < 0)
				{
					//echo "Cannot find matching `";
					return $i;
				}
			}
			else if ($sql{$i} == '@')
				return $i;
			$i++;
		}
		return -1;
	}
	public function FindNameEnd($sql, $nStart)
	{
		if ($nStart >= strlen($sql))
		{
			return strlen($sql);
		}
		$i = $nStart;
		$sSP = ',= +-*/><^|~)(;}\\';
		while($i < strlen($sql))
		{
			if(strpos($sSP, $sql{$i}) !== false)
				return $i;
			$i++;
		}
		return $i;
	}
	public function ParseParameters($sql)
	{
		$paramList = array();
		$i = 0;
		$j;
		$s;
		while (true)
		{
			$i = $this->FindParameterIndex($sql, $i);
			//echo 'pi='. $i. '<br>';
			if ($i >= 0)
			{
				$j = $this->FindNameEnd($sql, $i + 1);
				//echo 'nj='. $j. '<br>';
				if ($j > $i + 1)
				{
					$s = substr($sql,$i,$j-$i);
					//echo 'got:'. $s. '<br>';
					$paramList[] = $s;
					$sql = substr($sql,0, $i).'? '.substr($sql,$j);;
					$i += 2;
				}
				else
					break;
			}
			else
				break;
		}
		$this->script = $sql;
		return $paramList;
	}
	public function createSqlParameters($sql, $sqlvalues)
	{
		$values = array();
		$paramsUsed = $this->ParseParameters($sql);
		foreach($paramsUsed as $pa)
		{
			$found = false;
			foreach($sqlvalues as $a)
			{
				if($a->name == $pa)
				{
					$found = true;
					$values[] = $a;
					break;
				}
			}
			if(!$found)
			{
				throw new Exception('parameter '. $pa. ' not defined');
			}
		}
		return $values;
	}
}
?> 