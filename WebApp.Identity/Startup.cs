using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NToastNotify;
using System;
using System.Reflection;
using WebApp.Identity.Configurations;
using WebApp.Identity.Entities;
using WebApp.Identity.Helpers;
using WebApp.Identity.Persistences.Contexts;

namespace WebApp.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc
            (
                config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }

            )
            .AddNToastNotifyToastr(new ToastrOptions()
            {
                ProgressBar = false,
                TimeOut = 5000,
                PositionClass = ToastPositions.TopRight
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services
            //    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
                //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                //    options =>
                //    {
                //        options.LoginPath = new PathString("/Login/Authentication/");
                //        options.LogoutPath = new PathString();
                //        options.AccessDeniedPath = new PathString("/Account/Forbidden/");
                //    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MemberOnly", policy => policy.RequireClaim("Member"));
                options.AddPolicy("EmployeeId", policy => policy.RequireClaim("EmployeeId", "123", "456"));
            });


            MigrationConfiguration(services);

            PasswordConfiguration(services);

            TokenConfiguration(services);

            services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
         .AddCookie(options =>
         {
             options.LoginPath = "/Account/LogIn";
             options.LogoutPath = "/Account/LogOff";
         });

            //services.ConfigureApplicationCookie
            //(
            //    opt =>
            //    {
            //        opt.LoginPath = "/Login/Authentication";
            //        opt.ReturnUrlParameter = "/";
            //    }
            //);

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseNToastNotify();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void MigrationConfiguration(IServiceCollection services)
        {
            var connectionString = ConfigurationExtensions.GetConnectionString(this.Configuration, "DefaultConnection");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<MyDbContext>(
                opt =>
                    opt.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly))
            );
        }

        private void PasswordConfiguration(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(
                opt =>
                {
                    opt.SignIn.RequireConfirmedEmail = false;

                    opt.Password.RequireDigit = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequiredLength = 4;

                    opt.Lockout.AllowedForNewUsers = false;
                    opt.Lockout.MaxFailedAccessAttempts = 3;
                }
            )
            .AddEntityFrameworkStores<MyDbContext>()
            .AddDefaultTokenProviders()
            .AddPasswordValidator<Configurations.PasswordValidator<User>>();
        }

        private void TokenConfiguration(IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>
            (
                opt =>
                    opt.TokenLifespan = TimeSpan.FromMinutes(15)
            );
        }
    }
}
