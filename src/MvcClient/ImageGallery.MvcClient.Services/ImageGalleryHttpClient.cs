using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
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
            var accessToken = string.Empty;

            var currentContext = _httpContextAccessor.HttpContext;
            var expires_at = await currentContext.GetTokenAsync("expires_at");
            if (string.IsNullOrWhiteSpace(expires_at)
                || ((DateTime.Parse(expires_at).AddSeconds(-60)).ToUniversalTime() < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

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

        private async Task<string> RenewTokens()
        {
            // get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // get the metadata
            var discoveryClient = new DiscoveryClient(_configuration["IDPBaseAddress"]);
            var metaDataResponse = await discoveryClient.GetAsync();

            // create a new token client to get new tokens
            var tokenClient = new TokenClient(
                metaDataResponse.TokenEndpoint,
                _configuration["ClientId"],
                _configuration["ClientSecret"]);

            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);
            if (tokenResult.IsError)
            {
                throw new Exception("Problem encountered while refreshing tokens.", tokenResult.Exception);
            }

            // update the tokens & expiration value
            var updatedTokens = new List<AuthenticationToken>();
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = tokenResult.IdentityToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = tokenResult.AccessToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = tokenResult.RefreshToken
            });

            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
            });

            // get authenticate result, containing the current principal & properties
            var currentAuthenticateResult = await currentContext.AuthenticateAsync("Cookies");

            // store the updated tokens
            currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

            // sign in
            await currentContext.SignInAsync("Cookies",
             currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

            // return the new access token
            return tokenResult.AccessToken;
        }
    }
}