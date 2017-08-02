using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class FeedbackBase
    {
        public string OrderNo {get;set;}
        public string WorkProcedure { get; set; }
        public string BatchNo { get; set; }
        public string Model { get; set; }
        public string Qty { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public string FeedbackMan { get; set; }
        public DateTime FeedbackTime { get; set; }
    }
}
