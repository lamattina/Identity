using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Identity.Entities;

namespace WebApp.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IToastNotification _toastNotification;
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

        public AccountController(ILogger<LogoutModel> logger,
                                 UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IToastNotification toastNotification,
                                 IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            _toastNotification.AddSuccessToastMessage("Até breve!");

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(WebApp.Identity.Models.LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email) || string.IsNullOrWhiteSpace(model?.Password))
            {
                _toastNotification.AddAlertToastMessage("Para autenticar, informe o seu e-mail e a sua senha");
                return View();
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Usuário não encontrado");
                    return View();
                }
                else
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                        _toastNotification.AddSuccessToastMessage($"Seja bem-vindo, {user.UserName}");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _toastNotification.AddAlertToastMessage("Usuário ou senha inválido");
                    }
                }
            }

            return View();
        }
        
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(WebApp.Identity.Models.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _toastNotification.AddSuccessToastMessage("Usuário cadastrado com sucesso");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var erro in result.Errors)
                    {
                        _toastNotification.AddErrorToastMessage($"{erro.Code}: {erro.Description}");
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return View("Success");
                }
            }

            return View("Error");
        }

        private ClaimsPrincipal Store2FA(string userId, string provider)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("sub", userId),
                new Claim("amr",provider)
            }, IdentityConstants.TwoFactorUserIdScheme);

            return new ClaimsPrincipal(identity);
        }

        private void GenerateEmailConfirmation()
        {
            //if (result.Succeeded)
            //{
            //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //    var confirmationEmail = Url.Action("ConfirmEmailAddress", "Login",
            //        new { token = token, email = user.Email }, Request.Scheme);

            //    System.IO.File.WriteAllText("confirmationEmail.txt", confirmationEmail);
            //}
            //else
            //{
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError("", error.Description);
            //    }

            //    return View();
            //}
        }
    }
}