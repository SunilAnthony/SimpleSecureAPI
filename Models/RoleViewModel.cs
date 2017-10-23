using System.ComponentModel.DataAnnotations;

namespace SimpleAPI.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

        
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        
    }
}