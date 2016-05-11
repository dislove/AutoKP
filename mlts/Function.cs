using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using dmNet;
using System.Threading;

namespace mlts
{
    class SysFunction
    {
        const uint PROCESS_ALL_ACCESS = 0x001F0FFF;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        public static extern uint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(uint hProcess, uint lpBaseAddress, IntPtr lpBuffer, uint nSize, ref uint lpNumberOfBytesRead);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetWindowThreadProcessId(uint hwnd, out uint ID);

        [DllImport("Kernel32.dll")]
        private static extern int Beep(int dwFreq, int dwDuration);

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeDebugPrivilege";
        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;

        public static void Warning()
        {
            int a = 0X7FF;
            int b = 2000;
            Beep(a, b);
        }
        public static Queue<string> LogStrList = new Queue<string>();

        public static bool GetDbgPri()
        {
            bool ret = false;
            TokPriv1Luid tp;
            byte[] vBuffer = new byte[2];
            IntPtr memAdd = (IntPtr)0xB1CA90;
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0); // 得到缓冲区的地址

            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;

            ret = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            if (false == ret)
            {
                SysFunction.Log("获取进程Token错误");
                return false;
            }
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ret = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            if (false == ret)
            {
                SysFunction.Log("查询权限错误");
                return false;
            }
            ret = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            if (false == ret)
            {
                SysFunction.Log("获取权限错误");
                return false;
            }
            return true;
        }

        public static void Log(String StrLog)
        {
            String Text = "";
            Text = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "]" + StrLog;
            LogStrList.Enqueue(Text);
        }

        public static uint GetProcessId(uint hwnd)
        {
            uint ProcessId;
            uint ProcessHandler;
            uint ret;

            ret = GetWindowThreadProcessId(hwnd, out ProcessId);
            if (ret == 0 || ProcessId == 0)
            {
                return 0;
            }
            ProcessHandler = OpenProcess(0x0010, false, ProcessId);
            if (ProcessHandler == 0)
            {
                Log(GetLastError().ToString());
                return 0;
            }
            return ProcessHandler;
        }

        public static bool ReadMemory(uint ProcessHandle, uint MemAddr, out int Value, uint size)
        {
            byte[] vBuffer = new byte[size];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0);
            uint read = 0;
            bool ret = false;
            Value = 0;
            if (size > 4)
            {
                return false;
            }
            ret = ReadProcessMemory(ProcessHandle, MemAddr, vBytesAddress, size, ref read);
            if (ret == false)
            {
                return false;
            }
            if (size == 1)
            {
                Value = vBuffer[0];
            }
            if (size == 2)
            {
                Value = vBuffer[0] + vBuffer[1] * 256;
            }
            if (size == 4)
            {
                Value = vBuffer[0] + vBuffer[1] * 256 + vBuffer[2] * 256 * 256 + vBuffer[3] * 256 * 256 * 256;
            }
            return true;

        }
    }

    class GameHook
    {
        uint ProcessHandler;
        uint hwnd;
        uint hwnd_wg;
        public dmsoft dm;
        // public dmsoft dm_wg;
        WindowEnum win;
        List<WindowEnum.WindowInfo> wininfoList;


        public GameHook(uint iwnd, uint iwnd_wg, int ibinding)
        {
            hwnd = iwnd;
            ProcessHandler = SysFunction.GetProcessId(hwnd);
            string StrPath = System.Environment.CurrentDirectory + @"\..\..\..";
            dm = new dmsoft();
            string dxmouse = "windows", dxkeyboard = "windows", dxpublic = "dx.public.active.api";
            //int dm_ret = dm.BindWindow((int)hwnd, "dx2", "windows3", "windows", 0);
            if (ibinding == 0)
            {
                dxmouse = "dx.mouse.position.lock.api|dx.mouse.position.lock.message|dx.mouse.clip.lock.api|dx.mouse.input.lock.api|dx.mouse.state.api|dx.mouse.api|dx.mouse.cursor";
                dxkeyboard = "dx.keypad.input.lock.api|dx.keypad.state.api|dx.keypad.api";
                dxpublic = "dx.public.active.api|dx.public.active.message";
            }
            //else
            //{

            //}
            int dm_ret = dm.BindWindowEx((int)hwnd, "dx2", dxmouse, dxkeyboard, dxpublic, 0);
            //这里的内容要放到资源里，现在暂时写绝对路径
            //dm.SetPath(@"D:\魔力自动\鉴定");
            dm.SetPath(StrPath + @"\data");
            dm.SetDict(0, StrPath + @"\data\zk.txt");
            SysFunction.Log("绑定状态： " + dm_ret);

            win = new WindowEnum();
            hwnd_wg = iwnd_wg;
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            if (wininfoList.Count > 0)
            {
                SysFunction.Log("高速绑定成功！");
            }
            else
            {
                SysFunction.Log("高速绑定失败！");
            }
            // ProcessHandler = SysFunction.GetProcessId(hwnd_wg);
            // dm_wg = new dmsoft();
            // int dm_ret_wg = dm_wg.BindWindow((int)hwnd_wg, "dx2", "windows", "windows", 0);
            //int dm_ret_wg = dm_wg.BindWindowEx((int)hwnd_wg, "dx2", dxmouse, dxkeyboard, dxpublic, 0);
            //这里的内容要放到资源里，现在暂时写绝对路径
            //dm_wg.SetPath(StrPath + @"\鉴定");
            //dm_wg.SetPath(@"D:\魔力自动\鉴定");
            // dm_wg.SetPath(StrPath + @"\data");
            // dm_wg.SetDict(0, StrPath + @"\data\zk.txt");
            // SysFunction.Log("高速绑定状态： " + dm_ret_wg);
        }
        public void Close()
        {
            try
            {
                dm.UnBindWindow();
                //dm_wg.UnBindWindow();
                dm = null;
                //dm_wg = null;
                System.GC.Collect();
            }
            catch
            {
                System.GC.Collect();

            }
        }
        public void GetXY(out uint x, out uint y)
        {
            int xx = 0;
            int yy = 0;
            SysFunction.ReadMemory(ProcessHandler, 0xB1CA90, out xx, 2);
            SysFunction.ReadMemory(ProcessHandler, 0xC3B778, out yy, 2);

            x = (uint)xx;
            y = (uint)yy;
        }

        public void GetHp(out uint current, out uint max)
        {
            char[] xx = new char[20];
            int yy = 0;
            int i = 0;
            current = 0;
            max = 0;
            bool ret = SysFunction.ReadMemory(ProcessHandler, 0x00CF8A48, out yy, 1);
            if (ret == false)
            {
                return;
            }
            while (yy != 0)
            {
                xx[i++] = (char)yy;
                SysFunction.ReadMemory(ProcessHandler, (uint)(0x00CF8A48 + i - 1), out yy, 1);
            }

            string data = new string(xx);
            string[] data1 = data.Split('/');
            current = uint.Parse(data1[0]);
            max = uint.Parse(data1[1]);

        }
        public void GetMp(out uint current, out uint max)
        {
            char[] xx = new char[20];
            int yy = 0;
            int i = 0;
            current = 0;
            max = 0;
            bool ret = SysFunction.ReadMemory(ProcessHandler, 0x00D56408, out yy, 1);
            if (ret == false)
            {
                return;
            }
            while (yy != 0)
            {
                xx[i++] = (char)yy;
                SysFunction.ReadMemory(ProcessHandler, (uint)(0x00D56408 + i - 1), out yy, 1);
            }

            string data = new string(xx);
            string[] data1 = data.Split('/');
            current = uint.Parse(data1[0]);
            max = uint.Parse(data1[1]);
        }

        public void GetBag(string[] item)
        {
            int yy = 0;
            int i = 0;
            byte[] xx = new byte[20];
            uint firstAdd = 0x00D2133C;
            bool ret = false;

            for (uint j = 0; j < 20; j++)
            {
                i = 0;
                xx = new byte[20];
                xx[0] = 0;
                yy = 0;
                ret = SysFunction.ReadMemory(ProcessHandler, firstAdd + j * 1604, out yy, 1);
                if (ret == false)
                {
                    return;
                }
                while (yy != 0)
                {
                    xx[i++] = (byte)yy;
                    SysFunction.ReadMemory(ProcessHandler, (uint)(firstAdd + j * 1604 + i), out yy, 1);
                }
                item[j] = Encoding.GetEncoding(936).GetString(xx).Trim('\0');
            }
        }

        public void GetPetHp(out uint current, out uint max, uint index)
        {
            bool ret = false;
            current = 0;
            max = 0;
            int value;
            ret = SysFunction.ReadMemory(ProcessHandler, 0xCC35E8 + (index - 1) * 16, out value, 4);
            if (ret == false)
            {
                return;
            }
            current = (uint)value;
            ret = SysFunction.ReadMemory(ProcessHandler, 0xCC35E8 + (index - 1) * 16 + 4, out value, 4);
            if (ret == false)
            {
                return;
            }
            max = (uint)value;
        }
        public void GetPetMp(out uint current, out uint max, uint index)
        {
            bool ret = false;
            current = 0;
            max = 0;
            int value;
            ret = SysFunction.ReadMemory(ProcessHandler, 0xCC35E8 + (index - 1) * 16 + 8, out value, 4);
            if (ret == false)
            {
                return;
            }
            current = (uint)value;
            ret = SysFunction.ReadMemory(ProcessHandler, 0xCC35E8 + (index - 1) * 16 + 12, out value, 4);
            if (ret == false)
            {
                return;
            }
            max = (uint)value;
        }
        public void OpenGaosuCaiji()
        {
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "提速采集")
                {
                    //win.Click(wininfoList[i].hWnd);
                    //Thread.Sleep(200);
                    if (wininfoList[i - 1].szWindowName == "关")
                    {
                        SysFunction.Log("开启高速采集");
                        win.Click(wininfoList[i].hWnd);
                        //win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
                    }
                    else
                    {
                        SysFunction.Log("高速采集已开启，无需再开");

                    }
                    break;
                }
            }
        }
        public void CloseGaosuCaiji()
        {
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "提速采集")
                {
                    //win.Click(wininfoList[i].hWnd);
                    //Thread.Sleep(200);
                    if (wininfoList[i - 1].szWindowName == "开")
                    {
                        SysFunction.Log("高速采集关闭了");
                        win.Click(wininfoList[i].hWnd);
                    }
                    else
                    {
                        SysFunction.Log("高速采集已关闭，无需再关");

                    }
                    break;
                }
            }
        }
        public void OpenYuandi()
        {

            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "原地遇敌")
                {
                    //win.Click(wininfoList[i].hWnd);
                    //Thread.Sleep(200);
                    if (wininfoList[i - 1].szWindowName == "关")
                    {
                        SysFunction.Log("开启原地");
                        win.Click(wininfoList[i].hWnd);
                        //win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
                    }
                    else
                    {
                        SysFunction.Log("原地已开启，无需再开");

                    }
                    break;
                }
            }
        }
        public void CloseYuandi()
        {

            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "原地遇敌")
                {
                    //win.Click(wininfoList[i].hWnd);
                    //Thread.Sleep(200);
                    if (wininfoList[i - 1].szWindowName == "开")
                    {
                        SysFunction.Log("关闭原地");
                        win.Click(wininfoList[i].hWnd);
                    }
                    else
                    {
                        SysFunction.Log("原地已关闭，无需再关");

                    }
                    break;
                }
            }
        }
        public void OpenGaosuYidong()
        {
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "高速移动")
                {
                    //win.Click(wininfoList[i].hWnd);
                    //Thread.Sleep(200);
                    if (wininfoList[i + 1].szWindowName == "关")
                    {
                        SysFunction.Log("开启高速移动");
                        win.Click(wininfoList[i].hWnd);
                    }
                    else
                    {

                        SysFunction.Log("高速移动已开启，无需再开");
                    }
                    break;
                }
            }
        }
        public void CloseGaosuYidong()
        {
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "高速移动")
                {
                    //win.Click(wininfoList[i].hWnd);
                    //Thread.Sleep(200);
                    if (wininfoList[i + 1].szWindowName == "开")
                    {
                        SysFunction.Log("关闭高速移动");
                        win.Click(wininfoList[i].hWnd);
                        //win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
                    }
                    else
                    {
                        SysFunction.Log("高速移动已关闭，无需再开");

                    }
                    break;
                }
            }
        }
        public bool QuickIdentify()
        {
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "高速鉴定")
                {

                    SysFunction.Log("高速鉴定");
                    win.Click(wininfoList[i].hWnd);

                    return true;
                }
            }
            return false;

        }
        public bool QuickManufacturing()
        {
            wininfoList = win.EnumChildWindowsCallback((IntPtr)(hwnd_wg));
            for (int i = 0; i < wininfoList.Count; i++)
            {
                if (wininfoList[i].szWindowName == "高速制造")
                {

                    SysFunction.Log("高速制造");
                    win.Click(wininfoList[i].hWnd);

                    return true;
                }
            }
            return false;
        }
        public void Hide()
        {
            if (dm != null && hwnd != null)
            {
                dm.MoveWindow((int)hwnd, -640, -480);
            }
        }
        public void Show()
        {
            if (dm != null && hwnd != null)
            {
                dm.MoveWindow((int)hwnd, 0, 0);
            }
        }

    }
}
