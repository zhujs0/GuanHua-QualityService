using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QualityWebApi.Filter.Domain
{
    public class Audience
    {
        [Key]
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string Base64Secret { get; set; }
        public string Issuer { get; set; }
    }
}
