using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using Domain;

namespace QualityRepository
{
    public class DllZL_ControllTable
    {
        private SqlConnection _con { get; set; }
        public DllZL_ControllTable(SqlConnection con)
        {
            _con = con;
        }
        public List<ControllTable> GetByWhere(string strWhere)
        {
            string strSql = @"select * from ZL_ControllTable " + strWhere;
            return _con.Query<ControllTable>(strSql).AsList();
        }

        public bool SaveControll(ControllTable controll)
        {
            string strSql = @"update ZL_ControllTable set Measure=@Measure,Report=@Report where BatchNo=@BatchNo 
 and OrderNo=@OrderNo";
            if(_con.Execute(strSql,controll)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
