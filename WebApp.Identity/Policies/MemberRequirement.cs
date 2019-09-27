using Microsoft.AspNetCore.Authorization;

namespace WebApp.Identity.Policies
{
    public class MemberRequirement : IAuthorizationRequirement
    {
        public string Member { get; set; }

        public MemberRequirement(string member)
        {
            Member = member;
        }
    }
}
