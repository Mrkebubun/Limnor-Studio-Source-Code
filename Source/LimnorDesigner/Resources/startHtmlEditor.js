var JsonDataBinding;
var JSON;
var jscolor;
var HtmlEditor;
var editfullfile;
var editorFiles = new Array();
var editor;
var _originalDocType;
function getHtmlString() {
	if (editor) {
		return editor.getHtmlString();
	}
	return '';
}
function removePageEditor() {
	if (editor) {
		editor.removeEditor();
		editor = null;
	}
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
	editor = HtmlEditor.createEditor(null, { 'hideOKbutton': true, 'docType': _originalDocType });
	var head = document.getElementsByTagName('head')[0];
	for (var i = 0; i < editorFiles.length; i++) {
		head.removeChild(editorFiles[i]);
	}
}
var loading = 0;
startHtmlEditor = function(docType) {
	_originalDocType = docType;
	if (JSONIDENTIFIER) {
		if (loading < 1)
			loading = 1;
	}
	else {
		if (loading < 1) {
			loading = 1;
			loadjsfile('libjs/json2.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	if (JsonDataBinding) {
		if (loading < 2)
			loading = 2;
	}
	else {
		if (loading < 2) {
			loading = 2;
			loadjsfile('libjs/jsonDataBind.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	if (jscolor) {
		if (loading < 3)
			loading = 3;
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
			loadjsfile('libjs/htmleditor.js');
		}
		setTimeout(startHtmlEditor, 100);
		return;
	}
	sstartFileEditing();
}
