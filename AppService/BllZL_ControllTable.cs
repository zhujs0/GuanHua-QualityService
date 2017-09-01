using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Domain;
using QualityRepository;

namespace AppService
{
    public class BllZL_ControllTable
    {
        private DllZL_ControllTable Dll { get; set; }
        public BllZL_ControllTable(SqlConnection con)
        {
            Dll = new DllZL_ControllTable(con);
        }
        public List<ControllTable> GetByWhere(string strWhere)
        {
            return Dll.GetByWhere(strWhere);
        }

        public bool SaveControll(ControllTable controll)
        {
            return Dll.SaveControll(controll);
        }
    }
}
