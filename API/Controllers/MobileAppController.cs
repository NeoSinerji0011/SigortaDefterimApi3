using API.Models.Database;
using API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SigortaDefterimV2API.Helpers;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Models.Database;
using SigortaDefterimV2API.Models.Responses;
using SigortaDefterimV2API.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MobileAppController : ControllerBase
    {
        private MobileAppService _mobileAppService;
        private readonly AppSettings _appSettings;
        public MobileAppController(MobileAppService mobileAppService, IOptions<AppSettings> appSettings)
        {
            _mobileAppService = mobileAppService;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Mobil uygulama başlangıcında kullanıcıya gösterilmek üzere aktif açılış aktif ekranlarını liste halinde döndürür.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetLandingPages")]
        [SwaggerResponse(200, Type = typeof(List<MobilKarsilamaEkrani>))]
        [Produces("application/json")]
        public IActionResult GetLandingPages()
        {
            return Ok(_mobileAppService.GetLandingPages());
        }

        /// <summary>
        /// Mobil uygulama içerisindeki ana menüde gösterilen aktif kaydırılabilen içerikleri liste halinde döndürür.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetSliderPages")]
        [SwaggerResponse(200, Type = typeof(List<MobilKaydirmaEkrani>))]
        [Produces("application/json")]
        public IActionResult GetSliderPages()
        {
            return Ok(_mobileAppService.GetLandingPages());
        }

        /// <summary>
        /// Seyahat Poliçesi ülke tip ve ülke isimleri listesi
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetGlobalDataList")]
        [SwaggerResponse(200, Type = typeof(MobilGlobalDataList))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetGlobalDataList([FromBody] Kullanici Kullanici)
        {
            return Ok(_mobileAppService.GetMobilGlobalList(Kullanici.Tc));
        }
        /// <summary>
        /// Arac  tip verilerini getirir
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetAracTipList")]
        [SwaggerResponse(200, Type = typeof(List<AracTip>))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetAracTipList([FromBody] AracMarka aracMarka)
        {
            return Ok(_mobileAppService.GetAracTipList(aracMarka.MarkaKodu));
        }
        /// <summary>
        /// Brans koduna göre detayı getirir
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetTVMDetay")]
        [SwaggerResponse(200, Type = typeof(TVMDetay))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetTVMDetay([FromBody] TVMDetay tVMDetay)
        {
            return Ok(_mobileAppService.GetTVMDetay(tVMDetay.Kodu));
        }

        /// <summary>
        ///poliçe bildirim listesi
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetBildirim")]
        [SwaggerResponse(200, Type = typeof(BildirimResponse))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetBildirim([FromBody] Kullanici kullanici)
        {
            return Ok(_mobileAppService.GetBildirim(kullanici.Tc));
        }
        /// <summary>
        ///poliçenin bildirim sayfasındaki gorunurlugunu gizler
        /// </summary>
        /// <returns></returns> 
        [HttpPost("SetBildirimDisable")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult SetBildirimDisable([FromBody] Bildirim bildirim)
        {
            bool result = _mobileAppService.SetBildirimDisable(bildirim);
            return Ok(new Message() { message = result ? "Bildirim Silindi" : "Hata Oluştu Tekrar Deneyiniz", statusCode = result ? MessageStatus.Success : MessageStatus.Failed });
        }

        /// <summary>
        ///kullanıcınn mesajını firmaya gönderir.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("SetUserMessage")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult SetUserMessage([FromBody] MobilMessage oturum)
        {
            bool result = false;
            oturum.Tarih_Saat = DateTime.Now;
            oturum.Goruldumu = "0";
            oturum.Gonderici_Tip = "0";
            try
            {
                var mailData = _mobileAppService.getIletisimMailData(oturum.OturumId, false);
                userMessageMail(mailData);
                result = _mobileAppService.SetUserMessage(oturum);
            }
            catch (Exception)
            {
            }
            return Ok(new Message() { message = result ? "Mesajınız Gönderildi." : "Hata Oluştu Tekrar Deneyiniz.", statusCode = result ? MessageStatus.Success : MessageStatus.Failed });
        }
        void userMessageMail(CompanyMailData compnaymaildata)
        {

            //string konu = "Sigortadefterim Kullanıcı Mesajı";
            //string icerik = "Sayın "+ compnaymaildata.FirmaAdi+ ", SİGORTADEFTERİM.com kullanıcısı tarafından ekte detayları bulunan Teklif Talebini acilen değerlendirerek, kullanıcı e-posta adresine farklı sigorta şirketlerinden alınmış tekliflerinizi göndermenizi önemle bilgilerinize sunarız.<br>";
            //icerik += "Bu mesajı lütfen maksimum 1 saat saat içirisinde aşağıdaki linkte tıklayarak yanıtlamanız gerekmektedir.";
            //icerik += "Cevap vermek için linke tıklayınız: " + "https://sigortadefterimv2api.azurewebsites.net/api/MobileApp/CompanyMessageView?token="+ compnaymaildata.Token;

            //icerik += "<br><br>Saygılarımızla<br><br>SİGORTADEFTERİM.com<br><br>";

            string konu = "Sigortadefterim Kullanıcı Mesajı";
            //string icerik = "Sayın " + compnaymaildata.FirmaAdi + ", SİGORTADEFTERİM.com kullanıcısı tarafından gönderilen mesajı, haftaiçi 09:00 - 18:00 saatleri arasında maksimum 1 saat içerisinde aşağıdaki linke tıklayarak okuyup yanıtlamanız gerekmektedir. Cuma günü 18:00'den sonra gelen mesajlarınızı ise Pazartesi saat 10:00'a kadar yanıtlayabilirsiniz.<br><br>";
            string icerik = "Sayın " + compnaymaildata.FirmaAdi + ", ";
            icerik += UtilityService.ReadFromFile(Directory.GetCurrentDirectory() + "\\Files\\MailBody\\usermail.txt");
            icerik += "Mesajı okumak ve yanıtlamak için lütfen linke tıklayınız: " + "https://sigortadefterimv2api.azurewebsites.net/api/MobileApp/CompanyMessageView?token=" + compnaymaildata.Token;

            icerik += "<br><br>Saygılarımızla<br><br>SİGORTADEFTERİM.com<br><br>";

            UtilityService.SendEmail("dogubey61@gmail.com" /*compnaymaildata.FirmaAdi*/, konu, icerik, null, true);
        }

        /// <summary>
        ///kullanıcının bütün mesajlarını listeler.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetUserMessage")]
        [SwaggerResponse(200, Type = typeof(MobilIletisim))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetUserMessage([FromBody] Kullanici kullanici)
        {
            var res = _mobileAppService.GetUserMessage(kullanici);
            foreach (var item in res.iletisimResponse)
            {
                item.isActive = new UtilityService().checkToken(item.Token, _appSettings.Secret);
            }
            return Ok(res);
        }
        /// <summary>
        ///kullanıcıya gelen yeni mesaj sayısı.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetUserNewMessageCount")]
        [SwaggerResponse(200, Type = typeof(MobilIletisim))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult UserMessageCount([FromBody] Kullanici kullanici)
        { 
            return Ok(new Message { message = _mobileAppService.UserNewMessageCount(new MobilMessageOturum { KullaniciId = kullanici.Id }).ToString() });
        }
        /// <summary>
        ///kullanıcının bütün mesajlarını listeler.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("SetReadMessage")]
        [SwaggerResponse(200, Type = typeof(OkResult))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult SetReadMessage([FromBody] MobilMessage message)
        {
            var res = _mobileAppService.SetReadMessage(message);
            return Ok();
        }
        /// <summary>
        ///kullanıcıya yeni gelen mesajlarını listeler.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetUserNewMessage")]
        [SwaggerResponse(200, Type = typeof(MobilIletisim))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetUserNewMessage([FromBody] Kullanici kullanici)
        {
            return Ok(_mobileAppService.GetUserNewMessage(kullanici));
        }
        /// <summary>
        ///firmaya gelen mesajı gösterir.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetCompanyMessage")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        [Produces("application/json")]
        public IActionResult GetCompanyMessage()
        {
            var headers = Request.Headers.Values;
            string token = "";
            foreach (var item in headers)
            {
                if (item[0].Contains("Bearer"))
                {
                    token = item[0].Replace("Bearer ", "");
                    break;
                }
            }
            return Ok(_mobileAppService.GetCompanyMessage(token));
        }

        /// <summary>
        ///firma mesajını kullanıcıya gönderir.
        /// </summary>
        /// <returns></returns> 
        [HttpPost("companyMessage")]
        [SwaggerResponse(200, Type = typeof(Message))]
        [SwaggerResponse(404, Type = typeof(NotFoundResult))]
        public async Task<IActionResult> companyMessage()
        {

            var headers = Request.Headers.Values;
            string token = "";

            foreach (var item in headers)
            {
                if (item[0].Contains("Bearer"))
                {
                    token = item[0].Replace("Bearer ", "");
                    break;
                }
            }

            var fileList = Request.Form.Files;
            MobilMessage message = new MobilMessage();
            message.Mesaj = Request.Form["Message"];
            message.Gonderici_Tip = "1";
            message.OturumId = _mobileAppService.getSessionId(token);
            message.Tarih_Saat = DateTime.Now;
            message.Goruldumu = "0";
            List<string> fileNameList = new List<string>();
            string filename = "", filePath = "";
            FileStream stream;
            foreach (var formFile in fileList)
            {
                filename = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                fileNameList.Add(filename);
                filePath = Directory.GetCurrentDirectory() + "\\CompanyFiles\\" + filename;
                if (formFile.Length > 0)
                {
                    using (stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }


            bool result = _mobileAppService.SetCompanyMessage(message, fileNameList);
            return Ok(new Message() { message = result ? "Mesajınız Gönderildi." : "Hata Oluştu Tekrar Deneyiniz.", statusCode = result ? MessageStatus.Success : MessageStatus.Failed });

        }
        [AllowAnonymous]
        [HttpGet("CompanyMessageView")]
        [SwaggerResponse(200, Type = typeof(List<TVMDetay>))]
        [Produces("application/json")]
        public IActionResult CompanyMessageView()
        {
            var Result = new ViewResult
            {
                ViewName = "~/Views/Message.cshtml",
                ViewData = new ViewDataDictionary(
                                   metadataProvider: new EmptyModelMetadataProvider(),
                                   modelState: new ModelStateDictionary())
                {
                    Model = new Message { message = "", statusCode = MessageStatus.Success },
                },
            };

            return Result;
        }
       
        [AllowAnonymous]
        [HttpPost("test")]
        public async Task<IActionResult> test(/*MobilMessage message*/)
        {

            return Ok();
        }
    }
}