using AutoMapper;
using ImageGallery.WebApi.DomainClasses;
using ImageGallery.WebApi.Models;

namespace ImageGallery.WebApi.Mappings
{
    public class ImageMappingsProfile : Profile
    {
        public ImageMappingsProfile()
        {
            // Map from Image (entity) to Image, and back
            this.CreateMap<Image, ImageModel>().ReverseMap();
        }
    }
}