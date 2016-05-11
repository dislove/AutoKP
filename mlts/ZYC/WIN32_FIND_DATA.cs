using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[BestFitMapping(false)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WIN32_FIND_DATA
	{
		public int dwFileAttributes;
		public int ftCreationTime_dwLowDateTime;
		public int ftCreationTime_dwHighDateTime;
		public int ftLastAccessTime_dwLowDateTime;
		public int ftLastAccessTime_dwHighDateTime;
		public int ftLastWriteTime_dwLowDateTime;
		public int ftLastWriteTime_dwHighDateTime;
		public int nFileSizeHigh;
		public int nFileSizeLow;
		public int dwReserved0;
		public int dwReserved1;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string cFileName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}
}
