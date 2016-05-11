using System;
namespace ZYC
{
	public struct SHELLEXECUTEINFO
	{
		public int cbSize;
		public int fMask;
		public IntPtr hwnd;
		public string lpVerb;
		public string lpFile;
		public string lpParameters;
		public string lpDirectory;
		public int nShow;
		public IntPtr hInstApp;
		public IntPtr lpIDList;
		public string lpClass;
		public IntPtr hkeyClass;
		public int dwHotKey;
		public IntPtr hIcon;
		public IntPtr hProcess;
	}
}
