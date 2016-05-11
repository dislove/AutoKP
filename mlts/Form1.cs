using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using ZYC;
using System.Threading;

namespace mlts
{
    public partial class Form1 : Form
    {
        GameScript GameScriptHandle;

        private Image _imgAppCross;
        private Image _imgApp;
        private ArrayList innerintptr;
        private IntPtr _hWndCurrent;
        private APIMethod api = new APIMethod();
        private bool en = false;

        private int delay = 0;
        private Thread threadManufacturing = null;
        private DateTime manufacturingTime = DateTime.Now;

        public Form1()
        {
            //测试
            InitializeComponent();
            this.innerintptr = new ArrayList();
            this.innerintptr.Add(this.dragPictureBox.Handle);
            //this.innerintptr.Add(this.pictureBox1.Handle);
            // this.innerintptr.Add(this.panel1.Handle);
            this._imgAppCross = Resources.drag;
            this._imgApp = Resources.drag2;

            this.dragPictureBox.Image = this._imgAppCross;
            this.pictureBox1.Image = this._imgAppCross;

            //this.timer2.Interval = 1000;
            //this.timer2.Start();
            //this.timer2.
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool b = SysFunction.GetDbgPri();
            if (b)
                SysFunction.Log("成功");
            InitConfig();
            //Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void InitConfig()
        {
            cb_Binding.SelectedIndex = 0;
            cbox_script.SelectedIndex = 0;
            checkBox1.Checked = true;
            checkBox2.Checked = true;

            checkBox3.Checked = false;
            checkBox4.Checked = false;

            cb_HpLv.SelectedIndex = 2;
            cb_MpLv.SelectedIndex = 8;
            cb_BackHpLv.SelectedIndex = 8;
            cb_BackMpLv.SelectedIndex = 8;
            cb_PetHp.SelectedIndex = 2;
            cb_PetMp.SelectedIndex = 8;

            cb_AidInSkill.SelectedIndex = 0;
            cb_AidInLv.SelectedIndex = 0;
            cb_AidOutSkill.SelectedIndex = 0;
            cb_AidOutLv.SelectedIndex = 0;
            cb_PetAid.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                GameScriptHandle = new GameScript(uint.Parse(textBox1.Text), uint.Parse(textBox3.Text),
                    cbox_script.SelectedIndex, cb_Binding.SelectedIndex);

                LoadConfig();

                GameScriptHandle.Start();

                isStop = false;
                button1.Enabled = false;
                button3.Enabled = true;
            }
            catch
            {
                isStop = true;
                button1.Enabled = true;
                button3.Enabled = false;
                SysFunction.Log("启动失败!");
            }
            //GameScriptHandle.script();


        }

        private bool isPause = true;
        private void button2_Click(object sender, EventArgs e)
        {
            if (GameScriptHandle == null)
                return;
            if (isPause == true)
            {
                button2.Text = "恢复";
                GameScriptHandle.Pause();
                isPause = false;
            }
            else
            {
                button2.Text = "暂停";
                GameScriptHandle.Resume();
                isPause = true;
            }


        }
        private bool isStop = false;
        private void button3_Click(object sender, EventArgs e)
        {
            if (GameScriptHandle == null)
            {
                return;
            }
            else
            {
                GameScriptHandle.Close();
                button2.Text = "暂停";
                //GameScriptHandle.Resume();
                isPause = true;
            }
            button1.Enabled = true;
            button3.Enabled = false;


        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                String logtext;
                while (SysFunction.LogStrList.Count != 0)
                {
                    logtext = SysFunction.LogStrList.Dequeue();
                    textBox2.Text = logtext + "\r\n" + textBox2.Text;
                }

                uint x, y, hp, hpmax, mp, mpmax;
                if (GameScriptHandle == null || GameScriptHandle.GameHookHandle == null)
                {
                    return;
                }
                LoadXY(out x, out y);
                LoadHpMp(out hp, out hpmax, out mp, out mpmax);
                LoadBag();
                LoadPet();
            }
            catch
            {
            }
        }
        private void LoadPet()
        {
            uint curr, max;
            String Text;
            GameScriptHandle.GameHookHandle.GetPetHp(out curr, out max, 1);
            Text = "宠物1：血" + curr.ToString() + "/" + max.ToString();
            GameScriptHandle.GameHookHandle.GetPetMp(out curr, out max, 1);
            Text += "魔" + curr.ToString() + "/" + max.ToString() + "\r\n";

            GameScriptHandle.GameHookHandle.GetPetHp(out curr, out max, 2);
            Text += "宠物2：血" + curr.ToString() + "/" + max.ToString();
            GameScriptHandle.GameHookHandle.GetPetMp(out curr, out max, 2);
            Text += "魔" + curr.ToString() + "/" + max.ToString() + "\r\n";

            GameScriptHandle.GameHookHandle.GetPetHp(out curr, out max, 3);
            Text += "宠物3：血" + curr.ToString() + "/" + max.ToString();
            GameScriptHandle.GameHookHandle.GetPetMp(out curr, out max, 3);
            Text += "魔" + curr.ToString() + "/" + max.ToString() + "\r\n";

            GameScriptHandle.GameHookHandle.GetPetHp(out curr, out max, 4);
            Text += "宠物4：血" + curr.ToString() + "/" + max.ToString();
            GameScriptHandle.GameHookHandle.GetPetMp(out curr, out max, 4);
            Text += "魔" + curr.ToString() + "/" + max.ToString() + "\r\n";

            GameScriptHandle.GameHookHandle.GetPetHp(out curr, out max, 5);
            Text += "宠物5：血" + curr.ToString() + "/" + max.ToString();
            GameScriptHandle.GameHookHandle.GetPetMp(out curr, out max, 5);
            Text += "魔" + curr.ToString() + "/" + max.ToString() + "\r\n";

            if (textBox5.Text != Text)
            {
                textBox5.Text = Text;
            }
        }
        private void LoadXY(out uint x, out uint y)
        {

            GameScriptHandle.GameHookHandle.GetXY(out x, out y);
            label_xy.Text = x.ToString() + "," + y.ToString();
        }

        private void LoadHpMp(out uint hp, out uint hpmax, out uint mp, out uint mpmax)
        {
            GameScriptHandle.GameHookHandle.GetHp(out hp, out hpmax);
            GameScriptHandle.GameHookHandle.GetMp(out mp, out mpmax);
            label_hp.Text = hp.ToString();
            label_hpmax.Text = hpmax.ToString();
            label_mp.Text = mp.ToString();
            label_mpmax.Text = mpmax.ToString();
        }

        private void LoadBag()
        {
            string[] bag = new string[20];
            GameScriptHandle.GameHookHandle.GetBag(bag);
            int n = 0;
            for (int i = 0; i < bag.Length; i++)
            {
                foreach (Control item in panelbag.Controls)
                {
                    if ((item is Label) && item.Name.Equals("lbbag" + (i + 1)))
                    {
                        string s = bag[i];
                        s = s.Length == 0 ? "空" : s;
                        if (s != item.Text)
                        {
                            item.Text = s;
                        }
                        break;
                    }
                }
                n = bag[i].Length == 0 ? n : n + 1;
            }
            this.tabPage5.Text = "背包（" + n + "）";
        }
        #region SPY++小工具
        private bool _dragging;
        private void dragPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._dragging = true;
                this.Cursor = new Cursor(new MemoryStream(Resources.eye));
                this.dragPictureBox.Image = this._imgApp;
            }
        }

        private void dragPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                IntPtr intPtr = WindowsAPI.WindowFromPoint(Control.MousePosition);
                if (this.innerintptr.Contains(intPtr))
                {
                    intPtr = IntPtr.Zero;
                }
                if (intPtr != this._hWndCurrent)
                {
                    bool flag = 1 == 0;
                    this.api.DrawRevFrame(this._hWndCurrent);
                    this.api.DrawRevFrame(intPtr);
                    this._hWndCurrent = intPtr;
                    this.en = true;
                }
            }
        }

        private void dragPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                this._dragging = false;
                this.Cursor = Cursors.Default;
                if (this._hWndCurrent != IntPtr.Zero)
                {
                    this.api.DrawRevFrame(this._hWndCurrent);
                    this._hWndCurrent = IntPtr.Zero;
                    this.dragPictureBox.Image = this._imgAppCross;
                    WindowsAPI.SendMessage(base.Handle, 514u, 0, 0);

                    Point point = default(Point);
                    WindowsAPI.GetCursorPos(ref point);
                    IntPtr hwnd = WindowsAPI.WindowFromPoint(point);

                    this.textBox1.Text = hwnd.ToString();
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._dragging = true;
                this.Cursor = new Cursor(new MemoryStream(Resources.eye));
                this.dragPictureBox.Image = this._imgApp;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                IntPtr intPtr = WindowsAPI.WindowFromPoint(Control.MousePosition);
                if (this.innerintptr.Contains(intPtr))
                {
                    intPtr = IntPtr.Zero;
                }
                if (intPtr != this._hWndCurrent)
                {
                    bool flag = 1 == 0;
                    this.api.DrawRevFrame(this._hWndCurrent);
                    this.api.DrawRevFrame(intPtr);
                    this._hWndCurrent = intPtr;
                    this.en = true;
                }
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                this._dragging = false;
                this.Cursor = Cursors.Default;
                if (this._hWndCurrent != IntPtr.Zero)
                {
                    this.api.DrawRevFrame(this._hWndCurrent);
                    this._hWndCurrent = IntPtr.Zero;
                    this.dragPictureBox.Image = this._imgAppCross;
                    WindowsAPI.SendMessage(base.Handle, 514u, 0, 0);

                    Point point = default(Point);
                    WindowsAPI.GetCursorPos(ref point);
                    IntPtr hwnd = WindowsAPI.WindowFromPoint(point);

                    this.textBox3.Text = hwnd.ToString();
                }
            }
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GameScriptHandle == null)
            {
                return;
            }
            else
            {
                GameScriptHandle.Close();
            }
            if (threadManufacturing != null)
            {
                if (threadManufacturing.IsAlive)
                {
                    threadManufacturing.Abort();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (GameScriptHandle.GameHookHandle.QuickManufacturing())
                {
                    string time = this.textBox4.Text;
                    //manufacturingTime = DateTime.Now;
                    threadManufacturing = new Thread(Manufacturing);
                    threadManufacturing.IsBackground = true;
                    threadManufacturing.Start(time);
                }
            }
            catch
            {

            }

        }
        private void AsyncUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }
        private void Manufacturing(object o)
        {
            try
            {
                int time = Convert.ToInt32(o);
                for (int i = 0; i <= time; i++)
                {
                    AsyncUI(() =>
                    {
                        button4.Enabled = false;
                        button4.Text = (time - i).ToString();
                    });

                    Thread.Sleep(1000);
                }
                AsyncUI(() =>
                    {
                        button4.Enabled = true;
                        button4.Text = "高速制造";
                    });
            }
            catch
            {

            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            string i = this.textBox4.Text;
            try
            {
                int time = Convert.ToInt32(i);
                while (time >= (DateTime.Now - manufacturingTime).Seconds)
                {
                    this.button4.Enabled = false;
                    this.button4.Text = (DateTime.Now - manufacturingTime).Seconds.ToString();
                }
                if (this.button4.Enabled == false)
                {
                    this.button4.Text = "高速制造";
                    this.button4.Enabled = true;
                }
            }
            catch
            {

            }
        }

        private void LoadConfig()
        {
            if (GameScriptHandle != null)
            {
                GameScriptHandle.isWork = checkBox1.Checked;
                GameScriptHandle.isHit = checkBox2.Checked;
                GameScriptHandle.isAidIn = checkBox3.Checked;
                GameScriptHandle.isAidOut = checkBox4.Checked;

                GameScriptHandle.人物补血线 = Math.Round((10 - cb_HpLv.SelectedIndex) * 0.1, 2);
                GameScriptHandle.人物魔线 = Math.Round((10 - cb_MpLv.SelectedIndex) * 0.1, 2);
                GameScriptHandle.人物血线 = Math.Round((10 - cb_BackHpLv.SelectedIndex) * 0.1, 2);
                GameScriptHandle.人物魔线 = Math.Round((10 - cb_BackMpLv.SelectedIndex) * 0.1, 2);
                GameScriptHandle.宠物吸血线 = Math.Round((10 - cb_PetHp.SelectedIndex) * 0.1, 2);
                GameScriptHandle.宠物吸血魔线 = Math.Round((10 - cb_PetMp.SelectedIndex) * 0.1, 2);

                GameScriptHandle.补血技能 = cb_AidInSkill.SelectedIndex + 1;
                GameScriptHandle.补血等级 = cb_AidInLv.SelectedIndex + 1;
                GameScriptHandle.急救技能 = cb_AidOutSkill.SelectedIndex + 1;
                GameScriptHandle.急救等级 = cb_AidOutLv.SelectedIndex + 1;
                GameScriptHandle.宠物吸血 = cb_PetAid.SelectedIndex + 1;

                GameScriptHandle.人物血值 = Convert.ToDouble(tb_BackHp.Text);
                GameScriptHandle.人物魔值 = Convert.ToDouble(tb_BackMp.Text);
                GameScriptHandle.刷屏信息 = tb_Talk.Text;
            }
        }

        #region 配置参数界面响应
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                CheckBox c = sender as CheckBox;
                GameScriptHandle.isWork = c.Checked;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                CheckBox c = sender as CheckBox;
                GameScriptHandle.isHit = c.Checked;
            }

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                CheckBox c = sender as CheckBox;
                GameScriptHandle.isAidIn = c.Checked;
            }

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                CheckBox c = sender as CheckBox;
                GameScriptHandle.isAidOut = c.Checked;
            }

        }

        private void cb_HpLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.人物补血线 = Math.Round((10 - c.SelectedIndex) * 0.1, 2);
            }

        }

        private void cb_MpLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.人物魔线 = Math.Round((10 - c.SelectedIndex) * 0.1, 2);
            }

        }

        private void cb_BackHpLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.人物血线 = Math.Round((10 - c.SelectedIndex) * 0.1, 2);
            }

        }

        private void cb_BackMpLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.人物魔线 = Math.Round((10 - c.SelectedIndex) * 0.1, 2);
            }

        }

        private void cb_PetHp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.宠物吸血线 = Math.Round((10 - c.SelectedIndex) * 0.1, 2);
            }

        }

        private void cb_PetMp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.宠物吸血魔线 = Math.Round((10 - c.SelectedIndex) * 0.1, 2);
            }

        }

        private void cb_AidInSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.补血技能 = c.SelectedIndex + 1;
            }


        }

        private void cb_AidInLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.补血等级 = c.SelectedIndex + 1;
            }

        }

        private void cb_AidOutSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.急救技能 = c.SelectedIndex + 1;
            }

        }

        private void cb_AidOutLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.急救等级 = c.SelectedIndex + 1;
            }

        }

        private void cb_PetAid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                ComboBox c = sender as ComboBox;
                GameScriptHandle.宠物吸血 = c.SelectedIndex + 1;
            }

        }

        private void tb_BackHp_TextChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                TextBox c = sender as TextBox;
                GameScriptHandle.人物血值 = Convert.ToDouble(c.Text);
            }

        }

        private void tb_BackMp_TextChanged(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                TextBox c = sender as TextBox;
                GameScriptHandle.人物魔值 = Convert.ToDouble(c.Text);
            }

        }
        #endregion

        private void button7_Click(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                GameScriptHandle.Hide();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (GameScriptHandle != null)
            {
                GameScriptHandle.Show();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void lbbag_MouseClick(object sender, EventArgs e)
        {
            try
            {
                if (GameScriptHandle != null)
                {
                    Label l = sender as Label;

                    string s = l.Name.Substring(5, l.Name.Length - 5);
                    GameScriptHandle.DoubleClickItemManu(Convert.ToInt32(s));
                    SysFunction.Log("点第" + s + "个物品,为：" + l.Text);
                }
            }
            catch
            {
                SysFunction.Log("点击失败");
            }
        }

        private void tb_Talk_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (GameScriptHandle != null)
                {
                    TextBox t = sender as TextBox;

                    string s = t.Text;
                    GameScriptHandle.刷屏信息 = s;

                }
            }
            catch
            {
                SysFunction.Log("点击失败");
            }
        }

    }
}
