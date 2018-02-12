using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ParentsRules.Models
{
    public class PageManagerViewModel
    {
        public int ID { get; set; }
        [Required]
        public string BackgroundImageName { get; set; }
        public string BackgroundImageFileName { get; set; }
        public string BackgroundImageFilePath { get; set; }
       
        public Microsoft.AspNetCore.Http.IFormFile BackgroundImage { get; set; }
    }
}
