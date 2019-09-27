using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Identity.Configurations
{
    public class PasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var userName = await manager.GetUserNameAsync(user);

            if (userName == password)
                return IdentityResult.Failed(new IdentityError { Description = "A senha não pode ser igual ao Nome do Usuário" });

            if (InvalidPass().Contains(password))
                return IdentityResult.Failed(new IdentityError { Description = "Senha inválida" });

            return IdentityResult.Success;
        }

        private List<string> InvalidPass()
        {
            return new List<string>
            {
                "senha",
                "password"
            };
        }
    }
}
