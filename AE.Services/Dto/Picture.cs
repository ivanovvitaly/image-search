using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AE.Services.Dto
{
    public class Picture
    {
        public string Id { get; set; }
        
        [JsonProperty("cropped_picture")]
        public string CroppedPicture { get; set; }
    }
}