using System;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct DEVMODE
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmDeviceName;
		public int dmSpecVersion;
		public int dmDriverVersion;
		public int dmSize;
		public int dmDriverExtra;
		public int dmFields;
		public short dmOrientation;
		public short dmPaperSize;
		public short dmPaperLength;
		public short dmPaperWidth;
		public short dmScale;
		public short dmCopies;
		public short dmDefaultSource;
		public short dmPrintQuality;
		public Point dmPosition;
		public int dmDisplayOrientation;
		public int dmDisplayFixedOutput;
		public short dmColor;
		public short dmDuplex;
		public short dmYResolution;
		public short dmTTOption;
		public short dmCollate;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmFormName;
		public int dmLogPixels;
		public int dmBitsPerPel;
		public int dmPelsWidth;
		public int dmPelsHeight;
		public int dmDisplayFlags;
		public int dmNup;
		public int dmDisplayFrequency;
		public int dmICMMethod;
		public int dmICMIntent;
		public int dmMediaType;
		public int dmDitherType;
		public int dmReserved1;
		public int dmReserved2;
		public int dmPanningWidth;
		public int dmPanningHeight;
	}
}
