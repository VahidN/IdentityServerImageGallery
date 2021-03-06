﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.MvcClient.Services;
using ImageGallery.MvcClient.ViewModels;
using ImageGallery.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace ImageGallery.MvcClient.WebApp.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;
        private readonly IConfiguration _configuration;

        public GalleryController(
            IImageGalleryHttpClient imageGalleryHttpClient,
            IConfiguration configuration)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = await _imageGalleryHttpClient.GetHttpClientAsync();
            var response = await httpClient.GetAsync("api/images");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }
            response.EnsureSuccessStatusCode();

            var imagesAsString = await response.Content.ReadAsStringAsync();
            var galleryIndexViewModel = new GalleryIndexViewModel(
                JsonConvert.DeserializeObject<IList<ImageModel>>(imagesAsString).ToList());
            return View(galleryIndexViewModel);
        }

        public async Task<IActionResult> EditImage(Guid id)
        {
            var httpClient = await _imageGalleryHttpClient.GetHttpClientAsync();
            var response = await httpClient.GetAsync($"api/images/{id}");
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
            var httpClient = await _imageGalleryHttpClient.GetHttpClientAsync();
            var response = await httpClient.PutAsync(
                $"api/images/{editImageViewModel.Id}",
                new StringContent(serializedImageForUpdate, System.Text.Encoding.Unicode, "application/json"));
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var httpClient = await _imageGalleryHttpClient.GetHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/images/{id}");
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "PayingUser")]
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "PayingUser")]
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
            var httpClient = await _imageGalleryHttpClient.GetHttpClientAsync();
            var response = await httpClient.PostAsync(
                $"api/images",
                new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"));
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            ViewBag.IdentityToken = identityToken;
            return View();
        }

        public async Task Logout()
        {
            await revokeTokens();
            // Clears the  local cookie ("Cookies" must match the name of the scheme)
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        private async Task revokeTokens()
        {
            var discoveryClient = new DiscoveryClient(_configuration["IDPBaseAddress"]);
            var metaDataResponse = await discoveryClient.GetAsync();
            var tokenRevocationClient = new TokenRevocationClient(
                metaDataResponse.RevocationEndpoint,
                _configuration["ClientId"],
                _configuration["ClientSecret"]
            );

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var response = await tokenRevocationClient.RevokeAccessTokenAsync(accessToken);
                if (response.IsError)
                {
                    throw new Exception("Problem accessing the TokenRevocation endpoint.", response.Exception);
                }
            }

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var response = await tokenRevocationClient.RevokeRefreshTokenAsync(refreshToken);
                if (response.IsError)
                {
                    throw new Exception("Problem accessing the TokenRevocation endpoint.", response.Exception);
                }
            }
        }

        [Authorize(Policy = "CanOrderFrame")]
        public async Task<IActionResult> OrderFrame()
        {
            var discoveryClient = new DiscoveryClient(_configuration["IDPBaseAddress"]);
            var metaDataResponse = await discoveryClient.GetAsync();

            var userInfoClient = new UserInfoClient(metaDataResponse.UserInfoEndpoint);

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await userInfoClient.GetAsync(accessToken);
            if (response.IsError)
            {
                throw new Exception("Problem accessing the UserInfo endpoint.", response.Exception);
            }

            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            return View(new OrderFrameViewModel(address));
        }
    }
}
