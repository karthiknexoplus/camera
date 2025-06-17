using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FaceCaptureAndFTP
{
    public class Log
    {
        private string dataPath;//日志的路径
        private bool record;//是否记录日志开关
        private Object thisLock = new Object();

        #region 原方法
        StreamWriter m_swLog;
        #endregion
        #region 使用trace类
        FileStream fs;

        public void Initial(bool record, string path, string name)
        {
            this.record = record;
            string monthPart = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString();
            string dayPart = DateTime.Now.Day > 9 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString();          
            string timePart = DateTime.Now.Year.ToString() + "-" + monthPart + "-" + dayPart;
            dataPath = path + "\\"+ timePart + " " + name + ".log";           
        }

        public void WriteLog(string log,int iLevel)
        {
            lock (thisLock)
            {
                if (this.record)
                {
                    m_swLog = new StreamWriter(dataPath, true);
                    //m_swLog.WriteLine(DateTime.Now);
                    m_swLog.WriteLine(DateTime.Now + ":" + log);
                    m_swLog.WriteLine("");
                    m_swLog.Flush();
                    m_swLog.Close();
                }
            }
        }
        #endregion
    }
}
