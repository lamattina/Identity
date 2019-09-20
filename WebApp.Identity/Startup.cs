using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                //opt => opt.Filters.Add(new AuthorizeFilter())
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            MigrationConfiguration(services);

            PasswordConfiguration(services);

            TokenConfiguration(services);

            services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();

            services.ConfigureApplicationCookie
            (
                opt => 
                {
                    opt.LoginPath = "/Login/Authentication/";
                }
            );

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
                    opt.SignIn.RequireConfirmedEmail = true;

                    opt.Password.RequireDigit = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequiredLength = 4;

                    opt.Lockout.AllowedForNewUsers = true;
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
                    opt.TokenLifespan = TimeSpan.FromHours(3)
            );
        }
    }
}
