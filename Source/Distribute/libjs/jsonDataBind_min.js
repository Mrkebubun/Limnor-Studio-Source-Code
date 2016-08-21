// jsonDataBind.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

var JsonDataBinding=JsonDataBinding||{_binder:function(){var jsdb_serverPage=String();var jsdb_bind='jsdb';var jsdb_getdata='jsonDb_getData';var jsdb_putdata='jsonDb_putData';var const_userAlias='logonUserAlias';var e_onchange='onchange';var type_func='function';var _isIE=(navigator.appName=='Microsoft Internet Explorer');var _isFireFox=(navigator.userAgent.indexOf("Firefox")!=-1);var _isChrome=(navigator.userAgent.toLowerCase().indexOf('chrome')>-1);var _isSafari=((navigator.userAgent.toLowerCase().indexOf('safari')>-1)&&!_isChrome);IsIE=function(){return _isIE;}
IsFireFox=function(){return _isFireFox;}
IsChrome=function(){return _isChrome;}
IsSafari=function(){return _isSafari;}
getEventSender=function(e){var c;if(!e)var e=window.event;if(e.target)c=e.target;else if(e.srcElement)c=e.srcElement;if(typeof c!='undefined'){if(c.nodeType==3)
c=c.parentNode;}
return c;}
var _serverComponentName;var _clientEventsHolder;_getClientEventHolder=function(eventName,objectName){if(!_clientEventsHolder){_clientEventsHolder={};}
if(!_clientEventsHolder[eventName]){_clientEventsHolder[eventName]={};}
var eh=_clientEventsHolder[eventName];if(!eh[objectName]){eh[objectName]={};}
return eh[objectName];}
_getClientEventObject=function(eventName){if(_clientEventsHolder&&_serverComponentName){var eh=_clientEventsHolder[eventName];if(eh[_serverComponentName]){return eh[_serverComponentName];}}}
var jsdb_cultureName='cultureName';var _cultureName='en';var resTable={'TableName':'_pageResources_','Columns':[{'Name':'cultureName','ReadOnly':'true','Type':'string'}],'PrimaryKey':['cultureName'],'DataRelations':[],'Rows':[]};var _datetimePicker;var _datetimeInputs;_getdatetimepicker=function(){return _datetimePicker;}
_pushDatetimeInput=function(textBoxId){if(!_datetimeInputs){_datetimeInputs=new Array();}
_datetimeInputs.push(textBoxId)}
_setDatetimePicker=function(datetimePicker){_datetimePicker=datetimePicker;if(_datetimePicker){if(_datetimeInputs){for(var i=0;i<_datetimeInputs.length;i++){JsonDataBinding.CreateDatetimePickerForTextBox(_datetimeInputs[i]);}
_datetimeInputs=null;}}}
var dataChangeHandlers={};var sources=new Object();var tableAttributes=new Object();var handlersOnRowIndex=new Object();var onrowdeletehandlers=new Object();var hasActivity=false;var activityWatcher;activity=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){if(hasActivity){hasActivity=false;var uu=u.split(' ');if(uu.length>2){JsonDataBinding.setCookie(const_userAlias,u,uu[2]);}}
activityWatcher=setTimeout(activity,3000);}
else{window.location.reload();}}
else{window.location.reload();}}
var _sessionWatcher=null;var _sessionTimeout=20;var _sessionVariableNames=new Array();_sessionKeepAlive=function(){if(_sessionVariableNames.length>0){for(var i=0;i<_sessionVariableNames.length;i++){var v=JsonDataBinding.getCookie(_sessionVariableNames[i]);JsonDataBinding.setCookie(_sessionVariableNames[i],v,_sessionTimeout);}
_sessionWatcher=setTimeout(_sessionKeepAlive,_sessionTimeout*60*1000);}
else{_sessionWatcher=null;}}
_setSessionTimeout=function(tm){if(tm>=1){_sessionTimeout=tm;}}
_sessionVariableExists=function(variableName){if(_sessionVariableNames.indexOf){return(_sessionVariableNames.indexOf(variableName)>-1);}
else{for(var i=0;i<_sessionVariableNames.length;i++){if(_sessionVariableNames[i]===variableName){return true;}}
return false;}}
_setSessionVariable=function(variableName,value){if(_sessionVariableNames.length==0){if(!_sessionWatcher){_sessionWatcher=setTimeout(_sessionKeepAlive,_sessionTimeout*60*1000);}}
if(!_sessionVariableExists(variableName)){_sessionVariableNames.push(variableName);}
JsonDataBinding.setCookie(variableName,value,_sessionTimeout);}
_initSessionVariable=function(variableName,value){if(_sessionVariableNames.length==0){if(!_sessionWatcher){_sessionWatcher=setTimeout(_sessionKeepAlive,_sessionTimeout*60*1000);}}
if(!_sessionVariableExists(variableName)){_sessionVariableNames.push(variableName);JsonDataBinding.setCookie(variableName,value,_sessionTimeout);}}
_getSessionVariable=function(variableName){var v=JsonDataBinding.getCookie(variableName);if(v!=null){if(!_sessionVariableExists(variableName)){_sessionVariableNames.push(variableName);}}
return v;}
_eraseSessionVariable=function(variableName){for(var i=0;i<_sessionVariableNames.length;i++){if(_sessionVariableNames[i]===variableName){JsonDataBinding.eraseCookie(variableName);_sessionVariableNames.splice(i,1);if(_sessionVariableNames.length==0){if(_sessionWatcher){clearTimeout(_sessionWatcher);}}
break;}}}
_getSessionVariables=function(){var aret=new Array();var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];var pos=c.indexOf('=');if(pos>0){var nm=c.substr(0,pos).replace(/^\s+|\s+$/g,"");var o={'name':nm,'value':c.substr(pos+1)};aret.push(o);}}
return aret;}
_addPageCulture=function(cultureName){if(typeof cultureName=='undefined'||cultureName==null){cultureName='';}
var rowName='row_'+cultureName.replace('-','_');var r=eval(rowName);var idx=resTable.Rows.push(r)-1;if(sources[resTable.TableName]){sources[resTable.TableName].rowIndex=idx;_onRowIndexChange(resTable.TableName);}
else{var v=new Object();v.Tables=new Array();v.Tables.push(resTable);_setDataSource.call(v);}}
_setCulture=function(cultureName){_cultureName=cultureName;if(typeof _cultureName=='undefined'||_cultureName==null){_cultureName='';}
JsonDataBinding.setCookie(jsdb_cultureName,cultureName,null);var idx=-1;for(var i=0;i<resTable.Rows.length;i++){if(resTable.Rows[i].ItemArray[0]==cultureName){idx=i;break;}}
if(idx<0){var sPath=window.location.pathname;var sPage=sPath.substring(sPath.lastIndexOf('/')+1);sPage=sPage.substring(0,sPage.lastIndexOf('.'));var element1=document.createElement('script');if(_cultureName==''){element1.src='libjs/'+sPage+'.js';}
else{element1.src=cultureName+'/'+sPage+'.js';}
element1.type='text/javascript';element1.async=false;document.getElementsByTagName('head')[0].appendChild(element1);}
else{sources[resTable.TableName].rowIndex=idx;_onRowIndexChange(resTable.TableName);}}
_getCulture=function(){return JsonDataBinding.getCookie(jsdb_cultureName);}
_addPageResourceName=function(resName,resType){resTable.Columns.push({'Name':resName,'ReadOnly':'true','Type':resType});}
_setUserLogCookieName=function(nm){const_userAlias=nm;}
_getCurrentUserLevel=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){var uu=u.split(' ');if(uu.length>1){return uu[1];}
return 0;}}
return-1;}
_getCurrentUserAlias=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){var uu=u.split(' ');if(uu.length>0){return uu[0];}}}
return null;}
_userLoggedOn=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){return true;}}
return false;}
var _eventFirer;_setEventFirer=function(eo){_eventFirer=eo;}
_setServerPage=function(pageUrl){jsdb_serverPage=pageUrl;}
_addOnRowIndexChangeHandler=function(tableName,handler){if(typeof handlersOnRowIndex=='undefined'||handlersOnRowIndex==null){handlersOnRowIndex=new Object();}
if(typeof handlersOnRowIndex[tableName]=='undefined'){handlersOnRowIndex[tableName]=new Array();}
handlersOnRowIndex[tableName].push(handler);}
_hasLoggedOn=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){if(JsonDataBinding.TargetUserLevel&&JsonDataBinding.TargetUserLevel>=0){var uu=u.split(' ');if(uu.length>1){if(uu[1]<=JsonDataBinding.TargetUserLevel){return true;}}}
else{return true;}}}
return false;}
_logOff=function(){if(typeof activityWatcher!='undefined'&&activityWatcher!=null){clearTimeout(activityWatcher);}
activityWatcher=null;var u=JsonDataBinding.getCookie(const_userAlias);if(typeof u!='undefined'&&u!=null){if(u.length>0){JsonDataBinding.eraseCookie(const_userAlias);}}
window.location.reload();}
_loginPassed=function(login,expire,userLevel){if(userLevel){if(expire){JsonDataBinding.setCookie(const_userAlias,login+' '+userLevel+' '+expire,expire);}
else{JsonDataBinding.setCookie(const_userAlias,login+' '+userLevel,null);}}
else{if(expire){JsonDataBinding.setCookie(const_userAlias,login+' 0 '+expire,expire);}
else{JsonDataBinding.setCookie(const_userAlias,login+' 0',null);}}
_setupLoginWatcher();var eho=_getClientEventObject('UserLogin');if(eho){eho.UserLogin();}}
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
_setDataSource=function(dataName){var v=this;if(typeof v!='undefined'&&v!=null&&typeof v.Data!='undefined'){v=v.Data;}
if(typeof v!='undefined'&&v!=null&&typeof v.Tables!='undefined'&&v.Tables!=null){for(var i=0;i<v.Tables.length;i++){var name=v.Tables[i].TableName;if(typeof dataName!='undefined'&&dataName!=null&&dataName!=''){if(dataName!=name){continue;}}
var isDataStreaming=false;var dstreaming=_getTableAttribute(name,'isDataStreaming')
if(typeof dstreaming!='undefined'&&dstreaming!=null){isDataStreaming=dstreaming;}
if(isDataStreaming){sources[name].newRowStartIndex=sources[name].Rows.length;for(var r=0;r<v.Tables[i].Rows.length;r++){sources[name].Rows.push(v.Tables[i].Rows[r]);}}
else{sources[name]=v.Tables[i];sources[name].columnIndexes=new Object();sources[name].rowIndex=new Number();sources[name].rowIndex=0;for(var j=0;j<sources[name].Columns.length;j++){sources[name].columnIndexes[sources[name].Columns[j].Name]=j;}}}
for(var k=0;k<v.Tables.length;k++){if(typeof dataName!='undefined'&&dataName!=null&&dataName!=''){if(dataName!=v.Tables[k].TableName){continue;}}
bindTable(v.Tables[k].TableName,true);var eho=_getClientEventHolder('DataArrived',v.Tables[k].TableName);if(eho){if(eho.DataArrived){eho.DataArrived();}}}}}
function bindData(e,name,firstTime){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){if(typeof a.getAttribute!='undefined'){var bd=a.getAttribute(jsdb_bind);if(typeof bd!='undefined'&&bd!=null&&bd!=''){var binds=bd.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var dbTable=bind[0];if(dbTable==name){if(a.isTreeView){if(firstTime){a.jsData.onDataReady(sources[name]);}
else{a.jsData.onRowIndexChange(name);}}
else{if(bind.length==1){if(firstTime){if(a.IsDataRepeater){if(typeof a.jsData=='undefined'){a.jsData=JsonDataBinding.DataRepeater(a,sources[name]);}
else{a.jsData.onDataReady(sources[name]);}}
else{if(typeof a.tagName!='undefined'&&a.tagName!=null){if(a.tagName.toLowerCase()=="table"){if(a.chklist){a.chklist.loadRecords(sources[name].Rows);a.chklist.setMessage('');a.chklist.applyTargetdata();}
else{if(typeof a.jsData=='undefined'){a.jsData=JsonDataBinding.HtmlTableData(a,sources[name]);}
else{a.jsData.onDataReady(sources[name]);}}}}}}
else{if(a.IsDataRepeater){if(a.jsData){a.jsData.onPageIndexChange(name);}}
else{if(typeof a.tagName!='undefined'&&a.tagName!=null){if(a.tagName.toLowerCase()=="table"){if(typeof a.jsData!='undefined'&&a.jsData!=null){a.jsData.onRowIndexChange(name);}}}}}}
else if(bind.length==3){var isListbox=false;if(typeof a.tagName!='undefined'&&a.tagName!=null){isListbox=(a.tagName.toLowerCase()=="select");}
if(isListbox){var itemField=bind[1];var valueField=bind[2];var itemFieldIdx;var valueFieldIdx;for(var c=0;c<sources[name].Columns.length;c++){if(sources[name].Columns[c].Name==itemField){itemFieldIdx=c;}
if(sources[name].Columns[c].Name==valueField){valueFieldIdx=c;}
if(typeof valueFieldIdx!='undefined'&&typeof itemFieldIdx!='undefined'){break;}}
if(typeof valueFieldIdx=='undefined'){if(typeof itemFieldIdx!='undefined'){valueFieldIdx=itemFieldIdx;}}
if(typeof itemFieldIdx=='undefined'){if(typeof valueFieldIdx!='undefined'){itemFieldIdx=valueFieldIdx;}}
if(typeof itemFieldIdx!='undefined'){if(firstTime){if(typeof a.jsData=='undefined'){a.jsData=JsonDataBinding.HtmlListboxData(a,sources[name],itemFieldIdx,valueFieldIdx);}
else{a.jsData.onDataReady(sources[name]);}}
else{if(typeof a.jsData!='undefined'&&a.jsData!=null){a.jsData.onRowIndexChange(name);}}}}
else{var field=bind[1];var target=bind[2];var b=(typeof a.disableMonitor=='undefined')?false:a.disableMonitor;a.disableMonitor=true;if(sources[name].rowIndex>=0&&sources[name].rowIndex<sources[name].Rows.length){if(target=='innerText'){JsonDataBinding.SetInnerText(a,sources[name].Rows[sources[name].rowIndex].ItemArray[sources[name].columnIndexes[field]]);}
else{a[target]=sources[name].Rows[sources[name].rowIndex].ItemArray[sources[name].columnIndexes[field]];}
a.val=sources[name].Rows[sources[name].rowIndex].ItemArray[sources[name].columnIndexes[field]];}
else{if(target=='innerText'){JsonDataBinding.SetInnerText(a,'');}
else{a[target]='';}
a.val='';}
a.disableMonitor=b;a.jsdbRowIndex=sources[name].rowIndex;if(firstTime){if(target=='innerHTML'){if(a.tagName.toLowerCase()=='div'){a.oninnerHtmlChanged=changeBoundData;JsonDataBinding.addTextBoxObserver(a);}}
else if(isEventSupported(a,e_onchange)){JsonDataBinding.AttachEvent(a,e_onchange,changeBoundData);a.onsetbounddata=changeBoundData;JsonDataBinding.addTextBoxObserver(a);}
else{}}}}}}}}
else{bindData(a,name,firstTime);}}}}}
function bindTable(name,firstTime){bindData(document.body,name,firstTime);}
function getNextRowIndex(name,currentIndex){var idx2=-1;var idx=currentIndex+1;while(idx<sources[name].Rows.length){if(!sources[name].Rows[idx].deleted&&!sources[name].Rows[idx].removed){idx2=idx;break;}
idx++;}
return idx2;}
function getPreviousRowIndex(name,currentIndex){var idx2=-1;var idx=currentIndex-1;while(idx>=0){if(!sources[name].Rows[idx].deleted&&!sources[name].Rows[idx].removed){idx2=idx;break;}
idx--;}
return idx2;}
function onRowIndexChange(name){if(typeof sources!='undefined'&&typeof sources[name]!='undefined'){if(typeof handlersOnRowIndex!='undefined'){if(handlersOnRowIndex[name]!=null){for(var i=0;i<handlersOnRowIndex[name].length;i++){handlersOnRowIndex[name][i](sources[name]);}}}}}
_bindData=function(e,name,firstTime){bindData(e,name,firstTime);}
_onRowIndexChange=function(name){bindData(document.body,name,false);onRowIndexChange(name);}
_dataMoveToRecord=function(name,rowIndex){if(rowIndex>=0&&rowIndex<sources[name].Rows.length){sources[name].rowIndex=rowIndex;_onRowIndexChange(name);}}
_dataMoveFirst=function(name){var idx2=getNextRowIndex(name,-1);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}
_dataMoveLast=function(name){var idx2=getPreviousRowIndex(name,sources[name].Rows.length);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}
_dataMoveNext=function(name){if(typeof sources[name]!='undefined'&&sources[name].rowIndex<sources[name].Rows.length-1){var idx2=getNextRowIndex(name,sources[name].rowIndex);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}}
_dataMovePrevious=function(name){if(typeof sources[name]!='undefined'&&sources[name].rowIndex<sources[name].Rows.length&&sources[name].rowIndex>0){var idx2=getPreviousRowIndex(name,sources[name].rowIndex);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}}
_getModifiedRowCount=function(name){var r0=0;if(typeof sources[name]!='undefined'){for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].changed){r0++;}}}
return r0;}
_getDeletedRowCount=function(name){var r0=0;if(typeof sources[name]!='undefined'){for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].deleted){r0++;}}}
return r0;}
_getNewRowCount=function(name){var r0=0;if(typeof sources[name]!='undefined'){for(var r=0;r<sources[name].Rows.length;r++){if(sources[name].Rows[r].added){r0++;}}}
return r0;}
_columnValue=function(name,columnName,rowIndex){if(typeof sources!='undefined'){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){return sources[name].Rows[rowIndex].ItemArray[sources[name].columnIndexes[columnName]];}}
return null;}
_getColExpvalue=function(name,expression,idx){var exp=expression;for(var i=0;i<sources[name].Columns.length;i++){exp=exp.replace(new RegExp("{"+sources[name].Columns[i].Name+"}","gi"),sources[name].Rows[idx].ItemArray[i]);}
return eval(exp);}
_columnExpressionValue=function(name,expression,rowIndex){if(typeof sources!='undefined'){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){return _getColExpvalue(name,expression,rowIndex);}}
return null;}
_statistics=function(name,expression,operator){if(typeof sources!='undefined'){if(typeof rowIndex=='undefined'){rowIndex=sources[name].rowIndex;}
if(operator=='SUM'){var sum=0.0;for(var idx=0;idx<sources[name].Rows.length;idx++){sum=sum+_getColExpvalue(name,expression,idx);}
return sum;}
else if(operator=="AVG"){var sum=0.0;if(sources[name].Rows.length>0){for(var idx=0;idx<sources[name].Rows.length;idx++){var exp=expression;for(var i=0;i<sources[name].columnIndexes.length;i++){exp=exp.replace(new RegExp("{"+sources[name].Columns[i].Name+"}","gi"),sources[name].Rows[idx].ItemArray[sources[name].columnIndexes[columnName]]);}
sum=sum+_getColExpvalue(name,expression,idx);}
return sum/sources[name].Rows.length;}
return sum;}
else if(operator=="MIN"){if(sources[name].Rows.length>0){var m;var i;var idx=0;m=_getColExpvalue(name,expression,idx);for(idx=1;idx<sources[name].Rows.length;idx++){var v=_getColExpvalue(name,expression,idx);if(v<m){m=v;}}
return m;}}
else if(operator=="MAX"){if(sources[name].Rows.length>0){var m;var i;var idx=0;m=_getColExpvalue(name,expression,idx);for(idx=1;idx<sources[name].Rows.length;idx++){var v=_getColExpvalue(name,expression,idx);if(v>m){m=v;}}
return m;}}}
return null;}
_deleteCurrentRow=function(name){var idx=sources[name].rowIndex;if(idx>=0&&idx<sources[name].Rows.length){preserveKeys(name);sources[name].Rows[idx].deleted=true;var idx2=getNextRowIndex(name,idx);if(idx2<0){idx2=getPreviousRowIndex(name,idx);}
if(typeof onrowdeletehandlers[name]!='undefined'){for(var i=0;i<onrowdeletehandlers[name].length;i++){onrowdeletehandlers[name][i](name,idx);}}
sources[name].rowIndex=idx2;bindData(document.body,name,false);}}
_addRow=function(name){var r=new Object();r.added=true;r.ItemArray=new Array();for(var i=0;i<sources[name].Columns.length;i++){r.ItemArray[i]=null;}
var idx=sources[name].Rows.length;sources[name].Rows[idx]=r;sources[name].rowIndex=idx;bindData(document.body,name,false);return idx;}
_resetDataStreaming=function(name){_setTableAttribute(name,'isDataStreaming',false);}
function preserveKeys(name){var tbl=sources[name];if(tbl.rowIndex>=0&&tbl.rowIndex<tbl.Rows.length){if(tbl.PrimaryKey!=null&&tbl.PrimaryKey.length>0){if(!tbl.Rows[tbl.rowIndex].changed&&!tbl.Rows[tbl.rowIndex].deleted&&!tbl.Rows[tbl.rowIndex].removed){if(typeof tbl.Rows[tbl.rowIndex].KeyValues=='undefined'){tbl.Rows[tbl.rowIndex].KeyValues=new Array();for(var k=0;k<tbl.PrimaryKey.length;k++){tbl.Rows[tbl.rowIndex].KeyValues[k]=tbl.Rows[tbl.rowIndex].ItemArray[tbl.columnIndexes[tbl.PrimaryKey[k]]];}}}}}}
function changeBoundData(e){var a;var rIdx;if(e&&typeof e.jsdbRowIndex!='undefined'){rIdx=e.jsdbRowIndex;}
if(e&&typeof e.onsetbounddata=='function'){a=e;}
if(!a){a=getEventSender(e);if(!a){a=e;}}
if(a){if(typeof rIdx=='undefined'){if(typeof a.jsdbRowIndex!='undefined'){rIdx=a.jsdbRowIndex;}}
var dbs=a.getAttribute(jsdb_bind);if(typeof dbs!='undefined'&&dbs!=null&&dbs!=''){var binds=dbs.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var sourceName=bind[0];var tbl=sources[sourceName];if(typeof tbl!='undefined'){var field=bind[1];var target=bind[2];if(target=='value'||target=='innerHTML'){var rIdx0=tbl.rowIndex;if(typeof rIdx=='undefined'){rIdx=rIdx0;}
if(rIdx>=0&&rIdx<tbl.Rows.length){tbl.rowIndex=rIdx;preserveKeys(sourceName);var c=tbl.columnIndexes[field];tbl.Rows[rIdx].ItemArray[c]=a[target];tbl.Rows[rIdx].changed=true;JsonDataBinding.onvaluechanged(tbl,rIdx,c,a[target]);tbl.rowIndex=rIdx0;}
break;}}}}}}
_ondataupdated=function(name){for(p in sources){var item=sources[p];if(typeof item!='undefined'&&item!=null&&typeof(item)!=type_func){if(typeof item.TableName=='undefined'){continue;}
if(typeof name!='undefined'){if(name!=item.TableName){continue;}}
var rows=item.Rows;for(var i=0;i<rows.length;i++){if(rows[i].added){rows[i].added=false;}
if(rows[i].changed){rows[i].changed=false;}
if(rows[i].deleted){rows[i].deleted=false;rows[i].removed=true;}}
var eho=_getClientEventHolder('DataUpdated',name);if(eho){if(eho.DataUpdated){eho.DataUpdated();}}}}}
_sendBoundData=function(dataName,clientProperties,commands){var req=new Object();req.Calls=new Array();if(typeof commands!='undefined'){req.Calls=commands;}
if(typeof clientProperties!='undefined'&&clientProperties!=null){req.values=clientProperties;}
req.Data=new Array();var i=0;for(p in sources){var item=sources[p];if(typeof item!='undefined'&&item!=null&&typeof(item)!=type_func){if(typeof dataName!='undefined'&&dataName!=''&&dataName!=null){if(item.TableName!=dataName){continue;}}
req.Data[i]=new Object();for(n in item){var n0=item[n];if(typeof n0!='undefined'&&n0!=null&&typeof(n0)!=type_func){if(n=='Rows'){var rs=n0;var rs2=new Array();var k=0;for(var j=0;j<rs.length;j++){if(rs[j].changed||rs[j].deleted||rs[j].added){if(!(rs[j].deleted&&rs[j].added)){rs2[k++]=rs[j];}}}
req.Data[i][n]=rs2;}
else{req.Data[i][n]=n0;}}}
i++;}}
_callServer(req);}
_submitBoundData=function(){_sendBoundData('',null,[{method:jsdb_putdata,value:'0'}]);}
_putData=function(dataName){_sendBoundData(dataName,null,[{method:jsdb_putdata,value:dataName}]);}
var DEBUG_SYMBOL="F3E767376E6546a8A15D97951C849CE5";_processServerResponse=function(r,state){var v;var pos=r.indexOf(DEBUG_SYMBOL);if(pos>=0){var debug=r.substring(0,pos);r=r.substring(pos+DEBUG_SYMBOL.length);var winDebug=window.open("","debugWindows");winDebug.document.write('<h1>Debug Information from ');winDebug.document.write(jsdb_serverPage);winDebug.document.write('</h1>');winDebug.document.write('<h2>Client request</h2><p>');winDebug.document.write(debug);winDebug.document.write('</p>');winDebug.document.write('<h2>Server response</h2><p>');winDebug.document.write(r);winDebug.document.write('</p>');}
if(typeof r!='undefined'&&r!=null&&r.length>6){for(var k=0;k<r.length;k++){if(r.charAt(k)=='{'){r=r.substring(k);break;}}
try{v=JSON.parse(r);}
catch(err){var winDebug=window.open("","debugWindows");if(pos<0){winDebug.document.write('<h1>Debug Information from ');winDebug.document.write(jsdb_serverPage);winDebug.document.write('</h1>');winDebug.document.write('<h2>Client request</h2><p>');if(state&&state.Data){winDebug.document.write(JSON.stringify(state.Data));}
else{if(JsonDataBinding.SubmittedForm&&JsonDataBinding.SubmittedForm.clientRequest&&JsonDataBinding.SubmittedForm.clientRequest.value){pos=JsonDataBinding.SubmittedForm.clientRequest.value.indexOf(DEBUG_SYMBOL);var data;if(pos>=0){data=JsonDataBinding.SubmittedForm.clientRequest.value.substring(pos+DEBUG_SYMBOL.length);}
else{data=JsonDataBinding.SubmittedForm.clientRequest.value;}
winDebug.document.write(data);}}
winDebug.document.write('</p>');winDebug.document.write('<h2>Server response</h2><p>');winDebug.document.write(r);winDebug.document.write('</p>');}
winDebug.document.write('<h2>Json exception</h2><p>');winDebug.document.write('<table>');for(var p in err){winDebug.document.write('<tr><td>');winDebug.document.write(p);winDebug.document.write('</td><td>');winDebug.document.write(err[p]);winDebug.document.write('</td></tr>');}
winDebug.document.write('</table>');winDebug.document.write('</p>');}
if(v){JsonDataBinding.values=v.values;_serverComponentName=v.serverComponentName;if(JsonDataBinding.SubmittedForm){if(JsonDataBinding.values.SavedFiles){JsonDataBinding.SubmittedForm.SavedFilePaths=JsonDataBinding.values.SavedFiles;}}
if(typeof v.Data!='undefined'){_setDataSource.call(v.Data);}
if(typeof v.Calls!='undefined'&&v.Calls.length>0){var cf=function(){for(var i=0;i<v.Calls.length;i++){eval(v.Calls[i]);}}
cf.call(v);}}}
if(typeof state!='undefined'&&typeof state.JsEventOwner!='undefined'&&state.JsEventOwner!=null){if(typeof state.JsEventOwner.disabled!='undefined'){state.JsEventOwner.disabled=false;}}
else{if(typeof _eventFirer!='undefined'&&_eventFirer!=null){if(typeof _eventFirer.disabled!='undefined'){_eventFirer.disabled=false;_eventFirer=null;}}}
document.body.style.cursor='default';if(JsonDataBinding.ShowAjaxCallWaitingImage){JsonDataBinding.ShowAjaxCallWaitingImage.style.display='none';}
if(JsonDataBinding.ShowAjaxCallWaitingLabel){JsonDataBinding.ShowAjaxCallWaitingLabel.style.display='none';}}
_callServer=function(data,form){if(JsonDataBinding.LogonPage.length>0){if(!JsonDataBinding.hasLoggedOn()){var curUrl=JsonDataBinding.getPageFilename();window.location.href=JsonDataBinding.LogonPage+'?'+curUrl;return;}}
document.body.style.cursor='wait';if(JsonDataBinding.ShowAjaxCallWaitingImage){JsonDataBinding.ShowAjaxCallWaitingImage.style.display='';}
if(JsonDataBinding.ShowAjaxCallWaitingLabel){JsonDataBinding.ShowAjaxCallWaitingLabel.style.display='';}
if(form){if(form.submit){if(typeof JsonDataBinding.Debug!='undefined'&&JsonDataBinding.Debug){form.clientRequest.value=DEBUG_SYMBOL+JSON.stringify(data);}
else{form.clientRequest.value=JSON.stringify(data);}
JsonDataBinding.SubmittedForm=form;form.submit();return;}}
var xmlhttp;var state=new Object();state.Data=data;if(typeof _eventFirer!='undefined'&&_eventFirer!=null){if(typeof _eventFirer.disabled!='undefined'){_eventFirer.disabled=true;state.JsEventOwner=_eventFirer;}}
if(window.XMLHttpRequest){xmlhttp=new XMLHttpRequest();}
else{xmlhttp=new ActiveXObject('Microsoft.XMLHTTP');}
xmlhttp.onreadystatechange=function(){if(xmlhttp.readyState==4&&(xmlhttp.status==200||xmlhttp.status==500)){_processServerResponse(xmlhttp.responseText,state);}}
var url=jsdb_serverPage+'?timeStamp='+new Date().getTime();xmlhttp.open('POST',url,true);xmlhttp.setRequestHeader('Content-Type','application/x-www-form-urlencoded');if(typeof JsonDataBinding.Debug!='undefined'&&JsonDataBinding.Debug){xmlhttp.send(DEBUG_SYMBOL+JSON.stringify(data));}
else{xmlhttp.send(JSON.stringify(data));}}
_executeServerCommands=function(commands,clientProperties,data,form){var req;if(typeof data!='undefined'&&data!=null){if(typeof data=='boolean'){if(data){_sendBoundData('',clientProperties,commands);}
else{req=new Object();req.Calls=commands;if(typeof clientProperties!='undefined'){req.values=clientProperties;}}}
else if(typeof data=='string'){_sendBoundData(data,clientProperties,commands);}
else{req=new Object();req.Calls=commands;if(typeof clientProperties!='undefined'){req.values=clientProperties;}
req.Data=data;}}
else{req=new Object();req.Calls=commands;if(typeof clientProperties!='undefined'){req.values=clientProperties;}}
if(req){_callServer(req,form);}}
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
_getvaluechangehandler=function(tableName){var t=dataChangeHandlers[tableName];if(t){return t.onvaluechangehandlers;}}}(),setServerPage:function(pageUrl){_setServerPage(pageUrl);},getData:function(dataName,clientProperties){_getData(dataName,clientProperties);},putData:function(dataName,clientProperties){_putData(dataName,clientProperties);},callServer:function(commands,clientProperties,data){_executeServerCommands(commands,clientProperties,data);},executeServerMethod:function(command,clientProperties,form){_executeServerCommands([{method:command,value:'0'}],clientProperties,null,form);},addRow:function(dataName){return _addRow(dataName);},deleteCurrentRow:function(dataName){_deleteCurrentRow(dataName);},submitBoundData:function(){_submitBoundData();},dataMoveFirst:function(dataName){_dataMoveFirst(dataName);},dataMovePrevious:function(dataName){_dataMovePrevious(dataName);},dataMoveNext:function(dataName){_dataMoveNext(dataName);},dataMoveLast:function(dataName){_dataMoveLast(dataName);},dataMoveToRecord:function(dataName,rowIndex){_dataMoveToRecord(dataName,rowIndex);},columnValue:function(dataName,columnName,rowIndex){return _columnValue(dataName,columnName,rowIndex);},columnExpressionValue:function(dataName,expression,rowIndex){return _columnExpressionValue(dataName,expression,rowIndex);},statistics:function(dataName,expression,operator){return _statistics(dataName,expression,operator);},addOnRowIndexChangeHandler:function(tableName,handler){_addOnRowIndexChangeHandler(tableName,handler);},onRowIndexChange:function(name){_onRowIndexChange(name);},getTableBody:function(table){var tb;var tbd=table.getElementsByTagName('tbody');if(tbd){if(tbd.length>0){tb=tbd[0];}}
if(!tb){tb=document.createElement('tbody');table.appendChild(tb);}
return tb;},getSender:function(e){var c;if(!e)var e=window.event;if(e&&e.target)c=e.target;else if(e&&e.srcElement)c=e.srcElement;if(c){if(c.nodeType==3)
c=c.parentNode;}
return c;},getZOrder:function(e){var z=0;while(e){if(e.style){var d=(e.style.zIndex?e.style.zIndex:0);if(d>z)z=d;}
e=e.parentNode;}
return z;},values:Object,Debug:false,SetEventFirer:function(eo){if(typeof eo.disabled!='undefined'){_setEventFirer(eo);}
else{_setEventFirer(null);}},AttachOnRowDeleteHandler:function(name,handler){_attachOnRowDeleteHandler(name,handler);},getPageFilename:function(){var s=window.location.href;s=s.replace(/^.*(\\|\/|\:)/,'');return s;},getCookie:function(name){var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];var pos=c.indexOf('=');if(pos>0){var nm=c.substr(0,pos).replace(/^\s+|\s+$/g,"");if(nm==name){return c.substr(pos+1);}}}
return null;},setCookie:function(name,value,exMinutes){var expires;if(exMinutes){var date=new Date();date.setTime(date.getTime()+(exMinutes*60*1000));expires="; expires="+date.toGMTString();}
else expires="";var ck=name+"="+value+expires+"; path=/";document.cookie=ck;},eraseCookie:function(name){JsonDataBinding.setCookie(name,"",-1);},hasLoggedOn:function(){return _hasLoggedOn();},LoginFailed:function(msgId,msg){var lbl=document.getElementById(msgId);JsonDataBinding.SetInnerText(lbl,msg);var eho=_getClientEventObject('LoginFailed');if(eho){eho.LoginFailed();}},LoginPassed:function(login,expire,userLevel){_loginPassed(login,expire,userLevel);var n=window.location.href.indexOf('?');if(n>=0){window.location.href=window.location.href.substr(n+1);}},LoginPassed2:function(){var n=window.location.href.indexOf('?');if(n>=0){window.location.href=window.location.href.substr(n+1);}},LogOff:function(){_logOff();},LogonPage:'',setupLoginWatcher:function(){_setupLoginWatcher();},TargetUserLevel:0,GetCurrentUserAlias:function(){return _getCurrentUserAlias();},GetCurrentUserLevel:function(){return _getCurrentUserLevel();},UserLoggedOn:function(){return _userLoggedOn();},SetLoginCookieName:function(nm){_setUserLogCookieName(nm);},IsChrome:function(){return(navigator.userAgent.toLowerCase().indexOf('chrome')>-1);},IsIE:function(){return IsIE();},IsFireFox:function(){return IsFireFox();},IsOpera:function(){return(typeof(window.opera)!='undefined');},getInternetExplorerVersion:function(){var rv=-1;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null)
rv=parseFloat(RegExp.$1);}
return rv;},SimpleHandlerChain:function(previous,current){var pre=previous;var cur=current;return function(){pre();cur();}},AttachEvent:function(obj,eventName,handler){if(IsFireFox()||IsSafari()||IsChrome()){if(eventName.substring(0,2)=='on'){eventName=eventName.substring(2);}}
if(typeof(obj.attachEvent)=='function'){obj.attachEvent(eventName,handler);}
else{if(typeof(obj.addEventListener)=='function'){obj.addEventListener(eventName,handler,false);}
else{if(obj[eventName]){obj[eventName]=JsonDataBinding.SimpleHandlerChain(obj[eventName],handler);}
else{obj[eventName]=handler;}}}},SwitchCulture:function(cultreName){if(typeof cultreName=='undefined'||cultreName==null||cultreName=='null'){cultreName=_getCulture();}
if(!cultreName||cultreName=='null'){cultreName='';}
_setCulture(cultreName);},AddPageResourceName:function(resName,resType){_addPageResourceName(resName,resType);},AddPageCulture:function(cultureName){_addPageCulture(cultureName);},PageValues:Object,ProcessPageParameters:function(){if(typeof JsonDataBinding.PageValues=='undefined'){JsonDataBinding.PageValues=new Object();}
var query=window.location.search.substring(1);var vars=query.split("&");for(var i=0;i<vars.length;i++){var pair=vars[i].split("=");JsonDataBinding.PageValues[pair[0]]=unescape(pair[1]);}
JsonDataBinding.anchorAlign.initializeBodyAnchor();JsonDataBinding.AttachEvent(window,'onresize',JsonDataBinding.anchorAlign.applyBodyAnchorAlign);JsonDataBinding.anchorAlign.applyBodyAnchorAlign();},SetInnerText:function(c,v){if(IsIE()){c.innerText=v;}
else if(typeof(c.textContent)=='undefined'){c.innerText=v;}
else{c.textContent=v;}},GetInnerText:function(c){if(IsIE()){return c.innerText;}
else if(typeof(c.textContent)=='undefined'){return c.innerText;}
else{return c.textContent;}},GetSelectedListValue:function(list){if(list.selectedIndex>=0){return list.options[list.selectedIndex].value;}
return null;},GetSelectedListText:function(list){if(list.selectedIndex>=0){return list.options[list.selectedIndex].text;}
return'';},ProcessServerResponse:function(r){_processServerResponse(r);},IFrame:null,SubmittedForm:null,ProcessIFrame:function(){if(JsonDataBinding.IFrame){if(JsonDataBinding.IFrame.document){_processServerResponse(JsonDataBinding.IFrame.document.body.innerHTML);JsonDataBinding.IFrame.document.body.innerHTML='';}}},GetSelectedText:function(){var userSelection;if(window.getSelection){userSelection=window.getSelection();}
else if(document.selection){userSelection=document.selection.createRange();}
var selectedText=userSelection;if(userSelection.text)
selectedText=userSelection.text;else{if(userSelection.anchorNode){selectedText=userSelection.anchorNode.nodeValue;}}
return selectedText;},ShowAjaxCallWaitingImage:null,ShowAjaxCallWaitingLabel:null,SetDatetimePicker:function(datetimePicker){_setDatetimePicker(datetimePicker);},GetDatetimePicker:function(){return _getdatetimepicker();},CreateDatetimePickerForTextBox:function(textBoxId){var dp=JsonDataBinding.GetDatetimePicker();if(dp){var opts={formElements:{},showWeeks:true,statusFormat:"l-cc-sp-d-sp-F-sp-Y",bounds:{position:"absolute",inputRight:true,fontSize:"10px",inputTime:true}};opts.formElements[textBoxId]="Y-ds-m-ds-d";dp.createDatePicker(opts);}
else{_pushDatetimeInput(textBoxId);}},getClientEventHolder:function(eventName,objectName){return _getClientEventHolder(eventName,objectName);},eraseSessionVariable:function(name){_eraseSessionVariable(name);},getSessionVariable:function(name){return _getSessionVariable(name);},setSessionVariable:function(name,value){_setSessionVariable(name,value);},initSessionVariable:function(name,value){_initSessionVariable(name,value);},GetSessionVariables:function(){return _getSessionVariables();},bindDataToElement:function(e,name,firstTime){_bindData(e,name,firstTime);},resetDataStreaming:function(name){_resetDataStreaming(name);},getModifiedRowCount:function(name){return _getModifiedRowCount(name);},getDeletedRowCount:function(name){return _getDeletedRowCount(name);},getNewRowCount:function(name){return _getNewRowCount(name);},setTableAttribute:function(tableName,attributeName,value){_setTableAttribute(tableName,attributeName,value);},getTableAttribute:function(tableName,attributeName){return _getTableAttribute(tableName,attributeName);},addvaluechangehandler:function(tableName,handler){_addvaluechangehandler(tableName,handler);},onvaluechanged:function(t,r,c,val){var ta=_getvaluechangehandler(t.TableName);if(ta){for(var i=0;i<ta.length;i++){if(ta[i]){ta[i].oncellvaluechange(t.TableName,r,c,val);}}}},windowTools:{scrollBarPadding:17,centerElementOnScreen:function(element){var pageDimensions=this.updateDimensions();element.style.top=((this.pageDimensions.verticalOffset()+this.pageDimensions.windowHeight()/2)-(this.scrollBarPadding+element.offsetHeight/2))+'px';element.style.left=((this.pageDimensions.windowWidth()/2)-(this.scrollBarPadding+element.offsetWidth/2))+'px';element.style.position='absolute';},updateDimensions:function(){this.updatePageSize();this.updateWindowSize();this.updateScrollOffset();},updatePageSize:function(){var viewportWidth,viewportHeight;if(window.innerHeight&&window.scrollMaxY){viewportWidth=document.body.scrollWidth;viewportHeight=window.innerHeight+window.scrollMaxY;}
else
if(document.body.scrollHeight>document.body.offsetHeight){viewportWidth=document.body.scrollWidth;viewportHeight=document.body.scrollHeight;}
else{viewportWidth=document.body.offsetWidth;viewportHeight=document.body.offsetHeight;};this.pageSize={viewportWidth:viewportWidth,viewportHeight:viewportHeight};},updateWindowSize:function(){var windowWidth,windowHeight;if(self.innerHeight){windowWidth=self.innerWidth;windowHeight=self.innerHeight;}
else
if(document.documentElement&&document.documentElement.clientHeight){windowWidth=document.documentElement.clientWidth;windowHeight=document.documentElement.clientHeight;}
else
if(document.body){windowWidth=document.body.clientWidth;windowHeight=document.body.clientHeight;};this.windowSize={windowWidth:windowWidth,windowHeight:windowHeight};},updateScrollOffset:function(){var horizontalOffset,verticalOffset;if(self.pageYOffset){horizontalOffset=self.pageXOffset;verticalOffset=self.pageYOffset;}
else
if(document.documentElement&&document.documentElement.scrollTop){horizontalOffset=document.documentElement.scrollLeft;verticalOffset=document.documentElement.scrollTop;}
else if(document.body){horizontalOffset=document.body.scrollLeft;verticalOffset=document.body.scrollTop;};this.scrollOffset={horizontalOffset:horizontalOffset,verticalOffset:verticalOffset};},pageSize:{},windowSize:{},scrollOffset:{},pageDimensions:{pageWidth:function(){return JsonDataBinding.windowTools.pageSize.viewportWidth>JsonDataBinding.windowTools.windowSize.windowWidth?JsonDataBinding.windowTools.pageSize.viewportWidth:JsonDataBinding.windowTools.windowSize.windowWidth;},pageHeight:function(){return JsonDataBinding.windowTools.pageSize.viewportHeight>JsonDataBinding.windowTools.windowSize.windowHeight?JsonDataBinding.windowTools.pageSize.viewportHeight:JsonDataBinding.windowTools.windowSize.windowHeight;},windowWidth:function(){return JsonDataBinding.windowTools.windowSize.windowWidth;},windowHeight:function(){return JsonDataBinding.windowTools.windowSize.windowHeight;},horizontalOffset:function(){return JsonDataBinding.windowTools.scrollOffset.horizontalOffset;},verticalOffset:function(){return JsonDataBinding.windowTools.scrollOffset.verticalOffset;}}},anchorAlign:{getElementWidth:function(p,pageSize){if(p==document.body){if(pageSize){return pageSize.w;}
else{JsonDataBinding.windowTools.updateDimensions();return JsonDataBinding.windowTools.pageDimensions.windowWidth();}}
else
return Math.max(p.offsetWidth,p.scrollWidth);},getElementHeight:function(p,pageSize){if(p==document.body){if(pageSize){return pageSize.h;}
else{JsonDataBinding.windowTools.updateDimensions();return JsonDataBinding.windowTools.pageDimensions.windowHeight();}}
else
return Math.max(p.offsetHeight,p.scrollHeight);},getElementSize:function(p,pageSize){if(p==document.body){if(pageSize){return pageSize;}
else{JsonDataBinding.windowTools.updateDimensions();return{'w':JsonDataBinding.windowTools.pageDimensions.windowWidth(),'h':JsonDataBinding.windowTools.pageDimensions.windowHeight()};}}
else{return{'w':Math.max(p.offsetWidth,p.scrollWidth),'h':Math.max(p.offsetHeight,p.scrollHeight)};}},initializeAnchor:function(e,pageSize){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){if(typeof a.getAttribute!='undefined'){var ah=a.getAttribute('anchor');if(typeof ah!='undefined'&&ah!=null&&ah!=''){var ahs=ah.split(',');for(var k=0;k<ahs.length;k++){if(ahs[k]=='right'||ahs[k]=='bottom'){var psize=JsonDataBinding.anchorAlign.getElementSize(a.parentElement?a.parentElement:a.parentNode,pageSize);a.anchorSize={'x':psize.w-a.offsetLeft-a.offsetWidth,'y':psize.h-a.offsetTop-a.offsetHeight};break;}}}
JsonDataBinding.anchorAlign.initializeAnchor(a,pageSize);}}}},initializeBodyAnchor:function(){JsonDataBinding.windowTools.updateDimensions();var pageSize={'w':JsonDataBinding.windowTools.pageDimensions.windowWidth(),'h':JsonDataBinding.windowTools.pageDimensions.windowHeight()};JsonDataBinding.anchorAlign.initializeAnchor(document.body,pageSize);},anchorRight:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var x=pw-e.anchorSize.x-e.offsetWidth;if(x<0)x=0;e.style.left=x+"px";}},anchorBottom:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var y=ph-e.anchorSize.y-e.offsetHeight;if(y<0)y=0;e.style.top=y+"px";}},anchorLeftRight:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var w=pw-e.anchorSize.x-e.offsetLeft;if(w>0){e.style.width=w+"px";}}},anchorTopBottom:function(e,pageSize){if(e.anchorSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var h=ph-e.anchorSize.y-e.offsetTop;if(h>0){e.style.height=h+"px";}}},alignCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var w=e.offsetWidth;var h=e.offsetHeight;var x=(ps.w-w)/2;var y=(ps.h-h)/2;if(x<0)x=0;if(y<0)y=0;e.style.left=x+"px";e.style.top=y+"px";},alignTopCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var w=e.offsetWidth;var x=(pw-w)/2;if(x<0)x=0;e.style.left=x+"px";},alignBottomCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var h=e.offsetHeight;var w=e.offsetWidth;var y=ps.h-h;var x=(ps.w-w)/2;if(x<0)x=0;if(y<0)y=0;e.style.left=x+"px";e.style.top=y+"px";},alignTopRightElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var pw=JsonDataBinding.anchorAlign.getElementWidth(p,pageSize);var w=e.offsetWidth;var x=(pw-w);if(x<0)x=0;e.style.left=x+"px";},alignLeftCenterElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var h=e.offsetHeight;var y=(ph-h)/2;if(y<0)y=0;e.style.top=y+"px";},alignLeftBottomElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ph=JsonDataBinding.anchorAlign.getElementHeight(p,pageSize);var h=e.offsetHeight;var y=(ph-h);if(y<0)y=0;e.style.top=y+"px";},alignCenterRightElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var w=e.offsetWidth;var h=e.offsetHeight;var x=(ps.w-w);var y=(ps.h-h)/2;if(x<0)x=0;if(y<0)y=0;e.style.top=y+"px";e.style.left=x+"px";},alignBottomRightElement:function(e,pageSize){var p=e.parentElement?e.parentElement:e.parentNode;var ps=JsonDataBinding.anchorAlign.getElementSize(p,pageSize);var es=JsonDataBinding.anchorAlign.getElementSize(e,pageSize);var x=(ps.w-es.w)-2;var y=(ps.h-es.h)-2;if(x<0)x=0;if(y<0)y=0;e.style.left=x+"px";e.style.top=y+"px";},applyBodyAnchorAlign:function(){JsonDataBinding.windowTools.updateDimensions();var pageSize={'w':JsonDataBinding.windowTools.pageDimensions.windowWidth(),'h':JsonDataBinding.windowTools.pageDimensions.windowHeight()};JsonDataBinding.anchorAlign.applyAnchorAlign(document.body,pageSize);},applyAnchorAlign:function(e,pageSize){if(!e)return;for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(typeof a!='undefined'&&a!=null){if(typeof a.getAttribute!='undefined'){var ah=a.getAttribute('anchor');if(typeof ah!='undefined'&&ah!=null&&ah!=''){var ahs=ah.split(',');var ahLeft=false;var ahRight=false;var ahTop=false;var ahBottom=false;var posAlign='';for(var k=0;k<ahs.length;k++){if(ahs[k]=='right')ahRight=true;else if(ahs[k]=='bottom')ahBottom=true;else if(ahs[k]=='left')ahLeft=true;else if(ahs[k]=='top')ahTop=true;else posAlign=ahs[k];}
if(ahRight||ahBottom){if(ahRight){if(ahLeft)
JsonDataBinding.anchorAlign.anchorLeftRight(a,pageSize);else
JsonDataBinding.anchorAlign.anchorRight(a,pageSize);}
if(ahBottom){if(ahTop)
JsonDataBinding.anchorAlign.anchorTopBottom(a,pageSize);else
JsonDataBinding.anchorAlign.anchorBottom(a,pageSize);}
if((ahRight&&ahLeft)||(ahBottom&&ahTop)){JsonDataBinding.anchorAlign.applyAnchorAlign(a,pageSize);}}
else{if(posAlign=='center')
JsonDataBinding.anchorAlign.alignCenterElement(a,pageSize);else if(posAlign=='topcenter')
JsonDataBinding.anchorAlign.alignTopCenterElement(a,pageSize);else if(posAlign=='topright')
JsonDataBinding.anchorAlign.alignTopRightElement(a,pageSize);else if(posAlign=='leftcenter')
JsonDataBinding.anchorAlign.alignLeftCenterElement(a,pageSize);else if(posAlign=='leftbottom')
JsonDataBinding.anchorAlign.alignLeftBottomElement(a,pageSize);else if(posAlign=='bottomcenter')
JsonDataBinding.anchorAlign.alignBottomCenterElement(a,pageSize);else if(posAlign=='centerright')
JsonDataBinding.anchorAlign.alignCenterRightElement(a,pageSize);else if(posAlign=='bottomright')
JsonDataBinding.anchorAlign.alignBottomRightElement(a,pageSize);}}}}}}},ElementPosition:{_elementPos:function(){function __getIEVersion(){var rv=-1;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null)
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
return res;}}(),getElementPosition:function(element){return getElementAbsolutePos(element);}},_textBoxObserver:function(){var textBoxes;var timerId;var poll=function(){for(var i=0;i<textBoxes.length;i++){var ctrl=textBoxes[i];if(!ctrl.disableMonitor){var changed=false;if(ctrl.isHtml){if(ctrl.val!=ctrl.innerHTML){if(ctrl.val==null){if(ctrl.innerHTML!=null&&ctrl.innerHTML!='null'){changed=true;}}
else{changed=true;}}}
else{if(ctrl.val!=ctrl.value){if(ctrl.val==null){if(ctrl.value!=null&&ctrl.value!='null'){changed=true;}}
else{changed=true;}}}
if(changed){if(ctrl.isHtml){ctrl.val=ctrl.innerHTML;if(ctrl.oninnerHtmlChanged){ctrl.oninnerHtmlChanged(ctrl);}}
else{ctrl.val=ctrl.value;if(ctrl.fireEvent){ctrl.fireEvent("onchange");}
else if(document.createEvent&&ctrl.dispatchEvent){var evt=document.createEvent("HTMLEvents");evt.initEvent("change",true,true);ctrl.dispatchEvent(evt);}}}}}}
AddTextBox=function(textBox){if(typeof textBoxes=='undefined'){textBoxes=new Array();}
var found=false;for(var i=0;i<textBoxes.length;i++){if(textBoxes[i]==textBox){found=true;break;}}
if(!found){if(textBox.tagName.toLowerCase()=='div'){textBox.val=textBox.innerHTML;textBox.isHtml=true;}
else{textBox.val=textBox.value;textBox.isHtml=false;}
textBoxes.push(textBox);}
if(typeof timerId=='undefined'){timerId=window.setInterval(poll,300);}}
ShowTextBoxCount=function(){if(typeof textBoxes=='undefined')
return 0;return textBoxes.length;}}(),addTextBoxObserver:function(textBox){AddTextBox(textBox);},HtmlTableData:function(tableElement,jsTable){var _tblElement=tableElement;var _jsonTable=jsTable;var _readOnly=false;var _rowTemplate;var attr=_tblElement.getAttribute('readonly');if(typeof attr!='undefined'&&attr){_readOnly=true;}
var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody.rows.length>0){_rowTemplate=new Array();for(var k=0;k<tbody.rows[0].cells.length;k++){_rowTemplate[k]=tbody.rows[0].cells[k].style.cssText;}}
function init(){JsonDataBinding.addvaluechangehandler(_jsonTable.TableName,_tblElement);JsonDataBinding.AttachOnRowDeleteHandler(_jsonTable.TableName,onrowdelete);if(_tblElement.ReadOnlyFields){if(_tblElement.ReadOnlyFields.length>0){for(var i=0;i<_tblElement.ReadOnlyFields.length;i++){var cn=_tblElement.ReadOnlyFields[i].toLowerCase();for(var c=0;c<_jsonTable.Columns.length;c++){if(cn==_jsonTable.Columns[c].Name.toLowerCase()){_jsonTable.Columns[c].ReadOnly=true;break;}}}}}
recreateTableElement();}
var _textBoxElement;var _buttonElement;var _selectionElement;var _datetimePickerButton;var _lookupTableElements;var _chklstTableElements;var EDITOR_NONE=-1;var EDITOR_TEXT=0;var EDITOR_ENUM=1;var EDITOR_DATETIME=2;var EDITOR_DBLOOKUP=3;var EDITOR_CHKLIST=4;if(_tblElement.FieldEditors){for(var c=0;c<_jsonTable.Columns.length;c++){var ed=_tblElement.FieldEditors[c];if(ed){if(ed.Editor==EDITOR_CHKLIST){var tblList=JsonDataBinding.createCheckedList(ed.Tablename,_tblElement,c);if(!_chklstTableElements){_chklstTableElements={};}
_chklstTableElements[ed.Tablename]=tblList;tblList.chklist.setPosition("absolute");ed.CellPainter=tblList.chklist;}}}}
var _selectedRow;function getCellLocation(element){return JsonDataBinding.ElementPosition.getElementPosition(element);}
function getCell(e){var c=getEventSender(e);if(c){if(c.tr){return c;}
if(c.ownerCell){return c.ownerCell;}}
return null;}
function createCell(c){var td=document.createElement('TD');if(_rowTemplate){if(_rowTemplate.length>c){td.style.cssText=_rowTemplate[c];}}
return td;}
function showCell(td,c,val){var editor;if(_tblElement.FieldEditors&&_tblElement.FieldEditors[c]){editor=_tblElement.FieldEditors[c];}
if(editor&&editor.Editor==EDITOR_ENUM){var found=false;if(editor.Values){for(var i=0;i<editor.Values.length;i++){if(editor.Values[i][1]==val){found=true;var txt=document.createTextNode(editor.Values[i][0]);td.appendChild(txt);break;}}}
if(!found){var txt=document.createTextNode(val);td.appendChild(txt);}}
else if(editor&&editor.CellPainter){td.showCell=editor.CellPainter.showCell(td,c,val);}
else{if(typeof val=='undefined'||val==null){td.appendChild(document.createTextNode('{null}'));}
else{var txt=document.createTextNode(val);td.appendChild(txt);}}}
function addHtmlTableRow(tbody,r){var isDeleted=(_jsonTable.Rows[r].deleted||_jsonTable.Rows[r].removed);var tr=tbody.insertRow(-1);tr.datarownum=r;for(var c=0;c<_jsonTable.Columns.length;c++){var td=createCell(c);JsonDataBinding.AttachEvent(td,'onmouseover',onCellMouseOver);JsonDataBinding.AttachEvent(td,'onmouseout',onCellMouseOut);JsonDataBinding.AttachEvent(td,'onclick',onCellClick);showCell(td,c,_jsonTable.Rows[r].ItemArray[c]);td.datarownum=r;td.columnIndex=c;td.tr=tr;tr.appendChild(td);}
if(isDeleted){tr.style.display='none';}
else{tr.style.display='';}
if(_jsonTable.rowIndex==r){_selectedRow=tr;}
if(_tblElement.AlternateBackgroundColor&&r%2!=0){tr.style.backgroundColor=_tblElement.AlternateBackgroundColor;}}
function recreateTableElement(){_selectedRow=null;var tbody=JsonDataBinding.getTableBody(_tblElement);if(tbody==null){tbody=document.createElement('tbody');_tblElement.appendChild(tbody);}
tbody.isJs=true;while(tbody.rows.length>0){tbody.deleteRow(tbody.rows.length-1);}
var th=_tblElement.tHead;if(typeof th=='undefined'||th==null){th=document.createElement('thead');_tblElement.appendChild(th);}
var tr;if(th.rows.length>0){tr=th.rows[0];}
else{tr=document.createElement('tr');th.appendChild(tr);}
for(var c=0;c<_jsonTable.Columns.length;c++){if(c<tr.cells.length){if(tr.cells[c].innerHTML.length==0){JsonDataBinding.SetInnerText(tr.cells[c],_jsonTable.Columns[c].Name);}}
else{var td=document.createElement('td');td.appendChild(document.createTextNode(_jsonTable.Columns[c].Name));tr.appendChild(td);}}
for(var r=0;r<_jsonTable.Rows.length;r++){addHtmlTableRow(tbody,r);}
if(_tblElement.SelectedRowColor){if(_selectedRow!=null){var cells=_selectedRow.getElementsByTagName("td");for(var c=0;c<cells.length;c++){cells[c].style.backgroundColor=_tblElement.SelectedRowColor;}}}}
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
function oncellbuttonclick(){if(_buttonElement){if(_buttonElement.cell){var cell=_buttonElement.cell;if(_buttonElement.editor==EDITOR_ENUM){var needFill=true;if(typeof _selectionElement!='undefined'&&_selectionElement.cell&&_selectionElement.cell.columnIndex==_buttonElement.cell.columnIndex){needFill=false;}
if(needFill){var newSel=document.createElement('select');newSel.style.position='absolute';newSel.style.zIndex=_tblElement.style.zIndex+2;if(typeof _selectionElement!='undefined'){document.body.replaceChild(newSel,_selectionElement);}
else{document.body.appendChild(newSel);}
for(var i=0;i<_tblElement.FieldEditors[cell.columnIndex].Values.length;i++){var elOptNew=document.createElement('option');elOptNew.text=_tblElement.FieldEditors[cell.columnIndex].Values[i][0];elOptNew.value=_tblElement.FieldEditors[cell.columnIndex].Values[i][1];try{newSel.add(elOptNew,null);}
catch(ex){newSel.add(elOptNew);}}
_selectionElement=newSel;JsonDataBinding.AttachEvent(_selectionElement,'onclick',oncellselectionclick);JsonDataBinding.AttachEvent(_selectionElement,'onkeydown',oncellselectionkeydown);}
_selectionElement.cell=_buttonElement.cell;_selectionElement.style.left=_buttonElement.cell.cellLocation.x+document.body.scrollLeft;_selectionElement.style.top=_buttonElement.offsetTop+_buttonElement.offsetHeight;_selectionElement.style.display='block';_selectionElement.size=_selectionElement.options.length;JsonDataBinding.windowTools.updateDimensions();if(_selectionElement.offsetHeight+_selectionElement.offsetTop>JsonDataBinding.windowTools.pageDimensions.windowHeight()){var newTop=_buttonElement.offsetTop-_selectionElement.offsetHeight;if(newTop>=0){_selectionElement.style.top=newTop+"px";}}
_selectionElement.style.zIndex=100;_selectionElement.focus();}
else if(_buttonElement.editor==EDITOR_DBLOOKUP){if(!_lookupTableElements){_lookupTableElements={};}
if(_buttonElement.cell){if(_tblElement.FieldEditors){if(_tblElement.FieldEditors[_buttonElement.cell.columnIndex]){var tname=_tblElement.FieldEditors[_buttonElement.cell.columnIndex].TableName;var tbl=_lookupTableElements[tname];if(!tbl){var u=new Object();tbl=document.createElement("table");tbl.setAttribute("jsdb",tname);tbl.style.position='absolute';tbl.style.zIndex=1+(_tblElement.style.zIndex)?_tblElement.style.zIndex:1;tbl.border=1;tbl.id=tname;tbl.TargetTable=_tblElement;tbl.Editor=_tblElement.FieldEditors[_buttonElement.cell.columnIndex];tbl.style.backgroundColor='white';tbl.HighlightRowColor='#c0ffc0';tbl.SelectedRowColor='#c0c0ff';tbl.setAttribute('readonly','true');var _onmousedown=function(e){var target=getEventSender(e);if(target){var focusTbl;target=target.parentNode;while(target){if(target.tagName){if(target.tagName.toLowerCase()=="table"){focusTbl=target;break;}}
target=target.parentNode;}
if(focusTbl){if(focusTbl==tbl){return;}}}
tbl.style.display='none';}
document.body.appendChild(tbl);_lookupTableElements[tname]=tbl;tbl.style.zIndex=100;JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedown);var tbody;var tbds=tbl.getElementsByTagName('tbody');if(tbds){if(tbds.length>0){tbody=tbds[0];}}
if(!tbody){tbody=document.createElement('tbody');tbl.appendChild(tbody);}
var tr=tbody.insertRow(-1);var td=document.createElement("td");tr.appendChild(td);td.innerHTML="<font color=red>Loading data from web server...</font>";tbl.cell=_buttonElement.cell;tbl.style.left=_buttonElement.cell.cellLocation.x+document.body.scrollLeft+"px";tbl.style.top=(_buttonElement.offsetTop+_buttonElement.offsetHeight)+"px";tbl.style.display='block';JsonDataBinding.executeServerMethod(tname,u);}
else{tbl.cell=_buttonElement.cell;tbl.style.left=_buttonElement.cell.cellLocation.x+document.body.scrollLeft+"px";tbl.style.top=(_buttonElement.offsetTop+_buttonElement.offsetHeight)+"px";tbl.style.display='block';}
JsonDataBinding.windowTools.updateDimensions();if(tbl.offsetHeight+tbl.offsetTop>JsonDataBinding.windowTools.pageDimensions.windowHeight()){var newTop=_buttonElement.offsetTop-tbl.offsetHeight;if(newTop>=0){tbl.style.top=newTop+"px";}}
tbl.focus();}}}}
else if(_buttonElement.editor==EDITOR_CHKLIST){if(!_chklstTableElements){_chklstTableElements={};}
if(_buttonElement.cell){if(_tblElement.FieldEditors){var ed=_tblElement.FieldEditors[_buttonElement.cell.columnIndex];if(ed){var tname=ed.TableName;var tbl=_chklstTableElements[tname];if(!tbl){tbl=JsonDataBinding.createCheckedList(tname,_tblElement,_buttonElement.cell.columnIndex);_chklstTableElements[tname]=tbl;tbl.chklist.setPosition("absolute");ed.CellPainter=tbl.chklist;tbl.chklist.move(_buttonElement.cell.cellLocation.x+document.body.scrollLeft+"px",(_buttonElement.offsetTop+_buttonElement.offsetHeight)+"px");}}
tbl.chklist.move(_buttonElement.cell.cellLocation.x+document.body.scrollLeft+"px",(_buttonElement.offsetTop+_buttonElement.offsetHeight)+"px");tbl.chklist.loadData();tbl.chklist.move(_buttonElement.cell.cellLocation.x+document.body.scrollLeft+"px",(_buttonElement.offsetTop+_buttonElement.offsetHeight)+"px");tbl.chklist.setDisplay("block");tbl.chklist.setzindex(_buttonElement.style.zIndex+1);JsonDataBinding.windowTools.updateDimensions();if(tbl.chklist.getBottom()>JsonDataBinding.windowTools.pageDimensions.windowHeight()){var newTop=_buttonElement.offsetTop-tbl.offsetHeight;if(newTop>=0){tbl.chklist.move(_buttonElement.cell.cellLocation.x+document.body.scrollLeft+"px",newTop+"px");}}
tbl.style.zIndex=100;tbl.focus();}}}}}}
function oncellselectionchange(){if(_selectionElement.selectedIndex>=0&&_selectionElement.selectedIndex<_selectionElement.options.length){var val=_selectionElement.options[_selectionElement.selectedIndex].value;if(_jsonTable.Rows[_selectionElement.cell.datarownum].ItemArray[_selectionElement.cell.columnIndex]!=val){JsonDataBinding.SetInnerText(_selectionElement.cell,_selectionElement.options[_selectionElement.selectedIndex].text);_jsonTable.Rows[_selectionElement.cell.datarownum].changed=true;_jsonTable.Rows[_selectionElement.cell.datarownum].ItemArray[_selectionElement.cell.columnIndex]=val;if(_tblElement.ColumnValueChanged){_tblElement.ColumnValueChanged(_tblElement,_selectionElement.cell.datarownum,_selectionElement.cell.columnIndex,val);}}}
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
if(cell){_jsonTable.rowIndex=cell.datarownum;if(typeof _textBoxElement!='undefined'&&typeof _buttonElement!='undefined'){var cellReadOnly=false;if(typeof _jsonTable.Columns[cell.columnIndex].ReadOnly!='undefined'){cellReadOnly=_jsonTable.Columns[cell.columnIndex].ReadOnly;}
var editor=(cellReadOnly||_readOnly)?EDITOR_NONE:EDITOR_TEXT;if(typeof _tblElement.FieldEditors!='undefined'&&_tblElement.FieldEditors!=null){if(typeof _tblElement.FieldEditors[cell.columnIndex]!='undefined'&&_tblElement.FieldEditors[cell.columnIndex]!=null){if(typeof _tblElement.FieldEditors[cell.columnIndex].Editor!='undefined'){editor=_tblElement.FieldEditors[cell.columnIndex].Editor;}}}
if(editor==EDITOR_NONE){_textBoxElement.cell=null;_textBoxElement.style.display='none';_buttonElement.cell=null;_buttonElement.style.display='none';}
else{_buttonElement.editor=editor;if(editor==EDITOR_TEXT||editor==EDITOR_DATETIME){_textBoxElement.cell=null;_textBoxElement.disableMonitor=true;_textBoxElement.val=JsonDataBinding.GetInnerText(cell);_textBoxElement.value=JsonDataBinding.GetInnerText(cell);_textBoxElement.style.display='block';_textBoxElement.focus();cell.cellLocation=getCellLocation(cell);_textBoxElement.style.left=cell.cellLocation.x+document.body.scrollLeft;_textBoxElement.style.top=cell.cellLocation.y+document.body.scrollTop;_textBoxElement.style.width=cell.offsetWidth;_textBoxElement.cell=cell;_textBoxElement.disableMonitor=false;_buttonElement.cell=null;_buttonElement.style.display='none';if(editor==EDITOR_DATETIME){if(typeof _datetimePickerButton=='undefined'){var dp=JsonDataBinding.GetDatetimePicker();if(typeof dp!='undefined'){var opts={formElements:{},showWeeks:true,statusFormat:"l-cc-sp-d-sp-F-sp-Y",bounds:{position:"absolute",inputRight:true,fontSize:"10px",inputTime:true}};opts.formElements[_textBoxElement.id]="Y-ds-m-ds-d";_datetimePickerButton=dp.createDatePicker(opts);}}
if(typeof _datetimePickerButton!='undefined'){if(cell.offsetWidth>_datetimePickerButton.offsetWidth){_textBoxElement.style.width=(cell.offsetWidth-_datetimePickerButton.offsetWidth)+'px';}
var cx=cell.cellLocation.x+document.body.scrollLeft;var cy=cell.cellLocation.y+document.body.scrollTop;if(cx+cell.offsetWidth>_datetimePickerButton.offsetWidth){_datetimePickerButton.style.left=(cx+cell.offsetWidth-22)+'px';}
else{_datetimePickerButton.style.left=cx+'px';}
_datetimePickerButton.style.top=cy+'px';_datetimePickerButton.cell=cell;_datetimePickerButton.style.zIndex=110;_datetimePickerButton.style.display='block';}}}
else if(editor==EDITOR_ENUM||editor==EDITOR_DBLOOKUP||editor==EDITOR_CHKLIST){_textBoxElement.cell=null;_textBoxElement.style.display='none';if(editor==EDITOR_ENUM){_buttonElement.src='images/dropdownbutton.jpg';}
else if(editor==EDITOR_DBLOOKUP){_buttonElement.src='libjs/qry.jpg';}
else if(editor==EDITOR_CHKLIST){_buttonElement.src='libjs/chklist.jpg';}
var cl=(cell.cellLocation.x+document.body.scrollLeft+cell.offsetWidth-_buttonElement.offsetWidth)+'px';_buttonElement.style.left=cl;_buttonElement.style.top=cell.cellLocation.y+document.body.scrollTop;_buttonElement.style.width="17px";_buttonElement.style.height="15px";if(_buttonElement.offsetHeight>cell.offsetHeight){_buttonElement.style.height=cell.offsetHeight;_buttonElement.style.width=_buttonElement.style.height;}
_buttonElement.cell=cell;_buttonElement.style.display='block';if(_buttonElement.offsetHeight>cell.offsetHeight){_buttonElement.style.height=cell.offsetHeight;_buttonElement.style.width=_buttonElement.style.height;}
cl=(cell.cellLocation.x+document.body.scrollLeft+cell.offsetWidth-_buttonElement.offsetWidth)+'px';_buttonElement.style.left=cl;_buttonElement.style.zIndex=100;}}}
if(_tblElement.TargetTable){_tblElement.style.display='none';if(_tblElement.TargetTable.jsData&&_tblElement.Editor&&_tblElement.Editor.Map){for(var i=0;i<_tblElement.Editor.Map.length;i++){var map=_tblElement.Editor.Map[i];var val=_getColumnValue(map[1]);_tblElement.TargetTable.jsData.setColumnValue(map[0],val);}}}
else{JsonDataBinding.onRowIndexChange(_jsonTable.TableName);}}}
function _clickCell(cell){var r=-1;if(cell){r=cell.datarownum;}
if(r>=0&&r<_jsonTable.Rows.length){if(r!=_jsonTable.rowIndex){_jsonTable.rowIndex=r;_onRowIndexChange(_jsonTable.TableName);}
if(_selectedRow&&_selectedRow.datarownum==r){onCellClick(cell);}}}
function _onRowIndexChange(name){if(name==_jsonTable.TableName){var tbody=JsonDataBinding.getTableBody(_tblElement);if(_selectedRow){if(_selectedRow.datarownum!=_jsonTable.rowIndex){var cells=_selectedRow.getElementsByTagName("TD");var bkc=tbody.style.backgroundColor;if(_tblElement.AlternateBackgroundColor&&_selectedRow.datarownum%2!=0){bkc=_tblElement.AlternateBackgroundColor;}
_selectedRow.style.backgroundColor=bkc;for(var c=0;c<cells.length;c++){cells[c].style.backgroundColor=bkc;}}}
_selectedRow=null;var rn=tbody.rows.length;if(rn<_jsonTable.Rows.length){for(var r=rn;r<_jsonTable.Rows.length;r++){addHtmlTableRow(tbody,r);}}
for(var r=0;r<tbody.rows.length;r++){if(typeof tbody.rows[r].datarownum!='undefined'){if(tbody.rows[r].datarownum==_jsonTable.rowIndex){_selectedRow=tbody.rows[r];if(typeof _textBoxElement!='undefined'){if(typeof _textBoxElement.cell!='undefined'&&_textBoxElement.cell!=null){if(_textBoxElement.cell.datarownum!=_jsonTable.rowIndex){_textBoxElement.cell=null;_textBoxElement.style.display='none';}}}
break;}}}
if(_selectedRow!=null){var bkc;if(_tblElement.SelectedRowColor){bkc=_tblElement.SelectedRowColor;}
else{bkc=tbody.style.backgroundColor;}
_selectedRow.style.backgroundColor=bkc;var row=_jsonTable.Rows[_selectedRow.datarownum];var dataRowVer=typeof row.rowVersion=='undefined'?0:row.rowVersion;var viewRowVer=typeof _selectedRow.rowVersion=='undefined'?0:_selectedRow.rowVersion;var cells=_selectedRow.getElementsByTagName("TD");if(viewRowVer<dataRowVer){_selectedRow.rowVersion=dataRowVer;for(var c=0;c<cells.length;c++){cells[c].innerHTML='';showCell(cells[c],c,row.ItemArray[c]);}}
for(var c=0;c<cells.length;c++){cells[c].style.backgroundColor=bkc;}}}}
_tblElement.oncellvaluechange=function(name,r,c,value){if(_jsonTable.TableName==name){if(_selectedRow!=null){if(_selectedRow.datarownum==r){if(_tblElement.FieldEditors){if(_tblElement.FieldEditors[c]){if(_tblElement.FieldEditors[c].CellPainter){if(_selectedRow.cells[c].showCell){_selectedRow.cells[c].showCell.updateCell(value);return;}
else{_selectedRow.cells[c].showCell=_tblElement.FieldEditors[c].CellPainter.showCell(_selectedRow.cells[c],c,value);return;}}}}
if(typeof _selectedRow.cells[c].childNodes!='undefined'){for(var i=0;i<_selectedRow.cells[c].childNodes.length;i++){if(_selectedRow.cells[c].childNodes[i].nodeName=='#text'){_selectedRow.cells[c].childNodes[i].nodeValue=value;if(_textBoxElement&&_textBoxElement.cell==_selectedRow.cells[c]){if(_textBoxElement.value!=value){_textBoxElement.value=value;}}
break;}}}}}}}
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
function _getColumnValueByColumnIndex(columnIndex){if(_jsonTable.rowIndex>=0&&_jsonTable.rowIndex<_jsonTable.Rows.length){return _jsonTable.Rows[_jsonTable.rowIndex].ItemArray[columnIndex];}
return null;}
function _getColumnValue(columnName){if(_jsonTable.rowIndex>=0&&_jsonTable.rowIndex<_jsonTable.Rows.length){return _jsonTable.Rows[_jsonTable.rowIndex].ItemArray[_jsonTable.columnIndexes[columnName]];}
return null;}
function _setColumnValueByColumnIndex(columnIndex,val){if(_jsonTable.rowIndex>=0&&_jsonTable.rowIndex<_jsonTable.Rows.length){_jsonTable.Rows[_jsonTable.rowIndex].ItemArray[columnIndex]=val;_jsonTable.Rows[_jsonTable.rowIndex].changed=true;JsonDataBinding.onvaluechanged(_jsonTable,_jsonTable.rowIndex,columnIndex,val);}}
function _setColumnValue(columnName,val){_setColumnValueByColumnIndex(_jsonTable.columnIndexes[columnName],val);}
function _getTableName(){return _jsonTable.TableName;}
function _getModifiedRowCount(){var r0=0;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].changed){r0++;}}
return r0;}
function _getDeletedRowCount(){var r0=0;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].deleted){r0++;}}
return r0;}
function _getNewRowCount(){var r0=0;for(var r=0;r<_jsonTable.Rows.length;r++){if(_jsonTable.Rows[r].added){r0++;}}
return r0;}
_textBoxElement=document.createElement('input');_textBoxElement.id='txt'+_tblElement.id;_textBoxElement.type='text';_textBoxElement.style.position='absolute';_textBoxElement.style.zIndex=100;JsonDataBinding.AttachEvent(_textBoxElement,'onchange',ontextboxvaluechange);document.body.appendChild(_textBoxElement);_textBoxElement.style.display='none';JsonDataBinding.addTextBoxObserver(_textBoxElement);_buttonElement=document.createElement('img');_buttonElement.style.position='absolute';_buttonElement.style.cursor='pointer';_buttonElement.style.zIndex=10;_buttonElement.src='images/dropdownbutton.jpg';document.body.appendChild(_buttonElement);_buttonElement.style.display='none';JsonDataBinding.AttachEvent(_buttonElement,'onclick',oncellbuttonclick);function _onmousedownForEnum(e){if(!_selectionElement)return;var target=getEventSender(e);if(target){while(target){if(_selectionElement==target)return;target=target.parentNode;}}
_selectionElement.style.display='none';}
JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedownForEnum);init();return{getSelectedRowElement:function(){return _selectedRow;},onRowIndexChange:function(name){_onRowIndexChange(name);},oncellvaluechange:function(name,r,c,value){_oncellvaluechange(name,r,c,value);},onDataReady:function(jsData){_onDataReady(jsData);},onRowColorChanged:function(){_onRowColorChanged();},onSelectedRowColorChanged:function(){_onSelectedRowColorChanged();},setEventHandler:function(eventName,func){_setEventHandler(eventName,func);},getColumnValue:function(columnName){return _getColumnValue(columnName);},getColumnValueByColumnIndex:function(columnIndex){return _getColumnValueByColumnIndex(columnIndex);},setColumnValue:function(columnName,val){_setColumnValue(columnName,val);},setColumnValueByColumnIndex:function(columnIndex,val){_setColumnValueByColumnIndex(columnIndex,val);},getTableName:function(){return _getTableName();},clickCell:function(cell){_clickCell(cell);},getModifiedRowCount:function(){return _getModifiedRowCount();},getDeletedRowCount:function(){return _getDeletedRowCount();},getNewRowCount:function(){return _getNewRowCount();}}},HtmlListboxData:function(listElement,jsTable,itemFieldIdx,valueFieldIdx){var _listElement=listElement;var _jsonTable=jsTable
var _itemFieldIdx=itemFieldIdx;var _valueFieldIdx=valueFieldIdx;function init(){JsonDataBinding.AttachOnRowDeleteHandler(_jsonTable.TableName,onrowdelete);recreateListboxElements();}
function onrowdelete(name,r0){if(_jsonTable.TableName==name){var items=_listElement.getElementsByTagName('option');for(var r=0;r<items.length;r++){if(typeof items[r].datarownum!='undefined'){if(items[r].datarownum==r0){_listElement.remove(r);break;}}}}}
function getItem(e){return getEventSender(e);}
function onItemMouseOver(e){var op=getItem(e);if(typeof op!='undefined'){if(_listElement.HighlightRowColor){op.style.backgroundColor=_listElement.HighlightRowColor;}}}
function onItemMouseOut(e){var op=getItem(e);if(typeof op!='undefined'){var bkC;if(typeof _jsonTable.rowIndex!='undefined'&&op.datarownum==_jsonTable.rowIndex){if(_listElement.SelectedRowColor){bkC=_listElement.SelectedRowColor;}
else{bkC=_listElement.style.backgroundColor;}}
else{bkC=_listElement.style.backgroundColor;}
op.style.backgroundColor=bkC;}}
var addingMethod;function addElement(r){var isDeleted=(_jsonTable.Rows[r].deleted||_jsonTable.Rows[r].removed);if(!isDeleted){var text;var val;if(typeof _jsonTable.Rows[r].ItemArray[_itemFieldIdx]!='undefined'&&_jsonTable.Rows[r].ItemArray[_itemFieldIdx]!=null){text=_jsonTable.Rows[r].ItemArray[_itemFieldIdx];}
else{text='';}
if(typeof _jsonTable.Rows[r].ItemArray[_valueFieldIdx]!='undefined'&&_jsonTable.Rows[r].ItemArray[_valueFieldIdx]!=null){val=_jsonTable.Rows[r].ItemArray[_valueFieldIdx];}
else{val='';}
var op=document.createElement('option');op.text=text;op.value=val;op.datarownum=r;if(_jsonTable.rowIndex==r){op.selected=true;}
if(typeof addingMethod=='undefined'){try{_listElement.add(op,null);addingMethod=true;}
catch(e){_listElement.add(op);addingMethod=false;}}
else{if(addingMethod){_listElement.add(op,null);}
else{_listElement.add(op);}}}}
function recreateListboxElements(){_listElement.options.length=0;for(var r=0;r<_jsonTable.Rows.length;r++){addElement(r);}}
function onSelectedIndexChanged(){if(_listElement.selectedIndex>=0){var op=_listElement.options[_listElement.selectedIndex];if(typeof op!='undefined'&&op.datarownum!='undefined'){_jsonTable.rowIndex=op.datarownum;JsonDataBinding.onRowIndexChange(_jsonTable.TableName);}}}
function _onRowIndexChange(name){if(name==_jsonTable.TableName){var r0=_jsonTable.rowIndex;var items=_listElement.getElementsByTagName('option');for(var r=0;r<items.length;r++){if(typeof items[r].datarownum!='undefined'){if(items[r].datarownum==r0){items[r].selected=true;break;}}}}}
function _onDataReady(jsData){_jsonTable=jsData;init();}
init();JsonDataBinding.AttachEvent(_listElement,'onchange',onSelectedIndexChanged);return{onRowIndexChange:function(name){_onRowIndexChange(name);},onDataReady:function(jsData){_onDataReady(jsData);}}},FilesUploader:function(formName,imgWidth,displayElementName,displayImagePath){var _formName=formName;var _width=imgWidth;var fform=document.getElementById(_formName);var _displayElementName=displayElementName;var _displayImagePath=displayImagePath;function createFileInput(){var f=document.createElement("input");f.type='file';f.style.cssText="text-align: right; cursor:pointer; opacity:0.0;filter:alpha(opacity=0);moz-opacity:0; z-index: 1; position: absolute; left:0px; top:0px; width:"+_width+"px;";f.style.zIndex=1;f.onchange=function(event){onFileSelected(event);};f.name=_formName+"[]";fform.appendChild(f);if(IsFireFox()){f.style.left=-140;}}
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
function _removeFile(filePath){if(filePath&&filePath.toLowerCase){filePath=filePath.replace(',','').toLowerCase();var f2=filePath.replace(/^.*\\/,'');var inputs=fform.getElementsByTagName("input");var found=false;for(var i=0;i<inputs.length;i++){if(inputs[i].type.toLowerCase()=="file"){if(inputs[i].style.zIndex==0){var isFile=false;if(IsFireFox()||IsChrome()){var filename=inputs[i].value.replace(/^.*\\/,'').toLowerCase();isFile=(filename==f2);}
else{isFile=(filePath==inputs[i].value.toLowerCase());}
if(isFile){fform.removeChild(inputs[i]);found=true;}}}}
if(found){if(_displayElementName){var sp1=document.getElementById(_displayElementName);sp1.innerHTML='';inputs=fform.getElementsByTagName("input");for(var i=0;i<inputs.length;i++){if(inputs[i].value&&inputs[i].value.length>0){addFileToDisplay(inputs[i].value);}}}}}}
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
function _setCheckedByTexts(texts){for(var r=0;r<tbody.rows.length;r++){var chks=tbody.rows[r].getElementsByTagName("input");chks[0].checked=false;}
if(texts&&texts!=''){var ts=texts.split(',');for(var i=0;i<ts.length;i++){for(var r=0;r<tbody.rows.length;r++){var sps=tbody.rows[r].getElementsByTagName("span");if(sps[0].innerHTML==ts[i]){var chks=tbody.rows[r].getElementsByTagName("input");chks[0].checked=true;break;}}}}}
function _applyTargetdata(){if(_tagetTable.jsData){var s=_tagetTable.jsData.getColumnValueByColumnIndex(_columnIndex);_setCheckedByTexts(s);}}
function _setMessage(msg){spanMsg.innerHTML=msg;}
function _addrow(text,value,checked){var tr=tbody.insertRow(-1);var td=document.createElement('td');tr.appendChild(td);var chk=document.createElement('input');chk.type="checkbox";td.appendChild(chk);var span=document.createElement('span');span.innerHTML=text;td.appendChild(span);var val=document.createElement('input');val.type="hidden";val.value=value;td.appendChild(val);if(checked){chk.checked=true;}}
JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedown);JsonDataBinding.AttachEvent(imgCancel,'onclick',function(){div.style.display='none';});JsonDataBinding.AttachEvent(imgok,'onclick',onOK);var _dataLoaded=false;tbl.chklist={dataLoaded:function(){return _dataLoaded;},getTable:function(){return tbl;},setMessage:function(msg){_setMessage(msg);},setPosition:function(pos){div.style.position=pos;},move:function(x,y){div.style.left=x;div.style.top=y;},getBottom:function(){return div.offsetHeight+div.offsetTop;},setDisplay:function(dis){div.style.display=dis;},setzindex:function(zi){div.style.zIndex=zi;},addrow:function(text,value,checked){_addrow(text,value,checked);},loadData:function(){if(_dataLoaded){_applyTargetdata();}
else{_dataLoaded=true;if(_editor.UseDb){var u=new Object();JsonDataBinding.executeServerMethod(_editor.TableName,u);}
else{for(var i=0;i<_editor.List.length;i++){_addrow(_editor.List[i],i);}
_setMessage("");_applyTargetdata();}}},loadRecords:function(records){for(var i=0;i<records.length;i++){_addrow(records[i].ItemArray[0],i);}
_setMessage("");_applyTargetdata();},setCheckedByTexts:function(texts){_setCheckedByTexts(texts);},applyTargetdata:function(){_applyTargetdata();},showCell:function(td,c,val){var _cell=td;var imgplus;var txt0;var divContents;function plusclick(){if(divContents.style.display=='block'){divContents.style.display='none';imgplus.src='libjs/plus.gif';}
else{divContents.style.display='block';imgplus.src='libjs/minus.gif';}}
if(typeof val=='undefined'||val==null){td.appendChild(document.createTextNode('{null}'));}
else{var s=''+val;if(s.length<20){var txt=document.createTextNode(s);td.appendChild(txt);}
else{var s0=s.substr(0,6)+' ...';var ss=s.replace(new RegExp(',','g'),'<br>');imgplus=document.createElement("img");imgplus.src='libjs/plus.gif';td.appendChild(imgplus);imgplus.style.display='block';imgplus.style.cursor='pointer';imgplus.align='left';txt0=document.createTextNode(s0);td.appendChild(txt0);divContents=document.createElement("div");td.appendChild(divContents);divContents.style.display='none';divContents.innerHTML=ss;divContents.ownerCell=_cell;var clickCell=function(){_tagetTable.jsData.clickCell(_cell);};JsonDataBinding.AttachEvent(imgplus,'onclick',plusclick);}}
return{updateCell:function(val){for(var i=0;i<_cell.childNodes.length;i++){if(_cell.childNodes[i].nodeValue){_cell.childNodes[i].nodeValue='';}}
if(typeof val=='undefined'||val==null){if(imgplus){imgplus.style.display='none';}
if(divContents){divContents.style.display='none';}
if(!txt0){txt0=document.createTextNode('{null}');_cell.appendChild(txt0);}
else{txt0.nodeValue='{null}';}}
else{var s=''+val;if(s.length<20){if(imgplus){imgplus.style.display='none';}
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
return tbl;},DataRepeater:function(divElement,jsTable){var _divElement=divElement;var _jsonTable=jsTable;var _readOnly=false;var _pageIndex=0;var _itemCount=0;var _currentGroupIndex=0;var _navigatorPages=5;var _navigatorStart=-1;var _items;var _pageNumerHolders;var _firstTime=true;function _getTotalPages(){if(_jsonTable){if(_itemCount>0){return Math.ceil(_jsonTable.Rows.length/_itemCount);}}
return 0}
function _showPage(){var pn=_getTotalPages();if(_pageIndex>=0&&_pageIndex<pn){var baseIndex=_pageIndex*_itemCount;for(var g=0;g<_items.length;g++){var r=baseIndex+g;if(r>=0&&r<_jsonTable.Rows.length){_jsonTable.rowIndex=r;JsonDataBinding.bindDataToElement(_items[g],_jsonTable.TableName,_firstTime);_items[g].style.display='block';}
else{_items[g].style.display='none';}}
if(_pageNumerHolders){var nStart=Math.floor(_pageIndex/_navigatorPages)*_navigatorPages;_navigatorStart=nStart;var nEnd=nStart+_navigatorPages;var sh='';for(var g=nStart;g<nEnd&&g<pn;g++){if(g==_pageIndex){sh+="<span style='color:black;'>&nbsp;&nbsp;";sh+=(g+1);sh+="&nbsp;&nbsp;</span>";}
else{sh+="<span style='cursor:pointer;' onclick='";sh+=_divElement.id;sh+=".jsData.gotoPage(";sh+=g;sh+=");'>&nbsp;&nbsp;";sh+=(g+1);sh+="&nbsp;&nbsp;</span>";}}
for(var i=0;i<_pageNumerHolders.length;i++){_pageNumerHolders[i].innerHTML=sh;}}
if(typeof _divElement.onpageIndexChange=='function'){_divElement.onpageIndexChange();}}}
function init(){if(!_items){var id=_divElement.id;_items=new Array();var el=_divElement.getElementsByTagName("div");for(var i=0;i<el.length;i++){if(el[i].name&&el[i].name==id){_items.push(el[i]);}}
_itemCount=_items.length;}
if(!_pageNumerHolders){var id=_divElement.id+'_sp';_pageNumerHolders=new Array();var sps=_divElement.getElementsByTagName("span");for(var i=0;i<sps.length;i++){if(sps[i].name&&sps[i].name==id){_pageNumerHolders.push(sps[i]);}}}
if(_jsonTable){_pageIndex=0;_firstTime=true;_showPage();_firstTime=false;}}
function _groupsPerPage(){return _itemCount;}
function _onPageIndexChange(name){if(name==_jsonTable.TableName){var r0=_jsonTable.rowIndex;}}
function _onDataReady(jsData){_jsonTable=jsData;init();}
function _gotoPage(pageIndex){var pn=_getTotalPages();_pageIndex=pageIndex;if(_pageIndex>=pn-1){_pageIndex=pn-1;}
if(_pageIndex<0){_pageIndex=0;}
if(_pageIndex<pn){_showPage();}}
function _goNextPage(){var pn=_getTotalPages();if(_pageIndex<pn-1){_pageIndex++;_gotoPage(_pageIndex);}}
function _goPrevPage(){if(_pageIndex>0){_pageIndex--;_gotoPage(_pageIndex);}}
function _gotoFirstPage(){if(_pageIndex!=0){_gotoPage(0);}}
function _gotoLastPage(){var pn=_getTotalPages();if(_pageIndex!=pn-1){_gotoPage(pn-1);}}
function _setCurrentGroupIndex(index){_currentGroupIndex=index;}
function _getElement(id){return document.getElementById(id+'_'+_currentGroupIndex);}
function _attachElementEvent(id,evt){if(_items){var e0=document.getElementById(id);for(var g=0;g<_items.length;g++){var el=document.getElementById(id+'_'+g);el[evt]=(function(x){return function(){_currentGroupIndex=x;e0[evt]();};}(g));}}}
function _getPageIndex(){return _pageIndex;}
function _getCurrentGroupIndex(){return _currentGroupIndex;}
function _setNavigatorPages(pnumber){_navigatorPages=pnumber;}
function _getNavigatorPages(){return _navigatorPages;}
init();return{onDataReady:function(jsData){_onDataReady(jsData);},onPageIndexChange:function(name){_onPageIndexChange(name);},groupsPerPage:function(){return _groupsPerPage();},getPageIndex:function(){return _getPageIndex();},getTotalPages:function(){return _getTotalPages();},gotoNextPage:function(){_goNextPage();},gotoPrevPage:function(){_goPrevPage();},gotoFirstPage:function(){_gotoFirstPage();},gotoLastPage:function(){_gotoLastPage();},gotoPage:function(pageIndex){_gotoPage(pageIndex);},setCurrentGroupIndex:function(index){_setCurrentGroupIndex(index);},getCurrentGroupIndex:function(){return _getCurrentGroupIndex();},setNavigatorPages:function(pnumber){_setNavigatorPages(pnumber-1);},getNavigatorPages:function(){_getNavigatorPages();},getElement:function(id){return _getElement(id);},attachElementEvent:function(id,evt){_attachElementEvent(id,evt);}}}}