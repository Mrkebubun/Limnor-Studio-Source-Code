//htmlElement: a div for holding the treeview.
JsonDataBinding.CreateFileBrowser = function(htmlElement, rootFolder, rootName, filetypes, dontInit) {
	var _rootUrl;
	var _table;
	var _divFolders;
	var _divFiles;
	var _colorSelectedFile = '#ADD8E6';
	var _root;
	var _selectedFile;
	var _inited = false;
	var _fileSelectionHandlers = new Array();
	_initFolders();
	function _initFolders() {
		_rootUrl = ''; // window.location.protocol + '//' + window.location.host;
		if (rootFolder && rootFolder.length > 0) {
			if (rootFolder.charAt(0) == '/') {
				_rootUrl = _rootUrl + rootFolder;
			}
			else {
				_rootUrl = _rootUrl + '/' + rootFolder;
			}
		}
		else {
			rootFolder = ''; // 
			_rootUrl = _rootUrl + window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));
		}
		if (_rootUrl.charAt(_rootUrl.length - 1) == '/') {
			_rootUrl = _rootUrl.substr(0, _rootUrl.length - 1);
		}
		if (!rootName) {
			rootName = rootFolder;
			if (rootName.length == 0) {
				rootName = 'current folder';
			}
		}
	}
	function _addFileSelectHandler(h) {
		for (var i = 0; i < _fileSelectionHandlers.length; i++) {
			if (_fileSelectionHandlers[i] == h) {
				return;
			}
		}
		_fileSelectionHandlers.push(h);
	}
	function _removeFileSelectHandler(h) {
		for (var i = 0; i < _fileSelectionHandlers.length; i++) {
			if (_fileSelectionHandlers[i] == h) {
				_fileSelectionHandlers.splice(i, 1);
				return;
			}
		}
	}
	function getPath(node) {
		if (node == _root) {
			if (_root.name == '/')
				return '';
			return _root.name;
		}
		return getPath(node.parentNode) + '/' + node.name;
	}
	function adjustSize() {
		var h = htmlElement.offsetHeight - 10;
		if (h > 0) {
			_divFiles.style.height = h + 'px';
			_divFolders.style.height = h + 'px';
			_table.style.width = (htmlElement.offsetWidth - 2) + 'px';
			_table.style.height = (h + 2) + 'px';
		}
	}
	function _loadFolders(pnode, folderName) {
		if (!pnode.nodedata) {
			pnode.nodedata = {};
		}
		pnode.nodedata.nextLevelLoaded = true;
		JsonDataBinding.accessServer(null, 'loadFolders_filebrowser', folderName, { phpFolderName: 'libphp', filetypes: htmlElement.jsData.FileTypes() }, function() {
			if (JsonDataBinding.values && JsonDataBinding.values.folders) {
				for (var i = 0; i < JsonDataBinding.values.folders.length; i++) {
					_divFolders.jsData.addChildNode(pnode, JsonDataBinding.values.folders[i], JsonDataBinding.values.folders[i], 'libjs/html_file.png', { nextLevelLoaded: false });
				}
			}
			_divFolders.jsData.hideLoadingMessage();
			adjustSize();
		});
	}
	function _loadFiles(pnode) {
		JsonDataBinding.accessServer(null, 'loadFiles_filebrowser', getPath(pnode), { phpFolderName: 'libphp', filetypes: htmlElement.jsData.FileTypes() }, function() {
			if (JsonDataBinding.values && JsonDataBinding.values.filenames) {
				pnode.filelist = JsonDataBinding.values.filenames;
				showFiles(pnode);
			}
			_divFolders.jsData.hideLoadingMessage();
		});
	}
	function onfilemouseover(e) {
		var sp = JsonDataBinding.getSender(e);
		if (sp) {
			sp.style.backgroundColor = 'yellow';
		}
	}
	function onfileMouseOut(e) {
		var sp = JsonDataBinding.getSender(e);
		if (sp) {
			if (sp == _selectedFile)
				sp.style.backgroundColor = _colorSelectedFile;
			else
				sp.style.backgroundColor = 'white';
		}
	}
	function onfileMouseClick(e) {
		var sp = JsonDataBinding.getSender(e);
		if (sp) {
			if (_selectedFile) {
				_selectedFile.style.backgroundColor = 'white';
			}
			_selectedFile = sp;
			sp.style.backgroundColor = _colorSelectedFile;
			for (var i = 0; i < _fileSelectionHandlers.length; i++) {
				if (_fileSelectionHandlers[i]) {
					_fileSelectionHandlers[i]();
				}
			}
			if (htmlElement.onfileselected) {
				htmlElement.onfileselected();
			}
		}
	}
	function _getSelectedFile() {
		if (_selectedFile) {
			return _selectedFile.file;
		}
	}
	function _getSelectedFileFullPath() {
		if (_selectedFile) {
			var f = getPath(_divFolders.selectedNode) + '/' + _selectedFile.file;
			//			if (f.charAt(0) == '/')
			//				return _rootUrl + f;
			//			else
			//				return _rootUrl + '/' + f;
			return f;
		}
	}
	function _getSelectedFolder() {
		if (_divFolders.selectedNode) {
			return _divFolders.selectedNode.name;
		}
	}
	function _getSelectedFolderFullPath() {
		if (_divFolders.selectedNode) {
			return getPath(_divFolders.selectedNode) + '/';
		}
	}
	function showFiles(node) {
		//_listFiles.options.length = 0;
		if (node.filelist) {
			if (node.filelist.length <= 0) {
				_divFiles.innerHTML = 'file list is empty';
			}
			else {
				_divFiles.innerHTML = '';
				for (var i = 0; i < node.filelist.length; i++) {
					var sp = document.createElement('span');
					sp.style.cursor = 'pointer';
					sp.innerHTML = node.filelist[i];
					sp.file = node.filelist[i];
					_divFiles.appendChild(sp);
					var br = document.createElement('br');
					_divFiles.appendChild(br);
					//
					JsonDataBinding.AttachEvent(sp, 'onmouseover', onfilemouseover);
					JsonDataBinding.AttachEvent(sp, 'onmouseout', onfileMouseOut);
					JsonDataBinding.AttachEvent(sp, 'onclick', onfileMouseClick);
				}
			}
			adjustSize();
		}
		else {
			_divFiles.innerHTML = 'Searching files...';
			node.filelist = new Array();
			_divFolders.jsData.showLoadingMessage(node);
			_loadFiles(node);
		}
	}
	function _resetCurrentFolder() {
		if (_divFolders.selectedNode) {
			//_divFolders.selectedNode.jsData.clear();
			_divFolders.selectedNode.filelist = null;
			showFiles(_divFolders.selectedNode);
		}
	}
	function _init() {
		_inited = true;
		//htmlElement.style.overflow = 'scroll';
		_table = document.createElement('table');
		_table.border = 1;
		_table.cellPadding = 1;
		_table.cellSpacing = 1;
		_table.style.backgroundColor = 'white';
		_table.style.width = htmlElement.offsetWidth + 'px';
		_table.style.height = htmlElement.offsetHeight + 'px';
		htmlElement.appendChild(_table);
		var tblBody = JsonDataBinding.getTableBody(_table);
		var tr = tblBody.insertRow(-1);
		var td = document.createElement('td');
		td.setAttribute('valign', 'top');
		td.style.cssText = 'vertical-align: text-top;overflow:auto;';
		var tdFiles = document.createElement('td');
		tdFiles.setAttribute('valign', 'top');
		tdFiles.style.cssText = 'vertical-align: text-top;overflow:auto;';
		tr.appendChild(td);
		tr.appendChild(tdFiles);
		//
		_divFiles = document.createElement('div');
		_divFiles.style.overflow = 'auto';
		_divFiles.innerHTML = 'Click a folder to search for files';
		//
		_divFolders = document.createElement('div');
		//_listFiles = document.createElement('select');
		_divFolders.style.overflow = 'auto';
		//_listFiles.size = 2;
		td.appendChild(_divFolders);
		tdFiles.appendChild(_divFiles);
		//_divFiles.appendChild(_listFiles);
		//
		_divFolders.jsData = JsonDataBinding.CreateTreeView(_divFolders);
		_divFolders.jsData.addNodeSelectHandler(function() {
			if (_divFolders.selectedNode) {
				showFiles(_divFolders.selectedNode);
			}
		}
		);
		_divFolders.onnodeselected = function() {
			if (htmlElement.onnodeselected) {
				htmlElement.onnodeselected();
			}
		};
		_divFolders.nodesloader = function() {
			if (_divFolders.currentKeyNode.nodedata && _divFolders.currentKeyNode.nodedata.nextLevelLoaded) {
				_divFolders.jsData.hideLoadingMessage();
			}
			else {
				_loadFolders(_divFolders.currentKeyNode, getPath(_divFolders.currentKeyNode));
			}
		}
		//
		_root = _divFolders.jsData.addRootNode(rootFolder, rootName, 'libjs/html_file.png', { nextLevelLoaded: false });
		_loadFolders(_root, rootFolder);
	}
	function _restart(rootFolder2, rootName2, filetypes2) {
		rootFolder = rootFolder2;
		rootName = rootName2;
		filetypes = filetypes2;
		_initFolders();
		if (_inited) {
			_divFolders.jsData.clear();
			_root = _divFolders.jsData.addRootNode(rootFolder, rootName, 'libjs/html_file.png', { nextLevelLoaded: false });
			_loadFolders(_root, rootFolder);
		}
		else {
			_init();
			_divFolders.jsData.selectFirstNode();
			_resetCurrentFolder();
		}
	}
	htmlElement.jsData = {
		FileTypes: function() { return filetypes; },
		getFolderElement: function() {
			return _divFolders;
		},
		getTreeView: function() {
			return _divFolders.jsData.getTreeView();
		},
		addNodeSelectHandler: function(h) {
			_divFolders.jsData.addNodeSelectHandler(h);
		},
		removeNodeSelectHandler: function(h) {
			_divFolders.jsData.removeNodeSelectHandler(h);
		},
		addFileSelectHandler: function(h) {
			_addFileSelectHandler(h);
		},
		removeFileSelectHandler: function(h) {
			_removeFileSelectHandler(h);
		},
		getSelectedFile: function() {
			return _getSelectedFile();
		},
		getSelectedFileFullPath: function() {
			return _getSelectedFileFullPath();
		},
		getSelectedFolder: function() {
			return _getSelectedFolder();
		},
		getSelectedFolderFullPath: function() {
			return _getSelectedFolderFullPath();
		},
		resetCurrentFolder: function() {
			_resetCurrentFolder();
		},
		restart: function(rootFolder2, rootName2, filetypes2) {
			_restart(rootFolder2, rootName2, filetypes2);
		}
	};
	if (!dontInit) {
		_init();
	}
	return htmlElement.jsData;
}