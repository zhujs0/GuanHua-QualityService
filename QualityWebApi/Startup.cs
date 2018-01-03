using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QualityWebApi.Filter.Domain;
using QualityWebApi.Filter.ef;
using QualityWebApi.Filter.BLL;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;


namespace QualityWebApi
{
    public class Startup
    {
        private string _issuer;
        private string _audience;
        private string _singingkey;
        private string _appAudience;
        private string _commandHost;
        private string _tokenHost;
        private string _client_id;
        private string _token;
        private string _dbConnectString;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            #region=====安全验证=====
            //_issuer = Configuration["Issuer"];
            //_audience = Configuration["Audience"];
            //_singingkey = Configuration["SigningKey"];
            //_appAudience = Configuration["AppAudience"];
            //_commandHost = Configuration["CommonHost"];
            //_tokenHost = Configuration["TokenHost"];
            //_client_id = Configuration["ClientId"];
            //_token = "bearer " + GetToken().Result;
            //_dbConnectString = GetDbconnect().Result;
            #endregion
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors();
            #region=====安全验证=====
            //services.AddDbContext<GhDbContext>(options =>
            //{
            //    options.UseSqlServer(_dbConnectString);
            //});
            #endregion
            services.AddDirectoryBrowser();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "质量管理Api"
                });
            });

            services.AddTimedJob();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseSwagger();
            #region=====安全验证=====
            //var audience = GetAudience(_appAudience);
            //if (audience == null)
            //{
            //    return;
            //}
            //_issuer = audience.Issuer;
            //_audience = audience.ClientId;
            //_singingkey = audience.Base64Secret;

            //var keyByteArry = Base64UrlEncoder.DecodeBytes(_singingkey);

            //SymmetricSecurityKey signingKey = new SymmetricSecurityKey(keyByteArry);

            ////var sing = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    TokenValidationParameters = new TokenValidationParameters
            //    {
            //        IssuerSigningKey = signingKey,
            //        ValidAudience = _audience,
            //        ValidIssuer = _issuer,
            //        ValidateLifetime = true,
            //        ClockSkew = TimeSpan.Zero
            //    }
            //});
            #endregion


            //访问静态资源
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"ProblemPicture")),
                RequestPath = new PathString("/ProblemPicture")
                //EnableDirectoryBrowsing = true
            });
            //生成web api测试页面
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "质量管理Api");
            });
            //允许所有跨域访问
            app.UseCors(configurePolicy =>
            {
                configurePolicy.AllowAnyOrigin();
                configurePolicy.AllowAnyHeader();
                configurePolicy.AllowAnyMethod();
            });

            app.UseTimedJob();

            app.UseMvc();
        }

        private Audience GetAudience(string name)
        {
            var options = new DbContextOptionsBuilder<GhDbContext>();
            string connectstr = Configuration["DbConnection"];
            options.UseSqlServer(connectstr);
            var dbContext = new GhDbContext(options.Options);
            AudienceService service = new AudienceService(dbContext);
            return service.GetByName(name);

        }

        private async Task<string> GetDbconnect()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", _token);
            HttpResponseMessage response = await client.GetAsync(_commandHost + "/api/Dbconnect");

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return "";
            }

        }
        private async Task<string> GetToken()
        {
            HttpClient client = new HttpClient();
            string data = string.Format("client_id={0}&grant_type=password&userName=应用服务", _client_id);
            StringContent content = new StringContent(data);
            HttpResponseMessage response = await client.PostAsync(_tokenHost + "/token", content);
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                Token token = JsonConvert.DeserializeObject<Token>(result);
                return token.Access_token.Split(',')[0];


            }
            else
            {
                return "";
            }


        }
        private class Token
        {
            public string Access_token { get; set; }
            public string Token_type { get; set; }
            public string Expires_in { get; set; }
        }
    }
}
