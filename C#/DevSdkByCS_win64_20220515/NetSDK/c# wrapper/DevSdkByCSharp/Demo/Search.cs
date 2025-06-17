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
using System.Timers;
using System.Xml;
namespace Demo
{
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    partial class Live_Dialog
    {
        private Dictionary<int,NET_SDK_REC_FILE> m_fileList = new Dictionary<int,NET_SDK_REC_FILE> ();
        private Dictionary<int, NET_SDK_REC_EVENT> m_eventList = new Dictionary<int, NET_SDK_REC_EVENT>();
        private Dictionary<int, NET_SDK_REC_TIME> m_timeList = new Dictionary<int, NET_SDK_REC_TIME>();
        private Dictionary<int, NET_SDK_IMAGE> m_pictureList = new Dictionary<int, NET_SDK_IMAGE>();
        private int searchType;
        private string m_backPath;
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {           
            CloseAllLive();
            int channelNum = oNET_SDK_DEVICEINFO.videoInputNum;
            if (channelNum > 0)
            {
                cmb_CHNN.Items.Clear();
                for (int i = 0; i < channelNum; i++)
                {
                    cmb_CHNN.Items.Add("Channel:" + (i + 1));
                }
            }
            cmb_CHNN.SelectedIndex = 0;
        }
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchType = tabControl2.SelectedIndex;
            switch (tabControl2.SelectedIndex)
            {
                case 0:                   
                    gpb_LockFile.Visible = true;
                    gpb_EventType.Visible = false;
                    gpb_Backup.Visible = true;
                    cmb_CHNN.Visible = true;
                    label40.Text = "Tips:Double click the searched item to playback";
                    pb_SnapVehicle.Visible = false;
                    pb_SourceVehicle.Visible = false;
                    cb_DisplaySource.Visible = false;
                    cmb_VehicleListType.Visible = false;
                    tb_PlateNo.Visible = false;
                    label43.Visible = false;
                    label44.Visible = false;

                    break;
                case 1:
                    gpb_LockFile.Visible = false;
                    gpb_EventType.Visible = true;
                    gpb_Backup.Visible = false;
                    cmb_CHNN.Visible = true;
                    label40.Text = "Tips:Double click the searched item to playback";
                    pb_SnapVehicle.Visible = false;
                    pb_SourceVehicle.Visible = false;
                    cb_DisplaySource.Visible = false;
                    cmb_VehicleListType.Visible = false;
                    tb_PlateNo.Visible = false;
                    label43.Visible = false;
                    label44.Visible = false;
                    break;
                case 2:
                    gpb_LockFile.Visible = false;
                    gpb_EventType.Visible = false;
                    gpb_Backup.Visible = false;
                    cmb_CHNN.Visible = true;
                    label40.Text = "Tips:Double click the searched item to playback";
                    pb_SnapVehicle.Visible = false;
                    pb_SourceVehicle.Visible = false;
                    cb_DisplaySource.Visible = false;
                    cmb_VehicleListType.Visible = false;
                    tb_PlateNo.Visible = false;
                    label43.Visible = false;
                    label44.Visible = false;
                    break;
                case 3:
                    gpb_LockFile.Visible = false;
                    gpb_EventType.Visible = false;
                    gpb_Backup.Visible = false;
                    cmb_CHNN.Visible = true;
                    label40.Text = "";
                    pb_SnapVehicle.Visible = false;
                    pb_SourceVehicle.Visible = false;
                    cb_DisplaySource.Visible = false;
                    cmb_VehicleListType.Visible = false;
                    tb_PlateNo.Visible = false;
                    label43.Visible = false;
                    label44.Visible = false;
                    break;
                case 4:
                    gpb_LockFile.Visible = false;
                    gpb_EventType.Visible = false;
                    gpb_Backup.Visible = false;
                    cmb_CHNN.Visible = true;
                    label40.Text = "Tips:Double click the searched item to get the picture";
                    pb_SnapVehicle.Visible = false;
                    pb_SourceVehicle.Visible = false;
                    cb_DisplaySource.Visible = false;
                    cmb_VehicleListType.Visible = false;
                    tb_PlateNo.Visible = false;
                    label43.Visible = false;
                    label44.Visible = false;
                    break;
                case 5:
                    gpb_LockFile.Visible = false;
                    gpb_EventType.Visible = false;
                    gpb_Backup.Visible = false;
                    label40.Text = "Tips:Double click the searched item to get the picture";
                    cmb_CHNN.Visible = false;
                    pb_SnapVehicle.Visible = true;
                    pb_SourceVehicle.Visible = true;
                    cb_DisplaySource.Visible = true;
                    cmb_VehicleListType.Visible = true;
                    tb_PlateNo.Visible = true;
                    label43.Visible = true;
                    label44.Visible = true;
                    cmb_VehicleListType.SelectedIndex = 0;
                    break;
            }

        }
        private void btn_Search_Click(object sender, EventArgs e)
        {
            DD_TIME st = new DD_TIME()
            {
                year = (ushort)(DateTime.Now.Year - 1900),
                month = Convert.ToByte(dtp_Date.Value.Month - 1),
                mday = Convert.ToByte(dtp_Date.Value.Day),
                hour = 0,
                minute = 0,
                second = 0
            };
            DD_TIME et = new DD_TIME()
            {
                year = (ushort)(DateTime.Now.Year - 1900),
                month = Convert.ToByte(dtp_Date.Value.Month - 1),
                mday = Convert.ToByte(dtp_Date.Value.Day),
                hour = 23,
                minute = 59,
                second = 59
            };
            int channel = cmb_CHNN.SelectedIndex;
            switch (searchType)
            {
                case 0:
                    SearchByFile(channel,st,et);
                    break;
                case 1:
                    SearchByEvent(channel, st, et);
                    break;
                case 2:
                    SearchByTime(channel, st, et);
                    break;
                case 3:
                    FindRecDate();
                    break;
                case 4:
                    SearchPictures((uint)channel, DDTIME2DDTIMEEX(st), DDTIME2DDTIMEEX(et));
                    break;
                case 5:
                    SearchIPCVehicle(dtp_Date.Value);
                    break;   
            }

        }

        private string[] ChangeToItems(NET_SDK_REC_FILE file)
        {
            string startTime = (file.startTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.startTime.month + 1)) + "-" + AddZero(file.startTime.mday) + " " +
                AddZero(file.startTime.hour) + ":" + AddZero(file.startTime.minute) + ":" + AddZero(file.startTime.second);
            string stopTime = (file.stopTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.stopTime.month + 1)) + "-" + AddZero(file.stopTime.mday) + " " +
                AddZero(file.stopTime.hour) + ":" + AddZero(file.stopTime.minute) + ":" + AddZero(file.stopTime.second);
            string locked = (file.bFileLocked == 0 ? "" : "lock");
            string[] tempStrings = { "", (file.dwChannel + 1).ToString(), startTime, stopTime, locked };
            return tempStrings;
        }
        private string AddZero(byte time)
        {
            int a = time;
            return a > 9 ? a.ToString() : "0" + a;
        }
        private void SearchByFile(int channel,DD_TIME st,DD_TIME et)
        {
            lv_File.Items.Clear();
            //Int64 fileHandle = DevSdkHelper.NET_SDK_FindFile(userId, channel, ref st, ref et);
            POINTERHANDLE fileHandle = DevSdkHelper.NET_SDK_FindFile(userId, channel, ref st, ref et);
            if (fileHandle > 0)
            {
                
                m_fileList.Clear();
                while (true)
                {
                    NET_SDK_REC_FILE file = new NET_SDK_REC_FILE();
                    POINTERHANDLE result = DevSdkHelper.NET_SDK_FindNextFile(fileHandle, ref file);
                    if (result == 87 || result == 88)
                        break;
                    if (result == 86)
                    {
                        MessageBox.Show("Find nothing");
                        break;
                    }
                    //if (file.startTime.nTotalseconds == 0 && file.stopTime.nTotalseconds == 0)
                    //    break;
                    ListViewItem item = new ListViewItem(ChangeToItems(file));                   
                    lv_File.Items.Add(item);
                    m_fileList.Add(lv_File.Items.Count, file);
                }
                DevSdkHelper.NET_SDK_FindClose(fileHandle);
            }
            else
            {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void SearchByEvent(int channel, DD_TIME st, DD_TIME et)
        {
            lv_Event.Items.Clear();
            uint m_eventType;
            if (rb_Motion.Checked)
                m_eventType = (uint)(DD_RECORD_TYPE.DD_RECORD_TYPE_MOTION);
            else if (rb_Sensor.Checked)
                m_eventType = (uint)(DD_RECORD_TYPE.DD_RECORD_TYPE_SENSOR);
            else
                m_eventType = 0xffff;

            POINTERHANDLE searchHandle = DevSdkHelper.NET_SDK_FindEvent(userId, channel, m_eventType, ref st, ref et);
            if (searchHandle > 0)
            {
                
                m_eventList.Clear();

                while (true)
                {
                    NET_SDK_REC_EVENT recEvent = new NET_SDK_REC_EVENT();
                    POINTERHANDLE result = DevSdkHelper.NET_SDK_FindNextEvent(searchHandle, ref recEvent);
                    if (result == 87 || result == 88)
                        break;
                    if (result == 86)
                    {
                        MessageBox.Show("Find nothing");
                        break;
                    }
                    //if(recEvent.startTime.nTotalseconds == 0 && recEvent.stopTime.nTotalseconds == 0 )
                    //    break;
                    ListViewItem item = new ListViewItem(ChangeToItems(recEvent));
                    lv_Event.Items.Add(item);
                    m_eventList.Add(lv_Event.Items.Count, recEvent);

                }
                DevSdkHelper.NET_SDK_FindEventClose(searchHandle);
            }
            else
            {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void SearchByTime(int channel, DD_TIME st, DD_TIME et)
        {
            lv_Time.Items.Clear();
            POINTERHANDLE searchHandle = DevSdkHelper.NET_SDK_FindTime(userId, channel, ref st, ref et);
		
		    if (searchHandle > 0)
		    {
                
                m_timeList.Clear();
			    while(true)
			    {
				    NET_SDK_REC_TIME time = new NET_SDK_REC_TIME ();
                    //ZeroMemory(&time, sizeof(NET_SDK_REC_TIME));
                    POINTERHANDLE result = DevSdkHelper.NET_SDK_FindNextTime(searchHandle, ref time);
                    if (result == 87 || result == 88)				
				        break;				
				    if (result == 86)
				    {
					    MessageBox.Show("Find nothing");
					    break;
				    }
                    //if (time.startTime.nTotalseconds == 0 && time.stopTime.nTotalseconds == 0)
                    //    break;
                    ListViewItem item = new ListViewItem(ChangeToItems(time));
                    lv_Time.Items.Add(item);
                    m_timeList.Add(lv_Time.Items.Count, time);
               				
			    }
                DevSdkHelper.NET_SDK_FindTimeClose(searchHandle);
		    }
		    else
		    {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		    }
        }
        private string[] ChangeToItems(NET_SDK_REC_TIME file)
        {
            string startTime = (file.startTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.startTime.month + 1)) + "-" + AddZero(file.startTime.mday) + " " +
                    AddZero(file.startTime.hour) + ":" + AddZero(file.startTime.minute) + ":" + AddZero(file.startTime.second);
            string stopTime = (file.stopTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.stopTime.month + 1)) + "-" + AddZero(file.stopTime.mday) + " " +
                AddZero(file.stopTime.hour) + ":" + AddZero(file.stopTime.minute) + ":" + AddZero(file.stopTime.second);
            string[] tempStrings = {(file.dwChannel + 1).ToString(), startTime, stopTime};
            return tempStrings;
        }
        private string[] ChangeToItems(NET_SDK_REC_EVENT file)
        {
            string startTime = (file.startTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.startTime.month + 1)) + "-" + AddZero(file.startTime.mday) + " " +
                AddZero(file.startTime.hour) + ":" + AddZero(file.startTime.minute) + ":" + AddZero(file.startTime.second);
            string stopTime = (file.stopTime.year + 1900) + "-" + AddZero(Convert.ToByte(file.stopTime.month + 1)) + "-" + AddZero(file.stopTime.mday) + " " +
                AddZero(file.stopTime.hour) + ":" + AddZero(file.stopTime.minute) + ":" + AddZero(file.stopTime.second);
            string recType = "";
            switch (file.dwRecType)
            {
                case 1:
                    recType = "Manual Record";
                    break;
                case 2:
                    recType = "Schedule Record";
                    break;
                case 4:
                    recType = "Motion Record";
                    break;
                case 8:
                    recType = "Sensor Record";
                    break;
            }

            string[] tempStrings = { (file.dwChannel + 1).ToString(), startTime, stopTime, recType };
            return tempStrings;
        }
        private void FindRecDate()
        {
            lv_Date.Items.Clear();
            POINTERHANDLE handle = DevSdkHelper.NET_SDK_FindRecDate(userId);
            if (handle > 0)
            {
                
                while (true)
                {
                    DD_DATE date = new DD_DATE();
                    POINTERHANDLE result = DevSdkHelper.NET_SDK_FindNextRecDate(handle, ref date);
                    if (result == 87 || result == 88)                   
                        break;
                    if (result == 86)
                    {
                        MessageBox.Show("Find nothing");
                        break;
                    }
                    ListViewItem item = new ListViewItem(ChangeToItems(date));
                    lv_Date.Items.Add(item);
                   
                }
                DevSdkHelper.NET_SDK_FindRecDateClose(handle);
            }
        }
        private void SearchIPCVehicle(DateTime dt)
        {
            if (oNET_SDK_DEVICEINFO.deviceType != 2)
                return;
            string m_sendUrl = "SearchSnapVehicleByTime";
            string m_sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                <config><search>
                                <starttime type='string'><![CDATA[{0}]]></starttime>
                                <endtime type='string'><![CDATA[{1}]]></endtime>                                
                                <vehiclePlate type='string'><![CDATA[{2}]]></vehiclePlate>
                                <listType type='listType'>{3}</listType>
                                </search></config>";
            string startTime = dt.Year + "-" + dt.Month + "-" + dt.Day + " 00:00:00";
            string endTime = dt.Year + "-" + dt.Month + "-" + (dt.Day+1) + " 00:00:00";
            m_sendXml = string.Format(m_sendXml, startTime, endTime,tb_PlateNo.Text.Trim(),cmb_VehicleListType.SelectedItem.ToString());

            int lpBytesReturned = 0;
            
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, m_sendXml, m_sendUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret && lpBytesReturned > 0)
            {
                lv_IPCVehicle.Items.Clear();
                byte[] rec = new byte[lpBytesReturned];
                Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                string retStr = DevSdkHelper.ByteToStr(rec);
                if (retStr.Length > 0)
                {
                    if (!retStr.Contains("failed"))
                    {
                        XmlDocument oldXd = new XmlDocument();
                        oldXd.LoadXml(retStr);
                        XmlNodeList nodeList = oldXd.GetElementsByTagName("item");
                        if (nodeList.Count > 0)
                        {

                            for (int i = 0; i < nodeList.Count; i++)
                            {
                                string[] tempStrs = { nodeList[i].ChildNodes[1].InnerText, nodeList[i].ChildNodes[0].InnerText, nodeList[i].ChildNodes[2].InnerText };
                                ListViewItem item = new ListViewItem(tempStrs);
                                lv_IPCVehicle.Items.Add(item);
                            }
                        }
                    }
                    
                }
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }
        private void SearchPictures(uint channel, DD_TIME_EX st, DD_TIME_EX et)
        {
            lv_pics.Items.Clear();
            NET_SDK_IMAGE_SREACH sInSreachImage = new NET_SDK_IMAGE_SREACH ();
            sInSreachImage.dwChannel = channel;
            //st.year += 1900;
            //st.month += 1;
            //et.year += 1900;
            //et.month += 1;
            sInSreachImage.StartTime = st;
            sInSreachImage.StopTime = et;
            sInSreachImage.pageSize = 10;
            sInSreachImage.pageIndex = 1;
            sInSreachImage.sort = IMAGE_SORT_TYPE.IMAGE_SORT_DESC;

            NET_SDK_IMAGE[] pOutImage = new NET_SDK_IMAGE[10];
            Int32 pOutImageNum = 0;
            int singleBufferSize = Marshal.SizeOf(typeof(NET_SDK_IMAGE));
            int bufferSize = singleBufferSize * 10;
            IntPtr intPtrPout = PointArrayToIntPtr(pOutImage);
            IntPtr pTmp = intPtrPout;
            bool ret = DevSdkHelper.NET_SDK_SearchPictures(userId, sInSreachImage, bufferSize, intPtrPout, ref pOutImageNum);
            if (ret)
            {
                
                m_pictureList.Clear();
                for (int i = 0; i < pOutImageNum; i++, pTmp = IntPtr.Add(pTmp, singleBufferSize))
                {
                    byte[] pByte = new byte[singleBufferSize];
                    Marshal.Copy(pTmp, pByte, 0, singleBufferSize);
                    pOutImage[i] = (NET_SDK_IMAGE)BytesToStruct(pByte, typeof(NET_SDK_IMAGE));

                    ListViewItem item = new ListViewItem(ChangeToItems(pOutImage[i]));
                    lv_pics.Items.Add(item);
                    m_pictureList.Add(lv_pics.Items.Count, pOutImage[i]);
                }
            }

            else
            {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            Marshal.FreeHGlobal(intPtrPout);
            //Marshal.FreeHGlobal(pTmp);
                 

        }
        
        private IntPtr PointArrayToIntPtr(NET_SDK_IMAGE[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(NET_SDK_IMAGE));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }
        private string[] ChangeToItems(NET_SDK_IMAGE file)
        {
            string captureTime = (file.captureTime.year) + "-" + AddZero(Convert.ToByte(file.captureTime.month)) + "-" + AddZero(file.captureTime.mday) + " " +
                AddZero(file.captureTime.hour) + ":" + AddZero(file.captureTime.minute) + ":" + AddZero(file.captureTime.second);
            string capType = file.dwImageType == 1 ? "Manual" : "Alarm";
            string[] tempStrings = { (file.dwChannel + 1).ToString(), capType, captureTime };
            return tempStrings;
        }
        private string[] ChangeToItems(DD_DATE file)
        {
            string[] tempStrings = { file.year.ToString(), AddZero(file.month), AddZero(file.mday) };
            return tempStrings;
        }
        
        private void lv_File_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_File.HitTest(e.X, e.Y);
            if (info != null)
            {
                var videoitem = info.Item as ListViewItem;

                PlayBack_Dialog pd = new PlayBack_Dialog(userId, m_fileList[videoitem.Index + 1].startTime, m_fileList[videoitem.Index + 1].stopTime, m_fileList[videoitem.Index + 1].dwChannel);               
                pd.ShowDialog();

            }
        }
        private void lv_Event_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_Event.HitTest(e.X, e.Y);
            if (info != null)
            {
                var videoitem = info.Item as ListViewItem;

                PlayBack_Dialog pd = new PlayBack_Dialog(userId, m_eventList[videoitem.Index + 1].startTime, m_eventList[videoitem.Index + 1].stopTime, m_eventList[videoitem.Index + 1].dwChannel);
                pd.ShowDialog();

            }
        }

        private void lv_Time_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_Time.HitTest(e.X, e.Y);
            if (info != null)
            {
                var videoitem = info.Item as ListViewItem;

                PlayBack_Dialog pd = new PlayBack_Dialog(userId, m_timeList[videoitem.Index + 1].startTime, m_timeList[videoitem.Index + 1].stopTime, m_timeList[videoitem.Index + 1].dwChannel);
                pd.ShowDialog();

            }
        }
        private void lv_pics_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_pics.HitTest(e.X, e.Y);
            if (info != null)
            {

                saveFileDialog1.Filter = "bmp files(*.jpg)|*.jpg||";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string savePath = saveFileDialog1.FileName;
                    var videoitem = info.Item as ListViewItem;
                    NET_SDK_IMAGE img = m_pictureList[videoitem.Index + 1];

                    NET_SDK_IMAGE_INFO pOutImageInfo = new NET_SDK_IMAGE_INFO();
                    int outBufferSize = 2 * 1024 * 1024;
                    IntPtr tempIntPtr = Marshal.AllocHGlobal(outBufferSize);

                    bool ret = DevSdkHelper.NET_SDK_DownLoadPicture(userId, img, ref pOutImageInfo, tempIntPtr, outBufferSize);
                    if (ret)
                    {
                        byte[] data = new byte[outBufferSize];
                        Marshal.Copy(tempIntPtr, data, 0, outBufferSize);
                        Marshal.FreeHGlobal(tempIntPtr);
                        if (File.Exists(savePath))
                            File.Delete(savePath);
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(data, 0, data.Length);
                        fs.Flush();//数据写入图片文件
                        fs.Close();
                    }

                }
                            
            }
        }
        private void btn_Lock_Click(object sender, EventArgs e)
        {
                        
        }
        private void btn_Unlock_Click(object sender, EventArgs e)
        {
                        
        }
        private void btn_Delete_Click(object sender, EventArgs e)
        {
                       
        }
        private void btn_Browser_Click(object sender, EventArgs e)
        {            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tb_BackupPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void btn_Backup_Click(object sender, EventArgs e)
        {
            m_backPath = tb_BackupPath.Text;
            if (m_backPath.Length == 0)
            {
                MessageBox.Show("Path can't be empty!");
                return;
            }
            List<NET_SDK_REC_FILE> temp = new List<NET_SDK_REC_FILE>();
            for (int i = 0; i < lv_File.Items.Count; i++)
            {
                if (lv_File.Items[i].Checked)
                {
                    NET_SDK_REC_FILE file = m_fileList[i + 1];
                    temp.Add(file);
                    lv_File.Items[i].Checked = false;                   
                }

            }
            if (temp.Count == 0)
            {
                MessageBox.Show("0 File Selected!");
                return;
            }
            NET_SDK_REC_FILE[] filesToBackup = new NET_SDK_REC_FILE[temp.Count];
            for (int i = 0; i < temp.Count; i++)
                filesToBackup[i] = temp[i];

            //BackUp_Dialog bd = new BackUp_Dialog(userId, filesToBackup, temp.Count, m_backPath);
            //bd.ShowDialog();
            DateTime currentTime = DateTime.Now;
            string fileName = tb_BackupPath.Text + "Backup_" + currentTime.Year.ToString() + currentTime.Month.ToString() + currentTime.Hour.ToString() + currentTime.Minute.ToString() + currentTime.Second.ToString() + ".avi";
            DD_TIME tempStart = filesToBackup[0].startTime;
            DD_TIME tempStop = filesToBackup[0].stopTime;

            POINTERHANDLE fileHandle = DevSdkHelper.NET_SDK_GetFileByTimeEx(userId, (int)filesToBackup[0].dwChannel, ref tempStart, ref tempStop, fileName, false, false, null, IntPtr.Zero);//
            if (fileHandle == -1)
            {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                System.Timers.Timer oTimer = new System.Timers.Timer(1000);
                oTimer.AutoReset = true;
                oTimer.Enabled = true;
                oTimer.Elapsed += new ElapsedEventHandler((object senderr, ElapsedEventArgs ee) =>
                {
                    try
                    {
                        POINTERHANDLE process = DevSdkHelper.NET_SDK_GetDownloadPos(fileHandle);
                        Debug.Print(process.ToString());
                        if (process >= 100)
                        {
                            oTimer.Stop();
                            DevSdkHelper.NET_SDK_StopGetFile(fileHandle);
                            Invoke(new Action(() => { label39.Text = "100" + "%"; }));
                        }
                        else
                        {
                            oTimer.Enabled = true;
                            Invoke(new Action(() => { label39.Text = process.ToString() + "%"; }));
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                });
            }     
        }
        private void btn_BackupEx_Click(object sender, EventArgs e)
        {
            DD_TIME st = new DD_TIME()
            {
                year = (ushort)(dtp_BackupStartTime.Value.Year - 1900),
                month = Convert.ToByte(dtp_BackupStartTime.Value.Month - 1),
                mday = Convert.ToByte(dtp_BackupStartTime.Value.Day),
                hour = Convert.ToByte(dtp_BackupStartTime.Value.Hour),
                minute = Convert.ToByte(dtp_BackupStartTime.Value.Minute),
                second = Convert.ToByte(dtp_BackupStartTime.Value.Second)
            };
            DD_TIME et = new DD_TIME()
            {
                year = (ushort)(dtp_BackupEndTime.Value.Year - 1900),
                month = Convert.ToByte(dtp_BackupEndTime.Value.Month - 1),
                mday = Convert.ToByte(dtp_BackupEndTime.Value.Day),
                hour = Convert.ToByte(dtp_BackupEndTime.Value.Hour),
                minute = Convert.ToByte(dtp_BackupEndTime.Value.Minute),
                second = Convert.ToByte(dtp_BackupEndTime.Value.Second)
            };
            DateTime currentTime = DateTime.Now;
            string fileName = tb_BackupPath.Text + "Backup_" + currentTime.Year.ToString() + currentTime.Month.ToString() + currentTime.Hour.ToString() + currentTime.Minute.ToString() + currentTime.Second.ToString() + ".avi";

            //long fileHandle = DevSdkHelper.NET_SDK_GetFileByTimeEx(userId, cmb_CHNN.SelectedIndex, ref st, ref et, fileName, false, false, null, IntPtr.Zero);//
            bool isMain = rb_MainStream.Checked ? true : false;
            POINTERHANDLE fileHandle = DevSdkHelper.NET_SDK_GetFileByTimeExV2(userId, cmb_CHNN.SelectedIndex, ref st, ref et, fileName, 0, isMain, false, null, IntPtr.Zero);//
            
            if (fileHandle == -1)
            {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                System.Timers.Timer oTimer = new System.Timers.Timer(1000);
                oTimer.AutoReset = true;
                oTimer.Enabled = true;
                oTimer.Elapsed += new ElapsedEventHandler((object senderr, ElapsedEventArgs ee) =>
                {
                    try
                    {
                        POINTERHANDLE process = DevSdkHelper.NET_SDK_GetDownloadPos(fileHandle);
                        Debug.Print(process.ToString());
                        if (process >= 100)
                        {
                            oTimer.Stop();
                            DevSdkHelper.NET_SDK_StopGetFile(fileHandle);
                            Invoke(new Action(() => { label92.Text = "100" + "%"; }));                            
                        }
                        else
                        {
                            oTimer.Enabled = true;
                            Invoke(new Action(() => { label92.Text = process.ToString() + "%"; }));     
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                });
            }           
        }
        private void btn_Playback_Click(object sender, EventArgs e)
        {
            DD_TIME st = new DD_TIME()
            {
                year = (ushort)(dtp_BackupStartTime.Value.Year - 1900),
                month = Convert.ToByte(dtp_BackupStartTime.Value.Month - 1),
                mday = Convert.ToByte(dtp_BackupStartTime.Value.Day),
                hour = Convert.ToByte(dtp_BackupStartTime.Value.Hour),
                minute = Convert.ToByte(dtp_BackupStartTime.Value.Minute),
                second = Convert.ToByte(dtp_BackupStartTime.Value.Second)
            };
            DD_TIME et = new DD_TIME()
            {
                year = (ushort)(dtp_BackupEndTime.Value.Year - 1900),
                month = Convert.ToByte(dtp_BackupEndTime.Value.Month - 1),
                mday = Convert.ToByte(dtp_BackupEndTime.Value.Day),
                hour = Convert.ToByte(dtp_BackupEndTime.Value.Hour),
                minute = Convert.ToByte(dtp_BackupEndTime.Value.Minute),
                second = Convert.ToByte(dtp_BackupEndTime.Value.Second)
            };
            PlayBack_Dialog pd = new PlayBack_Dialog(userId, st, et, (uint)cmb_CHNN.SelectedIndex);
            pd.ShowDialog();
        }
        private DD_TIME_EX DDTIME2DDTIMEEX(DD_TIME dt)
        {
            DD_TIME_EX dte = new DD_TIME_EX();
            dte.year = (ushort)(dt.year + 1900);
            dte.month = (byte)(dt.month + 1);
            dte.mday = dt.mday;
            dte.wday = dt.wday;
            dte.hour = dt.hour;
            dte.minute = dt.minute;
            dte.second = dt.second;
            dte.nMicrosecond = dt.nMicrosecond;
            dte.nTotalseconds = dt.nTotalseconds;
            return dte;
        }
    }
}
