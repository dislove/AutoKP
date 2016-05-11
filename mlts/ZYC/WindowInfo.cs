using System;
namespace ZYC
{
	public struct WindowInfo
	{
		public IntPtr hWnd;
		public string szWindowName;
		public string szClassName;
	}
	public struct WINDOWINFO
	{
		public int cbSize;
		public RECT rcWindow;
		public RECT rcClient;
		public int dwStyle;
		public int dwExStyle;
		public int dwWindowStatus;
		public uint cxWindowBorders;
		public uint cyWindowBorders;
		public int atomWindowType;
		public int wCreatorVersion;
		public IntPtr hWnd;
		public string szWindowName;
		public string szClassName;
		public string szExePath;
	}
}
