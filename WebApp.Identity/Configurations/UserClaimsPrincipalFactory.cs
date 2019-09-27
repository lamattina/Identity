using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Identity.Entities;

namespace WebApp.Identity.Configurations
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        public UserClaimsPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected async override Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("EmployeeId", "123", ClaimValueTypes.String));
            identity.AddClaim(new Claim("Administrator", "Administrador"));
            identity.AddClaim(new Claim("Member", user.Member.ToString(), ClaimValueTypes.String));

            return identity;
        }
    }
}
