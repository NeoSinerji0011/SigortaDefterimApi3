using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class Il
    {
        [Key]
        public string IlKodu { get; set; }
        public string UlkeKodu { get; set; }
        public string IlAdi { get; set; }
    }
}
