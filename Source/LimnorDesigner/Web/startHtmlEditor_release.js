var JsonDataBinding;
var JSON;
var jscolor;
var HtmlEditor;
var editfullfile;
var editorFiles = new Array();
var editor;
function getHtmlString() {
	if (editor) {
		return editor.getHtmlString();
	}
	return '';
}
function editorStarted() {
	if (editor) {
		return 1;
	}
	else {
		return 0;
	}
}
function removePageEditor() {
	if (editor) {
		editor.removeEditor();
		editor = null;
	}
}
function getNamesUsed() {
	if (editor) {
		return editor.getNamesUsed();
	}
	return '';
}
function notifyUsedNames(usedNames) {
	if (editor) {
		notifyUsedNames(usedNames);
	}
}
function createOrGetId(guid) {
	if (editor) {
		return editor.createOrGetId(guid);
	}
	return '';
}
function getIdList() {
	if (editor) {
		return editor.getIdList();
	}
	return '';
}
function getIdByGuid(guid) {
	if (editor) {
		return editor.getIdByGuid(guid);
	}
	return '';
}
function getTagNameById(id) {
	var e = document.getElementById(id);
	if (e) {
		return e.tagName;
	}
	return '';
}
function setGuid(guid) {
	if (editor) {
		return editor.setGuid(guid);
	}
	return 2;
}
function getSelectedObject() {
	if (editor) {
		return editor.getSelectedObject();
	}
	return '';
}
function getMaps() {
	if (editor) {
		return editor.getMaps();
	}
	return '';
}
function getAreas(mapId) {
	if (editor) {
		return editor.getAreas(mapId);
	}
	return '';
}
function createNewMap(guid) {
	if (editor) {
		return editor.createNewMap(guid);
	}
	return '';
}
function setMapAreas(mapId, areas) {
	if (editor) {
		editor.setMapAreas(mapId, areas);
	}
}
function setUseMap(imgId, mapId) {
	if (editor) {
		editor.setUseMap(imgId, mapId);
	}
}
function setSelectedObject(guid) {
	if (editor) {
		return editor.setSelectedObject(guid);
	}
}
function setPropertyValue(propertyName, propertyValue) {
	if (editor) {
		return editor.setPropertyValue(propertyName, propertyValue);
	}
	return 2;
}
function appendArchiveFile(objId, filepath) {
	if (editor) {
		return editor.appendArchiveFile(objId, filepath);
	}
	return '';
}
function loadjsfile(f) {
	var head = document.getElementsByTagName('head')[0];
	var script = document.createElement('script');
	script.type = 'text/javascript';
	script.src = f;
	editorFiles.push(script);
	head.appendChild(script);
}
function sstartFileEditing() {
	if (!JsonDataBinding || !JSON || !jscolor || !HtmlEditor) {
		setTimeout(sstartFileEditing, 300);
		return;
	}
	jscolor.init();
	editor = HtmlEditor.createEditor(null, { 'forIDE': true });
	var head = document.getElementsByTagName('head')[0];
	for (var i = 0; i < editorFiles.length; i++) {
		head.removeChild(editorFiles[i]);
	}
	window.external.OnEditorStarted();
}
var loading = 0;
startHtmlEditor = function() {
	if (JSONIDENTIFIER) {
		if (loading == 0) {
			loading = 1;
		}
	}
	else {
		if (loading == 0) {
			loading++;
			loadjsfile('libjs/json2.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	if (JsonDataBinding) {
		if (loading < 2) {
			loading = 2;
		}
	}
	else {
		if (loading < 2) {
			loading = 2;
			loadjsfile('libjs/jsonDataBind_min.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	if (jscolor) {
		if (loading < 3) {
			loading = 3;
		}
	}
	else {
		if (loading < 3) {
			loading = 3;
			loadjsfile('libjs/jscolor.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	if (!HtmlEditor) {
		if (loading < 4) {
			loading = 4;
			loadjsfile('libjs/htmleditor_min.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	sstartFileEditing();
}
