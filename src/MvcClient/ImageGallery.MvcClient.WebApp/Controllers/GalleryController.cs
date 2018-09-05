using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.MvcClient.Services;
using ImageGallery.MvcClient.ViewModels;
using ImageGallery.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace ImageGallery.MvcClient.WebApp.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;
        private readonly ILogger<GalleryController> _logger;

        public GalleryController(
            IImageGalleryHttpClient imageGalleryHttpClient,
            ILogger<GalleryController> logger)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

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

        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            _logger.LogInformation($"Identity token: {identityToken}");

            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        public async Task Logout()
        {
            // Clears the  local cookie ("Cookies" must match the name of the scheme)
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }
    }
}
