using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Areas.MobilApi.Models.Database
{
    [Table("MobileSms")]
    public class MobileSms
    {
        [Key]
        public int Id { get; set; }

        public string FromPhone { get; set; }
        public string FromPhone2 { get; set; }
        public string ToPhone { get; set; }
        public string Body { get; set; }
        public DateTime SmsTarihi { get; set; }

        public string SirketAdi { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal CurrentTimeData { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal CurrentOldTimeData { get; set; }
    }
}
