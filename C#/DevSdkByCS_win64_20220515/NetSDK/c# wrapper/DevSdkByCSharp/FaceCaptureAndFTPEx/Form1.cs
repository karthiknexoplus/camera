using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevSdkByCS;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace FaceCaptureAndFTPEx
{
    public partial class Form1 : Form
    {
        const bool writeLog = true;
        const bool faceDetect = true;
        const bool faceMatch = true;
        public static ACCEPT_REGISTER_CALLBACK arc = null;
        public static SUBSCRIBE_CALLBACK sbsc = null;
        public static EXCEPTION_CALLBACK excertion = null;
        public static Dictionary<int, NET_SDK_DEVICEINFO_EX> devList = new Dictionary<int, NET_SDK_DEVICEINFO_EX>();

        public static string dir = AppDomain.CurrentDomain.BaseDirectory;
        public static UInt64 index = 1;

        private static Log theLog;
        public static byte[] data = new byte[1024*1024];
        public Form1()
        {
            InitializeComponent();
            foreach (Control ct in groupBox1.Controls)
                ct.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            theLog = new Log();
            theLog.Initial(true, dir, "theLog");//


            //CreateFolder();
            bool bResult = DevSdkHelper.NET_SDK_Init();

            WriteLog("NET_SDK_Init :" + bResult);

            if(tb_Port.Text.Trim().Length < 1)
            {
                MessageBox.Show("端口错误");
                DevSdkHelper.NET_SDK_Cleanup();
                return;
            }
            ushort portnum = ushort.Parse(tb_Port.Text.Trim());

            bResult = DevSdkHelper.NET_SDK_SetRegisterPort(portnum, IntPtr.Zero, 0);
            WriteLog("NET_SDK_SetRegisterPort :" + bResult);
            arc = acceptRegisterCallBack;
            bResult = DevSdkHelper.NET_SDK_SetRegisterCallback(arc, IntPtr.Zero);
            WriteLog("NET_SDK_SetRegisterCallback :" + bResult);


            sbsc = Subscribe;
            IntPtr pthis = new IntPtr();
            bResult = DevSdkHelper.NET_SDK_SetSubscribCallBack(sbsc, IntPtr.Zero);
            WriteLog("NET_SDK_SetSubscribCallBack :" + bResult);


            excertion = excertionCallback;
            bResult = DevSdkHelper.NET_SDK_SetSDKMessageCallBack(0, IntPtr.Zero, excertion, null);

            foreach (Control ct in groupBox1.Controls)
                ct.Enabled = true;
            btn_start.Enabled = false;

        }
        private static void WriteLog(string _log)
        {
            if (writeLog)
            {
                theLog.WriteLog(_log, 0);
            }
        }
        private static void Subscribe(Int32 lUserID, Int32 dwCommand, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser)
        {
            switch (dwCommand)
            {
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_VFD:
                    {
                        /*NET_DVR_IVE_VFD_RESULT_HEAD_T+
                         [NET_DVR_IVE_VFD_RESULT_DATA_INFO_T+源数据]+
                         [NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T+人脸数据+人脸特征值]+...+
                         [NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T+人脸数据+人脸特征值*/

                        int restLen = (int)dwBufLen;
                        //解析头
                        NET_DVR_IVE_VFD_RESULT_HEAD_T head = (NET_DVR_IVE_VFD_RESULT_HEAD_T)Marshal.PtrToStructure(pBuf, typeof(NET_DVR_IVE_VFD_RESULT_HEAD_T));
                        pBuf += Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_HEAD_T));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_HEAD_T));//剩余长度
                        var time = DateTime.Parse((DateTime.Parse("1970-01-01 08:00:00").AddMilliseconds(head.time / 1000)).ToString());
                        if (restLen < Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_DATA_INFO_T)))
                        {
                            break;
                        }
                        //解析原始图片数据
                        NET_DVR_IVE_VFD_RESULT_DATA_INFO_T src = (NET_DVR_IVE_VFD_RESULT_DATA_INFO_T)Marshal.PtrToStructure(pBuf, typeof(NET_DVR_IVE_VFD_RESULT_DATA_INFO_T));
                        pBuf += Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_DATA_INFO_T));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_DATA_INFO_T));//剩余长度


                        if (restLen < src.dataLen)
                        {
                            break;
                        }
                        if (src.dataLen > 0)
                        {
                            /*byte[]*/ data = new byte[src.dataLen];
                            Marshal.Copy(pBuf, data, 0, (int)src.dataLen);

                            string folerPath = CreatePath(lUserID, time);

                            string picName = folerPath + "\\" + AddZero(time.Hour) + AddZero(time.Minute) + AddZero(time.Second) + "_" + index + "_bj.png";

                            File.WriteAllBytes(picName, data);

                            WriteLog("saved source picture " + picName);
                            Array.Clear(data, 0, data.Length);
                            index++;
                            pBuf += (int)src.dataLen;//地址偏移
                            restLen -= (int)src.dataLen;//剩余长度
                        }
                        if (restLen < head.faceCnt * Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T)))
                        {
                            break;
                        }

                        for (int i = 0; i < head.faceCnt; i++)
                        {
                            NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T face = (NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T)Marshal.PtrToStructure(pBuf, typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));
                            pBuf += Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));//地址偏移
                            restLen -= Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));//剩余长度

                            string tempIndex = (i + 1).ToString();
                            switch (i)
                            {
                                case 0:
                                    tempIndex += "st";
                                    break;
                                case 1:
                                    tempIndex += "ed";
                                    break;
                                case 2:
                                    tempIndex += "rd";
                                    break;
                                default:
                                    tempIndex += "th";
                                    break;
                            }
                            if (face.temperature > 0)
                            {
                                float tt = ((float)face.temperature) / 100;
                                WriteLog("the temperature is:" + tt);
                            }

                            if (face.stFaceImgData.dataLen > 0)//face Image, 如果有
                            {
                                /*byte[]*/ data = new byte[face.stFaceImgData.dataLen];
                                Marshal.Copy(pBuf, data, 0, (int)face.stFaceImgData.dataLen);

                                string folerPath = CreatePath(lUserID, time);
                                string picName = folerPath + "\\" + AddZero(time.Hour) + AddZero(time.Minute) + AddZero(time.Second) + "_" + (index - 1) + "_" + face.faceId.ToString() + "_face.png";

                                File.WriteAllBytes(picName, data);
                                WriteLog("saved face picture " + picName);


                                pBuf += (int)face.stFaceImgData.dataLen;//地址偏移
                                restLen -= (int)face.stFaceImgData.dataLen;//剩余长度
                            }
                            else
                            {

                            }

                        
                    }

            }

                    break;
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_FACE_MATCH_FOR_IPC:
                    {
                        //pBuf为NET_SDK_IVE_BASE_INFO+NET_SDK_IVE_PICTURE_INFO+图片数据(实时)+NET_SDK_IVE_PICTURE_INFO+图片数据(相册)+NET_SDK_IVE_PICTURE_INFO+原始背景图片数据
                        /*	----------------------
		                |	NET_SDK_IVE_BASE_INFO	|
		                -------------------------
		                |	NET_SDK_IVE_PICTURE_INFO	|
		                -------------------------
		                |	图片数据(实时)		|
		                -------------------------
		                |	NET_SDK_IVE_PICTURE_INFO	|
		                -------------------------
		                |	图片数据(相册)		|
                         -------------------------
		                |	NET_SDK_IVE_PICTURE_INFO	|
		                -------------------------
		                |	原始背景图片数据		|
		                -------------------------*/
                        int restLen = (int)dwBufLen;

                        NET_SDK_IVE_BASE_INFO baseInfo = (NET_SDK_IVE_BASE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_BASE_INFO));
                        pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_BASE_INFO));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_SDK_IVE_BASE_INFO));//剩余长度

                        WriteLog("the temperature is:" + ((float)baseInfo.temperature) / 100);

                        if (restLen < Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO)))
                        {
                            break;
                        }
                        NET_SDK_IVE_PICTURE_INFO pictureInfo = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//剩余长度
                        if (restLen < pictureInfo.iPicSize)
                        {
                            break;
                        }

                        if (pictureInfo.iPicSize > 0)//图片数据(实时抓拍)
                        {

                            //var time = DateTime.Parse((DateTime.Parse("1970-01-01 08:00:00").AddMilliseconds(baseInfo.i64SnapTime / 1000)).ToString());

                            pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                            /*byte[]*/ data = new byte[pictureInfo.iPicSize];
                            Marshal.Copy(pBuf, data, 0, pictureInfo.iPicSize);
                            File.WriteAllBytes(dir + "\\FaceMatchImg\\" + baseInfo.iSnapPicId.ToString() + ".jpg", data);

                            pBuf += pictureInfo.iPicSize;//地址偏移
                            restLen -= pictureInfo.iPicSize;//地址偏移
                        }
                        if (restLen < Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO)))
                        {
                            break;
                        }


                        NET_SDK_IVE_PICTURE_INFO pictureInfo2 = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//剩余长度

                        if (restLen < pictureInfo2.iPicSize)
                        {
                            break;
                        }
                        if (pictureInfo2.iPicSize > 0)//图片数据(相册)
                        {
                            pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                            /*byte[]*/ data = new byte[pictureInfo2.iPicSize];
                            Marshal.Copy(pBuf, data, 0, pictureInfo2.iPicSize);
                            File.WriteAllBytes(dir + "\\FaceTargetImg\\" + baseInfo.iSnapPicId.ToString() + ".jpg", data);
                            pBuf += pictureInfo2.iPicSize;//地址偏移
                            restLen -= pictureInfo2.iPicSize;//地址偏移

                        }

                        if (restLen < Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO)))
                        {
                            break;
                        }
                        NET_SDK_IVE_PICTURE_INFO pictureInfo3 = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//剩余长度
                        if (restLen < pictureInfo3.iPicSize)
                        {
                            break;
                        }
                        if (pictureInfo3.iPicSize > 0)//原始背景图片数据(相册)
                        {
                            pBuf += pictureInfo3.iPicSize;//地址偏移
                            restLen -= pictureInfo3.iPicSize;//地址偏移
                        }

                    }
                    break;
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_FACE_MATCH:
                    {
                        //NET_SDK_IVE_FACE_MATCH_T sIVE_AVD = new NET_SDK_IVE_FACE_MATCH_T();
                        //if (dwBufLen <= Marshal.SizeOf(sIVE_AVD))
                        //{
                        //    WriteLog("NET_DVR_SMART_FACE_MATCH len error dwBufLen=" + dwBufLen);
                        //    return;
                        //}

                        //sIVE_AVD = (NET_SDK_IVE_FACE_MATCH_T)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_FACE_MATCH_T));
                        ////var time = DateTime.Parse((DateTime.Parse("1970-01-01 08:00:00").AddMilliseconds(sIVE_AVD.time / 1000)).ToString());
                        //if (dwBufLen != (Marshal.SizeOf(sIVE_AVD) + sIVE_AVD.imgLen))
                        //{
                        //    WriteLog("NET_DVR_SMART_FACE_MATCH len error dwBufLen=" + dwBufLen);
                        //    return;
                        //}
                        //string tempLog = string.Format("dwRealFaceID={0}, dwGrpID= {1}, dwLibFaceID = {2}, Channel={3}, imgLen= {4}, dwSimilar={5}, byName = {6}\n", sIVE_AVD.dwRealFaceID, sIVE_AVD.dwGrpID, sIVE_AVD.dwLibFaceID, sIVE_AVD.Channel, sIVE_AVD.imgLen,
                        //    sIVE_AVD.dwSimilar, sIVE_AVD.byName);
                        //WriteLog(tempLog);


                        //byte[] data = new byte[sIVE_AVD.imgLen];
                        //Marshal.Copy(pBuf + Marshal.SizeOf(sIVE_AVD), data, 0, (Int32)sIVE_AVD.imgLen);


                        //string folerPath = CreatePath(lUserID, time);
                        //string picName = folerPath + "\\" + AddZero(time.Hour) + AddZero(time.Minute) + AddZero(time.Second) + "_" + (index - 1) + "_" + face.faceId.ToString() + "_face.png";

                        //File.WriteAllBytes(picName, data);

                        //WriteLog("saved face picture " + picName);

                    }
                    break;
            }
            data = null;
        }
        private static void excertionCallback(UInt32 dwType, Int32 lUserID, Int32 lHandle, IntPtr pUser)
        {
            if (DevSdkHelper.DEVICE_DISCONNECT == dwType)
            {
                string temp = devList[lUserID].devIP + " disconnected!";
                WriteLog(temp);
                devList.Remove(lUserID);
            }
            else if (DevSdkHelper.DEVICE_RECONNECT == dwType)
            {

            }
            else if (DevSdkHelper.CHL_DISCONNECT == dwType)
            {

            }
            else if (DevSdkHelper.CHL_RECONNECT == dwType)
            {

            }
        }
        private static string CreatePath(int userId, DateTime dt)
        {
            NET_SDK_DEVICEINFO_EX devInfoEx = devList[userId];
            string tempPath = "\\" + devInfoEx.registerId + "\\" + devInfoEx.devIP + "\\" + dt.Year.ToString() + AddZero(dt.Month) + AddZero(dt.Day);
            if (!Directory.Exists(dir + tempPath))//人脸检测原始图片存储路径   
            {
                Directory.CreateDirectory(dir + tempPath);
                WriteLog("CreateDirectory :" + dir + tempPath);

            }

            return dir + tempPath;

        }
        private static string AddZero(int time)
        {
            int a = time;
            return a > 9 ? a.ToString() : "0" + a;
        }
        private static void CreateFolder()
        {
            if (Directory.Exists(dir + "\\SourceImg"))//人脸检测原始图片存储路径
                CommonMethods.ClearFolder(dir + "\\SourceImg");
            else
                Directory.CreateDirectory(dir + "\\SourceImg");
            if (Directory.Exists(dir + "\\FaceImg"))//人脸IPC实时抓拍图片存储路径
                CommonMethods.ClearFolder(dir + "\\FaceImg");
            else
                Directory.CreateDirectory(dir + "\\FaceImg");
            if (Directory.Exists(dir + "\\FaceMatchImg"))//人脸IPC实时比对图片存储路径
                CommonMethods.ClearFolder(dir + "\\FaceMatchImg");
            else
                Directory.CreateDirectory(dir + "\\FaceMatchImg");
            if (Directory.Exists(dir + "\\FaceTargetImg"))//人脸IPC实时比对目标图片存储路径
                CommonMethods.ClearFolder(dir + "\\FaceTargetImg");
            else
                Directory.CreateDirectory("\\FaceTargetImg");
        }
        private static void acceptRegisterCallBack(Int32 lUserID, Int32 lRegisterID, IntPtr pDeviceInfo, IntPtr pUser)
        {
            if (!devList.ContainsKey(lUserID))
            {
                NET_SDK_DEVICEINFO_EX devInfoEx = new NET_SDK_DEVICEINFO_EX();
                devInfoEx.devInfo = (NET_SDK_DEVICEINFO)Marshal.PtrToStructure(pDeviceInfo, typeof(NET_SDK_DEVICEINFO));
                devInfoEx.devIP = devInfoEx.devInfo.GetDeviceIPStr();
                devInfoEx.userId = lUserID;
                devInfoEx.devName = devInfoEx.devInfo.GetDeviceName();
                devInfoEx.registerId = lRegisterID;
                devList.Add(lUserID, devInfoEx);

                WriteLog("connected to device:" + devInfoEx.devIP + " name is " + devInfoEx.devName);

                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib = new NET_DVR_SUBSCRIBE_REPLY();
                int nSizeOfReply = Marshal.SizeOf(sSmartSubscrib);
                IntPtr ptrOfReply = Marshal.AllocHGlobal(nSizeOfReply);
                Marshal.StructureToPtr(sSmartSubscrib, ptrOfReply, true);

                int funcList = (int)devInfoEx.devInfo.function[0];
                if (faceDetect)
                {
                    int pos = (int)intelist.intelist_Vfd;
                    if (((funcList & (1 << pos)) >> pos == 1) && (devInfoEx.devInfo.deviceType == 2))
                    {
                        bool ret = DevSdkHelper.NET_SDK_SmartSubscrib(devInfoEx.userId, 1, 0, ptrOfReply);
                        WriteLog("device:" + devInfoEx.devIP + " NET_SDK_SmartSubscrib vfd  " + ret);
                    }


                }
                if (faceMatch)
                {
                    int pos = (int)intelist.intelist_Vfd_Match;
                    if (((funcList & (1 << pos)) >> pos == 1) && (devInfoEx.devInfo.deviceType == 2))
                    {
                        bool ret = DevSdkHelper.NET_SDK_SmartSubscrib(devInfoEx.userId, 2, 0, ptrOfReply);

                        WriteLog("device:" + devInfoEx.devIP + " NET_SDK_SmartSubscrib vfd match  " + ret);
                    }

                }
                Marshal.FreeHGlobal(ptrOfReply);

            }
        }

        private IntPtr PointArrayToIntPtr(REG_LOGIN_INFO[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(REG_LOGIN_INFO));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }

        private void btn_AddListen_Click(object sender, EventArgs e)
        {
            REG_LOGIN_INFO rli = new REG_LOGIN_INFO();
            rli.deviceId = UInt32.Parse(tb_DeviceId.Text.Trim());
            rli.m_szUserName = new byte[36];
            rli.m_szPasswd = new byte[36];
            Array.Copy(Encoding.UTF8.GetBytes(tb_Name.Text.Trim()), rli.m_szUserName, Encoding.UTF8.GetBytes(tb_Name.Text.Trim()).Length);
            Array.Copy(Encoding.UTF8.GetBytes(tb_Psw.Text.Trim()), rli.m_szPasswd, Encoding.UTF8.GetBytes(tb_Psw.Text.Trim()).Length);

            int nSize = Marshal.SizeOf(rli);
            IntPtr mptr = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(rli, mptr, true);

            DevSdkHelper.NET_SDK_AddRegisterDeviceInfo(mptr, 1);
            Marshal.FreeHGlobal(mptr);
        }
    }
}
