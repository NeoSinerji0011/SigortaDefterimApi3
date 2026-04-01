using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Responses
{

    public enum RegisterStatus
    {
        Success = 200,
        EmailExists = 400,
        TcExists = 401,
        Failed = 500
    }

    public class RegisterResponse
    {
        public RegisterStatus statusCode { get; set; }
        public string message { get; set; }
        public Kullanici kullanici { get; set; }
    }
}
