using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/ApprovalStream")]
    public class ApprovalStreamController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        [HttpGet]
        public Object Get(string OrderNo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                AppService.BllApprovalStream Bll = new AppService.BllApprovalStream(con);
                return Bll.GetStream(OrderNo);
            }
        }
    }
}