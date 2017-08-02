using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Domain;
using Dapper;

namespace QualityRepository
{
    public class FeedbackExHandleRepository
    {
        private SqlConnection _sqlconnection;
        public FeedbackExHandleRepository(SqlConnection sqlconnection)
        {
            _sqlconnection = sqlconnection;
        }

        public bool AddHandle(FeedbackExHandle handle, SqlTransaction transaction)
        {
            string strsql = @"insert into ZL_FeedbackExHandle(HandleMan,HandleSuggestion,HandleTime,OrderNo,HandleNote,QualityClass)
 values(@HandleMan,@HandleSuggestion,@HandleTime,@OrderNo,@HandleNote,@QualityClass)";
            if(_sqlconnection.Execute(strsql,handle,transaction)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<FeedbackExHandle> GetHandler(string strWhere)
        {
            string strsql = "select * from ZL_FeedbackExHandle " + strWhere;
            return _sqlconnection.Query<FeedbackExHandle>(strsql).AsList();
        }
    }
}
