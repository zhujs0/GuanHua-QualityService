using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using System.Data.SqlClient;
using AppService;
using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Completed")]
    public class CompletedController : Controller
    {
        //        private string _connectionString = @"Data Source=USER-20170609DT\SQLEXPRESS;
        //Initial Catalog=QualityTest;Persist Security Info=True;User ID=sa;Password=123456";
        private string _connectionString = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("host.json", optional: true).Build().GetSection("QualitySource").Value;
        [HttpGet]
        public Object Get(string OrderNo, string WorkProcedure, string BatchNo, string Model, string EquipmentNo,
            string FeedbackMan,string StartTime,string EndTime)
        {
            
            OrderNo = IsNull(OrderNo);
            WorkProcedure = IsNull(WorkProcedure);
            BatchNo = IsNull(BatchNo);
            Model = IsNull(Model);
            EquipmentNo = IsNull(EquipmentNo);
            FeedbackMan = IsNull(FeedbackMan);
            StartTime = IsNull(StartTime);
            EndTime = IsNull(EndTime);
            string strWhere = @" where OrderNo like '%{0}%' and WorkProcedure like
 '%{1}%' and BatchNo like '%{2}%' and Model like '%{3}%' and EquipmentNo like '%{4}%' and FeedbackMan like '%{5}%' and 
 OrderNo in (select OrderNo from ZL_FeedbackExHandle) ";
            strWhere=string.Format(strWhere, OrderNo, WorkProcedure, BatchNo, Model, EquipmentNo, FeedbackMan);
            if (StartTime!=""&& StartTime!=null& EndTime!=""&& EndTime!=null)
            {
                strWhere += " and FeedbackTime between '" + StartTime + "' and '" + EndTime + " 23:59:59'";
            }
            strWhere += " order by FeedbackTime desc";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    FeedbackBaseService BllBase = new FeedbackBaseService(con);
                    FeedbackExReasonService BllReason = new FeedbackExReasonService(con);
                    FeedbackExProblemService BllPro = new FeedbackExProblemService(con);
                    FeedbackExHandleService BllHandle = new FeedbackExHandleService(con);
                    List<FeedbackBase> BaseList = BllBase.GetOrderInfoByWhere(strWhere);
                    List<Completed_Request> RequestList = new List<Completed_Request>();
                    if (BaseList != null && BaseList.Count > 0)
                    {
                        foreach (var node in BaseList)
                        {
                            Completed_Request Request = new Completed_Request();
                            //①基础信息
                            string Request_OrderNo = node.OrderNo;
                            Request.OrderNo = Request_OrderNo;
                            Request.BatchNo = node.BatchNo;
                            Request.WorkProcedure = node.WorkProcedure;
                            Request.Model = node.Model;
                            Request.FeedbackMan = node.FeedbackMan;
                            Request.FeedbackTime = node.FeedbackTime.ToString("yyyy-MM-dd HH:mm");
                            Request.EquipmentName = node.EquipmentName;
                            Request.EquipmentNo = node.EquipmentNo;
                            Request.Qty = node.Qty;
                            //②原因信息
                            Request.ReasonList = BllReason.GetReasonByWhere(" where OrderNo='" + Request_OrderNo + "'");
                            //③问题
                            List<FeedbackExProblem> ProList = BllPro.GetProblemByWhere(" where OrderNo = '" + Request_OrderNo + "'");
                            Request.ProblemList = GetProblemList(ProList, con);
                            //④处理
                            FeedbackExHandle feedbackExHandle = BllHandle.GetHandler(" where OrderNo='" + Request_OrderNo + "'").FirstOrDefault();
                            ExHandle ExHandle = new ExHandle();
                            if (feedbackExHandle!=null)
                            {
                                ExHandle.HandleMan = feedbackExHandle.HandleMan;
                                ExHandle.HandleTime = feedbackExHandle.HandleTime.ToString("yyyy-MM-dd HH:mm");
                                ExHandle.HandleSuggestion = feedbackExHandle.HandleSuggestion;
                                ExHandle.QualityClass = feedbackExHandle.QualityClass;
                                ExHandle.HandleNote = feedbackExHandle.HandleNote;
                            }
                            Request.Handler = ExHandle;
                            //⑤关联
                            //循环找出最顶层的批号
                            string Loop = node.BatchNo;
                            bool flag = true;
                            while (flag)
                            {
                                string strsql = "select ParentNo from ZL_ParentTable where ChildNo='" + Loop + "'";
                                Parent parent = con.Query<Parent>(strsql).FirstOrDefault();
                                if (parent != null)
                                {
                                    Loop = parent.ParentNo;
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            //根据最顶层批号进行递归算法，查找相关的批号
                            string strJson = "{\"ParentNo\":\"" + Loop + "\",\"ChildNo\":["+GetJson(Loop, con) +"]}";
                            Request.Relate = strJson;
                            RequestList.Add(Request);
                        }
                    }
                    return RequestList;
                }
                catch(Exception ex)
                {
                    string ErrMsg = ex.Message;
                    return "";
                }
                finally
                {
                    if(con.State!= ConnectionState.Closed)
                    {
                        con.Close();
                    }
                }
            }
        }

        public string GetJson(string BatchNo,SqlConnection con)
        {
            string strsql = "select * from ZL_ParentTable where ParentNo='" + BatchNo + "'";
            List<Parent> parent = con.Query<Parent>(strsql).AsList();
            string strJson = "";
            //strJson = "{\"ParentNo\":\"" + BatchNo + "\",\"ChildNo\":[";
            if (parent!=null&&parent.Count>0)
            {
               foreach (var item in parent)
                {
                    strJson += "{\"ParentNo\":\"" + item.ChildNo + "\",\"ChildNo\":[" +GetJson(item.ChildNo,con) + "]},";
                }
              
            }
            if(strJson!="")
            {
                strJson = strJson.Substring(0, strJson.LastIndexOf(","));
            }
            //strJson = strJson + "]}";
            return strJson;
        }


        public List<Problem> GetProblemList(List<FeedbackExProblem> ProList, SqlConnection con)
        {
            CodeService BllCode = new CodeService(con);
            List<Problem> NewPro = new List<Problem>(); 
            foreach(var node in ProList)
            {
                Problem pro = new Problem();
                pro.ProblemID = node.ProblemID;
                pro.ProblemDetails = node.ProblemDetails;
                pro.CodeString = node.CodeString;
                pro.Suggestion = node.Suggestion;
                pro.OrderNo = node.OrderNo;
                pro.PicturePath = node.PicturePath;
                Code code= BllCode.GetCodeByWhere(" where CodeString='" + node.CodeString + "'").FirstOrDefault();
                if(code!=null)
                {
                    pro.QualityClass = code.QualityClass;
                }
                NewPro.Add(pro);
            }
            return NewPro;
        }

        public string IsNull(string parm)
        {
            if(parm==null)
            {
                parm = "";
            }
            return parm;
        }
        
    }

    public class Completed_Request
    {
        public string OrderNo { get; set; }
        public string WorkProcedure { get; set; }
        public string BatchNo { get; set; }
        public string Model { get; set; }
        public string Qty { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public string FeedbackMan { get; set; }
        public string FeedbackTime { get; set; }
        public List<FeedbackExReason> ReasonList { get; set; }
        public List<Problem> ProblemList { get; set; }
        public ExHandle Handler { get; set; }
        public string Relate { get; set; }//json格式,递归获取
    }

    public class ExHandle
    {
        public string HandleMan { get; set; }
        public string HandleSuggestion { get; set; }
        public string HandleTime { get; set; }
        public string HandleNote { get; set; }
        public string QualityClass { get; set; }
    }

    public class Parent
    {
        public string ParentNo { get; set; }
        public string ChildNo { get; set; }
    }


    public class Completed_Post
    {

    }
    public class Completed_Reuslt
    {

    }
}