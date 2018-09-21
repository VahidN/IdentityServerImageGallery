using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class IdentityClaimConfiguration : IEntityTypeConfiguration<IdentityClaim>
    {
        public void Configure(EntityTypeBuilder<IdentityClaim> claim)
        {
            claim.HasKey(x => x.Id);
            claim.Property(x => x.Type).HasMaxLength(200).IsRequired();
        }
    }
}