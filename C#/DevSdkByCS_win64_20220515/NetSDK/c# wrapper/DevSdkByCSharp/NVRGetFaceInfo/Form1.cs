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
using System.IO;
using System.Xml;

namespace NVRGetFaceInfo
{
    //using POINTERHANDLE = Int32;
    using POINTERHANDLE = Int64;
    public partial class Form1 : Form
    {
        int liveHandle;
        private string m_username;
        private string m_password;
        private string m_ip;
        private ushort m_port;
        private uint m_num;
        private uint m_similarity;
        private static string m_path;
        private Dictionary<string, chlIpAndName> chls = new Dictionary<string, chlIpAndName>();
        private static Dictionary<int, Dictionary<string, chlIpAndName>> devices = new Dictionary<int, Dictionary<string, chlIpAndName>>();
        private static Dictionary<int, NET_SDK_DEVICEINFO> deviceInfos = new Dictionary<int, NET_SDK_DEVICEINFO>();
        int userId = 0;
        NET_SDK_DEVICEINFO oNET_SDK_DEVICEINFO = new NET_SDK_DEVICEINFO();
        private List<NET_SDK_FACE_IMG_INFO_CH> m_snapList = new List<NET_SDK_FACE_IMG_INFO_CH>();
        public static SUBSCRIBE_CALLBACK sbsc = null;
        public static ACCEPT_REGISTER_CALLBACK arc = null;
        public static byte[] data;
        private bool subed = false;
        private bool inited = false;
        private static object lockObjStatic = new object();

        struct chlIpAndName
        {
            public string ip;
            public string name;
        }
        public Form1()
        {
            InitializeComponent();
            DateTime now = DateTime.Now;
            DateTime tempDt = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dtp_SmartSearchStart.Value = tempDt;
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            GetDlgItemText();
            bool bResult = DevSdkHelper.NET_SDK_Init();
            if (!bResult)
                return;

            sbsc = Subscribe;
            bResult = DevSdkHelper.NET_SDK_SetSubscribCallBack(sbsc, IntPtr.Zero);

            userId = DevSdkHelper.NET_SDK_LoginEx(m_ip, m_port, m_username, m_password, ref oNET_SDK_DEVICEINFO, NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP, "");
            if (userId > 0)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
                GetChlsIP();
            }
            else
                MessageBox.Show("连接失败");
        }
        private void GetDlgItemText()
        {
            m_username = tb_UserName.Text.Trim();
            m_password = tb_Psw.Text.Trim();

            m_ip = tb_IP.Text.Trim();
            string temp = tb_Port.Text.Trim();
            if (temp.Length > 0)
                m_port = Convert.ToUInt16(temp);
            else
                m_port = 0;

            string strnum = tb_Num.Text.Trim();
            if (strnum.Length > 0)
                m_num = Convert.ToUInt32(strnum);
            else
                m_num = 0;

            string strsim = tb_Similarity.Text.Trim();
            if (strsim.Length > 0)
                m_similarity = Convert.ToUInt32(strsim);
            else
                m_similarity = 0;

            m_path = tb_BackupPath.Text.Trim();
            if (!m_path.EndsWith("\\"))
                m_path += "\\";
        }

        private void btn_Browser_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tb_BackupPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private string GetCert(NET_SDK_FACE_IMG_INFO_CH imgInfo)
        {
            string cert = "";
            NET_SDK_FACE_INFO_LIST_GET nsfilg = new NET_SDK_FACE_INFO_LIST_GET();
            nsfilg.pageIndex = 1;
            nsfilg.pageSize = 1;

            nsfilg.itemId = imgInfo.targetImgId;
            IntPtr lpInputBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(nsfilg));
            Marshal.StructureToPtr(nsfilg, lpInputBuffer, true);
            IntPtr lpOutputBuffer = Marshal.AllocHGlobal(1000 * 1024);
            int lpBytesReturned = 0;

            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, (UInt32)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_INFO,
                lpInputBuffer, (uint)Marshal.SizeOf(nsfilg), lpOutputBuffer, 1000 * 1024, ref lpBytesReturned);
            if (ret && lpBytesReturned > 0)
            {
                int nSizeOfFaceItem = Marshal.SizeOf(typeof(NET_SDK_FACE_INFO_LIST_ITEM));
                NET_SDK_FACE_INFO_LIST nsfil = (NET_SDK_FACE_INFO_LIST)Marshal.PtrToStructure(lpOutputBuffer, typeof(NET_SDK_FACE_INFO_LIST));

                NET_SDK_FACE_INFO_LIST_ITEM Face_Item = (NET_SDK_FACE_INFO_LIST_ITEM)Marshal.PtrToStructure(nsfil.pFaceInfoListItem, typeof(NET_SDK_FACE_INFO_LIST_ITEM));
                cert = DevSdkHelper.ByteToStr(Face_Item.certificateNum);               
            }
            Marshal.FreeHGlobal(lpOutputBuffer);
            Marshal.FreeHGlobal(lpInputBuffer);
            return cert;
        }
       
        private byte[] GetFace(NET_SDK_FACE_IMG_INFO_CH imgInfo)
        {
            byte[] res = null;
            imgInfo.isPanorama = 0;
            IntPtr lpInputBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(imgInfo));
            Marshal.StructureToPtr(imgInfo, lpInputBuffer, true);
            IntPtr lpOutputBuffer = Marshal.AllocHGlobal(1000 * 1024);

            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, (UInt32)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_CH_SNAP_FACE_IMG,
                lpInputBuffer, (uint)Marshal.SizeOf(imgInfo), lpOutputBuffer, 1000 * 1024, ref lpBytesReturned);
            if (ret && lpBytesReturned > 0)
            {
                NET_SDK_FACE_INFO_IMG_DATA imgdata = (NET_SDK_FACE_INFO_IMG_DATA)Marshal.PtrToStructure(lpOutputBuffer, typeof(NET_SDK_FACE_INFO_IMG_DATA));

                res = new byte[imgdata.imgLen];
                Marshal.Copy(imgdata.imgData, res, 0, (int)imgdata.imgLen);
            }

            Marshal.FreeHGlobal(lpOutputBuffer);
            Marshal.FreeHGlobal(lpInputBuffer);
            return res;
        }
        private byte[] GetScene(NET_SDK_FACE_IMG_INFO_CH imgInfo)
        {
            byte[] res = null;
            imgInfo.isPanorama = 1;
            IntPtr lpInputBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(imgInfo));
            Marshal.StructureToPtr(imgInfo, lpInputBuffer, true);
            IntPtr lpOutputBuffer = Marshal.AllocHGlobal(1000 * 1024);

            int lpBytesReturned = 0;
            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, (UInt32)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_CH_SNAP_FACE_IMG,
                lpInputBuffer, (uint)Marshal.SizeOf(imgInfo), lpOutputBuffer, 1000 * 1024, ref lpBytesReturned);
            if (ret && lpBytesReturned > 0)
            {
                NET_SDK_FACE_INFO_IMG_DATA imgdata = (NET_SDK_FACE_INFO_IMG_DATA)Marshal.PtrToStructure(lpOutputBuffer, typeof(NET_SDK_FACE_INFO_IMG_DATA));
                res = new byte[imgdata.imgLen];
                Marshal.Copy(imgdata.imgData, res, 0, (int)imgdata.imgLen);
            }
            Marshal.FreeHGlobal(lpOutputBuffer);
            Marshal.FreeHGlobal(lpInputBuffer);
            return res;
        }

        private void btn_Export_Click(object sender, EventArgs e)
        {

            GetDlgItemText();
            NET_SDK_SEARCH_IMAGE_BY_IMAGE_V2 nssibiv = new NET_SDK_SEARCH_IMAGE_BY_IMAGE_V2();
            nssibiv.limitNum = m_num;

            nssibiv.startTime.year = Convert.ToUInt16(dtp_SmartSearchStart.Value.Year);
            nssibiv.startTime.month = Convert.ToByte(dtp_SmartSearchStart.Value.Month);
            nssibiv.startTime.mday = Convert.ToByte(dtp_SmartSearchStart.Value.Day);

            nssibiv.startTime.hour = Convert.ToByte(dtp_SmartSearchStart.Value.Hour);
            nssibiv.startTime.minute = Convert.ToByte(dtp_SmartSearchStart.Value.Minute);
            nssibiv.startTime.second = Convert.ToByte(dtp_SmartSearchStart.Value.Second);

            nssibiv.endTime.year = Convert.ToUInt16(dtp_SmartSearchEnd.Value.Year);
            nssibiv.endTime.month = Convert.ToByte(dtp_SmartSearchEnd.Value.Month);
            nssibiv.endTime.mday = Convert.ToByte(dtp_SmartSearchEnd.Value.Day);
            nssibiv.endTime.hour = Convert.ToByte(dtp_SmartSearchEnd.Value.Hour);
            nssibiv.endTime.minute = Convert.ToByte(dtp_SmartSearchEnd.Value.Minute);
            nssibiv.endTime.second = Convert.ToByte(dtp_SmartSearchEnd.Value.Second);

            nssibiv.similarity = m_similarity;
            nssibiv.searchType = DevSdkHelper.SEARCH_IMAGE_BY_EVENT;
            nssibiv.eventType = DevSdkHelper.BY_WHITELIST;

            int lpBytesReturned = 0;
            uint dwCommand = (UInt32)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_IMAGE_BY_IMG_V2;

            int nSize = Marshal.SizeOf(nssibiv);
            IntPtr inputPtr = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(nssibiv, inputPtr, true);

            IntPtr outputPtr = Marshal.AllocHGlobal(100 * 1024);
            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, inputPtr, (uint)nSize, outputPtr, 100 * 1024, ref lpBytesReturned);
            if (ret && lpBytesReturned > 0)
            {
                NET_SDK_CH_SNAP_FACE_IMG_LIST Img_List_Out = new NET_SDK_CH_SNAP_FACE_IMG_LIST();
                int nSizeofImgListOut = Marshal.SizeOf(Img_List_Out);
                Img_List_Out = (NET_SDK_CH_SNAP_FACE_IMG_LIST)Marshal.PtrToStructure(outputPtr, typeof(NET_SDK_CH_SNAP_FACE_IMG_LIST));
                if (Img_List_Out.listNum > 0)
                {
                    groupBox2.Enabled = false;
                    progressBar1.Maximum = (int)Img_List_Out.listNum;
                    progressBar1.Value = 0;
                    NET_SDK_FACE_IMG_INFO_CH Img_info = new NET_SDK_FACE_IMG_INFO_CH();
                    int nSizeofImgInfo = Marshal.SizeOf(Img_info);
                    for (int i = 0; i < Img_List_Out.listNum; ++i)
                    {
                        Img_info = (NET_SDK_FACE_IMG_INFO_CH)Marshal.PtrToStructure(outputPtr + nSizeofImgListOut + i * nSizeofImgInfo, typeof(NET_SDK_FACE_IMG_INFO_CH));
                        string tempPath = m_path + oNET_SDK_DEVICEINFO.deviceID + "_" + chls[(Img_info.chl + 1).ToString()];
                        if (!File.Exists(tempPath))
                            Directory.CreateDirectory(tempPath);
                        string time = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", Img_info.frameTime.year, Img_info.frameTime.month, Img_info.frameTime.mday,
                            Img_info.frameTime.hour, Img_info.frameTime.minute, Img_info.frameTime.second);
                        //获取身份证号
                        string cert = GetCert(Img_info);
                        //获取人脸图
                        string fileName = tempPath + "\\" + cert + "-" + time + "-" + "RL.jpg";
                        File.WriteAllBytes(fileName, GetFace(Img_info));
                        //获取背景图
                        fileName = tempPath + "\\" + cert + "-" + time + "-" + "CJ.jpg";
                        File.WriteAllBytes(fileName, GetScene(Img_info));
                        progressBar1.Value = i + 1;
                    }
                    groupBox2.Enabled = true;
                }
                else
                    MessageBox.Show("该时间段无比对记录");
            }
            else
                MessageBox.Show("该时间段无比对记录");
            Marshal.FreeHGlobal(inputPtr);
            Marshal.FreeHGlobal(outputPtr);
        }
        private void GetChlsIP()
        {
            chls.Clear();
            string szUrl = "queryDevList";
            string sendXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'>
                                <requireField>
                                <ip/><chlNum/>
                                </requireField>
                            </request>";
            int lpBytesReturned = 0;
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(100 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXML, szUrl, lpOutBuffer, 100 * 1024, ref lpBytesReturned);
            if (ret)
            {
                byte[] rec = new byte[lpBytesReturned];
                Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                string retStr = DevSdkHelper.ByteToStr(rec);

                XmlDocument oldXd = new XmlDocument();
                oldXd.LoadXml(retStr);
                XmlNodeList nodeList = oldXd.GetElementsByTagName("item");
                if (nodeList.Count > 0)
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        string chlNum = "";
                        string chlIP = "";
                        string chlName = "";
                        XmlNodeList childNodeList = nodeList[i].ChildNodes;
                        for (int j = 0; j < childNodeList.Count; j++)
                        {
                            if (childNodeList[j].Name == "chlNum")
                            {
                                chlNum = childNodeList[j].InnerText;
                                break;
                            }
                        }
                        for (int j = 0; j < childNodeList.Count; j++)
                        {
                            if (childNodeList[j].Name == "ip")
                            {
                                chlIP = childNodeList[j].InnerText;
                                break;
                            }
                        }
                        for (int j = 0; j < childNodeList.Count; j++)
                        {
                            if (childNodeList[j].Name == "name")
                            {
                                chlName = childNodeList[j].InnerText;
                                break;
                            }
                        }
                        chlIpAndName tempchl = new chlIpAndName();
                        tempchl.ip = chlIP;
                        tempchl.name = chlName;

                        chls.Add(chlNum, tempchl);

                        //chls.Add(chlNum, chlIP);
                    }
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }
        private void GetChlsIP(int uid)
        {
            chls.Clear();
            string szUrl = "queryDevList";
            string sendXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'>
                                <requireField>
                                <ip/><chlNum/><name/>
                                </requireField>
                            </request>";
            int lpBytesReturned = 0;
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(100 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(uid, sendXML, szUrl, lpOutBuffer, 100 * 1024, ref lpBytesReturned);
            if (ret)
            {
                byte[] rec = new byte[lpBytesReturned];
                Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                string retStr = DevSdkHelper.ByteToStr(rec);
                if (retStr.Contains("failed"))
                {
                    return;
                }

                XmlDocument oldXd = new XmlDocument();
                oldXd.LoadXml(retStr);
                XmlNodeList nodeList = oldXd.GetElementsByTagName("item");
                if (nodeList.Count > 0)
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        string chlNum = "";
                        string chlIP = "";
                        string chlName = "";
                        XmlNodeList childNodeList = nodeList[i].ChildNodes;
                        for (int j = 0; j < childNodeList.Count; j++)
                        {
                            if (childNodeList[j].Name == "chlNum")
                            {
                                chlNum = childNodeList[j].InnerText;
                                break;
                            }
                        }
                        for (int j = 0; j < childNodeList.Count; j++)
                        {
                            if (childNodeList[j].Name == "ip")
                            {
                                chlIP = childNodeList[j].InnerText;
                                break;
                            }
                        }
                        for (int j = 0; j < childNodeList.Count; j++)
                        {
                            if (childNodeList[j].Name == "name")
                            {
                                chlName = childNodeList[j].InnerText;
                                break;
                            }
                        }
                        chlIpAndName tempchl = new chlIpAndName();
                        tempchl.ip = chlIP;
                        tempchl.name = chlName;

                        chls.Add(chlNum, tempchl);
                        //chls.Add(chlNum, chlIP);
                    }
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btn_AddLogin_Click(object sender, EventArgs e)
        {
            GetDlgItemTextEx();
            bool bResult = false;
            if (!inited)
            {
                bResult = DevSdkHelper.NET_SDK_Init();
                if (!bResult)
                    return;
                inited = true;
            }            
            if (!subed)
            {
                sbsc = Subscribe;
                IntPtr pthis = new IntPtr();
                bResult = DevSdkHelper.NET_SDK_SetSubscribCallBack(sbsc, pthis);
                if (bResult)
                    subed = true;
            }
           
            userId = DevSdkHelper.NET_SDK_LoginEx(m_ip, m_port, m_username, m_password, ref oNET_SDK_DEVICEINFO, NET_SDK_CONNECT_TYPE.NET_SDK_CONNECT_TCP, "");
            if (userId > 0)
            {
                
                GetChlsIP(userId);
                lock (lockObjStatic)
                {
                    devices.Add(userId, chls);
                    deviceInfos.Add(userId, oNET_SDK_DEVICEINFO);
                }
                
                MessageBox.Show("连接成功");
            }
            else
                MessageBox.Show("连接失败");
        }
        private void GetDlgItemTextEx()
        {
            m_username = tb_UserNameEx.Text.Trim();
            m_password = tb_PswEx.Text.Trim();

            m_ip = tb_IPEx.Text.Trim();
            string temp = tb_PortEx.Text.Trim();
            if (temp.Length > 0)
                m_port = Convert.ToUInt16(temp);
            else
                m_port = 0;

            m_path = tb_BackupPathEx.Text.Trim();
            //char[] invalidPath = Path.GetInvalidPathChars();
            //if (!m_path.EndsWith("\\"))
            //    m_path += "\\";
        }
        private static void Subscribe(Int32 lUserID, Int32 dwCommand, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser)
        {
            switch (dwCommand)
            {
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_FACE_MATCH:
                    {
                        IntPtr tmpP = pBuf;
                        NET_SDK_IVE_FACE_MATCH_T sIVE_AVD = new NET_SDK_IVE_FACE_MATCH_T();
                        sIVE_AVD = (NET_SDK_IVE_FACE_MATCH_T)Marshal.PtrToStructure(tmpP, typeof(NET_SDK_IVE_FACE_MATCH_T));

                        tmpP += Marshal.SizeOf(sIVE_AVD);//地址偏移
                        data = new byte[sIVE_AVD.imgLen];
                        Marshal.Copy(tmpP, data, 0, (Int32)sIVE_AVD.imgLen);

                        tmpP += (Int32)sIVE_AVD.imgLen;//地址偏移

                        NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO faceInfo = new NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO();
                        faceInfo = (NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO)Marshal.PtrToStructure(tmpP, typeof(NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO));

                        tmpP += Marshal.SizeOf(faceInfo);//地址偏移
                        //tmpP += 172;

                        if (!File.Exists(m_path))
                        {
                            try
                            {
                                Directory.CreateDirectory(m_path);
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show("路径错误");
                            }
                            
                        }
                        try
                        {
                            string tempPath = "";
                            string chlName = "";
                            lock (lockObjStatic)
                            {
                                //tempPath = m_path + "\\"+ deviceInfos[lUserID].deviceID + "_" + devices[lUserID][(sIVE_AVD.Channel + 1).ToString()].ip;
                                tempPath = m_path.TrimEnd('\\') + "\\";
                                chlName = devices[lUserID][(sIVE_AVD.Channel + 1).ToString()].name;
                            }
                            
                            if (!File.Exists(tempPath))
                                Directory.CreateDirectory(tempPath);

                            string time = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", sIVE_AVD.frameTime.year, sIVE_AVD.frameTime.month, sIVE_AVD.frameTime.mday,
                               sIVE_AVD.frameTime.hour, sIVE_AVD.frameTime.minute, sIVE_AVD.frameTime.second);

                            //获取人员名称
                            //string cert = "123";
                            string cert = DevSdkHelper.ByteToStr(faceInfo.szName);

                            //保存人脸图
                            string fileName = tempPath + "\\" + cert + "-" + time + "-" + chlName + "-"+ "RL.jpg";
                            File.WriteAllBytes(fileName, data);

                            if (sIVE_AVD.dwLibFaceID > 0)//比对成功，0表示是陌生人
                            {
                                NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T faceData = (NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T)Marshal.PtrToStructure(tmpP, typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));
                                tmpP += Marshal.SizeOf(faceData);//地址偏移
                                tmpP += (int)faceData.stFaceImgData.dataLen;//地址偏移 stFaceImgData是人脸目标图

                                NET_SDK_TLV_BUFFER_DESC pbuffer = (NET_SDK_TLV_BUFFER_DESC)Marshal.PtrToStructure(tmpP, typeof(NET_SDK_TLV_BUFFER_DESC));
                                tmpP += Marshal.SizeOf(pbuffer);//地址偏移
                                data = new byte[pbuffer.dwSize];
                                Marshal.Copy(tmpP, data, 0, (int)pbuffer.dwSize);

                                //保存背景图
                                fileName = tempPath + "\\" + cert + "-" + time + "-" + chlName + "-" + "CJ.jpg";
                                File.WriteAllBytes(fileName, data);
                            }

                           
                        }
                        catch (System.Exception ex)
                        {
                            //MessageBox.Show("路径错误");
                        }
                        
                    }
                    break;
            }
        }

        private void btn_BrowserEx_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tb_BackupPathEx.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btn_GetDevList_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            lock (lockObjStatic)
            {
                sb.Append("一共连接了" + deviceInfos.Count + "个设备" + "\r\n");
                foreach (int key in deviceInfos.Keys)
                {
                    sb.Append("userId:" + key + " deviceName:" + DevSdkHelper.ByteToStr(deviceInfos[key].deviceName) + "\r\n");
                }
            }           
            MessageBox.Show(sb.ToString());
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = radioButton1.Checked;
            groupBox5.Enabled = radioButton2.Checked;
            foreach (Control ct in groupBox4.Controls)
            {
                ct.Enabled = radioButton1.Checked;
            }
            foreach (Control ct in groupBox5.Controls)
            {
                ct.Enabled = !radioButton1.Checked;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = radioButton1.Checked;
            groupBox5.Enabled = radioButton2.Checked;
            foreach (Control ct in groupBox4.Controls)
            {
                ct.Enabled = !radioButton2.Checked;
            }
            foreach (Control ct in groupBox5.Controls)
            {
                ct.Enabled = radioButton2.Checked;
            }
        }

        private void btn_AddListen_Click(object sender, EventArgs e)
        {
            REG_LOGIN_INFO rli = new REG_LOGIN_INFO();
            rli.deviceId = UInt32.Parse(tb_DeviceId.Text.Trim());
            rli.m_szUserName = new byte[36];
            rli.m_szPasswd = new byte[36];
            Array.Copy(Encoding.UTF8.GetBytes(tb_Name_L.Text.Trim()), rli.m_szUserName, Encoding.UTF8.GetBytes(tb_Name_L.Text.Trim()).Length);
            Array.Copy(Encoding.UTF8.GetBytes(tb_Psw_L.Text.Trim()), rli.m_szPasswd, Encoding.UTF8.GetBytes(tb_Psw_L.Text.Trim()).Length);

            int nSize = Marshal.SizeOf(rli);
            IntPtr mptr = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(rli, mptr, true);

            DevSdkHelper.NET_SDK_AddRegisterDeviceInfo(mptr, 1);
            Marshal.FreeHGlobal(mptr);
        }

        private void btn_startListen_Click(object sender, EventArgs e)
        {
            m_path = tb_BackupPathEx.Text.Trim();
            bool bResult = false;
            if (!inited)
            {
                bResult = DevSdkHelper.NET_SDK_Init();
                if (!bResult)
                    return;
                inited = true;
            }

            if (tb_Port_L.Text.Trim().Length < 1)
            {
                MessageBox.Show("端口错误");
                DevSdkHelper.NET_SDK_Cleanup();
                inited = false;
                return;
            }
            ushort portnum = ushort.Parse(tb_Port_L.Text.Trim());

            bResult = DevSdkHelper.NET_SDK_SetRegisterPort(portnum, IntPtr.Zero, 0);

            arc = acceptRegisterCallBack;
            bResult = DevSdkHelper.NET_SDK_SetRegisterCallback(arc, IntPtr.Zero);


            if (!subed)
            {
                sbsc = Subscribe;
                IntPtr pthis = new IntPtr();
                bResult = DevSdkHelper.NET_SDK_SetSubscribCallBack(sbsc, pthis);
                if (bResult)
                    subed = true;
            }
           
        }
        private void acceptRegisterCallBack(Int32 lUserID, Int32 lRegisterID, IntPtr pDeviceInfo, IntPtr pUser)
        {
            GetChlsIP(lUserID);
            lock (lockObjStatic)
            {
                devices.Add(lUserID, chls);
                NET_SDK_DEVICEINFO nsd = (NET_SDK_DEVICEINFO)Marshal.PtrToStructure(pDeviceInfo,typeof(NET_SDK_DEVICEINFO));
                deviceInfos.Add(lUserID, nsd);
            }
                                     
        }
    }
}
