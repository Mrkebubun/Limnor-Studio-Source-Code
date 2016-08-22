/*
	menubar library -- Javascript
	Copyright Longflow Enterprises Ltd.
	2013
	
	Depends on: jsonDataBind.js

	For designing menus, not used at runtime.
*/
var HtmlEditorMenuBar = HtmlEditorMenuBar || {
	limnorMenuStylesName: function() { return 'limnorstyles_menu'; },
	onCreatedObject: function(o, client) {
		client.addStdJsFile(1);
		o.jsData = HtmlEditorMenuBar.createMenuStyles(o);
		o.jsData.setMenuData();
		o.className = o.jsData.getClassName();
		var ul = document.createElement('ul');
		o.appendChild(ul);
		var li = document.createElement('li');
		ul.appendChild(li);
		var a = document.createElement('a');
		a.innerHTML = 'Home';
		li.appendChild(a);
		//
		li = document.createElement('li');
		ul.appendChild(li);
		a = document.createElement('a');
		a.innerHTML = 'Contact';
		li.appendChild(a);
	},
	designInit: function(pageEditor, calledClient) {
		var objs = document.getElementsByTagName('nav');
		if (objs) {
			tag = HtmlEditorMenuBar.limnorMenuStylesName();
			for (i = 0; i < objs.length; i++) {
				if (objs[i].className) {
					var cc = objs[i].className.split(' ');
					for (var j = 0; j < cc.length; j++) {
						if (JsonDataBinding.startsWith(cc[j], tag)) {
							HtmlEditorMenuBar.initializeMenuData(calledClient, objs[i]);
							break;
						}
					}
				}
			}
		}
	},
	cleanup: function(client) {
		var found = false;
		var uls = document.getElementsByTagName('nav');
		if (uls) {
			var tag = HtmlEditorMenuBar.limnorMenuStylesName();
			for (var i = 0; i < uls.length; i++) {
				var cssNames = uls[i].className;
				if (cssNames) {
					var ns = cssNames.split(' ');
					for (var k = 0; k < ns.length; k++) {
						if (JsonDataBinding.startsWithI(ns[k], tag)) {
							found = true;
							break;
						}
					}
					if (found) {
						break;
					}
				}
			}
		}
		return found;
	}
	, createMenuStyles: function(nav, menuData) {
		var _styleSheet;
		var mid;
		if (!menuData) {
			menuData = {};
			mid = HtmlEditorMenuBar.limnorMenuStylesName();
			menuData.fontFamily = '';
			menuData.fontSize = '';
			menuData.classNames = '';
		}
		else {
			mid = HtmlEditorMenuBar.limnorMenuStylesName() + (menuData.menuId ? menuData.menuId : '');
		}
		if (typeof menuData.fullwidth == 'undefined') menuData.fullwidth = false;
		if (!menuData.menubarHoverTextColor) menuData.menubarHoverTextColor = '#fff';
		if (!menuData.anchorTextColor) menuData.anchorTextColor = '#757575';
		if (!menuData.anchorPaddingY) menuData.anchorPaddingY = 10;
		if (!menuData.anchorPaddingX) menuData.anchorPaddingX = 40;
		if (!menuData.menubarUpperBkColor) menuData.menubarUpperBkColor = '#efefef';
		if (!menuData.menubarLowerBkColor) menuData.menubarLowerBkColor = '#bbbbbb';
		if (!menuData.menubarRadius) menuData.menubarRadius = 10;
		if (!menuData.menuItemHoverUpperBkColor) menuData.menuItemHoverUpperBkColor = '#4f5964';
		if (!menuData.menuItemHoverLowerBkColor) menuData.menuItemHoverLowerBkColor = '#bbbbbb';
		//
		if (!menuData.itemHoverUpperBkColor) menuData.itemHoverUpperBkColor = '#add8e6';
		if (!menuData.itemHoverLowerBkColor) menuData.itemHoverLowerBkColor = '#bbbbbb';
		//
		if (!menuData.dropdownUpperBkColor) menuData.dropdownUpperBkColor = '#5f6975';
		if (!menuData.dropdownLowerBkColor) menuData.dropdownLowerBkColor = '#bbbbbb';
		if (!menuData.itemRadius) menuData.itemRadius = 10;
		//
		if (!menuData.itemPaddingX) menuData.itemPaddingX = 40;
		if (!menuData.itemPaddingY) menuData.itemPaddingY = 8;
		if (!menuData.itemTextColor) menuData.itemTextColor = '#fff';
		//
		if (!menuData.marginTop && menuData.marginTop != 0) menuData.marginTop = 2;
		if (!menuData.marginBottom && menuData.marginBottom != 0) menuData.marginBottom = 2;
		//
		if (!menuData.classNames) menuData.classNames = '';
		//
		var _selectorMenubarFullWidth = 'nav.' + mid + ' > ul';
		var _selectorMenuArrow = 'nav.' + mid + ' ul a[isParent=1]::after';
		var _selectorMenubar = 'nav.' + mid + ' ul';
		var _selectorMenubarAfter = 'nav.' + mid + ' ul::after';
		var _selectorMenubarItem = 'nav.' + mid + ' ul li';
		var _selectorMenubarHover = 'nav.' + mid + ' ul li:hover';
		var _selectorHoverText = 'nav.' + mid + ' ul li:hover a';
		var _selectorItemHoverMenubar = 'nav.' + mid + ' ul li:hover > ul';
		var _selectorAnchor = 'nav.' + mid + ' ul li a';
		var _selectorDropdown = 'nav.' + mid + ' ul ul';
		var _selectorItem = 'nav.' + mid + ' ul ul li';
		var _selectorItemAnchor = 'nav.' + mid + ' ul ul li a';
		var _selectorItemHover = 'nav.' + mid + ' ul ul li a:hover';
		var _selectorSubItem = 'nav.' + mid + ' ul ul ul';
		//
		function setSelectorNames() {
			_selectorMenubarFullWidth = 'nav.' + mid + ' > ul';
			_selectorMenuArrow = 'nav.' + mid + ' ul a[isparent]::after';
			_selectorMenubar = 'nav.' + mid + ' ul';
			_selectorMenubarAfter = 'nav.' + mid + ' ul::after';
			_selectorMenubarItem = 'nav.' + mid + ' ul li';
			_selectorMenubarHover = 'nav.' + mid + ' ul li:hover';
			_selectorHoverText = 'nav.' + mid + ' ul li:hover a';
			_selectorItemHoverMenubar = 'nav.' + mid + ' ul li:hover > ul';
			_selectorAnchor = 'nav.' + mid + ' ul li a';
			_selectorDropdown = 'nav.' + mid + ' ul ul';
			_selectorItem = 'nav.' + mid + ' ul ul li';
			_selectorItemAnchor = 'nav.' + mid + ' ul ul li a';
			_selectorItemHover = 'nav.' + mid + ' ul ul li a:hover';
			_selectorSubItem = 'nav.' + mid + ' ul ul ul';
		}
		setSelectorNames();
		//
		function getCssMenubarFullWidth() {
			if (menuData.fullwidth)
				return _selectorMenubarFullWidth + '{width:100%;}';
			return _selectorMenubarFullWidth + '{width:auto;}';
		}
		function getCssMenubarArrow() {
			return _selectorMenuArrow + '{' +
					'content: url("/libjs/arrow.gif"); display: block;position:absolute;top:40%;left:90%;' +
					'}';
		}
		function getCssMenubar() {
			return _selectorMenubar + '{' +
					'background-image: linear-gradient(top, ' + menuData.menubarUpperBkColor + ' 0%, ' + menuData.menubarLowerBkColor + ' 100%);' +
					'background-image: -o-linear-gradient(top, ' + menuData.menubarUpperBkColor + ' 0%, ' + menuData.menubarLowerBkColor + ' 100%);' +
					'background-image: -moz-linear-gradient(top, ' + menuData.menubarUpperBkColor + ' 0%, ' + menuData.menubarLowerBkColor + ' 100%);' +
					'background-image: -webkit-linear-gradient(top, ' + menuData.menubarUpperBkColor + ' 0%, ' + menuData.menubarLowerBkColor + ' 100%);' +
					'background-image: -ms-linear-gradient(top, ' + menuData.menubarUpperBkColor + ' 0%, ' + menuData.menubarLowerBkColor + ' 100%);' +
					'box-shadow: 0px 0px 9px rgba(0,0,0,0.15);' +
					'padding: 0 20px;' +
					'border-top-left-radius: ' + menuData.menubarRadius + 'px;' +
					'border-top-right-radius: ' + menuData.menubarRadius + 'px;' +
					'border-bottom-right-radius: ' + menuData.menubarRadius + 'px;' +
					'border-bottom-left-radius: ' + menuData.menubarRadius + 'px;' +
					'list-style: none;' +
					'position: relative;' +
					'display: inline-table;' +
					'margin-top: ' + menuData.marginTop + 'px;' +
					'margin-bottom: ' + menuData.marginBottom + 'px;' +
					'}';
		}
		function getCssMenubarAfter() {
			return _selectorMenubarAfter + '{content: ""; clear: both; display: block;}';
		}
		function getCssMenubarItem() {
			return _selectorMenubarItem + '{float: left;}';
		}
		function getCssMenubarHover() {
			return _selectorMenubarHover + '{' +
				'background-image: linear-gradient(top, ' + menuData.menuItemHoverUpperBkColor + ' 0%, ' + menuData.menuItemHoverLowerBkColor + ' 100%);' +
				'background-image: -o-linear-gradient(top, ' + menuData.menuItemHoverUpperBkColor + ' 0%, ' + menuData.menuItemHoverLowerBkColor + ' 100%);' +
				'background-image: -moz-linear-gradient(top, ' + menuData.menuItemHoverUpperBkColor + ' 0%, ' + menuData.menuItemHoverLowerBkColor + ' 100%);' +
				'background-image: -webkit-linear-gradient(top, ' + menuData.menuItemHoverUpperBkColor + ' 0%, ' + menuData.menuItemHoverLowerBkColor + ' 100%);' +
				'background-image: -ms-linear-gradient(top , ' + menuData.menuItemHoverUpperBkColor + ' 0%, ' + menuData.menuItemHoverLowerBkColor + ' 100%);' +
				'box-shadow: 0px 0px 9px rgba(0,0,0,0.15);' +
				'border-top-left-radius: ' + menuData.menubarRadius + 'px;' +
				'border-top-right-radius: ' + menuData.menubarRadius + 'px;' +
				'border-bottom-right-radius: ' + menuData.menubarRadius + 'px;' +
				'border-bottom-left-radius: ' + menuData.menubarRadius + 'px;' +

				'}';
		}
		function getCssMenubarHoverTextColor() {
			return _selectorHoverText + '{color: ' + menuData.menubarHoverTextColor + ';}';
		}
		function getCssItemHoverMenubar() {
			return _selectorItemHoverMenubar + '{display: block;}';
		}
		function getCssMenubarItemText() {
			return _selectorAnchor + '{' +
				'padding-top: ' + menuData.anchorPaddingY + 'px;' +
				'padding-bottom: ' + menuData.anchorPaddingY + 'px;' +
				'padding-left: ' + menuData.anchorPaddingX + 'px;' +
				'padding-right: ' + menuData.anchorPaddingX + 'px;' +
				'color: ' + menuData.anchorTextColor + '; text-decoration: none; display: block;cursor:pointer;' +
				(menuData.fontFamily ? 'font-family:' + menuData.fontFamily + ';' : '') + (menuData.fontSize ? 'font-size:' + menuData.fontSize + ';' : '') +
				'}';
		}
		function getCssDropDown() {
			return _selectorDropdown + '{' +
				'background-image: linear-gradient(top, ' + menuData.dropdownUpperBkColor + ' 0%, ' + menuData.dropdownLowerBkColor + ' 100%);' +
				'background-image: -o-linear-gradient(top, ' + menuData.dropdownUpperBkColor + ' 0%, ' + menuData.dropdownLowerBkColor + ' 100%);' +
				'background-image: -moz-linear-gradient(top, ' + menuData.dropdownUpperBkColor + ' 0%, ' + menuData.dropdownLowerBkColor + ' 100%);' +
				'background-image: -webkit-linear-gradient(top, ' + menuData.dropdownUpperBkColor + ' 0%, ' + menuData.dropdownLowerBkColor + ' 100%);' +
				'background-image: -ms-linear-gradient(top, ' + menuData.dropdownUpperBkColor + ' 0%, ' + menuData.dropdownLowerBkColor + ' 100%);' +
				'border-top-left-radius: ' + menuData.itemRadius + 'px;' +
				'border-top-right-radius: ' + menuData.itemRadius + 'px;' +
				'border-bottom-right-radius: ' + menuData.itemRadius + 'px;' +
				'border-bottom-left-radius: ' + menuData.itemRadius + 'px;' +
				'padding: 0; margin-top:-2px; ' +
				'position: absolute; top: 100%;' +
				'display: none;' +
				'}';
		}
		function getCssDropdownItem() {
			return _selectorItem + '{' +
				'float: none;' +
				'border-top: 1px solid #6b727c;' +
				'border-bottom: 1px solid #575f6a;' +
				'position: relative;' +
				'border-radius: ' + menuData.itemRadius + 'px;' +
				'}';
		}
		function getCssItemAnchor() {
			return _selectorItemAnchor + '{' +
				'padding-top: ' + menuData.itemPaddingY + 'px;' +
				'padding-bottom: ' + menuData.itemPaddingY + 'px;' +
				'padding-left: ' + menuData.itemPaddingX + 'px;' +
				'padding-right: ' + menuData.itemPaddingX + 'px;' +
				'color: ' + menuData.itemTextColor + ';' +
				'border-radius: ' + menuData.itemRadius + 'px;' +
				'}';
		}
		function getCssItemHover() {
			return _selectorItemHover + '{' +
				'background-image: linear-gradient(top, ' + menuData.itemHoverUpperBkColor + ' 0%, ' + menuData.itemHoverLowerBkColor + ' 100%);' +
				'background-image: -o-linear-gradient(top, ' + menuData.itemHoverUpperBkColor + ' 0%, ' + menuData.itemHoverLowerBkColor + ' 100%);' +
				'background-image: -moz-linear-gradient(top, ' + menuData.itemHoverUpperBkColor + ' 0%, ' + menuData.itemHoverLowerBkColor + ' 100%);' +
				'background-image: -webkit-linear-gradient(top, ' + menuData.itemHoverUpperBkColor + ' 0%,' + menuData.itemHoverLowerBkColor + ' 100%);' +
				'background-image: -ms-linear-gradient(top , ' + menuData.itemHoverUpperBkColor + ' 0%, ' + menuData.itemHoverLowerBkColor + ' 100%);' +
				'border-radius: ' + menuData.itemRadius + 'px;' +
				'}';
		}
		function getCssSubItem() {
			return _selectorSubItem + '{position: absolute; left: 100%; top:0;}';
		}
		////////////////////////////////////
		function getMenuStyle() {
			if (!_styleSheet) {
				//				for (var s = 0; s < document.styleSheets.length; s++) {
				//					if (document.styleSheets[s].title == limnorHtmlEditorClient.limnorDynaStyleTitle) {
				//						_styleSheet = document.styleSheets[s];
				//						break;
				//					}
				//				}
				_styleSheet = limnorHtmlEditorClient.getDynamicStyleNode();
			}
			if (!_styleSheet) {
				var st = document.createElement('style');
				st.title = limnorHtmlEditorClient.limnorDynaStyleTitle;
				st.setAttribute('hidden', 'true');
				document.getElementsByTagName('head')[0].appendChild(st);
				for (var s = 0; s < document.styleSheets.length; s++) {
					if (document.styleSheets[s].title == limnorHtmlEditorClient.limnorDynaStyleTitle) {
						_styleSheet = document.styleSheets[s];
						break;
					}
				}
			}
			return _styleSheet;
		}
		function getRuleBySelector(selector) {
			var st = getMenuStyle();
			var rs;
			if (st.cssRules) {
				rs = st.cssRules;
			}
			else if (st.rules) {
				rs = st.rules;
			}
			if (rs) {
				for (var r = 0; r < rs.length; r++) {
					if (rs[r].selectorText == selector) {
						return { rule: rs[r], idx: r };
					}
				}
			}
		}
		function _setRule(selector, ruleGetter) {
			var rule = getRuleBySelector(selector);
			var idx;
			if (rule) {
				if (_styleSheet.deleteRule) {
					_styleSheet.deleteRule(rule.idx);
				}
				else {
					_styleSheet.removeRule(rule.idx);
				}
				idx = rule.idx;
			}
			else {
				if (_styleSheet.cssRules) {
					idx = _styleSheet.cssRules.length;
				}
				else if (_styleSheet.rules) {
					idx = _styleSheet.rules.length;
				}
				else {
					idx = 0;
				}
			}
			if (_styleSheet.insertRule) {
				_styleSheet.insertRule(ruleGetter(), idx);
			}
			else {
				_styleSheet.addRule(selector, ruleGetter(), idx);
			}
		}
		//////////////////////////////////////////////////
		function _setCssMenubarFullWidth() {
			_setRule(_selectorMenubarFullWidth, getCssMenubarFullWidth);
		}
		function _setCssMenubarArrow() {
			_setRule(_selectorMenuArrow, getCssMenubarArrow);
		}
		function _setCssMenubar() {
			_setRule(_selectorMenubar, getCssMenubar);
		}
		function _setCssMenubarAfter() {
			_setRule(_selectorMenubarAfter, getCssMenubarAfter);
		}
		function _setCssMenubarItem() {
			_setRule(_selectorMenubarItem, getCssMenubarItem);
		}
		function _setCssMenubarHover() {
			_setRule(_selectorMenubarHover, getCssMenubarHover);
		}
		function _setCssMenubarHoverTextColor() {
			_setRule(_selectorHoverText, getCssMenubarHoverTextColor);
		}
		function _setCssItemHoverMenubar() {
			_setRule(_selectorItemHoverMenubar, getCssItemHoverMenubar);
		}
		function _getClassName(){
			if (menuData.classNames && menuData.classNames.length > 0) {
				return menuData.classNames + ' ' + mid;
			}
			return mid;
		}
		function _setMenubarClasses() {
			nav.className = _getClassName();
		}
		function _setCssMenubarItemText() {
			_setRule(_selectorAnchor, getCssMenubarItemText);
		}
		function _setCssDropdown() {
			_setRule(_selectorDropdown, getCssDropDown);
		}
		function _setCssDropdownItem() {
			_setRule(_selectorItem, getCssDropdownItem);
		}
		function _setCssItemAnchor() {
			_setRule(_selectorItemAnchor, getCssItemAnchor);
		}
		function _setCssItemHover() {
			_setRule(_selectorItemHover, getCssItemHover);
		}
		function _setCssSubItem() {
			_setRule(_selectorSubItem, getCssSubItem);
		}
		////////////////////////////////////////////////////////////////////////
		//initialize styles
		function _init() {
			mid = HtmlEditorMenuBar.limnorMenuStylesName() + (menuData.menuId ? menuData.menuId : '');
			setSelectorNames();
			//
			_setMenubarClasses();
			_setCssMenubarFullWidth();
			_setCssMenubarArrow();
			_setCssMenubar();
			_setCssMenubarAfter();
			_setCssMenubarItem();
			_setCssMenubarHover();
			_setCssMenubarHoverTextColor();
			_setCssItemHoverMenubar();
			_setCssMenubarItemText();
			_setCssDropdown();
			_setCssDropdownItem();
			_setCssItemAnchor();
			_setCssItemHover();
			_setCssSubItem();
		}
		////////////////////////////////////////////////////////////////////////
		return {
			typename: 'menubar',
			createSubEditor: function(o, e) {
				return o;
			},
			setMenuData: function(data) {
				if (data) {
					menuData = data;
				}
				_init();
			},
			getFullWidth: function() {
				return menuData.fullwidth;
			},
			setFullWidth: function(v) {
				menuData.fullwidth = JsonDataBinding.isValueTrue(v);
				_setCssMenubarFullWidth();
			},
			getClassNames: function () {
				return menuData.classNames;
			},
			setClassNames: function (v) {
				if (menuData.classNames != v) {
					menuData.classNames = v;
					_setMenubarClasses();
				}
			},
			getFontFamily: function() {
				return menuData.fontFamily;
			},
			setFontFamily: function(v) {
				if (menuData.fontFamily != v) {
					menuData.fontFamily = v;
					_setCssMenubarItemText();
				}
			},
			getFontSize: function() {
				return menuData.fontSize;
			},
			setFontSize: function(v) {
				if (menuData.fontSize != v) {
					menuData.fontSize = v;
					_setCssMenubarItemText();
				}
			},
			getMenubarMarginTop: function () {
				return menuData.marginTop;
			},
			setMenubarMarginTop: function (v) {
				if (menuData.marginTop != v) {
					menuData.marginTop = v;
					_setCssMenubar();
				}
			},
			getMenubarMarginBottom: function () {
				return menuData.marginBottom;
			},
			setMenubarMarginBottom: function (v) {
				if (menuData.marginBottom != v) {
					menuData.marginBottom = v;
					_setCssMenubar();
				}
			},
			getMenubarPaddingX: function() {
				return menuData.anchorPaddingX;
			},
			setMenubarPaddingX: function(v) {
				if (menuData.anchorPaddingX != v) {
					menuData.anchorPaddingX = v;
					_setCssMenubarItemText();
				}
			},
			getMenubarPaddingY: function() {
				return menuData.anchorPaddingY;
			},
			setMenubarPaddingY: function(v) {
				if (menuData.anchorPaddingY != v) {
					menuData.anchorPaddingY = v;
					_setCssMenubarItemText();
				}
			},
			getMenubarRadius: function() {
				return menuData.menubarRadius;
			},
			setMenubarRadius: function(v) {
				if (menuData.menubarRadius != v) {
					menuData.menubarRadius = v;
					_setCssMenubar();
					_setCssMenubarHover();
				}
			},
			getMenubarTextColor: function() {
				return menuData.anchorTextColor;
			},
			setMenubarTextColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.anchorTextColor != v) {
						menuData.anchorTextColor = v;
						_setCssMenubarItemText();
					}
				}
			},
			getMenubarHoverTextColor: function() {
				return menuData.menubarHoverTextColor;
			},
			setMenubarHoverTextColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.menubarHoverTextColor != v) {
						menuData.menubarHoverTextColor = v;
						_setCssMenubarHoverTextColor();
					}
				}
			},
			getMenubarBkColor: function() {
				return menuData.menubarUpperBkColor;
			},
			setMenubarBkColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.menubarUpperBkColor != v) {
						menuData.menubarUpperBkColor = v;
						_setCssMenubar();
					}
				}
			},
			getMenubarGradientColor: function() {
				return menuData.menubarLowerBkColor;
			},
			setMenubarGradientColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.menubarLowerBkColor != v) {
						menuData.menubarLowerBkColor = v;
						_setCssMenubar();
					}
				}
			},
			getMenubarHoverBkColor: function() {
				return menuData.menuItemHoverUpperBkColor;
			},
			setMenubarHoverBkColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.menuItemHoverUpperBkColor != v) {
						menuData.menuItemHoverUpperBkColor = v;
						_setCssMenubarHover();
					}
				}
			},
			getMenubarHoverGradientColor: function() {
				return menuData.menuItemHoverLowerBkColor;
			},
			setMenubarHoverGradientColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.menuItemHoverLowerBkColor != v) {
						menuData.menuItemHoverLowerBkColor = v;
						_setCssMenubarHover();
					}
				}
			},
			getItemHoverBkColor: function() {
				return menuData.itemHoverUpperBkColor;
			},
			setItemHoverBkColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.itemHoverUpperBkColor != v) {
						menuData.itemHoverUpperBkColor = v;
						_setCssItemHover();
					}
				}
			},
			getItemHoverGradientColor: function() {
				return menuData.itemHoverLowerBkColor;
			},
			setItemHoverGradientColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.itemHoverLowerBkColor != v) {
						menuData.itemHoverLowerBkColor = v;
						_setCssItemHover();
					}
				}
			},
			//
			getDropdownBkColor: function() {
				return menuData.dropdownUpperBkColor;
			},
			setDropdownBkColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.dropdownUpperBkColor != v) {
						menuData.dropdownUpperBkColor = v;
						_setCssDropdown();
					}
				}
			},
			getDropdownGradientColor: function() {
				return menuData.dropdownLowerBkColor;
			},
			setDropdownGradientColor: function(v) {
				if (v) {
					if (v.charAt(0) != '#' && v.charAt(0) != 'r') {
						v = '#' + v;
					}
					if (menuData.dropdownLowerBkColor != v) {
						menuData.dropdownLowerBkColor = v;
						_setCssDropdown();
					}
				}
			},
			getItemPaddingX: function() {
				return menuData.itemPaddingX;
			},
			setItemPaddingX: function(v) {
				if (menuData.itemPaddingX != v) {
					menuData.itemPaddingX = v;
					_setCssItemAnchor();
				}
			},
			getItemPaddingY: function() {
				return menuData.itemPaddingY;
			},
			setItemPaddingY: function(v) {
				if (menuData.itemPaddingY != v) {
					menuData.itemPaddingY = v;
					_setCssItemAnchor();
				}
			},
			getItemRadius: function() {
				return menuData.itemRadius;
			},
			setItemRadius: function(v) {
				if (menuData.itemRadius != v) {
					menuData.itemRadius = v;
					_setCssDropdown();
					_setCssDropdownItem();
					_setCssItemAnchor();
					_setCssItemHover();
				}
			},
			getClassName: function () {
				return _getClassName();
			},
			getMenuData: function() {
				return menuData;
			},
			setStylesName: function(v) {
			},
			loadFromStyles: function(styles, client) {
				if (styles) {
					var rs;
					if (styles.cssRules) {
						rs = styles.cssRules;
					}
					else if (styles.rules) {
						rs = styles.rules;
					}
					if (rs) {
						function getbackground(txt) {
							var ret = {};
							var pos, pos2, v;
							var val = client.getCssProperty(txt, 'background');
							if (!val) {
								val = client.getCssProperty(txt, 'background-image');
							}
							if (val) {
								val = val.trim();
								pos = val.indexOf('linear-gradient');
								if (pos >= 0) {
									pos = val.indexOf('(', pos + 15);
									if (pos >= 0) {
										var pos2 = val.lastIndexOf(')');
										if (pos2 > pos) {
											v = val.substr(pos + 1, pos2 - pos - 1);
											pos = v.indexOf('rgb');
											if (pos >= 0) {
												pos2 = v.indexOf(')', pos);
												if (pos2 > 0) {
													ret.color1 = v.substr(pos, pos2 - pos + 1);
													pos = v.indexOf('rgb', pos2);
													if (pos > 0) {
														pos2 = v.indexOf(')', pos);
														if (pos2 > 0) {
															ret.color2 = v.substr(pos, pos2 - pos + 1);
														}
													}
												}
											}
										}
									}
								}
							}
							val = client.getCssProperty(txt, 'border-radius');
							if (!val) {
								val = client.getCssProperty(txt, 'border-top-left-radius');
							}
							if (val) {
								val = val.trim();
								pos = val.indexOf('px');
								if (pos >= 0) {
									if (pos > 0) {
										ret.radius = val.substr(0, pos);
									}
									else
										ret.radius = 0;
								}
								else
									ret.radius = val;
							}
							return ret;
						}
						function getTextAttrs(txt) {
							var ret = {};
							var pos;
							var val = client.getCssProperty(txt, 'padding');
							if (val) {
								val = val.trim();
								var vs = val.split(' ');
								if (vs.length > 0) {
									pos = vs[0].indexOf('px');
									if (pos >= 0) {
										if (pos > 0)
											ret.padY = vs[0].substr(0, pos);
										else
											ret.padY = 0;
									}
									else
										ret.padY = vs[0];
									if (vs.length > 1) {
										pos = vs[1].indexOf('px');
										if (pos >= 0) {
											if (pos > 0)
												ret.padX = vs[1].substr(0, pos);
											else
												ret.padX = 0;
										}
										else
											ret.padX = vs[1];
									}
								}
							}
							else {
								val = client.getCssProperty(txt, 'padding-top');
								if (val) {
									pos = val.indexOf('px');
									if (pos >= 0) {
										if (pos > 0)
											ret.padY = val.substr(0, pos);
										else
											ret.padY = 0;
									}
									else
										ret.padY = val;
								}
								val = client.getCssProperty(txt, 'padding-left');
								if (val) {
									pos = val.indexOf('px');
									if (pos >= 0) {
										if (pos > 0)
											ret.padX = val.substr(0, pos);
										else
											ret.padX = 0;
									}
									else
										ret.padX = val;
								}
							}
							val = client.getCssProperty(txt, 'color');
							if (val) {
								ret.color = val.trim();
							}
							val = client.getCssProperty(txt, 'font-family');
							if (val) {
								val = val.trim();
								if (val.length > 0) {
									if (val.substr(0, 1) == '"' || val.substr(0, 1) == "'") {
										val = val.substr(1);
									}
								}
								if (val.length > 0) {
									if (val.substr(val.length - 1, 1) == '"' || val.substr(val.length - 1, 1) == "'") {
										val = val.substr(0, val.length - 1);
									}
								}
								ret.fontFamily = val.trim();
							}
							val = client.getCssProperty(txt, 'font-size');
							if (val) {
								ret.fontSize = val.trim();
							}
							return ret;
						}
						for (var r = 0; r < rs.length; r++) {
							if (rs[r].selectorText && rs[r].selectorText.length > 0) {
								var val, pos;
								var s = rs[r].selectorText.toLowerCase();
								if (s == _selectorMenubarFullWidth) {
									val = client.getCssProperty(rs[r].cssText, 'width');
									if (val == 'auto')
										menuData.fullwidth = false;
									else if (val == '100%')
										menuData.fullwidth = true;
									else
										menuData.fullwidth = false;
								}
								else if (s == _selectorMenubar) {
									val = getbackground(rs[r].cssText);
									if (val) {
										if (val.color1)
											menuData.menubarUpperBkColor = val.color1;
										if (val.color2)
											menuData.menubarLowerBkColor = val.color2;
										if (typeof val.radius != 'undefined')
											menuData.menubarRadius = val.radius;
									}
									val = client.getCssProperty(rs[r].cssText, 'margin-top');
									if (typeof (val) != 'undefined' && val != null && val.length > 0) {
										if (val.length > 1 && val.substr(val.length-2) == 'px') {
											val = val.substr(0, val.length - 2).trim();
										}
										menuData.marginTop = parseInt(val);
									}
									val = client.getCssProperty(rs[r].cssText, 'margin-bottom');
									if (typeof (val) != 'undefined' && val != null && val.length > 0) {
										if (val.length > 1 && val.substr(val.length - 2) == 'px') {
											val = val.substr(0, val.length - 2).trim();
										}
										menuData.marginBottom = parseInt(val);
									}
								}
								else if (s == _selectorMenubarHover) {
									val = getbackground(rs[r].cssText);
									if (val) {
										if (val.color1)
											menuData.menuItemHoverUpperBkColor = val.color1;
										if (val.color2)
											menuData.menuItemHoverLowerBkColor = val.color2;
										if (typeof val.radius != 'undefined')
											menuData.menubarRadius = val.radius;
									}
								}
								else if (s == _selectorHoverText) {
									val = client.getCssProperty(rs[r].cssText, 'color');
									if (val) {
										menuData.menubarHoverTextColor = val;
									}
								}
								else if (s == _selectorAnchor) {
									val = getTextAttrs(rs[r].cssText);
									if (val) {
										if (typeof val.padX != 'undefined')
											menuData.anchorPaddingX = val.padX;
										if (typeof val.padY != 'undefined')
											menuData.anchorPaddingY = val.padY;
										if (val.fontFamily)
											menuData.fontFamily = val.fontFamily;
										if (val.fontSize)
											menuData.fontSize = val.fontSize;
										if (val.color)
											menuData.anchorTextColor = val.color;
									}
								}
								else if (s == _selectorDropdown) {
									val = getbackground(rs[r].cssText);
									if (val) {
										if (val.color1)
											menuData.dropdownUpperBkColor = val.color1;
										if (val.color2)
											menuData.dropdownLowerBkColor = val.color2;
										if (typeof val.radius != 'undefined')
											menuData.itemRadius = val.radius;
									}
								}
								else if (s == _selectorItemAnchor) {
									val = getTextAttrs(rs[r].cssText);
									if (val) {
										if (typeof val.padX != 'undefined')
											menuData.itemPaddingX = val.padX;
										if (typeof val.padY != 'undefined')
											menuData.itemPaddingY = val.padY;
									}
								}
								else if (s == _selectorItemHover) {
									val = getbackground(rs[r].cssText);
									if (val) {
										if (val.color1)
											menuData.itemHoverUpperBkColor = val.color1;
										if (val.color2)
											menuData.itemHoverLowerBkColor = val.color2;
										if (typeof val.radius != 'undefined')
											menuData.itemRadius = val.radius;
									}
								}
							}
						}
					}
				}
			}
		};
	},
	//===end of createMenuStyles==========
	initializeMenuData: function(client, nav) {
		var menuClass;
		var menuClasses = nav.className;
		if (menuClasses) {
			var cs = menuClasses.split(' ');
			if (cs) {
				for (var i = 0; i < cs.length; i++) {
					if (JsonDataBinding.startsWith(cs[i], HtmlEditorMenuBar.limnorMenuStylesName())) {
						menuClass = cs[i];
						break;
					}
				}
			}
		}
		if (menuClass) {
			var menuData = {};
			menuData.menuId = menuClass.substr(HtmlEditorMenuBar.limnorMenuStylesName().length);
			menuData.classNames = menuClasses.replace(menuClass, '');
			nav.typename = 'menubar';
			nav.jsData = HtmlEditorMenuBar.createMenuStyles(nav, menuData);
			var pageStyle = client.getPageCssLinkNode();
			nav.jsData.loadFromStyles(pageStyle.sheet ? pageStyle.sheet : pageStyle.styleSheet, client);
		}
	},
	//===end of initializeMenuData===
	onBeforeSave: function(dyStyleNode, client) {
		if (dyStyleNode) {
			var rs;
			if (dyStyleNode.cssRules) {
				rs = dyStyleNode.cssRules;
			}
			else if (dyStyleNode.rules) {
				rs = dyStyleNode.rules;
			}
			if (rs) {
				var r, k, b;
				var menuIds = new Array();
				for (r = 0; r < rs.length; r++) {
					if (JsonDataBinding.startsWith(rs[r].selectorText, 'nav.' + HtmlEditorMenuBar.limnorMenuStylesName())) {
						var pos = rs[r].selectorText.indexOf(' ');
						if (pos > 0) {
							var s = rs[r].selectorText.substr(4, pos - 4);
							b = false;
							for (k = 0; k < menuIds.length; k++) {
								if (menuIds[k] == s) {
									b = true;
									break;
								}
							}
							if (!b) {
								menuIds.push(s);
							}
						}
					}
				}
				var removedNames = client.getRemovedClassNames();
				if (removedNames) {
					for (r = 0; r < removedNames.length; r++) {
						b = false;
						for (k = 0; k < menuIds.length; k++) {
							if (menuIds[k] == removedNames[r]) {
								b = true;
								break;
							}
						}
						if (!b) {
							menuIds.push(removedNames[r]);
						}
					}
				}
				for (k = 0; k < menuIds.length; k++) {
					var navs = document.getElementsByClassName(menuIds[k]);
					if (navs && navs.length > 0) {
					}
					else {
						var s1 = 'nav.' + menuIds[k];
						var s2 = 'nav.' + menuIds[k] + ' ';
						r = 0;
						while (r < rs.length) {
							if (rs[r].selectorText == s1 || JsonDataBinding.startsWith(rs[r].selectorText, s2)) {
								if (dyStyleNode.deleteRule) {
									dyStyleNode.deleteRule(r);
								}
								else {
									dyStyleNode.removeRule(r);
								}
							}
							else {
								r++;
							}
						}
						var mid = menuIds[k];
						client.addToRemoveCssBySelector('nav.' + mid + ' > ul');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul a[isparent]::after');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul::after');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul li');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul li:hover');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul li:hover a');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul li:hover > ul');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul li a');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul ul');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul ul li');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul ul li a');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul ul li a:hover');
						client.addToRemoveCssBySelector('nav.' + mid + ' ul ul ul');
					}
				}
			}
		}
	}
	//===end of onBeforeSave==========
};