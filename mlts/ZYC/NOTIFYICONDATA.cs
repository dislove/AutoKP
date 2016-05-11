using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	public struct NOTIFYICONDATA
	{
		public int cbSize;
		public IntPtr hWnd;
		public uint uID;
		public uint uFlags;
		public uint uCallbackMessage;
		public IntPtr hIcon;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szTip;
		public int dwState;
		public int dwStateMask;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string szInfo;
		public uint uTimeout;
		public uint uVersion;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string szInfoTitle;
		public int dwInfoFlags;
	}
}
