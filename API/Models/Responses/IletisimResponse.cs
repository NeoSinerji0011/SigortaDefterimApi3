using API.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class IletisimResponse : MobilIletisim
    {
        public string? EkSureDisplay { get; set; }
        public string? PoliceNumarasi { get; set; }
        public int? BransKodu { get; set; }
        public int? TalepNo { get; set; }
        public string? AdSoyad { get; set; }
        public string? BrasAdi { get; set; }
       // public MobilMessage Message { get; set; }
        public List<MobilMessage> MessageList { get; set; }

        public bool isActive { get; set; }
        public bool isNewMssage { get; set; }
       
         
    }
    public class IletisimResponseMobil 
    { 
        public List<IletisimResponse> iletisimResponse { get; set; }
        public List<MobilMessage> MessageList { get; set; }
        public List<MobilMessageDosya> MessageDosyaList { get; set; }


    }
    public class CompanyMailData  
    {
        public string? FirmaAdi { get; set; }
        public string? Dosya { get; set; }
        public string? FirmaMail { get; set; }
        public string? Token { get; set; }

    }
}
