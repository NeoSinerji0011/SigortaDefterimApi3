using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilUlke
    {
        [Key]
        public int Id { get; set; }
        public int UlkeTipiKodu { get; set; }
        public string UlkeKodu { get; set; }
        public string UlkeAdi { get; set; }
    }
}
