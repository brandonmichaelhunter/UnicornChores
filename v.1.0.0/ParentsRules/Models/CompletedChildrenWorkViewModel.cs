using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models
{
    public class CompletedChildrenWorkViewModel
    {
        public int ID { get; set; }
        
        public float TotalPayout { get; set; }
        public string StartOfWeekDateDisplay { get; set; }
        public List<CompletedChildWorkEarningsViewModel> WeekWorkHistory { get; set; }
    }
}
