using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MODULEENTRY32
	{
		public int dwSize;
		public int th32ModuleID;
		public int th32ProcessID;
		public int GlblcntUsage;
		public int ProccntUsage;
		public byte modBaseAddr;
		public int modBaseSize;
		public IntPtr hModule;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string szModule;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szExePath;
	}
}
