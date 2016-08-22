// pageStarter.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

var limnorPageLoader=limnorPageLoader||{getPageStarterNode:function(create){var stData;var ss=document.getElementsByTagName('script');if(ss){for(var i=0;i<ss.length;i++){var s=ss[i].getAttribute('src');if(s){var p=s.lastIndexOf('/');if(p>=0){s=s.substr(p+1);}
p=s.lastIndexOf('\\');if(p>=0){s=s.substr(p+1);}
p=s.indexOf('?');if(p>=0){s=s.substr(0,p);}
s=s.toLowerCase();if(s=='pagestarter.js'){stData=ss[i];break;}}}}
if(stData){stData.setAttribute('hidden','true');}
else if(create){stData=document.createElement('script');stData.setAttribute('type','text/javascript');stData.setAttribute('src','/libjs/pageStarter.js');stData.setAttribute('hidden','true');var head=document.getElementsByTagName('head')[0];head.appendChild(stData);}
return stData;},startPage:function(){var editorFiles=new Array();function loadjsfile(f){var head=document.getElementsByTagName('head')[0];var script=document.createElement('script');script.type='text/javascript';script.setAttribute('hidden','true');script.src=f+'?r='+Math.random();editorFiles.push(script);head.appendChild(script);}
function onstartpage(){if(document.readyState!="complete"){setTimeout(onstartpage,200);return;}
var divmsg=document.createElement('div');divmsg.style.left='10px';divmsg.style.top='10px';divmsg.style.width='500px';divmsg.style.height='309px';divmsg.style.borderStyle='solid';divmsg.style.borderRadius='20px';divmsg.style.position='absolute';divmsg.style.backgroundColor='white';divmsg.style.margin='10px';divmsg.style.padding='10px';divmsg.style.color='red';divmsg.style.verticalAlign='middle';divmsg.style.fontSize='large';document.body.appendChild(divmsg);divmsg.innerHTML='Loading JavaScript files. please wait...';divmsg.style.zIndex=10;divmsg.style.display='block';var jsListUsages;var stData889923=limnorPageLoader.getPageStarterNode();if(stData889923){jsListUsages=stData889923.getAttribute('stdlib');}
if(typeof(JSONIDENTIFIER)=='undefined'){loadjsfile('/libjs/json2.js');}
function initPage(){if(typeof(JSONIDENTIFIER)=='undefined'){setTimeout(initPage,200);return;}
if(typeof(JsonDataBinding)=='undefined'){loadjsfile('/libjs/jsonDataBind.js');}
function initPage2(){if(typeof(JsonDataBinding)=='undefined'){setTimeout(initPage2,200);return;}
var loadingHEditor;var loadingStarter;var heitor=(jsListUsages&2);function loadHEditor(){if(heitor){heitor=false;if(typeof(HtmlEditor)=='undefined'){if(!loadingHEditor){loadingHEditor=true;loadjsfile('/libjs/htmlEditor.js');}
heitor=true;}
if(typeof(limnorHtmlEditor)=='undefined'){if(!loadingStarter){loadingStarter=true;loadjsfile('/libjs/htmlEditorStarter.js');}
heitor=true;}
if(heitor){setTimeout(loadHEditor,200);return;}}
var jsloaded=false,tryCount=0;var ss;if(stData889923){var fs=stData889923.getAttribute('jsfiles');if(typeof fs!='undefined'&&fs!=null&&fs.length>0){ss=fs.split(';');for(var i=0;i<ss.length;i++){ss[i]=ss[i].trim();loadjsfile('/libjs/'+ss[i]+'.js');}}}
function initPage3(){if(!jsloaded){jsloaded=true;if(ss&&ss.length>0){for(var i=0;i<ss.length;i++){if(typeof(JsonDataBinding[ss[i]])=='undefined'){jsloaded=false;break;}}}
if(!jsloaded){tryCount++;if(tryCount>20){alert('Timeout waiting for loading javascript files.');if(divmsg){divmsg.style.display='none';document.body.removeChild(divmsg);divmsg=null;}}
else{setTimeout(initPage3,300);}
return;}}
try{var head=document.getElementsByTagName('head')[0];for(var i=0;i<editorFiles.length;i++){head.removeChild(editorFiles[i]);}
editorFiles=new Array();JsonDataBinding.initializePage();if(ss&&ss.length>0){for(var i=0;i<ss.length;i++){if(JsonDataBinding[ss[i]].oninitpage){JsonDataBinding[ss[i]].oninitpage();}}}}
catch(err3){alert(err3.message);}
if(divmsg){divmsg.style.display='none';document.body.removeChild(divmsg);divmsg=null;}
try{if(typeof(limnorStudio)!='undefined')limnorStudio.onPageStarted();else{if(typeof(window.external)!='undefined'&&typeof(window.external.onPageStarted)=='function'){window.external.onPageStarted();}}}
catch(err){alert('error talking to IDE:'+err.message);}}
initPage3();}
loadHEditor();}
initPage2();}
initPage();}
onstartpage();}};function limnorPageLoaderPresent(){return true;}
limnorPageLoader.startPage();