// iframePlugin.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

var triedCount=0;function insertMouseCatcher(){var me;if(typeof(window.parent.limnorHtmlEditorClient)=='undefined'){triedCount++;if(triedCount>10){alert('unable to plug watcher');return;}
setTimeout(insertMouseCatcher,500);return;}
function getme(e){for(var i=0;i<e.children.length;i++){if(e.children[i].tagName.toLowerCase()=='iframe'){if(e.children[i].contentWindow==window){me=e.children[i];break;}}
else{getme(e.children[i]);if(me)
break;}}}
getme(window.parent.document.body);function onmousemove(e){e=e||window.event;if(e){var x=(e.pageX?e.pageX:e.clientX?e.clientX:e.x);var y=(e.pageY?e.pageY:e.clientY?e.clientY:e.y);window.parent.limnorHtmlEditorClient.client.onframemousemove(me,x,y);}}
function onmouseup(e){e=e||window.event;if(e){window.parent.limnorHtmlEditorClient.client.onframemouseup(me);}}
document.addEventListener('mousemove',onmousemove,false);document.addEventListener('mouseup',onmouseup,false);}
insertMouseCatcher();