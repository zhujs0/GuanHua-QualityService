using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using System.Data.SqlClient;
using QualityRepository;

namespace AppService
{
    public class BllZL_ToClass
    {
        private DllZL_ToClass Dll { get; set; }
        public BllZL_ToClass(SqlConnection con)
        {
            Dll = new DllZL_ToClass(con);
        }
        public List<ToClass> GetClass(string strWhere)
        {
            return Dll.GetClass(strWhere);
        }
    }
}
