using System.ComponentModel.DataAnnotations;
using System;

namespace API.Areas.MobilApi.Models.Database
{
    public class SirketSorulari
    {
        [Key]
        public int Id { get; set; }
        public int? TumKodu { get; set; }
        public string Soru { get; set; }
        public string InputTuru { get; set; } //select,radio,checkbox,text
        public Nullable<DateTime> KayitTarihi { get; set; }
        public string AktifPasif { get; set; }
        public int BransKodu { get; set; }
        public string SoruKodu { get; set; }//ikame_arac , meslek_adi

    }
}
