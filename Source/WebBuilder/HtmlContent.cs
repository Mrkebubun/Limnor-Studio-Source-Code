/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mshtml;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Drawing.Design;
using XmlUtility;
using System.Collections;
using System.Globalization;
using System.IO;
using VPL;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlContent), "Resources.html.bmp")]
	[Description("This is a part of HTML contents on a web page.")]
	public partial class HtmlContent : UserControl, ISearchableBrowser, IWebClientControl, ICustomTypeDescriptor, IDataBindingNameMapHolder, IDeserized, INewObjectInit, IInnerHtmlEdit, IScrollableWebControl//, IBeforeXmlSerialize
	{
		#region fields and constructors
		private IHTMLDocument2 doc;
		private bool updatingFontName = false;
		private bool updatingFontSize = false;
		private bool setup = false;

		public delegate void TickDelegate();

		public class EnterKeyEventArgs : EventArgs
		{
			private bool _cancel = false;
			public bool Cancel
			{
				get { return _cancel; }
				set { _cancel = value; }
			}
		}

		public event TickDelegate Tick;

		public event WebBrowserNavigatedEventHandler Navigated;

		public event EventHandler<EnterKeyEventArgs> EnterKeyEvent;

		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private EditContents _htmlEdit;
		private Dictionary<string, string> _htmlParts;
		private bool _unloading;
		private string _htmlBody;
		const string Pre_FILE = "file:///";
		public HtmlContent()
		{
			InitializeComponent();
			SetupEvents();
			SetupTimer();
			SetupBrowser();
			SetupFontComboBox();
			SetupFontSizeComboBox();
			_resourceFiles = new List<WebResourceFile>();
			_htmlEdit = new EditContents(this);
			toolStrip1.Visible = false;
			webBrowser1.GotFocus += new EventHandler(showToolbarToolStripMenuItem_Click);

			webBrowser1.DocumentTitleChanged += new EventHandler(webBrowser1_DocumentTitleChanged);
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			Overflow = EnumOverflow.visible;
		}

		void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
		{
			if (docReady())
			{
				DialogHtmlContents.SetIECompatible(webBrowser1);
				if (!string.IsNullOrEmpty(_htmlBody))
				{
					webBrowser1.Document.Body.InnerHtml = _htmlBody;
					_htmlBody = null;
				}
			}
		}
		private bool HasLoaded
		{
			get
			{
				return (webBrowser1.Document != null && webBrowser1.Document.Body != null);
			}
		}

		static HtmlContent()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("Font");
			_propertyNames.Add("BodyHtml");
			_propertyNames.Add("BorderStyle");
			_propertyNames.Add("HtmlContents");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("innerHTML");
			_propertyNames.Add("DataBindings");
		}
		#endregion

		#region private methods
		/// <summary>
		/// Setup navigation and focus event handlers.
		/// </summary>
		private void SetupEvents()
		{
			webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
			webBrowser1.GotFocus += new EventHandler(webBrowser1_GotFocus);
		}

		/// <summary>
		/// When this control receives focus, it transfers focus to the 
		/// document body.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void webBrowser1_GotFocus(object sender, EventArgs e)
		{
			SuperFocus();
		}

		/// <summary>
		/// This is called when the initial html/body framework is set up, 
		/// or when document.DocumentText is set.  At this point, the 
		/// document is editable.
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">navigation args</param>
		private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			webBrowser1.Document.ContextMenuShowing +=
				new HtmlElementEventHandler(Document_ContextMenuShowing);
			SetBackgroundColor(BackColor);
			if (Navigated != null)
			{
				Navigated(this, e);
			}
		}

		/// <summary>
		/// Setup timer with 200ms interval
		/// </summary>
		private void SetupTimer()
		{
			timer.Interval = 200;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		/// <summary>
		/// Add document body, turn on design mode on the whole document, 
		/// and overred the context menu
		/// </summary>
		private void SetupBrowser()
		{
		}

		/// <summary>
		/// Set the focus on the document body.  
		/// </summary>
		private void SuperFocus()
		{
			if (webBrowser1.Document != null &&
				webBrowser1.Document.Body != null)
				webBrowser1.Document.Body.Focus();
		}
		/// <summary>
		/// Set the background color of the body by setting it's CSS style
		/// </summary>
		/// <param name="value">the color to use for the background</param>
		private void SetBackgroundColor(Color value)
		{
			if (webBrowser1.Document != null &&
				webBrowser1.Document.Body != null)
				webBrowser1.Document.Body.Style =
					string.Format("background-color: {0}", value.Name);
		}

		/// <summary>
		/// Called when the editor context menu should be displayed.
		/// The return value of the event is set to false to disable the 
		/// default context menu.  A custom context menu (contextMenuStrip1) is 
		/// shown instead.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">HtmlElementEventArgs</param>
		private void Document_ContextMenuShowing(object sender, HtmlElementEventArgs e)
		{
			e.ReturnValue = false;
			cutToolStripMenuItem1.Enabled = CanCut();
			copyToolStripMenuItem2.Enabled = CanCopy();
			pasteToolStripMenuItem3.Enabled = CanPaste();
			deleteToolStripMenuItem.Enabled = CanDelete();
			showToolbarToolStripMenuItem.Enabled = !toolStrip1.Visible;
			hideToolbarToolStripMenuItem.Enabled = toolStrip1.Visible;
			contextMenuStrip1.Show(this, e.ClientMousePosition);
		}

		/// <summary>
		/// Populate the font size combobox.
		/// Add text changed and key press handlers to handle input and update 
		/// the editor selection font size.
		/// </summary>
		private void SetupFontSizeComboBox()
		{
			for (int x = 1; x <= 7; x++)
			{
				fontSizeComboBox.Items.Add(x.ToString());
			}
			fontSizeComboBox.TextChanged += new EventHandler(fontSizeComboBox_TextChanged);
			fontSizeComboBox.KeyPress += new KeyPressEventHandler(fontSizeComboBox_KeyPress);
		}

		/// <summary>
		/// Called when a key is pressed on the font size combo box.
		/// The font size in the boxy box is set to the key press value.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">KeyPressEventArgs</param>
		private void fontSizeComboBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Char.IsNumber(e.KeyChar))
			{
				e.Handled = true;
				if (e.KeyChar <= '7' && e.KeyChar > '0')
					fontSizeComboBox.Text = e.KeyChar.ToString();
			}
			else if (!Char.IsControl(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		/// <summary>
		/// Set editor's current selection to the value of the font size combo box.
		/// Ignore if the timer is currently updating the font size to synchronize 
		/// the font size combo box with the editor's current selection.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void fontSizeComboBox_TextChanged(object sender, EventArgs e)
		{
			if (updatingFontSize) return;
			switch (fontSizeComboBox.Text.Trim())
			{
				case "1":
					FontSize = FontSize.One;
					break;
				case "2":
					FontSize = FontSize.Two;
					break;
				case "3":
					FontSize = FontSize.Three;
					break;
				case "4":
					FontSize = FontSize.Four;
					break;
				case "5":
					FontSize = FontSize.Five;
					break;
				case "6":
					FontSize = FontSize.Six;
					break;
				case "7":
					FontSize = FontSize.Seven;
					break;
				default:
					FontSize = FontSize.Seven;
					break;
			}
		}

		/// <summary>
		/// Populate the font combo box and autocomplete handlers.
		/// Add a text changed handler to the font combo box to handle new font selections.
		/// </summary>
		private void SetupFontComboBox()
		{
			AutoCompleteStringCollection ac = new AutoCompleteStringCollection();
			foreach (FontFamily fam in FontFamily.Families)
			{
				fontComboBox.Items.Add(fam.Name);
				ac.Add(fam.Name);
			}
			fontComboBox.Leave += new EventHandler(fontComboBox_TextChanged);
			fontComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
			fontComboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
			fontComboBox.AutoCompleteCustomSource = ac;
		}

		/// <summary>
		/// Called when the font combo box has changed.
		/// Ignores the event when the timer is updating the font combo Box 
		/// to synchronize the editor selection with the font combo box.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void fontComboBox_TextChanged(object sender, EventArgs e)
		{
			if (updatingFontName) return;
			FontFamily ff;
			try
			{
				ff = new FontFamily(fontComboBox.Text);
			}
			catch (Exception)
			{
				updatingFontName = true;
				fontComboBox.Text = FontName.GetName(0);
				updatingFontName = false;
				return;
			}
			FontName = ff;
		}
		private bool docReady()
		{
			if (doc != null)
				return true;
			if (webBrowser1.Document != null)
			{
				doc = webBrowser1.Document.DomDocument as IHTMLDocument2;
			}
			return (doc != null);
		}
		/// <summary>
		/// Called when the timer fires to synchronize the format buttons 
		/// with the text editor current selection.
		/// SetupKeyListener if necessary.
		/// Set bold, italic, underline and link buttons as based on editor state.
		/// Synchronize the font combo box and the font size combo box.
		/// Finally, fire the Tick event to allow external components to synchronize 
		/// their state with the editor.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void timer_Tick(object sender, EventArgs e)
		{
			// don't process until browser is in ready state.
			if (ReadyState != ReadyState.Complete)
				return;
			if (_unloading)
			{
				return;
			}
			SetupKeyListener();
			boldButton.Checked = IsBold();
			italicButton.Checked = IsItalic();
			underlineButton.Checked = IsUnderline();
			orderedListButton.Checked = IsOrderedList();
			unorderedListButton.Checked = IsUnorderedList();
			justifyLeftButton.Checked = IsJustifyLeft();
			justifyCenterButton.Checked = IsJustifyCenter();
			justifyRightButton.Checked = IsJustifyRight();
			justifyFullButton.Checked = IsJustifyFull();

			linkButton.Enabled = SelectionType == SelectionType.Text;

			UpdateFontComboBox();
			UpdateFontSizeComboBox();

			if (Tick != null)
				Tick();
		}

		/// <summary>
		/// Update the font size combo box.
		/// Sets a flag to indicate that the combo box is updating, and should 
		/// not update the editor's selection.
		/// </summary>
		private void UpdateFontSizeComboBox()
		{
			if (!fontSizeComboBox.Focused)
			{
				int foo;
				switch (FontSize)
				{
					case FontSize.One:
						foo = 1;
						break;
					case FontSize.Two:
						foo = 2;
						break;
					case FontSize.Three:
						foo = 3;
						break;
					case FontSize.Four:
						foo = 4;
						break;
					case FontSize.Five:
						foo = 5;
						break;
					case FontSize.Six:
						foo = 6;
						break;
					case FontSize.Seven:
						foo = 7;
						break;
					case FontSize.NA:
						foo = 0;
						break;
					default:
						foo = 7;
						break;
				}
				string fontsize = Convert.ToString(foo);
				if (fontsize != fontSizeComboBox.Text)
				{
					updatingFontSize = true;
					fontSizeComboBox.Text = fontsize;
					updatingFontSize = false;
				}
			}
		}

		/// <summary>
		/// Update the font combo box.
		/// Sets a flag to indicate that the combo box is updating, and should 
		/// not update the editor's selection.
		/// </summary>
		private void UpdateFontComboBox()
		{
			if (!fontComboBox.Focused)
			{
				FontFamily fam = FontName;
				if (fam != null)
				{
					string fontname = fam.Name;
					if (fontname != fontComboBox.Text)
					{
						updatingFontName = true;
						fontComboBox.Text = fontname;
						updatingFontName = false;
					}
				}
			}
		}

		/// <summary>
		/// Set up a key listener on the body once.
		/// The key listener checks for specific key strokes and takes 
		/// special action in certain cases.
		/// </summary>
		private void SetupKeyListener()
		{
			if (!setup)
			{
				webBrowser1.Document.Body.KeyDown += new HtmlElementEventHandler(Body_KeyDown);
				setup = true;
			}
		}

		/// <summary>
		/// If the user hits the enter key, and event will fire (EnterKeyEvent), 
		/// and the consumers of this event can cancel the projecessing of the 
		/// enter key by cancelling the event.
		/// This is useful if your application would like to take some action 
		/// when the enter key is pressed, such as a submission to a web service.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">HtmlElementEventArgs</param>
		private void Body_KeyDown(object sender, HtmlElementEventArgs e)
		{
			if (e.KeyPressedCode == 13 && !e.ShiftKeyPressed)
			{
				// handle enter code cancellation
				bool cancel = false;
				if (EnterKeyEvent != null)
				{
					EnterKeyEventArgs args = new EnterKeyEventArgs();
					EnterKeyEvent(this, args);
					cancel = args.Cancel;
				}
				e.ReturnValue = !cancel;
			}
		}
		/// <summary>
		/// Paste the clipboard text into the current selection.
		/// This is a placeholder for future functionality.
		/// </summary>
		private void SuperPaste()
		{
			if (docReady())
			{
				if (Clipboard.ContainsText())
				{
					IHTMLTxtRange range =
						doc.selection.createRange() as IHTMLTxtRange;
					range.pasteHTML(Clipboard.GetText(TextDataFormat.Text));
					range.collapse(false);
					range.select();
				}
			}
		}
		/// <summary>
		/// Convert the custom integer (B G R) format to a color object.
		/// </summary>
		/// <param name="clrs">the custorm color as a string</param>
		/// <returns>the color</returns>
		private static Color ConvertToColor(string clrs)
		{
			int red, green, blue;
			// sometimes clrs is HEX organized as (RED)(GREEN)(BLUE)
			if (clrs.StartsWith("#"))
			{
				int clrn = Convert.ToInt32(clrs.Substring(1), 16);
				red = (clrn >> 16) & 255;
				green = (clrn >> 8) & 255;
				blue = clrn & 255;
			}
			else // otherwise clrs is DECIMAL organized as (BlUE)(GREEN)(RED)
			{
				int clrn = Convert.ToInt32(clrs);
				red = clrn & 255;
				green = (clrn >> 8) & 255;
				blue = (clrn >> 16) & 255;
			}
			Color incolor = Color.FromArgb(red, green, blue);
			return incolor;
		}

		/// <summary>
		/// Called when the cut tool strip button on the editor context menu 
		/// is clicked.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void cutToolStripButton_Click(object sender, EventArgs e)
		{
			Cut();
		}

		/// <summary>
		/// Called when the paste tool strip button on the editor context menu 
		/// is clicked.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void pasteToolStripButton_Click(object sender, EventArgs e)
		{
			Paste();
		}

		/// <summary>
		/// Called when the copy tool strip button on the editor context menu 
		/// is clicked.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void copyToolStripButton_Click(object sender, EventArgs e)
		{
			Copy();
		}

		/// <summary>
		/// Called when the bold button on the tool strip is pressed.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void boldButton_Click(object sender, EventArgs e)
		{
			Bold();
		}

		/// <summary>
		/// Called when the italic button on the tool strip is pressed.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void italicButton_Click(object sender, EventArgs e)
		{
			Italic();
		}

		/// <summary>
		/// Called when the underline button on the tool strip is pressed.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void underlineButton_Click(object sender, EventArgs e)
		{
			Underline();
		}

		/// <summary>
		/// Called when the foreground color button on the tool strip is pressed.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void colorButton_Click(object sender, EventArgs e)
		{
			SelectForeColor();
		}

		/// <summary>
		/// Called when the background color button on the tool strip is pressed.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void backColorButton_Click(object sender, EventArgs e)
		{
			SelectBackColor();
		}

		/// <summary>
		/// Show the interactive Color dialog.
		/// </summary>
		/// <param name="color">the input and output color</param>
		/// <returns>true if dialog accepted, false if dialog cancelled</returns>
		private bool ShowColorDialog(ref Color color)
		{
			bool selected;
			using (ColorDialog dlg = new ColorDialog())
			{
				dlg.SolidColorOnly = true;
				dlg.AllowFullOpen = false;
				dlg.AnyColor = false;
				dlg.FullOpen = false;
				dlg.CustomColors = null;
				dlg.Color = color;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					selected = true;
					color = dlg.Color;
				}
				else
				{
					selected = false;
				}
			}
			return selected;
		}

		/// <summary>
		/// Called when the link button on the toolstrip is pressed.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void linkButton_Click(object sender, EventArgs e)
		{
			SelectLink();
		}

		/// <summary>
		/// Called when the image button on the toolstrip is clicked.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void imageButton_Click(object sender, EventArgs e)
		{
			InsertImage();
		}

		/// <summary>
		/// Called when the outdent button on the toolstrip is clicked.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void outdentButton_Click(object sender, EventArgs e)
		{
			Outdent();
		}

		/// <summary>
		/// Called when the indent button on the toolstrip is clicked.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void indentButton_Click(object sender, EventArgs e)
		{
			Indent();
		}

		/// <summary>
		/// Called when the cut button is clicked on the editor context menu.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Cut();
		}

		/// <summary>
		/// Called when the copy button is clicked on the editor context menu.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void copyToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			Copy();
		}

		/// <summary>
		/// Called when the paste button is clicked on the editor context menu.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void pasteToolStripMenuItem3_Click(object sender, EventArgs e)
		{
			Paste();
		}

		/// <summary>
		/// Called when the delete button is clicked on the editor context menu.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Delete();
		}

		/// <summary>
		/// Event handler for the ordered list toolbar button
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void orderedListButton_Click(object sender, EventArgs e)
		{
			OrderedList();
		}

		/// <summary>
		/// Event handler for the unordered list toolbar button
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void unorderedListButton_Click(object sender, EventArgs e)
		{
			UnorderedList();
		}

		/// <summary>
		/// Event handler for the left justify toolbar button.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void justifyLeftButton_Click(object sender, EventArgs e)
		{
			JustifyLeft();
		}

		/// <summary>
		/// Event handler for the center justify toolbar button.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void justifyCenterButton_Click(object sender, EventArgs e)
		{
			JustifyCenter();
		}

		/// <summary>
		/// Event handler for the right justify toolbar button.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void justifyRightButton_Click(object sender, EventArgs e)
		{
			JustifyRight();
		}

		/// <summary>
		/// Event handler for the full justify toolbar button.
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">EventArgs</param>
		private void justifyFullButton_Click(object sender, EventArgs e)
		{
			JustifyFull();
		}

		private void insertHRbutton_Click(object sender, EventArgs e)
		{
			InsertBreak();
		}

		private void insertBreakButton_Click(object sender, EventArgs e)
		{
			InsertLineBreak();
		}

		private void showToolbarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStrip1.Visible = true;
		}

		private void hideToolbarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStrip1.Visible = false;
		}

		private void hideToolbarButton_Click(object sender, EventArgs e)
		{
			toolStrip1.Visible = false;
		}
		private void checkXmlContents(HtmlElement e)
		{
			foreach (HtmlElement he in e.Children)
			{
				IHTMLElement ihe = he.DomElement as IHTMLElement;
				IHTMLDOMNode hn = ihe as IHTMLDOMNode;
				if (hn != null)
				{
					IHTMLAttributeCollection ac = hn.attributes as IHTMLAttributeCollection;
					if (ac != null)
					{
						IEnumerator ie = ac.GetEnumerator();
						while (ie.MoveNext())
						{
							IHTMLDOMAttribute2 a2 = ie.Current as IHTMLDOMAttribute2;
							if (a2 != null)
							{
								if (!string.IsNullOrEmpty(a2.value))
								{
									if (a2.value.StartsWith(Pre_FILE, StringComparison.OrdinalIgnoreCase))
									{
										string file = a2.value.Substring(Pre_FILE.Length);
										if (File.Exists(file))
										{
											bool b;
											_resourceFiles.Add(new WebResourceFile(file, WebResourceFile.WEBFOLDER_Images, out b));
											a2.value = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(file));
										}
									}
								}
							}
						}
					}
				}
				if (he.Children.Count > 0)
				{
					checkXmlContents(he);
				}
			}
		}


		#endregion

		#region Methods

		[Description("Append an image element to the contents. ")]
		[WebClientMember]
		public HtmlImage AppendImage(string imageFilepath, string id)
		{
			return null;
		}
		[Description("Append HTML text to the contents. ")]
		[WebClientMember]
		public void AppendHtml(string html)
		{
		}
		[Description("Use the new HTML as the contents, removing existing contents. ")]
		[WebClientMember]
		public void SetHtml(string html)
		{
			webBrowser1.DocumentText = html;
		}
		[Description("Append text to the contents. ")]
		[WebClientMember]
		public void AppendText(string text)
		{
		}
		[Description("Append a new line of text to the contents. ")]
		[WebClientMember]
		public void AppendLine(string text)
		{
		}
		/// <summary>
		/// Clear the contents of the document, leaving the body intact.
		/// </summary>
		[Description("Set the contents to empty. ")]
		[WebClientMember]
		public void Clear()
		{
			if (webBrowser1.Document.Body != null)
				webBrowser1.Document.Body.InnerHtml = "";
		}

		public bool SetInlineEdit()
		{
			if (docReady())
			{
				webBrowser1.Document.Body.SetAttribute("contentEditable", "true");
				return true;
			}
			return false;
		}
		/// <summary>
		/// Determine the status of the Undo command in the document editor.
		/// </summary>
		/// <returns>whether or not an undo operation is currently valid</returns>
		public bool CanUndo()
		{
			if (docReady())
				return doc.queryCommandEnabled("Undo");
			return false;
		}

		/// <summary>
		/// Determine the status of the Redo command in the document editor.
		/// </summary>
		/// <returns>whether or not a redo operation is currently valid</returns>
		public bool CanRedo()
		{
			if (docReady())
				return doc.queryCommandEnabled("Redo");
			return false;
		}

		/// <summary>
		/// Determine the status of the Cut command in the document editor.
		/// </summary>
		/// <returns>whether or not a cut operation is currently valid</returns>
		public bool CanCut()
		{
			if (docReady())
				return doc.queryCommandEnabled("Cut");
			return false;
		}

		/// <summary>
		/// Determine the status of the Copy command in the document editor.
		/// </summary>
		/// <returns>whether or not a copy operation is currently valid</returns>
		public bool CanCopy()
		{
			if (docReady())
				return doc.queryCommandEnabled("Copy");
			return false;
		}

		/// <summary>
		/// Determine the status of the Paste command in the document editor.
		/// </summary>
		/// <returns>whether or not a copy operation is currently valid</returns>
		public bool CanPaste()
		{
			if (docReady())
				return doc.queryCommandEnabled("Paste");
			return false;
		}

		/// <summary>
		/// Determine the status of the Delete command in the document editor.
		/// </summary>
		/// <returns>whether or not a copy operation is currently valid</returns>
		public bool CanDelete()
		{
			if (docReady())
				return doc.queryCommandEnabled("Delete");
			return false;
		}

		/// <summary>
		/// Determine whether the current block is left justified.
		/// </summary>
		/// <returns>true if left justified, otherwise false</returns>
		public bool IsJustifyLeft()
		{
			if (docReady())
				return doc.queryCommandState("JustifyLeft");
			return false;
		}

		/// <summary>
		/// Determine whether the current block is right justified.
		/// </summary>
		/// <returns>true if right justified, otherwise false</returns>
		public bool IsJustifyRight()
		{
			if (docReady())
				return doc.queryCommandState("JustifyRight");
			return false;
		}

		/// <summary>
		/// Determine whether the current block is center justified.
		/// </summary>
		/// <returns>true if center justified, false otherwise</returns>
		public bool IsJustifyCenter()
		{
			if (docReady())
				return doc.queryCommandState("JustifyCenter");
			return false;
		}

		/// <summary>
		/// Determine whether the current block is full justified.
		/// </summary>
		/// <returns>true if full justified, false otherwise</returns>
		public bool IsJustifyFull()
		{
			if (docReady())
				return doc.queryCommandState("JustifyFull");
			return false;
		}

		/// <summary>
		/// Determine whether the current selection is in Bold mode.
		/// </summary>
		/// <returns>whether or not the current selection is Bold</returns>
		public bool IsBold()
		{
			if (docReady())
				return doc.queryCommandState("Bold");
			return false;
		}

		/// <summary>
		/// Determine whether the current selection is in Italic mode.
		/// </summary>
		/// <returns>whether or not the current selection is Italicized</returns>
		public bool IsItalic()
		{
			if (docReady())
				return doc.queryCommandState("Italic");
			return false;
		}

		/// <summary>
		/// Determine whether the current selection is in Underline mode.
		/// </summary>
		/// <returns>whether or not the current selection is Underlined</returns>
		public bool IsUnderline()
		{
			if (docReady())
				return doc.queryCommandState("Underline");
			return false;
		}

		/// <summary>
		/// Determine whether the current paragraph is an ordered list.
		/// </summary>
		/// <returns>true if current paragraph is ordered, false otherwise</returns>
		public bool IsOrderedList()
		{
			if (docReady())
				return doc.queryCommandState("InsertOrderedList");
			return false;
		}

		/// <summary>
		/// Determine whether the current paragraph is an unordered list.
		/// </summary>
		/// <returns>true if current paragraph is ordered, false otherwise</returns>
		public bool IsUnorderedList()
		{
			if (docReady())
				return doc.queryCommandState("InsertUnorderedList");
			return false;
		}


		/// <summary>
		/// Embed a break at the current selection.
		/// This is a placeholder for future functionality.
		/// </summary>
		public void EmbedBr()
		{
			if (docReady())
			{
				IHTMLTxtRange range =
					doc.selection.createRange() as IHTMLTxtRange;
				range.pasteHTML("<br/>");
				range.collapse(false);
				range.select();
			}
		}



		/// <summary>
		/// Print the current document
		/// </summary>
		public void Print()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Print", true, null);
		}

		/// <summary>
		/// Insert a paragraph break
		/// </summary>
		public void InsertParagraph()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("InsertParagraph", false, null);
		}

		/// <summary>
		/// Insert a horizontal rule
		/// </summary>
		public void InsertBreak()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("InsertHorizontalRule", false, null);
		}

		public void InsertLineBreak()
		{
			if (docReady())
			{
				mshtml.IHTMLTxtRange txt = (mshtml.IHTMLTxtRange)doc.selection.createRange();
				txt.pasteHTML("<br>");
			}
		}

		/// <summary>
		/// Select all text in the document.
		/// </summary>
		public void SelectAll()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("SelectAll", false, null);
		}

		/// <summary>
		/// Undo the last operation
		/// </summary>
		public void Undo()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Undo", false, null);
		}

		/// <summary>
		/// Redo based on the last Undo
		/// </summary>
		public void Redo()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Redo", false, null);
		}

		/// <summary>
		/// Cut the current selection and place it in the clipboard.
		/// </summary>
		public void Cut()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Cut", false, null);
		}

		/// <summary>
		/// Paste the contents of the clipboard into the current selection.
		/// </summary>
		public void Paste()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Paste", false, null);
		}

		/// <summary>
		/// Copy the current selection into the clipboard.
		/// </summary>
		public void Copy()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Copy", false, null);
		}

		/// <summary>
		/// Toggle the ordered list property for the current paragraph.
		/// </summary>
		public void OrderedList()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("InsertOrderedList", false, null);
		}

		/// <summary>
		/// Toggle the unordered list property for the current paragraph.
		/// </summary>
		public void UnorderedList()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("InsertUnorderedList", false, null);
		}

		/// <summary>
		/// Toggle the left justify property for the currnet block.
		/// </summary>
		public void JustifyLeft()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("JustifyLeft", false, null);
		}

		/// <summary>
		/// Toggle the right justify property for the current block.
		/// </summary>
		public void JustifyRight()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("JustifyRight", false, null);
		}

		/// <summary>
		/// Toggle the center justify property for the current block.
		/// </summary>
		public void JustifyCenter()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("JustifyCenter", false, null);
		}

		/// <summary>
		/// Toggle the full justify property for the current block.
		/// </summary>
		public void JustifyFull()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("JustifyFull", false, null);
		}

		/// <summary>
		/// Toggle bold formatting on the current selection.
		/// </summary>
		public void Bold()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Bold", false, null);
		}

		/// <summary>
		/// Toggle italic formatting on the current selection.
		/// </summary>
		public void Italic()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Italic", false, null);
		}

		/// <summary>
		/// Toggle underline formatting on the current selection.
		/// </summary>
		public void Underline()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Underline", false, null);
		}

		/// <summary>
		/// Delete the current selection.
		/// </summary>
		public void Delete()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Delete", false, null);
		}

		/// <summary>
		/// Insert an imange.
		/// </summary>
		public void InsertImage()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("InsertImage", true, null);
		}

		/// <summary>
		/// Indent the current paragraph.
		/// </summary>
		public void Indent()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Indent", false, null);
		}

		/// <summary>
		/// Outdent the current paragraph.
		/// </summary>
		public void Outdent()
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("Outdent", false, null);
		}

		/// <summary>
		/// Insert a link at the current selection.
		/// </summary>
		/// <param name="url">The link url</param>
		public void InsertLink(string url)
		{
			if (docReady())
				webBrowser1.Document.ExecCommand("CreateLink", false, url);
		}

		/// <summary>
		/// Initiate the foreground (text) color dialog for the current selection.
		/// </summary>
		public void SelectForeColor()
		{
			Color color = EditorForeColor;
			if (ShowColorDialog(ref color))
				EditorForeColor = color;
		}

		/// <summary>
		/// Initiate the background color dialog for the current selection.
		/// </summary>
		public void SelectBackColor()
		{
			Color color = EditorBackColor;
			if (ShowColorDialog(ref color))
				EditorBackColor = color;
		}


		/// <summary>
		/// Show a custom insert link dialog, and create the link.
		/// </summary>
		public void SelectLink()
		{
			using (LinkDialog dlg = new LinkDialog())
			{
				dlg.ShowDialog(this.ParentForm);
				if (!dlg.Accepted) return;
				string link = dlg.URI;
				if (link == null || link.Length == 0)
				{
					MessageBox.Show(this.ParentForm, "Invalid URL");
					return;
				}
				InsertLink(dlg.URL);
			}
		}

		/// <summary>
		/// Search the document from the current selection, and reset the 
		/// the selection to the text found, if successful.
		/// </summary>
		/// <param name="text">the text for which to search</param>
		/// <param name="forward">true for forward search, false for backward</param>
		/// <param name="matchWholeWord">true to match whole word, false otherwise</param>
		/// <param name="matchCase">true to match case, false otherwise</param>
		/// <returns></returns>
		public bool Search(string text, bool forward, bool matchWholeWord, bool matchCase)
		{
			bool success = false;
			if (webBrowser1.Document != null)
			{
				IHTMLDocument2 doc =
					webBrowser1.Document.DomDocument as IHTMLDocument2;
				if (doc != null)
				{
					IHTMLBodyElement body = doc.body as IHTMLBodyElement;
					if (body != null)
					{
						IHTMLTxtRange range;
						if (doc.selection != null)
						{
							range = doc.selection.createRange() as IHTMLTxtRange;
							IHTMLTxtRange dup = range.duplicate();
							dup.collapse(true);
							// if selection is degenerate, then search whole body
							if (range.isEqual(dup))
							{
								range = body.createTextRange();
							}
							else
							{
								if (forward)
									range.moveStart("character", 1);
								else
									range.moveEnd("character", -1);
							}
						}
						else
							range = body.createTextRange();
						int flags = 0;
						if (matchWholeWord) flags += 2;
						if (matchCase) flags += 4;
						success =
							range.findText(text, forward ? 999999 : -999999, flags);
						if (success)
						{
							range.select();
							range.scrollIntoView(!forward);
						}
					}
				}
			}
			return success;
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			toolStrip1.Visible = true;
		}
		protected override void OnHandleDestroyed(EventArgs e)
		{
			_unloading = true;
			base.OnHandleDestroyed(e);
		}
		#endregion

		#region Properties
		private SizeType _widthSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[WebClientMember]
		[Description("Gets and sets size type for width. Check out its effects by showing the page in a browser.")]
		public SizeType WidthType
		{
			get
			{
				return _widthSizeType;
			}
			set
			{
				_widthSizeType = value;
			}
		}
		private uint _width = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the width of this layout as a percentage of parent width. This value is used when WidthType is Percent.")]
		public uint WidthInPercent
		{
			get
			{
				return _width;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_width = value;
				}
			}
		}

		[Browsable(false)]
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the width of this layout as a percentage of parent width. This value is used when WidthType is Percent.")]
		public uint LayoutWidthPercent
		{
			get
			{
				//return _width;
				return WidthInPercent;
			}
			set
			{
				WidthInPercent = value;
			}
		}

		private SizeType _heightSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[WebClientMember]
		[Description("Gets and sets size type for height. Check out its effects by showing the page in a browser.")]
		public SizeType HeightType
		{
			get
			{
				return _heightSizeType;
			}
			set
			{
				_heightSizeType = value;
			}
		}
		private uint _height = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the height of this layout as a percentage of parent height. It is used when HeightType is Percent.")]
		public uint HeightInPercent
		{
			get
			{
				return _height;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_height = value;
				}
			}
		}
		[Browsable(false)]
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the height of this layout as a percentage of parent height. It is used when HeightType is Percent.")]
		public uint LayoutHeightPercent
		{
			get
			{
				return HeightInPercent;
			}
			set
			{
				HeightInPercent = value;
			}
		}
		//
		[Category("Layout")]
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
		[Description("Gets and sets anchor style. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public AnchorStyles PositionAnchor
		{
			get
			{
				return this.Anchor;
			}
			set
			{
				this.Anchor = value;
			}
		}
		[Category("Layout")]
		[DefaultValue(ContentAlignment.TopLeft)]
		[Description("Gets and sets position alignment. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public ContentAlignment PositionAlignment
		{
			get;
			set;
		}
		[Browsable(false)]
		[Editor(typeof(TypeEditorHtmlContents), typeof(UITypeEditor))]
		[Description("Html contents displayed in this part of the web page")]
		public EditContents HtmlContents
		{
			get
			{
				return _htmlEdit;
			}
		}
		[Editor(typeof(TypeEditorHtmlContents), typeof(UITypeEditor))]
		[DesigntimeReadOnly]
		[Bindable(true)]
		[Description("Gets and sets HTML contents at runtime.")]
		[WebClientMember]
		public string innerHTML
		{
			get
			{
				return BodyHtml;
			}
			set
			{
			}
		}
		public bool ToolbarVisible
		{
			get
			{
				return toolStrip1.Visible;
			}
			set
			{
				toolStrip1.Visible = value;
			}
		}

		[Browsable(false)]
		public bool WebContentLoaded
		{
			get
			{
				return HasLoaded;
			}
		}
		/// <summary>
		/// Get/Set the background color of the editor.
		/// Note that if this is called before the document is rendered and 
		/// complete, the navigated event handler will set the body's 
		/// background color based on the state of BackColor.
		/// </summary>
		[Browsable(true)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				if (ReadyState == ReadyState.Complete)
				{
					SetBackgroundColor(value);
				}
			}
		}
		/// <summary>
		/// Get the web browser component's document
		/// </summary>
		[Browsable(true)]
		public HtmlDocument Document
		{
			get { return webBrowser1.Document; }
		}

		/// <summary>
		/// Document text should be used to load/save the entire document, 
		/// including html and body start/end tags.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public string DocumentText
		{
			get
			{
				return webBrowser1.DocumentText;
			}
			set
			{
				webBrowser1.DocumentText = value;
			}
		}

		/// <summary>
		/// Get the html document title from document.
		/// </summary>
		[Browsable(false)]
		public string DocumentTitle
		{
			get
			{
				return webBrowser1.DocumentTitle;
			}
		}

		/// <summary>
		/// Get/Set the contents of the document Body, in html.
		/// </summary>
		[Browsable(false)]
		public string BodyHtml
		{
			get
			{
				if (webBrowser1.Document != null &&
					webBrowser1.Document.Body != null)
				{
					return webBrowser1.Document.Body.InnerHtml;
				}
				else
					return _htmlBody;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (value.StartsWith("<HTML", StringComparison.OrdinalIgnoreCase) || value.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
					{
						webBrowser1.DocumentText = value;
					}
					else
					{
						if (webBrowser1.Document == null || webBrowser1.Document.Body == null)
						{
							_htmlBody = value;
						}
						else
						{
							DialogHtmlContents.SetIECompatible(webBrowser1);
							webBrowser1.Document.Body.InnerHtml = value;
						}
					}
				}
				else
				{
					if (webBrowser1.Document != null && webBrowser1.Document.Body != null)
					{
						webBrowser1.Document.Body.InnerHtml = string.Empty;
					}
				}
			}
		}

		/// <summary>
		/// Get/Set the documents body as text.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public string BodyText
		{
			get
			{
				if (webBrowser1.Document != null &&
					webBrowser1.Document.Body != null)
				{
					return webBrowser1.Document.Body.InnerText;
				}
				else
					return string.Empty;
			}
			set
			{
				if (webBrowser1.Document != null && webBrowser1.Document.Body != null)
					webBrowser1.Document.Body.InnerText = value;
			}
		}



		/// <summary>
		/// Get the ready state of the internal browser component.
		/// </summary>
		[Browsable(true)]
		public ReadyState ReadyState
		{
			get
			{
				if (docReady())
				{
					switch (doc.readyState.ToLower())
					{
						case "uninitialized":
							return ReadyState.Uninitialized;
						case "loading":
							return ReadyState.Loading;
						case "loaded":
							return ReadyState.Loaded;
						case "interactive":
							return ReadyState.Interactive;
						case "complete":
							return ReadyState.Complete;
						default:
							return ReadyState.Uninitialized;
					}
				}
				return WebBuilder.ReadyState.Uninitialized;
			}
		}

		/// <summary>
		/// Get the current selection type.
		/// </summary>
		[Browsable(true)]
		public SelectionType SelectionType
		{
			get
			{
				if (docReady())
				{
					switch (doc.selection.type.ToLower())
					{
						case "text":
							return SelectionType.Text;
						case "control":
							return SelectionType.Control;
						case "none":
							return SelectionType.None;
						default:
							return SelectionType.None;
					}
				}
				return WebBuilder.SelectionType.None;
			}
		}

		/// <summary>
		/// Get/Set the current font size.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public FontSize FontSize
		{
			get
			{
				if (_unloading)
				{
					return FontSize.NA;
				}
				if (ReadyState != ReadyState.Complete)
					return FontSize.NA;
				switch (doc.queryCommandValue("FontSize").ToString())
				{
					case "1":
						return FontSize.One;
					case "2":
						return FontSize.Two;
					case "3":
						return FontSize.Three;
					case "4":
						return FontSize.Four;
					case "5":
						return FontSize.Five;
					case "6":
						return FontSize.Six;
					case "7":
						return FontSize.Seven;
					default:
						return FontSize.NA;
				}
			}
			set
			{
				if (docReady())
				{
					int sz;
					switch (value)
					{
						case FontSize.One:
							sz = 1;
							break;
						case FontSize.Two:
							sz = 2;
							break;
						case FontSize.Three:
							sz = 3;
							break;
						case FontSize.Four:
							sz = 4;
							break;
						case FontSize.Five:
							sz = 5;
							break;
						case FontSize.Six:
							sz = 6;
							break;
						case FontSize.Seven:
							sz = 7;
							break;
						default:
							sz = 7;
							break;
					}
					webBrowser1.Document.ExecCommand("FontSize", false, sz.ToString());
				}
			}
		}

		/// <summary>
		/// Get/Set the current font name.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public FontFamily FontName
		{
			get
			{
				if (_unloading)
				{
					return null;
				}
				if (ReadyState != ReadyState.Complete)
					return null;
				string name = doc.queryCommandValue("FontName") as string;
				if (name == null) return null;
				return new FontFamily(name);
			}
			set
			{
				if (docReady())
					if (value != null)
						webBrowser1.Document.ExecCommand("FontName", false, value.Name);
			}
		}

		/// <summary>
		/// Get/Set the editor's foreground (text) color for the current selection.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public Color EditorForeColor
		{
			get
			{
				if (ReadyState != ReadyState.Complete)
					return Color.Black;
				return ConvertToColor(doc.queryCommandValue("ForeColor").ToString());
			}
			set
			{
				if (docReady())
				{
					string colorstr =
						string.Format("#{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B);
					webBrowser1.Document.ExecCommand("ForeColor", false, colorstr);
				}
			}
		}

		/// <summary>
		/// Get/Set the editor's background color for the current selection.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public Color EditorBackColor
		{
			get
			{
				if (ReadyState != ReadyState.Complete)
					return Color.White;
				return ConvertToColor(doc.queryCommandValue("BackColor").ToString());
			}
			set
			{
				if (docReady())
				{
					string colorstr =
						string.Format("#{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B);
					webBrowser1.Document.ExecCommand("BackColor", false, colorstr);
				}
			}
		}
		#endregion

		#region Web events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }

		[Description("Occurs when the mouse is clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }

		[Description("Occurs when the mouse is pressed over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousedown { add { } remove { } }
		[Description("Occurs when the the mouse is released over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseup { add { } remove { } }
		[Description("Occurs when the mouse is moved onto the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseover { add { } remove { } }
		[Description("Occurs when the mouse is moved over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousemove { add { } remove { } }
		[Description("Occurs when the mouse is moved away from the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseout { add { } remove { } }
		#endregion

		#region IWebClientControl Members
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }

		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
		}
		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		private string _vaname;
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vaname = vname;
		}
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[DefaultValue(0)]
		[WebClientMember]
		public int zOrder { get; set; }

		private int _opacity = 100;
		[DefaultValue(100)]
		[Description("Gets and sets the opacity of the control. 0 is transparent. 100 is full opacity")]
		public int Opacity
		{
			get
			{
				if (_opacity < 0 || _opacity > 100)
				{
					_opacity = 100;
				}
				return _opacity;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					_opacity = value;
				}
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get { return _htmlParts; }
		}

		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_dataNode != null)
					return XmlUtil.GetNameAttribute(_dataNode);
				return _vaname;
			}
		}

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
			return name;
		}

		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return new MethodInfo[] { };
		}

		public EventInfo[] GetWebClientEvents(bool isStatic)
		{
			return new EventInfo[] { };
		}

		public PropertyDescriptorCollection GetWebClientProperties(bool isStatic)
		{
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
		}
		[Browsable(false)]
		public virtual Dictionary<string, string> DataBindNameMap
		{
			get
			{
				Dictionary<string, string> map = new Dictionary<string, string>();
				map.Add("Text", "innerHTML");
				return map;
			}
		}
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);

			_resourceFiles = new List<WebResourceFile>();
			StringBuilder sb = new StringBuilder();
			//
			if (this.BackColor != Color.White)
			{
				sb.Append("background-color:");
				sb.Append(ObjectCreationCodeGen.GetColorString(this.BackColor));
				sb.Append("; ");
			}

			//
			if (this.BorderStyle != BorderStyle.None)
			{
				sb.Append("border:1px solid black; ");
			}
			//
			sb.Append(ObjectCreationCodeGen.GetFontStyleString(this.Font));
			//
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);
			//
			if (_dataNode != null)
			{
				XmlNode pNode = _dataNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}[@name='Visible']", XmlTags.XML_PROPERTY));
				if (pNode != null)
				{
					string s = pNode.InnerText;
					if (!string.IsNullOrEmpty(s))
					{
						try
						{
							bool b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
							if (!b)
							{
								sb.Append("display:none; ");
							}
						}
						catch
						{
						}
					}
				}
			}
			XmlUtil.SetAttribute(node, "style", sb.ToString());
			//
			int nTime = 0;
			if (!HasLoaded)
			{
				OnDeserized(null);
			}
			while (!HasLoaded)
			{
				Application.DoEvents();
				nTime++;
				if (nTime > 500)
				{
					throw new Exception("HtmlContent cannot finish loading");
				}
			}
			//
			checkXmlContents(webBrowser1.Document.Body);
			//
			string htmlId = VPLUtil.GuidToString(Guid.NewGuid());
			_htmlParts = new Dictionary<string, string>();
			_htmlParts.Add(htmlId, this.BodyHtml);
			node.InnerText = htmlId;
			//
			WebPageCompilerUtility.WriteDataBindings(node, this.DataBindings, DataBindNameMap);
			XmlElement xe = (XmlElement)node;
			xe.IsEmpty = false;
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "AppendImage") == 0)
			{
				string path = parameters[0];
				string id = parameters[1];
				if (string.IsNullOrEmpty(path))
				{
					path = "''";
				}
				string img = string.Format(CultureInfo.InvariantCulture, "img{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				if (string.IsNullOrEmpty(id) || string.CompareOrdinal(id, "''") == 0)
				{
					id = string.Format(CultureInfo.InvariantCulture, "'{0}'", img);
				}
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\nvar {0} = document.createElement(\"img\"); \r\n{0}.src = {1}; \r\n{0}.id={2};\r\n",
					img, path, id));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\ndocument.getElementById('{0}').appendChild({1});\r\n",
					this.Site.Name, img));
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}={1};\r\n", returnReceiver, img));
				}
			}
			else if (string.CompareOrdinal(methodName, "AppendHtml") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\ndocument.getElementById('{0}').innerHTML = document.getElementById('{0}').innerHTML + {1};\r\n",
					this.Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "SetHtml") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\ndocument.getElementById('{0}').innerHTML = {1};\r\n",
					this.Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "AppendText") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\ndocument.getElementById('{0}').innerHTML = document.getElementById('{0}').innerHTML + {1};\r\n",
					this.Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "AppendLine") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\ndocument.getElementById('{0}').innerHTML = document.getElementById('{0}').innerHTML + '<br>' + {1};\r\n",
					this.Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "Clear") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"\r\ndocument.getElementById('{0}').innerHTML = '';\r\n",
					this.Site.Name));
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		public string ElementName
		{
			get { return "div"; }
		}
		[Browsable(false)]
		public virtual string MapJavaScriptVallue(string name, string value)
		{
			string s = WebPageCompilerUtility.MapJavaScriptVallue(name, value, _resourceFiles);
			if (s != null)
			{
				return s;
			}
			return value;
		}
		#endregion

		#region IWebClientControl Properties
		[Description("id of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string id { get { return Name; } }

		[Description("tag name of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return ElementName; } }

		[Description("Returns the viewable width of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientWidth { get { return 0; } }

		[Description("Returns the viewable height of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientHeight { get { return 0; } }

		[Description("Returns the height of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetHeight { get { return 0; } }

		[Description("Returns the width of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetWidth { get { return 0; } }

		[Description("Returns the horizontal offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetLeft { get { return 0; } }

		[Description("Returns the vertical offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetTop { get { return 0; } }

		[Description("Returns the entire height of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollHeight { get { return 0; } }

		[Description("Returns the distance between the actual left edge of an element and its left edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollLeft { get { return 0; } }

		[Description("Returns the distance between the actual top edge of an element and its top edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollTop { get { return 0; } }

		[Description("Returns the entire width of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollWidth { get { return 0; } }
		#endregion

		#region IXmlNodeHolder Members

		private XmlNode _dataNode;
		[Browsable(false)]
		public XmlNode DataXmlNode { get { return _dataNode; } set { _dataNode = value; } }

		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return WebClientValueCollection.GetWebClientProperties(this, _propertyNames, attributes);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region Static utility
		public static string HtmlToXml(string html)
		{
			if (html == null)
			{
				return string.Empty;
			}
			int n = html.IndexOf('<');
			if (n < 0)
			{
				return html;
			}
			StringBuilder sb = new StringBuilder();
			string s1 = html.Substring(0, n);
			sb.Append(s1);
			string s2 = html.Substring(n);
			n = s2.IndexOf('>');
			if (n > 0)
			{
				s1 = s2.Substring(0, n + 1);
				s2 = s2.Substring(n + 1);
				//process s1 ================================

				n = s1.IndexOf(' ');
				if (n > 0)
				{
					//tag name
					string sn = s1.Substring(0, n + 1);
					sb.Append(sn);
					s1 = s1.Substring(n + 1);
					//process attributes
					while (s1.Length > 0)
					{
						//
						n = s1.IndexOf('=');
						if (n >= 0)
						{
							sn = s1.Substring(0, n + 1);
							sb.Append(sn);
							s1 = s1.Substring(n + 1);
							//
							if (s1.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
							{
								n = 0;
								while (true)
								{
									int n2 = s1.IndexOf('"', n + 1);
									if (n2 > n)
									{
										if (s1[n2 - 1] != '\\')
										{
											n = n2;
											break;
										}
										else
										{
											n = n2;
										}
									}
									else
									{
										break;
									}
								}
								if (n > 0)
								{
									sn = s1.Substring(0, n + 1);
									sb.Append(sn);
									s1 = s1.Substring(n + 1);
								}
								else
								{
									if (s1[s1.Length - 2] == '/')
									{
										// .../>
										if (s1.Length > 2)
										{
											sn = s1.Substring(0, s1.Length - 2);
											sb.Append(sn);
										}
										sb.Append("\"");
										if (s1.Length > 2)
										{
											sn = s1.Substring(s1.Length - 2);
											sb.Append(sn);
										}
										else
										{
											sb.Append(s1);
										}
									}
									else
									{
										// ...>
										sn = s1.Substring(0, s1.Length - 1);
										sb.Append(sn);
										sb.Append("\"");
										sn = s1.Substring(s1.Length - 1);
										sb.Append(sn);
									}
									break;
								}
							}
							else if (s1.StartsWith("'", StringComparison.OrdinalIgnoreCase))
							{
								n = s1.IndexOf('\'', 1);
								if (n > 0)
								{
									sn = s1.Substring(0, n + 1);
									sb.Append(sn);
									s1 = s1.Substring(n + 1);
								}
								else
								{
									if (s1[s1.Length - 2] == '/')
									{
										// .../>
										if (s1.Length > 2)
										{
											sn = s1.Substring(0, s1.Length - 2);
											sb.Append(sn);
										}
										sb.Append("\"");
										if (s1.Length > 2)
										{
											sn = s1.Substring(s1.Length - 2);
											sb.Append(sn);
										}
										else
										{
											sb.Append(s1);
										}
									}
									else
									{
										// ...>
										sn = s1.Substring(0, s1.Length - 1);
										sb.Append(sn);
										sb.Append("\"");
										sn = s1.Substring(s1.Length - 1);
										sb.Append(sn);
									}
									break;
								}
							}
							else
							{
								//add quote, space is the delimiter
								sb.Append("\"");
								n = s1.IndexOf(' ');
								if (n == 0)
								{
									s1 = s1.Substring(1);
									sb.Append("\" ");
								}
								else if (n > 0)
								{
									sb.Append(s1.Substring(0, n));
									sb.Append("\"");
									s1 = s1.Substring(n);
								}
								else
								{
									//n < 0 -- finished
									if (s1.Length > 1)
									{
										if (s1[s1.Length - 2] == '/')
										{
											// .../>
											if (s1.Length > 2)
											{
												sn = s1.Substring(0, s1.Length - 2);
												sb.Append(sn);
											}
											sb.Append("\"");
											if (s1.Length > 2)
											{
												sn = s1.Substring(s1.Length - 2);
												sb.Append(sn);
											}
											else
											{
												sb.Append(s1);
											}
										}
										else
										{
											// ...>
											sn = s1.Substring(0, s1.Length - 1);
											sb.Append(sn);
											sb.Append("\"");
											sn = s1.Substring(s1.Length - 1);
											sb.Append(sn);
										}
									}
									else
									{
										//s1.Length == 1: ">"
										sb.Append("\"");
										sb.Append(s1);
									}
									break;
								}
							}
						}
						else
						{
							sb.Append(s1);
							break;
						}
					}//attributes
				}
				else
				{
					sb.Append(s1);
				}

				//===========================================
				//
				s2 = HtmlToXml(s2);
				sb.Append(s2);
				return sb.ToString();
			}
			else
			{
				sb.Append(s2);
			}
			return sb.ToString();
		}

		#endregion

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

		#endregion

		#region IWebClientComponent Members
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		private WebClientValueCollection _customValues;
		[WebClientMember]
		[RefreshProperties(RefreshProperties.All)]
		[EditorAttribute(typeof(TypeEditorWebClientValue), typeof(UITypeEditor))]
		[Description("A custom value is associated with an Html element. It provides a storage to hold data for the element.")]
		public WebClientValueCollection CustomValues
		{
			get
			{
				if (_customValues == null)
				{
					_customValues = new WebClientValueCollection(this);
				}
				return _customValues;
			}
		}
		[Bindable(true)]
		[WebClientMember]
		[Description("Gets and sets data associated with the element")]
		public string tag
		{
			get;
			set;
		}
		[Description("Associate a named data with the element")]
		[WebClientMember]
		public void SetOrCreateNamedValue(string name, string value)
		{

		}
		[Description("Gets a named data associated with the element")]
		[WebClientMember]
		public string GetNamedValue(string name)
		{
			return string.Empty;
		}
		[Description("Gets all child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getElementsByTagName(string tagName)
		{
			return null;
		}
		[Description("Gets all immediate child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getDirectChildElementsByTagName(string tagName)
		{
			return null;
		}
		#endregion

		#region IDeserized
		public void OnDeserized(object owner)
		{
			if (string.IsNullOrEmpty(_htmlBody))
			{
				_htmlBody = string.Empty;
			}
			BodyHtml = string.Format(CultureInfo.InvariantCulture, "<!DOCTYPE html><HTML><HEAD><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"/></HEAD><BODY>{0}</BODY></HTML>", _htmlBody);
			_htmlBody = null;
		}
		#endregion

		#region INewObjectInit
		public void OnNewInstanceCreated()
		{
			webBrowser1.DocumentText = "<!DOCTYPE html><HTML><HEAD><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"/></HEAD><BODY></BODY></HTML>";
		}
		#endregion

		#region IScrollableWebControl
		[Description("visible: The overflow is not clipped. It renders outside the element's box. This is default; hidden: The overflow is clipped, and the rest of the content will be invisible; scroll: The overflow is clipped, but a scroll-bar is added to see the rest of the content; auto: If overflow is clipped, a scroll-bar should be added to see the rest of the content;inherit: Specifies that the value of the overflow property should be inherited from the parent element")]
		[DefaultValue(EnumOverflow.visible)]
		[WebClientMember]
		public EnumOverflow Overflow
		{
			get;
			set;
		}
		#endregion
	}

	/// <summary>
	/// Enumeration of possible font sizes for the Editor component
	/// </summary>
	public enum FontSize
	{
		One,
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
		NA
	}

	public enum SelectionType
	{
		Text,
		Control,
		None
	}

	public enum ReadyState
	{
		Uninitialized,
		Loading,
		Loaded,
		Interactive,
		Complete
	}

}