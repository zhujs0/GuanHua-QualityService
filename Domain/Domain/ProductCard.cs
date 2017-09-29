using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ProductCard
    {
        /// <summary>
        /// 售前备货卡tb_GetBraidStockupdel
        /// </summary>
        public class BraidStockCard
        {
            public string chrItem { get; set; }
            public string chrType { get; set; }
            public string chrLot { get; set; }
            public string chritemid { get; set; }
            public string chrkind { get; set; }
            public string chrOrderId { get; set; }
            public decimal decBraidQuanty { get; set; }
        }
        /// <summary>
        /// 发货卡 sq_invoiceCard
        /// </summary>
        public class InvoiceCard
        {
            public string chrID { get; set; }
            public string chrOrderID { get; set; }
            public string chrKHName { get; set; }
            public string chrKHType { get; set; }
            public string chrKHXH { get; set; }
            public string chrGHXH1 { get; set; }
            public decimal intBH { get; set; }
            public string chrLOT { get; set; }
        }
        /// <summary>
        /// 流通备货卡 tb_PacketBraidStockupDetail
        /// </summary>
        public class PacketBraidCard
        {
            public string chrId { get; set; }
            public string chrPlanItemid { get; set; }
            public string chrPlanQualityType { get; set; }
            public string chrLot { get; set; }
            public string chrItemId { get; set; }
            public string chrQualityType { get; set; }
            public decimal decQuantity { get; set; }
            public decimal decBraidQuantity { get; set; }
        }

        public class CardInfo
        {
            public string FKOrderNo { get; set; }//反馈单单号
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
