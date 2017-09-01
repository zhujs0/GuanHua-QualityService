using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using QualityRepository;
using Domain;

namespace AppService
{
    public class BllApprovalStream
    {
        private DllApprovalStream Dll { get; set; }

        public BllApprovalStream(SqlConnection con)
        {
            Dll = new DllApprovalStream(con);
        }

        public List<ApprovalStream> GetStream(string strWhere)
        {
            return Dll.GetStream(strWhere);
        }
    }
}
