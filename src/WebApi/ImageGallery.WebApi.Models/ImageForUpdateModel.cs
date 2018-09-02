using System.ComponentModel.DataAnnotations;

namespace ImageGallery.WebApi.Models
{
    public class ImageForUpdateModel
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }      
    }
}
