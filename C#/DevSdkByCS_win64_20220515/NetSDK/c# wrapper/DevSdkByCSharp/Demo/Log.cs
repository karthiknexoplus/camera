using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Demo
{
    public class Log
    {
        private string dataPath;//日志的路径
        private bool record;//是否记录日志开关
        private Object thisLock = new Object();

        #region 原方法
        StreamWriter m_swLog;
        //public Log(bool record)
        //{
        //    this.record = record;
        //    dataPath = Form1.sharedPath + @"\Log\client.log";
        //    if (File.Exists(dataPath) == false)
        //    {
        //        FileStream fs = File.Create(dataPath);
        //        fs.Close();
        //    }
        //}
        ///// <summary>
        ///// 写日志信息
        ///// </summary>
        ///// <param name="log"></param>
        //public void WriteLog(string log)
        //{
        //    lock (thisLock)
        //    {
        //        if (this.record)
        //        {
        //            m_swLog = new StreamWriter(dataPath, true);
        //            m_swLog.WriteLine(DateTime.Now.ToString());
        //            m_swLog.WriteLine("#" + log + "#");
        //            m_swLog.WriteLine("");
        //            m_swLog.Flush();
        //            m_swLog.Close();
        //        }
        //    }
        //}
        #endregion
        #region 使用trace类
        FileStream fs;

        public void Initial(bool record, string path, string name)
        {
            this.record = record;
            string monthPart = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString();
            string dayPart = DateTime.Now.Day > 9 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString();
            //string hourPart = DateTime.Now.Hour > 9 ? DateTime.Now.Hour.ToString() : "0" + DateTime.Now.Hour.ToString();
            //string minutePart = DateTime.Now.Minute > 9 ? DateTime.Now.Minute.ToString() : "0" + DateTime.Now.Minute.ToString();
            //string secondPart = DateTime.Now.Second > 9 ? DateTime.Now.Second.ToString() : "0" + DateTime.Now.Second.ToString();
            //string timePart = DateTime.Now.Year.ToString() + "-" + monthPart + "-" + dayPart + " " + hourPart + "-" + minutePart + "-" + secondPart;
            string timePart = DateTime.Now.Year.ToString() + "-" + monthPart + "-" + dayPart;
            //dataPath = path + @"\Logs\" + timePart + " " + name + ".log";
            dataPath = path + "\\"+ timePart + " " + name + ".log";
            //if (File.Exists(dataPath))
            //    fs = new FileStream(dataPath, FileMode.Append);
            //else
            //    fs = new FileStream(dataPath, FileMode.Create);
            //fs = new FileStream(dataPath, FileMode.OpenOrCreate);
            //Trace.Listeners.RemoveAt(0);  //删除默认的侦听器
            //Trace.Listeners.Add(new TextWriterTraceListener(fs));//添加新的侦听器
        }
        /// <summary>
        /// 写日志信息
        /// </summary>
        /// <param name="log">信息</param>
        /// <param name="iLevel">缩进级别</param>
        //public void WriteLog(string log,int iLevel)
        //{
        //    lock (thisLock)
        //    {
        //        if (this.record)
        //        {
        //            Trace.IndentLevel = iLevel;
        //            Trace.WriteLine(DateTime.Now);
        //            Trace.WriteLine(log);
        //            Trace.WriteLine("");
        //            Trace.Flush();//保存
        //        }
        //    }
        //}
        public void WriteLog(string log,int iLevel)
        {
            lock (thisLock)
            {
                if (this.record)
                {
                    m_swLog = new StreamWriter(dataPath, true);
                    m_swLog.WriteLine(DateTime.Now);
                    m_swLog.WriteLine(log);
                    m_swLog.WriteLine("");
                    m_swLog.Flush();
                    m_swLog.Close();
                }
            }
        }
        public void CloseLog()
        {            
            //Trace.Close();
            //fs.Close();
        }
        #endregion
    }
}
