﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
namespace FaceCaptureAndFTP
{
    class FtpHelper    
    {        
        string ftpServerIP;        
        string ftpRemotePath;        
        string ftpUserID;        
        string ftpPassword;        
        string ftpURI;         
        /// <summary>        
        /// 连接FTP        
        /// </summary>        
        /// <param name="FtpServerIP">FTP连接地址</param>        
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>        
        /// <param name="FtpUserID">用户名</param>        
        /// <param name="FtpPassword">密码</param>      
        public FtpHelper(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)        
        {            
            ftpServerIP = FtpServerIP;            
            ftpRemotePath = FtpRemotePath;            
            ftpUserID = FtpUserID;            
            ftpPassword = FtpPassword;            
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";        
        }          
        /// <summary>        
        /// 下载        
        /// </summary>        
        /// <param name="filePath"></param>        
        /// <param name="fileName"></param>        
        public void Download(string filePath, string fileName)        
        {            
            try            
            {                
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);                
                FtpWebRequest reqFTP;                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));                
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;                
                reqFTP.UseBinary = true;                
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();                
                Stream ftpStream = response.GetResponseStream();                
                long cl = response.ContentLength;                
                int bufferSize = 2048;                
                int readCount;                
                byte[] buffer = new byte[bufferSize];                
                readCount = ftpStream.Read(buffer, 0, bufferSize);                
                while (readCount > 0)                
                {                    
                    outputStream.Write(buffer, 0, readCount);                    
                    readCount = ftpStream.Read(buffer, 0, bufferSize);                
                }                
                ftpStream.Close();                
                outputStream.Close();                
                response.Close();            
            }            
            catch (Exception ex)            
            {                
                throw new Exception(ex.Message);            
            }        
        }         
        public string[] GetAllList(string url)        
        {            
            List<string> list = new List<string>();            
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI));            
            req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);            
            req.Method = WebRequestMethods.Ftp.ListDirectory;            
            req.UseBinary = true;            
            req.UsePassive = true;            
            try            
            {                
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())                
                {                    
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))                    
                    {                        
                        string s;                        
                        while ((s = sr.ReadLine()) != null)                        
                        {                            
                            list.Add(s);                        
                        }                    
                    }                
                }            
            }            
            catch (Exception ex)            
            {                
                throw (ex);            
            }            
            return list.ToArray();        
        }           
        /// <summary>          
        /// 获取当前目录下明细(包含文件和文件夹)          
        /// </summary>          
        public string[] GetFilesDetailList()        
        {            
            try            
            {                
                StringBuilder result = new StringBuilder();                
                FtpWebRequest ftp;                
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));                
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;                
                WebResponse response = ftp.GetResponse();                
                StreamReader reader = new StreamReader(response.GetResponseStream());                
                string line = reader.ReadLine();                
                line = reader.ReadLine();                
                line = reader.ReadLine();                
                while (line != null)                
                {                    
                    result.Append(line);                    
                    result.Append("\n");                    
                    line = reader.ReadLine();                
                }                
                result.Remove(result.ToString().LastIndexOf("\n"), 1);                
                reader.Close();                
                response.Close();                
                return result.ToString().Split('\n');            
            }            
            catch (Exception ex)            
            {                
                throw new Exception(ex.Message);            
            }        
        }                  
        /// <summary>          
        /// 移动文件          
        /// </summary>          
        public void MovieFile(string currentFilename, string newDirectory)        
        {            
            ReName(currentFilename, newDirectory);        
        }
        /// <summary>          
        /// 更改文件名          
        /// </summary>         
        public void ReName(string currentFilename, string newFilename)        
        {            
            FtpWebRequest reqFTP;            
            try            
            {                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + currentFilename));                
                reqFTP.Method = WebRequestMethods.Ftp.Rename;                
                reqFTP.RenameTo = newFilename;                
                reqFTP.UseBinary = true;                
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();                
                Stream ftpStream = response.GetResponseStream();                
                ftpStream.Close();                
                response.Close();            
            }            
            catch (Exception ex)            { }        
        }        
        /// <summary>          
        /// 获取指定文件大小          
        /// </summary>          
        public long GetFileSize(string filename)        
        {            
            FtpWebRequest reqFTP;            
            long fileSize = 0;            
            try            
            {                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + filename));                
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;                
                reqFTP.UseBinary = true;                
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();                
                Stream ftpStream = response.GetResponseStream();                
                fileSize = response.ContentLength;                
                ftpStream.Close();                
                response.Close();            
            }            
            catch (Exception ex)            { }            
            return fileSize;        
        }         
        /// <summary>          
        /// 创建文件夹          
        /// </summary>           
        public void MakeDir(string dirName)        
        {            
            FtpWebRequest reqFTP;            
            try            
            {                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + dirName));                
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;                
                reqFTP.UseBinary = true;                
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();                
                Stream ftpStream = response.GetResponseStream();                
                ftpStream.Close();                
                response.Close();            
            }            
            catch (Exception ex)            { }        
        }                  
        /// <summary>          
        /// 删除文件          
        /// </summary>          
        public void Delete(string fileName)        
        {            
            try            
            {                
                FtpWebRequest reqFTP;                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));                
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;                
                reqFTP.KeepAlive = false;                
                string result = String.Empty;                
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();                
                long size = response.ContentLength;                
                Stream datastream = response.GetResponseStream();                
                StreamReader sr = new StreamReader(datastream);                
                result = sr.ReadToEnd();                
                sr.Close();                
                datastream.Close();                
                response.Close();            
            }            
            catch (Exception ex)            
            {                
                throw new Exception(ex.Message);            
            }        
        }          
        /// <summary>          
        /// 上传          
        /// </summary>           
        public void Upload(string filename)        
        {            
            FileInfo fileInf = new FileInfo(filename);            
            FtpWebRequest reqFTP;            
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileInf.Name));            
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);            
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;            
            reqFTP.KeepAlive = false;            
            reqFTP.UseBinary = true;            
            reqFTP.ContentLength = fileInf.Length;            
            int buffLength = 2048;            
            byte[] buff = new byte[buffLength];            
            int contentLen;            
            FileStream fs = fileInf.OpenRead();            
            try            
            {                
                Stream strm = reqFTP.GetRequestStream();                
                contentLen = fs.Read(buff, 0, buffLength);                
                while (contentLen != 0)                
                {                    
                    strm.Write(buff, 0, contentLen);                    
                    contentLen = fs.Read(buff, 0, buffLength);                
                }                
                strm.Close();                
                fs.Close();            
            }            
            catch (Exception ex)            
            {                
                throw new Exception(ex.Message);            
            }        
        }    
    }

}
