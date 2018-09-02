using ImageGallery.WebApi.DataLayer.Configurations;
using ImageGallery.WebApi.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.WebApi.DataLayer.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<Image> Images { set; get; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // Custom application mappings
            builder.ApplyConfiguration(new ImageConfiguration());
        }
    }
}