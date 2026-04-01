using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class AracKullanimTarzi
    {
        [Key] 
        public string KullanimTarziKodu { get; set; }
         
        public string Kod2 { get; set; }
        public Int16 KullanimSekliKodu { get; set; }
        public string KullanimTarzi { get; set; }
        public byte Durum { get; set; }
    }
}
