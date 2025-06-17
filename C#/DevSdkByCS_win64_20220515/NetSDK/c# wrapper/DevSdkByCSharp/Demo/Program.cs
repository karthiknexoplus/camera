using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevSdkByCS;

namespace Demo
{
    static class Program
    {
        //public static Live_Dialog live_Dialog;
        //public static Login_Dialog login_Dialog;
        public static bool live_Dialog_Showed = false;
        //public static Dictionary<string, NET_SDK_DEVICEINFO_EX> m_deviceDic = new Dictionary<string, NET_SDK_DEVICEINFO_EX>();//Dictionary for save device's userId
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new DiscoverDevices());
            Application.Run(new ConnectType_Dialog());
        }
    }
}
