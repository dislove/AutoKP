using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;


namespace mlts
{
    class GameScript
    {
        public GameHook GameHookHandle = null;
        Thread ThreadHandle = null;
        uint hwnd = 0;
        uint hwnd_wg = 0;
        int scriptIndex;

        //public TSPlugInterFace ts;

        public GameScript(uint ihwnd, uint ihwnd_wg, int iscriptIndex, int ibinding = 0)
        {
            hwnd = ihwnd;
            hwnd_wg = ihwnd_wg;
            scriptIndex = iscriptIndex;
            GameHookHandle = new GameHook(hwnd, hwnd_wg, ibinding);

        }

        public void Close()
        {
            hwnd = 0;
            hwnd_wg = 0;
            Stop();
            Thread.Sleep(200);
            if (GameHookHandle != null)
            {
                GameHookHandle.Close();
            }
        }

        public void Pause()
        {
            try
            {
                if (ThreadHandle == null)
                {
                    SysFunction.Log("脚本线程未启动启动");
                }
                else
                {
                    ThreadHandle.Suspend();
                    GameHookHandle.dm.UnBindWindow();
                    SysFunction.Log("脚本线程已经暂停");
                    return;
                }
            }
            catch
            {
                GameHookHandle.dm.UnBindWindow();
                SysFunction.Log("脚本线程未启动启动");
            }
        }
        public void Resume()
        {
            if (ThreadHandle == null)
            {
                SysFunction.Log("脚本线程未启动启动");
            }
            else
            {
                ThreadHandle.Resume();
                const string dxmouse = "dx.mouse.position.lock.api|dx.mouse.position.lock.message|dx.mouse.clip.lock.api|dx.mouse.input.lock.api|dx.mouse.state.api|dx.mouse.api|dx.mouse.cursor";
                const string dxkeyboard = "dx.keypad.input.lock.api|dx.keypad.state.api|dx.keypad.api";
                const string dxpublic = "dx.public.active.api|dx.public.active.message";
                int dm_ret = GameHookHandle.dm.BindWindowEx((int)hwnd, "dx2", dxmouse, dxkeyboard, dxpublic, 0);
                SysFunction.Log("脚本线程已经恢复");
                return;
            }
        }
        //83126006
        public void Start()
        {
            if (ThreadHandle == null)
            {
                ThreadHandle = new Thread(script);
                ThreadHandle.IsBackground = true;
            }
            else if (ThreadHandle.ThreadState == ThreadState.Running)
            {
                SysFunction.Log("脚本线程已经启动");
                return;
            }
            if (ThreadHandle == null)
            {
                SysFunction.Log("无法启动脚本线程");
                return;
            }
            ThreadHandle.Start();
        }
        public void Stop()
        {
            if (ThreadHandle == null)
            {
                SysFunction.Log("脚本线程未启动启动");
            }
            else if (ThreadHandle.ThreadState == ThreadState.Running)
            {
                ThreadHandle.Abort();
                ThreadHandle = null;
                SysFunction.Log("脚本线程已经停止");
                return;
            }

        }

        public void Hide()
        {
            if (GameHookHandle != null)
            {
                GameHookHandle.Hide();
            }
        }
        public void Show()
        {
            if (GameHookHandle != null)
            {
                GameHookHandle.Show();
            }
        }
        public bool Manufacturing()
        {
            return GameHookHandle.QuickManufacturing();
        }

        // 脚本区
        string[] 怪物固定坐标 = new string[] { "342,140", "288,176", "214,212", "150,248", "86,274", "282,90", "228,126", "154,162", "90,198", "26,224" };
        int px = -3;
        int py = -25;
        int Hwnd;
        int 战斗状态, 战斗ID;
        private bool isBack;

        //界面参数
        public bool isHit = false, isWork = true, isAidIn = true, isAidOut = false, isHeal;

        public double 人物血线 = 0.2, 人物魔线 = 0.1, 人物补血线 = 0.6;
        public double 人物血值 = 100, 人物魔值 = 15;

        public double 宠物血线 = 0.2, 宠物吸血魔线 = 0.1, 宠物吸血线 = 0.7;
        public double 宠物血值 = 20, 宠物魔值 = 20;

        public string 刷屏信息 = "";

        public int 补血技能 = 1, 补血等级 = 1, 急救技能 = 1, 急救等级 = 1, 治疗技能 = 3, 治疗等级 = 1;
        public int 宠物技能 = 1, 宠物吸血 = 2;



        public void script()
        {
            try
            {
                switch (scriptIndex)
                {
                    case -1:
                        break;

                    //无脚本跟随（纯战斗模式）
                    case 0:
                        SysFunction.Log("无脚本模式");
                        无脚本模式();
                        break;
                    //东门哥布林
                    case 1:
                        SysFunction.Log("东门挂哥布林脚本");
                        挂东门哥布林();
                        break;
                    //东门大树（不过桥）
                    case 2:
                        SysFunction.Log("东门挂大树（不过桥）脚本");
                        挂东门大树不过桥();
                        break;
                    //东门大树（过桥）
                    case 3:
                        SysFunction.Log("东门挂大树（过桥）脚本");
                        挂东门大树过桥();
                        break;
                    //挂阿村
                    case 4:
                        SysFunction.Log("挂阿村脚本");
                        挂阿村();
                        break;
                    //鉴定鱼
                    case 5:
                        SysFunction.Log("鉴定鱼脚本");
                        鉴定鱼();
                        break;
                    //手动高速制造
                    case 6:
                        SysFunction.Log("高速制造");
                        手动制造();
                        break;
                    case 7:
                        SysFunction.Log("挖铁矿");
                        挖铁矿();
                        break;
                    case 8:
                        SysFunction.Log("刷屏");
                        刷屏();
                        break;
                    case 9:
                        SysFunction.Log("挂机采集");
                        挂机采集();
                        break;
                    case 10:
                        SysFunction.Log("测试");
                        test();
                        break;

                }
                //挂阿村();
            }
            catch (Exception ex)
            {
                SysFunction.Log("脚本终止：原因" + ex.Message);
            }
        }

        private void 挂机采集()
        {
            SysFunction.Warning();
            开始采集();
            SysFunction.Log("采集结束");
            SysFunction.Warning();
            登出游戏();
        }
        void test()
        {

        }
        //自定义模块组合
        void 刷屏()
        {
            while (检测背包空位() < 20)
            {
                if (刷屏信息.Length > 0)
                {
                    Thread.Sleep(300);
                    GameHookHandle.dm.SendString2((int)hwnd, 刷屏信息);
                    SysFunction.Log("发送信息：" + 刷屏信息);
                    Thread.Sleep(300);
                    GameHookHandle.dm.KeyPress(13);
                    Thread.Sleep(2500);
                }
            }
            登出游戏();
        }
        void 挖铁矿()
        { //object x, y;
            //GameHookHandle.dm.FindPic(0, 0, 800, 600, "加号.bmp", "020202", 0.8, 0, out x, out y);
            //GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
            //Thread.Sleep(200);
            //GameHookHandle.dm.LeftDown();
            //Thread.Sleep(1200);
            //GameHookHandle.dm.LeftUp();
            while (true)
            {
                西医补魔();
                走出西门();
                走向矿洞挖铁();
                开始采集();
                西门传送去换铁矿();

                //对话换矿();
                去银行();
                对话存东西();
            }

        }
        void 手动制造()
        {

        }
        void 鉴定鱼()
        {
            while (true)
            {

                DateTime begTime = DateTime.Now;
                //记录成功次数
                int m = 0;
                //记录失败次数
                int n = 0;
                //记录时间
                TimeSpan ts = new TimeSpan(0);
                while ((m + n) <= 35 && 人物魔值检测() == 1)
                {
                    Random r = new Random();
                    int delay = r.Next(4000) + 1000;
                    //Thread.Sleep(delay);
                    //先问服务员要鱼？
                    GameHookHandle.dm.MoveTo(25, 420);
                    Thread.Sleep(100);
                    GameHookHandle.dm.RightClick();
                    Thread.Sleep(1000);
                    //对话选择“是”
                    object x = 0, y = 0;
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                    while ((int)x <= 0 || (int)y <= 0)
                    {
                        GameHookHandle.dm.MoveToEx(400, 80, 50, 20);
                        Thread.Sleep(1000);
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                    }
                    GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);

                    //判断拿到鱼了 
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "拿到鱼.bmp", "020202", 0.8, 0, out x, out y);
                    while ((int)x <= 0 || (int)y <= 0)
                    {
                        Thread.Sleep(1000);
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "拿到鱼.bmp", "020202", 0.8, 0, out x, out y);
                    }
                    Thread.Sleep(1000);

                    //打开鉴定技能栏-等级
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能名称.bmp", "020202", 0.8, 0, out x, out y);
                    while ((int)x <= 0 || (int)y <= 0)
                    {
                        GameHookHandle.dm.MoveToEx(115, 470, 10, 3);
                        Thread.Sleep(200);
                        GameHookHandle.dm.LeftClick();
                        Thread.Sleep(800);
                        object x_职业 = 0, y_职业 = 0;
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "职业.bmp", "020202", 0.8, 0, out x_职业, out y_职业);
                        Thread.Sleep(100);
                        GameHookHandle.dm.MoveTo((int)x_职业 + 20, (int)y_职业 + 40);
                        Thread.Sleep(200);
                        GameHookHandle.dm.LeftClick();
                        Thread.Sleep(500);
                        GameHookHandle.dm.MoveTo((int)x_职业 + 20, (int)y_职业 + 40);
                        Thread.Sleep(200);
                        GameHookHandle.dm.LeftClick();
                        Thread.Sleep(500);
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能名称.bmp", "020202", 0.8, 0, out x, out y);
                    }
                    //点击鉴定技能坐标
                    GameHookHandle.dm.MoveTo((int)x + 20, (int)y + 40);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(500);
                    //判断物品栏有没有鱼
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品栏.bmp", "020202", 0.8, 0, out x, out y);
                    while ((int)x <= 0 || (int)y <= 0)
                    {
                        Thread.Sleep(300);
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品栏.bmp", "020202", 0.8, 0, out x, out y);
                    }
                    object x_鱼 = 0, y_鱼 = 0;
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品栏-鱼.bmp", "020202", 0.8, 0, out x_鱼, out y_鱼);
                    if ((int)x_鱼 <= 0 || (int)y_鱼 <= 0)
                    {
                        SysFunction.Log("物品栏里没有鱼，重新来！");
                        continue;
                    }
                    object x_执行 = 0, y_执行 = 0;
                    while (true)
                    {
                        //鉴定
                        //开始鉴定，双击鱼的图标
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "选中鱼.bmp", "020202", 0.8, 0, out x, out y);
                        while ((int)x <= 0 || (int)y <= 0)
                        {
                            GameHookHandle.dm.MoveTo((int)x_鱼, (int)y_鱼);
                            Thread.Sleep(200);
                            GameHookHandle.dm.LeftDoubleClick();
                            Thread.Sleep(800);
                            //清空鼠标
                            GameHookHandle.dm.MoveToEx(500, 100, 10, 10);
                            Thread.Sleep(200);
                            GameHookHandle.dm.FindPic(0, 0, 800, 600, "选中鱼.bmp", "020202", 0.8, 0, out x, out y);
                        }
                        if (getMP() < 10)
                        {
                            SysFunction.Log("没蓝了！");
                            ts = begTime - DateTime.Now;
                            SysFunction.Log("成功次数：" + m + "  失败次数：" + n + "  总计经过了 ：" + ts.Minutes + " 分钟");
                            return;
                        }
                        //选择鉴定执行
                        Thread.Sleep(500);
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "执行.bmp", "020202", 0.8, 0, out x, out y);
                        while ((int)x > 0 || (int)y > 0)
                        {
                            x_执行 = x;
                            y_执行 = y;
                            //  GameHookHandle.dm_wg.MoveTo(3, 3);
                            GameHookHandle.dm.MoveTo((int)x, (int)y);
                            SysFunction.Log(x + "," + y);

                            delay = r.Next(2000) + 1000;
                            // Thread.Sleep(delay);
                            Thread.Sleep(200);
                            GameHookHandle.QuickIdentify();
                            //GameHookHandle.dm.LeftClick();
                            // Thread.Sleep(delay);
                            清空鼠标();
                            //GameHookHandle.dm.MoveToEx(500, 200, 50, 20);
                            Thread.Sleep(200);
                            GameHookHandle.dm.FindPic(0, 0, 800, 600, "执行.bmp", "020202", 0.8, 0, out x, out y);
                        }
                        //清空鼠标
                        GameHookHandle.dm.MoveToEx(500, 100, 10, 5);
                        //开始鉴定
                        SysFunction.Log("开始鉴定");

                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "鉴定结束.bmp", "020202", 0.8, 0, out x, out y);
                        while ((int)x <= 0 || (int)y <= 0)
                        {
                            Thread.Sleep(1000);
                            SysFunction.Log("鉴定中");
                            GameHookHandle.dm.FindPic(0, 0, 800, 600, "鉴定结束.bmp", "020202", 0.8, 0, out x, out y);
                        }

                        //鉴定结束判断是否成功
                        Thread.Sleep(500);
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "选中鱼.bmp", "020202", 0.8, 0, out x, out y);
                        if ((int)x > 0 && (int)y > 0)
                        {
                            //清空鼠标
                            SysFunction.Log("未鉴定成功！重来一次");

                            GameHookHandle.dm.MoveToEx((int)x_执行, (int)y_执行, 10, 3);
                            Thread.Sleep(200);
                            GameHookHandle.dm.LeftClick();
                            Thread.Sleep(500);
                            n++;
                            continue;

                        }
                        break;
                    }

                    SysFunction.Log("鉴定成功！下一张牌！");
                    m++;
                    ts = DateTime.Now - begTime;
                    SysFunction.Log("成功次数：" + m + "  失败次数：" + n + "  总计经过了 ：" + ts.Minutes + " 分" + ts.Seconds + " 秒");
                    GameHookHandle.dm.MoveTo((int)x_鱼, (int)y_鱼);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    //清空鼠标
                    GameHookHandle.dm.MoveToEx((int)x, (int)y - 100, 20, 5);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);

                }
                关闭所有窗口();
                object x1, y1;
                int wait = 0;

                移动(8, 13);
                移动(4, 13);
                Move(2, 1);
                Thread.Sleep(1000);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x1, out y1);

                while (((int)x1 <= 0 || (int)y1 <= 0) && wait < 10)
                {
                    Thread.Sleep(300);
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x1, out y1);
                    wait++;
                    SysFunction.Log("等了：" + wait + "次");
                }

                移动(216, 53);
                移动(215, 54);
                移动(214, 59);
                移动(214, 65);
                移动(214, 71);
                移动(214, 77);
                移动(214, 83);
                移动(221, 84);
                Move(3, 1);
                Thread.Sleep(3000);

                东医补魔(false, true);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x1, out y1);

                while (((int)x1 <= 0 || (int)y1 <= 0) && wait < 10)
                {
                    Thread.Sleep(300);
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x1, out y1);
                    wait++;
                    SysFunction.Log("等了：" + wait + "次");
                }
                移动(221, 83);
                移动(214, 83);
                移动(214, 77);
                移动(214, 71);
                移动(214, 65);
                移动(214, 59);
                移动(215, 54);
                移动(216, 53);
                Move(0, 1);
                Thread.Sleep(4000);

                移动(4, 13);
                移动(12, 14);
            }
        }
        void 挂东门大树过桥()
        {
            int i = 0;
            while (true)
            {
                i++;
                SysFunction.Log("第" + i + "次挂机开始！开始时间：" + DateTime.Now.ToShortTimeString());
                西医补魔卖石头();
                走出东门();
                走向大树挂机点();
                开始挂机();

            }
        }
        void 挂阿村()
        {
            int i = 0;
            while (true)
            {
                i++;
                SysFunction.Log("第" + i + "次挂机开始！开始时间：" + DateTime.Now.ToShortTimeString());
                阿村卖石头补魔打卡();
                阿村出南门到挂机点();
                阿村挂机();

            }
        }
        void 无脚本模式()
        {
            while (true)
            {
                战斗过程();
            }
        }
        void 挂东门大树不过桥()
        {
            int i = 0;
            while (true)
            {
                i++;
                SysFunction.Log("第" + i + "次挂机开始！开始时间：" + DateTime.Now.ToShortTimeString());
                西医补魔卖石头();
                走出东门();
                走向大树不过桥挂机点();
                开始挂机();

            }

        }
        void 挂东门哥布林()
        {
            int i = 0;
            while (true)
            {
                i++;
                SysFunction.Log("第" + i + "次挂机开始！开始时间：" + DateTime.Now.ToShortTimeString());
                西医补魔卖石头();
                走出东门();
                走向东门哥布林挂机点();
                开始挂机();

            }

        }

        void 走向大树挂机点()
        {
            isBack = false;
            GameHookHandle.OpenGaosuYidong();
            SysFunction.Log("出发去挂机咯！");
            城外移动(480, 199);
            城外移动(486, 202);
            城外移动(492, 205);
            城外移动(498, 208);
            城外移动(503, 212);
            城外移动(506, 218);
            城外移动(509, 224);
            城外移动(514, 228);
            城外移动(519, 232);
            城外移动(525, 233);
            城外移动(530, 233);
            城外移动(535, 234);
            城外移动(541, 235);
            城外移动(547, 236);
            城外移动(553, 233);
            城外移动(559, 233);
            城外移动(565, 233);
            城外移动(570, 234);
            城外移动(574, 234);
            城外移动(582, 235);
            城外移动(586, 231);
            城外移动(590, 228);
            城外移动(596, 228);
            城外移动(601, 224);
            城外移动(605, 220);
            城外移动(605, 221);

            Thread.Sleep(3000);
            SysFunction.Log("到达挂机点");
        }
        void 走向大树不过桥挂机点()
        {
            isBack = false;
            GameHookHandle.OpenGaosuYidong();
            SysFunction.Log("出发去挂机咯！");
            城外移动(479, 192);
            城外移动(482, 188);
            城外移动(486, 184);
            城外移动(490, 180);
            城外移动(494, 176);
            城外移动(498, 174);
            城外移动(500, 172);
            城外移动(505, 176);
            城外移动(510, 179);
            城外移动(515, 175);
            城外移动(518, 173);
            城外移动(520, 172);
            城外移动(524, 169);
            城外移动(527, 172);
            城外移动(532, 173);
            城外移动(535, 175);
            城外移动(537, 177);
            城外移动(542, 173);
            城外移动(546, 170);
            城外移动(551, 174);
            城外移动(556, 172);
            城外移动(561, 176);
            城外移动(565, 181);
            城外移动(562, 187);

            Thread.Sleep(3000);
            SysFunction.Log("到达挂机点");
            //if (isBack == false)
            //{
            //    背包扔垃圾();
            //    关闭背包();
            //}
        }
        void 走向东门哥布林挂机点()
        {
            isBack = false;
            GameHookHandle.OpenGaosuYidong();
            SysFunction.Log("出发去挂机咯！");
            城外移动(479, 192);
            城外移动(482, 188);
            城外移动(486, 184);
            城外移动(490, 180);
            城外移动(494, 176);
            城外移动(498, 174);
            城外移动(500, 172);
            城外移动(505, 176);
            城外移动(510, 179);

            城外移动(510, 176);

            Thread.Sleep(3000);
            SysFunction.Log("到达挂机点");

        }

        void 走向矿洞挖铁()
        {
            isBack = false;
            //GameHookHandle.OpenGaosuYidong();
            SysFunction.Log("出发去挂机咯！");
            城外移动(374, 195);
            城外移动(371, 189);
            城外移动(368, 182);
            城外移动(368, 182);
            城外移动(368, 177);
            城外移动(369, 171);
            城外移动(368, 165);

            城外移动(363, 161);
            城外移动(358, 157);
            城外移动(356, 150);
            城外移动(351, 146);
            Move(3, 1);
            Thread.Sleep(2000);
            //过图进矿洞

            if (!isBack)
            {
                移动(12, 29);
                移动(18, 32);
                移动(23, 33);
                移动(27, 33);
                移动(27, 27);
                移动(22, 23);
                Move(3, 1);
                //过图下楼
                Thread.Sleep(2000);
                GameHookHandle.OpenGaosuYidong();
                移动(10, 25);
                移动(16, 27);
                移动(19, 24);
                移动(22, 21);
                对话北();
                Thread.Sleep(2000);
                移动(22, 19);
                移动(23, 14);
                Move(3, 1);
                Thread.Sleep(2000);
            }
            城外移动(35, 31);
            城外移动(34, 32);
            城外移动(32, 32);
            城外移动(26, 31);
            城外移动(21, 29);
            城外移动(19, 27);
            城外移动(20, 23);
            城外移动(23, 19);
            城外移动(24, 13);
            城外移动(24, 7);
            城外移动(28, 3);
            Thread.Sleep(500);
            城外移动(28, 3);
            Thread.Sleep(500);
            城外移动(28, 3);
            if (!isBack)
            {
                Move(0, 1);

                Thread.Sleep(3000);
                GameHookHandle.CloseGaosuYidong();
                SysFunction.Log("到达挂机点");
                背包扔垃圾(true);
                关闭背包();
            }
        }
        //基础模块
        void 阿村卖石头补魔打卡()
        {
            GameHookHandle.CloseYuandi();

            背包扔垃圾();
            关闭背包();
            移动(16, 18);

            清空鼠标();
            关闭小地图();
            清空文字();
            object x = 0, y = 0;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "达文.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                SysFunction.Log("找达文中……");
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "达文.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y + 30, 5, 2);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(1200);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "达文2.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 20)
            {
                对话北();
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "达文2.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx(320 + px, 305 + py, 5, 2);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(800);
            //打开贩卖大框！
            对话卖魔石(ref x, ref y);
            //出卡片屋去补魔
            移动(16, 18);
            移动(18, 23);
            移动(18, 28);
            移动(18, 31);
            Move(1, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }

            移动(162, 156);
            移动(167, 157);
            移动(171, 162);
            移动(168, 167);
            移动(168, 175);
            移动(173, 179);
            移动(178, 182);
            移动(178, 189);
            移动(178, 195);
            移动(178, 203);
            移动(179, 211);
            移动(187, 211);
            移动(191, 211);
            移动(192, 209);
            Move(3, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "冒险者旅馆.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "冒险者旅馆.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }
            int 坐标X, 坐标Y;
            string 卡时 = "";
            //打卡
            if (isWork && 判断卡时(out 卡时))
            {
                SysFunction.Log("需要打卡且卡时足够！卡时为：" + 卡时);
                移动(8, 18);
                移动(10, 12);
                对话东();
                对话打卡();
                移动(8, 18);
            }
            //补魔
            移动(8, 18);
            移动(15, 18);
            移动(20, 17);
            移动(24, 17);
            对话北();
            对话补魔();
            Thread.Sleep(300);
            背包扔垃圾();
            关闭背包();

            移动(20, 17);
            移动(15, 18);
            移动(8, 18);
            移动(4, 22);
            Move(1, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }
            移动(192, 209);
            移动(191, 211);
            移动(187, 211);
            移动(179, 211);

        }
        void 阿村出南门到挂机点()
        {
            GameHookHandle.OpenGaosuYidong();
            移动(179, 211);
            移动(179, 216);
            移动(179, 222);
            移动(178, 226);
            Move(1, 1);
            object x, y;
            int n = 0;
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村出城.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村出城.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }

            移动(203, 346);
            移动(198, 348);
            移动(191, 349);
            Thread.Sleep(3000);
            SysFunction.Log("到达挂机点");
        }
        void 阿村挂机()
        {
            //如果该回城了，则停止挂机
            if (isBack == true)
            {
                return;
            }
            GameHookHandle.OpenYuandi();
            while (人物血线检测() == 1 && 检测背包空位() > 0)
            {
                战斗过程();
            }
            Thread.Sleep(200);
            GameHookHandle.CloseGaosuYidong();
            Thread.Sleep(200);
            GameHookHandle.CloseYuandi();
            阿村回城();
        }
        void 阿村回城()
        {
            城外移动(191, 349, false);
            城外移动(198, 348, false);
            城外移动(203, 146, false);
            城外移动(203, 145, false);

            //Move(3, 1);
            object x, y;
            int n = 0;
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "阿村.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }

            移动(178, 226);
            移动(179, 222);
            移动(179, 216);
            移动(179, 211);
            移动(178, 203);
            移动(178, 195);
            移动(178, 189);
            移动(178, 182);
            移动(173, 179);
            移动(168, 175);
            移动(168, 167);
            移动(171, 162);
            移动(167, 157);
            移动(162, 156);

            Move(3, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "卡片屋.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "卡片屋.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }
            Thread.Sleep(500);
            移动(16, 18);
            移动(18, 23);
            移动(18, 28);
            移动(18, 31);

        }

        void 走出西门()
        {
            int 坐标X, 坐标Y;
            string 卡时 = "";
            if (isWork && 判断卡时(out 卡时))
            {
                SysFunction.Log("需要打卡且卡时足够！卡时为：" + 卡时);
                西门打卡回西门传送();
            }
            else
            {
                SysFunction.Log("不需要打卡或卡时不足！卡时为：" + 卡时);
                坐标X = 固定X坐标();
                坐标Y = 固定Y坐标();
                if (坐标X != 242 && 坐标Y != 100)
                {
                    到西门传送石();
                }

            }
            移动(59, 83);
            移动(55, 87);
            移动(48, 88);
            移动(41, 88);
            移动(34, 88);
            移动(27, 88);
            移动(23, 88);
            Move(2, 1);
            清空鼠标();
            object x = 0, y = 0;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "芙蕾亚.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {

                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "芙蕾亚.bmp", "020202", 0.8, 0, out x, out y);
                n++;
            }
            Thread.Sleep(300);
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                GameHookHandle.dm.MoveToEx(567, 39, 4, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            }
        }
        void 走出东门()
        {
            int 坐标X, 坐标Y;
            string 卡时 = "";
            if (isWork && 判断卡时(out 卡时))
            {
                SysFunction.Log("需要打卡且卡时足够！卡时为：" + 卡时);
                西门打卡回西门传送();
                到东门传送石();
            }
            else
            {
                SysFunction.Log("不需要打卡或卡时不足！卡时为：" + 卡时);
                坐标X = 固定X坐标();
                坐标Y = 固定Y坐标();
                if (坐标X != 242 && 坐标Y != 100)
                {
                    到东门传送石();
                }

            }

            移动(245, 94);
            移动(248, 89);
            移动(252, 88);
            移动(257, 88);
            移动(262, 88);
            移动(267, 88);
            移动(272, 88);
            移动(277, 88);
            移动(280, 88);
            Move(0, 1);
            清空鼠标();
            object x = 0, y = 0;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "芙蕾亚.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {

                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "芙蕾亚.bmp", "020202", 0.8, 0, out x, out y);
                n++;
            }
            Thread.Sleep(300);
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                GameHookHandle.dm.MoveToEx(567, 39, 4, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            }
        }
        void 东医补魔(bool isExpert = false, bool isBack = false)
        {
            object x, y;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "医院.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "医院.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }
            if (isExpert)
            {

            }
            else
            {
                移动(12, 42);
                移动(10, 36);
                移动(8, 31);
            }
            对话北();
            对话补魔();
            Thread.Sleep(300);

            if (isBack)
            {
                移动(8, 31);
                移动(10, 36);
                移动(12, 41);
                Move(1, 1);
                Thread.Sleep(2000);
            }
            else
            {

            }
        }

        void 西门打卡回西门传送()
        {
            到西门传送石();
            移动(63, 79);
            移动(67, 76);
            移动(69, 70);
            移动(72, 65);
            移动(73, 61);
            Move(3, 1);
            清空鼠标();

            object x = 0, y = 0;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "打卡.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "打卡.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }
            移动(9, 24);
            移动(7, 18);
            移动(7, 12);
            移动(11, 10);
            对话北();
            对话打卡();

            移动(11, 10);
            移动(7, 12);
            移动(7, 18);
            移动(9, 23);

            Move(1, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x, out y);
                n++;
            }
            SysFunction.Log("等了：" + n + "次");
            移动(73, 61);
            移动(72, 65);
            移动(69, 70);
            移动(67, 76);
            移动(63, 79);


        }
        void 西医补魔卖石头(bool isExpert = false)
        {
            int n;
            object x;
            object y;
            西医补魔();
            if (检测背包空位() != 20)
            {
                背包扔垃圾();
                关闭背包();
            }
            int 空格 = 检测背包空位();
            if (空格 == 20)
            {
                SysFunction.Log("包是空的，先不清理");
                return;
            }
            移动(10, 37);
            移动(12, 41);
            Move(1, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "法兰城.bmp", "020202", 0.8, 0, out x, out y);
                n++;
            }
            SysFunction.Log("等了：" + n + "次");
            移动(82, 83);
            移动(86, 84);
            移动(88, 81);
            移动(92, 79);
            移动(93, 78);
            Move(0, 1);
            清空鼠标();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "达美店.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "达美店.bmp", "020202", 0.8, 0, out x, out y);
                n++;
            }
            SysFunction.Log("等了：" + n + "次");

            移动(10, 14);
            移动(15, 14);
            移动(17, 14);
            达美卖魔石();

        }

        void 西医补魔(bool isExpert = false)
        {
            int n;
            object x;
            object y;
            到西门传送石();
            // 登出();
            n = 0;
            移动(68, 84);
            移动(76, 84);
            移动(82, 84);
            Move(3, 1);
            清空鼠标();
            x = 0;
            y = 0;

            GameHookHandle.dm.FindPic(0, 0, 800, 600, "医院.bmp", "020202", 0.8, 0, out x, out y);
            n = 0;
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(300);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "医院.bmp", "020202", 0.8, 0, out x, out y);
                n++;
                SysFunction.Log("等了：" + n + "次");
            }
            if (isExpert)
            {

            }
            else
            {
                移动(10, 37);
                移动(9, 32);
            }
            对话北();
            对话补魔();
            Thread.Sleep(300);
        }
        void 达美卖魔石()
        {
            //要进入法兰城达美的店，并站在17,14的位置进行扫描
            清空鼠标();
            关闭小地图();
            清空文字();
            if (检测背包空位() != 20)
            {
                背包扔垃圾();
                关闭背包();
            }
            //找达美并走向她
            object x = 0, y = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "达美.bmp|达美2.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                SysFunction.Log("找达美中……");
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "达美.bmp|达美2.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x + 32, (int)y + 24, 5, 2);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(1200);
            清空鼠标();
            //打开NPC对话框并点击卖
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "卖石头NPC.bmp|达美3.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                对话东();
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "卖石头NPC.bmp|达美3.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx(320 + px, 305 + py, 5, 2);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(800);
            对话卖魔石(ref x, ref y);
        }
        void 去银行()
        {
            到东门传送石();

            移动(242, 100);
            移动(238, 104);
            移动(234, 107);
            移动(237, 111);
            Move(0, 1);
            //过图
            Thread.Sleep(3000);
            移动(2, 13);
            移动(7, 9);
            移动(11, 8);
            对话东();
        }
        void 西门传送去换铁矿()
        {
            到西门传送石();
            int r = new Random().Next(1, 10);

            if (r >= 5)
            {
                移动(68, 84);
                移动(76, 84);
                移动(82, 84);

                移动(86, 84);
                移动(88, 81);
                移动(92, 79);
                移动(92, 73);
                移动(95, 68);
                移动(100, 67);
                移动(104, 63);
                移动(106, 62);
            }
            else
            {
                移动(66, 75);
                移动(69, 70);
                移动(72, 67);

                移动(80, 67);
                移动(88, 67);
                移动(96, 67);
                移动(104, 67);
                移动(106, 62);
            }


            Move(3, 1);
            Thread.Sleep(3000);
            移动(26, 24);
            移动(26, 18);
            移动(26, 12);
            移动(28, 6);


            对话北();
            对话换矿();


        }
        void 开始挂机()
        {
            //如果该回城了，则停止挂机
            if (isBack == true)
            {
                return;
            }
            GameHookHandle.OpenYuandi();
            while (人物血线检测() == 1 && 检测背包空位() > 0)
            {

                战斗过程(true);
            }
            Thread.Sleep(200);
            GameHookHandle.CloseGaosuYidong();
            Thread.Sleep(200);
            GameHookHandle.CloseYuandi();
        }
        //进阶函数
        void 城外移动(int tarX, int tarY, bool Logout = true)
        {
            //如果该回城了，且可以登出，则放弃所有城外移动的执行
            if (isBack && Logout)
            {
                return;
            }
            int 向东格子数, 向南格子数, 屏幕坐标X, 屏幕坐标Y;
            int 坐标X = 0, 坐标Y = 0;
            //行进过程中，需要回城的时候
            //如果可以登出（Logout为真）则移动结束
            //如果不可以登出（Logout为假）则继续移动，结合走回城的坐标脚本
            while ((tarX != 坐标X || tarY != 坐标Y) && (!isBack && Logout || !Logout))
            {
                坐标X = 固定X坐标();
                坐标Y = 固定Y坐标();
                向东格子数 = tarX - 坐标X;
                向南格子数 = tarY - 坐标Y;
                if (Math.Abs(向东格子数) > 50 || Math.Abs(向南格子数) > 40)
                {
                    SysFunction.Log("不可能到达的坐标！有可能已经发生切图。");
                    return;
                }
                if (向东格子数 > 5 && 向南格子数 < -4 || 向南格子数 < -5)
                {
                    SysFunction.Log("计划坐标：" + tarX + "," + tarY + "，当前坐标：" + 坐标X + "," + 坐标Y
                        + "！超出范围修正中……");
                    城外移动((坐标X + tarX) / 2, (坐标Y + tarY) / 2);
                    坐标X = 固定X坐标();
                    坐标Y = 固定Y坐标();
                    向东格子数 = tarX - 坐标X;
                    向南格子数 = tarY - 坐标Y;
                }
                屏幕坐标X = 320 + px + 向东格子数 * 32 + 向南格子数 * 32;
                屏幕坐标Y = 260 + py - 向东格子数 * 24 + 向南格子数 * 24;
                int count = 0;
            go:
                GameHookHandle.dm.MoveTo(屏幕坐标X, 屏幕坐标Y);
                Thread.Sleep(150);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(300);
                清空鼠标();
                object x, y;
                int wait = 0;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out x, out y);
                while ((int)x > 0 && (int)y > 0)
                {
                    坐标X = 固定X坐标();
                    坐标Y = 固定Y坐标();
                    if (坐标X == tarX && 坐标Y == tarY)
                    {
                        return;
                    }
                    if (wait > 20)
                    {
                        break;
                    }
                    Thread.Sleep(50);
                    wait++;
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out x, out y);
                }
                if (wait > 20 && count < 10)
                {
                    SysFunction.Log("等等等！");
                    屏幕坐标X = 屏幕坐标X + 4;
                    屏幕坐标Y = 屏幕坐标Y + 16;
                    count++;
                    goto go;
                    //isBack = true;
                    //continue;
                }
                else if (count >= 10)
                {
                    SysFunction.Log("等待次数过多！该回城了！");
                    isBack = true;
                    return;
                }
                else
                {
                    isBack = 战斗过程();
                }
            }
        }
        /// <summary>
        /// 战斗过程
        /// </summary>
        /// <returns>返回真，表示要回城；返回假，表示仍继续</returns>
        bool 战斗过程(bool isGuaji = false)
        {
            object x, y, intX, intY;
            bool isFight = false, isFront = false;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp", "020202", 0.8, 0, out x, out y);
            //表示一直处于战斗中
            while ((int)x <= 0 && (int)y <= 0)
            {
                if (!isFight)
                {
                    清空鼠标();
                    SysFunction.Log("进入新的战斗！开始判断被偷袭");
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "被偷袭.bmp", "020202", 0.8, 0, out x, out y);
                    int n = 0;
                    while (((int)x < 0 && (int)y < 0) && n < 10)
                    {
                        Thread.Sleep(200);
                        n++;
                        GameHookHandle.dm.FindPic(0, 0, 800, 600, "被偷袭.bmp", "020202", 0.8, 0, out x, out y);
                    }
                    if (((int)x > 0 && (int)y > 0) || n < 10)
                    {
                        isFront = true;
                        SysFunction.Log("被偷袭了，人宠交换位置！");
                    }
                    else
                    {
                        SysFunction.Log("没有被偷袭！");

                    }
                    isFight = true;
                }

                //进入战斗回合
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家指令.bmp", "020202", 0.8, 0, out x, out y);
                while ((int)x > 0 && (int)y > 0)
                {
                    isFight = true;
                    SysFunction.Log("进入新的回合！");
                    //if ((人物血线检测() == 0) && (人物魔线检测() == 0))
                    //{
                    //    SysFunction.Log("战斗内，《血线魔线》不通过，打完回城！");
                    //    GameHookHandle.CloseYuandi();
                    //    //return true;
                    //}
                    if ((人物血值检测() == 0) && (人物魔值检测() == 0))
                    {
                        SysFunction.Log("战斗内，《血值魔值》不通过，打完回城！");
                        GameHookHandle.CloseYuandi();
                        //return true;
                    }
                    if (检测背包空位() == 0)
                    {
                        SysFunction.Log("战斗内，《背包已满》，打完扔东西！");
                        GameHookHandle.CloseYuandi();

                    }
                    DateTime dtBegin = DateTime.Now;
                    if (!人物补血检测() && isAidIn)
                    {
                        人物放技能(补血技能, 补血等级, 1, isFront);
                    }
                    else
                    {
                        人物普通攻击();
                    }

                    TimeSpan ts = DateTime.Now - dtBegin;


                    if ((宠物补血检测() != 1) && (宠物魔检测() == 1))
                    {
                        SysFunction.Log("宠物吸血！");

                        宠物放技能(宠物吸血);
                    }
                    else
                    {
                        宠物放技能();
                    }
                    Thread.Sleep(2000);
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家指令.bmp", "020202", 0.8, 0, out x, out y);
                }
                //回合外的判断
                //if ((人物血线检测() == 0) && (人物魔线检测() == 0))
                //{
                //    SysFunction.Log("战斗内，《血线魔线》不通过，打完回城！");
                //    GameHookHandle.CloseYuandi();
                //    //return true;
                //}
                if ((人物血值检测() == 0) && (人物魔值检测() == 0))
                {
                    SysFunction.Log("战斗内，《血值魔值》不通过，打完回城！");
                    GameHookHandle.CloseYuandi();
                    //return true;
                }
                if (检测背包空位() == 0)
                {
                    SysFunction.Log("战斗内，《背包已满》，打完扔东西！");
                    GameHookHandle.CloseYuandi();

                }
                if (!人物补血检测() && isAidOut)
                {
                    SysFunction.Log("战斗内，《背包已满》，打完扔东西！");
                    GameHookHandle.CloseYuandi();
                }
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp", "020202", 0.8, 0, out x, out y);
            }
            //战斗外血魔判断回城
            isFight = false;
            //返回假则继续
            SysFunction.Log("战斗外，血魔检测均通过，继续！");
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp", "020202", 0.8, 0, out x, out y);
            //观看结束场景动画
            int w = 0;
            while (((int)x <= 0 && (int)y <= 0) && w < 10)
            {
                Thread.Sleep(500);
                w++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp", "020202", 0.8, 0, out x, out y);
            }
            w = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "战斗结束.bmp", "020202", 0.7, 0, out x, out y);
            //观看结束场景动画
            while (((int)x < 0 || (int)y < 0) && w < 10)
            {
                Thread.Sleep(200);
                w++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "战斗结束.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x >= 0 && (int)y >= 0)
            {
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 50, 20);
                SysFunction.Log("点击战利品框！");
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(300);
            }

            if (人物血值检测() != 1 && 人物魔值检测() != 1)
            {
                //战斗结束返回真则回城
                SysFunction.Log("战斗结束，《人物血值》或《人物魔值》检测不通过，回城！");
                GameHookHandle.CloseYuandi();
                return true;
            }
            //战斗外捡东西
            else if (检测背包空位() == 0 && !isBack)
            {
                SysFunction.Log("背包已满，尝试打开背包扔垃圾！");
                GameHookHandle.CloseYuandi();
                Thread.Sleep(2000);
                背包扔垃圾();
                关闭背包();
                Thread.Sleep(200);
                if (检测背包空位() == 0)
                {
                    return true;
                }
                //开启原地
                if (isGuaji)
                    GameHookHandle.OpenYuandi();
            }
            //战斗外急救部分
            else if (!人物补血检测() && 人物魔值检测() == 1 && isAidOut)
            {
                SysFunction.Log("战斗结束，《人物血线》不通过，《人物魔线》通过，开始急救！");
                GameHookHandle.CloseYuandi();

                清空鼠标();
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp", "020202", 0.8, 0, out x, out y);
                //观看结束场景动画
                int wait = 0;
                while (((int)x <= 0 && (int)y <= 0) && wait < 10)
                {
                    Thread.Sleep(500);
                    wait++;
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp", "020202", 0.8, 0, out x, out y);
                }
                if (((int)x <= 0 && (int)y <= 0) && wait > 10 && !isBack && isGuaji)
                {
                    SysFunction.Log("等待战斗结束失败！放弃本次急救");
                    //开启原地
                    GameHookHandle.OpenYuandi();
                    return false;
                }

                while ((get_HPMAX() - getHP()) >= 30 * 急救等级 && isAidOut)
                {
                    if (人物魔值检测() == 0)
                    {
                        SysFunction.Log("《人物魔值》检测不通过，放弃急救，开始回城！");
                        return true;
                    }

                    人物用急救(急救技能, 急救等级, 0);
                }
                //如果是在回城路上，就不再开启原地了！
                if (!isBack && isGuaji)
                {
                    GameHookHandle.OpenYuandi();
                    return false;
                }
                else
                {
                    GameHookHandle.CloseYuandi();
                    return true;
                }
            }
            //如果是在回城路上，就不再开启原地了！
            if (!isBack && isGuaji)
            {
                GameHookHandle.OpenYuandi();
            }
            else
            {
                GameHookHandle.CloseYuandi();
            }
            return false;

        }
        void 人物普通攻击()
        {
            清空鼠标();
            清空文字();
            object x, y;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家攻击.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x < 0 || (int)y < 0) && n < 10)
            {
                GameHookHandle.dm.MoveToEx(388 + px, 54 + py, 10, 3);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(300);
                清空鼠标();
                Thread.Sleep(300);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家攻击.bmp", "020202", 0.8, 0, out x, out y);
            }
            Thread.Sleep(500);
            正向选怪();
        }
        /// <summary>
        /// 人物放技能
        /// </summary>
        /// <param name="技能"></param>
        /// <param name="等级"></param>
        /// <param name="目标">0是选怪，1是选自己，2是选宠物</param>
        void 人物放技能(int 技能 = 1, int 等级 = 1, int 目标 = 0, bool 前排 = false)
        {
            object x, y;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能框.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x < 0 && (int)y < 0) && n < 10)
            {
                GameHookHandle.dm.MoveToEx(460 + px, 50 + py, 10, 3);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(300);
                清空鼠标();
                Thread.Sleep(300);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能框.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x < 0 && (int)y < 0)
            {
                SysFunction.Log("没找到技能框，只能普通攻击");
                return;
            }
            GameHookHandle.dm.MoveToEx((int)x - 40, (int)y + 35 + 15 * 技能, 4, 2);
            Thread.Sleep(150);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);

            n = 等级;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能等级.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x > 0 && (int)y > 0) && n >= 1)
            {

                GameHookHandle.dm.MoveToEx((int)x - 40, (int)y + 30 + 15 * 等级, 4, 2);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                清空鼠标();
                Thread.Sleep(300);
                n--;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能等级.bmp", "020202", 0.8, 0, out x, out y);
            }
            if (n < 1 && (int)x > 0 && (int)y > 0)
            {
                SysFunction.Log("人物技能选择失败！进行普通攻击！");
                人物普通攻击();
                return;
            }
            if (目标 == 0)//0是选怪
            {
                正向选怪();
            }
            else if (目标 == 1)//1是选自己
            {
                if (前排)
                {
                    GameHookHandle.dm.MoveToEx(420 + px, 330 + py, 3, 2);
                }
                else
                {
                    GameHookHandle.dm.MoveToEx(480 + px, 365 + py, 3, 2);
                }
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                //480,350
            }
            else if (目标 == 2)//2是选宠物
            {
                if (前排)
                {
                    GameHookHandle.dm.MoveToEx(480 + px, 365 + py, 3, 2);
                }
                else
                {
                    GameHookHandle.dm.MoveToEx(420 + px, 330 + py, 3, 2);
                }
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                //420,325
            }

        }

        void 人物用急救(int 技能 = 1, int 等级 = 1, int 目标 = 0)
        {

            清空鼠标();
            object x, y;
            int n = 0, m = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "对象1.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 5)
            {
                object x_职业 = 0, y_职业 = 0;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "职业.bmp", "020202", 0.8, 0, out x_职业, out y_职业);
                while (((int)x_职业 <= 0 || (int)y_职业 <= 0) && m < 5)
                {
                    m = 0;
                    GameHookHandle.dm.MoveToEx(115, 470, 10, 3);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    清空鼠标();
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "职业.bmp", "020202", 0.8, 0, out x_职业, out y_职业);
                }
                Thread.Sleep(100);
                GameHookHandle.dm.MoveTo((int)x_职业 + 20, (int)y_职业 + 25 + 16 * 技能);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(800);
                GameHookHandle.dm.MoveTo((int)x_职业 + 20, (int)y_职业 + 25 + 16 * 等级);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(800);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "对象1.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x < 0 && (int)y < 0)
            {
                return;
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y - 100, 10, 2);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(800);
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "对象2.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 5)
            {
                Thread.Sleep(800);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "对象2.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x < 0 && (int)y < 0)
            {
                return;
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y - 130 + 目标 * 20, 10, 2);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(800);
            清空鼠标();
        }

        void 开始采集(int 技能 = 1)
        {
            //打开鉴定技能栏-等级
            GameHookHandle.OpenGaosuCaiji();
            object x, y;

            while (检测背包空位() > 0 && 人物魔值检测() > 0 && !isBack)
            {
                int n = 0;
                //清空鼠标();
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "工作状态.bmp", "020202", 0.8, 0, out x, out y);

                while (((int)x <= 0 || (int)y <= 0) && n <= 10)
                {
                    Thread.Sleep(500);
                    GameHookHandle.dm.MoveToEx(115, 470, 10, 3);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    object x_职业 = 0, y_职业 = 0;
                    清空鼠标();
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "职业.bmp", "020202", 0.8, 0, out x_职业, out y_职业);
                    Thread.Sleep(100);
                    GameHookHandle.dm.MoveTo((int)x_职业 + 20, (int)y_职业 + 25 + 16 * 技能);
                    Thread.Sleep(300);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    GameHookHandle.dm.MoveTo((int)x_职业 + 20, (int)y_职业 + 25 + 16 * 技能);
                    Thread.Sleep(300);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    n++;
                    清空鼠标();
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "工作状态.bmp", "020202", 0.8, 0, out x, out y);
                }
                if (((int)x <= 0 || (int)y <= 0) && n > 10)
                {
                    SysFunction.Log("无法开始挂机工作，出现问题");
                    return;
                }
                for (int i = 0; i < 5; i++)
                {
                    随机鼠标();
                    Thread.Sleep(600);

                }
                while ((受伤检测() != 1) && 人物魔值检测() > 0)
                {
                    人物用急救(治疗技能, 治疗等级, 0);
                }
            }
            // GameHookHandle.dm.FindPic(0, 0, 800, 600, "工作状态.bmp", "020202", 0.8, 0, out x, out y);

        }

        void 宠物放技能(int 宠物技能 = 1)
        {
            清空鼠标();
            清空文字();
            object x, y;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "宠物指令.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x < 0 && (int)y < 0) && n <= 10)
            {
                Thread.Sleep(300);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "宠物指令.bmp", "020202", 0.8, 0, out x, out y);
            }
            Thread.Sleep(200);
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能框.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x < 0 && (int)y < 0) && n <= 10)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.RightClick();
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能框.bmp", "020202", 0.8, 0, out x, out y);
                n++;
            }
            GameHookHandle.dm.MoveToEx((int)x - 40, (int)y + 35 + 15 * (宠物技能), 5, 2);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);

            Thread.Sleep(500);

            GameHookHandle.dm.FindPic(0, 0, 800, 600, "技能框.bmp", "020202", 0.8, 0, out x, out y);
            if ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx((int)x - 40, (int)y + 35 + 15, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }

            if (isHit)
            {
                正向选怪();
            }
            else
            {
                反向选怪();
            }
        }
        /// <summary>
        /// 后排优先
        /// </summary>
        void 正向选怪()
        {
            string[] mzb;
            int mx, my;
            for (int i = 0; i < 怪物固定坐标.Length; i++)
            {
                mzb = 怪物固定坐标[i].Split(',');
                mx = Convert.ToInt32(mzb[0]);
                my = Convert.ToInt32(mzb[1]);

                GameHookHandle.dm.MoveToEx(mx, my, 2, 2);
                Thread.Sleep(500);
                object x, y;
                GameHookHandle.dm.FindPic(mx, my, mx + 40, my + 40, "选中怪.bmp", "020202", 0.8, 0, out x, out y);
                if ((int)x > 0 && (int)y > 0)
                {
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(700);
                    //SysFunction.Log("选中了第" + (i + 1) + "个怪！");
                    清空鼠标();
                    return;
                }
                清空鼠标();

            }
            清空鼠标();

        }
        /// <summary>
        /// 前排优先
        /// </summary>
        void 反向选怪()
        {
            string[] mzb;
            int mx, my;
            for (int i = 怪物固定坐标.Length - 1; i >= 0; i--)
            {
                mzb = 怪物固定坐标[i].Split(',');
                mx = Convert.ToInt32(mzb[0]);
                my = Convert.ToInt32(mzb[1]);

                GameHookHandle.dm.MoveToEx(mx, my, 2, 2);
                Thread.Sleep(500);
                object x, y;
                GameHookHandle.dm.FindPic(mx, my, mx + 40, my + 40, "选中怪.bmp", "020202", 0.8, 0, out x, out y);
                if ((int)x > 0 && (int)y > 0)
                {
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(700);
                    // SysFunction.Log("选中了第" + (i + 1) + "个怪！");
                    清空鼠标();
                    return;
                }

            }
            清空鼠标();

        }


        //NPC对话交互
        void 对话卖魔石(ref object x, ref object y)
        {
            //打开贩卖大框！
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "商店.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "商店.bmp", "020202", 0.8, 0, out x, out y);
            }
            清空鼠标();
            //"全部"点不了，就取消
            Thread.Sleep(1000);
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "全部否.bmp", "020202", 0.8, 0, out x, out y);
            if ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔取消.bmp", "020202", 0.8, 0, out x, out y);
                while ((int)x <= 0 || (int)y <= 0)
                {
                    Thread.Sleep(500);
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔取消.bmp", "020202", 0.8, 0, out x, out y);
                }
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                return;
            }
            //点击全部
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "全部.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "全部.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
            //点击确定
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
            //点击是
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
        }
        void 对话打卡()
        {
            清空鼠标();
            object x = 0, y = 0;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "打卡对话.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n <= 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "打卡对话.bmp", "020202", 0.8, 0, out x, out y);
            }
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n <= 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n <= 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            }

            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
            清空鼠标();


        }
        void 对话换矿()
        {
            object x, y;
            清空鼠标();
            //打开贩卖大框！
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "商店.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                GameHookHandle.dm.MoveToEx(325 + px, 290 + py, 5, 5);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "商店.bmp", "020202", 0.8, 0, out x, out y);
            }
            清空鼠标();
            Thread.Sleep(1000);
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "加号.bmp", "020202", 0.8, 0, out x, out y);
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftDown();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "数量不够.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "数量不够.bmp", "020202", 0.8, 0, out x, out y);
            }
            Thread.Sleep(200);
            GameHookHandle.dm.LeftUp();
            if (((int)x > 0 && (int)y > 0) && n < 10)
            {
                //点击确定
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
                while ((int)x <= 0 || (int)y <= 0)
                {
                    Thread.Sleep(500);
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
                }
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                //点击是
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                while ((int)x <= 0 || (int)y <= 0)
                {
                    Thread.Sleep(500);
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                }
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }

        }
        void 对话补魔()
        {
            清空鼠标();
            object x = 0, y = 0;
            int n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            }

            随机鼠标();
            Thread.Sleep(500);
            GameHookHandle.dm.MoveToEx(320 + px, 220 + py, 5, 2);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
            清空鼠标();
            n = 0;
            if (get_MPMAX() != getMP())
            {
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                while ((int)x <= 0 || (int)y <= 0 && n < 10)
                {
                    Thread.Sleep(500);
                    n++;
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                }
                if ((int)x > 0 && (int)y > 0)
                {
                    GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(500);
                }
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            }
            随机鼠标();
            if ((int)x > 0 && (int)y > 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.MoveToEx(320 + px, 260 + py, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }
            清空鼠标();
            n = 0;
            if (get_HPMAX() != getHP())
            {
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                while (((int)x <= 0 || (int)y <= 0 && n < 10))
                {
                    Thread.Sleep(500);
                    n++;
                    GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
                }
                if ((int)x > 0 && (int)y > 0)
                {
                    GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(500);
                }
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            }
            随机鼠标();
            if ((int)x > 0 && (int)y > 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.MoveToEx(320 + px, 300 + py, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔-是.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔确定.bmp", "020202", 0.8, 0, out x, out y);
            }
            if ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
            }
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔界面.bmp", "020202", 0.8, 0, out x, out y);
            }
            随机鼠标();
            Thread.Sleep(500);
            清空鼠标();
            n = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔取消.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 10)
            {
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "补魔取消.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 5, 2);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
        }
        void 对话存东西()
        {
            object x, y;
            string dx = "铁条.bmp";
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "银行.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "银行.bmp", "020202", 0.8, 0, out x, out y);
            }
            string dm_ret = GameHookHandle.dm.FindPicEx((int)x + 20, (int)y + 25, (int)x + 275, (int)y + 260,
              dx, "010101", 1.0, 0);
            if (dm_ret.Length > 0)
            {
                string[] ss = dm_ret.Split('|');
                int index = 0;
                while (index < ss.Length)
                {
                    string[] sss = ss[index].Split(',');
                    int intX, intY;
                    intX = Convert.ToInt32(sss[1]);
                    intY = Convert.ToInt32(sss[2]);
                    GameHookHandle.dm.MoveTo(intX, intY);
                    Thread.Sleep(500);
                    GameHookHandle.dm.LeftDoubleClick();
                    Thread.Sleep(800);
                    index++;
                }
                SysFunction.Log("存了" + dm_ret.Length + "个东西！");
            }

        }
        //基础函数
        void 开启小地图()
        {
            object x = 0, y = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                GameHookHandle.dm.MoveToEx(565 + px, 38 + py, 3, 2);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            }

        }
        void 关闭小地图()
        {
            object x = 0, y = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx(565 + px, 38 + py, 3, 2);
                Thread.Sleep(100);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            }

        }
        void 背包扔垃圾(bool isMoshi = false)
        {
            object x = 0, y = 0;
            string lj;
            int n = 0;
            if (isMoshi)
            {
                lj = "邪灵水晶.bmp|水的水晶碎片.bmp|火的水晶碎片.bmp|风的水晶碎片.bmp|地的水晶碎片.bmp|红帽子.bmp";
                lj = lj + "|卡片.bmp|魔石红.bmp|魔石黄.bmp|魔石蓝.bmp|魔石绿.bmp";
            }
            else
            {
                lj = "邪灵水晶.bmp|水的水晶碎片.bmp|火的水晶碎片.bmp|风的水晶碎片.bmp|地的水晶碎片.bmp|红帽子.bmp";
            }
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品.bmp", "020202", 0.8, 0, out x, out y);
            while (((int)x <= 0 || (int)y <= 0) && n < 20)
            {
                GameHookHandle.dm.MoveToEx(200 + px, 495 + py, 10, 3);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                n++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品.bmp", "020202", 0.8, 0, out x, out y);
            }
            Thread.Sleep(500);
            string dm_ret = GameHookHandle.dm.FindPicEx((int)x - 20, (int)y - 20, (int)x + 255, (int)y + 245,
               lj, "010101", 1.0, 0);
            if (dm_ret.Length > 0)
            {
                string[] ss = dm_ret.Split('|');
                int index = 0;
                while (index < ss.Length)
                {
                    string[] sss = ss[index].Split(',');
                    int intX, intY;
                    intX = Convert.ToInt32(sss[1]);
                    intY = Convert.ToInt32(sss[2]);
                    GameHookHandle.dm.MoveTo(intX, intY);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(500);
                    GameHookHandle.dm.MoveTo((int)x - 50, (int)y);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(500);
                    index++;
                }
                SysFunction.Log("扔了" + dm_ret.Length + "个东西！");
            }

        }
        void 关闭背包()
        {
            object x = 0, y = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x > 0 && (int)y > 0)
            {
                GameHookHandle.dm.MoveToEx(200 + px, 495 + py, 10, 3);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品.bmp", "020202", 0.8, 0, out x, out y);
            }

            Thread.Sleep(500);
            清空鼠标();

        }
        void 随机鼠标()
        {
            GameHookHandle.dm.MoveToEx(0, 100, 600, 450);
        }
        void 清空文字()
        {
            GameHookHandle.dm.KeyPress(36);
        }
        void 清空鼠标()
        {
            GameHookHandle.dm.MoveToEx(600, 400, 40, 80);
        }

        void 移动(int tarX, int tarY)
        {
            int 向东格子数, 向南格子数, 屏幕坐标X, 屏幕坐标Y;
            int 坐标X, 坐标Y;
            坐标X = 固定X坐标();
            坐标Y = 固定Y坐标();
            向东格子数 = tarX - 坐标X;
            向南格子数 = tarY - 坐标Y;
            if (Math.Abs(向东格子数) > 50 || Math.Abs(向南格子数) > 40)
            {
                SysFunction.Log("不可能到达的坐标！有可能已经发生切图。");
                return;
            }
            if (向东格子数 > 5 && 向南格子数 < -4)
            {
                SysFunction.Log("计划坐标：" + tarX + "," + tarY + "，当前坐标：" + 坐标X + "," + 坐标Y
                    + "！超出范围修正中……");
                移动((坐标X + tarX) / 2, (坐标Y + tarY) / 2);
                坐标X = 固定X坐标();
                坐标Y = 固定Y坐标();
                向东格子数 = tarX - 坐标X;
                向南格子数 = tarY - 坐标Y;
            }

            屏幕坐标X = 320 + px + 向东格子数 * 32 + 向南格子数 * 32;
            屏幕坐标Y = 260 + py - 向东格子数 * 24 + 向南格子数 * 24;

        begin:
            GameHookHandle.dm.MoveTo(屏幕坐标X, 屏幕坐标Y);
            Thread.Sleep(100);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(300);
            清空鼠标();

            坐标X = 固定X坐标();
            坐标Y = 固定Y坐标();

            int retry = 0;
            while (坐标X != tarX || 坐标Y != tarY)
            {
                坐标X = 固定X坐标();
                坐标Y = 固定Y坐标();
                Thread.Sleep(100);
                if (retry++ > 20)
                {
                    break;
                }
            }
            if (retry > 20)
            {
                屏幕坐标X = 屏幕坐标X + 4;
                屏幕坐标Y = 屏幕坐标Y + 16;
                goto begin;//再来一次
            }


        }
        /// <summary>
        /// 用于过图的移动
        /// </summary>
        /// <param name="方向">0：东，1：南，2：西，3：北</param>
        /// <param name="格子数"></param>
        void Move(int 方向, int 格子数)
        {
            if (格子数 > 5)
            {
                return;
            }
            int tarX, tarY;
            switch (方向)
            {
                //东
                case 0:
                    tarX = 320 + px + 格子数 * 32;
                    tarY = 260 + py - 格子数 * 24;
                    GameHookHandle.dm.MoveTo(tarX, tarY);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    break;
                //南
                case 1:
                    tarX = 320 + px + 格子数 * 32;
                    tarY = 260 + py + 格子数 * 24;
                    GameHookHandle.dm.MoveTo(tarX, tarY);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    break;
                //西
                case 2:
                    tarX = 320 + px - 格子数 * 32;
                    tarY = 260 + py + 格子数 * 24;
                    GameHookHandle.dm.MoveTo(tarX, tarY);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    break;
                //北
                case 3:
                    tarX = 320 + px - 格子数 * 32;
                    tarY = 260 + py - 格子数 * 24;
                    GameHookHandle.dm.MoveTo(tarX, tarY);
                    Thread.Sleep(200);
                    GameHookHandle.dm.LeftClick();
                    Thread.Sleep(800);
                    break;

                default: break;
            }
        }
        void 到西门传送石()
        {
            Thread.Sleep(500);
            int 坐标x = 固定X坐标();
            int 坐标y = 固定Y坐标();
            while (坐标x != 141 && 坐标x != 63 && 坐标x != 242)
            {
                登出();
                Thread.Sleep(3000);
                坐标x = 固定X坐标();
            }
            while (坐标x != 63)
            {
                if (坐标x == 242)
                {
                    对话东();
                    Thread.Sleep(500);
                }
                else if (坐标x == 141)
                {
                    对话北();
                    Thread.Sleep(500);
                }
                Thread.Sleep(500);
                坐标x = 固定X坐标();
            }

        }
        void 到东门传送石()
        {
            Thread.Sleep(500);
            int 坐标x = 固定X坐标();
            int 坐标y = 固定Y坐标();
            while (坐标x != 141 && 坐标x != 63 && 坐标x != 242)
            {
                登出();
                Thread.Sleep(3000);
                坐标x = 固定X坐标();
            }
            while (坐标x != 242)
            {
                if (坐标x == 63)
                {
                    对话北();
                    Thread.Sleep(500);
                }
                else if (坐标x == 141)
                {
                    对话北();
                    Thread.Sleep(500);
                }
                Thread.Sleep(500);
                坐标x = 固定X坐标();
            }
        }
        void 对话东()
        {
            GameHookHandle.dm.MoveToEx(320 + 32 * 2 + px, 260 - 24 * 2 + py, 12, 9);
            Thread.Sleep(200);
            GameHookHandle.dm.RightClick();
            Thread.Sleep(500);

        }
        void 对话西()
        {
            GameHookHandle.dm.MoveToEx(320 - 32 * 2 + px, 260 + 24 * 2 + py, 12, 9);
            Thread.Sleep(200);
            GameHookHandle.dm.RightClick();
            Thread.Sleep(500);

        }
        void 对话南()
        {
            GameHookHandle.dm.MoveToEx(320 + 32 * 2 + px, 260 + 24 * 2 + py, 12, 9);
            Thread.Sleep(200);
            GameHookHandle.dm.RightClick();
            Thread.Sleep(500);

        }
        void 对话北()
        {
            GameHookHandle.dm.MoveToEx(320 - 32 * 2 + px, 260 - 24 * 2 + py, 12, 9);
            Thread.Sleep(200);
            GameHookHandle.dm.RightClick();
            Thread.Sleep(500);

        }
        int 固定X坐标()
        {
            object x = 0, y = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            int wait = 0;
            while (((int)x <= 0 || (int)y <= 0) && wait < 10)
            {
                GameHookHandle.dm.MoveTo(567 + px, 39 + py);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                wait++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            }
            string zbx = GameHookHandle.dm.Ocr((int)x - 60, (int)y, (int)x + 10, (int)y + 50, "ffffff-000000", 1.0);
            zbx = zbx.Length > 0 ? zbx : "0";
            //SysFunction.Log("X坐标为：" + zbx);
            return Convert.ToInt32(zbx);
        }
        int 固定Y坐标()
        {
            object x = 0, y = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            int wait = 0;
            while (((int)x <= 0 || (int)y <= 0) && wait < 10)
            {
                GameHookHandle.dm.MoveTo(567 + px, 39 + py);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                wait++;
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "地图.bmp", "020202", 0.8, 0, out x, out y);
            }
            string zby = GameHookHandle.dm.Ocr((int)x + 10, (int)y, (int)x + 120, (int)y + 50, "ffffff-000000", 1.0);
            zby = zby.Length > 0 ? zby : "0";
            return Convert.ToInt32(zby);
        }
        void 关闭所有窗口()
        {
            GameHookHandle.dm.KeyDown(160);
            Thread.Sleep(200);
            GameHookHandle.dm.KeyPress(123);
            Thread.Sleep(200);
            GameHookHandle.dm.KeyUp(160);
            Thread.Sleep(200);

        }
        int X坐标()
        {
            uint x = 0, y = 0;
            GameHookHandle.GetXY(out x, out y);
            SysFunction.Log(x + "," + y);
            return (int)x;
        }
        int Y坐标()
        {
            uint x = 0, y = 0;
            GameHookHandle.GetXY(out x, out y);
            SysFunction.Log(x + "," + y);
            return (int)y;
        }
        int 检测背包空位()
        {
            //object x = 0, y = 0;
            //int count = 0;
            //GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品.bmp", "020202", 0.8, 0, out x, out y);
            //while ((int)x <= 0 || (int)y <= 0)
            //{
            //    GameHookHandle.dm.MoveToEx(200 + px, 495 + py, 5, 2);
            //    Thread.Sleep(200);
            //    GameHookHandle.dm.LeftClick();
            //    Thread.Sleep(500);
            //    GameHookHandle.dm.FindPic(0, 0, 800, 600, "物品.bmp", "020202", 0.8, 0, out x, out y);
            //}
            //string dm_ret = GameHookHandle.dm.FindPicEx(0, 0, 800, 600, "空白格子.bmp", "020202", 1.0, 0);
            //if (dm_ret.Length > 0)
            //{
            //    string[] ss = dm_ret.Split('|');
            //    count = ss.Length;
            //}
            //SysFunction.Log(count + " 个空格。");
            //return count;

            string[] bag = new string[20];

            GameHookHandle.GetBag(bag);
            int n = 20;
            for (int i = 0; i < bag.Length; i++)
            {
                n = bag[i].Length > 0 ? n - 1 : n;
            }
            return n;
        }

        int 战利品检测()
        {
            string dm_ret = GameHookHandle.dm.FindPicEx(0, 0, 800, 600, "空白格子2.bmp", "020202", 1.0, 0);
            if (dm_ret.Length > 0)
            {
                string[] ss = dm_ret.Split('|');
                int count = ss.Length + 1;
                return 3 - count;
            }
            else
                return 0;
            //    ss = split(dm_ret,"|")
            //    index = 0
            //    count = UBound(ss) + 1
            //    战利品检测 = 3- count
            //End If
        }
        /// <summary>
        /// 非战斗状态的受伤检测
        /// </summary>
        /// <returns>1:正常，0:受伤,-1:超时未查到</returns>
        int 受伤检测()
        {
            object x, y;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out x, out y);
            int wait = 0;
            while (((int)x < 0 && (int)y < 0) && wait < 10)
            {
                Thread.Sleep(500);

                GameHookHandle.dm.FindPic(0, 0, 800, 600, "野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out x, out y);
            }
            if (((int)x < 0 && (int)y < 0) && wait >= 10)
            {
                return -1;
            }
            GameHookHandle.dm.FindColor(45 + px, 71 + py, 46 + px, 72 + py, "00CF1F", 1, 0, out x, out y);

            if ((int)x > 0 && (int)y > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }


        }

        /// <summary>
        /// 战斗外血的检测
        /// </summary>
        /// <returns>为1表示通过，为0表示不通过，为-1表示检测异常</returns>
        int 人物血线检测()
        {
            double hp = getHP();
            double hpmax = get_HPMAX();
            if (hp > 0 && hpmax > 0)
            {
                // SysFunction.Log("血含量：" + Math.Round((hp / hpmax) * 100, 2) + "%");
                if ((hp / hpmax >= 人物血线))
                {
                    return 1;
                }
                else
                    return 0;
            }
            else
                return -1;
        }
        int 人物血值检测()
        {
            double hp = getHP();
            double hpmax = get_HPMAX();
            if (hp > 0 && hpmax > 0)
            {
                // SysFunction.Log("血含量：" + Math.Round((hp / hpmax) * 100, 2) + "%");
                if (hp >= 人物血值)
                {
                    return 1;
                }
                else
                    return 0;
            }
            else
                return -1;
        }
        int 人物魔线检测()
        {
            double mp = getMP();
            double mpmax = get_MPMAX();
            if (mp > 0 && mpmax > 0)
            {
                //SysFunction.Log("魔含量：" + Math.Round((mp / mpmax) * 100, 2) + "%");
                if ((mp / mpmax >= 人物魔线))
                {
                    return 1;
                }
                else
                    return 0;
            }
            else
                return -1;
        }
        int 人物魔值检测()
        {
            double mp = getMP();
            double mpmax = get_MPMAX();
            if (mp > 0 && mpmax > 0)
            {
                //SysFunction.Log("魔含量：" + Math.Round((mp / mpmax) * 100, 2) + "%");
                if (mp >= 人物魔值)
                {
                    return 1;
                }
                else
                    return 0;
            }
            else
                return -1;
        }
        bool 人物补血检测()
        {
            double hp = getHP();
            double hpmax = get_HPMAX();
            // SysFunction.Log("人物补血线：" + 人物补血线);
            if (hp > 0 && hpmax > 0 && ((hp / hpmax) >= 人物补血线))
            {
                // SysFunction.Log("人物血检测通过！");

                return true;
            }
            else
            {
                //SysFunction.Log("人物血检测不通过！");
                return false;
            }

        }

        int 宠物补血检测(bool 前排 = false)
        {
            object x = 0, y = 0;
            int count = 0;
            double zb = 0;

            // SysFunction.Log("宠物吸血线：" + 宠物吸血线);
            if (前排)
            {
                GameHookHandle.dm.FindColor(461 + px, 385 + py, 492 + px, 387 + py, "000001", 0.8, 0, out x, out y);
                zb = 宠物吸血线 * 32 + 460 + px;
            }
            else
            {
                GameHookHandle.dm.FindColor(399 + px, 338 + py, 430 + px, 340 + py, "000001", 0.8, 0, out x, out y);
                zb = 宠物吸血线 * 32 + 398 + px;
            }
            if ((int)x >= zb)
            {
                //SysFunction.Log("宠物补血检测通过！");
                return 1;
            }
            else if ((int)x <= 0)
            {
                //SysFunction.Log("宠物补血检测通过！");
                return 1;
            }
            else
            {
                // SysFunction.Log("宠物补血检测不通过！");
                return 0;
            }
        }

        int 宠物血检测(bool 前排 = false)
        {
            object x = 0, y = 0;
            int count = 0;
            double zb = 0;

            if (前排)
            {
                GameHookHandle.dm.FindColor(461 + px, 385 + py, 492 + px, 387 + py, "000001", 0.8, 0, out x, out y);
                zb = 宠物血线 * 32 + 460 + px;
            }
            else
            {
                GameHookHandle.dm.FindColor(399 + px, 338 + py, 430 + px, 340 + py, "000001", 0.8, 0, out x, out y);
                zb = 宠物血线 * 32 + 398 + px;
            }
            if ((int)x >= zb)
            {
                return 1;
            }
            else if ((int)x <= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        int 宠物魔检测(bool 前排 = false)
        {
            object x = 0, y = 0;
            int count = 0;
            double zb = 0;

            //SysFunction.Log("宠物吸血魔线：" + 宠物吸血魔线);
            if (前排)
            {
                GameHookHandle.dm.FindColor(461 + px, 388 + py, 492 + px, 390 + py, "000001", 0.8, 0, out x, out y);
                zb = 宠物吸血魔线 * 32 + 460 + px;
            }
            else
            {
                GameHookHandle.dm.FindColor(399 + px, 341 + py, 430 + px, 343 + py, "000001", 0.8, 0, out x, out y);
                zb = 宠物吸血魔线 * 32 + 398 + px;
            }
            if ((int)x >= zb)
            {
                return 1;
            }
            else if ((int)x <= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        void 登出()
        {
            object x = 0, y = 0, xtX = 0, xtY = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家指令.bmp|野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out xtX, out xtY);
            while ((int)xtX <= 0 || (int)xtY <= 0)
            {
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家指令.bmp|野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out xtX, out xtY);
            }
            GameHookHandle.CloseYuandi();
            GameHookHandle.CloseGaosuYidong();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "系统设定.bmp", "020202", 0.8, 0, out xtX, out xtY);
            while ((int)xtX <= 0 || (int)xtY <= 0)
            {
                GameHookHandle.dm.MoveToEx(500 + px, 490 + py, 5, 2);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "系统设定.bmp", "020202", 0.8, 0, out xtX, out xtY);
            }
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "回到城内登入点.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                GameHookHandle.dm.MoveToEx((int)xtX + 80, (int)xtY + 40, 10, 3);
                Thread.Sleep(200);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(500);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "回到城内登入点.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(500);
        }
        void 登出游戏()
        {
            object x = 0, y = 0, xtX = 0, xtY = 0;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家指令.bmp|野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out xtX, out xtY);
            while ((int)xtX <= 0 || (int)xtY <= 0)
            {
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "玩家指令.bmp|野外非战斗.bmp|非战斗.bmp", "020202", 0.8, 0, out xtX, out xtY);
            }
            GameHookHandle.CloseYuandi();
            GameHookHandle.CloseGaosuYidong();
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "系统设定.bmp", "020202", 0.8, 0, out xtX, out xtY);
            while ((int)xtX <= 0 || (int)xtY <= 0)
            {
                GameHookHandle.dm.MoveToEx(500 + px, 490 + py, 5, 2);
                Thread.Sleep(300);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "系统设定.bmp", "020202", 0.8, 0, out xtX, out xtY);
            }
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "伺服器选择画面.bmp", "020202", 0.8, 0, out x, out y);
            while ((int)x <= 0 || (int)y <= 0)
            {
                GameHookHandle.dm.MoveToEx((int)xtX + 80, (int)xtY + 40, 10, 3);
                Thread.Sleep(300);
                GameHookHandle.dm.LeftClick();
                Thread.Sleep(800);
                GameHookHandle.dm.FindPic(0, 0, 800, 600, "伺服器选择画面.bmp", "020202", 0.8, 0, out x, out y);
            }
            GameHookHandle.dm.MoveToEx((int)x, (int)y, 10, 3);
            Thread.Sleep(300);
            GameHookHandle.dm.LeftClick();
            Thread.Sleep(800);
        }
        bool 判断卡时(out string 卡时)
        {
            卡时 = "";
            string s = GameHookHandle.dm.Ocr(105 + px, 28 + py, 175 + px, 61 + py, "fffffe-101010", 1.0);
            if (s.Length == 3)
            {
                卡时 = s[0] + ":" + s[1] + s[2];
            }
            return s != "000";

        }

        double getHP()
        {
            try
            {
                uint x = 0, y = 0;
                GameHookHandle.GetHp(out x, out y);
                //GameHookHandle.dm.FindPic(0, 0, 800, 600, "非战斗.bmp", "000000", 0.7, 0, out x, out y);
                //string ret;
                //ret = GameHookHandle.dm.Ocr((int)x + 20, (int)y - 5, (int)x + 60, (int)y + 15, "000000-010101", 1.0);
                return Convert.ToDouble(x);
            }
            catch
            {
                return 0;
            }

        }
        double getMP()
        {
            try
            {
                uint x = 0, y = 0;
                GameHookHandle.GetMp(out x, out y);
                //object x = 0, y = 0;
                //GameHookHandle.dm.FindPic(0, 0, 800, 600, "非战斗.bmp", "000000", 0.7, 0, out x, out y);
                //string ret;
                //ret = GameHookHandle.dm.Ocr((int)x + 20, (int)y + 5, (int)x + 60, (int)y + 50, "000000-010101", 1.0);
                return Convert.ToDouble(x);
            }
            catch
            {
                return 0;
            }
        }

        double get_HPMAX()
        {
            try
            {
                uint x = 0, y = 0;
                GameHookHandle.GetHp(out x, out y);
                //object x = 0, y = 0;
                //GameHookHandle.dm.FindPic(0, 0, 800, 600, "非战斗.bmp", "000000", 0.7, 0, out x, out y);
                //string ret;
                //ret = GameHookHandle.dm.Ocr((int)x + 45, (int)y - 5, (int)x + 85, (int)y + 15, "000000-010101", 1.0);
                return Convert.ToDouble(y);
            }
            catch
            {
                return 0;
            }
        }
        double get_MPMAX()
        {
            try
            {
                uint x = 0, y = 0;
                GameHookHandle.GetMp(out x, out y);
                //object x = 0, y = 0;
                //GameHookHandle.dm.FindPic(0, 0, 800, 600, "非战斗.bmp", "000000", 0.7, 0, out x, out y);
                //string ret;
                //ret = GameHookHandle.dm.Ocr((int)x + 45, (int)y + 5, (int)x + 85, (int)y + 50, "000000-010101", 1.0);
                return Convert.ToDouble(y);
            }
            catch
            {
                return 0;
            }

        }

        //double getPetHP(bool isFront = true)
        //{
        //    if (isFront)
        //    {

        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        #region 界面操作
        public void DoubleClickItemManu(int index)
        {
            object x, y;
            int tarX, tarY;
            GameHookHandle.dm.FindPic(0, 0, 800, 600, "制造.bmp", "020202", 0.8, 0, out x, out y);
            tarX = (int)x + 40 + ((index - 1) % 5) * 50;
            tarY = (int)y + 20 + ((index - 1) / 5) * 50;
            GameHookHandle.dm.MoveToEx(tarX, tarY, 5, 5);
            Thread.Sleep(200);
            GameHookHandle.dm.LeftDoubleClick();
            Thread.Sleep(500);
        }
        #endregion

    }
}

