using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace mlts
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    class WindowEnum
    {
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hwnd, int msg, uint wParam, uint lParam); 

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern bool EnumChildWindows(IntPtr hwndParent, WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);
        //[DllImport("user32.dll")] 
        //private static extern IntPtr FindWindowW(string lpClassName, string lpWindowName); 
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }


        public List<WindowInfo> EnumChildWindowsCallback(IntPtr handle)
        {
            List<WindowInfo> wndList = new List<WindowInfo>();
            EnumChildWindows(handle, delegate(IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                //get hwnd 
                wnd.hWnd = hWnd;
                //get window name 
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                //get window class 
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);
            return wndList;//.Where(it => it.szWindowName == name && it.szClassName == classname).ToList();
        }

        public List<WindowInfo> GetAllDesktopWindows(string name, string classname)
        {
            List<WindowInfo> wndList = new List<WindowInfo>();

            //enum all desktop windows 
            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                //get hwnd 
                wnd.hWnd = hWnd;
                //get window name 
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                //get window class 
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList.Where(it => it.szWindowName == name && it.szClassName == classname).ToList();
        }
        int WM_LBUTTONDOWN = 0x201;
        int WM_LBUTTONUP = 0x202;

        public void Click(IntPtr HWND)
        {
            PostMessage(HWND, 0x21, 0x410df2, 0x02010001);
            PostMessage(HWND, WM_LBUTTONDOWN, 1, 0x00060003);
            PostMessage(HWND, WM_LBUTTONDOWN, 1, 0x00060003);
            PostMessage(HWND, WM_LBUTTONUP, 0, 0x00060003);
        }
    }
}
