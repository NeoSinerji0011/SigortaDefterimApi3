using API.Models.Database;
using API.Models.Responses;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Services
{
    public class MobileAppService
    {
        private DataContext _context;

        public MobileAppService(DataContext context)
        {
            _context = context;
        }

        public List<MobilKarsilamaEkrani> GetLandingPages()
        {
            return _context.MobilKarsilamaEkrani.Where(landingPage => landingPage.Durum == true).ToList();
        }

        public List<MobilKaydirmaEkrani> GetSliderPages()
        {
            return _context.MobilKaydirmaEkrani.Where(sliderPage => sliderPage.Durum == true).ToList();
        }
        public MobilGlobalDataList GetMobilGlobalList(string KimlikNo)
        {
            MobilGlobalDataList mobilGlobalDataList = new MobilGlobalDataList();
            mobilGlobalDataList.mobilUlkeList = _context.MobilUlke.ToList();
            mobilGlobalDataList.mobilUlkeTipiList = _context.MobilUlkeTipi.ToList();
            mobilGlobalDataList.ilList = _context.Il.ToList();
            mobilGlobalDataList.ilceList = _context.Ilce.ToList();
            mobilGlobalDataList.bransList = _context.Brans.Where(X => X.BransKodu < 24).ToList();
            mobilGlobalDataList.aracKullanimTarziList = _context.AracKullanimTarzi.GroupBy(x => new { x.KullanimTarzi, x.Kod2, x.KullanimTarziKodu }).Select(x => new AracKullanimTarzi { Kod2 = x.Key.Kod2, KullanimTarzi = x.Key.KullanimTarzi, KullanimTarziKodu = x.Key.KullanimTarziKodu, Durum = 0, KullanimSekliKodu = 0 }).ToList();
            mobilGlobalDataList.aracMarkaList = _context.AracMarka.ToList();
            mobilGlobalDataList.meslekList = _context.Meslek.ToList();
            mobilGlobalDataList.sigortaSirketleriList = _context.SigortaSirketleri.ToList();
            mobilGlobalDataList.aracTipList = _context.MobilTeklifPolice.Where(x => x.KimlikNo == KimlikNo).Join(_context.AracTip, mt => new { mt.TipKodu, mt.MarkaKodu }, at => new { at.TipKodu, at.MarkaKodu }, (mt, at) => new AracTip { MarkaKodu = at.MarkaKodu, TipAdi = at.TipAdi, TipKodu = at.TipKodu }).Distinct().ToList();
            var tempAracTipList = (from ps in _context.PoliceSigortali join pa in _context.PoliceArac on ps.PoliceId equals pa.PoliceId join tip in _context.AracTip on new { A = pa.Marka, B = pa.AracinTipiKodu} equals new{A= tip.MarkaKodu,B=tip.TipKodu} where ps.KimlikNo == KimlikNo select new AracTip {MarkaKodu=pa.Marka,TipKodu=pa.AracinTipiKodu,TipAdi=pa.AracinTipiAciklama }).Distinct().ToList();
             
            tempAracTipList = tempAracTipList.Where(x => x.TipAdi != null).ToList();
            mobilGlobalDataList.aracTipList.AddRange(tempAracTipList);
            mobilGlobalDataList.acenteList = _context.PoliceSigortali.Where(x => x.KimlikNo == KimlikNo).Join(_context.PoliceGenel, ps => ps.PoliceId, pg => pg.PoliceId, (ps, pg) => new { pg.TVMKodu }).Distinct().Join(_context.TVMDetay, pg => pg.TVMKodu, tvm => tvm.Kodu, (pg, tvm) => tvm).ToList();
            List<int> tvmkodTemp = new List<int>();

            foreach (var item in mobilGlobalDataList.acenteList)
            {
                tvmkodTemp.Add(item.Kodu);
            }

            var acenteTemp = _context.MobilAcente.Where(x => x.Skor1 == 100 && !tvmkodTemp.Contains(x.TVMKodu)).Select(x => new TVMDetay { Email = x.BildirimEmail, Unvani = x.TVMUnvani, Kodu = x.TVMKodu }).ToList();
            mobilGlobalDataList.acenteList.AddRange(acenteTemp);
            return mobilGlobalDataList;
        }
        public List<AracTip> GetAracTipList(string markaKodu)
        {
            return _context.AracTip.GroupBy(x => new { x.TipAdi, x.TipKodu, x.MarkaKodu }).Select(x => new AracTip { TipKodu = x.Key.TipKodu, TipAdi = x.Key.TipAdi, MarkaKodu = x.Key.MarkaKodu }).Where(x => x.MarkaKodu == markaKodu).ToList();
        }
        public TVMDetay GetTVMDetay(int tvmKodu)
        {
            var result = _context.TVMDetay.Where(x => x.Kodu == tvmKodu).FirstOrDefault();
            return result != null ? result : new TVMDetay();
        }
        public BildirimResponse GetBildirim(string kimlikno)
        {
            BildirimResponse bildirimResponse = new BildirimResponse();
            List<MobilTeklifPolice> policeList = new List<MobilTeklifPolice>();

            List<MobilTeklifPolice> policeListTemp = _context.MobilTeklifPolice.Where(x => x.KimlikNo == kimlikno && x.TeklifPolice == 0).Select(x => new MobilTeklifPolice { BransKodu = x.BransKodu, PoliceNumarasi = x.PoliceNumarasi, BitisTarihi = x.BitisTarihi, SeyahatDonusTarihi = x.SeyahatDonusTarihi }).ToList();

            List<MobilTeklifPolice> policeListTemp2 = _context.PoliceSigortali.Where(x => x.KimlikNo == kimlikno).Join(_context.PoliceGenel, ps => ps.PoliceId, pg => pg.PoliceId, (ps, pg) => new MobilTeklifPolice { PoliceNumarasi = pg.PoliceNumarasi, BransKodu = pg.BransKodu, BitisTarihi = pg.BitisTarihi }).ToList();

            policeListTemp.AddRange(policeListTemp2);

            policeListTemp = policeListTemp.GroupBy(x => new { x.PoliceNumarasi, x.BransKodu, x.BitisTarihi, x.SeyahatDonusTarihi }).Select(x => new MobilTeklifPolice { BransKodu = x.Key.BransKodu, PoliceNumarasi = x.Key.PoliceNumarasi, BitisTarihi = x.Key.BitisTarihi, SeyahatDonusTarihi = x.Key.SeyahatDonusTarihi }).ToList();
            foreach (var item in policeListTemp)
            {
                if (DateDiff(item.BitisTarihi != null ? item.BitisTarihi.Value : item.SeyahatDonusTarihi.Value) > -61 && DateDiff(item.BitisTarihi != null ? item.BitisTarihi.Value : item.SeyahatDonusTarihi.Value) < 31)
                {
                    if (item.BransKodu == 21)
                    {
                        item.BitisTarihi = item.SeyahatDonusTarihi;
                    }
                    policeList.Add(item);
                }
            }
            bildirimResponse.bildirimDisableList = _context.Bildirim.Where(x => x.KimlikNo == kimlikno).ToList();
            bildirimResponse.bildirimPoliceList = policeList;

            return bildirimResponse;
        }
        public bool SetBildirimDisable(Bildirim bildirim)
        {
            _context.Bildirim.Add(bildirim);
            int result = _context.SaveChanges();
            return result > 0 ? true : false;
        }
        
        public bool SetUserMessage(MobilMessage message)
        {
            _context.MobilMessage.Add(message);
            int res = _context.SaveChanges();
            return res > -1 ? true : false;
        }
        public bool SetCompanyMessage(MobilMessage message, List<string> fileList)
        {
            _context.MobilMessage.Add(message);
            int res = _context.SaveChanges();
            if (res > -1)
            {
                if (fileList != null)
                    if (fileList.Count > 0)
                    {
                        MobilMessageDosya messageDosya;
                        foreach (var item in fileList)
                        {
                            messageDosya = new MobilMessageDosya();
                            messageDosya.MobilMessageId = message.Id;
                            messageDosya.DosyaUrl = "https://sigortadefterimv2api.azurewebsites.net/CompanyFiles/" + item;
                            messageDosya.DosyaTip = Path.GetExtension(item).Substring(1);
                            _context.MobilMessageDosya.Add(messageDosya);
                        }
                        _context.SaveChanges();
                    }
            }
            return res > -1 ? true : false;
        }
        public object GetCompanyMessage(string token)
        {
          
            var res=(from mo in _context.MobilMessageOturum join mtp in _context.MobilTeklifPolice on mo.PoliceId equals mtp.Id join br in _context.Brans on mtp.BransKodu equals br.BransKodu join ku in _context.Kullanici on mo.KullaniciId equals ku.Id  where mo.Token==token select new IletisimResponse {PoliceNumarasi=mtp.PoliceNumarasi,PoliceId=mtp.Id,TalepNo=mtp.TeklifIslemNo,BrasAdi=br.BransAdi,AdSoyad=ku.Adsoyad}).FirstOrDefault();
            res.MessageList = _context.MobilMessage.Where(x => x.OturumId == res.PoliceId).OrderBy(x=>x.Tarih_Saat).ToList();
            var temp = _context.MobilTeklifPolice.Where(x => x.Id == res.PoliceId).FirstOrDefault();
            switch ((int)temp.BransKodu)
            {
                case 1:
                case 2:
                    res.Urun_Plaka_Sehir = temp != null ? temp.Plaka : "Bulunamadı";
                    break;
                case 4:
                    var mKod = int.Parse(temp.Meslek);
                    var saglik = _context.Meslek.Where(x => x.MeslekKodu == mKod).FirstOrDefault();
                    res.Urun_Plaka_Sehir = saglik != null ? saglik.MeslekAdi : "Bulunamadı";
                    break;
                default:
                    var il = _context.Il.Where(x => x.IlKodu == temp.IlKodu).FirstOrDefault();
                    var ilce = _context.Ilce.Where(x => x.IlceKodu == temp.IlceKodu).FirstOrDefault();
                    var tmp = il != null ? il.IlAdi : "Bulunamadı";
                    tmp += "/";
                    tmp += ilce != null ? ilce.IlceAdi : "";
                    break;
            }
             
            return res;
        }

        public IletisimResponseMobil GetUserMessage(Kullanici kullanici)
        {

            IletisimResponseMobil res = new IletisimResponseMobil();
           
            res.iletisimResponse= (from mo in _context.MobilMessageOturum join mtp in _context.MobilTeklifPolice on mo.PoliceId equals mtp.Id join br in _context.Brans on mtp.BransKodu equals br.BransKodu join ku in _context.Kullanici on mo.KullaniciId equals ku.Id where mo.KullaniciId == kullanici.Id select new IletisimResponse { PoliceNumarasi = mtp.PoliceNumarasi, TalepNo = mtp.TeklifIslemNo, BransKodu = br.BransKodu,AcenteKodu=mo.AcenteKodu,  KullaniciId = ku.Id, PoliceId = mo.PoliceId, PoliceTip = mo.PoliceTip, SirketKodu = mo.SirketKodu, Token = mo.Token }).ToList();
            res.MessageList = (from mo in _context.MobilMessageOturum  join mm in _context.MobilMessage on mo.PoliceId equals mm.OturumId where mo.KullaniciId == kullanici.Id select new MobilMessage{Gonderici_Tip= mm.Gonderici_Tip,Goruldumu=mm.Goruldumu,OturumId=mm.OturumId,Id=mm.Id,Mesaj=mm.Mesaj,Tarih_Saat=mm.Tarih_Saat}).OrderByDescending(x=>x.Tarih_Saat).ToList();
            res.MessageDosyaList= (from mo in _context.MobilMessageOturum join mm in _context.MobilMessage on mo.PoliceId equals mm.OturumId join md in _context.MobilMessageDosya on mm.Id equals md.MobilMessageId where mo.KullaniciId == kullanici.Id select new MobilMessageDosya {Id=md.Id,DosyaTip=md.DosyaTip,DosyaUrl=md.DosyaUrl,MobilMessageId=md.MobilMessageId }).ToList();
            
            return res;
        }
        public bool SetReadMessage(MobilMessage message)
        {
            var result = _context.MobilMessage.Where(x => x.OturumId == message.OturumId).ToList();
            foreach (var item in result)
            {
                item.Goruldumu = "1";
            }
            int res = _context.SaveChanges();
            return res > -1 ? true : false;
        }
        public int UserNewMessageCount(MobilMessageOturum message)
        {
            var result = (from mo in _context.MobilMessageOturum join mm in _context.MobilMessage on mo.PoliceId equals mm.OturumId
                          where mo.KullaniciId == message.KullaniciId && mm.Goruldumu == "0" && mm.Gonderici_Tip == "1" select new{mo }).Count();
             
            return result;
        }
        public List<IletisimResponse> GetUserNewMessage(Kullanici kullanici)
        {
            var res = _context.MobilIletisim.Where(x => x.KullaniciId == kullanici.Id && x.Kullanici_Gordumu == 1).OrderByDescending(x => x.Id).ToList();

            List<IletisimResponse> iletisimResponse = new List<IletisimResponse>();
            foreach (var item in res)
            {
                iletisimResponse.Add(new IletisimResponse { AcenteKodu = item.AcenteKodu, Cevap_Suresi = item.Cevap_Suresi, Cevap_Tarihi = item.Cevap_Tarihi, Firma_Mesaji = item.Firma_Mesaji, Id = item.Id, KullaniciId = item.KullaniciId, Kullanici_Gordumu = item.Kullanici_Gordumu, Kullanici_Mesaj = item.Kullanici_Mesaj, PoliceId = item.PoliceId, PoliceTip = item.PoliceTip, SirketKodu = item.SirketKodu, Tarih_Saat = item.Tarih_Saat, Token = item.Token, EkSureDisplay = item.EkSure != null ? string.Format("{0:hh\\:mm\\:ss}", item.EkSure) : "" });

            }
            return iletisimResponse;
        }
         
        int DateDiff(DateTime finishdateTime)
        {
            var res = (finishdateTime - DateTime.Now).Days;
            return res != null ? res : 0;
        }
        public object test(string kimlikno)
        {
            return null;
        }
        public CompanyMailData getIletisimMailData(int policeid,bool isFirstMail=true)
        {
            CompanyMailData res;
            if (isFirstMail)
            {
                res = _context.MobilTeklifPolice.Where(x => x.Id == policeid).Join(_context.TVMDetay, mt => mt.AcenteUnvani, tvm => tvm.Kodu, (mt, tvm) => new { mt, tvm }).Join(_context.MobilPoliceDosya, mtt => mtt.mt.Id, mpd => mpd.MobilTeklifId, (mtt, mpd) => new CompanyMailData { FirmaAdi = mtt.tvm.Unvani, Dosya = mpd.DosyaUrl, FirmaMail = mtt.tvm.Email }).FirstOrDefault();
            }
            else
            {
                res = _context.MobilMessageOturum.Where(x => x.PoliceId == policeid).Join(_context.TVMDetay, mt => mt.AcenteKodu, tvm => tvm.Kodu, (mt, tvm) => new CompanyMailData { FirmaAdi = tvm.Unvani, FirmaMail = tvm.Email,Token=mt.Token }).FirstOrDefault();
            }
          

            return res;
        }
        public bool checkMessageOturum(int policeid)
        {
            var res = _context.MobilMessageOturum.Where(x => x.PoliceId==policeid).FirstOrDefault();
            return res != null ? true : false;
        }
        public int getSessionId(string token)
        {
            var res = _context.MobilMessageOturum.Where(x => x.Token== token).FirstOrDefault();
            return res.PoliceId;
        }
    }
}
