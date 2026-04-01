using API.Areas.MobilApi.Helper;
using API.Areas.MobilApi.Models.Database;
using API.Areas.MobilApi.Models.Input;
using API.Areas.MobilApi.Services;
using API.Areas.MobilApi.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Controllers
{
    public class MobilApiController : Controller
    {
        private SmsService _smsService;

        public MobilApiController(SmsService smsService)
        {
            _smsService = smsService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Json(new { Status = true });
        }
        [HttpGet]
        public IActionResult SmsTitles()
        {
            var res = JsonConvert.DeserializeObject<string[]>(Utils.ReadFile(Directory.GetCurrentDirectory() + "/areas/mobilapi/files/smstitles.json"));
            return Json(res);
        }
        [HttpPost]
        public IActionResult SmsReceiver([FromBody] SmsItem smsItem)
        {
            try
            {
                if (smsItem.fromPhone == "Hepiyi")
                {
                    smsItem.fromPhone = "HEPIYI";
                    smsItem.fromPhone2 = "HEPIYI";
                }
                _smsService.SmsIcerikYaz(smsItem);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
            }

            return Json(new { Status = 200, Result = true });
        }

        [HttpPost]
        public IActionResult GetSmsData2([FromBody] SmsItem smsItem)
        {
            if (smsItem.fromPhone == "Hepiyi")
            {
                smsItem.fromPhone = "HEPIYI";
                smsItem.fromPhone2 = "HEPIYI";
            }

            SmsFilter smsFilter = new SmsFilter(smsItem);
            smsFilter.SmsIsle2();

            return Json(smsFilter._resList);
        }
        [HttpPost]
        public IActionResult GetSmsDatabyTvmKodu([FromBody] TvmRequest tvmRequest)
        {
            return BadRequest();

        }
        [HttpPost]
        public IActionResult GetTumData([FromBody] TvmRequest tvmRequest)
        {
            var data = _smsService.SirketData(tvmRequest);
            return Json(data);
        }
        [HttpPost]
        public IActionResult GetTumList([FromBody] TvmRequest tvmRequest)
        {
            var data = _smsService.SirketKontrol(tvmRequest.TvmKodu);
            return Json(data);
        }
        [HttpPost]
        public IActionResult GetTvmList()
        {
            var data = _smsService.TvmKontrol();
            return Json(data);
        }
        [HttpPost]
        public IActionResult SirketSorlari([FromBody] TvmRequest tvmRequest)
        {
            var data = _smsService.SirketSorulari(tvmRequest.TvmKodu);
            return Json(data);
        }
        [HttpPost]
        public IActionResult AracMarka([FromBody] KeyRequest keyRequest)
        {
            var data = _smsService.AracMarka();
            return Json(data);
        }
        [HttpPost]
        public IActionResult AracTip([FromBody] KeyRequest keyRequest)
        {
            var data = _smsService.AracTipByMarkaKodu(keyRequest.MarkaKodu);
            return Json(data);
        }
        [HttpPost]
        public IActionResult TVMKullaniciByTvmKodu([FromBody] KeyRequest keyRequest)
        {
            var data = _smsService.TVMKullaniciByTvmKodu(keyRequest.TvmKodu);
            return Json(data);
        }
        [HttpPost]
        public IActionResult TVMDetaybyTvmKodu([FromBody] KeyRequest keyRequest)
        {
            var data = _smsService.TVMDetaybyTvmKodu(keyRequest.TvmKodu);
            return Json(data);
        }

    }
}
