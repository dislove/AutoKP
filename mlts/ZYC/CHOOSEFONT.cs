using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class CHOOSEFONT
	{
		public int lStructSize = Marshal.SizeOf(typeof(CHOOSEFONT));
		public IntPtr hwndOwner;
		public IntPtr hDC;
		public IntPtr lpLogFont;
		public int iPointSize;
		public int Flags;
		public int rgbColors;
		public IntPtr lCustData = IntPtr.Zero;
		public WndProc lpfnHook;
		public string lpTemplateName;
		public IntPtr hInstance;
		public string lpszStyle;
		public short nFontType;
		public short ___MISSING_ALIGNMENT__;
		public int nSizeMin;
		public int nSizeMax;
	}
}
