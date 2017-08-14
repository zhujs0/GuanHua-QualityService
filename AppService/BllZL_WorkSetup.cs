using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using QualityRepository;
using Domain;

namespace AppService
{
    public class BllZL_WorkSetup
    {
        private DllZL_WorkSetup _con { get; set; }
        public BllZL_WorkSetup(SqlConnection con)
        {
            _con = new DllZL_WorkSetup(con);
        }

        public List<ZL_WorkSetup> GetWork(string strWhere)
        {
            return _con.GetWork(strWhere);
        }
    }
}
