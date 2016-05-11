using System;
namespace ZYC
{
	public struct FLASHWINFO
	{
		public uint cbSize;
		public IntPtr hwnd;
		public int dwFlags;
		public uint uCount;
		public int dwTimeout;
	}
}
