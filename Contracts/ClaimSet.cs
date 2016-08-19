using System.Collections.Generic;

namespace Contracts
{
    public class ClaimSet
    {
        public string ServiceName { get; set; }
        public Dictionary<string, string> Claims { get; set; }
        public ClaimSet InnerClaimSet { get; set; }
    }
}
