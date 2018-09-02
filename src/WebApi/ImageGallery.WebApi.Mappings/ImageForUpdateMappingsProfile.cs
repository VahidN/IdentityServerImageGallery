using AutoMapper;
using ImageGallery.WebApi.DomainClasses;
using ImageGallery.WebApi.Models;

namespace ImageGallery.WebApi.Mappings
{
    public class ImageForUpdateMappingsProfile : Profile
    {
        public ImageForUpdateMappingsProfile()
        {
            // Map from ImageForUpdate to Image
            // ignore properties that shouldn't be mapped
            this.CreateMap<ImageForUpdateModel, Image>()
                .ForMember(m => m.FileName, options => options.Ignore())
                .ForMember(m => m.Id, options => options.Ignore())
                .ForMember(m => m.OwnerId, options => options.Ignore());
        }
    }
}