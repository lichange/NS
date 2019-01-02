//=====================================================================================
// All Rights Reserved , Copyright © Learun 2013
//=====================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading;
using NS.Framework.Global;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace NS.Component.Utility
{
    /// <summary>
    /// 文件下载类
    /// </summary>
    public class FileDownHelper
    {
        public FileDownHelper()
        { }
        /// <summary>
        /// 参数为虚拟路径
        /// </summary>
        public static string FileNameExtension(string FileName)
        {
            return Path.GetExtension(MapPathFile(FileName));
        }

        /// <summary>
        /// 获取物理地址
        /// </summary>
        public static string MapPathFile(string FileName)
        {
            return Path.Combine(NSWebPath.HostEnv.ContentRootPath, FileName);
        }
        /// <summary>
        /// 验证文件是否存在
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool FileExists(string FileName)
        {
            string destFileName = MapPathFile(FileName);
            if (File.Exists(destFileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 普通下载
        /// </summary>
        /// <param name="FileName">文件虚拟路径</param>
        ///  /// <param name="name">返回客户端名称</param>
        public static FileStreamResult DownLoadold(string FileName, string name)
        {
            string destFileName = MapPathFile(FileName);
            if (!File.Exists(destFileName))
            {
                return null;
            }

            var stream = new FileStream(destFileName, FileMode.Open);
            return new FileStreamResult(stream, "application/octet-stream");
        }
        /// <summary>
        /// 分块下载 2018.12.30
        /// </summary>
        /// <param name="FileName">文件虚拟路径</param>
        public static void DownLoad(string FileName)
        {
            string filePath = MapPathFile(FileName);
            long chunkSize = 204800;             //指定块大小 
            byte[] buffer = new byte[chunkSize]; //建立一个200K的缓冲区 
            long dataToRead = 0;                 //已读的字节数   
            FileStream stream = null;
            try
            {
                //打开文件   
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataToRead = stream.Length;

                //添加Http头   
                NSHttpContext.Current.Response.ContentType = "application/octet-stream";
                NSHttpContext.Current.Response.Headers.Add("Content-Disposition", "attachement;filename=" + HttpUtility.UrlEncode(Path.GetFileName(filePath)));
                NSHttpContext.Current.Response.ContentLength = dataToRead;

                while (dataToRead > 0)
                {
                    if (!NSHttpContext.Current.RequestAborted.IsCancellationRequested)
                    {
                        int length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));
                        NSHttpContext.Current.Response.Body.Write(buffer, 0, length);
                        NSHttpContext.Current.Response.Body.Flush();
                        //NSHttpContext.Current.Response.Clear();
                        dataToRead -= length;
                    }
                    else
                    {
                        dataToRead = -1; //防止client失去连接 
                    }
                }
            }
            catch (Exception ex)
            {
                byte[] decBytes = System.Text.Encoding.UTF8.GetBytes(ex.Message);
                NSHttpContext.Current.Response.Body.Write(decBytes, 0, decBytes.Length);
            }
            finally
            {
                if (stream != null) stream.Close();
                NSHttpContext.Current.Response.Body.Close();
            }
        }

        /// <summary>
        ///  输出硬盘文件，提供下载 支持大文件、续传、速度限制、资源占用小 2018.12.30(New)
        /// </summary>
        /// <param name="_Request">Page.Request对象</param>
        /// <param name="_Response">Page.Response对象</param>
        /// <param name="_fileName">下载文件名</param>
        /// <param name="_fullPath">带文件名下载路径</param>
        /// <param name="_speed">每秒允许下载的字节数</param>
        /// <returns>返回是否成功</returns>
        //---------------------------------------------------------------------
        //调用：
        // string FullPath=Server.MapPath("count.txt");
        // ResponseFile(this.Request,this.Response,"count.txt",FullPath,100);
        //---------------------------------------------------------------------
        public static bool ResponseFile(HttpRequest _Request, HttpResponse _Response, string _fileName, string _fullPath, long _speed)
        {
            try
            {
                FileStream myFile = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    _Response.Headers.Add("Accept-Ranges", "bytes");
                    _Response.Headers.Add("Cache-Control", "no-cache");

                    long fileLength = myFile.Length;
                    long startBytes = 0;
                    int pack = 10240;  //10K bytes
                    int sleep = (int)Math.Floor((double)(1000 * pack / _speed)) + 1;

                    if (!String.IsNullOrEmpty(_Request.Headers["Range"]))
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].ToString().Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.Headers.Add("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.Headers.Add("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }

                    _Response.Headers.Add("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.Headers.Add("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(_fileName, System.Text.Encoding.UTF8));

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((double)((fileLength - startBytes) / pack)) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (!_Response.HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            Byte[] val = br.ReadBytes(pack);
                            _Response.Body.Write(val, 0, val.Length);
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
