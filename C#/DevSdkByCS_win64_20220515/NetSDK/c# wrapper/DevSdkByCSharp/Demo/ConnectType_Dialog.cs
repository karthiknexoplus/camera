using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevSdkByCS;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Demo
{
    public partial class ConnectType_Dialog : Form
    {
        public EXCEPTION_CALLBACK excertion = null;
        public ACCEPT_REGISTER_CALLBACK arc = null;
        private int userId = -1;
        private NET_SDK_DEVICEINFO onsd = new NET_SDK_DEVICEINFO();
        private bool inited = false;
        private int addedDevice = 0;
        public ConnectType_Dialog()
        {
            InitializeComponent();
            this.Size = new Size(326, 176);
            label1.Visible = false;
            tb_Port.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            excertion = excertionCallback;
            DevSdkHelper.NET_SDK_SetSDKMessageCallBack(0, IntPtr.Zero, excertion, null);
            if (radioButton1.Checked)
            {
                this.Hide();
                Login_Dialog loginDialog = new Login_Dialog();
                loginDialog.ShowDialog();
            }
            else if (radioButton2.Checked)
            {
                if (addedDevice < 1)
                {
                    MessageBox.Show("No listening device information Added");
                    return;
                }
                             
                bool ret = DevSdkHelper.NET_SDK_SetRegisterPort(Convert.ToUInt16(tb_Port.Text),IntPtr.Zero,0);
                arc = acceptRegisterCallBack;
                bool rret = DevSdkHelper.NET_SDK_SetRegisterCallback(arc, IntPtr.Zero);
              
                int tryTimes = 0;
                while (userId == -1)
                {
                    Thread.Sleep(10);
                    tryTimes++;
                    if (tryTimes > 1000)
                    {
                        MessageBox.Show("No device register!");
                        DevSdkHelper.NET_SDK_Cleanup();
                        return;
                    }
                }
                this.Hide();
                Live_Dialog live_Dialog = new Live_Dialog(userId, onsd, "","");
                live_Dialog.ShowDialog();
                if (live_Dialog.DialogResult == DialogResult.OK)
                {
                    live_Dialog.Dispose();
                    live_Dialog = null;
                    this.Show();
                }
                else
                {
                    DevSdkHelper.NET_SDK_Cleanup();
                    Environment.Exit(0);
                }

            }
            else if (radioButton3.Checked)
            {
                bool bResult = DevSdkHelper.NET_SDK_Init();
                
                this.Hide();
                DiscoverDevices dd = new DiscoverDevices();
                if (dd.ShowDialog() == DialogResult.Cancel)
                {
                    DevSdkHelper.NET_SDK_Cleanup();
                    this.Show();
                }

            }

        }
        private void acceptRegisterCallBack(Int32 lUserID, Int32 lRegisterID, IntPtr pDeviceInfo, IntPtr pUser)
        {
            userId = lUserID;
            //onsd = pDeviceInfo;
            onsd = (NET_SDK_DEVICEINFO)Marshal.PtrToStructure(pDeviceInfo, typeof(NET_SDK_DEVICEINFO));
        }
        private void excertionCallback(UInt32 dwType, Int32 lUserID, Int32 lHandle, IntPtr pUser)
        {
            if (0 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_DISCONNECT\r\n", dwType, lUserID, lHandle); ;
                Debug.Print(temp);
            }
            else if (1 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_RECONNECT\r\n", dwType, lUserID, lHandle); ;
                Debug.Print(temp);
            }
            else if (2 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_CH_DISCONNECT\r\n", dwType, lUserID, lHandle); ;
                Debug.Print(temp);
            }
            else if (3 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_CH_RECONNECT\r\n", dwType, lUserID, lHandle); ;
                Debug.Print(temp);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            this.Size = new Size(this.Size.Width, this.Size.Height + 157);
            label1.Visible = true;
            tb_Port.Visible = true;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            this.Size = new Size(326, 176);
            label1.Visible = false;
            tb_Port.Visible = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            this.Size = new Size(326, 176);
            label1.Visible = false;
            tb_Port.Visible = false;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (!inited)
            {
                if (DevSdkHelper.NET_SDK_Init())
                    inited = true;                
            }
            
            REG_LOGIN_INFO rli = new REG_LOGIN_INFO();
            rli.deviceId = UInt32.Parse(tb_DeviceId.Text.Trim());
            rli.m_szUserName = new byte[36];
            rli.m_szPasswd = new byte[36];
            Array.Copy(Encoding.UTF8.GetBytes(tb_Name.Text.Trim()), rli.m_szUserName, Encoding.UTF8.GetBytes(tb_Name.Text.Trim()).Length);
            Array.Copy(Encoding.UTF8.GetBytes(tb_Psw.Text.Trim()), rli.m_szPasswd, Encoding.UTF8.GetBytes(tb_Psw.Text.Trim()).Length);

            int nSize = Marshal.SizeOf(rli);
            IntPtr mptr = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(rli, mptr, true);


            bool ret = DevSdkHelper.NET_SDK_AddRegisterDeviceInfo(mptr, 1);
            Marshal.FreeHGlobal(mptr);
            if (ret)
            {
                addedDevice += 1;
                MessageBox.Show("Add listening device information successful");
            }
                
        }
    }
}
