using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.ManageViewModels
{
    public class FriendsRequestViewModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateRequested { get; set; }
        public string StatusMessage { get; set; }
    }
}
