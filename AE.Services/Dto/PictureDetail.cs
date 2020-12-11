using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AE.Services.Dto
{
    public class PictureDetail
    {
        public string Id { get; set; }

        public string Author { get; set; }

        public string Camera { get; set; }
        
        public string Tags { get; set; }

        [JsonProperty("cropped_picture")]
        public string CroppedPicture { get; set; }

        [JsonProperty("full_picture")]
        public string FullPicture { get; set; }
    }
}