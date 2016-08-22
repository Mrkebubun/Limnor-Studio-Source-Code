/*
	Html Editor Library -- JavaScript
	Copyright Longflow Enterprises Ltd -- invention ideas: David Ge
	2011

	depends on: jsonDataBind.js, filebrowser.js, menubar.js, treeview.js, slideshow.js, limnorsvg.js, webfilepicker.html, web filepicker.php

			top level elements: _divProp,_divSelectElement, _resizer, _nameresizer, _divError, _divToolTips, _divLargeEnum
			_divProp structure: 4 child-level elements: _titleTable, _toolbarTable, _editorDiv, _messageBar
			_titleTable
			_toolbarTable
			_editorDiv
				_editorTable
					row 1:_tdParentList   | td properties
						  _objTable       | _divPropertyGrid
						row1: _parentList |   _tableProps
						row2:_tdObjDelim  |  row 1:tool bar, it is one td holds all elements, colspan=2
						row3: _tdObj	  |  other rows:
						                  |	columns: name | value
			_messageBar (a div)
				_spMsg
				
		inline editing:
		holder:containerDiv - external DIV to hold _divProp
		_editorTable: row 1 has one more td, containerDiv.tdContentContainer, to hold a DIV	to be edited
						add one button to hide/show the first two td's
		DIV:  table
			row 1: inlineBar - td colspan = 2, holds _divProp
			row 2: inlineProps - td 1 holds _editorDiv; tdContentContainer - td 2

*/
//
var HtmlEditor = HtmlEditor || {
	version: 'Visual HTML Editor (Version 1.1)',
	hitcountid: 'hc890326',
	svgshapemovegap: 10,
	borderStyle: ['none', 'hidden', 'dotted', 'dashed', 'solid', 'double', 'groove', 'ridge', 'inset', 'outset', 'inherit'],
	widthStyle: ['', 'thin', 'medium', 'thick', 'inherit'],
	textAlign: ['', 'left', 'right', 'center', 'justify', 'inherit'],
	overflow: ['', 'visible', 'hidden', 'scroll', 'auto', 'no-display', 'no-content'],
	cssFloat: ['', 'left', 'right', 'none', 'inherit'],
	cursorValues: ['', 'auto', 'crosshair', 'default', 'e-resize', 'help', 'move', 'n-resize', 'ne-resize', 'nw-resize', 'pointer', 's-resize', 'se-resize', 'sw-resize', 'text', 'w-resize', 'wait'],
	verticalAlign: ['', 'length in px', '%', 'baseline', 'sub', 'super', 'top', 'text-top', 'middle', 'bottom', 'text-bottom', 'inherit'],
	fontStyles: ['', 'normal', 'italic', 'oblique', 'inherit'],
	fontVariants: ['', 'normal', 'small-caps', 'inherit'],
	fontWeights: ['', 'normal', 'bold', 'bolder', 'lighter',
		'100', '200', '300', '400', '500', '600', '700', '800', '900', 'inherit'],
	blocks: ',P,H1,H2,H3,H4,H5,H6,UL,OL,DIR,MENU,DL,PRE,DIV,CENTER,BLOCKQUOTE,IFRAME,NOSCRIPT,NOFRAMES,FORM,ISINDEX,HR,TABLE,ADDRESS,FIELDSET,NAV,',
	inlines1: ',TT,I,B,U,S,STRIKE,BIG,SMALL,FONT,EM,STRONG,DFN,CODE,SAMP,KBD,VAR,CITE,ABBR,ACRONYM,SUB,SUP,Q,SPAN,BDO,',
	inlines2: ',A,OBJECT,APPLET,IMG,BASEFONT,BR,SCRIPT,MAP,INPUT,SELECT,TEXTAREA,LABEL,BUTTON,EMBED,',
	CanBeChild: function(p, c) {
		var p0 = ',' + p.toUpperCase() + ',';
		if (p0 == ',BODY,') return true;
		var c0 = ',' + c.toUpperCase() + ',';
		function isPInline() {
			return (HtmlEditor.inlines1.indexOf(p0) >= 0 || HtmlEditor.inlines2.indexOf(p0) >= 0);
		}
		function isCInline() {
			return (HtmlEditor.inlines1.indexOf(c0) >= 0 || HtmlEditor.inlines2.indexOf(c0) >= 0);
		}
		if (',P,H1,H2,H3,H4,H5,H6,'.indexOf(p0) >= 0) {
			if (isCInline()) return true;
		}
		else if (p0 == ',UL,' || p0 == ',OL,') {
			if (c0 == ',LI,') return true;
		}
		else if (p0 == ',DL,') {
			if (c0 == ',DT,') return true;
			if (c0 == ',DD,') return true;
		}
		else if (p0 == ',DT,') {
			if (isCInline()) return true;
		}
		else if (p0 == ',DD,DIV,CENTER,BLOCKQUOTE,IFRAME,NOSCRIPT,NOFRAMES,TH,TD,LI,') {
			if (isCInline()) return true;
			if (HtmlEditor.blocks.indexOf(c0) >= 0) return true;
		}
		else if (p0 == ',PRE,') {
			if (',IMG,OBJECT,APPLET,BIG,SMALL,SUB,SUP,FONT,BASEFONT,'.indexOf(c0) >= 0) return false;
			if (isCInline()) return true;
		}
		else if (p0 == ',FORM,') {
			if (c0 == ',FORM,') return false;
			if (isCInline()) return true;
			if (HtmlEditor.blocks.indexOf(c0) >= 0) return true;
		}
		else if (p0 == ',TABLE,') {
			if (',CAPTION,COLGROUP,COL,THEAD,TBODY,TBODY,'.indexOf(c0) >= 0) return true;
		}
		else if (p0 == ',CAPTION,') {
			if (isCInline()) return true;
		}
		else if (p0 == ',COLGROUP,') {
			if (c0 == ',COL,') return true;
		}
		else if (',THEAD,TBODY,TBODY,'.indexOf(p0) >= 0) {
			if (c0 == ',TR,') return true;
		}
		else if (p0 == ',TR,') {
			if (c0 == ',TH,' || c0 == ',TD,') return true;
		}
		else if (p0 == ',TH,' || p0 == ',TD,') {
			if (isCInline()) return true;
			if (HtmlEditor.blocks.indexOf(c0) >= 0) return true;
		}
		else if (p0 == ',ADDRESS,') {
			if (c0 == ',P,') return true;
			if (isCInline()) return true;
		}
		else if (p0 == ',FIELDSET,') {
			if (c0 == ',LEGEND,') return true;
			if (isCInline()) return true;
			if (HtmlEditor.blocks.indexOf(c0) >= 0) return true;
		}
		else if (p0 == ',LEGEND,') {
			if (isCInline()) return true;
		}
		else if (p0 == ',A,') {
			if (c0 != ',A,') {
				if (isCInline()) {
					return true;
				}
			}
		}
		else if (HtmlEditor.inlines1.indexOf(p0) >= 0) {
			return isCInline();
		}
		else if (p0 = ',OBJECT,' || p0 == ',APPLET,') {
			if (c0 == ',PARAM,') return true;
			if (isCInline()) return true;
			if (HtmlEditor.blocks.indexOf(c0) >= 0) {
				return true;
			}
		}
		else if (p0 == ',MAP,') {
			if (c0 == ',AREA,') return true;
			if (HtmlEditor.blocks.indexOf(c0) >= 0) return true;
		}
		else if (p0 == ',SELECT,') {
			if (c0 == ',OPTGROUP,') return true;
			if (c0 == ',OPTION,') return true;
		}
		else if (p0 == ',OPTGROUP,') {
			if (c0 == ',OPTION,') return true;
		}
		else if (p0 == ',BUTTON,') {
			if (',A,INPUT,SELECT,TEXTAREA,LABEL,BUTTON,FORM,ISINDEX,FIELDSET,IFRAME,'.indexOf(c0) < 0) {
				return true;
			}
		}
		return false;
	},
	getParent: function(p0, tag) {
		while (p0 && p0.tagName) {
			if (HtmlEditor.CanBeChild(p0.tagName, tag)) {
				return p0;
			}
			p0 = p0.parentNode;
		}
		return document.body;
	},
	removeQuoting: function(v) {
		if (typeof v != 'undefined' && v && v.length > 0) {
			if (v.substr(0, 1) == "'" || v.substr(0, 1) == '"') {
				v = v.substr(1);
			}
			if (v.length > 1) {
				if (v.substr(v.length - 1, 1) == "'" || v.substr(v.length - 1, 1) == '"') {
					v = v.substr(0, v.length - 1);
				}
			}
		}
		return v;
	},
	PickWebFile: function(title, fileTypes, handler, disableUpload, subFolder, subName, maxSize) {
		JsonDataBinding.setupChildManager();
		document.childManager.onChildWindowReady = function() {
			JsonDataBinding.executeByPageId(28310, 'setData', title, fileTypes, disableUpload, subFolder, subName, maxSize);
		}
		JsonDataBinding.AbortEvent = false;
		var vf4a10b76 = {};
		vf4a10b76.isDialog = true;
		vf4a10b76.url = 'WebFilePicker.html';
		vf4a10b76.center = true;
		vf4a10b76.top = 0;
		vf4a10b76.left = 0;
		vf4a10b76.width = 475;
		vf4a10b76.height = 500;
		vf4a10b76.resizable = true;
		vf4a10b76.border = '0px';
		vf4a10b76.icon = '/libjs/folder.gif';
		vf4a10b76.title = title ? title : 'Select web file';
		vf4a10b76.pageId = 28310;
		vf4a10b76.onDialogClose = function() {
			if (JsonDataBinding.isValueTrue((document.dialogResult) != ('ok'))) {
				JsonDataBinding.AbortEvent = true;
				return;
			}
			handler(JsonDataBinding.getPropertyByPageId(28310, 'propSelectedFile'));
		}
		JsonDataBinding.showChild(vf4a10b76);
	},
	GetIECompatableWarning: function() {
		if (JsonDataBinding.IsIE()) {
			return "Please turn off IE Compatibility View via Tools menu.";
		}
		return '';
	},
	IsNameValid: function(name) {
		if (name) {
			var regexp = /^[a-zA-Z_$][0-9a-zA-Z_$]*$/;
			if (name.search(regexp) != -1) {
				return true;
			}
		}
		return false;
	},
	getCssNameFromPropertyName: function(propName) {
		if (propName && propName.length > 0) {
			if (propName == 'cssFloat')
				return 'float';
			return propName.replace(/\W+/g, '-').replace(/([a-z\d])([A-Z])/g, '$1-$2').toLowerCase();
		}
	},
	getPropertyNameFromCssName: function(cssName) {
		if (cssName && cssName.length > 0) {
			if (cssName == 'float')
				return 'cssFloat';
			return cssName.replace(/\W+(.)/g, function(x, chr) {
				return chr.toUpperCase();
			})
		}
	},
	//editorOptions members:objEdited,hideOKbutton, finishHandler, initLocation, saveToFolder, saveTo, forIDE, client
	//objEdited: html element to be edited; if it is null and inline is true then the whole html is being edited (not fully tested)
	//finishHandler:function(htmlString) -- when the OK button is clicked, this function is called with the result html string
	//  usually it is for the purpose of manually uploading.
	//  if data binding is used then this function is not needed
	//hideOKbutton: do not display OK button
	//initLocation: x and y for initial location of the editor, optionally w:width, h:height
	//saveToFolder: where to save the web page
	//saveTo: web page name to be save as
	//forIDE: for Limnor Studio IDE usage 
	//htmlFileOption:for html editing only, 0-not showing Folder name and file name, 1-path read-only, 2-path editable
	//inline:if it is NOT true then objEdited is a window object of the page being edited. the editor is not inside the page being edited. 
	//  if it is true then it is for non-body editing only
	//  if !inline then the hosting page should not be editable
	//getPageHolder:returns a div which holds the page being edited. it is pre-set, not a parameter
	createEditor: function(containerDiv) {
		var _editorWindow = window;
		var _editorOptions;
		var _divProp;
		var _divPropertyGrid;
		var _divElementToolbar;
		var _titleTable;
		var _toolbarTable;
		var _editorDiv;
		var _tdTitle;
		var _toolbar;
		var _tdSelectedProperty;
		var _messageBar;
		var _tdParentList;
		var _tdObj;
		var _tdObjDelim;
		var _trObjDelim;
		var _editorTable;
		var _editorBody;
		var _parentList;
		var _propsBody;
		var _imgExpand;
		var _imgOK;
		var _imgCancel;
		var _imgSave;
		var _imgReset;
		var _imgShowProp;
		var _tableProps;
		var _divSelectElement;
		var _divLargeEnum;
		var _newElementSelectTableBackColor = '#E0FFFF';
		var inputColorPicker;
		var _inputX;
		var imgNewElement;
		var _elementEditorList;
		var _addableElementList;
		var _commandList;
		var _resizer;
		var _nameresizer;
		var _isIE;
		var _ieVer;
		var _isFireFox;
		var _isOpera;
		var _isChrome;
		var _fontCommands; //div holding command images
		var _fontSelect;
		var _headingSelect;
		var _colorSelect;
		var _imgStopTag;
		var _imgInsSpace;
		var _imgInsBr;
		var _imgAppBr;
		var _imgAppTxt;
		var _imgUndo;
		var _imgRedo;
		var _imgUndel;
		var _addOptionWithNull;
		var _divPropLocation;
		var _spMsg;
		//
		var _elementLocator;
		var _textInput;
		var _locatorInput;
		var _locatorList;
		var _imgLocatorElement;
		var _objectsInLocatorList;
		var _collectingResult;
		var _saveTryCount;
		var _divError;
		var _divToolTips;
		//
		var _topElements;
		var _propertyTopElements;
		var _combos;
		//
		var _custMouseDown;
		var _custMouseDownOwner;
		var _comboInput;
		//
		var _autoSaveMinutes = 10;
		var _webPath = JsonDataBinding.getWebSitePath();
		//
		var _editorStart = '<!--editorstart-->';
		var _editorEnd = '<!--editorend-->';
		var _editorStartJs = '//editorstart';
		var _editorEndJs = '//editorend';
		var _editorscript = '//limnorstudiohtmleditor';
		//
		var fontList = ['',
		'cursive', 'monospace', 'serif', 'sans-serif', 'fantasy', 'Arial', 'Arial Black', 'Arial Narrow', 'Arial Rounded MT Bold',
		'Bookman Old Style', 'Bradley Hand ITC', 'Century', 'Century Gothic', 'Comic Sans MS', 'Courier',
		'Courier New', 'Georgia', 'Gentium', 'Impact', 'King', 'Lucida Console', 'Lalit', 'Modena', 'Monotype Corsiva',
		'Papyrus', 'Tahoma', 'TeX', 'Times', 'Times New Roman', 'Trebuchet MS', 'Verdana', 'Verona'
		];
		var fontSizes = ['', 'xx-small', 'x-small', 'small', 'medium', 'large', 'x-large', 'xx-large', 'smaller', 'larger', 'size in px, cm, ...', '%', 'inherit'
		];
		//
		JsonDataBinding.inediting = true;
		//
		_ieVer = JsonDataBinding.getInternetExplorerVersion();
		_isIE = JsonDataBinding.IsIE();
		_isFireFox = JsonDataBinding.IsFireFox();
		_isOpera = JsonDataBinding.IsOpera();
		_isChrome = JsonDataBinding.IsChrome();
		//
		function _insertBefore(p, c, r) {
			_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [p, c, r]);
		}
		function _insertAfter(p, c, r) {
			_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [p, c, r?r.nextSibling:null]);
		}
		function _createElement(tag) {
			return _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, [tag]);
		}
		function _createSVGElement(tag) {
			return _editorOptions.client.createSVGElement.apply(_editorOptions.elementEditedWindow, [tag]);
		}
		function _duplicateSvg(svg) {
			var w = _editorOptions.client.duplicateSvg.apply(_editorOptions.elementEditedWindow, [svg]);
			selectEditElement(w);
			return w;
		}
		function _bringSvgToFront(svg) {
			_editorOptions.client.bringSvgToFront.apply(_editorOptions.elementEditedWindow, [svg]);
		}
		function _getUploadIFrame() {
			return _editorOptions.client.getUploadIFrame.apply(_editorOptions.elementEditedWindow);
		}
		function _appendChild(p, c) {
			_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [p, c]);
		}
		function _removeChild(p, c) {
			_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [p, c]);
		}
		function _getElementById(id) {
			return _editorOptions.client.getElementById.apply(_editorOptions.elementEditedWindow, [id]);
		}
		function _setElementStyleValue(obj, jsName, cssName, val) {
			var setCss = (_editorOptions.isEditingBody && !obj.subEditor);
			if (setCss) {
				if (obj.getAttribute) {
					var styleshare = obj.getAttribute('styleshare');
					if (typeof styleshare == 'undefined' || styleshare == null || styleshare.toLowerCase() != 'notshare') {
					}
					else {
						setCss = false;
					}
				}
			}
			if (setCss)
				_editorOptions.client.setElementStyleValue.apply(_editorOptions.editorWindow, [obj, jsName, cssName, val]);
			else {
				var o = obj.subEditor ? obj.obj : obj;
				try{
					o.style[jsName] = val;
				}
				catch (err) {
				}
				if (typeof val == 'undefined' || val == null || val == '') {
					JsonDataBinding.removeStyleAttribute(obj, jsName, cssName);
				}
			}
		}
		function addOptionToSelect(sel, op) {
			if (_addOptionWithNull) {
				if (_addOptionWithNull == 1) {
					sel.add(op, null);
				}
				else {
					try {
						sel.add(op);
					}
					catch (e) {
						alert(e.message);
					}
				}
			}
			else {
				try {
					sel.add(op, null);
					_addOptionWithNull = 1;
				}
				catch (e) {
					sel.add(op);
					_addOptionWithNull = 2;
				}
			}
		}
		function createTreeViewItemPropDescs(o, li) {
			function addSubItem() {
				var newLi = o.jsData.addItem(li);
				//showProperties(newLi);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
			function resetState() {
				if (o.jsData.resetState) {
					o.jsData.resetState();
				}
			}
			function movetvitemup() {
				LI_moveup(li);
				resetState();
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
			function movetvitemdown() {
				LI_movedown(li);
				resetState();
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
			return {
				subEditor: true,
				obj: li,
				owner: o,
				allowCommands: true,
				getString: function() {
					return elementToString(li, 'treeItem');
				},
				getStringForIDE: function () {
					var sRet = 'treeitem,,,';
					if (li.id && li.id.length > 0) {
						sRet += li.id;
					}
					sRet += ',';
					var g = li.getAttribute('limnorId');
					if (g) {
						sRet += g;
					}
					return sRet;
				},
				getProperties: function() {
					var props = new Array();
					props.push(idProp);
					for (i = 0; i < _elementEditorList.length; i++) {
						if (_elementEditorList[i].tagname == 'li') {
							for (var k = 0; k < _elementEditorList[i].properties.length; k++) {
								if (_elementEditorList[i].properties[k].name == 'background') {
									continue;
								}
								if (_elementEditorList[i].properties[k].editor == EDIT_CUST) {
									continue;
								}
								props.push(_elementEditorList[i].properties[k]);
							}
							break;
						}
					}
					props.push({
						name: 'delete', desc: 'delete the tree item', editor: EDIT_DEL,
						action: function() {
							if (confirm('Do you want to remove this tree item and all its sub items?')) {
								//var undoItem = { guid: 'body', undoInnerHTML: _editorOptions.elementEditedDoc.body.innerHTML };
								var newOwner = li.parentNode;
								o.jsData.delItem(li);
								selectEditElement(newOwner);
								if(typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
								//undoItem.redoInnerHTML = _editorOptions.elementEditedDoc.body.innerHTML;
								//undoItem.done = true;
								//addUndoItem(undoItem);
							}
						},
						notModify:true
					});
					props.push({
						name: 'addSubItem', toolTips: 'add a new sub tree item', editor: EDIT_CMD, IMG: '/libjs/newElement.png',
						action: addSubItem
					}
					);
					props.push({
						name: 'moveup', toolTips: 'move tree item up', editor: EDIT_CMD, IMG: '/libjs/moveup.png',
						action: movetvitemup
					}
					);
					props.push({
						name: 'movedown', toolTips: 'move tree item down', editor: EDIT_CMD, IMG: '/libjs/movedown.png',
						action: movetvitemdown
					}
					);
					return { props: props };
				}
			}
		}
		function _showResizer() {
			if (_divProp && _divProp.expanded && !containerDiv) {
				if (_resizer) {
					JsonDataBinding.windowTools.updateDimensions();
					var pos = JsonDataBinding.ElementPosition.getElementPosition(_divProp);
					_resizer.style.zIndex = JsonDataBinding.getZOrder(_divProp) + 1;
					var szOff = 22; //_isIE ? 22 : 16;
					_resizer.style.left = (pos.x + _divProp.offsetWidth - szOff) + 'px';
					_resizer.style.top = (pos.y + _divProp.offsetHeight - szOff) + 'px';
					_resizer.style.display = 'block';
				}
			}
		}
		function isEditorObject(c) {
			if (_editorOptions && _editorOptions.isEditingBody) {
				var c0 = c;
				while (c0) {
					if (c0.forProperties || c0 == _elementLocator) {
						return true;
					}
					c0 = c0.parentNode;
				}
			}
			return false;
		}
		function isMarker(c) {
			if (c) {
				if (_editorOptions) {
					if (c == _editorOptions.redbox)
						return true;
					//if (c.id == HtmlEditor.svgholderid)
					//	return true;
					if (_editorOptions.markers) {
						for (var i = 0; i < _editorOptions.markers.length; i++) {
							if (_editorOptions.markers[i] == c) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		function isEditingObject(c) {
			if (_editorOptions) {
				var c0 = c;
				while (c0) {
					if (_editorOptions.isEditingBody) {
						if (c0 == _editorOptions.elementEdited || c0 == _editorOptions.elementEditedDoc) {
							return true;
						}
					}
					else {
						if (c0 == _editorOptions.elementEdited) {
							return true;
						}
					}
					c0 = c0.parentNode;
				}
			}
			return false;
		}
		var UNDOSTATE_UNDO = 0;
		var UNDOSTATE_REDO = 1;
		var UNDOTYPE_KEYBOARD = 1;
		function hasUndo() {
			if (_editorOptions) {
				if (_editorOptions.undoState && _editorOptions.undoList && _editorOptions.undoList.length > 0) {
					if (_editorOptions.undoState.index >= 0 && _editorOptions.undoState.index < _editorOptions.undoList.length) {
						if (_editorOptions.undoState.index == 0 && _editorOptions.undoState.state == UNDOSTATE_UNDO) {
							return false;
						}
						return true;
					}
				}
			}
			return false;
		}
		function hasRedo() {
			if (_editorOptions) {
				if (_editorOptions.undoState && _editorOptions.undoList && _editorOptions.undoList.length > 0) {
					if (_editorOptions.undoState.index >= 0 && _editorOptions.undoState.index < _editorOptions.undoList.length) {
						if (_editorOptions.undoState.index == _editorOptions.undoList.length - 1 && _editorOptions.undoState.state == UNDOSTATE_REDO) {
							return false;
						}
						return true;
					}
				}
			}
			return false;
		}
		function showUndoRedo() {
			if (hasUndo()) {
				_imgUndo.src = '/libjs/undo.png';
			}
			else {
				_imgUndo.src = '/libjs/undo_inact.png';
			}
			if (hasRedo()) {
				_imgRedo.src = '/libjs/redo.png';
			}
			else {
				_imgRedo.src = '/libjs/redo_inact.png';
			}
		}
		function adjustSizes() {
			if (_editorOptions) {
				//if (_editorOptions.isEditingBody) {
				var propHeight = _divProp.offsetHeight - _titleTable.offsetHeight - _messageBar.offsetHeight - _toolbarTable.offsetHeight - 20;
				if (propHeight > 0) {
					_editorDiv.style.height = propHeight + 'px';
					//_editorTable's border is 1
					var ph = propHeight - _divElementToolbar.offsetHeight;
					if (ph > 28) {
						_divPropertyGrid.style.height = (propHeight - 28) + 'px';
						if (!_editorOptions.isEditingBody && containerDiv && containerDiv.tdContentContainer) {
							var divDlg;
							for (var i = 0; i < containerDiv.tdContentContainer.children.length; i++) {
								if (containerDiv.tdContentContainer.children[i].tagName.toLowerCase() == 'div') {
									divDlg = containerDiv.tdContentContainer.children[i];
									break;
								}
							}
							if (divDlg) {
								divDlg.style.height = (ph - 6) + 'px';
							}
						}
					}
					adjustObjectSize(propHeight);
				}
			}
		}
		//forSelOnly - true:for getting _editorOptions.selectedHtml
		//             false: for getting node coverring _editorOptions.selectedHtml
		function captureSelection(e, forSelOnly) {
			var selObj;
			var node;
			var obj = e.target;
			if (!obj) {
				obj = JsonDataBinding.getSender(e);
			}
			if (_isIE) {
				if (obj && obj.tagName && obj.tagName.toLowerCase() == 'html') {
					return;
				}
			}
			if (isMarker(obj)) {
				obj = null;
			}
			_editorOptions.lastSelHtml = _editorOptions.selectedHtml;
			_editorOptions.selectedHtml = null;
			if (_editorOptions.selectedObject) {
				selObj = _editorOptions.selectedObject.subEditor ? _editorOptions.selectedObject.obj : _editorOptions.selectedObject;
			}
			if (window.getSelection) { //W3C standard
				_editorOptions.selection = _editorOptions.elementEditedWindow.getSelection(); // a Selection object
				// Get range (standards)
				if (_editorOptions.selection.getRangeAt !== undefined) {
					if (_editorOptions.selection.rangeCount > 0) {
						_editorOptions.range = _editorOptions.selection.getRangeAt(0);
					}
				}
				else if (_editorOptions.elementEditedDoc.createRange &&
					_editorOptions.selection.anchorNode &&
					_editorOptions.selection.anchorOffset &&
					_editorOptions.selection.focusNode &&
					_editorOptions.selection.focusOffset) {
					// Get range (Safari 2)
					_editorOptions.range = _editorOptions.elementEditedDoc.createRange.apply(_editorOptions.elementEditedWindow);
					_editorOptions.range.setStart(_editorOptions.selection.anchorNode, _editorOptions.selection.anchorOffset);
					_editorOptions.range.setEnd(_editorOptions.selection.focusNode, _editorOptions.selection.focusOffset);
				}
				else {
					// Failure here, not handled by the rest of the script.
					// Probably IE or some older browser
					_editorOptions.range = null;
				}
				if (_editorOptions.range) {
					if (_editorOptions.range.cloneContents) {
						_editorOptions.selectedHtml = _editorOptions.range.cloneContents().textContent;
						if (forSelOnly) return;
					}
					if (_editorOptions.range.endContainer) {
						if (_editorOptions.range.endContainer.nodeType == 1) {
							node = _editorOptions.range.endContainer;
						}
						else {
							if (_editorOptions.range.endContainer.parentElement && _editorOptions.range.endContainer.parentElement.tagName) {
								node = _editorOptions.range.endContainer.parentElement;
							}
							else if (_editorOptions.range.endContainer.parentNode && _editorOptions.range.endContainer.parentNode.tagName) {
								node = _editorOptions.range.endContainer.parentNode;
							}
						}
					}
					else if (_editorOptions.range.startContainer) {
						if (_editorOptions.range.startContainer.nodeType == 1) {
							node = _editorOptions.range.startContainer;
						}
						else {
							if (_editorOptions.range.startContainer.parentElement && _editorOptions.range.startContainer.parentElement.tagName) {
								node = _editorOptions.range.startContainer.parentElement;
							}
							else if (_editorOptions.range.startContainer.parentNode && _editorOptions.range.startContainer.parentNode.tagName) {
								node = _editorOptions.range.startContainer.parentNode;
							}
						}
					}
					else if (_editorOptions.range.commonAncestorContainer) {
						if (_editorOptions.range.commonAncestorContainer.nodeType == 1) {
							node = _editorOptions.range.commonAncestorContainer;
						}
						else {
							if (_editorOptions.range.commonAncestorContainer.parentElement && _editorOptions.range.commonAncestorContainer.parentElement.tagName) {
								node = _editorOptions.range.commonAncestorContainer.parentElement;
							}
							else if (_editorOptions.range.commonAncestorContainer.parentNode && _editorOptions.range.commonAncestorContainer.parentNode.tagName) {
								node = _editorOptions.range.commonAncestorContainer.parentNode;
							}
						}
					}
				}
				if (!node) {
					if (_editorOptions.selection.focusNode) {
						if (_editorOptions.selection.focusNode.nodeType == 1) {
							node = _editorOptions.selection.focusNode;
						}
						else {
							if (_editorOptions.selection.focusNode.parentElement && _editorOptions.selection.focusNode.parentElement.tagName) {
								node = _editorOptions.selection.focusNode.parentElement;
							}
							else if (_editorOptions.selection.focusNode.parentNode && _editorOptions.selection.focusNode.parentNode.tagName) {
								node = _editorOptions.selection.focusNode.parentNode;
							}
						}
					}
					else if (_editorOptions.selection.anchorNode) {
						if (_editorOptions.selection.anchorNode.nodeType == 1) {
							node = _editorOptions.selection.anchorNode;
						}
						else {
							if (_editorOptions.selection.anchorNode.parentElement && _editorOptions.selection.anchorNode.parentElement.tagName) {
								node = _editorOptions.selection.anchorNode.parentElement;
							}
							else if (_editorOptions.selection.anchorNode.parentNode && _editorOptions.selection.anchorNode.parentNode.tagName) {
								node = _editorOptions.selection.anchorNode.parentNode;
							}
						}
					}
				}
				//if (node) {
				//	//if (isEditorObject(node)) {
				//	//	return;
				//	//}
				//	//if (!isEditingObject(node)) {
				//	//	return;
				//	//}
				//	if (node != _editorOptions.selectedObject) {
				//		selectEditElement(node);
				//	}
				//}
			}
			else if (_editorOptions.elementEditedDoc.selection && _editorOptions.elementEditedDoc.selection.createRange) { //IE 8/9/10
				if (_editorOptions.client) {
					_editorOptions.range = _editorOptions.client.createIErange.apply(_editorOptions.elementEditedWindow);
				}
				else {
					_editorOptions.range = _editorOptions.elementEditedDoc.selection.createRange.apply(_editorOptions.elementEditedWindow); // a Text Range object
				}
				if (_editorOptions.range.htmlText) {
					_editorOptions.selectedHtml = _editorOptions.range.htmlText;
				}
				else if (_editorOptions.range.length > 0) {
					_editorOptions.selectedHtml = _editorOptions.range.item(0).outerHTML;
				}
				if (forSelOnly) return;
				if (_editorOptions.elementEditedDoc.selection.type == 'Control') {
					node = _editorOptions.range(0);
				}
				else {
					node = _editorOptions.range.parentElement();
				}
				//if (node) {
				//	//if (!isEditorObject(node) && isEditingObject(node)) {
				//		if (node != selObj) {
				//			selectEditElement(node);
				//		}
				//	//}
				//}
			}
			if (node) {
				if (obj) {
					var op = obj;
					var isP = false;
					while (op) {
						if (op == node) {
							isP = true;
							break;
						}
						op = op.parentNode;
					}
					if (isP) {
						node = obj;
					}
				}
			}
			else {
				node = obj;
			}
			if (node) {
				if (node != selObj) {
					selectEditElement(node);
				}
			}
			_editorOptions.iframeFocus = true;
			showFontCommands();
		}
		function isNonBodyNode(c) {
			if (c) {
				if (c.tagName) {
					var tag = c.tagName.toLowerCase();
					if (tag == 'html' || tag == 'meta' || tag == 'link' || tag == 'script' || tag == 'head') {
						return true;
					}
					return false;
				}
				else {
					if (c.allowCommands) {
						return false;
					}
				}
			}
			return true;
		}
		function showFontCommands() {
			if (_editorOptions && _editorOptions.deleted) {
				_imgUndel.src = '/libjs/undel.png';
			}
			else {
				_imgUndel.src = '/libjs/undel_inact.png';
			}
			var c = _editorOptions ? (_editorOptions.propertiesOwner ? _editorOptions.propertiesOwner : _editorOptions.selectedObject) : null;
			if (showEditorButtons(c)) {
				showtextAlign(_editorOptions.selectedObject);
				var i;
				_fontCommands.style.display = 'inline';
				var imgs = _fontCommands.getElementsByTagName('img');
				for (i = 0; i < imgs.length; i++) {
					if (imgs[i].cmd && !imgs[i].isCust) {
						try{
							if (_editorOptions.elementEditedDoc.queryCommandState(imgs[i].cmd)) {
								imgs[i].src = imgs[i].actSrc;
							}
							else {
								imgs[i].src = imgs[i].inactSrc;
							}
						}
						catch (err) {
						}
					}
				}
				if (c) {
					var fontNode;
					var headingNode;
					var obj = c;
					while (obj && obj != _editorOptions.elementEditedDoc.body) {
						var tag = '';
						if (obj.tagName) {
							tag = obj.tagName.toLowerCase();
						}
						if (!fontNode) {
							if (tag == 'span') {
								fontNode = obj;
								if (headingNode) {
									break;
								}
							}
						}
						if (!headingNode) {
							if (tag == 'h1' || tag == 'h2' || tag == 'h3' || tag == 'h4' || tag == 'h5' || tag == 'h6') {
								headingNode = obj;
								if (fontNode) {
									break;
								}
							}
						}
						obj = obj.parentNode;
					}
					var found;
					if (_fontSelect && _fontSelect.options && _fontSelect.options.length > 0) {
						if (fontNode) {
							found = false;
							for (i = 0; i < _fontSelect.options.length; i++) {
								if (_fontSelect.options[i].value == fontNode.style.fontFamily) {
									_fontSelect.selectedIndex = i;
									found = true;
									break;
								}
							}
							if (!found) {
								_fontSelect.selectedIndex = 0;
							}
						}
						else {
							_fontSelect.selectedIndex = 0;
						}
					}
					if (fontNode) {
						_colorSelect.value = fontNode.style.color;
						_colorSelect.style.backgroundColor = fontNode.style.color;
					}
					else {
						_colorSelect.style.backgroundColor = 'white';
					}
					if (_headingSelect && _headingSelect.options && _headingSelect.options.length > 0) {
						if (headingNode) {
							found = false;
							for (i = 0; i < _headingSelect.options.length; i++) {
								if ('h' + _headingSelect.options[i].value == tag) {
									_headingSelect.selectedIndex = i;
									found = true;
									break;
								}
							}
							if (!found) {
								_headingSelect.selectedIndex = 0;
							}
						}
						else {
							_headingSelect.selectedIndex = 0;
						}
					}
				}
				else {
					_colorSelect.style.backgroundColor = 'white';
				}
				_colorSelect.style.color = _colorSelect.style.backgroundColor;
				if (canStopTag()) {
					_imgInsSpace.src = '/libjs/insSpace.png';
					_imgStopTag.src = '/libjs/stopTag.png';
					_imgInsBr.src = '/libjs/insBr.png';
					_imgAppBr.src = '/libjs/appBr.png';
				}
				else {
					_imgInsSpace.src = '/libjs/insSpace_inact.png';
					_imgStopTag.src = '/libjs/stopTag_inact.png';
					_imgInsBr.src = '/libjs/insBr_inact.png';
					_imgAppBr.src = '/libjs/appBr_inact.png';
				}
				showUndoRedo();
			}
		}
		function createSvg() {
			if (!_editorOptions) return;
			var svg = _createSVGElement('svg');
			svg.style.top = '0px';
			svg.style.left = '0px';
			svg.style.height = '300px';
			svg.style.width = '300px';
			svg.style.borderWidth = '1px';
			svg.style.borderStyle = 'dotted';
			_appendChild(_editorOptions.elementEditedDoc.body, svg);
			return svg;
		}
		function createElement(tagname, type, typename) {
			if (!_editorOptions) return;
			for (var k = 0; k < _elementEditorList.length; k++) {
				if (_elementEditorList[k].tagname == typename) {
					if (_elementEditorList[k].type) {
						if (_elementEditorList[k].type == type) {
							if (_elementEditorList[k].creator) {
								if (_editorOptions.inline)
									return _elementEditorList[k].creator();
								else
									return _elementEditorList[k].creator.apply(_editorOptions.objEdited);
							}
						}
					}
					else if (_elementEditorList[k].creator) {
						if (_editorOptions.inline)
							return _elementEditorList[k].creator();
						else
							return _elementEditorList[k].creator.apply(_editorOptions.objEdited);
					}
				}
			}
			return _createElement(tagname);
		}
		function _addCommandList(commands) {
			if (_commandList) {
				if (commands) {
					var i;
					for (i = 0; i < commands.length; i++) {
						for (var k = 0; k < _commandList.length; k++) {
							if (_commandList[k].cmd == commands[i].cmd) {
								_commandList.slice(k, 1);
								break;
							}
						}
					}
					for (i = 0; i < commands.length; i++) {
						_commandList.push(commands[i]);
					}
				}
			}
			else {
				_commandList = commands;
			}
		}
		function _addAddableElementList(addables) {
			if (_addableElementList) {
				if (addables) {
					var i;
					for (i = 0; i < addables.length; i++) {
						for (var k = 0; k < _addableElementList.length; k++) {
							if (_addableElementList[k].tag == addables[i].tag) {
								if (addables[i].type) {
									if (addables[i].type == _addableElementList[k].type) {
										_addableElementList = _addableElementList.slice(k, 1);
										break;
									}
								}
								else {
									_addableElementList = _addableElementList.slice(k, 1);
									break;
								}
							}
						}
					}
					for (i = 0; i < addables.length; i++) {
						_addableElementList.push(addables[i]);
					}
				}
			}
			else {
				_addableElementList = addables;
			}
		}
		function _getElementDesc(elementTag) {
			if (elementTag) {
				elementTag = elementTag.toLowerCase();
				if (_addableElementList) {
					for (var k = 0; k < _addableElementList.length; k++) {
						if (_addableElementList[k].tag == elementTag) {
							return _addableElementList[k].name;
						}
					}
				}
			}
			return elementTag;
		}
		function _addEditors(editors) {
			if (_elementEditorList) {
				if (editors) {
					var i;
					for (i = 0; i < editors.length; i++) {
						for (var k = 0; k < _elementEditorList.length; k++) {
							if (_elementEditorList[k].tagname == editors[i].tagname) {
								_elementEditorList.slice(k, 1);
								break;
							}
						}
					}
					for (i = 0; i < editors.length; i++) {
						_elementEditorList.push(editors[i]);
					}
				}
			}
			else {
				_elementEditorList = editors;
			}
		}
		function hideSelector() {
			if (_divSelectElement) {
				_divSelectElement.style.display = 'none';
			}
			if (_divLargeEnum) {
				_divLargeEnum.style.display = 'none';
			}
			_showResizer();
		}
		function framenotify(event) {
			if (_editorOptions)
				selectEditElement(_editorOptions.elementEditedDoc.activeElement);
		}
		//tagname: tagName of node
		function insertNodeOverSelection(node, tagname) {
			if (!_editorOptions.selectedObject) return;
			var newclass = null;
			var parentObj = _editorOptions.selectedObject.subEditor ? _editorOptions.selectedObject.obj : _editorOptions.selectedObject;
			if (parentObj.tagName) {
				var stag = parentObj.tagName.toLowerCase();
				if (stag == 'input' || stag == 'textarea') {
					showError('Cannot insert a new element into an input element');
					return;
				}
			}
			var undoItem;
			if (tagname == 'area') {
				var map = parentObj;
				while (map) {
					if (map.tagName && map.tagName.toLowerCase() == 'map')
						break;
					map = map.parentNode;
				}
				if (map) {
					//undoItem = { guid: 'body', undoInnerHTML: _editorOptions.elementEditedDoc.body.innerHTML };
					map.appendChild(node);
					selectEditElement(node);
				}
			}
			else {
				try {
					var needSwitchToDiv;
					var pp;
					var ptag = _editorOptions.selectedObject.subEditor ? _editorOptions.selectedObject.obj.tagName : _editorOptions.selectedObject.tagName;
					if (tagname != 'a' && !HtmlEditor.CanBeChild.apply(_editorOptions.elementEditedWindow, [ptag, tagname])) {
						if (ptag == 'p' || ptag == 'P') {
							needSwitchToDiv = true;
						}
						if (!needSwitchToDiv) {
							alert('Cannot add a child of "' + tagname + '" to a "' + ptag + '". If you cannot move the cursor outside of current element then click image "/>" on the toolbar.');
							return;
						}
					}
					//undoItem = { guid: 'body', undoInnerHTML: _editorOptions.elementEditedDoc.body.innerHTML };
					//if (pp) {
					//	var pp2 = parentObj;
					//	while (pp2 && pp2.parentNode != pp) {
					//		pp2 = pp2.parentNode;
					//	}
					//	if (pp2) {
					//		var nextSibling = getNextSibling(pp2);
					//		_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [pp, node, nextSibling]);
					//		if (node.focus) {
					//			_editorOptions.client.focus.apply(_editorOptions.elementEditedWindow, [node]);
					//		}
					//		selectEditElement(node);
					//	}
					//	else {
					//		return;
					//	}
					//}
					//else
					if (_editorOptions.objEdited.getSelection) { //W3C standard
						if (_editorOptions.range) {
							if (tagname == 'span' || tagname == 'h1' || tagname == 'h2' || tagname == 'h3' || tagname == 'h4' || tagname == 'h5' || tagname == 'h6') {
								if (_editorOptions.selectedHtml && _editorOptions.selectedHtml.length > 0)
									node.innerHTML = _editorOptions.selectedHtml;
								_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow, [_editorOptions.range, node]);
							}
							else if (tagname == 'a') {
								var stag = '';
								if (_editorOptions.selectedObject && _editorOptions.selectedObject.tagName) {
									stag = _editorOptions.selectedObject.tagName.toLowerCase();
								}
								if (stag == 'button' || stag == 'img') {
									_insertBefore(_editorOptions.selectedObject.parentNode, node, _editorOptions.selectedObject);
									_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [node, _editorOptions.selectedObject]);
								}
								else {
									if (stag == 'td') {
										if (_editorOptions.selectedHtml && _editorOptions.selectedHtml.length > 0) {
											node.innerHTML = _editorOptions.selectedHtml;
											_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow, [_editorOptions.range, node]);
										}
										else {
											var ht = _editorOptions.selectedObject.innerHTML;
											_editorOptions.selectedObject.innerHTML = '';
											node.innerHTML = ht;
											_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [_editorOptions.selectedObject, node]);
										}
									}
									else if (stag != 'tr' && stag != 'tbody' && stag != 'thead' && stag != 'tfoot' && stag != 'table') {
										if (_editorOptions.selectedHtml && _editorOptions.selectedHtml.length > 0) {
											node.innerHTML = _editorOptions.selectedHtml;
											_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow, [_editorOptions.range, node]);
										}
										else {
											if (_editorOptions.selectedObject) {
												_insertBefore(_editorOptions.selectedObject.parentNode, node, _editorOptions.selectedObject);
												_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [node, _editorOptions.selectedObject]);
											}
										}
									}
									else {
										return;
									}
								}
							}
							else {
								_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow, [_editorOptions.range, node]);
							}
							if (node.focus) {
								_editorOptions.client.focus.apply(_editorOptions.elementEditedWindow, [node]);
							}
						}
						else {
							if (tagname == 'a') {
								if (stag == 'button' || stag == 'img') {
									_insertBefore(_editorOptions.selectedObject.parentNode, node, _editorOptions.selectedObject);
									_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [node, _editorOptions.selectedObject]);
								}
								else {
									if (stag == 'td') {
										if (_editorOptions.selectedHtml && _editorOptions.selectedHtml.length > 0) {
											node.innerHTML = _editorOptions.selectedHtml;
											_editorOptions.client.addElement.apply(_editorOptions.elementEditedWindow, [_editorOptions.selectedObject, node]);
										}
										else {
											if (!node.innerHTML || node.innerHTML.length == 0) {
												node.innerHTML = 'hyper link';
											}
											_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [_editorOptions.selectedObject, node]);
										}
									}
									else if (stag != 'tr' && stag != 'tbody' && stag != 'thead' && stag != 'tfoot' && stag != 'table') {
										if (_editorOptions.selectedObject) {
											if (!node.innerHTML || node.innerHTML.length == 0) {
												node.innerHTML = 'hyper link';
											}
											_insertBefore(_editorOptions.selectedObject.parentNode, node, _editorOptions.selectedObject);
										}
										else {
											return;
										}
									}
									else {
										return;
									}
								}
							}
							else {
								_appendChild(parentObj, node);
							}
						}
					}
					else { //IE 8/9/10	   
						if (_editorOptions.range && _editorOptions.range.parentElement && parentObj) { // a Text Range object
							var o = _editorOptions.range.parentElement();
							if (tagname == 'span' || tagname == 'a' || tagname == 'h1' || tagname == 'h2' || tagname == 'h3' || tagname == 'h4' || tagname == 'h5' || tagname == 'h6') {
								if (_editorOptions.selectedHtml)
									node.innerHTML = _editorOptions.selectedHtml;
								else
									node.innerHTML = _editorOptions.range.text;
							}
							var id = 'v' + Math.floor(Math.random() * 65536);
							node.id = id;
							var html = (node.nodeType == 3) ? node.data : node.outerHTML;
							if (_editorOptions.client) {
								node = _editorOptions.client.pasteHtmlToRange.apply(_editorOptions.elementEditedWindow, [_editorOptions.range, html, id]);
							}
							else {
								_editorOptions.range.pasteHTML.apply(_editorOptions.elementEditedWindow, [html]);
								node = _editorOptions.elementEditedDoc.getElementById.apply(_editorOptions.elementEditedWindow, [id]);
							}
							if (newclass != null) {
								node.setAttribute('styleName', newclass);
								node.className = newclass;
							}
							if (node) {
								if (node.focus)
									node.focus();
								node.id = '';
							}
						}
						else {
							parentObj.appendChild.apply(_editorOptions.elementEditedWindow, [node]);
						}
					}
					if (needSwitchToDiv) {
						var div = _createElement('div');
						var parent = parentObj.parentNode;
						_insertBefore(parent, div, parentObj);
						while (parentObj.firstChild) {
							_appendChild(div, parentObj.firstChild);
						}
						_removeChild(parent, parentObj);
					}
					if (node) {
						if (node.focus)
							node.focus();
						if (tagname == 'iframe') {
							selectEditElement(node);
							var fdoc = node.contentDocument || node.contentWindow.document;
							fdoc.onclick = framenotify;
						}
						else if (tagname == 'a') {
							if (node.children.length > 0) {
								_editorOptions.selectedObject = null;
								selectEditElement(node.children[0]);
								for (var i = 0; i < _parentList.options.length; i++) {
									if (_parentList.options[i].objvalue == node) {
										_parentList.selectedIndex = i;
										break;
									}
								}
								showProperties(node);
							}
							else {
								selectEditElement(node);
							}
						}
						else {
							selectEditElement(node);
						}
					}
				}
				catch (err) {
					showError('Cannot insert new element. ' + (err.message ? err.message : err));
				}
			}
			//if (undoItem) {
			//	undoItem.redoInnerHTML = _editorOptions.elementEditedDoc.body.innerHTML;
			//	undoItem.done = true;
			//	addUndoItem(undoItem);
			//}
			return node;
		}
		function isOrContainsNode(ancestor, descendant) {
			var node = descendant;
			while (node) {
				if (node === ancestor)
					return true;
				node = node.parentNode;
			}
			return false;
		}
		function createLargeEnum() {
			_divLargeEnum.style.position = 'absolute';
			_divLargeEnum.style.display = 'none';
			//_divLargeEnum.style.overflow = 'scroll';
			//_divLargeEnum
			_divLargeEnum.contentEditable = false;
			_divLargeEnum.forProperties = true;
			var tbls = document.createElement('table');
			tbls.border = 0;
			tbls.cellPadding = 0;
			tbls.cellSpacing = 0;
			tbls.forProperties = true;
			_divLargeEnum.appendChild(tbls);
			function _setData(options, cmdImg) {
				var rn = Math.floor(Math.sqrt(options.length));
				var rC = rn + 3;
				var tC = Math.floor(options.length / rC);
				while (rC * tC < options.length) {
					tC++;
				}
				var tbody0 = JsonDataBinding.getTableBody(tbls);
				while (tbody0.firstChild) {
					tbody0.removeChild(tbody0.firstChild);
				}
				var tr0 = tbody0.insertRow(-1);
				var ci = 0;
				for (var ti = 0; ti < tC; ti++) {
					var td0 = document.createElement('td');
					td0.style.verticalAlign = 'top';
					tr0.appendChild(td0);
					//
					var tblSel = document.createElement('table');
					tblSel.style.verticalAlign = 'top';
					td0.appendChild(tblSel);
					tblSel.border = 1;
					tblSel.bgColor = _newElementSelectTableBackColor;
					tblSel.style.cursor = 'pointer';
					var tbody = JsonDataBinding.getTableBody(tblSel);
					for (var i = 0; i < rC; i++) {
						var tr = tbody.insertRow(-1);
						tr.opt = options[ci];
						var td = document.createElement('td');
						tr.appendChild(td);
						//var img = document.createElement('img');
						//img.src = _addableElementList[ci].image;
						//img.style.display = 'inline';
						//td.appendChild(img);
						//td = document.createElement('td');
						//tr.appendChild(td);
						td.innerHTML = options[ci].text;
						JsonDataBinding.AttachEvent(tr, 'onmouseover', onhsMouseOver);
						JsonDataBinding.AttachEvent(tr, 'onmouseout', onhsMouseOut);
						JsonDataBinding.AttachEvent(tr, 'onclick', onhsMouseClick);
						//if (_addableElementList[ci].toolTips) {
						//	hookTooltips(img, _addableElementList[ci].toolTips);
						//}
						ci++;
						if (ci >= options.length) break;
					}
				}
				function restoreBackColor(cells) {
					var bkc = _newElementSelectTableBackColor;
					for (var c = 0; c < cells.length; c++) {
						if (cells[c].isMouseOver) {
							cells[c].style.backgroundColor = '#ffffc0';
						}
						else {
							cells[c].style.backgroundColor = bkc;
						}
					}
				}
				//create a new element
				function onhsMouseClick(e) {
					if (!_editorOptions) return;
					var cell = JsonDataBinding.getSender(e);
					if (cell) {
						var tr = cell.parentNode;
						while (tr && tr.tagName && tr.tagName.toLowerCase() != 'tr') {
							tr = tr.parentNode;
						}
						if (tr && tr.addable) {
							hideSelector();
							if (_editorOptions.isEditingBody) {
								_editorOptions.elementEditedDoc.body.contentEditable = true;
							}
							if (tr.opt) {

							}
						}
					}
				}
				function onhsMouseOver(e) {
					var cell = JsonDataBinding.getSender(e); ;
					if (cell) {
						var tbl = cell.parentNode;
						while (tbl && tbl.tagName.toLowerCase() != 'table') {
							tbl = tbl.parentNode;
						}
						var cells = tbl.getElementsByTagName("td");
						for (var i = 0; i < cells.length; i++) {
							cells[i].isMouseOver = false;
						}
						cell.isMouseOver = true;
						restoreBackColor(cells);
					}
				}
				function onhsMouseOut(e) {
					var cell = JsonDataBinding.getSender(e);
					if (cell) {
						var tbl = cell.parentNode;
						while (tbl && tbl.tagName.toLowerCase() != 'table') {
							tbl = tbl.parentNode;
						}
						var cells = tbl.getElementsByTagName("td");
						cell.isMouseOver = false;
						restoreBackColor(cells);
					}
				}
			}
			return {
				setData: function(options, cmdImg) {
					_setData(options, cmdImg);
				}
			};
		}
		function createElementSelector() {
			_divSelectElement.style.position = 'absolute';
			_divSelectElement.style.display = 'none';
			_divSelectElement.contentEditable = false;
			_divSelectElement.forProperties = true;
			var rn = Math.floor(Math.sqrt(_addableElementList.length));
			var rC = rn + 3;
			var tC = Math.floor(_addableElementList.length / rC);
			while (rC * tC < _addableElementList.length) {
				tC++;
			}
			var tbls = document.createElement('table');
			tbls.border = 0;
			tbls.cellPadding = 0;
			tbls.cellSpacing = 0;
			tbls.forProperties = true;
			_divSelectElement.appendChild(tbls);
			var tbody0 = JsonDataBinding.getTableBody(tbls);
			var tr0 = tbody0.insertRow(-1);
			var ci = 0;
			for (var ti = 0; ti < tC; ti++) {
				var td0 = document.createElement('td');
				td0.style.verticalAlign = 'top';
				tr0.appendChild(td0);
				var tblSel = document.createElement('table');
				tblSel.style.verticalAlign = 'top';
				td0.appendChild(tblSel);
				tblSel.border = 1;
				tblSel.bgColor = _newElementSelectTableBackColor;
				tblSel.style.cursor = 'pointer';
				var tbody = JsonDataBinding.getTableBody(tblSel);
				for (var i = 0; i < rC; i++) {
					var tr = tbody.insertRow(-1);
					tr.addable = _addableElementList[ci];
					var td = document.createElement('td');
					tr.appendChild(td);
					var img = document.createElement('img');
					img.src = _addableElementList[ci].image;
					img.style.display = 'inline';
					td.appendChild(img);
					td = document.createElement('td');
					tr.appendChild(td);
					td.innerHTML = _addableElementList[ci].name;
					JsonDataBinding.AttachEvent(tr, 'onmouseover', onhsMouseOver);
					JsonDataBinding.AttachEvent(tr, 'onmouseout', onhsMouseOut);
					JsonDataBinding.AttachEvent(tr, 'onclick', onhsMouseClick);
					if (_addableElementList[ci].toolTips) {
						hookTooltips(img, _addableElementList[ci].toolTips);
					}
					ci++;
					if (ci >= _addableElementList.length) break;
				}
			}
			function restoreBackColor(cells) {
				var bkc = _newElementSelectTableBackColor;
				for (var c = 0; c < cells.length; c++) {
					if (cells[c].isMouseOver) {
						cells[c].style.backgroundColor = '#ffffc0';
					}
					else {
						cells[c].style.backgroundColor = bkc;
					}
				}
			}
			//create a new element
			function onhsMouseClick(e) {
				if (!_editorOptions) return;
				var cell = JsonDataBinding.getSender(e);
				if (cell) {
					var tr = cell.parentNode;
					while (tr && tr.tagName && tr.tagName.toLowerCase() != 'tr') {
						tr = tr.parentNode;
					}
					if (tr && tr.addable) {
						hideSelector();
						if (_editorOptions.isEditingBody) {
							_editorOptions.elementEditedDoc.body.contentEditable = true;
						}
						else {
							if (tr.addable.pageonly) {
								alert('This element is for page editing only');
								tr.style.display = 'none';
								return;
							}
						}
						if (tr.addable.tag) {
							if (tr.addable.isSVG) {
								var newNode = createSvg();
								if (newNode) {
									if (tr.addable.onCreated) {
										tr.addable.onCreated(newNode);
									}
									selectEditElement(newNode);
								}
								return;
							}
							if (_editorOptions.selectedObject) {
								if (tr.addable.tag == 'hitcount') {
									if (!_editorOptions.elementEditedDoc.body.getAttribute('pageId')) {
										return;
									}
									var hc = _getElementById(HtmlEditor.hitcountid);
									if (hc) {
										selectEditElement(hc);
										return;
									}
								}
								if (_editorOptions.selectedObject.typename || (_editorOptions.selectedObject.getAttribute && _editorOptions.selectedObject.getAttribute('typename'))) {
								}
								else {
									if (!_editorOptions.isEditingBody && (tr.addable.tag == 'treeview' || tr.addable.tag == 'menubar')) {
										alert(tr.addable.tag + ' can only be used to build web pages.');
										return;
									}
									var newElement = createElement(tr.addable.htmltag ? tr.addable.htmltag : tr.addable.tag, tr.addable.type, tr.addable.tag);
									var newNode = insertNodeOverSelection(newElement, tr.addable.htmltag ? tr.addable.htmltag : tr.addable.tag);
									if (tr.addable.onCreated) {
										tr.addable.onCreated(newNode);
									}
									_editorOptions.pageChanged = true;
								}
							}
						}
					}
				}
			}
			function onhsMouseOver(e) {
				var cell = JsonDataBinding.getSender(e); ;
				if (cell) {
					var tbl = cell.parentNode;
					while (tbl && tbl.tagName.toLowerCase() != 'table') {
						tbl = tbl.parentNode;
					}
					var cells = tbl.getElementsByTagName("td");
					for (var i = 0; i < cells.length; i++) {
						cells[i].isMouseOver = false;
					}
					cell.isMouseOver = true;
					restoreBackColor(cells);
				}
			}
			function onhsMouseOut(e) {
				var cell = JsonDataBinding.getSender(e);
				if (cell) {
					var tbl = cell.parentNode;
					while (tbl && tbl.tagName.toLowerCase() != 'table') {
						tbl = tbl.parentNode;
					}
					var cells = tbl.getElementsByTagName("td");
					cell.isMouseOver = false;
					restoreBackColor(cells);
					if (cell.toolTips) {
						cell.moves = 0;
						if (_divToolTips) {
							_divToolTips.style.display = 'none';
						}
					}
				}
			}
			function _checkAdditables() {
				var inline = true;
				if (_editorOptions && _editorOptions.isEditingBody) {
					inline = false;
				}
				var ccs = tr0.cells;
				for (var ti = 0; ti < ccs.length; ti++) {
					var td0 = ccs[ti];
					var tblSel;
					for (var i = 0; i < td0.children.length; i++) {
						if (td0.children[i].tagName.toLowerCase() == 'table') {
							tblSel = td0.children[i];
							break;
						}
					}
					if (tblSel) {
						var tbody = JsonDataBinding.getTableBody(tblSel);
						for (var i = 0; i < tbody.rows.length; i++) {
							var tr = tbody.rows[i];
							if (inline && tr.addable.pageonly) {
								tr.style.display = 'none';
							}
							else {
								if (tr.addable.tag == 'hitcount') {
									if (!_editorOptions.elementEditedDoc.body.getAttribute('pageId')) {
										tr.style.display = 'none';
									}
									else if (inline || _getElementById(HtmlEditor.hitcountid))
										tr.style.display = 'none';
									else
										tr.style.display = '';
								}
								else 
								tr.style.display = '';
							}
						}
					}
				}
			}
			_divSelectElement.jsData = {
				checkAdditables: function() {
					_checkAdditables();
				}
			};
			return _divSelectElement.jsData;
		} //end of createElementSelector
		//show properties of a parent
		function selectparent() {
			if (_parentList.selectedIndex >= 0 && _parentList.selectedIndex < _parentList.options.length) {
				var obj = _parentList.options[_parentList.selectedIndex].objvalue;
				showProperties(obj);
			}
		}
		function cancelClick() {
			askClose();
		}
		function askClose(saved) {
			if (_editorOptions) {
				if (_editorOptions.isEditingBody) {
					if (_editorOptions.closePage) {
						if (saved) {
							saved = confirm('The web page is saved.\n\n Do you want to close the page now?\n Click Cancel if you want to continue editing the page.');
						}
						else {
							saved = confirm('Do you want to close the page being edited? \n\nModifications will be discarded if you close it now.');
						}
						if (saved) {
							_editorOptions.closePage(true);
							_editorOptions = null;
							clearPropertiesDisplay();
						}
					}
				}
				else {
					if (_editorOptions.cancelHandler) {
						saved = confirm('Do you want to cancel your message?');
						if (saved) {
							_editorOptions.cancelHandler();
						}
					}
				}
			}
		}
		//it should called after saving
		function _close() {
			if (_editorOptions && _editorOptions.closePage) {
				_editorOptions.closePage(true);
				_editorOptions = null;
				clearPropertiesDisplay();
			}
		}
		//
		function finishEdit(p) {
			if (_editorOptions && _editorOptions.finishHandler) {
				if (_collectingResult) {
					_saveTryCount++;
					if (_saveTryCount < 2) {
						alert('Please wait for the last saving operation to finish');
					}
					//else {
						//if (confirm('The last saving operation has not finished. Do you want to start a new saving operation?')) {
						//	_collectingResult = false;
						//}
					//}
				}
				if (!_collectingResult) {
					_collectingResult = true;
					_saveTryCount = 0;
					try{
						if (_editorOptions.isEditingBody) {
							var toCache = p ? p.toCache : false;
							var manualInvoke = p ? p.manualInvoke : false;
							var html = _getresultHTML(!toCache);
							var pos = html.indexOf('dyStyle8831932');
							if (pos > 0) {
								var pos0 = html.indexOf('<style ');
								if (pos0 > 0 && pos0 < pos) {
									var pos1;
									while (true) {
										pos1 = html.indexOf('<style ', pos0 + 1);
										if (pos1 > pos0) {
											pos0 = pos1;
										}
										else
											break;
									}
									pos1 = html.indexOf('</style>', pos);
									var pos2 = html.indexOf('/>', pos);
									if (pos1 > 0) {
										if (pos2 > 0 && pos2 < pos1) {
											pos1 = pos2 + 2;
										}
										else {
											pos1 = pos1 + 8;
										}
									}
									else {
										pos1 = pos2 + 2;
									}
									if (pos1 > 0) {
										html = html.substr(0, pos0) + html.substr(pos1);
									}
								}
							}
							var dynText = _editorOptions.client.getDynamicCssText.apply(_editorOptions.elementEditedWindow, [_editorOptions.client]);
							_editorOptions.undoState = {};
							_editorOptions.undoList = new Array();
							_editorOptions.finishHandler.apply(_editorOptions.elementEditedWindow, [{ url: getWebAddress(), html: html, css: dynText, toCache: toCache, manualInvoke: manualInvoke, saveToFolder: _editorOptions.saveToFolder ? _editorOptions.saveToFolder : '', saveTo: _editorOptions.saveTo ? _editorOptions.saveTo : ''}]);
						}
						else {
							_editorOptions.client.cleanup.apply(_editorOptions.elementEditedWindow, [_editorOptions.client]);
							_editorOptions.finishHandler(_editorOptions.elementEdited.innerHTML);
						}
					}
					catch (eSave) {
						alert('Error saving page. ' + (eSave.message ? eSave.message : eSave));
					}
					_collectingResult = false;
				}
			}
		}
		function _getHtmlString() {
			if (_collectingResult) {
				return '';
			}
			else {
				_collectingResult = true;
				var ret = _getresultHTML();
				_collectingResult = false;
				return ret;
			}
		}
		function _saveAndFinish() {
			finishEdit();
			_editorOptions.pageChanged = false;
		}
		function _pageModified() {
			return (_editorOptions ? _editorOptions.pageChanged : false);
		}
		function _setDocType(docType) {
			_editorOptions.docType = docType;
		}
		function createTable() {
			var tbl = _editorOptions.elementEditedDoc.createElement('table');
			tbl.border = 1;
			var h = _editorOptions.elementEditedDoc.createElement('thead');
			tbl.appendChild(h);
			var r = _editorOptions.elementEditedDoc.createElement('tr');
			h.appendChild(r);
			var c = _editorOptions.elementEditedDoc.createElement('td');
			r.appendChild(c);
			c.innerHTML = 'column 1';
			c = _editorOptions.elementEditedDoc.createElement('td');
			r.appendChild(c);
			c.innerHTML = 'column 2';
			var tbody = JsonDataBinding.getTableBody(tbl);
			r = _editorOptions.elementEditedDoc.createElement('tr');
			tbody.appendChild(r);
			c = _editorOptions.elementEditedDoc.createElement('td');
			r.appendChild(c);
			c.innerHTML = 'row 1, cell 1';
			c = _editorOptions.elementEditedDoc.createElement('td');
			r.appendChild(c);
			c.innerHTML = 'row 1 cell 2';
			//
			r = _editorOptions.elementEditedDoc.createElement('tr');
			tbody.appendChild(r);
			c = _editorOptions.elementEditedDoc.createElement('td');
			r.appendChild(c);
			c.innerHTML = 'row 2, cell 1';
			c = _editorOptions.elementEditedDoc.createElement('td');
			r.appendChild(c);
			c.innerHTML = 'row 2 cell 2';
			return tbl;
		}
		function isInLocator(obj) {
			while (obj && obj != document.body && obj != window) {
				if (obj == _elementLocator || obj == _imgLocatorElement) {
					return true;
				}
				obj = obj.parentNode;
			}
			return false;
		}
		function isInTextInputor(obj) {
			while (obj && obj != document.body && obj != window) {
				if (obj == _textInput || obj == _textInput) {
					return true;
				}
				obj = obj.parentNode;
			}
			return false;
		}
		function locatorInputchanged() {
			if (_editorOptions && _locatorInput.value.length > 0) {
				var tag;
				var objs = _editorOptions.elementEditedDoc.getElementsByTagName(_locatorInput.value);
				if (!objs || objs.length == 0) {
					if (_addableElementList) {
						for (var i = 0; i < _addableElementList.length; i++) {
							if (_addableElementList[i].tag == _locatorInput.value) {
								objs = _editorOptions.elementEditedDoc.getElementsByTagName(_addableElementList[i].htmltag);
								tag = _addableElementList[i].tag;
								break;
							}
						}
					}
				}
				if (objs && objs.length > 0) {
					_locatorList.options.length = 0;
					_objectsInLocatorList = new Array();
					for (var i = 0; i < objs.length; i++) {
						if (!isMarker(objs[i])) {
							if (tag) {
								var tag0 = objs[i].typename;
								if (!tag0) {
									if (objs[i].getAttribute) {
										tag0 = objs[i].getAttribute('typename');
									}
								}
								if (!tag0) {
									if (objs[i].jsData) {
										tag0 = objs[i].jsData.typename;
									}
								}
								if (tag != tag0)
									continue;
							}
							var elOptNew = document.createElement('option');
							elOptNew.text = elementToString(objs[i]);
							_objectsInLocatorList.push(objs[i]);
							addOptionToSelect(_locatorList, elOptNew);
						}
					}
					if (_locatorList.options.length < 3) {
						_locatorList.size = 3;
					}
					else if (_locatorList.options.length > 10) {
						_locatorList.size = 10;
					}
					else {
						_locatorList.size = _locatorList.options.length;
					}
				}
			}
		}
		function selectedFromlocator() {
			if (_locatorList.selectedIndex >= 0) {
				var obj = _objectsInLocatorList[_locatorList.selectedIndex];
				_elementLocator.style.display = 'none';
				if (obj.parentNode) {
					selectEditElement(obj);
				}
				else {
					alert('This element has been removed. Please make another selection.');
				}
			}
		}
		function locateElement() {
			if (!_editorOptions) return;
			if (!_elementLocator) {
				_elementLocator = document.createElement('div');
				_elementLocator.style.backgroundColor = '#ADD8E6';
				_elementLocator.style.position = 'absolute';
				_elementLocator.style.border = "3px double black";
				_elementLocator.style.color = 'black';
				appendToBody(_elementLocator);
				var tbl = document.createElement("table");
				_elementLocator.appendChild(tbl);
				tbl.border = 0;
				tbl.cellPadding = 0;
				tbl.cellSpacing = 0;
				tbl.width = '100%';
				tbl.height = '100%';
				var tblBody = JsonDataBinding.getTableBody(tbl);
				var tr = tblBody.insertRow(-1);
				tr.setAttribute('valign', 'top');
				td = document.createElement('td');
				tr.appendChild(td);
				var cap = document.createElement('span');
				td.appendChild(cap);
				cap.innerHTML = 'tag name:';
				td = document.createElement('td');
				tr.appendChild(td);
				_locatorInput = document.createElement('input');
				_locatorInput.type = 'text';
				td.appendChild(_locatorInput);
				JsonDataBinding.AttachEvent(_locatorInput, 'onchange', locatorInputchanged);
				JsonDataBinding.addTextBoxObserver(_locatorInput);
				JsonDataBinding.AttachEvent(_locatorInput, 'onkeyup', locatorInputchanged);
				tr = tblBody.insertRow(-1);
				tr.style.height = '100%';
				tr.vAlign = 'top';
				td = document.createElement('td');
				tr.appendChild(td);
				td = document.createElement('td');
				tr.appendChild(td);
				_locatorList = document.createElement('select');
				td.appendChild(_locatorList);
				JsonDataBinding.AttachEvent(_locatorList, 'onclick', selectedFromlocator);
			}
			else {
				locatorInputchanged();
			}
			_elementLocator.style.width = '300px';
			_elementLocator.style.height = '185px';
			JsonDataBinding.windowTools.updateDimensions();
			var ap = JsonDataBinding.ElementPosition.getElementPosition(_imgLocatorElement);
			_elementLocator.style.left = (ap.x + 3) + 'px';
			_elementLocator.style.top = (ap.y + 3) + 'px';
			var zi = JsonDataBinding.getPageZIndex(_elementLocator) + 1;
			_elementLocator.style.zIndex = zi;
			_elementLocator.style.display = 'block';
		}
		function addElement(e) {
			if (!_editorOptions) return;
			var sender = JsonDataBinding.getSender(e);
			if (sender && _editorOptions.selectedObject) {
				JsonDataBinding.windowTools.updateDimensions();
				var ap = JsonDataBinding.ElementPosition.getElementPosition(sender);
				if (ap) {
					if (ap.x < 0) ap.x = 0;
					if (ap.y < 0) ap.y = 0;
					_divSelectElement.style.left = (ap.x + 3) + 'px';
					_divSelectElement.style.top = (ap.y + 3) + 'px';
					_divSelectElement.style.zIndex = JsonDataBinding.getZOrder(sender) + 1;
					_divSelectElement.style.display = 'block';
					_nameresizer.style.display = 'none';
				}
			}
			return;
		}
		function onFontSelected(e) {
			if (_fontSelect.selectedIndex >= 0) {
				if (_editorOptions && (_editorOptions.selectedHtml || _editorOptions.ifrmSelHtml)) {
					if (!_editorOptions.selectedHtml) {
						_editorOptions.selectedHtml = _editorOptions.ifrmSelHtml;
						_editorOptions.ifrmSelHtml = null;
					}
					var fi = _fontSelect.options[_fontSelect.selectedIndex];
					var fn = _editorOptions.elementEditedDoc.createElement('span');
					if (_editorOptions.isEditingBody) {
						var sl = 's' + Math.floor(Math.random() * 65536);
						fn.setAttribute('styleName', sl);
						fn.className = sl;
					}
					if (_fontSelect.selectedIndex > 0) {
						fn.style.fontFamily = fi.value;
						setCSStext(fn, 'font-family', fi.value);
					}
					insertNodeOverSelection(fn, 'span');
					_editorOptions.pageChanged = true;
				}
				else {
					alert('Please make a text selection to apply font');
				}
			}
		}
		function onHeadingSelected(e) {
			if (_editorOptions && _headingSelect.selectedIndex > 0) {
				var p, p2, fi, h, i;
				if (typeof _editorOptions.selectedHtml == 'undefined' || _editorOptions.selectedHtml == null || _editorOptions.selectedHtml.length == 0) {
					captureSelection({ target: _editorOptions.selectedObject }, true);
				}
				p = _editorOptions.selectedObject; // ? _editorOptions.selectedObject : _editorOptions.elementEditedDoc.activeElement;
				if (p) {
					if (p.subEditor) {
						p = p.obj;
					}
				}
				else {
					if (_editorOptions.isEditingBody) {
						p = _editorOptions.elementEditedDoc.activeElement;
					}
					else {
						p = _editorOptions.elementEdited;
					}
				}
				if (typeof _editorOptions.selectedHtml == 'undefined' || _editorOptions.selectedHtml == null || _editorOptions.selectedHtml.length == 0) {
					//make current object a heading
					if (p && p != _editorOptions.elementEdited) {
						var p3 = p.parentNode;
						if (p3) {
							fi = _headingSelect.options[_headingSelect.selectedIndex];
							h = _createElement('h' + fi.value); //_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['h' + fi.value]);
							h.innerHTML = p.innerHTML;
							var p4 = p.nextSibling;
							p3.removeChild(p);
							p2 = HtmlEditor.getParent.apply(_editorOptions.elementEditedWindow, [p3, 'h' + fi.value]);
							if (p2 != p3) {
								var found = false;
								while (p3 && !found) {
									for (i = 0; i < p2.children.length; i++) {
										if (p2.children[i] == p3) {
											found = true;
											break;
										}
									}
									if (!found) {
										p3 = p3.parentNode;
									}
								}
								if (p3) {
									_insertBefore(p2, h, p3); //_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [p2, h, p3]);
								}
								else {
									//_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [p2, h]);
									_appendChild(p2, h);
								}
							}
							else {
								_insertBefore(p2, h, p4); //_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [p2, h, p4]);
							}
							_editorOptions.pageChanged = true;
						}
					}
				}
				else {
					//make selected text into a heading
					if (!p) {
						p = _editorOptions.elementEdited;
					}
					fi = _headingSelect.options[_headingSelect.selectedIndex];
					h = _createElement('h' + fi.value); //_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow,['h' + fi.value]);
					h.innerHTML = _editorOptions.selectedHtml;
					//
					insertNodeOverSelection(h, 'h' + fi.value);
					_editorOptions.pageChanged = true;
					//p2 = HtmlEditor.getParent.apply(_editorOptions.elementEditedWindow, [p, 'h' + fi.value]);
					//if (p2 != p) {
					//	var found = false;
					//	while (p && !found) {
					//		for (i = 0; i < p2.children.length; i++) {
					//			if (p2.children[i] == p) {
					//				found = true;
					//				break;
					//			}
					//		}
					//		if (!found) {
					//			p = p.parentNode;
					//		}
					//	}
					//	if (p) {
					//		_insertBefore(p2, h, p);//_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [p2, h, p]);
					//	}
					//	else {
					//		_appendChild(p2, h);//_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [p2, h]);
					//	}
					//}
					//else {
					//	_appendChild(p, h);//_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [p, h]);
					//}
				}
			}
		}
		function canStopTag() {
			if (_editorOptions) {
				var tag;
				//if (_editorOptions.selectedObject && _editorOptions.selectedObject.tagName) {
				//	tag = _editorOptions.selectedObject.tagName.toLowerCase();
				//	if (tag == 'p' || tag == 'span' || tag == 'div' || tag == 'strong' || tag == 'em' || tag == 'u' || tag == 'sub' || tag == 'sup') {
				//		return true;
				//	}
				//}
				if (_editorOptions.propertiesOwner) {
					var p = _editorOptions.propertiesOwner.subEditor?_editorOptions.propertiesOwner.obj:_editorOptions.propertiesOwner;
					if (p) {
						tag = p.tagName ? p.tagName.toLowerCase() : null;
						if (tag && tag != 'td' && tag != 'tr' && tag != 'tbody' && tag != 'thead' && tag != 'tfoot' && tag != 'th') {
							p = p.parentNode;
							if (p) {
								tag = p.tagName ? p.tagName.toLowerCase() : null;
								if (tag && tag != 'tr' && tag != 'tbody' && tag != 'thead' && tag != 'tfoot' && tag != 'th' && tag != 'table' && tag != 'ul' && tag != 'ol' && tag != 'select' && tag != 'map') {
									return true;
								}
							}
						}
					}	 
				}
			}
			return false;
		}
		function restoreBodyInnerHtml(s) {
			var rb;
			_editorOptions.elementEditedDoc.body.innerHTML = s;
			if (_editorOptions.redbox) {
				_editorOptions.redbox.contentEditable = false;
				rb = _editorOptions.elementEditedDoc.getElementById(_editorOptions.redbox.id);
				if (rb) {
					_editorOptions.redbox = rb;
					_editorOptions.redbox.forProperties = true;
					_editorOptions.redbox.contentEditable = false;
				}
			}
			if (_editorOptions.markers) {
				for (var i = 0; i < _editorOptions.markers.length; i++) {
					if (_editorOptions.markers[i]) {
						_editorOptions.markers[i].contentEditable = false;
						rb = _editorOptions.elementEditedDoc.getElementById(_editorOptions.markers[i].id);
						if (rb) {
							_editorOptions.markers[i] = rb;
							_editorOptions.markers[i].forProperties = true;
							_editorOptions.markers[i].contentEditable = false;
						}
					}
				}
			}
		}
		function undelete() {
			if (_editorOptions && _editorOptions.deleted) {
				//_editorOptions.deleted = { parent: p, obj: c.owner, next:c.owner.nextSibling };
				try {
					if (_editorOptions.deleted.parent && _editorOptions.deleted.parent.parentNode) {
						if (_editorOptions.deleted.obj) {
							_insertBefore(_editorOptions.deleted.parent, _editorOptions.deleted.obj, _editorOptions.deleted.next);
						}
						else if (typeof (_editorOptions.deleted.originalHTML) != 'undefined' && _editorOptions.deleted.originalHTML != null) {
							_editorOptions.deleted.parent.innerHTML = _editorOptions.deleted.originalHTML;
						}
					}
				}
				catch (err) {
				}
				_editorOptions.deleted = null;
				_imgUndel.src = '/libjs/undel_inact.png';
			}
		}
		function undo() {
			if (hasUndo()) {
				if (_editorOptions.undoState.state == UNDOSTATE_UNDO) {
					_editorOptions.undoState.index--;
					restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].undoInnerHTML);
				}
				else if (_editorOptions.undoState.state == UNDOSTATE_REDO) {
					if (_editorOptions.undoState.index == _editorOptions.undoList.length - 1) {
						_editorOptions.undoList[_editorOptions.undoState.index].done = true;
						_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML = _editorOptions.elementEditedDoc.body.innerHTML;
					}
					restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].undoInnerHTML);
					_editorOptions.undoState.state = UNDOSTATE_UNDO;
				}
				showUndoRedo();
			}
		}
		function redo() {
			if (hasRedo()) {
				if (_editorOptions.undoState.state == UNDOSTATE_UNDO) {
					restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML);
					_editorOptions.undoState.state = UNDOSTATE_REDO;
				}
				else if (_editorOptions.undoState.state == UNDOSTATE_REDO) {
					_editorOptions.undoState.index++;
					restoreBodyInnerHtml(_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML);
				}
				showUndoRedo();
			}
		}
		function insSpaceClick() {
			if (canStopTag()) {
				var sp = document.createTextNode("space");
				var e = _editorOptions.propertiesOwner.subEditor ? _editorOptions.propertiesOwner.obj : _editorOptions.propertiesOwner;
				if (e && e.tagName && e.tagName.toLowerCase() == 'body') {
					if (e.children.length > 0) {
						_insertBefore(e, sp, e.children[0]);
					}
					else {
						_insertBefore(e, sp, null);
					}
				}
				else {
					var p = e.parentNode;
					_insertBefore(p, sp, e);
				}
				_editorOptions.pageChanged = true;
			}
		}
		function stopTagClick() {
			if (canStopTag()) {
				var sp = document.createTextNode("space");
				var e = _editorOptions.propertiesOwner.subEditor ? _editorOptions.propertiesOwner.obj : _editorOptions.propertiesOwner;
				if (e && e.tagName && e.tagName.toLowerCase() == 'body') {
					e.appendChild(sp);
				}
				else {
					var p = e.parentNode;
					_insertAfter(p, sp, e);
				}
				_editorOptions.pageChanged = true;
			}
		}
		function insBrClick() {
			if (canStopTag()) {
				var sp = document.createElement("br");
				var e = _editorOptions.propertiesOwner.subEditor ? _editorOptions.propertiesOwner.obj : _editorOptions.propertiesOwner;
				if (e && e.tagName && e.tagName.toLowerCase() == 'body') {
					if (e.children.length > 0) {
						_insertBefore(e, sp, e.children[0]);
					}
					else {
						_insertBefore(e, sp, null);
					}
				}
				else {
					var p = e.parentNode;
					_insertBefore(p, sp, e);
				}
				_editorOptions.pageChanged = true;
			}
		}
		function appBrClick() {
			if (canStopTag()) {
				var sp = document.createElement("br");
				var e = _editorOptions.propertiesOwner.subEditor ? _editorOptions.propertiesOwner.obj : _editorOptions.propertiesOwner;
				if (e && e.tagName && e.tagName.toLowerCase() == 'body') {
					e.appendChild(sp);
				}
				else {
					var p = e.parentNode;
					_insertAfter(p, sp, e);
				}
				_editorOptions.pageChanged = true;
			}
		}
		function insLFClick() {
			var br = _createElement('br');
			insertNodeOverSelection(br, 'br');
		}
		function appTxtClick() {
			if (_editorOptions && _editorOptions.client) {
				_editorOptions.client.appendBodyText.apply(_editorOptions.elementEditedWindow);
				_editorOptions.pageChanged = true;
			}
		}
		function fontCommandColorSelected(c) {
			_colorSelect.style.color = c;
			if (_editorOptions) {
				if (!_editorOptions.isEditingBody && !_editorOptions.selectedHtml && _editorOptions.lastSelHtml) {
					_editorOptions.selectedHtml = _editorOptions.lastSelHtml;
				}
				if (_editorOptions.selectedHtml) {
					var fn = _editorOptions.elementEditedDoc.createElement('span');
					if (_editorOptions.isEditingBody) {
						var sl = 's' + Math.floor(Math.random() * 65536);
						fn.setAttribute('styleName', sl);
						fn.className = sl;
					}
					fn = insertNodeOverSelection(fn, 'span');
					setCSStext(fn, 'color', c); //_colorSelect.value);
					_editorOptions.pageChanged = true;
				}
				else {
					var obj = _editorOptions.propertiesOwner.subEditor ? _editorOptions.propertiesOwner.obj : _editorOptions.propertiesOwner;
					if (!_editorOptions.isEditingBody) {
						if (obj == _editorOptions.elementEdited) {
							return;
						}
					}
					//obj.style.color = c; //_colorSelect.style.backgroundColor;
					//setCSStext(obj, 'color', c); //_colorSelect.value);
					_setElementStyleValue(obj, 'color', 'color', c);
					_editorOptions.pageChanged = true;
				}
			}
		}
		function getSelectedObjByTag(tag) {
			var obj = _editorOptions.selectedObject;
			while (obj) {
				if (obj.tagName && obj.tagName.toLowerCase() == tag) {
					return obj;
				}
				obj = obj.parentNode;
			}
		}
		function getPreviousSibling(obj) {
			if (obj) {
				var p = obj.parentNode;
				if (p) {
					var tag = obj.tagName ? obj.tagName.toLowerCase() : '';
					var ret = null;
					for (var i = 0; i < p.children.length; i++) {
						if (p.children[i] == obj) {
							return ret;
						}
						if (p.children[i].tagName.toLowerCase() == tag) {
							ret = p.children[i];
						}
					}
					return ret;
				}
			}
			return null;
		}
		function getNextSibling(obj) {
			if (obj) {
				var p = obj.parentNode;
				if (p) {
					var tag = obj.tagName ? obj.tagName.toLowerCase() : '';
					for (var i = 0; i < p.children.length; i++) {
						if (p.children[i] == obj) {
							for (var k = i + 1; k < p.children.length; k++) {
								if (p.children[k] && p.children[k].tagName && p.children[k].tagName.toLowerCase() == tag) {
									return p.children[k];
								}
							}
							break;
						}
					}
				}
			}
			return null;
		}
		function LI_indent(li) {
			if (li) {
				var ul0 = li.parentNode;
				var tag = ul0 ? (ul0.tagName ? ul0.tagName.toLowerCase() : null) : null;
				if (tag == 'ul' || tag == 'ol') {
					var preLI = getPreviousSibling(li);
					if (preLI) {
						var uol;
						for (var i = 0; i < preLI.children.length; i++) {
							if (preLI.children[i].tagName.toLowerCase() == tag) {
								uol = preLI.children[i];
								break;
							}
						}
						if (!uol) {
							uol = _createElement(tag); //_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, [tag]);
							_appendChild(preLI, uol); //_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [preLI, uol]);
						}
						_removeChild(ul0, li); //_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [ul0, li]);
						_appendChild(uol, li); //_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [uol, li]);
						return true;
					}
				}
			}
		}
		function LI_dedent(li) {
			if (li) {
				var ul0 = li.parentNode;
				var tag = ul0 ? (ul0.tagName ? ul0.tagName.toLowerCase() : null) : null;
				if (tag == 'ul' || tag == 'ol') {
					var pli = ul0.parentNode;
					if (pli && pli.tagName && pli.tagName.toLowerCase() == 'li') {
						var ul1 = pli.parentNode;
						if (ul1 && ul1.tagName && ul1.tagName.toLowerCase() == tag) {
							var pliNext = getNextSibling(pli);
							_removeChild(ul0, li); //_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [ul0, li]);
							_insertBefore(ul1, li, pliNext); //_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [ul1, li, pliNext]);
							if (ul0.children.length == 0) {
								_removeChild(pli, ul0); //_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [pli, ul0]);
							}
							return true;
						}
					}
				}
			}
		}
		function LI_moveup(li, inEditor) {
			if (li) {
				var ul0 = li.parentNode;
				var tag = ul0 ? (ul0.tagName ? ul0.tagName.toLowerCase() : null) : null;
				if (tag == 'ul') {
					var preLI = getPreviousSibling(li);
					if (preLI) {
						if (inEditor) {
							ul0.removeChild(li);
							ul0.insertBefore(li, preLI);
						}
						else {
							_removeChild(ul0, li); //_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [ul0, li]);
							_insertBefore(ul0, li, preLI); //_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [ul0, li, preLI]);
						}
						return true;
					}
				}
			}
		}
		function LI_movedown(li, inEditor) {
			if (li) {
				var ul0 = li.parentNode;
				var tag = ul0 ? (ul0.tagName ? ul0.tagName.toLowerCase() : null) : null;
				if (tag == 'ul') {
					var nextLI0 = getNextSibling(li);
					if (nextLI0) {
						var nextLI = getNextSibling(nextLI0);
						if (inEditor) {
							ul0.removeChild(li);
							ul0.insertBefore(li, nextLI);
						}
						else {
							_removeChild(ul0, li); //_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [ul0, li]);
							_insertBefore(ul0, li, nextLI); //_editorOptions.client.insertBefore.apply(_editorOptions.elementEditedWindow, [ul0, li, nextLI]);
						}
						return true;
					}
				}
			}
		}
		function showtextAlign(obj0) {
			if (_editorOptions) {
				var v;
				var obj = obj0.subEditor ? obj0.obj : obj0;
				if (_editorOptions.isEditingBody && _editorOptions.client)
					v = _editorOptions.client.getElementStyleValue.apply(_editorOptions.elementEditedWindow, [obj, 'text-align']);
				else
					v = obj.style.textAlign;
				var imgs = _fontCommands.getElementsByTagName('img');
				for (i = 0; i < imgs.length; i++) {
					if (imgs[i].cmd == 'alignLeft') {
						imgs[i].src = v == 'left' ? imgs[i].actSrc : imgs[i].inactSrc;
					}
					else if (imgs[i].cmd == 'alignCenter') {
						imgs[i].src = v == 'center' ? imgs[i].actSrc : imgs[i].inactSrc;
					}
					else if (imgs[i].cmd == 'alignRight') {
						imgs[i].src = v == 'right' ? imgs[i].actSrc : imgs[i].inactSrc;
					}
				}
			}
		}
		function fontCommandSelected(e) {
			if (!_editorOptions) return;
			var c = JsonDataBinding.getSender(e);
			if (c && c.cmd) {
				if (c.cmd == 'indent') {
					var li = getSelectedObjByTag('li');
					if (LI_indent(li)) {
						_editorOptions.pageChanged = true;
					}
					return;
				}
				else if (c.cmd == 'dedent') {
					var li = getSelectedObjByTag('li');
					if (LI_dedent(li)) {
						_editorOptions.pageChanged = true;
					}
					return;
				}
				else if (c.cmd == 'moveup') {
					var li = getSelectedObjByTag('li');
					if (LI_moveup(li)) {
						_editorOptions.pageChanged = true;
					}
					return;
				}
				else if (c.cmd == 'movedown') {
					var li = getSelectedObjByTag('li');
					if (LI_movedown(li)) {
						_editorOptions.pageChanged = true;
					}
					return;
				}
				else if (c.cmd == 'alignLeft') {
					if (_editorOptions.selectedObject) {
						_setElementStyleValue(_editorOptions.selectedObject, 'textAlign', 'text-align', 'left'); //_editorOptions.client.setElementStyleValue.apply(_editorOptions.editorWindow, [_editorOptions.selectedObject, 'textAlign', 'text-align', 'left']);
						showtextAlign(_editorOptions.selectedObject);
						_editorOptions.pageChanged = true;
					}
					return;
				}
				else if (c.cmd == 'alignCenter') {
					if (_editorOptions.selectedObject) {
						_setElementStyleValue(_editorOptions.selectedObject, 'textAlign', 'text-align', 'center'); //_editorOptions.client.setElementStyleValue.apply(_editorOptions.editorWindow, [_editorOptions.selectedObject, 'textAlign', 'text-align', 'center']);
						showtextAlign(_editorOptions.selectedObject);
						_editorOptions.pageChanged = true;
					}
					return;
				}
				else if (c.cmd == 'alignRight') {
					if (_editorOptions.selectedObject) {
						_setElementStyleValue(_editorOptions.selectedObject, 'textAlign', 'text-align', 'right'); //_editorOptions.client.setElementStyleValue.apply(_editorOptions.editorWindow, [_editorOptions.selectedObject, 'textAlign', 'text-align', 'right']);
						showtextAlign(_editorOptions.selectedObject);
						_editorOptions.pageChanged = true;
					}
					return;
				}
				var iurl;
				if (_editorOptions.selectedHtml || (c.cmd == 'createlink' && _editorOptions.selectedObject)) {
					if (typeof c.useUI != 'undefined') {
						if (typeof c.value != 'undefined') {
							_editorOptions.elementEditedDoc.execCommand(c.cmd, c.useUI, c.value);
						}
						else {
							_editorOptions.elementEditedDoc.execCommand(c.cmd, c.useUI);
						}
						captureSelection({ target: _editorOptions.elementEditedDoc.activeElement });
						_editorOptions.pageChanged = true;
					}
					else {
						if (c.cmd == 'createlink') {
							var a;
							a = _createElement('a'); //_editorOptions.elementEditedDoc.createElement('a');
							a.href = '#';
							insertNodeOverSelection(a, 'a');
							if (_editorOptions.isEditingBody) {
								var txt = a.textContent || a.innerText;
								if (txt && txt.length > 0) {
									if (txt.length > 30) {
										txt = txt.substr(0, 30);
									}
									a.name = txt;
									refreshProperties();
								}
							}
							else {
								a.savedhref = prompt('Enter hyperlink URL', '');
								a.target = '_blank';
								_editorOptions.selectedObject = null;
								selectEditElement(a);
							}
						}
						else {
							if (_isFireFox) {
								_editorOptions.elementEditedDoc.execCommand(c.cmd, null, false);
							}
							else {
								if (_isOpera) {
									if (_editorOptions.selectedObject) _editorOptions.selectedObject.contentEditable = true;
								}
								_editorOptions.elementEditedDoc.execCommand(c.cmd);
							}
							captureSelection({ target: _editorOptions.elementEditedDoc.activeElement });
						}
						_editorOptions.pageChanged = true;
					}
				}
				else {
					if (typeof c.useUI == 'undefined') {
						if (c.cmd == 'createlink') {
							alert('Please select text to be used for hyper-text');
						}
						else if (_editorOptions.selectedObject) {
							try {
								var fw;
								if (c.cmd == 'bold') {
									if (_editorOptions.selectedObject.tagName && _editorOptions.selectedObject.tagName.toLowerCase() == 'strong') {
										iurl = c.inactSrc;
										stripTag({ target: { owner: _editorOptions.selectedObject} });
									}
									else {
										if (_editorOptions.selectedObject.style.fontWeight == 'bold') {
											fw = '';
										}
										else {
											fw = 'bold'
										}
										_editorOptions.selectedObject.style.fontWeight = fw;
									}
								}
								else if (c.cmd == 'italic') {
									if (_editorOptions.selectedObject.tagName && _editorOptions.selectedObject.tagName.toLowerCase() == 'em') {
										iurl = c.inactSrc;
										stripTag({ target: { owner: _editorOptions.selectedObject} });
									}
									else {
										fw = _editorOptions.selectedObject.style.fontStyle;
										if (fw == 'italic' || fw == 'oblique') {
											fw = '';
										}
										else {
											fw = 'italic'
										}
										_editorOptions.selectedObject.style.fontStyle = fw;
									}
								}
								else if (c.cmd == 'underline') {
									if (_editorOptions.selectedObject.tagName && _editorOptions.selectedObject.tagName.toLowerCase() == 'u') {
										iurl = c.inactSrc;
										stripTag({ target: { owner: _editorOptions.selectedObject} });
									}
									else {
										fw = _editorOptions.selectedObject.style.textDecoration;
										if (fw == 'underline') {
											fw = '';
										}
										else {
											fw = 'underline'
										}
										_editorOptions.selectedObject.style.textDecoration = fw;
									}
								}
								else if (c.cmd == 'strikethrough') {
									if (_editorOptions.selectedObject.tagName && _editorOptions.selectedObject.tagName.toLowerCase() == 'strike') {
										iurl = c.inactSrc;
										stripTag({ target: { owner: _editorOptions.selectedObject} });
									}
									else {
										fw = _editorOptions.selectedObject.style.textDecoration;
										if (fw == 'line-through') {
											fw = '';
										}
										else {
											fw = 'line-through'
										}
										_editorOptions.selectedObject.style.textDecoration = fw;
									}
								}
								else if (c.cmd == 'subscript') {
									if (_editorOptions.selectedObject.tagName && _editorOptions.selectedObject.tagName.toLowerCase() == 'sub') {
										iurl = c.inactSrc;
										stripTag({ target: { owner: _editorOptions.selectedObject} });
									}
								}
								else if (c.cmd == 'superscript') {
									if (_editorOptions.selectedObject.tagName && _editorOptions.selectedObject.tagName.toLowerCase() == 'sup') {
										iurl = c.inactSrc;
										stripTag({ target: { owner: _editorOptions.selectedObject} });
									}
								}
								if (fw || fw == '') {
									if (fw == '') {
										iurl = c.inactSrc;
									}
									else {
										iurl = c.actSrc;
									}
									_editorOptions.styleChanged = true;
									showProperties(_editorOptions.selectedObject);
									showFontCommands();
								}
								_editorOptions.pageChanged = true;
							}
							catch (err) {
								alert('Error execting operator "' + c.cmd + '". ' + err.message);
							}
						}
					}
				}
				if (iurl) {
					c.src = iurl;
				}
				else {
					if (!_isFireFox) {
						if (_editorOptions.elementEditedDoc.queryCommandState(c.cmd)) {
							c.src = c.actSrc;
						}
						else {
							c.src = c.inactSrc;
						}
					}
				}
			}
		}
		function refreshProperties() {
			if (_editorOptions && _editorOptions.selectedObject) {
				var c = _editorOptions.selectedObject;
				_editorOptions.selectedObject = null;
				selectEditElement(c);
			}
		}
		function stripTag(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var node = c.owner;
				var parent = node.parentNode;
				var tag = (node.tagName ? node.tagName.toLowerCase() : '');
				if (tag == 'table' || tag == 'select' || tag == 'optgroup' || tag == 'option') {
					parent.removeChild(node);
				}
				else {
					while (node.firstChild) {
						parent.insertBefore(node.firstChild, node);
					}
					parent.removeChild(node);
				}
				selectEditElement(parent);
				if (_locatorList) {
					_locatorList.options.length = 0;
				}
			}
		}
		function changeHeading(e, htag) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var node = c.owner;
				var h = _editorOptions.elementEditedDoc.createElement(htag);
				var parent = node.parentNode;
				parent.insertBefore(h, node);
				while (node.firstChild) {
					h.appendChild(node.firstChild);
				}
				parent.removeChild(node);
				selectEditElement(h);
			}
		}
		function gotoChildByTag(e) {
			var c0 = JsonDataBinding.getSender(e);
			if (c0 && c0.owner && c0.propDesc) {
				var node = c0.owner;
				var cs = node.getElementsByTagName(c0.propDesc.name);
				var c;
				if (cs && cs.length > 0) {
					c = cs[0];
				}
				else {
					if (!c0.propDesc.notcreate) {
						c = _editorOptions.elementEditedDoc.createElement(c0.propDesc.name);
						node.appendChild(c);
					}
					else {
						c = c0;
					}
				}
				selectEditElement(c);
			}
		}
		function moveOutTag(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var node = c.owner;
				var parent = node.parentNode;
				if (parent != _editorOptions.elementEditedDoc.body) {
					var p = parent.parentNode;
					if (p) {
						parent.removeChild(node);
						p.insertBefore(node, parent.nextSibling);
						_editorOptions.selectedObject = null;
						selectEditElement(node);
					}
				}
			}
		}
		function getContainer(obj) {
			var p = obj.parentNode;
			if (obj.tagName) {
				var tn = obj.tagName.toLowerCase();
				if (tn == 'td' || tn == 'tr' || tn == 'th') {
					while (p) {
						if (p.tagName) {
							tn = p.tagName.toLowerCase();
							if (tn == 'table') {
								return p;
							}
						}
						p = p.parentNode;
					}
				}
				if (p.tagName) {
					tn = p.tagName.toLowerCase();
					while (tn == 'p' && p) {
						p = p.parentNode;
						if (p.tagName) {
							tn = p.tagName.toLowerCase();
						}
					}
				}
			}
			return p;
		}
		function getObjectAbsoluteLocation(obj) {
			if (obj && obj.style) {
				if (obj.style.position == 'absolute') {
					return { x: obj.offsetLeft, y: obj.offsetTop };
				}
				else {
					var objP = getContainer(obj);
					if (objP) {
						var p = getObjectAbsoluteLocation(objP);
						if (p) {
							return { x: obj.offsetLeft + p.x, y: obj.offsetTop + p.y };
						}
						else {
							return { x: obj.offsetLeft, y: obj.offsetTop };
						}
					}
					else {
						return { x: obj.offsetLeft, y: obj.offsetTop };
					}
				}
			}
		}
		function getParentDesignObj(obj) {
			if (obj.jsData && obj.jsData.createSubEditor) {
				var ed = obj.jsData.createSubEditor(obj, obj);
				if (ed) {
					return ed;
				}
			}
			var tag = obj.tagName.toLowerCase();
			if (tag == 'li') {
				var p = obj.parentNode;
				while (p) {
					if (p.jsData) {
						if (p.jsData.typename == 'treeview') {
							return createTreeViewItemPropDescs(p, obj);
						}
					}
					p = p.parentNode;
				}
			}
			return obj;
		}
		function showSelectionMark(obj) {
			if (!_editorOptions) return;
			var ap;
			var c = obj.subEditor ? obj.obj : obj;
			ap = _editorOptions.client.getElementPosition.apply(_editorOptions.editorWindow, [c]);
			if (ap) {
				if (_editorOptions.redbox && !_editorOptions.redbox.parentNode) {
					_appendChild(_editorOptions.elementEditedDoc.body, _editorOptions.redbox);
				}
				_editorOptions.redbox.contentEditable = false;
				_editorOptions.redbox.style.left = ap.x + 'px';
				_editorOptions.redbox.style.top = ap.y + 'px';
				if (obj == _editorOptions.elementEditedDoc.body) {
					_editorOptions.redbox.style.display = 'none';
				}
				else {
					_editorOptions.redbox.style.width = 8;
					_editorOptions.redbox.style.height = 8;
					_editorOptions.redbox.style.display = 'block';
					_editorOptions.redbox.style.zIndex = _editorOptions.client.getPageZIndex.apply(_editorOptions.editorWindow, [c]) + 1;
				}
			}
			_parentList.options.length = 0;
			var tab = '';
			var p = c;
			var jsData;
			while (p) {
				jsData = p.jsData;
				if (jsData)
					break;
				p = p.parentNode;
			}
			p = obj;
			while (p) {
				if (p.tagName && p.tagName.toLowerCase() != 'doctype') {
					if (p.hideParent || (jsData && jsData.editable && !jsData.editable(p))) {
						p = p.parentNode;
						continue;
					}
					var item = document.createElement('option');
					item.objvalue = getParentDesignObj(p);
					item.text = elementToString(item.objvalue);
					item.selected = false;
					addOptionToSelect(_parentList, item);
					item.selected = false;
					tab += '..';
					if (p.jsData && p.jsData.isTop) {
						break;
					}
					p = p.parentNode;
				}
				else if (p.subEditor) {
					if (p.hideParent ) {
						p = p.parentNode;
						continue;
					}
					var item = document.createElement('option');
					item.objvalue = p;
					item.text = elementToString(item.objvalue);
					item.selected = false;
					addOptionToSelect(_parentList, item);
					item.selected = false;
					if (p.isTop)
						break;
					p = p.obj.parentNode;
				}
				else {
					break;
				}
			}
			_parentList.size = _parentList.options.length;
			_parentList.selectedIndex = -1;
			for (var i = 0; i < _parentList.options.length; i++) {
				_parentList.options[i].selected = false;
			}
		}
		function getColumnCount(rowHolder) {
			var cn = 0;
			if (rowHolder.rows.length > 0) {
				for (var r = 0; r < rowHolder.rows.length; r++) {
					if (rowHolder.rows[r].cells.length > 0) {
						var c0 = 0;
						for (var c = 0; c < rowHolder.rows[r].cells.length; c++) {
							c0 += (rowHolder.rows[r].cells[c].colSpan ? rowHolder.rows[r].cells[c].colSpan : 1);
						}
						if (c0 > cn)
							cn = c0;
					}
				}
			}
			return cn;
		}
		function getRowCount(rowHolder) {
			return rowHolder.rows.length;
		}
		function mapGridColumnIndexToTableCellIndex(gridCellIndex, row) {
			var ci = 0;
			for (var c = 0; c < row.cells.length; c++) {
				ci += row.cells[c].colSpan ? row.cells[c].colSpan : 1;
				if (ci > gridCellIndex) {
					return c;
				}
			}
			return -1;
		}
		function getVirtualRowPosition(map, tr){
			for (var r = 0; r < map.length; r++) {
				for (var c = 0; c < map[r].length; c++) {
					if (map[r][c].parentNode == tr) {
						return { r: r, c: c };
					}
				}
			}
		}
		function getVirtualCellPosition(map, td) {
			for (var r = 0; r < map.length; r++) {
				for (var c = 0; c < map[r].length; c++) {
					if (map[r][c] == td) {
						return { r: r, c: c };
					}
				}
			}
		}
		function getVirtualColumnIndex(td, maprow) {
			for (var c = 0; c < maprow.length; c++) {
				if (maprow[c] == td) {
					return c;
				}
			}
			return -1;
		}
		function getMapRowSpan(map, r) {
			var ri = 0;
			for (var c = 0; c < map[r].length; c++) {
				if ((map[r][c].rowSpan ? map[r][c].rowSpan : 1) > ri) {
					ri = (map[r][c].rowSpan ? map[r][c].rowSpan : 1);
				}
			}
			return ri;
		}
		function getNextVirtualColumnIndex(td, maprow) {
			var f;
			for (var c = 0; c < maprow.length; c++) {
				if (f) {
					if (maprow[c] != td) {
						return c;
					}
				}
				else if (maprow[c] == td) {
					f = true;
				}
			}
			return -1;
		}
		//rowHolder:table body, header or footer
		//returns a 2-dimentional array holding all cells. for colSpan >1 or rowSpan>1, cells are repeated 
		function _getTableMap(rowHolder) {
			var vrCount = getRowCount(rowHolder);
			var vcCount = getColumnCount(rowHolder);
			//create grid with null cells
			var vcells = new Array(vrCount);
			var r, c;
			for (r = 0; r < vrCount; r++) {
				vcells[r] = new Array(vcCount);
				for (c = 0; c < vcCount; c++) {
					vcells[r][c] = null;
				}
			}
			//go through each row and column
			for (r = 0; r < rowHolder.rows.length; r++) {
				var ci = 0;
				for (c = 0; c < rowHolder.rows[r].cells.length; c++) {
					//assign the cell to the grid
					var cr = typeof rowHolder.rows[r].cells[c].colSpan != 'undefined' ? (rowHolder.rows[r].cells[c].colSpan > 0 ? rowHolder.rows[r].cells[c].colSpan : 1) : 1;
					var rr = typeof rowHolder.rows[r].cells[c].rowSpan != 'undefined' ? (rowHolder.rows[r].cells[c].rowSpan > 0 ? rowHolder.rows[r].cells[c].rowSpan : 1) : 1;
					//ci advanced due to row span previous row filled
					while (ci < vcCount && vcells[r][ci] != null) {
						ci++;
					}
					//fix invalid colspan
					if (ci + cr + (rowHolder.rows[r].cells.length - c - 1) > vcCount) {
						cr = vcCount - ci - (rowHolder.rows[r].cells.length - c - 1);
						setColSpan(rowHolder.rows[r].cells[c], cr);
					}
					//fix invalid rowspan
					if (rr + r > vrCount) {
						rr = vrCount - r;
						setRowSpan(rowHolder.rows[r].cells[c], rr);
					}
					//assign the cell to grid cells
					for (var i = ci; i < ci + cr; i++) { //assign to all spanned cells
						for (var j = r; j < r + rr && j < vcells.length; j++) { //assign to all spanned rows
							if (i < vcells[j].length) {
								vcells[j][i] = rowHolder.rows[r].cells[c];
							}
						}
					}
					ci += cr;
				}
			}
			return vcells;
		}
		function getTDbyVirtualPosition(rowHolder, vr, vc, vrCount, vcCount) {
			if (!vrCount) vrCount = getRowCount(rowHolder);
			if (!vcCount) vcCount = getColumnCount(rowHolder);
			if (vrCount > 0 && vcCount > 0) {
				var vrc = 0;
				for (var r = 0; r < vrCount; r++) {
					if (rowHolder.rows[r].cells.length > 0) {
						vrc += rowHolder.rows[r].cells[0].rowSpan ? rowHolder.rows[r].cells[0].rowSpan : 1;
					}
					else {
						vrc += 1;
					}
					if (vrc >= vr) {
						var row = rowHolder.rows[r];
						var vcc = 0;
						for (var c = 0; c < row.cells.length; c++) {
							vcc += row.cells[c].colSpan ? row.cells[c].colSpan : 1;
							if (vcc >= vc) {
								return row.cells[c];
							}
						}

					}
				}
			}
			return null;
		}
		function _verifyTableStruct(tbl) {
			var n,r,c,colCount;
			var ncolCount = 0;
			for (n = 0; n < tbl.children.length; n++) {
				if (tbl.children[n].rows) {
					for (r = 0; r < tbl.children[n].rows.length; r++) {
						if (tbl.children[n].rows[r].cells && tbl.children[n].rows[r].cells.length > 0) {
							colCount = 0;
							for (c = 0; c < tbl.children[n].rows[r].cells.length; c++) {
								colCount += (tbl.children[n].rows[r].cells[c].colSpan?tbl.children[n].rows[r].cells[c].colSpan:1);
							}
							if (colCount > ncolCount)
								ncolCount = colCount;
						}
					}
				}
			}
			if (ncolCount > 0) {
				for (n = 0; n < tbl.children.length; n++) {
					if (tbl.children[n].rows) {
						for (r = 0; r < tbl.children[n].rows.length; r++) {
							if (tbl.children[n].rows[r].cells && tbl.children[n].rows[r].cells.length>0) {
								colCount = 0;
								for (c = 0; c < tbl.children[n].rows[r].cells.length; c++) {
									colCount += (tbl.children[n].rows[r].cells[c].colSpan ? tbl.children[n].rows[r].cells[c].colSpan : 1);
								}
								if (colCount < ncolCount) {
									colCount = ncolCount - colCount;
									var n1=tbl.children[n].rows[r].cells && tbl.children[n].rows[r].cells.length-1;
									tbl.children[n].rows[r].cells[n1].colSpan = (tbl.children[n].rows[r].cells[n1].colSpan?tbl.children[n].rows[r].cells[n1].colSpan:1) + colCount;
								}
							}
						}
					}
				}
			}
		}
		function _refreshProperties() {
			if (_editorOptions) {
				if (_editorOptions.propertiesOwner && _editorOptions.propertiesOwner.tagName && _editorOptions.propertiesOwner.tagName.toLowerCase() == 'html') {
					showProperties(_editorOptions.propertiesOwner);
				}
			}
		}
		function _redisplayProperties() {
			if (_editorOptions && _editorOptions.propertiesOwner) {
				showProperties(_editorOptions.propertiesOwner);
			}
		}
		function _togglePropertiesWindow() {
			if (_divProp.expanded) {
				_divProp.fullHeight = _divProp.style.height;
				_tableProps.style.display = 'none';
				_editorDiv.style.display = 'none';
				if (_divSelectElement) {
					hideSelector();
				}
				_divProp.style.height = (_titleTable.offsetHeight /*+ (_isIE?5:0)*/) + 'px';
				_resizer.style.display = 'none';
				_nameresizer.style.display = 'none';
				_imgExpand.src = '/libjs/expand_res.png';
			}
			else {
				_divProp.style.height = _divProp.fullHeight;
				_tableProps.style.display = '';
				_editorDiv.style.display = '';
				_imgExpand.src = '/libjs/expand_min.png';
			}
			_divProp.expanded = !_divProp.expanded;
			_showResizer();
			if (_divProp.expanded) {
				adjustSizes();
			}
		}
		//editing types
		var EDIT_NONE = 0;
		var EDIT_TEXT = 1;
		var EDIT_NUM = 2;
		var EDIT_BOOL = 3;
		var EDIT_COLOR = 4;
		var EDIT_ENUM = 5;
		var EDIT_PARENT = 6;
		var EDIT_CMD = 7;
		var EDIT_CHILD = 8;
		var EDIT_PROPS = 9;
		var EDIT_CMD2 = 10;
		var EDIT_CMD3 = 11;
		var EDIT_MENUM = 12;
		var EDIT_DEL = 13;
		var EDIT_NODES = 14;
		var EDIT_GO = 15;
		var EDIT_CUST = 16;
		//property categories 
		var propCatColors;
		var PROP_BK = 1, PROP_BK_COLOR = '#FAFAFF';
		var PROP_BORDER = 2, PROP_BORDER_COLOR = '#FAFAFA';
		var PROP_BOX = 3, PROP_BOX_COLOR = '#FFFAFA'; //includes Dimension 
		var PROP_FONT = 4, PROP_FONT_COLOR = '#e2f7f1';
		var PROP_MARGIN = 5, PROP_MARGIN_COLOR = '#F0FFFF'; //include Padding
		var PROP_MULCOL = 6, PROP_MULCOL_COLOR = '#F5F5DC';
		var PROP_POS = 7, PROP_POS_COLOR = '#FAFFFA';
		var PROP_TEXT = 8, PROP_TEXT_COLOR = '#F5F5F5';
		var PROP_GENERAL = 9, PROP_GENERAL_COLOR = '#F0FFFF';
		var PROP_SIZELOC = 10, PROP_SIZELOC_COLOR = '#F5FFF0';
		var PROP_PAGECLASSES = 11, PROP_PAGECLASSES_COLOR = '#F0F8FF';
		var PROP_CUST1=12,	PROP_CUST1_COLOR='#FFFEEB';
		//
		function getPropCatColor(cat) {
			if (!propCatColors) {
				propCatColors = [PROP_BK_COLOR, PROP_BORDER_COLOR, PROP_BOX_COLOR, PROP_FONT_COLOR, PROP_MARGIN_COLOR, PROP_MULCOL_COLOR, PROP_POS_COLOR, PROP_TEXT_COLOR, PROP_GENERAL_COLOR, PROP_SIZELOC_COLOR, PROP_PAGECLASSES_COLOR, PROP_CUST1_COLOR];
			}
			return propCatColors[cat - 1];
		}
		function _showEditor(display) {
			_divProp.style.display = display;
		}
		function onselChangeForProperties(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner && c.selectedIndex >= 0) {
				var op = c.owner.options[c.selectedIndex];
				if (op) {
					selectEditElement(op);
				}
			}
		}
		function showListItems(sel, trCmd) {
			if (_isFireFox || _isOpera) return;
			var op0;
			while (sel) {
				if (sel.tagName) {
					if (sel.tagName.toLowerCase() == 'option') {
						op0 = sel;
					}
					else if (sel.tagName.toLowerCase() == 'select') {
						break;
					}
				}
				sel = sel.parentNode;
			}
			if (sel) {
				var sel2 = document.createElement('select');
				sel2.size = 1;
				trCmd.appendChild(sel2);
				var selIdx = -1;
				for (var i = 0; i < sel.options.length; i++) {
					var op = document.createElement('option');
					op.text = sel.options[i].text;
					if (sel.options[i] == op0) {
						op.selected = false;
					}
					addOptionToSelect(sel2, op);
				}
				sel2.owner = sel;
				sel2.selectedIndex = selIdx;
				JsonDataBinding.AttachEvent(sel2, "onchange", onselChangeForProperties);
			}
		}
		function deleteElement(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var p = c.owner.parentNode;
				if (p) {
					var o = c.owner;
					if (p.tagName == 'a' || p.tagName == 'A') {
						if (p.parentNode && p.parentNode.tagName && p.parentNode.tagName.toLowerCase() == 'svg') {
							o = p;
							p = p.parentNode;
						}
					}
					if (_editorOptions) {
						_editorOptions.deleted = { parent: p, obj: o, next:o.nextSibling };
					}
					//var undoItem = { guid: 'body', undoInnerHTML: _editorOptions.elementEditedDoc.body.innerHTML };
					p.removeChild(o);
					selectEditElement(p);
					//undoItem.redoInnerHTML = _editorOptions.elementEditedDoc.body.innerHTML;
					//undoItem.done = true;
					//addUndoItem(undoItem);
					_imgUndel.src = '/libjs/undel.png';
					if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
				}
			}
		}
		function gotoElement(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				_editorOptions.selectedObject = null;
				selectEditElement(c.owner);
			}
		}
		function gotoPageHead(e) {
			for (var i = 0; i < _parentList.options.length; i++) {
				var obj = _parentList.options[i].objvalue;
				if (obj && obj.tagName && obj.tagName.toLowerCase() == 'head') {
					_parentList.selectedIndex = i;
					showProperties(obj);
					return;
				}
			}
			gotoChildByTag(e);
		}
		function getWebAddress(h) {
			if (_editorOptions && _editorOptions.elementEditedWindow) {
				var u = _editorOptions.client.getPageAddress.apply(_editorOptions.elementEditedWindow, [false]);
				return u;
			}
		}
		function getMetaData(h, name) {
			var head = h.getElementsByTagName('head')[0];
			var mts = head.getElementsByTagName('meta');
			if (mts) {
				name = name.toLowerCase();
				for (var i = 0; i < mts.length; i++) {
					var nm;
					if (_isIE) {
						nm = mts[i].getAttribute('Name');
					}
					else {
						nm = mts[i].getAttribute('name');
					}
					if (nm) nm = nm.toLowerCase();
					if (nm == name) {
						return mts[i].getAttribute('content');
					}
				}
			}
		}
		function setContentNoCache(h) {
			var head = h.getElementsByTagName('head')[0];
			var mts = head.getElementsByTagName('meta');
			if (mts) {
				for (var i = 0; i < mts.length; i++) {
					var nm = mts[i].getAttribute('http-equiv');
					if (typeof nm != 'undefined' && nm != null && nm.toLowerCase() == 'pragma') {
						mts[i].setAttribute('content', 'NO-CACHE');
						return;
					}
					else {
						nm = mts[i].getAttribute('content');
						if (nm && nm.toLowerCase() == 'no-cache') {
							return;
						}
					}
				}
			}
			var m = _createElement('meta');
			_appendChild(head, m);
			m.setAttribute('http-equiv', 'PRAGMA');
			m.setAttribute('content', 'NO-CACHE');
		}
		function setContentType(h, val) {
			var head = h.getElementsByTagName('head')[0];
			var mts = head.getElementsByTagName('meta');
			if (mts) {
				for (var i = 0; i < mts.length; i++) {
					var nm = mts[i].getAttribute('http-equiv');
					if (typeof nm != 'undefined' && nm != null && nm.toLowerCase() == 'content-type') {
						mts[i].setAttribute('content', val);
						return;
					}
					else {
						nm = mts[i].getAttribute('content');
						if (nm && nm.toLowerCase() == 'text/html; charset=utf-8') {
							return;
						}
					}
				}
			}
			var m = _createElement('meta');
			_appendChild(head, m);
			m.setAttribute('http-equiv', 'Content-Type');
			m.setAttribute('content', val);
		}
		function setIECompatible(h) {
			var head = h.getElementsByTagName('head')[0];
			var mts = head.getElementsByTagName('meta');
			if (mts) {
				for (var i = 0; i < mts.length; i++) {
					var nm = mts[i].getAttribute('http-equiv');
					if (typeof nm != 'undefined' && nm != null && nm.toLowerCase() == 'x-ua-compatible') {
						mts[i].setAttribute('content', 'IE=edge');
						return;
					}
					else {
						nm = mts[i].getAttribute('content');
						if (nm && nm.toLowerCase() == 'ie=edge') {
							return;
						}
					}
				}
			}
			var m = _createElement('meta');
			_appendChild(head, m);
			m.setAttribute('http-equiv', 'X-UA-Compatible');
			m.setAttribute('content', 'IE=edge');
		}
		function setMetaData(h, name, val) {
			name = name.toLowerCase();
			var head = h.getElementsByTagName('head')[0];
			var mts = head.getElementsByTagName('meta');
			if (mts) {
				for (var i = 0; i < mts.length; i++) {
					var nm;
					if (_isIE) {
						nm = mts[i].getAttribute('Name') || mts[i].name;
					}
					else {
						nm = mts[i].getAttribute('name');
					}
					if (nm) nm = nm.toLowerCase();
					if (nm == name) {
						mts[i].setAttribute('content', val);
						return;
					}
				}
			}
			var m = _createElement('meta');
			_appendChild(head, m);
			if (_isIE) {
				m.setAttribute('Name', name);
				m.name = name;
			}
			else {
				m.setAttribute('name', name);
			}
			m.setAttribute('content', val);
		}
		function addmeta(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var head = c.owner;
				var m = _createElement('meta');
				_appendChild(head, m);
				selectEditElement(m);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function addlink(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var head = c.owner;
				var m = _createElement('link');
				_appendChild(head, m);
				selectEditElement(m);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function addcss(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var head = c.owner;
				var m = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['link']);
				_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [head, m]);
				m.setAttribute('rel', 'stylesheet');
				m.setAttribute('type', 'text/css');
				selectEditElement(m);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function addscript(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var head = c.owner;
				var m = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['script']);
				_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [head, m]);
				selectEditElement(m);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function addOptionGroup(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var sel = c.owner;
				while (sel) {
					if (sel.tagName) {
						if (sel.tagName.toLowerCase() == 'select') {
							break;
						}
					}
					sel = sel.parentNode;
				}
				if (sel) {
					var op = _createElement('optgroup'); //_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['optgroup']);
					op.label = 'option group';
					_appendChild(sel, op); //_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [sel, op]);
					selectEditElement(op);
				}
			}
		}
		function addpolygon(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var polygon = _createSVGElement('polygon');
					a.appendChild(polygon);
					polygon.setAttribute('points', '2,2 60,2 65,10 30,30');
					polygon.style.fill = 'yellow';
					selectEditElement(polygon);
				}
			}
		}
		function addpolyline(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var polygon = _createSVGElement('polyline');
					a.appendChild(polygon);
					polygon.setAttribute('points', '2,2 60,2 65,10 30,30');
					polygon.setAttribute('stroke', 'black');
					polygon.setAttribute('stroke-width', '1');
					polygon.style.fill = 'none';
					selectEditElement(polygon);
				}
			}
		}
		function addrect(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var rect = _createSVGElement('rect');
					a.appendChild(rect);
					rect.setAttribute('x', '2');
					rect.setAttribute('y', '2');
					rect.setAttribute('width', '100');
					rect.setAttribute('height', '30');
					rect.style.fill = 'blue';
					selectEditElement(rect);
				}
			}
		}
		function addtext(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var rect = _createSVGElement('text');
					a.appendChild(rect);
					rect.setAttribute('x', '50');
					rect.setAttribute('y', '50');
					rect.textContent = 'Hello World!';
					selectEditElement(rect);
				}
			}
		}
		function addcircle(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var rect = _createSVGElement('circle');
					a.appendChild(rect);
					rect.setAttribute('cx', '60');
					rect.setAttribute('cy', '60');
					rect.setAttribute('r', '50');
					rect.style.fill = 'green';
					selectEditElement(rect);
				}
			}
		}
		function addellipse(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var rect = _createSVGElement('ellipse');
					a.appendChild(rect);
					rect.setAttribute('cx', '60');
					rect.setAttribute('cy', '60');
					rect.setAttribute('rx', '50');
					rect.setAttribute('ry', '30');
					rect.style.fill = 'green';
					selectEditElement(rect);
				}
			}
		}
		function addline(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var svg = c.owner;
				if (svg) {
					var a = _createSVGElement('a');
					a.hideParent = true;
					svg.appendChild(a);
					var rect = _createSVGElement('line');
					a.appendChild(rect);
					rect.setAttribute('x1', '10');
					rect.setAttribute('y1', '10');
					rect.setAttribute('x2', '50');
					rect.setAttribute('y2', '50');
					rect.setAttribute('stroke', 'black');
					rect.setAttribute('stroke-width', '1');
					selectEditElement(rect);
				}
			}
		}
		function moveshapeleft(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var shape = c.owner;
				if (shape) {
					if (shape.tagName && shape.tagName.toLowerCase() == 'svg') {
						shape.style.position = 'absolute';
						var rect = shape.getBoundingClientRect()
						shape.style.left = (rect.left - HtmlEditor.svgshapemovegap) + 'px';
					}
					else if (shape.jsData) {
						shape.jsData.moveleft();
					}
				}
			}
		}
		function moveshaperight(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var shape = c.owner;
				if (shape) {
					if (shape.tagName && shape.tagName.toLowerCase() == 'svg') {
						shape.style.position = 'absolute';
						var rect = shape.getBoundingClientRect()
						shape.style.left = (rect.left + HtmlEditor.svgshapemovegap) + 'px';
					}
					else if (shape.jsData) {
						shape.jsData.moveright();
					}
				}
			}
		}
		function moveshapeup(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var shape = c.owner;
				if (shape) {
					if (shape.tagName && shape.tagName.toLowerCase() == 'svg') {
						shape.style.position = 'absolute';
						var rect = shape.getBoundingClientRect()
						shape.style.top = (rect.top - HtmlEditor.svgshapemovegap) + 'px';
					}
					else if (shape.jsData) {
						shape.jsData.moveup();
					}
				}
			}
		}
		function moveshapedown(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var shape = c.owner;
				if (shape) {
					if (shape.tagName && shape.tagName.toLowerCase() == 'svg') {
						shape.style.position = 'absolute';
						var rect = shape.getBoundingClientRect()
						shape.style.top = (rect.top + HtmlEditor.svgshapemovegap) + 'px';
					}
					else if (shape.jsData) {
						shape.jsData.movedown();
					}
				}
			}
		}
		function addOption(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var sel = c.owner;
				var og;
				while (sel) {
					if (sel.tagName) {
						if (sel.tagName.toLowerCase() == 'select') {
							break;
						}
						if (sel.tagName.toLowerCase() == 'optgroup') {
							og = sel;
							break;
						}
					}
					sel = sel.parentNode;
				}
				if (sel || og) {
					var op = _createElement('option'); //_editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['option']);
					if (og) {
						sel = og.parentNode;
						_appendChild(og, op); //_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [og, op]);
						if (sel && sel.tagName && sel.tagName.toLowerCase() == 'select') {
							op.text = 'new option ' + sel.options.length;
						}
						else {
							op.text = 'new option ' + op.childNodes.length;
						}
						op.value = op.text;
					}
					else {
						op.text = 'new option ' + sel.options.length;
						op.value = op.text;
						addOptionToSelect(sel, op);
					}
					selectEditElement(op);
				}
			}
		}
		function addDefinition(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var dl = c.owner;
				var dt = _editorOptions.elementEditedDoc.createElement('dt');
				dt.innerHTML = 'new definition';
				dl.appendChild(dt);
				var dd = _editorOptions.elementEditedDoc.createElement('dd');
				dd.innerHTML = 'new description';
				dl.appendChild(dd);
			}
		}
		function addObjectParameter(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var obj = c.owner;
				var pa = _editorOptions.elementEditedDoc.createElement('param');
				var nm = 'p' + Math.floor(Math.random() * 65536);
				pa.name = nm;
				pa.id = nm;
				obj.appendChild(pa);
				selectEditElement(pa);
			}
		}
		function addtheader(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var tbl = c.owner;
				var h;
				var tag;
				var tbd;
				for (var i = 0; i < tbl.children.length; i++) {
					tag = tbl.children[i].tagName.toLowerCase();
					if (tag == 'tbody')
						tbd = tbl.children[i];
					else if (tag == 'thead') {
						h = tbl.children[i];
						break;
					}
					else if (tag == 'tfoot') {
						if (!tbd)
							tbd = tbl.children[i];
					}
				}
				if (!h && tbd) {
					var map = _getTableMap(tbd);
					if (map.length > 0) {
						var cells = map[0];
						h = _editorOptions.elementEditedDoc.createElement('thead');
						tbl.appendChild(h);
						var tr = _editorOptions.elementEditedDoc.createElement('tr');
						h.appendChild(tr);
						if (cells) {
							var lastTD = null;
							for (var ci = 0; ci < cells.length; ci++) {
								if (lastTD != cells[ci]) {
									lastTD = cells[ci];
									var th = _editorOptions.elementEditedDoc.createElement('th');
									th.innerHTML = "Column " + (ci + 1);
									setColSpan(th, cells[ci].colSpan);
									tr.appendChild(th);
								}
							}
						}
					}
				}
				if (h) {
					selectEditElement(h);
					//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
				}
			}
		}
		function addtfooter(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var tbl = c.owner;
				var h;
				var tag;
				var tbd;
				for (var i = 0; i < tbl.children.length; i++) {
					tag = tbl.children[i].tagName.toLowerCase();
					if (tag == 'tbody')
						tbd = tbl.children[i];
					else if (tag == 'thead') {
						if (!tbd)
							tbd = tbl.children[i];
					}
					else if (tag == 'tfoot') {
						h = tbl.children[i];
						break;
					}
				}
				if (!h && tbd) {
					var map = _getTableMap(tbd);
					if (map.length > 0) {
						var cells = map[map.length-1];
						h = _editorOptions.elementEditedDoc.createElement('tfoot');
						tbl.appendChild(h);
						var tr = _editorOptions.elementEditedDoc.createElement('tr');
						h.appendChild(tr);
						if (cells) {
							var lastTD = null;
							for (var ci = 0; ci < cells.length; ci++) {
								if (cells[ci] != lastTD) {
									lastTD = cells[ci];
									var th = _editorOptions.elementEditedDoc.createElement('th');
									th.innerHTML = "Column " + (ci + 1);
									setColSpan(th, cells[ci].colSpan);
									tr.appendChild(th);
								}
							}
						}
					}
				}
				if (h) {
					selectEditElement(h);
					//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
				}
			}
		}
		function addMapArea(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var map = c.owner;
				var area = _editorOptions.elementEditedDoc.createElement('area');
				map.appendChild(area);
				selectEditElement(area);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function addLegend(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var fs = c.owner;
				var lg;
				var lgs = fs.getElementsByTagName('legend');
				if (lgs && lgs.length > 0) {
					lg = lgs[0];
					if (!lg.innerHTML || lg.innerHTML.length == 0) {
						lg.innerHTML = '<b>group</b>';
					}
				}
				else {
					lg = _editorOptions.elementEditedDoc.createElement('legend');
					lg.innerHTML = '<b>group</b>';
					fs.appendChild(lg);
				}
				selectEditElement(lg);
				//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function isSameColSpan(td1, td2) {
			if (td1.colSpan && td2.colSpan) {
				return (td1.colSpan == td2.colSpan);
			}
			if (!td1.colSpan && !td2.colSpan) {
				return true;
			}
			return false;
		}
		function isSameRowSpan(td1, td2) {
			if (td1.rowSpan && td2.rowSpan) {
				return (td1.rowSpan == td2.rowSpan);
			}
			if (!td1.rowSpan && !td2.rowSpan) {
				return true;
			}
			return false;
		}
		function showcolumnattrs(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				var tbl = td.parentNode;
				while (tbl && tbl.tagName && tbl.tagName.toLowerCase() != 'table') {
					tbl = tbl.parentNode;
				}
				if (tbl) {
					var cl;
					var cidx = td.cellIndex;
					var thd, tbd, cols, cg, tag, i;
					for (i = 0; i < tbl.children.length; i++) {
						if (tbl.children[i] && tbl.children[i].tagName) {
							tag = tbl.children[i].tagName.toLowerCase();
							if (tag == 'colgroup') {
								cg = tbl.children[i];
								var cols0 = cg.getElementsByTagName('col');
								cols = new Array();
								for (var k = 0; k < cols0.length; k++) {
									cols.push(cols0[k]);
								}
								break;
							}
							else if (tag == 'thead') {
								thd = tbl.children[i];
								break;
							}
							else if (tag == 'tbody') {
								tbd = tbl.children[i];
								break;
							}
						}
					}
					if (!cg) {
						cols = new Array();
						for (i = 0; i < tbl.children.length; i++) {
							if (tbl.children[i] && tbl.children[i].tagName && tbl.children[i].tagName.toLowerCase() == 'col') {
								cols.push(tbl.children[i]);
							}
						}
						cg = _editorOptions.elementEditedDoc.createElement("colgroup");
						if (thd) {
							tbl.insertBefore(cg, thd);
						}
						else {
							if (!tbd) {
								tbd = JsonDataBinding.getTableBody(table);
							}
							tbl.insertBefore(cg, tbd);
						}
						for (i = 0; i < cols.length; i++) {
							cg.appendChild(cols[i]);
						}
					}
					while (cidx > cols.length - 1) {
						cl = _editorOptions.elementEditedDoc.createElement("col");
						cg.appendChild(cl);
						cols.push(cl);
					}
					cl = cols[cidx];
					cl.cellIndex = cidx;
					showProperties(cl);
				}
			}
		}
		function removeTableRow(tr) {
			if (tr) {
				var p = tr.parentNode;
				if (p && p.rows) {
					var map = _getTableMap(p);
					var vp = getVirtualRowPosition(map, tr);
					var r = vp.r;
					var c;
					//cannot remove if current row starts row span
					for (c = 0; c < map[r].length; c++) {
						if (/*map[r][c].colSpan > 1 ||*/ map[r][c].rowSpan > 1) {
							alert("This row has merged cells with the next row and cannot be removed. You may use cell-split operations first and then try it again. You may also try a row-merge operation instead.");
							return;
						}
					}
					if (r > 0) {
						//adjust row span before removing the row which does not span more than one row
						var ri,i;
						var rowchecked = [];
						var tds = [];
						for (ri = 0; ri < r; ri++) {
							var checked = false;
							for (i = 0; i < rowchecked.length; i++) {
								if (rowchecked[i].row == map[ri]) {
									checked = true;
									break;
								}
							}
							if (!checked) {
								var row = { row: map[ri], cells:[] };
								rowchecked.push(row);
								for (c = 0; c < map[ri].length; c++) {
									checked = false;
									for (i = 0; i < row.cells.length; i++) {
										if (row.cells[i] == map[ri][c]) {
											checked = true;
											break;
										}
									}
									if (!checked) {
										row.cells.push(map[ri][c]);
										if (map[ri][c].rowSpan + ri > r) {
											tds.push(map[ri][c]);
										}
									}
								}
							}
						}
						for (c = 0; c < tds.length; c++) {
							setRowSpan(tds[c], tds[c].rowSpan - 1);
						}
					}
					p.removeChild(tr);
					//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
				}
			}
		}
		function rowSpanCleanup(table) {
			var map, span, spans, c, r0, r = 0;
			while (r < table.rows.length) {
				if (table.rows[r].cells.length == 0) {
					if (r > 0) {
						span = table.rows.length;//maximum span
						map = _getTableMap(table);
						r0 = r - 1;	 //try to reduce the row spans of the previous row
						for (c = 0; c < map[r0].length; c++) {
							if ((map[r0][c].rowSpan ? map[r0][c].rowSpan : 1) < span) {
								span = (map[r0][c].rowSpan ? map[r0][c].rowSpan : 1);
							}
							//if (typeof map[r0][c].rowSpan == 'undefined') {
							//	span = 1;
							//}
							//else {
							//	if (map[r0][c].rowSpan < span) {
							//		span = map[r0][c].rowSpan;
							//	}
							//}
							if (span < 2) {
								break;
							}
						}
						if (span > 1) {
							var lastTD = null;
							var lastspan = 1;
							spans = new Array(map[r0].length);
							for (c = 0; c < map[r0].length; c++) {
								if (map[r0][c] == lastTD) {
									spans[c] = lastspan;
								}
								else {
									lastTD = map[r0][c];
									lastspan = map[r0][c].rowSpan - 1;
									spans[c] = lastspan; 
								}
							}
							lastTD = null;
							for (c = 0; c < map[r0].length; c++) {
								if (map[r0][c] != lastTD) {
									lastTD = map[r0][c];
									setRowSpan(map[r0][c], spans[c]);
								}
							}
						}
					}
					table.deleteRow(r);
				}
				else {
					r++;
				}
			}
		}
		function colSpanCleanup(table) {
			var map, span, spans, c = 0, r0, r = 0;
			map = _getTableMap(table);
			if (map.length > 0) {
				var colCount = map[0].length;
				while (c < colCount) {
					span = colCount; //maximum span
					for (r = 0; r < map.length; r++) {
						if ((map[r][c].colSpan ? map[r][c].colSpan : 1) < span) {
							span = (map[r][c].colSpan ? map[r][c].colSpan : 1);
						}
						if (span <= 1) {
							break;
						}
					}
					if (span > 1) {
						var lastTD = null;
						var lastspan = 1;
						spans = new Array(map.length);
						span--;
						for (r = 0; r < map.length; r++) {
							if (map[r][c] != lastTD) {
								lastTD = map[r][c];
								lastspan = map[r][c].colSpan - span;
							}
							spans[r] = lastspan;
						}
						lastTD = null;
						for (r = 0; r < map.length; r++) {
							if (map[r][c] != lastTD) {
								lastTD = map[r][c];
								setColSpan(map[r][c], spans[r]);
							}
						}
						c += span;
					}
					c++;
				}
			}
		}
		function tableSpanCleanup(table) {
			//if (table) return;//disable it
			var i, tbl,tag;
			tbl = table;
			while (tbl && tbl.tagName.toLowerCase() != 'table') {
				tbl = tbl.parentNode;
			}
			if (tbl) {
				for (i = 0; i < tbl.children.length; i++) {
					tag = tbl.children[i].tagName.toLowerCase();
					if (tag == 'thead') {
						rowSpanCleanup(tbl.children[i]);
					}
					else if (tag == 'tfoot') {
						rowSpanCleanup(tbl.children[i]);
					}
					else if (tag == 'tbody') {
						rowSpanCleanup(tbl.children[i]);
					}
				}
			}
			if (tbl) {
				colSpanCleanup(tbl);
				for (r = 0; r < tbl.rows.length; r++) {
					for (c = 0; c < tbl.rows[r].cells.length; c++) {
						span = tbl.rows[r].cells[c].getAttribute('colspan');
						if (span == '1') {
							tbl.rows[r].cells[c].removeAttribute('colspan');
						}
						span = tbl.rows[r].cells[c].getAttribute('rowspan');
						if (span == '1') {
							tbl.rows[r].cells[c].removeAttribute('rowspan');
						}
					}
				}
			}
		}
		function setRowSpan(td, rs) {
			if (rs <= 1)
				td.removeAttribute('rowspan');
			else
				td.rowSpan = rs;
		}
		function setColSpan(td, cs) {
			if (cs <= 1)
				td.removeAttribute('colspan');
			else
				td.colSpan = cs;
		}
		function mergeRows(e, above) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var tr = c.owner;
				if (tr) {
					var p = tr.parentNode;
					while (p) {
						if (p.rows) {
							break;
						}
						p = p.parentNode;
					}
					if (p) {
						var map = _getTableMap(p);
						var vp = getVirtualRowPosition(map, tr);
						var rowIndex = vp.r + getMapRowSpan(map, vp.r)-1;
						if ((above && rowIndex > 0) || (!above && rowIndex < p.rows.length - 1)) {
							var i, csp0, csp1, rsp0, rsp1;
							var canMerge = true;
							var r0;
							if (above) {
								r0 = rowIndex - 1;
							}
							else {
								r0 = rowIndex;
								rowIndex = r0 + 1;
							}
							for (i = 0; i < map[r0].length; i++) {
								csp0 = 1;
								csp1 = 1;
								if (typeof map[r0][i].colSpan != 'undefined') csp0 = map[r0][i].colSpan;
								if (typeof map[rowIndex][i].colSpan != 'undefined') csp1 = map[rowIndex][i].colSpan;
								if (csp0 != csp1) {
									canMerge = false;
									break;
								}
							}
							if (canMerge) {
								var data = new Array();
								for (i = 0; i < map[r0].length; i++) {
									var td0 = map[r0][i];
									var td1 = map[rowIndex][i];
									var tr1 = map[rowIndex][i].parentNode;
									var cvi = getVirtualColumnIndex(td1, map[rowIndex]);
									rsp0 = 1;
									rsp1 = 1;
									if (typeof map[r0][i].rowSpan != 'undefined') rsp0 = map[r0][i].rowSpan;
									if (typeof map[rowIndex][i].rowSpan != 'undefined') rsp1 = map[rowIndex][i].rowSpan;
									rsp0 += rsp1;
									data.push({
										td0: td0, td1: td1, tr1: tr1, cvi: cvi, rsp0: rsp0, rsp1: rsp1, html: td0.innerHTML + td1.innerHTML
									});
								}
								for (i = 0; i < data.length; i++) {
									setRowSpan(data[i].td0, data[i].rsp0);
									data[i].td0.innerHTML = data[i].html;
									data[i].td1.parentNode.deleteCell(data[i].td1.cellIndex);
								}
								tableSpanCleanup(p);
							}
							else {
								alert('The two rows have different column spans and cannot be merged.');
							}
						}
					}
				}
			}
		}
		function mergeRowAbove(e) {
			mergeRows(e, true);
		}
		function mergeRowBelow(e) {
			mergeRows(e, false);
		}
		function addRowAbove(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				var row;
				if (td.cells) {
					row = td;
				}
				else {
					row = td.parentNode;
				}
				if (row && row.parentNode) {
					var rowParent = row.parentNode;
					var map = _getTableMap(rowParent);
					var vp = getVirtualRowPosition(map, row);
					var rIndex = vp.r;
					var nr = rowParent.insertRow(rIndex);
					var lastTD = null;
					for (var i = 0; i < map[rIndex].length; i++) {
						if (map[rIndex][i] != lastTD) {
							lastTD = map[rIndex][i];
							var isCross = false;
							if (rIndex > 0 && rIndex < map.length) {
								if (map[rIndex][i] == map[rIndex - 1][i]) {
									isCross = true;
								}
							}
							if (isCross) {
								setRowSpan(map[rIndex][i], (map[rIndex][i].rowSpan?map[rIndex][i].rowSpan:1) + 1);
							}
							else {
								var tdn = _editorOptions.elementEditedDoc.createElement("td");
								tdn.innerHTML = "cell";
								setColSpan(tdn, map[rIndex][i].colSpan);
								//tdn.bgColor = map[rIndex][i].bgColor;
								nr.appendChild(tdn);
							}
						}
					}
					showSelectionMark(_editorOptions.selectedObject);
				}
			}
		}
		function addRowBelow(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				var row;
				var dr,i;
				if (td.cells) {
					row = td;
					dr = 1;
					for (i = 0; i < row.cells.length; i++) {
						if ((row.cells[i].rowSpan ? row.cells[i].rowSpan : 1) > dr) {
							dr = row.cells[i].rowSpan ? row.cells[i].rowSpan : 1;
						}
					}
				}
				else {
					dr = (td.rowSpan ? td.rowSpan : 1);
					row = td.parentNode;
				}
				if (row && row.parentNode) {
					var rowParent = row.parentNode;
					var map = _getTableMap(rowParent);
					var vp = getVirtualRowPosition(map, row);
					var rIndex = vp.r + dr - 1;
					var nr = rowParent.insertRow(rIndex+1);
					var lastTD = null;
					for (i = 0; i < map[rIndex].length; i++) {
						if (map[rIndex][i] != lastTD) {
							lastTD = map[rIndex][i];
							var isCross = false;
							if (rIndex < map.length - 1) {
								if (map[rIndex][i] == map[rIndex + 1][i]) {
									isCross = true;
								}
							}
							if (isCross) {
								setRowSpan(map[rIndex][i], (map[rIndex][i].rowSpan?map[rIndex][i].rowSpan:1) + 1);
							}
							else {
								var tdn = _editorOptions.elementEditedDoc.createElement("td");
								tdn.innerHTML = "cell";
								setColSpan(tdn, map[rIndex][i].colSpan);
								nr.appendChild(tdn);
							}
						}
					}
					showSelectionMark(_editorOptions.selectedObject);
				}
			}
		}
		function addColumnToLeft(holder, map, colIdx) {
			var lastTD = null;
			//add new columns to each row
			for (var r = 0; r < map.length; r++) {
				//only process spanned rows once
				if (map[r][colIdx] != lastTD) {
					lastTD = map[r][colIdx];
					var isCross = false;
					if (colIdx > 0 && colIdx < map[r].length - 1) { //current cell is not first and last
						if (map[r][colIdx] == map[r][colIdx - 1]) { //at this row the cell spans to previous column
							if (map[r][colIdx] == map[r][colIdx + 1]) { //at this row the cell spans to next column
								isCross = true;
							}
						}
					}
					if (isCross) {
						//increase colSpan
						setColSpan(map[r][colIdx], (map[r][colIdx].colSpan ? map[r][colIdx].colSpan : 0) + 1);
					}
					else {
						//add a new cell
						var cidx = mapGridColumnIndexToTableCellIndex(colIdx, holder.rows[r]);
						var tdn = holder.rows[r].insertCell(cidx);
						tdn.innerHTML = "cell";
						setRowSpan(tdn, map[r][colIdx].rowSpan);
					}
				}
			}
		}
		function addColumnLeft(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					var rowParent = row.parentNode;
					var map = _getTableMap(rowParent);
					//current row index
					var vp = getVirtualCellPosition(map, td);
					var ri = vp.r;
					//current column index, new column is added to its left
					var colIdx = vp.c;//getVirtualColumnIndex(td, map[ri]); //grid column index
					addColumnToLeft(rowParent, map, colIdx);
					var tbl = rowParent;
					while (tbl.tagName) {
						if (tbl.tagName.toLowerCase() == 'table') {
							break;
						}
						tbl = tbl.parentNode;
					}
					var i;
					var holders = tbl.getElementsByTagName('tbody');
					if (holders && holders.length > 0) {
						for (i = 0; i < holders.length; i++) {
							if (holders[i] != rowParent) {
								map = _getTableMap(holders[i]);
								addColumnToLeft(holders[i], map, colIdx);
							}
						}
					}
					holders = tbl.getElementsByTagName('thead');
					if (holders && holders.length > 0) {
						for (i = 0; i < holders.length; i++) {
							if (holders[i] != rowParent) {
								map = _getTableMap(holders[i]);
								addColumnToLeft(holders[i], map, colIdx);
							}
						}
					}
					holders = tbl.getElementsByTagName('tfoot');
					if (holders && holders.length > 0) {
						for (i = 0; i < holders.length; i++) {
							if (holders[i] != rowParent) {
								map = _getTableMap(holders[i]);
								addColumnToLeft(holders[i], map, colIdx);
							}
						}
					}
					showSelectionMark(_editorOptions.selectedObject);
				}
			}
		}
		function addColumnToRight(holder, map, colIdx) {
			var lastTD = null;
			for (var r = 0; r < map.length; r++) {
				if (map[r][colIdx] != lastTD) {
					lastTD = map[r][colIdx];
					var isCross = false;
					if (colIdx > 0 && colIdx < map[r].length - 1) {
						if (map[r][colIdx] == map[r][colIdx - 1]) {
							if (map[r][colIdx] == map[r][colIdx + 1]) {
								isCross = true;
							}
						}
					}
					if (isCross) {
						setColSpan(map[r][colIdx], map[r][colIdx].colSpan + 1);
					}
					else {
						var tn = map[r][colIdx];
						var tnci = tn.cellIndex ? tn.cellIndex : 0;
						var tdn = tn.parentNode.insertCell(tnci + 1);
						tdn.innerHTML = "cell";
						setRowSpan(tdn, tn.rowSpan);
					}
				}
			}
		}
		function addColumnRight(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					var rowParent = row.parentNode;
					var map = _getTableMap(rowParent);
					var vp = getVirtualCellPosition(map, td);
					var ri = vp.r;
					var colIdx = vp.c;
					addColumnToRight(rowParent, map, colIdx);
					var tbl = rowParent;
					while (tbl.tagName) {
						if (tbl.tagName.toLowerCase() == 'table') {
							break;
						}
						tbl = tbl.parentNode;
					}
					var i;
					var holders = tbl.getElementsByTagName('tbody');
					if (holders && holders.length > 0) {
						for (i = 0; i < holders.length; i++) {
							if (holders[i] != rowParent) {
								map = _getTableMap(holders[i]);
								addColumnToRight(holders[i], map, colIdx);
							}
						}
					}
					holders = tbl.getElementsByTagName('thead');
					if (holders && holders.length > 0) {
						for (i = 0; i < holders.length; i++) {
							if (holders[i] != rowParent) {
								map = _getTableMap(holders[i]);
								addColumnToRight(holders[i], map, colIdx);
							}
						}
					}
					holders = tbl.getElementsByTagName('tfoot');
					if (holders && holders.length > 0) {
						for (i = 0; i < holders.length; i++) {
							if (holders[i] != rowParent) {
								map = _getTableMap(holders[i]);
								addColumnToRight(holders[i], map, colIdx);
							}
						}
					}
					showSelectionMark(_editorOptions.selectedObject);
				}
			}
		}
		function mergeColumnLeft(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					//
					var colIdx = td.cellIndex ? td.cellIndex : 0;
					if (colIdx > 0) {
						var canMerge = true;
						var rowParent = row.parentNode;
						var map = _getTableMap(rowParent);
						var vp = getVirtualCellPosition(map, td);
						var rIndex = vp.r;
						var td0 = row.cells[colIdx - 1];						
						var vci = vp.c;
						if (rIndex > 0 && map[rIndex][vci - 1] == map[rIndex - 1][vci - 1]) {
							canMerge = false;
						}
						else if (map[rIndex][vci - 1].rowSpan != map[rIndex][vci].rowSpan) {
							canMerge = false;
						}
						if (canMerge) {
							var cp = td.colSpan ? td.colSpan : 1;
							var hl = td.innerHTML;
							row.deleteCell(colIdx);
							setColSpan(td0, cp + (td0.colSpan ? td0.colSpan : 1));
							td0.innerHTML = td0.innerHTML + hl;
							tableSpanCleanup(rowParent);
							docClick({ target: td0 });
							showSelectionMark(td0);
						}
						else {
							alert('The two columns have different row spans and cannot be merged.');
						}
					}
				}
			}
		}
		function mergeColumnRight(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					var colIdx = td.cellIndex ? td.cellIndex : 0;
					if (colIdx >= 0 && colIdx < row.cells.length - 1) {
						var canMerge = true;
						var rowParent = row.parentNode;
						var map = _getTableMap(rowParent);
						var vp = getVirtualCellPosition(map, td);
						var rIndex = vp.r;
						var cidx = vp.c;//getVirtualColumnIndex(td, map[rIndex]);
						var nextcidx = getNextVirtualColumnIndex(td, map[rIndex]);
						if (nextcidx < 0) {
							canMerge = false;
						}
						else if (rIndex > 0 && map[rIndex][nextcidx] == map[rIndex - 1][nextcidx]) {
							canMerge = false;
						}
						else {
							if (!isSameRowSpan(map[rIndex][nextcidx], map[rIndex][cidx])) {
								canMerge = false;
							}
						}
						if (canMerge) {
							var td1 = row.cells[colIdx + 1];
							if (td.colSpan) {
								setColSpan(td, td1.colSpan + td.colSpan);
							}
							else {
								setColSpan(td, td1.colSpan + 1);
							}
							td.innerHTML = td.innerHTML + td1.innerHTML;
							row.deleteCell(colIdx + 1);
							tableSpanCleanup(rowParent);
							docClick({ target: td });
						}
						else {
							alert('The two columns have different row spans and cannot be merged.');
						}
					}
				}
			}
		}
		function mergeColumnAbove(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					var rowParent = row.parentNode;
					var map = _getTableMap(rowParent);
					var vp = getVirtualCellPosition(map, td);
					var colIdx = td.cellIndex ? td.cellIndex : 0;
					var rowIndex = vp.r;
					if (colIdx >= 0 && colIdx < row.cells.length && rowIndex > 0 && rowIndex < rowParent.rows.length) {
						var canMerge = true;
						var cvi = vp.c;
						if (cvi >= 0) {
							if (cvi > 0 && map[rowIndex - 1][cvi - 1] == map[rowIndex - 1][cvi]) {
								canMerge = false;
							}
							else if (!isSameColSpan(map[rowIndex - 1][cvi], map[rowIndex][cvi])) {
								canMerge = false;
							}
							if (canMerge) {
								var td0 = map[rowIndex - 1][cvi];
								if (td0.colSpan == td.colSpan) {
									setRowSpan(td0, td0.rowSpan + td.rowSpan);
									td0.innerHTML = td0.innerHTML + td.innerHTML;
									row.deleteCell(colIdx);
									tableSpanCleanup(rowParent);
									docClick({ target: td0 });
								}
							}
						}
					}
				}
			}
		}
		function mergeColumnBelow(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					var rowParent = row.parentNode;
					var map = _getTableMap(rowParent);
					var vp = getVirtualCellPosition(map, td);
					if (!vp) {
						alert('Error locating virtual cell');
						return;
					}
					var colIdx = td.cellIndex ? td.cellIndex : 0;
					var rowIndex = vp.r;
					if (colIdx >= 0 && colIdx < row.cells.length && rowIndex >= 0 && rowIndex < rowParent.rows.length - 1) {
						var cvi = vp.c;
						rowIndex = vp.r + (td.rowSpan ? td.rowSpan : 1) - 1;
						if (rowIndex < map.length - 1) {
							var n = 1;
							var td1 = map[rowIndex + 1][cvi]; //cell to be merged
							var vp1 = getVirtualCellPosition(map, td1);
							if (!vp1) {
								alert('Error locating the second virtual cell');
								return;
							}
							if (vp1.c != vp.c) {
								alert('Cannot merge the cells because their vertical positions are not the same.');
								return;
							}
							if (!isSameColSpan(td1, td)) {
								alert('Cannot merge the cells because they span differently.');
								return;
							}
							var sp = td1.rowSpan ? td1.rowSpan : 1;
							var hl = td1.innerHTML;
							var c1 = td1.cellIndex ? td1.cellIndex : 0;
							td1.parentNode.deleteCell(c1);
							setRowSpan(td, (td.rowSpan ? td.rowSpan : 1) + sp);
							td.innerHTML = td.innerHTML + hl;
							tableSpanCleanup(rowParent);
							docClick({ target: td });
						}
					}
				}
			}
		}
		function splitColumnH(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var tdn;
					var row = td.parentNode;
					var colIdx = td.cellIndex ? td.cellIndex : 0;
					if (colIdx >= 0 && colIdx < row.cells.length) {
						if (td.colSpan > 1) {
							tdn = row.insertCell(colIdx + 1);
							tdn.innerHTML = "cell";
							setColSpan(td, td.colSpan - 1);
							setRowSpan(tdn, td.rowSpan);
						}
						else {
							var r;
							var rowParent = row.parentNode;
							var tbl = rowParent;
							while (tbl && tbl.tagName && tbl.tagName.toLowerCase() != 'table') {
								tbl = tbl.parentNode;
							}
							if (tbl) {
								var rowIndex = row.rowIndex;
								var map = _getTableMap(tbl);
								var vci = getVirtualColumnIndex(td, map[rowIndex]);
								var lastTD = null;
								for (r = 0; r < map.length; r++) {
									if (map[r][vci] != lastTD) {
										lastTD = map[r][vci];
										if (lastTD == td) {
											tdn = row.insertCell(colIdx + 1);
											tdn.innerHTML = "cell";
											setRowSpan(tdn, td.rowSpan);
										}
										else {
											setColSpan(lastTD, (lastTD.colSpan ? lastTD.colSpan : 1) + 1);
										}
									}
								}
							}
						}
						showSelectionMark(_editorOptions.selectedObject);
					}
				}
			}
		}
		//add a cell below the current cell
		function splitColumnV(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.owner) {
				var td = c.owner;
				if (td.parentNode && td.parentNode.parentNode) {
					var row = td.parentNode;
					var colIdx = td.cellIndex ? td.cellIndex : 0;
					if (colIdx >= 0 && colIdx < row.cells.length) {
						var tdn;
						var rowParent = row.parentNode;
						var map = _getTableMap(rowParent);
						var vp = getVirtualCellPosition(map, td);
						var rowIndex = vp.r;
						var cIdx = vp.c;
						if (td.rowSpan > 1) {
							//add a new cell using the same col span, reduce row span of the current cell
							var rn = rowIndex + td.rowSpan - 1;
							var cni = mapGridColumnIndexToTableCellIndex(cIdx, rowParent.rows[rn]);
							tdn = rowParent.rows[rn].insertCell(cni);
							tdn.innerHTML = "cell";
							setColSpan(tdn, td.colSpan);
							setRowSpan(td, td.rowSpan - 1);
						}
						else {
							//add a new row, increase row span of all cells of the current row except current cell
							//insert a new cell to the new row using the same colspan of the current cell
							var nr = rowParent.insertRow(rowIndex + 1);
							var lastTD = null;
							for (var ci = 0; ci < map[rowIndex].length; ci++) {
								if (map[rowIndex][ci] != lastTD) {
									lastTD = map[rowIndex][ci];
									if (lastTD == td) {
										tdn = nr.insertCell(0);
										tdn.innerHTML = "cell";
										setColSpan(tdn, td.colSpan);
									}
									else {
										setRowSpan(lastTD, (lastTD.rowSpan ? lastTD.rowSpan : 1) + 1);
									}
								}
							}
						}
						showSelectionMark(_editorOptions.selectedObject);
					}
				}
			}
		}
		function getIframFullUrl(o) {
			if (typeof o.name != 'undefined' && o.name.length > 0) {
				var nm = o.name.trim();
				if (nm.length > 0) {
					var s;
					if (_editorOptions.forIDE)
						s = o.getAttribute('srcDesign');
					else
						s = o.src;
					if (typeof s != 'undefined' && s != null && s.length > 0) {
						var u = getWebAddress();
						return u + '?iframe_' + nm + '=' + s;
					}
				}
			}
		}
		function removeFromCSStext(css, name) {
			if (!css) return '';
			if (css.length == 0) return '';
			if (!name) return css;
			if (name.length == 0) return css;
			var cssi = css.toLowerCase();
			var namei = name.toLowerCase();
			var pos = cssi.indexOf(namei);
			while (pos >= 0) {
				var i = pos + namei.length;
				if (i == cssi.length) return css;
				if (pos > 0) {
					if (cssi.charAt(pos - 1) != ';' && cssi.charAt(pos - 1) != ' ' && cssi.charAt(pos - 1) != '{') {
						pos = cssi.indexOf(namei, i);
						continue;
					}
				}
				while (i < cssi.length) {
					if (cssi.charAt(i) == ':') {
						var j = i + 1;
						while (j < cssi.length) {
							if (cssi.charAt(j) == ';' || cssi.charAt(j) == '}') {
								break;
							}
							j++;
						}
						var cssNew;
						if (pos == 0) {
							if (j >= cssi.length - 1)
								cssNew = '';
							else
								cssNew = css.substr(j + 1);
						}
						else {
							if (j >= cssi.length - 1)
								cssNew = css.substr(0, pos);
							else
								cssNew = css.substr(0, pos) + css.substr(j + 1);
						}
						return cssNew;
					}
					else if (cssi.charAt(i) != ' ') {
						break;
					}
					i++;
				}
				pos = cssi.indexOf(namei, pos + namei.length);
			}
			return css;
		}
		function setCSStext(obj, name, val) {
			if (!name) return;
			var css = obj.style.cssText;
			if (!css || css.length == 0) {
				if (val && val.length > 0) {
					obj.style.cssText = name + ':' + val + ';';
				}
			}
			else {
				css = removeFromCSStext(css, name);
				if (val && val.length > 0) {
					obj.style.cssText = name + ':' + val + ';' + css;
				}
				else {
					obj.style.cssText = css;
				}
			}
		}
		function setElementInnerHTML(e) {
			if (!_editorOptions) return;
			if (e) {
				var sender = JsonDataBinding.getSender(e);
				if (sender) {
					c = sender.owner;
					if (c) {
						var txt;
						if (!_textInput) {
							_textInput = document.createElement('div');
							_textInput.style.backgroundColor = '#ADD8E6';
							_textInput.style.position = 'absolute';
							_textInput.style.border = "3px double black";
							_textInput.style.color = 'black';
							_textInput.style.width = '600px';
							_textInput.style.height = '400px';
							appendToBody(_textInput);
							//
							var imgOK = document.createElement('img');
							imgOK.src = '/libjs/ok.png';
							imgOK.style.cursor = 'pointer';
							_textInput.appendChild(imgOK);
							
							var imgCancel = document.createElement('img');
							imgCancel.src = '/libjs/cancel.png';
							imgCancel.style.cursor = 'pointer';
							_textInput.appendChild(imgCancel);

							var imgDel = document.createElement('img');
							imgDel.src = '/libjs/del.png';
							imgDel.style.cursor = 'pointer';
							_textInput.appendChild(imgDel);

							var imgPaste = document.createElement('img');
							imgPaste.src = '/libjs/paste.png';
							imgPaste.style.cursor = 'pointer';
							_textInput.appendChild(imgPaste);
							
							_textInput.appendChild(document.createElement('br'));
							txt = document.createElement('textarea');
							txt.style.width = '600px';
							txt.style.height = '380px';
							_textInput.appendChild(txt);
							JsonDataBinding.AttachEvent(imgCancel, 'onclick', function () { _textInput.style.display = 'none'; _textInput.valueTarget = null; });
							JsonDataBinding.AttachEvent(imgOK, 'onclick', function () {
								if (_textInput.valueTarget) {
									if (_editorOptions) {
										_editorOptions.deleted = { parent: _textInput.valueTarget, originalHTML: _textInput.valueTarget.innerHTML };
										_imgUndel.src = '/libjs/undel.png';
									}
									_textInput.valueTarget.innerHTML = txt.value;
								}
								_textInput.style.display = 'none';
								_textInput.valueTarget = null;
							});
							JsonDataBinding.AttachEvent(imgDel, 'onclick', function () {
								var p = txt.selectionStart;
								if (typeof (p) != 'undefined' && p >= 0) {
									var end = txt.selectionEnd;
									if (end > p) {
										txt.value = txt.value.substr(0, p) + txt.value.substr(end);
										txt.selectionStart = p;
										txt.selectionEnd = p;
									}
								}
							});
							hookTooltips(imgDel, 'Delete selected text');
							JsonDataBinding.AttachEvent(imgPaste, 'onclick', function () {
								var data = window.clipboardData.getData('text');
								if (typeof data != 'undefined' && data != null && data.length > 0) {
									var p = txt.selectionStart;
									var end;
									if (typeof (p) == 'undefined' || p < 0) {
										p = txt.value.length - 1;
										end = p + 1;
									}
									else {
										end = txt.selectionEnd;
										if (typeof (end) == 'undefined' || end < 0) {
											end = p + 1;
										}
										else if (end < p) {
											end = p;
										}
									}
									if (end >= p) {
										txt.value = txt.value.substr(0, p) + data + txt.value.substr(end);
										txt.selectionStart = p;
										txt.selectionEnd = p + data.length;
									}
								}
							});
							hookTooltips(imgPaste, 'paste text from clipboard to replace selected text');
						}
						txt = _textInput.getElementsByTagName('textarea')[0];
						_textInput.valueTarget = c;
						txt.value = c.innerHTML;
						JsonDataBinding.windowTools.updateDimensions();
						var ap = JsonDataBinding.ElementPosition.getElementPosition(sender);
						_textInput.style.left = (ap.x + 3) + 'px';
						_textInput.style.top = (ap.y + 3) + 'px';
						var zi = JsonDataBinding.getPageZIndex(_textInput) + 1;
						_textInput.style.zIndex = zi;
						_textInput.style.display = 'block';
						if (e.stopPropagation) e.stopPropagation();
						if (e.cancelBubble != null) e.cancelBubble = true;
						return true;
					}
				}
			}
		}
		function addAttr(e) {
			if (e) {
				var c = JsonDataBinding.getSender(e);
				if (c) {
					c = c.owner;
					if (c) {
						var attrName = prompt('Enter new attribute name', '');
						if (attrName) {
							attrName = attrName.toLowerCase().trim();
							if (attrName.length > 0) {
								if (attrName != 'id' && attrName != 'class' && attrName != 'style') {
									if (attrName.indexOf(' ') >= 0) {
										alert('An attribute name cannot include spaces');
									}
									else {
										c.setAttribute(attrName, 'new attribute');
										_redisplayProperties();
									}
								}
								else {
									alert('Cannot use the new attribute name. It is a reserved word.');
								}
							}
						}
					}
				}
			}
		}
		function _getElementProperties(tagname, c, attrs, typename) {
			if (_elementEditorList) {
				var props1;
				var internalAttributes;
				var i;
				var nounformat = false;
				var nodelete;
				var noMoveOut = false;
				var noCommonProps = false;
				var noCust = false;
				var isHidden = false;
				var noSetInnerHtml = false;
				var objSetter;
				var isSvgShape;
				var notclose;
				var issvgtext;
				var onpropscreated;
				var isCss = isCssNode(c);
				for (i = 0; i < _elementEditorList.length; i++) {
					if (_elementEditorList[i].tagname == typename) {
						isSvgShape = _elementEditorList[i].isSvgShape;
						notclose = _elementEditorList[i].notclose;
						issvgtext = _elementEditorList[i].issvgtext;
						onpropscreated = _elementEditorList[i].onpropscreated;
						//if (isSvgShape) {
						//	var a = c.parentNode;
						//	if (a) {
						//		a.hideParent = true;
						//	}
						//}
						objSetter = _elementEditorList[i].objSetter;
						if (_elementEditorList[i].type) {
							if (c.type) {
								if (_elementEditorList[i].type == c.type) {
									isHidden = (c.type == 'hidden' && typename == 'input');
									props1 = _elementEditorList[i].properties;
									if (_elementEditorList[i].nounformat) {
										nounformat = _elementEditorList[i].nounformat;
									}
									if (typeof _elementEditorList[i].nodelete != 'undefined') {
										nodelete = _elementEditorList[i].nodelete;
									}
									if (_elementEditorList[i].nomoveout) {
										noMoveOut = _elementEditorList[i].nomoveout;
									}
									if (_elementEditorList[i].nocommonprops) {
										noCommonProps = _elementEditorList[i].nocommonprops;
									}
									if (_elementEditorList[i].noCust) {
										noCust = _elementEditorList[i].noCust;
									}
									if (_elementEditorList[i].desc) {
										attrs.desc = _elementEditorList[i].desc;
									}
									else {
										attrs.desc = '';
									}
									if(_elementEditorList[i].noSetInnerHtml){
										noSetInnerHtml = _elementEditorList[i].noSetInnerHtml;
									}
									break;
								}
							}
						}
						else {
							props1 = _elementEditorList[i].properties;
							internalAttributes = _elementEditorList[i].internalAttributes;
							if (isCss) {
								var props = new Array();
								//get href property
								for (var k = 0; k < props1.length; k++) {
									if (props1[k].name == 'href') {
										props1[k].desc = 'the css file to be used.';
										props1[k].title = 'Select CSS File';
										props1[k].filetypes = 'css';
										props.push(props1[k]);
										break;
									}
								}
								props.push({ name: 'delete', desc: 'delete the stylesheet file', editor: EDIT_DEL })
								props.push({ name: 'title', desc: 'title of the stylesheet', editor: EDIT_TEXT })
								//list rules to provide deletion command
								props.push({
									name: 'rules', editor: EDIT_PROPS, desc:'CSS Rules',getter: function(o) {
										var fpath = o ? o.href : '';
										if (!fpath || fpath.length == 0) return;
										fpath = JsonDataBinding.urlToFilename(fpath.toLowerCase());
										function ruleProp(ss, index, ctext, sel, filepath) {
											var _filepath = filepath;
											var _styleSheet = ss;
											var _idx = index;
											var _text = ctext;
											var _name = sel;
											return {
												name: _name,
												desc: 'delete the styles',
												editor: EDIT_NONE,
												getter: function() { return _text; },
												IMG: '/libjs/cancel.png',
												action: function(e) {
													if (confirm('Do you want to remove styles ' + _name + '?')) {
														var c = JsonDataBinding.getSender(e);
														if (c) {
															c.style.display = 'none';
														}
														if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
														JsonDataBinding.accessServer('WebPageSaveStyle.php', 'removeStyleRule', _filepath, { selector: _name }, function() {
															var rdel = false;
															if (_styleSheet.removeRule) {
																_styleSheet.removeRule(_idx);
																rdel = true;
															}
															else if (_styleSheet.deleteRule) {
																_styleSheet.deleteRule(_idx);
																rdel = true;
															}
															else alert('Your browser does not support this feature.');
															if (rdel) {
																_redisplayProperties();
															}
														});
													}
												},
												notModify:true
											}
										}
										function getCssRuleText(st, rule) {
											var ctxt;
											if (JsonDataBinding.IsIE()) {
												if (rule.cssText) {
													ctxt = rule.cssText;
												}
												else if (st.cssText) {
													var pos = st.cssText.indexOf(rule.selectorText);
													if (pos >= 0) {
														ctxt = st.cssText.substr(pos);
														pos = ctxt.indexOf('}');
														if (pos > 0) {
															ctxt = ctxt.substr(0, pos + 1);
														}
													}
													else {
														ctxt = st.cssText;
													}
												}
											}
											else {
												ctxt = rule.cssText;
											}
											return ctxt;
										}
										var styleTitle = 'dyStyle8831932';
										var props = new Array();
										var st;
										var stDy;
										var stls = _editorOptions.elementEditedDoc.styleSheets;
										if (stls) {
											for (i = 0; i < stls.length; i++) {
												if (JsonDataBinding.urlToFilename(stls[i].href ? stls[i].href.toLowerCase() : '') == fpath) {
													st = stls[i];
													if (stDy) {
														break;
													}
												}
												else if (stls[i].title == styleTitle) {
													stDy = stls[i];
													if (st) {
														break;
													}
												}
											}
										}
										if (st) {
											var rs;
											if (st.cssRules) {
												rs = st.cssRules;
											}
											else if (st.rules) {
												rs = st.rules;
											}
											if (rs) {
												for (var r = 0; r < rs.length; r++) {
													var ctxt = getCssRuleText(st, rs[r]);
													props.push(ruleProp(st, r, ctxt, rs[r].selectorText, fpath));
												}
											}
										}
										if (stDy) {
											var rs;
											if (stDy.cssRules) {
												rs = stDy.cssRules;
											}
											else if (stDy.rules) {
												rs = stDy.rules;
											}
											if (rs) {
												for (var r = 0; r < rs.length; r++) {
													var isNew = true;
													for (var k = 0; k < props.length; k++) {
														if (props[k].name == rs[r].selectorText) {
															isNew = false;
															break;
														}
													}
													if (isNew) {
														var ctxt = getCssRuleText(stDy, rs[r]);
														props.push(ruleProp(stDy, r, ctxt, rs[r].selectorText, fpath));
													}
												}
											}
										}
										return { props: props, objSetter: objSetter };
									}
								})
								return { props: props, objSetter: objSetter };
							}
							else {
								if (_elementEditorList[i].nounformat) {
									nounformat = _elementEditorList[i].nounformat;
								}
								if (typeof _elementEditorList[i].nodelete != 'undefined') {
									nodelete = _elementEditorList[i].nodelete;
								}
								if (_elementEditorList[i].nomoveout) {
									noMoveOut = _elementEditorList[i].nomoveout;
								}
								if (_elementEditorList[i].nocommonprops) {
									noCommonProps = _elementEditorList[i].nocommonprops;
								}
								if (_elementEditorList[i].noCust) {
									noCust = _elementEditorList[i].noCust;
								}
								if (_elementEditorList[i].desc) {
									attrs.desc = _elementEditorList[i].desc;
								}
								else {
									attrs.desc = '';
								}
								if(_elementEditorList[i].noSetInnerHtml){
									noSetInnerHtml = _elementEditorList[i].noSetInnerHtml;
								}
							}
							break;
						}
					}
				}
				if (props1) {
					if (isHidden || tagname == 'body' || tagname == 'head' || tagname == 'meta' || tagname == 'script' || tagname == 'link') {
						return { props: props1, objSetter: objSetter }; // props1;
					}
					var props;
					if (tagname == 'html') {
						props = new Array();
						for (i = 0; i < props1.length; i++) {
							if (_editorOptions.forIDE) {
								if (props1[i].name == 'title' || props1[i].name == 'description' || props1[i].name == 'keywords') {
									continue;
								}
							}
							props.push(props1[i]);
						}
						if (_editorOptions.htmlFileOption != 0) {
							for (i = 0; i < htmlsaveToProps.length; i++) {
								props.push(htmlsaveToProps[i]);
							}
						}
						if (_editorOptions.isEditingBody && !_editorOptions.forIDE) {
							props.push({ name: 'PageComment', editor: EDIT_ENUM, values: ['', 'disabled', 'readOnly'], desc: 'disabled:page comments are not allowed; readOnly:existing page comments are visible, cannot add new comments', getter: function(o) { return _editorOptions.elementEditedDoc.body.getAttribute('commentOption'); }, setter: function(o, v) { _editorOptions.elementEditedDoc.body.setAttribute('commentOption', v); } });
						}
						return { props: props, objSetter: objSetter };
					}
					else {
						props = new Array();
						if (nounformat) {
							if (typeof nodelete != 'undefined' && !nodelete) {
								props.push({ name: 'delete', desc: 'delete the element', editor: EDIT_DEL });
							}
						}
						else {
							props.push({ name: 'delete', desc: 'delete the element', editor: EDIT_DEL });
							props.push({ name: 'commands', editor: EDIT_CMD2, toolTips: 'remove formatting by removing current tag and keeping the inner contents' });
						}
						if (!noMoveOut) {
							props.push({ name: 'commands', editor: EDIT_CMD3, toolTips: 'move the element out of its current parent' });
						}
						if(!noSetInnerHtml)
						props.push(innerHtmlProp);
						if (!noCust) {
							if (c.subEditor) {
								noCust = true;
							}
						}
						if (!noCust) {
							props.push({ name: 'newAttr', editor: EDIT_CMD, IMG: '/libjs/newAttr.png', toolTips: 'add a new attribute', action: addAttr });
							var attrs = c.attributes;
							if (attrs) {
								var k, isCust, attName, m;
								for (var j = 0; j < attrs.length; j++) {
									isCust = false;
									if (attrs[j].nodeName) {
										attName = attrs[j].nodeName.toLowerCase();
										if (attName != 'id' && attName != 'class' && attName != 'style' && attName != 'typename' && attName != 'scriptData' && attName != 'hidechild' && attName != 'youtube' && attName != 'youtubeID' && attName != 'onclick' && attName != 'src' && attName != 'limnorid' && attName != 'srcdesign') {
											isCust = true;
											for (k = 0; k < props1.length; k++) {
												if (props1[k].editorList && typeof (props1[k].editorList) != 'function') {
													var ps = props1[k].editorList;
													if (ps) {
														for (m = 0; m < ps.length; m++) {
															if (attName == ps[m].name.toLowerCase()) {
																isCust = false;
																break;
															}
														}
													}
													if (!isCust)
														break;
												}
												else if (attName == props1[k].name.toLowerCase()) {
													isCust = false;
													break;
												}
											}
											if (isCust) {
												for (k = 0; k < commonProperties.length; k++) {
													if (attName == commonProperties[k].name.toLowerCase()) {
														isCust = false;
														break;
													}
												}
											}
											if (isCust) {
												if (attrs[j].nodeValue && attrs[j].nodeValue.length > 0) {
												}
												else {
													isCust = false;
													c.removeAttribute(attName);
												}
											}
										} 
									}
									if (isCust) {
										if (internalAttributes) {
											for (k = 0; k < internalAttributes.length; k++) {
												if (internalAttributes[k] == attName) {
													isCust = false;
													break;
												}
											}
										}
									}
									if (isCust) {
										if (typeof (c.getAttribute(attName)) == 'function')
											continue;
										props1.splice(0, 0, {
											name: attName, editor: EDIT_CUST, desc: 'attribute ' + attName,
											getter: function(an) {
												return function(xx) {
													try {
														return xx.getAttribute(an);
													}
													catch (exp) {
														if (exp) {
															if (exp.message) {
																return exp.message;
															}
															else
																return 'Exception occurred';
														}
														else
															return 'Exception occurred';
													}
												};
											} (attName),
											setter: function(an) {
												return function(xx, val) {
													try {
														xx.setAttribute(an, val);
													}
													catch (exp) {
														if (exp) {
															if (exp.message) {
																alert(exp.message);
															}
															else
																alert('Exception occurred');
														}
														else
															alert('Exception occurred');
													}
												};
											} (attName)
										});
									}
								}
							}
						}
						var generalAdded = false;
						for (i = 0; i < props1.length; i++) {
							if (!noCommonProps && !generalAdded && props1[i].cat) {
								props.push({ name: 'general', editor: EDIT_PROPS, editorList: commonProperties, cat: PROP_GENERAL,desc:'Common properties for most elements' });
								generalAdded = true;
							}
							props.push(props1[i]);
						}
						if (!noCommonProps && !generalAdded) {
							props.push({ name: 'general', editor: EDIT_PROPS, editorList: commonProperties, cat: PROP_GENERAL, desc: 'Common properties for most elements' });
						}
						if (isSvgShape) {
							for (i = 0; i < svgshapeProps.length; i++) {
								if (notclose && svgshapeProps[i].closedonly)
									continue;
								if (issvgtext && svgshapeProps[i].nottext)
									continue;
								props.push(svgshapeProps[i]);
							}
						}
						if (onpropscreated) {
							onpropscreated(c, props);
						}
						return { props: props, objSetter: objSetter };
					}
				}
			}
		}
		function getPropertyCell(propName) {
			for (var i = 0; i < _propsBody.rows.length; i++) {
				if (_propsBody.rows[i].cells[0].colSpan != 2) {
					if (_propsBody.rows[i].cells[1].propertyDescriptor.name == propName) {
						return _propsBody.rows[i].cells[1];
					}
				}
			}
		}
		function applyValue(td, val) {
			td.value = val;
			var v = val;
			if (td.propertyDescriptor.isPixel || td.propertyDescriptor.canbepixel) {
				if (typeof v != 'undefined' && v != null && JsonDataBinding.isNumber(v)) {
					v = v + 'px';
				}
			}
			else if (td.propertyDescriptor.isUrl) {
				v = JsonDataBinding.getUrlFromPath(v, _webPath);
			}
			else if (td.propertyDescriptor.isFilePath) {
				if (typeof v != 'undefined' && v != null && v.length > 0 && v.indexOf('.') < 0) {
					if (!JsonDataBinding.startsWithI(v, 'javascript:')) {
						v = v + '.html';
					}
				}
			}
			if (td.curObj.jsData && td.curObj.jsData.designer && td.curObj.jsData.designer.hasSetter && td.curObj.jsData.designer.hasSetter(td.propertyDescriptor.name)) {
				td.curObj.jsData.designer.setter(td.propertyDescriptor.name, v);
			}
			else {
				var obj = td.curObj.subEditor ? td.curObj.obj : td.curObj;
				if (td.propertyDescriptor.setter) {
					td.propertyDescriptor.setter(obj, v);
				}
				else {
					if (td.propertyDescriptor.forStyle) {
						var c = HtmlEditor.getCssNameFromPropertyName(td.propertyDescriptor.name);
						_setElementStyleValue(td.curObj, td.propertyDescriptor.name, c, v);
						if (_editorOptions.forIDE) {
							if (td.curObj.tagName.toLowerCase() == 'body') {
								try {
									if (typeof (limnorStudio) != 'undefined') limnorStudio.onBodyStyleChanged(td.propertyDescriptor.name, v);
									else window.external.OnBodyStyleChanged(td.propertyDescriptor.name, v);
								}
								catch (err) {
									alert('body:'+err.message);
								}
							}
						}
					}
					else if (td.propertyDescriptor.byAttribute) {
						if (_isIE && td.propertyDescriptor.name == 'name') {
							obj.setAttribute('Name', v);
							obj.name = v;
						}
						else {
							if (typeof v == 'undefined' || v == null || v.length == 0) {
								obj.setAttribute(td.propertyDescriptor.name, '');
								obj.getAttribute(td.propertyDescriptor.name);
								obj.removeAttribute(td.propertyDescriptor.name);
							}
							else {
								obj.setAttribute(td.propertyDescriptor.name, v);
							}
						}
					}
					else {
						if (td.propertyDescriptor.cssName) {
							setCSStext(obj, td.propertyDescriptor.cssName, v);
						}
						else {
							try {
								if (td.propertyDescriptor.editor == EDIT_NUM && isNaN(v)) {
									v = '';
									if (td.propertyDescriptor.name == 'size') {
										obj.size = 1;
									}
									else {
										obj.setAttribute(td.propertyDescriptor.name, '');
										obj.removeAttribute(td.propertyDescriptor.name);
									}
								}
								else {
									if (_isIE && td.propertyDescriptor.name == 'name') {
										obj.setAttribute('Name', v);
										obj.name = v;
									}
									else {
										obj[td.propertyDescriptor.name] = v;
									}
								}
							}
							catch (exp2) {
								alert('Error setting property ' + td.propertyDescriptor.name + ' to ' + v + '. It might be a browser compatibility issue. Error message: ' + exp2.message + '. ' + HtmlEditor.GetIECompatableWarning());
							}
						}
					}
				}
				if (td.propertyDescriptor.editor == EDIT_CUST) {
					if (!v || v.length == 0) {
						obj.removeAttribute(td.propertyDescriptor.name);
					}
				}
				else if (td.propertyDescriptor.forStyle && (td.propertyDescriptor.name == 'width' || td.propertyDescriptor.name == 'height')) {
					if (obj.tagName && obj.tagName.toLowerCase() == 'iframe') {
						obj.removeAttribute(td.propertyDescriptor.name);
					}
				}
			}
			if (td.propertyDescriptor.onsetprop) {
				td.propertyDescriptor.onsetprop(td.curObj, v);
			}
			_editorOptions.pageChanged = true;
		}
		function onTextColorChanged(c) {
			var td = this.propCell;
			applyValue(td, c);
		}
		function ontxtChange(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && typeof c.value != 'undefined') {
				var td = c.propCell;
				if (td && td.propertyDescriptor) {
					var val = c.value;
					if (td.propertyDescriptor.IsDocType) {
						_editorOptions.docType = val;
						td.value = val;
					}
					else {
						if (td.propertyDescriptor.editor == EDIT_NUM) {
							try {
								val = parseInt(c.value);
							}
							catch (er) {
								return;
							}
						}
						applyValue(td, val);
					}
				}
			}
		}
		function onseltextChange(e) {
			var c = JsonDataBinding.getSender(e);
			if (c) {
				var td = c.getParentNode ? c.getParentNode() : c.parentNode;
				if (td && td.propertyDescriptor) {
					if (td.propertyDescriptor.ontextchange) {
						var sels = td.getElementsByTagName('select');
						if (sels && sels.length > 0 && c != sels[0]) {
							var val = td.propertyDescriptor.ontextchange((c.getValue ? c.getValue() : c.value), sels[0]);
							if (val) {
								applyValue(td, val);
							}
						}
						else {
							applyValue(td, (c.getValue ? c.getValue() : c.value));
						}
					}
					else {
						applyValue(td, (c.getValue ? c.getValue() : c.value));
					}
				}
			}
		}
		function onselChildChange(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.selectedIndex >= 0) {
				var val = c.options[c.selectedIndex].objvalue;
				if (val) {
					selectEditElement(val);
				}
			}
		}
		function onselChange_menum(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.selectedIndex >= 0) {
				var td = c.parentNode;
				if (td && td.propertyDescriptor) {
					var val = c.options[c.selectedIndex].value;
					if (td.propertyDescriptor.editor == EDIT_NUM) {
						try {
							val = parseInt(val);
						}
						catch (er) {
							return;
						}
					}
					else if (td.propertyDescriptor.editor == EDIT_MENUM) {
						if (td.propertyDescriptor.onselectindexchanged) {
							if (td.propertyDescriptor.onselectindexchanged(c)) {
								if (c.texteditor) {
									c.texteditor.style.display = 'none';
								}
							}
							else {
								if (c.texteditor) {
									c.texteditor.value = c.texteditor.value + ',' + val;
									c.texteditor.style.display = 'inline';
								}
								return;
							}
						}
					}
					applyValue(td, val);
				}
			}
		}
		function onselChange(e) {
			var c = JsonDataBinding.getSender(e);
			if (c && c.selectedIndex >= 0) {
				var td = c.parentNode;
				if (td && td.propertyDescriptor) {
					var val = c.options[c.selectedIndex].value;
					if (td.propertyDescriptor.editor == EDIT_NUM) {
						try {
							val = parseInt(val);
						}
						catch (er) {
							return;
						}
					}
					else if (td.propertyDescriptor.editor == EDIT_ENUM) {
						if (td.propertyDescriptor.onselectindexchanged) {
							if (td.propertyDescriptor.onselectindexchanged(c)) {
								if (c.texteditor) {
									c.texteditor.style.display = 'none';
								}
							}
							else {
								if (c.texteditor) {
									c.texteditor.style.display = 'inline';
								}
								return;
							}
						}
					}
					applyValue(td, val);
				}
			}
		}
		function ontxtClick(e) {
			if (_editorOptions.isEditingBody) {
				_editorOptions.elementEditedDoc.body.contentEditable = false;
			}
		}
		function ontxtBlur(e) {
			if (_editorOptions.isEditingBody) {
				_editorOptions.elementEditedDoc.body.contentEditable = true;
			}
		}
		function onSwitchToObject(event) {
			var c = JsonDataBinding.getSender(event);
			if (c && c.owner) {
				var obj = c.owner;
				var td = c.parentNode;
				if (td) {
					if (td.propertyDescriptor && td.propertyDescriptor.getter) {
						docClick({ target: td.propertyDescriptor.getter(obj) });
					}
					else {
						docClick({ target: obj.parentNode });
					}
				}
			}
		}
		function colorToHex(c) {
			var m = /rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/.exec(c);
			return m ? '#' + (1 << 24 | m[1] << 16 | m[2] << 8 | m[3]).toString(16).substr(1) : c;
		}
		function nodeToString(o, a) {
			if (a && typeof a == 'function') {
				return a(o);
			}
			var s = (o.id ? o.id : '') + (o.name ? ' - ' + o.name : '') + (o.getAttribute(a) ? ' ' + o.getAttribute(a) + ':' : '');
			var d = (o.nodeType == 3) ? o.data : o.outerHTML;
			if (d) {
				d = d.replace('<', '');
				d = d.replace('>', '');
				s += d.substr(0, 30);
			}
			return s;
		}
		function verifyEditorOwner() {
			var i;
			if (_topElements) {
				for (i = 0; i < _topElements.length; i++) {
					if (_topElements[i].parentNode != document.body) {
						document.body.appendChild(_topElements[i]);
					}
				}
			}
			if (_propertyTopElements) {
				for (i = 0; i < _propertyTopElements.length; i++) {
					if (_propertyTopElements[i].parentNode != document.body) {
						document.body.appendChild(_propertyTopElements[i]);
					}
				}
			}
			//if (_editorOptions && _editorOptions.client) {
			//	_editorOptions.client.verifyEditorOwner();
			//}
		}
		function isCssNode(c) {
			if (c && c.tagName && c.tagName.toLowerCase() == 'link') {
				var rel = c.getAttribute('rel');
				if (rel && rel == 'stylesheet') {
					return true;
				}
			}
			return false;
		}
		function clearPropertiesDisplay() {
			if (_propertyTopElements) {
				for (var k = 0; k < _propertyTopElements.length; k++) {
					if (_propertyTopElements[k]) {
						var pn = _propertyTopElements[k].parentNode;
						if (pn) {
							pn.removeChild(_propertyTopElements[k]);
						}
					}
				}
			}
			_parentList.options.length = 0;
			_tdTitle.innerHTML = HtmlEditor.version;
			while (_propsBody.rows.length > 0) {
				_propsBody.deleteRow(_propsBody.rows.length - 1);
			}
			if (_elementLocator) {
				_elementLocator.style.display = 'none';
			}
			if (_textInput) {
				_textInput.style.display = 'none';
			}
			_tdObj.innerHTML = '';
			_trObjDelim.style.display = 'none';
			showFontCommands();
		}
		function tooltipsOwnerMouseMove(e) {
			var obj = JsonDataBinding.getSender(e);
			if (obj && (obj.toolTips || obj.tooltipsSpan)) {
				if (!obj.moves) {
					if (_divToolTips) {
						_divToolTips.style.display = 'none';
					}
					obj.moves = 1;
				}
				else {
					obj.moves++;
					if (obj.moves == 15) {
						showToolTips(obj, obj.tooltipsSpan ? obj.innerHTML : obj.toolTips);
					}
				}
			}
		}
		function tooltipsOwnerMouseOut(e) {
			var obj = JsonDataBinding.getSender(e);
			if (obj && (obj.toolTips || obj.tooltipsSpan)) {
				obj.moves = 0;
				if (_divToolTips) {
					_divToolTips.style.display = 'none';
				}
			}
		}
		function selectWebFile(e) {
			if (_editorOptions && _editorOptions.isEditingBody) {
				var img = JsonDataBinding.getSender(e);
				if (img && img.ownertextbox) {
					var msize = 1024;
					if (typeof img.propertyDesc.maxSize != 'undefined') {
						msize = img.propertyDesc.maxSize;
					}
					if (_editorOptions.forIDE) {
						var file;
						if (typeof (limnorStudio) != 'undefined') file = limnorStudio.onSelectFile(img.propertyDesc.title, img.propertyDesc.filetypes, img.subFolder, img.subName, msize, img.propertyDesc.disableUpload);
						else file = window.external.OnSelectFile(img.propertyDesc.title, img.propertyDesc.filetypes, img.subFolder, img.subName, msize, img.propertyDesc.disableUpload);
						if (file) {
							img.ownertextbox.value = file;
							_editorOptions.pageChanged = true;
							if (img.ownertextbox.ontxtChange) {
								img.ownertextbox.ontxtChange({ target: img.ownertextbox });
							}
						}
					}
					else {
						HtmlEditor.PickWebFile(img.propertyDesc.title, img.propertyDesc.filetypes, function (file) {
							if (typeof file != 'undefined' && file != null && file.length > 0) {
								if (_editorOptions && typeof _editorOptions.serverFolder != 'undefined' && _editorOptions.serverFolder != null && _editorOptions.serverFolder.length > 0) {
									if (JsonDataBinding.startsWithI(file, _editorOptions.serverFolder)) {
										file = file.substr(_editorOptions.serverFolder.length);
										if (file.length > 0) {
											if (file.charAt(0) == '/' || file.charAt(0) == '\\') {
												file = file.substr(1);
											}
										}
									}
								}
							}
							img.ownertextbox.value = file;
							_editorOptions.pageChanged = true;
							if (img.ownertextbox.ontxtChange) {
								img.ownertextbox.ontxtChange({ target: img.ownertextbox });
							}
						}, img.propertyDesc.disableUpload, img.subFolder, img.subName, msize);
					}
				}
			}
		}
		function onremovecolor(e) {
			var img = JsonDataBinding.getSender(e);
			if (img && img.owner && img.owner.curObj && img.owner.propertyDescriptor && img.txtbox) {
				if (img.owner.curObj.jsData && img.owner.curObj.jsData.designer && img.owner.curObj.jsData.designer.removeColor) {
					img.owner.curObj.jsData.designer.removeColor.apply(_editorOptions.elementEditedWindow, [img.owner.curObj, img.owner.propertyDescriptor.name, HtmlEditor.getCssNameFromPropertyName(img.owner.propertyDescriptor.name)]);
				}
				else {
					if (img.owner.propertyDescriptor.name == 'linearGradientStartColor') {
						_editorOptions.client.setLinearGradientStartColor.apply(_editorOptions.editorWindow, [img.owner.curObj, null]);
					}
					else if (img.owner.propertyDescriptor.name == 'linearGradientEndColor') {
						_editorOptions.client.setLinearGradientEndColor.apply(_editorOptions.editorWindow, [img.owner.curObj, null]);
					}
					else {
						_setElementStyleValue(img.owner.curObj, img.owner.propertyDescriptor.name, HtmlEditor.getCssNameFromPropertyName(img.owner.propertyDescriptor.name), ''); //_editorOptions.client.setElementStyleValue.apply(_editorOptions.elementEditedWindow, [img.owner.curObj, img.owner.propertyDescriptor.name, HtmlEditor.getCssNameFromPropertyName(img.owner.propertyDescriptor.name), '']);
					}
				}
				img.txtbox.value = "";
				img.txtbox.style.backgroundColor = 'transparent';
			}
		}
		function refreshPropertyDisplay(name, v) {
			for (var i = 0; i < _propsBody.rows.length; i++) {
				if (_propsBody.rows[i].cells.length > 1) {
					var pd = _propsBody.rows[i].cells[0].propDesc;
					if (pd && pd.name == name) {
						_propsBody.rows[i].cells[1].innerHTML = v;
						break;
					}
				}
			}
		}
		function execAct(act) {
			return function (e) {
				act(e);
				if(typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
			}
		}
		function oncolorboxkeyup(e) {
			var txt = JsonDataBinding.getSender(e);
			if (txt && txt.propCell) {
				if (txt.value) {
					var c = txt.value;								   
					if (!JsonDataBinding.startsWith(c, '#')) {
						c = '#' + c;
					}
					var isColor = /(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(c);
					if (isColor) {
						onTextColorChanged.apply(txt, [c]);
					}
				}
			}
		}
		function showProperties(c) {
			if (!_editorOptions) return;
			_custMouseDown = null;
			_custMouseDownOwner = null;
			_comboInput = null;
			_editorOptions.propertiesOwner = c;
			_editorOptions.styleChanged = false;
			if (_editorOptions.markers) {
				for (var k = 0; k < _editorOptions.markers.length; k++) {
					if (_editorOptions.markers[k]) {
						var pn = _editorOptions.markers[k].parentNode;
						if (pn) {
							pn.removeChild(_editorOptions.markers[k]);
						}
					}
				}
				delete _editorOptions.markers;
			}
			if (_propertyTopElements) {
				for (var k = 0; k < _propertyTopElements.length; k++) {
					if (_propertyTopElements[k]) {
						var pn = _propertyTopElements[k].parentNode;
						if (pn) {
							pn.removeChild(_propertyTopElements[k]);
						}
					}
				}
			}
			var iscss = isCssNode(c);
			var cap = 'Properties - ' + elementToString(c) + '(' + (_editorOptions.useSavedUrl ? _editorOptions.elementEditedWindow.location.pathname.substr(0, _editorOptions.elementEditedWindow.location.pathname.length - 14) : _editorOptions.elementEditedWindow.location.pathname) + ')';
			try {
				_tdTitle.innerHTML = cap;
			}
			catch (err) {
			}
			while (_propsBody.rows.length > 0) {
				_propsBody.deleteRow(_propsBody.rows.length - 1);
			}
			if (!c.subEditor) {
				_tdObj.innerHTML = '';
				_trObjDelim.style.display = 'none';
			}
			_combos = null;
			var props;
			var tag;
			var attrs = { desc: '' };
			if (c.subEditor) {
				props = c.getProperties();
			}
			else {
				tag = c.tagName.toLowerCase();
				var tn;
				if (c.typename) {
					tn = c.typename;
				}
				else {
					if (c.getAttribute) {
						tn = c.getAttribute('typename');
					}
				}
				if (!tn || tn.length == 0) {
					tn = tag;
				}
				props = _getElementProperties(tag, c, attrs, tn);
			}
			var properties = props ? props.props : null;
			_setMessage(attrs.desc);
			if (!properties) {
				properties = [{ name: 'delete', desc: 'delete the element', editor: EDIT_DEL}];
			}
			_divElementToolbar.innerHTML = '';
			if (properties) {
				var cmdImg;
				if (props && props.objSetter) {
					_trObjDelim.style.display = '';
					props.objSetter(c);
				}
				for (var i = 0; i < properties.length; i++) {
					cmdImg = null;
					if (!properties[i]) {
						throw { message: 'element ' + (tag ? tag : '') + ' does not specify property ' + i.toString() + '. Property array may has an extra comma.' };
					}
					if (properties[i].editor == EDIT_CMD || properties[i].editor == EDIT_DEL || properties[i].editor == EDIT_CMD2 || properties[i].editor == EDIT_CMD3 || properties[i].editor == EDIT_GO) {
						if (properties[i].showCommand) {
							if (!properties[i].showCommand(c)) {
								continue;
							}
						}
						if (properties[i].editor == EDIT_CMD && properties[i].isInit) {
							if (properties[i].act) {
								properties[i].act(c);
							}
							continue;
						}
						if (properties[i].editor == EDIT_CMD && properties[i].isText) {
							var txtsvg = document.createElement('input');
							txtsvg.type = 'text';
							//txtsvg.forProperties = true;
							//txtsvg.contentEditable = false;
							txtsvg.owner = c;
							txtsvg.style.fontSize = 'x-small';
							txtsvg.style.position = 'relative';
							txtsvg.style.width = '30px';
							txtsvg.style.height = '16px';
							txtsvg.style.top = '-5px';
							txtsvg.readOnly = false;
							txtsvg.value = properties[i].getter(c);
							_divElementToolbar.appendChild(txtsvg);
							JsonDataBinding.AttachEvent(txtsvg, "onchange",
								function (settxt) {
									return function (e) {
										settxt(c, txtsvg.value);
									};
								}(properties[i].setter));
							continue;
						}
						if (properties[i].editor == EDIT_CMD2) {
							cmdImg = document.createElement("img");
							if (properties[i].IMG)
								cmdImg.src = properties[i].IMG;
							else
								cmdImg.src = '/libjs/removeTag.png';
							cmdImg.forProperties = true;
							cmdImg.contentEditable = false;
							cmdImg.owner = c;
							cmdImg.style.cursor = 'pointer';
							_divElementToolbar.appendChild(cmdImg);
							if (properties[i].action){
								if(properties[i].noModify)
									JsonDataBinding.AttachEvent(cmdImg, "onclick", properties[i].action);
								else
									JsonDataBinding.AttachEvent(cmdImg, "onclick", execAct(properties[i].action));
							}
							else
								JsonDataBinding.AttachEvent(cmdImg, "onclick", stripTag);
						}
						else if (properties[i].editor == EDIT_CMD3) {
							var pr = c.parentNode;
							if (pr && pr != _editorOptions.elementEditedDoc.body) {
								cmdImg = document.createElement("img");
								if (properties[i].IMG)
									cmdImg.src = properties[i].IMG;
								else
									cmdImg.src = '/libjs/removeOutTag.png';
								cmdImg.forProperties = true;
								cmdImg.contentEditable = false;
								cmdImg.owner = c;
								cmdImg.style.cursor = 'pointer';
								_divElementToolbar.appendChild(cmdImg);
								if (properties[i].action) {
									if(properties[i].notModify)
										JsonDataBinding.AttachEvent(cmdImg, "onclick", properties[i].action);
									else
										JsonDataBinding.AttachEvent(cmdImg, "onclick",execAct(properties[i].action));
								}
								else
									JsonDataBinding.AttachEvent(cmdImg, "onclick", moveOutTag);
							}
						}
						else if (properties[i].editor == EDIT_GO) {
							var show = true;
							var bt = null;
							if (properties[i].showCommand) {
								if (!properties[i].showCommand(c)) {
									show = false;
								}
							}
							if (!properties[i].IMG) {
								bt = document.createElement('button');
								bt.forProperties = true;
								bt.contentEditable = false;
								bt.owner = c;
								bt.propDesc = properties[i];
								bt.isCommandBar = true;
								_divElementToolbar.appendChild(bt);
							}
							if (show) {
								if (properties[i].cmdText) {
									cmdImg = null;
									var btText = document.createElement('span');
									btText.innerHTML = properties[i].cmdText;
									btText.style.color = 'green';
									btText.owner = c;
									btText.isCommandBar = true;
									btText.propDesc = properties[i];
									bt.appendChild(btText);
								}
								else {
									cmdImg = document.createElement("img");
									if (properties[i].IMG) {
										cmdImg.src = properties[i].IMG;
									}
									else {
										cmdImg.src = '/libjs/go.gif';
									}
									cmdImg.style.cursor = 'pointer';
								}
							}
							else {
								cmdImg = document.createElement("img");
								cmdImg.src = '/libjs/go_inact.gif';
								cmdImg.style.cursor = 'arrow';
								if (bt) {
									bt.disabled = true;
								}
							}
							if (cmdImg) {
								cmdImg.forProperties = true;
								cmdImg.contentEditable = false;
								cmdImg.owner = c;
								if (bt) {
									bt.appendChild(cmdImg);
								}
								else {
									_divElementToolbar.appendChild(cmdImg);
								}
							}
							if (bt) {
								if (properties[i].action) {
									if (properties[i].notModify) {
										JsonDataBinding.AttachEvent(bt, "onclick", properties[i].action);
									}
									else {
										JsonDataBinding.AttachEvent(bt, "onclick", execAct(properties[i].action));
									}
								}
								else {
									JsonDataBinding.AttachEvent(bt, "onclick", gotoChildByTag);
								}
							}
							else {
								if (properties[i].action) {
									if (properties[i].notModify) {
										JsonDataBinding.AttachEvent(cmdImg, "onclick", properties[i].action);
									}
									else {
										JsonDataBinding.AttachEvent(cmdImg, "onclick", execAct(properties[i].action));
									}
								}
								else {
									JsonDataBinding.AttachEvent(cmdImg, "onclick", gotoChildByTag);
								}
							}
						}
						else if (properties[i].createCommand) {
							properties[i].createCommand(c, _divElementToolbar);
						}
						else {
							cmdImg = document.createElement("img");
							if (properties[i].editor == EDIT_DEL) {
								cmdImg.src = '/libjs/delElement.png';
							}
							else {
								cmdImg.src = properties[i].IMG;
							}
							cmdImg.forProperties = true;
							cmdImg.contentEditable = false;
							cmdImg.owner = c;
							cmdImg.style.cursor = 'pointer';
							_divElementToolbar.appendChild(cmdImg);
							if (properties[i].editor == EDIT_DEL) {
								if (properties[i].action) {
									JsonDataBinding.AttachEvent(cmdImg, "onclick", function(h) {
										return function(e) {
											var c = JsonDataBinding.getSender(e);
											if (c && c.owner) {
												h(c.owner);
												if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
											}
										}
									} (properties[i].action));
								}
								else {
									JsonDataBinding.AttachEvent(cmdImg, "onclick", deleteElement);
								}
							}
							else {
								if (properties[i].action) {
									JsonDataBinding.AttachEvent(cmdImg, "onclick", execAct(properties[i].action));
								}
							}
						}
						if (cmdImg) {
							cmdImg.propDesc = properties[i];
							cmdImg.forProperties = true;
							cmdImg.contentEditable = false;
							cmdImg.style.cursor = 'pointer';
							if (properties[i].toolTips) {
								hookTooltips(cmdImg, properties[i].toolTips);
							}
							else if (properties[i].desc) {
								hookTooltips(cmdImg, properties[i].desc);
							}
							if (cmdImg.parentNode == _divElementToolbar) {
								var imgVsep = document.createElement('img');
								imgVsep.contentEditable = false;
								imgVsep.forProperties = true;
								imgVsep.border = 0;
								imgVsep.src = '/libjs/vSep.png';
								imgVsep.style.display = 'inline';
								_divElementToolbar.appendChild(imgVsep);
							}
						}
					}
				}
				function showProperty(tbody, propertyDesc, cat, pridx) {
					if (typeof pridx == 'undefined') {
						pridx = -1;
					}
					if (propertyDesc.editor == EDIT_PROPS) {
						if (propertyDesc.getter) {
							var props = propertyDesc.getter(c);
							if (props) {
								for (var k0 = 0; k0 < props.length; k0++) {
									showProperty(tbody, props[k0], null);
								}
							}
						}
						else {
							function loadPropInCat(pridx0) {
								var props;
								if (typeof propertyDesc.editorList == 'function') {
									props = propertyDesc.editorList(c);
								}
								else {
									props = propertyDesc.editorList;
								}
								if (props) {
									for (var k0 = 0; k0 < props.length; k0++) {
										if (_editorOptions.isEditingBody) {
											if (tag == 'td' && (props[k0].name == 'width' || props[k0].name == 'height')) {
												continue;
											}
										}
										else {
											if (props[k0].name == 'class' || props[k0].name == 'styleName') {
												continue;
											}
										}
										pridx0++;
										showProperty(tbody, props[k0], propertyDesc.cat, pridx0);
									}
								}
							}
							if (propertyDesc.cat) {
								//create a category row for toggling
								function toggleRows(e) {
									var c = JsonDataBinding.getSender(e);
									if (c.row) {
										function get_nextsibling(n) {
											x = n.nextSibling;
											while (x && x.nodeType != 1) {
												x = x.nextSibling;
											}
											return x;
										}
										c.row.expanded = !c.row.expanded;
										if (c.row.expanded) {
											c.src = '/libjs/expand_min.png';
											if (!c.row.ploaded) {
												c.row.ploaded = true;
												loadPropInCat(c.row.rowIndex);
											}
										}
										else {
											c.src = '/libjs/expand_res.png';
										}
										var r = get_nextsibling(c.row);
										while (r) {
											if (r.cat != c.row.cat) break;
											if (c.row.expanded) {
												r.style.display = '';
											}
											else {
												r.style.display = 'none';
											}
											r = get_nextsibling(r);
										}
									}
								}
								var cattr = tbody.insertRow(-1);
								cattr.style.backgroundColor = getPropCatColor(propertyDesc.cat);
								cattr.cat = propertyDesc.cat;
								cattr.expanded = false;
								cattr.ploaded = false;
								var cattd = document.createElement('td');
								cattr.appendChild(cattd);
								cattd.colSpan = 2;
								cattd.propDesc = propertyDesc;
								var catimg = document.createElement('img');
								catimg.src = '/libjs/expand_res.png';
								catimg.style.display = 'inline';
								catimg.style.cursor = 'pointer';
								cattd.appendChild(catimg);
								catimg.onclick = toggleRows;
								catimg.row = cattr;
								var catsp = document.createElement('span');
								catsp.style.display = 'inline';
								catsp.style.paddingLeft = '2px';
								catsp.innerHTML = propertyDesc.name;
								catsp.propDesc = propertyDesc;
								cattd.appendChild(catsp);
							}
							else {
								loadPropInCat(-1);
							}
						}
						return;
					}
					if (!_editorOptions.isEditingBody && propertyDesc.bodyOnly) {
						return;
					}
					if (propertyDesc.needGrow) {
						propertyDesc.grow = (function (idx0) { return function (xprop, idxd) { showProperty(tbody, xprop, propertyDesc.cat, idx0 + idxd); }; })(pridx);
					}
					var tdName;
					var tr = tbody.insertRow(pridx);
					if (cat) {
						tr.style.backgroundColor = getPropCatColor(cat);
						tr.cat = cat;
					}
					else {
						tr.style.backgroundColor = 'white';
					}
					tr.contentEditable = false;
					var td = document.createElement("td");
					tdName = td;
					td.contentEditable = false;
					td.propDesc = propertyDesc;
					td.innerHTML = propertyDesc.name;
					td.style.width = tbody.nameWidth + 'px';
					tr.appendChild(td);
					td = document.createElement("td");
					td.contentEditable = false;
					td.style.width = '100%';
					tr.appendChild(td);
					var s;
					if (c.jsData && c.jsData.designer && c.jsData.designer.hasSetter && c.jsData.designer.hasGetter(propertyDesc.name)) {
						s = c.jsData.designer.getter(propertyDesc.name);
					}
					else {
						if (propertyDesc.getter) {
							s = propertyDesc.getter(c);
						}
						else {
							if (propertyDesc.forStyle) {
								if (_editorOptions.isEditingBody) {
									s = _editorOptions.client.getElementStyleSetting.apply(_editorOptions.elementEditedWindow, [c.subEditor ? c.obj : c, HtmlEditor.getCssNameFromPropertyName(propertyDesc.name), propertyDesc.name]);
								}
								else {
									if (c.subEditor)
										s = c.obj.style[propertyDesc.name];
									else
										s = c.style[propertyDesc.name];
								}
							}
							else if (propertyDesc.byAttribute) {
								if (c.subEditor)
									s = c.obj.getAttribute(propertyDesc.name);
								else
									s = c.getAttribute(propertyDesc.name);
							}
							else {
								if (propertyDesc.IsDocType) {
									if (_editorOptions.docType)
										s = _editorOptions.docType;
									else {
										_editorOptions.docType = _editorOptions.client.getDocType.apply(_editorOptions.elementEditedWindow);
										if (_editorOptions.docType)
											s = _editorOptions.docType;
										else
											s = '';
									}
								}
								else {
									if (propertyDesc.name == 'name' && _isIE) {
										if (c.subEditor)
											s = c.obj.getAttribute('Name');
										else
											s = c.getAttribute('Name');
									}
									else {
										if (c.subEditor)
											s = c.obj[propertyDesc.name];
										else
											s = c[propertyDesc.name];
									}
								}
							}
							if (propertyDesc.isUrl || propertyDesc.isFilePath) {
								s = JsonDataBinding.getPathFromUrl(s, _webPath);
							}
						}
					}
					if (typeof s == 'undefined') s = null;
					var sel;
					var bt;
					var cmdImg;
					var elOptNew;
					td.value = s;
					td.tdName = tdName;
					if (propertyDesc.editor == EDIT_NONE) {
						if (propertyDesc.action) {
							var cmdImg = document.createElement("img");
							if (propertyDesc.IMG)
								cmdImg.src = propertyDesc.IMG;
							else
								cmdImg.src = '/libjs/go.gif';
							cmdImg.forProperties = true;
							cmdImg.contentEditable = false;
							cmdImg.owner = c;
							cmdImg.style.cursor = 'pointer';
							cmdImg.propDesc = propertyDesc;
							cmdImg.ownerTd = td;
							td.appendChild(cmdImg);
							JsonDataBinding.AttachEvent(cmdImg, "onclick", propertyDesc.action);
						}
						if (typeof s != 'undefined' && s != null) {
							td.appendChild(document.createTextNode(s));
						}
						else {
							td.innerHTML = "&nbsp;";
						}
					}
					else if (propertyDesc.editor == EDIT_NODES) {
						var isCss = (propertyDesc.name == 'css');
						var isLink = (propertyDesc.name == 'link');
						var isMeta = (propertyDesc.name == 'meta');
						var isScript = (propertyDesc.name == 'script');
						var cs = c.getElementsByTagName(isCss ? 'link' : propertyDesc.name);
						if (cs && cs.length > 0) {
							var first = true;
							var pagecss;
							if (isCss) {
								pagecss = _editorOptions.client.getPageCssFilename.apply(_editorOptions.elementEditedWindow);
								if (pagecss) pagecss = pagecss.toLowerCase();
							}
							for (var k = 0; k < cs.length; k++) {
								if (isCss) {
									var rel = cs[k].getAttribute('rel');
									if (!rel) continue;
									if (rel.toLowerCase() != 'stylesheet') continue;
									var f = cs[k].getAttribute('href');
									if (f) {
										var pos = f.indexOf('?');
										if (pos >= 0) {
											f=f.substr(0, pos);
										}
										f = JsonDataBinding.urlToFilename(f);
										if (f.toLowerCase() == pagecss) {
											continue;
										}
									}
								}
								else if (isLink) {
									var rel = cs[k].getAttribute('rel');
									if (rel && rel.toLowerCase() == 'stylesheet')
										continue;
								}
								else if (isMeta) {
									var nm = cs[k].getAttribute('name') || cs[k].name;
									if (nm) {
										nm = nm.toLowerCase();
										if (nm == 'generator' || nm == 'keywords' || nm == 'description' || nm == 'author')
											continue;
									}
									nm = cs[k].getAttribute('http-equiv');
									if (nm) {
										nm = nm.toLowerCase();
										if (nm == 'content-type' || nm == 'pragma' || nm == 'x-ua-compatible')
											continue;
									}
									nm = cs[k].getAttribute('content');
									if (nm) {
										nm = nm.toLowerCase();
										if (nm == 'no-cache' || nm == 'text/html; charset=utf-8' || nm == 'ie=edge')
											continue;
									}
								}
								else if (isScript) {
									var nm = cs[k].getAttribute('hidden');
									if (nm) {
										continue;
									}
									else {
										var f = cs[k].getAttribute('src');
										if (f) {
											var pos = f.indexOf('?');
											if (pos >= 0) {
												f = f.substr(0, pos);
											}
											f = JsonDataBinding.urlToFilename(f);
											if (f.toLowerCase() == 'ip2cty.php') {
												continue;
											}
										}
									}
								}
								if (first)
									first = false;
								else {
									var br = document.createElement('br');
									td.appendChild(br);
								}
								var o = cs[k];
								bt = document.createElement('button');
								bt.forProperties = true;
								bt.contentEditable = false;
								bt.owner = o;
								cmdImg = document.createElement("img");
								cmdImg.src = '/libjs/go.gif';
								cmdImg.forProperties = true;
								cmdImg.contentEditable = false;
								bt.appendChild(cmdImg);
								var sp = document.createElement('span');
								sp.innerHTML = nodeToString(o, propertyDesc.sig);
								sp.owner = o;
								bt.appendChild(sp);
								td.appendChild(bt);
								JsonDataBinding.AttachEvent(bt, "onclick", gotoElement);
							}
						}
					}
					else if (propertyDesc.editor == EDIT_CHILD) {
						sel = document.createElement("select");
						sel.contentEditable = false;
						sel.forProperties = true;
						td.appendChild(sel);
						var tags = propertyDesc.childTag.split(',');
						for (var z = 0; z < tags.length; z++) {
							var children = c.getElementsByTagName(tags[z]);
							if (children) {
								for (var j = 0; j < children.length; j++) {
									elOptNew = document.createElement('option');
									elOptNew.text = tags[z] + ' - ' + (children[j].id ? children[j].id : '') + ', ' + (children[j].name ? children[j].name : '');
									elOptNew.objvalue = children[j];
									addOptionToSelect(sel, elOptNew);
								}
								sel.selectedIndex = -1;
							}
						}
						JsonDataBinding.AttachEvent(sel, "onchange", onselChildChange);
					}
					else if (propertyDesc.editor == EDIT_PARENT) {
						bt = document.createElement("input");
						bt.type = "button";
						bt.value = properties[i].name;
						bt.owner = _editorOptions.selectedObject;
						bt.forProperties = true;
						bt.contentEditable = false;
						td.appendChild(bt);
						JsonDataBinding.AttachEvent(bt, "onclick", onSwitchToObject);
					}
					else if (propertyDesc.editor == EDIT_COLOR) {
						var txt = document.createElement("input");
						txt.type = "text";
						txt.className = "color";
						td.contentEditable = false;
						txt.forProperties = true;
						txt.style.width = '60px'; //'100%';
						txt.propCell = td;
						td.appendChild(txt);
						txt.value = colorToHex(s);
						var btsetdef = document.createElement("img");
						btsetdef.src = '/libjs/cancel.png';
						btsetdef.style.cursor = 'pointer';
						btsetdef.owner = td;
						btsetdef.txtbox = txt;
						hookTooltips(btsetdef, 'Remove color setting so that the default color or inherit color will be used');
						td.appendChild(btsetdef);
						jscolor.bind1(txt);
						JsonDataBinding.addTextBoxObserver(txt);
						txt.oncolorchanged = onTextColorChanged;
						txt.tdName = tdName;
						JsonDataBinding.AttachEvent(btsetdef, "onclick", onremovecolor);
						JsonDataBinding.AttachEvent(txt, "onkeyup", oncolorboxkeyup);
					}
					else if (propertyDesc.editor == EDIT_BOOL) {
						sel = document.createElement("select");
						sel.contentEditable = false;
						sel.forProperties = true;
						td.appendChild(sel);
						sel.tdName = tdName;
						JsonDataBinding.AttachEvent(sel, "onchange", onselChange);
						elOptNew = document.createElement('option');
						elOptNew.text = '';
						elOptNew.value = '';
						addOptionToSelect(sel, elOptNew);
						elOptNew = document.createElement('option');
						elOptNew.text = 'false';
						elOptNew.value = false;
						addOptionToSelect(sel, elOptNew);
						elOptNew = document.createElement('option');
						elOptNew.text = 'true';
						elOptNew.value = true;
						addOptionToSelect(sel, elOptNew);
						if (typeof s == 'undefined' || s === '' || s == null)
							sel.selectedIndex = 0;
						else if (JsonDataBinding.isValueTrue(s)) {
							sel.selectedIndex = 2;
						}
						else {
							sel.selectedIndex = 1;
						}
					}
					else if (propertyDesc.editor == EDIT_ENUM || propertyDesc.editor == EDIT_MENUM) {
						var vs;
						if (typeof propertyDesc.values == 'function') {
							vs = propertyDesc.values(c.subEditor ? c.obj : c);
						}
						else {
							vs = new Array();
							for (var jk = 0; jk < propertyDesc.values.length; jk++) {
								vs.push({ text: propertyDesc.values[jk], value: propertyDesc.values[jk] });
							}
						}
						if (propertyDesc.allowEdit) {
							//requires <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"> for IE
							td.combo = JsonDataBinding.CreateComboBox(td, vs, s, 0, true);
							td.combo.select.contentEditable = false;
							td.combo.select.forProperties = true;
							td.combo.select.tdName = tdName;
							if (propertyDesc.combosetter) {
								td.combo.setter = function (ownerObj, handler) { return function (v, inputElement) { handler(ownerObj, v, inputElement); }; }(c, propertyDesc.combosetter);
							}
							var input = td.combo.getInput();
							input.oncombotxtChange = td.combo.applyTextChanges;
							input.onhandleKeydown = function () {
								_comboInput = td.combo.input;
							};
							td.combo.select.multiple = (propertyDesc.editor == EDIT_MENUM);
							if (!_propertyTopElements) _propertyTopElements = new Array();
							_propertyTopElements.push(td.combo.select);
							JsonDataBinding.AttachEvent(td.combo, "onchange", onseltextChange);
							if (!_combos) {
								_combos = new Array();
							}
							_combos.push(td.combo);
						}
						else {
							sel = document.createElement("select");
							sel.contentEditable = false;
							sel.forProperties = true;
							sel.tdName = tdName;
							sel.multiple = (propertyDesc.editor == EDIT_MENUM);
							td.appendChild(sel);
							for (var jn = 0; jn < vs.length; jn++) {
								elOptNew = document.createElement('option');
								elOptNew.text = vs[jn].text;
								elOptNew.value = vs[jn].value;
								addOptionToSelect(sel, elOptNew);
								if (JsonDataBinding.stringEQi(vs[jn].value, s)) {
									sel.selectedIndex = jn;
								}
							}
							if (propertyDesc.editor == EDIT_MENUM) {
								JsonDataBinding.AttachEvent(sel, "onchange", onselChange_menum);
							}
							else {
								JsonDataBinding.AttachEvent(sel, "onchange", onselChange);
							}
							if (vs.length > 30) {
								sel.style.overflowY = 'scroll';
							}
							else {
								sel.style.height = 'auto';
								sel.style.overflowY = '';
							}
						}
					}
					else {
						td.contentEditable = false;
						var txt2 = document.createElement("input");
						txt2.type = "text";
						if (typeof s != 'undefined' && s != null) {
							txt2.value = s;
						}
						else {
							txt2.value = '';
						}
						txt2.tdName = tdName;
						txt2.style.width = '100%';
						if (typeof (propertyDesc.allowEdit) != 'undefined' && propertyDesc.allowEdit == false) {
							txt2.readOnly=true;
						}
						if (propertyDesc.isUrl || propertyDesc.isFilePath || propertyDesc.propEditor) {
							var tbln = document.createElement('table');
							tbln.border = 0;
							tbln.cellPadding = 0;
							tbln.cellSpacing = 0;
							var tbodyn = JsonDataBinding.getTableBody(tbln);
							var trn = tbodyn.insertRow(-1);
							var tdn = document.createElement('td');
							var tdn2 = document.createElement('td');
							tdn.style.width = '100%';
							trn.appendChild(tdn);
							trn.appendChild(tdn2);
							td.appendChild(tbln);
							//
							tdn.appendChild(txt2);
							//
							cmdImg = document.createElement("img");
							if (propertyDesc.propEditor) {
								cmdImg.src = '/libjs/propedit.png';
							}
							else {
								cmdImg.src = '/libjs/folder.gif';
							}
							cmdImg.forProperties = true;
							cmdImg.contentEditable = false;
							cmdImg.style.cursor = 'pointer';
							cmdImg.style.cssFloat = 'right';
							cmdImg.subFolder = _editorOptions.serverFolder;
							cmdImg.subName = _editorOptions.userAlias;
							//cmdImg.maxSize = propertyDesc.maxSize;
							tdn2.appendChild(cmdImg);
							cmdImg.ownertextbox = txt2;
							cmdImg.propertyDesc = propertyDesc;
							if (propertyDesc.propEditor) {
								JsonDataBinding.AttachEvent(cmdImg, "onclick", propertyDesc.propEditor);
								hookTooltips(cmdImg, 'Click this button to start setting this property by clicking on a point on the web page; click this button again to stop.');
							}
							else {
								JsonDataBinding.AttachEvent(cmdImg, "onclick", selectWebFile);
							}
						}
						else {
							td.appendChild(txt2);
						}
						txt2.propCell = td;
						txt2.ontxtChange = ontxtChange;
						if (propertyDesc.allowSetText) {
							propertyDesc.settext = function (val) {
								txt2.ontxtChange = null;
								txt2.value = val;
								txt2.ontxtChange = ontxtChange;
							}
						}
						JsonDataBinding.addTextBoxObserver(txt2);
						JsonDataBinding.AttachEvent(txt2, "onchange", ontxtChange);
						if (_editorOptions.forIDE) {
							JsonDataBinding.AttachEvent(txt2, "onmousedown", function (e2) {
								e2 = e2 || document.parentWindow.event;
								if (e2 && e2.button == JsonDataBinding.mouseButtonRight()) {
									_editorOptions.propInput = txt2;
									var pos = JsonDataBinding.ElementPosition.getElementPosition(txt2);
									var txtVal = txt2.value;
									var selText = txtVal.substring(txt2.selectionStart, txt2.selectionEnd);
									try {
										if (typeof (limnorStudio) != 'undefined') limnorStudio.onPropInputMouseDown(selText, txt2.selectionStart, txt2.selectionEnd, pos.x, pos.y);
										else window.external.OnPropInputMouseDown(selText, txt2.selectionStart, txt2.selectionEnd, pos.x, pos.y);
									}
									catch (err) {
										alert(err.message);
									}
								}
							});
						}
					}
					td.curObj = c;
					td.propertyDescriptor = propertyDesc;
				} //---end of function showProperty
				for (i = 0; i < properties.length; i++) {
					if (properties[i].editor == EDIT_CMD || properties[i].editor == EDIT_DEL || properties[i].editor == EDIT_CMD2 || properties[i].editor == EDIT_CMD3 || properties[i].editor == EDIT_GO) {
						continue;
					}
					if (!_editorOptions.isEditingBody) {
						if (properties[i].name == 'class' || properties[i].name == 'styleName') {
							continue;
						}
					}
					showProperty(_propsBody, properties[i]);
				}
			}
			else { //does not define properties
				var names = new Array();
				for (var n in c) {
					try {
						if (n.substr(0, 2) != 'on' && typeof c[n] != 'function') {
							names.push(n);
						}
					}
					catch (err) {
					}
				}
				var names2 = names.sort();
				for (i = 0; i < names2.length; i++) {
					var nm = names2[i];
					var tr = _propsBody.insertRow(-1);
					var td = document.createElement("td");
					tr.appendChild(td);
					td.innerHTML = nm;
					td = document.createElement("td");
					tr.appendChild(td);
					try {
						td.innerHTML = c[nm];
					}
					catch (err) {
						td.innerHTML = "<font color=red>" + err + "</font>";
					}
				}
			}
			_showResizer();
			if (_editorOptions.forIDE) {
				var se = _getSelectedObject(c);
				if (se) {
					try {
						if (typeof (limnorStudio) != 'undefined') limnorStudio.onElementSelected(se);
						else window.external.OnElementSelected(se);
					}
					catch (err) {
						alert('show properties:'+err.message);
					}
				}
			}
			showEditorButtons(c);
			adjustSizes();
		}
		function _setMessage(msg) {
			if (_spMsg) {
				if (msg)
					_spMsg.innerHTML = msg;
				else
					_spMsg.innerHTML = '***';
			}
		}
		function showEditorButtons(obj) {
			var show = false;
			if (_editorOptions) {
				if (_editorOptions.selectedHtml)
					show = true;
				else if (!isNonBodyNode(obj)) {
					show = true;
				}
			}
			else {
				_divElementToolbar.innerHTML = '';
			}
			if (!show) {
				_fontCommands.style.display = 'none';
				_imgLocatorElement.style.display = 'none'
				imgNewElement.style.display = 'none';
				return false;
			}
			else {
				_fontCommands.style.display = 'inline';
				_imgLocatorElement.style.display = 'inline';
				imgNewElement.style.display = 'inline';
				return true;
			}
		}
		function docClick(e) {
			if (_divError) {
				if (_divError.clicks) {
					_divError.clicks = _divError.clicks - 1;
				}
				else {
					_divError.style.display = 'none';
				}
			}
			if (_divToolTips) {
				if (_divToolTips.clicks) {
					_divToolTips.clicks = _divToolTips.clicks - 1;
				}
				else {
					_divToolTips.style.display = 'none';
				}
			}
			var sender = JsonDataBinding.getSender(e);
			if (_editorOptions && _editorOptions.forIDE) {
				if (jscolor && jscolor.picker && jscolor.picker.boxB) {
					while (sender && sender != _editorOptions.elementEditedDoc.body) {
						if (sender == jscolor.picker.boxB) {
							return true;
						}
						if (sender.forProperties) {
							return true;
						}
						sender = sender.parentNode;
					}
					if (jscolor.picker.boxB.parentNode == _editorOptions.elementEditedDoc.body) {
						_editorOptions.elementEditedDoc.body.removeChild(jscolor.picker.boxB);
					}
					jscolor.picker = null;
				}
			}
			if (_elementLocator) {
				//var c = JsonDataBinding.getSender(e);
				if (!isInLocator(sender)) {
					_elementLocator.style.display = 'none';
				}
			}
			if (_textInput) {
				if (!isInTextInputor(sender)) {
					_textInput.style.display = 'none';
				}
			}
			//if (_isIE && !_editorOptions.isEditingBody) {
			//	if (_editorOptions.iframeFocus) {
			//		if (sender == _headingSelect) {
			//			var s = _editorOptions.selectedHtml;
			//			_inputX.focus();
			//			_editorOptions.iframeFocus = false;
			//			_editorOptions.selectedHtml = s;
			//		}
			//		else if (sender == _fontSelect) {
			//			var s = _editorOptions.selectedHtml;
			//			_inputX.focus();
			//			_editorOptions.iframeFocus = false;
			//			_editorOptions.selectedHtml = s;
			//			_editorOptions.ifrmSelHtml = s;
			//			//_fontSelect.size = 5;//_fontSelect.options.length;
			//		}
			//	}
			//}
			return true;
		}
		function isLastNode(node) {
			var obj = node;
			while (obj && obj != _editorOptions.elementEditedDoc.body) {
				if (typeof (obj.nextSibling) != 'undefined' && obj.nextSibling != null && typeof (obj.nextSibling.tagName) != 'undefined') {
					return false;
				}
				obj = obj.parentNode;
			}
			return true;
		}
		function closeColorPicker() {
			if (jscolor && jscolor.picker && jscolor.picker.boxB) {
				if (jscolor.picker.boxB.parentNode == document.body) {
					document.body.removeChild(jscolor.picker.boxB);
				}
				jscolor.picker = null;
			}
		}
		function getValue(o, name) {
			var x = parseInt(o.getAttribute(name));
			if (isNaN(x)) {
				x = 1;
			}
			return x;
		}
		function createSvgCircle(o) {
			var isEllipse = o.tagName.toLowerCase() == 'ellipse';
			var propx, propy;
			function _showmarkers() {
			}
			function _getProperties() {
				var props = [];
				propx = { name: 'cx', editor: EDIT_NUM, byAttribute: true, allowSetText: true };
				props.push(propx);
				propy = { name: 'cy', editor: EDIT_NUM, byAttribute: true, allowSetText: true };
				props.push(propy);
				if (isEllipse) {
					props.push({ name: 'rx', editor: EDIT_NUM, byAttribute: true });
					props.push({ name: 'ry', editor: EDIT_NUM, byAttribute: true });
				}
				else {
					props.push({ name: 'r', editor: EDIT_NUM, byAttribute: true });
				}
				return props;
			}
			function _moveleft() {
				var x = getValue(o, 'cx') - HtmlEditor.svgshapemovegap;
				o.setAttribute('cx', x);
				if (propx && propx.settext) {
					propx.settext(x);
				}
			}
			function _moveright() {
				var x = getValue(o, 'cx') + HtmlEditor.svgshapemovegap;
				o.setAttribute('cx', x);
				if (propx && propx.settext) {
					propx.settext(x);
				}
			}
			function _moveup() {
				var x = getValue(o, 'cy') - HtmlEditor.svgshapemovegap;
				o.setAttribute('cy', x);
				if (propy && propy.settext) {
					propy.settext(x);
				}
			}
			function _movedown() {
				var x = getValue(o, 'cy') + HtmlEditor.svgshapemovegap;
				o.setAttribute('cy', x);
				if (propy && propy.settext) {
					propy.settext(x);
				}
			}
			_getProperties();
			return {
				showmarkers: function () {
					_showmarkers();
				},
				getProperties: function () {
					return _getProperties();
				},
				moveleft: function () {
					_moveleft();
				},
				moveright: function () {
					_moveright();
				},
				moveup: function () {
					_moveup();
				},
				movedown: function () {
					_movedown();
				}
			};
		}
		function createSvgText(o) {
			var propx, propy;
			function _showmarkers() {
			}
			function _getProperties() {
				var props = [];
				propx = { name: 'x', editor: EDIT_NUM, byAttribute: true, allowSetText: true };
				props.push(propx);
				propy = { name: 'y', editor: EDIT_NUM, byAttribute: true, allowSetText: true };
				props.push(propy);
				props.push({
					name: 'text', editor: EDIT_TEXT,
					getter: function (o) {
						return o.textContent;
					},
					setter: function (o, v) {
						o.textContent = v;
					}
				});
				props.push(propFontFamily);
				props.push(propFontSize);
				props.push(propFontStyle);
				props.push(propFontWeight);
				props.push(propFontVariant);
				return props;
			}
			function _moveleft() {
				var x = getValue(o, 'x') - HtmlEditor.svgshapemovegap;
				o.setAttribute('x', x);
				if (propx && propx.settext) {
					propx.settext(x);
				}
			}
			function _moveright() {
				var x = getValue(o, 'x') + HtmlEditor.svgshapemovegap;
				o.setAttribute('x', x);
				if (propx && propx.settext) {
					propx.settext(x);
				}
			}
			function _moveup() {
				var x = getValue(o, 'y') - HtmlEditor.svgshapemovegap;
				o.setAttribute('y', x);
				if (propy && propy.settext) {
					propy.settext(x);
				}
			}
			function _movedown() {
				var x = getValue(o, 'y') + HtmlEditor.svgshapemovegap;
				o.setAttribute('y', x);
				if (propy && propy.settext) {
					propy.settext(x);
				}
			}
			_getProperties();
			return {
				showmarkers: function () {
					_showmarkers();
				},
				getProperties: function () {
					return _getProperties();
				},
				moveleft: function () {
					_moveleft();
				},
				moveright: function () {
					_moveright();
				},
				moveup: function () {
					_moveup();
				},
				movedown: function () {
					_movedown();
				}
			};
		}
		function createSvgRect(o) {
			var propx, propy;
			function _showmarkers() {
			}
			function _getProperties() {
				var props = [];
				propx = { name: 'x', editor: EDIT_NUM, byAttribute: true, allowSetText: true };
				propy = { name: 'y', editor: EDIT_NUM, byAttribute: true, allowSetText: true };
				props.push(propx);
				props.push(propy);
				props.push({ name: 'width', editor: EDIT_NUM, byAttribute: true });
				props.push({ name: 'height', editor: EDIT_NUM, byAttribute: true });
				return props;
			}
			
			function _moveleft() {
				var x = getValue(o, 'x') - HtmlEditor.svgshapemovegap;
				o.setAttribute('x', x);
				if (propx && propx.settext) {
					propx.settext(x);
				}
			}
			function _moveright() {
				var x = getValue(o, 'x') + HtmlEditor.svgshapemovegap;
				o.setAttribute('x', x);
				if (propx && propx.settext) {
					propx.settext(x);
				}
			}
			function _moveup() {
				var x = getValue(o, 'y') - HtmlEditor.svgshapemovegap;
				o.setAttribute('y', x);
				if (propy && propy.settext) {
					propy.settext(x);
				}
			}
			function _movedown() {
				var x = getValue(o, 'y') + HtmlEditor.svgshapemovegap;
				o.setAttribute('y', x);
				if (propy && propy.settext) {
					propy.settext(x);
				}
			}
			return {
				showmarkers: function () {
					_showmarkers();
				},
				getProperties: function () {
					return _getProperties();
				},
				moveleft: function () {
					_moveleft();
				},
				moveright: function () {
					_moveright();
				},
				moveup: function () {
					_moveup();
				},
				movedown: function () {
					_movedown();
				}
			};
		}
		function createSvgLine(o) {
			var prop1,prop2;
			var point1, point2;
			function parseCoords() {
				point1 = {};
				point1.x = getValue(o, 'x1');
				point1.y = getValue(o, 'y1');
				point2 = {};
				point2.x = getValue(o, 'x2');
				point2.y = getValue(o, 'y2');
			}
			function _showmarkers() {
			}
			function applypoints() {
				if (point1) {
					o.setAttribute('x1', point1.x);
					o.setAttribute('y1', point1.y);
				}
				if (point2) {
					o.setAttribute('x2', point2.x);
					o.setAttribute('y2', point2.y);
				}
				if (o.jsData.showmarkers)
					o.jsData.showmarkers();
			}
			function onmove() {
				if (point1 && prop1 && prop1.settext) {
					prop1.settext(point1.x + ',' + point1.y);
				}
				if (point2 && prop2 && prop2.settext) {
					prop2.settext(point2.x + ',' + point2.y);
				}
			}
			function _getProperties() {
				parseCoords();
				props = [];
				prop1 = {
					name: 'point1', editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'one end point of the line', allowSetText: true,
					getter: function (o) {
						return point1.x + ',' + point1.y;
					},
					setter: function (o, v) {
						if (v && v.length > 0) {
							var ss = v.split(',');
							if (ss.length > 0) {
								point1.x = parseInt(ss[0]);
								o.setAttribute('x1', point1.x);
								if (ss.length > 1) {
									point1.y = parseInt(ss[1]);
									o.setAttribute('y1', point1.y);
								}
							}
						}
					}
				};
				props.push(prop1);
				prop2 = {
					name: 'point2', editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'one end point of the line', allowSetText: true,
					getter: function (o) {
						return point2.x + ',' + point2.y;
					},
					setter: function (o, v) {
						if (v && v.length > 0) {
							var ss = v.split(',');
							if (ss.length > 0) {
								point2.x = parseInt(ss[0]);
								o.setAttribute('x2', point2.x);
								if (ss.length > 1) {
									point2.y = parseInt(ss[1]);
									o.setAttribute('y2', point2.y);
								}
							}
						}
					}
				};
				props.push(prop2);
				function onsetprop(curObj, v) {
					_showmarkers();
				}
				function onclientmousedown(e) {
					if (_custMouseDownOwner && _custMouseDownOwner.ownertextbox) {
						e = e || window.event;
						if (e) {
							var x = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x);
							var y = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y);
							var svg = o.parentNode.parentNode;
							if (svg && svg.getBoundingClientRect) {
								var c = svg.getBoundingClientRect();
								x = x - c.left;
								y = y - c.top;
							}
							_custMouseDownOwner.ownertextbox.value = x + ',' + y;
						}
					}
				}
				function oneditprop(e) {
					var img = JsonDataBinding.getSender(e);
					if (img) {
						if (_custMouseDown && _custMouseDownOwner != img) {
							if (_custMouseDownOwner) {
								_custMouseDownOwner.src = 'libjs/propedit.png';
								img.active = false;
							}
						}
						img.active = !img.active;
						if (img.active) {
							img.src = 'libjs/propeditAct.png';
							_custMouseDown = onclientmousedown;
							_custMouseDownOwner = img;
						}
						else {
							img.src = 'libjs/propedit.png';
							_custMouseDown = null;
							_custMouseDownOwner = null;
						}
					}
				}
				for (var i = 0; i < props.length; i++) {
					if (!props[i].onsetprop) {
						props[i].onsetprop = onsetprop;
					}
					props[i].propEditor = oneditprop;
				}
				return props;
			}
			function _moveleft() {
				parseCoords();
				if (point1 && point2) {
					point1.x = point1.x - HtmlEditor.svgshapemovegap;
					point2.x = point2.x - HtmlEditor.svgshapemovegap;
					applypoints();
					onmove();
				}
			}
			function _moveright() {
				parseCoords();
				if (point1 && point2) {
					point1.x = point1.x + HtmlEditor.svgshapemovegap;
					point2.x = point2.x + HtmlEditor.svgshapemovegap;
					applypoints();
					onmove();
				}
			}
			function _moveup() {
				parseCoords();
				if (point1 && point2) {
					point1.y = point1.y - HtmlEditor.svgshapemovegap;
					point2.y = point2.y - HtmlEditor.svgshapemovegap;
					applypoints();
					onmove();
				}
			}
			function _movedown() {
				parseCoords();
				if (point1 && point2) {
					point1.y = point1.y + HtmlEditor.svgshapemovegap;
					point2.y = point2.y + HtmlEditor.svgshapemovegap;
					applypoints();
					onmove();
				}
			}
			parseCoords();
			return {
				showmarkers: function () {
					_showmarkers();
				},
				getProperties: function () {
					return _getProperties();
				},
				moveleft: function () {
					_moveleft();
				},
				moveright: function () {
					_moveright();
				},
				moveup: function () {
					_moveup();
				},
				movedown: function () {
					_movedown();
				}
			};
		}
		function createSvgPoly(o) {
			var props = [];
			var points = [];
			var coords;
			function parseCoords() {
				coords = o.getAttribute('points');
				if (coords) {
					var nums = coords.split(' ');
					points = [];
					for (var i = 0; i < nums.length; i++) {
						if (nums[i]) {
							var s = nums[i].trim();
							if (s.length > 0) {
								var pstr = s.split(',');
								var pt = {};
								pt.x = parseInt(pstr[0]);
								if (pstr.length > 1) {
									pt.y = parseInt(pstr[1]);
								}
								else
									pt.y = 0;
								pt.idx = points.push(pt) - 1;
							}
						}
					}
				}
			}
			function _showmarkers() {
			}
			function applypoints() {
				if (points) {
					var ss = '';
					for (var k = 0; k < points.length; k++) {
						if (!points[k].deleted) {
							if (ss.length > 0) {
								ss += ' ';
							}
							ss += (points[k].x + ',' + points[k].y);
						}
					}
					o.setAttribute('points', ss);
					if (o.jsData.showmarkers)
						o.jsData.showmarkers();
				}
			}
			function onmove() {
				if (points && props) {
					for (var k = 0; k < points.length; k++) {
						if (k < props.length) {
							if (!props[k].deleted) {
								if (props[k].settext) {
									props[k].settext(points[k].x + ',' + points[k].y);
								}
							}
						}
						else
							break;
					}
				}
			}
			function _getProperties() {
				parseCoords();
				props = [];
				function getterPoint(idx, obj) {
					return function (owner) {
						if (idx >= 0 && idx < points.length) {
							return points[idx].x + ',' + points[idx].y;
						}
					}
				}
				function setterPoint(idx, obj) {
					return function (owner, val) {
						var ss;
						if (val && val.length > 0) {
							ss = val.split(',');
							if (ss.length > 0) {
								var p;
								if (idx >= 0 && idx < points.length) {
									p = points[idx];
								}
								else {
									p = {};
									p.idx = idx;
									points.push(p);
								}
								p.x = parseInt(ss[0]);
								if (ss.length > 1) {
									p.y = parseInt(ss[1]);
								}
							}
						}
						else {
							if (idx >= 0 && idx < points.length) {
								p = points[idx];
								p.deleted = true;
							}
						}
						applypoints();
					}
				}
				function createPointProp(idx, needGrow) {
					var p = {
						name: 'point ' + idx, editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'a vertex point of a poly shape, in format of x,y', allowSetText: true,
						getter: getterPoint(idx, o),
						setter: setterPoint(idx, o)
					};
					if (needGrow) {
						p.needGrow = true;
						p.onsetprop = function (o, v) {
							_showmarkers();
							if (this.grow) {
								//do not grow if empty
								if (v && v.length > 0) {
									this.grow(createPointProp(idx + 1, true), 1);
									this.grow = null;
								}
							}
						};
					}
					return p;
				}
				if (points) {
					for (var i = 0; i < points.length; i++) {
						props.push(createPointProp(i, false));
					}
					props.push(createPointProp(points.length, true));
				}
				else {
					props.push(createPointProp(0, true));
				}
				function onsetprop(curObj, v) {
					_showmarkers();
				}
				function onclientmousedown(e) {
					if (_custMouseDownOwner && _custMouseDownOwner.ownertextbox) {
						e = e || window.event;
						if (e) {
							var x = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x);
							var y = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y);
							var svg = o.parentNode.parentNode;
							if (svg && svg.getBoundingClientRect) {
								var c = svg.getBoundingClientRect();
								x = x - c.left;
								y = y - c.top;
							}
							_custMouseDownOwner.ownertextbox.value = x + ',' + y;
						}
					}
				}
				function oneditprop(e) {
					var img = JsonDataBinding.getSender(e);
					if (img) {
						if (_custMouseDown && _custMouseDownOwner != img) {
							if (_custMouseDownOwner) {
								_custMouseDownOwner.src = 'libjs/propedit.png';
								img.active = false;
							}
						}
						img.active = !img.active;
						if (img.active) {
							img.src = 'libjs/propeditAct.png';
							_custMouseDown = onclientmousedown;
							_custMouseDownOwner = img;
						}
						else {
							img.src = 'libjs/propedit.png';
							_custMouseDown = null;
							_custMouseDownOwner = null;
						}
					}
				}
				for (var i = 0; i < props.length; i++) {
					if (!props[i].onsetprop) {
						props[i].onsetprop = onsetprop;
					}
					props[i].propEditor = oneditprop;
				}
				return props;
			}
			function _moveleft() {
				parseCoords();
				if (points) {
					for (var i = 0; i < points.length; i++) {
						points[i].x = points[i].x - HtmlEditor.svgshapemovegap;
					}
					applypoints();
					onmove();
				}
			}
			function _moveright() {
				parseCoords();
				if (points) {
					for (var i = 0; i < points.length; i++) {
						points[i].x = points[i].x + HtmlEditor.svgshapemovegap;
					}
					applypoints();
					onmove();
				}
			}
			function _moveup() {
				parseCoords();
				if (points) {
					for (var i = 0; i < points.length; i++) {
						points[i].y = points[i].y - HtmlEditor.svgshapemovegap;
					}
					applypoints();
					onmove();
				}
			}
			function _movedown() {
				parseCoords();
				if (points) {
					for (var i = 0; i < points.length; i++) {
						points[i].y = points[i].y + HtmlEditor.svgshapemovegap;
					}
					applypoints();
					onmove();
				}
			}
			parseCoords();
			return {
				showmarkers: function () {
					_showmarkers();
				},
				getProperties: function () {
					return _getProperties();
				},
				moveleft: function () {
					_moveleft();
				},
				moveright: function () {
					_moveright();
				},
				moveup: function () {
					_moveup();
				},
				movedown: function () {
					_movedown();
				}
			};
		}
		function createSvgShape(o) {
			if (o) {
				var tag = o.tagName ? o.tagName.toLowerCase() : '';
				o.setAttribute('styleshare', 'NotShare');
				if (tag == 'polygon') {
					return createSvgPoly(o);
				}
				else if (tag == 'rect') {
					return createSvgRect(o);
				}
				else if (tag == 'circle') {
					return createSvgCircle(o);
				}
				else if (tag == 'ellipse') {
					return createSvgCircle(o);
				}
				else if (tag == 'line') {
					return createSvgLine(o);
				}
				else if (tag == 'polyline') {
					return createSvgPoly(o);
				}
				else if (tag == 'text') {
					return createSvgText(o);
				}
			}
		}
		//it will not re-select _editorOptions.selectedObject
		function selectEditElement(c) {
			if(c.limnorDialog) return;
			if (isMarker(c))
				return;
			if (c && c.tagName && c.tagName.toLowerCase() == 'html') {
				if (_parentList && _parentList.options && _parentList.options.length > 0) {
					_parentList.selectedIndex = _parentList.options.length - 1;
					showProperties(_parentList.options[_parentList.options.length - 1].objvalue);
				}
				return c;
			}
			if (_elementLocator) {
				if (!isInLocator(c)) {
					_elementLocator.style.display = 'none';
				}
			}
			if (_textInput) {
				if (!isInTextInputor(c)) {
					_textInput.style.display = 'none';
				}
			}
			hideSelector();
			if (_editorOptions.isEditingBody) {
				if (!_editorOptions.elementEditedDoc.body.isContentEditable) {
					_editorOptions.elementEditedDoc.body.contentEditable = true;
				}
			}
			var obj;
			var curTag;
			if (c) {
				if (c.tagName) {
					obj = c;
					curTag = c.tagName.toLowerCase();
				}
			}
			if (_editorOptions.isEditingBody && !_editorOptions.client.getInitialized.apply(_editorOptions.editorWindow)) {
				obj = _editorOptions.elementEditedDoc.body;
			}
			var op = obj;
			while (op) {
				if (op.jsData) {
					if (op.jsData.createSubEditor) {
						var obj0 = op.jsData.createSubEditor(op, c);
						if (obj0) {
							obj = obj0;
							c = obj0.obj ? obj0.obj : obj0;
							break;
						}
					}
					else if (op.jsData.designer && op.jsData.designer.createSubEditor) {
						var obj0 = op.jsData.designer.createSubEditor(op, c);
						if (obj0) {
							obj = obj0;
							break;
						}
					}
				}
				else {
					if (op.getAttribute && op.getAttribute('hidechild')) {
						obj = op;
						c = op;
						break;
					}
					else {
						if (op.getAttribute) {
							var tpn = op.getAttribute('typename');
							if (typeof (tpn) != 'undefined' && tpn != null && tpn.length > 0) {
								if (_editorOptions.client.initPlugin.apply(_editorOptions.elementEditedWindow,[tpn,op])) {
									obj = op;
									c = op;
									break;
								}
							}
						}
					}
				}
				op = op.parentNode;
			}
			if (obj) {
				if (obj != _editorOptions.selectedObject) {
					_editorOptions.selectedObject = obj;
					if (obj.subEditor && obj.obj == c) {
						showSelectionMark(obj);
					}
					else {
						showSelectionMark(c);
					}
					showProperties(obj);
				}
			}
			else {
				_editorOptions.selectedObject = null;
				_tdTitle.innerHTML = 'Properties - none';
				while (_propsBody.rows.length > 0) {
					_propsBody.deleteRow(_propsBody.rows.length - 1);
				}
			}
			return obj;
		}
		function appendToBody(e) {
			document.body.appendChild(e);
			e.contentEditable = false;
			e.forProperties = true;
			_topElements.push(e);
		}
		function adjustObjectSize(h) {
			if (_tdObj.lastobj) {
				for (var i = 0; i < _tdObj.children.length; i++) {
					if (_tdObj.lastobj == _tdObj.children[i]) {
						if (h > _tdObj.lastobj.offsetTop) {
							_tdObj.lastobj.style.height = (h - _tdObj.offsetTop - _tdObj.lastobj.offsetTop) + 'px';
						}
						return;
					}
				}
			}
		}
		function mouseup(e) {
			//distribute it to all pages in editing
			//
			_divProp.isMousedown = false;
			if (_resizer) {
				_resizer.isMousedown = false;
			}
			if (_nameresizer) {
				_nameresizer.isMousedown = false;
			}
			//if (_editorOptions && _editorOptions.inline && e) {
			//	captureSelection(e);
			//}
			_showResizer();
			adjustSizes();
			return true;
		}
		function mousemove(e) {
			var diffx, diffy, w, p1, i;
			if (_divProp.isMousedown) {
				e = e || window.event;
				if (e) {
					diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _divProp.ox;
					diffy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - _divProp.oy;
					_divProp.style.left = diffx > 0 ? diffx + 'px' : "0px";
					_divProp.style.top = diffy > 0 ? diffy + 'px' : "0px";
				}
			}
			else if (_resizer && _resizer.isMousedown) {
				e = e || window.event;
				if (e) {
					diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _resizer.ox;
					diffy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - _resizer.oy;
					_resizer.style.left = diffx > 0 ? diffx + 'px' : "0px";
					_resizer.style.top = diffy > 0 ? diffy + 'px' : "0px";
					p1 = JsonDataBinding.ElementPosition.getElementPosition(_divProp);
					var p2 = JsonDataBinding.ElementPosition.getElementPosition(_resizer);
					w = p2.x - p1.x + _resizer.offsetWidth;
					var h = p2.y - p1.y + _resizer.offsetHeight;
					if (w > 120) {
						_divProp.style.width = w + 'px';
					}
					if (h > 60) {
						_divProp.style.height = h + 'px';
					}
				}
			}
			else if (_nameresizer && _nameresizer.isMousedown) {
				e = e || window.event;
				if (e) {
					diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _nameresizer.ox;
					p1 = JsonDataBinding.ElementPosition.getElementPosition(_divProp);
					if (diffx > p1.x + 30 && diffx < p1.x + _divProp.offsetWidth - 30) {
						_nameresizer.style.left = diffx + 'px';
						w = diffx - p1.x;
						for (i = 0; i < _propsBody.rows.length; i++) {
							if (_propsBody.rows[i].cells[0].colSpan != 2) {
								_propsBody.rows[i].cells[0].style.width = w + 'px';
							}
						}
					}
				}
			}
			else {
				if (_editorOptions) {
					if (_editorOptions.inline) {
						if (_editorOptions.styleChanged) {
							_editorOptions.styleChanged = false;
							if (_editorOptions.selectedObject) {
								var cssTD = getPropertyCell('cssText');
								if (cssTD) {
									var s = _editorOptions.selectedObject.style.cssText;
									if (s) {
										var txts = cssTD.getElementsByTagName('input');
										if (txts && txts.length > 0) {
											for (i = 0; i < txts.length; i++) {
												if (txts[i].type == 'text') {
													cssTD.value = s;
													txts[i].value = s;
													break;
												}
											}
										}
									}
								}
							}
						}
						//if (_isIE && _editorOptions.isEditingBody) {
						//	var obj = JsonDataBinding.getSender(e);
						//	if (isEditorObject(obj)) {
						//		_editorOptions.elementEditedDoc.body.contentEditable = false;
						//	}
						//	else {
						//		_editorOptions.elementEditedDoc.body.contentEditable = true;
						//	}
						//}
					}
					//else {
					e = e || window.event;
					if (e) {
						var obj = JsonDataBinding.getSender(e);
						if (obj) {
							var td;
							if (obj.tdName)
								td = obj.tdName;
							else if (obj.propDesc)
								td = obj;
							if (td && !td.isCommandBar) {
								if (td.propDesc && td.propDesc.editor == EDIT_PROPS) {
								}
								else {
									if (_tdSelectedProperty) {
										var tr = _tdSelectedProperty.parentNode;
										if (tr && tr.cat) {
											_tdSelectedProperty.style.backgroundColor = getPropCatColor(tr.cat);
										}
										else {
											_tdSelectedProperty.style.backgroundColor = 'white';
										}
									}
									_tdSelectedProperty = td;
									_tdSelectedProperty.style.backgroundColor = 'yellow'; // '#ADD8E6';
								}
								_setMessage(td.propDesc.desc ? td.propDesc.desc : (td.propDesc.toolTips ? td.propDesc.toolTips : td.propDesc.name));
							}
						}
					}
					//}
				}
			}
			return true;
		}
		function hookTooltips(obj, tips) {
			obj.toolTips = tips;
			obj.moves = 0;
			JsonDataBinding.AttachEvent(obj, "onmousemove", tooltipsOwnerMouseMove);
			JsonDataBinding.AttachEvent(obj, "onmouseout", tooltipsOwnerMouseOut);
		}
		function onPropGridScroll() {
			if (_combos) {
				for (var i = 0; i < _combos.length; i++) {
					_combos[i].adjustPosition();
				}
			}
		}
		function resetEditing() {
			if (_editorOptions && _editorOptions.client && _editorOptions.isEditingBody) {
				if (confirm('Waning. Do you want to remove all modifications made since last publish of the web page, including previously saved modifications?')) {
					_editorOptions.reset.apply(_editorOptions.elementEditedWindow);
				}
			}
		}
		function toggleShowHideProps() {
			var tr = _editorBody.rows[0];
			if (tr.cells[0].style.display == 'none') {
				tr.cells[0].style.display = '';
				tr.cells[1].style.display = '';
				tr.cells[2].style.width = '60%';
			}
			else {
				tr.cells[0].style.display = 'none';
				tr.cells[1].style.display = 'none';
				tr.cells[2].style.width = '100%';
			}
		}
		function handleSaveCss() {
			if (_editorOptions && _editorOptions.propertiesOwner) {
				if (!_editorOptions.propertiesOwner.style.cssText || _editorOptions.propertiesOwner.style.cssText == '') {
					alert('The element does not have styles set');
					return;
				}
				var elementTag = _editorOptions.propertiesOwner.tagName;
				var elementDesc = _getElementDesc(elementTag);
				var csstext = '{' + _editorOptions.propertiesOwner.style.cssText + '}';
				var cssFilepath = JsonDataBinding.urlToFilename(getWebAddress());
				var pos = cssFilepath.lastIndexOf('.');
				if (pos > 0) {
					cssFilepath = cssFilepath.substr(0, pos) + '.css';
				}
				JsonDataBinding.setupChildManager();
				document.childManager.onChildWindowReady = function() {
					JsonDataBinding.executeByPageId(2757476158, 'setData', elementDesc, csstext, elementTag, cssFilepath, JsonDataBinding.Debug);
				}
				JsonDataBinding.AbortEvent = false;
				var v3804300e = {};
				v3804300e.isDialog = true;
				v3804300e.url = 'WebPageSaveStyle.html';
				v3804300e.center = true;
				v3804300e.top = 0;
				v3804300e.left = 0;
				v3804300e.width = 680;
				v3804300e.height = 420;
				v3804300e.resizable = true;
				v3804300e.border = '2px double #008000';
				v3804300e.ieBorderOffset = 4;
				v3804300e.title = 'Save Styles';
				v3804300e.hideCloseButtons = true;
				v3804300e.pageId = 2757476158;
				v3804300e.editorOptions = _editorOptions;
				v3804300e.onDialogClose = function() {
					if (JsonDataBinding.isValueTrue((document.dialogResult) != ('ok'))) {
						JsonDataBinding.AbortEvent = true;
						return;

					}
					var cssFile = JsonDataBinding.getPropertyByPageId(2757476158, 'propFilename');
					if (cssFile && cssFile.length > 0) {
						var clname = JsonDataBinding.getPropertyByPageId(2757476158, 'propClassname');
						var selector = elementTag + ((clname && clname.length > 0) ? '.' + clname : '');
						var csstext = JsonDataBinding.getPropertyByPageId(2757476158, 'propCssText');
						v3804300e.editorOptions.client.addCssFile.apply(v3804300e.editorOptions.elementEditedWindow, [cssFile, selector, selector + csstext]);
					}
				}
				JsonDataBinding.showChild(v3804300e);
			}
		}
		function savePageCache() {
			handlePageCache(true);
		}
		function handlePageCache(manualSave) {
			if (_editorOptions && _editorOptions.wholePage) {
				if (!_editorOptions.forIDE) {
					finishEdit({ toCache: true, manualInvoke: manualSave });
				}
			}
		}
		//initialize the editor
		function _init() {
			//
			_topElements = new Array();
			//
			_addEditors(getDefaultEditors());
			_addAddableElementList(getDefaultAddables());
			_addCommandList(getDefaultCommands());
			//
			_divSelectElement = document.createElement("div");
			appendToBody(_divSelectElement);
			_divSelectElement.style.display = 'none';
			_divSelectElement.jsData = createElementSelector();
			//
			_divLargeEnum = document.createElement("div");
			appendToBody(_divSelectElement);
			_divLargeEnum.style.display = 'none';
			_divLargeEnum.jsData = createLargeEnum();
			//create main UI
			_divProp = document.createElement("div");
			_divProp.style.cssText = "overflow:hidden;background-color:#E0F8E6; z-index:100;border-top:2px solid blue;border-right:2px solid blue;border-bottom:2px solid blue;border-left:2px solid blue;";
			_divProp.scroll = 'no';
			_divProp.style.overflowX = 'hidden';
			_divProp.style.overflowY = 'hidden';
			_divProp.style.boxShadow = '10px 10px 5px #848484';
			_divProp.style.borderRadius = '10px';
			//
			_divProp.expanded = true;
			_divProp.fullHeight = _divProp.style.height;
			_divProp.ox = 0;
			_divProp.oy = 0;
			_divProp.ondragstart = function() { return false; };
			if (containerDiv) {
				_divProp.style.position = 'static';
				_divProp.style.left = '0px';
				_divProp.style.right = '0px';
				_divProp.style.width = '100%';
				_divProp.style.height = '100%';
				containerDiv.appendChild(_divProp);
			}
			else {
				_divProp.style.position = 'absolute';
				_divProp.style.left = '280px';
				_divProp.style.top = '380px';
				_divProp.style.width = '600px';
				_divProp.style.height = '300px';
				appendToBody(_divProp);
			}
			_divProp.style.display = 'block';
			//create resizer
			_resizer = document.createElement("img");
			_resizer.style.position = 'absolute';
			_resizer.style.zIndex = 10;
			_resizer.style.display = 'none';
			_resizer.style.cursor = 'nw-resize';
			_resizer.src = '/libjs/resizer.gif';
			_resizer.ondragstart = function() { return false; };
			if (JsonDataBinding.IsIE()) {
				_resizer.onresizestart = function() { return false; };
				_resizer.setAttribute("unselectable", "on");
			}
			appendToBody(_resizer);
			if (containerDiv) {
				_resizer.style.display = 'none';
			}
			//create property grid column resizer (not used)
			_nameresizer = document.createElement("img");
			_nameresizer.style.position = 'absolute';
			_nameresizer.style.zIndex = 10;
			_nameresizer.style.display = 'none';
			_nameresizer.style.cursor = 'e-resize';
			_nameresizer.style.width = '3px';
			_nameresizer.src = '/libjs/resizeV.png';
			_nameresizer.ondragstart = function() { return false; };
			if (_isIE) {
				_nameresizer.onresizestart = function() { return false; };
				_nameresizer.setAttribute("unselectable", "on");
			}
			appendToBody(_nameresizer);
			_nameresizer.style.cursor = 'e-resize';
			//table for title bar ====================================
			_titleTable = document.createElement("table");
			_divProp.appendChild(_titleTable);
			_titleTable.contentEditable = false;
			_titleTable.forProperties = true;
			_titleTable.style.backgroundColor = 'white';
			_titleTable.border = 0;
			if (containerDiv) {
				_titleTable.style.display = 'none';
			}
			//create title elements
			var _titleBody = JsonDataBinding.getTableBody(_titleTable);
			var tr = _titleBody.insertRow(-1);
			tr.style.cssText = "background-color:#A0CFEC;cursor='move';"
			//title text
			var td = document.createElement('td');
			td.contentEditable = false;
			td.forProperties = true;
			tr.appendChild(td);
			td.style.cssText = "width:100%; cursor:move;";
			td.innerHTML = HtmlEditor.version;
			_tdTitle = td;
			//expand button
			td = document.createElement('td');
			td.contentEditable = false;
			td.forProperties = true;
			tr.appendChild(td);
			_imgExpand = document.createElement("img");
			_imgExpand.src = "/libjs/expand_min.png";
			_imgExpand.style.cursor = "pointer";
			_imgExpand.contentEditable = false;
			_imgExpand.forProperties = true;
			hookTooltips(_imgExpand, 'Minimize or restore the editor window');
			td.appendChild(_imgExpand);
			//===end of title bar========================================
			//tool bar===========================================================
			_toolbarTable = document.createElement('table');
			_toolbarTable.cellPadding = 0;
			_toolbarTable.cellSpacing = 0;
			_toolbarTable.border = 0;
			_divProp.appendChild(_toolbarTable);
			//
			tr = _toolbarTable.insertRow(-1);
			_toolbar = tr;
			tr.forProperties = true;
			tr.contentEditable = false;
			td = document.createElement('td');
			td.contentEditable = false;
			td.forProperties = true;
			td.colSpan = 2; //next row has two columns: parent list and property grid
			tr.appendChild(td);
			//===add tool bar elements ==========================
			_imgOK = document.createElement('img');
			_imgOK.src = '/libjs/ok.png';
			_imgOK.border = 0;
			_imgOK.style.cursor = 'pointer';
			td.appendChild(_imgOK);
			_imgOK.contentEditable = false;
			_imgOK.forProperties = true;
			JsonDataBinding.AttachEvent(_imgOK, "onclick", finishEdit);
			hookTooltips(_imgOK, 'Publish current web page by saving the page to the web server. You will be prompted to close the page being edited');
			//
			_imgCancel = document.createElement('img');
			_imgCancel.src = '/libjs/cancel.png';
			_imgCancel.border = 0;
			_imgCancel.style.cursor = 'pointer';
			td.appendChild(_imgCancel);
			_imgCancel.contentEditable = false;
			_imgCancel.forProperties = true;
			JsonDataBinding.AttachEvent(_imgCancel, "onclick", cancelClick);
			hookTooltips(_imgCancel, 'Close the page being edited and discard all modifications.');
			//
			_imgSave = document.createElement('img');
			_imgSave.src = '/libjs/save.png';
			_imgSave.border = 0;
			_imgSave.style.cursor = 'pointer';
			td.appendChild(_imgSave);
			_imgSave.contentEditable = false;
			_imgSave.forProperties = true;
			JsonDataBinding.AttachEvent(_imgSave, "onclick", savePageCache);
			hookTooltips(_imgSave, 'Save current page editing to the web server without applying the modifications to the original web page.');
			//
			_imgReset = document.createElement('img');
			_imgReset.src = '/libjs/reset.png';
			_imgReset.border = 0;
			_imgReset.style.cursor = 'pointer';
			td.appendChild(_imgReset);
			_imgReset.contentEditable = false;
			_imgReset.forProperties = true;
			JsonDataBinding.AttachEvent(_imgReset, "onclick", resetEditing); //handleSaveCss);
			hookTooltips(_imgReset, 'Remove all the modifications since last publishing of the web page.');
			//
			var imgSpace = document.createElement('img');
			imgSpace.src = '/libjs/space.png';
			imgSpace.border = 0;
			imgSpace.style.width = '6px';
			imgSpace.style.display = 'inline';
			td.appendChild(imgSpace);
			imgSpace.contentEditable = false;
			imgSpace.forProperties = true;
			//
			_imgShowProp = document.createElement('img');
			_imgShowProp.src = '/libjs/tglProp.png';
			_imgShowProp.border = 0;
			_imgShowProp.style.cursor = 'pointer';
			_imgShowProp.style.display = 'none';
			td.appendChild(_imgShowProp);
			_imgShowProp.contentEditable = false;
			_imgShowProp.forProperties = true;
			JsonDataBinding.AttachEvent(_imgShowProp, "onclick", toggleShowHideProps);
			hookTooltips(_imgShowProp, 'Hide/show element properties.');
			//
			imgSpace = document.createElement('img');
			imgSpace.src = '/libjs/space.png';
			imgSpace.border = 0;
			imgSpace.style.width = '6px';
			imgSpace.style.display = 'inline';
			td.appendChild(imgSpace);
			imgSpace.contentEditable = false;
			imgSpace.forProperties = true;
			//
			imgNewElement = document.createElement('img');
			imgNewElement.src = '/libjs/newElement.png';
			imgNewElement.border = 0;
			imgNewElement.style.display = 'inline';
			imgNewElement.style.cursor = 'pointer';
			td.appendChild(imgNewElement);
			imgNewElement.contentEditable = false;
			imgNewElement.forProperties = true;
			JsonDataBinding.AttachEvent(imgNewElement, "onclick", addElement);
			hookTooltips(imgNewElement, 'Insert a new HTML element into the web page');
			//
			var imgHelp = document.createElement('img');
			imgHelp.src = '/libjs/help.png';
			imgHelp.border = 0;
			imgHelp.style.display = 'inline';
			imgHelp.style.cursor = 'pointer';
			td.appendChild(imgHelp);
			imgHelp.contentEditable = false;
			imgHelp.forProperties = true;
			JsonDataBinding.AttachEvent(imgHelp, "onclick", function () { window.open('http://www.limnor.com/home1/AA/AA/htmlEditor.html'); });
			hookTooltips(imgHelp, 'Show help');
			//
			_imgLocatorElement = document.createElement('img');
			_imgLocatorElement.src = '/libjs/elementLocator.png';
			_imgLocatorElement.border = 0;
			_imgLocatorElement.style.display = 'inline';
			_imgLocatorElement.style.cursor = 'pointer';
			_imgLocatorElement.style.width = '22px';
			_imgLocatorElement.style.height = '16px';
			td.appendChild(_imgLocatorElement);
			_imgLocatorElement.contentEditable = false;
			_imgLocatorElement.forProperties = true;
			JsonDataBinding.AttachEvent(_imgLocatorElement, "onclick", locateElement);
			hookTooltips(_imgLocatorElement, 'Search HTML element by tag, id and name');
			//
			_fontCommands = document.createElement('div');
			_fontCommands.style.display = 'none';
			td.appendChild(_fontCommands);
			_fontCommands.contentEditable = false;
			_fontCommands.forProperties = true;
			var i, imgCmd;
			if (_commandList) {
				var imgVsep = document.createElement('img');
				imgVsep.contentEditable = false;
				imgVsep.forProperties = true;
				imgVsep.border = 0;
				imgVsep.src = '/libjs/vSep.png';
				imgVsep.style.display = 'inline';
				_fontCommands.appendChild(imgVsep);
				for (i = 0; i < _commandList.length; i++) {
					imgCmd = document.createElement('img');
					imgCmd.contentEditable = false;
					imgCmd.forProperties = true;
					imgCmd.border = 0;
					imgCmd.src = _commandList[i].inactImg;
					imgCmd.style.display = 'inline';
					imgCmd.cmd = _commandList[i].cmd;
					imgCmd.style.cursor = 'pointer';
					imgCmd.actSrc = _commandList[i].actImg;
					imgCmd.inactSrc = _commandList[i].inactImg;
					if (typeof _commandList[i].useUI != 'undefined') imgCmd.useUI = _commandList[i].useUI;
					if (typeof _commandList[i].value != 'undefined') imgCmd.value = _commandList[i].value;
					if (typeof _commandList[i].isCust != 'undefined') imgCmd.isCust = _commandList[i].isCust;
					JsonDataBinding.AttachEvent(imgCmd, "onclick", fontCommandSelected);
					_fontCommands.appendChild(imgCmd);
					if (_commandList[i].tooltips) {
						hookTooltips(imgCmd, _commandList[i].tooltips);
					}
					imgVsep = document.createElement('img');
					imgVsep.contentEditable = false;
					imgVsep.forProperties = true;
					imgVsep.border = 0;
					imgVsep.src = '/libjs/vSep.png';
					imgVsep.style.display = 'inline';
					_fontCommands.appendChild(imgVsep);
				}
			}
			_fontSelect = document.createElement('select');
			_fontSelect.contentEditable = false;
			_fontSelect.forProperties = true;
			_fontCommands.appendChild(_fontSelect);
			_fontSelect.size = 1;
			JsonDataBinding.AttachEvent(_fontSelect, 'onchange', onFontSelected);
			for (i = 0; i < fontList.length; i++) {
				var fi = document.createElement('option');
				fi.text = fontList[i];
				fi.value = fontList[i];
				addOptionToSelect(_fontSelect, fi);
			}
			//
			_headingSelect = document.createElement('select');
			_headingSelect.contentEditable = false;
			_headingSelect.forProperties = true;
			_fontCommands.appendChild(_headingSelect);
			_headingSelect.size = 1;
			for (i = 0; i <= 6; i++) {
				var fi = document.createElement('option');
				if (i == 0) {
					fi.text = 'Headings';
				}
				else {
					fi.text = 'heading ' + i;
					fi.value = i;
				}
				addOptionToSelect(_headingSelect, fi);
			}
			JsonDataBinding.AttachEvent(_headingSelect, 'onchange', onHeadingSelected);
			//
			_colorSelect = document.createElement('input');
			_colorSelect.type = 'text';
			_colorSelect.className = 'color';
			_colorSelect.contentEditable = false;
			_colorSelect.forProperties = true;
			_colorSelect.readOnly = true;
			_colorSelect.style.display = 'inline';
			_colorSelect.style.cursor = 'pointer';
			_colorSelect.style.width = '16px';
			_colorSelect.style.color = _colorSelect.style.backgroundColor;
			hookTooltips(_colorSelect, 'Set element color');
			jscolor.bind1(_colorSelect);
			_colorSelect.oncolorchanged = fontCommandColorSelected;
			_fontCommands.appendChild(_colorSelect);
			//
			_imgInsSpace = document.createElement('img');
			_imgInsSpace.contentEditable = false;
			_imgInsSpace.forProperties = true;
			_imgInsSpace.src = '/libjs/insSpace_inact.png';
			_imgInsSpace.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgInsSpace);
			JsonDataBinding.AttachEvent(_imgInsSpace, "onclick", insSpaceClick);
			hookTooltips(_imgInsSpace, 'Add a space outside the beginning of the current element (owner of properties) so that you may enter new contents outside of the current element.');
			//
			_imgInsBr = document.createElement('img');
			_imgInsBr.contentEditable = false;
			_imgInsBr.forProperties = true;
			_imgInsBr.src = '/libjs/insBr_inact.png';
			_imgInsBr.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgInsBr);
			JsonDataBinding.AttachEvent(_imgInsBr, "onclick", insBrClick);
			hookTooltips(_imgInsBr, 'Add a line-break outside the beginning of the current element (owner of properties) so that you may enter new contents outside of the current element.');
			//
			_imgStopTag = document.createElement('img');
			_imgStopTag.contentEditable = false;
			_imgStopTag.forProperties = true;
			_imgStopTag.src = '/libjs/stopTag_inact.png';
			_imgStopTag.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgStopTag);
			JsonDataBinding.AttachEvent(_imgStopTag, "onclick", stopTagClick);
			hookTooltips(_imgStopTag, 'Add a space outside the end of the current element (owner of properties) so that you may enter new contents outside of the current element.');
			//
			_imgAppBr = document.createElement('img');
			_imgAppBr.contentEditable = false;
			_imgAppBr.forProperties = true;
			_imgAppBr.src = '/libjs/appBr_inact.png';
			_imgAppBr.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgAppBr);
			JsonDataBinding.AttachEvent(_imgAppBr, "onclick", appBrClick);
			hookTooltips(_imgAppBr, 'Add a line-break outside the ending of the current element (owner of properties) so that you may enter new contents outside of the current element.');
			//
			var _imgbr = document.createElement('img');
			_imgbr.contentEditable = false;
			_imgbr.forProperties = true;
			_imgbr.src = '/libjs/insLF.png';
			_imgbr.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgbr);
			JsonDataBinding.AttachEvent(_imgbr, "onclick", insLFClick);
			hookTooltips(_imgbr, 'Insert a line-break at the caret position.');
			//
			_imgAppTxt = document.createElement('img');
			_imgAppTxt.contentEditable = false;
			_imgAppTxt.forProperties = true;
			_imgAppTxt.src = '/libjs/appTxt.png';
			_imgAppTxt.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgAppTxt);
			JsonDataBinding.AttachEvent(_imgAppTxt, "onclick", appTxtClick);
			hookTooltips(_imgAppTxt, 'Append a new paragraph at the end of the web page.');
			//
			_imgUndel = document.createElement('img');
			_imgUndel.contentEditable = false;
			_imgUndel.forProperties = true;
			_imgUndel.src = '/libjs/undel_inact.png';
			_imgUndel.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgUndel);
			JsonDataBinding.AttachEvent(_imgUndel, "onclick", undelete);
			hookTooltips(_imgUndel, 'Make an undo to the last element deletion');
			//
			_imgUndo = document.createElement('img');
			_imgUndo.contentEditable = false;
			_imgUndo.forProperties = true;
			_imgUndo.src = '/libjs/undo_inact.png';
			_imgUndo.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgUndo);
			JsonDataBinding.AttachEvent(_imgUndo, "onclick", undo);
			hookTooltips(_imgUndo, 'Make an undo');
			//
			_imgRedo = document.createElement('img');
			_imgRedo.contentEditable = false;
			_imgRedo.forProperties = true;
			_imgRedo.src = '/libjs/redo_inact.png';
			_imgRedo.style.cursor = 'pointer';
			_fontCommands.appendChild(_imgRedo);
			JsonDataBinding.AttachEvent(_imgRedo, "onclick", redo);
			hookTooltips(_imgRedo, 'Make a redo');
			//===disable undo/redo for now
			_imgUndo.style.display = 'none';
			_imgRedo.style.display = 'none';
			//color to hex - for user convenience
			inputColorPicker = document.createElement('input');
			inputColorPicker.type = 'text';
			inputColorPicker.style.width = '60px';
			inputColorPicker.readOnly = true;
			inputColorPicker.style.display = 'inline';
			inputColorPicker.style.cursor = 'pointer';
			inputColorPicker.className = 'color';
			inputColorPicker.contentEditable = false;
			inputColorPicker.forProperties = true;
			_fontCommands.appendChild(inputColorPicker);
			jscolor.bind1(inputColorPicker);
			//
			if (containerDiv) {
				_fontSelect.style.display = 'none';
				_headingSelect.style.display = 'none';
				inputColorPicker.style.display = 'none';
			}
			//===end of tool bar=========================================
			//create editing area========================================
			_editorDiv = document.createElement('div');
			_divProp.appendChild(_editorDiv);
			_editorDiv.style.padding = 0;
			_editorDiv.style.margin = 0;
			_editorDiv.style.border = 0;
			_editorDiv.style.overflowX = 'hidden';
			_editorDiv.style.overflowY = 'hidden';
			_editorDiv.contentEditable = false;
			_editorDiv.forProperties = true;
			_editorDiv.style.width = '100%';
			_editorDiv.style.height = '100%';
			_editorTable = document.createElement("table");
			_editorDiv.appendChild(_editorTable);
			_editorTable.contentEditable = false;
			_editorTable.forProperties = true;
			_editorTable.style.backgroundColor = 'white';
			_editorTable.border = 1;
			_editorTable.width = '100%';
			_editorBody = JsonDataBinding.getTableBody(_editorTable);
			tr = _editorBody.insertRow(-1);
			//parent list
			td = document.createElement('td');
			td.vAlign = 'top';
			tr.appendChild(td);
			_tdParentList = td;
			_tdParentList.style.backgroundColor = '#A9F5A9';
			var _objTable = document.createElement('table');
			_objTable.border = 0;
			_objTable.cellPadding = 0;
			_objTable.cellSpacing = 0;
			_objTable.style.padding = '0px';
			_objTable.style.backgroundColor = '#A9F5A9';
			_tdParentList.appendChild(_objTable);
			var objTbody = JsonDataBinding.getTableBody(_objTable);
			var objRow = objTbody.insertRow(-1);
			_parentList = document.createElement('select');
			_parentList.style.border = 'none';
			_parentList.style.cursor = 'pointer';
			_parentList.style.backgroundColor = '#A9F5A9';
			td = document.createElement('td');
			objRow.appendChild(td);
			td.appendChild(_parentList);
			JsonDataBinding.AttachEvent(_parentList, "onchange", selectparent);
			_trObjDelim = objTbody.insertRow(-1);
			_tdObjDelim = document.createElement('td');
			_trObjDelim.appendChild(_tdObjDelim);
			_trObjDelim.style.height = '2px';
			_trObjDelim.style.backgroundColor = 'black';
			_trObjDelim.style.display = 'none';
			//
			objRow = objTbody.insertRow(-1);
			_tdObj = document.createElement('td');
			objRow.appendChild(_tdObj);
			_tdObj.style.overflowX = 'auto';
			_tdObj.style.overflowY = 'hidden';
			//_tdObj.style.height = '50%';
			//property grid ---------------------------------------------------------
			td = document.createElement('td');
			td.style.width = '100%';
			td.vAlign = 'top';
			tr.appendChild(td);
			//
			_divElementToolbar = document.createElement("div");
			_divElementToolbar.contentEditable = false;
			_divElementToolbar.forProperties = true;
			_divElementToolbar.style.width = '100%';
			_divElementToolbar.style.backgroundColor = '#FFFF9E';
			_divElementToolbar.style.textAlign = 'left';
			td.appendChild(_divElementToolbar);
			//var imgtest = document.createElement("img");
			//imgtest.src = '/libjs/html_a.png';
			//_divElementToolbar.appendChild(imgtest);
			//
			_divPropertyGrid = document.createElement("div");
			_divPropertyGrid.contentEditable = false;
			_divPropertyGrid.forProperties = true;
			_divPropertyGrid.style.width = '100%';
			_divPropertyGrid.style.textAlign = 'left';
			_divPropertyGrid.style.overflowX = 'hidden';
			_divPropertyGrid.style.overflowY = 'auto';
			_divPropertyGrid.onscroll = onPropGridScroll;
			td.appendChild(_divPropertyGrid);
			//
			_tableProps = document.createElement("table");
			_tableProps.border = 1;
			_tableProps.width = "100%";
			_divPropertyGrid.style.backgroundColor = '#E0F8E0';
			_divPropertyGrid.appendChild(_tableProps);
			_propsBody = JsonDataBinding.getTableBody(_tableProps);
			_propsBody.nameWidth = 60;
			//
			tr = _propsBody.insertRow(-1);
			td = document.createElement('td');
			tr.appendChild(td);
			td.innerHTML = "Name";
			td.style.width = _propsBody.nameWidth;
			td = document.createElement('td');
			tr.appendChild(td);
			td.innerHTML = "Value";
			td.style.width = '100%';
			//
			//message row at the bottom =======================
			_spMsg = document.createElement('span');
			_spMsg.style.width = '100%';
			_spMsg.style.height = '100%';
			_spMsg.style.overflow = 'visible';
			_spMsg.innerHTML = '';
			_spMsg.style.fontSize = 'x-small';
			_messageBar = document.createElement('div');
			_divProp.appendChild(_messageBar);
			_messageBar.style.verticalAlign = 'top';
			_messageBar.appendChild(_spMsg);
			//
			JsonDataBinding.AttachEvent(_divProp, "onscroll", editorScroll);
			//
			_divSelectElement.style.display = 'none';
			_showResizer();
			//
			if (_isIE) {
				document.execCommand('RespectVisibilityInDesign', true, null);
			}
			//
			var stl;
			for (var s = 0; s < document.styleSheets.length; s++) {
				if (document.styleSheets[s].title == 'htmlEditor_menuEditor') {
					stl = document.styleSheets[s];
					break;
				}
			}
			if (!stl) {
				var st = document.createElement('style');
				st.title = 'htmlEditor_menuEditor';
				st.setAttribute('hidden', 'true');
				document.getElementsByTagName('head')[0].appendChild(st);
				for (var s = 0; s < document.styleSheets.length; s++) {
					if (document.styleSheets[s].title == 'htmlEditor_menuEditor') {
						stl = document.styleSheets[s];
						break;
					}
				}
				var idx = 0;
				var selector = 'nav.htmlEditor_menuEditor';
				var rule = selector + ' {background:#E0FFFF;cursor:pointer;}';
				if (stl.insertRule) {
					stl.insertRule(rule, idx);
				}
				else {
					stl.addRule(selector, rule, idx);
				}
				idx++;
				rule = selector + ' ul li:hover {background:#D3D3D3;}';
				if (stl.insertRule) {
					stl.insertRule(rule, idx);
				}
				else {
					stl.addRule(selector + ' ul li:hover', rule, idx);
				}
				idx++;
				rule = selector + ' ul li a:hover {background:#D3D3D3;}';
				if (stl.insertRule) {
					stl.insertRule(rule, idx);
				}
				else {
					stl.addRule(selector + ' ul li a:hover', rule, idx);
				}
				idx++;
			}
			//
			_initSelection();
			//
			function editorScroll() {
				_showResizer();
			}
			//
			function mousedown(e) {
				var selectedElement;
				function onselectElement() {
					if (_editorOptions) {
						selectEditElement(selectedElement);
						if (_editorOptions.forIDE && e.button == JsonDataBinding.mouseButtonRight()) {
							var pos = _editorOptions.client.getElementPosition.apply(_editorOptions.editorWindow, [selectedElement]);//JsonDataBinding.ElementPosition.getElementPosition(selectedElement);
							var objStr = _getSelectedObject(selectedElement);
							if (typeof (limnorStudio) != 'undefined') limnorStudio.onRightClickElement(objStr, pos.x, pos.y);
							else window.external.OnRightClickElement(objStr, pos.x, pos.y);
						}
					}
				}
				e = e || document.parentWindow.event;
				var c = JsonDataBinding.getSender(e);
				var x, y;
				if (_resizer) {
					if (c == _resizer) {
						_resizer.isMousedown = true;
						_divProp.isMousedown = false;
						_nameresizer.isMousedown = false;
						x = _resizer.offsetLeft;
						y = _resizer.offsetTop;
						_resizer.ox = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - x;
						_resizer.oy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - y;
						if (_nameresizer) {
							_nameresizer.style.display = 'none';
						}
						return false;
					}
				}
				if (_nameresizer) {
					if (c == _nameresizer) {
						_nameresizer.isMousedown = true;
						_resizer.isMousedown = false;
						_divProp.isMousedown = false;
						x = _nameresizer.offsetLeft;
						y = _nameresizer.offsetTop;
						_nameresizer.ox = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - x;
						_nameresizer.oy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - y;
						return false;
					}
				}
				var c0 = c;
				var isAddNew = false;
				while (c0) {
					if (c0 == _divSelectElement) {
						isAddNew = true;
						break;
					}
					c0 = c0.parentNode;
				}
				if (!isAddNew) {
					hideSelector();
				}
				if (_tdTitle == c) {
					x = _divProp.offsetLeft;
					y = _divProp.offsetTop;
					_divProp.ox = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - x;
					_divProp.oy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - y;
					_divProp.isMousedown = true;
					_nameresizer.isMousedown = false;
					_resizer.isMousedown = false;
					if (_resizer) {
						_resizer.style.display = 'none';
					}
					if (_nameresizer) {
						_nameresizer.style.display = 'none';
					}
					//if (window.opera) {
					//	document.body.contentEditable = false;
					//}
					return false;
				}
				else {
					_divProp.isMousedown = false;
					_resizer.isMousedown = false;
					_nameresizer.isMousedown = false;
					//if (window.opera) {
					//	if (isEditorObject(c)) {
					//		document.body.contentEditable = false;
					//	}
					//}
					if (_editorOptions && _editorOptions.inline) {
						if (!isAddNew) {
							if (selectedElement && !selectedElement.forProperties) {
								selectedElement = c;
								//TBD: review logic
								if (_isIE) {
									if (_editorOptions.forIDE) {
										selectEditElement(selectedElement);
										if (e.button == JsonDataBinding.mouseButtonRight()) {
											var pos = _editorOptions.client.getElementPosition.apply(_editorOptions.editorWindow, [selectedElement]);
											var objStr = _getSelectedObject(selectedElement);
											if (typeof (limnorStudio) != 'undefined') limnorStudio.onRightClickElement(objStr, pos.x, pos.y);
											else window.external.OnRightClickElement(objStr, pos.x, pos.y);
										}
									}
									else {
										setTimeout(onselectElement, 10);
									}
								}
								else {
									selectEditElement(selectedElement);
								}
							}
							else {
							}
						}
					}
				}
				return true;
			}

			//function keyup(e) {
			//	if (_editorOptions && _editorOptions.inline && e) {
			//		//_capEvent=e;
			//		//captureSelection(e);
			//	}
			//	return true;
			//}

			function keydown(e) {
				e = e || event;
				if (!e) {
					return true;
				}
				var code = e.keyCode || e.which || null;
				if (code) {
					if (_editorOptions) {	 
						if (!(code >= 33 && code <= 40)) {
							_editorOptions.pageChanged = true;
						}
					}
					if (code == 8) {
						if (!_handleBackspace(false)) {
							window.event.returnValue = false;
							if (e.preventDefault) {
								e.preventDefault();
							}
							return false;
						}
					}
				}
				return true;
			}
			JsonDataBinding.AttachEvent(_imgExpand, "onclick", _togglePropertiesWindow);
			JsonDataBinding.AttachEvent(document, "onclick", docClick);
			JsonDataBinding.AttachEvent(document, "onmousedown", mousedown);
			JsonDataBinding.AttachEvent(document, "onmouseup", mouseup);
			JsonDataBinding.AttachEvent(document, "onmousemove", mousemove);
			//JsonDataBinding.AttachEvent(document, "onkeyup", keyup);
			if (document.addEventListener) {
				document.addEventListener('keydown', keydown, true);
			}
			else {
				if (document.attachEvent) {
					document.attachEvent('onkeydown', keydown);
				}
			}
			var ifrs = document.body.getElementsByTagName('iframe');
			if (ifrs && ifrs.length > 0) {
				for (i = 0; i < ifrs.length; i++) {
					var fdoc = ifrs[i].contentDocument || ifrs[i].contentWindow.document;
					fdoc.onclick = framenotify;
				}
			}
			if (containerDiv) {
				//_imgCancel.style.display = 'none';
				_imgSave.style.display = 'none';
				_imgReset.style.display = 'none';
				imgNewElement.style.display = 'none';
				_imgShowProp.style.display = 'inline';
				//
				//create one more td and append the page holder; the td is for holding child page
				var tr = _editorBody.rows[0];
				tr.cells[1].style.width = 'auto';
				var td;
				if (tr.cells.length < 3) {
					td = document.createElement('td');
					td.vAlign = 'top';
					td.style.width = '100%';
					//td.style.overflowY = 'scroll';
					tr.appendChild(td);
				}
				else {
					td = tr.cells[2];
				}
				td.innerHTML = '';
				containerDiv.tdContentContainer = td;
			}
			else {
				setInterval(handlePageCache, _autoSaveMinutes * 60 * 1000);
			}
		} //end of _init =====================================================================
		function removeEditorElements() {
			var i;
			if (_topElements) {
				for (i = 0; i < _topElements.length; i++) {
					var p = _topElements[i].parentNode;
					if (p) {
						p.removeChild(_topElements[i]);
					}
				}
			}
			if (_propertyTopElements) {
				for (i = 0; i < _propertyTopElements.length; i++) {
					var p = _propertyTopElements[i].parentNode;
					if (p) {
						p.removeChild(_propertyTopElements[i]);
					}
				}
			}
		}
		function _removeDuplicatedFile() {
			var head = _editorOptions.elementEditedDoc.documentElement.parentNode.getElementsByTagName('head')[0];
			var scriptNodes = head.getElementsByTagName('script');
			var linkNodes = head.getElementsByTagName('link');
			var i, k;
			var files, f, b, dups;
			if (scriptNodes && scriptNodes.length > 0) {
				files = new Array();
				dups = new Array();
				for (i = 0; i < scriptNodes.length; i++) {
					f = scriptNodes[i].getAttribute('src');
					if (f) {
						f = f.toLowerCase();
						b = false;
						for (k = 0; k < files.length; k++) {
							if (files[k] == f) {
								b = true;
								break;
							}
						}
						if (b) {
							dups.push(scriptNodes[i]);
						}
						else {
							files.push(f);
						}
					}
				}
				for (i = 0; i < dups.length; i++) {
					head.removeChild(dups[i]);
				}
			}
			if (linkNodes && linkNodes.length > 0) {
				files = new Array();
				dups = new Array();
				for (i = 0; i < linkNodes.length; i++) {
					f = linkNodes[i].getAttribute('href');
					if (f) {
						f = f.toLowerCase();
						b = false;
						for (k = 0; k < files.length; k++) {
							if (files[k] == f) {
								b = true;
								break;
							}
						}
						if (b) {
							dups.push(linkNodes[i]);
						}
						else {
							files.push(f);
						}
					}
				}
				for (i = 0; i < dups.length; i++) {
					head.removeChild(dups[i]);
				}
			}
		}
		function _getresultHTML(publishing) {
			if (jscolor && jscolor.picker) {
				if (jscolor.picker.owner) {
					delete jscolor.picker.owner;
				}
				if (jscolor.picker.boxB) {
					if (jscolor.picker.boxB.parentNode == document.body) {
						document.body.removeChild(jscolor.picker.boxB);
					}
				}
				jscolor.picker = null;
			}
			var prbox;
			if (_editorOptions.redbox) {
				prbox = _editorOptions.redbox.parentNode;
				if (prbox) {
					_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [prbox, _editorOptions.redbox]);
				}
			}
			var pmarkers = [];
			if (_editorOptions.markers) {
				for (var k = 0; k < _editorOptions.markers.length; k++) {
					pmarkers.push(null);
					if (_editorOptions.markers[k]) {
						prbox = _editorOptions.markers[k].parentNode;
						if (prbox) {
							pmarkers[k] = prbox;
							_editorOptions.client.removeChild.apply(_editorOptions.elementEditedWindow, [prbox, _editorOptions.markers[k]]);
						}
					}
				}
			}
			var _resultHtml;
			if (_editorOptions.isEditingBody) {
				_editorOptions.client.cleanup.apply(_editorOptions.elementEditedWindow, [_editorOptions.client, publishing]);
				_editorOptions.elementEdited.removeAttribute('contentEditable');
				if (_editorOptions.inline) {
					removeEditorElements();
				}
				var i;
				_removeDuplicatedFile();
				setMetaData(_editorOptions.elementEditedDoc.documentElement, 'generator', 'Limnor Visual HTML Editor -- Online Edition (http://www.limnor.com)');
				setContentType(_editorOptions.elementEditedDoc.documentElement, 'text/html; charset=UTF-8');
				setIECompatible(_editorOptions.elementEditedDoc.documentElement);
				setContentNoCache(_editorOptions.elementEditedDoc.documentElement);
				if (_editorOptions.elementEditedDoc.documentElement.outerHTML) {
					if (_editorOptions.docType) {
						_resultHtml = _editorOptions.docType + '\r\n' + _editorOptions.elementEditedDoc.documentElement.outerHTML;
					}
					else {
						_resultHtml = _editorOptions.elementEditedDoc.documentElement.outerHTML;
					}
				}
				else {
					var htmlLine = '<html';
					var attrs = _editorOptions.elementEditedDoc.documentElement.attributes;
					if (attrs) {
						for (i = 0; i < attrs.length; i++) {
							if (attrs[i].nodeValue && attrs[i].nodeValue.length > 0) {
								htmlLine += ' ';
								htmlLine += attrs[i].nodeName;
								htmlLine += '=';
								htmlLine += attrs[i].nodeValue;
							}
						}
					}
					htmlLine += '>';
					if (_editorOptions.docType) {
						_resultHtml = _editorOptions.docType + '\r\n' + htmlLine + _editorOptions.elementEditedDoc.documentElement.innerHTML + '</html>';
					}
					else {
						_resultHtml = htmlLine + _editorOptions.elementEditedDoc.documentElement.innerHTML + '</html>';
					}
				}
				_editorOptions.client.onAfterSaving.apply(_editorOptions.elementEditedWindow);
				var pos0, pos1;
				while (true) {
					pos0 = _resultHtml.indexOf(_editorStart);
					if (pos0 >= 0) {
						pos1 = _resultHtml.indexOf(_editorEnd);
						if (pos1 > 0) {
							_resultHtml = _resultHtml.substr(0, pos0) + _resultHtml.substr(pos1 + _editorEnd.length);
						}
						else break;
					}
					else break;
				}
				while (true) {
					pos0 = _resultHtml.indexOf(_editorStartJs);
					if (pos0 >= 0) {
						pos1 = _resultHtml.indexOf(_editorEndJs);
						if (pos1 > 0) {
							_resultHtml = _resultHtml.substr(0, pos0) + _resultHtml.substr(pos1 + _editorEndJs.length);
						}
						else break;
					}
					else break;
				}
				if (_editorOptions.inline && _editorOptions.isEditingBody) {
					if (_topElements) {
						for (i = 0; i < _topElements.length; i++) {
							document.body.appendChild(_topElements[i]);
						}
					}
					if (_propertyTopElements) {
						for (i = 0; i < _propertyTopElements.length; i++) {
							document.body.appendChild(_propertyTopElements[i]);
						}
					}
				}
				_editorOptions.elementEdited.contentEditable = true;
			}
			else {
				_editorOptions.elementEdited.contentEditable = false;
				if (_editorOptions.elementEdited.outerHTML) {
					_resultHtml = _editorOptions.elementEdited.outerHTML;
				}
				else {
					var p = _editorOptions.elementEdited.parentNode;
					var p2 = _editorOptions.elementEditedDoc.createElement(p.tagName);
					p2.appendChild(_editorOptions.elementEdited);
					_resultHtml = p2.innerHTML;
					p.appendChild(_editorOptions.elementEdited);
				}
				_editorOptions.elementEdited.contentEditable = true;
			}
			if (prbox) {
				_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [prbox, _editorOptions.redbox]);
				_editorOptions.redbox.contentEditable = false;
			}
			if (_editorOptions.markers) {
				for (var k = 0; k < _editorOptions.markers.length; k++) {
					if (_editorOptions.markers[k]) {
						prbox = pmarkers[k];
						if (prbox) {
							_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [prbox, _editorOptions.markers[k]]);
							_editorOptions.markers[k].contentEditable = false;
						}
					}
				}
			}
			return _resultHtml;
		}
		function _setSelectedObject(guid) {
			if (_editorOptions) {
				if (guid == '') {
					selectEditElement(_editorOptions.elementEditedDoc.body);
					return;
				}
				function selectElementByGuid(e, guid) {
					if (e.childNodes && e.childNodes.length > 0) {
						for (var i = 0; i < e.childNodes.length; i++) {
							if (e.childNodes[i].getAttribute) {
								var g = e.childNodes[i].getAttribute('limnorId');
								if (g && g == guid) {
									if (e.childNodes[i].id && e.childNodes[i].id.length > 0) {
									}
									else {
										e.childNodes[i].id = _createId(e.childNodes[i]);
									}
									selectEditElement(e.childNodes[i]);
									return true;
								}
							}
							var ret = selectElementByGuid(e.childNodes[i], guid);
							if (ret) {
								return ret;
							}
						}
					}
				}
				selectElementByGuid(_editorOptions.elementEditedDoc.body, guid);
			}
		}
		function _getMenubarString(obj) {
			var ret = '';
			function getmenubaritem(mi) {
				if (mi && mi.tagName && mi.tagName.toLowerCase() == 'li') {
					ret += '[';
					if (mi.id && mi.id.length > 0) {
					}
					else {
						mi.id = _createId(mi);
						_editorOptions.pageChanged = true;
					}
					ret += mi.id + ';';
					var txt = '';
					for (var k = 0; k < mi.children.length; k++) {
						if (mi.children[k].tagName && mi.children[k].tagName.toLowerCase() == 'a') {
							txt = mi.children[k].innerHTML;
							break;
						}
					}
					txt = txt.replace('[', '#005B#');
					txt = txt.replace(']', '#005D#');
					txt = txt.replace(',', '#002C#');
					ret += txt;
					getmesubnubar(mi);
					ret += ']';
				}
			}
			function getmesubnubar(o) {
				if (o.children.length > 0) {
					for (var i = 0; i < o.children.length; i++) {
						if (o.children[i] && o.children[i].tagName && o.children[i].tagName.toLowerCase() == 'ul') {
							//ret += '[';
							for (var j = 0; j < o.children[i].children.length; j++) {
								getmenubaritem(o.children[i].children[j]);
							}
							//ret += ']';
						}
					}
				}
			}
			getmesubnubar(obj);
			return ret;
		}
		function _getSelectedObject(obj) {
			var sRet = '';
			if (_editorOptions) {
				if (!obj) {
					obj = _editorOptions.ideCurrentobj || _editorOptions.selectedObject;
				}
				if (obj) {
					if (obj.getStringForIDE) {
						sRet = obj.getStringForIDE();
					}
					else if (obj.tagName) {
						var tag = obj.tagName.toLowerCase();
						if (tag == 'form') {
							if (obj.getAttribute('typename') == 'fileupload') {
								tag = 'fileupload';
							}
						}
						sRet = tag + ',';
						var t1;
						var t0 = obj.getAttribute('type');
						if (tag == 'nav') {
							if (obj.className == 'limnorstyles_menu') {
								t0 = 'menubar';
								t1 = _getMenubarString(obj);
							}
						}
						else if (tag == 'ul') {
							if (obj.className == 'limnortv') {
								t0 = 'tv';
							}
						}
						if (t0) sRet += t0;
						sRet += ',';
						if (!t1) t1 = obj.getAttribute('limnortype');
						if (t1) sRet += t1;
						sRet += ',';
						if (obj.id && obj.id.length > 0) {
							if (tag == 'fileupload') {
								sRet += obj.id.substr(0, obj.id.length-1);
							}
							else {
								sRet += obj.id;
							}
						}
						sRet += ',';
						var g = obj.getAttribute('limnorId');
						if (g) {
							sRet += g;
						}
						else {
							if (tag == 'fileupload') {
								for (var i = 0; i < obj.children.length; i++) {
									g = obj.children[i].getAttribute('limnorId');
									if (g) {
										sRet += g;
										break;
									}
								}
							}
						}
						var src0;
						if (tag == 'img' || tag == 'script') {
							sRet += ',src,';
							src0 = obj.src;
							if (src0) {
								src0 = src0.replace(_webPath, '');
								sRet += src0;
							}
						}
						else if (tag == 'link') {
							sRet += ',href,';
							src0 = obj.href;
							if (src0) {
								src0 = src0.replace(_webPath, '');
								sRet += src0;
							}
						}
						else if (tag == 'area') {
							sRet += ',shape,';
							src0 = obj.shape;
							if (src0) {
								sRet += src0;
							}
							sRet += ',coords,';
							src0 = obj.coords;
							if (src0) {
								sRet += src0.replace(/\,/g, '_');
							}
						}
						else if (tag == 'object') {
							sRet += ',data,';
							src0 = obj.data;
							if (src0) {
								sRet += src0;
							}
							sRet += ',archive,';
							src0 = obj.archive;
							if (src0) {
								sRet += src0;
							}
						}
						_editorOptions.ideCurrentobj = obj;
					}
				}
			}
			return sRet;
		}
		function _setbodyBk(bkFile, bkTile) {
			if (_editorOptions && _editorOptions.client) {
				_setElementStyleValue(_editorOptions.elementEditedDoc.body, 'backgroundImage','background-image', bkFile);
				_setElementStyleValue(_editorOptions.elementEditedDoc.body, 'backgroundRepeat','background-repeat', bkTile);
				_editorOptions.pageChanged = true;
				return true;
			}
			return false;
		}
		function _getIdList() {
			var s = '';
			function getidlist() {
				if (e.childNodes && e.childNodes.length > 0) {
					for (var i = 0; i < e.childNodes.length; i++) {
						if (e.childNodes[i].getAttribute) {
							var g = e.childNodes[i].getAttribute('limnorId');
							if (g && g.length > 0) {
								if (e.childNodes[i].id && e.childNodes[i].id.length > 0) {
								}
								else {
									e.childNodes[i].id = _createId(e.childNodes[i]);
								}
								if (s.length > 0) {
									s += ',';
								}
								s += (g + ':' + e.childNodes[i].id);
							}
						}
					}
				}
			}
			getidlist(_editorOptions.elementEditedDoc.body);
			return s;
		}
		function _setGuidById(id, guid) {
			if (_editorOptions.client.setGuidById.apply(_editorOptions.elementEditedWindow, [id, guid])) {
				_editorOptions.pageChanged = true;
				return true;
			}
			return false;
		}
		function _getIdByGuid(guid) {
			function getidbyguid(e) {
				if (e.childNodes && e.childNodes.length > 0) {
					for (var i = 0; i < e.childNodes.length; i++) {
						if (e.childNodes[i].getAttribute) {
							var g = e.childNodes[i].getAttribute('limnorId');
							if (g && g == guid) {
								if (e.childNodes[i].id && e.childNodes[i].id.length > 0) {
								}
								else {
									e.childNodes[i].id = _createId(e.childNodes[i]);
								}
								return e.childNodes[i].id;
							}
						}
						var id = getidbyguid(e.childNodes[i]);
						if (id && id.length > 0) {
							return id;
						}
					}
				}
			}
			return getidbyguid(_editorOptions.elementEditedDoc.body);
		}
		function _setGuid(guid) {
			if (_editorOptions.ideCurrentobj) {
				if (guid && guid.length > 0) {
					var g = _editorOptions.ideCurrentobj.getAttribute('limnorId');
					if (g && g.length > 0) {
						return 6;
					}
					else {
						_editorOptions.ideCurrentobj.setAttribute('limnorId', guid);
						return 0;
					}
				}
				else {
					return 3;
				}
			}
			else {
				return 1;
			}
		}
		function _onBeforeIDErun() {
			if (_editorOptions) {
				JsonDataBinding.pollModifications();
				//alert('C');
				//alert(_comboInput);
				if (_comboInput && _comboInput.oncombotxtChange) {
					_comboInput.oncombotxtChange();
					//alert('combo input');
				}
			}
		}
		function _doCopy() {
			if (_editorOptions) {
				if (_editorOptions.selectedObject) {
					captureSelection({ target: _editorOptions.selectedObject }, true);
					if (_editorOptions.selectedHtml && _editorOptions.selectedHtml.length > 0) {
						window.clipboardData.setData('text', _editorOptions.selectedHtml);
					}
					else {
						var tag = _editorOptions.selectedObject.tagName ? _editorOptions.selectedObject.tagName.toLowerCase() : '';
						if (tag != 'body' && tag != 'html' && tag != 'link' && tag != 'head') {
							var p = _editorOptions.selectedObject.parentNode;
							while (p) {
								tag = p.tagName ? p.tagName.toLowerCase() : '';
								if (tag == 'body') {
									window.clipboardData.setData('text', _editorOptions.selectedObject.outerHTML);
									return;
								}
								p = p.parentNode;
							}
						}
					}
				}
			}
		}
		function _doPaste() {
			var data = window.clipboardData.getData('text');
			if (typeof data != 'undefined' && data != null && data.length > 0) {
				if (data.substr(0, 1) == '<' && data.substr(data.length - 1, 1) == '>') {
					var div = _createElement('div');
					div.innerHTML = data;
					if (div.children.length > 0) {
						var c = div.children[0];
						div.removeChild(c);
						insertNodeOverSelection(c, c.tagName);
						if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
					}
				}
				else {
					if (_editorOptions.elementEditedDoc) {
						var range = _editorOptions.elementEditedDoc.selection.createRange();
						range.pasteHTML(data);
						if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
					}
				}
			}
		}
		function _pasteToHtmlInput(txt, selStart, selEnd) {
			if (_editorOptions && _editorOptions.propInput) {
				//alert(_editorOptions.propInput.selectionStart + ', ' + _editorOptions.propInput.selectionEnd);
				var newText = '';
				if (selStart >= 0 && selStart < _editorOptions.propInput.value.length)
					newText = _editorOptions.propInput.value.substr(0, selStart);
				newText = newText + txt;
				if (selEnd >= 0 && selEnd < _editorOptions.propInput.value.length)
					newText = newText + _editorOptions.propInput.value.substr(selEnd);
				else
					newText = _editorOptions.propInput.value + newText;
				_editorOptions.propInput.value = newText;
			}
		}
		function _createNewStyle(obj) {
			if (_editorOptions && _editorOptions.client) {
				var tag = obj.tagName.toLowerCase();
				if (tag == 'input') {
					var tp = obj.getAttribute('type');
					if (tp) {
						tag = tp;
					}
				}
				var bn = tag + 'style';
				return _editorOptions.client.createNewStyleName.apply(_editorOptions.editorWindow,[bn]);
			}
		}
		function getyoutubeid(v) {
			if (v) {
				v = v.trim();
				var pos = v.toLowerCase().indexOf('v=');
				if (pos >= 0) {
					v = v.substr(pos + 2);
				}
				else {
					pos = v.lastIndexOf('/');
					if (pos >= 0) {
						v = v.substr(pos + 1);
					}
				}
				pos = v.indexOf('&');
				if (pos > 0) {
					v = v.substr(0, pos);
				}
			}
			else {
				v = '';
			}
			return v;
		}
		function _updateYoutubeTarget(a) {
			var yid = a.getAttribute('youtube');
			var vid = a.getAttribute('youtubeID');
			if (yid && yid != '' && vid && vid != '') {
				a.setAttribute('onclick', "javascript:document.getElementById('" + yid + "').src='http://www.youtube.com/embed/" + vid + "';");
				a.savedhref = '';
				a.savedtarget = '';
				a.removeAttribute('mediaFile');
			}
			else {
				var mpr = a.getAttribute('mediaPlayer');
				var med = a.getAttribute('mediaFile');
				if (mpr && mpr != '' && med && med != '') {
					_updateMediaTarget(a);
				}
				else {
					a.removeAttribute('onclick');
				}
			}
		}
		function _updateMediaTarget(a) {
			var mpr = a.getAttribute('mediaPlayer');
			var med = a.getAttribute('mediaFile');
			if (mpr && mpr != '' && med && med != '') {
				a.setAttribute('onclick', "javascript:var e=document.getElementById('" + mpr + "');if(!e) return;var p=e.parentNode;if(!p) return;var o=e.cloneNode(false);o.src='" + med + "';p.insertBefore(o, e);p.removeChild(e);o.id=e.id;");
				a.savedhref = '';
				a.savedtarget = '';
				a.removeAttribute('youtubeID');
			}
			else {
				var yid = a.getAttribute('youtube');
				var vid = a.getAttribute('youtubeID');
				if (yid && yid != '' && vid && vid != '') {
					_updateYoutubeTarget(a);
				}
				else {
					a.removeAttribute('onclick');
				}
			}
		}
		function _createId(obj) {
			var tag = obj.tagName.toLowerCase();
			if (tag == 'input') {
				var tp = obj.getAttribute('type');
				if (tp) {
					tag = tp;
				}
			}
			var n = 1;
			var newId = tag + n;
			while (_editorOptions.elementEditedDoc.getElementById(newId)) {
				n++;
				newId = tag + n;
			}
			if (_editorOptions.ideUsedNames && _editorOptions.ideUsedNames.length > 0) {
				var exist = true;
				while (exist) {
					exist = false;
					for (var i = 0; i < _editorOptions.ideUsedNames.length; i++) {
						if (_editorOptions.ideUsedNames[i] == newId) {
							exist = true;
							break;
						}
					}
					if (exist) {
						n++;
						newId = tag + n;
					}
				}
			}
			return newId;
		}
		function _createOrGetId(guid) {
			if (_editorOptions.ideCurrentobj) {
				if (_editorOptions.ideCurrentobj.id && _editorOptions.ideCurrentobj.id.length > 0) {
				}
				else {
					_editorOptions.ideCurrentobj.id = _createId(_editorOptions.ideCurrentobj);
					_editorOptions.pageChanged = true;
				}
				if (guid && guid.length > 0) {
					var g = _editorOptions.ideCurrentobj.getAttribute('limnorId');
					if (g && g.length > 0) {
					}
					else {
						_editorOptions.ideCurrentobj.setAttribute('limnorId', guid);
						_editorOptions.pageChanged = true;
					}
				}
				return _editorOptions.ideCurrentobj.id;
			}
			else {
				return '';
			}
		}
		function _getMaps() {
			var ret = '';
			var maps = document.getElementsByTagName('map');
			if (maps) {
				for (var i = 0; i < maps.length; i++) {
					if (!(maps[i].id)) {
						maps[i].id = _createId(maps[i]);
					}
					var s = _getSelectedObject(maps[i]);
					if (i > 0) {
						ret += '|';
					}
					ret += s;
				}
			}
			return ret;
		}
		function _getAreas(mapId) {
			var ret = '';
			var map = document.getElementById(mapId);
			if (map) {
				var arealist = map.getElementsByTagName('area');
				if (arealist) {
					for (var i = 0; i < arealist.length; i++) {
						if (!(arealist[i].id)) {
							arealist[i].id = _createId(arealist[i]);
						}
						var s = _getSelectedObject(arealist[i]);
						if (i > 0) {
							ret += '|';
						}
						ret += s;
					}
				}
			}
			return ret;
		}
		function _createNewMap(guid) {
			var map = document.createElement('map');
			var mapId = _createId(map);
			map.id = mapId;
			map.setAttribute('limnorId', guid);
			document.body.appendChild(map);
			return _getSelectedObject(map);
		}
		function _setMapAreas(mapId, arealist) {
			var map = document.getElementById(mapId);
			if (map) {
				while (map.hasChildNodes()) {
					map.removeChild(map.firstChild);
				}
				var areaStrings = arealist.split('|');
				for (var i = 0; i < areaStrings.length; i++) {
					var ss = areaStrings[i].split(';');
					if (ss.length > 3) {
						var a;
						a = document.createElement('area');
						map.appendChild(a);
						a.id = ss[0];
						a.setAttribute('limnorId', ss[1]);
						a.setAttribute('shape', ss[2]);
						a.setAttribute('coords', ss[3]);
					}
				}
			}
		}
		function _bringToFront() {
			_divProp.style.zIndex = JsonDataBinding.getPageZIndex(_divProp) + 1;
		}
		function _setInlineTarget(pageHolder) {
			//add a div as the element to be edited
			var div = _createElement('div');
			_appendChild(_editorOptions.elementEditedDoc.body, div);
			//div.innerHTML = 'Hello';
			//div.style.backgroundColor = 'yellow';
			div.style.width = '100%';
			div.style.height = '100%';
			div.contentEditable = true;
			div.jsData = {};
			div.jsData.isTop = true;
			div.jsData.createSubEditor = function(p, c) {
				if (c == div) {
					var ret = {};
					ret.obj = div;
					ret.subEditor = true;
					ret.isTop = true;
					ret.allowCommands = true;
					ret.getProperties = function() {
						var ps = {};
						ps.props = new Array();
						ps.props.push(langProp);
						return ps;
					};
					ret.getString = function() {
						return 'Contents';
					}
					return ret;
				}
			}
			var ln = document.body.parentNode.getAttribute('lang');
			if (ln) {
				div.setAttribute('lang', ln);
			}
			_editorOptions.elementEdited = div;
			_editorOptions.selectedObject = null;
			selectEditElement(div);
			toggleShowHideProps();
		}
		function _setEditorTarget(editTarget) {
			if (!editTarget) {
				alert('edit target cannot be empty');
				return;
			}
			if (_editorOptions == editTarget) {
				_bringToFront();
			}
			else {
				_editorOptions = editTarget;
				if (!_editorOptions.redbox) {
					_editorOptions.redboxId = 'redbox889932';
					_editorOptions.redbox = _editorOptions.elementEditedDoc.getElementById(_editorOptions.redboxId);
					if (_editorOptions.redbox && !_editorOptions.redbox.parentNode) {
						_appendChild(_editorOptions.elementEditedDoc.body, _editorOptions.redbox);
					}
				}
				if (!_editorOptions.redbox) {
					_editorOptions.redbox = _createElement('img');
					_editorOptions.redbox.forProperties = true;
					_editorOptions.redbox.contentEditable = false;
					_editorOptions.redbox.id = _editorOptions.redboxId;
					_editorOptions.redbox.className = _editorOptions.redboxId;
					_editorOptions.redbox.style.height = '4px';
					_editorOptions.redbox.style.width = '4px';
					_editorOptions.redbox.src = '/libjs/redbox.png';
					_editorOptions.redbox.style.position = 'absolute';
					_appendChild(_editorOptions.elementEditedDoc.body, _editorOptions.redbox);
					if (_editorOptions.initLocation) {
						_divProp.style.left = _editorOptions.initLocation.x + 'px';
						_divProp.style.top = _editorOptions.initLocation.y + 'px';
						if (_editorOptions.initLocation.w) {
							_divProp.style.width = _editorOptions.initLocation.w + 'px';
						}
						if (_editorOptions.initLocation.h) {
							_divProp.style.height = _editorOptions.initLocation.h + 'px';
						}
					}
					if (_editorOptions.forIDE) {
						_imgOK.style.display = 'none';
						_imgCancel.style.display = 'none';
						_imgSave.style.display = 'none';
						_imgReset.style.display = 'none';
					}
					else {
						if (_editorOptions.hideOKbutton) {
							_imgOK.style.display = 'none';
						}
						else {
							_imgOK.style.display = 'inline';
						}
					}
					_initSelection();
				}
				else {
					var o = _editorOptions.selectedObject ? _editorOptions.selectedObject : _editorOptions.elementEdited;
					_editorOptions.selectedObject = null;
					selectEditElement(o);
				}
				if (_editorOptions.isEditingBody && _editorOptions.client) {
					if (_editorOptions.client.getPageHolder.apply(_editorOptions.editorWindow)) {
						var p = JsonDataBinding.ElementPosition.getElementPosition(_editorOptions.client.getPageHolder());
						if (p) {
							_divProp.style.left = (p.x + 80) + 'px';
							_divProp.style.top = (p.y + 60) + 'px';
						}
					}
				}
				if (!_divProp.expanded) {
					_togglePropertiesWindow();
				}
				_bringToFront();
				_showResizer();
				showFontCommands();
				adjustSizes();
				_divSelectElement.jsData.checkAdditables();
			}
		}
		function addUndoItem(undoitem) {
			if (!_editorOptions.undoList) {
				_editorOptions.undoList = new Array();
			}
			if (!_editorOptions.undoState) {
				_editorOptions.undoState = {};
			}
			if (_editorOptions.undoState.index == _editorOptions.undoList.length - 1) {
				if (!_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML) {
					_editorOptions.undoList[_editorOptions.undoState.index].redoInnerHTML = undoitem.redoInnerHTML;
				}
			}
			_editorOptions.undoList.push(undoitem);
			_editorOptions.undoState.index = _editorOptions.undoList.length - 1;
			_editorOptions.undoState.state = UNDOSTATE_REDO;
			showUndoRedo();
		}
		function _handleBackspace(fromClient) {
			if (fromClient) {
				if (_editorOptions) {
					if (_editorOptions.elementEditedDoc.activeElement && _editorOptions.elementEditedDoc.activeElement.type && _editorOptions.elementEditedDoc.activeElement.type.toLowerCase() == "text") {
						return true;
					}
					if (_editorOptions.selectedObject && _editorOptions.selectedObject.tagName) {
						var tag = _editorOptions.selectedObject.tagName.toLowerCase();
						if (tag != 'table') {
							return true;
						}
					}
				}
				return false;
			}
			else {
				if (!document.activeElement || !document.activeElement.type || document.activeElement.type.toLowerCase() != "text") {
					return false;
				}
			}
			return true;
		}
		function _onclientkeyup(obj) {
			//selectEditElement(obj);
			captureSelection({ target: obj });
			//if (!_editorOptions.isEditingBody) {
			//	_editorDiv.style.height = '100px'; //(_toolbarTable.offsetHeight + 3) + 'px';
			//}
		}
		function _setChanged() {
			_editorOptions.pageChanged = true;
		}
		function _onclientmouseup(obj, w) {
			if (_editorOptions) {
				if (_custMouseDown) {
					return;
				}
				if (w) {
					if (_editorOptions.getPageHolder()) {
						var ph = _editorOptions.getPageHolder();
						if (w != ph) {
							if (ph && ph.jsData && ph.jsData.onDocMouseUp) {
								ph.jsData.onDocMouseUp();
							}
						}
					}
				}
				captureSelection({ target: obj });
				mouseup({ target: obj, client: true });
				if (_editorOptions.isEditingBody && _editorOptions.elementEditedDoc.body != obj) {
					selectEditElement(obj);
				}
			}
		}
		function _onclientmousemove(x, y, w) {
			if (_editorOptions) {
				if (_editorOptions.styleChanged) {
					_editorOptions.styleChanged = false;
					if (_editorOptions.selectedObject) {
						var cssTD = getPropertyCell('cssText');
						if (cssTD) {
							var s = _editorOptions.selectedObject.style.cssText;
							if (s) {
								var txts = cssTD.getElementsByTagName('input');
								if (txts && txts.length > 0) {
									for (i = 0; i < txts.length; i++) {
										if (txts[i].type == 'text') {
											cssTD.value = s;
											txts[i].value = s;
											break;
										}
									}
								}
							}
						}
					}
				}
				//if (_editorOptions.getPageHolder) {
				//var pos = JsonDataBinding.ElementPosition.getElementPosition(_editorOptions.getPageHolder());
				//mousemove({ pageX: pos.x + x, pageY: pos.y + y });
				//}
			}
			if (w) {
				var pos = JsonDataBinding.ElementPosition.getElementPosition(w);
				if (_editorOptions && _editorOptions.getPageHolder()) {
					var ph = _editorOptions.getPageHolder();
					if (w != ph) {
						if (ph && ph.jsData && ph.jsData.onDocMouseMove) {
							var pos2 = JsonDataBinding.ElementPosition.getElementPosition(ph);
							ph.jsData.onDocMouseMove({ pageX: pos.x + x-pos2.x, pageY: pos.y + y-pos2.y });
						}
					}
				}
				mousemove({ pageX: pos.x + x, pageY: pos.y + y });
			}
		}
		function _onclientmousedown(obj,x,y, isRightClick) {
			if (_editorOptions) {
				if (_custMouseDown) {
					_custMouseDown({target:obj,x:x,y:y});
					return;
				}
				if (_isIE) {
					if (_editorOptions.forIDE) {
						var c = selectEditElement(obj);
						if (c && isRightClick) {
							var objStr = _getSelectedObject(c);
							if (typeof (limnorStudio) != 'undefined') limnorStudio.onRightClickElement(objStr, x, y);
							else window.external.OnRightClickElement(objStr, x, y);
						}
					}
				}
			}
		}
		function _initSelection() {
			if (!_editorOptions) return;
			_editorOptions.selectedObject = null;
			selectEditElement(_editorOptions.elementEdited);
			captureSelection({ target: _editorOptions.elementEdited });
			adjustSizes();
		}
		function _setUseMap(imgId, mapId) {
			var img = document.getElementById(imgId);
			if (img) {
				img.setAttribute('usemap', mapId);
				_editorOptions.pageChanged = true;
			}
		}
		function _setPropertyValue(propertyName, propertyValue) {
			if (_editorOptions.ideCurrentobj) {
				_editorOptions.ideCurrentobj[propertyName] = propertyValue;
				_editorOptions.pageChanged = true;
				showProperties(_editorOptions.ideCurrentobj);
				return 0;
			}
			else
				return 1;
		}
		//filePath is not null or empty, and trimmed
		function _appendArchiveFile(objId, filePath) {
			var obj;
			if (objId) {
				obj = _editorOptions.elementEditedDoc.getElementById(objId);
			}
			if (!obj) {
				obj = _editorOptions.ideCurrentobj;
			}
			if (obj) {
				obj.archive = obj.archive ? (obj.archive + ' ' + filePath) : filePath;
				showProperties(obj);
				return obj.archive
			}
			return '';
		}
		function _notifyUsedNames(usedNames) {
			_editorOptions.ideUsedNames = '';
			if (usedNames && usedNames.length > 0) {
				_editorOptions.ideUsedNames = usedNames.split(',');
			}
		}
		function _getChildElementIds(p) {
			var sRet = '';
			if (p) {
				if (p.id && p.id.length > 0) {
					sRet = p.id;
				}
				if (p.childNodes && p.childNodes.length > 0) {
					for (var i = 0; i < p.childNodes.length; i++) {
						var s = _getChildElementIds(p.childNodes[i]);
						if (s.length > 0) {
							if (sRet.length == 0) {
								sRet = s;
							}
							else {
								sRet += ',';
								sRet += s;
							}
						}
					}
				}
			}
			return sRet;
		}
		function _getNamesUsed() {
			return _getChildElementIds(document.body);
		}
		function showError(errorMsg) {
			var sp;
			if (_divError) {
				sp = _divError.sp;
			}
			else {
				_divError = document.createElement("div");
				_divError.style.backgroundColor = '#FFFFE0';
				_divError.style.position = 'absolute';
				_divError.style.border = "3px double yellow";
				_divError.style.color = 'red';
				appendToBody(_divError);
				var tbl = document.createElement("table");
				_divError.appendChild(tbl);
				tbl.border = 0;
				tbl.cellPadding = 0;
				tbl.cellSpacing = 0;
				tbl.width = '100%';
				tbl.height = '100%';
				var tblBody = JsonDataBinding.getTableBody(tbl);
				var tr = tblBody.insertRow(-1);
				td = document.createElement('td');
				tr.appendChild(td);
				td.align = 'center';
				sp = document.createElement('span');
				td.appendChild(sp);
				_divError.sp = sp;
				sp.style.color = 'red';
				sp.style.textAlign = 'center';
				sp.style.align = 'center';
				sp.style.verticalAlign = 'middle';
				tr.style.align = 'center';
				tr.style.verticalAlign = 'middle';
			}
			sp.innerHTML = errorMsg;
			_divError.style.width = '300px';
			_divError.style.height = '185px';
			var zi = JsonDataBinding.getPageZIndex(_divError) + 1;
			_divError.style.zIndex = zi;
			_divError.style.display = 'block';
			JsonDataBinding.anchorAlign.alignCenterElement(_divError);
			_divError.clicks = 1;
		}
		function showToolTips(element, tips) {
			if (!_divToolTips) {
				_divToolTips = document.createElement("div");
				_divToolTips.style.backgroundColor = '#FFFFE0';
				_divToolTips.style.position = 'absolute';
				_divToolTips.style.border = "1px double yellow";
				_divToolTips.style.color = 'blue';
				var sp = document.createElement('span');
				sp.style.backgroundColor = '#FFFF99';
				_divToolTips.appendChild(sp);
				_divToolTips.sp = sp;
				appendToBody(_divToolTips);
			}
			var zi = JsonDataBinding.getPageZIndex(_divToolTips) + 1;
			_divToolTips.style.zIndex = zi;
			_divToolTips.style.display = 'block';
			_divToolTips.clicks = 1;
			var pos = JsonDataBinding.ElementPosition.getElementPosition(element);
			_divToolTips.style.left = (pos.x - 20) + 'px';
			_divToolTips.style.top = (pos.y + 20) + 'px';
			_divToolTips.sp.innerHTML = '&nbsp;&nbsp;' + tips + '&nbsp;&nbsp;';
			_divToolTips.style.display = 'block';
		}
		function elementToString(e, typeName) {
			if (!e) return '';
			if (e.subEditor) {
				return e.getString();
			}
			var nm = '';
			if (isCssNode(e)) {
				nm = 'css file';
			}
			else {
				if (typeName)
					nm = typeName;
				else if (e.typename) {
					nm = e.typename;
				}
				else {
					if (e.getAttribute) {
						nm = e.getAttribute('typename');
					}
					if (!nm || nm.length == 0) {
						if (e.tagName) {
							var tag = e.tagName.toLowerCase();
							if (tag == 'input') {
								if (e.type) {
									nm = e.type;
								}
								else {
									nm = e.tagName;
								}
							}
							else {
								nm = e.tagName;
							}
						}
					}
				}
			}
			var eid = '';
			if (e.id) {
				if (nm == 'fileupload' && e.id.length > 1) {
					eid = e.id.substr(0, e.id.length - 1);
				}
				else
					eid = e.id;
			}
			var name = e.name;
			if (!name) {
				name = e.getAttribute('Name');
			}
			if (name) {
				if (eid.length > 0) {
					eid = eid + ',';
				}
				eid = eid + name;
			}
			name = e.getAttribute('styleName');
			if (name) {
				if (eid.length > 0) {
					eid = eid + ',';
				}
				eid = eid + name;
			}
			if (eid.length > 0) {
				return nm + '(' + eid + ')';
			}
			if (e.id) {
				return nm + '(' + e.id + ')';
			}
			return nm;
		}
		function setSingleValueAttr(o, name, val) {
			if (val && val != 'false') {
				o.setAttribute(name,name);
			}
			else {
				o.removeAttribute(name);
			}
		}
		function getSingleValueAttr(o, name) {
			return o.hasAttribute(name);
		}
		function getPercentAttr(o, name) {
			if (o && o.getAttribute) {
				var v = o.getAttribute(name);
				if (typeof (v) != 'undefined' && (v || v === 0)) {
					return Math.round(v);
				}
			}
		}
		function setPercentAttr(o, name, v) {
			if (o && o.setAttribute) { 
				if (v < 0) v = 0;
				if (v > 100) v = 100;
				o.setAttribute(name, v);
			}
		}
		function getAllIds() {
			var ids = [];
			if (_editorOptions) {
				var rid = _editorOptions.redbox ? _editorOptions.redbox.id : 'redbox889932';
				function getid(a) {
					if (a) {
						if (a.id && a.id != rid) {
							ids.push(a.id);
						}
						if (a.childNodes) {
							for (var i = 0; i < a.childNodes.length; i++) {
								getid(a.childNodes[i]);
							}
						}
					}
				}
				if (_editorOptions && _editorOptions.elementEditedDoc) {
					getid(_editorOptions.elementEditedDoc.body);
				}
			}
			return ids;
		}
		//==elements===========
		var langProp = {
			name: 'lang', editor: EDIT_ENUM, allowEdit: true, byAttribute: true,
			desc: 'the language of the element\'s content. Use ISO Language Codes.',
			values: function() {
				var ls = [
['Abkhazian', 'ab'],
['Afar', 'aa'],
['Afrikaans', 'af'],
['Albanian', 'sq'],
['Amharic', 'am'],
['Arabic', 'ar'],
['Aragonese', 'an'],
['Armenian', 'hy'],
['Assamese', 'as'],
['Aymara', 'ay'],
['Azerbaijani', 'az'],
['Bashkir', 'ba'],
['Basque', 'eu'],
['Bengali(Bangla)', 'bn'],
['Bhutani', 'dz'],
['Bihari', 'bh'],
['Bislama', 'bi'],
['Breton', 'br'],
['Bulgarian', 'bg'],
['Burmese', 'my'],
['Byelorussian(Belarusian)', 'be'],
['Cambodian', 'km'],
['Catalan', 'ca'],
['Chinese', 'zh'],
['Corsican', 'co'],
['Croatian', 'hr'],
['Czech', 'cs'],
['Danish', 'da'],
['English', 'en'],
['Esperanto', 'eo'],
['Estonian', 'et'],
['Faeroese', 'fo'],
['Farsi', 'fa'],
['Fiji', 'fj'],
['Finnish', 'fi'],
['French', 'fr'],
['Frisian', 'fy'],
['Galician', 'gl'],
['Gaelic(Scottish)', 'gd'],
['Gaelic(Manx)', 'gv'],
['Georgian', 'ka'],
['German', 'de'],
['Greek', 'el'],
['Greenlandic', 'kl'],
['Guarani', 'gn'],
['Gujarati', 'gu'],
['Haitian Creole', 'ht'],
['Hausa', 'ha'],
['Hebrew(he)', 'he'],
['Hebrew(iw)', 'iw'],
['Hindi', 'hi'],
['Hungarian', 'hu'],
['Icelandic', 'is'],
['Ido', 'io'],
['Indonesian(id)', 'id'],
['Indonesian(in)', 'in'],
['Interlingua', 'ia'],
['Interlingue', 'ie'],
['Inuktitut', 'iu'],
['Inupiak', 'ik'],
['Irish', 'ga'],
['Italian', 'it'],
['Japanese', 'ja'],
['Javanese', 'jv'],
['Kannada', 'kn'],
['Kashmiri', 'ks'],
['Kazakh', 'kk'],
['Kinyarwanda(Ruanda)', 'rw'],
['Kirghiz', 'ky'],
['Kirundi(Rundi)', 'rn'],
['Korean', 'ko'],
['Kurdish', 'ku'],
['Laothian', 'lo'],
['Latin', 'la'],
['Latvian(Lettish)', 'lv'],
['Limburgish(Limburger)', 'li'],
['Lingala', 'ln'],
['Lithuanian', 'lt'],
['Macedonian', 'mk'],
['Malagasy', 'mg'],
['Malay', 'ms'],
['Malayalam', 'ml'],
['Maltese', 'mt'],
['Maori', 'mi'],
['Marathi', 'mr'],
['Moldavian', 'mo'],
['Mongolian', 'mn'],
['Nauru', 'na'],
['Nepali', 'ne'],
['Norwegian', 'no'],
['Occitan', 'oc'],
['Oriya', 'or'],
['Oromo(Afaan Oromo)', 'om'],
['Pashto(Pushto)', 'ps'],
['Polish', 'pl'],
['Portuguese', 'pt'],
['Punjabi', 'pa'],
['Quechua', 'qu'],
['Rhaeto-Romance', 'rm'],
['Romanian', 'ro'],
['Russian', 'ru'],
['Samoan', 'sm'],
['Sangro', 'sg'],
['Sanskrit', 'sa'],
['Serbian', 'sr'],
['Serbo-Croatian', 'sh'],
['Sesotho', 'st'],
['Setswana', 'tn'],
['Shona', 'sn'],
['Sichuan Yi', 'ii'],
['Sindhi', 'sd'],
['Sinhalese', 'si'],
['Siswati', 'ss'],
['Slovak', 'sk'],
['Slovenian', 'sl'],
['Somali', 'so'],
['Spanish', 'es'],
['Sundanese', 'su'],
['Swahili(Kiswahili)', 'sw'],
['Swedish', 'sv'],
['Tagalog', 'tl'],
['Tajik', 'tg'],
['Tamil', 'ta'],
['Tatar', 'tt'],
['Telugu', 'te'],
['Thai', 'th'],
['Tibetan', 'bo'],
['Tigrinya', 'ti'],
['Tonga', 'to'],
['Tsonga', 'ts'],
['Turkish', 'tr'],
['Turkmen', 'tk'],
['Twi', 'tw'],
['Uighur', 'ug'],
['Ukrainian', 'uk'],
['Urdu', 'ur'],
['Uzbek', 'uz'],
['Vietnamese', 'vi'],
['Volapük', 'vo'],
['Wallon', 'wa'],
['Welsh', 'cy'],
['Wolof', 'wo'],
['Xhosa', 'xh'],
['Yiddish(yi)', 'yi'],
['Yiddish(ji)', 'ji'],
['Yoruba', 'yo'],
['Zulu', 'zu']
				];
				var ret = new Array();
				ret.push({ text: '', value: '' });
				for (var i = 0; i < ls.length; i++) {
					var s = ls[i];
					ret.push({ text: s[0], value: s[1] });
				}
				return ret;
			},
			onsetprop: function() {
				var head = _editorOptions.elementEditedDoc.getElementsByTagName('head')[0];
				var mts = head.getElementsByTagName('meta');
				var v4 = false;
				var v5 = false;
				if (mts) {
					for (var i = 0; i < mts.length; i++) {
						var hy = mts[i].getAttribute('http-equiv');
						if (hy == 'content-type') {
							mts[i].setAttribute('content', 'text/html; charset=utf-8');
							v4 = true;
						}
						//if (mts[i].hasAttribute('charset')) {
						//	mts[i].setAttribute('charset', 'UTF-8');
						//	v5 = true;
						//}
						hy = mts[i].getAttribute('charset');
						if (typeof (hy) != 'undefined' && hy != null) {
							mts[i].setAttribute('charset', 'UTF-8');
							v5 = true;
						}
						if (v4 && v5)
							break;
					}
				}
				var m;
				if (!v4) {
					m = _createElement('meta');
					_appendChild(head, m);
					m.setAttribute('http-equiv', 'content-type');
					m.setAttribute('content', 'text/html; charset=utf-8');
				}
				if (!v5) {
					m = _createElement('meta');
					_appendChild(head, m);
					m.setAttribute('charset', 'UTF-8');
				}
			}
		};
		var cssClass = {
			name: 'class', editor: EDIT_ENUM, allowEdit: true,
			values: function(o) {
				return _editorOptions.client.getClassNames.apply(_editorOptions.elementEditedWindow, [o]);
			},
			desc: 'one or more class names, separated by spaces, for specifying styles. Each class name refers to a class in a style sheet.',
			getter: function (o) {
				if (o.className && typeof(o.className.baseVal) != 'undefined')
					return o.className.baseVal;
				return o.className;
			},
			setter: function(o, val) {
				//_editorOptions.client.addClass.apply(_editorOptions.elementEditedWindow, [o, val]);
				if (o.className && typeof (o.className.baseVal) != 'undefined') {
					o.className.baseVal = val;
					return;
				}
				o.className = val;
			}
		};
		var idProp = {
			name: 'id', desc: 'a unique id for an element', editor: EDIT_TEXT, getter: function(o) { return o.subEditor ? o.obj.id : o.id; }, setter: function(o, val) {
				if (o.subEditor) o = o.obj;
				if (_divError) { _divError.style.display = 'none'; }
				if (o.id != val) {
					var g = o.getAttribute('limnorId');
					if (val == null || val.length == 0) {
						if (g) {
							showError('id for this element cannot be removed because it is used in programming.');
							return;
						}
					}
					if (val && val.length > 0) {
						if (val == 'body') {
							showError('"body" cannot be used as id.');
							return;
						}
						if (HtmlEditor.IsNameValid(val)) {
							var o0 = document.getElementById(val);
							if (o0) {
								showError('The id is in use.');
								return;
							}
						}
						else {
							showError('The id is invalid. Please only use alphanumeric characters. The first letter can only be a-z, A-Z, $, and underscore');
							return;
						}
					}
					o.id = val;
					if (_editorOptions.forIDE) {
						if (g) {
							if (typeof (limnorStudio) != 'undefined') limnorStudio.onElementIdChanged(g, val);
							else window.external.OnElementIdChanged(g, val);
						}
					}
				}
			}
		};
		var validateCmdProp = {
			name: 'validateUl', toolTips: 'validate and fix tree view structure errors', editor: EDIT_CMD, IMG: '/libjs/validate.png',
			action: function(e) {
				var c = JsonDataBinding.getSender(e);
				if (c && c.owner) {
					var obj = c.owner;
					if (obj) {
						var tag = obj.tagName ? obj.tagName.toLowerCase() : '';
						if (tag == 'ul') {
							var errCount = 0;
							function validateUL(ul) {
								var lastli, i, j;
								var invals = new Array();
								for (i = 0; i < ul.children.length; i++) {
									var c = ul.children[i];
									var tag0 = c.tagName.toLowerCase();
									if (tag0 == 'li') {
										for (j = 0; j < c.children.length; j++) {
											if (c.children[j].tagName.toLowerCase() == 'ul') {
												validateUL(c.children[j]);
											}
										}
										lastli = c;
									}
									else if (tag0 == 'ul') {
										var p = ul;
										if (lastli) {
											p = null;
											for (j = 0; j < lastli.children.length; j++) {
												if (lastli.children[j].tagName.toLowerCase() == 'ul') {
													p = lastli.children[j];
													break;
												}
											}
											if (!p) {
												p = _createElement('ul');
												_appendChild(lastli, p);
											}
										}
										var cs = new Array();
										for (j = 0; j < c.children.length; j++) {
											if (c.children[j].tagName.toLowerCase() == 'li') {
												cs.push(c.children[j]);
											}
										}
										for (j = 0; j < cs.length; j++) {
											_appendChild(p, cs[j]);
											validateUL(cs[j]);
										}
										invals.push(c);
									}
								}
								for (i = 0; i < invals.length; i++) {
									ul.removeChild(invals[i]);
									errCount++;
								}
							}
							validateUL(obj);
							if (errCount > 0) {
								alert('Errors fixed:' + errCount + '. ');
								while (obj) {
									if (obj.jsData && obj.jsData.resetState) {
										obj.jsData.resetState();
										break;
									}
									obj = obj.parentNode;
								}
								//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
							}
							else {
								alert('No error found');
							}
						}
					}
				}
			}
		};
		var propFontFamily = { name: 'fontFamily', desc: 'specifies the font for an element.', forStyle: true, cat: PROP_FONT, editor: EDIT_ENUM, values: fontList };
		var propFontSize = {
			name: 'fontSize', forStyle: true, cat: PROP_FONT, editor: EDIT_ENUM, values: fontSizes, allowEdit: true, canbepixel: true,
			onselectindexchanged:
			function (sel) {
				if (sel.selectedIndex) {
					var sv = sel.options[sel.selectedIndex].value;
					if (sv) {
						if (sv == '%' || sv == 'size in px, cm, ...') {
							return false;
						}
						return true;
					}
				}
				return false;
			},
			ontextchange:
			function (txt, sel) {
				if (txt && sel.selectedIndex) {
					var sv = sel.options[sel.selectedIndex].value;
					if (sv == 'size in px, cm, ...')
						return txt;
					if (sv == '%' && txt.length > 0)
						if (txt.substr(txt.length - 1, 1) == '%')
							return txt;
						else
							return txt;
				}
			}
		};
		//it is automatically added to elements	under general group
		var commonProperties = [
			idProp
		,
		{ name: 'accessKey', editor: EDIT_TEXT, desc: 'a shortcut key to activate/focus an element' },
		{ name: 'cursor', forStyle: true, editor: EDIT_ENUM, allowEdit: true, values: HtmlEditor.cursorValues, desc: 'specifies the type of cursor to be displayed when pointing on an element.' },
		{
			name: 'styleShare', desc: 'controls whether new style settings should be shared or not', editor: EDIT_ENUM, values: ['', 'Share', 'NotShare']
			, getter: function (o) {
				var s = o.getAttribute('styleshare');
				if (typeof s == 'undefined' || s == null || s.length == 0 || s.toLowerCase() != 'notshare') {
					return 'Share';
				}
				return 'NotShare';
			}
			, setter: function (o, s) {
				if (typeof s == 'undefined' || s == null || s.length == 0 || s.toLowerCase() != 'notshare') {
					o.removeAttribute('styleshare');
				}
				else {
					o.setAttribute('styleshare', 'NotShare');
				}
			}
		},
		{
			name: 'styleName', desc: 'Setting styleName to a non-empty string will add it as a class name to the element; all your modifications of the element styles will be added to the class and thus applied to all elements using the class.',
			byAttribute: true, editor: EDIT_ENUM, allowEdit: true,
			values: function(o) {
				return _editorOptions.client.getStyleNames.apply(_editorOptions.elementEditedWindow, [o]);
			},
			getter: function (o) {
				return o.getAttribute('styleName');
			},
			setter: function(o, v) {
				var curStyleName = o.getAttribute('styleName');
				if (v) {
					v = v.toLowerCase();
					o.setAttribute('styleName', v);
				}
				else {
					o.removeAttribute('styleName');
				}
				if (curStyleName && curStyleName.length > 0 && curStyleName != v) {
					_editorOptions.client.removeClass.apply(_editorOptions.elementEditedWindow, [o, curStyleName]);
				}
			},
			onsetprop: function(o, v) {
				if (v && v.length > 0) {
					v = v.toLowerCase();
					_editorOptions.client.addClass.apply(_editorOptions.elementEditedWindow, [o, v]);
				}
			}
		},
		cssClass,
		{ name: 'title', editor: EDIT_TEXT, desc: 'extra information about an element' }
		];
		var frameWidthProp = {
			name: 'fixedWidth', editor: EDIT_NUM, isPixel: true, desc: 'Specifies the width, in pixels, of an iframe element',
			getter: function (o) { return o.width; },
			setter: function (o, v) {
				o.style.width = '';
				JsonDataBinding.removeStyleAttribute(o, 'width');
				if (_editorOptions && _editorOptions.isEditingBody)
					_editorOptions.client.setElementStyleValue.apply(_editorOptions.elementEditedWindow, [o, 'width', 'width', null]);
				if (isNaN(v)) o.removeAttribute('width'); else o.width = v;
			}
		};
		var frameHeightProp = {
			name: 'fixedHeight', editor: EDIT_NUM, isPixel: true, desc: 'Specifies the height, in pixels, of an iframe element',
			getter: function (o) { return o.height; },
			setter: function (o, v) {
				o.style.height = '';
				JsonDataBinding.removeStyleAttribute(o, 'height');
				if (_editorOptions && _editorOptions.isEditingBody)
					_editorOptions.client.setElementStyleValue.apply(_editorOptions.elementEditedWindow, [o, 'height', 'height', null]);
				if (isNaN(v)) o.removeAttribute('height'); else o.height = v;
			}
		};
		var innerHtmlProp = {
			name: 'innerHtml', editor: EDIT_CMD, desc: 'Set innerHTML of the element', IMG: '/libjs/innerHTML.png', action: setElementInnerHTML
		};
		var verticalAlignProp = {
			name: 'verticalAlign', forStyle: true, editor: EDIT_ENUM, values: HtmlEditor.verticalAlign, allowEdit: true,
			onselectindexchanged:
				function (sel) {
					if (sel.selectedIndex) {
						var sv = sel.options[sel.selectedIndex].value;
						if (sv) {
							if (sv == '%' || sv == 'length in px') {
								return false;
							}
							return true;
						}
					}
					return false;
				},
			ontextchange:
				function (txt, sel) {
					if (txt && sel.selectedIndex) {
						var sv = sel.options[sel.selectedIndex].value;
						if (sv == 'length in px')
							return txt;
						if (sv == '%' && txt.length > 0)
							if (txt.substr(txt.length - 1, 1) == '%')
								return txt;
							else
								return txt;
					}
					return txt;
				}
		};
		var propFontStyle = { name: 'fontStyle', desc: 'Specifies the font style for a text.', forStyle: true, cat: PROP_FONT, editor: EDIT_ENUM, values: HtmlEditor.fontStyles };
		var propFontWeight = { name: 'fontWeight', desc: 'Sets how thick or thin characters in text should be displayed.', forStyle: true, cat: PROP_FONT, editor: EDIT_ENUM, values: HtmlEditor.fontWeights };
		var propFontVariant = { name: 'fontVariant', desc: 'Specifies whether or not a text should be displayed in a small-caps font. In a small-caps font, all lowercase letters are converted to uppercase letters. However, the converted uppercase letters appears in a smaller font size than the original uppercase letters in the text.', forStyle: true, cat: PROP_FONT, editor: EDIT_ENUM, values: HtmlEditor.fontVariants };
		var editor_font = [
			propFontFamily,
			propFontSize,
			propFontStyle,
			propFontWeight,
			propFontVariant,
			{ name: 'color', desc: 'Specifies text color', forStyle: true, cat: PROP_FONT, editor: EDIT_COLOR }
		];
		var editor_txt0 = [
			langProp,
			{ name: 'tabindex', editor: EDIT_NUM },
			{ name: 'backgroundColor', forStyle: true, editor: EDIT_COLOR },
			{ name: 'direction', forStyle: true, editor: EDIT_ENUM, values: ['', 'ltr', 'rtl', 'inherit'] },
			{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, desc:'font related properties' },
			{ name: 'text-decoration', editor: EDIT_ENUM, values: ['', 'none', 'underline', 'overline', 'line-through', 'blink', 'inherit'], cssName: 'text-decoration' }
		];
		var editor_txt = [
			{ name: 'txt', editor: EDIT_PROPS, editorList: editor_txt0, desc:'text related properties' },
			{ name: 'textAlign', editor: EDIT_ENUM, values: HtmlEditor.textAlign, cssName: 'text-align' }
		];
		//for h1 - h6
		var editor_heading = [
			{ name: 'txt', editor: EDIT_PROPS, editorList: editor_txt, desc: 'text related properties' }
		];
		var editor_common_txt = [
			{ name: 'txt', editor: EDIT_PROPS, editorList: editor_txt, desc: 'text related properties' }
		];
		var editor_rel = ['', 'alternate',
'appendix',
'bookmark',
'chapter',
'contents',
'copyright',
'glossary',
'help',
'home',
'index',
'next',
'prev',
'section',
'start',
'stylesheet',
'subsection'
		];
		var htmlsaveToProps = [
			{ name: 'folder name', editor: EDIT_TEXT, getter: function(o) { return _editorOptions.saveToFolder; }, setter: function(o, val) { _editorOptions.saveToFolder = val; } },
			{ name: 'filename', editor: EDIT_TEXT, getter: function(o) { return _editorOptions.saveTo; }, setter: function(o, val) { _editorOptions.saveTo = val; } }
		];
		var backgroundProps = [
			{ name: 'backgroundImage', forStyle: true, cat: PROP_BK, isUrl: true, maxSize: 1048576, filetypes: '.image', title: 'Select Image File', editor: EDIT_TEXT, desc: 'The background-image property sets one or more background images for an element.' },
			{ name: 'backgroundRepeat', forStyle: true, cat: PROP_BK, editor: EDIT_ENUM, values: ['', 'repeat', 'repeat-x', 'repeat-y', 'no-repeat', 'inherit'], desc: 'The background-repeat property sets if/how a background image will be repeated' },
			{
			name: 'backgroundPosition', forStyle: true, cat: PROP_BK, allowEdit: true, editor: EDIT_ENUM,
			values: ['', 'left top', 'left center', 'left bottom', 'right top', 'right center', 'right bottom', 'center top', 'center center', 'center bottom'], desc: 'The background-position property sets the starting position of a background image.'
			},
			{ name: 'backgroundColor', forStyle: true, cat: PROP_BK, editor: EDIT_COLOR, desc: 'The background-color property sets the background color of an element.' },
			{ name: 'backgroundAttachment', forStyle: true, cat: PROP_BK, editor: EDIT_ENUM, values: ['', 'scroll', 'fixed', 'inherit'], desc: 'The background-attachment property sets whether a background image is fixed or scrolls with the rest of the page.' },
			{ name: 'backgroundSize', forStyle: true, cat: PROP_BK, allowEdit: true, editor: EDIT_ENUM, values: ['', 'cover', 'contain'], desc: 'The background-size property specifies the size of the background images.' },
			{ name: 'backgroundClip', forStyle: true, cat: PROP_BK, editor: EDIT_ENUM, values: ['', 'border-box', 'padding-box', 'content-box'], desc: 'The background-clip property specifies the painting area of the background.' },
			{ name: 'backgroundOrigin', forStyle: true, cat: PROP_BK, editor: EDIT_ENUM, values: ['', 'padding-box', 'border-box', 'content-box'], desc: 'The background-origin property specifies what the background-position property should be relative to.' },
			{
				name: 'linearGradientStartColor', forStyle: true, cat: PROP_BK, editor: EDIT_COLOR, desc: 'The color for the starting point for forming linear gradient background. See https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient',
				getter: function(o) {
					return _editorOptions.client.getLinearGradientStartColor.apply(_editorOptions.editorWindow, [o]);
				}, setter: function(o, val) {
					_editorOptions.client.setLinearGradientStartColor.apply(_editorOptions.editorWindow, [o, val]);
				}
			},
			{
				name: 'linearGradientEndColor', forStyle: true, cat: PROP_BK, editor: EDIT_COLOR, desc: 'The color for the ending point for forming linear gradient background. See https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient',
				getter: function(o) {
					return _editorOptions.client.getLinearGradientEndColor.apply(_editorOptions.editorWindow, [o]);
				}, setter: function(o, val) {
					_editorOptions.client.setLinearGradientEndColor.apply(_editorOptions.editorWindow, [o, val]);
				}
			},
			{
				name: 'linearGradientAngle', forStyle: true, cat: PROP_BK, editor: EDIT_NUM, desc: 'The angle, in degree, for forming linear gradient background. See https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient',
				getter: function(o) {
					return _editorOptions.client.getLinearGradientAngle.apply(_editorOptions.editorWindow, [o]);
				}, setter: function(o, val) {
					_editorOptions.client.setLinearGradientAngle.apply(_editorOptions.editorWindow, [o, val]);
				}
			}
		];
		var borderProps = [
			{ name: 'borderColor', desc: ' Sets the color of the four borders', forStyle: true, cat: PROP_BORDER, editor: EDIT_COLOR },
			{ name: 'borderStyle', desc: ' Sets the style of the four borders', forStyle: true, cat: PROP_BORDER, editor: EDIT_ENUM, values: HtmlEditor.borderStyle },
			{ name: 'borderWidth', desc: ' Sets the width of the four borders. Select a width type or enter a width length, such as, 3px', forStyle: true, cat: PROP_BORDER, editor: EDIT_ENUM, values: HtmlEditor.widthStyle, canbepixel: true, allowEdit: true },
			{ name: 'outlineColor', desc: ' Sets the color of an outline', forStyle: true, cat: PROP_BORDER, editor: EDIT_COLOR },
			{ name: 'outlineStyle', desc: ' Sets the style of an outline', forStyle: true, cat: PROP_BORDER, editor: EDIT_ENUM, values: HtmlEditor.borderStyle },
			{ name: 'outlineWidth', desc: ' Sets the width of an outline. Select a width type or enter a width length, such as, 3px', forStyle: true, cat: PROP_BORDER, editor: EDIT_ENUM, values: HtmlEditor.widthStyle, canbepixel: true, allowEdit: true },
			{ name: 'borderImage', desc: ' A shorthand property for setting all the border-image-* properties', forStyle: true, cat: PROP_BORDER, editor: EDIT_TEXT },
			{ name: 'borderRadius', desc: ' A shorthand property for setting all the four border-*-radius properties.', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'boxShadow', desc: ' Attaches one or more drop-shadows to the box, by "horizontal vertical blur spread color inset", for example, 10px 10px 5px #888888. You may use the color picker to get color value.', forStyle: true, cat: PROP_BORDER, editor: EDIT_TEXT/*, getter: function (o) { return JsonDataBinding.getBoxShadow(o); }, setter: function (o, val) { JsonDataBinding.setBoxShadow(o, val); }*/ }
		];
		var boxDimProps = [
			{ name: 'overflow', desc: ' Specifies what happens if content overflows an element\'s box', forStyle: true, cat: PROP_POS, editor: EDIT_ENUM, values: ['', 'visible', 'hidden', 'scroll', 'auto', 'inherit'] },
			{ name: 'overflowX', desc: ' Specifies whether or not to clip the left/right edges of the content, if it overflows the element\'s content area', forStyle: true, cat: PROP_BOX, editor: EDIT_ENUM, values: HtmlEditor.overflow },
			{ name: 'overflowY', desc: ' Specifies whether or not to clip the top/bottom edges of the content, if it overflows the element\'s content area', forStyle: true, cat: PROP_BOX, editor: EDIT_ENUM, values: HtmlEditor.overflow },
			{ name: 'height', desc: ' Sets the height of an element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'maxHeight', desc: ' Sets the maximum height of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'maxWidth', desc: ' Sets the maximum width of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'minHeight', desc: ' Sets the minimum height of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'minWidth', desc: ' Sets the minimum width of an element. It can be none, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'width', desc: ' Sets the width of an element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100%', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT }
		];
		var multiColsProps = [
			{
				name: 'columnCount', cat: PROP_MULCOL, desc: 'the number of columns an element should be divided into.', editor: EDIT_TEXT,
				getter: function(o) {
					return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'count']);
				},
				setter: function(o, val) {
					_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'columnCount', 'count', val]);
				}
			},
			{
				name: 'columnWidth', cat: PROP_MULCOL, desc: 'width of the columns', editor: EDIT_TEXT, canbepixel: true,
				getter: function(o) {
					return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'width']);
				},
				setter: function(o, val) {
					_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'columnWidth', 'width', val]);
				}
			},
			{
				name: 'columnGap', cat: PROP_MULCOL, desc: 'gap between the columns', editor: EDIT_TEXT, isPixel: true,
				getter: function(o) {
					return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'gap']);
				},
				setter: function(o, val) {
					_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'columnGap', 'gap', val]);
				}
			},
			{
				name: 'columnRuleStyle', cat: PROP_MULCOL, desc: 'style of the rule between columns', editor: EDIT_ENUM, values: HtmlEditor.borderStyle,
				getter: function(o) {
					return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'rule-style']);
				},
				setter: function(o, val) {
					_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'columnRuleStyle', 'rule-style', val]);
				}
			},
			{
				name: 'columnRuleWidth', cat: PROP_MULCOL, desc: 'width of the rule between columns', editor: EDIT_TEXT, canbepixel: true,
				getter: function(o) {
					return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'rule-width']);
				},
				setter: function(o, val) {
					_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'columnRuleWidth', 'rule-width', val]);
				}
			},
			{
				name: 'columnRuleColor', cat: PROP_MULCOL, desc: 'color of the rule between columns', editor: EDIT_COLOR,
				getter: function(o) {
					return _editorOptions.client.getColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'rule-color']);
				},
				setter: function(o, val) {
					_editorOptions.client.setColumnProperty.apply(_editorOptions.elementEditedWindow, [o, 'columnRuleColor', 'rule-color', val]);
				}
			}
		];
		var sizelocProps = [
			{ name: 'clientLeft', editor: EDIT_NONE, desc: 'width of the left border in pixels. the distance between the offsetLeft property and the true left side of the client area' },
			{ name: 'clientTop', editor: EDIT_NONE, desc: 'height of the top border in pixels. the distance between the offsetTop property and the true top of the client area. ' },
			{ name: 'clientWidth', editor: EDIT_NONE, desc: 'the width of the visible area for an object, in pixels. the width of the object including padding, but not including margin, border, or scroll bar.' },
			{ name: 'clientHeight', editor: EDIT_NONE, desc: 'the height of the visible area for an object, in pixels. the height of the object including padding, but not including margin, border, or scroll bar. ' },
			{ name: 'offsetLeft', editor: EDIT_NONE, desc: 'the left position of an object relative to the left side of its offsetParent element, in pixels. the calculated left position of the object relative to the layout or coordinate parent, as specified by the offsetParent property.' },
			{ name: 'offsetTop', editor: EDIT_NONE, desc: 'the top position of the object relative to the top side of its offsetParent element, in pixels.' },
			{ name: 'offsetWidth', editor: EDIT_NONE, desc: 'the width of the visible area for an object, in pixels. The value contains the width with the padding, scrollBar, and the border, but does not include the margin.' },
			{ name: 'offsetHeight', editor: EDIT_NONE, desc: 'the height of the visible area for an object, in pixels. The value contains the height with the padding, scrollBar, and the border, but does not include the margin.' },
			{ name: 'scrollLeft', editor: EDIT_NONE, desc: 'the number of pixels by which the contents of an object are scrolled to the left.' },
			{ name: 'scrollTop', editor: EDIT_NONE, desc: 'the number of pixels by which the contents of an object are scrolled upward.' },
			{ name: 'scrollWidth', editor: EDIT_NONE, desc: 'the total width of an element\'s contents, in pixels. The value contains the width with the padding, but does not include the scrollBar, border, and the margin.' },
			{ name: 'scrollHeight', editor: EDIT_NONE, desc: 'the total height of an element\'s contents, in pixels. The value contains the height with the padding, but does not include the scrollBar, border, and the margin.' }
		];
		var marginDesc = 'margin can be specified in following ways: auto, inherit, or in percent, i.e. 1%, or in length value, i.e. 2px';
		var paddingDesc = 'padding can be specified by length, i.e. 2px, or in percent, i.e. 1%';
		var marginPaddingProps = [
			{ name: 'marginBottom', desc: marginDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'marginLeft', desc: marginDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'marginRight', desc: marginDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'marginTop', desc: marginDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'paddingBottom', desc: paddingDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'paddingLeft', desc: paddingDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'paddingRight', desc: paddingDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'paddingTop', desc: paddingDesc, cat: PROP_MARGIN, forStyle: true, canbepixel: true, editor: EDIT_TEXT }
		];
		var posProps = [
			{ name: 'clip', desc: ' Clips an absolutely positioned element. It can be auto, inherit, or rect(top, right, bottom, left), i.e. rect(0px,50px,50px,0px)', forStyle: true, cat: PROP_POS, editor: EDIT_TEXT },
			{ name: 'display', desc: ' Specifies how a certain HTML element should be displayed', forStyle: true, cat: PROP_POS, editor: EDIT_ENUM, values: ['', 'box', 'block', 'flex', 'inline', 'inline-block', 'inline-flex', 'inline-table', 'list-item', 'none', 'table', 'table-caption', 'table-cell', 'table-column', 'table-column-group', 'table-footer-group', 'table-header-group', 'table-row', 'table-row-group', 'inherit'] },
			{ name: 'visibility', desc: ' Specifies whether or not an element is visible', forStyle: true, cat: PROP_POS, editor: EDIT_ENUM, values: ['', 'visible', 'hidden', 'collapse', 'inherit'] },
			{ name: 'position', desc: ' Specifies the type of positioning method used for an element (static, relative, absolute or fixed)', forStyle: true, cat: PROP_POS, editor: EDIT_ENUM, values: ['', 'static', 'relative', 'absolute', 'fixed', 'inherit'] },
			{ name: 'clear', desc: ' Specifies which sides of an element where other floating elements are not allowed', forStyle: true, cat: PROP_POS, editor: EDIT_ENUM, values: ['', 'left', 'right', 'both', 'none', 'inherit'] },
			{ name: 'right', desc: ' Specifies the right position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'left', desc: ' Specifies the left position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'top', desc: ' Specifies the top position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_BORDER, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'bottom', desc: ' Specifies the bottom position of a positioned element. It can be auto, inherit, or a length value, i.e. 100px, or in percent of its container, i.e. 100% ', forStyle: true, cat: PROP_POS, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'zIndex', desc: ' Sets the stack order of a positioned element', forStyle: true, cat: PROP_POS, editor: EDIT_NUM },
			{ name: 'tabindex', desc: 'specifies the tab order of an element (when the "tab" button is used for navigating).', editor: EDIT_NUM },
			{ name: 'cssFloat', forStyle: true, editor: EDIT_ENUM, values: HtmlEditor.cssFloat, desc: 'sets or returns the horizontal alignment of an object.' },
			{ name: 'opacity', editor: EDIT_NUM, getter: function(o) { return _editorOptions.client.getOpacity.apply(_editorOptions.elementEditedWindow, [o]); }, setter: function(o, val) { _editorOptions.client.setOpacity.apply(_editorOptions.elementEditedWindow, [o, val]); }, desc: 'sets the opacity level for an element, in percentage of 0 to 100.' }
		];

		//formatting
		var textProps = [
			{ name: 'color', desc: ' Sets the color of text', forStyle: true, cat: PROP_TEXT, editor: EDIT_COLOR },
			langProp,
			{name: 'direction', desc: ' Specifies the text direction/writing direction', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'ltr', 'rtr', 'inherit'] },
			{ name: 'letterSpacing', desc: ' Increases or decreases the space between characters in a text. It can be normal, inherit, or in length such as 2px', forStyle: true, cat: PROP_TEXT, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'lineHeight', desc: ' Sets the line height. It can be normal or inherit, or a number that will be multiplied with the current font size to set the line height, or a fixed line height in px, pt, cm, etc., i.e. 1px, or a line height in percent of the current font size, i.e. 20%', forStyle: true, cat: PROP_TEXT, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'textAlign', desc: ' Specifies the horizontal alignment of text', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'left', 'right', 'center', 'justify', 'inherit'] },
			{ name: 'textDecoration', desc: ' Specifies the decoration added to text', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'none', 'underline', 'overline', 'line-through', 'blink', 'inherit'] },
			{ name: 'textIndent', desc: ' Specifies the indentation of the first line in a text-block. It can be a length defining a fixed indentation in px, pt, cm, em, etc., i.e. 5px, or be the indentation in % of the width of the parent element', forStyle: true, cat: PROP_TEXT, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'textTransform', desc: ' Controls the capitalization of text', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'none', 'capitalize', 'uppercase', 'lowercase', 'inherit'] },
			{ name: 'verticalAlign', desc: ' Sets the vertical alignment of an element. It can be a length which raises or lowers an element by the specified length, i.e. 2px, negative values are allowed;  it can be in a percent of the "line-height" property, i.e. 10%, which raises or lowers an element, negative values are allowed; ', forStyle: true, cat: PROP_TEXT, canbepixel: true, editor: EDIT_ENUM, allowEdit: true, values: ['', 'baseline', 'sub', 'super', 'top', 'text-top', 'middle', 'bottom', 'text-bottom', 'inherit'] },
			{ name: 'whiteSpace', desc: ' Specifies how white-space inside an element is handled ', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'normal', 'nowrap', 'pre', 'pre-line', 'pre-wrap', 'inherit'] },
			{ name: 'wordSpacing', desc: ' Increases or decreases the space between words in a tex. It can be normal or inherit, or in a length defining an extra space between words in px, pt, cm, em, etc., i.e. 3px. Negative values are allowed', forStyle: true, cat: PROP_TEXT, canbepixel: true, editor: EDIT_TEXT },
			{ name: 'textAlignLast', desc: ' Describes how the last line of a block or a line right before a forced line break is aligned when text-align is "justify"', forStyle: true, cat: PROP_TEXT, editor: EDIT_TEXT },
			{ name: 'textJustify', desc: ' Specifies the justification method used when text-align is "justify"', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'auto', 'inter-word', 'inter-ideograph', 'inter-cluster', 'distribute', 'kashida', 'none'] },
			{ name: 'textOverflow', desc: ' Specifies what should happen when text overflows the containing element. It can be clip to clip the text; or ellipsis to show ..., or specify a string to be displayed', forStyle: true, cat: PROP_TEXT, editor: EDIT_TEXT },
			{ name: 'textShadow', desc: ' Adds shadow to text. IE does not support it. It is specified as h v blur (optional) color (optional). h:The position of the horizontal shadow. Negative values are allowed. v:The position of the vertical shadow. Negative values are allowed. example:2px 2px #ff0000', forStyle: true, cat: PROP_TEXT, editor: EDIT_TEXT },
			{ name: 'wordBreak', desc: ' Specifies line breaking rules for non-CJK scripts. Not supported by Opera.', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'normal', 'break-all', 'hyphenate'] },
			{ name: 'wordWrap', desc: ' Allows long, unbreakable words to be broken and wrap to the next line ', forStyle: true, cat: PROP_TEXT, editor: EDIT_ENUM, values: ['', 'normal', 'break-word'] }
		];
		//SVG shape common elements
		var svgmleft = { name: 'moveleft', editor: EDIT_CMD, IMG: '/libjs/moveleft.png', action: moveshapeleft, toolTips: 'moves the shape to left. A text box on the right shows moving dustance for each mouse click.' };
		var svgmright = { name: 'moveright', editor: EDIT_CMD, IMG: '/libjs/moveright.png', action: moveshaperight, toolTips: 'moves the shape to right. A text box on the right shows moving dustance for each mouse click.' };
		var svgmup = { name: 'moveup', editor: EDIT_CMD, IMG: '/libjs/moveup.png', action: moveshapeup, toolTips: 'moves the shape up. A text box on the right shows moving dustance for each mouse click.' };
		var svgmdown = { name: 'movedown', editor: EDIT_CMD, IMG: '/libjs/movedown.png', action: moveshapedown, toolTips: 'moves the shape down. A text box on the right shows moving dustance for each mouse click.' };
		var svgmdist = {
			name: 'movedistance', editor: EDIT_CMD, isText: true,
			getter: function (o) {
				return HtmlEditor.svgshapemovegap;
			},
			setter: function (o, v) {
				if (v) {
					var n = parseInt(v);
					if (n > 0 && n < 300) {
						HtmlEditor.svgshapemovegap = n;
					}
				}
			}
		};
		var svgdup = {
			name: 'duplicate', editor: EDIT_CMD, IMG: '/libjs/copy.png', toolTips: 'duplicate this element',
			action: function (e) {
				var c = JsonDataBinding.getSender(e);
				if (c && c.owner) {
					_duplicateSvg(c.owner);
				}
			}
		};
		var svgshapeProps = [
			svgmleft,
			svgmright,
			svgmup,
			svgmdown,
			svgmdist,
			{
				name: 'bringToFront', editor: EDIT_CMD, IMG: '/libjs/bfront.png', toolTips: 'bring this element to front',
				action: function (e) {
					var c = JsonDataBinding.getSender(e);
					if (c && c.owner) {
						_bringSvgToFront(c.owner);
					}
				}
			},
			svgdup,
			{ name: 'stroke', editor: EDIT_COLOR, byAttribute: true },
			{ name: 'stroke-width', editor: EDIT_NUM, byAttribute: true, nottext:true },
			{
				name: 'mouseoveropacity', editor: EDIT_NUM, byAttribute: true, desc: 'Specify opacity to be used when mouse moves over the shape, in percent from 0 to 100.',
				getter: function (o) {
					return getPercentAttr(o, 'mouseoveropacity');
				},
				setter: function (o, v) {
					setPercentAttr(o, 'mouseoveropacity', v);
				}
			},
			{ name: 'mouseoverfillcolor', desc: 'Specify fill color to be used when mouse moves over the shape', editor: EDIT_COLOR, byAttribute: true,closedonly:true },
			{
				name: 'mouseoutopacity', editor: EDIT_NUM, byAttribute: true, desc: 'Specify opacity to be used when mouse moves oou of the shape, in percent from 0 to 100',
				getter: function (o) {
					return getPercentAttr(o, 'mouseoutopacity');
				},
				setter: function (o, v) {
					setPercentAttr(o, 'mouseoutopacity', v);
				}
			},
			{ name: 'mouseoutfillcolor', desc: 'Specify fill color to be used when mouse moves out of the shape', editor: EDIT_COLOR, byAttribute: true, closedonly: true },
			{
				name: 'showelements', editor: EDIT_ENUM, allowEdit: true, byAttribute: true, desc: 'Specify an id list, separated by comma. Aach id represents one element, on mouseover event the element is displayed, on mouseout event the element is hidden.',
				values: function () {
					var ret = new Array();
					ret.push({ text: '', value: '' });
					var ids = getAllIds();
					for (var i = 0; i < ids.length; i++) {
						ret.push({ text: ids[i], value: ids[i] });
					}
					return ret;
				},
				combosetter: function (o, v, inputElement) {
					if (v) {
						v = v.trim();
						if (v.length > 0) {
							var s;
							var a = o.getAttribute('showelements');
							if (a && a.length > 0) {
								var ss = a.split(',');
								for (var i = 0; i < ss.length; i++) {
									if (ss[i]) {
										if (v == ss[i].trim()) {
											return;
										}
									}
								}
								var s = '';
								for (var i = 0; i < ss.length; i++) {
									if (ss[i]) {
										var s0 = ss[i].trim();
										if (s0.length > 0) {
											if (s.length > 0) {
												s += ',';
											}
											s += s0;
										}
									}
								}
								s += ',';
								s += v;
							}
							else {
								s = v;
							}
							inputElement.value = s;
							o.setAttribute('showelements', s);
						}
					}
				}
			},
			{
				name: 'automouseouteffects', editor: EDIT_BOOL, desc: 'set this property to true so that mouseoutfillcolor and mouseoutopacity will be used on event of mouse out, and elements listed in showelements will be made invisible. If this attribute is not set or it is false then these effects will be used at a mouse click.', byAttribute: true
			},
			{
				name: 'href', editor: EDIT_TEXT, desc: 'hyper destination for the shape',
				getter: function (o) {
					var a = o.parentNode;
					if (a) {
						var s = a.getAttribute('href');
						if (s && s.baseVal)
							return s.baseVal;
						return s;
					}
				},
				setter: function (o, v) {
					var a = o.parentNode;
					if (a) {
						a.setAttribute('href',v);
					}
				}
			},
			{
				name: 'target', editor: EDIT_ENUM, values: ['', '_blank', '_self', '_top', '_parent'], desc: 'It specifies where to open the linked document. The linked document is specified by href.',
				getter: function (o) {
					var a = o.parentNode;
					if (a) {
						return a.getAttribute('target');
					}
				},
				setter: function (o, v) {
					var a = o.parentNode;
					if (a) {
						a.setAttribute('target', v);
					}
				}
			},
			{
				name: 'fill', forStyle: true, editor: EDIT_COLOR, closedonly: true
			},
			{
				name: 'initshape', editor: EDIT_CMD, isInit: true,
				act: function (o) {
					o.jsData = o.jsData || createSvgShape(o);
				}
			}
		];

		function getfileinput(form) {
			for (var i = 0; i < form.children.length; i++) {
				if (form.children[i].tagName.toLowerCase() == 'input') {
					var f = form.children[i].type;
					if (f && f.toLowerCase() == 'file') {
						return form.children[i];
					}
				}
				else {
					var d = getfileinput(form.children[i]);
					if (d)
						return d;
				}
			}
		}
		function getfilemaxsizeinput(form) {
			for (var i = 0; i < form.children.length; i++) {
				if (form.children[i].tagName.toLowerCase() == 'input' && form.children[i].getAttribute('name') == 'MAX_FILE_SIZE') {
					return form.children[i];
				}
				else {
					var d = getfilemaxsizeinput(form.children[i]);
					if (d) return d;
				}
			}
		}
		function getDefaultEditors() {
			return [
			{
				tagname: 'html', nodelete: true, properties: [
				{ name: 'head', editor: EDIT_GO, cmdText: 'show page head', action: gotoPageHead },
				{ name: 'PageURL', desc: 'Web address for the page as it is hosted on the current web server. You may copy it and give it to your web site visitors.', editor: EDIT_NONE, getter: function(o) { return getWebAddress(o); } },
				{
					name: 'docType', editor: EDIT_ENUM, values: [
						'<!DOCTYPE html>',
						'<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">',
						'<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">',
						'<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">',
						'<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN" "http://www.w3.org/TR/html4/frameset.dtd">',
						'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">',
						'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">',
						'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">'
						], allowEdit: true, IsDocType: true, desc: 'In HTML 4.01, the DOCTYPE declaration refers to a DTD, because HTML 4.01 was based on SGML. The DTD specifies the rules for the markup language, so that the browsers render the content correctly. HTML5 is not based on SGML, and therefore does not require a reference to a DTD; you may simply use "html" as the DOCTYPE for HTML5. Always add the DOCTYPE declaration to your HTML documents, so that the browser knows what type of document to expect.', setter: function(o, val) { _editorOptions.docType = val; }
				},
				{ name: 'title', desc: 'A title is required in all HTML documents and it defines the title of the document. Some usages of the HTML document title: defines a title in the browser toolbar; provides a title for the page when it is added to favorites; displays a title for the page in search-engine results; etc.', editor: EDIT_TEXT, getter: function(o) { return _editorOptions.elementEditedDoc.title; }, setter: function(o, val) { _editorOptions.elementEditedDoc.title = val; } },
				{ name: 'author', desc: 'Specifies the name of the author of the document.', editor: EDIT_TEXT, getter: function(o) { return getMetaData(o, 'author'); }, setter: function(o, val) { setMetaData(o, 'author', val); } },
				{ name: 'description', desc: 'Specifies a description of the page. Search engines can pick up this description to show with the results of searches.', editor: EDIT_TEXT, getter: function(o) { return getMetaData(o, 'description'); }, setter: function(o, val) { setMetaData(o, 'description', val); } },
				{ name: 'keywords', desc: 'Specifies a comma-separated list of keywords - relevant to the page (Informs search engines what the page is about). Always specify keywords (needed by search engines to catalogize the page).', editor: EDIT_TEXT, getter: function(o) { return getMetaData(o, 'keywords'); }, setter: function(o, val) { setMetaData(o, 'keywords', val); } },
				langProp,
				{ name: 'lastSave', desc: 'Time of the last saving or backup of the page to web server.', editor: EDIT_NONE, getter: function(o) { if (_editorOptions) return _editorOptions.lastsavetime ? _editorOptions.lastsavetime : (_editorOptions.useSavedUrl ? 're-loaded' : 'not saved'); } }
				]
			},
			{
				tagname: 'head', nodelete: true, properties: [
				{ name: 'addmeta', editor: EDIT_CMD, IMG: '/libjs/addmeta.png', action: addmeta, toolTips: 'add a new meta element to the page head' },
				{ name: 'addscript', editor: EDIT_CMD, IMG: '/libjs/addscript.png', action: addscript, toolTips: 'add a new script element to the page head' },
				{ name: 'addcss', editor: EDIT_CMD, IMG: '/libjs/addcss.png', action: addcss, toolTips: 'add a new CSS file to the page head' },
				{ name: 'addlink', editor: EDIT_CMD, IMG: '/libjs/addlink.png', action: addlink, toolTips: 'add a new link to the page head' },
				{ name: 'meta', desc: 'Metadata is data (information) about data. The "meta" tag provides metadata about the HTML document. Metadata will not be displayed on the page, but will be machine parsable. Meta elements are typically used to specify page description, keywords, author of the document, last modified, and other metadata. The metadata can be used by browsers (how to display content or reload page), search engines (keywords), or other web services. You may specify common meta data, author, description and keywords, by selecting HTML object.', editor: EDIT_NODES, sig: function(o) { var ct = o.getAttribute('content'); var nm = o.getAttribute('name'); if (nm) { return nm + ':' + (ct ? ct : ''); } else { var hr = o.getAttribute('http-equiv'); return (hr ? hr : '?') + ':' + (ct ? ct : '') } } },
				{ name: 'script', desc: 'The "script" tag is used to define a client-side script, such as a JavaScript. The "script" element either contains scripting statements, or it points to an external script file through the src attribute. Common uses for JavaScript are image manipulation, form validation, and dynamic changes of content.', editor: EDIT_NODES, sig: 'src', isScript: true },
				{ name: 'css', desc: 'Link CSS files to the HTML document so that the elements in the HTML may re-use the styles defined in the CSS files.', editor: EDIT_NODES, sig: 'href' },
				{ name: 'link', desc: 'The "link" tag defines the relationship between a document and an external resource. The "link" tag is mostly used to link to style sheets. So, most likely you do not need to use "link"; you may use "css" instead.', editor: EDIT_NODES, sig: function(o) { var f = o.getAttribute('href'); var rel = o.getAttribute('rel'); if (rel && rel == 'stylesheet') { return 'css file:' + f; } else { return 'external file:' + f; } } },
				{
					name: 'page classes', editor: EDIT_PROPS,
					editorList: function(o) {
						var props = [];
						var cl = _editorOptions.client.getPagesClasses.apply(_editorOptions.elementEditedWindow);
						for (var i = 0; i < cl.length; i++) {
							props.push({
								name: cl[i].selector, editor: EDIT_NONE,
								getter: function(v) { return function() { return v; }; } (cl[i].cssText),
								action: function(e) {
									var c = JsonDataBinding.getSender(e);
									if (c && c.propDesc) {
										if (confirm('Do you want to remove the styles identified by ' + c.propDesc.name + '? ')) {
											_editorOptions.client.removePageStyleBySelector.apply(_editorOptions.elementEditedWindow, [c.propDesc.name]);
											if (c.ownerTd) {
												var tr = c.ownerTd.parentNode;
												if (tr) {
													tr.style.display = 'none';
												}
											}
											_editorOptions.pageChanged = true;
										}
									}
								},
								notModify:true,
								IMG: '/libjs/cancel.png',
								toolTips: 'Each style was created when you edit element properties. Click the cancel button to delete the corresponding styles if you are sure the styles are no longer needed, i.e. all elements using the styles are removed.'
							});
						}
						return props;
					},
					cat: PROP_PAGECLASSES
				}
			]
			},
			{
				tagname: 'link', nodelete: true, properties: [
				{ name: 'delete', editor: EDIT_DEL, desc: 'delete the file link' },
				{ name: 'href', editor: EDIT_TEXT, byAttribute: true, isFilePath: true, maxSize: 1024, desc: 'the external file to be lined', title: 'Select External File', filetypes: '.href' },
				{ name: 'rel', editor: EDIT_ENUM, values: editor_rel, byAttribute: true },
				{ name: 'type', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'hreflang', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'rev', editor: EDIT_ENUM, values: editor_rel, byAttribute: true },
				{ name: 'target', editor: EDIT_ENUM, values: ['', '_blank', '_self', '_top', '_parent'], byAttribute: true },
				{ name: 'media', editor: EDIT_ENUM, values: ['', 'all', 'braille', 'print', 'projection', 'screen', 'speech'], byAttribute: true },
				{ name: 'charset', editor: EDIT_TEXT, byAttribute: true }
				]
			},
			{
				tagname: 'meta', nodelete: true, properties: [
				{ name: 'delete', editor: EDIT_DEL },
				{ name: 'charset', byAttribute: true, desc: 'Specifies the character encoding for the HTML document. Common values:UTF-8 - Character encoding for Unicode;ISO-8859-1 - Character encoding for the Latin alphabet. In theory, any character encoding can be used, but no browser understands all of them. The more widely a character encoding is used, the better the chance that a browser will understand it. To view all available character encodings, look at <a href="http://www.iana.org/assignments/character-sets" target=_new>IANA character sets</a>.', editor: EDIT_TEXT },
				{ name: 'content', byAttribute: true, desc: 'Gives the value associated with the http-equiv or name attribute.', editor: EDIT_TEXT },
				{ name: 'name', byAttribute: true, desc: 'Specifies a name for the metadata', editor: EDIT_ENUM, values: ['', /*'author', 'description', 'keywords',*/'application-name', 'revised'], allowEdit: true },
				{ name: 'http-equiv', byAttribute: true, desc: 'Provides an HTTP header for the information/value of the content attribute.', editor: EDIT_ENUM, values: ['', 'content-script-type', 'default-style', 'refresh', 'expires', 'set-cookie'], allowEdit: true }
				]
			},
			{
				tagname: 'script', nodelete: true, properties: [
				{ name: 'delete', editor: EDIT_DEL },
				{ name: 'type', byAttribute: true, editor: EDIT_ENUM, values: ['', 'text/javascript', 'text/ecmascript', 'application/ecmascript', 'application/javascript', 'text/vbscript'], allowEdit: true },
				{ name: 'src', byAttribute: true, editor: EDIT_TEXT, isFilePath: true, maxSize: 10240, title: 'Select Script File', filetypes: 'js' },
				{ name: 'charset', byAttribute: true, editor: EDIT_ENUM, values: ['', 'ISO-8859-1', 'UTF-8'], allowEdit: true },
				{ name: 'defer', byAttribute: true, editor: EDIT_ENUM, values: ['', 'defer'] },
				{ name: 'async', byAttribute: true, editor: EDIT_ENUM, values: ['', 'async'] }
				]
			},
			{
				tagname: 'body', nodelete: true, nomoveout: true, noCust: true, properties: [
				cssClass,
				{ name: 'cursor', forStyle: true, editor: EDIT_ENUM, allowEdit: true, values: HtmlEditor.cursorValues, desc: 'specifies the type of cursor to be displayed when pointing on an element.' },
				{ name: 'maps', editor: EDIT_CHILD, childTag: 'map', desc: 'maps contained in the page' },
				{ name: 'drawings', editor: EDIT_CHILD, childTag: 'svg', desc: 'svg elements contained in the page' },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT,desc:'font related properties' },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'columns', editor: EDIT_PROPS, editorList: multiColsProps, cat: PROP_MULCOL }
				]
			},
			{
				tagname: 'table', nounformat: true, nodelete: false, properties: [
				{ name: 'thead', editor: EDIT_GO, IMG: '/libjs/tableHead.png', toolTips: 'show table head properties', notcreate: true, showCommand: function(o) { var hs = o.getElementsByTagName('thead'); return (hs && hs.length > 0); } },
				{ name: 'tfoot', editor: EDIT_GO, IMG: '/libjs/tableFoot.png', toolTips: 'show table foot properties', notcreate: true, showCommand: function(o) { var hs = o.getElementsByTagName('tfoot'); return (hs && hs.length > 0); } },
				{ name: 'addHeader', editor: EDIT_CMD, IMG: '/libjs/addthead.png', toolTips: 'add table head', action: addtheader, showCommand: function(o) { var hs = o.getElementsByTagName('thead'); return (!hs || hs.length == 0); } },
				{ name: 'addFooter', editor: EDIT_CMD, IMG: '/libjs/addtfoot.png', toolTips: 'add table foot', action: addtfooter, showCommand: function(o) { var hs = o.getElementsByTagName('tfoot'); return (!hs || hs.length == 0); } },
				{ name: 'border', editor: EDIT_NUM, desc: 'The value "1" indicates borders should be displayed, and that the table is NOT being used for layout purposes.' },
				{ name: 'cellpadding', editor: EDIT_NUM, byAttribute: true, desc: 'specifies the space, in pixels, between the cell wall and the cell content.' },
				{ name: 'cellspacing', editor: EDIT_NUM, byAttribute: true, desc: 'specifies the space, in pixels, between cells.' },
				{ name: 'rowCount', editor: EDIT_NONE, getter: function (o) { return o.rows ? o.rows.length : 0; }},
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'border style', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: createTable
			},
			{
				tagname: 'col', nodelete: true, nomoveout: true, nounformat: true, noCust: true, properties: [
				{ name: 'cellIndex', editor: EDIT_NONE, getter: function(o) { return o.cellIndex; } },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				]
			},
			{
				tagname: 'td', nodelete: true, nomoveout: true, nounformat: true, noCust: true, properties: [
				{ name: 'colAttrs', editor: EDIT_CMD, IMG: '/libjs/html_col.png', toolTips: 'modify column attributes', action: showcolumnattrs },
				{ name: 'newRowAbove', editor: EDIT_CMD, IMG: '/libjs/newAbove.png', toolTips: 'add a new row above this row', action: addRowAbove },
				{ name: 'newRowBelow', editor: EDIT_CMD, IMG: '/libjs/newBelow.png', toolTips: 'add a new row below this row', action: addRowBelow },
				{ name: 'newColumnLeft', editor: EDIT_CMD, IMG: '/libjs/newLeft.png', toolTips: 'add a new column on left of this column', action: addColumnLeft },
				{ name: 'newColumnRight', editor: EDIT_CMD, IMG: '/libjs/newRight.png', toolTips: 'add a new column on right of this column', action: addColumnRight },
				{ name: 'mergeColumnLeft', editor: EDIT_CMD, IMG: '/libjs/mergeLeft.png', toolTips: 'merge this cell with the cell on its left side', action: mergeColumnLeft },
				{ name: 'mergeColumnRight', editor: EDIT_CMD, IMG: '/libjs/mergeRight.png', toolTips: 'merge this cell with the cell on its right side', action: mergeColumnRight },
				{ name: 'mergeColumnAbove', editor: EDIT_CMD, IMG: '/libjs/mergeAbove.png', toolTips: 'merge this cell with the cell above it', action: mergeColumnAbove },
				{ name: 'mergeColumnBelow', editor: EDIT_CMD, IMG: '/libjs/mergeBelow.png', toolTips: 'merge this cell with the cell below it', action: mergeColumnBelow },
				{ name: 'splitColumnH', editor: EDIT_CMD, IMG: '/libjs/splitH.png', toolTips: 'split this cell by creating a new cell on right side of this cell', action: splitColumnH },
				{ name: 'splitColumnV', editor: EDIT_CMD, IMG: '/libjs/splitV.png', toolTips: 'split this cell by creating a new cell below this cell', action: splitColumnV },
				{ name: 'cellIndex', editor: EDIT_NONE, desc: 'Column index. 0: the first column; 1: the second column; and so on' },
				{ name: 'rowIndex', editor: EDIT_NONE, desc: 'Row index. 0: the first row; 1: the second row; and so on', getter: function(o) { return o.parentNode.rowIndex; } },
				{ name: 'colSpan', editor: EDIT_NONE, desc: 'defines the number of columns a cell should span' },
				{ name: 'rowSpan', editor: EDIT_NONE, desc: 'defines the number of rows a cell should span' },
				{
					name: 'width', editor: EDIT_TEXT, canbepixel: true, desc: 'defines the width of the current cell. This setting will not be shared. To remove this setting, set it to empty.',
					getter: function (o) { return o.style.width; },
					setter: function (o, v) {
						if (typeof v == 'undefined' || v == null || v.length == 0) {
							JsonDataBinding.removeStyleAttribute(o, 'width');
						}
						else {
							try {
								o.style.width = v;
							}
							catch (err) {
							}
						}
					}
				},
				{
					name: 'height', editor: EDIT_TEXT, canbepixel: true, desc: 'defines the height of the current cell. This setting will not be shared. To remove this setting, set it to empty.',
					getter: function (o) { return o.style.height; },
					setter: function (o, v) {
						if (typeof v == 'undefined' || v == null || v.length == 0) {
							JsonDataBinding.removeStyleAttribute(o, 'height');
						}
						else {
							try {
								o.style.height = v;
							}
							catch (err) {
							}
						}
					}
				},
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'tr', nodelete: true, nomoveout: true, nounformat: true, noCust: true, properties: [
				{ name: 'delRow', editor: EDIT_DEL, toolTips: 'remove this row', action: removeTableRow },
				{ name: 'mergeRowAbove', editor: EDIT_CMD, IMG: '/libjs/mergeAbove.png', toolTips: 'merge this row with the row above it', action: mergeRowAbove },
				{ name: 'mergeRowBelow', editor: EDIT_CMD, IMG: '/libjs/mergeBelow.png', toolTips: 'merge this row with the row below it', action: mergeRowBelow },
				{ name: 'newRowAbove', editor: EDIT_CMD, IMG: '/libjs/newAbove.png', toolTips: 'add a new row above this row', action: addRowAbove },
				{ name: 'newRowBelow', editor: EDIT_CMD, IMG: '/libjs/newBelow.png', toolTips: 'add a new row below this row', action: addRowBelow },
				{ name: 'rowIndex', editor: EDIT_NONE },
				{ name: 'cellCount', editor: EDIT_NONE, getter: function (o) { return o.cells ? o.cells.length : 0; } },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'tbody', nodelete: true, nomoveout: true, nounformat: true, noCust: true, properties: [
				{ name: 'rowCount', editor: EDIT_NONE, getter: function (o) { return o.rows ? o.rows.length : 0; }},
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				]
			},
			{
				tagname: 'th', nodelete: true, nomoveout: true, nounformat: true, noCust: true, properties: [
				{ name: 'newRowAbove', editor: EDIT_CMD, IMG: '/libjs/newAbove.png', toolTips: 'add a new row above this row', action: addRowAbove },
				{ name: 'newRowBelow', editor: EDIT_CMD, IMG: '/libjs/newBelow.png', toolTips: 'add a new row below this row', action: addRowBelow },
				{ name: 'newColumnLeft', editor: EDIT_CMD, IMG: '/libjs/newLeft.png', toolTips: 'add a new column on left of this column', action: addColumnLeft },
				{ name: 'newColumnRight', editor: EDIT_CMD, IMG: '/libjs/newRight.png', toolTips: 'add a new column on right of this column', action: addColumnRight },
				{ name: 'mergeColumnLeft', editor: EDIT_CMD, IMG: '/libjs/mergeLeft.png', toolTips: 'merge this cell with the cell on its left side', action: mergeColumnLeft },
				{ name: 'mergeColumnRight', editor: EDIT_CMD, IMG: '/libjs/mergeRight.png', toolTips: 'merge this cell with the cell on its right side', action: mergeColumnRight },
				{ name: 'mergeColumnAbove', editor: EDIT_CMD, IMG: '/libjs/mergeAbove.png', toolTips: 'merge this cell with the cell above it', action: mergeColumnAbove },
				{ name: 'mergeColumnBelow', editor: EDIT_CMD, IMG: '/libjs/mergeBelow.png', toolTips: 'merge this cell with the cell below it', action: mergeColumnBelow },
				{ name: 'splitColumnH', editor: EDIT_CMD, IMG: '/libjs/splitH.png', toolTips: 'split this cell by creating a new cell on right side of this cell', action: splitColumnH },
				{ name: 'splitColumnV', editor: EDIT_CMD, IMG: '/libjs/splitV.png', toolTips: 'split this cell by creating a new cell below this cell', action: splitColumnV },
				{ name: 'cellIndex', editor: EDIT_NONE, desc: 'column index' },
				{ name: 'rowIndex', editor: EDIT_NONE, getter: function(o) { return o.parentNode.rowIndex; }, desc: 'row index' },
				{ name: 'colSpan', editor: EDIT_NUM, desc: 'number of columns a header cell should span' },
				{ name: 'rowSpan', editor: EDIT_NUM, desc: 'number of rows a header cell should span' },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'thead', nodelete: true, nomoveout: true, nounformat: true, noCust: true, properties: [
				{ name: 'delete', editor: EDIT_DEL },
				{ name: 'rowCount', editor: EDIT_NONE, getter: function (o) { return o.rows ? o.rows.length : 0; }},
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'tfoot', nodelete: true, nomoveout: true, nounformat: true, properties: [
				{ name: 'delete', editor: EDIT_DEL },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'p', properties: [
				{ name: 'columns', editor: EDIT_PROPS, editorList: multiColsProps, cat: PROP_MULCOL },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('p'); obj.innerHTML = 'new paragraph'; return obj; }
			},
			{
				tagname: 'font', properties: [
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font }
				]
			},
			{
				tagname: 'b', properties: [
				]
			},
			{
				tagname: 'strong', properties: [
				]
			},
			{
				tagname: 'em', properties: [
				]
			},
			{
				tagname: 'i', properties: [
				]
			},
			{
				tagname: 'u', properties: [
				]
			},
			{
				tagname: 'strike', properties: [
				]
			},
			{
				tagname: 'sub', properties: [
				]
			},
			{
				tagname: 'sup', properties: [
				]
			},
			{
				tagname: 'br', properties: [
				]
			},
			{
				tagname: 'div', properties: [
				{ name: 'columns', editor: EDIT_PROPS, editorList: multiColsProps, cat: PROP_MULCOL },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('div'); obj.innerHTML = 'new div'; return obj; }
			},
			{
				tagname: 'a', properties: [
				{
					name: 'href', editor: EDIT_TEXT, desc: 'the URL of the page the link goes to', isFilePath: true, title: 'Target Web Page', filetypes: '.href', disableUpload: true, getter: function (o) { return o.savedhref ? o.savedhref : ''; },
					setter: function (o, val) {
						o.savedhref = val;
						if (val && val.length > 0) {
							o.removeAttribute('youtubeID');
							_updateYoutubeTarget(o);
						}
					}
				},
				{ name: 'hreflang', editor: EDIT_TEXT, desc: 'the language of the linked document' },
				{ name: 'name', editor: EDIT_TEXT, desc: 'Not supported in HTML5. Specifies the name of an anchor' },
				{ name: 'rel', editor: EDIT_TEXT, desc: 'relationship between the current document and the linked document' },
				{ name: 'rev', editor: EDIT_TEXT, desc: 'Not supported in HTML5. the relationship between the linked document and the current document' },
				{
					name: 'target', bodyOnly: true, editor: EDIT_ENUM, values: ['', '_blank', '_parent', '_self', '_top'], allowEdit: true, desc: 'where to open the linked document', getter: function(o) { return o.savedtarget ? o.savedtarget : ''; }, setter: function(o, val) { o.savedtarget = val; },
					values: function() {
						var ret = new Array();
						ret.push({ text: '', value: '' });
						ret.push({ text: '_blank', value: '_blank' });
						ret.push({ text: '_parent', value: '_parent' });
						ret.push({ text: '_self', value: '_self' });
						ret.push({ text: '_top', value: '_top' });
						var ifs = _editorOptions.elementEditedDoc.getElementsByTagName('iframe');
						if (ifs && ifs.length > 0) {
							for (var i = 0; i < ifs.length; i++) {
								var nm = ifs[i].name;
								if (typeof nm != 'undefined') {
									nm = nm.trim();
									if (nm.length > 0) {
										ret.push({ text: nm, value: nm });
									}
								}
							}
						}
						return ret;
					}
				},
				{ name: 'shape', editor: EDIT_ENUM, values: ['', 'default', 'rect', 'circle', 'poly'], desc: 'Not supported in HTML5. the shape of a link' },
				{ name: 'media', editor: EDIT_TEXT, desc: 'HTML 5. what media/device the linked document is optimized for' },
				{ name: 'type', editor: EDIT_TEXT, desc: 'HTML 5. MIME type of the linked document' },
				{
					name: 'youtubePlayer', editor: EDIT_ENUM, desc: 'youtube player on the web page to be used to play video. youtubeID specifies the video to be played',
					values: function (o) {
						var ret = [{ text: '', value: '' }];
						var ifs = _editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow, ['iframe']);
						if (ifs) {
							for (var i = 0; i < ifs.length; i++) {
								if (ifs[i].typename == 'youtube'||ifs[i].getAttribute('typename') == 'youtube') {
									if (!ifs[i].id || ifs[i].id == '') {
										ifs[i].id = _createId(ifs[i]);
									}
									ret.push({ text: ifs[i].id, value: ifs[i].id });
								}
							}
						}
						return ret;
					},
					getter: function (o) {
						return o.getAttribute('youtube');
					},
					setter: function (o, v) {
						if (!v || v.length == 0) {
							o.removeAttribute('youtube');
						}
						else {
							o.setAttribute('youtube', v);
						}
						_updateYoutubeTarget(o);
					}
				},
				{
					name: 'youtubeID', editor: EDIT_TEXT, desc: 'ID of a youtube video to be played',
					getter: function (o) {
						return o.getAttribute('youtubeID');
					},
					setter: function (o, v) {
						if (!v || v == '') {
							o.removeAttribute('youtubeID');
						}
						else {
							o.setAttribute('youtubeID', getyoutubeid(v));
							var yid = o.getAttribute('youtube');
							if (!yid || yid == '') {
								var ifs = _editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow, ['iframe']);
								if (ifs) {
									for (var i = 0; i < ifs.length; i++) {
										if (ifs[i].typename=='youtube'||ifs[i].getAttribute('typename') == 'youtube') {
											if (!ifs[i].id || ifs[i].id == '') {
												ifs[i].id = _createId(ifs[i]);
											}
											o.setAttribute('youtube', ifs[i].id);
											break;
										}
									}
								}
							}
						}
						_updateYoutubeTarget(o);
					}
				},
				{
					name: 'mediaPlayer', editor: EDIT_ENUM, desc: 'a flash or music player on the web page to be used to play flash video or music. "mediaFile" property specifies the flash or music to be played',
					values: function (o) {
						var ret = [{ text: '', value: '' }];
						var ifs = _editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow, ['embed']);
						if (ifs) {
							var s = o.getAttribute('mediaFile');
							var t = 3;
							if (s && s.length > 0) {
								if (JsonDataBinding.endsWithI(s, '.swf')) {
									t = 1;
								}
								else {
									t=2;
								}
							}
							for (var i = 0; i < ifs.length; i++) {
								var icl = false;
								if (t == 1) {
									if (ifs[i].typename == 'flash' || ifs[i].getAttribute('typename') == 'flash') {
										icl = true;
									}
								}
								else if (t == 2) {
									if (ifs[i].typename == 'music' || ifs[i].getAttribute('typename') == 'music') {
										icl = true;
									}
								}
								else {
									if (ifs[i].typename == 'music' || ifs[i].getAttribute('typename') == 'music' || ifs[i].typename == 'flash' || ifs[i].getAttribute('typename') == 'flash') {
										icl = true;
									}
								}
								if(icl){
									if (!ifs[i].id || ifs[i].id == '') {
										ifs[i].id = _createId(ifs[i]);
									}
									ret.push({ text: ifs[i].id, value: ifs[i].id });
								}
							}
						}
						return ret;
					},
					getter: function (o) {
						return o.getAttribute('mediaPlayer');
					},
					setter: function (o, v) {
						if (!v || v.length == 0) {
							o.removeAttribute('mediaPlayer');
						}
						else {
							o.setAttribute('mediaPlayer', v);
						}
						_updateMediaTarget(o);
					}
				},
				{
					name: 'mediaFile', editor: EDIT_TEXT, desc: 'it can be an URL for a flash file (*.swf) or a music file (*.mp3) to be played. mediaPlayer property indicates which media player on the web page should be used.',
					getter: function (o) {
						return o.getAttribute('mediaFile');
					},
					setter: function (o, v) {
						if (!v || v == '') {
							o.removeAttribute('mediaFile');
						}
						else {
							o.setAttribute('mediaFile', v);
							var yid = o.getAttribute('mediaPlayer');
							if (!yid || yid == '') {
								var t = 3;
								if (JsonDataBinding.endsWithI(v, '.swf')) {
									t = 1;
								}
								else {
									t = 2;
								}
								var ifs = _editorOptions.client.getElementsByTagName.apply(_editorOptions.editorWindow, ['embed']);
								if (ifs) {
									for (var i = 0; i < ifs.length; i++) {
										if (t==1 && ifs[i].typename == 'flash' || ifs[i].getAttribute('typename') == 'flash') {
											if (!ifs[i].id || ifs[i].id == '') {
												ifs[i].id = _createId(ifs[i]);
											}
											o.setAttribute('mediaPlayer', ifs[i].id);
											break;
										}
										else if (t == 1 && ifs[i].typename == 'music' || ifs[i].getAttribute('typename') == 'music') {
											if (!ifs[i].id || ifs[i].id == '') {
												ifs[i].id = _createId(ifs[i]);
											}
											o.setAttribute('mediaPlayer', ifs[i].id);
											break;
										}
									}
								}
							}
						}
						_updateMediaTarget(o);
					}
				},
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('a'); obj.innerHTML = 'new hyperlink'; if (!_editorOptions.isEditingBody) obj.target = '_blank'; return obj; }
			},
			{
				tagname: 'span', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('span'); obj.innerHTML = 'new formatted text'; return obj; }
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'readonly', editor: EDIT_ENUM, values: ['', 'readonly'], byAttribute: true },
				{ name: 'maxlength', editor: EDIT_NUM },
				{ name: 'size', editor: EDIT_NUM },
				{ name: 'width', editor: EDIT_TEXT, forStyle: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'text',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'text'; obj.value = 'new input'; return obj; }
			},
			{
				tagname: 'button', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'width', editor: EDIT_TEXT, forStyle: true },
				{ name: 'type', editor: EDIT_ENUM, values: ['', 'button', 'reset', 'submit'] },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() {
					var obj = _editorOptions.elementEditedDoc.createElement('button');
					obj.innerHTML = 'new button';
					return obj;
				}
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'checked', editor: EDIT_ENUM, values: ['', 'checked'], byAttribute: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'checkbox',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'checkbox'; obj.value = 'new check box'; return obj; }
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'checked', editor: EDIT_ENUM, values: ['', 'checked'], byAttribute: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'radio',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'radio'; obj.value = 'new radio button'; return obj; }
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'readonly', editor: EDIT_ENUM, values: ['', 'readonly'], byAttribute: true },
				{ name: 'maxlength', editor: EDIT_NUM },
				{ name: 'size', editor: EDIT_NUM },
				{ name: 'width', editor: EDIT_TEXT, forStyle: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'password',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'password'; obj.value = 'password'; return obj; }
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'readonly', editor: EDIT_ENUM, values: ['', 'readonly'], byAttribute: true },
				{ name: 'maxlength', editor: EDIT_NUM },
				{ name: 'size', editor: EDIT_NUM },
				{ name: 'width', editor: EDIT_TEXT, forStyle: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'file',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'file'; obj.value = 'new file upload'; return obj; }
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'width', editor: EDIT_TEXT, forStyle: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'reset',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'reset'; obj.value = 'reset button'; return obj; }
			},
			{
				tagname: 'input', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, desc: 'display text', byAttribute: true },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'Specifies that an input element should be disabled', byAttribute: true },
				{ name: 'width', editor: EDIT_TEXT, forStyle: true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				type: 'submit',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'submit'; obj.value = 'submit button'; return obj; }
			},
			{
				tagname: 'input', properties: [
				idProp,
				{ name: 'delete', desc: 'delete the element', editor: EDIT_DEL },
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, desc: 'value to be sent to server', byAttribute: true }
				],
				type: 'hidden',
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('input'); obj.type = 'hidden'; obj.value = ''; return obj; }
			},
			{
				tagname: 'img', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'src', editor: EDIT_TEXT, isFilePath: true, maxSize: 1048576, desc: 'the URL of an image', title: 'Select Image File', filetypes: '.image' },
				{ name: 'alt', editor: EDIT_TEXT, desc: 'an alternate text for an image', byAttribute: true },
				{ name: 'longdesc', editor: EDIT_TEXT, desc: ' Not supported in HTML5.  the URL to a document that contains a long description of an image ' },
				{ name: 'height', editor: EDIT_TEXT, desc: 'height of an image', byAttribute: true },
				{ name: 'width', editor: EDIT_TEXT, desc: 'width of an image', byAttribute: true },
				{ name: 'ismap', editor: EDIT_ENUM, values: ['', 'ismap'], desc: 'an image as a server-side image-map' },
				{ name: 'usemap', editor: EDIT_TEXT, desc: 'an image as a client-side image-map' },
				{ name: 'tabindex', editor: EDIT_NUM },
				verticalAlignProp,
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'border style', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('img'); obj.src = '/libjs/html_img.png'; obj.removeAttribute('width'); obj.removeAttribute('height'); return obj; }
			},
			{
				tagname: 'ul', properties: [
				validateCmdProp,
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('ul'); var li = _editorOptions.elementEditedDoc.createElement('li'); li.innerHTML = 'item 1'; obj.appendChild(li); return obj; }
			},
			{
				tagname: 'menu', desc: 'The menu element is <font color=red>deprecated</font> in HTML 4.01 and redefined in HTML5. The menu tag is used to create a list of menu choices.', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('menu'); var li = _editorOptions.elementEditedDoc.createElement('li'); li.innerHTML = 'item 1'; obj.appendChild(li); return obj; }
			},
			{
				tagname: 'ol', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('ol'); var li = _editorOptions.elementEditedDoc.createElement('li'); li.innerHTML = 'item 1'; obj.appendChild(li); return obj; }
			},
			{
				tagname: 'li', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				]
			},
			{
				tagname: 'dl', properties: [
				{ name: 'newDefinition', editor: EDIT_CMD, IMG: '/libjs/newDefinition.png', action: addDefinition },
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('dl'); var dt = _editorOptions.elementEditedDoc.createElement('dt'); dt.innerHTML = 'item 1'; obj.appendChild(dt); var dd = _editorOptions.elementEditedDoc.createElement('dd'); dd.innerHTML = 'description of item 1'; obj.appendChild(dd); return obj; }
			},
			{
				tagname: 'dt', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				]
			},
			{
				tagname: 'dd', properties: [
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				]
			},
			{
				tagname: 'textarea', properties: [
				{ name: 'autofocus', editor: EDIT_ENUM, values: ['', 'autofocus'], desc: 'HTML5. the element should automatically get focus when the page loads' },
				{ name: 'form', editor: EDIT_TEXT, desc: 'HTML5. one or more forms the element belongs to' },
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'value', editor: EDIT_TEXT, desc: 'text inside the area' },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'] },
				{ name: 'readonly', editor: EDIT_ENUM, values: ['', 'readonly'] },
				{ name: 'cols', editor: EDIT_NUM, desc: 'the visible width of a text area' },
				{ name: 'rows', editor: EDIT_NUM, desc: 'the visible number of lines in a text area' },
				{ name: 'size', editor: EDIT_NUM },
				{ name: 'maxlength', editor: EDIT_NUM, desc: 'HTML 5. the maximum number of characters allowed in the text area' },
				{ name: 'placeholder', editor: EDIT_TEXT, desc: 'HTML 5. a short hint that describes the expected value of a text area' },
				{ name: 'required', editor: EDIT_ENUM, values: ['', 'required'], desc: 'HTML 5.  a text area is required/must be filled out' },
				{ name: 'wrap', editor: EDIT_ENUM, values: ['', 'hard', 'soft'], desc: 'HTML 5.  how the text in a text area is to be wrapped when submitted in a form' },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'select', properties: [
				{ name: 'newOptionGroup', editor: EDIT_CMD, IMG: '/libjs/newOptionGroup.png', action: addOptionGroup },
				{ name: 'newOption', editor: EDIT_CMD, IMG: '/libjs/newDefinition.png', action: addOption },
				{ name: 'options', editor: EDIT_CMD, createCommand: showListItems },
				{ name: 'groups', editor: EDIT_CHILD, childTag: 'optgroup' },
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'form', editor: EDIT_TEXT, desc: 'HTML5. one or more forms the select field belongs to' },
				{ name: 'size', editor: EDIT_NUM, desc: 'the number of visible options in a drop-down list' },
				{ name: 'multiple', editor: EDIT_BOOL, desc: 'multiple options can be selected at once' },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'a drop-down list should be disabled' },
				{ name: 'autofocus', editor: EDIT_ENUM, values: ['', 'autofocus'], desc: 'HTML5. the drop-down list should automatically get focus when the page loads' },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('select'); if (_isFireFox) obj.contentEditable = false; return obj; }
			},
			{
				tagname: 'option', properties: [
				{ name: 'newOption', editor: EDIT_CMD, IMG: '/libjs/newDefinition.png', action: addOption },
				{ name: 'options', editor: EDIT_CMD, createCommand: showListItems },
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'text', editor: EDIT_TEXT, desc: 'display text' },
				{ name: 'value', editor: EDIT_TEXT, desc: 'the value to be sent to a server' },
				{ name: 'label', editor: EDIT_TEXT, desc: 'a shorter label for an option' },
				{ name: 'disabled', editor: EDIT_ENUM, values: ['', 'disabled'], desc: 'an option should be disabled' },
				{ name: 'selected', editor: EDIT_ENUM, values: ['', 'selected'], desc: 'an option should be pre-selected when the page loads' },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				]
			},
			{
				tagname: 'hr', properties: [
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				]
			},
			{
				tagname: 'h1', properties: [
				{ name: 'h2', editor: EDIT_CMD, IMG: '/libjs/html_h2.png', action: function(e) { changeHeading(e, 'h2'); } },
				{ name: 'h3', editor: EDIT_CMD, IMG: '/libjs/html_h3.png', action: function(e) { changeHeading(e, 'h3'); } },
				{ name: 'h4', editor: EDIT_CMD, IMG: '/libjs/html_h4.png', action: function(e) { changeHeading(e, 'h4'); } },
				{ name: 'h5', editor: EDIT_CMD, IMG: '/libjs/html_h5.png', action: function(e) { changeHeading(e, 'h5'); } },
				{ name: 'h6', editor: EDIT_CMD, IMG: '/libjs/html_h6.png', action: function(e) { changeHeading(e, 'h6'); } },
				{ name: 'heading', editor: EDIT_PROPS, editorList: editor_heading }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('h1'); obj.innerHTML = 'new heading'; return obj; }
			},
			{
				tagname: 'h2', properties: [
				{ name: 'h1', editor: EDIT_CMD, IMG: '/libjs/html_h1.png', action: function(e) { changeHeading(e, 'h1'); } },
				{ name: 'h3', editor: EDIT_CMD, IMG: '/libjs/html_h3.png', action: function(e) { changeHeading(e, 'h3'); } },
				{ name: 'h4', editor: EDIT_CMD, IMG: '/libjs/html_h4.png', action: function(e) { changeHeading(e, 'h4'); } },
				{ name: 'h5', editor: EDIT_CMD, IMG: '/libjs/html_h5.png', action: function(e) { changeHeading(e, 'h5'); } },
				{ name: 'h6', editor: EDIT_CMD, IMG: '/libjs/html_h6.png', action: function(e) { changeHeading(e, 'h6'); } },
				{ name: 'heading', editor: EDIT_PROPS, editorList: editor_heading }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('h2'); obj.innerHTML = 'new heading'; return obj; }
			},
			{
				tagname: 'h3', properties: [
				{ name: 'h1', editor: EDIT_CMD, IMG: '/libjs/html_h1.png', action: function(e) { changeHeading(e, 'h1'); } },
				{ name: 'h2', editor: EDIT_CMD, IMG: '/libjs/html_h2.png', action: function(e) { changeHeading(e, 'h2'); } },
				{ name: 'h4', editor: EDIT_CMD, IMG: '/libjs/html_h4.png', action: function(e) { changeHeading(e, 'h4'); } },
				{ name: 'h5', editor: EDIT_CMD, IMG: '/libjs/html_h5.png', action: function(e) { changeHeading(e, 'h5'); } },
				{ name: 'h6', editor: EDIT_CMD, IMG: '/libjs/html_h6.png', action: function(e) { changeHeading(e, 'h6'); } },
				{ name: 'heading', editor: EDIT_PROPS, editorList: editor_heading }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('h3'); obj.innerHTML = 'new heading'; return obj; }
			},
			{
				tagname: 'h4', properties: [
				{ name: 'h1', editor: EDIT_CMD, IMG: '/libjs/html_h1.png', action: function(e) { changeHeading(e, 'h1'); } },
				{ name: 'h2', editor: EDIT_CMD, IMG: '/libjs/html_h2.png', action: function(e) { changeHeading(e, 'h2'); } },
				{ name: 'h3', editor: EDIT_CMD, IMG: '/libjs/html_h3.png', action: function(e) { changeHeading(e, 'h3'); } },
				{ name: 'h5', editor: EDIT_CMD, IMG: '/libjs/html_h5.png', action: function(e) { changeHeading(e, 'h5'); } },
				{ name: 'h6', editor: EDIT_CMD, IMG: '/libjs/html_h6.png', action: function(e) { changeHeading(e, 'h6'); } },
				{ name: 'heading', editor: EDIT_PROPS, editorList: editor_heading }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('h4'); obj.innerHTML = 'new heading'; return obj; }
			},
			{
				tagname: 'h5', properties: [
				{ name: 'h1', editor: EDIT_CMD, IMG: '/libjs/html_h1.png', action: function(e) { changeHeading(e, 'h1'); } },
				{ name: 'h2', editor: EDIT_CMD, IMG: '/libjs/html_h2.png', action: function(e) { changeHeading(e, 'h2'); } },
				{ name: 'h3', editor: EDIT_CMD, IMG: '/libjs/html_h3.png', action: function(e) { changeHeading(e, 'h3'); } },
				{ name: 'h4', editor: EDIT_CMD, IMG: '/libjs/html_h4.png', action: function(e) { changeHeading(e, 'h4'); } },
				{ name: 'h6', editor: EDIT_CMD, IMG: '/libjs/html_h6.png', action: function(e) { changeHeading(e, 'h6'); } },
				{ name: 'heading', editor: EDIT_PROPS, editorList: editor_heading }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('h5'); obj.innerHTML = 'new heading'; return obj; }
			},
			{
				tagname: 'h6', properties: [
				{ name: 'h1', editor: EDIT_CMD, IMG: '/libjs/html_h1.png', action: function(e) { changeHeading(e, 'h1'); } },
				{ name: 'h2', editor: EDIT_CMD, IMG: '/libjs/html_h2.png', action: function(e) { changeHeading(e, 'h2'); } },
				{ name: 'h3', editor: EDIT_CMD, IMG: '/libjs/html_h3.png', action: function(e) { changeHeading(e, 'h3'); } },
				{ name: 'h4', editor: EDIT_CMD, IMG: '/libjs/html_h4.png', action: function(e) { changeHeading(e, 'h4'); } },
				{ name: 'h5', editor: EDIT_CMD, IMG: '/libjs/html_h5.png', action: function(e) { changeHeading(e, 'h5'); } },
				{ name: 'heading', editor: EDIT_PROPS, editorList: editor_heading }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('h6'); obj.innerHTML = 'new heading'; return obj; }
			},
			{
				tagname: 'label', properties: [
				{
					name: 'htmlFor', editor: EDIT_ENUM, allowEdit: true, desc: 'Specifies which element a label is bound to',
					values: function () {
						var ret = new Array();
						ret.push({ text: '', value: '' });
						function getIdArray(e) {
							if (e.childNodes && e.childNodes.length) {
								for (var i = 0; i < e.childNodes.length; i++) {
									if (e.childNodes[i].id && e.childNodes[i].id.length > 0) {
										ret.push({ text: elementToString(e.childNodes[i]), value: e.childNodes[i].id });
									}
									getIdArray(e.childNodes[i]);
								}
							}
						}
						getIdArray(_editorOptions.elementEditedDoc.body);
						return ret;
					}
				},
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'border style', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('label'); obj.innerHTML = 'new label'; return obj; }
			},
			{
				tagname: 'address', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('address'); obj.innerHTML = 'address'; return obj; }
			},
			{
				tagname: 'map', properties: [
				{ name: 'addArea', editor: EDIT_CMD, IMG: '/libjs/addarea.png', action: addMapArea },
				{ name: 'name', editor: EDIT_TEXT, desc: 'Required. Specifies the name of an image-map' },
				{ name: 'areas', editor: EDIT_CHILD, childTag: 'area' },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				]
			},
			{
				tagname: 'area', properties: [
				{ name: 'alt', editor: EDIT_TEXT, desc: 'Specifies an alternate text for the area. Required if the href attribute is present' },
				{
					name: 'showmarkers', editor: EDIT_CMD, isInit:true, act: function (o) {
						o.jsData = o.jsData || function () {
							var left = 0, top = 0, right = 0, bottom = 0;
							var x = 0, y = 0, r = 0;
							var points = [];
							var coords;
							var shape;
							function parseCoords() {
								coords = o.getAttribute('coords');
								shape = o.getAttribute('shape');
								if (coords) {
									var nums = coords.split(',');
									if (shape == 'rest') {
										if (nums.length > 0) {
											left = nums[0];
											if (nums.length > 1) {
												top = nums[1];
												if (nums.length > 2) {
													right = nums[2];
													if (nums.length > 3) {
														bottom = nums[3];
													}
												}
											}
										}
									}
									else if (shape == 'circle') {
										if (nums.length > 0) {
											x = nums[0];
											if (nums.length > 1) {
												y = nums[1];
												if (nums.length > 2) {
													r = nums[2];
												}
											}
										}
									}
									else if (shape == 'poly') {
										points = [];
										var i = 0;
										while (i < nums.length) {
											var pt = {};
											pt.x = parseInt(nums[i]);
											i++;
											if (i < nums.length) {
												pt.y = parseInt(nums[i]);
												i++;
											}
											else
												pt.y = 0;
											pt.idx = points.push(pt) - 1;
										}
									}
								}
							}
							function _showmarkers() {
								if (_editorOptions.markers) {
									for (var k = 0; k < _editorOptions.markers.length; k++) {
										if (_editorOptions.markers[k]) {
											var p = _editorOptions.markers[k].parentNode;
											if (p) {
												p.removeChild(_editorOptions.markers[k]);
											}
										}
									}
									delete _editorOptions.markers;
								}
								coords = o.getAttribute('coords');
								shape = o.getAttribute('shape');
								if (shape == 'rest') {
								}
								else if (shape == 'circle') {
								}
								else if (shape == 'poly') {
									if (coords) {
										var svg = _createSVGElement('svg');
										_appendChild(_editorOptions.elementEditedDoc.body, svg);
										svg.style.position = 'absolute';
										svg.style.top = '0px';
										svg.style.left = '0px';
										svg.style.opacity = '0.2';
										var polygon = _createSVGElement('polygon');
										svg.appendChild(polygon);
										polygon.setAttribute('points', coords);
										polygon.style.fill = 'yellow';
										_editorOptions.markers = [];
										_editorOptions.markers.push(svg);
									}
								}
							}
							function _getProperties() {
								parseCoords();
								var props = [];
								if (shape == 'rest') {
									function setCoordsRect() {
										coords = left + ',' + top + ',' + right + ',' + bottom;
										o.setAttribute('coords', coords);
										if (o.jsData.showmarkers)
											o.jsData.showmarkers();
									}
									props.push({
										name: 'left', editor: EDIT_NUM, getter: function (e) { return left; },
										setter: function (e, val) {
											left = val;
											setCoordsRect();
										}
									});
									props.push({
										name: 'top', editor: EDIT_NUM, getter: function (e) { return top; },
										setter: function (e, val) {
											top = val;
											setCoordsRect();
										}
									});
									props.push({
										name: 'right', editor: EDIT_NUM, getter: function (e) { return right; },
										setter: function (e, val) {
											right = val;
											setCoordsRect();
										}
									});
									props.push({
										name: 'bottom', editor: EDIT_NUM, getter: function (e) { return bottom; },
										setter: function (e, val) {
											bottom = val;
											setCoordsRect();
										}
									});
								}
								else if (shape == 'circle') {
									function setCoordsCircle() {
										coords = x + ',' + y + ',' + r;
										o.setAttribute('coords', coords);
										if (o.jsData.showmarkers)
											o.jsData.showmarkers();
									}
									props.push({
										name: 'x', editor: EDIT_NUM, getter: function (e) { return x; },
										setter: function (e, val) {
											x = val;
											setCoordsCircle();
										}
									});
									props.push({
										name: 'y', editor: EDIT_NUM, getter: function (e) { return y; },
										setter: function (e, val) {
											y = val;
											setCoordsCircle();
										}
									});
									props.push({
										name: 'radius', editor: EDIT_NUM, getter: function (e) { return r; },
										setter: function (e, val) {
											r = val;
											setCoordsCircle();
										}
									});
								}
								else if (shape == 'poly') {
									function getterPoint(idx, obj) {
										return function (owner) {
											if (idx >= 0 && idx < points.length) {
												return points[idx].x + ',' + points[idx].y;
											}
										}
									}
									function setterPoint(idx, obj) {
										return function (owner, val) {
											var ss;
											if (val && val.length > 0) {
												ss = val.split(',');
												if (ss.length > 0) {
													var p;
													if (idx >= 0 && idx < points.length) {
														p = points[idx];
													}
													else {
														p = {};
														p.idx = idx;
														points.push(p);
													}
													p.x = parseInt(ss[0]);
													if (ss.length > 1) {
														p.y = parseInt(ss[1]);
													}
												}
											}
											else {
												if (idx >= 0 && idx < points.length) {
													p = points[idx];
													p.deleted = true;
												}
											}
											ss = '';
											var k = 0;
											for (var k = 0; k < points.length; k++) {
												if (!points[k].deleted) {
													if (ss.length > 0) {
														ss += ',';
													}
													ss += (points[k].x + ',' + points[k].y);
												}
											}
											o.setAttribute('coords', ss);
											if (o.jsData.showmarkers)
												o.jsData.showmarkers();
										}
									}
									function createPointProp(idx, needGrow) {
										var p = {
											name: 'point ' + idx, editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'a vertex point of a poly shape, in format of x,y',
											getter: getterPoint(idx, o),
											setter: setterPoint(idx, o)
										};
										if (needGrow) {
											p.needGrow = true;
											p.onsetprop = function (o, v) {
												_showmarkers();
												if (this.grow) {
													//do not grow if empty
													if (v && v.length > 0) {
														this.grow(createPointProp(idx + 1, true), 1);
														this.grow = null;
													}
												}
											};
										}
										return p;
									}
									if (points) {
										for (var i = 0; i < points.length; i++) {
											props.push(createPointProp(i, false));
										}
										props.push(createPointProp(points.length, true));
									}
									else {
										props.push(createPointProp(0, true));
									}
								}
								function onsetprop(curObj, v) {
									_showmarkers();
								}
								function onclientmousedown(e) {
									if (_custMouseDownOwner && _custMouseDownOwner.ownertextbox) {
										e = e || window.event;
										if (e) {
											var x = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x);
											var y = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y);
											_custMouseDownOwner.ownertextbox.value = x + ',' + y;
										}
									}
								}
								function oneditprop(e) {
									var img = JsonDataBinding.getSender(e);
									if (img) {
										if (_custMouseDown && _custMouseDownOwner != img) {
											if (_custMouseDownOwner) {
												_custMouseDownOwner.src = 'libjs/propedit.png';
												img.active = false;
											}
										}
										img.active = !img.active;
										if (img.active) {
											img.src = 'libjs/propeditAct.png';
											_custMouseDown = onclientmousedown;
											_custMouseDownOwner = img;
										}
										else {
											img.src = 'libjs/propedit.png';
											_custMouseDown = null;
											_custMouseDownOwner = null;
										}
									}
								}
								for (var i = 0; i < props.length; i++) {
									if (!props[i].onsetprop) {
										props[i].onsetprop = onsetprop;
									}
									props[i].propEditor = oneditprop;
								}
								return props;
							}
							parseCoords();
							return {
								showmarkers: function () {
									_showmarkers();
								},
								getProperties: function () {
									return _getProperties();
								}
							};
						}();
						o.jsData.showmarkers();
					}
				},
				{
					name: 'coords', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the coordinates of the area', cat: PROP_CUST1,
					editorList: function (o) {
						if(o.jsData)
						return o.jsData.getProperties();
					}
				},
				{ name: 'href', editor: EDIT_TEXT, desc: 'Specifies the hyperlink target for the area' },
				{ name: 'nohref', editor: EDIT_ENUM, values: ['', 'nohref'], desc: 'Not supported in HTML5. Specifies that an area has no associated link' },
				{ name: 'shape', editor: EDIT_ENUM, values: ['', 'default', 'rect', 'circle', 'poly'], desc: 'Specifies the shape of the area' },
				{ name: 'target', editor: EDIT_ENUM, values: ['', '_blank', '_parent', '_self', '_top'], allowEdit: true, desc: 'Specifies where to open the target URL' },
				{ name: 'type', editor: EDIT_TEXT, desc: 'MIME type of the target URL' },
				{ name: 'rel', editor: EDIT_ENUM, values: ['', 'alternate', 'author', 'bookmark', 'help', 'license', 'next', 'nofollow', 'noreferrer', 'prefetch', 'prev', 'search', 'tag'], desc: 'Specifies the relationship between the current document and the target URL' },
				{ name: 'media', editor: EDIT_TEXT, desc: 'Specifies what media/device the target URL is optimized for' },
				{ name: 'hreflang', editor: EDIT_TEXT, desc: 'Specifies the language of the target URL' },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('area'); obj.shape = 'rect'; return obj; }
			},
			{
				tagname: 'bdo', properties: [
				{ name: 'dir', editor: EDIT_ENUM, values: ['', 'ltr', 'rtl'], desc: 'Specifies the text direction of the text inside the bdo element' },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('bdo'); obj.innerHTML = 'new bdo'; return obj; }
			},
			{
				tagname: 'blockquote', properties: [
				{ name: 'cite', editor: EDIT_TEXT, desc: 'Specifies the source of the quotation', byAttribute:true },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('blockquote'); obj.innerHTML = 'new blockquote'; return obj; }
			},
			{
				tagname: 'cite', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('cite'); obj.innerHTML = 'new citation'; return obj; }
			},
			{
				tagname: 'code', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('code'); obj.innerHTML = '/*code sample*/ var v=2;'; return obj; }
			},
			{
				tagname: 'fieldset', properties: [
				{ name: 'name', editor: EDIT_TEXT, desc: 'Specifies the name of the fieldset' },
				{ name: 'disabled', editor: EDIT_BOOL, desc: 'Specifies that a group of related form elements should be disabled' },
				{ name: 'addLegend', editor: EDIT_CMD, IMG: '/libjs/newlegend.png', action: addLegend },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('fieldset'); var lg = _editorOptions.elementEditedDoc.createElement('legend'); obj.appendChild(lg); lg.innerHTML = '<b>new group</b>'; return obj; }
			},
			{
				tagname: 'legend', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('legend '); obj.innerHTML = '<b>group</b>'; return obj; }
			},
			{
				tagname: 'form', properties: [
				{ name: 'name', editor: EDIT_TEXT, desc: 'Specifies the name of a form' },
				{ name: 'action', editor: EDIT_TEXT, desc: 'Specifies where to send the form-data when a form is submitted' },
				{ name: 'method', editor: EDIT_ENUM, values: ['get', 'post'], desc: 'Specifies the HTTP method to use when sending form-data' },
				{ name: 'enctype', editor: EDIT_ENUM, values: ['', 'ication/x-www-form-urlencoded', 'multipart/form-data'], desc: 'Specifies how the form-data should be encoded when submitting it to the server (only for method="post")' },
				{ name: 'accept-charset', editor: EDIT_TEXT, desc: 'Specifies the character encodings that are to be used for the form submission' },
				{ name: 'accept', editor: EDIT_TEXT, desc: 'Not supported in HTML5. Specifies the types of files that the server accepts (that can be submitted through a file upload)' },
				{ name: 'dox', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'margings and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('form'); var sb = _editorOptions.elementEditedDoc.createElement('input'); sb.type = 'submit'; obj.appendChild(sb); return obj; }
			},
			{
				tagname: 'iframe', properties: [
				{ name: 'name', editor: EDIT_TEXT, desc: 'Specifies the name of an iframe element' },
				{
					name: 'src', editor: EDIT_TEXT, desc: 'Specifies the address of the document to embed in the iframe element',
					getter: function (o) {
						if (_editorOptions.forIDE) {
							var s = o.getAttribute('srcDesign');
							if(s)
								return s;
						}
						return o.src;
					},
					setter: function (o, v) {
						if (_editorOptions.forIDE) {
							o.setAttribute('srcDesign', v);
							o.setAttribute('src', '/libjs/iframeDesign.jpg');
						}
						else
							o.src = v;
					},
					onsetprop: function(o, v) {
						var furl = getIframFullUrl(o);
						refreshPropertyDisplay('fullUrl', furl);
					}
				},
				{
					name: 'fullUrl', editor: EDIT_NONE, desc: 'Specifies the address of the document to be used for web visiting.',
					getter: function(o) {
						return getIframFullUrl(o);
					}
				},
				frameWidthProp,
				frameHeightProp,
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'margings and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function () { var obj = _editorOptions.elementEditedDoc.createElement('iframe'); obj.width = '615'; obj.height = '380'; obj.style.pointerEvents = 'none'; return obj; }
			},
			{
				tagname: 'kbd', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('kbd'); obj.innerHTML = 'keyboard input'; return obj; }
			},
			{
				tagname: 'dfn', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('dfn'); obj.innerHTML = 'a definition term'; return obj; }
			},
			{
				tagname: 'samp', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('samp'); obj.innerHTML = 'sample output'; return obj; }
			},
			{
				tagname: 'pre', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('pre'); obj.innerHTML = 'preformatted text'; return obj; }
			},
			{
				tagname: 'var', properties: [
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('var'); obj.innerHTML = 'variable name'; return obj; }
			},
			{
				tagname: 'embed', properties: [
				{ name: 'src', editor: EDIT_TEXT, desc: 'the address of the external file to embed', isFilePath: true, filetypes: '.href', disableUpload: true, title: 'Select file to embed' },
				{ name: 'height', editor: EDIT_NUM, desc: 'the height of the embedded content' },
				{ name: 'width', editor: EDIT_NUM, desc: 'the width of the embedded content' },
				{ name: 'type', editor: EDIT_TEXT, desc: 'the MIME type of the embedded content' }
				]
			},
			{
				tagname: 'object', properties: [
				{ name: 'addParam', editor: EDIT_CMD, IMG: '/libjs/addparam.png', action: addObjectParameter },
				{ name: 'parameters', editor: EDIT_CHILD, childTag: 'param' },
				{ name: 'name', editor: EDIT_TEXT },
				{ name: 'archive', editor: EDIT_TEXT, desc: 'Not supported in HTML5. A space separated list of URL\'s to archives. The archives contains resources relevant to the object' },
				{ name: 'classid', editor: EDIT_TEXT, desc: 'Not supported in HTML5. Defines a class ID value as set in the Windows Registry or a URL' },
				{ name: 'codebase', editor: EDIT_TEXT, desc: 'Not supported in HTML5. Defines where to find the code for the object' },
				{ name: 'codetype', editor: EDIT_TEXT, desc: 'Not supported in HTML5. The internet media type of the code referred to by the classid attribute' },
				{ name: 'data', editor: EDIT_TEXT, desc: 'URL of the resource to be used by the object' },
				{ name: 'declare', editor: EDIT_ENUM, values: ['', 'declare'], desc: 'Not supported in HTML5. Defines that the object should only be declared, not created or instantiated until needed' },
				{ name: 'standby', editor: EDIT_TEXT, desc: 'Not supported in HTML5. Defines a text to display while the object is loading' },
				{ name: 'type', editor: EDIT_TEXT, desc: 'MIME type of data specified in the data attribute' },
				{ name: 'usemap', editor: EDIT_TEXT, desc: 'name of a client-side image map to be used with the object' },
				{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER },
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'margins and paddings', editor: EDIT_PROPS, editorList: marginPaddingProps, cat: PROP_MARGIN },
				{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('object'); obj.width = '615'; obj.height = '380'; return obj; }
			},
			{
				tagname: 'param', properties: [
				{ name: 'name', editor: EDIT_TEXT, desc: 'Specifies the name of a parameter' },
				{ name: 'value', editor: EDIT_TEXT, desc: 'Specifies the value of the parameter' },
				{ name: 'valuetype', editor: EDIT_ENUM, values: ['', 'data', 'ref', 'object'], desc: 'Not supported in HTML5. Specifies the type of the value' },
				{ name: 'type', editor: EDIT_TEXT, desc: 'Not supported in HTML5. MIME type of the parameter' }
				],
				creator: function() {
					var obj = _editorOptions.elementEditedDoc.createElement('param');
					var nm = 'p' + Math.floor(Math.random() * 65536);
					obj.name = nm; return obj;
				}
			},
			{
				tagname: 'optgroup', properties: [
				{ name: 'newOption', editor: EDIT_CMD, IMG: '/libjs/newDefinition.png', action: addOption },
				{ name: 'items', editor: EDIT_CHILD, childTag: 'option' },
				{ name: 'label', editor: EDIT_TEXT, desc: 'Specifies a label for an option-group' },
				{ name: 'formatting', editor: EDIT_PROPS, editorList: textProps, cat: PROP_TEXT },
				{ name: 'background', editor: EDIT_PROPS, editorList: backgroundProps, cat: PROP_BK },
				{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				],
				creator: function() { var obj = _editorOptions.elementEditedDoc.createElement('optgroup'); obj.innerHTML = 'new group'; return obj; }
			},
			{
				tagname: 'video', properties: [
				{ name: 'autoplay', editor: EDIT_BOOL, getter: function (o) { return getSingleValueAttr(o, 'autoplay'); }, setter: function (o, v) { setSingleValueAttr(o, 'autoplay', v); }, desc: 'Specifies that the video will start playing as soon as it is ready' },
				{ name: 'controls', editor: EDIT_BOOL, getter: function (o) { return getSingleValueAttr(o, 'controls'); }, setter: function (o, v) { setSingleValueAttr(o, 'controls', v); }, desc: 'Specifies that video controls should be displayed (such as a play/pause button etc).' },
				{ name: 'loop', editor: EDIT_BOOL, getter: function (o) { return getSingleValueAttr(o, 'loop'); }, setter: function (o, v) { setSingleValueAttr(o, 'loop', v); }, desc: 'Specifies that the video will start over again, every time it is finished' },
				{ name: 'muted', editor: EDIT_BOOL, getter: function (o) { return getSingleValueAttr(o, 'muted'); }, setter: function (o, v) { setSingleValueAttr(o, 'muted', v); }, desc: 'Specifies that the audio output of the video should be muted' },
				{ name: 'height', editor: EDIT_NUM, isPixel: true, desc: 'Sets the height of the video player' },
				{ name: 'width', editor: EDIT_NUM, isPixel: true, desc: 'Sets the width of the video player' },
				{ name: 'poster', editor: EDIT_TEXT, isFilePath: true, maxSize: 1048576, title: 'Select Image File', filetypes: '.image', desc: 'Specifies an image to be shown while the video is downloading, or until the user hits the play button' },
				{ name: 'preload', editor: EDIT_ENUM, values: ['', 'auto', 'metadata', 'none'], desc: 'Specifies if and how the author thinks the video should be loaded when the page loads' },
				{ name: 'src', editor: EDIT_TEXT, isFilePath: true, filetypes: '.mp4,.ogg,.webm', desc: 'Specifies the URL of the video file' },
				{
					name: 'alternativeSources', desc: 'specifies alternative video URLs to support different web browsers', editor: EDIT_PROPS, cat: PROP_CUST1,
					editorList: function (o) {
						var i = 0;
						var props = [];
						var srcs = o.getElementsByTagName('source');
						function createSrcProp(idx, s, needGrow) {
							var p = {
								name: 'source ' + (1 + idx), editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'alternative video source URL for supporting various web browsers', isFilePath: true, title: 'Select video File', filetypes: '.mp4,.ogg,.webm', getter: function (o0) { if (!s.isNew) return s.getAttribute('src'); },
								setter: function (o0, val) {
									if (val && val.length > 0) {
										var pos = val.lastIndexOf('.');
										if (pos > 0) {
											if (s.isNew) {
												s = _createElement('source');
												o.appendChild(s);
											}
											s.setAttribute('src', val);
											s.setAttribute('type', 'video/' + val.substr(pos));
										}
									}
									else {
										if (!s.isNew) {
											o.removeChild(s);
											s = {isNew:true};
										}
									}
								}
							};
							if (needGrow) {
								p.needGrow = true;
								p.onsetprop = function (o0, v) {
									if (this.grow) {
										if (v && v.length > 0) {
											this.grow(createSrcProp(idx + 1,{isNew:true}, true), 1);
											this.grow = null;
										}
									}
								};
							}
							return p;
						}
						if (srcs != null && srcs.length > 0) {
							var j = 0;
							for (i = 0; i < srcs.length; i++) {
								if (srcs[i].hasAttribute('src')) {
									props.push(createSrcProp(j, srcs[i], false));
									j++;
								}
								else {
									o.removeChild(srcs[i]);
								}
							}
						}
						props.push(createSrcProp(i, {isNew:true}, true));
						return props;
					}
				}
				]
			},
			{
				tagname: 'youtube', properties: [
				{
					name: 'youtubeID', editor: EDIT_TEXT, desc: 'Specifies the video to be used. It is a YouTube video ID. If you copy your video URL from YouTube then it will try to figure out the YouTube video ID by looking up "v="',
					getter: function (o) {
						var src;
						if (_editorOptions.forIDE)
							src = o.getAttribute('srcDesign');
						else
							src = o.src;
						if (src && src.length > "http://www.youtube.com/embed/".length) {
							return src.substr("http://www.youtube.com/embed/".length);
						}
					},
					setter: function (o, v) {
						if (_editorOptions.forIDE) {
							o.setAttribute('src', '/libjs/youtube.jpg');
							o.setAttribute('srcDesign', 'http://www.youtube.com/embed/' + getyoutubeid(v));
						}
						else
							o.src = 'http://www.youtube.com/embed/' + getyoutubeid(v);
					}
				},
				frameWidthProp,
				frameHeightProp,
				{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
				{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				internalAttributes: [
					'allowfullscreen',
					'typename',
					'type',
					'frameborder',
					'src'
				]
			},
			{
				tagname: 'menubar', nounformat: true, nocommonprops: true, noCust: true,nodelete:false,
				objSetter: function(o) {
					o.jsData.editor = {
						addItem: function(ul, ulsrc) {
							_addItem(ul, ulsrc);
						}
					}
					var sp = document.createElement('span');
					sp.innerHTML = 'menu styles';
					sp.style.cursor = 'pointer';
					_tdObj.appendChild(sp);
					JsonDataBinding.AttachEvent(sp, "onclick", function() { showProperties(o); });
					JsonDataBinding.AttachEvent(sp, "onmouseover", function() { sp.style.backgroundColor = '#D3D3D3'; });
					JsonDataBinding.AttachEvent(sp, "onmouseout", function() { sp.style.backgroundColor = ''; });
					var o2 = o.cloneNode(true);
					o2.className = 'htmlEditor_menuEditor';
					o2.sourceNav = o;
					o2.style.overflow = 'auto';
					_tdObj.lastobj = o2;
					_tdObj.appendChild(o2);
					adjustSizes();
					function onmenuitemclick(li0) {
						var li = li0;
						function isRoot() {
							return (li0.parentNode.parentNode.tagName.toLowerCase() == 'nav');
						}
						function getAnchor() {
							for (var i = 0; i < li.children.length; i++) {
								if (li.children[i].tagName.toLowerCase() == 'a') {
									return li.children[i];
								}
							}
						}
						function getAnchorSource() {
							for (var i = 0; i < li.sourceLi.children.length; i++) {
								if (li.sourceLi.children[i].tagName.toLowerCase() == 'a') {
									return li.sourceLi.children[i];
								}
							}
						}
						function getText() {
							var a = getAnchor();
							return a.innerHTML;
						}
						function setText(val) {
							var a = getAnchor();
							a.innerHTML = val;
							a = getAnchorSource();
							a.innerHTML = val;
						}
						function getTargetUrl() {
							var a = getAnchorSource();
							return a.savedhref ? a.savedhref : '';
						}
						function setTargetUrl(val) {
							var a = getAnchorSource();
							a.savedhref = val;
						}
						function getTargetPlace() {
							var a = getAnchorSource();
							return a.savedtarget ? a.savedtarget : '';
						}
						function setTargetPlace(val) {
							var a = getAnchorSource();
							a.savedtarget = val;
						}
						function addSubItem() {
							var ul;
							for (var i = 0; i < li.children.length; i++) {
								if (li.children[i].tagName.toLowerCase() == 'ul') {
									ul = li.children[i];
									break;
								}
							}
							if (!ul) {
								ul = document.createElement('ul');
								li.appendChild(ul);
							}
							//
							var ulsrc;
							for (var i = 0; i < li.sourceLi.children.length; i++) {
								if (li.sourceLi.children[i].tagName.toLowerCase() == 'ul') {
									ulsrc = li.sourceLi.children[i];
									break;
								}
							}
							if (!ulsrc) {
								ulsrc = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['ul']);
								_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [li.sourceLi, ulsrc]);
							}
							ul.sourceUl = ulsrc;
							o.jsData.editor.addItem(ul, ulsrc);
							//o.jsData.applyMenuStyles(o);
							_editorOptions.client.executeJsDb.apply(_editorOptions.elementEditedWindow, ['initMenubar', o]);
						}
						function moveItemUp() {
							LI_moveup(li.sourceLi);
							LI_moveup(li, true);
						}
						function moveItemDown() {
							LI_movedown(li.sourceLi);
							LI_movedown(li, true);
						}
						return function(ev) {
							showProperties({
								subEditor: true,
								obj: li,
								owner: o,
								getString: function() {
									return '{' + elementToString(this.owner) + '}.{' + getText() + '}';
								},
								getProperties: function() {
									var props = new Array();
									props.push({
										name: 'delete', toolTips: 'delete the menu item', editor: EDIT_DEL,
										action: function() {
											if (confirm('Do you want to remove this menu item ' + getText() + ' and all its sub items?')) {
												var newOwner;
												//var undoItem = { guid: 'body', undoInnerHTML: _editorOptions.elementEditedDoc.body.innerHTML };
												var ul = li.parentNode;
												var ulsrc = li.sourceLi.parentNode;
												ul.removeChild(li);
												ulsrc.removeChild(li.sourceLi);
												newOwner = ulsrc;
												if (ul.children.length == 0) { //no more items
													if (ul.parentNode.tagName.toLowerCase() == 'li') {
														//remove the ul from the li
														var puLi = ul.parentNode;
														puLi.removeChild(ul);
														var ulsrc = ul.sourceUl;
														var plisrc = ulsrc.parentNode;
														plisrc.removeChild(ulsrc);
														newOwner = plisrc;
													}
												}
												//o.jsData.applyMenuStyles(o);
												_editorOptions.client.executeJsDb.apply(_editorOptions.elementEditedWindow, ['initMenubar', o]);
												selectEditElement(newOwner);
												//undoItem.redoInnerHTML = _editorOptions.elementEditedDoc.body.innerHTML;
												//undoItem.done = true;
												//addUndoItem(undoItem);
												if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
											}
										},
										notModify:true
									});
									props.push({
										name: 'addSubItem', toolTips: 'add a new sub menu item', editor: EDIT_CMD, IMG: '/libjs/newElement.png',
										action: addSubItem
									}
									);
									if (isRoot()) {
										props.push({
											name: 'moveleft', toolTips: 'move current menu item to left', editor: EDIT_CMD, IMG: '/libjs/moveleft.png',
											action: moveItemUp
										}
										);
										props.push({
											name: 'moveright', toolTips: 'move current menu item to right', editor: EDIT_CMD, IMG: '/libjs/moveright.png',
											action: moveItemDown
										}
										);
									}
									else {
										props.push({
											name: 'moveup', toolTips: 'move current menu item up', editor: EDIT_CMD, IMG: '/libjs/moveup.png',
											action: moveItemUp
										}
										);
										props.push({
											name: 'movedown', toolTips: 'move current menu item down', editor: EDIT_CMD, IMG: '/libjs/movedown.png',
											action: moveItemDown
										}
										);
									}
									props.push({ name: 'Text', desc: 'menu item text', editor: EDIT_TEXT, getter: function() { return getText(); }, setter: function(o, val) { setText(val); } });
									props.push({
										name: 'TargetURL', isFilePath: true, title: 'Target Web Page', filetypes: '.href', disableUpload: true, desc: 'the URL of page the menu item goes to. ', editor: EDIT_TEXT, getter: function () { return getTargetUrl(); },
										setter: function(o, val) {
											setTargetUrl(val);
										}
									});
									props.push({ name: 'TargetPlace', desc: 'specifies where to open the linked document', editor: EDIT_ENUM, values: ['', '_blank', '_parent', '_self', '_top'], allowEdit: true, getter: function() { return getTargetPlace(); }, setter: function(o, val) { setTargetPlace(val); } });
									return { props: props };
								}
							});
							if (ev && ev.stopPropagation) {
								ev.stopPropagation();
							}
							else if (window.event) {
								window.event.cancelBubble = true;
							}
							//return true;
						}
					}
					function _addItem(ul, ulsrc) {
						var li = document.createElement('li');
						var a = document.createElement('a');
						var lisrc = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['li']);
						var asrc = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['a']);
						a.innerHTML = 'Item';
						a.href = '#';
						asrc.innerHTML = 'Item';
						ul.appendChild(li);
						li.appendChild(a);
						li.sourceLi = lisrc;
						_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [ulsrc, lisrc]);
						_editorOptions.client.appendChild.apply(_editorOptions.elementEditedWindow, [lisrc, asrc]);
						JsonDataBinding.AttachEvent(li, "onclick", onmenuitemclick(li));
					}
					function hookMenuEditor(ul, ulsrc) {
						ul.sourceUl = ulsrc;
						for (var i = 0; i < ul.children.length; i++) {
							ul.children[i].sourceLi = ulsrc.children[i];
							for (var m = 0; m < ul.children[i].children.length; m++) {
								if (ul.children[i].children[m].tagName.toLowerCase() == 'a') {
									ul.children[i].children[m].href = '#';
									break;
								}
							}
							JsonDataBinding.AttachEvent(ul.children[i], "onclick", onmenuitemclick(ul.children[i]));
							for (var k = 0; k < ul.children[i].children.length; k++) {
								var tag = ul.children[i].children[k].tagName ? ul.children[i].children[k].tagName.toLowerCase() : '';
								if (tag == 'ul') {
									hookMenuEditor(ul.children[i].children[k], ulsrc.children[i].children[k]);
								}
							}
						}
					}
					if (o2.children.length > 0) {
						hookMenuEditor(o2.children[0], o.children[0]);
					}
				},
				properties: [
					idProp,
					{
						name: 'class', toolTips: 'class names for the menubar', editor: EDIT_TEXT, getter: function (o) { return o.jsData.getClassNames(); }, setter: function (o, val) { o.jsData.setClassNames(val);}
					},
					{
						name: 'newBarItem', toolTips: 'add a new menu bar item', editor: EDIT_CMD, IMG: '/libjs/newElement.png',
						action: function(e) {
							var c = JsonDataBinding.getSender(e);
							if (c && c.owner) {
								var nav = c.owner;
								if (nav.jsData) {
									var navObj = _tdObj.getElementsByTagName('nav');
									if (navObj && navObj.length > 0) {
										navObj = navObj[0];
										var ul = navObj.children[0];
										var ulsrc = nav.children[0];
										nav.jsData.editor.addItem(ul, ulsrc);
										//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
									}
								}
							}
						}
					},
					{ name: 'menuStyleName', desc: 'the name identifying menu styles', editor: EDIT_TEXT, getter: function(o) { if (o.className && JsonDataBinding.startsWith(o.className, 'limnorstyles_menu')) { return o.className.substr('limnorstyles_menu'.length); } }, setter: function(o, val) { o.jsData.setStylesName.apply(_editorOptions.elementEditedWindow, [val]); } },
					{
						name: 'fontFamily', forStyle: true, editor: EDIT_ENUM, values: fontList, getter: function(o) { return o.jsData.getFontFamily(); }, setter: function(o, val) {
							o.jsData.setFontFamily(val);
						}
					},
					{
						name: 'fontSize', forStyle: true, editor: EDIT_ENUM, values: fontSizes, allowEdit: true,
						getter: function (o) { return o.jsData.getFontSize(); }, setter: function (o, val) {
							o.jsData.setFontSize(val);
						},
						onselectindexchanged:
						function (sel) {
							if (sel.selectedIndex) {
								var sv = sel.options[sel.selectedIndex].value;
								if (sv) {
									if (sv == '%' || sv == 'size in px, cm, ...') {
										return false;
									}
									return true;
								}
							}
							return false;
						},
						ontextchange:
						function (txt, sel) {
							if (txt && sel.selectedIndex) {
								var sv = sel.options[sel.selectedIndex].value;
								if (sv == 'size in px, cm, ...')
									return txt;
								if (sv == '%' && txt.length > 0)
									if (txt.substr(txt.length - 1, 1) == '%')
										return txt;
									else
										return txt;
							}
						}
					},
					{
						name: 'FullWidth', desc: 'Whether the menu bar takes full width', editor: EDIT_BOOL, getter: function(o) { return o.jsData.getFullWidth(); },
						setter: function(o, val) {
							o.jsData.setFullWidth(val);
						}
					},
					{ name: 'marginTop', desc: 'Top margin, in pixels, of the menu bar', editor: EDIT_NUM, getter: function (o) { return o.jsData.getMenubarMarginTop(); }, setter: function (o, val) { o.jsData.setMenubarMarginTop(val); } },
					{ name: 'marginBottom', desc: 'Bottom margin, in pixels, of the menu bar', editor: EDIT_NUM, getter: function (o) { return o.jsData.getMenubarMarginBottom(); }, setter: function (o, val) { o.jsData.setMenubarMarginBottom(val); } },
					{ name: 'menuTextColor', desc: 'Color of menu bar texts', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getMenubarTextColor(); }, setter: function(o, val) { o.jsData.setMenubarTextColor(val); } },
					{ name: 'menuHoverTextColor', desc: 'Color of menu bar text when mouse pointer moves on text', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getMenubarHoverTextColor(); }, setter: function(o, val) { o.jsData.setMenubarHoverTextColor(val); } },
					{ name: 'menuBarHorizontalPadding', desc: 'Horizontal paddings between menu bar texts', editor: EDIT_NUM, getter: function(o) { return o.jsData.getMenubarPaddingX(); }, setter: function(o, val) { o.jsData.setMenubarPaddingX(val); } },
					{ name: 'menuBarVerialPadding', desc: 'Vertical paddings between menu bar texts', editor: EDIT_NUM, getter: function(o) { return o.jsData.getMenubarPaddingY(); }, setter: function(o, val) { o.jsData.setMenubarPaddingY(val); } },
					{ name: 'menuBarRadius', desc: 'Radius of menu bar corners', editor: EDIT_NUM, getter: function(o) { return o.jsData.getMenubarRadius(); }, setter: function(o, val) { o.jsData.setMenubarRadius(val); } },
					{ name: 'menuBarBackColor', desc: 'Background color of menu bar', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getMenubarBkColor(); }, setter: function(o, val) { o.jsData.setMenubarBkColor(val); } },
					{ name: 'menuBarGradientColor', desc: 'Background gradient color of menu bar', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getMenubarGradientColor(); }, setter: function(o, val) { o.jsData.setMenubarGradientColor(val); } },
					{ name: 'menuBarHoverBackColor', desc: 'Background color of menu bar text  when mouse pointer moves on text', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getMenubarHoverBkColor(); }, setter: function(o, val) { o.jsData.setMenubarHoverBkColor(val); } },
					{ name: 'menuBarHoverGradientColor', desc: 'Background gradient color of menu bar text when mouse pointer moves on text', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getMenubarHoverGradientColor(); }, setter: function(o, val) { o.jsData.setMenubarHoverGradientColor(val); } },
					{ name: 'dropdownHorizontalPadding', desc: 'Horizontal paddings for drop down texts', editor: EDIT_NUM, getter: function(o) { return o.jsData.getItemPaddingX(); }, setter: function(o, val) { o.jsData.setItemPaddingX(val); } },
					{ name: 'dropdownVerticalPadding', desc: 'Vertical paddings for drop down texts', editor: EDIT_NUM, getter: function(o) { return o.jsData.getItemPaddingY(); }, setter: function(o, val) { o.jsData.setItemPaddingY(val); } },
					{ name: 'dropdownRadius', desc: 'Radius of drop down corners', editor: EDIT_NUM, getter: function(o) { return o.jsData.getItemRadius(); }, setter: function(o, val) { o.jsData.setItemRadius(val); } },
					{ name: 'dropdownBackColor', desc: 'Background color of a drop down. a drop down appears when mouse pointer moves over its parent', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getDropdownBkColor(); }, setter: function(o, val) { o.jsData.setDropdownBkColor(val); } },
					{ name: 'dropdownGradientColor', desc: 'Background color of a drop down. a drop down appears when mouse pointer moves over its parent', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getDropdownGradientColor(); }, setter: function(o, val) { o.jsData.setDropdownGradientColor(val); } },
					{ name: 'dropdownItemBackColor', desc: 'Background color of a drop down item when mouse pointer moves over it', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getItemHoverBkColor(); }, setter: function(o, val) { o.jsData.setItemHoverBkColor(val); } },
					{ name: 'dropdownItemGradientColor', desc: 'Background gradient color of a drop down item when mouse pointer moves over it', editor: EDIT_COLOR, getter: function(o) { return o.jsData.getItemHoverGradientColor(); }, setter: function(o, val) { o.jsData.setItemHoverGradientColor(val); } }
				],
				creator: function() {
					var obj = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['nav']);
					obj.typename = 'menubar';
					obj.jsData = _editorOptions.client.createMenuStyles.apply(_editorOptions.elementEditedWindow,[obj]);
					obj.contentEditable = false;
					return obj;
				}
			},
			{
				tagname: 'treeview', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true,nodelete:false,
				properties: [
					idProp,
					{
						name: 'newRootItem', toolTips: 'add a new root item', editor: EDIT_CMD, IMG: '/libjs/newElement.png',
						action: function(e) {
							var c = JsonDataBinding.getSender(e);
							if (c && c.owner) {
								var ul = c.owner;
								if (ul && ul.jsData) {
									ul.jsData.addItem.apply(_editorOptions.elementEditedWindow);
									//if (typeof _editorOptions != 'undefined') _editorOptions.pageChanged = true;
								}
							}
						}
					},
					validateCmdProp,
					{
						name: 'StyleName', desc: 'Modifying of one treeView styles will be applied to all tree views with the same StyleName.',
						getter: function(o) {
							return o.jsData.designer.getStyleName.apply(_editorOptions.elementEditedWindow);
						},
						setter: function(o, val) {
							o.jsData.designer.setStyleName.apply(_editorOptions.elementEditedWindow, [val]);
						}
					},
					{
						name: 'HoverBackColor', desc: 'Background color of tree view item when mouse pointer moves on a tree view item. This property is for all tree views.', editor: EDIT_COLOR,
						getter: function(o) {
							return o.jsData.designer.getHoverBkColor.apply(_editorOptions.elementEditedWindow);
						},
						setter: function(o, val) {
							o.jsData.designer.setHoverBkColor.apply(_editorOptions.elementEditedWindow, [val]);
						}
					}
					, { name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
				],
				creator: function() {
					var obj = _editorOptions.client.createElement.apply(_editorOptions.elementEditedWindow, ['ul']);
					_editorOptions.client.onCreatedObject.apply(_editorOptions.elementEditedWindow, ['HtmlEditorTreeview', obj, _editorOptions.client]);
					_initPageElement(obj);
					_editorOptions.client.executeClientFunc.apply(_editorOptions.elementEditedWindow, [obj.jsData.addItem]);
					return obj;
				}
			},
			{
				tagname: 'slideshow', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete:false,
				properties: [
					idProp
					, {
						name: 'slides', editor: EDIT_PROPS, editorList: function (o) {
							var i;
							var slides = o.jsData.getSlides();
							var props = [];
							function getterActImg(idx, obj) {
								return function (owner) {
									var sl = owner.jsData.getSlide(idx);
									if (sl) {
										return sl.img;
									}
								}
							}
							function setterActImg(idx, obj) {
								return function (owner, val) {
									var sl = owner.jsData.getSlide(idx);
									if (sl) {
										sl.img = val;
									}
								}
							}
							function getterActTxt(idx, obj) {
								return function (owner) {
									var sl = owner.jsData.getSlide(idx);
									if (sl) {
										return sl.txt;
									}
								}
							}
							function setterActTxt(idx, obj) {
								return function (owner, val) {
									var sl = owner.jsData.getSlide(idx);
									if (sl) {
										sl.txt = val;
									}
								}
							}
							function createImgProp(idx, needGrow) {
								var p = { name: 'image ' + idx, editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'slide image', isFilePath: true, maxSize: 1048576, title: 'Select Image File', filetypes: '.image', getter: getterActImg(idx, o), setter: setterActImg(idx, o) };
								if (needGrow) {
									p.needGrow = true;
									p.onsetprop = function (o, v) {
										if (this.grow) {
											if (v && v.length > 0) {
												this.grow(createImgProp(idx+1, true), 2);
												this.grow(createTxtProp(idx+1), 3);
												this.grow = null;
											}
										}
										o.jsData.showSlide();
									};
								}
								return p;
							}
							function createTxtProp(idx) {
								return { name: 'text ' + idx, editor: EDIT_TEXT, cat: PROP_CUST1, desc: 'slide caption', getter: getterActTxt(idx, o), setter: setterActTxt(idx, o) };
							}
							for (i = 0; i < slides.length; i++) {
								props.push(createImgProp(i,false));
								props.push(createTxtProp(i));
							}
							props.push(createImgProp(i,true));
							props.push(createTxtProp(i));
							return props;
						}, cat: PROP_CUST1
					}
					, {
						name: 'width', editor: EDIT_TEXT, canbepixel: true, desc: 'Sets the width of the slide show. It can be a length value in pixels, i.e. 300px, or in percent of its container, i.e. 100%. If both width and height are in % then only % of height is ignored.'
						, getter: function (o) {
							return o.getAttribute('w');
						}
						, setter: function (o, v) {
							if (typeof (v) == 'undefined' || v == null || v.length == 0) {
								o.removeAttribute('w');
								if (o.style.removeProperty) {
									o.style.removeProperty('width');
								}
								else {
									o.style.removeAttribute('width');
								}
							}
							else {
								o.setAttribute('w', v);
								if (v.charAt(v.length - 1) == '%') {
								}
								else {
									o.style.width = v;
								}
							}
							if (o.jsData) {
								o.jsData.onsizechanged();
							}
						}
					}
					, {
						name: 'height', editor: EDIT_TEXT, canbepixel: true, desc: 'Sets the height of the slide show. It can be a length value in pixels, i.e. 300px, or in percent of its container, i.e. 100%. If both width and height are in % then only % of height is ignored.'
						, getter: function (o) {
							return o.getAttribute('h');
						}
						, setter: function (o, v) {
							if (typeof (v) == 'undefined' || v == null || v.length == 0) {
								o.removeAttribute('h');
								if (o.style.removeProperty) {
									o.style.removeProperty('height');
								}
								else {
									o.style.removeAttribute('height');
								}
							}
							else {
								o.setAttribute('h', v);
								if (v.charAt(v.length - 1) == '%') {
								}
								else {
									o.style.height = v;
								}
							}
							if (o.jsData) {
								o.jsData.onsizechanged();
							}
						}
					}
					, {
						name: 'backgroundColor', editor: EDIT_COLOR, desc: 'background color'
						, getter: function (o) {
							return o.style.backgroundColor;
						}
						, setter: function (o, v) {
							o.style.backgroundColor=v;
						}
					}
					, { name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER }
					//, { name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX }
					, { name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT }
					, { name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'music', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete: false,
				properties: [
				idProp
					, {
						name: 'src', editor: EDIT_TEXT, byAttribute: true, allowEdit: false, isFilePath: true, maxSize: 1048576, desc: 'the URL of a music', title: 'Select music File', filetypes: '.mp3;.mid'
						, setter: function (o, v) {
							var p = o.parentNode;
							if (p) {
								o.setAttribute('src', v);
								var e2 = o.cloneNode(false);
								e2.setAttribute('src', v);
								p.insertBefore(e2, o);
								p.removeChild(o);
								e2.id = o.id;
								_editorOptions.selectedObject = null;
								selectEditElement(e2);
								//alert(e2.parentNode);
							}
						}
					}
					, { name: 'loop', editor: EDIT_BOOL, byAttribute: true }
					, { name: 'autostart', editor: EDIT_BOOL, byAttribute: true }
					, { name: 'volume', editor: EDIT_NUM, byAttribute: true,desc:'use 0 to 100 to specify music volume' }
					, {
						name: 'left', editor: EDIT_TEXT, canbepixel: true
						, getter: function (o) {
							return o.style.left;
						}
						, setter: function (o, v) {
							if (typeof v == 'undefined' || v == null || v.length == 0) {
								JsonDataBinding.removeStyleAttribute('left');
							}
							else if (JsonDataBinding.isNumber(v)) {
								o.style.left = v + 'px';
							}
							else {
								try {
									o.style.left = v;
								}
								catch (e) {
								}
							}
						}
					}
					, {
						name: 'top', editor: EDIT_TEXT, canbepixel: true
						, getter: function (o) {
							return o.style.top;
						}
						, setter: function (o, v) {
							if (typeof v == 'undefined' || v == null || v.length == 0) {
								JsonDataBinding.removeStyleAttribute('top');
							}
							else if (JsonDataBinding.isNumber(v)) {
								o.style.top = v + 'px';
							}
							else {
								try {
									o.style.top = v;
								}
								catch (e) {
								}
							}
						}
					}
					, {
						name: 'height', editor: EDIT_NUM, desc: 'height of the player', byAttribute: true
					}
					, {
						name: 'width', editor: EDIT_NUM, desc: 'width of the player', byAttribute: true
					}
					, { name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function () {
					var o = _createElement('embed');
					var src = prompt('Enter URL for music file, for example, a MP3 file. You may leave it blank and later set src property', '');
					o.typename = 'music';
					o.setAttribute('typename', 'music');
					o.setAttribute('src', src);
					o.setAttribute('type', "audio/x-pn-realaudio-plugin");
					o.setAttribute('controls', "ControlPanel");
					o.setAttribute('loop', "true");
					o.setAttribute('autostart', "true");
					o.setAttribute('volume', "100");
					o.setAttribute('initfn', "load-types");
					o.setAttribute('mime-types', "mime.types");
					o.setAttribute('height', "30");
					o.setAttribute('width', "300");
					return o;
				}
			},
			{
				tagname: 'flash', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete: false,
				properties: [
					idProp
					, {
						name: 'src', editor: EDIT_TEXT, byAttribute: true,allowEdit:false, isFilePath: true, maxSize: 1048576, desc: 'the URL of a flash', title: 'Select Flash File', filetypes: '.swf'
						, setter: function (o, v) {
							var p = o.parentNode;
							if (p) {
								o.setAttribute('src', v);
								var e2 = o.cloneNode(false);
								e2.setAttribute('src', v);
								p.insertBefore(e2, o);
								p.removeChild(o);
								e2.id = o.id;
								_editorOptions.selectedObject = null;
								selectEditElement(e2);
								//alert(e2.parentNode);
							}
						}
					}
					, {
						name: 'left', editor: EDIT_TEXT, canbepixel: true
						, getter: function (o) {
							return o.style.left;
						}
						, setter: function (o, v) {
							if (typeof v == 'undefined' || v == null || v.length == 0) {
								JsonDataBinding.removeStyleAttribute('left');
							}
							else if (JsonDataBinding.isNumber(v)) {
								o.style.left = v + 'px';
							}
							else {
								try
								{
									o.style.left = v;
								}
								catch (e) {
								}
							}
						}
					}
					, {
						name: 'top', editor: EDIT_TEXT, canbepixel: true
						, getter: function (o) {
							return o.style.top;
						}
						, setter: function (o, v) {
							if (typeof v == 'undefined' || v == null || v.length == 0) {
								JsonDataBinding.removeStyleAttribute('top');
							}
							else if (JsonDataBinding.isNumber(v)) {
								o.style.top = v + 'px';
							}
							else {
								try {
									o.style.top = v;
								}
								catch (e) {
								}
							}
						}
					}
					, {
						name: 'height', editor: EDIT_NUM, desc: 'height of the player', byAttribute: true
					}
					, {
						name: 'width', editor: EDIT_NUM, desc: 'width of the player', byAttribute: true
					}
					, { name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				],
				creator: function () {
					var o = _createElement('embed');
					o.typename = 'flash';
					o.setAttribute('typename', 'flash');
					o.setAttribute('hidechild', 'true');
					var src = prompt('Enter SWF file path for your flash player. You may leave it blank and later set src property with your SWF file', '');
					o.setAttribute('src', src);
					o.setAttribute('allowFullScreen', 'true');
					o.setAttribute('quality', 'high');
					o.setAttribute('width', '680');
					o.setAttribute('height', '460');
					o.setAttribute('align', 'middle');
					o.setAttribute('allowScriptAccess', 'always');
					o.setAttribute('type', 'application/x-shockwave-flash');
					o.setAttribute('PLUGINSPAGE', 'http://www.macromedia.com/go/getflashplayer');					
					return o;
				}
			},
			{
				tagname: 'hitcount', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete:false,
				properties: [
					{ name: 'font', editor: EDIT_PROPS, editorList: editor_font, cat: PROP_FONT },
					{ name: 'size and location', editor: EDIT_PROPS, editorList: sizelocProps, cat: PROP_SIZELOC }
				]
			},
			{
				tagname: 'fileupload', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, noSetInnerHtml:true, nodelete:false,
				properties: [
					{
						name: 'id', desc: 'a unique id for an element', editor: EDIT_TEXT,
						getter: function (o) {
							var d = getfileinput(o);
							if (d) {
								return d.id;
							}
						},
						setter: function (o, val) {
							if (_divError) { _divError.style.display = 'none'; }
							var finput = getfileinput(o);
							if (finput && finput.id != val) {
								var g = finput.getAttribute('limnorId')
								if (val == null || val.length == 0) {
									if (g) {
										showError('id for this element cannot be removed because it is used in programming.');
										return;
									}
								}
								if (val && val.length > 0) {
									if (val == 'body') {
										showError('"body" cannot be used as id.');
										return;
									}
									if (HtmlEditor.IsNameValid(val)) {
										var o0 = document.getElementById(val);
										if (o0) {
											showError('The id is in use.');
											return;
										}
										else {
											o0 = document.getElementById(val+'f');
											if (o0) {
												showError('The id is in use. Please try another one');
												return;
											}
										}
									}
									else {
										showError('The id is invalid. Please only use alphanumeric characters. The first letter can only be a-z, A-Z, $, and underscore');
										return;
									}
								}
								o.id = val + 'f';
								finput.id = val;
								finput.setAttribute('name', val);
								if (_editorOptions.forIDE) {
									if (g) {
										if (typeof (limnorStudio) != 'undefined') limnorStudio.onElementIdChanged(g, val);
										else window.external.OnElementIdChanged(g, val);
									}
								}
							}
						}
					},
					{
						name: 'width', editor: EDIT_TEXT, forStyle: true,
						getter: function (o) {
							var d = getfileinput(o);
							if (d) {
								return d.style.width;
							}
						},
						setter: function (o, v) {
							if (_divError) { _divError.style.display = 'none'; }
							var finput = getfileinput(o);
							if (finput) {
								if (typeof v != 'undefined' && v != null && JsonDataBinding.isNumber(v)) {
									v = v + 'px';
								}
								finput.style.width = v;
							}
						}
					},
					{
						name: 'MaximumFileSize', editor: EDIT_NUM, desc: 'Gets an integer indicating allowed maximum size, in KB, for file upload.',
						getter: function (o) {
							var d = getfilemaxsizeinput(o);
							if (d) return d.value;
							return 0;
						},
						setter: function (o, v) {
							var d = getfilemaxsizeinput(o);
							if (d) d.value = v;
						}
					}
				]
			},
			{
				tagname: 'svg', nounformat: true, nomoveout: true, noCust: true, nodelete: false,
				properties: [
					svgmleft,
					svgmright,
					svgmup,
					svgmdown,
					svgmdist,
					svgdup,
					{ name: 'addpolygon', editor: EDIT_CMD, IMG: '/libjs/html_svgpoly.png', action: addpolygon, toolTips: 'add a new polygon element to the drawing group' },
					{ name: 'addrect', editor: EDIT_CMD, IMG: '/libjs/html_svgrect.png', action: addrect, toolTips: 'add a new rectangle element to the drawing group' },
					{ name: 'addcircle', editor: EDIT_CMD, IMG: '/libjs/html_svgcircle.png', action: addcircle, toolTips: 'add a new circle element to the drawing group' },
					{ name: 'addellipse', editor: EDIT_CMD, IMG: '/libjs/html_svgellipse.png', action: addellipse, toolTips: 'add a new ellipse element to the drawing group' },
					{ name: 'addline', editor: EDIT_CMD, IMG: '/libjs/html_svgline.png', action: addline, toolTips: 'add a new line element to the drawing group' },
					{ name: 'addpolyline', editor: EDIT_CMD, IMG: '/libjs/html_svgpolyline.png', action: addpolyline, toolTips: 'add a new polyline element to the drawing group' },
					{ name: 'addtext', editor: EDIT_CMD, IMG: '/libjs/html_svgtext.png', action: addtext, toolTips: 'add a new text element to the drawing group' },
					{ name: 'shapes', editor: EDIT_CHILD, childTag: 'polygon,rect,circle,ellipse,polyline,line,path,text', desc: 'maps contained in the page' },
					{ name: 'position', editor: EDIT_PROPS, editorList: posProps, cat: PROP_POS },
					{ name: 'box', editor: EDIT_PROPS, editorList: boxDimProps, cat: PROP_BOX },
					{ name: 'border', editor: EDIT_PROPS, editorList: borderProps, cat: PROP_BORDER }
				]
			},
			{
				tagname: 'polygon', nounformat: true, nomoveout: true, noCust: true, nodelete: false,isSvgShape:true,
				properties: [
					{
						name: 'points', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the vertex points of the polygon', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgPoly(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			},
			{
				tagname: 'rect', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete: false, isSvgShape: true,
				properties: [
					{
						name: 'rectangle', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the rectangle parameters', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgRect(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			},
			{
				tagname: 'circle', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete: false, isSvgShape: true,
				properties: [
					{
						name: 'circle', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the circle parameters', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgCircle(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			},
			{
				tagname: 'ellipse', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete: false, isSvgShape: true,
				properties: [
					{
						name: 'ellipse', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the ellipse parameters', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgCircle(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			},
			{
				tagname: 'line', nounformat: true, nocommonprops: true, nomoveout: true, noCust: true, nodelete: false, isSvgShape: true, notclose:true,
				properties: [
					{
						name: 'line', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the line parameters', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgLine(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			},
			{
				tagname: 'polyline', nounformat: true, nomoveout: true, noCust: true, nodelete: false, isSvgShape: true, notclose:true,
				properties: [
					{
						name: 'points', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies the joint points of the polyline', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgPoly(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			},
			{
				tagname: 'text', nounformat: true, nomoveout: true, noCust: true, nodelete: false, isSvgShape: true, notclose: true, issvgtext:true,
				properties: [
					{
						name: 'text', editor: EDIT_PROPS, byAttribute: true, desc: 'Specifies text attributes', cat: PROP_CUST1,
						editorList: function (o) {
							if (!o.jsData)
								o.jsData = createSvgText(o);
							if (o.jsData)
								return o.jsData.getProperties();
						}
					}
				]
			}
			];
		} //end of getDefaultEditors
		var headingDesc = 'The "h1" to "h6" tags are used to define HTML headings. "h1" defines the most important heading. "h6" defines the least important heading.';
		function getDefaultAddables() {
			return [
				{ tag: 'a', name: 'a', image: '/libjs/html_a.png', toolTips: 'The "a" tag defines a hyperlink, which is used to link from one page to another.' }
				, { tag: 'bdo', name: 'bdo', image: '/libjs/html_bdo.png', toolTips: 'bdo stands for Bi-Directional Override. The "bdo" tag is used to override the current text direction.' }
				, { tag: 'blockquote', name: 'blockquote', image: '/libjs/html_blockquote.png', toolTips: '"blockquote" tag specifies a section that is quoted from another source.' }
				, { tag: 'button', name: 'button', image: '/libjs/html_button.png', pageonly: true, toolTips: 'The "button" tag defines a clickable button. Inside a "button" element you can put content, like text or images. This is the difference between this element and buttons created with the "input" element.' }
				, { tag: 'input', name: 'checkbox', image: '/libjs/html_checkbox.png', pageonly: true, type: 'checkbox', toolTips: 'Create a checkbox, it is an "input" with type=checkbox. ' }
				, { tag: 'cite', name: 'cite', image: '/libjs/html_cite.png', toolTips: 'The "cite" tag defines the title of a work (e.g. a book, a song, a movie, a TV show, a painting, a sculpture, etc.).' }
				, { tag: 'code', name: 'code', image: '/libjs/html_code.png', toolTips: '"code" defines a piece of computer code' }
				, { tag: 'dl', name: 'definition list', image: '/libjs/html_dl.png', toolTips: 'The "dl" tag defines a definition list. The "dl" tag is used in conjunction with "dt" (defines the item in the list) and "dd" (describes the item in the list).' }
				, { tag: 'div', name: 'division', image: '/libjs/html_div.png', pageonly: true, toolTips: 'The "div" tag defines a division or a section in an HTML document. The "div" tag is used to group block-elements to format them with styles.' }
				, { tag: 'fieldset', name: 'fieldset', image: '/libjs/html_fieldset.png', pageonly: true, toolTips: 'The "fieldset" tag is used to group related elements in a form. The "fieldset" tag draws a box around the related elements.' }
				/*, { tag: 'input', name: 'file', image: '/libjs/html_file.png', pageonly: true, type: 'file', toolTips: 'Create a file upload control, it is an "input" with type=file' }*/
				, { tag: 'form', name: 'form', image: '/libjs/html_form.png', pageonly: true, toolTips: 'The "form" tag is used to create an HTML form for user input. The "form" element can contain one or more of the following form elements: "input", "textarea", "button", "select", "option", "optgroup", "fieldset", and "label"' }
				, { tag: 'h1', name: 'heading 1', image: '/libjs/html_h.png', toolTips: headingDesc }
				, { tag: 'h2', name: 'heading 2', image: '/libjs/html_h.png', toolTips: headingDesc }
				, { tag: 'h3', name: 'heading 3', image: '/libjs/html_h.png', toolTips: headingDesc }
				, { tag: 'h4', name: 'heading 4', image: '/libjs/html_h.png', toolTips: headingDesc }
				, { tag: 'h5', name: 'heading 5', image: '/libjs/html_h.png', toolTips: headingDesc }
				, { tag: 'h6', name: 'heading 6', image: '/libjs/html_h.png', toolTips: headingDesc }
				, { tag: 'hr', name: 'horizontal line', image: '/libjs/html_hr.png', pageonly: true, toolTips: 'The "hr" tag defines a thematic break in an HTML page (e.g. a shift of topic). The "hr" element is used to separate content (or define a change) in an HTML page.' }
				, { tag: 'input', name: 'hidden', image: '/libjs/html_hidden.png', pageonly: true, type: 'hidden', toolTips: 'The Hidden object represents a hidden input field in an HTML form (this input field is invisible for the user). With this element you can send hidden form data to a server.' }
				, {
					tag: 'iframe', name: 'iframe', image: '/libjs/html_iframe.png', pageonly: true,
					onCreated: function(o) {
						if (o) {
							o.setAttribute('src', '/libjs/iframeDesign.jpg');
						}
					},
					toolTips: 'The "iframe" tag specifies an inline frame. An inline frame is used to embed another document within the current HTML document.'
				}
				, { tag: 'img', name: 'image', image: '/libjs/html_img.png', toolTips: 'The "img" tag defines an image in an HTML page.' }
				, { tag: 'input', name: 'input', image: '/libjs/html_input.png', pageonly: true, type: 'text', toolTips: 'Create a text box using an "input" tag with type=text' }
				, { tag: 'kbd', name: 'kbd', image: '/libjs/html_kbd.png', pageonly: true, toolTips: '"kbd" defines keyboard input' }
				, { tag: 'label', name: 'label', image: '/libjs/html_label.png', pageonly: true, toolTips: 'The "label" tag defines a label for an "input" element. The "label" element does not render as anything special for the user. However, it provides a usability improvement for mouse users, because if the user clicks on the text within the "label" element, it toggles the control. The for attribute of the "label" tag should be equal to the id attribute of the related element to bind them together.' }
				, { tag: 'br', name: 'line break', image: '/libjs/html_br.png', toolTips: 'The "br" tag inserts a single line break. ' }
				, { tag: 'ul', name: 'list', image: '/libjs/html_ul.png', toolTips: 'The "ul" tag defines an unordered (bulleted) list. Use the "ul" tag together with the "li" tag to create unordered lists.' }
				, { tag: 'select', name: 'list box', image: '/libjs/html_select.png', pageonly: true, toolTips: 'The "select" element is used to create a drop-down list. The "option" tags inside the "select" element define the available options in the list.' }
				, { tag: 'map', name: 'map', image: '/libjs/html_map.png', pageonly: true, toolTips: 'The "map" tag is used to define a client-side image-map. An image-map is an image with clickable areas. The required name attribute of the "map" element is associated with the "img"\'s usemap attribute and creates a relationship between the image and the map. The "map" element contains a number of "area" elements, that defines the clickable areas in the image map.' }
				, { tag: 'object', name: 'object', image: '/libjs/html_object.png', pageonly: true, toolTips: 'The "object" tag defines an embedded object within an HTML document. Use this element to embed multimedia (like audio, video, Java applets, ActiveX, PDF, and Flash) in your web pages. You can also use the "object" tag to embed another webpage into your HTML document. You can use the "param" tag to pass parameters to plugins that have been embedded with the "object" tag.' }
				, { tag: 'ol', name: 'ordered list', image: '/libjs/html_ol.png', toolTips: 'The "ol" tag defines an ordered list. An ordered list can be numerical or alphabetical. Use the "li" tag to define list items.' }
				, { tag: 'p', name: 'paragraph', image: '/libjs/html_p.png', toolTips: 'The "p" tag defines a paragraph. Browsers automatically add some space (margin) before and after each "p" element. The margins can be modified with the margin properties.' }
				, { tag: 'input', name: 'password', image: '/libjs/html_password.png', pageonly: true, type: 'password', toolTips: 'Create a password input text box using an "input" with type=password' }
				, { tag: 'pre', name: 'pre', image: '/libjs/html_pre.png', toolTips: 'The "pre" tag defines preformatted text. Text in a "pre" element is displayed in a fixed-width font (usually Courier), and it preserves both spaces and line breaks. Use the "pre" element when displaying text with unusual formatting, or some sort of computer code.' }
				, { tag: 'span', name: 'span', image: '/libjs/html_span.png', toolTips: 'The "span" tag is used to group inline-elements in a document. The "span" tag provides no visual change by itself. The "span" tag provides a way to add a hook to a part of a text or a part of a document. When a text is hooked in a "span" element, you can style it with styles, or manipulate it with JavaScript.' }
				, { tag: 'input', name: 'radio', image: '/libjs/html_radio.png', pageonly: true, type: 'radio', toolTips: 'Create a radio button using "input" with type=radio' }
				, { tag: 'input', name: 'reset', image: '/libjs/html_reset.png', pageonly: true, type: 'reset', toolTips: 'Create a reset button using "input" with type=reset' }
				, { tag: 'samp', name: 'sample', image: '/libjs/html_samp.png', toolTips: '"samp" defines sample output from a computer program' }
				, { tag: 'input', name: 'submit', image: '/libjs/html_submit.png', pageonly: true, type: 'submit', toolTips: 'Create a submit button using "input" with type=submit' }
				, { tag: 'table', name: 'table', image: '/libjs/html_table.png', toolTips: 'The "table" tag defines an HTML table. An HTML table consists of the "table" element and one or more "tr", "th", and "td" elements. The "tr" element defines a table row, the "th" element defines a table header, and the "td" element defines a table cell.' }
				, { tag: 'textarea', name: 'text area', image: '/libjs/html_textarea.png', pageonly: true, toolTips: 'The "textarea" tag defines a multi-line text input control. A text area can hold an unlimited number of characters, and the text renders in a fixed-width font (usually Courier). The size of a text area can be specified by the cols and rows attributes, or even better; through CSS\' height and width properties.' }
				, { tag: 'var', name: 'var', image: '/libjs/html_var.png', toolTips: '"var" defines a variable' }
				, {
					tag: 'menubar', htmltag: 'nav', name: 'menu-bar', image: '/libjs/html_menu.png', pageonly: true, toolTips: 'It defines a menu bar',
					onCreated: function(o) {
						if (o) {
							_editorOptions.client.onCreatedObject.apply(_editorOptions.elementEditedWindow, ['HtmlEditorMenuBar', o, _editorOptions.client]);
							_redisplayProperties();
						}
					}
				}
				, {
					tag: 'treeview', htmltag: 'ul', name: 'tree-view', image: '/libjs/html_treeview.png', pageonly: true, toolTips: 'It defines a tree view',
					onCreated: function(o) {
						if (o) {
							_editorOptions.client.onCreatedObject.apply(_editorOptions.elementEditedWindow, ['HtmlEditorTreeview', o, _editorOptions.client]);
							_initPageElement(o);
							_redisplayProperties();
						}
					}
				}
				, { tag: 'embed', name: 'embed', image: '/libjs/html_embed.png', pageonly: true, toolTips: '"embed" is used to embed an object in web page, for example, a music player' }
				, {
					tag: 'video', name: 'video', image: '/libjs/html_video.png', pageonly: false, toolTips: '"video" is used to embed a video in web page, for example, a mp4.',
					onCreated: function (o) {
						o.setAttribute('width', 320);
						o.setAttribute('height', 240);
						o.setAttribute('controls', 'controls');
						o.appendChild(_editorOptions.elementEditedDoc.createTextNode('Your browser does not support the video tag.'));
					}
				}
				, {
					tag: 'music', htmltag: 'embed', name: 'music', image: '/libjs/html_music.png', toolTips: 'use embed to add a music player plugin',
					onCreated: function(o) {
						if (o) {
							if (JsonDataBinding.IsIE()) {
								var p = o.parentNode;
								if (p) {
									var e2 = o.cloneNode(false);
									p.insertBefore(e2, o);
									p.removeChild(o);
									//e2.id = o.id;
									_editorOptions.selectedObject = null;
									selectEditElement(e2);
								}
							}
							else {
								_editorOptions.selectedObject = null;
								selectEditElement(o);
							}
						}
					}
				}
				, {
					tag: 'youtube', htmltag: 'iframe', name: 'youtube', image: '/libjs/html_youtube.png', toolTips: 'embed a YouTube video in the page',
					onCreated: function(o) {
						if (o) {
							o.typename = 'youtube';
							var src = prompt('Enter vedio ID for your YouTube video. You may leave it blank and later set youtubeID property with your video ID. You may use video URL from YouTube.', '');
							o.setAttribute('src', '/libjs/youtube.jpg');
							o.setAttribute('srcDesign', 'http://www.youtube.com/embed/' + getyoutubeid(src));
							o.setAttribute('type', "text/html");
							o.setAttribute('frameborder', "0");
							o.setAttribute('allowfullscreen', "true");
							o.setAttribute('height', "385");
							o.setAttribute('width', "640");
							o.setAttribute('typename', "youtube");
							_editorOptions.selectedObject = null;
							selectEditElement(o);
						}
					}
				}
				, {
					tag: 'slideshow', htmltag: 'div', name: 'slide show', image: '/libjs/html_slideshow.png', toolTips: 'use a list of images, each with a caption, to form slide show',
					onCreated: function (o) {
						if (o) {
							var tryCount = 0;
							o.style.width = '300px';
							o.style.height = '300px';
							o.style.border = 'solid';
							o.style.borderRadius = '15px';
							o.setAttribute('styleName', 'limnorslideshow');
							o.setAttribute('styleshare', 'NotShare');
							o.className = 'limnorslideshow';
							o.setAttribute('scriptData', _editorOptions.client.createDataId.apply(_editorOptions.editorWindow, ['ss', true]));
							_editorOptions.client.addCustJsLisFile.apply(_editorOptions.editorWindow, ['slideshow', o]);
							function showNewObj() {
								if (!o.jsData) {
									tryCount++;
									if (tryCount < 30) {
										setTimeout(showNewObj, 300);
									}
									else {
										alert('Timeout loading slide show editor. You may save the page and re-load it to make the slide show editor appear.');
										limnorHtmlEditor.hideMessage();
									}
									return;
								}
								limnorHtmlEditor.hideMessage();
								_editorOptions.selectedObject = null;
								selectEditElement(o);
							}
							limnorHtmlEditor.showMessage('Loading slide show editor, please wait.');
							showNewObj();
						}
					}
				}
				, {
					tag: 'flash', htmltag: 'embed', name: 'flash player', image: '/libjs/html_flash.png', toolTips: 'play flas video file (*.swf)',
					onCreated: function (o) {
						if (o) {
							if (JsonDataBinding.IsIE()) {
								var p = o.parentNode;
								if (p) {
									var e2 = o.cloneNode(false);
									p.insertBefore(e2, o);
									p.removeChild(o);
									_editorOptions.selectedObject = null;
									selectEditElement(e2);
								}
							}
							else {
								_editorOptions.selectedObject = null;
								selectEditElement(o);
							}
						}
					}
				}
				, {
					tag: 'hitcount', htmltag: 'div', name: 'hit count', pageonly:true,image: '/libjs/html_hitcount.png', toolTips: 'show hit counts of my web home or the page',
					onCreated: function (o) {
						if (o) {
							o.typename = 'hitcount';
							o.setAttribute('typename', 'hitcount');
							o.setAttribute('hidechild', true);
							o.setAttribute('styleshare', 'notshare');
							o.id = HtmlEditor.hitcountid;
							o.style.fontSize = 'small';
							o.style.fontFamily = 'arial';
							o.style.fontWeight = 'normal';
							o.style.color = 'black';
							o.innerHTML = 'Home visits:*****  this page:****';
							o.contentEditable = false;
							_editorOptions.selectedObject = null;
							selectEditElement(o);
						}
					}
				}
				, {
					tag: 'fileupload', htmltag: 'form', name: 'file upload', pageonly: true, image: '/libjs/html_upload.png', toolTips: 'upload a file from web page to web server',
					onCreated: function (o) {
						if (o) {
							o.typename = 'fileupload';
							o.setAttribute('typename', 'fileupload');
							o.setAttribute('hidechild', 'true');
							o.setAttribute('method', 'post');
							o.setAttribute('enctype', 'multipart/form-data');
							var hid = _createElement('input');
							o.appendChild(hid);
							hid.id = _createId(hid);
							hid.setAttribute('type', 'hidden');
							hid.setAttribute('name', 'clientRequest');
							o.setAttribute('clientRequest', hid.id);
							var hidSize = _createElement('input');
							o.appendChild(hidSize);
							hidSize.setAttribute('name', 'MAX_FILE_SIZE');
							hidSize.setAttribute('type', 'hidden');
							hidSize.setAttribute('value', '2048');
							var f = _createElement('input');
							o.appendChild(f);
							var n = 1;
							while (_editorOptions.elementEditedDoc.getElementById('file' + n) && _editorOptions.elementEditedDoc.getElementById('file' + n + 'f')) {
								n++;
							}
							o.id = 'file' + n + 'f';
							f.id = 'file' + n;
							f.setAttribute('type', 'file');
							f.setAttribute('name', f.id);
							//o.setAttribute('clientRequest', f.id);
							if (_editorOptions.forIDE) {
								try {
									if (typeof (limnorStudio) != 'undefined') limnorStudio.onAddFileupload(f.id);
									else window.external.OnAddFileupload(f.id);
								}
								catch (err) {
									alert(err.message);
								}
							}
							_editorOptions.selectedObject = null;
							selectEditElement(o);
						}
					}
				},
				{
					tag: 'drawingGroup', htmltag: 'svg', name: 'drawing group', isSVG: true, pageonly: true, image: '/libjs/html_svg.png', toolTips: 'a container for holding Scalable Vector Graphics',
					onCreated: function (o) {
						if (o) {
							var tryCount = 0;
							o.style.width = '300px';
							o.style.height = '300px';
							o.setAttribute('styleshare', 'NotShare');
							_editorOptions.client.addCustJsLisFile.apply(_editorOptions.editorWindow, ['limnorsvg', o]);
							function showNewObj() {
								if (!o.jsData) {
									tryCount++;
									if (tryCount < 30) {
										setTimeout(showNewObj, 300);
									}
									else {
										alert('Timeout loading SVG drawing editor. You may save the page and re-load it to make the SVG drawing editor appear.');
										limnorHtmlEditor.hideMessage();
									}
									return;
								}
								limnorHtmlEditor.hideMessage();
								_editorOptions.selectedObject = null;
								selectEditElement(o);
							}
							limnorHtmlEditor.showMessage('Loading SVG drawing editor, please wait.');
							showNewObj();
						}
					}
				}
			];
		} //end of getDefaultAddables
		function getDefaultCommands() {
			//useUI and value are optional
			return [
				{ cmd: 'bold', actImg: '/libjs/bold_act.png', inactImg: '/libjs/bold.png', tooltips: 'Toggle bold face for the selected text or the element under the cursor' }
				, { cmd: 'italic', actImg: '/libjs/italic_act.png', inactImg: '/libjs/italic.png', tooltips: 'Toggle italic for the selected text or the element under the cursor' }
				, { cmd: 'underline', actImg: '/libjs/underline_act.png', inactImg: '/libjs/underline.png', tooltips: 'Toggle underline for the selected text or the element under the cursor' }
				, { cmd: 'strikethrough', actImg: '/libjs/strikethrough_act.png', inactImg: '/libjs/strikethrough.png', tooltips: 'Toggle strike-through for the selected text' }
				, { cmd: 'subscript', actImg: '/libjs/subscript_act.png', inactImg: '/libjs/subscript.png', tooltips: 'Toggle subscript for the selected text' }
				, { cmd: 'superscript', actImg: '/libjs/superscript_act.png', inactImg: '/libjs/superscript.png', tooltips: 'Toggle superscript for the selected text' }

				, { cmd: 'alignLeft', actImg: '/libjs/alignLeft.png', inactImg: '/libjs/alignLeftInact.png', isCust: true, tooltips: 'text left alignment.' }
				, { cmd: 'alignCenter', actImg: '/libjs/alignCenter.png', inactImg: '/libjs/alignCenterInact.png', isCust: true, tooltips: 'text center alignment.' }
				, { cmd: 'alignRight', actImg: '/libjs/alignRight.png', inactImg: '/libjs/alignRightInact.png', isCust: true, tooltips: 'text right alignment.' }

				, { cmd: 'indent', actImg: '/libjs/indent.png', inactImg: '/libjs/indent.png', isCust: true, tooltips: 'Make current list item the sub item of its previous sibling.' }
				, { cmd: 'dedent', actImg: '/libjs/dedent.png', inactImg: '/libjs/dedent.png', isCust: true, tooltips: 'Make the current list item a sibling of its parent.' }
				, { cmd: 'moveup', actImg: '/libjs/moveup.png', inactImg: '/libjs/moveup.png', isCust: true, tooltips: 'Moves the current list item before its previous sibling.' }
				, { cmd: 'movedown', actImg: '/libjs/movedown.png', inactImg: '/libjs/movedown.png', isCust: true, tooltips: 'Moves the current list item below its sibling.' }
				, { cmd: 'createlink', actImg: '/libjs/html_a.png', inactImg: '/libjs/html_a.png', tooltips: 'Set hyper link for the selected text' }
			];
		}
		_init();
		function _initPageElement(obj) {
			if (obj && obj.jsData && obj.jsData.typename == 'treeview') {
				obj.jsData.createSubEditor = function(o, e) {
					if (o == e)
						return o;
					if (e && e.tagName && e.tagName.toLowerCase() == 'li') {
						return createTreeViewItemPropDescs(obj, e);
					}
				};
			}
		}
		return {
			removePropertyFromCssText: function (css, name) {
				return removeFromCSStext(css, name);
			},
			editorWindow: function () {
				return _editorWindow;
			},
			promptClose: function (saved) {
				askClose(saved);
			},
			close: function () {
				_close();
			},
			showEditor: function (display) {
				_showEditor(display);
			},
			togglePropertiesWindow: function () {
				_togglePropertiesWindow();
			},
			refreshProperties: function () {
				_refreshProperties();
			},
			getTableMap: function (rowHolder) {
				return _getTableMap(rowHolder);
			},
			addSupportedElements: function (supportedElements) {
				_addEditors(supportedElements);
			},
			removeEditor: function () {
				removeEditorElements();
			},
			getHtmlString: function () {
				return _getHtmlString();
			},
			saveAndFinish: function () {
				_saveAndFinish();
			},
			pageModified: function () {
				return _pageModified();
			},
			setDocType: function (docType) {
				_setDocType(docType);
			},
			getSelectedObject: function () {
				return _getSelectedObject();
			},
			setSelectedObject: function (guid) {
				_setSelectedObject(guid);
			},
			createOrGetId: function (guid) {
				return _createOrGetId(guid);
			},
			notifyUsedNames: function (usedNames) {
				_notifyUsedNames(usedNames);
			},
			getNamesUsed: function () {
				return _getNamesUsed();
			},
			setPropertyValue: function (propertyName, propertyValue) {
				return _setPropertyValue(propertyName, propertyValue);
			},
			appendArchiveFile: function (objId, filePath) {
				return _appendArchiveFile(objId, filePath);
			},
			setGuid: function (guid) {
				return _setGuid(guid);
			},
			getIdList: function () {
				return _getIdList();
			},
			getIdByGuid: function (guid) {
				return _getIdByGuid(guid);
			},
			setGuidById: function (id, guid) {
				return _setGuidById(id, guid);
			},
			getMaps: function () {
				return _getMaps();
			},
			getAreas: function (mapId) {
				return _getAreas(mapId);
			},
			createNewMap: function (guid) {
				return _createNewMap(guid);
			},
			setMapAreas: function (mapId, arealist) {
				_setMapAreas(mapId, arealist);
			},
			setUseMap: function (imgId, mapId) {
				_setUseMap(imgId, mapId);
			},
			setbodyBk: function (bkFile, bkTile) {
				return _setbodyBk(bkFile, bkTile);
			},
			onBeforeIDErun: function () {
				_onBeforeIDErun();
			},
			doCopy: function () {
				_doCopy();
			},
			doPaste: function () {
				_doPaste();
			},
			pasteToHtmlInput: function (txt, selStart, selEnd) {
				_pasteToHtmlInput(txt, selStart, selEnd);
			},
			initSelection: function () {
				_initSelection();
			},
			setEditOptions: function (editOptions, verify) {
				if (!editOptions.objEdited) {
					alert('object being edited cannot be null or undefined');
					return;
				}
				editOptions.isEditingBody = !(editOptions.inline);
				editOptions.elementEditedWindow = editOptions.objEdited; //objEdited is a window
				editOptions.elementEditedDoc = editOptions.elementEditedWindow.document;
				if (editOptions.isEditingBody) {
					editOptions.elementEdited = editOptions.elementEditedDoc.body;
				}
				if ('htmlFileOption' in editOptions) {
				}
				else {
					editOptions.htmlFileOption = 0;
				}
				_setEditorTarget(editOptions);
			},
			setInlineTarget: function (divEditor) {
				_setInlineTarget(divEditor);
			},
			bringToFront: function () {
				_bringToFront();
			},
			onclientmousedown: function (obj,x,y, isRightClick) {
				_onclientmousedown(obj,x,y, isRightClick);
			},
			onclientmousemove: function (x, y, ph) {
				_onclientmousemove(x, y, ph);
			},
			onclientmouseup: function (obj, ph) {
				_onclientmouseup(obj, ph);
			},
			onclientkeyup: function (obj) {
				_onclientkeyup(obj);
			},
			setChanged: function () {
				_setChanged();
			},
			onclientBackspace: function () {
				return _handleBackspace(true);
			},
			initPageElement: function (obj) {
				_initPageElement(obj);
			},
			showEditorMessage: function (message) {
				_setMessage(message);
			}
		};
	}
	//end of createEditor
};
