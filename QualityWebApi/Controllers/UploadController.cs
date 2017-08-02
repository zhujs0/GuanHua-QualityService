using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Upload")]
    public class UploadController : Controller
    {
        private string path = Directory.GetCurrentDirectory() + "/ProblemPicture/";
        [HttpPost]
        public string Post()
        {
            var files = HttpContext.Request.Form.Files;
            if(files.Count>0)
            {
                for(int i=0;i<files.Count;i++)
                {
                    var file = HttpContext.Request.Form.Files[0];

                    var filename = ContentDispositionHeaderValue
                              .Parse(file.ContentDisposition)
                              .FileName
                              .Trim('"');
                    long size = file.Length;
                    //string houzhui = filename.Substring(filename.IndexOf("."));
                    string filePath = path + filename;
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }
            
            return "http://192.168.1.33:8000/ProblemPicture/t0176ee418172932841.jpg";
                
        }
    }
}