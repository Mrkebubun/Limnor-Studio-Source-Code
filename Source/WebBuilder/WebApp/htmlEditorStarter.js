/*
	Html Editor Library -- JavaScript
	Copyright Longflow Enterprises Ltd
	2011

*/

var limnorHtmlEditor = limnorHtmlEditor || {
	serverType: 'php',
	showMessage: function (msg) {
		if (!document.divWaitMessage) {
			document.divWaitMessage = document.createElement('div');
			document.divWaitMessage.style.width = '500px';
			document.divWaitMessage.style.height = '309px';
			document.divWaitMessage.style.borderStyle = 'solid';
			document.divWaitMessage.style.borderRadius = '20px';
			document.divWaitMessage.style.position = 'absolute';
			document.divWaitMessage.style.backgroundColor = 'white';
			document.divWaitMessage.style.margin = '10px';
			document.divWaitMessage.style.padding = '10px';
			document.divWaitMessage.style.color = 'red';
			document.divWaitMessage.style.verticalAlign = 'middle';
			document.divWaitMessage.style.fontSize = 'large';
			document.body.appendChild(document.divWaitMessage);
		}
		var zi = JsonDataBinding.getPageZIndex(document.divWaitMessage) + 1;
		document.divWaitMessage.innerHTML = msg;
		document.divWaitMessage.style.zIndex = zi;
		document.divWaitMessage.style.display = 'block';
		JsonDataBinding.windowTools.centerElementOnScreen(document.divWaitMessage);
	},
	hideMessage: function () {
		if (document.divWaitMessage) {
			document.divWaitMessage.style.display = 'none';
		}
	},
	createPageEditor: function() {
		if (typeof (HtmlEditor) == 'undefined') {
			var headNode = document.getElementsByTagName('head')[0];
			var scriptNode = document.createElement('script');
			scriptNode.setAttribute('type', 'text/javascript');
			scriptNode.src = '/libjs/htmlEditor.js';
			headNode.appendChild(scriptNode);
		}
		var tryCount = 0;
		function createEditor() {
			if (typeof (HtmlEditor) == 'undefined') {
				tryCount++;
				if (tryCount > 30) {
					alert('Timeout loading Javascript');
				}
				else {
					setTimeout(createEditor, 200);
				}
				return;
			}
			if (!HtmlEditor.pageEditor) {
				HtmlEditor.pageEditor = HtmlEditor.createEditor();
			}
		}
		createEditor();
	},
	editHtmlFile: function (url, inlineOptions, pageId, serverFolder, userAlias) {
		var isforIDE = (inlineOptions ? (inlineOptions.forIDE ? true : false) : false);
		if (isforIDE) {
			return startEditor(false, false);
		}
		else {
			if (typeof limnorHtmlEditor.editorList == 'undefined') {
				limnorHtmlEditor.editorList = new Array();
			}
			if (url) {
				for (var i = 0; i < limnorHtmlEditor.editorList.length; i++) {
					if (limnorHtmlEditor.editorList[i].url == url) {
						if (limnorHtmlEditor.editorList[i].finished && limnorHtmlEditor.editorList[i].finished()) {
							limnorHtmlEditor.editorList.splice(i, 1);
							break;
						}
						else if (limnorHtmlEditor.editorList[i].activate) {
							limnorHtmlEditor.editorList[i].activate();
							return;
						}
					}
				}
			}
			var autosaveUrl = url ? url + '.autoSave.html' : null;
			if (autosaveUrl) {
				limnorHtmlEditor.editorList.push(
					{
						url: url
					});
				var clientProperties = {
					lang: JsonDataBinding.GetCulture()
				};
				JsonDataBinding.accessServer('WebPageUpdater.php', 'checkHtmlCacheFileExist', url, clientProperties, function () {
					return startEditor(JsonDataBinding.values.fileExists);
				});
			}
			else {
				autosaveUrl = 'about:blank';
				url = autosaveUrl;
				return startEditor(false, true);
			}
		}
		function startEditor(useSavedUrl, isInline) {
			var _editor;
			if (isInline) {
				//editor contained inside inlineOptions.holder
				//inlineOptions.holder.tdContentContainer is the td for containing div to be edited
				_editor = HtmlEditor.createEditor(inlineOptions.holder);
			}
			else {
				if (!HtmlEditor.pageEditor) {
					HtmlEditor.pageEditor = HtmlEditor.createEditor();
				}
				_editor = HtmlEditor.pageEditor;
			}
			JsonDataBinding.setupChildManager();
			document.childManager.onChildWindowReady = function(event) {
				var e = event || window.event || (document.parentWindow && document.parentWindow.event);
				var sender = JsonDataBinding.getSender(e);
				JsonDataBinding.AbortEvent = false;
			}
			var windowParams = {};
			windowParams.isDialog = false;
			if (isforIDE) {
				windowParams.forceNew = true;
				windowParams.url = url;
				windowParams.hideTitle = true;
				windowParams.center = true;
				windowParams.width = '100%';
				windowParams.height = '100%';
			}
			else if (isInline) {
				windowParams.url = url;
				windowParams.hideTitle = true;
				windowParams.contentsHolder = inlineOptions.holder.tdContentContainer;
				windowParams.width = '100%';
				windowParams.height = 0;
			}
			else {
				if (url == 'about:blank')
					windowParams.url = url;
				else
					windowParams.url = (useSavedUrl ? autosaveUrl : url) + '?htmleditor=1&r=' + Math.random();
				windowParams.center = true;
				windowParams.width = 970;
				windowParams.height = 600;
			}
			windowParams.top = 0;
			windowParams.left = 0;
			windowParams.hideCloseButtons = true;
			windowParams.resizable = !isInline;
			windowParams.border = '2px double #0000ff';
			windowParams.ieBorderOffset = 4;
			windowParams.icon = '/libjs/editor.png';
			windowParams.title = ' Web page - ' + url;
			if (typeof pageId != 'undefined' && pageId != 0) {
				windowParams.pageId = pageId;
			}
			else {
				windowParams.pageId = Math.floor((Math.random() * 1000000000) + 1);
			}
			if (!isInline) {
				limnorHtmlEditor.showMessage('Loading page editor, please wait. <br><br>If it takes too long to finish then you may close all web browser windows and restart.');
			}
			var childObj = JsonDataBinding.showChild(windowParams);
			if (!isInline) {
				var zi = JsonDataBinding.getPageZIndex(document.divWaitMessage) + 1;
				document.divWaitMessage.style.zIndex = zi;
				document.divWaitMessage.style.display = 'block';
			}
			var pageWindow = childObj.getPageWindow();
			var started = false;
			var nstartCount = 0;
			function start() {
				if (pageWindow.document && pageWindow.document.readyState == 'interactive') {
					nstartCount++;
					if (nstartCount > 3) {
						started = true;
					}
				}
				if (started || (pageWindow.document && pageWindow.document.readyState == 'complete')) {
					var headNode = pageWindow.document.getElementsByTagName('head')[0];
					var scriptNode = pageWindow.document.createElement('script');
					scriptNode.setAttribute('type', 'text/javascript');
					if(isforIDE)
						scriptNode.src = '/libjs/limnorStudioClient.js';
					else
						scriptNode.src = '/libjs/htmlEditorClient.js';
					headNode.appendChild(scriptNode);
					//
					var editorObj;
					var nLoadCount = 0;
					function openEditor() {
						editorObj = pageWindow.limnorHtmlEditorClient;
						if (!editorObj) {
							editorObj = pageWindow.document.limnorHtmlEditorClient;
						}
						if (editorObj) {
							editorObj.loadEditor.apply(pageWindow, [false, isInline]);
							//
							var headNode = pageWindow.document.getElementsByTagName('head')[0];
							headNode.removeChild(scriptNode);
							//
							function removeEditor() {
								if (limnorHtmlEditor.editorList) {
									for (var i = 0; i < limnorHtmlEditor.editorList.length; i++) {
										if (limnorHtmlEditor.editorList[i].url == url) {
											limnorHtmlEditor.editorList.splice(i, 1);
											break;
										}
									}
								}
							}
							function openClient() {
								if (editorObj.client) {
									if (editorObj.client.pageInitialized.apply(pageWindow)) {
										editorObj.client.HtmlEditor = HtmlEditor;
										editorObj.client.isforIDE = isforIDE;
										childObj.getPageHolder().style.boxShadow = '10px 10px 5px #888888';
										editorObj.client.setSavedUrlFlag(useSavedUrl, childObj.getPageHolder());
										var editorOptions = {
											forIDE:isforIDE, 
											useSavedUrl: useSavedUrl,
											wholePage: true,
											client: editorObj.client,
											objEdited: pageWindow,
											serverFolder: serverFolder,
											userAlias: userAlias,
											finishHandler: function(pageData) {
												var clientProperties = {};
												clientProperties.html = JsonDataBinding.base64Encode(pageData.html);
												clientProperties.css = JsonDataBinding.base64Encode(pageData.css);
												clientProperties.publish = !pageData.toCache;
												function onFinish(serverFailure) {
													if (serverFailure) {
														alert(serverFailure);
													}
													else {
														if (pageData.toCache) {
															editorOptions.lastsavetime = new Date();
															HtmlEditor.pageEditor.refreshProperties();
															if (pageData.manualInvoke) {
																if (confirm('The page modifications are saved.\r\n Do you want to finish editing this page now?\r\nClick Cancel if you want to continue editing this page.')) {
																	HtmlEditor.pageEditor.close();
																	removeEditor();
																}
															}
														}
														else {
															if (isforIDE) {
																if (typeof (limnorStudio) != 'undefined') limnorStudio.onUpdated();
																else window.external.OnUpdated();
															}
															else {
																HtmlEditor.pageEditor.close();
																removeEditor();
																//window.open(url + '?' + Math.random(), 'test_editing');
																if (limnorHtmlEditor.onPublishPage) {
																	limnorHtmlEditor.onPublishPage(pageId, url, pageData.url);
																}
															}
														}
													}
												}
												var saveTo = url;
												if (isforIDE) {
													var li = saveTo.lastIndexOf('/');
													if (li > 0) {
														saveTo = saveTo.substr(li + 1);
													}
												}
												if (_editor.saveToFolder) {
													clientProperties.saveToFolder = _editor.saveToFolder;
												}
												JsonDataBinding.accessServer('./WebPageUpdater.php', 'callSaveHtmlPage', saveTo, clientProperties, onFinish);
											},
											reset: function() {
												var clientProperties = {};
												function onReset(serverFailure) {
													if (serverFailure) {
														alert(serverFailure);
													}
													else {
														HtmlEditor.pageEditor.close();
														removeEditor();
													}
												}
												JsonDataBinding.accessServer('WebPageUpdater.php', 'removeCacheFiles', autosaveUrl, clientProperties, onReset);
											},
											deleteAutoSave: function() {
												JsonDataBinding.deleteWebFile(limnorHtmlEditor.serverType, autosaveUrl, function(errmsg) { if (errmsg) { alert(errmsg); } });
											},
											getPageHolder: function() {
												return childObj.getPageHolder();
											},
											closePage: function(cancel) {
												if (cancel)
													childObj.cancelDialog();
												else
													childObj.closeDialog();
												removeEditor();
											}
										};
										if (isInline) {
											editorOptions.wholePage = false;
											editorOptions.inline = true;
											editorOptions.finishHandler = function(pageData) {
												if (inlineOptions.holder.FinishedHtmlEdit) {
													inlineOptions.holder.FinishedHtmlEdit({ target: inlineOptions.holder }, inlineOptions.holder, pageData);
												}
											};
											editorOptions.cancelHandler = function() {
												if (inlineOptions.holder.CancelHtmlEdit) {
													inlineOptions.holder.CancelHtmlEdit({ target: inlineOptions.holder }, inlineOptions.holder);
												}
											};
										}
										else {
											editorOptions.initLocation = { x: 100, y: 100, w: 600, h: 350 };
											editorObj.client.resetDynamicStyles.apply(pageWindow);
										}
										editorOptions.docType = editorObj.client.getDocType.apply(pageWindow);
										_editor.setEditOptions(editorOptions, true);
										childObj.setOnBringToFront(function() {
											if (_editor) {
												_editor.setEditOptions(editorOptions);
												_editor.bringToFront();
											}
										});
										editorObj.client.setPageEditor.apply(pageWindow, [_editor, pageId]);
										if (isInline) {
											_editor.setInlineTarget(childObj);
										}
										else {
											editorObj.client.initEditor.apply(pageWindow, [_editor, editorObj.client]);
											//document.divWaitMessage.style.display = 'none';
											limnorHtmlEditor.hideMessage();
											if (limnorHtmlEditor.editorList && limnorHtmlEditor.editorList.length > 0) {
												for (var i = 0; i < limnorHtmlEditor.editorList.length; i++) {
													if (limnorHtmlEditor.editorList[i].url == url) {
														limnorHtmlEditor.editorList[i].finished = function () {
															if (childObj)
																return childObj.finished;
															return false;
														};
														limnorHtmlEditor.editorList[i].activate = function () {
															if (!childObj.finished) {
																childObj.bringToFront();
															}
														}
														break;
													}
												}
											}
										}
										if (isforIDE) {
											try{
												if (typeof (limnorStudio) != 'undefined') limnorStudio.onEditorStarted();
												else window.external.OnEditorStarted();
											}
											catch (err) {
												alert('start:'+err.message);
											}
										}
									}
									else
										setTimeout(openClient, 100);
								}
								else {
									setTimeout(openClient, 100);
								}
							}
							openClient();
						}
						else {
							nLoadCount++;
							if (nLoadCount < 10) {
								setTimeout(openEditor, 300);
							}
							else {
								if (typeof (limnorStudio) == 'undefined' || limnorStudio == null) {
									var s = 'A browser related error occurred. Your browser';
									if (JsonDataBinding.IsChrome()) {
										s = s + ', a Chrome, ';
									}
									else if (JsonDataBinding.IsSafari()) {
										s = s + ', a Safari, ';
									}
									s = s + ' could not load a javascript file for editing the page.';
									if (isforIDE) {
										s = s + 'The system will try it again.';
									}
									else {
										s = s + 'You may try it again or refresh the page-manager page.';
									}
									alert(s);
									childObj.cancelDialog();
									pageWindow.close();
								}
								else {
									if (isforIDE) {
										if (typeof(limnorStudio) != 'undefined') limnorStudio.onLoadEditorFailed();
										else window.external.OnLoadEditorFailed();
									}
								}
							}
						}
					}
					setTimeout(openEditor, 200);
				}
				else setTimeout(start, 30);
			}
			setTimeout(start, 300);
			return _editor;
		}
	}
};
var _vhe;
function limnorHtmlEditoreditHtmlFile(url) {
	if (typeof (url) == 'undefined' || url == null || url.length == 0) {
		var s = window.location.href;
		var pos = s.indexOf('?');
		if (pos > 0) {
			url = s.substr(pos + 1);
		}
	}
	if (typeof (url) != 'undefined' && url != null && url.length > 0) {
		_vhe = limnorHtmlEditor.editHtmlFile(url, { forIDE: true });
	}
}
function vheLoaded() {
	return typeof (_vhe) != 'undefined' && _vhe != null;
}
function setSelectedObject(guid) {
	_vhe.setSelectedObject(guid);
}
function getHtmlString() {
	return _vhe.getHtmlString();
}
function removePageEditor() {
	_vhe.removePageEditor();
}
function createOrGetId(guid) {
	return _vhe.createOrGetId(guid);
}
function setGuidById(id, guid) {
	return _vhe.setGuidById(id, guid);
}
function getTagNameById(id) {
	return _vhe.getTagNameById(id);
}
function getIdByGuid(guid) {
	return _vhe.getIdByGuid(guid);
}
function getIdList() {
	return _vhe.getIdList();
}
function getMaps() {
	return _vhe.getMaps();
}
function getAreas() {
	return _vhe.getAreas();
}
function createNewMap(guid) {
	return _vhe.createNewMap(guid);
}
function setMapAreas(mapId, areas) {
	_vhe.setMapAreas(mapId, areas);
}
function setUseMap(imgId, mapId) {
	_vhe.setUseMap(imgId, mapId);
}
function setPropertyValue(name, value) {
	_vhe.setPropertyValue(name, value);
}
function appendArchiveFile(name, filePath) {
	_vhe.appendArchiveFile(name, filePath);
}
function pageModified() {
	return _vhe.pageModified();
}
function saveAndFinish(targetFolder) {
	if (typeof (targetFolder) != 'undefined') {
		_vhe.saveToFolder = targetFolder;
	}
	_vhe.saveAndFinish();
}
function setbodyBk(bkFile, bkTile) {
	return _vhe.setbodyBk(bkFile, bkTile)
}
function setTargetFolder(folder) {
	_vhe.saveToFolder = folder;
}
function showEditorMessage(message) {
	_vhe.showEditorMessage(message);
}
function setDebug(debug) {
	JsonDataBinding.Debug = debug;
}
function onBeforeIDErun() {
	_vhe.onBeforeIDErun();
}
function doCopy() {
	_vhe.doCopy();
}
function doPaste() {
	_vhe.doPaste();
}
function pasteToHtmlInput(txt, selStart, selEnd) {
	_vhe.pasteToHtmlInput(txt, selStart, selEnd);
}
