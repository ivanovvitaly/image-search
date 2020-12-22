using System;
using AE.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AE.Services.Configuration
{
    public static class ImagesServiceServicesCollectionExtensions
    {
        public static void AddImagesService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ImagesServiceSettings>(configuration.GetSection(nameof(ImagesServiceSettings)));

            services.AddTransient<ImagesServiceAuthenticationMiddleware>();
            services.AddSingleton<IImageServiceTokenStorage, ImageServiceTokenStorage>();
            services.AddSingleton<IImagesServiceCache, ImagesServiceCache>();
            services
                .AddHttpClient<IImagesService, ImagesService>(ImagesServiceConstants.HttpClientKey, 
                    (provider, httpClient) =>
                    {
                        var settings = provider.GetService<IOptions<ImagesServiceSettings>>();
                        httpClient.BaseAddress = new Uri(settings.Value.ApiBaseUrl);
                    })
                .AddHttpMessageHandler<ImagesServiceAuthenticationMiddleware>();
        }

        public static void AddImagesCaching(this IServiceCollection services)
        {
            services.AddHostedService<ImagesCacheBuilderService>();
        }
    }
}