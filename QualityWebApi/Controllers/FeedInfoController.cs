using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using AppService;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/FeedInfo")]
    public class FeedInfoController : Controller
    {
        //        private string _connectionString = @"Data Source=USER-20170609DT\SQLEXPRESS;
        //Initial Catalog=QualityTest;Persist Security Info=True;User ID=sa;Password=123456";
        private string _connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("QualitySource").Value;
        [HttpGet]
        public Object Get(string OrderNo, string BatchNo, string WorkProcedure, string StartTime, string EndTime)
        {
            string strWhere = "";
            if (OrderNo == null)
            {
                OrderNo = "";
            }
            if (BatchNo == null)
            {
                BatchNo = "";
            }
            if (WorkProcedure == null)
            {
                WorkProcedure = "";
            }
            strWhere = " where OrderNo not in (select OrderNo  from ZL_FeedbackExHandle ) and OrderNo like '%" + OrderNo + "%' and BatchNo like '%" + BatchNo + "%'";
            if (WorkProcedure != "")
            {
                strWhere += " and WorkProcedure='" + WorkProcedure + "'";
            }
            if (StartTime != null && StartTime != "" && EndTime != null && EndTime != "")
            {
                strWhere += " and FeedbackTime between '" + StartTime + "' and '" + EndTime + " 23:59:59'";
            }
            strWhere += " order by FeedbackTime desc";
            using (SqlConnection sqlconnection = new SqlConnection(_connectionString))
            {
                FeedbackBaseService bllBase = new FeedbackBaseService(sqlconnection);
                FeedbackExReasonService bllReason = new FeedbackExReasonService(sqlconnection);
                FeedbackExProblemService bllPro = new FeedbackExProblemService(sqlconnection);
                CodeService bllCode = new CodeService(sqlconnection);

                List<FeedbackBase> baseList = bllBase.GetOrderInfoByWhere(strWhere);
                List<FeedInfo> InfoList = new List<FeedInfo>();
                if (baseList != null && baseList.Count > 0)
                {
                    for (int i = 0; i < baseList.Count; i++)
                    {
                        FeedInfo info = new FeedInfo();
                        info.OrderNo = baseList[i].OrderNo;
                        info.WorkProcedure = baseList[i].WorkProcedure;
                        info.Model = baseList[i].Model;
                        info.BatchNo = baseList[i].BatchNo;
                        info.Qty = baseList[i].Qty;
                        info.EquipmentName = baseList[i].EquipmentName;
                        info.EquipmentNo = baseList[i].EquipmentNo;
                        info.FeedbackMan = baseList[i].FeedbackMan;
                        info.FeedbackTime = baseList[i].FeedbackTime.ToString("yyyy-MM-dd HH:mm");
                        List<FeedbackExReason> reason = bllReason.GetReasonByWhere(" where OrderNo='" + baseList[i].OrderNo + "'");
                        info.ReasonList = reason;
                        List<FeedbackExProblem> problem = bllPro.GetProblemByWhere(" where OrderNo='" + baseList[i].OrderNo + "'");
                        List<Problem> problemList = new List<Problem>();
                        if (problem != null && problem.Count > 0)
                        {
                            for (int j = 0; j < problem.Count; j++)
                            {
                                Problem newProblem = new Problem();
                                string QualityClass = "";
                                string Suggestion = "";
                                string CodeString = problem[j].CodeString;
                                Code code = bllCode.GetCodeByWhere(" where CodeString='" + CodeString + "'").FirstOrDefault();
                                if (code != null)
                                {
                                    QualityClass = code.QualityClass;
                                    Suggestion = code.Suggestion;
                                }
                                newProblem.QualityClass = QualityClass;
                                newProblem.Suggestion = Suggestion;
                                newProblem.CodeString = CodeString;
                                newProblem.OrderNo = baseList[i].OrderNo;
                                newProblem.PicturePath = problem[j].PicturePath;
                                newProblem.ProblemDetails = problem[j].ProblemDetails;
                                newProblem.ProblemID = problem[j].ProblemID;
                                problemList.Add(newProblem);
                            }
                        }
                        info.ProblemList = problemList;
                        InfoList.Add(info);
                    }
                }
                return InfoList;
            }
        }


        [HttpPost]
        public Object Post([FromBody] PostData postdata)
        {
            FeedbackExHandle handler = new FeedbackExHandle();
            handler.HandleMan = postdata.HandleMan;
            handler.HandleSuggestion = postdata.HandleSuggestion;
            handler.OrderNo = postdata.OrderNo;
            handler.HandleNote = postdata.HandleNote;
            handler.QualityClass = postdata.QualityClass;
            handler.HandleTime = DateTime.Now;
            RequestResult result = new RequestResult();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    FeedbackExHandleService bllHandler = new FeedbackExHandleService(con);
                    if (bllHandler.AddHandle(handler, tran))
                    {
                        FeedbackExProblemService bllPro = new FeedbackExProblemService(con);
                        List<PostData_Problem> ProList = postdata.Post_Problem;
                        if (ProList != null && ProList.Count > 0)
                        {
                            for (int i = 0; i < ProList.Count; i++)
                            {
                                long ProblemID = ProList[i].ProblemID;
                                string Suggestion = ProList[i].Suggestion;
                                if (!bllPro.UpdateSuggestion(Suggestion, ProblemID, tran))
                                {
                                    Exception ex = new Exception("±£´æÊ§°Ü");
                                    throw ex;
                                }
                            }
                        }
                        tran.Commit();
                        result.Result = true;
                        result.ResultMsg = "±£´æ³É¹¦";
                    }
                    else
                    {
                        result.Result = false;
                        result.ResultMsg = "±£´æÊ§°Ü";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result.Result = false;
                    result.ResultMsg = ex.Message;
                    return result;
                }
                finally
                {
                    con.Close();
                }

            }
        }

        [HttpDelete]
        public Object Delete(string OrderNo)
        {
            RequestResult result = new RequestResult();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    FeedbackBaseService service = new FeedbackBaseService(con);
                    List<FeedbackBase> BaseList = service.GetOrderInfoByWhere(" where OrderNo='" + OrderNo + "'");
                    SqlTransaction tran = con.BeginTransaction();
                    if(BaseList!=null&&BaseList.Count>0)
                    {
                        if (service.Delete(OrderNo, tran))
                        {
                            tran.Commit();
                            result.Result = true;
                        }
                        else
                        {
                            result.Result = false;
                            result.ResultMsg = "É¾³ýÊ§°Ü";
                        }
                    }
                    else
                    {
                        result.Result = true;
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ResultMsg = "É¾³ýÊ§°Ü";
                    return result;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        //[HttpGet]
        //public Object Get(string OrderNo,string ReasonType,string ProblemCode)
        //{
        //    using (SqlConnection sqlconnection = new SqlConnection(_connectionString))
        //    {
        //        string strWhere = "";
        //        FeedbackBaseService fbs = new FeedbackBaseService(sqlconnection);
        //        FeedbackExReasonService ReasonBll = new FeedbackExReasonService(sqlconnection);
        //        FeedbackExProblemService ProblemBll = new FeedbackExProblemService(sqlconnection);
        //        if (OrderNo!=""&&OrderNo!=null)
        //        {
        //            strWhere = " where OrderNo not in (select FeedbackExHandle.OrderNo from FeedbackExHandle) and OrderNo like '%" + OrderNo + "%'";
        //        }
        //        List<FeedInfo> info = new List<FeedInfo>();
        //        List<FeedbackBase> ListBase = fbs.GetOrderInfoByWhere(strWhere);
        //        if(ListBase != null&& ListBase.Count>0)
        //        {
        //            for(int i=0;i< ListBase.Count;i++)
        //            {
        //                bool flag = true;
        //                FeedInfo item = new FeedInfo();
        //                string orderNo = ListBase[i].OrderNo;
        //                item.OrderNo = orderNo;
        //                item.WorkProcedure = ListBase[i].WorkProcedure;
        //                item.BatchNo = ListBase[i].BatchNo;
        //                item.Model = ListBase[i].Model;
        //                item.Qty = ListBase[i].Qty;
        //                item.EquipmentName = ListBase[i].EquipmentName;
        //                item.EquipmentNo = ListBase[i].EquipmentNo;
        //                item.FeedbackMan = ListBase[i].FeedbackMan;
        //                item.FeedbackTime = ListBase[i].FeedbackTime.ToString("yyyy-MM-dd hh:mm");
        //                List<FeedbackExReason> reason = new List<FeedbackExReason>();
        //                reason = ReasonBll.GetReasonByWhere(" where  OrderNo='" + orderNo + "'");
        //                item.ReasonList = reason;
        //                if (ReasonType!=""&&ReasonType!=null)
        //                {
        //                    reason = ReasonBll.GetReasonByWhere(" where ReasonType='" + ReasonType + "' and OrderNo='" + orderNo + "'");
        //                    if (reason != null&&reason.Count>0)
        //                    {
        //                        item.ReasonList = reason;
        //                    }
        //                    else
        //                    {
        //                        flag = false;
        //                    }
        //                }
        //                if (flag)
        //                {
        //                    List<FeedbackExProblem> problem = new List<FeedbackExProblem>();
        //                    problem = ProblemBll.GetProblemByWhere(" where  OrderNo='" + orderNo + "'");
        //                    item.ProblemList = problem;
        //                    if (ProblemCode != "" && ProblemCode != null)
        //                    {
        //                        problem = ProblemBll.GetProblemByWhere(" where CodeString='" + ProblemCode + "' and OrderNo='" + orderNo + "'");
        //                        if (problem!=null&& problem.Count>0)
        //                        {
        //                            item.ProblemList = problem;
        //                        }
        //                        else
        //                        {
        //                            flag = false;
        //                        }
        //                    }
        //                }
        //                if(flag)
        //                {
        //                    info.Add(item);
        //                }
        //            }
        //        }
        //        return info;
        //    }
        //}
    }

    //public class FeedInfo
    //{
    //    public string OrderNo { get; set; }
    //    public string WorkProcedure { get; set; }
    //    public string BatchNo { get; set; }
    //    public string Model { get; set; }
    //    public string Qty { get; set; }
    //    public string EquipmentName { get; set; }
    //    public string EquipmentNo { get; set; }
    //    public string FeedbackMan { get; set; }
    //    public string FeedbackTime { get; set; }
    //    public List<FeedbackExReason> ReasonList { get; set; }
    //    public List<FeedbackExProblem> ProblemList { get; set; }
    //}

    public class RequestResult
    {
        public string ResultMsg { get; set; }
        public bool Result { get; set; }
    }
    public class PostData
    {
        public string HandleMan { get; set; }
        public string HandleSuggestion { get; set; }
        public string OrderNo { get; set; }
        public string HandleNote { get; set; }
        public string QualityClass { get; set; }
        public List<PostData_Problem> Post_Problem { get; set; }
    }

    public class PostData_Problem
    {
        public long ProblemID { get; set; }
        public string Suggestion { get; set; }
    }

    public class FeedInfo
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
    }

    public class Problem
    {
        public long ProblemID { get; set; }
        public string CodeString { get; set; }
        public string ProblemDetails { get; set; }
        public string PicturePath { get; set; }
        public string OrderNo { get; set; }
        public string Suggestion { get; set; }
        public string QualityClass { get; set; }
    }
}