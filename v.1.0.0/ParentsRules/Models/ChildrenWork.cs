using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models
{
    public class ChildrenWork
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "A chore must be selected.")]
        public string Chore { get; set; }

        public int RoomID { get; set; }
        [Required(ErrorMessage = "A child must be selected for the chore")]
        public string UserID { get; set; }
        [Display(Name = "Allowence")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public float DollarAmount { get; set; }
        public string ParentID { get; set; }
        public DateTime? DateDue { get; set; }
        public bool Monday { get; set; }
        public bool MondayCompleted { get; set; }
        public bool Tuesday { get; set; }
        public bool TuesdayCompleted { get; set; }
        public bool Wednesday { get; set; }
        public bool WednesdayCompleted { get; set; }
        public bool Thursday { get; set; }
        public bool ThursdayCompleted { get; set; }
        public bool Friday { get; set; }
        public bool FridayCompleted { get; set; }
        public bool Saturday { get; set; }
        public bool SaturdayCompleted { get; set; }
        public bool Sunday { get; set; }
        public bool SundayCompleted { get; set; }
        public bool ChildCompleted { get; set; }
        public bool ChoreCompleted { get; set; }
        public DateTime? DateChoreCompleted { get; set; }
        public bool ParentVerified { get; set; }
        public DateTime? ParentVerifiedDate { get; set; }
        public int ChoreID { get; set; }
        public float TotalEarned { get; set; }
        public DateTime StartOfWeekDate { get; set; }
        public string StartOfWeekDateDisplay { get; set; }
    }
}



