using SigortaDefterimV2API.Models.Inputs;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Examples
{
    public class UpdateCarInfoExample : IExamplesProvider<UpdateCarInfoInput>
    {
        public UpdateCarInfoInput GetExamples()
        {
            return new UpdateCarInfoInput
            {
                PolicyId = 13,
                AracKullanimTarzi = "111+10",
                MarkaKodu = "061",
                ModelYili = 2010,
                TipKodu = "096"
            };
        }
    }
}
