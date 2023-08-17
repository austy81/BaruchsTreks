using Microsoft.AspNetCore.Identity;

namespace BaruchsTreks.Data
{
    public class AppUser : IdentityUser
    {
        public bool IsContributor { get; set; }
    }
}
