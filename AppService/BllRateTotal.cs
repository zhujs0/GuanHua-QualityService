using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Domain;
using QualityRepository;

namespace AppService
{
    public class BllRateTotal
    {
        private DllRateTotal Dll { get; set; }
        public BllRateTotal(SqlConnection con)
        {
            Dll = new DllRateTotal(con);
        }
        public List<RateTotal.tm_item> GetTm_item()
        {
            return Dll.GetTm_item();
        }

        public RateTotal.PutCount GetPut(string strWhere)
        {
            return Dll.GetPut(strWhere);
        }

        public RateTotal.FeedCount GetFeedCount(string strWhere)
        {
            return Dll.GetFeedCount(strWhere);
        }

        public RateTotal.MonthCheckProd GetMonthCheckProd(string strWhere,string strWhere2)
        {
            return Dll.GetMonthCheckProd(strWhere, strWhere2);
        }

        public List<RateTotal.dTempStoreBalance> GetdTempStoreBalance(string strWhere)
        {
            return Dll.GetdTempStoreBalance(strWhere);
        }

        public RateTotal.ZhuanZhengpin GetZhuanZhengpin(string strWhere)
        {
            return Dll.GetZhuanZhengpin(strWhere);
        }

        public decimal Get_WarehouseStorage(string strWhere)
        {
            return Dll.Get_WarehouseStorage(strWhere);
        }

        public decimal Get_Feeding(string strWhere)
        {
            return Dll.Get_Feeding(strWhere);
        }

        public decimal Get_CirculationStock(string strWhere)
        {
            return Dll.Get_CirculationStock(strWhere);
        }

        public decimal Get_OutInOfStorage(string strWhere)
        {
            return Dll.Get_OutInOfStorage(strWhere);
        }

        public bool InsertToStorage(string CreateTime, SqlTransaction tran)
        {
            return Dll.InsertToStorage(CreateTime, tran);
        }
        public bool InsertToOutInOfStorage(string CreateTime, SqlTransaction tran)
        {
            return Dll.InsertToOutInOfStorage(CreateTime, tran);
        }
        public bool InsertToFeeding(string CreateTime, SqlTransaction tran)
        {
            return Dll.InsertToFeeding(CreateTime, tran);
        }
        public bool InsertToCirculationStock(string CreateTime, SqlTransaction tran)
        {
            return Dll.InsertToCirculationStock(CreateTime, tran);
        }
    }
}
