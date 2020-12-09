using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PollyTest.Services;

namespace PollyTest
{
    public static class ConfigureServices
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IImagesService, ImagesService>();
            services.Configure<ImagesServiceConfig>(configuration.GetSection("Images"));
        }
    }
}