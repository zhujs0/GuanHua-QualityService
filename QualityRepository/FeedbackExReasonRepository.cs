using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using Domain;

namespace QualityRepository
{
    public class FeedbackExReasonRepository
    {
        private SqlConnection _sqlconnnect { get; set; }
        public FeedbackExReasonRepository(SqlConnection sqlconnect)
        {
            _sqlconnnect = sqlconnect;
        }

        public bool AddReason(FeedbackExReason Reason,SqlTransaction tran)
        {
            string strsql = @"insert into ZL_FeedbackExReason (ReasonType,ReasonDetails,OrderNo) 
 values(@ReasonType,@ReasonDetails,@OrderNo)";
            int iResult = _sqlconnnect.Execute(strsql, Reason, tran);
            if(iResult>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<FeedbackExReason> GetReasonByWhere(string strWhere)
        {
            string strsql = @"select * from ZL_FeedbackExReason " + strWhere;
            return _sqlconnnect.Query<FeedbackExReason>(strsql).AsList();
        }

        public bool UpdateReasonInfo(FeedbackExReason ReasonInfo,SqlTransaction tran)
        {
            string strsql = @"update ZL_FeedbackExReason set ReasonType=@ReasonType,ReasonDetails=@ReasonDetails where 
 ReasonID=@ReasonID";
            if(_sqlconnnect.Execute(strsql,ReasonInfo,tran)>0)
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
