using API.Areas.MobilApi.Models.Database;
using SigortaDefterimV2API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Responses
{
    public class PolicyResponse
    {
    }
    public class MobilPolicyResponse : MobilTeklifPolice
    {
        public string AcenteAdi { get; set; }
        public string AcenteTelNo { get; set; }
        public string PdfUrl { get; set; }
        public string ManuelMi { get; set; }
    }
    public class MobilDamagePolicyResponse : MobilPoliceHasar
    {
        public string AcenteAdi { get; set; }
        public string AcenteTelNo { get; set; }
        public string PdfUrl { get; set; }

    }
}
