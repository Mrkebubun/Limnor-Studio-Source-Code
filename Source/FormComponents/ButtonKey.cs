/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using VPL;
using System.Drawing.Design;

namespace FormComponents
{
	[ToolboxBitmapAttribute(typeof(ButtonKey), "Resources.buttonkey.bmp")]
	[Description("Button as a key for an onscreen keyboard. When it is clicked/touched the contents of KeysToSend property are sent to the window having input focus.")]
	public class ButtonKey : Button, INoFocus
	{
		private string _keysToSend;
		private bool vkSet = false;
		private KeyPairList _keyPairList;
		private KeyMapsConverter _keyConverter;
		public ButtonKey()
		{
			_keyConverter = new KeyMapsConverter();
		}
		[Browsable(false)]
		public bool IsRunMode
		{
			get
			{
				return !(Site != null && Site.DesignMode);
			}
		}

		[Browsable(false)]
		public KeyPairList Keys
		{
			get
			{
				if (_keyPairList == null)
				{
					_keyPairList = new KeyPairList();
					_keyPairList.thisKey = _keysToSend;
				}
				return _keyPairList;
			}
		}
		[Editor(typeof(TypeEditKeyPairs), typeof(UITypeEditor))]
		[Description("A list of keys to watch for. If the previous key sent is among the key list, a coressponding keys are sent to replace the previous key. This function is enabled only when SendKeys property is not empty.")]
		public string KeyMaps
		{
			get
			{
				return (string)_keyConverter.ConvertTo(Keys, typeof(string));
			}
			set
			{
				_keyPairList = (KeyPairList)_keyConverter.ConvertFromInvariantString(value);
				if (_keyPairList != null)
				{
					_keyPairList.thisKey = _keysToSend;
				}
			}
		}
		[Description("This property indicates the string of keystrokes to be sent to the active application when this button is clicked by the user. To specify characters that aren't displayed when you press a key, such as ENTER or TAB, and keys that represent actions rather than characters, or specify key combinations, see the codes and descriptions in the Limnor Studio tutorial, and the sample applications.")]
		public string KeysToSend
		{
			get
			{
				return _keysToSend;
			}
			set
			{
				_keysToSend = value;
				if (_keyPairList != null)
				{
					_keyPairList.thisKey = _keysToSend;
				}
				IForm frm = this.FindForm() as IForm;
				if (frm != null)
				{
					if (!frm.IsDisposed)
					{
						if (!string.IsNullOrEmpty(_keysToSend))
						{
							if (IsRunMode)
								frm.NotAllowFocus();
							vkSet = true;
						}
						else
						{
							vkSet = false;
							INoFocus bt;
							bool bNoMore = true;
							for (int i = 0; i < frm.Controls.Count; i++)
							{
								bt = frm.Controls[i] as INoFocus;
								if (bt != null)
								{
									if (bt.NoFocus())
									{
										bNoMore = false;
										break;
									}
								}
							}
							if (bNoMore)
							{
								frm.AllowFocus();
							}
						}
					}
				}
			}
		}
		protected void OnClick0(EventArgs e)
		{
			if (IsRunMode)
			{
				if (!string.IsNullOrEmpty(_keysToSend))
				{
					KeyPair kp = null;
					if (_keyPairList != null && _keyPairList.Count > 0)
					{
						kp = _keyPairList.GetCombinedKey();//get key-pair based on keyboard recording
						if (kp != null)
						{
							int len = kp.PreviousKey.Length; //key-companiation
							//remove the key-combination and then send the result keys
							string s = "{BS " + len.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}" + kp.Value;
							System.Windows.Forms.SendKeys.Send(s);
							//replace used key-companiation with the result keys
							KeyPairList.ReplaceKey(len, kp.Value);
							KeyPairList.PushKeyBuffer(_keysToSend);
						}
					}
					if (kp == null)
					{
						System.Windows.Forms.SendKeys.Send(_keysToSend);
						//record keyboard
						if (string.Compare(_keysToSend, "{BACKSPACE}", StringComparison.OrdinalIgnoreCase) == 0
							|| string.Compare(_keysToSend, "{BS}", StringComparison.OrdinalIgnoreCase) == 0
							|| string.Compare(_keysToSend, "{BKSP}", StringComparison.OrdinalIgnoreCase) == 0)
						{
							KeyPairList.PopKeyBuffer();
						}
						else if (string.Compare(_keysToSend, "{DELETE}", StringComparison.OrdinalIgnoreCase) == 0
							|| string.Compare(_keysToSend, "{DEL}", StringComparison.OrdinalIgnoreCase) == 0)
						{

						}
						else if (_keysToSend.StartsWith("{", StringComparison.OrdinalIgnoreCase))
						{

						}
						else
						{
							KeyPairList.PushKey(_keysToSend);
						}
					}
				}
			}
			base.OnClick(e);
		}
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (vkSet)
			{
				System.Windows.Forms.Form frm = this.FindForm();
				if (frm != null)
				{
					if (!frm.IsDisposed)
					{
						vkSet = false;
					}
				}
			}
			base.OnHandleDestroyed(e);
		}
		protected override void WndProc(ref Message m)
		{
			if (vkSet && IsRunMode)
			{
				if (m.Msg == 0x203)
				{
					m.Result = (IntPtr)0;
					this.OnDoubleClick(EventArgs.Empty);
					return;
				}
				if (m.Msg == 0x201)
				{
					m.Result = (IntPtr)0;
					System.Drawing.Point p = this.PointToClient(Control.MousePosition);
					this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0));
					OnClick0(EventArgs.Empty);
					return;
				}
				if (m.Msg == 0x202)
				{
					m.Result = (IntPtr)0;
					System.Drawing.Point p = this.PointToClient(Control.MousePosition);
					base.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0));
					return;
				}
				if (m.Msg == 0x21)
				{
					m.Result = (IntPtr)3;
					return;
				}
				if (m.Msg == 0x6)
				{
					m.Result = (IntPtr)0;
					return;
				}
			}
			base.WndProc(ref m);
		}
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			IForm f = this.Parent as IForm;
			if (f != null)
			{
				if (!string.IsNullOrEmpty(_keysToSend))
				{
					f.NotAllowFocus();
					vkSet = true;
				}
			}
		}
		#region INoFocus Members

		public bool NoFocus()
		{
			return !string.IsNullOrEmpty(_keysToSend);
		}

		#endregion
	}
}
