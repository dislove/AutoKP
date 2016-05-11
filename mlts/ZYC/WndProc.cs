using System;
namespace ZYC
{
	public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
	public delegate int WNDPROC(IntPtr hwnd, uint uMsg, int wParam, int lParam);
}
