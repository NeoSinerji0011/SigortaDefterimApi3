using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Inputs.Car
{
    public class GetAracTipInput
    {
        [DefaultValue("003")]
        public string MarkaKodu { get; set; }
    }
}
