using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ClientSecretConfiguration : IEntityTypeConfiguration<ClientSecret>
    {
        public void Configure(EntityTypeBuilder<ClientSecret> secret)
        {
            secret.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            secret.Property(x => x.Type).HasMaxLength(250);
            secret.Property(x => x.Description).HasMaxLength(2000);
        }
    }
}