using API.Areas.MobilApi.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Models.Responses
{
    public class LoginResponse
    {
        public Kullanici kullanici { get; set; }
        public string Token { get; set; }
    }
}
