using System.Text.Json.Serialization;

namespace AE.Services.Dto
{
    public class Picture
    {
        public string Id { get; set; }

        [JsonPropertyName("cropped_picture")]
        public string CroppedPicture { get; set; }
    }
}