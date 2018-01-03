using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;

namespace QualityWebApi.Common
{
    public class GeneralMethod
    {
        public GeneralMethod()
        {

        }

        public void Upload(string filename, out string StrErrorMsg, string NewFileName)
        {
            StrErrorMsg = "";
            //FileInfo fileInf = new FileInfo(filename);//获取要上传的文件
            //string uri = ftpURI + NewFileName;
            ////string uri = ftpURI + fileInf.Name;//文件存放位置
            //FtpWebRequest reqFTP;

            //reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));//简建立ftp链接
            ////reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);//设置链接ftp的账号密码
            //reqFTP.KeepAlive = false;//设置请求完成后是否保持连接
            //reqFTP.Method = WebRequestMethods.Ftp.UploadFile;//设置ftp执行的命令
            //reqFTP.UseBinary = true;//制定数据传输类型
            //reqFTP.ContentLength = fileInf.Length;//设置ftp传输的内容大小
            //int buffLength = 2048;//设置缓冲
            //byte[] buff = new byte[buffLength];
            //int contentLen;
            //using (FileStream fs = fileInf.OpenRead())//读取需要上传的文件
            //{
            //    try
            //    {
            //        Stream strm = reqFTP.GetRequestStream();//定义ftp写入流
            //        contentLen = fs.Read(buff, 0, buffLength);//每次读文件的大小
            //        while (contentLen != 0)//循环直至文件读取完毕
            //        {
            //            strm.Write(buff, 0, contentLen);
            //            contentLen = fs.Read(buff, 0, buffLength);
            //        }
            //        strm.Close();
            //        fs.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        StrErrorMsg = ex.Message;
            //    }
            //}
        }

        public void SaveToExcel<T>(string FilePath,List<T> Data,List<T> Properties)
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                //T Item = Properties[i].GetProperty();
                
            }

            //FileInfo file = new FileInfo(FilePath);
            //if (file.Exists)
            //{
            //    file.Delete();
            //    file = new FileInfo(FilePath);
            //}

           
            //using (ExcelPackage package = new ExcelPackage(file))
            //{
            //    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SheetName");
            //    //添加列名
            //    for(int i=0;i<Properties.Count; i++)
            //    {
            //        T Item = Properties[i];
                    
            //    }
            //    worksheet.Cells[1, 1].Value = "ID";
            //    worksheet.Cells[1, 2].Value = "Name";
            //    worksheet.Cells[1, 3].Value = "Url";
               
            //    //添加数据
            //    worksheet.Cells["A2"].Value = 1000;
            //    worksheet.Cells["B2"].Value = "笑";
            //    worksheet.Cells["C2"].Value = "http://www.cnblogs.com/linezero/";

            //    worksheet.Cells["A3"].Value = 1001;
            //    worksheet.Cells["B3"].Value = "LineZero GitHub";
            //    worksheet.Cells["C3"].Value = "https://github.com/linezero";
            //    //添加样式
            //    worksheet.Cells["C3"].Style.Font.Bold = true;

            //    package.Save();
            //}
        }

        public string MD5Encrypt(string Data)
        {
            var md5 = MD5.Create();
            byte[] byt=md5.ComputeHash(Encoding.UTF8.GetBytes(Data));
            string result= Convert.ToBase64String(byt);
            return result;
        }

        public string HttpPost(string Data,string Urls)
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(new Uri(Urls));
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
                string result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();
                rsp.Dispose();
                return result;
                //可能存在的缺陷：捕获异常时，没关闭IO流
            }
            catch (Exception ex)
            {
                //GC.Collect();
                throw ex;
            }


        }

        public string HttpGet(string parms,string Urls)
        {
            string urls = Urls + parms;
            HttpClient httpClient = new HttpClient();
            Task<byte[]> Request = httpClient.GetByteArrayAsync(urls);
            Request.Wait();
            var Response = Encoding.UTF8.GetString(Request.Result);
            return Response;
        }

        /// <summary>
        /// jwt生成token
        /// </summary>
        /// <param name="Header">头部：格式:{ "typ": "JWT", "alg": "HS256"}</param>
        /// <param name="Payload">带标准格式的内容</param>
        /// <param name="SecretKey">秘钥</param>
        /// <returns></returns>
        public string CreateTokenByJWT(string Header,string Payload,string SecretKey)
        {
            var encodedString = EnBase64(Header) + "." + EnBase64(Payload);
            
            HMACSHA256 hash256 = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
            byte[] byteText= hash256.ComputeHash(Encoding.UTF8.GetBytes(encodedString));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < byteText.Length; i++)
            {
                sBuilder.Append(byteText[i].ToString("x2"));
            }
            string result = sBuilder.ToString();

            //string result = Encoding.UTF8.GetString(byteText);
            return result;
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public string EnBase64(string payload)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            string str = Convert.ToBase64String(bytes);
            return str;
        }

        public string DeBase64(string payload)
        {
            byte[] outputb = Convert.FromBase64String(payload);
            string orgStr = Encoding.UTF8.GetString(outputb);
            return orgStr;
        }


    



        //     public string GetToken(string UserName,string PassWord)
        //     {
        //         Task identity =null;
        //         if (UserName == "TEST" && PassWord == "TEST123")
        //         {
        //             identity=Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(UserName, "Token"), new Claim[] { }));
        //         }
        //         if(identity==null)
        //         {
        //             return "400";
        //         }
        //         TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
        //         var now = DateTime.UtcNow;
        //         var time = Math.Round((now.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        //         var claims = new Claim[]
        //         {
        //             new Claim(JwtRegisteredClaimNames.Sub, UserName),
        //             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //             new Claim(JwtRegisteredClaimNames.Iat, time.ToString(), ClaimValueTypes.Integer64)
        //         };
        //         return "";


        //         //var jwt = new JwtSecurityToken(
        ////issuer: _options.Issuer,
        ////audience: _options.Audience,
        ////claims: claims,
        ////notBefore: now,
        ////expires: now.Add(_options.Expiration),
        ////signingCredentials: _options.SigningCredentials);
        ////         var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        //     }
    }
}
