using System;
using System.Collections.Generic;
using System.Text;
using QualityRepository;
using Domain;
using System.Data.SqlClient;

namespace AppService
{
    public class FeedbackExProblemService
    {
        private FeedbackExProblemRepository _repository { get; set; }

        public FeedbackExProblemService(SqlConnection sqlconnection)
        {
            _repository = new FeedbackExProblemRepository(sqlconnection);
        }

        public bool AddProblem(FeedbackExProblem Problem, SqlTransaction tran)
        {
            return _repository.AddProblem(Problem, tran);
        }
        public List<FeedbackExProblem> GetProblemByWhere(string strWhere)
        {
            return _repository.GetProblemByWhere(strWhere);
        }

        public bool UpdateSuggestion(string Suggestion, long ProblemID, SqlTransaction tran)
        {
            return _repository.UpdateSuggestion(Suggestion, ProblemID, tran);
        }

        public bool UpdateProblemInfo(FeedbackExProblem ProblemInfo, SqlTransaction tran)
        {
            return _repository.UpdateProblemInfo(ProblemInfo, tran);
        }
    }
}
