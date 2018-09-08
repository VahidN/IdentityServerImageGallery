using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.WebApi.DataLayer.Context;
using ImageGallery.WebApi.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.WebApi.Services
{
    public interface IImagesService
    {
        Task<bool> ImageExistsAsync(Guid id);
        Task<Image> GetImageAsync(Guid id);
        Task<List<Image>> GetImagesAsync(string ownerId);
        Task<Image> AddImageAsync(Image image);
        Task UpdateImageAsync(Image image);
        Task DeleteImageAsync(Image image);
        Task<bool> IsImageOwnerAsync(Guid id, string ownerId);
    }

    public class ImagesService : IImagesService
    {

        private readonly IUnitOfWork _uow;
        private readonly DbSet<Image> _images;

        public ImagesService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _images = _uow.Set<Image>();
        }

        public Task<bool> ImageExistsAsync(Guid id)
        {
            return _images.AnyAsync(i => i.Id == id);
        }

        public Task<Image> GetImageAsync(Guid id)
        {
            return _images.FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<List<Image>> GetImagesAsync(string ownerId)
        {
            return _images.Where(image => image.OwnerId == ownerId).OrderBy(image => image.Title).ToListAsync();
        }

        public Task<bool> IsImageOwnerAsync(Guid id, string ownerId)
        {
            return _images.AnyAsync(i => i.Id == id && i.OwnerId == ownerId);
        }

        public async Task<Image> AddImageAsync(Image image)
        {
            var imageEntry = _images.Add(image);
            await _uow.SaveChangesAsync();
            return imageEntry.Entity;
        }

        public Task UpdateImageAsync(Image image)
        {
            return Task.CompletedTask;
        }

        public async Task DeleteImageAsync(Image image)
        {
            _images.Remove(image);
            await _uow.SaveChangesAsync();
        }
    }
}
