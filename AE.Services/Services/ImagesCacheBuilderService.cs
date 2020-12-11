using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AE.Services.Configuration;
using AE.Services.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace AE.Services.Services
{
    public class ImagesCacheBuilderService : BackgroundService
    {
        private readonly TimeSpan cacheDuration;
        private readonly IImagesService imagesService;
        private readonly IImagesServiceCache _imagesCache;
        private readonly ILogger<ImagesCacheBuilderService> logger;

        public ImagesCacheBuilderService(
            IOptions<ImagesServiceSettings> settings,
            IImagesService imagesService,
            IImagesServiceCache imagesCache,
            ILogger<ImagesCacheBuilderService> logger)
        {
            this.cacheDuration = TimeSpan.FromMinutes(Math.Max(ImagesServiceSettings.MinCacheDuration, settings.Value.CacheDuration));
            this.imagesService = imagesService;
            this._imagesCache = imagesCache;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await CacheImages(stoppingToken);
                await Task.Delay(cacheDuration);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Service Stopped.");            
            _imagesCache.Clear();

            return Task.CompletedTask;
        }

        private async Task CacheImages(CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("CacheStart", DateTime.Now))
            {
                logger.LogInformation($"Start images caching. Duration = {cacheDuration}.");

                var page = 0;
                var hasMore = false;
                var imageList = new List<PictureDetail>();

                do
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var pagedPictures = await imagesService.GetImages(++page);
                    logger.LogDebug($"Page {page} Pictures = {{@Pictures}}", pagedPictures);
                    hasMore = pagedPictures.HasMore;

                    var images = await Task.WhenAll(pagedPictures.Pictures.Select(p => imagesService.GetImage(p.Id)));
                    imageList.AddRange(images);
                }
                while (hasMore);

                _imagesCache.Set(imageList);

                logger.LogInformation("End images caching.");
            }
        }
    }
}