using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{ 
    public partial class MobilPoliceHasar
    {
        public int Id { get; set; }
        public string KimlikNo { get; set; }
        public int? AcenteUnvani { get; set; }
        public string SirketKodu { get; set; }
        public int? BransKodu { get; set; }
        public string PoliceNumarasi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string Plaka { get; set; }
        public string RuhsatSeriKodu { get; set; }
        public string RuhsatSeriNo { get; set; }
        public string AsbisNo { get; set; }
        public int? ModelYili { get; set; }
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public string AracKullanimTarzi { get; set; }
        public string IlKodu { get; set; }
        public int? IlceKodu { get; set; }
        public byte? BinaYapiTarzi { get; set; }
        public byte? BinaYapimYili { get; set; }
        public byte? BinaKatSayisi { get; set; }
        public int? DaireBrut { get; set; }
        public string Adres { get; set; }
        public decimal? EsyaBedeli { get; set; }
        public decimal? BinaBedeli { get; set; }
        public string Meslek { get; set; }
        public DateTime? SeyahatGidisTarihi { get; set; }
        public DateTime? SeyahatDonusTarihi { get; set; }
        public string SeyahatUlkeKodu { get; set; }
        public int? SeyahatEdenKisiSayisi { get; set; }
        public string Aciklama { get; set; }
        public byte TeklifPolice { get; set; }
        public int? YenilemeNo { get; set; }
        public int? YenilemeIslemNo { get; set; }
        public int? BinaKullanimSekli { get; set; }
        public int? TeklifIslemNo { get; set; }
        public string KoordinatX { get; set; }
        public string KoordinatY { get; set; }
        public string PoliceAciklama { get; set; }
        public string Dosya { get; set; }
    }

}
