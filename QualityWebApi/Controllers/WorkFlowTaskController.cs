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
    [Route("api/WorkFlowTask")]
    public class WorkFlowTaskController : Controller
    {
        private string conString = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        private string WaitConfirmStepID = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Confirm_StepID").Value;
        private string WaitHandleStepID = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Wait_StepID").Value;
        [HttpPost]
        public bool UpdateOrderStatus([FromBody] FormData formData)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                FeedbackBaseService Bll = new FeedbackBaseService(con);
                if(Bll.UpdateStatus(formData.ID, formData.StepID, formData.Status))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        public class FormData
        {
            public string ID { get; set; }
            public int Status { get; set; }
            public string StepID { get; set; }

        }



    }
}