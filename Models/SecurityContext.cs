using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SimpleAPI.Models
{
    public class SecurityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public SecurityContext(DbContextOptions<SecurityContext> options) : base(options){}
        
    }
}