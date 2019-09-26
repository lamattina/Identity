﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Identity.Entities;
using WebApp.Identity.Models;

namespace WebApp.Identity.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IToastNotification _toastNotification;
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

        public LoginController(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IToastNotification toastNotification,
                               IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Authentication()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authentication(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email) || string.IsNullOrWhiteSpace(model?.Password))
            {
                _toastNotification.AddAlertToastMessage("Para autenticar, informe o seu e-mail e a sua senha");
                return View();
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user == null)
                {
                    ModelState.AddModelError("", "Usuário não encontrado");
                    return View();
                }else
                {
                    if(await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                        //return RedirectToAction("About");

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

            //if (user != null && !await _userManager.IsLockedOutAsync(user))
            //{
            //    if (await _userManager.CheckPasswordAsync(user, model.Password))
            //    {
            //        if (!await _userManager.IsEmailConfirmedAsync(user))
            //        {
            //            ModelState.AddModelError("", "Email não confirmado. Por favor, faça a confirmação antes de continuar");
            //            return View();
            //        }

            //        await _userManager.ResetAccessFailedCountAsync(user);

            //        if (await _userManager.GetTwoFactorEnabledAsync(user))
            //        {
            //            var validator = await _userManager.GetValidTwoFactorProvidersAsync(user);

            //            if (validator.Contains("Email"))
            //            {
            //                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            //                System.IO.File.WriteAllText("email2cv.txt", token);

            //                await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, Store2FA(user.Id, "Email"));

            //                return RedirectToAction("TwoFactor");
            //            }
            //        }

            //        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            //        await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, principal);
            //        //var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            //        //if (signInResult.Succeeded)
            //        //{
            //        return RedirectToAction("Index", "Home");
            //        //}
            //    }

            //    await _userManager.AccessFailedAsync(user);

            //    if (await _userManager.IsLockedOutAsync(user))
            //    {
            //        // Email deve ser enviado sugerindo a troca de senha
            //    }
            //}

            //ModelState.AddModelError("", "Usuário ou Senha Inválida");
            ////}

            //return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
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