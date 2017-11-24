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
        public string Status { get; set; }

        public string ProblemLevel { get; set; }
        public string ProductClass { get; set; }

        public int OrderType { get; set; }

        public string Measure { get; set; }
        public string Report { get; set; }

        public Guid ID { get; set; }

        public int ProvalStatus { get; set; }

        public string TechnologistMembers { get; set; }
        public string EmployeeID { get; set; }
        public string StepID { get; set; }
        public string StepName { get; set; }
        public int Sort { get; set; }
        public int PrevStatus { get; set; }

        public string IsControl { get; set; }
    }
}
