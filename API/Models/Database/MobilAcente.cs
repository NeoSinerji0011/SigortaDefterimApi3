using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilAcente
    {
        [Key] 
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public byte Durumu { get; set; }
        public string Logo{ get; set; }
        public string YetkiliAdSoyad{ get; set; }
        public string YetkiliCep { get; set; }
        public string YetkiliEmail { get; set; }
        public string BildirimEmail { get; set; }
        public string OfisTel { get; set; }
        public int? Skor1 { get; set; }
        public int? Skor2 { get; set; }
        public int? Skor3 { get; set; }
        public int? Acente_Grup_Kodu { get; set; }
        public string Grup_Adi { get; set; }
        public Nullable<DateTime> Kayit_Tarihi { get; set; }
        public int? Yonlenen_Yenileme_Adedi { get; set; }
        public int? Yonlenen_Teklif_Adedi { get; set; }
    }
}
