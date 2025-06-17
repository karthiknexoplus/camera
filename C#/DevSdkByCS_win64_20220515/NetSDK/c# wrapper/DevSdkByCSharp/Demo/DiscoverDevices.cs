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
    public partial class DiscoverDevices : Form
    {
        private Dictionary<int, NET_SDK_DEVICE_DISCOVERY_INFO> m_deviceInfoList = new Dictionary<int, NET_SDK_DEVICE_DISCOVERY_INFO>();
        private NET_SDK_DEVICEINFO onsd = new NET_SDK_DEVICEINFO();
        private int userId = -1;
        public DiscoverDevices()
        {
            InitializeComponent();
            btn_OK_Click(null, null);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            int listNum = 100;
            NET_SDK_DEVICE_DISCOVERY_INFO[] discoverDev = new NET_SDK_DEVICE_DISCOVERY_INFO[listNum];
            int returnListNum = 0;
            int size = Marshal.SizeOf(typeof(NET_SDK_DEVICE_DISCOVERY_INFO));
            IntPtr devListInfo = PointArrayToIntPtr(discoverDev);
            IntPtr pTmp = devListInfo;
            int bufferSize = size * listNum;


            returnListNum = DevSdkHelper.NET_SDK_DiscoverDevice(devListInfo, bufferSize, 5);

            if (returnListNum > 0)
            {
                returnListNum = returnListNum > listNum ? listNum : returnListNum;

                lv_DevInfo.Items.Clear();
                m_deviceInfoList.Clear();
                for (int i = 0; i < returnListNum; i++, pTmp = IntPtr.Add(pTmp, size))
                {
                    byte[] pByte = new byte[size];
                    Marshal.Copy(pTmp, pByte, 0, size);
                    discoverDev[i] = (NET_SDK_DEVICE_DISCOVERY_INFO)BytesToStruct(pByte, typeof(NET_SDK_DEVICE_DISCOVERY_INFO));
                    ListViewItem item = new ListViewItem(ChangeToItems(discoverDev[i]));
                    lv_DevInfo.Items.Add(item);
                    m_deviceInfoList.Add(lv_DevInfo.Items.Count, discoverDev[i]);
                }
            }
            try { Marshal.FreeHGlobal(devListInfo); }
            catch { }

        }
        private IntPtr PointArrayToIntPtr(NET_SDK_DEVICE_DISCOVERY_INFO[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(NET_SDK_DEVICE_DISCOVERY_INFO));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
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

        private string[] ChangeToItems(NET_SDK_DEVICE_DISCOVERY_INFO file)
        {
            string devType = "";
            switch (file.deviceType)
            {
                case 0:
                    devType = "DVR";
                    break;
                case 1:
                    devType = "DVS";
                    break;
                case 2:
                    devType = "IPC";
                    break;
                case 3:
                    devType = "NVR";
                    break;

                case 5:
                    devType = "DECODER";
                    break;
                default:
                    devType = "other";
                    break;
            }
            //string productType = Encoding.Default.GetString((byte[])(Array)(file.productType));
            string productType = Encoding.Default.GetString(file.productType);
            productType = productType.Replace("\0", "");
            //string devIp = Encoding.Default.GetString((byte[])(Array)(file.strIP));
            string devIp = Encoding.Default.GetString(file.strIP);
            devIp = devIp.Replace("\0", "");

            string secondIp = Encoding.Default.GetString(file.dwSecondIP);
            secondIp = secondIp.Replace("\0", "");

            string[] tempStrings = { devIp, devType, productType, secondIp };
            return tempStrings;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void lv_DevInfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_DevInfo.HitTest(e.X, e.Y);
            if (info != null)
            {
                var videoitem = info.Item as ListViewItem;
                NET_SDK_DEVICE_DISCOVERY_INFO selectDev = m_deviceInfoList[videoitem.Index + 1];

                uint devType = selectDev.deviceType;
                string ip = Encoding.Default.GetString(selectDev.strIP);
                ushort port;
                if (devType == 0 || devType == 3 || devType == 2)
                {
                    if (devType == 2)
                        port = 9008;
                    else
                        port = 6036;
                    userId = DevSdkHelper.NET_SDK_LoginEx(ip, port, "admin", "123456", ref onsd, NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP, "");
                    onsd.deviceIP = DevSdkHelper.IpToInt(ip);
                    if (userId == -1)
                    {
                        MessageBox.Show("Login in failed! End ...");
                        return;
                    }
                    else
                    {
                        this.Hide();
                        Live_Dialog live_Dialog = new Live_Dialog(userId, onsd,"","");
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
        }

        private void btn_product_subID_Click(object sender, EventArgs e)
        {
            if (lv_DevInfo.SelectedItems.Count > 0)
            {
                int index = lv_DevInfo.SelectedItems[0].Index;
                NET_SDK_DEVICE_DISCOVERY_INFO selectDev = m_deviceInfoList[index + 1];

                uint devType = selectDev.deviceType;
                string ip = Encoding.Default.GetString(selectDev.strIP);
                ushort port;
                if (devType == 0 || devType == 3 || devType == 2)
                {
                    if (devType == 2)
                        port = 9008;
                    else
                        port = 6036;
                    userId = DevSdkHelper.NET_SDK_LoginEx(ip, port, "admin", "123456", ref onsd, NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP, "");
                    onsd.deviceIP = DevSdkHelper.IpToInt(ip);
                    if (userId == -1)
                    {
                        MessageBox.Show("Login in failed! End ...");
                        return;
                    }
                    else
                    {
                        this.Hide();
                        Live_Dialog live_Dialog = new Live_Dialog(userId, onsd, "admin", "123456");
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
        }
    }
}
