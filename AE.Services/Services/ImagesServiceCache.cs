using System.Collections.Generic;
using System.Linq;
using AE.Services.Dto;
using Microsoft.Extensions.Logging;

namespace AE.Services.Services
{
    public class ImagesServiceCache : IImagesServiceCache
    {
        private readonly ILogger<ImagesServiceCache> logger;
        private List<PictureDetail> cache = new List<PictureDetail>();
        
        public ImagesServiceCache(ILogger<ImagesServiceCache> logger)
        {
            this.logger = logger;
        }

        public void Clear()
        {
            logger.LogDebug("Clearing image cache.");
            this.cache.Clear();
        }

        public IEnumerable<PictureDetail> Get()
        {
            return cache;
        }

        public void Set(IEnumerable<PictureDetail> pictures)
        {
            logger.LogDebug("Updating images cache. {@Images}", pictures);
            cache = new List<PictureDetail>(pictures ?? Enumerable.Empty<PictureDetail>());
        }
    }
}