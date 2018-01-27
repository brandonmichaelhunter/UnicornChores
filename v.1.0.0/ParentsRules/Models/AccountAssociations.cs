using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace ParentsRules.Models
{
    public class AccountAssociations
    {
        public int ID { get; set; }
        public string PrimaryUserID { get; set; }
        public string AssociatedUserID { get; set; }
        public bool IsChild { get; set; }
    }
}
