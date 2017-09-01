using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityWebApi.Domain
{
    public class QualityDomain
    {
        public class ReturnData
        {
            public string OrderNo { get; set; }
            public string WorkProcedure { get; set; }
            public string BatchNo { get; set; }
            public string Model { get; set; }
            public string Qty { get; set; }
            public string EquipmentName { get; set; }
            public string EquipmentNo { get; set; }
            public string FeedbackMan { get; set; }
            public string FeedbackTime { get; set; }
            public List<FeedbackExReason> ReasonList { get; set; }
            public List<Problem> ProblemList { get; set; }
            public string Status { get; set; }
            public List<ApprovalStream> ApStream { get; set; }
            public long RowCount { get; set; }
        }

        public class Problem
        {
            public long ProblemID { get; set; }
            public string CodeString { get; set; }
            public string ProblemDetails { get; set; }
            public string PicturePath { get; set; }
            public string OrderNo { get; set; }
            public string Suggestion { get; set; }
            public string QualityClass { get; set; }
            public Code code { get; set; }
        }
    }
}
