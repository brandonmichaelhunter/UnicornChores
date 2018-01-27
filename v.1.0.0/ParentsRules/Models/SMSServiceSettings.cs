using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models
{
    public class SMSServiceSettings
    {
        public string AccountSID { get; set; }
        public string AuthenticationToken { get; set; }
        public string TwilioNumber { get; set; }
    }
}
