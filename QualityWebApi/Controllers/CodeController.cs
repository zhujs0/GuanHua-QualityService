using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppService;
using Domain;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Code")]
    public class CodeController : Controller
    {
        private string connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        [HttpGet]
        public Object GetCode(string RoomName,string TypeName,string Problem,string TopClass)
        {
            using (var sqlconnection = new SqlConnection(connectionString))
            {
                CodeService repository = new CodeService(sqlconnection);
                string strsql = " where 1=1 ";
                if(RoomName!=""&& RoomName!=null)
                {
                    strsql += " and RoomName='" + RoomName + "' ";
                }
                if (TypeName != "" && TypeName != null)
                {
                    strsql += " and TypeName='" + TypeName + "'";
                }
                if (Problem != "" && Problem != null)
                {
                    strsql += " and Problem='" + Problem + "'";
                }
                if (TopClass != "" && TopClass != null)
                {
                    strsql += " and TopClass='" + TopClass + "'";
                }
                return repository.GetCodeByWhere(strsql);
            }
        }

        [HttpPost]
        public Object AddCode([FromBody]Code code)
        {
            ReturnData result = new ReturnData();
            result.Msg = "";
            try
            {
                using (var sqlconnection = new SqlConnection(connectionString))
                {
                    CodeService repository = new CodeService(sqlconnection);
                    if (repository.AddCode(code))
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                if(ex.Message.Contains("重复键"))
                {
                    result.Msg = "已存在相同的编码，编码不能重复添加";
                }
                else
                {
                    result.Msg = ex.Message;
                }
                
                return result;
            }
        }

        [HttpDelete]
        public Object DelCode(string CodeString)
        {
            ReturnData result = new ReturnData();
            result.Msg = "";
            using (var sqlconnection = new SqlConnection(connectionString))
            {
                bool flag = true;
                if (sqlconnection.State == ConnectionState.Closed)
                {
                    sqlconnection.Open();
                }
                SqlTransaction transaction = sqlconnection.BeginTransaction();
                try
                {
                    CodeService repository = new CodeService(sqlconnection);
                    for (int i = 0; i < CodeString.Split(',').Length; i++)
                    {
                        string strCodeString = (CodeString.Split(','))[i].ToString();
                        if (!repository.DelCode((CodeString.Split(','))[i].ToString(), transaction))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        transaction.Commit();
                        result.Result = true;
                    }
                   else
                    {
                        result.Result = false;
                    }
                    return result;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    result.Msg = ex.Message;
                    result.Result = false;
                    return result;
                }
                finally
                {
                    sqlconnection.Close();
                }
            }
        }

        [HttpPut]
        public Object UpdateCode(Code code)
        {
            ReturnData result = new ReturnData();
            result.Msg = "";
            using (var sqlconnection = new SqlConnection(connectionString))
            {
                sqlconnection.Open();
                try
                {
                    CodeService repository = new CodeService(sqlconnection);
                    if(repository.UpdateCode(code))
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    if(ex.Message.Contains("重复键"))
                    {
                        result.Msg = "已存在相同的编码，编码不能重复添加";
                    }
                    else
                    {
                        result.Msg = ex.Message;
                    }
                    result.Result = false;
                    return result;
                }
                finally
                {
                    sqlconnection.Close();
                }
            }
        }
    }

    public class ReturnData
    {
        public string Msg { get; set; }
        public bool Result { get; set; }
    }



}