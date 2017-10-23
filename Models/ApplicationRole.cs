using System;
using Microsoft.AspNetCore.Identity;

namespace SimpleAPI.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}