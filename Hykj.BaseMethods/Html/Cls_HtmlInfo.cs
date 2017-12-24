using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Hykj.BaseMethods
{
    public class HtmlInfo
    {
        /*
         * 读取网页中的字符串信息
         */
        public static string GetHtmlString(string url,out string logMessage)
        {
            logMessage = string.Empty;
            string strBuff = string.Empty;//定义文本字符串，用来保存下载的html  
            int byteRead = 0;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                try
                {
                    //若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理   
                    Stream reader = webResponse.GetResponseStream();
                    ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8）  
                    StreamReader respStreamReader = new StreamReader(reader, Encoding.UTF8);

                    ///分段，分批次获取网页源码  
                    char[] cbuffer = new char[256];
                    byteRead = respStreamReader.Read(cbuffer, 0, 256);

                    while (byteRead != 0)
                    {
                        string strResp = new string(cbuffer, 0, byteRead);
                        strBuff = strBuff + strResp;
                        byteRead = respStreamReader.Read(cbuffer, 0, 256);
                    }
                    respStreamReader.Close();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logMessage = "GetHtmlString方法WriteFile出错，html地址为：" + url + "，错误信息：" + ex.Message + ex.Source;
                }
                return strBuff;
            }
            catch (Exception ex)
            {
                logMessage = "GetHtmlString方法GetResponse出错，html地址为：" + url + "，错误信息：" + ex.Message + ex.Source;
                return strBuff;
            }
        }
    }
}
