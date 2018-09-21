using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> client)
        {
            client.HasKey(x => x.Id);

            client.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
            client.Property(x => x.ProtocolType).HasMaxLength(200).IsRequired();
            client.Property(x => x.ClientName).HasMaxLength(200);
            client.Property(x => x.ClientUri).HasMaxLength(2000);
            client.Property(x => x.LogoUri).HasMaxLength(2000);
            client.Property(x => x.Description).HasMaxLength(1000);
            client.Property(x => x.FrontChannelLogoutUri).HasMaxLength(2000);
            client.Property(x => x.BackChannelLogoutUri).HasMaxLength(2000);
            client.Property(x => x.ClientClaimsPrefix).HasMaxLength(200);
            client.Property(x => x.PairWiseSubjectSalt).HasMaxLength(200);

            client.HasIndex(x => x.ClientId).IsUnique();

            client.HasMany(x => x.AllowedGrantTypes).WithOne(x => x.Client).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.RedirectUris).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.AllowedScopes).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.ClientSecrets).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.Claims).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.IdentityProviderRestrictions).WithOne(x => x.Client).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.AllowedCorsOrigins).WithOne(x => x.Client).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            client.HasMany(x => x.Properties).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}