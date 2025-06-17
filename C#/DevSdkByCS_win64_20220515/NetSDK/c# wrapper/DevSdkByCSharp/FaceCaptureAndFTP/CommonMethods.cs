using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Xml;
using System.Net;
using System.IO;
//using Excel = Microsoft.Office.Interop.Excel;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Drawing.Imaging

namespace FaceCaptureAndFTP
{
    class CommonMethods
    {
        /// <summary>  
        /// 剪裁 -- 用GDI+   
        /// </summary>  
        /// <param name="b">原始Bitmap</param>  
        /// <param name="StartX">开始坐标X</param>  
        /// <param name="StartY">开始坐标Y</param>  
        /// <param name="iWidth">宽度</param>  
        /// <param name="iHeight">高度</param>  
        /// <returns>剪裁后的Bitmap</returns>  
        //public static Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        //{
        //    if (b == null)
        //    {
        //        return null;
        //    }
        //    int w = b.Width;
        //    int h = b.Height;
        //    if (StartX >= w || StartY >= h)
        //    {
        //        return null;
        //    }
        //    if (StartX + iWidth > w)
        //    {
        //        iWidth = w - StartX;
        //    }
        //    if (StartY + iHeight > h)
        //    {
        //        iHeight = h - StartY;
        //    }
        //    try
        //    {
        //        Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
        //        Graphics g = Graphics.FromImage(bmpOut);
        //        g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
        //        g.Dispose();
        //        return bmpOut;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        /// <summary>
        /// 把用户名、密码转换成base64编码的字符串
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="psw">密码</param>
        /// <returns></returns>
        public static string GetBase64String(string userName, string psw)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(userName + ":" + psw));
        }

        /// <summary>
        /// 获取xml中指定节点的值
        /// </summary>
        /// <param name="xmlContent">xml内容</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="index">节点出现的位置</param>
        /// <returns></returns>
        public static string GetNodeValue(string xmlContent, string nodeName, int index = 0)
        {
            try
            {
                XmlNode tempNode = GetNodePosByIndex(xmlContent, nodeName, index);
                if (tempNode != null)
                {
                    if (tempNode.InnerText.Contains("CDATA"))
                    {
                        string str = tempNode.InnerText;
                        return str.Substring(8, str.Length - 10);
                    }
                    else
                        return tempNode.InnerText;
                }
                else
                    return "";

            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 获取节点的指定属性的值
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="attributeName">节点中指定的属性名称</param>
        /// <returns>返回属性的值</returns>
        public static string GetNodeAttribute(XmlNode node, string attributeName)
        {
            string result = "";
            if (node != null && node.Attributes.Count > 0)
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].Name == attributeName)
                    {
                        result = node.Attributes[i].InnerText;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 定位到指定名称节点的第index个位置
        /// </summary>
        /// <param name="xmlContent">要定位的xml内容</param>
        /// <param name="nodeName">要定位的节点名称</param>
        /// <param name="index">节点出现的位置</param>
        /// <returns></returns>
        public static XmlNode GetNodePosByIndex(string xmlContent, string nodeName, int index = 0)//
        {
            try
            {
                XmlDocument oldXd = new XmlDocument();
                oldXd.LoadXml(xmlContent);
                XmlNodeList nodeList = oldXd.GetElementsByTagName(nodeName);
                if (nodeList.Count > index)
                    return nodeList[index];
                else
                    return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取指定节点名称的节点数量
        /// </summary>
        /// <param name="xmlContent">xml内容</param>
        /// <param name="nodeName">要查找的节点名称</param>
        /// <returns>节点数量</returns>
        public static int GetNodeNum(string xmlContent, string nodeName)
        {
            try
            {
                XmlDocument oldXd = new XmlDocument();
                oldXd.LoadXml(xmlContent);
                XmlNodeList nodeList = oldXd.GetElementsByTagName(nodeName);
                return nodeList.Count;
            }
            catch
            {
                return 0;
            }
            
        }

        /// <summary>
        /// http发送命令
        /// </summary>
        /// <param name="Url">命令</param>
        /// <param name="postDataStr">附加内容</param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, string name, string psw)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(name + ":" + psw)));
            request.Credentials = new NetworkCredential(name, psw);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            //request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        
        /// <summary>
        /// 返回当前的时间值
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }
        /// <summary>
        /// 查找指定指定目录下的图片，并转移到另一个目录下，返回图片的分辨率
        /// </summary>
        /// <param name="oldPath">查找目录</param>
        /// <param name="newPath">新目录</param>
        /// <param name="partName">原图片的部分名称</param>
        /// <param name="methodName">方法名称</param>
        /// <returns>图片的分辨率</returns>
        //public static string MoveAndRenamePicture(string oldPath, string newPath, string partName)
        //{

        //    string width = "";
        //    string height = "";
        //    string res = "";
        //    string res1 = "";
        //    try
        //    {
        //        if (Directory.Exists(oldPath))//如果有图片文件夹存在
        //        {
        //            foreach (FileInfo fi in (new DirectoryInfo(oldPath)).GetFiles())//遍历文件夹中的文件
        //            {
        //                if (fi.Name.EndsWith(partName))
        //                {
        //                    Image img = Image.FromFile(fi.FullName);
        //                    width = img.PhysicalDimension.Width.ToString();
        //                    height = img.PhysicalDimension.Height.ToString();
        //                    //res = width + "x" + height;
        //                    res1 = fi.FullName;
        //                    img.Dispose();
        //                    break;
        //                }
        //            }
        //        }
        //        //string tempPicPath = newPath + "\\" + methodName + GetTimeNumber() + ".jpg";
        //        //Form1.tempPicPath = newPath + "\\" + methodName + GetTimeNumber() + ".jpg";
        //        File.Move(res1, newPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        res = ex.Message;
        //    }

        //    return res;
        //}
        /// <summary>
        /// 获取当前时间数字显示
        /// </summary>
        /// <returns>返回数字显示的时间</returns>
        public static string GetTimeNumber()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            string res;
            res = currentTime.Year.ToString() + currentTime.Month.ToString() + currentTime.Day.ToString() + currentTime.Hour.ToString() + currentTime.Minute.ToString() + currentTime.Second.ToString();
            return res;
        }

        public static bool PingTong(string ip)
        {
            string result = string.Empty;
            //构造Ping实例  
            Ping pingSender = new Ping();
            //Ping 选项设置
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            //测试数据
            string data = "test data abcabc";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            //调用同步 send 方法发送数据,将返回结果保存至PingReply实例

            try
            {
                PingReply reply = pingSender.Send(ip, 120, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            
        }
        public static bool IsNum(string str1)//利用正则表达式判断输入的行数是否是整数
        {
            Regex r = new Regex(@"^\d+$");
            if (r.IsMatch(str1))
                return true;
            else
                return false;
        }
        // 将char[]数组转换为string类型并返回  
        public static string CharArrayTosting(char[] cha, int len)
        {
            string str = "";
            for (int i = 0; i < len; i++)
            {
                str += string.Format("{0}", cha[i]);
            }
            return str;
        }
        /// <summary>
        /// 判断域名是否合法
        /// </summary>
        /// <param name="ipAddr">要判断的域名</param>
        /// <returns>返回true或false</returns>
        public static bool IsDomain(string ipAddr)
        {           
            bool res = false;
            try
            {
                IPHostEntry host = Dns.GetHostEntry(ipAddr);
                IPAddress ipa = host.AddressList[0];
                res = true;
            }
            catch
            { }
            return res;           
        }
        /// <summary>
        /// 判断ip地址是否合法
        /// </summary>
        /// <param name="ipAddr">要判断的ip地址</param>
        /// <returns>返回true或false</returns>
        public static bool IsIP(string ipAddr)
        {
            bool res = false;
            try
            {
                IPAddress ipa = IPAddress.Parse(ipAddr);
                res = true;
            }
            catch
            { }
            return res;

        }
        /// <summary>
        /// 把ascii码转成字符串
        /// </summary>
        /// <param name="asciiCode">ascii码编号</param>
        /// <returns>对应的字符串</returns>
        public static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }
        /// <summary>
        /// 控制报警输出状态
        /// </summary>
        /// <param name="switchIP">报警输出的设备IP</param>
        /// <param name="switchId">报警输出的id，用F12在网页中查找</param>
        /// <param name="status">true为触发报警，false为关闭报警</param>
        public static void Switch(string switchIP, string switchId, string status)
        {
            string switchXml;
            string cmdStr = "http://" + switchIP + "/setAlarmOutStatus";
            string emptyXml = @"<?xml version='1.0' encoding='UTF - 8'?>
                                  <request version = '1.0' systemType = 'NVMS-9000' clientType='WEB'>     
                                         <content>    
                                             <switch>status to be replaced</switch>    
                                             <alarmOutIds type='list'>
                                                <item id='{id to be replaced}'/>
                                             </alarmOutIds>    
		                                </content>
	                              </request>";
            try
            {
                switchXml = emptyXml.Replace("id to be replaced", switchId);
                switchXml = switchXml.Replace("status to be replaced", status);
                HttpPost(cmdStr, switchXml,"admin","123456");
            }
            catch
            {
                //button2_Click(null, null);
            }
        }
        public static void ClearFolder(string path)
        {
            if (Directory.Exists(path))//如果有图片文件夹存在，则清空里面的图片文件
            {
                foreach (FileInfo fi in (new DirectoryInfo(path)).GetFiles())//遍历文件夹中的文件
                {
                    if (fi != null)
                        File.Delete(fi.FullName);//删除单个文件
                }
            }
        }

        public static void BmpToJpg(string bmp, string jpg)
        {
            //Stream tempStream = File.OpenRead(bmp);
            //Bitmap bm = new Bitmap(tempStream);
            //Image ig = bm;
            //Image newIg = ig.GetThumbnailImage(bm.Width, bm.Height, null, new IntPtr());
            //Graphics g = Graphics.FromImage(newIg);
            //g.DrawImage(newIg, 0, 0, newIg.Width, newIg.Height);
            //g.Dispose();
            //tempStream.Close();
            //newIg.Save(jpg);

        }
        /// <summary>
        /// 删除指定文件夹中指定后缀的文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="endWith">后缀名称</param>
        public static void RemoveFile(string path, string endWith)
        {
            try
            {
                if (Directory.Exists(path))//如果有图片文件夹存在
                {
                    foreach (FileInfo fi in (new DirectoryInfo(path)).GetFiles())//遍历文件夹中的文件
                    {
                        if (fi.Name.EndsWith(endWith))
                        {
                            File.Delete(fi.FullName);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                
            }
        }
        public static string MoveAndChangeToJpg(string path, string newPath)
        {
            string tempPath = "";
            try
            {
                if (Directory.Exists(path))//如果有图片文件夹存在
                {
                    foreach (FileInfo fi in (new DirectoryInfo(path)).GetFiles())//遍历文件夹中的文件
                    {
                        if (fi.Name.EndsWith("bmp"))
                        {
                            tempPath = newPath + "\\" + GetTimeNumber() + ".jpg";
                            BmpToJpg(fi.FullName, tempPath);
                            File.Delete(fi.FullName);//删除原照片
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            
            return tempPath;
            
        }
        public static void MoveAndChangeToJpg1(string path, string newPath)
        {
            try
            {
                if (Directory.Exists(path))//如果有图片文件夹存在
                {
                    foreach (FileInfo fi in (new DirectoryInfo(path)).GetFiles())//遍历文件夹中的文件
                    {
                        if (fi.Name.EndsWith("bmp"))
                        {                           
                            BmpToJpg(fi.FullName, newPath);
                            File.Delete(fi.FullName);//删除原照片
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }


        }
        public static void RegisterDLL(string dllPath)
        {
            Process p = new Process();
            p.StartInfo.FileName = "regsvr32";
            p.StartInfo.Arguments = dllPath;
            p.Start();
            p.WaitForExit();
            p.Close();
            p.Dispose();
        }
        public static void WriteFile(byte[] byteArray, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create); //创建一个文件流           
            fs.Write(byteArray, 0, byteArray.Length);//将byte数组写入文件中                                                    
            fs.Close();//所有流类型都要关闭流，否则会出现内存泄露问题
        }
        public static void AppendFile(byte[] byteArray, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Append,FileAccess.ReadWrite); //创建一个文件流           
            fs.Write(byteArray, 0, byteArray.Length);//将byte数组写入文件中      
                                                          
            fs.Close();//所有流类型都要关闭流，否则会出现内存泄露问题
        }
        public static bool IsToEmailVaild(string csMail)
        {
            if (csMail.Length < 1)
                return false;

            Regex r = new Regex("^([a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)*;)*$");// ^([a-zA-Z0-9_-]+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_\-])+;)*$ 完全匹配
            if (r.IsMatch(csMail))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 把十进制字节转成十六进制
        /// </summary>
        /// <param name="dec">十进制字节数</param>
        /// <returns></returns>
        public static string ChangeToHex(byte dec)
        {
            string str = dec.ToString("X");
            str = str.Length == 1 ? "0" + str : str;
            return str;
        }

        /// <summary>
        /// base64编码的文本 转为图片
        /// </summary>
        /// <param name="basestr">base64字符串</param>
        /// <returns>转换后的Bitmap对象</returns>
        //public Bitmap Base64StringToImage(string basestr)
        //{
        //    Bitmap bitmap = null;
        //    try
        //    {
        //        String inputStr = basestr;
        //        byte[] arr = Convert.FromBase64String(inputStr);
        //        MemoryStream ms = new MemoryStream(arr);
        //        Bitmap bmp = new Bitmap(ms);
        //        ms.Close();
        //        bitmap = bmp;
        //    }
        //    catch
        //    {
                
        //    }

        //    return bitmap;
        //}
    }
}
