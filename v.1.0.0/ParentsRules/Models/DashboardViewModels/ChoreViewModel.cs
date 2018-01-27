using ParentsRules.Models.Chroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.DashboardViewModels
{
    public class ChoreViewModel:UserChores
    {
        public int ID { get; set; }
        public int ChoreID { get; set; }
        public int RoomID { get; set; }
        public string UserID { get; set; }
        public float DollarAmount { get; set; }
        public string ParentID { get; set; }
        public DateTime DateDue { get; set; }
        public bool IsDaily { get; set; }
        public bool IsWeekly { get; set; }
        public bool IsMonthly { get; set; }
        public bool IsYearly { get; set; }
        public string Comments { get; set; }
    }
}
