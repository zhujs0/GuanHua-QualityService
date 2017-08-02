using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Domain;
using AppService;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Machine")]
    public class MachineController : Controller
    {
//        private string _ConnectionString= @"Data Source=10.10.152.75;
//Initial Catalog=GHLPY;Persist Security Info=True;User ID=productUser;Password=ghtest Product";

        private string _ConnectionString = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        [HttpGet]
        public Object Get(string chrMachineID)
        {
            using (SqlConnection sqlconnection = new SqlConnection(_ConnectionString))
            {
                FeedbackBaseService service = new FeedbackBaseService(sqlconnection);
                return service.GetMachine(chrMachineID);
            }
        }
    }
}