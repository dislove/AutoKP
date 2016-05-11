using System;
using System.Drawing;
namespace ZYC
{
	public struct MSG
	{
		public IntPtr hwnd;
		public uint message;
		public int wParam;
		public int lParam;
		public int time;
		public Point pt;
	}
}
