using Microsoft.AspNetCore.Identity;

namespace WebApp.Identity.Entities
{
    public class User : IdentityUser
    {
        public string Member { get; set; } = "Member";
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
