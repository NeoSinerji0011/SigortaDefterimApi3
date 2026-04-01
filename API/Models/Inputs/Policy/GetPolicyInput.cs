using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Inputs
{
    public class GetPolicyInput
    {
        [DefaultValue("14")]
        public int PolicyId { get; set; }
    }
}
