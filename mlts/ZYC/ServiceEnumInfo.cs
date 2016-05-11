using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	public struct ServiceEnumInfo
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
		public string szPrefix;
		public string szDllName;
		public IntPtr hServiceHandle;
		public int dwServiceState;
	}
}
