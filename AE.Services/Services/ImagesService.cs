using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using AE.Services.Dto;
using AE.Services.Configuration;
using System.Linq;
using System.Net.Http.Json;

namespace AE.Services.Services
{
    public class ImagesService : IImagesService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ImagesServiceSettings> _serviceSettings;
        private readonly IImagesServiceCache _imagesCache;
        private readonly ILogger<ImagesService> _logger;
        private string _token;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _authenticationPolicy;

        public ImagesService(
            HttpClient httpClient,
            IOptions<ImagesServiceSettings> serviceSettings,
            IImagesServiceCache imagesCache,
            ILogger<ImagesService> logger)
        {
            _httpClient = httpClient;
            _serviceSettings = serviceSettings;
            _imagesCache = imagesCache;
            _httpClient.BaseAddress = new Uri(_serviceSettings.Value.ApiBaseUrl);

            _logger = logger;

            _authenticationPolicy = Policy
                  .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                  .RetryAsync(1, async (result, retryCount, context) =>
                  {
                      _logger.LogWarning($"Received: {{Error}}. Performing Authentication Retry({retryCount}).", result.Result.ReasonPhrase);
                      await CreateAccessToken();
                  });
        }

        public async Task<PagedPictures> GetImages(int? page)
        {
            _logger.LogDebug($"{nameof(GetImages)} {{Page}}", page);
            var imagesUrl = "/images";

            if (page != null)
            {
                imagesUrl += $"?page={page}";
            }

            var response = await _authenticationPolicy.ExecuteAsync(() => Http.GetAsync(imagesUrl));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PagedPictures>();
        }

        public async Task<PictureDetail> GetImage(string id)
        {
            _logger.LogDebug($"{nameof(GetImage)} {{Id}}.", id);
            var response = await _authenticationPolicy.ExecuteAsync(() => Http.GetAsync($"/images/{id}"));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<PictureDetail>();
            }

            return null;
        }

        public async Task<string> CreateAccessToken()
        {
            var response = await _httpClient.PostAsJsonAsync("/auth", new TokenRequest(_serviceSettings.Value.ApiKey));
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _token = tokenResponse.Token;
            return _token;
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

        private HttpClient Http
        {
            get
            {
                if (!string.IsNullOrEmpty(_token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }

                return _httpClient;
            }
        }


        private record TokenResponse(string Token);

        private record TokenRequest(string ApiKey);
    }
}