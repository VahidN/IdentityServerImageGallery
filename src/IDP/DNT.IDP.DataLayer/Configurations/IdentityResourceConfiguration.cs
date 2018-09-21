using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class IdentityResourceConfiguration : IEntityTypeConfiguration<IdentityResource>
    {
        public void Configure(EntityTypeBuilder<IdentityResource> identityResource)
        {
            identityResource.HasKey(x => x.Id);

            identityResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
            identityResource.Property(x => x.DisplayName).HasMaxLength(200);
            identityResource.Property(x => x.Description).HasMaxLength(1000);

            identityResource.HasIndex(x => x.Name).IsUnique();

            identityResource.HasMany(x => x.UserClaims).WithOne(x => x.IdentityResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}