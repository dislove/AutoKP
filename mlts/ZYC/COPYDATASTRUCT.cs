using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	public struct COPYDATASTRUCT
	{
		public IntPtr dwData;
		public int cbData;
		[MarshalAs(UnmanagedType.LPStr)]
		public string lpData;
	}
}
