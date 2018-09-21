using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ApiResourceClaimConfiguration : IEntityTypeConfiguration<ApiResourceClaim>
    {
        public void Configure(EntityTypeBuilder<ApiResourceClaim> apiClaim)
        {
            apiClaim.HasKey(x => x.Id);

            apiClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
        }
    }
}