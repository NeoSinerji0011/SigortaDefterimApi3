using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class AracKullanimSekli
    {
        [Key]
        public Int16 KullanimSekliKodu { get; set; }
        public string KullanimSekli { get; set; }
    }
}
