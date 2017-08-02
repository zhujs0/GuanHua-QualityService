using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QualityWebApi.Filter.Domain
{
    public class Permission
    {
        public string AuthName { get; set; }
        [Key]
        public string AuthId { get; set; }
        public string UserId { get; set; }
    }
}
