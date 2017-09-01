using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ApprovalStream
    {
        public long AutoID { get; set; }
        public string ManPosition { get; set; }
        public string Man { get; set; }
        public string HandlingSuggestion { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string ToClass { get; set; }
        public string OrderNo { get; set; }
    }
}
