using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilIletisim
    {
        [Key]
        public int Id { get; set; }
        public int PoliceId { get; set; }
        public byte PoliceTip { get; set; }//0 teklif , 1: hasar
        public int KullaniciId { get; set; }
        public string Kullanici_Mesaj { get; set; }
        public DateTime Tarih_Saat{ get; set; }
        public Nullable<int> AcenteKodu  { get; set; }
        public string SirketKodu  { get; set; }
        public string Token { get; set; }
        public string Firma_Mesaji { get; set; }
        public Nullable<DateTime> Cevap_Suresi { get; set; }
        public Nullable<DateTime> Cevap_Tarihi { get; set; }
        public Nullable<TimeSpan> EkSure { get; set; }
        public byte Kullanici_Gordumu { get; set; }
        public string Urun_Plaka_Sehir { get; set; }
       
         

    }
}
