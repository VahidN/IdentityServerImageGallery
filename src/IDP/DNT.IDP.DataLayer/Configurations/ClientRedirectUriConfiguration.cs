using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ClientRedirectUriConfiguration : IEntityTypeConfiguration<ClientRedirectUri>
    {
        public void Configure(EntityTypeBuilder<ClientRedirectUri> redirectUri)
        {
            redirectUri.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
        }
    }
}