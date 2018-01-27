using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ParentsRules.Models;

namespace ParentsRules.Services
{
    public class SMSSender : ISMSSender
    {
        private readonly ILogger _logger;
        public SMSServiceSettings _smsSettings { get; }
        public SMSSender(IOptions<SMSServiceSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }
        public async Task SendSMSMessage(string message, List<string> phonenumbers)
        {
            try
            {           
                ITwilioRestClient _client;
                _client = new TwilioRestClient(_smsSettings.AccountSID, _smsSettings.AuthenticationToken);
                foreach(string phonenumber in phonenumbers)
                {
                    await MessageResource.CreateAsync(new PhoneNumber(phonenumber), from: new PhoneNumber(_smsSettings.TwilioNumber), body: message, client: _client);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
            }
        }
    }
}
