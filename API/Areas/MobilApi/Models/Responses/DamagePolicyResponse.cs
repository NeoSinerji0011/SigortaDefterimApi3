using API.Areas.MobilApi.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Responses
{
    public class DamagePolicyResponse
    {
        public List<MobilDamagePolicyResponse> mobilPoliceHasarList { get; set; }
        public List<MobilPoliceDosya> mobilPoliceDosyaList { get; set; }

    }
}
