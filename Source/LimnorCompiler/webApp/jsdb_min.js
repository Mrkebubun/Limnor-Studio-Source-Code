
var JsonDataBinding=JsonDataBinding||{_binder:function(){var jsdb_serverPage=String();var jsdb_bind='jsdb';var jsdb_getdata='jsonDb_getData';var jsdb_putdata='jsonDb_putData';var const_userAlias='logonUserAlias';var e_onchange='onchange';var type_func='function';var sources=new Object();var handlersOnRowIndex=new Object();var onrowdeletehandlers=new Object();var hasActivity=false;var activityWatcher;activity=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(u!=undefined&&u!=null){if(u.length>0){if(hasActivity){hasActivity=false;var uu=u.split(' ');if(uu.length>2){JsonDataBinding.setCookie(const_userAlias,u,uu[2]);}}
activityWatcher=setTimeout(activity,3000);}}}
_setUserLogCookieName=function(nm){const_userAlias=nm;}
_getCurrentUserLevel=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(u!=undefined&&u!=null){if(u.length>0){var uu=u.split(' ');if(uu.length>2){return uu[1];}}}
return 0;}
_getCurrentUserAlias=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(u!=undefined&&u!=null){if(u.length>0){var uu=u.split(' ');if(uu.length>2){return uu[0];}}}
return null;}
var _eventFirer;_setEventFirer=function(eo){_eventFirer=eo;}
_setServerPage=function(pageUrl){jsdb_serverPage=pageUrl;}
_addOnRowIndexChangeHandler=function(tableName,handler){if(handlersOnRowIndex==undefined||handlersOnRowIndex==null){handlersOnRowIndex=new Object();}
if(handlersOnRowIndex[tableName]==undefined){handlersOnRowIndex[tableName]=new Array();}
handlersOnRowIndex[tableName].push(handler);}
_hasLoggedOn=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(u!=undefined&&u!=null){if(u.length>0){var uu=u.split(' ');if(uu.length>2){if(uu[1]<=JsonDataBinding.TargetUserLevel){return true;}}}}
return false;}
_logOff=function(){if(activityWatcher!=undefined&&activityWatcher!=null){clearTimeout(activityWatcher);}
activityWatcher=null;var u=JsonDataBinding.getCookie(const_userAlias);if(u!=undefined&&u!=null){if(u.length>0){JsonDataBinding.eraseCookie(const_userAlias);}}}
_loginPassed=function(login,expire){JsonDataBinding.setCookie(const_userAlias,login+' '+expire,expire);_setupLoginWatcher();}
function addloader(func){var oldonload=window.onload;if(typeof window.onload!='function'){window.onload=func;}else{window.onload=function(){if(oldonload){oldonload();}
func();}}}
function addMouseWatcher(func){var oldonload=document.body.onmousemove;if(typeof document.body.onmousemove!='function'){document.body.onmousemove=func;}
else{document.body.onmousemove=function(){if(oldonload){oldonload();}
func();}}}
function addKeyboardWatcher(func){var oldonload=document.body.onkeydown;if(typeof document.body.onkeydown!='function'){document.body.onkeydown=func;}
else{document.body.onkeydown=function(){if(oldonload){oldonload();}
func();}}}
_setupLoginWatcher=function(){var u=JsonDataBinding.getCookie(const_userAlias);if(u==undefined||u==null){return;}
if(u.length==0){return;}
addKeyboardWatcher(function(){hasActivity=true;});addMouseWatcher(function(){hasActivity=true;});activityWatcher=setTimeout(activity,3000);}
_setDataSource=function(dataName){var v=this;if(v!=undefined&&v.Data!=undefined){v=v.Data;}
if(v!=undefined&&v.Tables!=undefined){for(var i=0;i<v.Tables.length;i++){var name=v.Tables[i].TableName;if(dataName!=undefined&&dataName!=null&&dataName!=''){if(dataName!=name){continue;}}
sources[name]=v.Tables[i];sources[name].columnIndexes=new Object();sources[name].rowIndex=new Number();sources[name].rowIndex=0;for(var j=0;j<sources[name].Columns.length;j++){sources[name].columnIndexes[sources[name].Columns[j].Name]=j;}}
for(var k=0;k<v.Tables.length;k++){if(dataName!=undefined&&dataName!=null&&dataName!=''){if(dataName!=v.Tables[k].TableName){continue;}}
bindTable(v.Tables[k].TableName,true);}}}
function bindData(e,name,firstTime){for(var i=0;i<e.childNodes.length;i++){var a=e.childNodes[i];if(a!=undefined){if(a.getAttribute!=undefined){var bd=a.getAttribute(jsdb_bind);if(bd!=undefined&&bd!=null&&bd!=''){var binds=bd.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var dbTable=bind[0];if(dbTable==name){if(bind.length==1){if(firstTime){if(a.tagName!=undefined){if(a.tagName.toLowerCase()=="table"){if(a.jsData==undefined){a.jsData=JsonDataBinding.HtmlTableData(a,sources[name]);}
else{a.jsData.onDataReady(sources[name]);}}}}
else{if(a.tagName!=undefined){if(a.tagName.toLowerCase()=="table"){if(a.jsData!=undefined){a.jsData.onRowIndexChange(name);}}}}}
else if(bind.length==3){var field=bind[1];var target=bind[2];if(sources[name].rowIndex>=0&&sources[name].rowIndex<sources[name].Rows.length){a[target]=sources[name].Rows[sources[name].rowIndex].ItemArray[sources[name].columnIndexes[field]];}
else{a[target]='';}
if(firstTime){if(isEventSupported(a,e_onchange)){if(a.attachEvent!=undefined){a.attachEvent(e_onchange,changeBoundData);}
else{if(a.addEventListener!=undefined){a.addEventListener(e_onchange,changeBoundData,false);}
else{a.onchange=changeBoundData;}}}}}}}}
else{bindData(a,name,firstTime);}}}}}
function bindTable(name,firstTime){bindData(document.body,name,firstTime);}
function getNextRowIndex(name,currentIndex){var idx2=-1;var idx=currentIndex+1;while(idx<sources[name].Rows.length){if(sources[name].Rows[idx].deleted==undefined){idx2=idx;break;}
if(sources[name].Rows[idx].deleted==false){idx2=idx;break;}
idx++;}
return idx2;}
function getPreviousRowIndex(name,currentIndex){var idx2=-1;var idx=currentIndex-1;while(idx>=0){if(sources[name].Rows[idx].deleted==undefined){idx2=idx;break;}
if(sources[name].Rows[idx].deleted==false){idx2=idx;break;}
idx--;}
return idx2;}
function onRowIndexChange(name){if(sources!=undefined&&sources[name]!=undefined){if(handlersOnRowIndex!=undefined){if(handlersOnRowIndex[name]!=null){for(var i=0;i<handlersOnRowIndex[name].length;i++){handlersOnRowIndex[name][i](sources[name]);}}}}}
_onRowIndexChange=function(name){bindData(document.body,name,false);onRowIndexChange(name);}
_dataMoveFirst=function(name){var idx2=getNextRowIndex(name,-1);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}
_dataMoveLast=function(name){var idx2=getPreviousRowIndex(name,sources[name].Rows.length);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}
_dataMoveNext=function(name){if(sources[name]!=undefined&&sources[name].rowIndex<sources[name].Rows.length-1){var idx2=getNextRowIndex(name,sources[name].rowIndex);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}}
_dataMovePrevious=function(name){if(sources[name]!=undefined&&sources[name].rowIndex<sources[name].Rows.length&&sources[name].rowIndex>0){var idx2=getPreviousRowIndex(name,sources[name].rowIndex);if(idx2>=0){sources[name].rowIndex=idx2;_onRowIndexChange(name);}}}
_columnValue=function(name,columnName,rowIndex){if(sources!=undefined){if(rowIndex==undefined){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){return sources[name].Rows[rowIndex].ItemArray[sources[name].columnIndexes[columnName]];}}
return null;}
_getColExpvalue=function(name,expression,idx){var exp=expression;for(var i=0;i<sources[name].Columns.length;i++){exp=exp.replace(new RegExp("{"+sources[name].Columns[i].Name+"}","gi"),sources[name].Rows[idx].ItemArray[i]);}
return eval(exp);}
_columnExpressionValue=function(name,expression,rowIndex){if(sources!=undefined){if(rowIndex==undefined){rowIndex=sources[name].rowIndex;}
if(rowIndex<sources[name].Rows.length&&rowIndex>=0){return _getColExpvalue(name,expression,rowIndex);}}
return null;}
_statistics=function(name,expression,operator){if(sources!=undefined){if(rowIndex==undefined){rowIndex=sources[name].rowIndex;}
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
if(onrowdeletehandlers[name]!=undefined){for(var i=0;i<onrowdeletehandlers[name].length;i++){onrowdeletehandlers[name][i](name,idx);}}
sources[name].rowIndex=idx2;bindData(document.body,name,false);}}
_addRow=function(name){var r=new Object();r.added=true;r.ItemArray=new Array();for(var i=0;i<sources[name].Columns.length;i++){r.ItemArray[i]='';}
var idx=sources[name].Rows.length;sources[name].Rows[idx]=r;sources[name].rowIndex=idx;bindData(document.body,name,false);}
function preserveKeys(name){var tbl=sources[name];if(tbl.rowIndex>=0&&tbl.rowIndex<tbl.Rows.length){if(tbl.PrimaryKey!=null&&tbl.PrimaryKey.length>0){if(!tbl.Rows[tbl.rowIndex].changed&&!tbl.Rows[tbl.rowIndex].deleted){if(tbl.Rows[tbl.rowIndex].KeyValues==undefined){tbl.Rows[tbl.rowIndex].KeyValues=new Array();for(var k=0;k<tbl.PrimaryKey.length;k++){tbl.Rows[tbl.rowIndex].KeyValues[k]=tbl.Rows[tbl.rowIndex].ItemArray[tbl.columnIndexes[tbl.PrimaryKey[k]]];}}}}}}
function changeBoundData(e){var ev;if(!e)ev=window.event;else ev=e;var a;if(ev.target)a=ev.target;else if(ev.srcElement)a=ev.srcElement;if(a!=undefined){if(a.nodeType==3)
a=a.parentNode;var dbs=a.getAttribute(jsdb_bind);if(dbs!=undefined&&dbs!=null&&dbs!=''){var binds=dbs.split(';');for(var sIdx=0;sIdx<binds.length;sIdx++){var bind=binds[sIdx].split(':');var sourceName=bind[0];var tbl=sources[sourceName];if(tbl!=undefined){var field=bind[1];var target=bind[2];if(target=='value'){if(tbl.rowIndex>=0&&tbl.rowIndex<tbl.Rows.length){preserveKeys(sourceName);var c=tbl.columnIndexes[field];tbl.Rows[tbl.rowIndex].ItemArray[c]=a[target];tbl.Rows[tbl.rowIndex].changed=true;if(tbl.onvaluechange!=undefined){tbl.onvaluechange(tbl.TableName,tbl.rowIndex,c,a[target]);}}
break;}}}}}}
function dataUpdated(name){for(p in sources){var item=sources[p];if(item!=undefined&&item!=null&&typeof(item)!=type_func){if(item.TableName==undefined){continue;}
if(name!=undefined){if(name!=item.TableName){continue;}}
var rows=item.Rows;var i=0;while(i<rows.length){if(rows[i].deleted){rows.splice(i,1);}
else{i++;}}
for(var i=0;i<rows.length;i++){if(rows[i].added){rows[i].added=false;}
if(rows[i].changed){rows[i].changed=false;}}
item.Rows=rows;}}}
_sendBoundData=function(dataName,clientProperties,commands){var req=new Object();req.Calls=new Array();if(commands!=undefined){req.Calls=commands;}
if(clientProperties!=undefined&&clientProperties!=null){req.values=clientProperties;}
req.Data=new Array();var i=0;for(p in sources){var item=sources[p];if(item!=undefined&&item!=null&&typeof(item)!=type_func){if(dataName!=undefined&&dataName!=''&&dataName!=null){if(item.TableName!=dataName){continue;}}
req.Data[i]=new Object();for(n in item){var n0=item[n];if(n0!=undefined&&n0!=null&&typeof(n0)!=type_func){if(n=='Rows'){var rs=n0;var rs2=new Array();var k=0;for(var j=0;j<rs.length;j++){if(rs[j].changed||rs[j].deleted||rs[j].added){if(!(rs[j].deleted&&rs[j].added)){rs2[k++]=rs[j];}}}
req.Data[i][n]=rs2;}
else{req.Data[i][n]=n0;}}}
i++;}}
_callServer(req);}
_submitBoundData=function(){_sendBoundData('',null,[{method:jsdb_putdata,value:'0'}]);}
_putData=function(dataName){_sendBoundData(dataName,null,[{method:jsdb_putdata,value:dataName}]);}
_callServer=function(data){var DEBUG_SYMBOL="F3E767376E6546a8A15D97951C849CE5";var xmlhttp;if(JsonDataBinding.LogonPage.length>0){if(!JsonDataBinding.hasLoggedOn()){var curUrl=JsonDataBinding.getPageFilename();window.location.href=JsonDataBinding.LogonPage+'?'+curUrl;}}
if(window.XMLHttpRequest){xmlhttp=new XMLHttpRequest();}
else{xmlhttp=new ActiveXObject('Microsoft.XMLHTTP');}
if(_eventFirer!=undefined&&_eventFirer!=null){if(_eventFirer.disabled!=undefined){_eventFirer.disabled=true;xmlhttp.JsEventOwner=_eventFirer;}}
xmlhttp.onreadystatechange=function(){if(xmlhttp.readyState==4&&xmlhttp.status==200){var v;var r;r=xmlhttp.responseText;var pos=r.indexOf(DEBUG_SYMBOL);if(pos>=0){var debug=r.substring(0,pos);r=r.substring(pos+DEBUG_SYMBOL.length);var winDebug=window.open("","debugWindows");winDebug.document.write('<h1>Debug Information from ');winDebug.document.write(jsdb_serverPage);winDebug.document.write('</h1>');winDebug.document.write('<h2>Client request</h2><p>');winDebug.document.write(debug);winDebug.document.write('</p>');winDebug.document.write('<h2>Server response</h2><p>');winDebug.document.write(r);winDebug.document.write('</p>');}
if(r!=undefined&&r.length>6){for(var k=0;k<6;k++){if(r.charAt(k)=='{'){r=r.substring(k);break;}}
try{v=r.parseJSON();}
catch(err){var winDebug=window.open("","debugWindows");if(pos<0){winDebug.document.write('<h1>Debug Information from ');winDebug.document.write(jsdb_serverPage);winDebug.document.write('</h1>');winDebug.document.write('<h2>Client request</h2><p>');winDebug.document.write(data.toJSONString());winDebug.document.write('</p>');winDebug.document.write('<h2>Server response</h2><p>');winDebug.document.write(r);winDebug.document.write('</p>');}
winDebug.document.write('<h2>Json exception</h2><p>');winDebug.document.write('<table>');for(var p in err){winDebug.document.write('<tr><td>');winDebug.document.write(p);winDebug.document.write('</td><td>');winDebug.document.write(err[p]);winDebug.document.write('</td></tr>');}
winDebug.document.write('</table>');winDebug.document.write('</p>');}
JsonDataBinding.values=v.values;if(v.Calls!=undefined&&v.Calls.length>0){var cf=function(){for(var i=0;i<v.Calls.length;i++){eval(v.Calls[i]);}}
cf.call(v);}
if(v.Data!=undefined){_setDataSource.call(v.Data);}}
if(xmlhttp.JsEventOwner!=undefined&&xmlhttp.JsEventOwner!=null){if(xmlhttp.JsEventOwner.disabled!=undefined){xmlhttp.JsEventOwner.disabled=false;}}}}
var url=jsdb_serverPage+'?timeStamp='+new Date().getTime();xmlhttp.open('POST',url,true);xmlhttp.setRequestHeader('Content-Type','application/x-www-form-urlencoded');if(JsonDataBinding.Debug!=undefined&&JsonDataBinding.Debug){xmlhttp.send(DEBUG_SYMBOL+data.toJSONString());}
else{xmlhttp.send(data.toJSONString());}}
_executeServerCommands=function(commands,clientProperties,data){if(data!=undefined&&data!=null){if(typeof data=='boolean'){if(data){_sendBoundData('',clientProperties,commands);}
else{var req=new Object();req.Calls=commands;if(clientProperties!=undefined){req.values=clientProperties;}
_callServer(req);}}
else if(typeof data=='string'){_sendBoundData(data,clientProperties,commands);}
else{var req=new Object();req.Calls=commands;if(clientProperties!=undefined){req.values=clientProperties;}
req.Data=data;_callServer(req);}}
else{var req=new Object();req.Calls=commands;if(clientProperties!=undefined){req.values=clientProperties;}
_callServer(req);}}
_getData=function(dataName,clientProperties){var req=new Object();req.Calls=new Array();req.Calls[0]=new Object();req.Calls[0].method=jsdb_getdata;req.Calls[0].value=dataName;if(clientProperties!=undefined){req.values=clientProperties;}
_callServer(req);}
_attachOnRowDeleteHandler=function(name,handler){if(onrowdeletehandlers[name]==undefined){onrowdeletehandlers[name]=new Array();}
var exist=false;for(var i=0;i<onrowdeletehandlers[name].length;i++){if(onrowdeletehandlers[name][i]==handler){exist=true;break;}}
if(!exist){onrowdeletehandlers[name].push(handler);}}
function isEventSupported(el,eventName){var isSupported=(eventName in el);if(!isSupported){el.setAttribute(eventName,'return;');isSupported=typeof el[eventName]==type_func;}
return isSupported;}}(),setServerPage:function(pageUrl){_setServerPage(pageUrl);},getData:function(dataName,clientProperties){_getData(dataName,clientProperties);},putData:function(dataName,clientProperties){_putData(dataName,clientProperties);},callServer:function(commands,clientProperties,data){_executeServerCommands(commands,clientProperties,data);},executeServerMethod:function(command,clientProperties){_executeServerCommands([{method:command,value:'0'}],clientProperties);},addRow:function(dataName){_addRow(dataName);},deleteCurrentRow:function(dataName){_deleteCurrentRow(dataName);},submitBoundData:function(){_submitBoundData();},dataMoveFirst:function(dataName){_dataMoveFirst(dataName);},dataMovePrevious:function(dataName){_dataMovePrevious(dataName);},dataMoveNext:function(dataName){_dataMoveNext(dataName);},dataMoveLast:function(dataName){_dataMoveLast(dataName);},columnValue:function(dataName,columnName,rowIndex){return _columnValue(dataName,columnName,rowIndex);},columnExpressionValue:function(dataName,expression,rowIndex){return _columnExpressionValue(dataName,expression,rowIndex);},statistics:function(dataName,expression,operator){return _statistics(dataName,expression,operator);},addOnRowIndexChangeHandler:function(tableName,handler){_addOnRowIndexChangeHandler(tableName,handler);},onRowIndexChange:function(name){_onRowIndexChange(name);},values:Object,Debug:false,SetEventFirer:function(eo){if(eo.disabled!=undefined){_setEventFirer(eo);}
else{_setEventFirer(null);}},AttachOnRowDeleteHandler:function(name,handler){_attachOnRowDeleteHandler(name,handler);},getPageFilename:function(){return window.location.href.replace(/^.*(\\|\/|\:)/,'');},getCookie:function(name){var nameEQ=name+"=";var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];while(c.charAt(0)==' ')c=c.substring(1,c.length);if(c.indexOf(nameEQ)==0)return c.substring(nameEQ.length,c.length);}
return null;},setCookie:function(name,value,exMinutes){if(exMinutes){var date=new Date();date.setTime(date.getTime()+(exMinutes*60*1000));var expires="; expires="+date.toGMTString();}
else var expires="";document.cookie=name+"="+value+expires+"; path=/";},eraseCookie:function(name){JsonDataBinding.setCookie(name,"",-1);},hasLoggedOn:function(){return _hasLoggedOn();},LoginFailed:function(msgId,msg){var lbl=document.getElementById(msgId);lbl.innerText=msg;},LoginPassed:function(login,expire){_loginPassed(login,expire);var n=window.location.href.indexOf('?');window.location.href=window.location.href.substring(n+1);},LoginPassed2:function(){var n=window.location.href.indexOf('?');window.location.href=window.location.href.substring(n+1);},LogOff:function(){_logOff();},LogonPage:'',setupLoginWatcher:function(){_setupLoginWatcher();},TargetUserLevel:0,GetCurrentUserAlias:function(){return _getCurrentUserAlias();},GetCurrentUserLevel:function(){return _getCurrentUserLevel();},SetLoginCookieName:function(nm){_setUserLogCookieName(nm);},_textBoxObserver:function(){var textBoxes;var timerId;var poll=function(){for(var i=0;i<textBoxes.length;i++){var ctrl=textBoxes[i].box;if(textBoxes[i].val!=ctrl.value){textBoxes[i].val=ctrl.value;if(ctrl.fireEvent){ctrl.fireEvent("onchange");}
else if(document.createEvent&&ctrl.dispatchEvent){var evt=document.createEvent("HTMLEvents");evt.initEvent("change",true,true);ctrl.dispatchEvent(evt);}}}}
AddTextBox=function(textBox){if(textBoxes==undefined){textBoxes=new Array();}
textBoxes.push({box:textBox,val:textBox.valueOf});if(timerId==undefined){timerId=window.setInterval(poll,300);}}
ShowTextBoxCount=function(){if(textBoxes==undefined)
return 0;return textBoxes.length;}}(),addTextBoxObserver:function(textBox){AddTextBox(textBox);},HtmlTableData:function(tableElement,jsTable){var _tblElement=tableElement;var _jsonTable=jsTable;var _existOnvalueChange;var _readOnly=false;var attr=_tblElement.getAttribute('readonly');if(attr!=undefined&&attr){_readOnly=true;}
function init(){if(_jsonTable.onvaluechange!=undefined){_existOnvalueChange=_jsonTable.onvaluechange;}
_jsonTable.onvaluechange=_oncellvaluechange;JsonDataBinding.AttachOnRowDeleteHandler(_jsonTable.TableName,onrowdelete);recreateTableElement();}
var _textBoxElement;var _selectedRow;var _cellcolorhilight='#FFFFCC';var _rowcolorhilight='#CCFFCC';var _rowcolorselection='#CCFFFF';function getCellLocation(element){var x=y=0;if(element.offsetParent){do{x+=element.offsetLeft;y+=element.offsetTop;}while(element=element.offsetParent);}
return{x:x,y:y};}
function getCell(e){var c;if(!e)var e=window.event;if(e.target)c=e.target;else if(e.srcElement)c=e.srcElement;if(c!=undefined){if(c.nodeType==3)
c=c.parentNode;}
return c;}
function addHtmlTableRow(tbody,r){var isDeleted=(_jsonTable.Rows[r].deleted!=undefined&&_jsonTable.Rows[r].deleted);tr=document.createElement('TR');tbody.appendChild(tr);tr.datarownum=r;for(var c=0;c<_jsonTable.Columns.length;c++){var td=document.createElement('TD');td.attachEvent('onmouseover',onCellMouseOver);td.attachEvent('onmouseout',onCellMouseOut);td.attachEvent('onclick',onCellClick);if(_jsonTable.Rows[r].ItemArray[c]!=undefined&&_jsonTable.Rows[r].ItemArray[c]!=null){var txt=document.createTextNode(_jsonTable.Rows[r].ItemArray[c]);if(txt.length==0){txt.nodeValue='{null}';}
td.appendChild(txt);}
else{td.appendChild(document.createTextNode('{null}'));}
td.datarownum=r;td.columnIndex=c;td.tr=tr;tr.appendChild(td);}
if(isDeleted){tr.style.display='none';}
else{tr.style.display='block';}
if(_jsonTable.rowIndex==r){_selectedRow=tr;}}
function getTbody(){if(_tblElement.tBodies!=undefined&&_tblElement.tBodies!=null){for(var i=0;i<_tblElement.tBodies.length;i++){if(_tblElement.tBodies[i].isJs!=undefined){if(_tblElement.tBodies[i].isJs){return _tblElement.tBodies[i];}}}}
return null;}
function recreateTableElement(){_selectedRow=null;_tblElement.deleteTHead();_tblElement.deleteTFoot();while(_tblElement.rows.length>0){_tblElement.deleteRow(_tblElement.rows.length-1);}
var th=document.createElement('THEAD');_tblElement.appendChild(th);var tr=document.createElement('TR');th.appendChild(tr);for(var c=0;c<_jsonTable.Columns.length;c++){var td=document.createElement('TD');td.appendChild(document.createTextNode(_jsonTable.Columns[c].Name));tr.appendChild(td);}
var tbody=getTbody();if(tbody==null){tbody=document.createElement('TBODY');_tblElement.appendChild(tbody);}
tbody.isJs=true;for(var r=0;r<_jsonTable.Rows.length;r++){addHtmlTableRow(tbody,r);}
if(_selectedRow!=null){var cells=_selectedRow.getElementsByTagName("TD");for(var c=0;c<cells.length;c++){cells[c].style.background=_rowcolorselection;}}}
function _onDataReady(jsTable){_jsonTable=jsTable;init();}
function onCellMouseOver(e){var cell=getCell(e);if(cell!=undefined){var cells=cell.tr.getElementsByTagName("TD");for(var c=0;c<cells.length;c++){cells[c].style.background=_rowcolorhilight;}
cell.style.background=_cellcolorhilight;cell.cellLocation=getCellLocation(cell);}}
function onCellMouseOut(e){var cell=getCell(e);if(cell!=undefined){var cells=cell.tr.getElementsByTagName("TD");var bkC;if(_jsonTable.rowIndex!=undefined&&cell.datarownum==_jsonTable.rowIndex){bkC=_rowcolorselection;}
else{bkC=_tblElement.style.background;}
for(var c=0;c<cells.length;c++){cells[c].style.background=bkC;}}}
function oncellvaluechange(){if(_textBoxElement!=undefined){if(_textBoxElement.cell){_textBoxElement.cell.innerText=_textBoxElement.value;_jsonTable.Rows[_textBoxElement.cell.datarownum].changed=true;_jsonTable.Rows[_textBoxElement.cell.datarownum].ItemArray[_textBoxElement.cell.columnIndex]=_textBoxElement.value;}}}
function onCellClick(e){var cell=getCell(e);if(cell!=undefined){_jsonTable.rowIndex=cell.datarownum;if(_textBoxElement!=undefined){var cellReadOnly=false;if(_jsonTable.Columns[cell.columnIndex].ReadOnly!=undefined){cellReadOnly=_jsonTable.Columns[cell.columnIndex].ReadOnly;}
if(cellReadOnly||_readOnly){_textBoxElement.cell=null;_textBoxElement.style.display='none';}
else{_textBoxElement.cell=null;_textBoxElement.value=cell.innerText;_textBoxElement.cell=cell;_textBoxElement.style.display='block';_textBoxElement.focus();_textBoxElement.style.left=cell.cellLocation.x;_textBoxElement.style.top=cell.cellLocation.y;_textBoxElement.style.width=cell.offsetWidth;}}
JsonDataBinding.onRowIndexChange(_jsonTable.TableName);}}
function _onRowIndexChange(name){if(name==_jsonTable.TableName){if(_selectedRow!=undefined){if(_selectedRow.datarownum!=_jsonTable.rowIndex){var cells=_selectedRow.getElementsByTagName("TD");for(var c=0;c<cells.length;c++){cells[c].style.background=_tblElement.style.background;}}}
_selectedRow=null;var rn=_tblElement.rows.length;if(_tblElement.tHead!=null){rn--;}
if(_tblElement.tFoot!=null){rn--;}
if(rn<_jsonTable.Rows.length){var tbody=getTbody();if(tbody!=null){for(var r=rn;r<_jsonTable.Rows.length;r++){addHtmlTableRow(tbody,r);}}}
else{for(var r=0;r<_tblElement.rows.length;r++){if(_tblElement.rows[r].datarownum!=undefined){if(_tblElement.rows[r].datarownum==_jsonTable.rowIndex){_selectedRow=_tblElement.rows[r];if(_textBoxElement!=undefined){if(_textBoxElement.cell!=undefined){if(_textBoxElement.cell.datarownum!=_jsonTable.rowIndex){_textBoxElement.style.display='none';}}}
break;}}}}
if(_selectedRow!=null){var cells=_selectedRow.getElementsByTagName("TD");for(var c=0;c<cells.length;c++){cells[c].style.background=_rowcolorselection;}}}}
_oncellvaluechange=function(name,r,c,value){if(_jsonTable.TableName==name){if(_existOnvalueChange!=undefined){_existOnvalueChange(name,r,c,value);}
if(_selectedRow!=null){if(_selectedRow.datarownum==r){if(_selectedRow.cells[c].childNodes!=undefined){for(var i=0;i<_selectedRow.cells[c].childNodes.length;i++){if(_selectedRow.cells[c].childNodes[i].nodeName=='#text'){_selectedRow.cells[c].childNodes[i].nodeValue=value;break;}}}}}}}
function onrowdelete(name,r0){if(_jsonTable.TableName==name){for(var r=0;r<_tblElement.rows.length;r++){if(_tblElement.rows[r].datarownum!=undefined){if(_tblElement.rows[r].datarownum==r0){_tblElement.rows[r].style.display='none';break;}}}}}
_textBoxElement=document.createElement('input');_textBoxElement.type='text';_textBoxElement.style.position='absolute';_textBoxElement.style.zIndex=_tblElement.style.zIndex+1;_textBoxElement.attachEvent('onchange',oncellvaluechange);document.body.appendChild(_textBoxElement);_textBoxElement.style.display='none';init();return{onRowIndexChange:function(name){_onRowIndexChange(name);},oncellvaluechange:function(name,r,c,value){_oncellvaluechange(name,r,c,value);},onDataReady:function(jsData){_onDataReady(jsData);}}}}