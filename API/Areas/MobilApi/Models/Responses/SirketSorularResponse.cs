
using API.Areas.MobilApi.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Responses
{
    public class SirketSorularResponse
    { 
        public List<SirketVerileriResponseItem> SirketVerileri { get; set; }
    }
    public class SirketVerileriResponseItem
    {
        public int Id { get; set; }
        public int? TumKodu { get; set; }
        public string TumAdi { get; set; }
        public Nullable<DateTime> KayitTarihi { get; set; }
        public string AktifPasif { get; set; }
        public int BransKodu { get; set; }
        public List<SirketSorularResponseItem> SirketSorulari { get; set; }


    }
    public class SirketSorularResponseItem
    {
        public string Soru { get; set; }
        public string InputTuru { get; set; }
        public string SoruKodu { get; set; }

        public List<SirketSoruDegerleri> SirketSoruDegerleri { get; set; }

    }
}
