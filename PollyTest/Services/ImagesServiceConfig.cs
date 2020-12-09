using System;

namespace PollyTest.Services
{
    public class ImagesServiceConfig
    {
        public string ApiBaseUrl { get; set; }

        public string ApiKey { get; set; }

        public TimeSpan CacheDuration { get; set; }
    }
}
