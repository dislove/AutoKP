using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace ZYC
{
	public sealed class APIMethod
	{
		public class ScreenCapture
		{
			public Image CaptureScreen()
			{
				return this.CaptureWindow(WindowsAPI.GetDesktopWindow());
			}
			public Image CaptureScreen(IntPtr hwnd)
			{
				return this.CaptureWindow(hwnd);
			}
			public Image CaptureWindow(IntPtr handle)
			{
				IntPtr windowDC = WindowsAPI.GetWindowDC(handle);
				RECT rECT = default(RECT);
				WindowsAPI.GetWindowRect(handle, ref rECT);
				int nWidth = rECT.right - rECT.left;
				int nHeight = rECT.bottom - rECT.top;
				IntPtr intPtr = WindowsAPI.CreateCompatibleDC(windowDC);
				IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
				IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, intPtr2);
				WindowsAPI.BitBlt(intPtr, 0, 0, nWidth, nHeight, windowDC, 0, 0, 13369376u);
				WindowsAPI.SelectObject(intPtr, hgdiobj);
				WindowsAPI.DeleteDC(intPtr);
				WindowsAPI.ReleaseDC(handle, windowDC);
				Image result = Image.FromHbitmap(intPtr2);
				WindowsAPI.DeleteObject(intPtr2);
				return result;
			}
			public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
			{
				Image image = this.CaptureWindow(handle);
				image.Save(filename, format);
			}
			public void CaptureScreenToFile(string filename, ImageFormat format)
			{
				Image image = this.CaptureScreen();
				image.Save(filename, format);
			}
		}
		private delegate bool Delegate_Beep(uint dwFreq, uint dwDuration);
		public class PlayAudio
		{
			public enum State
			{
				mPlaying = 1,
				mPuase,
				mStop
			}
			public struct structMCI
			{
				public bool bMut;
				public int iDur;
				public int iPos;
				public int iVol;
				public int iBal;
				public string iName;
				public APIMethod.PlayAudio.State state;
			}
			public delegate void Loop(object sender, EventArgs e);
			private System.Windows.Forms.Timer time;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			private string Name = "";
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			private string durLength = "";
			[MarshalAs(UnmanagedType.LPTStr)]
			private string TemStr = "";
			private int ilong;
			public APIMethod.PlayAudio.structMCI mc = default(APIMethod.PlayAudio.structMCI);
			public event APIMethod.PlayAudio.Loop Playing
			{
				[MethodImpl(MethodImplOptions.Synchronized)]
				add
				{
					///this.Playing += (APIMethod.PlayAudio.Loop)Delegate.Combine(this.Playing, value);
				}
				[MethodImpl(MethodImplOptions.Synchronized)]
				remove
				{
					//this.Playing -= (APIMethod.PlayAudio.Loop)Delegate.Remove(this.Playing, value);
				}
			}
			public event APIMethod.PlayAudio.Loop Stoped
			{
				[MethodImpl(MethodImplOptions.Synchronized)]
				add
				{
					//this.Stoped = (APIMethod.PlayAudio.Loop)Delegate.Combine(this.Stoped, value);
				}
				[MethodImpl(MethodImplOptions.Synchronized)]
				remove
				{
					//this.Stoped = (APIMethod.PlayAudio.Loop)Delegate.Remove(this.Stoped, value);
				}
			}
			public event APIMethod.PlayAudio.Loop Paused
			{
				[MethodImpl(MethodImplOptions.Synchronized)]
				add
				{
					//this.Paused = (APIMethod.PlayAudio.Loop)Delegate.Combine(this.Paused, value);
				}
				[MethodImpl(MethodImplOptions.Synchronized)]
				remove
				{
					//this.Paused = (APIMethod.PlayAudio.Loop)Delegate.Remove(this.Paused, value);
				}
			}
			public string FileName
			{
				get
				{
					return this.mc.iName;
				}
				set
				{
					try
					{
						this.TemStr = "";
						this.TemStr = this.TemStr.PadLeft(127, Convert.ToChar(" "));
						this.Name = this.Name.PadLeft(260, Convert.ToChar(" "));
						this.mc.iName = value;
						if (!Path.HasExtension(this.mc.iName))
						{
							this.mc.iName = Path.GetFileNameWithoutExtension(this.mc.iName);
							this.mc.iName = this.mc.iName + ".mp3";
						}
						this.ilong = WindowsAPI.GetShortPathName(this.mc.iName, this.Name, this.Name.Length);
						this.Name = this.GetCurrPath(this.Name);
						this.Name = string.Concat(new object[]
						{
							"open ",
							Convert.ToChar(34),
							this.Name,
							Convert.ToChar(34),
							" alias media"
						});
						this.ilong = WindowsAPI.mciSendString("close all", this.TemStr, this.TemStr.Length, IntPtr.Zero);
						this.ilong = WindowsAPI.mciSendString(this.Name, this.TemStr, this.TemStr.Length, IntPtr.Zero);
						this.ilong = WindowsAPI.mciSendString("set media time format milliseconds", this.TemStr, this.TemStr.Length, IntPtr.Zero);
						this.mc.state = APIMethod.PlayAudio.State.mStop;
					}
					catch
					{
						MessageBox.Show("出错错误!");
					}
				}
			}
			public int Duration
			{
				get
				{
					this.durLength = "";
					this.durLength = this.durLength.PadLeft(128, Convert.ToChar(" "));
					WindowsAPI.mciSendString("status media length", this.durLength, this.durLength.Length, IntPtr.Zero);
					this.durLength = this.durLength.Trim();
					int result;
					if (this.durLength == "")
					{
						result = 0;
					}
					else
					{
						result = (int)(Convert.ToDouble(this.durLength) / 1000.0);
					}
					return result;
				}
			}
			public int CurrentPosition
			{
				get
				{
					this.durLength = "";
					this.durLength = this.durLength.PadLeft(128, Convert.ToChar(" "));
					WindowsAPI.mciSendString("status media position", this.durLength, this.durLength.Length, IntPtr.Zero);
					this.durLength = this.durLength.Trim();
					int result;
					if (this.durLength == "")
					{
						result = 0;
					}
					else
					{
						try
						{
							this.mc.iPos = (int)(Convert.ToDouble(this.durLength) / 1000.0);
							result = this.mc.iPos;
						}
						catch
						{
							result = (this.mc.iPos = 0);
						}
					}
					return result;
				}
			}
			public PlayAudio()
			{
				this.time = new System.Windows.Forms.Timer();
				this.time.Interval = 500;
				this.time.Tick += new EventHandler(this.time_Tick);
			}
			public void Play()
			{
				this.time.Enabled = true;
				this.TemStr = "";
				this.TemStr = this.TemStr.PadLeft(127, Convert.ToChar(" "));
				WindowsAPI.mciSendString("play media", this.TemStr, this.TemStr.Length, IntPtr.Zero);
				this.mc.state = APIMethod.PlayAudio.State.mPlaying;
			}
			public void Stop()
			{
				this.TemStr = "";
				this.TemStr = this.TemStr.PadLeft(128, Convert.ToChar(" "));
				this.ilong = WindowsAPI.mciSendString("close media", this.TemStr, 128, IntPtr.Zero);
				this.ilong = WindowsAPI.mciSendString("close all", this.TemStr, 128, IntPtr.Zero);
				this.mc.state = APIMethod.PlayAudio.State.mStop;
				this.time.Enabled = false;
			}
			public void Puase()
			{
				this.TemStr = "";
				this.TemStr = this.TemStr.PadLeft(128, Convert.ToChar(" "));
				this.ilong = WindowsAPI.mciSendString("pause media", this.TemStr, this.TemStr.Length, IntPtr.Zero);
				this.mc.state = APIMethod.PlayAudio.State.mPuase;
			}
			public void OpenCD()
			{
				WindowsAPI.mciSendString("set cdaudio door open", "", 0, IntPtr.Zero);
			}
			public void CloseCD()
			{
				WindowsAPI.mciSendString("set cdaudio door closed", "", 0, IntPtr.Zero);
			}
			private string GetCurrPath(string name)
			{
				string result;
				if (name.Length < 1)
				{
					result = "";
				}
				else
				{
					name = name.Trim();
					name = name.Substring(0, name.Length - 1);
					result = name;
				}
				return result;
			}
			private void time_Tick(object sender, EventArgs e)
			{
				if (this.mc.state == APIMethod.PlayAudio.State.mPlaying)
				{
					this.OnPlayLoop();
				}
				else
				{
					if (this.mc.state == APIMethod.PlayAudio.State.mPuase)
					{
						this.OnPaused();
					}
					else
					{
						if (this.mc.state == APIMethod.PlayAudio.State.mStop)
						{
							this.OnStoped();
						}
					}
				}
			}
			private void OnPlayLoop()
			{
				try
				{
					//this.Playing(this, new EventArgs());
				}
				catch
				{
				}
			}
			private void OnPaused()
			{
				try
				{
					//this.Paused(this, new EventArgs());
				}
				catch
				{
				}
			}
			private void OnStoped()
			{
				try
				{
					//this.Stoped(this, new EventArgs());
				}
				catch
				{
				}
			}
		}
		public class SystemMenu
		{
			public const int M_AboutID = 256;
			public const int M_ResetID = 257;
			public const int M_Separator = 258;
			private IntPtr syshwnd;
			private int count = 0;
			public SystemMenu(IntPtr hwnd)
			{
				this.syshwnd = WindowsAPI.GetSystemMenu(hwnd, 0);
				this.count = this.GetMenuCount(hwnd);
			}
			public static void ResetSystemMenu(IntPtr hwnd)
			{
				WindowsAPI.GetSystemMenu(hwnd, 1);
			}
			public bool InsertSeparator(uint Pos)
			{
				return WindowsAPI.InsertMenu(this.syshwnd, Pos, 3072u, 258u, "");
			}
			public bool InsertMenu(uint Pos, uint ID, string Item)
			{
				return WindowsAPI.InsertMenu(this.syshwnd, Pos, 1024u, ID, Item);
			}
			public bool InsertMenu(uint Pos, uint Flags, uint ID, string Item)
			{
				return WindowsAPI.InsertMenu(this.syshwnd, Pos, Flags, ID, Item);
			}
			public bool AppendSeparator()
			{
				return WindowsAPI.AppendMenu(this.syshwnd, 3072u, 258u, "");
			}
			public bool AppendMenu(uint ID, string Item)
			{
				return WindowsAPI.AppendMenu(this.syshwnd, 0u, ID, Item);
			}
			public bool AppendMenu(uint ID, string Item, uint Flags)
			{
				return WindowsAPI.AppendMenu(this.syshwnd, Flags, ID, Item);
			}
			public bool ModifyMenu(uint Pos, uint flags, uint ID, string title)
			{
				return WindowsAPI.ModifyMenu(this.syshwnd, Pos, flags, ID, title);
			}
			public bool DeleteMove()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61456u, 4096u);
			}
			public bool DeleteSIZE()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61440u, 4096u);
			}
			public bool DeleteMINIMIZE()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61472u, 4096u);
			}
			public bool DeleteMAXIMIZE()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61488u, 4096u);
			}
			public bool DeleteCLOSE()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61536u, 4096u);
			}
			public bool DeleteRESTORE()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61728u, 4096u);
			}
			public bool DeleteSEPARATOR()
			{
				return WindowsAPI.DeleteMenu(this.syshwnd, 61455u, 2048u);
			}
			public bool DeleteAll()
			{
				bool result = false;
				for (int i = this.count - 1; i >= 0; i--)
				{
					result = WindowsAPI.DeleteMenu(this.syshwnd, (uint)i, 1024u);
				}
				return result;
			}
			private bool DeleteMove(IntPtr hMenu)
			{
				return WindowsAPI.DeleteMenu(hMenu, 61456u, 4096u);
			}
			private bool DeleteSIZE(IntPtr hMenu)
			{
				return WindowsAPI.DeleteMenu(hMenu, 61440u, 4096u);
			}
			private bool DeleteMINIMIZE(IntPtr hMenu)
			{
				return WindowsAPI.DeleteMenu(hMenu, 61472u, 4096u);
			}
			private bool DeleteMAXIMIZE(IntPtr hMenu)
			{
				return WindowsAPI.DeleteMenu(hMenu, 61488u, 4096u);
			}
			private bool DeleteCLOSE(IntPtr hMenu)
			{
				return WindowsAPI.DeleteMenu(hMenu, 61536u, 4096u);
			}
			private bool DeleteRESTORE(IntPtr hMenu)
			{
				return WindowsAPI.DeleteMenu(hMenu, 61728u, 4096u);
			}
			private bool RemoveMove(IntPtr hMenu)
			{
				return WindowsAPI.RemoveMenu(hMenu, 61456u, 4096u);
			}
			private bool RemoveSIZE(IntPtr hMenu)
			{
				return WindowsAPI.RemoveMenu(hMenu, 61440u, 4096u);
			}
			private bool RemoveMINIMIZE(IntPtr hMenu)
			{
				return WindowsAPI.RemoveMenu(hMenu, 61472u, 4096u);
			}
			private bool RemoveMAXIMIZE(IntPtr hMenu)
			{
				return WindowsAPI.RemoveMenu(hMenu, 61488u, 4096u);
			}
			private bool RemoveCLOSE(IntPtr hMenu)
			{
				return WindowsAPI.RemoveMenu(hMenu, 61536u, 4096u);
			}
			private bool RemoveRESTORE(IntPtr hMenu)
			{
				return WindowsAPI.RemoveMenu(hMenu, 61728u, 4096u);
			}
			public MENUINFO GetWindowMenu(IntPtr hwnd)
			{
				MENUINFO mENUINFO = default(MENUINFO);
				mENUINFO.cbSize = Marshal.SizeOf(mENUINFO);
				mENUINFO.fMask = 31;
				WindowsAPI.GetMenuInfo(WindowsAPI.GetSystemMenu(hwnd, 0), ref mENUINFO);
				return mENUINFO;
			}
			public void SystemMenuColor(Form frm, Color color)
			{
				MENUINFO mENUINFO = default(MENUINFO);
				mENUINFO.cbSize = Marshal.SizeOf(mENUINFO);
				mENUINFO.fMask = 2;
				Bitmap bitmap = new Bitmap(200, 200);
				Brush brush = new SolidBrush(color);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.FillRectangle(brush, new Rectangle(0, 0, 200, 200));
				if (bitmap == null)
				{
					mENUINFO.hbrBack = IntPtr.Zero;
				}
				else
				{
					mENUINFO.hbrBack = WindowsAPI.CreatePatternBrush(bitmap.GetHbitmap());
				}
				try
				{
					WindowsAPI.SetMenuInfo(WindowsAPI.GetSystemMenu(frm.Handle, 0), ref mENUINFO);
				}
				catch
				{
				}
			}
			public void SystemMenuColor(Form frm, Image image)
			{
				MENUINFO mENUINFO = default(MENUINFO);
				mENUINFO.cbSize = Marshal.SizeOf(mENUINFO);
				mENUINFO.fMask = 2;
				Bitmap bitmap = new Bitmap(200, 200);
				Brush brush = new TextureBrush(image);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.FillRectangle(brush, new Rectangle(0, 0, 200, 200));
				if (bitmap == null)
				{
					mENUINFO.hbrBack = IntPtr.Zero;
				}
				else
				{
					mENUINFO.hbrBack = WindowsAPI.CreatePatternBrush(bitmap.GetHbitmap());
				}
				try
				{
					WindowsAPI.SetMenuInfo(WindowsAPI.GetSystemMenu(frm.Handle, 0), ref mENUINFO);
				}
				catch
				{
				}
			}
			public void SystemMenuColor(Form frm, Color color1, Color color2, int direct)
			{
				MENUINFO mENUINFO = default(MENUINFO);
				mENUINFO.cbSize = Marshal.SizeOf(mENUINFO);
				mENUINFO.fMask = 2;
				Bitmap bitmap = new Bitmap(200, 200);
				Point point;
				Point point2;
				if (direct == 0)
				{
					point = new Point(0, 0);
					point2 = new Point(200, 0);
				}
				else
				{
					if (direct == 1)
					{
						point = new Point(0, 0);
						point2 = new Point(0, 200);
					}
					else
					{
						if (direct == 2)
						{
							point2 = new Point(0, 0);
							point = new Point(200, 200);
						}
						else
						{
							point2 = new Point(200, 0);
							point = new Point(0, 200);
						}
					}
				}
				Brush brush = new LinearGradientBrush(point, point2, color1, color2);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.FillRectangle(brush, new Rectangle(0, 0, 200, 200));
				if (bitmap == null)
				{
					mENUINFO.hbrBack = IntPtr.Zero;
				}
				else
				{
					mENUINFO.hbrBack = WindowsAPI.CreatePatternBrush(bitmap.GetHbitmap());
				}
				try
				{
					WindowsAPI.SetMenuInfo(WindowsAPI.GetSystemMenu(frm.Handle, 0), ref mENUINFO);
				}
				catch
				{
				}
			}
			public void UnSystemMenuColor(Form frm)
			{
				MENUINFO mENUINFO = default(MENUINFO);
				mENUINFO.cbSize = Marshal.SizeOf(mENUINFO);
				mENUINFO.fMask = 2;
				mENUINFO.hbrBack = IntPtr.Zero;
				try
				{
					WindowsAPI.SetMenuInfo(WindowsAPI.GetSystemMenu(frm.Handle, 0), ref mENUINFO);
				}
				catch
				{
				}
			}
			public int GetMenuCount(IntPtr hwnd)
			{
				IntPtr systemMenu = WindowsAPI.GetSystemMenu(hwnd, 0);
				return WindowsAPI.GetMenuItemCount(systemMenu);
			}
		}
		public class WindowsMessage
		{
			public static void SendMessage(string destProcessName, int msgID, string strMsg)
			{
				if (strMsg != null)
				{
					COPYDATASTRUCT cOPYDATASTRUCT;
					cOPYDATASTRUCT.dwData = (IntPtr)msgID;
					cOPYDATASTRUCT.lpData = strMsg;
					cOPYDATASTRUCT.cbData = Encoding.Default.GetBytes(strMsg).Length + 1;
					WindowsAPI.SendMessage(WindowsAPI.FindWindow(null, destProcessName), 74u, 0, ref cOPYDATASTRUCT);
				}
			}
			public static string ReceiveMessage(ref Message m)
			{
				return ((COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT))).lpData;
			}
		}
		public class Shell_NotifyIconEx
		{
			internal delegate void delegateOfCallBack(MouseButtons mb);
			public static readonly Version myVersion = new Version(1, 2);
			private readonly IntPtr formTmpHwnd = IntPtr.Zero;
			private readonly bool VersionOk = false;
			private bool forgetDelNotifyBox = false;
			internal IntPtr formHwnd = IntPtr.Zero;
			internal IntPtr contextMenuHwnd = IntPtr.Zero;
			internal APIMethod.Shell_NotifyIconEx.delegateOfCallBack _delegateOfCallBack = null;
			internal readonly int WM_NOTIFY_TRAY = 3025;
			internal readonly uint uID = 5000u;
			public bool VersionPass
			{
				get
				{
					return this.VersionOk;
				}
			}
			public Shell_NotifyIconEx(IntPtr hwnd)
			{
				this.WM_NOTIFY_TRAY++;
				this.uID += 1u;
				this.formTmpHwnd = hwnd;
				this.VersionOk = (this.GetShell32VersionInfo() >= 5);
			}
			~Shell_NotifyIconEx()
			{
				if (this.forgetDelNotifyBox)
				{
					this.DelNotifyBox();
				}
			}
			private NOTIFYICONDATA GetNOTIFYICONDATA(IntPtr iconHwnd, string sTip, string boxTitle, string boxText)
			{
				NOTIFYICONDATA nOTIFYICONDATA = default(NOTIFYICONDATA);
				nOTIFYICONDATA.cbSize = Marshal.SizeOf(nOTIFYICONDATA);
				nOTIFYICONDATA.hWnd = this.formTmpHwnd;
				nOTIFYICONDATA.uID = this.uID;
				nOTIFYICONDATA.uFlags = 23u;
				nOTIFYICONDATA.uCallbackMessage = (uint)this.WM_NOTIFY_TRAY;
				nOTIFYICONDATA.hIcon = iconHwnd;
				nOTIFYICONDATA.uTimeout = 10003u;
				nOTIFYICONDATA.dwInfoFlags = 1;
				nOTIFYICONDATA.szTip = sTip;
				nOTIFYICONDATA.szInfoTitle = boxTitle;
				nOTIFYICONDATA.szInfo = boxText;
				return nOTIFYICONDATA;
			}
			private int GetShell32VersionInfo()
			{
				FileInfo fileInfo = new FileInfo(Path.Combine(Environment.SystemDirectory, "shell32.dll"));
				int result;
				if (fileInfo.Exists)
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
					int num = versionInfo.FileVersion.IndexOf('.');
					if (num > 0)
					{
						try
						{
							result = int.Parse(versionInfo.FileVersion.Substring(0, num));
							return result;
						}
						catch
						{
						}
					}
				}
				result = 0;
				return result;
			}
			public int AddNotifyBox(IntPtr iconHwnd, string sTip, string boxTitle, string boxText)
			{
				int result;
				if (!this.VersionOk)
				{
					result = -1;
				}
				else
				{
					NOTIFYICONDATA nOTIFYICONDATA = this.GetNOTIFYICONDATA(iconHwnd, sTip, boxTitle, boxText);
					if (WindowsAPI.Shell_NotifyIcon(0, ref nOTIFYICONDATA))
					{
						this.forgetDelNotifyBox = true;
						result = 1;
					}
					else
					{
						result = 0;
					}
				}
				return result;
			}
			public int DelNotifyBox()
			{
				int result;
				if (!this.VersionOk)
				{
					result = -1;
				}
				else
				{
					NOTIFYICONDATA nOTIFYICONDATA = this.GetNOTIFYICONDATA(IntPtr.Zero, null, null, null);
					if (WindowsAPI.Shell_NotifyIcon(2, ref nOTIFYICONDATA))
					{
						this.forgetDelNotifyBox = false;
						result = 1;
					}
					else
					{
						result = 0;
					}
				}
				return result;
			}
			public int ModiNotifyBox(IntPtr iconHwnd, string sTip, string boxTitle, string boxText)
			{
				int result;
				if (!this.VersionOk)
				{
					result = -1;
				}
				else
				{
					NOTIFYICONDATA nOTIFYICONDATA = this.GetNOTIFYICONDATA(iconHwnd, sTip, boxTitle, boxText);
					result = (WindowsAPI.Shell_NotifyIcon(1, ref nOTIFYICONDATA) ? 1 : 0);
				}
				return result;
			}
			public void ConnectMyMenu(IntPtr _formHwnd, IntPtr _contextMenuHwnd)
			{
				this.formHwnd = _formHwnd;
				this.contextMenuHwnd = _contextMenuHwnd;
			}
			public void Dispose()
			{
				this._delegateOfCallBack = null;
			}
		}
		private List<WindowInfo> childList = new List<WindowInfo>();
		public IntPtr DeskHwnd
		{
			get
			{
				return WindowsAPI.GetDesktopWindow();
			}
		}
		public void DoExitWin(int flg)
		{
			IntPtr currentProcess = WindowsAPI.GetCurrentProcess();
			IntPtr zero = IntPtr.Zero;
			bool flag = WindowsAPI.OpenProcessToken(currentProcess, 40, ref zero);
			TokPriv1Luid tokPriv1Luid;
			tokPriv1Luid.Count = 1;
			tokPriv1Luid.Luid = 0L;
			tokPriv1Luid.Attr = 2;
			flag = WindowsAPI.LookupPrivilegeValue(null, "SeShutdownPrivilege", ref tokPriv1Luid.Luid);
			flag = WindowsAPI.AdjustTokenPrivileges(zero, false, ref tokPriv1Luid, 0, IntPtr.Zero, IntPtr.Zero);
			flag = WindowsAPI.ExitWindowsEx(flg, 0);
		}
		public void DoExitWin(WinExit exit)
		{
			int uFlags = 0;
			if (exit == WinExit.SHUTDOWN)
			{
				uFlags = 1;
			}
			else
			{
				if (exit == WinExit.REBOOT)
				{
					uFlags = 2;
				}
				else
				{
					if (exit == WinExit.POWEROFF)
					{
						uFlags = 8;
					}
					else
					{
						if (exit == WinExit.LOGOFF)
						{
							uFlags = 0;
						}
						else
						{
							if (exit == WinExit.FORCEIFHUNG)
							{
								uFlags = 16;
							}
							else
							{
								if (exit == WinExit.FORCE)
								{
									uFlags = 4;
								}
							}
						}
					}
				}
			}
			IntPtr currentProcess = WindowsAPI.GetCurrentProcess();
			IntPtr zero = IntPtr.Zero;
			bool flag = WindowsAPI.OpenProcessToken(currentProcess, 40, ref zero);
			TokPriv1Luid tokPriv1Luid;
			tokPriv1Luid.Count = 1;
			tokPriv1Luid.Luid = 0L;
			tokPriv1Luid.Attr = 2;
			flag = WindowsAPI.LookupPrivilegeValue(null, "SeShutdownPrivilege", ref tokPriv1Luid.Luid);
			flag = WindowsAPI.AdjustTokenPrivileges(zero, false, ref tokPriv1Luid, 0, IntPtr.Zero, IntPtr.Zero);
			flag = WindowsAPI.ExitWindowsEx(uFlags, 0);
		}
		public void DisplaySetting(int width, int height, int frequency)
		{
			DEVMODE dEVMODE = default(DEVMODE);
			dEVMODE.dmSize = (int)((short)Marshal.SizeOf(typeof(DEVMODE)));
			dEVMODE.dmPelsWidth = width;
			dEVMODE.dmPelsHeight = height;
			dEVMODE.dmDisplayFrequency = frequency;
			dEVMODE.dmFields = 5767168;
			long num = (long)WindowsAPI.ChangeDisplaySettings(ref dEVMODE, 1);
		}
		public void DisplaySetting(DisplaySettings dis, int frequency)
		{
			int dmPelsWidth = 1024;
			int dmPelsHeight = 768;
			switch (dis)
			{
			case DisplaySettings.Smallest:
				dmPelsWidth = 600;
				dmPelsHeight = 480;
				break;
			case DisplaySettings.Small:
				dmPelsWidth = 800;
				dmPelsHeight = 600;
				break;
			case DisplaySettings.Normal:
				dmPelsWidth = 1024;
				dmPelsHeight = 768;
				break;
			case DisplaySettings.Largest:
				dmPelsWidth = 1280;
				dmPelsHeight = 1024;
				break;
			}
			DEVMODE dEVMODE = default(DEVMODE);
			dEVMODE.dmSize = (int)((short)Marshal.SizeOf(typeof(DEVMODE)));
			dEVMODE.dmPelsWidth = dmPelsWidth;
			dEVMODE.dmPelsHeight = dmPelsHeight;
			dEVMODE.dmDisplayFrequency = frequency;
			dEVMODE.dmFields = 5767168;
			long num = (long)WindowsAPI.ChangeDisplaySettings(ref dEVMODE, 1);
		}
		public string GetWindowsDirectory()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WindowsAPI.GetWindowsDirectory(stringBuilder, 100);
			return stringBuilder.ToString() + "\\";
		}
		public string GetSystemDirectory()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WindowsAPI.GetSystemDirectory(stringBuilder, 100);
			return stringBuilder.ToString() + "\\";
		}
		public string GetTempPath()
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			WindowsAPI.GetTempPath(stringBuilder.Capacity, stringBuilder);
			return stringBuilder.ToString();
		}
		public string PathRemoveBack(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(path);
			string text = WindowsAPI.PathRemoveBackslash(stringBuilder);
			return stringBuilder.ToString();
		}
		public string PathRemoveArgs(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(path);
			WindowsAPI.PathRemoveArgs(stringBuilder);
			return stringBuilder.ToString();
		}
		public string PathRemoveBlanks(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(path);
			WindowsAPI.PathRemoveBlanks(stringBuilder);
			return stringBuilder.ToString();
		}
		public string PathRemoveExtension(string path)
		{
			StringBuilder stringBuilder = new StringBuilder(path);
			WindowsAPI.PathRemoveExtension(stringBuilder);
			return stringBuilder.ToString();
		}
		public string PathRenameExtension(string path, string ext)
		{
			StringBuilder stringBuilder = new StringBuilder(path);
			WindowsAPI.PathRenameExtension(stringBuilder, ext);
			return stringBuilder.ToString();
		}
		public void SleepProgram(int time)
		{
			WindowsAPI.Sleep(time);
		}
		public int GetInternetTempFiles()
		{
			TStringList tStringList = new TStringList();
			int num = 0;
			int num2 = 0;
			WindowsAPI.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref num2);
			int result;
			if (Marshal.GetLastWin32Error() == 259)
			{
				result = 0;
			}
			else
			{
				int num3 = num2;
				IntPtr intPtr = Marshal.AllocHGlobal(num3);
				IntPtr hEnumHandle = WindowsAPI.FindFirstUrlCacheEntry(null, intPtr, ref num2);
				while (true)
				{
					INTERNET_CACHE_ENTRY_INFO iNTERNET_CACHE_ENTRY_INFO = (INTERNET_CACHE_ENTRY_INFO)Marshal.PtrToStructure(intPtr, typeof(INTERNET_CACHE_ENTRY_INFO));
					string text = this.FileTimeToWindowsTime(iNTERNET_CACHE_ENTRY_INFO.LastModifiedTime).ToString();
					string text2 = this.FileTimeToWindowsTime(iNTERNET_CACHE_ENTRY_INFO.ExpireTime).ToString();
					string text3 = this.FileTimeToWindowsTime(iNTERNET_CACHE_ENTRY_INFO.LastAccessTime).ToString();
					string text4 = this.FileTimeToWindowsTime(iNTERNET_CACHE_ENTRY_INFO.LastSyncTime).ToString();
					try
					{
						string text5 = Marshal.PtrToStringAuto(iNTERNET_CACHE_ENTRY_INFO.lpszSourceUrlName);
						num++;
					}
					catch
					{
					}
					num2 = num3;
					bool flag = WindowsAPI.FindNextUrlCacheEntry(hEnumHandle, intPtr, ref num2);
					if (!flag && Marshal.GetLastWin32Error() == 259)
					{
						break;
					}
					if (!flag && num2 > num3)
					{
						num3 = num2;
						intPtr = Marshal.ReAllocHGlobal(intPtr, (IntPtr)num3);
						WindowsAPI.FindNextUrlCacheEntry(hEnumHandle, intPtr, ref num2);
					}
				}
				Marshal.FreeHGlobal(intPtr);
				result = num;
			}
			return result;
		}
		public Point GetLastMessagePosition()
		{
			return new Point
			{
				X = WindowsAPI.GET_X_LPARAM(WindowsAPI.GetMessagePos()),
				Y = WindowsAPI.GET_Y_LPARAM(WindowsAPI.GetMessagePos())
			};
		}
		public OSVERSIONINFO GetWindowsVersion()
		{
			OSVERSIONINFO oSVERSIONINFO = default(OSVERSIONINFO);
			oSVERSIONINFO.dwOSVersionInfoSize = Marshal.SizeOf(oSVERSIONINFO);
			WindowsAPI.GetVersionEx(ref oSVERSIONINFO);
			return oSVERSIONINFO;
		}
		public OSVERSIONINFOEX GetWindowsVersionEx()
		{
			OSVERSIONINFOEX oSVERSIONINFOEX = default(OSVERSIONINFOEX);
			oSVERSIONINFOEX.dwOSVersionInfoSize = Marshal.SizeOf(oSVERSIONINFOEX);
			WindowsAPI.GetVersionEx(ref oSVERSIONINFOEX);
			return oSVERSIONINFOEX;
		}
		private OSVERSIONINFOEX VerifyVersionInfo()
		{
			OSVERSIONINFOEX oSVERSIONINFOEX = default(OSVERSIONINFOEX);
			oSVERSIONINFOEX.dwOSVersionInfoSize = Marshal.SizeOf(oSVERSIONINFOEX);
			WindowsAPI.VerifyVersionInfo(ref oSVERSIONINFOEX, 255, 6);
			return oSVERSIONINFOEX;
		}
		public string GetCurrentWindowsVersion()
		{
			OSVERSIONINFO windowsVersion = this.GetWindowsVersion();
			string a = string.Concat(new string[]
			{
				windowsVersion.dwMajorVersion.ToString(),
				".",
				windowsVersion.dwMinorVersion.ToString(),
				".",
				windowsVersion.dwBuildNumber.ToString(),
				" ",
				windowsVersion.szCSDVersion,
				" ",
				windowsVersion.dwPlatformId.ToString()
			});
			string result;
			if (a == "5.1.2600  2" || a == "5.1.2600 Service Pack 1 2" || a == "5.1.2600 Service Pack 2 2" || a == "5.1.2600 Service Pack 3 2")
			{
				result = "Windows XP";
			}
			else
			{
				if (a == "6.0.6000  2" || a == "6.0.6001 Service Pack 1 2")
				{
					result = "Windows Vista";
				}
				else
				{
					if (a == "6.0.6001")
					{
						result = "Windows 2008";
					}
					else
					{
						result = "Unknown";
					}
				}
			}
			return result;
		}
		public string GetCurrentWindowsVersionEx()
		{
			OSVERSIONINFOEX windowsVersionEx = this.GetWindowsVersionEx();
			string a = string.Concat(new string[]
			{
				windowsVersionEx.dwMajorVersion.ToString(),
				".",
				windowsVersionEx.dwMinorVersion.ToString(),
				".",
				windowsVersionEx.dwBuildNumber.ToString(),
				" ",
				windowsVersionEx.szCSDVersion,
				" ",
				windowsVersionEx.dwPlatformId.ToString()
			});
			string result;
			if (a == "5.1.2600  2")
			{
				result = "Windows XP";
			}
			else
			{
				if (a == "5.1.2600 Service Pack 1 2")
				{
					result = "Windows XP SP1";
				}
				else
				{
					if (a == "5.1.2600 Service Pack 2 2")
					{
						result = "Windows XP SP2";
					}
					else
					{
						if (a == "5.1.2600 Service Pack 3 2")
						{
							result = "Windows XP SP2";
						}
						else
						{
							if (a == "6.0.6000  2")
							{
								result = "Windows Vista";
							}
							else
							{
								if (a == "6.0.6001 Service Pack 1 2")
								{
									result = "Windows Vista Sevice Pack 1";
								}
								else
								{
									if (a == "6.0.6001")
									{
										result = "Windows 2008";
									}
									else
									{
										result = "Unknown";
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public Size GetScreenSize()
		{
			Size result = new Size(WindowsAPI.GetSystemMetrics(0), WindowsAPI.GetSystemMetrics(1));
			return result;
		}
		public void NotifyIcon(IntPtr hwnd, IntPtr icon, string tip)
		{
			NOTIFYICONDATA nOTIFYICONDATA = default(NOTIFYICONDATA);
			nOTIFYICONDATA.cbSize = Marshal.SizeOf(nOTIFYICONDATA);
			nOTIFYICONDATA.hWnd = hwnd;
			nOTIFYICONDATA.hIcon = icon;
			nOTIFYICONDATA.uID = 100u;
			nOTIFYICONDATA.uFlags = 63u;
			nOTIFYICONDATA.szTip = tip;
			nOTIFYICONDATA.dwState = 2;
			nOTIFYICONDATA.dwStateMask = 2;
			nOTIFYICONDATA.dwInfoFlags = 1;
			WindowsAPI.Shell_NotifyIcon(0, ref nOTIFYICONDATA);
		}
		public void WindowMove(IntPtr hwnd, int x, int y)
		{
			Size windowSize = this.GetWindowSize(hwnd);
			WindowsAPI.MoveWindow(hwnd, x, y, windowSize.Width, windowSize.Height, true);
		}
		public void WindowMove(IntPtr hwnd, Point p)
		{
			Size windowSize = this.GetWindowSize(hwnd);
			WindowsAPI.MoveWindow(hwnd, p.X, p.Y, windowSize.Width, windowSize.Height, true);
		}
		public IntPtr GetCurrentIntPtr()
		{
			Point point = default(Point);
			WindowsAPI.GetCursorPos(ref point);
			return WindowsAPI.WindowFromPoint(point);
		}
		public Point GetCurrentPos()
		{
			Point result = default(Point);
			WindowsAPI.GetCursorPos(ref result);
			return result;
		}
		public Color GetColor(IntPtr hwnd)
		{
			Point currentPos = this.GetCurrentPos();
			IntPtr dC = WindowsAPI.GetDC(hwnd);
			int pixel = WindowsAPI.GetPixel(dC, currentPos.X, currentPos.Y);
			return Color.FromArgb(pixel & 255, (pixel & 65280) >> 8, (pixel & 16711680) >> 16);
		}
		public string HColor(Color color)
		{
			return "#" + Convert.ToInt32(color.R).ToString("X").PadLeft(2, '0') + Convert.ToInt32(color.G).ToString("X").PadLeft(2, '0') + Convert.ToInt32(color.B).ToString("X").PadLeft(2, '0');
		}
		public Color GetColor(Point P)
		{
			IntPtr hdc = WindowsAPI.CreateDC("DISPLAY", null, null, IntPtr.Zero);
			Graphics graphics = Graphics.FromHdc(hdc);
			Bitmap bitmap = new Bitmap(1, 1, graphics);
			Graphics graphics2 = Graphics.FromImage(bitmap);
			IntPtr hdc2 = graphics.GetHdc();
			IntPtr hdc3 = graphics2.GetHdc();
			WindowsAPI.BitBlt(hdc3, 0, 0, 1, 1, hdc2, P.X, P.Y, 13369376u);
			graphics.ReleaseHdc(hdc2);
			graphics2.ReleaseHdc(hdc3);
			Color pixel = bitmap.GetPixel(0, 0);
			graphics.Dispose();
			graphics2.Dispose();
			bitmap.Dispose();
			return pixel;
		}
		public void AnimateWindowOpen(IntPtr hwnd)
		{
			WindowsAPI.AnimateWindow(hwnd, 3000, 655360);
		}
		public void AnimateWindowOpen(IntPtr hwnd, int time)
		{
			WindowsAPI.AnimateWindow(hwnd, time, 655360);
		}
		public void AnimateWindowClose(IntPtr hwnd)
		{
			WindowsAPI.AnimateWindow(hwnd, 3000, 65538);
		}
		public void AnimateWindowClose(IntPtr hwnd, int time)
		{
			WindowsAPI.AnimateWindow(hwnd, time, 65538);
		}
		public void SetWallpaperA(string path)
		{
			if (File.Exists(path))
			{
				RegeditManageMent regeditManageMent = new RegeditManageMent();
				regeditManageMent.RegeditKey = Registry.CurrentUser;
				regeditManageMent.RegeditPath = "Control Panel\\desktop";
				regeditManageMent.SetStringValue("TileWallpaper", "0");
				regeditManageMent.SetStringValue("WallpaperStyle", "0");
				WindowsAPI.SystemParametersInfo(20u, 0u, path, 1u);
			}
		}
		public void SetWallpaperB(string path)
		{
			if (File.Exists(path))
			{
				RegeditManageMent regeditManageMent = new RegeditManageMent();
				regeditManageMent.RegeditKey = Registry.CurrentUser;
				regeditManageMent.RegeditPath = "Control Panel\\desktop";
				regeditManageMent.SetStringValue("TileWallpaper", "1");
				regeditManageMent.SetStringValue("WallpaperStyle", "0");
				WindowsAPI.SystemParametersInfo(20u, 0u, path, 1u);
			}
		}
		public void SetWallpaperC(string path)
		{
			if (File.Exists(path))
			{
				RegeditManageMent regeditManageMent = new RegeditManageMent();
				regeditManageMent.RegeditKey = Registry.CurrentUser;
				regeditManageMent.RegeditPath = "Control Panel\\desktop";
				regeditManageMent.SetStringValue("TileWallpaper", "0");
				regeditManageMent.SetStringValue("WallpaperStyle", "2");
				WindowsAPI.SystemParametersInfo(20u, 0u, path, 1u);
			}
		}
		public Point CenterPoint(IntPtr hwnd)
		{
			Size windowSize = this.GetWindowSize(hwnd);
			Size screenSize = this.GetScreenSize();
			Point result = new Point((screenSize.Width - windowSize.Width) / 2, (screenSize.Height - windowSize.Height) / 2 - 14);
			return result;
		}
		public Point CenterPoint(Size size)
		{
			Size screenSize = this.GetScreenSize();
			Point result = new Point((screenSize.Width - size.Width) / 2, (screenSize.Height - size.Height) / 2 - 14);
			return result;
		}
		public RECT GetRect(IntPtr hwnd)
		{
			RECT result = default(RECT);
			WindowsAPI.GetWindowRect(hwnd, ref result);
			return result;
		}
		public void ArrangeWindowsA(IntPtr hwnd)
		{
			RECT rect = this.GetRect(hwnd);
			uint cKids = (uint)this.GetAllChildControls(hwnd).Length;
			WindowsAPI.CascadeWindows(IntPtr.Zero, 2u, ref rect, cKids, IntPtr.Zero);
		}
		public void ArrangeWindowsB(IntPtr hwnd)
		{
			RECT rect = this.GetRect(hwnd);
			uint cKids = (uint)this.GetAllChildControls(hwnd).Length;
			WindowsAPI.CascadeWindows(IntPtr.Zero, 4u, ref rect, cKids, IntPtr.Zero);
		}
		public void ArrangeWindowsC(IntPtr hwnd)
		{
			RECT rect = this.GetRect(hwnd);
			uint cKids = (uint)this.GetAllChildControls(hwnd).Length;
			WindowsAPI.TileWindows(IntPtr.Zero, 1u, ref rect, cKids, IntPtr.Zero);
		}
		public void ArrangeWindowsD(IntPtr hwnd)
		{
			RECT rect = this.GetRect(hwnd);
			uint cKids = (uint)this.GetAllChildControls(hwnd).Length;
			WindowsAPI.TileWindows(IntPtr.Zero, 0u, ref rect, cKids, IntPtr.Zero);
		}
		public int GetMinimizeCount(IntPtr hwnd)
		{
			uint value = WindowsAPI.ArrangeIconicWindows(this.DeskHwnd);
			return Convert.ToInt32(value);
		}
		public void CreateWindowEx(IntPtr hwnd)
		{
			WindowsAPI.CreateWindowEx(0, "button", "API", 1342177280, 320, 10, 75, 23, hwnd, IntPtr.Zero, WindowsAPI.GetModuleHandle(this.GetExecutePath(hwnd)), IntPtr.Zero);
		}
		public void ControlSound()
		{
			WindowsAPI.waveOutSetVolume(IntPtr.Zero, (long)-1);
			long num;
			WindowsAPI.waveOutGetVolume(IntPtr.Zero, out num);
			string text = string.Format("{0:X}", num);
			text = text.PadLeft(8, '0');
			int num2 = int.Parse(text.Substring(0, 4), NumberStyles.HexNumber);
			if (num2 < 65535)
			{
				text = (num2 + 655).ToString();
				text.PadLeft(4, '0');
				text += text;
				WindowsAPI.waveOutSetVolume(IntPtr.Zero, long.Parse(text, NumberStyles.HexNumber));
			}
		}
		public void WinExec(ControlPanel cp)
		{
			switch (cp)
			{
			case ControlPanel.辅助功能选项:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL access.cpl", 5u);
				break;
			case ControlPanel.添加或删除程序:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL appwiz.cpl", 5u);
				break;
			case ControlPanel.显示属性:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL desk.cpl", 5u);
				break;
			case ControlPanel.Windows防火墙:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL firewall.cpl", 5u);
				break;
			case ControlPanel.添加硬件向导:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL hdwwiz.cpl", 5u);
				break;
			case ControlPanel.Internet属性:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL inetcpl.cpl", 5u);
				break;
			case ControlPanel.区域和语言选项:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL intl.cpl", 5u);
				break;
			case ControlPanel.游戏控制器:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL joy.cpl", 5u);
				break;
			case ControlPanel.鼠标属性:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL main.cpl", 5u);
				break;
			case ControlPanel.声音和音频设备属性:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL mmsys.cpl", 5u);
				break;
			case ControlPanel.网络连接:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL ncpa.cpl", 5u);
				break;
			case ControlPanel.网络安装向导:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL netsetup.cpl", 5u);
				break;
			case ControlPanel.用户账户:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL nusrmgr.cpl", 5u);
				break;
			case ControlPanel.ODBC数据元管理器:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL odbccp32.cpl", 5u);
				break;
			case ControlPanel.电源选项:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL powercfg.cpl", 5u);
				break;
			case ControlPanel.系统属性:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL sysdm.cpl", 5u);
				break;
			case ControlPanel.位置信息:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL telephon.cpl", 5u);
				break;
			case ControlPanel.日期和时间属性:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL timedate.cpl", 5u);
				break;
			case ControlPanel.Windows安全中心:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL wscui.cpl", 5u);
				break;
			case ControlPanel.自动更新:
				WindowsAPI.WinExec("rundll32.exe shell32.dll,Control_RunDLL wuaucpl.cpl", 5u);
				break;
			}
		}
		public void ShellExecute(ControlPanel cp)
		{
			SHELLEXECUTEINFO sHELLEXECUTEINFO = default(SHELLEXECUTEINFO);
			sHELLEXECUTEINFO.cbSize = Marshal.SizeOf(sHELLEXECUTEINFO);
			sHELLEXECUTEINFO.lpFile = "rundll32.exe";
			switch (cp)
			{
			case ControlPanel.辅助功能选项:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL access.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.添加或删除程序:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL appwiz.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.显示属性:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL desk.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.Windows防火墙:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL firewall.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.添加硬件向导:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL hdwwiz.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.Internet属性:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL inetcpl.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.区域和语言选项:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL intl.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.游戏控制器:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL joy.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.鼠标属性:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL main.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.声音和音频设备属性:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL mmsys.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.网络连接:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL ncpa.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.网络安装向导:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL netsetup.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.用户账户:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL nusrmgr.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.ODBC数据元管理器:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL odbccp32.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.电源选项:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL powercfg.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.系统属性:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL sysdm.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.位置信息:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL telephon.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.日期和时间属性:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL timedate.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.Windows安全中心:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL wscui.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			case ControlPanel.自动更新:
				sHELLEXECUTEINFO.lpParameters = "shell32.dll,Control_RunDLL wuaucpl.cpl";
				WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
				break;
			}
		}
		public IntPtr RunProgram(string exename, string dir)
		{
			SHELLEXECUTEINFO sHELLEXECUTEINFO = default(SHELLEXECUTEINFO);
			sHELLEXECUTEINFO.cbSize = Marshal.SizeOf(sHELLEXECUTEINFO);
			sHELLEXECUTEINFO.fMask = 12;
			sHELLEXECUTEINFO.dwHotKey = 0;
			sHELLEXECUTEINFO.lpClass = null;
			sHELLEXECUTEINFO.lpDirectory = dir;
			sHELLEXECUTEINFO.lpFile = exename;
			sHELLEXECUTEINFO.lpIDList = new IntPtr(0);
			sHELLEXECUTEINFO.lpParameters = null;
			sHELLEXECUTEINFO.lpVerb = "open";
			sHELLEXECUTEINFO.nShow = 5;
			WindowsAPI.ShellExecuteEx(ref sHELLEXECUTEINFO);
			return sHELLEXECUTEINFO.hwnd;
		}
		public bool SwapMouseButton()
		{
			return WindowsAPI.GetSystemMetrics(23) != 0;
		}
		public IntPtr FirstChildIntPtr(IntPtr hwnd)
		{
			return WindowsAPI.GetWindow(hwnd, 5u);
		}
		public IntPtr GetTaskBar()
		{
			IntPtr desktopWindow = WindowsAPI.GetDesktopWindow();
			IntPtr hwndParent = WindowsAPI.FindWindowEx(desktopWindow, IntPtr.Zero, "Shell_TrayWnd", null);
			IntPtr hwndParent2 = WindowsAPI.FindWindowEx(hwndParent, IntPtr.Zero, "ReBarWindow32", null);
			IntPtr hwndParent3 = WindowsAPI.FindWindowEx(hwndParent2, IntPtr.Zero, "MSTaskSwWClass", null);
			return WindowsAPI.FindWindowEx(hwndParent3, IntPtr.Zero, "ToolbarWindow32", null);
		}
		public void EmptyRecycleBin()
		{
			WindowsAPI.SHEmptyRecycleBin(IntPtr.Zero, null, 0);
		}
		public WindowInfo[] GetAllDesktopWindows()
		{
			List<WindowInfo> wndList = new List<WindowInfo>();
			WindowsAPI.EnumWindows(delegate(IntPtr hWnd, int lParam)
			{
				WindowInfo item = default(WindowInfo);
				StringBuilder stringBuilder = new StringBuilder(512);
				item.hWnd = hWnd;
				WindowsAPI.GetWindowTextW(hWnd, stringBuilder, stringBuilder.Capacity);
				item.szWindowName = stringBuilder.ToString();
				WindowsAPI.GetClassNameW(hWnd, stringBuilder, stringBuilder.Capacity);
				item.szClassName = stringBuilder.ToString();
				wndList.Add(item);
				return true;
			}, 0);
			return wndList.ToArray();
		}
		public WINDOWINFO[] GetAllDesktopWindowsEx()
		{
			List<WINDOWINFO> wndList = new List<WINDOWINFO>();
			WindowsAPI.EnumWindows(delegate(IntPtr hWnd, int lParam)
			{
				WINDOWINFO wINDOWINFO = default(WINDOWINFO);
				wINDOWINFO.cbSize = Marshal.SizeOf(wINDOWINFO);
				StringBuilder stringBuilder = new StringBuilder(512);
				WindowsAPI.GetWindowInfo(hWnd, ref wINDOWINFO);
				wINDOWINFO.hWnd = hWnd;
				WindowsAPI.GetWindowTextW(hWnd, stringBuilder, stringBuilder.Capacity);
				wINDOWINFO.szWindowName = stringBuilder.ToString();
				WindowsAPI.GetClassNameW(hWnd, stringBuilder, stringBuilder.Capacity);
				wINDOWINFO.szClassName = stringBuilder.ToString();
				wINDOWINFO.szExePath = this.GetExecutePath(wINDOWINFO.hWnd);
				wndList.Add(wINDOWINFO);
				return true;
			}, 0);
			return wndList.ToArray();
		}
		public IntPtr[] GetAllDesktopWindowsHandle()
		{
			List<IntPtr> wndList = new List<IntPtr>();
			WindowsAPI.EnumWindows(delegate(IntPtr hWnd, int lParam)
			{
				wndList.Add(hWnd);
				return true;
			}, 0);
			return wndList.ToArray();
		}
		public WindowInfo[] GetAllChildControls(IntPtr phwnd)
		{
			WindowsAPI.ChildWindowsProc lpEnumFunc = new WindowsAPI.ChildWindowsProc(this.EnumWinowsChildPro);
			WindowsAPI.EnumChildWindows(phwnd, lpEnumFunc, 0);
			return this.childList.ToArray();
		}
		public WindowInfo[] GetAllChildControlsW(IntPtr phwnd)
		{
			List<WindowInfo> child = new List<WindowInfo>();
			WindowsAPI.EnumChildWindows(phwnd, delegate(IntPtr hWnd, int lParam)
			{
				WindowInfo item = default(WindowInfo);
				StringBuilder stringBuilder = new StringBuilder(512);
				item.hWnd = hWnd;
				WindowsAPI.GetWindowTextW(hWnd, stringBuilder, stringBuilder.Capacity);
				item.szWindowName = stringBuilder.ToString();
				WindowsAPI.GetClassNameW(hWnd, stringBuilder, stringBuilder.Capacity);
				item.szClassName = stringBuilder.ToString();
				child.Add(item);
				return true;
			}, 0);
			return child.ToArray();
		}
		public WINDOWINFO[] GetAllChildControlsEx(IntPtr phwnd)
		{
			List<WINDOWINFO> child = new List<WINDOWINFO>();
			WindowsAPI.EnumChildWindows(phwnd, delegate(IntPtr hWnd, int lParam)
			{
				WINDOWINFO wINDOWINFO = default(WINDOWINFO);
				wINDOWINFO.cbSize = Marshal.SizeOf(wINDOWINFO);
				StringBuilder stringBuilder = new StringBuilder(512);
				WindowsAPI.GetWindowInfo(hWnd, ref wINDOWINFO);
				wINDOWINFO.hWnd = hWnd;
				WindowsAPI.GetWindowTextW(hWnd, stringBuilder, stringBuilder.Capacity);
				wINDOWINFO.szWindowName = stringBuilder.ToString();
				WindowsAPI.GetClassNameW(hWnd, stringBuilder, stringBuilder.Capacity);
				wINDOWINFO.szClassName = stringBuilder.ToString();
				wINDOWINFO.szExePath = this.GetExecutePath(wINDOWINFO.hWnd);
				child.Add(wINDOWINFO);
				return true;
			}, 0);
			return child.ToArray();
		}
		public IntPtr[] GetAllChildControlsHandle(IntPtr phwnd)
		{
			List<IntPtr> child = new List<IntPtr>();
			WindowsAPI.EnumChildWindows(phwnd, delegate(IntPtr hWnd, int lParam)
			{
				child.Add(hWnd);
				return true;
			}, 0);
			return child.ToArray();
		}
		private bool EnumWinowsChildPro(IntPtr hWnd, int lParam)
		{
			WindowInfo item = default(WindowInfo);
			StringBuilder stringBuilder = new StringBuilder(512);
			item.hWnd = hWnd;
			WindowsAPI.GetWindowTextW(hWnd, stringBuilder, stringBuilder.Capacity);
			item.szWindowName = stringBuilder.ToString();
			WindowsAPI.GetClassNameW(hWnd, stringBuilder, stringBuilder.Capacity);
			item.szClassName = stringBuilder.ToString();
			this.childList.Add(item);
			return true;
		}
		public int[] GetAllProcesses(IntPtr handle)
		{
			int num = 0;
			int[] array = new int[256];
			WindowsAPI.EnumProcesses(array, 256, ref num);
			ArrayList arrayList = new ArrayList(array);
			while (arrayList.IndexOf(0) != -1)
			{
				arrayList.Remove(0);
			}
			Array array2 = arrayList.ToArray();
			array = new int[array2.Length];
			array2.CopyTo(array, 0);
			return array;
		}
		public IntPtr[] GetAllProcessModules(IntPtr handle)
		{
			int iDProcess = 0;
			IntPtr[] array = new IntPtr[256];
			WindowsAPI.GetWindowThreadProcessId(handle, ref iDProcess);
			IntPtr hProcess = WindowsAPI.OpenProcess(1040, false, iDProcess);
			WindowsAPI.EnumProcessModules(hProcess, array, 256, ref iDProcess);
			ArrayList arrayList = new ArrayList(array);
			while (arrayList.IndexOf(IntPtr.Zero) != -1)
			{
				arrayList.Remove(IntPtr.Zero);
			}
			Array array2 = arrayList.ToArray();
			array = new IntPtr[array2.Length];
			array2.CopyTo(array, 0);
			return array;
		}
		public void APIProcess(string path)
		{
			STARTUPINFO sTARTUPINFO = default(STARTUPINFO);
			PROCESS_INFORMATION pROCESS_INFORMATION = default(PROCESS_INFORMATION);
			if (!WindowsAPI.CreateProcess(null, new StringBuilder(path), null, null, false, 0, null, null, ref sTARTUPINFO, ref pROCESS_INFORMATION))
			{
				throw new Exception("调用失败");
			}
			int uExitCode = 0;
			WindowsAPI.GetExitCodeProcess(pROCESS_INFORMATION.hProcess, ref uExitCode);
			WindowsAPI.TerminateProcess(pROCESS_INFORMATION.hProcess, uExitCode);
			WindowsAPI.CloseHandle(pROCESS_INFORMATION.hProcess);
			WindowsAPI.CloseHandle(pROCESS_INFORMATION.hThread);
		}
		public DateTime GetPorcessCreationTime()
		{
			IntPtr intPtr = this.GetCurrentIntPtr();
			int iDProcess = 0;
			WindowsAPI.GetWindowThreadProcessId(intPtr, ref iDProcess);
			intPtr = WindowsAPI.OpenProcess(1040, false, iDProcess);
			_FILETIME time = default(_FILETIME);
			_FILETIME fILETIME = default(_FILETIME);
			_FILETIME fILETIME2 = default(_FILETIME);
			_FILETIME fILETIME3 = default(_FILETIME);
			WindowsAPI.GetProcessTimes(intPtr, ref time, ref fILETIME, ref fILETIME2, ref fILETIME3);
			return this.FileTimeToWindowsTimeEx(time);
		}
		public DateTime GetPorcessCreationTime(IntPtr hwnd)
		{
			int iDProcess = 0;
			WindowsAPI.GetWindowThreadProcessId(hwnd, ref iDProcess);
			hwnd = WindowsAPI.OpenProcess(1040, false, iDProcess);
			_FILETIME time = default(_FILETIME);
			_FILETIME fILETIME = default(_FILETIME);
			_FILETIME fILETIME2 = default(_FILETIME);
			_FILETIME fILETIME3 = default(_FILETIME);
			WindowsAPI.GetProcessTimes(hwnd, ref time, ref fILETIME, ref fILETIME2, ref fILETIME3);
			return this.FileTimeToWindowsTimeEx(time);
		}
		public string GetProcessRunTime(IntPtr hwnd)
		{
			DateTime porcessCreationTime = this.GetPorcessCreationTime(hwnd);
			int num = DateTime.Now.Hour - porcessCreationTime.Hour;
			int num2 = DateTime.Now.Minute - porcessCreationTime.Minute;
			int num3 = DateTime.Now.Second - porcessCreationTime.Second;
			int num4 = num * 60 * 60 * 1000 + num2 * 60 * 1000 + num3 * 1000;
			num = num4 / 1000 / 60 / 60;
			num %= 24;
			num2 = num4 / 1000 / 60;
			num2 %= 60;
			num3 = num4 / 1000;
			num3 %= 60;
			return string.Concat(new string[]
			{
				num.ToString().PadLeft(2, '0'),
				"小时",
				num2.ToString().PadLeft(2, '0'),
				"分",
				num3.ToString().PadLeft(2, '0'),
				"秒"
			});
		}
		public WINDOWINFO GetWindowInfo(IntPtr hwnd)
		{
			WINDOWINFO wINDOWINFO = default(WINDOWINFO);
			wINDOWINFO.cbSize = Marshal.SizeOf(wINDOWINFO);
			WindowsAPI.GetWindowInfo(hwnd, ref wINDOWINFO);
			return wINDOWINFO;
		}
		public int GetWindowBorder(IntPtr hwnd)
		{
			WINDOWINFO wINDOWINFO = default(WINDOWINFO);
			wINDOWINFO.cbSize = Marshal.SizeOf(wINDOWINFO);
			WindowsAPI.GetWindowInfo(hwnd, ref wINDOWINFO);
			return Convert.ToInt32(wINDOWINFO.cxWindowBorders);
		}
		public int GetWindowThreadProcessId(IntPtr hwnd)
		{
			int result = 0;
			WindowsAPI.GetWindowThreadProcessId(hwnd, ref result);
			return result;
		}
		public ProcessInfo ProcessInfo(IntPtr hwnd)
		{
			ProcessInfo result = default(ProcessInfo);
			WINDOWINFO windowInfo = this.GetWindowInfo(hwnd);
			result.hwnd = hwnd;
			result.csize = this.GetWindowClientSize(hwnd);
			result.location = this.GetLocation(hwnd);
			result.wsize = this.GetWindowSize(hwnd);
			result.WindowText = this.GetWindowText(hwnd);
			result.ClassName = this.GetClassName(hwnd);
			result.cxWindowBorders = windowInfo.cxWindowBorders;
			result.cyWindowBorders = windowInfo.cyWindowBorders;
			result.dwExStyle = windowInfo.dwExStyle;
			result.dwStyle = windowInfo.dwStyle;
			result.id = this.GetWindowThreadProcessId(hwnd);
			result.text = this.GetControlText(hwnd);
			result.starttime = this.GetPorcessCreationTime(hwnd);
			result.runtime = this.GetProcessRunTime(hwnd);
			result.path = this.GetExecutePath(hwnd);
			result.processsize = this.GetFileSize(result.path);
			result.phwnd = WindowsAPI.GetParent(hwnd);
			return result;
		}
		public void RestrictCursor(IntPtr handle)
		{
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(handle, ref rECT);
			WindowsAPI.ClipCursor(ref rECT);
		}
		public void ReleaseCursor()
		{
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(WindowsAPI.GetDesktopWindow(), ref rECT);
			WindowsAPI.ClipCursor(ref rECT);
		}
		public void IsShowCursor(bool bshow)
		{
			WindowsAPI.ShowCursor(bshow);
		}
		public CPUInformation CPUInfo()
		{
			CPUInformation result = default(CPUInformation);
			result.core = 0u;
			result.level2 = 0u;
			result.type = "";
			try
			{
				SYSTEM_INFO sYSTEM_INFO = default(SYSTEM_INFO);
				WindowsAPI.GetSystemInfo(ref sYSTEM_INFO);
				result.core = sYSTEM_INFO.dwNumberOfProcessors;
				result.level2 = sYSTEM_INFO.dwPageSize;
				decimal d = sYSTEM_INFO.lpMaximumApplicationAddress / 1024u / 1024u;
				result.masterfrequency = (uint)Math.Round(d, 0);
				uint dwProcessorType = sYSTEM_INFO.dwProcessorType;
				if (dwProcessorType <= 486u)
				{
					if (dwProcessorType == 386u)
					{
						result.type = "Intel 386";
						goto IL_115;
					}
					if (dwProcessorType == 486u)
					{
						result.type = "Intel 486";
						goto IL_115;
					}
				}
				else
				{
					if (dwProcessorType == 586u)
					{
						result.type = "Intel Pentium";
						goto IL_115;
					}
					if (dwProcessorType == 4000u)
					{
						result.type = "MIPS R4000";
						goto IL_115;
					}
					if (dwProcessorType == 21064u)
					{
						result.type = "DEC Alpha 21064";
						goto IL_115;
					}
				}
				result.type = "(unknown)";
				IL_115:;
			}
			catch
			{
			}
			return result;
		}
		public MemoryInformation MemoryInfo()
		{
			MEMORYSTATUS mEMORYSTATUS = default(MEMORYSTATUS);
			WindowsAPI.GlobalMemoryStatus(ref mEMORYSTATUS);
			return new MemoryInformation
			{
				AvailablePageFile = mEMORYSTATUS.dwAvailPageFile / 1024u / 1024u,
				AvailablePhysicalMemory = mEMORYSTATUS.dwAvailPhys / 1024u / 1024u,
				AvailableVirtualMemory = mEMORYSTATUS.dwAvailVirtual / 1024u / 1024u,
				SizeofStructure = mEMORYSTATUS.dwLength,
				MemoryInUse = mEMORYSTATUS.dwMemoryLoad,
				TotalPageSize = mEMORYSTATUS.dwTotalPageFile / 1024u / 1024u,
				TotalPhysicalMemory = mEMORYSTATUS.dwTotalPhys / 1024u / 1024u,
				TotalVirtualMemory = mEMORYSTATUS.dwTotalVirtual / 1024u / 1024u
			};
		}
		public void DynamicDebug()
		{
			IntPtr intPtr = WindowsAPI.LoadLibrary("Kernel32.dll");
			IntPtr procAddress = WindowsAPI.GetProcAddress(intPtr, "Beep");
			APIMethod.Delegate_Beep delegate_Beep = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(APIMethod.Delegate_Beep)) as APIMethod.Delegate_Beep;
			delegate_Beep(100u, 100u);
			WindowsAPI.FreeLibrary(intPtr);
		}
		public float GetCharWidth(IntPtr handle, char char1, char char2)
		{
			float result = 0f;
			int num = 0;
			IntPtr dC = WindowsAPI.GetDC(handle);
			bool flag = WindowsAPI.GetCharWidthFloatA(dC, (uint)char1, (uint)char2, ref result);
			flag = WindowsAPI.GetCharWidth32A(dC, (uint)char1, (uint)char2, ref num);
			return result;
		}
		public DISPLAY_DEVICE EnumDisplayDevices()
		{
			DISPLAY_DEVICE dISPLAY_DEVICE = default(DISPLAY_DEVICE);
			dISPLAY_DEVICE.cb = Marshal.SizeOf(dISPLAY_DEVICE);
			WindowsAPI.EnumDisplayDevices(null, 0, ref dISPLAY_DEVICE, 1);
			return dISPLAY_DEVICE;
		}
		public string DisplayDeviceName()
		{
			DISPLAY_DEVICE dISPLAY_DEVICE = default(DISPLAY_DEVICE);
			dISPLAY_DEVICE.cb = Marshal.SizeOf(dISPLAY_DEVICE);
			WindowsAPI.EnumDisplayDevices(null, 0, ref dISPLAY_DEVICE, 1);
			return dISPLAY_DEVICE.DeviceName;
		}
		public DateTime FileTimeToWindowsTime(_FILETIME time)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(_FILETIME)));
			IntPtr intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SYSTEMTIME)));
			Marshal.StructureToPtr(time, intPtr, true);
			WindowsAPI.FileTimeToSystemTime(intPtr, intPtr2);
			SYSTEMTIME sYSTEMTIME = (SYSTEMTIME)Marshal.PtrToStructure(intPtr2, typeof(SYSTEMTIME));
			string s = string.Concat(new string[]
			{
				sYSTEMTIME.wYear.ToString(),
				"/",
				sYSTEMTIME.wMonth.ToString(),
				"/",
				sYSTEMTIME.wDay.ToString(),
				" ",
				sYSTEMTIME.wHour.ToString(),
				":",
				sYSTEMTIME.wMinute.ToString(),
				":",
				sYSTEMTIME.wSecond.ToString()
			});
			return DateTime.Parse(s);
		}
		public DateTime FileTimeToWindowsTimeEx(_FILETIME time)
		{
			_FILETIME fILETIME = default(_FILETIME);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(_FILETIME)));
			IntPtr intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SYSTEMTIME)));
			WindowsAPI.FileTimeToLocalFileTime(ref time, out fILETIME);
			time = fILETIME;
			Marshal.StructureToPtr(time, intPtr, true);
			WindowsAPI.FileTimeToSystemTime(intPtr, intPtr2);
			SYSTEMTIME sYSTEMTIME = (SYSTEMTIME)Marshal.PtrToStructure(intPtr2, typeof(SYSTEMTIME));
			string s = string.Concat(new string[]
			{
				sYSTEMTIME.wYear.ToString(),
				"/",
				sYSTEMTIME.wMonth.ToString(),
				"/",
				sYSTEMTIME.wDay.ToString(),
				" ",
				sYSTEMTIME.wHour.ToString(),
				":",
				sYSTEMTIME.wMinute.ToString(),
				":",
				sYSTEMTIME.wSecond.ToString()
			});
			return DateTime.Parse(s);
		}
		public _FILETIME WindowsTimeToFileTime(DateTime dt)
		{
			_FILETIME result = default(_FILETIME);
			_FILETIME fILETIME = default(_FILETIME);
			SYSTEMTIME sYSTEMTIME = default(SYSTEMTIME);
			sYSTEMTIME.wDay = (ushort)dt.Day;
			sYSTEMTIME.wDayOfWeek = (ushort)dt.DayOfWeek;
			sYSTEMTIME.wHour = (ushort)dt.Hour;
			sYSTEMTIME.wMilliseconds = (ushort)dt.Millisecond;
			sYSTEMTIME.wMinute = (ushort)dt.Minute;
			sYSTEMTIME.wMonth = (ushort)dt.Month;
			sYSTEMTIME.wSecond = (ushort)dt.Second;
			sYSTEMTIME.wYear = (ushort)dt.Year;
			WindowsAPI.SystemTimeToFileTime(ref sYSTEMTIME, out fILETIME);
			WindowsAPI.LocalFileTimeToFileTime(ref fILETIME, out result);
			return result;
		}
		public SYSTEMTIME WindowsTimeToSystemTime(DateTime dt)
		{
			return new SYSTEMTIME
			{
				wDay = (ushort)dt.Day,
				wDayOfWeek = (ushort)dt.DayOfWeek,
				wHour = (ushort)dt.Hour,
				wMilliseconds = (ushort)dt.Millisecond,
				wMinute = (ushort)dt.Minute,
				wMonth = (ushort)dt.Month,
				wSecond = (ushort)dt.Second,
				wYear = (ushort)dt.Year
			};
		}
		public DateTime SystemTimeToWindowsTime(SYSTEMTIME st)
		{
			return DateTime.Parse(string.Concat(new string[]
			{
				st.wYear.ToString(),
				"/",
				st.wMonth.ToString(),
				"/",
				st.wDay.ToString(),
				" ",
				st.wHour.ToString(),
				":",
				st.wMinute.ToString(),
				":",
				st.wSecond.ToString()
			}));
		}
		public string GetRunTime()
		{
			int tickCount = WindowsAPI.GetTickCount();
			int num = tickCount / 1000 / 60 / 60 / 24;
			int num2 = tickCount / 1000 / 60 / 60;
			num2 %= 24;
			int num3 = tickCount / 1000 / 60;
			num3 %= 60;
			int num4 = tickCount / 1000;
			num4 %= 60;
			return string.Concat(new string[]
			{
				num.ToString().PadLeft(2, '0'),
				"天",
				num2.ToString().PadLeft(2, '0'),
				"小时",
				num3.ToString().PadLeft(2, '0'),
				"分",
				num4.ToString().PadLeft(2, '0'),
				"秒"
			});
		}
		public IntPtr GetFileHandleOpenFile(string path)
		{
			path = Path.GetFullPath(path);
			OFSTRUCT oFSTRUCT = default(OFSTRUCT);
			oFSTRUCT.cBytes = (byte)Marshal.SizeOf(oFSTRUCT);
			return WindowsAPI.OpenFile(path, ref oFSTRUCT, 2u);
		}
		public IntPtr GetFileHandleOpenFileEx(string path)
		{
			path = Path.GetFullPath(path);
			OFSTRUCT oFSTRUCT = default(OFSTRUCT);
			oFSTRUCT.cBytes = (byte)Marshal.SizeOf(oFSTRUCT);
			return WindowsAPI.OpenFile(path, ref oFSTRUCT, 1073741824u);
		}
		public IntPtr GetFileHandleCreateFile(string path)
		{
			path = Path.GetFullPath(path);
			IntPtr intPtr = WindowsAPI.CreateFile(path, 1u, 3, 0, 4, 128u, 0);
			WindowsAPI.CloseHandle(intPtr);
			return intPtr;
		}
		public int RenameFile(string path, string newname)
		{
			path = Path.GetFullPath(path);
			string fullPath = Path.GetFullPath(newname);
			if (Path.GetDirectoryName(path) != Path.GetDirectoryName(fullPath))
			{
				newname = Path.GetDirectoryName(path) + "\\" + Path.GetFileName(newname);
			}
			return WindowsAPI.SHFileOperation(new _SHFILEOPSTRUCT
			{
				wFunc = 4u,
				pFrom = path + '\0',
				pTo = newname,
				fFlags = 80
			});
		}
		public bool SetWindowsTime(DateTime dt)
		{
			SYSTEMTIME sYSTEMTIME = default(SYSTEMTIME);
			sYSTEMTIME.wDay = (ushort)dt.Day;
			sYSTEMTIME.wDayOfWeek = (ushort)dt.DayOfWeek;
			sYSTEMTIME.wHour = (ushort)(dt.Hour % 24 - 8);
			sYSTEMTIME.wMilliseconds = (ushort)dt.Millisecond;
			sYSTEMTIME.wMinute = (ushort)dt.Minute;
			sYSTEMTIME.wMonth = (ushort)dt.Month;
			sYSTEMTIME.wSecond = (ushort)dt.Second;
			sYSTEMTIME.wYear = (ushort)dt.Year;
			return WindowsAPI.SetSystemTime(ref sYSTEMTIME);
		}
		public bool SetWindowsTimeEx(DateTime dt)
		{
			SYSTEMTIME sYSTEMTIME = default(SYSTEMTIME);
			sYSTEMTIME.wDay = (ushort)dt.Day;
			sYSTEMTIME.wDayOfWeek = (ushort)dt.DayOfWeek;
			sYSTEMTIME.wHour = (ushort)dt.Hour;
			sYSTEMTIME.wMilliseconds = (ushort)dt.Millisecond;
			sYSTEMTIME.wMinute = (ushort)dt.Minute;
			sYSTEMTIME.wMonth = (ushort)dt.Month;
			sYSTEMTIME.wSecond = (ushort)dt.Second;
			sYSTEMTIME.wYear = (ushort)dt.Year;
			return WindowsAPI.SetLocalTime(ref sYSTEMTIME);
		}
		public void WritePrivateProfileString(string option, object[] caption, object[] text, string path)
		{
			for (int i = 0; i < text.Length; i++)
			{
				WindowsAPI.WritePrivateProfileString(option, caption[i].ToString(), text[i].ToString(), path);
			}
		}
		public void WritePrivateProfileSection(string option, string text, string path)
		{
			WindowsAPI.WritePrivateProfileSection(option, text, path);
		}
		public string[] GetPrivateProfileStrings(string option, object[] caption, string path)
		{
			StringBuilder stringBuilder = new StringBuilder(32767);
			TStringList tStringList = new TStringList();
			for (int i = 0; i < caption.Length; i++)
			{
				WindowsAPI.GetPrivateProfileString(option, caption[i].ToString(), "", stringBuilder, stringBuilder.Capacity, path);
				tStringList.Add(stringBuilder.ToString());
			}
			return tStringList.Strings;
		}
		public string GetPrivateProfileString(string option, object caption, string path)
		{
			StringBuilder stringBuilder = new StringBuilder(32767);
			WindowsAPI.GetPrivateProfileString(option, caption.ToString(), "", stringBuilder, stringBuilder.Capacity, path);
			return stringBuilder.ToString();
		}
		public uint[] GetPrivateProfileInt(string option, object[] caption, string path)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < caption.Length; i++)
			{
				arrayList.Add(WindowsAPI.GetPrivateProfileInt(option, caption[i].ToString(), 0, path));
			}
			int count = arrayList.Count;
			uint[] array = new uint[count];
			Array.Copy(arrayList.ToArray(), array, count);
			return array;
		}
		public string GetPrivateProfileSection(string option, string path)
		{
			StringBuilder stringBuilder = new StringBuilder(32767);
			WindowsAPI.GetPrivateProfileSection(option, stringBuilder, stringBuilder.Capacity, path);
			return stringBuilder.ToString();
		}
		public int GetFileSize(string path)
		{
			int num = 0;
			IntPtr fileHandleOpenFileEx = this.GetFileHandleOpenFileEx(path);
			return WindowsAPI.GetFileSize(fileHandleOpenFileEx, ref num);
		}
		public BY_HANDLE_FILE_INFORMATION GetFileInformationByHandle(string path)
		{
			BY_HANDLE_FILE_INFORMATION result = default(BY_HANDLE_FILE_INFORMATION);
			IntPtr fileHandleOpenFile = this.GetFileHandleOpenFile(path);
			WindowsAPI.GetFileInformationByHandle(fileHandleOpenFile, ref result);
			return result;
		}
		public int GetCompressedFileSize(string path)
		{
			int num = 0;
			return WindowsAPI.GetCompressedFileSize("D:\\Paint.rar", ref num);
		}
		public string GetInternetURL(string path)
		{
			AdditionalMethod.ModifyExpandName(ref path, ".url");
			string result;
			if (File.Exists(path))
			{
				result = this.GetPrivateProfileString("InternetShortcut", "URL", path);
			}
			else
			{
				result = "";
			}
			return result;
		}
		public string[] FileFind(string path, string extension)
		{
			WIN32_FIND_DATA wIN32_FIND_DATA = default(WIN32_FIND_DATA);
			TStringList tStringList = new TStringList();
			string[] strings;
			if (!Directory.Exists(path))
			{
				strings = tStringList.Strings;
			}
			else
			{
				extension = Path.GetExtension(extension);
				path = Path.GetFullPath(path) + "*" + extension;
				IntPtr hndFindFile = WindowsAPI.FindFirstFile(path, ref wIN32_FIND_DATA);
				while (WindowsAPI.FindNextFile(hndFindFile, ref wIN32_FIND_DATA))
				{
					tStringList.Add(wIN32_FIND_DATA.cFileName);
				}
				strings = tStringList.Strings;
			}
			return strings;
		}
		public IntPtr LoadImage(string path)
		{
			return WindowsAPI.LoadImage(IntPtr.Zero, path, 0u, 0, 0, 16u);
		}
		public bool APIOpenFileDialog(IntPtr owner, string title, string initialDir, string filter, bool multibutton)
		{
			filter = filter.Replace("|*", "\0*");
			filter = filter.Replace("|", "\0");
			if (filter.Substring(filter.Length - 1, 1) != "\0")
			{
				filter += "\0";
			}
			OPENFILENAME oPENFILENAME = new OPENFILENAME();
			if (!Directory.Exists(initialDir))
			{
				initialDir = this.GetWindowsDirectory();
			}
			oPENFILENAME.structSize = Marshal.SizeOf(oPENFILENAME);
			oPENFILENAME.filter = filter;
			if (multibutton)
			{
				oPENFILENAME.flags = 524800;
			}
			else
			{
				oPENFILENAME.flags = 524804;
			}
			oPENFILENAME.dlgOwner = owner;
			oPENFILENAME.file = new string(new char[256]);
			oPENFILENAME.maxFile = oPENFILENAME.file.Length;
			oPENFILENAME.fileTitle = new string(new char[64]);
			oPENFILENAME.maxFileTitle = oPENFILENAME.fileTitle.Length;
			oPENFILENAME.initialDir = initialDir;
			oPENFILENAME.title = title;
			oPENFILENAME.defExt = "txt";
			return WindowsAPI.GetOpenFileName(oPENFILENAME);
		}
		public bool APISaveFileDialog(IntPtr owner, string title, string initialDir, string filter)
		{
			filter = filter.Replace("|*", "\0*");
			filter = filter.Replace("|", "\0");
			if (filter.Substring(filter.Length - 1, 1) != "\0")
			{
				filter += "\0";
			}
			OPENFILENAME oPENFILENAME = new OPENFILENAME();
			if (!Directory.Exists(initialDir))
			{
				initialDir = this.GetWindowsDirectory();
			}
			oPENFILENAME.structSize = Marshal.SizeOf(oPENFILENAME);
			oPENFILENAME.filter = filter;
			oPENFILENAME.flags = 524800;
			oPENFILENAME.dlgOwner = owner;
			oPENFILENAME.file = new string(new char[256]);
			oPENFILENAME.maxFile = oPENFILENAME.file.Length;
			oPENFILENAME.fileTitle = new string(new char[64]);
			oPENFILENAME.maxFileTitle = oPENFILENAME.fileTitle.Length;
			oPENFILENAME.initialDir = initialDir;
			oPENFILENAME.title = title;
			oPENFILENAME.defExt = "txt";
			return WindowsAPI.GetSaveFileName(ref oPENFILENAME);
		}
		public void APIFindDialog(IntPtr hwnd)
		{
			FINDREPLACE fINDREPLACE = default(FINDREPLACE);
			fINDREPLACE.lStructSize = Marshal.SizeOf(fINDREPLACE);
			fINDREPLACE.hwndOwner = hwnd;
			fINDREPLACE.Flags = 65537;
			fINDREPLACE.lpstrFindWhat = "";
			fINDREPLACE.wFindWhatLen = 256;
			WindowsAPI.FindText(ref fINDREPLACE);
		}
		public void APIReplaceDialog(IntPtr hwnd)
		{
			FINDREPLACE fINDREPLACE = default(FINDREPLACE);
			fINDREPLACE.lStructSize = Marshal.SizeOf(fINDREPLACE);
			fINDREPLACE.hwndOwner = hwnd;
			fINDREPLACE.Flags = 65536;
			fINDREPLACE.lpstrReplaceWith = "";
			fINDREPLACE.wReplaceWithLen = 80;
			fINDREPLACE.lpstrFindWhat = "";
			fINDREPLACE.wFindWhatLen = 80;
			WindowsAPI.ReplaceText(ref fINDREPLACE);
		}
		public void APIChooseColorDialog(IntPtr hwnd)
		{
			Color black = Color.Black;
			int[] array = new int[]
			{
				(int)Color.Black.A,
				(int)Color.Black.B,
				(int)Color.Black.G
			};
			int a = (int)Color.Black.A;
			CHOOSECOLOR cHOOSECOLOR = new CHOOSECOLOR();
			IntPtr intPtr = Marshal.AllocCoTaskMem(64);
			try
			{
				Marshal.Copy(array, 0, intPtr, 16);
				cHOOSECOLOR.hwndOwner = hwnd;
				cHOOSECOLOR.rgbResult = ColorTranslator.ToWin32(black);
				cHOOSECOLOR.lpCustColors = intPtr;
				int flags = 17;
				cHOOSECOLOR.Flags = flags;
				Marshal.Copy(intPtr, array, 0, 16);
				WindowsAPI.ChooseColor(ref cHOOSECOLOR);
			}
			finally
			{
				Marshal.FreeCoTaskMem(intPtr);
			}
		}
		public void APIChooseFont(IntPtr hwnd)
		{
			CHOOSEFONT cHOOSEFONT = new CHOOSEFONT();
			LOGFONT lOGFONT = new LOGFONT();
			lOGFONT.lfHeight = 9;
			lOGFONT.lfFaceName = "Arial";
			WindowsAPI.CreateFontIndirect(ref lOGFONT);
			cHOOSEFONT.lStructSize = Marshal.SizeOf(cHOOSEFONT);
			cHOOSEFONT.hwndOwner = hwnd;
			cHOOSEFONT.rgbColors = Color.Black.ToArgb();
			cHOOSEFONT.nFontType = 1024;
			cHOOSEFONT.nSizeMin = 10;
			cHOOSEFONT.nSizeMax = 72;
			bool flag = WindowsAPI.ChooseFont(ref cHOOSEFONT);
		}
		public void SetControlReadOnly(IntPtr hwnd, bool only)
		{
			int wParam = 1;
			if (!only)
			{
				wParam = 0;
			}
			WindowsAPI.SendMessage(hwnd, 207u, wParam, 0);
		}
		public void CloseProgram(IntPtr hwnd)
		{
			WindowsAPI.SendMessage(hwnd, 16u, 0, 0);
		}
		public void StopPaint(IntPtr handle)
		{
			WindowsAPI.SendMessage(handle, 11u, 0, 0);
		}
		public void StartPaint(IntPtr handle)
		{
			WindowsAPI.SendMessage(handle, 11u, 1, 0);
		}
		public WINDOWPLACEMENT GetWindowPlacement(IntPtr handle)
		{
			WINDOWPLACEMENT wINDOWPLACEMENT = default(WINDOWPLACEMENT);
			wINDOWPLACEMENT.length = Marshal.SizeOf(wINDOWPLACEMENT);
			wINDOWPLACEMENT.flags = 7;
			WindowsAPI.GetWindowPlacement(handle, ref wINDOWPLACEMENT);
			return wINDOWPLACEMENT;
		}
		public bool SetWindowPlacement(IntPtr handle)
		{
			WINDOWPLACEMENT wINDOWPLACEMENT = default(WINDOWPLACEMENT);
			wINDOWPLACEMENT.length = Marshal.SizeOf(wINDOWPLACEMENT);
			wINDOWPLACEMENT.flags = 7;
			return WindowsAPI.SetWindowPlacement(handle, ref wINDOWPLACEMENT);
		}
		public string GetControlText(IntPtr hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder(99999999);
			WindowsAPI.SendMessage(hwnd, 13u, 999, stringBuilder);
			return stringBuilder.ToString();
		}
		public string GetControlText(string lpWindowName, string lpChildText)
		{
			IntPtr intPtr = WindowsAPI.FindWindow(null, lpWindowName);
			IntPtr hWnd = WindowsAPI.FindWindowEx(intPtr, IntPtr.Zero, null, lpChildText);
			int windowLong = WindowsAPI.GetWindowLong(hWnd, -12);
			IntPtr dlgItem = WindowsAPI.GetDlgItem(intPtr, windowLong);
			StringBuilder stringBuilder = new StringBuilder(99999999);
			WindowsAPI.SendMessage(dlgItem, 13u, 999, stringBuilder);
			return stringBuilder.ToString();
		}
		public int SetControlText(string lpWindowName, string lpChildText, string newtext)
		{
			IntPtr hwndParent = WindowsAPI.FindWindow(null, lpWindowName);
			IntPtr hwnd = WindowsAPI.FindWindowEx(hwndParent, IntPtr.Zero, null, lpChildText);
			return WindowsAPI.SendMessage(hwnd, 12u, 0, newtext);
		}
		public int SetControlText(IntPtr hwnd, string newtext)
		{
			return WindowsAPI.SendMessage(hwnd, 12u, 0, newtext);
		}
		public string GetClassName(IntPtr hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			WindowsAPI.GetClassNameW(hwnd, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}
		public string GetWindowText(IntPtr hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			WindowsAPI.GetWindowTextW(hwnd, stringBuilder, stringBuilder.MaxCapacity);
			return stringBuilder.ToString();
		}
		public string GetExecutePath(IntPtr hwnd)
		{
			int th32ProcessID = 0;
			WindowsAPI.GetWindowThreadProcessId(hwnd, ref th32ProcessID);
			MODULEENTRY32 mODULEENTRY = default(MODULEENTRY32);
			mODULEENTRY.dwSize = Marshal.SizeOf(mODULEENTRY);
			IntPtr hSnapshot = WindowsAPI.CreateToolhelp32Snapshot(8, th32ProcessID);
			WindowsAPI.Module32First(hSnapshot, ref mODULEENTRY);
			return mODULEENTRY.szExePath;
		}
		public string GetExecutePathEx(IntPtr hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			int iDProcess = 0;
			WindowsAPI.GetWindowThreadProcessId(hwnd, ref iDProcess);
			IntPtr hProcess = WindowsAPI.OpenProcess(1040, false, iDProcess);
			WindowsAPI.GetModuleFileNameEx(hProcess, IntPtr.Zero, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}
		private void GetExecutePath_SB(IntPtr hwnd)
		{
			IntPtr[] allProcessModules = this.GetAllProcessModules(hwnd);
			int iDProcess = 0;
			MODULEINFO mODULEINFO = default(MODULEINFO);
			WindowsAPI.GetWindowThreadProcessId(hwnd, ref iDProcess);
			IntPtr hProcess = WindowsAPI.OpenProcess(1040, false, iDProcess);
			WindowsAPI.GetModuleInformation(hProcess, IntPtr.Zero, ref mODULEINFO, Marshal.SizeOf(mODULEINFO));
		}
		public void HideInTaskBar(IntPtr hwnd)
		{
			WindowsAPI.SetWindowLong(hwnd, -8, 128);
		}
		public void ShowInTaskBar(IntPtr hwnd)
		{
			WindowsAPI.SetWindowLong(hwnd, -20, 262144);
		}
		public TStringList GetWindowStyle(IntPtr hwnd)
		{
			int windowLong = WindowsAPI.GetWindowLong(hwnd, -16);
			TStringList tStringList = new TStringList();
			if ((windowLong & 8388608) != 0)
			{
				tStringList.Add("WS_BORDER");
			}
			if ((windowLong & 12582912) != 0)
			{
				tStringList.Add("WS_CAPTION");
			}
			if ((windowLong & 1073741824) != 0)
			{
				tStringList.Add("WS_CHILD");
			}
			if ((windowLong & 1073741824) != 0)
			{
				tStringList.Add("WS_CHILDWINDOW");
			}
			if ((windowLong & 33554432) != 0)
			{
				tStringList.Add("WS_CLIPCHILDREN");
			}
			if ((windowLong & 67108864) != 0)
			{
				tStringList.Add("WS_CLIPSIBLINGS");
			}
			if ((windowLong & 134217728) != 0)
			{
				tStringList.Add("WS_DISABLED");
			}
			if ((windowLong & 4194304) != 0)
			{
				tStringList.Add("WS_DLGFRAME");
			}
			if ((windowLong & 131072) != 0)
			{
				tStringList.Add("WS_GROUP");
			}
			if ((windowLong & 1048576) != 0)
			{
				tStringList.Add("WS_HSCROLL");
			}
			if ((windowLong & 536870912) != 0)
			{
				tStringList.Add("WS_ICONIC");
			}
			if ((windowLong & 16777216) != 0)
			{
				tStringList.Add("WS_MAXIMIZE");
			}
			if ((windowLong & 65536) != 0)
			{
				tStringList.Add("WS_MAXIMIZEBOX");
			}
			if ((windowLong & 536870912) != 0)
			{
				tStringList.Add("WS_MINIMIZE");
			}
			if ((windowLong & 131072) != 0)
			{
				tStringList.Add("WS_MINIMIZEBOX");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_OVERLAPPED");
			}
			if ((windowLong & 13565952) != 0)
			{
				tStringList.Add("WS_OVERLAPPEDWINDOW");
			}
			if (((long)windowLong & (long)(-2147483648)) != 0L)
			{
				tStringList.Add("WS_POPUP");
			}
			if (((long)windowLong & (long)(-2138570752)) != 0L)
			{
				tStringList.Add("WS_POPUPWINDOW");
			}
			if ((windowLong & 262144) != 0)
			{
				tStringList.Add("WS_SIZEBOX");
			}
			if ((windowLong & 524288) != 0)
			{
				tStringList.Add("WS_SYSMENU");
			}
			if ((windowLong & 65536) != 0)
			{
				tStringList.Add("WS_TABSTOP");
			}
			if ((windowLong & 262144) != 0)
			{
				tStringList.Add("WS_THICKFRAME");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_TILED");
			}
			if ((windowLong & 13565952) != 0)
			{
				tStringList.Add("WS_TILEDWINDOW");
			}
			if ((windowLong & 268435456) != 0)
			{
				tStringList.Add("WS_VISIBLE");
			}
			if ((windowLong & 2097152) != 0)
			{
				tStringList.Add("WS_VSCROLL");
			}
			string lastString = tStringList.LastString;
			return tStringList;
		}
		public TStringList GetWindowStyleEx(IntPtr hwnd)
		{
			int windowLong = WindowsAPI.GetWindowLong(hwnd, -20);
			TStringList tStringList = new TStringList();
			if ((windowLong & 16) != 0)
			{
				tStringList.Add("WS_EX_ACCEPTFILES");
			}
			if ((windowLong & 262144) != 0)
			{
				tStringList.Add("WS_EX_APPWINDOW");
			}
			if ((windowLong & 512) != 0)
			{
				tStringList.Add("WS_EX_CLIENTEDGE");
			}
			if ((windowLong & 33554432) != 0)
			{
				tStringList.Add("WS_EX_COMPOSITED");
			}
			if ((windowLong & 1024) != 0)
			{
				tStringList.Add("WS_EX_CONTEXTHELP");
			}
			if ((windowLong & 65536) != 0)
			{
				tStringList.Add("WS_EX_CONTROLPARENT");
			}
			if ((windowLong & 1) != 0)
			{
				tStringList.Add("WS_EX_DLGMODALFRAME");
			}
			if ((windowLong & 524288) != 0)
			{
				tStringList.Add("WS_EX_LAYERED");
			}
			if ((windowLong & 4194304) != 0)
			{
				tStringList.Add("WS_EX_LAYOUTRTL");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_EX_LEFT");
			}
			if ((windowLong & 16384) != 0)
			{
				tStringList.Add("WS_EX_LEFTSCROLLBAR");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_EX_LTRREADING");
			}
			if ((windowLong & 64) != 0)
			{
				tStringList.Add("WS_EX_MDICHILD");
			}
			if ((windowLong & 134217728) != 0)
			{
				tStringList.Add("WS_EX_NOACTIVATE");
			}
			if ((windowLong & 1048576) != 0)
			{
				tStringList.Add("WS_EX_NOINHERITLAYOUT");
			}
			if ((windowLong & 4) != 0)
			{
				tStringList.Add("WS_EX_NOPARENTNOTIFY");
			}
			if ((windowLong & 768) != 0)
			{
				tStringList.Add("WS_EX_OVERLAPPEDWINDOW");
			}
			if ((windowLong & 392) != 0)
			{
				tStringList.Add("WS_EX_PALETTEWINDOW");
			}
			if ((windowLong & 4096) != 0)
			{
				tStringList.Add("WS_EX_RIGHT");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_EX_RIGHTSCROLLBAR");
			}
			if ((windowLong & 8192) != 0)
			{
				tStringList.Add("WS_EX_RTLREADING");
			}
			if ((windowLong & 131072) != 0)
			{
				tStringList.Add("WS_EX_STATICEDGE");
			}
			if ((windowLong & 128) != 0)
			{
				tStringList.Add("WS_EX_TOOLWINDOW");
			}
			if ((windowLong & 8) != 0)
			{
				tStringList.Add("WS_EX_TOPMOST");
			}
			if ((windowLong & 32) != 0)
			{
				tStringList.Add("WS_EX_TRANSPARENT");
			}
			if ((windowLong & 256) != 0)
			{
				tStringList.Add("WS_EX_WINDOWEDGE");
			}
			return tStringList;
		}
		public TStringList GetWindowStyleEx(int style)
		{
			TStringList tStringList = new TStringList();
			if ((style & 16) != 0)
			{
				tStringList.Add("WS_EX_ACCEPTFILES");
			}
			if ((style & 262144) != 0)
			{
				tStringList.Add("WS_EX_APPWINDOW");
			}
			if ((style & 512) != 0)
			{
				tStringList.Add("WS_EX_CLIENTEDGE");
			}
			if ((style & 33554432) != 0)
			{
				tStringList.Add("WS_EX_COMPOSITED");
			}
			if ((style & 1024) != 0)
			{
				tStringList.Add("WS_EX_CONTEXTHELP");
			}
			if ((style & 65536) != 0)
			{
				tStringList.Add("WS_EX_CONTROLPARENT");
			}
			if ((style & 1) != 0)
			{
				tStringList.Add("WS_EX_DLGMODALFRAME");
			}
			if ((style & 524288) != 0)
			{
				tStringList.Add("WS_EX_LAYERED");
			}
			if ((style & 4194304) != 0)
			{
				tStringList.Add("WS_EX_LAYOUTRTL");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_EX_LEFT");
			}
			if ((style & 16384) != 0)
			{
				tStringList.Add("WS_EX_LEFTSCROLLBAR");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_EX_LTRREADING");
			}
			if ((style & 64) != 0)
			{
				tStringList.Add("WS_EX_MDICHILD");
			}
			if ((style & 134217728) != 0)
			{
				tStringList.Add("WS_EX_NOACTIVATE");
			}
			if ((style & 1048576) != 0)
			{
				tStringList.Add("WS_EX_NOINHERITLAYOUT");
			}
			if ((style & 4) != 0)
			{
				tStringList.Add("WS_EX_NOPARENTNOTIFY");
			}
			if ((style & 768) != 0)
			{
				tStringList.Add("WS_EX_OVERLAPPEDWINDOW");
			}
			if ((style & 392) != 0)
			{
				tStringList.Add("WS_EX_PALETTEWINDOW");
			}
			if ((style & 4096) != 0)
			{
				tStringList.Add("WS_EX_RIGHT");
			}
			if (0 != 0)
			{
				tStringList.Add("WS_EX_RIGHTSCROLLBAR");
			}
			if ((style & 8192) != 0)
			{
				tStringList.Add("WS_EX_RTLREADING");
			}
			if ((style & 131072) != 0)
			{
				tStringList.Add("WS_EX_STATICEDGE");
			}
			if ((style & 128) != 0)
			{
				tStringList.Add("WS_EX_TOOLWINDOW");
			}
			if ((style & 8) != 0)
			{
				tStringList.Add("WS_EX_TOPMOST");
			}
			if ((style & 32) != 0)
			{
				tStringList.Add("WS_EX_TRANSPARENT");
			}
			if ((style & 256) != 0)
			{
				tStringList.Add("WS_EX_WINDOWEDGE");
			}
			return tStringList;
		}
		public void TransparentA(IntPtr Handle)
		{
			int windowLong = WindowsAPI.GetWindowLong(Handle, -20);
			long num = WindowsAPI.SetWindowLong(Handle, -20, 524320);
			WindowsAPI.SetLayeredWindowAttributes(Handle, 0, 100, 2);
		}
		public void TransparentA(IntPtr Handle, int trans)
		{
			int windowLong = WindowsAPI.GetWindowLong(Handle, -20);
			long num = WindowsAPI.SetWindowLong(Handle, -20, 524320);
			WindowsAPI.SetLayeredWindowAttributes(Handle, 0, trans, 2);
		}
		public void TransparentB(IntPtr Handle)
		{
			int windowLong = WindowsAPI.GetWindowLong(Handle, -20);
			long num = WindowsAPI.SetWindowLong(Handle, -20, 524288);
			WindowsAPI.SetLayeredWindowAttributes(Handle, 0, 100, 2);
		}
		public void TransparentB(IntPtr Handle, int tr)
		{
			int windowLong = WindowsAPI.GetWindowLong(Handle, -20);
			long num = WindowsAPI.SetWindowLong(Handle, -20, 524288);
			WindowsAPI.SetLayeredWindowAttributes(Handle, 0, tr, 2);
		}
		public void TransparentC(IntPtr Handle)
		{
			WindowsAPI.SetLayeredWindowAttributes(Handle, 0, 255, 0);
		}
		public bool FlashForm(IntPtr hwnd, int times, int style)
		{
			FLASHWINFO fLASHWINFO = default(FLASHWINFO);
			fLASHWINFO.cbSize = (uint)Marshal.SizeOf(fLASHWINFO);
			fLASHWINFO.dwFlags = 1;
			fLASHWINFO.uCount = (uint)times;
			fLASHWINFO.hwnd = hwnd;
			return WindowsAPI.FlashWindowEx(ref fLASHWINFO);
		}
		public Size GetControlSize(IntPtr handle)
		{
			Size result = default(Size);
			WINDOWPLACEMENT wINDOWPLACEMENT = default(WINDOWPLACEMENT);
			wINDOWPLACEMENT.length = Marshal.SizeOf(wINDOWPLACEMENT);
			wINDOWPLACEMENT.flags = 7;
			WindowsAPI.GetWindowPlacement(handle, ref wINDOWPLACEMENT);
			result.Width = wINDOWPLACEMENT.rcNormalPosition.right - wINDOWPLACEMENT.rcNormalPosition.left;
			result.Height = wINDOWPLACEMENT.rcNormalPosition.bottom - wINDOWPLACEMENT.rcNormalPosition.top;
			return result;
		}
		public Point GetLocation()
		{
			IntPtr currentIntPtr = this.GetCurrentIntPtr();
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(currentIntPtr, ref rECT);
			return new Point(rECT.left, rECT.top);
		}
		public Point GetLocation(IntPtr hwnd)
		{
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(hwnd, ref rECT);
			return new Point(rECT.left, rECT.top);
		}
		public bool SetCurrentWindowState(IntPtr handle, int hwnd)
		{
			return WindowsAPI.SetWindowPos(handle, (IntPtr)hwnd, 0, 0, 0, 0, 3u);
		}
		public void ReverseFormTitle(IntPtr hwnd)
		{
			WindowsAPI.SetWindowLong(hwnd, -20, 173867264);
		}
		public void ReverseFormTitleBack(IntPtr hwnd)
		{
			WindowsAPI.SetWindowLong(hwnd, -20, 327936);
		}
		public Size GetWindowSize()
		{
			IntPtr currentIntPtr = this.GetCurrentIntPtr();
			Size result = default(Size);
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(currentIntPtr, ref rECT);
			result.Width = rECT.right - rECT.left;
			result.Height = rECT.bottom - rECT.top;
			return result;
		}
		public Size GetWindowSize(IntPtr hwnd)
		{
			Size result = default(Size);
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(hwnd, ref rECT);
			result.Width = rECT.right - rECT.left;
			result.Height = rECT.bottom - rECT.top;
			return result;
		}
		public Size GetWindowClientSize(IntPtr hwnd)
		{
			RECT rECT = default(RECT);
			WindowsAPI.GetClientRect(hwnd, ref rECT);
			Size result = new Size(rECT.right - rECT.left, rECT.bottom - rECT.top);
			return result;
		}
		public void RemoveMaximizeBox(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num -= 65536;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void RemoveMinimizeBox(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num -= 131072;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void RemoveTitle(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num -= 12582912;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void RemoveSystemMenu(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num -= 262144;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void RemoveSizeBox(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num -= 262144;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void AddMaximizeBox(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num |= 65536;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void AddMinimizeBox(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num += 131072;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void AddTitle(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num += 12582912;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void AddSystemMenu(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num += 262144;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void AddSizeBox(IntPtr hwnd)
		{
			int num = WindowsAPI.GetWindowLong(hwnd, -16);
			num += 262144;
			WindowsAPI.SetWindowLong(hwnd, -16, num);
		}
		public void CloseTheme(IntPtr hwnd)
		{
			WindowsAPI.SetWindowTheme(hwnd, "", "");
		}
		public void OpenTheme(IntPtr hwnd)
		{
			WindowsAPI.SetWindowTheme(hwnd, null, null);
		}
		public Point GetWindowOrgEx(IntPtr hwnd)
		{
			IntPtr windowDC = WindowsAPI.GetWindowDC(hwnd);
			Point result = default(Point);
			bool windowOrgEx = WindowsAPI.GetWindowOrgEx(windowDC, ref result);
			return result;
		}
		public bool ShowTaskBar()
		{
			IntPtr hWnd = WindowsAPI.FindWindow("Shell_TrayWnd", null);
			bool result;
			if (WindowsAPI.IsWindowVisible(hWnd))
			{
				WindowsAPI.ShowWindow(hWnd, 0);
				result = false;
			}
			else
			{
				WindowsAPI.ShowWindow(hWnd, 5);
				result = true;
			}
			return result;
		}
		public bool NetState()
		{
			int num = 0;
			return WindowsAPI.IsNetworkAlive(ref num);
		}
		public string NetworkState()
		{
			int num = 0;
			string result = "";
			if (WindowsAPI.IsNetworkAlive(ref num))
			{
				if ((num & 4) == 4)
				{
					result = "AOL";
				}
				if ((num & 1) == 1)
				{
					result = "LAN";
				}
				if ((num & 2) == 2)
				{
					result = "WAN";
				}
			}
			else
			{
				result = "OFFLINE";
			}
			return result;
		}
		public string NetworkStateA()
		{
			int num = 0;
			WindowsAPI.InternetGetConnectedState(ref num, 0);
			if ((num & 2) == 2)
			{
			}
			if ((num & 1) == 1)
			{
			}
			if ((num & 4) == 4)
			{
			}
			if ((num & 8) == 8)
			{
			}
			return "OFFLINE";
		}
		public int NetStatus()
		{
			int result = 0;
			WindowsAPI.InternetGetConnectedStateEx(ref result, null, 254, 0);
			return result;
		}
		public void DrawRectangle()
		{
			Point point = default(Point);
			RECT rECT = default(RECT);
			IntPtr desktopWindow = WindowsAPI.GetDesktopWindow();
			IntPtr hdc = WindowsAPI.GetWindowDC(desktopWindow);
			int fnDrawMode = WindowsAPI.SetROP2(hdc, 10);
			WindowsAPI.GetCursorPos(ref point);
			IntPtr intPtr = WindowsAPI.WindowFromPoint(point);
			IntPtr handle = intPtr;
			WindowsAPI.GetWindowRect(handle, ref rECT);
			if (rECT.left < 0)
			{
				rECT.left = 0;
			}
			if (rECT.top < 0)
			{
				rECT.top = 0;
			}
			IntPtr intPtr2 = WindowsAPI.CreatePen(0, 3, 0);
			IntPtr hgdiobj = WindowsAPI.SelectObject(hdc, intPtr2);
			WindowsAPI.Rectangle(hdc, rECT.left, rECT.top, rECT.right, rECT.bottom);
			WindowsAPI.Sleep(300);
			WindowsAPI.Rectangle(hdc, rECT.left, rECT.top, rECT.right, rECT.bottom);
			WindowsAPI.SetROP2(hdc, fnDrawMode);
			WindowsAPI.SelectObject(hdc, hgdiobj);
			WindowsAPI.DeleteObject(intPtr2);
			hdc = IntPtr.Zero;
		}
		public void DrawRectangleEx(RECT rc)
		{
			IntPtr desktopWindow = WindowsAPI.GetDesktopWindow();
			IntPtr hdc = WindowsAPI.GetWindowDC(desktopWindow);
			int fnDrawMode = WindowsAPI.SetROP2(hdc, 10);
			if (rc.left < 0)
			{
				rc.left = 0;
			}
			if (rc.top < 0)
			{
				rc.top = 0;
			}
			IntPtr intPtr = WindowsAPI.CreatePen(0, 2, 0);
			IntPtr hgdiobj = WindowsAPI.SelectObject(hdc, intPtr);
			WindowsAPI.Rectangle(hdc, rc.left, rc.top, rc.right, rc.bottom);
			WindowsAPI.Sleep(10);
			WindowsAPI.Rectangle(hdc, rc.left, rc.top, rc.right, rc.bottom);
			WindowsAPI.SetROP2(hdc, fnDrawMode);
			WindowsAPI.SelectObject(hdc, hgdiobj);
			WindowsAPI.DeleteObject(intPtr);
			WindowsAPI.ReleaseDC(desktopWindow);
			hdc = IntPtr.Zero;
		}
		private void DrawRectangleDesk(IntPtr hwnd)
		{
			RECT rECT = default(RECT);
			WindowsAPI.GetWindowRect(hwnd, ref rECT);
			IntPtr intPtr = WindowsAPI.CreateCompatibleDC(hwnd);
			IntPtr hgdiobj = WindowsAPI.CreateCompatibleBitmap(hwnd, rECT.right, rECT.bottom);
			IntPtr hgdiobj2 = WindowsAPI.SelectObject(WindowsAPI.GetWindowDC(hwnd), hgdiobj);
			WindowsAPI.BitBlt(WindowsAPI.GetWindowDC(this.DeskHwnd), this.GetLocation().X, this.GetLocation().Y, rECT.right - rECT.left, rECT.bottom - rECT.top, WindowsAPI.GetWindowDC(hwnd), rECT.left, rECT.top, 13369376u);
			WindowsAPI.SelectObject(WindowsAPI.GetWindowDC(hwnd), hgdiobj2);
		}
		public void DrawRevFrame(IntPtr hWnd)
		{
			if (!(hWnd == IntPtr.Zero))
			{
				IntPtr windowDC = WindowsAPI.GetWindowDC(hWnd);
				RECT rECT = default(RECT);
				WindowsAPI.GetWindowRect(hWnd, ref rECT);
				WindowsAPI.OffsetRect(ref rECT, -rECT.left, -rECT.top);
				WindowsAPI.PatBlt(windowDC, rECT.left, rECT.top, rECT.right - rECT.left, 3, 5570569);
				WindowsAPI.PatBlt(windowDC, rECT.left, rECT.bottom - 3, 3, -(rECT.bottom - rECT.top - 6), 5570569);
				WindowsAPI.PatBlt(windowDC, rECT.right - 3, rECT.top + 3, 3, rECT.bottom - rECT.top - 6, 5570569);
				WindowsAPI.PatBlt(windowDC, rECT.right, rECT.bottom - 3, -(rECT.right - rECT.left), 3, 5570569);
			}
		}
		public Icon GetFileIconEx(string FileName)
		{
			SHFILEINFO sHFILEINFO = default(SHFILEINFO);
			Icon result = null;
			int num = WindowsAPI.SHGetFileInfo(FileName, 100, ref sHFILEINFO, 0u, 273u);
			if (num > 0)
			{
				result = Icon.FromHandle(sHFILEINFO.hIcon);
			}
			return result;
		}
		public Icon GetFileIcon(IntPtr hwnd)
		{
			int num = 0;
			return Icon.FromHandle(WindowsAPI.ExtractAssociatedIcon(hwnd, this.GetExecutePath(hwnd), ref num));
		}
		public Icon GetFileIcon(IntPtr hwnd, string path)
		{
			int num = 0;
			return Icon.FromHandle(WindowsAPI.ExtractAssociatedIcon(hwnd, path, ref num));
		}
		public void PlaySound(string path)
		{
			if (File.Exists(path))
			{
				new APIMethod.PlayAudio
				{
					FileName = path
				}.Play();
			}
		}
		public Bitmap GetScreen()
		{
			IntPtr hdc = WindowsAPI.CreateDC("DISPLAY", null, null, IntPtr.Zero);
			Graphics graphics = Graphics.FromHdc(hdc);
			Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, graphics);
			Graphics graphics2 = Graphics.FromImage(bitmap);
			IntPtr hdc2 = graphics.GetHdc();
			IntPtr hdc3 = graphics2.GetHdc();
			WindowsAPI.BitBlt(hdc3, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, hdc2, 0, 0, 13369376u);
			graphics.ReleaseHdc(hdc2);
			graphics2.ReleaseHdc(hdc3);
			return bitmap;
		}
		public Bitmap GetScreen(IntPtr hwnd)
		{
			IntPtr windowDC = WindowsAPI.GetWindowDC(hwnd);
			Graphics graphics = Graphics.FromHdc(windowDC);
			Size windowSize = this.GetWindowSize(hwnd);
			int num = this.GetWindowBorder(hwnd);
			if (num == 3)
			{
				num = 10;
			}
			Bitmap bitmap = new Bitmap(windowSize.Width + num, windowSize.Height + num, graphics);
			Graphics graphics2 = Graphics.FromImage(bitmap);
			IntPtr hdc = graphics.GetHdc();
			IntPtr hdc2 = graphics2.GetHdc();
			WindowsAPI.BitBlt(hdc2, 0, 0, windowSize.Width + num, windowSize.Height + num, hdc, 0, 0, 1087111200u);
			graphics.ReleaseHdc(hdc);
			graphics2.ReleaseHdc(hdc2);
			return bitmap;
		}
		public Bitmap GetScreenSnapShot()
		{
			int systemMetrics = WindowsAPI.GetSystemMetrics(0);
			int systemMetrics2 = WindowsAPI.GetSystemMetrics(1);
			IntPtr dC = WindowsAPI.GetDC(this.DeskHwnd);
			IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
			IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, systemMetrics, systemMetrics2);
			IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, intPtr2);
			WindowsAPI.BitBlt(intPtr, 0, 0, systemMetrics, systemMetrics2, dC, 0, 0, 1087111200u);
			WindowsAPI.SelectObject(intPtr, hgdiobj);
			Bitmap result = Image.FromHbitmap(intPtr2);
			WindowsAPI.DeleteDC(intPtr);
			WindowsAPI.DeleteObject(intPtr2);
			return result;
		}
		public Bitmap GetScreenSnapShot(IntPtr hwnd)
		{
			Point location = this.GetLocation(hwnd);
			int x = location.X;
			int nYSrc = location.Y;
			int width = this.GetWindowSize(hwnd).Width;
			int num = this.GetWindowSize(hwnd).Height;
			string currentWindowsVersion = this.GetCurrentWindowsVersion();
			if (currentWindowsVersion.StartsWith("Windows XP") && WindowsAPI.GetParent(hwnd) == IntPtr.Zero)
			{
				x = location.X;
				nYSrc = location.Y + 2;
				num -= 2;
			}
			IntPtr dC = WindowsAPI.GetDC(this.DeskHwnd);
			IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
			IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, width, num);
			IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, intPtr2);
			WindowsAPI.BitBlt(intPtr, 0, 0, width, num, dC, x, nYSrc, 1087111200u);
			WindowsAPI.SelectObject(intPtr, hgdiobj);
			Bitmap result = Image.FromHbitmap(intPtr2);
			WindowsAPI.DeleteDC(intPtr);
			WindowsAPI.DeleteObject(intPtr2);
			return result;
		}
		public Bitmap GetScreenSnapShot(RECT rc)
		{
			int left = rc.left;
			int top = rc.top;
			int nWidth = Math.Abs(rc.right - rc.left);
			int nHeight = Math.Abs(rc.bottom - rc.top);
			IntPtr dC = WindowsAPI.GetDC(this.DeskHwnd);
			IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
			IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, nWidth, nHeight);
			IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, intPtr2);
			WindowsAPI.BitBlt(intPtr, 0, 0, nWidth, nHeight, dC, left, top, 1087111200u);
			WindowsAPI.SelectObject(intPtr, hgdiobj);
			Bitmap result = Image.FromHbitmap(intPtr2);
			WindowsAPI.DeleteDC(intPtr);
			WindowsAPI.DeleteObject(intPtr2);
			return result;
		}
		public Bitmap GetScreenEx()
		{
			IntPtr hdc = WindowsAPI.CreateDC("DISPLAY", null, null, IntPtr.Zero);
			Graphics graphics = Graphics.FromHdc(hdc);
			Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, graphics);
			Graphics graphics2 = Graphics.FromImage(bitmap);
			IntPtr hdc2 = graphics.GetHdc();
			IntPtr hdc3 = graphics2.GetHdc();
			WindowsAPI.BitBlt(hdc3, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, hdc2, 0, 0, 1087111200u);
			graphics.ReleaseHdc(hdc2);
			graphics2.ReleaseHdc(hdc3);
			string text = this.GetTempPath() + "CSGraphicsSnapShot.bmp";
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			bitmap.Save(text, ImageFormat.Bmp);
			bitmap.Dispose();
			return new Bitmap(text);
		}
		public void SaveScreen(string path, string filename)
		{
			if (!Directory.Exists(path))
			{
				path = Environment.SpecialFolder.Personal.ToString();
			}
			if (filename == "")
			{
				filename = "屏幕截图.bmp";
			}
			Bitmap screen = this.GetScreen();
			screen.Save(path + filename);
		}
		public Bitmap GetCurrentWindowPicture()
		{
			IntPtr currentIntPtr = this.GetCurrentIntPtr();
			int num = this.GetWindowSize(currentIntPtr).Width;
			int num2 = this.GetWindowSize(currentIntPtr).Height;
			int num3 = this.GetLocation(currentIntPtr).X;
			int num4 = this.GetLocation(currentIntPtr).Y;
			string currentWindowsVersion = this.GetCurrentWindowsVersion();
			int windowBorder = this.GetWindowBorder(currentIntPtr);
			if (currentWindowsVersion.StartsWith("Windows Vista"))
			{
				if (windowBorder == 3)
				{
					num3 -= 5;
					num4 -= 5;
					num += 10;
					num2 += 10;
				}
			}
			if (currentWindowsVersion.StartsWith("Windows XP"))
			{
				num4 += 2;
			}
			Bitmap bitmap = new Bitmap(num, num2);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(num3, num4, 0, 0, new Size(num, num2), (CopyPixelOperation)1087111200);
			return bitmap;
		}
		public Bitmap GetCurrentWindowPicture(IntPtr hwnd)
		{
			int num = this.GetWindowSize(hwnd).Width;
			int num2 = this.GetWindowSize(hwnd).Height;
			int num3 = this.GetLocation(hwnd).X;
			int num4 = this.GetLocation(hwnd).Y;
			string currentWindowsVersion = this.GetCurrentWindowsVersion();
			int windowBorder = this.GetWindowBorder(hwnd);
			if (currentWindowsVersion.StartsWith("Windows Vista"))
			{
				if (windowBorder == 3)
				{
					num3 -= 5;
					num4 -= 5;
					num += 10;
					num2 += 10;
				}
			}
			if (currentWindowsVersion.StartsWith("Windows XP"))
			{
				num4 += 2;
			}
			Bitmap bitmap = new Bitmap(num, num2);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(num3, num4, 0, 0, new Size(num, num2));
			return bitmap;
		}
		public Bitmap GetCurrentWindowPicture(IntPtr hwnd, ref Point p)
		{
			int num = this.GetWindowSize(hwnd).Width;
			int num2 = this.GetWindowSize(hwnd).Height;
			int num3 = this.GetLocation(hwnd).X;
			int num4 = this.GetLocation(hwnd).Y;
			string currentWindowsVersion = this.GetCurrentWindowsVersion();
			int windowBorder = this.GetWindowBorder(hwnd);
			if (currentWindowsVersion.StartsWith("Windows Vista"))
			{
				if (windowBorder == 3)
				{
					num3 -= 5;
					num4 -= 5;
					num += 10;
					num2 += 10;
				}
			}
			if (currentWindowsVersion.StartsWith("Windows XP"))
			{
				if (WindowsAPI.GetParent(hwnd) == IntPtr.Zero)
				{
					num4 += 2;
				}
			}
			Bitmap bitmap = new Bitmap(num, num2);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(num3, num4, 0, 0, new Size(num, num2));
			p = new Point(num3, num4);
			return bitmap;
		}
		public Point GetCurrentWindowPicturePoint(IntPtr hwnd)
		{
			int num = this.GetLocation(hwnd).X;
			int num2 = this.GetLocation(hwnd).Y;
			string currentWindowsVersion = this.GetCurrentWindowsVersion();
			if (currentWindowsVersion.StartsWith("Windows Vista"))
			{
				if (WindowsAPI.GetParent(hwnd) == IntPtr.Zero)
				{
					num -= 5;
					num2 -= 5;
				}
			}
			if (currentWindowsVersion.StartsWith("Windows XP"))
			{
				if (WindowsAPI.GetParent(hwnd) == IntPtr.Zero)
				{
					num2 += 2;
				}
			}
			return new Point(num, num2);
		}
		public Bitmap GetCurrentWindowPicture(RECT rect)
		{
			Bitmap bitmap = new Bitmap(Math.Abs(rect.right - rect.left), Math.Abs(rect.bottom - rect.top));
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(Math.Abs(rect.right - rect.left), Math.Abs(rect.bottom - rect.top)));
			return bitmap;
		}
		public Bitmap GetCurrentWindowPictureEx(RECT rect)
		{
			int nWidth = Math.Abs(rect.right - rect.left);
			int nHeight = Math.Abs(rect.bottom - rect.top);
			IntPtr dC = WindowsAPI.GetDC(this.DeskHwnd);
			IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
			IntPtr intPtr2 = WindowsAPI.CreateCompatibleBitmap(dC, nWidth, nHeight);
			IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, intPtr2);
			WindowsAPI.BitBlt(intPtr, 0, 0, nWidth, nHeight, dC, rect.left, rect.top, 1087111200u);
			WindowsAPI.SelectObject(intPtr, hgdiobj);
			Bitmap result = Image.FromHbitmap(intPtr2);
			WindowsAPI.DeleteDC(intPtr);
			WindowsAPI.DeleteObject(intPtr2);
			return result;
		}
		public Bitmap GetWindowCaptureAsBitmap(int hwnd)
		{
			IntPtr intPtr = new IntPtr(hwnd);
			RECT rECT = default(RECT);
			Bitmap result;
			if (!WindowsAPI.GetWindowRect(intPtr, ref rECT))
			{
				result = null;
			}
			else
			{
				Bitmap bitmap = new Bitmap(rECT.right - rECT.left, rECT.bottom - rECT.top);
				Graphics graphics = Graphics.FromImage(bitmap);
				IntPtr hdc = graphics.GetHdc();
				IntPtr windowDC = WindowsAPI.GetWindowDC(intPtr);
				WindowsAPI.BitBlt(hdc, 0, 0, rECT.right - rECT.left, rECT.bottom - rECT.top, windowDC, 0, 0, 13369376u);
				graphics.ReleaseHdc(hdc);
				WindowsAPI.ReleaseDC(intPtr, windowDC);
				graphics.Dispose();
				result = bitmap;
			}
			return result;
		}
		public Bitmap GetWindowCaptureAsBitmap(IntPtr hwnd)
		{
			RECT rECT = default(RECT);
			Bitmap result;
			if (!WindowsAPI.GetWindowRect(hwnd, ref rECT))
			{
				result = null;
			}
			else
			{
				Bitmap bitmap = new Bitmap(rECT.right - rECT.left, rECT.bottom - rECT.top);
				Graphics graphics = Graphics.FromImage(bitmap);
				IntPtr hdc = graphics.GetHdc();
				IntPtr windowDC = WindowsAPI.GetWindowDC(hwnd);
				WindowsAPI.BitBlt(hdc, 0, 0, rECT.right - rECT.left, rECT.bottom - rECT.top, windowDC, 0, 0, 13369376u);
				graphics.ReleaseHdc(hdc);
				WindowsAPI.ReleaseDC(hwnd, windowDC);
				graphics.Dispose();
				result = bitmap;
			}
			return result;
		}
		public Bitmap GetWindowCaptureAsBitmapEx(IntPtr hwnd)
		{
			RECT rECT = default(RECT);
			Bitmap result;
			if (!WindowsAPI.GetWindowRect(hwnd, ref rECT))
			{
				result = null;
			}
			else
			{
				Bitmap bitmap = new Bitmap(rECT.right - rECT.left, rECT.bottom - rECT.top);
				Graphics graphics = Graphics.FromImage(bitmap);
				IntPtr hdc = graphics.GetHdc();
				IntPtr windowDC = WindowsAPI.GetWindowDC(hwnd);
				WindowsAPI.BitBlt(hdc, 0, 0, rECT.right - rECT.left, rECT.bottom - rECT.top, windowDC, 0, 0, 1087111200u);
				graphics.ReleaseHdc(hdc);
				WindowsAPI.ReleaseDC(hwnd, windowDC);
				graphics.Dispose();
				result = bitmap;
			}
			return result;
		}
		public void PrintScreen()
		{
			WindowsAPI.keybd_event(44, 0, 2, 0);
		}
		public void PrintScreenEx()
		{
			INPUT iNPUT = default(INPUT);
			iNPUT.ki.wVk = 44;
			iNPUT.ki.wScan = 0;
			iNPUT.ki.dwFlags = 1;
			iNPUT.ki.dwExtraInfo = IntPtr.Zero;
			iNPUT.type = 1;
			WindowsAPI.SendInput(1u, new INPUT[]
			{
				iNPUT
			}, Marshal.SizeOf(iNPUT));
		}
		public Image GetDestopImage()
		{
			int width = Screen.PrimaryScreen.Bounds.Width;
			int height = Screen.PrimaryScreen.Bounds.Height;
			Bitmap bitmap = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
			Clipboard.SetImage(bitmap);
			return bitmap;
		}
		public Bitmap CaptureHandle(IntPtr handle)
		{
			Bitmap bitmap = null;
			try
			{
				using (Graphics graphics = Graphics.FromHwnd(handle))
				{
					RECT rECT = default(RECT);
					WindowsAPI.GetWindowRect(handle, ref rECT);
					if ((int)graphics.VisibleClipBounds.Width > 0 && (int)graphics.VisibleClipBounds.Height > 0)
					{
						bitmap = new Bitmap(rECT.right - rECT.left, rECT.bottom - rECT.top, graphics);
						using (Graphics graphics2 = Graphics.FromImage(bitmap))
						{
							graphics2.CopyFromScreen(rECT.left, rECT.top, 0, 0, new Size(rECT.right - rECT.left, rECT.bottom - rECT.top), CopyPixelOperation.SourceCopy);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			return bitmap;
		}
		public void Magnify(Bitmap bmp, int multiple)
		{
			if (bmp != null)
			{
				if (multiple < 1)
				{
					multiple = 1;
				}
				Size size = bmp.Size;
				IntPtr hbitmap = bmp.GetHbitmap();
				IntPtr dC = WindowsAPI.GetDC(this.DeskHwnd);
				IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
				IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, hbitmap);
				Point currentPos = this.GetCurrentPos();
				WindowsAPI.StretchBlt(dC, currentPos.X - 3, currentPos.Y - 6, bmp.Width * multiple, bmp.Height * multiple, intPtr, 0, 0, bmp.Width, bmp.Height, 13369376);
				IntPtr hObject = WindowsAPI.SelectObject(intPtr, hgdiobj);
				WindowsAPI.DeleteObject(hObject);
				WindowsAPI.DeleteDC(intPtr);
			}
		}
		public void Magnify(Bitmap bmp, int multiple, IntPtr hwnd)
		{
			if (bmp != null)
			{
				Point location = this.GetLocation(hwnd);
				if (multiple < 1)
				{
					multiple = 1;
				}
				Size size = bmp.Size;
				IntPtr hbitmap = bmp.GetHbitmap();
				IntPtr dC = WindowsAPI.GetDC(hwnd);
				IntPtr intPtr = WindowsAPI.CreateCompatibleDC(dC);
				IntPtr hgdiobj = WindowsAPI.SelectObject(intPtr, hbitmap);
				WindowsAPI.StretchBlt(dC, 0, location.Y - 10, bmp.Width * multiple, bmp.Height * multiple, intPtr, 0, 0, bmp.Width, bmp.Height, 13369376);
				IntPtr hObject = WindowsAPI.SelectObject(intPtr, hgdiobj);
				WindowsAPI.DeleteObject(hObject);
				WindowsAPI.DeleteDC(intPtr);
			}
		}
		public Bitmap MagnifyEx(int Width, int Height, int x, int y, int multiple)
		{
			IntPtr intPtr = WindowsAPI.CreateDC("DISPLAY", null, null, IntPtr.Zero);
			IntPtr intPtr2 = WindowsAPI.CreateCompatibleDC(intPtr);
			IntPtr intPtr3 = WindowsAPI.CreateCompatibleBitmap(intPtr, Width, Height);
			IntPtr intPtr4 = WindowsAPI.SelectObject(intPtr2, intPtr3);
			WindowsAPI.BitBlt(intPtr2, 0, 0, Width, Height, intPtr, x, y, 1087111200u);
			Bitmap result;
			if (WindowsAPI.StretchBlt(intPtr2, 0, 0, Width * multiple, Height * multiple, intPtr2, 0, 0, Width, Height, 13369376))
			{
				Bitmap bitmap = Image.FromHbitmap(WindowsAPI.SelectObject(intPtr2, intPtr4));
				WindowsAPI.ReleaseDC(intPtr3, intPtr);
				WindowsAPI.DeleteDC(intPtr);
				WindowsAPI.DeleteDC(intPtr2);
				WindowsAPI.DeleteDC(intPtr4);
				WindowsAPI.DeleteObject(intPtr3);
				result = bitmap;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public void Register(IntPtr hwnd, Keys key)
		{
			WindowsAPI.RegisterHotKey(hwnd, 17, 2u, key);
		}
		public void Register(IntPtr hwnd, int key)
		{
			int num = 4;
			int wParam = num * 256 + 65;
			WindowsAPI.SendMessage(hwnd, 50u, wParam, 0);
		}
		public void UnRegister(IntPtr hwnd)
		{
			WindowsAPI.UnregisterHotKey(hwnd, 17);
		}
		public Point GetCursorPos(TextBox textBox)
		{
			Point result = new Point(0, 0);
			int num = WindowsAPI.SendMessage(textBox.Handle, 201u, textBox.SelectionStart, 0);
			int num2 = textBox.SelectionStart - WindowsAPI.SendMessage(textBox.Handle, 187u, num, 0);
			result.Y = num + 1;
			result.X = num2 + 1;
			return result;
		}
		public Point GetCursorPos(RichTextBox richTextBox)
		{
			Point result = new Point(0, 0);
			int num = WindowsAPI.SendMessage(richTextBox.Handle, 201u, richTextBox.SelectionStart, 0);
			int num2 = richTextBox.SelectionStart - WindowsAPI.SendMessage(richTextBox.Handle, 187u, num, 0);
			result.Y = num + 1;
			result.X = num2 + 1;
			return result;
		}
		public void GoToLine(TextBox textBox, int line)
		{
			textBox.SelectionStart = WindowsAPI.SendMessage(textBox.Handle, 187u, line - 1, 0);
			textBox.ScrollToCaret();
		}
		public void RandomDrag(Form form)
		{
			form.MouseDown += new MouseEventHandler(this.form_MouseDown);
		}
		public void RandomDrag(Control control)
		{
			control.MouseDown += new MouseEventHandler(this.control_MouseDown);
		}
		private void form_MouseDown(object sender, MouseEventArgs e)
		{
			Form form = (Form)sender;
			WindowsAPI.ReleaseCapture();
			if (!form.IsDisposed)
			{
				WindowsAPI.SendMessage(form.Handle, 161u, 2, 0);
			}
		}
		private void control_MouseDown(object sender, MouseEventArgs e)
		{
			Control control = (Control)sender;
			WindowsAPI.ReleaseCapture();
			WindowsAPI.SendMessage(control.Handle, 161u, 2, 0);
		}
		public void Shake(IntPtr hwnd, int times)
		{
			RECT rECT = default(RECT);
			int dwMilliseconds = 40;
			int num = 4;
			for (int i = 0; i < times; i++)
			{
				WindowsAPI.GetWindowRect(hwnd, ref rECT);
				this.WindowMove(hwnd, rECT.left - num, rECT.top);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left, rECT.top - num);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left + num, rECT.top);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left, rECT.top + num);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left, rECT.top);
			}
		}
		public void Shake(IntPtr hwnd, int times, int speed)
		{
			RECT rECT = default(RECT);
			int num = 4;
			for (int i = 0; i < times; i++)
			{
				WindowsAPI.GetWindowRect(hwnd, ref rECT);
				this.WindowMove(hwnd, rECT.left - num, rECT.top);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left, rECT.top - num);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left + num, rECT.top);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left, rECT.top + num);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left, rECT.top);
			}
		}
		public void ShakeQQ(IntPtr hwnd, int times)
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			string path = regeditManageMent.GetValue("SOFTWARE\\TENCENT\\QQ\\", Registry.LocalMachine, "Install") + "sound\\up.wav";
			this.PlaySound(path);
			RECT rECT = default(RECT);
			int dwMilliseconds = 40;
			int num = 4;
			for (int i = 0; i < times; i++)
			{
				WindowsAPI.GetWindowRect(hwnd, ref rECT);
				this.WindowMove(hwnd, rECT.left + num, rECT.top);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left, rECT.top - num);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left - num, rECT.top);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left, rECT.top + num);
				WindowsAPI.Sleep(dwMilliseconds);
				this.WindowMove(hwnd, rECT.left, rECT.top);
			}
		}
		public void ShakeQQ(IntPtr hwnd, int times, int speed)
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			string path = regeditManageMent.GetValue("SOFTWARE\\TENCENT\\QQ\\", Registry.LocalMachine, "Install") + "sound\\up.wav";
			this.PlaySound(path);
			RECT rECT = default(RECT);
			int num = 4;
			for (int i = 0; i < times; i++)
			{
				WindowsAPI.GetWindowRect(hwnd, ref rECT);
				this.WindowMove(hwnd, rECT.left + num, rECT.top);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left, rECT.top - num);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left - num, rECT.top);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left, rECT.top + num);
				WindowsAPI.Sleep(speed);
				this.WindowMove(hwnd, rECT.left, rECT.top);
			}
		}
		public void ShakeExA(IntPtr hwnd)
		{
			int maxValue = 5;
			int x = this.GetLocation(hwnd).X;
			int y = this.GetLocation(hwnd).Y;
			Random random = new Random();
			for (int i = 0; i < 100; i++)
			{
				int num = random.Next(maxValue);
				int num2 = random.Next(maxValue);
				if (num % 2 == 0)
				{
					this.WindowMove(hwnd, x + num, y);
				}
				else
				{
					this.WindowMove(hwnd, x - num, y);
				}
				if (num2 % 2 == 0)
				{
					this.WindowMove(hwnd, x, y + num2);
				}
				else
				{
					this.WindowMove(hwnd, x, y + num2);
				}
			}
			this.WindowMove(hwnd, x, y);
		}
		public void ShakeExB(Form form)
		{
			int maxValue = 5;
			int x = form.Location.X;
			int y = form.Location.Y;
			Random random = new Random();
			for (int i = 0; i < 50; i++)
			{
				int num = random.Next(maxValue);
				int num2 = random.Next(maxValue);
				if (num % 2 == 0)
				{
					form.Left += num;
				}
				else
				{
					form.Left -= num;
				}
				if (num2 % 2 == 0)
				{
					form.Top += num2;
				}
				else
				{
					form.Top -= num2;
				}
				Thread.Sleep(1);
			}
			form.Left = x;
			form.Top = y;
		}
		public Icon GetFileIcon(string path, int index)
		{
			Icon result;
			if (!File.Exists(path))
			{
				result = SystemIcons.WinLogo;
			}
			else
			{
				int[] phiconLarge = new int[1];
				int[] array = new int[1];
				WindowsAPI.ExtractIconEx(path, index, phiconLarge, array, 1u);
				IntPtr handle = new IntPtr(array[0]);
				result = Icon.FromHandle(handle);
			}
			return result;
		}
		public void SetIcon(Form form, string path, int index)
		{
			if (File.Exists(path))
			{
				IntPtr intPtr = new IntPtr(WindowsAPI.ExtractIcon(form.Handle.ToInt32(), path, index));
				if (!(intPtr == IntPtr.Zero) && intPtr.ToInt32() != 0)
				{
					form.Icon = Icon.FromHandle(intPtr);
				}
			}
		}
		public void OwnIcon(Form form)
		{
			IntPtr intPtr = new IntPtr(WindowsAPI.ExtractIcon(form.Handle.ToInt32(), "./" + Path.GetFileName(Application.ExecutablePath), 0));
			if (!(intPtr == IntPtr.Zero))
			{
				form.Icon = Icon.FromHandle(intPtr);
			}
		}
	}
}
