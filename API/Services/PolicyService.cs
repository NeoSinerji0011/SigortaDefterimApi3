using API.Models.Database;
using API.Models.Inputs.Policy;
using API.Models.Responses;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Database;
using SigortaDefterimV2API.Models.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Services
{
    class SystemPolicy
    {
        public PoliceArac pa { get; set; }
        public PoliceGenel pg { get; set; }
        public PoliceSigortali ps { get; set; }
    }
    public class PolicyService
    {
        private DataContext _context;

        public PolicyService(DataContext context)
        {
            _context = context;
        }
        List<int> bransKodulist = new List<int> { 1, 2, 4, 8, 11, 21, 22 };
        public List<MobilPolicyResponse> GetPolicies(string KimlikNo, int BransKodu)
        {
            List<MobilPolicyResponse> policeList = new List<MobilPolicyResponse>();
            List<MobilTeklifPolice> res = new List<MobilTeklifPolice>();
            MobilPolicyResponse temp = new MobilPolicyResponse();
            List<MobilPoliceDosya> listPolicyFile = null;
            if (BransKodu < 0)
            {

                res = _context.MobilTeklifPolice.Where(police => police.KimlikNo == KimlikNo && !bransKodulist.Contains((int)police.BransKodu)).ToList();
            }
            else
            {
                res = _context.MobilTeklifPolice.Where(police => police.KimlikNo == KimlikNo && police.BransKodu == BransKodu).ToList();
                listPolicyFile = (from mtp in _context.MobilTeklifPolice
                                  join mpd in _context.MobilPoliceDosya on mtp.Id equals mpd.MobilTeklifId
                                  where mtp.KimlikNo == KimlikNo && mtp.BransKodu == BransKodu && mpd.DosyaTipi == "pdf"
                                  select new MobilPoliceDosya { DosyaUrl = mpd.DosyaUrl, MobilTeklifId = mpd.MobilTeklifId }).ToList();
            }
            foreach (var item in res)
            {
                temp = new MobilPolicyResponse();
                convertResponseClass(item, temp, listPolicyFile);
                setAcenteDetail(temp, null, item.AcenteUnvani);
                temp.ManuelMi = temp.TeklifPolice == 0 ? "1" : "0";
                policeList.Add(temp);
            }
            return policeList;
        }
        void setAcenteDetail(MobilPolicyResponse mobilPolicyResponse, MobilDamagePolicyResponse mobilDamagePolicyResponse, int? acenteKodu)
        {
            var resultAcente = acenteKodu != null ? getAcenteDetail((int)acenteKodu) : null;
            if (mobilPolicyResponse != null)
            {
                mobilPolicyResponse.AcenteAdi = resultAcente != null ? resultAcente.Unvani : "Portal Üyesi Değil";
                mobilPolicyResponse.AcenteTelNo = resultAcente == null ? "Acentenin telefon bilgisi bulunamadı" : !string.IsNullOrEmpty(resultAcente.Telefon) ? resultAcente.Telefon : "Acentenin telefon bilgisi bulunamadı";
            }
            else
            {
                mobilDamagePolicyResponse.AcenteAdi = resultAcente != null ? resultAcente.Unvani : "Portal Üyesi Değil";
                mobilDamagePolicyResponse.AcenteTelNo = resultAcente == null ? "Acentenin telefon bilgisi bulunamadı" : !string.IsNullOrEmpty(resultAcente.Telefon) ? resultAcente.Telefon : "Acentenin telefon bilgisi bulunamadı";
            }
        }
        void convertResponseClass(MobilTeklifPolice mobilTeklifPolice, MobilPolicyResponse mobilPolicyResponse, List<MobilPoliceDosya> listPolicyFile = null)
        {

            mobilPolicyResponse.AcenteUnvani = mobilTeklifPolice.AcenteUnvani;
            mobilPolicyResponse.Aciklama = mobilTeklifPolice.Aciklama;
            mobilPolicyResponse.Adres = mobilTeklifPolice.Adres;
            mobilPolicyResponse.AracKullanimTarzi = mobilTeklifPolice.AracKullanimTarzi;
            mobilPolicyResponse.AsbisNo = mobilTeklifPolice.AsbisNo;
            mobilPolicyResponse.BaslangicTarihi = mobilTeklifPolice.BaslangicTarihi;
            mobilPolicyResponse.BinaBedeli = mobilTeklifPolice.BinaBedeli;
            mobilPolicyResponse.BinaKatSayisi = mobilTeklifPolice.BinaKatSayisi;
            mobilPolicyResponse.BinaKullanimSekli = mobilTeklifPolice.BinaKullanimSekli;
            mobilPolicyResponse.BinaYapimYili = mobilTeklifPolice.BinaYapimYili;
            mobilPolicyResponse.BinaYapiTarzi = mobilTeklifPolice.BinaYapiTarzi;
            mobilPolicyResponse.BitisTarihi = mobilTeklifPolice.BitisTarihi;
            mobilPolicyResponse.BransKodu = mobilTeklifPolice.BransKodu;
            mobilPolicyResponse.DaireBrut = mobilTeklifPolice.DaireBrut;
            mobilPolicyResponse.EsyaBedeli = mobilTeklifPolice.EsyaBedeli;
            mobilPolicyResponse.Id = mobilTeklifPolice.Id;
            mobilPolicyResponse.IlceKodu = mobilTeklifPolice.IlceKodu;
            mobilPolicyResponse.IlKodu = mobilTeklifPolice.IlKodu;
            mobilPolicyResponse.KimlikNo = mobilTeklifPolice.KimlikNo;
            mobilPolicyResponse.MarkaKodu = mobilTeklifPolice.MarkaKodu;
            mobilPolicyResponse.Meslek = mobilTeklifPolice.Meslek;
            mobilPolicyResponse.ModelYili = mobilTeklifPolice.ModelYili;
            mobilPolicyResponse.Plaka = mobilTeklifPolice.Plaka;
            mobilPolicyResponse.PoliceNumarasi = mobilTeklifPolice.PoliceNumarasi;
            mobilPolicyResponse.RuhsatSeriKodu = mobilTeklifPolice.RuhsatSeriKodu;
            mobilPolicyResponse.RuhsatSeriNo = mobilTeklifPolice.RuhsatSeriNo;
            mobilPolicyResponse.SeyahatDonusTarihi = mobilTeklifPolice.SeyahatDonusTarihi;
            mobilPolicyResponse.SeyahatEdenKisiSayisi = mobilTeklifPolice.SeyahatEdenKisiSayisi;
            mobilPolicyResponse.SeyahatGidisTarihi = mobilTeklifPolice.SeyahatGidisTarihi;
            mobilPolicyResponse.SeyahatUlkeKodu = mobilTeklifPolice.SeyahatUlkeKodu;
            mobilPolicyResponse.SirketKodu = mobilTeklifPolice.SirketKodu;
            mobilPolicyResponse.TeklifIslemNo = mobilTeklifPolice.TeklifIslemNo;
            mobilPolicyResponse.TeklifPolice = mobilTeklifPolice.TeklifPolice;
            mobilPolicyResponse.TipKodu = mobilTeklifPolice.TipKodu;
            mobilPolicyResponse.YenilemeNo = mobilTeklifPolice.YenilemeNo;
            var temp = listPolicyFile != null ? listPolicyFile.Where(x => x.MobilTeklifId == mobilTeklifPolice.Id).FirstOrDefault() : null;
            mobilPolicyResponse.PdfUrl = temp != null ? temp.DosyaUrl : null;
        }
        public List<MobilPolicyResponse> getSystemPolicies(string kimlikno, int BransKodu)
        {
            List<SystemPolicy> listTemp;
            if (BransKodu < 0)
                listTemp = (from ps in _context.PoliceSigortali join pg in _context.PoliceGenel on ps.PoliceId equals pg.PoliceId join pa in _context.PoliceArac on pg.PoliceId equals pa.PoliceId into paa from pa in paa.DefaultIfEmpty() where ps.KimlikNo == kimlikno && !bransKodulist.Contains((int)pg.BransKodu) select new SystemPolicy { pg = pg, pa = pa, ps = ps }).ToList();
            else
                listTemp = (from ps in _context.PoliceSigortali join pg in _context.PoliceGenel on ps.PoliceId equals pg.PoliceId join pa in _context.PoliceArac on pg.PoliceId equals pa.PoliceId into paa from pa in paa.DefaultIfEmpty() where ps.KimlikNo == kimlikno && pg.BransKodu == BransKodu select new SystemPolicy { pg = pg, pa = pa, ps = ps }).ToList();

            List<MobilPolicyResponse> policeList = new List<MobilPolicyResponse>();
            MobilPolicyResponse temp;
            MobilTeklifPolice mobilTeklifPolice;
            foreach (var item in listTemp)
            {
                mobilTeklifPolice = new MobilTeklifPolice();
                mobilTeklifPolice.AcenteUnvani = item.pg.TVMKodu;
                mobilTeklifPolice.TeklifPolice = 0;
                mobilTeklifPolice.KimlikNo = item.ps.KimlikNo;
                if (item.pg.BransKodu != 21)
                {
                    mobilTeklifPolice.BaslangicTarihi = item.pg.BaslangicTarihi;
                    mobilTeklifPolice.BitisTarihi = item.pg.BitisTarihi;
                }
                else
                {
                    mobilTeklifPolice.SeyahatGidisTarihi = item.pg.BaslangicTarihi;
                    mobilTeklifPolice.SeyahatDonusTarihi = item.pg.BitisTarihi;
                }
                mobilTeklifPolice.PoliceNumarasi = item.pg.PoliceNumarasi;
                mobilTeklifPolice.YenilemeNo = item.pg.YenilemeNo;
                mobilTeklifPolice.SirketKodu = item.pg.TUMBirlikKodu;
                mobilTeklifPolice.BransKodu = item.pg.BransKodu;
                mobilTeklifPolice.Aciklama = "";
                //mobilTeklifPolice.AcenteUnvani


                switch (item.pg.BransKodu)
                {
                    case 1:
                    case 2:
                        mobilTeklifPolice.Plaka = (item.pa.PlakaKodu != null ? item.pa.PlakaKodu.Length == 3 ? item.pa.PlakaKodu.Substring(1) : item.pa.PlakaKodu : "") + item.pa.PlakaNo;
                        mobilTeklifPolice.ModelYili = item.pa.Model;
                        mobilTeklifPolice.RuhsatSeriKodu = item.pa.TescilSeriKod != null ? item.pa.TescilSeriKod : "";
                        mobilTeklifPolice.RuhsatSeriNo = item.pa.TescilSeriNo != null ? item.pa.TescilSeriNo : "";
                        var tipTemp = _context.AracTip.Where(x => x.TipKodu == item.pa.AracinTipiKodu).FirstOrDefault();
                        tipTemp = tipTemp != null ? tipTemp : _context.AracTip.Where(x => x.TipAdi == item.pa.AracinTipiAciklama).FirstOrDefault();
                        mobilTeklifPolice.TipKodu = tipTemp != null ? tipTemp.TipKodu : "";
                        var markaTemp = _context.AracMarka.Where(x => x.MarkaAdi == item.pa.MarkaAciklama).FirstOrDefault();
                        markaTemp = markaTemp != null ? markaTemp : _context.AracMarka.Where(x => x.MarkaKodu == item.pa.Marka).FirstOrDefault();
                        mobilTeklifPolice.MarkaKodu = markaTemp != null ? markaTemp.MarkaKodu : "";
                        mobilTeklifPolice.AsbisNo = item.pa.AsbisNo != null ? item.pa.AsbisNo : "";
                        break;
                    case 4:
                        mobilTeklifPolice.Meslek = "";
                        break;
                    case 11:
                        mobilTeklifPolice.BinaYapimYili = null;
                        mobilTeklifPolice.BinaYapiTarzi = null;
                        mobilTeklifPolice.BinaKullanimSekli = null;

                        mobilTeklifPolice.IlKodu = getIlData(new Il() { IlKodu = item.ps.IlKodu, IlAdi = item.ps.IlAdi });

                        mobilTeklifPolice.IlceKodu = getIlceData(new Ilce() { IlceAdi = item.ps.IlceAdi, IlceKodu = item.ps.IlceKodu }, new Il() { IlKodu = item.ps.IlKodu, IlAdi = item.ps.IlAdi });
                        mobilTeklifPolice.Adres = item.ps.Adres;
                        break;
                    case 21:
                        mobilTeklifPolice.SeyahatUlkeKodu = "";
                        mobilTeklifPolice.SeyahatEdenKisiSayisi = null;
                        break;
                    case 22:
                        mobilTeklifPolice.IlKodu = getIlData(new Il() { IlKodu = item.ps.IlKodu, IlAdi = item.ps.IlAdi });

                        mobilTeklifPolice.IlceKodu = getIlceData(new Ilce() { IlceAdi = item.ps.IlceAdi, IlceKodu = item.ps.IlceKodu }, new Il() { IlKodu = item.ps.IlKodu, IlAdi = item.ps.IlAdi });
                        mobilTeklifPolice.Adres = item.ps.Adres;
                        mobilTeklifPolice.EsyaBedeli = 0;
                        mobilTeklifPolice.BinaBedeli = 0;
                        break;
                    default:
                        mobilTeklifPolice.IlKodu = getIlData(new Il() { IlKodu = item.ps.IlKodu, IlAdi = item.ps.IlAdi });

                        mobilTeklifPolice.IlceKodu = getIlceData(new Ilce() { IlceAdi = item.ps.IlceAdi, IlceKodu = item.ps.IlceKodu }, new Il() { IlKodu = item.ps.IlKodu, IlAdi = item.ps.IlAdi });
                        break;
                }
                temp = new MobilPolicyResponse();
                convertResponseClass(mobilTeklifPolice, temp);
                setAcenteDetail(temp, null, mobilTeklifPolice.AcenteUnvani);
                temp.ManuelMi = "0";

                policeList.Add(temp);
            }
            return policeList;
        }

        public MobilTeklifPolice GetPolicy(int policyId)
        {
            return _context.MobilTeklifPolice.Where(police => police.Id == policyId).FirstOrDefault();
        }
        string getIlData(Il il)
        {
            Il ilTemp = _context.Il.Where(x => x.IlKodu == il.IlKodu).FirstOrDefault();
            ilTemp = ilTemp != null ? ilTemp : _context.Il.Where(x => x.IlAdi == il.IlAdi).FirstOrDefault();
            return ilTemp != null ? ilTemp.IlKodu : null;
        }
        int? getIlceData(Ilce ilce, Il il)
        {
            string ilkodu = getIlData(il);
            Ilce ilceTemp = _context.Ilce.Where(x => x.IlceKodu == ilce.IlceKodu).FirstOrDefault();
            ilceTemp = ilceTemp != null ? ilceTemp : _context.Ilce.Where(x => x.IlceAdi == ilce.IlceAdi && x.IlKodu == ilkodu).FirstOrDefault();
            if (ilceTemp != null)
                return (int)ilceTemp.IlceKodu;
            return null;
        }
        public DamagePolicyResponse GetDamagePolicies(string KimlikNo, int BransKodu)
        {
            DamagePolicyResponse result = new DamagePolicyResponse();
            List<MobilDamagePolicyResponse> mobilpolicehasarList = new List<MobilDamagePolicyResponse>();
            List<MobilPoliceDosya> mobilpolicedosyaList = new List<MobilPoliceDosya>();
            List<MobilPoliceHasar> res = new List<MobilPoliceHasar>();
            MobilDamagePolicyResponse temp;
            if (BransKodu == -1)
            {
                res = _context.MobilPoliceHasar.Where(x => x.KimlikNo == KimlikNo && x.BransKodu != 1 && x.BransKodu != 2 && x.BransKodu != 22 && x.BransKodu != 11 && x.BransKodu != 4 && x.BransKodu != 8 && x.BransKodu != 21).ToList();
                mobilpolicedosyaList = _context.MobilPoliceDosya.Join(_context.MobilPoliceHasar.Where(x => x.KimlikNo == KimlikNo && x.BransKodu != 1 && x.BransKodu != 2 && x.BransKodu != 22 && x.BransKodu != 11 && x.BransKodu != 4 && x.BransKodu != 8 && x.BransKodu != 21), mpd => mpd.MobilHasarId, mph => mph.Id, (mpd, mph) => new MobilPoliceDosya
                {
                    MobilHasarId = mph.Id,
                    DosyaTipi = mpd.DosyaTipi,
                    DosyaUrl = mpd.DosyaUrl
                }).ToList();

            }
            else
            {
                res = _context.MobilPoliceHasar.Where(x => x.KimlikNo == KimlikNo && x.BransKodu == BransKodu).ToList();
                mobilpolicedosyaList = _context.MobilPoliceDosya.Join(_context.MobilPoliceHasar.Where(x => x.KimlikNo == KimlikNo && x.BransKodu == BransKodu), mpd => mpd.MobilHasarId, mph => mph.Id, (mpd, mph) => new MobilPoliceDosya
                {
                    MobilHasarId = mph.Id,
                    DosyaTipi = mpd.DosyaTipi,
                    DosyaUrl = mpd.DosyaUrl
                }).ToList();
            }
            foreach (var item in res)
            {
                temp = new MobilDamagePolicyResponse();
                convertDamageResponseClass(item, temp);
                setAcenteDetail(null, temp, item.AcenteUnvani);
                mobilpolicehasarList.Add(temp);
            }
            result.mobilPoliceHasarList = mobilpolicehasarList;
            result.mobilPoliceDosyaList = mobilpolicedosyaList;
            return result;

        }
        void convertDamageResponseClass(MobilPoliceHasar mobilTeklifPolice, MobilDamagePolicyResponse mobilPolicyResponse)
        {

            mobilPolicyResponse.AcenteUnvani = mobilTeklifPolice.AcenteUnvani;
            mobilPolicyResponse.Aciklama = mobilTeklifPolice.Aciklama;
            mobilPolicyResponse.Adres = mobilTeklifPolice.Adres;
            mobilPolicyResponse.AracKullanimTarzi = mobilTeklifPolice.AracKullanimTarzi;
            mobilPolicyResponse.AsbisNo = mobilTeklifPolice.AsbisNo;
            mobilPolicyResponse.BaslangicTarihi = mobilTeklifPolice.BaslangicTarihi;
            mobilPolicyResponse.BinaBedeli = mobilTeklifPolice.BinaBedeli;
            mobilPolicyResponse.BinaKatSayisi = mobilTeklifPolice.BinaKatSayisi;
            mobilPolicyResponse.BinaKullanimSekli = mobilTeklifPolice.BinaKullanimSekli;
            mobilPolicyResponse.BinaYapimYili = mobilTeklifPolice.BinaYapimYili;
            mobilPolicyResponse.BinaYapiTarzi = mobilTeklifPolice.BinaYapiTarzi;
            mobilPolicyResponse.BitisTarihi = mobilTeklifPolice.BitisTarihi;
            mobilPolicyResponse.BransKodu = mobilTeklifPolice.BransKodu;
            mobilPolicyResponse.DaireBrut = mobilTeklifPolice.DaireBrut;
            mobilPolicyResponse.Dosya = mobilTeklifPolice.Dosya;
            mobilPolicyResponse.EsyaBedeli = mobilTeklifPolice.EsyaBedeli;
            mobilPolicyResponse.Id = mobilTeklifPolice.Id;
            mobilPolicyResponse.IlceKodu = mobilTeklifPolice.IlceKodu;
            mobilPolicyResponse.IlKodu = mobilTeklifPolice.IlKodu;
            mobilPolicyResponse.KimlikNo = mobilTeklifPolice.KimlikNo;
            mobilPolicyResponse.MarkaKodu = mobilTeklifPolice.MarkaKodu;
            mobilPolicyResponse.Meslek = mobilTeklifPolice.Meslek;
            mobilPolicyResponse.ModelYili = mobilTeklifPolice.ModelYili;
            mobilPolicyResponse.Plaka = mobilTeklifPolice.Plaka;
            mobilPolicyResponse.PoliceNumarasi = mobilTeklifPolice.PoliceNumarasi;
            mobilPolicyResponse.RuhsatSeriKodu = mobilTeklifPolice.RuhsatSeriKodu;
            mobilPolicyResponse.RuhsatSeriNo = mobilTeklifPolice.RuhsatSeriNo;
            mobilPolicyResponse.SeyahatDonusTarihi = mobilTeklifPolice.SeyahatDonusTarihi;
            mobilPolicyResponse.SeyahatEdenKisiSayisi = mobilTeklifPolice.SeyahatEdenKisiSayisi;
            mobilPolicyResponse.SeyahatGidisTarihi = mobilTeklifPolice.SeyahatGidisTarihi;
            mobilPolicyResponse.SeyahatUlkeKodu = mobilTeklifPolice.SeyahatUlkeKodu;
            mobilPolicyResponse.SirketKodu = mobilTeklifPolice.SirketKodu;
            mobilPolicyResponse.TeklifIslemNo = mobilTeklifPolice.TeklifIslemNo;
            mobilPolicyResponse.TeklifPolice = mobilTeklifPolice.TeklifPolice;
            mobilPolicyResponse.TipKodu = mobilTeklifPolice.TipKodu;
            mobilPolicyResponse.YenilemeIslemNo = mobilTeklifPolice.YenilemeIslemNo;
            mobilPolicyResponse.YenilemeNo = mobilTeklifPolice.YenilemeNo;
            mobilPolicyResponse.KoordinatX = mobilTeklifPolice.KoordinatX;
            mobilPolicyResponse.KoordinatY = mobilTeklifPolice.KoordinatY;
            mobilPolicyResponse.PoliceNumarasi = mobilTeklifPolice.PoliceNumarasi;

        }

        public MobilPoliceHasar GetDamagePolicy(int policyId)
        {
            return _context.MobilPoliceHasar.Where(police => police.Id == policyId).FirstOrDefault();
        }

        public bool AddDamagePolicy(AddDamagePolicyTemp addDamagePolicyTemp)
        {
            //addDamagePolicyTemp.MobilPoliceHasar.TeklifIslemNo = getHasarTalepNo(addDamagePolicyTemp.MobilPoliceHasar.KimlikNo);
            _context.MobilPoliceHasar.Add(addDamagePolicyTemp.MobilPoliceHasar);
            int result = _context.SaveChanges();
            if (addDamagePolicyTemp.mobilPoliceDosyaList != null)
                if (addDamagePolicyTemp.mobilPoliceDosyaList.Count > 0)
                {
                    foreach (var item in addDamagePolicyTemp.mobilPoliceDosyaList)
                    {
                        item.MobilHasarId = addDamagePolicyTemp.MobilPoliceHasar.Id;
                        item.Tarih = DateTime.Now;
                    }
                    _context.MobilPoliceDosya.AddRange(addDamagePolicyTemp.mobilPoliceDosyaList);
                    _context.SaveChanges();
                }

            return result > 0 ? true : false;
        }
        public MobilTeklifPolice AddRenewPolicy(AddDamagePolicyTemp addDamagePolicyTemp)
        {
            //addDamagePolicyTemp.MobilTeklifPolice.TeklifIslemNo = getYenileTalepNo(addDamagePolicyTemp.MobilTeklifPolice.KimlikNo);
            _context.MobilTeklifPolice.Add(addDamagePolicyTemp.MobilTeklifPolice);
            _context.SaveChanges();
            if (addDamagePolicyTemp.mobilPoliceDosyaList != null)
                if (addDamagePolicyTemp.mobilPoliceDosyaList.Count > 0)
                {
                    foreach (var item in addDamagePolicyTemp.mobilPoliceDosyaList)
                    {
                        item.MobilTeklifId = addDamagePolicyTemp.MobilTeklifPolice.Id;
                        item.Tarih = DateTime.Now;
                    }
                    _context.MobilPoliceDosya.AddRange(addDamagePolicyTemp.mobilPoliceDosyaList);
                    _context.SaveChanges();
                }
            return addDamagePolicyTemp.MobilTeklifPolice;
        }
        public void addMessageOturum(MobilMessageOturum messageOturum)
        {
            _context.MobilMessageOturum.Add(messageOturum);
            _context.SaveChanges();
        }
        public bool AddPolicy(MobilTeklifPolice mobilTeklifPolice)
        {
            _context.MobilTeklifPolice.Add(mobilTeklifPolice);
            int result = _context.SaveChanges();
            return result > 0 ? true : false;
        }
        public bool checkPolicy(MobilTeklifPolice police)
        {
            var res = _context.MobilTeklifPolice.Where(x => x.PoliceNumarasi == police.PoliceNumarasi && x.YenilemeNo == police.YenilemeNo && x.KimlikNo == police.KimlikNo).FirstOrDefault();
            return res != null ? true : false;
        }
        public void UpdateCarInfo(MobilTeklifPolice police)
        {
            _context.Update(police);
            _context.SaveChanges();
        }

        public int getHasarTalepNo(string KimlikNo)
        {
            var result = _context.MobilPoliceHasar.Where(x => x.KimlikNo == KimlikNo).OrderByDescending(x => x.TeklifIslemNo).Take(1).SingleOrDefault();
            return result != null ? (int)result.TeklifIslemNo + 1 : 1;
        }
        public int getYenileTalepNo(string KimlikNo)
        {
            var result = _context.MobilTeklifPolice.Where(x => x.KimlikNo == KimlikNo && x.TeklifPolice == 1).OrderByDescending(x => x.TeklifIslemNo).Take(1).SingleOrDefault();
            return result == null ? 1 : result.TeklifIslemNo != null ? (int)result.TeklifIslemNo + 1 : 1;
        }
        public SigortaSirketleri getSirket(string sirketkodu)
        {
            var result = _context.SigortaSirketleri.Where(x => x.SirketKodu == sirketkodu).FirstOrDefault();
            return result != null ? result : new SigortaSirketleri();
        }
        public TVMDetay getAcente(int acentekodu)
        {
            var result = _context.TVMDetay.Where(x => x.Kodu == acentekodu).FirstOrDefault();
            if (result == null)
            {
                result = _context.MobilAcente.Where(x => x.Skor1 == 100).Take(1).Select(x => new TVMDetay { Unvani = x.TVMUnvani, Logo = x.Logo, Email = x.BildirimEmail, Kodu = x.TVMKodu }).FirstOrDefault();
            }
            return result != null ? result : new TVMDetay();
        }
        public List<TVMDetay> getTVMGrup(int acentekodu, string teklifTipi) //teklifTipi->"Y" yenileme -"T" teklif
        {
            List<TVMDetay> tvmDetayList = null;
            var grupId = _context.MobilAcente.Where(x => x.TVMKodu == acentekodu).FirstOrDefault();
            if (grupId != null)
            {
                if (teklifTipi == "T")
                {
                    tvmDetayList = _context.MobilAcente.Where(x => x.Acente_Grup_Kodu == grupId.Acente_Grup_Kodu).Select(x => new TVMDetay { Unvani = x.TVMUnvani, Logo = x.Logo, Email = x.BildirimEmail, Kodu = x.TVMKodu }).ToList();
                }
                else if (teklifTipi == "Y")
                {
                    tvmDetayList = _context.MobilAcente.Where(x => x.Acente_Grup_Kodu == grupId.Acente_Grup_Kodu && x.TVMKodu != grupId.TVMKodu).Select(x => new TVMDetay { Unvani = x.TVMUnvani, Logo = x.Logo, Email = x.BildirimEmail, Kodu = x.TVMKodu }).ToList();
                }
                else
                {
                    tvmDetayList = _context.MobilAcente.Where(x => x.TVMKodu == grupId.TVMKodu).Select(x => new TVMDetay { Unvani = x.TVMUnvani, Logo = x.Logo, Email = x.BildirimEmail, Kodu = x.TVMKodu }).ToList();
                }
            }
            else
            {
                tvmDetayList = _context.MobilAcente.Select(x => new TVMDetay { Unvani = x.TVMUnvani, Logo = x.Logo, Email = x.BildirimEmail, Kodu = x.TVMKodu }).ToList();
            }
            //if (mobilAcente == null)
            //{
            //    result = _context.MobilAcente.Where(x => x.Skor1 == 100).Take(1).Select(x => new TVMDetay { Unvani = x.TVMUnvani, Logo = x.Logo, Email = x.BildirimEmail, Kodu = x.TVMKodu }).FirstOrDefault();
            //}

            return tvmDetayList != null ? tvmDetayList : new List<TVMDetay>();
        }
        public TVMDetay getAcenteDetail(int acentekodu)
        {
            var result = _context.TVMDetay.Where(x => x.Kodu == acentekodu).FirstOrDefault();
            return result;
        }
        public string getBransAdi(int branskodu)
        {
            var result = _context.Brans.Where(x => x.BransKodu == branskodu).SingleOrDefault();
            return result != null ? result.BransAdi : "";
        }
        public string getIlAdi(string ilkodu)
        {
            var result = _context.Il.Where(x => x.IlKodu == ilkodu).SingleOrDefault();
            return result != null ? result.IlAdi : "";
        }
        public string getIlceAdi(int ilcekodu)
        {
            var result = _context.Ilce.Where(x => x.IlceKodu == ilcekodu).SingleOrDefault();
            return result != null ? result.IlceAdi : "";
        }

        public string getAracMarka(string markakodu)
        {
            var result = _context.AracMarka.Where(x => x.MarkaKodu == markakodu).SingleOrDefault();
            return result != null ? result.MarkaAdi : "";
        }
        public string getAracTipi(string tipkodu, string markakodu)
        {
            var result = _context.AracTip.Where(x => x.TipKodu == tipkodu && x.MarkaKodu == markakodu).SingleOrDefault();
            return result != null ? result.TipAdi : "";
        }
        public string getAracKullanimTarzi(string KullanimTarziKodu, string kod2)
        {
            var result = _context.AracKullanimTarzi.Where(x => x.KullanimTarziKodu == KullanimTarziKodu && x.Kod2 == kod2).SingleOrDefault();
            return result != null ? result.KullanimTarzi : "";
        }
        public Kullanici getKullanici(string KimlikNo)
        {
            var result = _context.Kullanici.Where(x => x.Tc == KimlikNo).SingleOrDefault();
            return result != null ? result : new Kullanici();
        }
        public string getMeslek(int meslekkodu)
        {
            var result = _context.Meslek.Where(x => x.MeslekKodu == meslekkodu).SingleOrDefault();
            return result != null ? result.MeslekAdi : "";
        }
        public string getUlke(string ulkekodu)
        {
            var result = _context.MobilUlke.Where(x => x.UlkeKodu == ulkekodu).Take(1).SingleOrDefault();
            return result != null ? result.UlkeAdi : "";
        }
        public string getUlkeTipi(string ulkekodu)
        {
            var result = _context.MobilUlke.Where(x => x.UlkeKodu == ulkekodu).Join(_context.MobilUlkeTipi, mu => mu.UlkeTipiKodu, mtip => mtip.UlkeTipiKodu, (mu, mtip) => new { mtip }).FirstOrDefault();
            return result != null ? result.mtip.UlkeTipiAdi : "";
        }
        public void setYenilemTeklifAdedi(int acentekodu, bool yenileme = true)
        {
            var result = _context.MobilAcente.Where(x => x.TVMKodu == acentekodu).FirstOrDefault();
            if (yenileme)
            {
                if (result.Yonlenen_Yenileme_Adedi == null)
                    result.Yonlenen_Yenileme_Adedi = 1;
                else
                    result.Yonlenen_Yenileme_Adedi += 1;

            }
            else
            {
                if (result.Yonlenen_Teklif_Adedi == null)
                    result.Yonlenen_Teklif_Adedi = 1;
                else
                    result.Yonlenen_Teklif_Adedi += 1;
            }
            _context.SaveChanges();
        }
        public List<MobilAcente> GetAcenteMailList()
        {
            var result = _context.MobilAcente.Where(x => x.Skor1 == 100).Take(1).ToList();
            return result != null ? result : new List<MobilAcente>();
        }
        public List<MobilTeklifPolice> test(string kimlikno)
        {
            var listTemp = (from ps in _context.PoliceSigortali join pg in _context.PoliceGenel on ps.PoliceId equals pg.PoliceId join pa in _context.PoliceArac on pg.PoliceId equals pa.PoliceId into paa from pa in paa.DefaultIfEmpty() where ps.KimlikNo == kimlikno select new { pg, pa, ps });
            var markaList = _context.AracMarka.ToList();
            List<MobilTeklifPolice> policeList = new List<MobilTeklifPolice>();
            MobilTeklifPolice mobilTeklifPolice;
            foreach (var item in listTemp)
            {
                var markaTemp = markaList.Where(x => x.MarkaAdi == item.pa.Marka).FirstOrDefault();
                mobilTeklifPolice = new MobilTeklifPolice();
                if (item.pg.BransKodu != 21)
                {
                    mobilTeklifPolice.BaslangicTarihi = item.pg.BaslangicTarihi;
                    mobilTeklifPolice.BitisTarihi = item.pg.BitisTarihi;
                }
                else
                {
                    mobilTeklifPolice.SeyahatGidisTarihi = item.pg.BaslangicTarihi;
                    mobilTeklifPolice.SeyahatDonusTarihi = item.pg.BitisTarihi;
                }
                mobilTeklifPolice.PoliceNumarasi = item.pg.PoliceNumarasi;
                mobilTeklifPolice.YenilemeNo = item.pg.YenilemeNo;
                mobilTeklifPolice.SirketKodu = item.pg.TUMBirlikKodu;
                mobilTeklifPolice.BransKodu = item.pg.BransKodu;
                mobilTeklifPolice.Aciklama = "";
                //mobilTeklifPolice.AcenteUnvani
                switch (item.pg.BransKodu)
                {
                    case 1:
                    case 2:
                        mobilTeklifPolice.Plaka = (item.pa.PlakaKodu != null ? item.pa.PlakaKodu.Length == 3 ? item.pa.PlakaKodu.Substring(1) : item.pa.PlakaKodu : "") + item.pa.PlakaNo;
                        mobilTeklifPolice.ModelYili = item.pa.Model;
                        mobilTeklifPolice.RuhsatSeriKodu = item.pa.TescilSeriKod != null ? item.pa.TescilSeriKod : "";
                        mobilTeklifPolice.RuhsatSeriNo = item.pa.TescilSeriNo != null ? item.pa.TescilSeriNo : "";
                        var tipTemp = _context.AracTip.Where(x => x.TipKodu == item.pa.AracinTipiKodu).FirstOrDefault();
                        tipTemp = tipTemp != null ? tipTemp : _context.AracTip.Where(x => x.TipAdi == item.pa.AracinTipiAciklama).FirstOrDefault();
                        mobilTeklifPolice.TipKodu = tipTemp != null ? tipTemp.TipKodu : "";
                        mobilTeklifPolice.AsbisNo = item.pa.AsbisNo != null ? item.pa.AsbisNo : "";
                        break;
                    case 4:
                        mobilTeklifPolice.Meslek = "";
                        break;
                    case 11:
                        mobilTeklifPolice.BinaYapimYili = null;
                        mobilTeklifPolice.BinaYapiTarzi = null;
                        mobilTeklifPolice.BinaKullanimSekli = null;
                        mobilTeklifPolice.IlKodu = item.ps.IlKodu != null ? item.ps.IlKodu : "";
                        mobilTeklifPolice.IlceKodu = item.ps.IlceKodu != null ? item.ps.IlceKodu : null;
                        mobilTeklifPolice.Adres = item.ps.Adres;
                        break;
                    case 21:
                        mobilTeklifPolice.SeyahatUlkeKodu = "";
                        mobilTeklifPolice.SeyahatEdenKisiSayisi = null;
                        break;
                    case 22:
                        mobilTeklifPolice.IlKodu = item.ps.IlKodu != null ? item.ps.IlKodu : "";
                        mobilTeklifPolice.IlceKodu = item.ps.IlceKodu != null ? item.ps.IlceKodu : null;
                        mobilTeklifPolice.Adres = item.ps.Adres;
                        mobilTeklifPolice.EsyaBedeli = 0;
                        mobilTeklifPolice.BinaBedeli = 0;
                        break;
                    default:
                        mobilTeklifPolice.IlKodu = item.ps.IlKodu != null ? item.ps.IlKodu : "";
                        mobilTeklifPolice.IlceKodu = item.ps.IlceKodu != null ? item.ps.IlceKodu : null;
                        break;
                }
                policeList.Add(mobilTeklifPolice);
            }
            return policeList;
        }
        public CompanyMailData getIletisimMailData(int policeid, bool isFirstMail = true)
        {
            CompanyMailData res;
            if (isFirstMail)
            {
                res = _context.MobilTeklifPolice.Where(x => x.Id == policeid).Join(_context.TVMDetay, mt => mt.AcenteUnvani, tvm => tvm.Kodu, (mt, tvm) => new { mt, tvm }).Join(_context.MobilPoliceDosya, mtt => mtt.mt.Id, mpd => mpd.MobilTeklifId, (mtt, mpd) => new CompanyMailData { FirmaAdi = mtt.tvm.Unvani, Dosya = mpd.DosyaUrl, FirmaMail = mtt.tvm.Email }).FirstOrDefault();
            }
            else
            {
                res = _context.MobilMessageOturum.Where(x => x.Id == policeid).Join(_context.TVMDetay, mt => mt.AcenteKodu, tvm => tvm.Kodu, (mt, tvm) => new CompanyMailData { FirmaAdi = tvm.Unvani, FirmaMail = tvm.Email, Token = mt.Token }).FirstOrDefault();
            }


            return res;
        }
    }
}
