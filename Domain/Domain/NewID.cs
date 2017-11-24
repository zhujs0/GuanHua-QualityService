using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class NewID
    {
        public Guid ID { get; set; }
    }

    public class OrderSuggestion
    {
        public string CodeString { get; set; }
        public string Suggestion { get; set; }
        public string OrderNo { get; set; }

    }
}
