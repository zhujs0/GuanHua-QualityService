using Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace QualityRepository
{
    public class DllZL_PersonSetup
    {
        private SqlConnection _con { get; set; }
        public DllZL_PersonSetup(SqlConnection con)
        {
            _con = con;
        }
        public List<ZL_PersonSetup> GetPersonWork(string strWhere)
        {
            string strsql = @"select * from ZL_PersonSetup {0}";
            strsql = string.Format(strsql, strWhere);
            return _con.Query<ZL_PersonSetup>(strsql).AsList();
        }

        public bool AddPerson(ZL_PersonSetup person,SqlTransaction tran)
        {
            string strsql = @"insert into ZL_PersonSetup(EmployeeNo,EmployeeName,WorkProduct,WorkName) 
values(@EmployeeNo,@EmployeeName,@WorkProduct,@WorkName)";
            if(_con.Execute(strsql,person,tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DelPerson(long PersonAutoID,SqlTransaction tran)
        {
            string strsql = @"delete from ZL_PersonSetup where PersonAutoID=@PersonAutoID";
            if(_con.Execute(strsql,new { PersonAutoID = PersonAutoID },tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdatePerson(ZL_PersonSetup person, SqlTransaction tran)
        {
            string strsql = @"update ZL_PersonSetup set EmployeeNo=@EmployeeNo ,EmployeeName=@EmployeeName,WorkProduct=@WorkProduct,
WorkName=@WorkName where PersonAutoID=@PersonAutoID";
            if(_con.Execute(strsql,person,tran)>0)
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
