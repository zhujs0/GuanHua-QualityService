using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;
using AppService;
using System.IO;
using System.Data.SqlClient;
using QualityWebApi;
using QualityWebApi.Controllers;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //private string _ConString = new ConfigurationBuilder()
        //           .SetBasePath(Directory.GetCurrentDirectory())
        //           .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        private string _ConString = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("host.json", optional: true).Build().GetSection("ghSource").Value;
        [TestMethod]
        public void TestMethod1()
        {
            string ConString = "Data Source=10.10.152.113;Initial Catalog=gh;Persist Security Info=True;" +
                "User ID=productUser;Password=ghtest Product;Connect Timeout=500";
            RateTotalController Bll = new RateTotalController();
            using (SqlConnection con = new SqlConnection(ConString))
            {
                //con.Open();
                BllRateTotal Rate = new BllRateTotal(con);
                Stopwatch Stopwatch = new Stopwatch();
                Stopwatch.Start();
                //for (int i=0;i<30;i++)
                //{
                //    Bll.GetPut("", "", "", "", "2017-11-1", "2017-12-1", false, con, "1");
                //}
                Stopwatch.Stop();
                Console.WriteLine(Stopwatch);

            }
                


        }
    }
}
