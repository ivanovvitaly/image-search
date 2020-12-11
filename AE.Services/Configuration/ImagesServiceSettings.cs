using System;

namespace AE.Services.Configuration
{
    public class ImagesServiceSettings
    {
        public string ApiBaseUrl { get; set; }

        public string ApiKey { get; set; }

        public TimeSpan CacheDuration { get; set; }
    }
}
