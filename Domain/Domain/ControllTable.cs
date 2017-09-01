using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ControllTable
    {
        public long AutoID { get; set; }
        public string ControllNo { get; set; }
        public string OrderNo { get; set; }
        public string ApprovalStream { get; set; }
        public string Measure { get; set; }
        public string Report { get; set; }
        public string ApprovalStatus { get; set; }
        public string BatchNo { get; set; }

    }
}
