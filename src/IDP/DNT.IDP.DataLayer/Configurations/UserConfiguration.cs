using DNT.IDP.Common;
using DNT.IDP.DomainClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(
                new User
                {
                    IsActive = true,
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "User 1",
                    Password = "password".GetSha256Hash()
                },
                new User
                {
                    IsActive = true,
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "User 2",
                    Password = "password".GetSha256Hash()
                }
            );
        }
    }
}