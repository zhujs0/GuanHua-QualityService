using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Domain;
using QualityRepository;

namespace AppService
{
    public class FeedbackExHandleService
    {
        private FeedbackExHandleRepository _repository;
        public FeedbackExHandleService(SqlConnection sqlconnection)
        {
            _repository = new FeedbackExHandleRepository(sqlconnection);
        }
        public bool AddHandle(FeedbackExHandle handle, SqlTransaction transaction)
        {
            return _repository.AddHandle(handle, transaction);
        }

        public List<FeedbackExHandle> GetHandler(string strWhere)
        {
            return _repository.GetHandler(strWhere);
        }
    }
}
