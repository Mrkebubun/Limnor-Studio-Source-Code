/*
	limnortv library -- Javascript
	Copyright Longflow Enterprises Ltd.
	2013
	
	Depends on: jsonDataBind.js, htmlEditorClient.js

	For designing treeview, not used at runtime.
*/
var HtmlEditorTreeview = HtmlEditorTreeview || {
	onCreatedObject: function(o, client) {
		client.addStdJsFile(1);
		if(client.isforIDE)
			client.addCssLinkFile('css/limnortv.css', true);
		else
			client.addCssLinkFile('/css/limnortv.css', true);
		o.className = 'limnortv';
		JsonDataBinding.createTreeView(o);
		o.jsData.designer = HtmlEditorTreeview.createDesigner(o, client);
	},
	designInit: function(pageEditor, calledClient) {
		var objs = document.getElementsByTagName('ul');
		if (objs) {
			for (var i = 0; i < objs.length; i++) {
				if (objs[i].className) {
					if (JsonDataBinding.hasClass(objs[i], 'limnortv')) {
						pageEditor.initPageElement.apply(pageEditor.editorWindow(), [objs[i]]);
						objs[i].jsData.designer = HtmlEditorTreeview.createDesigner(objs[i], calledClient);
					}
				}
			}
		}
	},
	cleanup: function(client) {
		var tvCssNode, i;
		var head = document.getElementsByTagName('head')[0];
		var cssLst = head.getElementsByTagName('link');
		if (cssLst) {
			for (i = 0; i < cssLst.length; i++) {
				var s = cssLst[i].getAttribute('href');
				if (JsonDataBinding.endsWithI(s, '/limnortv.css')) {
					tvCssNode = cssLst[i];
					break;
				}
			}
		}
		if (tvCssNode) {
			var selectors = new Array();
			var uls = document.getElementsByTagName('ul');
			if (uls) {
				for (i = 0; i < uls.length; i++) {
					var cssNames = uls[i].className;
					if (cssNames) {
						var found = false;
						var sy = '';
						var ns = cssNames.split(' ');
						for (var k = 0; k < ns.length; k++) {
							if (ns[k] == 'limnortv') {
								found = true;
							}
							else {
								if (ns[k] && ns[k].length > 0) {
									sy = ns[k];
								}
							}
						}
						if (found) {
							//selectors.push(sy);
							if (sy.length > 0) {
								selectors.push('ul.' + sy + '.limnortv');
								selectors.push('ul.limnortv.' + sy);
							}
							else {
								selectors.push('ul.limnortv');
							}
						}
					}
				}
			}
			if (selectors.length == 0) {
				head.removeChild(tvCssNode);
			}
			function removeTVstyles(tvSheet) {
				if (tvSheet) {
					var rs;
					if (tvSheet.cssRules) {
						rs = tvSheet.cssRules;
					}
					else if (tvSheet.rules) {
						rs = tvSheet.rules;
					}
					else {
						if (tvSheet.sheet) {
							if (tvSheet.sheet.cssRules) {
								rs = tvSheet.sheet.cssRules;
							}
							else if (tvSheet.sheet.rules) {
								rs = tvSheet.sheet.rules;
							}
						}
					}
					if (rs) {
						for (var i = 0; i < rs.length; i++) {
							if (rs[i].selectorText) {
								var sl = rs[i].selectorText.toLowerCase();
								var pos = sl.indexOf(' ');
								var sl2 = '';
								if (pos > 0) {
									sl2 = sl.substr(pos);
									sl = sl.substr(0, pos);
								}
								if (sl == 'ul.limnortv' || JsonDataBinding.startsWith(sl, 'ul.limnortv.')
								|| (JsonDataBinding.startsWith(sl, 'ul.') && JsonDataBinding.endsWith(sl, '.limnortv'))) {
									var found = false;
									for (var k = 0; k < selectors.length; k++) {
										if (selectors[k] == sl) {
											found = true;
											break;
										}
									}
									if (!found) {
										client.addToRemoveCssBySelector(rs[i].selectorText);
										if (sl != 'ul.limnortv') {
											if (JsonDataBinding.startsWith(sl, 'ul.limnortv.')) {
												sl = 'ul.' + sl.substr(12) + '.limnortv';
											}
											else {
												sl = 'ul.limnortv.' + sl.substr(3, sl.length - 12);
											}
										}
										client.addToRemoveCssBySelector(sl + sl2);
									}
								}
							}
						}
					}
				}
			}
			removeTVstyles(client.getPageCssLinkNode());
			removeTVstyles(client.getDynamicStyleNode());
			return (selectors.length > 0);
		}
	}
	, createDesigner: function(tv, client) {
		function _getSelectorHoverColor(st) {
			if (typeof st != 'undefined' && st.length > 0)
				return 'ul.limnortv.' + st + ' li[hoverstate]';
			else
				return 'ul.limnortv li[hoverstate]';
		}
		function _getSelectorHoverColor2(st) {
			if (typeof st != 'undefined' && st.length > 0)
				return 'ul.' + st + '.limnortv li[hoverstate]';
			else
				return 'ul.limnortv li[hoverstate]';
		}
		function _getSelectorTreeview(st) {
			if (typeof st != 'undefined' && st.length > 0)
				return 'ul.limnortv.' + st;
			else
				return 'ul.limnortv';
		}
		function _getSelectorTreeview2(st) {
			if (typeof st != 'undefined' && st.length > 0)
				return 'ul.' + st + '.limnortv';
			else
				return 'ul.limnortv';
		}
		function _getHoverBkColor() {
			var st = _getStyleName();
			if (typeof limnorPageData != 'undefined' && limnorPageData.limnortv) {
				if (st && st.length > 0) {
					if (limnorPageData.limnortv.hoverBkColors && limnorPageData.limnortv.hoverBkColors[st]) {
						return limnorPageData.limnortv.hoverBkColors[st];
					}
				}
				else {
					if (limnorPageData.limnortv.hoverBkColor) {
						return limnorPageData.limnortv.hoverBkColor;
					}
				}
			}
			var lnkNode = client.getPageCssStyle();
			if (lnkNode) {
				var rs;
				if (lnkNode.cssRules) {
					rs = lnkNode.cssRules;
				}
				else if (lnkNode.rules) {
					rs = lnkNode.rules;
				}
				if (rs) {
					var i;
					var selector = _getSelectorHoverColor(st);
					for (i = 0; i < rs.length; i++) {
						if (rs[i].selectorText == selector) {
							return client.getCssProperty(rs[i].cssText, 'background-color');
						}
					}
					selector = _getSelectorHoverColor2(st);
					for (i = 0; i < rs.length; i++) {
						if (rs[i].selectorText == selector) {
							return client.getCssProperty(rs[i].cssText, 'background-color');
						}
					}
				}
			}
		}
		function _setHoverBkColor(val) {
			var st = _getStyleName();
			if (typeof limnorPageData == 'undefined')
				limnorPageData = {};
			limnorPageData.limnortv = limnorPageData.limnortv || {};
			if (st && st.length > 0) {
				limnorPageData.limnortv.hoverBkColors = limnorPageData.limnortv.hoverBkColors || {};
				limnorPageData.limnortv.hoverBkColors[st] = val;
			}
			else {
				limnorPageData.limnortv.hoverBkColor = val;
			}
			var selector = _getSelectorHoverColor(st);
			var selector2 = _getSelectorHoverColor2(st);
			client.updateDynamicStyle(selector2, val, 'backgroundColor', 'background-color', selector);
			tv.jsData.enableHoverState(val != null);
		}
		function _getStyleName() {
			var cs = tv.className;
			if (cs) {
				var ns = cs.split(' ');
				for (var i = 0; i < ns.length; i++) {
					if (ns[i] && ns[i].length > 0 && ns[i] != 'limnortv') {
						return ns[i];
					}
				}
			}
		}
		function _setStyleName(val) {
			var curName = _getStyleName();
			if (val != curName) {
				if (typeof val != 'undefined' && val.length > 0) {
					if (val.indexOf('limnortv') >= 0) {
						alert('Please do not use limnortv in your style name');
					}
					else {
						if (client.HtmlEditor.IsNameValid(val)) {
							tv.className = 'limnortv ' + val;
						}
						else {
							alert('It is an invalid style name. Use alphanumeric and underscores, starting with an alphabetic or underscore.');
						}
					}
				}
				else {
					tv.className = 'limnortv';
				}
			}
		}
		function _setter(name, value) {
			var cssName = client.HtmlEditor.getCssNameFromPropertyName(name);
			if (cssName) {
				var st = _getStyleName();
				var selector = _getSelectorTreeview(st);
				var selector2 = _getSelectorTreeview2(st);
				client.updateDynamicStyle(selector2, value, name, cssName, selector);
			}
		}
		function _getter(name) {
			var cssName = client.HtmlEditor.getCssNameFromPropertyName(name);
			if (cssName) {
				var st = _getStyleName();
				var ss = new Array();
				ss.push(_getSelectorTreeview(st));
				if (st && st.length > 0) {
					ss.push(_getSelectorTreeview2(st));
				}
				var v = client.getStyleValue(ss, cssName);
				if (typeof v == 'undefined') {
					v = client.getElementStyleValue(tv, cssName);
				}
				v = client.HtmlEditor.removeQuoting(v);
				return v;
			}
		}
		function _hasSetter(name) {
			if (name == 'fontStyle' || name == 'fontWeight' || name == 'fontVariant' || name == 'color' || name == 'fontFamily'
			|| name == 'fontSize'
				)
				return true;
		}
		function _removeColor(obj, name, cssName) {
			if (name == 'color') {
				var st = _getStyleName();
				var selector = _getSelectorTreeview(st);
				var selector2 = _getSelectorTreeview2(st);
				client.updateDynamicStyle(selector2, null, 'color', 'color', selector);
			}
			else if (name == 'HoverBackColor') {
				_setHoverBkColor(null);
			}
		}
		return {
			//createSubEditor: function (o, c) {
			//}
			hasSetter: function(name) {
				return _hasSetter(name);
			}
			, setter: function(name, value) {
				_setter(name, value);
			}
			, hasGetter: function(name) {
				return _hasSetter(name);
			}
			, getter: function(name) {
				return _getter(name);
			}
			, getStyleName: function() {
				return _getStyleName();
			}
			, setStyleName: function(val) {
				_setStyleName(val);
			}
			, getHoverBkColor: function() {
				return _getHoverBkColor();
			}
			, setHoverBkColor: function(val) {
				_setHoverBkColor(val);
			}
			, removeColor: function(obj, name, cssName) {
				_removeColor(obj, name, cssName);
			}
		};
	}
};