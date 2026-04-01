using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class MobilMessageOturum
    {
        [Key]
        public int Id { get; set; }
        public int PoliceId { get; set; }
        public byte PoliceTip { get; set; }//0 teklif , 1: hasar
        public int KullaniciId { get; set; }  
        public Nullable<int> AcenteKodu { get; set; }
        public string SirketKodu { get; set; }
        public string Token { get; set; } 
        public Nullable<DateTime> Cevap_Suresi { get; set; } 
        public Nullable<TimeSpan> EkSure { get; set; } 
    }
}
