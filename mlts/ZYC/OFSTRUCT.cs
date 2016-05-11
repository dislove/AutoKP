using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	public struct OFSTRUCT
	{
		public byte cBytes;
		public byte fFixedDisk;
		public ushort nErrCode;
		public ushort Reserved1;
		public ushort Reserved2;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
		public string szPathName;
	}
}
