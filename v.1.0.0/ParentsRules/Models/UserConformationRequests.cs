using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models
{
    public class UserConformationRequests
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IsConfirmed { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateConfirmed { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string RequestedUserID { get; set; }
        public string RegistrationCode { get; set; }
    }
}
