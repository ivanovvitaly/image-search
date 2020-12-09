using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PollyTest.Dto
{
    public class Picture
    {
        public string Id { get; set; }
        
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string CroppedPicture { get; set; }
    }
}