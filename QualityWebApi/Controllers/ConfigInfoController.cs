using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/ConfigInfo")]
    public class ConfigInfoController : Controller
    {
        [HttpGet]
        public Object Get()
        {
            string WaitConfirmStepID = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Confirm_StepID").Value;
            string WaitHandleStepID = new ConfigurationBuilder()
                                      .SetBasePath(Directory.GetCurrentDirectory())
                                      .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Wait_StepID").Value;
            string EditStepID = new ConfigurationBuilder()
                                      .SetBasePath(Directory.GetCurrentDirectory())
                                      .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Edit_StepID").Value;
            ConfigValue Config = new ConfigValue();
            Config.Confirm_StepID = WaitConfirmStepID;
            Config.Wait_StepID = WaitHandleStepID;
            Config.EditStepID = EditStepID;
            return Config;
        }
        public class ConfigValue
        {
            public string Confirm_StepID { get; set; }
            public string Wait_StepID { get; set; }
            public string EditStepID { get; set; }
        }
    }
}