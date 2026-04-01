using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilPoliceDosya
    {
        [Key]
        public int? Id { get; set; } 
        public int? MobilHasarId { get; set; }
        public int? MobilTeklifId { get; set; }
        public string DosyaUrl { get; set; }
        public string DosyaTipi { get; set; }
        public DateTime Tarih { get; set; }
    }
}
