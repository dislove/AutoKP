using System;
namespace ZYC
{
	public struct BY_HANDLE_FILE_INFORMATION
	{
		public int dwFileAttributes;
		public _FILETIME ftCreationTime;
		public _FILETIME ftLastAccessTime;
		public _FILETIME ftLastWriteTime;
		public int dwVolumeSerialNumber;
		public int nFileSizeHigh;
		public int nFileSizeLow;
		public int nNumberOfLinks;
		public int nFileIndexHigh;
		public int nFileIndexLow;
		public int dwOID;
	}
}
