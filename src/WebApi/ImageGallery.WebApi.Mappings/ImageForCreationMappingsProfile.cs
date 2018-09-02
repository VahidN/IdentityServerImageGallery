using AutoMapper;
using ImageGallery.WebApi.DomainClasses;
using ImageGallery.WebApi.Models;

namespace ImageGallery.WebApi.Mappings
{
    public class ImageForCreationMappingsProfile : Profile
    {
        public ImageForCreationMappingsProfile()
        {
            // Map from ImageForCreation to Image
            // Ignore properties that shouldn't be mapped
            this.CreateMap<ImageForCreationModel, Image>()
                .ForMember(m => m.FileName, options => options.Ignore())
                .ForMember(m => m.Id, options => options.Ignore())
                .ForMember(m => m.OwnerId, options => options.Ignore());
        }
    }
}