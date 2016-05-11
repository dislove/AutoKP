using System;
namespace ZYC
{
	public struct MONITORINFO
	{
		public int cbSize;
		public RECT rcMonitor;
		public RECT rcWork;
		public int dwFlags;
	}
}
