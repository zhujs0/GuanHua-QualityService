using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;
using AppService;
using System.IO;
using System.Data.SqlClient;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //private string _ConString = new ConfigurationBuilder()
        //           .SetBasePath(Directory.GetCurrentDirectory())
        //           .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        [TestMethod]
        public void TestMethod1()
        {
            string ConString = @"Data Source=10.10.152.75;
Initial Catalog=GHLPY;Persist Security Info=True;User ID=productUser;Password=ghtest Product";
           
               
        }
    }
}
