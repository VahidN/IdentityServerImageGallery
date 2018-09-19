using DNT.IDP.DomainClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNT.IDP.DataLayer.Configurations
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.HasOne(userLogin => userLogin.User)
                .WithMany(user => user.UserLogins)
                .HasForeignKey(userLogin => userLogin.SubjectId);

            builder.HasIndex(userLogin => new {userLogin.SubjectId, userLogin.LoginProvider}).IsUnique();
        }
    }
}