using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct TIME_ZONE_INFORMATION
	{
		public long Bias;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string StandardName;
		public SYSTEMTIME StandardDate;
		public long StandardBias;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DaylightName;
		private SYSTEMTIME DaylightDate;
		public long DaylightBias;
	}
}
