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
//using System.Web.UI;

namespace Demo
{
    partial class Live_Dialog
    {
        private string configPath = "";
        private void btn_Enter_Exit_Config_Click(object sender, EventArgs e)
        {
            if (btn_Enter_Exit_Config.Text == "Enter Config")
            {

                int ret = DevSdkHelper.NET_SDK_EnterDVRConfig(userId);
                if (ret < 0)
                {
                    string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {                   
                    EnableControl(true);
                    tb_RecvXml.ReadOnly = true;
                    btn_Enter_Exit_Config.Text = "Exit Config";
                }

            }
            else
            {
                bool ret = DevSdkHelper.NET_SDK_ExitDVRConfig(userId);
                if (ret)
                {
                    EnableControl(false);
                    btn_Enter_Exit_Config.Enabled = true;
                    btn_Enter_Exit_Config.Text = "Enter Config";
                }
                else
                {
                    string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
                        
            
        }
        private void ConfigMenuInit()
        {
            EnableControl(false);
            btn_Enter_Exit_Config.Enabled = true;
            btn_Enter_Exit_Config.Text = "Enter Config";
            //cmb_Config_Chnn.Items.Add("All");
            //cmb_Config_Chnn.SelectedIndex = 0;

        }
        private void EnableControl(bool status)
        {
            foreach (System.Windows.Forms.Control ct in tabPage3.Controls)           
                ct.Enabled = status;
            
        }
        private void btn_Config_Path_Browse_Click(object sender, EventArgs e)
        {
            //if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    tb_Config_Path.Text = folderBrowserDialog1.SelectedPath;
            //}
        }
        private void btn_Config_Default_Click(object sender, EventArgs e)
        {
            //configPath = tb_Config_Path.Text;
            //if (configPath.Length == 0)
            //{
            //    MessageBox.Show("Pls select a path");
            //    return;
            //}
            //LoadConfig();
                

            
        }
        private void LoadConfig()
        {
            //if (ckb_Config_SysBasic.Checked)
            //{
            //    LoadBasicConfig();
            //}
        }
        private void LoadBasicConfig()
        {
            DD_BASIC_CONFIG dbc = new DD_BASIC_CONFIG();
            int sizeOfBc = Marshal.SizeOf(dbc);
            IntPtr intptrBc = Marshal.AllocHGlobal(sizeOfBc);
            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_SYSTEM_BASIC, -1, intptrBc, sizeOfBc, ref lpBytesReturned, true);
            DD_BASIC_CONFIG temp = (DD_BASIC_CONFIG)Marshal.PtrToStructure(intptrBc, typeof(DD_BASIC_CONFIG));

            DD_DEVICE_INFO ddi = new DD_DEVICE_INFO();
            int sizeOfDi = Marshal.SizeOf(ddi);
            IntPtr intptrDi = Marshal.AllocHGlobal(sizeOfDi);
            ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_DEVICE_INFO, -1, intptrDi, sizeOfDi, ref lpBytesReturned, true);
            DD_DEVICE_INFO temp1 = (DD_DEVICE_INFO)Marshal.PtrToStructure(intptrDi, typeof(DD_DEVICE_INFO));

            StringBuilder sb1 = new StringBuilder();
            sb1.Append(DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(temp1.deviceName)) + "|");
            sb1.Append(temp1.deviceID + "|");
            sb1.Append((temp.videoFormat == 1 ? "NTSC" : "PAL") + "|");
            sb1.Append(GetResolution(temp.videoOutResolution) + "|");
            sb1.Append(temp.VGARefresh.ToString() + "|");
            sb1.Append(temp.screensaver.ToString() + "|");
            sb1.Append(temp.deviceLanguage.ToString() + "|");
            sb1.Append((temp.passwordCheck == 0 ? "false" : "true") + "|");
            sb1.Append(temp.RecycleRecord == 0 ? "false" : "true");
            
            StringBuilder sb2 = new StringBuilder();
            sb2.Append("device name" + "|");
            sb2.Append("device no" + "|");
            sb2.Append("video format" + "|");
            sb2.Append("resolution" + "|");
            sb2.Append("refresh" + "|");
            sb2.Append("screen saver" + "|");
            sb2.Append("language" + "|");
            sb2.Append("password check" + "|");
            sb2.Append("recycle record");



            Marshal.FreeHGlobal(intptrBc);
            Marshal.FreeHGlobal(intptrDi);

            //HtmlGeneratorHelp.CreateMain(@"C:\Users\Born\Desktop\main.html");
            //HtmlGeneratorHelp.CreateIndex(@"C:\Users\Born\Desktop\index.html","BasicConfig");

            //HtmlGeneratorHelp.CreateConfig(@"C:\Users\Born\Desktop\config.html", "BasicConfig",sb1.ToString(),sb2.ToString());

            //System.Diagnostics.Process.Start(@"C:\Users\Born\Desktop\main.html"); 
        }
        private void SaveBasicConfig()
        {
            int lpBytesReturned = 0;
            DD_DEVICE_INFO ddi = new DD_DEVICE_INFO();
            int sizeOfDi = Marshal.SizeOf(ddi);
            IntPtr intptrDi = Marshal.AllocHGlobal(sizeOfDi);
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_DEVICE_INFO, -1, intptrDi, sizeOfDi, ref lpBytesReturned, true);
            DD_DEVICE_INFO temp1 = (DD_DEVICE_INFO)Marshal.PtrToStructure(intptrDi, typeof(DD_DEVICE_INFO));

            Marshal.FreeHGlobal(intptrDi);
            //string newName = "abc";
            string newName = "48.56_N9000DVR";
            while (newName.Length < 64)
                newName += "\0";
            temp1.deviceName = Encoding.UTF8.GetBytes(newName);
                       
            sizeOfDi = Marshal.SizeOf(temp1);
            intptrDi = Marshal.AllocHGlobal(sizeOfDi);
            Marshal.StructureToPtr(temp1, intptrDi, true);
            ret = DevSdkHelper.NET_SDK_SetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_DEVICE_INFO, -1, intptrDi, sizeOfDi);
            Marshal.FreeHGlobal(intptrDi);
            if (ret)
                MessageBox.Show("save success");

        }
        private void btn_Config_Save_Click(object sender, EventArgs e)
        {
            SaveBasicConfig();
            //SaveAccountConfig();
            SaveNTPConfig();
        }
        private void btn_Config_Get_Click(object sender, EventArgs e)
        {
            //LoadBasicConfig();
            //LoadAccountsConfig();
            //LoadNTPConfig();
            LoadNetworkInfo();
        }


        private void btn_ChangeTime_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 time = Convert.ToInt32(dateTimePicker1.Value.Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                bool ret = DevSdkHelper.NET_SDK_ChangTime(userId, time);
                if (ret)
                    MessageBox.Show("Change time success");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void btn_GetDeviceIpCamInfo_Click(object sender, EventArgs e)
        {
            Int32 lIPCCount = 64;
            NET_SDK_IPC_DEVICE_INFO[] ipcInfo = new NET_SDK_IPC_DEVICE_INFO[lIPCCount];
            IntPtr intptrIpcInfo = PointArrayToIntPtr(ipcInfo);
            IntPtr pTmp = intptrIpcInfo;
            int bufferSize = NET_SDK_IPC_DEVICE_INFO.GetSize() * (lIPCCount);
            //int bufferSize = NET_SDK_IPC_DEVICE_INFO.GetSize() * lIPCCount + 1;
            bool ret = DevSdkHelper.NET_SDK_GetDeviceIPCInfo(userId, intptrIpcInfo, bufferSize, ref lIPCCount);
            if (ret)
            {
                StringBuilder ipcInfomation = new StringBuilder();
                string ipStr = "";
                
                for (int i = 0; i < lIPCCount; i++, pTmp = IntPtr.Add(pTmp, NET_SDK_IPC_DEVICE_INFO.GetSize()))
                {
                    byte[] pByte = new byte[NET_SDK_IPC_DEVICE_INFO.GetSize()];
                    Marshal.Copy(pTmp, pByte, 0, NET_SDK_IPC_DEVICE_INFO.GetSize());
                    ipcInfo[i] = (NET_SDK_IPC_DEVICE_INFO)BytesToStruct(pByte, typeof(NET_SDK_IPC_DEVICE_INFO));
                    //ipcInfomation.AppendLine("Channel:" + ipcInfo[i].channel + " IP:" + Encoding.UTF8.GetString(ipcInfo[i].szServer) + " status:" + ipcInfo[i].status);
                    ipStr = " IP:" + Encoding.UTF8.GetString(ipcInfo[i].szServer).Replace("\0", " ").Trim();
                    ipcInfomation.AppendLine("Channel:" + ipcInfo[i].channel + ipStr + " status:" + ipcInfo[i].status);
                }
                MessageBox.Show(ipcInfomation.ToString());
            }
            Marshal.FreeHGlobal(intptrIpcInfo);
        }
        private void btn_GetDevice_Click(object sender, EventArgs e)
        {
           
        }
        
        private IntPtr PointArrayToIntPtr(DD_PTZ_PRESET_CONFIG_Ex[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(DD_PTZ_PRESET_CONFIG_Ex));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }
        private IntPtr PointArrayToIntPtr(NET_SDK_CHANNEL_PTZ[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(NET_SDK_CHANNEL_PTZ));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }

        private IntPtr PointArrayToIntPtr(DD_CRUISE_POINT_INFO[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(DD_CRUISE_POINT_INFO));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }


        private IntPtr PointArrayToIntPtr(NET_SDK_CH_DEVICE_STATUS[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(NET_SDK_CH_DEVICE_STATUS));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }

        private IntPtr PointArrayToIntPtr(NET_SDK_IPC_DEVICE_INFO[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(NET_SDK_IPC_DEVICE_INFO));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
                tmp += size;
                
                
                
            }
            return lpoints;
            //return tmp;
        }
        private object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        private void LoadAccountsConfig()
        {
            DD_ACCOUNT_CONFIG[] dacs = new DD_ACCOUNT_CONFIG[64];
            int oneSize = Marshal.SizeOf(typeof(DD_ACCOUNT_CONFIG));
            int sizeOfAc = oneSize * 64;
            IntPtr intptrAc = PointArrayToIntPtr(dacs);
            IntPtr pTmp = intptrAc;
            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_ACCOUNT, -1, intptrAc, sizeOfAc, ref lpBytesReturned, true);

            for (int i = 0; i < lpBytesReturned / oneSize ; i++, pTmp = IntPtr.Add(pTmp, oneSize))
            {
                byte[] pByte = new byte[oneSize];
                Marshal.Copy(pTmp, pByte, 0, oneSize);
                dacs[i] = (DD_ACCOUNT_CONFIG)BytesToStruct(pByte, typeof(DD_ACCOUNT_CONFIG));
                MessageBox.Show(DevSdkHelper.RemoveEmptyChar(Encoding.ASCII.GetString(dacs[i].name)));
            }
            Marshal.FreeHGlobal(intptrAc);
            
        }
        private IntPtr PointArrayToIntPtr(DD_ACCOUNT_CONFIG[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(DD_ACCOUNT_CONFIG));
            IntPtr lpoints = System.Runtime.InteropServices.Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                System.Runtime.InteropServices.Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }
        private void SaveAccountConfig()
        {
            DD_ACCOUNT_CONFIG[] dacs = new DD_ACCOUNT_CONFIG[64];
            int oneSize = Marshal.SizeOf(typeof(DD_ACCOUNT_CONFIG));
            int sizeOfAc = oneSize * 64;
            IntPtr intptrAc = PointArrayToIntPtr(dacs);
            IntPtr pTmp = intptrAc;
            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_ACCOUNT, -1, intptrAc, sizeOfAc, ref lpBytesReturned, true);

            for (int i = 0; i < oneSize / lpBytesReturned; i++, pTmp = IntPtr.Add(pTmp, oneSize))
            {
                byte[] pByte = new byte[oneSize];
                Marshal.Copy(pTmp, pByte, 0, oneSize);
                dacs[i] = (DD_ACCOUNT_CONFIG)BytesToStruct(pByte, typeof(DD_ACCOUNT_CONFIG));
                dacs[i].logSearch = 0;//edit the user's log search right
            }
            Marshal.FreeHGlobal(intptrAc);

            intptrAc = PointArrayToIntPtr(dacs);
            ret = DevSdkHelper.NET_SDK_SetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_ACCOUNT, -1, intptrAc, sizeOfAc);
            Marshal.FreeHGlobal(intptrAc);
        }
        private void LoadNTPConfig()
        {
            DD_DATE_TIME_CONFIG dtc = new DD_DATE_TIME_CONFIG();
            int sizeOfDtc = Marshal.SizeOf(typeof(DD_DATE_TIME_CONFIG));
            IntPtr intptrDtc = Marshal.AllocHGlobal(sizeOfDtc);

            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_DATE_TIME, -1, intptrDtc, sizeOfDtc, ref lpBytesReturned, true);

            dtc = (DD_DATE_TIME_CONFIG)Marshal.PtrToStructure(intptrDtc, typeof(DD_DATE_TIME_CONFIG));
            Marshal.FreeHGlobal(intptrDtc);
            MessageBox.Show(dtc.dateFormat.ToString());
        }

        private void SaveNTPConfig()
        {
            DD_DATE_TIME_CONFIG dtc = new DD_DATE_TIME_CONFIG();
            int sizeOfDtc = Marshal.SizeOf(typeof(DD_DATE_TIME_CONFIG));
            IntPtr intptrDtc = Marshal.AllocHGlobal(sizeOfDtc);

            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_DATE_TIME, -1, intptrDtc, sizeOfDtc, ref lpBytesReturned, true);
            dtc = (DD_DATE_TIME_CONFIG)Marshal.PtrToStructure(intptrDtc, typeof(DD_DATE_TIME_CONFIG));
            Marshal.FreeHGlobal(intptrDtc);

            dtc.dateFormat = 2;
            sizeOfDtc = Marshal.SizeOf(dtc);
            intptrDtc = Marshal.AllocHGlobal(sizeOfDtc);
            Marshal.StructureToPtr(dtc, intptrDtc, true);
            ret = DevSdkHelper.NET_SDK_SetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_DATE_TIME, -1, intptrDtc, sizeOfDtc);
            Marshal.FreeHGlobal(intptrDtc);
        }
        private string GetResolution(uint res)
        {
            switch (res)
            {
                case 1:
                    return "640X480";
                case 2:
                    return "720X480";
                case 4:
                    return "720X576";
                case 8:
                    return "800X600";
                case 16:
                    return "1024X768";
                case 32:
                    return "1280X960";
                case 64:
                    return "1280X1024";
                case 128:
                    return "1920X1080";
                case 256:
                    return "320X240";
                case 512:
                    return "352X240";
                case 1024:
                    return "480X240";
                case 2048:
                    return "704X480";
                case 4096:
                    return "704X576";
                case 8192:
                    return "960X480";
                case 16384:
                    return "960X576";
                case 32768:
                    return "960X1080";
                case 65536:
                    return "1280X720";
                case 131072:
                    return "1600X1200";
                case 262144:
                    return "1920X1536";
                case 524288:
                    return "2048X1536";
                case 1048576:
                    return "2304X1296";
                case 2097152:
                    return "2560X1440";
                case 4194304:
                    return "2592X1520";
                case 8388608:
                    return "2592X1944";
                case 16777216:
                    return "3840X2160";
                default:
                    return "";
            }
        }
        private void LoadNetworkInfo()
        {
            DD_NETWORK_IP_CONFIG dtc = new DD_NETWORK_IP_CONFIG();
            int sizeOfDtc = Marshal.SizeOf(typeof(DD_NETWORK_IP_CONFIG));
            IntPtr intptrDtc = Marshal.AllocHGlobal(sizeOfDtc);

            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_GetDVRConfig(userId, (uint)DD_CONFIG_ITEM_ID.DD_CONFIG_ITEM_NETWORK_IP, -1, intptrDtc, sizeOfDtc, ref lpBytesReturned, true);

            dtc = (DD_NETWORK_IP_CONFIG)Marshal.PtrToStructure(intptrDtc, typeof(DD_NETWORK_IP_CONFIG));
            Marshal.FreeHGlobal(intptrDtc);
            MessageBox.Show(IntToIp(dtc.IP));
            
        }
        private string IntToIp(long ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ipInt & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 24) & 0xFF);                                 
            return sb.ToString();
        }
    }

}
