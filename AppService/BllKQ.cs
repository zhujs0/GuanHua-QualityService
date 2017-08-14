using Domain;
using QualityRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppService
{
    public class BllKQ
    {
        private DllKQ _con { get; set; }
        public BllKQ(SqlConnection con)
        {
            _con = new DllKQ(con);
        }
        public KQ GetUserById(string EMPLOYEEID)
        {
            return _con.GetUserById(EMPLOYEEID);
        }
    }
}
