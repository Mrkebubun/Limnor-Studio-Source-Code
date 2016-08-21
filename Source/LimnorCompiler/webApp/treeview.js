//htmlElement: a div for holding the treeview.
JsonDataBinding.CreateTreeView = function(htmlElement) {
	var _div = htmlElement;
	var _nodeSelectionHandlers = new Array();
	var _readOnly = true;
	var _jsTable;
	var _loadingMsg;
	//var _existOnvalueChange;
	var nodenameIdx = -1;
	var textIdx = -1;
	var imageIdx = -1;
	var nodedataIdx = -1;
	_div.mouseOverColor = '#c0ffc0';
	_div.nodeBackColor = 'White';
	_div.selectedNodeColor = '#c0c0ff';
	_div.isTreeView = true;
	//
	function _selectNode(nd) {
		if (_div.selectedNode) {
			_div.selectedNode.isSelected = false;
			_div.selectedNode.style.backgroundColor = _div.nodeBackColor;
		}
		if (nd == null) {
			return;
		}
		nd.isSelected = true;
		_div.selectedNode = nd;
		nd.style.backgroundColor = _div.selectedNodeColor;
		if (typeof nd.rowIndex != 'undefined') {
			if (_jsTable.rowIndex != nd.rowIndex) {
				JsonDataBinding.dataMoveToRecord(_jsTable.TableName, nd.rowIndex);
			}
		}
		_div.jsData.afterSelectNode();
	}
	function _setCurrentPrimaryKey(parentNode) {
		if (_jsTable) {
			_div.currentPrimaryKeyValue = _jsTable.Rows[parentNode.rowIndex].ItemArray[_jsTable.columnIndexes[_div.primaryKey]];
			JsonDataBinding.setTableAttribute(_jsTable.TableName, 'isDataStreaming', true);
			JsonDataBinding.setTableAttribute(_jsTable.TableName, 'tv_parentnode', parentNode);
			if (typeof _div.currentPrimaryKeyValue == 'undefined' || _div.currentPrimaryKeyValue == null) {
				JsonDataBinding.setTableAttribute(_jsTable.TableName, 'isFisrtTime', true);
			}
			else {
				JsonDataBinding.setTableAttribute(_jsTable.TableName, 'isFisrtTime', false);
			}
		}
		_div.currentKeyNode = parentNode;
	}
	function findChildNodeByRowIndex(parentTreeNode, r) {
		for (var i = 0; i < parentTreeNode.childNodes.length; i++) {
			if (typeof parentTreeNode.childNodes[i].rowIndex != 'undefined') {
				if (parentTreeNode.childNodes[i].rowIndex == r) {
					return parentTreeNode.childNodes[i];
				}
				var nd = findChildNodeByRowIndex(parentTreeNode.childNodes[i], r);
				if (nd) {
					return nd;
				}
			}
		}
		return null;
	}
	function _findNodeByRowIndex(r) {
		return findChildNodeByRowIndex(_div, r);
	}
	function _onRowIndexChange(dataname) {
		if (_jsTable) {
			if (_jsTable.TableName == dataname) {
				if (_div.selectedNode) {
					if (_div.selectedNode.rowIndex == _jsTable.rowIndex) {
						return;
					}
				}
				if (_jsTable.rowIndex >= 0) {
					var nd = _findNodeByRowIndex(_jsTable.rowIndex);
					_selectNode(nd);
				}
			}
		}
	}
	//other bound control modified value, update corresponding cell
	_div.oncellvaluechange = function(name, r, c, value) {
		if (_jsTable.TableName == name) {
			if (c == nodenameIdx || c == textIdx || c == imageIdx || c == nodedataIdx) {
				_onRowIndexChange(name);
				if (_div.selectedNode) {
					if (_div.selectedNode.rowIndex == r) {
						if (c == imageIdx)
							_div.selectedNode.jsData.setIconImage(value);
						else if (c == textIdx)
							_div.selectedNode.jsData.setNodeText(value);
						else if (c == nodenameIdx)
							_div.selectedNode.name = value;
						else if (c == nodedataIdx)
							_div.selectedNode.nodedata = value;
					}
				}
			}
		}
	}
	function _hideLoadingMessage() {
		if (_loadingMsg) {
			_loadingMsg.style.display = 'none';
		}
	}
	function _showLoadingMessage(node) {
		var pos = getElementAbsolutePos(node);
		if (!_loadingMsg) {
			_loadingMsg = document.createElement("div");
			document.body.appendChild(_loadingMsg);
			_loadingMsg.innerHTML = 'Loading';
			_loadingMsg.style.zIndex = 100;
			_loadingMsg.style.position = 'absolute';
			_loadingMsg.style.color = 'red';
			_loadingMsg.style.backgroundColor = 'white';
		}
		_loadingMsg.style.left = pos.x + 'px';
		_loadingMsg.style.top = pos.y + 'px';
		_loadingMsg.style.display = 'block';
	}
	function _onDataReady(dataTable) {
		var jsdb = _div.getAttribute('jsdb');
		if (jsdb) {
			var binds = jsdb.split(':');
			if (binds.length > 0) {
				if (binds[0] == dataTable.TableName) {
					_jsTable = dataTable;
					nodenameIdx = -1;
					textIdx = -1;
					imageIdx = -1;
					nodedataIdx = -1;
					if (binds.length > 1 && binds[1].length > 0) {
						textIdx = dataTable.columnIndexes[binds[1]];
					}
					if (binds.length > 2 && binds[2].length > 0) {
						nodenameIdx = dataTable.columnIndexes[binds[2]];
					}
					if (binds.length > 3 && binds[3].length > 0) {
						imageIdx = dataTable.columnIndexes[binds[3]];
					}
					if (binds.length > 4 && binds[4].length > 0) {
						nodedataIdx = dataTable.columnIndexes[binds[4]];
					}
					var r;
					var text;
					var image;
					var nd;
					var nodename;
					var nodedata;
					if (JsonDataBinding.getTableAttribute(dataTable.TableName, 'isDataStreaming')) {
						var parentnode = JsonDataBinding.getTableAttribute(dataTable.TableName, 'tv_parentnode'); // dataTable.parentnode;
						for (r = dataTable.newRowStartIndex; r < dataTable.Rows.length; r++) {
							nodename = null;
							if (textIdx >= 0)
								text = dataTable.Rows[r].ItemArray[textIdx];
							else
								text = '';
							if (nodenameIdx >= 0) {
								nodename = dataTable.Rows[r].ItemArray[nodenameIdx];
							}
							if (imageIdx >= 0) {
								image = dataTable.Rows[r].ItemArray[imageIdx];
							}
							else
								image = null;
							if (nodedataIdx >= 0) {
								nodedata = dataTable.Rows[r].ItemArray[nodedataIdx];
							}
							else
								nodedata = null;
							nd = _addNode(parentnode, nodename, text, image, nodedata, r);
							//nd.rowIndex = r;
						}
						if (_loadingMsg) {
							_loadingMsg.style.display = 'none';
						}
					}
					else {
						JsonDataBinding.addvaluechangehandler(_jsTable.TableName, _div);
						var nds = _getNodes(_div);
						for (var i = 0; i < nds.length; i++) {
							_div.removeChild(nds[i]);
						}
						for (r = 0; r < dataTable.Rows.length; r++) {
							nodename = null;
							if (textIdx >= 0)
								text = dataTable.Rows[r].ItemArray[textIdx];
							else
								text = '';
							if (nodenameIdx >= 0) {
								nodename = dataTable.Rows[r].ItemArray[nodenameIdx];
							}
							if (imageIdx >= 0) {
								image = dataTable.Rows[r].ItemArray[imageIdx];
							}
							else
								image = null;
							if (nodedataIdx >= 0) {
								nodedata = dataTable.Rows[r].ItemArray[nodedataIdx];
							}
							else
								nodedata = null;
							nd = _addNode(_div, nodename, text, image, nodedata, r);
							//nd.rowIndex = r;
						}
					}
				}
			}
		}
	}
	_onAddNodeToTable = function(parentNode, nodename, text, image, nodedata) {
		if (_jsTable) {
			var r = JsonDataBinding.addRow(_jsTable.TableName);
			var row = _jsTable.Rows[r];
			//
			if (textIdx >= 0) {
				_jsTable.Rows[r].ItemArray[textIdx] = text;
			}
			if (nodenameIdx >= 0) {
				_jsTable.Rows[r].ItemArray[nodenameIdx] = nodename;
			}
			if (imageIdx >= 0) {
				_jsTable.Rows[r].ItemArray[imageIdx] = image;
			}
			if (nodedataIdx >= 0) {
				_jsTable.Rows[r].ItemArray[nodedataIdx] = nodedata;
			}
			if (parentNode) {
				var pkey = parentNode.jsData.getPrimaryKey();
				_jsTable.Rows[r].ItemArray[_jsTable.columnIndexes[_div.foreignKey]] = pkey;
			}
			//
			return r;
		}
		return -1;
	}
	function _getNodes(parentTreeNode) {
		var nodes = new Array();
		var l = 0;
		if (typeof parentTreeNode.treelevel != 'undefined') {
			l = parentTreeNode.treelevel + 1;
		}
		for (var i = 0; i < parentTreeNode.childNodes.length; i++) {
			if (parentTreeNode.childNodes[i].treelevel == l) {
				nodes.push(parentTreeNode.childNodes[i]);
			}
		}
		return nodes;
	}
	function _hasChild() {
		for (var i = 0; i < _div.childNodes.length; i++) {
			if (typeof _div.childNodes[i].treelevel != 'undefined') {
				return true;
			}
		}
		return false;
	}
	function _addNodeSelectHandler(h) {
		for (var i = 0; i < _nodeSelectionHandlers.length; i++) {
			if (_nodeSelectionHandlers[i] == h) {
				return;
			}
		}
		_nodeSelectionHandlers.push(h);
	}
	function _removeNodeSelectHandler(h) {
		for (var i = 0; i < _nodeSelectionHandlers.length; i++) {
			if (_nodeSelectionHandlers[i] == h) {
				_nodeSelectionHandlers.splice(i, 1);
				return;
			}
		}
	}
	function _afterSelectNode() {
		for (var i = 0; i < _nodeSelectionHandlers.length; i++) {
			if (_nodeSelectionHandlers[i]) {
				_nodeSelectionHandlers[i]();
			}
		}
		if (_div.onnodeselected) {
			_div.onnodeselected();
		}
	}
	function _addChildNode(parentTreeNode, nodename, text, image, nodedata) {
		if (parentTreeNode) {
			if (parentTreeNode.jsData.canAddChildNode()) {
				var nd = _addNode(parentTreeNode, nodename, text, image, nodedata);
				var rIdx = _onAddNodeToTable(parentTreeNode, nodename, text, image, nodedata);
				if (rIdx >= 0) {
					nd.rowIndex = rIdx;
					_jsTable.Rows[rIdx].rowVersion = 1;
					nd.jsData.preventnextLevel();
					JsonDataBinding.dataMoveToRecord(_jsTable.TableName, rIdx);
				}
				return nd;
			}
		}
		return null;
	}
	function _deleteSelectedNode() {
		var selNode = _div.selectedNode;
		if (selNode) {
			var rIdx = typeof selNode.rowIndex == 'undefined' ? -1 : selNode.rowIndex;
			if (rIdx >= 0) {
				var canRemove = false;
				if (selNode.jsData.nextLevelLoaded()) {
					var pkey = selNode.jsData.getPrimaryKey();
					if (typeof pkey != 'undefined' && pkey != null) {
						if (!selNode.jsData.hasChild()) {
							canRemove = true;
						}
					}
				}
				if (canRemove) {
					if (_jsTable.rowIndex != rIdx) {
						JsonDataBinding.dataMoveToRecord(_jsTable.TableName, rIdx);
					}
					if (_jsTable.rowIndex == rIdx) {
						JsonDataBinding.deleteCurrentRow(_jsTable.TableName);
						selNode.style.display = 'none';
						return true;
					}
				}
			}
			else {
				var pe = selNode.parentNode;
				pe.removeChild(selNode);
				return true;
			}
		}
		return false;
	}
	function _addNode(parentTreeNode, nodename, text, image, nodedata, rIdx) {
		var _divNode;
		var _tblHolder;
		var _imgNodeState;
		var _imgNodeIcon;
		var _spText;
		var _tree;
		var _tdPad;
		var _tdPad2;
		//
		var _nextLevelLoaded = false;
		//
		var STATE_COLLAPSED = 0;
		var STATE_EXPANDED = 1;
		var _state = STATE_COLLAPSED;
		var POS_TOP = 0; //the first root node
		var POS_LAST = 1; //last child node in any level
		var POS_MIDDLE = 2; //it has previous and next sibblings
		var POS_TOPSINGLE = 3;
		//
		function _hasChild() {
			for (var i = 0; i < _divNode.childNodes.length; i++) {
				if (typeof _divNode.childNodes[i].treelevel != 'undefined') {
					return true;
				}
			}
			return false;
		}
		function isFirst() {
			if (_divNode.previousSibling) {
				if (typeof _divNode.previousSibling.treelevel != 'undefined') {
					return false;
				}
			}
			return true;
		}
		function _isLast() {
			if (_divNode.nextSibling) {
				if (typeof _divNode.nextSibling.treelevel != 'undefined') {
					return false;
				}
			}
			return true;
		}
		function _showStateImage() {
			var pos = POS_TOP;
			var isFirstNode = isFirst();
			var isLastNode = _isLast();
			if (isLastNode) {
				_tdPad2.style.backgroundImage = "url('./treeview/w20.png')";
			}
			else {
				_tdPad2.style.backgroundImage = "url('./treeview/h1.png')";
			}
			if (isFirstNode && _divNode.treelevel == 0) {
				_tdPad.style.backgroundImage = "url('./treeview/w20.png')";
			}
			else {
				_tdPad.style.backgroundImage = "url('./treeview/h1.png')";
			}
			if (_divNode.treelevel == 0 && isFirstNode) {
				if (isLastNode) {
					pos = POS_TOPSINGLE;
				}
				else {
					pos = POS_TOP;
				}
			}
			else {
				if (isLastNode) {
					pos = POS_LAST;
				}
				else {
					pos = POS_MIDDLE;
				}
			}
			var haschildren = _hasChild();
			if (_state == STATE_COLLAPSED) {
				if (!_nextLevelLoaded || haschildren) {
					switch (pos) {
						case POS_TOPSINGLE:
							_imgNodeState.src = "./treeview/plus_0.png"; break;
						case POS_TOP:
							_imgNodeState.src = "./treeview/plus_dn.png"; break;
						case POS_MIDDLE:
							_imgNodeState.src = "./treeview/plus_up_dn.png"; break;
						case POS_LAST:
							_imgNodeState.src = "./treeview/plus_up.png"; break;
					}
				}
				else {
					switch (pos) {
						case POS_TOPSINGLE:
							_imgNodeState.src = "./treeview/empty_0.png"; break;
						case POS_TOP:
							_imgNodeState.src = "./treeview/empty_dn.png"; break;
						case POS_MIDDLE:
							_imgNodeState.src = "./treeview/empty_up_dn.png"; break;
						case POS_LAST:
							_imgNodeState.src = "./treeview/empty_up.png"; break;
					}
				}
			}
			else {
				if (haschildren || !_nextLevelLoaded) {
					switch (pos) {
						case POS_TOPSINGLE:
							_imgNodeState.src = "./treeview/minus_0.png"; break;
						case POS_TOP:
							_imgNodeState.src = "./treeview/minus_dn.png"; break;
						case POS_MIDDLE:
							_imgNodeState.src = "./treeview/minus_up_dn.png"; break;
						case POS_LAST:
							_imgNodeState.src = "./treeview/minus_up.png";
							break;
					}
				}
				else {
					switch (pos) {
						case POS_TOPSINGLE:
							_imgNodeState.src = "./treeview/empty_0.png"; break;
						case POS_TOP:
							_imgNodeState.src = "./treeview/empty_dn.png"; break;
						case POS_MIDDLE:
							_imgNodeState.src = "./treeview/empty_up_dn.png"; break;
						case POS_LAST:
							_imgNodeState.src = "./treeview/empty_up.png"; break;
					}
				}
			}
		} //===end of _showStateImage
		function _showNode() {
			_divNode.style.display = 'block';
			if (_tblHolder.offsetHeight > _imgNodeState.offsetHeight + 2) {
				var ph = Math.floor((_tblHolder.offsetHeight - _imgNodeState.offsetHeight) / 2);
				if (ph > 1) {
					if (_isLast()) {
						_tdPad2.style.backgroundImage = "url('./treeview/w20.png')";
					}
					else {
						_tdPad2.style.backgroundImage = "url('./treeview/h1.png')";
					}
					_tdPad.style.height = ph + "px";
					_tdPad2.style.height = ph + "px";
				}
			}
		}
		function toggleExpand() {
			if (_state == STATE_COLLAPSED) {
				if (!_nextLevelLoaded) {
					_nextLevelLoaded = true;
					if (_tree.nodesloader) {
						_showLoadingMessage(_divNode);
						_tree.jsData.setCurrentPrimaryKey(_divNode);
						_tree.nodesloader();
					}
				}
				_state = STATE_EXPANDED;
			}
			else
				_state = STATE_COLLAPSED;
			var i;
			if (_state == STATE_COLLAPSED) {
				for (i = 0; i < _divNode.childNodes.length; i++) {
					if (typeof _divNode.childNodes[i].treelevel != 'undefined') {
						_divNode.childNodes[i].style.display = 'none';
					}
				}
			}
			else {
				for (i = 0; i < _divNode.childNodes.length; i++) {
					if (typeof _divNode.childNodes[i].treelevel != 'undefined') {
						_divNode.childNodes[i].jsData.showNode();
					}
				}
			}
			_showStateImage();
		}
		function onNodemouseover() {
			_divNode.style.backgroundColor = _tree.mouseOverColor;
		}
		function onNodeMouseOut() {
			if (_divNode.isSelected) {
				_divNode.style.backgroundColor = _tree.selectedNodeColor;
			}
			else {
				if (typeof _divNode.backColor == 'undefined') {
					_divNode.style.backgroundColor = _tree.nodeBackColor;
				}
				else {
					_divNode.style.backgroundColor = _divNode.backColor;
				}
			}
		}
		function onNodeMouseClick() {
			_selectNode(_divNode);
			if (_divNode.onnodeclick) {
				_divNode.onnodeclick();
			}
		}
		function onNodemousedown(e) {
		}
		function _getTreeView() {
			var p = _divNode.parentNode;
			if (p)
				return p.jsData.getTreeView();
		}
		function _getState() {
			return _state;
		}
		function _addChildNode(text, image, nodedata) {
			_addNode(_divNode, text, image, nodedata);
		}
		function _setNodeText(text) {
			JsonDataBinding.SetInnerText(_spText, text);
		}
		function _getNodeText() {
			return JsonDataBinding.GetInnerText(_spText);
		}
		function _setNodeHtmlText(html) {
			_spText.innerHTML = html;
		}
		function _getNodeHtmlText() {
			return _spText.innerHTML;
		}
		function _setIconImage(imagePath) {
			if (imagePath && imagePath != null && imagePath != 'null') {
				if (imagePath.length > 0) {
					_imgNodeIcon.src = imagePath;
					_imgNodeIcon.style.display = "inline";
					_showNode();
					return;
				}
			}
			_imgNodeIcon.src = null;
			_imgNodeIcon.style.display = "none";
			_showNode();
		}
		function _getIconImage() {
			return _imgNodeIcon.src;
		}
		function _getFontFamily() {
			return _spText.style.fontFamily;
		}
		function _setFontFamily(fontFamily) {
			_spText.style.fontFamily = fontFamily;
		}
		function _getFontSize() {
			return _spText.style.fontSize;
		}
		function _setFontSize(fontSize) {
			_spText.style.fontSize = fontSize + "px";
		}
		function _getFontColor() {
			return _spText.style.color;
		}
		function _setFontColor(fontColor) {
			_spText.style.color = fontColor;
		}
		function _getBackColor() {
			return _divNode.backColor;
		}
		function _setBackColor(bkColor) {
			_divNode.backColor = bkColor;
			if (typeof bkColor == 'undefined' || bkColor == null)
				_divNode.style.backgroundColor = _tree.nodeBackColor;
			else
				_divNode.style.backgroundColor = bkColor;
		}
		function _getPrimaryKey() {
			if (_jsTable && typeof _divNode.rowIndex != 'undefined') {
				if (!_jsTable.Rows[_divNode.rowIndex].added) {
					return _jsTable.Rows[_divNode.rowIndex].ItemArray[_jsTable.columnIndexes[_div.primaryKey]];
				}
			}
			return null;
		}
		//nodename, text, image, nodedata
		_tree = parentTreeNode.jsData.getTreeView();

		_divNode = document.createElement("div");
		_divNode.style.verticalAlign = 'top';
		_divNode.nodedata = nodedata;
		_divNode.name = nodename;
		//_jsTable.Rows[parentNode.rowIndex].ItemArray[_jsTable.columnIndexes[_div.primaryKey]]
		if (typeof parentTreeNode.treelevel == 'undefined')
			_divNode.treelevel = 0;
		else
			_divNode.treelevel = parentTreeNode.treelevel + 1;
		_divNode.style.backgroundColor = _tree.nodeBackColor;
		_divNode.style.display = 'block';
		_divNode.style.padding = 0;
		//
		_imgNodeState = document.createElement("img");
		_imgNodeState.src = "./treeview/plus_0.png";
		_imgNodeState.style.border = 0;
		_imgNodeState.style.display = 'inline';
		_imgNodeState.style.cursor = 'pointer';
		JsonDataBinding.AttachEvent(_imgNodeState, 'onclick', toggleExpand);
		//
		_imgNodeIcon = document.createElement("img");
		_imgNodeIcon.style.border = 0;
		if (image && image.length > 0) {
			_imgNodeIcon.src = image;
			_imgNodeIcon.style.display = 'inline';
		}
		else {
			_imgNodeIcon.style.display = 'none';
		}
		JsonDataBinding.AttachEvent(_imgNodeIcon, 'onmouseover', onNodemouseover);
		JsonDataBinding.AttachEvent(_imgNodeIcon, 'onmouseout', onNodeMouseOut);
		JsonDataBinding.AttachEvent(_imgNodeIcon, 'onclick', onNodeMouseClick);
		JsonDataBinding.AttachEvent(_imgNodeIcon, 'onmousedown', onNodemousedown);
		//
		_spText = document.createElement("span");
		_spText.innerHTML = text;
		if (typeof _tree.noteFontFamily != 'undefined') {
			_spText.style.fontFamily = _tree.noteFontFamily;
		}
		if (typeof _tree.noteFontSize != 'undefined') {
			_spText.style.fontSize = _tree.noteFontSize;
		}
		if (typeof _tree.noteFontColor != 'undefined') {
			_spText.style.color = _tree.noteFontColor;
		}
		_spText.style.display = 'inline';
		JsonDataBinding.AttachEvent(_spText, 'onmouseover', onNodemouseover);
		JsonDataBinding.AttachEvent(_spText, 'onmouseout', onNodeMouseOut);
		JsonDataBinding.AttachEvent(_spText, 'onclick', onNodeMouseClick);
		JsonDataBinding.AttachEvent(_spText, 'onmousedown', onNodemousedown);
		//
		parentTreeNode.appendChild(_divNode);
		//_divNode.style.margin = "-3px 0 0 0";
		_divNode.style.margin = '0px';
		_divNode.style.padding = 0;
		//
		_tblHolder = document.createElement('table');
		_divNode.appendChild(_tblHolder);
		_tblHolder.border = 0;
		_tblHolder.setAttribute("border", "0");
		_tblHolder.style.margin = 0;
		_tblHolder.style.padding = 0;
		_tblHolder.cellPadding = 0;
		_tblHolder.cellSpacing = 0;
		//
		var tbd = null;
		var tbds = _tblHolder.getElementsByTagName('tbody');
		if (tbds) {
			if (tbds.length > 0) {
				tbd = tbds[0];
			}
		}
		if (!tbd) {
			tbd = document.createElement('tbody');
			_tblHolder.appendChild(tbd);
		}
		var tr = document.createElement('tr');
		tbd.appendChild(tr);
		var tr2 = document.createElement('tr');
		tbd.appendChild(tr2);
		var tr3 = document.createElement('tr');
		tbd.appendChild(tr3);
		tr.setAttribute("valign", 'center');//'bottom');
		tr2.setAttribute('valign', 'center');//'bottom');
		//
		var parents = {};
		var p = parentTreeNode;
		while (p) {
			if (p.jsData) {
				if (typeof p.treelevel == 'undefined') {
					break;
				}
				else {
					parents['t' + (p.treelevel)] = p;
					p = p.parentNode;
				}
			}
			else {
				break;
			}
		}
		for (var i = 0; i < _divNode.treelevel; i++) {
			var td = document.createElement('td');
			tr.appendChild(td);
			td.rowSpan = 3;
			//
			p = parents['t' + i];
			if (i > 0 && p.jsData.isLastNode()) {
				td.style.backgroundImage = "url('./treeview/w20.png')";
			}
			else {
				td.style.backgroundImage = "url('./treeview/vl.png')";
			}
			td.style.width = "20px";
		}

		_tdPad = document.createElement('td');
		tr.appendChild(_tdPad);
		if (_divNode.treelevel == 0 && isFirst()) {
			_tdPad.style.backgroundImage = "url('./treeview/w20.png')";
		}
		else {
			_tdPad.style.backgroundImage = "url('./treeview/h1.png')";
		}
		td = document.createElement('td');
		tr2.appendChild(td);
		td.appendChild(_imgNodeState);
		td.style.height = "20px";
		//
		_tdPad2 = document.createElement('td');
		tr3.appendChild(_tdPad2);
		if (_isLast()) {
			_tdPad2.style.backgroundImage = "url('./treeview/w20.png')";
		}
		else {
			_tdPad2.style.backgroundImage = "url('./treeview/h1.png')";
		}
		//
		td = document.createElement('td');
		tr.appendChild(td);
		td.rowSpan = 3;
		var lbl = document.createElement('label');
		td.appendChild(lbl);
		lbl.appendChild(_imgNodeIcon);
		lbl.appendChild(_spText);
		//
		//JsonDataBinding.AttachEvent(td, 'onmouseover', onNodemouseover);
		//JsonDataBinding.AttachEvent(td, 'onmouseout', onNodeMouseOut);
		//JsonDataBinding.AttachEvent(td, 'onclick', onNodeMouseClick);
		//
		if (_divNode.previousSibling && _divNode.previousSibling.jsData) {
			_divNode.previousSibling.jsData.showStateImage();
		}

		//
		_showStateImage();
		//
		if (_divNode.treelevel > 0) {
			if (parentTreeNode.jsData.getState() == STATE_COLLAPSED) {
				_divNode.style.display = 'none';
			}
			else {
				_showNode();
			}
			parentTreeNode.jsData.showStateImage();
		}
		else {
			_showNode();
		}
		//
		_divNode.jsData = {
			clear: function() {
				var i;
				var roots = new Array();
				for (i = 0; i < _divNode.childNodes.length; i++) {
					if (typeof _divNode.childNodes[i].treelevel != 'undefined') {
						roots.push(_divNode.childNodes[i]);
					}
				}
				for (i = 0; i < roots.length; i++) {
					_divNode.removeChild(roots[i]);
				}
			},
			getState: function() {
				return _getState();
			},
			addChildNode: function(text, image, nodedata) {
				return _addChildNode(text, image, nodedata);
			},
			getTreeView: function() {
				return _getTreeView();
			},
			hasChild: function() {
				return _hasChild();
			},
			showStateImage: function() {
				_showStateImage();
			},
			showNode: function() {
				_showNode();
			},
			isLastNode: function() {
				return _isLast();
			},
			setNodeData: function(data) {
				_setNodeData(data);
			},
			getNodeData: function() {
				return _getNodeData();
			},
			setNodeText: function(text) {
				_setNodeText(text);
			},
			getNodeText: function() {
				return _getNodeText();
			},
			setNodeHtmlText: function(html) {
				return _setNodeHtmlText(html);
			},
			getNodeHtmlText: function() {
				return _getNodeHtmlText();
			},
			setIconImage: function(imagePath) {
				_setIconImage(imagePath);
			},
			getIconImage: function() {
				return _getIconImage();
			},
			getFontFamily: function() {
				return _getFontFamily();
			},
			setFontFamily: function(fontFamily) {
				_setFontFamily(fontFamily);
			},
			getFontSize: function() {
				return _getFontSize();
			},
			setFontSize: function(fontSize) {
				_setFontSize(fontSize);
			},
			getFontColor: function() {
				return _getFontColor();
			},
			setFontColor: function(fontColor) {
				_setFontColor(fontColor);
			},
			getBackColor: function() {
				return _getBackColor();
			},
			setBackColor: function(bkColor) {
				_setBackColor(bkColor);
			},
			preventnextLevel: function() {
				_nextLevelLoaded = true;
			},
			nextLevelLoaded: function() {
				return _nextLevelLoaded;
			},
			getPrimaryKey: function() {
				return _getPrimaryKey();
			},
			canAddChildNode: function() {
				if (_jsTable) {
					var pkey = _getPrimaryKey();
					return (pkey != null);
				}
				return true;
			}
		};
		if (typeof rIdx != 'undefined') {
			_divNode.rowIndex = rIdx;
		}
		if (_tree.onnodecreated) {
			_tree.onnodecreated(null, _tree, _divNode);
		}
		return _divNode;
	} //end of _addNode
	function _getReadOnly() {
		return _readOnly;
	}
	function _setReadOnly(readOnly) {
		_readOnly = readOnly;
		if (_readOnly) {
		}
	}
	function _clear() {
		var i;
		var roots = new Array();
		for (i = 0; i < _div.childNodes.length; i++) {
			if (typeof _div.childNodes[i].treelevel != 'undefined') {
				roots.push(_div.childNodes[i]);
			}
		}
		for (i = 0; i < roots.length; i++) {
			_div.removeChild(roots[i]);
		}
	}
	function _refreshBindColumnDisplay(name, rowidx, colIdx) {
		if (_jsTable && _jsTable.TableName == name) {
			if (colIdx == imageIdx || colIdx == textIdx || colIdx == nodenameIdx || colIdx == nodedataIdx) {
				var node = _findNodeByRowIndex(rowidx);
				if (node && node.jsData) {
					if (colIdx == imageIdx) {
						node.jsData.setIconImage(_jsTable.Rows[rowidx].ItemArray[colIdx]);
					}
					else if (colIdx == textIdx) {
						node.jsData.setNodeText(_jsTable.Rows[rowidx].ItemArray[colIdx]);
					}
					else if (colIdx == nodenameIdx) {
						node.name = _jsTable.Rows[rowidx].ItemArray[colIdx];
					}
					else if (colIdx == nodedataIdx) {
						node.jsData.setNodeData(_jsTable.Rows[rowidx].ItemArray[colIdx]);
					}
				}
			}
		}
	}
	function _selectFirstNode() {
		for (var i = 0; i < _div.childNodes.length; i++) {
			if (typeof _div.childNodes[i].treelevel != 'undefined') {
				_selectNode(_div.childNodes[i]);
				return true;
			}
		}
	}
	htmlElement.jsData = {
		clear: function() {
			_clear();
		},
		addRootNode: function(nodename, text, image, nodedata) {
			return _addChildNode(_div, nodename, text, image, nodedata);
		},
		addChildNodeToSelectedNode: function(nodename, text, image, nodedata) {
			return _addChildNode(_div.selectedNode, nodename, text, image, nodedata);
		},
		addChildNode: function(parentTreeNode, nodename, text, image, nodedata) {
			return _addChildNode(parentTreeNode, nodename, text, image, nodedata);
		},
		deleteSelectedNode: function() {
			return _deleteSelectedNode();
		},
		getNodes: function() {
			return _getNodes(_div);
		},
		getTreeView: function() {
			return _div;
		},
		hasChild: function() {
			return _hasChild();
		},
		addNodeSelectHandler: function(h) {
			_addNodeSelectHandler(h);
		},
		removeNodeSelectHandler: function(h) {
			_removeNodeSelectHandler(h);
		},
		afterSelectNode: function() {
			_afterSelectNode();
		},
		setReadOnly: function(readOnly) {
			_setReadOnly(readOnly);
		},
		getReadOnly: function() {
			return _getReadOnly();
		},
		onDataReady: function(dataTable) {
			_onDataReady(dataTable);
		},
		hideLoadingMessage: function() {
			_hideLoadingMessage();
		},
		showLoadingMessage: function(node) {
			_showLoadingMessage(node);
		},
		setCurrentPrimaryKey: function(parentNode) {
			_setCurrentPrimaryKey(parentNode);
		},
		onRowIndexChange: function(dataname) {
			_onRowIndexChange(dataname);
		},
		getPrimaryKey: function() {
			return null;
		},
		canAddChildNode: function() {
			return true;
		},
		refreshBindColumnDisplay: function(name, rowidx, colIdx) {
			_refreshBindColumnDisplay(name, rowidx, colIdx);
		},
		selectFirstNode: function() {
			_selectFirstNode();
		}
	};
	return htmlElement.jsData;
}
