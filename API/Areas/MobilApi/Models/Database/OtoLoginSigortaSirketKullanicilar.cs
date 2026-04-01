using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class OtoLoginSigortaSirketKullanicilar
    {

        [Key]
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public int TUMKodu { get; set; }
        public string? SmsKodTelNo { get; set; }
        public string ProxyIpPort { get; set; }
        public string SigortaSirketAdi { get; set; } 
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
        public string? SmsKodSecretKey1 { get; set; }
        public string? SmsKodSecretKey2 { get; set; }

    }
}
