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
using System.Xml;
namespace Demo
{
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    public partial class Live_Dialog : Form
    {
        private int temphandle;
        private bool isPlay;
        [DllImport("gdi32.dll")]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        public Live_Dialog()
        {
            dfc = null;
            sync = SynchronizationContext.Current;           
            InitializeComponent();           
        }
        public void SetDeviceInfo(NET_SDK_DEVICEINFO_EX devInfoEx)
        {
            
            CloseAllLive();
            userId = devInfoEx.userId;
            oNET_SDK_DEVICEINFO = devInfoEx.devInfo;
            ItemDataInit();

            //RefreshDeviceList();            
            RefreshDeviceChls();
           
            foreach (Control ct in tabPage1.Controls)
            {
                if (ct.GetType() == typeof(Panel))
                {
                    ct.Paint += panel_Paint;
                    ct.MouseClick += panel_Click;
                    if (!theStatus.ContainsKey(ct.Handle))
                        theStatus.Add(ct.Handle, initItemStatus);
                    else
                        theStatus[ct.Handle] = initItemStatus;
                }
            }
            ContextMenuInit();
            ManualRecStatusInit(oNET_SDK_DEVICEINFO);
            cmb_Channel.SelectedItem = cmb_Channel.Items[0];
            

            uint streamNum = DevSdkHelper.NET_SDK_SupportStreamNum(userId, 0);
            for (uint i = 2; i < streamNum; i++)
            {
                cmb_StreamType.Items.Add("Stream" + (i + 1));
            }
            cmb_StreamType.SelectedItem = cmb_StreamType.Items[0];

            cmb_Channel.SelectedIndexChanged += cmb_Channel_SelectedIndexChanged;
            cmb_StreamType.SelectedIndexChanged += cmb_StreamType_SelectedIndexChanged;
#region 16windows
            cmb_Channel_16windows.SelectedItem = cmb_Channel_16windows.Items[0];
            cmb_StreamType_16windows.SelectedItem = cmb_StreamType.Items[0];
            _16PanelHandles = new IntPtr[16];
            _16PanelHandles[0] = panel5.Handle;
            _16PanelHandles[1] = panel6.Handle;
            _16PanelHandles[2] = panel7.Handle;
            _16PanelHandles[3] = panel8.Handle;
            _16PanelHandles[4] = panel9.Handle;
            _16PanelHandles[5] = panel10.Handle;
            _16PanelHandles[6] = panel11.Handle;
            _16PanelHandles[7] = panel12.Handle;
            _16PanelHandles[8] = panel13.Handle;
            _16PanelHandles[9] = panel14.Handle;
            _16PanelHandles[10] = panel15.Handle;
            _16PanelHandles[11] = panel16.Handle;
            _16PanelHandles[12] = panel17.Handle;
            _16PanelHandles[13] = panel18.Handle;
            _16PanelHandles[14] = panel19.Handle;
            _16PanelHandles[15] = panel20.Handle;

            _16PlayHandles = new int[16];
            for (int i = 0; i < 16; i++)
            {
                _16PlayHandles[i] = -1;
            }
#endregion

                currentPanelHandle = panel1.Handle;
            lastPanelHandle = panel1.Handle;

            ConfigMenuInit();
            
            PtzUIInit();
            if (!LogTypeInited)
            {
                LogTypeInit();
                LogTypeInited = true;
            }
            
            DateTime now = DateTime.Now;
            DateTime tempDt = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dtp_start.Value = tempDt;

            nmc = new NET_MESSAGE_CALLBACK_EX(fNET_MESSAGE_CALLBACK);
            bool ret = DevSdkHelper.NET_SDK_SetDVRMessageCallBackEx(nmc, IntPtr.Zero);

            FaceMatchInit();
            IVMinit();
            SmartSearchInit();
            GetSupport();
        }
        private void GetSupport()
        {
            bool ret = DevSdkHelper.NET_SDK_GetDeviceSupportFunction(userId, ref oDevSupport);
            if (ret)
            {
                int test = 0;
            }
            
        }
        public Live_Dialog(int id, NET_SDK_DEVICEINFO devInfo, string usr, string psw)
        {
            this.usr = usr;
            this.psw = psw;
            dfc = null;
            sync = SynchronizationContext.Current;
            ItemDataInit();
            InitializeComponent();
            userId = id;
            oNET_SDK_DEVICEINFO = devInfo;
            //cmb_Device.Text = Login_Dialog.m_ip;
            cmb_Device.Items.Add(devInfo.GetDeviceIPStr());
            cmb_Device.SelectedIndex = 0;
            int channelNum = oNET_SDK_DEVICEINFO.videoInputNum;
            if (channelNum > 0)
            {
                for (int i = 0; i < channelNum; i++)
                {
                    cmb_Channel.Items.Add("Channel:" + (i + 1));
                    treeViewNvr.Nodes.Add("Channel:" + (i + 1));
                    theTreeNodeName[i] = "Channel:" + (i + 1);
                }
            }

            foreach (Control ct in tabPage1.Controls)
            {
                if (ct.GetType() == typeof(Panel))
                {
                    ct.Paint += panel_Paint;
                    ct.MouseClick += panel_Click;
                    theStatus.Add(ct.Handle, initItemStatus);
                }
            }
            ContextMenuInit();

            cmb_Channel.SelectedItem = cmb_Channel.Items[0];
            cmb_StreamType.SelectedItem = cmb_StreamType.Items[0];
            cmb_Channel.SelectedIndexChanged += cmb_Channel_SelectedIndexChanged;
            cmb_StreamType.SelectedIndexChanged += cmb_StreamType_SelectedIndexChanged;

            currentPanelHandle = panel1.Handle;
            lastPanelHandle = panel1.Handle;

            ConfigMenuInit();

            ManualRecStatusInit(oNET_SDK_DEVICEINFO);

            PtzUIInit();

            LogTypeInit();
            DateTime now = DateTime.Now;
            DateTime tempDt = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dtp_start.Value = tempDt;

            //nmc = new NET_MESSAGE_CALLBACK(fNET_MESSAGE_CALLBACK);
            //bool ret = DevSdkHelper.NET_SDK_SetDVRMessageCallBack(nmc, IntPtr.Zero);

            FaceMatchInit();
            IVMinit();
            SmartSearchInit();
            GetSupport();
            //ttime.Elapsed += new System.Timers.ElapsedEventHandler(ttime_Elapsed);
            //ttime.Enabled = true;

            //InitChlCkbs();
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                //enableControlsOfControl(tabPage5, false);
                enableControlsOfControl(tabPage11, false);
                enableControlsOfControl(tabPage14, false);
                enableControlsOfControl(tabPage15, false);
                enableControlsOfControl(groupBox3, false);
            }
            else
            {
                //enableControlsOfControl(tabPage16, false);
                enableControlsOfControl(tabPage14, false);
                enableControlsOfControl(groupBox11, false);
                enableControlsOfControl(groupBox13, false);
            }
        }
        private void enableControlsOfControl(Control pg, bool enab)
        {
            foreach (Control ct in pg.Controls)
                ct.Enabled = enab;
        }
        private void RefreshDeviceList()
        {
        //    cmb_Device.Items.Clear();
        //    treeView1.Nodes.Clear();
        //    foreach (string ipStr in Program.m_deviceDic.Keys)
        //    {
        //        cmb_Device.Items.Add(ipStr);

        //        TreeNode tmp;
        //        StringBuilder tempSb = new StringBuilder(ipStr + "_");
        //        tempSb.Append(Program.m_deviceDic[ipStr].devInfo.GetDeviceName());
        //        tmp = new TreeNode(tempSb.ToString());
        //        sync.Post(AddTreeRootNode, tmp);
                
        //        sync.Post(SetSelectedNode, tmp);
        //        int channelNum = Program.m_deviceDic[ipStr].devInfo.videoInputNum;
        //        if (channelNum > 0)
        //        {                   
        //            for (int i = 0; i < channelNum; i++)
        //            {
        //                TreeNode tmpChild;
        //                tmpChild = new TreeNode(tempSb.ToString() + "_Channel:" + (i + 1));
        //                sync.Post(AddTreeChildNode, tmpChild);

        //                ChannelInfo tempChlInfo = new ChannelInfo();
        //                tempChlInfo.userId = Program.m_deviceDic[ipStr].userId;
        //                tempChlInfo.chlIndex = i;
        //                m_dicItem.Add(tmpChild, tempChlInfo);
        //            }
        //        }
        //    }
        //    cmb_Device.SelectedIndex = cmb_Device.Items.Count - 1;
        }
        private void RefreshDeviceChls()
        {
            cmb_Channel.Items.Clear();
            theTreeNodeName.Clear();
            int channelNum = oNET_SDK_DEVICEINFO.videoInputNum;
            if (channelNum > 0)
            {
                for (int i = 0; i < channelNum; i++)
                {
                    cmb_Channel.Items.Add("Channel:" + (i + 1));
                    treeViewNvr.Nodes.Add("Channel:" + (i + 1));
                    theTreeNodeName[i] = "Channel:" + (i + 1);
                }
            }
#region 16windows
            cmb_Channel_16windows.Items.Clear();
            if (channelNum > 0)
            {
                for (int i = 0; i < channelNum; i++)
                {
                    cmb_Channel_16windows.Items.Add("Channel:" + (i + 1));
                }
            }
#endregion
        }
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Panel pa = (Panel)sender;
            ControlPaint.DrawBorder(e.Graphics,
            pa.ClientRectangle,
            panelBorderColor, 1, ButtonBorderStyle.Solid,
            panelBorderColor, 1, ButtonBorderStyle.Solid,
            panelBorderColor, 1, ButtonBorderStyle.Solid,
            panelBorderColor, 1, ButtonBorderStyle.Solid);
            if ((theStatus[pa.Handle].playStatus) && blnDraw)
            {               
                if (rect != null && rect.Width > 0 && rect.Height > 0)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), rect);//重新绘制
                }
                
            }
        }

        
        //private void panel_Click(object sender, EventArgs e)
        private void panel_Click(object sender, MouseEventArgs e)
        {          
            Panel pa = (Panel)sender;
            currentPanelHandle = pa.Handle;
            
            if (currentPanelHandle != lastPanelHandle)
            {    
          
                UpdateUI(currentPanelHandle);

                #region 改边框颜色
                //panelBorderColor = Color.Green;
                //pa.Invalidate();
                //pa.Update();
                //string temp = pa.Name;
                //foreach (Control ct in tabPage1.Controls)
                //{
                //    if (ct.GetType() == typeof(Panel))
                //    {
                //        if (ct.Name != temp)
                //        {
                //            panelBorderColor = Color.Transparent;
                //            ct.Invalidate();
                //            ct.Update();
                //        }
                //    }
                //}
                #endregion
                lastPanelHandle = currentPanelHandle;
            }
            if(theStatus[currentPanelHandle].playStatus)//如果正在播放
            {
                NET_SDK_IMAGE_EFFECT_T bright = new NET_SDK_IMAGE_EFFECT_T();
                NET_SDK_IMAGE_EFFECT_T contrast = new NET_SDK_IMAGE_EFFECT_T();
                NET_SDK_IMAGE_EFFECT_T saturation = new NET_SDK_IMAGE_EFFECT_T();
                NET_SDK_IMAGE_EFFECT_T hue = new NET_SDK_IMAGE_EFFECT_T();
                bool tempbool = DevSdkHelper.NET_SDK_GetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, ref bright, ref contrast, ref saturation, ref hue);
                if (tempbool)
                {
                    if (tempbool)
                    {
                        //if (bright.curValue > 100)
                        //    tkbar_Brightness.Value = (int)(bright.curValue / 255 * 100);
                        //else
                            tkbar_Brightness.Maximum = (int)bright.maxValue;
                            tkbar_Brightness.Value = (int)bright.curValue;

                        //if (contrast.curValue > 100)
                        //    tkbar_Contrast.Value = (int)(contrast.curValue / 255 * 100);
                        //else
                            tkbar_Contrast.Maximum = (int)contrast.maxValue;
                            tkbar_Contrast.Value = (int)contrast.curValue;

                        //if (saturation.curValue > 100)
                        //    tkbar_Saturation.Value = (int)(saturation.curValue / 255 * 100);
                        //else
                            tkbar_Saturation.Maximum = (int)saturation.maxValue;
                            tkbar_Saturation.Value = (int)saturation.curValue;

                        //if (hue.curValue > 100)
                        //    tkbar_Hue.Value = (int)(hue.curValue / 255 * 100);
                        //else
                            tkbar_Hue.Maximum = (int)hue.maxValue;
                            tkbar_Hue.Value = (int)hue.curValue;
                    }
                    UpdateItemData();

                }
            }                              

        }



        private void cmb_Channel_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_ManualRecordTxtUpdate();
            UpdateItemData();
            if (theStatus[currentPanelHandle].playStatus)//if the panel is playing
            {
                if (currentPanelHandle == lastPanelHandle)
                {
                    RefreshPlaying();
                }
                //RefreshPlaying();
            }
        }

        private void cmb_StreamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //foreach (Control ct in tabPage17.Controls)
                //{
                //    Invoke(new Action(() => { ct.Enabled = false; }));

                //}
                UpdateItemData();
                if (theStatus[currentPanelHandle].playStatus)//if the panel is playing
                {
                    if (currentPanelHandle == lastPanelHandle)
                    {
                        
                        RefreshPlaying();
                    }
                    //RefreshPlaying();
                }
                //foreach (Control ct in tabPage17.Controls)
                //{
                //    Invoke(new Action(() => { ct.Enabled = true; }));

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void RefreshPlaying()
        {
            bool stopPlay = DevSdkHelper.NET_SDK_StopLivePlay(theStatus[currentPanelHandle].playHandle);
            if (stopPlay)
            {
                //
                Invoke(new Action(() =>
                {
                    Control.FromHandle(currentPanelHandle).BackColor = Color.Transparent;
                    Control.FromHandle(currentPanelHandle).BackColor = Color.Gray;
                    Control.FromHandle(currentPanelHandle).ContextMenuStrip = null;//右键菜单删除
                }));
               

                ItemStatus tempStatus = theStatus[currentPanelHandle];
                tempStatus.playStatus = false;
                theStatus[currentPanelHandle] = tempStatus;

                NET_SDK_CLIENTINFO clientInfo = new NET_SDK_CLIENTINFO();
                clientInfo.lChannel = theStatus[currentPanelHandle].channelIndex;
                clientInfo.hPlayWnd = currentPanelHandle;
                clientInfo.streamType = theStatus[currentPanelHandle].streamType;
                int playHandle = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, null, IntPtr.Zero);
                yuvldc = YuvLiveDataCallBack;
                DevSdkHelper.NET_SDK_SetYUVCallBack(playHandle, yuvldc, IntPtr.Zero);
                #region 更新数据
                if (playHandle != -1)
                {
                    tempStatus = theStatus[currentPanelHandle];
                    tempStatus.playHandle = playHandle;
                    tempStatus.playStatus = true;
                    theStatus[currentPanelHandle] = tempStatus;
                    UpdateItemData();
                }
                #endregion
            }
        }
        private bool RegisterDraw = false;
        private void btn_Live_Click(object sender, EventArgs e)
        {
            try
            {
                NET_SDK_CLIENTINFO clientInfo = new NET_SDK_CLIENTINFO();
                clientInfo.lChannel = theStatus[currentPanelHandle].channelIndex;
                clientInfo.hPlayWnd = currentPanelHandle;
                clientInfo.streamType = theStatus[currentPanelHandle].streamType;
                //clientInfo.bNoDecode = 1;
                if (((Button)sender).Text == "Stop live")
                {
                    ItemStatus tempStatus = theStatus[currentPanelHandle];
                    if (tempStatus.btn_LiveSave_txt == "Stop save")
                    {
                        bool ret = DevSdkHelper.NET_SDK_StopSaveLiveData(theStatus[currentPanelHandle].playHandle);
                        if (ret)
                        {
                            btn_LiveSave.Text = "Save live video";
                        }
                    }
                    bool stopPlay = DevSdkHelper.NET_SDK_StopLivePlay(theStatus[currentPanelHandle].playHandle);
                    if (stopPlay)
                    {
                        Control.FromHandle(currentPanelHandle).BackColor = Color.Transparent;
                        Control.FromHandle(currentPanelHandle).BackColor = Color.Gray;
                        Control.FromHandle(currentPanelHandle).ContextMenuStrip = null;//右键菜单删除

                        //tempStatus = theStatus[currentPanelHandle];
                        tempStatus.playStatus = false;
                        theStatus[currentPanelHandle] = tempStatus;

                        ((Button)sender).Text = "Start live";
                        btn_LiveSave.Enabled = false;
                        btn_Color_Default.Enabled = false;
                        btn_Track_Record.Enabled = false;
                        btn_Track.Enabled = false;
                        UpdateItemData();
                    }
                    ///
                    btn_OsdChlName.Enabled = false;
                    btn_OsdTitle.Enabled = false;
                    btn_OsdTime.Enabled = false;
                    btn_OsdChlName.Text = "chlName Open";
                    btn_OsdTitle.Text = "Title Open";
                    btn_OsdTime.Text = "Time Open";
                }
                else
                {
                    m_waveOutLive = new MyWaveOut();
                    WaveFormat whdrLive = new WaveFormat(8000, 16, 1);
                    m_waveOutLive.Start(whdrLive);
                    #region 增加回调
                    //NET_SDK_FRAME_INFO finfo = new NET_SDK_FRAME_INFO();
                    //WaveFormat whdrLive = new WaveFormat(8000, 16, 1);
                    //m_waveOutLive.Start(whdrLive);
                    myldc = MyLiveDataCallBack;
                    //myldc = null;
                    int playHandle = (int)DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, myldc, IntPtr.Zero);
                    //POINTERHANDLE playHandle = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, myldc, IntPtr.Zero);
                    yuvldc = YuvLiveDataCallBack;


                    DevSdkHelper.NET_SDK_SetYUVCallBack(playHandle, yuvldc, IntPtr.Zero);
                    #endregion
                    //Int32 playHandle = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, null, IntPtr.Zero);
                    int dwBufNum = 1;
//                     bool quality = DevSdkHelper.NET_SDK_SetPlayerBufNumber(playHandle, dwBufNum);
//                     if (!quality)
//                         ShowErrorMessage();
//                     else
//                         MessageBox.Show("dwBufNum is set to " + dwBufNum + "(the smaller the dwBufNum is, the smalller the video delay is)");

                    if (playHandle != -1)
                    {
                        temphandle = playHandle;
                        isPlay = true;
                        if(soundChannelOccupied)
                            Control.FromHandle(currentPanelHandle).ContextMenuStrip = audioDisabled;    
                        else
                            Control.FromHandle(currentPanelHandle).ContextMenuStrip = audioClosed;

                        #region 回调修改
                        //NET_SDK_FRAME_INFO finfo = new NET_SDK_FRAME_INFO();
                        //ldc = LiveDataCallBack;
                        //ldc(playHandle, finfo, IntPtr.Zero, currentPanelHandle);
                        #endregion
                        //if (dfc == null)
                        //    dfc = new DRAW_FUN_CALLBACK(OsdDisplay);
                        //bool ret = DevSdkHelper.NET_SDK_RegisterDrawFun(playHandle, dfc, currentPanelHandle);
                                          

                        ItemStatus tempStatus = theStatus[currentPanelHandle];
                        tempStatus.playHandle = playHandle;
                        tempStatus.playStatus = true;
                        theStatus[currentPanelHandle] = tempStatus;

                        ((Button)sender).Text = "Stop live";
                        btn_LiveSave.Enabled = true;
                        btn_Color_Default.Enabled = true;
                        btn_Track_Record.Enabled = true;
                        btn_Track.Enabled = true;

                        NET_SDK_IMAGE_EFFECT_T bright = new NET_SDK_IMAGE_EFFECT_T();
                        NET_SDK_IMAGE_EFFECT_T contrast = new NET_SDK_IMAGE_EFFECT_T();
                        NET_SDK_IMAGE_EFFECT_T saturation = new NET_SDK_IMAGE_EFFECT_T();
                        NET_SDK_IMAGE_EFFECT_T hue = new NET_SDK_IMAGE_EFFECT_T();
                        bool tempbool = DevSdkHelper.NET_SDK_GetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, ref bright, ref contrast, ref saturation, ref hue);
                        if (tempbool)
                        {
                            tkbar_Brightness.Maximum = (int)bright.maxValue;
                            tkbar_Brightness.Value = (int)bright.curValue;

                            tkbar_Contrast.Maximum = (int)contrast.maxValue;
                            tkbar_Contrast.Value = (int)contrast.curValue;

                           
                            tkbar_Saturation.Maximum = (int)saturation.maxValue;
                            tkbar_Saturation.Value = (int)saturation.curValue;
                     
                            tkbar_Hue.Maximum = (int)hue.maxValue;
                            if ((int)hue.curValue < 0)
                            {
                                tkbar_Hue.Enabled = false;
                            }
                            else
                            {
                                tkbar_Hue.Enabled = true;
                                tkbar_Hue.Value = (int)hue.curValue;
                            }
                                                 
                        }
                        UpdateItemData();
                    }
                    else
                    {
                        UInt32 dwErrorCode = DevSdkHelper.NET_SDK_GetLastError();
                        string strMsg = String.Format("{0} \r\nerror code: {1}", STR_LIVE_FAILED,dwErrorCode);
                        MessageBox.Show(strMsg,"Error",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    //
                    //btn_OsdChlName;
                    btn_OsdChlName.Enabled = true;
                    btn_OsdTitle.Enabled = true;
                    btn_OsdTime.Enabled = true;
                    btn_OsdChlName.Text = "chlName Open";
                    btn_OsdTitle.Text = "Title Open";
                    btn_OsdTime.Text = "Time Open";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void btn_Capture_Click(object sender, EventArgs e)
        {
            try
            {
                if (theStatus[currentPanelHandle].playStatus)
                {
                    saveFileDialog1.Filter = "jpg files(*.jpg)|*.jpg||";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string savePath = saveFileDialog1.FileName;
                       
                        bool ret = DevSdkHelper.NET_SDK_CapturePicture(theStatus[currentPanelHandle].playHandle, savePath);
                        if (ret)
                        {
                            MessageBox.Show("capture picture success");
                        }
                        else
                            MessageBox.Show("capture picture failed");
                    }
                }
                else
                {
                    saveFileDialog1.Filter = "jpg files(*.jpg)|*.jpg||";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string savePath = saveFileDialog1.FileName;
                        int picSize = 512 * 1024;
                        IntPtr sp = Marshal.AllocHGlobal(picSize);
                        int size = 0;
                        bool ret = DevSdkHelper.NET_SDK_CaptureJpeg(userId, theStatus[currentPanelHandle].channelIndex, 0, sp, picSize,ref size);
                        if (ret)
                        {
                            byte[] data = new byte[size];
                            Marshal.Copy(sp, data, 0, size);                         
                            if (File.Exists(savePath))
                                File.Delete(savePath);
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            BinaryWriter bw = new BinaryWriter(fs);
                            bw.Write(data, 0, data.Length);
                            fs.Flush();//数据写入图片文件
                            fs.Close();
                        }
                        Marshal.FreeHGlobal(sp);
                    }
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void btn_DeviceSnap_Click(object sender, EventArgs e)
        {
            bool ret = DevSdkHelper.NET_SDK_RemoteSnap(userId, cmb_Channel.SelectedIndex);
            if (!ret)
                ShowErrorMessage();
           
        }

        private void loginLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool ret = DevSdkHelper.NET_SDK_Logout(userId);
            if (ret)
            {
                //dfc -= OsdDisplay;
                //this.DialogResult = DialogResult.OK;
                //CloseAllLive();
                //string devIpToRemove = "";
                //foreach (NET_SDK_DEVICEINFO_EX devInfoEx in Program.m_deviceDic.Values)
                //{
                //    if(devInfoEx.userId == userId)
                //    {
                //        devIpToRemove = devInfoEx.devIP;
                //        break;
                //    }
                //}
                //if(devIpToRemove.Length > 0)
                //{
                //    Program.m_deviceDic.Remove(devIpToRemove);
                //    cmb_Device.Items.Remove(devIpToRemove);
                //    cmb_Device.SelectedIndex = cmb_Device.Items.Count - 1;
                //}
                    
            }
            else
                ShowErrorMessage();
            
        }

        private void AboutAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void deviceInformationMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NET_SDK_DEVICEINFO devInfo = new NET_SDK_DEVICEINFO ();
            bool ret = DevSdkHelper.NET_SDK_GetDeviceInfo(userId, ref devInfo);
            if (ret)
            {
                StringBuilder sbNET_SDK_DEVICEINFO_format = new StringBuilder();
                sbNET_SDK_DEVICEINFO_format.AppendLine("firmwareVersion: " + devInfo.GetFirmwareVersion());
                sbNET_SDK_DEVICEINFO_format.AppendLine("kernelVersion: " + devInfo.GetKernelVersion());
                sbNET_SDK_DEVICEINFO_format.AppendLine("hardwareVersion: " + devInfo.GetHardwareVersion());
                sbNET_SDK_DEVICEINFO_format.AppendLine("MCUVersion: " + devInfo.GetMCUVersion());
                sbNET_SDK_DEVICEINFO_format.AppendLine("firmwareVersionEx: " + devInfo.GetFirmwareVersionEx());
                sbNET_SDK_DEVICEINFO_format.AppendLine("deviceProduct: " + devInfo.GetDeviceProduct());
                sbNET_SDK_DEVICEINFO_format.AppendLine("deviceName: " + devInfo.GetDeviceName());
                sbNET_SDK_DEVICEINFO_format.AppendLine("deviceMac: " + devInfo.GetDeviceMAC());
                sbNET_SDK_DEVICEINFO_format.AppendLine("devicePort: " + devInfo.devicePort);
                sbNET_SDK_DEVICEINFO_format.AppendLine("device SN: " + DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(devInfo.szSN).Trim()));
                MessageBox.Show(sbNET_SDK_DEVICEINFO_format.ToString());
            }
            else
                ShowErrorMessage();
                       
        }

        private void btn_LiveSave_Click(object sender, EventArgs e)
        {
            try
            {               
                if (((Button)sender).Text == "Save live video")
                {
                    
                    saveFileDialog1.Filter = "avi files(*.avi)|*.avi||";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    { 
                        string savePath = saveFileDialog1.FileName;
                        bool ret = DevSdkHelper.NET_SDK_SaveLiveData(theStatus[currentPanelHandle].playHandle, savePath);
                        if (ret)
                        {
                            ((Button)sender).Text = "Stop save";
                            UpdateItemData();
                        }
                        else
                            ShowErrorMessage();
                    }

                }
                else
                {
                    bool ret = DevSdkHelper.NET_SDK_StopSaveLiveData(theStatus[currentPanelHandle].playHandle);
                    if (ret)
                    {
                        ((Button)sender).Text = "Save live video";
                        UpdateItemData();
                    }
                   
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MyLiveDataCallBack(int lLiveHandle, NET_SDK_FRAME_INFO frameInfo, IntPtr pBuffer, IntPtr pUser)//保存实时视频数据
        {
            //int curDivide = -1;
            //if (PBAudioDecHandles == null)
            //{
            //    PBAudioDecHandles = new int[16];
            //}
            //for (int i = 0; i < 16; i++)
            //{
            //    if (PBAudioDecHandles[i] == lLiveHandle)
            //    {
            //        curDivide = i;
            //    }
            //}

            if (theStatus[currentPanelHandle].playHandle != lLiveHandle)
            {
                return;
            }

            if (frameInfo.frameType == 5)//video format frame
            {                              
                if (frameInfo.length == Marshal.SizeOf(typeof(BITMAPINFOHEADER)))
                {
                    BITMAPINFOHEADER bitmapinfo = (BITMAPINFOHEADER)Marshal.PtrToStructure(pBuffer,typeof(BITMAPINFOHEADER));
                    if(DevSdkHelper.GetbiCompression(bitmapinfo.biCompression) == "H264" )
                        Debug.Print("is H264 \n");
                    else if(DevSdkHelper.GetbiCompression(bitmapinfo.biCompression) == "HEVC" )
                        Debug.Print("is H265 \n");
                }

                SDK_FRAME_INFO info = new SDK_FRAME_INFO();
                info.keyFrame = frameInfo.keyFrame;
                info.nLength = frameInfo.length;
                info.nHeight = frameInfo.height;
                info.nWidth = frameInfo.width;
                info.nStamp = frameInfo.time;
                info.frameType = frameInfo.frameType;


                int bb = Marshal.SizeOf(info);
                byte[] aa = StructToBytes(info, bb);
                MyWriteLog(aa, "record11_" + lLiveHandle + ".txt");//
                MyWriteFile(aa, "record11_" + lLiveHandle + ".txt");//
                uint len = frameInfo.length;
                aa = new byte[len];
                Marshal.Copy(pBuffer, aa, 0, (int)len);
                //if (dataIndex > 0)
                MyWriteLog(aa, "record11_" + lLiveHandle + ".txt");
                MyWriteFile(aa, "record11_" + lLiveHandle + ".txt");//
                //dataIndex++;
            }
            else if (frameInfo.frameType == 1)//video data frame
            {
                SDK_FRAME_INFO info = new SDK_FRAME_INFO();
                info.keyFrame = frameInfo.keyFrame;
                info.nLength = frameInfo.length;
                info.nHeight = frameInfo.height;
                info.nWidth = frameInfo.width;
                info.nStamp = frameInfo.time;
                info.frameType = frameInfo.frameType;


                int bb = Marshal.SizeOf(info);
                byte[] aa = StructToBytes(info, bb);
                MyWriteLog(aa, "record_" + lLiveHandle + ".txt");//
                //MyWriteFile(aa, "record_" + lLiveHandle + ".txt");//
                uint len = frameInfo.length;
                aa = new byte[len];
                Marshal.Copy(pBuffer, aa, 0, (int)len);
                //if (dataIndex > 0)
                MyWriteLog(aa, "record_" + lLiveHandle + ".txt");
                //MyWriteFile(aa, "record_" + lLiveHandle + ".txt");//
            }
            else if (frameInfo.frameType == 6)//audio format frame
            {
                PBAudioDecHandle = DevSdkHelper.NET_SDK_InitAudioDecoder(pBuffer, frameInfo.length);
            }
            else if (frameInfo.frameType == 2)//audio format frame
            {
                if (PBAudioOutBuffer != null)
                {
                    PBAudioOutBuffer = Marshal.AllocHGlobal(3200); ;
                    //IntPtr[] ptrs = new IntPtr[3200];
                }
                int outLen = 0;
                DevSdkHelper.NET_SDK_DecodeAudioFrame(PBAudioDecHandle, pBuffer, frameInfo.length, PBAudioOutBuffer, ref outLen);
                m_waveOutLive.WriteBuf(PBAudioOutBuffer, outLen);
            }
        }

        private void YuvLiveDataCallBack(int lLiveHandle, DECODE_FRAME_INFO frameInfo, IntPtr pUser)//YUV视频数据
        {
            Debug.Print("is Yuv live data callback \n");
        }
        private void MyWriteLog(byte[] byteArray, string name)
        {
            string fileName = name;
            StreamWriter fs;
            if (fileCreated)
            {
                
                fs = new StreamWriter(fileName, true);
                fs.WriteLine("original:");
                for(int i = 0;i< byteArray.Length;i++)
                {
                    fs.Write(byteArray[i].ToString() + " ");
                }
                fs.Flush();                                   
                fs.Close();//所有流类型都要关闭流，否则会出现内存泄露问题
            }
            else
            {
                FileStream fss = new FileStream(fileName, FileMode.Create); //创建一个文件流                                                       
                fss.Close();//所有流类型都要关闭流，否则会出现内存泄露问题

                fs = new StreamWriter(fileName, true);
                fs.WriteLine("original:");
                for (int i = 0; i < byteArray.Length; i++)
                {
                    fs.Write(byteArray[i].ToString() + " ");
                }
                fs.Flush();
                fs.Close();//所有流类型都要关闭流，否则会出现内存泄露问题
                fileCreated = true;
            }
        }
        private byte[] StructToBytes(object structObj, int size)
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷贝到byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }
        private bool fileCreated = false;//是否创建好录像保存文件
        //private int dataIndex = 0;//回调实时视频次数
        private void MyWriteFile(byte[] byteArray, string name)
        {
            string fileName = name;
            FileStream fs;
            if (fileCreated)
            {
                fs = new FileStream(fileName, FileMode.Append, FileAccess.Write); //   
                fs.Write(byteArray, 0, byteArray.Length);//将byte数组写入文件中                                                   
                fs.Close();//所有流类型都要关闭流，否则会出现内存泄露问题
            }
            else
            {
                fs = new FileStream(fileName, FileMode.Create); //创建一个文件流 
                fs.Write(byteArray, 0, byteArray.Length);//将byte数组写入文件中                                                    
                fs.Close();//所有流类型都要关闭流，否则会出现内存泄露问题
                fileCreated = true;
            }
        }
        private void LiveDataCallBack(Int32 lLiveHandle, NET_SDK_FRAME_INFO frameInfo, IntPtr pBuffer, IntPtr pUser)
        {         
            NET_SDK_IMAGE_EFFECT_T bright = new NET_SDK_IMAGE_EFFECT_T();
            NET_SDK_IMAGE_EFFECT_T contrast = new NET_SDK_IMAGE_EFFECT_T();
            NET_SDK_IMAGE_EFFECT_T saturation = new NET_SDK_IMAGE_EFFECT_T();
            NET_SDK_IMAGE_EFFECT_T hue = new NET_SDK_IMAGE_EFFECT_T();
            bool tempbool = DevSdkHelper.NET_SDK_GetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, ref bright, ref contrast, ref saturation, ref hue);
            if (tempbool)
            {
                //defaultBright = bright.defaultValue;
                //defaultContrast = contrast.defaultValue;
                //defaultSaturation = saturation.defaultValue;
                //defaultHue = hue.defaultValue;

                //tkbar_Brightness.Value = bright.curValue;
                //tkbar_Contrast.Value = contrast.curValue;
                //tkbar_Saturation.Value = saturation.curValue;
                //tkbar_Hue.Value = hue.curValue;

                //sync.Post(BrightnessUpate, bright.curValue);
                //sync.Post(ContrastUpate, contrast.curValue);
                //sync.Post(SaturationUpate, saturation.curValue);
                //sync.Post(HueUpate, hue.curValue);

                UpdateItemData();

            }
            else
                ShowErrorMessage();
        }
        private void BrightnessUpate(object obj)
        {
            tkbar_Brightness.Value = (int)obj;
        }
        private void ContrastUpate(object obj)
        {
            tkbar_Contrast.Value = (int)obj;
        }
        private void SaturationUpate(object obj)
        {
            tkbar_Saturation.Value = (int)obj;
        }
        private void HueUpate(object obj)
        {
            tkbar_Hue.Value = (int)obj;
        }

        private void tkbar_Brightness_Scroll(object sender, EventArgs e)
        {
            int bright = ((TrackBar)sender).Value;
            int contrast = theStatus[currentPanelHandle].contrastValue;
            int saturation = theStatus[currentPanelHandle].saturationValue;
            int hue = theStatus[currentPanelHandle].hueValue;

            bool tempbool = DevSdkHelper.NET_SDK_SetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, bright, contrast, saturation, hue);
            if (!tempbool)
                ShowErrorMessage();
            else
            {
                UpdateItemData();
            }
        }

        private void tkbar_Saturation_Scroll(object sender, EventArgs e)
        {
            int bright = theStatus[currentPanelHandle].brightValue;
            int contrast = theStatus[currentPanelHandle].contrastValue;
            int saturation = ((TrackBar)sender).Value;
            int hue = theStatus[currentPanelHandle].hueValue;

            bool tempbool = DevSdkHelper.NET_SDK_SetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, bright, contrast, saturation, hue);
            if (!tempbool)
                ShowErrorMessage();
            else           
                UpdateItemData();
            
        }

        private void tkbar_Hue_Scroll(object sender, EventArgs e)
        {
            int bright = theStatus[currentPanelHandle].brightValue;
            int contrast = theStatus[currentPanelHandle].contrastValue;
            int saturation = theStatus[currentPanelHandle].saturationValue;
            int hue = ((TrackBar)sender).Value;

            bool tempbool = DevSdkHelper.NET_SDK_SetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, bright, contrast, saturation, hue);
            if (!tempbool)
                ShowErrorMessage();
            else           
                UpdateItemData();
            
        }

        private void tkbar_Contrast_Scroll(object sender, EventArgs e)
        {
            int bright = theStatus[currentPanelHandle].brightValue;
            int contrast = ((TrackBar)sender).Value;
            int saturation = theStatus[currentPanelHandle].saturationValue;
            int hue = theStatus[currentPanelHandle].hueValue;

            bool tempbool = DevSdkHelper.NET_SDK_SetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, bright, contrast, saturation, hue);
            if (!tempbool)
                ShowErrorMessage();
            else           
                UpdateItemData();
            
        }

        private void btn_Color_Default_Click(object sender, EventArgs e)
        {
            tkbar_Brightness.Value = 25;
            tkbar_Contrast.Value = 50;
            tkbar_Saturation.Value = 50;
            tkbar_Hue.Value = 50;
            bool tempbool = DevSdkHelper.NET_SDK_SetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, 25, 50, 50, 50);
            if (!tempbool)
                ShowErrorMessage();
            else
                UpdateItemData();
            //tkbar_Brightness.Value = defaultBright;
            //tkbar_Contrast.Value = defaultContrast;
            //tkbar_Saturation.Value = defaultSaturation;
            //tkbar_Hue.Value = defaultHue;

            //bool tempbool = DevSdkHelper.NET_SDK_SetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, defaultBright, defaultContrast, defaultSaturation, defaultHue);
            //if (!tempbool)
            //    ShowErrorMessage();
            //else            
            //    UpdateItemData();
            
        }

        private void ToolStripMenuItem_StopAudio_Click(object sender, EventArgs e)
        {
            try
            {
                IntPtr cc = ((Panel)(((ContextMenuStrip)(((ToolStripMenuItem)sender).GetCurrentParent())).SourceControl)).Handle;
                currentPanelHandle = cc;
                bool ret = DevSdkHelper.NET_SDK_CloseSound();

                if (ret)
                {
                    soundChannelOccupied = false;
                    Control.FromHandle(currentPanelHandle).ContextMenuStrip = audioClosed;
                    tkbar_Volume.Enabled = false;
                    
                    UpdateItemData();
                    //************其他音频closed************
                    List<IntPtr> tempList = new List<IntPtr>();
                    ItemStatus tempStatus;
                    foreach (IntPtr tempPtr in theStatus.Keys)
                    {
                        if (tempPtr != currentPanelHandle)
                        {
                            tempStatus = theStatus[tempPtr];
                            if (tempStatus.playStatus)
                                tempList.Add(tempPtr);
                        }
                    }
                    foreach (IntPtr tempPtr in tempList)
                    {
                        tempStatus = theStatus[tempPtr];
                        tempStatus.cms = audioClosed;
                        Control.FromHandle(tempPtr).ContextMenuStrip = audioClosed;
                        theStatus[tempPtr] = tempStatus;
                    }
                    //**********************************************
                }
                //ShowErrorMessage(); 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
        private void ToolStripMenuItem_StartAudio_Click(object sender, EventArgs e)
        {
            try
            {
                IntPtr cc = ((Panel)(((ContextMenuStrip)(((ToolStripMenuItem)sender).GetCurrentParent())).SourceControl)).Handle;
                currentPanelHandle = cc;
                bool ret = DevSdkHelper.NET_SDK_OpenSound(theStatus[currentPanelHandle].playHandle);
                if (ret)
                {
                    soundChannelOccupied = true;
                    Control.FromHandle(currentPanelHandle).ContextMenuStrip = audioOpened;
                    tkbar_Volume.Enabled = true;
              
                    UpdateItemData();
                    //************其他音频disabled************
                    List<IntPtr> tempList = new List<IntPtr>();
                    ItemStatus tempStatus;
                    foreach (IntPtr tempPtr in theStatus.Keys)
                    {
                        if (tempPtr != currentPanelHandle)
                        {
                            tempStatus = theStatus[tempPtr];
                            if (tempStatus.playStatus)
                                tempList.Add(tempPtr);
                        }
                    }
                    foreach (IntPtr tempPtr in tempList)
                    {
                        tempStatus = theStatus[tempPtr];                  
                        tempStatus.cms = audioDisabled;
                        Control.FromHandle(tempPtr).ContextMenuStrip = audioDisabled;
                        theStatus[tempPtr] = tempStatus;
                    }
                    //**********************************************
                }
                else
                    ShowErrorMessage(); 
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
        private void ContextMenuInit()
        {
            audioOpened = new ContextMenuStrip();
            audioClosed = new ContextMenuStrip();
            audioDisabled = new ContextMenuStrip();

            openAudio = new ToolStripMenuItem("Start audio");            
            closeAudio = new ToolStripMenuItem("Stop audio");            
            disableAudio = new ToolStripMenuItem("Open audio");
            ToolStripMenuItem snap1 = new ToolStripMenuItem("Snap");
            ToolStripMenuItem snap2 = new ToolStripMenuItem("Snap");
            ToolStripMenuItem snap3 = new ToolStripMenuItem("Snap");

            openAudio.Click += ToolStripMenuItem_StartAudio_Click;
            closeAudio.Click += ToolStripMenuItem_StopAudio_Click;
            //snap.Click += ToolStripMenuItem_Click;
            disableAudio.Enabled = false;

            audioOpened.Items.Add(closeAudio);
            //audioOpened.Items.Add(snap1);

            audioClosed.Items.Add(openAudio);
            //audioClosed.Items.Add(snap2);

            audioDisabled.Items.Add(disableAudio);
            //audioDisabled.Items.Add(snap3);

        }

        private void tkbar_Volume_Scroll(object sender, EventArgs e)
        {
            if (((TrackBar)sender).Enabled)
            {
                double tempVolume = ((TrackBar)sender).Value;
                tempVolume = tempVolume / 100 * 0xffff;
                bool ret = DevSdkHelper.NET_SDK_Volume(theStatus[currentPanelHandle].playHandle,(int)tempVolume);
                if (!ret)
                    ShowErrorMessage();
            }
        }
        private void CloseAllLive()
        {
            //************************
            List<IntPtr> tempList = new List<IntPtr>();
            ItemStatus tempStatus;
            foreach (IntPtr tempPtr in theStatus.Keys)
            {                
                tempStatus = theStatus[tempPtr];
                if (tempStatus.playStatus)
                    tempList.Add(tempPtr);               
            }
            foreach (IntPtr tempPtr in tempList)
            {
                bool stopPlay = DevSdkHelper.NET_SDK_StopLivePlay(theStatus[tempPtr].playHandle);
                if (stopPlay)
                {
                    ItemDataInit();
                    theStatus[tempPtr] = initItemStatus; UpdateUI(tempPtr);
                }
            }
            //**********************************************           
        }

        private void btn_ManualRecord_Click(object sender, EventArgs e)
        {
            int tempChannel = cmb_Channel.SelectedIndex;
            if (btn_ManualRecord.Text == "Device Manual Record")
            {                
                bool ret = DevSdkHelper.NET_SDK_StartDVRRecord(userId, tempChannel, 0);
                if (ret)
                {
                    btn_ManualRecord.Text = "Stop Manual Record";
                    manualRecStatus[tempChannel] = true;
                    MessageBox.Show("Channel " + (tempChannel + 1) + " start manual recording");
                }
                else
                    ShowErrorMessage();

            }
            else
            {
                bool ret = DevSdkHelper.NET_SDK_StopDVRRecord(userId, tempChannel);
                if (ret)
                {
                    btn_ManualRecord.Text = "Device Manual Record";
                    manualRecStatus[tempChannel] = false;
                    MessageBox.Show("Channel " + (tempChannel + 1) + " stopped manual record");
                }
                else
                    ShowErrorMessage();
            }
        }
        private void btn_ManualRecordTxtUpdate()
        {
            int tempChannel = cmb_Channel.SelectedIndex;
            if(manualRecStatus[tempChannel])
                btn_ManualRecord.Text = "Stop Manual Record";
            else
                btn_ManualRecord.Text = "Device Manual Record";
        }
        private void ShowErrorMessage()
        {
            string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
            MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btn_MakeIFrame_Click(object sender, EventArgs e)
        {
            if (cmb_StreamType.SelectedIndex == 0)           
               DevSdkHelper.NET_SDK_MakeKeyFrame(userId, cmb_Channel.SelectedIndex);
            else            
                DevSdkHelper.NET_SDK_MakeKeyFrameSub(userId, cmb_Channel.SelectedIndex);                                              
        }
        private void OsdTitleDisplay(int lLiveHandle, IntPtr hDC, IntPtr pUser)
        {
            //string txt = textBox_Title.Text;
            //指定字体
            Font drawFont = new Font("Arial", 12);
            Graphics gh = Graphics.FromHdc(hDC);
            SolidBrush drawBrush = new SolidBrush(Color.Yellow);
            if (osdTitle)
            {
                Point a = new Point(5, 20);
                gh.DrawString(textBox_Title.Text, drawFont, drawBrush, a);
            }
            if (osdChlName)
            {
                Point b = new Point(5, 40);
                gh.DrawString("channel:1", drawFont, drawBrush, b);
            }
            if (osdTime)
            {
                Point c = new Point(5, 60);
                gh.DrawString(DateTime.Now.ToString(), drawFont, drawBrush, c);
            }

            //Point b = new Point(50, 50);
            //gh.DrawString("lala", drawFont, drawBrush, b);
            //IntPtr last_font = SelectObject(hDC, drawFont.ToHfont());
            //TextOut(hDC, 10, 10, txt, txt.Length);
        }

        private void OsdDisplay(int lLiveHandle, IntPtr hDC, IntPtr pUser)
        {
            string txt = "example to show the OSD";
            //指定字体
            Font drawFont = new Font("Arial", 18);
            Graphics gh = Graphics.FromHdc(hDC);
            SolidBrush drawBrush = new SolidBrush(Color.Red);
            Point a = new Point(10, 10);
            gh.DrawString(txt, drawFont, drawBrush, a);

            Point b = new Point(50, 50);
            gh.DrawString("lala", drawFont, drawBrush, b);
            //IntPtr last_font = SelectObject(hDC, drawFont.ToHfont());
            //TextOut(hDC, 10, 10, txt, txt.Length);
        }
        private void deviceTimeTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DD_TIME tm = new DD_TIME();
            bool ret = DevSdkHelper.NET_SDK_GetDeviceTime(userId, ref tm);
            if (ret)
            {               
                int year = tm.year + 1900;
                byte month =(byte)(tm.month + 1);
                string _format = year.ToString() + "." + AddZero(month) + "." + AddZero(tm.mday) + " " + AddZero(tm.hour) + ":" + AddZero(tm.minute) + ":" + AddZero(tm.second);
                MessageBox.Show(_format.ToString());
            }
            else
                ShowErrorMessage();
                

        }

        private void deviceCHStatusCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int chnCount = NET_SDK_CH_DEVICE_STATUS.GetSize() * oNET_SDK_DEVICEINFO.videoInputNum;

            NET_SDK_CH_DEVICE_STATUS[] chnInfo = new NET_SDK_CH_DEVICE_STATUS[chnCount];
            IntPtr intptrChnInfo = PointArrayToIntPtr(chnInfo);
            IntPtr pTmp = intptrChnInfo;
            int bufferSize = NET_SDK_CH_DEVICE_STATUS.GetSize() * chnCount;
            bool ret = DevSdkHelper.NET_SDK_GetDeviceCHStatus(userId, intptrChnInfo, bufferSize, ref chnCount);
            if (ret)
            {
                StringBuilder ipcInfomation = new StringBuilder();
                string name = "";
                for (int i = 0; i < chnCount; i++, pTmp = IntPtr.Add(pTmp, NET_SDK_CH_DEVICE_STATUS.GetSize()))
                {
                    byte[] pByte = new byte[NET_SDK_CH_DEVICE_STATUS.GetSize()];
                    Marshal.Copy(pTmp, pByte, 0, NET_SDK_CH_DEVICE_STATUS.GetSize());
                    chnInfo[i] = (NET_SDK_CH_DEVICE_STATUS)BytesToStruct(pByte, typeof(NET_SDK_CH_DEVICE_STATUS));
                    name = " name:" + Encoding.UTF8.GetString(chnInfo[i].name).Replace("\0", " ").Trim();
                    ipcInfomation.AppendLine("Channel:" + chnInfo[i].channel + " status:" + chnInfo[i].status + " chlType:" + chnInfo[i].chlType + name);              
                }
                MessageBox.Show(ipcInfomation.ToString());
            }
            Marshal.FreeHGlobal(intptrChnInfo);
        }

        private void SDK_GUIDEDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            { 
                Process.Start(Application.StartupPath + "\\Device Net SDK manual.CHM"); 
            }
            catch
            { }
        }

        private void PtzControl_MouseDown(object sender, MouseEventArgs e)
        {
            int index =Convert.ToInt16(((Button)sender).Tag);
            bool ret = DevSdkHelper.NET_SDK_PTZControl(theStatus[currentPanelHandle].playHandle, index, tkbar_Speed.Value);
            if(!ret)
                ret = DevSdkHelper.NET_SDK_PTZControl_Other(userId, cmb_Channel.SelectedIndex, index, tkbar_Speed.Value);
        }
        private void PtzControl_MouseUp(object sender, MouseEventArgs e)
        {
            bool ret = DevSdkHelper.NET_SDK_PTZControl(theStatus[currentPanelHandle].playHandle, 0, tkbar_Speed.Value);
            if (!ret)
                ret = DevSdkHelper.NET_SDK_PTZControl_Other(userId, cmb_Channel.SelectedIndex, 0, tkbar_Speed.Value);
        }

        private void Preset_Click(object sender, EventArgs e)
        {
            int temp = theStatus[currentPanelHandle].playHandle;
            int index = Convert.ToInt16(((Button)sender).Tag);
            bool ret = DevSdkHelper.NET_SDK_PTZPreset(temp, index, cmb_Preset.SelectedIndex + 1);
            if (!ret)
                ret = DevSdkHelper.NET_SDK_PTZPreset_Other(userId, cmb_Channel.SelectedIndex, index, cmb_Preset.SelectedIndex + 1);
        }

        private void CruiseRun_Click(object sender, EventArgs e)
        {

        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (theStatus[((Panel)sender).Handle].playStatus)
            {
                start = e.Location;
                Invalidate();
                blnDraw = true;
            }
           
        }
        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            Panel pa = (Panel)sender;
            if (theStatus[pa.Handle].playStatus)
            {
                blnDraw = false;

                PTZ_3D_POINT_INFO ptz3DInfo = new PTZ_3D_POINT_INFO();
                ptz3DInfo.selBeginX = start.X;
                ptz3DInfo.selBeginY = start.Y;
                ptz3DInfo.selEndX = e.X;
                ptz3DInfo.selEndY = e.Y;
                ptz3DInfo.displayWidth = pa.Width;
                ptz3DInfo.displayHeight = pa.Height;

                bool bRet = DevSdkHelper.NET_SDK_PTZControl_3D(theStatus[pa.Handle].playHandle, 0, ref ptz3DInfo);              
            }
        }
        private void panel_MouseMove(object sender, MouseEventArgs e)
        {

            if (blnDraw)
            {
                if (e.Button != MouseButtons.Left)//判断是否按下左键
                    return;
                Point tempEndPoint = e.Location; //记录框的位置和大小
                rect.Location = new Point(
                Math.Min(start.X, tempEndPoint.X),
                Math.Min(start.Y, tempEndPoint.Y));
                rect.Size = new Size(
                Math.Abs(start.X - tempEndPoint.X),
                Math.Abs(start.Y - tempEndPoint.Y));
                ((Panel)sender).Invalidate();
            }
                                    
        }

        private void btn_PTZ_3D_Ctrl_Click(object sender, EventArgs e)
        {
            m_bPTZ3DCtrling = !m_bPTZ3DCtrling;
            m_bEnablePTZ3DCtrl = m_bPTZ3DCtrling;
            if (!m_bPTZ3DCtrling)
            {
                ((Button)sender).Text = "Start";
                foreach (Control ct in tabPage1.Controls)
                {
                    if (ct.GetType() == typeof(Panel))
                    {
                        ct.MouseDown -= panel_MouseDown;
                        ct.MouseMove -= panel_MouseMove;
                        ct.MouseUp -= panel_MouseUp;
                    }
                }
            }
            else
            {
                ((Button)sender).Text = "Stop";
                foreach (Control ct in tabPage1.Controls)
                {
                    if (ct.GetType() == typeof(Panel))
                    {
                        ct.MouseDown += panel_MouseDown;
                        ct.MouseMove += panel_MouseMove;
                        ct.MouseUp += panel_MouseUp;                       
                    }
                }
                
            }
        }

        private void btn_Cruise_Setup_Click(object sender, EventArgs e)
        {            
            if (cmb_Cruise.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a cruise");
            }
            else
            {
                int temp = theStatus[currentPanelHandle].playHandle;
                int index = Convert.ToInt16(((Button)sender).Tag);
                //bool ret = DevSdkHelper.NET_SDK_PTZCruise(temp, index, cmb_Cruise.SelectedIndex);
                //bool ret = DevSdkHelper.NET_SDK_PTZCruise(temp, 18, cmb_Cruise.SelectedIndex);
                //if (!ret)
                //    ret = DevSdkHelper.NET_SDK_PTZCruise_Other(userId, cmb_Channel.SelectedIndex, index, cmb_Cruise.SelectedIndex);
                bool ret = true;
                if (ret)
                {
                    Cruise cs = new Cruise();
                    if (cs.ShowDialog() == DialogResult.OK)
                    {
                        cruiseInfoArray = cs.pCruisePoint;
                        int presetNum = cs.pCruisePoint.Length;

                        int size = Marshal.SizeOf(typeof(DD_CRUISE_POINT_INFO));

                        IntPtr intptrCruiseInfo = PointArrayToIntPtr(cruiseInfoArray);
                        IntPtr pTmp = intptrCruiseInfo;
                        int bufferSize = size * presetNum;

                        ret = DevSdkHelper.NET_SDK_PTZSetCruise(temp, cmb_Cruise.SelectedIndex + 1, intptrCruiseInfo, presetNum);
                        if (!ret)
                        {
                            ret = DevSdkHelper.NET_SDK_PTZSetCruise_Other(userId, cmb_Channel.SelectedIndex, cmb_Cruise.SelectedIndex + 1, intptrCruiseInfo, presetNum);
                        }
                        Marshal.FreeHGlobal(intptrCruiseInfo);
                    }
                }
            }
           
        }

        private void btn_Track_Record_Click(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);
            string tempTxt = btn.Text;
            int temp = theStatus[currentPanelHandle].playHandle;
            int index = Convert.ToInt16(btn.Tag);
            bool ret = DevSdkHelper.NET_SDK_PTZTrack(temp, index);
            if (!ret)
                ret = DevSdkHelper.NET_SDK_PTZTrack_Other(userId, cmb_Channel.SelectedIndex, index);
            if (ret)
            {
                if (tempTxt == "Record")
                {
                    btn.Text = "Stop";
                    btn.Tag = 29;
                }
                else
                {
                    btn.Text = "Record";
                    btn.Tag = 28;
                }
                UpdateItemData();
            }           

        }

        private void btn_Track_Click(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);
            string tempTxt = btn.Text;
            int temp = theStatus[currentPanelHandle].playHandle;
            int index = Convert.ToInt16(btn.Tag);
            bool ret = DevSdkHelper.NET_SDK_PTZTrack(temp, index);
            if (!ret)
                ret = DevSdkHelper.NET_SDK_PTZTrack_Other(userId, cmb_Channel.SelectedIndex, index);
            if (ret)
            {
                if (tempTxt == "Start")
                {
                    btn.Text = "Stop";
                    btn.Tag = 27;
                }
                else
                {
                    btn.Text = "Start";
                    btn.Tag = 26;
                }
                UpdateItemData();
            }                                 
        }

        private void btn_AutoScan_Click(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);
            string tempTxt = btn.Text;
            int temp = theStatus[currentPanelHandle].playHandle;
            int index = Convert.ToInt16(btn.Tag);
            bool ret = DevSdkHelper.NET_SDK_PTZAutoScan(temp, index, tkbar_Speed.Value, false);
            if(!ret)
                ret = DevSdkHelper.NET_SDK_PTZAutoScan_Other(userId, cmb_Channel.SelectedIndex, true);
            if (ret)
            {
                if (tempTxt == "Start")
                {
                    btn.Text = "Stop";
                    btn.Tag = 30;
                }
                else
                {
                    btn.Text = "Start";
                    btn.Tag = 29;
                }
            }
        }

        private void btn_AutoScan2_Click(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);
            string tempTxt = btn.Text;
            int temp = theStatus[currentPanelHandle].playHandle;
            int index = Convert.ToInt16(btn.Tag);
            bool ret = DevSdkHelper.NET_SDK_PTZAutoScan(temp, index, tkbar_Speed.Value);
            if (!ret)
                ret = DevSdkHelper.NET_SDK_PTZAutoScan_Other(userId, cmb_Channel.SelectedIndex, false);
            if (ret)
            {
                if (tempTxt == "Start")
                {
                    btn.Text = "Stop";
                    btn.Tag = 30;
                }
                else
                {
                    btn.Text = "Start";
                    btn.Tag = 29;
                }
            }
        }

        private void CHTypePToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int camType = -1;
            bool ret = DevSdkHelper.NET_SDK_GetPTZCameraType(userId, ref camType);
            if (ret)
            {
                switch (camType)
                {
                    case 0:
                        MessageBox.Show("This camera does not support PTZ");
                        break;
                    case 1:
                        MessageBox.Show("This camera is support PTZ box camera");
                        break;
                    case 2:
                        MessageBox.Show("This camera is support PTZ dome");
                        break;
                }

            }
            else
                MessageBox.Show("This device is not camera or dome");
        }

        private void pTZListLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int listNum = oNET_SDK_DEVICEINFO.videoInputNum;
            NET_SDK_CHANNEL_PTZ[] pOutChannelPtz = new NET_SDK_CHANNEL_PTZ[listNum];
            int returnListNum = 0;

            int size = Marshal.SizeOf(typeof(NET_SDK_CHANNEL_PTZ));

            IntPtr ptzListInfo = PointArrayToIntPtr(pOutChannelPtz);
            IntPtr pTmp = ptzListInfo;
            int bufferSize = size * listNum;

            bool ret = DevSdkHelper.NET_SDK_GetSupportPtzList(userId, listNum, ptzListInfo, ref returnListNum);
            if (ret)
            {
                StringBuilder ptzListInfomation = new StringBuilder();
                string tempStr = "";
                for (int i = 0; i < returnListNum; i++, pTmp = IntPtr.Add(pTmp, size))
                {
                    byte[] pByte = new byte[size];
                    Marshal.Copy(pTmp, pByte, 0, size);
                    pOutChannelPtz[i] = (NET_SDK_CHANNEL_PTZ)BytesToStruct(pByte, typeof(NET_SDK_CHANNEL_PTZ));
                    tempStr = " channel:" + pOutChannelPtz[i].dwChannel + " channelType:" + judgeChnType((int)pOutChannelPtz[i].eChanneltype);
                    ptzListInfomation.AppendLine(tempStr);
                }
                MessageBox.Show(ptzListInfomation.ToString());
                
            }
            Marshal.FreeHGlobal(ptzListInfo);
 
        }
        private string judgeChnType(int i)
        {
            string res = "";
            switch (i)
            {
                case 0:
                    res = "Null channel";
                    break;
                case 1:
                    res = "Digital channel";
                    break;
                case 2:
                    res = "Analog channel";
                    break;
                case 3:
                    res = "Alarm out channel";
                    break;
                case 4:
                    res = "Sensor channel";
                    break;
            }
            return res;
        }

        private void getChnPresetPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*************************get channel's presets**********************************************/
            #region get channel's presets
            DD_PTZ_PRESET_CONFIG_Ex[] sPreset = new DD_PTZ_PRESET_CONFIG_Ex[255];
            int size = Marshal.SizeOf(typeof(DD_PTZ_PRESET_CONFIG_Ex));

            int bufferSize = size * 255;
            IntPtr presetInfo = Marshal.AllocHGlobal(bufferSize);

            int length = 0;
            //IntPtr tempInt = Marshal.AllocHGlobal(4);

            bool ret = DevSdkHelper.NET_SDK_GetPTZConfig(userId, cmb_Channel.SelectedIndex, 1, IntPtr.Zero, 0, presetInfo, bufferSize, ref length);
            if (ret && length > 0)
            {
                IntPtr tempPtr = presetInfo;
                for (int i = 0; i < length / size; i++)
                {
                    sPreset[i] = (DD_PTZ_PRESET_CONFIG_Ex)Marshal.PtrToStructure(tempPtr + i * size, typeof(DD_PTZ_PRESET_CONFIG_Ex));
                }
            }
            Marshal.FreeHGlobal(presetInfo);
            #endregion
            /***********************************************************************/
            /*************************get channel's cruises**********************************************/
            #region get channel's cruise           
            //DD_CH_CRUISE[] sCruse = new DD_CH_CRUISE[128];
            //int size = Marshal.SizeOf(typeof(DD_CH_CRUISE));

            //int bufferSize = size * 128;
            //IntPtr cruiseInfo = Marshal.AllocHGlobal(bufferSize);

            //int length = 0;
            ////IntPtr tempInt = Marshal.AllocHGlobal(4);

            //bool ret = DevSdkHelper.NET_SDK_GetPTZConfig(userId, cmb_Channel.SelectedIndex, 1, IntPtr.Zero, 0, cruiseInfo, bufferSize, ref length);
            //if (ret && length > 0)
            //{
            //    IntPtr tempPtr = cruiseInfo;
            //    for (int i = 0; i < length / size; i++)
            //    {
            //        sCruse[i] = (DD_CH_CRUISE)Marshal.PtrToStructure(tempPtr + i * size, typeof(DD_CH_CRUISE));
            //    }
            //}
            //Marshal.FreeHGlobal(cruiseInfo);
            #endregion
            /***********************************************************************/
            /*************************get cruise's information **********************************************/
            #region get cruise's information
            //DD_CRUISE_POINT_INFO[] sCruiseInfo = new DD_CRUISE_POINT_INFO[128];
            //int size = Marshal.SizeOf(typeof(DD_CRUISE_POINT_INFO));

            //int bufferSize = size * 128;
            //IntPtr cruiseInfo = Marshal.AllocHGlobal(bufferSize);

            //int length = 0;
            //IntPtr tempInt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));          
            //int cruiseIndex = 1;//suppose the cruise's index is 1
            //Marshal.WriteInt32(tempInt, cruiseIndex);

            //bool ret = DevSdkHelper.NET_SDK_GetPTZConfig(userId, cmb_Channel.SelectedIndex, 1, tempInt, Marshal.SizeOf(typeof(int)), cruiseInfo, bufferSize, ref length);
            //if (ret && length > 0)
            //{
            //    IntPtr tempPtr = cruiseInfo;
            //    for (int i = 0; i < length / size; i++)
            //    {
            //        sCruiseInfo[i] = (DD_CRUISE_POINT_INFO)Marshal.PtrToStructure(tempPtr + i * size, typeof(DD_CRUISE_POINT_INFO));
            //    }
            //}
            //Marshal.FreeHGlobal(tempInt);
            //Marshal.FreeHGlobal(cruiseInfo);
            #endregion
            /***********************************************************************/
        }

        private void btn_SendXml_Click(object sender, EventArgs e)
        {
            string m_sendUrl = tb_SendUrl.Text;
            if (m_sendUrl == null || m_sendUrl.Length == 0)
                return;
            tb_RecvXml.Clear();
            string m_sendXml = tb_SendXml.Text;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                
                tb_RecvXml.Text =DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (Faces.Count > 0)
            //{
            //    saveFileDialog1.Filter = "txt files(*.txt)|*.txt||";
            //    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //    {
            //        string path = saveFileDialog1.FileName;
            //        if (!System.IO.File.Exists(path))
            //        {
            //            FileStream stream = System.IO.File.Create(path);
            //            stream.Close();
            //            stream.Dispose();
            //        }
            //        using (StreamWriter writer = new StreamWriter(path, true))
            //        {
            //            for (int i = 0; i < Faces.Count; i++)
            //            {
            //                writer.WriteLine(Faces[i].time + "  " + Faces[i].idNum);
            //            }
            //            MessageBox.Show("导出成功");
            //        }
            //    }
            //}
            if (lv_Faces.Items.Count > 0)
            {
                saveFileDialog1.Filter = "txt files(*.txt)|*.txt||";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog1.FileName;
                    if (!System.IO.File.Exists(path))
                    {
                        FileStream stream = System.IO.File.Create(path);
                        stream.Close();
                        stream.Dispose();
                    }
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        for (int i = 0; i < lv_Faces.Items.Count; i++)
                        {
                            writer.WriteLine(lv_Faces.Items[i].SubItems[1].Text + "  " + lv_Faces.Items[i].SubItems[2].Text);
                        }
                        MessageBox.Show("导出成功");
                    }
                }
            }

        }
        private Log myExceptionLog;
        private Log myAlarmLog;
        public static EXCEPTION_CALLBACK excertion = null;
        private void button2_Click(object sender, EventArgs e)
        {
            myExceptionLog = new Log();
            myExceptionLog.Initial(true, Application.StartupPath, "ExceptionLog");//
            excertion = excertionCallback;
            DevSdkHelper.NET_SDK_SetSDKMessageCallBack(0, IntPtr.Zero, excertion, null);
        }
        bool dvrDisconnected = false;
        private void excertionCallback(UInt32 dwType, Int32 lUserID, Int32 lHandle, IntPtr pUser)
        {
            if (0 == dwType)
            {
                dvrDisconnected = true;
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_DISCONNECT\r\n", dwType, lUserID, lHandle);
                //MessageBox.Show("device disconnected");
                myExceptionLog.WriteLog(temp, 0);
                Debug.Print(temp);
            }
            else if (1 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_RECONNECT\r\n", dwType, lUserID, lHandle);
                //MessageBox.Show("device reconnected"); ;
                myExceptionLog.WriteLog(temp, 0);
                Debug.Print(temp);
            }
            else if (2 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_CH_DISCONNECT\r\n", dwType, lUserID, lHandle); ;
                myExceptionLog.WriteLog(temp, 0);
                Debug.Print(temp);
            }
            else if (3 == dwType)
            {
                string temp = string.Format("\r\ntype:{0}, userID:{1}, handle:{2}---NETWORK_CH_RECONNECT\r\n", dwType, lUserID, lHandle); ;
                myExceptionLog.WriteLog(temp, 0);
                Debug.Print(temp);
            }
        }

        private void btn_SendTransparent_Click(object sender, EventArgs e)
        {
            string m_sendUrl = tb_SendUrl.Text;
            if (m_sendUrl == null || m_sendUrl.Length == 0)
                return;
            tb_RecvXml.Clear();

            string temp1 = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'>
                                <content>
                                <userName><![CDATA[";
            string temp2 = @"]]></userName>
                                <password><![CDATA[123456]]></password>
                                <email><![CDATA[]]></email>                                <modifyPassword>true</modifyPassword>                                <authGroupId><![CDATA[]]></authGroupId>                                <bindMacSwitch>false</bindMacSwitch>                                <mac><![CDATA[00:00:00:00:00:00]]></mac>
                                <enabled>true</enabled>
                                <authEffective>true</authEffective>
                                </content>
                                </request>";
//            string temp2 = @"]]></userName>
//                                <password><![CDATA[123456]]></password>
//                                <email><![CDATA[]]></email>//                                <modifyPassword>true</modifyPassword>//                                <authGroupId><![CDATA[{60AAD568-6366-9844-87DB-664CBF60657A}]]></authGroupId>//                                <bindMacSwitch>false</bindMacSwitch>//                                <mac><![CDATA[00:00:00:00:00:00]]></mac>
//                                <enabled>true</enabled>
//                                <authEffective>true</authEffective>
//                                </content>
//                                </request>";
            string sendXML = temp1 + get_uft8("admin1") + temp2;
            //string m_sendXml = sendXML;
            //tb_SendXml.Text = "lala";
            string m_sendXml = tb_SendXml.Text.Trim();
            //string m_sendXml = tb_SendXml.Text;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);

                tb_RecvXml.Text = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }
        private List<UserInfo> userInfoDic = new List<UserInfo>();
        public struct UserInfo
        {
            public string id;//user id
            public string userName;
            public string password;
            public authGroupStruct authGroup;
            public string bindMacSwitch;
            public string mac;
            public string email;
            public string enabled;
            public string modifyPassword;
            public string comment;
            public string authEffective;
        }
        public struct authGroupStruct
        {
            public string id;
            public string name;
        }
        private void btn_QueryUserList_Click(object sender, EventArgs e)
        {
            InitUserCkbs();
            userInfoDic.Clear();
            string m_sendUrl = "queryUserList";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    XmlDocument oldXd = new XmlDocument();
                    oldXd.LoadXml(retStr);
                    XmlNodeList nodeList = oldXd.GetElementsByTagName("item");
                    if (nodeList.Count > 0)
                    {
                        userInfoDic.Clear();
                        
                        string tempRoot = @"<?xml version = '1.0' encoding='utf-8'?><temp>";
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            UserInfo tempUserInfo = new UserInfo();
                            authGroupStruct tempAuthGroupInfo = new authGroupStruct();
                            string a = tempRoot + nodeList[i].InnerXml + "</temp>";
                            //XmlDocument newXd = new XmlDocument();
                            //newXd.LoadXml(a);
                            tempUserInfo.id = nodeList[i].Attributes[0].InnerText;
                            tempUserInfo.userName = CommonMethods.GetNodeValue(a, "userName");
                            tempUserInfo.password = CommonMethods.GetNodeValue(a, "password");
                            tempAuthGroupInfo.name = CommonMethods.GetNodeValue(a, "authGroup");
                            tempAuthGroupInfo.id = CommonMethods.GetNodeAttribute(CommonMethods.GetNodePosByIndex(a,"authGroup"),"id");

                            tempUserInfo.authGroup = tempAuthGroupInfo;

                            tempUserInfo.bindMacSwitch = CommonMethods.GetNodeValue(a, "bindMacSwitch");

                            tempUserInfo.mac = CommonMethods.GetNodeValue(a, "mac");
                            tempUserInfo.email = CommonMethods.GetNodeValue(a, "email");
                            tempUserInfo.enabled = CommonMethods.GetNodeValue(a, "enabled");
                            tempUserInfo.modifyPassword = CommonMethods.GetNodeValue(a, "modifyPassword");
                            tempUserInfo.comment = CommonMethods.GetNodeValue(a, "comment");
                            tempUserInfo.authEffective = CommonMethods.GetNodeValue(a, "authEffective");

                            userInfoDic.Add(tempUserInfo);
                            
                        }
                    }

                }
                UpdateUserList();               
            }
            Marshal.FreeHGlobal(lpOutBuffer);

        }
        private void UpdateUserList()
        {
            if (userInfoDic.Count > 0)
            {
                lv_UserList.Items.Clear();
                for (int i = 0; i < userInfoDic.Count;i++ )
                {
                    string authGroupName;
                    if (userInfoDic[i].authGroup.name.Length == 0)
                        authGroupName = "Administrator";
                    else
                        authGroupName = userInfoDic[i].authGroup.name;
                    string[] tempStrings = { userInfoDic[i].id, userInfoDic[i].userName, authGroupName };
                    ListViewItem item = new ListViewItem(tempStrings);
                    lv_UserList.Items.Add(item);
                }                
            }
        }

        private void lv_UserList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_UserList.HitTest(e.X, e.Y);
            if (info != null)
            {
                InitUserCkbs();
                var videoitem = info.Item as ListViewItem;
                UserInfo tempUserInfo = userInfoDic[videoitem.Index];
                tb_name.Text = tempUserInfo.userName;
                tb_psw.Text = tempUserInfo.password;
                tb_bms.Text = tempUserInfo.bindMacSwitch;
                tb_mac.Text = tempUserInfo.mac;
                tb_email.Text = tempUserInfo.email;
                tb_enabled.Text = tempUserInfo.enabled;
                tb_modifypsw.Text = tempUserInfo.modifyPassword;
                tb_comment.Text = tempUserInfo.comment;
                tb_authEff.Text = tempUserInfo.authEffective;
                
            }
        }
        public struct chlAuthStruct
        {
            public string id;
            public string name;
            public string auth;
        }
        public struct systemAuthStruct
        {
            public string localChlMgr;
            public string remoteChlMgr;
            public string diskMgr;
            public string talk;
            public string alarmMgr;
            public string net;
            public string rec;
            public string remoteLogin;
            public string scheduleMgr;
            public string localSysCfgAndMaintain;
            public string remoteSysCfgAndMaintain;
            public string securityMgr;
        }
        public struct AuthGroupStuctEx
        {
            public string id;
            public string name;
            public string isDefault;
            public string enableEdit;
            public chlAuthStruct[] cas;
            public systemAuthStruct sas;
        }
        private List<AuthGroupStuctEx> permissionList = new List<AuthGroupStuctEx>();
        private void btn_QueryGroupList_Click(object sender, EventArgs e)
        {
            InitChlCkbs();
            permissionList.Clear();
            string m_sendUrl = "queryAuthGroupList";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    XmlDocument oldXd = new XmlDocument();
                    oldXd.LoadXml(retStr);
                    XmlNodeList nodeList = oldXd.GetElementsByTagName("item");
                    if (nodeList.Count > 0)
                    {
                        XmlNode tempNode = nodeList[0];
                        while (tempNode != null)
                        {
                            AuthGroupStuctEx agse = new AuthGroupStuctEx();
                            systemAuthStruct sas = new systemAuthStruct();
                            chlAuthStruct[] cas = new chlAuthStruct[chlNum];
                            agse.id = tempNode.Attributes[0].InnerText;
                            agse.name = tempNode.ChildNodes[0].InnerText;
                            agse.isDefault = tempNode.ChildNodes[1].InnerText;
                            agse.enableEdit = tempNode.ChildNodes[2].InnerText;

                            for (int i = 0; i < tempNode.ChildNodes[3].ChildNodes.Count; i++)
                            {
                                cas[i].id = tempNode.ChildNodes[3].ChildNodes[i].Attributes[0].InnerText;
                                cas[i].name = tempNode.ChildNodes[3].ChildNodes[i].ChildNodes[0].InnerText;
                                cas[i].auth = tempNode.ChildNodes[3].ChildNodes[i].ChildNodes[1].InnerText;
                            }
                            agse.cas = cas;

                            sas.localChlMgr = tempNode.ChildNodes[4].ChildNodes[0].InnerText;
                            sas.remoteChlMgr = tempNode.ChildNodes[4].ChildNodes[1].InnerText;
                            sas.diskMgr = tempNode.ChildNodes[4].ChildNodes[2].InnerText;
                            sas.talk = tempNode.ChildNodes[4].ChildNodes[3].InnerText;
                            sas.alarmMgr = tempNode.ChildNodes[4].ChildNodes[4].InnerText;
                            sas.net = tempNode.ChildNodes[4].ChildNodes[5].InnerText;
                            sas.rec = tempNode.ChildNodes[4].ChildNodes[6].InnerText;
                            sas.remoteLogin = tempNode.ChildNodes[4].ChildNodes[7].InnerText;
                            sas.scheduleMgr = tempNode.ChildNodes[4].ChildNodes[8].InnerText;
                            sas.localSysCfgAndMaintain = tempNode.ChildNodes[4].ChildNodes[9].InnerText;
                            sas.remoteSysCfgAndMaintain = tempNode.ChildNodes[4].ChildNodes[10].InnerText;
                            sas.securityMgr = tempNode.ChildNodes[4].ChildNodes[11].InnerText;
                            agse.sas = sas;
                            permissionList.Add(agse);
                            tempNode = tempNode.NextSibling;
                        }
                        
                    }
                    UpdatePemissionGroupList();
                }
                Marshal.FreeHGlobal(lpOutBuffer);
                
            }
        }
        private void UpdatePemissionGroupList()
        {
            if (permissionList.Count > 0)
            {
                lv_PemissionList.Items.Clear();
                for (int i = 0; i < permissionList.Count; i++)
                {

                    string[] tempStrings = { permissionList[i].id, permissionList[i].name };
                    ListViewItem item = new ListViewItem(tempStrings);
                    lv_PemissionList.Items.Add(item);
                }         
            }
        }
        private int chlNum = 0;
        private void lv_PemissionList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_PemissionList.HitTest(e.X, e.Y);
            if (info != null)
            {
                InitGroupCkbs();
                var videoitem = info.Item as ListViewItem;
                AuthGroupStuctEx tempAuthGroupInfo = permissionList[videoitem.Index];
                tb_GroupName.Text = tempAuthGroupInfo.name;
                if (tempAuthGroupInfo.sas.localChlMgr == "true")
                    ckb_localChlMgr.Checked = true;
                if (tempAuthGroupInfo.sas.remoteChlMgr == "true")
                    ckb_remoteChlMgr.Checked = true;
                if (tempAuthGroupInfo.sas.diskMgr == "true")
                    ckb_diskMgr.Checked = true;
                if (tempAuthGroupInfo.sas.talk == "true")
                    ckb_talk.Checked = true;
                if (tempAuthGroupInfo.sas.alarmMgr == "true")
                    ckb_alarmMgr.Checked = true;
                if (tempAuthGroupInfo.sas.net == "true")
                    ckb_net.Checked = true;
                if (tempAuthGroupInfo.sas.rec == "true")
                    ckb_rec.Checked = true;
                if (tempAuthGroupInfo.sas.remoteLogin == "true")
                    ckb_remoteLogin.Checked = true;
                if (tempAuthGroupInfo.sas.scheduleMgr == "true")
                    ckb_scheduleMgr.Checked = true;
                if (tempAuthGroupInfo.sas.localSysCfgAndMaintain == "true")
                    ckb_localSysCfgAndMaintain.Checked = true;
                if (tempAuthGroupInfo.sas.remoteSysCfgAndMaintain == "true")
                    ckb_remoteSysCfgAndMaintain.Checked = true;
                if (tempAuthGroupInfo.sas.securityMgr == "true")
                    ckb_securityMgr.Checked = true;

                for (int i = 0; i < chlNum; i++)
                {
                    foreach (Control ct in tabPage12.Controls)
                    {
                        if (ct.GetType() == typeof(CheckBox))
                        {
                            if (ct.Name.EndsWith(tempAuthGroupInfo.cas[i].name + i))
                            {
                                if (ct.Name.StartsWith("cb1"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("@ad"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }                                        
                                }
                                if (ct.Name.StartsWith("cb2"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("@bk"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb3"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("@lp"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb4"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("@ptz"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb5"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("@spr"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                            }
                            
                        }
                            
                    }
                    foreach (Control ct in tabPage13.Controls)
                    {
                        if (ct.GetType() == typeof(CheckBox))
                        {
                            if (ct.Name.EndsWith(tempAuthGroupInfo.cas[i].name + i))
                            {                              
                                if (ct.Name.StartsWith("cb6"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("_ad"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb7"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("_bk"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb8"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("_lp"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb9"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("_ptz"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                                if (ct.Name.StartsWith("cb10"))
                                {
                                    if (tempAuthGroupInfo.cas[i].auth.Contains("_spr"))
                                    {
                                        ((CheckBox)ct).Checked = true;
                                        continue;
                                    }
                                }
                            }

                        }

                    }
                }
            }
        }
        private void InitUserCkbs()
        {
            
            foreach (Control ct in groupBox16.Controls)
            {
                if (ct.GetType() == typeof(CheckBox))
                    ((CheckBox)ct).Checked = false;
            }
            
        }
        private void InitGroupCkbs()
        {
            InitChlCkbs();
            foreach (Control ct in groupBox18.Controls)
            {
                if (ct.GetType() == typeof(CheckBox))
                    ((CheckBox)ct).Checked = false;
            }
            foreach (Control ct in tabPage12.Controls)
            {
                if (ct.GetType() == typeof(CheckBox))
                    ((CheckBox)ct).Checked = false;
            }
            foreach (Control ct in tabPage13.Controls)
            {
                if (ct.GetType() == typeof(CheckBox))
                    ((CheckBox)ct).Checked = false;
            }
        }

        private List<chlAuthStruct> chlList = new List<chlAuthStruct>();
        private void InitChlCkbs()
        {
            chlList.Clear();
            string m_sendUrl = "queryDevList";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'>
                                <requireField>
                                <name/><ip/><port/><userName/><password/><protocolType/><productModel/><chlIndex/><index/><chlType/><chlNum/>
                                </requireField></request>";
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    XmlDocument oldXd = new XmlDocument();
                    oldXd.LoadXml(retStr);
                    XmlNodeList nodeList = oldXd.GetElementsByTagName("item");
                    if (nodeList.Count > 0)
                    {
                        chlNum = nodeList.Count;
                        for(int i = 0;i< nodeList.Count;i++)
                        {
                            chlAuthStruct cas = new chlAuthStruct();
                            cas.id = nodeList[i].Attributes[0].InnerText;
                            cas.name = nodeList[i].ChildNodes[1].InnerText;
                            chlList.Add(cas);
                            Label lb1 = new Label();
                            Label lb2 = new Label();
                            CheckBox cb1 = new CheckBox();
                            CheckBox cb2 = new CheckBox();
                            CheckBox cb3 = new CheckBox();
                            CheckBox cb4 = new CheckBox();
                            CheckBox cb5 = new CheckBox();
                            CheckBox cb6 = new CheckBox();
                            CheckBox cb7 = new CheckBox();
                            CheckBox cb8 = new CheckBox();
                            CheckBox cb9 = new CheckBox();
                            CheckBox cb10 = new CheckBox();
                            cb1.Name = "cb1" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb2.Name = "cb2" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb3.Name = "cb3" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb4.Name = "cb4" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb5.Name = "cb5" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb6.Name = "cb6" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb7.Name = "cb7" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb8.Name = "cb8" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb9.Name = "cb9" + nodeList[i].ChildNodes[1].InnerText + i;
                            cb10.Name = "cb10" + nodeList[i].ChildNodes[1].InnerText + i;


                            lb1.Text = nodeList[i].ChildNodes[1].InnerText;
                            cb1.Text = "Preview";
                            cb2.Text = "Playback";
                            cb3.Text = "Backup";
                            cb4.Text = "Audio";
                            cb5.Text = "PTZ";
                            tabPage12.Controls.Add(cb1);
                            tabPage12.Controls.Add(cb2);
                            tabPage12.Controls.Add(cb3);
                            tabPage12.Controls.Add(cb4);
                            tabPage12.Controls.Add(cb5);
                            tabPage12.Controls.Add(lb1);
                            cb1.Top = i * 30 + 30;
                            cb1.Left = 50;
                            cb2.Top = i * 30 + 30;
                            cb2.Left = 50 + 110 * 1;
                            cb3.Top = i * 30 + 30;
                            cb3.Left = 50 + 110 * 2;
                            cb4.Top = i * 30 + 30;
                            cb4.Left = 50 + 110 * 3;
                            cb5.Top = i * 30 + 30;
                            cb5.Left = 50 + 110 * 4;
                            lb1.Top = i * 30 + 33;
                            lb2.Text = nodeList[i].ChildNodes[1].InnerText;
                            cb6.Text = "Preview";
                            cb7.Text = "Playback";
                            cb8.Text = "Backup";
                            cb9.Text = "Audio";
                            cb10.Text = "PTZ";
                            tabPage13.Controls.Add(cb6);
                            tabPage13.Controls.Add(cb7);
                            tabPage13.Controls.Add(cb8);
                            tabPage13.Controls.Add(cb9);
                            tabPage13.Controls.Add(cb10);
                            tabPage13.Controls.Add(lb2);
                            cb6.Top = i * 30 + 30;
                            cb6.Left = 50;
                            cb7.Top = i * 30 + 30;
                            cb7.Left = 50 + 110 * 1;
                            cb8.Top = i * 30 + 30;
                            cb8.Left = 50 + 110 * 2;
                            cb9.Top = i * 30 + 30;
                            cb9.Left = 50 + 110 * 3;
                            cb10.Top = i * 30 + 30;
                            cb10.Left = 50 + 110 * 4;
                            lb2.Top = i * 30 + 33;

                            
                        }
                        
                    }
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }

        private void btn_EditGroup_Click(object sender, EventArgs e)
        {
            if (lv_PemissionList.SelectedItems.Count < 1)
            {
                MessageBox.Show("Select User Group first !!");
                return;
            }
            int Index = lv_PemissionList.SelectedItems[0].Index;

            AuthGroupStuctEx tempAuthGroupInfo = permissionList[Index];
            if (tempAuthGroupInfo.name == "Administrator")
                return;

            string m_sendUrl = "editAuthGroup";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

            XmlDocument oldXd = new XmlDocument();
            oldXd.LoadXml(m_sendXml);

            var root = oldXd.DocumentElement;//取到根结点
            XmlNode newNode = oldXd.CreateNode("element", "content", "");

            root.AppendChild(newNode);


            XmlNode newNode1 = oldXd.CreateNode(XmlNodeType.Element, "id", "");
            newNode1.InnerText = tempAuthGroupInfo.id;
            newNode.AppendChild(newNode1);

            XmlElement tempElement = oldXd.CreateElement("name");
            XmlCDataSection cd = oldXd.CreateCDataSection(tempAuthGroupInfo.name);
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            tempElement = oldXd.CreateElement("chlAuthNote");
            string temp = @"local: [_lp:live preview, _spr: search and play record, _bk:backup,_ad:audio, _ptz:PTZ control],remote: [@lp:live preview, @spr: search and play record, @bk:backup,@ad:audio, @ptz: PTZ control]";
            cd = oldXd.CreateCDataSection(temp);
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "chlAuth", "");           
            newNode.AppendChild(newNode1);
            tempElement = (XmlElement)newNode1;
            tempElement.SetAttribute("type", "list");

            XmlNode newNode2 = oldXd.CreateNode(XmlNodeType.Element, "itemType", "");
            XmlNode newNode3 = oldXd.CreateNode(XmlNodeType.Element, "name", "");
            XmlNode newNode4 = oldXd.CreateNode(XmlNodeType.Element, "auth", "");
            newNode2.AppendChild(newNode3);
            newNode2.AppendChild(newNode4);
            newNode1.AppendChild(newNode2);

            for (int i = 0; i < tempAuthGroupInfo.cas.Length; i++)
            {
                newNode2 = oldXd.CreateNode(XmlNodeType.Element, "item", "");
                ((XmlElement)newNode2).SetAttribute("id", tempAuthGroupInfo.cas[i].id);

                tempElement = oldXd.CreateElement("name");
                cd = oldXd.CreateCDataSection(tempAuthGroupInfo.cas[i].name);
                tempElement.AppendChild(cd);
                newNode2.AppendChild(tempElement);

                StringBuilder auth = new StringBuilder();
#region 

                foreach (Control ct in tabPage12.Controls)
                {
                    if (ct.GetType() == typeof(CheckBox))
                    {
                        if (ct.Name.EndsWith(tempAuthGroupInfo.cas[i].name + i))
                        {
                            if (ct.Name.StartsWith("cb1"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@ad,");
                                    continue;
                                }                                
                            }
                            if (ct.Name.StartsWith("cb2"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@bk,");
                                    continue;
                                }                                   
                            }
                            if (ct.Name.StartsWith("cb3"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@lp,");
                                    continue;
                                } 
                            }
                            if (ct.Name.StartsWith("cb4"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@ptz,");
                                    continue;
                                }
                                
                            }
                            if (ct.Name.StartsWith("cb5"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@spr,");
                                    continue;
                                }
                            }
                        }

                    }

                }
                foreach (Control ct in tabPage13.Controls)
                {
                    if (ct.GetType() == typeof(CheckBox))
                    {
                        if (ct.Name.EndsWith(tempAuthGroupInfo.cas[i].name + i))
                        {
                            if (ct.Name.StartsWith("cb6"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_ad,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb7"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_bk,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb8"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_lp,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb9"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_ptz,");
                                    continue;
                                }

                            }
                            if (ct.Name.StartsWith("cb10"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_spr,");
                                    continue;
                                }
                            }
                        }

                    }

                }
#endregion
                if(auth.Length > 0)
                    auth = auth.Remove(auth.Length - 1, 1);
                tempElement = oldXd.CreateElement("auth");
                cd = oldXd.CreateCDataSection(auth.ToString());
                tempElement.AppendChild(cd);
                newNode2.AppendChild(tempElement);

                newNode1.AppendChild(newNode2);
            }
            newNode.AppendChild(newNode1);

            newNode3 = oldXd.CreateNode(XmlNodeType.Element, "systemAuth", "");
            string tempSystemAuth = "false";
            if (ckb_localChlMgr.Checked)           
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "localChlMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);


            tempSystemAuth = "false";
            if (ckb_remoteChlMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "remoteChlMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);


            tempSystemAuth = "false";
            if (ckb_remoteLogin.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "remoteLogin", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_diskMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "diskMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_talk.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "talk", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_alarmMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "alarmMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_net.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "net", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_scheduleMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "scheduleMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_rec.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "rec", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_localSysCfgAndMaintain.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "localSysCfgAndMaintain", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_remoteSysCfgAndMaintain.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "remoteSysCfgAndMaintain", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_securityMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "securityMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            newNode.AppendChild(newNode3);
            m_sendXml = oldXd.InnerXml;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    if (retStr.Contains("success"))
                    {
                        btn_QueryGroupList_Click(null,null);
                        MessageBox.Show("Edit User Group success");
                    }
                        
                    else
                        MessageBox.Show("Edit User Group failed");
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }

        private void btn_DelGroup_Click(object sender, EventArgs e)
        {
            if (lv_PemissionList.SelectedItems.Count < 1)
            {
                MessageBox.Show("Select User Group first !!");
                return;
            }
            int Index = lv_PemissionList.SelectedItems[0].Index;

            AuthGroupStuctEx tempAuthGroupInfo = permissionList[Index];
            if (Index < 3)
                return;

            string m_sendUrl = "delAuthGroup";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

            XmlDocument oldXd = new XmlDocument();
            oldXd.LoadXml(m_sendXml);
            var root = oldXd.DocumentElement;//
            XmlNode newNode = oldXd.CreateNode("element", "condition", "");
            root.AppendChild(newNode);

            XmlNode newNode1 = oldXd.CreateNode("element", "authGroupIds", "");
            ((XmlElement)newNode1).SetAttribute("type", "list");
            newNode.AppendChild(newNode1);



            XmlElement tempElement = oldXd.CreateElement("item");
            XmlCDataSection cd = oldXd.CreateCDataSection(tempAuthGroupInfo.name);
            tempElement.AppendChild(cd);
            tempElement.SetAttribute("id", tempAuthGroupInfo.id);
            newNode1.AppendChild(tempElement);

            m_sendXml = oldXd.InnerXml;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    if (retStr.Contains("success"))
                    {
                        btn_QueryGroupList_Click(null, null);
                        MessageBox.Show("Delete User Group success");
                    }
                    else
                        MessageBox.Show("Delete User Group failed");
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
            
        }

        private void btn_AddGroup_Click(object sender, EventArgs e)
        {
            string groupName = tb_GroupName.Text.Trim();

            string m_sendUrl = "createAuthGroup";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

            XmlDocument oldXd = new XmlDocument();
            oldXd.LoadXml(m_sendXml);

            var root = oldXd.DocumentElement;//取到根结点
            XmlNode newNode = oldXd.CreateNode("element", "content", "");

            root.AppendChild(newNode);

            XmlElement tempElement = oldXd.CreateElement("name");
            XmlCDataSection cd = oldXd.CreateCDataSection(groupName);
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            tempElement = oldXd.CreateElement("chlAuthNote");
            string temp = @"local: [_lp:live preview, _spr: search and play record, _bk:backup,_ad:audio, _ptz:PTZ control],remote: [@lp:live preview, @spr: search and play record, @bk:backup,@ad:audio, @ptz: PTZ control]";
            cd = oldXd.CreateCDataSection(temp);
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            XmlNode newNode1 = oldXd.CreateNode(XmlNodeType.Element, "chlAuth", "");
            newNode.AppendChild(newNode1);
            tempElement = (XmlElement)newNode1;
            tempElement.SetAttribute("type", "list");

            XmlNode newNode2 = oldXd.CreateNode(XmlNodeType.Element, "itemType", "");
            XmlNode newNode3 = oldXd.CreateNode(XmlNodeType.Element, "name", "");
            XmlNode newNode4 = oldXd.CreateNode(XmlNodeType.Element, "auth", "");
            newNode2.AppendChild(newNode3);
            newNode2.AppendChild(newNode4);
            newNode1.AppendChild(newNode2);

            for (int i = 0; i < chlList.Count; i++)
            {
                //newNode2 = oldXd.CreateNode(XmlNodeType.Element, "item", "");
                //((XmlElement)newNode2).SetAttribute("id", chlList[i].id);

                StringBuilder auth = new StringBuilder();
                #region

                foreach (Control ct in tabPage12.Controls)
                {
                    if (ct.GetType() == typeof(CheckBox))
                    {
                        if (ct.Name.EndsWith(chlList[i].name + i))
                        {
                            if (ct.Name.StartsWith("cb1"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@ad,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb2"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@bk,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb3"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@lp,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb4"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@ptz,");
                                    continue;
                                }

                            }
                            if (ct.Name.StartsWith("cb5"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("@spr,");
                                    continue;
                                }
                            }
                        }

                    }

                }
                foreach (Control ct in tabPage13.Controls)
                {
                    if (ct.GetType() == typeof(CheckBox))
                    {
                        if (ct.Name.EndsWith(chlList[i].name + i))
                        {
                            if (ct.Name.StartsWith("cb6"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_ad,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb7"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_bk,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb8"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_lp,");
                                    continue;
                                }
                            }
                            if (ct.Name.StartsWith("cb9"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_ptz,");
                                    continue;
                                }

                            }
                            if (ct.Name.StartsWith("cb10"))
                            {
                                if (((CheckBox)ct).Checked)
                                {
                                    auth.Append("_spr,");
                                    continue;
                                }
                            }
                        }

                    }

                }
                #endregion
                if (auth.Length > 0)
                    auth = auth.Remove(auth.Length - 1, 1);


                tempElement = oldXd.CreateElement("item");
                cd = oldXd.CreateCDataSection(auth.ToString());
                tempElement.AppendChild(cd);
                tempElement.SetAttribute("id", chlList[i].id);

                newNode1.AppendChild(tempElement);
            }
            newNode.AppendChild(newNode1);

            newNode3 = oldXd.CreateNode(XmlNodeType.Element, "systemAuth", "");
            string tempSystemAuth = "false";
            if (ckb_localChlMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "localChlMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);


            tempSystemAuth = "false";
            if (ckb_remoteChlMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "remoteChlMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);


            tempSystemAuth = "false";
            if (ckb_remoteLogin.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "remoteLogin", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_diskMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "diskMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_talk.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "talk", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_alarmMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "alarmMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_net.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "net", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_scheduleMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "scheduleMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_rec.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "rec", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_localSysCfgAndMaintain.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "localSysCfgAndMaintain", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_remoteSysCfgAndMaintain.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "remoteSysCfgAndMaintain", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            tempSystemAuth = "false";
            if (ckb_securityMgr.Checked)
                tempSystemAuth = "true";
            newNode4 = oldXd.CreateNode(XmlNodeType.Element, "securityMgr", "");
            newNode4.InnerText = tempSystemAuth;
            newNode3.AppendChild(newNode4);

            newNode.AppendChild(newNode3);
            m_sendXml = oldXd.InnerXml;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    if (retStr.Contains("success"))
                    {
                        btn_QueryGroupList_Click(null, null);
                        MessageBox.Show("Add User Group success");
                    }

                    else
                        MessageBox.Show("Add User Group failed");
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }

        private void btn_EditUser_Click(object sender, EventArgs e)
        {
            if (lv_UserList.SelectedItems.Count < 1)
            {
                MessageBox.Show("Select User first !!");
                return;
            }
            int Index = lv_UserList.SelectedItems[0].Index;

            UserInfo tempUserInfo = userInfoDic[Index];
            if (Index < 1)
                return;

            string m_sendUrl = "editUser";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

            XmlDocument oldXd = new XmlDocument();
            oldXd.LoadXml(m_sendXml);

            var root = oldXd.DocumentElement;//
            XmlNode newNode = oldXd.CreateNode("element", "content", "");
            root.AppendChild(newNode);


            XmlNode newNode1 = oldXd.CreateNode(XmlNodeType.Element, "userId", "");
            newNode1.InnerText = tempUserInfo.id;
            newNode.AppendChild(newNode1);

            XmlElement tempElement = oldXd.CreateElement("userName");
            XmlCDataSection cd = oldXd.CreateCDataSection(tempUserInfo.userName);
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);            
            
            tempElement = oldXd.CreateElement("authGroup");
            if (lv_PemissionList.SelectedItems.Count < 1)
            {
                tempElement.SetAttribute("id", tempUserInfo.authGroup.id);
            }
            else
            {
                Index = lv_PemissionList.SelectedItems[0].Index;
                tempElement.SetAttribute("id", permissionList[Index].id);               
            }
            newNode.AppendChild(tempElement);


            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "bindMacSwitch", "");
            string tempStr = tb_bms.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = tempUserInfo.bindMacSwitch;
            newNode.AppendChild(newNode1);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "modifyPassword", "");
            tempStr = tb_modifypsw.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = tempUserInfo.modifyPassword;
            newNode.AppendChild(newNode1);

            tempElement = oldXd.CreateElement("mac");
            cd = oldXd.CreateCDataSection(tb_mac.Text.Trim());
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            tempElement = oldXd.CreateElement("email");
            cd = oldXd.CreateCDataSection(tb_email.Text.Trim());
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "enabled", "");
            tempStr = tb_enabled.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = tempUserInfo.enabled;
            newNode.AppendChild(newNode1);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "authEffective", "");
            tempStr = tb_authEff.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = tempUserInfo.authEffective;
            newNode.AppendChild(newNode1);

            m_sendXml = oldXd.InnerXml;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    if (retStr.Contains("success"))
                    {
                        btn_QueryUserList_Click(null, null);
                        MessageBox.Show("Edit User success");
                    }

                    else
                        MessageBox.Show("Edit User failed");
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
   
        }

        private void btn_DeleteUser_Click(object sender, EventArgs e)
        {
            if (lv_UserList.SelectedItems.Count < 1)
            {
                MessageBox.Show("Select User first !!");
                return;
            }
            int Index = lv_UserList.SelectedItems[0].Index;

            UserInfo tempUserInfo = userInfoDic[Index];
            if (Index < 1)
                return;

            string m_sendUrl = "delUser";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

            XmlDocument oldXd = new XmlDocument();
            oldXd.LoadXml(m_sendXml);

            var root = oldXd.DocumentElement;//
            XmlNode newNode = oldXd.CreateNode("element", "condition", "");
            root.AppendChild(newNode);

            XmlNode newNode1 = oldXd.CreateNode(XmlNodeType.Element, "userIds", "");
            ((XmlElement)newNode1).SetAttribute("type", "list");
            newNode.AppendChild(newNode1);

            XmlElement tempElement = oldXd.CreateElement("item");
            XmlCDataSection cd = oldXd.CreateCDataSection(tempUserInfo.userName);
            tempElement.AppendChild(cd);
            tempElement.SetAttribute("id", tempUserInfo.id);
            newNode1.AppendChild(tempElement);

            m_sendXml = oldXd.InnerXml;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    if (retStr.Contains("success"))
                    {
                        btn_QueryUserList_Click(null, null);
                        MessageBox.Show("Delete User success");
                    }

                    else
                        MessageBox.Show("Delete User failed");
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);

        }

        private void btn_AddUser_Click(object sender, EventArgs e)
        {
            if (permissionList.Count < 1)
                return;
            string m_sendUrl = "createUser";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

            XmlDocument oldXd = new XmlDocument();
            oldXd.LoadXml(m_sendXml);

            var root = oldXd.DocumentElement;//
            XmlNode newNode = oldXd.CreateNode("element", "content", "");
            root.AppendChild(newNode);

            XmlElement tempElement = oldXd.CreateElement("userName");
            XmlCDataSection cd = oldXd.CreateCDataSection(tb_name.Text.Trim());
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            tempElement = oldXd.CreateElement("password");
            cd = oldXd.CreateCDataSection(CommonMethods.GenerateMD5(tb_psw.Text.Trim()).ToUpper());
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            tempElement = oldXd.CreateElement("email");
            cd = oldXd.CreateCDataSection(tb_email.Text.Trim());
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            XmlNode newNode1 = oldXd.CreateNode(XmlNodeType.Element, "modifyPassword", "");
            string tempStr = tb_modifypsw.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = "false";
            newNode.AppendChild(newNode1);



            tempElement = oldXd.CreateElement("authGroupId");
            if (lv_PemissionList.SelectedItems.Count < 1)
            {
                cd = oldXd.CreateCDataSection(permissionList[0].id);
                
            }
            else
            {
                int Index = lv_PemissionList.SelectedItems[0].Index;
                cd = oldXd.CreateCDataSection(permissionList[Index].id);
            }
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "bindMacSwitch", "");
            tempStr = tb_bms.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = "false";
            newNode.AppendChild(newNode1);

            tempElement = oldXd.CreateElement("mac");
            cd = oldXd.CreateCDataSection(tb_mac.Text.Trim());
            tempElement.AppendChild(cd);
            newNode.AppendChild(tempElement);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "enabled", "");
            tempStr = tb_enabled.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = "false";
            newNode.AppendChild(newNode1);

            newNode1 = oldXd.CreateNode(XmlNodeType.Element, "authEffective", "");
            tempStr = tb_authEff.Text.Trim();
            if (tempStr == "true" || tempStr == "false")
                newNode1.InnerText = tempStr;
            else
                newNode1.InnerText = "true";
            newNode.AppendChild(newNode1);

            XmlNode authNode = oldXd.CreateNode("element", "auth", "");
            root.AppendChild(authNode);

            tempElement = oldXd.CreateElement("userName");
            tempElement.InnerText = usr;
            authNode.AppendChild(tempElement);

            tempElement = oldXd.CreateElement("password");

            IntPtr outS = Marshal.AllocHGlobal(64);
            int a = 0;
            DevSdkHelper.NET_SDK_GetSha1Encrypt(userId, psw, psw.Length, outS, 64, ref a);
            byte[] efg = new byte[40];
            Marshal.Copy(outS, efg, 0, 40);
            Marshal.FreeHGlobal(outS);
            string tempenc = DevSdkHelper.ByteToStr(efg);


#region zhushi
            //string tempenc = CommonMethods.GenerateMD5(psw);
            //tempenc = CommonMethods.Sha1(tempenc);
#endregion


            tempElement.InnerText = tempenc;
            authNode.AppendChild(tempElement);



            m_sendXml = oldXd.InnerXml;
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string retStr = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                if (retStr.Length > 0)
                {
                    if (retStr.Contains("success"))
                    {
                        btn_QueryUserList_Click(null, null);
                        MessageBox.Show("Add User success");
                    }

                    else
                        MessageBox.Show("Add User failed");
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }

        private void addDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Program.login_Dialog.Show();
        }

        private void cmb_Device_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CloseAllLive();
            //userId = Program.m_deviceDic[cmb_Device.SelectedItem.ToString()].userId;
            //oNET_SDK_DEVICEINFO = Program.m_deviceDic[cmb_Device.SelectedItem.ToString()].devInfo;
            //ItemDataInit();
            //RefreshDeviceChls();
            //foreach (Control ct in tabPage1.Controls)
            //{
            //    if (ct.GetType() == typeof(Panel))
            //    {
            //        ct.Paint += panel_Paint;
            //        ct.MouseClick += panel_Click;
            //        if (!theStatus.ContainsKey(ct.Handle))
            //            theStatus.Add(ct.Handle, initItemStatus);
            //        else
            //            theStatus[ct.Handle] = initItemStatus;
            //    }
            //}
            //ContextMenuInit();
            //ManualRecStatusInit(oNET_SDK_DEVICEINFO);
            //cmb_Channel.SelectedItem = cmb_Channel.Items[0];
            //cmb_StreamType.SelectedItem = cmb_StreamType.Items[0];
            //cmb_Channel.SelectedIndexChanged += cmb_Channel_SelectedIndexChanged;
            //cmb_StreamType.SelectedIndexChanged += cmb_StreamType_SelectedIndexChanged;

            //currentPanelHandle = panel1.Handle;
            //lastPanelHandle = panel1.Handle;

            //ConfigMenuInit();

            //PtzUIInit();
            //if (!LogTypeInited)
            //{
            //    LogTypeInit();
            //    LogTypeInited = true;
            //}

            //DateTime now = DateTime.Now;
            //DateTime tempDt = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            //dtp_start.Value = tempDt;

            //nmc = fNET_MESSAGE_CALLBACK;
            //bool ret = DevSdkHelper.NET_SDK_SetDVRMessageCallBackEx(nmc, IntPtr.Zero);

            //FaceMatchInit();
            //IVMinit();
        }

        private void viewAlarmLogVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string monthPart = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString();
                string dayPart = DateTime.Now.Day > 9 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString();
                string timePart = DateTime.Now.Year.ToString() + "-" + monthPart + "-" + dayPart;
                string dataPath = Application.StartupPath + "\\"+ timePart + " " + "AlarmLog" + ".log";

                System.Diagnostics.Process.Start(dataPath);
            }
            catch (System.Exception ex)
            {
            	
            }
            
        }

        private void viewExceptionLogEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string monthPart = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString();
                string dayPart = DateTime.Now.Day > 9 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString();
                string timePart = DateTime.Now.Year.ToString() + "-" + monthPart + "-" + dayPart;
                string dataPath = Application.StartupPath + "\\" + timePart + " " + "ExceptionLog" + ".log";

                System.Diagnostics.Process.Start(dataPath);
            }
            catch (System.Exception ex)
            {

            }
        }

        private void btn_Logout_Click(object sender, EventArgs e)
        {

        }

        private void btn_Live_16windows_Click(object sender, EventArgs e)
        {
            if (_16Index > 15)
            {
                return;
            }
            if (_16PlayHandles[_16Index] > 0)
            {
                bool stopPlay = DevSdkHelper.NET_SDK_StopLivePlay(_16PlayHandles[_16Index]);
                if (stopPlay)
                {
                    _16PlayHandles[_16Index] = -1;
                }
            }
            NET_SDK_CLIENTINFO clientInfo = new NET_SDK_CLIENTINFO();
            clientInfo.lChannel = cmb_Channel_16windows.SelectedIndex;
            clientInfo.hPlayWnd = _16PanelHandles[_16Index];
            clientInfo.streamType = /*cmb_StreamType_16windows.SelectedIndex*/1;


            _16PlayHandles[_16Index] = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, null, IntPtr.Zero);
            yuvldc = YuvLiveDataCallBack;
            DevSdkHelper.NET_SDK_SetYUVCallBack(_16PlayHandles[_16Index], yuvldc, IntPtr.Zero);
            if (_16PlayHandles[_16Index] > 0)
            {
                _16Index++;
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Thread aa = new Thread(closeLiveplay);
            aa.IsBackground = true;
            aa.Start();
        }
        private void closeLiveplay()
        {
            for (int i = 0; i < 16; i++)
            {
                if (_16PlayHandles[i] > 0)
                {
                    bool stopPlay = DevSdkHelper.NET_SDK_StopLivePlay(_16PlayHandles[i]);
                    if (stopPlay)
                    {
                        _16PlayHandles[i] = -1;
                        Invoke(new Action(() =>
                        {
                            Control.FromHandle(_16PanelHandles[i]).BackColor = Color.Transparent;
                            Control.FromHandle(_16PanelHandles[i]).BackColor = Color.Gray;
                        }));
                        Thread.Sleep(500);
                    }
                }

            }
            _16Index = 0;
        }
        private void AddTreeRootNode(object obj)
        {
            treeView1.Nodes.Add((TreeNode)obj);
            treeView1.ExpandAll();
        }
        private void AddTreeChildNode(object obj)
        {
            treeView1.SelectedNode.Nodes.Add((TreeNode)obj);
            treeView1.ExpandAll();
        }
        private void SetSelectedNode(object obj)
        {
            treeView1.SelectedNode = (TreeNode)obj;
        }
        private TreeNode FindNode(TreeNode tnParent, string strValue)
        {
            if (tnParent == null) return null;
            if (tnParent.Text == strValue)
                return tnParent;
            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                tnRet = FindNode(tn, strValue);
                if (tnRet != null)
                    break;
            }
            return tnRet;
        }
        private Dictionary<TreeNode, ChannelInfo> m_dicItem = new Dictionary<TreeNode, ChannelInfo>();
        public struct ChannelInfo
        {
            public int userId;
            public int chlIndex;
        }
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(m_dicItem.ContainsKey(e.Node))
            {
                ChannelInfo tempChlInfo = m_dicItem[e.Node];

                if (_16Index > 15)
                {
                    return;
                }
                if (_16PlayHandles[_16Index] > 0)
                {
                    bool stopPlay = DevSdkHelper.NET_SDK_StopLivePlay(_16PlayHandles[_16Index]);
                    if (stopPlay)
                    {
                        _16PlayHandles[_16Index] = -1;
                    }
                }
                NET_SDK_CLIENTINFO clientInfo = new NET_SDK_CLIENTINFO();
                clientInfo.lChannel = tempChlInfo.chlIndex;
                clientInfo.hPlayWnd = _16PanelHandles[_16Index];
                clientInfo.streamType = /*cmb_StreamType_16windows.SelectedIndex*/0;


                _16PlayHandles[_16Index] = DevSdkHelper.NET_SDK_LivePlay(tempChlInfo.userId, ref clientInfo, null, IntPtr.Zero);
                if (_16PlayHandles[_16Index] > 0)
                {
                    _16Index++;
                    yuvldc = YuvLiveDataCallBack;
                    DevSdkHelper.NET_SDK_SetYUVCallBack(_16PlayHandles[_16Index], yuvldc, IntPtr.Zero);
                }

            }          
        }
        private void SmartSearchInit()
        {
            foreach (Control ct in gb_SmartSearchType.Controls)
            {
                if (ct.GetType() == typeof(RadioButton))
                {
                    ct.Click += rb_SEARCH_TYPE_Click;
                }
            }

            rb_Face.Checked = true;

        }
        private void rb_SEARCH_TYPE_Click(object sender, EventArgs e)
        {
            RadioButton rdb = (RadioButton)sender;
            string tempStr = rdb.Text;
            foreach (Control ct in gb_SmartSearchType.Controls)
            {
                if (ct.GetType() == typeof(GroupBox))
                {
                    if (ct.Text == tempStr)
                        ct.Visible = true;
                    else
                        ct.Visible = false;
                }
            }
        }
        private List<NET_SDK_FACE_IMG_INFO_CH> m_snapList = new List<NET_SDK_FACE_IMG_INFO_CH>();
        private void btn_SmartSearch_Click(object sender, EventArgs e)
        {
            NET_SDK_SEARCH_IMAGE_BY_IMAGE_V2 nssibiv = new NET_SDK_SEARCH_IMAGE_BY_IMAGE_V2();

            nssibiv.limitNum = 10;

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

            nssibiv.similarity = 75;


            /************图片来源是目标库***********/
            //nssibiv.imgSourceType = DevSdkHelper.FROM_ALBUM;
            //nssibiv.imgNum = 1;

            //int[] imgids = new int[nssibiv.imgNum];
            //imgids[0] = 7;//目标图在目标库中的序号
            //IntPtr pImgids = Marshal.AllocHGlobal(4 * 1);
            //Marshal.Copy(imgids, 0, pImgids, 1);

            //nssibiv.imgId = pImgids;
            //nssibiv.searchType = DevSdkHelper.SEARCH_IMAGE_BY_IMAGE;
            /***********************************/


            /************图片来源是外部图片***********/
            //NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceImgData sImgData = new NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceImgData();
            //MemoryStream ms = new MemoryStream();
            //if (pictureBox9.Image == null)
            //    return;
            //pictureBox9.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //byte[] array = ms.ToArray();

            //sImgData.imgData = Marshal.AllocHGlobal(array.Length);
            //Marshal.Copy(array, 0, sImgData.imgData, array.Length);
            //sImgData.imgWidth = 256;    //timg.jpg的宽 //图片必须是jpg的，高宽可以不传
            //sImgData.imgHeight = 256;   //timg.jpg的高
            //sImgData.imgLen = (UInt32)array.Length;

            //nssibiv.sfaceImgData = sImgData;
            //nssibiv.sfaceImgsNum = 1;
            //nssibiv.imgSourceType = DevSdkHelper.FROM_EXTERNAL;
            //nssibiv.searchType = DevSdkHelper.SEARCH_IMAGE_BY_IMAGE;
            /***********************************/
            if (rb_Face.Checked)
            {
                nssibiv.searchType = DevSdkHelper.SEARCH_IMAGE_BY_EVENT;
                if (rb_Search_All.Checked)
                    nssibiv.eventType = DevSdkHelper.BY_ALL;
                else if (rb_Search_AllowList.Checked)
                    nssibiv.eventType = DevSdkHelper.BY_WHITELIST;
                else if (rb_Search_Stranger.Checked)
                    nssibiv.eventType = DevSdkHelper.BY_STRANGERLIST;
                else if (rb_Search_BlockList.Checked)
                    nssibiv.eventType = DevSdkHelper.BY_BLACKLIST;
            }


            uint lpBytesReturned = 0;
            int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_IMAGE_BY_IMG_V2;

            int nSize = Marshal.SizeOf(nssibiv);
            IntPtr inputPtr = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(nssibiv, inputPtr, true);

            
            IntPtr outputPtr = Marshal.AllocHGlobal(100*1024);
            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, inputPtr, (uint)nSize, outputPtr, 100 * 1024, ref lpBytesReturned);
            if (ret)
            {
                NET_SDK_CH_SNAP_FACE_IMG_LIST Img_List_Out = new NET_SDK_CH_SNAP_FACE_IMG_LIST();
                int nSizeofImgListOut = Marshal.SizeOf(Img_List_Out);
                Img_List_Out = (NET_SDK_CH_SNAP_FACE_IMG_LIST)Marshal.PtrToStructure(outputPtr, typeof(NET_SDK_CH_SNAP_FACE_IMG_LIST));
                if (Img_List_Out.listNum > 0)
                {
                    m_snapList.Clear();
                    NET_SDK_FACE_IMG_INFO_CH Img_info = new NET_SDK_FACE_IMG_INFO_CH();
                    int nSizeofImgInfo = Marshal.SizeOf(Img_info);
                    for (int i = 0; i < Img_List_Out.listNum; ++i)
                    {
                        Img_info = (NET_SDK_FACE_IMG_INFO_CH)Marshal.PtrToStructure(outputPtr + nSizeofImgListOut + i * nSizeofImgInfo, typeof(NET_SDK_FACE_IMG_INFO_CH));
                        m_snapList.Add(Img_info);
                    }
                    UpdateSnapList();

                }
                

            }
            //Marshal.FreeHGlobal(pImgids);
            Marshal.FreeHGlobal(inputPtr);
            Marshal.FreeHGlobal(outputPtr);
        }
        private void UpdateSnapList()
        {
            lv_ChSnapList.Items.Clear();
            for (int i = 0; i < m_snapList.Count; i++)
            {
                ListViewItem item = new ListViewItem(ChangeToItems(m_snapList[i], i + 1));
                lv_ChSnapList.Items.Add(item);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //FtpHelper fh = new FtpHelper(tb_ftpServerIp.Text.Trim(), tb_ftpPath.Text.Trim(), tb_ftpUserName.Text.Trim(), tb_ftpPsw.Text.Trim());
            //MessageBox.Show(fh.Upload("C:\\test.txt"));

            //IntPtr outS = Marshal.AllocHGlobal(64);
            //DevSdkHelper.NET_SDK_GetSha1Encrypt(userId, "123456", 6, outS);
            //byte[] efg = new byte[64];
            //Marshal.Copy(outS, efg, 0, 64);
            //string abc = Encoding.UTF8.GetString(efg);
            //MessageBox.Show(abc);
            //Marshal.FreeHGlobal(outS);
            //DevSdkHelper.NET_SDK_SetLogToFile(true, "D:\\A文档", false, 5);
            NET_SDK_DEV_SUPPORT sup = new NET_SDK_DEV_SUPPORT();
            DevSdkHelper.NET_SDK_GetDeviceSupportFunction(userId, ref sup);
            int a = 0;
        }

        private void btn_smart_subscrib_Vehicle_Click(object sender, EventArgs e)
        {
            bool ret = false;
            //int chn = Convert.ToInt16(cmb_IVM_Chl.SelectedItem.ToString());
            if (!m_SubscribVehicle)
            {
                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib = new NET_DVR_SUBSCRIBE_REPLY();

                //int nSizeOfReply = Marshal.SizeOf(sSmartSubscrib);
                //IntPtr ptrOfReply = Marshal.AllocHGlobal(nSizeOfReply);
                //Marshal.StructureToPtr(sSmartSubscrib, ptrOfReply, true);

                ret = DevSdkHelper.NET_SDK_SmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_IPC_SMART_VIHICLE, 0, ref sSmartSubscrib);
                if (ret)
                {
                    //sSmartSubscrib = (NET_DVR_SUBSCRIBE_REPLY)Marshal.PtrToStructure(ptrOfReply, typeof(NET_DVR_SUBSCRIBE_REPLY));
                    m_serverAddressAVD = sSmartSubscrib.serverAddress;
                    m_SubscribVehicle = true;
                    button7.Text = "UnSubscribVehicle";
                    button7.FlatStyle = FlatStyle.Popup;
                }
                else
                    ShowErrorMessage();
                //Marshal.FreeHGlobal(ptrOfReply);
            }
            else
            {
                int dwResult = 0;
                IntPtr temp = PointArrayToIntPtr(m_serverAddressAVD);
                bool bret = DevSdkHelper.NET_SDK_UnSmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_IPC_SMART_VIHICLE, 0, temp, ref dwResult);
                Marshal.FreeHGlobal(temp);
                if (bret)
                {
                    m_SubscribVehicle = false;
                    button7.Text = "SubscribVehicle";
                    button7.FlatStyle = FlatStyle.Standard;
                }
                else
                    ShowErrorMessage();
            }
        }

        private void btn_GetTemperatureConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportThermometry;  
            if((oDevSupport.support & (1 << pos)) >> pos == 1)
            {

                string m_sendUrl = "GetTakeTemperatureConfig";
                string m_sendXml = "";
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    string highThreshold = "";
                    string lowThreshold = "";
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);

                    if (CommonMethods.GetNodeValue(retStr, "takeEanble").ToLower() == "true")                   
                        cb_temperatureEnable.Checked = true;                  
                    else
                        cb_temperatureEnable.Checked = false;
                    if (CommonMethods.GetNodeValue(retStr, "tempUnits").ToLower() == "centigrade")
                    {
                        highThreshold = "highThreshold";
                        lowThreshold = "lowThreshold";
                        cmb_Unit.SelectedIndex = 0;
                    }                       
                    else
                    {
                        highThreshold = "FhighThreshold";
                        lowThreshold = "FlowThreshold";
                        cmb_Unit.SelectedIndex = 1;
                    }

                    XmlNode tempnode = CommonMethods.GetNodePosByIndex(retStr, highThreshold);
                    string temp = tempnode.OuterXml;
                    if (CommonMethods.GetNodeValue(temp, "switch").ToLower() == "true")
                    {
                        cb_TooHighAlarmEnable.Checked = true;
                    }
                    else
                    {
                        cb_TooHighAlarmEnable.Checked = false;
                    }
                    string temperature = CommonMethods.GetNodeValue(temp, "value");
                    tempnode = CommonMethods.GetNodePosByIndex(temp, "value");
                    string temperatureLL = CommonMethods.GetNodeAttribute(tempnode, "min");
                    string temperatureHL = CommonMethods.GetNodeAttribute(tempnode, "max");

                    temperatureHmin = int.Parse(temperatureLL) / 100;
                    temperatureHmax = int.Parse(temperatureHL) / 100;

                    tb_HighLimit.Text = (float.Parse(temperature) / 100).ToString();

                    if (CommonMethods.GetNodeValue(retStr, "tempUnits").ToLower() == "centigrade")
                    {
                        label99.Text = "(" + temperatureHmin.ToString() + "-" + temperatureHmax.ToString() + "）℃";
                    }
                    else
                    {
                        label99.Text = "(" + temperatureHmin.ToString() + "-" + temperatureHmax.ToString() + "）℉ ";
                    }
                    


                    tempnode = CommonMethods.GetNodePosByIndex(retStr, lowThreshold);
                    temp = tempnode.OuterXml;
                    if (CommonMethods.GetNodeValue(temp, "switch").ToLower() == "true")
                    {
                        cb_TooLowAlarmEnable.Checked = true;
                    }
                    else
                    {
                        cb_TooLowAlarmEnable.Checked = false;
                    }
                    temperature = CommonMethods.GetNodeValue(temp, "value");
                    tempnode = CommonMethods.GetNodePosByIndex(temp, "value");
                    temperatureLL = CommonMethods.GetNodeAttribute(tempnode, "min");
                    temperatureHL = CommonMethods.GetNodeAttribute(tempnode, "max");

                    temperatureLmin = int.Parse(temperatureLL) / 100;
                    temperatureLmax = int.Parse(temperatureHL) / 100;

                    tb_LowLimit.Text = (float.Parse(temperature) / 100).ToString();

                    if (CommonMethods.GetNodeValue(retStr, "tempUnits").ToLower() == "centigrade")
                    {
                        label100.Text = "(" + temperatureLmin.ToString() + "-" + temperatureLmax.ToString() + "）℃";
                    }
                    else
                    {
                        label100.Text = "(" + temperatureLmin.ToString() + "-" + temperatureLmax.ToString() + "）℉";
                    }

                    int holdTime = int.Parse(CommonMethods.GetNodeValue(retStr, "alarmHoldTime"));
                    if (holdTime >= 0 && holdTime <= 3)
                        cmb_AlarmTimeTemperature.SelectedIndex = 0;
                    else if(holdTime > 3 && holdTime <= 5)
                        cmb_AlarmTimeTemperature.SelectedIndex = 1;
                    else if(holdTime > 5 && holdTime <= 10)
                        cmb_AlarmTimeTemperature.SelectedIndex = 2;
                    else if(holdTime > 10 && holdTime <= 20)
                        cmb_AlarmTimeTemperature.SelectedIndex = 3;
                    else if(holdTime > 20 && holdTime <= 30)
                        cmb_AlarmTimeTemperature.SelectedIndex = 4;
                    else if (holdTime > 30 && holdTime <= 60)
                        cmb_AlarmTimeTemperature.SelectedIndex = 5;
                    else if (holdTime > 60)
                        cmb_AlarmTimeTemperature.SelectedIndex = 6;

                    if (CommonMethods.GetNodeValue(retStr, "audioSwitch").ToLower() == "true")
                        cb_BeeperEnable_T.Checked = true;
                    else
                        cb_BeeperEnable_T.Checked = false;

                    if (CommonMethods.GetNodeValue(retStr, "savePicSwitch").ToLower() == "true")
                        cb_SDCaptureEnable_T.Checked = true;
                    else
                        cb_SDCaptureEnable_T.Checked = false;

                    if (CommonMethods.GetNodeValue(retStr, "sdRecSwitch").ToLower() == "true")
                        cb_SDRecordEnable_T.Checked = true;
                    else
                        cb_SDRecordEnable_T.Checked = false;

                }

                Marshal.FreeHGlobal(lpOutBuffer);

            }
            else
                MessageBox.Show("not supportThermometry!");


           
        }

        private void btn_SetTemperatureConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportThermometry;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string takeEnable = cb_temperatureEnable.Checked ? "true" : "false";
                string tempUnits = cmb_Unit.SelectedIndex == 0 ? "centigrade" : "Fahrenheit";

                string highThreshold = tempUnits == "centigrade" ? "highThreshold" : "FhighThreshold";
                string lowThreshold = tempUnits == "centigrade" ? "lowThreshold" : "FlowThreshold";

                string temperatureH = (float.Parse(tb_HighLimit.Text.Trim()) * 100).ToString() ;
                string switchEnableH = cb_TooHighAlarmEnable.Checked? "true" : "false";

                string temperatureL = (float.Parse(tb_LowLimit.Text.Trim()) * 100).ToString() ;
                string switchEnableL = cb_TooLowAlarmEnable.Checked? "true" : "false";
                
                string threshHoldXml = @"<{0}>
                            <switch type='boolean' default='false'>{1}</switch>
                            <value type='uint32' min='0' max='{2}'>{3}</value>
                            </{0}>";
                if (tempUnits == "centigrade")
                {
                    string threshHoldXml1 = string.Format(threshHoldXml, highThreshold, switchEnableH, "9900", temperatureH);
                    string threshHoldXml2 = string.Format(threshHoldXml, lowThreshold, switchEnableL, "9900", temperatureL);

                    temperatureH = ((int)((float.Parse(tb_HighLimit.Text.Trim()) * 1.8 + 32) * 100)).ToString();//摄氏换算成华氏
                    string threshHoldXml3 = string.Format(threshHoldXml, "FhighThreshold", switchEnableH, "21200", temperatureH);

                    temperatureL = ((int)((float.Parse(tb_LowLimit.Text.Trim()) * 1.8 + 32) * 100)).ToString();//摄氏换算成华氏
                    string threshHoldXml4 = string.Format(threshHoldXml, "FlowThreshold", switchEnableL, "21200", temperatureL);

                    threshHoldXml = threshHoldXml1 + threshHoldXml2 + threshHoldXml3 + threshHoldXml4;                   
                }
                else
                {

                    string threshHoldXml1 = string.Format(threshHoldXml, highThreshold, switchEnableH, "21200", temperatureH);
                    string threshHoldXml2 = string.Format(threshHoldXml, lowThreshold, switchEnableL, "21200", temperatureL);


                    temperatureH = ((int)((((float.Parse(tb_HighLimit.Text.Trim()) - 32) / 1.8)) * 100)).ToString();//华氏换算成摄氏
                    string threshHoldXml3 = string.Format(threshHoldXml, "FhighThreshold", switchEnableH, "9900", temperatureH);

                    temperatureL = ((int)((((float.Parse(tb_LowLimit.Text.Trim()) - 32) / 1.8)) * 100)).ToString();//华氏换算成摄氏
                    string threshHoldXml4 = string.Format(threshHoldXml, "FlowThreshold", switchEnableL, "9900", temperatureL);

                    threshHoldXml = threshHoldXml3 + threshHoldXml4 + threshHoldXml1 + threshHoldXml2;           
                }
                string holdTime = "";
                switch (cmb_AlarmTimeTemperature.SelectedIndex)
                {
                    case 0:
                        holdTime = "3";
                        break;
                    case 1:
                        holdTime = "5";
                        break;
                    case 2:
                        holdTime = "10";
                        break;
                    case 3:
                        holdTime = "20";
                        break;
                    case 4:
                        holdTime = "30";
                        break;
                    case 5:
                        holdTime = "60";
                        break;
                    case 6:
                        holdTime = "120";
                        break;
                }
                string savePicSwitch = cb_SDCaptureEnable_T.Checked ? "true" : "false";
                string sdRecSwitch = cb_SDRecordEnable_T.Checked ? "true" : "false";
                string audioSwitch = cb_BeeperEnable_T.Checked ? "true" : "false";

                string xml = @"<?xml version='1.0' encoding='UTF-8'?>
                            <config version='1.7' xmlns='http://www.ipc.com/ver10'>
                            <types>
                            <tempUnitsType>
                            <enum>centigrade</enum>
                            <enum>Fahrenheit</enum>
                            </tempUnitsType>
                            <temperatureMode>
                            <enum>speed</enum>
                            <enum>precision</enum>
                            </temperatureMode>
                            </types>
                            <TakeTemperature>
                            <takeEanble type='boolean' default='true'>{0}</takeEanble>
                            <tempUnits type='tempUnitsType'>{1}</tempUnits>
                            <temperatureMode type='temperatureMode'>speed</temperatureMode>
                            {2}
                            <alarmHoldTime type='uint32'>{3}</alarmHoldTime>
                            
                           
                            <savePicSwitch type='boolean'>{4}</savePicSwitch>
                            <sdRecSwitch type='boolean'>{5}</sdRecSwitch>
                            <audioSwitch type='boolean'>{6}</audioSwitch>
                            </TakeTemperature>
                            </config>";

                xml = string.Format(xml, takeEnable, tempUnits, threshHoldXml, holdTime, savePicSwitch, sdRecSwitch, audioSwitch);

                string m_sendUrl = "SetTakeTemperatureConfig";
                string m_sendXml = xml;
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);
                    if (retStr.Contains("success"))
                    {
                        MessageBox.Show(" SetTakeTemperatureConfig success!");
                    }
                    else
                        MessageBox.Show(" SetTakeTemperatureConfig faild!");
                }
                else
                {
                    MessageBox.Show(" SetTakeTemperatureConfig faild!");
                }
                Marshal.FreeHGlobal(lpOutBuffer);

            }
            else
                MessageBox.Show("not supportThermometry!");
        }

        private void btn_GetMaskConfig_Click(object sender, EventArgs e)
        {
            
            int pos = (int)devIntelist.supportThermometry;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {

                string m_sendUrl = "GetWearmaskDetectConfig";
                string m_sendXml = "";
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);


                    if (CommonMethods.GetNodeValue(retStr, "switch").ToLower() == "true")
                        cb_MaskEnable.Checked = true;
                    else
                        cb_MaskEnable.Checked = false;

                    int holdTime = int.Parse(CommonMethods.GetNodeValue(retStr, "alarmHoldTime"));
                    if (holdTime >= 0 && holdTime <= 3)
                        cmb_AlarmTimeMask.SelectedIndex = 0;
                    else if (holdTime > 3 && holdTime <= 5)
                        cmb_AlarmTimeMask.SelectedIndex = 1;
                    else if (holdTime > 5 && holdTime <= 10)
                        cmb_AlarmTimeMask.SelectedIndex = 2;
                    else if (holdTime > 10 && holdTime <= 20)
                        cmb_AlarmTimeMask.SelectedIndex = 3;
                    else if (holdTime > 20 && holdTime <= 30)
                        cmb_AlarmTimeMask.SelectedIndex = 4;
                    else if (holdTime > 30 && holdTime <= 60)
                        cmb_AlarmTimeMask.SelectedIndex = 5;
                    else if (holdTime > 60)
                        cmb_AlarmTimeMask.SelectedIndex = 6;

                    if (CommonMethods.GetNodeValue(retStr, "audioSwitch").ToLower() == "true")
                        cb_BeeperEnable_M.Checked = true;
                    else
                        cb_BeeperEnable_M.Checked = false;

                    if (CommonMethods.GetNodeValue(retStr, "savePicSwitch").ToLower() == "true")
                        cb_SDCaptureEnable_M.Checked = true;
                    else
                        cb_SDCaptureEnable_M.Checked = false;

                    if (CommonMethods.GetNodeValue(retStr, "sdRecSwitch").ToLower() == "true")
                        cb_SDRecordEnable_M.Checked = true;
                    else
                        cb_SDRecordEnable_M.Checked = false;



                }
            }
            else
                MessageBox.Show("not supportThermometry!");
        }

        private void btn_SetMaskConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportThermometry;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string takeEnable = cb_MaskEnable.Checked ? "true" : "false";

                string holdTime = "";
                switch (cmb_AlarmTimeMask.SelectedIndex)
                {
                    case 0:
                        holdTime = "3";
                        break;
                    case 1:
                        holdTime = "5";
                        break;
                    case 2:
                        holdTime = "10";
                        break;
                    case 3:
                        holdTime = "20";
                        break;
                    case 4:
                        holdTime = "30";
                        break;
                    case 5:
                        holdTime = "60";
                        break;
                    case 6:
                        holdTime = "120";
                        break;
                }
                string savePicSwitch = cb_SDCaptureEnable_M.Checked ? "true" : "false";
                string sdRecSwitch = cb_SDRecordEnable_M.Checked ? "true" : "false";
                string audioSwitch = cb_BeeperEnable_M.Checked ? "true" : "false";

                string xml = @"<?xml version='1.0' encoding='UTF-8'?>
                                <config version='1.7' xmlns='http://www.ipc.com/ver10'>
                                <WearmaskDetect>
                                <switch type='boolean' default='true'>{0}</switch>
                                <alarmHoldTime type='uint32'>{1}</alarmHoldTime>                                                                                        
                                <savePicSwitch type='boolean'>{2}</savePicSwitch>
                                <sdRecSwitch type='boolean'>{3}</sdRecSwitch>
                                <audioSwitch type='boolean'>{4}</audioSwitch>
                                </WearmaskDetect>
                                </config>";

                xml = string.Format(xml, takeEnable, holdTime, savePicSwitch, sdRecSwitch, audioSwitch);

                string m_sendUrl = "SetWearmaskDetectConfig";
                string m_sendXml = xml;
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);
                    if (retStr.Contains("success"))
                    {
                        MessageBox.Show(" SetWearmaskDetectConfig success!");
                    }
                    else
                        MessageBox.Show(" SetWearmaskDetectConfig faild!");
                }
                else
                {
                    MessageBox.Show(" SetWearmaskDetectConfig faild!");
                }
                Marshal.FreeHGlobal(lpOutBuffer);

            }
            else
                MessageBox.Show("not supportThermometry!");
        }

        private void btn_GetThermalConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportThermal;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string m_sendUrl = "GetMeasureTemperatureConfig";
                string m_sendXml = "";
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);

                    if (CommonMethods.GetNodeValue(retStr, "takeEanble").ToLower() == "true")
                        cb_ThermalEnable.Checked = true;
                    else
                        cb_ThermalEnable.Checked = false;
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            else
                MessageBox.Show("not supportThermal!");
        }

        private void btn_SetThermalConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportThermal;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
               
                string takeEanble = cb_ThermalEnable.Checked ? "true" : "false";



                //string m_sendUrl = "SetMeasureTemperatureConfig";
                //string m_sendXml = "";
                //int lpBytesReturned = 0;
                //byte[] rec = new byte[1024 * 1024];
                //IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                //bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                //if (ret && lpBytesReturned > 0)
                //{
                //    rec = new byte[lpBytesReturned];
                //    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                //    string retStr = DevSdkHelper.ByteToStr(rec);

                    
                //}
                //Marshal.FreeHGlobal(lpOutBuffer);
            }
            else
                MessageBox.Show("not supportThermal!");
        }

        private void btn_GetPassLineConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportPassLine;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string m_sendUrl = "GetPassLineCountConfig";
                string m_sendXml = "";
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);

                    if (CommonMethods.GetNodeValue(retStr, "switch").ToLower() == "true")
                        cb_PassLineEnable.Checked = true;
                    else
                        cb_PassLineEnable.Checked = false;
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            else
                MessageBox.Show("not passline!");
        }

        private void btn_SetPassLineConfig_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportPassLine;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string m_sendUrl = "GetPassLineCountConfig";
                string m_sendXml = "";
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);

                    int pos1 = retStr.IndexOf("<switch type=\"boolean\">");
                    int pos2 = retStr.IndexOf("</switch>");
                    int tempLen = "<switch type=\"boolean\">".Length;

                    string subStr1 = retStr.Substring(0, pos1 + tempLen);
                    string subStr2 = retStr.Substring(pos2);

                    string enabelStr = cb_PassLineEnable.Checked.ToString().ToLower();
                    retStr = subStr1 + enabelStr + subStr2;

                    m_sendUrl = "SetPassLineCountConfig";
                    ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, retStr, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                    if (ret && lpBytesReturned > 0)
                    {
                        rec = new byte[lpBytesReturned];
                        Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                        retStr = DevSdkHelper.ByteToStr(rec);

                        if (retStr.Contains("success"))
                        {
                            MessageBox.Show("SetPassLineCountConfig success!");
                        }
                        else
                            MessageBox.Show("SetPassLineCountConfig faild!");
                    }
                   
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            else
                MessageBox.Show("not passline!");
        }

        private void btn_smart_subscrib_AVD_Click(object sender, EventArgs e)
        {
            bool ret = false;
            int chn = 0;
            if (!m_SubscribAVD)
            {
                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib = new NET_DVR_SUBSCRIBE_REPLY();

                //int nSizeOfReply = Marshal.SizeOf(sSmartSubscrib);
                //IntPtr ptrOfReply = Marshal.AllocHGlobal(nSizeOfReply);
                //Marshal.StructureToPtr(sSmartSubscrib, ptrOfReply, true);

                ret = DevSdkHelper.NET_SDK_SmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_AVD, chn, ref sSmartSubscrib);
                if (ret)
                {
                    //sSmartSubscrib = (NET_DVR_SUBSCRIBE_REPLY)Marshal.PtrToStructure(ptrOfReply, typeof(NET_DVR_SUBSCRIBE_REPLY));
                    m_serverAddressAVD = sSmartSubscrib.serverAddress;
                    m_SubscribAVD = true;
                    button9.Text = "UnSubscribAVD";
                    button9.FlatStyle = FlatStyle.Popup;
                }
                else
                    ShowErrorMessage();
                //Marshal.FreeHGlobal(ptrOfReply);
            }
            else
            {
                int dwResult = 0;
                IntPtr temp = PointArrayToIntPtr(m_serverAddressAVD);
                bool bret = DevSdkHelper.NET_SDK_UnSmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_AVD, chn, temp, ref dwResult);
                Marshal.FreeHGlobal(temp);
                if (bret)
                {
                    m_SubscribAVD = false;
                    button9.Text = "SubscribAVD";
                    button9.FlatStyle = FlatStyle.Standard;
                }
                else
                    ShowErrorMessage();
            }
        }


        private void btn_smart_subscrib_VFD_Click(object sender, EventArgs e)
        {
            bool ret = false;
            int chn = 0;
            if (!m_SubscribVFD)
            {
                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib = new NET_DVR_SUBSCRIBE_REPLY();

                //int nSizeOfReply = Marshal.SizeOf(sSmartSubscrib);
                //IntPtr ptrOfReply = Marshal.AllocHGlobal(nSizeOfReply);
                //Marshal.StructureToPtr(sSmartSubscrib, ptrOfReply, true);

                ret = DevSdkHelper.NET_SDK_SmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_VFD, chn, ref sSmartSubscrib);
                if (ret)
                {
                    //sSmartSubscrib = (NET_DVR_SUBSCRIBE_REPLY)Marshal.PtrToStructure(ptrOfReply, typeof(NET_DVR_SUBSCRIBE_REPLY));
                    m_serverAddressVFD = sSmartSubscrib.serverAddress;
                    m_SubscribVFD = true;
                    button10.Text = "UnSubscribVFD";
                    button10.FlatStyle = FlatStyle.Popup;
                }
                else
                    ShowErrorMessage();
                //Marshal.FreeHGlobal(ptrOfReply);

            }
            else
            {
                int dwResult = 0;
                IntPtr temp = PointArrayToIntPtr(m_serverAddressAVD);
                bool bret = DevSdkHelper.NET_SDK_UnSmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_VFD, chn, temp, ref dwResult);
                Marshal.FreeHGlobal(temp);
                if (bret)
                {
                    m_SubscribVFD = false;
                    button10.Text = "SubscribVFD";
                    button10.FlatStyle = FlatStyle.Standard;
                }
                else
                    ShowErrorMessage();
            }
        }

        private void btn_smart_subscrib_FaceMatch_Click(object sender, EventArgs e)
        {
            bool ret = false;
            int chn = 0;
            if (!m_SubscribFaceMatch)
            {
                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib = new NET_DVR_SUBSCRIBE_REPLY();
                //int nSizeOfReply = Marshal.SizeOf(sSmartSubscrib);
                //IntPtr ptrOfReply = Marshal.AllocHGlobal(nSizeOfReply);
                //Marshal.StructureToPtr(sSmartSubscrib, ptrOfReply, true);

                ret = DevSdkHelper.NET_SDK_SmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_VFD_MATCH, chn, ref sSmartSubscrib);
                if (ret)
                {
                    //sSmartSubscrib = (NET_DVR_SUBSCRIBE_REPLY)Marshal.PtrToStructure(ptrOfReply, typeof(NET_DVR_SUBSCRIBE_REPLY));
                    m_serverAddressVFD = sSmartSubscrib.serverAddress;
                    m_SubscribFaceMatch = true;
                    button11.Text = "UnSubscribVFDMatch";
                    button11.FlatStyle = FlatStyle.Popup;
                }
                else
                    ShowErrorMessage();
                ret = DevSdkHelper.NET_SDK_SmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_VFD_MATCH_FAILED, chn, ref sSmartSubscrib);

                //Marshal.FreeHGlobal(ptrOfReply);
            }
            else
            {
                int dwResult = 0;
                IntPtr temp = PointArrayToIntPtr(m_serverAddressAVD);
                bool bret = DevSdkHelper.NET_SDK_UnSmartSubscrib(userId, (int)NET_DVR_SMART_TYPE.NET_DVR_SMART_VFD_MATCH, chn, temp, ref dwResult);
                Marshal.FreeHGlobal(temp);
                if (bret)
                {
                    m_SubscribFaceMatch = false;
                    button11.Text = "SubscribVFDMatch";
                    button11.FlatStyle = FlatStyle.Standard;
                }
                else
                    ShowErrorMessage();


            }
        }

        //private void lv_FaceIPC_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    ListViewHitTestInfo info = lv_FaceIPC.HitTest(e.X, e.Y);
        //    if (info != null)
        //    {
        //        var videoitem = info.Item as ListViewItem;
        //        string selectFace = videoitem.SubItems[1].Text;
        //        foreach (FileInfo fi in (new DirectoryInfo(Application.StartupPath + "\\FaceImg")).GetFiles())//遍历文件夹中的文件
        //        {
        //            if (fi.Name.Substring(0, fi.Name.Length - 4) == selectFace)
        //            {
        //                pictureBox5.Image = null;
        //                pictureBox5.ImageLocation = fi.FullName;
        //            }
        //        }
        //    }
        //}
        //private void lv_FaceMatchIPC_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    ListViewHitTestInfo info = lv_FaceMatchIPC.HitTest(e.X, e.Y);
        //    if (info != null)
        //    {
        //        var videoitem = info.Item as ListViewItem;
        //        string selectFace = videoitem.SubItems[1].Text;
        //        foreach (FileInfo fi in (new DirectoryInfo(Application.StartupPath + "\\FaceMatchImg")).GetFiles())//遍历文件夹中的文件
        //        {
        //            if (fi.Name.Substring(0, fi.Name.Length - 4) == selectFace)
        //            {
        //                pictureBox6.Image = null;
        //                pictureBox6.ImageLocation = fi.FullName;
        //            }
        //        }
        //        foreach (FileInfo fi in (new DirectoryInfo(Application.StartupPath + "\\FaceTargetImg")).GetFiles())//遍历文件夹中的文件
        //        {
        //            if (fi.Name.Substring(0, fi.Name.Length - 4) == selectFace)
        //            {
        //                pictureBox7.Image = null;
        //                pictureBox7.ImageLocation = fi.FullName;
        //            }
        //        }
        //    }
        //}

        private void cmb_Unit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_Unit.SelectedIndex == 0)
            {
                temperatureHmin = (float)(((float)(temperatureHmin - 32)) / 1.8);
                temperatureHmax = (float)(((float)(temperatureHmax - 32)) / 1.8);
                temperatureLmin = (float)(((float)(temperatureLmin - 32)) / 1.8);
                temperatureLmax = (float)(((float)(temperatureLmax - 32)) / 1.8);


                if (tb_HighLimit.Text.Trim().Length > 0)
                {
                    tb_HighLimit.Text = ((float)((((float.Parse(tb_HighLimit.Text.Trim()) - 32) / 1.8)))).ToString("0.#");//华氏换算成摄氏
                }

                if (tb_LowLimit.Text.Trim().Length > 0)
                {
                    tb_LowLimit.Text = ((float)((((float.Parse(tb_LowLimit.Text.Trim()) - 32) / 1.8)))).ToString("0.#");//华氏换算成摄氏
                }
                    
                
                label99.Text = "(" + temperatureHmin.ToString() + "-" + temperatureHmax.ToString() + "）℃";
                label100.Text = "(" + temperatureLmin.ToString() + "-" + temperatureLmax.ToString() + "）℃";
            }
            else
            {
                temperatureHmin = (float)(((float)temperatureHmin) * 1.8 + 32);
                temperatureHmax = (float)(((float)temperatureHmax) * 1.8 + 32);
                temperatureLmin = (float)(((float)temperatureLmin) * 1.8 + 32);
                temperatureLmax = (float)(((float)temperatureLmax) * 1.8 + 32);
                if (tb_HighLimit.Text.Trim().Length > 0)
                {
                    tb_HighLimit.Text = ((int)((float.Parse(tb_HighLimit.Text.Trim()) * 1.8 + 32))).ToString("0.#");//摄氏换算成华氏
                }

                if (tb_LowLimit.Text.Trim().Length > 0)
                {
                    tb_LowLimit.Text = ((int)((float.Parse(tb_LowLimit.Text.Trim()) * 1.8 + 32))).ToString("0.#");//摄氏换算成华氏
                }
                    

                label99.Text = "(" + temperatureHmin.ToString() + "-" + temperatureHmax.ToString() + "）℉ ";
                label100.Text = "(" + temperatureLmin.ToString() + "-" + temperatureLmax.ToString() + "）℉";
            }

        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpg files(*.jpg)|*.jpg||";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                FileStream file = new FileStream(path, FileMode.Open);
                byte[] b = new byte[file.Length];
                file.Read(b, 0, b.Length);

                //MemoryStream stream = new MemoryStream(b);
                //if (pictureBox1.Image != null)
                //    pictureBox1.Image.Dispose();
                //pictureBox1.Image = null;
                //pictureBox1.Image = Image.FromStream(stream);
                //stream.Close();
                file.Close();
                LoadPicture(b, pictureBox9);
            }
        }

        private void btn_ResetPassLineData_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportPassLine;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string m_sendUrl = "SetPassLineCountConfig";
                string m_sendXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <config version=\"1.7\" xmlns=\"http://www.ipc.com/ver10\">" +
                                    "<passlinecount>" +
                                    "<forceReset type=\"boolean\">true</forceReset>" +
                                    "</passlinecount>" +
                                    "</config>";

                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);
                    if (retStr.Contains("success"))
                    {
                        MessageBox.Show("SetPassLineCountConfig success!");
                    }
                    else
                        MessageBox.Show("SetPassLineCountConfig faild!");
                    
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            else
                MessageBox.Show("not passline!");
        }

        private void btn_GetPassLineData_Click(object sender, EventArgs e)
        {
            int pos = (int)devIntelist.supportPassLine;
            if ((oDevSupport.support & (1 << pos)) >> pos == 1)
            {
                string m_sendUrl = "GetPassLineCountStatistics";
                string m_sendXml = "";

                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);
                    label41.Text = retStr;

                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            else
                MessageBox.Show("not passline!");
        }

        private void btn_ChangePsw_Click(object sender, EventArgs e)
        {
            bool changeOher = false;
            if (lv_UserList.SelectedItems.Count < 1)
            {
                MessageBox.Show("Select User first !!");
                return;
            }
            int Index = lv_UserList.SelectedItems[0].Index;

            UserInfo tempUserInfo = userInfoDic[Index];
            if (Index < 0)
                return;
            if (tempUserInfo.userName != "admin" && tempUserInfo.userName != this.usr)
            {
                changeOher = true;
//                 MessageBox.Show("Can only change logined user's password!");
//                 return;
            }
            ChangePsw cp = new ChangePsw();
            cp.SetUserName(tempUserInfo.userName);
            cp.ShowDialog();
            if (cp.DialogResult == DialogResult.OK)
            {
                string m_sendUrl = "";
                if (!changeOher)
                {
                    m_sendUrl = "editUserPassword";
                }
                else
                    m_sendUrl = "editUser";
                string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <request version='1.0'   systemType='NVMS-9000' clientType='WEB'/>";

                XmlDocument oldXd = new XmlDocument();
                oldXd.LoadXml(m_sendXml);

                var root = oldXd.DocumentElement;//
                XmlNode newNode = oldXd.CreateNode("element", "content", "");
                root.AppendChild(newNode);

                if (changeOher)
                {
                    XmlElement tempElement11 = oldXd.CreateElement("userId");
                    XmlCDataSection cd11 = oldXd.CreateCDataSection(tempUserInfo.id);
                    tempElement11.AppendChild(cd11);
                    newNode.AppendChild(tempElement11);


                    tempElement11 = oldXd.CreateElement("password");
                    string md5str = CommonMethods.GenerateMD5(cp.newPsw).ToUpper();

                    tempElement11.AppendChild(oldXd.CreateCDataSection(md5str));
                    newNode.AppendChild(tempElement11);
                }
                else
                {
                    XmlElement tempElement = oldXd.CreateElement("oldPassword");
                    string md5str = CommonMethods.GenerateMD5(cp.currentPsw).ToUpper();
                    byte[] bytes = Encoding.Default.GetBytes(md5str);
                    string ba64 = Convert.ToBase64String(bytes);

                    XmlCDataSection cd = oldXd.CreateCDataSection(ba64);
                    tempElement.AppendChild(cd);
                    newNode.AppendChild(tempElement);

                    tempElement = oldXd.CreateElement("password");
                    md5str = CommonMethods.GenerateMD5(cp.newPsw).ToUpper();
                    bytes = Encoding.Default.GetBytes(md5str);
                    ba64 = Convert.ToBase64String(bytes);

                    cd = oldXd.CreateCDataSection(ba64);
                    tempElement.AppendChild(cd);
                    newNode.AppendChild(tempElement);
                }
                           
                m_sendXml = oldXd.InnerXml;
                int lpBytesReturned = 0;
               
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    byte[] rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);
                    if (retStr.Length > 0)
                    {
                        if (retStr.Contains("success"))
                        {
                            if (!changeOher)
                            {
                                MessageBox.Show("Edit password success, application will close, please login again");
                                this.Close();
                            }
                            else
                                MessageBox.Show("Edit "+ tempUserInfo.userName +"'s password success");
                            
                        }

                        else
                            MessageBox.Show("Edit password failed");
                    }
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            
        }

        private void btn_ViewSnapPic_Click(object sender, EventArgs e)
        {
            if (lv_ChSnapList.SelectedItems.Count < 1)
            {
                MessageBox.Show("no snap face image selected! ");
                return;
            }
            int Index = lv_ChSnapList.SelectedItems[0].Index;
            if (Index > m_snapList.Count)
            {
                //MessageBox.Show("no snap face image selected! ");
                return;
            }
            NET_SDK_FACE_IMG_INFO_CH targetFaceImg = m_snapList[Index + 1];
            if (rb_TargetInfo.Checked)
            {
                NET_SDK_FACE_INFO_LIST_GET nsfilg = new NET_SDK_FACE_INFO_LIST_GET();
                nsfilg.pageIndex = 1;
                nsfilg.pageSize = 15;
                nsfilg.itemId = targetFaceImg.targetImgId;

                IntPtr lpInputBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(nsfilg));
                Marshal.StructureToPtr(nsfilg, lpInputBuffer, true);
                IntPtr lpOutputBuffer = Marshal.AllocHGlobal(1000 * 1024);
                uint lpBytesReturned = 0;

                bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_INFO,
                    lpInputBuffer, (uint)Marshal.SizeOf(nsfilg), lpOutputBuffer, 1000 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    int nSizeOfFaceItem = Marshal.SizeOf(typeof(NET_SDK_FACE_INFO_LIST_ITEM));
                    NET_SDK_FACE_INFO_LIST nsfil = (NET_SDK_FACE_INFO_LIST)Marshal.PtrToStructure(lpOutputBuffer, typeof(NET_SDK_FACE_INFO_LIST));
                    for (int i = 0; i < nsfil.listNum; ++i)
                    {
                        NET_SDK_FACE_INFO_LIST_ITEM Face_Item = (NET_SDK_FACE_INFO_LIST_ITEM)Marshal.PtrToStructure(nsfil.pFaceInfoListItem + i * nSizeOfFaceItem, typeof(NET_SDK_FACE_INFO_LIST_ITEM));
                        string name = DevSdkHelper.ByteToStr(Face_Item.name);
                        string cert = DevSdkHelper.ByteToStr(Face_Item.certificateNum);
                        MessageBox.Show(name + cert);
                    }
                }
                Marshal.FreeHGlobal(lpOutputBuffer);
                Marshal.FreeHGlobal(lpInputBuffer);
            }
            else
            {
                targetFaceImg.isPanorama = (byte)(rb_FacePic.Checked ? 0 : 1);

                IntPtr lpInputBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(targetFaceImg));
                Marshal.StructureToPtr(targetFaceImg, lpInputBuffer, true);
                IntPtr lpOutputBuffer = Marshal.AllocHGlobal(1000 * 1024);

                uint lpBytesReturned = 0;
                bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_CH_SNAP_FACE_IMG,
                    lpInputBuffer, (uint)Marshal.SizeOf(targetFaceImg), lpOutputBuffer, 1000 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    NET_SDK_FACE_INFO_IMG_DATA imgdata = (NET_SDK_FACE_INFO_IMG_DATA)Marshal.PtrToStructure(lpOutputBuffer, typeof(NET_SDK_FACE_INFO_IMG_DATA));

                    if (pictureBox8.Image != null)
                        pictureBox8.Image.Dispose();
                    pictureBox8.Image = null;

                    byte[] data = new byte[imgdata.imgLen];
                    Marshal.Copy(imgdata.imgData, data, 0, (int)imgdata.imgLen);
                    MemoryStream stream = new MemoryStream(data);
                    pictureBox8.Image = Image.FromStream(stream);
                }

                Marshal.FreeHGlobal(lpOutputBuffer);
                Marshal.FreeHGlobal(lpInputBuffer);
            }                                   	
        }

        private void btn_Extra_Click(object sender, EventArgs e)
        {
            if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
            {
                string szUrl = "GetSearchImageByImage";
                string tempXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <request version='1.0' xmlns='http://www.ipc.com/ver10'>
                                                    <pageIndex>{0}</pageIndex> 
                                                    <pageSize>18</pageSize>
                                                    <condition>   
                                                         <startTime>{1}</startTime> 
                                                         <endTime>{2}</endTime>
                                                        <chls type='list'>{3}</chls>
                                                        <recognizedFilter>    
                                                            <isContainRecognized>true</isContainRecognized>      
                                                            <isContainNotRecognized>false</isContainNotRecognized>
                                                            <faceFeatureGroup type='list'>{4}</faceFeatureGroup>  
                                                        </recognizedFilter>
                                                        <similarity>75</similarity>
                                                        <resultCountLimit>9999</resultCountLimit> 
                                                    </condition>
                                                </request>";
                string chls = @"<item id='{0}'></item>";
                StringBuilder sb = new StringBuilder();
                List<string> chlIds = GetChlIds();
                if (chlIds.Count > 0)
                {
                    for (int kk = 0; kk < chlIds.Count; kk++)
                        sb.Append(string.Format(chls, chlIds[kk]));
                }

                StringBuilder groups = new StringBuilder();
                List<string> groupIds = GetGroupIds();
                if (groupIds.Count > 0)
                {
                    for (int kk = 0; kk < groupIds.Count; kk++)
                        groups.Append(string.Format(chls, groupIds[kk]));
                }

                string startTime = dtp_face_start1.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string endTime = dtp_face_end1.Value.ToString("yyyy-MM-dd HH:mm:ss");

                long pageIndex = 1;
                progressBar1.Value = 1;
                getFacesByAPI(tempXML, szUrl, pageIndex, startTime, endTime, sb.ToString(), groups.ToString());
            }

        }

        private void btn_testChangeStreamType_Click(object sender, EventArgs e)
        {
            //Thread asdf = new Thread(runtest);
            //asdf.IsBackground = true;
            //asdf.Start();
            //for (int i = 0; i < 10; i++)
            //{
            //    IntPtr iip = Marshal.AllocHGlobal(4);
            //    Marshal.StructureToPtr(i, iip, true);

            //    MessageBox.Show(DevSdkHelper.NET_SDK_GetErrorMsg(iip));
            //    Marshal.FreeHGlobal(iip);
            //}
                
        }

        private void runtest()
        {
            //    Invoke(new Action(() =>
            //    {
            foreach (Control ct in tabPage17.Controls)
            {
                Invoke(new Action(() => { ct.Enabled = false; }));

            }
            POINTERHANDLE playHandle = 0;
            int times = 100000;
            for (int i = 0; i < times; i++)
            {
                NET_SDK_CLIENTINFO clientInfo = new NET_SDK_CLIENTINFO();
                Invoke(new Action(() =>
                {

                    clientInfo.lChannel = cmb_Channel.SelectedIndex;
                    clientInfo.hPlayWnd = panel1.Handle;
                    clientInfo.streamType = i % 2 == 0 ? 0 : 1;
                }));



                Thread.Sleep(100);
                playHandle = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, null, IntPtr.Zero);
                if (playHandle < 1)
                    break;

                Thread.Sleep(300);
                bool ret = DevSdkHelper.NET_SDK_StopLivePlay((int)playHandle);

                if (!ret)
                    break;
                Invoke(new Action(() =>
                {
                    //label40.Text = "tested time: " + (i + 1);
                    panel1.BackColor = Color.Transparent;
                    panel1.BackColor = Color.Gray;
                }));

                Thread.Sleep(100);
            }

            foreach (Control ct in tabPage17.Controls)
            {
                Invoke(new Action(() => { ct.Enabled = true; }));
            }
            //}));
        }

        private void btn_Display_Click(object sender, EventArgs e)
        {
            sbsc = Subscribe;
            //IntPtr pthis = new IntPtr();
            bool sret = DevSdkHelper.NET_SDK_SetSubscribCallBack(sbsc, IntPtr.Zero);
            if (sret)
            {
                Debug.Print("Set Subscrib success!");
            }
        }

        private void btn_NotDisplay_Click(object sender, EventArgs e)
        {
            DevSdkHelper.NET_SDK_SetSubscribCallBack(null, IntPtr.Zero);
            pb_SnapPic.Image = null;
            pb_TargetPic.Image = null;
            pb_SourcePic.Image = null;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //long aa = 1603078489730770 / 1000;
            long aa = 1603078489730770;
            DateTime dt = DevSdkHelper.GetDateTime(aa);
            int a = 0;


            long abc = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "jpg files(*.jpg)|*.jpg||";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string savePath = saveFileDialog1.FileName;
                int picSize = 512 * 1024;
                IntPtr sp = Marshal.AllocHGlobal(picSize);
                int size = 0;
                bool ret = DevSdkHelper.NET_SDK_CaptureJpeg(userId, theStatus[currentPanelHandle].channelIndex, 0, sp, picSize, ref size);
                if (ret)
                {
                    byte[] data = new byte[size];
                    Marshal.Copy(sp, data, 0, size);
                    if (File.Exists(savePath))
                        File.Delete(savePath);
                    FileStream fs = new FileStream(savePath, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(data, 0, data.Length);
                    fs.Flush();//数据写入图片文件
                    fs.Close();
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(sp);
            }
        }

        private void lv_IPCVehicle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (oNET_SDK_DEVICEINFO.deviceType != 2)
                return;           
            ListViewHitTestInfo info = lv_IPCVehicle.HitTest(e.X, e.Y);
            if (info != null)
            {                              
                var videoitem = info.Item as ListViewItem;
                string m_sendUrl = "SearchSnapVehicleByKey";

                string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                            <config><search>
                            <snapTime type='uint64'>{0}</snapTime>  
                            <vehicleID type='uint32'>{1}</vehicleID>  
                            <requestPanoramicPic type='boolean'>{2}</requestPanoramicPic>
                            </search></config>";
                m_sendXml = string.Format(m_sendXml, videoitem.SubItems[1].Text, videoitem.SubItems[0].Text,cb_DisplaySource.Checked.ToString().ToLower());
                int lpBytesReturned = 0;
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(10 * 1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 10 * 1024 * 1024, ref lpBytesReturned);
                if (ret && lpBytesReturned > 0)
                {
                    byte[] rec = new byte[lpBytesReturned];
                    Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                    string retStr = DevSdkHelper.ByteToStr(rec);
                    if (retStr.Length > 0)
                    {
                        string snapData = CommonMethods.GetNodeValue(retStr, "pictureData", 0);
                        LoadPicture(Convert.FromBase64String(snapData), pb_SnapVehicle);//display the snap vehicle

                        if (CommonMethods.GetNodePosByIndex(retStr, "panoramicInfo") != null)
                        {
                            snapData = CommonMethods.GetNodeValue(retStr, "pictureData", 1);
                            LoadPicture(Convert.FromBase64String(snapData), pb_SourceVehicle);//display the panoramicInfo
                        }                           
                    }
                }
                Marshal.FreeHGlobal(lpOutBuffer);                
            }
        }

        private void treeViewNvr_DoubleClick(object sender, EventArgs e)
        {
            //cmb_Channel.setc
            string Index = treeViewNvr.SelectedNode.Text;
            foreach (int key in theTreeNodeName.Keys)
            {
                if (Index == theTreeNodeName[key])
                {
                    cmb_Channel.SelectedIndex = key;
                    break;
                }
            }
            try
            {
                NET_SDK_CLIENTINFO clientInfo = new NET_SDK_CLIENTINFO();
                clientInfo.lChannel = theStatus[currentPanelHandle].channelIndex;
                clientInfo.hPlayWnd = currentPanelHandle;
                clientInfo.streamType = theStatus[currentPanelHandle].streamType;
                //clientInfo.bNoDecode = 1;
                {
                    #region 增加回调
                    //NET_SDK_FRAME_INFO finfo = new NET_SDK_FRAME_INFO();
                    myldc = MyLiveDataCallBack;
                    myldc = null;
                    int playHandle = (int)DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, myldc, IntPtr.Zero);
                    yuvldc = YuvLiveDataCallBack;
                    DevSdkHelper.NET_SDK_SetYUVCallBack(playHandle, yuvldc, IntPtr.Zero);
                    //POINTERHANDLE playHandle = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, myldc, IntPtr.Zero);
                    #endregion
                    //Int32 playHandle = DevSdkHelper.NET_SDK_LivePlay(userId, ref clientInfo, null, IntPtr.Zero);
                    int dwBufNum = 1;
                    //                     bool quality = DevSdkHelper.NET_SDK_SetPlayerBufNumber(playHandle, dwBufNum);
                    //                     if (!quality)
                    //                         ShowErrorMessage();
                    //                     else
                    //                         MessageBox.Show("dwBufNum is set to " + dwBufNum + "(the smaller the dwBufNum is, the smalller the video delay is)");

                    if (playHandle != -1)
                    {
                        temphandle = playHandle;
                        isPlay = true;
                        if (soundChannelOccupied)
                            Control.FromHandle(currentPanelHandle).ContextMenuStrip = audioDisabled;
                        else
                            Control.FromHandle(currentPanelHandle).ContextMenuStrip = audioClosed;

                        #region 回调修改
                        //NET_SDK_FRAME_INFO finfo = new NET_SDK_FRAME_INFO();
                        //ldc = LiveDataCallBack;
                        //ldc(playHandle, finfo, IntPtr.Zero, currentPanelHandle);
                        #endregion
                        //if (dfc == null)
                        //    dfc = new DRAW_FUN_CALLBACK(OsdDisplay);
                        //bool ret = DevSdkHelper.NET_SDK_RegisterDrawFun(playHandle, dfc, currentPanelHandle);


                        ItemStatus tempStatus = theStatus[currentPanelHandle];
                        tempStatus.playHandle = playHandle;
                        tempStatus.playStatus = true;
                        theStatus[currentPanelHandle] = tempStatus;

                        btn_Live.Text = "Stop live";
                        btn_LiveSave.Enabled = true;
                        btn_Color_Default.Enabled = true;
                        btn_Track_Record.Enabled = true;
                        btn_Track.Enabled = true;

                        NET_SDK_IMAGE_EFFECT_T bright = new NET_SDK_IMAGE_EFFECT_T();
                        NET_SDK_IMAGE_EFFECT_T contrast = new NET_SDK_IMAGE_EFFECT_T();
                        NET_SDK_IMAGE_EFFECT_T saturation = new NET_SDK_IMAGE_EFFECT_T();
                        NET_SDK_IMAGE_EFFECT_T hue = new NET_SDK_IMAGE_EFFECT_T();
                        bool tempbool = DevSdkHelper.NET_SDK_GetVideoEffect_Ex(userId, theStatus[currentPanelHandle].channelIndex, ref bright, ref contrast, ref saturation, ref hue);
                        if (tempbool)
                        {
                            tkbar_Brightness.Maximum = (int)bright.maxValue;
                            tkbar_Brightness.Value = (int)bright.curValue;

                            tkbar_Contrast.Maximum = (int)contrast.maxValue;
                            tkbar_Contrast.Value = (int)contrast.curValue;


                            tkbar_Saturation.Maximum = (int)saturation.maxValue;
                            tkbar_Saturation.Value = (int)saturation.curValue;

                            tkbar_Hue.Maximum = (int)hue.maxValue;
                            if ((int)hue.curValue < 0)
                            {
                                tkbar_Hue.Enabled = false;
                            }
                            else
                            {
                                tkbar_Hue.Enabled = true;
                                tkbar_Hue.Value = (int)hue.curValue;
                            }

                        }
                        UpdateItemData();
                    }
                    else
                    {
                        UInt32 dwErrorCode = DevSdkHelper.NET_SDK_GetLastError();
                        string strMsg = String.Format("{0} \r\nerror code: {1}", STR_LIVE_FAILED, dwErrorCode);
                        MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                //btn_OsdChlName;
                btn_OsdChlName.Enabled = true;
                btn_OsdTitle.Enabled = true;
                btn_OsdTime.Enabled = true;
                btn_OsdChlName.Text = "chlName Open";
                btn_OsdTitle.Text = "Title Open";
                btn_OsdTime.Text = "Time Open";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_OsdTitle_Click(object sender, EventArgs e)
        {
            if (btn_OsdTitle.Text.Equals("Title Open"))
            {
                osdTitle = true;
                if (dfc == null)
                    dfc = new DRAW_FUN_CALLBACK(OsdTitleDisplay);
                bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(theStatus[currentPanelHandle].playHandle, dfc, currentPanelHandle);
                //btn_OsdTitle.ResetText("Title close");
                btn_OsdTitle.Text = "Title Close";
            }
            else if (btn_OsdTitle.Text.Equals("Title Close"))
            {
                //bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(0, null, currentPanelHandle);
                osdTitle = false;
                btn_OsdTitle.Text = "Title Open";
            }
            return;
        }

        private void btn_OsdChlName_Click(object sender, EventArgs e)
        {
            osdChlName = true;
            if (btn_OsdChlName.Text.Equals("chlName Open"))
            {
                if (dfc == null)
                    dfc = new DRAW_FUN_CALLBACK(OsdTitleDisplay);
                bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(theStatus[currentPanelHandle].playHandle, dfc, currentPanelHandle);
                btn_OsdChlName.Text = "chlName Close";
            }
            else if (btn_OsdChlName.Text.Equals("chlName Close"))
            {
                osdChlName = false;
                btn_OsdChlName.Text = "chlName Open";
            }
        }

        private void btn_OsdTime_Click(object sender, EventArgs e)
        {
            osdTime = true;
            if (btn_OsdTime.Text.Equals("Time Open"))
            {
                if (dfc == null)
                    dfc = new DRAW_FUN_CALLBACK(OsdTitleDisplay);
                bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(theStatus[currentPanelHandle].playHandle, dfc, currentPanelHandle);
                btn_OsdTime.Text = "Time Close";
            }
            else if (btn_OsdTime.Text.Equals("Time Close"))
            {
                osdTime = false;
                btn_OsdTime.Text = "Time Open";
            }
        }

        private void btn_RtspUrl_Click(object sender, EventArgs e)
        {
            int lChlID = 1;
            string Index = cmb_Channel.Text;
            foreach (int key in theTreeNodeName.Keys)
            {
                if (Index == theTreeNodeName[key])
                {
                    lChlID = key + 1;
                    break;
                }
            }

            int lStreamType = cmb_StreamType.SelectedIndex;

            IntPtr pdata = Marshal.AllocHGlobal(32);
            bool bRet = DevSdkHelper.NET_SDK_GetRtspUrl(userId, lChlID, lStreamType, pdata);
            byte[] bIp = new byte[256];
            Marshal.Copy(pdata, bIp, 0, 256);

            string strUrl = System.Text.Encoding.UTF8.GetString(bIp);
            Marshal.FreeHGlobal(pdata);

            textBox_Rtsp.Text = strUrl;
        }

    }
}
