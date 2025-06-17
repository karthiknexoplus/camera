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
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    partial class Live_Dialog
    {
        private Dictionary<int, NET_SDK_LOG> m_logList = new Dictionary<int, NET_SDK_LOG>();
        private Dictionary<int, NET_SDK_EVENT> m_eventInfoList = new Dictionary<int, NET_SDK_EVENT>();
        private Dictionary<int, string> m_LogTypeList = new Dictionary<int, string>();
        private void LogTypeInit()
        {
            m_LogTypeList.Add(256, "LOG_ALARM_MOTION");
            m_LogTypeList.Add(257, "LOG_ALARM_SENSOR");
            m_LogTypeList.Add(258, "LOG_ALARM_ALARMOUTPUT");
            m_LogTypeList.Add(259, "LOG_OPERATE_RECORD_SPB");
            m_LogTypeList.Add(260, "LOG_OPERATE_MANUAL_RECORD");
            m_LogTypeList.Add(261, "LOG_OPERATE_MANUAL_ALARM");
            m_LogTypeList.Add(262, "LOG_OPERATE_SYSTEM_MAINTENANCE");
            m_LogTypeList.Add(263, "LOG_OPERATE_PTZ_CONTROL");
            m_LogTypeList.Add(264, "LOG_OPERATE_AUDIO_TALK");
            m_LogTypeList.Add(265, "LOG_OPERATE_SYSTEM_SCR");
            m_LogTypeList.Add(266, "LOG_OPERATE_LOGIN_LOGOUT");
            m_LogTypeList.Add(267, "LOG_OPERATE_SNAPSHOT_MSPB");
            m_LogTypeList.Add(268, "LOG_OPERATE_FORMAT_HD");
            m_LogTypeList.Add(269, "LOG_CONFIG_CHANNEL");
            m_LogTypeList.Add(270, "LOG_CONFIG_RECORD");
            m_LogTypeList.Add(271, "LOG_CONFIG_ALARM");
            m_LogTypeList.Add(272, "LOG_CONFIG_DISK");
            m_LogTypeList.Add(273, "LOG_CONFIG_NETWORK");
            m_LogTypeList.Add(274, "LOG_CONFIG_SCHEDULE");
            m_LogTypeList.Add(275, "LOG_CONFIG_USER");
            m_LogTypeList.Add(276, "LOG_CONFIG_BASIC");
            m_LogTypeList.Add(277, "LOG_EXCEPTION_UNLAWFUL_ACCESS");
            m_LogTypeList.Add(278, "LOG_EXCEPTION_DISK_FULL");
            m_LogTypeList.Add(279, "LOG_EXCEPTION_DISK_IO_ERROR");
            m_LogTypeList.Add(280, "LOG_EXCEPTION_IP_COLLISION");
            m_LogTypeList.Add(281, "LOG_EXCEPTION_INTERNET_DISCONNECT");
            m_LogTypeList.Add(282, "LOG_EXCEPTION_IPC_DISCONNECT");
            m_LogTypeList.Add(283, "LOG_EXCEPTION_ABNORMAL_SHUTDOWN");
            m_LogTypeList.Add(284, "LOG_EXCEPTION_NO_DISK");
            m_LogTypeList.Add(285, "LOG_EXCEPTION_VIDEO_LOSS");

        }

        private bool m_isOpenMsgCallback = false;
        private List<int> m_msgCBHandle = new List<int>();
        private Msg_Dialog mg;
        public static NET_MESSAGE_CALLBACK_EX nmc = null;

        private bool m_bManualAlarm = false;

        private bool m_isStartVoiceComm_MR = false;
        private bool m_isStartVoiceComm = false;
        private int m_talkTransHandle = -1;
        private int m_audioDecoderHandle = -1;
        private int m_voiceHandle = -1;
        public TALK_DATA_CALLBACK tdc = null;

        private bool m_SubscribAVD = false;
        private bool m_SubscribVehicle = false;
        private bool m_SubscribVFD = false;
        private bool m_SubscribFaceMatch = false;//人脸比对订阅开关
        private byte[] m_serverAddressAVD;
        private byte[] m_serverAddressVFD;

        private bool m_SubscribPassLine = false;//订阅过线统计
        public static SUBSCRIBE_CALLBACK sbsc = null;

        private MyWaveOut m_waveOut;
        private WaveInRecorder m_WaveIn;
        private BufferDoneEventHandler bdeh = null;

        private void btn_SearchLog_Click(object sender, EventArgs e)
        {

            DD_TIME st = new DD_TIME()
            {
                year = (ushort)(dtp_start.Value.Year - 1900),
                month = Convert.ToByte(dtp_start.Value.Month - 1),
                mday = Convert.ToByte(dtp_start.Value.Day),
                hour = Convert.ToByte(dtp_start.Value.Hour),
                minute = Convert.ToByte(dtp_start.Value.Minute),
                second = Convert.ToByte(dtp_start.Value.Second)
            };
            DD_TIME et = new DD_TIME()
            {
                year = (ushort)(dtp_end.Value.Year - 1900),
                month = Convert.ToByte(dtp_end.Value.Month - 1),
                mday = Convert.ToByte(dtp_end.Value.Day),
                hour = Convert.ToByte(dtp_end.Value.Hour),
                minute = Convert.ToByte(dtp_end.Value.Minute),
                second = Convert.ToByte(dtp_end.Value.Second)
            };
            #region find log

            POINTERHANDLE loghandle = DevSdkHelper.NET_SDK_FindDVRLog(userId, 1, ref st, ref et);
            int result = 0;
            NET_SDK_LOG log = new NET_SDK_LOG();
            int size = Marshal.SizeOf(typeof(NET_SDK_LOG));
            IntPtr plog = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(log, plog, true);

            if (loghandle > 0)
            {
                lv_Log.Items.Clear();
                m_logList.Clear();
                while (true)
                {
                    result = DevSdkHelper.NET_SDK_FindNextLog(loghandle, plog);
                    if (result == 87 || result == 88)
                        break;
                    if (result == 86)
                    {
                        MessageBox.Show("Find nothing");
                        break;
                    }
                    log = (NET_SDK_LOG)Marshal.PtrToStructure(plog, typeof(NET_SDK_LOG));
                    ListViewItem item = new ListViewItem(ChangeToItems(log));
                    lv_Log.Items.Add(item);
                    m_logList.Add(lv_Log.Items.Count, log);

                }
                DevSdkHelper.NET_SDK_FindLogClose(loghandle);
            }
            Marshal.FreeHGlobal(plog);

            #endregion
            #region find eventInfo

            //int loghandle = DevSdkHelper.NET_SDK_FindEventInfo(userId, 4, 1, ref st, ref et);
            //int result = 0;
            //NET_SDK_EVENT log = new NET_SDK_EVENT();
            //int size = Marshal.SizeOf(typeof(NET_SDK_EVENT));
            //IntPtr plog = Marshal.AllocHGlobal(size);
            //Marshal.StructureToPtr(log, plog, true);

            //if (loghandle > 0)
            //{
            //    lv_Log.Items.Clear();
            //    m_eventInfoList.Clear();
            //    while (true)
            //    {
            //        result = DevSdkHelper.NET_SDK_FindNextEventInfo(loghandle, plog);
            //        if (result == 87)
            //            break;
            //        log = (NET_SDK_EVENT)Marshal.PtrToStructure(plog, typeof(NET_SDK_EVENT));
            //        ListViewItem item = new ListViewItem(ChangeToItems(log));
            //        lv_Log.Items.Add(item);
            //        m_eventInfoList.Add(lv_Log.Items.Count, log);

            //    }
            //    DevSdkHelper.NET_SDK_FindEventInfoClose(loghandle);
            //}
            //Marshal.FreeHGlobal(plog);

            #endregion

        }
        private string[] ChangeToItems(NET_SDK_LOG file)
        {
            string startTime = (file.strLogTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.strLogTime.month + 1)) + "-" + AddZero(file.strLogTime.mday) + " " +
                    AddZero(file.strLogTime.hour) + ":" + AddZero(file.strLogTime.minute) + ":" + AddZero(file.strLogTime.second);
            string logType = "";

            string netUser = Encoding.Default.GetString(file.sNetUser);
            string logDetail = Encoding.Default.GetString(file.sContent);
            if (m_LogTypeList.ContainsKey((int)file.dwMinorType))
                logType = m_LogTypeList[(int)file.dwMinorType];
            string[] tempStrings = { logType, netUser, startTime, file.dwRemoteHostAddr.ToString(), logDetail };
            return tempStrings;
        }
        private string[] ChangeToItems(NET_SDK_EVENT file)
        {
            string startTime = (file.startTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.startTime.month + 1)) + "-" + AddZero(file.startTime.mday) + " " +
                    AddZero(file.startTime.hour) + ":" + AddZero(file.startTime.minute) + ":" + AddZero(file.startTime.second);
            string endTime = (file.endTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.endTime.month + 1)) + "-" + AddZero(file.endTime.mday) + " " +
                    AddZero(file.endTime.hour) + ":" + AddZero(file.endTime.minute) + ":" + AddZero(file.endTime.second);

            string logType = file.type.ToString();

            string netUser = file.chnn.ToString();

            string[] tempStrings = { logType, netUser, startTime, endTime, "" };
            return tempStrings;
        }
        private bool mgDialogShow = false;
        private void btn_msg_callback_Click(object sender, EventArgs e)
        {
            if (m_isOpenMsgCallback)
            {
                foreach (int cbHandle in m_msgCBHandle)
                {
                    bool ret = DevSdkHelper.NET_SDK_CloseAlarmChan(cbHandle);
                    if (ret)
                    {
                        //mg.Close();
                        ((Button)sender).Text = "Start";
                        m_isOpenMsgCallback = false;
                    }
                }               
            }
            else
            {
                myAlarmLog = new Log();
                myAlarmLog.Initial(true, Application.StartupPath, "AlarmLog");//

                //mg = new Msg_Dialog();
                //foreach (NET_SDK_DEVICEINFO_EX devInfoEx in Program.m_deviceDic.Values)
                {
                    int handle = DevSdkHelper.NET_SDK_SetupAlarmChan(userId);
                    if (handle > 0)
                    {
                        ((Button)sender).Text = "Stop";
                        m_msgCBHandle.Add(handle);
                        m_isOpenMsgCallback = true;

                        //if (!mgDialogShow)
                        //    mg.Show();

                        Thread th = new Thread(GetCurrentAlarmStatus);
                        ////Thread th = new Thread(GetCurrentAlarmStatusEx);
                        th.IsBackground = true;
                        th.Start();
                        //ttime.Start();
                    }
                }
                
            }
        }
        private void GetCurrentAlarmStatus()
        {
            while (m_isOpenMsgCallback)
            {
                int listNum = 128;
                DD_ALARM_STATUS_INFO[] alarmStatus = new DD_ALARM_STATUS_INFO[listNum];
                int lpBytesReturned = 0;
                int size = Marshal.SizeOf(typeof(DD_ALARM_STATUS_INFO));
                IntPtr alarmListInfo = PointArrayToIntPtr(alarmStatus);
                IntPtr pTmp = alarmListInfo;
                int bufferSize = size * listNum;
                bool rret = DevSdkHelper.NET_SDK_GetAlarmStatus(userId, alarmListInfo, bufferSize, ref lpBytesReturned);
                if (rret)
                {
                    lpBytesReturned = lpBytesReturned > listNum ? listNum : lpBytesReturned;
                    for (int i = 0; i < lpBytesReturned; i++, pTmp = IntPtr.Add(pTmp, size))
                    {
                        byte[] pByte = new byte[size];
                        Marshal.Copy(pTmp, pByte, 0, size);
                        alarmStatus[i] = (DD_ALARM_STATUS_INFO)BytesToStruct(pByte, typeof(DD_ALARM_STATUS_INFO));
                        sync.Post(UpdateTbAlarmStatus, GetCurrentAlarmStatusDetail(alarmStatus[i]));
                    }

                }

                try
                { Marshal.FreeHGlobal(alarmListInfo); }
                catch
                { }
                //break;
                Thread.Sleep(100);
            }
        }
        //private void GetCurrentAlarmStatusEx()
        //{
        //    while (m_isOpenMsgCallback)
        //    {
        //        int listNum = 4096;
        //        byte[] alarmStatus = new byte[listNum];
        //        int lpBytesReturned = 0;
        //        int exNum = 0;

        //        IntPtr alarmListInfo = Marshal.AllocHGlobal(listNum);
        //        int size = Marshal.SizeOf(typeof(DD_ALARM_STATUS_INFO_Ex));
        //        bool rret = DevSdkHelper.NET_SDK_GetAlarmStatusEx(userId, alarmListInfo, listNum, ref lpBytesReturned, ref exNum);
        //        if (rret)
        //        {
        //            if (exNum > 0)
        //            {
        //                DD_ALARM_STATUS_INFO_Ex[] pALARM_STATUS_INFO_Ex = new DD_ALARM_STATUS_INFO_Ex[exNum];
        //                for (int i = 0; i < exNum; i++)
        //                {
        //                    pALARM_STATUS_INFO_Ex[i] = (DD_ALARM_STATUS_INFO_Ex)Marshal.PtrToStructure(alarmListInfo, typeof(DD_ALARM_STATUS_INFO_Ex));
        //                    alarmListInfo += size;//地址偏移
        //                    sync.Post(UpdateTbAlarmStatus, "alarm type:" + pALARM_STATUS_INFO_Ex[i].alarmType.ToString() + "alarm node:" + pALARM_STATUS_INFO_Ex[i].alarmNode.ToString());
        //                }
        //                int restSize = lpBytesReturned - exNum * size;
        //                int num = restSize / Marshal.SizeOf(typeof(DD_ALARM_STATUS_INFO));
        //                DD_ALARM_STATUS_INFO[] pALARM_STATUS_INFO = new DD_ALARM_STATUS_INFO[num];
        //                for (int i = 0; i < num; i++)
        //                {
        //                    pALARM_STATUS_INFO[i] = (DD_ALARM_STATUS_INFO)Marshal.PtrToStructure(alarmListInfo, typeof(DD_ALARM_STATUS_INFO));
        //                    alarmListInfo += Marshal.SizeOf(typeof(DD_ALARM_STATUS_INFO));//地址偏移
        //                    sync.Post(UpdateTbAlarmStatus, GetCurrentAlarmStatusDetail(pALARM_STATUS_INFO[i]));
        //                }

        //            }

        //        }

        //        try
        //        { Marshal.FreeHGlobal(alarmListInfo); }
        //        catch
        //        { }
        //        break;
        //        Thread.Sleep(100);
        //    }
        //}
        private IntPtr PointArrayToIntPtr(DD_ALARM_STATUS_INFO[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(DD_ALARM_STATUS_INFO));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            tb_AlarmStatus.Clear();
        }
        private void UpdateTbAlarmStatus(object obj)
        {
            if (obj.ToString().Length > 0)
            {
                tb_AlarmStatus.AppendText(obj.ToString());
                tb_AlarmStatus.AppendText("\r\n");
            }
        }
        private string GetCurrentAlarmStatusDetail(DD_ALARM_STATUS_INFO itype)
        {
            string type = "";
            if (itype.alarmType > 72)
            {
                switch (itype.alarmType)
                {
                    case 1:
                        type = " Motion Alarm";
                        break;
                    case 2:
                        type = " Sensor Alarm";
                        break;
                    case 3:
                        type = " Video Loss Alarm";
                        break;
                    case 4:
                        type = " IPC off line Alarm";
                        break;
                    case 5:
                        type = " OSC Alarm";
                        break;
                    case 6:
                        type = " AVD Alarm";
                        break;
                    case 7:
                        type = " PEA_TRIPWIRE Alarm";
                        break;
                    case 8:
                        type = " PEA_PERIMETER Alarm";
                        break;
                    case 9:
                        type = " PEA Alarm";
                        break;
                    case 81:
                        type = " Alarm out";
                        break;
                    case 72:
                        type = "Network Disconnection";
                        break;
                    case 73:
                        type = "NO_DISK";
                        break;
                }
            }
            
            if (type.Length > 0)
                type = "Channel:" + itype.chanl + type;
            return type;
        }

        private void btn_ManualAlarm_Click(object sender, EventArgs e)
        {
            int lManualCount = oNET_SDK_DEVICEINFO.sensorOutputNum;
            int[] pChannel = new int[lManualCount];
            int[] pValue = new int[lManualCount];
            if (!m_bManualAlarm)
            {
                for (int i = 0; i < lManualCount; i++)
                {
                    pChannel[i] = i;
                    pValue[i] = 1;
                }
                bool ret = DevSdkHelper.NET_SDK_SetDeviceManualAlarm(userId, pChannel, pValue, lManualCount, true);
                if (ret)
                {
                    ((Button)sender).Text = "CleanAlarm";
                    m_bManualAlarm = true;
                }
                else
                    ShowErrorMessage();
            }
            else
            {
                for (int i = 0; i < lManualCount; i++)
                {
                    pChannel[i] = i;
                    pValue[i] = 0;
                }
                bool ret = DevSdkHelper.NET_SDK_SetDeviceManualAlarm(userId, pChannel, pValue, lManualCount, false);
                if (ret)
                {
                    ((Button)sender).Text = "ManualAlarm";
                    m_bManualAlarm = false;
                }
                else
                    ShowErrorMessage();
            }

        }
        private void btn_Talk_Trans_Click(object sender, EventArgs e)
        {
            m_waveOut = new MyWaveOut();
            if (m_isStartVoiceComm)
            {
                return;
            }
            if (m_isStartVoiceComm_MR)
            {
                if (m_audioDecoderHandle > 0)
                {
                    m_audioDecoderHandle = -1;
                }
                DevSdkHelper.NET_SDK_StopVoiceCom(m_talkTransHandle);

                //m_WaveIn.Stop();
                //m_waveOut.Stop();
                //m_runTalkTransThread = FALSE;

                m_isStartVoiceComm_MR = false;
                m_talkTransHandle = -1;

                ((Button)sender).Text = "Transport";
                btn_Voice_Comm.Enabled = true;
            }
            else
            {
                tdc = TalkTransCallBack;
                int handle = DevSdkHelper.NET_SDK_StartVoiceCom_MR(userId, true, tdc, IntPtr.Zero);
                if (handle > 0)
                {
                    ((Button)sender).Text = "Close";
                    btn_Voice_Comm.Enabled = false;

                    byte[] pAudioInfo = new byte[20];
                    int nSize = pAudioInfo.Length;
                    IntPtr ptrTemp = Marshal.AllocHGlobal(nSize);
                    Marshal.Copy(pAudioInfo, 0, ptrTemp, 20);

                    bool bRet = DevSdkHelper.NET_SDK_GetAudioInfo(handle, ptrTemp, nSize);
                    //if (bRet)
                    //{
                    //    WaveFormat whdr = (WaveFormat)Marshal.PtrToStructure(ptrTemp, typeof(WaveFormat));
                      
                    //}
                    Marshal.FreeHGlobal(ptrTemp);
                    m_talkTransHandle = handle;
                    m_isStartVoiceComm_MR = true;
                    WaveFormat whdr1 = new WaveFormat(8000, 16, 1);
                    m_waveOut.Start(whdr1);

                    //int ret = m_WaveIn.Start(wfx, &CToolDlg::TalkTransWaveInCallBack, this);
                    //bdeh = DataArrived;
                    //try
                    //{
                    //    m_WaveIn = new WaveInRecorder(-1, whdr1, 102400, 4, bdeh);
                    //}
                    //catch
                    //{
                    //    if (m_WaveIn != null)
                    //        m_WaveIn.Dispose();
                    //}
                    
                }
            }
        }
        private void DataArrived(IntPtr data, int size)
        {
            //if (m_RecBuffer == null || m_RecBuffer.Length < size)
            //    m_RecBuffer = new byte[size];
            //System.Runtime.InteropServices.Marshal.Copy(data, m_RecBuffer, 0, size);
            //m_Fifo.Write(m_RecBuffer, 0, m_RecBuffer.Length);
        }
        private void btn_Voice_Comm_Click(object sender, EventArgs e)
        {
            if (m_isStartVoiceComm_MR)
            {
                return;
            }
            if (!m_isStartVoiceComm)
            {
                int handle = DevSdkHelper.NET_SDK_StartVoiceCom(userId, false, null, IntPtr.Zero);
                if (handle > 0)
                {
                    m_voiceHandle = handle;
                    m_isStartVoiceComm = true;

                    ((Button)sender).Text = "Close";
                    btn_Talk_Trans.Enabled = false;
                    tkbar_VoiceComm_Volum.Enabled = true;

                }
                else
                {
                    ShowErrorMessage();
                }
            }
            else
            {
                DevSdkHelper.NET_SDK_StopVoiceCom(m_voiceHandle);
                m_isStartVoiceComm = false;
                m_voiceHandle = -1;

                ((Button)sender).Text = "Start";
                btn_Talk_Trans.Enabled = true;
                tkbar_VoiceComm_Volum.Enabled = false;
            }
        }
        private void tkbar_VoiceComm_Volum_Scroll(object sender, EventArgs e)
        {
            //int volume = ((TrackBar)sender).Value;
            double tempVolume = ((TrackBar)sender).Value;
            tempVolume = tempVolume / 100 * 0xffff;
            bool ret = DevSdkHelper.NET_SDK_SetVoiceComClientVolume(m_voiceHandle, (int)tempVolume);
            int a = 0;
        }
        private void TalkTransCallBack(int lVoiceComHandle, IntPtr pRecvDataBuffer, int dwBufSize, byte byAudioFlag, IntPtr pUser)
        {
            m_waveOut.WriteBuf(pRecvDataBuffer, dwBufSize);
        }
        private void IVMinit()
        {
            if (Directory.Exists(Application.StartupPath + "\\SourceImg"))//人脸检测原始图片存储路径
                CommonMethods.ClearFolder(Application.StartupPath + "\\SourceImg");
            else
                Directory.CreateDirectory(Application.StartupPath + "\\SourceImg");
            if(Directory.Exists(Application.StartupPath + "\\FaceImg"))//人脸IPC实时抓拍图片存储路径
                CommonMethods.ClearFolder(Application.StartupPath + "\\FaceImg");
            else
                Directory.CreateDirectory(Application.StartupPath + "\\FaceImg");
            if (Directory.Exists(Application.StartupPath + "\\FaceMatchImg"))//人脸IPC实时比对图片存储路径
                CommonMethods.ClearFolder(Application.StartupPath + "\\FaceMatchImg");
            else
                Directory.CreateDirectory(Application.StartupPath + "\\FaceMatchImg");
            if (Directory.Exists(Application.StartupPath + "\\FaceTargetImg"))//人脸IPC实时比对目标图片存储路径
                CommonMethods.ClearFolder(Application.StartupPath + "\\FaceTargetImg");
            else
                Directory.CreateDirectory(Application.StartupPath + "\\FaceTargetImg");
                                           
        }
        
        private void rb_VFD_CONFIG_Click(object sender, EventArgs e)
        {
            
        }
        
        private IntPtr PointArrayToIntPtr(byte[] points)
        {
            int size = points.Length;
            if (size > 0)
            {
                IntPtr buffer = Marshal.AllocHGlobal(size);
                Marshal.Copy(points, 0, buffer, size);
                return buffer;
            }
            else
                return IntPtr.Zero;                  
        }

        
        private List<string> IPCFace = new List<string>();
        private void Subscribe(Int32 lUserID, Int32 dwCommand, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser)
        {
            switch (dwCommand)
            {
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_VEHICE:
                    {
                        //NET_SDK_IVE_VEHICE_HEAD_INFO+[NET_SDK_IVE_VEHICE_ITEM_INFO+源数据]+[NET_SDK_IVE_VEHICE_ITEM_INFO+车牌数据]+...+[NET_SDK_IVE_VEHICE_ITEM_INFO+车牌数据] 
                        NET_SDK_IVE_VEHICE_HEAD_INFO vehiceHeadInfo = new NET_SDK_IVE_VEHICE_HEAD_INFO();
                        int offset = Marshal.SizeOf(vehiceHeadInfo);
                        if(dwBufLen < Marshal.SizeOf(vehiceHeadInfo))
                        {
                            break;
                        }
                        vehiceHeadInfo = (NET_SDK_IVE_VEHICE_HEAD_INFO)Marshal.PtrToStructure(pBuf,typeof(NET_SDK_IVE_VEHICE_HEAD_INFO));
                        pBuf += offset;//指针偏移
                        uint cntVheCle = vehiceHeadInfo.item_cnt;
		
			            if (cntVheCle > 0)
			            {
				            for(int i = 0;i< cntVheCle;i++)
				            {
					            NET_SDK_IVE_VEHICE_ITEM_INFO vehicleinfo = new NET_SDK_IVE_VEHICE_ITEM_INFO();
                                vehicleinfo = (NET_SDK_IVE_VEHICE_ITEM_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_VEHICE_ITEM_INFO));
                                Invoke(new Action(() =>
                                {
                                    StringBuilder strToShow = new StringBuilder();
                                    strToShow.Append("plate number:" + DevSdkHelper.ByteToStr(vehicleinfo.plate) + "\r\n");
                                    strToShow.Append("charactor count:" + vehicleinfo.plateCharCount + "\r\n");                                  
                                    label51.Text = strToShow.ToString();
                                }));
                                
                                offset = Marshal.SizeOf(typeof(NET_SDK_IVE_VEHICE_ITEM_INFO));//偏移量
                                pBuf += offset;//指针偏移					            					            					
					            if (vehicleinfo.jpeg_len > 0)
					            {
						            if(i == 0)//第1张是源图片
						            {
                                        byte[] data = new byte[vehicleinfo.jpeg_len];
                                        Marshal.Copy(pBuf, data, 0, (int)vehicleinfo.jpeg_len);
                                        LoadPicture(data, pb_SourcePic);

                                        //string picName = vehicleinfo.plateId.ToString();
                                        //File.WriteAllBytes(Application.StartupPath + "\\SourceImg\\" + picName.ToString() + ".jpg", data);
                                        pBuf += (int)vehicleinfo.jpeg_len;//地址偏移
						            }
						            else//其他是车牌照片
						            {
                                        byte[] data = new byte[vehicleinfo.jpeg_len];
                                        Marshal.Copy(pBuf, data, 0, (int)vehicleinfo.jpeg_len);
                                        LoadPicture(data, pb_SnapPic);

                                        //string picName = vehicleinfo.plateId.ToString();
                                        //File.WriteAllBytes(Application.StartupPath + "\\FaceImg\\" + picName.ToString() + ".jpg", data);
                                        pBuf += (int)vehicleinfo.jpeg_len;//地址偏移
						            }
		 					
					            }

					            if (vehicleinfo.jpeg_vir_len > vehicleinfo.jpeg_len)
					            {
                                    pBuf += (int)(vehicleinfo.jpeg_vir_len - vehicleinfo.jpeg_len);
					            }											
				            }
			            }
                        break;
                    }
                /*	----------------------
                    |	NET_SDK_IVE_FACE_MATCH_T	|
                    -------------------------
                    |	图片数据(抓拍)	|
                    -------------------------
                    |	NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO		|
                    -------------------------
                    |	NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T		|
                    -------------------------
                    |	图片数据(人脸目标)	|
                    -------------------------
                    |	NET_SDK_TLV_BUFFER_DESC		|
                    -------------------------
                    |	图片数据(背景)	|
                    -------------------------*/
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_FACE_MATCH:
                    {
                        uint restLen = dwBufLen;
                        NET_SDK_IVE_FACE_MATCH_T sIVE_AVD = new NET_SDK_IVE_FACE_MATCH_T();
                        if (dwBufLen <= Marshal.SizeOf(sIVE_AVD))
                        {
                            return;
                        }

                        sIVE_AVD = (NET_SDK_IVE_FACE_MATCH_T)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_FACE_MATCH_T));

                        pBuf += Marshal.SizeOf(sIVE_AVD);
                        restLen -= (uint)Marshal.SizeOf(sIVE_AVD);
                        
                        if (restLen < sIVE_AVD.imgLen)
                        {
                            return;
                        }
                       
                        byte[] imgdata = new byte[sIVE_AVD.imgLen];
                        Marshal.Copy(pBuf, imgdata, 0, (int)sIVE_AVD.imgLen);
                        LoadPicture(imgdata, pb_SnapPic);
                        pBuf += (int)sIVE_AVD.imgLen;
                        restLen -= sIVE_AVD.imgLen;

                        //FileStream fs = File.Create("testface.jpg");
                        //fs.Write(imgdata, 0, (Int32)sIVE_AVD.imgLen);
                        //fs.Close();
                        NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO pFaceInfo = new NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO();
                        if (restLen < Marshal.SizeOf(pFaceInfo))
                            return;
                        pFaceInfo = (NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO));
                        pBuf += Marshal.SizeOf(pFaceInfo);
                        restLen -= (uint)Marshal.SizeOf(pFaceInfo);
                     
                        Invoke(new Action(() =>
                        {

                            StringBuilder strToShow = new StringBuilder();

                            //DateTime st = DevSdkHelper.GetDateTime(baseInfo.i64SnapTime);
                            strToShow.Append("time:" + sIVE_AVD.frameTime.year + "_" + 
                                (sIVE_AVD.frameTime.month + 1) + "_" + sIVE_AVD.frameTime.mday + " " +
                                sIVE_AVD.frameTime.hour + ":" + sIVE_AVD.frameTime.minute + ":" +sIVE_AVD.frameTime.second + "\r\n");
                            strToShow.Append("similarity:" + sIVE_AVD.dwSimilar + "\r\n");
                            strToShow.Append("name:" + DevSdkHelper.ByteToStr(pFaceInfo.szName) + "\r\n");
                            strToShow.Append("birthday:" + pFaceInfo.dwBirth + "\r\n");
                            strToShow.Append("native place:" + DevSdkHelper.ByteToStr(pFaceInfo.szNativePlace) + "\r\n");
                            strToShow.Append("identifyNum:" + DevSdkHelper.ByteToStr(pFaceInfo.szCredential) + "\r\n");
                            strToShow.Append("tel:" + DevSdkHelper.ByteToStr(pFaceInfo.byPhoneNum) + "\r\n");
                            strToShow.Append("ID:" + DevSdkHelper.ByteToStr(pFaceInfo.byIDParam) + "\r\n");


                            label51.Text = strToShow.ToString();
                        }));

                        NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T pFaceDataInfo = new NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T();
                        if (restLen < Marshal.SizeOf(pFaceDataInfo))
                            return;
                        pFaceDataInfo = (NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T)Marshal.PtrToStructure(pBuf, typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));
                        pBuf += Marshal.SizeOf(pFaceDataInfo);
                        restLen -= (uint)Marshal.SizeOf(pFaceDataInfo);

                        if (restLen < pFaceDataInfo.stFaceImgData.dataLen)
                            return;

                        byte[] targetImg = new byte[pFaceDataInfo.stFaceImgData.dataLen];
                        Marshal.Copy(pBuf, targetImg, 0, (int)pFaceDataInfo.stFaceImgData.dataLen);
                        LoadPicture(targetImg, pb_TargetPic);
                        pBuf += (int)pFaceDataInfo.stFaceImgData.dataLen;
                        restLen -= pFaceDataInfo.stFaceImgData.dataLen;

                        NET_SDK_TLV_BUFFER_DESC pBufferDesc = new NET_SDK_TLV_BUFFER_DESC();
                        if (restLen < Marshal.SizeOf(pBufferDesc))
                            return;

                        pBufferDesc = (NET_SDK_TLV_BUFFER_DESC)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_TLV_BUFFER_DESC));
                        pBuf += Marshal.SizeOf(pBufferDesc);
                        restLen -= (uint)Marshal.SizeOf(pBufferDesc);

                        if (restLen < pBufferDesc.dwSize)
                            return;
                        byte[] sourceImg = new byte[pBufferDesc.dwSize];
                        Marshal.Copy(pBuf, sourceImg, 0, (int)pBufferDesc.dwSize);
                        LoadPicture(sourceImg, pb_SourcePic);
                    }
                    break;
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
                            byte[] data = new byte[src.dataLen];
                            Marshal.Copy(pBuf, data, 0, (int)src.dataLen);
                            LoadPicture(data, pb_SourcePic);
                            //string picName = time.Year.ToString() + time.Month.ToString() + time.Day.ToString() + time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString();
                            //File.WriteAllBytes(Application.StartupPath + "\\SourceImg\\" + picName.ToString() + ".jpg", data);

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

                            float temperature = 0;
                            if (face.temperature > 0)
                            {
                                temperature = (float)face.temperature / 100;
                            }
                            Invoke(new Action(() =>
                            {                               
                                StringBuilder strToShow = new StringBuilder();
                                strToShow.Append("temperature:" + temperature + "\r\n");
                                strToShow.Append("wearmask:" + face.wearmask + "\r\n");
                                strToShow.Append("comprehensive_score:" + face.comprehensive_score + "\r\n");
                                strToShow.Append("faceliveness:" + face.faceliveness + "\r\n");
                                label51.Text = strToShow.ToString();
                            }));


                            pBuf += Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));//地址偏移
                            restLen -= Marshal.SizeOf(typeof(NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T));//剩余长度
                            if (face.stFaceImgData.dataLen > 0)//face Image, 如果有
                            {
                                //TimeAndId tai = new TimeAndId();
                                //tai.time = time.ToString();
                                //tai.id = face.faceId;
                                //sync.Post(IPCFaceAddItem, tai);

                                byte[] data = new byte[face.stFaceImgData.dataLen];
                                Marshal.Copy(pBuf, data, 0, (int)face.stFaceImgData.dataLen);
                                LoadPicture(data, pb_SnapPic);
                                //File.WriteAllBytes(Application.StartupPath + "\\FaceImg\\" + face.faceId.ToString() + ".jpg", data);

                                pBuf += (int)face.stFaceImgData.dataLen;//地址偏移
                                restLen -= (int)face.stFaceImgData.dataLen;//剩余长度
                            }
                            //if (face.featureSize > 0)//feature, 如果有
                            //{
                            //    pBuf += (int)face.featureSize;//地址偏移
                            //}
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

                        string szName = DevSdkHelper.ByteToStr(baseInfo.szName);
                        float temperature = 0;
                        if (baseInfo.temperature > 0)
                        {
                            temperature = (float)baseInfo.temperature / 100;
                        }
                        Invoke(new Action(() => {

                            StringBuilder strToShow = new StringBuilder();

                            DateTime st = DevSdkHelper.GetDateTime(baseInfo.i64SnapTime);
                            strToShow.Append("time(UTC):" + st.ToLongDateString() + st.ToLongTimeString() + "\r\n");
                            strToShow.Append("similarity:" + baseInfo.iSimilarity + "\r\n");
                            strToShow.Append("name:" + DevSdkHelper.ByteToStr(baseInfo.szName) + "\r\n");
                            string strTemp = baseInfo.iMale == 1 ? "male" : "female";
                            strToShow.Append("sex:" + strTemp + "\r\n");
                            strToShow.Append("age:" + baseInfo.iAge + "\r\n");
                            strToShow.Append("identifyNum:" + DevSdkHelper.ByteToStr(baseInfo.szIdentifyNum) + "\r\n");
                            strToShow.Append("tel:" + DevSdkHelper.ByteToStr(baseInfo.szTel) + "\r\n");

                            strToShow.Append("temperature:" + temperature + "\r\n");
                            strToShow.Append("comparisonRes:" + baseInfo.comparisonRes + "\r\n");

                            label51.Text = strToShow.ToString();
                        }));


                        pBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_BASE_INFO));//地址偏移
                        restLen -= Marshal.SizeOf(typeof(NET_SDK_IVE_BASE_INFO));

                        NET_SDK_IVE_PICTURE_INFO pictureInfo = new NET_SDK_IVE_PICTURE_INFO();
                        if (restLen < Marshal.SizeOf(pictureInfo))
                            return;
                        pictureInfo = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(pictureInfo);//地址偏移
                        restLen -= Marshal.SizeOf(pictureInfo);
                        if (restLen < pictureInfo.iPicSize)
                            return;

                        //TimeAndIdAndSim taiai = new TimeAndIdAndSim();
                        //var time = DateTime.Parse((DateTime.Parse("1970-01-01 08:00:00").AddMilliseconds(baseInfo.i64SnapTime / 1000)).ToString());
                        //taiai.time = time.ToString();
                        //taiai.id = (int)baseInfo.iSnapPicId;
                        //taiai.sim = baseInfo.iSimilarity;
                        //sync.Post(IPCFaceMatchAddItem, taiai);

                        byte[] data = new byte[pictureInfo.iPicSize];
                        Marshal.Copy(pBuf, data, 0, pictureInfo.iPicSize);
                        LoadPicture(data, pb_SnapPic);

                        //File.WriteAllBytes(Application.StartupPath + "\\FaceMatchImg\\" + baseInfo.iSnapPicId.ToString() + ".jpg" , data);
                        pBuf += pictureInfo.iPicSize;//地址偏移
                        restLen -= pictureInfo.iPicSize;

                        NET_SDK_IVE_PICTURE_INFO pictureInfo2 = new NET_SDK_IVE_PICTURE_INFO();
                        if (restLen < Marshal.SizeOf(pictureInfo2))
                            return;
                        pictureInfo2 = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(pictureInfo2);//地址偏移
                        restLen -= Marshal.SizeOf(pictureInfo2);

                        if (restLen < pictureInfo2.iPicSize)
                            return;

                        data = new byte[pictureInfo2.iPicSize];
                        Marshal.Copy(pBuf, data, 0, pictureInfo2.iPicSize);
                        LoadPicture(data, pb_TargetPic);
                        //File.WriteAllBytes(Application.StartupPath + "\\FaceTargetImg\\" + baseInfo.iSnapPicId.ToString() + ".jpg", data);
                        pBuf += pictureInfo2.iPicSize;//地址偏移
                        restLen -= pictureInfo2.iPicSize;

                        NET_SDK_IVE_PICTURE_INFO pictureInfo3 = new NET_SDK_IVE_PICTURE_INFO();
                        if (restLen < Marshal.SizeOf(pictureInfo3))
                            return;
                        pictureInfo3 = (NET_SDK_IVE_PICTURE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_PICTURE_INFO));
                        pBuf += Marshal.SizeOf(pictureInfo3);//地址偏移
                        restLen -= Marshal.SizeOf(pictureInfo3);
                        if (restLen < pictureInfo3.iPicSize)
                            return;
                        data = new byte[pictureInfo3.iPicSize];
                        Marshal.Copy(pBuf, data, 0, pictureInfo3.iPicSize);
                        LoadPicture(data, pb_SourcePic);
                    }
                    break;
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_PASSLINE:
                    {
                        if (dwBufLen <= Marshal.SizeOf(typeof(NET_SDK_IVE_PASSLINECOUNT_T)))
                        {                          
                            return;
                        }

                        NET_SDK_IVE_PASSLINECOUNT_T sIVE_PassLine = (NET_SDK_IVE_PASSLINECOUNT_T)Marshal.PtrToStructure(pBuf,typeof(NET_SDK_IVE_PASSLINECOUNT_T));

                        Invoke(new Action(() =>
                        {
                            StringBuilder strToShow = new StringBuilder();
                            strToShow.Append("enter cars:" + sIVE_PassLine.enterCarCount + "\r\n");
                            strToShow.Append("enter persons:" + sIVE_PassLine.enterPersonCount + "\r\n");
                            strToShow.Append("enter bikes:" + sIVE_PassLine.enterBikeCount + "\r\n");
                            strToShow.Append("leave cars:" + sIVE_PassLine.leaveCarCount + "\r\n");
                            strToShow.Append("leave persons:" + sIVE_PassLine.leavePersonCount + "\r\n");
                            strToShow.Append("leave bikes:" + sIVE_PassLine.leaveBikeCount + "\r\n");
                            label51.Text = strToShow.ToString();
                        }));
                    }
                    break;
            }

        }
        private void IPCFaceAddItem(object obj)
        {
            //lv_FaceIPC.Items.Add(new ListViewItem(new string[]{((TimeAndId)obj).time, ((TimeAndId)obj).id.ToString()}));
        }
        private void IPCFaceMatchAddItem(object obj)
        {
            //lv_FaceMatchIPC.Items.Add(new ListViewItem(new string[] { ((TimeAndIdAndSim)obj).time, ((TimeAndIdAndSim)obj).id.ToString(), ((TimeAndIdAndSim)obj).sim.ToString() }));
        }
        public struct TimeAndId
        {
            public string time;
            public int id;
        }
        public struct TimeAndIdAndSim
        {
            public string time;
            public int id;
            public int sim;
        }

        
        private void btn_smart_subscrib_PassLine_Click(object sender, EventArgs e)
        {
            bool ret = false;
            int chn = 0;
            if (!m_SubscribPassLine)
            {
                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib = new NET_DVR_SUBSCRIBE_REPLY();
                //int nSizeOfReply = Marshal.SizeOf(sSmartSubscrib);
                //IntPtr ptrOfReply = Marshal.AllocHGlobal(nSizeOfReply);
                //Marshal.StructureToPtr(sSmartSubscrib, ptrOfReply, true);

                ret = DevSdkHelper.NET_SDK_SmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_IPC_SMART_PASSLINE, chn, ref sSmartSubscrib);
                if (ret)
                {
                    //sSmartSubscrib = (NET_DVR_SUBSCRIBE_REPLY)Marshal.PtrToStructure(ptrOfReply, typeof(NET_DVR_SUBSCRIBE_REPLY));
                    m_serverAddressVFD = sSmartSubscrib.serverAddress;
                    m_SubscribPassLine = true;
                    button8.Text = "UnSubscribPassLine";
                    button8.FlatStyle = FlatStyle.Popup;
                }
                else
                    ShowErrorMessage();

                //Marshal.FreeHGlobal(ptrOfReply);
            }
            else
            {
                int dwResult = 0;
                IntPtr temp = PointArrayToIntPtr(m_serverAddressAVD);
                bool bret = DevSdkHelper.NET_SDK_UnSmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_IPC_SMART_PASSLINE, chn, temp, ref dwResult);
                Marshal.FreeHGlobal(temp);
                if (bret)
                {
                    m_SubscribPassLine = false;
                    button8.Text = "SubscribPassLine";
                    button8.FlatStyle = FlatStyle.Standard;
                }
                else
                    ShowErrorMessage();


            }
        }
    }
}
