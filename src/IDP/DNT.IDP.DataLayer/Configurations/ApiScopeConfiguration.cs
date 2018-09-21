using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ApiScopeConfiguration : IEntityTypeConfiguration<ApiScope>
    {
        public void Configure(EntityTypeBuilder<ApiScope> apiScope)
        {
            apiScope.HasKey(x => x.Id);

            apiScope.Property(x => x.Name).HasMaxLength(200).IsRequired();
            apiScope.Property(x => x.DisplayName).HasMaxLength(200);
            apiScope.Property(x => x.Description).HasMaxLength(1000);

            apiScope.HasIndex(x => x.Name).IsUnique();

            apiScope.HasMany(x => x.UserClaims).WithOne(x => x.ApiScope).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}