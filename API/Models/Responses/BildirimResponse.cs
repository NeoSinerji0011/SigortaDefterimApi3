using API.Models.Database;
using SigortaDefterimV2API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class BildirimResponse
    {
       public List<MobilTeklifPolice> bildirimPoliceList { get; set; }
        public List<Bildirim> bildirimDisableList { get; set; }
    }
}
