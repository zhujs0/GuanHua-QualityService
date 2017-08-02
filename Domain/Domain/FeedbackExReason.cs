using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class FeedbackExReason
    {
        public long ReasonID { get; set; }
        public string ReasonType { get; set; }
        public string ReasonDetails { get; set; }
        public string OrderNo { get; set; }
    }
}
