using System;

namespace PollyTest
{
    public class ImagesServiceConfig
    {
        public string ApiBaseUrl { get; set; }

        public string ApiKey { get; set; }

        public TimeSpan CacheDuration { get; set; }
    }
}
