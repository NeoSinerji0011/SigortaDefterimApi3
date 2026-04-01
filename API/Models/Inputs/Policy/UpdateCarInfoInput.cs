using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Inputs
{
    public class UpdateCarInfoInput
    {
        [DefaultValue(13)]
        public int PolicyId { get; set; }
        [DefaultValue(2010)]
        public int ModelYili { get; set; }
        [DefaultValue("061")]
        public string MarkaKodu { get; set; }
        [DefaultValue("096")]
        public string TipKodu { get; set; }
        [DefaultValue("111+10")]
        public string AracKullanimTarzi { get; set; }
    }
}
