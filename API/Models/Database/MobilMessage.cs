using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Database
{
    public class MobilMessage
    {
        [Key]
        public int Id { get; set; }
        public int OturumId { get; set; }
        public string Gonderici_Tip { get; set; }
        public string Mesaj { get; set; }
        public DateTime Tarih_Saat{ get; set; }
        public string Goruldumu { get; set; }
    }
}
