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
    [Route("api/RateTotal")]
    public class RateTotalController : Controller
    {
        private string _ConString = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("host.json", optional: true).Build().GetSection("ghSource").Value;
        [HttpGet]
        public Object Get(string ActionType,string ProductSize,string Porcelain,string chrType,
            string Time,string Material)
        {
            string Model = chrType;
            using (SqlConnection con = new SqlConnection(_ConString))
            {
                try
                {
                    ReturnData Item = new ReturnData();
                    BllRateTotal Bll = new BllRateTotal(con);
                    if (ActionType == "GetPorcelain")
                    {
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
                                            EndTime = Now.ToString("yyyy/MM/dd HH:mm:ss");
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
                        //string[] chr = new string[]{"Y", "B" ,"N"};
                        if(IsGet)
                        {
                           


                            //①入库数正品
                            RateTotal.PutCount StockList = GetPut(ProductSize, Material, Porcelain,
                                Model, StartTime+" 00:00:00", EndTime+" 23:59:59", true, con);
                            decimal StockAmount = StockList != null ? StockList.decQuantity : 0;
                            //②入库数非命中数
                            RateTotal.PutCount StockListHit = GetPut(ProductSize, Material, Porcelain,
                                Model, StartTime + " 00:00:00", EndTime + " 23:59:59", false, con);
                            decimal HitAmount = StockListHit != null ? StockListHit.decQuantity : 0;

                            //③投料数
                            RateTotal.FeedCount FeedList = GetFeedCount(ProductSize, Material, Porcelain, Model,
                                StartTime+ " 08:00:00", 
                                Convert.ToDateTime(EndTime).AddDays(+1).ToString("yyyy/MM/dd")+ " 07:59:59", con, null);
                            decimal FeedCount = FeedList != null ? FeedList.decquantity : 0;

                            //④上月盘点数含合批仓
                            RateTotal.MonthCheckProd MonthList = GetMonthCheckProd(ProductSize, Material, Porcelain, Model,
                                (Date.AddMonths(-1)).ToString("yyyy/MM"), con, null);
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
                                Time, con, null);
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
                                    Porcelain, Model, con);
                                LTKunCun = dList.Count > 0 ? dList[0].DecQtyNal : 0;
                            }

                            //⑥次品转正品
                            decimal CiPinZhuanAmount = 0;
                            RateTotal.ZhuanZhengpin CiZhuan = GetZhuanZhengPin(ProductSize, Material, Porcelain, Model,
                                "12", con, StartTime + " 00:00:00", EndTime + " 23:59:59");
                            CiPinZhuanAmount = CiZhuan != null ? CiZhuan.decChangeInQnty : 0;

                            //⑦非命中转正品
                            RateTotal.ZhuanZhengpin HitZhuan= GetZhuanZhengPin(ProductSize, Material, Porcelain, Model,
                                "13", con, StartTime + " 00:00:00", EndTime + " 23:59:59");
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
                            return Item;
                        }
                        else
                        {
                            return null;
                        }
                        
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    ResultData result = new ResultData();
                    result.Result = false;
                    result.ErrMsg = ex.Message;
                    return result;
                }
               
            }
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
            string StartTime, string EndTime,bool flag, SqlConnection con)
        {
            string strWhere = "";
            if (ProductSize != null)
            {
                strWhere = " and  substring(chrItemId,3,4)='" + ProductSize + "'";
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
            string StartTime, string EndTime, SqlConnection con,string chrLot)
        {
            string strWhere = "";
            if (ProductSize != null)
            {
                strWhere = " and  SUBSTRING(ChrItemID,0,5)='"+ ProductSize + "'";
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
           string Time,  SqlConnection con,string chrBatchID)
        {
            string strWhere = "";
            string strWhere2 = "";
            if (ProductSize != null)
            {
                strWhere += " and SUBSTRING(chrType,0,5)='" + ProductSize + "'";
                strWhere2 += " and left(ChrItemID,4)='" + ProductSize + "'";
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
            string Porcelain, string Model, SqlConnection con)
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
                strWhere += " and SUBSTRING(ChrItemID,0,5)='" + ProductSize + "'";
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
            string Porcelain, string Model,string Type, SqlConnection con, string StartTime, string EndTime)
        {
            string strWhere = "  ";
            if (Material != null)
            {
                strWhere += @"And ((t2.DecInQnty <> 0 And dbo.GetDepotIdByType(t2.chrItemID) ='"+Material+"')"
 +" Or(Isnull(t2.decChangeInQnty, 0) <> 0 And dbo.GetDepotIdByType(t2.chrChangeItemID) ='"+ Material + "')) ";
            }
            if (ProductSize != null)
            {
                strWhere += " and left(substring(t1.chrItemid,3,len(t1.chrItemid)-2),4)='"+ProductSize+"' ";
            }
            if (Porcelain != null)
            {
                strWhere += " and t2.chrChangeLot in (select tp_carCraft.chrBatchID from tp_carCraft where chrPowder='" + Porcelain + "') ";
            }
            if (Model != null)
            {
                strWhere += " and substring(t2.chrChangeItemid,3,len(t2.chrChangeItemid)-2) like '%"+Model+"%'";
            }
            if(StartTime!=null&&EndTime!=null)
            {
                strWhere += " and t2.datInOutStock Between '" + StartTime + "' And '" + EndTime + " '";
            }
            if(Type!=null)
            {
                strWhere += " and   t1.chrItemid Like '" + Type + "%'";
            }

            BllRateTotal Bll = new BllRateTotal(con);
            return Bll.GetZhuanZhengpin(strWhere);
        }

    }
}