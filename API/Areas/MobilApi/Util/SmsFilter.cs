using API.Areas.MobilApi.Helper;
using API.Areas.MobilApi.Models;
using API.Areas.MobilApi.Models.Input;
using Microsoft.Extensions.Options;
using SigortaDefterimV2API.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Util
{
    public class SmsFilter
    {
        public string _SirketAdi = "", _path = "", _bodyPlain = "", _dogrulamaKodu = "";
        string newfilepath = Directory.GetCurrentDirectory() + "/areas/mobilapi/files/smscode/smscodefile.json";
        List<string> _bodyList;
        public List<string> _resList;
        SmsItem _smsItem;
        string smscodepath = "mobilapi/files/smscode/", smscodelogpath = "mobilapi/files/smscodelog/";

        public SmsFilter(SmsItem smsItem, string path = "")
        {
            _SirketAdi = smsItem.SirketAdi;
            _path = path;
            _smsItem = smsItem;
        }

        public void SmsIsle2()
        {
            readFile2();
            selectProcessTVM();
        }
        void selectProcessTVM()
        {
            if (_smsItem.fromPhone == "ALLIANZ")
            {
                AllianzCode();
            }
            else if (_smsItem.fromPhone == "SOMPO")
            {
                SompoCode();
            }
            else if (_smsItem.fromPhone == "AKSIGORTA")
            {
                AkSigortaCode();
            }
            else if (_smsItem.fromPhone == "AXA" || _smsItem.fromPhone == ".AXA." || _smsItem.fromPhone.Trim() == ".AXA." || _smsItem.fromPhone.Contains("AXA"))
            {
                AxaCode();
            }
            else if (_smsItem.fromPhone == "EthicaSgrt")
            {
                EthicaCode();
            }
            else if (_smsItem.fromPhone == "EUREKO")
            {
                EurekoCode();
            }
            else if (_smsItem.fromPhone == "GULFSIGORTA")
            {
                GulfCode();
            }
            else if (_smsItem.fromPhone == "QUICKSGORTA")
            {
                QuickCode();
            }
            else if (_smsItem.fromPhone == "RAY SIGORTA")
            {
                RayCode();
            }
            else if (_smsItem.fromPhone == "ZURICH")
            {
                ZurichCode();
            }
            else if (_smsItem.fromPhone == "UNICO")
            {
                UnicoCode();
            }
            else if (_smsItem.fromPhone == "ANADOLUSIG")
            {
                AnadoluCode();
            }
            else if (_smsItem.fromPhone == "MgdburgerSg")
            {
                MagdeburgerCode();
            }
            else if (_smsItem.fromPhone == "AREXSIGORTA")
            {
                ArexCode();
            }
            else if (_smsItem.fromPhone == "Hepiyi" || _smsItem.fromPhone == "HEPIYI")
            {
                HepIyiCode();
            }
        }

        void readFile2()
        {
            var data = Utils.prevData(_smsItem, true);
            _bodyList = data.Where(x => (x.fromPhone == _smsItem.fromPhone || x.fromPhone.Contains(_smsItem.fromPhone)) && (x.toPhone == _smsItem.toPhone || x.toPhone.Contains(_smsItem.toPhone))).Select(x => x.body).ToList();
            if (_smsItem.fromPhone2 != null)
                _bodyList = data.Where(x => (x.fromPhone == _smsItem.fromPhone2 || x.fromPhone.Contains(_smsItem.fromPhone2)) && (x.toPhone == _smsItem.toPhone || x.toPhone.Contains(_smsItem.toPhone))).Select(x => x.body).ToList();
            _bodyList.Reverse();
            _bodyList = _bodyList.Take(2).ToList();
        }


        string trKarakterCikar(string data)
        {
            data = data.Replace("ı", "i").Replace("ş", "s").Replace("ğ", "g").Replace("ç", "c").Replace("ü", "u").Replace("ö", "o");
            return data;
        }
        #region sirketkoduOku
        void AllianzCode()
        {
            dogrulamaKodu = string.Empty;
            //Degerli Kullanicimiz, Digitall sistemine giris icin onay sifreniz: 326640. B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("onay sifreniz: ");
                if (yer1 != -1)
                {
                    yer1 = body.IndexOf(":", yer1) + 1;
                    int yer2 = body.IndexOf(". B", yer1);
                    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim();

                    if (dogrulamaKodu.Length > 0)
                    {
                        _resList.Add(dogrulamaKodu);
                    }
                }
            }
        }
        string dogrulamaKodu = string.Empty, body = string.Empty;
        void SompoCode()
        {
            dogrulamaKodu = string.Empty;
            body = string.Empty;
            //ERKOC kullanicisinin sisteme giris yapmak icin dogrulama kodunuz: 329996 B002 // eski
            //AAKDAGCI17 kullanıcısının sisteme giriş yapması için doğrulama kodunuz: 4677 B002
            //AAKDAGCI17 kullanıcısının sisteme giriş yapması için doğrulama kodunuz: 1593 B016
            _resList = new List<string>();
            foreach (var item in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                body = item;
                body = trKarakterCikar(body);
                int yer1 = body.IndexOf("dogrulama kodunuz");
                if (yer1 != -1)
                {
                    yer1 = body.IndexOf("uz", yer1) + 3;
                    int yer2 = body.IndexOf(" B", yer1);
                    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim().Replace(":", "");

                    if (dogrulamaKodu.Length > 0)
                    {
                        _resList.Add(dogrulamaKodu);
                    }
                }
            }
        }
        void AkSigortaCode()
        {
            dogrulamaKodu = string.Empty;
            //SAT sistemlerimize giriş için kullanacağınız tek kullanımlık şifreniz 850476 B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("ifreniz");
                if (yer1 != -1)
                {
                    yer1 = body.IndexOf(" ", yer1) + 1;
                    int yer2 = body.IndexOf(" B", yer1);
                    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim();

                    if (dogrulamaKodu.Length > 0)
                    {
                        _resList.Add(dogrulamaKodu);
                    }
                }
            }

        }
        void AxaCode()
        {
            dogrulamaKodu = string.Empty;
            //Degerli Kullanicimiz, AXA Sigorta sistemlerine giris icin tek kullanimlik sifreniz 557239 Saglikli günler dileriz. B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("kullanimlik sifreniz ");
                if (yer1 != -1)
                {
                    yer1 = body.IndexOf("niz ", yer1) + 4;
                    int yer2 = body.IndexOf(" ", yer1);
                    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim();

                    if (dogrulamaKodu.Length > 0)
                    {
                        _resList.Add(dogrulamaKodu);
                    }
                }
            }

        }
        void EthicaCode()
        {
            dogrulamaKodu = string.Empty;
            //977889 kodunu PATHICA'ya giriş yapmak için kullanabilirsiniz. B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("PATHICA");
                if (yer1 != -1)
                {
                    dogrulamaKodu = body.Substring(0, 6).Trim();

                    if (dogrulamaKodu.Length > 0)
                    {
                        _resList.Add(dogrulamaKodu);
                    }
                }
            }
        }
        void EurekoCode()
        {
            dogrulamaKodu = string.Empty;
            //Tek kullanimlik Dogrulama Kodunuz: 83764 Oturum Anahtari: NGFT B5050
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("Tek kullan");
                if (yer1 != -1)
                {
                    yer1 = body.IndexOf("nuz: ", yer1) + 5;
                    int yer2 = body.IndexOf("Oturum", yer1);
                    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim();

                    if (dogrulamaKodu.Length > 0)
                    {
                        _resList.Add(dogrulamaKodu);
                    }
                }
            }

        }
        void GulfCode()
        {
            dogrulamaKodu = string.Empty;
            //46556 sifreniz ile sisteme giris yapabilirsiniz. B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("ifreniz ile sisteme");
                if (yer1 != -1)
                {
                    dogrulamaKodu = body.Substring(0, 6).Trim();

                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }
        }
        void QuickCode()
        {
            dogrulamaKodu = string.Empty;
            //60239 Sifreniz ile sisteme giris yapabilirsiniz. B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("ile sisteme giri");
                if (yer1 != -1)
                {
                    dogrulamaKodu = body.Substring(0, 6).Trim();

                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }

        }
        void RayCode()
        {
            dogrulamaKodu = string.Empty;
            //297076 Sifreniz ile sisteme giris yapabilirsiniz.Sifrenizi Ray Sigorta Personeli dahil kimse ile paylasmayiniz. B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("sisteme giris");
                if (yer1 != -1)
                {
                    dogrulamaKodu = body.Substring(0, 6).Trim();

                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }
        }
        void ZurichCode()
        {
            dogrulamaKodu = string.Empty;
            //GIRIS SIFRENIZ=563754 Mersis:0833007681100018 B002
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("GIRIS SIFRENIZ");
                if (yer1 != -1)
                {
                    yer1 = body.IndexOf("=", yer1) + 1;
                    int yer2 = body.IndexOf(" Mersis", yer1);
                    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim().Replace(":", "");

                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }
        }
        void UnicoCode()
        {
            dogrulamaKodu = string.Empty;
            //3454 nolu sifre ile isleminizi yapabilirsiniz. 29.11.2021 - 14:40:06 B016
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                int yer1 = body.IndexOf("leminizi yapabilirsiniz");
                if (yer1 != -1)
                {
                    dogrulamaKodu = body.Substring(0, 5).Trim();

                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }
        }
        void AnadoluCode()
        {
            dogrulamaKodu = string.Empty;
            body = string.Empty;
            //ERKOC kullanicisinin sisteme giris yapmak icin dogrulama kodunuz: 329996 B002 -- eski
            //Karsilama Sayfasina dogrulama kodunuz ile giris yapabilirsiniz. Dogrulama kodu : 968796 B002 -- eski
            //626534 doğrulama kodu ile Asenta'ya giriş yapabilirsiniz. B016
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                if (body.Length > 5)
                {
                    dogrulamaKodu = body.Substring(0, 6).Trim();
                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
                //body = item;
                //body = trKarakterCikar(body);
                //int yer1 = body.IndexOf("Dogrulama kodu");
                //if (yer1 != -1)
                //{
                //    yer1 = body.IndexOf("du", yer1) + 3;
                //    int yer2 = body.IndexOf(" B", yer1);
                //    dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim().Replace(":", "");

                //    if (dogrulamaKodu.Length > 0)
                //    {
                //        _resList.Add(dogrulamaKodu);
                //    }
                //}
            }
        }
        void MagdeburgerCode()
        {
            dogrulamaKodu = string.Empty;
            //52645 Sifreniz ile sisteme giris yapabilirsiniz. B331 -- eski
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                if (body.Length > 5)
                {
                    dogrulamaKodu = body.Substring(0, 5).Trim();
                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }
        }
        void ArexCode()
        {
            dogrulamaKodu = string.Empty;
            //768087 dogrulama kodu ile islem yapabilirsiniz. B153
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (_resList.Count == 2)
                    break;
                if (body.Length > 5)
                {
                    dogrulamaKodu = body.Substring(0, 6).Trim();
                    if (dogrulamaKodu.Length > 0)
                        if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                        {
                            _resList.Add(dogrulamaKodu);
                        }
                }
            }
        }
        void HepIyiCode()
        {


            dogrulamaKodu = string.Empty;

            //Acente Portalı Doğrulama Kod: 123435 B043
            _resList = new List<string>();
            foreach (var body in _bodyList)
            {
                if (body.IndexOf("Doğrulama Kod") > 0)
                {
                    if (_resList.Count == 2)
                        break;
                    int yer1 = body.IndexOf("Doğrulama Kod");
                    if (yer1 != -1)
                    {
                        yer1 = body.IndexOf(":", yer1) + 1;
                        int yer2 = body.IndexOf(" B043", yer1);
                        dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim().Replace(":", "");

                        if (dogrulamaKodu.Length > 0)
                            if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                            {
                                _resList.Add(dogrulamaKodu);
                            }
                    }
                }
                else
                {
                    if (_resList.Count == 2)
                        break;
                    int yer1 = body.IndexOf("Dogrulama Kod");
                    if (yer1 != -1)
                    {
                        yer1 = body.IndexOf(":", yer1) + 1;
                        int yer2 = body.IndexOf(" B002", yer1);
                        dogrulamaKodu = body.Substring(yer1, yer2 - yer1).Trim().Replace(":", "");

                        if (dogrulamaKodu.Length > 0)
                            if (int.TryParse(dogrulamaKodu, out int dogrulemeKoduOnay))
                            {
                                _resList.Add(dogrulamaKodu);
                            }
                    }
                }
            }
        }
        #endregion
    }
}