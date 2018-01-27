using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ParentsRules.Models.Rooms
{
    public class UserRooms
    {
        public int ID { get; set; }
        public string Room { get; set; }
        public string UserID { get; set; }
        
        public DateTime? DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}
