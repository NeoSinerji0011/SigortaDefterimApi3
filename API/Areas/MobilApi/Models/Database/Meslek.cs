using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class Meslek
    {
        [Key]
        public int MeslekKodu { get; set; }
        public string MeslekAdi { get; set; } 
    }
}
