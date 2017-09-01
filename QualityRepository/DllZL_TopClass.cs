using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Domain;
using System.Data.SqlClient;

namespace QualityRepository
{
    public class DllZL_TopClass
    {
        private SqlConnection _con { get; set; }
        public DllZL_TopClass(SqlConnection con)
        {
            _con = con;
        }
        public List<ZL_TopClass> GetByWhere(string strWhere)
        {
            string strSql = @"select * from ZL_TopClass " + strWhere;
            return _con.Query<ZL_TopClass>(strSql).AsList();
        }
    }
}
