using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using AppService;
using Domain;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Employee")]
    public class EmployeeController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("host.json", optional: true).Build().GetSection("KQSource").Value;
        [HttpGet]
        public Object Get(string EmployeeNo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                ResponseData data = new ResponseData();
                BllKQ bll = new BllKQ(con);
                KQ kq = bll.GetUserById(EmployeeNo);
                if(kq!=null)
                {
                    data.EmployeeName = kq.NAME;
                }
                return data;
            }
        }
        public class ResponseData
        {
            public string EmployeeName { get; set; }
        }
    }
}