using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class Ilce
    {
        [Key]
        public int? IlceKodu { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; } 
        public string IlceAdi { get; set; }
    }
}
