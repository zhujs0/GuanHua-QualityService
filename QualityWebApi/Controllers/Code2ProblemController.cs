using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using AppService;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Code2Problem")]
    public class Code2ProblemController : Controller
    {
//        private string _connectionString = @"Data Source=USER-20170609DT\SQLEXPRESS;
//Initial Catalog=QualityTest;Persist Security Info=True;User ID=sa;Password=123456";
        [HttpGet]
        public Object Get(string CodeString)
        {
            string _connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
            if (CodeString != null && CodeString != "")
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    CodeService BllCode = new CodeService(con);
                    Code code = BllCode.GetCodeByWhere("  where CodeString='" + CodeString + "'").FirstOrDefault();
                    return code;
                }
            }
            else
            {
                return null;
            }
        }
    }
}