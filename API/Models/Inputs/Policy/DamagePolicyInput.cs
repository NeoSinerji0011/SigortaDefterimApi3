using API.Models.Database;
using SigortaDefterimV2API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Inputs.Policy
{
    public class PolicyInput
    {
        public int Id { get; set; }
        [DefaultValue("32132132132")]
        public string KimlikNo { get; set; }
        public int? AcenteUnvani { get; set; }
        public string SirketKodu { get; set; }
        [DefaultValue(1)]
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
    }
    public class DamagePolicyInput:PolicyInput
    {
        
        public string KoordinatX { get; set; }
        public string KoordinatY { get; set; } 
        public byte KullaniciGonderiTuru { get; set; }//acentesi olmayan kayıtları için ilgili şirkete telefon veya mail ile gönderim türü -- 1:mail,2:telefon
        public List<string> SesDosyaList { get; set; }
        public List<string> ResimDosyaList { get; set; }
    }

    public class AddDamagePolicyTemp
    {
        public MobilPoliceHasar MobilPoliceHasar = new MobilPoliceHasar();
        public MobilTeklifPolice MobilTeklifPolice = new MobilTeklifPolice();
        public List<MobilPoliceDosya> mobilPoliceDosyaList = new List<MobilPoliceDosya>(); 
        
    }
    public partial class MobilTeklifPoliceInput: DamagePolicyInput
    {  
        //
        // Özet:
        //      0:kendi acenteme,1:diğer acentelere,2:her ikisine gönder
        //
        // Döndürülenler:
        public byte EmailType { get; set; }
        public int? KullaniciId { get; set; }


    }
}
