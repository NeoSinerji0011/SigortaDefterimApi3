using API.Models.Database;
using SigortaDefterimV2API.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class GlobalReponse
    {

    }
    public class MobilGlobalDataList
    {
        public List<MobilUlke> mobilUlkeList { get; set; }//+
        public List<MobilUlkeTipi> mobilUlkeTipiList{ get; set; }//+
        public List<SigortaSirketleri> sigortaSirketleriList{ get; set; }
        public List<Il> ilList{ get; set; }//+
        public List<Ilce> ilceList{ get; set; }//+
        public List<AracMarka> aracMarkaList{ get; set; }//+
        public List<AracTip> aracTipList{ get; set; } 
        public List<AracKullanimTarzi> aracKullanimTarziList{ get; set; }//+
        public List<Brans> bransList{ get; set; }//+
        public List<Meslek> meslekList{ get; set; } //+
        public List<TVMDetay> acenteList{ get; set; } //

    }
}
