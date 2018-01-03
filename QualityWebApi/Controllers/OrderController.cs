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
using QualityWebApi.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        private string conString = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        private string OAconString = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("host.json", optional: true).Build().GetSection("OASource").Value;

        private string GHConString = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("host.json", optional: true).Build().GetSection("ghSource").Value;
        private string WaitConfirmStepID = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Confirm_StepID").Value;
        private string WaitHandleStepID = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("host.json", optional: true).Build().GetSection("WorkFlowTask_Wait_StepID").Value;

          
        [HttpGet]
        public Object Get(string ActionType, long PageIndex, long PageSize, string OrderNo,
            string WorkProcedure, string BatchNo, string Model, string EquipmentNo,
            string FeedbackMan, string StartTime, string EndTime, string Status, int OrderType,string EmployeeID)
        {
            if (ActionType == "GetOrderInfo")
            {
                return GetOrderInfo(PageIndex, PageSize, OrderNo,WorkProcedure, BatchNo, Model, EquipmentNo,
                                    FeedbackMan, StartTime, EndTime, Status, OrderType);
            }
            else if (ActionType=="GetAmount")
            {
                return GetAmount(WorkProcedure, BatchNo);
            }
            else if (ActionType== "GetWaitConfirm")
            {
                return GetWaitConfirm(EmployeeID, WorkProcedure);
            }
            else
            {
                return null;
            }
           
        }



        /// <summary>
        /// ��ȡ��������ȷ�϶���
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <param name="WorkProcedure"></param>
        /// <returns></returns>
        public Object GetWaitConfirm(string EmployeeID,string WorkProcedure)
        {

            //EmployeeID = "00095";
            try
            {

                string strWhere = @" where b.Status=0 and b.StepID='" + WaitConfirmStepID + "'and EmployeeID = '" + EmployeeID
                    + "' order by FeedbackTime desc";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    FeedbackBaseService Bll = new FeedbackBaseService(con);
                    return Bll.GetWaitConfirm(strWhere);
                }
            }catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// ��ȡĳ������ĳ���������
        /// </summary>
        /// <param name="WorkProcedure">����</param>
        /// <param name="BatchNo">����</param>
        /// <returns></returns>
        public Object GetAmount(string WorkProcedure,string BatchNo)
        {
            decimal Amount = 0;
            WorkProcedure = WorkProcedure != null ? WorkProcedure : "";
            WorkProcedure = WorkProcedure.Replace(" ", "").Trim().Replace("����", "");
            BatchNo= BatchNo.Replace(" ", "").Trim();
            using (SqlConnection con = new SqlConnection(GHConString))
            {
                FeedbackBaseService BllBase = new FeedbackBaseService(con);
                RateTotal.tm_dTempStoreIO obj = new RateTotal.tm_dTempStoreIO();
                if (WorkProcedure.Contains("����") || WorkProcedure.Contains("˿ӡ") || WorkProcedure.Contains("�¹���"))
                {
                    //chrBatchID,chrType,intChipAmount
                    List<tp_carCraft> carCraftList = new List<tp_carCraft>();
                    carCraftList = BllBase.GetInfo(BatchNo);

                    obj.decSumQty = carCraftList != null ? Convert.ToDecimal(carCraftList.FirstOrDefault().intChipAmount) : 0;
                }
                else
                {
                    obj = BllBase.GetTempStoreAmount(BatchNo, WorkProcedure);
                }
                return obj;
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
            string strWhere = @"   OrderNo like '%{0}%' and isnull(WorkProcedure,'') like
 '%{1}%' and isnull(BatchNo,'') like '%{2}%' and isnull(Model,'') like '%{3}%' and  isnull(EquipmentNo,'') like '%{4}%' and isnull(FeedbackMan,'') like '%{5}%'  ";
            strWhere = string.Format(strWhere, OrderNo, WorkProcedure, BatchNo, Model, EquipmentNo, FeedbackMan);
            if (StartTime != "" & EndTime != "")
            {
                strWhere += " and FeedbackTime between '" + StartTime + "' and '" + EndTime + " 23:59:59'";
            }
            /*  
             *  Status��ʾ����״̬����ֵ�������¼�������� Null:ȫ����E��������W:��ȷ��;T:������;P:�����;B:���˵�;
             *  W״̬�£�b.Status=0&&b.StepID=˽�й�������WaitConfirmStepID
             *  E״̬��: b.Status=0&&b.StepID=˽�й�������WaitHandleStepID����QC���ڲ���StepID
             *  T״̬��: b.Status=0
             *  P״̬�£�b.Status=2
             *  B״̬�£�b.Status=3
            */
            switch(Status)
            {
                case "E":
                    strWhere += @" and (b.Status=0 and b.StepID='" + WaitHandleStepID + "')";
                    break;
                case "W":
                    strWhere += @" and (b.Status=0 and b.StepID='" + WaitConfirmStepID + "')";
                    break;
                case "T":
                    strWhere += @" and (b.Status=0)";
                    break;
                case "P":
                    strWhere += @" and (b.Status=2)";
                    break;
                case "B":
                    strWhere += @" and (b.Status=3)";
                    break;
                default:
                    break;
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
                    List<FeedbackBase> BaseList = BllBase.GetOrderByWorkFlowTaskOnPage(PageIndex, PageSize, strWhere, out RowCount);
                    List<OrderInfo> OrderList = new List<OrderInfo>();
                    if (BaseList != null && BaseList.Count > 0)
                    {
                        foreach (var node in BaseList)
                        {
                            OrderInfo Item = new OrderInfo();
                            //�ٻ�����Ϣ
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
                            //Item.Status= node.Status;
                            Item.Status = node.ProvalStatus.ToString();
                            Item.EmployeeID = node.EmployeeID;
                            Item.StepID = node.StepID;
                            Item.StepName = node.StepName;
                            Item.ID = (node.ID).ToString();
                            Item.PrevStatus = node.PrevStatus;
                            Item.IsControl = node.IsControl;
                            Item.ImageList = node.ImageList;



                            //����������
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
                            //��ԭ�����
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
                            //�ܿͻ���Ϣ
                            Item.CardList= BllCard.GetCard(" where FKOrderNo='" + node.OrderNo + "' and CardNo <>''");
                            //�ݷ������������
                            Item.ApprovalStream = BllStream.GetStream(" where OrderNo='" + node.OrderNo + "' and StreamType=0");
                            if(OrderType==1)
                            {
                                //�޿��Ʊ������
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
            ResultData result = new ResultData();//������������
            SqlConnection con = new SqlConnection(conString);
            if(con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            //��֤�Ƿ��Ѵ��ڸõ��ŵĵ��ݣ�����ɾ��������ӣ�������ֱ�����
            bool Delete = false;//false�������ڣ�true������
            string OrderNo = PostData.OrderNo!=null? PostData.OrderNo:"";
            OrderNo = OrderNo.Trim();
            FeedbackBaseService BllBase = new FeedbackBaseService(con);
            Guid NewID = BllBase.GetNewID();

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
                    if (!BllBase.Delete(OrderNo, tran))//ɾ���ɵ���
                    {
                        result.ErrMsg = "�ύʧ�ܣ�ԭ��ɾ��ԭʼ��Ϣ�쳣";
                        result.Result = false;
                        return result;
                    }
                }
                //���屣�������

                string EmployeeID = PostData.EmployeeID;
                FeedbackBase Domain = new FeedbackBase();
                Domain.ID = NewID;
                Domain.OrderNo = OrderNo;//���ţ����Ʊ���KZ��ͷ����������FK��ͷ
                Domain.BatchNo = PostData.BatchNo;
                Domain.Model = PostData.Model;
                Domain.Qty = PostData.Qty;
                Domain.Status = "0";//Ĭ��T1˳��������
                Domain.WorkProcedure = PostData.WorkProcedure;
                Domain.EquipmentName = PostData.EquipmentName;
                Domain.EquipmentNo = PostData.EquipmentNo;
                Domain.FeedbackMan = PostData.FeedbackMan;
                Domain.ProblemLevel = PostData.ProblemLevel;//�������̴���
                Domain.ImageList = PostData.ImageList;
                Domain.ImageHtml = PostData.ImageHtml;

                if (PostData.FeedbackTime==null|| PostData.FeedbackTime=="")
                {
                    PostData.FeedbackTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
                Domain.FeedbackTime = Convert.ToDateTime(PostData.FeedbackTime);
                Domain.ProductClass = PostData.ProductClass;
                int OrderType = 0;
                int FlowTempId = 1;
                if (OrderNo.StartsWith("FK"))
                {
                    FlowTempId = 0;
                    OrderType = 0;
                }
                else
                {
                    FlowTempId = 1;
                    OrderType = 1;
                }
                Domain.OrderType = OrderType;
                Domain.Measure = PostData.Measure != null ? PostData.Measure : "";
                Domain.Report = PostData.Report != null ? PostData.Report : "";
                Domain.EmployeeID = EmployeeID;
                List <ProductCard.CardInfo> CardList = PostData.CardList;
                List<OrderReason> ReasonData = PostData.ReasonData;
                List<OrderProblem> ProblemData= PostData.ProblemData;

                #region======Post����¼������ϵͳ=====
                //GeneralMethod Method = new GeneralMethod();
                string title = "";
                if (FlowTempId == 1)
                {
                    title = "������������(" + OrderNo + ")";
                }
                else
                {
                    title = "������������(" + OrderNo + ")";
                }

                string urls = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("host.json", optional: true).Build().GetSection("ApprovalPostAddress").Value;
                using (HttpClient Client = new HttpClient())
                {
                    var values = new List<KeyValuePair<string, string>>();
                    values.Add(new KeyValuePair<string, string>("Title", title));
                    values.Add(new KeyValuePair<string, string>("InstanceID", NewID.ToString()));
                    values.Add(new KeyValuePair<string, string>("FlowID", "D3BBCCBF-8968-4833-8A0F-CE5B19B718E2"));
                    values.Add(new KeyValuePair<string, string>("Account", EmployeeID));
                    values.Add(new KeyValuePair<string, string>("WorkProcedure", PostData.WorkProcedure));
                    var content = new FormUrlEncodedContent(values);
                    var response = Client.PostAsync(urls, content);
                    response.Wait();

                    HttpResponseMessage resp = response.Result;

                    var res2 = resp.Content.ReadAsStringAsync();
                    res2.Wait();
                    string Message = res2.Result;
                    JObject Robj = JObject.Parse(Message);
                    if (Robj["result"].ToString() == "1")
                    {
                        Domain.TechnologistMembers = Robj["msg"].ToString();
                    }
                    else
                    {
                        Exception ex = new Exception(Robj["msg"] + "(Զ�˽���ʧ��)");
                        throw ex;
                    }
                }
                #endregion



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
                        if(ProblemData[i].suggestion!=""&& ProblemData[i].suggestion!=null&& ProblemData[i].suggestion.Contains("suggestion"))
                        {
                            JArray JList = JArray.Parse(ProblemData[i].suggestion);
                            foreach (var node in JList)
                            {
                                string suggestion = node["suggestion"].ToString();
                                if(!BllBase.InsertOrderSuggestion(tran, suggestion, OrderNo, ProblemData[i].codeString))
                                {
                                    Exception ex = new Exception("�ύʧ�ܣ�ԭ�򣺱�������������봦�����ʧ��");
                                    throw ex;
                                }
                            }
                        }

                        if (!BllProblem.AddProblem(Problem,tran))
                        {
                            Exception ex = new Exception("�ύʧ�ܣ�ԭ�����������ύ�쳣");
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
                            Exception ex = new Exception("�ύʧ�ܣ�ԭ��ԭ������ύ�쳣");
                            throw ex;
                        }
                    }
                   foreach(var node in CardList)
                    {
                        node.FKOrderNo = OrderNo;
                        if (!BllCard.InsertCard(node, tran))
                        {
                            Exception ex = new Exception("�ύʧ�ܣ�ԭ�򣺿ͻ���Ϣ�ύ�쳣");
                            throw ex;
                        }
                    }

                    tran.Commit();
                    result.Result = true;
                    result.ErrMsg = "�ύ�ɹ�";
                }
                else
                {
                    result.Result = false;
                    result.ErrMsg = "�ύʧ�ܣ�ԭ�򣺻�����Ϣ�ύ�쳣";
                    BllBase.DeleteWorkFlowTask(NewID.ToString(), tran);
                    tran.Commit();
                }
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                tran.Dispose();
                try
                {
                    BllBase.DeleteWorkFlowTask(NewID.ToString());
                }
                catch (Exception ex2)
                {

                }
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
            public string EmployeeID { get; set; }
            public string StepID { get; set; }
            public string StepName { get; set; }
            public string ID { get; set; }
            public int PrevStatus { get; set; }
            public string IsControl { get; set; }
            public string ImageList { get; set; }

            public string ImageHtml { get; set; }
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