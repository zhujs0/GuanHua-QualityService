using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Domain;

namespace QualityRepository
{
    public class DllZL_WorkSetup
    {
        private SqlConnection _con { get; set; }
        public DllZL_WorkSetup(SqlConnection con)
        {
            _con = con;
        }

        public List<ZL_WorkSetup> GetWork(string strWhere)
        {
            string strsql = @"select * from ZL_WorkSetup {0}" ;
            strsql = string.Format(strsql, strWhere);
            return _con.Query<ZL_WorkSetup>(strsql).AsList();
        }
    }
}
