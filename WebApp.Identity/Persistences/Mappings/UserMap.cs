using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Identity.Entities;

namespace WebApp.Identity.Persistences.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Member);
            builder.Property(x => x.Name).HasMaxLength(100);
            builder.Property(x => x.NormalizedName).HasMaxLength(100);
        }
    }
}
