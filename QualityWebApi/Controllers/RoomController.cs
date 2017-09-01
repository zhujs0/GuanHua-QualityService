using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using AppService;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Room")]
    public class RoomController : Controller
    {
        //        private string connectionString = @"Data Source=USER-20170609DT\SQLEXPRESS;
        //Initial Catalog=QualityTest;Persist Security Info=True;User ID=sa;Password=123456";
        private string connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        [HttpGet]
        public Object Get()
        {
            using (var sqlconnection = new SqlConnection(connectionString))
            {
                RoomService repository = new RoomService(sqlconnection);
                return repository.GetRoom();
            }
        }
    }
}