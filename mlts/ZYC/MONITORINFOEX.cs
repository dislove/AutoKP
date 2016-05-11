using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	public struct MONITORINFOEX
	{
		public int cbSize;
		public RECT rcMonitor;
		public RECT rcWork;
		public int dwFlags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string szDevice;
	}
}
