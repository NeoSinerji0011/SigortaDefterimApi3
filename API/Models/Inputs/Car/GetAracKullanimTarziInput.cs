using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Inputs.Car
{
    public class GetAracKullanimTarziInput
    {
        [DefaultValue(0)]
        public Int16 KullanimSekliKodu { get; set; }
    }
}
