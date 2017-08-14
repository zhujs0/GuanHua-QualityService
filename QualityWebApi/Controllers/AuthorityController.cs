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
    [Route("api/Authority")]
    public class AuthorityController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("QualitySource").Value;
        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Object Get(string employeeNo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                BllZL_PersonSetup bll = new BllZL_PersonSetup(con);
                return bll.GetPersonWork(" where EmployeeNo='" + employeeNo + "'");
            }
        }
    }
}