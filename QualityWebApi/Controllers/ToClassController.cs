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

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/ToClass")]
    public class ToClassController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        [HttpGet]
        public Object Get()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                BllZL_ToClass Bll = new BllZL_ToClass(con);
                var obj = Bll.GetClass("");
                return Bll.GetClass("");
            }
        }

    }
}