using Microsoft.AspNetCore.Identity;
using WebApp.Identity.Enums;

namespace WebApp.Identity.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string NormalizedName { get { return Name.ToUpper(); } private set { Name.ToUpper(); } }
        public EMember Member { get; set; } = EMember.Google;
    }
}
