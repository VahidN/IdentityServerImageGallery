using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ApiResourceConfiguration : IEntityTypeConfiguration<ApiResource>
    {
        public void Configure(EntityTypeBuilder<ApiResource> apiResource)
        {
            apiResource.HasKey(x => x.Id);

            apiResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
            apiResource.Property(x => x.DisplayName).HasMaxLength(200);
            apiResource.Property(x => x.Description).HasMaxLength(1000);

            apiResource.HasIndex(x => x.Name).IsUnique();

            apiResource.HasMany(x => x.Secrets).WithOne(x => x.ApiResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
            apiResource.HasMany(x => x.Scopes).WithOne(x => x.ApiResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
            apiResource.HasMany(x => x.UserClaims).WithOne(x => x.ApiResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}