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
using System.Timers;
using System.Threading;

namespace Demo
{
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    public partial class PlayBack_Dialog : Form
    {
        private int userId = 0;
        private uint channelIndex;
        private DD_TIME startTime;
        private DD_TIME endTime;
        private bool isPlay = false;
        private bool isPause = false;
        private int m_playbackHandle = -1;

        private bool osdPlayBackTime = false;
        private bool osdPlayBackChlName = false;
        private bool osdPlayBackTitle = false;

        private ToolStripMenuItem openAudio;
        private ToolStripMenuItem disableAudio;
        private ToolStripMenuItem snap;
        private ToolStripMenuItem openDraw;

        private ContextMenuStrip audioOpened;

        private IntPtr currentPanelHandle;


        private IntPtr PBAudioOutBuffer;
        private int PBAudioDecHandle;

        private MyWaveOut m_waveOut;

        private static DRAW_FUN_CALLBACK playbackdfc;

        private System.Timers.Timer oTimer;
        private SynchronizationContext sc;
        private int currentSpeed;
        private bool forward = true;
        private static PLAY_DATA_CALLBACK pdc = null;
        private static PLAY_YUV_DATA_CALLBACK yuvpdc = null;
        private static DRAW_FUN_CALLBACK dfc = null;
        private string streamFormat;
        public PlayBack_Dialog(int id, DD_TIME start, DD_TIME end, uint index)
        {
            InitializeComponent();
            OneWindowDisplay();
            //FourWindowDisplay();
            userId = id;
            channelIndex = index;
            startTime = start;
            endTime = end;

            ContextMenuInit();
            comboBox1.SelectedIndex = 0;
            sc = SynchronizationContext.Current;

            comboFastSpeed.SelectedIndexChanged += comboFastSpeed_SelectedIndexChanged;
            currentSpeed = (int)NET_SDK_RPB_SPEED.NET_SDK_RPB_SPEED_1X;

            UpdateSpeedDisplay();
            m_waveOut = new MyWaveOut();
        }
        private void OneWindowDisplay()
        {
            panel1.Size = new Size(762, 532);
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;           
        }
        private void FourWindowDisplay()
        {
            panel1.Size = new Size(381, 266);
            panel2.Visible = true;
            panel3.Visible = true;
            panel4.Visible = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                OneWindowDisplay();
            else
                FourWindowDisplay();
            if (isPlay && isPause)
            {
                lock (this)
                {
                    bool ret = DevSdkHelper.NET_SDK_RefreshPlay(m_playbackHandle);
                }               
            }
        }

        private void comboFastSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentSpeed = comboFastSpeed.SelectedIndex + 1;
        }
        private void btn_PlayBack_Click(object sender, EventArgs e)
        {
            uint[] channels = new uint[1];
            channels[0] = channelIndex;
            //channels[0] = 10;
            //channels[1] = channelIndex + 2;
            IntPtr[] ptrs = new IntPtr[1];
            ptrs[0] = panel1.Handle;
            //ptrs[1] = panel2.Handle;

            if (!isPlay)//not playback
            {
                m_playbackHandle = DevSdkHelper.NET_SDK_PlayBackByTime(userId, channels, 1, ref startTime, ref endTime, ptrs);
                //m_playbackHandle = DevSdkHelper.NET_SDK_PlayBackByTimeEx(userId, channels, 1, ref startTime, ref endTime, null, false);
                if (m_playbackHandle > 0)
                {
                    updateUI_Playing();
                    WaveFormat whdr1 = new WaveFormat(8000, 16, 1);
                    m_waveOut.Start(whdr1);
                    //if(pdc == null)
                        pdc = PLayCBFun;
                        yuvpdc = PlayYuvCBFun;
                    DevSdkHelper.NET_SDK_SetPlayDataCallBack(m_playbackHandle, pdc, IntPtr.Zero);
                    DevSdkHelper.NET_SDK_SetPlayYUVCallBack(m_playbackHandle, yuvpdc, IntPtr.Zero);
                    panel1.ContextMenuStrip = audioOpened;
                    currentSpeed = (int)NET_SDK_RPB_SPEED.NET_SDK_RPB_SPEED_1X;
                    UpdateSpeedDisplay();
                    StartTimer();
                }
                else
                {
                    string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else 
            {
                m_waveOut.Stop();
                if (!isPause)//playing
                {
                    bool ret = false;
                    lock (this)
                    {
                        ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_PAUSE), 0, null);
                    }
                    
                    if (ret)
                    {
                        //DevSdkHelper.NET_SDK_SetPlayDataCallBack(m_playbackHandle, null, IntPtr.Zero);
                        updateUI_Paused();                       
                    }
                    else
                    {
                        string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                        MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else//pauseed
                {
                    lock (this)
                    {
                        DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_RESUME), currentSpeed, null);
                    }
                    
                    //DevSdkHelper.NET_SDK_SetPlayDataCallBack(m_playbackHandle, pdc, IntPtr.Zero);
                    updateUI_Playing();
                }
            }
            
            

        }
        private void StartTimer()
        {
            oTimer = new System.Timers.Timer(1000);
            oTimer.AutoReset = true;
            oTimer.Enabled = true;
            oTimer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>
                {
                    lock (this)
                    {
                        if (m_playbackHandle > 0)
                        {
                            DD_TIME t = new DD_TIME();
                            bool ret = DevSdkHelper.NET_SDK_GetPlayBackOsdTime(m_playbackHandle, ref t);
                            if (ret)
                            {
                                int year = t.year + 1900;
                                int month = t.month + 1;
                                int day = t.mday;
                                int hour = t.hour;
                                int minute = t.minute;
                                int second = t.second;
                                //sc.Post(UpdateTimeDisplay, t.hour.ToString() + ":" + t.minute.ToString() + ":" + t.second.ToString());
                                sc.Post(UpdateTimeDisplay, t);
                            }
                        }            
                    }
                                                                                      
                });
        }
        private void StopTimer()
        {
            oTimer.Stop();
            oTimer.Enabled = false;
            label2.Text = "";
        }
        private void UpdateTimeDisplay(object obj)
        {
            DD_TIME t = (DD_TIME)obj;
            DateTime currentDt = new DateTime(t.year + 1900, t.month + 1, t.mday, t.hour, t.minute, t.second);          
            DateTime startDt = new DateTime(startTime.year + 1900, startTime.month + 1, startTime.mday, startTime.hour, startTime.minute, startTime.second);
            DateTime endDt = new DateTime(endTime.year + 1900, endTime.month + 1, endTime.mday, endTime.hour, endTime.minute, endTime.second);
            label2.Text = t.hour.ToString() + ":" + t.minute.ToString() + ":" + t.second.ToString();
            int ct = Convert.ToInt32(currentDt.Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
            int st = Convert.ToInt32(startDt.Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
            int et = Convert.ToInt32(endDt.Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
            if (ct < et)
            {
                if (ct > st)
                {
                    int m_playbackPos = ((ct - st) * 100) / (et - st);
                    tkBar_PlayPos.Value = m_playbackPos;
                }               
            }
            else
            {
                btn_Stop_Click(null, null);
            }
        }
        private void updateUI_Paused()
        {
            isPlay = true;
            isPause = true;
            btn_PlayBack.Text = ">";
            btn_Stop.Enabled = true;
            btn_Rew.Enabled = false;
            btn_Ff.Enabled = false;
            btn_Frame.Enabled = true;
            tkBar_Volume.Enabled = true;

            btn_PlaybackTitle.Enabled = false;
            btn_PlaybackChlName.Enabled = false;
            btn_PlaybackTime.Enabled = false;
            //btn_PlaybackTitle.Text = "Title Open";
            //btn_PlaybackChlName.Text = "ChlName Open";
            //btn_PlaybackTime.Text = "Time Open";
        }
        private void updateUI_Playing()
        {
            isPlay = true;
            isPause = false;
            btn_PlayBack.Text = "||";
            btn_Stop.Enabled = true;
            btn_Rew.Enabled = true;
            btn_Ff.Enabled = true;
            btn_Frame.Enabled = false;
            tkBar_Volume.Enabled = true;

            btn_PlaybackTitle.Enabled = true;
            btn_PlaybackChlName.Enabled = true;
            btn_PlaybackTime.Enabled = true;
            btn_PlaybackTitle.Text = "Title Open";
            btn_PlaybackChlName.Text = "ChlName Open";
            btn_PlaybackTime.Text = "Time Open";
        }
        private void updateUI_Stoped()
        {
            isPlay = false;
            isPause = false;
            btn_PlayBack.Text = ">";
            btn_Stop.Enabled = false;
            btn_Rew.Enabled = false;
            btn_Ff.Enabled = false;
            btn_Frame.Enabled = false;
            tkBar_Volume.Enabled = false;
            tkBar_PlayPos.Value = 0;

            btn_PlaybackTitle.Enabled = false;
            btn_PlaybackChlName.Enabled = false;
            btn_PlaybackTime.Enabled = false;
            btn_PlaybackTitle.Text = "Title Open";
            btn_PlaybackChlName.Text = "ChlName Open";
            btn_PlaybackTime.Text = "Time Open";
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_STOP), 0, null);
                if (ret)
                {
                    if (DevSdkHelper.NET_SDK_StopPlayBack(m_playbackHandle))
                    {
                        m_waveOut.Stop();
                        DevSdkHelper.NET_SDK_SetPlayDataCallBack(m_playbackHandle, null, IntPtr.Zero);
                        streamFormat = "";
                        updateUI_Stoped();
                        m_playbackHandle = -1;
                        panel1.BackColor = Color.Transparent;
                        panel1.BackColor = Color.Gray;
                        panel1.ContextMenuStrip = null;
                        StopTimer();
                    }
                }
            }
            
                
                          
        }
        private void PLayCBFun(int lLiveHandle, NET_SDK_FRAME_INFO frameInfo, IntPtr pBuffer, IntPtr pUser)
        {
            //if (frameInfo.frameType == 5)//video format frame
            //{
            //    //if (frameInfo.length >= Marshal.SizeOf(typeof(BITMAPINFOHEADER)))
            //    {
            //        BITMAPINFOHEADER bitmapinfo = (BITMAPINFOHEADER)Marshal.PtrToStructure(pBuffer, typeof(BITMAPINFOHEADER));
            //        if (DevSdkHelper.GetbiCompression(bitmapinfo.biCompression) == "H264")
            //        {
            //            streamFormat = "Current playing stream format is H264";
            //            this.Invoke(new Action(() =>
            //            {
            //                lb_format.Text = "Current playing stream format is H264";
            //            }));
            //            Debug.Print("is H264 \n");
            //        }

            //        else if (DevSdkHelper.GetbiCompression(bitmapinfo.biCompression) == "HEVC")
            //        {
            //            streamFormat = "Current playing stream format is H265";
            //            this.Invoke(new Action(() =>
            //            {
            //                lb_format.Text = "Current playing stream format is H265";
            //            }));
            //            Debug.Print("is H265 \n"); 
            //        }
                        
            //    }
            //}

            if (frameInfo.frameType == 6)//audio format frame
            {
                PBAudioDecHandle = DevSdkHelper.NET_SDK_InitAudioDecoder(pBuffer, frameInfo.length);
            }
            else if (frameInfo.frameType == 2)//audio frame
            {
                if (PBAudioOutBuffer != null)
		        {
			        PBAudioOutBuffer = Marshal.AllocHGlobal(3200);;
                    //IntPtr[] ptrs = new IntPtr[3200];
		        }
                int outLen = 0;
                DevSdkHelper.NET_SDK_DecodeAudioFrame(PBAudioDecHandle, pBuffer, frameInfo.length, PBAudioOutBuffer, ref outLen);
                m_waveOut.WriteBuf(PBAudioOutBuffer, outLen);
            }
            //if (frameInfo.frameType == (uint)(DD_FRAME_TYPE.DD_FRAME_TYPE_END))
            //{
            //    //录像回放结束
            //    Console.WriteLine("录像回放结束,回放句柄lLiveHandle");
            //}
        }

        private void PlayYuvCBFun(int lLiveHandle, DECODE_FRAME_INFO frameInfo,IntPtr pUser)
        {
            int i = 0;
            Debug.Print("is playback yuv data \n"); 
            //if (frameInfo.frameType == 5)//video format frame
            //{
            //    //if (frameInfo.length >= Marshal.SizeOf(typeof(BITMAPINFOHEADER)))
            //    {
            //        BITMAPINFOHEADER bitmapinfo = (BITMAPINFOHEADER)Marshal.PtrToStructure(pBuffer, typeof(BITMAPINFOHEADER));
            //        if (DevSdkHelper.GetbiCompression(bitmapinfo.biCompression) == "H264")
            //        {
            //            streamFormat = "Current playing stream format is H264";
            //            this.Invoke(new Action(() =>
            //            {
            //                lb_format.Text = "Current playing stream format is H264";
            //            }));
            //            Debug.Print("is H264 \n");
            //        }

            //        else if (DevSdkHelper.GetbiCompression(bitmapinfo.biCompression) == "HEVC")
            //        {
            //            streamFormat = "Current playing stream format is H265";
            //            this.Invoke(new Action(() =>
            //            {
            //                lb_format.Text = "Current playing stream format is H265";
            //            }));
            //            Debug.Print("is H265 \n");
            //        }

            //    }
            //}
            //if (frameInfo.frameType == (uint)(DD_FRAME_TYPE.DD_FRAME_TYPE_END))
            //{
            //    //录像回放结束
            //    Console.WriteLine("录像回放结束,回放句柄lLiveHandle");
            //}
        }

        private void btn_PlayBackSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (((Button)sender).Text == "Save")               
                {
                    saveFileDialog1.Filter = "avi files(*.avi)|*.avi||";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string savePath = saveFileDialog1.FileName;
                        bool ret = false;
                        lock (this)
                        {
                            ret = DevSdkHelper.NET_SDK_PlayBackSaveData(m_playbackHandle,(int)channelIndex, savePath);
                        }
                        if (ret)
                        {
                            ((Button)sender).Text = "Stop";                           
                        }
                        else
                        {
                            string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                            MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }

                }
                else
                {
                    bool ret = false;
                    lock (this)
                    {
                        ret = DevSdkHelper.NET_SDK_StopPlayBackSave(m_playbackHandle, (int)channelIndex);
                    }
                    if (ret)
                    {
                        ((Button)sender).Text = "Save";
                        
                    }
                    else
                    {
                        string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                        MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ContextMenuInit()
        {
            audioOpened = new ContextMenuStrip();

            openAudio = new ToolStripMenuItem("Start audio");
            disableAudio = new ToolStripMenuItem("Open audio");
            openDraw = new ToolStripMenuItem("Open draw");
            snap = new ToolStripMenuItem("Snap");

            openAudio.Click += ToolStripMenuItem_StartAudio_Click;
            openDraw.Click += ToolStripMenuItem_OpenDraw_Click;
            snap.Click += ToolStripMenuItem_Snap_Click;


            disableAudio.Enabled = false;

            audioOpened.Items.Add(openAudio);
            audioOpened.Items.Add(snap);
            audioOpened.Items.Add(openDraw);
        }
        private void ToolStripMenuItem_StartAudio_Click(object sender, EventArgs e)
        {
            try
            {
                IntPtr cc = ((Panel)(((ContextMenuStrip)(((ToolStripMenuItem)sender).GetCurrentParent())).SourceControl)).Handle;
                currentPanelHandle = cc;
                if (Control.FromHandle(currentPanelHandle).ContextMenuStrip.Items[0].Text == "Start audio")
                {
                    bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_STARTAUDIO), currentSpeed, null);
                    if (ret)
                    {
                        tkBar_Volume.Enabled = true;
                        Control.FromHandle(currentPanelHandle).ContextMenuStrip.Items[0].Text = "Stop audio";
                    }
                    else
                    {
                        string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                        MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_STOPAUDIO), currentSpeed, null);
                    if (ret)
                    {
                        tkBar_Volume.Enabled = true;
                        Control.FromHandle(currentPanelHandle).ContextMenuStrip.Items[0].Text = "Start audio";
                    }
                    else
                    {
                        string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                        MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void ToolStripMenuItem_OpenDraw_Click(object sender, EventArgs e)
        {
            try
            {
                IntPtr cc = ((Panel)(((ContextMenuStrip)(((ToolStripMenuItem)sender).GetCurrentParent())).SourceControl)).Handle;
                currentPanelHandle = cc;
                if (Control.FromHandle(currentPanelHandle).ContextMenuStrip.Items[2].Text == "Open draw")
                {
                    dfc = OsdDisplay;
                    DevSdkHelper.NET_SDK_RegisterDrawFun(m_playbackHandle, dfc, currentPanelHandle);
                    Control.FromHandle(currentPanelHandle).ContextMenuStrip.Items[2].Text = "Close draw";
                }
                else
                {
                    DevSdkHelper.NET_SDK_RegisterDrawFun(m_playbackHandle, null, currentPanelHandle);
                    Control.FromHandle(currentPanelHandle).ContextMenuStrip.Items[2].Text = "Open draw";
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void OsdDisplay(int lLiveHandle, IntPtr hDC, IntPtr pUser)
        {
            Font drawFont = new Font("Arial", 18);
            Graphics gh = Graphics.FromHdc(hDC);
            SolidBrush drawBrush = new SolidBrush(Color.Red);
            Point a = new Point(10, 10);
            gh.DrawString(streamFormat, drawFont, drawBrush, a);
        }
        private void ToolStripMenuItem_Snap_Click(object sender, EventArgs e)
        { 
            saveFileDialog1.Filter = "bmp files(*.bmp)|*.bmp||";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string savePath = saveFileDialog1.FileName;
                bool ret = DevSdkHelper.NET_SDK_PlayBackCaptureFile(m_playbackHandle, (int)channelIndex, savePath);
                if (!ret)
                {
                    string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        private void tkBar_PlayPos_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_playbackHandle > 0)
            {
                DateTime dt0 = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

                DateTime dtStart = new DateTime(startTime.year + 1900, startTime.month + 1, startTime.mday, startTime.hour, startTime.minute, startTime.second);
                TimeSpan tsStart = new TimeSpan(dtStart.Ticks - dt0.Ticks);
                double nStart = tsStart.TotalSeconds;

                DateTime dtEnd = new DateTime(endTime.year + 1900, endTime.month + 1, endTime.mday, endTime.hour, endTime.minute, endTime.second);
                TimeSpan tsEnd = new TimeSpan(dtEnd.Ticks - dt0.Ticks);
                double nEnd = tsEnd.TotalSeconds;
                int nPos = Convert.ToInt32(tkBar_PlayPos.Value.ToString());
                double nPosTime = ((nEnd - nStart) * nPos) / 100 + nStart;
                bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_SETPOS), (int)nPosTime, null);
                if (ret)
                {
                    Console.WriteLine("set pos success");
                }
                else
                {
                    Console.WriteLine("set pos fail");
                }
            }
        }

        private void btn_Slow_Click(object sender, EventArgs e)
        {
            if (currentSpeed > (int)NET_SDK_RPB_SPEED.NET_SDK_RPB_SPEED_1_32X)
            {
                currentSpeed--;
                int forwardOrBack;
                if (forward)
                {
                    forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_FF);
                }
                else
                {
                    forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_REW);
                }
                bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, forwardOrBack, currentSpeed, null);
                if (!ret)
                    currentSpeed++;
                UpdateSpeedDisplay();
            }
               
        }

        private void btn_Fast_Click(object sender, EventArgs e)
        {
            if (currentSpeed < (int)NET_SDK_RPB_SPEED.NET_SDK_RPB_SPEED_32X)
            {
                currentSpeed++;
                int forwardOrBack;
                if (forward)
                {
                    forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_FF);
                }
                else
                {
                    forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_REW);
                }
                bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, forwardOrBack, currentSpeed, null);
                if (!ret)
                    currentSpeed--;
                UpdateSpeedDisplay();
            }
        }
        private void UpdateSpeedDisplay()
        {
            this.comboFastSpeed.SelectedIndex = currentSpeed - 1;
            switch (currentSpeed)
            {
                case 1:
                    lb_Speed.Text = "1/32X";
                    break;
                case 2:
                    lb_Speed.Text = "1/16X";
                    break;
                case 3:
                    lb_Speed.Text = "1/8X";
                    break;
                case 4:
                    lb_Speed.Text = "1/4X";
                    break;
                case 5:
                    lb_Speed.Text = "1/2X";
                    break;
                case 6:
                    lb_Speed.Text = "1X";
                    break;
                case 7:
                    lb_Speed.Text = "2X";
                    break;
                case 8:
                    lb_Speed.Text = "4X";
                    break;
                case 9:
                    lb_Speed.Text = "8X";
                    break;
                case 10:
                    lb_Speed.Text = "16X";
                    break;
                case 11:
                    lb_Speed.Text = "32X";
                    break;
            }
        }

        private void btn_Ff_Click(object sender, EventArgs e)
        {
            forward = true;
            int forwardOrBack;
            if (forward)
            {
                forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_FF);
            }
            else
            {
                forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_REW);
            }
            bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, forwardOrBack, currentSpeed, null);
        }

        private void btn_Rew_Click(object sender, EventArgs e)
        {
            forward = false;
            int forwardOrBack;
            if (forward)
            {
                forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_FF);
            }
            else
            {
                forwardOrBack = (int)(NET_SDK_PLAYCTRL_TYPE.NET_SDK_PLAYCTRL_REW);
            }
            bool ret = DevSdkHelper.NET_SDK_PlayBackControl(m_playbackHandle, forwardOrBack, currentSpeed, null);
        }

        private void PlayBack_Dialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //StopTimer();
                //DevSdkHelper.NET_SDK_StopPlayBack(m_playbackHandle);
                btn_Stop_Click(null, null);
                m_playbackHandle = -1;
                DevSdkHelper.NET_SDK_SetPlayDataCallBack(m_playbackHandle, null, IntPtr.Zero);
                streamFormat = "";
            }
            catch (System.Exception ex)
            {

            }
        }

        private void OsdPlayBackDisplay(int lLiveHandle, IntPtr hDC, IntPtr pUser)
        {
            //string txt = textBox_Title.Text;
            //指定字体
            Font drawFont = new Font("Arial", 12);
            Graphics gh = Graphics.FromHdc(hDC);
            SolidBrush drawBrush = new SolidBrush(Color.Yellow);
            if (osdPlayBackTitle)
            {
                Point a = new Point(5, 20);
                gh.DrawString(textBox_PlaybackTitle.Text, drawFont, drawBrush, a);
            }
            if (osdPlayBackChlName)
            {
                Point b = new Point(5, 40);
                gh.DrawString("channel:1", drawFont, drawBrush, b);
            }
            if (osdPlayBackTime)
            {
                Point c = new Point(5, 60);
                gh.DrawString(DateTime.Now.ToString(), drawFont, drawBrush, c);
            }

            //Point b = new Point(50, 50);
            //gh.DrawString("lala", drawFont, drawBrush, b);
            //IntPtr last_font = SelectObject(hDC, drawFont.ToHfont());
            //TextOut(hDC, 10, 10, txt, txt.Length);
        }

        private void btn_PlaybackTitle_Click(object sender, EventArgs e)
        {
            if (btn_PlaybackTitle.Text.Equals("Title Open"))
            {
                osdPlayBackTitle = true;
                if (playbackdfc == null)
                    playbackdfc = new DRAW_FUN_CALLBACK(OsdPlayBackDisplay);
                bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(m_playbackHandle, playbackdfc, currentPanelHandle);
                //btn_OsdTitle.ResetText("Title close");
                btn_PlaybackTitle.Text = "Title Close";
            }
            else if (btn_PlaybackTitle.Text.Equals("Title Close"))
            {
                //bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(0, null, currentPanelHandle);
                osdPlayBackTitle = false;
                btn_PlaybackTitle.Text = "Title Open";
            }
            return;
        }

        private void btn_PlaybackChlName_Click(object sender, EventArgs e)
        {
            if (btn_PlaybackChlName.Text.Equals("ChlName Open"))
            {
                osdPlayBackChlName = true;
                if (playbackdfc == null)
                    playbackdfc = new DRAW_FUN_CALLBACK(OsdPlayBackDisplay);
                bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(m_playbackHandle, playbackdfc, currentPanelHandle);
                //btn_OsdTitle.ResetText("Title close");
                btn_PlaybackChlName.Text = "ChlName Close";
            }
            else if (btn_PlaybackChlName.Text.Equals("ChlName Close"))
            {
                //bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(0, null, currentPanelHandle);
                osdPlayBackChlName = false;
                btn_PlaybackChlName.Text = "ChlName Open";
            }
        }

        private void btn_PlaybackTime_Click(object sender, EventArgs e)
        {
            if (btn_PlaybackTime.Text.Equals("Time Open"))
            {
                osdPlayBackTime = true;
                if (playbackdfc == null)
                    playbackdfc = new DRAW_FUN_CALLBACK(OsdPlayBackDisplay);
                bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(m_playbackHandle, playbackdfc, currentPanelHandle);
                //btn_OsdTitle.ResetText("Title close");
                btn_PlaybackTime.Text = "Time Close";
            }
            else if (btn_PlaybackTime.Text.Equals("Time Close"))
            {
                //bool ret1 = DevSdkHelper.NET_SDK_RegisterDrawFun(0, null, currentPanelHandle);
                osdPlayBackTime = false;
                btn_PlaybackTime.Text = "Time Open";
            }
        }

       

    }
}
