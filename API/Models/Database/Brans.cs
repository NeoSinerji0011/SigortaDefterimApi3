using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class Brans
    {
        [Key]
        public int BransKodu { get; set; }
        public string BransAdi { get; set; }
        public byte Durum { get; set; }
    }
}
