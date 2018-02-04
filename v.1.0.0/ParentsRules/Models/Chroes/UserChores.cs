using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.Chroes
{
    public class UserChores
    {
        public int ID { get; set; }
        [Required(ErrorMessage ="A chore must be selected.")]
        public string Chore { get; set; }
        [Display(Name ="Description")]
        [DataType(DataType.MultilineText)]
        public string ChoreDescription { get; set; }
        public int RoomID { get; set; }
        [Required(ErrorMessage ="A child must be selected for the chore")]
        public string UserID { get; set; }
        [Display(Name ="Allowence")]
        [DataType(DataType.Currency)]
        public float DollarAmount { get; set; }
        public string ParentID { get; set; }
        public DateTime? DateDue { get; set; }
        public bool IsDaily { get; set; }
        public bool IsWeekly { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool ChoreCompleted { get; set; }
        public DateTime? DateChoreCompleted { get; set; }
        public bool ParentVerified { get; set; }
        public DateTime? ParentVerifiedDate { get; set; }
        public string PublishStatus { get; set; }
        public DateTime LastPublishDate { get; set; }

    }
}
