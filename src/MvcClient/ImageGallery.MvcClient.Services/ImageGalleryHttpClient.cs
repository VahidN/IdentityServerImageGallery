using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.MvcClient.Services
{
    public interface IImageGalleryHttpClient
    {
        Task<HttpClient> GetHttpClientAsync();
    }

    /// <summary>
    /// A typed HttpClient.
    /// </summary>
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ImageGalleryHttpClient> _logger;

        public ImageGalleryHttpClient(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ImageGalleryHttpClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<HttpClient> GetHttpClientAsync()
        {
            var currentContext = _httpContextAccessor.HttpContext;
            var accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogInformation($"Using Access Token: {accessToken}");
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.BaseAddress = new Uri(_configuration["WebApiBaseAddress"]);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }
    }
}