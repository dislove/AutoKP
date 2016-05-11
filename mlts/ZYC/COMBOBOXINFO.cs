using System;
namespace ZYC
{
	public struct COMBOBOXINFO
	{
		public int cbSize;
		public RECT rcItem;
		public RECT rcButton;
		public int stateButton;
		public IntPtr hwndCombo;
		public IntPtr hwndItem;
		public IntPtr hwndList;
	}
}
