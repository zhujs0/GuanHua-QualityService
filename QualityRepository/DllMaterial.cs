using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Domain;
using System.Linq;

namespace QualityRepository
{
    public class DllMaterial
    {
        private SqlConnection _con { get; set; }
        public DllMaterial(SqlConnection con)
        {
            _con = con;
        }
        public List<Material.MStandard> Get(long PageIndex, long PageSize, string strWhere, out long RowCount)
        {
            RowCount = 0;
            string strSql = @"select * from (select * , ROW_NUMBER() OVER (ORDER BY mAutoID desc) AS RowNumber from 
ZL_MaterialStandard " + strWhere + ") as t where RowNumber > @PageSize*(@PageIndex-1) and RowNumber<=@PageSize*@PageIndex";
            string RowSql = @"select count(1) as RowAmount from ZL_MaterialStandard " + strWhere;
            RowCount = _con.Query<Material.MStandard>(RowSql).AsList().FirstOrDefault().RowAmount;
            return _con.Query<Material.MStandard>(strSql, new { PageIndex = PageIndex, PageSize = PageSize }).AsList();
        }

        public bool InsertOrUpdate(Material.MStandard parm,SqlTransaction tran)
        {
            string strSql = @"
 if exists (select 1 from ZL_MaterialStandard where mAutoID<>@mAutoID and chrType=@chrType)
	begin
		raiserror('已存在相同材料记录',11,1)
	end
else
	begin
		if  @mAutoID>0
			begin
				update ZL_MaterialStandard set chrType=@chrType ,d10=@d10,d50=@d50,d90=@d90,
					surfaceArea=@surfaceArea,water=@water,vbDensity=@vbDensity,pdDensity=@pdDensity,
					plDensity=@plDensity,loi=@loi where mAutoID=@mAutoID
			end
		else
			begin
				insert into ZL_MaterialStandard(chrType,d10,d50,d90,surfaceArea,water,vbDensity,pdDensity,plDensity,loi)
				values(@chrType,@d10,@d50,@d90,@surfaceArea,@water,@vbDensity,@pdDensity,@plDensity,@loi)
			end
	end
";
            return _con.Execute(strSql, parm,tran) > 0 ? true : false;
        }
    }
}
