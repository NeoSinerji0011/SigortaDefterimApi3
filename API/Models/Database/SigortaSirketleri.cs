using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class SigortaSirketleri
    {
        [Key]
        public string SirketKodu { get; set; }
        public string SirketAdi { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public string SirketLogo { get; set; }
        public byte? UygulamaKodu { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; } 
    }
}
