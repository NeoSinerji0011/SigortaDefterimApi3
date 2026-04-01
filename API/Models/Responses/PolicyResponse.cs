using API.Models.Database; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
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
