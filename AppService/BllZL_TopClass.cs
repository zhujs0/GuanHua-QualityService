using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Domain;
using QualityRepository;

namespace AppService
{
    public class BllZL_TopClass
    {
        private DllZL_TopClass Dal { get; set; }
        public BllZL_TopClass(SqlConnection con)
        {
            Dal = new DllZL_TopClass(con);
        }
        public List<ZL_TopClass> GetByWhere(string strWhere)
        {
            return Dal.GetByWhere(strWhere);
        }
    }
}
