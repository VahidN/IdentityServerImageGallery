using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ApiSecretConfiguration : IEntityTypeConfiguration<ApiSecret>
    {
        public void Configure(EntityTypeBuilder<ApiSecret> apiSecret)
        {
            apiSecret.HasKey(x => x.Id);

            apiSecret.Property(x => x.Description).HasMaxLength(1000);
            apiSecret.Property(x => x.Value).HasMaxLength(2000);
            apiSecret.Property(x => x.Type).HasMaxLength(250);
        }
    }
}