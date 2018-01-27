using ParentsRules.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Extensions
{
    public static class UserProfileExtensions
    {
        public static string GetDisplayName(ApplicationDbContext context, string UserID)
        {
            string displayName = string.Empty;
            var User = context.AccountUsers.Where(a => a.Id == UserID).FirstOrDefault();
            if(User != null)
            {
                displayName = string.Format("{0} {1} {2}", User.FirstName, User.MiddleName, User.LastName);
            }
            else
            {
                displayName = "Ghost User";
            }
            return displayName;
        }
    }
}
