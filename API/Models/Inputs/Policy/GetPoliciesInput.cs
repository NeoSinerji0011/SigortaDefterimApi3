using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Inputs
{
    public class GetPoliciesInput
    {
        [DefaultValue("32132132132")]
        public string KimlikNo { get; set; }

        [DefaultValue(4)]
        public int BransKodu { get; set; }
    }
    public class NeoConnectInput
    {
        public int SirketKod { get; set; }
        public string SecretKey { get; set; }

    }
}
