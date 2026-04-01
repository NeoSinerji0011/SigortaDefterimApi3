using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Inputs.Policy
{
    public class PdfPolicy
    {
        public string KimlikNo { get; set; }
        public bool isAcente{ get; set; }
        public string AdSoyad { get; set; }
        public string AcenteUnvani { get; set; }
        public string AcenteLogo { get; set; }
        public string AcenteMail { get; set; }
        public string Sirket { get; set; }
        public string SirketLogo { get; set; }
        public string SirketMail { get; set; }
        public string BransAdi { get; set; }
        public int BransKodu { get; set; }
        public string PoliceNumarasi { get; set; }
        public int PoliceId { get; set; }
        public string Token { get; set; }
        public string BaslangicTarihi { get; set; }
        public string BitisTarihi { get; set; }
        public string Plaka { get; set; }
        public string RuhsatSeriKodu { get; set; }
        public string RuhsatSeriNo { get; set; }
        public string AsbisNo { get; set; }
        public int ModelYili { get; set; }
        public string Marka { get; set; }
        public string Tip { get; set; }
        public string AracKullanimTarzi { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public byte BinaYapiTarzi { get; set; }
        public byte BinaYapimYili { get; set; }
        public byte BinaKatSayisi { get; set; }
        public int DaireBrut { get; set; }
        public string Adres { get; set; }
        public decimal EsyaBedeli { get; set; }
        public decimal BinaBedeli { get; set; }
        public string Meslek { get; set; }
        public string SeyahatGidisTarihi { get; set; }
        public string SeyahatDonusTarihi { get; set; }
        public string SeyahatUlke { get; set; }
        public string SeyahatUlkeTipi { get; set; }
        public int SeyahatEdenKisiSayisi { get; set; }
        public string Aciklama { get; set; }
        public byte TeklifPolice { get; set; }
        public int YenilemeNo { get; set; }
        public int YenilemeIslemNo { get; set; }
        public int BinaKullanimSekli { get; set; }
        public int? TeklifIslemNo { get; set; } 
        public string Konum { get; set; }
        public int processType { get; set; }
    }
}
