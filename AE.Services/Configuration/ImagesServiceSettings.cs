using System;

namespace AE.Services.Configuration
{
    public class ImagesServiceSettings
    {
        public string ApiBaseUrl { get; set; }

        public string ApiKey { get; set; }

        public int CacheDuration { get; set; }

        public static int MinCacheDuration { get; } = 1;
    }
}
