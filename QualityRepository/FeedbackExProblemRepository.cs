using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using Domain;


namespace QualityRepository
{
    public class FeedbackExProblemRepository
    {
        private SqlConnection _sqlconnnect { get; set; }
        public FeedbackExProblemRepository(SqlConnection sqlconnect)
        {
            _sqlconnnect = sqlconnect;
        }

        public bool AddProblem(FeedbackExProblem Problem,SqlTransaction tran)
        {
            string strsql = @"insert into ZL_FeedbackExProblem (CodeString,ProblemDetails,PicturePath,OrderNo,Suggestion,RoomName,
TypeName,Problem,Present,TopClass,QualityClass,ProblemLevel) 
 values(@CodeString,@ProblemDetails,@PicturePath,@OrderNo,@Suggestion,@RoomName,
@TypeName,@Problem,@Present,@TopClass,@QualityClass,@ProblemLevel)";
            int iResult = _sqlconnnect.Execute(strsql, Problem, tran);
            if (iResult > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<FeedbackExProblem> GetProblemByWhere(string strWhere)
        {
            string strsql = @"select * from ZL_FeedbackExProblem " + strWhere;
            return _sqlconnnect.Query<FeedbackExProblem>(strsql).AsList();
        }

        public bool UpdateSuggestion(string Suggestion,long ProblemID,SqlTransaction tran)
        {
            string strsql = @"update ZL_FeedbackExProblem set Suggestion=@Suggestion where ProblemID=@ProblemID";
            if (_sqlconnnect.Execute(strsql, new { Suggestion = Suggestion ,ProblemID=ProblemID },tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateProblemInfo(FeedbackExProblem ProblemInfo,SqlTransaction tran)
        {
            string strsql = @"update ZL_FeedbackExProblem set CodeString=@CodeString,ProblemDetails=@ProblemDetails,
PicturePath=@PicturePath,Suggestion=@Suggestion where ProblemID=@ProblemID";
            if(_sqlconnnect.Execute(strsql,ProblemInfo,tran)>0)
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
