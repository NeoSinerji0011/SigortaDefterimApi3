using System;
using System.Collections.Generic;

namespace API.Areas.MobilApi.Models.Database
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string Adsoyad { get; set; }
        public string Adres { get; set; }
        public string Tc { get; set; }
        public string Tc_Es { get; set; }
        public string Tc_Cocuk { get; set; }
        public string Tc_Diger { get; set; }
        public string Eposta { get; set; }
        public string Telefon { get; set; }
        public string gsm_1 { get; set; }
        public string gsm_2 { get; set; }
        public string Sifre { get; set; }
        public string Resim { get; set; }
        public string Guvenlik { get; set; }
        public string Onaykodu { get; set; }
        public string Durum { get; set; }
    }
}
