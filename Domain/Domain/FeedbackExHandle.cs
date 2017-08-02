using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class FeedbackExHandle
    {
        public long HandleID { get; set; }
        public string HandleMan { get; set; }
        public string HandleSuggestion { get; set; }
        public DateTime HandleTime { get; set; }
        public string OrderNo { get; set; }
        public string HandleNote { get; set; }
        public string QualityClass { get; set; }

    }
}
