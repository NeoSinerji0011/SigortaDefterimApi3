using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilUlkeTipi
    {
        [Key]
        public int UlkeTipiKodu { get; set; }
        public string UlkeTipiAdi { get; set; } 
    }
}
