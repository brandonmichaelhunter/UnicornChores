using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.Rooms
{
    public class RoomTypes
    {
        public int ID { get; set; }
        public string Room { get; set; }
        public bool IsActive { get; set; }
        public string UserID { get; set; }
    }
}
