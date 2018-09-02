using System;
using ImageGallery.WebApi.DomainClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageGallery.WebApi.DataLayer.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasData(
                new Image
                {
                    Id = new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                    Title = "An image by User 1",
                    FileName = "3fbe2aea-2257-44f2-b3b1-3d8bacade89c.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("1efe7a31-8dcc-4ff0-9b2d-5f148e2989cc"),
                    Title = "An image by User 1",
                    FileName = "43de8b65-8b19-4b87-ae3c-df97e18bd461.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("b24e3df5-0394-468d-9c1d-db1252fea920"),
                    Title = "An image by User 1",
                    FileName = "46194927-ccda-432f-bc95-4820318c08c7.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("9f35e705-637a-4bbe-8c35-402b2ecf7128"),
                    Title = "An image by User 1",
                    FileName = "4cdd494c-e6e1-4af1-9e54-24a8e80ea2b4.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("939df3fd-de57-4caf-96dc-c5e110322a96"),
                    Title = "An image by User 1",
                    FileName = "5c20ca95-bb00-4ef1-8b85-c4b11e66eb54.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("d70f656d-75a7-45fc-b385-e4daa834e6a8"),
                    Title = "An image by User 1",
                    FileName = "6b33c074-65cf-4f2b-913a-1b2d4deb7050.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("ce1d2b1c-7869-4df5-9a32-2cbaca8c3234"),
                    Title = "An image by User 1",
                    FileName = "7e80a4c8-0a8a-4593-a16f-2e257294a1f9.jpg",
                    OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"
                },
                new Image
                {
                    Id = new Guid("2645bd94-3624-43fc-b21f-1209d730fc71"),
                    Title = "An image by User 2",
                    FileName = "8d351bbb-f760-4b56-9d4e-e8d61619bf70.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                },
                new Image
                {
                    Id = new Guid("3f41dc87-e8de-42ee-ac8d-355e4d3e1a2d"),
                    Title = "An image by User 2",
                    FileName = "b2894002-0b7c-4f05-896a-856283012c87.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                },
                new Image
                {
                    Id = new Guid("d3118665-43e3-4905-8848-5e335a428dd5"),
                    Title = "An image by User 2",
                    FileName = "cc412f08-2a7b-4225-b659-07fdb168302d.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                },
                new Image
                {
                    Id = new Guid("136f358d-98fb-4961-855c-59d5a6d1fa19"),
                    Title = "An image by User 2",
                    FileName = "cd139111-c82e-4bc8-9f7d-43a1059bfe73.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                },
                new Image
                {
                    Id = new Guid("5e0e1379-3e8e-4f51-99f1-1fb9ec3a19b0"),
                    Title = "An image by User 2",
                    FileName = "dc3f39bf-d1da-465d-9ea4-935902c2e3d2.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                },
                new Image
                {
                    Id = new Guid("ab46efdb-0384-400c-89cb-95bba1c500e9"),
                    Title = "An image by User 2",
                    FileName = "e0e32179-109c-4a8a-bf91-3d65ff83ca29.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                },
                new Image
                {
                    Id = new Guid("ae03d971-40a6-4350-b8a9-7b12e1d93d71"),
                    Title = "An image by User 2",
                    FileName = "fdfe7329-e05c-41fb-a7c7-4f3226d28c49.jpg",
                    OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
                }
             );
        }
    }
}