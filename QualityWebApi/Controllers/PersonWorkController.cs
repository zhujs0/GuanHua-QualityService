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
using System.Data;
using Domain;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/PersonWork")]
    public class PersonWorkController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("QualitySource").Value;
        [HttpGet]
        public Object Get(string WorkProduct, string WorkName)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                if(WorkName==null)
                {
                    WorkName = "";
                }
                if(WorkProduct==null)
                {
                    WorkProduct = "";
                }
                BllZL_PersonSetup bll = new BllZL_PersonSetup(con);
                string strWhere = "where WorkProduct like '%{0}%' and WorkName like '%{1}%'";
                strWhere = string.Format(strWhere, WorkProduct, WorkName);
                //List<ZL_PersonSetup> person= bll.GetPersonWork(strWhere);
                return bll.GetPersonWork(strWhere);
            }
        }
        [HttpPost]
        public ResponseData AddPerson([FromBody]ZL_PersonSetup person)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                ResponseData response = new ResponseData();
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlTransaction tran = con.BeginTransaction();
                    BllZL_PersonSetup bll = new BllZL_PersonSetup(con);
                    if (bll.AddPerson(person, tran))
                    {
                        tran.Commit();
                        response.result = true;
                        response.msg = "添加成功";
                    }
                    else
                    {
                        response.result = false;
                        response.msg = "添加失败";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.msg = ex.Message;
                }
                return response;
            }
        }
        [HttpDelete]
        public ResponseData DelPerson(long PersonAutoID)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                ResponseData response = new ResponseData();
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlTransaction tran = con.BeginTransaction();
                    BllZL_PersonSetup bll = new BllZL_PersonSetup(con);
                    if (bll.DelPerson(PersonAutoID, tran))
                    {
                        tran.Commit();
                        response.result = true;
                        response.msg = "删除成功";
                    }
                    else
                    {
                        response.result = false;
                        response.msg = "删除失败";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.msg = ex.Message;
                }
                return response;
            }
        }
        [HttpPut]
        public ResponseData UpdatePerson([FromBody]ZL_PersonSetup person)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                ResponseData response = new ResponseData();
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlTransaction tran = con.BeginTransaction();
                    BllZL_PersonSetup bll = new BllZL_PersonSetup(con);
                    if (bll.UpdatePerson(person, tran))
                    {
                        tran.Commit();
                        response.result = true;
                        response.msg = "添加成功";
                    }
                    else
                    {
                        response.result = false;
                        response.msg = "添加失败";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.msg = ex.Message;
                }
                return response;
            }
        }

        public class RequestData
        {
            public string EmployeeNo { get; set; }
            public string EmployeeName { get; set; }
            public string WorkProduct { get; set; }
            public string WorkName { get; set; }
        }

        public class ResponseData
        {
            public bool result { get; set; }
            public string msg { get; set; }
        }


    }
}