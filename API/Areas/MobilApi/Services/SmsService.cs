using API.Areas.MobilApi.Helper;
using API.Areas.MobilApi.Models;
using API.Areas.MobilApi.Models.Database;
using API.Areas.MobilApi.Models.Input;
using API.Areas.MobilApi.Models.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SigortaDefterimV2API.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Services
{
    public class SmsService
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        JwtSecurityTokenHandler tokenHandler;
        byte[] key;

        public SmsService(DataContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
            tokenHandler = new JwtSecurityTokenHandler();
            key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        }

        public void SmsIcerikYaz(SmsItem smsItem)
        {
            try
            {
                Utils.WriteFile2(smsItem);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog(ex.Message);
                return;
            }

            try
            {
                MobileSmsKaydet(smsItem);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog(ex.ToString());
            }
        }

        public void MobileSmsKaydet(SmsItem smsItem)
        {
            if (smsItem == null)
                return;

            _context.MobileSms.Add(CreateMobileSmsEntity(smsItem));
            _context.SaveChanges();
        }

        private static MobileSms CreateMobileSmsEntity(SmsItem smsItem)
        {
            return new MobileSms
            {
                FromPhone = smsItem.fromPhone ?? "",
                FromPhone2 = smsItem.fromPhone2,
                ToPhone = smsItem.toPhone ?? "",
                Body = smsItem.body ?? "",
                SmsTarihi = smsItem.date != default ? smsItem.date : Utils.getTRDateTime(),
                SirketAdi = smsItem.SirketAdi,
                CurrentTimeData = smsItem.currentTimeData,
                CurrentOldTimeData = smsItem.currentOldTimeData
            };
        }
        public OtoLoginSigortaSirketKullanicilar TvmSmsData(TvmRequest tvmRequest)
        {
            var res = _context.OtoLoginSigortaSirketKullanicilar.Where(x => x.TUMKodu == tvmRequest.TumKodu && x.TVMKodu == tvmRequest.TvmKodu).FirstOrDefault();
            res.SmsKodTelNo = Utils.EncryptRijndael(res.SmsKodTelNo, "!082017?");
            res.SmsKodSecretKey1 = Utils.EncryptRijndael(res.SmsKodSecretKey1, "!082017?");
            res.SmsKodSecretKey2 = Utils.EncryptRijndael(res.SmsKodSecretKey2, "!082017?");
            res.KullaniciAdi = "";
            res.Sifre = "";
            res.Id = -1;
            return res;
        }
        public bool UpdateSecretKeys(OtoLoginSigortaSirketKullanicilar item)
        {
            var data = _context.OtoLoginSigortaSirketKullanicilar.Where(x => x.Id == item.Id).FirstOrDefault();
            data.SmsKodSecretKey1 = item.SmsKodSecretKey1;//googlekey
            data.SmsKodSecretKey2 = item.SmsKodSecretKey2;//qrcodekey
            _context.SaveChanges();
            return true;
        }
        public OtoLoginSigortaSirketKullanicilar SirketData(TvmRequest item)
        { 
            var izinVerilenler = JsonConvert.DeserializeObject<List<int>>(System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\areas\mobilapi\files\other\tumdata.json"));
            if (izinVerilenler.Contains(item.TumKodu))
            {
                var tvmkodu = item.TvmKodu;
                var tumkodu = item.TumKodu;

                var res = _context.OtoLoginSigortaSirketKullanicilar.Where(a => a.TVMKodu == tvmkodu && a.TUMKodu == tumkodu).FirstOrDefault();
                res.Id = -1;
                return res;
            }
            else return null;
           
        }
        public List<OtoLoginSigortaSirketKullanicilar> SirketKontrol(int tvmkodu)
        {
            var res = _context.OtoLoginSigortaSirketKullanicilar.Where(a => a.TVMKodu == tvmkodu).ToList();
            res = res.Where((x) =>
            {
                if (x.Sifre == null || x.KullaniciAdi == null)
                    return false;
                return true;
            }).Where((x) =>
            {
                if (x.Sifre.Trim() == "" || x.KullaniciAdi.Trim() == "")
                    return false;
                return true;
            }).Select(x => new OtoLoginSigortaSirketKullanicilar { TUMKodu = x.TUMKodu, SigortaSirketAdi = x.SigortaSirketAdi }).OrderBy(x => x.SigortaSirketAdi).ToList();
            var izinVerilenler = JsonConvert.DeserializeObject<List<int>>(System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\areas\mobilapi\files\other\tumdata.json"));
            res = res.Where(x => izinVerilenler.Contains(x.TUMKodu)).ToList();
            return res;
        } 
        public object TvmKontrol()
        {
            var tvmlist = JsonConvert.DeserializeObject<List<int>>(System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\areas\mobilapi\files\other\tvmdata.json"));
            //List<int> tvmlist = new List<int> { 132, 189, 188, 192, 191, 170, 193, 194};
            var res = _context.TVMDetay.Where(a => tvmlist.Contains(a.Kodu)).Select(x => new  { Unvani = x.Unvani, Kodu = x.Kodu }).ToList();
            return res;
        }
        public object SirketSorulari(int TvmKodu)
        {
            var res = _context.OtoLoginSigortaSirketKullanicilar.Where(a => a.TVMKodu == TvmKodu).ToList();
            var res1 = res.Where((x) =>
            {
                if (x.Sifre == null || x.KullaniciAdi == null)
                    return false;
                return true;
            }).Where((x) =>
            {
                if (x.Sifre.Trim() == "" || x.KullaniciAdi.Trim() == "")
                    return false;
                return true;
            }).Select(x => x.TUMKodu).Distinct().ToList();

            SirketVerileriResponseItem sirketVerileriResponseItem = null;
            SirketSorularResponseItem sirketSorularResponse = null; 
            List<SirketVerileriResponseItem> sirketVerileriResponse = new List<SirketVerileriResponseItem>(); 
            var tempss1 = _context.SirketSorulari.Where(x => res1.Contains(x.TumKodu.Value)).Distinct().ToList();


            foreach (var item in tempss1)
            {
                sirketVerileriResponseItem = new SirketVerileriResponseItem();
                sirketVerileriResponseItem.AktifPasif = item.AktifPasif;
                sirketVerileriResponseItem.TumAdi = TumAdi(item.TumKodu.Value);
                sirketVerileriResponseItem.TumKodu = item.TumKodu;
                sirketVerileriResponseItem.BransKodu = item.BransKodu;
                sirketVerileriResponseItem.KayitTarihi = item.KayitTarihi;
                sirketVerileriResponseItem.SirketSorulari = new List<SirketSorularResponseItem>();
                sirketSorularResponse = new SirketSorularResponseItem();
                sirketSorularResponse.InputTuru = item.InputTuru;
                sirketSorularResponse.SoruKodu = item.SoruKodu;
                sirketSorularResponse.Soru = item.Soru;
                sirketSorularResponse.SirketSoruDegerleri = _context.SirketSoruDegerleri.Where(x => x.SirketSorulariId == item.Id).ToList();

                sirketVerileriResponseItem.SirketSorulari.Add(sirketSorularResponse);
                if (sirketVerileriResponse.Where(x => x.TumKodu == item.TumKodu).FirstOrDefault() == null)
                {
                    sirketVerileriResponse.Add(sirketVerileriResponseItem);
                }
                else
                {
                    var temp = sirketVerileriResponse.Where(x => x.TumKodu == item.TumKodu).FirstOrDefault();
                    temp.SirketSorulari.Add(sirketSorularResponse);
                }
            }
            sirketVerileriResponse = sirketVerileriResponse.OrderBy(x => x.TumKodu).ToList();
            return sirketVerileriResponse;
        }
        public TVMKullanicilar TVMKullaniciByTvmKodu(int tvmkodu)
        {
            var res = _context.TVMKullanicilar.Where(a => a.TVMKodu == tvmkodu).FirstOrDefault();
            return res;
        }
        public object AracMarka()
        {
            var res = _context.AracMarka.Select(x => new { id = x.MarkaKodu, text = x.MarkaAdi }).ToList();
            return res;
        }
        public object AracTipByMarkaKodu(string markaKodu)
        {
            var res = _context.AracTip.Where(x => x.MarkaKodu == markaKodu).Select(x => new { id = x.TipKodu, text = x.TipAdi }).ToList();
            return res;
        }
        string TumAdi(int tumkodu)
        {
            var res = _context.TUMDetay.Where(x => x.Kodu == tumkodu).FirstOrDefault();
            if (res != null)
            {
                return res.Unvani;
            }
            else
                return "-";
        }
        public TVMDetay TVMDetaybyTvmKodu(int tvmkodu)
        {
            var res = _context.TVMDetay.Where(a => a.Kodu == tvmkodu).FirstOrDefault();
            return res;
        }
       
    }
}
