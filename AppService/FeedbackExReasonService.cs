using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using QualityRepository;
using System.Data.SqlClient;

namespace AppService
{
    public class FeedbackExReasonService
    {
        private FeedbackExReasonRepository _repository { get; set; }

        public FeedbackExReasonService(SqlConnection sqlconnection)
        {
            _repository = new FeedbackExReasonRepository(sqlconnection);
        }

        public bool AddReason(FeedbackExReason Reason, SqlTransaction tran)
        {
            return _repository.AddReason(Reason, tran);
        }

        public List<FeedbackExReason> GetReasonByWhere(string strWhere)
        {
            return _repository.GetReasonByWhere(strWhere);
        }

        public bool UpdateReasonInfo(FeedbackExReason ReasonInfo, SqlTransaction tran)
        {
            return _repository.UpdateReasonInfo(ReasonInfo,tran);
        }
    }
}
