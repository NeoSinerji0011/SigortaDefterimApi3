using API.Models.Database;
using API.Models.Inputs.Policy;
using API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SigortaDefterimV2API.Helpers;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Inputs;
using SigortaDefterimV2API.Models.Responses;
using SigortaDefterimV2API.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;

namespace SigortaDefterimV2API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private PolicyService _policyService;
        private readonly AppSettings _appSettings;
        PdfPolicy _pdfPolicy;
        Kullanici kullanici;
        AddDamagePolicyTemp addDamagePolicyTemp;
        MobilPoliceDosya mobilPoliceDosya;
        List<Attachment> mailFile;
        List<string> mailFilePathList;
        List<Attachment> mailFiletemp;
        Attachment[] att = new Attachment[2];
        string[] tempfilename;
        MobilMessageOturum messageOturum;
        public PolicyController(PolicyService policyService, IOptions<AppSettings> appSettings)
        {
            _policyService = policyService;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// İlgili TC Kimlik no ve branş koduyla eşleşen tüm MobilTeklifPolice kayıtlarını geriye liste halinde döndürür.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetPolicies")]
        [SwaggerResponse(200, Type = typeof(List<MobilTeklifPolice>))]
        [Produces("application/json")]
        public IActionResult GetPolicies([FromQuery] GetPoliciesInput input)
        {
            List<MobilPolicyResponse> policeList = new List<MobilPolicyResponse>();
            policeList.AddRange(_policyService.GetPolicies(input.KimlikNo, input.BransKodu));
            policeList.AddRange(_policyService.getSystemPolicies(input.KimlikNo, input.BransKodu));
            return Ok(policeList);
        }

        /// <summary>
        /// Id'si parametre olarak verilen MobilTeklifPolice kaydını mevcut ise geriye döndürür,
        /// bulunamaz ise 404 Not Found hatasını cevap olarak döndürür.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetPolicy")]
        [SwaggerResponse(200, Type = typeof(MobilTeklifPolice))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult GetPolicy([FromQuery] GetPolicyInput input)
        {
            MobilTeklifPolice police = _policyService.GetPolicy(input.PolicyId);
            if (police == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(police);
            }
        }

        /// <summary>
        /// İlgili TC Kimlik no ve branş koduyla eşleşen tüm MobilPoliceHasar kayıtlarını geriye liste halinde döndürür.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetDamagePolicies")]
        [SwaggerResponse(200, Type = typeof(List<MobilPoliceHasar>))]
        [Produces("application/json")]
        public IActionResult GetDamagePolicies([FromQuery] GetPoliciesInput input)
        {
            return Ok(_policyService.GetDamagePolicies(input.KimlikNo, input.BransKodu));
        }
        /// <summary>
        /// Id'si parametre olarak verilen MobilPoliceHasar kaydını mevcut ise geriye döndürür,
        /// bulunamaz ise 404 Not Found hatasını cevap olarak döndürür.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetDamagePolicy")]
        [SwaggerResponse(200, Type = typeof(MobilPoliceHasar))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult GetDamagePolicy([FromQuery] GetPolicyInput input)
        {
            MobilPoliceHasar police = _policyService.GetDamagePolicy(input.PolicyId);
            if (police == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(police);
            }
        }

        /// <summary>
        ///  MobilPoliceHasar kaydıekler, 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AddDamagePolicy")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404)]
        [Produces("application/json")]

        public IActionResult AddDamagePolicy([FromBody] DamagePolicyInput input)
        {
            AddDamagePolicyTemp addDamagePolicyTemp = selectDamageUrun(input);
            if (addDamagePolicyTemp == null)
            {
                return Ok(new Message() { message = "Hata Oluştur Lütfen Tekrar Deneyiniz", statusCode = MessageStatus.Error });
            }
            bool result = _policyService.AddDamagePolicy(addDamagePolicyTemp);
            if (!result)
            {
                return NotFound();
            }
            else
            {
                return Ok(new Message() { message = "Hasar Kaydı Alınmıştır", statusCode = MessageStatus.Success });
            }
        }

        /// <summary>
        ///  Police yenileme kaydı ekler, 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AddRenewPolicy")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult AddRenewPolicy([FromBody] MobilTeklifPoliceInput input)
        {
            var result = false;
            TVMDetay tvmDetay;
            int oncekiTvmKodu = input.AcenteUnvani != null ? (int)input.AcenteUnvani : 0;
            
            List<TVMDetay> tvmGrupList = null;
            switch (input.EmailType)
            {
                case 0:
                    tvmGrupList = _policyService.getTVMGrup(oncekiTvmKodu, "K");//kendi acentesi
                    break;
                case 1:
                    tvmGrupList = _policyService.getTVMGrup(oncekiTvmKodu, "Y");
                    break;
                default:
                    break;
            }

            foreach (var item in tvmGrupList)
            {
                _pdfPolicy = new PdfPolicy();
                _pdfPolicy.AcenteUnvani = item.Unvani;
                _pdfPolicy.AcenteLogo = item.Logo;
                input.AcenteUnvani = item.Kodu;
                //_pdfPolicy.AcenteMail= item.Email;
                //_pdfPolicy.AcenteMail = "dogubey61@gmail.com";
                _pdfPolicy.AcenteMail = "mehmet.ust@neosinerji.com.tr";
                //manuel girişleri için görüşülecek

                addDamagePolicyTemp = selectPolicyUrun(input, 2);

                if (addDamagePolicyTemp.MobilTeklifPolice == null)
                {
                    return Ok(new Message() { message = "Hata Oluştu Lütfen Tekrar Deneyiniz", statusCode = MessageStatus.Error });
                }
                addDamagePolicyTemp.MobilTeklifPolice.TeklifPolice = 1;
                addDamagePolicyTemp.MobilTeklifPolice.TeklifTipi = input.EmailType == 1 ? "Y" : null;
                addDamagePolicyTemp.MobilTeklifPolice.OncekiTVM_Kodu = input.EmailType == 1 ? oncekiTvmKodu : 0;
                try
                {
                    var res = _policyService.AddRenewPolicy(addDamagePolicyTemp);
                    _pdfPolicy.PoliceId = res.Id;
                    _pdfPolicy.Token = new UtilityService().createToken(_pdfPolicy.KimlikNo, _appSettings.Secret);
                    messageOturum = new MobilMessageOturum();
                    messageOturum.PoliceId = res.Id;
                    messageOturum.AcenteKodu = res.AcenteUnvani;
                    messageOturum.Cevap_Suresi = DateTime.Now.AddHours(1);
                    messageOturum.PoliceTip = 0;
                    messageOturum.Token = _pdfPolicy.Token;
                    messageOturum.KullaniciId = (int)input.KullaniciId;
                    _policyService.addMessageOturum(messageOturum);
                    sendingMail();
                    _policyService.setYenilemTeklifAdedi(item.Kodu);
                    result = true;
                }
                catch (Exception)
                { }
            }

            if (!result)
            {
                return NotFound();
            }
            else
            {
                return Ok(new Message() { message = "Yenileme Talebi İletilmiştir.Mesajınız Maksimum 3 Saat İçerisinde Cevaplanacaktır.", statusCode = MessageStatus.Success });
            }

        }

        /// <summary>
        ///  teklif al  kaydı ekler, 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AddGetOfferPolicy")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult AddGetOfferPolicy([FromBody] MobilTeklifPoliceInput input)
        {
            bool result = false;
            TVMDetay tvmDetay; 
            int acentekodu = input.AcenteUnvani != null ? (int)input.AcenteUnvani : 0;
            List<TVMDetay> tvmGrupList = _policyService.getTVMGrup(acentekodu, "T");
            foreach (var item in tvmGrupList)
            {
                _pdfPolicy = new PdfPolicy();
                _pdfPolicy.AcenteUnvani = item.Unvani;
                _pdfPolicy.AcenteLogo = item.Logo;
                input.AcenteUnvani = item.Kodu;
                //_pdfPolicy.AcenteMail= item.Email;
                //_pdfPolicy.AcenteMail = "dogubey61@gmail.com";
                _pdfPolicy.AcenteMail = "mehmet.ust@neosinerji.com.tr";

                addDamagePolicyTemp = selectPolicyUrun(input, 0);

                if (addDamagePolicyTemp.MobilTeklifPolice == null)
                {
                    return Ok(new Message() { message = "Hata Oluştur Lütfen Tekrar Deneyiniz", statusCode = MessageStatus.Error });
                }
                addDamagePolicyTemp.MobilTeklifPolice.TeklifPolice = 1;
                addDamagePolicyTemp.MobilTeklifPolice.TeklifTipi ="T" ; 
                try
                {
                    var res = _policyService.AddRenewPolicy(addDamagePolicyTemp);
                    _pdfPolicy.PoliceId = res.Id;
                    _pdfPolicy.Token = new UtilityService().createToken(_pdfPolicy.KimlikNo, _appSettings.Secret);
                    messageOturum = new MobilMessageOturum();
                    messageOturum.PoliceId = res.Id;
                    messageOturum.AcenteKodu = res.AcenteUnvani;
                    messageOturum.Cevap_Suresi = DateTime.Now.AddHours(1);
                    messageOturum.PoliceTip = 0;
                    messageOturum.Token = _pdfPolicy.Token;
                    messageOturum.KullaniciId = (int)input.KullaniciId;
                    _policyService.addMessageOturum(messageOturum);
                    sendingMail();
                    _policyService.setYenilemTeklifAdedi(item.Kodu,false);
                    result = true;
                }
                catch (Exception)
                { }
            }
             
            if (!result)
            {
                return NotFound();

            }
            else
            {
                return Ok(new Message() { message = "Teklif Talebi İletilmiştir.Mesajınız Maksimum 3 Saat İçerisinde Cevaplanacaktır.", statusCode = MessageStatus.Success });
            }
        }


        /// <summary>
        ///  Manuel Poliçe kaydı ekler, 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AddPolicy")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404)]
        [Produces("application/json")]
        public IActionResult AddPolicy([FromBody] MobilTeklifPoliceInput input)
        {
            MobilTeklifPolice mobilTeklifPolice = selectPolicyUrun(input, 1).MobilTeklifPolice;
            mobilTeklifPolice.TeklifPolice = 0;
            if (mobilTeklifPolice == null)
            {
                return Ok(new Message() { message = "Hata Oluştur Lütfen Tekrar Deneyiniz", statusCode = MessageStatus.Error });
            }
            if (_policyService.checkPolicy(mobilTeklifPolice))
                return Ok(new Message() { message = "Bu poliçeyi daha önce eklediniz.", statusCode = MessageStatus.Error });
            bool result = _policyService.AddPolicy(mobilTeklifPolice);
            if (!result)
            {
                return NotFound();
            }
            else
            {
                return Ok(new Message() { message = "Poliçeniz Kaydedilmiştir.", statusCode = MessageStatus.Success });
            }
        }

        /// <summary>
        ///  0:yenileme, 1:hasar, 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        AddDamagePolicyTemp selectDamageUrun(DamagePolicyInput damagePolicyInput)
        {
            addDamagePolicyTemp = new AddDamagePolicyTemp();

            addDamagePolicyTemp.MobilPoliceHasar.KimlikNo = damagePolicyInput.KimlikNo;
            addDamagePolicyTemp.MobilPoliceHasar.BransKodu = damagePolicyInput.BransKodu;
            addDamagePolicyTemp.MobilPoliceHasar.Aciklama = damagePolicyInput.Aciklama;
            addDamagePolicyTemp.MobilPoliceHasar.SirketKodu = damagePolicyInput.SirketKodu;
            addDamagePolicyTemp.MobilPoliceHasar.PoliceNumarasi = damagePolicyInput.PoliceNumarasi;
            addDamagePolicyTemp.MobilPoliceHasar.YenilemeNo = damagePolicyInput.YenilemeNo != null ? damagePolicyInput.YenilemeNo : 0;
            addDamagePolicyTemp.MobilPoliceHasar.AcenteUnvani = damagePolicyInput.AcenteUnvani;
            addDamagePolicyTemp.MobilPoliceHasar.KoordinatX = damagePolicyInput.KoordinatX;
            addDamagePolicyTemp.MobilPoliceHasar.KoordinatY = damagePolicyInput.KoordinatY;
            addDamagePolicyTemp.MobilPoliceHasar.TeklifIslemNo = _policyService.getHasarTalepNo(damagePolicyInput.KimlikNo);
            damagePolicyInput.TeklifIslemNo = addDamagePolicyTemp.MobilPoliceHasar.TeklifIslemNo;
            if (damagePolicyInput.BransKodu != 21)
            {
                addDamagePolicyTemp.MobilPoliceHasar.BaslangicTarihi = damagePolicyInput.BaslangicTarihi;
                addDamagePolicyTemp.MobilPoliceHasar.BitisTarihi = damagePolicyInput.BitisTarihi;
            }
            else
            {
                addDamagePolicyTemp.MobilPoliceHasar.SeyahatGidisTarihi = damagePolicyInput.SeyahatGidisTarihi;
                addDamagePolicyTemp.MobilPoliceHasar.SeyahatDonusTarihi = damagePolicyInput.SeyahatDonusTarihi;
            }

            switch (damagePolicyInput.BransKodu)
            {
                case 1:
                case 2:
                    addDamagePolicyTemp.MobilPoliceHasar.Plaka = damagePolicyInput.Plaka.ToUpper();
                    addDamagePolicyTemp.MobilPoliceHasar.RuhsatSeriKodu = damagePolicyInput.RuhsatSeriKodu.ToUpper();
                    addDamagePolicyTemp.MobilPoliceHasar.RuhsatSeriNo = damagePolicyInput.RuhsatSeriNo;
                    addDamagePolicyTemp.MobilPoliceHasar.AsbisNo = damagePolicyInput.AsbisNo;
                    addDamagePolicyTemp.MobilPoliceHasar.ModelYili = damagePolicyInput.ModelYili;
                    addDamagePolicyTemp.MobilPoliceHasar.MarkaKodu = damagePolicyInput.MarkaKodu;
                    addDamagePolicyTemp.MobilPoliceHasar.TipKodu = damagePolicyInput.TipKodu;
                    addDamagePolicyTemp.MobilPoliceHasar.AracKullanimTarzi = damagePolicyInput.AracKullanimTarzi;

                    break;//arac
                case 4:
                    addDamagePolicyTemp.MobilPoliceHasar.Meslek = damagePolicyInput.Meslek;
                    break;//saglik
                case 11:
                    addDamagePolicyTemp.MobilPoliceHasar.BinaKatSayisi = damagePolicyInput.BinaKatSayisi;
                    addDamagePolicyTemp.MobilPoliceHasar.BinaKullanimSekli = damagePolicyInput.BinaKullanimSekli;
                    addDamagePolicyTemp.MobilPoliceHasar.BinaYapimYili = damagePolicyInput.BinaYapimYili;
                    addDamagePolicyTemp.MobilPoliceHasar.BinaYapiTarzi = damagePolicyInput.BinaYapiTarzi;
                    addDamagePolicyTemp.MobilPoliceHasar.DaireBrut = damagePolicyInput.DaireBrut;
                    addDamagePolicyTemp.MobilPoliceHasar.Adres = damagePolicyInput.Adres;
                    addDamagePolicyTemp.MobilPoliceHasar.IlKodu = damagePolicyInput.IlKodu;
                    addDamagePolicyTemp.MobilPoliceHasar.IlceKodu = damagePolicyInput.IlceKodu;
                    break;//dask
                case 21:
                    addDamagePolicyTemp.MobilPoliceHasar.SeyahatEdenKisiSayisi = damagePolicyInput.SeyahatEdenKisiSayisi;
                    addDamagePolicyTemp.MobilPoliceHasar.SeyahatUlkeKodu = damagePolicyInput.SeyahatUlkeKodu;
                    break;//seyahat
                case 22:
                    addDamagePolicyTemp.MobilPoliceHasar.BinaBedeli = damagePolicyInput.BinaBedeli;
                    addDamagePolicyTemp.MobilPoliceHasar.EsyaBedeli = damagePolicyInput.EsyaBedeli;
                    addDamagePolicyTemp.MobilPoliceHasar.Adres = damagePolicyInput.Adres;
                    addDamagePolicyTemp.MobilPoliceHasar.IlKodu = damagePolicyInput.IlKodu;
                    addDamagePolicyTemp.MobilPoliceHasar.IlceKodu = damagePolicyInput.IlceKodu;

                    break;//konut

                default:
                    addDamagePolicyTemp.MobilPoliceHasar.Adres = damagePolicyInput.Adres;
                    addDamagePolicyTemp.MobilPoliceHasar.IlKodu = damagePolicyInput.IlKodu;
                    addDamagePolicyTemp.MobilPoliceHasar.IlceKodu = damagePolicyInput.IlceKodu;
                    break;//diğer
            }
            mailFilePathList = new List<string>();


            if (damagePolicyInput.ResimDosyaList != null)
                if (damagePolicyInput.ResimDosyaList.Count > 0)
                    foreach (var item in damagePolicyInput.ResimDosyaList)
                    {
                        mobilPoliceDosya = new MobilPoliceDosya();
                        mobilPoliceDosya.DosyaTipi = "image";
                        tempfilename = new UtilityService(0).fileUrl(item);
                        mobilPoliceDosya.DosyaUrl = tempfilename[0];
                        mailFilePathList.Add(tempfilename[1]);
                        addDamagePolicyTemp.mobilPoliceDosyaList.Add(mobilPoliceDosya);

                    }
            if (damagePolicyInput.SesDosyaList != null)
                if (damagePolicyInput.SesDosyaList.Count > 0)
                    foreach (var item in damagePolicyInput.SesDosyaList)
                    {
                        mobilPoliceDosya = new MobilPoliceDosya();
                        mobilPoliceDosya.DosyaTipi = "sound";
                        tempfilename = new UtilityService(1).fileUrl(item);
                        mobilPoliceDosya.DosyaUrl = tempfilename[0];
                        mailFilePathList.Add(tempfilename[1]);
                        addDamagePolicyTemp.mobilPoliceDosyaList.Add(mobilPoliceDosya);
                    }
            try
            {
                sendMail(damagePolicyInput, 1);
            }
            catch (Exception)
            {
            }

            return addDamagePolicyTemp;
        }

        /// <summary>
        ///  processType->2:yenileme işlemi(mail gönderim işlemi),0:teklif al işlemi(mail gönderim işlemi),1:poliçe ekleme, 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AddDamagePolicyTemp selectPolicyUrun(MobilTeklifPoliceInput mobilTeklifPoliceInput, byte processType)
        {
            addDamagePolicyTemp = new AddDamagePolicyTemp();
            MobilTeklifPolice mobilTeklifPolice = new MobilTeklifPolice();

            mobilTeklifPolice.KimlikNo = mobilTeklifPoliceInput.KimlikNo;
            mobilTeklifPolice.BransKodu = mobilTeklifPoliceInput.BransKodu;
            mobilTeklifPolice.Aciklama = mobilTeklifPoliceInput.Aciklama;
            mobilTeklifPolice.SirketKodu = mobilTeklifPoliceInput.SirketKodu;
            mobilTeklifPolice.PoliceNumarasi = mobilTeklifPoliceInput.PoliceNumarasi;
            mobilTeklifPolice.YenilemeNo = mobilTeklifPoliceInput.YenilemeNo != null ? mobilTeklifPoliceInput.YenilemeNo : 0;
            mobilTeklifPolice.AcenteUnvani = mobilTeklifPoliceInput.AcenteUnvani;
            mobilTeklifPolice.TeklifIslemNo = _policyService.getYenileTalepNo(mobilTeklifPoliceInput.KimlikNo);

            mobilTeklifPoliceInput.TeklifIslemNo = mobilTeklifPolice.TeklifIslemNo;
            if (mobilTeklifPoliceInput.BransKodu != 21)
            {
                mobilTeklifPolice.BaslangicTarihi = mobilTeklifPoliceInput.BaslangicTarihi;
                mobilTeklifPolice.BitisTarihi = mobilTeklifPoliceInput.BitisTarihi;
            }
            else
            {
                mobilTeklifPolice.SeyahatGidisTarihi = mobilTeklifPoliceInput.SeyahatGidisTarihi;
                mobilTeklifPolice.SeyahatDonusTarihi = mobilTeklifPoliceInput.SeyahatDonusTarihi;
            }

            switch (mobilTeklifPoliceInput.BransKodu)
            {
                case 1:
                case 2://arac 
                    mobilTeklifPolice.Plaka = mobilTeklifPoliceInput.Plaka.ToUpper();
                    mobilTeklifPolice.RuhsatSeriKodu = mobilTeklifPoliceInput.RuhsatSeriKodu.ToUpper();
                    mobilTeklifPolice.RuhsatSeriNo = mobilTeklifPoliceInput.RuhsatSeriNo;
                    mobilTeklifPolice.AsbisNo = mobilTeklifPoliceInput.AsbisNo;
                    mobilTeklifPolice.ModelYili = mobilTeklifPoliceInput.ModelYili;
                    mobilTeklifPolice.MarkaKodu = mobilTeklifPoliceInput.MarkaKodu;
                    mobilTeklifPolice.TipKodu = mobilTeklifPoliceInput.TipKodu;
                    mobilTeklifPolice.AracKullanimTarzi = mobilTeklifPoliceInput.AracKullanimTarzi;
                    break;//arac
                case 4:
                    mobilTeklifPolice.Meslek = mobilTeklifPoliceInput.Meslek;
                    break;//saglik
                case 11:
                    mobilTeklifPolice.BinaKatSayisi = mobilTeklifPoliceInput.BinaKatSayisi;
                    mobilTeklifPolice.BinaKullanimSekli = mobilTeklifPoliceInput.BinaKullanimSekli;
                    mobilTeklifPolice.BinaYapimYili = mobilTeklifPoliceInput.BinaYapimYili;
                    mobilTeklifPolice.BinaYapiTarzi = mobilTeklifPoliceInput.BinaYapiTarzi;
                    mobilTeklifPolice.DaireBrut = mobilTeklifPoliceInput.DaireBrut;
                    mobilTeklifPolice.Adres = mobilTeklifPoliceInput.Adres;
                    mobilTeklifPolice.IlKodu = mobilTeklifPoliceInput.IlKodu;
                    mobilTeklifPolice.IlceKodu = mobilTeklifPoliceInput.IlceKodu;
                    break;//dask
                case 21:
                    mobilTeklifPolice.SeyahatEdenKisiSayisi = mobilTeklifPoliceInput.SeyahatEdenKisiSayisi;
                    mobilTeklifPolice.SeyahatUlkeKodu = mobilTeklifPoliceInput.SeyahatUlkeKodu;
                    break;//seyahat
                case 22:
                    mobilTeklifPolice.BinaBedeli = mobilTeklifPoliceInput.BinaBedeli;
                    mobilTeklifPolice.EsyaBedeli = mobilTeklifPoliceInput.EsyaBedeli;
                    mobilTeklifPolice.Adres = mobilTeklifPoliceInput.Adres;
                    mobilTeklifPolice.IlKodu = mobilTeklifPoliceInput.IlKodu;
                    mobilTeklifPolice.IlceKodu = mobilTeklifPoliceInput.IlceKodu;

                    break;//konut

                default:
                    mobilTeklifPolice.Adres = mobilTeklifPoliceInput.Adres;
                    mobilTeklifPolice.IlKodu = mobilTeklifPoliceInput.IlKodu;
                    mobilTeklifPolice.IlceKodu = mobilTeklifPoliceInput.IlceKodu;
                    break;//diğer
            }
            addDamagePolicyTemp.MobilTeklifPolice = mobilTeklifPolice;

            if (processType != 1)
            {
                createMailData(mobilTeklifPoliceInput, processType);
                if (mobilTeklifPolice.AcenteUnvani == null)
                    mobilTeklifPolice.AcenteUnvani = mobilTeklifPoliceInput.AcenteUnvani;
            }
            return addDamagePolicyTemp;
        }

        /// <summary>
        /// Request Body ile gönderilen JSON data içerisinde Id'si eşleşen MobilTeklifPolice kaydının MarkaKodu, ModelYili, TipKodu ve AracKullanimTarzi alanlarını günceller.
        /// Eğer eşleşen kayıt yoksa 404 Not Found hatası verir.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("UpdateCarInfo")]
        [SwaggerResponse(200, Type = typeof(OkResult))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        public IActionResult UpdateCarInfo([FromBody] UpdateCarInfoInput input)
        {
            MobilTeklifPolice police = _policyService.GetPolicy(input.PolicyId);
            if (police == null)
            {
                return NotFound();
            }
            else
            {
                police.MarkaKodu = input.MarkaKodu;
                police.ModelYili = input.ModelYili;
                police.TipKodu = input.TipKodu;
                police.AracKullanimTarzi = input.AracKullanimTarzi;
                _policyService.UpdateCarInfo(police);

                return Ok();
            }
        }
        void createMailData(MobilTeklifPoliceInput mobilTeklifPoliceInput, byte processType)//0:teklif al, 2:yenileme
        {

            //_pdfPolicy = new PdfPolicy();
            _pdfPolicy.isAcente = true;
            _pdfPolicy.processType = processType;
            _pdfPolicy.BransKodu = (int)mobilTeklifPoliceInput.BransKodu;
            _pdfPolicy.TeklifIslemNo = mobilTeklifPoliceInput.TeklifIslemNo;
            SigortaSirketleri sirketData = null;
            // TVMDetay tvmDetay = null;
            //_pdfPolicy.AcenteUnvani = "";
            //_pdfPolicy.AcenteLogo = "";
            sirketData = _policyService.getSirket(mobilTeklifPoliceInput.SirketKodu);
            _pdfPolicy.Sirket = sirketData.SirketAdi;
            _pdfPolicy.SirketLogo = sirketData.SirketLogo;

            //if (processType == 0)//koşul belirlenince değişecek
            //{
            //    tvmDetay = _policyService.getAcente(mobilTeklifPoliceInput.AcenteUnvani != null ? (int)mobilTeklifPoliceInput.AcenteUnvani : -1);
            //    _pdfPolicy.AcenteUnvani = tvmDetay.Unvani;
            //    _pdfPolicy.AcenteLogo = tvmDetay.Logo;
            //    mobilTeklifPoliceInput.AcenteUnvani = tvmDetay.Kodu;

            //    //_pdfPolicy.AcenteMail= tvmDetay.Email;
            //    _pdfPolicy.AcenteMail = "dogubey61@gmail.com";
            //}
            //else
            //{
            //    switch (mobilTeklifPoliceInput.EmailType)
            //    {
            //        case 0:
            //            tvmDetay = _policyService.getAcente(mobilTeklifPoliceInput.AcenteUnvani != null ? (int)mobilTeklifPoliceInput.AcenteUnvani : -1);
            //            break;
            //        case 1:
            //            tvmDetay = _policyService.getAcente(-1);
            //            break;
            //        default:
            //            tvmDetay = _policyService.getAcente(mobilTeklifPoliceInput.AcenteUnvani != null ? (int)mobilTeklifPoliceInput.AcenteUnvani : -1);
            //            break;
            //    }
            //    _pdfPolicy.AcenteUnvani = tvmDetay.Unvani;
            //    _pdfPolicy.AcenteLogo = tvmDetay.Logo;
            //    mobilTeklifPoliceInput.AcenteUnvani = tvmDetay.Kodu; 
            //    //_pdfPolicy.AcenteMail= tvmDetay.Email;
            //    _pdfPolicy.AcenteMail = "dogubey61@gmail.com";
            //}

            _pdfPolicy.BransAdi = mobilTeklifPoliceInput.BransKodu != null ? _policyService.getBransAdi((int)mobilTeklifPoliceInput.BransKodu) : "Bulunamadı";
            _pdfPolicy.PoliceNumarasi = mobilTeklifPoliceInput.PoliceNumarasi != null ? mobilTeklifPoliceInput.PoliceNumarasi : "";
            _pdfPolicy.YenilemeNo = mobilTeklifPoliceInput.YenilemeNo != null ? (int)mobilTeklifPoliceInput.YenilemeNo : 0;
            if (mobilTeklifPoliceInput.BransKodu != 21)
            {
                _pdfPolicy.BaslangicTarihi = mobilTeklifPoliceInput.BaslangicTarihi != null ? UtilityService.EditDateTime(mobilTeklifPoliceInput.BaslangicTarihi.Value.ToShortDateString()) : "";
                _pdfPolicy.BitisTarihi = mobilTeklifPoliceInput.BitisTarihi != null ? UtilityService.EditDateTime(mobilTeklifPoliceInput.BitisTarihi.Value.ToShortDateString()) : "";
            }
            else
            {
                _pdfPolicy.SeyahatGidisTarihi = mobilTeklifPoliceInput.SeyahatGidisTarihi != null ? UtilityService.EditDateTime(mobilTeklifPoliceInput.SeyahatGidisTarihi.Value.ToShortDateString()) : "";
                _pdfPolicy.SeyahatDonusTarihi = mobilTeklifPoliceInput.SeyahatDonusTarihi != null ? UtilityService.EditDateTime(mobilTeklifPoliceInput.SeyahatDonusTarihi.Value.ToShortDateString()) : "";
            }

            _pdfPolicy.KimlikNo = mobilTeklifPoliceInput.KimlikNo;
            kullanici = _policyService.getKullanici(mobilTeklifPoliceInput.KimlikNo);
            _pdfPolicy.AdSoyad = kullanici.Adsoyad;
            selectPdfUrun(mobilTeklifPoliceInput);

            _pdfPolicy.Konum = "";
            _pdfPolicy.Aciklama = mobilTeklifPoliceInput.Aciklama;

            tempfilename = new UtilityService(2).createPdf(_pdfPolicy, processType);
            mobilPoliceDosya = new MobilPoliceDosya();
            mobilPoliceDosya.DosyaTipi = "pdf";
            mobilPoliceDosya.DosyaUrl = tempfilename[0];
            addDamagePolicyTemp.mobilPoliceDosyaList.Add(mobilPoliceDosya);

            att[0] = new Attachment(tempfilename[1]);
            att[1] = new Attachment(tempfilename[1]);

        }
        void sendingMail()//yenileme -teklif
        {

            mailFile = new List<Attachment>();
            mailFile.Add(att[0]);

            string subject = "Sigortadefterim - " + _pdfPolicy.BransAdi + " Sigortası " + (_pdfPolicy.processType == 0 ? "Teklif Talebi - Talep No : " : _pdfPolicy.processType == 1 ? "Hasar Bildirimi" : "Yenileme Talebi - Talep No : ") + _pdfPolicy.TeklifIslemNo;

            string body = "    Sayın " + (!string.IsNullOrEmpty(_pdfPolicy.AcenteUnvani) ? _pdfPolicy.AcenteUnvani : _pdfPolicy.Sirket) + ", SİGORTADEFTERİM.com kullanıcısı tarafından ekte detayları bulunan " + (_pdfPolicy.processType == 0 ? "Teklif Talebini" : _pdfPolicy.processType == 1 ? "Hasar Bildirimini" : "Yenileme Talebini") + " acilen değerlendirerek, " + (_pdfPolicy.processType == 1 ? "gerekli işlemlerin başlatılması hususunu" : " farklı sigorta şirketlerinden alınmış tekliflerinizi aşağıdaki linke tıklayarak ilgili uygulama üzerinden göndermenizi");
            body += " önemle bilgilerinize sunarız.<br>";

            body += UtilityService.ReadFromFile(Directory.GetCurrentDirectory() + "\\Files\\MailBody\\teklifmail.txt");
            body += "Teklifinizi iletmek için lütfen linke tıklayınız : https://sigortadefterimv2api.azurewebsites.net/api/MobileApp/CompanyMessageView?token=" + _pdfPolicy.Token;
            body += "<br><br>Saygılarımızla<br>SİGORTADEFTERİM.com";


            string body2 = "    Sayın " + kullanici.Adsoyad + ", talebiniz acenteye iletilmiştir.  Ekte detayları bulunan " + (_pdfPolicy.processType == 0 ? "Teklif Talebiniz" : "Yenileme Talebiniz") + " acente tarafından en kısa sürede değerlendirilerek  farklı sigorta şirketlerinden alınmış teklifleri tarafınıza uygulamadaki \"MESAJLARIM\" bölümünden iletecektir. Lütfen aralıklarla mesajlarınızı kontrol ediniz.<br><br>Saygılarımızla<br><br>SİGORTADEFTERİM.com";
            //şimdilik kendi acentesi varsa kendi acentesine yoksa skor 100 olan bir kaç acenteye mail gidiyor burası daha sonra düzeltilecek
            UtilityService.SendEmail("", subject, body, mailFile, true, new List<MobilAcente>() { new MobilAcente() { TVMUnvani = _pdfPolicy.AcenteUnvani, BildirimEmail = _pdfPolicy.AcenteMail } }, /*mobilTeklifPoliceInput.EmailType*/1);//acente ve sigortadefterim.com a mail gonderir
            mailFile = new List<Attachment>();
            mailFile.Add(att[1]);
            UtilityService.SendEmail(kullanici.Eposta, subject, body2, mailFile, true, null, /*mobilTeklifPoliceInput.EmailType*/1);//kullanıcıya a mail gonderir


        }

        void sendMail(DamagePolicyInput damagePolicyInput, byte processType)
        {
            _pdfPolicy = new PdfPolicy();
            _pdfPolicy.BransKodu = (int)damagePolicyInput.BransKodu;
            _pdfPolicy.TeklifIslemNo = damagePolicyInput.TeklifIslemNo;
            SigortaSirketleri sirketData = null;
            TVMDetay tvmDetay = null;
            _pdfPolicy.AcenteUnvani = "";
            _pdfPolicy.AcenteLogo = "";
            if (damagePolicyInput.KullaniciGonderiTuru == 1)//koşul belirlenince değişecek //1 ise maili şirkete gonderir
            {
                _pdfPolicy.isAcente = false;
                sirketData = _policyService.getSirket(damagePolicyInput.SirketKodu);
                _pdfPolicy.Sirket = sirketData.SirketAdi;
                _pdfPolicy.SirketLogo = sirketData.SirketLogo;
                sirketData.Email = "dogubey61@gmail.com";
            }
            else
            {
                _pdfPolicy.isAcente = true;
                tvmDetay = _policyService.getAcente(damagePolicyInput.AcenteUnvani != null ? (int)damagePolicyInput.AcenteUnvani : -1);
                _pdfPolicy.AcenteUnvani = tvmDetay.Unvani;
                _pdfPolicy.AcenteLogo = tvmDetay.Logo;
                sirketData = _policyService.getSirket(damagePolicyInput.SirketKodu);
                _pdfPolicy.Sirket = sirketData.SirketAdi;
                _pdfPolicy.SirketLogo = sirketData.SirketLogo;
                sirketData.Email = "dogubey61@gmail.com";
            }

            _pdfPolicy.BransAdi = damagePolicyInput.BransKodu != null ? _policyService.getBransAdi((int)damagePolicyInput.BransKodu) : "Bulunamadı";
            _pdfPolicy.PoliceNumarasi = damagePolicyInput.PoliceNumarasi != null ? damagePolicyInput.PoliceNumarasi : "";
            _pdfPolicy.YenilemeNo = damagePolicyInput.YenilemeNo != null ? (int)damagePolicyInput.YenilemeNo : 0;
            if (damagePolicyInput.BransKodu != 21)
            {
                _pdfPolicy.BaslangicTarihi = damagePolicyInput.BaslangicTarihi != null ? UtilityService.EditDateTime(damagePolicyInput.BaslangicTarihi.Value.ToShortDateString()) : "";
                _pdfPolicy.BitisTarihi = damagePolicyInput.BitisTarihi != null ? UtilityService.EditDateTime(damagePolicyInput.BitisTarihi.Value.ToShortDateString()) : "";
            }
            else
            {
                _pdfPolicy.SeyahatGidisTarihi = damagePolicyInput.SeyahatGidisTarihi != null ? UtilityService.EditDateTime(damagePolicyInput.SeyahatGidisTarihi.Value.ToShortDateString()) : "";
                _pdfPolicy.SeyahatDonusTarihi = damagePolicyInput.SeyahatDonusTarihi != null ? UtilityService.EditDateTime(damagePolicyInput.SeyahatDonusTarihi.Value.ToShortDateString()) : "";
            }

            _pdfPolicy.KimlikNo = damagePolicyInput.KimlikNo;

            kullanici = _policyService.getKullanici(damagePolicyInput.KimlikNo);
            _pdfPolicy.AdSoyad = kullanici.Adsoyad;

            selectPdfUrun(damagePolicyInput);

            //https://www.google.com/maps/@41.0638393,28.9905254,14.5z
            _pdfPolicy.Konum = !string.IsNullOrEmpty(damagePolicyInput.KoordinatX) ? "https://www.google.com/maps/@" + damagePolicyInput.KoordinatX + "," + damagePolicyInput.KoordinatY + ",19z " : "";
            _pdfPolicy.Aciklama = damagePolicyInput.Aciklama;

            tempfilename = new UtilityService(2).createPdf(_pdfPolicy, processType);
            mobilPoliceDosya = new MobilPoliceDosya();
            mobilPoliceDosya.DosyaTipi = "pdf";
            mobilPoliceDosya.DosyaUrl = tempfilename[0];
            addDamagePolicyTemp.mobilPoliceDosyaList.Add(mobilPoliceDosya);
            mailFilePathList.Add(tempfilename[1]);

            mailFile = new List<Attachment>();
            foreach (var item in mailFilePathList)
            {
                att[0] = new Attachment(item);
                mailFile.Add(att[0]);
            }


            string body = "    Sayın " + (!string.IsNullOrEmpty(_pdfPolicy.AcenteUnvani) ? _pdfPolicy.AcenteUnvani : _pdfPolicy.Sirket) + ", SİGORTADEFTERİM.com kullanıcısı tarafından ekte detayları bulunan " + (processType == 0 ? "Teklif Talebini" : processType == 1 ? "Hasar Bildirimini" : "Yenileme Talebini") + " acilen değerlendirerek, " + (processType == 1 ? "gerekli işlemlerin başlatılması hususunu" : "kullanıcı e-posta adresine farklı sigorta şirketlerinden alınmış tekliflerinizi göndermenizi") + " önemle bilgilerinize sunarız.";
            body += "<br><br>Saygılarımızla<br><br>SİGORTADEFTERİM.com";
            body += "<br><br>Konum: " + (!string.IsNullOrEmpty(_pdfPolicy.Konum) ? _pdfPolicy.Konum : "Konum Bilgisi Bulunamadı.");
            string subject = "Sigortadefterim - " + _pdfPolicy.BransAdi + " Sigortası " + (processType == 0 ? "Teklif Talebi" : processType == 1 ? "Hasar Bildirimi - Talep No : " + _pdfPolicy.TeklifIslemNo : "Yenileme Talebi");
            UtilityService.SendEmail("", subject, body, mailFile, true, damagePolicyInput.KullaniciGonderiTuru != 1 ? _policyService.GetAcenteMailList() : new List<MobilAcente>() { new MobilAcente() { TVMUnvani = sirketData.SirketAdi, BildirimEmail = sirketData.Email } }, 1); //acentelerde düzenleme yapılacak bi tane acentes olması lazım simdilik skor 100 olanlara gidiyor

            mailFiletemp = new List<Attachment>();
            foreach (var item in mailFilePathList)
            {
                att[0] = new Attachment(item);
                mailFiletemp.Add(att[0]);
            }

            //string body2 = "    Sayın " + kullanici.Adsoyad + ", talebiniz acenteye iletilmiştir.  Ekte detayları bulunan " + (_pdfPolicy.processType == 0 ? "Tekli f Talebiniz" : "Yenileme Talebiniz") + " acente tarafından en kısa sürede değerlendirilerek  farklı sigorta şirketlerinden alınmış teklifleri tarafınıza uygulamadaki \"MESAJLARIM\" bölümünden iletecektir. Lütfen aralıklarla mesajlarınızı kontrol ediniz.<br><br>Saygılarımızla<br><br>SİGORTADEFTERİM.com";
            string body2 = "    Sayın " + kullanici.Adsoyad + ",Ekte detayları bulunan Hasar Bildiriminiz, gerekli işlemlerin başlatılması için " + (!string.IsNullOrEmpty(_pdfPolicy.AcenteUnvani) ? _pdfPolicy.AcenteUnvani : _pdfPolicy.Sirket) + "  iletilmiştir.Önemle bilgilerinize sunarız.<br><br>Saygılarımızla<br><br>SİGORTADEFTERİM.com";
            //kullanıcıya a mail gonderir
            //Sayın Ad Soyad, Ekte detayları bulunan Hasar Bildiriminiz, gerekli işlemlerin başlatılması için  Acente yada Sig. Şirket Adı 'ya  iletilmiştir. 
            //            Bilgi almak için SİGORTADEFTERİM uygulamasında ki 'MESAJLARIM' adımından iletişime geçebilirsiniz.Önemle bilgilerinize sunarız.

            UtilityService.SendEmail(kullanici.Eposta, subject, body2, mailFiletemp, true, null, 1);

        }

        [HttpGet("testfonksiyon")]
        //[Produces("text/html")]
        [Produces("application/json")]
        [AllowAnonymous]
        public IActionResult testfonksiyon(string test)
        {
            //_policyService.test(test)
            return Ok();

        }
        void selectPdfUrun(DamagePolicyInput damagePolicyInput)
        {
            switch (damagePolicyInput.BransKodu)
            {
                case 1:
                case 2://arac 
                    _pdfPolicy.Plaka = damagePolicyInput.Plaka;
                    _pdfPolicy.RuhsatSeriKodu = damagePolicyInput.RuhsatSeriKodu;
                    _pdfPolicy.RuhsatSeriNo = damagePolicyInput.RuhsatSeriNo;
                    _pdfPolicy.ModelYili = (int)damagePolicyInput.ModelYili;
                    string[] aracKullanimTarziList = damagePolicyInput.AracKullanimTarzi.Split('+');
                    _pdfPolicy.AracKullanimTarzi = _policyService.getAracKullanimTarzi(aracKullanimTarziList[0], aracKullanimTarziList[1]);
                    _pdfPolicy.Marka = _policyService.getAracMarka(damagePolicyInput.MarkaKodu);
                    _pdfPolicy.Tip = _policyService.getAracTipi(damagePolicyInput.TipKodu, damagePolicyInput.MarkaKodu);

                    break;//arac
                case 4:
                    _pdfPolicy.Meslek = _policyService.getMeslek(int.Parse(damagePolicyInput.Meslek));
                    break;//saglik
                case 11:
                    _pdfPolicy.BinaKatSayisi = (byte)damagePolicyInput.BinaKatSayisi;
                    _pdfPolicy.BinaKullanimSekli = (int)damagePolicyInput.BinaKullanimSekli;
                    _pdfPolicy.BinaYapimYili = (byte)damagePolicyInput.BinaYapimYili;
                    _pdfPolicy.BinaYapiTarzi = (byte)damagePolicyInput.BinaYapiTarzi;
                    _pdfPolicy.DaireBrut = (int)damagePolicyInput.DaireBrut;
                    _pdfPolicy.Adres = damagePolicyInput.Adres;
                    _pdfPolicy.Il = _policyService.getIlAdi(damagePolicyInput.IlKodu);
                    _pdfPolicy.Ilce = _policyService.getIlceAdi((int)damagePolicyInput.IlceKodu);
                    break;//dask
                case 21:
                    _pdfPolicy.SeyahatEdenKisiSayisi = (int)damagePolicyInput.SeyahatEdenKisiSayisi;
                    _pdfPolicy.SeyahatUlke = _policyService.getUlke(damagePolicyInput.SeyahatUlkeKodu);
                    _pdfPolicy.SeyahatUlkeTipi = _policyService.getUlkeTipi(damagePolicyInput.SeyahatUlkeKodu);

                    break;//seyahat
                case 22:
                    _pdfPolicy.BinaBedeli = (decimal)damagePolicyInput.BinaBedeli;
                    _pdfPolicy.EsyaBedeli = (decimal)damagePolicyInput.EsyaBedeli;
                    _pdfPolicy.Adres = damagePolicyInput.Adres;
                    _pdfPolicy.Il = _policyService.getIlAdi(damagePolicyInput.IlKodu);
                    _pdfPolicy.Ilce = _policyService.getIlceAdi((int)damagePolicyInput.IlceKodu);

                    break;//konut

                default:
                    _pdfPolicy.Adres = damagePolicyInput.Adres;
                    _pdfPolicy.Il = _policyService.getIlAdi(damagePolicyInput.IlKodu);
                    _pdfPolicy.Ilce = _policyService.getIlceAdi((int)damagePolicyInput.IlceKodu);
                    break;//diğer
            }
        }
    }
}