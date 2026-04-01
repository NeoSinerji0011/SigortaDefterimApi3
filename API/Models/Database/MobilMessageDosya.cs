using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilMessageDosya
    {
        [Key]
        public int Id { get; set; }
        public int MobilMessageId { get; set; }
        public string DosyaUrl { get; set; }
        public string DosyaTip { get; set; }
    }
}
