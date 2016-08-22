// limnorsvg.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

JsonDataBinding.limnorsvg={oninitpage:function(e){if(e){JsonDataBinding.limnorsvg.createSVG(e);}
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