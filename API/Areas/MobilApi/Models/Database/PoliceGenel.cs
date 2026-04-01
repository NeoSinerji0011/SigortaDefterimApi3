using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class PoliceGenel
    {
        [Key]
        public int PoliceId { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public Nullable<int> TUMKodu { get; set; }
        public Nullable<int> UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public string TUMBransKodu { get; set; }
        public string TUMBransAdi { get; set; }
        public string TUMBirlikKodu { get; set; }
        public string PoliceNumarasi { get; set; }
        public Nullable<int> EkNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; } 
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }  


    }
}
