using Domain;
using QualityRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppService
{
    public class BllZL_PersonSetup
    {
        private DllZL_PersonSetup _con { get; set; }
        public BllZL_PersonSetup(SqlConnection con)
        {
            _con = new DllZL_PersonSetup(con);
        }

        public List<ZL_PersonSetup> GetPersonWork(string strWhere)
        {
            return _con.GetPersonWork(strWhere);
        }
        public bool AddPerson(ZL_PersonSetup person, SqlTransaction tran)
        {
            return _con.AddPerson(person, tran);
        }
        public bool DelPerson(long PersonAutoID, SqlTransaction tran)
        {
            return _con.DelPerson(PersonAutoID, tran);
        }
        public bool UpdatePerson(ZL_PersonSetup person, SqlTransaction tran)
        {
            return _con.UpdatePerson(person, tran);
        }
    }
}
