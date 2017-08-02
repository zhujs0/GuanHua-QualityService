using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
   public class FeedbackExProblem
    {
        public long ProblemID { get; set; }
        public string CodeString { get; set; }
        public string ProblemDetails { get; set; }
        public string PicturePath { get; set; }
        public string OrderNo { get; set; }
        public string Suggestion { get; set; }

    }
}
