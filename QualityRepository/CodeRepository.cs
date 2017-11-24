using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Domain;
using Dapper;

namespace QualityRepository
{
    public class CodeRepository
    {
        private SqlConnection _sqlconnnect { get; set; }
        public CodeRepository(SqlConnection sqlconnect)
        {
            _sqlconnnect = sqlconnect;
        }

        /// <summary>
        /// 根据条件获取编码
        /// </summary>
        /// <param name="strsql">条件查询（Demo：where CodeID=1）</param>
        /// <returns></returns>
        public List<Code> GetCodeByWhere(string strsql)
        {
            string sql= @"select * from dbo.ZL_QualityCode  " + strsql;

//            string sql = @"select dbo.QualityCode.CodeString,CodeID,RoomName,RoomCode,TypeName,
//TypeCode,Problem,ProCode,Present,PreCode,Suggestion,SuggestionID from dbo.QualityCode left join dbo.Suggestion 
//on dbo.Suggestion.CodeString=dbo.QualityCode.CodeString " + strsql;
            return _sqlconnnect.Query<Code>(sql).AsList();
        }

        public bool AddCode(Code code)
        {
            string strSql = @"
delete from dbo.ZL_QualityCode where CodeID=@CodeID;
insert into dbo.ZL_QualityCode(CodeString,RoomName,RoomCode,
            TypeName,TypeCode,Problem,ProCode,Present,PreCode,Suggestion,QualityClass,ProblemLevel,TopClass,TopClassCode) values(@CodeString,@RoomName,@RoomCode,
            @TypeName,@TypeCode,@Problem,@ProCode,@Present,@PreCode,@Suggestion,@QualityClass,@ProblemLevel,@TopClass,@TopClassCode)";
            if(_sqlconnnect.Execute(strSql,code)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateCode(Code code)
        {
            string strsql = @"update dbo.ZL_QualityCode set CodeString=@CodeString,RoomName=@RoomName,RoomCode=@RoomCode,
            TypeName=@TypeName,TypeCode=@TypeCode,Problem=@Problem,ProCode=@ProCode,Present=@Present,PreCode=@PreCode
 where CodeID=@CodeID";
            if(_sqlconnnect.Execute(strsql,code)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DelCode(string CodeString,SqlTransaction transaction)
        {
            string strsql = "delete from ZL_QualityCode where CodeString=@CodeString";
            if(_sqlconnnect.Execute(strsql,new {CodeString= CodeString}, transaction) >0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public List<Code> GetCodeByWhereOnPage(string strWhere,int PageIndex,int PageSize,out int RowCount)
        {
            RowCount = 0;
            string strSql = @"select * from (select *, ROW_NUMBER() OVER (ORDER BY CodeID desc) AS RowNumber 
from dbo.ZL_QualityCode "+ strWhere + ") as t where t.RowNumber>@PageSize*(@PageIndex-1) and RowNumber<=@PageSize*@PageIndex";
            string GetCountSql = @"select 1 from ZL_QualityCode " + strWhere;
            RowCount = _sqlconnnect.Query<Code>(GetCountSql).AsList().Count;
            return _sqlconnnect.Query<Code>(strSql,new { PageIndex = PageIndex ,PageSize=PageSize}).AsList();
        }


    }
}
