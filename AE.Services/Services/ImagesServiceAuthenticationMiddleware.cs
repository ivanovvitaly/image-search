using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using AE.Services.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace AE.Services.Services
{
    public class ImagesServiceAuthenticationMiddleware : DelegatingHandler
    {
        private readonly IImageServiceTokenStorage tokenStorage;
        private readonly ILogger<ImagesServiceAuthenticationMiddleware> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptions<ImagesServiceSettings> settings;

        public ImagesServiceAuthenticationMiddleware(
            IImageServiceTokenStorage tokenStorage,
            ILogger<ImagesServiceAuthenticationMiddleware> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<ImagesServiceSettings> settings)
        {
            this.logger = logger;
            this.tokenStorage = tokenStorage;
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var retry = Policy
                .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (result, retryCount, context) =>
                {
                    logger.LogWarning($"Received: {{Error}}. Performing Authentication Retry({retryCount}).", result.Result.ReasonPhrase);
                    await CreateAccessToken();
                });
                
            return await retry.ExecuteAsync(
                () =>
                {
                    if (!string.IsNullOrEmpty(this.tokenStorage.Token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.tokenStorage.Token);
                    }

                    return base.SendAsync(request, cancellationToken);
                });
        }

        private async Task<string> CreateAccessToken()
        {
            var response = await httpClientFactory
                .CreateClient(ImagesServiceConstants.HttpClientKey)
                .PostAsJsonAsync("/auth", new TokenRequest(settings.Value.ApiKey));
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            tokenStorage.Token = tokenResponse.Token;
            return tokenStorage.Token;
        }

        private record TokenResponse(string Token);

        private record TokenRequest(string ApiKey);
    }    
}