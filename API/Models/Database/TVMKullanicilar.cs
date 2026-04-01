using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Database
{
    public class TVMKullanicilar
    {
        [Key]
        public int KullaniciKodu { get; set; }
        public int TVMKodu { get; set; }
        public byte Gorevi { get; set; }
        public int YetkiGrubu { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TCKN { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string CepTelefon { get; set; }
        public DateTime SifreGondermeTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public string Sifre { get; set; }
        public DateTime SifreTarihi { get; set; }
        public byte SifreDurumKodu { get; set; }
        public int HataliSifreGirisSayisi { get; set; }
        public DateTime HataliSifreGirisTarihi { get; set; }
        public int DepartmanKodu { get; set; }
        public int YoneticiKodu { get; set; }
        public string MTKodu { get; set; }
        public byte TeklifPoliceUretimi { get; set; }
        public string TeknikPersonelKodu { get; set; }
        public byte Durum { get; set; }
        public string EmailOnayKodu { get; set; }
        public string FotografURL { get; set; }
        public DateTime SonGirisTarihi { get; set; }
        public String SkypeNumara { get; set; }
        public bool AYPmi { get; set; }
        public string MobilDogrulamaKodu { get; set; }
        public string MobilDogrulamaOnaylandiMi { get; set; }

    }
}
