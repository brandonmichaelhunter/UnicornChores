using ParentsRules.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Extensions
{
    public class ChoresExtensions
    {
        public static string GetChoreName(ApplicationDbContext context, string ChoreID)
        {
            var chore = context.ChoreTypes.Where(a => a.ID == Convert.ToInt16(ChoreID)).FirstOrDefault();
            return (chore != null) ? chore.Chore : "N/A";
        }
        public static string GetChoreDescription(ApplicationDbContext context, string ChoreID)
        {
            var chore = context.ChoreTypes.Where(a => a.ID == Convert.ToInt16(ChoreID)).FirstOrDefault();
            return (chore != null) ? chore.Description : "N/A";
        }
        public static string GetRoomName(ApplicationDbContext context, int RoomID)
        {
            var chore = context.UserRooms.Where(a => a.ID == RoomID).FirstOrDefault();
            return (chore != null) ? chore.Room : "N/A";
        }
        public static string GetDisplayName(ApplicationDbContext context, string UserID)
        {
            string displayName = string.Empty;
            var User = context.AccountUsers.Where(a => a.Id == UserID).FirstOrDefault();
            if (User != null)
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
