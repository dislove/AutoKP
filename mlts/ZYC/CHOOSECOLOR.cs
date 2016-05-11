using System;
using System.Runtime.InteropServices;
namespace ZYC
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class CHOOSECOLOR
	{
		public int lStructSize = Marshal.SizeOf(typeof(CHOOSECOLOR));
		public IntPtr hwndOwner;
		public IntPtr hInstance;
		public int rgbResult;
		public IntPtr lpCustColors;
		public int Flags;
		public IntPtr lCustData = IntPtr.Zero;
		public WndProc lpfnHook;
		public string lpTemplateName;
	}
}
