using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.Chroes
{
    public class ChoresViewModel
    {
        public int ID { get; set; }
        public string Chore { get; set; }
        public int ChoreID { get; set; }
        public bool Monday { get; set; }
        public int MondayCompletedStatus { get; set; }
        public bool Tuesday { get; set; }
        public int TuesdayCompletedStatus { get; set; }
        public bool Wednesday { get; set; }
        public int WednesdayCompletedStatus { get; set; }
        public bool Thursday { get; set; }
        public int ThursdayCompletedStatus { get; set; }
        public bool Friday { get; set; }
        public int FridayCompletedStatus { get; set; }
        public bool Saturday { get; set; }
        public int SaturdayCompletedStatus { get; set; }
        public bool Sunday { get; set; }
        public int SundayCompletedStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string MondayAllowence { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string TuesdayAllowence { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string WednesdayAllowence { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string ThursdayAllowence { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string FridayAllowence { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string SaturdayAllowence { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public string SundayAllowence { get; set; }

    }
}
