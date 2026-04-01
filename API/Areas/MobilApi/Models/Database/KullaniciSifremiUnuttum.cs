using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.MobilApi.Models.Database
{
    public class KullaniciSifremiUnuttum
    {
        [Key]
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? ResetDate { get; set; }
        public string YeniSifre { get; set; }
        public byte Status { get; set; }


    }
}
