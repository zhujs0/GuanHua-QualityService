using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QualityWebApi.Common
{
    public class HttpGETOrPost
    {
        //TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
        //ts.TotalMilliseconds;
        private string _Urls { get; set; }
        public HttpGETOrPost(string Url)
        {
            _Urls = Url;
        }
        public string HttpPost(string Data)
        {
            
            HttpWebRequest req = WebRequest.CreateHttp(new Uri(_Urls));
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            req.Method = "POST";
            req.ContinueTimeout = 60000;
            byte[] postData = Encoding.UTF8.GetBytes(Data);
            Stream reqStream = req.GetRequestStreamAsync().Result;
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Dispose();
            var rsp = (HttpWebResponse)req.GetResponseAsync().Result;
            Stream stream = rsp.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string result= reader.ReadToEnd();
            stream.Dispose();
            reader.Dispose();
            rsp.Dispose();
            return result;

        }

        public string HttpGet(string parms)
        {
            string urls = _Urls + parms;
            HttpClient httpClient = new HttpClient();
            Task<byte[]> Request = httpClient.GetByteArrayAsync(urls);
            Request.Wait();
            var Response = Encoding.UTF8.GetString(Request.Result);
            return Response;
        }


    }
}
