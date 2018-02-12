using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ParentsRules.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ParentUserID { get; set; }
        public string AssociatedUserID { get; set; }
        public string IsChild { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public string BGPageImage { get; set; }
        public bool IsAdmin { get; set; }
    }
}
