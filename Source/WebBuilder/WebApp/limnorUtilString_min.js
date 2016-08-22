// limnorUtilityString.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

﻿var limnorUtility=limnorUtility||{};limnorUtility.replaceAll=function(s,key,value){if(key!=value){while(s.indexOf(key)>=0){s=s.replace(key,value);}}
return s;}
limnorUtility.RegExpQuote=function(str){return str.replace(/([.?*+^$[\]\\(){}|-])/g,"\\$1");};limnorUtility.string=function(value,fields){var _value=value;var _fields=fields;return{setProperty:function(key,val){_fields[key]=val;},getProperty:function(key){return _fields[key];},formValue:function(){var s=_value;for(var nm in _fields){if(typeof _fields[nm]!='function'){var key='[!'+nm+'!]';s=s.replace(new RegExp(limnorUtility.RegExpQuote(key),'g'),_fields[nm]);}}
return s;},formValueByParams:function(params){var s=_value;for(var nm in _fields){if(typeof _fields[nm]!='function'){var key='[!'+nm+'!]';if(typeof params[nm]!='undefined'){s=s.replace(new RegExp(limnorUtility.RegExpQuote(key),'g'),params[nm]);}
else{s=s.replace(new RegExp(limnorUtility.RegExpQuote(key),'g'),'');}}}
return s;},UnformValue:function(){return _value;},append:function(val){return _value+val;},replace:function(key,val){return _value.replace(new RegExp(limnorUtility.RegExpQuote(key),'g'),val);},replaceI:function(key,val){return _value.replace(new RegExp(limnorUtility.RegExpQuote(key),'gi'),val);},Length:function(){return _value.length;}};}