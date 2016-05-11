using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public class _SHFILEOPSTRUCT
	{
		public IntPtr hwnd;
		public uint wFunc;
		public string pFrom;
		public string pTo;
		public ushort fFlags;
		public int fAnyOperationsAborted;
		public IntPtr hNameMappings;
		public string lpszProgressTitle;
	}
}
