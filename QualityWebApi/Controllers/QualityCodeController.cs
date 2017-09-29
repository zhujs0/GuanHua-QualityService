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
    [Route("api/QualityCode")]
    public class QualityCodeController : Controller
    {
        private string _connectionString = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        [HttpGet]
        public Object Get(string ActionType,string CodeString,string RoomName,string TypeName,string Problem,string TopClass)
        {
            if(ActionType=="SelectCode")
            {
                if (CodeString != null && CodeString != "")
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        CodeService BllCode = new CodeService(con);
                        return BllCode.GetCodeByWhere("  where CodeString like '%" + CodeString + "%'");
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (ActionType == "FuzzyByItem")
            {
                using (var sqlconnection = new SqlConnection(_connectionString))
                {
                    CodeService repository = new CodeService(sqlconnection);
                    string strsql = " where 1=1 ";
                    if (RoomName != "" && RoomName != null)
                    {
                        strsql += " and RoomName='" + RoomName + "' ";
                    }
                    if (TypeName != "" && TypeName != null)
                    {
                        strsql += " and TypeName like '%" + TypeName + "%'";
                    }
                    if (Problem != "" && Problem != null)
                    {
                        strsql += " and Problem like '%" + Problem + "%'";
                    }
                    if (TopClass != "" && TopClass != null)
                    {
                        strsql += " and TopClass='" + TopClass + "'";
                    }
                    return repository.GetCodeByWhere(strsql);
                }
            }
            else
            {
                return null;
            }
        }
    }
}