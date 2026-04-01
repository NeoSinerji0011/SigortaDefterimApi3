using System.ComponentModel.DataAnnotations;

namespace API.Areas.MobilApi.Models.Database
{
    public class SirketSoruDegerleri
    {
        [Key]
        public int Id { get; set; }
        public int? SirketSorulariId { get; set; }
        public string Deger { get; set; }
    }
}
