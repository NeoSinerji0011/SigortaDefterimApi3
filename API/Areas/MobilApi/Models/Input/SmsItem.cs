using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Input
{
    public class SmsItem : SmsRequest
    {
        public string fromPhone { get; set; }
        public string fromPhone2 { get; set; }
        public string toPhone { get; set; }
        public string body { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }
    public class SmsRequest
    {
        public string SirketAdi { get; set; }
        public decimal currentTimeData { get; set; }
        public decimal currentOldTimeData { get; set; }

    }
    public class TvmRequest: KeyRequest
    {
      
    }
    public class KeyRequest
    {
        public int TvmKodu { get; set; }
        public int TumKodu { get; set; }
        public string MarkaKodu { get; set; }

    }
    /// <summary>
    /// qrcode,text,secretqrcode(outpath a gidecek olan tür)
    /// </summary>
    public class AuthenticatorRequest
    {
        public int TvmKodu { get; set; }  
        public IFormFile FormFile { get; set; }  

    }
}
