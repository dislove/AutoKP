using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct INTERNET_CACHE_ENTRY_INFO
	{
		public int dwStructSize;
		public IntPtr lpszSourceUrlName;
		public IntPtr lpszLocalFileName;
		public int CacheEntryType;
		public int dwUseCount;
		public int dwHitRate;
		public int dwSizeLow;
		public int dwSizeHigh;
		public _FILETIME LastModifiedTime;
		public _FILETIME ExpireTime;
		public _FILETIME LastAccessTime;
		public _FILETIME LastSyncTime;
		public IntPtr lpHeaderInfo;
		public int dwHeaderInfoSize;
		public IntPtr lpszFileExtension;
		public int dwExemptDelta;
	}
}
