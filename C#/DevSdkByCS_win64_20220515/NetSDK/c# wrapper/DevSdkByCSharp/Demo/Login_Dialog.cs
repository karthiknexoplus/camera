using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevSdkByCS;
using System.Runtime.InteropServices;


namespace Demo
{
    public partial class Login_Dialog : Form
    {
        public static string m_username;
        public static string m_password;
        public static string m_ip;
        public static ushort m_port;
        string m_strDeviceSN;
        NET_SDK_CONNECT_TYPE m_eConnectType;
        static int userId = 0;
        static NET_SDK_DEVICEINFO oNET_SDK_DEVICEINFO = new NET_SDK_DEVICEINFO();
        public Login_Dialog()
        {
            InitializeComponent();
            tb_UserName.Text = Properties.Settings.Default.userName;
            tb_Psw.Text = Properties.Settings.Default.psw;
            tb_IP.Text = Properties.Settings.Default.deviceIP;
            tb_Port.Text = Properties.Settings.Default.port;
        }
        //public Login_Dialog(string ip,string port)
        //{
        //    InitializeComponent();
        //    tb_UserName.Text = Properties.Settings.Default.userName;
        //    tb_Psw.Text = Properties.Settings.Default.psw;
        //    tb_IP.Text = ip;
        //    tb_Port.Text = port;
        //}
        private void radioButton1_Click(object sender, EventArgs e)
        {
            tb_IP.Visible = true;
            tb_Domain.Visible = false;
            tb_P2P.Visible = false;
            tb_Port.Text = "6036";
            tb_Sn.Enabled = false;
            label5.Text = "IP";
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            tb_IP.Visible = false;
            tb_Domain.Visible = true;
            tb_P2P.Visible = false;
            tb_Port.Text = "6036";
            tb_Sn.Enabled = false;
            label5.Text = "Domain";
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            tb_IP.Visible = false;
            tb_Domain.Visible = false;
            tb_P2P.Visible = true;
            tb_Port.Text = "40002";
            tb_Sn.Enabled = true;
            label5.Text = "Server";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveParameter();
            Environment.Exit(0);
        }

        private void Login_Dialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveParameter();
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                m_eConnectType = NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP;
            }
            else if (radioButton2.Checked)
            {
                m_eConnectType = NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP;
            }
            else if (radioButton3.Checked)
            {
                m_eConnectType = NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_NAT;
            }
            else if (radioButton4.Checked)
            {
                m_eConnectType = NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_NAT20;
            }
            SaveParameter();
            GetDlgItemText();
            Login();
        }
        private void Login()
        {          
            bool bResult = DevSdkHelper.NET_SDK_Init();
            DevSdkHelper.NET_SDK_SetConnectTime();
            DevSdkHelper.NET_SDK_SetReconnect();
            if (bResult)
            {
                //int size = Marshal.SizeOf(typeof(NET_SDK_DEVICEINFO));
                //IntPtr tempPtr = Marshal.AllocHGlobal(size);
                //Marshal.StructureToPtr(log, tempPtr, true);
                //userId = DevSdkHelper.NET_SDK_LoginEx(m_ip, m_port, m_username, m_password, ref oNET_SDK_DEVICEINFO, m_eConnectType, "");
                //userId = DevSdkHelper.NET_SDK_LoginEx(m_ip, m_port, m_username, m_password, ref oNET_SDK_DEVICEINFO, m_eConnectType, m_strDeviceSN);
                //oNET_SDK_DEVICEINFO.deviceIP = DevSdkHelper.IpToInt(m_ip);
                //userId = DevSdkHelper.NET_SDK_LoginEx(m_ip, m_port, m_username, m_password, tempPtr, m_eConnectType, "");
                //oNET_SDK_DEVICEINFO = (NET_SDK_DEVICEINFO)Marshal.PtrToStructure(tempPtr, typeof(NET_SDK_DEVICEINFO));
                if (m_eConnectType == NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP)
                {
                    userId = DevSdkHelper.NET_SDK_Login(m_ip, m_port, m_username, m_password, ref oNET_SDK_DEVICEINFO);
                }
                else if (m_eConnectType == NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_NAT || m_eConnectType == NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_NAT20)
                {
                    if (radioButton4.Checked)
                        DevSdkHelper.NET_SDK_SetNat2Addr(m_ip, m_port);
                    for (int i = 0; i < 10; i++)
                    {
                        if (userId < 1)
                            userId = DevSdkHelper.NET_SDK_LoginEx(m_ip, m_port, m_username, m_password, ref oNET_SDK_DEVICEINFO, m_eConnectType, m_strDeviceSN);
                        else
                            break;
                    }
                }               
                if (userId < 1)
                {
                    MessageBox.Show("Login in failed! End ...");
                    return;
                }
                else
                {
                    this.Hide();
                    Live_Dialog live_Dialog = new Live_Dialog(userId, oNET_SDK_DEVICEINFO, m_username, m_password);
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

            }

        }



        private void GetDlgItemText()
        {
            m_username = tb_UserName.Text.Trim();
            m_password = tb_Psw.Text.Trim();
            if (m_eConnectType == NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_NAT || m_eConnectType == NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_NAT20)//如果是P2P方式
            {
                m_ip = tb_P2P.Text.Trim();
            }
            else
                m_ip = tb_IP.Text.Trim();
            string temp = tb_Port.Text.Trim();
            if (temp.Length > 0)
                m_port = Convert.ToUInt16(temp);

            else
                m_port = 0;
            m_strDeviceSN = tb_Sn.Text.Trim();
        }
        private void SaveParameter()
        {
            Properties.Settings.Default.userName = tb_UserName.Text;
            Properties.Settings.Default.psw = tb_Psw.Text;
            Properties.Settings.Default.deviceIP = tb_IP.Text;
            Properties.Settings.Default.port = tb_Port.Text;
            Properties.Settings.Default.Save();
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            tb_IP.Visible = false;
            tb_Domain.Visible = false;
            tb_P2P.Visible = true;
            tb_Port.Text = "40002";
            tb_Sn.Enabled = true;
            label5.Text = "Server";
        }

    }
}
