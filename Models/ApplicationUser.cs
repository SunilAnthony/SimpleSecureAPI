using System;
using Microsoft.AspNetCore.Identity;

namespace SimpleAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime JoinDate { get; set; }   
        public string JobTitle { get; set; } 

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        
    }
}