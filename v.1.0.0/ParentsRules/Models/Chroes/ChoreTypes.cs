using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.Chroes
{
    public class ChoreTypes
    {
        public int ID { get; set; }
        public string Chore { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string AuthorUserID { get; set; }
    }
}
