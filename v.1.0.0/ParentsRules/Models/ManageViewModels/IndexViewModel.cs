﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParentsRules.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string AssociatedUserEmail { get; set; }
        [Display(Name = "PFirst Name")]
        public string AssociatedUserFirstName { get; set; }
        [Display(Name = "Last Name")]
        public String AssociatedUserLastName { get; set; }
        public string BGPageImage { get; set; }
        /* Stores a list users linked to current user. */
        public List<ApplicationUser> AssociatedUsers { get; set; }
    }
}
