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
    [Route("api/ProductCard")]
    public class ProductCardController : Controller
    {
        private string conString = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("host.json", optional: true).Build().GetSection("ghSource").Value;

        //[HttpGet("/ProductCard/{id}")]
        //[HttpGet("/ProductCard")]
        [HttpGet]
        public Object Get(string ActionType,string CardNo,string ProductClass,string ProductModel,
            string BatchNo,string Customer)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                string strWhere = "";
                CardInfo item = new CardInfo();
                BllProductCard Bll = new BllProductCard(con);
                if (ActionType == "First")
                {
                    if (CardNo.StartsWith("BH"))
                    {
                        strWhere = " where chrOrderId='" + CardNo + "' ";
                    }
                    else if(CardNo.StartsWith("ZD"))
                    {
                        strWhere = " where chrID='" + CardNo + "' ";
                    }
                    else if(CardNo.StartsWith("LB"))
                    {
                        strWhere = " where chrId='" + CardNo + "' ";
                    }
                }
                else
                {
                    if (CardNo.StartsWith("BH"))
                    {
                        strWhere = " where chrOrderId='" + CardNo + "' and chrkind='"
                                        + ProductClass + "' and chritemid like '%" + ProductModel
                                + "' and chrLot='" + BatchNo + "'"; 
                    }
                    else if (CardNo.StartsWith("ZD"))
                    {
                        strWhere = " where chrID='" + CardNo+ "' and chrGHXH1 like '%" + ProductModel 
                                + "' and left(chrGHXH1,2)='" + ProductClass
                               + "' and chrKHName='" + Customer + "' and chrLOT='" + BatchNo + "' "; 
                    }
                    else if (CardNo.StartsWith("LB"))
                    {
                        strWhere = " where chrId='" + CardNo + "' and chrItemId='" + ProductModel 
                            + "' and chrQualityType='" + ProductClass+ "' and chrLot='" + BatchNo + "'";
                    }
                }
                if (CardNo.StartsWith("BH"))
                {
                    ProductCard.BraidStockCard BraidStockCard = Bll.GetBraidStockCard(strWhere);
                    if (BraidStockCard != null)
                    {
                        item.cardNo = CardNo;
                        item.tempAmount = BraidStockCard.decBraidQuanty;
                        item.batchNo = BraidStockCard.chrLot;
                        item.cardNo = BraidStockCard.chrOrderId;
                        item.tempmodel = BraidStockCard.chrItem;
                        item.productModel = (BraidStockCard.chritemid).Substring(2, (BraidStockCard.chritemid).Length-2);
                        item.tempClass = BraidStockCard.chrType;
                        item.batchNo = BraidStockCard.chrLot;
                        item.productClass = BraidStockCard.chrkind;
                    }
                }
                else if (CardNo.StartsWith("ZD"))
                {
                    ProductCard.InvoiceCard InvoiceCard = Bll.InvoiceCard(strWhere);
                    if (InvoiceCard != null)
                    {
                        item.cardNo = CardNo;
                        item.orderNo = InvoiceCard.chrOrderID;
                        item.customer = InvoiceCard.chrKHName;
                        item.tempClass = InvoiceCard.chrKHType;
                        item.tempmodel = InvoiceCard.chrKHXH;
                        item.productModel = (InvoiceCard.chrGHXH1).Substring(4, (InvoiceCard.chrGHXH1).Length-4);
                        item.tempAmount = InvoiceCard.intBH;
                        item.batchNo = InvoiceCard.chrLOT;
                        item.productClass = InvoiceCard.chrGHXH1.Substring(0, 2);
                    }
                }
                else if (CardNo.StartsWith("LB"))
                {
                    ProductCard.PacketBraidCard PacketBraidCard = Bll.PacketBraidCard(strWhere);
                    if (PacketBraidCard != null)
                    {
                        item.cardNo = CardNo;
                        item.tempAmount = PacketBraidCard.decBraidQuantity;
                        item.amount = PacketBraidCard.decQuantity;
                        item.cardNo = PacketBraidCard.chrId;
                        item.tempmodel = PacketBraidCard.chrPlanItemid;
                        item.productModel = PacketBraidCard.chrItemId;
                        item.tempClass = PacketBraidCard.chrPlanQualityType;
                        item.productClass = PacketBraidCard.chrQualityType;
                        item.batchNo = PacketBraidCard.chrLot;
                    }
                }
                return item;
            }
               
        }


        //[HttpGet]
        //public Object Get(string First, string CardList)
        //{
        //    using (SqlConnection con = new SqlConnection(conString))
        //    {
        //        BllProductCard Bll = new BllProductCard(con);
        //        string strWhere = "";
        //        string CardNo = "", BatchNo = "" ;
        //        string ProductClass = "", Model="";
        //        string Customer = "";
        //        string[] chrList = CardList.Split(',');
        //        ResponseData response = new ResponseData();
        //        List<CardInfo> list = new List<CardInfo>();
        //        if (chrList.Length>0)
        //        {
        //            CardNo = chrList[0].ToString();
        //            var CardType = CardNo.Substring(0, 2);
        //            if (CardNo.StartsWith("BH"))
        //            {
        //                strWhere = " where chrOrderId='" + CardNo + "' ";
        //                ProductCard.BraidStockCard BraidStockCard = Bll.GetBraidStockCard(strWhere);
        //                if(BraidStockCard!=null)
        //                {
        //                    CardInfo info = new CardInfo();
        //                    BatchNo = BraidStockCard.chrLot;
        //                    ProductClass = BraidStockCard.chrkind;
        //                    Model = (BraidStockCard.chritemid).Substring(2, (BraidStockCard.chritemid).Length - 2);

        //                    info.tempAmount = BraidStockCard.decBraidQuanty;
        //                    info.batchNo = BraidStockCard.chrLot;
        //                    info.cardNo = BraidStockCard.chrOrderId;
        //                    info.tempmodel = BraidStockCard.chrItem;
        //                    info.productModel = BraidStockCard.chritemid;
        //                    info.tempClass = BraidStockCard.chrType;
        //                    info.batchNo = BraidStockCard.chrLot;
        //                    list.Add(info);
        //                    for (int i = 1; i < chrList.Length; i++)
        //                    {
        //                        if (chrList[i].StartsWith(CardType))
        //                        {
        //                            strWhere = " where chrOrderId='"+ chrList [i]+ "' and chrkind='" 
        //                                + ProductClass + "' and chritemid like '%" + Model
        //                        + "' and chrLot='" + BatchNo + "'";
        //                            BraidStockCard = Bll.GetBraidStockCard(strWhere);
        //                            if(BraidStockCard!=null)
        //                            {
        //                                CardInfo item = new CardInfo();
        //                                item.tempAmount = BraidStockCard.decBraidQuanty;
        //                                item.batchNo = BraidStockCard.chrLot;
        //                                item.cardNo = BraidStockCard.chrOrderId;
        //                                item.tempmodel = BraidStockCard.chrItem;
        //                                item.productModel = BraidStockCard.chritemid;
        //                                item.tempClass = BraidStockCard.chrType;
        //                                item.batchNo = BraidStockCard.chrLot;
        //                                list.Add(item);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            response.result = false;
        //                            response.msg = "输入的不是售前备货卡";
        //                        }
        //                    }
        //                }
        //            }
        //            else if (CardNo.StartsWith("ZD"))
        //            {
        //                strWhere = " where chrID='" + CardNo + "' ";
        //                ProductCard.InvoiceCard InvoiceCard = Bll.InvoiceCard(strWhere);
        //                if(InvoiceCard!=null)
        //                {

        //                    CardInfo info = new CardInfo();
        //                    BatchNo = InvoiceCard.chrLOT;
        //                    ProductClass = InvoiceCard.chrGHXH1;
        //                    Model = (InvoiceCard.chrGHXH1).Substring(4, (InvoiceCard.chrGHXH1).Length - 4);

        //                    info.orderNo = InvoiceCard.chrOrderID;
        //                    info.customer = InvoiceCard.chrKHName;
        //                    info.tempClass = InvoiceCard.chrKHType;
        //                    info.tempmodel = InvoiceCard.chrKHXH;
        //                    info.productModel = InvoiceCard.chrGHXH1;
        //                    info.tempAmount = InvoiceCard.intBH;
        //                    info.batchNo = InvoiceCard.chrLOT;
        //                    list.Add(info);
        //                    for(int i=1;i<chrList.Length;i++)
        //                    {
        //                        if (chrList[i].StartsWith(CardType))
        //                        {
        //                            strWhere = " where chrID='"+ chrList [i]+ "' and chrGHXH1 like '%" + Model + "' and left(chrGHXH1,2)='" + ProductClass
        //                        + "' and chrKHName='" + Customer + "' and chrLOT='" + BatchNo + "' ";
        //                            InvoiceCard = Bll.InvoiceCard(strWhere);
        //                            if (InvoiceCard != null)
        //                            {
        //                                CardInfo item = new CardInfo();
        //                                item.orderNo = InvoiceCard.chrOrderID;
        //                                item.customer = InvoiceCard.chrKHName;
        //                                item.tempClass = InvoiceCard.chrKHType;
        //                                item.tempmodel = InvoiceCard.chrKHXH;
        //                                item.productModel = InvoiceCard.chrGHXH1;
        //                                item.tempAmount = InvoiceCard.intBH;
        //                                item.batchNo = InvoiceCard.chrLOT;
        //                                list.Add(item);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            response.result = false;
        //                            response.msg = "输入的不是售前备货卡";
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                strWhere = " where chrId='" + CardNo + "' ";
        //                ProductCard.PacketBraidCard PacketBraidCard = Bll.PacketBraidCard(strWhere);
        //                if(PacketBraidCard!=null)
        //                {
        //                    CardInfo info = new CardInfo();
        //                    BatchNo = PacketBraidCard.chrLot;
        //                    ProductClass = PacketBraidCard.chrQualityType;
        //                    Model = PacketBraidCard.chrItemId;

        //                    info.tempAmount = PacketBraidCard.decBraidQuantity;
        //                    info.amount = PacketBraidCard.decQuantity;
        //                    info.cardNo = PacketBraidCard.chrId;
        //                    info.tempmodel = PacketBraidCard.chrPlanItemid;
        //                    info.productModel = PacketBraidCard.chrItemId;
        //                    info.tempClass = PacketBraidCard.chrPlanQualityType;
        //                    info.productClass = PacketBraidCard.chrQualityType;
        //                    info.batchNo = PacketBraidCard.chrLot;
        //                    list.Add(info);
        //                    for (int i = 1; i < chrList.Length; i++)
        //                    {
        //                        if (chrList[i].StartsWith(CardType))
        //                        {
        //                            strWhere = " where chrId='"+ chrList[i] + "' and chrItemId='" + Model + "' and chrQualityType='" + ProductClass
        //                       + "' and chrLot='" + BatchNo + "'";
        //                            PacketBraidCard = Bll.PacketBraidCard(strWhere);
        //                            if (PacketBraidCard != null)
        //                            {
        //                                CardInfo item = new CardInfo();
        //                                item.tempAmount = PacketBraidCard.decBraidQuantity;
        //                                item.amount = PacketBraidCard.decQuantity;
        //                                item.cardNo = PacketBraidCard.chrId;
        //                                item.tempmodel = PacketBraidCard.chrPlanItemid;
        //                                item.productModel = PacketBraidCard.chrItemId;
        //                                item.tempClass = PacketBraidCard.chrPlanQualityType;
        //                                item.productClass = PacketBraidCard.chrQualityType;
        //                                item.batchNo = PacketBraidCard.chrLot;
        //                                list.Add(item);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            response.result = false;
        //                            response.msg = "输入的不是售前备货卡";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return list;
        //    }
        //}

        public class ResponseData
        {
            public bool result { get; set; }
            public string msg { get; set; }
            List<CardInfo> cardInfo { get; set; }
            public string cardList { get; set; }
        }
        public class CardInfo
        {
            public string cardNo { get; set; }//卡号
            public string orderNo { get; set; }//订单号
            public string customer { get; set; }//客户名称
            public string tempClass { get; set; }//备货/客户类别
            public string tempmodel { get; set; }//备货型号规格
            public string productModel { get; set; }//产品型号规格
            public string productClass { get; set; }//产品类别
            public decimal amount { get; set; }//数量
            public decimal tempAmount { get; set; }//备货数量
            public string batchNo { get; set; }//批号
        }

    }
}