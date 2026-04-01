using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Models.Responses
{

    public enum MessageStatus
    {
        Success = 200, 
        Error = 400,
        Failed = 500
    }
    public class Message
    {
        public MessageStatus statusCode { get; set; }

        public string message { get; set; }
    }
}
