using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Identity.Entities;

namespace WebApp.Identity.Persistences.Mappings
{
    public class OrganizationMap : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("AspNetOrganizations", schema:"ctm");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(50).IsRequired();

            builder.HasMany<User>()
                .WithOne()
                .HasForeignKey(x => x.OrganizationId)
                .IsRequired(false);
        }
    }
}
