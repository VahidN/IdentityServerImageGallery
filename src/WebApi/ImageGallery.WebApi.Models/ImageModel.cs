using System;

namespace ImageGallery.WebApi.Models
{
    public class ImageModel
    {      
        public Guid Id { get; set; }
 
        public string Title { get; set; }
 
        public string FileName { get; set; }    
    }
}
