using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MyTest1.Worker
{
    public class Request
    {
        private string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
        private int TimeOut = 300000;

        public Request()
        {
            
        }

        public int GetUrlTime(string url)
        {
            
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = UserAgent;
                request.Timeout = TimeOut;
                Stopwatch timer = Stopwatch.StartNew();
                var response = (HttpWebResponse)request.GetResponse();
                timer.Stop();
                response.Close();
                return timer.Elapsed.Milliseconds;
            }
            catch (Exception )
            {
                return 0;
            }
          
            
        }

        public string GetUrl(string url)
        {

            try
            {
                string Content = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = UserAgent;
                request.Timeout = TimeOut;
                request.AllowAutoRedirect = true;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    Content = reader.ReadToEnd();
                }
                return Content;
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        


    }
}