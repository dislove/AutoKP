using System;
namespace ZYC
{
	public struct FINDREPLACE
	{
		public int lStructSize;
		public IntPtr hwndOwner;
		public IntPtr hInstance;
		public int Flags;
		public string lpstrFindWhat;
		public string lpstrReplaceWith;
		public ushort wFindWhatLen;
		public ushort wReplaceWithLen;
		public uint lCustData;
		public FRHookProc lpfnHook;
		public string lpTemplateName;
	}
}
