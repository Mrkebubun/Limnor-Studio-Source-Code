// htmlEditorClient.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

var limnorHtmlEditorClient=limnorHtmlEditorClient||{limnorDynaStyleTitle:'dyStyle8831932',pageCommentFrameId:'pcf8831932',getPageStarterNode:function(create){var stData;var ss=document.getElementsByTagName('script');if(ss){for(var i=0;i<ss.length;i++){var s=ss[i].getAttribute('src');if(s){var p=s.lastIndexOf('/');if(p>=0){s=s.substr(p+1);}
p=s.lastIndexOf('\\');if(p>=0){s=s.substr(p+1);}
p=s.indexOf('?');if(p>=0){s=s.substr(0,p);}
s=s.toLowerCase();if(s=='pagestarter.js'){stData=ss[i];break;}}}}
if(stData){stData.setAttribute('hidden','true');}
else if(create){stData=document.createElement('script');stData.setAttribute('type','text/javascript');stData.setAttribute('src','/libjs/pageStarter.js');stData.setAttribute('hidden','true');var head=document.getElementsByTagName('head')[0];head.appendChild(stData);}
return stData;},getDynamicStyleNode:function(){for(var s=0;s<document.styleSheets.length;s++){if(document.styleSheets[s].title==limnorHtmlEditorClient.limnorDynaStyleTitle){return document.styleSheets[s];}}},createDataId:function(baseName,create){var n=1;var nm=baseName+''+n;var ss=document.getElementsByTagName('script');if(ss){while(true){var exists=false;for(var i=0;i<ss.length;i++){var nm0=ss[i].getAttribute('scriptData');if(nm==nm0){exists=true;break;}}
if(exists){n++;nm=baseName+''+n;}
else
break;}}
while(document.getElementById(nm)){n++;nm=baseName+''+n;}
if(create){var stData=document.createElement('script');stData.setAttribute('type','text/javascript');stData.setAttribute('id',nm);stData.setAttribute('hidden','true');var head=document.getElementsByTagName('head')[0];head.appendChild(stData);}
return nm;},addScriptData:function(dataId,data,ownerTag){var stData=document.getElementById(dataId);if(!stData){var head=document.getElementsByTagName('head')[0];stData=document.createElement('script');stData.setAttribute('type','text/javascript');stData.setAttribute('id',dataId);stData.setAttribute('hidden','true');if(ownerTag&&ownerTag.length>0){stData.setAttribute('datatag',ownerTag);}
head.appendChild(stData);}
stData.innerHTML='var limnorData=limnorData||{};limnorData.'+dataId+'='+JSON.stringify(data);return stData;},loadEditor:function(debugMode,inPlace){var editfullfile;var loadingModal;var pageEditor;var _pageId;var _lastActiveElement;var _initialized;var _pageHolder;var editorFiles=new Array();function mousemove(e){e=e||window.event;if(e){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);if(JsonDataBinding.IsChrome()){x-=document.body.scrollLeft;y-=document.body.scrollTop;}
pageEditor.onclientmousemove(x,y,_pageHolder);}
return true;}
function mouseup(e){e=e||window.event;if(e){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);var c=document.elementFromPoint(e.clientX,e.clientY);if(c){var b,pos,tag;for(var i=0;i<c.children.length;i++){tag=c.children[i].tagName.toLowerCase();if(tag=='iframe'||tag=='embed'){if(!b){b=true;JsonDataBinding.windowTools.updateDimensions();}
pos=JsonDataBinding.ElementPosition.getElementPosition(c.children[i]);if(x>=pos.x&&y>=pos.y&&x<=(pos.x+c.children[i].offsetWidth)&&y<=(pos.y+c.children[i].offsetHeight)){c=c.children[i];break;}}}
pageEditor.onclientmouseup(c,_pageHolder);}}
return true;}
var _waitKeyup,_keyupTmID;function _onWaitKeyup(){pageEditor.onclientkeyup();_waitKeyup=false;}
function keyup(e){e=e||window.event;if(e){var code=e.keyCode||e.which||null;if(code){if((code>=37&&code<=40)||code==13||code==8||code==46){if(_waitKeyup){clearTimeout(_keyupTmID);_keyupTmID=setTimeout(_onWaitKeyup,600);}
else{_waitKeyup=true;_keyupTmID=setTimeout(_onWaitKeyup,600);}}}}
return true;}
function keydownPre(e){e=e||event;if(!e){return true;}
var code=e.keyCode||e.which||null;if(code){if(code==8){if(!pageEditor.onclientBackspace()){window.event.returnValue=false;if(e.preventDefault){e.preventDefault();}
return false;}}}
return true;}
function mousedown(e){e=e||window.event;if(e){var c=JsonDataBinding.getSender(e);if(c){pageEditor.onclientmousedown(c,e.button==JsonDataBinding.mouseButtonRight());}}
return true;}
function _setPageEditor(peditor,pageId){pageEditor=peditor;_pageId=pageId;if(pageId){document.body.setAttribute('pageId',pageId);}
JsonDataBinding.AttachEvent(document,"onmousedown",mousedown);JsonDataBinding.AttachEvent(document,"onmouseup",mouseup);JsonDataBinding.AttachEvent(document,"onmousemove",mousemove);JsonDataBinding.AttachEvent(document,"onkeyup",keyup);if(document.addEventListener){document.addEventListener('keydown',keydownPre,true);}
else{if(document.attachEvent){document.attachEvent('onkeydown',keydownPre);}}}
function getFileName(){var sPath=window.location.pathname;var sPage=sPath.substring(sPath.lastIndexOf('/')+1);return sPage;}
function loadjsfile(f){var head=document.getElementsByTagName('head')[0];var script=document.createElement('script');script.type='text/javascript';script.src=f+'?lr='+Math.random();editorFiles.push(script);head.appendChild(script);return script;}
var loadingJDB;var loadingMenubar;var loadingTv;function sstartFileEditing(){if(typeof(JSONIDENTIFIER)=='undefined'){setTimeout(sstartFileEditing,200);return;}
if(typeof(JsonDataBinding)=='undefined'){if(!loadingJDB){loadingJDB=true;loadjsfile('/libjs/jsonDataBind.js');}
setTimeout(sstartFileEditing,200);return;}
if(!inPlace&&(typeof(HtmlEditorMenuBar)=='undefined')){if(!loadingMenubar){loadingMenubar=true;loadjsfile('/libjs/menubar.js');}
setTimeout(sstartFileEditing,100);return;}
if(!inPlace&&(typeof(HtmlEditorTreeview)=='undefined')){if(!loadingTv){loadingTv=true;loadjsfile('/libjs/limnortv.js');}
setTimeout(sstartFileEditing,100);return;}
if(debugMode){JsonDataBinding.Debug=true;}
var head=document.getElementsByTagName('head')[0];for(var i=0;i<editorFiles.length;i++){head.removeChild(editorFiles[i]);}
editorFiles=new Array();limnorHtmlEditorClient.client=createClient();}
function getDocTypeString(){var node=document.doctype;if(node){return'<!DOCTYPE '
+node.name
+(node.publicId?' PUBLIC "'+node.publicId+'"':'')
+(!node.publicId&&node.systemId?' SYSTEM':'')
+(node.systemId?' "'+node.systemId+'"':'')
+'>';}
else{if(document.all&&document.all[0]){if(document.all[0].nodeType==8){return'<!'+document.all[0].nodeValue+'>';}}}}
if(typeof(JSONIDENTIFIER)=='undefined'){loadjsfile('/libjs/json2.js');}
sstartFileEditing();function getLoaderScripts(){return limnorHtmlEditorClient.getPageStarterNode(true);}
function getCssNode(cssFile){var head=document.getElementsByTagName('head')[0];var cssLst=head.getElementsByTagName('link');if(cssLst){var css0=cssFile.toLowerCase();for(var i=0;i<cssLst.length;i++){var s=cssLst[i].getAttribute('href');if(s){var pos=s.indexOf('?');if(pos>=0){s=s.substr(0,pos);}
s=s.toLowerCase();if(css0==s){return cssLst[i];}}}}}
function addCssFile(cssFile,beforeAll){var lnkNode=getCssNode(cssFile);if(!lnkNode){var head=document.getElementsByTagName('head')[0];lnkNode=document.createElement('link');lnkNode.setAttribute('rel','stylesheet');lnkNode.setAttribute('href',cssFile);if(beforeAll){if(head.children.length>0){head.insertBefore(lnkNode,head.children[0]);}
else{head.appendChild(lnkNode);}}
else{head.appendChild(lnkNode);}}}
function addStdJsLisFile(stdlib){var sts=getLoaderScripts();var sl=sts.getAttribute('stdlib');if(!sl){sl=0;}
sl|=stdlib;sts.setAttribute('stdlib',sl);}
function _addCustJsLisFile(custlib,e){if(custlib){custlib=custlib.trim();if(custlib.length>0){var sts=getLoaderScripts();var tryCount=0;function _addfile(){if(typeof(limnorPageLoader)=='undefined'){tryCount++;if(tryCount>20){alert('Timeout loading Javascript');}
else{setTimeout(_addfile,200);return;}}
var fileExists=false;var sl=sts.getAttribute('jsfiles');if(!sl){sl=custlib;}
else{var ss=sl.split(';');for(var i=0;i<ss.length;i++){if(ss[i].trim().toLowerCase()==custlib){fileExists=true;break;}}
if(!fileExists){sl=sl+';'+custlib;}}
if(!fileExists){sts.setAttribute('jsfiles',sl);var jsNode;if(typeof(JsonDataBinding[custlib])=='undefined'){jsNode=loadjsfile('/libjs/'+custlib+'.js');}}
var tryCount=0;function initobj(){if(typeof(JsonDataBinding[custlib])=='undefined'){tryCount++;if(tryCount>30){alert('Timeout loading javascript');}
else{setTimeout(initobj,200);}
return;}
var head=document.getElementsByTagName('head')[0];var scriptNodes=head.getElementsByTagName('script');for(var i=0;i<editorFiles.length;i++){head.removeChild(editorFiles[i]);}
editorFiles=new Array();if(JsonDataBinding[custlib].oninitpage){JsonDataBinding[custlib].oninitpage(e);}}
initobj();}
_addfile();}}}
function removeCustJsLisFile(custlib){if(custlib){custlib=custlib.trim();if(custlib.length>0){var sts=limnorHtmlEditorClient.getPageStarterNode();if(sts){var sl=sts.getAttribute('jsfiles');if(sl){var ss=sl.split(';');var ss2='';for(var i=0;i<ss.length;i++){if(ss[i].trim()!=custlib){if(ss2.length>0){ss2=ss2+';';}
ss2=ss2+ss[i];}}
sts.setAttribute('jsfiles',ss2);}}}}}
function _enableAnchors(){var i;var alst=document.getElementsByTagName('a');if(alst){for(i=0;i<alst.length;i++){if(alst[i].savedhref){alst[i].href=alst[i].savedhref;}
if(alst[i].savedtarget){alst[i].target=alst[i].savedtarget;}}}}
function _enableIFrames(){var i;var alst=document.getElementsByTagName('iframe');if(alst){for(i=0;i<alst.length;i++){if(alst[i].savedpointerEvents){alst[i].style.pointerEvents=alst[i].savedpointerEvents;}
else{alst[i].style.pointerEvents='';}}}}
function _oncleanuppage(){var i;var scripts=document.getElementsByTagName('script');if(scripts){var ss0=[];for(i=0;i<scripts.length;i++){var s=scripts[i].src;if(!s||s.length==0){s=scripts[i].innerHTML;if(!s||s.length==0){ss0.push(scripts[i]);}
else{s=scripts[i].getAttribute('datatag');if(s&&s.length>0){var found;var ows=document.getElementsByTagName(s);if(ows){for(var k=0;k<ows.length;k++){var did=ows[k].getAttribute('scriptData');if(did&&did.length>0){if(did==scripts[i].id){found=true;break;}}}}
if(!found){ss0.push(scripts[i]);}}}}}
for(i=0;i<ss0.length;i++){var p=ss0[i].parentNode;p.removeChild(ss0[i]);}}
scripts=document.getElementsByTagName('style');if(scripts){var ss0=[];for(i=0;i<scripts.length;i++){var s=scripts[i].src;if(!s||s.length==0){s=scripts[i].innerHTML;if(!s||s.length==0){s=scripts[i].getAttribute('title');if(s!='dyStyle8831932'){ss0.push(scripts[i]);}}}}
for(i=0;i<ss0.length;i++){var p=ss0[i].parentNode;p.removeChild(ss0[i]);}}
var stData889923=limnorHtmlEditorClient.getPageStarterNode();if(stData889923){var ss;var fs=stData889923.getAttribute('jsfiles');if(typeof fs!='undefined'&&fs!=null&&fs.length>0){ss=fs.split(';');for(i=0;i<ss.length;i++){var sn=ss[i].trim();if(sn.length>0){if(typeof(JsonDataBinding[sn])!='undefined'){if(JsonDataBinding[sn].oncleanuppage){if(!JsonDataBinding[sn].oncleanuppage()){removeCustJsLisFile(sn);}}}}}}}}
function _plugMouseWatcher(iframe){var triedCount=0;function tryPlugin(){try{var doc=iframe.contentDocument?iframe.contentDocument:(iframe.contentWindow?iframe.contentWindow.document:null);if(doc&&doc.readyState=='complete'){var headNode=doc.getElementsByTagName('head')[0];var scriptNode=doc.createElement('script');scriptNode.setAttribute('type','text/javascript');scriptNode.src='/libjs/iframePlugin.js';headNode.appendChild(scriptNode);}
else{triedCount++;if(triedCount<10){setTimeout(tryPlugin,200);}}}
catch(err){}}
tryPlugin();}
function _disableIFrames(){var i;var alst=document.getElementsByTagName('iframe');if(alst){for(i=0;i<alst.length;i++){alst[i].style.pointerEvents='none';if(JsonDataBinding.IsIE()){_plugMouseWatcher(alst[i]);}}}}
function _disableAnchor(){var i;var alst=document.getElementsByTagName('a');if(alst){var h=JsonDataBinding.getWebSitePath();var hr=window.location.href;var hr1=hr+'#';for(i=0;i<alst.length;i++){var s=alst[i].href;if(s){if(s==hr||s==hr1){s='';}
else if(JsonDataBinding.startsWithI(s,h)){s=s.substr(h.length);}
alst[i].savedhref=s;alst[i].href='#';}
if(alst[i].target){alst[i].savedtarget=alst[i].target;alst[i].target='';}}}}
function createClient(){var _useSavedUrl;var _docType=getDocTypeString();var _defaultCssValues={cursor:'auto',fontFamily:'default',fontSize:'medium',verticalAlign:'baseline',fontStyle:'normal',fontWeight:'normal',fontVariant:'normal',color:'rgb(0,0,0)',direction:'ltr',backgroundImage:'none',backgroundRepeat:'repeat',backgroundPosition:'0% 0%',backgroundColor:'transparent',backgroundAttachment:'scroll',backgroundSize:'auto',backgroundClip:'border-box',backgroundOrigin:'padding-box',borderBottomColor:'rgb(0,0,0)',borderBottomStyle:'default',borderBottomWidth:'medium',borderColor:'rgb(0,0,0)',borderLeftColor:'rgb(0,0,0)',borderLeftStyle:'default',borderLeftWidth:'medium',borderRightColor:'rgb(0,0,0)',borderRightStyle:'default',borderRightWidth:'medium',borderStyle:'default',borderTopColor:'rgb(0,0,0)',borderTopStyle:'default',borderTopWidth:'medium',borderWidth:'medium',outlineColor:'rgb(0,0,0)',outlineStyle:'none',outlineWidth:'medium',borderBottomLeftRadius:'0',borderBottomRightRadius:'0',borderImage:'none',borderRadius:'0',borderTopLeftRadius:'0',borderTopRightRadius:'0',boxShadow:'none',overflowX:'visible',overflowY:'visible',height:'',maxHeight:'none',maxWidth:'none',minHeight:'none',minWidth:'none',width:'',marginBottom:'0',marginLeft:'0',marginRight:'0',marginTop:'0',paddingBottom:'0',paddingLeft:'0',paddingRight:'0',paddingTop:'0',clip:'auto',display:'inline',visibility:'visible',position:'static',clear:'none',overflow:'visible',right:'auto',left:'auto',top:'auto',bottom:'auto',zIndex:'auto',cssFloat:'none',letterSpacing:'normal',lineHeight:'normal',textAlign:'default',textDecoration:'none',textIndent:'0',textTransform:'none',whiteSpace:'normal',wordSpacing:'normal',textAlignLast:'default',textJustify:'auto',textOverflow:'clip',textShadow:'none',wordBreak:'normal',wordWrap:'normal'};var _removedCss;var _removedClassNames;function _saveVirtualCssData(cssId,cssData){var scriptNode;var head=document.getElementsByTagName('head')[0];for(var i=0;i<head.children.length;i++){if(head.children[i].tagName.toLowerCase()=='script'){if(head.children[i].id==cssId){scriptNode=head.children[i];break;}}}
if(!scriptNode){scriptNode=document.createElement('script');scriptNode.setAttribute('id',cssId);scriptNode.setAttribute('type','text/javascript');scriptNode.setAttribute('hidden','true');head.appendChild(scriptNode);}
var txt='var limnorPageData=limnorPageData||{};limnorPageData.'+cssId+'='+
JSON.stringify(cssData)+';';scriptNode.text=txt;}
function _getPageAddress(inEdit){var u=window.location.href;var p=u.indexOf('?');if(p>0){u=u.substr(0,p);}
if(!inEdit&&_useSavedUrl){u=u.substr(0,u.length-14);}
return u;}
function _getPageFilename(inEdit){var u=_getPageAddress(inEdit);return JsonDataBinding.urlToFilename(u);}
function _linkFileExists(filename){filename=filename.toLowerCase();var head=document.getElementsByTagName('head')[0];var links=head.getElementsByTagName('link');if(links){for(i=0;i<links.length;i++){var f=links[i].getAttribute('href');if(f){var pos=f.indexOf('?');if(pos>=0){f=f.substr(0,pos);}
f=f.toLowerCase();if(f==filename){return links[i];}}}}}
function _removedCssFile(filename){filename=filename.toLowerCase();var head=document.getElementsByTagName('head')[0];var links=head.getElementsByTagName('link');if(links){for(i=0;i<links.length;i++){var f=links[i].getAttribute('href');if(f){var pos=f.indexOf('?');if(pos>=0){f=f.substr(0,pos);}
f=f.toLowerCase();if(f==filename){head.removeChild(links[i]);break;}}}}}
function _addcssfile(filename,beforeAll){if(_linkFileExists(filename))
return false;var head=document.getElementsByTagName('head')[0];filename=filename.toLowerCase();if(head.childNodes){var styleNode;var metaNode;var scriptNode;var linkNode;var titleNode;var i;for(i=0;i<head.childNodes.length;i++){if(head.childNodes[i]&&head.childNodes[i].tagName){var tag=head.childNodes[i].tagName.toLowerCase();if(tag=='style'){if(!styleNode)styleNode=head.childNodes[i];}
else if(tag=='meta'){metaNode=head.childNodes[i];}
else if(tag=='script'){if(!scriptNode)scriptNode=head.childNodes[i];}
else if(tag=='link'){if(!linkNode||!beforeAll){linkNode=head.childNodes[i];}}
else if(tag=='title'){titleNode=head.childNodes[i];}}}}
var link=document.createElement('link');link.setAttribute('rel','stylesheet');link.setAttribute('type','text/css');link.setAttribute('hidden','true');link.setAttribute('href',filename);if(linkNode){if(beforeAll)
head.insertBefore(link,linkNode);else
head.insertBefore(link,linkNode.nextSibling);}
else if(styleNode){head.insertBefore(link,styleNode);}
else if(scriptNode){head.insertBefore(link,scriptNode);}
else if(metaNode){head.insertBefore(link,metaNode.nextSibling);}
else if(titleNode){head.insertBefore(link,titleNode.nextSibling);}
else{head.appendChild(link);}
return true;}
function _getPageCssFilename(){var pageCssFilename=_getPageFilename(true);if(JsonDataBinding.endsWithI(pageCssFilename,'.html')){pageCssFilename=pageCssFilename.substr(0,pageCssFilename.length-5);}
pageCssFilename=pageCssFilename.toLowerCase()+'.css';return pageCssFilename;}
function _getPageCssLinkNode(){var pageCssFilename=_getPageCssFilename();_addcssfile(pageCssFilename,false);return _linkFileExists(pageCssFilename);}
function _getPageCssStyle(){var pageCssFilename=_getPageCssFilename();var stls=document.styleSheets;if(stls){pageCssFilename=pageCssFilename.toLowerCase();for(i=0;i<stls.length;i++){var f=stls[i].href;if(f){f=JsonDataBinding.urlToFilename(f);var pos=f.indexOf('?');if(pos>=0){f=f.substr(0,pos);}
if(f.toLowerCase()==pageCssFilename){return stls[i];}}}}}
function _getDynamicStyleNode(){var dyStyleNode;var stls=document.styleSheets;if(stls){for(i=0;i<stls.length;i++){if(stls[i].title==limnorHtmlEditorClient.limnorDynaStyleTitle){dyStyleNode=stls[i];break;}}}
if(!dyStyleNode){var head=document.getElementsByTagName('head')[0];dyStyleNode=document.createElement('style');dyStyleNode.type='text/css';dyStyleNode.title=limnorHtmlEditorClient.limnorDynaStyleTitle;var pageCssLink=_getPageCssLinkNode();head.insertBefore(dyStyleNode,pageCssLink.nextSibling);if(!window.createPopup){dyStyleNode.appendChild(document.createTextNode(''));}
stls=document.styleSheets;if(stls){for(i=0;i<stls.length;i++){if(stls[i].title==limnorHtmlEditorClient.limnorDynaStyleTitle){dyStyleNode=stls[i];break;}}}}
return dyStyleNode;}
function removeColumnProperty(s,name){s=pageEditor.removePropertyFromCssText(s,'-webkit-column-'+name);s=pageEditor.removePropertyFromCssText(s,'column-'+name);s=pageEditor.removePropertyFromCssText(s,'-moz-column-'+name);var pos=s.indexOf('{');var selector=s.substr(0,pos).trim();addCssPropToRemovedList(selector,'-webkit-column-'+name);addCssPropToRemovedList(selector,'column-'+name);addCssPropToRemovedList(selector,'-moz-column-'+name);return s;}
function processColumnProperty(s,name){var nm1='-webkit-column-'+name+':';var nm2='column-'+name+':';var nm3='-moz-column-'+name+':';var pos=s.indexOf(nm1);if(pos>0){var pos2=s.indexOf(';',pos);pos+=nm1.length;v=s.substr(pos,pos2-pos);v2=nm2+v+';'+nm3+v+';';s=s.substr(0,pos2+1)+v2+s.substr(pos2+1);}
else{pos=s.indexOf(nm3);if(pos>0){pos2=s.indexOf(';',pos);pos+=nm3.length;v=s.substr(pos,pos2-pos);v2=nm2+v+';'+nm1+v+';';s=s.substr(0,pos2+1)+v2+s.substr(pos2+1);}
else{pos=s.indexOf(nm2);if(pos>0){pos2=s.indexOf(';',pos);pos+=nm2.length;v=s.substr(pos,pos2-pos);v2=nm3+v+';'+nm1+v+';';s=s.substr(0,pos2+1)+v2+s.substr(pos2+1);}}}
return s;}
function processColumnProperties(s){s=processColumnProperty(s,'count');s=processColumnProperty(s,'width');s=processColumnProperty(s,'gap');if(s.indexOf('column-rule:')>0){s=removeColumnProperty(s,'rule-style');s=removeColumnProperty(s,'rule-width');s=removeColumnProperty(s,'rule-color');s=processColumnProperty(s,'rule');}
else{s=removeColumnProperty(s,'rule');s=processColumnProperty(s,'rule-style');s=processColumnProperty(s,'rule-width');s=processColumnProperty(s,'rule-color');}
return s;}
function _getDynamicCssText(client){var c;var dyStyleNode=_getDynamicStyleNode();if(dyStyleNode){HtmlEditorMenuBar.onBeforeSave(dyStyleNode,client);var rs,pos,pos2,v,v2,pos3;if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}
if(rs){var host=window.location.protocol+'//'+window.location.host;host=host.toLowerCase();c='';for(var r=0;r<rs.length;r++){var s=rs[r].cssText;if(s){s=s.replace(host,'');pos=s.indexOf('linear-gradient');if(pos>0){s=s.replace('-moz-linear-gradient(center ','-moz-linear-gradient(');s=s.replace(' repeat scroll 0% 0% transparent','');pos=s.indexOf('(',pos);if(pos>0){pos2=s.indexOf(';',pos);v=s.substr(pos,pos2-pos);var posLast=v.lastIndexOf(')');if(posLast>0){v=v.substr(0,posLast+1);}
v2='';if(s.indexOf('-ms-linear-gradient')<0){v2=v2+'background-image: -ms-linear-gradient'+v+';';}
if(s.indexOf('-o-linear-gradient')<0){v2=v2+'background-image: -o-linear-gradient'+v+';';}
if(s.indexOf('-moz-linear-gradient')<0){v2=v2+'background-image: -moz-linear-gradient'+v+';';}
if(s.indexOf('-webkit-linear-gradient')<0){v2=v2+'background-image: -webkit-linear-gradient'+v+';';}
s=s.substr(0,pos2+1)+v2+s.substr(pos2+1);}}
else{s=processColumnProperties(s);}
c=c+' '+s;}}}}
if(_removedCss&&_removedCss.length>0){c=JSON.stringify(_removedCss)+'$$$'+(c?c:'');}
return c;}
function _getDefaultCssValue(jsName){return _defaultCssValues[jsName];}
function _formCssValueStr(jsName,value){if(jsName){var vDef;if(value){vDef=_getDefaultCssValue(jsName);if(vDef==value+''){return'';}
if(typeof value.length!='undefined'){if(value.length>0){if(value.substr(value.length-1,1)==';')
return value;else
return value+';';}
else{return'';}}
else{return value+';';}}
else{if(typeof value=='undefined'||value==null){return'';}
else{vDef=_getDefaultCssValue(jsName);if(vDef==value+''){return'';}
if(typeof value.length!='undefined'&&value.length==0){return'';}
else{return value+';';}}}}
else{return value;}}
function _getStyleValue(selectors,cssName){var v;var p1;function processvalue(){if(cssName=='box-shadow'){if(v.length>0){p1=-1;v=v.trim();if(JsonDataBinding.startsWith(v,'rgb')){p1=v.indexOf(')');}
else{var vn=v.charAt(0);if(!JsonDataBinding.isNumber(vn)){p1=v.indexOf(' ');}}
if(p1>0){v=v.substr(p1+1).trim()+' '+v.substr(0,p1);}}}
return v;}
function alternative(){if(cssName=='border-style'||cssName=='border-color'||cssName=='border-width'){v=_getStyleValue(selectors,'border');if(typeof v!='undefined'&&v!=null){p1=v.indexOf(' ');if(p1>0){if(cssName=='border-width'){return v.substr(0,p1).trim();}
v=v.substr(p1+1).trim();p1=v.indexOf(' ');if(p1>0){if(cssName=='border-style'){return v.substr(0,p1).trim();}
return v.substr(p1+1).trim();}}}}
if(cssName=='border-radius'){var v1=_getStyleValue(selectors,'border-top-left-radius');if(typeof v1!='undefined'&&v1!=null){var v2=_getStyleValue(selectors,'border-top-right-radius');if(typeof v2!='undefined'&&v2!=null){var v3=_getStyleValue(selectors,'border-bottom-right-radius');if(typeof v3!='undefined'&&v3!=null){var v4=_getStyleValue(selectors,'border-bottom-left-radius');if(typeof v4!='undefined'&&v4!=null)
return v1+' '+v2+' '+v3+' '+v4;else
return v1+' '+v2+' '+v3;}
else{return v1+' '+v2;}}
else
return v1;}}
if(cssName=='border-style'){return _getStyleValue(selectors,'border-top-style');}
else if(cssName=='border-color'){return _getStyleValue(selectors,'border-top-color');}
else if(cssName=='border-width'){return _getStyleValue(selectors,'border-top-width');}
else if(cssName=='outline-style'||cssName=='outline-color'||cssName=='outline-width'){v=_getStyleValue(selectors,'outline');if(typeof v!='undefined'&&v!=null){if(JsonDataBinding.startsWith(v,'rgb')){p1=v.indexOf(')');if(p1>0){if(cssName=='outline-color')
return v.substr(0,p1);v=v.substr(p1+1).trim();p1=v.indexOf(' ');if(p1>0){if(cssName=='outline-style')
return v.substr(0,p1).trim();else
return v.substr(p1+1).trim();}}}
else{p1=v.indexOf(' ');if(p1>0){if(cssName=='outline-width'){return v.substr(0,p1).trim();}
v=v.substr(p1+1).trim();p1=v.indexOf(' ');if(p1>0){if(cssName=='outline-style'){return v.substr(0,p1).trim();}
return v.substr(p1+1).trim();}}}}}}
var dyStyleNode=_getDynamicStyleNode();var rs,r,i;if(dyStyleNode){if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}}
if(rs){for(r=0;r<rs.length;r++){for(i=0;i<selectors.length;i++){if(rs[r].selectorText==selectors[i]){v=_getCssProperty(rs[r].cssText,cssName);if(typeof v=='undefined'){return alternative();}
else{return processvalue();}}}}}
dyStyleNode=_getPageCssStyle();if(dyStyleNode){if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}}
if(rs){for(r=0;r<rs.length;r++){for(i=0;i<selectors.length;i++){if(rs[r].selectorText==selectors[i]){v=_getCssProperty(rs[r].cssText,cssName);if(typeof v=='undefined'){return alternative();}
else{return processvalue();}}}}}
return alternative();}
function _getLinearGradient(selector){var s=_getStyleValue([selector],'background-image');if(s&&s.length>0){var pos=s.indexOf('-ms-linear-gradient');if(pos<0){pos=s.indexOf('-o-linear-gradient');if(pos<0){pos=s.indexOf('-moz-linear-gradient');if(pos<0){pos=s.indexOf('-webkit-linear-gradient');if(pos<0){pos=s.indexOf('linear-gradient');}}}}
if(pos>=0){pos=s.indexOf('(',pos);if(pos>0){var pos2=s.lastIndexOf(')');if(pos2>pos+2){s=s.substr(pos+1,pos2-pos-1);return s;}}}}}
function _setLinearGradient(selector,val,a){_updateDynamicStyle(selector,val,'backgroundImage','background-image');}
function _updateLinearGradient(obj,selector,angle,startcolor,endcolor){var a=angle.substr(0,angle.length-3);if(JsonDataBinding.isNumber(a)){var v=angle+', '+startcolor+', '+endcolor;if(JsonDataBinding.IsChrome()||JsonDataBinding.IsSafari()){_setLinearGradient(selector,'-webkit-linear-gradient('+v+');');}
else if(JsonDataBinding.IsFireFox()){_setLinearGradient(selector,'-moz-linear-gradient('+v+');');}
else if(JsonDataBinding.IsOpera()){_setLinearGradient(selector,'-o-linear-gradient('+v+');');}
else if(JsonDataBinding.IsIE()){_setLinearGradient(selector,'-ms-linear-gradient('+v+');');}}}
function _setLinearGradientStartColor(obj,selector,val){var endcolor=_getLinearGradientEndColor(selector);var angle=_getLinearGradientAngle(selector);if(typeof(angle)=='undefined'){angle='0deg';}
else{angle=angle+'';if(angle.length==0){angle='0deg';}
else{if(!JsonDataBinding.endsWithI(angle,'deg')){angle=angle+'deg';}}}
if(typeof endcolor=='undefined'||endcolor=='rgb(255, 255, 255)'){if(val==null){_setLinearGradient(selector,null);return;}
if(typeof endcolor=='undefined'){endcolor='#FFF';}}
else{if(val==null){val='#FFF';}}
_updateLinearGradient(obj,selector,angle,val,endcolor);}
function _setLinearGradientEndColor(obj,selector,val){var startcolor=_getLinearGradientStartColor(selector);var angle=_getLinearGradientAngle(selector);if(typeof(angle)=='undefined'){angle='0deg';}
else{angle=angle+'';if(angle.length==0){angle='0deg';}
else{if(!JsonDataBinding.endsWithI(angle,'deg')){angle=angle+'deg';}}}
if(typeof startcolor=='undefined'||startcolor=='rgb(255, 255, 255)'){if(val==null){_setLinearGradient(selector,null);return;}
if(typeof startcolor=='undefined'){startcolor='#FFF';}}
else{if(val==null){val='#FFF';}}
_updateLinearGradient(obj,selector,angle,startcolor,val);}
function _setLinearGradientAngle(obj,selector,val){var startcolor=_getLinearGradientStartColor(selector);var endcolor=_getLinearGradientEndColor(selector);if(typeof(val)=='undefined'){val='0deg';}
else{val=val+'';if(val.length==0){val='0deg';}
else{if(!JsonDataBinding.endsWithI(val,'deg')){val=val+'deg';}}}
if(typeof startcolor=='undefined'){startcolor='#FFF';}
if(typeof endcolor=='undefined'){endcolor='#FFF';}
_updateLinearGradient(obj,selector,val,startcolor,endcolor);}
function _getLinearGradientAngle(selector){var s=_getLinearGradient(selector);if(s){var pos=s.indexOf(',');if(pos>0){s=s.substr(0,pos).trim();if(JsonDataBinding.endsWithI(s,'deg')){s=s.substr(0,s.length-3);}
return s;}}}
function _getLinearGradientStartColor(selector){var s=_getLinearGradient(selector);if(s){var pos=s.indexOf(',');if(pos>=0){s=s.substr(pos+1).trim();if(s.charAt(0)=='#'){pos=s.indexOf(',');if(pos>0){s=s.substr(0,pos).trim();return s;}}
else{if(s.substr(0,4)=='rgb('){pos=s.indexOf(')');if(pos>0){s=s.substr(0,pos+1).trim();return s;}}}}}}
function _getLinearGradientEndColor(selector){var s=_getLinearGradient(selector);if(s){var pos=s.indexOf(',');if(pos>=0){var found=false;s=s.substr(pos+1).trim();if(s.charAt(0)=='#'){pos=s.indexOf(',');if(pos>0){s=s.substr(pos+1).trim();found=true;}}
else{if(s.substr(0,4)=='rgb('){pos=s.indexOf(')');if(pos>0){pos=s.indexOf(',',pos);if(pos>0){s=s.substr(pos+1).trim();found=true;}}}}
if(found){if(s.charAt(0)=='#'){pos=s.indexOf(',');if(pos>0){s=s.substr(0,pos).trim();return s;}}
else{if(s.substr(0,4)=='rgb('){pos=s.indexOf(')');if(pos>0){s=s.substr(0,pos+1).trim();return s;}}}}}}}
function addCssPropToRemovedList(selector,cssName){var sel;if(!_removedCss){_removedCss=[];}
for(i=0;i<_removedCss.length;i++){if(_removedCss[i].selector==selector){sel=_removedCss[i];break;}}
if(!sel){sel={selector:selector,props:[]};_removedCss.push(sel);}
if(!sel.all){var exist=false;if(sel.props){for(i=0;i<sel.props.length;i++){if(sel.props[i]==cssName){exist=true;break;}}}
if(!exist){if(!sel.props){sel.props=new Array();}
sel.props.push(cssName);}}}
function _updateDynamicStyle(selector,value,name,cssName,selector2){var dyStyleNode=_getDynamicStyleNode();var rs;var found=false;if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}
var v=_formCssValueStr(name,value);var idx=0;if(rs){idx=rs.length;for(var r=0;r<rs.length;r++){if(rs[r].selectorText==selector||rs[r].selectorText==selector2){if(name){var v0;if(v&&v.length>0&&v.substr(v.length-1,1)==';'){v0=v.substr(0,v.length-1);}
else if(!v||(v&&v.length==0)){v0=_getDefaultCssValue(name);}
else{v0=v;}
rs[r].style[name]=v0;if(name=='columnCount'){rs[r].style['webkitColumnCount']=v0;rs[r].style['mozColumnCount']=v0;}
else if(name=='columnWidth'){rs[r].style['webkitColumnWidth']=v0;rs[r].style['mozColumnWidth']=v0;}
else if(name=='columnGap'){rs[r].style['webkitColumnGap']=v0;rs[r].style['mozColumnGap']=v0;}
else if(name=='columnRuleStyle'){rs[r].style['webkitColumnRuleStyle']=v0;rs[r].style['mozColumnRuleStyle']=v0;}
else if(name=='columnRuleWidth'){rs[r].style['webkitColumnRuleWidth']=v0;rs[r].style['mozColumnRuleWidth']=v0;rs[r].style['MozColumnRuleWidth']=v0;}
else if(name=='columnRuleColor'){rs[r].style['webkitColumnRuleColor']=v0;rs[r].style['mozColumnRuleColor']=v0;}
found=true;}
else{idx=r;}
break;}}}
var sel,i,fullText,pos,c;var selectorToMerge=selector;if(!found){if(selector2){var pageCss=_getPageCssStyle();var prs;if(pageCss){if(pageCss.cssRules){prs=pageCss.cssRules;}
else if(pageCss.rules){prs=pageCss.rules;}}
if(prs){for(i=0;i<prs.length;i++){if(prs[i].selectorText==selector2){selectorToMerge=selector2;break;}}}}}
if(value!=null&&v.length>0){if(!found){if(cssName){fullText=selectorToMerge+' {'+cssName+':'+v+'}';}
else{fullText=value;}
if(dyStyleNode.insertRule){dyStyleNode.insertRule(fullText,idx);}else if(dyStyleNode.addRule){pos=fullText.indexOf('{');c=fullText.substr(pos+1);c=c.substr(0,c.length-1);dyStyleNode.addRule(selectorToMerge,c,idx);}}
if(cssName){if(_removedCss){for(i=0;i<_removedCss.length;i++){if(_removedCss[i].selector==selector){sel=_removedCss[i];break;}}
if(sel){for(i=0;i<sel.props.length;i++){if(sel.props[i]==cssName){sel.props.splice(i,1);break;}}}
if(selector2){for(i=0;i<_removedCss.length;i++){if(_removedCss[i].selector==selector2){sel=_removedCss[i];break;}}
if(sel){for(i=0;i<sel.props.length;i++){if(sel.props[i]==cssName){sel.props.splice(i,1);break;}}}}}}}
else{if(cssName){addCssPropToRemovedList(selector,cssName);addCssPropToRemovedList(selector2,cssName);if(!found){v=_getDefaultCssValue(name);fullText=selectorToMerge+' {'+cssName+':'+v+';}';if(dyStyleNode.insertRule){dyStyleNode.insertRule(fullText,idx);}else if(dyStyleNode.addRule){pos=fullText.indexOf('{');c=fullText.substr(pos+1);c=c.substr(0,c.length-1);dyStyleNode.addRule(selectorToMerge,c,idx);}}}}
if(cssName=='border-style'){if(v==null||v==''||v=='none'||v=='none;'||(!JsonDataBinding.IsOpera()&&!JsonDataBinding.IsSafari())){addCssPropToRemovedList(selector,'border-top-style');addCssPropToRemovedList(selector,'border-left-style');addCssPropToRemovedList(selector,'border-right-style');addCssPropToRemovedList(selector,'border-bottom-style');}
if(v==null||v==''||v=='none'||v=='none;'){addCssPropToRemovedList(selector,'border-style');}}
else if(cssName=='border-color'){var isEmpty=(v==null||v==''||v=='none'||v=='none;');if(!isEmpty){v=v.replace(' ','');isEmpty=(v=='rgb(0,0,0)'||v=='rgb(0,0,0);');}
if(isEmpty||(!JsonDataBinding.IsOpera()&&!JsonDataBinding.IsSafari())){addCssPropToRemovedList(selector,'border-top-color');addCssPropToRemovedList(selector,'border-left-color');addCssPropToRemovedList(selector,'border-right-color');addCssPropToRemovedList(selector,'border-bottom-color');}
if(isEmpty){addCssPropToRemovedList(selector,'border-color');}}
else if(cssName=='border-width'){if(v==null||v==''||v=='none'||v=='none;'||(!JsonDataBinding.IsOpera()&&!JsonDataBinding.IsSafari())){addCssPropToRemovedList(selector,'border-top-width');addCssPropToRemovedList(selector,'border-left-width');addCssPropToRemovedList(selector,'border-right-width');addCssPropToRemovedList(selector,'border-bottom-width');}
if(v==null||v==''||v=='none'||v=='none;'){addCssPropToRemovedList(selector,'border-width');}
else{if(JsonDataBinding.IsOpera()||JsonDataBinding.IsSafari()){addCssPropToRemovedList(selector,'border-width');}}}}
function _getPropertyNameFromCssName(cssName){if(cssName&&cssName.length>0){if(cssName=='float')
return'cssFloat';return cssName.replace(/\W+(.)/g,function(x,chr){return chr.toUpperCase();})}}
function _getElementStyleSetting(e,cssName,jsName){if(e&&e.subEditor){e=e.obj;}
var v=e.style[jsName];if(typeof v=='undefined'||v==null||v.length==0){var sel=_getElementSelector(e);v=_getStyleValue([sel.selector],cssName);}
return v;}
function _getElementStyleValue(e,cssName){if(e&&e.subEditor){e=e.obj;}
var st;if(window.getComputedStyle){st=window.getComputedStyle(e);if(st&&st.getPropertyValue){return st.getPropertyValue(cssName);}}
if(document.defaultView&&document.defaultView.getComputedStyle){st=document.defaultView.getComputedStyle(e);if(st&&st.getPropertyValue){return st.getPropertyValue(cssName);}}
if(e.currentStyle){if(e.currentStyle.getPropertyValue)
return e.currentStyle.getPropertyValue(cssName);else{return e.currentStyle[_getPropertyNameFromCssName(cssName)];}}}
function _addClass(e,cn){if(cn){if(e.className){var mc=e.className.split(' ');var found=false;for(var i=0;i<mc.length;i++){if(mc[i]==cn){found=true;break;}}
if(!found){e.className+=(' '+cn);}}
else{e.className=cn;}}}
function _removeClass(e,cn){if(cn){if(e.className){if(e.className==cn){e.className='';}
else{var m=JsonDataBinding.replaceAll(e.className,' '+cn+' ',' ',true);if(JsonDataBinding.startsWithI(m,cn+' ')){m=m.substr(cn.length+1);m=m.trim();}
if(JsonDataBinding.endsWithI(m,' '+cn)){m=m.substr(0,m.length-cn.length);m=m.trim();}
e.className=m;}}}}
function _getElementSelector(e){var tag=e.tagName.toLowerCase();var stylename=e.getAttribute('styleName');var ruleName=tag+((typeof stylename!='undefined'&&stylename!=null&&stylename.length>0)?('.'+stylename):'');if(tag=='col'||tag=='td'){var tbl=e.parentNode;while(tbl&&tbl.tagName&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
if(tbl){var tblStyle=tbl.getAttribute('styleName');if(typeof tblStyle!='undefined'&&tblStyle!=null&&tblStyle.length>0){ruleName='table.'+tblStyle+' '+ruleName;}}}
return{selector:ruleName,styelName:stylename};}
function _setElementStyleValue(e,name,cssName,value){var sel=_getElementSelector(e);_updateDynamicStyle(sel.selector,value,name,cssName);if(name&&cssName){if(e.style.removeProperty){e.style.removeProperty(cssName);}
else{e.style.removeAttribute(name);}}
_addClass(e,sel.styleName);}
function _getElementClassNamesBy(styleNode,pref){var ret=[];var rs;if(styleNode){if(styleNode.cssRules){rs=styleNode.cssRules;}
else if(styleNode.rules){rs=styleNode.rules;}}
if(rs){for(var r=0;r<rs.length;r++){if(rs[r].selectorText&&rs[r].selectorText.length>0){if(JsonDataBinding.startsWithI(rs[r].selectorText,pref)){var sn=rs[r].selectorText.substr(pref.length);sn=sn.trim();ret.push(sn);}}}}
return ret;}
function _getStyleNames(e){var ret=new Array();ret.push({text:'',value:''});if(e&&e.tagName){var exist,n,k;var tag=e.tagName.toLowerCase();var es=document.getElementsByTagName(e.tagName);if(es){for(n=0;n<es.length;n++){var nm=es[n].getAttribute('styleName');if(nm&&nm.length>0){exist=false;for(k=0;k<ret.length;k++){if(ret[k].text==nm){exist=true;break;}}
if(!exist){ret.push({text:nm,value:nm});}}}}
var pref=tag+'.';var stls=document.styleSheets;if(stls){var names,i;for(n=0;n<stls.length;n++){names=_getElementClassNamesBy(stls[n],pref);for(i=0;i<names.length;i++){if(names[i]&&names[i].length>0){var exist=false;for(k=0;k<ret.length;k++){if(ret[k].text==names[i]){exist=true;break;}}
if(!exist){ret.push({text:names[i],value:names[i]});}}}}}}
return ret;}
function _getClassNames(e){var ret=new Array();ret.push({text:'',value:''});if(e&&e.tagName){var tag=e.tagName.toLowerCase();var pref=tag+'.';var stls=document.styleSheets;if(stls){for(i=0;i<stls.length;i++){var names=_getElementClassNamesBy(stls[i],pref);for(var m=0;m<names.length;m++){var exist=false;for(var k=0;k<ret.length;k++){if(ret[k].text==names[m]){exist=true;break;}}
if(!exist){ret.push({text:names[m],value:names[m]});}}}}}
return ret;}
function _getPagesClasses(){var rs;var ret=new Array();var dyStyleNode=_getDynamicStyleNode();var lnkNode=_getPageCssStyle();var names,i,r;if(dyStyleNode){if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}
if(rs){for(r=0;r<rs.length;r++){ret.push({selector:rs[r].selectorText,cssText:rs[r].cssText});}}}
if(lnkNode){if(lnkNode.cssRules){rs=lnkNode.cssRules;}
else if(lnkNode.rules){rs=lnkNode.rules;}
else
rs=null;if(rs){for(r=0;r<rs.length;r++){var exist=false;for(var k=0;k<ret.length;k++){if(ret[k].selector==rs[r].selectorText){exist=true;break;}}
if(!exist){ret.push({selector:rs[r].selectorText,cssText:rs[r].cssText});}}}}
return ret;}
function _removePageStyleBySelector(selector){var rs,r;var dyStyleNode=_getDynamicStyleNode();if(dyStyleNode){if(dyStyleNode.cssRules){rs=dyStyleNode.cssRules;}
else if(dyStyleNode.rules){rs=dyStyleNode.rules;}
if(rs){for(r=0;r<rs.length;r++){if(rs[r].selectorText==selector){rs[r].cssText='';if(dyStyleNode.deleteRule){dyStyleNode.deleteRule(r);}
else if(dyStyleNode.removeRule){dyStyleNode.removeRule(r);}
break;}}}}
var lnkNode=_getPageCssStyle();if(lnkNode){if(lnkNode.cssRules){rs=lnkNode.cssRules;}
else if(lnkNode.rules){rs=lnkNode.rules;}
else
rs=null;if(rs){for(r=0;r<rs.length;r++){if(rs[r].selectorText==selector){rs[r].cssText='';if(lnkNode.deleteRule){lnkNode.deleteRule(r);}
else if(lnkNode.removeRule){lnkNode.removeRule(r);}
break;}}}}
_addToRemoveCssBySelector(selector);}
function _addToRemoveCssBySelector(selector){if(!_removedCss){_removedCss=[];}
var sel;for(var i=0;i<_removedCss.length;i++){if(_removedCss[i].selector==selector){sel=_removedCss[i];sel.all=true;break;}}
if(!sel){sel={selector:selector,all:true};_removedCss.push(sel);}}
function _pageCssFilename(toCache){var u=window.location.href;var p=u.indexOf('?');if(p>0){u=u.substr(0,p);}
u=JsonDataBinding.urlToFilename(u);if(toCache){if(_useSavedUrl){if(JsonDataBinding.endsWithI(u,'.html')){u=u.substr(0,u.length-5);}
u=u+'.css';}
else{u=u+'.autoSave.css';}}
else{if(_useSavedUrl){u=u.substr(0,u.length-14);if(JsonDataBinding.endsWithI(u,'.html')){u=u.substr(0,u.length-5);}
u=u+'.css';}
else{if(JsonDataBinding.endsWithI(u,'.html')){u=u.substr(0,u.length-5);}
u=u+'.css';}}
return u;}
function _adjustPageCssFile(toCache){var u=_pageCssFilename(toCache);var v=_pageCssFilename(!toCache);_removedCssFile(v);_addcssfile(u,false);}
function _addRemovedCalss(className){if(className){if(!_removedClassNames){_removedClassNames=new Array();}
className=className.toLowerCase();var b=false;for(var i=0;i<_removedClassNames.length;i++){if(_removedClassNames[i]==className){b=true;break;}}
if(!b){_removedClassNames.push(className);}}}
function _getCssProperty(cssText,propName){var pos=cssText.indexOf('{');if(pos>=0){var css=cssText.substr(pos+1);if(css.length>0){if(css.substr(css.length-1,1)=='}'){css=css.substr(0,css.length-1);}
css=css.trim();propName=propName.toLowerCase().trim();while(css.length>0){var val;pos=css.indexOf(';');if(pos>=0){val=css.substr(0,pos);css=css.substr(pos+1).trim();}
else{val=css;css='';}
pos=val.indexOf(':');if(pos>0){var nm=val.substr(0,pos).trim();if(nm==propName){return(val.substr(pos+1)).trim();}}}}}}
function _removeComments(){var pc=document.getElementById(limnorHtmlEditorClient.pageCommentFrameId);if(pc){var pcp=pc.parentNode;if(pcp){pcp.removeChild(pc);}}}
function _addPageComment(){_removeComments();var pco=document.body.getAttribute('commentOption');if(pco!='disabled'){var pid=document.body.getAttribute('pageId');if(pid){var pcf=document.createElement('iframe');pcf.id=limnorHtmlEditorClient.pageCommentFrameId;pcf.style.width='98%';pcf.style.height='100%';pcf.src='../../../HomeComment.html?pageId='+pid+(pco=='readOnly'?'&readOnly=true':'');document.body.appendChild(pcf);}}}
return{addStdJsFile:function(stdlib){addStdJsLisFile(stdlib);},addCssLinkFile:function(url,beforeAll){addCssFile(url,beforeAll);},executeJsDb:function(fn,val){var func=JsonDataBinding[fn];return func(val);},executeClientFunc:function(fn,val){return fn(val);},setSavedUrlFlag:function(saved,pageHolder){_useSavedUrl=saved;_pageHolder=pageHolder;},getPageHolder:function(){return _pageHolder;},getPageAddress:function(inEdit){return _getPageAddress(inEdit);},getPageFilename:function(){return _getPageFilename(false);},getPageCssFilename:function(){return _getPageCssFilename();},getPageCssLinkNode:function(){return _getPageCssLinkNode();},getDynamicStyleNode:function(){return _getDynamicStyleNode();},addToRemoveCssBySelector:function(selector){_addToRemoveCssBySelector(selector);},addRemovedCalss:function(className){_addRemovedCalss(className);},getRemovedClassNames:function(){return _removedClassNames;},setDebugMode:function(debugMode){JsonDataBinding.Debug=debugMode;},createElement:function(tag){return document.createElement(tag);},appendChild:function(p,c){p.appendChild(c);},getElementById:function(id){return document.getElementById(id);},getElementsByTagName:function(tag){return document.getElementsByTagName(tag);},insertBefore:function(p,newChild,refChild){p.insertBefore(newChild,refChild);},getElementStyleValue:function(e,cssName){return _getElementStyleValue(e,cssName);},getElementStyleSetting:function(e,cssName,jsName){return _getElementStyleSetting(e,cssName,jsName);},getStyleValue:function(selectors,cssName){return _getStyleValue(selectors,cssName);},setElementStyleValue:function(e,name,cssName,value){_setElementStyleValue(e,name,cssName,value);},getOpacity:function(obj){var o=_getElementStyleValue(obj,'opacity');if(typeof(o)!='undefined'&&(o||o===0)){return Math.round(o*100);}
o=_getElementStyleValue(o,'-moz-opacity');if(typeof(o)!='undefined'&&(o||o===0)){return Math.round(o*100);}
o=_getElementStyleValue(o,'-khtml-opacity');if(typeof(o)!='undefined'&&(o||o===0)){return Math.round(o*100);}
o=_getElementStyleValue(o,'filter');if(typeof(o)!='undefined'&&o!=null){var pos=o.indexOf('=');if(pos>0){var s=o.substr(pos+1);if(s&&s.length>0){if(s[s.length-1]==')'){s=s.substr(0,s.length-1);}
return parseInt(s);}}}},setOpacity:function(obj,opacityValue){if(opacityValue<0)opacityValue=0;if(opacityValue>100)opacityValue=100;_setElementStyleValue(obj,'opacity','opacity',(opacityValue/100));_setElementStyleValue(obj,'MozOpacity','-moz-opacity',(opacityValue/100));_setElementStyleValue(obj,'KhtmlOpacity','-khtml-opacity',(opacityValue/100));_setElementStyleValue(obj,'filter','filter','alpha(opacity='+opacityValue+')');},getColumnProperty:function(obj,name){var o=_getElementStyleValue(obj,'-moz-column-'+name);if(typeof(o)!='undefined'&&(o||o===0)){return o;}
o=_getElementStyleValue(obj,'-webkit-column-'+name);if(typeof(o)!='undefined'&&(o||o===0)){return o;}
o=_getElementStyleValue(obj,'column-'+name);if(typeof(o)!='undefined'&&(o||o===0)){return o;}},setColumnProperty:function(obj,jsname,name,val){if(!val||val<=1){val=null;}
_setElementStyleValue(obj,jsname,'-moz-column-'+name,val);_setElementStyleValue(obj,jsname,'-webkit-column-'+name,val);_setElementStyleValue(obj,jsname,'column-'+name,val);},getLinearGradientAngle:function(obj){var sel=_getElementSelector(obj);return _getLinearGradientAngle(sel.selector);},getLinearGradientStartColor:function(obj){var sel=_getElementSelector(obj);return _getLinearGradientStartColor(sel.selector);},getLinearGradientEndColor:function(obj){var sel=_getElementSelector(obj);return _getLinearGradientEndColor(sel.selector);},setLinearGradientAngle:function(obj,val){var sel=_getElementSelector(obj);_setLinearGradientAngle(obj,sel.selector,val);},setLinearGradientStartColor:function(obj,val){var sel=_getElementSelector(obj);_setLinearGradientStartColor(obj,sel.selector,val);},setLinearGradientEndColor:function(obj,val){var sel=_getElementSelector(obj);_setLinearGradientEndColor(obj,sel.selector,val);},getDynamicCssText:function(client){return _getDynamicCssText(client);},getStyleNames:function(e){return _getStyleNames(e);},getClassNames:function(e){return _getClassNames(e);},getPagesClasses:function(){return _getPagesClasses();},addClass:function(e,cn){_addClass(e,cn);},removeClass:function(e,cn){_removeClass(e,cn);},removePageStyleBySelector:function(selector){_removePageStyleBySelector(selector);},adjustPageCssFile:function(toCache){_adjustPageCssFile(toCache);},verifyEditorOwner:function(){},getElementPosition:function(obj){return JsonDataBinding.ElementPosition.getElementPosition.apply(window,[obj]);},getPageZIndex:function(obj){return JsonDataBinding.getPageZIndex.apply(window,[obj]);},setPageEditor:function(peditor,pageId){_setPageEditor(peditor,pageId);},pasteHtmlToRange:function(range,html,newId){window.focus();range.pasteHTML(html);return document.getElementById(newId);},createIErange:function(){return document.selection.createRange();},addElement:function(range,node){range.deleteContents();range.insertNode(node);},focus:function(obj){obj.focus();},getDocType:function(){if(!_docType){_docType=getDocTypeString();if(!_docType){return'<!DOCTYPE html>';}}
return _docType;},createMenuStyles:function(){return HtmlEditorMenuBar.createMenuStyles();},onCreatedObject:function(ns,o,client){var designer=window[ns];designer.onCreatedObject(o,client);},pageInitialized:function(){if(typeof(JsonDataBinding)!='undefined'){if(!JsonDataBinding.stylesInitialized){JsonDataBinding.initializePage();}
return JsonDataBinding.stylesInitialized;}},createDataId:function(baseName,create){return limnorHtmlEditorClient.createDataId(baseName,create);},initEditor:function(pageEditor,calledClient){_removeComments();_disableAnchor();_disableIFrames();function initStyles(){if(typeof(JsonDataBinding)=='undefined'){setTimeout(initStyles,100);}
else if(!JsonDataBinding.stylesInitialized){JsonDataBinding.initializePage();setTimeout(initStyles,100);}
else{document.body.parentNode.style.height='100%';document.body.style.width='100%';document.body.style.height='100%';_getPageCssLinkNode();if(!inPlace){HtmlEditorTreeview.designInit(pageEditor,calledClient);HtmlEditorMenuBar.designInit(pageEditor,calledClient);}
calledClient.setInitialized();}}
initStyles();},getInitialized:function(){return _initialized;},setInitialized:function(){_initialized=true;},onAfterSaving:function(){_disableAnchor();},removeChild:function(p,c){p.removeChild(c);},resetDynamicStyles:function(){function onresetDynamicStyles(){if(typeof JsonDataBinding!='undefined'&&JsonDataBinding.dynsStyleProcessed){setTimeout(onresetDynamicStyles,100);return;}
var head=document.getElementsByTagName('head')[0];var ss=head.getElementsByTagName('style');if(ss&&ss.length>0){var dys=new Array();for(i=0;i<ss.length;i++){if(ss[i].title==limnorHtmlEditorClient.limnorDynaStyleTitle){dys.push(ss[i]);}}
for(i=0;i<dys.length;i++){head.removeChild(dys[i]);}}
var st=document.createElement('style');st.title=limnorHtmlEditorClient.limnorDynaStyleTitle;st.setAttribute('hidden','true');head.appendChild(st);}
onresetDynamicStyles();},addCssFile:function(filename,selector,cssText){if(filename){if(!_addcssfile(filename,false)){_updateDynamicStyle(selector,cssText);}
else{_addcssfile(filename,false);}}},getCssProperty:function(cssText,propName){return _getCssProperty(cssText,propName);},updateDynamicStyle:function(selector,value,name,cssName,selector2){_updateDynamicStyle(selector,value,name,cssName,selector2);},getPageCssStyle:function(){return _getPageCssStyle();},createNewStyleName:function(baseName){var n=1;var st=baseName+n;function styleNameExists(e){var stylename=e.getAttribute('styleName');if(st==stylename){return true;}
for(var i=0;i<e.children.length;i++){if(styleNameExists(e.children[i])){return true;}}
return false;}
while(styleNameExists(document.body)){n++;st=baseName+n;}
return st;},appendBodyText:function(){var p=document.createElement('p');p.innerHTML='New paragraph';document.body.appendChild(p);},onframemousemove:function(iframe,x,y){pageEditor.onclientmousemove(iframe.offsetLeft+x-document.documentElement.scrollLeft,iframe.offsetTop+y-document.documentElement.scrollTop,_pageHolder);},onframemouseup:function(iframe){pageEditor.onclientmouseup(iframe,_pageHolder);},addCustJsLisFile:function(custlib,e){_addCustJsLisFile(custlib,e);},initPlugin:function(plugin,obj){if(JsonDataBinding[plugin]&&JsonDataBinding[plugin].oninitpage){JsonDataBinding[plugin].oninitpage(obj);return true;}
return false;},cleanup:function(client,publishing){var p;var i;_enableAnchors();_enableIFrames();_oncleanuppage();var head=document.getElementsByTagName('head')[0];for(var i=0;i<editorFiles.length;i++){head.removeChild(editorFiles[i]);}
editorFiles=new Array();var hitCounter;var scriptNodes=head.getElementsByTagName('script');for(var i=0;i<scriptNodes.length;i++){if(scriptNodes[i].src!=null){var s=scriptNodes[i].src.toLowerCase();if(s.indexOf('/ip2cty.php?')>=0){hitCounter=scriptNodes[i];}}}
if(publishing){_addPageComment();var sts=document.getElementsByTagName('script');if(sts){var dl=new Array();for(i=0;i<sts.length;i++){var s=sts[i].src;if(s&&s.length>0){s=s.toLowerCase();if(s.indexOf('htmleditor')>=0){dl.push(sts[i]);}}}
for(i=0;i<dl.length;i++){p=dl[i].parentNode;p.removeChild(dl[i]);}}
if(_pageId){if(!hitCounter){hitCounter=document.createElement('script');head.appendChild(hitCounter);}
hitCounter.setAttribute('defer','defer');hitCounter.setAttribute('async','async');hitCounter.setAttribute('hidden','true');var src='/ip2cty.php?pid='+_pageId;if(document.getElementById('hc890326')){src=src+'&hc=hc890326';}
hitCounter.setAttribute('src',src);}}
else{if(hitCounter){head.removeChild(hitCounter);}}
if(!inPlace){var foundTV=HtmlEditorTreeview.cleanup(client);var foundMenu=HtmlEditorMenuBar.cleanup(client);if(!foundTV&&!foundMenu){var scripts=limnorHtmlEditorClient.getPageStarterNode();if(scripts){var fs=scripts.getAttribute('jsfiles');if(typeof fs=='undefined'||fs==null||fs.length==0){p=scripts.parentNode;p.removeChild(scripts);}}}}}}}},client:null}