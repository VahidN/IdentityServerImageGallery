using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ImageGallery.MvcClient.Services
{
    public interface IImageGalleryHttpClient
    {
        HttpClient HttpClient { get; }
    }

    /// <summary>
    /// A typed HttpClient.
    /// </summary>
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        public HttpClient HttpClient { get; }

        public ImageGalleryHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:7001/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient = httpClient;
        }
    }
}