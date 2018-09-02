using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.MvcClient.Services;
using ImageGallery.MvcClient.ViewModels;
using ImageGallery.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ImageGallery.MvcClient.WebApp.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        public GalleryController(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _imageGalleryHttpClient.HttpClient.GetAsync("api/images");
            response.EnsureSuccessStatusCode();

            var imagesAsString = await response.Content.ReadAsStringAsync();
            var galleryIndexViewModel = new GalleryIndexViewModel(
                JsonConvert.DeserializeObject<IList<ImageModel>>(imagesAsString).ToList());
            return View(galleryIndexViewModel);
        }

        public async Task<IActionResult> EditImage(Guid id)
        {
            var response = await _imageGalleryHttpClient.HttpClient.GetAsync($"api/images/{id}");
            response.EnsureSuccessStatusCode();

            var imageAsString = await response.Content.ReadAsStringAsync();
            var deserializedImage = JsonConvert.DeserializeObject<ImageModel>(imageAsString);
            var editImageViewModel = new EditImageViewModel
            {
                Id = deserializedImage.Id,
                Title = deserializedImage.Title
            };
            return View(editImageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var imageForUpdate = new ImageForUpdateModel { Title = editImageViewModel.Title };
            var serializedImageForUpdate = JsonConvert.SerializeObject(imageForUpdate);
            var response = await _imageGalleryHttpClient.HttpClient.PutAsync(
                $"api/images/{editImageViewModel.Id}",
                new StringContent(serializedImageForUpdate, System.Text.Encoding.Unicode, "application/json"));
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var response = await _imageGalleryHttpClient.HttpClient.DeleteAsync($"api/images/{id}");
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddImageViewModel addImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var imageForCreation = new ImageForCreationModel { Title = addImageViewModel.Title };
            var imageFile = addImageViewModel.Files.First();
            if (imageFile.Length > 0)
            {
                using (var fileStream = imageFile.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();
                }
            }

            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);
            var response = await _imageGalleryHttpClient.HttpClient.PostAsync(
                $"api/images",
                new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"));
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }
    }
}
