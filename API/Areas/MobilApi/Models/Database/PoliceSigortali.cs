using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class PoliceSigortali
    {
        [Key]
        public int PoliceId { get; set; }
        public string KimlikNo { get; set; }
        public string VergiKimlikNo { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string IlKodu { get; set; }
        public string IlAdi { get; set; }
        public Nullable<int> IlceKodu { get; set; }
        public string IlceAdi { get; set; }
        public string Adres { get; set; }


    }
}
