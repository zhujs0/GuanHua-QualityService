using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using System.Data.SqlClient;
using QualityRepository;

namespace AppService
{
    public class BllMaterial
    {
        private DllMaterial Dll { get; set; }
        public BllMaterial(SqlConnection con)
        {
            Dll = new DllMaterial(con);
        }

        public List<Material.MStandard> Get(long PageIndex, long PageSize, string strWhere, out long RowCount)
        {
            return Dll.Get( PageIndex,  PageSize,  strWhere, out  RowCount);
        }

        public bool InsertOrUpdate(Material.MStandard parm,SqlTransaction tran)
        {
            return Dll.InsertOrUpdate(parm, tran);
        }
    }
}
