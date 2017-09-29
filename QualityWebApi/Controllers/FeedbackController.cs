using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Domain;
using AppService;
using QualityWebApi.Common;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Feedback")]
    public class FeedbackController : Controller
    {
 
        private string conString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("host.json", optional: true).Build().GetSection("GHLPYSource").Value;

        private string SavePath = Directory.GetCurrentDirectory()+"/ProblemPicture";
        [HttpPut]
        public Object GetOrderNo()
        {
            ResultData result = new ResultData();
            try
            {
                using (SqlConnection sqlconnection = new SqlConnection(conString))
                {
                    FeedbackBaseService service = new FeedbackBaseService(sqlconnection);
                    result.Result = true;
                    result.ErrMsg= service.GetOrderNo();
                    return result;
                }
            }
            catch(Exception ex)
            {
                result.Result = false;
                result.ErrMsg = ex.Message;
                return result;
            }
        }

        [HttpGet]
       public Object GetBatchInfo(string chrBatchID)
        {
            ResultData result = new ResultData();
            using (SqlConnection sqlconnection = new SqlConnection(conString))
            {
                tp_carCraft BatchInfo = new tp_carCraft();
                FeedbackBaseService service = new FeedbackBaseService(sqlconnection);
                return service.GetInfo(chrBatchID);
            }
        }

        [HttpPost]
        public Object AddFeedback([FromBody]FeedbackData PostData)
        {
            ResultData result = new ResultData();
            using (SqlConnection sqlconnection = new SqlConnection(conString))
            {
                bool flag = true;
                FeedbackBaseService service = new FeedbackBaseService(sqlconnection);
                List<FeedbackBase> BaseList = service.GetOrderInfoByWhere(" where OrderNo='" + PostData.OrderNo + "'");
                if (sqlconnection.State == ConnectionState.Closed)
                {
                    sqlconnection.Open();
                }
                SqlTransaction transaction = sqlconnection.BeginTransaction();
                try
                {
                    FeedbackBase feedbackbase = new FeedbackBase();
                    feedbackbase.OrderNo = PostData.OrderNo;
                    feedbackbase.Model = PostData.Model;
                    feedbackbase.FeedbackMan = PostData.FeedbackMan;
                    //feedbackbase.FeedbackTime = DateTime.Now;
                    feedbackbase.Qty = PostData.Qty;
                    feedbackbase.WorkProcedure = PostData.WorkProcedure;
                    feedbackbase.BatchNo = PostData.BatchNo;
                    feedbackbase.EquipmentName = PostData.EquipmentName;
                    feedbackbase.EquipmentNo = PostData.EquipmentNo;
                    feedbackbase.ProductClass = PostData.ProductClass;
                    if (PostData.FeedbackTime=="")
                    {
                        feedbackbase.FeedbackTime = DateTime.Now;
                    }
                    else
                    {
                        feedbackbase.FeedbackTime = Convert.ToDateTime(PostData.FeedbackTime);
                    }
                    
                    feedbackbase.Status = PostData.Status;
                    feedbackbase.ProblemLevel = PostData.ProblemLevel;
                    //FeedbackBaseService service = new FeedbackBaseService(sqlconnection);
                    //List<FeedbackBase> BaseList= service.GetOrderInfoByWhere(" where OrderNo='" + PostData.OrderNo + "'",transaction);
                    if (BaseList!=null&&BaseList.Count>0)
                    {
                        flag=service.Delete(PostData.OrderNo, transaction);
                    }
                    if(!Directory.Exists(SavePath))
                    {
                        Directory.CreateDirectory(SavePath);
                    }
                    if (service.AddFeedbackBase(feedbackbase, transaction))
                    {
                        FeedbackExProblemService PService = new FeedbackExProblemService(sqlconnection);
                        FeedbackExReasonService RService = new FeedbackExReasonService(sqlconnection);
                        BllProductCard BllCard = new BllProductCard(sqlconnection);
                        FeedbackExProblem Problem = new FeedbackExProblem();
                        FeedbackExReason Reason = new FeedbackExReason();
                        foreach (var list in PostData.ReasonData)
                        {

                            if (list.ReasonType != "" && list.ReasonDetails != "")
                            {
                                Reason.ReasonType = list.ReasonType;
                                Reason.ReasonDetails = list.ReasonDetails;
                                Reason.OrderNo = PostData.OrderNo;
                                if (!RService.AddReason(Reason, transaction))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        foreach (var list in PostData.ProblemData)
                        {
                            if(list.CodeString!=""|| list.PicturePath!=""|| list.ProblemDetails!="")
                            {
                                Problem.CodeString = list.CodeString;
                                Problem.OrderNo = PostData.OrderNo;
                                string PicturePath = list.PicturePath;
                                string filePath = "";
                                if(PicturePath!=""&& PicturePath!=null)
                                {
                                    var chrPath = PicturePath.Split('|');
                                    for(int iPath=0; iPath < chrPath.Length; iPath++)
                                    {
                                        if(chrPath[iPath].Contains("base64"))
                                        {
                                            SaveImage save = new SaveImage();
                                            string fileName = DateTime.Now.Ticks.ToString();
                                            save.SaveInageForBase(chrPath[iPath].Split(',')[1], SavePath, fileName);
                                            filePath = filePath+"ProblemPicture/" + fileName + ".png" + ",";
                                        }
                                        else if(chrPath[iPath]!="")
                                        {
                                            string tempPath = "";
                                            for(var i=0;i< chrPath[iPath].Split('/').Length;i++)
                                            {
                                                if(i>=3)
                                                {
                                                    tempPath = tempPath + chrPath[iPath].Split('/')[i] + "/";
                                                }
                                            }
                                            if(tempPath!="")
                                            {
                                                tempPath = tempPath.Substring(0, tempPath.LastIndexOf("/"));
                                                filePath = filePath + tempPath + ",";
                                            }
                                            
                                        }
                                    }
                                }
                                if(filePath!="")
                                {
                                    filePath = filePath.Substring(0, filePath.LastIndexOf(","));
                                }
 
                                Problem.PicturePath = filePath;
                                Problem.ProblemDetails = list.ProblemDetails;
                                if (!PService.AddProblem(Problem, transaction))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        foreach (var list in PostData.CardList)
                        {
                            list.FKOrderNo = PostData.OrderNo;
                            if (!BllCard.InsertCard(list,transaction))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if(PostData.IfQC)
                        {
                            FeedbackExHandleService BllHandler = new FeedbackExHandleService(sqlconnection);
                            FeedbackExHandle Handler = new FeedbackExHandle();
                            Handler.HandleMan = PostData.HandleMan;
                            Handler.HandleNote = PostData.HandleNote;
                            Handler.HandleSuggestion = PostData.HandleSuggestion;
                            Handler.HandleTime = DateTime.Now;
                            Handler.QualityClass = PostData.QualityClass;
                            Handler.OrderNo = PostData.OrderNo;
                            if(!BllHandler.AddHandle(Handler,transaction))
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            transaction.Commit();
                            result.ErrMsg = "";
                            result.Result = true;
                            return result;
                        }
                        else
                        {
                            transaction.Rollback();
                            result.ErrMsg = "反馈失败";
                            result.Result = false;
                            return result;
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        result.ErrMsg = "反馈失败";
                        result.Result = false;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Result = false;
                    if (ex.Message.Contains("重复键"))
                    {
                        result.ErrMsg = "已存在相同的单号，编码不能重复添加";
                    }
                    else
                    {
                        result.ErrMsg = ex.Message;
                    }
                    return result;
                }
            }
        }
    }

    public class ResultData
    {
        public bool Result { get; set; }
        public string ErrMsg { get; set; }
    }

    public class FeedbackData
    {
        public bool IfQC { get; set; }
        public string OrderNo { get; set; }
        public string WorkProcedure { get; set; }
        public string BatchNo { get; set; }
        public string Model { get; set; }
        public string Qty { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public string FeedbackMan { get; set; }
        //public string FeedbackTime { get; set; }
        public List<ReasonData> ReasonData { get; set; }
        public List<ProblemData> ProblemData { get; set; }
        public string FeedbackTime { get; set; }

        public string HandleMan { get; set; }
        public string HandleSuggestion { get; set; }
        public string QualityClass { get; set; }
        public string HandleNote { get; set; }
        public string Status { get; set; }
        public string ProblemLevel { get; set; }
        public List<ProductCard.CardInfo> CardList { get; set; }
        public string ProductClass { get; set; }
    }

    

    public class ReasonData
    {
        public string ReasonType { get; set; }
        public string ReasonDetails { get; set; }
    }

    public class ProblemData
    {
        public string CodeString { get; set; }
        public string ProblemDetails { get; set; }
        public string PicturePath { get; set; }
    }
}