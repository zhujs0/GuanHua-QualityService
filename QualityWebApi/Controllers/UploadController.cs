using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Net.Http.Headers;
using QualityWebApi.Common;
using Microsoft.Extensions.Configuration;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Upload")]
    public class UploadController : Controller
    {
        private string path = Directory.GetCurrentDirectory() + "/ProblemPicture/";
        private string GHConString = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("host.json", optional: true).Build().GetSection("urls").Value;
        [HttpPost]
        public string Post()
        {

            var files = HttpContext.Request.Form.Files;
            var FilePath = "";
            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[0];
                    var filename = file.FileName;
                    //var filename = ContentDispositionHeaderValue
                    //          .Parse(file.ContentDisposition)
                    //          .FileName
                    //          .Trim();
                    long size = file.Length;
                    //string houzhui = filename.Substring(filename.IndexOf("."));
                    string filePath = path + filename;
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    FilePath = GHConString + "/ProblemPicture/" + filename;
                }
            }

            return FilePath;

        }

        public string Get(string parm)
        {


            GeneralMethod options = new GeneralMethod();
            string Header = "{ \"typ\": \"JWT\", \"alg\": \"HS256\"}";
            string Payload = "{\"iss\": \"ninghao.net\",\"exp\": \"1438955445\",\"name\": \"wanghao\",\"admin\": true}";
            string test = options.CreateTokenByJWT(Header, Payload, "secret");
            return test;
        }
    }
}