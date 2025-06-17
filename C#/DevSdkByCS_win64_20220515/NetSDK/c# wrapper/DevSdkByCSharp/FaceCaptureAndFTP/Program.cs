using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevSdkByCS;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Threading;
namespace FaceCaptureAndFTP
{
    class Program
    {
        const bool writeConsole = true;
        const bool writeLog = true;
        const bool faceDetect = true;
        const bool faceMatch = false;
        public static ACCEPT_REGISTER_CALLBACK arc = null;
        public static SUBSCRIBE_CALLBACK sbsc = null;
        public static EXCEPTION_CALLBACK excertion = null;
        public static Dictionary<int, NET_SDK_DEVICEINFO_EX> devList = new Dictionary<int, NET_SDK_DEVICEINFO_EX>();
        public static FtpHelper fh;
        public static string dir = AppDomain.CurrentDomain.BaseDirectory;
        public static UInt64 index = 1;

        private static Log theLog;
        static void Main(string[] args)
        {
            theLog = new Log();
            theLog.Initial(true, dir, "theLog");//

            string ftpServerIP = ConfigurationManager.AppSettings["ftpServerIP"];
            string ftpPath = ConfigurationManager.AppSettings["ftpPath"];
            string ftpUserName = ConfigurationManager.AppSettings["ftpUserName"];
            string ftpPsw = ConfigurationManager.AppSettings["ftpPsw"];
            string ftpPort = ConfigurationManager.AppSettings["ftpPort"];
            string registerPort = ConfigurationManager.AppSettings["registerPort"];

            fh = new FtpHelper(ftpServerIP, ftpPath, ftpUserName, ftpPsw);
            //CreateFolder();
            bool bResult = DevSdkHelper.NET_SDK_Init();
            WriteConsole("NET_SDK_Init :" + bResult);
            WriteLog("NET_SDK_Init :" + bResult);

            DevSdkHelper.NET_SDK_SetConnectTime();
            DevSdkHelper.NET_SDK_SetReconnect();

            bResult = DevSdkHelper.NET_SDK_SetRegisterPort(ushort.Parse(registerPort));
            WriteConsole("NET_SDK_SetRegisterPort :" + bResult);
            WriteLog("NET_SDK_SetRegisterPort :" + bResult);
            arc = acceptRegisterCallBack;
            bResult = DevSdkHelper.NET_SDK_SetRegisterCallback(arc, IntPtr.Zero);
            WriteConsole("NET_SDK_SetRegisterCallback :" + bResult);
            WriteLog("NET_SDK_SetRegisterCallback :" + bResult);

            sbsc = Subscribe;
            IntPtr pthis = new IntPtr();
            bResult = DevSdkHelper.NET_SDK_SetSubscribCallBack(sbsc, pthis);
            WriteConsole("NET_SDK_SetSubscribCallBack :" + bResult);
            WriteLog("NET_SDK_SetSubscribCallBack :" + bResult);


            excertion = excertionCallback;
            bResult = DevSdkHelper.NET_SDK_SetSDKMessageCallBack(0, IntPtr.Zero, excertion, null);

            while(Console.ReadLine().Trim() != "exit")
            {
                if (Console.ReadLine().Trim() == "port")
                {
                    WriteConsole(registerPort);
                }
                if (Console.ReadLine().Trim() == "device")
                {
                    if (devList.Count > 0)
                    {
                        WriteConsole("Get devices list :" + devList.Count + " devices connected");
                        WriteLog("Get devices list :" + devList.Count + " devices connected");
                        foreach (int key in devList.Keys)
                        {
                            WriteConsole(devList[key].devIP);
                            WriteLog(devList[key].devIP);
                        }
                    }
                    else
                    {
                        WriteConsole("Get devices list :" + "No device connected!");
                        WriteLog("Get devices list :" + "No device connected!");
                    }
                }
                Thread.Sleep(100);
            }
      
        }
        private static void excertionCallback(UInt32 dwType, Int32 lUserID, Int32 lHandle, IntPtr pUser)
        {
            if (DevSdkHelper.DEVICE_DISCONNECT == dwType)
            {
                string temp = devList[lUserID].devIP + " disconnected!";
                WriteConsole(temp);
                WriteLog(temp);
                devList.Remove(lUserID);
            }
            else if (DevSdkHelper.DEVICE_RECONNECT == dwType)
            {
                //string temp = devList[lUserID].devIP + " reconnected!";
                //WriteConsole(temp);
                //WriteLog(temp);
            }
            else if (DevSdkHelper.CHL_DISCONNECT == dwType)
            {
               
            }
            else if (DevSdkHelper.CHL_RECONNECT == dwType)
            {
               
            }
        }
        private static void WriteConsole(string _log)
        {
            if (writeConsole)
            {
                Console.WriteLine(_log);
            }
        }
        private static void WriteLog(string _log)
        {
            if (writeLog)
            {
                theLog.WriteLog(_log, 0);
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

                WriteConsole("connected to device:" + devInfoEx.devIP + " name is " + devInfoEx.devName);
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
                        WriteConsole("device:" + devInfoEx.devIP + " NET_SDK_SmartSubscrib vfd  " + ret);
                        WriteLog("device:" + devInfoEx.devIP + " NET_SDK_SmartSubscrib vfd  " + ret);
                    }

                   
                }
                if (faceMatch)
                {
                    int pos = (int)intelist.intelist_Vfd_Match;
                    if (((funcList & (1 << pos)) >> pos == 1) && (devInfoEx.devInfo.deviceType == 2))
                    {
                        bool ret = DevSdkHelper.NET_SDK_SmartSubscrib(devInfoEx.userId, 2, 0, ptrOfReply);
                        WriteConsole("device:" + devInfoEx.devIP + " NET_SDK_SmartSubscrib vfd match  " + ret);
                        WriteLog("device:" + devInfoEx.devIP + " NET_SDK_SmartSubscrib vfd match  " + ret);
                    }
                   
                }                             
                Marshal.FreeHGlobal(ptrOfReply);
                
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

                        //WriteConsole("source image data's lenth is  " + src.dataLen + " status is :" + src.status);
                       // WriteLog("source image data's lenth is  " + src.dataLen + " status is :" + src.status);

                        if (restLen < src.dataLen)
                        {
                            break;
                        }
                        if (src.dataLen > 0)
                        {
                            byte[] data = new byte[src.dataLen];
                            Marshal.Copy(pBuf, data, 0, (int)src.dataLen);

                            string folerPath = CreatePath(lUserID, time);
                            string picName = folerPath + "\\" + AddZero(time.Hour) + AddZero(time.Minute) + AddZero(time.Second) + "_" + index + "_bj.png"; 

                            File.WriteAllBytes(picName, data);
                            WriteConsole("saved source picture " + picName);
                            WriteLog("saved source picture " + picName);

                            index++;
                            pBuf += (int)src.dataLen;//地址偏移
                            restLen -= (int)src.dataLen;//剩余长度
                        }
                        if (restLen < head.faceCnt * Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T)))
                        {
                            break;
                        }
                        //WriteConsole("there are " + head.faceCnt + " faces in the source picture");
                        //WriteLog("there are " + head.faceCnt + " faces in the source picture");
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
                                WriteConsole("the temperature is:" + tt);
                                WriteLog("the temperature is:" + tt);
                            }
                            
                            //WriteConsole("the " + tempIndex + " face image data's lenth is  " + face.stFaceImgData.dataLen + " status is :" + face.stFaceImgData.status);
                            //WriteLog("the " + tempIndex + " face image data's lenth is  " + face.stFaceImgData.dataLen + " status is :" + face.stFaceImgData.status);
                            if (face.stFaceImgData.dataLen > 0)//face Image, 如果有
                            {                               
                                byte[] data = new byte[face.stFaceImgData.dataLen];
                                Marshal.Copy(pBuf, data, 0, (int)face.stFaceImgData.dataLen);

                                string folerPath = CreatePath(lUserID, time);
                                string picName = folerPath + "\\" + AddZero(time.Hour) + AddZero(time.Minute) + AddZero(time.Second) + "_" + (index-1) + "_" + face.faceId.ToString() + "_face.png"; 

                                File.WriteAllBytes(picName, data);
                                WriteConsole("saved face picture " + picName);
                                WriteLog("saved face picture " + picName);
                                //fh.Upload(dir + "\\FaceImg\\" + devList[lUserID].devName + "_" + face.faceId.ToString() + ".jpg");

                               
                                
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

                        WriteConsole("the temperature is:" + ((float)baseInfo.temperature) / 100);
                        WriteLog("the temperature is:" + ((float)baseInfo.temperature) / 100);

                        if (restLen < Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO)))
                        {
                            break;
                        }
                        NET_SDK_IVE_PICTURE_INFO pictureInfo = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//剩余长度
                        if(restLen < pictureInfo.iPicSize)
                        {
                            break;
                        }
                      
                        if (pictureInfo.iPicSize > 0)//图片数据(实时抓拍)
                        {
                               
                            //var time = DateTime.Parse((DateTime.Parse("1970-01-01 08:00:00").AddMilliseconds(baseInfo.i64SnapTime / 1000)).ToString());
                               
                            //pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                            //byte[] data = new byte[pictureInfo.iPicSize];
                            //Marshal.Copy(pBuf, data, 0, pictureInfo.iPicSize);
                            //File.WriteAllBytes(dir + "\\FaceMatchImg\\" + baseInfo.iSnapPicId.ToString() + ".jpg", data);

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
                            //pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_PICTURE_INFO));//地址偏移
                            //byte[] data = new byte[pictureInfo2.iPicSize];
                            //Marshal.Copy(pBuf, data, 0, pictureInfo2.iPicSize);
                            //File.WriteAllBytes(dir + "\\FaceTargetImg\\" + baseInfo.iSnapPicId.ToString() + ".jpg", data);
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
            }

        }
    }
}
