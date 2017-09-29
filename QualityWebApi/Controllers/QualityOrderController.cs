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
using Domain;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/QualityOrder")]
    public class QualityOrderController : Controller
    {
        private string _ConString = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;
        [HttpGet]
        public Object Get(string ActionType,long PageIndex,long PageSize, string OrderNo, 
            string WorkProcedure, string BatchNo, string Model, string EquipmentNo,
            string FeedbackMan, string StartTime, string EndTime, string Status)
        {
            //ActionType=GetQualityOrder获取质量反馈单
            if(ActionType== "GetQualityOrder")
            {
                return GetQualityOrder(PageIndex, PageSize, OrderNo, WorkProcedure, BatchNo, Model, EquipmentNo,
             FeedbackMan, StartTime, EndTime, Status);
            }
            else if(ActionType=="")
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        public Object GetQualityOrder(long PageIndex, long PageSize, string OrderNo,
            string WorkProcedure, string BatchNo, string Model, string EquipmentNo,
            string FeedbackMan, string StartTime, string EndTime, string Status)
        {
            using (SqlConnection con = new SqlConnection(_ConString))
            {
                try
                {
                    OrderNo = IsNull(OrderNo);WorkProcedure = IsNull(WorkProcedure);
                    BatchNo = IsNull(BatchNo);Model = IsNull(Model);
                    EquipmentNo = IsNull(EquipmentNo);FeedbackMan = IsNull(FeedbackMan);
                    StartTime = IsNull(StartTime);EndTime = IsNull(EndTime);
                    Status = IsNull(Status);
                    string strWhere = @" where OrderNo like '%{0}%' and WorkProcedure like
 '%{1}%' and BatchNo like '%{2}%' and Model like '%{3}%' and EquipmentNo like '%{4}%' and FeedbackMan like '%{5}%'  ";
                    strWhere = string.Format(strWhere, OrderNo, WorkProcedure, BatchNo, Model, EquipmentNo, FeedbackMan);
                    if (StartTime != "" &&  EndTime != "" )
                    {
                        strWhere += " and FeedbackTime between '" + StartTime + "' and '" + EndTime + " 23:59:59'";
                    }
                    if (Status == "T1")
                    {
                        //待处理
                        strWhere += " and Status!='P' ";
                    }
                    else if (Status == "P")
                    {
                        //已完成
                        strWhere += " and Status='P' ";
                    }
                    else if (Status == "B")
                    {
                        //待审批
                        strWhere += " and Status='B'";
                    }
                    BllApprovalStream Bll = new BllApprovalStream(con);
                    FeedbackBaseService BllBase = new FeedbackBaseService(con);
                    FeedbackExReasonService BllReason = new FeedbackExReasonService(con);
                    FeedbackExProblemService BllPro = new FeedbackExProblemService(con);
                    BllProductCard BllCard = new BllProductCard(con);
                    List<ReturnData> RequestList = new List<ReturnData>();
                    long RowCount = 0, PageCount = 0;
                    List<FeedbackBase> BaseList = BllBase.GetQualityOrder(PageIndex,PageSize,strWhere,out RowCount);

                    foreach(var node in BaseList)
                    {
                        ReturnData Request = new ReturnData();
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
                        Request.Status = node.Status;
                        Request.ProductClass = node.ProductClass;
                        //②原因信息
                        Request.ReasonList = BllReason.GetReasonByWhere(" where OrderNo='" + Request_OrderNo + "'");
                        //③问题
                        List<FeedbackExProblem> ProList = BllPro.GetProblemByWhere("  where OrderNo = '" + Request_OrderNo + "'");
                        Request.ProblemList = GetProblemList(ProList, con);
                        //④处理
                        Request.ApStream = Bll.GetStream(" where OrderNo='" + node.OrderNo + "'");
                        //⑤客户列表
                        Request.CardList = BllCard.GetCard(" where FKOrderNo='" + node.OrderNo + "'");
                        Request.RowCount = RowCount;
                        RequestList.Add(Request);
                    }
                    return RequestList;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<Problem> GetProblemList(List<FeedbackExProblem> ProList, SqlConnection con)
        {
            CodeService BllCode = new CodeService(con);
            List<Problem> NewPro = new List<Problem>();
            foreach (var node in ProList)
            {
                Problem pro = new Problem();
                pro.ProblemID = node.ProblemID;
                pro.ProblemDetails = node.ProblemDetails;
                pro.CodeString = node.CodeString;
                pro.Suggestion = node.Suggestion;
                pro.OrderNo = node.OrderNo;
                pro.PicturePath = node.PicturePath;
                Code code = BllCode.GetCodeByWhere(" where CodeString='" + node.CodeString + "'").FirstOrDefault();
                pro.code = code;
                if (code != null)
                {
                    pro.QualityClass = code.QualityClass;
                }
                NewPro.Add(pro);
            }
            return NewPro;
        }

        public string IsNull(string parm)
        {
            if (parm == null)
            {
                parm = "";
            }
            return parm;
        }
        public class ReturnData
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
            public string Status { get; set; }
            public List<ApprovalStream> ApStream { get; set; }
            public long RowCount { get; set; }
            public List<ProductCard.CardInfo> CardList { get; set; }
            public string ProductClass { get; set; }
        }
    }

}