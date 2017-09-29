using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using Domain;
using System.Data.SqlClient;
using System.Data;
using AppService;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        private string conString = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        [HttpGet]
        public Object Get(string ActionType, long PageIndex, long PageSize, string OrderNo,
            string WorkProcedure, string BatchNo, string Model, string EquipmentNo,
            string FeedbackMan, string StartTime, string EndTime, string Status, int OrderType)
        {
            if (ActionType == "GetOrderInfo")
            {
                return GetOrderInfo(PageIndex, PageSize, OrderNo,WorkProcedure, BatchNo, Model, EquipmentNo,
                                    FeedbackMan, StartTime, EndTime, Status, OrderType);
            }
            else
            {
                return null;
            }
           
        }

        public Object GetOrderInfo( long PageIndex, long PageSize, string OrderNo,
            string WorkProcedure, string BatchNo, string Model, string EquipmentNo,
            string FeedbackMan, string StartTime, string EndTime, string Status, int OrderType)
        {
            OrderNo = OrderNo != null ? OrderNo : "";
            WorkProcedure = WorkProcedure != null ? WorkProcedure : "";
            BatchNo = BatchNo != null ? BatchNo : "";
            Model = Model != null ? Model : "";
            EquipmentNo = EquipmentNo != null ? EquipmentNo : ""; ;
            FeedbackMan = FeedbackMan != null ? FeedbackMan : "";
            StartTime = StartTime != null ? StartTime : "";
            EndTime = EndTime != null ? EndTime : "";
            Status = Status != null ? Status : "";
            string strWhere = @" where OrderNo like '%{0}%' and WorkProcedure like
 '%{1}%' and BatchNo like '%{2}%' and Model like '%{3}%' and EquipmentNo like '%{4}%' and FeedbackMan like '%{5}%'  ";
            strWhere = string.Format(strWhere, OrderNo, WorkProcedure, BatchNo, Model, EquipmentNo, FeedbackMan);
            if (StartTime != "" & EndTime != "")
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
            else if(Status=="W")
            {
                strWhere += " and Status=''";
            }
            strWhere += " and OrderType=" + OrderType+ "  ";
            using (SqlConnection con = new SqlConnection(conString))
            {
                try
                {
                    BllApprovalStream BllStream = new BllApprovalStream(con);
                    FeedbackBaseService BllBase = new FeedbackBaseService(con);
                    FeedbackExReasonService BllReason = new FeedbackExReasonService(con);
                    FeedbackExProblemService BllPro = new FeedbackExProblemService(con);
                    BllProductCard BllCard = new BllProductCard(con);
                    long RowCount = 0, PageCount = 0;
                    List<FeedbackBase> BaseList = BllBase.GetQualityOrder(PageIndex, PageSize, strWhere, out RowCount);
                    List<OrderInfo> OrderList = new List<OrderInfo>();
                    if (BaseList != null && BaseList.Count > 0)
                    {
                        foreach (var node in BaseList)
                        {
                            OrderInfo Item = new OrderInfo();
                            //①基础信息
                            Item.OrderNo = node.OrderNo;
                            Item.BatchNo = node.BatchNo;
                            Item.Model = node.Model;
                            Item.Qty = node.Qty;
                            Item.ProductClass = node.ProductClass;
                            Item.WorkProcedure = node.WorkProcedure;
                            Item.EquipmentName = node.EquipmentName;
                            Item.EquipmentNo = node.EquipmentNo;
                            Item.FeedbackMan = node.FeedbackMan;
                            Item.FeedbackTime = node.FeedbackTime.ToString("yyyy-MM-dd HH:mm");
                            Item.ProblemLevel = node.ProblemLevel;
                            Item.Report = node.Report;
                            Item.Measure = node.Measure;
                            Item.Status= node.Status;
                            //②质量问题
                            List<FeedbackExProblem> Problem = BllPro.GetProblemByWhere(" where OrderNo='" + node.OrderNo + "'");
                            List<OrderProblem> OrderProblem = new List<OrderProblem>();
                            foreach (var list in Problem)
                            {
                                OrderProblem Obj_Problem = new OrderProblem();
                                Obj_Problem.codeString = list.CodeString;
                                Obj_Problem.topClass = list.TopClass;
                                Obj_Problem.roomName = list.RoomName;
                                Obj_Problem.typeName = list.TypeName;
                                Obj_Problem.present = list.Present;
                                Obj_Problem.problem = list.Problem;
                                Obj_Problem.suggestion = list.Suggestion;
                                Obj_Problem.problemDetails = list.ProblemDetails;
                                Obj_Problem.qualityClass = list.QualityClass;
                                Obj_Problem.problemLevel = list.ProblemLevel;
                                OrderProblem.Add(Obj_Problem);
                            }
                            Item.ProblemData = OrderProblem;
                            //③原因分析
                            List<FeedbackExReason> Reason = BllReason.GetReasonByWhere(" where OrderNo='" + node.OrderNo + "'");
                            List<OrderReason> OrderReason = new List<OrderController.OrderReason>();
                            foreach (var list in Reason)
                            {
                                OrderReason Obj_Reason = new OrderReason();
                                Obj_Reason.reasonType = list.ReasonType;
                                Obj_Reason.reasonDetails = list.ReasonDetails;
                                Obj_Reason.orderNo = list.OrderNo;
                                OrderReason.Add(Obj_Reason);
                            }
                            Item.ReasonData = OrderReason;
                            //④客户信息
                            Item.CardList= BllCard.GetCard(" where FKOrderNo='" + node.OrderNo + "'");
                            //⑤反馈单处理意见
                            Item.ApprovalStream = BllStream.GetStream(" where OrderNo='" + node.OrderNo + "' and StreamType=0");
                            if(OrderType==1)
                            {
                                //⑥控制表处理意见
                                Item.ControlStream = BllStream.GetStream(" where OrderNo='" + node.OrderNo + "' and StreamType=1");
                            }
                            Item.RowCount = RowCount;
                            OrderList.Add(Item);
                        }
                    }
                    return OrderList;
                }
                catch (Exception ex)
                {
                    string ErrMsg = ex.Message;
                    return "";
                }
                finally
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                }
            }
        }

        [HttpPost]
        public Object Post([FromBody]OrderInfo PostData)
        {
            return InsertOrder(PostData);
        }
        public Object InsertOrder(OrderInfo PostData)
        {
            ResultData result = new ResultData();//返回数据类型
            SqlConnection con = new SqlConnection(conString);
            if(con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            //验证是否已存在该单号的单据，存在删除，再添加，不存在直接添加
            bool Delete = false;//false：不存在，true：存在
            string OrderNo = PostData.OrderNo!=null? PostData.OrderNo:"";
            FeedbackBaseService BllBase = new FeedbackBaseService(con);
            FeedbackBase Base = BllBase.GetOrderInfoByWhere(" where OrderNo='" + OrderNo + "'").FirstOrDefault();
            if (Base != null)
            {
                Delete = true;
            }
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                if (Delete)
                {
                    if(!BllBase.Delete(OrderNo, tran))//删除旧单据
                    {
                        result.ErrMsg = "提交失败，原因：删除原始信息异常";
                        result.Result = false;
                        return result;
                    }
                }
                //定义保存的数据
                FeedbackBase Domain = new FeedbackBase();
                Domain.OrderNo = OrderNo;//单号，控制表以KZ开头，反馈单已FK开头
                Domain.BatchNo = PostData.BatchNo;
                Domain.Model = PostData.Model;
                Domain.Qty = PostData.Qty;
                Domain.Status = "T1";//默认T1顺序审批人
                Domain.WorkProcedure = PostData.WorkProcedure;
                Domain.EquipmentName = PostData.EquipmentName;
                Domain.EquipmentNo = PostData.EquipmentNo;
                Domain.FeedbackMan = PostData.FeedbackMan;
                Domain.ProblemLevel = PostData.ProblemLevel;//审批流程代号
                if(PostData.FeedbackTime==null|| PostData.FeedbackTime=="")
                {
                    PostData.FeedbackTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
                Domain.FeedbackTime = Convert.ToDateTime(PostData.FeedbackTime);
                Domain.ProductClass = PostData.ProductClass;
                if(OrderNo.StartsWith("FK"))
                {
                    Domain.OrderType = 0;
                }
                else
                {
                    Domain.OrderType = 1;
                }
                Domain.Measure = PostData.Measure != null ? PostData.Measure : "";
                Domain.Report = PostData.Report != null ? PostData.Report : "";
                List <ProductCard.CardInfo> CardList = PostData.CardList;
                List<OrderReason> ReasonData = PostData.ReasonData;
                List<OrderProblem> ProblemData= PostData.ProblemData;
                if (BllBase.Insert(Domain, tran))
                {
                    FeedbackExProblemService BllProblem = new FeedbackExProblemService(con);
                    FeedbackExReasonService BllReason = new FeedbackExReasonService(con);
                    BllProductCard BllCard = new BllProductCard(con);
                    for (int i=0;i<ProblemData.Count;i++)
                    {
                        FeedbackExProblem Problem = new FeedbackExProblem();
                        Problem.OrderNo = OrderNo;
                        Problem.CodeString = ProblemData[i].codeString;
                        Problem.TopClass = ProblemData[i].topClass;
                        Problem.RoomName = ProblemData[i].roomName;
                        Problem.Problem = ProblemData[i].problem;
                        Problem.TypeName = ProblemData[i].typeName;
                        Problem.Present = ProblemData[i].present;
                        Problem.ProblemDetails = ProblemData[i].problemDetails;
                        Problem.Suggestion = ProblemData[i].suggestion;
                        Problem.QualityClass = ProblemData[i].qualityClass;
                        Problem.ProblemLevel = ProblemData[i].problemLevel;
                        if(!BllProblem.AddProblem(Problem,tran))
                        {
                            Exception ex = new Exception("提交失败，原因：质量问题提交异常");
                            throw ex;
                        }
                    }
                    for(int i=0;i<ReasonData.Count;i++)
                    {
                        FeedbackExReason Reason = new FeedbackExReason();
                        Reason.OrderNo = OrderNo;
                        Reason.ReasonDetails = ReasonData[i].reasonDetails;
                        Reason.ReasonType = ReasonData[i].reasonType;
                        if(!BllReason.AddReason(Reason,tran))
                        {
                            Exception ex = new Exception("提交失败，原因：原因分析提交异常");
                            throw ex;
                        }
                    }
                   foreach(var node in CardList)
                    {
                        node.FKOrderNo = OrderNo;
                        if (!BllCard.InsertCard(node, tran))
                        {
                            Exception ex = new Exception("提交失败，原因：客户信息提交异常");
                            throw ex;
                        }
                    }
                    tran.Commit();
                    result.Result = true;
                    result.ErrMsg = "提交成功";
                }
                else
                {
                    result.Result = false;
                    result.ErrMsg = "提交失败，原因：基础信息提交异常";
                }
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                result.Result = false;
                result.ErrMsg = ex.Message;
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public class OrderInfo
        {
            public string OrderNo { get; set; }
            public string WorkProcedure { get; set; }
            public string BatchNo { get; set; }
            public string Model { get; set; }
            public string Qty { get; set; }
            public string EquipmentName { get; set; }
            public string EquipmentNo { get; set; }
            public string FeedbackMan { get; set; }
            public List<OrderReason> ReasonData { get; set; }
            public List<OrderProblem> ProblemData { get; set; }
            public string FeedbackTime { get; set; }
            public string ProblemLevel { get; set; }
            public List<ProductCard.CardInfo> CardList { get; set; }
            public string ProductClass { get; set; }
            public string Measure { get; set; }
            public string Report { get; set; }
            public List<ApprovalStream> ApprovalStream { get; set; }
            public long RowCount { get; set; }
            public List<ApprovalStream> ControlStream { get; set; }
            public string Status { get; set; }
        }

        public class OrderProblem
        {
            public string codeString { get; set; }
            public string orderNo { get; set; }
            public string present { get; set; }
            public string problem { get; set; }
            public string problemDetails { get; set; }
            public string problemLevel { get; set; }
            public string qualityClass { get; set; }
            public string roomName { get; set; }
            public string topClass { get; set; }
            public string typeName { get; set; }
            public string suggestion { get; set; }

        }

        public class OrderReason
        {
            public string orderNo { get; set; }
            public string reasonDetails { get; set; }
            public string reasonType { get; set; }
        }
    }
}