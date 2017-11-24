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

namespace QualityWebApi.Common
{
    public class GeneralMethod
    {
        public GeneralMethod()
        {

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
