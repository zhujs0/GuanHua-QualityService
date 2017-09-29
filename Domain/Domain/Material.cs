using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Material
    {
        public class MStandard
        {
            public long mAutoID { get; set; }
            public string material { get; set; }
            public string chrType { get; set; }
            public decimal d10 { get; set; }
            public decimal d50 { get; set; }
            public decimal d90 { get; set; }
            public decimal surfaceArea { get; set; }
            public decimal water { get; set; }
            public decimal vbDensity { get; set; }
            public decimal pdDensity { get; set; }
            public decimal plDensity { get; set; }
            public decimal loi { get; set; }
            public int RowAmount { get; set; }
        }
    }
}
