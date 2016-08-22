// htmleditorDiv.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

var limnorHtmlEditorDIV=limnorHtmlEditorDIV||{loading:false,loaded:false,loadLibs:function(divs){if(!limnorHtmlEditorDIV.loaded){if(!limnorHtmlEditorDIV.loading){limnorHtmlEditorDIV.loading=true;var editorFiles=new Array();var loadingJson=false;var loadingColor=false;var loadingDb=false;var loadingEditor=false;var loadingClient=false;var loadingStarter=false;var loadingModal=false;function loadjsfile(f){var head=document.getElementsByTagName('head')[0];var script=document.createElement('script');script.type='text/javascript';script.setAttribute('hidden','true');script.src=f+'?r='+Math.random();editorFiles.push(script);head.appendChild(script);}
function load(){if(typeof(JSONIDENTIFIER)=='undefined'){if(!loadingJson){loadingJson=true;loadjsfile('/libjs/json2.js');}
setTimeout(load,200);return;}
if(typeof(jscolor)=='undefined'){if(!loadingColor){loadingColor=true;loadjsfile('/libjs/jscolor.js');}
setTimeout(load,200);return;}
if(typeof(JsonDataBinding)=='undefined'){if(!loadingDb){loadingDb=true;loadjsfile('/libjs/jsonDataBind.js');}
setTimeout(load,200);return;}
if(typeof(JsonDataBinding.DialogueBox)=='undefined'){if(!loadingModal){loadingModal=true;loadjsfile('/libjs/modal.js');}
setTimeout(load,200);return;}
if(typeof(HtmlEditor)=='undefined'){if(!loadingEditor){loadingEditor=true;loadjsfile('/libjs/htmlEditor.js');}
setTimeout(load,200);return;}
if(typeof(limnorHtmlEditorClient)=='undefined'){if(!loadingClient){loadingClient=true;loadjsfile('/libjs/htmlEditorClient.js');}
setTimeout(load,200);return;}
if(typeof(limnorHtmlEditor)=='undefined'){if(!loadingStarter){loadingStarter=true;loadjsfile('/libjs/htmlEditorStarter.js');}
setTimeout(load,200);return;}
var i;limnorHtmlEditorDIV.loaded=true;var head=document.getElementsByTagName('head')[0];for(i=0;i<editorFiles.length;i++){head.removeChild(editorFiles[i]);}
if(divs&&divs.length>0){for(i=0;i<divs.length;i++){limnorHtmlEditorDIV.createEditor(document.getElementById(divs[i]));}}}
load();}}},createEditor:function(div){while(!limnorHtmlEditorDIV.loaded){limnorHtmlEditorDIV.loadLibs();if(confirm('Still loading libraries. Do you want to wait a little longer?')){}
else{break;}}
if(limnorHtmlEditorDIV.loaded){limnorHtmlEditor.editHtmlFile(null,{holder:div});}}}