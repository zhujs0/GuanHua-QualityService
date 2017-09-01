using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using Domain;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/ControllTable")]
    public class ControllTableController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        [HttpGet]
        public Object Get(string WorkProcedure,string ApprovalStatus,string ActionType, string OrderNo,string BatchNo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                if(ActionType == "GetList")
                {
                    AppService.BllZL_ControllTable Bll = new AppService.BllZL_ControllTable(con);
                    return Bll.GetByWhere(@"  join ZL_FeedbackBase on ZL_FeedbackBase.OrderNo=ZL_ControllTable.OrderNo
 where ZL_FeedbackBase.WorkProcedure = '" + WorkProcedure + "' and ZL_ControllTable.ApprovalStatus='" + ApprovalStatus + "'");
                }
                else if(ActionType == "GetControll")
                {
                    AppService.BllZL_ControllTable Bll = new AppService.BllZL_ControllTable(con);
                    return Bll.GetByWhere(" where OrderNo='"+ OrderNo + "' and BatchNo='"+ BatchNo + "'");
                }
                else
                {
                    return null;
                }
               
            }
        }
        [HttpPost]
        public Object Save([FromBody] ControllTable Controll)
        {
            ResultData result = new ResultData();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    AppService.BllZL_ControllTable Bll = new AppService.BllZL_ControllTable(con);
                    if(Bll.SaveControll(Controll))
                    {
                        result.Result = true;
                        result.ErrMsg = "";
                    }
                    else
                    {
                        result.Result = false;
                        result.ErrMsg = "±£¥Ê ß∞‹";
                    }

                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ErrMsg = ex.Message;
                }
            }
                return result;
        }
    }
}