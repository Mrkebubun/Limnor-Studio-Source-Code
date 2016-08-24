// limnorStudio.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

var JSONIDENTIFIER=JSONIDENTIFIER||{};var JSON;if(!JSON){JSON={};}
(function(){'use strict';function f(n){return n<10?'0'+n:n;}
if(typeof Date.prototype.toJSON!=='function'){Date.prototype.toJSON=function(key){return isFinite(this.valueOf())?this.getUTCFullYear()+'-'+
f(this.getUTCMonth()+1)+'-'+
f(this.getUTCDate())+'T'+
f(this.getUTCHours())+':'+
f(this.getUTCMinutes())+':'+
f(this.getUTCSeconds())+'Z':null;};String.prototype.toJSON=Number.prototype.toJSON=Boolean.prototype.toJSON=function(key){return this.valueOf();};}
var cx=/[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,escapable=/[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,gap,indent,meta={'\b':'\\b','\t':'\\t','\n':'\\n','\f':'\\f','\r':'\\r','"':'\\"','\\':'\\\\'},rep;function quote(string){escapable.lastIndex=0;return escapable.test(string)?'"'+string.replace(escapable,function(a){var c=meta[a];return typeof c==='string'?c:'\\u'+('0000'+a.charCodeAt(0).toString(16)).slice(-4);})+'"':'"'+string+'"';}
function str(key,holder){var i,k,v,length,mind=gap,partial,value=holder[key];if(value&&typeof value==='object'&&typeof value.toJSON==='function'){value=value.toJSON(key);}
if(typeof rep==='function'){value=rep.call(holder,key,value);}
switch(typeof value){case'string':return quote(value);case'number':return isFinite(value)?String(value):'null';case'boolean':case'null':return String(value);case'object':if(!value){return'null';}
gap+=indent;partial=[];if(Object.prototype.toString.apply(value)==='[object Array]'){length=value.length;for(i=0;i<length;i+=1){partial[i]=str(i,value)||'null';}
v=partial.length===0?'[]':gap?'[\n'+gap+partial.join(',\n'+gap)+'\n'+mind+']':'['+partial.join(',')+']';gap=mind;return v;}
if(rep&&typeof rep==='object'){length=rep.length;for(i=0;i<length;i+=1){if(typeof rep[i]==='string'){k=rep[i];v=str(k,value);if(v){partial.push(quote(k)+(gap?': ':':')+v);}}}}else{for(k in value){if(Object.prototype.hasOwnProperty.call(value,k)){v=str(k,value);if(v){partial.push(quote(k)+(gap?': ':':')+v);}}}}
v=partial.length===0?'{}':gap?'{\n'+gap+partial.join(',\n'+gap)+'\n'+mind+'}':'{'+partial.join(',')+'}';gap=mind;return v;}}
if(typeof JSON.stringify!=='function'){JSON.stringify=function(value,replacer,space){var i;gap='';indent='';if(typeof space==='number'){for(i=0;i<space;i+=1){indent+=' ';}}else if(typeof space==='string'){indent=space;}
rep=replacer;if(replacer&&typeof replacer!=='function'&&(typeof replacer!=='object'||typeof replacer.length!=='number')){throw new Error('JSON.stringify');}
return str('',{'':value});};}
if(typeof JSON.parse!=='function'){JSON.parse=function(text,reviver){var j;function walk(holder,key){var k,v,value=holder[key];if(value&&typeof value==='object'){for(k in value){if(Object.prototype.hasOwnProperty.call(value,k)){v=walk(value,k);if(v!==undefined){value[k]=v;}else{delete value[k];}}}}
return reviver.call(holder,key,value);}
text=String(text);cx.lastIndex=0;if(cx.test(text)){text=text.replace(cx,function(a){return'\\u'+
('0000'+a.charCodeAt(0).toString(16)).slice(-4);});}
if(/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,'@').replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,']').replace(/(?:^|:|,)(?:\s*\[)+/g,''))){j=eval('('+text+')');return typeof reviver==='function'?walk({'':j},''):j;}
throw new SyntaxError('JSON.parse');};}}());var JsonDataBinding=JsonDataBinding||{NullDisplay:'',NullDisplayText:'{null}',DebugLevel:0,UseLocalStore:false,print:function(e){if(e&&e.outerHTML){var w=window.open('','','left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0');w.document.write(e.outerHTML);w.document.close();w.focus();w.print();w.close();}},toText:function(v){if(typeof(v)=='undefined'){return'';}
if(v==null)
return'';if(v instanceof Date){return JsonDataBinding.datetime.toIso(v);}
if(typeof(v.toTimeString)=='function')
return v.toTimeString();return v;},OpenDebugWindow:function(){return window.top.open("","debugWindows");},ShowDebugInfoLine:function(msg){if(JsonDataBinding.DebugLevel>0){var winDebug=JsonDataBinding.OpenDebugWindow();if(winDebug==null){alert('Debug information cannot be displayed. Your web browser has disabled pop-up window');}
else{winDebug.document.write(JsonDataBinding.datetime.toIso(new Date()));winDebug.document.write(' - ');winDebug.document.write(msg);winDebug.document.write('<br>');}}},fireEvent:function(sender,eventName){if(!sender){return;}
try{var eventObj;if(document.createEvent){eventObj=document.createEvent('HTMLEvents');if(JsonDataBinding.startsWith(eventName,'on')){eventName=eventName.substr(2);}
eventObj.initEvent(eventName,true,true);sender.dispatchEvent(eventObj);}else if(document.createEventObject){eventObj=document.createEventObject();if(!JsonDataBinding.startsWith(eventName,'on')){eventName='on'+eventName;}
sender.fireEvent(eventName,eventObj);}else{if(!JsonDataBinding.startsWith(eventName,'on')){eventName='on'+eventName;}
if(sender[eventName]){sender[eventName]();}}}
catch(e){alert(e.message?e.message:e);}},bytes2utf8:function(){function _encode(stringToEncode,insertBOM){stringToEncode=stringToEncode.replace(/\r\n/g,"\n");var utftext=[];if(insertBOM==true){utftext[0]=0xef;utftext[1]=0xbb;utftext[2]=0xbf;}
for(var n=0;n<stringToEncode.length;n++){var c=stringToEncode.charCodeAt(n);if(c<128){utftext[utftext.length]=c;}
else if((c>127)&&(c<2048)){utftext[utftext.length]=(c>>6)|192;utftext[utftext.length]=(c&63)|128;}
else{utftext[utftext.length]=(c>>12)|224;utftext[utftext.length]=((c>>6)&63)|128;utftext[utftext.length]=(c&63)|128;}}
return utftext;};var obj={encode:function(stringToEncode){return _encode(stringToEncode,false);},encodeWithBOM:function(stringToEncode){return _encode(stringToEncode,true);},decode:function(dotNetBytes){var result="";var i=0;var c=c1=c2=0;if(dotNetBytes.length>=3){if((dotNetBytes[0]&0xef)==0xef&&(dotNetBytes[1]&0xbb)==0xbb&&(dotNetBytes[2]&0xbf)==0xbf){i=3;}}
while(i<dotNetBytes.length){c=dotNetBytes[i]&0xff;if(c<128){result+=String.fromCharCode(c);i++;}
else if((c>191)&&(c<224)){if(i+1>=dotNetBytes.length)
throw"Un-expected encoding error, UTF-8 stream truncated, or incorrect";c2=dotNetBytes[i+1]&0xff;result+=String.fromCharCode(((c&31)<<6)|(c2&63));i+=2;}
else{if(i+2>=dotNetBytes.length||i+1>=dotNetBytes.length)
throw"Un-expected encoding error, UTF-8 stream truncated, or incorrect";c2=dotNetBytes[i+1]&0xff;c3=dotNetBytes[i+2]&0xff;result+=String.fromCharCode(((c&15)<<12)|((c2&63)<<6)|(c3&63));i+=3;}}
return result;}};return obj;}(),Base64:function(){var _keyStr="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";_base64encode=function(input){if(typeof input!='undefined'){var output="";var chr1,chr2,chr3,enc1,enc2,enc3,enc4;var i=0;input=_utf8_encode(input);while(i<input.length){chr1=input.charCodeAt(i++);chr2=input.charCodeAt(i++);chr3=input.charCodeAt(i++);enc1=chr1>>2;enc2=((chr1&3)<<4)|(chr2>>4);enc3=((chr2&15)<<2)|(chr3>>6);enc4=chr3&63;if(isNaN(chr2)){enc3=enc4=64;}else if(isNaN(chr3)){enc4=64;}
output=output+
_keyStr.charAt(enc1)+_keyStr.charAt(enc2)+
_keyStr.charAt(enc3)+_keyStr.charAt(enc4);}
return output;}}
_base64decode=function(input){if(typeof input!='undefined'){var output="";var chr1,chr2,chr3;var enc1,enc2,enc3,enc4;var i=0;input=input.replace(/[^A-Za-z0-9\+\/\=]/g,"");while(i<input.length){enc1=_keyStr.indexOf(input.charAt(i++));enc2=_keyStr.indexOf(input.charAt(i++));enc3=_keyStr.indexOf(input.charAt(i++));enc4=_keyStr.indexOf(input.charAt(i++));chr1=(enc1<<2)|(enc2>>4);chr2=((enc2&15)<<4)|(enc3>>2);chr3=((enc3&3)<<6)|enc4;output=output+String.fromCharCode(chr1);if(enc3!=64){output=output+String.fromCharCode(chr2);}
if(enc4!=64){output=output+String.fromCharCode(chr3);}}
output=_utf8_decode(output);return output;}}
function _utf8_encode(string){string=string.replace(/\r\n/g,"\n");var utftext="";for(var n=0;n<string.length;n++){var c=string.charCodeAt(n);if(c<128){utftext+=String.fromCharCode(c);}
else if((c>127)&&(c<2048)){utftext+=String.fromCharCode((c>>6)|192);utftext+=String.fromCharCode((c&63)|128);}
else{utftext+=String.fromCharCode((c>>12)|224);utftext+=String.fromCharCode(((c>>6)&63)|128);utftext+=String.fromCharCode((c&63)|128);}}
return utftext;}
function _utf8_decode(utftext){var string="";var i=0;var c=0,c1=0,c2=0;while(i<utftext.length){c=utftext.charCodeAt(i);if(c<128){string+=String.fromCharCode(c);i++;}
else if((c>191)&&(c<224)){c2=utftext.charCodeAt(i+1);string+=String.fromCharCode(((c&31)<<6)|(c2&63));i+=2;}
else{c2=utftext.charCodeAt(i+1);c3=utftext.charCodeAt(i+2);string+=String.fromCharCode(((c&15)<<12)|((c1&63)<<6)|(c2&63));i+=3;}}
return string;}}(),base64Encode:function(input){return _base64encode(input);},base64Decode:function(input){return _base64decode(input);},isNumber:function(n){return!isNaN(parseFloat(n))&&isFinite(n);},removeStyleAttribute:function(o,name,cssName){if(o.style.removeProperty)
o.style.removeProperty(cssName?cssName:name)
else if(o.style.removeAttribute)
o.style.removeAttribute(name);else
alert('You browser does not support removing style');},_binder:function(){var jsdb_serverPage=String();var jsdb_bind='jsdb';var jsdb_getdata='jsonDb_getData';var jsdb_putdata='jsonDb_putData';var const_userAlias='logonUserAlias';var e_onchange='onchange';var type_func='function';var _isIE=((navigator.appName=='Microsoft Internet Explorer')||((navigator.appName=='Netscape')&&(new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})").exec(navigator.userAgent)!=null)));var _isFireFox=(navigator.userAgent.indexOf("Firefox")!=-1);var _isChrome=(navigator.userAgent.toLowerCase().indexOf('chrome')>-1);var _isSafari=((navigator.userAgent.toLowerCase().indexOf('safari')>-1)&&!_isChrome);if(!String.prototype.trim){String.prototype.trim=function(){return this.replace(/^\s+|\s+$/g,'');}}
IsIE=function(){return _isIE;}
IsFireFox=function(){return _isFireFox;}
IsChrome=function(){return _isChrome;}
IsSafari=function(){return _isSafari;}
IsOpera=function(){return(typeof(window.opera)!='undefined');}
getEventSender=function(e){var c;if(!e)e=window.event;if(e.target)c=e.target;else if(e.srcElement)c=e.srcElement;if(typeof c!='undefined'){if(c.nodeType==3)
c=c.parentNode;}
return c;}
var _windows=new Array();_addWindow=function(w){var i=0;while(i<_windows.length){if(!_windows[i]){_windows.splice(i,1);}
else if(_windows[i].closed){_windows.splice(i,1);}
else{i++;}}
_windows.push(w);}
_getWindowById=function(pageId){for(var i=0;i<_windows.length;i++){if(_windows[i]){if(!_windows[i].closed){if(_windows[i].document.pageId==pageId){return _windows[i];}}}}
if(JsonDataBinding.getChildWindowById){return JsonDataBinding.getChildWindowById(pageId);}}
_getWindowByPageFilename=function(pageFilename){for(var i=0;i<_windows.length;i++){if(_windows[i]){if(!_windows[i].closed){if(JsonDataBinding.endsWithI(_windows[i].document.URL,pageFilename)){return _windows[i];}}}}}
var _serverComponentName;var _clientEventsHolder;_getClientEventHolder=function(eventName,objectName){if(eventName&&objectName){if(!_clientEventsHolder){_clientEventsHolder={};}
if(!_clientEventsHolder[eventName]){_clientEventsHolder[eventName]={};}
var eh=_clientEventsHolder[eventName];if(!eh[objectName]){eh[objectName]={};eh[objectName].handlers=new Array();}
return eh[objectName];}}
_attachExtendedEvent=function(eventName,objectName,handler){var eho=_getClientEventHolder(eventName,objectName);if(eho){if(!eho.handlers){eho.handlers=new Array();}
var b=false;for(var i=0;i<eho.handlers.length;i++){if(eho.handlers[i]==handler){b=true;break;}}
if(!b){eho.handlers.push(handler);}}}
_detachExtendedEvent=function(eventName,objectName,handler){var eho=_getClientEventHolder(eventName,objectName);if(eho&&eho.handlers){for(var i=0;i<eho.handlers.length;i++){if(eho.handlers[i]==handler){eho.handlers.splice(i,1);break;}}}}
_clearExtendedEvent=function(eventName,objectName){var eho=_getClientEventHolder(eventName,objectName);if(eho){eho.handlers=new Array();}}
_switchExtendedEvent=function(eventName,objectName,handler){_clearExtendedEvent(eventName,objectName);_attachExtendedEvent(eventName,objectName,handler)}
_fetchDetailRows=function(objectName){var detailTbls=_getTableAttribute(objectName,'LinkedDetails');if(detailTbls&&sources){if(sources[objectName]&&sources[objectName].Rows&&sources[objectName].Rows.length>0&&sources[objectName].rowIndex>=0){for(var i=0;i<detailTbls.length;i++){if(sources[detailTbls[i].detailTableName]&&detailTbls[i].dataset&&detailTbls[i].dataset.length>sources[objectName].rowIndex&&detailTbls[i].dataset[sources[objectName].rowIndex]){sources[detailTbls[i].detailTableName].Rows=detailTbls[i].dataset[sources[objectName].rowIndex];sources[detailTbls[i].detailTableName].rowIndex=sources[detailTbls[i].detailTableName].Rows.length>0?0:-1;bindTable(detailTbls[i].detailTableName,true);}
else{_setTableAttribute(detailTbls[i].detailTableName,'LinkedMaster',detailTbls[i]);var rel={};for(var k=0;k<detailTbls[i].fields.length;k++){rel[detailTbls[i].fields[k].code]=JsonDataBinding.columnValue(detailTbls[i].masterTableName,detailTbls[i].fields[k].name);}
JsonDataBinding.executeServerMethod(detailTbls[i].masterMethod,rel);}}}
else{for(var i=0;i<detailTbls.length;i++){if(sources&&sources[detailTbls[i].detailTableName]){sources[detailTbls[i].detailTableName].Rows=[];sources[detailTbls[i].detailTableName].rowIndex=-1;bindTable(detailTbls[i].detailTableName,true);}}}}}
_refetchDetailRows=function(objectName,detailTableName){var detailTbls=_getTableAttribute(objectName,'LinkedDetails');if(detailTbls&&sources){if(sources[objectName]&&sources[objectName].Rows&&sources[objectName].Rows.length>0&&sources[objectName].rowIndex>=0){for(var i=0;i<detailTbls.length;i++){if(detailTbls[i].detailTableName==detailTableName){_setTableAttribute(detailTbls[i].detailTableName,'LinkedMaster',detailTbls[i]);var rel={};for(var k=0;k<detailTbls[i].fields.length;k++){rel[detailTbls[i].fields[k].code]=JsonDataBinding.columnValue(detailTbls[i].masterTableName,detailTbls[i].fields[k].name);}
JsonDataBinding.executeServerMethod(detailTbls[i].masterMethod,rel);}}}
else{for(var i=0;i<detailTbls.length;i++){if(sources&&sources[detailTbls[i].detailTableName]){sources[detailTbls[i].detailTableName].Rows=[];sources[detailTbls[i].detailTableName].rowIndex=-1;bindTable(detailTbls[i].detailTableName,true);}}}}}
_executeEventHandlers=function(eventName,objectName,data,attrs){if(eventName=='CurrentRowIndexChanged'){_fetchDetailRows(objectName);}
else if(eventName=='DataArrived'){if(attrs&&attrs.isFirstTime){var detailTbls=_getTableAttribute(objectName,'LinkedDetails');if(detailTbls){for(var i=0;i<detailTbls.length;i++){detailTbls[i].dataset=null;if(sources&&sources[detailTbls[i].detailTableName]){sources[detailTbls[i].detailTableName].Rows=[];sources[detailTbls[i].detailTableName].rowIndex=-1;bindTable(detailTbls[i].detailTableName,true);}}}}
if(data){var obj=data[objectName];if(obj&&obj.fields){var rel=_getTableAttribute(objectName,'LinkedMaster');if(rel&&sources[rel.masterTableName]&&sources[rel.masterTableName].Rows&&sources[rel.masterTableName].Rows.length>0){var masterRowIdx=sources[rel.masterTableName].rowIndex;var isCurrent=(masterRowIdx>=0);if(isCurrent){for(var k=0;k<rel.fields.length;k++){if(obj.fields[rel.fields[k].name]!=JsonDataBinding.columnValue(rel.masterTableName,rel.fields[k].name)){isCurrent=false;break;}}}
if(isCurrent){if(!rel.dataset){rel.dataset=new Array(sources[rel.masterTableName].Rows.length);}
rel.dataset[masterRowIdx]=sources[objectName].Rows;}}}}}
var eho=_getClientEventHolder(eventName,objectName);if(eho&&eho.handlers){for(var i=0;i<eho.handlers.length;i++){if(eventName=='DataUpdated'&&data){eho.handlers[i](null,data);}
else{eho.handlers[i]();}}}}
_getClientEventObject=function(eventName){if(_clientEventsHolder&&_serverComponentName){var eh=_clientEventsHolder[eventName];if(eh&&eh[_serverComponentName]){return eh[_serverComponentName];}}}
_executeClientEventObject=function(eventName){var eho=_getClientEventObject(eventName);if(eho&&eho.handlers){for(var i=0;i<eho.handlers.length;i++){eho.handlers[i](JsonDataBinding.values.serverFailure);}}}
var _objectProperties={};_getObjectProperty=function(objectName,propertyName){if(_objectProperties[objectName]){var obj=_objectProperties[objectName];return obj[propertyName];}}
_setObjectProperty=function(objectName,propertyName,value){if(!_objectProperties[objectName]){_objectProperties[objectName]={};}
var obj=_objectProperties[objectName];obj[propertyName]=value;}
_onSetCustomValue=function(obj,valueName){var dbs=obj.getAttribute(jsdb_bind);if(typeof dbs!='undefined'&&dbs!=null&&dbs!=''){var binds=dbs.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var sourceName=bind[0];var tbl=sources[sourceName];if(typeof tbl!='undefined'){var field=bind[1];var target=bind[2];if(valueName==target){var rIdx;var rIdxs;if(typeof obj.jsdbRowIndex!='undefined'){rIdxs=obj.jsdbRowIndex;}
if(rIdxs){rIdx=rIdxs[sourceName];}
var rIdx0=tbl.rowIndex;if(typeof rIdx=='undefined'){rIdx=rIdx0;}
if(rIdx>=0&&rIdx<tbl.Rows.length){tbl.rowIndex=rIdx;preserveKeys(sourceName);var c=_columnNameToIndex(tbl.TableName,field);var v;v=obj[target];tbl.Rows[rIdx].ItemArray[c]=v;tbl.Rows[rIdx].changed=true;JsonDataBinding.onvaluechanged(tbl,rIdx,c,v);tbl.rowIndex=rIdx0;}
break;}}}}}
var jsdb_cultureName='cultureName';var _cultureName='en';var resTable={'TableName':'_pageResources_','Columns':[{'Name':'cultureName','ReadOnly':'true','Type':'string'}],'PrimaryKey':['cultureName'],'DataRelations':[],'Rows':[]};var _datetimePicker;var _datetimeInputs;_getdatetimepicker=function(){return _datetimePicker;}
_pushDatetimeInput=function(textBoxId){if(!_datetimeInputs){_datetimeInputs=new Array();}
_datetimeInputs.push(textBoxId)}
_setDatetimePicker=function(datetimePicker){_datetimePicker=datetimePicker;if(_datetimePicker){if(_datetimeInputs){for(var i=0;i<_datetimeInputs.length;i++){JsonDataBinding.CreateDatetimePickerForTextBox(_datetimeInputs[i].textBoxId,_datetimeInputs[i].fontsize,_datetimeInputs[i].inputTime,_datetimeInputs[i].standalone,_datetimeInputs[i].container,_datetimeInputs[i].disableMove);}
_datetimeInputs=null;}}}
var dataChangeHandlers={};var sources=new Object();var tableAttributes=new Object();var handlersOnRowIndex=new Object();var onrowdeletehandlers=new Object();var hasActivity=false;var activityWatcher;activity=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){if(hasActivity){hasActivity=false;var uu=u.split(' ');if(uu.length>3){JsonDataBinding.setCookie(const_userAlias,u,uu[3]);}}
activityWatcher=setTimeout(activity,3000);}
else{window.location.reload();}}
else{window.location.reload();}}
var _sessionWatcher=null;var _sessionTimeout=20;_startSessionWatcher=function(){if(!_sessionWatcher){_sessionWatcher=setTimeout(_sessionKeepAlive,_sessionTimeout*3000);}}
_sessionKeepAlive=function(){var vs=_getSessionVariables();if(vs!=null&&vs.length>0){for(var i=0;i<vs.length;i++){JsonDataBinding.setCookie(vs[i].name,vs[i].value,_sessionTimeout);}
_sessionWatcher=setTimeout(_sessionKeepAlive,_sessionTimeout*3000);}
else{_sessionWatcher=null;}}
_setSessionTimeout=function(tm){if(tm>=1){_sessionTimeout=tm;}}
_getSessionTimeout=function(){return _sessionTimeout;}
_sessionVariableExists=function(variableName){return JsonDataBinding.cookieExists(variableName);}
_setSessionVariable=function(variableName,value){JsonDataBinding.setCookie(variableName,value,_sessionTimeout);_startSessionWatcher()}
_initSessionVariable=function(variableName,value){if(!JsonDataBinding.cookieExists(variableName)){JsonDataBinding.setCookie(variableName,value,_sessionTimeout);}
_startSessionWatcher();}
_getSessionVariable=function(variableName){var v=JsonDataBinding.getCookie(variableName);return v;}
_eraseSessionVariable=function(variableName){JsonDataBinding.eraseCookie(variableName);}
_getSessionVariables=function(){var aret=new Array();if(JsonDataBinding.UseLocalStore){if(window.localStorage){var gv=window.localStorage['limnor_gv'];if(gv&&gv.length>0){var vs=JSON.parse(gv);for(var nm in vs){if(nm!=const_userAlias){var vl=vs[nm];if(typeof(vl)!='undefined'&&vl!==null){if(typeof(vl.e)!='undefined'&&vl.e!==null){if(vl.e<new Date()){delete vs[nm];gv=JSON.stringify(vs);window.localStorage['limnor_gv']=gv;continue;}}
aret.push({name:nm,value:vl.v});}}}}}}
else{var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];var pos=c.indexOf('=');if(pos>0){var nm=c.substr(0,pos).replace(/^\s+|\s+$/g,"");if(nm!=const_userAlias){var o={'name':nm,'value':c.substr(pos+1)};aret.push(o);}}}}
return aret;}
_addPageCulture=function(cultureName){if(typeof cultureName=='undefined'||cultureName==null){cultureName='';}
var rowName='row_'+cultureName.replace('-','_');var r=eval(rowName);var idx=resTable.Rows.push(r)-1;if(sources[resTable.TableName]){sources[resTable.TableName].rowIndex=idx;_onRowIndexChange(resTable.TableName);}
else{var v=new Object();v.Tables=new Array();v.Tables.push(resTable);_setDataSource.call(v);}}
_getValueInCurrentCulture=function(valueName){if(typeof valueName!='undefined'&&valueName!=null){valueName=valueName.toLowerCase();var idx=sources[resTable.TableName]?sources[resTable.TableName].rowIndex:0;if(idx<0||idx>=resTable.Rows.length){idx=0;}
if(idx<resTable.Rows.length){for(var c=0;c<resTable.Columns.length;c++){if(valueName==resTable.Columns[c].Name){return resTable.Rows[idx].ItemArray[c];}}}}}
_setCulture=function(cultureName){_cultureName=cultureName;if(typeof _cultureName=='undefined'||_cultureName==null){_cultureName='';}
JsonDataBinding.setCookie(jsdb_cultureName,cultureName,99999);var idx=-1;for(var i=0;i<resTable.Rows.length;i++){if(resTable.Rows[i].ItemArray[0]==cultureName){idx=i;break;}}
if(idx<0){var sPath=window.location.pathname;var sPage=sPath.substring(sPath.lastIndexOf('/')+1);sPage=sPage.substring(0,sPage.lastIndexOf('.'));var element1=document.createElement('script');if(_cultureName==''){element1.src='libjs/'+sPage+'.js';}
else{element1.src=cultureName+'/'+sPage+'.js';}
element1.type='text/javascript';element1.async=false;document.getElementsByTagName('head')[0].appendChild(element1);}
else{sources[resTable.TableName].rowIndex=idx;_onRowIndexChange(resTable.TableName);}}
_getCulture=function(){return JsonDataBinding.getCookie(jsdb_cultureName);}
_addPageResourceName=function(resName,resType){resTable.Columns.push({'Name':resName,'ReadOnly':'true','Type':resType});}
_setUserLogCookieName=function(nm){const_userAlias=nm;}
_getloginValue=function(vid,defVal){if(const_userAlias=='logonUserAlias'){var u=JsonDataBinding.getCookieByStartsWith('WebLgin');if(u.length>0){var uu=u[0].value.split(' ');if(uu.length>vid){return uu[vid];}}}
else{var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){var uu=u.split(' ');if(uu.length>vid){return uu[vid];}}}}
return defVal;}
_getCurrentUserAlias=function(){var u=_getloginValue(0,null);if(u!=null){u=JsonDataBinding.replaceAll(u,'#nbsp#',' ',false);}
return u;}
_getCurrentUserLevel=function(){return _getloginValue(1,-1);}
_getCurrentUserID=function(){return _getloginValue(2,0);}
_userLoggedOn=function(){if(const_userAlias=='logonUserAlias'){var u=JsonDataBinding.getCookieByStartsWith('WebLgin');return(u.length>0);}
else{var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){return true;}}}
return false;}
var _eventFirer;_setEventFirer=function(eo){_eventFirer=eo;}
_setServerPage=function(pageUrl){jsdb_serverPage=pageUrl;}
_getServerPage=function(){return jsdb_serverPage;}
_addOnRowIndexChangeHandler=function(tableName,handler){if(typeof handlersOnRowIndex=='undefined'||handlersOnRowIndex==null){handlersOnRowIndex=new Object();}
if(typeof handlersOnRowIndex[tableName]=='undefined'){handlersOnRowIndex[tableName]=new Array();}
handlersOnRowIndex[tableName].push(handler);}
_hasLoggedOn=function(){if(const_userAlias=='logonUserAlias'){var u=JsonDataBinding.getCookieByStartsWith('WebLgin');if(u.length>0){return 2;}}
else{var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){if(typeof JsonDataBinding.TargetUserLevel!='undefined'&&JsonDataBinding.TargetUserLevel!=null&&JsonDataBinding.TargetUserLevel>=0){var uu=u.split(' ');if(uu.length>1){if(parseInt(uu[1])<=JsonDataBinding.TargetUserLevel){return 2;}
else{return 1;}}
else{return 1;}}
else{return 2;}}}}
return 0;}
_logOff=function(){if(typeof activityWatcher!='undefined'&&activityWatcher!=null){clearTimeout(activityWatcher);}
activityWatcher=null;if(const_userAlias=='logonUserAlias'){var u=JsonDataBinding.getCookieByStartsWith('WebLgin');for(var i=0;i<u.length;i++){JsonDataBinding.eraseCookie(u[i].name);}
window.location.reload();}
else{var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){JsonDataBinding.eraseCookie(const_userAlias);window.location.reload();}}}}
_loginPassed=function(login,expire,userLevel,userid){login=JsonDataBinding.replaceAll(login,' ','#nbsp#',false);if(userLevel){if(expire){if(userid){JsonDataBinding.setCookie(const_userAlias,login+' '+userLevel+' '+userid+' '+expire,expire);}
else{JsonDataBinding.setCookie(const_userAlias,login+' '+userLevel+' 0 '+expire,expire);}}
else{if(userid){JsonDataBinding.setCookie(const_userAlias,login+' '+userLevel+' '+userid,null);}
else{JsonDataBinding.setCookie(const_userAlias,login+' '+userLevel+' 0',null);}}}
else{if(expire){if(userid){JsonDataBinding.setCookie(const_userAlias,login+' 0 '+userid+' '+expire,expire);}
else{JsonDataBinding.setCookie(const_userAlias,login+' 0 0 '+expire,expire);}}
else{if(userid){JsonDataBinding.setCookie(const_userAlias,login+' 0 '+userid,null);}
else{JsonDataBinding.setCookie(const_userAlias,login+' 0 0',null);}}}
_setupLoginWatcher();_executeClientEventObject('UserLogin');}
function addloader(func){var oldonload=window.onload;if(typeof window.onload!='function'){window.onload=func;}else{window.onload=function(){if(oldonload){oldonload();}
func();}}}
function addMouseWatcher(func){var oldonload=document.body.onmousemove;if(typeof document.body.onmousemove!='function'){document.body.onmousemove=func;}
else{document.body.onmousemove=function(){if(oldonload){oldonload();}
func();}}}
function addKeyboardWatcher(func){var oldonload=document.body.onkeydown;if(typeof document.body.onkeydown!='function'){document.body.onkeydown=func;}
else{document.body.onkeydown=function(){if(oldonload){oldonload();}
func();}}}
_setupLoginWatcher=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u=='undefined'||u==null){return;}
if(u.length==0){return;}
addKeyboardWatcher(function(){hasActivity=true;});addMouseWatcher(function(){hasActivity=true;});activityWatcher=setTimeout(activity,3000);}
_columnNameToIndex=function(tablename,columnname){if(sources[tablename]){return sources[tablename].columnIndexes[columnname.toLowerCase()];}}
_setDataSource=function(dataAttrs){var v=this;if(typeof v!='undefined'&&v!=null&&typeof v.Data!='undefined'){v=v.Data;}
if(typeof v!='undefined'&&v!=null&&typeof v.Tables!='undefined'&&v.Tables!=null){var name;var dataIgnore={};for(var i=0;i<v.Tables.length;i++){name=v.Tables[i].TableName;var isFirstTime=true;var isDataStreaming=false;var streamStatus;var utc=_getTableAttribute(name,'AutoConvertUTCforWeb');if(dataAttrs&&dataAttrs[name]&&dataAttrs[name].streamId){isDataStreaming=true;streamStatus=JsonDataBinding.getTableAttribute(name,'batchStatus');if(!streamStatus){streamStatus={};streamStatus.streamId=dataAttrs[name].streamId;streamStatus.functionName=dataAttrs[name].functionName;JsonDataBinding.setTableAttribute(name,'batchStatus',streamStatus);}
if(!streamStatus.batchSize){if(v.Tables[i].Rows.length>0){streamStatus.batchSize=v.Tables[i].Rows.length;}
else{streamStatus.batchSize=100;}}
if(dataAttrs[name].isFirstBatch){streamStatus.streamId=dataAttrs[name].streamId;streamStatus.functionName=dataAttrs[name].functionName;}
else{if(streamStatus.functionName!=dataAttrs[name].functionName||streamStatus.streamId!=dataAttrs[name].streamId){dataIgnore[name]=true;}
isFirstTime=false;}
if(!dataIgnore[name]){streamStatus.batchKey=dataAttrs[name].batchKey;streamStatus.serverComponentName=dataAttrs[name].serverComponentName;streamStatus.parameters=dataAttrs[name].parameters;}}
else{if(JsonDataBinding.values.isdatastreaming&&JsonDataBinding.values.isdatastreaming.length>0){for(var k=0;k<JsonDataBinding.values.isdatastreaming.length;k++){if(JsonDataBinding.values.isdatastreaming[k]==name){isDataStreaming=true;isFirstTime=false;break;}}}
if(!isDataStreaming){var dstreaming=_getTableAttribute(name,'isDataStreaming')
if(typeof dstreaming!='undefined'&&dstreaming!=null){isDataStreaming=dstreaming;var isf=_getTableAttribute(name,'isFisrtTime')
if(typeof isf!='undefined'&&isf!=null){isFirstTime=isf;}}}}
if(!dataIgnore[name]){var j,r;var hasBlob=false;var blobFields=new Array();var isFieldImages=JsonDataBinding.getObjectProperty(name,'IsFieldImage');for(j=0;j<v.Tables[i].Columns.length;j++){if(v.Tables[i].Columns[j].Type==252){if(isFieldImages&&isFieldImages.length>j){if(isFieldImages[j]){continue;}}
hasBlob=true;blobFields.push(j);}}
if(hasBlob||utc){for(r=0;r<v.Tables[i].Rows.length;r++){if(hasBlob){for(j=0;j<blobFields.length;j++){v.Tables[i].Rows[r].ItemArray[blobFields[j]]=JsonDataBinding.decodeBase64(v.Tables[i].Rows[r].ItemArray[blobFields[j]]);}}
if(utc){for(j=0;j<v.Tables[i].Columns.length;j++){if(v.Tables[i].Columns[j].Type==12){v.Tables[i].Rows[r].ItemArray[j]=JsonDataBinding.datetime.toLocalDate(v.Tables[i].Rows[r].ItemArray[j]);}}}}}
if(isDataStreaming&&!isFirstTime&&sources[name]){if(sources[name].Rows){sources[name].newRowStartIndex=sources[name].Rows.length;}
else{sources[name].Rows=new Array();sources[name].newRowStartIndex=0;}
for(r=0;r<v.Tables[i].Rows.length;r++){sources[name].Rows.push(v.Tables[i].Rows[r]);}}
else{sources[name]=v.Tables[i];sources[name].columnIndexes=new Object();sources[name].rowIndex=0;for(j=0;j<sources[name].Columns.length;j++){sources[name].columnIndexes[sources[name].Columns[j].Name.toLowerCase()]=j;sources[name].columnIndexes[sources[name].Columns[j].Name]=j;}
var readonlyfields=_getTableAttribute(name,'readonlyfields');if(readonlyfields&&readonlyfields.length>0){for(j=0;j<readonlyfields.length;j++){var jc0=sources[name].columnIndexes[readonlyfields[j]];if(jc0>=0&&jc0<sources[name].Columns.length){sources[name].Columns[jc0].ReadOnly=true;}}}}}}
for(var k=0;k<v.Tables.length;k++){name=v.Tables[k].TableName;if(!dataIgnore[name]){_setTableAttribute(name,'IsDataReady',false);bindTable(name,isFirstTime,isDataStreaming);_executeEventHandlers('DataArrived',name,dataAttrs,{isFirstTime:isFirstTime});if(isFirstTime||!isDataStreaming){if(v.Tables[k].Rows&&v.Tables[k].Rows.length>0){_executeEventHandlers('CurrentRowIndexChanged',name);}}
_setTableAttribute(name,'IsDataReady',true);}}
for(var k=0;k<v.Tables.length;k++){name=v.Tables[k].TableName;if(!dataIgnore[name]){if(dataAttrs&&dataAttrs[name]&&dataAttrs[name].streamId){streamStatus=JsonDataBinding.getTableAttribute(name,'batchStatus');if(streamStatus&&streamStatus.batchKey&&v.Tables[k].Rows.length>=streamStatus.batchSize){var obj={};if(dataAttrs[name].uploadedValues){for(var nm in dataAttrs[name].uploadedValues){if(dataAttrs[name].uploadedValues.hasOwnProperty(nm)){obj[nm]=dataAttrs[name].uploadedValues[nm];}}}
if(streamStatus.parameters){for(var nm in streamStatus.parameters){if(streamStatus.parameters.hasOwnProperty(nm)){obj[nm]=streamStatus.parameters[nm];}}}
if(dataAttrs[name].batchWhere){obj.batchWhere=dataAttrs[name].batchWhere;}
if(dataAttrs[name].batchWhereParams){obj.batchparameters=dataAttrs[name].batchWhereParams;}
obj.batchStreamId=streamStatus.streamId;obj.serverComponentName=streamStatus.serverComponentName;if(streamStatus.batchDelay>0){function fetchnextbatch(){_executeServerCommands([{method:streamStatus.functionName,value:streamStatus.batchKey}],obj,null,{background:true});}
setTimeout(fetchnextbatch,streamStatus.batchDelay);}
else{_executeServerCommands([{method:streamStatus.functionName,value:streamStatus.batchKey}],obj,null,{background:true});}}}}}}}
bindData=function(e,name,firstTime,isDataStreaming){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){var pchld=false;if(typeof a.getAttribute!='undefined'){var bd=a.getAttribute(jsdb_bind);if(typeof bd!='undefined'&&bd!=null&&bd!=''){var binds=bd.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var dbTable=bind[0];if(dbTable==name){if(a.isTreeView){if(firstTime||isDataStreaming){a.jsData.onDataReady(sources[name]);}
else{a.jsData.onRowIndexChange(name);}}
else{if(bind.length==1){if(firstTime){if(a.IsDataRepeater){if(typeof a.jsData=='undefined'){a.jsData=JsonDataBinding.DataRepeater(a,sources[name]);}
else{a.jsData.onDataReady(sources[name]);}}
else{if(typeof a.tagName!='undefined'&&a.tagName!=null){if(a.tagName.toLowerCase()=="table"){if(a.chklist){a.chklist.loadRecords(sources[name].Rows);a.chklist.setMessage('');a.chklist.applyTargetdata();}
else{if(typeof a.jsData=='undefined'){a.jsData=JsonDataBinding.HtmlTableData(a,sources[name]);}
else{a.jsData.onDataReady(sources[name]);}}}}}}
else{if(a.IsDataRepeater){if(a.jsData){a.jsData.onPageIndexChange(name);}}
else{if(typeof a.tagName!='undefined'&&a.tagName!=null){if(a.tagName.toLowerCase()=="table"){if(typeof a.jsData!='undefined'&&a.jsData!=null){a.jsData.onRowIndexChange(name);}}}}}}
else if(bind.length==3){var isListbox=false;var isfieldset=false;if(typeof a.tagName!='undefined'&&a.tagName!=null){var tag=a.tagName.toLowerCase();isfieldset=(tag=='fieldset');isListbox=(tag=="select");}
if(isListbox){var itemField=bind[1];var valueField=bind[2];var itemFieldIdx=-1;var valueFieldIdx=-1;for(var c=0;c<sources[name].Columns.length;c++){if(sources[name].Columns[c].Name==itemField){itemFieldIdx=c;}
if(sources[name].Columns[c].Name==valueField){valueFieldIdx=c;}
if(valueFieldIdx>=0&&itemFieldIdx>=0){break;}}
if(valueFieldIdx<0){if(itemFieldIdx>=0){valueFieldIdx=itemFieldIdx;}}
if(itemFieldIdx<0){if(valueFieldIdx>=0){itemFieldIdx=valueFieldIdx;}}
if(itemFieldIdx>=0){if(firstTime){if(typeof a.jsData=='undefined'){a.jsData=JsonDataBinding.HtmlListboxData(a,sources[name],itemFieldIdx,valueFieldIdx);}
else{a.jsData.onDataReady(sources[name]);}}
else{if(typeof a.jsData!='undefined'&&a.jsData!=null){a.jsData.onRowIndexChange(name);}}}}
else{var field=bind[1];var target=bind[2];var ci=_columnNameToIndex(name,field);var b=(typeof a.disableMonitor=='undefined')?false:a.disableMonitor;a.disableMonitor=true;if(sources[name].rowIndex>=0&&sources[name].rowIndex<sources[name].Rows.length){if(target=='innerText'){JsonDataBinding.SetInnerText(a,sources[name].Rows[sources[name].rowIndex].ItemArray[ci]);}
else if(target=='ImageData'){JsonDataBinding.SetImageData(a,sources[name].Rows[sources[name].rowIndex].ItemArray[ci]);}
else{var lg=null;if(isfieldset){for(var ai=0;ai<a.children.length;ai++){if(a.children[ai].tagName.toLowerCase()=='legend'){lg=a.children[ai];break;}}}
if(sources[name].Rows[sources[name].rowIndex].ItemArray[ci]==null){a[target]=JsonDataBinding.NullDisplay;a.nullDisplayEmpty=true;if(lg){lg.innerHTML='';}}
else{var v0=JsonDataBinding.toText(sources[name].Rows[sources[name].rowIndex].ItemArray[ci]);a[target]=v0;if(lg){lg.innerHTML=v0;}}}
a.val=JsonDataBinding.toText(sources[name].Rows[sources[name].rowIndex].ItemArray[ci]);}
else{if(target=='innerText'){JsonDataBinding.SetInnerText(a,'');}
else if(target=='ImageData'){JsonDataBinding.SetImageData(a);}
else{a[target]='';}
a.val='';}
a.disableMonitor=b;if(typeof a.jsdbRowIndex=='undefined'){a.jsdbRowIndex={}}
a.jsdbRowIndex[name]=sources[name].rowIndex;if(firstTime){var tag=a.tagName.toLowerCase();if(target=='innerHTML'){if(tag=='div'){a.oninnerHtmlChanged=changeBoundData;JsonDataBinding.addTextBoxObserver(a);}}
else if(tag=='input'&&target=='checked'){a.isCheckBox=true;JsonDataBinding.AttachEvent(a,'onclick',changeBoundData);a.onCheckedChanged=changeBoundData;JsonDataBinding.addTextBoxObserver(a);}
else if(tag=='input'||(tag=='textarea')){JsonDataBinding.AttachEvent(a,e_onchange,changeBoundData);a.onsetbounddata=changeBoundData;JsonDataBinding.addTextBoxObserver(a);}}
if(isfieldset){bindData(a,name,firstTime,isDataStreaming);pchld=true;}}}}}}}
else{bindData(a,name,firstTime,isDataStreaming);pchld=true;}}
if(name=='_pageResources_'){if(a.ActControls&&a.ActControls.length>0){var ps={childNodes:new Array()};for(var k=0;k<a.ActControls.length;k++){if(a.ActControls[k]){var c=document.getElementById(a.ActControls[k]);if(c){var pexists=false;for(var k2=0;k2<ps.childNodes.length;k2++){if(ps.childNodes[k2]===c){pexists=true;break;}}
if(!pexists){ps.childNodes.push(c);}}}}
bindData(ps,name,firstTime,isDataStreaming);}
else{if(!pchld){if(a.tagName&&a.tagName.toLowerCase()=='div'){bindData(a,name,firstTime,isDataStreaming);}}}}}}};function bindTable(name,firstTime,isDataStreaming){bindData(document.body,name,firstTime,isDataStreaming);}
function refreshTableBindColumnDisplay(e,name,rowidx,colIdx){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){if(typeof a.getAttribute!='undefined'){var bd=a.getAttribute(jsdb_bind);if(typeof bd!='undefined'&&bd!=null&&bd!=''){var binds=bd.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var dbTable=bind[0];if(dbTable==name){if(a.jsData&&a.jsData.refreshBindColumnDisplay){a.jsData.refreshBindColumnDisplay(name,rowidx,colIdx);}
else if(bind.length==3){if(rowidx==sources[name].rowIndex){var field=bind[1];var target=bind[2];var ci=_columnNameToIndex(name,field);var b=(typeof a.disableMonitor=='undefined')?false:a.disableMonitor;a.disableMonitor=true;if(rowidx<sources[name].Rows.length){if(target=='innerText'){JsonDataBinding.SetInnerText(a,sources[name].Rows[rowidx].ItemArray[ci]);}
else if(target=='ImageData'){JsonDataBinding.SetImageData(a,sources[name].Rows[rowidx].ItemArray[ci]);}
else{if(sources[name].Rows[rowidx].ItemArray[ci]==null){a[target]=JsonDataBinding.NullDisplay;a.nullDisplayEmpty=true;}
else{a[target]=JsonDataBinding.toText(sources[name].Rows[rowidx].ItemArray[ci]);}}
a.val=JsonDataBinding.toText(sources[name].Rows[rowidx].ItemArray[ci]);}
a.disableMonitor=b;}}}}}
else{refreshTableBindColumnDisplay(a,name,rowidx,colIdx);}}}}}
function refreshBindColumnDisplay(name,rowidx,colIdx){refreshTableBindColumnDisplay(document.body,name,rowidx,colIdx);}
function getNextRowIndex(name,currentIndex){var idx2=-1;var idx=currentIndex+1;while(idx<sources[name].Rows.length){if(!sources[name].Rows[idx].deleted&&!sources[name].Rows[idx].removed){idx2=idx;break;}
idx++;}
return idx2;}
function getPreviousRowIndex(name,currentIndex){var idx2=-1;var idx=currentIndex-1;while(idx>=0){if(!sources[name].Rows[idx].deleted&&!sources[name].Rows[idx].removed){idx2=idx;break;}
idx--;}
return idx2;}
function onRowIndexChange(name){if(typeof sources!='undefined'&&typeof sources[name]!='undefined'){if(typeof handlersOnRowIndex!='undefined'){if(handlersOnRowIndex[name]!=null){for(var i=0;i<handlersOnRowIndex[name].length;i++){handlersOnRowIndex[name][i](sources[name]);}}}
_executeEventHandlers('CurrentRowIndexChanged',name);}}
_bindData=function(e,name,firstTime,isDataStreaming){bindData(e,name,firstTime,isDataStreaming);}
_onRowIndexChange=function(name){bindData(document.body,name,false);onRowIndexChange(name);}
_dataMoveToRecord=function(name,rowIndex){if(sources&&sources[name]&&rowIndex>=0&&rowIndex<sources[name].Rows.length){JsonDataBinding.pollModifications();sources[name].rowIndex=rowIndex;_onRowIndexChange(name);return true;}
return false;}
_dataMoveFirst=function(name){if(sources&&sources[name]){JsonDataBinding.pollModifications();var idx2=getNextRowIndex(name,-1);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);return true;}}
return false;}
_dataMoveLast=function(name){if(sources&&sources[name]){JsonDataBinding.pollModifications();var idx2=getPreviousRowIndex(name,sources[name].Rows.length);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);return true;}}
return false;}
_dataMoveNext=function(name){if(sources&&typeof sources[name]!='undefined'&&sources[name].rowIndex<sources[name].Rows.length-1){JsonDataBinding.pollModifications();var idx2=getNextRowIndex(name,sources[name].rowIndex);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);return true;}}
return false;}
_dataMovePrevious=function(name){if(sources&&typeof sources[name]!='undefined'&&sources[name].rowIndex<sources[name].Rows.length&&sources[name].rowIndex>0){JsonDataBinding.pollModifications();var idx2=getPreviousRowIndex(name,sources[name].rowIndex);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);return true;}}
return false;}
_clearTableRows=function(name){if(sources&&typeof sources[name]!='undefined'){sources[name].Rows=[];sources[name].rowIndex=-1;bindData(document.body,name,true,false);}}
_getModifiedRowCount=function(name){JsonDataBinding.pollModifications();var r0=0;if(sources&&sources[name]){if(typeof sources[name]!='undefined'){for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].changed){r0++;}}}}
return r0;}
_getDeletedRowCount=function(name){var r0=0;if(sources&&sources[name]){if(typeof sources[name]!='undefined'){for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].deleted){r0++;}}}}
return r0;}
_getActiveRowCount=function(name){var r0=0;if(sources&&sources[name]){if(typeof sources[name]!='undefined'){r0=sources[name].Rows.length;for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].deleted){r0--;}}}}
return r0;}
_getNewRowCount=function(name){var r0=0;if(sources&&sources[name]){for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].added){r0++;}}}
return r0;}
_columnValueByIndex=function(name,ci,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
else{rowIndex=parseInt(rowIndex);}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0&&ci>=0&&ci<sources[name].Columns.length){if(sources[name].Columns[ci].Type==12){if(sources[name].Rows[rowIndex].ItemArray[ci]){if(typeof(sources[name].Rows[rowIndex].ItemArray[ci])=='string'){sources[name].Rows[rowIndex].ItemArray[ci]=JsonDataBinding.datetime.parseIso(sources[name].Rows[rowIndex].ItemArray[ci]);}}
else{return new Date(0);}}
else if(sources[name].Columns[ci].Type==11){if(sources[name].Rows[rowIndex].ItemArray[ci]){if(typeof(sources[name].Rows[rowIndex].ItemArray[ci])=='string'){var ts=new JsonDataBinding.timespan();ts.parseIsoString(sources[name].Rows[rowIndex].ItemArray[ci]);sources[name].Rows[rowIndex].ItemArray[ci]=ts;}}
else{return new JsonDataBinding.timespan();}}
return sources[name].Rows[rowIndex].ItemArray[ci];}}
return null;}
_columnValue=function(name,columnName,rowIndex){var ci=_columnNameToIndex(name,columnName);return _columnValueByIndex(name,ci,rowIndex);}
_isColumnValueNull=function(name,columnName,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){var ci=_columnNameToIndex(name,columnName);if(typeof sources[name].Rows[rowIndex].ItemArray[ci]=='undefined'||sources[name].Rows[rowIndex].ItemArray[ci]==null){return true;}
else{return false;}}}
return true;}
_isColumnValueNullOrEmpty=function(name,columnName,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){var ci=_columnNameToIndex(name,columnName);if(typeof sources[name].Rows[rowIndex].ItemArray[ci]=='undefined'||sources[name].Rows[rowIndex].ItemArray[ci]==null){return true;}
else{if(typeof sources[name].Rows[rowIndex].ItemArray[ci]=='string'){return(sources[name].Rows[rowIndex].ItemArray[ci].length==0);}
else{return false;}}}}
return true;}
_isColumnValueNotNull=function(name,columnName,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){var ci=_columnNameToIndex(name,columnName);if(typeof sources[name].Rows[rowIndex].ItemArray[ci]=='undefined'||sources[name].Rows[rowIndex].ItemArray[ci]==null){return false;}
else{return true;}}}
return false;}
_isColumnValueNotNullOrEmpty=function(name,columnName,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){var ci=_columnNameToIndex(name,columnName);if(typeof sources[name].Rows[rowIndex].ItemArray[ci]=='undefined'||sources[name].Rows[rowIndex].ItemArray[ci]==null){return false;}
else{if(typeof sources[name].Rows[rowIndex].ItemArray[ci]=='string'){return(sources[name].Rows[rowIndex].ItemArray[ci].length>0);}
else{return true;}}}}
return false;}
_setcolumnValue=function(name,columnName,val0,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){var columnIndex=_columnNameToIndex(name,columnName);var val=val0;sources[name].Rows[rowIndex].ItemArray[columnIndex]=val;sources[name].Rows[rowIndex].changed=true;_onRowIndexChange(name);JsonDataBinding.onvaluechanged(sources[name],rowIndex,columnIndex,val);}}}
_getColExpvalue=function(name,expression,idx){if(sources&&sources[name]){var exp=expression;for(var i=0;i<sources[name].Columns.length;i++){exp=exp.replace(new RegExp("{"+sources[name].Columns[i].Name+"}","gi"),sources[name].Rows[idx].ItemArray[i]);}
return eval(exp);}
return null;}
_columnExpressionValue=function(name,expression,rowIndex){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){return _getColExpvalue(name,expression,rowIndex);}}
return null;}
_columnSum=function(name,fieldName){return _statistics(name,fieldName,'SUM');}
_statistics=function(name,expression,operator){if(sources&&sources[name]){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
var sum=0.0,idx,i,m,v;if(operator=='SUM'){for(idx=0;idx<sources[name].Rows.length;idx++){sum=sum+_getColExpvalue(name,expression,idx);}
return sum;}
else if(operator=="AVG"){if(sources[name].Rows.length>0){for(idx=0;idx<sources[name].Rows.length;idx++){var exp=expression;for(i=0;i<sources[name].columnIndexes.length;i++){exp=exp.replace(new RegExp("{"+sources[name].Columns[i].Name+"}","gi"),sources[name].Rows[idx].ItemArray[_columnNameToIndex(name,sources[name].Columns[i].Name)]);}
sum=sum+_getColExpvalue(name,expression,idx);}
return sum/sources[name].Rows.length;}
return sum;}
else if(operator=="MIN"){if(sources[name].Rows.length>0){idx=0;m=_getColExpvalue(name,expression,idx);for(idx=1;idx<sources[name].Rows.length;idx++){v=_getColExpvalue(name,expression,idx);if(v<m){m=v;}}
return m;}}
else if(operator=="MAX"){if(sources[name].Rows.length>0){idx=0;m=_getColExpvalue(name,expression,idx);for(idx=1;idx<sources[name].Rows.length;idx++){v=_getColExpvalue(name,expression,idx);if(v>m){m=v;}}
return m;}}}
return null;}
_toNumber=function(v){if(typeof v=='undefined'||v==null)
return 0;return v;}
_sortOnColumn=function(name,columnName,sortAsc,ignoreCase){if(sources&&sources[name]&&sources[name].Rows.length>0){var columnIndex=_columnNameToIndex(name,columnName);if(sources[name].Columns[columnIndex].Type==16){sources[name].Rows.sort(function(a,b){var a0=_toNumber(a.ItemArray[columnIndex]);var b0=_toNumber(b.ItemArray[columnIndex]);if(a0<b0){if(sortAsc)
return-1;else
return 1;}
else if(a0>b0){if(sortAsc)
return 1;else
return-1;}
return 0;});}
else if(sources[name].Columns[columnIndex].Type<10||sources[name].Columns[columnIndex].Type==246){sources[name].Rows.sort(function(a,b){var a0=_toNumber(a.ItemArray[columnIndex]);var b0=_toNumber(b.ItemArray[columnIndex]);if(sortAsc)
return a0-b0;else
return b0-a0;});}
else if(sources[name].Columns[columnIndex].Type<16){sources[name].Rows.sort(function(a,b){var a0=JsonDataBinding.datetime.toDate0(a.ItemArray[columnIndex]);var b0=JsonDataBinding.datetime.toDate0(b.ItemArray[columnIndex]);if(sortAsc)
return a0-b0;else
return b0-a0;});}
else{sources[name].Rows.sort(function(a,b){var a0=(typeof a.ItemArray[columnIndex]=='undefined'||a.ItemArray[columnIndex]==null)?'':a.ItemArray[columnIndex];var b0=(typeof b.ItemArray[columnIndex]=='undefined'||b.ItemArray[columnIndex]==null)?'':b.ItemArray[columnIndex];if(ignoreCase){a0=a0.toLowerCase();b0=b0.toLowerCase();}
if(a0==b0)
return 0;if(sortAsc){if(a0>b0)
return 1;else
return-1;}
else{if(b0>a0)
return 1;else
return-1;}});}
bindTable(name,true,false);}}
_deleteCurrentRow=function(name){if(sources&&sources[name]){var idx=sources[name].rowIndex;if(idx>=0&&idx<sources[name].Rows.length){preserveKeys(name);sources[name].Rows[idx].deleted=true;var idx2=getNextRowIndex(name,idx);if(idx2<0){idx2=getPreviousRowIndex(name,idx);}
if(typeof onrowdeletehandlers[name]!='undefined'){for(var i=0;i<onrowdeletehandlers[name].length;i++){onrowdeletehandlers[name][i](name,idx);}}
sources[name].rowIndex=idx2;bindData(document.body,name,false);}}}
_getCurrentRowIndex=function(name){if(sources&&sources[name]){return sources[name].rowIndex;}
return-1;}
_getRowCount=function(name){if(sources&&sources[name]){return sources[name].Rows.length;}
return 0;}
_addRow=function(name){if(sources&&sources[name]){var r=new Object();r.added=true;r.ItemArray=new Array();for(var i=0;i<sources[name].Columns.length;i++){if(sources[name].Columns[i].isAutoNumber){r.ItemArray[i]=-Math.floor(Math.random()*1000000);}
else{r.ItemArray[i]=null;}}
var idx=sources[name].Rows.length;sources[name].Rows[idx]=r;sources[name].rowIndex=idx;bindData(document.body,name,false);return idx;}
return-1;}
_resetDataStreaming=function(name){_setTableAttribute(name,'isDataStreaming',false);}
_saveEdsToOffline=function(name,dataName){if(window.localStorage){if(sources[name]){var n='limnoreds_'+dataName;var vstr=JSON.stringify(sources[name]);window.localStorage[n]=vstr;}}}
_loadEdsFromOffline=function(name,dataName){if(window.localStorage){var n='limnoreds_'+dataName;var vstr=window.localStorage[n];if(typeof vstr!='undefined'){var data=JSON.parse(vstr);sources[name]=data;_bindData(document.body,name,true,false);}}}
_removeEdsFromOffline=function(dataName){if(window.localStorage){var n='limnoreds_'+dataName;if(typeof window.localStorage[n]!='undefined'){delete window.localStorage[n];}}}
function preserveKeys(name){var tbl=sources[name];if(tbl.rowIndex>=0&&tbl.rowIndex<tbl.Rows.length){if(tbl.PrimaryKey!=null&&tbl.PrimaryKey.length>0){if(!tbl.Rows[tbl.rowIndex].changed&&!tbl.Rows[tbl.rowIndex].deleted&&!tbl.Rows[tbl.rowIndex].removed){if(typeof tbl.Rows[tbl.rowIndex].KeyValues=='undefined'){tbl.Rows[tbl.rowIndex].KeyValues=new Array();for(var k=0;k<tbl.PrimaryKey.length;k++){var ci=_columnNameToIndex(tbl.TableName,tbl.PrimaryKey[k]);tbl.Rows[tbl.rowIndex].KeyValues[k]=tbl.Rows[tbl.rowIndex].ItemArray[ci];}}}}}}
function changeBoundData(e){var a;var rIdx;var rIdxs;if(e&&typeof e.jsdbRowIndex!='undefined'){rIdxs=e.jsdbRowIndex;}
if(e&&typeof e.onsetbounddata=='function'){a=e;}
if(!a){a=getEventSender(e);if(!a){a=e;}}
if(a){if(typeof rIdx=='undefined'){if(typeof a.jsdbRowIndex!='undefined'){rIdxs=a.jsdbRowIndex;}}
var dbs=a.getAttribute(jsdb_bind);if(typeof dbs!='undefined'&&dbs!=null&&dbs!=''){var binds=dbs.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var sourceName=bind[0];var tbl=sources[sourceName];if(typeof tbl!='undefined'){var field=bind[1];var target=bind[2];if(target=='checked'||target=='value'||target=='innerHTML'||target=='innerText'){if(rIdxs){rIdx=rIdxs[sourceName];}
var rIdx0=tbl.rowIndex;if(typeof rIdx=='undefined'){rIdx=rIdx0;}
if(rIdx>=0&&rIdx<tbl.Rows.length){tbl.rowIndex=rIdx;preserveKeys(sourceName);var c=_columnNameToIndex(tbl.TableName,field);var v;if(target=='innerText'){v=JsonDataBinding.GetInnerText(a);}
else{v=a[target];}
tbl.Rows[rIdx].ItemArray[c]=v;tbl.Rows[rIdx].changed=true;JsonDataBinding.onvaluechanged(tbl,rIdx,c,v);tbl.rowIndex=rIdx0;}}}}}}}
_ondataupdatefailed=function(name){_executeEventHandlers('DataUpdateFailed',name);}
_ondataupdated=function(name){for(p in sources){var item=sources[p];if(typeof item!='undefined'&&item!=null&&typeof(item)!=type_func){if(typeof item.TableName=='undefined'){continue;}
if(typeof name!='undefined'){if(name!=item.TableName){continue;}}
var rows=item.Rows;for(var i=0;i<rows.length;i++){if(rows[i].added){rows[i].added=false;}
if(rows[i].changed){rows[i].changed=false;}
if(rows[i].deleted){rows[i].deleted=false;rows[i].removed=true;}}
_executeEventHandlers('DataUpdated',name);}}}
_sendBoundData=function(dataName,clientProperties,commands){JsonDataBinding.pollModifications();var req=new Object();req.Calls=new Array();if(typeof commands!='undefined'){req.Calls=commands;}
if(typeof clientProperties!='undefined'&&clientProperties!=null){req.values=clientProperties;}
req.Data=new Array();var i=0;var c0;for(p in sources){var item=sources[p];if(typeof item!='undefined'&&item!=null&&typeof(item)!=type_func){if(typeof dataName!='undefined'&&dataName!=''&&dataName!=null){if(item.TableName!=dataName){continue;}}
var hasDate=false;var utc=_getTableAttribute(dataName,'AutoConvertUTCforWeb');var hasImage=false;var imageFlags=_getObjectProperty(dataName,'IsFieldImage');var nDim=0;if(imageFlags&&imageFlags.length>0){nDim=Math.min(imageFlags.length,item.Columns.length);for(c0=0;c0<nDim;c0++){if(imageFlags[c0]&&item.Columns[c0].Type==252){hasImage=true;item.Columns[c0].ReadOnly=true;}}}
for(c0=0;c0<item.Columns.length;c0++){if(item.Columns[c0].Type==11||item.Columns[c0].Type==12){hasDate=true;break;}}
req.Data[i]=new Object();for(n in item){var n0=item[n];if(typeof n0!='undefined'&&n0!=null&&typeof(n0)!=type_func){if(n=='Rows'){var rs=n0;var rs2=new Array();var k=0;for(var j=0;j<rs.length;j++){if(rs[j].changed||rs[j].deleted||rs[j].added){if(!(rs[j].deleted&&rs[j].added)){if(hasImage||hasDate){var rowBuf={};for(nr in rs[j]){var nr0=rs[j][nr];if(typeof nr0!='undefined'&&nr0!=null&&typeof(nr0)!=type_func){if(nr=='ItemArray'){rowBuf[nr]=new Array();for(c0=0;c0<nr0.length;c0++){if(c0<nDim&&imageFlags[c0]&&item.Columns[c0].Type==252){rowBuf[nr].push('');}
else{if(typeof(nr0[c0])=='undefined'||nr0[c0]==null){rowBuf[nr].push(null);}
else if(item.Columns[c0].Type==12){if(utc){rowBuf[nr].push(JsonDataBinding.datetime.toIsoUTC(nr0[c0]));}
else{rowBuf[nr].push(JsonDataBinding.datetime.toIso(nr0[c0]));}}
else if(item.Columns[c0].Type==11){if(typeof(nr0[c0].toTimeString)==type_func)
rowBuf[nr].push(nr0[c0].toTimeString());else
rowBuf[nr].push(nr0[c0]);}
else{rowBuf[nr].push(nr0[c0]);}}}}
else{rowBuf[nr]=nr0;}}}
rs2[k++]=rowBuf;}
else{rs2[k++]=rs[j];}}}}
req.Data[i][n]=rs2;}
else{req.Data[i][n]=n0;}}}
i++;}}
_callServer(req);}
_submitBoundData=function(){_sendBoundData('',null,[{method:jsdb_putdata,value:'0'}]);}
_putData=function(dataName){_sendBoundData(dataName,null,[{method:jsdb_putdata,value:dataName}]);}
_mergeValues=function(vs){var obj={};for(var n in vs){if(vs.hasOwnProperty(n)){JsonDataBinding.values[n]=vs[n];var name;if(JsonDataBinding.startsWith(n,'autoNumList_')){name=n.substr(12);var kvs=vs[n];var tbl=sources[name];if(tbl&&tbl.Rows&&kvs&&kvs.length>0){var ai=-1;for(j=0;j<tbl.Columns.length;j++){if(tbl.Columns[j].isAutoNumber){ai=j;break;}}
if(ai>=0){var kl=kvs.length;var k0=0;for(var r=0;r<tbl.Rows.length;r++){for(var r0=0;r0<kl;r0++){if(kvs[r0].key==tbl.Rows[r].ItemArray[ai]){tbl.Rows[r].ItemArray[ai]=kvs[r0].value;refreshBindColumnDisplay(name,r,ai);k0++;break;}}
if(k0>=kl){break;}}}}}
else if(JsonDataBinding.startsWith(n,'batchSreamID_')){name=n.substr(13);if(!obj[name]){obj[name]={};}
obj[name].streamId=vs[n];}
else if(JsonDataBinding.startsWith(n,'batchFunction_')){name=n.substr(14);if(!obj[name]){obj[name]={};}
obj[name].functionName=vs[n];}
else if(JsonDataBinding.startsWith(n,'batchKey_')){name=n.substr(9);if(!obj[name]){obj[name]={};}
obj[name].batchKey=vs[n];}
else if(JsonDataBinding.startsWith(n,'batchIsFirst_')){name=n.substr(13);if(!obj[name]){obj[name]={};}
obj[name].isFirstBatch=vs[n];}
else if(JsonDataBinding.startsWith(n,'batchObjName_')){name=n.substr(13);if(!obj[name]){obj[name]={};}
obj[name].serverComponentName=vs[n];}
else if(JsonDataBinding.startsWith(n,'batchParameter_')){var name2=n.substr(15);var pos=name2.indexOf('_');if(pos>0){var pa=name2.substr(0,pos);name=name2.substr(pos+1);if(!obj[name]){obj[name]={};}
var ob=obj[name];if(!ob.parameters){ob.parameters={};}
ob.parameters[pa]=vs[n];}}
else if(JsonDataBinding.startsWith(n,'batchWhere_')){name=n.substr(11);if(!obj[name]){obj[name]={};}
obj[name].batchWhere=vs[n];}
else if(JsonDataBinding.startsWith(n,'batchWhereParams_')){name=n.substr(17);if(!obj[name]){obj[name]={};}
obj[name].batchWhereParams=vs[n];}
else if(JsonDataBinding.startsWith(n,'uploadedValues_')){name=n.substr(15);if(!obj[name]){obj[name]={};}
obj[name].uploadedValues=vs[n];}
else if(JsonDataBinding.startsWith(n,'masterfield_')){name2=n.substr(12);var pos=name2.indexOf('_');if(pos>0){var pa=name2.substr(0,pos);name=name2.substr(pos+1);if(!obj[pa]){obj[pa]={};}
var ob=obj[pa];if(!ob.fields){ob.fields={};}
ob.fields[name]=vs[n];}}}}
return obj;}
function _getFormClientHolder(form){if(form){function getfc(f){for(var i=0;i<f.children.length;i++){if(f.children[i].getAttribute('name')=='clientRequest'){return f.children[i];}
else{var e=getfc(f.children[i]);if(e)
return e;}}}
var d=getfc(form);if(d)
return d;}
return document.clientRequest;}
function _getFormMaxSizeHolder(form){if(form){function getfc(f){for(var i=0;i<f.children.length;i++){if(f.children[i].getAttribute('name')=='MAX_FILE_SIZE'){return f.children[i];}
else{var e=getfc(f.children[i]);if(e)
return e;}}}
return getfc(form);}}
var DEBUG_SYMBOL="F3E767376E6546a8A15D97951C849CE5";_processServerResponse=function(r,state,reportError){var v,winDebug;var raw0=r;var pos=r.indexOf(DEBUG_SYMBOL);if(pos>=0||JsonDataBinding.Debug||reportError){var debug;if(pos>=0){debug=r.substring(0,pos);r=r.substring(pos+DEBUG_SYMBOL.length);}
else{debug=r;}
winDebug=JsonDataBinding.OpenDebugWindow();if(winDebug==null){alert('Debug information cannot be displayed. Your web browser has disabled pop-up window');}
else{winDebug.document.write('<h1>Debug Information from ');winDebug.document.write(window.location.pathname);winDebug.document.write('</h1>');winDebug.document.write('<h2>Client request</h2><p>');winDebug.document.write('Client page:');winDebug.document.write(window.location.href);winDebug.document.write('<br>');winDebug.document.write(debug);winDebug.document.write('</p>');winDebug.document.write('<h2>Server response</h2><p>');winDebug.document.write('Server page:');if(state&&state.serverPage){winDebug.document.write(state.serverPage);}
winDebug.document.write('<br>');winDebug.document.write(r);winDebug.document.write('</p>');}}
if(typeof r!='undefined'&&r!=null&&r.length>6){var signCC=r.substr(0,6).toLowerCase();if(signCC=='<html>'){winDebug=JsonDataBinding.OpenDebugWindow();if(winDebug==null){alert('Debug information cannot be displayed. Your web browser has disabled pop-up window');}
else{winDebug.document.documentElement.innerHTML=r;}}
else{for(var k=0;k<r.length;k++){if(r.charAt(k)=='{'){r=r.substring(k);break;}}
pos=r.length-1;while(r.charAt(pos)!='}'){pos--;if(pos<=0){r='{}';break;}}
if(pos>0&&pos<r.length-1){r=r.substr(0,pos+1);}
try{v=JSON.parse(r);}
catch(err){winDebug=JsonDataBinding.OpenDebugWindow();if(winDebug==null){alert('Debug information cannot be displayed. Your web browser has disabled pop-up window');}
else{winDebug.document.write('<h1>Exception Information from ');winDebug.document.write(window.location.pathname);winDebug.document.write('</h1>');winDebug.document.write('Client page:');winDebug.document.write(window.location.href);winDebug.document.write('<br>');winDebug.document.write('Server page:');if(state&&state.serverPage){winDebug.document.write(state.serverPage);}
winDebug.document.write('<br>');if(pos<0){winDebug.document.write('<h2>Client request</h2><p>');if(state&&state.Data){winDebug.document.write(JSON.stringify(state.Data));}
else{var formCust=_getFormClientHolder(JsonDataBinding.SubmittedForm);if(formCust&&formCust.value){pos=formCust.value.indexOf(DEBUG_SYMBOL);var data;if(pos>=0){data=formCust.value.substring(pos+DEBUG_SYMBOL.length);}
else{data=formCust.value;}
winDebug.document.write(data);}}
winDebug.document.write('</p>');winDebug.document.write('<h2>Server response</h2><p>');winDebug.document.write(r);winDebug.document.write('</p>');}
winDebug.document.write('<h2>Json exception</h2><p>');winDebug.document.write('<table>');for(var p in err){winDebug.document.write('<tr><td>');winDebug.document.write(p);winDebug.document.write('</td><td>');winDebug.document.write(err[p]);winDebug.document.write('</td></tr>');}
winDebug.document.write('</table>');winDebug.document.write('</p>');winDebug.document.write('Server response:<br><textarea readonly  rows="30" style="width:90%; ">');winDebug.document.write(raw0);winDebug.document.write('</textarea><br>Json data:<br><textarea readonly  rows="30" style="width:90%; ">');winDebug.document.write(r);winDebug.document.write('</textarea>');}}
if(v){var dataAttrs=_mergeValues(v.values);_serverComponentName=v.serverComponentName;var addednewrecord=JsonDataBinding.values.addednewrecord;var serverFailure=JsonDataBinding.values.serverFailure;if(typeof JsonDataBinding.values.addednewrecord!='undefined'){delete JsonDataBinding.values.addednewrecord;}
if(typeof JsonDataBinding.values.serverFailure!='undefined'){delete JsonDataBinding.values.serverFailure;}
if(JsonDataBinding.SubmittedForm){if(JsonDataBinding.values.SavedFiles){JsonDataBinding.SubmittedForm.SavedFilePaths=JsonDataBinding.values.SavedFiles;}}
if(typeof v.Data!='undefined'){_setDataSource.call(v.Data,dataAttrs);}
if(typeof v.Calls!='undefined'&&v.Calls.length>0){var cf=function(){for(var i=0;i<v.Calls.length;i++){eval(v.Calls[i]);}}
cf.call(v);}
if(typeof JsonDataBinding=='undefined')return;JsonDataBinding.values.isdatastreaming=null;_executeClientEventObject('onProcessServerCall');_executeClientEventObject('ExecuteFinish');_executeClientEventObject('FinishedDataTransfer');if(addednewrecord&&addednewrecord.length>0){for(var i=0;i<addednewrecord.length;i++){_executeEventHandlers('DataUpdated',addednewrecord[i],true);}}
if(_clientEventsHolder&&serverFailure){var eh=_clientEventsHolder['onwebserverreturn'];if(eh){for(var cname in eh){var eho=eh[cname];if(eho&&eho.handlers&&eho.handlers.length>0){for(var i=0;i<eho.handlers.length;i++){eho.handlers[i](serverFailure);}}}}}}}}
if(!JsonDataBinding.AbortEvent&&state&&state.Data&&state.Data.values&&state.Data.values.nextBlock){state.Data.values.nextBlock();}
if(typeof state!='undefined'&&typeof state.JsEventOwner!='undefined'&&state.JsEventOwner!=null){if(typeof state.JsEventOwner.disabled!='undefined'){state.JsEventOwner.disabled=false;}}
else{if(typeof _eventFirer!='undefined'&&_eventFirer!=null){if(typeof _eventFirer.disabled!='undefined'){_eventFirer.disabled=false;_eventFirer=null;}}}
if(JsonDataBinding.ShowAjaxCallWaitingImage){JsonDataBinding.ShowAjaxCallWaitingImage.style.display='none';}
if(JsonDataBinding.ShowAjaxCallWaitingLabel){JsonDataBinding.ShowAjaxCallWaitingLabel.style.display='none';}}
_callServer=function(data,form,execAttrs){if(JsonDataBinding.LogonPage.length>0){if(JsonDataBinding.hasLoggedOn()!=2){var curUrl=JsonDataBinding.getPageFilename();window.location.href=JsonDataBinding.LogonPage+'?'+curUrl;return;}}
var state={};if(JsonDataBinding.ShowAjaxCallWaitingImage){JsonDataBinding.ShowAjaxCallWaitingImage.style.display='';}
if(JsonDataBinding.ShowAjaxCallWaitingLabel){JsonDataBinding.ShowAjaxCallWaitingLabel.style.display='';}
state.Data=data;if(typeof _eventFirer!='undefined'&&_eventFirer!=null){if(typeof _eventFirer.disabled!='undefined'){_eventFirer.disabled=true;state.JsEventOwner=_eventFirer;}}
JsonDataBinding.pageMoveout=false;if(data.values){for(var nm in data.values){if(data.values[nm]&&typeof(data.values[nm].toTimeString)=='function'){data.values[nm]=data.values[nm].toTimeString();}}}
if(form){if(form.submit){var sizeInput=_getFormMaxSizeHolder(form);if(sizeInput){var msize=sizeInput.getAttribute('value');if(typeof msize!='undefined'&&msize!=null&&msize>0){if(!data.values)
data.values={};data.values.allowedFileSize=msize;}}
if(typeof JsonDataBinding.Debug!='undefined'&&JsonDataBinding.Debug){_getFormClientHolder(form).value=DEBUG_SYMBOL+JSON.stringify(data);}
else{_getFormClientHolder(form).value=JSON.stringify(data);}
JsonDataBinding.SubmittedForm=form;state.serverPage=form.action;JsonDataBinding.SubmittedForm.state=state;if(JsonDataBinding.Debug){JsonDataBinding.ShowDebugInfoLine('submit to :'+state.serverPage);}
form.submit();return;}}
state.serverPage=jsdb_serverPage;var xmlhttp;var ajaxWatcher;if(window.XMLHttpRequest){xmlhttp=new XMLHttpRequest();}
else{xmlhttp=new ActiveXObject('Microsoft.XMLHTTP');}
var ajaxCanceled=false;xmlhttp.onreadystatechange=function(){if(xmlhttp.readyState==4){if(typeof(ajaxWatcher)!='undefined'){clearTimeout(ajaxWatcher);}
if(xmlhttp.status==200||xmlhttp.status==500){_processServerResponse(xmlhttp.responseText,state);if(JsonDataBinding.startsWith(xmlhttp.responseText,'PHP ')){var s=xmlhttp.responseText.substr(4).trim();var idx=s.indexOf(':');if(idx>0){s=s.substr(idx+1).trim();if(JsonDataBinding.startsWith(s,'Maximum execution time of')){onajaxtimeout();}
else{if(document.onPhpFatalError){JsonDataBinding.values.ServerError=xmlhttp.responseText;document.onPhpFatalError();}}}}}
else{if(!JsonDataBinding.pageMoveout){if(xmlhttp.status!=0||JsonDataBinding.Debug){if(!ajaxCanceled){_processServerResponse((xmlhttp.status==0?'This web page must be served by a web server, not from a local file system. ':'')+'server call failed with status '+xmlhttp.status+'. '+xmlhttp.responseText,state,true);onajaxtimeout();}}}}}}
var url=jsdb_serverPage+'?timeStamp='+new Date().getTime();if(JsonDataBinding.Debug){JsonDataBinding.ShowDebugInfoLine('send to :'+url);}
xmlhttp.open('POST',url,true);xmlhttp.setRequestHeader('Content-Type','application/x-www-form-urlencoded');if(execAttrs&&execAttrs.headers){for(var i=0;i<execAttrs.headers.length;i++){xmlhttp.setRequestHeader(execAttrs.headers[i].name,execAttrs.headers[i].value);}}
if(JsonDataBinding.AjaxTimeout>0){function onajaxtimeout(){ajaxCanceled=true;xmlhttp.abort();if(document.onAjaxTimeout){document.onAjaxTimeout();}}
ajaxWatcher=setTimeout(onajaxtimeout,JsonDataBinding.AjaxTimeout*1000);}
if(JsonDataBinding.Debug){xmlhttp.send(DEBUG_SYMBOL+JSON.stringify(data));}
else{xmlhttp.send(JSON.stringify(data));}}
_executeServerCommands=function(commands,clientProperties,data,form,execAttrs){var req;if(JsonDataBinding.endsWithI(jsdb_serverPage,'.php')){if(typeof clientProperties!='undefined'){for(var nm in clientProperties){if(clientProperties[nm]instanceof Date){clientProperties[nm]=JsonDataBinding.datetimeToString(clientProperties[nm]);}}}}
if(typeof data!='undefined'&&data!=null){if(typeof data=='boolean'){if(data){_sendBoundData('',clientProperties,commands);}
else{req=new Object();req.Calls=commands;if(typeof clientProperties!='undefined'){req.values=clientProperties;}}}
else if(typeof data=='string'){_sendBoundData(data,clientProperties,commands);}
else{req=new Object();req.Calls=commands;if(typeof clientProperties!='undefined'){req.values=clientProperties;}
req.Data=data;}}
else{req=new Object();req.Calls=commands;if(typeof clientProperties!='undefined'){req.values=clientProperties;}}
if(req){_callServer(req,form,execAttrs);}}
_getData=function(dataName,clientProperties){var req=new Object();req.Calls=new Array();req.Calls[0]=new Object();req.Calls[0].method=jsdb_getdata;req.Calls[0].value=dataName;if(typeof clientProperties!='undefined'){req.values=clientProperties;}
_callServer(req);}
_attachOnRowDeleteHandler=function(name,handler){if(typeof onrowdeletehandlers[name]=='undefined'){onrowdeletehandlers[name]=new Array();}
var exist=false;for(var i=0;i<onrowdeletehandlers[name].length;i++){if(onrowdeletehandlers[name][i]==handler){exist=true;break;}}
if(!exist){onrowdeletehandlers[name].push(handler);}}
function isEventSupported(el,eventName){var isSupported=(eventName in el);if(!isSupported){el.setAttribute(eventName,'return;');isSupported=typeof el[eventName]==type_func;}
return isSupported;}
_getTableAttribute=function(tableName,attributeName){if(tableAttributes){if(tableAttributes[tableName]){var attrs=tableAttributes[tableName];return attrs[attributeName];}}}
_setTableAttribute=function(tableName,attributeName,value){var attrs;if(!tableAttributes[tableName]){tableAttributes[tableName]=new Object();}
attrs=tableAttributes[tableName];attrs[attributeName]=value;}
_addvaluechangehandler=function(tableName,handler){var t=dataChangeHandlers[tableName];if(!t){t={};dataChangeHandlers[tableName]=t;}
if(!t.onvaluechangehandlers){t.onvaluechangehandlers=new Array();}
for(var i=0;i<t.onvaluechangehandlers.length;i++){if(t.onvaluechangehandlers[i]==handler){return;}}
t.onvaluechangehandlers.push(handler);}
_getvaluechangehandler=function(tableName){var t=dataChangeHandlers[tableName];if(t){return t.onvaluechangehandlers;}}
var focusedElement;var snedkeysinitialized;function saveFocused(e){if(e){if(e.tagName){var tag=e.tagName.toLowerCase();if(tag=='input'){var s=e.type?e.type.toLowerCase():'';if(s=='text'||s=='password'){focusedElement=e;}}
else if(tag=='textarea'){focusedElement=e;}}}}
function onDocMouseDown(e){var sender=JsonDataBinding.getSender(e);saveFocused(sender);}
function onDocKeyup(){saveFocused(document.activeElement);}
_initSendKeys=function(){if(!snedkeysinitialized){snedkeysinitialized=true;if(!focusedElement){_selectNextInput();}
if(IsIE()){JsonDataBinding.AttachEvent(document,"onfocusin",onDocKeyup);}
else{document.addEventListener('focus',onDocKeyup,true);}}}
_selectNextInput=function(){var f=focusedElement;var currentTab=-100;if(f){currentTab=f.tabIndex;}
var gotNextTab=false;var gotMinTab=false;var nextTab;var minTab;var eNextTab;var eMinTab=f;function getNextTab(e){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(a&&a.tabIndex&&a.tagName){var tag=a.tagName.toLowerCase();if((tag=='input'&&a.type&&a.type.toLowerCase()=='text')||tag=='textarea'){if(a.tabIndex>currentTab){if(gotNextTab){if(a.tabIndex<nextTab){nextTab=a.tabIndex;eNextTab=a;}}
else{nextTab=a.tabIndex;eNextTab=a;gotNextTab=true;}}
else{if(gotMinTab){if(minTab>a.tabIndex){minTab=a.tabIndex;eMinTab=a;}}
else{minTab=a.tabIndex;eMinTab=a;gotMinTab=true;}}}}
getNextTab(a);}}
getNextTab(document.body);if(gotNextTab){eNextTab.focus();focusedElement=eNextTab;}
else{if(gotMinTab){eMinTab.focus();focusedElement=eMinTab;}}
f=focusedElement;if(f){var range;if(document.selection&&document.selection.createRange){range=document.selection.createRange();if(range){range.moveStart("character",f.value.length);range.moveEnd("character",f.value.length);range.select();}}
else if(f.setSelectionRange||f.createTextRange){var pos=f.value.length;if(f.setSelectionRange){f.focus();f.setSelectionRange(pos,pos);}
else if(f.createTextRange){range=f.createTextRange();range.collapse(true);range.moveEnd('character',pos);range.moveStart('character',pos);range.select();f.focus();}}}}
_sendKeys=function(key){var f=focusedElement;if(f){if(key=='{TAB}'){key='\t';}
f.focus();var range;if(document.selection&&document.selection.createRange){range=document.selection.createRange();if(range){range.text=key;range.collapse(false);range.select();}}
else if(f.setSelectionRange||f.createTextRange){if(f.setSelectionRange){var len=f.value.length;var start=f.selectionStart;var end=f.selectionEnd;f.value=f.value.substring(0,start)+key+f.value.substring(end,len);var pos=start+key.length;f.focus();f.setSelectionRange(pos,pos);}
else if(f.createTextRange){range=f.createTextRange();range.collapse(true);range.moveEnd('character',pos);range.moveStart('character',pos);range.select();}
f.focus();}}}}(),refreshDataBind:function(e,name){bindData({childNodes:[e]},name);},createId:function(baseName){return baseName+'xxxxxxxx'.replace(/[x]/g,function(c){var r=Math.random()*16|0;return r.toString(16);});},updateTextFile:function(serverType,fileContents,filePath,onFinish){var serverPage=_getServerPage();if(serverType=='php'){_setServerPage('limnor_updateFile.php');}
else if(serverType=='aspx'){_setServerPage('Limnor_webUtility.aspx');}
else{if(onFinish){onFinish('unsupported server type:'+serverType);}
return;}
var curId=JsonDataBinding.createId('id');_attachExtendedEvent('onProcessServerCall',curId,onFinish);_executeServerCommands([{method:'updateFile',value:filePath}],{contents:JsonDataBinding.base64Encode(fileContents),serverComponentName:curId});_setServerPage(serverPage);},deleteWebFile:function(serverType,filePath,onFinish){var serverPage=_getServerPage();if(serverType=='php'){_setServerPage('limnor_updateFile.php');}
else if(serverType=='aspx'){_setServerPage('Limnor_webUtility.aspx');}
else{if(onFinish){onFinish('unsupported server type:'+serverType);}
return;}
var curId=JsonDataBinding.createId('id');_attachExtendedEvent('onProcessServerCall',curId,onFinish);_executeServerCommands([{method:'deleteWebFile',value:filePath}],{serverComponentName:curId});_setServerPage(serverPage);},checkUrlExist:function(serverType,url,onFinish){var serverPage=_getServerPage();if(serverType=='php'){_setServerPage('limnor_updateFile.php');}
else if(serverType=='aspx'){_setServerPage('Limnor_webUtility.aspx');}
else{if(onFinish){onFinish('unsupported server type:'+serverType);}
return;}
var curId=JsonDataBinding.createId('id');_attachExtendedEvent('onProcessServerCall',curId,onFinish);_executeServerCommands([{method:'checkUrlExist',value:url}],{serverComponentName:curId});_setServerPage(serverPage);},checkFileExist:function(serverType,filepath,onFinish){var serverPage=_getServerPage();if(serverType=='php'){_setServerPage('limnor_updateFile.php');}
else if(serverType=='aspx'){_setServerPage('Limnor_webUtility.aspx');}
else{if(onFinish){onFinish('unsupported server type:'+serverType);}
return;}
var curId=JsonDataBinding.createId('id');_attachExtendedEvent('onProcessServerCall',curId,onFinish);_executeServerCommands([{method:'checkFileExist',value:filepath}],{serverComponentName:curId});_setServerPage(serverPage);},accessServer:function(procPage,methodName,paramValue,clientProperties,onFinish){var serverPage=_getServerPage();var curId=JsonDataBinding.createId('id');if(procPage){_setServerPage(procPage);}
_attachExtendedEvent('onProcessServerCall',curId,onFinish);if(clientProperties){clientProperties.serverComponentName=curId;}
else{clientProperties={serverComponentName:curId};}
_executeServerCommands([{method:methodName,value:paramValue}],clientProperties);_setServerPage(serverPage);},setServerPage:function(pageUrl){_setServerPage(pageUrl);},getData:function(dataName,clientProperties){_getData(dataName,clientProperties);},putData:function(dataName,clientProperties){_putData(dataName,clientProperties);},callServer:function(commands,clientProperties,data){_executeServerCommands(commands,clientProperties,data);},executeServerMethod:function(command,clientProperties,form){_executeServerCommands([{method:command,value:'0'}],clientProperties,null,form);},sendRawData:function(data){var debug=JsonDataBinding.Debug;JsonDataBinding.Debug=false;_callServer(data);JsonDataBinding.Debug=debug;},sendRawDataToURL:function(data,url,headers){var debug=JsonDataBinding.Debug;var srv=_getServerPage();JsonDataBinding.Debug=false;_setServerPage(url);_callServer(data,null,headers);JsonDataBinding.Debug=debug;_setServerPage(srv);},submitBoundData:function(){_submitBoundData();},addRow:function(dataName){return _addRow(dataName);},saveEdsToOffline:function(dataName,tag){_saveEdsToOffline(dataName,tag);},loadEdsFromOffline:function(dataName,tag){_loadEdsFromOffline(dataName,tag);},removeEdsFromOffline:function(tag){_removeEdsFromOffline(tag);},deleteCurrentRow:function(dataName){_deleteCurrentRow(dataName);},getCurrentRowIndex:function(dataName){return _getCurrentRowIndex(dataName);},getRowCount:function(dataName){return _getRowCount(dataName);},dataMoveFirst:function(dataName){return _dataMoveFirst(dataName);},dataMovePrevious:function(dataName){return _dataMovePrevious(dataName);},dataMoveNext:function(dataName){return _dataMoveNext(dataName);},dataMoveLast:function(dataName){return _dataMoveLast(dataName);},dataMoveToRecord:function(dataName,rowIndex){return _dataMoveToRecord(dataName,rowIndex);},clearTableRows:function(dataName){return _clearTableRows(dataName);},sortOnColumn:function(dataName,columnName,sortAsc,ignoreCase){return _sortOnColumn(dataName,columnName,sortAsc,ignoreCase);},columnSum:function(dataName,columnName){return _columnSum(dataName,columnName);},columnValue:function(dataName,columnName,rowIndex){return _columnValue(dataName,columnName,rowIndex);},columnValueByIndex:function(dataName,columnIndex,rowIndex){return _columnValueByIndex(dataName,columnIndex,rowIndex);},isColumnValueNull:function(dataName,columnName,rowIndex){return _isColumnValueNull(dataName,columnName,rowIndex);},isColumnValueNullOrEmpty:function(dataName,columnName,rowIndex){return _isColumnValueNullOrEmpty(dataName,columnName,rowIndex);},isColumnValueNotNull:function(dataName,columnName,rowIndex){return _isColumnValueNotNull(dataName,columnName,rowIndex);},isColumnValueNotNullOrEmpty:function(dataName,columnName,rowIndex){return _isColumnValueNotNullOrEmpty(dataName,columnName,rowIndex);},setColumnValue:function(dataName,columnName,val,rowIndex){_setcolumnValue(dataName,columnName,val,rowIndex);},getColumnValue:function(dataName,columnName,rowIndex){return _columnValue(dataName,columnName,rowIndex);},columnExpressionValue:function(dataName,expression,rowIndex){return _columnExpressionValue(dataName,expression,rowIndex);},statistics:function(dataName,expression,operator){return _statistics(dataName,expression,operator);},addOnRowIndexChangeHandler:function(tableName,handler){_addOnRowIndexChangeHandler(tableName,handler);},onRowIndexChange:function(name){_onRowIndexChange(name);},refetchDetailRows:function(mainTableName,detailTableName){_refetchDetailRows(mainTableName,detailTableName);},getTableBody:function(table){var i;var tb;for(i=0;i<table.children.length;i++){if(table.children[i]&&table.children[i].tagName&&table.children[i].tagName.toLowerCase()=='tbody'){tb=table.children[i];break;}}
if(!tb){tb=document.createElement('tbody');var tf;for(i=0;i<table.children.length;i++){if(table.children[i]&&table.children[i].tagName&&table.children[i].tagName.toLowerCase()=='tfoot'){tf=table.children[i];break;}}
if(tf){table.insertBefore(tb,tf);}
else{table.appendChild(tb);}}
return tb;},getSender:function(e){var c;if(!e)e=window.event;if(e){if(e.target)c=e.target;else if(e.srcElement)c=e.srcElement;if(typeof c!='undefined'){if(c.nodeType==3)
c=c.parentNode;}
else{c=e;}}
return c;},getZOrder:function(e){var z=0;while(e){if(e.style&&e.style.zIndex){var d=parseInt(e.style.zIndex);if(d>z)z=d;}
e=e.parentNode;}
return z;},AjaxTimeout:0,values:{},Debug:false,SetEventFirer:function(eo){if(typeof eo.disabled!='undefined'){_setEventFirer(eo);}
else{_setEventFirer(null);}},AttachOnRowDeleteHandler:function(name,handler){_attachOnRowDeleteHandler(name,handler);},urlToFilename:function(url){if(url){return url.replace(/^.*(\\|\/|\:)/,'');}},getPageFilename:function(){var s=window.location.href;return JsonDataBinding.urlToFilename(s);},getPageFilenameWithoutParameters:function(){var s=JsonDataBinding.getPageFilename();var pos=s.indexOf('?');if(pos>0){return s.substr(0,pos);}
return s;},getPageFileFullPath:function(){var s=window.location.pathname;if(s.charAt(0)){s=s.substr(1);}
return s;},getWebSitePath:function(){var s=window.location.href;var f=JsonDataBinding.getPageFilename();var w=s.replace(f,'');return w;},gotoWebPage:function(pageFilepath){if(pageFilepath){var u1=pageFilepath.toLowerCase();var wp=window;var alreadyLoaded=false;while(wp){var u0=wp.location.href;u0=JsonDataBinding.urlToFilename(u0);var idx=u0.indexOf('?');if(idx>0){u0=u0.substr(0,idx);}
u0=u0.toLowerCase();if(u0==u1){alreadyLoaded=true;break;}
if(wp.parent==wp)
break;if(!wp.parent)
break;wp=wp.parent;}
if(wp){if(alreadyLoaded){if(wp!=window){if(JsonDataBinding.closePage)
JsonDataBinding.closePage();}
else{if(IsFireFox()){setTimeout('window.location.reload(true);',0);}
else{window.location.reload(true);}}}
else{JsonDataBinding.pageMoveout=true;window.location.href=pageFilepath;}}}},eraseOfflineData:function(name){if(window.localStorage){var gv=window.localStorage['limnor_gv'];if(gv&&gv.length>0){var vs=JSON.parse(gv);if(name in vs){delete vs[name];gv=JSON.stringify(vs);window.localStorage['limnor_gv']=gv;}}}},offlineDataExists:function(name){if(window.localStorage){var gv=window.localStorage['limnor_gv'];if(gv&&gv.length>0){var vs=JSON.parse(gv);if(name in vs){var d=vs[name];if(typeof(d)!='undefined'&&d!==null&&typeof(d.v)!='undefined'&&d.v!==null){if(typeof(d.e)!='undefined'&&d.e!==null){d.e=new Date(d.e);if(d.e<new Date()){delete vs[name];gv=JSON.stringify(vs);window.localStorage['limnor_gv']=gv;}
else
return true;}
else{return true;}}}}}
return false;},cookieExists:function(name){if(JsonDataBinding.UseLocalStore){return JsonDataBinding.offlineDataExists(name);}
var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];var pos=c.indexOf('=');if(pos>0){var nm=c.substr(0,pos).replace(/^\s+|\s+$/g,"");if(nm==name){return true;}}}
return false;},getOfflineDataByStartsWith:function(name){var ret=new Array();if(window.localStorage){var gv=window.localStorage['limnor_gv'];if(gv&&gv.length>0){var vs=JSON.parse(gv);for(var nm in vs){if(JsonDataBinding.startsWithI(nm,name)){var vl=vs[nm];if(typeof(vl)!='undefined'&&vl!==null){if(typeof(vl.e)!='undefined'&&vl.e!==null){vl.e=new Date(vl.e);if(vl.e<new Date()){delete vs[nm];gv=JSON.stringify(vs);window.localStorage['limnor_gv']=gv;continue;}}
ret.push({name:nm,value:vl.v});}}}}}
return ret;},getCookieByStartsWith:function(name){if(JsonDataBinding.UseLocalStore){return JsonDataBinding.getOfflineDataByStartsWith(name);}
var ret=new Array();var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];var pos=c.indexOf('=');if(pos>0){var nm=c.substr(0,pos).replace(/^\s+|\s+$/g,"");if(JsonDataBinding.startsWithI(nm,name)){ret.push({name:nm,value:c.substr(pos+1)});}}}
return ret;},getOfflineData:function(name){if(window.localStorage){var gv=window.localStorage['limnor_gv'];if(gv&&gv.length>0){var vs=JSON.parse(gv);if(name in vs){var vl=vs[name];if(typeof(vl)!='undefined'&&vl!==null){if(typeof(vl.e)!='undefined'&&vl.e!==null){vl.e=new Date(vl.e);var dn=new Date();if(vl.e<dn){delete vs[name];gv=JSON.stringify(vs);window.localStorage['limnor_gv']=gv;return;}}
return vl.v;}}}}},getCookie:function(name){if(JsonDataBinding.UseLocalStore){return JsonDataBinding.getOfflineData(name);}
var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];var pos=c.indexOf('=');if(pos>0){var nm=c.substr(0,pos).replace(/^\s+|\s+$/g,"");if(nm==name){return c.substr(pos+1);}}}
return null;},setOfflineData:function(name,value,exMinutes){if(window.localStorage){var vs={};var gv=window.localStorage['limnor_gv'];if(gv&&gv.length>0){vs=JSON.parse(gv);}
var da={};da.v=value;if(exMinutes){var date=new Date();da.e=new Date(date.getTime()+parseInt(exMinutes)*60*1000);}
vs[name]=da;gv=JSON.stringify(vs);window.localStorage['limnor_gv']=gv;}},setCookie:function(name,value,exMinutes){if(JsonDataBinding.UseLocalStore){JsonDataBinding.setOfflineData(name,value,exMinutes);}
else{var expires;if(exMinutes){var date=new Date();date.setTime(date.getTime()+(parseInt(exMinutes)*60*1000));expires="; expires="+date.toGMTString();}
else expires="";var ck=name+"="+value+expires+"; path=/;";document.cookie=ck;}},eraseCookie:function(name){if(JsonDataBinding.UseLocalStore){JsonDataBinding.eraseOfflineData(name);}
else{JsonDataBinding.setCookie(name,"",-1);}},getReturnUrl:function(){var s=window.location.href;var n=s.indexOf('?');if(n>=0){s=s.substr(n+1);if(JsonDataBinding.endsWith(s,'$')){s=s.substr(0,s.length-1);}
if(!JsonDataBinding.startsWithI(s,'debugRef=')){return s;}}},ShowPermissionError:function(labelName,msg){var s=window.location.href;var n=s.indexOf('?');if(n>=0){if(JsonDataBinding.endsWith(s,'$')){if(!msg||msg.length==0){msg='You do not have permission to visit this web page.';}
if(labelName&&labelName.length>0){var lbl=document.getElementById(labelName);if(lbl){lbl.innerHTML=msg;return;}}
_executeClientEventObject('LoginFailed');}}},hasLoggedOn:function(){return _hasLoggedOn();},LoginFailed:function(msgId,msg){var lbl=document.getElementById(msgId);JsonDataBinding.SetInnerText(lbl,msg);_executeClientEventObject('LoginFailed');},LoginPassed:function(login,expire,userLevel,userid){try{_loginPassed(login,expire,userLevel,userid);if(typeof JsonDataBinding!='undefined'){var surl=JsonDataBinding.getReturnUrl();if(surl){window.location.href=surl;}}}
catch(err){alert('Error handling post-login. '+(err.message?err.message:err));}},LoginPassed2:function(){if(typeof JsonDataBinding!='undefined'){var surl=JsonDataBinding.getReturnUrl();if(surl){window.location.href=surl;}}},LogOff:function(){_logOff();},LogonPage:'',setupLoginWatcher:function(){_setupLoginWatcher();},TargetUserLevel:0,GetCurrentUserAlias:function(){return _getCurrentUserAlias();},GetCurrentUserID:function(){return _getCurrentUserID();},GetCurrentUserLevel:function(){return _getCurrentUserLevel();},UserLoggedOn:function(){return _userLoggedOn();},SetLoginCookieName:function(nm){_setUserLogCookieName(nm);},IsChrome:function(){return(navigator.userAgent.toLowerCase().indexOf('chrome')>-1);},IsSafari:function(){return IsSafari();},IsIE:function(){return IsIE();},IsFireFox:function(){return IsFireFox();},IsOpera:function(){return IsOpera();},getInternetExplorerVersion:function(){var rv=-1;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null)
rv=parseFloat(RegExp.$1);}
return rv;},SimpleHandlerChain:function(previous,current){var pre=previous;var cur=current;return function(){pre();cur();}},getPageZIndex:function(e0){function _getzIndex(e,zi){if(e!=e0){var zx=0;for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(a.style&&a.style.zIndex){zx=parseInt(a.style.zIndex);if(zx>zi){zi=zx;}}
var z2=_getzIndex(a,zi);if(z2>zi){zi=z2;}}}
return zi;}
return _getzIndex(document.body,0);},AttachEvent:function(obj,eventName,handler){if(!obj||!eventName||!handler)
return;if(eventName=='')return;if(!obj.handlerHolders)
obj.handlerHolders={};if(!obj.handlerHolders[eventName])
obj.handlerHolders[eventName]=[];obj.handlerHolders[eventName].push(handler);if(IsFireFox()||IsSafari()||IsChrome()||IsOpera()){if(eventName.substring(0,2)=='on'){eventName=eventName.substring(2);}}
if(typeof(obj.attachEvent)=='function'){obj.attachEvent(eventName,handler);}
else{if(typeof(obj.addEventListener)=='function'){if(eventName.substring(0,2)=='on'){eventName=eventName.substring(2);}
obj.addEventListener(eventName,handler,false);}
else{if(obj[eventName]){obj[eventName]=JsonDataBinding.SimpleHandlerChain(obj[eventName],handler);}
else{obj[eventName]=handler;}}}},DetachEvent:function(obj,eventName,handler){if(obj.handlerHolders&&obj.handlerHolders[eventName]){for(var i=0;i<obj.handlerHolders[eventName].length;i++){if(obj.handlerHolders[eventName][i]==handler){obj.handlerHolders[eventName].splice(i,1);break;}}}
if(IsFireFox()||IsSafari()||IsChrome()||IsOpera()){if(eventName.substring(0,2)=='on'){eventName=eventName.substring(2);}}
if(typeof(obj.detachEvent)=='function'){obj.detachEvent(eventName,handler);}
else{if(typeof(obj.removeEventListener)=='function'){obj.removeEventListener(eventName,handler,false);}
else{if(obj[eventName]){obj[eventName]=null;}}}},ClearEvent:function(obj,eventName){var en=eventName;if(IsFireFox()||IsSafari()||IsChrome()||IsOpera()){if(en.substring(0,2)=='on'){en=en.substring(2);}}
if(obj[eventName]){obj[eventName]=null;}
if(obj.handlerHolders&&obj.handlerHolders[eventName]){for(var i=0;i<obj.handlerHolders[eventName].length;i++){if(obj.handlerHolders[eventName][i]){if(typeof(obj.detachEvent)=='function'){obj.detachEvent(en,obj.handlerHolders[eventName][i]);}
if(typeof(obj.removeEventListener)=='function'){obj.removeEventListener(en,obj.handlerHolders[eventName][i],false);}}}
obj.handlerHolders[eventName]=null;}},SwitchCulture:function(cultreName){if(typeof cultreName=='undefined'||cultreName==null||cultreName=='null'){cultreName=_getCulture();}
if(!cultreName||cultreName=='null'){cultreName='';}
_setCulture(cultreName);},GetCulture:function(){return _getCulture();},GetValueInCurrentCulture:function(valueName){return _getValueInCurrentCulture(valueName);},AddPageResourceName:function(resName,resType){_addPageResourceName(resName,resType);},AddPageCulture:function(cultureName){_addPageCulture(cultureName);},PageValues:Object,ProcessPageParameters:function(){var fname;var purl;if(typeof JsonDataBinding.PageValues=='undefined'){JsonDataBinding.PageValues=new Object();}
var query=window.location.search.substring(1);var vars=query.split("&");for(var i=0;i<vars.length;i++){var pair=vars[i].split("=");if(pair&&pair.length>0){if(pair.length==1){JsonDataBinding.PageValues['P10936C6EB1D741fbA2B8A25A7E2B61EF']=unescape(pair[0]);}
else if(pair.length==2){JsonDataBinding.PageValues[pair[0]]=unescape(pair[1]);if(pair[0].substr(0,7)=='iframe_'){fname=pair[0].substr(7);purl=JsonDataBinding.PageValues[pair[0]];}
else if(pair[0]=='lang'){JsonDataBinding.SwitchCulture(pair[1]);}}}}
JsonDataBinding.anchorAlign.initializeBodyAnchor();JsonDataBinding.AttachEvent(window,'onresize',JsonDataBinding.anchorAlign.applyBodyAnchorAlign);JsonDataBinding.anchorAlign.applyBodyAnchorAlign();if(typeof fname!='undefined'&&fname.length>0){window.open(purl,fname);}},onPageInitialize:function(){if(document.onparentload){function checkParentReady(){if(window.parent){if(window.parent.document.readyState!='complete'){setTimeout(checkParentReady,200);return;}
document.onparentload();}}
checkParentReady();}},SetImageData:function(c,v){if(v){c.src='data:image/jpg;base64,'+v;}
else{c.src=null;}},SetInnerText:function(c,v0){if(c&&c.tagName){var v=JsonDataBinding.toText(v0);if(c.tagName.toLowerCase()=='input'){c.value=v;}
else{if(IsIE()){c.innerText=v;}
else if(typeof(c.textContent)=='undefined'){c.innerText=v;}
else{c.textContent=v;}}}},GetInnerText:function(c){if(c&&c.tagName){if(c.tagName.toLowerCase()=='input'){return c.value;}
else{if(IsIE()){return c.innerText;}
else if(typeof(c.textContent)=='undefined'){return c.innerText;}
else{return c.textContent;}}}},GetSelectedListValue:function(list){if(list.selectedIndex>=0){return list.options[list.selectedIndex].value;}
return null;},GetSelectedListText:function(list){if(list.selectedIndex>=0){return list.options[list.selectedIndex].text;}
return'';},SetSelectedListValue:function(list,v){if(list.selectedIndex>=0){list.options[list.selectedIndex].value=v;}
return null;},SetSelectedListText:function(list,v){if(list.selectedIndex>=0){list.options[list.selectedIndex].text=v;}
return'';},SetTextHeightToContent:function(ta){function resize(){ta.style.height='auto';ta.style.height=ta.scrollHeight+'px';if(ta.onHeightAdjusted){ta.onHeightAdjusted(ta);}}
window.setTimeout(resize,0);},ProcessServerResponse:function(r){_processServerResponse(r);},IFrame:null,SubmittedForm:null,ProcessIFrame:function(){if(typeof JsonDataBinding!='undefined'){if(JsonDataBinding.IFrame){try{if(JsonDataBinding.IFrame.document){if(JsonDataBinding.SubmittedForm&&JsonDataBinding.SubmittedForm.state)
_processServerResponse(JsonDataBinding.IFrame.document.body.innerHTML,JsonDataBinding.SubmittedForm.state);else
_processServerResponse(JsonDataBinding.IFrame.document.body.innerHTML);JsonDataBinding.IFrame.document.body.innerHTML='';}}
catch(exp){if(typeof JsonDataBinding!='undefined'){if(JsonDataBinding.SubmittedForm&&JsonDataBinding.SubmittedForm.state)
_processServerResponse('Error processing form submit. '+(exp.message?exp.message:exp)+'. You may try to use Chrome to get more detailed and accurate information',JsonDataBinding.SubmittedForm.state,true);else
_processServerResponse('Error processing form submit. '+(exp.message?exp.message:exp)+'. You may try to use Chrome to get more detailed and accurate information',true);}}}}},GetSelectedText:function(){var userSelection;if(window.getSelection){userSelection=window.getSelection();}
else if(document.selection){userSelection=document.selection.createRange();}
var selectedText=userSelection;if(userSelection.text)
selectedText=userSelection.text;else{if(userSelection.anchorNode){selectedText=userSelection.anchorNode.nodeValue;}}
return selectedText;},ShowAjaxCallWaitingImage:null,ShowAjaxCallWaitingLabel:null,SetDatetimePicker:function(datetimePicker){_setDatetimePicker(datetimePicker);},GetDatetimePicker:function(){return _getdatetimepicker();},GetDatetimePickerSelectedValue:function(textBoxId){var dp=JsonDataBinding.GetDatetimePicker();if(dp){return dp.getSelectedDate(textBoxId);}},SetDatetimePickerSelectedValue:function(textBoxId,d){var dp=JsonDataBinding.GetDatetimePicker();if(dp){if(d){var yyyymmdd;var hh,mm,ss;if(d instanceof Date){var mo=d.getMonth()+1;yyyymmdd=''+d.getFullYear()+(mo<10?'0'+mo:mo)+(d.getDate()<10?'0'+d.getDate():d.getDate());hh=d.getHours();mm=d.getMinutes();ss=d.getSeconds();}
else{d=JsonDataBinding.replaceAll(d,'-','');var i=d.indexOf(' ');if(i>0){yyyymmdd=d.substr(0,i);var hhmmss=d.substr(i+1);if(hhmmss&&hhmmss.length>0){i=hhmmss.indexOf(':');if(i>=0){hh=hhmmss.substr(0,i);hhmmss=hhmmss.substr(i+1);i=hhmmss.indexOf(':');if(i>=0){mm=hhmmss.substr(0,i);ss=hhmmss.substr(i+1);}}
else{if(hhmmss.length==6){hh=hhmmss.substr(0,2);mm=hhmmss.substr(2,2);ss=hhmmss.substr(4,2);}}}}
else{yyyymmdd=d;}}
if(yyyymmdd){var b=dp.setSelectedDate(textBoxId,yyyymmdd,hh,mm,ss);return b;}}}},DisableDatetimePicker:function(textBoxId,d){var dp=JsonDataBinding.GetDatetimePicker();if(dp){if(d)
dp.disable(textBoxId);else
dp.enable(textBoxId);}},IsDatetimePickerIncludeTime:function(textBoxId){var dp=JsonDataBinding.GetDatetimePicker();if(dp){return dp.getEnableTime(textBoxId);}},SetDatetimePickerUseTime:function(textBoxId,d){var dp=JsonDataBinding.GetDatetimePicker();if(dp){dp.setEnableTime(textBoxId,d);}},adjustDatePickerButtonPos:function(textBoxId){var dp=JsonDataBinding.GetDatetimePicker();if(dp){dp.adjustButtonPos(textBoxId);}},showHideDatePicker:function(show,inpId){var dp=JsonDataBinding.GetDatetimePicker();if(dp){if(show)
dp.show(inpId);else
dp.hide(inpId);}},CreateDatetimePickerForTextBox:function(textBoxId,fontsize,includeTime,isStandalone,container,disableMove){var dp=JsonDataBinding.GetDatetimePicker();if(dp){function onselecteddatetime(args){if(args&&args.id){var o=document.getElementById(args.id);if(o&&o.onselectedDateTime){o.onselectedDateTime({target:o});}}}
var fsize=(typeof fontsize=='undefined'?'12px':fontsize);var bTime=typeof includeTime=='undefined'?true:includeTime;var bStandalone=typeof isStandalone=='undefined'?false:isStandalone;var pos=bStandalone?'static':'absolute';var opts={formElements:{},showWeeks:true,statusFormat:"l-cc-sp-d-sp-F-sp-Y",bounds:{position:pos,inputRight:true,fontSize:fsize,inputTime:bTime,forStandalone:bStandalone}};opts.formElements[textBoxId]="Y-ds-m-ds-d";opts.callbackFunctions={'dateset':[onselecteddatetime]};if(container){opts.bounds.container=container;}
if(disableMove){opts.bounds.disableMove=disableMove;}
dp.createDatePicker(opts);if(bStandalone){dp.show(textBoxId);var el=document.getElementById(textBoxId);if(el){el.datepickerStandaloe=true;var dpdiv=document.getElementById('fd-'+textBoxId);if(dpdiv){dpdiv.style.left=el.style.left;dpdiv.style.top=el.style.top;}
el.style.display='none';var but=document.getElementById("fd-but-"+textBoxId);if(but){but.style.display='none';}}}}
else{_pushDatetimeInput({textBoxId:textBoxId,fontsize:fontsize,inputTime:includeTime,standalone:isStandalone,container:container,disableMove:disableMove});}},getClientEventHolder:function(eventName,objectName){return _getClientEventHolder(eventName,objectName);},attachExtendedEvent:function(eventName,objectName,handler){_attachExtendedEvent(eventName,objectName,handler);},detachExtendedEvent:function(eventName,objectName,handler){_detachExtendedEvent(eventName,objectName,handler);},clearExtendedEvent:function(eventName,objectName){_clearExtendedEvent(eventName,objectName);},switchExtendedEvent:function(eventName,objectName,handler){_clearExtendedEvent(eventName,objectName);_attachExtendedEvent(eventName,objectName,handler)},executeEventHandlers:function(eventName,objectName){_executeEventHandlers(eventName,objectName);},getObjectProperty:function(objectName,propertyName){return _getObjectProperty(objectName,propertyName);},setObjectProperty:function(objectName,propertyName,value){_setObjectProperty(objectName,propertyName,value);},onSetCustomValue:function(obj,valueName){_onSetCustomValue(obj,valueName);},eraseSessionVariable:function(name){_eraseSessionVariable(name);},getSessionVariable:function(name){return _getSessionVariable(name);},setSessionVariable:function(name,value){_setSessionVariable(name,value);},initSessionVariable:function(name,value){_initSessionVariable(name,value);},StartSessionWatcher:function(){_startSessionWatcher();},GetSessionVariables:function(){return _getSessionVariables();},setSessionTimeout:function(timeoutMinutes){_setSessionTimeout(timeoutMinutes);},getSessionTimeout:function(){return _getSessionTimeout();},bindDataToElement:function(e,name,firstTime){_bindData(e,name,firstTime);},resetDataStreaming:function(name){_resetDataStreaming(name);},getModifiedRowCount:function(name){return _getModifiedRowCount(name);},getDeletedRowCount:function(name){return _getDeletedRowCount(name);},getNewRowCount:function(name){return _getNewRowCount(name);},getRowCount:function(name){return _getRowCount(name);},getActiveRowCount:function(name){return _getActiveRowCount(name);},setTableAttribute:function(tableName,attributeName,value){_setTableAttribute(tableName,attributeName,value);},getTableAttribute:function(tableName,attributeName){return _getTableAttribute(tableName,attributeName);},addTableLink:function(tableName,value){var detailTbls=_getTableAttribute(tableName,'LinkedDetails');if(!detailTbls){detailTbls=[];_setTableAttribute(tableName,'LinkedDetails',detailTbls);}
for(var i=0;i<detailTbls.length;i++){if(detailTbls[i].detailTableName==value.detailTableName){detailTbls[i]=value;return;}}
detailTbls.push(value);},confirmResult:false,addvaluechangehandler:function(tableName,handler){_addvaluechangehandler(tableName,handler);},onvaluechanged:function(t,r,c,val){var ta=_getvaluechangehandler(t.TableName);if(ta){for(var i=0;i<ta.length;i++){if(ta[i]){ta[i].oncellvaluechange(t.TableName,r,c,val);}}}},endsWith:function(container,ends){if(container&&ends){if(container.length>=ends.length){return container.indexOf(ends,container.length-ends.length)!==-1;}}
return false;},endsWithI:function(container,ends){if(container&&ends){if(container.length>=ends.length){var c=container.toLowerCase();var e=ends.toLowerCase();return c.indexOf(e,c.length-e.length)!==-1;}}
return false;},startsWith:function(container,starts){if(container&&starts){if(container.length>=starts.length){var c=container.substr(0,starts.length);return(c==starts);}}
return false;},startsWithI:function(container,starts){if(container&&starts){if(container.length>=starts.length){var c=container.substr(0,starts.length).toLowerCase();var e=starts.toLowerCase();return(c==e);}}
return false;},stringEQi:function(s1,s2){if(s1&&s2){if(typeof s1=='string'){if(typeof s2=='string'){return(s1.toLowerCase()==s2.toLowerCase());}
else{var s20=s2.toString().toLowerCase();return s1==s20;}}
else{var s10=s1.toString();if(typeof s2=='string'){return s10==s2;}}}
return(s1===s2);},datetimeToString:function(d){if(d instanceof Date){var mo=d.getMonth()+1;var dy=d.getDate();var hr=d.getHours();var mi=d.getMinutes();var se=d.getSeconds();return d.getFullYear()+"-"+(mo<10?"0"+mo:mo)+"-"+(dy<10?"0"+dy:dy)+" "+(hr<10?"0"+hr:hr)+":"+(mi<10?"0"+mi:mi)+":"+(se<10?"0"+se:se);}
return d;},getAlphaNumeric:function(s){if(s){return s.replace(/[^\w]+/g,"");}
return s;},getAlphaNumericEx:function(s){if(s){return s.replace(/[^a-zA-Z0-9_-]+/g,"");}
return s;},getAlphaNumericPlus:function(s){if(s){return s.replace(/[^a-zA-Z0-9 _+-]+/g,"");}
return s;},randomString:function(string_length){var chars="0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";var randomstring='';for(var i=0;i<string_length;i++){var rnum=Math.floor(Math.random()*chars.length);randomstring+=chars.substring(rnum,rnum+1);}
return randomstring;},replaceAll:function(str,token,newToken,ignoreCase){if(str&&token){str=str+'';token=token+'';if(str.length>0&&token.length>0){var _token;var i=-1;if(typeof newToken=='undefined'||newToken==null)newToken='';if(ignoreCase){_token=token.toLowerCase();while((i=str.toLowerCase().indexOf(token,i>=0?i+newToken.length:0))!==-1){str=str.substring(0,i)+
newToken+
str.substring(i+token.length);}}else{return str.split(token).join(newToken);}}}
return str;},getFilename:function(f){if(typeof f!='undefined'){var pos=f.lastIndexOf('/');if(pos>=0){f=f.substr(pos+1);}
pos=f.lastIndexOf('\\');if(pos>=0){f=f.substr(pos+1);}
return f;}},getFilenameNoExt:function(f){f=JsonDataBinding.getFilename(f);if(typeof f!='undefined'){var pos=f.lastIndexOf('.');if(pos>=0){f=f.substr(pos+1);}
return f;}},removeArrayItem:function(oa,v){var ret=new Array();if(oa&&oa.length>0){for(var i=0;i<oa.length;i++){if(oa[i]!=v){ret.push(oa[i]);}}}
return ret;},removeEmptyArrayItem:function(oa){var ret=new Array();if(oa&&oa.length>0){for(var i=0;i<oa.length;i++){if(typeof oa[i]!='undefined'&&oa[i]!=null&&oa[i]!=''){ret.push(oa[i]);}}}
return ret;},isEmailAddress:function(email){var re=/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;return re.test(email);},isEmailAddressList:function(emails){if(typeof emails=='string'&&emails!=null&&emails.length>0){var ss=emails.split(';');for(var i=0;i<ss.length;i++){if(ss[i].length>0&&!JsonDataBinding.isEmailAddress(ss[i])){return false;}}
return true;}
return false;},getContentSize:function(el,adjustHeight){var sz={x:0,y:0};function getsize(e){if(e){for(var i=0;i<e.children.length;i++){if(e.children[i].offsetLeft+e.children[i].scrollWidth>sz.x){sz.x=e.children[i].offsetLeft+e.children[i].scrollWidth;}
if(e.children[i].offsetTop+e.children[i].scrollHeight>sz.y){sz.y=e.children[i].offsetTop+e.children[i].scrollHeight;}
if(adjustHeight){e.children[i].style.height=e.children[i].scrollHeight+'px';}
getsize(e.children[i]);}}}
getsize(el);return sz;},setFont:function(e,ft){var fnt={fontStyle:'',fontWeight:'',textDecoration:''}
for(var i=0;i<ft.length;i++){fnt[ft[i].name]=ft[i].value;}
for(var nm in fnt){e.style[nm]=fnt[nm];}},decodeBase64:function(input){if(!input){return input;}
var bytes=[];var output="";var chr1,chr2,chr3="";var enc1,enc2,enc3,enc4="";var i=0;var keyStr="ABCDEFGHIJKLMNOP"+"QRSTUVWXYZabcdef"+"ghijklmnopqrstuv"+"wxyz0123456789+/"+"=";input=input.replace(/[^A-Za-z0-9\+\/\=]/g,"");if(input.length==0){return input;}
do{enc1=keyStr.indexOf(input.charAt(i++));enc2=keyStr.indexOf(input.charAt(i++));enc3=keyStr.indexOf(input.charAt(i++));enc4=keyStr.indexOf(input.charAt(i++));chr1=(enc1<<2)|(enc2>>4);chr2=((enc2&15)<<4)|(enc3>>2);chr3=((enc3&3)<<6)|enc4;bytes.push(chr1);if(enc3!=64){bytes.push(chr2);}
if(enc4!=64){bytes.push(chr3);}
chr1=chr2=chr3="";enc1=enc2=enc3=enc4="";}while(i<input.length);output=JsonDataBinding.bytes2utf8.decode(bytes);return unescape(output);},getPathFromUrl:function(urlStr,webPath){var fn=urlStr;if(urlStr&&urlStr.length>4){if(urlStr.indexOf('url(')==0){fn=urlStr.substr(4);if(fn.indexOf(')',fn.length-1)!=-1){fn=fn.substr(0,fn.length-1);}}}
if(fn&&fn.length>1){while(fn.indexOf("'")!=-1){fn=fn.replace("'","");}
while(fn.indexOf('"')!=-1){fn=fn.replace('"','');}
if(webPath&&webPath.length>0){if(JsonDataBinding.startsWithI(fn,webPath)){fn=fn.substr(webPath.length);}}}
return fn;},getUrlFromPath:function(urlStr,webPath){if(urlStr&&urlStr.length>0){if(urlStr.indexOf('url(')!=0){var fn=urlStr;while(fn.indexOf("'")!=-1){fn=fn.replace("'","");}
while(fn.indexOf('"')!=-1){fn=fn.replace('"','');}
if(webPath&&webPath.length>0){if(JsonDataBinding.startsWithI(fn,webPath)){fn=fn.substr(webPath.length);}}
fn='url('+fn+')';return fn;}}
return urlStr;},getWindowByPageFilename:function(pageFilename){return _getWindowByPageFilename(pageFilename);},getWindowById:function(pageId){return _getWindowById(pageId);},getDocumentById:function(id){if(document.pageId==id){return document;}
if(document.currentDialog&&!document.currentDialog.finished){if(document.currentDialog.getPageId()==id){return document.currentDialog.getPageDoc();}}
var o=opener;while(o){if(o.document.pageId==id){return o.document;}
o=o.opener;}
o=parent;while(o){if(o.document.pageId==id){return o.document;}
if(o==o.parent){break;}
o=o.parent;}
var w=_getWindowById(id);if(w){return w.document;}},getElementByPageIdId:function(pid,id){var doc=JsonDataBinding.getDocumentById(pid);if(doc){return doc.getElementById(id);}},addWindow:function(w){_addWindow(w);},getParentWindowValue:function(name){if(window.parent!=window){return window.parent[name];}},setParentWindowValue:function(name,v){if(window.parent!=window){window.parent[name]=v;}},getParentWindowUrl:function(name){if(window.parent!=window){return window.parent.document.URL;}
return'';},isValueTrue:function(v){if(v==null){return false;}
var t=typeof v;if(t=='undefined')
return false;if(t=='boolean'){return v;}
else if(t=='number'){return v!=0;}
else if(t=='string'){if(v.length==0){return false;}
else if(v=='none'){return false;}
else{if((/^\s*false\s*$/i).test(v)){return false}
else if((/^\s*0*\s*$/).test(v)){return false}
else if((/^\s*no\s*$/i).test(v)){return false}
else if((/^\s*off\s*$/i).test(v)){return false}}}
return true;},applyData:function(data,dataAttrs){if(data){_setDataSource.call(data,dataAttrs);}},windowTools:{scrollBarPadding:17,centerElementOnScreen:function(element){this.updateDimensions();var left=((this.pageDimensions.horizontalOffset()+this.pageDimensions.windowWidth()/2)-(this.scrollBarPadding+element.offsetWidth/2));var top=((this.pageDimensions.verticalOffset()+this.pageDimensions.windowHeight()/2)-(this.scrollBarPadding+element.offsetHeight/2));if(left<0)left=0;if(top<0)top=0;element.style.top=top+'px';element.style.left=left+'px';element.style.position='absolute';},updateDimensions:function(){this.updatePageSize();this.updateWindowSize();this.updateScrollOffset();},updatePageSize:function(){var viewportWidth,viewportHeight;if(window.innerHeight&&window.scrollMaxY){viewportWidth=document.body.scrollWidth;viewportHeight=window.innerHeight+window.scrollMaxY;}
else
if(document.body.scrollHeight>document.body.offsetHeight){viewportWidth=document.body.scrollWidth;viewportHeight=document.body.scrollHeight;}
else{viewportWidth=document.body.offsetWidth;viewportHeight=document.body.offsetHeight;};this.pageSize={viewportWidth:viewportWidth,viewportHeight:viewportHeight};},updateWindowSize:function(){var windowWidth,windowHeight;if(self.innerHeight){windowWidth=self.innerWidth;windowHeight=self.innerHeight;}
else
if(document.documentElement&&document.documentElement.clientHeight){windowWidth=document.documentElement.clientWidth;windowHeight=document.documentElement.clientHeight;}
else
if(document.body){windowWidth=document.body.clientWidth;windowHeight=document.body.clientHeight;};this.windowSize={windowWidth:windowWidth,windowHeight:windowHeight};},updateScrollOffset:function(){var horizontalOffset,verticalOffset;if(self.pageYOffset){horizontalOffset=self.pageXOffset;verticalOffset=self.pageYOffset;}
else
if(document.documentElement&&document.documentElement.scrollTop){horizontalOffset=document.documentElement.scrollLeft;verticalOffset=document.documentElement.scrollTop;}
else if(document.body){horizontalOffset=document.body.scrollLeft;verticalOffset=document.body.scrollTop;};this.scrollOffset={horizontalOffset:horizontalOffset,verticalOffset:verticalOffset};},pageSize:{},windowSize:{},scrollOffset:{},pageDimensions:{pageWidth:function(){return JsonDataBinding.windowTools.pageSize.viewportWidth>JsonDataBinding.windowTools.windowSize.windowWidth?JsonDataBinding.windowTools.pageSize.viewportWidth:JsonDataBinding.windowTools.windowSize.windowWidth;},pageHeight:function(){return JsonDataBinding.windowTools.pageSize.viewportHeight>JsonDataBinding.windowTools.windowSize.windowHeight?JsonDataBinding.windowTools.pageSize.viewportHeight:JsonDataBinding.windowTools.windowSize.windowHeight;},windowWidth:function(){return JsonDataBinding.windowTools.windowSize.windowWidth;},windowHeight:function(){return JsonDataBinding.windowTools.windowSize.windowHeight;},horizontalOffset:function(){return JsonDataBinding.windowTools.scrollOffset.horizontalOffset;},verticalOffset:function(){return JsonDataBinding.windowTools.scrollOffset.verticalOffset;}}},adjustElementHeight:function(e){if(e&&e.children){var h=0;for(var i=0;i<e.children.length;i++){if(e.children[i]){var t=e.children[i].offsetTop+e.children[i].offsetHeight;if(t>h){h=t;}}}
if(h>0){e.style.height=(h+5)+'px';}}},anchorAlign:{getElementWidth:function(p,pageSize){if(p==document.body){if(pageSize){return pageSize.w;}
else{JsonDataBinding.windowTools.updateDimensions();return JsonDataBinding.windowTools.pageDimensions.windowWidth();}}
else
return Math.max(p.offsetWidth,p.scrollWidth);},getElementHeight:function(p,pageSize){if(p==document.body){if(pageSize){return pageSize.h;}
else{JsonDataBinding.windowTools.updateDimensions();return JsonDataBinding.windowTools.pageDimensions.windowHeight();}}
else
return Math.max(p.offsetHeight,p.scrollHeight);},getElementSize:function(p,pageSize){if(p==document.body){if(pageSize){return pageSize;}
else{JsonDataBinding.windowTools.updateDimensions();return{'w':JsonDataBinding.windowTools.pageDimensions.windowWidth(),'h':JsonDataBinding.windowTools.pageDimensions.windowHeight()};}}
else{return{'w':Math.max(p.offsetWidth,p.scrollWidth),'h':Math.max(p.offsetHeight,p.scrollHeight)};}},initializeAnchor:function(e,pageSize){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){if(typeof a.getAttribute!='undefined'){var ah=a.getAttribute('anchor');if(typeof ah!='undefined'&&ah!=null&&ah!=''){var ahs=ah.split(',');for(var k=0;k<ahs.length;k++){if(ahs[k]=='right'||ahs[k]=='bottom'){var psize=JsonDataBinding.anchorAlign.getElementSize(a.parentElement?a.parentElement:a.parentNode,pageSize);a.anchorSize={'x':psize.w-a.offsetLeft-a.offsetWidth,'y':psize.h-a.offsetTop-a.offsetHeight};break;}}}
JsonDataBinding.anchorAlign.initializeAnchor(a,pageSize);}}}},initializeBodyAnchor:function(){JsonDataBinding.windowTools.updateDimensions();var pageSize={'w':JsonDataBinding.windowTools.pageDimensions.windowWidth(),'h':JsonDataBinding.windowTools.pageDimensions.windowHeight()};JsonDataBinding.anchorAlign.initializeAnchor(document.body,pageSize);},anchorRight:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var x=pw-e.anchorSize.x-e.offsetWidth;if(x<0)x=0;e.style.left=x+'px';}},anchorBottom:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var y=ph-e.anchorSize.y-e.offsetHeight;if(y<0)y=0;e.style.top=y+"px";}},anchorLeftRight:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var w=pw-e.anchorSize.x-e.offsetLeft;if(w>0){e.style.width=w+"px";}}},anchorTopBottom:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var h=ph-e.anchorSize.y-e.offsetTop;if(h>0){e.style.height=h+"px";}}},alignCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var w=e.offsetWidth;var h=e.offsetHeight;var x=(ps.w-w)/2;var y=(ps.h-h)/2;if(x<0)x=0;if(y<0)y=0;e.style.left=x+'px';e.style.top=y+'px';},alignTopCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var w=e.offsetWidth;var x=(pw-w)/2;if(x<0)x=0;e.style.left=x+'px';},alignBottomCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var h=e.offsetHeight;var w=e.offsetWidth;var y=ps.h-h;var x=(ps.w-w)/2;if(x<0)x=0;if(y<0)y=0;e.style.left=x+'px';e.style.top=y+'px';},alignTopRightElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var w=e.offsetWidth;var x=(pw-w);if(x<0)x=0;e.style.left=x+'px';},alignLeftCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var h=e.offsetHeight;var y=(ph-h)/2;if(y<0)y=0;e.style.top=y+"px";},alignLeftBottomElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var h=e.offsetHeight;var y=(ph-h);if(y<0)y=0;e.style.top=y+"px";},alignCenterRightElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var w=e.offsetWidth;var h=e.offsetHeight;var x=(ps.w-w);var y=(ps.h-h)/2;if(x<0)x=0;if(y<0)y=0;e.style.top=y+'px';e.style.left=x+'px';},alignBottomRightElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var es=JsonDataBinding.anchorAlign.getElementSize(e,pageSize);var x=(ps.w-es.w)-2;var y=(ps.h-es.h)-2;if(x<0)x=0;if(y<0)y=0;e.style.left=x+'px';e.style.top=y+'px';},applyBodyAnchorAlign:function(){JsonDataBinding.windowTools.updateDimensions();var pageSize={'w':JsonDataBinding.windowTools.pageDimensions.windowWidth(),'h':JsonDataBinding.windowTools.pageDimensions.windowHeight()};JsonDataBinding.anchorAlign.applyAnchorAlign(document.body,pageSize);},applyAnchorAlign:function(e,pageSize){if(!e)return;for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){if(typeof a.getAttribute!='undefined'){var ah=a.getAttribute('anchor');if(typeof ah!='undefined'&&ah!=null&&ah!=''){var ahs=ah.split(',');var ahLeft=false;var ahRight=false;var ahTop=false;var ahBottom=false;var posAlign='';for(var k=0;k<ahs.length;k++){if(ahs[k]=='right')ahRight=true;else if(ahs[k]=='bottom')ahBottom=true;else if(ahs[k]=='left')ahLeft=true;else if(ahs[k]=='top')ahTop=true;else posAlign=ahs[k];}
var bAdjusted=false;if(ahRight||ahBottom){if(ahRight){if(ahLeft)
JsonDataBinding.anchorAlign.anchorLeftRight(a,pageSize);else
JsonDataBinding.anchorAlign.anchorRight(a,pageSize);bAdjusted=true;}
if(ahBottom){if(ahTop)
JsonDataBinding.anchorAlign.anchorTopBottom(a,pageSize);else
JsonDataBinding.anchorAlign.anchorBottom(a,pageSize);bAdjusted=true;}
if((ahRight&&ahLeft)||(ahBottom&&ahTop)){JsonDataBinding.anchorAlign.applyAnchorAlign(a,pageSize);bAdjusted=true;}}
else{bAdjusted=true;if(posAlign=='center')
JsonDataBinding.anchorAlign.alignCenterElement(a,pageSize);else if(posAlign=='topcenter')
JsonDataBinding.anchorAlign.alignTopCenterElement(a,pageSize);else if(posAlign=='topright')
JsonDataBinding.anchorAlign.alignTopRightElement(a,pageSize);else if(posAlign=='leftcenter')
JsonDataBinding.anchorAlign.alignLeftCenterElement(a,pageSize);else if(posAlign=='leftbottom')
JsonDataBinding.anchorAlign.alignLeftBottomElement(a,pageSize);else if(posAlign=='bottomcenter')
JsonDataBinding.anchorAlign.alignBottomCenterElement(a,pageSize);else if(posAlign=='centerright')
JsonDataBinding.anchorAlign.alignCenterRightElement(a,pageSize);else if(posAlign=='bottomright')
JsonDataBinding.anchorAlign.alignBottomRightElement(a,pageSize);else
bAdjusted=false;}
if(bAdjusted){function refreshDataRepeater(e0){if(e0&&e0.children){for(var i=0;i<e0.children.length;i++){if(e0.children[i].IsDataRepeater&&e0.children[i].jsData){e0.children[i].jsData.refreshPage();}
else{refreshDataRepeater(e0.children[i]);}}}}
refreshDataRepeater(a);if(a.onAdjustAnchorAlign){a.onAdjustAnchorAlign();}}}}}}}},ElementPosition:{_elementPos:function(){function __getIEVersion(){var rv=-1;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null)
rv=parseFloat(RegExp.$1);}
return rv;}
function __getOperaVersion(){var rv=0;if(window.opera){var sver=window.opera.version();rv=parseFloat(sver);}
return rv;}
var __userAgent=navigator.userAgent;var __isIE=navigator.appVersion.match(/MSIE/)!=null;var __IEVersion=__getIEVersion();var __isIENew=__isIE&&__IEVersion>=8;var __isIEOld=__isIE&&!__isIENew;var __isFireFox=__userAgent.match(/firefox/i)!=null;var __isFireFoxOld=__isFireFox&&((__userAgent.match(/firefox\/2./i)!=null)||(__userAgent.match(/firefox\/1./i)!=null));var __isFireFoxNew=__isFireFox&&!__isFireFoxOld;var __isWebKit=navigator.appVersion.match(/WebKit/)!=null;var __isChrome=navigator.appVersion.match(/Chrome/)!=null;var __isOpera=window.opera!=null;var __operaVersion=__getOperaVersion();var __isOperaOld=__isOpera&&(__operaVersion<10);function __parseBorderWidth(width){var res=0;if(typeof(width)=="string"&&width!=null&&width!=""){var p=width.indexOf("px");if(p>=0){res=parseInt(width.substring(0,p));}
else{res=1;}}
return res;}
function __getBorderWidth(element){var res=new Object();res.left=0;res.top=0;res.right=0;res.bottom=0;if(window.getComputedStyle){var elStyle=window.getComputedStyle(element,null);res.left=parseInt(elStyle.borderLeftWidth.slice(0,-2));res.top=parseInt(elStyle.borderTopWidth.slice(0,-2));res.right=parseInt(elStyle.borderRightWidth.slice(0,-2));res.bottom=parseInt(elStyle.borderBottomWidth.slice(0,-2));}
else{res.left=__parseBorderWidth(element.style.borderLeftWidth);res.top=__parseBorderWidth(element.style.borderTopWidth);res.right=__parseBorderWidth(element.style.borderRightWidth);res.bottom=__parseBorderWidth(element.style.borderBottomWidth);}
return res;}
getElementAbsolutePos=function(element){var res=new Object();res.x=0;res.y=0;if(element!==null){try{if(element.getBoundingClientRect){var viewportElement;if(IsSafari()||IsChrome()){viewportElement=document.body;}
else{viewportElement=document.documentElement;}
var box=element.getBoundingClientRect();var scrollLeft=viewportElement.scrollLeft;var scrollTop=viewportElement.scrollTop;res.x=box.left+scrollLeft;res.y=box.top+scrollTop;}
else{res.x=element.offsetLeft;res.y=element.offsetTop;var parentNode=element.parentNode;var borderWidth=null;while(offsetParent!=null){res.x+=offsetParent.offsetLeft;res.y+=offsetParent.offsetTop;var parentTagName=offsetParent.tagName.toLowerCase();if((__isIEOld&&parentTagName!="table")||((__isFireFoxNew||__isChrome)&&parentTagName=="td")){borderWidth=kGetBorderWidth(offsetParent);res.x+=borderWidth.left;res.y+=borderWidth.top;}
if(offsetParent!=document.body&&offsetParent!=document.documentElement){res.x-=offsetParent.scrollLeft;res.y-=offsetParent.scrollTop;}
if(!__isIE&&!__isOperaOld||__isIENew){while(offsetParent!=parentNode&&parentNode!==null){res.x-=parentNode.scrollLeft;res.y-=parentNode.scrollTop;if(__isFireFoxOld||__isWebKit){borderWidth=kGetBorderWidth(parentNode);res.x+=borderWidth.left;res.y+=borderWidth.top;}
parentNode=parentNode.parentNode;}}
parentNode=offsetParent.parentNode;offsetParent=offsetParent.offsetParent;}}}
catch(err){if(element.parentNode){return getElementAbsolutePos(element.parentNode);}}}
return res;}}(),getElementPosition:function(element){return getElementAbsolutePos(element);}},_textBoxObserver:function(){var textBoxes;var timerId;var poll=function(){if(!textBoxes)return;for(var i=0;i<textBoxes.length;i++){var ctrl=textBoxes[i];if(!ctrl.disableMonitor){var changed=false;if(ctrl.isCheckBox){if(ctrl.val!=ctrl.checked){if(ctrl.val==null){if(ctrl.checked!=null&&ctrl.checked!='null'){changed=true;}}
else{changed=true;}}}
else if(ctrl.isHtml){if(ctrl.val!=ctrl.innerHTML){if(ctrl.val==null){if(ctrl.innerHTML!=null&&ctrl.innerHTML!='null'){changed=true;}}
else{changed=true;}}}
else if(ctrl.isInnerText){var txt=JsonDataBinding.GetInnerText(ctrl);if(ctrl.val!=txt){if(ctrl.val==null){if(txt!=null&&txt!='null'){changed=true;}}
else{changed=true;}}}
else{if(ctrl.val!=ctrl.value){if(ctrl.val==null){if(ctrl.nullDisplayEmpty){if(ctrl.value!=null&&ctrl.value!=JsonDataBinding.NullDisplay){changed=true;}}
else if(ctrl.value!=null&&ctrl.value!='null'){changed=true;}}
else{changed=true;}}}
if(changed){var evt;if(ctrl.isCheckBox){ctrl.val=ctrl.checked;if(ctrl.onCheckedChanged){ctrl.onCheckedChanged(ctrl);}}
else if(ctrl.isHtml){ctrl.val=ctrl.innerHTML;if(ctrl.oninnerHtmlChanged){ctrl.oninnerHtmlChanged(ctrl);}}
else if(ctrl.isInnerText){ctrl.val=JsonDataBinding.GetInnerText(ctrl);if(ctrl.onsetbounddata){ctrl.onsetbounddata(ctrl);}
if(ctrl.ontxtChange){ctrl.ontxtChange({target:ctrl});}
else{JsonDataBinding.fireEvent(ctrl,'change');}}
else{ctrl.val=ctrl.value;if(ctrl.onsetbounddata){ctrl.onsetbounddata(ctrl);}
if(ctrl.ontxtChange){ctrl.ontxtChange({target:ctrl});}
else{JsonDataBinding.fireEvent(ctrl,'change');}}}}}}
AddTextBox=function(textBox){if(typeof textBoxes=='undefined'){textBoxes=new Array();}
var found=false;for(var i=0;i<textBoxes.length;i++){if(textBoxes[i]==textBox){found=true;break;}}
if(!found){if(textBox.isCheckBox){textBox.val=textBox.checked;}
else{var tag=textBox.tagName.toLowerCase();if(tag=='div'){textBox.val=textBox.innerHTML;textBox.isHtml=true;}
else{textBox.val=textBox.value;textBox.isHtml=false;textBox.isInnerText=false;}}
textBoxes.push(textBox);}
if(typeof timerId=='undefined'){timerId=window.setInterval(poll,300);}}
ShowTextBoxCount=function(){if(typeof textBoxes=='undefined')
return 0;return textBoxes.length;}
_pollModifications=function(){poll();}}(),addTextBoxObserver:function(textBox){AddTextBox(textBox);},pollModifications:function(){_pollModifications();},HtmlTableData:function(tableElement,jsTable){var _tblElement=tableElement;var _jsonTable=jsTable;var _readOnly=false;var _rowTemplate;var _actCtrls;var _selectedRow;var _textBoxElement;var _buttonElement;var _selectionElement;var _datetimePickerButton;var _lookupTableElements;var _chklstTableElements;var EDITOR_NONE=-1;var EDITOR_TEXT=0;var EDITOR_ENUM=1;var EDITOR_DATETIME=2;var EDITOR_DBLOOKUP=3;var EDITOR_CHKLIST=4;var attr=_tblElement.getAttribute('readonly');if(typeof attr!='undefined'&&attr){_readOnly=true;}
var tbody=JsonDataBinding.getTableBody(_tblElement);var k;if(tbody.rows.length>0){_rowTemplate=new Array();for(k=0;k<tbody.rows[0].cells.length;k++){_rowTemplate[k]=tbody.rows[0].cells[k].style.cssText;}}
else{var th=getHeader();if(th.rows&&th.rows.length>0){_rowTemplate=new Array();for(k=0;k<th.rows[0].cells.length;k++){_rowTemplate[k]=th.rows[0].cells[k].style.cssText;}}}
if(_tblElement.ActControls){_actCtrls=new Array();for(k=0;k<_tblElement.ActControls.length;k++){var ac=document.getElementById(_tblElement.ActControls[k]);if(ac){_actCtrls.push(ac);}}}
function showActCtrls(cells){if(_actCtrls&&_actCtrls.length>0){var tdAct;if(cells.length<=_jsonTable.Columns.length){}
else{tdAct=cells[_jsonTable.Columns.length];}
if(tdAct){if(_tblElement.ActColWidth){tdAct.style.width=_tblElement.ActColWidth+'px';}
for(c=0;c<_actCtrls.length;c++){_actCtrls[c].style.position='static';_actCtrls[c].style.left='auto';_actCtrls[c].style.top='auto';tdAct.appendChild(_actCtrls[c]);_actCtrls[c].style.display='inline';}}}}
function init(){JsonDataBinding.addvaluechangehandler(_jsonTable.TableName,_tblElement);JsonDataBinding.AttachOnRowDeleteHandler(_jsonTable.TableName,onrowdelete);if(_tblElement.ReadOnlyFields){if(_tblElement.ReadOnlyFields.length>0){for(var i=0;i<_tblElement.ReadOnlyFields.length;i++){var cn=_tblElement.ReadOnlyFields[i].toLowerCase();for(var c=0;c<_jsonTable.Columns.length;c++){if(cn==_jsonTable.Columns[c].Name.toLowerCase()){_jsonTable.Columns[c].ReadOnly=true;break;}}}}}
recreateTableElement();}
if(_tblElement.FieldEditors){for(var c=0;c<_jsonTable.Columns.length;c++){var ed=_tblElement.FieldEditors[c];if(ed){if(ed.Editor==EDITOR_CHKLIST){var tblList=JsonDataBinding.createCheckedList(ed.Tablename,_tblElement,c);if(!_chklstTableElements){_chklstTableElements={};}
_chklstTableElements[ed.Tablename]=tblList;tblList.chklist.setPosition("absolute");ed.CellPainter=tblList.chklist;}}}}
function getCellLocation(element){return JsonDataBinding.ElementPosition.getElementPosition(element);}
function getCell(e){var c=getEventSender(e);if(c){if(c.tr){return c;}
if(c.ownerCell){return c.ownerCell;}}
return null;}
function createCell(c){var td=document.createElement('TD');if(_rowTemplate){if(_rowTemplate.length>c&&_rowTemplate[c]&&_rowTemplate[c].length>0){td.style.cssText=_rowTemplate[c];}}
if(_tblElement){if(_tblElement.ColumnAligns&&_tblElement.ColumnAligns[c]){td.align=_tblElement.ColumnAligns[c];}
if(_tblElement.ColumnWidths&&_tblElement.ColumnWidths[c]){td.style.width=_tblElement.ColumnWidths[c];}
if(_tblElement.InvisibleColumns&&_tblElement.InvisibleColumns[c]){td.style.display='none';}}
return td;}
function showCell(td,c,val0){var editor;var isBlob=false;var val=JsonDataBinding.toText(val0);if(_jsonTable.Columns.length>c){isBlob=(_jsonTable.Columns[c].Type==252);}
var isImage=false;var isImages=JsonDataBinding.getObjectProperty(_jsonTable.TableName,'IsFieldImage');if(isImages&&isImages.length>c){isImage=isImages[c];}
if(_tblElement.FieldEditors&&_tblElement.FieldEditors[c]){editor=_tblElement.FieldEditors[c];}
if(editor&&editor.Editor==EDITOR_ENUM){var found=false;if(editor.Values){for(var i=0;i<editor.Values.length;i++){if(editor.Values[i][1]==val){found=true;var txt=document.createTextNode(editor.Values[i][0]);td.appendChild(txt);break;}}}
if(!found){var txt2=document.createTextNode(val);td.appendChild(txt2);}}
else if(editor&&editor.CellPainter){td.showCell=editor.CellPainter.showCell(td,c,val);}
else{if(typeof val=='undefined'||val==null){td.appendChild(document.createTextNode(JsonDataBinding.NullDisplayText));}
else{if(isImage){var img=td.getElementsByTagName('img');if(img&&img.length>0){img=img[0];}
else{img=document.createElement('img');td.appendChild(img);}
if(isBlob){img.src='data:image/jpg;base64,'+val;}
else{img.src=val;}}
else{if(_tblElement.ColumnAsHTML&&_tblElement.ColumnAsHTML[c]){td.innerHTML=val;}
else{var txt3=document.createTextNode(val);td.appendChild(txt3);}}}}}
function addHtmlTableRow(tbody,r){var isDeleted=(_jsonTable.Rows[r].deleted||_jsonTable.Rows[r].removed);var tr=tbody.insertRow(-1);tr.datarownum=r;for(var c=0;c<_jsonTable.Columns.length;c++){var td=createCell(c);JsonDataBinding.AttachEvent(td,'onmouseover',onCellMouseOver);JsonDataBinding.AttachEvent(td,'onmouseout',onCellMouseOut);JsonDataBinding.AttachEvent(td,'onclick',onCellClick);showCell(td,c,_jsonTable.Rows[r].ItemArray[c]);td.datarownum=r;td.columnIndex=c;td.tr=tr;tr.appendChild(td);}
if(_actCtrls&&_actCtrls.length>0){var tdAct=document.createElement('td');tdAct.datarownum=r;JsonDataBinding.AttachEvent(tdAct,'onmouseover',onCellMouseOver);JsonDataBinding.AttachEvent(tdAct,'onmouseout',onCellMouseOut);JsonDataBinding.AttachEvent(tdAct,'onclick',onCellClick);tdAct.tr=tr;tr.appendChild(tdAct);}
if(isDeleted){tr.style.display='none';}
else{tr.style.display='';}
if(_jsonTable.rowIndex==r){_selectedRow=tr;}
if(_tblElement.AlternateBackgroundColor&&r%2!=0){tr.style.backgroundColor=_tblElement.AlternateBackgroundColor;}}
function getHeader(){var th=_tblElement.tHead;if(typeof th=='undefined'||th==null){th=document.createElement('thead');_tblElement.appendChild(th);}
return th;}
function recreateTableElement(){_selectedRow=null;var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody==null){tbody=document.createElement('tbody');_tblElement.appendChild(tbody);}
tbody.isJs=true;while(tbody.rows.length>0){tbody.deleteRow(tbody.rows.length-1);}
var th=getHeader();var tr;if(th.rows.length>0){tr=th.rows[0];}
else{tr=document.createElement('tr');th.appendChild(tr);}
var c;for(c=0;c<_jsonTable.Columns.length;c++){if(c<tr.cells.length){if(tr.cells[c].innerHTML.length==0){JsonDataBinding.SetInnerText(tr.cells[c],_jsonTable.Columns[c].Name);}}
else{var td=document.createElement('td');td.appendChild(document.createTextNode(_jsonTable.Columns[c].Name));tr.appendChild(td);}
if(_tblElement&&_tblElement.InvisibleColumns&&_tblElement.InvisibleColumns[c]){tr.cells[c].style.display='none';}}
for(var r=0;r<_jsonTable.Rows.length;r++){addHtmlTableRow(tbody,r);}
if(_selectedRow!=null){var cells=_selectedRow.getElementsByTagName("td");showActCtrls(cells);if(_tblElement.SelectedRowColor){for(c=0;c<cells.length;c++){cells[c].style.backgroundColor=_tblElement.SelectedRowColor;}}}}
function _onDataReady(jsTable){_jsonTable=jsTable;init();}
function onCellMouseOver(e){var tbody=JsonDataBinding.getTableBody(_tblElement);if(!tbody)
return;var cell=getCell(e);if(cell){var cells=cell.tr.getElementsByTagName("td");var bkc=tbody.style.backgroundColor;if(_tblElement.HighlightRowColor){bkc=_tblElement.HighlightRowColor;}
for(var c=0;c<cells.length;c++){cells[c].style.backgroundColor=bkc;}
if(_tblElement.HighlightCellColor){cell.style.backgroundColor=_tblElement.HighlightCellColor;}
cell.cellLocation=getCellLocation(cell);}}
function onCellMouseOut(e){var tbody=JsonDataBinding.getTableBody(_tblElement);if(!tbody)
return;var cell=getCell(e);if(cell){var cells=cell.tr.getElementsByTagName("td");var bkC;if(typeof _jsonTable.rowIndex!='undefined'&&cell.datarownum==_jsonTable.rowIndex){if(_tblElement.SelectedRowColor){bkC=_tblElement.SelectedRowColor;}
else{bkC=tbody.style.backgroundColor;}}
else{if(_tblElement.AlternateBackgroundColor&&cell.datarownum%2!=0){bkC=_tblElement.AlternateBackgroundColor;}
else{bkC=tbody.style.backgroundColor;}}
for(var c=0;c<cells.length;c++){cells[c].style.backgroundColor=bkC;}}}
function ontextboxvaluechange(){if(typeof _textBoxElement!='undefined'){if(!_textBoxElement.disableMonitor){if(typeof _textBoxElement.cell!='undefined'&&_textBoxElement.cell!=null){if(JsonDataBinding.GetInnerText(_textBoxElement.cell)!=_textBoxElement.value){JsonDataBinding.SetInnerText(_textBoxElement.cell,_textBoxElement.value);_jsonTable.Rows[_textBoxElement.cell.datarownum].changed=true;_jsonTable.Rows[_textBoxElement.cell.datarownum].ItemArray[_textBoxElement.cell.columnIndex]=_textBoxElement.value;JsonDataBinding.onvaluechanged(_jsonTable,_textBoxElement.cell.datarownum,_textBoxElement.cell.columnIndex,_textBoxElement.value);if(_tblElement.ColumnValueChanged){_tblElement.ColumnValueChanged(_tblElement,_textBoxElement.cell.datarownum,_textBoxElement.cell.columnIndex,_textBoxElement.value);}}}}}}
function oncellbuttonclick(){if(_tblElement.ReadOnly)return;var zi;if(_buttonElement){if(_buttonElement.cell){var cell=_buttonElement.cell;if(_buttonElement.editor==EDITOR_ENUM){var needFill=true;if(typeof _selectionElement!='undefined'&&_selectionElement.cell&&_selectionElement.cell.columnIndex==_buttonElement.cell.columnIndex){needFill=false;}
if(needFill){var newSel=document.createElement('select');newSel.style.position='absolute';zi=parseInt(_tblElement.style.zIndex?_tblElement.style.zIndex:0)+2;newSel.style.zIndex=zi;if(typeof _selectionElement!='undefined'){document.body.replaceChild(newSel,_selectionElement);}
else{document.body.appendChild(newSel);}
for(var i=0;i<_tblElement.FieldEditors[cell.columnIndex].Values.length;i++){var elOptNew=document.createElement('option');elOptNew.text=_tblElement.FieldEditors[cell.columnIndex].Values[i][0];elOptNew.value=_tblElement.FieldEditors[cell.columnIndex].Values[i][1];try{newSel.add(elOptNew,null);}
catch(ex){newSel.add(elOptNew);}}
_selectionElement=newSel;JsonDataBinding.AttachEvent(_selectionElement,'onclick',oncellselectionclick);JsonDataBinding.AttachEvent(_selectionElement,'onkeydown',oncellselectionkeydown);}
_selectionElement.cell=_buttonElement.cell;_selectionElement.style.left=(_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px';_selectionElement.style.top=(_buttonElement.offsetTop+_buttonElement.offsetHeight)+'px';_selectionElement.style.display='block';_selectionElement.size=_selectionElement.options.length;JsonDataBinding.windowTools.updateDimensions();if(_selectionElement.offsetHeight+_selectionElement.offsetTop>JsonDataBinding.windowTools.pageDimensions.windowHeight()){var newTop=_buttonElement.offsetTop-_selectionElement.offsetHeight;if(newTop>=0){_selectionElement.style.top=newTop+"px";}}
zi=JsonDataBinding.getPageZIndex(_selectionElement)+1;_selectionElement.style.zIndex=zi;_selectionElement.focus();}
else if(_buttonElement.editor==EDITOR_DBLOOKUP){if(!_lookupTableElements){_lookupTableElements={};}
if(_buttonElement.cell){if(_tblElement.FieldEditors){if(_tblElement.FieldEditors[_buttonElement.cell.columnIndex]){var tname=_tblElement.FieldEditors[_buttonElement.cell.columnIndex].TableName;var tbl=_lookupTableElements[tname];if(!tbl){var u=new Object();tbl=document.createElement("table");tbl.setAttribute("jsdb",tname);tbl.style.position='absolute';zi=1+parseInt((_tblElement.style.zIndex)?_tblElement.style.zIndex:1);tbl.style.zIndex=zi;tbl.border=1;tbl.id=tname;tbl.TargetTable=_tblElement;tbl.Editor=_tblElement.FieldEditors[_buttonElement.cell.columnIndex];tbl.style.backgroundColor='white';tbl.HighlightRowColor='#c0ffc0';tbl.SelectedRowColor='#c0c0ff';tbl.setAttribute('readonly','true');var _onmousedown=function(e){var target=getEventSender(e);if(target){var focusTbl;target=target.parentNode;while(target){if(target.tagName){if(target.tagName.toLowerCase()=="table"){focusTbl=target;break;}}
target=target.parentNode;}
if(focusTbl){if(focusTbl==tbl){return;}}}
tbl.style.display='none';}
document.body.appendChild(tbl);_lookupTableElements[tname]=tbl;zi=JsonDataBinding.getPageZIndex(tbl)+1;tbl.style.zIndex=zi;JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedown);var tbody;var tbds=tbl.getElementsByTagName('tbody');if(tbds){if(tbds.length>0){tbody=tbds[0];}}
if(!tbody){tbody=document.createElement('tbody');tbl.appendChild(tbody);}
var tr=tbody.insertRow(-1);var td=document.createElement("td");tr.appendChild(td);td.innerHTML="<font color=red>Loading data from web server...</font>";tbl.cell=_buttonElement.cell;tbl.style.left=(_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px';tbl.style.top=(_buttonElement.offsetTop+_buttonElement.offsetHeight)+'px';tbl.style.display='block';JsonDataBinding.executeServerMethod(tname,u);}
else{tbl.cell=_buttonElement.cell;tbl.style.left=(_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px';tbl.style.top=(_buttonElement.offsetTop+_buttonElement.offsetHeight)+'px';tbl.style.display='block';}
JsonDataBinding.windowTools.updateDimensions();if(tbl.offsetHeight+tbl.offsetTop>JsonDataBinding.windowTools.pageDimensions.windowHeight()){var newTop2=_buttonElement.offsetTop-tbl.offsetHeight;if(newTop2>=0){tbl.style.top=newTop2+"px";}}
tbl.focus();}}}}
else if(_buttonElement.editor==EDITOR_CHKLIST){if(!_chklstTableElements){_chklstTableElements={};}
if(_buttonElement.cell){if(_tblElement.FieldEditors){var ed=_tblElement.FieldEditors[_buttonElement.cell.columnIndex];if(ed){var tname2=ed.TableName;tbl=_chklstTableElements[tname2];if(!tbl){tbl=JsonDataBinding.createCheckedList(tname2,_tblElement,_buttonElement.cell.columnIndex);_chklstTableElements[tname2]=tbl;tbl.chklist.setPosition("absolute");ed.CellPainter=tbl.chklist;tbl.chklist.move((_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px',(_buttonElement.offsetTop+_buttonElement.offsetHeight)+'px');}
tbl.chklist.move((_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px',(_buttonElement.offsetTop+_buttonElement.offsetHeight)+'px');tbl.chklist.loadData();tbl.chklist.move((_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px',(_buttonElement.offsetTop+_buttonElement.offsetHeight)+'px');tbl.chklist.setDisplay("block");tbl.chklist.setzindex(parseInt(_buttonElement.style.zIndex?_buttonElement.style.zIndex:0)+1);JsonDataBinding.windowTools.updateDimensions();if(tbl.chklist.getBottom()>JsonDataBinding.windowTools.pageDimensions.windowHeight()){var newTop3=_buttonElement.offsetTop-tbl.offsetHeight;if(newTop3>=0){tbl.chklist.move((_buttonElement.cell.cellLocation.x+document.body.scrollLeft)+'px',newTop3+"px");}}
zi=JsonDataBinding.getPageZIndex(tbl)+1;tbl.style.zIndex=zi;tbl.focus();}}}}}}}
function oncellselectionchange(){if(_selectionElement.selectedIndex>=0&&_selectionElement.selectedIndex<_selectionElement.options.length){var val=_selectionElement.options[_selectionElement.selectedIndex].value;var txt=JsonDataBinding.toText(_jsonTable.Rows[_selectionElement.cell.datarownum].ItemArray[_selectionElement.cell.columnIndex]);if(txt!=val){JsonDataBinding.SetInnerText(_selectionElement.cell,_selectionElement.options[_selectionElement.selectedIndex].text);_jsonTable.Rows[_selectionElement.cell.datarownum].changed=true;_jsonTable.Rows[_selectionElement.cell.datarownum].ItemArray[_selectionElement.cell.columnIndex]=val;if(_tblElement.ColumnValueChanged){_tblElement.ColumnValueChanged(_tblElement,_selectionElement.cell.datarownum,_selectionElement.cell.columnIndex,val);}}}
_selectionElement.style.display='none';}
function oncellselectionclick(){oncellselectionchange();}
function oncellselectionkeydown(e){var evt=e||window.event;if(evt.keyCode==13){oncellselectionchange();}}
function onCellClick(e){if(typeof _textBoxElement!='undefined'){_textBoxElement.cell=null;_textBoxElement.style.display='none';}
if(typeof _buttonElement!='undefined'){_buttonElement.cell=null;_buttonElement.style.display='none';}
if(typeof _selectionElement!='undefined'){_selectionElement.cell=null;_selectionElement.style.display='none';}
if(typeof _datetimePickerButton!='undefined'){_datetimePickerButton.cell=null;_datetimePickerButton.style.display='none';}
if(!_lookupTableElements){for(var nm in _lookupTableElements){var t=_lookupTableElements[nm];if(t&&t.tagName&&t.tagName.toLowerCase()=='table'){t.style.display='none';}}}
var cell;if(e){if(typeof e.datarownum!='undefined'){cell=e;}}
if(!cell){cell=getCell(e);}
if(cell){_jsonTable.rowIndex=cell.datarownum;if(typeof cell.columnIndex!='undefined'&&!_tblElement.ReadOnly&&typeof _textBoxElement!='undefined'&&typeof _buttonElement!='undefined'){var cellReadOnly=false;if(typeof _jsonTable.Columns[cell.columnIndex].ReadOnly!='undefined'){cellReadOnly=_jsonTable.Columns[cell.columnIndex].ReadOnly;}
var editor=(cellReadOnly||_readOnly)?EDITOR_NONE:EDITOR_TEXT;if(typeof _tblElement.FieldEditors!='undefined'&&_tblElement.FieldEditors!=null){if(typeof _tblElement.FieldEditors[cell.columnIndex]!='undefined'&&_tblElement.FieldEditors[cell.columnIndex]!=null){if(typeof _tblElement.FieldEditors[cell.columnIndex].Editor!='undefined'){editor=_tblElement.FieldEditors[cell.columnIndex].Editor;}}}
if(editor==EDITOR_NONE){_textBoxElement.cell=null;_textBoxElement.style.display='none';_buttonElement.cell=null;_buttonElement.style.display='none';}
else{_buttonElement.editor=editor;if(editor==EDITOR_TEXT||editor==EDITOR_DATETIME){_textBoxElement.cell=null;_textBoxElement.disableMonitor=true;_textBoxElement.val=JsonDataBinding.GetInnerText(cell);_textBoxElement.value=JsonDataBinding.GetInnerText(cell);_textBoxElement.style.display='block';_textBoxElement.focus();cell.cellLocation=getCellLocation(cell);_textBoxElement.style.left=(cell.cellLocation.x+document.body.scrollLeft)+'px';_textBoxElement.style.top=(cell.cellLocation.y+document.body.scrollTop)+'px';_textBoxElement.style.width=cell.offsetWidth;_textBoxElement.cell=cell;_textBoxElement.disableMonitor=false;_buttonElement.cell=null;_buttonElement.style.display='none';if(editor==EDITOR_DATETIME){if(typeof _datetimePickerButton=='undefined'){var dp=JsonDataBinding.GetDatetimePicker();if(typeof dp!='undefined'){function onselecteddatetime(args){if(args&&args.date){_textBoxElement.value=JsonDataBinding.toText(args.date);ontextboxvaluechange();}}
var fts=_tblElement.DatePickerFonstSize?_tblElement.DatePickerFonstSize+'px':'12px';var opts={formElements:{},showWeeks:true,statusFormat:"l-cc-sp-d-sp-F-sp-Y",bounds:{position:"absolute",inputRight:true,fontSize:fts,inputTime:true}};opts.formElements[_textBoxElement.id]="Y-ds-m-ds-d";opts.callbackFunctions={'dateset':[onselecteddatetime]};_datetimePickerButton=dp.createDatePicker(opts);}}
if(typeof _datetimePickerButton!='undefined'){if(cell.offsetWidth>_datetimePickerButton.offsetWidth){_textBoxElement.style.width=(cell.offsetWidth-_datetimePickerButton.offsetWidth)+'px';}
var cx=cell.cellLocation.x+document.body.scrollLeft;var cy=cell.cellLocation.y+document.body.scrollTop;if(cx+cell.offsetWidth>_datetimePickerButton.offsetWidth){_datetimePickerButton.style.left=(cx+cell.offsetWidth-22)+'px';}
else{_datetimePickerButton.style.left=cx+'px';}
_datetimePickerButton.style.top=cy+'px';_datetimePickerButton.cell=cell;var zi=JsonDataBinding.getZOrder(_tblElement)+1;_datetimePickerButton.style.zIndex=zi;_datetimePickerButton.style.display='block';}}}
else if(editor==EDITOR_ENUM||editor==EDITOR_DBLOOKUP||editor==EDITOR_CHKLIST){_textBoxElement.cell=null;_textBoxElement.style.display='none';if(editor==EDITOR_ENUM){_buttonElement.src='images/dropdownbutton.jpg';}
else if(editor==EDITOR_DBLOOKUP){_buttonElement.src='libjs/qry.jpg';}
else if(editor==EDITOR_CHKLIST){_buttonElement.src='libjs/chklist.jpg';}
var cl=(cell.cellLocation.x+document.body.scrollLeft+cell.offsetWidth-_buttonElement.offsetWidth)+'px';_buttonElement.style.left=cl;_buttonElement.style.top=(cell.cellLocation.y+document.body.scrollTop)+'px';_buttonElement.style.width='17px';_buttonElement.style.height='15px';if(_buttonElement.offsetHeight>cell.offsetHeight){_buttonElement.style.height=cell.offsetHeight;_buttonElement.style.width=_buttonElement.style.height;}
_buttonElement.cell=cell;_buttonElement.style.display='block';if(_buttonElement.offsetHeight>cell.offsetHeight){_buttonElement.style.height=cell.offsetHeight;_buttonElement.style.width=_buttonElement.style.height;}
cl=(cell.cellLocation.x+document.body.scrollLeft+cell.offsetWidth-_buttonElement.offsetWidth)+'px';_buttonElement.style.left=cl;var zi2=JsonDataBinding.getZOrder(_tblElement)+1;_buttonElement.style.zIndex=zi2;}}}
if(_tblElement.TargetTable){_tblElement.style.display='none';if(_tblElement.TargetTable.jsData&&_tblElement.Editor&&_tblElement.Editor.Map){for(var i=0;i<_tblElement.Editor.Map.length;i++){var map=_tblElement.Editor.Map[i];var val=_getColumnValue(map[1]);_tblElement.TargetTable.jsData.setColumnValue(map[0],val);}
if(_tblElement.TargetTable.DatabaseLookupSelected){var cn=_tblElement.TargetTable.jsData.getColumnCount();var cid=-1;for(var i=0;i<cn;i++){if(_tblElement.Editor==_tblElement.TargetTable.FieldEditors[i]){cid=i;break;}}
_tblElement.TargetTable.DatabaseLookupSelected(_tblElement.TargetTable,cid);}}}
else{JsonDataBinding.onRowIndexChange(_jsonTable.TableName);}}}
function _clickCell(cell){var r=-1;if(cell){r=cell.datarownum;}
if(r>=0&&r<_jsonTable.Rows.length){if(r!=_jsonTable.rowIndex){_jsonTable.rowIndex=r;_onRowIndexChange(_jsonTable.TableName);}
if(_selectedRow&&_selectedRow.datarownum==r){onCellClick(cell);}}}
function _onRowIndexChange(name){if(name==_jsonTable.TableName){var tbody=JsonDataBinding.getTableBody(_tblElement);var bkc,cells,c;if(_selectedRow){if(_selectedRow.datarownum!=_jsonTable.rowIndex){cells=_selectedRow.getElementsByTagName("TD");bkc=tbody.style.backgroundColor;if(_tblElement.AlternateBackgroundColor&&_selectedRow.datarownum%2!=0){bkc=_tblElement.AlternateBackgroundColor;}
_selectedRow.style.backgroundColor=bkc;for(c=0;c<cells.length;c++){cells[c].style.backgroundColor=bkc;}}}
_selectedRow=null;var r;var rn=tbody.rows.length;if(rn<_jsonTable.Rows.length){for(r=rn;r<_jsonTable.Rows.length;r++){addHtmlTableRow(tbody,r);}}
for(r=0;r<tbody.rows.length;r++){if(typeof tbody.rows[r].datarownum!='undefined'){if(tbody.rows[r].datarownum==_jsonTable.rowIndex){_selectedRow=tbody.rows[r];if(typeof _textBoxElement!='undefined'){if(typeof _textBoxElement.cell!='undefined'&&_textBoxElement.cell!=null){if(_textBoxElement.cell.datarownum!=_jsonTable.rowIndex){_textBoxElement.cell=null;_textBoxElement.style.display='none';}}}
break;}}}
if(_selectedRow){if(_tblElement.SelectedRowColor){bkc=_tblElement.SelectedRowColor;}
else{bkc=tbody.style.backgroundColor;}
_selectedRow.style.backgroundColor=bkc;var row=_jsonTable.Rows[_selectedRow.datarownum];var dataRowVer=typeof row.rowVersion=='undefined'?0:row.rowVersion;var viewRowVer=typeof _selectedRow.rowVersion=='undefined'?0:_selectedRow.rowVersion;cells=_selectedRow.getElementsByTagName("TD");if(viewRowVer<dataRowVer){_selectedRow.rowVersion=dataRowVer;for(c=0;c<cells.length&&c<_jsonTable.Columns.length;c++){cells[c].innerHTML='';showCell(cells[c],c,row.ItemArray[c]);}}
showActCtrls(cells);for(c=0;c<cells.length;c++){cells[c].style.backgroundColor=bkc;}}}}
_tblElement.oncellvaluechange=function(name,r,c,value0){if(_jsonTable.TableName==name){var row;if(_selectedRow!=null&&_selectedRow.datarownum==r){row=_selectedRow;}
else{var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody){if(r>=0&&r<tbody.rows.length){row=tbody.rows[r];}}}
if(row){var value=JsonDataBinding.toText(value0);if(_tblElement.FieldEditors){if(_tblElement.FieldEditors[c]){if(_tblElement.FieldEditors[c].CellPainter){if(row.cells[c].showCell){row.cells[c].showCell.updateCell(value);return;}
else{row.cells[c].showCell=_tblElement.FieldEditors[c].CellPainter.showCell(row.cells[c],c,value);return;}}}}
if(typeof row.cells[c].childNodes!='undefined'){for(var i=0;i<row.cells[c].childNodes.length;i++){if(row.cells[c].childNodes[i].nodeName=='#text'){row.cells[c].childNodes[i].nodeValue=value;if(_textBoxElement&&_textBoxElement.cell==row.cells[c]){if(_textBoxElement.value!=value){_textBoxElement.value=value;}}
break;}}}}}}
function onrowdelete(name,r0){if(_jsonTable.TableName==name){for(var r=0;r<_tblElement.rows.length;r++){if(typeof _tblElement.rows[r].datarownum!='undefined'){if(_tblElement.rows[r].datarownum==r0){_tblElement.rows[r].style.display='none';if(_textBoxElement.cell!='undefined'&&_textBoxElement.cell!=null){if(_textBoxElement.cell.datarownum==r0){_textBoxElement.cell=null;_textBoxElement.style.display='none';}}
break;}}}}}
function _onSelectedRowColorChanged(){if(_selectedRow!=null){var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody){var bkc;if(_tblElement.SelectedRowColor){bkc=_tblElement.SelectedRowColor;}
else{if(_tblElement.AlternateBackgroundColor&&_selectedRow.datarownum%2!=0){bkc=_tblElement.AlternateBackgroundColor;}
else{bkc=tbody.style.backgroundColor;}}
_selectedRow.style.backgroundColor=bkc;for(var j=0;j<_selectedRow.cells.length;j++){_selectedRow.cells[j].style.backgroundColor=bkc;}}}}
function _onRowColorChanged(){var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody){if(tbody.rows){for(var i=0;i<tbody.rows.length;i++){if(_selectedRow&&_selectedRow.datarownum==i){continue;}
var bkc;if(_tblElement.AlternateBackgroundColor&&tbody.rows[i].datarownum%2!=0){bkc=_tblElement.AlternateBackgroundColor;}
else{bkc=tbody.style.backgroundColor;}
tbody.rows[i].style.backgroundColor=bkc;for(var j=0;j<tbody.rows[i].cells.length;j++){tbody.rows[i].cells[j].style.backgroundColor=bkc;}}
_onSelectedRowColorChanged();}}}
function _setEventHandler(eventName,func){if(eventName=='ColumnValueChanged'){eventCellValueChanged=func;}}
function _getColumnValueByColumnIndex(columnIndex){if(_jsonTable.rowIndex>=0&&_jsonTable.rowIndex<_jsonTable.Rows.length){return JsonDataBinding.columnValueByIndex(_jsonTable.TableName,columnIndex,_jsonTable.rowIndex);}
return null;}
function _getColumnValue(columnName){return _getColumnValueByColumnIndex(_columnNameToIndex(_jsonTable.TableName,columnName));}
function _setColumnValueByColumnIndex(columnIndex,val){if(_jsonTable.rowIndex>=0&&_jsonTable.rowIndex<_jsonTable.Rows.length){_jsonTable.Rows[_jsonTable.rowIndex].ItemArray[columnIndex]=val;_jsonTable.Rows[_jsonTable.rowIndex].changed=true;JsonDataBinding.onvaluechanged(_jsonTable,_jsonTable.rowIndex,columnIndex,val);}}
function _setColumnValue(columnName,val){_setColumnValueByColumnIndex(_columnNameToIndex(_jsonTable.TableName,columnName),val);}
function _getTableName(){return _jsonTable.TableName;}
function _getModifiedRowCount(){var r0=0;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].changed){r0++;}}
return r0;}
function _getDeletedRowCount(){var r0=0;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].deleted){r0++;}}
return r0;}
function _getNewRowCount(){var r0=0;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].added){r0++;}}
return r0;}
function _getRowCount(){var r0=_jsonTable.Rows.length;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].deleted){r0--;}}
return r0;}
_textBoxElement=document.createElement('input');_textBoxElement.id='txt'+_tblElement.id;_textBoxElement.type='text';_textBoxElement.style.position='absolute';var zi=JsonDataBinding.getZOrder(_tblElement)+1;_textBoxElement.style.zIndex=zi;JsonDataBinding.AttachEvent(_textBoxElement,'onchange',ontextboxvaluechange);document.body.appendChild(_textBoxElement);_textBoxElement.style.display='none';JsonDataBinding.addTextBoxObserver(_textBoxElement);_buttonElement=document.createElement('img');_buttonElement.style.position='absolute';_buttonElement.style.cursor='pointer';zi=JsonDataBinding.getZOrder(_tblElement)+1;_buttonElement.style.zIndex=zi;_buttonElement.src='images/dropdownbutton.jpg';document.body.appendChild(_buttonElement);_buttonElement.style.display='none';JsonDataBinding.AttachEvent(_buttonElement,'onclick',oncellbuttonclick);function _onmousedownForEnum(e){if(!_selectionElement)return;var target=getEventSender(e);if(target){while(target){if(_selectionElement==target)return;target=target.parentNode;}}
_selectionElement.style.display='none';}
function _setColumnVisible(cidx,visible){var changed=false;if(visible){if(_tblElement.InvisibleColumns&&_tblElement.InvisibleColumns[cidx]){changed=true;}}
else{if(!_tblElement.InvisibleColumns){_tblElement.InvisibleColumns={};_tblElement.InvisibleColumns[cidx]=true;changed=true;}
else if(!_tblElement.InvisibleColumns[cidx]){_tblElement.InvisibleColumns[cidx]=true;changed=true;}}
if(changed){var i;var stl=visible?'block':'none';var th=_tblElement.tHead;if(th&&th.rows){for(i=0;i<th.rows.length;i++){if(th.rows[i].cells&&th.rows[i].cells.length>cidx){th.rows[i].cells[cidx].style.display=stl;}}}
var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody&&tbody.rows){for(i=0;i<tbody.rows.length;i++){if(tbody.rows[i].cells&&tbody.rows[i].cells.length>cidx){tbody.rows[i].cells[cidx].style.display=stl;}}}}}
function _refreshBindColumnDisplay(name,rowidx,colIdx){if(_jsonTable.TableName==name){var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody&&tbody.rows){for(var r=0;r<tbody.rows.length;r++){if(tbody.rows[r].datarownum==rowidx){if(tbody.rows[r].cells&&tbody.rows[r].cells.length>colIdx){if(tbody.rows[r].cells[colIdx].showCell){tbody.rows[r].cells[colIdx].showCell.updateCell(JsonDataBinding.toText(_jsonTable.Rows[rowidx].ItemArray[colIdx]));}
else{var c=colIdx;var value=JsonDataBinding.toText(_jsonTable.Rows[rowidx].ItemArray[colIdx]);if(typeof tbody.rows[r].cells[c].childNodes!='undefined'){for(var i=0;i<tbody.rows[r].cells[c].childNodes.length;i++){if(tbody.rows[r].cells[c].childNodes[i].nodeName=='#text'){tbody.rows[r].cells[c].childNodes[i].nodeValue=value;if(_textBoxElement&&_textBoxElement.cell==tbody.rows[r].cells[c]){if(_textBoxElement.value!=value){_textBoxElement.value=value;}}
return;}}}
tbody.rows[r].cells[c].innerHTML=value;}}
break;}}}}}
JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedownForEnum);init();return{getColumnCount:function(){if(_jsonTable)
return _jsonTable.Columns.length;},getSelectedRowElement:function(){return _selectedRow;},onRowIndexChange:function(name){_onRowIndexChange(name);},oncellvaluechange:function(name,r,c,value){_oncellvaluechange(name,r,c,value);},onDataReady:function(jsData){_onDataReady(jsData);},onRowColorChanged:function(){_onRowColorChanged();},onSelectedRowColorChanged:function(){_onSelectedRowColorChanged();},setEventHandler:function(eventName,func){_setEventHandler(eventName,func);},getColumnValue:function(columnName){return _getColumnValue(columnName);},getColumnValueByColumnIndex:function(columnIndex){return _getColumnValueByColumnIndex(columnIndex);},setColumnValue:function(columnName,val){_setColumnValue(columnName,val);},setColumnValueByColumnIndex:function(columnIndex,val){_setColumnValueByColumnIndex(columnIndex,val);},getTableName:function(){return _getTableName();},clickCell:function(cell){_clickCell(cell);},getModifiedRowCount:function(){return _getModifiedRowCount();},getDeletedRowCount:function(){return _getDeletedRowCount();},getNewRowCount:function(){return _getNewRowCount();},setColumnVisible:function(cidx,visible){_setColumnVisible(cidx,visible);},refreshBindColumnDisplay:function(name,rowidx,colIdx){_refreshBindColumnDisplay(name,rowidx,colIdx);}}},HtmlListboxData:function(listElement,jsTable,itemFieldIdx,valueFieldIdx){var _listElement=listElement;var _jsonTable=jsTable
var _itemFieldIdx=itemFieldIdx;var _valueFieldIdx=valueFieldIdx;var _useBlank=listElement.getAttribute('useblank');function init(){JsonDataBinding.addvaluechangehandler(_jsonTable.TableName,_listElement);JsonDataBinding.AttachOnRowDeleteHandler(_jsonTable.TableName,onrowdelete);recreateListboxElements();if(_listElement.parentNode&&_listElement.parentNode.jsData&&_listElement.parentNode.jsData.onChildListBoxFilled){_listElement.parentNode.jsData.onChildListBoxFilled();}}
function onrowdelete(name,r0){if(_jsonTable.TableName==name){var items=_listElement.getElementsByTagName('option');for(var r=0;r<items.length;r++){if(typeof items[r].datarownum!='undefined'){if(items[r].datarownum==r0){_listElement.remove(r);break;}}}}}
function getItem(e){return getEventSender(e);}
function onItemMouseOver(e){var op=getItem(e);if(typeof op!='undefined'){if(_listElement.HighlightRowColor){op.style.backgroundColor=_listElement.HighlightRowColor;}}}
function onItemMouseOut(e){var op=getItem(e);if(typeof op!='undefined'){var bkC;if(typeof _jsonTable.rowIndex!='undefined'&&op.datarownum==_jsonTable.rowIndex){if(_listElement.SelectedRowColor){bkC=_listElement.SelectedRowColor;}
else{bkC=_listElement.style.backgroundColor;}}
else{bkC=_listElement.style.backgroundColor;}
op.style.backgroundColor=bkC;}}
var addingMethod;function addElement(r){var isDeleted=(_jsonTable.Rows[r].deleted||_jsonTable.Rows[r].removed);if(!isDeleted){var text;var val;if(typeof _jsonTable.Rows[r].ItemArray[_itemFieldIdx]!='undefined'&&_jsonTable.Rows[r].ItemArray[_itemFieldIdx]!=null){text=JsonDataBinding.toText(_jsonTable.Rows[r].ItemArray[_itemFieldIdx]);}
else{text='';}
if(typeof _jsonTable.Rows[r].ItemArray[_valueFieldIdx]!='undefined'&&_jsonTable.Rows[r].ItemArray[_valueFieldIdx]!=null){val=_jsonTable.Rows[r].ItemArray[_valueFieldIdx];}
else{val='';}
var op=document.createElement('option');op.text=text;op.value=val;op.datarownum=r;if(!_useBlank&&_jsonTable.rowIndex==r){op.selected=true;}
if(typeof addingMethod=='undefined'){try{_listElement.add(op,null);addingMethod=true;}
catch(e){_listElement.add(op);addingMethod=false;}}
else{if(addingMethod){_listElement.add(op,null);}
else{_listElement.add(op);}}}}
function recreateListboxElements(){_listElement.options.length=0;_useBlank=_listElement.getAttribute('useblank');if(_useBlank){var op=document.createElement('option');op.text='';op.value=null;op.datarownum=-1;op.selected=true;if(typeof addingMethod=='undefined'){try{_listElement.add(op,null);addingMethod=true;}
catch(e){_listElement.add(op);addingMethod=false;}}
else{if(addingMethod){_listElement.add(op,null);}
else{_listElement.add(op);}}}
for(var r=0;r<_jsonTable.Rows.length;r++){addElement(r);}}
function onSelectedIndexChanged(){if(_listElement.selectedIndex>=0){var op=_listElement.options[_listElement.selectedIndex];if(typeof op!='undefined'&&op.datarownum!='undefined'){_jsonTable.rowIndex=op.datarownum;JsonDataBinding.onRowIndexChange(_jsonTable.TableName);}}}
function _onRowIndexChange(name){if(name==_jsonTable.TableName){var r;var rn=_listElement.options.length;var lastItem=_listElement.options[rn-1];var rn0=lastItem.datarownum;if(rn0<_jsonTable.Rows.length){for(r=rn0+1;r<_jsonTable.Rows.length;r++){addElement(r);}}
var r0=_jsonTable.rowIndex;var items=_listElement.getElementsByTagName('option');for(var r=0;r<items.length;r++){if(typeof items[r].datarownum!='undefined'){if(items[r].datarownum==r0){items[r].selected=true;break;}}}}}
function _onDataReady(jsData){_jsonTable=jsData;init();}
_listElement.oncellvaluechange=function(name,r,c,value){if((c==_itemFieldIdx||c==_valueFieldIdx)&&_jsonTable.TableName==name){var items=_listElement.getElementsByTagName('option');for(var r0=0;r0<items.length;r0++){if(typeof items[r0].datarownum!='undefined'){if(items[r0].datarownum==r){if(c==_itemFieldIdx){items[r0].text=value;}
else if(c==_valueFieldIdx){items[r0].value=value;}
break;}}}}}
init();JsonDataBinding.AttachEvent(_listElement,'onchange',onSelectedIndexChanged);return{onRowIndexChange:function(name){_onRowIndexChange(name);},onDataReady:function(jsData){_onDataReady(jsData);}}},FileSizeInLimit:function(maxBytes,fileControl){if(fileControl.files&&fileControl.files.length==1){if(fileControl.files[0].size>maxBytes){return false;}}
return true;},CreateFileUploader:function(fileInput){function getFileSizeControl(){var form=fileInput.parentNode;if(form){for(var i=0;i<form.children.length;i++){if(form.children[i].name=='MAX_FILE_SIZE'){return form.children[i];}}}}
return{IsFileSizeValid:function(showError){var sc=getFileSizeControl();if(sc){var maxSize=parseInt(sc.getAttribute("value"));if(maxSize<=0){maxSize=1024;}
if(JsonDataBinding.FileSizeInLimit(maxSize,fileInput)){return true;}
else{if(showError){if(maxSize>=1048576){alert("The file size must be less than "+(maxSize/1024/1024)+" MB");}
else{alert("The file size must be less than "+(maxSize/1024)+" KB");}}}}
return false;},SetMaxFileSize:function(msize){var sc=getFileSizeControl();if(sc){sc.setAttribute('value',msize);}},GetMaxFileSize:function(){var sc=getFileSizeControl();if(sc){return sc.getAttribute('value');}}};},FilesUploader:function(formName,imgWidth,displayElementName,displayImagePath){var _formName=formName;var _width=imgWidth;var fform=document.getElementById(_formName);var _displayElementName=displayElementName;var _displayImagePath=displayImagePath;function createFileInput(){var f=document.createElement("input");f.type='file';f.style.cssText="text-align: right; cursor:pointer; opacity:0.0;filter:alpha(opacity=0);moz-opacity:0; z-index: 1; position: absolute; left:0px; top:0px; width:"+_width+"px;";f.style.zIndex=1;f.onchange=function(event){onFileSelected(event);};f.name=_formName+"[]";fform.appendChild(f);if(IsFireFox()){f.style.left='-140px';}}
function addFileToDisplay(fn){if(_displayElementName){var sp1=document.getElementById(_displayElementName);var sp=document.createElement("span");if(_displayImagePath){var img=document.createElement("img");img.src=_displayImagePath;sp.appendChild(img);}
var spt=document.createElement("span");spt.innerHTML="<b>"+fn+",</b>";sp.appendChild(spt);sp1.appendChild(sp);}}
function clearDisplay(){if(_displayElementName){var sp1=document.getElementById(_displayElementName);sp1.innerHTML='';}}
function onFileSelected(e){var c=getEventSender(e);var fn=c.value;var inputs=fform.getElementsByTagName("input");var found=false;for(var i=0;i<inputs.length;i++){if(inputs[i].type.toLowerCase()=="file"){if(inputs[i].style.zIndex==0){if(inputs[i].value==fn){found=true;break;}}}}
if(found){fform.removeChild(c);}
else{c.style.zIndex=0;addFileToDisplay(fn);if(fform.onFileSelected){fform.onFileSelected();}}
createFileInput();}
function init(){createFileInput();}
function _clearFiles(){var found=false;for(var k=0;k<3;k++){var inputs=fform.getElementsByTagName("input");for(var i=0;i<inputs.length;i++){if(inputs[i].type.toLowerCase()=="file"){fform.removeChild(inputs[i]);}}}
clearDisplay();createFileInput();}
function _removeFile(filePath){if(filePath&&filePath.toLowerCase){filePath=filePath.replace(',','').toLowerCase();var f2=filePath.replace(/^.*\\/,'');var inputs=fform.getElementsByTagName("input");var found=false;var i;for(i=0;i<inputs.length;i++){if(inputs[i].type.toLowerCase()=="file"){if(inputs[i].style.zIndex==0){var isFile=false;if(IsFireFox()||IsChrome()){var filename=inputs[i].value.replace(/^.*\\/,'').toLowerCase();isFile=(filename==f2);}
else{isFile=(filePath==inputs[i].value.toLowerCase());}
if(isFile){fform.removeChild(inputs[i]);found=true;}}}}
if(found){if(_displayElementName){var sp1=document.getElementById(_displayElementName);sp1.innerHTML='';inputs=fform.getElementsByTagName("input");for(i=0;i<inputs.length;i++){if(inputs[i].value&&inputs[i].value.length>0){addFileToDisplay(inputs[i].value);}}}}}}
function getfiles(){var files=new Array();var inputs=fform.getElementsByTagName("input");for(var i=0;i<inputs.length;i++){if(inputs[i].value&&inputs[i].value.length>0){files.push(inputs[i].value);}}
return files;}
init();return{ClearFiles:function(){_clearFiles();},RemoveFile:function(filePath){_removeFile(filePath);},SelectedFiles:function(){return getfiles();}}},createCheckedList:function(name,targetTable,idx){var _tagetTable=targetTable;var _columnIndex=idx;var _editor=_tagetTable.FieldEditors[idx];var div=document.createElement("div");document.body.appendChild(div);div.style.border="1px solid black";div.style.backgroundColor="white";div.style.display='none';var imgok=document.createElement("img");div.appendChild(imgok);imgok.src="libjs/ok.png";imgok.style.cursor="pointer";var imgCancel=document.createElement("img");div.appendChild(imgCancel);imgCancel.src="libjs/cancel.png";imgCancel.style.cursor="pointer";var spanMsg=document.createElement('span');div.appendChild(spanMsg);spanMsg.innerHTML="<font color=red>Loading data...</font>";var tbl=document.createElement("table");div.appendChild(tbl);tbl.id=name;tbl.border=0;tbl.frame="void";tbl.style.display="block";tbl.setAttribute('readonly','true');tbl.setAttribute('jsdb',name);function _onmousedown(e){var target=getEventSender(e);if(target){var focusTbl;target=target.parentNode;while(target){if(target.tagName){if(target.tagName.toLowerCase()=="div"){focusTbl=target;break;}}
target=target.parentNode;}
if(focusTbl){if(focusTbl==div){return;}}}
div.style.display='none';}
var tbody;var tbd=tbl.getElementsByTagName('tbody');if(tbd){if(tbd.length>0){tbody=tbd[0];}}
if(!tbody){tbody=document.createElement('tbody');tbl.appendChild(tbody);}
function getCheckedItems(){var s;for(var r=0;r<tbody.rows.length;r++){var chks=tbody.rows[r].getElementsByTagName("input");if(chks[0].checked){var sps=tbody.rows[r].getElementsByTagName("span");if(s){s+=",";s+=sps[0].innerHTML;}
else{s=sps[0].innerHTML;}}}
if(!s){s='';}
return s;}
function onOK(){var s=getCheckedItems();_tagetTable.jsData.setColumnValueByColumnIndex(_columnIndex,s);div.style.display='none';}
function _setCheckedByTexts(texts){var r,chks;for(r=0;r<tbody.rows.length;r++){chks=tbody.rows[r].getElementsByTagName("input");chks[0].checked=false;}
if(texts&&texts!=''){var ts=texts.split(',');for(var i=0;i<ts.length;i++){for(r=0;r<tbody.rows.length;r++){var sps=tbody.rows[r].getElementsByTagName("span");if(sps[0].innerHTML==ts[i]){chks=tbody.rows[r].getElementsByTagName("input");chks[0].checked=true;break;}}}}}
function _applyTargetdata(){if(_tagetTable.jsData){var s=_tagetTable.jsData.getColumnValueByColumnIndex(_columnIndex);_setCheckedByTexts(s);}}
function _setMessage(msg){spanMsg.innerHTML=msg;}
function _addrow(text,value,checked){var tr=tbody.insertRow(-1);var td=document.createElement('td');tr.appendChild(td);var chk=document.createElement('input');chk.type="checkbox";td.appendChild(chk);var span=document.createElement('span');span.innerHTML=text;td.appendChild(span);var val=document.createElement('input');val.type="hidden";val.value=value;td.appendChild(val);if(checked){chk.checked=true;}}
JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedown);JsonDataBinding.AttachEvent(imgCancel,'onclick',function(){div.style.display='none';});JsonDataBinding.AttachEvent(imgok,'onclick',onOK);var _dataLoaded=false;tbl.chklist={dataLoaded:function(){return _dataLoaded;},getTable:function(){return tbl;},setMessage:function(msg){_setMessage(msg);},setPosition:function(pos){div.style.position=pos;},move:function(x,y){div.style.left=x;div.style.top=y;},getBottom:function(){return div.offsetHeight+div.offsetTop;},setDisplay:function(dis){div.style.display=dis;},setzindex:function(zi){div.style.zIndex=zi;},addrow:function(text,value,checked){_addrow(text,value,checked);},loadData:function(){if(_dataLoaded){_applyTargetdata();}
else{_dataLoaded=true;if(_editor.UseDb){var u=new Object();JsonDataBinding.executeServerMethod(_editor.TableName,u);}
else{for(var i=0;i<_editor.List.length;i++){_addrow(_editor.List[i],i);}
_setMessage("");_applyTargetdata();}}},loadRecords:function(records){for(var i=0;i<records.length;i++){_addrow(records[i].ItemArray[0],i);}
_setMessage("");_applyTargetdata();},setCheckedByTexts:function(texts){_setCheckedByTexts(texts);},applyTargetdata:function(){_applyTargetdata();},showCell:function(td,c,val){var _cell=td;var imgplus;var txt0;var divContents;function plusclick(){if(divContents.style.display=='block'){divContents.style.display='none';imgplus.src='libjs/plus.gif';}
else{divContents.style.display='block';imgplus.src='libjs/minus.gif';}}
if(typeof val=='undefined'||val==null){td.appendChild(document.createTextNode(JsonDataBinding.NullDisplayText));}
else{var s=''+JsonDataBinding.toText(val);if(s.length<20){var txt=document.createTextNode(s);td.appendChild(txt);}
else{var s0=s.substr(0,6)+' ...';var ss=s.replace(new RegExp(',','g'),'<br>');imgplus=document.createElement("img");imgplus.src='libjs/plus.gif';td.appendChild(imgplus);imgplus.style.display='block';imgplus.style.cursor='pointer';imgplus.align='left';txt0=document.createTextNode(s0);td.appendChild(txt0);divContents=document.createElement("div");td.appendChild(divContents);divContents.style.display='none';divContents.innerHTML=ss;divContents.ownerCell=_cell;var clickCell=function(){_tagetTable.jsData.clickCell(_cell);};JsonDataBinding.AttachEvent(imgplus,'onclick',plusclick);}}
return{updateCell:function(val){for(var i=0;i<_cell.childNodes.length;i++){if(_cell.childNodes[i].nodeValue){_cell.childNodes[i].nodeValue='';}}
if(typeof val=='undefined'||val==null){if(imgplus){imgplus.style.display='none';}
if(divContents){divContents.style.display='none';}
if(!txt0){txt0=document.createTextNode(JsonDataBinding.NullDisplayText);_cell.appendChild(txt0);}
else{txt0.nodeValue=JsonDataBinding.NullDisplayText;}}
else{var s=''+JsonDataBinding.toText(val);if(s.length<20){if(imgplus){imgplus.style.display='none';}
if(divContents){divContents.style.display='none';}
if(!txt0){txt0=document.createTextNode(s);td.appendChild(txt0);}
else{txt0.nodeValue=s;}}
else{var s0=s.substr(0,6)+' ...';var ss=s.replace(new RegExp(',','g'),'<br>');var expended=false;if(divContents){if(divContents.style.display=='block'){expended=true;}}
if(!imgplus){imgplus=document.createElement("img");_cell.appendChild(imgplus);imgplus.style.cursor='pointer';imgplus.align='left';JsonDataBinding.AttachEvent(imgplus,'onclick',plusclick);}
if(expended){imgplus.src='libjs/minus.gif';}
else{imgplus.src='libjs/plus.gif';}
imgplus.style.display='block';if(!txt0){txt0=document.createTextNode(s0);_cell.appendChild(txt0);}
else{txt0.nodeValue=s0;}
if(!divContents){divContents=document.createElement("div");_cell.appendChild(divContents);divContents.ownerCell=_cell;}
divContents.innerHTML=ss;if(expended){divContents.style.display='block';}
else{divContents.style.display='none';}}}}};}}
return tbl;},DataRepeater:function(divElement,jsTable){var _divElement=divElement;var _jsonTable=jsTable;var _readOnly=false;var _pageIndex=0;var _itemCount=0;var _currentGroupIndex=0;var _navigatorPages=5;var _navigatorStart=-1;var _items;var _pageNumerHolders;var _firstTime=true;var _elementEvents;function _getTotalPages(){if(_divElement.ShowAllRecords){return 1;}
if(_jsonTable){if(_itemCount>0){var d=0;for(var i=0;i<_jsonTable.Rows.length;i++){if(_jsonTable.Rows[i].deleted){d++;}}
return Math.ceil((_jsonTable.Rows.length-d)/_itemCount);}}
return 0}
function _adjustItemHeight(){function adjIH(e){for(var i=0;i<e.children.length;i++){if(e.children[i].style){e.children[i].style.height="";var o=e.children[i].style.overflow;e.children[i].style.overflow='scroll';e.children[i].scrollTop=1;e.children[i].scrollTop=0;e.children[i].style.height=e.children[i].scrollHeight+'px';e.children[i].style.overflow=o;adjIH(e.children[i]);}}}
for(var g=0;g<_items.length;g++){if(_items[g].style.display!='none'){adjIH(_items[g]);_items[g].style.height="";var o=_items[g].style.overflow;_items[g].style.overflow='scroll';_items[g].scrollTop=1;_items[g].scrollTop=0;_items[g].style.height=_items[g].scrollHeight+'px';_items[g].style.overflow=o;}}}
function _showPage(){function adjustDPpos(c){if(c.getAttribute('useDP')){var pid=c.getAttribute('useDPid');if(typeof pid=='undefined'||pid==null||pid.length==0){var dpbut=document.getElementById('fd-but-'+c.id);if(dpbut){dpbut.style.display=c.style.display;}
JsonDataBinding.adjustDatePickerButtonPos(c.id);}
else{var x=c.getAttribute('useDPx');var y=c.getAttribute('useDPy');if(x&&y){var dp=document.getElementById('fd-'+c.id);if(dp){dp.style.top=y+'px';dp.style.left=x+'px';}}
if(!c.dpset){c.dpset=true;if(c.getAttribute('useDPv')){var dpdiv=document.getElementById('fd-'+c.id);if(dpdiv){dpdiv.style.display='none';}}}}}
else{for(var i=0;i<c.children.length;i++){adjustDPpos(c.children[i]);}}}
var pn=_getTotalPages();if(_pageIndex>=0&&(_pageIndex<pn||pn==0)){var autoCR=typeof(_divElement.AutoColumnsAndRows)!='undefined'&&_divElement.AutoColumnsAndRows;var fillDirection=typeof(_divElement.ItemFillDirection)!='undefined'?_divElement.ItemFillDirection:0;var tbl;var topNavigatorHeight=0;if(autoCR){for(var i=0;i<_divElement.children.length;i++){var tag=_divElement.children[i].tagName.toLowerCase();if(tag=='table'){tbl=_divElement.children[i];break;}}
if(typeof(_divElement.showTopNavigator)!='undefined'&&_divElement.showTopNavigator){for(var i=0;i<_divElement.children.length;i++){var tag=_divElement.children[i].tagName.toLowerCase();if(tag=='input'){topNavigatorHeight=_divElement.children[i].offsetHeight;break;}}}}
var baseIndex=_pageIndex*_itemCount;var rDeleted=0,r;for(r=0;r<baseIndex;r++){if(_jsonTable.Rows[r].deleted){baseIndex++;}}
var g;var dpParent=_divElement.parentNode;var parentWidth;var parentHeight;if(dpParent&&dpParent.tagName&&dpParent.tagName.toLowerCase()!='body'){parentWidth=dpParent.offsetWidth;parentHeight=dpParent.offsetHeight;}
if(!parentWidth||parentWidth==0){parentWidth=_divElement.parentwidth?_divElement.parentwidth:(document.body.offsetWidth?document.body.offsetWidth:_divElement.offsetWidth);}
if(!parentHeight||parentHeight==0){parentHeight=_divElement.parentheight?_divElement.parentheight:(document.body.offsetHeight?document.body.offsetHeight:600);}
var colIdx=0;var widthused=0;var heightused=topNavigatorHeight;var lastheight=0;var heightMax=topNavigatorHeight;for(g=0;g<_items.length;g++){r=baseIndex+g+rDeleted;while((r>=0&&r<_jsonTable.Rows.length)&&_jsonTable.Rows[r].deleted){rDeleted++;r=baseIndex+g+rDeleted;}
if(r>=0&&r<_jsonTable.Rows.length){_jsonTable.rowIndex=r;_currentGroupIndex=g;JsonDataBinding.bindDataToElement(_items[g],_jsonTable.TableName,_firstTime);_items[g].style.display='block';_items[g].rowIndex=r;if(autoCR){if(fillDirection==0){lastheight=_items[g].offsetHeight;if(widthused+_items[g].offsetWidth>parentWidth){heightused+=_items[g].offsetHeight;widthused=0;heightMax=heightused;}
_items[g].style.left=widthused+'px';_items[g].style.top=heightused+'px';widthused+=_items[g].offsetWidth;}
else{if(heightused+_items[g].offsetHeight>parentHeight){widthused+=_items[g].offsetWidth;heightused=topNavigatorHeight;}
_items[g].style.left=widthused+'px';_items[g].style.top=heightused+'px';heightused+=_items[g].offsetHeight;if(heightMax<heightused){heightMax=heightused;}}}
if(!_items[g].dpset){_items[g].dpset=true;adjustDPpos(_items[g]);}
_currentGroupIndex=g;if(_divElement.ondisplayItem){_divElement.ondisplayItem();}}
else{_items[g].style.display='none';}}
if(_divElement.adjustItemHeight){setTimeout(_adjustItemHeight,0);}
if(autoCR){heightMax+=lastheight;tbl.style.height=heightMax+'px';}
if(_pageNumerHolders){var nStart=Math.floor(_pageIndex/_navigatorPages)*_navigatorPages;_navigatorStart=nStart;var nEnd=nStart+_navigatorPages;var sh='';for(g=nStart;g<nEnd&&g<pn;g++){if(g==_pageIndex){sh+="<span style='color:black;'>&nbsp;&nbsp;";sh+=(g+1);sh+="&nbsp;&nbsp;</span>";}
else{sh+="<span style='cursor:pointer;' onclick='";sh+=_divElement.id;sh+=".jsData.gotoPage(";sh+=g;sh+=");'>&nbsp;&nbsp;";sh+=(g+1);sh+="&nbsp;&nbsp;</span>";}}
if(g<pn){if(g<pn-1){sh+=" ...";}
sh+=" <span style='cursor:pointer;' onclick='";sh+=_divElement.id;sh+=".jsData.gotoPage(";sh+=(pn-1);sh+=");'>&nbsp;&nbsp;";sh+=(pn);sh+="&nbsp;&nbsp;</span>";}
for(var i=0;i<_pageNumerHolders.length;i++){_pageNumerHolders[i].innerHTML=sh;}}
if(typeof _divElement.onpageIndexChange=='function'){_divElement.onpageIndexChange();}}}
function adjustIds(e,idx){if(e){if(e.id){e.id=e.id+'_'+idx;}
if(e.children){for(var i=0;i<e.children.length;i++){adjustIds(e.children[i],idx);}}}}
function init(){function setDatePicker(v){if(v.getAttribute('useDP')){var pid=v.getAttribute('useDPid');if(typeof pid=='undefined'||pid==null||pid.length==0){JsonDataBinding.CreateDatetimePickerForTextBox(v.id,v.getAttribute('useDPSize'),v.getAttribute('useDPTime'));}
else if(pid=='*'){JsonDataBinding.CreateDatetimePickerForTextBox(v.id,v.getAttribute('useDPSize'),v.getAttribute('useDPTime'),true,null,v.getAttribute('useDPfix'));}
else{JsonDataBinding.CreateDatetimePickerForTextBox(v.id,v.getAttribute('useDPSize'),v.getAttribute('useDPTime'),true,v.parentNode,v.getAttribute('useDPfix'));}}
else{for(var k=0;k<v.children.length;k++){setDatePicker(v.children[k]);}}}
var id,i;if(!_items){id=_divElement.id;_items=new Array();var el=_divElement.getElementsByTagName("div");for(i=0;i<el.length;i++){var nm=el[i].getAttribute('name');if(nm&&nm==id){_items.push(el[i]);}}
_itemCount=_items.length;for(i=0;i<_itemCount;i++){setDatePicker(_items[i]);}}
if(_divElement.ShowAllRecords){if(_jsonTable){if(_itemCount<_jsonTable.Rows.length){id=_divElement.id;var divTemp;var tbl;for(i=0;i<_divElement.children.length;i++){var tag=_divElement.children[i].tagName.toLowerCase();if(tag=='table'){tbl=_divElement.children[i];}
else if(tag=='div'){divTemp=_divElement.children[i];}
if(tbl&&divTemp)break;}
var tblBody=JsonDataBinding.getTableBody(tbl);while(_itemCount<_jsonTable.Rows.length){var tr=tblBody.insertRow(-1);for(i=0;i<divElement.repeatColumnCount;i++){var td=document.createElement('td');tr.appendChild(td);var divNew=divTemp.cloneNode(true);adjustIds(divNew,i+_itemCount);td.appendChild(divNew);_items.push(divNew);if(_elementEvents){for(var k=0;k<_elementEvents.length;k++){if(_elementEvents[k].events){var e0=document.getElementById(_elementEvents[k].id);for(var m=0;m<_elementEvents[k].events.length;m++){activateEvent(e0,_elementEvents[k].id,i+_itemCount,_elementEvents[k].events[m]);}}}}
setDatePicker(divNew);}
_itemCount+=_divElement.repeatColumnCount;}}}}
else{if(!_pageNumerHolders){id=_divElement.id+'_sp';_pageNumerHolders=new Array();var sps=_divElement.getElementsByTagName("span");for(i=0;i<sps.length;i++){var nm=sps[i].getAttribute('name');if(nm==id){_pageNumerHolders.push(sps[i]);}}}}
if(_jsonTable){_pageIndex=0;_firstTime=true;_showPage();_firstTime=false;}}
function _onvaluechanged(tblname,r,c,val){if(_jsonTable&&_jsonTable.TableName==tblname){}}
function _groupsPerPage(){return _itemCount;}
function _onPageIndexChange(name){}
function _onDataReady(jsData){var diff=(_jsonTable!=jsData);_jsonTable=jsData;init();if(diff){_addvaluechangehandler(_jsonTable.TableName,obj);}}
function _gotoPage(pageIndex){var pn=_getTotalPages();_pageIndex=pageIndex;if(_pageIndex>=pn-1){_pageIndex=pn-1;}
if(_pageIndex<0){_pageIndex=0;}
if(_pageIndex<pn){_showPage();}}
function _goNextPage(){var pn=_getTotalPages();if(_pageIndex<pn-1){_pageIndex++;_gotoPage(_pageIndex);}}
function _goPrevPage(){if(_pageIndex>0){_pageIndex--;_gotoPage(_pageIndex);}}
function _gotoFirstPage(){if(_pageIndex!=0){_gotoPage(0);}}
function _gotoLastPage(){var pn=_getTotalPages();if(_pageIndex!=pn-1){_gotoPage(pn-1);}}
function _setTableRowIndex(){if(_jsonTable&&_items&&_currentGroupIndex>=0&&_currentGroupIndex<_items.length){if(_items[_currentGroupIndex]&&_items[_currentGroupIndex].rowIndex>=0&&_items[_currentGroupIndex].rowIndex<_jsonTable.Rows.length){JsonDataBinding.dataMoveToRecord(_jsonTable.TableName,_items[_currentGroupIndex].rowIndex);}}}
function _setCurrentGroupIndex(index){_currentGroupIndex=index;}
function _getElement(id){return document.getElementById(id+'_'+_currentGroupIndex);}
function activateEvent(e0,id,g,evt){var el=document.getElementById(id+'_'+g);el[evt]=(function(x){return function(e){_currentGroupIndex=x;_setTableRowIndex();e0[evt](e);};}(g));}
function _attachElementEvent(id,evt){if(_items){var e0=document.getElementById(id);for(var g=0;g<_items.length;g++){activateEvent(e0,id,g,evt);}}}
function _pushEvent(id,evt){var i;if(!_elementEvents)_elementEvents=new Array();var e;for(i=0;i<_elementEvents.length;i++){if(_elementEvents[i].id==id){e=_elementEvents[i];break;}}
if(!e){e={};e.id=id;_elementEvents.push(e);}
if(!e.events)e.events=new Array();for(i=0;i<e.events.length;i++){if(e.events[i]==evt){return;}}
e.events.push(evt);}
function _getPageIndex(){return _pageIndex;}
function _getCurrentGroupIndex(){return _currentGroupIndex;}
function _setNavigatorPages(pnumber){_navigatorPages=pnumber;}
function _getNavigatorPages(){return _navigatorPages;}
function _refreshDisplay(){_pageIndex=-1;_gotoFirstPage();}
function _refreshCurrentPage(){_gotoPage(_pageIndex);}
init();var obj={refreshPage:function(){_showPage();},oncellvaluechange:function(tblname,r,c,val){_onvaluechanged(tblname,r,c,val);},onDataReady:function(jsData){_onDataReady(jsData);},onPageIndexChange:function(name){_onPageIndexChange(name);},groupsPerPage:function(){return _groupsPerPage();},getPageIndex:function(){return _getPageIndex();},getTotalPages:function(){return _getTotalPages();},gotoNextPage:function(){_goNextPage();},gotoPrevPage:function(){_goPrevPage();},gotoFirstPage:function(){_gotoFirstPage();},gotoLastPage:function(){_gotoLastPage();},gotoPage:function(pageIndex){_gotoPage(pageIndex);},setCurrentGroupIndex:function(index){_setCurrentGroupIndex(index);},getCurrentGroupIndex:function(){return _getCurrentGroupIndex();},setNavigatorPages:function(pnumber){_setNavigatorPages(pnumber-1);},getNavigatorPages:function(){_getNavigatorPages();},getElement:function(id){return _getElement(id);},attachElementEvent:function(id,evt){_pushEvent(id,evt);_attachElementEvent(id,evt);},refreshDisplay:function(){_refreshDisplay();},refreshCurrentPage:function(){_refreshCurrentPage();}}
if(jsTable){_addvaluechangehandler(jsTable.TableName,obj);}
return obj;},datetime:{isValidDate:function(d){if(typeof d!='undefined'&&d!=null){if(d instanceof Date){return!isNaN(d.getTime());}}
return false;},toDate:function(d){if(typeof d!='undefined'&&d!=null){var d0;if(JsonDataBinding.datetime.isValidDate(d)){d0=d;}
else{d0=new Date(d);if(!JsonDataBinding.datetime.isValidDate(d0)){d0=JsonDataBinding.datetime.parseIso(d);if(!JsonDataBinding.datetime.isValidDate(d0)){return null;}}}
return d0;}
return null;},toDate0:function(d){var d2=JsonDataBinding.datetime.toDate(d);if(typeof d2=='undefined'||d2==null){return new Date(0,0,0,0,0,0,0);}
return d2;},setDate:function(d,n){d.setDate(n);return d;},setMonth:function(d,n){d.setMonth(n);return d;},setFullYear:function(d,n){d.setFullYear(n);return d;},setHours:function(d,n){d.setHours(n);return d;},setMinutes:function(d,n){d.setMinutes(n);return d;},setSeconds:function(d,n){d.setSeconds(n);return d;},setMilliseconds:function(d,n){d.setMilliseconds(n);return d;},setTime:function(d,n){d.setTime(n);return d;},setUTCDate:function(d,n){d.setUTCDate(n);return d;},setUTCMonth:function(d,n){d.setUTCMonth(n);return d;},setUTCFullYear:function(d,n){d.setUTCFullYear(n);return d;},setUTCHours:function(d,n){d.setUTCHours(n);return d;},setUTCMinutes:function(d,n){d.setUTCMinutes(n);return d;},setUTCSeconds:function(d,n){d.setUTCSeconds(n);return d;},setUTCMilliseconds:function(d,n){d.setUTCMilliseconds(n);return d;},addMilliseconds:function(d,milliseconds){var d2=JsonDataBinding.datetime.toDate(d);if(d2 instanceof Date){d2.setMilliseconds(d.getMilliseconds()+milliseconds);return d2;}},addSeconds:function(d,seconds){var d2=JsonDataBinding.datetime.toDate(d);if(d2 instanceof Date){d2.setSeconds(d.getSeconds()+seconds);return d2;}},addMinutes:function(d,minutes){var d2=JsonDataBinding.datetime.toDate(d);if(d2 instanceof Date){d2.setMinutes(d.getMinutes()+minutes);return d2;}},addHours:function(d,hours){var d2=JsonDataBinding.datetime.toDate(d);if(d2 instanceof Date){d2.setHours(d.getHours()+hours);return d2;}},addDays:function(d,days){var d2=JsonDataBinding.datetime.toDate(d);if(d2 instanceof Date){d2.setDate(d.getDate()+days);return d2;}},setValue:function(d,yr,mo,dy,hr,mi,se,ml){d.setFullYear(yr);d.setMonth(mo);d.setDate(dy);d.setHours(hr);d.setMinutes(mi);d.setSeconds(se);d.setMilliseconds(ml);return d;},setUTCValue:function(d,yr,mo,dy,hr,mi,se,ml){d.setUTCFullYear(yr);d.setUTCMonth(mo);d.setUTCDate(dy);d.setUTCHours(hr);d.setUTCMinutes(mi);d.setUTCSeconds(se);d.setUTCMilliseconds(ml);return d;},parseIso:function(iso){if(iso){var regexp="([0-9]{4})(-([0-9]{2})(-([0-9]{2})"+"([T]|[ ]([0-9]{2}):([0-9]{2})(:([0-9]{2})(\.([0-9]+))?)?"+"(Z|(([-+])([0-9]{2}):([0-9]{2})))?)?)?)?";var d=iso.match(new RegExp(regexp));if(typeof d!='undefined'&&d!=null){if(d.length>1){var date=new Date(d[1],0,1);if(d[3]){date.setMonth(d[3]-1);}
if(d[5]){date.setDate(d[5]);}
if(d[7]){date.setHours(d[7]);}
if(d[8]){date.setMinutes(d[8]);}
if(d[10]){date.setSeconds(d[10]);}
if(d[12]){date.setMilliseconds(Number("0."+d[12])*1000);}
return date;}}}},parseIsoUTC:function(iso){if(iso){var regexp="([0-9]{4})(-([0-9]{2})(-([0-9]{2})"+"([T]|[ ]([0-9]{2}):([0-9]{2})(:([0-9]{2})(\.([0-9]+))?)?"+"(Z|(([-+])([0-9]{2}):([0-9]{2})))?)?)?)?";var d=iso.match(new RegExp(regexp));if(typeof d!='undefined'&&d!=null){if(d.length>1){var offset=0;var date=new Date(d[1],0,1);if(d[3]){date.setMonth(d[3]-1);}
if(d[5]){date.setDate(d[5]);}
if(d[7]){date.setHours(d[7]);}
if(d[8]){date.setMinutes(d[8]);}
if(d[10]){date.setSeconds(d[10]);}
if(d[12]){date.setMilliseconds(Number("0."+d[12])*1000);}
if(d[14]){offset=(Number(d[16])*60)+Number(d[17]);offset*=((d[15]=='-')?1:-1);}
offset-=date.getTimezoneOffset();return new Date((Number(date)+(offset*60*1000)));}}}},toIso:function(d0){if(typeof d0!='undefined'&&d0!=null){var d=JsonDataBinding.datetime.toDate(d0);if(d instanceof Date){var mo=d.getMonth()+1;var dy=d.getDate();var hh=d.getHours();var mi=d.getMinutes();var s=d.getSeconds();return''.concat(d.getFullYear(),'-',(mo>9?mo:'0'+mo),'-',(dy>9?dy:'0'+dy),' ',(hh>9?hh:'0'+hh),':',(mi>9?mi:'0'+mi),':',(s>9?s:'0'+s));}}},toIsoUTC:function(d0){if(typeof d0!='undefined'&&d0!=null){var d=JsonDataBinding.datetime.toDate(d0);if(d instanceof Date){var mo=d.getUTCMonth()+1;var dy=d.getUTCDate();var hh=d.getUTCHours();var mi=d.getUTCMinutes();var s=d.getUTCSeconds();return''.concat(d.getUTCFullYear(),'-',(mo>9?mo:'0'+mo),'-',(dy>9?dy:'0'+dy),' ',(hh>9?hh:'0'+hh),':',(mi>9?mi:'0'+mi),':',(s>9?s:'0'+s));}}},toLocalDate:function(d0){if(typeof d0!='undefined'&&d0!=null){var d=JsonDataBinding.datetime.toDate(d0);if(d instanceof Date){var d2=new Date();d2.setUTCFullYear(d.getFullYear(),d.getMonth(),d.getDate());d2.setUTCHours(d.getHours(),d.getMinutes(),d.getSeconds(),d.getMilliseconds());return d2;}}
return d0;},isValid:function(d){if(Object.prototype.toString.call(d)!=="[object Date]")
return false;return!isNaN(d.getTime());},getTimespan:function(dstart,dend){var ts=new JsonDataBinding.timespan();ts.setWholeTimeByMilliseconds(dend-dstart);return ts;}},timespan:function(tsdays,tshours,tsminutes,tsseconds,tsmilliseconds){var _wholemilliseconds=0;var _days=0;var _hours=0;var _minutes=0;var _seconds=0;var _milliseconds=0;var day2mis=86400000;function intToDisp(s){s=Math.abs(s);if(s<10)
return'0'+s;return''+s;}
function _isNegative(){return _wholemilliseconds<0;}
function _setSign(isPositive){if(isPositive){if(_wholemilliseconds<0){_wholemilliseconds=-_wholemilliseconds;_onchange();}}
else{if(_wholemilliseconds>0){_wholemilliseconds=-_wholemilliseconds;_onchange();}}}
function _onchange(){if(_wholemilliseconds==0){_milliseconds=0;_seconds=0;_minutes=0;_hours=0;_days=0;}
else if(_wholemilliseconds>0){_milliseconds=Math.floor(_wholemilliseconds%1000);var d=_wholemilliseconds/1000.0;_seconds=Math.floor(d%60);d=d/60.0;_minutes=Math.floor(d%60);d=d/60.0;_hours=Math.floor(d%24);d=d/24.0;_days=Math.floor(d);}
else{_milliseconds=Math.ceil(_wholemilliseconds%1000);var d=_wholemilliseconds/1000.0;_seconds=Math.ceil(d%60);d=d/60.0;_minutes=Math.ceil(d%60);d=d/60.0;_hours=Math.ceil(d%24);d=d/24.0;_days=Math.ceil(d);}}
function _addmilli(pmi){_wholemilliseconds=_wholemilliseconds+pmi;_onchange();}
function _setmilli(pmi){_wholemilliseconds=0;_addmilli(pmi);}
function _addDays(pdays){pdays=parseFloat(pdays);if(typeof pdays=="number"&&!isNaN(pdays)){_addmilli(day2mis*pdays);}}
function _addHours(phours){phours=parseFloat(phours);if(typeof phours=="number"&&!isNaN(phours)){_addmilli(3600000*phours);}}
function _addMinutes(pminutes){pminutes=parseFloat(pminutes);if(typeof pminutes=="number"&&!isNaN(pminutes)){_addmilli(60000*pminutes);}}
function _addSeconds(pseconds){pseconds=parseFloat(pseconds);if(typeof pseconds=="number"&&!isNaN(pseconds)){_addmilli(1000*pseconds);}}
function _addMilliseconds(pmilliseconds){pmilliseconds=parseFloat(pmilliseconds);if(typeof pmilliseconds=="number"&&!isNaN(pmilliseconds)){_addmilli(pmilliseconds);}}
function _addTimeSpan(time){if(typeof time!='undefined'&&typeof time.wholeMillisecondsDecimal=='function'){_wholemilliseconds=_wholemilliseconds+time.wholeMillisecondsDecimal();_onchange();}}
function _setWholeTimeByMilliseconds(pmis){pmis=parseFloat(pmis);if(typeof pmis!='undefined'&&!isNaN(pmis)){_wholemilliseconds=pmis;_onchange();}}
function _differenceInTimeSpan(starttime){var ts=new JsonDataBinding.timespan();if(typeof starttime!='undefined'&&typeof starttime.wholeMillisecondsDecimal=='function'){ts.setWholeTimeByMilliseconds(_wholemilliseconds-starttime.wholeMillisecondsDecimal());}
else
ts.setWholeTimeByMilliseconds(_wholemilliseconds);return ts;}
function _setValues(pdays,phours,pminutes,psecs,pmis){_wholemilliseconds=0;pdays=parseFloat(pdays);phours=parseFloat(phours);pminutes=parseFloat(pminutes);psecs=parseFloat(psecs);pmis=parseFloat(pmis);if(typeof pdays!='undefined'&&!isNaN(pdays))_wholemilliseconds+=(day2mis*pdays);if(typeof phours!='undefined'&&!isNaN(phours))_wholemilliseconds+=(3600000*phours);if(typeof pminutes!='undefined'&&!isNaN(pminutes))_wholemilliseconds+=(60000*pminutes);if(typeof psecs!='undefined'&&!isNaN(psecs))_wholemilliseconds+=(1000*psecs);if(typeof pmis!='undefined'&&!isNaN(pmis))_wholemilliseconds+=pmis;_onchange();}
function _parseIsoString(days,time){if(typeof time=='string'){time=time.trim();if(time.length>0){var hours=0,minutes=0,seconds=0,milliseconds=0;var ss=time.split(':');var h=parseInt(ss[0]);if(!isNaN(h)){hours=h;}
if(ss.length>1){var mi=parseInt(ss[1]);if(!isNaN(mi)){minutes=mi;}
if(ss.length>2){var ses;var p=ss[2].indexOf('.');if(p>=0){ses=ss[2].substr(0,p);seconds=parseInt(ses);milliseconds=parseInt(ss[2].substr(p+1));}
else{seconds=parseInt(ss[2]);}}}
var mis;mis=milliseconds+1000*(seconds+60*(minutes+60*(Math.abs(hours)+24*Math.abs(days))));if(days==0){if(hours<0)
mis=-mis;}
else{if(days<0){mis=-mis;}}
_wholemilliseconds=mis;_onchange();}}}
var ret={isNegative:function(){return _isNegative();},days:function(){return _days;},hours:function(){return _hours;},minutes:function(){return _minutes;},seconds:function(){return _seconds;},milliseconds:function(){return _milliseconds;},wholeMilliseconds:function(){return Math.floor(_wholemilliseconds);},wholeMillisecondsDecimal:function(){return _wholemilliseconds;},wholeSeconds:function(){return Math.floor(_wholemilliseconds/1000);},wholeSecondsDecimal:function(){return _wholemilliseconds/1000.0;},wholeMinutes:function(){return Math.floor((_wholemilliseconds/1000)/60);},wholeMinutesDecimal:function(){return(_wholemilliseconds/1000.0)/60.0;},wholeHours:function(){return Math.floor(((_wholemilliseconds/1000)/60)/60);},wholeHoursDecimal:function(){return((_wholemilliseconds/1000.0)/60.0)/60.0;},wholeDays:function(){return Math.floor((((_wholemilliseconds/1000)/60)/60)/24);},wholeDaysDecimal:function(){return(((_wholemilliseconds/1000.0)/60.0)/60.0)/24.0;},toWholeString:function(){return(_isNegative()?'-':'')+Math.abs(_days)+' '+intToDisp(_hours)+':'+intToDisp(_minutes)+':'+intToDisp(_seconds)+(_milliseconds!=0?'.'+Math.abs(_milliseconds):'');},toTimeString:function(){return(_isNegative()?'-':'')+intToDisp(Math.abs(_hours)+24*Math.abs(_days))+':'+intToDisp(_minutes)+':'+intToDisp(_seconds)+(_milliseconds!=0?'.'+Math.abs(_milliseconds):'');},toShortTimeString:function(){return(_isNegative()?'-':'')+intToDisp(Math.abs(_hours)+24*Math.abs(_days))+':'+intToDisp(_minutes)+':'+intToDisp(_seconds);},addMilliseconds:function(pmilliseconds){_addMilliseconds(pmilliseconds);},addSeconds:function(pseconds){_addSeconds(pseconds);},addMinutes:function(pminutes){_addMinutes(pminutes);},addHours:function(phours){_addHours(phours);},addDays:function(pdays){_addDays(pdays);},addTimeSpan:function(time){_addTimeSpan(time);},parseIsoString:function(time){_parseIsoString(0,time);},parseTimeSpan:function(time){if(typeof time=='string'){var d=0;var p=time.indexOf(' ');if(p>0){d=parseInt(time.substr(0,p));time=time.substr(p+1);}
_parseIsoString(d,time);}},equalExact:function(time){if(typeof time!='undefined'&&typeof time.wholeMillisecondsDecimal=='function'){return _wholemilliseconds==time.wholeMillisecondsDecimal();}
return false;},equal:function(time){if(typeof time!='undefined'&&typeof time.wholeMilliseconds=='function'){return Math.floor(_wholemilliseconds)==time.wholeMilliseconds();}
return false;},differenceInTimeSpan:function(time){return _differenceInTimeSpan(time);},differenceInMilliseconds:function(time){var ts=_differenceInTimeSpan(time);if(ts){return ts.wholeMillisecondsDecimal();}},differenceInSeconds:function(time){var ts=_differenceInTimeSpan(time);if(ts){return ts.wholeSecondsDecimal();}},differenceInMinutes:function(time){var ts=_differenceInTimeSpan(time);if(ts){return ts.wholeMinutesDecimal();}},differenceInHours:function(time){var ts=_differenceInTimeSpan(time);if(ts){return ts.wholeHoursDecimal();}},differenceInDays:function(time){var ts=_differenceInTimeSpan(time);if(ts){return ts.wholeDaysDecimal();}},setTimeSpan:function(time){if(typeof time!='undefined'&&typeof time.wholeMillisecondsDecimal=='function'){_setWholeTimeByMilliseconds(time.wholeMillisecondsDecimal());}},setValues:function(pdays,phours,pminutes,psecs,pmis){_setValues(pdays,phours,pminutes,psecs,pmis);},setDays:function(pdays){pdays=parseFloat(pdays);if(typeof pdays!='undefined'&&!isNaN(pdays)){var c=_days*day2mis;var e=Math.floor(parseFloat(pdays))*day2mis;_wholemilliseconds=_wholemilliseconds+e-c;_onchange();}},setHours:function(phours){phours=parseFloat(phours);if(typeof phours!='undefined'&&!isNaN(phours)){var c=_hours*3600000;var e=Math.floor(parseFloat(phours))*3600000;_wholemilliseconds=_wholemilliseconds+e-c;_onchange();}},setMinutes:function(pminutes){pminutes=parseFloat(pminutes);if(typeof pminutes!='undefined'&&!isNaN(pminutes)){var c=_minutes*60000;var e=Math.floor(parseFloat(pminutes))*60000;_wholemilliseconds=_wholemilliseconds+e-c;_onchange();}},setSeconds:function(psecs){psecs=parseFloat(psecs);if(typeof psecs!='undefined'&&!isNaN(psecs)){var c=_seconds*1000;var e=Math.floor(parseFloat(psecs))*1000;_wholemilliseconds=_wholemilliseconds+e-c;_onchange();}},setMilliseconds:function(pmis){pmis=parseFloat(pmis);if(typeof pmis!='undefined'&&!isNaN(pmis)){var c=_milliseconds;var e=Math.floor(parseFloat(pmis));_wholemilliseconds=_wholemilliseconds+e-c;_onchange();}},setWholeTimeByMilliseconds:function(pmis){_setWholeTimeByMilliseconds(pmis);},setWholeTimeBySeconds:function(psecs){psecs=parseFloat(psecs);if(typeof psecs!='undefined'&&!isNaN(psecs)){_setWholeTimeByMilliseconds(psecs*1000);}},setWholeTimeByMinutes:function(pmins){pmins=parseFloat(pmins);if(typeof pmins!='undefined'&&!isNaN(pmins)){_setWholeTimeByMilliseconds(pmins*60000);}},setWholeTimeByHours:function(phours){phours=parseFloat(phours);if(typeof phours!='undefined'&&!isNaN(phours)){_setWholeTimeByMilliseconds(phours*3600000);}},setWholeTimeByDays:function(pdays){pdays=parseFloat(pdays);if(typeof pdays!='undefined'&&!isNaN(pdays)){_setWholeTimeByMilliseconds(pdays*day2mis);}},setWholeTimeByDates:function(start,end){var mis=end-start;if(typeof mis!='undefined'&&!isNaN(mis)){_setWholeTimeByMilliseconds(mis);}}};if(typeof tsdays!="undefined"){if(typeof tshours=='undefined'){if(typeof tsdays=="number"){_setWholeTimeByMilliseconds(tsdays*day2mis);}
else if(typeof tsdays.wholeMillisecondsDecimal=='function'){_setWholeTimeByMilliseconds(tsdays.wholeMillisecondsDecimal());}}
else{_setValues(tsdays,tshours,tsminutes,tsseconds,tsmilliseconds);}}
return ret;},CreateComboBox:function(parentElement,values,initValue,initWidth,fillParent,textSearch){var _img,_input;var _div=document.createElement('div');parentElement.appendChild(_div);_div.style.position='relative';_div.style.display='inline';var tbl=document.createElement('table');tbl.cellPadding=0;tbl.cellSpacing=0;tbl.border=0;tbl.style.margin=0;var tblBody=JsonDataBinding.getTableBody(tbl);var tr=tblBody.insertRow(-1);var td=document.createElement('td');tr.appendChild(td);_input=document.createElement('input');_input.type='text';_input.setAttribute('autocomplete','off');_input.value=initValue;_input.style.width='100%';td.appendChild(_input);td.style.width='100%';var td0=document.createElement('td');if(JsonDataBinding.IsFireFox()||JsonDataBinding.IsOpera()){td0.innerHTML='&nbsp;&nbsp;';}
else{td0.innerHTML='&nbsp;';}
tr.appendChild(td0);var td2=document.createElement('td');tr.appendChild(td2);_img=document.createElement('img');_img.src='libjs/downArrow_t16.png';_img.style.cursor='pointer';td2.appendChild(_img);_div.appendChild(tbl);var _select=document.createElement('select');_select.style.display='none';_select.style.position='absolute';_select.size=values.length;if(_select.size>30){_select.style.height='180px';_select.style.overflowY='scroll';}
else{_select.style.height='auto';_select.style.overflowY='';}
document.body.appendChild(_select);for(var i=0;i<values.length;i++){var o=document.createElement('option');o.text=values[i].text;o.value=values[i].value;try{_select.add(o,null);}
catch(ex){_select.add(o);}
if(initValue==values[i].value){_select.selectedIndex=_select.options.length-1;}}
function showDropDown(){var zi=JsonDataBinding.getPageZIndex(_div);_div.style.zIndex=zi+1;var pos=JsonDataBinding.ElementPosition.getElementPosition(_input);_select.style.zIndex=zi+1;_select.style.top=(pos.y+20)+'px';_select.style.left=pos.x+'px';_select.style.display='block';_select.focus();}
function ontxtchange(){if(_div.jsData){if(_div.jsData.onchange){_div.jsData.onchange({target:_div.jsData});}
else if(_div.jsData.change){_div.jsData.change({target:_div.jsData});}}
return true;}
function comboselect_onchange(){if(_select.selectedIndex!=-1){if(_div.jsData.setter){_div.jsData.setter(_select.options[_select.selectedIndex].value,_input);}
else{_input.value=_select.options[_select.selectedIndex].value;}
ontxtchange();}
return true;}
function txtkeydown(e){e=window.event||e;var keyCode=e?e.keyCode:0;if(keyCode==40||keyCode==38){showDropDown();comboselect_onchange();}
else if(keyCode==13){e.cancelBubble=true;if(e.returnValue)e.returnValue=false;if(e.stopPropagation)e.stopPropagation();comboselect_onchange();_select.style.display='none';_input.focus();return false;}
else if(keyCode==9)return true;else{var itemfound=false;if(textSearch&&keyCode){_select.style.display='block';showDropDown();var c=String.fromCharCode(keyCode);c=c.toUpperCase();toFind=_input.value.toUpperCase()+c;for(i=0;i<_select.options.length;i++){nextOptionText=_select.options[i].text.toUpperCase();if(toFind==nextOptionText){_select.selectedIndex=i;itemfound=true;break;}
if(i<_select.options.length-1){lookAheadOptionText=_select.options[i+1].text.toUpperCase();if((toFind>nextOptionText)&&(toFind<lookAheadOptionText)){_select.selectedIndex=i+1;itemfound=true;break;}}
else{if(toFind>nextOptionText){_select.selectedIndex=_select.options.length-1;itemfound=true;break;}}}}
if(!itemfound){if(_input.onhandleKeydown){_input.onhandleKeydown();}}}
return true;}
function comboselect_imageClick(){showDropDown();return true;}
function comboselect_blur(){_select.style.display='none';}
function comboselect_onkeyup(e){e=window.event||e;if(e){var keyCode=e.keyCode;if(keyCode){if(keyCode==13){comboselect_onchange();_select.style.display='none';_input.focus();}}}
return true;}
function document_click(e){var sender=JsonDataBinding.getSender(e);if(sender!=_img){_select.style.display='none';}
return true;}
function _setWidth(width){var w=(_img.offsetWidth?_img.offsetWidth:20)+20;if(width>w){_input.style.width=(width-w-1)+'px';}}
function _adjustPosition(){}
function _init(){}
JsonDataBinding.AttachEvent(_input,'onkeydown',txtkeydown);JsonDataBinding.AttachEvent(_input,'onchange',ontxtchange);JsonDataBinding.AttachEvent(_img,'onclick',comboselect_imageClick);JsonDataBinding.AttachEvent(_select,'onblur',comboselect_blur);JsonDataBinding.AttachEvent(_select,'onchange',comboselect_onchange);JsonDataBinding.AttachEvent(_select,'onkeyup',comboselect_onkeyup);JsonDataBinding.AttachEvent(document,'onclick',document_click);if(initWidth){_setWidth(initWidth);}
_div.jsData={div:_div,select:_select,input:_input,setWidth:function(width){_setWidth(width);},getParentNode:function(){return _div.parentNode;},getValue:function(){return _input.value;},init:function(){_init();},adjustPosition:function(){_adjustPosition();},applyTextChanges:function(){ontxtchange();},getInput:function(){return _input;}}
return _div.jsData;},initSendKeys:function(){_initSendKeys();},sendKeys:function(keys){_sendKeys(keys);},selectNextInput:function(){_selectNextInput();},MousehButton:function(event){if('which'in event){switch(event.which){case 1:return 1;case 2:return 4;case 3:return 2;}}
else{if('button'in event){var buttons=0;if(event.button&1){buttons+=1;}
if(event.button&2){buttons+=2;}
if(event.button&4){buttons+=4;}
return buttons;}}
return 0;},setOpacity:function(obj,opacityValue){if(opacityValue<0)opacityValue=0;if(opacityValue>100)opacityValue=100;obj.style.opacity=(opacityValue/100);obj.style.MozOpacity=(opacityValue/100);obj.style.KhtmlOpacity=(opacityValue/100);obj.style.filter='alpha(opacity='+opacityValue+')';},getOpacity:function(obj){if(obj.style){if(typeof(obj.style.opacity)!='undefined'&&(obj.style.opacity||obj.style.opacity===0)){return obj.style.opacity*100;}
if(typeof(obj.style.MozOpacity)!='undefined'&&(obj.style.MozOpacity||obj.style.MozOpacity===0)){return obj.style.MozOpacity*100;}
if(typeof(obj.style.KhtmlOpacity)!='undefined'&&(obj.style.KhtmlOpacity||obj.style.KhtmlOpacity===0)){return obj.style.KhtmlOpacity*100;}
if(typeof(obj.style.filter)!='undefined'&&obj.style.filter!=null){var pos=obj.style.filter.indexOf('=');if(pos>0){var s=obj.style.filter.substr(pos+1);if(s&&s.length>0){if(s[s.length-1]==')'){s=s.substr(0,s.length-1);}
return parseInt(s);}}}}
return 100;},setBoxShadow:function(obj,shadow){obj.style.boxShadow=shadow;obj.style.mozBoxShadow=shadow;obj.style.webkitBoxShadow=shadow;},getBoxShadow:function(obj){return obj.style.boxShadow?obj.style.boxShadow:(obj.style.webkitBoxShadow?obj.style.webkitBoxShadow:obj.style.mozBoxShadow);},setButtonImage:function(bt,imgUrl,imgWidth,imgHeight){bt.style.backgroundImage="url('"+imgUrl+"')";bt.style.backgroundRepeat='no-repeat';var h=(bt.offsetHeight-imgHeight)/2;if(h<0)h=0;bt.style.backgroundPosition='2px '+h+'px';bt.style.paddingLeft=(imgWidth+2)+'px';bt.style.verticalAlign='middle';},mouseButtonLeft:function(){if(IsIE()){return 1;}
return 0;},mouseButtonRight:function(){return 2;},getDirectChildElementsByTagName:function(e,tag){if(tag){var a=e.childNodes;var aRet=new Array();if(a&&a.length>0){for(var i=0;i<a.length;i++){if(a[i].parentNode==e){aRet.push(a[i]);}}}
return aRet;}
else{return e.childNodes;}},setVisible:function(e,v){var b=false;if(v==='none'){b=false;}
else if(v==='block'||v===''){b=true;}
else{if(JsonDataBinding.isValueTrue(v)){v='block';b=true;}
else{v='none';b=false;}}
if(e.getAttribute('usedpid')){var dpdiv=document.getElementById('fd-'+e.id);if(dpdiv){dpdiv.style.display=v;}}
else if(e.getAttribute('useDP')){e.style.display=v;var dpbut=document.getElementById('fd-but-'+e.id);if(dpbut){dpbut.style.display=v;JsonDataBinding.adjustDatePickerButtonPos(e.id);}}
else{e.style.display=v;if(b){function adjustDPpos(c){if(c.tagName.toLowerCase()=='input'){JsonDataBinding.adjustDatePickerButtonPos(c.id);}
else{for(var i=0;i<c.children.length;i++){adjustDPpos(c.children[i]);}}}
adjustDPpos(e);}}},colorObj:function(c){var _red=0;var _green=0;var _blue=0;function setHex(h){if(h.charAt(0)=="#")h=h.substring(1);_red=parseInt(h.substring(0,2),16);_green=parseInt(h.substring(2,4),16);_blue=parseInt(h.substring(4,6),16);};function setRGB(rgb){rgb=rgb.substring(4,rgb.length-1);var colors=rgb.split(',');_red=parseInt(colors[0]);_green=parseInt(colors[1]);_blue=parseInt(colors[2]);}
if(c.indexOf("rgb(")==0)
setRGB(c);else
setHex(c);return{r:_red,g:_green,b:_blue};},compareColor:function(c1,c2){var a=JsonDataBinding.colorObj(c1);var b=JsonDataBinding.colorObj(c2);return(a.r==b.r&&a.g==b.g&&a.b==b.b);},hasClass:function(e,c){if(e&&e.className&&c){var cs=e.className.split(' ');var c0=c.toLowerCase();for(var i=0;i<cs.length;i++){if(cs[i]&&cs[i].toLowerCase()==c0){return true;}}}},initMenubar:function(nav){for(var m=0;m<nav.children.length;m++){var ul=nav.children[m];if(ul&&ul.tagName&&ul.tagName.toLowerCase()=='ul'){for(var n=0;n<ul.children.length;n++){var li=ul.children[n];if(li&&li.tagName&&li.tagName.toLowerCase()=='li'){for(var t=0;t<li.children.length;t++){var ul2=li.children[t];if(ul2&&ul2.tagName&&ul2.tagName.toLowerCase()=='ul'){var anchs=ul2.getElementsByTagName('a');if(anchs){for(var i=0;i<anchs.length;i++){var pli=anchs[i].parentNode;var isParent=false;if(pli&&pli.tagName&&pli.tagName.toLowerCase()=='li'){for(var k=0;k<pli.children.length;k++){if(pli.children[k].tagName.toLowerCase()=='ul'){isParent=true;break;}}}
if(isParent){anchs[i].setAttribute('isparent','1');}
else{anchs[i].removeAttribute('isparent');}}}}}}}}}},limnorTreeViewStylesName:function(){return'limnortv';},createTreeView:function(tvul){if(typeof tvul.jsData!='undefined')
return tvul.jsData;tvul.typename='treeview';function _getChildUL(li){if(li.children){for(var i=0;i<li.children.length;i++){if(li.children[i]&&li.children[i].tagName&&li.children[i].tagName.toLowerCase()=='ul'){return li.children[i];}}}}
function _setDefaultStateLI(li){var hasSub=false;var isLast=true;var isRoot=false;for(var i=0;i<li.children.length;i++){if(li.children[i].tagName.toLowerCase()=='ul'){for(var k=0;k<li.children[i].children.length;k++){if(li.children[i].children[k].tagName.toLowerCase()=='li'){hasSub=true;break;}}
break;}}
if(li.nextElementSibling){isLast=false;}
if(JsonDataBinding.hasClass(li.parentNode,'limnortv')){isRoot=true;}
var st0=li.getAttribute('expState');var st=9;if(isRoot){if(isLast){var isSingle=true;var ulParent=li.parentNode;for(var i=0;i<ulParent.children.length;i++){if(ulParent.children[i]!=li){if(ulParent.children[i].tagName.toLowerCase()=='li'){isSingle=false;break;}}}
if(hasSub){if(isSingle){if(st0!=3){st=2;}
else
st=st0;}
else{if(st0!=8){st=7;}
else
st=st0;}}
else{if(isSingle)
st=0;else
st=6;}}
else{var isFirst=true;var ulParent=li.parentNode;for(var i=0;i<ulParent.children.length;i++){if(ulParent.children[i].tagName.toLowerCase()=='li'){if(ulParent.children[i]!=li){isFirst=false;}
break;}}
if(hasSub){if(isFirst){if(st0!=5)
st=4;else
st=st0;}
else{if(st0!=11)
st=10;else
st=st0;}}
else{if(isFirst)
st=1;else
st=101;}}}
else{if(isLast){if(hasSub){if(st0!=8)
st=7;else
st=st0;}
else{st=6;}}
else{if(hasSub){if(st0!=11)
st=10;else
st=st0;}
else{}}}
if(st==9){if(typeof st0!='undefined'){li.removeAttribute('expState');}}
else if(st!=st0){li.setAttribute('expState',st);}
var cu=_getChildUL(li);if(cu){_setDefaultStateUL(cu);}}
function _setDefaultStateUL(u){if(u.children&&u.children.length>0){for(var i=0;i<u.children.length;i++){if(u.children[i]&&u.children[i].tagName&&u.children[i].tagName.toLowerCase()=='li'){_setDefaultStateLI(u.children[i]);}}}}
function _hasChildren(u){if(u.children&&u.children.length>0){for(var i=0;i<u.children.length;i++){if(u.children[i]&&u.children[i].tagName&&u.children[i].tagName.toLowerCase()=='li'){return true;}}}
return false;}
function _addItem(ownerLI){var cu=ownerLI?_getChildUL(ownerLI):tvul;if(!cu){cu=document.createElement('ul');ownerLI.appendChild(cu);}
var li=document.createElement('li');li.innerHTML='item';cu.appendChild(li);if(ownerLI)
_setDefaultStateLI(ownerLI);else
_setDefaultStateUL(cu);return li;}
function _delItem(ownerLI){var ul=ownerLI.parentNode;if(ul){ul.removeChild(ownerLI);if(ul.className=='limnortv'){_setDefaultStateUL(ul);}
else{if(_hasChildren(ul)){_setDefaultStateUL(ul);}
else{var li=ul.parentNode;if(li&&li.tagName&&li.tagName.toLowerCase()=='li'){li.removeChild(ul);ul=li.parentNode;if(ul&&ul.tagName&&ul.tagName.toLowerCase()=='ul'){_setDefaultStateUL(ul);}}}}}}
function _delChildren(owner){if(owner&&owner.tagName){var tag=owner.tagName.toLowerCase();var ul;var li;if(tag=='li')
li=owner;else if(tag=='ul'){if(owner.className=='limnortv'){owner.innerHTML='';return;}
if(owner.parentNode&&owner.parentNode.tagName&&owner.parentNode.tagName.toLowerCase()=='li'){li=owner.parentNode;}}
if(li){ul=_getChildUL(li);if(ul){li.removeChild(ul);}
ul=li.parentNode;if(ul&&ul.tagName&&ul.tagName.toLowerCase()=='ul'){_setDefaultStateUL(ul);}}}}
function _getEventLI(e){e=e||window.event;if(e){var isLi=false;var sender=JsonDataBinding.getSender(e);while(sender){if(sender.tagName&&sender.tagName.toLowerCase()=='li'){isLi=true;break;}
else{sender=sender.parentNode;}}
if(isLi){return sender;}}}
function _treeviewOnMouseup(e){var sender=_getEventLI(e);e=e||window.event;if(e&&sender){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);var pos=JsonDataBinding.ElementPosition.getElementPosition(sender);if(JsonDataBinding.IsIE()){var x0=window.pageXOffset?window.pageXOffset:(window.scrollX?window.scrollX:document.body.scrollLeft);var y0=window.pageYOffset?window.pageYOffset:(window.scrollY?window.scrollY:document.body.scrollTop);x+=x0;y+=y0;}
var c=(x>pos.x-20&&x<pos.x+16)&&(y>pos.y&&y<pos.y+22);if(c){var state=sender.getAttribute('expState');var st=state;if(state==2)
st=3;else if(state==3)
st=2;else if(state==4)
st=5;else if(state==5)
st=4;else if(state==7)
st=8;else if(state==8)
st=7;else if(state==11)
st=10;else if(state==10)
st=11;if(st!=state){sender.setAttribute('expState',st);if(JsonDataBinding.IsIE()){sender.className=sender.className;}}}
return false;}}
var _mouseoverOwner;var _expOwner;var _disableHover;function _onmouseover(e){if(!_disableHover){var sender=_getEventLI(e);if(sender){if(_mouseoverOwner){_mouseoverOwner.removeAttribute('hoverstate');}
_mouseoverOwner=sender;_mouseoverOwner.setAttribute('hoverstate','1');}}}
function _onmouseout(e){var sender=_getEventLI(e);if(sender){sender.removeAttribute('hoverstate');}}
function _onmousemove(e){e=e||window.event;if(e){var sender=_getEventLI(e);if(sender){var ext=sender.getAttribute('expState');if(ext&&ext!='9'&&ext!='6'){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);var pos=JsonDataBinding.ElementPosition.getElementPosition(sender);if(JsonDataBinding.IsIE()){var x0=window.pageXOffset?window.pageXOffset:(window.scrollX?window.scrollX:document.body.scrollLeft);var y0=window.pageYOffset?window.pageYOffset:(window.scrollY?window.scrollY:document.body.scrollTop);x+=x0;y+=y0;}
var c=(x>pos.x-20&&x<pos.x+16)&&(y>pos.y&&y<pos.y+22);if(c){if(_expOwner!=sender){_expOwner=sender;_expOwner.setAttribute('cursorstate','1');}}
else{if(_expOwner){_expOwner.removeAttribute('cursorstate');_expOwner=null;}}}}
else{if(_expOwner){_expOwner.removeAttribute('cursorstate');_expOwner=null;}}}}
_setDefaultStateUL(tvul);JsonDataBinding.AttachEvent(tvul,'onmouseup',_treeviewOnMouseup);JsonDataBinding.AttachEvent(tvul,'onmouseover',_onmouseover);JsonDataBinding.AttachEvent(tvul,'onmouseout',_onmouseout);JsonDataBinding.AttachEvent(tvul,'onmousemove',_onmousemove);tvul.jsData={typename:'treeview',resetState:function(){_setDefaultStateUL(tvul);},enableHoverState:function(enable){_disableHover=!enable;if(_disableHover){var lis=tvul.getElementsByTagName('li');if(lis){for(var i=0;i<lis.length;i++){lis[i].removeAttribute('hoverstate');}}}},editable:function(e){if(e){if(e!=tvul){if(e.tagName&&e.tagName.toLowerCase()=='ul'){return false;}}
return true;}
return false;},addItem:function(ownerLI){return _addItem(ownerLI);},delItem:function(ownerLI){_delItem(ownerLI);},delChildren:function(owner){_delChildren(owner);}};return tvul.jsData;},createMultiSelection:function(multiSelBox){var _selected=[];function _onCancel(){multiSelBox.style.display='none';if(multiSelBox.onClickCancel){multiSelBox.onClickCancel();}}
function _onOK(e){multiSelBox.style.display='none';if(multiSelBox.onClickOK){multiSelBox.onClickOK();}
if(multiSelBox.onSelectedItem){var sel=multiSelBox.getElementsByTagName('select')[0];for(var i=0;i<sel.options.length;i++){if(sel.options[i].selected){multiSelBox.onSelectedItem(e,i,sel.options[i].text,sel.options[i].value);}}}}
function _onchange(){var sels=multiSelBox.getElementsByTagName('select')[0];var lst=sels.getElementsByTagName('option');if(lst&&lst.length>0){for(var i=0;i<lst.length;i++){if(lst[i].selected){_selected[i]=!_selected[i];}
lst[i].selected=_selected[i];}}}
function _init(firstTime){var sels=multiSelBox.getElementsByTagName('select')[0];var lst=sels.getElementsByTagName('option');_selected=[];if(lst&&lst.length>0){for(var i=0;i<lst.length;i++){_selected.push(lst[i].selected);}}
if(firstTime){JsonDataBinding.AttachEvent(sels,'onchange',_onchange);}}
function _selectAll(select){var sels=multiSelBox.getElementsByTagName('select')[0];var lst=sels.getElementsByTagName('option');_selected=[];if(lst&&lst.length>0){for(var i=0;i<lst.length;i++){lst[i].selected=select;_selected.push(lst[i].selected);}}}
_init(true);multiSelBox.jsData={onCancel:function(){_onCancel();},onOK:function(){_onOK();},onChildListBoxFilled:function(){_init(false);},selectAll:function(){_selectAll(true);},selectNone:function(){_selectAll(false);}};return multiSelBox.jsData;},initializePage:function(forEditor){if(!JsonDataBinding.initializing){JsonDataBinding.initializing=true;if(forEditor){JsonDataBinding.inediting=true;}
var i,objs,tag;objs=document.getElementsByTagName('nav');if(objs){tag='limnorstyles_menu';for(i=0;i<objs.length;i++){if(objs[i].className){if(JsonDataBinding.startsWith(objs[i].className,tag)){JsonDataBinding.initMenubar(objs[i]);}}}}
objs=document.getElementsByTagName('ul');if(objs){tag=JsonDataBinding.limnorTreeViewStylesName();for(i=0;i<objs.length;i++){if(objs[i].className){if(JsonDataBinding.startsWith(objs[i].className,tag)){JsonDataBinding.createTreeView(objs[i]);}}}}
objs=document.getElementsByTagName('iframe');if(objs){for(i=0;i<objs.length;i++){tag=objs[i].getAttribute('typename');if(tag=='youtube'){objs[i].typename=tag;}}}
JsonDataBinding.stylesInitialized=true;JsonDataBinding.ProcessPageParameters.apply(window);}}}
JsonDataBinding.DialogueBox={showDialog:function(dlg){var _divDialog;var _frmDialog;var _docDialog;var _frmWait;var _docWait;var _tdTitle;var _imgIcon;var _resizer;var _api;var _imgOK;var _imgCancel;var _onBringToFront;var _isIE=JsonDataBinding.IsIE();function _showResizer(visible){if(_resizer){if(visible){var pos=JsonDataBinding.ElementPosition.getElementPosition(_divDialog);var zi=JsonDataBinding.getZOrder(_divDialog)+1;var ieOffset=_isIE?(dlg.ieBorderOffset?dlg.ieBorderOffset:0):0;_resizer.style.zIndex=zi;_resizer.style.display='block';_resizer.style.left=(pos.x+_divDialog.offsetWidth-_resizer.offsetWidth-ieOffset)+'px';_resizer.style.top=(pos.y+_divDialog.offsetHeight-_resizer.offsetHeight-ieOffset)+'px';}
else{_resizer.style.display='none';}}}
function _bringToFront(){var z=JsonDataBinding.getPageZIndex(_divDialog)+1;var z2=z+1;_divDialog.style.display='block';_divDialog.style.zIndex=z;_resizer.style.zIndex=z2;_showResizer(dlg.resizable);if(_onBringToFront){_onBringToFront();}}
function _hide(ret,close){if(close){try{_docDialog=_frmDialog.contentDocument?_frmDialog.contentDocument:_frmDialog.contentWindow.document;if(ret){if(_docDialog.childManager&&_docDialog.childManager.CloseDialogPrompt&&_docDialog.childManager.CloseDialogPrompt.length>0){if(confirm(_docDialog.childManager.CloseDialogPrompt)){_docDialog.dialogResult='ok';}
else{return;}}
document.dialogResult='ok';}
else{if(_docDialog.childManager&&_docDialog.childManager.CancelDialogPrompt&&_docDialog.childManager.CancelDialogPrompt.length>0){if(confirm(_docDialog.childManager.CancelDialogPrompt)){_docDialog.dialogResult='cancel';}
else{return;}}
document.dialogResult='cancel';}
_docDialog.dialogResult=document.dialogResult;if(_docDialog.childManager&&_docDialog.childManager.onClosingWindow){_docDialog.childManager.onClosingWindow();if(_docDialog.dialogResult==''){return;}}}
catch(er){}
dlg.finished=true;_api.finished=true;if(dlg.isDialog&&dlg.onDialogClose){dlg.onDialogClose();}
_frmDialog.src="about:blank";_divDialog.style.display='none';_frmWait.style.display='none';_resizer.style.display='none';if(JsonDataBinding.IsIE()){_frmDialog.parentNode.innerHTML='';document.body.removeChild(_divDialog);document.body.removeChild(_frmWait);document.body.removeChild(_resizer);}
else{if(_frmWait&&_frmWait.parentNode){_frmWait.parentNode.removeChild(_frmWait);}
if(_resizer&&_resizer.parentNode){_resizer.parentNode.removeChild(_resizer);}
if(_divDialog&&_divDialog.parentNode){_divDialog.parentNode.removeChild(_divDialog);}}
document.childManager.remove(_api);}
else{_divDialog.style.display='none';_frmWait.style.display='none';_resizer.style.display='none';}}
function onOK(e){_hide(true,true);}
function onCancel(e){_hide(false,true);}
function docmousedown(e){e=e||window.event;var x,y;var c=JsonDataBinding.getSender(e);if(c==_resizer){x=_resizer.offsetLeft;y=_resizer.offsetTop;_resizer.ox=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-x;_resizer.oy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-y;_resizer.isMousedown=true;_divDialog.isMousedown=false;_bringToFront();return false;}
else if(c==_tdTitle){x=_divDialog.offsetLeft;y=_divDialog.offsetTop;_divDialog.ox=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-x;_divDialog.oy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-y;_divDialog.isMousedown=true;_resizer.isMousedown=false;_bringToFront();if(_isIE){if(_resizer){_resizer.style.left='-30px';}}
else{_showResizer(false);}
return false;}
else{_divDialog.isMousedown=false;_resizer.isMousedown=false;}}
function docmouseup(e){_divDialog.isMousedown=false;_resizer.isMousedown=false;_showResizer(dlg.resizable);return true;}
function dlgdocMouseDown(e){_bringToFront();}
function dlgdocmousemove(e){e=e||_frmDialog.contentWindow.event;var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);if(_resizer.isMousedown||_divDialog.isMousedown){var dx=_divDialog.offsetLeft;var dy=_divDialog.offsetTop;docmousemove.apply(window,[{pageX:x+dx,pageY:y+dy,target:_resizer}]);}}
function waitdocmousemove(e){e=e||_frmWait.contentWindow.event;var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);if(_resizer.isMousedown||_divDialog.isMousedown){docmousemove({pageX:x,pageY:y,target:_resizer});}}
function docmousemove(e){var diffx,diffy;var obj,o;var isInDlg;var dx=0;var dy=0;if(_resizer.isMousedown){e=e||window.event;if(e){diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_resizer.ox+dx;diffy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-_resizer.oy+dy;_resizer.style.left=diffx>0?diffx+'px':'0px';_resizer.style.top=diffy>0?diffy+'px':'0px';var p1=JsonDataBinding.ElementPosition.getElementPosition(_divDialog);var p2=JsonDataBinding.ElementPosition.getElementPosition(_resizer);var w=p2.x-p1.x+_resizer.offsetWidth;var h=p2.y-p1.y+_resizer.offsetHeight;if(w>120){_divDialog.style.width=w+'px';}
if(h>60){_divDialog.style.height=h+'px';}}}
else if(_divDialog.isMousedown){e=e||window.event;if(!e){_docWait=_frmWait.contentDocument?_frmWait.contentDocument:_frmWait.contentWindow.document;e=_frmWait.contentWindow.event;if(!e){_docDialog=_frmDialog.contentDocument?_frmDialog.contentDocument:_frmDialog.contentWindow.document;e=_frmDialog.contentWindow.event;}}
if(e){diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_divDialog.ox+dx;diffy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-_divDialog.oy+dy;_divDialog.style.left=diffx>0?diffx+'px':'0px';_divDialog.style.top=diffy>0?diffy+'px':'0px';}}}
function attachWaitFrmEvent(){var ntryCount=0;var started=false;function _tryAttach(){if(_frmWait){_docWait=_frmWait.contentDocument?_frmWait.contentDocument:(_frmWait.contentWindow?_frmWait.contentWindow.document:null);if(_docWait){if(_docWait.readyState=='interactive'){ntryCount++;if(ntryCount>2){started=true;}}
if(started||_docWait.readyState=='complete'){JsonDataBinding.AttachEvent(_docWait,"onmouseup",docmouseup);JsonDataBinding.AttachEvent(_docWait,"onmousemove",waitdocmousemove);_docWait.body.style.background='#000';}
else{setTimeout(_tryAttach,300);}}}}
_tryAttach();}
function _onDocMouseDown(e){_bringToFront();}
function _onDocMouseMove(e){var dx,dy,diffx,diffy;if(_resizer.isMousedown){e=e||window.event;if(e){dx=_divDialog.offsetLeft-_resizer.offsetWidth;dy=_divDialog.offsetTop-_resizer.offsetHeight;diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_resizer.ox+dx;diffy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-_resizer.oy+dy;_resizer.style.left=diffx>0?diffx+'px':'0px';_resizer.style.top=diffy>0?diffy+'px':'0px';var p1=JsonDataBinding.ElementPosition.getElementPosition(_divDialog);var p2=JsonDataBinding.ElementPosition.getElementPosition(_resizer);var w=p2.x-p1.x;var h=p2.y-p1.y;if(w>120){_divDialog.style.width=w+'px';}
if(h>60){_divDialog.style.height=h+'px';}}}
else if(_divDialog.isMousedown){e=e||window.event;if(e){dx=_divDialog.offsetLeft;dy=_divDialog.offsetTop;diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_divDialog.ox+dx;diffy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-_divDialog.oy+dy;_divDialog.style.left=diffx>0?diffx+'px':'0px';_divDialog.style.top=diffy>0?diffy+'px':'0px';}}}
function _onDocMouseUp(e){_divDialog.isMousedown=false;_resizer.isMousedown=false;_showResizer(dlg.resizable);return true;}
function attachDlgDocEvent(){var ntryCount=0;var started=false;function _tryAttach(){try{if(_frmDialog){_docDialog=_frmDialog.contentDocument?_frmDialog.contentDocument:(_frmDialog.contentWindow?_frmDialog.contentWindow.document:null);if(_docDialog){if(_docDialog.readyState=='interactive'){ntryCount++;if(ntryCount>2){started=true;}}
if(started||(_docDialog.readyState=='complete'&&(dlg.url=='about:blank'||dlg.url==null||_docDialog.URL!='about:blank'))){JsonDataBinding.AttachEvent.apply(_frmDialog.contentWindow,[_docDialog,"onmousedown",dlgdocMouseDown]);JsonDataBinding.AttachEvent.apply(_frmDialog.contentWindow,[_docDialog,"onmouseup",docmouseup]);JsonDataBinding.AttachEvent.apply(_frmDialog.contentWindow,[_docDialog,"onmousemove",dlgdocmousemove]);if(dlg.isDialog){document.currentDialog=_api;}
_docDialog.childApi=_api;}
else{setTimeout(_tryAttach,300);}}}}
catch(er){}}
_tryAttach();}
if(!_divDialog){_divDialog=document.createElement('div');if(dlg.contentsHolder){_divDialog.style.position="static";_divDialog.style.left='0px';_divDialog.style.top='0px';_divDialog.style.width='100%';_divDialog.style.height='100%';_divDialog.style.overflowY='scroll';dlg.contentsHolder.appendChild(_divDialog);}
else{document.body.appendChild(_divDialog);_divDialog.style.position="absolute";_divDialog.style.opacity=".99";_divDialog.style.filter="alpha(opacity=99)";_divDialog.style.overflowY='hidden';}
_divDialog.style.backgroundColor="#000";_divDialog.style.overflow='hidden';var tbl=document.createElement('table');tbl.style.backgroundColor="white";tbl.border=0;tbl.style.padding=0;tbl.cellPadding=0;tbl.cellSpacing=0;tbl.style.width='100%';tbl.style.height='100%';_divDialog.appendChild(tbl);var tbody=JsonDataBinding.getTableBody(tbl);var tr=document.createElement('tr');tbody.appendChild(tr);var td=document.createElement('td');tr.appendChild(td);_imgIcon=document.createElement("img");_imgIcon.border=0;_imgIcon.src='libjs/dlg.png';td.appendChild(_imgIcon);td=document.createElement('td');tr.appendChild(td);td.style.cssText="width:100%; cursor:move;";td.innerHTML='Title';_tdTitle=td;tr.style.height="20px";tr.style.backgroundColor="#A0CFEC";td=document.createElement('td');td.style.width='16px';tr.appendChild(td);_imgOK=document.createElement("img");_imgOK.src="libjs/ok.png";_imgOK.style.cursor="pointer";_imgOK.style.display='inline';td.appendChild(_imgOK);td=document.createElement('td');td.style.width='16px';tr.appendChild(td);_imgCancel=document.createElement("img");_imgCancel.src="libjs/cancel.png";_imgCancel.style.cursor="pointer";_imgCancel.style.display='inline';td.appendChild(_imgCancel);tr=document.createElement('tr');tbody.appendChild(tr);td=document.createElement('td');td.colSpan=4;tr.appendChild(td);td.style.width="100%";td.style.height="100%";tr.style.width="100%";tr.style.height='100%';_frmDialog=document.createElement('iframe');_frmDialog.limnorDialog=true;_frmDialog.style.height='100%';_frmDialog.style.width='100%';_frmDialog.border=0;_frmDialog.style.border='none';_frmDialog.style.overflow='hidden';td.appendChild(_frmDialog);_frmWait=document.createElement('iframe');document.body.appendChild(_frmWait);_frmWait.style.position="absolute";_frmWait.style.top='0px';_frmWait.style.left='0px';_frmWait.style.height='100%';_frmWait.style.width='100%';_frmWait.style.border='none';_frmWait.style.opacity='0.10';_frmWait.style.filter='alpha(opacity=10)';_resizer=document.createElement("img");_resizer.style.position='absolute';_resizer.style.display='none';_resizer.style.cursor='nw-resize';_resizer.src='libjs/resizer.gif';_resizer.ondragstart=function(){return false;};if(JsonDataBinding.IsIE()){_resizer.onresizestart=function(){return false;};_resizer.setAttribute("unselectable","on");}
document.body.appendChild(_resizer);JsonDataBinding.AttachEvent(document,"onmousedown",docmousedown);JsonDataBinding.AttachEvent(document,"onmouseup",docmouseup);JsonDataBinding.AttachEvent(document,"onmousemove",docmousemove);JsonDataBinding.AttachEvent(_imgOK,"onclick",onOK);JsonDataBinding.AttachEvent(_imgCancel,"onclick",onCancel);attachWaitFrmEvent();}
if(typeof dlg.overflow!='undefined'){_frmDialog.style.overflow=dlg.overflow;}
dlg.finished=false;var zi=JsonDataBinding.getPageZIndex()+1;_frmWait.style.zIndex=zi;if(dlg.icon&&dlg.icon.length>0){_imgIcon.src=dlg.icon;}
else{_imgIcon.style.display='none';}
if(dlg.title){_tdTitle.innerHTML=dlg.title;}
else{_tdTitle.innerHTML='';}
if(dlg.hideTitle){_tdTitle.parentNode.style.display='none';}
if(dlg.titleBkColor){_tdTitle.style.backgroundColor=dlg.titleBkColor;}
if(dlg.titleColor){_tdTitle.style.color=dlg.titleColor;}
if(dlg.iconClose){_imgCancel.src=dlg.iconClose;}
if(dlg.iconClose){_imgOK.src=dlg.iconOK;}
if(dlg.border)
_divDialog.style.border=dlg.border;else
_divDialog.style.border="1px solid gray";if(dlg.borderWidth)
_divDialog.style.borderWidth=dlg.borderWidth;else
_divDialog.style.borderWidth="thin";if(dlg.url){_frmDialog.src=dlg.url+(dlg.forceNew?'?fnew='+(Math.random()):'');}
else{_frmDialog.src='';}
zi++;_divDialog.style.zIndex=zi;_divDialog.style.display="block";if(typeof dlg.width=='undefined'||dlg.width==0)
_divDialog.style.width="600px";else{if(dlg.width){if(JsonDataBinding.isNumber(dlg.width)){_divDialog.style.width=dlg.width+'px';}
else{_divDialog.style.width=dlg.width;}}}
if(typeof dlg.height=='undefined'||dlg.height==0)
_divDialog.style.height="500px";else{if(dlg.height){if(JsonDataBinding.isNumber(dlg.height)){_divDialog.style.height=dlg.height+'px';}
else{_divDialog.style.height=dlg.height;}}}
if(dlg.isDialog){_frmWait.style.height='100%';_frmWait.style.width='100%';_frmWait.style.display="block";_imgOK.style.display="inline";}
else{_frmWait.style.display="none";_imgOK.style.display="none";}
if(dlg.hideCloseButtons==1||dlg.hideCloseButtons=='HideOKCancel'){_imgOK.style.display="none";_imgCancel.style.display="none";}
else if(dlg.hideCloseButtons==2||dlg.hideCloseButtons=='HideOK'){_imgOK.style.display="none";}
else if(dlg.hideCloseButtons==3||dlg.hideCloseButtons=='HideCancel'){_imgCancel.style.display="none";}
if(dlg.center){JsonDataBinding.windowTools.centerElementOnScreen(_divDialog);}
else{if(dlg.top){_divDialog.style.top=dlg.top+'px';}
else{_divDialog.style.top='0px';}
if(dlg.left){_divDialog.style.left=dlg.left+'px';}
else{_divDialog.style.left='0px';}}
_showResizer(dlg.resizable);function _closeDialog(){_hide(true,true);}
function _cancelDialog(){_hide(false,true);}
function _hideWindow(){_hide(false,false);}
function _getPageDoc(){if(_docDialog){return _docDialog;}
if(_frmDialog){if(_frmDialog.contentDocument){_docDialog=_frmDialog.contentDocument;return _docDialog;}
if(_frmDialog.contentWindow){_docDialog=_frmDialog.contentWindow.document;return _docDialog;}}}
function _getPageWindow(){if(_frmDialog){return _frmDialog.contentWindow;}}
function _getChildElement(id){var d=_getPageDoc();_docDialog=d||_docDialog;return _docDialog.getElementById(id);}
function _getPageId(){var msg;try{var d=_getPageDoc();_docDialog=d||_docDialog;if(_docDialog&&_docDialog.pageId){return _docDialog.pageId;}}
catch(e){msg=(e.message?e.message:e);}
try{if(dlg.pageId){return dlg.pageId;}}
catch(e){msg=(e.message?e.message:e);}
if(msg){alert('Error accessing dialogues. The page might not working properly. '+msg);}}
function _isDialog(){return dlg.isDialog;}
function _hasFinished(){return dlg.finished;}
function _isVisible(){return _divDialog.style.display!='none';}
function _isSamePage(p){return p.url==dlg.url;}
function _getPageUrl(){return dlg.url;}
function _setOnBringToFront(h){_onBringToFront=h;}
function _displayResizer(){_showResizer(dlg.resizable);}
_api={getPageId:function(){return _getPageId();},getPageUrl:function(){return _getPageUrl();},getPageDoc:function(){return _getPageDoc();},getPageWindow:function(){return _getPageWindow();},closeDialog:function(){_closeDialog();},cancelDialog:function(){_cancelDialog();},hideWindow:function(){_hideWindow();},getChildElement:function(id){return _getChildElement(id);},bringToFront:function(){_bringToFront();},isDialog:function(){return _isDialog();},isVisible:function(){return _isVisible();},hasFinished:function(){return _hasFinished();},isSamePage:function(p){return _isSamePage(p);},onDocMouseDown:function(e){_onDocMouseDown(e);},onDocMouseMove:function(e){_onDocMouseMove(e);},onDocMouseUp:function(e){_onDocMouseUp(e);},isDocLoaded:function(){if(_frmDialog){if(_frmDialog.contentWindow){if(_frmDialog.contentWindow.document){return _frmDialog.contentWindow.document.readyState=='complete';}}}
return false;},onDocLoaded:function(){_displayResizer();},setOnBringToFront:function(h){_setOnBringToFront(h);},getPageHolder:function(){return _divDialog;}};_divDialog.jsData=_api;attachDlgDocEvent();if(dlg.resizable){setTimeout(function(){_showResizer(true);},10);}
return _api;}};JsonDataBinding.childWindows=function(){var _childObjs;function _showChild(p){if(!_childObjs){_childObjs=new Array();}
var i;for(i=0;i<_childObjs.length;i++){if(_childObjs[i].hasFinished()){_childObjs.splice(i,1);break;}}
for(i=0;i<_childObjs.length;i++){if(_childObjs[i].isDialog()){return;}}
var o;for(i=0;i<_childObjs.length;i++){if(_childObjs[i].isSamePage(p)){_childObjs[i].bringToFront();o=_childObjs[i];break;}}
if(!o){o=JsonDataBinding.DialogueBox.showDialog(p);_childObjs.push(o);}
function checkChildReady(){try{if(o.isDocLoaded()){o.onDocLoaded();if(document.childManager.onChildWindowReady){document.childManager.onChildWindowReady({target:o});}
return;}
setTimeout(checkChildReady,100);}
catch(er){}}
setTimeout(checkChildReady,100);return o;}
function _getDocById(pid){if(_childObjs){for(i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageId()==pid){var w=_childObjs[i].getPageWindow();if(w){return w.document;}}}}}
function _getWindowById(pid){if(_childObjs){for(i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageId()==pid){return _childObjs[i].getPageWindow();}}}}
function _hideChildbyUrl(url){if(_childObjs){var o;for(var i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageUrl()==url){o=_childObjs[i];break;}}
if(o){o.hideWindow();}}}
function _closeChildByUrl(url){if(_childObjs){var o;for(var i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageUrl()==url){o=_childObjs[i];break;}}
if(o){o.cancelDialog();}}}
function _remove(o){if(_childObjs){for(var i=0;i<_childObjs.length;i++){if(_childObjs[i]==o){_childObjs.splice(i,1);break;}}}}
function _closeAll(ws){if(_childObjs){if(!ws)
ws=new Array();for(var i=0;i<_childObjs.length;i++){ws.push(_childObjs[i]);}
for(var i=0;i<ws.length;i++){ws[i].closeDialog();}}}
function _confirmDialog(pid){if(_childObjs){var o;for(var i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageId()==pid){o=_childObjs[i];break;}}
if(o){o.closeDialog();}}}
function _closeWindow(pid){if(_childObjs){var o;for(var i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageId()==pid){o=_childObjs[i];break;}}
if(o){o.cancelDialog();return true;}}}
function _hideWindow(pid){if(_childObjs){var o;for(var i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageId()==pid){o=_childObjs[i];break;}}
if(o){o.hideWindow();}}}
function _getChildByPageId(pid){if(_childObjs){for(var i=0;i<_childObjs.length;i++){if(_childObjs[i].getPageId()==pid){return _childObjs[i];}}}}
function _onChildMouseDown(pid,e){var api=_getChildByPageId(pid);if(api){api.onDocMouseDown(e);}}
function _onChildMouseMove(pid,e){var api=_getChildByPageId(pid);if(api){api.onDocMouseMove(e);}}
function _onChildMouseUp(pid,e){var api=_getChildByPageId(pid);if(api){api.onDocMouseUp(e);}}
function _getProperty(name){return JsonDataBinding.PageValues[name];}
function _setProperty(name,val){JsonDataBinding.PageValues[name]=val;}
function _getServerProperty(name){return JsonDataBinding.values[name];}
function _execute(methodName,args){var m=window[methodName];if(!m){if(limnorPage){m=limnorPage[methodName];}}
if(m){return m.apply(window,args);}}
return{showChild:function(p){return _showChild(p);},hideChildbyUrl:function(url){_hideChildbyUrl(url);},closeChildByUrl:function(url){_closeChildByUrl(url);},remove:function(o){_remove(o);},closeWindow:function(pid){_closeWindow(pid);},confirmDialog:function(pid){_confirmDialog(pid);},hideWindow:function(pid){_hideWindow(pid);},closeAll:function(){_closeAll();},onChildMouseDown:function(pid,e){_onChildMouseDown(pid,e);},onChildDocMouseMove:function(pid,e){_onChildMouseMove(pid,e);},onChildDocMouseUp:function(pid,e){_onChildMouseUp(pid,e);},getDocById:function(pid){return _getDocById(pid);},getWindowById:function(pid){return _getWindowById(pid);},CloseDialogPrompt:'',CancelDialogPrompt:'',onClosingWindow:null,onChildWindowReady:null,getProperty:function(name){return _getProperty(name);},setProperty:function(name,val){return _setProperty(name,val);},getServerProperty:function(name){return _getServerProperty(name);},execute:function(methodName,args){return _execute(methodName,args);},createArray:function(){return new Array();},executeOne:function(methodName,arg){var args=new Array();args.push(arg);return _execute(methodName,args);}}};JsonDataBinding.showChild=function(p){return document.childManager.showChild(p);};JsonDataBinding.getDocById=function(pid){return document.childManager.getDocById(pid);};JsonDataBinding.getChildWindowById=function(pid){return document.childManager.getWindowById(pid);};JsonDataBinding.hideChild=function(url){return document.childManager.hideChildbyUrl(url);};JsonDataBinding.closeChild=function(url){return document.childManager.closeChildByUrl(url);};JsonDataBinding.setupChildManager=function(){if(!document.childManager){document.childManager=JsonDataBinding.childWindows();if(window.parent&&window!=window.parent){if(!JsonDataBinding.IsFireFox()&&!JsonDataBinding.IsIE()){function onDocMouseDown(e){window.parent.document.childManager.onChildMouseDown(document.pageId,e);}
function onDocMouseMove(e){window.parent.document.childManager.onChildDocMouseMove(document.pageId,e);}
function onDocMouseUp(e){window.parent.document.childManager.onChildDocMouseUp(document.pageId,e);}
JsonDataBinding.AttachEvent(document,"onmousedown",onDocMouseDown);JsonDataBinding.AttachEvent(document,"onmousemove",onDocMouseMove);JsonDataBinding.AttachEvent(document,"onmouseup",onDocMouseUp);}}
else{}}}
JsonDataBinding.getPropertyByPageId=function(pid,propertyName){var win=JsonDataBinding.getWindowById(pid);if(win){var doc=win.document;if(doc){var a=doc.childManager.createArray();a.push(propertyName);return doc.childManager.getProperty.apply(win,a);}}}
JsonDataBinding.setPropertyByPageId=function(pid,propertyName,val){var doc=JsonDataBinding.getDocumentById(pid);if(doc){return doc.childManager.setProperty(propertyName,val);}}
JsonDataBinding.getServerPropertyByPageId=function(pid,propertyName){var doc=JsonDataBinding.getDocumentById(pid);if(doc){return doc.childManager.getServerProperty(propertyName);}}
JsonDataBinding.executeByPageId=function(pid,methodName){var doc=JsonDataBinding.getDocumentById(pid);if(doc){var a=doc.childManager.createArray();for(var i=2;i<arguments.length;i++){a.push(arguments[i]);}
return doc.childManager.execute(methodName,a);}
else{}}
JsonDataBinding.hidePage=function(){if(window!=window.parent){if(window.parent.document.childManager){window.parent.document.childManager.hideWindow(document.pageId);}}}
JsonDataBinding.closePage=function(){if(window!=window.parent){if(window.parent.document.childManager){if(!window.parent.document.childManager.closeWindow(document.pageId)){try{window.parent.document.childManager.closeAll();}
catch(e){}}}}}
JsonDataBinding.confirmDialog=function(){if(window!=window.parent){if(window.parent.document.childManager){window.parent.document.childManager.confirmDialog(document.pageId);}}}
JsonDataBinding.UploadFile=function(targetFolder,onFinish){}
var HtmlEditorMenuBar=HtmlEditorMenuBar||{limnorMenuStylesName:function(){return'limnorstyles_menu';},onCreatedObject:function(o,client){client.addStdJsFile(1);o.jsData=HtmlEditorMenuBar.createMenuStyles(o);o.jsData.setMenuData();o.className=o.jsData.getClassName();var ul=document.createElement('ul');o.appendChild(ul);var li=document.createElement('li');ul.appendChild(li);var a=document.createElement('a');a.innerHTML='Home';li.appendChild(a);li=document.createElement('li');ul.appendChild(li);a=document.createElement('a');a.innerHTML='Contact';li.appendChild(a);},designInit:function(pageEditor,calledClient){var objs=document.getElementsByTagName('nav');if(objs){tag=HtmlEditorMenuBar.limnorMenuStylesName();for(i=0;i<objs.length;i++){if(objs[i].className){var cc=objs[i].className.split(' ');for(var j=0;j<cc.length;j++){if(JsonDataBinding.startsWith(cc[j],tag)){HtmlEditorMenuBar.initializeMenuData(calledClient,objs[i]);break;}}}}}},cleanup:function(client){var found=false;var uls=document.getElementsByTagName('nav');if(uls){var tag=HtmlEditorMenuBar.limnorMenuStylesName();for(var i=0;i<uls.length;i++){var cssNames=uls[i].className;if(cssNames){var ns=cssNames.split(' ');for(var k=0;k<ns.length;k++){if(JsonDataBinding.startsWithI(ns[k],tag)){found=true;break;}}
if(found){break;}}}}
return found;},createMenuStyles:function(nav,menuData){var _styleSheet;var mid;if(!menuData){menuData={};mid=HtmlEditorMenuBar.limnorMenuStylesName();menuData.fontFamily='';menuData.fontSize='';menuData.classNames='';}
else{mid=HtmlEditorMenuBar.limnorMenuStylesName()+(menuData.menuId?menuData.menuId:'');}
if(typeof menuData.fullwidth=='undefined')menuData.fullwidth=false;if(!menuData.menubarHoverTextColor)menuData.menubarHoverTextColor='#fff';if(!menuData.anchorTextColor)menuData.anchorTextColor='#757575';if(!menuData.anchorPaddingY)menuData.anchorPaddingY=10;if(!menuData.anchorPaddingX)menuData.anchorPaddingX=40;if(!menuData.menubarUpperBkColor)menuData.menubarUpperBkColor='#efefef';if(!menuData.menubarLowerBkColor)menuData.menubarLowerBkColor='#bbbbbb';if(!menuData.menubarRadius)menuData.menubarRadius=10;if(!menuData.menuItemHoverUpperBkColor)menuData.menuItemHoverUpperBkColor='#4f5964';if(!menuData.menuItemHoverLowerBkColor)menuData.menuItemHoverLowerBkColor='#bbbbbb';if(!menuData.itemHoverUpperBkColor)menuData.itemHoverUpperBkColor='#add8e6';if(!menuData.itemHoverLowerBkColor)menuData.itemHoverLowerBkColor='#bbbbbb';if(!menuData.dropdownUpperBkColor)menuData.dropdownUpperBkColor='#5f6975';if(!menuData.dropdownLowerBkColor)menuData.dropdownLowerBkColor='#bbbbbb';if(!menuData.itemRadius)menuData.itemRadius=10;if(!menuData.itemPaddingX)menuData.itemPaddingX=40;if(!menuData.itemPaddingY)menuData.itemPaddingY=8;if(!menuData.itemTextColor)menuData.itemTextColor='#fff';if(!menuData.marginTop&&menuData.marginTop!=0)menuData.marginTop=2;if(!menuData.marginBottom&&menuData.marginBottom!=0)menuData.marginBottom=2;if(!menuData.classNames)menuData.classNames='';var _selectorMenubarFullWidth='nav.'+mid+' > ul';var _selectorMenuArrow='nav.'+mid+' ul a[isParent=1]::after';var _selectorMenubar='nav.'+mid+' ul';var _selectorMenubarAfter='nav.'+mid+' ul::after';var _selectorMenubarItem='nav.'+mid+' ul li';var _selectorMenubarHover='nav.'+mid+' ul li:hover';var _selectorHoverText='nav.'+mid+' ul li:hover a';var _selectorItemHoverMenubar='nav.'+mid+' ul li:hover > ul';var _selectorAnchor='nav.'+mid+' ul li a';var _selectorDropdown='nav.'+mid+' ul ul';var _selectorItem='nav.'+mid+' ul ul li';var _selectorItemAnchor='nav.'+mid+' ul ul li a';var _selectorItemHover='nav.'+mid+' ul ul li a:hover';var _selectorSubItem='nav.'+mid+' ul ul ul';function setSelectorNames(){_selectorMenubarFullWidth='nav.'+mid+' > ul';_selectorMenuArrow='nav.'+mid+' ul a[isparent]::after';_selectorMenubar='nav.'+mid+' ul';_selectorMenubarAfter='nav.'+mid+' ul::after';_selectorMenubarItem='nav.'+mid+' ul li';_selectorMenubarHover='nav.'+mid+' ul li:hover';_selectorHoverText='nav.'+mid+' ul li:hover a';_selectorItemHoverMenubar='nav.'+mid+' ul li:hover > ul';_selectorAnchor='nav.'+mid+' ul li a';_selectorDropdown='nav.'+mid+' ul ul';_selectorItem='nav.'+mid+' ul ul li';_selectorItemAnchor='nav.'+mid+' ul ul li a';_selectorItemHover='nav.'+mid+' ul ul li a:hover';_selectorSubItem='nav.'+mid+' ul ul ul';}
setSelectorNames();function getCssMenubarFullWidth(){if(menuData.fullwidth)
return _selectorMenubarFullWidth+'{width:100%;}';return _selectorMenubarFullWidth+'{width:auto;}';}
function getCssMenubarArrow(){return _selectorMenuArrow+'{'+'content: url("/libjs/arrow.gif"); display: block;position:absolute;top:40%;left:90%;'+'}';}
function getCssMenubar(){return _selectorMenubar+'{'+'background-image: linear-gradient(top, '+menuData.menubarUpperBkColor+' 0%, '+menuData.menubarLowerBkColor+' 100%);'+'background-image: -o-linear-gradient(top, '+menuData.menubarUpperBkColor+' 0%, '+menuData.menubarLowerBkColor+' 100%);'+'background-image: -moz-linear-gradient(top, '+menuData.menubarUpperBkColor+' 0%, '+menuData.menubarLowerBkColor+' 100%);'+'background-image: -webkit-linear-gradient(top, '+menuData.menubarUpperBkColor+' 0%, '+menuData.menubarLowerBkColor+' 100%);'+'background-image: -ms-linear-gradient(top, '+menuData.menubarUpperBkColor+' 0%, '+menuData.menubarLowerBkColor+' 100%);'+'box-shadow: 0px 0px 9px rgba(0,0,0,0.15);'+'padding: 0 20px;'+'border-top-left-radius: '+menuData.menubarRadius+'px;'+'border-top-right-radius: '+menuData.menubarRadius+'px;'+'border-bottom-right-radius: '+menuData.menubarRadius+'px;'+'border-bottom-left-radius: '+menuData.menubarRadius+'px;'+'list-style: none;'+'position: relative;'+'display: inline-table;'+'margin-top: '+menuData.marginTop+'px;'+'margin-bottom: '+menuData.marginBottom+'px;'+'}';}
function getCssMenubarAfter(){return _selectorMenubarAfter+'{content: ""; clear: both; display: block;}';}
function getCssMenubarItem(){return _selectorMenubarItem+'{float: left;}';}
function getCssMenubarHover(){return _selectorMenubarHover+'{'+'background-image: linear-gradient(top, '+menuData.menuItemHoverUpperBkColor+' 0%, '+menuData.menuItemHoverLowerBkColor+' 100%);'+'background-image: -o-linear-gradient(top, '+menuData.menuItemHoverUpperBkColor+' 0%, '+menuData.menuItemHoverLowerBkColor+' 100%);'+'background-image: -moz-linear-gradient(top, '+menuData.menuItemHoverUpperBkColor+' 0%, '+menuData.menuItemHoverLowerBkColor+' 100%);'+'background-image: -webkit-linear-gradient(top, '+menuData.menuItemHoverUpperBkColor+' 0%, '+menuData.menuItemHoverLowerBkColor+' 100%);'+'background-image: -ms-linear-gradient(top , '+menuData.menuItemHoverUpperBkColor+' 0%, '+menuData.menuItemHoverLowerBkColor+' 100%);'+'box-shadow: 0px 0px 9px rgba(0,0,0,0.15);'+'border-top-left-radius: '+menuData.menubarRadius+'px;'+'border-top-right-radius: '+menuData.menubarRadius+'px;'+'border-bottom-right-radius: '+menuData.menubarRadius+'px;'+'border-bottom-left-radius: '+menuData.menubarRadius+'px;'+'}';}
function getCssMenubarHoverTextColor(){return _selectorHoverText+'{color: '+menuData.menubarHoverTextColor+';}';}
function getCssItemHoverMenubar(){return _selectorItemHoverMenubar+'{display: block;}';}
function getCssMenubarItemText(){return _selectorAnchor+'{'+'padding-top: '+menuData.anchorPaddingY+'px;'+'padding-bottom: '+menuData.anchorPaddingY+'px;'+'padding-left: '+menuData.anchorPaddingX+'px;'+'padding-right: '+menuData.anchorPaddingX+'px;'+'color: '+menuData.anchorTextColor+'; text-decoration: none; display: block;cursor:pointer;'+
(menuData.fontFamily?'font-family:'+menuData.fontFamily+';':'')+(menuData.fontSize?'font-size:'+menuData.fontSize+';':'')+'}';}
function getCssDropDown(){return _selectorDropdown+'{'+'background-image: linear-gradient(top, '+menuData.dropdownUpperBkColor+' 0%, '+menuData.dropdownLowerBkColor+' 100%);'+'background-image: -o-linear-gradient(top, '+menuData.dropdownUpperBkColor+' 0%, '+menuData.dropdownLowerBkColor+' 100%);'+'background-image: -moz-linear-gradient(top, '+menuData.dropdownUpperBkColor+' 0%, '+menuData.dropdownLowerBkColor+' 100%);'+'background-image: -webkit-linear-gradient(top, '+menuData.dropdownUpperBkColor+' 0%, '+menuData.dropdownLowerBkColor+' 100%);'+'background-image: -ms-linear-gradient(top, '+menuData.dropdownUpperBkColor+' 0%, '+menuData.dropdownLowerBkColor+' 100%);'+'border-top-left-radius: '+menuData.itemRadius+'px;'+'border-top-right-radius: '+menuData.itemRadius+'px;'+'border-bottom-right-radius: '+menuData.itemRadius+'px;'+'border-bottom-left-radius: '+menuData.itemRadius+'px;'+'padding: 0; margin-top:-2px; '+'position: absolute; top: 100%;'+'display: none;'+'}';}
function getCssDropdownItem(){return _selectorItem+'{'+'float: none;'+'border-top: 1px solid #6b727c;'+'border-bottom: 1px solid #575f6a;'+'position: relative;'+'border-radius: '+menuData.itemRadius+'px;'+'}';}
function getCssItemAnchor(){return _selectorItemAnchor+'{'+'padding-top: '+menuData.itemPaddingY+'px;'+'padding-bottom: '+menuData.itemPaddingY+'px;'+'padding-left: '+menuData.itemPaddingX+'px;'+'padding-right: '+menuData.itemPaddingX+'px;'+'color: '+menuData.itemTextColor+';'+'border-radius: '+menuData.itemRadius+'px;'+'}';}
function getCssItemHover(){return _selectorItemHover+'{'+'background-image: linear-gradient(top, '+menuData.itemHoverUpperBkColor+' 0%, '+menuData.itemHoverLowerBkColor+' 100%);'+'background-image: -o-linear-gradient(top, '+menuData.itemHoverUpperBkColor+' 0%, '+menuData.itemHoverLowerBkColor+' 100%);'+'background-image: -moz-linear-gradient(top, '+menuData.itemHoverUpperBkColor+' 0%, '+menuData.itemHoverLowerBkColor+' 100%);'+'background-image: -webkit-linear-gradient(top, '+menuData.itemHoverUpperBkColor+' 0%,'+menuData.itemHoverLowerBkColor+' 100%);'+'background-image: -ms-linear-gradient(top , '+menuData.itemHoverUpperBkColor+' 0%, '+menuData.itemHoverLowerBkColor+' 100%);'+'border-radius: '+menuData.itemRadius+'px;'+'}';}
function getCssSubItem(){return _selectorSubItem+'{position: absolute; left: 100%; top:0;}';}
function getMenuStyle(){if(!_styleSheet){_styleSheet=limnorHtmlEditorClient.getDynamicStyleNode();}
if(!_styleSheet){var st=document.createElement('style');st.title=limnorHtmlEditorClient.limnorDynaStyleTitle;st.setAttribute('hidden','true');document.getElementsByTagName('head')[0].appendChild(st);for(var s=0;s<document.styleSheets.length;s++){if(document.styleSheets[s].title==limnorHtmlEditorClient.limnorDynaStyleTitle){_styleSheet=document.styleSheets[s];break;}}}
return _styleSheet;}
function getRuleBySelector(selector){var st=getMenuStyle();var rs;if(st.cssRules){rs=st.cssRules;}
else if(st.rules){rs=st.rules;}
if(rs){for(var r=0;r<rs.length;r++){if(rs[r].selectorText==selector){return{rule:rs[r],idx:r};}}}}
function _setRule(selector,ruleGetter){var rule=getRuleBySelector(selector);var idx;if(rule){if(_styleSheet.deleteRule){_styleSheet.deleteRule(rule.idx);}
else{_styleSheet.removeRule(rule.idx);}
idx=rule.idx;}
else{if(_styleSheet.cssRules){idx=_styleSheet.cssRules.length;}
else if(_styleSheet.rules){idx=_styleSheet.rules.length;}
else{idx=0;}}
if(_styleSheet.insertRule){_styleSheet.insertRule(ruleGetter(),idx);}
else{_styleSheet.addRule(selector,ruleGetter(),idx);}}
function _setCssMenubarFullWidth(){_setRule(_selectorMenubarFullWidth,getCssMenubarFullWidth);}
function _setCssMenubarArrow(){_setRule(_selectorMenuArrow,getCssMenubarArrow);}
function _setCssMenubar(){_setRule(_selectorMenubar,getCssMenubar);}
function _setCssMenubarAfter(){_setRule(_selectorMenubarAfter,getCssMenubarAfter);}
function _setCssMenubarItem(){_setRule(_selectorMenubarItem,getCssMenubarItem);}
function _setCssMenubarHover(){_setRule(_selectorMenubarHover,getCssMenubarHover);}
function _setCssMenubarHoverTextColor(){_setRule(_selectorHoverText,getCssMenubarHoverTextColor);}
function _setCssItemHoverMenubar(){_setRule(_selectorItemHoverMenubar,getCssItemHoverMenubar);}
function _getClassName(){if(menuData.classNames&&menuData.classNames.length>0){return menuData.classNames+' '+mid;}
return mid;}
function _setMenubarClasses(){nav.className=_getClassName();}
function _setCssMenubarItemText(){_setRule(_selectorAnchor,getCssMenubarItemText);}
function _setCssDropdown(){_setRule(_selectorDropdown,getCssDropDown);}
function _setCssDropdownItem(){_setRule(_selectorItem,getCssDropdownItem);}
function _setCssItemAnchor(){_setRule(_selectorItemAnchor,getCssItemAnchor);}
function _setCssItemHover(){_setRule(_selectorItemHover,getCssItemHover);}
function _setCssSubItem(){_setRule(_selectorSubItem,getCssSubItem);}
function _init(){mid=HtmlEditorMenuBar.limnorMenuStylesName()+(menuData.menuId?menuData.menuId:'');setSelectorNames();_setMenubarClasses();_setCssMenubarFullWidth();_setCssMenubarArrow();_setCssMenubar();_setCssMenubarAfter();_setCssMenubarItem();_setCssMenubarHover();_setCssMenubarHoverTextColor();_setCssItemHoverMenubar();_setCssMenubarItemText();_setCssDropdown();_setCssDropdownItem();_setCssItemAnchor();_setCssItemHover();_setCssSubItem();}
return{typename:'menubar',createSubEditor:function(o,e){return o;},setMenuData:function(data){if(data){menuData=data;}
_init();},getFullWidth:function(){return menuData.fullwidth;},setFullWidth:function(v){menuData.fullwidth=JsonDataBinding.isValueTrue(v);_setCssMenubarFullWidth();},getClassNames:function(){return menuData.classNames;},setClassNames:function(v){if(menuData.classNames!=v){menuData.classNames=v;_setMenubarClasses();}},getFontFamily:function(){return menuData.fontFamily;},setFontFamily:function(v){if(menuData.fontFamily!=v){menuData.fontFamily=v;_setCssMenubarItemText();}},getFontSize:function(){return menuData.fontSize;},setFontSize:function(v){if(menuData.fontSize!=v){menuData.fontSize=v;_setCssMenubarItemText();}},getMenubarMarginTop:function(){return menuData.marginTop;},setMenubarMarginTop:function(v){if(menuData.marginTop!=v){menuData.marginTop=v;_setCssMenubar();}},getMenubarMarginBottom:function(){return menuData.marginBottom;},setMenubarMarginBottom:function(v){if(menuData.marginBottom!=v){menuData.marginBottom=v;_setCssMenubar();}},getMenubarPaddingX:function(){return menuData.anchorPaddingX;},setMenubarPaddingX:function(v){if(menuData.anchorPaddingX!=v){menuData.anchorPaddingX=v;_setCssMenubarItemText();}},getMenubarPaddingY:function(){return menuData.anchorPaddingY;},setMenubarPaddingY:function(v){if(menuData.anchorPaddingY!=v){menuData.anchorPaddingY=v;_setCssMenubarItemText();}},getMenubarRadius:function(){return menuData.menubarRadius;},setMenubarRadius:function(v){if(menuData.menubarRadius!=v){menuData.menubarRadius=v;_setCssMenubar();_setCssMenubarHover();}},getMenubarTextColor:function(){return menuData.anchorTextColor;},setMenubarTextColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.anchorTextColor!=v){menuData.anchorTextColor=v;_setCssMenubarItemText();}}},getMenubarHoverTextColor:function(){return menuData.menubarHoverTextColor;},setMenubarHoverTextColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.menubarHoverTextColor!=v){menuData.menubarHoverTextColor=v;_setCssMenubarHoverTextColor();}}},getMenubarBkColor:function(){return menuData.menubarUpperBkColor;},setMenubarBkColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.menubarUpperBkColor!=v){menuData.menubarUpperBkColor=v;_setCssMenubar();}}},getMenubarGradientColor:function(){return menuData.menubarLowerBkColor;},setMenubarGradientColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.menubarLowerBkColor!=v){menuData.menubarLowerBkColor=v;_setCssMenubar();}}},getMenubarHoverBkColor:function(){return menuData.menuItemHoverUpperBkColor;},setMenubarHoverBkColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.menuItemHoverUpperBkColor!=v){menuData.menuItemHoverUpperBkColor=v;_setCssMenubarHover();}}},getMenubarHoverGradientColor:function(){return menuData.menuItemHoverLowerBkColor;},setMenubarHoverGradientColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.menuItemHoverLowerBkColor!=v){menuData.menuItemHoverLowerBkColor=v;_setCssMenubarHover();}}},getItemHoverBkColor:function(){return menuData.itemHoverUpperBkColor;},setItemHoverBkColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.itemHoverUpperBkColor!=v){menuData.itemHoverUpperBkColor=v;_setCssItemHover();}}},getItemHoverGradientColor:function(){return menuData.itemHoverLowerBkColor;},setItemHoverGradientColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.itemHoverLowerBkColor!=v){menuData.itemHoverLowerBkColor=v;_setCssItemHover();}}},getDropdownBkColor:function(){return menuData.dropdownUpperBkColor;},setDropdownBkColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.dropdownUpperBkColor!=v){menuData.dropdownUpperBkColor=v;_setCssDropdown();}}},getDropdownGradientColor:function(){return menuData.dropdownLowerBkColor;},setDropdownGradientColor:function(v){if(v){if(v.charAt(0)!='#'&&v.charAt(0)!='r'){v='#'+v;}
if(menuData.dropdownLowerBkColor!=v){menuData.dropdownLowerBkColor=v;_setCssDropdown();}}},getItemPaddingX:function(){return menuData.itemPaddingX;},setItemPaddingX:function(v){if(menuData.itemPaddingX!=v){menuData.itemPaddingX=v;_setCssItemAnchor();}},getItemPaddingY:function(){return menuData.itemPaddingY;},setItemPaddingY:function(v){if(menuData.itemPaddingY!=v){menuData.itemPaddingY=v;_setCssItemAnchor();}},getItemRadius:function(){return menuData.itemRadius;},setItemRadius:function(v){if(menuData.itemRadius!=v){menuData.itemRadius=v;_setCssDropdown();_setCssDropdownItem();_setCssItemAnchor();_setCssItemHover();}},getClassName:function(){return _getClassName();},getMenuData:function(){return menuData;},setStylesName:function(v){},loadFromStyles:function(styles,client){if(styles){var rs;if(styles.cssRules){rs=styles.cssRules;}
else if(styles.rules){rs=styles.rules;}
if(rs){function getbackground(txt){var ret={};var pos,pos2,v;var val=client.getCssProperty(txt,'background');if(!val){val=client.getCssProperty(txt,'background-image');}
if(val){val=val.trim();pos=val.indexOf('linear-gradient');if(pos>=0){pos=val.indexOf('(',pos+15);if(pos>=0){var pos2=val.lastIndexOf(')');if(pos2>pos){v=val.substr(pos+1,pos2-pos-1);pos=v.indexOf('rgb');if(pos>=0){pos2=v.indexOf(')',pos);if(pos2>0){ret.color1=v.substr(pos,pos2-pos+1);pos=v.indexOf('rgb',pos2);if(pos>0){pos2=v.indexOf(')',pos);if(pos2>0){ret.color2=v.substr(pos,pos2-pos+1);}}}}}}}}
val=client.getCssProperty(txt,'border-radius');if(!val){val=client.getCssProperty(txt,'border-top-left-radius');}
if(val){val=val.trim();pos=val.indexOf('px');if(pos>=0){if(pos>0){ret.radius=val.substr(0,pos);}
else
ret.radius=0;}
else
ret.radius=val;}
return ret;}
function getTextAttrs(txt){var ret={};var pos;var val=client.getCssProperty(txt,'padding');if(val){val=val.trim();var vs=val.split(' ');if(vs.length>0){pos=vs[0].indexOf('px');if(pos>=0){if(pos>0)
ret.padY=vs[0].substr(0,pos);else
ret.padY=0;}
else
ret.padY=vs[0];if(vs.length>1){pos=vs[1].indexOf('px');if(pos>=0){if(pos>0)
ret.padX=vs[1].substr(0,pos);else
ret.padX=0;}
else
ret.padX=vs[1];}}}
else{val=client.getCssProperty(txt,'padding-top');if(val){pos=val.indexOf('px');if(pos>=0){if(pos>0)
ret.padY=val.substr(0,pos);else
ret.padY=0;}
else
ret.padY=val;}
val=client.getCssProperty(txt,'padding-left');if(val){pos=val.indexOf('px');if(pos>=0){if(pos>0)
ret.padX=val.substr(0,pos);else
ret.padX=0;}
else
ret.padX=val;}}
val=client.getCssProperty(txt,'color');if(val){ret.color=val.trim();}
val=client.getCssProperty(txt,'font-family');if(val){val=val.trim();if(val.length>0){if(val.substr(0,1)=='"'||val.substr(0,1)=="'"){val=val.substr(1);}}
if(val.length>0){if(val.substr(val.length-1,1)=='"'||val.substr(val.length-1,1)=="'"){val=val.substr(0,val.length-1);}}
ret.fontFamily=val.trim();}
val=client.getCssProperty(txt,'font-size');if(val){ret.fontSize=val.trim();}
return ret;}
for(var r=0;r<rs.length;r++){if(rs[r].selectorText&&rs[r].selectorText.length>0){var val,pos;var s=rs[r].selectorText.toLowerCase();if(s==_selectorMenubarFullWidth){val=client.getCssProperty(rs[r].cssText,'width');if(val=='auto')
menuData.fullwidth=false;else if(val=='100%')
menuData.fullwidth=true;else
menuData.fullwidth=false;}
else if(s==_selectorMenubar){val=getbackground(rs[r].cssText);if(val){if(val.color1)
menuData.menubarUpperBkColor=val.color1;if(val.color2)
menuData.menubarLowerBkColor=val.color2;if(typeof val.radius!='undefined')
menuData.menubarRadius=val.radius;}
val=client.getCssProperty(rs[r].cssText,'margin-top');if(typeof(val)!='undefined'&&val!=null&&val.length>0){if(val.length>1&&val.substr(val.length-2)=='px'){val=val.substr(0,val.length-2).trim();}
menuData.marginTop=parseInt(val);}
val=client.getCssProperty(rs[r].cssText,'margin-bottom');if(typeof(val)!='undefined'&&val!=null&&val.length>0){if(val.length>1&&val.substr(val.length-2)=='px'){val=val.substr(0,val.length-2).trim();}
menuData.marginBottom=parseInt(val);}}
else if(s==_selectorMenubarHover){val=getbackground(rs[r].cssText);if(val){if(val.color1)
menuData.menuItemHoverUpperBkColor=val.color1;if(val.color2)
menuData.menuItemHoverLowerBkColor=val.color2;if(typeof val.radius!='undefined')
menuData.menubarRadius=val.radius;}}
else if(s==_selectorHoverText){val=client.getCssProperty(rs[r].cssText,'color');if(val){menuData.menubarHoverTextColor=val;}}
else if(s==_selectorAnchor){val=getTextAttrs(rs[r].cssText);if(val){if(typeof val.padX!='undefined')
menuData.anchorPaddingX=val.padX;if(typeof val.padY!='undefined')
menuData.anchorPaddingY=val.padY;if(val.fontFamily)
menuData.fontFamily=val.fontFamily;if(val.fontSize)
menuData.fontSize=val.fontSize;if(val.color)
menuData.anchorTextColor=val.color;}}
else if(s==_selectorDropdown){val=getbackground(rs[r].cssText);if(val){if(val.color1)
menuData.dropdownUpperBkColor=val.color1;if(val.color2)
menuData.dropdownLowerBkColor=val.color2;if(typeof val.radius!='undefined')
menuData.itemRadius=val.radius;}}
else if(s==_selectorItemAnchor){val=getTextAttrs(rs[r].cssText);if(val){if(typeof val.padX!='undefined')
menuData.itemPaddingX=val.padX;if(typeof val.padY!='undefined')
menuData.itemPaddingY=val.padY;}}
else if(s==_selectorItemHover){val=getbackground(rs[r].cssText);if(val){if(val.color1)
menuData.itemHoverUpperBkColor=val.color1;if(val.color2)
menuData.itemHoverLowerBkColor=val.color2;if(typeof val.radius!='undefined')
menuData.itemRadius=val.radius;}}}}}}}};},initializeMenuData:function(client,nav){var menuClass;var menuClasses=nav.className;if(menuClasses){var cs=menuClasses.split(' ');if(cs){for(var i=0;i<cs.length;i++){if(JsonDataBinding.startsWith(cs[i],HtmlEditorMenuBar.limnorMenuStylesName())){menuClass=cs[i];break;}}}}
if(menuClass){var menuData={};menuData.menuId=menuClass.substr(HtmlEditorMenuBar.limnorMenuStylesName().length);menuData.classNames=menuClasses.replace(menuClass,'');nav.typename='menubar';nav.jsData=HtmlEditorMenuBar.createMenuStyles(nav,menuData);var pageStyle=client.getPageCssLinkNode();nav.jsData.loadFromStyles(pageStyle.sheet?pageStyle.sheet:pageStyle.styleSheet,client);}},onBeforeSave:function(dyStyleNode,client){if(dyStyleNode){var rs;if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}
if(rs){var r,k,b;var menuIds=new Array();for(r=0;r<rs.length;r++){if(JsonDataBinding.startsWith(rs[r].selectorText,'nav.'+HtmlEditorMenuBar.limnorMenuStylesName())){var pos=rs[r].selectorText.indexOf(' ');if(pos>0){var s=rs[r].selectorText.substr(4,pos-4);b=false;for(k=0;k<menuIds.length;k++){if(menuIds[k]==s){b=true;break;}}
if(!b){menuIds.push(s);}}}}
var removedNames=client.getRemovedClassNames();if(removedNames){for(r=0;r<removedNames.length;r++){b=false;for(k=0;k<menuIds.length;k++){if(menuIds[k]==removedNames[r]){b=true;break;}}
if(!b){menuIds.push(removedNames[r]);}}}
for(k=0;k<menuIds.length;k++){var navs=document.getElementsByClassName(menuIds[k]);if(navs&&navs.length>0){}
else{var s1='nav.'+menuIds[k];var s2='nav.'+menuIds[k]+' ';r=0;while(r<rs.length){if(rs[r].selectorText==s1||JsonDataBinding.startsWith(rs[r].selectorText,s2)){if(dyStyleNode.deleteRule){dyStyleNode.deleteRule(r);}
else{dyStyleNode.removeRule(r);}}
else{r++;}}
var mid=menuIds[k];client.addToRemoveCssBySelector('nav.'+mid+' > ul');client.addToRemoveCssBySelector('nav.'+mid+' ul a[isparent]::after');client.addToRemoveCssBySelector('nav.'+mid+' ul');client.addToRemoveCssBySelector('nav.'+mid+' ul::after');client.addToRemoveCssBySelector('nav.'+mid+' ul li');client.addToRemoveCssBySelector('nav.'+mid+' ul li:hover');client.addToRemoveCssBySelector('nav.'+mid+' ul li:hover a');client.addToRemoveCssBySelector('nav.'+mid+' ul li:hover > ul');client.addToRemoveCssBySelector('nav.'+mid+' ul li a');client.addToRemoveCssBySelector('nav.'+mid+' ul ul');client.addToRemoveCssBySelector('nav.'+mid+' ul ul li');client.addToRemoveCssBySelector('nav.'+mid+' ul ul li a');client.addToRemoveCssBySelector('nav.'+mid+' ul ul li a:hover');client.addToRemoveCssBySelector('nav.'+mid+' ul ul ul');}}}}}};var HtmlEditorTreeview=HtmlEditorTreeview||{onCreatedObject:function(o,client){client.addStdJsFile(1);if(client.isforIDE)
client.addCssLinkFile('css/limnortv.css',true);else
client.addCssLinkFile('/css/limnortv.css',true);o.className='limnortv';JsonDataBinding.createTreeView(o);o.jsData.designer=HtmlEditorTreeview.createDesigner(o,client);},designInit:function(pageEditor,calledClient){var objs=document.getElementsByTagName('ul');if(objs){for(var i=0;i<objs.length;i++){if(objs[i].className){if(JsonDataBinding.hasClass(objs[i],'limnortv')){pageEditor.initPageElement.apply(pageEditor.editorWindow(),[objs[i]]);objs[i].jsData.designer=HtmlEditorTreeview.createDesigner(objs[i],calledClient);}}}}},cleanup:function(client){var tvCssNode,i;var head=document.getElementsByTagName('head')[0];var cssLst=head.getElementsByTagName('link');if(cssLst){for(i=0;i<cssLst.length;i++){var s=cssLst[i].getAttribute('href');if(JsonDataBinding.endsWithI(s,'/limnortv.css')){tvCssNode=cssLst[i];break;}}}
if(tvCssNode){var selectors=new Array();var uls=document.getElementsByTagName('ul');if(uls){for(i=0;i<uls.length;i++){var cssNames=uls[i].className;if(cssNames){var found=false;var sy='';var ns=cssNames.split(' ');for(var k=0;k<ns.length;k++){if(ns[k]=='limnortv'){found=true;}
else{if(ns[k]&&ns[k].length>0){sy=ns[k];}}}
if(found){if(sy.length>0){selectors.push('ul.'+sy+'.limnortv');selectors.push('ul.limnortv.'+sy);}
else{selectors.push('ul.limnortv');}}}}}
if(selectors.length==0){head.removeChild(tvCssNode);}
function removeTVstyles(tvSheet){if(tvSheet){var rs;if(tvSheet.cssRules){rs=tvSheet.cssRules;}
else if(tvSheet.rules){rs=tvSheet.rules;}
else{if(tvSheet.sheet){if(tvSheet.sheet.cssRules){rs=tvSheet.sheet.cssRules;}
else if(tvSheet.sheet.rules){rs=tvSheet.sheet.rules;}}}
if(rs){for(var i=0;i<rs.length;i++){if(rs[i].selectorText){var sl=rs[i].selectorText.toLowerCase();var pos=sl.indexOf(' ');var sl2='';if(pos>0){sl2=sl.substr(pos);sl=sl.substr(0,pos);}
if(sl=='ul.limnortv'||JsonDataBinding.startsWith(sl,'ul.limnortv.')||(JsonDataBinding.startsWith(sl,'ul.')&&JsonDataBinding.endsWith(sl,'.limnortv'))){var found=false;for(var k=0;k<selectors.length;k++){if(selectors[k]==sl){found=true;break;}}
if(!found){client.addToRemoveCssBySelector(rs[i].selectorText);if(sl!='ul.limnortv'){if(JsonDataBinding.startsWith(sl,'ul.limnortv.')){sl='ul.'+sl.substr(12)+'.limnortv';}
else{sl='ul.limnortv.'+sl.substr(3,sl.length-12);}}
client.addToRemoveCssBySelector(sl+sl2);}}}}}}}
removeTVstyles(client.getPageCssLinkNode());removeTVstyles(client.getDynamicStyleNode());return(selectors.length>0);}},createDesigner:function(tv,client){function _getSelectorHoverColor(st){if(typeof st!='undefined'&&st.length>0)
return'ul.limnortv.'+st+' li[hoverstate]';else
return'ul.limnortv li[hoverstate]';}
function _getSelectorHoverColor2(st){if(typeof st!='undefined'&&st.length>0)
return'ul.'+st+'.limnortv li[hoverstate]';else
return'ul.limnortv li[hoverstate]';}
function _getSelectorTreeview(st){if(typeof st!='undefined'&&st.length>0)
return'ul.limnortv.'+st;else
return'ul.limnortv';}
function _getSelectorTreeview2(st){if(typeof st!='undefined'&&st.length>0)
return'ul.'+st+'.limnortv';else
return'ul.limnortv';}
function _getHoverBkColor(){var st=_getStyleName();if(typeof limnorPageData!='undefined'&&limnorPageData.limnortv){if(st&&st.length>0){if(limnorPageData.limnortv.hoverBkColors&&limnorPageData.limnortv.hoverBkColors[st]){return limnorPageData.limnortv.hoverBkColors[st];}}
else{if(limnorPageData.limnortv.hoverBkColor){return limnorPageData.limnortv.hoverBkColor;}}}
var lnkNode=client.getPageCssStyle();if(lnkNode){var rs;if(lnkNode.cssRules){rs=lnkNode.cssRules;}
else if(lnkNode.rules){rs=lnkNode.rules;}
if(rs){var i;var selector=_getSelectorHoverColor(st);for(i=0;i<rs.length;i++){if(rs[i].selectorText==selector){return client.getCssProperty(rs[i].cssText,'background-color');}}
selector=_getSelectorHoverColor2(st);for(i=0;i<rs.length;i++){if(rs[i].selectorText==selector){return client.getCssProperty(rs[i].cssText,'background-color');}}}}}
function _setHoverBkColor(val){var st=_getStyleName();if(typeof limnorPageData=='undefined')
limnorPageData={};limnorPageData.limnortv=limnorPageData.limnortv||{};if(st&&st.length>0){limnorPageData.limnortv.hoverBkColors=limnorPageData.limnortv.hoverBkColors||{};limnorPageData.limnortv.hoverBkColors[st]=val;}
else{limnorPageData.limnortv.hoverBkColor=val;}
var selector=_getSelectorHoverColor(st);var selector2=_getSelectorHoverColor2(st);client.updateDynamicStyle(selector2,val,'backgroundColor','background-color',selector);tv.jsData.enableHoverState(val!=null);}
function _getStyleName(){var cs=tv.className;if(cs){var ns=cs.split(' ');for(var i=0;i<ns.length;i++){if(ns[i]&&ns[i].length>0&&ns[i]!='limnortv'){return ns[i];}}}}
function _setStyleName(val){var curName=_getStyleName();if(val!=curName){if(typeof val!='undefined'&&val.length>0){if(val.indexOf('limnortv')>=0){alert('Please do not use limnortv in your style name');}
else{if(client.HtmlEditor.IsNameValid(val)){tv.className='limnortv '+val;}
else{alert('It is an invalid style name. Use alphanumeric and underscores, starting with an alphabetic or underscore.');}}}
else{tv.className='limnortv';}}}
function _setter(name,value){var cssName=client.HtmlEditor.getCssNameFromPropertyName(name);if(cssName){var st=_getStyleName();var selector=_getSelectorTreeview(st);var selector2=_getSelectorTreeview2(st);client.updateDynamicStyle(selector2,value,name,cssName,selector);}}
function _getter(name){var cssName=client.HtmlEditor.getCssNameFromPropertyName(name);if(cssName){var st=_getStyleName();var ss=new Array();ss.push(_getSelectorTreeview(st));if(st&&st.length>0){ss.push(_getSelectorTreeview2(st));}
var v=client.getStyleValue(ss,cssName);if(typeof v=='undefined'){v=client.getElementStyleValue(tv,cssName);}
v=client.HtmlEditor.removeQuoting(v);return v;}}
function _hasSetter(name){if(name=='fontStyle'||name=='fontWeight'||name=='fontVariant'||name=='color'||name=='fontFamily'||name=='fontSize')
return true;}
function _removeColor(obj,name,cssName){if(name=='color'){var st=_getStyleName();var selector=_getSelectorTreeview(st);var selector2=_getSelectorTreeview2(st);client.updateDynamicStyle(selector2,null,'color','color',selector);}
else if(name=='HoverBackColor'){_setHoverBkColor(null);}}
return{hasSetter:function(name){return _hasSetter(name);},setter:function(name,value){_setter(name,value);},hasGetter:function(name){return _hasSetter(name);},getter:function(name){return _getter(name);},getStyleName:function(){return _getStyleName();},setStyleName:function(val){_setStyleName(val);},getHoverBkColor:function(){return _getHoverBkColor();},setHoverBkColor:function(val){_setHoverBkColor(val);},removeColor:function(obj,name,cssName){_removeColor(obj,name,cssName);}};}};JsonDataBinding.CreateTreeView=function(htmlElement){var _div=htmlElement;var _nodeSelectionHandlers=new Array();var _readOnly=true;var _jsTable;var _loadingMsg;var nodenameIdx=-1;var textIdx=-1;var imageIdx=-1;var nodedataIdx=-1;_div.mouseOverColor='#c0ffc0';_div.nodeBackColor='White';_div.selectedNodeColor='#c0c0ff';_div.isTreeView=true;function _selectNode(nd){if(_div.selectedNode){_div.selectedNode.isSelected=false;_div.selectedNode.style.backgroundColor=_div.nodeBackColor;}
if(nd==null){return;}
nd.isSelected=true;_div.selectedNode=nd;nd.style.backgroundColor=_div.selectedNodeColor;if(typeof nd.rowIndex!='undefined'){if(_jsTable.rowIndex!=nd.rowIndex){JsonDataBinding.dataMoveToRecord(_jsTable.TableName,nd.rowIndex);}}
_div.jsData.afterSelectNode();}
function _setCurrentPrimaryKey(parentNode){if(_jsTable){_div.currentPrimaryKeyValue=_jsTable.Rows[parentNode.rowIndex].ItemArray[_jsTable.columnIndexes[_div.primaryKey]];JsonDataBinding.setTableAttribute(_jsTable.TableName,'isDataStreaming',true);JsonDataBinding.setTableAttribute(_jsTable.TableName,'tv_parentnode',parentNode);if(typeof _div.currentPrimaryKeyValue=='undefined'||_div.currentPrimaryKeyValue==null){JsonDataBinding.setTableAttribute(_jsTable.TableName,'isFisrtTime',true);}
else{JsonDataBinding.setTableAttribute(_jsTable.TableName,'isFisrtTime',false);}}
_div.currentKeyNode=parentNode;}
function findChildNodeByRowIndex(parentTreeNode,r){for(var i=0;i<parentTreeNode.childNodes.length;i++){if(typeof parentTreeNode.childNodes[i].rowIndex!='undefined'){if(parentTreeNode.childNodes[i].rowIndex==r){return parentTreeNode.childNodes[i];}
var nd=findChildNodeByRowIndex(parentTreeNode.childNodes[i],r);if(nd){return nd;}}}
return null;}
function _findNodeByRowIndex(r){return findChildNodeByRowIndex(_div,r);}
function _onRowIndexChange(dataname){if(_jsTable){if(_jsTable.TableName==dataname){if(_div.selectedNode){if(_div.selectedNode.rowIndex==_jsTable.rowIndex){return;}}
if(_jsTable.rowIndex>=0){var nd=_findNodeByRowIndex(_jsTable.rowIndex);_selectNode(nd);}}}}
_div.oncellvaluechange=function(name,r,c,value){if(_jsTable.TableName==name){if(c==nodenameIdx||c==textIdx||c==imageIdx||c==nodedataIdx){_onRowIndexChange(name);if(_div.selectedNode){if(_div.selectedNode.rowIndex==r){if(c==imageIdx)
_div.selectedNode.jsData.setIconImage(value);else if(c==textIdx)
_div.selectedNode.jsData.setNodeText(value);else if(c==nodenameIdx)
_div.selectedNode.name=value;else if(c==nodedataIdx)
_div.selectedNode.nodedata=value;}}}}}
function _hideLoadingMessage(){if(_loadingMsg){_loadingMsg.style.display='none';}}
function _showLoadingMessage(node){var pos=getElementAbsolutePos(node);if(!_loadingMsg){_loadingMsg=document.createElement("div");document.body.appendChild(_loadingMsg);_loadingMsg.innerHTML='Loading';_loadingMsg.style.zIndex=100;_loadingMsg.style.position='absolute';_loadingMsg.style.color='red';_loadingMsg.style.backgroundColor='white';}
_loadingMsg.style.left=pos.x+'px';_loadingMsg.style.top=pos.y+'px';_loadingMsg.style.display='block';}
function _onDataReady(dataTable){var jsdb=_div.getAttribute('jsdb');if(jsdb){var binds=jsdb.split(':');if(binds.length>0){if(binds[0]==dataTable.TableName){_jsTable=dataTable;nodenameIdx=-1;textIdx=-1;imageIdx=-1;nodedataIdx=-1;if(binds.length>1&&binds[1].length>0){textIdx=dataTable.columnIndexes[binds[1]];}
if(binds.length>2&&binds[2].length>0){nodenameIdx=dataTable.columnIndexes[binds[2]];}
if(binds.length>3&&binds[3].length>0){imageIdx=dataTable.columnIndexes[binds[3]];}
if(binds.length>4&&binds[4].length>0){nodedataIdx=dataTable.columnIndexes[binds[4]];}
var r;var text;var image;var nd;var nodename;var nodedata;if(JsonDataBinding.getTableAttribute(dataTable.TableName,'isDataStreaming')){var parentnode=JsonDataBinding.getTableAttribute(dataTable.TableName,'tv_parentnode');for(r=dataTable.newRowStartIndex;r<dataTable.Rows.length;r++){nodename=null;if(textIdx>=0)
text=dataTable.Rows[r].ItemArray[textIdx];else
text='';if(nodenameIdx>=0){nodename=dataTable.Rows[r].ItemArray[nodenameIdx];}
if(imageIdx>=0){image=dataTable.Rows[r].ItemArray[imageIdx];}
else
image=null;if(nodedataIdx>=0){nodedata=dataTable.Rows[r].ItemArray[nodedataIdx];}
else
nodedata=null;nd=_addNode(parentnode,nodename,text,image,nodedata,r);}
if(_loadingMsg){_loadingMsg.style.display='none';}}
else{JsonDataBinding.addvaluechangehandler(_jsTable.TableName,_div);var nds=_getNodes(_div);for(var i=0;i<nds.length;i++){_div.removeChild(nds[i]);}
for(r=0;r<dataTable.Rows.length;r++){nodename=null;if(textIdx>=0)
text=dataTable.Rows[r].ItemArray[textIdx];else
text='';if(nodenameIdx>=0){nodename=dataTable.Rows[r].ItemArray[nodenameIdx];}
if(imageIdx>=0){image=dataTable.Rows[r].ItemArray[imageIdx];}
else
image=null;if(nodedataIdx>=0){nodedata=dataTable.Rows[r].ItemArray[nodedataIdx];}
else
nodedata=null;nd=_addNode(_div,nodename,text,image,nodedata,r);}}}}}}
_onAddNodeToTable=function(parentNode,nodename,text,image,nodedata){if(_jsTable){var r=JsonDataBinding.addRow(_jsTable.TableName);var row=_jsTable.Rows[r];if(textIdx>=0){_jsTable.Rows[r].ItemArray[textIdx]=text;}
if(nodenameIdx>=0){_jsTable.Rows[r].ItemArray[nodenameIdx]=nodename;}
if(imageIdx>=0){_jsTable.Rows[r].ItemArray[imageIdx]=image;}
if(nodedataIdx>=0){_jsTable.Rows[r].ItemArray[nodedataIdx]=nodedata;}
if(parentNode){var pkey=parentNode.jsData.getPrimaryKey();_jsTable.Rows[r].ItemArray[_jsTable.columnIndexes[_div.foreignKey]]=pkey;}
return r;}
return-1;}
function _getNodes(parentTreeNode){var nodes=new Array();var l=0;if(typeof parentTreeNode.treelevel!='undefined'){l=parentTreeNode.treelevel+1;}
for(var i=0;i<parentTreeNode.childNodes.length;i++){if(parentTreeNode.childNodes[i].treelevel==l){nodes.push(parentTreeNode.childNodes[i]);}}
return nodes;}
function _hasChild(){for(var i=0;i<_div.childNodes.length;i++){if(typeof _div.childNodes[i].treelevel!='undefined'){return true;}}
return false;}
function _addNodeSelectHandler(h){for(var i=0;i<_nodeSelectionHandlers.length;i++){if(_nodeSelectionHandlers[i]==h){return;}}
_nodeSelectionHandlers.push(h);}
function _removeNodeSelectHandler(h){for(var i=0;i<_nodeSelectionHandlers.length;i++){if(_nodeSelectionHandlers[i]==h){_nodeSelectionHandlers.splice(i,1);return;}}}
function _afterSelectNode(){for(var i=0;i<_nodeSelectionHandlers.length;i++){if(_nodeSelectionHandlers[i]){_nodeSelectionHandlers[i]();}}
if(_div.onnodeselected){_div.onnodeselected();}}
function _addChildNode(parentTreeNode,nodename,text,image,nodedata){if(parentTreeNode){if(parentTreeNode.jsData.canAddChildNode()){var nd=_addNode(parentTreeNode,nodename,text,image,nodedata);var rIdx=_onAddNodeToTable(parentTreeNode,nodename,text,image,nodedata);if(rIdx>=0){nd.rowIndex=rIdx;_jsTable.Rows[rIdx].rowVersion=1;nd.jsData.preventnextLevel();JsonDataBinding.dataMoveToRecord(_jsTable.TableName,rIdx);}
return nd;}}
return null;}
function _deleteSelectedNode(){var selNode=_div.selectedNode;if(selNode){var rIdx=typeof selNode.rowIndex=='undefined'?-1:selNode.rowIndex;if(rIdx>=0){var canRemove=false;if(selNode.jsData.nextLevelLoaded()){var pkey=selNode.jsData.getPrimaryKey();if(typeof pkey!='undefined'&&pkey!=null){if(!selNode.jsData.hasChild()){canRemove=true;}}}
if(canRemove){if(_jsTable.rowIndex!=rIdx){JsonDataBinding.dataMoveToRecord(_jsTable.TableName,rIdx);}
if(_jsTable.rowIndex==rIdx){JsonDataBinding.deleteCurrentRow(_jsTable.TableName);selNode.style.display='none';return true;}}}
else{var pe=selNode.parentNode;pe.removeChild(selNode);return true;}}
return false;}
function _addNode(parentTreeNode,nodename,text,image,nodedata,rIdx){var _divNode;var _tblHolder;var _imgNodeState;var _imgNodeIcon;var _spText;var _tree;var _tdPad;var _tdPad2;var _nextLevelLoaded=false;var STATE_COLLAPSED=0;var STATE_EXPANDED=1;var _state=STATE_COLLAPSED;var POS_TOP=0;var POS_LAST=1;var POS_MIDDLE=2;var POS_TOPSINGLE=3;function _hasChild(){for(var i=0;i<_divNode.childNodes.length;i++){if(typeof _divNode.childNodes[i].treelevel!='undefined'){return true;}}
return false;}
function isFirst(){if(_divNode.previousSibling){if(typeof _divNode.previousSibling.treelevel!='undefined'){return false;}}
return true;}
function _isLast(){if(_divNode.nextSibling){if(typeof _divNode.nextSibling.treelevel!='undefined'){return false;}}
return true;}
function _showStateImage(){var pos=POS_TOP;var isFirstNode=isFirst();var isLastNode=_isLast();if(isLastNode){_tdPad2.style.backgroundImage="url('./treeview/w20.png')";}
else{_tdPad2.style.backgroundImage="url('./treeview/h1.png')";}
if(isFirstNode&&_divNode.treelevel==0){_tdPad.style.backgroundImage="url('./treeview/w20.png')";}
else{_tdPad.style.backgroundImage="url('./treeview/h1.png')";}
if(_divNode.treelevel==0&&isFirstNode){if(isLastNode){pos=POS_TOPSINGLE;}
else{pos=POS_TOP;}}
else{if(isLastNode){pos=POS_LAST;}
else{pos=POS_MIDDLE;}}
var haschildren=_hasChild();if(_state==STATE_COLLAPSED){if(!_nextLevelLoaded||haschildren){switch(pos){case POS_TOPSINGLE:_imgNodeState.src="./treeview/plus_0.png";break;case POS_TOP:_imgNodeState.src="./treeview/plus_dn.png";break;case POS_MIDDLE:_imgNodeState.src="./treeview/plus_up_dn.png";break;case POS_LAST:_imgNodeState.src="./treeview/plus_up.png";break;}}
else{switch(pos){case POS_TOPSINGLE:_imgNodeState.src="./treeview/empty_0.png";break;case POS_TOP:_imgNodeState.src="./treeview/empty_dn.png";break;case POS_MIDDLE:_imgNodeState.src="./treeview/empty_up_dn.png";break;case POS_LAST:_imgNodeState.src="./treeview/empty_up.png";break;}}}
else{if(haschildren||!_nextLevelLoaded){switch(pos){case POS_TOPSINGLE:_imgNodeState.src="./treeview/minus_0.png";break;case POS_TOP:_imgNodeState.src="./treeview/minus_dn.png";break;case POS_MIDDLE:_imgNodeState.src="./treeview/minus_up_dn.png";break;case POS_LAST:_imgNodeState.src="./treeview/minus_up.png";break;}}
else{switch(pos){case POS_TOPSINGLE:_imgNodeState.src="./treeview/empty_0.png";break;case POS_TOP:_imgNodeState.src="./treeview/empty_dn.png";break;case POS_MIDDLE:_imgNodeState.src="./treeview/empty_up_dn.png";break;case POS_LAST:_imgNodeState.src="./treeview/empty_up.png";break;}}}}
function _showNode(){_divNode.style.display='block';if(_tblHolder.offsetHeight>_imgNodeState.offsetHeight+2){var ph=Math.floor((_tblHolder.offsetHeight-_imgNodeState.offsetHeight)/2);if(ph>1){if(_isLast()){_tdPad2.style.backgroundImage="url('./treeview/w20.png')";}
else{_tdPad2.style.backgroundImage="url('./treeview/h1.png')";}
_tdPad.style.height=ph+"px";_tdPad2.style.height=ph+"px";}}}
function toggleExpand(){if(_state==STATE_COLLAPSED){if(!_nextLevelLoaded){_nextLevelLoaded=true;if(_tree.nodesloader){_showLoadingMessage(_divNode);_tree.jsData.setCurrentPrimaryKey(_divNode);_tree.nodesloader();}}
_state=STATE_EXPANDED;}
else
_state=STATE_COLLAPSED;var i;if(_state==STATE_COLLAPSED){for(i=0;i<_divNode.childNodes.length;i++){if(typeof _divNode.childNodes[i].treelevel!='undefined'){_divNode.childNodes[i].style.display='none';}}}
else{for(i=0;i<_divNode.childNodes.length;i++){if(typeof _divNode.childNodes[i].treelevel!='undefined'){_divNode.childNodes[i].jsData.showNode();}}}
_showStateImage();}
function onNodemouseover(){_divNode.style.backgroundColor=_tree.mouseOverColor;}
function onNodeMouseOut(){if(_divNode.isSelected){_divNode.style.backgroundColor=_tree.selectedNodeColor;}
else{if(typeof _divNode.backColor=='undefined'){_divNode.style.backgroundColor=_tree.nodeBackColor;}
else{_divNode.style.backgroundColor=_divNode.backColor;}}}
function onNodeMouseClick(){_selectNode(_divNode);if(_divNode.onnodeclick){_divNode.onnodeclick();}}
function onNodemousedown(e){}
function _getTreeView(){var p=_divNode.parentNode;if(p)
return p.jsData.getTreeView();}
function _getState(){return _state;}
function _addChildNode(text,image,nodedata){_addNode(_divNode,text,image,nodedata);}
function _setNodeText(text){JsonDataBinding.SetInnerText(_spText,text);}
function _getNodeText(){return JsonDataBinding.GetInnerText(_spText);}
function _setNodeHtmlText(html){_spText.innerHTML=html;}
function _getNodeHtmlText(){return _spText.innerHTML;}
function _setIconImage(imagePath){if(imagePath&&imagePath!=null&&imagePath!='null'){if(imagePath.length>0){_imgNodeIcon.src=imagePath;_imgNodeIcon.style.display="inline";_showNode();return;}}
_imgNodeIcon.src=null;_imgNodeIcon.style.display="none";_showNode();}
function _getIconImage(){return _imgNodeIcon.src;}
function _getFontFamily(){return _spText.style.fontFamily;}
function _setFontFamily(fontFamily){_spText.style.fontFamily=fontFamily;}
function _getFontSize(){return _spText.style.fontSize;}
function _setFontSize(fontSize){_spText.style.fontSize=fontSize+"px";}
function _getFontColor(){return _spText.style.color;}
function _setFontColor(fontColor){_spText.style.color=fontColor;}
function _getBackColor(){return _divNode.backColor;}
function _setBackColor(bkColor){_divNode.backColor=bkColor;if(typeof bkColor=='undefined'||bkColor==null)
_divNode.style.backgroundColor=_tree.nodeBackColor;else
_divNode.style.backgroundColor=bkColor;}
function _getPrimaryKey(){if(_jsTable&&typeof _divNode.rowIndex!='undefined'){if(!_jsTable.Rows[_divNode.rowIndex].added){return _jsTable.Rows[_divNode.rowIndex].ItemArray[_jsTable.columnIndexes[_div.primaryKey]];}}
return null;}
_tree=parentTreeNode.jsData.getTreeView();_divNode=document.createElement("div");_divNode.style.verticalAlign='top';_divNode.nodedata=nodedata;_divNode.name=nodename;if(typeof parentTreeNode.treelevel=='undefined')
_divNode.treelevel=0;else
_divNode.treelevel=parentTreeNode.treelevel+1;_divNode.style.backgroundColor=_tree.nodeBackColor;_divNode.style.display='block';_divNode.style.padding=0;_imgNodeState=document.createElement("img");_imgNodeState.src="./treeview/plus_0.png";_imgNodeState.style.border=0;_imgNodeState.style.display='inline';_imgNodeState.style.cursor='pointer';JsonDataBinding.AttachEvent(_imgNodeState,'onclick',toggleExpand);_imgNodeIcon=document.createElement("img");_imgNodeIcon.style.border=0;if(image&&image.length>0){_imgNodeIcon.src=image;_imgNodeIcon.style.display='inline';}
else{_imgNodeIcon.style.display='none';}
JsonDataBinding.AttachEvent(_imgNodeIcon,'onmouseover',onNodemouseover);JsonDataBinding.AttachEvent(_imgNodeIcon,'onmouseout',onNodeMouseOut);JsonDataBinding.AttachEvent(_imgNodeIcon,'onclick',onNodeMouseClick);JsonDataBinding.AttachEvent(_imgNodeIcon,'onmousedown',onNodemousedown);_spText=document.createElement("span");_spText.innerHTML=text;if(typeof _tree.noteFontFamily!='undefined'){_spText.style.fontFamily=_tree.noteFontFamily;}
if(typeof _tree.noteFontSize!='undefined'){_spText.style.fontSize=_tree.noteFontSize;}
if(typeof _tree.noteFontColor!='undefined'){_spText.style.color=_tree.noteFontColor;}
_spText.style.display='inline';JsonDataBinding.AttachEvent(_spText,'onmouseover',onNodemouseover);JsonDataBinding.AttachEvent(_spText,'onmouseout',onNodeMouseOut);JsonDataBinding.AttachEvent(_spText,'onclick',onNodeMouseClick);JsonDataBinding.AttachEvent(_spText,'onmousedown',onNodemousedown);parentTreeNode.appendChild(_divNode);_divNode.style.margin='0px';_divNode.style.padding=0;_tblHolder=document.createElement('table');_divNode.appendChild(_tblHolder);_tblHolder.border=0;_tblHolder.setAttribute("border","0");_tblHolder.style.margin=0;_tblHolder.style.padding=0;_tblHolder.cellPadding=0;_tblHolder.cellSpacing=0;var tbd=null;var tbds=_tblHolder.getElementsByTagName('tbody');if(tbds){if(tbds.length>0){tbd=tbds[0];}}
if(!tbd){tbd=document.createElement('tbody');_tblHolder.appendChild(tbd);}
var tr=document.createElement('tr');tbd.appendChild(tr);var tr2=document.createElement('tr');tbd.appendChild(tr2);var tr3=document.createElement('tr');tbd.appendChild(tr3);tr.setAttribute("valign",'center');tr2.setAttribute('valign','center');var parents={};var p=parentTreeNode;while(p){if(p.jsData){if(typeof p.treelevel=='undefined'){break;}
else{parents['t'+(p.treelevel)]=p;p=p.parentNode;}}
else{break;}}
for(var i=0;i<_divNode.treelevel;i++){var td=document.createElement('td');tr.appendChild(td);td.rowSpan=3;p=parents['t'+i];if(i>0&&p.jsData.isLastNode()){td.style.backgroundImage="url('./treeview/w20.png')";}
else{td.style.backgroundImage="url('./treeview/vl.png')";}
td.style.width="20px";}
_tdPad=document.createElement('td');tr.appendChild(_tdPad);if(_divNode.treelevel==0&&isFirst()){_tdPad.style.backgroundImage="url('./treeview/w20.png')";}
else{_tdPad.style.backgroundImage="url('./treeview/h1.png')";}
td=document.createElement('td');tr2.appendChild(td);td.appendChild(_imgNodeState);td.style.height="20px";_tdPad2=document.createElement('td');tr3.appendChild(_tdPad2);if(_isLast()){_tdPad2.style.backgroundImage="url('./treeview/w20.png')";}
else{_tdPad2.style.backgroundImage="url('./treeview/h1.png')";}
td=document.createElement('td');tr.appendChild(td);td.rowSpan=3;var lbl=document.createElement('label');td.appendChild(lbl);lbl.appendChild(_imgNodeIcon);lbl.appendChild(_spText);if(_divNode.previousSibling&&_divNode.previousSibling.jsData){_divNode.previousSibling.jsData.showStateImage();}
_showStateImage();if(_divNode.treelevel>0){if(parentTreeNode.jsData.getState()==STATE_COLLAPSED){_divNode.style.display='none';}
else{_showNode();}
parentTreeNode.jsData.showStateImage();}
else{_showNode();}
_divNode.jsData={clear:function(){var i;var roots=new Array();for(i=0;i<_divNode.childNodes.length;i++){if(typeof _divNode.childNodes[i].treelevel!='undefined'){roots.push(_divNode.childNodes[i]);}}
for(i=0;i<roots.length;i++){_divNode.removeChild(roots[i]);}},getState:function(){return _getState();},addChildNode:function(text,image,nodedata){return _addChildNode(text,image,nodedata);},getTreeView:function(){return _getTreeView();},hasChild:function(){return _hasChild();},showStateImage:function(){_showStateImage();},showNode:function(){_showNode();},isLastNode:function(){return _isLast();},setNodeData:function(data){_setNodeData(data);},getNodeData:function(){return _getNodeData();},setNodeText:function(text){_setNodeText(text);},getNodeText:function(){return _getNodeText();},setNodeHtmlText:function(html){return _setNodeHtmlText(html);},getNodeHtmlText:function(){return _getNodeHtmlText();},setIconImage:function(imagePath){_setIconImage(imagePath);},getIconImage:function(){return _getIconImage();},getFontFamily:function(){return _getFontFamily();},setFontFamily:function(fontFamily){_setFontFamily(fontFamily);},getFontSize:function(){return _getFontSize();},setFontSize:function(fontSize){_setFontSize(fontSize);},getFontColor:function(){return _getFontColor();},setFontColor:function(fontColor){_setFontColor(fontColor);},getBackColor:function(){return _getBackColor();},setBackColor:function(bkColor){_setBackColor(bkColor);},preventnextLevel:function(){_nextLevelLoaded=true;},nextLevelLoaded:function(){return _nextLevelLoaded;},getPrimaryKey:function(){return _getPrimaryKey();},canAddChildNode:function(){if(_jsTable){var pkey=_getPrimaryKey();return(pkey!=null);}
return true;}};if(typeof rIdx!='undefined'){_divNode.rowIndex=rIdx;}
if(_tree.onnodecreated){_tree.onnodecreated(null,_tree,_divNode);}
return _divNode;}
function _getReadOnly(){return _readOnly;}
function _setReadOnly(readOnly){_readOnly=readOnly;if(_readOnly){}}
function _clear(){var i;var roots=new Array();for(i=0;i<_div.childNodes.length;i++){if(typeof _div.childNodes[i].treelevel!='undefined'){roots.push(_div.childNodes[i]);}}
for(i=0;i<roots.length;i++){_div.removeChild(roots[i]);}}
function _refreshBindColumnDisplay(name,rowidx,colIdx){if(_jsTable&&_jsTable.TableName==name){if(colIdx==imageIdx||colIdx==textIdx||colIdx==nodenameIdx||colIdx==nodedataIdx){var node=_findNodeByRowIndex(rowidx);if(node&&node.jsData){if(colIdx==imageIdx){node.jsData.setIconImage(_jsTable.Rows[rowidx].ItemArray[colIdx]);}
else if(colIdx==textIdx){node.jsData.setNodeText(_jsTable.Rows[rowidx].ItemArray[colIdx]);}
else if(colIdx==nodenameIdx){node.name=_jsTable.Rows[rowidx].ItemArray[colIdx];}
else if(colIdx==nodedataIdx){node.jsData.setNodeData(_jsTable.Rows[rowidx].ItemArray[colIdx]);}}}}}
function _selectFirstNode(){for(var i=0;i<_div.childNodes.length;i++){if(typeof _div.childNodes[i].treelevel!='undefined'){_selectNode(_div.childNodes[i]);return true;}}}
htmlElement.jsData={clear:function(){_clear();},addRootNode:function(nodename,text,image,nodedata){return _addChildNode(_div,nodename,text,image,nodedata);},addChildNodeToSelectedNode:function(nodename,text,image,nodedata){return _addChildNode(_div.selectedNode,nodename,text,image,nodedata);},addChildNode:function(parentTreeNode,nodename,text,image,nodedata){return _addChildNode(parentTreeNode,nodename,text,image,nodedata);},deleteSelectedNode:function(){return _deleteSelectedNode();},getNodes:function(){return _getNodes(_div);},getTreeView:function(){return _div;},hasChild:function(){return _hasChild();},addNodeSelectHandler:function(h){_addNodeSelectHandler(h);},removeNodeSelectHandler:function(h){_removeNodeSelectHandler(h);},afterSelectNode:function(){_afterSelectNode();},setReadOnly:function(readOnly){_setReadOnly(readOnly);},getReadOnly:function(){return _getReadOnly();},onDataReady:function(dataTable){_onDataReady(dataTable);},hideLoadingMessage:function(){_hideLoadingMessage();},showLoadingMessage:function(node){_showLoadingMessage(node);},setCurrentPrimaryKey:function(parentNode){_setCurrentPrimaryKey(parentNode);},onRowIndexChange:function(dataname){_onRowIndexChange(dataname);},getPrimaryKey:function(){return null;},canAddChildNode:function(){return true;},refreshBindColumnDisplay:function(name,rowidx,colIdx){_refreshBindColumnDisplay(name,rowidx,colIdx);},selectFirstNode:function(){_selectFirstNode();}};return htmlElement.jsData;}
JsonDataBinding.slideshow={oninitpage:function(e){if(e){JsonDataBinding.slideshow.createSlideShow(e);}
else{var divs=document.getElementsByTagName('div');if(divs&&divs.length>0){for(var i=0;i<divs.length;i++){var s=divs[i].getAttribute('typename');if(s=='slideshow'){JsonDataBinding.slideshow.createSlideShow(divs[i]);}}}}},oncleanuppage:function(){var ret=false;var divs=document.getElementsByTagName('div');if(divs&&divs.length>0){for(var i=0;i<divs.length;i++){var s=divs[i].getAttribute('typename');if(s=='slideshow'){if(!divs[i].jsData){JsonDataBinding.slideshow.createSlideShow(divs[i]);}
if(divs[i].jsData){divs[i].jsData.saveSlides();}
ret=true;}}}
return ret;},createSlideShow:function(divElement){if(!divElement.jsData&&!divElement.loadingSS){divElement.loadingSS=true;divElement.contentEditable=false;var _dataId=divElement.getAttribute('scriptData');var _index;var _slides;var _imgBack;var _imgNext;var _spanTitle;var _divPos;var _wpercent=0;var _hpercent=0;function _onmouseover(e){var img=JsonDataBinding.getSender(e);if(img){JsonDataBinding.setOpacity(img,100);}}
function _onmouseout(e){var img=JsonDataBinding.getSender(e);if(img){JsonDataBinding.setOpacity(img,80);}}
function _moveBack(){_index--;if(_index<0){if(_slides){_index=_slides.length-1;}}
_showSlide();}
function _moveNext(){if(_slides){_index++;if(_index>=_slides.length){_index=0;}}
_showSlide();}
function _windowResize(){if(_wpercent>0||_hpercent>0){var p=divElement.parentNode;if(p){if(_wpercent>0){var w=p.offsetWidth*(_wpercent/100.0);divElement.style.width=(w-2)+'px';}
else if(_hpercent>0){var h=p.offsetHeight*(_hpercent/100.0);divElement.style.height=(h-2)+'px';}}}}
divElement.innerHTML='';divElement.typename='slideshow';divElement.setAttribute('typename','slideshow');divElement.style.verticalAlign='middle';divElement.style.display='table-cell';divElement.style.textAlign='center';divElement.style.backgroundPosition='center';divElement.style.backgroundRepeat='no-repeat';divElement.style.backgroundSize='100%';_imgBack=document.createElement('img');_imgBack.src='/libjs/tr_arr_left.png';_imgBack.style.cursor='pointer';_imgBack.style.cssFloat='left';_imgBack.style.boxShadow='5px 5px 2px #222222';_imgBack.contentEditable=false;JsonDataBinding.setOpacity(_imgBack,80);divElement.appendChild(_imgBack);JsonDataBinding.AttachEvent(_imgBack,'onmouseover',_onmouseover);JsonDataBinding.AttachEvent(_imgBack,'onmouseout',_onmouseout);JsonDataBinding.AttachEvent(_imgBack,'onclick',_moveBack);_imgNext=document.createElement('img');_imgNext.src='/libjs/tr_arr_right.png';_imgNext.style.cursor='pointer';_imgNext.style.cssFloat='right';_imgNext.style.boxShadow='5px 5px 2px #222222';_imgNext.contentEditable=false;JsonDataBinding.setOpacity(_imgNext,80);divElement.appendChild(_imgNext);JsonDataBinding.AttachEvent(_imgNext,'onmouseover',_onmouseover);JsonDataBinding.AttachEvent(_imgNext,'onmouseout',_onmouseout);JsonDataBinding.AttachEvent(_imgNext,'onclick',_moveNext);JsonDataBinding.AttachEvent(window,'onresize',_windowResize);_divPos=JsonDataBinding.ElementPosition.getElementPosition(divElement);_spanTitle=document.createElement('span');_spanTitle.contentEditable=false;_spanTitle.style.position='absolute';_spanTitle.style.top=(_divPos.y+20)+'px';_spanTitle.style.left=(_divPos.x+(divElement.offsetWidth-_spanTitle.offsetWidth)/2)+'px';_spanTitle.innerHTML='This is a sub title';divElement.appendChild(_spanTitle);_index=-1;_slides=[];function _showSlide(){if(_index>=0&&_index<_slides.length){_divPos=JsonDataBinding.ElementPosition.getElementPosition(divElement);divElement.style.backgroundImage='url('+_slides[_index].img+')';_spanTitle.innerHTML=_slides[_index].txt;_spanTitle.style.top=(_divPos.y+20)+'px';_spanTitle.style.left=(_divPos.x+(divElement.offsetWidth-_spanTitle.offsetWidth)/2)+'px';}}
if(_dataId){if(typeof(limnorData)!='undefined'&&typeof(limnorData[_dataId])!='undefined'){_slides=limnorData[_dataId];if(_slides&&_slides.length>0){_index=0;_showSlide();}
else
_slides=[];}}
function _saveSlides(){if(!_dataId){_dataId=limnorHtmlEditorClient.createDataId('ss',true);}
divElement.setAttribute('scriptData',_dataId);limnorHtmlEditorClient.addScriptData(_dataId,_getSlides(),'div');}
function _getSlides(){if(_slides){var ss=[];for(var i=0;i<_slides.length;i++){if(!_slides[i].img)
_slides[i].img='';else
_slides[i].img=_slides[i].img.trim();if(_slides[i].img.length>0){ss.push(_slides[i]);}}
_slides=ss;}
else{_slides=[];}
return _slides;}
function _getSlide(idx){if(idx>=0){if(!_slides){_slides=[];}
while(_slides.length<=idx){_slides.push({img:'',txt:''});}
return _slides[idx];}}
function _onsizechanged(){var w=divElement.getAttribute('w');if(typeof(w)=='undefined'||w==null||w.length==0){_wpercent=0;}
else{if(w.charAt(w.length-1)=='%'){w=w.substr(0,w.length-1);if(isNaN(w)){_wpercent=0;}
else{_wpercent=parseFloat(w);}}}
var h=divElement.getAttribute('h');if(typeof(h)=='undefined'||h==null||h.length==0){_hpercent=0;}
else{if(h.charAt(h.length-1)=='%'){h=h.substr(0,h.length-1);if(isNaN(h)){_hpercent=0;}
else{_hpercent=parseFloat(h);}}}
_windowResize();}
divElement.jsData={moveBack:function(){_moveBack();},moveNext:function(){_moveNext();},getSlides:function(){return _getSlides();},getSlide:function(idx){return _getSlide(idx);},saveSlides:function(){_saveSlides();},createSubEditor:function(o,c){return o;},onsizechanged:function(){_onsizechanged();},showSlide:function(idx){if(typeof(idx)=='undefined'){_index=0;}
else
_index=idx;_showSlide();}};}
_onsizechanged();return divElement.jsData;}};JsonDataBinding.limnorsvg={oninitpage:function(e){if(e){JsonDataBinding.limnorsvg.createSVG(e);}
else{var svgs=document.getElementsByTagName('svg');if(svgs&&svgs.length>0){for(var i=0;i<svgs.length;i++){JsonDataBinding.limnorsvg.createSVG(svgs[i]);}}}},oncleanuppage:function(){var ret=false;var svgs=document.getElementsByTagName('svg');if(svgs&&svgs.length>0){for(var i=0;i<svgs.length;i++){if(!svgs[i].jsData){JsonDataBinding.limnorsvg.createSVG(svgs[i]);}
ret=true;}}
return ret;},designInit:function(pageEditor,calledClient){var svgs=document.getElementsByTagName('svg');if(svgs&&svgs.length>0){for(var i=0;i<svgs.length;i++){if(!svgs[i].jsData){JsonDataBinding.limnorsvg.createSVG(svgs[i]);}
svgs[i].jsData.initEdit();}}},createSVG:function(svg0){if(!svg0.jsData&&!svg0.loadingSS){svg0.loadingSS=true;var svg=svg0;var propx,propy;var HtmlEditor;function isparent(p,c){if(p&&c){var a=c;while(a&&a==document.body){if(a==p){return true;}
a=a.parentNode;}}}
function _showElements(s,v,opname,fillname){if(s){var op=s.getAttribute(opname);if(op){s.style.opacity=op/100.0;}
var color=s.getAttribute(fillname);if(color){s.style.fill=color;}
if(!JsonDataBinding.inediting){var idstrs=s.getAttribute('showelements');if(idstrs){var ids=idstrs.split(',');if(ids&&ids.length>0){for(var i=0;i<ids.length;i++){if(ids[i]&&ids[i].length>0){var id=ids[i].trim();if(id.length>0){var c=document.getElementById(id);if(c){c.style.display=v;}}}}}}}}}
function _hideElement(o){_showElements(o,'none','mouseoutopacity','mouseoutfillcolor');}
function _onmouseover(e){var s=JsonDataBinding.getSender(e);if(s){_showElements(s,'block','mouseoveropacity','mouseoverfillcolor');var svgid;var tag=s.tagName?s.tagName.toLowerCase():'';if(tag!='svg'){var p=s.parentNode;while(p){tag=p.tagName?p.tagName.toLowerCase():'';if(tag=='svg'){svgid=p.id;break;}
else if(tag=='body'){break;}
p=p.parentNode;}}
if(document.cursvgs&&document.cursvgs.length>0){var n=-1;for(var i=document.cursvgs.length;i>=0;i--){if(_isinactivates(document.cursvgs[i],s.id,svgid)){n=i;break;}
else{_hideElement(document.cursvgs[i]);}}
if(n==-1){document.cursvgs=[];}
else if(n<document.cursvgs.length-1){var aa=[];for(var i=0;i<=n;i++){aa.push(document.cursvgs[i]);}
document.cursvgs=aa;}}
else{document.cursvgs=[];}
document.cursvgs.push(s);}}
function _onmouseout(e){var s=JsonDataBinding.getSender(e);if(s.getAttribute('automouseouteffects')){_hideElement(s);}}
function _attachShape(o){JsonDataBinding.AttachEvent(o,'onmouseover',_onmouseover);JsonDataBinding.AttachEvent(o,'onmouseout',_onmouseout);_hideElement(o);}
function _initEdit(){}
function isShapeTag(tag){return(tag=='polygon'||tag=='rect'||tag=='circle'||tag=='ellipse'||tag=='line'||tag=='polyline'||tag=='text');}
function _isinactivates(s,sid,svgid){if(s){var idstrs=s.getAttribute('showelements');if(idstrs){var ids=idstrs.split(',');if(ids&&ids.length>0){for(var i=0;i<ids.length;i++){if(ids[i]&&ids[i].length>0){var id=ids[i].trim();if(id.length>0){if(id==sid||id==svgid){return true;}}}}}}}}
function _ondocclick(){if(svg.childNodes){for(var i=0;i<svg.childNodes.length;i++){var tag=svg.childNodes[i].tagName?svg.childNodes[i].tagName.toLowerCase():'';if(tag=='a'){svg.childNodes[i].hideParent=true;for(var j=0;j<svg.childNodes[i].childNodes.length;j++){tag=svg.childNodes[i].childNodes[j].tagName?svg.childNodes[i].childNodes[j].tagName.toLowerCase():'';if(isShapeTag(tag)){_hideElement(svg.childNodes[i].childNodes[j]);}}}
else{if(isShapeTag(tag)){_hideElement(svg.childNodes[i]);}}}}}
JsonDataBinding.AttachEvent(document.body,'onclick',_ondocclick);if(svg.childNodes){for(var i=0;i<svg.childNodes.length;i++){var tag=svg.childNodes[i].tagName?svg.childNodes[i].tagName.toLowerCase():'';if(tag=='a'){svg.childNodes[i].hideParent=true;for(var j=0;j<svg.childNodes[i].childNodes.length;j++){tag=svg.childNodes[i].childNodes[j].tagName?svg.childNodes[i].childNodes[j].tagName.toLowerCase():'';if(isShapeTag(tag)){_attachShape(svg.childNodes[i].childNodes[j]);}}}
else{if(isShapeTag(tag)){_attachShape(svg.childNodes[i]);}}}}
function _hideInactiveElements(){}
function getValue(name){var x=parseInt(svg.getAttribute(name));if(isNaN(x)){x=1;}
return x;}
function _moveleft(){var x=getValue('x')-HtmlEditor.svgshapemovegap;svg.setAttribute('x',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveright(){var x=getValue('x')+HtmlEditor.svgshapemovegap;svg.setAttribute('x',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveup(){var x=getValue('y')-HtmlEditor.svgshapemovegap;svg.setAttribute('y',x);if(propy&&propy.settext){propy.settext(x);}}
function _movedown(){var x=getValue('y')+HtmlEditor.svgshapemovegap;svg.setAttribute('y',x);if(propy&&propy.settext){propy.settext(x);}}
svg.jsData={attachShape:function(o){_attachShape(o);},moveleft:function(){_moveleft();},moveright:function(){_moveright();},moveup:function(){_moveup();},movedown:function(){_movedown();},setProps:function(ps,e){propx=ps[0];propy=ps[1];HtmlEditor=e;},hideInactiveElements:function(){_hideInactiveElements();},initEdit:function(){_initEdit();}};}
return svg0.jsData;}}
var HtmlEditor=HtmlEditor||{version:'Visual HTML Editor (Version 1.1)',hitcountid:'hc890326',svgshapemovegap:10,borderStyle:['none','hidden','dotted','dashed','solid','double','groove','ridge','inset','outset','inherit'],widthStyle:['','thin','medium','thick','inherit'],textAlign:['','left','right','center','justify','inherit'],overflow:['','visible','hidden','scroll','auto','no-display','no-content'],cssFloat:['','left','right','none','inherit'],cursorValues:['','auto','crosshair','default','e-resize','help','move','n-resize','ne-resize','nw-resize','pointer','s-resize','se-resize','sw-resize','text','w-resize','wait'],verticalAlign:['','length in px','%','baseline','sub','super','top','text-top','middle','bottom','text-bottom','inherit'],fontStyles:['','normal','italic','oblique','inherit'],fontVariants:['','normal','small-caps','inherit'],fontWeights:['','normal','bold','bolder','lighter','100','200','300','400','500','600','700','800','900','inherit'],blocks:',P,H1,H2,H3,H4,H5,H6,UL,OL,DIR,MENU,DL,PRE,DIV,CENTER,BLOCKQUOTE,IFRAME,NOSCRIPT,NOFRAMES,FORM,ISINDEX,HR,TABLE,ADDRESS,FIELDSET,NAV,',inlines1:',TT,I,B,U,S,STRIKE,BIG,SMALL,FONT,EM,STRONG,DFN,CODE,SAMP,KBD,VAR,CITE,ABBR,ACRONYM,SUB,SUP,Q,SPAN,BDO,',inlines2:',A,OBJECT,APPLET,IMG,BASEFONT,BR,SCRIPT,MAP,INPUT,SELECT,TEXTAREA,LABEL,BUTTON,EMBED,',CanBeChild:function(p,c){var p0=','+p.toUpperCase()+',';if(p0==',BODY,')return true;var c0=','+c.toUpperCase()+',';function isPInline(){return(HtmlEditor.inlines1.indexOf(p0)>=0||HtmlEditor.inlines2.indexOf(p0)>=0);}
function isCInline(){return(HtmlEditor.inlines1.indexOf(c0)>=0||HtmlEditor.inlines2.indexOf(c0)>=0);}
if(',P,H1,H2,H3,H4,H5,H6,'.indexOf(p0)>=0){if(isCInline())return true;}
else if(p0==',UL,'||p0==',OL,'){if(c0==',LI,')return true;}
else if(p0==',DL,'){if(c0==',DT,')return true;if(c0==',DD,')return true;}
else if(p0==',DT,'){if(isCInline())return true;}
else if(p0==',DD,DIV,CENTER,BLOCKQUOTE,IFRAME,NOSCRIPT,NOFRAMES,TH,TD,LI,'){if(isCInline())return true;if(HtmlEditor.blocks.indexOf(c0)>=0)return true;}
else if(p0==',PRE,'){if(',IMG,OBJECT,APPLET,BIG,SMALL,SUB,SUP,FONT,BASEFONT,'.indexOf(c0)>=0)return false;if(isCInline())return true;}
else if(p0==',FORM,'){if(c0==',FORM,')return false;if(isCInline())return true;if(HtmlEditor.blocks.indexOf(c0)>=0)return true;}
else if(p0==',TABLE,'){if(',CAPTION,COLGROUP,COL,THEAD,TBODY,TBODY,'.indexOf(c0)>=0)return true;}
else if(p0==',CAPTION,'){if(isCInline())return true;}
else if(p0==',COLGROUP,'){if(c0==',COL,')return true;}
else if(',THEAD,TBODY,TBODY,'.indexOf(p0)>=0){if(c0==',TR,')return true;}
else if(p0==',TR,'){if(c0==',TH,'||c0==',TD,')return true;}
else if(p0==',TH,'||p0==',TD,'){if(isCInline())return true;if(HtmlEditor.blocks.indexOf(c0)>=0)return true;}
else if(p0==',ADDRESS,'){if(c0==',P,')return true;if(isCInline())return true;}
else if(p0==',FIELDSET,'){if(c0==',LEGEND,')return true;if(isCInline())return true;if(HtmlEditor.blocks.indexOf(c0)>=0)return true;}
else if(p0==',LEGEND,'){if(isCInline())return true;}
else if(p0==',A,'){if(c0!=',A,'){if(isCInline()){return true;}}}
else if(HtmlEditor.inlines1.indexOf(p0)>=0){return isCInline();}
else if(p0=',OBJECT,'||p0==',APPLET,'){if(c0==',PARAM,')return true;if(isCInline())return true;if(HtmlEditor.blocks.indexOf(c0)>=0){return true;}}
else if(p0==',MAP,'){if(c0==',AREA,')return true;if(HtmlEditor.blocks.indexOf(c0)>=0)return true;}
else if(p0==',SELECT,'){if(c0==',OPTGROUP,')return true;if(c0==',OPTION,')return true;}
else if(p0==',OPTGROUP,'){if(c0==',OPTION,')return true;}
else if(p0==',BUTTON,'){if(',A,INPUT,SELECT,TEXTAREA,LABEL,BUTTON,FORM,ISINDEX,FIELDSET,IFRAME,'.indexOf(c0)<0){return true;}}
return false;},getParent:function(p0,tag){while(p0&&p0.tagName){if(HtmlEditor.CanBeChild(p0.tagName,tag)){return p0;}
p0=p0.parentNode;}
return document.body;},removeQuoting:function(v){if(typeof v!='undefined'&&v&&v.length>0){if(v.substr(0,1)=="'"||v.substr(0,1)=='"'){v=v.substr(1);}
if(v.length>1){if(v.substr(v.length-1,1)=="'"||v.substr(v.length-1,1)=='"'){v=v.substr(0,v.length-1);}}}
return v;},PickWebFile:function(title,fileTypes,handler,disableUpload,subFolder,subName,maxSize){JsonDataBinding.setupChildManager();document.childManager.onChildWindowReady=function(){JsonDataBinding.executeByPageId(28310,'setData',title,fileTypes,disableUpload,subFolder,subName,maxSize);}
JsonDataBinding.AbortEvent=false;var vf4a10b76={};vf4a10b76.isDialog=true;vf4a10b76.url='WebFilePicker.html';vf4a10b76.center=true;vf4a10b76.top=0;vf4a10b76.left=0;vf4a10b76.width=475;vf4a10b76.height=500;vf4a10b76.resizable=true;vf4a10b76.border='0px';vf4a10b76.icon='/libjs/folder.gif';vf4a10b76.title=title?title:'Select web file';vf4a10b76.pageId=28310;vf4a10b76.onDialogClose=function(){if(JsonDataBinding.isValueTrue((document.dialogResult)!=('ok'))){JsonDataBinding.AbortEvent=true;return;}
handler(JsonDataBinding.getPropertyByPageId(28310,'propSelectedFile'));}
JsonDataBinding.showChild(vf4a10b76);},GetIECompatableWarning:function(){if(JsonDataBinding.IsIE()){return"Please turn off IE Compatibility View via Tools menu.";}
return'';},IsNameValid:function(name){if(name){var regexp=/^[a-zA-Z_$][0-9a-zA-Z_$]*$/;if(name.search(regexp)!=-1){return true;}}
return false;},getCssNameFromPropertyName:function(propName){if(propName&&propName.length>0){if(propName=='cssFloat')
return'float';return propName.replace(/\W+/g,'-').replace(/([a-z\d])([A-Z])/g,'$1-$2').toLowerCase();}},getPropertyNameFromCssName:function(cssName){if(cssName&&cssName.length>0){if(cssName=='float')
return'cssFloat';return cssName.replace(/\W+(.)/g,function(x,chr){return chr.toUpperCase();})}},createEditor:function(containerDiv){var _editorWindow=window;var _editorOptions;var _divProp;var _divPropertyGrid;var _divElementToolbar;var _titleTable;var _toolbarTable;var _editorDiv;var _tdTitle;var _toolbar;var _tdSelectedProperty;var _messageBar;var _tdParentList;var _tdObj;var _tdObjDelim;var _trObjDelim;var _editorTable;var _editorBody;var _parentList;var _propsBody;var _imgExpand;var _imgOK;var _imgCancel;var _imgSave;var _imgReset;var _imgShowProp;var _tableProps;var _divSelectElement;var _divLargeEnum;var _newElementSelectTableBackColor='#E0FFFF';var inputColorPicker;var _inputX;var imgNewElement;var _elementEditorList;var _addableElementList;var _commandList;var _resizer;var _nameresizer;var _isIE;var _ieVer;var _isFireFox;var _isOpera;var _isChrome;var _fontCommands;var _fontSelect;var _headingSelect;var _colorSelect;var _imgStopTag;var _imgInsSpace;var _imgInsBr;var _imgAppBr;var _imgAppTxt;var _imgUndo;var _imgRedo;var _imgUndel;var _addOptionWithNull;var _divPropLocation;var _spMsg;var _elementLocator;var _textInput;var _locatorInput;var _locatorList;var _imgLocatorElement;var _objectsInLocatorList;var _collectingResult;var _saveTryCount;var _divError;var _divToolTips;var _topElements;var _propertyTopElements;var _combos;var _custMouseDown;var _custMouseDownOwner;var _comboInput;var _autoSaveMinutes=10;var _webPath=JsonDataBinding.getWebSitePath();var _editorStart='<!--editorstart-->';var _editorEnd='<!--editorend-->';var _editorStartJs='//editorstart';var _editorEndJs='//editorend';var _editorscript='//limnorstudiohtmleditor';var fontList=['','cursive','monospace','serif','sans-serif','fantasy','Arial','Arial Black','Arial Narrow','Arial Rounded MT Bold','Bookman Old Style','Bradley Hand ITC','Century','Century Gothic','Comic Sans MS','Courier','Courier New','Georgia','Gentium','Impact','King','Lucida Console','Lalit','Modena','Monotype Corsiva','Papyrus','Tahoma','TeX','Times','Times New Roman','Trebuchet MS','Verdana','Verona'];var fontSizes=['','xx-small','x-small','small','medium','large','x-large','xx-large','smaller','larger','size in px, cm, ...','%','inherit'];JsonDataBinding.inediting=true;_ieVer=JsonDataBinding.getInternetExplorerVersion();_isIE=JsonDataBinding.IsIE();_isFireFox=JsonDataBinding.IsFireFox();_isOpera=JsonDataBinding.IsOpera();_isChrome=JsonDataBinding.IsChrome();function _insertBefore(p,c,r){_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow,[p,c,r]);}
function _insertAfter(p,c,r){_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow,[p,c,r?r.nextSibling:null]);}
function _createElement(tag){return _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,[tag]);}
function _createSVGElement(tag){return _editorOptions.client.createSVGElement.apply(_editorOptions.elementEditedWindow,[tag]);}
function _duplicateSvg(svg){var w=_editorOptions.client.duplicateSvg.apply(_editorOptions.elementEditedWindow,[svg]);selectEditElement(w);return w;}
function _bringSvgToFront(svg){_editorOptions.client.bringSvgToFront.apply(_editorOptions.elementEditedWindow,[svg]);}
function _getUploadIFrame(){return _editorOptions.client.getUploadIFrame.apply(_editorOptions.elementEditedWindow);}
function _appendChild(p,c){_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[p,c]);}
function _removeChild(p,c){_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow,[p,c]);}
function _getElementById(id){return _editorOptions.client.getElementById.apply(_editorOptions.elementEditedWindow,[id]);}
function _setElementStyleValue(obj,jsName,cssName,val){var setCss=(_editorOptions.isEditingBody&&!obj.subEditor);if(setCss){if(obj.getAttribute){var styleshare=obj.getAttribute('styleshare');if(typeof styleshare=='undefined'||styleshare==null||styleshare.toLowerCase()!='notshare'){}
else{setCss=false;}}}
if(setCss)
_editorOptions.client.setElementStyleValue.apply(_editorOptions.editorWindow,[obj,jsName,cssName,val]);else{var o=obj.subEditor?obj.obj:obj;try{o.style[jsName]=val;}
catch(err){}
if(typeof val=='undefined'||val==null||val==''){JsonDataBinding.removeStyleAttribute(obj,jsName,cssName);}}}
function addOptionToSelect(sel,op){if(_addOptionWithNull){if(_addOptionWithNull==1){sel.add(op,null);}
else{try{sel.add(op);}
catch(e){alert(e.message);}}}
else{try{sel.add(op,null);_addOptionWithNull=1;}
catch(e){sel.add(op);_addOptionWithNull=2;}}}
function createTreeViewItemPropDescs(o,li){function addSubItem(){var newLi=o.jsData.addItem(li);}
function resetState(){if(o.jsData.resetState){o.jsData.resetState();}}
function movetvitemup(){LI_moveup(li);resetState();}
function movetvitemdown(){LI_movedown(li);resetState();}
return{subEditor:true,obj:li,owner:o,allowCommands:true,getString:function(){return elementToString(li,'treeItem');},getStringForIDE:function(){var sRet='treeitem,,,';if(li.id&&li.id.length>0){sRet+=li.id;}
sRet+=',';var g=li.getAttribute('limnorId');if(g){sRet+=g;}
return sRet;},getProperties:function(){var props=new Array();props.push(idProp);for(i=0;i<_elementEditorList.length;i++){if(_elementEditorList[i].tagname=='li'){for(var k=0;k<_elementEditorList[i].properties.length;k++){if(_elementEditorList[i].properties[k].name=='background'){continue;}
if(_elementEditorList[i].properties[k].editor==EDIT_CUST){continue;}
props.push(_elementEditorList[i].properties[k]);}
break;}}
props.push({name:'delete',desc:'delete the tree item',editor:EDIT_DEL,action:function(){if(confirm('Do you want to remove this tree item and all its sub items?')){var newOwner=li.parentNode;o.jsData.delItem(li);selectEditElement(newOwner);if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}},notModify:true});props.push({name:'addSubItem',toolTips:'add a new sub tree item',editor:EDIT_CMD,IMG:'/libjs/newElement.png',action:addSubItem});props.push({name:'moveup',toolTips:'move tree item up',editor:EDIT_CMD,IMG:'/libjs/moveup.png',action:movetvitemup});props.push({name:'movedown',toolTips:'move tree item down',editor:EDIT_CMD,IMG:'/libjs/movedown.png',action:movetvitemdown});return{props:props};}}}
function _showResizer(){if(_divProp&&_divProp.expanded&&!containerDiv){if(_resizer){JsonDataBinding.windowTools.updateDimensions();var pos=JsonDataBinding.ElementPosition.getElementPosition(_divProp);_resizer.style.zIndex=JsonDataBinding.getZOrder(_divProp)+1;var szOff=22;_resizer.style.left=(pos.x+_divProp.offsetWidth-szOff)+'px';_resizer.style.top=(pos.y+_divProp.offsetHeight-szOff)+'px';_resizer.style.display='block';}}}
function isEditorObject(c){if(_editorOptions&&_editorOptions.isEditingBody){var c0=c;while(c0){if(c0.forProperties||c0==_elementLocator){return true;}
c0=c0.parentNode;}}
return false;}
function isMarker(c){if(c){if(_editorOptions){if(c==_editorOptions.redbox)
return true;if(_editorOptions.markers){for(var i=0;i<_editorOptions.markers.length;i++){if(_editorOptions.markers[i]==c){return true;}}}}}
return false;}
function isEditingObject(c){if(_editorOptions){var c0=c;while(c0){if(_editorOptions.isEditingBody){if(c0==_editorOptions.elementEdited||c0==_editorOptions.elementEditedDoc){return true;}}
else{if(c0==_editorOptions.elementEdited){return true;}}
c0=c0.parentNode;}}
return false;}
var UNDOSTATE_UNDO=0;var UNDOSTATE_REDO=1;var UNDOTYPE_KEYBOARD=1;function hasUndo(){if(_editorOptions){if(_editorOptions.undoState&&_editorOptions.undoList&&_editorOptions.undoList.length>0){if(_editorOptions.undoState.index>=0&&_editorOptions.undoState.index<_editorOptions.undoList.length){if(_editorOptions.undoState.index==0&&_editorOptions.undoState.state==UNDOSTATE_UNDO){return false;}
return true;}}}
return false;}
function hasRedo(){if(_editorOptions){if(_editorOptions.undoState&&_editorOptions.undoList&&_editorOptions.undoList.length>0){if(_editorOptions.undoState.index>=0&&_editorOptions.undoState.index<_editorOptions.undoList.length){if(_editorOptions.undoState.index==_editorOptions.undoList.length-1&&_editorOptions.undoState.state==UNDOSTATE_REDO){return false;}
return true;}}}
return false;}
function showUndoRedo(){if(hasUndo()){_imgUndo.src='/libjs/undo.png';}
else{_imgUndo.src='/libjs/undo_inact.png';}
if(hasRedo()){_imgRedo.src='/libjs/redo.png';}
else{_imgRedo.src='/libjs/redo_inact.png';}}
function adjustSizes(){if(_editorOptions){var propHeight=_divProp.offsetHeight-_titleTable.offsetHeight-_messageBar.offsetHeight-_toolbarTable.offsetHeight-20;if(propHeight>0){_editorDiv.style.height=propHeight+'px';var ph=propHeight-_divElementToolbar.offsetHeight;if(ph>28){_divPropertyGrid.style.height=(propHeight-28)+'px';if(!_editorOptions.isEditingBody&&containerDiv&&containerDiv.tdContentContainer){var divDlg;for(var i=0;i<containerDiv.tdContentContainer.children.length;i++){if(containerDiv.tdContentContainer.children[i].tagName.toLowerCase()=='div'){divDlg=containerDiv.tdContentContainer.children[i];break;}}
if(divDlg){divDlg.style.height=(ph-6)+'px';}}}
adjustObjectSize(propHeight);}}}
function captureSelection(e,forSelOnly){var selObj;var node;var obj=e.target;if(!obj){obj=JsonDataBinding.getSender(e);}
if(_isIE){if(obj&&obj.tagName&&obj.tagName.toLowerCase()=='html'){return;}}
if(isMarker(obj)){obj=null;}
_editorOptions.lastSelHtml=_editorOptions.selectedHtml;_editorOptions.selectedHtml=null;if(_editorOptions.selectedObject){selObj=_editorOptions.selectedObject.subEditor?_editorOptions.selectedObject.obj:_editorOptions.selectedObject;}
if(window.getSelection){_editorOptions.selection=_editorOptions.elementEditedWindow.getSelection();if(_editorOptions.selection.getRangeAt!==undefined){if(_editorOptions.selection.rangeCount>0){_editorOptions.range=_editorOptions.selection.getRangeAt(0);}}
else if(_editorOptions.elementEditedDoc.createRange&&_editorOptions.selection.anchorNode&&_editorOptions.selection.anchorOffset&&_editorOptions.selection.focusNode&&_editorOptions.selection.focusOffset){_editorOptions.range=_editorOptions.elementEditedDoc.createRange.apply(_editorOptions.elementEditedWindow);_editorOptions.range.setStart(_editorOptions.selection.anchorNode,_editorOptions.selection.anchorOffset);_editorOptions.range.setEnd(_editorOptions.selection.focusNode,_editorOptions.selection.focusOffset);}
else{_editorOptions.range=null;}
if(_editorOptions.range){if(_editorOptions.range.cloneContents){_editorOptions.selectedHtml=_editorOptions.range.cloneContents().textContent;if(forSelOnly)return;}
if(_editorOptions.range.endContainer){if(_editorOptions.range.endContainer.nodeType==1){node=_editorOptions.range.endContainer;}
else{if(_editorOptions.range.endContainer.parentElement&&_editorOptions.range.endContainer.parentElement.tagName){node=_editorOptions.range.endContainer.parentElement;}
else if(_editorOptions.range.endContainer.parentNode&&_editorOptions.range.endContainer.parentNode.tagName){node=_editorOptions.range.endContainer.parentNode;}}}
else if(_editorOptions.range.startContainer){if(_editorOptions.range.startContainer.nodeType==1){node=_editorOptions.range.startContainer;}
else{if(_editorOptions.range.startContainer.parentElement&&_editorOptions.range.startContainer.parentElement.tagName){node=_editorOptions.range.startContainer.parentElement;}
else if(_editorOptions.range.startContainer.parentNode&&_editorOptions.range.startContainer.parentNode.tagName){node=_editorOptions.range.startContainer.parentNode;}}}
else if(_editorOptions.range.commonAncestorContainer){if(_editorOptions.range.commonAncestorContainer.nodeType==1){node=_editorOptions.range.commonAncestorContainer;}
else{if(_editorOptions.range.commonAncestorContainer.parentElement&&_editorOptions.range.commonAncestorContainer.parentElement.tagName){node=_editorOptions.range.commonAncestorContainer.parentElement;}
else if(_editorOptions.range.commonAncestorContainer.parentNode&&_editorOptions.range.commonAncestorContainer.parentNode.tagName){node=_editorOptions.range.commonAncestorContainer.parentNode;}}}}
if(!node){if(_editorOptions.selection.focusNode){if(_editorOptions.selection.focusNode.nodeType==1){node=_editorOptions.selection.focusNode;}
else{if(_editorOptions.selection.focusNode.parentElement&&_editorOptions.selection.focusNode.parentElement.tagName){node=_editorOptions.selection.focusNode.parentElement;}
else if(_editorOptions.selection.focusNode.parentNode&&_editorOptions.selection.focusNode.parentNode.tagName){node=_editorOptions.selection.focusNode.parentNode;}}}
else if(_editorOptions.selection.anchorNode){if(_editorOptions.selection.anchorNode.nodeType==1){node=_editorOptions.selection.anchorNode;}
else{if(_editorOptions.selection.anchorNode.parentElement&&_editorOptions.selection.anchorNode.parentElement.tagName){node=_editorOptions.selection.anchorNode.parentElement;}
else if(_editorOptions.selection.anchorNode.parentNode&&_editorOptions.selection.anchorNode.parentNode.tagName){node=_editorOptions.selection.anchorNode.parentNode;}}}}}
else if(_editorOptions.elementEditedDoc.selection&&_editorOptions.elementEditedDoc.selection.createRange){if(_editorOptions.client){_editorOptions.range=_editorOptions.client.createIErange.apply(_editorOptions.elementEditedWindow);}
else{_editorOptions.range=_editorOptions.elementEditedDoc.selection.createRange.apply(_editorOptions.elementEditedWindow);}
if(_editorOptions.range.htmlText){_editorOptions.selectedHtml=_editorOptions.range.htmlText;}
else if(_editorOptions.range.length>0){_editorOptions.selectedHtml=_editorOptions.range.item(0).outerHTML;}
if(forSelOnly)return;if(_editorOptions.elementEditedDoc.selection.type=='Control'){node=_editorOptions.range(0);}
else{node=_editorOptions.range.parentElement();}}
if(node){if(obj){var op=obj;var isP=false;while(op){if(op==node){isP=true;break;}
op=op.parentNode;}
if(isP){node=obj;}}}
else{node=obj;}
if(node){if(node!=selObj){selectEditElement(node);}}
_editorOptions.iframeFocus=true;showFontCommands();}
function isNonBodyNode(c){if(c){if(c.tagName){var tag=c.tagName.toLowerCase();if(tag=='html'||tag=='meta'||tag=='link'||tag=='script'||tag=='head'){return true;}
return false;}
else{if(c.allowCommands){return false;}}}
return true;}
function showFontCommands(){if(_editorOptions&&_editorOptions.deleted){_imgUndel.src='/libjs/undel.png';}
else{_imgUndel.src='/libjs/undel_inact.png';}
var c=_editorOptions?(_editorOptions.propertiesOwner?_editorOptions.propertiesOwner:_editorOptions.selectedObject):null;if(showEditorButtons(c)){showtextAlign(_editorOptions.selectedObject);var i;_fontCommands.style.display='inline';var imgs=_fontCommands.getElementsByTagName('img');for(i=0;i<imgs.length;i++){if(imgs[i].cmd&&!imgs[i].isCust){try{if(_editorOptions.elementEditedDoc.queryCommandState(imgs[i].cmd)){imgs[i].src=imgs[i].actSrc;}
else{imgs[i].src=imgs[i].inactSrc;}}
catch(err){}}}
if(c){var fontNode;var headingNode;var obj=c;while(obj&&obj!=_editorOptions.elementEditedDoc.body){var tag='';if(obj.tagName){tag=obj.tagName.toLowerCase();}
if(!fontNode){if(tag=='span'){fontNode=obj;if(headingNode){break;}}}
if(!headingNode){if(tag=='h1'||tag=='h2'||tag=='h3'||tag=='h4'||tag=='h5'||tag=='h6'){headingNode=obj;if(fontNode){break;}}}
obj=obj.parentNode;}
var found;if(_fontSelect&&_fontSelect.options&&_fontSelect.options.length>0){if(fontNode){found=false;for(i=0;i<_fontSelect.options.length;i++){if(_fontSelect.options[i].value==fontNode.style.fontFamily){_fontSelect.selectedIndex=i;found=true;break;}}
if(!found){_fontSelect.selectedIndex=0;}}
else{_fontSelect.selectedIndex=0;}}
if(fontNode){_colorSelect.value=fontNode.style.color;_colorSelect.style.backgroundColor=fontNode.style.color;}
else{_colorSelect.style.backgroundColor='white';}
if(_headingSelect&&_headingSelect.options&&_headingSelect.options.length>0){if(headingNode){found=false;for(i=0;i<_headingSelect.options.length;i++){if('h'+_headingSelect.options[i].value==tag){_headingSelect.selectedIndex=i;found=true;break;}}
if(!found){_headingSelect.selectedIndex=0;}}
else{_headingSelect.selectedIndex=0;}}}
else{_colorSelect.style.backgroundColor='white';}
_colorSelect.style.color=_colorSelect.style.backgroundColor;if(canStopTag()){_imgInsSpace.src='/libjs/insSpace.png';_imgStopTag.src='/libjs/stopTag.png';_imgInsBr.src='/libjs/insBr.png';_imgAppBr.src='/libjs/appBr.png';}
else{_imgInsSpace.src='/libjs/insSpace_inact.png';_imgStopTag.src='/libjs/stopTag_inact.png';_imgInsBr.src='/libjs/insBr_inact.png';_imgAppBr.src='/libjs/appBr_inact.png';}
showUndoRedo();}}
function createSvg(){if(!_editorOptions)return;var svg=_createSVGElement('svg');svg.style.top='0px';svg.style.left='0px';svg.style.height='300px';svg.style.width='300px';svg.style.borderWidth='1px';svg.style.borderStyle='dotted';_appendChild(_editorOptions.elementEditedDoc.body,svg);return svg;}
function createElement(tagname,type,typename){if(!_editorOptions)return;for(var k=0;k<_elementEditorList.length;k++){if(_elementEditorList[k].tagname==typename){if(_elementEditorList[k].type){if(_elementEditorList[k].type==type){if(_elementEditorList[k].creator){if(_editorOptions.inline)
return _elementEditorList[k].creator();else
return _elementEditorList[k].creator.apply(_editorOptions.objEdited);}}}
else if(_elementEditorList[k].creator){if(_editorOptions.inline)
return _elementEditorList[k].creator();else
return _elementEditorList[k].creator.apply(_editorOptions.objEdited);}}}
return _createElement(tagname);}
function _addCommandList(commands){if(_commandList){if(commands){var i;for(i=0;i<commands.length;i++){for(var k=0;k<_commandList.length;k++){if(_commandList[k].cmd==commands[i].cmd){_commandList.slice(k,1);break;}}}
for(i=0;i<commands.length;i++){_commandList.push(commands[i]);}}}
else{_commandList=commands;}}
function _addAddableElementList(addables){if(_addableElementList){if(addables){var i;for(i=0;i<addables.length;i++){for(var k=0;k<_addableElementList.length;k++){if(_addableElementList[k].tag==addables[i].tag){if(addables[i].type){if(addables[i].type==_addableElementList[k].type){_addableElementList=_addableElementList.slice(k,1);break;}}
else{_addableElementList=_addableElementList.slice(k,1);break;}}}}
for(i=0;i<addables.length;i++){_addableElementList.push(addables[i]);}}}
else{_addableElementList=addables;}}
function _getElementDesc(elementTag){if(elementTag){elementTag=elementTag.toLowerCase();if(_addableElementList){for(var k=0;k<_addableElementList.length;k++){if(_addableElementList[k].tag==elementTag){return _addableElementList[k].name;}}}}
return elementTag;}
function _addEditors(editors){if(_elementEditorList){if(editors){var i;for(i=0;i<editors.length;i++){for(var k=0;k<_elementEditorList.length;k++){if(_elementEditorList[k].tagname==editors[i].tagname){_elementEditorList.slice(k,1);break;}}}
for(i=0;i<editors.length;i++){_elementEditorList.push(editors[i]);}}}
else{_elementEditorList=editors;}}
function hideSelector(){if(_divSelectElement){_divSelectElement.style.display='none';}
if(_divLargeEnum){_divLargeEnum.style.display='none';}
_showResizer();}
function framenotify(event){if(_editorOptions)
selectEditElement(_editorOptions.elementEditedDoc.activeElement);}
function insertNodeOverSelection(node,tagname){if(!_editorOptions.selectedObject)return;var newclass=null;var parentObj=_editorOptions.selectedObject.subEditor?_editorOptions.selectedObject.obj:_editorOptions.selectedObject;if(parentObj.tagName){var stag=parentObj.tagName.toLowerCase();if(stag=='input'||stag=='textarea'){showError('Cannot insert a new element into an input element');return;}}
var undoItem;if(tagname=='area'){var map=parentObj;while(map){if(map.tagName&&map.tagName.toLowerCase()=='map')
break;map=map.parentNode;}
if(map){map.appendChild(node);selectEditElement(node);}}
else{try{var needSwitchToDiv;var pp;var ptag=_editorOptions.selectedObject.subEditor?_editorOptions.selectedObject.obj.tagName:_editorOptions.selectedObject.tagName;if(tagname!='a'&&!HtmlEditor.CanBeChild.apply(_editorOptions.elementEditedWindow,[ptag,tagname])){if(ptag=='p'||ptag=='P'){needSwitchToDiv=true;}
if(!needSwitchToDiv){alert('Cannot add a child of "'+tagname+'" to a "'+ptag+'". If you cannot move the cursor outside of current element then click image "/>" on the toolbar.');return;}}
if(_editorOptions.objEdited.getSelection){if(_editorOptions.range){if(tagname=='span'||tagname=='h1'||tagname=='h2'||tagname=='h3'||tagname=='h4'||tagname=='h5'||tagname=='h6'){if(_editorOptions.selectedHtml&&_editorOptions.selectedHtml.length>0)
node.innerHTML=_editorOptions.selectedHtml;_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow,[_editorOptions.range,node]);}
else if(tagname=='a'){var stag='';if(_editorOptions.selectedObject&&_editorOptions.selectedObject.tagName){stag=_editorOptions.selectedObject.tagName.toLowerCase();}
if(stag=='button'||stag=='img'){_insertBefore(_editorOptions.selectedObject.parentNode,node,_editorOptions.selectedObject);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[node,_editorOptions.selectedObject]);}
else{if(stag=='td'){if(_editorOptions.selectedHtml&&_editorOptions.selectedHtml.length>0){node.innerHTML=_editorOptions.selectedHtml;_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow,[_editorOptions.range,node]);}
else{var ht=_editorOptions.selectedObject.innerHTML;_editorOptions.selectedObject.innerHTML='';node.innerHTML=ht;_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[_editorOptions.selectedObject,node]);}}
else if(stag!='tr'&&stag!='tbody'&&stag!='thead'&&stag!='tfoot'&&stag!='table'){if(_editorOptions.selectedHtml&&_editorOptions.selectedHtml.length>0){node.innerHTML=_editorOptions.selectedHtml;_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow,[_editorOptions.range,node]);}
else{if(_editorOptions.selectedObject){_insertBefore(_editorOptions.selectedObject.parentNode,node,_editorOptions.selectedObject);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[node,_editorOptions.selectedObject]);}}}
else{return;}}}
else{_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow,[_editorOptions.range,node]);}
if(node.focus){_editorOptions.client.focus.apply(_editorOptions.elementEditedWindow,[node]);}}
else{if(tagname=='a'){if(stag=='button'||stag=='img'){_insertBefore(_editorOptions.selectedObject.parentNode,node,_editorOptions.selectedObject);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[node,_editorOptions.selectedObject]);}
else{if(stag=='td'){if(_editorOptions.selectedHtml&&_editorOptions.selectedHtml.length>0){node.innerHTML=_editorOptions.selectedHtml;_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow,[_editorOptions.selectedObject,node]);}
else{if(!node.innerHTML||node.innerHTML.length==0){node.innerHTML='hyper link';}
_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[_editorOptions.selectedObject,node]);}}
else if(stag!='tr'&&stag!='tbody'&&stag!='thead'&&stag!='tfoot'&&stag!='table'){if(_editorOptions.selectedObject){if(!node.innerHTML||node.innerHTML.length==0){node.innerHTML='hyper link';}
_insertBefore(_editorOptions.selectedObject.parentNode,node,_editorOptions.selectedObject);}
else{return;}}
else{return;}}}
else{_appendChild(parentObj,node);}}}
else{if(_editorOptions.range&&_editorOptions.range.parentElement&&parentObj){var o=_editorOptions.range.parentElement();if(tagname=='span'||tagname=='a'||tagname=='h1'||tagname=='h2'||tagname=='h3'||tagname=='h4'||tagname=='h5'||tagname=='h6'){if(_editorOptions.selectedHtml)
node.innerHTML=_editorOptions.selectedHtml;else
node.innerHTML=_editorOptions.range.text;}
var id='v'+Math.floor(Math.random()*65536);node.id=id;var html=(node.nodeType==3)?node.data:node.outerHTML;if(_editorOptions.client){node=_editorOptions.client.pasteHtmlToRange.apply(_editorOptions.elementEditedWindow,[_editorOptions.range,html,id]);}
else{_editorOptions.range.pasteHTML.apply(_editorOptions.elementEditedWindow,[html]);node=_editorOptions.elementEditedDoc.getElementById.apply(_editorOptions.elementEditedWindow,[id]);}
if(newclass!=null){node.setAttribute('styleName',newclass);node.className=newclass;}
if(node){if(node.focus)
node.focus();node.id='';}}
else{parentObj.appendChild.apply(_editorOptions.elementEditedWindow,[node]);}}
if(needSwitchToDiv){var div=_createElement('div');var parent=parentObj.parentNode;_insertBefore(parent,div,parentObj);while(parentObj.firstChild){_appendChild(div,parentObj.firstChild);}
_removeChild(parent,parentObj);}
if(node){if(node.focus)
node.focus();if(tagname=='iframe'){selectEditElement(node);var fdoc=node.contentDocument||node.contentWindow.document;fdoc.onclick=framenotify;}
else if(tagname=='a'){if(node.children.length>0){_editorOptions.selectedObject=null;selectEditElement(node.children[0]);for(var i=0;i<_parentList.options.length;i++){if(_parentList.options[i].objvalue==node){_parentList.selectedIndex=i;break;}}
showProperties(node);}
else{selectEditElement(node);}}
else{selectEditElement(node);}}}
catch(err){showError('Cannot insert new element. '+(err.message?err.message:err));}}
return node;}
function isOrContainsNode(ancestor,descendant){var node=descendant;while(node){if(node===ancestor)
return true;node=node.parentNode;}
return false;}
function createLargeEnum(){_divLargeEnum.style.position='absolute';_divLargeEnum.style.display='none';_divLargeEnum.contentEditable=false;_divLargeEnum.forProperties=true;var tbls=document.createElement('table');tbls.border=0;tbls.cellPadding=0;tbls.cellSpacing=0;tbls.forProperties=true;_divLargeEnum.appendChild(tbls);function _setData(options,cmdImg){var rn=Math.floor(Math.sqrt(options.length));var rC=rn+3;var tC=Math.floor(options.length/rC);while(rC*tC<options.length){tC++;}
var tbody0=JsonDataBinding.getTableBody(tbls);while(tbody0.firstChild){tbody0.removeChild(tbody0.firstChild);}
var tr0=tbody0.insertRow(-1);var ci=0;for(var ti=0;ti<tC;ti++){var td0=document.createElement('td');td0.style.verticalAlign='top';tr0.appendChild(td0);var tblSel=document.createElement('table');tblSel.style.verticalAlign='top';td0.appendChild(tblSel);tblSel.border=1;tblSel.bgColor=_newElementSelectTableBackColor;tblSel.style.cursor='pointer';var tbody=JsonDataBinding.getTableBody(tblSel);for(var i=0;i<rC;i++){var tr=tbody.insertRow(-1);tr.opt=options[ci];var td=document.createElement('td');tr.appendChild(td);td.innerHTML=options[ci].text;JsonDataBinding.AttachEvent(tr,'onmouseover',onhsMouseOver);JsonDataBinding.AttachEvent(tr,'onmouseout',onhsMouseOut);JsonDataBinding.AttachEvent(tr,'onclick',onhsMouseClick);ci++;if(ci>=options.length)break;}}
function restoreBackColor(cells){var bkc=_newElementSelectTableBackColor;for(var c=0;c<cells.length;c++){if(cells[c].isMouseOver){cells[c].style.backgroundColor='#ffffc0';}
else{cells[c].style.backgroundColor=bkc;}}}
function onhsMouseClick(e){if(!_editorOptions)return;var cell=JsonDataBinding.getSender(e);if(cell){var tr=cell.parentNode;while(tr&&tr.tagName&&tr.tagName.toLowerCase()!='tr'){tr=tr.parentNode;}
if(tr&&tr.addable){hideSelector();if(_editorOptions.isEditingBody){_editorOptions.elementEditedDoc.body.contentEditable=true;}
if(tr.opt){}}}}
function onhsMouseOver(e){var cell=JsonDataBinding.getSender(e);;if(cell){var tbl=cell.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");for(var i=0;i<cells.length;i++){cells[i].isMouseOver=false;}
cell.isMouseOver=true;restoreBackColor(cells);}}
function onhsMouseOut(e){var cell=JsonDataBinding.getSender(e);if(cell){var tbl=cell.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");cell.isMouseOver=false;restoreBackColor(cells);}}}
return{setData:function(options,cmdImg){_setData(options,cmdImg);}};}
function createElementSelector(){_divSelectElement.style.position='absolute';_divSelectElement.style.display='none';_divSelectElement.contentEditable=false;_divSelectElement.forProperties=true;var rn=Math.floor(Math.sqrt(_addableElementList.length));var rC=rn+3;var tC=Math.floor(_addableElementList.length/rC);while(rC*tC<_addableElementList.length){tC++;}
var tbls=document.createElement('table');tbls.border=0;tbls.cellPadding=0;tbls.cellSpacing=0;tbls.forProperties=true;_divSelectElement.appendChild(tbls);var tbody0=JsonDataBinding.getTableBody(tbls);var tr0=tbody0.insertRow(-1);var ci=0;for(var ti=0;ti<tC;ti++){var td0=document.createElement('td');td0.style.verticalAlign='top';tr0.appendChild(td0);var tblSel=document.createElement('table');tblSel.style.verticalAlign='top';td0.appendChild(tblSel);tblSel.border=1;tblSel.bgColor=_newElementSelectTableBackColor;tblSel.style.cursor='pointer';var tbody=JsonDataBinding.getTableBody(tblSel);for(var i=0;i<rC;i++){var tr=tbody.insertRow(-1);tr.addable=_addableElementList[ci];var td=document.createElement('td');tr.appendChild(td);var img=document.createElement('img');img.src=_addableElementList[ci].image;img.style.display='inline';td.appendChild(img);td=document.createElement('td');tr.appendChild(td);td.innerHTML=_addableElementList[ci].name;JsonDataBinding.AttachEvent(tr,'onmouseover',onhsMouseOver);JsonDataBinding.AttachEvent(tr,'onmouseout',onhsMouseOut);JsonDataBinding.AttachEvent(tr,'onclick',onhsMouseClick);if(_addableElementList[ci].toolTips){hookTooltips(img,_addableElementList[ci].toolTips);}
ci++;if(ci>=_addableElementList.length)break;}}
function restoreBackColor(cells){var bkc=_newElementSelectTableBackColor;for(var c=0;c<cells.length;c++){if(cells[c].isMouseOver){cells[c].style.backgroundColor='#ffffc0';}
else{cells[c].style.backgroundColor=bkc;}}}
function onhsMouseClick(e){if(!_editorOptions)return;var cell=JsonDataBinding.getSender(e);if(cell){var tr=cell.parentNode;while(tr&&tr.tagName&&tr.tagName.toLowerCase()!='tr'){tr=tr.parentNode;}
if(tr&&tr.addable){hideSelector();if(_editorOptions.isEditingBody){_editorOptions.elementEditedDoc.body.contentEditable=true;}
else{if(tr.addable.pageonly){alert('This element is for page editing only');tr.style.display='none';return;}}
if(tr.addable.tag){if(tr.addable.isSVG){var newNode=createSvg();if(newNode){if(tr.addable.onCreated){tr.addable.onCreated(newNode);}
selectEditElement(newNode);}
return;}
if(_editorOptions.selectedObject){if(tr.addable.tag=='hitcount'){if(!_editorOptions.elementEditedDoc.body.getAttribute('pageId')){return;}
var hc=_getElementById(HtmlEditor.hitcountid);if(hc){selectEditElement(hc);return;}}
if(_editorOptions.selectedObject.typename||(_editorOptions.selectedObject.getAttribute&&_editorOptions.selectedObject.getAttribute('typename'))){}
else{if(!_editorOptions.isEditingBody&&(tr.addable.tag=='treeview'||tr.addable.tag=='menubar')){alert(tr.addable.tag+' can only be used to build web pages.');return;}
var newElement=createElement(tr.addable.htmltag?tr.addable.htmltag:tr.addable.tag,tr.addable.type,tr.addable.tag);var newNode=insertNodeOverSelection(newElement,tr.addable.htmltag?tr.addable.htmltag:tr.addable.tag);if(tr.addable.onCreated){tr.addable.onCreated(newNode);}
_editorOptions.pageChanged=true;}}}}}}
function onhsMouseOver(e){var cell=JsonDataBinding.getSender(e);;if(cell){var tbl=cell.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");for(var i=0;i<cells.length;i++){cells[i].isMouseOver=false;}
cell.isMouseOver=true;restoreBackColor(cells);}}
function onhsMouseOut(e){var cell=JsonDataBinding.getSender(e);if(cell){var tbl=cell.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");cell.isMouseOver=false;restoreBackColor(cells);if(cell.toolTips){cell.moves=0;if(_divToolTips){_divToolTips.style.display='none';}}}}
function _checkAdditables(){var inline=true;if(_editorOptions&&_editorOptions.isEditingBody){inline=false;}
var ccs=tr0.cells;for(var ti=0;ti<ccs.length;ti++){var td0=ccs[ti];var tblSel;for(var i=0;i<td0.children.length;i++){if(td0.children[i].tagName.toLowerCase()=='table'){tblSel=td0.children[i];break;}}
if(tblSel){var tbody=JsonDataBinding.getTableBody(tblSel);for(var i=0;i<tbody.rows.length;i++){var tr=tbody.rows[i];if(inline&&tr.addable.pageonly){tr.style.display='none';}
else{if(tr.addable.tag=='hitcount'){if(!_editorOptions.elementEditedDoc.body.getAttribute('pageId')){tr.style.display='none';}
else if(inline||_getElementById(HtmlEditor.hitcountid))
tr.style.display='none';else
tr.style.display='';}
else
tr.style.display='';}}}}}
_divSelectElement.jsData={checkAdditables:function(){_checkAdditables();}};return _divSelectElement.jsData;}
function selectparent(){if(_parentList.selectedIndex>=0&&_parentList.selectedIndex<_parentList.options.length){var obj=_parentList.options[_parentList.selectedIndex].objvalue;showProperties(obj);}}
function cancelClick(){askClose();}
function askClose(saved){if(_editorOptions){if(_editorOptions.isEditingBody){if(_editorOptions.closePage){if(saved){saved=confirm('The web page is saved.\n\n Do you want to close the page now?\n Click Cancel if you want to continue editing the page.');}
else{saved=confirm('Do you want to close the page being edited? \n\nModifications will be discarded if you close it now.');}
if(saved){_editorOptions.closePage(true);_editorOptions=null;clearPropertiesDisplay();}}}
else{if(_editorOptions.cancelHandler){saved=confirm('Do you want to cancel your message?');if(saved){_editorOptions.cancelHandler();}}}}}
function _close(){if(_editorOptions&&_editorOptions.closePage){_editorOptions.closePage(true);_editorOptions=null;clearPropertiesDisplay();}}
function finishEdit(p){if(_editorOptions&&_editorOptions.finishHandler){if(_collectingResult){_saveTryCount++;if(_saveTryCount<2){alert('Please wait for the last saving operation to finish');}}
if(!_collectingResult){_collectingResult=true;_saveTryCount=0;try{if(_editorOptions.isEditingBody){var toCache=p?p.toCache:false;var manualInvoke=p?p.manualInvoke:false;var html=_getresultHTML(!toCache);var pos=html.indexOf('dyStyle8831932');if(pos>0){var pos0=html.indexOf('<style ');if(pos0>0&&pos0<pos){var pos1;while(true){pos1=html.indexOf('<style ',pos0+1);if(pos1>pos0){pos0=pos1;}
else
break;}
pos1=html.indexOf('</style>',pos);var pos2=html.indexOf('/>',pos);if(pos1>0){if(pos2>0&&pos2<pos1){pos1=pos2+2;}
else{pos1=pos1+8;}}
else{pos1=pos2+2;}
if(pos1>0){html=html.substr(0,pos0)+html.substr(pos1);}}}
var dynText=_editorOptions.client.getDynamicCssText.apply(_editorOptions.elementEditedWindow,[_editorOptions.client]);_editorOptions.undoState={};_editorOptions.undoList=new Array();_editorOptions.finishHandler.apply(_editorOptions.elementEditedWindow,[{url:getWebAddress(),html:html,css:dynText,toCache:toCache,manualInvoke:manualInvoke,saveToFolder:_editorOptions.saveToFolder?_editorOptions.saveToFolder:'',saveTo:_editorOptions.saveTo?_editorOptions.saveTo:''}]);}
else{_editorOptions.client.cleanup.apply(_editorOptions.elementEditedWindow,[_editorOptions.client]);_editorOptions.finishHandler(_editorOptions.elementEdited.innerHTML);}}
catch(eSave){alert('Error saving page. '+(eSave.message?eSave.message:eSave));}
_collectingResult=false;}}}
function _getHtmlString(){if(_collectingResult){return'';}
else{_collectingResult=true;var ret=_getresultHTML();_collectingResult=false;return ret;}}
function _saveAndFinish(){finishEdit();_editorOptions.pageChanged=false;}
function _pageModified(){return(_editorOptions?_editorOptions.pageChanged:false);}
function _setDocType(docType){_editorOptions.docType=docType;}
function createTable(){var tbl=_editorOptions.elementEditedDoc.createElement('table');tbl.border=1;var h=_editorOptions.elementEditedDoc.createElement('thead');tbl.appendChild(h);var r=_editorOptions.elementEditedDoc.createElement('tr');h.appendChild(r);var c=_editorOptions.elementEditedDoc.createElement('td');r.appendChild(c);c.innerHTML='column 1';c=_editorOptions.elementEditedDoc.createElement('td');r.appendChild(c);c.innerHTML='column 2';var tbody=JsonDataBinding.getTableBody(tbl);r=_editorOptions.elementEditedDoc.createElement('tr');tbody.appendChild(r);c=_editorOptions.elementEditedDoc.createElement('td');r.appendChild(c);c.innerHTML='row 1, cell 1';c=_editorOptions.elementEditedDoc.createElement('td');r.appendChild(c);c.innerHTML='row 1 cell 2';r=_editorOptions.elementEditedDoc.createElement('tr');tbody.appendChild(r);c=_editorOptions.elementEditedDoc.createElement('td');r.appendChild(c);c.innerHTML='row 2, cell 1';c=_editorOptions.elementEditedDoc.createElement('td');r.appendChild(c);c.innerHTML='row 2 cell 2';return tbl;}
function isInLocator(obj){while(obj&&obj!=document.body&&obj!=window){if(obj==_elementLocator||obj==_imgLocatorElement){return true;}
obj=obj.parentNode;}
return false;}
function isInTextInputor(obj){while(obj&&obj!=document.body&&obj!=window){if(obj==_textInput||obj==_textInput){return true;}
obj=obj.parentNode;}
return false;}
function locatorInputchanged(){if(_editorOptions&&_locatorInput.value.length>0){var tag;var objs=_editorOptions.elementEditedDoc.getElementsByTagName(_locatorInput.value);if(!objs||objs.length==0){if(_addableElementList){for(var i=0;i<_addableElementList.length;i++){if(_addableElementList[i].tag==_locatorInput.value){objs=_editorOptions.elementEditedDoc.getElementsByTagName(_addableElementList[i].htmltag);tag=_addableElementList[i].tag;break;}}}}
if(objs&&objs.length>0){_locatorList.options.length=0;_objectsInLocatorList=new Array();for(var i=0;i<objs.length;i++){if(!isMarker(objs[i])){if(tag){var tag0=objs[i].typename;if(!tag0){if(objs[i].getAttribute){tag0=objs[i].getAttribute('typename');}}
if(!tag0){if(objs[i].jsData){tag0=objs[i].jsData.typename;}}
if(tag!=tag0)
continue;}
var elOptNew=document.createElement('option');elOptNew.text=elementToString(objs[i]);_objectsInLocatorList.push(objs[i]);addOptionToSelect(_locatorList,elOptNew);}}
if(_locatorList.options.length<3){_locatorList.size=3;}
else if(_locatorList.options.length>10){_locatorList.size=10;}
else{_locatorList.size=_locatorList.options.length;}}}}
function selectedFromlocator(){if(_locatorList.selectedIndex>=0){var obj=_objectsInLocatorList[_locatorList.selectedIndex];_elementLocator.style.display='none';if(obj.parentNode){selectEditElement(obj);}
else{alert('This element has been removed. Please make another selection.');}}}
function locateElement(){if(!_editorOptions)return;if(!_elementLocator){_elementLocator=document.createElement('div');_elementLocator.style.backgroundColor='#ADD8E6';_elementLocator.style.position='absolute';_elementLocator.style.border="3px double black";_elementLocator.style.color='black';appendToBody(_elementLocator);var tbl=document.createElement("table");_elementLocator.appendChild(tbl);tbl.border=0;tbl.cellPadding=0;tbl.cellSpacing=0;tbl.width='100%';tbl.height='100%';var tblBody=JsonDataBinding.getTableBody(tbl);var tr=tblBody.insertRow(-1);tr.setAttribute('valign','top');td=document.createElement('td');tr.appendChild(td);var cap=document.createElement('span');td.appendChild(cap);cap.innerHTML='tag name:';td=document.createElement('td');tr.appendChild(td);_locatorInput=document.createElement('input');_locatorInput.type='text';td.appendChild(_locatorInput);JsonDataBinding.AttachEvent(_locatorInput,'onchange',locatorInputchanged);JsonDataBinding.addTextBoxObserver(_locatorInput);JsonDataBinding.AttachEvent(_locatorInput,'onkeyup',locatorInputchanged);tr=tblBody.insertRow(-1);tr.style.height='100%';tr.vAlign='top';td=document.createElement('td');tr.appendChild(td);td=document.createElement('td');tr.appendChild(td);_locatorList=document.createElement('select');td.appendChild(_locatorList);JsonDataBinding.AttachEvent(_locatorList,'onclick',selectedFromlocator);}
else{locatorInputchanged();}
_elementLocator.style.width='300px';_elementLocator.style.height='185px';JsonDataBinding.windowTools.updateDimensions();var ap=JsonDataBinding.ElementPosition.getElementPosition(_imgLocatorElement);_elementLocator.style.left=(ap.x+3)+'px';_elementLocator.style.top=(ap.y+3)+'px';var zi=JsonDataBinding.getPageZIndex(_elementLocator)+1;_elementLocator.style.zIndex=zi;_elementLocator.style.display='block';}
function addElement(e){if(!_editorOptions)return;var sender=JsonDataBinding.getSender(e);if(sender&&_editorOptions.selectedObject){JsonDataBinding.windowTools.updateDimensions();var ap=JsonDataBinding.ElementPosition.getElementPosition(sender);if(ap){if(ap.x<0)ap.x=0;if(ap.y<0)ap.y=0;_divSelectElement.style.left=(ap.x+3)+'px';_divSelectElement.style.top=(ap.y+3)+'px';_divSelectElement.style.zIndex=JsonDataBinding.getZOrder(sender)+1;_divSelectElement.style.display='block';_nameresizer.style.display='none';}}
return;}
function onFontSelected(e){if(_fontSelect.selectedIndex>=0){if(_editorOptions&&(_editorOptions.selectedHtml||_editorOptions.ifrmSelHtml)){if(!_editorOptions.selectedHtml){_editorOptions.selectedHtml=_editorOptions.ifrmSelHtml;_editorOptions.ifrmSelHtml=null;}
var fi=_fontSelect.options[_fontSelect.selectedIndex];var fn=_editorOptions.elementEditedDoc.createElement('span');if(_editorOptions.isEditingBody){var sl='s'+Math.floor(Math.random()*65536);fn.setAttribute('styleName',sl);fn.className=sl;}
if(_fontSelect.selectedIndex>0){fn.style.fontFamily=fi.value;setCSStext(fn,'font-family',fi.value);}
insertNodeOverSelection(fn,'span');_editorOptions.pageChanged=true;}
else{alert('Please make a text selection to apply font');}}}
function onHeadingSelected(e){if(_editorOptions&&_headingSelect.selectedIndex>0){var p,p2,fi,h,i;if(typeof _editorOptions.selectedHtml=='undefined'||_editorOptions.selectedHtml==null||_editorOptions.selectedHtml.length==0){captureSelection({target:_editorOptions.selectedObject},true);}
p=_editorOptions.selectedObject;if(p){if(p.subEditor){p=p.obj;}}
else{if(_editorOptions.isEditingBody){p=_editorOptions.elementEditedDoc.activeElement;}
else{p=_editorOptions.elementEdited;}}
if(typeof _editorOptions.selectedHtml=='undefined'||_editorOptions.selectedHtml==null||_editorOptions.selectedHtml.length==0){if(p&&p!=_editorOptions.elementEdited){var p3=p.parentNode;if(p3){fi=_headingSelect.options[_headingSelect.selectedIndex];h=_createElement('h'+fi.value);h.innerHTML=p.innerHTML;var p4=p.nextSibling;p3.removeChild(p);p2=HtmlEditor.getParent.apply(_editorOptions.elementEditedWindow,[p3,'h'+fi.value]);if(p2!=p3){var found=false;while(p3&&!found){for(i=0;i<p2.children.length;i++){if(p2.children[i]==p3){found=true;break;}}
if(!found){p3=p3.parentNode;}}
if(p3){_insertBefore(p2,h,p3);}
else{_appendChild(p2,h);}}
else{_insertBefore(p2,h,p4);}
_editorOptions.pageChanged=true;}}}
else{if(!p){p=_editorOptions.elementEdited;}
fi=_headingSelect.options[_headingSelect.selectedIndex];h=_createElement('h'+fi.value);h.innerHTML=_editorOptions.selectedHtml;insertNodeOverSelection(h,'h'+fi.value);_editorOptions.pageChanged=true;}}}
function canStopTag(){if(_editorOptions){var tag;if(_editorOptions.propertiesOwner){var p=_editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;if(p){tag=p.tagName?p.tagName.toLowerCase():null;if(tag&&tag!='td'&&tag!='tr'&&tag!='tbody'&&tag!='thead'&&tag!='tfoot'&&tag!='th'){p=p.parentNode;if(p){tag=p.tagName?p.tagName.toLowerCase():null;if(tag&&tag!='tr'&&tag!='tbody'&&tag!='thead'&&tag!='tfoot'&&tag!='th'&&tag!='table'&&tag!='ul'&&tag!='ol'&&tag!='select'&&tag!='map'){return true;}}}}}}
return false;}
function restoreBodyInnerHtml(s){var rb;_editorOptions.elementEditedDoc.body.innerHTML=s;if(_editorOptions.redbox){_editorOptions.redbox.contentEditable=false;rb=_editorOptions.elementEditedDoc.getElementById(_editorOptions.redbox.id);if(rb){_editorOptions.redbox=rb;_editorOptions.redbox.forProperties=true;_editorOptions.redbox.contentEditable=false;}}
if(_editorOptions.markers){for(var i=0;i<_editorOptions.markers.length;i++){if(_editorOptions.markers[i]){_editorOptions.markers[i].contentEditable=false;rb=_editorOptions.elementEditedDoc.getElementById(_editorOptions.markers[i].id);if(rb){_editorOptions.markers[i]=rb;_editorOptions.markers[i].forProperties=true;_editorOptions.markers[i].contentEditable=false;}}}}}
function undelete(){if(_editorOptions&&_editorOptions.deleted){try{if(_editorOptions.deleted.parent&&_editorOptions.deleted.parent.parentNode){if(_editorOptions.deleted.obj){_insertBefore(_editorOptions.deleted.parent,_editorOptions.deleted.obj,_editorOptions.deleted.next);}
else if(typeof(_editorOptions.deleted.originalHTML)!='undefined'&&_editorOptions.deleted.originalHTML!=null){_editorOptions.deleted.parent.innerHTML=_editorOptions.deleted.originalHTML;}}}
catch(err){}
_editorOptions.deleted=null;_imgUndel.src='/libjs/undel_inact.png';}}
function undo(){if(hasUndo()){if(_editorOptions.undoState.state==UNDOSTATE_UNDO){_editorOptions.undoState.index--;restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].undoInnerHTML);}
else if(_editorOptions.undoState.state==UNDOSTATE_REDO){if(_editorOptions.undoState.index==_editorOptions.undoList.length-1){_editorOptions.undoList[_editorOptions.undoState.index].done=true;_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML=_editorOptions.elementEditedDoc.body.innerHTML;}
restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].undoInnerHTML);_editorOptions.undoState.state=UNDOSTATE_UNDO;}
showUndoRedo();}}
function redo(){if(hasRedo()){if(_editorOptions.undoState.state==UNDOSTATE_UNDO){restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML);_editorOptions.undoState.state=UNDOSTATE_REDO;}
else if(_editorOptions.undoState.state==UNDOSTATE_REDO){_editorOptions.undoState.index++;restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML);}
showUndoRedo();}}
function insSpaceClick(){if(canStopTag()){var sp=document.createTextNode("space");var e=_editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;if(e&&e.tagName&&e.tagName.toLowerCase()=='body'){if(e.children.length>0){_insertBefore(e,sp,e.children[0]);}
else{_insertBefore(e,sp,null);}}
else{var p=e.parentNode;_insertBefore(p,sp,e);}
_editorOptions.pageChanged=true;}}
function stopTagClick(){if(canStopTag()){var sp=document.createTextNode("space");var e=_editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;if(e&&e.tagName&&e.tagName.toLowerCase()=='body'){e.appendChild(sp);}
else{var p=e.parentNode;_insertAfter(p,sp,e);}
_editorOptions.pageChanged=true;}}
function insBrClick(){if(canStopTag()){var sp=document.createElement("br");var e=_editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;if(e&&e.tagName&&e.tagName.toLowerCase()=='body'){if(e.children.length>0){_insertBefore(e,sp,e.children[0]);}
else{_insertBefore(e,sp,null);}}
else{var p=e.parentNode;_insertBefore(p,sp,e);}
_editorOptions.pageChanged=true;}}
function appBrClick(){if(canStopTag()){var sp=document.createElement("br");var e=_editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;if(e&&e.tagName&&e.tagName.toLowerCase()=='body'){e.appendChild(sp);}
else{var p=e.parentNode;_insertAfter(p,sp,e);}
_editorOptions.pageChanged=true;}}
function insLFClick(){var br=_createElement('br');insertNodeOverSelection(br,'br');}
function appTxtClick(){if(_editorOptions&&_editorOptions.client){_editorOptions.client.appendBodyText.apply(_editorOptions.elementEditedWindow);_editorOptions.pageChanged=true;}}
function fontCommandColorSelected(c){_colorSelect.style.color=c;if(_editorOptions){if(!_editorOptions.isEditingBody&&!_editorOptions.selectedHtml&&_editorOptions.lastSelHtml){_editorOptions.selectedHtml=_editorOptions.lastSelHtml;}
if(_editorOptions.selectedHtml){var fn=_editorOptions.elementEditedDoc.createElement('span');if(_editorOptions.isEditingBody){var sl='s'+Math.floor(Math.random()*65536);fn.setAttribute('styleName',sl);fn.className=sl;}
fn=insertNodeOverSelection(fn,'span');setCSStext(fn,'color',c);_editorOptions.pageChanged=true;}
else{var obj=_editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;if(!_editorOptions.isEditingBody){if(obj==_editorOptions.elementEdited){return;}}
_setElementStyleValue(obj,'color','color',c);_editorOptions.pageChanged=true;}}}
function getSelectedObjByTag(tag){var obj=_editorOptions.selectedObject;while(obj){if(obj.tagName&&obj.tagName.toLowerCase()==tag){return obj;}
obj=obj.parentNode;}}
function getPreviousSibling(obj){if(obj){var p=obj.parentNode;if(p){var tag=obj.tagName?obj.tagName.toLowerCase():'';var ret=null;for(var i=0;i<p.children.length;i++){if(p.children[i]==obj){return ret;}
if(p.children[i].tagName.toLowerCase()==tag){ret=p.children[i];}}
return ret;}}
return null;}
function getNextSibling(obj){if(obj){var p=obj.parentNode;if(p){var tag=obj.tagName?obj.tagName.toLowerCase():'';for(var i=0;i<p.children.length;i++){if(p.children[i]==obj){for(var k=i+1;k<p.children.length;k++){if(p.children[k]&&p.children[k].tagName&&p.children[k].tagName.toLowerCase()==tag){return p.children[k];}}
break;}}}}
return null;}
function LI_indent(li){if(li){var ul0=li.parentNode;var tag=ul0?(ul0.tagName?ul0.tagName.toLowerCase():null):null;if(tag=='ul'||tag=='ol'){var preLI=getPreviousSibling(li);if(preLI){var uol;for(var i=0;i<preLI.children.length;i++){if(preLI.children[i].tagName.toLowerCase()==tag){uol=preLI.children[i];break;}}
if(!uol){uol=_createElement(tag);_appendChild(preLI,uol);}
_removeChild(ul0,li);_appendChild(uol,li);return true;}}}}
function LI_dedent(li){if(li){var ul0=li.parentNode;var tag=ul0?(ul0.tagName?ul0.tagName.toLowerCase():null):null;if(tag=='ul'||tag=='ol'){var pli=ul0.parentNode;if(pli&&pli.tagName&&pli.tagName.toLowerCase()=='li'){var ul1=pli.parentNode;if(ul1&&ul1.tagName&&ul1.tagName.toLowerCase()==tag){var pliNext=getNextSibling(pli);_removeChild(ul0,li);_insertBefore(ul1,li,pliNext);if(ul0.children.length==0){_removeChild(pli,ul0);}
return true;}}}}}
function LI_moveup(li,inEditor){if(li){var ul0=li.parentNode;var tag=ul0?(ul0.tagName?ul0.tagName.toLowerCase():null):null;if(tag=='ul'){var preLI=getPreviousSibling(li);if(preLI){if(inEditor){ul0.removeChild(li);ul0.insertBefore(li,preLI);}
else{_removeChild(ul0,li);_insertBefore(ul0,li,preLI);}
return true;}}}}
function LI_movedown(li,inEditor){if(li){var ul0=li.parentNode;var tag=ul0?(ul0.tagName?ul0.tagName.toLowerCase():null):null;if(tag=='ul'){var nextLI0=getNextSibling(li);if(nextLI0){var nextLI=getNextSibling(nextLI0);if(inEditor){ul0.removeChild(li);ul0.insertBefore(li,nextLI);}
else{_removeChild(ul0,li);_insertBefore(ul0,li,nextLI);}
return true;}}}}
function showtextAlign(obj0){if(_editorOptions){var v;var obj=obj0.subEditor?obj0.obj:obj0;if(_editorOptions.isEditingBody&&_editorOptions.client)
v=_editorOptions.client.getElementStyleValue.apply(_editorOptions.elementEditedWindow,[obj,'text-align']);else
v=obj.style.textAlign;var imgs=_fontCommands.getElementsByTagName('img');for(i=0;i<imgs.length;i++){if(imgs[i].cmd=='alignLeft'){imgs[i].src=v=='left'?imgs[i].actSrc:imgs[i].inactSrc;}
else if(imgs[i].cmd=='alignCenter'){imgs[i].src=v=='center'?imgs[i].actSrc:imgs[i].inactSrc;}
else if(imgs[i].cmd=='alignRight'){imgs[i].src=v=='right'?imgs[i].actSrc:imgs[i].inactSrc;}}}}
function fontCommandSelected(e){if(!_editorOptions)return;var c=JsonDataBinding.getSender(e);if(c&&c.cmd){if(c.cmd=='indent'){var li=getSelectedObjByTag('li');if(LI_indent(li)){_editorOptions.pageChanged=true;}
return;}
else if(c.cmd=='dedent'){var li=getSelectedObjByTag('li');if(LI_dedent(li)){_editorOptions.pageChanged=true;}
return;}
else if(c.cmd=='moveup'){var li=getSelectedObjByTag('li');if(LI_moveup(li)){_editorOptions.pageChanged=true;}
return;}
else if(c.cmd=='movedown'){var li=getSelectedObjByTag('li');if(LI_movedown(li)){_editorOptions.pageChanged=true;}
return;}
else if(c.cmd=='alignLeft'){if(_editorOptions.selectedObject){_setElementStyleValue(_editorOptions.selectedObject,'textAlign','text-align','left');showtextAlign(_editorOptions.selectedObject);_editorOptions.pageChanged=true;}
return;}
else if(c.cmd=='alignCenter'){if(_editorOptions.selectedObject){_setElementStyleValue(_editorOptions.selectedObject,'textAlign','text-align','center');showtextAlign(_editorOptions.selectedObject);_editorOptions.pageChanged=true;}
return;}
else if(c.cmd=='alignRight'){if(_editorOptions.selectedObject){_setElementStyleValue(_editorOptions.selectedObject,'textAlign','text-align','right');showtextAlign(_editorOptions.selectedObject);_editorOptions.pageChanged=true;}
return;}
var iurl;if(_editorOptions.selectedHtml||(c.cmd=='createlink'&&_editorOptions.selectedObject)){if(typeof c.useUI!='undefined'){if(typeof c.value!='undefined'){_editorOptions.elementEditedDoc.execCommand(c.cmd,c.useUI,c.value);}
else{_editorOptions.elementEditedDoc.execCommand(c.cmd,c.useUI);}
captureSelection({target:_editorOptions.elementEditedDoc.activeElement});_editorOptions.pageChanged=true;}
else{if(c.cmd=='createlink'){var a;a=_createElement('a');a.href='#';insertNodeOverSelection(a,'a');if(_editorOptions.isEditingBody){var txt=a.textContent||a.innerText;if(txt&&txt.length>0){if(txt.length>30){txt=txt.substr(0,30);}
a.name=txt;refreshProperties();}}
else{a.savedhref=prompt('Enter hyperlink URL','');a.target='_blank';_editorOptions.selectedObject=null;selectEditElement(a);}}
else{if(_isFireFox){_editorOptions.elementEditedDoc.execCommand(c.cmd,null,false);}
else{if(_isOpera){if(_editorOptions.selectedObject)_editorOptions.selectedObject.contentEditable=true;}
_editorOptions.elementEditedDoc.execCommand(c.cmd);}
captureSelection({target:_editorOptions.elementEditedDoc.activeElement});}
_editorOptions.pageChanged=true;}}
else{if(typeof c.useUI=='undefined'){if(c.cmd=='createlink'){alert('Please select text to be used for hyper-text');}
else if(_editorOptions.selectedObject){try{var fw;if(c.cmd=='bold'){if(_editorOptions.selectedObject.tagName&&_editorOptions.selectedObject.tagName.toLowerCase()=='strong'){iurl=c.inactSrc;stripTag({target:{owner:_editorOptions.selectedObject}});}
else{if(_editorOptions.selectedObject.style.fontWeight=='bold'){fw='';}
else{fw='bold'}
_editorOptions.selectedObject.style.fontWeight=fw;}}
else if(c.cmd=='italic'){if(_editorOptions.selectedObject.tagName&&_editorOptions.selectedObject.tagName.toLowerCase()=='em'){iurl=c.inactSrc;stripTag({target:{owner:_editorOptions.selectedObject}});}
else{fw=_editorOptions.selectedObject.style.fontStyle;if(fw=='italic'||fw=='oblique'){fw='';}
else{fw='italic'}
_editorOptions.selectedObject.style.fontStyle=fw;}}
else if(c.cmd=='underline'){if(_editorOptions.selectedObject.tagName&&_editorOptions.selectedObject.tagName.toLowerCase()=='u'){iurl=c.inactSrc;stripTag({target:{owner:_editorOptions.selectedObject}});}
else{fw=_editorOptions.selectedObject.style.textDecoration;if(fw=='underline'){fw='';}
else{fw='underline'}
_editorOptions.selectedObject.style.textDecoration=fw;}}
else if(c.cmd=='strikethrough'){if(_editorOptions.selectedObject.tagName&&_editorOptions.selectedObject.tagName.toLowerCase()=='strike'){iurl=c.inactSrc;stripTag({target:{owner:_editorOptions.selectedObject}});}
else{fw=_editorOptions.selectedObject.style.textDecoration;if(fw=='line-through'){fw='';}
else{fw='line-through'}
_editorOptions.selectedObject.style.textDecoration=fw;}}
else if(c.cmd=='subscript'){if(_editorOptions.selectedObject.tagName&&_editorOptions.selectedObject.tagName.toLowerCase()=='sub'){iurl=c.inactSrc;stripTag({target:{owner:_editorOptions.selectedObject}});}}
else if(c.cmd=='superscript'){if(_editorOptions.selectedObject.tagName&&_editorOptions.selectedObject.tagName.toLowerCase()=='sup'){iurl=c.inactSrc;stripTag({target:{owner:_editorOptions.selectedObject}});}}
if(fw||fw==''){if(fw==''){iurl=c.inactSrc;}
else{iurl=c.actSrc;}
_editorOptions.styleChanged=true;showProperties(_editorOptions.selectedObject);showFontCommands();}
_editorOptions.pageChanged=true;}
catch(err){alert('Error execting operator "'+c.cmd+'". '+err.message);}}}}
if(iurl){c.src=iurl;}
else{if(!_isFireFox){if(_editorOptions.elementEditedDoc.queryCommandState(c.cmd)){c.src=c.actSrc;}
else{c.src=c.inactSrc;}}}}}
function refreshProperties(){if(_editorOptions&&_editorOptions.selectedObject){var c=_editorOptions.selectedObject;_editorOptions.selectedObject=null;selectEditElement(c);}}
function stripTag(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var node=c.owner;var parent=node.parentNode;var tag=(node.tagName?node.tagName.toLowerCase():'');if(tag=='table'||tag=='select'||tag=='optgroup'||tag=='option'){parent.removeChild(node);}
else{while(node.firstChild){parent.insertBefore(node.firstChild,node);}
parent.removeChild(node);}
selectEditElement(parent);if(_locatorList){_locatorList.options.length=0;}}}
function changeHeading(e,htag){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var node=c.owner;var h=_editorOptions.elementEditedDoc.createElement(htag);var parent=node.parentNode;parent.insertBefore(h,node);while(node.firstChild){h.appendChild(node.firstChild);}
parent.removeChild(node);selectEditElement(h);}}
function gotoChildByTag(e){var c0=JsonDataBinding.getSender(e);if(c0&&c0.owner&&c0.propDesc){var node=c0.owner;var cs=node.getElementsByTagName(c0.propDesc.name);var c;if(cs&&cs.length>0){c=cs[0];}
else{if(!c0.propDesc.notcreate){c=_editorOptions.elementEditedDoc.createElement(c0.propDesc.name);node.appendChild(c);}
else{c=c0;}}
selectEditElement(c);}}
function moveOutTag(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var node=c.owner;var parent=node.parentNode;if(parent!=_editorOptions.elementEditedDoc.body){var p=parent.parentNode;if(p){parent.removeChild(node);p.insertBefore(node,parent.nextSibling);_editorOptions.selectedObject=null;selectEditElement(node);}}}}
function getContainer(obj){var p=obj.parentNode;if(obj.tagName){var tn=obj.tagName.toLowerCase();if(tn=='td'||tn=='tr'||tn=='th'){while(p){if(p.tagName){tn=p.tagName.toLowerCase();if(tn=='table'){return p;}}
p=p.parentNode;}}
if(p.tagName){tn=p.tagName.toLowerCase();while(tn=='p'&&p){p=p.parentNode;if(p.tagName){tn=p.tagName.toLowerCase();}}}}
return p;}
function getObjectAbsoluteLocation(obj){if(obj&&obj.style){if(obj.style.position=='absolute'){return{x:obj.offsetLeft,y:obj.offsetTop};}
else{var objP=getContainer(obj);if(objP){var p=getObjectAbsoluteLocation(objP);if(p){return{x:obj.offsetLeft+p.x,y:obj.offsetTop+p.y};}
else{return{x:obj.offsetLeft,y:obj.offsetTop};}}
else{return{x:obj.offsetLeft,y:obj.offsetTop};}}}}
function getParentDesignObj(obj){if(obj.jsData&&obj.jsData.createSubEditor){var ed=obj.jsData.createSubEditor(obj,obj);if(ed){return ed;}}
var tag=obj.tagName.toLowerCase();if(tag=='li'){var p=obj.parentNode;while(p){if(p.jsData){if(p.jsData.typename=='treeview'){return createTreeViewItemPropDescs(p,obj);}}
p=p.parentNode;}}
return obj;}
function showSelectionMark(obj){if(!_editorOptions)return;var ap;var c=obj.subEditor?obj.obj:obj;ap=_editorOptions.client.getElementPosition.apply(_editorOptions.editorWindow,[c]);if(ap){if(_editorOptions.redbox&&!_editorOptions.redbox.parentNode){_appendChild(_editorOptions.elementEditedDoc.body,_editorOptions.redbox);}
_editorOptions.redbox.contentEditable=false;_editorOptions.redbox.style.left=ap.x+'px';_editorOptions.redbox.style.top=ap.y+'px';if(obj==_editorOptions.elementEditedDoc.body){_editorOptions.redbox.style.display='none';}
else{_editorOptions.redbox.style.width=8;_editorOptions.redbox.style.height=8;_editorOptions.redbox.style.display='block';_editorOptions.redbox.style.zIndex=_editorOptions.client.getPageZIndex.apply(_editorOptions.editorWindow,[c])+1;}}
_parentList.options.length=0;var tab='';var p=c;var jsData;while(p){jsData=p.jsData;if(jsData)
break;p=p.parentNode;}
p=obj;while(p){if(p.tagName&&p.tagName.toLowerCase()!='doctype'){if(p.hideParent||(jsData&&jsData.editable&&!jsData.editable(p))){p=p.parentNode;continue;}
var item=document.createElement('option');item.objvalue=getParentDesignObj(p);item.text=elementToString(item.objvalue);item.selected=false;addOptionToSelect(_parentList,item);item.selected=false;tab+='..';if(p.jsData&&p.jsData.isTop){break;}
p=p.parentNode;}
else if(p.subEditor){if(p.hideParent){p=p.parentNode;continue;}
var item=document.createElement('option');item.objvalue=p;item.text=elementToString(item.objvalue);item.selected=false;addOptionToSelect(_parentList,item);item.selected=false;if(p.isTop)
break;p=p.obj.parentNode;}
else{break;}}
_parentList.size=_parentList.options.length;_parentList.selectedIndex=-1;for(var i=0;i<_parentList.options.length;i++){_parentList.options[i].selected=false;}}
function getColumnCount(rowHolder){var cn=0;if(rowHolder.rows.length>0){for(var r=0;r<rowHolder.rows.length;r++){if(rowHolder.rows[r].cells.length>0){var c0=0;for(var c=0;c<rowHolder.rows[r].cells.length;c++){c0+=(rowHolder.rows[r].cells[c].colSpan?rowHolder.rows[r].cells[c].colSpan:1);}
if(c0>cn)
cn=c0;}}}
return cn;}
function getRowCount(rowHolder){return rowHolder.rows.length;}
function mapGridColumnIndexToTableCellIndex(gridCellIndex,row){var ci=0;for(var c=0;c<row.cells.length;c++){ci+=row.cells[c].colSpan?row.cells[c].colSpan:1;if(ci>gridCellIndex){return c;}}
return-1;}
function getVirtualRowPosition(map,tr){for(var r=0;r<map.length;r++){for(var c=0;c<map[r].length;c++){if(map[r][c].parentNode==tr){return{r:r,c:c};}}}}
function getVirtualCellPosition(map,td){for(var r=0;r<map.length;r++){for(var c=0;c<map[r].length;c++){if(map[r][c]==td){return{r:r,c:c};}}}}
function getVirtualColumnIndex(td,maprow){for(var c=0;c<maprow.length;c++){if(maprow[c]==td){return c;}}
return-1;}
function getMapRowSpan(map,r){var ri=0;for(var c=0;c<map[r].length;c++){if((map[r][c].rowSpan?map[r][c].rowSpan:1)>ri){ri=(map[r][c].rowSpan?map[r][c].rowSpan:1);}}
return ri;}
function getNextVirtualColumnIndex(td,maprow){var f;for(var c=0;c<maprow.length;c++){if(f){if(maprow[c]!=td){return c;}}
else if(maprow[c]==td){f=true;}}
return-1;}
function _getTableMap(rowHolder){var vrCount=getRowCount(rowHolder);var vcCount=getColumnCount(rowHolder);var vcells=new Array(vrCount);var r,c;for(r=0;r<vrCount;r++){vcells[r]=new Array(vcCount);for(c=0;c<vcCount;c++){vcells[r][c]=null;}}
for(r=0;r<rowHolder.rows.length;r++){var ci=0;for(c=0;c<rowHolder.rows[r].cells.length;c++){var cr=typeof rowHolder.rows[r].cells[c].colSpan!='undefined'?(rowHolder.rows[r].cells[c].colSpan>0?rowHolder.rows[r].cells[c].colSpan:1):1;var rr=typeof rowHolder.rows[r].cells[c].rowSpan!='undefined'?(rowHolder.rows[r].cells[c].rowSpan>0?rowHolder.rows[r].cells[c].rowSpan:1):1;while(ci<vcCount&&vcells[r][ci]!=null){ci++;}
if(ci+cr+(rowHolder.rows[r].cells.length-c-1)>vcCount){cr=vcCount-ci-(rowHolder.rows[r].cells.length-c-1);setColSpan(rowHolder.rows[r].cells[c],cr);}
if(rr+r>vrCount){rr=vrCount-r;setRowSpan(rowHolder.rows[r].cells[c],rr);}
for(var i=ci;i<ci+cr;i++){for(var j=r;j<r+rr&&j<vcells.length;j++){if(i<vcells[j].length){vcells[j][i]=rowHolder.rows[r].cells[c];}}}
ci+=cr;}}
return vcells;}
function getTDbyVirtualPosition(rowHolder,vr,vc,vrCount,vcCount){if(!vrCount)vrCount=getRowCount(rowHolder);if(!vcCount)vcCount=getColumnCount(rowHolder);if(vrCount>0&&vcCount>0){var vrc=0;for(var r=0;r<vrCount;r++){if(rowHolder.rows[r].cells.length>0){vrc+=rowHolder.rows[r].cells[0].rowSpan?rowHolder.rows[r].cells[0].rowSpan:1;}
else{vrc+=1;}
if(vrc>=vr){var row=rowHolder.rows[r];var vcc=0;for(var c=0;c<row.cells.length;c++){vcc+=row.cells[c].colSpan?row.cells[c].colSpan:1;if(vcc>=vc){return row.cells[c];}}}}}
return null;}
function _verifyTableStruct(tbl){var n,r,c,colCount;var ncolCount=0;for(n=0;n<tbl.children.length;n++){if(tbl.children[n].rows){for(r=0;r<tbl.children[n].rows.length;r++){if(tbl.children[n].rows[r].cells&&tbl.children[n].rows[r].cells.length>0){colCount=0;for(c=0;c<tbl.children[n].rows[r].cells.length;c++){colCount+=(tbl.children[n].rows[r].cells[c].colSpan?tbl.children[n].rows[r].cells[c].colSpan:1);}
if(colCount>ncolCount)
ncolCount=colCount;}}}}
if(ncolCount>0){for(n=0;n<tbl.children.length;n++){if(tbl.children[n].rows){for(r=0;r<tbl.children[n].rows.length;r++){if(tbl.children[n].rows[r].cells&&tbl.children[n].rows[r].cells.length>0){colCount=0;for(c=0;c<tbl.children[n].rows[r].cells.length;c++){colCount+=(tbl.children[n].rows[r].cells[c].colSpan?tbl.children[n].rows[r].cells[c].colSpan:1);}
if(colCount<ncolCount){colCount=ncolCount-colCount;var n1=tbl.children[n].rows[r].cells&&tbl.children[n].rows[r].cells.length-1;tbl.children[n].rows[r].cells[n1].colSpan=(tbl.children[n].rows[r].cells[n1].colSpan?tbl.children[n].rows[r].cells[n1].colSpan:1)+colCount;}}}}}}}
function _refreshProperties(){if(_editorOptions){if(_editorOptions.propertiesOwner&&_editorOptions.propertiesOwner.tagName&&_editorOptions.propertiesOwner.tagName.toLowerCase()=='html'){showProperties(_editorOptions.propertiesOwner);}}}
function _redisplayProperties(){if(_editorOptions&&_editorOptions.propertiesOwner){showProperties(_editorOptions.propertiesOwner);}}
function _togglePropertiesWindow(){if(_divProp.expanded){_divProp.fullHeight=_divProp.style.height;_tableProps.style.display='none';_editorDiv.style.display='none';if(_divSelectElement){hideSelector();}
_divProp.style.height=(_titleTable.offsetHeight)+'px';_resizer.style.display='none';_nameresizer.style.display='none';_imgExpand.src='/libjs/expand_res.png';}
else{_divProp.style.height=_divProp.fullHeight;_tableProps.style.display='';_editorDiv.style.display='';_imgExpand.src='/libjs/expand_min.png';}
_divProp.expanded=!_divProp.expanded;_showResizer();if(_divProp.expanded){adjustSizes();}}
var EDIT_NONE=0;var EDIT_TEXT=1;var EDIT_NUM=2;var EDIT_BOOL=3;var EDIT_COLOR=4;var EDIT_ENUM=5;var EDIT_PARENT=6;var EDIT_CMD=7;var EDIT_CHILD=8;var EDIT_PROPS=9;var EDIT_CMD2=10;var EDIT_CMD3=11;var EDIT_MENUM=12;var EDIT_DEL=13;var EDIT_NODES=14;var EDIT_GO=15;var EDIT_CUST=16;var propCatColors;var PROP_BK=1,PROP_BK_COLOR='#FAFAFF';var PROP_BORDER=2,PROP_BORDER_COLOR='#FAFAFA';var PROP_BOX=3,PROP_BOX_COLOR='#FFFAFA';var PROP_FONT=4,PROP_FONT_COLOR='#e2f7f1';var PROP_MARGIN=5,PROP_MARGIN_COLOR='#F0FFFF';var PROP_MULCOL=6,PROP_MULCOL_COLOR='#F5F5DC';var PROP_POS=7,PROP_POS_COLOR='#FAFFFA';var PROP_TEXT=8,PROP_TEXT_COLOR='#F5F5F5';var PROP_GENERAL=9,PROP_GENERAL_COLOR='#F0FFFF';var PROP_SIZELOC=10,PROP_SIZELOC_COLOR='#F5FFF0';var PROP_PAGECLASSES=11,PROP_PAGECLASSES_COLOR='#F0F8FF';var PROP_CUST1=12,PROP_CUST1_COLOR='#FFFEEB';function getPropCatColor(cat){if(!propCatColors){propCatColors=[PROP_BK_COLOR,PROP_BORDER_COLOR,PROP_BOX_COLOR,PROP_FONT_COLOR,PROP_MARGIN_COLOR,PROP_MULCOL_COLOR,PROP_POS_COLOR,PROP_TEXT_COLOR,PROP_GENERAL_COLOR,PROP_SIZELOC_COLOR,PROP_PAGECLASSES_COLOR,PROP_CUST1_COLOR];}
return propCatColors[cat-1];}
function _showEditor(display){_divProp.style.display=display;}
function onselChangeForProperties(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner&&c.selectedIndex>=0){var op=c.owner.options[c.selectedIndex];if(op){selectEditElement(op);}}}
function showListItems(sel,trCmd){if(_isFireFox||_isOpera)return;var op0;while(sel){if(sel.tagName){if(sel.tagName.toLowerCase()=='option'){op0=sel;}
else if(sel.tagName.toLowerCase()=='select'){break;}}
sel=sel.parentNode;}
if(sel){var sel2=document.createElement('select');sel2.size=1;trCmd.appendChild(sel2);var selIdx=-1;for(var i=0;i<sel.options.length;i++){var op=document.createElement('option');op.text=sel.options[i].text;if(sel.options[i]==op0){op.selected=false;}
addOptionToSelect(sel2,op);}
sel2.owner=sel;sel2.selectedIndex=selIdx;JsonDataBinding.AttachEvent(sel2,"onchange",onselChangeForProperties);}}
function deleteElement(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var p=c.owner.parentNode;if(p){var o=c.owner;if(p.tagName=='a'||p.tagName=='A'){if(p.parentNode&&p.parentNode.tagName&&p.parentNode.tagName.toLowerCase()=='svg'){o=p;p=p.parentNode;}}
if(_editorOptions){_editorOptions.deleted={parent:p,obj:o,next:o.nextSibling};}
p.removeChild(o);selectEditElement(p);_imgUndel.src='/libjs/undel.png';if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}}}
function gotoElement(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){_editorOptions.selectedObject=null;selectEditElement(c.owner);}}
function gotoPageHead(e){for(var i=0;i<_parentList.options.length;i++){var obj=_parentList.options[i].objvalue;if(obj&&obj.tagName&&obj.tagName.toLowerCase()=='head'){_parentList.selectedIndex=i;showProperties(obj);return;}}
gotoChildByTag(e);}
function getWebAddress(h){if(_editorOptions&&_editorOptions.elementEditedWindow){var u=_editorOptions.client.getPageAddress.apply(_editorOptions.elementEditedWindow,[false]);return u;}}
function getMetaData(h,name){var head=h.getElementsByTagName('head')[0];var mts=head.getElementsByTagName('meta');if(mts){name=name.toLowerCase();for(var i=0;i<mts.length;i++){var nm;if(_isIE){nm=mts[i].getAttribute('Name');}
else{nm=mts[i].getAttribute('name');}
if(nm)nm=nm.toLowerCase();if(nm==name){return mts[i].getAttribute('content');}}}}
function setContentNoCache(h){var head=h.getElementsByTagName('head')[0];var mts=head.getElementsByTagName('meta');if(mts){for(var i=0;i<mts.length;i++){var nm=mts[i].getAttribute('http-equiv');if(typeof nm!='undefined'&&nm!=null&&nm.toLowerCase()=='pragma'){mts[i].setAttribute('content','NO-CACHE');return;}
else{nm=mts[i].getAttribute('content');if(nm&&nm.toLowerCase()=='no-cache'){return;}}}}
var m=_createElement('meta');_appendChild(head,m);m.setAttribute('http-equiv','PRAGMA');m.setAttribute('content','NO-CACHE');}
function setContentType(h,val){var head=h.getElementsByTagName('head')[0];var mts=head.getElementsByTagName('meta');if(mts){for(var i=0;i<mts.length;i++){var nm=mts[i].getAttribute('http-equiv');if(typeof nm!='undefined'&&nm!=null&&nm.toLowerCase()=='content-type'){mts[i].setAttribute('content',val);return;}
else{nm=mts[i].getAttribute('content');if(nm&&nm.toLowerCase()=='text/html; charset=utf-8'){return;}}}}
var m=_createElement('meta');_appendChild(head,m);m.setAttribute('http-equiv','Content-Type');m.setAttribute('content',val);}
function setIECompatible(h){var head=h.getElementsByTagName('head')[0];var mts=head.getElementsByTagName('meta');if(mts){for(var i=0;i<mts.length;i++){var nm=mts[i].getAttribute('http-equiv');if(typeof nm!='undefined'&&nm!=null&&nm.toLowerCase()=='x-ua-compatible'){mts[i].setAttribute('content','IE=edge');return;}
else{nm=mts[i].getAttribute('content');if(nm&&nm.toLowerCase()=='ie=edge'){return;}}}}
var m=_createElement('meta');_appendChild(head,m);m.setAttribute('http-equiv','X-UA-Compatible');m.setAttribute('content','IE=edge');}
function setMetaData(h,name,val){name=name.toLowerCase();var head=h.getElementsByTagName('head')[0];var mts=head.getElementsByTagName('meta');if(mts){for(var i=0;i<mts.length;i++){var nm;if(_isIE){nm=mts[i].getAttribute('Name')||mts[i].name;}
else{nm=mts[i].getAttribute('name');}
if(nm)nm=nm.toLowerCase();if(nm==name){mts[i].setAttribute('content',val);return;}}}
var m=_createElement('meta');_appendChild(head,m);if(_isIE){m.setAttribute('Name',name);m.name=name;}
else{m.setAttribute('name',name);}
m.setAttribute('content',val);}
function addmeta(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var head=c.owner;var m=_createElement('meta');_appendChild(head,m);selectEditElement(m);}}
function addlink(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var head=c.owner;var m=_createElement('link');_appendChild(head,m);selectEditElement(m);}}
function addcss(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var head=c.owner;var m=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['link']);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[head,m]);m.setAttribute('rel','stylesheet');m.setAttribute('type','text/css');selectEditElement(m);}}
function addscript(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var head=c.owner;var m=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['script']);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[head,m]);selectEditElement(m);}}
function addOptionGroup(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var sel=c.owner;while(sel){if(sel.tagName){if(sel.tagName.toLowerCase()=='select'){break;}}
sel=sel.parentNode;}
if(sel){var op=_createElement('optgroup');op.label='option group';_appendChild(sel,op);selectEditElement(op);}}}
function addpolygon(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var polygon=_createSVGElement('polygon');a.appendChild(polygon);polygon.setAttribute('points','2,2 60,2 65,10 30,30');polygon.style.fill='yellow';selectEditElement(polygon);}}}
function addpolyline(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var polygon=_createSVGElement('polyline');a.appendChild(polygon);polygon.setAttribute('points','2,2 60,2 65,10 30,30');polygon.setAttribute('stroke','black');polygon.setAttribute('stroke-width','1');polygon.style.fill='none';selectEditElement(polygon);}}}
function addrect(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var rect=_createSVGElement('rect');a.appendChild(rect);rect.setAttribute('x','2');rect.setAttribute('y','2');rect.setAttribute('width','100');rect.setAttribute('height','30');rect.style.fill='blue';selectEditElement(rect);}}}
function addtext(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var rect=_createSVGElement('text');a.appendChild(rect);rect.setAttribute('x','50');rect.setAttribute('y','50');rect.textContent='Hello World!';selectEditElement(rect);}}}
function addcircle(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var rect=_createSVGElement('circle');a.appendChild(rect);rect.setAttribute('cx','60');rect.setAttribute('cy','60');rect.setAttribute('r','50');rect.style.fill='green';selectEditElement(rect);}}}
function addellipse(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var rect=_createSVGElement('ellipse');a.appendChild(rect);rect.setAttribute('cx','60');rect.setAttribute('cy','60');rect.setAttribute('rx','50');rect.setAttribute('ry','30');rect.style.fill='green';selectEditElement(rect);}}}
function addline(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var svg=c.owner;if(svg){var a=_createSVGElement('a');a.hideParent=true;svg.appendChild(a);var rect=_createSVGElement('line');a.appendChild(rect);rect.setAttribute('x1','10');rect.setAttribute('y1','10');rect.setAttribute('x2','50');rect.setAttribute('y2','50');rect.setAttribute('stroke','black');rect.setAttribute('stroke-width','1');selectEditElement(rect);}}}
function moveshapeleft(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var shape=c.owner;if(shape){if(shape.tagName&&shape.tagName.toLowerCase()=='svg'){shape.style.position='absolute';var rect=shape.getBoundingClientRect()
shape.style.left=(rect.left-HtmlEditor.svgshapemovegap)+'px';}
else if(shape.jsData){shape.jsData.moveleft();}}}}
function moveshaperight(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var shape=c.owner;if(shape){if(shape.tagName&&shape.tagName.toLowerCase()=='svg'){shape.style.position='absolute';var rect=shape.getBoundingClientRect()
shape.style.left=(rect.left+HtmlEditor.svgshapemovegap)+'px';}
else if(shape.jsData){shape.jsData.moveright();}}}}
function moveshapeup(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var shape=c.owner;if(shape){if(shape.tagName&&shape.tagName.toLowerCase()=='svg'){shape.style.position='absolute';var rect=shape.getBoundingClientRect()
shape.style.top=(rect.top-HtmlEditor.svgshapemovegap)+'px';}
else if(shape.jsData){shape.jsData.moveup();}}}}
function moveshapedown(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var shape=c.owner;if(shape){if(shape.tagName&&shape.tagName.toLowerCase()=='svg'){shape.style.position='absolute';var rect=shape.getBoundingClientRect()
shape.style.top=(rect.top+HtmlEditor.svgshapemovegap)+'px';}
else if(shape.jsData){shape.jsData.movedown();}}}}
function addOption(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var sel=c.owner;var og;while(sel){if(sel.tagName){if(sel.tagName.toLowerCase()=='select'){break;}
if(sel.tagName.toLowerCase()=='optgroup'){og=sel;break;}}
sel=sel.parentNode;}
if(sel||og){var op=_createElement('option');if(og){sel=og.parentNode;_appendChild(og,op);if(sel&&sel.tagName&&sel.tagName.toLowerCase()=='select'){op.text='new option '+sel.options.length;}
else{op.text='new option '+op.childNodes.length;}
op.value=op.text;}
else{op.text='new option '+sel.options.length;op.value=op.text;addOptionToSelect(sel,op);}
selectEditElement(op);}}}
function addDefinition(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var dl=c.owner;var dt=_editorOptions.elementEditedDoc.createElement('dt');dt.innerHTML='new definition';dl.appendChild(dt);var dd=_editorOptions.elementEditedDoc.createElement('dd');dd.innerHTML='new description';dl.appendChild(dd);}}
function addObjectParameter(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var obj=c.owner;var pa=_editorOptions.elementEditedDoc.createElement('param');var nm='p'+Math.floor(Math.random()*65536);pa.name=nm;pa.id=nm;obj.appendChild(pa);selectEditElement(pa);}}
function addtheader(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var tbl=c.owner;var h;var tag;var tbd;for(var i=0;i<tbl.children.length;i++){tag=tbl.children[i].tagName.toLowerCase();if(tag=='tbody')
tbd=tbl.children[i];else if(tag=='thead'){h=tbl.children[i];break;}
else if(tag=='tfoot'){if(!tbd)
tbd=tbl.children[i];}}
if(!h&&tbd){var map=_getTableMap(tbd);if(map.length>0){var cells=map[0];h=_editorOptions.elementEditedDoc.createElement('thead');tbl.appendChild(h);var tr=_editorOptions.elementEditedDoc.createElement('tr');h.appendChild(tr);if(cells){var lastTD=null;for(var ci=0;ci<cells.length;ci++){if(lastTD!=cells[ci]){lastTD=cells[ci];var th=_editorOptions.elementEditedDoc.createElement('th');th.innerHTML="Column "+(ci+1);setColSpan(th,cells[ci].colSpan);tr.appendChild(th);}}}}}
if(h){selectEditElement(h);}}}
function addtfooter(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var tbl=c.owner;var h;var tag;var tbd;for(var i=0;i<tbl.children.length;i++){tag=tbl.children[i].tagName.toLowerCase();if(tag=='tbody')
tbd=tbl.children[i];else if(tag=='thead'){if(!tbd)
tbd=tbl.children[i];}
else if(tag=='tfoot'){h=tbl.children[i];break;}}
if(!h&&tbd){var map=_getTableMap(tbd);if(map.length>0){var cells=map[map.length-1];h=_editorOptions.elementEditedDoc.createElement('tfoot');tbl.appendChild(h);var tr=_editorOptions.elementEditedDoc.createElement('tr');h.appendChild(tr);if(cells){var lastTD=null;for(var ci=0;ci<cells.length;ci++){if(cells[ci]!=lastTD){lastTD=cells[ci];var th=_editorOptions.elementEditedDoc.createElement('th');th.innerHTML="Column "+(ci+1);setColSpan(th,cells[ci].colSpan);tr.appendChild(th);}}}}}
if(h){selectEditElement(h);}}}
function addMapArea(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var map=c.owner;var area=_editorOptions.elementEditedDoc.createElement('area');map.appendChild(area);selectEditElement(area);}}
function addLegend(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var fs=c.owner;var lg;var lgs=fs.getElementsByTagName('legend');if(lgs&&lgs.length>0){lg=lgs[0];if(!lg.innerHTML||lg.innerHTML.length==0){lg.innerHTML='<b>group</b>';}}
else{lg=_editorOptions.elementEditedDoc.createElement('legend');lg.innerHTML='<b>group</b>';fs.appendChild(lg);}
selectEditElement(lg);}}
function isSameColSpan(td1,td2){if(td1.colSpan&&td2.colSpan){return(td1.colSpan==td2.colSpan);}
if(!td1.colSpan&&!td2.colSpan){return true;}
return false;}
function isSameRowSpan(td1,td2){if(td1.rowSpan&&td2.rowSpan){return(td1.rowSpan==td2.rowSpan);}
if(!td1.rowSpan&&!td2.rowSpan){return true;}
return false;}
function showcolumnattrs(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;var tbl=td.parentNode;while(tbl&&tbl.tagName&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
if(tbl){var cl;var cidx=td.cellIndex;var thd,tbd,cols,cg,tag,i;for(i=0;i<tbl.children.length;i++){if(tbl.children[i]&&tbl.children[i].tagName){tag=tbl.children[i].tagName.toLowerCase();if(tag=='colgroup'){cg=tbl.children[i];var cols0=cg.getElementsByTagName('col');cols=new Array();for(var k=0;k<cols0.length;k++){cols.push(cols0[k]);}
break;}
else if(tag=='thead'){thd=tbl.children[i];break;}
else if(tag=='tbody'){tbd=tbl.children[i];break;}}}
if(!cg){cols=new Array();for(i=0;i<tbl.children.length;i++){if(tbl.children[i]&&tbl.children[i].tagName&&tbl.children[i].tagName.toLowerCase()=='col'){cols.push(tbl.children[i]);}}
cg=_editorOptions.elementEditedDoc.createElement("colgroup");if(thd){tbl.insertBefore(cg,thd);}
else{if(!tbd){tbd=JsonDataBinding.getTableBody(table);}
tbl.insertBefore(cg,tbd);}
for(i=0;i<cols.length;i++){cg.appendChild(cols[i]);}}
while(cidx>cols.length-1){cl=_editorOptions.elementEditedDoc.createElement("col");cg.appendChild(cl);cols.push(cl);}
cl=cols[cidx];cl.cellIndex=cidx;showProperties(cl);}}}
function removeTableRow(tr){if(tr){var p=tr.parentNode;if(p&&p.rows){var map=_getTableMap(p);var vp=getVirtualRowPosition(map,tr);var r=vp.r;var c;for(c=0;c<map[r].length;c++){if(map[r][c].rowSpan>1){alert("This row has merged cells with the next row and cannot be removed. You may use cell-split operations first and then try it again. You may also try a row-merge operation instead.");return;}}
if(r>0){var ri,i;var rowchecked=[];var tds=[];for(ri=0;ri<r;ri++){var checked=false;for(i=0;i<rowchecked.length;i++){if(rowchecked[i].row==map[ri]){checked=true;break;}}
if(!checked){var row={row:map[ri],cells:[]};rowchecked.push(row);for(c=0;c<map[ri].length;c++){checked=false;for(i=0;i<row.cells.length;i++){if(row.cells[i]==map[ri][c]){checked=true;break;}}
if(!checked){row.cells.push(map[ri][c]);if(map[ri][c].rowSpan+ri>r){tds.push(map[ri][c]);}}}}}
for(c=0;c<tds.length;c++){setRowSpan(tds[c],tds[c].rowSpan-1);}}
p.removeChild(tr);}}}
function rowSpanCleanup(table){var map,span,spans,c,r0,r=0;while(r<table.rows.length){if(table.rows[r].cells.length==0){if(r>0){span=table.rows.length;map=_getTableMap(table);r0=r-1;for(c=0;c<map[r0].length;c++){if((map[r0][c].rowSpan?map[r0][c].rowSpan:1)<span){span=(map[r0][c].rowSpan?map[r0][c].rowSpan:1);}
if(span<2){break;}}
if(span>1){var lastTD=null;var lastspan=1;spans=new Array(map[r0].length);for(c=0;c<map[r0].length;c++){if(map[r0][c]==lastTD){spans[c]=lastspan;}
else{lastTD=map[r0][c];lastspan=map[r0][c].rowSpan-1;spans[c]=lastspan;}}
lastTD=null;for(c=0;c<map[r0].length;c++){if(map[r0][c]!=lastTD){lastTD=map[r0][c];setRowSpan(map[r0][c],spans[c]);}}}}
table.deleteRow(r);}
else{r++;}}}
function colSpanCleanup(table){var map,span,spans,c=0,r0,r=0;map=_getTableMap(table);if(map.length>0){var colCount=map[0].length;while(c<colCount){span=colCount;for(r=0;r<map.length;r++){if((map[r][c].colSpan?map[r][c].colSpan:1)<span){span=(map[r][c].colSpan?map[r][c].colSpan:1);}
if(span<=1){break;}}
if(span>1){var lastTD=null;var lastspan=1;spans=new Array(map.length);span--;for(r=0;r<map.length;r++){if(map[r][c]!=lastTD){lastTD=map[r][c];lastspan=map[r][c].colSpan-span;}
spans[r]=lastspan;}
lastTD=null;for(r=0;r<map.length;r++){if(map[r][c]!=lastTD){lastTD=map[r][c];setColSpan(map[r][c],spans[r]);}}
c+=span;}
c++;}}}
function tableSpanCleanup(table){var i,tbl,tag;tbl=table;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
if(tbl){for(i=0;i<tbl.children.length;i++){tag=tbl.children[i].tagName.toLowerCase();if(tag=='thead'){rowSpanCleanup(tbl.children[i]);}
else if(tag=='tfoot'){rowSpanCleanup(tbl.children[i]);}
else if(tag=='tbody'){rowSpanCleanup(tbl.children[i]);}}}
if(tbl){colSpanCleanup(tbl);for(r=0;r<tbl.rows.length;r++){for(c=0;c<tbl.rows[r].cells.length;c++){span=tbl.rows[r].cells[c].getAttribute('colspan');if(span=='1'){tbl.rows[r].cells[c].removeAttribute('colspan');}
span=tbl.rows[r].cells[c].getAttribute('rowspan');if(span=='1'){tbl.rows[r].cells[c].removeAttribute('rowspan');}}}}}
function setRowSpan(td,rs){if(rs<=1)
td.removeAttribute('rowspan');else
td.rowSpan=rs;}
function setColSpan(td,cs){if(cs<=1)
td.removeAttribute('colspan');else
td.colSpan=cs;}
function mergeRows(e,above){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var tr=c.owner;if(tr){var p=tr.parentNode;while(p){if(p.rows){break;}
p=p.parentNode;}
if(p){var map=_getTableMap(p);var vp=getVirtualRowPosition(map,tr);var rowIndex=vp.r+getMapRowSpan(map,vp.r)-1;if((above&&rowIndex>0)||(!above&&rowIndex<p.rows.length-1)){var i,csp0,csp1,rsp0,rsp1;var canMerge=true;var r0;if(above){r0=rowIndex-1;}
else{r0=rowIndex;rowIndex=r0+1;}
for(i=0;i<map[r0].length;i++){csp0=1;csp1=1;if(typeof map[r0][i].colSpan!='undefined')csp0=map[r0][i].colSpan;if(typeof map[rowIndex][i].colSpan!='undefined')csp1=map[rowIndex][i].colSpan;if(csp0!=csp1){canMerge=false;break;}}
if(canMerge){var data=new Array();for(i=0;i<map[r0].length;i++){var td0=map[r0][i];var td1=map[rowIndex][i];var tr1=map[rowIndex][i].parentNode;var cvi=getVirtualColumnIndex(td1,map[rowIndex]);rsp0=1;rsp1=1;if(typeof map[r0][i].rowSpan!='undefined')rsp0=map[r0][i].rowSpan;if(typeof map[rowIndex][i].rowSpan!='undefined')rsp1=map[rowIndex][i].rowSpan;rsp0+=rsp1;data.push({td0:td0,td1:td1,tr1:tr1,cvi:cvi,rsp0:rsp0,rsp1:rsp1,html:td0.innerHTML+td1.innerHTML});}
for(i=0;i<data.length;i++){setRowSpan(data[i].td0,data[i].rsp0);data[i].td0.innerHTML=data[i].html;data[i].td1.parentNode.deleteCell(data[i].td1.cellIndex);}
tableSpanCleanup(p);}
else{alert('The two rows have different column spans and cannot be merged.');}}}}}}
function mergeRowAbove(e){mergeRows(e,true);}
function mergeRowBelow(e){mergeRows(e,false);}
function addRowAbove(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;var row;if(td.cells){row=td;}
else{row=td.parentNode;}
if(row&&row.parentNode){var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualRowPosition(map,row);var rIndex=vp.r;var nr=rowParent.insertRow(rIndex);var lastTD=null;for(var i=0;i<map[rIndex].length;i++){if(map[rIndex][i]!=lastTD){lastTD=map[rIndex][i];var isCross=false;if(rIndex>0&&rIndex<map.length){if(map[rIndex][i]==map[rIndex-1][i]){isCross=true;}}
if(isCross){setRowSpan(map[rIndex][i],(map[rIndex][i].rowSpan?map[rIndex][i].rowSpan:1)+1);}
else{var tdn=_editorOptions.elementEditedDoc.createElement("td");tdn.innerHTML="cell";setColSpan(tdn,map[rIndex][i].colSpan);nr.appendChild(tdn);}}}
showSelectionMark(_editorOptions.selectedObject);}}}
function addRowBelow(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;var row;var dr,i;if(td.cells){row=td;dr=1;for(i=0;i<row.cells.length;i++){if((row.cells[i].rowSpan?row.cells[i].rowSpan:1)>dr){dr=row.cells[i].rowSpan?row.cells[i].rowSpan:1;}}}
else{dr=(td.rowSpan?td.rowSpan:1);row=td.parentNode;}
if(row&&row.parentNode){var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualRowPosition(map,row);var rIndex=vp.r+dr-1;var nr=rowParent.insertRow(rIndex+1);var lastTD=null;for(i=0;i<map[rIndex].length;i++){if(map[rIndex][i]!=lastTD){lastTD=map[rIndex][i];var isCross=false;if(rIndex<map.length-1){if(map[rIndex][i]==map[rIndex+1][i]){isCross=true;}}
if(isCross){setRowSpan(map[rIndex][i],(map[rIndex][i].rowSpan?map[rIndex][i].rowSpan:1)+1);}
else{var tdn=_editorOptions.elementEditedDoc.createElement("td");tdn.innerHTML="cell";setColSpan(tdn,map[rIndex][i].colSpan);nr.appendChild(tdn);}}}
showSelectionMark(_editorOptions.selectedObject);}}}
function addColumnToLeft(holder,map,colIdx){var lastTD=null;for(var r=0;r<map.length;r++){if(map[r][colIdx]!=lastTD){lastTD=map[r][colIdx];var isCross=false;if(colIdx>0&&colIdx<map[r].length-1){if(map[r][colIdx]==map[r][colIdx-1]){if(map[r][colIdx]==map[r][colIdx+1]){isCross=true;}}}
if(isCross){setColSpan(map[r][colIdx],(map[r][colIdx].colSpan?map[r][colIdx].colSpan:0)+1);}
else{var cidx=mapGridColumnIndexToTableCellIndex(colIdx,holder.rows[r]);var tdn=holder.rows[r].insertCell(cidx);tdn.innerHTML="cell";setRowSpan(tdn,map[r][colIdx].rowSpan);}}}}
function addColumnLeft(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);var ri=vp.r;var colIdx=vp.c;addColumnToLeft(rowParent,map,colIdx);var tbl=rowParent;while(tbl.tagName){if(tbl.tagName.toLowerCase()=='table'){break;}
tbl=tbl.parentNode;}
var i;var holders=tbl.getElementsByTagName('tbody');if(holders&&holders.length>0){for(i=0;i<holders.length;i++){if(holders[i]!=rowParent){map=_getTableMap(holders[i]);addColumnToLeft(holders[i],map,colIdx);}}}
holders=tbl.getElementsByTagName('thead');if(holders&&holders.length>0){for(i=0;i<holders.length;i++){if(holders[i]!=rowParent){map=_getTableMap(holders[i]);addColumnToLeft(holders[i],map,colIdx);}}}
holders=tbl.getElementsByTagName('tfoot');if(holders&&holders.length>0){for(i=0;i<holders.length;i++){if(holders[i]!=rowParent){map=_getTableMap(holders[i]);addColumnToLeft(holders[i],map,colIdx);}}}
showSelectionMark(_editorOptions.selectedObject);}}}
function addColumnToRight(holder,map,colIdx){var lastTD=null;for(var r=0;r<map.length;r++){if(map[r][colIdx]!=lastTD){lastTD=map[r][colIdx];var isCross=false;if(colIdx>0&&colIdx<map[r].length-1){if(map[r][colIdx]==map[r][colIdx-1]){if(map[r][colIdx]==map[r][colIdx+1]){isCross=true;}}}
if(isCross){setColSpan(map[r][colIdx],map[r][colIdx].colSpan+1);}
else{var tn=map[r][colIdx];var tnci=tn.cellIndex?tn.cellIndex:0;var tdn=tn.parentNode.insertCell(tnci+1);tdn.innerHTML="cell";setRowSpan(tdn,tn.rowSpan);}}}}
function addColumnRight(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);var ri=vp.r;var colIdx=vp.c;addColumnToRight(rowParent,map,colIdx);var tbl=rowParent;while(tbl.tagName){if(tbl.tagName.toLowerCase()=='table'){break;}
tbl=tbl.parentNode;}
var i;var holders=tbl.getElementsByTagName('tbody');if(holders&&holders.length>0){for(i=0;i<holders.length;i++){if(holders[i]!=rowParent){map=_getTableMap(holders[i]);addColumnToRight(holders[i],map,colIdx);}}}
holders=tbl.getElementsByTagName('thead');if(holders&&holders.length>0){for(i=0;i<holders.length;i++){if(holders[i]!=rowParent){map=_getTableMap(holders[i]);addColumnToRight(holders[i],map,colIdx);}}}
holders=tbl.getElementsByTagName('tfoot');if(holders&&holders.length>0){for(i=0;i<holders.length;i++){if(holders[i]!=rowParent){map=_getTableMap(holders[i]);addColumnToRight(holders[i],map,colIdx);}}}
showSelectionMark(_editorOptions.selectedObject);}}}
function mergeColumnLeft(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var colIdx=td.cellIndex?td.cellIndex:0;if(colIdx>0){var canMerge=true;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);var rIndex=vp.r;var td0=row.cells[colIdx-1];var vci=vp.c;if(rIndex>0&&map[rIndex][vci-1]==map[rIndex-1][vci-1]){canMerge=false;}
else if(map[rIndex][vci-1].rowSpan!=map[rIndex][vci].rowSpan){canMerge=false;}
if(canMerge){var cp=td.colSpan?td.colSpan:1;var hl=td.innerHTML;row.deleteCell(colIdx);setColSpan(td0,cp+(td0.colSpan?td0.colSpan:1));td0.innerHTML=td0.innerHTML+hl;tableSpanCleanup(rowParent);docClick({target:td0});showSelectionMark(td0);}
else{alert('The two columns have different row spans and cannot be merged.');}}}}}
function mergeColumnRight(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var colIdx=td.cellIndex?td.cellIndex:0;if(colIdx>=0&&colIdx<row.cells.length-1){var canMerge=true;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);var rIndex=vp.r;var cidx=vp.c;var nextcidx=getNextVirtualColumnIndex(td,map[rIndex]);if(nextcidx<0){canMerge=false;}
else if(rIndex>0&&map[rIndex][nextcidx]==map[rIndex-1][nextcidx]){canMerge=false;}
else{if(!isSameRowSpan(map[rIndex][nextcidx],map[rIndex][cidx])){canMerge=false;}}
if(canMerge){var td1=row.cells[colIdx+1];if(td.colSpan){setColSpan(td,td1.colSpan+td.colSpan);}
else{setColSpan(td,td1.colSpan+1);}
td.innerHTML=td.innerHTML+td1.innerHTML;row.deleteCell(colIdx+1);tableSpanCleanup(rowParent);docClick({target:td});}
else{alert('The two columns have different row spans and cannot be merged.');}}}}}
function mergeColumnAbove(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);var colIdx=td.cellIndex?td.cellIndex:0;var rowIndex=vp.r;if(colIdx>=0&&colIdx<row.cells.length&&rowIndex>0&&rowIndex<rowParent.rows.length){var canMerge=true;var cvi=vp.c;if(cvi>=0){if(cvi>0&&map[rowIndex-1][cvi-1]==map[rowIndex-1][cvi]){canMerge=false;}
else if(!isSameColSpan(map[rowIndex-1][cvi],map[rowIndex][cvi])){canMerge=false;}
if(canMerge){var td0=map[rowIndex-1][cvi];if(td0.colSpan==td.colSpan){setRowSpan(td0,td0.rowSpan+td.rowSpan);td0.innerHTML=td0.innerHTML+td.innerHTML;row.deleteCell(colIdx);tableSpanCleanup(rowParent);docClick({target:td0});}}}}}}}
function mergeColumnBelow(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);if(!vp){alert('Error locating virtual cell');return;}
var colIdx=td.cellIndex?td.cellIndex:0;var rowIndex=vp.r;if(colIdx>=0&&colIdx<row.cells.length&&rowIndex>=0&&rowIndex<rowParent.rows.length-1){var cvi=vp.c;rowIndex=vp.r+(td.rowSpan?td.rowSpan:1)-1;if(rowIndex<map.length-1){var n=1;var td1=map[rowIndex+1][cvi];var vp1=getVirtualCellPosition(map,td1);if(!vp1){alert('Error locating the second virtual cell');return;}
if(vp1.c!=vp.c){alert('Cannot merge the cells because their vertical positions are not the same.');return;}
if(!isSameColSpan(td1,td)){alert('Cannot merge the cells because they span differently.');return;}
var sp=td1.rowSpan?td1.rowSpan:1;var hl=td1.innerHTML;var c1=td1.cellIndex?td1.cellIndex:0;td1.parentNode.deleteCell(c1);setRowSpan(td,(td.rowSpan?td.rowSpan:1)+sp);td.innerHTML=td.innerHTML+hl;tableSpanCleanup(rowParent);docClick({target:td});}}}}}
function splitColumnH(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var tdn;var row=td.parentNode;var colIdx=td.cellIndex?td.cellIndex:0;if(colIdx>=0&&colIdx<row.cells.length){if(td.colSpan>1){tdn=row.insertCell(colIdx+1);tdn.innerHTML="cell";setColSpan(td,td.colSpan-1);setRowSpan(tdn,td.rowSpan);}
else{var r;var rowParent=row.parentNode;var tbl=rowParent;while(tbl&&tbl.tagName&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
if(tbl){var rowIndex=row.rowIndex;var map=_getTableMap(tbl);var vci=getVirtualColumnIndex(td,map[rowIndex]);var lastTD=null;for(r=0;r<map.length;r++){if(map[r][vci]!=lastTD){lastTD=map[r][vci];if(lastTD==td){tdn=row.insertCell(colIdx+1);tdn.innerHTML="cell";setRowSpan(tdn,td.rowSpan);}
else{setColSpan(lastTD,(lastTD.colSpan?lastTD.colSpan:1)+1);}}}}}
showSelectionMark(_editorOptions.selectedObject);}}}}
function splitColumnV(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var td=c.owner;if(td.parentNode&&td.parentNode.parentNode){var row=td.parentNode;var colIdx=td.cellIndex?td.cellIndex:0;if(colIdx>=0&&colIdx<row.cells.length){var tdn;var rowParent=row.parentNode;var map=_getTableMap(rowParent);var vp=getVirtualCellPosition(map,td);var rowIndex=vp.r;var cIdx=vp.c;if(td.rowSpan>1){var rn=rowIndex+td.rowSpan-1;var cni=mapGridColumnIndexToTableCellIndex(cIdx,rowParent.rows[rn]);tdn=rowParent.rows[rn].insertCell(cni);tdn.innerHTML="cell";setColSpan(tdn,td.colSpan);setRowSpan(td,td.rowSpan-1);}
else{var nr=rowParent.insertRow(rowIndex+1);var lastTD=null;for(var ci=0;ci<map[rowIndex].length;ci++){if(map[rowIndex][ci]!=lastTD){lastTD=map[rowIndex][ci];if(lastTD==td){tdn=nr.insertCell(0);tdn.innerHTML="cell";setColSpan(tdn,td.colSpan);}
else{setRowSpan(lastTD,(lastTD.rowSpan?lastTD.rowSpan:1)+1);}}}}
showSelectionMark(_editorOptions.selectedObject);}}}}
function getIframFullUrl(o){if(typeof o.name!='undefined'&&o.name.length>0){var nm=o.name.trim();if(nm.length>0){var s;if(_editorOptions.forIDE)
s=o.getAttribute('srcDesign');else
s=o.src;if(typeof s!='undefined'&&s!=null&&s.length>0){var u=getWebAddress();return u+'?iframe_'+nm+'='+s;}}}}
function removeFromCSStext(css,name){if(!css)return'';if(css.length==0)return'';if(!name)return css;if(name.length==0)return css;var cssi=css.toLowerCase();var namei=name.toLowerCase();var pos=cssi.indexOf(namei);while(pos>=0){var i=pos+namei.length;if(i==cssi.length)return css;if(pos>0){if(cssi.charAt(pos-1)!=';'&&cssi.charAt(pos-1)!=' '&&cssi.charAt(pos-1)!='{'){pos=cssi.indexOf(namei,i);continue;}}
while(i<cssi.length){if(cssi.charAt(i)==':'){var j=i+1;while(j<cssi.length){if(cssi.charAt(j)==';'||cssi.charAt(j)=='}'){break;}
j++;}
var cssNew;if(pos==0){if(j>=cssi.length-1)
cssNew='';else
cssNew=css.substr(j+1);}
else{if(j>=cssi.length-1)
cssNew=css.substr(0,pos);else
cssNew=css.substr(0,pos)+css.substr(j+1);}
return cssNew;}
else if(cssi.charAt(i)!=' '){break;}
i++;}
pos=cssi.indexOf(namei,pos+namei.length);}
return css;}
function setCSStext(obj,name,val){if(!name)return;var css=obj.style.cssText;if(!css||css.length==0){if(val&&val.length>0){obj.style.cssText=name+':'+val+';';}}
else{css=removeFromCSStext(css,name);if(val&&val.length>0){obj.style.cssText=name+':'+val+';'+css;}
else{obj.style.cssText=css;}}}
function setElementInnerHTML(e){if(!_editorOptions)return;if(e){var sender=JsonDataBinding.getSender(e);if(sender){c=sender.owner;if(c){var txt;if(!_textInput){_textInput=document.createElement('div');_textInput.style.backgroundColor='#ADD8E6';_textInput.style.position='absolute';_textInput.style.border="3px double black";_textInput.style.color='black';_textInput.style.width='600px';_textInput.style.height='400px';appendToBody(_textInput);var imgOK=document.createElement('img');imgOK.src='/libjs/ok.png';imgOK.style.cursor='pointer';_textInput.appendChild(imgOK);var imgCancel=document.createElement('img');imgCancel.src='/libjs/cancel.png';imgCancel.style.cursor='pointer';_textInput.appendChild(imgCancel);var imgDel=document.createElement('img');imgDel.src='/libjs/del.png';imgDel.style.cursor='pointer';_textInput.appendChild(imgDel);var imgPaste=document.createElement('img');imgPaste.src='/libjs/paste.png';imgPaste.style.cursor='pointer';_textInput.appendChild(imgPaste);_textInput.appendChild(document.createElement('br'));txt=document.createElement('textarea');txt.style.width='600px';txt.style.height='380px';_textInput.appendChild(txt);JsonDataBinding.AttachEvent(imgCancel,'onclick',function(){_textInput.style.display='none';_textInput.valueTarget=null;});JsonDataBinding.AttachEvent(imgOK,'onclick',function(){if(_textInput.valueTarget){if(_editorOptions){_editorOptions.deleted={parent:_textInput.valueTarget,originalHTML:_textInput.valueTarget.innerHTML};_imgUndel.src='/libjs/undel.png';}
_textInput.valueTarget.innerHTML=txt.value;}
_textInput.style.display='none';_textInput.valueTarget=null;});JsonDataBinding.AttachEvent(imgDel,'onclick',function(){var p=txt.selectionStart;if(typeof(p)!='undefined'&&p>=0){var end=txt.selectionEnd;if(end>p){txt.value=txt.value.substr(0,p)+txt.value.substr(end);txt.selectionStart=p;txt.selectionEnd=p;}}});hookTooltips(imgDel,'Delete selected text');JsonDataBinding.AttachEvent(imgPaste,'onclick',function(){var data=window.clipboardData.getData('text');if(typeof data!='undefined'&&data!=null&&data.length>0){var p=txt.selectionStart;var end;if(typeof(p)=='undefined'||p<0){p=txt.value.length-1;end=p+1;}
else{end=txt.selectionEnd;if(typeof(end)=='undefined'||end<0){end=p+1;}
else if(end<p){end=p;}}
if(end>=p){txt.value=txt.value.substr(0,p)+data+txt.value.substr(end);txt.selectionStart=p;txt.selectionEnd=p+data.length;}}});hookTooltips(imgPaste,'paste text from clipboard to replace selected text');}
txt=_textInput.getElementsByTagName('textarea')[0];_textInput.valueTarget=c;txt.value=c.innerHTML;JsonDataBinding.windowTools.updateDimensions();var ap=JsonDataBinding.ElementPosition.getElementPosition(sender);_textInput.style.left=(ap.x+3)+'px';_textInput.style.top=(ap.y+3)+'px';var zi=JsonDataBinding.getPageZIndex(_textInput)+1;_textInput.style.zIndex=zi;_textInput.style.display='block';if(e.stopPropagation)e.stopPropagation();if(e.cancelBubble!=null)e.cancelBubble=true;return true;}}}}
function addAttr(e){if(e){var c=JsonDataBinding.getSender(e);if(c){c=c.owner;if(c){var attrName=prompt('Enter new attribute name','');if(attrName){attrName=attrName.toLowerCase().trim();if(attrName.length>0){if(attrName!='id'&&attrName!='class'&&attrName!='style'){if(attrName.indexOf(' ')>=0){alert('An attribute name cannot include spaces');}
else{c.setAttribute(attrName,'new attribute');_redisplayProperties();}}
else{alert('Cannot use the new attribute name. It is a reserved word.');}}}}}}}
function _getElementProperties(tagname,c,attrs,typename){if(_elementEditorList){var props1;var internalAttributes;var i;var nounformat=false;var nodelete;var noMoveOut=false;var noCommonProps=false;var noCust=false;var isHidden=false;var noSetInnerHtml=false;var objSetter;var isSvgShape;var notclose;var issvgtext;var onpropscreated;var isCss=isCssNode(c);for(i=0;i<_elementEditorList.length;i++){if(_elementEditorList[i].tagname==typename){isSvgShape=_elementEditorList[i].isSvgShape;notclose=_elementEditorList[i].notclose;issvgtext=_elementEditorList[i].issvgtext;onpropscreated=_elementEditorList[i].onpropscreated;objSetter=_elementEditorList[i].objSetter;if(_elementEditorList[i].type){if(c.type){if(_elementEditorList[i].type==c.type){isHidden=(c.type=='hidden'&&typename=='input');props1=_elementEditorList[i].properties;if(_elementEditorList[i].nounformat){nounformat=_elementEditorList[i].nounformat;}
if(typeof _elementEditorList[i].nodelete!='undefined'){nodelete=_elementEditorList[i].nodelete;}
if(_elementEditorList[i].nomoveout){noMoveOut=_elementEditorList[i].nomoveout;}
if(_elementEditorList[i].nocommonprops){noCommonProps=_elementEditorList[i].nocommonprops;}
if(_elementEditorList[i].noCust){noCust=_elementEditorList[i].noCust;}
if(_elementEditorList[i].desc){attrs.desc=_elementEditorList[i].desc;}
else{attrs.desc='';}
if(_elementEditorList[i].noSetInnerHtml){noSetInnerHtml=_elementEditorList[i].noSetInnerHtml;}
break;}}}
else{props1=_elementEditorList[i].properties;internalAttributes=_elementEditorList[i].internalAttributes;if(isCss){var props=new Array();for(var k=0;k<props1.length;k++){if(props1[k].name=='href'){props1[k].desc='the css file to be used.';props1[k].title='Select CSS File';props1[k].filetypes='css';props.push(props1[k]);break;}}
props.push({name:'delete',desc:'delete the stylesheet file',editor:EDIT_DEL})
props.push({name:'title',desc:'title of the stylesheet',editor:EDIT_TEXT})
props.push({name:'rules',editor:EDIT_PROPS,desc:'CSS Rules',getter:function(o){var fpath=o?o.href:'';if(!fpath||fpath.length==0)return;fpath=JsonDataBinding.urlToFilename(fpath.toLowerCase());function ruleProp(ss,index,ctext,sel,filepath){var _filepath=filepath;var _styleSheet=ss;var _idx=index;var _text=ctext;var _name=sel;return{name:_name,desc:'delete the styles',editor:EDIT_NONE,getter:function(){return _text;},IMG:'/libjs/cancel.png',action:function(e){if(confirm('Do you want to remove styles '+_name+'?')){var c=JsonDataBinding.getSender(e);if(c){c.style.display='none';}
if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;JsonDataBinding.accessServer('WebPageSaveStyle.php','removeStyleRule',_filepath,{selector:_name},function(){var rdel=false;if(_styleSheet.removeRule){_styleSheet.removeRule(_idx);rdel=true;}
else if(_styleSheet.deleteRule){_styleSheet.deleteRule(_idx);rdel=true;}
else alert('Your browser does not support this feature.');if(rdel){_redisplayProperties();}});}},notModify:true}}
function getCssRuleText(st,rule){var ctxt;if(JsonDataBinding.IsIE()){if(rule.cssText){ctxt=rule.cssText;}
else if(st.cssText){var pos=st.cssText.indexOf(rule.selectorText);if(pos>=0){ctxt=st.cssText.substr(pos);pos=ctxt.indexOf('}');if(pos>0){ctxt=ctxt.substr(0,pos+1);}}
else{ctxt=st.cssText;}}}
else{ctxt=rule.cssText;}
return ctxt;}
var styleTitle='dyStyle8831932';var props=new Array();var st;var stDy;var stls=_editorOptions.elementEditedDoc.styleSheets;if(stls){for(i=0;i<stls.length;i++){if(JsonDataBinding.urlToFilename(stls[i].href?stls[i].href.toLowerCase():'')==fpath){st=stls[i];if(stDy){break;}}
else if(stls[i].title==styleTitle){stDy=stls[i];if(st){break;}}}}
if(st){var rs;if(st.cssRules){rs=st.cssRules;}
else if(st.rules){rs=st.rules;}
if(rs){for(var r=0;r<rs.length;r++){var ctxt=getCssRuleText(st,rs[r]);props.push(ruleProp(st,r,ctxt,rs[r].selectorText,fpath));}}}
if(stDy){var rs;if(stDy.cssRules){rs=stDy.cssRules;}
else if(stDy.rules){rs=stDy.rules;}
if(rs){for(var r=0;r<rs.length;r++){var isNew=true;for(var k=0;k<props.length;k++){if(props[k].name==rs[r].selectorText){isNew=false;break;}}
if(isNew){var ctxt=getCssRuleText(stDy,rs[r]);props.push(ruleProp(stDy,r,ctxt,rs[r].selectorText,fpath));}}}}
return{props:props,objSetter:objSetter};}})
return{props:props,objSetter:objSetter};}
else{if(_elementEditorList[i].nounformat){nounformat=_elementEditorList[i].nounformat;}
if(typeof _elementEditorList[i].nodelete!='undefined'){nodelete=_elementEditorList[i].nodelete;}
if(_elementEditorList[i].nomoveout){noMoveOut=_elementEditorList[i].nomoveout;}
if(_elementEditorList[i].nocommonprops){noCommonProps=_elementEditorList[i].nocommonprops;}
if(_elementEditorList[i].noCust){noCust=_elementEditorList[i].noCust;}
if(_elementEditorList[i].desc){attrs.desc=_elementEditorList[i].desc;}
else{attrs.desc='';}
if(_elementEditorList[i].noSetInnerHtml){noSetInnerHtml=_elementEditorList[i].noSetInnerHtml;}}
break;}}}
if(props1){if(isHidden||tagname=='body'||tagname=='head'||tagname=='meta'||tagname=='script'||tagname=='link'){return{props:props1,objSetter:objSetter};}
var props;if(tagname=='html'){props=new Array();for(i=0;i<props1.length;i++){if(_editorOptions.forIDE){if(props1[i].name=='title'||props1[i].name=='description'||props1[i].name=='keywords'){continue;}}
props.push(props1[i]);}
if(_editorOptions.htmlFileOption!=0){for(i=0;i<htmlsaveToProps.length;i++){props.push(htmlsaveToProps[i]);}}
if(_editorOptions.isEditingBody&&!_editorOptions.forIDE){props.push({name:'PageComment',editor:EDIT_ENUM,values:['','disabled','readOnly'],desc:'disabled:page comments are not allowed; readOnly:existing page comments are visible, cannot add new comments',getter:function(o){return _editorOptions.elementEditedDoc.body.getAttribute('commentOption');},setter:function(o,v){_editorOptions.elementEditedDoc.body.setAttribute('commentOption',v);}});}
return{props:props,objSetter:objSetter};}
else{props=new Array();if(nounformat){if(typeof nodelete!='undefined'&&!nodelete){props.push({name:'delete',desc:'delete the element',editor:EDIT_DEL});}}
else{props.push({name:'delete',desc:'delete the element',editor:EDIT_DEL});props.push({name:'commands',editor:EDIT_CMD2,toolTips:'remove formatting by removing current tag and keeping the inner contents'});}
if(!noMoveOut){props.push({name:'commands',editor:EDIT_CMD3,toolTips:'move the element out of its current parent'});}
if(!noSetInnerHtml)
props.push(innerHtmlProp);if(!noCust){if(c.subEditor){noCust=true;}}
if(!noCust){props.push({name:'newAttr',editor:EDIT_CMD,IMG:'/libjs/newAttr.png',toolTips:'add a new attribute',action:addAttr});var attrs=c.attributes;if(attrs){var k,isCust,attName,m;for(var j=0;j<attrs.length;j++){isCust=false;if(attrs[j].nodeName){attName=attrs[j].nodeName.toLowerCase();if(attName!='id'&&attName!='class'&&attName!='style'&&attName!='typename'&&attName!='scriptData'&&attName!='hidechild'&&attName!='youtube'&&attName!='youtubeID'&&attName!='onclick'&&attName!='src'&&attName!='limnorid'&&attName!='srcdesign'){isCust=true;for(k=0;k<props1.length;k++){if(props1[k].editorList&&typeof(props1[k].editorList)!='function'){var ps=props1[k].editorList;if(ps){for(m=0;m<ps.length;m++){if(attName==ps[m].name.toLowerCase()){isCust=false;break;}}}
if(!isCust)
break;}
else if(attName==props1[k].name.toLowerCase()){isCust=false;break;}}
if(isCust){for(k=0;k<commonProperties.length;k++){if(attName==commonProperties[k].name.toLowerCase()){isCust=false;break;}}}
if(isCust){if(attrs[j].nodeValue&&attrs[j].nodeValue.length>0){}
else{isCust=false;c.removeAttribute(attName);}}}}
if(isCust){if(internalAttributes){for(k=0;k<internalAttributes.length;k++){if(internalAttributes[k]==attName){isCust=false;break;}}}}
if(isCust){if(typeof(c.getAttribute(attName))=='function')
continue;props1.splice(0,0,{name:attName,editor:EDIT_CUST,desc:'attribute '+attName,getter:function(an){return function(xx){try{return xx.getAttribute(an);}
catch(exp){if(exp){if(exp.message){return exp.message;}
else
return'Exception occurred';}
else
return'Exception occurred';}};}(attName),setter:function(an){return function(xx,val){try{xx.setAttribute(an,val);}
catch(exp){if(exp){if(exp.message){alert(exp.message);}
else
alert('Exception occurred');}
else
alert('Exception occurred');}};}(attName)});}}}}
var generalAdded=false;for(i=0;i<props1.length;i++){if(!noCommonProps&&!generalAdded&&props1[i].cat){props.push({name:'general',editor:EDIT_PROPS,editorList:commonProperties,cat:PROP_GENERAL,desc:'Common properties for most elements'});generalAdded=true;}
props.push(props1[i]);}
if(!noCommonProps&&!generalAdded){props.push({name:'general',editor:EDIT_PROPS,editorList:commonProperties,cat:PROP_GENERAL,desc:'Common properties for most elements'});}
if(isSvgShape){for(i=0;i<svgshapeProps.length;i++){if(notclose&&svgshapeProps[i].closedonly)
continue;if(issvgtext&&svgshapeProps[i].nottext)
continue;props.push(svgshapeProps[i]);}}
if(onpropscreated){onpropscreated(c,props);}
return{props:props,objSetter:objSetter};}}}}
function getPropertyCell(propName){for(var i=0;i<_propsBody.rows.length;i++){if(_propsBody.rows[i].cells[0].colSpan!=2){if(_propsBody.rows[i].cells[1].propertyDescriptor.name==propName){return _propsBody.rows[i].cells[1];}}}}
function applyValue(td,val){td.value=val;var v=val;if(td.propertyDescriptor.isPixel||td.propertyDescriptor.canbepixel){if(typeof v!='undefined'&&v!=null&&JsonDataBinding.isNumber(v)){v=v+'px';}}
else if(td.propertyDescriptor.isUrl){v=JsonDataBinding.getUrlFromPath(v,_webPath);}
else if(td.propertyDescriptor.isFilePath){if(typeof v!='undefined'&&v!=null&&v.length>0&&v.indexOf('.')<0){if(!JsonDataBinding.startsWithI(v,'javascript:')){v=v+'.html';}}}
if(td.curObj.jsData&&td.curObj.jsData.designer&&td.curObj.jsData.designer.hasSetter&&td.curObj.jsData.designer.hasSetter(td.propertyDescriptor.name)){td.curObj.jsData.designer.setter(td.propertyDescriptor.name,v);}
else{var obj=td.curObj.subEditor?td.curObj.obj:td.curObj;if(td.propertyDescriptor.setter){td.propertyDescriptor.setter(obj,v);}
else{if(td.propertyDescriptor.forStyle){var c=HtmlEditor.getCssNameFromPropertyName(td.propertyDescriptor.name);_setElementStyleValue(td.curObj,td.propertyDescriptor.name,c,v);if(_editorOptions.forIDE){if(td.curObj.tagName.toLowerCase()=='body'){try{if(typeof(limnorStudio)!='undefined')limnorStudio.onBodyStyleChanged(td.propertyDescriptor.name,v);else window.external.OnBodyStyleChanged(td.propertyDescriptor.name,v);}
catch(err){alert('body:'+err.message);}}}}
else if(td.propertyDescriptor.byAttribute){if(_isIE&&td.propertyDescriptor.name=='name'){obj.setAttribute('Name',v);obj.name=v;}
else{if(typeof v=='undefined'||v==null||v.length==0){obj.setAttribute(td.propertyDescriptor.name,'');obj.getAttribute(td.propertyDescriptor.name);obj.removeAttribute(td.propertyDescriptor.name);}
else{obj.setAttribute(td.propertyDescriptor.name,v);}}}
else{if(td.propertyDescriptor.cssName){setCSStext(obj,td.propertyDescriptor.cssName,v);}
else{try{if(td.propertyDescriptor.editor==EDIT_NUM&&isNaN(v)){v='';if(td.propertyDescriptor.name=='size'){obj.size=1;}
else{obj.setAttribute(td.propertyDescriptor.name,'');obj.removeAttribute(td.propertyDescriptor.name);}}
else{if(_isIE&&td.propertyDescriptor.name=='name'){obj.setAttribute('Name',v);obj.name=v;}
else{obj[td.propertyDescriptor.name]=v;}}}
catch(exp2){alert('Error setting property '+td.propertyDescriptor.name+' to '+v+'. It might be a browser compatibility issue. Error message: '+exp2.message+'. '+HtmlEditor.GetIECompatableWarning());}}}}
if(td.propertyDescriptor.editor==EDIT_CUST){if(!v||v.length==0){obj.removeAttribute(td.propertyDescriptor.name);}}
else if(td.propertyDescriptor.forStyle&&(td.propertyDescriptor.name=='width'||td.propertyDescriptor.name=='height')){if(obj.tagName&&obj.tagName.toLowerCase()=='iframe'){obj.removeAttribute(td.propertyDescriptor.name);}}}
if(td.propertyDescriptor.onsetprop){td.propertyDescriptor.onsetprop(td.curObj,v);}
_editorOptions.pageChanged=true;}
function onTextColorChanged(c){var td=this.propCell;applyValue(td,c);}
function ontxtChange(e){var c=JsonDataBinding.getSender(e);if(c&&typeof c.value!='undefined'){var td=c.propCell;if(td&&td.propertyDescriptor){var val=c.value;if(td.propertyDescriptor.IsDocType){_editorOptions.docType=val;td.value=val;}
else{if(td.propertyDescriptor.editor==EDIT_NUM){try{val=parseInt(c.value);}
catch(er){return;}}
applyValue(td,val);}}}}
function onseltextChange(e){var c=JsonDataBinding.getSender(e);if(c){var td=c.getParentNode?c.getParentNode():c.parentNode;if(td&&td.propertyDescriptor){if(td.propertyDescriptor.ontextchange){var sels=td.getElementsByTagName('select');if(sels&&sels.length>0&&c!=sels[0]){var val=td.propertyDescriptor.ontextchange((c.getValue?c.getValue():c.value),sels[0]);if(val){applyValue(td,val);}}
else{applyValue(td,(c.getValue?c.getValue():c.value));}}
else{applyValue(td,(c.getValue?c.getValue():c.value));}}}}
function onselChildChange(e){var c=JsonDataBinding.getSender(e);if(c&&c.selectedIndex>=0){var val=c.options[c.selectedIndex].objvalue;if(val){selectEditElement(val);}}}
function onselChange_menum(e){var c=JsonDataBinding.getSender(e);if(c&&c.selectedIndex>=0){var td=c.parentNode;if(td&&td.propertyDescriptor){var val=c.options[c.selectedIndex].value;if(td.propertyDescriptor.editor==EDIT_NUM){try{val=parseInt(val);}
catch(er){return;}}
else if(td.propertyDescriptor.editor==EDIT_MENUM){if(td.propertyDescriptor.onselectindexchanged){if(td.propertyDescriptor.onselectindexchanged(c)){if(c.texteditor){c.texteditor.style.display='none';}}
else{if(c.texteditor){c.texteditor.value=c.texteditor.value+','+val;c.texteditor.style.display='inline';}
return;}}}
applyValue(td,val);}}}
function onselChange(e){var c=JsonDataBinding.getSender(e);if(c&&c.selectedIndex>=0){var td=c.parentNode;if(td&&td.propertyDescriptor){var val=c.options[c.selectedIndex].value;if(td.propertyDescriptor.editor==EDIT_NUM){try{val=parseInt(val);}
catch(er){return;}}
else if(td.propertyDescriptor.editor==EDIT_ENUM){if(td.propertyDescriptor.onselectindexchanged){if(td.propertyDescriptor.onselectindexchanged(c)){if(c.texteditor){c.texteditor.style.display='none';}}
else{if(c.texteditor){c.texteditor.style.display='inline';}
return;}}}
applyValue(td,val);}}}
function ontxtClick(e){if(_editorOptions.isEditingBody){_editorOptions.elementEditedDoc.body.contentEditable=false;}}
function ontxtBlur(e){if(_editorOptions.isEditingBody){_editorOptions.elementEditedDoc.body.contentEditable=true;}}
function onSwitchToObject(event){var c=JsonDataBinding.getSender(event);if(c&&c.owner){var obj=c.owner;var td=c.parentNode;if(td){if(td.propertyDescriptor&&td.propertyDescriptor.getter){docClick({target:td.propertyDescriptor.getter(obj)});}
else{docClick({target:obj.parentNode});}}}}
function colorToHex(c){var m=/rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/.exec(c);return m?'#'+(1<<24|m[1]<<16|m[2]<<8|m[3]).toString(16).substr(1):c;}
function nodeToString(o,a){if(a&&typeof a=='function'){return a(o);}
var s=(o.id?o.id:'')+(o.name?' - '+o.name:'')+(o.getAttribute(a)?' '+o.getAttribute(a)+':':'');var d=(o.nodeType==3)?o.data:o.outerHTML;if(d){d=d.replace('<','');d=d.replace('>','');s+=d.substr(0,30);}
return s;}
function verifyEditorOwner(){var i;if(_topElements){for(i=0;i<_topElements.length;i++){if(_topElements[i].parentNode!=document.body){document.body.appendChild(_topElements[i]);}}}
if(_propertyTopElements){for(i=0;i<_propertyTopElements.length;i++){if(_propertyTopElements[i].parentNode!=document.body){document.body.appendChild(_propertyTopElements[i]);}}}}
function isCssNode(c){if(c&&c.tagName&&c.tagName.toLowerCase()=='link'){var rel=c.getAttribute('rel');if(rel&&rel=='stylesheet'){return true;}}
return false;}
function clearPropertiesDisplay(){if(_propertyTopElements){for(var k=0;k<_propertyTopElements.length;k++){if(_propertyTopElements[k]){var pn=_propertyTopElements[k].parentNode;if(pn){pn.removeChild(_propertyTopElements[k]);}}}}
_parentList.options.length=0;_tdTitle.innerHTML=HtmlEditor.version;while(_propsBody.rows.length>0){_propsBody.deleteRow(_propsBody.rows.length-1);}
if(_elementLocator){_elementLocator.style.display='none';}
if(_textInput){_textInput.style.display='none';}
_tdObj.innerHTML='';_trObjDelim.style.display='none';showFontCommands();}
function tooltipsOwnerMouseMove(e){var obj=JsonDataBinding.getSender(e);if(obj&&(obj.toolTips||obj.tooltipsSpan)){if(!obj.moves){if(_divToolTips){_divToolTips.style.display='none';}
obj.moves=1;}
else{obj.moves++;if(obj.moves==15){showToolTips(obj,obj.tooltipsSpan?obj.innerHTML:obj.toolTips);}}}}
function tooltipsOwnerMouseOut(e){var obj=JsonDataBinding.getSender(e);if(obj&&(obj.toolTips||obj.tooltipsSpan)){obj.moves=0;if(_divToolTips){_divToolTips.style.display='none';}}}
function selectWebFile(e){if(_editorOptions&&_editorOptions.isEditingBody){var img=JsonDataBinding.getSender(e);if(img&&img.ownertextbox){var msize=1024;if(typeof img.propertyDesc.maxSize!='undefined'){msize=img.propertyDesc.maxSize;}
if(_editorOptions.forIDE){var file;if(typeof(limnorStudio)!='undefined')file=limnorStudio.onSelectFile(img.propertyDesc.title,img.propertyDesc.filetypes,img.subFolder,img.subName,msize,img.propertyDesc.disableUpload);else file=window.external.OnSelectFile(img.propertyDesc.title,img.propertyDesc.filetypes,img.subFolder,img.subName,msize,img.propertyDesc.disableUpload);if(file){img.ownertextbox.value=file;_editorOptions.pageChanged=true;if(img.ownertextbox.ontxtChange){img.ownertextbox.ontxtChange({target:img.ownertextbox});}}}
else{HtmlEditor.PickWebFile(img.propertyDesc.title,img.propertyDesc.filetypes,function(file){if(typeof file!='undefined'&&file!=null&&file.length>0){if(_editorOptions&&typeof _editorOptions.serverFolder!='undefined'&&_editorOptions.serverFolder!=null&&_editorOptions.serverFolder.length>0){if(JsonDataBinding.startsWithI(file,_editorOptions.serverFolder)){file=file.substr(_editorOptions.serverFolder.length);if(file.length>0){if(file.charAt(0)=='/'||file.charAt(0)=='\\'){file=file.substr(1);}}}}}
img.ownertextbox.value=file;_editorOptions.pageChanged=true;if(img.ownertextbox.ontxtChange){img.ownertextbox.ontxtChange({target:img.ownertextbox});}},img.propertyDesc.disableUpload,img.subFolder,img.subName,msize);}}}}
function onremovecolor(e){var img=JsonDataBinding.getSender(e);if(img&&img.owner&&img.owner.curObj&&img.owner.propertyDescriptor&&img.txtbox){if(img.owner.curObj.jsData&&img.owner.curObj.jsData.designer&&img.owner.curObj.jsData.designer.removeColor){img.owner.curObj.jsData.designer.removeColor.apply(_editorOptions.elementEditedWindow,[img.owner.curObj,img.owner.propertyDescriptor.name,HtmlEditor.getCssNameFromPropertyName(img.owner.propertyDescriptor.name)]);}
else{if(img.owner.propertyDescriptor.name=='linearGradientStartColor'){_editorOptions.client.setLinearGradientStartColor.apply(_editorOptions.editorWindow,[img.owner.curObj,null]);}
else if(img.owner.propertyDescriptor.name=='linearGradientEndColor'){_editorOptions.client.setLinearGradientEndColor.apply(_editorOptions.editorWindow,[img.owner.curObj,null]);}
else{_setElementStyleValue(img.owner.curObj,img.owner.propertyDescriptor.name,HtmlEditor.getCssNameFromPropertyName(img.owner.propertyDescriptor.name),'');}}
img.txtbox.value="";img.txtbox.style.backgroundColor='transparent';}}
function refreshPropertyDisplay(name,v){for(var i=0;i<_propsBody.rows.length;i++){if(_propsBody.rows[i].cells.length>1){var pd=_propsBody.rows[i].cells[0].propDesc;if(pd&&pd.name==name){_propsBody.rows[i].cells[1].innerHTML=v;break;}}}}
function execAct(act){return function(e){act(e);if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}}
function oncolorboxkeyup(e){var txt=JsonDataBinding.getSender(e);if(txt&&txt.propCell){if(txt.value){var c=txt.value;if(!JsonDataBinding.startsWith(c,'#')){c='#'+c;}
var isColor=/(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(c);if(isColor){onTextColorChanged.apply(txt,[c]);}}}}
function showProperties(c){if(!_editorOptions)return;_custMouseDown=null;_custMouseDownOwner=null;_comboInput=null;_editorOptions.propertiesOwner=c;_editorOptions.styleChanged=false;if(_editorOptions.markers){for(var k=0;k<_editorOptions.markers.length;k++){if(_editorOptions.markers[k]){var pn=_editorOptions.markers[k].parentNode;if(pn){pn.removeChild(_editorOptions.markers[k]);}}}
delete _editorOptions.markers;}
if(_propertyTopElements){for(var k=0;k<_propertyTopElements.length;k++){if(_propertyTopElements[k]){var pn=_propertyTopElements[k].parentNode;if(pn){pn.removeChild(_propertyTopElements[k]);}}}}
var iscss=isCssNode(c);var cap='Properties - '+elementToString(c)+'('+(_editorOptions.useSavedUrl?_editorOptions.elementEditedWindow.location.pathname.substr(0,_editorOptions.elementEditedWindow.location.pathname.length-14):_editorOptions.elementEditedWindow.location.pathname)+')';try{_tdTitle.innerHTML=cap;}
catch(err){}
while(_propsBody.rows.length>0){_propsBody.deleteRow(_propsBody.rows.length-1);}
if(!c.subEditor){_tdObj.innerHTML='';_trObjDelim.style.display='none';}
_combos=null;var props;var tag;var attrs={desc:''};if(c.subEditor){props=c.getProperties();}
else{tag=c.tagName.toLowerCase();var tn;if(c.typename){tn=c.typename;}
else{if(c.getAttribute){tn=c.getAttribute('typename');}}
if(!tn||tn.length==0){tn=tag;}
props=_getElementProperties(tag,c,attrs,tn);}
var properties=props?props.props:null;_setMessage(attrs.desc);if(!properties){properties=[{name:'delete',desc:'delete the element',editor:EDIT_DEL}];}
_divElementToolbar.innerHTML='';if(properties){var cmdImg;if(props&&props.objSetter){_trObjDelim.style.display='';props.objSetter(c);}
for(var i=0;i<properties.length;i++){cmdImg=null;if(!properties[i]){throw{message:'element '+(tag?tag:'')+' does not specify property '+i.toString()+'. Property array may has an extra comma.'};}
if(properties[i].editor==EDIT_CMD||properties[i].editor==EDIT_DEL||properties[i].editor==EDIT_CMD2||properties[i].editor==EDIT_CMD3||properties[i].editor==EDIT_GO){if(properties[i].showCommand){if(!properties[i].showCommand(c)){continue;}}
if(properties[i].editor==EDIT_CMD&&properties[i].isInit){if(properties[i].act){properties[i].act(c);}
continue;}
if(properties[i].editor==EDIT_CMD&&properties[i].isText){var txtsvg=document.createElement('input');txtsvg.type='text';txtsvg.owner=c;txtsvg.style.fontSize='x-small';txtsvg.style.position='relative';txtsvg.style.width='30px';txtsvg.style.height='16px';txtsvg.style.top='-5px';txtsvg.readOnly=false;txtsvg.value=properties[i].getter(c);_divElementToolbar.appendChild(txtsvg);JsonDataBinding.AttachEvent(txtsvg,"onchange",function(settxt){return function(e){settxt(c,txtsvg.value);};}(properties[i].setter));continue;}
if(properties[i].editor==EDIT_CMD2){cmdImg=document.createElement("img");if(properties[i].IMG)
cmdImg.src=properties[i].IMG;else
cmdImg.src='/libjs/removeTag.png';cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.owner=c;cmdImg.style.cursor='pointer';_divElementToolbar.appendChild(cmdImg);if(properties[i].action){if(properties[i].noModify)
JsonDataBinding.AttachEvent(cmdImg,"onclick",properties[i].action);else
JsonDataBinding.AttachEvent(cmdImg,"onclick",execAct(properties[i].action));}
else
JsonDataBinding.AttachEvent(cmdImg,"onclick",stripTag);}
else if(properties[i].editor==EDIT_CMD3){var pr=c.parentNode;if(pr&&pr!=_editorOptions.elementEditedDoc.body){cmdImg=document.createElement("img");if(properties[i].IMG)
cmdImg.src=properties[i].IMG;else
cmdImg.src='/libjs/removeOutTag.png';cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.owner=c;cmdImg.style.cursor='pointer';_divElementToolbar.appendChild(cmdImg);if(properties[i].action){if(properties[i].notModify)
JsonDataBinding.AttachEvent(cmdImg,"onclick",properties[i].action);else
JsonDataBinding.AttachEvent(cmdImg,"onclick",execAct(properties[i].action));}
else
JsonDataBinding.AttachEvent(cmdImg,"onclick",moveOutTag);}}
else if(properties[i].editor==EDIT_GO){var show=true;var bt=null;if(properties[i].showCommand){if(!properties[i].showCommand(c)){show=false;}}
if(!properties[i].IMG){bt=document.createElement('button');bt.forProperties=true;bt.contentEditable=false;bt.owner=c;bt.propDesc=properties[i];bt.isCommandBar=true;_divElementToolbar.appendChild(bt);}
if(show){if(properties[i].cmdText){cmdImg=null;var btText=document.createElement('span');btText.innerHTML=properties[i].cmdText;btText.style.color='green';btText.owner=c;btText.isCommandBar=true;btText.propDesc=properties[i];bt.appendChild(btText);}
else{cmdImg=document.createElement("img");if(properties[i].IMG){cmdImg.src=properties[i].IMG;}
else{cmdImg.src='/libjs/go.gif';}
cmdImg.style.cursor='pointer';}}
else{cmdImg=document.createElement("img");cmdImg.src='/libjs/go_inact.gif';cmdImg.style.cursor='arrow';if(bt){bt.disabled=true;}}
if(cmdImg){cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.owner=c;if(bt){bt.appendChild(cmdImg);}
else{_divElementToolbar.appendChild(cmdImg);}}
if(bt){if(properties[i].action){if(properties[i].notModify){JsonDataBinding.AttachEvent(bt,"onclick",properties[i].action);}
else{JsonDataBinding.AttachEvent(bt,"onclick",execAct(properties[i].action));}}
else{JsonDataBinding.AttachEvent(bt,"onclick",gotoChildByTag);}}
else{if(properties[i].action){if(properties[i].notModify){JsonDataBinding.AttachEvent(cmdImg,"onclick",properties[i].action);}
else{JsonDataBinding.AttachEvent(cmdImg,"onclick",execAct(properties[i].action));}}
else{JsonDataBinding.AttachEvent(cmdImg,"onclick",gotoChildByTag);}}}
else if(properties[i].createCommand){properties[i].createCommand(c,_divElementToolbar);}
else{cmdImg=document.createElement("img");if(properties[i].editor==EDIT_DEL){cmdImg.src='/libjs/delElement.png';}
else{cmdImg.src=properties[i].IMG;}
cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.owner=c;cmdImg.style.cursor='pointer';_divElementToolbar.appendChild(cmdImg);if(properties[i].editor==EDIT_DEL){if(properties[i].action){JsonDataBinding.AttachEvent(cmdImg,"onclick",function(h){return function(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){h(c.owner);if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}}}(properties[i].action));}
else{JsonDataBinding.AttachEvent(cmdImg,"onclick",deleteElement);}}
else{if(properties[i].action){JsonDataBinding.AttachEvent(cmdImg,"onclick",execAct(properties[i].action));}}}
if(cmdImg){cmdImg.propDesc=properties[i];cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.style.cursor='pointer';if(properties[i].toolTips){hookTooltips(cmdImg,properties[i].toolTips);}
else if(properties[i].desc){hookTooltips(cmdImg,properties[i].desc);}
if(cmdImg.parentNode==_divElementToolbar){var imgVsep=document.createElement('img');imgVsep.contentEditable=false;imgVsep.forProperties=true;imgVsep.border=0;imgVsep.src='/libjs/vSep.png';imgVsep.style.display='inline';_divElementToolbar.appendChild(imgVsep);}}}}
function showProperty(tbody,propertyDesc,cat,pridx){if(typeof pridx=='undefined'){pridx=-1;}
if(propertyDesc.editor==EDIT_PROPS){if(propertyDesc.getter){var props=propertyDesc.getter(c);if(props){for(var k0=0;k0<props.length;k0++){showProperty(tbody,props[k0],null);}}}
else{function loadPropInCat(pridx0){var props;if(typeof propertyDesc.editorList=='function'){props=propertyDesc.editorList(c);}
else{props=propertyDesc.editorList;}
if(props){for(var k0=0;k0<props.length;k0++){if(_editorOptions.isEditingBody){if(tag=='td'&&(props[k0].name=='width'||props[k0].name=='height')){continue;}}
else{if(props[k0].name=='class'||props[k0].name=='styleName'){continue;}}
pridx0++;showProperty(tbody,props[k0],propertyDesc.cat,pridx0);}}}
if(propertyDesc.cat){function toggleRows(e){var c=JsonDataBinding.getSender(e);if(c.row){function get_nextsibling(n){x=n.nextSibling;while(x&&x.nodeType!=1){x=x.nextSibling;}
return x;}
c.row.expanded=!c.row.expanded;if(c.row.expanded){c.src='/libjs/expand_min.png';if(!c.row.ploaded){c.row.ploaded=true;loadPropInCat(c.row.rowIndex);}}
else{c.src='/libjs/expand_res.png';}
var r=get_nextsibling(c.row);while(r){if(r.cat!=c.row.cat)break;if(c.row.expanded){r.style.display='';}
else{r.style.display='none';}
r=get_nextsibling(r);}}}
var cattr=tbody.insertRow(-1);cattr.style.backgroundColor=getPropCatColor(propertyDesc.cat);cattr.cat=propertyDesc.cat;cattr.expanded=false;cattr.ploaded=false;var cattd=document.createElement('td');cattr.appendChild(cattd);cattd.colSpan=2;cattd.propDesc=propertyDesc;var catimg=document.createElement('img');catimg.src='/libjs/expand_res.png';catimg.style.display='inline';catimg.style.cursor='pointer';cattd.appendChild(catimg);catimg.onclick=toggleRows;catimg.row=cattr;var catsp=document.createElement('span');catsp.style.display='inline';catsp.style.paddingLeft='2px';catsp.innerHTML=propertyDesc.name;catsp.propDesc=propertyDesc;cattd.appendChild(catsp);}
else{loadPropInCat(-1);}}
return;}
if(!_editorOptions.isEditingBody&&propertyDesc.bodyOnly){return;}
if(propertyDesc.needGrow){propertyDesc.grow=(function(idx0){return function(xprop,idxd){showProperty(tbody,xprop,propertyDesc.cat,idx0+idxd);};})(pridx);}
var tdName;var tr=tbody.insertRow(pridx);if(cat){tr.style.backgroundColor=getPropCatColor(cat);tr.cat=cat;}
else{tr.style.backgroundColor='white';}
tr.contentEditable=false;var td=document.createElement("td");tdName=td;td.contentEditable=false;td.propDesc=propertyDesc;td.innerHTML=propertyDesc.name;td.style.width=tbody.nameWidth+'px';tr.appendChild(td);td=document.createElement("td");td.contentEditable=false;td.style.width='100%';tr.appendChild(td);var s;if(c.jsData&&c.jsData.designer&&c.jsData.designer.hasSetter&&c.jsData.designer.hasGetter(propertyDesc.name)){s=c.jsData.designer.getter(propertyDesc.name);}
else{if(propertyDesc.getter){s=propertyDesc.getter(c);}
else{if(propertyDesc.forStyle){if(_editorOptions.isEditingBody){s=_editorOptions.client.getElementStyleSetting.apply(_editorOptions.elementEditedWindow,[c.subEditor?c.obj:c,HtmlEditor.getCssNameFromPropertyName(propertyDesc.name),propertyDesc.name]);}
else{if(c.subEditor)
s=c.obj.style[propertyDesc.name];else
s=c.style[propertyDesc.name];}}
else if(propertyDesc.byAttribute){if(c.subEditor)
s=c.obj.getAttribute(propertyDesc.name);else
s=c.getAttribute(propertyDesc.name);}
else{if(propertyDesc.IsDocType){if(_editorOptions.docType)
s=_editorOptions.docType;else{_editorOptions.docType=_editorOptions.client.getDocType.apply(_editorOptions.elementEditedWindow);if(_editorOptions.docType)
s=_editorOptions.docType;else
s='';}}
else{if(propertyDesc.name=='name'&&_isIE){if(c.subEditor)
s=c.obj.getAttribute('Name');else
s=c.getAttribute('Name');}
else{if(c.subEditor)
s=c.obj[propertyDesc.name];else
s=c[propertyDesc.name];}}}
if(propertyDesc.isUrl||propertyDesc.isFilePath){s=JsonDataBinding.getPathFromUrl(s,_webPath);}}}
if(typeof s=='undefined')s=null;var sel;var bt;var cmdImg;var elOptNew;td.value=s;td.tdName=tdName;if(propertyDesc.editor==EDIT_NONE){if(propertyDesc.action){var cmdImg=document.createElement("img");if(propertyDesc.IMG)
cmdImg.src=propertyDesc.IMG;else
cmdImg.src='/libjs/go.gif';cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.owner=c;cmdImg.style.cursor='pointer';cmdImg.propDesc=propertyDesc;cmdImg.ownerTd=td;td.appendChild(cmdImg);JsonDataBinding.AttachEvent(cmdImg,"onclick",propertyDesc.action);}
if(typeof s!='undefined'&&s!=null){td.appendChild(document.createTextNode(s));}
else{td.innerHTML="&nbsp;";}}
else if(propertyDesc.editor==EDIT_NODES){var isCss=(propertyDesc.name=='css');var isLink=(propertyDesc.name=='link');var isMeta=(propertyDesc.name=='meta');var isScript=(propertyDesc.name=='script');var cs=c.getElementsByTagName(isCss?'link':propertyDesc.name);if(cs&&cs.length>0){var first=true;var pagecss;if(isCss){pagecss=_editorOptions.client.getPageCssFilename.apply(_editorOptions.elementEditedWindow);if(pagecss)pagecss=pagecss.toLowerCase();}
for(var k=0;k<cs.length;k++){if(isCss){var rel=cs[k].getAttribute('rel');if(!rel)continue;if(rel.toLowerCase()!='stylesheet')continue;var f=cs[k].getAttribute('href');if(f){var pos=f.indexOf('?');if(pos>=0){f=f.substr(0,pos);}
f=JsonDataBinding.urlToFilename(f);if(f.toLowerCase()==pagecss){continue;}}}
else if(isLink){var rel=cs[k].getAttribute('rel');if(rel&&rel.toLowerCase()=='stylesheet')
continue;}
else if(isMeta){var nm=cs[k].getAttribute('name')||cs[k].name;if(nm){nm=nm.toLowerCase();if(nm=='generator'||nm=='keywords'||nm=='description'||nm=='author')
continue;}
nm=cs[k].getAttribute('http-equiv');if(nm){nm=nm.toLowerCase();if(nm=='content-type'||nm=='pragma'||nm=='x-ua-compatible')
continue;}
nm=cs[k].getAttribute('content');if(nm){nm=nm.toLowerCase();if(nm=='no-cache'||nm=='text/html; charset=utf-8'||nm=='ie=edge')
continue;}}
else if(isScript){var nm=cs[k].getAttribute('hidden');if(nm){continue;}
else{var f=cs[k].getAttribute('src');if(f){var pos=f.indexOf('?');if(pos>=0){f=f.substr(0,pos);}
f=JsonDataBinding.urlToFilename(f);if(f.toLowerCase()=='ip2cty.php'){continue;}}}}
if(first)
first=false;else{var br=document.createElement('br');td.appendChild(br);}
var o=cs[k];bt=document.createElement('button');bt.forProperties=true;bt.contentEditable=false;bt.owner=o;cmdImg=document.createElement("img");cmdImg.src='/libjs/go.gif';cmdImg.forProperties=true;cmdImg.contentEditable=false;bt.appendChild(cmdImg);var sp=document.createElement('span');sp.innerHTML=nodeToString(o,propertyDesc.sig);sp.owner=o;bt.appendChild(sp);td.appendChild(bt);JsonDataBinding.AttachEvent(bt,"onclick",gotoElement);}}}
else if(propertyDesc.editor==EDIT_CHILD){sel=document.createElement("select");sel.contentEditable=false;sel.forProperties=true;td.appendChild(sel);var tags=propertyDesc.childTag.split(',');for(var z=0;z<tags.length;z++){var children=c.getElementsByTagName(tags[z]);if(children){for(var j=0;j<children.length;j++){elOptNew=document.createElement('option');elOptNew.text=tags[z]+' - '+(children[j].id?children[j].id:'')+', '+(children[j].name?children[j].name:'');elOptNew.objvalue=children[j];addOptionToSelect(sel,elOptNew);}
sel.selectedIndex=-1;}}
JsonDataBinding.AttachEvent(sel,"onchange",onselChildChange);}
else if(propertyDesc.editor==EDIT_PARENT){bt=document.createElement("input");bt.type="button";bt.value=properties[i].name;bt.owner=_editorOptions.selectedObject;bt.forProperties=true;bt.contentEditable=false;td.appendChild(bt);JsonDataBinding.AttachEvent(bt,"onclick",onSwitchToObject);}
else if(propertyDesc.editor==EDIT_COLOR){var txt=document.createElement("input");txt.type="text";txt.className="color";td.contentEditable=false;txt.forProperties=true;txt.style.width='60px';txt.propCell=td;td.appendChild(txt);txt.value=colorToHex(s);var btsetdef=document.createElement("img");btsetdef.src='/libjs/cancel.png';btsetdef.style.cursor='pointer';btsetdef.owner=td;btsetdef.txtbox=txt;hookTooltips(btsetdef,'Remove color setting so that the default color or inherit color will be used');td.appendChild(btsetdef);jscolor.bind1(txt);JsonDataBinding.addTextBoxObserver(txt);txt.oncolorchanged=onTextColorChanged;txt.tdName=tdName;JsonDataBinding.AttachEvent(btsetdef,"onclick",onremovecolor);JsonDataBinding.AttachEvent(txt,"onkeyup",oncolorboxkeyup);}
else if(propertyDesc.editor==EDIT_BOOL){sel=document.createElement("select");sel.contentEditable=false;sel.forProperties=true;td.appendChild(sel);sel.tdName=tdName;JsonDataBinding.AttachEvent(sel,"onchange",onselChange);elOptNew=document.createElement('option');elOptNew.text='';elOptNew.value='';addOptionToSelect(sel,elOptNew);elOptNew=document.createElement('option');elOptNew.text='false';elOptNew.value=false;addOptionToSelect(sel,elOptNew);elOptNew=document.createElement('option');elOptNew.text='true';elOptNew.value=true;addOptionToSelect(sel,elOptNew);if(typeof s=='undefined'||s===''||s==null)
sel.selectedIndex=0;else if(JsonDataBinding.isValueTrue(s)){sel.selectedIndex=2;}
else{sel.selectedIndex=1;}}
else if(propertyDesc.editor==EDIT_ENUM||propertyDesc.editor==EDIT_MENUM){var vs;if(typeof propertyDesc.values=='function'){vs=propertyDesc.values(c.subEditor?c.obj:c);}
else{vs=new Array();for(var jk=0;jk<propertyDesc.values.length;jk++){vs.push({text:propertyDesc.values[jk],value:propertyDesc.values[jk]});}}
if(propertyDesc.allowEdit){td.combo=JsonDataBinding.CreateComboBox(td,vs,s,0,true);td.combo.select.contentEditable=false;td.combo.select.forProperties=true;td.combo.select.tdName=tdName;if(propertyDesc.combosetter){td.combo.setter=function(ownerObj,handler){return function(v,inputElement){handler(ownerObj,v,inputElement);};}(c,propertyDesc.combosetter);}
var input=td.combo.getInput();input.oncombotxtChange=td.combo.applyTextChanges;input.onhandleKeydown=function(){_comboInput=td.combo.input;};td.combo.select.multiple=(propertyDesc.editor==EDIT_MENUM);if(!_propertyTopElements)_propertyTopElements=new Array();_propertyTopElements.push(td.combo.select);JsonDataBinding.AttachEvent(td.combo,"onchange",onseltextChange);if(!_combos){_combos=new Array();}
_combos.push(td.combo);}
else{sel=document.createElement("select");sel.contentEditable=false;sel.forProperties=true;sel.tdName=tdName;sel.multiple=(propertyDesc.editor==EDIT_MENUM);td.appendChild(sel);for(var jn=0;jn<vs.length;jn++){elOptNew=document.createElement('option');elOptNew.text=vs[jn].text;elOptNew.value=vs[jn].value;addOptionToSelect(sel,elOptNew);if(JsonDataBinding.stringEQi(vs[jn].value,s)){sel.selectedIndex=jn;}}
if(propertyDesc.editor==EDIT_MENUM){JsonDataBinding.AttachEvent(sel,"onchange",onselChange_menum);}
else{JsonDataBinding.AttachEvent(sel,"onchange",onselChange);}
if(vs.length>30){sel.style.overflowY='scroll';}
else{sel.style.height='auto';sel.style.overflowY='';}}}
else{td.contentEditable=false;var txt2=document.createElement("input");txt2.type="text";if(typeof s!='undefined'&&s!=null){txt2.value=s;}
else{txt2.value='';}
txt2.tdName=tdName;txt2.style.width='100%';if(typeof(propertyDesc.allowEdit)!='undefined'&&propertyDesc.allowEdit==false){txt2.readOnly=true;}
if(propertyDesc.isUrl||propertyDesc.isFilePath||propertyDesc.propEditor){var tbln=document.createElement('table');tbln.border=0;tbln.cellPadding=0;tbln.cellSpacing=0;var tbodyn=JsonDataBinding.getTableBody(tbln);var trn=tbodyn.insertRow(-1);var tdn=document.createElement('td');var tdn2=document.createElement('td');tdn.style.width='100%';trn.appendChild(tdn);trn.appendChild(tdn2);td.appendChild(tbln);tdn.appendChild(txt2);cmdImg=document.createElement("img");if(propertyDesc.propEditor){cmdImg.src='/libjs/propedit.png';}
else{cmdImg.src='/libjs/folder.gif';}
cmdImg.forProperties=true;cmdImg.contentEditable=false;cmdImg.style.cursor='pointer';cmdImg.style.cssFloat='right';cmdImg.subFolder=_editorOptions.serverFolder;cmdImg.subName=_editorOptions.userAlias;tdn2.appendChild(cmdImg);cmdImg.ownertextbox=txt2;cmdImg.propertyDesc=propertyDesc;if(propertyDesc.propEditor){JsonDataBinding.AttachEvent(cmdImg,"onclick",propertyDesc.propEditor);hookTooltips(cmdImg,'Click this button to start setting this property by clicking on a point on the web page; click this button again to stop.');}
else{JsonDataBinding.AttachEvent(cmdImg,"onclick",selectWebFile);}}
else{td.appendChild(txt2);}
txt2.propCell=td;txt2.ontxtChange=ontxtChange;if(propertyDesc.allowSetText){propertyDesc.settext=function(val){txt2.ontxtChange=null;txt2.value=val;txt2.ontxtChange=ontxtChange;}}
JsonDataBinding.addTextBoxObserver(txt2);JsonDataBinding.AttachEvent(txt2,"onchange",ontxtChange);if(_editorOptions.forIDE){JsonDataBinding.AttachEvent(txt2,"onmousedown",function(e2){e2=e2||document.parentWindow.event;if(e2&&e2.button==JsonDataBinding.mouseButtonRight()){_editorOptions.propInput=txt2;var pos=JsonDataBinding.ElementPosition.getElementPosition(txt2);var txtVal=txt2.value;var selText=txtVal.substring(txt2.selectionStart,txt2.selectionEnd);try{if(typeof(limnorStudio)!='undefined')limnorStudio.onPropInputMouseDown(selText,txt2.selectionStart,txt2.selectionEnd,pos.x,pos.y);else window.external.OnPropInputMouseDown(selText,txt2.selectionStart,txt2.selectionEnd,pos.x,pos.y);}
catch(err){alert(err.message);}}});}}
td.curObj=c;td.propertyDescriptor=propertyDesc;}
for(i=0;i<properties.length;i++){if(properties[i].editor==EDIT_CMD||properties[i].editor==EDIT_DEL||properties[i].editor==EDIT_CMD2||properties[i].editor==EDIT_CMD3||properties[i].editor==EDIT_GO){continue;}
if(!_editorOptions.isEditingBody){if(properties[i].name=='class'||properties[i].name=='styleName'){continue;}}
showProperty(_propsBody,properties[i]);}}
else{var names=new Array();for(var n in c){try{if(n.substr(0,2)!='on'&&typeof c[n]!='function'){names.push(n);}}
catch(err){}}
var names2=names.sort();for(i=0;i<names2.length;i++){var nm=names2[i];var tr=_propsBody.insertRow(-1);var td=document.createElement("td");tr.appendChild(td);td.innerHTML=nm;td=document.createElement("td");tr.appendChild(td);try{td.innerHTML=c[nm];}
catch(err){td.innerHTML="<font color=red>"+err+"</font>";}}}
_showResizer();if(_editorOptions.forIDE){var se=_getSelectedObject(c);if(se){try{if(typeof(limnorStudio)!='undefined')limnorStudio.onElementSelected(se);else window.external.OnElementSelected(se);}
catch(err){alert('show properties:'+err.message);}}}
showEditorButtons(c);adjustSizes();}
function _setMessage(msg){if(_spMsg){if(msg)
_spMsg.innerHTML=msg;else
_spMsg.innerHTML='***';}}
function showEditorButtons(obj){var show=false;if(_editorOptions){if(_editorOptions.selectedHtml)
show=true;else if(!isNonBodyNode(obj)){show=true;}}
else{_divElementToolbar.innerHTML='';}
if(!show){_fontCommands.style.display='none';_imgLocatorElement.style.display='none'
imgNewElement.style.display='none';return false;}
else{_fontCommands.style.display='inline';_imgLocatorElement.style.display='inline';imgNewElement.style.display='inline';return true;}}
function docClick(e){if(_divError){if(_divError.clicks){_divError.clicks=_divError.clicks-1;}
else{_divError.style.display='none';}}
if(_divToolTips){if(_divToolTips.clicks){_divToolTips.clicks=_divToolTips.clicks-1;}
else{_divToolTips.style.display='none';}}
var sender=JsonDataBinding.getSender(e);if(_editorOptions&&_editorOptions.forIDE){if(jscolor&&jscolor.picker&&jscolor.picker.boxB){while(sender&&sender!=_editorOptions.elementEditedDoc.body){if(sender==jscolor.picker.boxB){return true;}
if(sender.forProperties){return true;}
sender=sender.parentNode;}
if(jscolor.picker.boxB.parentNode==_editorOptions.elementEditedDoc.body){_editorOptions.elementEditedDoc.body.removeChild(jscolor.picker.boxB);}
jscolor.picker=null;}}
if(_elementLocator){if(!isInLocator(sender)){_elementLocator.style.display='none';}}
if(_textInput){if(!isInTextInputor(sender)){_textInput.style.display='none';}}
return true;}
function isLastNode(node){var obj=node;while(obj&&obj!=_editorOptions.elementEditedDoc.body){if(typeof(obj.nextSibling)!='undefined'&&obj.nextSibling!=null&&typeof(obj.nextSibling.tagName)!='undefined'){return false;}
obj=obj.parentNode;}
return true;}
function closeColorPicker(){if(jscolor&&jscolor.picker&&jscolor.picker.boxB){if(jscolor.picker.boxB.parentNode==document.body){document.body.removeChild(jscolor.picker.boxB);}
jscolor.picker=null;}}
function getValue(o,name){var x=parseInt(o.getAttribute(name));if(isNaN(x)){x=1;}
return x;}
function createSvgCircle(o){var isEllipse=o.tagName.toLowerCase()=='ellipse';var propx,propy;function _showmarkers(){}
function _getProperties(){var props=[];propx={name:'cx',editor:EDIT_NUM,byAttribute:true,allowSetText:true};props.push(propx);propy={name:'cy',editor:EDIT_NUM,byAttribute:true,allowSetText:true};props.push(propy);if(isEllipse){props.push({name:'rx',editor:EDIT_NUM,byAttribute:true});props.push({name:'ry',editor:EDIT_NUM,byAttribute:true});}
else{props.push({name:'r',editor:EDIT_NUM,byAttribute:true});}
return props;}
function _moveleft(){var x=getValue(o,'cx')-HtmlEditor.svgshapemovegap;o.setAttribute('cx',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveright(){var x=getValue(o,'cx')+HtmlEditor.svgshapemovegap;o.setAttribute('cx',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveup(){var x=getValue(o,'cy')-HtmlEditor.svgshapemovegap;o.setAttribute('cy',x);if(propy&&propy.settext){propy.settext(x);}}
function _movedown(){var x=getValue(o,'cy')+HtmlEditor.svgshapemovegap;o.setAttribute('cy',x);if(propy&&propy.settext){propy.settext(x);}}
_getProperties();return{showmarkers:function(){_showmarkers();},getProperties:function(){return _getProperties();},moveleft:function(){_moveleft();},moveright:function(){_moveright();},moveup:function(){_moveup();},movedown:function(){_movedown();}};}
function createSvgText(o){var propx,propy;function _showmarkers(){}
function _getProperties(){var props=[];propx={name:'x',editor:EDIT_NUM,byAttribute:true,allowSetText:true};props.push(propx);propy={name:'y',editor:EDIT_NUM,byAttribute:true,allowSetText:true};props.push(propy);props.push({name:'text',editor:EDIT_TEXT,getter:function(o){return o.textContent;},setter:function(o,v){o.textContent=v;}});props.push(propFontFamily);props.push(propFontSize);props.push(propFontStyle);props.push(propFontWeight);props.push(propFontVariant);return props;}
function _moveleft(){var x=getValue(o,'x')-HtmlEditor.svgshapemovegap;o.setAttribute('x',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveright(){var x=getValue(o,'x')+HtmlEditor.svgshapemovegap;o.setAttribute('x',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveup(){var x=getValue(o,'y')-HtmlEditor.svgshapemovegap;o.setAttribute('y',x);if(propy&&propy.settext){propy.settext(x);}}
function _movedown(){var x=getValue(o,'y')+HtmlEditor.svgshapemovegap;o.setAttribute('y',x);if(propy&&propy.settext){propy.settext(x);}}
_getProperties();return{showmarkers:function(){_showmarkers();},getProperties:function(){return _getProperties();},moveleft:function(){_moveleft();},moveright:function(){_moveright();},moveup:function(){_moveup();},movedown:function(){_movedown();}};}
function createSvgRect(o){var propx,propy;function _showmarkers(){}
function _getProperties(){var props=[];propx={name:'x',editor:EDIT_NUM,byAttribute:true,allowSetText:true};propy={name:'y',editor:EDIT_NUM,byAttribute:true,allowSetText:true};props.push(propx);props.push(propy);props.push({name:'width',editor:EDIT_NUM,byAttribute:true});props.push({name:'height',editor:EDIT_NUM,byAttribute:true});return props;}
function _moveleft(){var x=getValue(o,'x')-HtmlEditor.svgshapemovegap;o.setAttribute('x',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveright(){var x=getValue(o,'x')+HtmlEditor.svgshapemovegap;o.setAttribute('x',x);if(propx&&propx.settext){propx.settext(x);}}
function _moveup(){var x=getValue(o,'y')-HtmlEditor.svgshapemovegap;o.setAttribute('y',x);if(propy&&propy.settext){propy.settext(x);}}
function _movedown(){var x=getValue(o,'y')+HtmlEditor.svgshapemovegap;o.setAttribute('y',x);if(propy&&propy.settext){propy.settext(x);}}
return{showmarkers:function(){_showmarkers();},getProperties:function(){return _getProperties();},moveleft:function(){_moveleft();},moveright:function(){_moveright();},moveup:function(){_moveup();},movedown:function(){_movedown();}};}
function createSvgLine(o){var prop1,prop2;var point1,point2;function parseCoords(){point1={};point1.x=getValue(o,'x1');point1.y=getValue(o,'y1');point2={};point2.x=getValue(o,'x2');point2.y=getValue(o,'y2');}
function _showmarkers(){}
function applypoints(){if(point1){o.setAttribute('x1',point1.x);o.setAttribute('y1',point1.y);}
if(point2){o.setAttribute('x2',point2.x);o.setAttribute('y2',point2.y);}
if(o.jsData.showmarkers)
o.jsData.showmarkers();}
function onmove(){if(point1&&prop1&&prop1.settext){prop1.settext(point1.x+','+point1.y);}
if(point2&&prop2&&prop2.settext){prop2.settext(point2.x+','+point2.y);}}
function _getProperties(){parseCoords();props=[];prop1={name:'point1',editor:EDIT_TEXT,cat:PROP_CUST1,desc:'one end point of the line',allowSetText:true,getter:function(o){return point1.x+','+point1.y;},setter:function(o,v){if(v&&v.length>0){var ss=v.split(',');if(ss.length>0){point1.x=parseInt(ss[0]);o.setAttribute('x1',point1.x);if(ss.length>1){point1.y=parseInt(ss[1]);o.setAttribute('y1',point1.y);}}}}};props.push(prop1);prop2={name:'point2',editor:EDIT_TEXT,cat:PROP_CUST1,desc:'one end point of the line',allowSetText:true,getter:function(o){return point2.x+','+point2.y;},setter:function(o,v){if(v&&v.length>0){var ss=v.split(',');if(ss.length>0){point2.x=parseInt(ss[0]);o.setAttribute('x2',point2.x);if(ss.length>1){point2.y=parseInt(ss[1]);o.setAttribute('y2',point2.y);}}}}};props.push(prop2);function onsetprop(curObj,v){_showmarkers();}
function onclientmousedown(e){if(_custMouseDownOwner&&_custMouseDownOwner.ownertextbox){e=e||window.event;if(e){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);var svg=o.parentNode.parentNode;if(svg&&svg.getBoundingClientRect){var c=svg.getBoundingClientRect();x=x-c.left;y=y-c.top;}
_custMouseDownOwner.ownertextbox.value=x+','+y;}}}
function oneditprop(e){var img=JsonDataBinding.getSender(e);if(img){if(_custMouseDown&&_custMouseDownOwner!=img){if(_custMouseDownOwner){_custMouseDownOwner.src='libjs/propedit.png';img.active=false;}}
img.active=!img.active;if(img.active){img.src='libjs/propeditAct.png';_custMouseDown=onclientmousedown;_custMouseDownOwner=img;}
else{img.src='libjs/propedit.png';_custMouseDown=null;_custMouseDownOwner=null;}}}
for(var i=0;i<props.length;i++){if(!props[i].onsetprop){props[i].onsetprop=onsetprop;}
props[i].propEditor=oneditprop;}
return props;}
function _moveleft(){parseCoords();if(point1&&point2){point1.x=point1.x-HtmlEditor.svgshapemovegap;point2.x=point2.x-HtmlEditor.svgshapemovegap;applypoints();onmove();}}
function _moveright(){parseCoords();if(point1&&point2){point1.x=point1.x+HtmlEditor.svgshapemovegap;point2.x=point2.x+HtmlEditor.svgshapemovegap;applypoints();onmove();}}
function _moveup(){parseCoords();if(point1&&point2){point1.y=point1.y-HtmlEditor.svgshapemovegap;point2.y=point2.y-HtmlEditor.svgshapemovegap;applypoints();onmove();}}
function _movedown(){parseCoords();if(point1&&point2){point1.y=point1.y+HtmlEditor.svgshapemovegap;point2.y=point2.y+HtmlEditor.svgshapemovegap;applypoints();onmove();}}
parseCoords();return{showmarkers:function(){_showmarkers();},getProperties:function(){return _getProperties();},moveleft:function(){_moveleft();},moveright:function(){_moveright();},moveup:function(){_moveup();},movedown:function(){_movedown();}};}
function createSvgPoly(o){var props=[];var points=[];var coords;function parseCoords(){coords=o.getAttribute('points');if(coords){var nums=coords.split(' ');points=[];for(var i=0;i<nums.length;i++){if(nums[i]){var s=nums[i].trim();if(s.length>0){var pstr=s.split(',');var pt={};pt.x=parseInt(pstr[0]);if(pstr.length>1){pt.y=parseInt(pstr[1]);}
else
pt.y=0;pt.idx=points.push(pt)-1;}}}}}
function _showmarkers(){}
function applypoints(){if(points){var ss='';for(var k=0;k<points.length;k++){if(!points[k].deleted){if(ss.length>0){ss+=' ';}
ss+=(points[k].x+','+points[k].y);}}
o.setAttribute('points',ss);if(o.jsData.showmarkers)
o.jsData.showmarkers();}}
function onmove(){if(points&&props){for(var k=0;k<points.length;k++){if(k<props.length){if(!props[k].deleted){if(props[k].settext){props[k].settext(points[k].x+','+points[k].y);}}}
else
break;}}}
function _getProperties(){parseCoords();props=[];function getterPoint(idx,obj){return function(owner){if(idx>=0&&idx<points.length){return points[idx].x+','+points[idx].y;}}}
function setterPoint(idx,obj){return function(owner,val){var ss;if(val&&val.length>0){ss=val.split(',');if(ss.length>0){var p;if(idx>=0&&idx<points.length){p=points[idx];}
else{p={};p.idx=idx;points.push(p);}
p.x=parseInt(ss[0]);if(ss.length>1){p.y=parseInt(ss[1]);}}}
else{if(idx>=0&&idx<points.length){p=points[idx];p.deleted=true;}}
applypoints();}}
function createPointProp(idx,needGrow){var p={name:'point '+idx,editor:EDIT_TEXT,cat:PROP_CUST1,desc:'a vertex point of a poly shape, in format of x,y',allowSetText:true,getter:getterPoint(idx,o),setter:setterPoint(idx,o)};if(needGrow){p.needGrow=true;p.onsetprop=function(o,v){_showmarkers();if(this.grow){if(v&&v.length>0){this.grow(createPointProp(idx+1,true),1);this.grow=null;}}};}
return p;}
if(points){for(var i=0;i<points.length;i++){props.push(createPointProp(i,false));}
props.push(createPointProp(points.length,true));}
else{props.push(createPointProp(0,true));}
function onsetprop(curObj,v){_showmarkers();}
function onclientmousedown(e){if(_custMouseDownOwner&&_custMouseDownOwner.ownertextbox){e=e||window.event;if(e){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);var svg=o.parentNode.parentNode;if(svg&&svg.getBoundingClientRect){var c=svg.getBoundingClientRect();x=x-c.left;y=y-c.top;}
_custMouseDownOwner.ownertextbox.value=x+','+y;}}}
function oneditprop(e){var img=JsonDataBinding.getSender(e);if(img){if(_custMouseDown&&_custMouseDownOwner!=img){if(_custMouseDownOwner){_custMouseDownOwner.src='libjs/propedit.png';img.active=false;}}
img.active=!img.active;if(img.active){img.src='libjs/propeditAct.png';_custMouseDown=onclientmousedown;_custMouseDownOwner=img;}
else{img.src='libjs/propedit.png';_custMouseDown=null;_custMouseDownOwner=null;}}}
for(var i=0;i<props.length;i++){if(!props[i].onsetprop){props[i].onsetprop=onsetprop;}
props[i].propEditor=oneditprop;}
return props;}
function _moveleft(){parseCoords();if(points){for(var i=0;i<points.length;i++){points[i].x=points[i].x-HtmlEditor.svgshapemovegap;}
applypoints();onmove();}}
function _moveright(){parseCoords();if(points){for(var i=0;i<points.length;i++){points[i].x=points[i].x+HtmlEditor.svgshapemovegap;}
applypoints();onmove();}}
function _moveup(){parseCoords();if(points){for(var i=0;i<points.length;i++){points[i].y=points[i].y-HtmlEditor.svgshapemovegap;}
applypoints();onmove();}}
function _movedown(){parseCoords();if(points){for(var i=0;i<points.length;i++){points[i].y=points[i].y+HtmlEditor.svgshapemovegap;}
applypoints();onmove();}}
parseCoords();return{showmarkers:function(){_showmarkers();},getProperties:function(){return _getProperties();},moveleft:function(){_moveleft();},moveright:function(){_moveright();},moveup:function(){_moveup();},movedown:function(){_movedown();}};}
function createSvgShape(o){if(o){var tag=o.tagName?o.tagName.toLowerCase():'';o.setAttribute('styleshare','NotShare');if(tag=='polygon'){return createSvgPoly(o);}
else if(tag=='rect'){return createSvgRect(o);}
else if(tag=='circle'){return createSvgCircle(o);}
else if(tag=='ellipse'){return createSvgCircle(o);}
else if(tag=='line'){return createSvgLine(o);}
else if(tag=='polyline'){return createSvgPoly(o);}
else if(tag=='text'){return createSvgText(o);}}}
function selectEditElement(c){if(c.limnorDialog)return;if(isMarker(c))
return;if(c&&c.tagName&&c.tagName.toLowerCase()=='html'){if(_parentList&&_parentList.options&&_parentList.options.length>0){_parentList.selectedIndex=_parentList.options.length-1;showProperties(_parentList.options[_parentList.options.length-1].objvalue);}
return c;}
if(_elementLocator){if(!isInLocator(c)){_elementLocator.style.display='none';}}
if(_textInput){if(!isInTextInputor(c)){_textInput.style.display='none';}}
hideSelector();if(_editorOptions.isEditingBody){if(!_editorOptions.elementEditedDoc.body.isContentEditable){_editorOptions.elementEditedDoc.body.contentEditable=true;}}
var obj;var curTag;if(c){if(c.tagName){obj=c;curTag=c.tagName.toLowerCase();}}
if(_editorOptions.isEditingBody&&!_editorOptions.client.getInitialized.apply(_editorOptions.editorWindow)){obj=_editorOptions.elementEditedDoc.body;}
var op=obj;while(op){if(op.jsData){if(op.jsData.createSubEditor){var obj0=op.jsData.createSubEditor(op,c);if(obj0){obj=obj0;c=obj0.obj?obj0.obj:obj0;break;}}
else if(op.jsData.designer&&op.jsData.designer.createSubEditor){var obj0=op.jsData.designer.createSubEditor(op,c);if(obj0){obj=obj0;break;}}}
else{if(op.getAttribute&&op.getAttribute('hidechild')){obj=op;c=op;break;}
else{if(op.getAttribute){var tpn=op.getAttribute('typename');if(typeof(tpn)!='undefined'&&tpn!=null&&tpn.length>0){if(_editorOptions.client.initPlugin.apply(_editorOptions.elementEditedWindow,[tpn,op])){obj=op;c=op;break;}}}}}
op=op.parentNode;}
if(obj){if(obj!=_editorOptions.selectedObject){_editorOptions.selectedObject=obj;if(obj.subEditor&&obj.obj==c){showSelectionMark(obj);}
else{showSelectionMark(c);}
showProperties(obj);}}
else{_editorOptions.selectedObject=null;_tdTitle.innerHTML='Properties - none';while(_propsBody.rows.length>0){_propsBody.deleteRow(_propsBody.rows.length-1);}}
return obj;}
function appendToBody(e){document.body.appendChild(e);e.contentEditable=false;e.forProperties=true;_topElements.push(e);}
function adjustObjectSize(h){if(_tdObj.lastobj){for(var i=0;i<_tdObj.children.length;i++){if(_tdObj.lastobj==_tdObj.children[i]){if(h>_tdObj.lastobj.offsetTop){_tdObj.lastobj.style.height=(h-_tdObj.offsetTop-_tdObj.lastobj.offsetTop)+'px';}
return;}}}}
function mouseup(e){_divProp.isMousedown=false;if(_resizer){_resizer.isMousedown=false;}
if(_nameresizer){_nameresizer.isMousedown=false;}
_showResizer();adjustSizes();return true;}
function mousemove(e){var diffx,diffy,w,p1,i;if(_divProp.isMousedown){e=e||window.event;if(e){diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_divProp.ox;diffy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-_divProp.oy;_divProp.style.left=diffx>0?diffx+'px':"0px";_divProp.style.top=diffy>0?diffy+'px':"0px";}}
else if(_resizer&&_resizer.isMousedown){e=e||window.event;if(e){diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_resizer.ox;diffy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-_resizer.oy;_resizer.style.left=diffx>0?diffx+'px':"0px";_resizer.style.top=diffy>0?diffy+'px':"0px";p1=JsonDataBinding.ElementPosition.getElementPosition(_divProp);var p2=JsonDataBinding.ElementPosition.getElementPosition(_resizer);w=p2.x-p1.x+_resizer.offsetWidth;var h=p2.y-p1.y+_resizer.offsetHeight;if(w>120){_divProp.style.width=w+'px';}
if(h>60){_divProp.style.height=h+'px';}}}
else if(_nameresizer&&_nameresizer.isMousedown){e=e||window.event;if(e){diffx=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-_nameresizer.ox;p1=JsonDataBinding.ElementPosition.getElementPosition(_divProp);if(diffx>p1.x+30&&diffx<p1.x+_divProp.offsetWidth-30){_nameresizer.style.left=diffx+'px';w=diffx-p1.x;for(i=0;i<_propsBody.rows.length;i++){if(_propsBody.rows[i].cells[0].colSpan!=2){_propsBody.rows[i].cells[0].style.width=w+'px';}}}}}
else{if(_editorOptions){if(_editorOptions.inline){if(_editorOptions.styleChanged){_editorOptions.styleChanged=false;if(_editorOptions.selectedObject){var cssTD=getPropertyCell('cssText');if(cssTD){var s=_editorOptions.selectedObject.style.cssText;if(s){var txts=cssTD.getElementsByTagName('input');if(txts&&txts.length>0){for(i=0;i<txts.length;i++){if(txts[i].type=='text'){cssTD.value=s;txts[i].value=s;break;}}}}}}}}
e=e||window.event;if(e){var obj=JsonDataBinding.getSender(e);if(obj){var td;if(obj.tdName)
td=obj.tdName;else if(obj.propDesc)
td=obj;if(td&&!td.isCommandBar){if(td.propDesc&&td.propDesc.editor==EDIT_PROPS){}
else{if(_tdSelectedProperty){var tr=_tdSelectedProperty.parentNode;if(tr&&tr.cat){_tdSelectedProperty.style.backgroundColor=getPropCatColor(tr.cat);}
else{_tdSelectedProperty.style.backgroundColor='white';}}
_tdSelectedProperty=td;_tdSelectedProperty.style.backgroundColor='yellow';}
_setMessage(td.propDesc.desc?td.propDesc.desc:(td.propDesc.toolTips?td.propDesc.toolTips:td.propDesc.name));}}}}}
return true;}
function hookTooltips(obj,tips){obj.toolTips=tips;obj.moves=0;JsonDataBinding.AttachEvent(obj,"onmousemove",tooltipsOwnerMouseMove);JsonDataBinding.AttachEvent(obj,"onmouseout",tooltipsOwnerMouseOut);}
function onPropGridScroll(){if(_combos){for(var i=0;i<_combos.length;i++){_combos[i].adjustPosition();}}}
function resetEditing(){if(_editorOptions&&_editorOptions.client&&_editorOptions.isEditingBody){if(confirm('Waning. Do you want to remove all modifications made since last publish of the web page, including previously saved modifications?')){_editorOptions.reset.apply(_editorOptions.elementEditedWindow);}}}
function toggleShowHideProps(){var tr=_editorBody.rows[0];if(tr.cells[0].style.display=='none'){tr.cells[0].style.display='';tr.cells[1].style.display='';tr.cells[2].style.width='60%';}
else{tr.cells[0].style.display='none';tr.cells[1].style.display='none';tr.cells[2].style.width='100%';}}
function handleSaveCss(){if(_editorOptions&&_editorOptions.propertiesOwner){if(!_editorOptions.propertiesOwner.style.cssText||_editorOptions.propertiesOwner.style.cssText==''){alert('The element does not have styles set');return;}
var elementTag=_editorOptions.propertiesOwner.tagName;var elementDesc=_getElementDesc(elementTag);var csstext='{'+_editorOptions.propertiesOwner.style.cssText+'}';var cssFilepath=JsonDataBinding.urlToFilename(getWebAddress());var pos=cssFilepath.lastIndexOf('.');if(pos>0){cssFilepath=cssFilepath.substr(0,pos)+'.css';}
JsonDataBinding.setupChildManager();document.childManager.onChildWindowReady=function(){JsonDataBinding.executeByPageId(2757476158,'setData',elementDesc,csstext,elementTag,cssFilepath,JsonDataBinding.Debug);}
JsonDataBinding.AbortEvent=false;var v3804300e={};v3804300e.isDialog=true;v3804300e.url='WebPageSaveStyle.html';v3804300e.center=true;v3804300e.top=0;v3804300e.left=0;v3804300e.width=680;v3804300e.height=420;v3804300e.resizable=true;v3804300e.border='2px double #008000';v3804300e.ieBorderOffset=4;v3804300e.title='Save Styles';v3804300e.hideCloseButtons=true;v3804300e.pageId=2757476158;v3804300e.editorOptions=_editorOptions;v3804300e.onDialogClose=function(){if(JsonDataBinding.isValueTrue((document.dialogResult)!=('ok'))){JsonDataBinding.AbortEvent=true;return;}
var cssFile=JsonDataBinding.getPropertyByPageId(2757476158,'propFilename');if(cssFile&&cssFile.length>0){var clname=JsonDataBinding.getPropertyByPageId(2757476158,'propClassname');var selector=elementTag+((clname&&clname.length>0)?'.'+clname:'');var csstext=JsonDataBinding.getPropertyByPageId(2757476158,'propCssText');v3804300e.editorOptions.client.addCssFile.apply(v3804300e.editorOptions.elementEditedWindow,[cssFile,selector,selector+csstext]);}}
JsonDataBinding.showChild(v3804300e);}}
function savePageCache(){handlePageCache(true);}
function handlePageCache(manualSave){if(_editorOptions&&_editorOptions.wholePage){if(!_editorOptions.forIDE){finishEdit({toCache:true,manualInvoke:manualSave});}}}
function _init(){_topElements=new Array();_addEditors(getDefaultEditors());_addAddableElementList(getDefaultAddables());_addCommandList(getDefaultCommands());_divSelectElement=document.createElement("div");appendToBody(_divSelectElement);_divSelectElement.style.display='none';_divSelectElement.jsData=createElementSelector();_divLargeEnum=document.createElement("div");appendToBody(_divSelectElement);_divLargeEnum.style.display='none';_divLargeEnum.jsData=createLargeEnum();_divProp=document.createElement("div");_divProp.style.cssText="overflow:hidden;background-color:#E0F8E6; z-index:100;border-top:2px solid blue;border-right:2px solid blue;border-bottom:2px solid blue;border-left:2px solid blue;";_divProp.scroll='no';_divProp.style.overflowX='hidden';_divProp.style.overflowY='hidden';_divProp.style.boxShadow='10px 10px 5px #848484';_divProp.style.borderRadius='10px';_divProp.expanded=true;_divProp.fullHeight=_divProp.style.height;_divProp.ox=0;_divProp.oy=0;_divProp.ondragstart=function(){return false;};if(containerDiv){_divProp.style.position='static';_divProp.style.left='0px';_divProp.style.right='0px';_divProp.style.width='100%';_divProp.style.height='100%';containerDiv.appendChild(_divProp);}
else{_divProp.style.position='absolute';_divProp.style.left='280px';_divProp.style.top='380px';_divProp.style.width='600px';_divProp.style.height='300px';appendToBody(_divProp);}
_divProp.style.display='block';_resizer=document.createElement("img");_resizer.style.position='absolute';_resizer.style.zIndex=10;_resizer.style.display='none';_resizer.style.cursor='nw-resize';_resizer.src='/libjs/resizer.gif';_resizer.ondragstart=function(){return false;};if(JsonDataBinding.IsIE()){_resizer.onresizestart=function(){return false;};_resizer.setAttribute("unselectable","on");}
appendToBody(_resizer);if(containerDiv){_resizer.style.display='none';}
_nameresizer=document.createElement("img");_nameresizer.style.position='absolute';_nameresizer.style.zIndex=10;_nameresizer.style.display='none';_nameresizer.style.cursor='e-resize';_nameresizer.style.width='3px';_nameresizer.src='/libjs/resizeV.png';_nameresizer.ondragstart=function(){return false;};if(_isIE){_nameresizer.onresizestart=function(){return false;};_nameresizer.setAttribute("unselectable","on");}
appendToBody(_nameresizer);_nameresizer.style.cursor='e-resize';_titleTable=document.createElement("table");_divProp.appendChild(_titleTable);_titleTable.contentEditable=false;_titleTable.forProperties=true;_titleTable.style.backgroundColor='white';_titleTable.border=0;if(containerDiv){_titleTable.style.display='none';}
var _titleBody=JsonDataBinding.getTableBody(_titleTable);var tr=_titleBody.insertRow(-1);tr.style.cssText="background-color:#A0CFEC;cursor='move';"
var td=document.createElement('td');td.contentEditable=false;td.forProperties=true;tr.appendChild(td);td.style.cssText="width:100%; cursor:move;";td.innerHTML=HtmlEditor.version;_tdTitle=td;td=document.createElement('td');td.contentEditable=false;td.forProperties=true;tr.appendChild(td);_imgExpand=document.createElement("img");_imgExpand.src="/libjs/expand_min.png";_imgExpand.style.cursor="pointer";_imgExpand.contentEditable=false;_imgExpand.forProperties=true;hookTooltips(_imgExpand,'Minimize or restore the editor window');td.appendChild(_imgExpand);_toolbarTable=document.createElement('table');_toolbarTable.cellPadding=0;_toolbarTable.cellSpacing=0;_toolbarTable.border=0;_divProp.appendChild(_toolbarTable);tr=_toolbarTable.insertRow(-1);_toolbar=tr;tr.forProperties=true;tr.contentEditable=false;td=document.createElement('td');td.contentEditable=false;td.forProperties=true;td.colSpan=2;tr.appendChild(td);_imgOK=document.createElement('img');_imgOK.src='/libjs/ok.png';_imgOK.border=0;_imgOK.style.cursor='pointer';td.appendChild(_imgOK);_imgOK.contentEditable=false;_imgOK.forProperties=true;JsonDataBinding.AttachEvent(_imgOK,"onclick",finishEdit);hookTooltips(_imgOK,'Publish current web page by saving the page to the web server. You will be prompted to close the page being edited');_imgCancel=document.createElement('img');_imgCancel.src='/libjs/cancel.png';_imgCancel.border=0;_imgCancel.style.cursor='pointer';td.appendChild(_imgCancel);_imgCancel.contentEditable=false;_imgCancel.forProperties=true;JsonDataBinding.AttachEvent(_imgCancel,"onclick",cancelClick);hookTooltips(_imgCancel,'Close the page being edited and discard all modifications.');_imgSave=document.createElement('img');_imgSave.src='/libjs/save.png';_imgSave.border=0;_imgSave.style.cursor='pointer';td.appendChild(_imgSave);_imgSave.contentEditable=false;_imgSave.forProperties=true;JsonDataBinding.AttachEvent(_imgSave,"onclick",savePageCache);hookTooltips(_imgSave,'Save current page editing to the web server without applying the modifications to the original web page.');_imgReset=document.createElement('img');_imgReset.src='/libjs/reset.png';_imgReset.border=0;_imgReset.style.cursor='pointer';td.appendChild(_imgReset);_imgReset.contentEditable=false;_imgReset.forProperties=true;JsonDataBinding.AttachEvent(_imgReset,"onclick",resetEditing);hookTooltips(_imgReset,'Remove all the modifications since last publishing of the web page.');var imgSpace=document.createElement('img');imgSpace.src='/libjs/space.png';imgSpace.border=0;imgSpace.style.width='6px';imgSpace.style.display='inline';td.appendChild(imgSpace);imgSpace.contentEditable=false;imgSpace.forProperties=true;_imgShowProp=document.createElement('img');_imgShowProp.src='/libjs/tglProp.png';_imgShowProp.border=0;_imgShowProp.style.cursor='pointer';_imgShowProp.style.display='none';td.appendChild(_imgShowProp);_imgShowProp.contentEditable=false;_imgShowProp.forProperties=true;JsonDataBinding.AttachEvent(_imgShowProp,"onclick",toggleShowHideProps);hookTooltips(_imgShowProp,'Hide/show element properties.');imgSpace=document.createElement('img');imgSpace.src='/libjs/space.png';imgSpace.border=0;imgSpace.style.width='6px';imgSpace.style.display='inline';td.appendChild(imgSpace);imgSpace.contentEditable=false;imgSpace.forProperties=true;imgNewElement=document.createElement('img');imgNewElement.src='/libjs/newElement.png';imgNewElement.border=0;imgNewElement.style.display='inline';imgNewElement.style.cursor='pointer';td.appendChild(imgNewElement);imgNewElement.contentEditable=false;imgNewElement.forProperties=true;JsonDataBinding.AttachEvent(imgNewElement,"onclick",addElement);hookTooltips(imgNewElement,'Insert a new HTML element into the web page');var imgHelp=document.createElement('img');imgHelp.src='/libjs/help.png';imgHelp.border=0;imgHelp.style.display='inline';imgHelp.style.cursor='pointer';td.appendChild(imgHelp);imgHelp.contentEditable=false;imgHelp.forProperties=true;JsonDataBinding.AttachEvent(imgHelp,"onclick",function(){window.open('http://www.limnor.com/home1/AA/AA/htmlEditor.html');});hookTooltips(imgHelp,'Show help');_imgLocatorElement=document.createElement('img');_imgLocatorElement.src='/libjs/elementLocator.png';_imgLocatorElement.border=0;_imgLocatorElement.style.display='inline';_imgLocatorElement.style.cursor='pointer';_imgLocatorElement.style.width='22px';_imgLocatorElement.style.height='16px';td.appendChild(_imgLocatorElement);_imgLocatorElement.contentEditable=false;_imgLocatorElement.forProperties=true;JsonDataBinding.AttachEvent(_imgLocatorElement,"onclick",locateElement);hookTooltips(_imgLocatorElement,'Search HTML element by tag, id and name');_fontCommands=document.createElement('div');_fontCommands.style.display='none';td.appendChild(_fontCommands);_fontCommands.contentEditable=false;_fontCommands.forProperties=true;var i,imgCmd;if(_commandList){var imgVsep=document.createElement('img');imgVsep.contentEditable=false;imgVsep.forProperties=true;imgVsep.border=0;imgVsep.src='/libjs/vSep.png';imgVsep.style.display='inline';_fontCommands.appendChild(imgVsep);for(i=0;i<_commandList.length;i++){imgCmd=document.createElement('img');imgCmd.contentEditable=false;imgCmd.forProperties=true;imgCmd.border=0;imgCmd.src=_commandList[i].inactImg;imgCmd.style.display='inline';imgCmd.cmd=_commandList[i].cmd;imgCmd.style.cursor='pointer';imgCmd.actSrc=_commandList[i].actImg;imgCmd.inactSrc=_commandList[i].inactImg;if(typeof _commandList[i].useUI!='undefined')imgCmd.useUI=_commandList[i].useUI;if(typeof _commandList[i].value!='undefined')imgCmd.value=_commandList[i].value;if(typeof _commandList[i].isCust!='undefined')imgCmd.isCust=_commandList[i].isCust;JsonDataBinding.AttachEvent(imgCmd,"onclick",fontCommandSelected);_fontCommands.appendChild(imgCmd);if(_commandList[i].tooltips){hookTooltips(imgCmd,_commandList[i].tooltips);}
imgVsep=document.createElement('img');imgVsep.contentEditable=false;imgVsep.forProperties=true;imgVsep.border=0;imgVsep.src='/libjs/vSep.png';imgVsep.style.display='inline';_fontCommands.appendChild(imgVsep);}}
_fontSelect=document.createElement('select');_fontSelect.contentEditable=false;_fontSelect.forProperties=true;_fontCommands.appendChild(_fontSelect);_fontSelect.size=1;JsonDataBinding.AttachEvent(_fontSelect,'onchange',onFontSelected);for(i=0;i<fontList.length;i++){var fi=document.createElement('option');fi.text=fontList[i];fi.value=fontList[i];addOptionToSelect(_fontSelect,fi);}
_headingSelect=document.createElement('select');_headingSelect.contentEditable=false;_headingSelect.forProperties=true;_fontCommands.appendChild(_headingSelect);_headingSelect.size=1;for(i=0;i<=6;i++){var fi=document.createElement('option');if(i==0){fi.text='Headings';}
else{fi.text='heading '+i;fi.value=i;}
addOptionToSelect(_headingSelect,fi);}
JsonDataBinding.AttachEvent(_headingSelect,'onchange',onHeadingSelected);_colorSelect=document.createElement('input');_colorSelect.type='text';_colorSelect.className='color';_colorSelect.contentEditable=false;_colorSelect.forProperties=true;_colorSelect.readOnly=true;_colorSelect.style.display='inline';_colorSelect.style.cursor='pointer';_colorSelect.style.width='16px';_colorSelect.style.color=_colorSelect.style.backgroundColor;hookTooltips(_colorSelect,'Set element color');jscolor.bind1(_colorSelect);_colorSelect.oncolorchanged=fontCommandColorSelected;_fontCommands.appendChild(_colorSelect);_imgInsSpace=document.createElement('img');_imgInsSpace.contentEditable=false;_imgInsSpace.forProperties=true;_imgInsSpace.src='/libjs/insSpace_inact.png';_imgInsSpace.style.cursor='pointer';_fontCommands.appendChild(_imgInsSpace);JsonDataBinding.AttachEvent(_imgInsSpace,"onclick",insSpaceClick);hookTooltips(_imgInsSpace,'Add a space outside the beginning of the current element (owner of properties) so that you may enter new contents outside of the current element.');_imgInsBr=document.createElement('img');_imgInsBr.contentEditable=false;_imgInsBr.forProperties=true;_imgInsBr.src='/libjs/insBr_inact.png';_imgInsBr.style.cursor='pointer';_fontCommands.appendChild(_imgInsBr);JsonDataBinding.AttachEvent(_imgInsBr,"onclick",insBrClick);hookTooltips(_imgInsBr,'Add a line-break outside the beginning of the current element (owner of properties) so that you may enter new contents outside of the current element.');_imgStopTag=document.createElement('img');_imgStopTag.contentEditable=false;_imgStopTag.forProperties=true;_imgStopTag.src='/libjs/stopTag_inact.png';_imgStopTag.style.cursor='pointer';_fontCommands.appendChild(_imgStopTag);JsonDataBinding.AttachEvent(_imgStopTag,"onclick",stopTagClick);hookTooltips(_imgStopTag,'Add a space outside the end of the current element (owner of properties) so that you may enter new contents outside of the current element.');_imgAppBr=document.createElement('img');_imgAppBr.contentEditable=false;_imgAppBr.forProperties=true;_imgAppBr.src='/libjs/appBr_inact.png';_imgAppBr.style.cursor='pointer';_fontCommands.appendChild(_imgAppBr);JsonDataBinding.AttachEvent(_imgAppBr,"onclick",appBrClick);hookTooltips(_imgAppBr,'Add a line-break outside the ending of the current element (owner of properties) so that you may enter new contents outside of the current element.');var _imgbr=document.createElement('img');_imgbr.contentEditable=false;_imgbr.forProperties=true;_imgbr.src='/libjs/insLF.png';_imgbr.style.cursor='pointer';_fontCommands.appendChild(_imgbr);JsonDataBinding.AttachEvent(_imgbr,"onclick",insLFClick);hookTooltips(_imgbr,'Insert a line-break at the caret position.');_imgAppTxt=document.createElement('img');_imgAppTxt.contentEditable=false;_imgAppTxt.forProperties=true;_imgAppTxt.src='/libjs/appTxt.png';_imgAppTxt.style.cursor='pointer';_fontCommands.appendChild(_imgAppTxt);JsonDataBinding.AttachEvent(_imgAppTxt,"onclick",appTxtClick);hookTooltips(_imgAppTxt,'Append a new paragraph at the end of the web page.');_imgUndel=document.createElement('img');_imgUndel.contentEditable=false;_imgUndel.forProperties=true;_imgUndel.src='/libjs/undel_inact.png';_imgUndel.style.cursor='pointer';_fontCommands.appendChild(_imgUndel);JsonDataBinding.AttachEvent(_imgUndel,"onclick",undelete);hookTooltips(_imgUndel,'Make an undo to the last element deletion');_imgUndo=document.createElement('img');_imgUndo.contentEditable=false;_imgUndo.forProperties=true;_imgUndo.src='/libjs/undo_inact.png';_imgUndo.style.cursor='pointer';_fontCommands.appendChild(_imgUndo);JsonDataBinding.AttachEvent(_imgUndo,"onclick",undo);hookTooltips(_imgUndo,'Make an undo');_imgRedo=document.createElement('img');_imgRedo.contentEditable=false;_imgRedo.forProperties=true;_imgRedo.src='/libjs/redo_inact.png';_imgRedo.style.cursor='pointer';_fontCommands.appendChild(_imgRedo);JsonDataBinding.AttachEvent(_imgRedo,"onclick",redo);hookTooltips(_imgRedo,'Make a redo');_imgUndo.style.display='none';_imgRedo.style.display='none';inputColorPicker=document.createElement('input');inputColorPicker.type='text';inputColorPicker.style.width='60px';inputColorPicker.readOnly=true;inputColorPicker.style.display='inline';inputColorPicker.style.cursor='pointer';inputColorPicker.className='color';inputColorPicker.contentEditable=false;inputColorPicker.forProperties=true;_fontCommands.appendChild(inputColorPicker);jscolor.bind1(inputColorPicker);if(containerDiv){_fontSelect.style.display='none';_headingSelect.style.display='none';inputColorPicker.style.display='none';}
_editorDiv=document.createElement('div');_divProp.appendChild(_editorDiv);_editorDiv.style.padding=0;_editorDiv.style.margin=0;_editorDiv.style.border=0;_editorDiv.style.overflowX='hidden';_editorDiv.style.overflowY='hidden';_editorDiv.contentEditable=false;_editorDiv.forProperties=true;_editorDiv.style.width='100%';_editorDiv.style.height='100%';_editorTable=document.createElement("table");_editorDiv.appendChild(_editorTable);_editorTable.contentEditable=false;_editorTable.forProperties=true;_editorTable.style.backgroundColor='white';_editorTable.border=1;_editorTable.width='100%';_editorBody=JsonDataBinding.getTableBody(_editorTable);tr=_editorBody.insertRow(-1);td=document.createElement('td');td.vAlign='top';tr.appendChild(td);_tdParentList=td;_tdParentList.style.backgroundColor='#A9F5A9';var _objTable=document.createElement('table');_objTable.border=0;_objTable.cellPadding=0;_objTable.cellSpacing=0;_objTable.style.padding='0px';_objTable.style.backgroundColor='#A9F5A9';_tdParentList.appendChild(_objTable);var objTbody=JsonDataBinding.getTableBody(_objTable);var objRow=objTbody.insertRow(-1);_parentList=document.createElement('select');_parentList.style.border='none';_parentList.style.cursor='pointer';_parentList.style.backgroundColor='#A9F5A9';td=document.createElement('td');objRow.appendChild(td);td.appendChild(_parentList);JsonDataBinding.AttachEvent(_parentList,"onchange",selectparent);_trObjDelim=objTbody.insertRow(-1);_tdObjDelim=document.createElement('td');_trObjDelim.appendChild(_tdObjDelim);_trObjDelim.style.height='2px';_trObjDelim.style.backgroundColor='black';_trObjDelim.style.display='none';objRow=objTbody.insertRow(-1);_tdObj=document.createElement('td');objRow.appendChild(_tdObj);_tdObj.style.overflowX='auto';_tdObj.style.overflowY='hidden';td=document.createElement('td');td.style.width='100%';td.vAlign='top';tr.appendChild(td);_divElementToolbar=document.createElement("div");_divElementToolbar.contentEditable=false;_divElementToolbar.forProperties=true;_divElementToolbar.style.width='100%';_divElementToolbar.style.backgroundColor='#FFFF9E';_divElementToolbar.style.textAlign='left';td.appendChild(_divElementToolbar);_divPropertyGrid=document.createElement("div");_divPropertyGrid.contentEditable=false;_divPropertyGrid.forProperties=true;_divPropertyGrid.style.width='100%';_divPropertyGrid.style.textAlign='left';_divPropertyGrid.style.overflowX='hidden';_divPropertyGrid.style.overflowY='auto';_divPropertyGrid.onscroll=onPropGridScroll;td.appendChild(_divPropertyGrid);_tableProps=document.createElement("table");_tableProps.border=1;_tableProps.width="100%";_divPropertyGrid.style.backgroundColor='#E0F8E0';_divPropertyGrid.appendChild(_tableProps);_propsBody=JsonDataBinding.getTableBody(_tableProps);_propsBody.nameWidth=60;tr=_propsBody.insertRow(-1);td=document.createElement('td');tr.appendChild(td);td.innerHTML="Name";td.style.width=_propsBody.nameWidth;td=document.createElement('td');tr.appendChild(td);td.innerHTML="Value";td.style.width='100%';_spMsg=document.createElement('span');_spMsg.style.width='100%';_spMsg.style.height='100%';_spMsg.style.overflow='visible';_spMsg.innerHTML='';_spMsg.style.fontSize='x-small';_messageBar=document.createElement('div');_divProp.appendChild(_messageBar);_messageBar.style.verticalAlign='top';_messageBar.appendChild(_spMsg);JsonDataBinding.AttachEvent(_divProp,"onscroll",editorScroll);_divSelectElement.style.display='none';_showResizer();if(_isIE){document.execCommand('RespectVisibilityInDesign',true,null);}
var stl;for(var s=0;s<document.styleSheets.length;s++){if(document.styleSheets[s].title=='htmlEditor_menuEditor'){stl=document.styleSheets[s];break;}}
if(!stl){var st=document.createElement('style');st.title='htmlEditor_menuEditor';st.setAttribute('hidden','true');document.getElementsByTagName('head')[0].appendChild(st);for(var s=0;s<document.styleSheets.length;s++){if(document.styleSheets[s].title=='htmlEditor_menuEditor'){stl=document.styleSheets[s];break;}}
var idx=0;var selector='nav.htmlEditor_menuEditor';var rule=selector+' {background:#E0FFFF;cursor:pointer;}';if(stl.insertRule){stl.insertRule(rule,idx);}
else{stl.addRule(selector,rule,idx);}
idx++;rule=selector+' ul li:hover {background:#D3D3D3;}';if(stl.insertRule){stl.insertRule(rule,idx);}
else{stl.addRule(selector+' ul li:hover',rule,idx);}
idx++;rule=selector+' ul li a:hover {background:#D3D3D3;}';if(stl.insertRule){stl.insertRule(rule,idx);}
else{stl.addRule(selector+' ul li a:hover',rule,idx);}
idx++;}
_initSelection();function editorScroll(){_showResizer();}
function mousedown(e){var selectedElement;function onselectElement(){if(_editorOptions){selectEditElement(selectedElement);if(_editorOptions.forIDE&&e.button==JsonDataBinding.mouseButtonRight()){var pos=_editorOptions.client.getElementPosition.apply(_editorOptions.editorWindow,[selectedElement]);var objStr=_getSelectedObject(selectedElement);if(typeof(limnorStudio)!='undefined')limnorStudio.onRightClickElement(objStr,pos.x,pos.y);else window.external.OnRightClickElement(objStr,pos.x,pos.y);}}}
e=e||document.parentWindow.event;var c=JsonDataBinding.getSender(e);var x,y;if(_resizer){if(c==_resizer){_resizer.isMousedown=true;_divProp.isMousedown=false;_nameresizer.isMousedown=false;x=_resizer.offsetLeft;y=_resizer.offsetTop;_resizer.ox=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-x;_resizer.oy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-y;if(_nameresizer){_nameresizer.style.display='none';}
return false;}}
if(_nameresizer){if(c==_nameresizer){_nameresizer.isMousedown=true;_resizer.isMousedown=false;_divProp.isMousedown=false;x=_nameresizer.offsetLeft;y=_nameresizer.offsetTop;_nameresizer.ox=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-x;_nameresizer.oy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-y;return false;}}
var c0=c;var isAddNew=false;while(c0){if(c0==_divSelectElement){isAddNew=true;break;}
c0=c0.parentNode;}
if(!isAddNew){hideSelector();}
if(_tdTitle==c){x=_divProp.offsetLeft;y=_divProp.offsetTop;_divProp.ox=(e.pageX?e.pageX:e.clientX?e.clientX:e.x)-x;_divProp.oy=(e.pageY?e.pageY:e.clientY?e.clientY:e.y)-y;_divProp.isMousedown=true;_nameresizer.isMousedown=false;_resizer.isMousedown=false;if(_resizer){_resizer.style.display='none';}
if(_nameresizer){_nameresizer.style.display='none';}
return false;}
else{_divProp.isMousedown=false;_resizer.isMousedown=false;_nameresizer.isMousedown=false;if(_editorOptions&&_editorOptions.inline){if(!isAddNew){if(selectedElement&&!selectedElement.forProperties){selectedElement=c;if(_isIE){if(_editorOptions.forIDE){selectEditElement(selectedElement);if(e.button==JsonDataBinding.mouseButtonRight()){var pos=_editorOptions.client.getElementPosition.apply(_editorOptions.editorWindow,[selectedElement]);var objStr=_getSelectedObject(selectedElement);if(typeof(limnorStudio)!='undefined')limnorStudio.onRightClickElement(objStr,pos.x,pos.y);else window.external.OnRightClickElement(objStr,pos.x,pos.y);}}
else{setTimeout(onselectElement,10);}}
else{selectEditElement(selectedElement);}}
else{}}}}
return true;}
function keydown(e){e=e||event;if(!e){return true;}
var code=e.keyCode||e.which||null;if(code){if(_editorOptions){if(!(code>=33&&code<=40)){_editorOptions.pageChanged=true;}}
if(code==8){if(!_handleBackspace(false)){window.event.returnValue=false;if(e.preventDefault){e.preventDefault();}
return false;}}}
return true;}
JsonDataBinding.AttachEvent(_imgExpand,"onclick",_togglePropertiesWindow);JsonDataBinding.AttachEvent(document,"onclick",docClick);JsonDataBinding.AttachEvent(document,"onmousedown",mousedown);JsonDataBinding.AttachEvent(document,"onmouseup",mouseup);JsonDataBinding.AttachEvent(document,"onmousemove",mousemove);if(document.addEventListener){document.addEventListener('keydown',keydown,true);}
else{if(document.attachEvent){document.attachEvent('onkeydown',keydown);}}
var ifrs=document.body.getElementsByTagName('iframe');if(ifrs&&ifrs.length>0){for(i=0;i<ifrs.length;i++){var fdoc=ifrs[i].contentDocument||ifrs[i].contentWindow.document;fdoc.onclick=framenotify;}}
if(containerDiv){_imgSave.style.display='none';_imgReset.style.display='none';imgNewElement.style.display='none';_imgShowProp.style.display='inline';var tr=_editorBody.rows[0];tr.cells[1].style.width='auto';var td;if(tr.cells.length<3){td=document.createElement('td');td.vAlign='top';td.style.width='100%';tr.appendChild(td);}
else{td=tr.cells[2];}
td.innerHTML='';containerDiv.tdContentContainer=td;}
else{setInterval(handlePageCache,_autoSaveMinutes*60*1000);}}
function removeEditorElements(){var i;if(_topElements){for(i=0;i<_topElements.length;i++){var p=_topElements[i].parentNode;if(p){p.removeChild(_topElements[i]);}}}
if(_propertyTopElements){for(i=0;i<_propertyTopElements.length;i++){var p=_propertyTopElements[i].parentNode;if(p){p.removeChild(_propertyTopElements[i]);}}}}
function _removeDuplicatedFile(){var head=_editorOptions.elementEditedDoc.documentElement.parentNode.getElementsByTagName('head')[0];var scriptNodes=head.getElementsByTagName('script');var linkNodes=head.getElementsByTagName('link');var i,k;var files,f,b,dups;if(scriptNodes&&scriptNodes.length>0){files=new Array();dups=new Array();for(i=0;i<scriptNodes.length;i++){f=scriptNodes[i].getAttribute('src');if(f){f=f.toLowerCase();b=false;for(k=0;k<files.length;k++){if(files[k]==f){b=true;break;}}
if(b){dups.push(scriptNodes[i]);}
else{files.push(f);}}}
for(i=0;i<dups.length;i++){head.removeChild(dups[i]);}}
if(linkNodes&&linkNodes.length>0){files=new Array();dups=new Array();for(i=0;i<linkNodes.length;i++){f=linkNodes[i].getAttribute('href');if(f){f=f.toLowerCase();b=false;for(k=0;k<files.length;k++){if(files[k]==f){b=true;break;}}
if(b){dups.push(linkNodes[i]);}
else{files.push(f);}}}
for(i=0;i<dups.length;i++){head.removeChild(dups[i]);}}}
function _getresultHTML(publishing){if(jscolor&&jscolor.picker){if(jscolor.picker.owner){delete jscolor.picker.owner;}
if(jscolor.picker.boxB){if(jscolor.picker.boxB.parentNode==document.body){document.body.removeChild(jscolor.picker.boxB);}}
jscolor.picker=null;}
var prbox;if(_editorOptions.redbox){prbox=_editorOptions.redbox.parentNode;if(prbox){_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow,[prbox,_editorOptions.redbox]);}}
var pmarkers=[];if(_editorOptions.markers){for(var k=0;k<_editorOptions.markers.length;k++){pmarkers.push(null);if(_editorOptions.markers[k]){prbox=_editorOptions.markers[k].parentNode;if(prbox){pmarkers[k]=prbox;_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow,[prbox,_editorOptions.markers[k]]);}}}}
var _resultHtml;if(_editorOptions.isEditingBody){_editorOptions.client.cleanup.apply(_editorOptions.elementEditedWindow,[_editorOptions.client,publishing]);_editorOptions.elementEdited.removeAttribute('contentEditable');if(_editorOptions.inline){removeEditorElements();}
var i;_removeDuplicatedFile();setMetaData(_editorOptions.elementEditedDoc.documentElement,'generator','Limnor Visual HTML Editor -- Online Edition (http://www.limnor.com)');setContentType(_editorOptions.elementEditedDoc.documentElement,'text/html; charset=UTF-8');setIECompatible(_editorOptions.elementEditedDoc.documentElement);setContentNoCache(_editorOptions.elementEditedDoc.documentElement);if(_editorOptions.elementEditedDoc.documentElement.outerHTML){if(_editorOptions.docType){_resultHtml=_editorOptions.docType+'\r\n'+_editorOptions.elementEditedDoc.documentElement.outerHTML;}
else{_resultHtml=_editorOptions.elementEditedDoc.documentElement.outerHTML;}}
else{var htmlLine='<html';var attrs=_editorOptions.elementEditedDoc.documentElement.attributes;if(attrs){for(i=0;i<attrs.length;i++){if(attrs[i].nodeValue&&attrs[i].nodeValue.length>0){htmlLine+=' ';htmlLine+=attrs[i].nodeName;htmlLine+='=';htmlLine+=attrs[i].nodeValue;}}}
htmlLine+='>';if(_editorOptions.docType){_resultHtml=_editorOptions.docType+'\r\n'+htmlLine+_editorOptions.elementEditedDoc.documentElement.innerHTML+'</html>';}
else{_resultHtml=htmlLine+_editorOptions.elementEditedDoc.documentElement.innerHTML+'</html>';}}
_editorOptions.client.onAfterSaving.apply(_editorOptions.elementEditedWindow);var pos0,pos1;while(true){pos0=_resultHtml.indexOf(_editorStart);if(pos0>=0){pos1=_resultHtml.indexOf(_editorEnd);if(pos1>0){_resultHtml=_resultHtml.substr(0,pos0)+_resultHtml.substr(pos1+_editorEnd.length);}
else break;}
else break;}
while(true){pos0=_resultHtml.indexOf(_editorStartJs);if(pos0>=0){pos1=_resultHtml.indexOf(_editorEndJs);if(pos1>0){_resultHtml=_resultHtml.substr(0,pos0)+_resultHtml.substr(pos1+_editorEndJs.length);}
else break;}
else break;}
if(_editorOptions.inline&&_editorOptions.isEditingBody){if(_topElements){for(i=0;i<_topElements.length;i++){document.body.appendChild(_topElements[i]);}}
if(_propertyTopElements){for(i=0;i<_propertyTopElements.length;i++){document.body.appendChild(_propertyTopElements[i]);}}}
_editorOptions.elementEdited.contentEditable=true;}
else{_editorOptions.elementEdited.contentEditable=false;if(_editorOptions.elementEdited.outerHTML){_resultHtml=_editorOptions.elementEdited.outerHTML;}
else{var p=_editorOptions.elementEdited.parentNode;var p2=_editorOptions.elementEditedDoc.createElement(p.tagName);p2.appendChild(_editorOptions.elementEdited);_resultHtml=p2.innerHTML;p.appendChild(_editorOptions.elementEdited);}
_editorOptions.elementEdited.contentEditable=true;}
if(prbox){_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[prbox,_editorOptions.redbox]);_editorOptions.redbox.contentEditable=false;}
if(_editorOptions.markers){for(var k=0;k<_editorOptions.markers.length;k++){if(_editorOptions.markers[k]){prbox=pmarkers[k];if(prbox){_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[prbox,_editorOptions.markers[k]]);_editorOptions.markers[k].contentEditable=false;}}}}
return _resultHtml;}
function _setSelectedObject(guid){if(_editorOptions){if(guid==''){selectEditElement(_editorOptions.elementEditedDoc.body);return;}
function selectElementByGuid(e,guid){if(e.childNodes&&e.childNodes.length>0){for(var i=0;i<e.childNodes.length;i++){if(e.childNodes[i].getAttribute){var g=e.childNodes[i].getAttribute('limnorId');if(g&&g==guid){if(e.childNodes[i].id&&e.childNodes[i].id.length>0){}
else{e.childNodes[i].id=_createId(e.childNodes[i]);}
selectEditElement(e.childNodes[i]);return true;}}
var ret=selectElementByGuid(e.childNodes[i],guid);if(ret){return ret;}}}}
selectElementByGuid(_editorOptions.elementEditedDoc.body,guid);}}
function _getMenubarString(obj){var ret='';function getmenubaritem(mi){if(mi&&mi.tagName&&mi.tagName.toLowerCase()=='li'){ret+='[';if(mi.id&&mi.id.length>0){}
else{mi.id=_createId(mi);_editorOptions.pageChanged=true;}
ret+=mi.id+';';var txt='';for(var k=0;k<mi.children.length;k++){if(mi.children[k].tagName&&mi.children[k].tagName.toLowerCase()=='a'){txt=mi.children[k].innerHTML;break;}}
txt=txt.replace('[','#005B#');txt=txt.replace(']','#005D#');txt=txt.replace(',','#002C#');ret+=txt;getmesubnubar(mi);ret+=']';}}
function getmesubnubar(o){if(o.children.length>0){for(var i=0;i<o.children.length;i++){if(o.children[i]&&o.children[i].tagName&&o.children[i].tagName.toLowerCase()=='ul'){for(var j=0;j<o.children[i].children.length;j++){getmenubaritem(o.children[i].children[j]);}}}}}
getmesubnubar(obj);return ret;}
function _getSelectedObject(obj){var sRet='';if(_editorOptions){if(!obj){obj=_editorOptions.ideCurrentobj||_editorOptions.selectedObject;}
if(obj){if(obj.getStringForIDE){sRet=obj.getStringForIDE();}
else if(obj.tagName){var tag=obj.tagName.toLowerCase();if(tag=='form'){if(obj.getAttribute('typename')=='fileupload'){tag='fileupload';}}
sRet=tag+',';var t1;var t0=obj.getAttribute('type');if(tag=='nav'){if(obj.className=='limnorstyles_menu'){t0='menubar';t1=_getMenubarString(obj);}}
else if(tag=='ul'){if(obj.className=='limnortv'){t0='tv';}}
if(t0)sRet+=t0;sRet+=',';if(!t1)t1=obj.getAttribute('limnortype');if(t1)sRet+=t1;sRet+=',';if(obj.id&&obj.id.length>0){if(tag=='fileupload'){sRet+=obj.id.substr(0,obj.id.length-1);}
else{sRet+=obj.id;}}
sRet+=',';var g=obj.getAttribute('limnorId');if(g){sRet+=g;}
else{if(tag=='fileupload'){for(var i=0;i<obj.children.length;i++){g=obj.children[i].getAttribute('limnorId');if(g){sRet+=g;break;}}}}
var src0;if(tag=='img'||tag=='script'){sRet+=',src,';src0=obj.src;if(src0){src0=src0.replace(_webPath,'');sRet+=src0;}}
else if(tag=='link'){sRet+=',href,';src0=obj.href;if(src0){src0=src0.replace(_webPath,'');sRet+=src0;}}
else if(tag=='area'){sRet+=',shape,';src0=obj.shape;if(src0){sRet+=src0;}
sRet+=',coords,';src0=obj.coords;if(src0){sRet+=src0.replace(/\,/g,'_');}}
else if(tag=='object'){sRet+=',data,';src0=obj.data;if(src0){sRet+=src0;}
sRet+=',archive,';src0=obj.archive;if(src0){sRet+=src0;}}
_editorOptions.ideCurrentobj=obj;}}}
return sRet;}
function _setbodyBk(bkFile,bkTile){if(_editorOptions&&_editorOptions.client){_setElementStyleValue(_editorOptions.elementEditedDoc.body,'backgroundImage','background-image',bkFile);_setElementStyleValue(_editorOptions.elementEditedDoc.body,'backgroundRepeat','background-repeat',bkTile);_editorOptions.pageChanged=true;return true;}
return false;}
function _getIdList(){var s='';function getidlist(){if(e.childNodes&&e.childNodes.length>0){for(var i=0;i<e.childNodes.length;i++){if(e.childNodes[i].getAttribute){var g=e.childNodes[i].getAttribute('limnorId');if(g&&g.length>0){if(e.childNodes[i].id&&e.childNodes[i].id.length>0){}
else{e.childNodes[i].id=_createId(e.childNodes[i]);}
if(s.length>0){s+=',';}
s+=(g+':'+e.childNodes[i].id);}}}}}
getidlist(_editorOptions.elementEditedDoc.body);return s;}
function _setGuidById(id,guid){if(_editorOptions.client.setGuidById.apply(_editorOptions.elementEditedWindow,[id,guid])){_editorOptions.pageChanged=true;return true;}
return false;}
function _getIdByGuid(guid){function getidbyguid(e){if(e.childNodes&&e.childNodes.length>0){for(var i=0;i<e.childNodes.length;i++){if(e.childNodes[i].getAttribute){var g=e.childNodes[i].getAttribute('limnorId');if(g&&g==guid){if(e.childNodes[i].id&&e.childNodes[i].id.length>0){}
else{e.childNodes[i].id=_createId(e.childNodes[i]);}
return e.childNodes[i].id;}}
var id=getidbyguid(e.childNodes[i]);if(id&&id.length>0){return id;}}}}
return getidbyguid(_editorOptions.elementEditedDoc.body);}
function _setGuid(guid){if(_editorOptions.ideCurrentobj){if(guid&&guid.length>0){var g=_editorOptions.ideCurrentobj.getAttribute('limnorId');if(g&&g.length>0){return 6;}
else{_editorOptions.ideCurrentobj.setAttribute('limnorId',guid);return 0;}}
else{return 3;}}
else{return 1;}}
function _onBeforeIDErun(){if(_editorOptions){JsonDataBinding.pollModifications();if(_comboInput&&_comboInput.oncombotxtChange){_comboInput.oncombotxtChange();}}}
function _doCopy(){if(_editorOptions){if(_editorOptions.selectedObject){captureSelection({target:_editorOptions.selectedObject},true);if(_editorOptions.selectedHtml&&_editorOptions.selectedHtml.length>0){window.clipboardData.setData('text',_editorOptions.selectedHtml);}
else{var tag=_editorOptions.selectedObject.tagName?_editorOptions.selectedObject.tagName.toLowerCase():'';if(tag!='body'&&tag!='html'&&tag!='link'&&tag!='head'){var p=_editorOptions.selectedObject.parentNode;while(p){tag=p.tagName?p.tagName.toLowerCase():'';if(tag=='body'){window.clipboardData.setData('text',_editorOptions.selectedObject.outerHTML);return;}
p=p.parentNode;}}}}}}
function _doPaste(){var data=window.clipboardData.getData('text');if(typeof data!='undefined'&&data!=null&&data.length>0){if(data.substr(0,1)=='<'&&data.substr(data.length-1,1)=='>'){var div=_createElement('div');div.innerHTML=data;if(div.children.length>0){var c=div.children[0];div.removeChild(c);insertNodeOverSelection(c,c.tagName);if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}}
else{if(_editorOptions.elementEditedDoc){var range=_editorOptions.elementEditedDoc.selection.createRange();range.pasteHTML(data);if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}}}}
function _pasteToHtmlInput(txt,selStart,selEnd){if(_editorOptions&&_editorOptions.propInput){var newText='';if(selStart>=0&&selStart<_editorOptions.propInput.value.length)
newText=_editorOptions.propInput.value.substr(0,selStart);newText=newText+txt;if(selEnd>=0&&selEnd<_editorOptions.propInput.value.length)
newText=newText+_editorOptions.propInput.value.substr(selEnd);else
newText=_editorOptions.propInput.value+newText;_editorOptions.propInput.value=newText;}}
function _createNewStyle(obj){if(_editorOptions&&_editorOptions.client){var tag=obj.tagName.toLowerCase();if(tag=='input'){var tp=obj.getAttribute('type');if(tp){tag=tp;}}
var bn=tag+'style';return _editorOptions.client.createNewStyleName.apply(_editorOptions.editorWindow,[bn]);}}
function getyoutubeid(v){if(v){v=v.trim();var pos=v.toLowerCase().indexOf('v=');if(pos>=0){v=v.substr(pos+2);}
else{pos=v.lastIndexOf('/');if(pos>=0){v=v.substr(pos+1);}}
pos=v.indexOf('&');if(pos>0){v=v.substr(0,pos);}}
else{v='';}
return v;}
function _updateYoutubeTarget(a){var yid=a.getAttribute('youtube');var vid=a.getAttribute('youtubeID');if(yid&&yid!=''&&vid&&vid!=''){a.setAttribute('onclick',"javascript:document.getElementById('"+yid+"').src='http://www.youtube.com/embed/"+vid+"';");a.savedhref='';a.savedtarget='';a.removeAttribute('mediaFile');}
else{var mpr=a.getAttribute('mediaPlayer');var med=a.getAttribute('mediaFile');if(mpr&&mpr!=''&&med&&med!=''){_updateMediaTarget(a);}
else{a.removeAttribute('onclick');}}}
function _updateMediaTarget(a){var mpr=a.getAttribute('mediaPlayer');var med=a.getAttribute('mediaFile');if(mpr&&mpr!=''&&med&&med!=''){a.setAttribute('onclick',"javascript:var e=document.getElementById('"+mpr+"');if(!e) return;var p=e.parentNode;if(!p) return;var o=e.cloneNode(false);o.src='"+med+"';p.insertBefore(o, e);p.removeChild(e);o.id=e.id;");a.savedhref='';a.savedtarget='';a.removeAttribute('youtubeID');}
else{var yid=a.getAttribute('youtube');var vid=a.getAttribute('youtubeID');if(yid&&yid!=''&&vid&&vid!=''){_updateYoutubeTarget(a);}
else{a.removeAttribute('onclick');}}}
function _createId(obj){var tag=obj.tagName.toLowerCase();if(tag=='input'){var tp=obj.getAttribute('type');if(tp){tag=tp;}}
var n=1;var newId=tag+n;while(_editorOptions.elementEditedDoc.getElementById(newId)){n++;newId=tag+n;}
if(_editorOptions.ideUsedNames&&_editorOptions.ideUsedNames.length>0){var exist=true;while(exist){exist=false;for(var i=0;i<_editorOptions.ideUsedNames.length;i++){if(_editorOptions.ideUsedNames[i]==newId){exist=true;break;}}
if(exist){n++;newId=tag+n;}}}
return newId;}
function _createOrGetId(guid){if(_editorOptions.ideCurrentobj){if(_editorOptions.ideCurrentobj.id&&_editorOptions.ideCurrentobj.id.length>0){}
else{_editorOptions.ideCurrentobj.id=_createId(_editorOptions.ideCurrentobj);_editorOptions.pageChanged=true;}
if(guid&&guid.length>0){var g=_editorOptions.ideCurrentobj.getAttribute('limnorId');if(g&&g.length>0){}
else{_editorOptions.ideCurrentobj.setAttribute('limnorId',guid);_editorOptions.pageChanged=true;}}
return _editorOptions.ideCurrentobj.id;}
else{return'';}}
function _getMaps(){var ret='';var maps=document.getElementsByTagName('map');if(maps){for(var i=0;i<maps.length;i++){if(!(maps[i].id)){maps[i].id=_createId(maps[i]);}
var s=_getSelectedObject(maps[i]);if(i>0){ret+='|';}
ret+=s;}}
return ret;}
function _getAreas(mapId){var ret='';var map=document.getElementById(mapId);if(map){var arealist=map.getElementsByTagName('area');if(arealist){for(var i=0;i<arealist.length;i++){if(!(arealist[i].id)){arealist[i].id=_createId(arealist[i]);}
var s=_getSelectedObject(arealist[i]);if(i>0){ret+='|';}
ret+=s;}}}
return ret;}
function _createNewMap(guid){var map=document.createElement('map');var mapId=_createId(map);map.id=mapId;map.setAttribute('limnorId',guid);document.body.appendChild(map);return _getSelectedObject(map);}
function _setMapAreas(mapId,arealist){var map=document.getElementById(mapId);if(map){while(map.hasChildNodes()){map.removeChild(map.firstChild);}
var areaStrings=arealist.split('|');for(var i=0;i<areaStrings.length;i++){var ss=areaStrings[i].split(';');if(ss.length>3){var a;a=document.createElement('area');map.appendChild(a);a.id=ss[0];a.setAttribute('limnorId',ss[1]);a.setAttribute('shape',ss[2]);a.setAttribute('coords',ss[3]);}}}}
function _bringToFront(){_divProp.style.zIndex=JsonDataBinding.getPageZIndex(_divProp)+1;}
function _setInlineTarget(pageHolder){var div=_createElement('div');_appendChild(_editorOptions.elementEditedDoc.body,div);div.style.width='100%';div.style.height='100%';div.contentEditable=true;div.jsData={};div.jsData.isTop=true;div.jsData.createSubEditor=function(p,c){if(c==div){var ret={};ret.obj=div;ret.subEditor=true;ret.isTop=true;ret.allowCommands=true;ret.getProperties=function(){var ps={};ps.props=new Array();ps.props.push(langProp);return ps;};ret.getString=function(){return'Contents';}
return ret;}}
var ln=document.body.parentNode.getAttribute('lang');if(ln){div.setAttribute('lang',ln);}
_editorOptions.elementEdited=div;_editorOptions.selectedObject=null;selectEditElement(div);toggleShowHideProps();}
function _setEditorTarget(editTarget){if(!editTarget){alert('edit target cannot be empty');return;}
if(_editorOptions==editTarget){_bringToFront();}
else{_editorOptions=editTarget;if(!_editorOptions.redbox){_editorOptions.redboxId='redbox889932';_editorOptions.redbox=_editorOptions.elementEditedDoc.getElementById(_editorOptions.redboxId);if(_editorOptions.redbox&&!_editorOptions.redbox.parentNode){_appendChild(_editorOptions.elementEditedDoc.body,_editorOptions.redbox);}}
if(!_editorOptions.redbox){_editorOptions.redbox=_createElement('img');_editorOptions.redbox.forProperties=true;_editorOptions.redbox.contentEditable=false;_editorOptions.redbox.id=_editorOptions.redboxId;_editorOptions.redbox.className=_editorOptions.redboxId;_editorOptions.redbox.style.height='4px';_editorOptions.redbox.style.width='4px';_editorOptions.redbox.src='/libjs/redbox.png';_editorOptions.redbox.style.position='absolute';_appendChild(_editorOptions.elementEditedDoc.body,_editorOptions.redbox);if(_editorOptions.initLocation){_divProp.style.left=_editorOptions.initLocation.x+'px';_divProp.style.top=_editorOptions.initLocation.y+'px';if(_editorOptions.initLocation.w){_divProp.style.width=_editorOptions.initLocation.w+'px';}
if(_editorOptions.initLocation.h){_divProp.style.height=_editorOptions.initLocation.h+'px';}}
if(_editorOptions.forIDE){_imgOK.style.display='none';_imgCancel.style.display='none';_imgSave.style.display='none';_imgReset.style.display='none';}
else{if(_editorOptions.hideOKbutton){_imgOK.style.display='none';}
else{_imgOK.style.display='inline';}}
_initSelection();}
else{var o=_editorOptions.selectedObject?_editorOptions.selectedObject:_editorOptions.elementEdited;_editorOptions.selectedObject=null;selectEditElement(o);}
if(_editorOptions.isEditingBody&&_editorOptions.client){if(_editorOptions.client.getPageHolder.apply(_editorOptions.editorWindow)){var p=JsonDataBinding.ElementPosition.getElementPosition(_editorOptions.client.getPageHolder());if(p){_divProp.style.left=(p.x+80)+'px';_divProp.style.top=(p.y+60)+'px';}}}
if(!_divProp.expanded){_togglePropertiesWindow();}
_bringToFront();_showResizer();showFontCommands();adjustSizes();_divSelectElement.jsData.checkAdditables();}}
function addUndoItem(undoitem){if(!_editorOptions.undoList){_editorOptions.undoList=new Array();}
if(!_editorOptions.undoState){_editorOptions.undoState={};}
if(_editorOptions.undoState.index==_editorOptions.undoList.length-1){if(!_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML){_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML=undoitem.redoInnerHTML;}}
_editorOptions.undoList.push(undoitem);_editorOptions.undoState.index=_editorOptions.undoList.length-1;_editorOptions.undoState.state=UNDOSTATE_REDO;showUndoRedo();}
function _handleBackspace(fromClient){if(fromClient){if(_editorOptions){if(_editorOptions.elementEditedDoc.activeElement&&_editorOptions.elementEditedDoc.activeElement.type&&_editorOptions.elementEditedDoc.activeElement.type.toLowerCase()=="text"){return true;}
if(_editorOptions.selectedObject&&_editorOptions.selectedObject.tagName){var tag=_editorOptions.selectedObject.tagName.toLowerCase();if(tag!='table'){return true;}}}
return false;}
else{if(!document.activeElement||!document.activeElement.type||document.activeElement.type.toLowerCase()!="text"){return false;}}
return true;}
function _onclientkeyup(obj){captureSelection({target:obj});}
function _setChanged(){_editorOptions.pageChanged=true;}
function _onclientmouseup(obj,w){if(_editorOptions){if(_custMouseDown){return;}
if(w){if(_editorOptions.getPageHolder()){var ph=_editorOptions.getPageHolder();if(w!=ph){if(ph&&ph.jsData&&ph.jsData.onDocMouseUp){ph.jsData.onDocMouseUp();}}}}
captureSelection({target:obj});mouseup({target:obj,client:true});if(_editorOptions.isEditingBody&&_editorOptions.elementEditedDoc.body!=obj){selectEditElement(obj);}}}
function _onclientmousemove(x,y,w){if(_editorOptions){if(_editorOptions.styleChanged){_editorOptions.styleChanged=false;if(_editorOptions.selectedObject){var cssTD=getPropertyCell('cssText');if(cssTD){var s=_editorOptions.selectedObject.style.cssText;if(s){var txts=cssTD.getElementsByTagName('input');if(txts&&txts.length>0){for(i=0;i<txts.length;i++){if(txts[i].type=='text'){cssTD.value=s;txts[i].value=s;break;}}}}}}}}
if(w){var pos=JsonDataBinding.ElementPosition.getElementPosition(w);if(_editorOptions&&_editorOptions.getPageHolder()){var ph=_editorOptions.getPageHolder();if(w!=ph){if(ph&&ph.jsData&&ph.jsData.onDocMouseMove){var pos2=JsonDataBinding.ElementPosition.getElementPosition(ph);ph.jsData.onDocMouseMove({pageX:pos.x+x-pos2.x,pageY:pos.y+y-pos2.y});}}}
mousemove({pageX:pos.x+x,pageY:pos.y+y});}}
function _onclientmousedown(obj,x,y,isRightClick){if(_editorOptions){if(_custMouseDown){_custMouseDown({target:obj,x:x,y:y});return;}
if(_isIE){if(_editorOptions.forIDE){var c=selectEditElement(obj);if(c&&isRightClick){var objStr=_getSelectedObject(c);if(typeof(limnorStudio)!='undefined')limnorStudio.onRightClickElement(objStr,x,y);else window.external.OnRightClickElement(objStr,x,y);}}}}}
function _initSelection(){if(!_editorOptions)return;_editorOptions.selectedObject=null;selectEditElement(_editorOptions.elementEdited);captureSelection({target:_editorOptions.elementEdited});adjustSizes();}
function _setUseMap(imgId,mapId){var img=document.getElementById(imgId);if(img){img.setAttribute('usemap',mapId);_editorOptions.pageChanged=true;}}
function _setPropertyValue(propertyName,propertyValue){if(_editorOptions.ideCurrentobj){_editorOptions.ideCurrentobj[propertyName]=propertyValue;_editorOptions.pageChanged=true;showProperties(_editorOptions.ideCurrentobj);return 0;}
else
return 1;}
function _appendArchiveFile(objId,filePath){var obj;if(objId){obj=_editorOptions.elementEditedDoc.getElementById(objId);}
if(!obj){obj=_editorOptions.ideCurrentobj;}
if(obj){obj.archive=obj.archive?(obj.archive+' '+filePath):filePath;showProperties(obj);return obj.archive}
return'';}
function _notifyUsedNames(usedNames){_editorOptions.ideUsedNames='';if(usedNames&&usedNames.length>0){_editorOptions.ideUsedNames=usedNames.split(',');}}
function _getChildElementIds(p){var sRet='';if(p){if(p.id&&p.id.length>0){sRet=p.id;}
if(p.childNodes&&p.childNodes.length>0){for(var i=0;i<p.childNodes.length;i++){var s=_getChildElementIds(p.childNodes[i]);if(s.length>0){if(sRet.length==0){sRet=s;}
else{sRet+=',';sRet+=s;}}}}}
return sRet;}
function _getNamesUsed(){return _getChildElementIds(document.body);}
function showError(errorMsg){var sp;if(_divError){sp=_divError.sp;}
else{_divError=document.createElement("div");_divError.style.backgroundColor='#FFFFE0';_divError.style.position='absolute';_divError.style.border="3px double yellow";_divError.style.color='red';appendToBody(_divError);var tbl=document.createElement("table");_divError.appendChild(tbl);tbl.border=0;tbl.cellPadding=0;tbl.cellSpacing=0;tbl.width='100%';tbl.height='100%';var tblBody=JsonDataBinding.getTableBody(tbl);var tr=tblBody.insertRow(-1);td=document.createElement('td');tr.appendChild(td);td.align='center';sp=document.createElement('span');td.appendChild(sp);_divError.sp=sp;sp.style.color='red';sp.style.textAlign='center';sp.style.align='center';sp.style.verticalAlign='middle';tr.style.align='center';tr.style.verticalAlign='middle';}
sp.innerHTML=errorMsg;_divError.style.width='300px';_divError.style.height='185px';var zi=JsonDataBinding.getPageZIndex(_divError)+1;_divError.style.zIndex=zi;_divError.style.display='block';JsonDataBinding.anchorAlign.alignCenterElement(_divError);_divError.clicks=1;}
function showToolTips(element,tips){if(!_divToolTips){_divToolTips=document.createElement("div");_divToolTips.style.backgroundColor='#FFFFE0';_divToolTips.style.position='absolute';_divToolTips.style.border="1px double yellow";_divToolTips.style.color='blue';var sp=document.createElement('span');sp.style.backgroundColor='#FFFF99';_divToolTips.appendChild(sp);_divToolTips.sp=sp;appendToBody(_divToolTips);}
var zi=JsonDataBinding.getPageZIndex(_divToolTips)+1;_divToolTips.style.zIndex=zi;_divToolTips.style.display='block';_divToolTips.clicks=1;var pos=JsonDataBinding.ElementPosition.getElementPosition(element);_divToolTips.style.left=(pos.x-20)+'px';_divToolTips.style.top=(pos.y+20)+'px';_divToolTips.sp.innerHTML='&nbsp;&nbsp;'+tips+'&nbsp;&nbsp;';_divToolTips.style.display='block';}
function elementToString(e,typeName){if(!e)return'';if(e.subEditor){return e.getString();}
var nm='';if(isCssNode(e)){nm='css file';}
else{if(typeName)
nm=typeName;else if(e.typename){nm=e.typename;}
else{if(e.getAttribute){nm=e.getAttribute('typename');}
if(!nm||nm.length==0){if(e.tagName){var tag=e.tagName.toLowerCase();if(tag=='input'){if(e.type){nm=e.type;}
else{nm=e.tagName;}}
else{nm=e.tagName;}}}}}
var eid='';if(e.id){if(nm=='fileupload'&&e.id.length>1){eid=e.id.substr(0,e.id.length-1);}
else
eid=e.id;}
var name=e.name;if(!name){name=e.getAttribute('Name');}
if(name){if(eid.length>0){eid=eid+',';}
eid=eid+name;}
name=e.getAttribute('styleName');if(name){if(eid.length>0){eid=eid+',';}
eid=eid+name;}
if(eid.length>0){return nm+'('+eid+')';}
if(e.id){return nm+'('+e.id+')';}
return nm;}
function setSingleValueAttr(o,name,val){if(val&&val!='false'){o.setAttribute(name,name);}
else{o.removeAttribute(name);}}
function getSingleValueAttr(o,name){return o.hasAttribute(name);}
function getPercentAttr(o,name){if(o&&o.getAttribute){var v=o.getAttribute(name);if(typeof(v)!='undefined'&&(v||v===0)){return Math.round(v);}}}
function setPercentAttr(o,name,v){if(o&&o.setAttribute){if(v<0)v=0;if(v>100)v=100;o.setAttribute(name,v);}}
function getAllIds(){var ids=[];if(_editorOptions){var rid=_editorOptions.redbox?_editorOptions.redbox.id:'redbox889932';function getid(a){if(a){if(a.id&&a.id!=rid){ids.push(a.id);}
if(a.childNodes){for(var i=0;i<a.childNodes.length;i++){getid(a.childNodes[i]);}}}}
if(_editorOptions&&_editorOptions.elementEditedDoc){getid(_editorOptions.elementEditedDoc.body);}}
return ids;}
var langProp={name:'lang',editor:EDIT_ENUM,allowEdit:true,byAttribute:true,desc:'the language of the element\'s content. Use ISO Language Codes.',values:function(){var ls=[['Abkhazian','ab'],['Afar','aa'],['Afrikaans','af'],['Albanian','sq'],['Amharic','am'],['Arabic','ar'],['Aragonese','an'],['Armenian','hy'],['Assamese','as'],['Aymara','ay'],['Azerbaijani','az'],['Bashkir','ba'],['Basque','eu'],['Bengali(Bangla)','bn'],['Bhutani','dz'],['Bihari','bh'],['Bislama','bi'],['Breton','br'],['Bulgarian','bg'],['Burmese','my'],['Byelorussian(Belarusian)','be'],['Cambodian','km'],['Catalan','ca'],['Chinese','zh'],['Corsican','co'],['Croatian','hr'],['Czech','cs'],['Danish','da'],['English','en'],['Esperanto','eo'],['Estonian','et'],['Faeroese','fo'],['Farsi','fa'],['Fiji','fj'],['Finnish','fi'],['French','fr'],['Frisian','fy'],['Galician','gl'],['Gaelic(Scottish)','gd'],['Gaelic(Manx)','gv'],['Georgian','ka'],['German','de'],['Greek','el'],['Greenlandic','kl'],['Guarani','gn'],['Gujarati','gu'],['Haitian Creole','ht'],['Hausa','ha'],['Hebrew(he)','he'],['Hebrew(iw)','iw'],['Hindi','hi'],['Hungarian','hu'],['Icelandic','is'],['Ido','io'],['Indonesian(id)','id'],['Indonesian(in)','in'],['Interlingua','ia'],['Interlingue','ie'],['Inuktitut','iu'],['Inupiak','ik'],['Irish','ga'],['Italian','it'],['Japanese','ja'],['Javanese','jv'],['Kannada','kn'],['Kashmiri','ks'],['Kazakh','kk'],['Kinyarwanda(Ruanda)','rw'],['Kirghiz','ky'],['Kirundi(Rundi)','rn'],['Korean','ko'],['Kurdish','ku'],['Laothian','lo'],['Latin','la'],['Latvian(Lettish)','lv'],['Limburgish(Limburger)','li'],['Lingala','ln'],['Lithuanian','lt'],['Macedonian','mk'],['Malagasy','mg'],['Malay','ms'],['Malayalam','ml'],['Maltese','mt'],['Maori','mi'],['Marathi','mr'],['Moldavian','mo'],['Mongolian','mn'],['Nauru','na'],['Nepali','ne'],['Norwegian','no'],['Occitan','oc'],['Oriya','or'],['Oromo(Afaan Oromo)','om'],['Pashto(Pushto)','ps'],['Polish','pl'],['Portuguese','pt'],['Punjabi','pa'],['Quechua','qu'],['Rhaeto-Romance','rm'],['Romanian','ro'],['Russian','ru'],['Samoan','sm'],['Sangro','sg'],['Sanskrit','sa'],['Serbian','sr'],['Serbo-Croatian','sh'],['Sesotho','st'],['Setswana','tn'],['Shona','sn'],['Sichuan Yi','ii'],['Sindhi','sd'],['Sinhalese','si'],['Siswati','ss'],['Slovak','sk'],['Slovenian','sl'],['Somali','so'],['Spanish','es'],['Sundanese','su'],['Swahili(Kiswahili)','sw'],['Swedish','sv'],['Tagalog','tl'],['Tajik','tg'],['Tamil','ta'],['Tatar','tt'],['Telugu','te'],['Thai','th'],['Tibetan','bo'],['Tigrinya','ti'],['Tonga','to'],['Tsonga','ts'],['Turkish','tr'],['Turkmen','tk'],['Twi','tw'],['Uighur','ug'],['Ukrainian','uk'],['Urdu','ur'],['Uzbek','uz'],['Vietnamese','vi'],['Volap�k','vo'],['Wallon','wa'],['Welsh','cy'],['Wolof','wo'],['Xhosa','xh'],['Yiddish(yi)','yi'],['Yiddish(ji)','ji'],['Yoruba','yo'],['Zulu','zu']];var ret=new Array();ret.push({text:'',value:''});for(var i=0;i<ls.length;i++){var s=ls[i];ret.push({text:s[0],value:s[1]});}
return ret;},onsetprop:function(){var head=_editorOptions.elementEditedDoc.getElementsByTagName('head')[0];var mts=head.getElementsByTagName('meta');var v4=false;var v5=false;if(mts){for(var i=0;i<mts.length;i++){var hy=mts[i].getAttribute('http-equiv');if(hy=='content-type'){mts[i].setAttribute('content','text/html; charset=utf-8');v4=true;}
hy=mts[i].getAttribute('charset');if(typeof(hy)!='undefined'&&hy!=null){mts[i].setAttribute('charset','UTF-8');v5=true;}
if(v4&&v5)
break;}}
var m;if(!v4){m=_createElement('meta');_appendChild(head,m);m.setAttribute('http-equiv','content-type');m.setAttribute('content','text/html; charset=utf-8');}
if(!v5){m=_createElement('meta');_appendChild(head,m);m.setAttribute('charset','UTF-8');}}};var cssClass={name:'class',editor:EDIT_ENUM,allowEdit:true,values:function(o){return _editorOptions.client.getClassNames.apply(_editorOptions.elementEditedWindow,[o]);},desc:'one or more class names, separated by spaces, for specifying styles. Each class name refers to a class in a style sheet.',getter:function(o){if(o.className&&typeof(o.className.baseVal)!='undefined')
return o.className.baseVal;return o.className;},setter:function(o,val){if(o.className&&typeof(o.className.baseVal)!='undefined'){o.className.baseVal=val;return;}
o.className=val;}};var idProp={name:'id',desc:'a unique id for an element',editor:EDIT_TEXT,getter:function(o){return o.subEditor?o.obj.id:o.id;},setter:function(o,val){if(o.subEditor)o=o.obj;if(_divError){_divError.style.display='none';}
if(o.id!=val){var g=o.getAttribute('limnorId');if(val==null||val.length==0){if(g){showError('id for this element cannot be removed because it is used in programming.');return;}}
if(val&&val.length>0){if(val=='body'){showError('"body" cannot be used as id.');return;}
if(HtmlEditor.IsNameValid(val)){var o0=document.getElementById(val);if(o0){showError('The id is in use.');return;}}
else{showError('The id is invalid. Please only use alphanumeric characters. The first letter can only be a-z, A-Z, $, and underscore');return;}}
o.id=val;if(_editorOptions.forIDE){if(g){if(typeof(limnorStudio)!='undefined')limnorStudio.onElementIdChanged(g,val);else window.external.OnElementIdChanged(g,val);}}}}};var validateCmdProp={name:'validateUl',toolTips:'validate and fix tree view structure errors',editor:EDIT_CMD,IMG:'/libjs/validate.png',action:function(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var obj=c.owner;if(obj){var tag=obj.tagName?obj.tagName.toLowerCase():'';if(tag=='ul'){var errCount=0;function validateUL(ul){var lastli,i,j;var invals=new Array();for(i=0;i<ul.children.length;i++){var c=ul.children[i];var tag0=c.tagName.toLowerCase();if(tag0=='li'){for(j=0;j<c.children.length;j++){if(c.children[j].tagName.toLowerCase()=='ul'){validateUL(c.children[j]);}}
lastli=c;}
else if(tag0=='ul'){var p=ul;if(lastli){p=null;for(j=0;j<lastli.children.length;j++){if(lastli.children[j].tagName.toLowerCase()=='ul'){p=lastli.children[j];break;}}
if(!p){p=_createElement('ul');_appendChild(lastli,p);}}
var cs=new Array();for(j=0;j<c.children.length;j++){if(c.children[j].tagName.toLowerCase()=='li'){cs.push(c.children[j]);}}
for(j=0;j<cs.length;j++){_appendChild(p,cs[j]);validateUL(cs[j]);}
invals.push(c);}}
for(i=0;i<invals.length;i++){ul.removeChild(invals[i]);errCount++;}}
validateUL(obj);if(errCount>0){alert('Errors fixed:'+errCount+'. ');while(obj){if(obj.jsData&&obj.jsData.resetState){obj.jsData.resetState();break;}
obj=obj.parentNode;}}
else{alert('No error found');}}}}}};var propFontFamily={name:'fontFamily',desc:'specifies the font for an element.',forStyle:true,cat:PROP_FONT,editor:EDIT_ENUM,values:fontList};var propFontSize={name:'fontSize',forStyle:true,cat:PROP_FONT,editor:EDIT_ENUM,values:fontSizes,allowEdit:true,canbepixel:true,onselectindexchanged:function(sel){if(sel.selectedIndex){var sv=sel.options[sel.selectedIndex].value;if(sv){if(sv=='%'||sv=='size in px, cm, ...'){return false;}
return true;}}
return false;},ontextchange:function(txt,sel){if(txt&&sel.selectedIndex){var sv=sel.options[sel.selectedIndex].value;if(sv=='size in px, cm, ...')
return txt;if(sv=='%'&&txt.length>0)
if(txt.substr(txt.length-1,1)=='%')
return txt;else
return txt;}}};var commonProperties=[idProp,{name:'accessKey',editor:EDIT_TEXT,desc:'a shortcut key to activate/focus an element'},{name:'cursor',forStyle:true,editor:EDIT_ENUM,allowEdit:true,values:HtmlEditor.cursorValues,desc:'specifies the type of cursor to be displayed when pointing on an element.'},{name:'styleShare',desc:'controls whether new style settings should be shared or not',editor:EDIT_ENUM,values:['','Share','NotShare'],getter:function(o){var s=o.getAttribute('styleshare');if(typeof s=='undefined'||s==null||s.length==0||s.toLowerCase()!='notshare'){return'Share';}
return'NotShare';},setter:function(o,s){if(typeof s=='undefined'||s==null||s.length==0||s.toLowerCase()!='notshare'){o.removeAttribute('styleshare');}
else{o.setAttribute('styleshare','NotShare');}}},{name:'styleName',desc:'Setting styleName to a non-empty string will add it as a class name to the element; all your modifications of the element styles will be added to the class and thus applied to all elements using the class.',byAttribute:true,editor:EDIT_ENUM,allowEdit:true,values:function(o){return _editorOptions.client.getStyleNames.apply(_editorOptions.elementEditedWindow,[o]);},getter:function(o){return o.getAttribute('styleName');},setter:function(o,v){var curStyleName=o.getAttribute('styleName');if(v){v=v.toLowerCase();o.setAttribute('styleName',v);}
else{o.removeAttribute('styleName');}
if(curStyleName&&curStyleName.length>0&&curStyleName!=v){_editorOptions.client.removeClass.apply(_editorOptions.elementEditedWindow,[o,curStyleName]);}},onsetprop:function(o,v){if(v&&v.length>0){v=v.toLowerCase();_editorOptions.client.addClass.apply(_editorOptions.elementEditedWindow,[o,v]);}}},cssClass,{name:'title',editor:EDIT_TEXT,desc:'extra information about an element'}];var frameWidthProp={name:'fixedWidth',editor:EDIT_NUM,isPixel:true,desc:'Specifies the width, in pixels, of an iframe element',getter:function(o){return o.width;},setter:function(o,v){o.style.width='';JsonDataBinding.removeStyleAttribute(o,'width');if(_editorOptions&&_editorOptions.isEditingBody)
_editorOptions.client.setElementStyleValue.apply(_editorOptions.elementEditedWindow,[o,'width','width',null]);if(isNaN(v))o.removeAttribute('width');else o.width=v;}};var frameHeightProp={name:'fixedHeight',editor:EDIT_NUM,isPixel:true,desc:'Specifies the height, in pixels, of an iframe element',getter:function(o){return o.height;},setter:function(o,v){o.style.height='';JsonDataBinding.removeStyleAttribute(o,'height');if(_editorOptions&&_editorOptions.isEditingBody)
_editorOptions.client.setElementStyleValue.apply(_editorOptions.elementEditedWindow,[o,'height','height',null]);if(isNaN(v))o.removeAttribute('height');else o.height=v;}};var innerHtmlProp={name:'innerHtml',editor:EDIT_CMD,desc:'Set innerHTML of the element',IMG:'/libjs/innerHTML.png',action:setElementInnerHTML};var verticalAlignProp={name:'verticalAlign',forStyle:true,editor:EDIT_ENUM,values:HtmlEditor.verticalAlign,allowEdit:true,onselectindexchanged:function(sel){if(sel.selectedIndex){var sv=sel.options[sel.selectedIndex].value;if(sv){if(sv=='%'||sv=='length in px'){return false;}
return true;}}
return false;},ontextchange:function(txt,sel){if(txt&&sel.selectedIndex){var sv=sel.options[sel.selectedIndex].value;if(sv=='length in px')
return txt;if(sv=='%'&&txt.length>0)
if(txt.substr(txt.length-1,1)=='%')
return txt;else
return txt;}
return txt;}};var propFontStyle={name:'fontStyle',desc:'Specifies the font style for a text.',forStyle:true,cat:PROP_FONT,editor:EDIT_ENUM,values:HtmlEditor.fontStyles};var propFontWeight={name:'fontWeight',desc:'Sets how thick or thin characters in text should be displayed.',forStyle:true,cat:PROP_FONT,editor:EDIT_ENUM,values:HtmlEditor.fontWeights};var propFontVariant={name:'fontVariant',desc:'Specifies whether or not a text should be displayed in a small-caps font. In a small-caps font, all lowercase letters are converted to uppercase letters. However, the converted uppercase letters appears in a smaller font size than the original uppercase letters in the text.',forStyle:true,cat:PROP_FONT,editor:EDIT_ENUM,values:HtmlEditor.fontVariants};var editor_font=[propFontFamily,propFontSize,propFontStyle,propFontWeight,propFontVariant,{name:'color',desc:'Specifies text color',forStyle:true,cat:PROP_FONT,editor:EDIT_COLOR}];var editor_txt0=[langProp,{name:'tabindex',editor:EDIT_NUM},{name:'backgroundColor',forStyle:true,editor:EDIT_COLOR},{name:'direction',forStyle:true,editor:EDIT_ENUM,values:['','ltr','rtl','inherit']},{name:'font',editor:EDIT_PROPS,editorList:editor_font,desc:'font related properties'},{name:'text-decoration',editor:EDIT_ENUM,values:['','none','underline','overline','line-through','blink','inherit'],cssName:'text-decoration'}];var editor_txt=[{name:'txt',editor:EDIT_PROPS,editorList:editor_txt0,desc:'text related properties'},{name:'textAlign',editor:EDIT_ENUM,values:HtmlEditor.textAlign,cssName:'text-align'}];var editor_heading=[{name:'txt',editor:EDIT_PROPS,editorList:editor_txt,desc:'text related properties'}];var editor_common_txt=[{name:'txt',editor:EDIT_PROPS,editorList:editor_txt,desc:'text related properties'}];var editor_rel=['','alternate','appendix','bookmark','chapter','contents','copyright','glossary','help','home','index','next','prev','section','start','stylesheet','subsection'];var htmlsaveToProps=[{name:'folder name',editor:EDIT_TEXT,getter:function(o){return _editorOptions.saveToFolder;},setter:function(o,val){_editorOptions.saveToFolder=val;}},{name:'filename',editor:EDIT_TEXT,getter:function(o){return _editorOptions.saveTo;},setter:function(o,val){_editorOptions.saveTo=val;}}];var backgroundProps=[{name:'backgroundImage',forStyle:true,cat:PROP_BK,isUrl:true,maxSize:1048576,filetypes:'.image',title:'Select Image File',editor:EDIT_TEXT,desc:'The background-image property sets one or more background images for an element.'},{name:'backgroundRepeat',forStyle:true,cat:PROP_BK,editor:EDIT_ENUM,values:['','repeat','repeat-x','repeat-y','no-repeat','inherit'],desc:'The background-repeat property sets if/how a background image will be repeated'},{name:'backgroundPosition',forStyle:true,cat:PROP_BK,allowEdit:true,editor:EDIT_ENUM,values:['','left top','left center','left bottom','right top','right center','right bottom','center top','center center','center bottom'],desc:'The background-position property sets the starting position of a background image.'},{name:'backgroundColor',forStyle:true,cat:PROP_BK,editor:EDIT_COLOR,desc:'The background-color property sets the background color of an element.'},{name:'backgroundAttachment',forStyle:true,cat:PROP_BK,editor:EDIT_ENUM,values:['','scroll','fixed','inherit'],desc:'The background-attachment property sets whether a background image is fixed or scrolls with the rest of the page.'},{name:'backgroundSize',forStyle:true,cat:PROP_BK,allowEdit:true,editor:EDIT_ENUM,values:['','cover','contain'],desc:'The background-size property specifies the size of the background images.'},{name:'backgroundClip',forStyle:true,cat:PROP_BK,editor:EDIT_ENUM,values:['','border-box','padding-box','content-box'],desc:'The background-clip property specifies the painting area of the background.'},{name:'backgroundOrigin',forStyle:true,cat:PROP_BK,editor:EDIT_ENUM,values:['','padding-box','border-box','content-box'],desc:'The background-origin property specifies what the background-position property should be relative to.'},{name:'linearGradientStartColor',forStyle:true,cat:PROP_BK,editor:EDIT_COLOR,desc:'The color for the starting point for forming linear gradient background. See https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient',getter:function(o){return _editorOptions.client.getLinearGradientStartColor.apply(_editorOptions.editorWindow,[o]);},setter:function(o,val){_editorOptions.client.setLinearGradientStartColor.apply(_editorOptions.editorWindow,[o,val]);}},{name:'linearGradientEndColor',forStyle:true,cat:PROP_BK,editor:EDIT_COLOR,desc:'The color for the ending point for forming linear gradient background. See https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient',getter:function(o){return _editorOptions.client.getLinearGradientEndColor.apply(_editorOptions.editorWindow,[o]);},setter:function(o,val){_editorOptions.client.setLinearGradientEndColor.apply(_editorOptions.editorWindow,[o,val]);}},{name:'linearGradientAngle',forStyle:true,cat:PROP_BK,editor:EDIT_NUM,desc:'The angle, in degree, for forming linear gradient background. See https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient',getter:function(o){return _editorOptions.client.getLinearGradientAngle.apply(_editorOptions.editorWindow,[o]);},setter:function(o,val){_editorOptions.client.setLinearGradientAngle.apply(_editorOptions.editorWindow,[o,val]);}}];var borderProps=[{name:'borderColor',desc:' Sets the color of the four borders',forStyle:true,cat:PROP_BORDER,editor:EDIT_COLOR},{name:'borderStyle',desc:' Sets the style of the four borders',forStyle:true,cat:PROP_BORDER,editor:EDIT_ENUM,values:HtmlEditor.borderStyle},{name:'borderWidth',desc:' Sets the width of the four borders. Select a width type or enter a width length, such as, 3px',forStyle:true,cat:PROP_BORDER,editor:EDIT_ENUM,values:HtmlEditor.widthStyle,canbepixel:true,allowEdit:true},{name:'outlineColor',desc:' Sets the color of an outline',forStyle:true,cat:PROP_BORDER,editor:EDIT_COLOR},{name:'outlineStyle',desc:' Sets the style of an outline',forStyle:true,cat:PROP_BORDER,editor:EDIT_ENUM,values:HtmlEditor.borderStyle},{name:'outlineWidth',desc:' Sets the width of an outline. Select a width type or enter a width length, such as, 3px',forStyle:true,cat:PROP_BORDER,editor:EDIT_ENUM,values:HtmlEditor.widthStyle,canbepixel:true,allowEdit:true},{name:'borderImage',desc:' A shorthand property for setting all the border-image-* properties',forStyle:true,cat:PROP_BORDER,editor:EDIT_TEXT},{name:'borderRadius',desc:' A shorthand property for setting all the four border-*-radius properties.',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'boxShadow',desc:' Attaches one or more drop-shadows to the box, by "horizontal vertical blur spread color inset", for example, 10px 10px 5px #888888. You may use the color picker to get color value.',forStyle:true,cat:PROP_BORDER,editor:EDIT_TEXT}];var boxDimProps=[{name:'overflow',desc:' Specifies what happens if content overflows an element\'s box',forStyle:true,cat:PROP_POS,editor:EDIT_ENUM,values:['','visible','hidden','scroll','auto','inherit']},{name:'overflowX',desc:' Specifies whether or not to clip the left/right edges of the content, if it overflows the element\'s content area',forStyle:true,cat:PROP_BOX,editor:EDIT_ENUM,values:HtmlEditor.overflow},{name:'overflowY',desc:' Specifies whether or not to clip the top/bottom edges of the content, if it overflows the element\'s content area',forStyle:true,cat:PROP_BOX,editor:EDIT_ENUM,values:HtmlEditor.overflow},{name:'height',desc:' Sets the height of an element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'maxHeight',desc:' Sets the maximum height of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'maxWidth',desc:' Sets the maximum width of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'minHeight',desc:' Sets the minimum height of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'minWidth',desc:' Sets the minimum width of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'width',desc:' Sets the width of an element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100%',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT}];var multiColsProps=[{name:'columnCount',cat:PROP_MULCOL,desc:'the number of columns an element should be divided into.',editor:EDIT_TEXT,getter:function(o){return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'count']);},setter:function(o,val){_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'columnCount','count',val]);}},{name:'columnWidth',cat:PROP_MULCOL,desc:'width of the columns',editor:EDIT_TEXT,canbepixel:true,getter:function(o){return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'width']);},setter:function(o,val){_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'columnWidth','width',val]);}},{name:'columnGap',cat:PROP_MULCOL,desc:'gap between the columns',editor:EDIT_TEXT,isPixel:true,getter:function(o){return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'gap']);},setter:function(o,val){_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'columnGap','gap',val]);}},{name:'columnRuleStyle',cat:PROP_MULCOL,desc:'style of the rule between columns',editor:EDIT_ENUM,values:HtmlEditor.borderStyle,getter:function(o){return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'rule-style']);},setter:function(o,val){_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'columnRuleStyle','rule-style',val]);}},{name:'columnRuleWidth',cat:PROP_MULCOL,desc:'width of the rule between columns',editor:EDIT_TEXT,canbepixel:true,getter:function(o){return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'rule-width']);},setter:function(o,val){_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'columnRuleWidth','rule-width',val]);}},{name:'columnRuleColor',cat:PROP_MULCOL,desc:'color of the rule between columns',editor:EDIT_COLOR,getter:function(o){return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'rule-color']);},setter:function(o,val){_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow,[o,'columnRuleColor','rule-color',val]);}}];var sizelocProps=[{name:'clientLeft',editor:EDIT_NONE,desc:'width of the left border in pixels. the distance between the offsetLeft property and the true left side of the client area'},{name:'clientTop',editor:EDIT_NONE,desc:'height of the top border in pixels. the distance between the offsetTop property and the true top of the client area. '},{name:'clientWidth',editor:EDIT_NONE,desc:'the width of the visible area for an object, in pixels. the width of the object including padding, but not including margin, border, or scroll bar.'},{name:'clientHeight',editor:EDIT_NONE,desc:'the height of the visible area for an object, in pixels. the height of the object including padding, but not including margin, border, or scroll bar. '},{name:'offsetLeft',editor:EDIT_NONE,desc:'the left position of an object relative to the left side of its offsetParent element, in pixels. the calculated left position of the object relative to the layout or coordinate parent, as specified by the offsetParent property.'},{name:'offsetTop',editor:EDIT_NONE,desc:'the top position of the object relative to the top side of its offsetParent element, in pixels.'},{name:'offsetWidth',editor:EDIT_NONE,desc:'the width of the visible area for an object, in pixels. The value contains the width with the padding, scrollBar, and the border, but does not include the margin.'},{name:'offsetHeight',editor:EDIT_NONE,desc:'the height of the visible area for an object, in pixels. The value contains the height with the padding, scrollBar, and the border, but does not include the margin.'},{name:'scrollLeft',editor:EDIT_NONE,desc:'the number of pixels by which the contents of an object are scrolled to the left.'},{name:'scrollTop',editor:EDIT_NONE,desc:'the number of pixels by which the contents of an object are scrolled upward.'},{name:'scrollWidth',editor:EDIT_NONE,desc:'the total width of an element\'s contents, in pixels. The value contains the width with the padding, but does not include the scrollBar, border, and the margin.'},{name:'scrollHeight',editor:EDIT_NONE,desc:'the total height of an element\'s contents, in pixels. The value contains the height with the padding, but does not include the scrollBar, border, and the margin.'}];var marginDesc='margin can be specified in following ways: auto, inherit, or in percent, i.e. 1%, or in length value, i.e. 2px';var paddingDesc='padding can be specified by length, i.e. 2px, or in percent, i.e. 1%';var marginPaddingProps=[{name:'marginBottom',desc:marginDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'marginLeft',desc:marginDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'marginRight',desc:marginDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'marginTop',desc:marginDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'paddingBottom',desc:paddingDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'paddingLeft',desc:paddingDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'paddingRight',desc:paddingDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT},{name:'paddingTop',desc:paddingDesc,cat:PROP_MARGIN,forStyle:true,canbepixel:true,editor:EDIT_TEXT}];var posProps=[{name:'clip',desc:' Clips an absolutely positioned element. It can be auto, inherit, or rect(top, right, bottom, left), i.e. rect(0px,50px,50px,0px)',forStyle:true,cat:PROP_POS,editor:EDIT_TEXT},{name:'display',desc:' Specifies how a certain HTML element should be displayed',forStyle:true,cat:PROP_POS,editor:EDIT_ENUM,values:['','box','block','flex','inline','inline-block','inline-flex','inline-table','list-item','none','table','table-caption','table-cell','table-column','table-column-group','table-footer-group','table-header-group','table-row','table-row-group','inherit']},{name:'visibility',desc:' Specifies whether or not an element is visible',forStyle:true,cat:PROP_POS,editor:EDIT_ENUM,values:['','visible','hidden','collapse','inherit']},{name:'position',desc:' Specifies the type of positioning method used for an element (static, relative, absolute or fixed)',forStyle:true,cat:PROP_POS,editor:EDIT_ENUM,values:['','static','relative','absolute','fixed','inherit']},{name:'clear',desc:' Specifies which sides of an element where other floating elements are not allowed',forStyle:true,cat:PROP_POS,editor:EDIT_ENUM,values:['','left','right','both','none','inherit']},{name:'right',desc:' Specifies the right position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'left',desc:' Specifies the left position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'top',desc:' Specifies the top position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_BORDER,canbepixel:true,editor:EDIT_TEXT},{name:'bottom',desc:' Specifies the bottom position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ',forStyle:true,cat:PROP_POS,canbepixel:true,editor:EDIT_TEXT},{name:'zIndex',desc:' Sets the stack order of a positioned element',forStyle:true,cat:PROP_POS,editor:EDIT_NUM},{name:'tabindex',desc:'specifies the tab order of an element (when the "tab" button is used for navigating).',editor:EDIT_NUM},{name:'cssFloat',forStyle:true,editor:EDIT_ENUM,values:HtmlEditor.cssFloat,desc:'sets or returns the horizontal alignment of an object.'},{name:'opacity',editor:EDIT_NUM,getter:function(o){return _editorOptions.client.getOpacity.apply(_editorOptions.elementEditedWindow,[o]);},setter:function(o,val){_editorOptions.client.setOpacity.apply(_editorOptions.elementEditedWindow,[o,val]);},desc:'sets the opacity level for an element, in percentage of 0 to 100.'}];var textProps=[{name:'color',desc:' Sets the color of text',forStyle:true,cat:PROP_TEXT,editor:EDIT_COLOR},langProp,{name:'direction',desc:' Specifies the text direction/writing direction',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','ltr','rtr','inherit']},{name:'letterSpacing',desc:' Increases or decreases the space between characters in a text. It can be normal, inherit, or in length such as 2px',forStyle:true,cat:PROP_TEXT,canbepixel:true,editor:EDIT_TEXT},{name:'lineHeight',desc:' Sets the line height. It can be normal or inherit, or a number that will be multiplied with the current font size to set the line height, or a fixed line height in px, pt, cm, etc., i.e. 1px, or a line height in percent of the current font size, i.e. 20%',forStyle:true,cat:PROP_TEXT,canbepixel:true,editor:EDIT_TEXT},{name:'textAlign',desc:' Specifies the horizontal alignment of text',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','left','right','center','justify','inherit']},{name:'textDecoration',desc:' Specifies the decoration added to text',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','none','underline','overline','line-through','blink','inherit']},{name:'textIndent',desc:' Specifies the indentation of the first line in a text-block. It can be a length defining a fixed indentation in px, pt, cm, em, etc., i.e. 5px, or be the indentation in % of the width of the parent element',forStyle:true,cat:PROP_TEXT,canbepixel:true,editor:EDIT_TEXT},{name:'textTransform',desc:' Controls the capitalization of text',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','none','capitalize','uppercase','lowercase','inherit']},{name:'verticalAlign',desc:' Sets the vertical alignment of an element. It can be a length which raises or lowers an element by the specified length, i.e. 2px, negative values are allowed;  it can be in a percent of the "line-height" property, i.e. 10%, which raises or lowers an element, negative values are allowed; ',forStyle:true,cat:PROP_TEXT,canbepixel:true,editor:EDIT_ENUM,allowEdit:true,values:['','baseline','sub','super','top','text-top','middle','bottom','text-bottom','inherit']},{name:'whiteSpace',desc:' Specifies how white-space inside an element is handled ',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','normal','nowrap','pre','pre-line','pre-wrap','inherit']},{name:'wordSpacing',desc:' Increases or decreases the space between words in a tex. It can be normal or inherit, or in a length defining an extra space between words in px, pt, cm, em, etc., i.e. 3px. Negative values are allowed',forStyle:true,cat:PROP_TEXT,canbepixel:true,editor:EDIT_TEXT},{name:'textAlignLast',desc:' Describes how the last line of a block or a line right before a forced line break is aligned when text-align is "justify"',forStyle:true,cat:PROP_TEXT,editor:EDIT_TEXT},{name:'textJustify',desc:' Specifies the justification method used when text-align is "justify"',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','auto','inter-word','inter-ideograph','inter-cluster','distribute','kashida','none']},{name:'textOverflow',desc:' Specifies what should happen when text overflows the containing element. It can be clip to clip the text; or ellipsis to show ..., or specify a string to be displayed',forStyle:true,cat:PROP_TEXT,editor:EDIT_TEXT},{name:'textShadow',desc:' Adds shadow to text. IE does not support it. It is specified as h v blur (optional) color (optional). h:The position of the horizontal shadow. Negative values are allowed. v:The position of the vertical shadow. Negative values are allowed. example:2px 2px #ff0000',forStyle:true,cat:PROP_TEXT,editor:EDIT_TEXT},{name:'wordBreak',desc:' Specifies line breaking rules for non-CJK scripts. Not supported by Opera.',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','normal','break-all','hyphenate']},{name:'wordWrap',desc:' Allows long, unbreakable words to be broken and wrap to the next line ',forStyle:true,cat:PROP_TEXT,editor:EDIT_ENUM,values:['','normal','break-word']}];var svgmleft={name:'moveleft',editor:EDIT_CMD,IMG:'/libjs/moveleft.png',action:moveshapeleft,toolTips:'moves the shape to left. A text box on the right shows moving dustance for each mouse click.'};var svgmright={name:'moveright',editor:EDIT_CMD,IMG:'/libjs/moveright.png',action:moveshaperight,toolTips:'moves the shape to right. A text box on the right shows moving dustance for each mouse click.'};var svgmup={name:'moveup',editor:EDIT_CMD,IMG:'/libjs/moveup.png',action:moveshapeup,toolTips:'moves the shape up. A text box on the right shows moving dustance for each mouse click.'};var svgmdown={name:'movedown',editor:EDIT_CMD,IMG:'/libjs/movedown.png',action:moveshapedown,toolTips:'moves the shape down. A text box on the right shows moving dustance for each mouse click.'};var svgmdist={name:'movedistance',editor:EDIT_CMD,isText:true,getter:function(o){return HtmlEditor.svgshapemovegap;},setter:function(o,v){if(v){var n=parseInt(v);if(n>0&&n<300){HtmlEditor.svgshapemovegap=n;}}}};var svgdup={name:'duplicate',editor:EDIT_CMD,IMG:'/libjs/copy.png',toolTips:'duplicate this element',action:function(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){_duplicateSvg(c.owner);}}};var svgshapeProps=[svgmleft,svgmright,svgmup,svgmdown,svgmdist,{name:'bringToFront',editor:EDIT_CMD,IMG:'/libjs/bfront.png',toolTips:'bring this element to front',action:function(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){_bringSvgToFront(c.owner);}}},svgdup,{name:'stroke',editor:EDIT_COLOR,byAttribute:true},{name:'stroke-width',editor:EDIT_NUM,byAttribute:true,nottext:true},{name:'mouseoveropacity',editor:EDIT_NUM,byAttribute:true,desc:'Specify opacity to be used when mouse moves over the shape, in percent from 0 to 100.',getter:function(o){return getPercentAttr(o,'mouseoveropacity');},setter:function(o,v){setPercentAttr(o,'mouseoveropacity',v);}},{name:'mouseoverfillcolor',desc:'Specify fill color to be used when mouse moves over the shape',editor:EDIT_COLOR,byAttribute:true,closedonly:true},{name:'mouseoutopacity',editor:EDIT_NUM,byAttribute:true,desc:'Specify opacity to be used when mouse moves oou of the shape, in percent from 0 to 100',getter:function(o){return getPercentAttr(o,'mouseoutopacity');},setter:function(o,v){setPercentAttr(o,'mouseoutopacity',v);}},{name:'mouseoutfillcolor',desc:'Specify fill color to be used when mouse moves out of the shape',editor:EDIT_COLOR,byAttribute:true,closedonly:true},{name:'showelements',editor:EDIT_ENUM,allowEdit:true,byAttribute:true,desc:'Specify an id list, separated by comma. Aach id represents one element, on mouseover event the element is displayed, on mouseout event the element is hidden.',values:function(){var ret=new Array();ret.push({text:'',value:''});var ids=getAllIds();for(var i=0;i<ids.length;i++){ret.push({text:ids[i],value:ids[i]});}
return ret;},combosetter:function(o,v,inputElement){if(v){v=v.trim();if(v.length>0){var s;var a=o.getAttribute('showelements');if(a&&a.length>0){var ss=a.split(',');for(var i=0;i<ss.length;i++){if(ss[i]){if(v==ss[i].trim()){return;}}}
var s='';for(var i=0;i<ss.length;i++){if(ss[i]){var s0=ss[i].trim();if(s0.length>0){if(s.length>0){s+=',';}
s+=s0;}}}
s+=',';s+=v;}
else{s=v;}
inputElement.value=s;o.setAttribute('showelements',s);}}}},{name:'automouseouteffects',editor:EDIT_BOOL,desc:'set this property to true so that mouseoutfillcolor and mouseoutopacity will be used on event of mouse out, and elements listed in showelements will be made invisible. If this attribute is not set or it is false then these effects will be used at a mouse click.',byAttribute:true},{name:'href',editor:EDIT_TEXT,desc:'hyper destination for the shape',getter:function(o){var a=o.parentNode;if(a){var s=a.getAttribute('href');if(s&&s.baseVal)
return s.baseVal;return s;}},setter:function(o,v){var a=o.parentNode;if(a){a.setAttribute('href',v);}}},{name:'target',editor:EDIT_ENUM,values:['','_blank','_self','_top','_parent'],desc:'It specifies where to open the linked document. The linked document is specified by href.',getter:function(o){var a=o.parentNode;if(a){return a.getAttribute('target');}},setter:function(o,v){var a=o.parentNode;if(a){a.setAttribute('target',v);}}},{name:'fill',forStyle:true,editor:EDIT_COLOR,closedonly:true},{name:'initshape',editor:EDIT_CMD,isInit:true,act:function(o){o.jsData=o.jsData||createSvgShape(o);}}];function getfileinput(form){for(var i=0;i<form.children.length;i++){if(form.children[i].tagName.toLowerCase()=='input'){var f=form.children[i].type;if(f&&f.toLowerCase()=='file'){return form.children[i];}}
else{var d=getfileinput(form.children[i]);if(d)
return d;}}}
function getfilemaxsizeinput(form){for(var i=0;i<form.children.length;i++){if(form.children[i].tagName.toLowerCase()=='input'&&form.children[i].getAttribute('name')=='MAX_FILE_SIZE'){return form.children[i];}
else{var d=getfilemaxsizeinput(form.children[i]);if(d)return d;}}}
function getDefaultEditors(){return[{tagname:'html',nodelete:true,properties:[{name:'head',editor:EDIT_GO,cmdText:'show page head',action:gotoPageHead},{name:'PageURL',desc:'Web address for the page as it is hosted on the current web server. You may copy it and give it to your web site visitors.',editor:EDIT_NONE,getter:function(o){return getWebAddress(o);}},{name:'docType',editor:EDIT_ENUM,values:['<!DOCTYPE html>','<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">','<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">','<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">','<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN" "http://www.w3.org/TR/html4/frameset.dtd">','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">'],allowEdit:true,IsDocType:true,desc:'In HTML 4.01, the DOCTYPE declaration refers to a DTD, because HTML 4.01 was based on SGML. The DTD specifies the rules for the markup language, so that the browsers render the content correctly. HTML5 is not based on SGML, and therefore does not require a reference to a DTD; you may simply use "html" as the DOCTYPE for HTML5. Always add the DOCTYPE declaration to your HTML documents, so that the browser knows what type of document to expect.',setter:function(o,val){_editorOptions.docType=val;}},{name:'title',desc:'A title is required in all HTML documents and it defines the title of the document. Some usages of the HTML document title: defines a title in the browser toolbar; provides a title for the page when it is added to favorites; displays a title for the page in search-engine results; etc.',editor:EDIT_TEXT,getter:function(o){return _editorOptions.elementEditedDoc.title;},setter:function(o,val){_editorOptions.elementEditedDoc.title=val;}},{name:'author',desc:'Specifies the name of the author of the document.',editor:EDIT_TEXT,getter:function(o){return getMetaData(o,'author');},setter:function(o,val){setMetaData(o,'author',val);}},{name:'description',desc:'Specifies a description of the page. Search engines can pick up this description to show with the results of searches.',editor:EDIT_TEXT,getter:function(o){return getMetaData(o,'description');},setter:function(o,val){setMetaData(o,'description',val);}},{name:'keywords',desc:'Specifies a comma-separated list of keywords - relevant to the page (Informs search engines what the page is about). Always specify keywords (needed by search engines to catalogize the page).',editor:EDIT_TEXT,getter:function(o){return getMetaData(o,'keywords');},setter:function(o,val){setMetaData(o,'keywords',val);}},langProp,{name:'lastSave',desc:'Time of the last saving or backup of the page to web server.',editor:EDIT_NONE,getter:function(o){if(_editorOptions)return _editorOptions.lastsavetime?_editorOptions.lastsavetime:(_editorOptions.useSavedUrl?'re-loaded':'not saved');}}]},{tagname:'head',nodelete:true,properties:[{name:'addmeta',editor:EDIT_CMD,IMG:'/libjs/addmeta.png',action:addmeta,toolTips:'add a new meta element to the page head'},{name:'addscript',editor:EDIT_CMD,IMG:'/libjs/addscript.png',action:addscript,toolTips:'add a new script element to the page head'},{name:'addcss',editor:EDIT_CMD,IMG:'/libjs/addcss.png',action:addcss,toolTips:'add a new CSS file to the page head'},{name:'addlink',editor:EDIT_CMD,IMG:'/libjs/addlink.png',action:addlink,toolTips:'add a new link to the page head'},{name:'meta',desc:'Metadata is data (information) about data. The "meta" tag provides metadata about the HTML document. Metadata will not be displayed on the page, but will be machine parsable. Meta elements are typically used to specify page description, keywords, author of the document, last modified, and other metadata. The metadata can be used by browsers (how to display content or reload page), search engines (keywords), or other web services. You may specify common meta data, author, description and keywords, by selecting HTML object.',editor:EDIT_NODES,sig:function(o){var ct=o.getAttribute('content');var nm=o.getAttribute('name');if(nm){return nm+':'+(ct?ct:'');}else{var hr=o.getAttribute('http-equiv');return(hr?hr:'?')+':'+(ct?ct:'')}}},{name:'script',desc:'The "script" tag is used to define a client-side script, such as a JavaScript. The "script" element either contains scripting statements, or it points to an external script file through the src attribute. Common uses for JavaScript are image manipulation, form validation, and dynamic changes of content.',editor:EDIT_NODES,sig:'src',isScript:true},{name:'css',desc:'Link CSS files to the HTML document so that the elements in the HTML may re-use the styles defined in the CSS files.',editor:EDIT_NODES,sig:'href'},{name:'link',desc:'The "link" tag defines the relationship between a document and an external resource. The "link" tag is mostly used to link to style sheets. So, most likely you do not need to use "link"; you may use "css" instead.',editor:EDIT_NODES,sig:function(o){var f=o.getAttribute('href');var rel=o.getAttribute('rel');if(rel&&rel=='stylesheet'){return'css file:'+f;}else{return'external file:'+f;}}},{name:'page classes',editor:EDIT_PROPS,editorList:function(o){var props=[];var cl=_editorOptions.client.getPagesClasses.apply(_editorOptions.elementEditedWindow);for(var i=0;i<cl.length;i++){props.push({name:cl[i].selector,editor:EDIT_NONE,getter:function(v){return function(){return v;};}(cl[i].cssText),action:function(e){var c=JsonDataBinding.getSender(e);if(c&&c.propDesc){if(confirm('Do you want to remove the styles identified by '+c.propDesc.name+'? ')){_editorOptions.client.removePageStyleBySelector.apply(_editorOptions.elementEditedWindow,[c.propDesc.name]);if(c.ownerTd){var tr=c.ownerTd.parentNode;if(tr){tr.style.display='none';}}
_editorOptions.pageChanged=true;}}},notModify:true,IMG:'/libjs/cancel.png',toolTips:'Each style was created when you edit element properties. Click the cancel button to delete the corresponding styles if you are sure the styles are no longer needed, i.e. all elements using the styles are removed.'});}
return props;},cat:PROP_PAGECLASSES}]},{tagname:'link',nodelete:true,properties:[{name:'delete',editor:EDIT_DEL,desc:'delete the file link'},{name:'href',editor:EDIT_TEXT,byAttribute:true,isFilePath:true,maxSize:1024,desc:'the external file to be lined',title:'Select External File',filetypes:'.href'},{name:'rel',editor:EDIT_ENUM,values:editor_rel,byAttribute:true},{name:'type',editor:EDIT_TEXT,byAttribute:true},{name:'hreflang',editor:EDIT_TEXT,byAttribute:true},{name:'rev',editor:EDIT_ENUM,values:editor_rel,byAttribute:true},{name:'target',editor:EDIT_ENUM,values:['','_blank','_self','_top','_parent'],byAttribute:true},{name:'media',editor:EDIT_ENUM,values:['','all','braille','print','projection','screen','speech'],byAttribute:true},{name:'charset',editor:EDIT_TEXT,byAttribute:true}]},{tagname:'meta',nodelete:true,properties:[{name:'delete',editor:EDIT_DEL},{name:'charset',byAttribute:true,desc:'Specifies the character encoding for the HTML document. Common values:UTF-8 - Character encoding for Unicode;ISO-8859-1 - Character encoding for the Latin alphabet. In theory, any character encoding can be used, but no browser understands all of them. The more widely a character encoding is used, the better the chance that a browser will understand it. To view all available character encodings, look at <a href="http://www.iana.org/assignments/character-sets" target=_new>IANA character sets</a>.',editor:EDIT_TEXT},{name:'content',byAttribute:true,desc:'Gives the value associated with the http-equiv or name attribute.',editor:EDIT_TEXT},{name:'name',byAttribute:true,desc:'Specifies a name for the metadata',editor:EDIT_ENUM,values:['','application-name','revised'],allowEdit:true},{name:'http-equiv',byAttribute:true,desc:'Provides an HTTP header for the information/value of the content attribute.',editor:EDIT_ENUM,values:['','content-script-type','default-style','refresh','expires','set-cookie'],allowEdit:true}]},{tagname:'script',nodelete:true,properties:[{name:'delete',editor:EDIT_DEL},{name:'type',byAttribute:true,editor:EDIT_ENUM,values:['','text/javascript','text/ecmascript','application/ecmascript','application/javascript','text/vbscript'],allowEdit:true},{name:'src',byAttribute:true,editor:EDIT_TEXT,isFilePath:true,maxSize:10240,title:'Select Script File',filetypes:'js'},{name:'charset',byAttribute:true,editor:EDIT_ENUM,values:['','ISO-8859-1','UTF-8'],allowEdit:true},{name:'defer',byAttribute:true,editor:EDIT_ENUM,values:['','defer']},{name:'async',byAttribute:true,editor:EDIT_ENUM,values:['','async']}]},{tagname:'body',nodelete:true,nomoveout:true,noCust:true,properties:[cssClass,{name:'cursor',forStyle:true,editor:EDIT_ENUM,allowEdit:true,values:HtmlEditor.cursorValues,desc:'specifies the type of cursor to be displayed when pointing on an element.'},{name:'maps',editor:EDIT_CHILD,childTag:'map',desc:'maps contained in the page'},{name:'drawings',editor:EDIT_CHILD,childTag:'svg',desc:'svg elements contained in the page'},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT,desc:'font related properties'},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'columns',editor:EDIT_PROPS,editorList:multiColsProps,cat:PROP_MULCOL}]},{tagname:'table',nounformat:true,nodelete:false,properties:[{name:'thead',editor:EDIT_GO,IMG:'/libjs/tableHead.png',toolTips:'show table head properties',notcreate:true,showCommand:function(o){var hs=o.getElementsByTagName('thead');return(hs&&hs.length>0);}},{name:'tfoot',editor:EDIT_GO,IMG:'/libjs/tableFoot.png',toolTips:'show table foot properties',notcreate:true,showCommand:function(o){var hs=o.getElementsByTagName('tfoot');return(hs&&hs.length>0);}},{name:'addHeader',editor:EDIT_CMD,IMG:'/libjs/addthead.png',toolTips:'add table head',action:addtheader,showCommand:function(o){var hs=o.getElementsByTagName('thead');return(!hs||hs.length==0);}},{name:'addFooter',editor:EDIT_CMD,IMG:'/libjs/addtfoot.png',toolTips:'add table foot',action:addtfooter,showCommand:function(o){var hs=o.getElementsByTagName('tfoot');return(!hs||hs.length==0);}},{name:'border',editor:EDIT_NUM,desc:'The value "1" indicates borders should be displayed, and that the table is NOT being used for layout purposes.'},{name:'cellpadding',editor:EDIT_NUM,byAttribute:true,desc:'specifies the space, in pixels, between the cell wall and the cell content.'},{name:'cellspacing',editor:EDIT_NUM,byAttribute:true,desc:'specifies the space, in pixels, between cells.'},{name:'rowCount',editor:EDIT_NONE,getter:function(o){return o.rows?o.rows.length:0;}},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'border style',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:createTable},{tagname:'col',nodelete:true,nomoveout:true,nounformat:true,noCust:true,properties:[{name:'cellIndex',editor:EDIT_NONE,getter:function(o){return o.cellIndex;}},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}]},{tagname:'td',nodelete:true,nomoveout:true,nounformat:true,noCust:true,properties:[{name:'colAttrs',editor:EDIT_CMD,IMG:'/libjs/html_col.png',toolTips:'modify column attributes',action:showcolumnattrs},{name:'newRowAbove',editor:EDIT_CMD,IMG:'/libjs/newAbove.png',toolTips:'add a new row above this row',action:addRowAbove},{name:'newRowBelow',editor:EDIT_CMD,IMG:'/libjs/newBelow.png',toolTips:'add a new row below this row',action:addRowBelow},{name:'newColumnLeft',editor:EDIT_CMD,IMG:'/libjs/newLeft.png',toolTips:'add a new column on left of this column',action:addColumnLeft},{name:'newColumnRight',editor:EDIT_CMD,IMG:'/libjs/newRight.png',toolTips:'add a new column on right of this column',action:addColumnRight},{name:'mergeColumnLeft',editor:EDIT_CMD,IMG:'/libjs/mergeLeft.png',toolTips:'merge this cell with the cell on its left side',action:mergeColumnLeft},{name:'mergeColumnRight',editor:EDIT_CMD,IMG:'/libjs/mergeRight.png',toolTips:'merge this cell with the cell on its right side',action:mergeColumnRight},{name:'mergeColumnAbove',editor:EDIT_CMD,IMG:'/libjs/mergeAbove.png',toolTips:'merge this cell with the cell above it',action:mergeColumnAbove},{name:'mergeColumnBelow',editor:EDIT_CMD,IMG:'/libjs/mergeBelow.png',toolTips:'merge this cell with the cell below it',action:mergeColumnBelow},{name:'splitColumnH',editor:EDIT_CMD,IMG:'/libjs/splitH.png',toolTips:'split this cell by creating a new cell on right side of this cell',action:splitColumnH},{name:'splitColumnV',editor:EDIT_CMD,IMG:'/libjs/splitV.png',toolTips:'split this cell by creating a new cell below this cell',action:splitColumnV},{name:'cellIndex',editor:EDIT_NONE,desc:'Column index. 0: the first column; 1: the second column; and so on'},{name:'rowIndex',editor:EDIT_NONE,desc:'Row index. 0: the first row; 1: the second row; and so on',getter:function(o){return o.parentNode.rowIndex;}},{name:'colSpan',editor:EDIT_NONE,desc:'defines the number of columns a cell should span'},{name:'rowSpan',editor:EDIT_NONE,desc:'defines the number of rows a cell should span'},{name:'width',editor:EDIT_TEXT,canbepixel:true,desc:'defines the width of the current cell. This setting will not be shared. To remove this setting, set it to empty.',getter:function(o){return o.style.width;},setter:function(o,v){if(typeof v=='undefined'||v==null||v.length==0){JsonDataBinding.removeStyleAttribute(o,'width');}
else{try{o.style.width=v;}
catch(err){}}}},{name:'height',editor:EDIT_TEXT,canbepixel:true,desc:'defines the height of the current cell. This setting will not be shared. To remove this setting, set it to empty.',getter:function(o){return o.style.height;},setter:function(o,v){if(typeof v=='undefined'||v==null||v.length==0){JsonDataBinding.removeStyleAttribute(o,'height');}
else{try{o.style.height=v;}
catch(err){}}}},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'tr',nodelete:true,nomoveout:true,nounformat:true,noCust:true,properties:[{name:'delRow',editor:EDIT_DEL,toolTips:'remove this row',action:removeTableRow},{name:'mergeRowAbove',editor:EDIT_CMD,IMG:'/libjs/mergeAbove.png',toolTips:'merge this row with the row above it',action:mergeRowAbove},{name:'mergeRowBelow',editor:EDIT_CMD,IMG:'/libjs/mergeBelow.png',toolTips:'merge this row with the row below it',action:mergeRowBelow},{name:'newRowAbove',editor:EDIT_CMD,IMG:'/libjs/newAbove.png',toolTips:'add a new row above this row',action:addRowAbove},{name:'newRowBelow',editor:EDIT_CMD,IMG:'/libjs/newBelow.png',toolTips:'add a new row below this row',action:addRowBelow},{name:'rowIndex',editor:EDIT_NONE},{name:'cellCount',editor:EDIT_NONE,getter:function(o){return o.cells?o.cells.length:0;}},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'tbody',nodelete:true,nomoveout:true,nounformat:true,noCust:true,properties:[{name:'rowCount',editor:EDIT_NONE,getter:function(o){return o.rows?o.rows.length:0;}},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}]},{tagname:'th',nodelete:true,nomoveout:true,nounformat:true,noCust:true,properties:[{name:'newRowAbove',editor:EDIT_CMD,IMG:'/libjs/newAbove.png',toolTips:'add a new row above this row',action:addRowAbove},{name:'newRowBelow',editor:EDIT_CMD,IMG:'/libjs/newBelow.png',toolTips:'add a new row below this row',action:addRowBelow},{name:'newColumnLeft',editor:EDIT_CMD,IMG:'/libjs/newLeft.png',toolTips:'add a new column on left of this column',action:addColumnLeft},{name:'newColumnRight',editor:EDIT_CMD,IMG:'/libjs/newRight.png',toolTips:'add a new column on right of this column',action:addColumnRight},{name:'mergeColumnLeft',editor:EDIT_CMD,IMG:'/libjs/mergeLeft.png',toolTips:'merge this cell with the cell on its left side',action:mergeColumnLeft},{name:'mergeColumnRight',editor:EDIT_CMD,IMG:'/libjs/mergeRight.png',toolTips:'merge this cell with the cell on its right side',action:mergeColumnRight},{name:'mergeColumnAbove',editor:EDIT_CMD,IMG:'/libjs/mergeAbove.png',toolTips:'merge this cell with the cell above it',action:mergeColumnAbove},{name:'mergeColumnBelow',editor:EDIT_CMD,IMG:'/libjs/mergeBelow.png',toolTips:'merge this cell with the cell below it',action:mergeColumnBelow},{name:'splitColumnH',editor:EDIT_CMD,IMG:'/libjs/splitH.png',toolTips:'split this cell by creating a new cell on right side of this cell',action:splitColumnH},{name:'splitColumnV',editor:EDIT_CMD,IMG:'/libjs/splitV.png',toolTips:'split this cell by creating a new cell below this cell',action:splitColumnV},{name:'cellIndex',editor:EDIT_NONE,desc:'column index'},{name:'rowIndex',editor:EDIT_NONE,getter:function(o){return o.parentNode.rowIndex;},desc:'row index'},{name:'colSpan',editor:EDIT_NUM,desc:'number of columns a header cell should span'},{name:'rowSpan',editor:EDIT_NUM,desc:'number of rows a header cell should span'},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'thead',nodelete:true,nomoveout:true,nounformat:true,noCust:true,properties:[{name:'delete',editor:EDIT_DEL},{name:'rowCount',editor:EDIT_NONE,getter:function(o){return o.rows?o.rows.length:0;}},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'tfoot',nodelete:true,nomoveout:true,nounformat:true,properties:[{name:'delete',editor:EDIT_DEL},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'p',properties:[{name:'columns',editor:EDIT_PROPS,editorList:multiColsProps,cat:PROP_MULCOL},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('p');obj.innerHTML='new paragraph';return obj;}},{tagname:'font',properties:[{name:'font',editor:EDIT_PROPS,editorList:editor_font}]},{tagname:'b',properties:[]},{tagname:'strong',properties:[]},{tagname:'em',properties:[]},{tagname:'i',properties:[]},{tagname:'u',properties:[]},{tagname:'strike',properties:[]},{tagname:'sub',properties:[]},{tagname:'sup',properties:[]},{tagname:'br',properties:[]},{tagname:'div',properties:[{name:'columns',editor:EDIT_PROPS,editorList:multiColsProps,cat:PROP_MULCOL},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('div');obj.innerHTML='new div';return obj;}},{tagname:'a',properties:[{name:'href',editor:EDIT_TEXT,desc:'the URL of the page the link goes to',isFilePath:true,title:'Target Web Page',filetypes:'.href',disableUpload:true,getter:function(o){return o.savedhref?o.savedhref:'';},setter:function(o,val){o.savedhref=val;if(val&&val.length>0){o.removeAttribute('youtubeID');_updateYoutubeTarget(o);}}},{name:'hreflang',editor:EDIT_TEXT,desc:'the language of the linked document'},{name:'name',editor:EDIT_TEXT,desc:'Not supported in HTML5. Specifies the name of an anchor'},{name:'rel',editor:EDIT_TEXT,desc:'relationship between the current document and the linked document'},{name:'rev',editor:EDIT_TEXT,desc:'Not supported in HTML5. the relationship between the linked document and the current document'},{name:'target',bodyOnly:true,editor:EDIT_ENUM,values:['','_blank','_parent','_self','_top'],allowEdit:true,desc:'where to open the linked document',getter:function(o){return o.savedtarget?o.savedtarget:'';},setter:function(o,val){o.savedtarget=val;},values:function(){var ret=new Array();ret.push({text:'',value:''});ret.push({text:'_blank',value:'_blank'});ret.push({text:'_parent',value:'_parent'});ret.push({text:'_self',value:'_self'});ret.push({text:'_top',value:'_top'});var ifs=_editorOptions.elementEditedDoc.getElementsByTagName('iframe');if(ifs&&ifs.length>0){for(var i=0;i<ifs.length;i++){var nm=ifs[i].name;if(typeof nm!='undefined'){nm=nm.trim();if(nm.length>0){ret.push({text:nm,value:nm});}}}}
return ret;}},{name:'shape',editor:EDIT_ENUM,values:['','default','rect','circle','poly'],desc:'Not supported in HTML5. the shape of a link'},{name:'media',editor:EDIT_TEXT,desc:'HTML 5. what media/device the linked document is optimized for'},{name:'type',editor:EDIT_TEXT,desc:'HTML 5. MIME type of the linked document'},{name:'youtubePlayer',editor:EDIT_ENUM,desc:'youtube player on the web page to be used to play video. youtubeID specifies the video to be played',values:function(o){var ret=[{text:'',value:''}];var ifs=_editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow,['iframe']);if(ifs){for(var i=0;i<ifs.length;i++){if(ifs[i].typename=='youtube'||ifs[i].getAttribute('typename')=='youtube'){if(!ifs[i].id||ifs[i].id==''){ifs[i].id=_createId(ifs[i]);}
ret.push({text:ifs[i].id,value:ifs[i].id});}}}
return ret;},getter:function(o){return o.getAttribute('youtube');},setter:function(o,v){if(!v||v.length==0){o.removeAttribute('youtube');}
else{o.setAttribute('youtube',v);}
_updateYoutubeTarget(o);}},{name:'youtubeID',editor:EDIT_TEXT,desc:'ID of a youtube video to be played',getter:function(o){return o.getAttribute('youtubeID');},setter:function(o,v){if(!v||v==''){o.removeAttribute('youtubeID');}
else{o.setAttribute('youtubeID',getyoutubeid(v));var yid=o.getAttribute('youtube');if(!yid||yid==''){var ifs=_editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow,['iframe']);if(ifs){for(var i=0;i<ifs.length;i++){if(ifs[i].typename=='youtube'||ifs[i].getAttribute('typename')=='youtube'){if(!ifs[i].id||ifs[i].id==''){ifs[i].id=_createId(ifs[i]);}
o.setAttribute('youtube',ifs[i].id);break;}}}}}
_updateYoutubeTarget(o);}},{name:'mediaPlayer',editor:EDIT_ENUM,desc:'a flash or music player on the web page to be used to play flash video or music. "mediaFile" property specifies the flash or music to be played',values:function(o){var ret=[{text:'',value:''}];var ifs=_editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow,['embed']);if(ifs){var s=o.getAttribute('mediaFile');var t=3;if(s&&s.length>0){if(JsonDataBinding.endsWithI(s,'.swf')){t=1;}
else{t=2;}}
for(var i=0;i<ifs.length;i++){var icl=false;if(t==1){if(ifs[i].typename=='flash'||ifs[i].getAttribute('typename')=='flash'){icl=true;}}
else if(t==2){if(ifs[i].typename=='music'||ifs[i].getAttribute('typename')=='music'){icl=true;}}
else{if(ifs[i].typename=='music'||ifs[i].getAttribute('typename')=='music'||ifs[i].typename=='flash'||ifs[i].getAttribute('typename')=='flash'){icl=true;}}
if(icl){if(!ifs[i].id||ifs[i].id==''){ifs[i].id=_createId(ifs[i]);}
ret.push({text:ifs[i].id,value:ifs[i].id});}}}
return ret;},getter:function(o){return o.getAttribute('mediaPlayer');},setter:function(o,v){if(!v||v.length==0){o.removeAttribute('mediaPlayer');}
else{o.setAttribute('mediaPlayer',v);}
_updateMediaTarget(o);}},{name:'mediaFile',editor:EDIT_TEXT,desc:'it can be an URL for a flash file (*.swf) or a music file (*.mp3) to be played. mediaPlayer property indicates which media player on the web page should be used.',getter:function(o){return o.getAttribute('mediaFile');},setter:function(o,v){if(!v||v==''){o.removeAttribute('mediaFile');}
else{o.setAttribute('mediaFile',v);var yid=o.getAttribute('mediaPlayer');if(!yid||yid==''){var t=3;if(JsonDataBinding.endsWithI(v,'.swf')){t=1;}
else{t=2;}
var ifs=_editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow,['embed']);if(ifs){for(var i=0;i<ifs.length;i++){if(t==1&&ifs[i].typename=='flash'||ifs[i].getAttribute('typename')=='flash'){if(!ifs[i].id||ifs[i].id==''){ifs[i].id=_createId(ifs[i]);}
o.setAttribute('mediaPlayer',ifs[i].id);break;}
else if(t==1&&ifs[i].typename=='music'||ifs[i].getAttribute('typename')=='music'){if(!ifs[i].id||ifs[i].id==''){ifs[i].id=_createId(ifs[i]);}
o.setAttribute('mediaPlayer',ifs[i].id);break;}}}}}
_updateMediaTarget(o);}},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('a');obj.innerHTML='new hyperlink';if(!_editorOptions.isEditingBody)obj.target='_blank';return obj;}},{tagname:'span',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('span');obj.innerHTML='new formatted text';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'readonly',editor:EDIT_ENUM,values:['','readonly'],byAttribute:true},{name:'maxlength',editor:EDIT_NUM},{name:'size',editor:EDIT_NUM},{name:'width',editor:EDIT_TEXT,forStyle:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'text',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='text';obj.value='new input';return obj;}},{tagname:'button',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'width',editor:EDIT_TEXT,forStyle:true},{name:'type',editor:EDIT_ENUM,values:['','button','reset','submit']},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('button');obj.innerHTML='new button';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'checked',editor:EDIT_ENUM,values:['','checked'],byAttribute:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'checkbox',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='checkbox';obj.value='new check box';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'checked',editor:EDIT_ENUM,values:['','checked'],byAttribute:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'radio',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='radio';obj.value='new radio button';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'readonly',editor:EDIT_ENUM,values:['','readonly'],byAttribute:true},{name:'maxlength',editor:EDIT_NUM},{name:'size',editor:EDIT_NUM},{name:'width',editor:EDIT_TEXT,forStyle:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'password',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='password';obj.value='password';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'readonly',editor:EDIT_ENUM,values:['','readonly'],byAttribute:true},{name:'maxlength',editor:EDIT_NUM},{name:'size',editor:EDIT_NUM},{name:'width',editor:EDIT_TEXT,forStyle:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'file',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='file';obj.value='new file upload';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'width',editor:EDIT_TEXT,forStyle:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'reset',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='reset';obj.value='reset button';return obj;}},{tagname:'input',properties:[{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,desc:'display text',byAttribute:true},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'Specifies that an input element should be disabled',byAttribute:true},{name:'width',editor:EDIT_TEXT,forStyle:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],type:'submit',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='submit';obj.value='submit button';return obj;}},{tagname:'input',properties:[idProp,{name:'delete',desc:'delete the element',editor:EDIT_DEL},{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,desc:'value to be sent to server',byAttribute:true}],type:'hidden',creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('input');obj.type='hidden';obj.value='';return obj;}},{tagname:'img',properties:[{name:'name',editor:EDIT_TEXT},{name:'src',editor:EDIT_TEXT,isFilePath:true,maxSize:1048576,desc:'the URL of an image',title:'Select Image File',filetypes:'.image'},{name:'alt',editor:EDIT_TEXT,desc:'an alternate text for an image',byAttribute:true},{name:'longdesc',editor:EDIT_TEXT,desc:' Not supported in HTML5.  the URL to a document that contains a long description of an image '},{name:'height',editor:EDIT_TEXT,desc:'height of an image',byAttribute:true},{name:'width',editor:EDIT_TEXT,desc:'width of an image',byAttribute:true},{name:'ismap',editor:EDIT_ENUM,values:['','ismap'],desc:'an image as a server-side image-map'},{name:'usemap',editor:EDIT_TEXT,desc:'an image as a client-side image-map'},{name:'tabindex',editor:EDIT_NUM},verticalAlignProp,{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'border style',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('img');obj.src='/libjs/html_img.png';obj.removeAttribute('width');obj.removeAttribute('height');return obj;}},{tagname:'ul',properties:[validateCmdProp,{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('ul');var li=_editorOptions.elementEditedDoc.createElement('li');li.innerHTML='item 1';obj.appendChild(li);return obj;}},{tagname:'menu',desc:'The menu element is <font color=red>deprecated</font> in HTML 4.01 and redefined in HTML5. The menu tag is used to create a list of menu choices.',properties:[{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('menu');var li=_editorOptions.elementEditedDoc.createElement('li');li.innerHTML='item 1';obj.appendChild(li);return obj;}},{tagname:'ol',properties:[{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('ol');var li=_editorOptions.elementEditedDoc.createElement('li');li.innerHTML='item 1';obj.appendChild(li);return obj;}},{tagname:'li',properties:[{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}]},{tagname:'dl',properties:[{name:'newDefinition',editor:EDIT_CMD,IMG:'/libjs/newDefinition.png',action:addDefinition},{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('dl');var dt=_editorOptions.elementEditedDoc.createElement('dt');dt.innerHTML='item 1';obj.appendChild(dt);var dd=_editorOptions.elementEditedDoc.createElement('dd');dd.innerHTML='description of item 1';obj.appendChild(dd);return obj;}},{tagname:'dt',properties:[{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}]},{tagname:'dd',properties:[{name:'name',editor:EDIT_TEXT},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}]},{tagname:'textarea',properties:[{name:'autofocus',editor:EDIT_ENUM,values:['','autofocus'],desc:'HTML5. the element should automatically get focus when the page loads'},{name:'form',editor:EDIT_TEXT,desc:'HTML5. one or more forms the element belongs to'},{name:'name',editor:EDIT_TEXT},{name:'value',editor:EDIT_TEXT,desc:'text inside the area'},{name:'disabled',editor:EDIT_ENUM,values:['','disabled']},{name:'readonly',editor:EDIT_ENUM,values:['','readonly']},{name:'cols',editor:EDIT_NUM,desc:'the visible width of a text area'},{name:'rows',editor:EDIT_NUM,desc:'the visible number of lines in a text area'},{name:'size',editor:EDIT_NUM},{name:'maxlength',editor:EDIT_NUM,desc:'HTML 5. the maximum number of characters allowed in the text area'},{name:'placeholder',editor:EDIT_TEXT,desc:'HTML 5. a short hint that describes the expected value of a text area'},{name:'required',editor:EDIT_ENUM,values:['','required'],desc:'HTML 5.  a text area is required/must be filled out'},{name:'wrap',editor:EDIT_ENUM,values:['','hard','soft'],desc:'HTML 5.  how the text in a text area is to be wrapped when submitted in a form'},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'select',properties:[{name:'newOptionGroup',editor:EDIT_CMD,IMG:'/libjs/newOptionGroup.png',action:addOptionGroup},{name:'newOption',editor:EDIT_CMD,IMG:'/libjs/newDefinition.png',action:addOption},{name:'options',editor:EDIT_CMD,createCommand:showListItems},{name:'groups',editor:EDIT_CHILD,childTag:'optgroup'},{name:'name',editor:EDIT_TEXT},{name:'form',editor:EDIT_TEXT,desc:'HTML5. one or more forms the select field belongs to'},{name:'size',editor:EDIT_NUM,desc:'the number of visible options in a drop-down list'},{name:'multiple',editor:EDIT_BOOL,desc:'multiple options can be selected at once'},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'a drop-down list should be disabled'},{name:'autofocus',editor:EDIT_ENUM,values:['','autofocus'],desc:'HTML5. the drop-down list should automatically get focus when the page loads'},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('select');if(_isFireFox)obj.contentEditable=false;return obj;}},{tagname:'option',properties:[{name:'newOption',editor:EDIT_CMD,IMG:'/libjs/newDefinition.png',action:addOption},{name:'options',editor:EDIT_CMD,createCommand:showListItems},{name:'name',editor:EDIT_TEXT},{name:'text',editor:EDIT_TEXT,desc:'display text'},{name:'value',editor:EDIT_TEXT,desc:'the value to be sent to a server'},{name:'label',editor:EDIT_TEXT,desc:'a shorter label for an option'},{name:'disabled',editor:EDIT_ENUM,values:['','disabled'],desc:'an option should be disabled'},{name:'selected',editor:EDIT_ENUM,values:['','selected'],desc:'an option should be pre-selected when the page loads'},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}]},{tagname:'hr',properties:[{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},]},{tagname:'h1',properties:[{name:'h2',editor:EDIT_CMD,IMG:'/libjs/html_h2.png',action:function(e){changeHeading(e,'h2');}},{name:'h3',editor:EDIT_CMD,IMG:'/libjs/html_h3.png',action:function(e){changeHeading(e,'h3');}},{name:'h4',editor:EDIT_CMD,IMG:'/libjs/html_h4.png',action:function(e){changeHeading(e,'h4');}},{name:'h5',editor:EDIT_CMD,IMG:'/libjs/html_h5.png',action:function(e){changeHeading(e,'h5');}},{name:'h6',editor:EDIT_CMD,IMG:'/libjs/html_h6.png',action:function(e){changeHeading(e,'h6');}},{name:'heading',editor:EDIT_PROPS,editorList:editor_heading}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('h1');obj.innerHTML='new heading';return obj;}},{tagname:'h2',properties:[{name:'h1',editor:EDIT_CMD,IMG:'/libjs/html_h1.png',action:function(e){changeHeading(e,'h1');}},{name:'h3',editor:EDIT_CMD,IMG:'/libjs/html_h3.png',action:function(e){changeHeading(e,'h3');}},{name:'h4',editor:EDIT_CMD,IMG:'/libjs/html_h4.png',action:function(e){changeHeading(e,'h4');}},{name:'h5',editor:EDIT_CMD,IMG:'/libjs/html_h5.png',action:function(e){changeHeading(e,'h5');}},{name:'h6',editor:EDIT_CMD,IMG:'/libjs/html_h6.png',action:function(e){changeHeading(e,'h6');}},{name:'heading',editor:EDIT_PROPS,editorList:editor_heading}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('h2');obj.innerHTML='new heading';return obj;}},{tagname:'h3',properties:[{name:'h1',editor:EDIT_CMD,IMG:'/libjs/html_h1.png',action:function(e){changeHeading(e,'h1');}},{name:'h2',editor:EDIT_CMD,IMG:'/libjs/html_h2.png',action:function(e){changeHeading(e,'h2');}},{name:'h4',editor:EDIT_CMD,IMG:'/libjs/html_h4.png',action:function(e){changeHeading(e,'h4');}},{name:'h5',editor:EDIT_CMD,IMG:'/libjs/html_h5.png',action:function(e){changeHeading(e,'h5');}},{name:'h6',editor:EDIT_CMD,IMG:'/libjs/html_h6.png',action:function(e){changeHeading(e,'h6');}},{name:'heading',editor:EDIT_PROPS,editorList:editor_heading}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('h3');obj.innerHTML='new heading';return obj;}},{tagname:'h4',properties:[{name:'h1',editor:EDIT_CMD,IMG:'/libjs/html_h1.png',action:function(e){changeHeading(e,'h1');}},{name:'h2',editor:EDIT_CMD,IMG:'/libjs/html_h2.png',action:function(e){changeHeading(e,'h2');}},{name:'h3',editor:EDIT_CMD,IMG:'/libjs/html_h3.png',action:function(e){changeHeading(e,'h3');}},{name:'h5',editor:EDIT_CMD,IMG:'/libjs/html_h5.png',action:function(e){changeHeading(e,'h5');}},{name:'h6',editor:EDIT_CMD,IMG:'/libjs/html_h6.png',action:function(e){changeHeading(e,'h6');}},{name:'heading',editor:EDIT_PROPS,editorList:editor_heading}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('h4');obj.innerHTML='new heading';return obj;}},{tagname:'h5',properties:[{name:'h1',editor:EDIT_CMD,IMG:'/libjs/html_h1.png',action:function(e){changeHeading(e,'h1');}},{name:'h2',editor:EDIT_CMD,IMG:'/libjs/html_h2.png',action:function(e){changeHeading(e,'h2');}},{name:'h3',editor:EDIT_CMD,IMG:'/libjs/html_h3.png',action:function(e){changeHeading(e,'h3');}},{name:'h4',editor:EDIT_CMD,IMG:'/libjs/html_h4.png',action:function(e){changeHeading(e,'h4');}},{name:'h6',editor:EDIT_CMD,IMG:'/libjs/html_h6.png',action:function(e){changeHeading(e,'h6');}},{name:'heading',editor:EDIT_PROPS,editorList:editor_heading}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('h5');obj.innerHTML='new heading';return obj;}},{tagname:'h6',properties:[{name:'h1',editor:EDIT_CMD,IMG:'/libjs/html_h1.png',action:function(e){changeHeading(e,'h1');}},{name:'h2',editor:EDIT_CMD,IMG:'/libjs/html_h2.png',action:function(e){changeHeading(e,'h2');}},{name:'h3',editor:EDIT_CMD,IMG:'/libjs/html_h3.png',action:function(e){changeHeading(e,'h3');}},{name:'h4',editor:EDIT_CMD,IMG:'/libjs/html_h4.png',action:function(e){changeHeading(e,'h4');}},{name:'h5',editor:EDIT_CMD,IMG:'/libjs/html_h5.png',action:function(e){changeHeading(e,'h5');}},{name:'heading',editor:EDIT_PROPS,editorList:editor_heading}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('h6');obj.innerHTML='new heading';return obj;}},{tagname:'label',properties:[{name:'htmlFor',editor:EDIT_ENUM,allowEdit:true,desc:'Specifies which element a label is bound to',values:function(){var ret=new Array();ret.push({text:'',value:''});function getIdArray(e){if(e.childNodes&&e.childNodes.length){for(var i=0;i<e.childNodes.length;i++){if(e.childNodes[i].id&&e.childNodes[i].id.length>0){ret.push({text:elementToString(e.childNodes[i]),value:e.childNodes[i].id});}
getIdArray(e.childNodes[i]);}}}
getIdArray(_editorOptions.elementEditedDoc.body);return ret;}},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'border style',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('label');obj.innerHTML='new label';return obj;}},{tagname:'address',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('address');obj.innerHTML='address';return obj;}},{tagname:'map',properties:[{name:'addArea',editor:EDIT_CMD,IMG:'/libjs/addarea.png',action:addMapArea},{name:'name',editor:EDIT_TEXT,desc:'Required. Specifies the name of an image-map'},{name:'areas',editor:EDIT_CHILD,childTag:'area'},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}]},{tagname:'area',properties:[{name:'alt',editor:EDIT_TEXT,desc:'Specifies an alternate text for the area. Required if the href attribute is present'},{name:'showmarkers',editor:EDIT_CMD,isInit:true,act:function(o){o.jsData=o.jsData||function(){var left=0,top=0,right=0,bottom=0;var x=0,y=0,r=0;var points=[];var coords;var shape;function parseCoords(){coords=o.getAttribute('coords');shape=o.getAttribute('shape');if(coords){var nums=coords.split(',');if(shape=='rest'){if(nums.length>0){left=nums[0];if(nums.length>1){top=nums[1];if(nums.length>2){right=nums[2];if(nums.length>3){bottom=nums[3];}}}}}
else if(shape=='circle'){if(nums.length>0){x=nums[0];if(nums.length>1){y=nums[1];if(nums.length>2){r=nums[2];}}}}
else if(shape=='poly'){points=[];var i=0;while(i<nums.length){var pt={};pt.x=parseInt(nums[i]);i++;if(i<nums.length){pt.y=parseInt(nums[i]);i++;}
else
pt.y=0;pt.idx=points.push(pt)-1;}}}}
function _showmarkers(){if(_editorOptions.markers){for(var k=0;k<_editorOptions.markers.length;k++){if(_editorOptions.markers[k]){var p=_editorOptions.markers[k].parentNode;if(p){p.removeChild(_editorOptions.markers[k]);}}}
delete _editorOptions.markers;}
coords=o.getAttribute('coords');shape=o.getAttribute('shape');if(shape=='rest'){}
else if(shape=='circle'){}
else if(shape=='poly'){if(coords){var svg=_createSVGElement('svg');_appendChild(_editorOptions.elementEditedDoc.body,svg);svg.style.position='absolute';svg.style.top='0px';svg.style.left='0px';svg.style.opacity='0.2';var polygon=_createSVGElement('polygon');svg.appendChild(polygon);polygon.setAttribute('points',coords);polygon.style.fill='yellow';_editorOptions.markers=[];_editorOptions.markers.push(svg);}}}
function _getProperties(){parseCoords();var props=[];if(shape=='rest'){function setCoordsRect(){coords=left+','+top+','+right+','+bottom;o.setAttribute('coords',coords);if(o.jsData.showmarkers)
o.jsData.showmarkers();}
props.push({name:'left',editor:EDIT_NUM,getter:function(e){return left;},setter:function(e,val){left=val;setCoordsRect();}});props.push({name:'top',editor:EDIT_NUM,getter:function(e){return top;},setter:function(e,val){top=val;setCoordsRect();}});props.push({name:'right',editor:EDIT_NUM,getter:function(e){return right;},setter:function(e,val){right=val;setCoordsRect();}});props.push({name:'bottom',editor:EDIT_NUM,getter:function(e){return bottom;},setter:function(e,val){bottom=val;setCoordsRect();}});}
else if(shape=='circle'){function setCoordsCircle(){coords=x+','+y+','+r;o.setAttribute('coords',coords);if(o.jsData.showmarkers)
o.jsData.showmarkers();}
props.push({name:'x',editor:EDIT_NUM,getter:function(e){return x;},setter:function(e,val){x=val;setCoordsCircle();}});props.push({name:'y',editor:EDIT_NUM,getter:function(e){return y;},setter:function(e,val){y=val;setCoordsCircle();}});props.push({name:'radius',editor:EDIT_NUM,getter:function(e){return r;},setter:function(e,val){r=val;setCoordsCircle();}});}
else if(shape=='poly'){function getterPoint(idx,obj){return function(owner){if(idx>=0&&idx<points.length){return points[idx].x+','+points[idx].y;}}}
function setterPoint(idx,obj){return function(owner,val){var ss;if(val&&val.length>0){ss=val.split(',');if(ss.length>0){var p;if(idx>=0&&idx<points.length){p=points[idx];}
else{p={};p.idx=idx;points.push(p);}
p.x=parseInt(ss[0]);if(ss.length>1){p.y=parseInt(ss[1]);}}}
else{if(idx>=0&&idx<points.length){p=points[idx];p.deleted=true;}}
ss='';var k=0;for(var k=0;k<points.length;k++){if(!points[k].deleted){if(ss.length>0){ss+=',';}
ss+=(points[k].x+','+points[k].y);}}
o.setAttribute('coords',ss);if(o.jsData.showmarkers)
o.jsData.showmarkers();}}
function createPointProp(idx,needGrow){var p={name:'point '+idx,editor:EDIT_TEXT,cat:PROP_CUST1,desc:'a vertex point of a poly shape, in format of x,y',getter:getterPoint(idx,o),setter:setterPoint(idx,o)};if(needGrow){p.needGrow=true;p.onsetprop=function(o,v){_showmarkers();if(this.grow){if(v&&v.length>0){this.grow(createPointProp(idx+1,true),1);this.grow=null;}}};}
return p;}
if(points){for(var i=0;i<points.length;i++){props.push(createPointProp(i,false));}
props.push(createPointProp(points.length,true));}
else{props.push(createPointProp(0,true));}}
function onsetprop(curObj,v){_showmarkers();}
function onclientmousedown(e){if(_custMouseDownOwner&&_custMouseDownOwner.ownertextbox){e=e||window.event;if(e){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);_custMouseDownOwner.ownertextbox.value=x+','+y;}}}
function oneditprop(e){var img=JsonDataBinding.getSender(e);if(img){if(_custMouseDown&&_custMouseDownOwner!=img){if(_custMouseDownOwner){_custMouseDownOwner.src='libjs/propedit.png';img.active=false;}}
img.active=!img.active;if(img.active){img.src='libjs/propeditAct.png';_custMouseDown=onclientmousedown;_custMouseDownOwner=img;}
else{img.src='libjs/propedit.png';_custMouseDown=null;_custMouseDownOwner=null;}}}
for(var i=0;i<props.length;i++){if(!props[i].onsetprop){props[i].onsetprop=onsetprop;}
props[i].propEditor=oneditprop;}
return props;}
parseCoords();return{showmarkers:function(){_showmarkers();},getProperties:function(){return _getProperties();}};}();o.jsData.showmarkers();}},{name:'coords',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the coordinates of the area',cat:PROP_CUST1,editorList:function(o){if(o.jsData)
return o.jsData.getProperties();}},{name:'href',editor:EDIT_TEXT,desc:'Specifies the hyperlink target for the area'},{name:'nohref',editor:EDIT_ENUM,values:['','nohref'],desc:'Not supported in HTML5. Specifies that an area has no associated link'},{name:'shape',editor:EDIT_ENUM,values:['','default','rect','circle','poly'],desc:'Specifies the shape of the area'},{name:'target',editor:EDIT_ENUM,values:['','_blank','_parent','_self','_top'],allowEdit:true,desc:'Specifies where to open the target URL'},{name:'type',editor:EDIT_TEXT,desc:'MIME type of the target URL'},{name:'rel',editor:EDIT_ENUM,values:['','alternate','author','bookmark','help','license','next','nofollow','noreferrer','prefetch','prev','search','tag'],desc:'Specifies the relationship between the current document and the target URL'},{name:'media',editor:EDIT_TEXT,desc:'Specifies what media/device the target URL is optimized for'},{name:'hreflang',editor:EDIT_TEXT,desc:'Specifies the language of the target URL'},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('area');obj.shape='rect';return obj;}},{tagname:'bdo',properties:[{name:'dir',editor:EDIT_ENUM,values:['','ltr','rtl'],desc:'Specifies the text direction of the text inside the bdo element'},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('bdo');obj.innerHTML='new bdo';return obj;}},{tagname:'blockquote',properties:[{name:'cite',editor:EDIT_TEXT,desc:'Specifies the source of the quotation',byAttribute:true},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('blockquote');obj.innerHTML='new blockquote';return obj;}},{tagname:'cite',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('cite');obj.innerHTML='new citation';return obj;}},{tagname:'code',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('code');obj.innerHTML='/*code sample*/ var v=2;';return obj;}},{tagname:'fieldset',properties:[{name:'name',editor:EDIT_TEXT,desc:'Specifies the name of the fieldset'},{name:'disabled',editor:EDIT_BOOL,desc:'Specifies that a group of related form elements should be disabled'},{name:'addLegend',editor:EDIT_CMD,IMG:'/libjs/newlegend.png',action:addLegend},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('fieldset');var lg=_editorOptions.elementEditedDoc.createElement('legend');obj.appendChild(lg);lg.innerHTML='<b>new group</b>';return obj;}},{tagname:'legend',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('legend ');obj.innerHTML='<b>group</b>';return obj;}},{tagname:'form',properties:[{name:'name',editor:EDIT_TEXT,desc:'Specifies the name of a form'},{name:'action',editor:EDIT_TEXT,desc:'Specifies where to send the form-data when a form is submitted'},{name:'method',editor:EDIT_ENUM,values:['get','post'],desc:'Specifies the HTTP method to use when sending form-data'},{name:'enctype',editor:EDIT_ENUM,values:['','ication/x-www-form-urlencoded','multipart/form-data'],desc:'Specifies how the form-data should be encoded when submitting it to the server (only for method="post")'},{name:'accept-charset',editor:EDIT_TEXT,desc:'Specifies the character encodings that are to be used for the form submission'},{name:'accept',editor:EDIT_TEXT,desc:'Not supported in HTML5. Specifies the types of files that the server accepts (that can be submitted through a file upload)'},{name:'dox',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'margings and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('form');var sb=_editorOptions.elementEditedDoc.createElement('input');sb.type='submit';obj.appendChild(sb);return obj;}},{tagname:'iframe',properties:[{name:'name',editor:EDIT_TEXT,desc:'Specifies the name of an iframe element'},{name:'src',editor:EDIT_TEXT,desc:'Specifies the address of the document to embed in the iframe element',getter:function(o){if(_editorOptions.forIDE){var s=o.getAttribute('srcDesign');if(s)
return s;}
return o.src;},setter:function(o,v){if(_editorOptions.forIDE){o.setAttribute('srcDesign',v);o.setAttribute('src','/libjs/iframeDesign.jpg');}
else
o.src=v;},onsetprop:function(o,v){var furl=getIframFullUrl(o);refreshPropertyDisplay('fullUrl',furl);}},{name:'fullUrl',editor:EDIT_NONE,desc:'Specifies the address of the document to be used for web visiting.',getter:function(o){return getIframFullUrl(o);}},frameWidthProp,frameHeightProp,{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'margings and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('iframe');obj.width='615';obj.height='380';obj.style.pointerEvents='none';return obj;}},{tagname:'kbd',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('kbd');obj.innerHTML='keyboard input';return obj;}},{tagname:'dfn',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('dfn');obj.innerHTML='a definition term';return obj;}},{tagname:'samp',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('samp');obj.innerHTML='sample output';return obj;}},{tagname:'pre',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('pre');obj.innerHTML='preformatted text';return obj;}},{tagname:'var',properties:[{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('var');obj.innerHTML='variable name';return obj;}},{tagname:'embed',properties:[{name:'src',editor:EDIT_TEXT,desc:'the address of the external file to embed',isFilePath:true,filetypes:'.href',disableUpload:true,title:'Select file to embed'},{name:'height',editor:EDIT_NUM,desc:'the height of the embedded content'},{name:'width',editor:EDIT_NUM,desc:'the width of the embedded content'},{name:'type',editor:EDIT_TEXT,desc:'the MIME type of the embedded content'}]},{tagname:'object',properties:[{name:'addParam',editor:EDIT_CMD,IMG:'/libjs/addparam.png',action:addObjectParameter},{name:'parameters',editor:EDIT_CHILD,childTag:'param'},{name:'name',editor:EDIT_TEXT},{name:'archive',editor:EDIT_TEXT,desc:'Not supported in HTML5. A space separated list of URL\'s to archives. The archives contains resources relevant to the object'},{name:'classid',editor:EDIT_TEXT,desc:'Not supported in HTML5. Defines a class ID value as set in the Windows Registry or a URL'},{name:'codebase',editor:EDIT_TEXT,desc:'Not supported in HTML5. Defines where to find the code for the object'},{name:'codetype',editor:EDIT_TEXT,desc:'Not supported in HTML5. The internet media type of the code referred to by the classid attribute'},{name:'data',editor:EDIT_TEXT,desc:'URL of the resource to be used by the object'},{name:'declare',editor:EDIT_ENUM,values:['','declare'],desc:'Not supported in HTML5. Defines that the object should only be declared, not created or instantiated until needed'},{name:'standby',editor:EDIT_TEXT,desc:'Not supported in HTML5. Defines a text to display while the object is loading'},{name:'type',editor:EDIT_TEXT,desc:'MIME type of data specified in the data attribute'},{name:'usemap',editor:EDIT_TEXT,desc:'name of a client-side image map to be used with the object'},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'margins and paddings',editor:EDIT_PROPS,editorList:marginPaddingProps,cat:PROP_MARGIN},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('object');obj.width='615';obj.height='380';return obj;}},{tagname:'param',properties:[{name:'name',editor:EDIT_TEXT,desc:'Specifies the name of a parameter'},{name:'value',editor:EDIT_TEXT,desc:'Specifies the value of the parameter'},{name:'valuetype',editor:EDIT_ENUM,values:['','data','ref','object'],desc:'Not supported in HTML5. Specifies the type of the value'},{name:'type',editor:EDIT_TEXT,desc:'Not supported in HTML5. MIME type of the parameter'}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('param');var nm='p'+Math.floor(Math.random()*65536);obj.name=nm;return obj;}},{tagname:'optgroup',properties:[{name:'newOption',editor:EDIT_CMD,IMG:'/libjs/newDefinition.png',action:addOption},{name:'items',editor:EDIT_CHILD,childTag:'option'},{name:'label',editor:EDIT_TEXT,desc:'Specifies a label for an option-group'},{name:'formatting',editor:EDIT_PROPS,editorList:textProps,cat:PROP_TEXT},{name:'background',editor:EDIT_PROPS,editorList:backgroundProps,cat:PROP_BK},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}],creator:function(){var obj=_editorOptions.elementEditedDoc.createElement('optgroup');obj.innerHTML='new group';return obj;}},{tagname:'video',properties:[{name:'autoplay',editor:EDIT_BOOL,getter:function(o){return getSingleValueAttr(o,'autoplay');},setter:function(o,v){setSingleValueAttr(o,'autoplay',v);},desc:'Specifies that the video will start playing as soon as it is ready'},{name:'controls',editor:EDIT_BOOL,getter:function(o){return getSingleValueAttr(o,'controls');},setter:function(o,v){setSingleValueAttr(o,'controls',v);},desc:'Specifies that video controls should be displayed (such as a play/pause button etc).'},{name:'loop',editor:EDIT_BOOL,getter:function(o){return getSingleValueAttr(o,'loop');},setter:function(o,v){setSingleValueAttr(o,'loop',v);},desc:'Specifies that the video will start over again, every time it is finished'},{name:'muted',editor:EDIT_BOOL,getter:function(o){return getSingleValueAttr(o,'muted');},setter:function(o,v){setSingleValueAttr(o,'muted',v);},desc:'Specifies that the audio output of the video should be muted'},{name:'height',editor:EDIT_NUM,isPixel:true,desc:'Sets the height of the video player'},{name:'width',editor:EDIT_NUM,isPixel:true,desc:'Sets the width of the video player'},{name:'poster',editor:EDIT_TEXT,isFilePath:true,maxSize:1048576,title:'Select Image File',filetypes:'.image',desc:'Specifies an image to be shown while the video is downloading, or until the user hits the play button'},{name:'preload',editor:EDIT_ENUM,values:['','auto','metadata','none'],desc:'Specifies if and how the author thinks the video should be loaded when the page loads'},{name:'src',editor:EDIT_TEXT,isFilePath:true,filetypes:'.mp4,.ogg,.webm',desc:'Specifies the URL of the video file'},{name:'alternativeSources',desc:'specifies alternative video URLs to support different web browsers',editor:EDIT_PROPS,cat:PROP_CUST1,editorList:function(o){var i=0;var props=[];var srcs=o.getElementsByTagName('source');function createSrcProp(idx,s,needGrow){var p={name:'source '+(1+idx),editor:EDIT_TEXT,cat:PROP_CUST1,desc:'alternative video source URL for supporting various web browsers',isFilePath:true,title:'Select video File',filetypes:'.mp4,.ogg,.webm',getter:function(o0){if(!s.isNew)return s.getAttribute('src');},setter:function(o0,val){if(val&&val.length>0){var pos=val.lastIndexOf('.');if(pos>0){if(s.isNew){s=_createElement('source');o.appendChild(s);}
s.setAttribute('src',val);s.setAttribute('type','video/'+val.substr(pos));}}
else{if(!s.isNew){o.removeChild(s);s={isNew:true};}}}};if(needGrow){p.needGrow=true;p.onsetprop=function(o0,v){if(this.grow){if(v&&v.length>0){this.grow(createSrcProp(idx+1,{isNew:true},true),1);this.grow=null;}}};}
return p;}
if(srcs!=null&&srcs.length>0){var j=0;for(i=0;i<srcs.length;i++){if(srcs[i].hasAttribute('src')){props.push(createSrcProp(j,srcs[i],false));j++;}
else{o.removeChild(srcs[i]);}}}
props.push(createSrcProp(i,{isNew:true},true));return props;}}]},{tagname:'youtube',properties:[{name:'youtubeID',editor:EDIT_TEXT,desc:'Specifies the video to be used. It is a YouTube video ID. If you copy your video URL from YouTube then it will try to figure out the YouTube video ID by looking up "v="',getter:function(o){var src;if(_editorOptions.forIDE)
src=o.getAttribute('srcDesign');else
src=o.src;if(src&&src.length>"http://www.youtube.com/embed/".length){return src.substr("http://www.youtube.com/embed/".length);}},setter:function(o,v){if(_editorOptions.forIDE){o.setAttribute('src','/libjs/youtube.jpg');o.setAttribute('srcDesign','http://www.youtube.com/embed/'+getyoutubeid(v));}
else
o.src='http://www.youtube.com/embed/'+getyoutubeid(v);}},frameWidthProp,frameHeightProp,{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],internalAttributes:['allowfullscreen','typename','type','frameborder','src']},{tagname:'menubar',nounformat:true,nocommonprops:true,noCust:true,nodelete:false,objSetter:function(o){o.jsData.editor={addItem:function(ul,ulsrc){_addItem(ul,ulsrc);}}
var sp=document.createElement('span');sp.innerHTML='menu styles';sp.style.cursor='pointer';_tdObj.appendChild(sp);JsonDataBinding.AttachEvent(sp,"onclick",function(){showProperties(o);});JsonDataBinding.AttachEvent(sp,"onmouseover",function(){sp.style.backgroundColor='#D3D3D3';});JsonDataBinding.AttachEvent(sp,"onmouseout",function(){sp.style.backgroundColor='';});var o2=o.cloneNode(true);o2.className='htmlEditor_menuEditor';o2.sourceNav=o;o2.style.overflow='auto';_tdObj.lastobj=o2;_tdObj.appendChild(o2);adjustSizes();function onmenuitemclick(li0){var li=li0;function isRoot(){return(li0.parentNode.parentNode.tagName.toLowerCase()=='nav');}
function getAnchor(){for(var i=0;i<li.children.length;i++){if(li.children[i].tagName.toLowerCase()=='a'){return li.children[i];}}}
function getAnchorSource(){for(var i=0;i<li.sourceLi.children.length;i++){if(li.sourceLi.children[i].tagName.toLowerCase()=='a'){return li.sourceLi.children[i];}}}
function getText(){var a=getAnchor();return a.innerHTML;}
function setText(val){var a=getAnchor();a.innerHTML=val;a=getAnchorSource();a.innerHTML=val;}
function getTargetUrl(){var a=getAnchorSource();return a.savedhref?a.savedhref:'';}
function setTargetUrl(val){var a=getAnchorSource();a.savedhref=val;}
function getTargetPlace(){var a=getAnchorSource();return a.savedtarget?a.savedtarget:'';}
function setTargetPlace(val){var a=getAnchorSource();a.savedtarget=val;}
function addSubItem(){var ul;for(var i=0;i<li.children.length;i++){if(li.children[i].tagName.toLowerCase()=='ul'){ul=li.children[i];break;}}
if(!ul){ul=document.createElement('ul');li.appendChild(ul);}
var ulsrc;for(var i=0;i<li.sourceLi.children.length;i++){if(li.sourceLi.children[i].tagName.toLowerCase()=='ul'){ulsrc=li.sourceLi.children[i];break;}}
if(!ulsrc){ulsrc=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['ul']);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[li.sourceLi,ulsrc]);}
ul.sourceUl=ulsrc;o.jsData.editor.addItem(ul,ulsrc);_editorOptions.client.executeJsDb.apply(_editorOptions.elementEditedWindow,['initMenubar',o]);}
function moveItemUp(){LI_moveup(li.sourceLi);LI_moveup(li,true);}
function moveItemDown(){LI_movedown(li.sourceLi);LI_movedown(li,true);}
return function(ev){showProperties({subEditor:true,obj:li,owner:o,getString:function(){return'{'+elementToString(this.owner)+'}.{'+getText()+'}';},getProperties:function(){var props=new Array();props.push({name:'delete',toolTips:'delete the menu item',editor:EDIT_DEL,action:function(){if(confirm('Do you want to remove this menu item '+getText()+' and all its sub items?')){var newOwner;var ul=li.parentNode;var ulsrc=li.sourceLi.parentNode;ul.removeChild(li);ulsrc.removeChild(li.sourceLi);newOwner=ulsrc;if(ul.children.length==0){if(ul.parentNode.tagName.toLowerCase()=='li'){var puLi=ul.parentNode;puLi.removeChild(ul);var ulsrc=ul.sourceUl;var plisrc=ulsrc.parentNode;plisrc.removeChild(ulsrc);newOwner=plisrc;}}
_editorOptions.client.executeJsDb.apply(_editorOptions.elementEditedWindow,['initMenubar',o]);selectEditElement(newOwner);if(typeof _editorOptions!='undefined')_editorOptions.pageChanged=true;}},notModify:true});props.push({name:'addSubItem',toolTips:'add a new sub menu item',editor:EDIT_CMD,IMG:'/libjs/newElement.png',action:addSubItem});if(isRoot()){props.push({name:'moveleft',toolTips:'move current menu item to left',editor:EDIT_CMD,IMG:'/libjs/moveleft.png',action:moveItemUp});props.push({name:'moveright',toolTips:'move current menu item to right',editor:EDIT_CMD,IMG:'/libjs/moveright.png',action:moveItemDown});}
else{props.push({name:'moveup',toolTips:'move current menu item up',editor:EDIT_CMD,IMG:'/libjs/moveup.png',action:moveItemUp});props.push({name:'movedown',toolTips:'move current menu item down',editor:EDIT_CMD,IMG:'/libjs/movedown.png',action:moveItemDown});}
props.push({name:'Text',desc:'menu item text',editor:EDIT_TEXT,getter:function(){return getText();},setter:function(o,val){setText(val);}});props.push({name:'TargetURL',isFilePath:true,title:'Target Web Page',filetypes:'.href',disableUpload:true,desc:'the URL of page the menu item goes to. ',editor:EDIT_TEXT,getter:function(){return getTargetUrl();},setter:function(o,val){setTargetUrl(val);}});props.push({name:'TargetPlace',desc:'specifies where to open the linked document',editor:EDIT_ENUM,values:['','_blank','_parent','_self','_top'],allowEdit:true,getter:function(){return getTargetPlace();},setter:function(o,val){setTargetPlace(val);}});return{props:props};}});if(ev&&ev.stopPropagation){ev.stopPropagation();}
else if(window.event){window.event.cancelBubble=true;}}}
function _addItem(ul,ulsrc){var li=document.createElement('li');var a=document.createElement('a');var lisrc=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['li']);var asrc=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['a']);a.innerHTML='Item';a.href='#';asrc.innerHTML='Item';ul.appendChild(li);li.appendChild(a);li.sourceLi=lisrc;_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[ulsrc,lisrc]);_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow,[lisrc,asrc]);JsonDataBinding.AttachEvent(li,"onclick",onmenuitemclick(li));}
function hookMenuEditor(ul,ulsrc){ul.sourceUl=ulsrc;for(var i=0;i<ul.children.length;i++){ul.children[i].sourceLi=ulsrc.children[i];for(var m=0;m<ul.children[i].children.length;m++){if(ul.children[i].children[m].tagName.toLowerCase()=='a'){ul.children[i].children[m].href='#';break;}}
JsonDataBinding.AttachEvent(ul.children[i],"onclick",onmenuitemclick(ul.children[i]));for(var k=0;k<ul.children[i].children.length;k++){var tag=ul.children[i].children[k].tagName?ul.children[i].children[k].tagName.toLowerCase():'';if(tag=='ul'){hookMenuEditor(ul.children[i].children[k],ulsrc.children[i].children[k]);}}}}
if(o2.children.length>0){hookMenuEditor(o2.children[0],o.children[0]);}},properties:[idProp,{name:'class',toolTips:'class names for the menubar',editor:EDIT_TEXT,getter:function(o){return o.jsData.getClassNames();},setter:function(o,val){o.jsData.setClassNames(val);}},{name:'newBarItem',toolTips:'add a new menu bar item',editor:EDIT_CMD,IMG:'/libjs/newElement.png',action:function(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var nav=c.owner;if(nav.jsData){var navObj=_tdObj.getElementsByTagName('nav');if(navObj&&navObj.length>0){navObj=navObj[0];var ul=navObj.children[0];var ulsrc=nav.children[0];nav.jsData.editor.addItem(ul,ulsrc);}}}}},{name:'menuStyleName',desc:'the name identifying menu styles',editor:EDIT_TEXT,getter:function(o){if(o.className&&JsonDataBinding.startsWith(o.className,'limnorstyles_menu')){return o.className.substr('limnorstyles_menu'.length);}},setter:function(o,val){o.jsData.setStylesName.apply(_editorOptions.elementEditedWindow,[val]);}},{name:'fontFamily',forStyle:true,editor:EDIT_ENUM,values:fontList,getter:function(o){return o.jsData.getFontFamily();},setter:function(o,val){o.jsData.setFontFamily(val);}},{name:'fontSize',forStyle:true,editor:EDIT_ENUM,values:fontSizes,allowEdit:true,getter:function(o){return o.jsData.getFontSize();},setter:function(o,val){o.jsData.setFontSize(val);},onselectindexchanged:function(sel){if(sel.selectedIndex){var sv=sel.options[sel.selectedIndex].value;if(sv){if(sv=='%'||sv=='size in px, cm, ...'){return false;}
return true;}}
return false;},ontextchange:function(txt,sel){if(txt&&sel.selectedIndex){var sv=sel.options[sel.selectedIndex].value;if(sv=='size in px, cm, ...')
return txt;if(sv=='%'&&txt.length>0)
if(txt.substr(txt.length-1,1)=='%')
return txt;else
return txt;}}},{name:'FullWidth',desc:'Whether the menu bar takes full width',editor:EDIT_BOOL,getter:function(o){return o.jsData.getFullWidth();},setter:function(o,val){o.jsData.setFullWidth(val);}},{name:'marginTop',desc:'Top margin, in pixels, of the menu bar',editor:EDIT_NUM,getter:function(o){return o.jsData.getMenubarMarginTop();},setter:function(o,val){o.jsData.setMenubarMarginTop(val);}},{name:'marginBottom',desc:'Bottom margin, in pixels, of the menu bar',editor:EDIT_NUM,getter:function(o){return o.jsData.getMenubarMarginBottom();},setter:function(o,val){o.jsData.setMenubarMarginBottom(val);}},{name:'menuTextColor',desc:'Color of menu bar texts',editor:EDIT_COLOR,getter:function(o){return o.jsData.getMenubarTextColor();},setter:function(o,val){o.jsData.setMenubarTextColor(val);}},{name:'menuHoverTextColor',desc:'Color of menu bar text when mouse pointer moves on text',editor:EDIT_COLOR,getter:function(o){return o.jsData.getMenubarHoverTextColor();},setter:function(o,val){o.jsData.setMenubarHoverTextColor(val);}},{name:'menuBarHorizontalPadding',desc:'Horizontal paddings between menu bar texts',editor:EDIT_NUM,getter:function(o){return o.jsData.getMenubarPaddingX();},setter:function(o,val){o.jsData.setMenubarPaddingX(val);}},{name:'menuBarVerialPadding',desc:'Vertical paddings between menu bar texts',editor:EDIT_NUM,getter:function(o){return o.jsData.getMenubarPaddingY();},setter:function(o,val){o.jsData.setMenubarPaddingY(val);}},{name:'menuBarRadius',desc:'Radius of menu bar corners',editor:EDIT_NUM,getter:function(o){return o.jsData.getMenubarRadius();},setter:function(o,val){o.jsData.setMenubarRadius(val);}},{name:'menuBarBackColor',desc:'Background color of menu bar',editor:EDIT_COLOR,getter:function(o){return o.jsData.getMenubarBkColor();},setter:function(o,val){o.jsData.setMenubarBkColor(val);}},{name:'menuBarGradientColor',desc:'Background gradient color of menu bar',editor:EDIT_COLOR,getter:function(o){return o.jsData.getMenubarGradientColor();},setter:function(o,val){o.jsData.setMenubarGradientColor(val);}},{name:'menuBarHoverBackColor',desc:'Background color of menu bar text  when mouse pointer moves on text',editor:EDIT_COLOR,getter:function(o){return o.jsData.getMenubarHoverBkColor();},setter:function(o,val){o.jsData.setMenubarHoverBkColor(val);}},{name:'menuBarHoverGradientColor',desc:'Background gradient color of menu bar text when mouse pointer moves on text',editor:EDIT_COLOR,getter:function(o){return o.jsData.getMenubarHoverGradientColor();},setter:function(o,val){o.jsData.setMenubarHoverGradientColor(val);}},{name:'dropdownHorizontalPadding',desc:'Horizontal paddings for drop down texts',editor:EDIT_NUM,getter:function(o){return o.jsData.getItemPaddingX();},setter:function(o,val){o.jsData.setItemPaddingX(val);}},{name:'dropdownVerticalPadding',desc:'Vertical paddings for drop down texts',editor:EDIT_NUM,getter:function(o){return o.jsData.getItemPaddingY();},setter:function(o,val){o.jsData.setItemPaddingY(val);}},{name:'dropdownRadius',desc:'Radius of drop down corners',editor:EDIT_NUM,getter:function(o){return o.jsData.getItemRadius();},setter:function(o,val){o.jsData.setItemRadius(val);}},{name:'dropdownBackColor',desc:'Background color of a drop down. a drop down appears when mouse pointer moves over its parent',editor:EDIT_COLOR,getter:function(o){return o.jsData.getDropdownBkColor();},setter:function(o,val){o.jsData.setDropdownBkColor(val);}},{name:'dropdownGradientColor',desc:'Background color of a drop down. a drop down appears when mouse pointer moves over its parent',editor:EDIT_COLOR,getter:function(o){return o.jsData.getDropdownGradientColor();},setter:function(o,val){o.jsData.setDropdownGradientColor(val);}},{name:'dropdownItemBackColor',desc:'Background color of a drop down item when mouse pointer moves over it',editor:EDIT_COLOR,getter:function(o){return o.jsData.getItemHoverBkColor();},setter:function(o,val){o.jsData.setItemHoverBkColor(val);}},{name:'dropdownItemGradientColor',desc:'Background gradient color of a drop down item when mouse pointer moves over it',editor:EDIT_COLOR,getter:function(o){return o.jsData.getItemHoverGradientColor();},setter:function(o,val){o.jsData.setItemHoverGradientColor(val);}}],creator:function(){var obj=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['nav']);obj.typename='menubar';obj.jsData=_editorOptions.client.createMenuStyles.apply(_editorOptions.elementEditedWindow,[obj]);obj.contentEditable=false;return obj;}},{tagname:'treeview',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,properties:[idProp,{name:'newRootItem',toolTips:'add a new root item',editor:EDIT_CMD,IMG:'/libjs/newElement.png',action:function(e){var c=JsonDataBinding.getSender(e);if(c&&c.owner){var ul=c.owner;if(ul&&ul.jsData){ul.jsData.addItem.apply(_editorOptions.elementEditedWindow);}}}},validateCmdProp,{name:'StyleName',desc:'Modifying of one treeView styles will be applied to all tree views with the same StyleName.',getter:function(o){return o.jsData.designer.getStyleName.apply(_editorOptions.elementEditedWindow);},setter:function(o,val){o.jsData.designer.setStyleName.apply(_editorOptions.elementEditedWindow,[val]);}},{name:'HoverBackColor',desc:'Background color of tree view item when mouse pointer moves on a tree view item. This property is for all tree views.',editor:EDIT_COLOR,getter:function(o){return o.jsData.designer.getHoverBkColor.apply(_editorOptions.elementEditedWindow);},setter:function(o,val){o.jsData.designer.setHoverBkColor.apply(_editorOptions.elementEditedWindow,[val]);}},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT}],creator:function(){var obj=_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['ul']);_editorOptions.client.onCreatedObject.apply(_editorOptions.elementEditedWindow,['HtmlEditorTreeview',obj,_editorOptions.client]);_initPageElement(obj);_editorOptions.client.executeClientFunc.apply(_editorOptions.elementEditedWindow,[obj.jsData.addItem]);return obj;}},{tagname:'slideshow',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,properties:[idProp,{name:'slides',editor:EDIT_PROPS,editorList:function(o){var i;var slides=o.jsData.getSlides();var props=[];function getterActImg(idx,obj){return function(owner){var sl=owner.jsData.getSlide(idx);if(sl){return sl.img;}}}
function setterActImg(idx,obj){return function(owner,val){var sl=owner.jsData.getSlide(idx);if(sl){sl.img=val;}}}
function getterActTxt(idx,obj){return function(owner){var sl=owner.jsData.getSlide(idx);if(sl){return sl.txt;}}}
function setterActTxt(idx,obj){return function(owner,val){var sl=owner.jsData.getSlide(idx);if(sl){sl.txt=val;}}}
function createImgProp(idx,needGrow){var p={name:'image '+idx,editor:EDIT_TEXT,cat:PROP_CUST1,desc:'slide image',isFilePath:true,maxSize:1048576,title:'Select Image File',filetypes:'.image',getter:getterActImg(idx,o),setter:setterActImg(idx,o)};if(needGrow){p.needGrow=true;p.onsetprop=function(o,v){if(this.grow){if(v&&v.length>0){this.grow(createImgProp(idx+1,true),2);this.grow(createTxtProp(idx+1),3);this.grow=null;}}
o.jsData.showSlide();};}
return p;}
function createTxtProp(idx){return{name:'text '+idx,editor:EDIT_TEXT,cat:PROP_CUST1,desc:'slide caption',getter:getterActTxt(idx,o),setter:setterActTxt(idx,o)};}
for(i=0;i<slides.length;i++){props.push(createImgProp(i,false));props.push(createTxtProp(i));}
props.push(createImgProp(i,true));props.push(createTxtProp(i));return props;},cat:PROP_CUST1},{name:'width',editor:EDIT_TEXT,canbepixel:true,desc:'Sets the width of the slide show. It can be a length value in pixels, i.e. 300px, or in percent of its container, i.e. 100%. If both width and height are in % then only % of height is ignored.',getter:function(o){return o.getAttribute('w');},setter:function(o,v){if(typeof(v)=='undefined'||v==null||v.length==0){o.removeAttribute('w');if(o.style.removeProperty){o.style.removeProperty('width');}
else{o.style.removeAttribute('width');}}
else{o.setAttribute('w',v);if(v.charAt(v.length-1)=='%'){}
else{o.style.width=v;}}
if(o.jsData){o.jsData.onsizechanged();}}},{name:'height',editor:EDIT_TEXT,canbepixel:true,desc:'Sets the height of the slide show. It can be a length value in pixels, i.e. 300px, or in percent of its container, i.e. 100%. If both width and height are in % then only % of height is ignored.',getter:function(o){return o.getAttribute('h');},setter:function(o,v){if(typeof(v)=='undefined'||v==null||v.length==0){o.removeAttribute('h');if(o.style.removeProperty){o.style.removeProperty('height');}
else{o.style.removeAttribute('height');}}
else{o.setAttribute('h',v);if(v.charAt(v.length-1)=='%'){}
else{o.style.height=v;}}
if(o.jsData){o.jsData.onsizechanged();}}},{name:'backgroundColor',editor:EDIT_COLOR,desc:'background color',getter:function(o){return o.style.backgroundColor;},setter:function(o,v){o.style.backgroundColor=v;}},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER},{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'music',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,properties:[idProp,{name:'src',editor:EDIT_TEXT,byAttribute:true,allowEdit:false,isFilePath:true,maxSize:1048576,desc:'the URL of a music',title:'Select music File',filetypes:'.mp3;.mid',setter:function(o,v){var p=o.parentNode;if(p){o.setAttribute('src',v);var e2=o.cloneNode(false);e2.setAttribute('src',v);p.insertBefore(e2,o);p.removeChild(o);e2.id=o.id;_editorOptions.selectedObject=null;selectEditElement(e2);}}},{name:'loop',editor:EDIT_BOOL,byAttribute:true},{name:'autostart',editor:EDIT_BOOL,byAttribute:true},{name:'volume',editor:EDIT_NUM,byAttribute:true,desc:'use 0 to 100 to specify music volume'},{name:'left',editor:EDIT_TEXT,canbepixel:true,getter:function(o){return o.style.left;},setter:function(o,v){if(typeof v=='undefined'||v==null||v.length==0){JsonDataBinding.removeStyleAttribute('left');}
else if(JsonDataBinding.isNumber(v)){o.style.left=v+'px';}
else{try{o.style.left=v;}
catch(e){}}}},{name:'top',editor:EDIT_TEXT,canbepixel:true,getter:function(o){return o.style.top;},setter:function(o,v){if(typeof v=='undefined'||v==null||v.length==0){JsonDataBinding.removeStyleAttribute('top');}
else if(JsonDataBinding.isNumber(v)){o.style.top=v+'px';}
else{try{o.style.top=v;}
catch(e){}}}},{name:'height',editor:EDIT_NUM,desc:'height of the player',byAttribute:true},{name:'width',editor:EDIT_NUM,desc:'width of the player',byAttribute:true},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var o=_createElement('embed');var src=prompt('Enter URL for music file, for example, a MP3 file. You may leave it blank and later set src property','');o.typename='music';o.setAttribute('typename','music');o.setAttribute('src',src);o.setAttribute('type',"audio/x-pn-realaudio-plugin");o.setAttribute('controls',"ControlPanel");o.setAttribute('loop',"true");o.setAttribute('autostart',"true");o.setAttribute('volume',"100");o.setAttribute('initfn',"load-types");o.setAttribute('mime-types',"mime.types");o.setAttribute('height',"30");o.setAttribute('width',"300");return o;}},{tagname:'flash',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,properties:[idProp,{name:'src',editor:EDIT_TEXT,byAttribute:true,allowEdit:false,isFilePath:true,maxSize:1048576,desc:'the URL of a flash',title:'Select Flash File',filetypes:'.swf',setter:function(o,v){var p=o.parentNode;if(p){o.setAttribute('src',v);var e2=o.cloneNode(false);e2.setAttribute('src',v);p.insertBefore(e2,o);p.removeChild(o);e2.id=o.id;_editorOptions.selectedObject=null;selectEditElement(e2);}}},{name:'left',editor:EDIT_TEXT,canbepixel:true,getter:function(o){return o.style.left;},setter:function(o,v){if(typeof v=='undefined'||v==null||v.length==0){JsonDataBinding.removeStyleAttribute('left');}
else if(JsonDataBinding.isNumber(v)){o.style.left=v+'px';}
else{try
{o.style.left=v;}
catch(e){}}}},{name:'top',editor:EDIT_TEXT,canbepixel:true,getter:function(o){return o.style.top;},setter:function(o,v){if(typeof v=='undefined'||v==null||v.length==0){JsonDataBinding.removeStyleAttribute('top');}
else if(JsonDataBinding.isNumber(v)){o.style.top=v+'px';}
else{try{o.style.top=v;}
catch(e){}}}},{name:'height',editor:EDIT_NUM,desc:'height of the player',byAttribute:true},{name:'width',editor:EDIT_NUM,desc:'width of the player',byAttribute:true},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}],creator:function(){var o=_createElement('embed');o.typename='flash';o.setAttribute('typename','flash');o.setAttribute('hidechild','true');var src=prompt('Enter SWF file path for your flash player. You may leave it blank and later set src property with your SWF file','');o.setAttribute('src',src);o.setAttribute('allowFullScreen','true');o.setAttribute('quality','high');o.setAttribute('width','680');o.setAttribute('height','460');o.setAttribute('align','middle');o.setAttribute('allowScriptAccess','always');o.setAttribute('type','application/x-shockwave-flash');o.setAttribute('PLUGINSPAGE','http://www.macromedia.com/go/getflashplayer');return o;}},{tagname:'hitcount',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,properties:[{name:'font',editor:EDIT_PROPS,editorList:editor_font,cat:PROP_FONT},{name:'size and location',editor:EDIT_PROPS,editorList:sizelocProps,cat:PROP_SIZELOC}]},{tagname:'fileupload',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,noSetInnerHtml:true,nodelete:false,properties:[{name:'id',desc:'a unique id for an element',editor:EDIT_TEXT,getter:function(o){var d=getfileinput(o);if(d){return d.id;}},setter:function(o,val){if(_divError){_divError.style.display='none';}
var finput=getfileinput(o);if(finput&&finput.id!=val){var g=finput.getAttribute('limnorId')
if(val==null||val.length==0){if(g){showError('id for this element cannot be removed because it is used in programming.');return;}}
if(val&&val.length>0){if(val=='body'){showError('"body" cannot be used as id.');return;}
if(HtmlEditor.IsNameValid(val)){var o0=document.getElementById(val);if(o0){showError('The id is in use.');return;}
else{o0=document.getElementById(val+'f');if(o0){showError('The id is in use. Please try another one');return;}}}
else{showError('The id is invalid. Please only use alphanumeric characters. The first letter can only be a-z, A-Z, $, and underscore');return;}}
o.id=val+'f';finput.id=val;finput.setAttribute('name',val);if(_editorOptions.forIDE){if(g){if(typeof(limnorStudio)!='undefined')limnorStudio.onElementIdChanged(g,val);else window.external.OnElementIdChanged(g,val);}}}}},{name:'width',editor:EDIT_TEXT,forStyle:true,getter:function(o){var d=getfileinput(o);if(d){return d.style.width;}},setter:function(o,v){if(_divError){_divError.style.display='none';}
var finput=getfileinput(o);if(finput){if(typeof v!='undefined'&&v!=null&&JsonDataBinding.isNumber(v)){v=v+'px';}
finput.style.width=v;}}},{name:'MaximumFileSize',editor:EDIT_NUM,desc:'Gets an integer indicating allowed maximum size, in KB, for file upload.',getter:function(o){var d=getfilemaxsizeinput(o);if(d)return d.value;return 0;},setter:function(o,v){var d=getfilemaxsizeinput(o);if(d)d.value=v;}}]},{tagname:'svg',nounformat:true,nomoveout:true,noCust:true,nodelete:false,properties:[svgmleft,svgmright,svgmup,svgmdown,svgmdist,svgdup,{name:'addpolygon',editor:EDIT_CMD,IMG:'/libjs/html_svgpoly.png',action:addpolygon,toolTips:'add a new polygon element to the drawing group'},{name:'addrect',editor:EDIT_CMD,IMG:'/libjs/html_svgrect.png',action:addrect,toolTips:'add a new rectangle element to the drawing group'},{name:'addcircle',editor:EDIT_CMD,IMG:'/libjs/html_svgcircle.png',action:addcircle,toolTips:'add a new circle element to the drawing group'},{name:'addellipse',editor:EDIT_CMD,IMG:'/libjs/html_svgellipse.png',action:addellipse,toolTips:'add a new ellipse element to the drawing group'},{name:'addline',editor:EDIT_CMD,IMG:'/libjs/html_svgline.png',action:addline,toolTips:'add a new line element to the drawing group'},{name:'addpolyline',editor:EDIT_CMD,IMG:'/libjs/html_svgpolyline.png',action:addpolyline,toolTips:'add a new polyline element to the drawing group'},{name:'addtext',editor:EDIT_CMD,IMG:'/libjs/html_svgtext.png',action:addtext,toolTips:'add a new text element to the drawing group'},{name:'shapes',editor:EDIT_CHILD,childTag:'polygon,rect,circle,ellipse,polyline,line,path,text',desc:'maps contained in the page'},{name:'position',editor:EDIT_PROPS,editorList:posProps,cat:PROP_POS},{name:'box',editor:EDIT_PROPS,editorList:boxDimProps,cat:PROP_BOX},{name:'border',editor:EDIT_PROPS,editorList:borderProps,cat:PROP_BORDER}]},{tagname:'polygon',nounformat:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,properties:[{name:'points',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the vertex points of the polygon',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgPoly(o);if(o.jsData)
return o.jsData.getProperties();}}]},{tagname:'rect',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,properties:[{name:'rectangle',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the rectangle parameters',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgRect(o);if(o.jsData)
return o.jsData.getProperties();}}]},{tagname:'circle',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,properties:[{name:'circle',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the circle parameters',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgCircle(o);if(o.jsData)
return o.jsData.getProperties();}}]},{tagname:'ellipse',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,properties:[{name:'ellipse',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the ellipse parameters',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgCircle(o);if(o.jsData)
return o.jsData.getProperties();}}]},{tagname:'line',nounformat:true,nocommonprops:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,notclose:true,properties:[{name:'line',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the line parameters',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgLine(o);if(o.jsData)
return o.jsData.getProperties();}}]},{tagname:'polyline',nounformat:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,notclose:true,properties:[{name:'points',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies the joint points of the polyline',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgPoly(o);if(o.jsData)
return o.jsData.getProperties();}}]},{tagname:'text',nounformat:true,nomoveout:true,noCust:true,nodelete:false,isSvgShape:true,notclose:true,issvgtext:true,properties:[{name:'text',editor:EDIT_PROPS,byAttribute:true,desc:'Specifies text attributes',cat:PROP_CUST1,editorList:function(o){if(!o.jsData)
o.jsData=createSvgText(o);if(o.jsData)
return o.jsData.getProperties();}}]}];}
var headingDesc='The "h1" to "h6" tags are used to define HTML headings. "h1" defines the most important heading. "h6" defines the least important heading.';function getDefaultAddables(){return[{tag:'a',name:'a',image:'/libjs/html_a.png',toolTips:'The "a" tag defines a hyperlink, which is used to link from one page to another.'},{tag:'bdo',name:'bdo',image:'/libjs/html_bdo.png',toolTips:'bdo stands for Bi-Directional Override. The "bdo" tag is used to override the current text direction.'},{tag:'blockquote',name:'blockquote',image:'/libjs/html_blockquote.png',toolTips:'"blockquote" tag specifies a section that is quoted from another source.'},{tag:'button',name:'button',image:'/libjs/html_button.png',pageonly:true,toolTips:'The "button" tag defines a clickable button. Inside a "button" element you can put content, like text or images. This is the difference between this element and buttons created with the "input" element.'},{tag:'input',name:'checkbox',image:'/libjs/html_checkbox.png',pageonly:true,type:'checkbox',toolTips:'Create a checkbox, it is an "input" with type=checkbox. '},{tag:'cite',name:'cite',image:'/libjs/html_cite.png',toolTips:'The "cite" tag defines the title of a work (e.g. a book, a song, a movie, a TV show, a painting, a sculpture, etc.).'},{tag:'code',name:'code',image:'/libjs/html_code.png',toolTips:'"code" defines a piece of computer code'},{tag:'dl',name:'definition list',image:'/libjs/html_dl.png',toolTips:'The "dl" tag defines a definition list. The "dl" tag is used in conjunction with "dt" (defines the item in the list) and "dd" (describes the item in the list).'},{tag:'div',name:'division',image:'/libjs/html_div.png',pageonly:true,toolTips:'The "div" tag defines a division or a section in an HTML document. The "div" tag is used to group block-elements to format them with styles.'},{tag:'fieldset',name:'fieldset',image:'/libjs/html_fieldset.png',pageonly:true,toolTips:'The "fieldset" tag is used to group related elements in a form. The "fieldset" tag draws a box around the related elements.'},{tag:'form',name:'form',image:'/libjs/html_form.png',pageonly:true,toolTips:'The "form" tag is used to create an HTML form for user input. The "form" element can contain one or more of the following form elements: "input", "textarea", "button", "select", "option", "optgroup", "fieldset", and "label"'},{tag:'h1',name:'heading 1',image:'/libjs/html_h.png',toolTips:headingDesc},{tag:'h2',name:'heading 2',image:'/libjs/html_h.png',toolTips:headingDesc},{tag:'h3',name:'heading 3',image:'/libjs/html_h.png',toolTips:headingDesc},{tag:'h4',name:'heading 4',image:'/libjs/html_h.png',toolTips:headingDesc},{tag:'h5',name:'heading 5',image:'/libjs/html_h.png',toolTips:headingDesc},{tag:'h6',name:'heading 6',image:'/libjs/html_h.png',toolTips:headingDesc},{tag:'hr',name:'horizontal line',image:'/libjs/html_hr.png',pageonly:true,toolTips:'The "hr" tag defines a thematic break in an HTML page (e.g. a shift of topic). The "hr" element is used to separate content (or define a change) in an HTML page.'},{tag:'input',name:'hidden',image:'/libjs/html_hidden.png',pageonly:true,type:'hidden',toolTips:'The Hidden object represents a hidden input field in an HTML form (this input field is invisible for the user). With this element you can send hidden form data to a server.'},{tag:'iframe',name:'iframe',image:'/libjs/html_iframe.png',pageonly:true,onCreated:function(o){if(o){o.setAttribute('src','/libjs/iframeDesign.jpg');}},toolTips:'The "iframe" tag specifies an inline frame. An inline frame is used to embed another document within the current HTML document.'},{tag:'img',name:'image',image:'/libjs/html_img.png',toolTips:'The "img" tag defines an image in an HTML page.'},{tag:'input',name:'input',image:'/libjs/html_input.png',pageonly:true,type:'text',toolTips:'Create a text box using an "input" tag with type=text'},{tag:'kbd',name:'kbd',image:'/libjs/html_kbd.png',pageonly:true,toolTips:'"kbd" defines keyboard input'},{tag:'label',name:'label',image:'/libjs/html_label.png',pageonly:true,toolTips:'The "label" tag defines a label for an "input" element. The "label" element does not render as anything special for the user. However, it provides a usability improvement for mouse users, because if the user clicks on the text within the "label" element, it toggles the control. The for attribute of the "label" tag should be equal to the id attribute of the related element to bind them together.'},{tag:'br',name:'line break',image:'/libjs/html_br.png',toolTips:'The "br" tag inserts a single line break. '},{tag:'ul',name:'list',image:'/libjs/html_ul.png',toolTips:'The "ul" tag defines an unordered (bulleted) list. Use the "ul" tag together with the "li" tag to create unordered lists.'},{tag:'select',name:'list box',image:'/libjs/html_select.png',pageonly:true,toolTips:'The "select" element is used to create a drop-down list. The "option" tags inside the "select" element define the available options in the list.'},{tag:'map',name:'map',image:'/libjs/html_map.png',pageonly:true,toolTips:'The "map" tag is used to define a client-side image-map. An image-map is an image with clickable areas. The required name attribute of the "map" element is associated with the "img"\'s usemap attribute and creates a relationship between the image and the map. The "map" element contains a number of "area" elements, that defines the clickable areas in the image map.'},{tag:'object',name:'object',image:'/libjs/html_object.png',pageonly:true,toolTips:'The "object" tag defines an embedded object within an HTML document. Use this element to embed multimedia (like audio, video, Java applets, ActiveX, PDF, and Flash) in your web pages. You can also use the "object" tag to embed another webpage into your HTML document. You can use the "param" tag to pass parameters to plugins that have been embedded with the "object" tag.'},{tag:'ol',name:'ordered list',image:'/libjs/html_ol.png',toolTips:'The "ol" tag defines an ordered list. An ordered list can be numerical or alphabetical. Use the "li" tag to define list items.'},{tag:'p',name:'paragraph',image:'/libjs/html_p.png',toolTips:'The "p" tag defines a paragraph. Browsers automatically add some space (margin) before and after each "p" element. The margins can be modified with the margin properties.'},{tag:'input',name:'password',image:'/libjs/html_password.png',pageonly:true,type:'password',toolTips:'Create a password input text box using an "input" with type=password'},{tag:'pre',name:'pre',image:'/libjs/html_pre.png',toolTips:'The "pre" tag defines preformatted text. Text in a "pre" element is displayed in a fixed-width font (usually Courier), and it preserves both spaces and line breaks. Use the "pre" element when displaying text with unusual formatting, or some sort of computer code.'},{tag:'span',name:'span',image:'/libjs/html_span.png',toolTips:'The "span" tag is used to group inline-elements in a document. The "span" tag provides no visual change by itself. The "span" tag provides a way to add a hook to a part of a text or a part of a document. When a text is hooked in a "span" element, you can style it with styles, or manipulate it with JavaScript.'},{tag:'input',name:'radio',image:'/libjs/html_radio.png',pageonly:true,type:'radio',toolTips:'Create a radio button using "input" with type=radio'},{tag:'input',name:'reset',image:'/libjs/html_reset.png',pageonly:true,type:'reset',toolTips:'Create a reset button using "input" with type=reset'},{tag:'samp',name:'sample',image:'/libjs/html_samp.png',toolTips:'"samp" defines sample output from a computer program'},{tag:'input',name:'submit',image:'/libjs/html_submit.png',pageonly:true,type:'submit',toolTips:'Create a submit button using "input" with type=submit'},{tag:'table',name:'table',image:'/libjs/html_table.png',toolTips:'The "table" tag defines an HTML table. An HTML table consists of the "table" element and one or more "tr", "th", and "td" elements. The "tr" element defines a table row, the "th" element defines a table header, and the "td" element defines a table cell.'},{tag:'textarea',name:'text area',image:'/libjs/html_textarea.png',pageonly:true,toolTips:'The "textarea" tag defines a multi-line text input control. A text area can hold an unlimited number of characters, and the text renders in a fixed-width font (usually Courier). The size of a text area can be specified by the cols and rows attributes, or even better; through CSS\' height and width properties.'},{tag:'var',name:'var',image:'/libjs/html_var.png',toolTips:'"var" defines a variable'},{tag:'menubar',htmltag:'nav',name:'menu-bar',image:'/libjs/html_menu.png',pageonly:true,toolTips:'It defines a menu bar',onCreated:function(o){if(o){_editorOptions.client.onCreatedObject.apply(_editorOptions.elementEditedWindow,['HtmlEditorMenuBar',o,_editorOptions.client]);_redisplayProperties();}}},{tag:'treeview',htmltag:'ul',name:'tree-view',image:'/libjs/html_treeview.png',pageonly:true,toolTips:'It defines a tree view',onCreated:function(o){if(o){_editorOptions.client.onCreatedObject.apply(_editorOptions.elementEditedWindow,['HtmlEditorTreeview',o,_editorOptions.client]);_initPageElement(o);_redisplayProperties();}}},{tag:'embed',name:'embed',image:'/libjs/html_embed.png',pageonly:true,toolTips:'"embed" is used to embed an object in web page, for example, a music player'},{tag:'video',name:'video',image:'/libjs/html_video.png',pageonly:false,toolTips:'"video" is used to embed a video in web page, for example, a mp4.',onCreated:function(o){o.setAttribute('width',320);o.setAttribute('height',240);o.setAttribute('controls','controls');o.appendChild(_editorOptions.elementEditedDoc.createTextNode('Your browser does not support the video tag.'));}},{tag:'music',htmltag:'embed',name:'music',image:'/libjs/html_music.png',toolTips:'use embed to add a music player plugin',onCreated:function(o){if(o){if(JsonDataBinding.IsIE()){var p=o.parentNode;if(p){var e2=o.cloneNode(false);p.insertBefore(e2,o);p.removeChild(o);_editorOptions.selectedObject=null;selectEditElement(e2);}}
else{_editorOptions.selectedObject=null;selectEditElement(o);}}}},{tag:'youtube',htmltag:'iframe',name:'youtube',image:'/libjs/html_youtube.png',toolTips:'embed a YouTube video in the page',onCreated:function(o){if(o){o.typename='youtube';var src=prompt('Enter vedio ID for your YouTube video. You may leave it blank and later set youtubeID property with your video ID. You may use video URL from YouTube.','');o.setAttribute('src','/libjs/youtube.jpg');o.setAttribute('srcDesign','http://www.youtube.com/embed/'+getyoutubeid(src));o.setAttribute('type',"text/html");o.setAttribute('frameborder',"0");o.setAttribute('allowfullscreen',"true");o.setAttribute('height',"385");o.setAttribute('width',"640");o.setAttribute('typename',"youtube");_editorOptions.selectedObject=null;selectEditElement(o);}}},{tag:'slideshow',htmltag:'div',name:'slide show',image:'/libjs/html_slideshow.png',toolTips:'use a list of images, each with a caption, to form slide show',onCreated:function(o){if(o){var tryCount=0;o.style.width='300px';o.style.height='300px';o.style.border='solid';o.style.borderRadius='15px';o.setAttribute('styleName','limnorslideshow');o.setAttribute('styleshare','NotShare');o.className='limnorslideshow';o.setAttribute('scriptData',_editorOptions.client.createDataId.apply(_editorOptions.editorWindow,['ss',true]));_editorOptions.client.addCustJsLisFile.apply(_editorOptions.editorWindow,['slideshow',o]);function showNewObj(){if(!o.jsData){tryCount++;if(tryCount<30){setTimeout(showNewObj,300);}
else{alert('Timeout loading slide show editor. You may save the page and re-load it to make the slide show editor appear.');limnorHtmlEditor.hideMessage();}
return;}
limnorHtmlEditor.hideMessage();_editorOptions.selectedObject=null;selectEditElement(o);}
limnorHtmlEditor.showMessage('Loading slide show editor, please wait.');showNewObj();}}},{tag:'flash',htmltag:'embed',name:'flash player',image:'/libjs/html_flash.png',toolTips:'play flas video file (*.swf)',onCreated:function(o){if(o){if(JsonDataBinding.IsIE()){var p=o.parentNode;if(p){var e2=o.cloneNode(false);p.insertBefore(e2,o);p.removeChild(o);_editorOptions.selectedObject=null;selectEditElement(e2);}}
else{_editorOptions.selectedObject=null;selectEditElement(o);}}}},{tag:'hitcount',htmltag:'div',name:'hit count',pageonly:true,image:'/libjs/html_hitcount.png',toolTips:'show hit counts of my web home or the page',onCreated:function(o){if(o){o.typename='hitcount';o.setAttribute('typename','hitcount');o.setAttribute('hidechild',true);o.setAttribute('styleshare','notshare');o.id=HtmlEditor.hitcountid;o.style.fontSize='small';o.style.fontFamily='arial';o.style.fontWeight='normal';o.style.color='black';o.innerHTML='Home visits:*****  this page:****';o.contentEditable=false;_editorOptions.selectedObject=null;selectEditElement(o);}}},{tag:'fileupload',htmltag:'form',name:'file upload',pageonly:true,image:'/libjs/html_upload.png',toolTips:'upload a file from web page to web server',onCreated:function(o){if(o){o.typename='fileupload';o.setAttribute('typename','fileupload');o.setAttribute('hidechild','true');o.setAttribute('method','post');o.setAttribute('enctype','multipart/form-data');var hid=_createElement('input');o.appendChild(hid);hid.id=_createId(hid);hid.setAttribute('type','hidden');hid.setAttribute('name','clientRequest');o.setAttribute('clientRequest',hid.id);var hidSize=_createElement('input');o.appendChild(hidSize);hidSize.setAttribute('name','MAX_FILE_SIZE');hidSize.setAttribute('type','hidden');hidSize.setAttribute('value','2048');var f=_createElement('input');o.appendChild(f);var n=1;while(_editorOptions.elementEditedDoc.getElementById('file'+n)&&_editorOptions.elementEditedDoc.getElementById('file'+n+'f')){n++;}
o.id='file'+n+'f';f.id='file'+n;f.setAttribute('type','file');f.setAttribute('name',f.id);if(_editorOptions.forIDE){try{if(typeof(limnorStudio)!='undefined')limnorStudio.onAddFileupload(f.id);else window.external.OnAddFileupload(f.id);}
catch(err){alert(err.message);}}
_editorOptions.selectedObject=null;selectEditElement(o);}}},{tag:'drawingGroup',htmltag:'svg',name:'drawing group',isSVG:true,pageonly:true,image:'/libjs/html_svg.png',toolTips:'a container for holding Scalable Vector Graphics',onCreated:function(o){if(o){var tryCount=0;o.style.width='300px';o.style.height='300px';o.setAttribute('styleshare','NotShare');_editorOptions.client.addCustJsLisFile.apply(_editorOptions.editorWindow,['limnorsvg',o]);function showNewObj(){if(!o.jsData){tryCount++;if(tryCount<30){setTimeout(showNewObj,300);}
else{alert('Timeout loading SVG drawing editor. You may save the page and re-load it to make the SVG drawing editor appear.');limnorHtmlEditor.hideMessage();}
return;}
limnorHtmlEditor.hideMessage();_editorOptions.selectedObject=null;selectEditElement(o);}
limnorHtmlEditor.showMessage('Loading SVG drawing editor, please wait.');showNewObj();}}}];}
function getDefaultCommands(){return[{cmd:'bold',actImg:'/libjs/bold_act.png',inactImg:'/libjs/bold.png',tooltips:'Toggle bold face for the selected text or the element under the cursor'},{cmd:'italic',actImg:'/libjs/italic_act.png',inactImg:'/libjs/italic.png',tooltips:'Toggle italic for the selected text or the element under the cursor'},{cmd:'underline',actImg:'/libjs/underline_act.png',inactImg:'/libjs/underline.png',tooltips:'Toggle underline for the selected text or the element under the cursor'},{cmd:'strikethrough',actImg:'/libjs/strikethrough_act.png',inactImg:'/libjs/strikethrough.png',tooltips:'Toggle strike-through for the selected text'},{cmd:'subscript',actImg:'/libjs/subscript_act.png',inactImg:'/libjs/subscript.png',tooltips:'Toggle subscript for the selected text'},{cmd:'superscript',actImg:'/libjs/superscript_act.png',inactImg:'/libjs/superscript.png',tooltips:'Toggle superscript for the selected text'},{cmd:'alignLeft',actImg:'/libjs/alignLeft.png',inactImg:'/libjs/alignLeftInact.png',isCust:true,tooltips:'text left alignment.'},{cmd:'alignCenter',actImg:'/libjs/alignCenter.png',inactImg:'/libjs/alignCenterInact.png',isCust:true,tooltips:'text center alignment.'},{cmd:'alignRight',actImg:'/libjs/alignRight.png',inactImg:'/libjs/alignRightInact.png',isCust:true,tooltips:'text right alignment.'},{cmd:'indent',actImg:'/libjs/indent.png',inactImg:'/libjs/indent.png',isCust:true,tooltips:'Make current list item the sub item of its previous sibling.'},{cmd:'dedent',actImg:'/libjs/dedent.png',inactImg:'/libjs/dedent.png',isCust:true,tooltips:'Make the current list item a sibling of its parent.'},{cmd:'moveup',actImg:'/libjs/moveup.png',inactImg:'/libjs/moveup.png',isCust:true,tooltips:'Moves the current list item before its previous sibling.'},{cmd:'movedown',actImg:'/libjs/movedown.png',inactImg:'/libjs/movedown.png',isCust:true,tooltips:'Moves the current list item below its sibling.'},{cmd:'createlink',actImg:'/libjs/html_a.png',inactImg:'/libjs/html_a.png',tooltips:'Set hyper link for the selected text'}];}
_init();function _initPageElement(obj){if(obj&&obj.jsData&&obj.jsData.typename=='treeview'){obj.jsData.createSubEditor=function(o,e){if(o==e)
return o;if(e&&e.tagName&&e.tagName.toLowerCase()=='li'){return createTreeViewItemPropDescs(obj,e);}};}}
return{removePropertyFromCssText:function(css,name){return removeFromCSStext(css,name);},editorWindow:function(){return _editorWindow;},promptClose:function(saved){askClose(saved);},close:function(){_close();},showEditor:function(display){_showEditor(display);},togglePropertiesWindow:function(){_togglePropertiesWindow();},refreshProperties:function(){_refreshProperties();},getTableMap:function(rowHolder){return _getTableMap(rowHolder);},addSupportedElements:function(supportedElements){_addEditors(supportedElements);},removeEditor:function(){removeEditorElements();},getHtmlString:function(){return _getHtmlString();},saveAndFinish:function(){_saveAndFinish();},pageModified:function(){return _pageModified();},setDocType:function(docType){_setDocType(docType);},getSelectedObject:function(){return _getSelectedObject();},setSelectedObject:function(guid){_setSelectedObject(guid);},createOrGetId:function(guid){return _createOrGetId(guid);},notifyUsedNames:function(usedNames){_notifyUsedNames(usedNames);},getNamesUsed:function(){return _getNamesUsed();},setPropertyValue:function(propertyName,propertyValue){return _setPropertyValue(propertyName,propertyValue);},appendArchiveFile:function(objId,filePath){return _appendArchiveFile(objId,filePath);},setGuid:function(guid){return _setGuid(guid);},getIdList:function(){return _getIdList();},getIdByGuid:function(guid){return _getIdByGuid(guid);},setGuidById:function(id,guid){return _setGuidById(id,guid);},getMaps:function(){return _getMaps();},getAreas:function(mapId){return _getAreas(mapId);},createNewMap:function(guid){return _createNewMap(guid);},setMapAreas:function(mapId,arealist){_setMapAreas(mapId,arealist);},setUseMap:function(imgId,mapId){_setUseMap(imgId,mapId);},setbodyBk:function(bkFile,bkTile){return _setbodyBk(bkFile,bkTile);},onBeforeIDErun:function(){_onBeforeIDErun();},doCopy:function(){_doCopy();},doPaste:function(){_doPaste();},pasteToHtmlInput:function(txt,selStart,selEnd){_pasteToHtmlInput(txt,selStart,selEnd);},initSelection:function(){_initSelection();},setEditOptions:function(editOptions,verify){if(!editOptions.objEdited){alert('object being edited cannot be null or undefined');return;}
editOptions.isEditingBody=!(editOptions.inline);editOptions.elementEditedWindow=editOptions.objEdited;editOptions.elementEditedDoc=editOptions.elementEditedWindow.document;if(editOptions.isEditingBody){editOptions.elementEdited=editOptions.elementEditedDoc.body;}
if('htmlFileOption'in editOptions){}
else{editOptions.htmlFileOption=0;}
_setEditorTarget(editOptions);},setInlineTarget:function(divEditor){_setInlineTarget(divEditor);},bringToFront:function(){_bringToFront();},onclientmousedown:function(obj,x,y,isRightClick){_onclientmousedown(obj,x,y,isRightClick);},onclientmousemove:function(x,y,ph){_onclientmousemove(x,y,ph);},onclientmouseup:function(obj,ph){_onclientmouseup(obj,ph);},onclientkeyup:function(obj){_onclientkeyup(obj);},setChanged:function(){_setChanged();},onclientBackspace:function(){return _handleBackspace(true);},initPageElement:function(obj){_initPageElement(obj);},showEditorMessage:function(message){_setMessage(message);}};}};var limnorHtmlEditor=limnorHtmlEditor||{serverType:'php',showMessage:function(msg){if(!document.divWaitMessage){document.divWaitMessage=document.createElement('div');document.divWaitMessage.style.width='500px';document.divWaitMessage.style.height='309px';document.divWaitMessage.style.borderStyle='solid';document.divWaitMessage.style.borderRadius='20px';document.divWaitMessage.style.position='absolute';document.divWaitMessage.style.backgroundColor='white';document.divWaitMessage.style.margin='10px';document.divWaitMessage.style.padding='10px';document.divWaitMessage.style.color='red';document.divWaitMessage.style.verticalAlign='middle';document.divWaitMessage.style.fontSize='large';document.body.appendChild(document.divWaitMessage);}
var zi=JsonDataBinding.getPageZIndex(document.divWaitMessage)+1;document.divWaitMessage.innerHTML=msg;document.divWaitMessage.style.zIndex=zi;document.divWaitMessage.style.display='block';JsonDataBinding.windowTools.centerElementOnScreen(document.divWaitMessage);},hideMessage:function(){if(document.divWaitMessage){document.divWaitMessage.style.display='none';}},createPageEditor:function(){if(typeof(HtmlEditor)=='undefined'){var headNode=document.getElementsByTagName('head')[0];var scriptNode=document.createElement('script');scriptNode.setAttribute('type','text/javascript');scriptNode.src='/libjs/htmlEditor.js';headNode.appendChild(scriptNode);}
var tryCount=0;function createEditor(){if(typeof(HtmlEditor)=='undefined'){tryCount++;if(tryCount>30){alert('Timeout loading Javascript');}
else{setTimeout(createEditor,200);}
return;}
if(!HtmlEditor.pageEditor){HtmlEditor.pageEditor=HtmlEditor.createEditor();}}
createEditor();},editHtmlFile:function(url,inlineOptions,pageId,serverFolder,userAlias){var isforIDE=(inlineOptions?(inlineOptions.forIDE?true:false):false);if(isforIDE){return startEditor(false,false);}
else{if(typeof limnorHtmlEditor.editorList=='undefined'){limnorHtmlEditor.editorList=new Array();}
if(url){for(var i=0;i<limnorHtmlEditor.editorList.length;i++){if(limnorHtmlEditor.editorList[i].url==url){if(limnorHtmlEditor.editorList[i].finished&&limnorHtmlEditor.editorList[i].finished()){limnorHtmlEditor.editorList.splice(i,1);break;}
else if(limnorHtmlEditor.editorList[i].activate){limnorHtmlEditor.editorList[i].activate();return;}}}}
var autosaveUrl=url?url+'.autoSave.html':null;if(autosaveUrl){limnorHtmlEditor.editorList.push({url:url});var clientProperties={lang:JsonDataBinding.GetCulture()};JsonDataBinding.accessServer('WebPageUpdater.php','checkHtmlCacheFileExist',url,clientProperties,function(){return startEditor(JsonDataBinding.values.fileExists);});}
else{autosaveUrl='about:blank';url=autosaveUrl;return startEditor(false,true);}}
function startEditor(useSavedUrl,isInline){var _editor;if(isInline){_editor=HtmlEditor.createEditor(inlineOptions.holder);}
else{if(!HtmlEditor.pageEditor){HtmlEditor.pageEditor=HtmlEditor.createEditor();}
_editor=HtmlEditor.pageEditor;}
JsonDataBinding.setupChildManager();document.childManager.onChildWindowReady=function(event){var e=event||window.event||(document.parentWindow&&document.parentWindow.event);var sender=JsonDataBinding.getSender(e);JsonDataBinding.AbortEvent=false;}
var windowParams={};windowParams.isDialog=false;if(isforIDE){windowParams.forceNew=true;windowParams.url=url;windowParams.hideTitle=true;windowParams.center=true;windowParams.width='100%';windowParams.height='100%';}
else if(isInline){windowParams.url=url;windowParams.hideTitle=true;windowParams.contentsHolder=inlineOptions.holder.tdContentContainer;windowParams.width='100%';windowParams.height=0;}
else{if(url=='about:blank')
windowParams.url=url;else
windowParams.url=(useSavedUrl?autosaveUrl:url)+'?htmleditor=1&r='+Math.random();windowParams.center=true;windowParams.width=970;windowParams.height=600;}
windowParams.top=0;windowParams.left=0;windowParams.hideCloseButtons=true;windowParams.resizable=!isInline;windowParams.border='2px double #0000ff';windowParams.ieBorderOffset=4;windowParams.icon='/libjs/editor.png';windowParams.title=' Web page - '+url;if(typeof pageId!='undefined'&&pageId!=0){windowParams.pageId=pageId;}
else{windowParams.pageId=Math.floor((Math.random()*1000000000)+1);}
if(!isInline){limnorHtmlEditor.showMessage('Loading page editor, please wait. <br><br>If it takes too long to finish then you may close all web browser windows and restart.');}
var childObj=JsonDataBinding.showChild(windowParams);if(!isInline){var zi=JsonDataBinding.getPageZIndex(document.divWaitMessage)+1;document.divWaitMessage.style.zIndex=zi;document.divWaitMessage.style.display='block';}
var pageWindow=childObj.getPageWindow();var started=false;var nstartCount=0;function start(){if(pageWindow.document&&pageWindow.document.readyState=='interactive'){nstartCount++;if(nstartCount>3){started=true;}}
if(started||(pageWindow.document&&pageWindow.document.readyState=='complete')){var headNode=pageWindow.document.getElementsByTagName('head')[0];var scriptNode=pageWindow.document.createElement('script');scriptNode.setAttribute('type','text/javascript');if(isforIDE)
scriptNode.src='/libjs/limnorStudioClient.js';else
scriptNode.src='/libjs/htmlEditorClient.js';headNode.appendChild(scriptNode);var editorObj;var nLoadCount=0;function openEditor(){editorObj=pageWindow.limnorHtmlEditorClient;if(!editorObj){editorObj=pageWindow.document.limnorHtmlEditorClient;}
if(editorObj){editorObj.loadEditor.apply(pageWindow,[false,isInline]);var headNode=pageWindow.document.getElementsByTagName('head')[0];headNode.removeChild(scriptNode);function removeEditor(){if(limnorHtmlEditor.editorList){for(var i=0;i<limnorHtmlEditor.editorList.length;i++){if(limnorHtmlEditor.editorList[i].url==url){limnorHtmlEditor.editorList.splice(i,1);break;}}}}
function openClient(){if(editorObj.client){if(editorObj.client.pageInitialized.apply(pageWindow)){editorObj.client.HtmlEditor=HtmlEditor;editorObj.client.isforIDE=isforIDE;childObj.getPageHolder().style.boxShadow='10px 10px 5px #888888';editorObj.client.setSavedUrlFlag(useSavedUrl,childObj.getPageHolder());var editorOptions={forIDE:isforIDE,useSavedUrl:useSavedUrl,wholePage:true,client:editorObj.client,objEdited:pageWindow,serverFolder:serverFolder,userAlias:userAlias,finishHandler:function(pageData){var clientProperties={};clientProperties.html=JsonDataBinding.base64Encode(pageData.html);clientProperties.css=JsonDataBinding.base64Encode(pageData.css);clientProperties.publish=!pageData.toCache;function onFinish(serverFailure){if(serverFailure){alert(serverFailure);}
else{if(pageData.toCache){editorOptions.lastsavetime=new Date();HtmlEditor.pageEditor.refreshProperties();if(pageData.manualInvoke){if(confirm('The page modifications are saved.\r\n Do you want to finish editing this page now?\r\nClick Cancel if you want to continue editing this page.')){HtmlEditor.pageEditor.close();removeEditor();}}}
else{if(isforIDE){if(typeof(limnorStudio)!='undefined')limnorStudio.onUpdated();else window.external.OnUpdated();}
else{HtmlEditor.pageEditor.close();removeEditor();if(limnorHtmlEditor.onPublishPage){limnorHtmlEditor.onPublishPage(pageId,url,pageData.url);}}}}}
var saveTo=url;if(isforIDE){var li=saveTo.lastIndexOf('/');if(li>0){saveTo=saveTo.substr(li+1);}}
if(_editor.saveToFolder){clientProperties.saveToFolder=_editor.saveToFolder;}
JsonDataBinding.accessServer('./WebPageUpdater.php','callSaveHtmlPage',saveTo,clientProperties,onFinish);},reset:function(){var clientProperties={};function onReset(serverFailure){if(serverFailure){alert(serverFailure);}
else{HtmlEditor.pageEditor.close();removeEditor();}}
JsonDataBinding.accessServer('WebPageUpdater.php','removeCacheFiles',autosaveUrl,clientProperties,onReset);},deleteAutoSave:function(){JsonDataBinding.deleteWebFile(limnorHtmlEditor.serverType,autosaveUrl,function(errmsg){if(errmsg){alert(errmsg);}});},getPageHolder:function(){return childObj.getPageHolder();},closePage:function(cancel){if(cancel)
childObj.cancelDialog();else
childObj.closeDialog();removeEditor();}};if(isInline){editorOptions.wholePage=false;editorOptions.inline=true;editorOptions.finishHandler=function(pageData){if(inlineOptions.holder.FinishedHtmlEdit){inlineOptions.holder.FinishedHtmlEdit({target:inlineOptions.holder},inlineOptions.holder,pageData);}};editorOptions.cancelHandler=function(){if(inlineOptions.holder.CancelHtmlEdit){inlineOptions.holder.CancelHtmlEdit({target:inlineOptions.holder},inlineOptions.holder);}};}
else{editorOptions.initLocation={x:100,y:100,w:600,h:350};editorObj.client.resetDynamicStyles.apply(pageWindow);}
editorOptions.docType=editorObj.client.getDocType.apply(pageWindow);_editor.setEditOptions(editorOptions,true);childObj.setOnBringToFront(function(){if(_editor){_editor.setEditOptions(editorOptions);_editor.bringToFront();}});editorObj.client.setPageEditor.apply(pageWindow,[_editor,pageId]);if(isInline){_editor.setInlineTarget(childObj);}
else{editorObj.client.initEditor.apply(pageWindow,[_editor,editorObj.client]);limnorHtmlEditor.hideMessage();if(limnorHtmlEditor.editorList&&limnorHtmlEditor.editorList.length>0){for(var i=0;i<limnorHtmlEditor.editorList.length;i++){if(limnorHtmlEditor.editorList[i].url==url){limnorHtmlEditor.editorList[i].finished=function(){if(childObj)
return childObj.finished;return false;};limnorHtmlEditor.editorList[i].activate=function(){if(!childObj.finished){childObj.bringToFront();}}
break;}}}}
if(isforIDE){try{if(typeof(limnorStudio)!='undefined')limnorStudio.onEditorStarted();else window.external.OnEditorStarted();}
catch(err){alert('start:'+err.message);}}}
else
setTimeout(openClient,100);}
else{setTimeout(openClient,100);}}
openClient();}
else{nLoadCount++;if(nLoadCount<10){setTimeout(openEditor,300);}
else{if(typeof(limnorStudio)=='undefined'||limnorStudio==null){var s='A browser related error occurred. Your browser';if(JsonDataBinding.IsChrome()){s=s+', a Chrome, ';}
else if(JsonDataBinding.IsSafari()){s=s+', a Safari, ';}
s=s+' could not load a javascript file for editing the page.';if(isforIDE){s=s+'The system will try it again.';}
else{s=s+'You may try it again or refresh the page-manager page.';}
alert(s);childObj.cancelDialog();pageWindow.close();}
else{if(isforIDE){if(typeof(limnorStudio)!='undefined')limnorStudio.onLoadEditorFailed();else window.external.OnLoadEditorFailed();}}}}}
setTimeout(openEditor,200);}
else setTimeout(start,30);}
setTimeout(start,300);return _editor;}}};var _vhe;function limnorHtmlEditoreditHtmlFile(url){if(typeof(url)=='undefined'||url==null||url.length==0){var s=window.location.href;var pos=s.indexOf('?');if(pos>0){url=s.substr(pos+1);}}
if(typeof(url)!='undefined'&&url!=null&&url.length>0){_vhe=limnorHtmlEditor.editHtmlFile(url,{forIDE:true});}}
function vheLoaded(){return typeof(_vhe)!='undefined'&&_vhe!=null;}
function setSelectedObject(guid){_vhe.setSelectedObject(guid);}
function getHtmlString(){return _vhe.getHtmlString();}
function removePageEditor(){_vhe.removePageEditor();}
function createOrGetId(guid){return _vhe.createOrGetId(guid);}
function setGuidById(id,guid){return _vhe.setGuidById(id,guid);}
function getTagNameById(id){return _vhe.getTagNameById(id);}
function getIdByGuid(guid){return _vhe.getIdByGuid(guid);}
function getIdList(){return _vhe.getIdList();}
function getMaps(){return _vhe.getMaps();}
function getAreas(){return _vhe.getAreas();}
function createNewMap(guid){return _vhe.createNewMap(guid);}
function setMapAreas(mapId,areas){_vhe.setMapAreas(mapId,areas);}
function setUseMap(imgId,mapId){_vhe.setUseMap(imgId,mapId);}
function setPropertyValue(name,value){_vhe.setPropertyValue(name,value);}
function appendArchiveFile(name,filePath){_vhe.appendArchiveFile(name,filePath);}
function pageModified(){return _vhe.pageModified();}
function saveAndFinish(targetFolder){if(typeof(targetFolder)!='undefined'){_vhe.saveToFolder=targetFolder;}
_vhe.saveAndFinish();}
function setbodyBk(bkFile,bkTile){return _vhe.setbodyBk(bkFile,bkTile)}
function setTargetFolder(folder){_vhe.saveToFolder=folder;}
function showEditorMessage(message){_vhe.showEditorMessage(message);}
function setDebug(debug){JsonDataBinding.Debug=debug;}
function onBeforeIDErun(){_vhe.onBeforeIDErun();}
function doCopy(){_vhe.doCopy();}
function doPaste(){_vhe.doPaste();}
function pasteToHtmlInput(txt,selStart,selEnd){_vhe.pasteToHtmlInput(txt,selStart,selEnd);}