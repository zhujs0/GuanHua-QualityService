using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using AppService;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Work")]
    public class WorkController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("QualitySource").Value;
        [HttpGet]
        public Object Get()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                BllZL_WorkSetup bll = new BllZL_WorkSetup(con);
                return bll.GetWork("");
            }
        }
    }
}