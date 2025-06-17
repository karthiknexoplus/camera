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
        private string usr = "";
        private string psw = "";
        private float temperatureHmin;
        private float temperatureHmax;
        private float temperatureLmin;
        private float temperatureLmax;

        //private float temperatureHuashiH;
        //private float temperatureHuashiL;
        //private float temperatureSheshiH;
        //private float temperatureSheshiL;

        private NET_SDK_DEV_SUPPORT oDevSupport;
        private int userId = 0;
        private NET_SDK_DEVICEINFO oNET_SDK_DEVICEINFO;
        private Color panelBorderColor = Color.Transparent;
        private IntPtr currentPanelHandle;
        private IntPtr lastPanelHandle;
        private IntPtr[] _16PanelHandles;
        private int _16Index = 0;
        private int[] _16PlayHandles;
        private Dictionary<IntPtr, ItemStatus> theStatus = new Dictionary<IntPtr, ItemStatus>();
        private const string STR_LIVE_FAILED = "failed to play live";
        private int defaultBright;
        private int defaultContrast;
        private int defaultHue;
        private int defaultSaturation;
        private ItemStatus initItemStatus;
        private bool soundChannelOccupied = false;
        private ToolStripMenuItem openAudio;
        private ToolStripMenuItem closeAudio;
        private ToolStripMenuItem disableAudio;
        private ToolStripMenuItem snap;
        private ContextMenuStrip audioOpened;
        private ContextMenuStrip audioClosed;
        private ContextMenuStrip audioDisabled;
        private Dictionary<int, string> theTreeNodeName = new Dictionary<int, string>();

        private bool osdTime = false;
        private bool osdChlName = false;
        private bool osdTitle = false;
        private bool[] manualRecStatus;

        private static LIVE_DATA_CALLBACK ldc;
        private static LIVE_DATA_CALLBACK myldc = null;
        private static YUV_DATA_CALLBACK yuvldc = null;
        private static DRAW_FUN_CALLBACK dfc;
        private SynchronizationContext sync;

        private bool m_bPTZ3DCtrling = false;
        private bool m_bEnablePTZ3DCtrl;
        private Point start;//画框的起始点
        private Point end;//画框的结束点;
        private bool blnDraw = false;//判断是否绘制
        private Rectangle rect;

        private bool m_isCruiseStart = false;//
        private bool m_isCruiseRunning;

        private DD_CRUISE_POINT_INFO[] cruiseInfoArray;

        public static string passStr;

        //private int[] PBAudioDecHandles;
        private IntPtr PBAudioOutBuffer;
        private int PBAudioDecHandle;

        private MyWaveOut m_waveOutLive;

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessage(int hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        //定义消息常数 
        public const int CUSTOM_MESSAGE = 0X400 + 2;//自定义消息
        public const string STR_MOTION = "Motion";
        public const string STR_SENSOR = "Sensor";
        public const string STR_VIDEO_LOSS = "Video Loss";
        public const string STR_SHELTER = "Video Shelter";
        public const string STR_DISK_FULL = "Disk Full";
        public const string STR_DISK_UNFORMAT = "Disk Unformat";
        public const string STR_DISK_READ_WRITE_ERROR = "Disk Error";
        private bool LogTypeInited = false;
        public void SendMsgToForm(int MSG)
        {
            int WINDOW_HANDLER = FindWindow(null, "Message");
            if (WINDOW_HANDLER != 0)
            {
                long result = SendMessage(WINDOW_HANDLER, MSG, new IntPtr(14), IntPtr.Zero).ToInt64();
            }
            
        }
        private uint dwChannel_temp = 0;
        private string tempType_temp = "";
        private string tempDetail_temp = "";
        
        private void fNET_MESSAGE_CALLBACK(Int32 lCommand, Int32 lUserID, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser)
        {
            string tempDevIp = "";
            //foreach (string ipStr in Program.m_deviceDic.Keys)
            //{
            //    if (Program.m_deviceDic[ipStr].userId == lUserID)
            //    {
            //        tempDevIp = ipStr;
            //    }
            //}
            switch (lCommand)
            {
                #region alarm

                case (int)NET_SDK_DEVICE_MSG_TYPE.NET_SDK_ALARM:
                    if (dvrDisconnected)
                        return;
                    int size = Marshal.SizeOf(typeof(NET_SDK_ALARMINFO_EX));
                    long num = dwBufLen / size;
                    NET_SDK_ALARMINFO_EX[] alarmInfo = new NET_SDK_ALARMINFO_EX[num];
                    IntPtr pTmp = pBuf;
                    string tempType = "";
                    string tempDetail = "";
                    string alarmTime = "";
                    for (int i = 0; i < num; i++, pTmp += size)
                    {
                        alarmInfo[i] = (NET_SDK_ALARMINFO_EX)Marshal.PtrToStructure(pTmp, typeof(NET_SDK_ALARMINFO_EX));
                        alarmTime = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(alarmInfo[i].alarmTime));
                        
                        switch (alarmInfo[i].dwAlarmType)
                        {
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_MOTION:
                                tempType = STR_MOTION;
                                tempDetail = "Channel:" + alarmInfo[i].dwChannel;                               
                                break;
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_SENSOR:
                                tempType = STR_SENSOR;
                                string sensorName = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(alarmInfo[i].sensorName));
                                if (alarmInfo[i].dwChannel <= 64)
                                    tempDetail = "Sensor:" + sensorName + " pop up channel:" + alarmInfo[i].dwChannel;
                                else
                                    tempDetail = "Sensor:" + sensorName + " no pop up channel:";                               
                                break;
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_VLOSS:
                                tempType = STR_VIDEO_LOSS;
                                tempDetail = "Channel:" + alarmInfo[i].dwChannel;                              
                                break;
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_SHELTER:
                                tempType = STR_SHELTER;
                                tempDetail = "Channel:" + alarmInfo[i].dwChannel;
                                
                                break;
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_DISK_FULL:
                                tempType = STR_DISK_FULL;
                                tempDetail = "Disk:" + alarmInfo[i].dwDisk;
                                
                                break;
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_DISK_UNFORMATTED:
                                tempType = STR_DISK_UNFORMAT;
                                tempDetail = "Disk:" + alarmInfo[i].dwDisk;
                                
                                break;
                            case (int)NET_SDK_ALARM_TYPE.NET_SDK_ALARM_TYPE_DISK_WRITE_FAIL:
                                tempType = STR_DISK_READ_WRITE_ERROR;
                                tempDetail = "Disk:" + alarmInfo[i].dwDisk;
                                
                                break;
                            case (int)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_NO_DISK:
                                tempType = "No disk";
                                
                                break;
                            case (int)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_HDD_PULL_OUT:
                                tempType = "Disk pull out";
                                tempDetail = "Disk:" + alarmInfo[i].dwDisk;
                                
                                break;
                        }
                        if(alarmTime!= null)
                            myAlarmLog.WriteLog("Device:" + tempDevIp + " channel:" + alarmInfo[i].dwChannel
                            + " type:" + tempType + " detail:" + tempDetail + " alarm time:" + alarmTime, 0);
                        else
                            myAlarmLog.WriteLog("Device:" + tempDevIp + " channel:" + alarmInfo[i].dwChannel
                            + " type:" + tempType + " detail:" + tempDetail, 0);
                        //if (mg != null)
                        //{
                        //    //if (alarmInfo[i].dwChannel != dwChannel_temp && tempType != tempType_temp && tempDetail != tempDetail_temp)
                        //    //{   
                            
                        //    //sync.Post(mg.ShowMsg, "Device:" + tempDevIp + " channel:" + alarmInfo[i].dwChannel + " type:" + tempType + " detail:" + tempDetail + " time:" + DateTime.Now);
                            
                        //        lock (this)
                        //        { dwChannel_temp = alarmInfo[i].dwChannel; tempType_temp = tempType; tempDetail_temp = tempDetail; }
                        //    //}                              
                        //}
                            
                    }                    

                    break;
#endregion
                #region record status

                case (int)NET_SDK_DEVICE_MSG_TYPE.NET_SDK_RECORD:

                    size = Marshal.SizeOf(typeof(NET_SDK_RECORD_STATUS_EX));
                    num = dwBufLen / size;
                    NET_SDK_RECORD_STATUS_EX[] recInfo = new NET_SDK_RECORD_STATUS_EX[num];
                    pTmp = pBuf;

                    for (int i = 0; i < num; i++, pTmp += size)
                    {
                        recInfo[i] = (NET_SDK_RECORD_STATUS_EX)Marshal.PtrToStructure(pTmp, typeof(NET_SDK_RECORD_STATUS_EX));
                        tempType = "";
                        tempDetail = "";
                        if ((recInfo[i].dwRecordType & (int)DD_RECORD_TYPE.DD_RECORD_TYPE_MANUAL) != 0)
                        {
                            if (tempType.Length == 0)
                                tempType = "Manual Record";
                            else
                                tempType += "|" + "Manual Record";
                        }

                        if ((recInfo[i].dwRecordType & (int)DD_RECORD_TYPE.DD_RECORD_TYPE_SCHEDULE) != 0)
                        {
                            if (tempType.Length == 0)
                                tempType = "Schedule Record";
                            else
                                tempType += "|" + "Schedule Record";
                        }

                        if ((recInfo[i].dwRecordType & (int)DD_RECORD_TYPE.DD_RECORD_TYPE_MOTION) != 0)
                        {
                            if (tempType.Length == 0)
                                tempType = "Motion Record";
                            else
                                tempType += "|" + "Motion Record";
                        }
                        if ((recInfo[i].dwRecordType & (int)DD_RECORD_TYPE.DD_RECORD_TYPE_SENSOR) != 0)
                        {
                            if (tempType.Length == 0)
                                tempType = "Sensor Record";
                            else
                                tempType += "|" + "Sensor Record";
                        }
                        if ((recInfo[i].dwRecordType & (int)DD_RECORD_TYPE.DD_RECORD_TYPE_BEHAVIOR) != 0)
                        {
                            if (tempType.Length == 0)
                                tempType = "Behavior Record";
                            else
                                tempType += "|" + "Behavior Record";
                        }
                        if (recInfo[i].dwRecordType == (int)DD_RECORD_TYPE.DD_RECORD_TYPE_NONE)
                        {
                            tempType = "None Record";
                        }
                        string tStatus = "";
                        if (recInfo[i].dwRecordStatus == DevSdkHelper.DD_RECORD_STATUS_OFF)
                            tStatus = "record stoped";
                        else if (recInfo[i].dwRecordStatus == DevSdkHelper.DD_RECORD_STATUS_ON)
                            tStatus = "recording";
                        else if (recInfo[i].dwRecordStatus == DevSdkHelper.DD_RECORD_STATUS_ABNORMAL)
                            tStatus = "record abnormal";

                        myAlarmLog.WriteLog("Device:" + tempDevIp + " channel:" + recInfo[i].dwChannel + " type:" + tempType + " status:" + tStatus, 0);
                        //if (mg != null)
                        //    sync.Post(mg.ShowMsg, "Device:" + tempDevIp + "channel:" + recInfo[i].dwChannel + " type:" + tempType);
                    }
                    
                    break;
#endregion
                #region scene change alarm

                case (int)NET_SDK_DEVICE_MSG_TYPE.NET_SDK_IVM_RULE:

                    int curLen = 0; ;
                    while (curLen < dwBufLen)
                    {                        
                        string ttemp = "";
                        string ttype = "";
                        IntPtr tmpBuf = pBuf + curLen;

                        NET_SDK_IVM_RULE_STATUS pIVMRuleStatus = (NET_SDK_IVM_RULE_STATUS)Marshal.PtrToStructure(tmpBuf, typeof(NET_SDK_IVM_RULE_STATUS));
                        curLen += Marshal.SizeOf(pIVMRuleStatus);
                        switch (pIVMRuleStatus.dwIVMType)
                        {
                            case DevSdkHelper.NET_SDK_IVM_RULE_TYPE_VFD:
                            case DevSdkHelper.NET_SDK_IVM_RULE_TYPE_AVD_SCENE:
                            case DevSdkHelper.NET_SDK_IVM_RULE_TYPE_AVD_CLARITY:
                            case DevSdkHelper.NET_SDK_IVM_RULE_TYPE_AVD_COLOR:
                                {
                                    if (pIVMRuleStatus.dwIVMSize != Marshal.SizeOf(pIVMRuleStatus))//lib与头文件不匹配才会如此
                                    {                                       
                                        break;
                                    }
                                    if ((curLen + pIVMRuleStatus.dwIVMSize) > dwBufLen)
                                    {                                       
                                        break;
                                    }
                                    tmpBuf += curLen;
                                    NET_SDK_IVM_STATUS_INFO pVFDStatus = (NET_SDK_IVM_STATUS_INFO)Marshal.PtrToStructure(tmpBuf, typeof(NET_SDK_IVM_STATUS_INFO));
                                    if (pIVMRuleStatus.dwIVMType == DevSdkHelper.NET_SDK_IVM_RULE_TYPE_VFD)
                                        ttype = "face recognize alarm";
                                    else if (pIVMRuleStatus.dwIVMType == DevSdkHelper.NET_SDK_IVM_RULE_TYPE_AVD_SCENE)
                                        ttype = "scene change alarm";
                                    else if (pIVMRuleStatus.dwIVMType == DevSdkHelper.NET_SDK_IVM_RULE_TYPE_AVD_CLARITY)
                                        ttype = "scene's clarity too low";
                                    else if (pIVMRuleStatus.dwIVMType == DevSdkHelper.NET_SDK_IVM_RULE_TYPE_AVD_COLOR)
                                        ttype = "scene's color is abnormal";
                                    ttemp = string.Format(" channel:{0}", pVFDStatus.dwChannel);
                                    curLen += Marshal.SizeOf(pVFDStatus);
                                }
                                break;
                            default:
                                ttemp = string.Format(" NET_SDK_IVM_RULE dwIVMType= {0}, dwIVMSize={1}", pIVMRuleStatus.dwIVMType, pIVMRuleStatus.dwIVMSize);
                                
                                break;
                        }

                        myAlarmLog.WriteLog("Device:" + tempDevIp + ttype + ttemp, 0);

                    }
                    break;
#endregion

            }
            

        }
        private void ManualRecStatusInit(NET_SDK_DEVICEINFO devInfo)
        {
            int channelNum = devInfo.videoInputNum;
            manualRecStatus = new bool[channelNum];
            for (int i = 0; i < channelNum; i++)
                manualRecStatus[i] = false;
        }
        private struct ItemStatus
        {
            public int playHandle { get; set; }
            public bool playStatus { get; set; }
            public int channelIndex { get; set; }
            public int streamType { get; set; }
            public string btn_Play_txt { get; set; }
            public int brightValue { get; set; }
            public int contrastValue { get; set; }
            public int saturationValue { get; set; }
            public int hueValue { get; set; }
            public bool saveLive { get; set; }
            public string btn_LiveSave_txt { get; set; }
            public bool defaultColor { get; set; }

            public ContextMenuStrip cms { get; set; }
            public bool volumeControl { get; set; }

            public bool trackRecBtnEnable { get; set; }
            public bool trackRunBtnEnable { get; set; }
            public string trackRecBtnTxt { get; set; }
            public string trackRunBtnTxt { get; set; }


        }
        private void ItemDataInit()
        {
            initItemStatus = new ItemStatus();
            initItemStatus.playHandle = -1;
            initItemStatus.playStatus = false;
            initItemStatus.channelIndex = 0;
            initItemStatus.streamType = 0;
            initItemStatus.btn_Play_txt = "Start";
            initItemStatus.brightValue = 0;
            initItemStatus.contrastValue = 0;
            initItemStatus.saturationValue = 0;
            initItemStatus.hueValue = 0;
            initItemStatus.saveLive = false;
            initItemStatus.btn_LiveSave_txt = "Save";
            initItemStatus.defaultColor = false;

            initItemStatus.cms = null;
            initItemStatus.volumeControl = false;

            initItemStatus.trackRecBtnEnable = false;
            initItemStatus.trackRunBtnEnable = false;
            initItemStatus.trackRecBtnTxt = "Record";
            initItemStatus.trackRunBtnTxt = "Start";

        }
        private void UpdateItemData()
        {
            Invoke(new Action(() => {
                if (currentPanelHandle == null)
                    currentPanelHandle = panel1.Handle;
                ItemStatus tempStatus = theStatus[currentPanelHandle];


                tempStatus.btn_Play_txt = btn_Live.Text;
                tempStatus.brightValue = tkbar_Brightness.Value;
                tempStatus.contrastValue = tkbar_Contrast.Value;
                tempStatus.saturationValue = tkbar_Saturation.Value;
                tempStatus.hueValue = tkbar_Hue.Value;
                tempStatus.saveLive = btn_LiveSave.Enabled;
                tempStatus.btn_LiveSave_txt = btn_LiveSave.Text;
                tempStatus.defaultColor = btn_Color_Default.Enabled;

                tempStatus.channelIndex = cmb_Channel.SelectedIndex;
                tempStatus.streamType = cmb_StreamType.SelectedIndex;

                tempStatus.cms = Control.FromHandle(currentPanelHandle).ContextMenuStrip;
                tempStatus.volumeControl = tkbar_Volume.Enabled;

                tempStatus.trackRecBtnEnable = btn_Track_Record.Enabled;
                tempStatus.trackRunBtnEnable = btn_Track.Enabled;
                tempStatus.trackRecBtnTxt = btn_Track_Record.Text;
                tempStatus.trackRunBtnTxt = btn_Track.Text;

                theStatus[currentPanelHandle] = tempStatus;
            
            }));



        }
        private void UpdateUI(IntPtr handle)
        {
            Invoke(new Action(() =>
            {
                btn_Live.Text = theStatus[handle].btn_Play_txt;

                tkbar_Brightness.Value = theStatus[handle].brightValue;
                tkbar_Contrast.Value = theStatus[handle].contrastValue;
                tkbar_Saturation.Value = theStatus[handle].saturationValue;
                tkbar_Hue.Value = theStatus[handle].hueValue;

                btn_LiveSave.Enabled = theStatus[currentPanelHandle].saveLive;
                btn_LiveSave.Text = theStatus[currentPanelHandle].btn_LiveSave_txt;
                btn_Color_Default.Enabled = theStatus[currentPanelHandle].defaultColor;

                cmb_Channel.SelectedIndex = theStatus[handle].channelIndex;
                cmb_StreamType.SelectedIndex = theStatus[handle].streamType;

                btn_Track_Record.Enabled = theStatus[handle].trackRecBtnEnable;
                btn_Track.Enabled = theStatus[handle].trackRunBtnEnable;
                btn_Track_Record.Text = theStatus[handle].trackRecBtnTxt;
                btn_Track.Text = theStatus[handle].trackRunBtnTxt;

                Control.FromHandle(currentPanelHandle).ContextMenuStrip = theStatus[handle].cms;
                tkbar_Volume.Enabled = theStatus[handle].volumeControl;
            }));           
        }
        private void PtzUIInit()
        {
            tkbar_Speed.Maximum = 8;
            tkbar_Speed.Minimum = 1;
            tkbar_Speed.Value = 4;

            foreach (Control ct in groupBox1.Controls)
            {
                if (ct.GetType() == typeof(Button))
                {
                    if (ct.Tag != null)
                    {
                        int tempIndex = Convert.ToInt16(ct.Tag);
                        if (tempIndex < 15)
                        {
                            ct.MouseDown += PtzControl_MouseDown;
                            ct.MouseUp += PtzControl_MouseUp;
                        }
                        else if (tempIndex >= 15 && tempIndex <= 17)
                        {
                            ct.Click += Preset_Click;
                        }
                        else if (tempIndex >= 22 && tempIndex <= 24)
                        {
                            ct.Click += CruiseRun_Click;
                        }
                        
                    }
                }
            }
        }
    }
}
