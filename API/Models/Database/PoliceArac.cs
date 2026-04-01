using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class PoliceArac
    {
        [Key]
        public int PoliceId { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public string Marka { get; set; }
        public string MarkaAciklama { get; set; }
        public string AracinTipiKodu { get; set; }
        public string AracinTipiAciklama { get; set; }
        public string AracinTipiKodu2 { get; set; }
        public string AracinTipiAciklama2 { get; set; }
        public Nullable<int> Model { get; set; }
        public string Cinsi { get; set; }
        public string TescilSeriKod { get; set; }
        public string TescilSeriNo { get; set; }
        public string AsbisNo { get; set; }
        public string KullanimSekli { get; set; }
        public string KullanimTarzi { get; set; }

    }

}
