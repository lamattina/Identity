using Microsoft.AspNetCore.Identity;
using WebApp.Identity.Enums;

namespace WebApp.Identity.Entities
{
    public class User : IdentityUser
    {
        public EMember Member { get; set; } = EMember.Microsoft;
    }
}
