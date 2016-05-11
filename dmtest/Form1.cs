using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using dm_new;

namespace dmtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AutoRegCom("regsvr32 D:\\dm.dll /s");  
           
        }
        dmsoft dm;
        string hwnd;
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        private string AutoRegCom(string strCmd)
        {
            //strCmd = "regsvr32 D:\\dm.dll /s";
            string rInfo;
            try
            {
                Process myProcess = new Process();
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo("cmd.exe");
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.CreateNoWindow = true;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo = myProcessStartInfo;
                myProcessStartInfo.Arguments = "/c " + strCmd;
                myProcess.Start();
                StreamReader myStreamReader = myProcess.StandardOutput;
                rInfo = myStreamReader.ReadToEnd();
                myProcess.Close();
                rInfo = strCmd + "\r\n" + rInfo;

                return rInfo;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dm = new dmsoft();
            hwnd = textBox1.Text;

            dm.BindWindow(Convert.ToInt32(hwnd), "dx2", "windows", "windows", 0);
        }
    }
}
