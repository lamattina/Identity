using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Identity.Entities;
using WebApp.Identity.Persistences.Mappings;

namespace WebApp.Identity.Persistences.Contexts
{
    public class MyDbContext : IdentityDbContext<User>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new OrganizationMap());

            //base.OnModelCreating(builder);

            //builder.Entity<Organization>(org =>
            //{
            //    org.ToTable("Organizations");
            //    org.HasKey(x => x.Id);

            //    org.HasMany<User>()
            //        .WithOne()
            //        .HasForeignKey(x => x.OrganizationId)
            //        .IsRequired(false);
            //}
            //);
        }
    }
}
