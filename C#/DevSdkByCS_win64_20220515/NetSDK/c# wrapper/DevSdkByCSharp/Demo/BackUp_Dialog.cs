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
using System.Threading;


namespace Demo
{
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    public partial class BackUp_Dialog : Form
    {
        private int userId = 0;
        private NET_SDK_REC_FILE[] files;
        private List<NET_SDK_REC_FILE> fileList = new List<NET_SDK_REC_FILE>();
             
        private int num = 0;
        private string path = "";
        private string fileName = "";

        private System.Timers.Timer oTimer;
        private SynchronizationContext sc;
        private bool isDownloading = false;
        private POINTERHANDLE fileHandle = -1;
        private int currentIndex;

        public BackUp_Dialog(int m_userID, NET_SDK_REC_FILE[] fileToBackup, int fileNum, string m_backPath)
        {
            InitializeComponent();
            userId = m_userID;
            files = fileToBackup;
            num = fileNum;
            path = m_backPath;

            fileList = fileToBackup.ToList<NET_SDK_REC_FILE>();

            sc = SynchronizationContext.Current;
            label1.Text = "1" + "/" + num;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            currentIndex = 1;
            Download(fileList);
            
        }
        private void Download(List<NET_SDK_REC_FILE> _fileList)
        {            
            if (path.Substring(path.Length - 1) != "\\")
                path += "\\";

            sc.Post(UpdateLabel, currentIndex);
            DateTime currentTime = DateTime.Now;
            fileName = path + "Backup_" + currentTime.Year.ToString() + currentTime.Month.ToString() + currentTime.Hour.ToString() + currentTime.Minute.ToString() + currentTime.Second.ToString() + ".avi";
            DD_TIME tempStart = _fileList[0].startTime;
            DD_TIME tempStop = _fileList[0].stopTime;
            //fileHandle = DevSdkHelper.NET_SDK_GetFileByTime(userId, (int)_fileList[0].dwChannel, ref tempStart, ref tempStop, fileName);//private protocal
            fileHandle = DevSdkHelper.NET_SDK_GetFileByTimeEx(userId, (int)_fileList[0].dwChannel, ref tempStart, ref tempStop, fileName, false, false, null, IntPtr.Zero);//
            if (fileHandle == -1)
            {
                string strMsg = String.Format("error code: {0}", DevSdkHelper.NET_SDK_GetLastError());
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                oTimer = new System.Timers.Timer(1000);
                oTimer.AutoReset = true;
                oTimer.Enabled = true;
                oTimer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>
                {
                    try
                    {
                        //if (this.DialogResult == DialogResult.None)
                        {
                            POINTERHANDLE process = DevSdkHelper.NET_SDK_GetDownloadPos(fileHandle);
                            Debug.Print(process.ToString());
                            if (process >= 100)
                            {
                                oTimer.Stop();
                                DevSdkHelper.NET_SDK_StopGetFile(fileHandle);
                                sc.Post(UpdateProgressBar, 100);
                                _fileList.RemoveAt(0);
                                if (_fileList.Count > 0)
                                {
                                    currentIndex += 1;
                                    Download(_fileList);
                                }
                                else
                                {
                                    DevSdkHelper.NET_SDK_StopGetFile(fileHandle);
                                    fileHandle = -1;
                                    //this.Invoke(new Action(() => Close()));
                                }
                            }
                            else
                            {
                                oTimer.Enabled = true;
                                sc.Post(UpdateProgressBar, process); Debug.Print(process.ToString());
                            }
                        }
                       

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                });
            }           
        }
        private void UpdateProgressBar(object obj)
        {
            progressBar1.Value = (int)obj;
        }
        private void UpdateLabel(object obj)
        {
            label1.Text = obj.ToString() + "/" + num;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oTimer.Stop();
            DevSdkHelper.NET_SDK_StopGetFile(fileHandle);
            fileHandle = -1;
            this.Close();
        }

    }
}
