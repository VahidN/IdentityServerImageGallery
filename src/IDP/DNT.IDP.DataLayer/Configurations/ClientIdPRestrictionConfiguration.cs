using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class ClientIdPRestrictionConfiguration : IEntityTypeConfiguration<ClientIdPRestriction>
    {
        public void Configure(EntityTypeBuilder<ClientIdPRestriction> idPRestriction)
        {
            idPRestriction.Property(x => x.Provider).HasMaxLength(200).IsRequired();
        }
    }
}