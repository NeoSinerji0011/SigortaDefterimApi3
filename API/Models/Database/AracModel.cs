using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Database
{
    public class AracModel
    {
        [Key]

        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public int Marka { get; set; }
        public decimal Fiyat { get; set; }
    }
}
