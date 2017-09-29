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
using AppService;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Material")]
    public class MaterialController : Controller
    {
        private string _ConString = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        [HttpGet]
        public Object Get(string ActionType,string chrType,long PageIndex,long PageSize)
        {
            if(ActionType=="Select")
            {
                string strWhere = " where chrType like '%" + chrType + "%' ";
                long RowAmount = 0;
                using (SqlConnection con = new SqlConnection(_ConString))
                {
                    BllMaterial Bll = new BllMaterial(con);
                    return Bll.Get(PageIndex, PageSize, strWhere,out RowAmount);
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public Object Post([FromBody] Material.MStandard newMaterial)
        {
            ReturnData Result = new ReturnData();
            try
            {
                SqlConnection con = new SqlConnection(_ConString);
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    BllMaterial Bll = new BllMaterial(con);
                    if (Bll.InsertOrUpdate(newMaterial, tran))
                    {
                        tran.Commit();
                        Result.Result = true;
                        Result.Msg = "³É¹¦";
                    }
                    else
                    {
                        tran.Rollback();
                        Result.Result = false;
                        Result.Msg = "Ê§°Ü";
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Result.Result = false;
                    Result.Msg = ex.Message;
                }
                finally
                {
                    con.Close();
                }
                return Result;
            }
            catch(Exception ex)
            {
                Result.Result = false;
                Result.Msg = ex.Message;
                return Result;
            }
        }

    }
}