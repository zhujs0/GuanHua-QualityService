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
using System.Threading;
using QualityWebApi.Common;
using OfficeOpenXml;
using System.Data;
using QualityWebApi.Filter;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace QualityWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/RateTotal")]
    public class RateTotalController : Controller
    {
        private string _ConString = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("host.json", optional: true).Build().GetSection("ghSource").Value;
        [HttpGet]
        public Object Get(string ActionType,string ProductSize,string Porcelain,string chrType,
            string Time,string Material,string SizeMark)
        {
            string Model = chrType;
            using (SqlConnection con = new SqlConnection(_ConString))
            {
                try
                {
                    //SqlCommand cmd = new SqlCommand();
                    //cmd.CommandTimeout = 180;
                    ReturnData Item = new ReturnData();
                    
                    if (ActionType == "GetPorcelain")
                    {
                        BllRateTotal Bll = new BllRateTotal(con);
                        return Bll.GetTm_item();
                    }
                    else if (ActionType == "GetPut")
                    {
                        DateTime Date = Convert.ToDateTime(Time);
                        DateTime Now = DateTime.Now;
                        int Month = Date.Month;
                        int NowMonth = DateTime.Now.Month;
                        string StartTime = "", EndTime = "";
                        bool flag = false;//计算当前的流通库存或者盘点库存
                        bool IsGet = true;//是否需要获取返回值
                        if (Date.Year == Now.Year)
                        {
                            
                            if (Month < NowMonth)
                            {
                                flag = true;
                                switch (Month)
                                {
                                    case 1:
                                        StartTime = Time + "/01 ";
                                        EndTime = Time + "/25 ";
                                        break;
                                    case 6:
                                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                        EndTime = Time + "/30 ";
                                        break;
                                    case 7:
                                        StartTime = Time + "/01 ";
                                        EndTime = Time + "/25 ";    
                                        break;
                                    default:
                                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                        EndTime = Time + "/25 ";
                                        break;
                                }
                            }
                            else if (Month == NowMonth)
                            {
                                switch (Month)
                                {
                                    case 1:
                                        if (Now.Day > 25)
                                        {
                                            StartTime = Time + "/01 ";
                                            EndTime = Time + "/25 ";
                                            flag = true;
                                        }
                                        else
                                        {
                                            StartTime = Time + "/01 ";
                                            EndTime = Now.ToString("yyyy/MM/dd ");
                                            flag = false;
                                        }
                                        break;
                                    case 6:
                                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                        EndTime = Now.ToString("yyyy/MM/dd ");
                                        flag = false;
                                        break;
                                    case 7:
                                        if (Now.Day > 25)
                                        {
                                            StartTime = Time + "/01 ";
                                            EndTime = Time + "/25 ";
                                            flag = true;
                                        }
                                        else
                                        {
                                            StartTime = Time + "/01 ";
                                            EndTime = Now.ToString("yyyy/MM/dd ");
                                            flag = false;
                                        }
                                        break;
                                    case 12:
                                        if (Now.Day > 30)
                                        {
                                            StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                            EndTime = Time + "/30 ";
                                            flag = true;
                                        }
                                        else
                                        {
                                            StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                            EndTime = Now.ToString("yyyy/MM/dd");
                                            flag = false;
                                        }
                                        break;
                                    default:
                                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                        if (Now.Day > 25)
                                        {
                                            EndTime = Time + "/25 ";
                                            flag = true;
                                        }
                                        else
                                        {
                                            EndTime = DateTime.Now.ToString("yyyy/MM/dd");
                                            flag = false;
                                        }
                                        break;
                                }
                            }
                            else if(Month==NowMonth+1)
                            {
                                if(Now.Day>=26&&Month!=7)
                                {
                                    StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                    EndTime = Now.ToString("yyyy/MM/dd ");
                                    flag = false;
                                }
                                else
                                {
                                    IsGet = false;
                                }
                            }
                            else
                            {
                                IsGet = false;
                            }
                            
                        }
                        else if (Date.Year < Now.Year)
                        {
                            flag = true;
                            switch (Month)
                            {
                                case 1:
                                    StartTime = Time + "/01 ";
                                    EndTime = Time + "/25 ";
                                    break;
                                case 6:
                                    StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                    EndTime = Time + "/30 ";
                                    break;
                                case 7:
                                    StartTime = Time + "/01 ";
                                    EndTime = Time + "/25 ";
                                    break;
                                case 12:
                                    StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                    EndTime = Time + "/30 ";
                                    break;
                                default:
                                    StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                    EndTime = Time + "/25 ";
                                    break;
                            }
                        }
                        else if(Date.Year>Now.Year)
                        {
                            IsGet = false;
                        }
                        if(IsGet)
                        {
                            switch (SizeMark)
                            {
                                case "0":
                                    SizeMark = ">";
                                    break;
                                case "1":
                                    SizeMark = "=";
                                    break;
                                case "2":
                                    SizeMark = "<";
                                    break;
                                case "3":
                                    SizeMark = ">=";
                                    break;
                                case "4":
                                    SizeMark = "<=";
                                    break;
                                default:
                                    break;
                            }
                            RateTotal.MonthCheckProd LastMonth = GetMonthCheckProd(ProductSize, Material, Porcelain, Model,
                               (Date.AddMonths(-1)).ToString("yyyy/MM"), con, null, SizeMark);
                            decimal LastMonthCheck = LastMonth != null ?
                                LastMonth.LiuTongAmount + LastMonth.MonthCheckAmount + LastMonth.ZhengpinAmount : 0;
                            //decimal LastMonthCheck = 0;
                            return  GetData(Convert.ToDateTime(StartTime), Convert.ToDateTime(EndTime),
                                Model, ProductSize, Material, Porcelain, SizeMark, LastMonthCheck, flag, con);
                           
                            #region  =======旧逻辑，只查询一个点，即当天=======
                            /*//①入库数正品
                            RateTotal.PutCount StockList = GetPut(ProductSize, Material, Porcelain,
                                Model, StartTime+" 00:00:00", EndTime+" 23:59:59", true, con,SizeMark);
                            decimal StockAmount = StockList != null ? StockList.decQuantity : 0;
                            //②入库数非命中数
                            RateTotal.PutCount StockListHit = GetPut(ProductSize, Material, Porcelain,
                                Model, StartTime + " 00:00:00", EndTime + " 23:59:59", false, con, SizeMark);
                            decimal HitAmount = StockListHit != null ? StockListHit.decQuantity : 0;

                            //③投料数
                            RateTotal.FeedCount FeedList = GetFeedCount(ProductSize, Material, Porcelain, Model,
                                StartTime+ " 08:00:00", 
                                Convert.ToDateTime(EndTime).AddDays(+1).ToString("yyyy/MM/dd")+ " 07:59:59", con, null,
                                SizeMark);
                            decimal FeedCount = FeedList != null ? FeedList.decquantity : 0;

                            //④上月盘点数含合批仓
                            RateTotal.MonthCheckProd MonthList = GetMonthCheckProd(ProductSize, Material, Porcelain, Model,
                                (Date.AddMonths(-1)).ToString("yyyy/MM"), con, null, SizeMark);
                            decimal MonthCheckProdCount = MonthList != null ?
                                MonthList.LiuTongAmount + MonthList.MonthCheckAmount + MonthList.ZhengpinAmount : 0;


                            #region=====如果选择时间为2017-09；上月盘点数确定=======
                            if (Date.Year==2017&&Date.Month==9)
                            {
                                if(Material=="Y")
                                {
                                    MonthCheckProdCount = 1558380000;
                                }
                                else if(Material == "B")
                                {
                                    MonthCheckProdCount = 6368310000;
                                }
                                else if(Material=="N")
                                {
                                    MonthCheckProdCount = 4847410000;
                                }
                            }
                            else if(Date.Year == 2017 && Date.Month == 8)
                            {
                                if (Material == "Y")
                                {
                                    MonthCheckProdCount = 2241360000;
                                }
                                else if (Material == "B")
                                {
                                    MonthCheckProdCount = 5617150000;
                                }
                                else if (Material == "N")
                                {
                                    MonthCheckProdCount = 4227370000;
                                }
                            }
                            #endregion

                            //⑤本月半成品数
                            decimal LTKunCun = 0;
                            if (flag)
                            {
                                //5.1  已过盘点日期：本月盘点数
                                RateTotal.MonthCheckProd temp = GetMonthCheckProd(ProductSize, Material, Porcelain, Model,
                                Time, con, null, SizeMark);
                                LTKunCun = temp != null ? temp.ZhengpinAmount + temp.MonthCheckAmount + temp.LiuTongAmount : 0;

                                #region=====如果选择时间为2017-08；本月半成品数确定=======
                                if (Date.Year == 2017 && Date.Month == 8)
                                {
                                    if (Material == "Y")
                                    {
                                        LTKunCun = 1558380000;
                                    }
                                    else if (Material == "B")
                                    {
                                        LTKunCun = 6368310000;
                                    }
                                    else if (Material == "N")
                                    {
                                        LTKunCun = 4847410000;
                                    }
                                }
                                #endregion

                            }
                            else
                            {
                                //5.2 未过盘点日期：本月流通库存
                                List<RateTotal.dTempStoreBalance> dList = GetdTempStoreBalance(ProductSize, Material,
                                    Porcelain, Model, con, SizeMark);
                                LTKunCun = dList.Count > 0 ? dList[0].DecQtyNal : 0;
                            }

                            //⑥次品转正品
                            decimal CiPinZhuanAmount = 0;
                            RateTotal.ZhuanZhengpin CiZhuan = GetZhuanZhengPin(ProductSize, Material, Porcelain, Model,
                                "12", con, StartTime + " 00:00:00", EndTime + " 23:59:59", SizeMark);
                            CiPinZhuanAmount = CiZhuan != null ? CiZhuan.decChangeInQnty : 0;

                            //⑦非命中转正品
                            RateTotal.ZhuanZhengpin HitZhuan= GetZhuanZhengPin(ProductSize, Material, Porcelain, Model,
                                "13", con, StartTime + " 00:00:00", EndTime + " 23:59:59", SizeMark);
                            decimal HitZhuanAmount = HitZhuan != null ? HitZhuan.decChangeInQnty : 0;
                            Item.Material = Material;
                            Item.dTempStoreBalance = LTKunCun;
                            Item.FeedCount = FeedCount;
                            Item.MonthCheckProd = MonthCheckProdCount;
                            Item.PutCount = HitAmount+ StockAmount;
                            Item.HitAmount = HitAmount;
                            Item.StockAmount = StockAmount;
                            Item.CiPinZhuanAmount = CiPinZhuanAmount;
                            Item.HitZhuanAmount = HitZhuanAmount;
                            return Item;*/
                            #endregion
                        }
                        else
                        {
                            return null;
                        }
                        
                    }
                    else if (ActionType=="ToExcelBySize")
                    {

                        List<ExcelEntity> list = ToExcelBySize(Time, con);
                        EntityToExcelByNpoi npoi = new EntityToExcelByNpoi();
                        MemoryStream stream= npoi.EntityToStream<ExcelEntity>(list);
                        GlobalSize.result = true;
                        GlobalSize.SizeProcess = 100;
                        //返回文件流
                        return stream;
                        #region
                        ////返回Http
                        //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                        //result.Content = new StreamContent(stream);
                        //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        //result.Content.Headers.ContentDisposition.FileName = "file.xls";
                        //return result;
                        ////返回文件地址
                        //string urls = new ConfigurationBuilder()
                        //.SetBasePath(Directory.GetCurrentDirectory())
                        //.AddJsonFile("host.json", optional: true).Build().GetSection("urls").Value;
                        //string FileName = Convert.ToDateTime(Time).ToString("yyyy-MM") + ".xlsx";
                        //string path = Directory.GetCurrentDirectory() + "/ProblemPicture/File/" + FileName;
                        //using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                        //{
                        //    byte[] data = stream.ToArray();
                        //    fs.Write(data, 0, data.Length);
                        //    fs.Flush();
                        //}
                        //return urls + "/ProblemPicture/File/" + FileName;
                        #endregion
                    }
                    else if (ActionType=="GetSizeProcess")
                    {
                        ResultData result = new ResultData();
                        result.Result = GlobalSize.result;
                        if(GlobalSize.result)
                        {
                            result.ErrMsg = GlobalSize.SizeProcess.ToString();
                        }
                        else
                        {
                            result.ErrMsg = GlobalSize.Error;
                        }
                        //string strJson = "{\"result\":" + GlobalSize.result + ",\"sizeProcess\":" + GlobalSize.SizeProcess + ",\"error\":\""
                        //    + GlobalSize.Error + "\"}";
                        return result;
                    }
                    else if (ActionType == "ExcelMProcess")
                    {
                        List<ExcelEntity> list = ToExcelByMaterial(Time, con);
                        EntityToExcelByNpoi npoi = new EntityToExcelByNpoi();
                        MemoryStream stream = npoi.EntityToStream<ExcelEntity>(list);
                        GlobalMaterial.result = true;
                        GlobalMaterial.MaterialProcess = 100;
                        return stream;
                    }
                    else if (ActionType == "GetMProcess")
                    {
                        ResultData result = new ResultData();
                        result.Result = GlobalMaterial.result;
                        if (GlobalMaterial.result)
                        {
                            result.ErrMsg = GlobalMaterial.MaterialProcess.ToString();
                        }
                        else
                        {
                            result.ErrMsg = GlobalMaterial.Error;
                        }
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    ResultData result = new ResultData();
                    GlobalSize.result = false;
                    GlobalSize.Error = ex.Message;
                    GlobalMaterial.result = false;
                    GlobalMaterial.Error= ex.Message;
                    result.Result = false;
                    result.ErrMsg = ex.Message;
                    return result;
                }
               
            }
        }

        public List<ExcelEntity> ToExcelBySize(string Time, SqlConnection con)
        {
            GlobalSize.SizeProcess = 0;
            GlobalSize.result = true;
            List<ExcelEntity> list = new List<ExcelEntity>();
            string ChrSzie = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("host.json", optional: true).Build().GetSection("RateToExcelBySize").Value;
            DateTime Date = Convert.ToDateTime(Time);
            string StartTime = "", EndTime = "";
            bool MonthCheck = SetTime(Time, out StartTime, out EndTime);
            string[] ChrSize = ChrSzie.Split(",");
            int Temp = (100 / ChrSize.Length);
            for (int i = 0; i < ChrSize.Length; i++)
            {
                ExcelEntity excelEntity = new ExcelEntity();
                string strSize = ChrSize[i].ToString();
                RateTotal.MonthCheckProd LastMonth = GetMonthCheckProd(strSize, null, null, null,
                  (Date.AddMonths(-1)).ToString("yyyy/MM"), con, null, "=");
                decimal LastMonthCheck = LastMonth != null ?
                    LastMonth.LiuTongAmount + LastMonth.MonthCheckAmount + LastMonth.ZhengpinAmount : 0;
                decimal Dec_Month = 0;
                //①开始时间至当前时间正品入库数
                RateTotal.PutCount Obj_Put = GetPut(strSize, null, null, null, StartTime + " 00:00:00",
                    EndTime + " 23:59:59", true, con, "=");
                decimal Dec_FIA = Obj_Put != null ? Obj_Put.decQuantity : 0;
                //②开始时间至当前时间非命中入库数
                Obj_Put = GetPut(strSize, null, null, null, StartTime + " 00:00:00", EndTime + " 23:59:59", false, con, "=");
                decimal Dec_FIE = Obj_Put != null ? Obj_Put.decQuantity : 0;
                //③开始时间至当前时间投料数
                RateTotal.FeedCount Obj_Feed = GetFeedCount(strSize, null, null, null, StartTime + " 08:00:00",
                    Convert.ToDateTime(EndTime).AddDays(+1).ToString("yyyy/MM/dd") + " 07:59:59", con, null, "=");
                decimal Dec_Feeding = Obj_Feed != null ? Obj_Feed.decquantity : 0;
                RateTotal.ZhuanZhengpin Obj_CiZhuanZheng = GetZhuanZhengPin(strSize, null, null, null, "12",
                    con, StartTime + " 00:00:00", EndTime + " 23:59:59", "=");
                RateTotal.ZhuanZhengpin Obj_FeiZhuanZheng = GetZhuanZhengPin(strSize, null, null, null, "13", con,
                    StartTime + " 00:00:00", EndTime + " 23:59:59", "=");
                decimal CiZhuanZheng = Obj_CiZhuanZheng != null ? Obj_CiZhuanZheng.decChangeInQnty : 0;
                decimal FeiZhuanZheng = Obj_FeiZhuanZheng != null ? Obj_FeiZhuanZheng.decChangeInQnty : 0;
                if (MonthCheck == true)
                {
                    //获取本月半成品盘点库存
                    RateTotal.MonthCheckProd Obj_MonthCheckProd = GetMonthCheckProd(strSize, null, null, null,
                       Time, con, null, "=");
                    Dec_Month = Obj_MonthCheckProd != null ? Obj_MonthCheckProd.MonthCheckAmount
                        + Obj_MonthCheckProd.LiuTongAmount + Obj_MonthCheckProd.ZhengpinAmount : 0;
                }
                else
                {
                    //获取流通库存
                    List<RateTotal.dTempStoreBalance> Obj_StoreBalance = GetdTempStoreBalance(strSize, null,
                        null, null, con, "=");
                    Dec_Month = (Obj_StoreBalance != null && Obj_StoreBalance.Count > 0) ? Obj_StoreBalance.FirstOrDefault().DecQtyNal : 0;
                }
                excelEntity.title = strSize;
                excelEntity.lastMonthCheck = LastMonthCheck;
                excelEntity.feedingAmount = Math.Round(Dec_Feeding,0);
                excelEntity.stockAmount = Dec_FIA;
                excelEntity.noHitAmount = Dec_FIE;
                excelEntity.monthCheck = Dec_Month;
                excelEntity.date = Time;
                list.Add(excelEntity);
                GlobalSize.SizeProcess+=Temp;
            }
            return list;
        }

        public List<ExcelEntity> ToExcelByMaterial(string Time,SqlConnection con)
        {
            GlobalMaterial.MaterialProcess = 0;
            GlobalMaterial.result = true;
            List<ExcelEntity> list = new List<ExcelEntity>();
            BllRateTotal Bll = new BllRateTotal(con);
            List<RateTotal.tm_item> item =Bll.GetTm_item();
            DateTime Date = Convert.ToDateTime(Time);
            string StartTime = "", EndTime = "";
            bool MonthCheck = SetTime(Time, out StartTime, out EndTime);
            int Temp = (100 / item.Count);
            for (int i = 0; i < item.Count; i++)
            {
                ExcelEntity excelEntity = new ExcelEntity();
                string strMaterial = (item[i].chrspec.ToString()).Trim();
                RateTotal.MonthCheckProd LastMonth = GetMonthCheckProd(null, null, strMaterial, null,
                  (Date.AddMonths(-1)).ToString("yyyy/MM"), con, null, "=");
                decimal LastMonthCheck = LastMonth != null ?
                    LastMonth.LiuTongAmount + LastMonth.MonthCheckAmount + LastMonth.ZhengpinAmount : 0;
                decimal Dec_Month = 0;
                //①开始时间至当前时间正品入库数
                RateTotal.PutCount Obj_Put = GetPut(null, null, strMaterial, null, StartTime + " 00:00:00",
                    EndTime + " 23:59:59", true, con, "=");
                decimal Dec_FIA = Obj_Put != null ? Obj_Put.decQuantity : 0;
                //②开始时间至当前时间非命中入库数
                Obj_Put = GetPut(null, null, strMaterial, null, StartTime + " 00:00:00", EndTime + " 23:59:59", false, con, "=");
                decimal Dec_FIE = Obj_Put != null ? Obj_Put.decQuantity : 0;
                //③开始时间至当前时间投料数
                RateTotal.FeedCount Obj_Feed = GetFeedCount(null, null, strMaterial, null, StartTime + " 08:00:00",
                    Convert.ToDateTime(EndTime).AddDays(+1).ToString("yyyy/MM/dd") + " 07:59:59", con, null, "=");
                decimal Dec_Feeding = Obj_Feed != null ? Obj_Feed.decquantity : 0;
                RateTotal.ZhuanZhengpin Obj_CiZhuanZheng = GetZhuanZhengPin(null, null, strMaterial, null, "12",
                    con, StartTime + " 00:00:00", EndTime + " 23:59:59", "=");
                RateTotal.ZhuanZhengpin Obj_FeiZhuanZheng = GetZhuanZhengPin(null, null, strMaterial, null, "13", con,
                    StartTime + " 00:00:00", EndTime + " 23:59:59", "=");
                decimal CiZhuanZheng = Obj_CiZhuanZheng != null ? Obj_CiZhuanZheng.decChangeInQnty : 0;
                decimal FeiZhuanZheng = Obj_FeiZhuanZheng != null ? Obj_FeiZhuanZheng.decChangeInQnty : 0;
                if (MonthCheck == true)
                {
                    //获取本月半成品盘点库存
                    RateTotal.MonthCheckProd Obj_MonthCheckProd = GetMonthCheckProd(null, null, strMaterial, null,
                       Time, con, null, "=");
                    Dec_Month = Obj_MonthCheckProd != null ? Obj_MonthCheckProd.MonthCheckAmount
                        + Obj_MonthCheckProd.LiuTongAmount + Obj_MonthCheckProd.ZhengpinAmount : 0;
                }
                else
                {
                    //获取流通库存
                    List<RateTotal.dTempStoreBalance> Obj_StoreBalance = GetdTempStoreBalance(null, null,
                        strMaterial, null, con, "=");
                    Dec_Month = (Obj_StoreBalance != null && Obj_StoreBalance.Count > 0) ? Obj_StoreBalance.FirstOrDefault().DecQtyNal : 0;
                }
                excelEntity.title = strMaterial;
                excelEntity.lastMonthCheck = LastMonthCheck;
                excelEntity.feedingAmount = Math.Round(Dec_Feeding, 0);
                excelEntity.stockAmount = Dec_FIA;
                excelEntity.noHitAmount = Dec_FIE;
                excelEntity.monthCheck = Dec_Month;
                excelEntity.date = Time;
                list.Add(excelEntity);
                GlobalMaterial.MaterialProcess += Temp;
            }
            return list;
        }

        public class ReturnData
        {
            public string Material { get; set; }
            public decimal MonthCheckProd { get; set; }
            public decimal FeedCount { get; set; }
            public decimal PutCount { get; set; }
            public decimal StockAmount { get; set; }
            public decimal HitAmount { get; set; }
            public decimal dTempStoreBalance { get; set; }
            public decimal HitZhuanAmount { get; set; }
            public decimal CiPinZhuanAmount { get; set; }

        }

        public RateTotal.PutCount GetPut(string ProductSize, string Material, string Porcelain, string Model,
            string StartTime, string EndTime,bool flag, SqlConnection con,string SizeMark)
        {
            string strWhere = "";
            if (ProductSize != null)
            {
                switch(SizeMark)
                {
                    case "0":
                        SizeMark = ">";
                        break;
                    case "1":
                        SizeMark = "=";
                        break;
                    case "2":
                        SizeMark = "<";
                        break;
                    case "3":
                        SizeMark = ">=";
                        break;
                    case "4":
                        SizeMark = "<=";
                        break;
                    default:
                        break;
                }
                strWhere = " and  substring(chrItemId,3,4)"+SizeMark+"'" + ProductSize + "'";
            }
            if (Material != null)
            {
                if (Material == "Y")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(substring(chrItemId,3,len(chrItemId)-2),'0')='Y5V' ";
                }
                else if (Material == "B")
                {
                    strWhere += " and( dbo.getTemperatureForlabelByType(substring(chrItemId,3,len(chrItemId)-2),'0')='X7R' or"
                        + " dbo.getTemperatureForlabelByType(substring(chrItemId,3,len(chrItemId)-2),'0')='X5R') ";
                }
                else if (Material == "N")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(substring(chrItemId,3,len(chrItemId)-2),'0')='COG' ";
                }
            }
            if (Porcelain != null)
            {
                strWhere += "and chrLot in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='" + Porcelain + "')";
            }
            if (Model != null)
            {
                strWhere += "and chrSpec like '%" + Model + "%'";
            }
            if(flag)
            {
                strWhere += "and (chrType='FIA')";
            }
            else
            {
                strWhere += "and (chrType='FIE')";
            }
            if (StartTime != null && EndTime != null)
            {
                strWhere += "And datIssRptDate Between'" + StartTime + " ' And '" + EndTime + " '";
            }
            BllRateTotal Bll = new BllRateTotal(con);
            return Bll.GetPut(strWhere);
        }

        public RateTotal.FeedCount GetFeedCount(string ProductSize, string Material, string Porcelain, string Model,
            string StartTime, string EndTime, SqlConnection con,string chrLot, string SizeMark)
        {
            string strWhere = "";
            if (ProductSize != null)
            {
                switch (SizeMark)
                {
                    case "0":
                        SizeMark = ">";
                        break;
                    case "1":
                        SizeMark = "=";
                        break;
                    case "2":
                        SizeMark = "<";
                        break;
                    case "3":
                        SizeMark = ">=";
                        break;
                    case "4":
                        SizeMark = "<=";
                        break;
                    default:
                        break;
                }
                strWhere = " and  SUBSTRING(ChrItemID,0,5)"+SizeMark+"'"+ ProductSize + "'";
            }
            if (Material != null)
            {
                if (Material == "Y")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(ChrItemID,'0')='Y5V' ";
                }
                else if (Material == "B")
                {
                    strWhere += " and(dbo.getTemperatureForlabelByType(ChrItemID,'0')='X7R' or"
                        + " dbo.getTemperatureForlabelByType(ChrItemID,'0')='X5R') ";
                }
                else if (Material == "N")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(ChrItemID,'0')='COG' ";
                }
            }
            if (Porcelain != null)
            {
                strWhere += "and chrLot in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='" + Porcelain + "')";
            }
            if (Model != null)
            {
                strWhere += "and ChrItemID like '%" + Model + "%'";
            }
            if (StartTime != null && EndTime != null)
            {
                strWhere += "And datInOutDate Between'" + StartTime + " ' And '" + EndTime + " '";
            }
            if(chrLot!=null)
            {
                strWhere += " and chrLot='" + chrLot + "' ";
            }
            BllRateTotal Bll = new BllRateTotal(con);
            return Bll.GetFeedCount(strWhere);
        }

        public RateTotal.MonthCheckProd GetMonthCheckProd(string ProductSize, string Material, string Porcelain, string Model,
           string Time,  SqlConnection con,string chrBatchID, string SizeMark)
        {
            string strWhere = "";
            string strWhere2 = "";
            if (ProductSize != null)
            {
                switch (SizeMark)
                {
                    case "0":
                        SizeMark = ">";
                        break;
                    case "1":
                        SizeMark = "=";
                        break;
                    case "2":
                        SizeMark = "<";
                        break;
                    case "3":
                        SizeMark = ">=";
                        break;
                    case "4":
                        SizeMark = "<=";
                        break;
                    default:
                        break;
                }
                
                strWhere += " and SUBSTRING(chrType,0,5)"+SizeMark+"'" + ProductSize + "'";
                strWhere2 += " and left(ChrItemID,4)"+SizeMark+"'" + ProductSize + "'";
            }
            if (Material != null)
            {
                if (Material == "Y")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(chrType,'0')='Y5V' ";
                    strWhere2 += " and dbo.getTemperatureForlabelByType(ChrItemID,'0')='Y5V'";
                }
                else if (Material == "B")
                {
                    strWhere += " and(dbo.getTemperatureForlabelByType(chrType,'0')='X7R' or"
                        + " dbo.getTemperatureForlabelByType(chrType,'0')='X5R') ";
                    strWhere2 += " and (dbo.getTemperatureForlabelByType(ChrItemID,'0')='X7R' or"
                        + " dbo.getTemperatureForlabelByType(ChrItemID,'0')='X5R')";
                }
                else if (Material == "N")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(chrType,'0')='COG' ";
                    strWhere2 += " and dbo.getTemperatureForlabelByType(ChrItemID,'0')='COG'";
                }
            }
            if (Porcelain != null)
            {
                strWhere += "and chrBatchID in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='"+ Porcelain + "')";
                strWhere2 += " and chrLot in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='" + Porcelain + "')";
            }
            if (Model != null)
            {
                strWhere += "and chrType like '%" + Model + "%'";
                strWhere2 += " and ChrItemID like '%" + Model + "%'";
            }
            if (Time != null )
            {
                strWhere += " And year(datDate)="+Convert.ToDateTime(Time).Year
                    + " and month(datDate)="+ Convert.ToDateTime(Time).Month + "";
                strWhere2 += " and year(SaveDate)=" + Convert.ToDateTime(Time).Year
                    + " and month(SaveDate)=" + Convert.ToDateTime(Time).Month + "";
            }
            if (chrBatchID!=null)
            {
                strWhere += " and chrBatchID='" + chrBatchID + "'";
            }
            BllRateTotal Bll = new BllRateTotal(con);
            return Bll.GetMonthCheckProd(strWhere,strWhere2);
        }

        public List<RateTotal.dTempStoreBalance> GetdTempStoreBalance(string ProductSize, string Material,
            string Porcelain, string Model, SqlConnection con, string SizeMark)
        {
            string strWhere = "  ";
            if (Material != null)
            {
                if (Material == "Y")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(ChrItemID,'0')='Y5V' ";
                }
                else if (Material == "B")
                {
                    strWhere += " and  (dbo.getTemperatureForlabelByType(ChrItemID,'0')='X7R' or"
                        + " dbo.getTemperatureForlabelByType(ChrItemID,'0')='X5R') ";
                }
                else if (Material == "N")
                {
                    strWhere += " and dbo.getTemperatureForlabelByType(ChrItemID,'0')='COG' ";
                }
            }
            if (ProductSize != null)
            {
                switch (SizeMark)
                {
                    case "0":
                        SizeMark = ">";
                        break;
                    case "1":
                        SizeMark = "=";
                        break;
                    case "2":
                        SizeMark = "<";
                        break;
                    case "3":
                        SizeMark = ">=";
                        break;
                    case "4":
                        SizeMark = "<=";
                        break;
                    default:
                        break;
                }
                strWhere += " and SUBSTRING(ChrItemID,0,5)"+SizeMark+"'" + ProductSize + "'";
            }
            if (Porcelain != null)
            {
                strWhere += "and chrLot in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='" + Porcelain + "')";
            }
            if (Model != null)
            {
                strWhere += "and ChrItemID  like '%" + Model + "%'";
            }
            BllRateTotal Bll = new BllRateTotal(con);
            return Bll.GetdTempStoreBalance(strWhere);
        }

        public RateTotal.ZhuanZhengpin GetZhuanZhengPin(string ProductSize, string Material,
            string Porcelain, string Model,string Type, SqlConnection con, string StartTime, string EndTime,
            string SizeMark)
        {
            string strWhere = "  ";
            if (Material != null)
            {
               strWhere += @"And ((t2.DecInQnty <> 0 And dbo.GetDepotIdByType(t2.chrItemID) ='"+Material+"')"
 +" Or(Isnull(t2.decChangeInQnty, 0) <> 0 And dbo.GetDepotIdByType(t2.chrChangeItemID) ='"+ Material + "')) ";
            }
            if (ProductSize != null)
            {
                switch (SizeMark)
                {
                    case "0":
                        SizeMark = ">";
                        break;
                    case "1":
                        SizeMark = "=";
                        break;
                    case "2":
                        SizeMark = "<";
                        break;
                    case "3":
                        SizeMark = ">=";
                        break;
                    case "4":
                        SizeMark = "<=";
                        break;
                    default:
                        break;
                }
                strWhere += " and left(substring(t1.chrItemid,3,len(t1.chrItemid)-2),4)"+SizeMark+"'"+ProductSize+"' "
                    + " and left(substring(t2.chrChangeItemid,3,len(t2.chrChangeItemid)-2),4)"+SizeMark+"'"+ ProductSize + "' ";

            }
            if (Porcelain != null)
            {
                strWhere += " and t2.chrChangeLot in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='" + Porcelain + "') ";
            }
            if (Model != null)
            {
                strWhere += " and substring(t2.chrChangeItemid,3,len(t2.chrChangeItemid)-2) like '%"+Model+"%'"
                    + " and substring(t2.chritemid,3,len(t2.chritemid)-3) like '%"+Model+"%' ";
            }
            if(StartTime!=null&&EndTime!=null)
            {
                strWhere += " and t2.datInOutStock Between '" + StartTime + "' And '" + EndTime + " ' ";
            }
            if(Type!=null)
            {
                strWhere += " and   t1.chrItemid Like '" + Type + "%' ";
            }

            BllRateTotal Bll = new BllRateTotal(con);
            return Bll.GetZhuanZhengpin(strWhere);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="ChrItemID"></param>
        /// <param name="ChrSize"></param>
        /// <param name="ChrClass"></param>
        /// <param name="ChrPorcelain"></param>
        /// <param name="SizeMark"></param>
        /// <param name="LastMonthCheck"></param>
        /// <param name="IsMonth"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<AmountData>  GetData(DateTime StartTime, DateTime EndTime, string ChrItemID, string ChrSize, string ChrClass, 
            string ChrPorcelain,string SizeMark,decimal LastMonthCheck,bool IsMonth,SqlConnection con)
        {
            //string strStartTime = "2017-12-1";
            //string strEndTime = "2017-12-6";
            string ExtendSql_Storage2FIE = Set_ExtendSql(ChrItemID, ChrSize, ChrClass, ChrPorcelain, SizeMark, 1, "FIE");//非命中品入库
            string ExtendSql_Storage2FIA = Set_ExtendSql(ChrItemID, ChrSize, ChrClass, ChrPorcelain, SizeMark, 1, "FIA");//正品入库
            string ExtendSql_OutIn= Set_ExtendSql(ChrItemID, ChrSize, ChrClass, ChrPorcelain, SizeMark, 2, "12");//次品转正品借用出入库
            string ExtendSql_OutIn2 = Set_ExtendSql(ChrItemID, ChrSize, ChrClass, ChrPorcelain, SizeMark, 2, "13");//非命中品转正品借用出入库
            string ExtendSql_Feeding = Set_ExtendSql(ChrItemID, ChrSize, ChrClass, ChrPorcelain, SizeMark, 3);//投料数
            string ExtendSql_Month = @" where 1=1 "
              + " and ChrItemID like '%" + ChrItemID + "%' and ChrClass like '%" + ChrClass + "%'"
              + @" and ChrProcessID not in ('待丝印','丝印','薄膜膜片仓','新工艺一楼印刷','新工艺二楼叠层','新工艺一楼叠层',
'新工艺二楼印刷','新工艺','新工艺印刷','新工艺叠层','新工艺待印刷')";//流通库存
            if (ChrSize != ""&& ChrSize!=null)
            {
                ExtendSql_Month += " and ChrSize" + SizeMark + "'" + ChrSize + "' ";
            }
            if(ChrPorcelain!=null&& ChrPorcelain!="")
            {
                ExtendSql_Month += " and ChrPorcelain='" + ChrPorcelain + "' ";
            }
          
            DateTime TempTime = StartTime;
            List<AmountData> List_Data = new List<AmountData>();
            TimeSpan TimeSpan = EndTime.Subtract(StartTime);
            for (int i=0;i<= TimeSpan.Days;i++)
            {
                AmountData AmountData = new AmountData();
                string Temp_Data= " and Date between '" + StartTime.ToString("yyyy-MM-dd") + "' and '"
                    + TempTime.ToString("yyyy-MM-dd") + " 23:59:59' ";
                string Temp_Storage2FIE = ExtendSql_Storage2FIE + Temp_Data;
                string Temp_Storage2FIA = ExtendSql_Storage2FIA + Temp_Data;
                string Temp_OutIn = ExtendSql_OutIn + Temp_Data;
                string Temp_OutIn2 = ExtendSql_OutIn2 + Temp_Data;
                string Temp_Feeding= ExtendSql_Feeding +" and Date between '" + StartTime.ToString("yyyy-MM-dd") + " 08:00:00' and '"
                    + TempTime.AddDays(1).ToString("yyyy-MM-dd") + " 07:59:59' ";
                string Temp_Month = ExtendSql_Month + " and Date between '" + TempTime.ToString("yyyy-MM-dd") + "' and '"
                    + TempTime.ToString("yyyy-MM-dd") + " 23:59:59' "; ;
                AmountData.Date = TempTime.ToString("MM-dd");
                BllRateTotal Bll = new BllRateTotal(con);
                if(i == TimeSpan.Days)
                {
                    decimal Dec_Month = 0;
                    //①开始时间至当前时间正品入库数
                    RateTotal.PutCount Obj_Put = GetPut(ChrSize, ChrClass, ChrPorcelain, ChrItemID, StartTime.ToString("yyyy-MM-dd"),
                        EndTime.ToString("yyyy-MM-dd 23:59:59"), true, con, SizeMark);
                    decimal Dec_FIA = Obj_Put != null ? Obj_Put.decQuantity : 0;
                    //②开始时间至当前时间非命中入库数
                    Obj_Put = GetPut(ChrSize, ChrClass, ChrPorcelain, ChrItemID, StartTime.ToString("yyyy-MM-dd"),
                        EndTime.ToString("yyyy-MM-dd 23:59:59"), false, con, SizeMark);
                    decimal Dec_FIE = Obj_Put != null ? Obj_Put.decQuantity : 0;
                    //③开始时间至当前时间投料数
                    RateTotal.FeedCount  Obj_Feed = GetFeedCount(ChrSize, ChrClass, ChrPorcelain, ChrItemID, StartTime.ToString("yyyy-MM-dd 08:00:00"),
                         EndTime.AddDays(1).ToString("yyyy-MM-dd 07:59:59"), con, null, SizeMark);
                    decimal Dec_Feeding = Obj_Feed != null ? Obj_Feed.decquantity : 0;
                    RateTotal.ZhuanZhengpin Obj_CiZhuanZheng= GetZhuanZhengPin(ChrSize, ChrClass, ChrPorcelain, ChrItemID, "12", con, StartTime.ToString("yyyy-MM-dd"),
                        EndTime.ToString("yyyy-MM-dd 23:59:59"), SizeMark);
                    RateTotal.ZhuanZhengpin Obj_FeiZhuanZheng = GetZhuanZhengPin(ChrSize, ChrClass, ChrPorcelain, ChrItemID, "13", con, StartTime.ToString("yyyy-MM-dd"),
                        EndTime.ToString("yyyy-MM-dd 23:59:59"), SizeMark);
                    decimal CiZhuanZheng = Obj_CiZhuanZheng != null ? Obj_CiZhuanZheng.decChangeInQnty : 0;
                    decimal FeiZhuanZheng = Obj_FeiZhuanZheng != null ? Obj_FeiZhuanZheng.decChangeInQnty : 0;
                    if (IsMonth == true)
                    {
                        //获取本月半成品盘点库存
                        RateTotal.MonthCheckProd Obj_MonthCheckProd= GetMonthCheckProd(ChrSize, ChrClass, ChrPorcelain, ChrItemID,
                            EndTime.ToString("yyyy-MM-dd 23:59:59"), con, null, SizeMark);
                        Dec_Month = Obj_MonthCheckProd != null ? Obj_MonthCheckProd.MonthCheckAmount 
                            + Obj_MonthCheckProd.LiuTongAmount + Obj_MonthCheckProd.ZhengpinAmount : 0;
                    }
                    else
                    {
                        //获取流通库存
                        List<RateTotal.dTempStoreBalance> Obj_StoreBalance = GetdTempStoreBalance(ChrSize, ChrClass,
                            ChrPorcelain, ChrItemID, con, SizeMark);
                        Dec_Month = (Obj_StoreBalance != null && Obj_StoreBalance.Count > 0) ? Obj_StoreBalance.FirstOrDefault().DecQtyNal : 0;
                    }
                    AmountData.RealThing = Dec_FIA; 
                    AmountData.NotHitThing = Dec_FIE;
                    AmountData.FeedingAmount = Dec_Feeding;
                    AmountData.LastMonthCheck = LastMonthCheck;
                    AmountData.OutInDefective = CiZhuanZheng;
                    AmountData.OutInNotHit = FeiZhuanZheng;
                    AmountData.MonthCheck = Dec_Month;
                    AmountData.ChrClass = ChrClass;
                }
                else
                {

                    //①开始时间至当前时间正品入库数
                    decimal Dec_FIA = Bll.Get_WarehouseStorage(Temp_Storage2FIA);
                    //②开始时间至当前时间非命中入库数
                    decimal Dec_FIE = Bll.Get_WarehouseStorage(Temp_Storage2FIE);
                    //③开始时间至当前时间投料数
                    decimal Dec_Feeding = Bll.Get_Feeding(Temp_Feeding);
                    //④开始时间至当前时间半成品(即流通库存)
                    decimal Dec_Month = 0;
                    Dec_Month = Bll.Get_CirculationStock(Temp_Month);
                    //⑤开始时间至当前时间次品转正品
                    decimal OutIn_Defective = Bll.Get_OutInOfStorage(Temp_OutIn) ;
                    //⑤开始时间至当前时间非命中转正品
                    decimal OutIn_NotHit = Bll.Get_OutInOfStorage(Temp_OutIn2);
                    AmountData.RealThing = Dec_FIA;
                    AmountData.NotHitThing = Dec_FIE;
                    AmountData.FeedingAmount = Dec_Feeding;
                    AmountData.LastMonthCheck = LastMonthCheck;
                    AmountData.OutInDefective = OutIn_Defective;
                    AmountData.OutInNotHit = OutIn_NotHit;
                    AmountData.MonthCheck = Dec_Month;
                    AmountData.ChrClass = ChrClass;
                }
                List_Data.Add(AmountData);
                TempTime = TempTime.AddDays(1);
            }
            return List_Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChrItemID">型号规格</param>
        /// <param name="ChrSize">尺寸</param>
        /// <param name="ChrClass">Y/B/N</param>
        /// <param name="ChrPorcelain">瓷粉</param>
        /// <param name="SizeMark">尺寸比较符号</param>
        /// <param name="ExtendSql_Type">Sql类型，1：成品仓入库；2：借用出入库；3：投料数；4：流通库存;</param>
        /// <param name="ChrType">
        /// 当ExtendSql_Type=1时，ChrType表示‘成品仓入库’正品/非命中品入库；
        /// 当ExtendSql_Type=2时，ChrType表示‘借用出入库’次品转正品/非命中转正品；
        /// </param>
        /// <param name="ChrParentProcId">表示‘流通库存’当前产品所在工序</param>
        /// <param name="ChrProcessID">表示‘流通库存’产品所在前工序</param>
        /// <returns></returns>
        public string Set_ExtendSql(string ChrItemID, string ChrSize, string ChrClass, string ChrPorcelain,
             string SizeMark, int ExtendSql_Type = 0,string ChrType="",string ChrParentProcId="",string ChrProcessID="")
        {
            ChrItemID = ChrItemID != null ? ChrItemID : "";
            ChrSize= ChrSize!=null ? ChrSize : "";
            ChrClass = ChrClass != null ? ChrClass : "";
            ChrPorcelain = ChrPorcelain != null ? ChrPorcelain : "";
            ChrType = ChrType != null ? ChrType : "";
            string strSql = @" where 1=1 ";
            if(ChrSize!="")
            {
                strSql += " and ChrSize" + SizeMark + "'" + ChrSize + "' ";
            }
            if(ChrPorcelain!="")
            {
                strSql += " and ChrPorcelain='"+ ChrPorcelain + "'";
            }
            switch(ExtendSql_Type)
            {
                case 1:
                    strSql += " and ChrType ='" + ChrType + "' and ChrItemID like '%" + ChrItemID + "%' and ChrClass like '%" + ChrClass + "%' ";
                    break;
                case 2:
                    strSql += " and ChrType ='" + ChrType + "' and ((isnull(DecInQnty,0)<>0 and ChrClass='" + ChrClass
                        + "') or (isnull(DecQty,0)<>0 and ChrClass='" + ChrClass + "') ) and ("+
                        " ( ChrItemID like '%" + ChrItemID + "%')or  (ChrChangeItemID like '%"+ ChrItemID + "%')"
                        + ") ";
                    break;
                case 3:
                    strSql += " and ChrItemID like '%" + ChrItemID + "%' and  ChrClass like '%" + ChrClass + "%' ";
                    break;
                case 4:
                    strSql += " and  ChrItemID like '%" + ChrItemID + "%' and  ChrClass like '%" + ChrClass + "%' and ChrParentProcId like '%"+
                        ChrParentProcId + "%' and ChrProcessID like '%"+ ChrProcessID + "%'";
                    break;
                default:
                    break;
                   

            }
            return strSql;
        }

        /// <summary>
        /// 
        /// </summary>
        public class AmountData
        {
            public decimal RealThing { get; set; }//正品
            public decimal NotHitThing { get; set; }//非命中品
            public decimal FeedingAmount { get; set; }//投料数
            public decimal LastMonthCheck { get; set; }//上月半成品
            //public decimal OutInOfStorage { get; set; }//非命中转正品/次品转正品
            public decimal MonthCheck { get; set; }//本月半成品
            public string Date { get; set; }
            public string ChrClass { get; set; }
            public decimal OutInDefective { get; set; }
            public decimal OutInNotHit { get; set; }

        }


        public bool SetTime(string Time,out string StartTime,out string EndTime)
        {
            DateTime Date = Convert.ToDateTime(Time);
            DateTime Now = DateTime.Now;
            int Month = Date.Month;
            int NowMonth = DateTime.Now.Month;
            StartTime = "";
            EndTime = "";
            bool flag = false;//计算当前的流通库存或者盘点库存
            bool IsGet = true;//是否需要获取返回值
            if (Date.Year == Now.Year)
            {

                if (Month < NowMonth)
                {
                    flag = true;
                    switch (Month)
                    {
                        case 1:
                            StartTime = Time + "/01 ";
                            EndTime = Time + "/25 ";
                            break;
                        case 6:
                            StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                            EndTime = Time + "/30 ";
                            break;
                        case 7:
                            StartTime = Time + "/01 ";
                            EndTime = Time + "/25 ";
                            break;
                        default:
                            StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                            EndTime = Time + "/25 ";
                            break;
                    }
                }
                else if (Month == NowMonth)
                {
                    switch (Month)
                    {
                        case 1:
                            if (Now.Day > 25)
                            {
                                StartTime = Time + "/01 ";
                                EndTime = Time + "/25 ";
                                flag = true;
                            }
                            else
                            {
                                StartTime = Time + "/01 ";
                                EndTime = Now.ToString("yyyy/MM/dd ");
                                flag = false;
                            }
                            break;
                        case 6:
                            StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                            EndTime = Now.ToString("yyyy/MM/dd ");
                            flag = false;
                            break;
                        case 7:
                            if (Now.Day > 25)
                            {
                                StartTime = Time + "/01 ";
                                EndTime = Time + "/25 ";
                                flag = true;
                            }
                            else
                            {
                                StartTime = Time + "/01 ";
                                EndTime = Now.ToString("yyyy/MM/dd ");
                                flag = false;
                            }
                            break;
                        case 12:
                            if (Now.Day > 30)
                            {
                                StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                EndTime = Time + "/30 ";
                                flag = true;
                            }
                            else
                            {
                                StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                                EndTime = Now.ToString("yyyy/MM/dd");
                                flag = false;
                            }
                            break;
                        default:
                            StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                            if (Now.Day > 25)
                            {
                                EndTime = Time + "/25 ";
                                flag = true;
                            }
                            else
                            {
                                EndTime = DateTime.Now.ToString("yyyy/MM/dd");
                                flag = false;
                            }
                            break;
                    }
                }
                else if (Month == NowMonth + 1)
                {
                    if (Now.Day >= 26 && Month != 7)
                    {
                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                        EndTime = Now.ToString("yyyy/MM/dd ");
                        flag = false;
                    }
                    else
                    {
                        IsGet = false;
                    }
                }
                else
                {
                    IsGet = false;
                }

            }
            else if (Date.Year < Now.Year)
            {
                flag = true;
                switch (Month)
                {
                    case 1:
                        StartTime = Time + "/01 ";
                        EndTime = Time + "/25 ";
                        break;
                    case 6:
                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                        EndTime = Time + "/30 ";
                        break;
                    case 7:
                        StartTime = Time + "/01 ";
                        EndTime = Time + "/25 ";
                        break;
                    case 12:
                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                        EndTime = Time + "/30 ";
                        break;
                    default:
                        StartTime = (Date.AddMonths(-1)).ToString("yyyy/MM") + "/26 ";
                        EndTime = Time + "/25 ";
                        break;
                }
            }
            return flag;
        }

        public class ExcelEntity
        {
            public string title { get; set; }
            public decimal lastMonthCheck { get; set; }
            public decimal feedingAmount { get; set; }
            public decimal stockAmount { get; set; }
            public decimal noHitAmount { get; set; }
            public decimal allStockAmount
            {
                get
                {
                    return stockAmount + noHitAmount;
                }
                set
                {
                    allStockAmount = value;
                }
            }
            public decimal monthCheck { get; set; }
            public decimal allRate
            {
                get { return (Math.Round((allStockAmount / ((feedingAmount + lastMonthCheck) - monthCheck))*100,2)); }
                set { allRate = value; }
            }

            public decimal stockRate
            {
                get
                {
                    return (Math.Round((stockAmount / ((feedingAmount + lastMonthCheck) - monthCheck))*100, 2)) ;
                }
                set
                {
                    stockRate = value;
                }
            }
            public string date
            {
                get;set;
            }

        }



    }
}