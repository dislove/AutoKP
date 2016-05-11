using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Explicit)]
	public struct INPUT
	{
		[FieldOffset(0)]
		public int type;
		[FieldOffset(4)]
		public MOUSEINPUT mi;
		[FieldOffset(4)]
		public KEYBDINPUT ki;
		[FieldOffset(4)]
		public HARDWAREINPUT hi;
	}
}
