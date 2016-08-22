// menu.js 2012-01-09 Copyright Longflow Enterprises Ltd. http://www.limnor.com

﻿
JsonDataBinding.CreateMenu=function(htmlElement){var _htmlTable=htmlElement;var _selectedId;var _selectedText;hideSubMenus=function(subMenuTable){if(subMenuTable){subMenuTable.style.display='none';var tds=subMenuTable.getElementsByTagName('td');if(tds){for(var i=0;i<tds.length;i++){if(tds[i].isMenu){if(tds[i].jsData){var t=tds[i].jsData.getSubMenuTable();if(t){hideSubMenus(t);}}}}}}}
hideMenus=function(){var tbls=document.getElementsByTagName('table');if(tbls){for(var i=0;i<tbls.length;i++){if(tbls[i].isMenu){tbls[i].style.display='none';}}}}
function init(){var tds=_htmlTable.getElementsByTagName('td');if(tds){for(var i=0;i<tds.length;i++){if(tds[i].id){JsonDataBinding.AttachEvent(tds[i],'onmouseover',onTopMenuMouseOver);JsonDataBinding.AttachEvent(tds[i],'onmouseout',onTopMenuMouseOut);JsonDataBinding.AttachEvent(tds[i],'onclick',onTopMenuClick);}}}
var _onmousedown=function(e){if(_htmlTable.currentSubMenu){var target=getEventSender(e);while(target){if(target==_htmlTable.currentSubMenu){return;}
target=target.parentNode;}
_htmlTable.currentSubMenu.style.display='none';_htmlTable.currentSubMenu=null;}
hideMenus();}
JsonDataBinding.AttachEvent(document,'onmousedown',_onmousedown);}
function getCell(e){var cell=getEventSender(e);;while(cell){if(cell.tagName){if(cell.tagName.toLowerCase()=='td'){return cell;}}
cell=cell.parentNode;}}
function restoreBackColor(cells){var bkc=_htmlTable.bgColor;for(var c=0;c<cells.length;c++){if(cells[c].isSelected){if(_htmlTable.selectedMenuBackColor){cells[c].style.backgroundColor=_htmlTable.selectedMenuBackColor;}
else{cells[c].style.backgroundColor=bkc;}}
else{cells[c].style.backgroundColor=bkc;}}}
function onTopMenuClick(e){var cell=getCell(e);;if(cell){var tr=cell.parentNode;if(tr){var cells=tr.getElementsByTagName("td");for(var c=0;c<cells.length;c++){cells[c].isSelected=false;}
cell.isSelected=true;restoreBackColor(cells);_selectedId=cell.id;_selectedText=cell.getAttribute('menuText');if(cell.onclickHandler){cell.onclickHandler();}}}}
function onTopMenuMouseOver(e){var cell=getCell(e);;if(cell){hideMenus();var tr=cell.parentNode;if(tr){var cells=tr.getElementsByTagName("td");restoreBackColor(cells);if(_htmlTable.mouseOverColor){cell.style.backgroundColor=_htmlTable.mouseOverColor;}
if(_htmlTable.currentSubMenu){_htmlTable.currentSubMenu.style.display='none';}
if(cell.jsData){cell.jsData.showSubMenus();}}}}
function onTopMenuMouseOut(e){var cell=getCell(e);if(cell){var tr=cell.parentNode;if(tr){var cells=tr.getElementsByTagName("td");restoreBackColor(cells);}}}
_attachMenuHandler=function(menuId,handler){var td=document.getElementById(menuId);if(td){td.onclickHandler=handler;}}
function onMenuMouseClick(e){var cell=getCell(e);;if(cell){var tr=cell.parentNode;while(tr&&tr.tagName.toLowerCase()!='tr'){tr=tr.parentNode;}
var tbl=cell.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");for(var c=0;c<cells.length;c++){cells[c].isSelected=false;}
cell.isSelected=true;restoreBackColor(cells);var cells0=tr.getElementsByTagName("td");if(cells0){for(var i=0;i<cells0.length;i++){if(cells0[i].isMenu){cell=cells0[i];break;}}}
_selectedId=cell.id;_selectedText=cell.getAttribute('menuText');if(cell.onclickHandler){cell.onclickHandler();}
hideMenus();}}
function onMenuMouseOver(e){var cell=getCell(e);;if(cell){var tr=cell.parentNode;while(tr&&tr.tagName.toLowerCase()!='tr'){tr=tr.parentNode;}
if(tr){var tbl=tr.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");restoreBackColor(cells);var cells0=tr.getElementsByTagName("td");if(cells0){for(var i=0;i<cells0.length;i++){if(cells0[i].isMenu){cell=cells0[i];break;}}}
if(_htmlTable.mouseOverColor){tr.style.backgroundColor=_htmlTable.mouseOverColor;cell.style.backgroundColor=_htmlTable.mouseOverColor;}
if(cell.jsData){cell.jsData.showSubMenus();}
else{if(cell.isMenu){if(tbl.currentSubMenu){tbl.currentSubMenu.style.display='none';}}}}}}
function onMenuMouseOut(e){var cell=getCell(e);if(cell){var tbl=cell.parentNode;while(tbl&&tbl.tagName.toLowerCase()!='table'){tbl=tbl.parentNode;}
var cells=tbl.getElementsByTagName("td");restoreBackColor(cells);}}
createMenuData=function(htmltd,submenus){var _td=htmltd;var _subMenus=submenus;var _parentMenuTable=_td.parentNode;while(_parentMenuTable.tagName.toLowerCase()!='table'){_parentMenuTable=_parentMenuTable.parentNode;}
var _subMenuTable;function _createSubMenu(parentMenu,menuData){var _parent=parentMenu;var _tbl=document.createElement("table");var _parentTable=_parent.parentNode;while(_parentTable.tagName.toLowerCase()!='table'){_parentTable=_parentTable.parentNode;}
_tbl.isMenu=true;_tbl.border=0;_tbl.bgColor=_htmlTable.bgColor;_tbl.style.color=_htmlTable.style.color;_tbl.style.fontFamily=_htmlTable.style.fontFamily;_tbl.style.fontSize=_htmlTable.style.fontSize;_tbl.style.position='absolute';_tbl.style.display='none';_tbl.style.cursor='pointer';_tbl.style.zIndex=_parentTable.style.zIndex?(_parentTable.style.zIndex+1):2;document.body.appendChild(_tbl);_tbl.parentMenuTable=_parentTable;_subMenuTable=_tbl;if(menuData){var _tbody;var tbd=_tbl.getElementsByTagName('tbody');if(tbd){if(tbd.length>0){_tbody=tbd[0];}}
if(!_tbody){_tbody=document.createElement('tbody');_tbl.appendChild(_tbody);}
for(var i=0;i<menuData.length;i++){var tr=document.createElement("tr");_tbody.appendChild(tr);if(menuData[i].text=='-'){var tdImg=document.createElement("td");tr.appendChild(tdImg);tdImg.colSpan=3;var img=document.createElement("img");tdImg.appendChild(img);img.style.display='block';img.src='images/menusep.png';img.style.width='100%';img.style.height="2px";img.height="2px";tdImg.style.height="2px";tdImg.height="2px";}
else{var tdImg=document.createElement("td");tr.appendChild(tdImg);var img=document.createElement("img");tdImg.appendChild(img);img.style.display='none';if(menuData[i].imagePath){if(menuData[i].imagePath.length>0){img.src=menuData[i].imagePath;img.style.display='inline';}}
var td=document.createElement("td");td.isMenu=true;td.id=menuData[i].id;td.setAttribute("menuText",menuData[i].text);td.innerHTML=menuData[i].text;tr.appendChild(td);JsonDataBinding.AttachEvent(tr,'onmouseover',onMenuMouseOver);JsonDataBinding.AttachEvent(tr,'onmouseout',onMenuMouseOut);JsonDataBinding.AttachEvent(tr,'onmousedown',onMenuMouseClick);var tdArrow=document.createElement("td");tdArrow.style.width="9px";tr.appendChild(tdArrow);if(menuData[i].subItems){if(menuData[i].subItems.length>0){var imgArrow=document.createElement("img");imgArrow.src="images/arrow.gif";tdArrow.appendChild(imgArrow);td.jsData=createMenuData(td,menuData[i].subItems);}}}}}
return _tbl;}
function _addSubMenus(subMenus){if(subMenus){if(_subMenus){for(var i=0;i<submenus.length;i++){_subMenus.push(subMenus[i]);}}
else{_subMenus=subMenus;}
createSubMenus();}}
function createSubMenus(){if(!_subMenuTable&&_subMenus){_subMenuTable=_createSubMenu(_td,_subMenus);}}
function _showSubMenus(){if(_parentMenuTable.currentSubMenu){hideSubMenus(_parentMenuTable.currentSubMenu);}
if(_parentMenuTable.style.display!='none'){if(_subMenuTable){var ps={};if(_parentMenuTable==_htmlTable){_td.cellLocation=JsonDataBinding.ElementPosition.getElementPosition(_td);ps.x=_td.cellLocation.x+document.body.scrollLeft;ps.y=(_td.cellLocation.y+_td.offsetHeight);}
else{var pos=_td.cellLocation=JsonDataBinding.ElementPosition.getElementPosition(_td.parentNode);ps.x=(pos.x+_td.parentNode.offsetWidth+document.body.scrollLeft);ps.y=pos.y;}
_parentMenuTable.currentSubMenu=_subMenuTable;_subMenuTable.style.left=ps.x+'px';_subMenuTable.style.top=ps.y+'px';_subMenuTable.style.display='block';JsonDataBinding.windowTools.updateDimensions();ps=JsonDataBinding.ElementPosition.getElementPosition(_subMenuTable);var pw=JsonDataBinding.anchorAlign.getElementWidth(_subMenuTable.parentNode);var ph=JsonDataBinding.anchorAlign.getElementHeight(_subMenuTable.parentNode);if(ps.x+_subMenuTable.scrollWidth>pw){var x;if(_parentMenuTable==_htmlTable){x=pw-_subMenuTable.offsetWidth+document.body.scrollLeft;}
else{x=_parentMenuTable.offsetLeft-_subMenuTable.offsetWidth;}
if(x<0)x=0;_subMenuTable.style.left=x+'px';}
if(ps.y+_subMenuTable.scrollHeight>ph){var y;if(_parentMenuTable==_htmlTable){y=_htmlTable.offsetTop-_subMenuTable.offsetHeight+document.body.scrollTop;}
else{y=_parentMenuTable.offsetTop-_subMenuTable.offsetHeight;}
if(y<0)y=0;_subMenuTable.style.top=y+'px';}}}}
function _getSubMenuTable(){return _subMenuTable;}
function init(){createSubMenus();}
init();return{addSubMenus:function(subMenus){_addSubMenus(subMenus);},showSubMenus:function(){_showSubMenus();},getSubMenuTable:function(){return _getSubMenuTable();}};}
function _addSubMenu(parentId,subMenus){var menu=document.getElementById(parentId);if(menu){if(menu.jsData){menu.jsData.addSubMenus(subMenus);}
else{menu.jsData=createMenuData(menu,subMenus);}}}
function _getSelectedMenuId(){return _selectedId;}
function _getSelectedMenuText(){return _selectedText;}
init();return{attachMenuHandler:function(menuId,handler){_attachMenuHandler(menuId,handler);},addSubMenu:function(parentId,subMenus){_addSubMenu(parentId,subMenus);},getSelectedMenuId:function(){return _getSelectedMenuId();},getSelectedMenuText:function(){return _getSelectedMenuText();}};}