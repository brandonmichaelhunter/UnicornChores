using ParentsRules.Models.Chroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.DashboardViewModels
{
    public class DashboardViewModel
    {
        public int ID { get; set; }
        public string AssignedChildID { get; set; }
        public string AssignedChildName { get; set; }
        public List<UserChores> Chores { get; set; }
    }
}
