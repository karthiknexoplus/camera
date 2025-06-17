using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevSdkByCS;
using System.Threading;

namespace Demo
{
    using POINTERHANDLE = Int32;
    public partial class Form1 : Form
    {
        private string filePath;//输入文件路径
        //private Excel.Application excelApp = null;  //excel对象
        //private Excel.Workbook book = null;//excel的book对象
        //private Excel.Worksheet sheet = null;//excel的sheet对象
        private string resPath;//结果文档完整名称
        public static string rootPath = Application.StartupPath;//根目录
        //RarProgressBar rpb;//进度条
        private int column_Test = 1;//数据表的是否测试列
        private int column_Name = 4;//数据表的测试名称列
        private int column_Param = 5;//数据表的测试参数列
        private int column_ExpRes = 8;// 期望结果列
        private int column_ActRes = 9;// 实际结果、图片比较结果列
        private int column_Pic = 10;//数据表的图片列
        private int column_Pic2 = 12;//异常图片列
        private int currentX;
        private bool isRunning;//测试是否正在运行  
        private SynchronizationContext scc;
        private int tryTimes = 0;
        private static int userId = 0;
        private static NET_SDK_DEVICEINFO oNET_SDK_DEVICEINFO = new NET_SDK_DEVICEINFO();
        private int waitTime = 3000;
        string loginIp;//设备IP
        UInt16 loginPort;//设备端口号
        string loginUserName;//设备登录用户名
        string loginPwd;//设备登录密码
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DevSdkHelper.NET_SDK_Init();
            Thread.Sleep(waitTime);
            loginIp = "172.16.36.4";
            loginPort = 6036;
            loginUserName = "admin";
            loginPwd = "123456";
            //userId = DevSdkHelper.NET_SDK_LoginEx(loginIp, loginPort, loginUserName, loginPwd, ref oNET_SDK_DEVICEINFO, NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP, "");
            Thread.Sleep(waitTime);
            MessageBox.Show(oNET_SDK_DEVICEINFO.deviceID.ToString());

            //bool tempRes = DevSdkHelper.NET_SDK_GetDeviceInfo(userId, ref oNET_SDK_DEVICEINFO);
            //MessageBox.Show(tempRes.ToString());

            //DD_TIME tempTime = new DD_TIME();
            //DevSdkHelper.NET_SDK_GetDeviceTime(userId, ref tempTime);
            //MessageBox.Show(tempTime.ToString());

            NET_SDK_CLIENTINFO cinfo = new NET_SDK_CLIENTINFO();
            NET_SDK_FRAME_INFO finfo = new NET_SDK_FRAME_INFO();
            finfo.deviceID = oNET_SDK_DEVICEINFO.deviceID;
            finfo.channel = 0;
            finfo.frameType = 0x01;
            finfo.length = 10;

            //IntPtr pthis = new IntPtr();
            LIVE_DATA_CALLBACK ldc = new LIVE_DATA_CALLBACK((POINTERHANDLE lLiveHandle, NET_SDK_FRAME_INFO frameInfo, IntPtr pBuffer, IntPtr pUser) =>
            {
                //实时预览
            });
            ldc(0, finfo, IntPtr.Zero, panel1.Handle);
            Int64 a = DevSdkHelper.NET_SDK_LivePlay(userId, ref cinfo, ldc, IntPtr.Zero);




            MessageBox.Show(a.ToString());
        }
    }
}
