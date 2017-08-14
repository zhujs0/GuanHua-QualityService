using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Domain;
using Dapper;
using System.Linq;

namespace QualityRepository
{
    public class DllKQ
    {
        private SqlConnection _con { get; set; }
        public DllKQ(SqlConnection con)
        {
            _con = con;
        }
        public KQ GetUserById(string EMPLOYEEID)
        {
            string strsql = @"select EMPLOYEEID,NAME from Employee where  EMPLOYEEID=@EMPLOYEEID";
            return _con.Query<KQ>(strsql, new { EMPLOYEEID = EMPLOYEEID }).FirstOrDefault();
        }
    }
}
