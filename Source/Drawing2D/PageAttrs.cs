/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Limnor.Drawing2D
{
	public enum EnumPageUnit { Pixel = 0, Inch, Centimeter }
	//all A-serial paper has 1:sroot(2) width:length ratio
	//public enum EnumPageSize{Custom=0,A04,A02,A0,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,B0,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,C0,C1,C2,C3,C4,C5,C6,C7,C8,C9,C10}

	/// <summary>
	/// Summary description for PageAttrs.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PageAttrs : ICloneable
	{
		#region fields and constructors
		public const double intoc = 2.54;
		private EnumPageUnit _unit = EnumPageUnit.Pixel;
		private PaperKind _pageSize = PaperKind.A4;
		private double nPageWidth = 210;
		private double nPageHeight = 297;
		private bool _showPrintSize = false;
		private float _dpiX = 96;
		private float _dpiY = 96;
		public PageAttrs()
		{
			//
			//
		}
		#endregion
		#region Methods
		public System.Drawing.Printing.PaperSize CreatePageSize()
		{
			double width, height;
			//mm/10 -> c , c / intoc -> inc , inc * 100 -> hundredths of an inch
			width = 10.0 * (((double)PageWidthInMM) / intoc);
			height = 10.0 * (((double)PageHeightInMM) / intoc);
			//
			// Summary:
			//     Initializes a new instance of the System.Drawing.Printing.PaperSize class.
			//
			// Parameters:
			//   name:
			//     The name of the paper.
			//
			//   width:
			//     The width of the paper, in hundredths of an inch.
			//
			//   height:
			//     The height of the paper, in hundredths of an inch.
			return new System.Drawing.Printing.PaperSize(PageSize.ToString(), (int)width, (int)height);
		}
		public void SetDPI(float dpiX, float dpiY)
		{
			_dpiX = dpiX;
			_dpiY = dpiY;
		}
		public void GetPageSize(float dpiX, float dpiY, out double width, out double height)
		{
			SetDPI(dpiX, dpiY);
			width = 0;
			height = 0;
			switch (PageUnit)
			{
				case EnumPageUnit.Centimeter:
					width = ((double)PageWidthInMM) / 10.0;
					height = ((double)PageHeightInMM) / 10.0;
					break;
				case EnumPageUnit.Inch:
					width = (((double)PageWidthInMM) / 10.0) / intoc;
					height = (((double)PageHeightInMM) / 10.0) / intoc;
					break;
				case EnumPageUnit.Pixel:
					width = ((((double)PageWidthInMM) / 10.0) / intoc) * ((double)dpiX);
					height = ((((double)PageHeightInMM) / 10.0) / intoc) * ((double)dpiY);
					break;
			}
		}
		public void SetPageSize(float dpiX, float dpiY, double width, double height)
		{
			SetDPI(dpiX, dpiY);
			switch (PageUnit)
			{
				case EnumPageUnit.Centimeter:
					nPageWidth = (int)(width * 10.0);
					nPageHeight = (int)(height * 10.0);
					break;
				case EnumPageUnit.Inch:
					nPageWidth = (int)(width * 10.0 * intoc);
					nPageHeight = (int)(height * 10.0 * intoc);
					break;
				case EnumPageUnit.Pixel:
					nPageWidth = (int)(width * 10.0 * intoc * ((double)dpiX));
					nPageHeight = (int)(height * 10.0 * intoc * ((double)dpiY));
					break;
			}
		}
		public void SetPageSizeMM(double width, double height)
		{
			nPageWidth = width;
			nPageHeight = height;
		}
		public int PageWidthPixel(float dpiX)
		{
			return (int)(((PageWidthInMM / 10.0) / intoc) * ((double)dpiX));
		}
		public int PageHeightPixel(float dpiY)
		{
			return (int)(((PageHeightInMM / 10.0) / intoc) * ((double)dpiY));
		}
		/// <summary>
		/// get paper size in inch
		/// </summary>
		/// <param name="pk"></param>
		/// <returns></returns>
		public static void GetPaperSize(PaperKind pk, out double w, out double h)
		{
			switch (pk)
			{
				case PaperKind.Letter://  Letter paper (8.5 in. by 11 in.).  
					w = 8.5; h = 11.0; break;
				case PaperKind.Legal://  Legal paper (8.5 in. by 14 in.).  
					w = 8.5; h = 14; break;
				case PaperKind.A4://  A4 paper (210 mm by 297 mm).  
					w = 21 / intoc; h = 29.7 / intoc; break;
				case PaperKind.CSheet://  C paper (17 in. by 22 in.).  
					w = 17; h = 22; break;
				case PaperKind.DSheet://  D paper (22 in. by 34 in.).  
					w = 22; h = 34; break;
				case PaperKind.ESheet://  E paper (34 in. by 44 in.).  
					w = 34; h = 44; break;
				case PaperKind.LetterSmall:// Letter small paper (8.5 in. by 11 in.).
					w = 8.5; h = 11; break;
				case PaperKind.Tabloid:// Tabloid paper (11 in. by 17 in.).  
					w = 11; h = 17; break;
				case PaperKind.Ledger:// Ledger paper (17 in. by 11 in.).  
					w = 17; h = 11; break;
				case PaperKind.Statement:// Statement paper (5.5 in. by 8.5 in.).  
					w = 5.5; h = 8.5; break;
				case PaperKind.Executive://  Executive paper (7.25 in. by 10.5 in.).  
					w = 7.25; h = 10.5; break;
				case PaperKind.A3://  A3 paper (297 mm by 420 mm).  
					w = 29.7 / intoc; h = 42 / intoc; break;
				case PaperKind.A4Small://  A4 small paper (210 mm by 297 mm).  
					w = 21 / intoc; h = 29.7 / intoc; break;
				case PaperKind.A5://  A5 paper (148 mm by 210 mm).  
					w = 14.8 / intoc; h = 21 / intoc; break;
				case PaperKind.B4://  B4 paper (250 mm by 353 mm).  
					w = 25 / intoc; h = 35.3 / intoc; break;
				case PaperKind.B5://  B5 paper (176 mm by 250 mm).  
					w = 17.6 / intoc; h = 25 / intoc; break;
				case PaperKind.Folio://  Folio paper (8.5 in. by 13 in.).  
					w = 8.5; h = 13; break;
				case PaperKind.Quarto://  Quarto paper (215 mm by 275 mm). 
					w = 21.5 / intoc; h = 27.5 / intoc; break;
				case PaperKind.Standard10x14://  Standard paper (10 in. by 14 in.). 
					w = 10; h = 14; break;
				case PaperKind.Standard11x17:// Standard paper (11 in. by 17 in.).  
					w = 11; h = 17; break;
				case PaperKind.Note://  Note paper (8.5 in. by 11 in.).  
					w = 8.5; h = 11; break;
				case PaperKind.Number9Envelope://  #9 envelope (3.875 in. by 8.875 in.).  
					w = 3.875; h = 8.875; break;
				case PaperKind.Number10Envelope://  #10 envelope (4.125 in. by 9.5 in.).  
					w = 4.125; h = 9.5; break;
				case PaperKind.Number11Envelope://  #11 envelope (4.5 in. by 10.375 in.).  
					w = 4.5; h = 10.375; break;
				case PaperKind.Number12Envelope://  #12 envelope (4.75 in. by 11 in.).  
					w = 4.75; h = 11; break;
				case PaperKind.Number14Envelope://  #14 envelope (5 in. by 11.5 in.).  
					w = 5; h = 11.5; break;
				case PaperKind.DLEnvelope://  DL envelope (110 mm by 220 mm).  
					w = 11 / intoc; h = 22 / intoc; break;
				case PaperKind.C5Envelope:// C5 envelope (162 mm by 229 mm). 
					w = 16.2 / intoc; h = 22.9 / intoc; break;
				case PaperKind.C3Envelope:// C3 envelope (324 mm by 458 mm).  
					w = 32.4 / intoc; h = 45.8 / intoc; break;
				case PaperKind.C4Envelope:// C4 envelope (229 mm by 324 mm).  
					w = 22.9 / intoc; h = 32.4 / intoc; break;
				case PaperKind.C6Envelope://C6 envelope (114 mm by 162 mm).  
					w = 11.4 / intoc; h = 16.2 / intoc; break;
				case PaperKind.C65Envelope:// C65 envelope (114 mm by 229 mm).  
					w = 11.4 / intoc; h = 22.9 / intoc; break;
				case PaperKind.B4Envelope:// B4 envelope (250 mm by 353 mm).  
					w = 25 / intoc; h = 35.3 / intoc; break;
				case PaperKind.B5Envelope:// B5 envelope (176 mm by 250 mm).  
					w = 17.6 / intoc; h = 25 / intoc; break;
				case PaperKind.B6Envelope:// B6 envelope (176 mm by 125 mm).  
					w = 17.6 / intoc; h = 12.5 / intoc; break;
				case PaperKind.ItalyEnvelope://  Italy envelope (110 mm by 230 mm).  
					w = 11 / intoc; h = 23 / intoc; break;
				case PaperKind.MonarchEnvelope://  Monarch envelope (3.875 in. by 7.5 in.).  
					w = 3.875; h = 7.5; break;
				case PaperKind.PersonalEnvelope:// 6 3/4 envelope (3.625 in. by 6.5 in.).  
					w = 3.625; h = 6.5; break;
				case PaperKind.USStandardFanfold:// US standard fanfold (14.875 in. by 11 in.).  
					w = 14.875; h = 11; break;
				case PaperKind.GermanStandardFanfold://  German standard fanfold (8.5 in. by 12 in.).  
					w = 8.5; h = 12; break;
				case PaperKind.GermanLegalFanfold:// German legal fanfold (8.5 in. by 13 in.).  
					w = 8.5; h = 13; break;
				case PaperKind.IsoB4:// ISO B4 (250 mm by 353 mm).  
					w = 25 / intoc; h = 35.3 / intoc; break;
				case PaperKind.JapanesePostcard://  Japanese postcard (100 mm by 148 mm).  
					w = 10 / intoc; h = 14.8 / intoc; break;
				case PaperKind.Standard9x11:// Standard paper (9 in. by 11 in.).  
					w = 9; h = 11; break;
				case PaperKind.Standard10x11:// Standard paper (10 in. by 11 in.).  
					w = 10; h = 11; break;
				case PaperKind.Standard15x11:// Standard paper (15 in. by 11 in.).  
					w = 15; h = 11; break;
				case PaperKind.InviteEnvelope://  Invitation envelope (220 mm by 220 mm).  
					w = 22 / intoc; h = 22 / intoc; break;
				case PaperKind.LetterExtra://  Letter extra paper (9.275 in. by 12 in.). This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.  
					w = 9.275; h = 12; break;
				case PaperKind.LegalExtra://  Legal extra paper (9.275 in. by 15 in.). This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.  
					w = 9.275; h = 15; break;
				case PaperKind.TabloidExtra:// Tabloid extra paper (11.69 in. by 18 in.). This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.  
					w = 11.69; h = 18; break;
				case PaperKind.A4Extra:// A4 extra paper (236 mm by 322 mm). This value is specific to the PostScript driver and is used only by Linotronic printers to help save paper.  
					w = 23.6 / intoc; h = 32.2 / intoc; break;
				case PaperKind.LetterTransverse:// Letter transverse paper (8.275 in. by 11 in.).  
					w = 8.275; h = 11; break;
				case PaperKind.A4Transverse://  A4 transverse paper (210 mm by 297 mm).  
					w = 21 / intoc; h = 29.7 / intoc; break;
				case PaperKind.LetterExtraTransverse://  Letter extra transverse paper (9.275 in. by 12 in.). 
					w = 9.275; h = 12; break;
				case PaperKind.APlus:// SuperA/SuperA/A4 paper (227 mm by 356 mm).  
					w = 22.7 / intoc; h = 35.6 / intoc; break;
				case PaperKind.BPlus:// SuperB/SuperB/A3 paper (305 mm by 487 mm).  
					w = 30.5 / intoc; h = 48.7 / intoc; break;
				case PaperKind.LetterPlus:// Letter plus paper (8.5 in. by 12.69 in.). 
					w = 8.5; h = 12.69; break;
				case PaperKind.A4Plus:// A4 plus paper (210 mm by 330 mm).  
					w = 21 / intoc; h = 33 / intoc; break;
				case PaperKind.A5Transverse:// A5 transverse paper (148 mm by 210 mm).  
					w = 14.8 / intoc; h = 21 / intoc; break;
				case PaperKind.B5Transverse:// JIS B5 transverse paper (182 mm by 257 mm).  
					w = 18.2 / intoc; h = 25.7 / intoc; break;
				case PaperKind.A3Extra:// A3 extra paper (322 mm by 445 mm).  
					w = 32.2 / intoc; h = 44.5 / intoc; break;
				case PaperKind.A5Extra://  A5 extra paper (174 mm by 235 mm).  
					w = 17.4 / intoc; h = 23.5 / intoc; break;
				case PaperKind.B5Extra:// ISO B5 extra paper (201 mm by 276 mm).  
					w = 20.1 / intoc; h = 27.6 / intoc; break;
				case PaperKind.A2:// A2 paper (420 mm by 594 mm).
					w = 42 / intoc; h = 59.4 / intoc; break;
				case PaperKind.A3Transverse:// A3 transverse paper (297 mm by 420 mm).  
					w = 29.7 / intoc; h = 42 / intoc; break;
				case PaperKind.A3ExtraTransverse://  A3 extra transverse paper (322 mm by 445 mm).  
					w = 32.2 / intoc; h = 44.5 / intoc; break;
				case PaperKind.JapaneseDoublePostcard://Japanese double postcard (200 mm by 148 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 20 / intoc; h = 14.8 / intoc; break;
				case PaperKind.A6:// A6 paper (105 mm by 148 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 10.5 / intoc; h = 14.8 / intoc; break;
				case PaperKind.JapaneseEnvelopeKakuNumber2://  Japanese Kaku #2 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 33.2 / intoc; h = 24 / intoc; break;
				case PaperKind.JapaneseEnvelopeKakuNumber3:// Japanese Kaku #3 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 27.7 / intoc; h = 21.6 / intoc; break;
				case PaperKind.JapaneseEnvelopeChouNumber3:// Japanese Chou #3 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 23.5 / intoc; h = 12 / intoc; break;
				case PaperKind.JapaneseEnvelopeChouNumber4:// Japanese Chou #4 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 20.5 / intoc; h = 9 / intoc; break;
				case PaperKind.LetterRotated:// Letter rotated paper (11 in. by 8.5 in.).
					w = 11; h = 8.5; break;
				case PaperKind.A3Rotated:// A3 rotated paper (420 mm by 297 mm).  
					w = 42 / intoc; h = 29.7 / intoc; break;
				case PaperKind.A4Rotated://  A4 rotated paper (297 mm by 210 mm). Requires Windows 98, Windows NT 4.0, or later. 
					w = 29.7 / intoc; h = 21 / intoc; break;
				case PaperKind.A5Rotated:// A5 rotated paper (210 mm by 148 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 21 / intoc; h = 14.8 / intoc; break;
				case PaperKind.B4JisRotated:// JIS B4 rotated paper (364 mm by 257 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 36.4 / intoc; h = 25.7 / intoc; break;
				case PaperKind.B5JisRotated:// JIS B5 rotated paper (257 mm by 182 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 25.7 / intoc; h = 18.2 / intoc; break;
				case PaperKind.JapanesePostcardRotated://  Japanese rotated postcard (148 mm by 100 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 14.8 / intoc; h = 10 / intoc; break;
				case PaperKind.JapaneseDoublePostcardRotated:// Japanese rotated double postcard (148 mm by 200 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 14.8 / intoc; h = 20 / intoc; break;
				case PaperKind.A6Rotated:// A6 rotated paper (148 mm by 105 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 14.8 / intoc; h = 10.5 / intoc; break;
				case PaperKind.JapaneseEnvelopeKakuNumber2Rotated://  Japanese rotated Kaku #2 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 24 / intoc; h = 33.2 / intoc; break;
				case PaperKind.JapaneseEnvelopeKakuNumber3Rotated:// Japanese rotated Kaku #3 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 21.6 / intoc; h = 27.7 / intoc; break;
				case PaperKind.JapaneseEnvelopeChouNumber3Rotated://  Japanese rotated Chou #3 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 12 / intoc; h = 23.5 / intoc; break;
				case PaperKind.JapaneseEnvelopeChouNumber4Rotated://  Japanese rotated Chou #4 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 9 / intoc; h = 20.5 / intoc; break;
				case PaperKind.B6Jis:// JIS B6 paper (128 mm by 182 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 12.8 / intoc; h = 18.2 / intoc; break;
				case PaperKind.B6JisRotated:// JIS B6 rotated paper (182 mm by 128 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 18.2 / intoc; h = 12.8 / intoc; break;
				case PaperKind.Standard12x11:// Standard paper (12 in. by 11 in.). Requires Windows 98, Windows NT 4.0, or later.  
					w = 12; h = 11; break;
				case PaperKind.JapaneseEnvelopeYouNumber4://  Japanese You #4 envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 23.5 / intoc; h = 10.5 / intoc; break;
				case PaperKind.JapaneseEnvelopeYouNumber4Rotated:// Japanese You #4 rotated envelope. Requires Windows 98, Windows NT 4.0, or later.  
					w = 10.5 / intoc; h = 23.5 / intoc; break;
				case PaperKind.Prc16K:// People's Republic of China 16K paper (146 mm by 215 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 14.6 / intoc; h = 21.5 / intoc; break;
				case PaperKind.Prc32K:// People's Republic of China 32K paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 9.7 / intoc; h = 15.1 / intoc; break;
				case PaperKind.Prc32KBig:// People's Republic of China 32K big paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 9.7 / intoc; h = 15.1 / intoc; break;
				case PaperKind.PrcEnvelopeNumber1:// People's Republic of China #1 envelope (102 mm by 165 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 10.2 / intoc; h = 16.5 / intoc; break;
				case PaperKind.PrcEnvelopeNumber2://  People's Republic of China #2 envelope (102 mm by 176 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 10.2 / intoc; h = 17.6 / intoc; break;
				case PaperKind.PrcEnvelopeNumber3://  People's Republic of China #3 envelope (125 mm by 176 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 12.5 / intoc; h = 17.6 / intoc; break;
				case PaperKind.PrcEnvelopeNumber4://  People's Republic of China #4 envelope (110 mm by 208 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 11 / intoc; h = 20.8 / intoc; break;
				case PaperKind.PrcEnvelopeNumber5://  People's Republic of China #5 envelope (110 mm by 220 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 11 / intoc; h = 22 / intoc; break;
				case PaperKind.PrcEnvelopeNumber6://  People's Republic of China #6 envelope (120 mm by 230 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 12 / intoc; h = 23 / intoc; break;
				case PaperKind.PrcEnvelopeNumber7://  People's Republic of China #7 envelope (160 mm by 230 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 16 / intoc; h = 23 / intoc; break;
				case PaperKind.PrcEnvelopeNumber8:// People's Republic of China #8 envelope (120 mm by 309 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 12 / intoc; h = 30.9 / intoc; break;
				case PaperKind.PrcEnvelopeNumber9://  People's Republic of China #9 envelope (229 mm by 324 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 22.9 / intoc; h = 32.4 / intoc; break;
				case PaperKind.PrcEnvelopeNumber10:// People's Republic of China #10 envelope (324 mm by 458 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 32.4 / intoc; h = 45.8 / intoc; break;
				case PaperKind.Prc16KRotated:// People's Republic of China 16K rotated paper (146 mm by 215 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 14.6 / intoc; h = 21.5 / intoc; break;
				case PaperKind.Prc32KRotated:// People's Republic of China 32K rotated paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 9.7 / intoc; h = 15.1 / intoc; break;
				case PaperKind.Prc32KBigRotated://  People's Republic of China 32K big rotated paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 9.7 / intoc; h = 15.1 / intoc; break;
				case PaperKind.PrcEnvelopeNumber1Rotated:// People's Republic of China #1 rotated envelope (165 mm by 102 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 16.5 / intoc; h = 10.2 / intoc; break;
				case PaperKind.PrcEnvelopeNumber2Rotated://  People's Republic of China #2 rotated envelope (176 mm by 102 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 17.6 / intoc; h = 10.2 / intoc; break;
				case PaperKind.PrcEnvelopeNumber3Rotated:// People's Republic of China #3 rotated envelope (176 mm by 125 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 17.6 / intoc; h = 12.5 / intoc; break;
				case PaperKind.PrcEnvelopeNumber4Rotated:// People's Republic of China #4 rotated envelope (208 mm by 110 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 20.8 / intoc; h = 11 / intoc; break;
				case PaperKind.PrcEnvelopeNumber5Rotated:// People's Republic of China Envelope #5 rotated envelope (220 mm by 110 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 22 / intoc; h = 11 / intoc; break;
				case PaperKind.PrcEnvelopeNumber6Rotated:// People's Republic of China #6 rotated envelope (230 mm by 120 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 23 / intoc; h = 12 / intoc; break;
				case PaperKind.PrcEnvelopeNumber7Rotated:// People's Republic of China #7 rotated envelope (230 mm by 160 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 23 / intoc; h = 16 / intoc; break;
				case PaperKind.PrcEnvelopeNumber8Rotated:// People's Republic of China #8 rotated envelope (309 mm by 120 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 30.9 / intoc; h = 12 / intoc; break;
				case PaperKind.PrcEnvelopeNumber9Rotated:// People's Republic of China #9 rotated envelope (324 mm by 229 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 32.4 / intoc; h = 22.9 / intoc; break;
				case PaperKind.PrcEnvelopeNumber10Rotated://People's Republic of China #10 rotated envelope (458 mm by 324 mm). Requires Windows 98, Windows NT 4.0, or later.  
					w = 45.8 / intoc; h = 32.4 / intoc; break;
				default:
					w = 21 / intoc; h = 29.7 / intoc; break;
			}
		}

		public void SetDocumentPageAttributes(PrintDocument doc)
		{
			if (PageSize == PaperKind.Custom)
			{
				doc.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)(PageWidthInInc * 100.0), (int)(PageHeightInInc * 100.0));
			}
			else
			{
				if (doc.PrinterSettings.PaperSizes.Count > 0)
				{
					foreach (PaperSize ps in doc.PrinterSettings.PaperSizes)
					{
						if (ps.Kind == PageSize)
						{
							doc.DefaultPageSettings.PaperSize = ps;
							break;
						}
					}
				}
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}: {1},{2} ({3})", PageSize, PageWidth.ToString("#.##"), PageHeight.ToString("#.##"), PageUnit);
		}
		#endregion
		#region Properties
		[Description("Gets and sets a Boolean indicating whether to show print page edges on the drawing board in design time.")]
		public bool ShowPrintPageEdges
		{
			get
			{
				return _showPrintSize;
			}
			set
			{
				_showPrintSize = value;
			}
		}
		[DefaultValue(96)]
		[Description("Gets and sets a number indicating horizontal printer resolution, in DPI (Dots Per Inch)")]
		public float PrinterDPI_X
		{
			get
			{
				return _dpiX;
			}
			set
			{
				if (value > 0)
					_dpiX = value;
			}
		}
		[DefaultValue(96)]
		[Description("Gets and sets a number indicating Vertical printer resolution, in DPI (Dots Per Inch)")]
		public float PrinterDPI_Y
		{
			get
			{
				return _dpiY;
			}
			set
			{
				if (value > 0)
					_dpiY = value;
			}
		}
		[DefaultValue(EnumPageUnit.Pixel)]
		[Description("Indicate the unit for PageWidth and PageHeight properties")]
		public EnumPageUnit PageUnit
		{
			get
			{
				return _unit;
			}
			set
			{
				_unit = value;
			}
		}
		[DefaultValue(PaperKind.A4)]
		[Description("Indicate the size of the page")]
		public PaperKind PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}
		[Browsable(false)]
		public double PageHeightInMM
		{
			get
			{
				if (_pageSize == PaperKind.Custom)
				{
					return nPageHeight;
				}
				else
				{
					double w, h;
					GetPaperSize(_pageSize, out w, out h);
					return 10.0 * h * intoc;
				}
			}
		}
		[Browsable(false)]
		public double PageWidthInMM
		{
			get
			{
				if (PageSize == PaperKind.Custom)
				{
					return nPageWidth;
				}
				else
				{
					double w, h;
					GetPaperSize(_pageSize, out w, out h);
					return 10.0 * w * intoc;
				}
			}
		}
		[Browsable(false)]
		public double PageHeightInInc
		{
			get
			{
				if (_pageSize == PaperKind.Custom)
				{
					return (((double)nPageHeight) / 10.0) / intoc;
				}
				else
				{
					double w, h;
					GetPaperSize(_pageSize, out w, out h);
					return h;
				}
			}
		}
		[Browsable(false)]
		public double PageWidthInInc
		{
			get
			{
				if (_pageSize == PaperKind.Custom)
				{
					return (((double)nPageWidth) / 10.0) / intoc;
				}
				else
				{
					double w, h;
					GetPaperSize(_pageSize, out w, out h);
					return w;
				}
			}
		}
		[Description("Gets page size in pixels")]
		public Size PageSizeInPixels
		{
			get
			{
				double nIncW = PageWidthInInc;
				double nIncH = PageHeightInInc;
				return new Size((int)(nIncW * _dpiX), (int)(nIncH * _dpiY));
			}
		}
		/// <summary>
		/// The width of the page in unit specified by PageUnit property
		/// </summary>
		[DefaultValue(210)]
		[XmlIgnore]
		[Description("The width of the page in unit specified by PageUnit property")]
		public double PageWidth
		{
			get
			{
				double nInc = PageWidthInInc;
				switch (_unit)
				{
					case EnumPageUnit.Inch:
						return nInc;
					case EnumPageUnit.Centimeter:
						return nInc * intoc;
					case EnumPageUnit.Pixel:
						return nInc * _dpiX;
				}
				return nInc;
			}
			set
			{
				switch (_unit)
				{
					case EnumPageUnit.Centimeter:
						nPageWidth = value * 10.0;
						break;
					case EnumPageUnit.Inch:
						nPageWidth = value * intoc * 10.0;
						break;
					case EnumPageUnit.Pixel:
						nPageWidth = (value / _dpiX) * intoc * 10.0;
						break;
				}
			}
		}

		/// <summary>
		/// The height of the page in unit specified by PageUnit property
		/// </summary>
		//[ReadOnly(true)]
		[DefaultValue(297)]
		[XmlIgnore]
		[Description("The height of the page in unit specified by PageUnit property")]
		public double PageHeight
		{
			get
			{
				double nInc = PageHeightInInc;
				switch (_unit)
				{
					case EnumPageUnit.Inch:
						return nInc;
					case EnumPageUnit.Centimeter:
						return nInc * intoc;
					case EnumPageUnit.Pixel:
						return nInc * _dpiY;
				}
				return nInc;
			}
			set
			{
				switch (_unit)
				{
					case EnumPageUnit.Centimeter:
						nPageHeight = value * 10.0;
						break;
					case EnumPageUnit.Inch:
						nPageHeight = value * intoc * 10.0;
						break;
					case EnumPageUnit.Pixel:
						nPageHeight = (value / _dpiY) * intoc * 10.0;
						break;
				}
			}
		}
		/// <summary>
		/// serialized in Millisecond
		/// </summary>
		[DefaultValue(210)]
		[Browsable(false)]
		public double Width
		{
			get
			{
				return nPageWidth;
			}
			set
			{
				nPageWidth = value;
			}
		}
		/// <summary>
		/// serialized in Millisecond
		/// </summary>
		[DefaultValue(297)]
		[Browsable(false)]
		public double Height
		{
			get
			{
				return nPageHeight;
			}
			set
			{
				nPageHeight = value;
			}
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			PageAttrs obj = new PageAttrs();
			obj.PageUnit = PageUnit;
			obj.PageSize = PageSize;
			obj.nPageWidth = nPageWidth;
			obj.nPageHeight = nPageHeight;
			obj._dpiX = _dpiX;
			obj._dpiY = _dpiY;
			obj.ShowPrintPageEdges = _showPrintSize;
			return obj;
		}

		#endregion
		#region Operators
		private static bool compare(PageAttrs p1, PageAttrs p2)
		{
			if ((object)p1 == null)
			{
				return ((object)p2 == null);
			}
			if ((object)p2 == null)
			{
				return ((object)p1 == null);
			}
			if (p1._pageSize == PaperKind.Custom)
			{
				if (p2._pageSize == PaperKind.Custom)
				{
					return (p1.nPageHeight == p2.nPageHeight) && (p1.nPageWidth == p2.nPageWidth);
				}
			}
			else
			{
				if (p1._pageSize == p2._pageSize && p1._unit == p2._unit)
				{
					return true;
				}
			}

			return false;
		}
		public static bool operator ==(PageAttrs p1, PageAttrs p2)
		{
			return compare(p1, p2);
		}
		public static bool operator !=(PageAttrs p1, PageAttrs p2)
		{
			return !compare(p1, p2);
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is PageAttrs)
			{
				return compare(this, obj as PageAttrs);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		#endregion
	}
}
