using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential)]
	public class SECURITY_ATTRIBUTES
	{
		public int nLength;
		public string lpSecurityDescriptor;
		public bool bInheritHandle;
	}
}
