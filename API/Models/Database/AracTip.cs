using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Database
{
    public class AracTip
    {
        [Key]

        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public string TipAdi { get; set; }
        public string KullanimSekli1 { get; set; }
        public string KullanimSekli2 { get; set; }
        public string KullanimSekli3 { get; set; }
        public string KullanimSekli4 { get; set; }
        public Int16? KisiSayisi { get; set; }
    }
}
