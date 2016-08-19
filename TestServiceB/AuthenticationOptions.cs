using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestServiceB
{
    public class AuthenticationOptions
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string Audience { get; set; }

        public string Authority => $"{Instance}{TenantId}";
    }
}
