using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class CommomDomain
    {
        public class WorkFlowTask
        {
            public int Sort { get; set; }
            public string InstanceID { get; set; }
            public string StepID { get; set; }
            public string StepName { get; set; }
            public int Status { get; set; }
        }
    }
}
