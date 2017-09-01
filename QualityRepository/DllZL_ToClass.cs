using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Domain;
using System.Data.SqlClient;

namespace QualityRepository
{
    public class DllZL_ToClass
    {
        private SqlConnection _con { get; set; }
        public DllZL_ToClass(SqlConnection con)
        {
            _con = con;
        }
        public List<ToClass> GetClass(string strWhere)
        {
            string strSql = @"select * from ZL_ToClass " + strWhere;
            return _con.Query<ToClass>(strSql).AsList();
        }
    }
}
