using DNT.IDP.DataLayer.Configurations;
using DNT.IDP.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace DNT.IDP.DataLayer.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<User> Users { set; get; }
        public virtual DbSet<UserClaim> UserClaims { set; get; }
        public virtual DbSet<UserLogin> UserLogins { set; get; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // Custom application mappings
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
        }
    }
}