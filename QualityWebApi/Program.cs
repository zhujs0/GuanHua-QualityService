using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace QualityWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                //方法一：Timer time = new Timer(Tick, "----------------------", 5000, 1000),每秒判断时间是否为23：59分，执行任务
                //方法二：数据库计划任务


                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build();
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseConfiguration(config)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();

                host.Run();
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        static void Tick(object data)
        {
            Console.WriteLine(data);
        }
    }
}
