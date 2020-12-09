using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using PollyTest.Dto;

namespace PollyTest.Services
{
    public class ImagesService : IImagesService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ImagesServiceConfig> _config;
        private readonly ILogger<ImagesService> _logger;
        private string _token = null;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryWithReauthorizationPolicy;

        public ImagesService(
            HttpClient httpClient,
            IOptions<ImagesServiceConfig> config,
            ILogger<ImagesService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config.Value.ApiBaseUrl);

            _logger = logger;

            _retryWithReauthorizationPolicy = Policy
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

            var response = await _retryWithReauthorizationPolicy.ExecuteAsync(() => Http.GetAsync(imagesUrl));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PagedPictures>();
        }

        public async Task<PictureDetail> GetImage(string id)
        {
            _logger.LogDebug($"{nameof(GetImage)} {{Id}}.", id);
            var response = await _retryWithReauthorizationPolicy.ExecuteAsync(() => Http.GetAsync($"/images/{id}"));

            return response.StatusCode == HttpStatusCode.OK
                ? await response.Content.ReadFromJsonAsync<PictureDetail>()
                : null;
        }

        public async Task<string> CreateAccessToken()
        {
            var response = await _httpClient.PostAsJsonAsync("/auth", new TokenRequest(_config.Value.ApiKey));
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _token = tokenResponse.Token;
            return _token;
        }

        private HttpClient Http
        {
            get
            {
                if (!string.IsNullOrEmpty(_token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        _token);
                }

                return _httpClient;
            }
        }


        private record TokenResponse(string Token);

        private record TokenRequest(string ApiKey);
    }
}