using System;
using System.ComponentModel.DataAnnotations;

namespace ImageGallery.MvcClient.ViewModels
{
    public class EditImageViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public Guid Id { get; set; }  
    }
}
