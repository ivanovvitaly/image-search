using System.Text.Json.Serialization;

namespace AE.Services.Dto
{
    public class PictureDetail : Picture
    {
        public string Author { get; set; }

        public string Camera { get; set; }
        
        public string Tags { get; set; }

        [JsonPropertyName("full_picture")]
        public string FullPicture { get; set; }
    }
}