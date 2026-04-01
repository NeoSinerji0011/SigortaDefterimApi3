using API.Areas.MobilApi.Models.Database; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Responses
{
    public class BildirimResponse
    {
       public List<MobilTeklifPolice> bildirimPoliceList { get; set; }
        public List<Bildirim> bildirimDisableList { get; set; }
    }
}
