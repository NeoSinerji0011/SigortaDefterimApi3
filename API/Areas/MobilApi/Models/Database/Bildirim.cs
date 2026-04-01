using SigortaDefterimV2API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{ 
    public class Bildirim
    {
        [Key]
        public int Id { get; set; }
        public string KimlikNo{ get; set; }
        public string PoliceNumarasi { get; set; }
        public DateTime BitisTarihi{ get; set; }
    }
}
