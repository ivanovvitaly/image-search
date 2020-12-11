using AE.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AE.Services.Configuration 
{   
    public static class ImagesServiceServicesCollectionExtensions
    {
        public static void AddImagesService(this IServiceCollection services, IConfiguration configuration) 
        {
            services.Configure<ImagesServiceSettings>(configuration.GetSection(nameof(ImagesServiceSettings)));
            services.AddHttpClient<IImagesService, ImagesService>();
        }
    }
}