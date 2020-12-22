using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AE.Services.Dto;
using System.Linq;
using System.Net.Http.Json;

namespace AE.Services.Services
{
    public class ImagesService : IImagesService
    {
        private readonly HttpClient _httpClient;
        private readonly IImagesServiceCache _imagesCache;
        private readonly ILogger<ImagesService> _logger;

        public ImagesService(
            HttpClient httpClient,
            IImagesServiceCache imagesCache,
            ILogger<ImagesService> logger)
        {
            _httpClient = httpClient;
            _imagesCache = imagesCache;
            _logger = logger;
        }

        public async Task<PagedPictures> GetImages(int? page)
        {
            _logger.LogDebug($"{nameof(GetImages)} {{Page}}", page);
            var imagesUrl = "/images";

            if (page != null)
            {
                imagesUrl += $"?page={page}";
            }

            var response = await _httpClient.GetAsync(imagesUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PagedPictures>();
        }

        public async Task<PictureDetail> GetImage(string id)
        {
            _logger.LogDebug($"{nameof(GetImage)} {{Id}}.", id);
            var response = await _httpClient.GetAsync($"/images/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<PictureDetail>();
            }

            return null;
        }

        public PictureDetail[] Search(string term)
        {
            return _imagesCache.Get().Where(picture =>
            {
                var searchableProperties = picture.GetType()
                                                  .GetProperties()
                                                  .Where(prop => prop.PropertyType == typeof(string));

                return searchableProperties.Any(prop => ((string)prop.GetValue(picture) ?? "").Contains(term, StringComparison.OrdinalIgnoreCase));

            }).ToArray();
        }
    }
}