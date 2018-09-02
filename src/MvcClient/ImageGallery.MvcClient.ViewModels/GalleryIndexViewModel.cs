using System.Collections.Generic;
using ImageGallery.WebApi.Models;

namespace ImageGallery.MvcClient.ViewModels
{
    public class GalleryIndexViewModel
    {
        public IEnumerable<ImageModel> Images { get; }

        public GalleryIndexViewModel(IEnumerable<ImageModel> images)
        {
            Images = images;
        }
    }
}
