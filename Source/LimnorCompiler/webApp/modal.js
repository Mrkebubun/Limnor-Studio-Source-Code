/*
	Html Editor Library -- JavaScript
	Copyright Longflow Enterprises Ltd
	2011

*/
if (typeof JsonDataBinding != 'undefined') {
	JsonDataBinding.DialogueBox = {
		//url, center, top, left, width, height, border, borderWidth, icon, resizable, isDialog, hideCloseButtons, ieBorderOffset
		showDialog: function (dlg) {
			var _divDialog;
			var _frmDialog;
			var _docDialog;
			var _frmWait;
			var _docWait;
			var _tdTitle;
			var _imgIcon;
			var _resizer;
			var _api;
			var _imgOK;
			var _imgCancel;
			var _onBringToFront;
			var _isIE = JsonDataBinding.IsIE();
			//var _isOpera = JsonDataBinding.IsOpera();
			function _showResizer(visible) {
				if (_resizer) {
					if (visible) {
						var pos = JsonDataBinding.ElementPosition.getElementPosition(_divDialog);
						var zi = JsonDataBinding.getZOrder(_divDialog) + 1;
						var ieOffset = _isIE ? (dlg.ieBorderOffset ? dlg.ieBorderOffset : 0) : 0;
						_resizer.style.zIndex = zi;
						_resizer.style.display = 'block';
						_resizer.style.left = (pos.x + _divDialog.offsetWidth - _resizer.offsetWidth - ieOffset) + 'px';
						_resizer.style.top = (pos.y + _divDialog.offsetHeight - _resizer.offsetHeight - ieOffset) + 'px';
					}
					else {
						_resizer.style.display = 'none';
					}
				}
			}
			function _bringToFront() {
				var z = JsonDataBinding.getPageZIndex(_divDialog) + 1;
				var z2 = z + 1;
				_divDialog.style.display = 'block';
				_divDialog.style.zIndex = z;
				_resizer.style.zIndex = z2;
				_showResizer(dlg.resizable);
				if (_onBringToFront) {
					_onBringToFront();
				}
			}
			function _hide(ret, close) {
				if (close) {
					try {
						_docDialog = _frmDialog.contentDocument ? _frmDialog.contentDocument : _frmDialog.contentWindow.document;
						if (ret) {
							if (_docDialog.childManager && _docDialog.childManager.CloseDialogPrompt && _docDialog.childManager.CloseDialogPrompt.length > 0) {
								if (confirm(_docDialog.childManager.CloseDialogPrompt)) {
									_docDialog.dialogResult = 'ok';
								}
								else {
									return;
								}
							}
							document.dialogResult = 'ok';
						}
						else {
							if (_docDialog.childManager && _docDialog.childManager.CancelDialogPrompt && _docDialog.childManager.CancelDialogPrompt.length > 0) {
								if (confirm(_docDialog.childManager.CancelDialogPrompt)) {
									_docDialog.dialogResult = 'cancel';
								}
								else {
									return;
								}
							}
							document.dialogResult = 'cancel';
						}
						//not close yet. the user may cancel
						_docDialog.dialogResult = document.dialogResult;
						if (_docDialog.childManager && _docDialog.childManager.onClosingWindow) {
							_docDialog.childManager.onClosingWindow();
							if (_docDialog.dialogResult == '') {
								return;
							}
						}
					}
					catch (er) {
					}
					dlg.finished = true;
					_api.finished = true;
					if (dlg.isDialog && dlg.onDialogClose) {
						dlg.onDialogClose();
					}
					_frmDialog.src = "about:blank";
					_divDialog.style.display = 'none';
					_frmWait.style.display = 'none';
					_resizer.style.display = 'none';
					if (JsonDataBinding.IsIE()) {
						_frmDialog.parentNode.innerHTML = '';
						document.body.removeChild(_divDialog);
						document.body.removeChild(_frmWait);
						document.body.removeChild(_resizer);
					}
					else {
						if (_frmWait && _frmWait.parentNode) {
							_frmWait.parentNode.removeChild(_frmWait);
						}
						if (_resizer && _resizer.parentNode) {
							_resizer.parentNode.removeChild(_resizer);
						}
						if (_divDialog && _divDialog.parentNode) {
							_divDialog.parentNode.removeChild(_divDialog);
						}
					}
					document.childManager.remove(_api);
				}
				else {
					_divDialog.style.display = 'none';
					_frmWait.style.display = 'none';
					_resizer.style.display = 'none';
				}
			}
			//
			function onOK(e) {
				_hide(true, true);
			}
			function onCancel(e) {
				_hide(false, true);
			}
			function docmousedown(e) {
				e = e || window.event;
				var x, y;
				var c = JsonDataBinding.getSender(e);
				if (c == _resizer) {
					x = _resizer.offsetLeft;
					y = _resizer.offsetTop;
					_resizer.ox = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - x;
					_resizer.oy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - y;
					_resizer.isMousedown = true;
					_divDialog.isMousedown = false;
					_bringToFront();
					return false;
				}
				else if (c == _tdTitle) {
					x = _divDialog.offsetLeft;
					y = _divDialog.offsetTop;
					_divDialog.ox = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - x;
					_divDialog.oy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - y;
					_divDialog.isMousedown = true;
					_resizer.isMousedown = false;
					_bringToFront();
					if (_isIE) {
						if (_resizer) {
							_resizer.style.left = '-30px';
						}
					}
					else {
						_showResizer(false);
					}
					return false;
				}
				else {
					_divDialog.isMousedown = false;
					_resizer.isMousedown = false;
				}
			}
			function docmouseup(e) {
				_divDialog.isMousedown = false;
				_resizer.isMousedown = false;
				_showResizer(dlg.resizable);
				return true;
			}
			function dlgdocMouseDown(e) {
				_bringToFront();
			}
			function dlgdocmousemove(e) {
				e = e || _frmDialog.contentWindow.event;
				var x = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x);
				var y = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y);
				if (_resizer.isMousedown || _divDialog.isMousedown) {
					var dx = _divDialog.offsetLeft;
					var dy = _divDialog.offsetTop;
					docmousemove.apply(window, [{ pageX: x + dx, pageY: y + dy, target: _resizer }]);
				}
			}
			function waitdocmousemove(e) {
				e = e || _frmWait.contentWindow.event;
				var x = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x);
				var y = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y);
				if (_resizer.isMousedown || _divDialog.isMousedown) {
					docmousemove({ pageX: x, pageY: y, target: _resizer });
				}
			}
			function docmousemove(e) {
				var diffx, diffy;
				var obj, o;
				var isInDlg;
				var dx = 0;
				var dy = 0;
				if (_resizer.isMousedown) {
					e = e || window.event;
					if (e) {
						//dx=dy=0: event from _resizer
						diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _resizer.ox + dx;
						diffy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - _resizer.oy + dy;
						_resizer.style.left = diffx > 0 ? diffx + 'px' : '0px';
						_resizer.style.top = diffy > 0 ? diffy + 'px' : '0px';
						var p1 = JsonDataBinding.ElementPosition.getElementPosition(_divDialog);
						var p2 = JsonDataBinding.ElementPosition.getElementPosition(_resizer);
						var w = p2.x - p1.x + _resizer.offsetWidth;
						var h = p2.y - p1.y + _resizer.offsetHeight;
						if (w > 120) {
							_divDialog.style.width = w + 'px';
						}
						if (h > 60) {
							_divDialog.style.height = h + 'px';
						}
					}
				}
				else if (_divDialog.isMousedown) {
					e = e || window.event;
					if (!e) {
						_docWait = _frmWait.contentDocument ? _frmWait.contentDocument : _frmWait.contentWindow.document;
						e = _frmWait.contentWindow.event;
						if (!e) {
							_docDialog = _frmDialog.contentDocument ? _frmDialog.contentDocument : _frmDialog.contentWindow.document;
							e = _frmDialog.contentWindow.event;
						}
					}
					if (e) {
						diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _divDialog.ox + dx;
						diffy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - _divDialog.oy + dy;
						_divDialog.style.left = diffx > 0 ? diffx + 'px' : '0px';
						_divDialog.style.top = diffy > 0 ? diffy + 'px' : '0px';
					}
				}
			}
			function attachWaitFrmEvent() {
				var ntryCount = 0;
				var started = false;
				function _tryAttach() {
					if (_frmWait) {
						_docWait = _frmWait.contentDocument ? _frmWait.contentDocument : (_frmWait.contentWindow ? _frmWait.contentWindow.document : null);
						if (_docWait) {
							if (_docWait.readyState == 'interactive') {
								ntryCount++;
								if (ntryCount > 2) {
									started = true;
								}
							}
							if (started || _docWait.readyState == 'complete') {
								JsonDataBinding.AttachEvent(_docWait, "onmouseup", docmouseup);
								JsonDataBinding.AttachEvent(_docWait, "onmousemove", waitdocmousemove);
								_docWait.body.style.background = '#000';
							}
							else {
								setTimeout(_tryAttach, 300);
							}
						}
					}
				}
				_tryAttach();
			}
			function _onDocMouseDown(e) {
				_bringToFront();
			}
			//called through api
			function _onDocMouseMove(e) {
				var dx, dy, diffx, diffy;
				if (_resizer.isMousedown) {
					e = e || window.event;
					if (e) {
						dx = _divDialog.offsetLeft - _resizer.offsetWidth;
						dy = _divDialog.offsetTop - _resizer.offsetHeight;
						diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _resizer.ox + dx;
						diffy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - _resizer.oy + dy;
						_resizer.style.left = diffx > 0 ? diffx + 'px' : '0px';
						_resizer.style.top = diffy > 0 ? diffy + 'px' : '0px';
						var p1 = JsonDataBinding.ElementPosition.getElementPosition(_divDialog);
						var p2 = JsonDataBinding.ElementPosition.getElementPosition(_resizer);
						var w = p2.x - p1.x;
						var h = p2.y - p1.y;
						if (w > 120) {
							_divDialog.style.width = w + 'px';
						}
						if (h > 60) {
							_divDialog.style.height = h + 'px';
						}
					}
				}
				else if (_divDialog.isMousedown) {
					e = e || window.event;
					if (e) {
						dx = _divDialog.offsetLeft;
						dy = _divDialog.offsetTop;
						diffx = (e.pageX ? e.pageX : e.clientX ? e.clientX : e.x) - _divDialog.ox + dx;
						diffy = (e.pageY ? e.pageY : e.clientY ? e.clientY : e.y) - _divDialog.oy + dy;
						_divDialog.style.left = diffx > 0 ? diffx + 'px' : '0px';
						_divDialog.style.top = diffy > 0 ? diffy + 'px' : '0px';
					}
				}
			}
			function _onDocMouseUp(e) {
				_divDialog.isMousedown = false;
				_resizer.isMousedown = false;
				_showResizer(dlg.resizable);
				return true;
			}

			function attachDlgDocEvent() {
				var ntryCount = 0;
				var started = false;
				function _tryAttach() {
					try {
						if (_frmDialog) {
							_docDialog = _frmDialog.contentDocument ? _frmDialog.contentDocument : (_frmDialog.contentWindow ? _frmDialog.contentWindow.document : null);
							if (_docDialog) {
								if (_docDialog.readyState == 'interactive') {
									ntryCount++;
									if (ntryCount > 2) {
										started = true;
									}
								}
								if (started || (_docDialog.readyState == 'complete' && (dlg.url == 'about:blank' || dlg.url == null || _docDialog.URL != 'about:blank'))) {
									JsonDataBinding.AttachEvent.apply(_frmDialog.contentWindow, [_docDialog, "onmousedown", dlgdocMouseDown]);
									JsonDataBinding.AttachEvent.apply(_frmDialog.contentWindow, [_docDialog, "onmouseup", docmouseup]);
									JsonDataBinding.AttachEvent.apply(_frmDialog.contentWindow, [_docDialog, "onmousemove", dlgdocmousemove]);
									if (dlg.isDialog) {
										document.currentDialog = _api;
									}
									_docDialog.childApi = _api;
								}
								else {
									setTimeout(_tryAttach, 300);
								}
							}
						}
					}
					catch (er) {
					}
				}
				_tryAttach();
			}
			//3 body children: _divDialog, _frmWait, _resizer
			//--------------------
			//_divDialog
			//  tbl--------------
			//    tr title bar: icon, _tdTitle, _imgOK, _imgCancel
			//    tr page:_frmDialog
			//--------------------
			if (!_divDialog) {
				_divDialog = document.createElement('div');
				if (dlg.contentsHolder) {
					_divDialog.style.position = "static";
					_divDialog.style.left = '0px';
					_divDialog.style.top = '0px';
					_divDialog.style.width = '100%';
					_divDialog.style.height = '100%';
					_divDialog.style.overflowY = 'scroll';
					dlg.contentsHolder.appendChild(_divDialog);
				}
				else {
					document.body.appendChild(_divDialog);
					_divDialog.style.position = "absolute";
					_divDialog.style.opacity = ".99";
					_divDialog.style.filter = "alpha(opacity=99)";
					_divDialog.style.overflowY = 'hidden';
				}
				_divDialog.style.backgroundColor = "#000";
				_divDialog.style.overflow = 'hidden';
				//
				var tbl = document.createElement('table');
				tbl.style.backgroundColor = "white";
				tbl.border = 0;
				tbl.style.padding = 0;
				tbl.cellPadding = 0;
				tbl.cellSpacing = 0;
				tbl.style.width = '100%';
				tbl.style.height = '100%';
				_divDialog.appendChild(tbl);
				//
				var tbody = JsonDataBinding.getTableBody(tbl);
				//title bar
				var tr = document.createElement('tr');
				tbody.appendChild(tr);
				var td = document.createElement('td');
				tr.appendChild(td);
				_imgIcon = document.createElement("img");
				_imgIcon.border = 0;
				_imgIcon.src = 'libjs/dlg.png';
				td.appendChild(_imgIcon);
				//
				td = document.createElement('td');
				tr.appendChild(td);
				td.style.cssText = "width:100%; cursor:move;";
				td.innerHTML = 'Title';
				_tdTitle = td;
				tr.style.height = "20px";
				tr.style.backgroundColor = "#A0CFEC";
				//
				td = document.createElement('td');
				td.style.width = '16px';
				tr.appendChild(td);
				_imgOK = document.createElement("img");
				_imgOK.src = "libjs/ok.png";
				_imgOK.style.cursor = "pointer";
				_imgOK.style.display = 'inline';
				td.appendChild(_imgOK);
				td = document.createElement('td');
				td.style.width = '16px';
				tr.appendChild(td);
				_imgCancel = document.createElement("img");
				_imgCancel.src = "libjs/cancel.png";
				_imgCancel.style.cursor = "pointer";
				_imgCancel.style.display = 'inline';
				td.appendChild(_imgCancel);
				//===contents===
				tr = document.createElement('tr');
				tbody.appendChild(tr);
				td = document.createElement('td');
				td.colSpan = 4; //icon,title,ok,cancel
				tr.appendChild(td);
				td.style.width = "100%";
				td.style.height = "100%";
				tr.style.width = "100%";
				tr.style.height = '100%';
				//
				_frmDialog = document.createElement('iframe');
				_frmDialog.limnorDialog = true;
				_frmDialog.style.height = '100%';
				_frmDialog.style.width = '100%';
				_frmDialog.border = 0;
				_frmDialog.style.border = 'none';
				_frmDialog.style.overflow = 'hidden';
				//_frmDialog.style.backgroundColor = 'green';
				td.appendChild(_frmDialog);
				//===background==========
				_frmWait = document.createElement('iframe');
				document.body.appendChild(_frmWait);
				_frmWait.style.position = "absolute";
				_frmWait.style.top = '0px';
				_frmWait.style.left = '0px';
				_frmWait.style.height = '100%';
				_frmWait.style.width = '100%';
				_frmWait.style.border = 'none';
				_frmWait.style.opacity = '0.10';
				_frmWait.style.filter = 'alpha(opacity=10)';
				//
				_resizer = document.createElement("img");
				_resizer.style.position = 'absolute';
				_resizer.style.display = 'none';
				_resizer.style.cursor = 'nw-resize';
				_resizer.src = 'libjs/resizer.gif';
				_resizer.ondragstart = function () { return false; };
				if (JsonDataBinding.IsIE()) {
					_resizer.onresizestart = function () { return false; };
					_resizer.setAttribute("unselectable", "on");
				}
				document.body.appendChild(_resizer);
				//
				JsonDataBinding.AttachEvent(document, "onmousedown", docmousedown);
				JsonDataBinding.AttachEvent(document, "onmouseup", docmouseup);
				JsonDataBinding.AttachEvent(document, "onmousemove", docmousemove);
				JsonDataBinding.AttachEvent(_imgOK, "onclick", onOK);
				JsonDataBinding.AttachEvent(_imgCancel, "onclick", onCancel);
				//
				attachWaitFrmEvent();
				//
			}
			if (typeof dlg.overflow != 'undefined') {
				_frmDialog.style.overflow = dlg.overflow;
			}
			dlg.finished = false;
			var zi = JsonDataBinding.getPageZIndex() + 1;
			_frmWait.style.zIndex = zi;
			//
			if (dlg.icon && dlg.icon.length > 0) {
				_imgIcon.src = dlg.icon;
			}
			else {
				//_imgIcon.src = 'libjs/dlg.png';
				_imgIcon.style.display = 'none';
			}
			if (dlg.title) {
				_tdTitle.innerHTML = dlg.title;
			}
			else {
				_tdTitle.innerHTML = '';
			}
			if (dlg.hideTitle) {
				_tdTitle.parentNode.style.display = 'none';
			}
			if (dlg.titleBkColor) {
				_tdTitle.style.backgroundColor = dlg.titleBkColor;
			}
			if (dlg.titleColor) {
				_tdTitle.style.color = dlg.titleColor;
			}
			if (dlg.iconClose) {
				_imgCancel.src = dlg.iconClose;
			}
			if (dlg.iconClose) {
				_imgOK.src = dlg.iconOK;
			}
			//
			if (dlg.border)
				_divDialog.style.border = dlg.border;
			else
				_divDialog.style.border = "1px solid gray";
			if (dlg.borderWidth)
				_divDialog.style.borderWidth = dlg.borderWidth;
			else
				_divDialog.style.borderWidth = "thin";
			//
			if (dlg.url) {
				_frmDialog.src = dlg.url + (dlg.forceNew ? '?fnew=' + (Math.random()) : '');
			}
			else {
				_frmDialog.src = '';
			}
			zi++;
			_divDialog.style.zIndex = zi;
			_divDialog.style.display = "block";
			if (typeof dlg.width == 'undefined' || dlg.width == 0)
				_divDialog.style.width = "600px";
			else {
				if (dlg.width) {
					if (JsonDataBinding.isNumber(dlg.width)) {
						_divDialog.style.width = dlg.width + 'px';
					}
					else {
						_divDialog.style.width = dlg.width;
					}
				}
			}
			if (typeof dlg.height == 'undefined' || dlg.height == 0)
				_divDialog.style.height = "500px";
			else {
				if (dlg.height) {
					if (JsonDataBinding.isNumber(dlg.height)) {
						_divDialog.style.height = dlg.height + 'px';
					}
					else {
						_divDialog.style.height = dlg.height;
					}
				}
			}
			if (dlg.isDialog) {
				_frmWait.style.height = '100%';
				_frmWait.style.width = '100%';
				_frmWait.style.display = "block";
				_imgOK.style.display = "inline";
			}
			else {
				_frmWait.style.display = "none";
				_imgOK.style.display = "none";
			}
			if (dlg.hideCloseButtons == 1 || dlg.hideCloseButtons == 'HideOKCancel') {
				_imgOK.style.display = "none";
				_imgCancel.style.display = "none";
			}
			else if (dlg.hideCloseButtons == 2 || dlg.hideCloseButtons == 'HideOK') {
				_imgOK.style.display = "none";
			}
			else if (dlg.hideCloseButtons == 3 || dlg.hideCloseButtons == 'HideCancel') {
				_imgCancel.style.display = "none";
			}
			if (dlg.center) {
				JsonDataBinding.windowTools.centerElementOnScreen(_divDialog);
			}
			else {
				if (dlg.top) {
					_divDialog.style.top = dlg.top + 'px';
				}
				else {
					_divDialog.style.top = '0px';
				}
				if (dlg.left) {
					_divDialog.style.left = dlg.left + 'px';
				}
				else {
					_divDialog.style.left = '0px';
				}
			}
			_showResizer(dlg.resizable);
			//
			function _closeDialog() {
				_hide(true, true);
			}
			function _cancelDialog() {
				_hide(false, true);
			}
			function _hideWindow() {
				_hide(false, false);
			}
			function _getPageDoc() {
				if (_docDialog) {
					return _docDialog;
				}
				if (_frmDialog) {
					if (_frmDialog.contentDocument) {
						_docDialog = _frmDialog.contentDocument;
						return _docDialog;
					}
					if (_frmDialog.contentWindow) {
						_docDialog = _frmDialog.contentWindow.document;
						return _docDialog;
					}
				}
			}
			function _getPageWindow() {
				if (_frmDialog) {
					return _frmDialog.contentWindow;
				}
			}
			function _getChildElement(id) {
				var d = _getPageDoc();
				_docDialog = d || _docDialog;
				return _docDialog.getElementById(id);
			}
			function _getPageId() {
				var msg;
				try {
					var d = _getPageDoc();
					_docDialog = d || _docDialog;
					if (_docDialog && _docDialog.pageId) {
						return _docDialog.pageId;
					}
				}
				catch (e) {
					msg = (e.message ? e.message : e);
				}
				try {
					if (dlg.pageId) {
						return dlg.pageId;
					}
				}
				catch (e) {
					msg = (e.message ? e.message : e);
				}
				if (msg) {
					alert('Error accessing dialogues. The page might not working properly. ' + msg);
				}
			}

			function _isDialog() {
				return dlg.isDialog;
			}
			function _hasFinished() {
				return dlg.finished;
			}
			function _isVisible() {
				return _divDialog.style.display != 'none';
			}
			function _isSamePage(p) {
				return p.url == dlg.url;
			}
			function _getPageUrl() {
				return dlg.url;
			}
			function _setOnBringToFront(h) {
				_onBringToFront = h;
			}
			function _displayResizer() {
				_showResizer(dlg.resizable);
			}
			//
			_api = {
				getPageId: function () {
					return _getPageId();
				},
				getPageUrl: function () {
					return _getPageUrl();
				},
				getPageDoc: function () {
					return _getPageDoc();
				},
				getPageWindow: function () {
					return _getPageWindow();
				},
				closeDialog: function () {
					_closeDialog();
				},
				cancelDialog: function () {
					_cancelDialog();
				},
				hideWindow: function () {
					_hideWindow();
				},
				getChildElement: function (id) {
					return _getChildElement(id);
				},
				bringToFront: function () {
					_bringToFront();
				},
				isDialog: function () {
					return _isDialog();
				},
				isVisible: function () {
					return _isVisible();
				},
				hasFinished: function () {
					return _hasFinished();
				},
				isSamePage: function (p) {
					return _isSamePage(p);
				},
				onDocMouseDown: function (e) {
					_onDocMouseDown(e);
				},
				onDocMouseMove: function (e) {
					_onDocMouseMove(e);
				},
				onDocMouseUp: function (e) {
					_onDocMouseUp(e);
				},
				isDocLoaded: function () {
					if (_frmDialog) {
						if (_frmDialog.contentWindow) {
							if (_frmDialog.contentWindow.document) {
								return _frmDialog.contentWindow.document.readyState == 'complete';
							}
						}
					}
					return false;
				},
				onDocLoaded: function () {
					_displayResizer();
				},
				setOnBringToFront: function (h) {
					_setOnBringToFront(h);
				},
				getPageHolder: function () {
					return _divDialog;
				}
			};
			_divDialog.jsData = _api;
			attachDlgDocEvent();
			if (dlg.resizable) {
				setTimeout(function () { _showResizer(true); }, 10);
			}
			return _api;
		}
	};
	JsonDataBinding.childWindows = function () {
		var _childObjs;
		function _showChild(p) {
			if (!_childObjs) {
				_childObjs = new Array();
			}
			var i;
			for (i = 0; i < _childObjs.length; i++) {
				if (_childObjs[i].hasFinished()) {
					_childObjs.splice(i, 1);
					break;
				}
			}
			for (i = 0; i < _childObjs.length; i++) {
				if (_childObjs[i].isDialog()) {
					return;
				}
			}
			var o;
			for (i = 0; i < _childObjs.length; i++) {
				if (_childObjs[i].isSamePage(p)) {
					_childObjs[i].bringToFront();
					o = _childObjs[i];
					break;
				}
			}
			if (!o) {
				o = JsonDataBinding.DialogueBox.showDialog(p);
				_childObjs.push(o);
			}

			function checkChildReady() {
				try {
					if (o.isDocLoaded()) {
						o.onDocLoaded();
						if (document.childManager.onChildWindowReady) {
							document.childManager.onChildWindowReady({ target: o });
						}
						return;
					}
					setTimeout(checkChildReady, 100);
				}
				catch (er) {
				}
			}
			setTimeout(checkChildReady, 100);
			return o;
		}
		function _getDocById(pid) {
			if (_childObjs) {
				for (i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageId() == pid) {
						var w = _childObjs[i].getPageWindow();
						if (w) {
							return w.document;
						}
					}
				}
			}
		}
		function _getWindowById(pid) {
			if (_childObjs) {
				for (i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageId() == pid) {
						return _childObjs[i].getPageWindow();
					}
				}
			}
		}
		function _hideChildbyUrl(url) {
			if (_childObjs) {
				var o;
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageUrl() == url) {
						o = _childObjs[i];
						break;
					}
				}
				if (o) {
					o.hideWindow();
				}
			}
		}
		function _closeChildByUrl(url) {
			if (_childObjs) {
				var o;
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageUrl() == url) {
						o = _childObjs[i];
						break;
					}
				}
				if (o) {
					o.cancelDialog();
				}
			}
		}
		function _remove(o) {
			if (_childObjs) {
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i] == o) {
						_childObjs.splice(i, 1);
						break;
					}
				}
			}
		}
		function _closeAll(ws) {
			if (_childObjs) {
				if (!ws)
					ws = new Array();
				for (var i = 0; i < _childObjs.length; i++) {
					ws.push(_childObjs[i]);
				}
				for (var i = 0; i < ws.length; i++) {
					ws[i].closeDialog();
				}
			}
		}
		function _confirmDialog(pid) {
			if (_childObjs) {
				var o;
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageId() == pid) {
						o = _childObjs[i];
						break;
					}
				}
				if (o) {
					o.closeDialog();
				}
			}
		}
		function _closeWindow(pid) {
			if (_childObjs) {
				var o;
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageId() == pid) {
						o = _childObjs[i];
						break;
					}
				}
				if (o) {
					o.cancelDialog();
					return true;
				}
			}
		}
		function _hideWindow(pid) {
			if (_childObjs) {
				var o;
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageId() == pid) {
						o = _childObjs[i];
						break;
					}
				}
				if (o) {
					o.hideWindow();
				}
			}
		}
		function _getChildByPageId(pid) {
			if (_childObjs) {
				for (var i = 0; i < _childObjs.length; i++) {
					if (_childObjs[i].getPageId() == pid) {
						return _childObjs[i];
					}
				}
			}
		}
		function _onChildMouseDown(pid, e) {
			var api = _getChildByPageId(pid);
			if (api) {
				api.onDocMouseDown(e);
			}
		}
		function _onChildMouseMove(pid, e) {
			var api = _getChildByPageId(pid);
			if (api) {
				api.onDocMouseMove(e);
			}
		}
		function _onChildMouseUp(pid, e) {
			var api = _getChildByPageId(pid);
			if (api) {
				api.onDocMouseUp(e);
			}
		}
		function _getProperty(name) {
			return JsonDataBinding.PageValues[name];
		}
		function _setProperty(name, val) {
			JsonDataBinding.PageValues[name] = val;
		}
		function _getServerProperty(name) {
			return JsonDataBinding.values[name];
		}
		function _execute(methodName, args) {
			var m = window[methodName];
			if (!m) {
				if (limnorPage) {
					m = limnorPage[methodName];
				}
			}
			if (m) {
				return m.apply(window, args);
			}
		}
		return {
			showChild: function (p) {
				return _showChild(p);
			},
			hideChildbyUrl: function (url) {
				_hideChildbyUrl(url);
			},
			closeChildByUrl: function (url) {
				_closeChildByUrl(url);
			},
			remove: function (o) {
				_remove(o);
			},
			closeWindow: function (pid) {
				_closeWindow(pid);
			},
			confirmDialog: function (pid) {
				_confirmDialog(pid);
			},
			hideWindow: function (pid) {
				_hideWindow(pid);
			},
			closeAll: function () {
				_closeAll();
			},
			onChildMouseDown: function (pid, e) {
				_onChildMouseDown(pid, e);
			},
			onChildDocMouseMove: function (pid, e) {
				_onChildMouseMove(pid, e);
			},
			onChildDocMouseUp: function (pid, e) {
				_onChildMouseUp(pid, e);
			},
			getDocById: function (pid) {
				return _getDocById(pid);
			},
			getWindowById: function (pid) {
				return _getWindowById(pid);
			},
			CloseDialogPrompt: '',
			CancelDialogPrompt: '',
			onClosingWindow: null,
			onChildWindowReady: null,
			getProperty: function (name) {
				return _getProperty(name);
			},
			setProperty: function (name, val) {
				return _setProperty(name, val);
			},
			getServerProperty: function (name) {
				return _getServerProperty(name);
			},
			execute: function (methodName, args) {
				return _execute(methodName, args);
			},
			createArray: function () {
				return new Array();
			},
			executeOne: function (methodName, arg) {
				var args = new Array();
				args.push(arg);
				return _execute(methodName, args);
			}
		}
	};
	JsonDataBinding.showChild = function (p) {
		return document.childManager.showChild(p);
	};
	JsonDataBinding.getDocById = function (pid) {
		return document.childManager.getDocById(pid);
	};
	JsonDataBinding.getChildWindowById = function (pid) {
		return document.childManager.getWindowById(pid);
	};
	JsonDataBinding.hideChild = function (url) {
		return document.childManager.hideChildbyUrl(url);
	};
	JsonDataBinding.closeChild = function (url) {
		return document.childManager.closeChildByUrl(url);
	};
	JsonDataBinding.setupChildManager = function () {
		if (!document.childManager) {
			document.childManager = JsonDataBinding.childWindows();
			if (window.parent && window != window.parent) {
				if (!JsonDataBinding.IsFireFox() && !JsonDataBinding.IsIE()) {
					function onDocMouseDown(e) {
						window.parent.document.childManager.onChildMouseDown(document.pageId, e);
					}
					function onDocMouseMove(e) {
						window.parent.document.childManager.onChildDocMouseMove(document.pageId, e);
					}
					function onDocMouseUp(e) {
						window.parent.document.childManager.onChildDocMouseUp(document.pageId, e);
					}
					JsonDataBinding.AttachEvent(document, "onmousedown", onDocMouseDown);
					JsonDataBinding.AttachEvent(document, "onmousemove", onDocMouseMove);
					JsonDataBinding.AttachEvent(document, "onmouseup", onDocMouseUp);
				}
			}
			else {

			}
		}
	}
	JsonDataBinding.getPropertyByPageId = function (pid, propertyName) {
		var win = JsonDataBinding.getWindowById(pid);
		if (win) {
			var doc = win.document; // JsonDataBinding.getDocumentById(pid);
			if (doc) {
				var a = doc.childManager.createArray();
				a.push(propertyName);
				return doc.childManager.getProperty.apply(win, a);
			}
		}
	}
	JsonDataBinding.setPropertyByPageId = function (pid, propertyName, val) {
		var doc = JsonDataBinding.getDocumentById(pid);
		if (doc) {
			return doc.childManager.setProperty(propertyName, val);
		}
	}
	JsonDataBinding.getServerPropertyByPageId = function (pid, propertyName) {
		var doc = JsonDataBinding.getDocumentById(pid);
		if (doc) {
			return doc.childManager.getServerProperty(propertyName);
		}
	}
	JsonDataBinding.executeByPageId = function (pid, methodName) {
		var doc = JsonDataBinding.getDocumentById(pid);
		if (doc) {
			var a = doc.childManager.createArray();
			for (var i = 2; i < arguments.length; i++) {
				a.push(arguments[i]);
			}
			return doc.childManager.execute(methodName, a);
		}
		else {
			//JsonDataBinding.ShowDebugInfoLine('page with id [' + pid + '] not found from [' + window.location.href + '] for executing method [' + methodName + ']');
		}
	}
	JsonDataBinding.hidePage = function () {
		if (window != window.parent) {
			if (window.parent.document.childManager) {
				window.parent.document.childManager.hideWindow(document.pageId);
			}
		}
	}
	JsonDataBinding.closePage = function () {
		if (window != window.parent) {
			if (window.parent.document.childManager) {
				if (!window.parent.document.childManager.closeWindow(document.pageId)) {
					try {
						window.parent.document.childManager.closeAll();
					}
					catch (e) {
					}
				}
			}
		}
	}
	JsonDataBinding.confirmDialog = function () {
		if (window != window.parent) {
			if (window.parent.document.childManager) {
				window.parent.document.childManager.confirmDialog(document.pageId);
			}
		}
	}
	JsonDataBinding.UploadFile = function (targetFolder, onFinish) {
	}
}