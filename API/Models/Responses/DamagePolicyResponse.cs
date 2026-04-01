using API.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class DamagePolicyResponse
    {
        public List<MobilDamagePolicyResponse> mobilPoliceHasarList { get; set; }
        public List<MobilPoliceDosya> mobilPoliceDosyaList { get; set; }

    }
}
