using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Database
{
    public class MobilKaydirmaEkrani
    {
        public int Id { get; set; }
        public string ResimUrl { get; set; }
        public bool Durum { get; set; }
        public string Aciklama { get; set; }
    }
}
