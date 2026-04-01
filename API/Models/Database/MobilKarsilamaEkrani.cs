using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Database
{
    public class MobilKarsilamaEkrani
    {
        public int Id { get; set; }
        public string ResimUrl { get; set; }
        public int Indis { get; set; }
        public bool Durum { get; set; }
    }
}
