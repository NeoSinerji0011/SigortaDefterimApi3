using SigortaDefterimV2API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class UserResponse
    {
         
        public string adsoyad { get; set; }
        public string email { get; set; }
        public string message { get; set; }
        public string subject{ get; set; }
    }
}
