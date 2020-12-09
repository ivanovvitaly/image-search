using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PollyTest.Dto
{
    public class PictureDetail
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Camera { get; set; }
        public string Tags { get; set; }

        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string CroppedPicture { get; set; }

        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string FullPicture { get; set; }
    }
}