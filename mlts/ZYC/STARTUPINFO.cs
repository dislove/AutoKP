using System;
namespace ZYC
{
	public struct STARTUPINFO
	{
		public int cb;
		public string lpReserved;
		public string lpDesktop;
		public string lpTitle;
		public int dwX;
		public int dwY;
		public int dwXSize;
		public int dwYSize;
		public int dwXCountChars;
		public int dwYCountChars;
		public int dwFillAttribute;
		public int dwFlags;
		public int wShowWindow;
		public int cbReserved2;
		public byte lpReserved2;
		public IntPtr hStdInput;
		public IntPtr htdOutput;
		public IntPtr hStdError;
	}
}
