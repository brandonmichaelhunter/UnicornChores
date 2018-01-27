using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Services
{
    public interface ISMSSender
    {
        Task SendSMSMessage(string message, List<string> phonenumbers);
    }
}
