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
        public string RoomName { get; set; }

        public string TypeName { get; set; }
        public string Problem { get; set; }
        public string Present { get; set; }
        public string TopClass { get; set; }
        public string QualityClass { get; set; }
        public string ProblemLevel { get; set; }


    }
}
