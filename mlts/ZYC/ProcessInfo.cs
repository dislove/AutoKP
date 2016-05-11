using System;
using System.Drawing;
namespace ZYC
{
	public struct ProcessInfo
	{
		public IntPtr hwnd;
		public string ClassName;
		public string WindowText;
		public string path;
		public int processsize;
		public Point location;
		public Size wsize;
		public Size csize;
		public DateTime starttime;
		public string runtime;
		public IntPtr phwnd;
		public int id;
		public string text;
		public int dwStyle;
		public int dwExStyle;
		public uint cxWindowBorders;
		public uint cyWindowBorders;
	}
}
