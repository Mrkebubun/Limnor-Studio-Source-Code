// slideshow.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

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
_onsizechanged();return divElement.jsData;}};