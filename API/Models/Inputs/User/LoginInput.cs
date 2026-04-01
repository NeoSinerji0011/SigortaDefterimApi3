using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models
{
    public class LoginInput
    {
        [DefaultValue("mehmetust@gmail.com")]
        public string Email { get; set; }
        [DefaultValue("222222")]
        public string Password { get; set; }
    }
}
