using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ClientClaimConfiguration : IEntityTypeConfiguration<ClientClaim>
    {
        public void Configure(EntityTypeBuilder<ClientClaim> claim)
        {
            claim.Property(x => x.Type).HasMaxLength(250).IsRequired();
            claim.Property(x => x.Value).HasMaxLength(250).IsRequired();
        }
    }
}