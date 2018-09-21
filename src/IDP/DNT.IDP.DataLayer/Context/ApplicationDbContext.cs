using DNT.IDP.DataLayer.Configurations;
using DNT.IDP.DomainClasses;
using DNT.IDP.DomainClasses.IdentityServer4Entities;
using Microsoft.EntityFrameworkCore;
using UserClaim = DNT.IDP.DomainClasses.UserClaim;

namespace DNT.IDP.DataLayer.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<User> Users { set; get; }
        public virtual DbSet<UserClaim> UserClaims { set; get; }
        public virtual DbSet<UserLogin> UserLogins { set; get; }
        
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<IdentityResource> IdentityResources { get; set; }
        public virtual DbSet<ApiResource> ApiResources { get; set; }
        public virtual DbSet<PersistedGrant> PersistedGrants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // Custom application mappings
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
            
            builder.ApplyConfiguration(new PersistedGrantConfiguration());
            builder.ApplyConfiguration(new IdentityResourceConfiguration());
            builder.ApplyConfiguration(new IdentityClaimConfiguration());
            builder.ApplyConfiguration(new ClientSecretConfiguration());
            builder.ApplyConfiguration(new ClientScopeConfiguration());
            builder.ApplyConfiguration(new ClientRedirectUriConfiguration());
            builder.ApplyConfiguration(new ClientPropertyConfiguration());
            builder.ApplyConfiguration(new ClientPostLogoutRedirectUriConfiguration());
            builder.ApplyConfiguration(new ClientIdPRestrictionConfiguration());
            builder.ApplyConfiguration(new ClientGrantTypeConfiguration());
            builder.ApplyConfiguration(new ClientCorsOriginConfiguration());
            builder.ApplyConfiguration(new ClientConfiguration());
            builder.ApplyConfiguration(new ClientClaimConfiguration());
            builder.ApplyConfiguration(new ApiSecretConfiguration());
            builder.ApplyConfiguration(new ApiScopeConfiguration());
            builder.ApplyConfiguration(new ApiScopeClaimConfiguration());
            builder.ApplyConfiguration(new ApiResourceConfiguration());
            builder.ApplyConfiguration(new ApiResourceClaimConfiguration());
        }
    }
}