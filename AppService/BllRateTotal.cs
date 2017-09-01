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
    }
}
