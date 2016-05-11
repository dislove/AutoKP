using System;
namespace ZYC
{
	public struct MENUITEMINFO
	{
		public uint cbSize;
		public uint fMask;
		public uint fType;
		public uint fState;
		public int wID;
		public int hSubMenu;
		public int hbmpChecked;
		public int hbmpUnchecked;
		public int dwItemData;
		public IntPtr dwTypeData;
		public uint cch;
	}
}
