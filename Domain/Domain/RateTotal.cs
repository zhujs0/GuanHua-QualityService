using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class RateTotal
    {
        public class tm_item
        {
            public string chritemid { get; set; }
            public string chrspec { get; set; }
        }

        public class PutCount
        {

            public string chrItemId { get; set; }
            public string chrSpec { get; set; }
            public string chrLot { get; set; }
            public decimal decQuantity { get; set; }
            public DateTime datIssRptDate { get; set; }
            //public string chrlot { get; set; }
            //public string chritemid { get; set; }
            //public string decquantity { get; set; }
            //public string chrqualitytype { get; set; }
            //public string datissrptdate { get; set; }
            //public string ChrType { get; set; }
            //public string Model { get; set; }

        }
        public class FeedCount
        {
            public string ChrItemID { get; set; }
            public decimal decquantity { get; set; }
            public string datInOutDate { get; set; }
            public string chrLot { get; set; }
        }

        public class MonthCheckProd
        {
            //public string chrBatchID { get; set; }
            //public string chrType { get; set; }
            //public decimal decCheckNum { get; set; }
            //public string datDate { get; set; }
            public decimal LiuTongAmount { get;set;}
            public decimal ZhengpinAmount { get;set;}
            public decimal MonthCheckAmount { get; set; }

        }

        public class dTempStoreBalance
        {
            public string chrLot { get; set; }
            public string ChrItemID { get; set; }
            public decimal DecQtyNal { get; set; }
        }

       public class ZhuanZhengpin
        {
            public decimal decChangeInQnty { get; set; }
        }

        public class tm_dTempStoreIO
        {
            public decimal decSumQty { get; set; }
        }
    }
}
