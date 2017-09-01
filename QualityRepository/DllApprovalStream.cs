using Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace QualityRepository
{
    public class DllApprovalStream
    {
        private SqlConnection _con { get; set; }
        public DllApprovalStream(SqlConnection con)
        {
            _con = con;
        }

        public List<ApprovalStream> GetStream(string strWhere)
        {
            string strSql = @"select * from ZL_ApprovalStream " + strWhere;
            return _con.Query<ApprovalStream>(strSql).AsList();
        }
    }
}
