using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models
{
    public class CompletedChildWorkEarningsViewModel
    {
        public int ID { get; set; }
        public string ChildID { get; set; }
        public float TotalWeekEarnings { get; set; }
        public string ChildName { get; set; }
        public List<CompletedChildrenWork> CompletedChores { get; set; }
    }
}
